# FAQ — What Really Happens When You Run a .NET Application?

**Q: Is IL the same thing as bytecode in Java?**
A: Conceptually similar — both are portable, intermediate instruction sets compiled from source and executed via a virtual-machine-like runtime. They are not binary-compatible or spec-identical, but they solve the same problem: portability plus a managed execution environment.

**Q: Does every method get JIT-compiled at startup?**
A: No. Methods are compiled lazily, the first time they're actually called. Code paths never hit during a run are never JIT-compiled at all — this is why a large application can still start reasonably fast.

**Q: Does Native AOT mean there's no CLR at all?**
A: There's no separate JIT step and no assembly-loading-from-IL step at run time, but runtime services like garbage collection still exist — they're linked into the native binary rather than loaded as a separate managed runtime.

**Q: Where does "managed" actually stop and "unmanaged" begin?**
A: At any P/Invoke call, COM interop boundary, or raw OS syscall your code (or a library you depend on) makes. Inside that boundary, the GC has no visibility into memory, and you're responsible for correct cleanup — typically via `SafeHandle` or `IDisposable`.

**Q: Why does my app feel slower on its very first request after deployment (cold start)?**
A: That's JIT warm-up compounded across many methods being called for the first time simultaneously — assembly loading, type loading, and JIT compilation all happening under real request load. ReadyToRun or Native AOT are the standard mitigations.
