# Summary — JIT Compilation Explained

- Every method's table slot starts pointing at a shared **prestub**; the first call triggers JIT compilation, and the prestub then **backpatches** the slot to point directly at the compiled native code — no runtime "is this compiled?" check exists after that.
- **Tiered Compilation** (default since .NET Core 3.0): most methods JIT quickly and minimally-optimized first (**Tier 0**, instrumented with call-count hooks), then get recompiled fully optimized on a background thread (**Tier 1**) once a call-count threshold is crossed — the entry point is swapped over without blocking callers.
- Long-running loops can transition from Tier 0 to optimized code **mid-execution** via **On-Stack Replacement (OSR)**, instead of waiting for the method to be re-entered.
- **ReadyToRun (R2R)** ships precompiled native code alongside IL as a **versioned cache keyed to an exact CLR match** — used directly if the running CLR matches, otherwise falls back to normal JIT transparently. R2R code can still be promoted to Tier 1 later if it's hot.
- **Dynamic PGO** extends Tier 0's instrumentation beyond call counts to real branch outcomes and call-site type data, letting Tier 1 recompilation optimize based on actual observed program behavior (hot-branch layout, guarded devirtualization).
- **Native AOT** removes the JIT step entirely: full ahead-of-time compilation + trimming to one native binary per RID, no embedded IL, no runtime fallback — trading reflection/dynamic-codegen flexibility for near-instant startup and lower memory footprint.

**Previous:** [Episode 3 — Understanding the CLR](../002-clr/README.md)
**Next:** [Episode 5 — Assemblies, DLLs & Metadata](../004-assemblies-metadata/README.md)
