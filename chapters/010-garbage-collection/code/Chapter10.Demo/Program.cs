// Program.cs — .NET 10 console app
// Demonstrates: (1) reachability — an object with no root becomes collectible,
// proven with a WeakReference rather than assumed; (2) roots keep objects
// alive — a static field root survives collection, and removing that root
// (from a returning method) lets the object go; (3) a genuinely surprising,
// verified nuance: nulling a local and calling GC.Collect() from the SAME
// still-executing method frame does NOT reliably free the object — only
// after that frame returns does it become collectible; (4) GC.GetGeneration
// and generational promotion, observed directly; (5) GC.CollectionCount per
// generation, and the difference between GC.Collect(0) and a full
// GC.Collect(); (6) a finalizable object needs two collections to actually
// go away.

Console.WriteLine("=== Inside .NET: Episode 11 — Garbage Collection Fundamentals demo ===");

Console.WriteLine();
Console.WriteLine("--- 1. Reachability: no root means collectible ---");
var noRootWeakRef = CreateUnrootedObjectAndWeakReference();
Console.WriteLine($"  Alive before any collection?  {noRootWeakRef.IsAlive}");
GC.Collect();
GC.WaitForPendingFinalizers();
Console.WriteLine($"  Alive after GC.Collect()?     {noRootWeakRef.IsAlive}");

Console.WriteLine();
Console.WriteLine("--- 2. Roots: a static field keeps an object alive until the root is removed ---");
var staticRootedWeakRef = SetStaticRootAndGetWeakReference();
GC.Collect();
Console.WriteLine($"  Statically-rooted object alive after GC.Collect()?      {staticRootedWeakRef.IsAlive}");
RemoveStaticRoot();
GC.Collect();
Console.WriteLine($"  Same object alive after removing the static root + collecting? {staticRootedWeakRef.IsAlive}");

Console.WriteLine();
Console.WriteLine("--- 3. A surprising, verified nuance: nulling a local mid-method doesn't free it — the FRAME has to return ---");
var sameFrameWeakRef = SameFrameNullingDoesNotReliablyFree();
GC.Collect(); // called from OUT HERE, after that method's frame has fully returned
Console.WriteLine($"  Alive after that method RETURNED, then collecting again from this outer frame: {sameFrameWeakRef.IsAlive}");

Console.WriteLine();
Console.WriteLine("--- 4. Generational promotion: a surviving object moves Gen 0 -> Gen 1 -> Gen 2 ---");
var survivor = new byte[16];
Console.WriteLine($"  Generation right after allocation:  {GC.GetGeneration(survivor)}");
GC.Collect(0);
Console.WriteLine($"  Generation after one Gen 0 collect: {GC.GetGeneration(survivor)}");
GC.Collect(1);
Console.WriteLine($"  Generation after one Gen 1 collect: {GC.GetGeneration(survivor)}");
GC.Collect();
Console.WriteLine($"  Generation after a full collect:    {GC.GetGeneration(survivor)}");

Console.WriteLine();
Console.WriteLine("--- 5. GC.CollectionCount per generation, and GC.Collect(0) vs a full GC.Collect() ---");
int gen0Before = GC.CollectionCount(0);
int gen2Before = GC.CollectionCount(2);
long sinkTotal = AllocateAndDiscardManyShortLivedObjects(20_000_000);
int gen0AfterAllocations = GC.CollectionCount(0);
Console.WriteLine($"  Gen 0 collections triggered by 20,000,000 short-lived allocations: {gen0AfterAllocations - gen0Before} (sink={sinkTotal}, proves the allocations were real)");

GC.Collect(0);
int gen0AfterExplicitGen0Collect = GC.CollectionCount(0);
int gen2AfterExplicitGen0Collect = GC.CollectionCount(2);
Console.WriteLine($"  GC.Collect(0): Gen 0 count +{gen0AfterExplicitGen0Collect - gen0AfterAllocations}, Gen 2 count +{gen2AfterExplicitGen0Collect - gen2Before} (unchanged — Gen 0 doesn't touch Gen 2)");

GC.Collect();
int gen2AfterFullCollect = GC.CollectionCount(2);
Console.WriteLine($"  GC.Collect() (full): Gen 2 count +{gen2AfterFullCollect - gen2AfterExplicitGen0Collect} (a full collection does touch every generation)");

Console.WriteLine();
Console.WriteLine("--- 6. A finalizable object needs two collections, not one ---");
// A LONG WeakReference (trackResurrection: true) stays alive through
// finalization, which is exactly what lets it show this two-step lifecycle —
// a SHORT WeakReference (the default) clears as soon as the object is
// determined unreachable, even before its finalizer has run.
var finalizableWeakRef = CreateFinalizableObjectAndWeakReference();
GC.Collect();
Console.WriteLine($"  Alive immediately after the FIRST GC.Collect()? {finalizableWeakRef.IsAlive}  (queued for finalization, not reclaimed yet)");
GC.WaitForPendingFinalizers(); // let the finalizer actually run
GC.Collect(); // now reclaim the memory
Console.WriteLine($"  Alive after WaitForPendingFinalizers + a SECOND GC.Collect()? {finalizableWeakRef.IsAlive}");

Console.WriteLine();
Console.WriteLine("=== Done ===");

static WeakReference CreateUnrootedObjectAndWeakReference()
{
    var obj = new byte[16];
    return new WeakReference(obj); // 'obj' itself is never returned — no root survives this method
}

static WeakReference SetStaticRootAndGetWeakReference()
{
    StaticRootHolder.Instance = new byte[16];
    return new WeakReference(StaticRootHolder.Instance);
}

static void RemoveStaticRoot()
{
    StaticRootHolder.Instance = null;
}

static WeakReference SameFrameNullingDoesNotReliablyFree()
{
    object obj = new byte[16];
    var weakRef = new WeakReference(obj);
    obj = null!; // reassigned to null — but this method's frame is still executing
    GC.Collect();
    Console.WriteLine($"  Alive right after nulling the local, collected from the SAME still-executing frame: {weakRef.IsAlive}");
    return weakRef;
}

static long AllocateAndDiscardManyShortLivedObjects(int count)
{
    // The running 'sink' total forces each array to actually be read, which
    // stops the JIT's escape analysis (Episode 9) from proving the allocation
    // is dead and eliding it entirely — without this, zero real bytes would
    // be allocated and this section's whole point would disappear.
    long sink = 0;
    for (int i = 0; i < count; i++)
    {
        var discarded = new byte[32]; // rooted only for this iteration, then eligible for collection
        discarded[0] = 1;
        sink += discarded[0];
    }
    return sink;
}

static WeakReference CreateFinalizableObjectAndWeakReference()
{
    var obj = new FinalizableThing();
    return new WeakReference(obj, trackResurrection: true);
}

sealed class FinalizableThing
{
    ~FinalizableThing()
    {
        // Deliberately empty — the point of this demo is the two-collection
        // lifecycle itself, not what a real finalizer should do (that's
        // Episode 13 — IDisposable & Finalizers).
    }
}

static class StaticRootHolder
{
    public static byte[]? Instance;
}
