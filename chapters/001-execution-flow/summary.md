# Summary — What Really Happens When You Run a .NET Application?

- C# → **Roslyn** compiles to **IL + metadata** (not machine code) at build time.
- `dotnet run` / apphost → **hostfxr** locates a runtime → **coreclr** loads.
- **Assembly loader** parses the manifest and metadata; **type loader** resolves types on first touch.
- **JIT (RyuJIT)** compiles each method to native code *on first call*, then caches it for the process's life — this is JIT warm-up.
- Once running, the CLR continuously provides **GC, exception handling, security, and threading** services.
- Managed code (yours + BCL) is GC-tracked and type-safe; crossing into **unmanaged** code (P/Invoke, COM) loses that safety net.
- **Tiered Compilation**, **ReadyToRun**, and **Native AOT** are three different answers to "how do we reduce JIT/startup cost."

**Next:** [Episode 3 — Understanding the CLR](../002-clr/README.md)
