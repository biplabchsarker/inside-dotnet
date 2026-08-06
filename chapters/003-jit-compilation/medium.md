# Inside .NET — Episode 4: JIT Compilation Explained

*Part I — The Foundation*

Episode 2 established the fact: IL gets JIT-compiled to native code the first time a method is called, and the result is cached for the process's life. This episode is about the mechanism behind that sentence — because in modern .NET, "compile once, cache forever" is only half the picture.

## The chef, not the tailor

A dinner service, not a suit fitting: the first order for a dish tonight gets *something correct* onto the pass fast — no time for perfection (Tier 0, quick and minimally optimized). While cooking it, the chef counts how often it's ordered (Tier 0's call-count instrumentation). Once it's ordered enough, the chef works out the fully refined version and serves that from then on, without stopping service to do it (Tier 1, recompiled in the background). Menu items the head chef already perfected before service opened need no improvisation at all (ReadyToRun). A food truck with one fixed, fully-prepped menu needs no chef improvising anything at all (Native AOT).

## The prestub: how "first call" actually works

A method table slot doesn't start out pointing at real code — it points at a shared **prestub**. The prestub's job: notice the method has never run, trigger JIT compilation, then **backpatch** the slot to point directly at the compiled native code. Every call after the first skips the prestub entirely — it's a rewritten pointer, not a runtime check.

## Tiered compilation, precisely

- **Tier 0** — Quick JIT: reduced inlining, no loop optimization, simple register allocation. Fast to produce, instrumented with call-count hooks.
- **Tier 1** — full optimization, triggered once a call-count threshold is crossed, compiled on a background thread, swapped in via the same backpatching mechanism.
- **On-Stack Replacement (OSR)** — lets a currently-executing long-running loop jump to optimized code mid-flight, instead of waiting for re-entry.

This is why a long-running service tends to get measurably faster minutes into steady-state operation — methods graduating from Tier 0 to Tier 1 under real load.

## ReadyToRun: a versioned cache, not a bypass

R2R ships precompiled native code alongside IL. If the running CLR matches the exact version/ABI the image was built for, that code runs directly — zero JIT cost. If it doesn't match, the CLR falls back to normal JIT, transparently. And a hot R2R'd method can still get promoted to Tier 1 later — R2R and tiering aren't competing, they answer different questions.

## Dynamic PGO: instrumentation with a purpose

Tier 0 doesn't just count calls — dynamic PGO has it record which branch actually fires and which concrete type shows up at a polymorphic call site. Tier 1 recompilation uses that real data: favoring the hot branch, speculatively devirtualizing a dominant call-site type with a guarded fallback, inlining based on observed frequency instead of static guesses.

## Native AOT: removing the JIT, not just deferring it

Native AOT compiles the whole trimmed application to a native binary at publish time — no embedded IL, no JIT, no tiering, because there's nothing left to compile at run time. GC and exception handling are still there, statically linked. What's gone is anything depending on IL being compilable on demand: unbounded reflection, `Reflection.Emit`, some plugin patterns.

## Try it yourself

The [companion demo](code/Chapter03.Demo/Program.cs) runs a hot method across timed batches — watch the early batches be slower and noisier (Tier 0, still promoting) and later batches settle into a faster, steadier per-call time (Tier 1). Set `DOTNET_TieredCompilation=0` and compare.

*Next: [Episode 5 — Assemblies, DLLs & Metadata](../004-assemblies-metadata/medium.md)*
