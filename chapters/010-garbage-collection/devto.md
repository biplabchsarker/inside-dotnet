---
title: Inside .NET — Episode 11: Garbage Collection Fundamentals
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 10 — Strings & Interning](../009-strings-interning/article.md).*

```csharp
static bool Check()
{
    object obj = new byte[16];
    var weakRef = new WeakReference(obj);
    obj = null!;
    GC.Collect();
    return weakRef.IsAlive; // ... True??
}
```

I expected `False`. It printed `True`. This chapter is the mechanism behind why, plus real `BenchmarkDotNet` numbers on two other things I assumed instead of measured.

## Reachability, not reference counting

The CLR traces from a fixed set of roots (stack, statics, CPU registers, GC handles) through every reachable reference — anything not reached is garbage, even a reference cycle nothing external points to (the case pure reference counting can't clean up alone). Every collection is mark → sweep → compact; compaction exists because the bump-pointer allocator from Episode 8 can't reuse a scattered hole.

## The frame-lifetime nuance, verified three ways

Nulling a local and calling `GC.Collect()` in the *same still-executing method* doesn't reliably free the object — I proved this with `WeakReference`, `GC.GetTotalMemory`, and repeated calls to rule out JIT tiering as the explanation. It's only once that method **returns**, and a fresh collection runs from the caller, that the object actually becomes collectible. The CLR's real guarantee is "unreachable once the holding frame is gone," not "unreachable the instant you reassign a variable."

## The generational hypothesis, quantified

```
CollectGen0 (baseline):  94.50 µs
CollectGen1:             98.15 µs  (1.05x)
CollectGen2 (full):     544.38 µs  (5.84x)
```

A full collection costs **5.84x** a Gen 0 collection, against the identical live object graph — the measured payoff of "most objects die young."

## Server GC isn't automatically faster

```
Workstation:  9.998 ms
Server:      10.934 ms
```

Same 2,000,000-object single-threaded allocation workload, on a 22-logical-processor machine — Server GC was *slower*. Its per-core heaps exist to parallelize concurrent allocation across threads; with one thread allocating, there's nothing to parallelize and the extra heaps just add overhead.

## The actual free win

```
GrowingList (empty start):     3.062 ms, 8 MB
PreSizedList (capacity known): 1.596 ms, 3.82 MB
```

~1.9x faster, under half the allocation — pre-sizing a collection when you know (or can estimate) the final size has no downside.

## Try it yourself

```bash
cd chapters/010-garbage-collection/code/Chapter10.Demo
dotnet run -c Release
```

The [demo](code/Chapter10.Demo/Program.cs) proves reachability, the frame-lifetime nuance, generational promotion, and the two-collection finalization lifecycle with real output. The [benchmarks project](code/Chapter10.Benchmarks/Program.cs) backs every number here — `dotnet run -c Release` reproduces them on your own machine.

*Next: [Episode 12 — GC Generations & the Large Object Heap](../011-gc-generations-loh/article.md).*
