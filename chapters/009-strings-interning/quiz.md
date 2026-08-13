# Quiz — Strings & Interning

1. What IL instruction do string literals compile to, and what does the CLR check when that instruction executes?
2. Does `string.Concat` on two runtime variables produce a string that's automatically added to the intern pool?
3. If `string.IsInterned(s)` returns `null`, what does that tell you — and does calling it change anything?
4. Under what exact condition does `Substring` return the original string instance instead of allocating a new one?
5. Name two distinct reasons `StringComparison.Ordinal` is preferred over `CurrentCulture` for comparing identifiers.

<details>
<summary>Answers</summary>

1. `ldstr`. When it executes, the CLR checks the process-wide intern pool for a string with the exact same content, returning the existing reference if one is found, or adding this one and returning it if not.
2. No. Runtime construction — including `string.Concat`, `+` on variables, `StringBuilder.ToString()`, and `string.Format` — always allocates a new string with no automatic intern-pool check, regardless of whether the content matches something already interned.
3. It tells you `s`'s content is not currently in the intern pool. Calling `IsInterned` is read-only — it never adds anything to the pool itself.
4. When the requested range is the entire string: `startIndex == 0` and `length == s.Length`. Any other range always allocates a new string.
5. Performance (ordinal comparison is roughly 25× faster in this chapter's measured benchmark, since it skips the globalization engine entirely) and correctness (culture-aware case folding genuinely differs by culture, e.g. the Turkish-I problem, so an ordinal comparison gives a locale-independent, predictable answer for non-linguistic text).

</details>
