# Senior Interview Questions

### 1. What happens internally when an object with a finalizer is collected?
**Answer:** 
When the GC finds the object is unreachable, it checks the Finalization Queue. Seeing the object there, it moves the object to the **F-Reachable Queue**. The F-Reachable queue acts as a strong GC Root, which "resurrects" the object. Because it survived the collection, it is promoted to Gen 1. Later, a dedicated background Finalizer Thread reads from the F-Reachable queue and executes the finalizer. Only on the *next* GC cycle will the memory actually be reclaimed.

### 2. What does `GC.SuppressFinalize(this)` do, and why is it important?
**Answer:**
It sets a bit in the object's object header that tells the CLR to remove the object from the Finalization Queue. If you implement `Dispose()` to clean up resources explicitly, you must call this. Otherwise, the object will *still* be sent to the F-Reachable queue and promoted to Gen 1, wasting memory and CPU cycles even though the resources were already cleaned up.

### 3. Why is it dangerous to reference other managed objects inside a finalizer?
**Answer:**
Finalization order is non-deterministic. If your finalizer references a managed `FileStream`, that `FileStream` might have already been finalized and closed by the time your finalizer executes. Finalizers should strictly only clean up *unmanaged* fields (like `IntPtr`).

### 4. How does `SafeHandle` improve upon raw finalizers?
**Answer:**
`SafeHandle` is a specialized class that inherits from `CriticalFinalizerObject`. It guarantees that its finalizer will run even in catastrophic situations (like a `ThreadAbortException` or `OutOfMemoryException`), and it prevents handle recycling exploits. By wrapping unmanaged resources in a `SafeHandle`, your parent class no longer needs a finalizer at all—it just needs to implement `IDisposable` and dispose the `SafeHandle`.
