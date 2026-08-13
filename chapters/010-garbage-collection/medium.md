# Inside .NET — Episode 11: Garbage Collection Fundamentals

*Part II — Memory*

Episodes 8 and 9 covered how the CLR hands out memory — fast, bump-pointer allocation that assumes there's always room at the end of the segment. That only works if something reclaims the space objects no longer need. This chapter is the mechanism behind that reclaim, plus real numbers on what it costs — including two results that genuinely surprised me while measuring them.

## Reachability, not reference counting

The CLR doesn't count references the way Python or classic COM do. Instead, it traces from a fixed set of **roots** — stack variables, static fields, CPU registers, GC handles — through every reference reachable from them, transitively. Anything not reached is garbage, full stop, even a pair of objects that only reference *each other* in a cycle nothing else points to. That's the case pure reference counting can't clean up without extra machinery bolted on; a tracing collector handles it for free.

Every collection is the same three phases: **mark** (find what's live), **sweep** (reclaim what isn't), **compact** (slide survivors together, closing the gaps). Compaction specifically exists because the bump-pointer allocator from Episode 8 can only extend a single pointer forward — it has no idea what to do with a scattered hole.

## The surprise I didn't expect: nulling a local isn't enough

Here's the thing most explanations of "unreachable objects get collected" gloss over. I wrote this, expecting it to print `False`:

```csharp
static bool Check()
{
    object obj = new byte[16];
    var weakRef = new WeakReference(obj);
    obj = null!;
    GC.Collect();
    return weakRef.IsAlive; // still True!
}
```

It printed `True`. The object was still considered reachable, in that same still-executing frame, *even after I nulled the only variable pointing at it and forced a collection*. Only when I moved the check to a separate method — called *after* `Check()`-equivalent code had actually returned — did it report `False`. The CLR's real guarantee is "unreachable once the frame holding the last root returns," not "unreachable the instant you reassign a variable." I verified this three separate ways before believing it myself.

## The generational hypothesis, quantified

Most objects die young — that's the whole premise the GC is built around, and it's why most collections barely cost anything. I proved it directly: I built a live 50,000-object graph, promoted copies of it to Gen 0, Gen 1, and Gen 2, and forced a collection at each level.

| Method | Mean | Ratio |
|---|---|---|
| `CollectGen0` (baseline) | 94.50 µs | 1.00 |
| `CollectGen1` | 98.15 µs | 1.05 |
| `CollectGen2` (full) | 544.38 µs | 5.84× |

A full collection costs **5.84×** a Gen 0 collection against the identical graph — because it's the one case where the GC has to walk the *entire* live object set instead of just the newest slice. Most of the time, it never has to pay that price.

## Server GC isn't automatically the fast one

This is the other measured surprise. On a 22-logical-processor machine, I ran the same single-threaded, 2,000,000-object allocation workload under both GC modes:

| Job | Mean | Allocated |
|---|---|---|
| Workstation | 9.998 ms | 167.85 MB |
| Server | 10.934 ms | 167.85 MB |

Server GC was *slower*. Not a measurement fluke — Server GC creates one heap per logical core specifically so multiple concurrently-allocating threads can parallelize. With exactly one thread allocating, there's no parallelism for those extra heaps to provide, and their overhead shows up with nothing to offset it. ASP.NET Core defaults to Server GC because it has many concurrent requests. A single-threaded batch tool usually doesn't, and shouldn't just flip the switch because it "sounds faster."

## The genuinely free win: pre-sizing collections

```csharp
var list = new List<int>(1_000_000); // vs. new List<int>() growing from empty
```

| Method | Mean | Allocated |
|---|---|---|
| `GrowingList` (baseline) | 3.062 ms | 8 MB |
| `PreSizedList` | 1.596 ms | 3.82 MB |

Pre-sizing was **~1.9× faster** and allocated **less than half** the memory. Unlike the GC-mode result above, this one has no trade-off — if you know (or can cheaply estimate) the final size, there's no reason not to pass it in.

## Try it yourself

The [companion demo](code/Chapter10.Demo/Program.cs) walks through reachability, the frame-lifetime nuance above, generational promotion, and the two-collection finalization lifecycle with real printed output. The [benchmarks project](code/Chapter10.Benchmarks/Program.cs) is the source of every number in this article — run it yourself with `dotnet run -c Release`.

*Next: [Episode 12 — GC Generations & the Large Object Heap](../011-gc-generations-loh/article.md).*
