# Inside .NET — Episode 8: Object Allocation

*Part II — Memory*

Somewhere between `new SomeClass()` and holding a usable reference, five specific things happen — in a fixed order, at a cost engineered to be almost invisible. This chapter is that sequence, in full.

## The bump, not the search

Forget the mental model of a general-purpose allocator scanning for free blocks. In the common case, `new` does a size check against the current thread's own **allocation context** — a small private slice of the shared Gen 0 segment — and if there's room, hands back the current "next free" address and moves that pointer forward by the object's size. That's it. No free list, no fragmentation search, no lock. This is what [Episode 6](../005-stack-vs-heap/article.md) meant by "heap allocation is also just a pointer bump," made concrete.

## Why every thread gets its own context

If every thread bumped the same shared pointer, every allocation anywhere in the process would need to synchronize against every other one — a global lock taken on the single most frequent operation any managed program performs. Per-thread allocation contexts sidestep that entirely: bumping your own context's pointer needs no coordination with anyone. Coordination only happens on the comparatively rare event of a context running dry and needing a fresh slice of the shared segment.

## The header goes on before your code runs

As [Episode 3 — The CLR](../002-clr/article.md) covered, every heap object carries a method table pointer (its exact runtime type) and a sync block index (the historical hook for `lock`/`Monitor`). The allocator writes both as part of producing the object — your constructor's first line executes against an object that's already fully typed and identifiable.

## Zeroed — but not zeroed *now*

.NET guarantees every new object starts at its default field values: `0`, `null`, `false`. That guarantee is real and unconditional — but the work behind it isn't performed at the moment of your `new`. Freshly committed OS pages arrive already zeroed; pages reclaimed by a collection get re-zeroed by the GC during collection/compaction. The cost is real, it's just relocated off the allocation hot path — which is exactly why the fast path stays as cheap as it does.

## What happens when the tray runs empty

When a thread's allocation context can't satisfy the next allocation, it needs a refill. If the shared Gen 0 segment still has unclaimed space, that's cheap. If it doesn't, the allocation triggers a Gen 0 collection first. This chapter stops right there, deliberately — what the collection itself does is the GC chapters' job, not this one's.

## Two paths this chapter mentions and doesn't cover

Objects at or above roughly 85,000 bytes skip this entire mechanism and go to the Large Object Heap instead — a genuinely different strategy, covered in [Episode 12 — GC Generations & LOH](../011-gc-generations-loh/article.md). Boxing a value type allocates through this exact same fast path; only what happens *afterward* differs, and that's [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md)'s job.

## Construction order: the bug this chapter proves, not just describes

Once memory is allocated, headered, and zeroed, constructors run **base class first, then derived** — enforced by the compiler implicitly chaining to the base constructor. The consequence: while a base constructor's body is executing, every derived-class field is still at its zeroed default. If that base constructor calls a `virtual` method the derived class overrides, the override runs against an object whose derived-specific state doesn't exist yet.

```csharp
abstract class RiskyBase
{
    protected RiskyBase() => Console.WriteLine(Describe()); // runs BEFORE derived fields are set
    public abstract string Describe();
}

sealed class RiskyDerived : RiskyBase
{
    private readonly string _label;
    public RiskyDerived(string label) => _label = label;
    public override string Describe() => _label ?? "(uninitialized)";
}
```

Run it, and the base constructor's call prints `"(uninitialized)"` — not `"ready"`. The fix is straightforward once you see the mechanism: don't call virtual members from a constructor unless you're certain no override could ever depend on derived-only state.

## Try it yourself

The [companion demo](code/Chapter07.Demo/Program.cs) measures real allocation throughput via `GC.GetAllocatedBytesForCurrentThread()`, then proves both the construction order and the virtual-call gotcha with actual printed output, not just an explanation.

*Next: [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md)*
