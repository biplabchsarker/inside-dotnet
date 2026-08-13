# Quiz — Garbage Collection Fundamentals

1. Name the four kinds of GC roots the CLR traces from.
2. Why does .NET's collector compact the heap rather than just sweeping and leaving holes?
3. Does a `GC.Collect(1)` collect Gen 2 objects?
4. What's the difference between a short and a long `WeakReference`, and why does it matter for observing a finalizable object's lifecycle?
5. On a 22-logical-processor machine, this chapter measured Server GC as slower than Workstation GC for one specific kind of workload. What kind, and why?

<details>
<summary>Answers</summary>

1. Local variables/parameters live on a thread's stack, static fields, CPU registers holding a reference, and `GCHandle`s.
2. Because the bump-pointer allocator from Episode 8 can only extend a single contiguous pointer forward — it can't reuse scattered holes left by sweeping alone. Compaction keeps that fast path working by closing every gap.
3. No. `GC.Collect(1)` collects Gen 1 and Gen 0 (everything younger than or equal to generation 1) but leaves Gen 2 completely untouched — only a full collection touches Gen 2.
4. A short `WeakReference` (the default) clears the moment an object is determined unreachable, even before its finalizer runs. A long `WeakReference` (`trackResurrection: true`) stays alive through finalization, which is what lets you observe the real two-collection lifecycle: alive-but-queued after the first collect, gone only after the finalizer runs and a second collect happens.
5. A single-threaded allocation workload — one thread allocating 2,000,000 small objects with nothing else running concurrently. Server GC's per-core-heap parallelism only pays off when multiple threads allocate at the same time; with just one, the extra heaps add overhead with nothing to offset it.

</details>
