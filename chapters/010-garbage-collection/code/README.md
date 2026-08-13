# Chapter 010 — Code Samples

Two projects, matching the Example/Advanced/Performance tiers described in `article.md`.

## `Chapter10.Demo` (Example + Advanced)

Demonstrates:
- Reachability — an object with no root becomes collectible (`WeakReference` proves it, not assumed)
- Roots — a static field keeps an object alive; removing the root (from a returning method) lets it go
- A genuinely surprising, verified nuance: nulling a local and collecting from the SAME still-executing method frame does not reliably free the object — only after that frame returns does it become collectible
- Generational promotion via `GC.GetGeneration` — the same surviving object reported as Gen 0, then Gen 1, then Gen 2
- `GC.CollectionCount` per generation, and the difference between `GC.Collect(0)` and a full `GC.Collect()`
- A finalizable object's two-collection lifecycle, using a *long* `WeakReference` to observe it directly

```bash
cd Chapter10.Demo
dotnet run -c Release
```

### Expected output

Exact counts may vary slightly by machine/runtime patch, but the shape is stable:

```
=== Inside .NET: Episode 11 — Garbage Collection Fundamentals demo ===

--- 1. Reachability: no root means collectible ---
  Alive before any collection?  True
  Alive after GC.Collect()?     False

--- 2. Roots: a static field keeps an object alive until the root is removed ---
  Statically-rooted object alive after GC.Collect()?      True
  Same object alive after removing the static root + collecting? False

--- 3. A surprising, verified nuance: nulling a local mid-method doesn't free it — the FRAME has to return ---
  Alive right after nulling the local, collected from the SAME still-executing frame: True
  Alive after that method RETURNED, then collecting again from this outer frame: False

--- 4. Generational promotion: a surviving object moves Gen 0 -> Gen 1 -> Gen 2 ---
  Generation right after allocation:  0
  Generation after one Gen 0 collect: 1
  Generation after one Gen 1 collect: 2
  Generation after a full collect:    2

--- 5. GC.CollectionCount per generation, and GC.Collect(0) vs a full GC.Collect() ---
  Gen 0 collections triggered by 20,000,000 short-lived allocations: 89 (sink=20000000, proves the allocations were real)
  GC.Collect(0): Gen 0 count +1, Gen 2 count +0 (unchanged — Gen 0 doesn't touch Gen 2)
  GC.Collect() (full): Gen 2 count +1 (a full collection does touch every generation)

--- 6. A finalizable object needs two collections, not one ---
  Alive immediately after the FIRST GC.Collect()? True  (queued for finalization, not reclaimed yet)
  Alive after WaitForPendingFinalizers + a SECOND GC.Collect()? False

=== Done ===
```

## `Chapter10.Benchmarks` (Performance)

Real `BenchmarkDotNet` numbers backing this chapter's Performance Notes claims — no estimates, no "should be roughly."

```bash
cd Chapter10.Benchmarks
dotnet run -c Release
```

Takes roughly 5-6 minutes (the GC-mode comparison runs the same workload under two separate job configurations).

### What it measures

1. **`GcModeThroughputBenchmarks`** — the same 2,000,000-small-object allocation workload, run once under Workstation GC and once under Server GC via `Job.WithGcServer(...)`, isolating the throughput trade-off itself rather than any code difference.
2. **`GenerationCollectionCostBenchmarks`** — `GC.Collect(0)` vs. `GC.Collect(1)` vs. a full `GC.Collect(2)`, each forced against a live 50,000-object graph pre-promoted to that generation first.
3. **`PreSizedVsGrowingListBenchmarks`** — a `List<int>` grown from empty vs. one pre-sized to its final capacity up front, 1,000,000 items added either way.

See the results tables in [`../../article.md`](../../article.md#performance-notes) for the measured numbers from this run, including the (deliberately counter-intuitive) result that Server GC measured slower than Workstation GC on this chapter's 22-logical-processor test machine — explained there, not just reported.
