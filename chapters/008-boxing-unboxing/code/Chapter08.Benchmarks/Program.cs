using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<BoxingArithmeticBenchmarks>();
BenchmarkRunner.Run<CollectionBoxingBenchmarks>();
BenchmarkRunner.Run<InterfaceDispatchBenchmarks>();

// Sums N ints two ways: staying in int the whole time (baseline) vs boxing
// each value into an object and unboxing it back out before adding — the
// literal cost of a box+unbox round trip per element, isolated from any
// container or dispatch mechanism.
[MemoryDiagnoser]
public class BoxingArithmeticBenchmarks
{
    const int N = 100_000;

    [Benchmark(Baseline = true)]
    public long SumUnboxed()
    {
        long sum = 0;
        for (int i = 0; i < N; i++)
        {
            sum += i;
        }
        return sum;
    }

    [Benchmark]
    public long SumBoxedRoundTrip()
    {
        long sum = 0;
        for (int i = 0; i < N; i++)
        {
            object boxed = i;      // box
            sum += (int)boxed;     // unbox
        }
        return sum;
    }
}

// ArrayList.Add boxes every int; List<int>.Add doesn't. Same N items,
// same Add-then-Sum shape, to isolate the collection's boxing behavior
// rather than anything about the loop around it.
[MemoryDiagnoser]
public class CollectionBoxingBenchmarks
{
    const int N = 100_000;

    [Benchmark(Baseline = true)]
    public long ListOfIntAddAndSum()
    {
        var list = new List<int>(N);
        for (int i = 0; i < N; i++)
        {
            list.Add(i);
        }
        long sum = 0;
        foreach (var item in list)
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark]
    public long ArrayListAddAndSum()
    {
        var list = new System.Collections.ArrayList(N);
        for (int i = 0; i < N; i++)
        {
            list.Add(i); // boxes 'i' before storing it
        }
        long sum = 0;
        foreach (int item in list) // unboxes each element on the way out
        {
            sum += item;
        }
        return sum;
    }
}

// Same struct, same method, three call shapes: direct (no boxing), through
// an interface reference (boxes once per assignment, then dispatches via
// the box), and through a generic constraint (the JIT specializes per
// value type — no box, ever).
[MemoryDiagnoser]
public class InterfaceDispatchBenchmarks
{
    const int N = 100_000;

    public interface IIncrementable
    {
        int Value { get; }
        void Increment();
    }

    public struct Counter(int startingValue) : IIncrementable
    {
        private int _value = startingValue;
        public int Value => _value;
        public void Increment() => _value++;
    }

    [Benchmark(Baseline = true)]
    public int DirectStructCalls()
    {
        var counter = new Counter(0);
        for (int i = 0; i < N; i++)
        {
            counter.Increment();
        }
        return counter.Value;
    }

    [Benchmark]
    public int GenericConstraintCalls()
    {
        var counter = new Counter(0);
        return RunGeneric(counter);

        static int RunGeneric<T>(T incrementable) where T : IIncrementable
        {
            for (int i = 0; i < N; i++)
            {
                incrementable.Increment();
            }
            return incrementable.Value;
        }
    }

    [Benchmark]
    public int InterfaceReferenceCalls()
    {
        IIncrementable counter = new Counter(0); // boxes once, here
        for (int i = 0; i < N; i++)
        {
            counter.Increment(); // dispatches against the same box every time
        }
        return counter.Value;
    }
}
