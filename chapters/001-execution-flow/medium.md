# Inside .NET — Episode 2: What Really Happens When You Run a .NET Application?

*Part I — The Foundation*

You type `dotnet run`. A second later, "Hello, World!" prints and the process exits. In that second: a process was created, an assembly was loaded, its metadata was parsed, a method was JIT-compiled to native machine code, and a managed heap was armed and ready — before your `Main` method ran a single instruction.

## The suit-tailor analogy

Ordering a custom suit: you describe what you want (your C# source), a pattern-maker translates it into a standardized, language-neutral pattern (the compiler produces IL, not machine code), the pattern sits ready with your measurements (assembly metadata) — nobody cuts fabric yet. Only at the fitting does a tailor cut and sew a suit sized exactly to you (the JIT turns IL into native code for *this* CPU). Once cut, it's not redone next time (the JIT caches compiled code for the process's life).

## Why not compile straight to machine code?

Three reasons .NET deliberately keeps a two-stage model:

- **Portability** — the same assembly runs on Windows, Linux, macOS, x64, or ARM64, since CPU-specific translation happens at load/run time.
- **Runtime services** — GC, type safety, and exception handling all need the runtime to understand your code's *structure*, which raw machine code discards.
- **Run-time optimization** — the JIT can tailor native code to the actual CPU it's running on, something a build-time compiler targeting "any x64" can't do.

## The pipeline, step by step

1. **Roslyn** compiles your `.cs` files to IL + metadata — not machine code.
2. **hostfxr** resolves which installed .NET runtime to use, then loads `coreclr`.
3. The **assembly loader** parses the manifest and prepares metadata tables for lookup.
4. The **type loader** resolves a type from metadata the first time it's touched, building method tables and vtables.
5. **RyuJIT** compiles each method to native machine code *the first time it's called*, then caches that native code for the process's lifetime — this is why the first call into a method is often slower than the thousandth.
6. While running, the CLR continuously provides **GC, exception handling, security, and thread scheduling** underneath your code.
7. Calls into the OS or native libraries cross into **unmanaged** territory, where the GC loses visibility and manual cleanup rules apply.

## What this explains in practice

- Why .NET isn't "interpreted" — IL is compiled to real native code, just later than C/C++.
- Why "slow startup" complaints usually mean JIT warm-up + assembly loading, not steady-state speed.
- Why **Tiered Compilation** exists: the JIT emits a fast, less-optimized Tier 0 version first, then recompiles hot methods with full optimization (Tier 1) once they're called enough.
- Why **ReadyToRun** and **Native AOT** exist: both move JIT cost from run time to build/publish time, trading flexibility for startup speed.

## Try it yourself

The [companion code sample](code/Chapter01.Demo/Program.cs) times a recursive `Fibonacci` call twice — the first call includes JIT compilation cost, the second reuses cached native code. Paste the method into [sharplab.io](https://sharplab.io) to see the actual IL your C# compiled to.

*Next: [Episode 3 — Understanding the CLR](../002-clr/medium.md)*
