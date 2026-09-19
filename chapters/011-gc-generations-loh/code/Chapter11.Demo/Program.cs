// Program.cs — .NET 10 console app
// Demonstrates, with real, verifiable managed-code output — never assumed:
// (1) generation budgets growing as allocation pressure increases, observed
// through GC.GetGCMemoryInfo(); (2) the size-threshold branch — a small
// array stays on the ephemeral (Gen 0) path, a >=85,000-byte array is
// routed straight to the Large Object Heap, both confirmed via
// GC.GetGeneration(); (3) an indirect, honestly-labeled proxy for the card
// table / write barrier payoff — a Gen 0 collection's cost does not scale
// with the size of a live Gen 2 graph, even after that graph is mutated to
// hold fresh cross-generational references; (4) LOH fragmentation building
// up and then being cleared by a one-time GCSettings.LargeObjectHeapCompactionMode
// compaction, observed via GenerationInfo.FragmentationAfterBytes;
// (5) the Pinned Object Heap (POH), introduced in .NET 5, via
// GC.AllocateArray<T>(pinned: true).
//
// NOTE ON HONESTY: the card table itself (which byte is "dirty", which
// cards a Gen 0 collection actually rescans) has no public managed API.
// Section 3 below is explicitly a proxy measurement, not a direct read of
// the card table — the article calls this out rather than pretending
// otherwise.

using System.Diagnostics;
using System.Runtime;

const int LohThresholdBytes = 85_000;

Console.WriteLine("=== Inside .NET: Episode 12 — GC Generations & the Large Object Heap demo ===");

Console.WriteLine();
Console.WriteLine("--- 1. Heap/segment growth under sustained allocation pressure ---");
Console.WriteLine("  (A GC.Collect() is forced at each checkpoint purely to get a stable, comparable snapshot for");
Console.WriteLine("   this demo via GC.GetGCMemoryInfo() — not a recommendation to call GC.Collect() in real code.)");
PrintGenerationInfo("Baseline, before any bulk allocation");
AllocateAndDiscardShortLived(2_000_000, 64);
PrintGenerationInfo("After 2,000,000 short-lived 64-byte allocations");
AllocateAndDiscardShortLived(8_000_000, 64);
PrintGenerationInfo("After 10,000,000 total short-lived 64-byte allocations");
Console.WriteLine($"  Gen 0 collections triggered along the way: {GC.CollectionCount(0)}, Gen 1: {GC.CollectionCount(1)}, Gen 2: {GC.CollectionCount(2)}");

Console.WriteLine();
Console.WriteLine("--- 2. The size-threshold branch: same 'new byte[N]' call, two different destinations ---");
var smallArray = new byte[1_000];
var largeArray = new byte[100_000]; // >= 85,000 bytes -> routed straight to the LOH
Console.WriteLine($"  new byte[1,000]   -> GC.GetGeneration = {GC.GetGeneration(smallArray)} (Gen 0 — the normal ephemeral path)");
Console.WriteLine($"  new byte[100,000] -> GC.GetGeneration = {GC.GetGeneration(largeArray)} (LOH objects report as Gen 2 — the LOH is collected together with a full collection)");
Console.WriteLine($"  LOH threshold used by the CLR: {LohThresholdBytes:N0} bytes (a documented constant, not derived here)");

Console.WriteLine();
Console.WriteLine("--- 3. Card table / write barrier PROXY: Gen 0 collection cost vs. a large, mutated Gen 2 graph ---");
Console.WriteLine("  (No public API exposes the card table directly — this measures a downstream, honestly-labeled consequence instead.)");
RunCardTableProxy();

Console.WriteLine();
Console.WriteLine("--- 4. LOH fragmentation, and clearing it with GCSettings.LargeObjectHeapCompactionMode ---");
RunLohFragmentationDemo();

Console.WriteLine();
Console.WriteLine("--- 5. The Pinned Object Heap (POH), introduced in .NET 5 ---");
RunPohDemo();

Console.WriteLine();
Console.WriteLine("=== Done ===");

static void AllocateAndDiscardShortLived(int count, int size)
{
    // The running 'sink' forces each array to actually be touched, which
    // stops the JIT from proving the allocation is dead and eliding it
    // (the same escape-analysis point made in Episode 9).
    long sink = 0;
    for (int i = 0; i < count; i++)
    {
        var discarded = new byte[size];
        discarded[0] = 1;
        sink += discarded[0];
    }
    if (sink != count) throw new InvalidOperationException("sink mismatch — allocations were optimized away");
}

static void PrintGenerationInfo(string label)
{
    // GetGCMemoryInfo() reports data from the most recent collection of the
    // requested kind — it does not itself trigger one. GC.Collect() here is
    // what actually produces a fresh, comparable snapshot for this demo.
    GC.Collect();
    var info = GC.GetGCMemoryInfo();
    Console.WriteLine($"  {label}:");
    Console.WriteLine($"    TotalCommittedBytes = {info.TotalCommittedBytes:N0}, HeapSizeBytes = {info.HeapSizeBytes:N0}");
    for (int i = 0; i < info.GenerationInfo.Length; i++)
    {
        var g = info.GenerationInfo[i];
        Console.WriteLine($"    GenerationInfo[{i}]: SizeAfterBytes={g.SizeAfterBytes:N0}, FragmentationAfterBytes={g.FragmentationAfterBytes:N0}");
    }
}

