"The JIT compiles IL to native code on first call." True, but it's the least interesting part of the story in modern .NET.

Here's what actually happens:

→ A method's very first call doesn't go through any "is this compiled?" check. It's redirected through a shared "prestub," which triggers compilation and then BACKPATCHES the method's entry point to point straight at the native code. Every call after that is a direct jump — no runtime branching involved.

→ That first compile is deliberately cheap. Tier 0 (Quick JIT) skips the expensive optimization passes to minimize startup latency, and quietly instruments the method with call-count hooks while it's at it.

→ If a method turns out to be hot, the runtime recompiles it FULLY optimized on a background thread — your calling threads never wait for this — then swaps the entry point over. This is Tiered Compilation, and it's been the default since .NET Core 3.0.

→ ReadyToRun (R2R) ships precompiled native code alongside IL as a versioned cache. It's used directly if it matches the exact running CLR — otherwise it silently falls back to normal JIT. It's not "never JIT'd," it's "JIT'd conditionally."

→ Dynamic PGO takes Tier 0's instrumentation further: real branch outcomes, real call-site types. Tier 1 recompilation then optimizes based on what your program ACTUALLY does, not generic heuristics.

→ Native AOT removes the JIT step entirely — full ahead-of-time compilation, no fallback, near-instant startup — at the cost of reflection-heavy/dynamic-codegen scenarios needing rework.

This is Episode 4 of Inside .NET. Full mechanics, diagrams, and a runnable demo that surfaces the Tier 0 → Tier 1 transition in the comments.

#dotnet #csharp #clr #jit #softwarearchitecture
