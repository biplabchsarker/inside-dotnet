using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<GcModeThroughputBenchmarks>();
BenchmarkRunner.Run<GenerationCollectionCostBenchmarks>();
BenchmarkRunner.Run<PreSizedVsGrowingListBenchmarks>();

// Same sustained small-object allocation workload, run under two separate GC
// modes via BenchmarkDotNet's job configuration — Workstation (the default
// for a plain console/desktop app) vs. Server GC (the default for ASP.NET
// Core). Isolates the throughput trade-off itself, not any code difference.
file class GcModeConfig : ManualConfig
{
    public GcModeConfig()
    {
        AddJob(Job.Default.WithGcServer(false).WithId("Workstation"));
        AddJob(Job.Default.WithGcServer(true).WithId("Server"));
        AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);
    }
}

[Config(typeof(GcModeConfig))]
public class GcModeThroughputBenchmarks
{
    const int N = 2_000_000;

    [Benchmark]
    public long AllocateSmallObjects()
    {
        long sink = 0;
        for (int i = 0; i < N; i++)
        {
            var buffer = new byte[64];
            buffer[0] = 1;
            sink += buffer[0];
        }
        return sink;
    }
}

// GC.Collect(0) vs GC.Collect(1) vs GC.Collect(2) (a full collection), each
// forced against a live object graph that's actually been promoted to that
// generation first — isolating the real cost difference between collecting
// a shallow generation and walking the entire heap.
[MemoryDiagnoser]
public class GenerationCollectionCostBenchmarks
{
    const int LiveObjectCount = 50_000;

    List<byte[]>? _gen0Live;
    List<byte[]>? _gen1Live;
    List<byte[]>? _gen2Live;

    [IterationSetup(Target = nameof(CollectGen0))]
    public void SetupGen0()
    {
        _gen0Live = CreateLiveObjects(); // freshly allocated — still in Gen 0
    }

    [IterationSetup(Target = nameof(CollectGen1))]
    public void SetupGen1()
    {
        _gen1Live = CreateLiveObjects();
        GC.Collect(0); // promote this graph into Gen 1
    }

    [IterationSetup(Target = nameof(CollectGen2))]
    public void SetupGen2()
    {
        _gen2Live = CreateLiveObjects();
        GC.Collect(0);
        GC.Collect(1); // promote this graph all the way into Gen 2
    }

    [Benchmark(Baseline = true)]
    public void CollectGen0() => GC.Collect(0);

    [Benchmark]
    public void CollectGen1() => GC.Collect(1);

    [Benchmark]
    public void CollectGen2() => GC.Collect(2);

    static List<byte[]> CreateLiveObjects()
    {
        var list = new List<byte[]>(LiveObjectCount);
        for (int i = 0; i < LiveObjectCount; i++)
        {
            list.Add(new byte[64]);
        }
        return list;
    }
}

// List<T> growing from a default/empty capacity (repeatedly reallocating and
// copying its backing array as it fills) vs. a List<T> pre-sized to its final
// capacity up front — same N items added either way.
[MemoryDiagnoser]
public class PreSizedVsGrowingListBenchmarks
{
    const int N = 1_000_000;

    [Benchmark(Baseline = true)]
    public int GrowingList()
    {
        var list = new List<int>(); // starts empty, reallocates as it grows
        for (int i = 0; i < N; i++)
        {
            list.Add(i);
        }
        return list.Count;
    }

    [Benchmark]
    public int PreSizedList()
    {
        var list = new List<int>(N); // capacity known up front — one allocation
        for (int i = 0; i < N; i++)
        {
            list.Add(i);
        }
        return list.Count;
    }
}
