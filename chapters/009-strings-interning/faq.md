# FAQ — Strings & Interning

**Q: Are all strings interned automatically?**
A: No — only string literals (and `const` expressions the compiler folds into a single literal) are interned automatically, via the `ldstr` IL instruction. Anything built at runtime — concatenating variables, `StringBuilder.ToString()`, `string.Format`, interpolation with a runtime value, `Substring`, reading from a file or the console — allocates a genuinely new string every time, with no automatic check against the intern pool.

**Q: If two runtime-built strings have identical content, are they `ReferenceEquals`?**
A: No, unless something explicitly interned one of them. Equal content only guarantees `Equals`/`==` returns `true` — it says nothing about whether the two strings are the same object. This chapter's demo shows exactly this: two separately-built strings with matching content are distinct objects until `string.Intern` is called.

**Q: What's the difference between `string.Intern` and `string.IsInterned`?**
A: `string.Intern(s)` mutates the pool if needed: it returns the existing pooled reference if `s`'s content is already interned, or adds `s` itself to the pool and returns it if not. `string.IsInterned(s)` is read-only — it returns the pooled instance if present, or `null`, and never adds anything to the pool.

**Q: Is it safe to intern strings to speed up comparisons or save memory?**
A: Only for a small, bounded, recurring vocabulary. Every interned string is rooted for the life of the process — nothing is ever removed from the pool. This chapter measured interning 200,000 unique runtime strings and found ~16 MB left permanently unreclaimable after a full garbage collection, versus the same strings returning to near-baseline memory when simply discarded. Interning unbounded content (user input, request IDs, arbitrary external data) is a genuine memory-leak pattern, not an optimization.

**Q: Does `Substring` always allocate a new string?**
A: Almost always, but not literally always — `s.Substring(0, s.Length)`, requesting the entire string back, returns `s` itself with zero allocation. Any other range, even one character short of the full length, allocates a new string and copies the requested characters into it. There's no partial buffer sharing — if you want zero-copy slicing, use `ReadOnlySpan<char>`/`AsSpan()` instead.

**Q: Why is `StringComparison.Ordinal` recommended over `CurrentCulture` for things like file paths or identifiers?**
A: Two measured reasons: it's roughly 25× faster in this chapter's benchmark (culture-aware comparison routes through the globalization engine's linguistic rules instead of a raw byte comparison), and it avoids a real correctness bug class — culture-aware case folding genuinely differs by culture for the same input. Under the `tr-TR` culture, `"I".ToLower()` produces `"ı"`, not the ASCII `"i"` an ordinal comparison expects, so code that's correct under one culture can silently misbehave under another.

**Q: Can I rely on a string's `GetHashCode()` being the same across different runs of my program?**
A: No. Since .NET Core, string hash codes are randomized with a per-process seed (a mitigation against hash-flooding denial-of-service attacks), so the same string will produce a different hash code the next time the process starts. Never persist a string's hash code to a file, cache, or database expecting it to be meaningful later.
