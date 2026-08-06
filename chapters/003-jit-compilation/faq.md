# FAQ — JIT Compilation Explained

**Q: Is Tier 0 code "unoptimized," as in debug-build-quality code?**
A: No. Tier 0 skips the *expensive* optimization passes (aggressive inlining, loop optimizations, elaborate register allocation) to keep compile time low, but it's still a real release-quality JIT compile of your actual release IL — not a debug build, and not an interpreter. It's "minimally optimized," not "unoptimized."

**Q: If a method is in a ReadyToRun image, can it still end up running Tier 1 optimized code?**
A: Yes. R2R only decides whether the *initial* native code comes from a precompiled cache instead of a fresh JIT compile. If that method turns out to be called often enough at run time, tiered compilation can still promote it to a fully optimized Tier 1 version, same as any JIT-compiled method.

**Q: Does disabling tiered compilation make my app faster?**
A: It depends what you're optimizing for. Disabling it (`DOTNET_TieredCompilation=0`, illustrative) forces every method to compile fully optimized on first call — slower startup across the board, but every method runs at Tier-1 quality from its very first call, with no promotion delay. For long-running services with real hot paths, tiering (the default) usually wins overall; for tiny, short-lived CLI invocations it can occasionally be a wash or a loss.

**Q: How is dynamic PGO different from the "static PGO" or profile-based optimization some other compilers use?**
A: Static/offline PGO in other toolchains typically requires a separate training run, capturing a profile, and feeding it back into a later ahead-of-time build. Dynamic PGO in modern .NET happens *within a single running process*: Tier 0 instruments itself, the runtime observes real behavior from that same run, and Tier 1 recompilation for that same process consumes it immediately — no separate training step or profile file required.

**Q: Does Native AOT still have a garbage collector and exception handling?**
A: Yes. Native AOT removes the JIT compiler and the "load IL, compile on demand" model — it does not remove the CLR's runtime services. GC, structured exception handling, and the rest of what Episode 3 covered are statically linked into the AOT binary and still run exactly as they would under the JIT-based model.
