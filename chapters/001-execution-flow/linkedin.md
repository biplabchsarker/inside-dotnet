Ever wondered what actually happens between typing `dotnet run` and seeing "Hello, World!" print?

Here's the short version: your C# code is NOT compiled to machine code by the C# compiler. It's compiled to IL — Intermediate Language — a portable, CPU-agnostic instruction set. That's it. No CPU on earth executes IL directly.

The real translation happens at run time: the CLR loads your assembly, resolves its types from metadata, and hands each method to the JIT compiler THE FIRST TIME it's called. The JIT compiles that method to real native machine code, specific to the CPU it's running on right now, and caches it for the rest of the process's life.

This is why:
→ .NET runs unmodified on Windows, Linux, macOS, x64, ARM64 — the CPU-specific step happens on your machine, not at build time.
→ The first call to a method is often slower than the 1000th — you're paying a one-time JIT compilation cost.
→ Native AOT exists at all — it moves that JIT step to publish time, trading some runtime flexibility for near-instant startup.

Underneath all of this, the CLR is also continuously running garbage collection, exception handling, security checks, and thread scheduling — services your code depends on without ever calling them directly.

This is Episode 2 of Inside .NET — the foundation every later topic in the series (memory, DI, async, EF Core, architecture) builds on. Full breakdown with diagrams and runnable code in the comments.

#dotnet #csharp #clr #softwarearchitecture
