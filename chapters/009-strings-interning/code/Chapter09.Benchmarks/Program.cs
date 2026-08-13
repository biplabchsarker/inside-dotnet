using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<StringComparisonBenchmarks>();
BenchmarkRunner.Run<ReferenceEqualityFastPathBenchmarks>();
BenchmarkRunner.Run<SubstringAllocationBenchmarks>();

// Same two strings (equal content, different case), compared 100,000 times
// under three different StringComparison modes — isolates the real cost of
// culture-aware comparison versus ordinal comparison, the exact trade-off
// Microsoft's own string-comparison guidance is built around.
[MemoryDiagnoser]
public class StringComparisonBenchmarks
{
    const int N = 100_000;

    static readonly string Left = "Contoso-Invoice-Reference-Number-2026-08-13-Batch-0042";
    static readonly string Right = "CONTOSO-INVOICE-REFERENCE-NUMBER-2026-08-13-BATCH-0042"; // same content, different case

    [Benchmark(Baseline = true)]
    public int OrdinalIgnoreCase()
    {
        int matches = 0;
        for (int i = 0; i < N; i++)
        {
            if (string.Equals(Left, Right, StringComparison.OrdinalIgnoreCase)) matches++;
        }
        return matches;
    }

    [Benchmark]
    public int CurrentCultureIgnoreCase()
    {
        int matches = 0;
        for (int i = 0; i < N; i++)
        {
            if (string.Equals(Left, Right, StringComparison.CurrentCultureIgnoreCase)) matches++;
        }
        return matches;
    }

    [Benchmark]
    public int InvariantCultureIgnoreCase()
    {
        int matches = 0;
        for (int i = 0; i < N; i++)
        {
            if (string.Equals(Left, Right, StringComparison.InvariantCultureIgnoreCase)) matches++;
        }
        return matches;
    }
}

// string.Equals checks ReferenceEquals first, before comparing any content.
// This isolates that fast path: the SAME reference compared to itself a
// million times versus a different object holding equal, but longer, content
// that must be scanned char-by-char before Equals can return true.
[MemoryDiagnoser]
public class ReferenceEqualityFastPathBenchmarks
{
    const int N = 1_000_000;

    static readonly string Original = string.Intern(new string('x', 500));
    static readonly string SameReference = Original;
    static readonly string EqualButDifferentReference = new string('x', 500); // equal content, deliberately not interned

    [Benchmark(Baseline = true)]
    public int EqualsAgainstSameReference()
    {
        int matches = 0;
        for (int i = 0; i < N; i++)
        {
            if (Original.Equals(SameReference)) matches++;
        }
        return matches;
    }

    [Benchmark]
    public int EqualsAgainstEqualButDifferentReference()
    {
        int matches = 0;
        for (int i = 0; i < N; i++)
        {
            if (Original.Equals(EqualButDifferentReference)) matches++;
        }
        return matches;
    }
}

// Substring(0, this.Length) hits a documented self-reference fast path and
// returns the original string with no allocation; any other range always
// allocates a new string. Same source string, same call shape, only the
// requested range differs.
[MemoryDiagnoser]
public class SubstringAllocationBenchmarks
{
    const int N = 100_000;

    static readonly string Source = new string('a', 64);

    [Benchmark(Baseline = true)]
    public int WholeRangeSubstring()
    {
        int totalLength = 0;
        for (int i = 0; i < N; i++)
        {
            string result = Source.Substring(0, Source.Length); // returns Source itself, no allocation
            totalLength += result.Length;
        }
        return totalLength;
    }

    [Benchmark]
    public int PartialSubstring()
    {
        int totalLength = 0;
        for (int i = 0; i < N; i++)
        {
            string result = Source.Substring(1, Source.Length - 1); // always allocates
            totalLength += result.Length;
        }
        return totalLength;
    }
}
