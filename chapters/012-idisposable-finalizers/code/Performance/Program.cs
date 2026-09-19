using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Performance;

[MemoryDiagnoser]
public class FinalizerBenchmark
{
    private const int ObjectCount = 100_000;

    class ClassWithoutFinalizer
    {
        public int Value { get; set; } = 42;
    }

    class ClassWithFinalizer
    {
        public int Value { get; set; } = 42;
        ~ClassWithFinalizer() 
        {
            // Empty finalizer to demonstrate the overhead of the finalization queue alone.
        }
    }

    [Benchmark(Baseline = true)]
    public void AllocateWithoutFinalizers()
    {
        for (int i = 0; i < ObjectCount; i++)
        {
            var obj = new ClassWithoutFinalizer();
        }
    }

    [Benchmark]
    public void AllocateWithFinalizers()
    {
        for (int i = 0; i < ObjectCount; i++)
        {
            var obj = new ClassWithFinalizer();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<FinalizerBenchmark>();
    }
}
