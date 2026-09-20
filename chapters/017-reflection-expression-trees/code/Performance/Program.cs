using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<ReflectionVsExpressionBenchmarks>();

public class BenchmarkModel
{
    public int Id { get; set; } = 42;
    public string Name { get; set; } = "Inside .NET";
    public decimal Value { get; set; } = 99.95m;

    public decimal ComputeTax(decimal rate) => Value * rate;
}

[MemoryDiagnoser]
public class ReflectionVsExpressionBenchmarks
{
    private BenchmarkModel _model = null!;
    private PropertyInfo _propInfo = null!;
    private MethodInfo _methodInfo = null!;
    private Func<BenchmarkModel, decimal> _compiledExprGetter = null!;
    private Func<BenchmarkModel, decimal, decimal> _compiledExprMethod = null!;
    private Func<BenchmarkModel, decimal> _openDelegateGetter = null!;
    private object[] _methodArgs = null!;

    [GlobalSetup]
    public void Setup()
    {
        _model = new BenchmarkModel();
        _propInfo = typeof(BenchmarkModel).GetProperty("Value")!;
        _methodInfo = typeof(BenchmarkModel).GetMethod("ComputeTax")!;
        _methodArgs = new object[] { 0.20m };

        // 1. Compile Expression Tree Getter
        var param = Expression.Parameter(typeof(BenchmarkModel), "m");
        var propExpr = Expression.Property(param, _propInfo);
        _compiledExprGetter = Expression.Lambda<Func<BenchmarkModel, decimal>>(propExpr, param).Compile();

        // 2. Compile Expression Tree Method Call
        var rateParam = Expression.Parameter(typeof(decimal), "rate");
        var callExpr = Expression.Call(param, _methodInfo, rateParam);
        _compiledExprMethod = Expression.Lambda<Func<BenchmarkModel, decimal, decimal>>(callExpr, param, rateParam).Compile();

        // 3. Open Delegate via Delegate.CreateDelegate
        _openDelegateGetter = (Func<BenchmarkModel, decimal>)Delegate.CreateDelegate(
            typeof(Func<BenchmarkModel, decimal>), _propInfo.GetGetMethod()!);
    }

    [Benchmark(Baseline = true)]
    public decimal DirectPropertyAccess()
    {
        return _model.Value;
    }

    [Benchmark]
    public decimal DelegateCreateDelegateAccess()
    {
        return _openDelegateGetter(_model);
    }

    [Benchmark]
    public decimal CompiledExpressionPropertyAccess()
    {
        return _compiledExprGetter(_model);
    }

    [Benchmark]
    public decimal ReflectionPropertyGetValue()
    {
        return (decimal)_propInfo.GetValue(_model)!;
    }

    [Benchmark]
    public decimal DirectMethodCall()
    {
        return _model.ComputeTax(0.20m);
    }

    [Benchmark]
    public decimal CompiledExpressionMethodCall()
    {
        return _compiledExprMethod(_model, 0.20m);
    }

    [Benchmark]
    public decimal ReflectionMethodInvoke()
    {
        return (decimal)_methodInfo.Invoke(_model, _methodArgs)!;
    }
}
