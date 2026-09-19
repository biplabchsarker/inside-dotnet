# FAQ — GC Generations & the Large Object Heap

**Q: Is the Gen 0 "budget" a fixed number I can rely on across machines or runs?**
A: No. The GC recomputes each generation's allocation budget after every collection based on how much survived and how expensive the collection was relative to allocation throughput. This chapter's demo shows the consequence directly: `GC.GetGCMemoryInfo().TotalCommittedBytes` grows sharply under a burst of allocation pressure, then plateaus at a stable size — the same workload repeated afterward doesn't force further growth. There's a real tuning knob (`GCgen0size` / `DOTNET_GCgen0size`) if you need to influence the *initial* budget, but the adaptive behavior on top of it is not something you should hard-code assumptions around.

**Q: How does the GC avoid rescanning the entire Gen 2 heap on every single Gen 0 collection?**
A: The card table and write barrier. Any store of a reference-type value that could create a reference from an older generation into a younger one passes through a write barrier the JIT inserts automatically, which marks a byte "dirty" in the card table — a small side-structure, not a heap-wide scan. A Gen 0 collection only needs to check the (tiny) card table for dirty regions and treat objects there as extra roots; clean regions of Gen 2 are never touched.

**Q: What's the exact size threshold for the Large Object Heap?**
A: 85,000 bytes of *total object size*, not element count. This chapter verified the crossover to the exact byte: `new byte[84_975]` reports `GC.GetGeneration() == 0` (Gen 0), while `new byte[84_976]` — one byte more — reports `GC.GetGeneration() == 2`, because its total size including the array's ~24-byte header (on a 64-bit runtime) crosses 85,000.

**Q: Why does an LOH object report generation 2 instead of some separate "generation 3"?**
A: Because the LOH is collected together with a full (Gen 2) collection — it's a physically distinct segment, but not a separate generation from a collection-counting perspective. The same is true for Pinned Object Heap allocations.

**Q: Does the GC compact the Large Object Heap the way it compacts Gen 2?**
A: Not by default. Moving a large object during compaction costs proportionally to its size, so by default the LOH is only swept — freed blocks become reusable free-list entries, but survivors don't move. `GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce` forces exactly one compacting pass on the next collection that reaches the LOH, then resets itself back to `Default` — you have to set it again for another compaction.

**Q: Can I run out of memory even if my process's total free memory looks fine?**
A: Yes, via LOH fragmentation — this chapter reproduced it directly. If the LOH's free list is made up of many small holes from repeated alloc/free of differently-sized large objects, a later large allocation can fail because no single hole is big enough, even though the sum of all the holes would be. `ArrayPool<T>.Shared` or a deliberate, occasional `CompactOnce` are the practical fixes.

**Q: What's the Pinned Object Heap, and do I need to think about it in normal code?**
A: POH, introduced in .NET 5, is a dedicated segment for pinned allocations (`GC.AllocateArray<T>(count, pinned: true)`), so a long-held pin no longer sits as an immovable obstacle inside an otherwise-compactable segment. Most application code never allocates on it directly — it mainly matters if you're writing low-level interop or buffer-pooling code that pins for a meaningful length of time.
