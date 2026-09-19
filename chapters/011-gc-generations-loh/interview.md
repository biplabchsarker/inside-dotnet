# Interview Questions — GC Generations & the Large Object Heap

**Q1: What exactly decides whether an allocation lands on the generational heap or the Large Object Heap?**
A: Total object size, not element count — an object (commonly, but not only, an array) whose total size, including its header, is 85,000 bytes or more is allocated directly on the LOH instead of entering Gen 0. This chapter verified the exact crossover directly: `new byte[84_975]` reports `GC.GetGeneration() == 0`, while `new byte[84_976]` — one byte more, once the ~24-byte array header is included — reports `GC.GetGeneration() == 2`.

**Q2: Why does `GC.GetGeneration()` report `2` for a Large Object Heap object instead of some separate "generation 3"?**
A: Because the LOH is logically collected together with a full (Gen 2) collection — it's a physically distinct segment, but not a separate generation from a collection-counting perspective. The same is true of Pinned Object Heap allocations, which also report generation 2.

**Q3: How does the CLR avoid rescanning the entire Gen 2 heap every time it needs to run a cheap Gen 0 collection?**
A: The card table and write barrier. Every store of a reference-type value that could create a reference from an older generation into a younger one passes through a JIT-inserted write barrier, which marks one byte "dirty" in a small side-structure (the card table) covering that region of the heap. A Gen 0 (or Gen 1) collection scans the (tiny, relative to the heap) card table and only treats objects in dirty regions as extra roots — clean regions of Gen 2 are never touched.

**Q4: Is the Large Object Heap compacted the same way Gen 2 is?**
A: No — not by default. Because moving a large object during compaction costs proportionally to its size, the LOH is swept (freed blocks become reusable free-list entries) but survivors are not moved, by default. `GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce` forces exactly one compacting pass on the next collection that reaches the LOH, then resets itself back to `Default` — it's a deliberate, one-shot request, not a persistent setting.

**Q5: Why can a service hit an `OutOfMemoryException` on a large allocation even when its total free memory looks more than sufficient?**
A: LOH fragmentation. Because the LOH isn't compacted by default, repeated allocation and freeing of differently-sized large objects can leave a free list made up of many holes, none of which is individually large enough for the next large request — even though the sum of all the holes would be. This chapter measured that fragmentation directly by freeing every other object out of 200 large allocations and observing real, nonzero `FragmentationAfterBytes`.

**Q6: What is the Pinned Object Heap, and what problem does it solve that existed before .NET 5?**
A: Before POH, a long-held pinned object (via `fixed` or `GCHandle.Alloc(..., GCHandleType.Pinned)`) sat as an immovable obstacle inside whatever segment it happened to land in, forcing the GC to work around it during every compaction of that segment. `GC.AllocateArray<T>(count, pinned: true)`, introduced in .NET 5, allocates directly onto a separate Pinned Object Heap instead, so pinned data no longer blocks compaction of the regular generational segments or the LOH.

**Q7: Are Gen 0 and Gen 1 allocation budgets fixed values you could reasonably hard-code assumptions around?**
A: No. The GC recomputes each generation's budget after every collection from the survival rate it just observed and the collection's cost relative to allocation throughput — growing the budget when collections are cheap and survival is low, holding back growth when survival is high (since a bigger budget just means promoting more into Gen 1/Gen 2 before reclaiming anything). This chapter observed the consequence directly via `GC.GetGCMemoryInfo()`: committed heap bytes grew sharply under an allocation burst, then plateaued rather than growing indefinitely under the same repeated pattern.
