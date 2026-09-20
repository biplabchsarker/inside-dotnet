using System.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<GenericsBenchmarks>();

public interface ISummable
{
    int Add(int value);
}

public struct ValueSummer : ISummable
{
    private int _accumulator;
    public int Add(int value) => _accumulator += value;
}

[MemoryDiagnoser]
public class GenericsBenchmarks
{
    private const int Count = 10_000;
    private int[] _data = null!;
    private ValueSummer _summer;

    [GlobalSetup]
    public void Setup()
    {
        _data = new int[Count];
        for (int i = 0; i < Count; i++) _data[i] = i;
        _summer = new ValueSummer();
    }

    [Benchmark(Baseline = true)]
    public int GenericList_Specialized()
    {
        var list = new List<int>(Count);
        for (int i = 0; i < Count; i++)
        {
            list.Add(_data[i]);
        }
        return list.Count;
    }

    [Benchmark]
    public int NonGeneric_ArrayList_Boxing()
    {
        var list = new ArrayList(Count);
        for (int i = 0; i < Count; i++)
        {
            list.Add(_data[i]); // Boxes int to object on every iteration!
        }
        return list.Count;
    }

    [Benchmark]
    public int ConstrainedGenericStructCall()
    {
        var s = _summer;
        int sum = 0;
        for (int i = 0; i < Count; i++)
        {
            sum = InvokeConstrained(ref s, i); // Direct call, zero boxing
        }
        return sum;
    }

    [Benchmark]
    public int InterfaceBoxingCall()
    {
        ISummable boxed = _summer; // Boxes struct to interface reference!
        int sum = 0;
        for (int i = 0; i < Count; i++)
        {
            sum = boxed.Add(i); // Virtual interface dispatch
        }
        return sum;
    }

    private static int InvokeConstrained<T>(ref T summable, int val) where T : struct, ISummable
    {
        return summable.Add(val);
    }
}
