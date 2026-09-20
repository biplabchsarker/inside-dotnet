using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<DispatchBenchmarks>();

public class NonVirtualHolder
{
    public int Compute(int x) => x + 1;
}

public class Base
{
    public virtual int Compute(int x) => x + 1;
}

public class Derived : Base
{
    public override int Compute(int x) => x + 1;
}

public sealed class SealedDerived : Base
{
    public sealed override int Compute(int x) => x + 1;
}

public interface ICompute
{
    int Compute(int x);
}

public class InterfaceImpl : ICompute
{
    public int Compute(int x) => x + 1;
}

[MemoryDiagnoser]
public class DispatchBenchmarks
{
    private const int Iterations = 50_000_000;

    // Declared type is the NON-virtual class itself — every call is a plain,
    // statically-bound `call`, resolved once at compile time.
    private readonly NonVirtualHolder _direct = new();

    // Declared type is BASE (not sealed) holding a Derived instance — the
    // JIT must go through the object's own method table (vtable) on every
    // call, because Base could have any number of further overrides.
    private readonly Base _virtual = new Derived();

    // Declared type is a SEALED class — no further override is even
    // possible, so the JIT can prove there is only one implementation and
    // devirtualize the call (turn it back into a direct call, sometimes
    // even inlining it) without any runtime type check.
    private readonly SealedDerived _sealedVirtual = new();

    // Declared type is an interface — dispatch goes through the type's
    // Interface Method Table (a separate structure from the class vtable,
    // since one class can implement many interfaces), one more layer of
    // indirection than a class virtual call.
    private readonly ICompute _interface = new InterfaceImpl();

    [Benchmark(Baseline = true)]
    public int DirectCall()
    {
        int sum = 0;
        for (int i = 0; i < Iterations; i++)
            sum += _direct.Compute(i);
        return sum;
    }

    [Benchmark]
    public int VirtualCall()
    {
        int sum = 0;
        for (int i = 0; i < Iterations; i++)
            sum += _virtual.Compute(i);
        return sum;
    }

    [Benchmark]
    public int SealedVirtualCall()
    {
        int sum = 0;
        for (int i = 0; i < Iterations; i++)
            sum += _sealedVirtual.Compute(i);
        return sum;
    }

    [Benchmark]
    public int InterfaceCall()
    {
        int sum = 0;
        for (int i = 0; i < Iterations; i++)
            sum += _interface.Compute(i);
        return sum;
    }
}
