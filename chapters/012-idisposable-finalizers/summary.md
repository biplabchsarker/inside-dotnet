# Chapter 012: IDisposable & Finalizers

**TL;DR**
Adding a finalizer to a C# class makes its garbage collection **tens of times slower** (47.65×-73.53× across two measured runs) by forcing the object to survive Gen 0, get promoted to Gen 1, and stall the dedicated Finalizer Thread. Only use them as an absolute last resort.

## The Core Concept
The `.NET` Garbage Collector handles *managed* memory perfectly. But it knows nothing about *unmanaged* resources (database connections, file handles, network sockets, OS handles). To clean these up, we need the `IDisposable` interface.

## Dual-Path Cleanup Pattern
When you wrap an unmanaged resource, you must provide two ways to clean it up:
1. **The Fast Path (Dispose):** The developer explicitly calls `Dispose()` (usually via a `using` statement). The resource is freed instantly, and you call `GC.SuppressFinalize(this)` to tell the GC to ignore the finalizer. The object dies cheaply in Gen 0.
2. **The Slow Path (Finalizer):** A safety net in case the developer forgets. The GC sees the object is dead, puts it on the **F-Reachable Queue** (resurrecting it into Gen 1), and a background thread eventually calls `~MyClass()`.

## Modern Best Practice
Avoid finalizers completely by wrapping your raw `IntPtr` resources in a `SafeHandle`. The `SafeHandle` takes care of the finalization safely, leaving your custom class to only implement `IDisposable`.

## The One-Line Rule
> Adding a finalizer guarantees an object will survive Gen 0, get promoted to Gen 1, and stall the Finalizer Thread — avoid them unless strictly necessary.
