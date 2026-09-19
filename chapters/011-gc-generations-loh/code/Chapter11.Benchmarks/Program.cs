using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Runtime;

BenchmarkRunner.Run<LohVsSmallObjectBenchmarks>();
BenchmarkRunner.Run<LohFragmentationCompactionBenchmarks>();
BenchmarkRunner.Run<CardTableProxyBenchmarks>();

// Allocating the same total number of bytes either as many small (Gen 0-path)
// objects or as fewer Large-Object-Heap objects, isolating the LOH's own
// allocation cost from the generational path's, at an equal total-byte size.
[MemoryDiagnoser]
public class LohVsSmallObjectBenchmarks
{
    const int TotalBytes = 100_000_000;
    const int SmallObjectSize = 1_600;   // stays under the 85,000-byte LOH threshold
    const int LargeObjectSize = 100_000; // over the threshold -> routed to the LOH
    const int SmallObjectCount = TotalBytes / SmallObjectSize;
    const int LargeObjectCount = TotalBytes / LargeObjectSize;

    [Benchmark(Baseline = true)]
    public long AllocateManySmallObjects()
    {
        long sink = 0;
        for (int i = 0; i < SmallObjectCount; i++)
        {
            var buffer = new byte[SmallObjectSize];
            buffer[0] = 1;
            sink += buffer[0];
        }
        return sink;
    }

    [Benchmark]
    public long AllocateFewLargeObjects()
    {
        long sink = 0;
        for (int i = 0; i < LargeObjectCount; i++)
        {
            var buffer = new byte[LargeObjectSize];
            buffer[0] = 1;
            sink += buffer[0];
        }
        return sink;
    }
}

// The cost of a full, compacting GC.Collect() while the Large Object Heap is
// fragmented vs. immediately after a one-time GCSettings.LargeObjectHeapCompactionMode
// compaction. Each iteration rebuilds fragmentation from scratch, since
// GCSettings.LargeObjectHeapCompactionMode.CompactOnce resets itself back to
// Default after the collection that consumes it.
[SimpleJob(warmupCount: 3, iterationCount: 7)]
[MemoryDiagnoser]
public class LohFragmentationCompactionBenchmarks
{
    const int ObjectSize = 150_000; // over the 85,000-byte LOH threshold
    const int ObjectCount = 400;

    List<byte[]>? _kept;

    [IterationSetup]
    public void BuildFragmentedLoh()
    {
        _kept = new List<byte[]>(ObjectCount / 2);
        var toFree = new List<byte[]>(ObjectCount / 2);
        for (int i = 0; i < ObjectCount; i++)
        {
            var array = new byte[ObjectSize];
            if (i % 2 == 0) _kept.Add(array);
            else toFree.Add(array);
        }
        toFree.Clear();
        GC.Collect(); // reclaim the freed half, leaving alternating live/free LOH blocks

        // Reset to the default (non-compacting) mode before each iteration,
        // so the CompactOnce-only benchmark method is the one thing that
        // differs between the two [Benchmark] methods below.
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.Default;
    }

    [Benchmark(Baseline = true)]
    public void FullCollectWithFragmentation() => GC.Collect();

    [Benchmark]
    public void FullCollectWithCompactOnce()
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
    }
}

// A proxy for the card table / write barrier payoff. The claim under test:
// a Gen 0 collection's cost tracks how much of Gen 2 is actually DIRTY
// (holds a live cross-generational reference) far more than how large the
// rest of Gen 2 is. To isolate that, the number of dirtied objects is held
// FIXED (DirtyObjectCount) while the surrounding, never-touched Gen 2 graph
// size varies -- measured results show cost growing SUB-LINEARLY with graph
// size (not perfectly flat -- see the article's own caveat about this
// benchmark's noise), consistent with a Gen 0 collection mostly rescanning
// dirty cards rather than the whole of Gen 2. No public API exposes the
// card table's dirty bits directly, so this measures the closest observable
// consequence instead.
[SimpleJob(warmupCount: 3, iterationCount: 7)]
public class CardTableProxyBenchmarks
{
    const int DirtyObjectCount = 2_000; // held constant across every graph size below

    [Params(50_000, 500_000, 2_000_000)]
    public int Gen2GraphSize { get; set; }

    CrossGenHolder[]? _graph;

    [IterationSetup]
    public void Setup()
    {
        _graph = new CrossGenHolder[Gen2GraphSize];
        for (int i = 0; i < Gen2GraphSize; i++)
        {
            _graph[i] = new CrossGenHolder();
        }
        GC.Collect(0);
        GC.Collect(1); // promote the whole graph into Gen 2

        // Fire the write barrier on only DirtyObjectCount of these Gen 2
        // objects, regardless of how large Gen2GraphSize is -- the rest of
        // the graph is live, but never written to after promotion, so it
        // should stay off the card table entirely.
        for (int i = 0; i < DirtyObjectCount; i++)
        {
            _graph[i].Reference = new byte[16];
        }
    }

    [Benchmark]
    public void CollectGen0WithFixedDirtySet() => GC.Collect(0);

    sealed class CrossGenHolder
    {
        public byte[]? Reference;
    }
}
