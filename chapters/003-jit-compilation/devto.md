---
title: "Inside .NET — Episode 4: JIT Compilation Explained"
published: false
tags: dotnet, csharp, clr, performance
---

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
static long HotMethod(int n)
{
    long x = n;
    if ((n & 1) == 0) x = x * 3 + 1;
    else x = x ^ (x << 2);
    return x % 97;
}
```

Run that in a loop across 12 timed batches of 2 million calls each, and something interesting shows up: the early batches are slower and noisier than the later ones, which settle into a lower, stable per-call time. That's not GC warm-up. That's the JIT switching gears underneath you, and it's worth understanding exactly how.

## "The JIT compiles IL to native code on first call" is true, and also the least interesting part

Modern .NET doesn't JIT a method once and call it done. Here's the actual pipeline:

**1. First call → the prestub, not a straight compile.** A method table slot doesn't start out pointing at real code — it points at a shared **prestub**. The prestub notices the method has never run, triggers compilation, and then **backpatches** the slot to point directly at the compiled native code. Every call after that is a direct jump. No runtime "is this compiled?" check exists — the redirect is baked into the pointer itself.

**2. That first compile is deliberately cheap — Tier 0.** Quick JIT skips the expensive optimization passes (aggressive inlining, loop unrolling, elaborate register allocation) to minimize compile time, and instruments the method with call-count hooks while it's at it. This is the default since .NET Core 3.0 — it's called **tiered compilation**.

**3. Hot methods get recompiled — Tier 1.** Once a method's call count crosses a threshold, the runtime recompiles it fully optimized on a **background thread** — your calling threads never block on this — then swaps the entry point over via the same backpatching mechanism. A long-running loop that's already mid-execution when this happens doesn't have to wait for re-entry either: **On-Stack Replacement (OSR)** lets it jump to optimized code mid-flight.

**4. ReadyToRun (R2R) short-circuits Tier 0 — sometimes.** R2R ships precompiled native code alongside IL, produced ahead of time against a specific CLR version. If the running CLR matches exactly, that code runs directly with zero JIT cost. If it doesn't match, the CLR falls back to normal JIT, transparently. It's a versioned cache, not an unconditional bypass — and a hot R2R'd method can still get promoted to Tier 1 later.

**5. Dynamic PGO makes Tier 1 smarter, not just present.** Tier 0's instrumentation goes beyond call counts under PGO — it records which branch actually fires and which concrete type shows up at a call site. Tier 1 recompilation uses that real data to favor the hot branch, speculatively devirtualize a dominant call-site type (with a guarded fallback), and inline based on observed frequency instead of static guesses.

**6. Native AOT removes the JIT step, period.** The whole trimmed app compiles to one native binary ahead of time — no embedded IL, no JIT, no tiering, nothing left to compile at run time. GC and exception handling are still there, statically linked. What's gone: unbounded reflection, `Reflection.Emit`, and anything else that assumes IL is around to compile on demand.

## Why this matters beyond trivia

If you're choosing a deployment model for a system, this isn't academic. A fleet of serverless functions rarely lives long enough to reach Tier 1, let alone benefit from PGO's traffic-shaped optimization — Native AOT's "already optimized, zero compile at start" tradeoff wins there. A long-running service inverts that: the JIT-plus-tiering-plus-R2R model amortizes its one-time cost across millions of requests and keeps adapting to real traffic via dynamic PGO, which a frozen AOT binary structurally can't do. Picking one model for every workload in your system, without asking which side of that tradeoff you're actually on, is the mistake.

## Try it yourself

The full demo — including the `RuntimeFeature.IsDynamicCodeCompiled` check and the full batch-timing table — is in [`code/Chapter03.Demo`](code/Chapter03.Demo/Program.cs).

```bash
cd Chapter03.Demo
dotnet run -c Release
```

Then compare against tiering disabled:

```bash
DOTNET_TieredCompilation=0 dotnet run -c Release
```

Full mechanics, all five diagrams, and the Architect's Perspective breakdown (JIT vs. Native AOT as a system-level decision, not a flag) are in the [full article](article.md).

*This is Episode 4 of Inside .NET — Part I, The Foundation.*
