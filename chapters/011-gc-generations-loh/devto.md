---
title: Inside .NET — Episode 12: GC Generations & the Large Object Heap
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/article.md).*

```csharp
var a = new byte[84_975];
var b = new byte[84_976];

Console.WriteLine(GC.GetGeneration(a)); // 0
Console.WriteLine(GC.GetGeneration(b)); // 2 — one byte more!
```

One byte flips the answer, because the LOH's 85,000-byte threshold compares *total object size* — data plus a ~24-byte array header — not the element count you typed. This chapter is the mechanism behind that, plus the two questions Episode 11 deliberately deferred: how the Gen 0/Gen 1 budget actually adapts, and how a Gen 0 collection avoids rescanning a huge Gen 2 heap.

## Budgets adapt — observed via GC.GetGCMemoryInfo()

```
Baseline:                           TotalCommittedBytes = 221,184
After 2,000,000 allocations:        TotalCommittedBytes = 12,804,096
After 10,000,000 total allocations: TotalCommittedBytes = 16,928,768
```

Grows under pressure, then plateaus — the GC re-sizes for the workload once, rather than growing indefinitely under a repeated pattern.

## The card table: a proxy measurement, honestly labeled

No public API reads the card table's dirty bits, so I measured the closest observable consequence: hold the *dirtied* subset of a Gen 2 graph fixed at 2,000 objects, vary the total graph size.

```
Gen2GraphSize =    50,000 -> 201.9 μs
Gen2GraphSize =   500,000 -> 637.3 μs
Gen2GraphSize = 2,000,000 -> 1,973.6 μs
```

40× graph growth, under 10× cost growth — sub-linear, not flat (this measurement is noisy; see the full chapter for the caveats), but consistent with the card table sparing the untouched majority of Gen 2 from a rescan.

## LOH compaction: real, effective, and not free

```
FullCollectWithFragmentation (sweep only): 52.15 μs
FullCollectWithCompactOnce:              4,527.08 μs   (87.41x)
```

`GCSettings.LargeObjectHeapCompactionMode = CompactOnce` forces exactly one compacting pass on the LOH, then resets itself to `Default`. 87× the cost of a plain sweep — which is exactly why it isn't the default behavior.

## LOH vs. small-object allocation: the wall-clock numbers didn't agree with themselves

```
AllocateManySmallObjects (baseline): 3.258 ms
AllocateFewLargeObjects:             3.589 ms   (1.11x)
```

A repeat run flipped which one was faster — genuine run-to-run noise at this size. What stayed rock-stable across runs was the *collection count*:

```
Small objects: ~8,086 Gen0-only collections / 1,000 runs
Large objects: ~31,246 FULL (Gen0+1+2) collections / 1,000 runs
```

Same bytes, similar wall-clock time, roughly 4× the full-collection frequency for the large-object approach. That's the number that matters for tail latency, not the noisy mean.

## Try it yourself

```bash
cd chapters/011-gc-generations-loh/code/Chapter11.Demo
dotnet run -c Release
```

The [demo](code/Chapter11.Demo/Program.cs) proves the exact LOH byte threshold, segment growth, the card-table proxy, LOH fragmentation/compaction, and the Pinned Object Heap with real output. The [benchmarks project](code/Chapter11.Benchmarks/Program.cs) backs every number here — `dotnet run -c Release` reproduces them (with some run-to-run wobble) on your own machine.

*Next: [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/article.md).*
