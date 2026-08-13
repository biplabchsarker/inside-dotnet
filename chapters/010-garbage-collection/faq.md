# FAQ — Garbage Collection Fundamentals

**Q: Does .NET use reference counting, like Python or classic COM?**
A: No. The CLR's garbage collector traces reachability from a fixed set of roots (stack references, static fields, CPU registers, GC handles) — anything reachable transitively from a root is live, anything not reached is garbage. This correctly reclaims reference cycles (two objects that only reference each other, with nothing else pointing to either) that pure reference counting cannot reclaim without extra cycle-detection machinery.

**Q: Why does the GC bother compacting the heap instead of just sweeping away dead objects?**
A: Because the bump-pointer allocator described in Episode 8 only knows how to extend a single contiguous pointer forward — it can't reuse a scattered hole left behind by a swept object. Compaction slides every surviving object together, closing every gap, so that fast allocation path keeps working exactly as it did before.

**Q: Does calling `GC.Collect()` help my application's performance?**
A: Almost never, in application code. It forces a full collection — measured in this chapter at roughly 5.84× the cost of a Gen 0 collection — undoing exactly the generational-hypothesis efficiency the GC was designed around. The GC has much better information about actual memory pressure than a guess embedded in your code; let it decide.

**Q: If I set a local variable to `null`, is the object it pointed to immediately eligible for collection?**
A: Not necessarily, if you're still inside the same executing method. This chapter's demo proves it directly: nulling a local and calling `GC.Collect()` from the same still-executing frame can still show the object as reachable via a `WeakReference`. The real guarantee is that it becomes collectible once the frame holding the last root to it returns — not the instant you reassign the variable.

**Q: Why does a finalizable object take two collections to actually go away?**
A: The first collection determines it's unreachable but routes it to the finalization queue instead of reclaiming it immediately, since its finalizer hasn't run yet. Only after `GC.WaitForPendingFinalizers()` runs the finalizer, followed by a second collection, is the memory actually freed. You can observe this directly with a *long* `WeakReference` (`trackResurrection: true`) — the default *short* one clears before the finalizer even runs, so it can't show you this two-step lifecycle.

**Q: Should I turn on Server GC for better performance?**
A: Only if your workload actually has multiple threads allocating concurrently — that's exactly what Server GC's per-core heaps are built to parallelize. Measured directly in this chapter, on a 22-logical-processor machine, Server GC was slightly *slower* than Workstation GC for a single-threaded allocation workload, because the extra heaps' overhead had no concurrent allocation to offset it. ASP.NET Core defaults to Server GC because it genuinely has many concurrently-handled requests; a single-threaded console tool usually doesn't.

**Q: Is pre-sizing a `List<T>` or `Dictionary<TKey,TValue>` actually worth doing?**
A: Yes, measurably. This chapter found pre-sizing a million-item `List<int>` roughly 1.9× faster and allocating less than half the memory of letting it grow from empty — every doubling-and-reallocating step a growing collection performs is avoidable GC pressure when the final size is already known or cheaply estimable.
