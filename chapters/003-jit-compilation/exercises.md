# Exercises — JIT Compilation Explained

1. **Watch tiering disappear.** Run the companion demo twice: once as `dotnet run -c Release`, then again with `DOTNET_TieredCompilation=0` set (see [`code/README.md`](code/README.md) for the exact syntax on your shell). Compare the shape of the two output tables — specifically, does the first batch in the tiering-disabled run look like the *last* batch of the normal run, or like its own first batch? Explain what that tells you about where Tier 0's "quick but slower" cost is actually paid.

2. **Isolate OSR from ordinary tiering.** Add a second method to the demo that runs a single very long loop (tens of millions of iterations) inside *one* call, rather than the demo's current pattern of many short calls in a loop. Time that one call with `DOTNET_TieredCompilation=0` vs. the default, and separately with `DOTNET_TC_QuickJitForLoops=0` (illustrative name — check current `dotnet/runtime` docs for the live switch) forcing loop methods to skip Quick JIT entirely. Explain, in your own words, why a single long-running call behaves differently under tiering than the demo's original short, frequently-repeated calls do.

3. **Stretch — publish the demo as Native AOT and diff the behavior.** Add a second, minimal project (or an AOT-specific `PublishAot` property on a copy of the demo) and `dotnet publish -c Release -r <your-RID> /p:PublishAot=true` it. Run the published binary and compare `RuntimeFeature.IsDynamicCodeCompiled` against the JIT-based run's output. Then try adding a trivial `System.Reflection`-based call (e.g., enumerating a type's properties via `GetType().GetProperties()`) to a copy of the demo and re-publish as AOT — does it still work, warn, or fail, and why does that match this chapter's "Native AOT removes the JIT step, not just defers it" claim?

## Challenge

**Predict the output before running it.**

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
static long Calc(long x) => x * 3 + 1;

var sw1 = Stopwatch.StartNew();
for (int i = 0; i < 5; i++) Calc(i);
sw1.Stop();

var sw2 = Stopwatch.StartNew();
for (int i = 0; i < 5_000_000; i++) Calc(i);
sw2.Stop();

Console.WriteLine(sw1.Elapsed.TotalMilliseconds / 5 > sw2.Elapsed.TotalMilliseconds / 5_000_000); // A
```

Will `A` print `True` or `False`? Write down your reasoning — specifically, name which two things are being averaged away in each per-call figure (Tier 0 compile cost amortized over 5 calls vs. over 5,000,000) — before running it. This is the same "gap between first and second call" idea from Episode 2, now with the actual tiering mechanism behind it.

**Previous:** [Episode 3 — Understanding the CLR](../002-clr/article.md)
**Next:** [Episode 5 — Assemblies, DLLs & Metadata](../004-assemblies-metadata/article.md)
