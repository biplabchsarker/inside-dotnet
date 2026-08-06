---
title: "Inside .NET — Episode 2: What Really Happens When You Run a .NET Application?"
published: false
tags: dotnet, csharp, clr, softwarearchitecture
---

```csharp
TimeSpan first = Time(() => Fibonacci(28));
TimeSpan second = Time(() => Fibonacci(28));
Console.WriteLine($"First call    : {first.TotalMilliseconds:F3} ms (includes JIT compile)");
Console.WriteLine($"Second call   : {second.TotalMilliseconds:F3} ms (native code already cached)");

static long Fibonacci(int n) => n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
```

Run that on your machine and the first call is almost always slower than the second — same method, same input, no caching logic anywhere in the code. That gap is the whole subject of this chapter.

## Your C# never becomes machine code directly

The C# compiler (Roslyn) doesn't produce machine code. It produces **IL** — Intermediate Language, a portable, CPU-agnostic instruction set — plus metadata describing every type and method. No CPU on earth executes IL directly.

The real translation happens at run time. `dotnet run` (or the generated `apphost`) uses `hostfxr` to find an installed runtime, loads `coreclr`, and the CLR's assembly loader parses your assembly's metadata. The first time a method is actually *called*, the JIT compiler (RyuJIT) compiles that one method to native machine code — specific to the exact CPU it's running on — and caches the result for the rest of the process's life. Call it again, and you get the cached native code with zero recompilation.

## Why bother with two stages?

Three reasons .NET deliberately keeps IL and native code as separate steps:

- **Portability** — one compiled assembly runs unmodified on Windows, Linux, macOS, x64, ARM64. The CPU-specific step happens on the machine actually running it, not at build time.
- **Runtime services** — garbage collection, type safety, and exception handling all need the runtime to understand your code's *structure*. A raw native binary throws that structure away.
- **Run-time optimization** — the JIT tailors native code to the actual CPU it's on. A build-time compiler targeting "any x64 chip" can't do that.

## The suit-tailor analogy, if you want the non-technical version

Ordering a custom suit: you describe what you want (your C# source), a pattern-maker turns it into a standardized pattern (Roslyn produces IL) — nobody's cutting fabric yet. Only at the fitting does a tailor cut a suit sized exactly to you (the JIT compiles IL to native code for *this* CPU). Once it's cut, it isn't redone next time you walk in with the same body (the JIT caches compiled code for the process's life).

## What this explains

- Why .NET isn't "interpreted" the way early Java myths claimed — IL is JIT-*compiled* to real native code, just later than C/C++.
- Why "slow startup" complaints are usually JIT warm-up + assembly loading, not steady-state execution speed — and get fixed differently depending on which one you're actually paying for.
- **Tiered Compilation** (on by default): the JIT emits a fast, unoptimized Tier 0 version first, then recompiles hot methods with full optimization (Tier 1) once they're called enough.
- **ReadyToRun** and **Native AOT**: both move JIT cost from run time to publish time. R2R keeps the CLR and trims JIT cost; Native AOT removes the CLR's load/JIT step entirely for near-instant startup, at the cost of losing runtime reflection-heavy features the AOT trimmer can't see statically.

## When Native AOT is actually worth it

Not "always," and not "because it sounds faster." It's worth the migration cost specifically when cold-start latency is a billed or SLA-relevant cost — scale-to-zero containers, serverless functions charged per invocation including cold start — *and* an audit confirms the codebase doesn't lean on unbounded runtime reflection or dynamic codegen. Skipping that audit is how "let's try Native AOT" turns into a failed publish step three sprints later.

## Try it yourself

The full [companion code sample](code/Chapter01.Demo/Program.cs) also inspects live CLR version info and assembly metadata via `System.Reflection` — clone it, run `dotnet run`, and paste `Fibonacci` into [sharplab.io](https://sharplab.io) to see the actual IL opcodes (`ldarg.0`, `call`, `add`) your C# compiled to.

Full chapter — with 8 diagrams, the three-tier Architect's Perspective, and interview questions — is in [`article.md`](article.md).

*Next: [Episode 3 — Understanding the CLR](../002-clr/devto.md)*
