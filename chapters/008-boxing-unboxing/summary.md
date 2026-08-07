# Summary — Boxing & Unboxing

**TL;DR:** Boxing allocates a heap object through the exact same fast path as Episode 8 (header write + a full copy of the value's bits), so a value type can participate in APIs built around a uniform object reference — non-generic collections, `object` parameters, interface dispatch. Unboxing is an exact-type check, not a conversion — `(long)someBoxedInt` throws even though `int` widens to `long` everywhere else in C#. A generic constraint avoids the box entirely via `constrained.` calls. And boxing's real cost depends on whether the box escapes the method that creates it — measured directly in this chapter, an escaping box costs 24 bytes and ~6x the time of staying unboxed, while a provably non-escaping one can measure at zero.

**Key takeaways:**

- Boxing rides Episode 8's allocator unmodified -- a header write plus a full-value memcpy is the only difference from a plain `new`.
- Unboxing checks the box's method table for an EXACT type match and produces an interior pointer on success -- it is not a numeric conversion, which is why `(long)boxedInt` throws instead of widening.
- Boxing shows up without the word "box" nearby: `ArrayList`/`Hashtable`, `object`/`params object[]` parameters, and assigning a struct to an interface-typed variable are the three sites worth recognizing on sight.
- A struct assigned to an interface-typed variable boxes once, at that assignment -- later calls through the same variable dispatch against the existing box, they don't re-box.
- A generic method constrained to an interface avoids boxing a value-typed `T` via the `constrained.` IL prefix, which lets the JIT call the value type's own implementation directly.
- Measured with `BenchmarkDotNet`: a plain box/unbox round trip is ~6x slower and allocates 24 bytes per box; `ArrayList` vs `List<int>` is ~4.4x slower and allocates 8x more -- but a boxed struct used only for same-method dispatch measured at **zero allocated bytes**, because modern RyuJIT's escape analysis proved the box never leaves the method and removed it.
- `Nullable<T>` boxing is special-cased: `HasValue == false` boxes to `null` (no allocation); `HasValue == true` boxes to a plain box of the underlying type, with no `Nullable<T>` wrapper visible afterward.
