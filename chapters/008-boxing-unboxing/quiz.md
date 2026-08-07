# Quiz — Boxing & Unboxing

1. What IL instruction performs boxing, and what does it write into the newly allocated block, in what order?
2. Why does unboxing an `int` box as a `long` throw `InvalidCastException` instead of performing a widening conversion?
3. Does assigning a struct to an interface-typed variable box it once per assignment, or once per subsequent call through that variable?
4. What IL mechanism lets a generic method with an interface constraint avoid boxing a value-typed `T`?
5. What does boxing a `Nullable<int>` actually produce when `HasValue` is `true`, versus when it's `false`?

<details>
<summary>Answers</summary>

1. The `box` instruction. It allocates a block sized for the object header plus the value's own size (through the same fast-path allocator as Episode 8), writes the object header first (method table pointer, then sync block index), then copies the value's bits into the remaining space.
2. Because unboxing is a type check against the box's *exact* method table, not a conversion — `unbox` never consults C#'s implicit numeric conversion rules. An `int` box's method table simply doesn't match a `long` type token, so the check fails and `InvalidCastException` is thrown immediately.
3. Once per assignment. The box happens exactly at the point a struct is stored into an interface- or object-typed variable; every subsequent call through that same variable dispatches against the one existing box.
4. The `constrained.` IL prefix before the interface `callvirt`. Because the JIT specializes generic code per instantiation, it knows at that call site whether the type argument is a value type, and if so, can call the value type's own interface implementation directly against its address instead of boxing it first.
5. When `HasValue` is `true`, boxing produces an ordinary box of the underlying `int` — no trace of `Nullable<T>` remains. When `HasValue` is `false`, boxing produces a plain `null` reference, with no allocation at all.

</details>
