# Interview Questions — Garbage Collection Fundamentals

**Q1: What makes an object eligible for garbage collection — a reference count reaching zero, or something else?**
A: Reachability from a root, not reference counting. The CLR traces from a fixed set of roots (stack references, static fields, CPU registers, GC handles) through every reachable reference, transitively. Anything not reached this way is garbage — including two objects that only reference each other in a cycle with no path back to a root, a case reference counting cannot reclaim on its own without extra cycle-detection machinery.

**Q2: Why does .NET's garbage collector compact the heap instead of just sweeping and leaving holes?**
A: Because the bump-pointer allocator covered in Episode 8 only knows how to extend a single contiguous pointer forward — it has no mechanism for reusing a scattered hole left by a swept object. Compaction slides every surviving object together, closing every gap, so the fast allocation path keeps working exactly as it did before any garbage existed.

**Q3: Does `GC.Collect(0)` ever touch Gen 2 objects?**
A: No. This chapter's demo confirms it directly: calling `GC.Collect(0)` increments the Gen 0 collection counter while leaving the Gen 2 counter completely unchanged. Only a full, parameterless `GC.Collect()` — or an explicit `GC.Collect(2)` — touches every generation.

**Q4: Why does a finalizable object need two collections before it's actually reclaimed?**
A: The first collection determines the object is unreachable and queues it for finalization rather than reclaiming it immediately — its finalizer hasn't run yet. Only after `GC.WaitForPendingFinalizers()` lets the finalizer execute, followed by a second collection, is the memory actually freed. A *long* `WeakReference` (`trackResurrection: true`) is what lets you observe this directly — the default *short* `WeakReference` clears as soon as the object is unreachable, before its finalizer even runs.

**Q5: Should every application enable Server GC because it sounds like the higher-performance option?**
A: No — measured directly in this chapter, on a 22-logical-processor machine, Server GC was slightly *slower* than Workstation GC for a workload where only one thread allocates. Server GC creates one heap per logical core specifically to let multiple concurrently-allocating threads parallelize — with only one allocating thread, that parallelism has nothing to apply to, and the extra heaps' overhead isn't offset by anything.

**Q6: If nulling a local variable and calling `GC.Collect()` doesn't reliably free an object in the same method, what does actually guarantee it becomes collectible?**
A: The frame that holds the last root to that object has to return. This chapter's demo shows a `WeakReference` still reporting `IsAlive == true` immediately after nulling a local and collecting from within the same still-executing method — and reporting `false` only after that method returns and a fresh collection runs from the caller. The CLR guarantees "unreachable once the holding frame is gone," not "unreachable the instant a variable is reassigned."

**Q7: What's the actual measured benefit of pre-sizing a `List<T>` versus letting it grow?**
A: In this chapter's benchmark, pre-sizing a million-item `List<int>` (`new List<int>(N)`) was roughly 1.9× faster and allocated less than half the memory (3.82 MB vs. 8 MB) compared to letting it grow from an empty default capacity. Every doubling-and-copying reallocation a growing collection performs along the way is itself avoidable GC pressure.
