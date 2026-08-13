# Summary — Garbage Collection Fundamentals

**TL;DR:** .NET uses a tracing garbage collector — reachability from a fixed set of roots (stack, statics, CPU registers, GC handles) decides what survives, not reference counting, which correctly reclaims reference cycles reference counting can't handle alone. Every collection is mark (find what's live), sweep (reclaim what isn't), and compact (close the gaps so the bump-pointer allocator from Episode 8 keeps working). The generational hypothesis — most objects die young — is why most collections are cheap: measured directly, a full Gen 2 collection cost 5.84× a Gen 0 collection against the same live graph. A finalizable object needs two collections to actually disappear, and Server GC's real advantage requires concurrently allocating threads — measured on a 22-logical-processor machine, it was actually slower than Workstation GC for a single-threaded workload.

**Key takeaways:**

- The CLR traces reachability from four kinds of roots (stack, statics, registers, GC handles) -- it does not use reference counting, and correctly reclaims unreachable cycles that reference counting alone cannot.
- Mark, sweep, compact is the whole cycle -- compaction specifically exists to keep the bump-pointer allocator from Episode 8 working, since it can't reuse scattered holes.
- A genuinely surprising, verified fact: nulling a local mid-method does not reliably free it within that same still-executing frame -- the real guarantee is "collectible once the holding frame returns."
- `GC.Collect(0)` never touches Gen 2 -- measured directly, only a full `GC.Collect()` does, and a full collection costs 5.84x a Gen 0 collection against the same live object graph.
- A finalizable object needs two collections -- queued for finalization on the first, reclaimed after `WaitForPendingFinalizers` and a second collection -- observable with a *long* `WeakReference` since the default *short* one clears before the finalizer even runs.
- Server GC's advantage requires concurrent allocating threads, not just available cores -- measured slower than Workstation GC on a 22-logical-processor machine for a single-threaded workload. Pre-sizing a growing collection, by contrast, measured ~1.9x faster with under half the allocation -- a genuinely free win.
