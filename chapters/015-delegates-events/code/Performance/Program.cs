using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<DelegateBenchmarks>();

public class TargetClass
{
    private int _state = 10;
    public int Compute(int x) => x + _state;
    public static int ComputeStatic(int x) => x + 10;
}

[MemoryDiagnoser]
public class DelegateBenchmarks
{
    private TargetClass _target = null!;
    private Func<int, int> _instanceDelegate = null!;
    private Func<int, int> _staticDelegate = null!;
    private Func<int, int> _multicastDelegate = null!;

    [GlobalSetup]
    public void Setup()
    {
        _target = new TargetClass();
        _instanceDelegate = _target.Compute;
        _staticDelegate = TargetClass.ComputeStatic;

        TargetClass second = new TargetClass();
        _multicastDelegate = _target.Compute;
        _multicastDelegate += second.Compute;
    }

    [Benchmark(Baseline = true)]
    public int DirectCall()
    {
        return _target.Compute(42);
    }

    [Benchmark]
    public int StaticMethodDirectCall()
    {
        return TargetClass.ComputeStatic(42);
    }

    [Benchmark]
    public int InstanceDelegateCall()
    {
        return _instanceDelegate(42);
    }

    [Benchmark]
    public int StaticDelegateCall()
    {
        return _staticDelegate(42);
    }

    [Benchmark]
    public int MulticastDelegateCall()
    {
        return _multicastDelegate(42);
    }

    [Benchmark]
    public int ClosureAllocationCall()
    {
        int captured = 42; // captured variable causes display class allocation
        Func<int, int> func = x => x + captured;
        return func(10);
    }

    [Benchmark]
    public int StaticLambdaNoAllocationCall()
    {
        // Static lambda does not capture state, delegate can be cached
        Func<int, int> func = static x => x + 42;
        return func(10);
    }
}
