---
title: Inside .NET — Episode 8: Object Allocation
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/article.md).*

```csharp
abstract class RiskyBase
{
    protected RiskyBase() => Console.WriteLine(Describe());
    public abstract string Describe();
}

sealed class RiskyDerived : RiskyBase
{
    private readonly string _label;
    public RiskyDerived(string label) => _label = label;
    public override string Describe() => _label ?? "(uninitialized)";
}

new RiskyDerived("ready"); // prints "(uninitialized)", not "ready"
```

If that output surprises you, this chapter is the mechanism behind why — and the allocation mechanics that make it possible in the first place.

## The fast path is a pointer bump, not a search

Every `new` you write, in the common case, resolves to: check if the current thread's **allocation context** (a small private slice of the shared Gen 0 segment) has room, and if so, hand back the current address and move the pointer forward by the object's size. No free-list search, no fragmentation handling, no lock. This is [Episode 6](../005-stack-vs-heap/article.md)'s "heap allocation is also just a pointer bump" claim, in full.

## Why per-thread contexts exist

One shared pointer would mean one global lock, taken on every single allocation across every thread in the process. Per-thread contexts let each thread bump its own pointer with zero coordination — coordination only happens when a context runs dry and needs a fresh slice from the shared segment.

## What gets written, and when

1. Reserve N bytes (bump the pointer).
2. Write the method table pointer (exact runtime type — recap from [Episode 3 — The CLR](../002-clr/article.md)).
3. Write the sync block index.
4. Fields already read as zero/null/false — nothing to write.
5. Constructors run.

Step 4 is the detail people miss: zeroing isn't performed *at* allocation time. Fresh OS pages arrive zeroed; the GC re-zeroes reclaimed memory during collection. The guarantee is unconditional; the work is relocated off the hot path.

## The trigger for a Gen 0 collection

Not a timer. Not a periodic check. Specifically: a thread's allocation context runs out, and there's no unclaimed space left in the shared segment to refill from. That's the boundary — what the collection itself does belongs to the GC chapters ahead, deliberately not here.

## Two things this chapter mentions and doesn't cover

- Objects ≥ 85,000 bytes skip this path for the **Large Object Heap** — [Episode 12](../011-gc-generations-loh/article.md)'s job.
- **Boxing** allocates through this exact same mechanism — [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md)'s job.

## Construction order — the gotcha, proven

Constructors run base class first, then derived — the compiler enforces this by implicitly chaining to the base constructor. So when `RiskyBase`'s constructor calls the virtual `Describe()`, `RiskyDerived`'s `_label` field hasn't been assigned yet — it's still at its zeroed default (`null`). That's why the output above says `"(uninitialized)"`.

## Try it yourself

The [companion demo](code/Chapter07.Demo/Program.cs) measures real allocation throughput and proves this exact gotcha with real output:

```bash
cd chapters/007-object-allocation/code/Chapter07.Demo
dotnet run
```

*Next: [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md).*
