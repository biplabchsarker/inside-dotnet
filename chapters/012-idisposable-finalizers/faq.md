# Frequently Asked Questions

**Q: Do I need to implement `IDisposable` if my class only holds references to other managed objects?**
A: No, unless those managed objects themselves implement `IDisposable` and your class "owns" them. If your class just holds a `string` or a `List<int>`, let the GC handle it.

**Q: Should I set objects to `null` in `Dispose()`?**
A: Usually, no. Setting fields to `null` doesn't help the GC (reachability is based on roots, not internal fields). The only exception is if your object will stay alive for a long time after being disposed, and you want to aggressively free a large child object.

**Q: Why does the `using` statement exist if we have `try/finally`?**
A: It is purely syntactic sugar. The C# compiler translates `using (var x = new X()) { }` directly into a `try { } finally { x?.Dispose(); }` block. It saves boilerplate and prevents you from forgetting the `finally` block.

**Q: Can a finalizer run while the object is still being used?**
A: Yes! If the JIT compiler determines that the current method makes no further reference to the `this` pointer after a certain line, the object becomes eligible for garbage collection *before the method finishes executing*. This is why `GC.KeepAlive(this)` sometimes exists in low-level interop code.

**Q: What is the difference between `Dispose()` and `Close()`?**
A: Historically, some classes (like `SqlConnection` or `FileStream`) implement `Close()` to mean "shut down the connection, but you might be able to reopen it," whereas `Dispose()` means "destroy the object permanently." Under the hood, `Dispose()` usually just calls `Close()`. Prefer `using`/`Dispose()`.
