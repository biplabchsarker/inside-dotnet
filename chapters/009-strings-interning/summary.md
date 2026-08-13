# Summary — Strings & Interning

**TL;DR:** The CLR automatically interns every string literal (via `ldstr`) so identical literal text shares one heap object process-wide, but it never automatically interns anything built at runtime — concatenation, `StringBuilder`, `string.Format`, `Substring` — even when the content matches something already interned. `string.Intern`/`string.IsInterned` let you opt runtime strings into the pool explicitly, but every interned string is rooted for the process's life, which is a real, measured memory-growth risk for unbounded content. `Equals`/`==` check `ReferenceEquals` first (51.7× faster for identical references), `Substring` has a genuine zero-allocation fast path for the whole-string case, and culture-aware comparison is both ~25× slower and a real correctness bug class (the Turkish-I problem) — default to `Ordinal`.

**Key takeaways:**

- String literals and compiler-folded `const` expressions intern automatically via `ldstr` -- runtime-constructed strings never do, regardless of matching content.
- `string.Intern` returns the pool's existing reference if content is already interned, or adds and returns your own instance if not; `string.IsInterned` is read-only and returns `null` if nothing matches.
- `Equals`/`==` check `ReferenceEquals` before comparing any characters -- measured at a 51.7x speed difference between comparing a string to itself versus an equal-but-different object.
- `Substring(0, s.Length)` -- the whole string -- returns the original instance with zero allocation; every other range allocates a full, independent copy.
- Culture-aware comparison (`CurrentCulture`/`InvariantCulture`) is ~25x slower than `Ordinal` in this chapter's measured benchmark, and can produce genuinely different results across cultures for the same input (the Turkish-I problem) -- default to `Ordinal`/`OrdinalIgnoreCase` for anything that isn't user-facing text.
- Interning is a real trade-off, not a free win: measured directly, interning 200,000 unique runtime strings left ~16 MB permanently unreclaimable by a full GC collection, while discarding the same strings returned memory to near-baseline.
