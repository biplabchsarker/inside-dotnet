# Exercises — Garbage Collection Fundamentals

1. **Prove the GC reclaims a reference cycle that reference counting alone couldn't.** Define two small classes, `A` and `B`, where `A` has a field of type `B` and `B` has a field of type `A`. Create one instance of each, point them at each other, then make sure nothing else references either one (put the creation inside a helper method that returns, per this chapter's frame-lifetime lesson). Take a `WeakReference` to one of them before it goes out of scope, force a full `GC.Collect()`, and confirm `IsAlive` is `false`. Explain, in a sentence, why a pure reference-counting collector (with no extra cycle detector) would never have reclaimed this pair.

2. **Reproduce the frame-lifetime nuance with a twist.** Starting from `SameFrameNullingDoesNotReliablyFree` in [`code/Chapter10.Demo/Program.cs`](code/Chapter10.Demo/Program.cs), add a call to some other, unrelated method (e.g., `Console.WriteLine("...")`) *between* nulling the local and calling `GC.Collect()`, still within the same method. Does that change whether the object is reported alive in that same frame? Then try moving the `GC.Collect()` call itself into a second helper method that the first one calls, rather than keeping it in the same frame — record what changes and explain why, in terms of which frame is "still executing" at the moment of the collection.

3. **Measure how the Gen 0 vs. full-collection cost gap changes with graph size.** Starting from `GenerationCollectionCostBenchmarks` in [`code/Chapter10.Benchmarks/Program.cs`](code/Chapter10.Benchmarks/Program.cs), change `LiveObjectCount` from 50,000 to 5,000 and then to 500,000, re-running with `dotnet run -c Release` each time. Record the `CollectGen2`-to-`CollectGen0` ratio at each size. Does the ratio grow, shrink, or stay roughly flat as the live graph gets bigger — and what does that tell you about what a full collection's cost actually scales with?

4. **Stretch — watch `% Time in GC` on a real workload with `dotnet-counters`.** Write a small console loop that allocates continuously (short-lived objects, similar to this chapter's `AllocateAndDiscardManyShortLivedObjects`) for 30+ seconds, and attach `dotnet-counters monitor --counters System.Runtime` to the running process. Record the steady-state `% Time in GC` and `Gen 0 Size`. Then add a `List<T>` that keeps every 100th allocated object alive for the whole run (simulating a slow, genuine memory growth pattern) and re-run — confirm `Gen 2 Size` starts climbing where it didn't before, and connect that back to the generational promotion path from this chapter's "Under the Hood."

## Challenge

**Predict the output before running it.**

```csharp
Console.WriteLine(Check());

static bool Check()
{
    object obj = new byte[16];
    var weakRef = new WeakReference(obj);
    obj = null!;
    GC.Collect();
    return weakRef.IsAlive; // checked HERE, same frame
}
```

versus:

```csharp
var weakRef = Create();
GC.Collect();
Console.WriteLine(weakRef.IsAlive); // checked HERE, in the CALLER, after Create() returned

static WeakReference Create()
{
    object obj = new byte[16];
    var weakRef = new WeakReference(obj);
    obj = null!;
    return weakRef;
}
```

Before running either: do both print the same value? Most people assume they must, since both null out `obj` before any collection happens. Write down, in terms of *which method's frame is still executing at the moment `GC.Collect()` runs*, why one of these reliably prints `True` and the other reliably prints `False` — even though the sequence of operations on `obj` and `weakRef` looks the same in both.
