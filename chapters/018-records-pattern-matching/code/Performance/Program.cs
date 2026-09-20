using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<RecordPatternBenchmarks>();

[MemoryDiagnoser]
public class RecordPatternBenchmarks
{
    private PersonClass _c1 = null!;
    private PersonClass _c2 = null!;
    private PersonRecord _r1 = null!;
    private PersonRecord _r2 = null!;
    private PersonRecordStruct _s1;
    private PersonRecordStruct _s2;
    private object[] _sampleObjects = null!;

    [GlobalSetup]
    public void Setup()
    {
        _c1 = new PersonClass("Grace", "Hopper", 85);
        _c2 = new PersonClass("Grace", "Hopper", 85);

        _r1 = new PersonRecord("Grace", "Hopper", 85);
        _r2 = new PersonRecord("Grace", "Hopper", 85);

        _s1 = new PersonRecordStruct("Grace", "Hopper", 85);
        _s2 = new PersonRecordStruct("Grace", "Hopper", 85);

        _sampleObjects = new object[]
        {
            new ShapeCircle(12.5),
            new ShapeRectangle(10.0, 20.0),
            new ShapeTriangle(5.0, 8.0),
            "text",
            42
        };
    }

    [Benchmark(Baseline = true)]
    public bool ClassReferenceEquality() => _c1 == _c2;

    [Benchmark]
    public bool RecordClassValueEquality() => _r1 == _r2;

    [Benchmark]
    public bool RecordStructValueEquality() => _s1 == _s2;

    [Benchmark]
    public PersonRecord RecordClassWithClone() => _r1 with { Age = 86 };

    [Benchmark]
    public PersonRecordStruct RecordStructWithClone() => _s1 with { Age = 86 };

    [Benchmark]
    public double PatternMatchingSwitch()
    {
        double total = 0;
        foreach (var obj in _sampleObjects)
        {
            total += obj switch
            {
                ShapeCircle { Radius: > 0 } c => Math.PI * c.Radius * c.Radius,
                ShapeRectangle r => r.Width * r.Height,
                ShapeTriangle t => 0.5 * t.Base * t.Height,
                _ => 0.0
            };
        }
        return total;
    }

    [Benchmark]
    public double IfElseTypeCheck()
    {
        double total = 0;
        foreach (var obj in _sampleObjects)
        {
            if (obj is ShapeCircle c && c.Radius > 0)
                total += Math.PI * c.Radius * c.Radius;
            else if (obj is ShapeRectangle r)
                total += r.Width * r.Height;
            else if (obj is ShapeTriangle t)
                total += 0.5 * t.Base * t.Height;
        }
        return total;
    }
}

// Supporting Types
public class PersonClass
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public PersonClass(string first, string last, int age)
    {
        FirstName = first;
        LastName = last;
        Age = age;
    }
}

public record PersonRecord(string FirstName, string LastName, int Age);

public readonly record struct PersonRecordStruct(string FirstName, string LastName, int Age);

public record ShapeCircle(double Radius);
public record ShapeRectangle(double Width, double Height);
public record ShapeTriangle(double Base, double Height);
