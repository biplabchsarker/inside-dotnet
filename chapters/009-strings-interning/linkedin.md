Interning a string "to make comparisons faster" sounds harmless. I measured what it actually costs — and it's not what most people expect.

I built 200,000 unique runtime strings, discarded them, and force-collected: heap returned to baseline. Then I built 200,000 *different* unique strings, interned every one of them, and force-collected again: **16 MB stayed permanently allocated.** Not "eventually collected." Permanently — for the life of the process. The intern pool is a GC root that never releases anything, by design. That's correct for a small, fixed vocabulary of recurring literals. It's a silent memory leak the moment what you're interning is unbounded — user input, request IDs, anything external.

Two more things this chapter measured instead of just claiming:

`Equals`/`==` checks `ReferenceEquals` before comparing a single character. Comparing a 500-character string to *itself* is **51.7x faster** than comparing it to a different object holding identical text. Same answer either way — wildly different cost.

And `StringComparison.CurrentCulture` isn't just ~25x slower than `Ordinal` in my benchmark — it can give you a genuinely different, wrong answer. Under the Turkish culture, `"I".ToLower()` produces `"ı"`, not the ASCII `"i"` an ordinal comparison expects. That's a real bug class hiding behind a performance number.

This is Episode 10 of Inside .NET — full interning mechanics, five diagrams, and four real BenchmarkDotNet/measured-memory result tables, not estimates.

#dotnet #csharp #memorymanagement #softwarearchitecture
