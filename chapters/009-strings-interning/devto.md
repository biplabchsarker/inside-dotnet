---
title: Inside .NET — Episode 10: Strings & Interning
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md).*

```csharp
long baseline = ForceFullCollectionAndMeasure();
// build + discard 200,000 unique strings → collect
long afterDiscarded = ForceFullCollectionAndMeasure();   // ~56,544 B — back near baseline
// intern 200,000 DIFFERENT unique strings → collect
long afterInterned = ForceFullCollectionAndMeasure();    // 16,157,680 B — never comes back
```

Interning "to speed things up" quietly turned into a 16 MB memory leak, measured, not assumed. This chapter is the full mechanism behind why, plus the two other measured surprises that come with it.

## Literals intern automatically. Runtime strings never do.

Every string literal compiles to `ldstr`; the CLR pools the first occurrence of any given text and hands back that same reference every time it sees that exact text again. `const` expressions fold to a literal at compile time, so they intern too. But `string.Concat`, `StringBuilder.ToString()`, `string.Format`, and interpolation with a runtime value never touch the pool automatically — even when the result matches something already interned, character for character.

## `Equals` checks `ReferenceEquals` first — measured at 51.7x

```csharp
// 1,000,000 iterations each
Original.Equals(SameReference);                 // 309.7 µs  — reference check short-circuits
Original.Equals(EqualButDifferentReference);     // 15,985.4 µs — full 500-char scan
```

Same result both times (`true`). The cost difference is the reference-equality fast path baked into `string.Equals`, confirmed directly rather than assumed.

## The intern pool never releases anything — that's the whole risk

| Scenario | Heap after a forced full collection |
|---|---|
| Baseline | 57,120 B |
| Build + discard 200K unique strings | 56,544 B |
| Intern 200K unique strings | **16,157,680 B** |

Interning is exactly right for a small, bounded, recurring vocabulary — and a genuine, silent memory leak the moment the content is unbounded (user input, request IDs, anything external). The pool is a permanent GC root; nothing you intern is ever coming back.

## Culture-aware comparison: ~25x slower, and sometimes just wrong

```csharp
"I".ToLower(CultureInfo.GetCultureInfo("tr-TR")); // "ı" — not the ASCII "i" ordinal comparison expects
```

Beyond the measured ~25x slowdown vs. `Ordinal` in this chapter's benchmark, culture-aware comparison can produce a genuinely different answer depending on locale. Default to `Ordinal`/`OrdinalIgnoreCase` for anything that isn't user-facing text — Microsoft's own CA1309/CA1310 analyzer rules exist to catch exactly this.

## Try it yourself

```bash
cd chapters/009-strings-interning/code/Chapter09.Demo
dotnet run
```

The [demo](code/Chapter09.Demo/Program.cs) proves literal interning, `string.Intern`/`IsInterned`'s exact semantics, the `Substring` self-reference fast path, and the memory-retention numbers above with real output. The [benchmarks project](code/Chapter09.Benchmarks/Program.cs) backs every timing number here — `dotnet run -c Release` reproduces them on your own machine.

*Next: [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/article.md).*
