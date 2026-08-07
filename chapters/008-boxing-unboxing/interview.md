# Interview Questions — Boxing & Unboxing

**Q1: What actually happens, mechanically, when a value type is boxed?**
A: The `box` IL instruction computes the size needed (the value's size plus a standard object header), allocates that many bytes through the exact same fast-path allocator described in Episode 8 (a per-thread allocation-context bump, no lock), writes an object header — a method table pointer identifying the value's exact runtime type, plus a sync block index — and copies the value's bits into the space after the header. It returns a new object reference. The only thing distinguishing this from a plain `new SomeClass()` is that the "constructor" is a full-value memory copy instead of user code.

**Q2: Why does unboxing to a "compatible" type — say, unboxing an `int` box as a `long` — throw, instead of just converting?**
A: Unboxing isn't a conversion; it's a type check followed by a copy. The `unbox` IL instruction compares the box's method table pointer against the *exact* type token requested and produces an interior pointer into the box only on an exact match. There's no implicit-numeric-conversion step being consulted — the CLR isn't asking "can a `long` be built from this," it's asking "is this box literally holding an `int`," and a `long` request against an `int` box fails that check immediately, producing `InvalidCastException`.

**Q3: Does calling a member through an interface-typed variable holding a struct box on every call?**
A: No — only once, at the point the struct is assigned to the interface-typed variable (that assignment is where the actual `box` happens). Every subsequent call through that same variable is an ordinary virtual dispatch against the one box's method table; it doesn't create a new box per call.

**Q4: Why doesn't a generic method with an interface constraint (`where T : ISomeInterface`) box `T` when `T` is a value type?**
A: The compiler emits a `constrained.` prefix before the interface call. Since the JIT specializes per generic instantiation, it knows at that call site whether `T` is a value type, and if that value type implements the interface member itself (rather than inheriting an unmodified implementation from `object`), the `constrained.` prefix lets the JIT call it directly against the value's address — no box is created for that instantiation.

**Q5: What happens when you box a `Nullable<T>`?**
A: It's special-cased. If `HasValue` is `false`, boxing produces a plain `null` reference — no allocation, and no trace of `Nullable<T>` at all. If `HasValue` is `true`, boxing produces an ordinary box of the *underlying* `T`, again with no `Nullable<T>` wrapper visible in the box. This is why `((object)someNullableInt) is int` succeeds for a non-null `int?`, and why a boxed null `int?` really does compare equal to `null`.

**Q6: This chapter measured a boxing scenario with zero allocated bytes. How is that possible, and is it something to rely on?**
A: Modern RyuJIT performs escape analysis and can stack-allocate — or eliminate entirely — an object (including a box) it can prove never escapes the method that creates it. In the measured case, a boxed struct was used only for same-method interface dispatch, never stored, returned, or placed in a collection, so the JIT removed the heap allocation. It's real and reproducible for that exact code shape, but it depends on the JIT's ability to prove non-escape, which is sensitive to how the value is used — not a documented guarantee to design code around.
