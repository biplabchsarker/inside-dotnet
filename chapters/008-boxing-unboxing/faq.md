# FAQ — Boxing & Unboxing

**Q: Is boxing "always slow"?**
A: Not as a blanket statement. Boxing allocates through the same fast bump-pointer path as any other object (Episode 8), so the cost is real but bounded — and this chapter measured a case where a boxed struct used only for same-method interface dispatch cost **zero bytes**, because the JIT's escape analysis proved the box never left the method and removed the allocation. The honest answer is: it depends on whether the box escapes, and that's measurable, not a matter of opinion.

**Q: Why does unboxing an `int` box as a `long` throw, when `int` implicitly converts to `long` everywhere else in C#?**
A: Because unboxing isn't a conversion — it's a type check against the box's exact method table pointer, followed by a copy. `unbox` never consults C#'s implicit numeric conversion rules; it asks "is this box literally holding an `int`," and an `int` box fails that check against a `long` request immediately, throwing `InvalidCastException`.

**Q: Does calling a member on a struct through an interface-typed variable box it on every call?**
A: No. The box happens exactly once — at the point the struct is assigned to the interface-typed variable. Every subsequent call through that same variable is an ordinary virtual dispatch against the one existing box.

**Q: Why doesn't a generic method with an interface constraint box its value-typed argument?**
A: The compiler emits a `constrained.` prefix before the interface call. Because generic code is specialized per instantiation, the JIT knows at that call site whether the type argument is a value type, and if that type implements the interface member itself, it calls it directly against the value's address instead of boxing first.

**Q: What actually happens when you box a `Nullable<int>`?**
A: It's special-cased. If `HasValue` is `false`, boxing produces a plain `null` reference — no allocation at all. If `HasValue` is `true`, boxing produces an ordinary box of the underlying `int`, with no trace of `Nullable<T>` remaining.

**Q: Should I still avoid `ArrayList`/`Hashtable` even though the JIT can sometimes eliminate boxing?**
A: Yes — the escape-analysis elimination measured in this chapter only applies when the JIT can prove a box never leaves its creating method. `ArrayList.Add(x)` stores the box in the list's backing array, which is a textbook escape — the allocation is real and measured at 8x the memory of the `List<int>` equivalent in this chapter's benchmark. There's no scenario in modern .NET where a non-generic collection over value types is the better default.

**Q: Does an interpolated string like `$"{someInt}"` box `someInt`?**
A: Since C# 10, usually not. An interpolated string compiles against `DefaultInterpolatedStringHandler` when the target accepts it — including a direct `Console.WriteLine(string)` call — and the handler's generic `AppendFormatted<T>(T value)` path formats the value without boxing. The classic `string.Format("{0}", someInt)` or `Console.WriteLine("{0}", someInt)` calls, which go through a `params object[]` overload, do box every value-typed argument.
