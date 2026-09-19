# Inside .NET — Episode 12: GC Generations & the Large Object Heap

*Part II — Memory*

[Episode 11](../010-garbage-collection/article.md) covered the fundamentals: roots, mark/sweep/compact, the generational hypothesis. It deliberately stopped short of three questions — how the Gen 0/Gen 1 "budget" actually works, how a Gen 0 collection avoids rescanning a huge Gen 2 heap, and what happens to objects too big to move cheaply. This chapter answers all three, with real measurements — including one number I found down to the exact byte.

## The exact byte where an allocation jumps to the LOH

```csharp
var a = new byte[84_975];
var b = new byte[84_976];

Console.WriteLine(GC.GetGeneration(a)); // 0
Console.WriteLine(GC.GetGeneration(b)); // 2
```

One byte flips the answer. The CLR's Large Object Heap threshold is 85,000 bytes of *total object size* — data plus a ~24-byte array header on a 64-bit runtime — not the element count you actually typed. `new byte[84_975]` totals 84,999 bytes and stays on the normal Gen 0 path; `new byte[84_976]` totals exactly 85,000 and gets routed straight to the LOH, which is why `GC.GetGeneration()` reports `2` for it — the LOH is collected together with a full (Gen 2) collection.

## Budgets aren't fixed, they're adaptive

Gen 0 and Gen 1 each have an allocation budget, and it's not a constant — the GC recomputes it after every collection based on survival rate and collection cost. There's no public API that hands you the number directly, but the consequence is observable. This chapter's demo forces a `GC.Collect()` at checkpoints and reads `GC.GetGCMemoryInfo().TotalCommittedBytes`:

```
Baseline:                          TotalCommittedBytes = 221,184
After 2,000,000 allocations:       TotalCommittedBytes = 12,804,096
After 10,000,000 total allocations: TotalCommittedBytes = 16,928,768
```

Committed memory grew sharply under the initial burst, then plateaued — the same allocation pattern repeated afterward didn't force it to keep growing. That's the adaptive budget doing its job: sized once for the workload's real behavior, then left alone.

## The card table: why a Gen 0 collection doesn't rescan Gen 2

If Gen 2 can hold millions of live objects, and a Gen 2 object holding a reference to a Gen 0 object is a legitimate root, how does a "cheap" Gen 0 collection avoid checking all of Gen 2 for exactly that case? The **card table**. Every reference-type store that could create a cross-generational reference passes through a JIT-inserted write barrier, which marks one byte dirty in a small side-structure. A Gen 0 collection scans that (tiny) table and treats only dirty regions as extra roots.

No public API reads the card table's actual bits, so I measured the closest honest proxy instead — holding the *dirtied* subset of a Gen 2 graph fixed at 2,000 objects while growing the total graph:

| Gen2GraphSize | Mean |
|---|---|
| 50,000 | 201.9 μs |
| 500,000 | 637.3 μs |
| 2,000,000 | 1,973.6 μs |

The graph grew 40×; the cost grew under 10×. Sub-linear, not perfectly flat (the measurement itself is noisy — see the full table in the chapter for the caveats), but the shape supports the mechanism: what matters is the size of what *changed* in Gen 2, not the size of Gen 2 itself.

## The LOH isn't compacted by default — and compacting it isn't free either

```
FullCollectWithFragmentation (sweep only): 52.15 μs
FullCollectWithCompactOnce:              4,527.08 μs   (87.41x)
```

Sweeping a fragmented LOH just links freed blocks into a free list — fast. Forcing that same collection to actually compact — sliding ~30 MB of live LOH data together and fixing up every reference — cost 87× more. That's precisely why the LOH isn't compacted by default: paying this cost on every collection, for every service, whether or not the space was actually needed back, would be a bad trade almost all the time. `GCSettings.LargeObjectHeapCompactionMode = CompactOnce` exists for the rare, deliberate case where you do need it — and it resets itself back to `Default` the moment that one collection consumes it.

## The one that didn't cooperate with a clean story

I also compared allocating ~100 MB as ~62,500 small (Gen 0-path) objects vs. 1,000 large (LOH-path) objects. Wall-clock time came out close — close enough that re-running the exact same benchmark flipped which one was faster. That's worth sitting with: at this size, system noise genuinely outweighed the real difference, and quoting a single ratio without checking the error bars would have been dishonest.

What *didn't* move between runs: the collection counts.

| Method | Gen0/1K ops | Gen1/1K ops | Gen2/1K ops |
|---|---|---|---|
| Small objects | 8,086 | – | – |
| Large (LOH) objects | 31,246 | 31,246 | 31,246 |

Same total bytes, similar wall-clock time, wildly different GC pause profile — the large-object approach was triggering full collections roughly four times as often as the small-object approach triggered any collection at all. That's the number that actually matters if you're reasoning about tail latency, not the wall-clock mean.

## Try it yourself

The [companion demo](code/Chapter11.Demo/Program.cs) walks through segment growth, the exact LOH byte threshold, the card-table proxy, LOH fragmentation and compaction, and the Pinned Object Heap, with real printed output. The [benchmarks project](code/Chapter11.Benchmarks/Program.cs) is the source of every number above — run it yourself with `dotnet run -c Release`, and don't be surprised if your own numbers wobble a little between runs too.

*Next: [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/article.md).*
