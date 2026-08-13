# Chapter 009 — Code Samples

Two projects, matching the Example/Advanced/Performance tiers described in `article.md`.

## `Chapter09.Demo` (Example + Advanced)

Demonstrates:
- Literal interning — identical string literals are the same heap object (`ReferenceEquals` proves it)
- `const` expressions fold into a single literal at compile time, so they intern too
- Runtime-constructed strings (concatenation, `StringBuilder.ToString()`) are never auto-interned, even with matching content
- `string.Intern`'s and `string.IsInterned`'s exact return semantics
- `Substring`'s self-reference fast path (`Substring(0, s.Length)` returns the original object) versus the ordinary allocating case
- `new string(char[])` is never auto-interned
- The Turkish-I culture-comparison gotcha, made concrete
- A measured proof, via `GC.GetTotalMemory(true)`, that interning unbounded runtime content leaves memory permanently unreclaimable while discarding the same content lets the GC reclaim it normally

```bash
cd Chapter09.Demo
dotnet run
```

### Expected output

Exact byte counts may vary slightly by machine/runtime patch, but the shape is stable:

```
=== Inside .NET: Episode 10 — Strings & Interning demo ===

--- 1. Literal interning: identical literals are the same object ---
  literalA equals literalB (value)?     True
  literalA is literalB (reference)?     True

--- 2. `const` expressions fold at compile time, so they intern too ---
  constGreeting is "Hello, World" (reference)? True

--- 3. Runtime-constructed strings are NOT auto-interned ---
  runtimeConcat value:                    "Hello, Quokka7Xk"
  runtimeConcat equals the literal?        True
  runtimeConcat is the literal (ref)?      False
  runtimeBuilder is runtimeConcat (ref)?   False

--- 4. string.Intern and string.IsInterned ---
  IsInterned(neverSeenBefore) before interning: null (not interned)
  After string.Intern(neverSeenBefore):
    internedResult is neverSeenBefore (ref)?      True
    IsInterned(neverSeenBefore) now finds it?     True
    secondEqualRuntimeString is neverSeenBefore before interning it (ref)? False
    After interning it too, both intern results share one object (ref)?   True

--- 5. Substring's own-reference fast path ---
  original.Substring(0, original.Length) is original (ref)? True
  original.Substring(4, 5) is original (ref)?               False  -- value: "quick"

--- 6. `new string(char[])` is never auto-interned ---
  fromCharArray equals the literal (value)? True
  fromCharArray is the literal (reference)? False

--- 7. Culture-aware comparison is a real correctness risk, not just interning trivia ---
  "TITLE" vs "title", StringComparison.OrdinalIgnoreCase:        True
  Both lowered under tr-TR then compared ordinally:               False
  Both lowered invariantly then compared ordinally:               True
  "I".ToLower(tr-TR) = "ı" (not the ASCII 'i' you'd expect from an invariant/ordinal comparison)

--- 8. `==` is value equality (via string.Equals), not reference equality ---
  valueA == valueB (operator, value-based)? True
  ReferenceEquals(valueA, valueB)?          False

--- 9. Interning unbounded runtime content is a measurable memory-growth risk ---
  Baseline heap after a full collection:                          57,120 bytes
  After creating+discarding 200,000 unique strings, then collecting: 56,544 bytes (returns near baseline — ordinary garbage)
  After interning 200,000 unique strings, then collecting:            16,157,680 bytes (does NOT return to baseline — the pool holds every one of them)

=== Done ===
```

## `Chapter09.Benchmarks` (Performance)

Real `BenchmarkDotNet` numbers backing this chapter's Performance Notes claims — no estimates, no "should be roughly."

```bash
cd Chapter09.Benchmarks
dotnet run -c Release
```

Takes roughly 3 minutes (three benchmark classes, each with its own warm-up + iterations — the culture-comparison and reference-equality classes run a million-plus iterations per benchmark method).

### What it measures

1. **`StringComparisonBenchmarks`** — the same case-mismatched pair of strings compared 100,000 times under `OrdinalIgnoreCase`, `CurrentCultureIgnoreCase`, and `InvariantCultureIgnoreCase`.
2. **`ReferenceEqualityFastPathBenchmarks`** — a 500-character string compared to itself vs. to a different object with identical content, 1,000,000 times each, isolating `Equals`'s reference-equality fast path.
3. **`SubstringAllocationBenchmarks`** — `Substring(0, s.Length)` (the self-reference fast path) vs. any other range, 100,000 calls each.

See the results tables in [`../../article.md`](../../article.md#performance-notes) for the measured numbers from this run, including the intern-pool memory-retention proof (measured separately in `Chapter09.Demo`, not `BenchmarkDotNet`, since it's about process-lifetime retention rather than per-call cost).