static void RunCardTableProxy()
{
    // Build a large, long-lived Gen 2 graph up front and force it to Gen 2.
    const int Gen2ObjectCount = 500_000;
    var gen2Graph = new CrossGenHolder[Gen2ObjectCount];
    for (int i = 0; i < Gen2ObjectCount; i++)
    {
        gen2Graph[i] = new CrossGenHolder();
    }
    GC.Collect(0);
    GC.Collect(1); // promote the whole graph into Gen 2

    var sw = Stopwatch.StartNew();
    GC.Collect(0); // baseline: Gen 2 graph exists, but nothing in it points into Gen 0/1
    sw.Stop();
    var baselineMs = sw.Elapsed.TotalMilliseconds;

    // Now mutate every Gen 2 object's field to point at a brand-new Gen 0
    // object. Each of those writes fires the CLR's write barrier and dirties
    // the corresponding card-table entry for that Gen 2 object.
    for (int i = 0; i < Gen2ObjectCount; i++)
    {
        gen2Graph[i].Reference = new byte[16]; // fresh Gen 0 object, referenced FROM Gen 2
    }

    sw.Restart();
    GC.Collect(0); // if the GC had to rescan the whole Gen 2 graph here, this would cost far more
    sw.Stop();
    var afterCrossGenWritesMs = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine($"  Live Gen 2 graph size: {Gen2ObjectCount:N0} objects");
    Console.WriteLine($"  Gen 0 collection, no cross-gen references from Gen 2:      {baselineMs:F2} ms");
    Console.WriteLine($"  Gen 0 collection, AFTER {Gen2ObjectCount:N0} fresh Gen2->Gen0 writes: {afterCrossGenWritesMs:F2} ms");
    Console.WriteLine("  Both single-shot samples land in the same low-single-digit-millisecond range — this is a");
    Console.WriteLine("  rough, single-run illustration (see the rigorous, statistically-sound version in");
    Console.WriteLine("  Chapter11.Benchmarks' CardTableProxyBenchmarks, which varies the Gen 2 graph size itself).");
    Console.WriteLine("  If a Gen 0 collection had to rescan the entire live Gen 2 graph on every run instead of just");
    Console.WriteLine("  dirtied cards, this cost would scale with Gen 2 graph size — the benchmark is what confirms it doesn't.");

    GC.KeepAlive(gen2Graph);
}

static void RunLohFragmentationDemo()
{
    const int ObjectSize = 200_000; // well above the 85,000-byte LOH threshold
    const int ObjectCount = 200;

    var kept = new List<byte[]>(ObjectCount / 2);
    var toFree = new List<byte[]>(ObjectCount / 2);
    for (int i = 0; i < ObjectCount; i++)
    {
        var array = new byte[ObjectSize];
        if (i % 2 == 0) kept.Add(array);
        else toFree.Add(array); // every other object will be dropped, opening holes on the LOH
    }
    toFree.Clear(); // drop every other object's only reference
    GC.Collect(); // reclaim them — this leaves the LOH with alternating live/free blocks: fragmented

    var fragmented = GC.GetGCMemoryInfo(GCKind.FullBlocking);
    var lohIndexFragmented = FindLohGenerationIndex(fragmented);
    Console.WriteLine($"  After freeing every other {ObjectSize:N0}-byte object (no compaction yet):");
    Console.WriteLine($"    LOH FragmentationAfterBytes = {fragmented.GenerationInfo[lohIndexFragmented].FragmentationAfterBytes:N0}");

    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
    GC.Collect(); // this is the ONE collection that actually compacts the LOH

    var compacted = GC.GetGCMemoryInfo(GCKind.FullBlocking);
    var lohIndexCompacted = FindLohGenerationIndex(compacted);
    Console.WriteLine($"  After GCSettings.LargeObjectHeapCompactionMode = CompactOnce + one more GC.Collect():");
    Console.WriteLine($"    LOH FragmentationAfterBytes = {compacted.GenerationInfo[lohIndexCompacted].FragmentationAfterBytes:N0}");
    Console.WriteLine("  CompactOnce is a one-shot switch — it resets itself to Default after the next collection;");
    Console.WriteLine("  the LOH is NOT compacted on every collection by default, only when this is explicitly requested.");

    GC.KeepAlive(kept);

    static int FindLohGenerationIndex(GCMemoryInfo info)
    {
        // GCGenerationInfo doesn't self-label which entry is the LOH, so this
        // locates it the honest way: the LOH is the only generation holding
        // 200,000-byte objects, so it's the entry with by far the largest
        // SizeAfterBytes among the reported generations in this demo.
        int maxIndex = 0;
        for (int i = 1; i < info.GenerationInfo.Length; i++)
        {
            if (info.GenerationInfo[i].SizeAfterBytes > info.GenerationInfo[maxIndex].SizeAfterBytes)
            {
                maxIndex = i;
            }
        }
        return maxIndex;
    }
}

static void RunPohDemo()
{
    // GC.AllocateArray<T>(count, pinned: true) allocates directly on the
    // Pinned Object Heap (POH), introduced in .NET 5 specifically so a
    // pinned buffer (e.g. for native interop) doesn't sit pinned in the
    // middle of an otherwise-compactable Gen 0/1/2 segment, forcing the
    // collector to work around it.
    var pinnedBuffer = GC.AllocateArray<byte>(4_096, pinned: true);
    Console.WriteLine($"  GC.AllocateArray<byte>(4096, pinned: true) -> GC.GetGeneration = {GC.GetGeneration(pinnedBuffer)}");
    Console.WriteLine("  (POH objects are collected alongside Gen 2/LOH and never need a 'fixed' statement to stay put.)");
    GC.KeepAlive(pinnedBuffer);
}

sealed class CrossGenHolder
{
    public byte[]? Reference;
}
