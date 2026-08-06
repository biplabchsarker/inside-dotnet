# Interview Questions — What Really Happens When You Run a .NET Application?

**Q1: Walk me through what happens between running `dotnet MyApp.dll` and your `Main` method executing.**
A: The `dotnet` host (via `hostfxr`) resolves which installed runtime version to use based on `MyApp.runtimeconfig.json`, then loads `coreclr` into the process. The CLR's assembly loader reads `MyApp.dll`'s PE header and manifest, resolving referenced assemblies. When `Main` is first invoked, the type loader resolves the containing type from metadata, and the JIT compiler compiles `Main`'s IL to native machine code, which is then cached and executed.

**Q2: What's the difference between IL and native machine code, and why does .NET have both?**
A: IL is a portable, stack-based, CPU-agnostic instruction set that the C# compiler produces — it says *what* to do but not in terms any specific CPU understands. Native machine code is what the CPU actually executes. .NET has both so the same compiled assembly can run unmodified on any supported OS/CPU combination — the CPU-specific translation (JIT) happens at run time, on the machine that's actually running it.

**Q3: What is "tiered compilation" and what problem does it solve?**
A: The JIT initially compiles a method with minimal optimization (Tier 0) to minimize the delay before that method can run, then — if the method is called frequently — recompiles it with full optimizations (Tier 1) in the background. It solves the tension between "optimize everything fully" (slow startup) and "never optimize" (permanently slower steady-state execution).

**Q4: When and why would you reach for Native AOT instead of the default JIT model?**
A: When cold-start latency and memory footprint matter more than flexibility — CLI tools, containers that scale to zero, serverless functions billed by execution time. The tradeoff is losing runtime reflection-heavy/dynamic-codegen scenarios that depend on IL still being present and JIT-compilable at run time.

**Q5: If GC, exception handling, and threading are "runtime services," what do they have in common that requires the CLR specifically?**
A: All three require the runtime to understand the *structure* of running code — object layouts and live references for GC, stack frames and protected regions for exception unwinding, execution contexts for thread scheduling. That structural knowledge comes from CLR metadata and type information, which a purely native compiled binary (without a runtime) doesn't preserve.
