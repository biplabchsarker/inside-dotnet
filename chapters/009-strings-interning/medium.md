# Inside .NET — Episode 10: Strings & Interning

*Part II — Memory*

Strings are reference types that behave like value types — immutable, compared by content, not identity. That mismatch is exactly where the interesting bugs live. This chapter is the full mechanism behind the intern pool, plus real numbers on what leaning on it actually costs — including a case where it costs you 16 MB you'll never get back.

## Literals intern automatically. Nothing else does.

Every string literal compiles to an `ldstr` IL instruction. The first time the CLR sees a given literal's exact text, it allocates and pools it; every later `ldstr` with the same text — anywhere in the process — gets the same reference back. `const` expressions fold to a single literal at compile time, so they intern too. But the moment a string is built at runtime — `string.Concat`, `StringBuilder.ToString()`, `string.Format`, interpolation with a variable — none of that happens. It's a brand-new heap object, full stop, even if the content is byte-for-byte identical to something already interned.

## `Equals` checks reference identity before it checks a single character

This is the detail most explanations skip: `string.Equals` — which both `==` and `.Equals()` route through — starts with `ReferenceEquals`. Only if that fails does it compare lengths, then walk the buffers. I measured this directly: comparing a 500-character string to *itself*, a million times, took 309.7 µs. Comparing it to a *different* object holding identical text took 15,985.4 µs — **51.7x slower**, same answer both times.

| Method | Mean | Ratio |
|---|---|---|
| Equals against same reference | 309.7 µs | 1.00 |
| Equals against equal-but-different reference | 15,985.4 µs | 51.74x |

## The intern pool is a trade-off, not a free win

Here's the number that actually changed how I think about `string.Intern`. I built 200,000 unique strings, discarded them, forced a full GC collection: heap returned to 56,544 bytes, essentially baseline. Then I built 200,000 *different* unique strings, interned every single one, and forced a full collection again: **16,157,680 bytes — over 16 MB — did not come back.**

| Scenario | Heap after collection |
|---|---|
| Baseline | 57,120 B |
| Build + discard 200K unique strings | 56,544 B |
| Intern 200K unique strings | 16,157,680 B |

The intern pool is a GC root. It never releases anything, by design — which is exactly right for a small, bounded vocabulary of recurring literals, and exactly wrong for interning unbounded content: user input, request IDs, arbitrary external data. That's not a performance optimization anymore. It's a leak with a delay on it.

## Culture-aware comparison: slower, and sometimes just wrong

`StringComparison.CurrentCulture` measured **~25x slower** than `Ordinal` in this chapter's benchmark for a 55-character identifier-like string. But the real reason to default to `Ordinal` isn't the speed — it's that culture-aware case folding can give you a different answer depending on where your process happens to be running. Under the Turkish culture, `"I".ToLower()` produces `"ı"` — dotless i — not the ASCII `"i"` an ordinal comparison would expect. A case-insensitive check that's correct in one locale can silently fail in another. Microsoft's own analyzer rules (CA1309/CA1310) exist specifically to catch this.

## `Substring` has a fast path worth knowing precisely

`s.Substring(0, s.Length)` — the whole string, requested back — returns `s` itself. Zero allocation, confirmed both by `ReferenceEquals` in the demo and by a dedicated benchmark showing zero bytes allocated. Any other range allocates a real, independent copy — 152 bytes for a 64-character source string in my measurement, matching the same 24-byte header math from Episode 8, applied to a different type.

## Try it yourself

The [companion demo](code/Chapter09.Demo/Program.cs) walks through literal interning, `string.Intern`/`IsInterned`'s exact semantics, the Substring fast path, the Turkish-I gotcha, and the memory-retention proof above with real printed output. The [benchmarks project](code/Chapter09.Benchmarks/Program.cs) is the source of every timing number in this article — run it yourself with `dotnet run -c Release`.

*Next: [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/article.md).*
