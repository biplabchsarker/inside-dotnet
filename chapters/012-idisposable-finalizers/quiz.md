# Quiz — IDisposable & Finalizers

1. Why does an object with a finalizer survive Gen 0 collection?
2. What does `GC.SuppressFinalize(this)` do?
3. Can you predict exactly when a finalizer will run?
4. Why is it dangerous to access other managed objects inside a finalizer?
5. What happens if a finalizer throws an unhandled exception?

<details>
<summary>Answers</summary>

1. Because when the GC determines the object is unreachable, it checks the Finalization Queue. Finding the object there, the GC moves it to the F-Reachable queue. The F-Reachable queue acts as a strong GC Root, "resurrecting" the object until the Finalizer Thread can process it. Surviving the collection automatically promotes it to Gen 1.
2. It sets a bit in the object's header telling the CLR to remove it from the Finalization Queue. This ensures that when the object becomes unreachable, the GC reclaims it immediately without promoting it to the F-Reachable queue.
3. No. Finalizers run non-deterministically. They only run after a GC collection occurs (which is driven by memory pressure, not time), and they run on a background thread at an unspecified time.
4. Because there is no guaranteed order for finalization. If your object holds a reference to a managed `FileStream`, that `FileStream` might have already been finalized and closed by the time your finalizer runs.
5. The Finalizer Thread crashes, which immediately terminates the entire application process (since .NET Framework 2.0).

</details>
