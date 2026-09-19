# Chapter 011 — Code Samples

Two projects, matching the Example/Advanced/Performance tiers described in `article.md`.

## `Chapter11.Demo` (Example + Advanced)

Demonstrates:
- Heap/segment growth under sustained allocation pressure, observed via `GC.GetGCMemoryInfo()` — committed bytes growing sharply under an allocation burst, then plateauing
- The exact size-threshold crossover for the Large Object Heap, verified to the byte: `new byte[84_975]` is Gen 0, `new byte[84_976]` is already on the LOH
- A card table / write barrier proxy measurement — Gen 0 collection cost against a large, mutated Gen 2 graph, honestly labeled as an indirect proxy (no public API reads the card table directly)
- LOH fragmentation building up from alternating alloc/free, then being driven down by `GCSettings.LargeObjectHeapCompactionMode.CompactOnce`, observed via `GenerationInfo.FragmentationAfterBytes`
- The Pinned Object Heap (.NET 5+) via `GC.AllocateArray<T>(pinned: true)`

```bash
cd Chapter11.Demo
dotnet run -c Release
```

### Expected output

Exact byte counts and timings may vary slightly by machine/runtime patch, but the shape is stable:

```
=== Inside .NET: Episode 12 — GC Generations & the Large Object Heap demo ===

--- 1. Heap/segment growth under sustained allocation pressure ---
  (A GC.Collect() is forced at each checkpoint purely to get a stable, comparable snapshot for
   this demo via GC.GetGCMemoryInfo() — not a recommendation to call GC.Collect() in real code.)
  Baseline, before any bulk allocation:
    TotalCommittedBytes = 221,184, HeapSizeBytes = 50,624
    GenerationInfo[0]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[1]: SizeAfterBytes=42,440, FragmentationAfterBytes=440
    GenerationInfo[2]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[3]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[4]: SizeAfterBytes=8,184, FragmentationAfterBytes=0
  After 2,000,000 short-lived 64-byte allocations:
    TotalCommittedBytes = 12,804,096, HeapSizeBytes = 56,088
    GenerationInfo[0]: SizeAfterBytes=560, FragmentationAfterBytes=456
    GenerationInfo[1]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[2]: SizeAfterBytes=47,344, FragmentationAfterBytes=0
    GenerationInfo[3]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[4]: SizeAfterBytes=8,184, FragmentationAfterBytes=0
  After 10,000,000 total short-lived 64-byte allocations:
    TotalCommittedBytes = 16,928,768, HeapSizeBytes = 56,088
    GenerationInfo[0]: SizeAfterBytes=560, FragmentationAfterBytes=456
    GenerationInfo[1]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[2]: SizeAfterBytes=47,344, FragmentationAfterBytes=0
    GenerationInfo[3]: SizeAfterBytes=0, FragmentationAfterBytes=0
    GenerationInfo[4]: SizeAfterBytes=8,184, FragmentationAfterBytes=0
  Gen 0 collections triggered along the way: 73, Gen 1: 3, Gen 2: 3

--- 2. The size-threshold branch: same 'new byte[N]' call, two different destinations ---
  new byte[1,000]   -> GC.GetGeneration = 0 (Gen 0 — the normal ephemeral path)
  new byte[100,000] -> GC.GetGeneration = 2 (LOH objects report as Gen 2 — the LOH is collected together with a full collection)
  LOH threshold used by the CLR: 85,000 bytes (a documented constant, not derived here)

--- 3. Card table / write barrier PROXY: Gen 0 collection cost vs. a large, mutated Gen 2 graph ---
  (No public API exposes the card table directly — this measures a downstream, honestly-labeled consequence instead.)
  Live Gen 2 graph size: 500,000 objects
  Gen 0 collection, no cross-gen references from Gen 2:      1.12 ms
  Gen 0 collection, AFTER 500,000 fresh Gen2->Gen0 writes: 0.45 ms
  Both single-shot samples land in the same low-single-digit-millisecond range — this is a
  rough, single-run illustration (see the rigorous, statistically-sound version in
  Chapter11.Benchmarks' CardTableProxyBenchmarks, which varies the Gen 2 graph size itself).
  If a Gen 0 collection had to rescan the entire live Gen 2 graph on every run instead of just
  dirtied cards, this cost would scale with Gen 2 graph size — the benchmark is what confirms it doesn't.

--- 4. LOH fragmentation, and clearing it with GCSettings.LargeObjectHeapCompactionMode ---
  After freeing every other 200,000-byte object (no compaction yet):
    LOH FragmentationAfterBytes = 23,808,864
  After GCSettings.LargeObjectHeapCompactionMode = CompactOnce + one more GC.Collect():
    LOH FragmentationAfterBytes = 3,264
  CompactOnce is a one-shot switch — it resets itself to Default after the next collection;
  the LOH is NOT compacted on every collection by default, only when this is explicitly requested.

--- 5. The Pinned Object Heap (POH), introduced in .NET 5 ---
  GC.AllocateArray<byte>(4096, pinned: true) -> GC.GetGeneration = 2
  (POH objects are collected alongside Gen 2/LOH and never need a 'fixed' statement to stay put.)

=== Done ===
```

`GenerationInfo` is indexed `[Gen 0, Gen 1, Gen 2, LOH, POH]` per `System.GCGenerationInfo`'s documented ordering — index 3 (LOH) and index 4 (POH) are visibly nonzero even at baseline because the .NET host itself allocates some pinned/large data during startup.

## `Chapter11.Benchmarks` (Performance)

Real `BenchmarkDotNet` numbers backing this chapter's Performance Notes claims — no estimates, no "should be roughly."

```bash
cd Chapter11.Benchmarks
dotnet run -c Release
```

Takes roughly 10-15 minutes — the LOH-heavy benchmarks and the card-table proxy's three parameterized graph sizes each rebuild multi-million-object graphs per iteration.

### What it measures

1. **`LohVsSmallObjectBenchmarks`** — the same ~100 MB total allocated either as ~62,500 small (1,600-byte, Gen 0-path) objects or as 1,000 large (100,000-byte, LOH-path) objects, isolating the LOH's own allocation cost at an equal total-byte size.
2. **`LohFragmentationCompactionBenchmarks`** — the cost of a full `GC.Collect()` against a fragmented LOH (every other one of 400 large objects freed) vs. the same collection forced to compact via `GCSettings.LargeObjectHeapCompactionMode.CompactOnce`, rebuilt fresh every iteration since `CompactOnce` consumes itself.
3. **`CardTableProxyBenchmarks`** — `GC.Collect(0)` forced against Gen 2 graphs of 50,000 / 500,000 / 2,000,000 objects, with the *dirtied* (cross-gen-referencing) subset held fixed at 2,000 objects regardless of total graph size — isolating whether Gen 0 collection cost tracks the size of what changed in Gen 2, or the size of Gen 2 itself.

See the results tables in [`../../article.md`](../../article.md#performance-notes) for the measured numbers from this run, including the more interesting finding buried under benchmark 1's wall-clock numbers: allocating the same ~100 MB total as 1,000 large (LOH-path) objects instead of ~62,500 small (Gen 0-path) ones triggers roughly 31 full (Gen 0+1+2) collections per 1,000 invocations, against roughly 8 Gen-0-only collections for the small-object approach — a real difference in GC pause profile that wall-clock time alone doesn't surface.
