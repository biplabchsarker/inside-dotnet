# Self-Check Quiz — JIT Compilation Explained

1. What is the "prestub," and what does it do the first time a method is called?
2. What's the difference between Tier 0 and Tier 1 compilation, and what triggers the transition from one to the other?
3. If a method's code comes from a ReadyToRun image, can it still later be recompiled by the JIT? Why or why not?
4. What extra information does dynamic PGO capture beyond a plain call counter, and what does Tier 1 recompilation do with it?
5. Name one capability that generally does NOT work under Native AOT, and explain why the runtime can't fall back to handling it the way it would under the standard JIT model.

<details>
<summary>Answers</summary>

1. The prestub is the shared, initial native code every method table slot points at before that method has ever been compiled. On first call, it triggers JIT compilation and then backpatches the method table slot (and any resolved call sites) to point directly at the compiled native code — every later call bypasses the prestub entirely.
2. Tier 0 is a quick, minimally-optimized compile (instrumented with call-count hooks); Tier 1 is a fully optimized recompile. The transition is triggered once a method's Tier 0 call count crosses a threshold, at which point Tier 1 compilation happens on a background thread and the entry point is swapped over.
3. Yes. R2R only determines where the *initial* native code comes from (a precompiled cache vs. a fresh JIT compile). If the method is hot enough, tiered compilation can still promote it to a fully optimized Tier 1 version regardless of whether it started as R2R or freshly JIT'd code.
4. Dynamic PGO captures real branch-outcome data (which side of an `if`/`switch` actually executes) and observed concrete types at polymorphic call sites. Tier 1 recompilation uses this to favor the hot branch in code layout, speculatively devirtualize dominant call-site types (with a guarded fallback), and make inlining decisions based on real behavior instead of static heuristics.
5. Unbounded runtime reflection over arbitrary types (or `Reflection.Emit`-based dynamic code generation) generally doesn't work under Native AOT, because there's no embedded IL and no JIT compiler linked into the binary — the runtime has no mechanism left to compile something it wasn't told about ahead of time at publish time.

</details>
