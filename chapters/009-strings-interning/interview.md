# Interview Questions — Strings & Interning

**Q1: What makes two string literals with identical text `ReferenceEquals` to each other?**
A: The compiler emits an `ldstr` IL instruction for each literal. When `ldstr` executes, the CLR checks the process-wide intern pool for a string with that exact content — if one already exists (from this literal or any other with the same text, anywhere in the process), it returns the existing reference instead of allocating a new one. Only the first occurrence of a given literal text actually allocates.

**Q2: Does concatenating two string variables at runtime produce an interned string?**
A: No. Runtime construction — `string.Concat`, `+` on non-constant operands, `StringBuilder.ToString()`, `string.Format`, interpolation with a runtime value — always allocates a genuinely new string through the ordinary heap allocator, with no automatic check against the intern pool, even if the resulting content exactly matches something already interned.

**Q3: What exactly does `string.Intern` return if the string's content is already in the pool, versus if it isn't?**
A: If the content is already interned, it returns the *existing* pooled reference — not the instance you passed in. If it isn't yet interned, it adds the exact instance you passed in to the pool and returns that same reference back. `string.IsInterned` is the read-only version: it returns the pooled instance if present, or `null`, and never adds anything.

**Q4: When does `Substring` allocate nothing at all?**
A: When the requested range is the entire string — `s.Substring(0, s.Length)` — which returns `s` itself. Any other range, even one character short of the full length, allocates a new string and copies the requested characters into it; there's no partial buffer sharing.

**Q5: Why is `StringComparison.Ordinal` the recommended default over `CurrentCulture` for things like identifiers or file paths?**
A: Two reasons, both measurable: performance (culture-aware comparison routes through the globalization engine's linguistic rules and measured roughly 25× slower in this chapter's benchmark) and correctness — culture-aware case folding genuinely differs by culture for the same input. Under `tr-TR`, `"I".ToLower()` produces `"ı"`, not the ASCII `"i"` an ordinal comparison expects, so a case-insensitive check correct under one culture can silently fail under another.

**Q6: Why is interning arbitrary user input a bad idea, even though interning itself is a real performance optimization?**
A: Because every interned string is rooted for the life of the process — the pool never releases anything, by design, since it's meant for a small, fixed vocabulary of recurring literals. Interning content that's unbounded (user input, request IDs, arbitrary external data) turns each unique value into a permanent, non-collectible allocation — measured in this chapter at roughly 16 MB retained for 200,000 unique interned strings that a full garbage collection could not reclaim, versus the same 200,000 strings returning to near-baseline memory when simply discarded instead.

**Q7: Does `Equals`/`==` on strings compare references before comparing content?**
A: Yes. `string.Equals(string, string)` — which both `==` and the instance `.Equals()` route through — checks `ReferenceEquals` first; only if that fails does it compare lengths, and only if those match does it walk the character buffers. This chapter measured a 51.7× speed difference between comparing a string to itself versus to a different object with identical content, direct confirmation of the fast path.
