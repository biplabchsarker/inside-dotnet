using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.WriteLine("=== Inside .NET: Episode 4 demo — Tiered Compilation in action ===");
Console.WriteLine();

// 1. Inspect runtime feature flags relevant to JIT behavior. These report what
//    THIS running CLR supports/has enabled — not universal constants.
Console.WriteLine("-- RuntimeFeature flags --");
Console.WriteLine($"IsDynamicCodeSupported : {RuntimeFeature.IsDynamicCodeSupported}");
Console.WriteLine($"IsDynamicCodeCompiled  : {RuntimeFeature.IsDynamicCodeCompiled}");
// Under Native AOT, IsDynamicCodeCompiled is false (nothing is JIT-compiled at run
// time); under the standard JIT-based model used here, it's true.

Console.WriteLine();
Console.WriteLine("-- Observing Tier 0 -> Tier 1 promotion via batch timing --");
Console.WriteLine("(A hot method starts on quick, minimally-optimized Tier 0 code.");
Console.WriteLine(" After enough calls, the background JIT swaps in a fully");
Console.WriteLine(" optimized Tier 1 version. We can't force this to happen at an");
Console.WriteLine(" exact call count, but running enough batches typically surfaces");
Console.WriteLine(" a clear drop and then a plateau in per-batch time.)");
Console.WriteLine();

const int batches = 12;
const int callsPerBatch = 2_000_000;

Console.WriteLine($"{"Batch",6} | {"Calls",10} | {"Elapsed (ms)",12} | {"ns/call",10}");
Console.WriteLine(new string('-', 46));

long total = 0;
for (int batch = 1; batch <= batches; batch++)
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < callsPerBatch; i++)
    {
        total += HotMethod(i);
    }
    sw.Stop();

    double nsPerCall = sw.Elapsed.TotalMilliseconds * 1_000_000.0 / callsPerBatch;
    Console.WriteLine($"{batch,6} | {callsPerBatch,10} | {sw.Elapsed.TotalMilliseconds,12:F3} | {nsPerCall,10:F2}");
}

Console.WriteLine();
Console.WriteLine($"(checksum, to keep the JIT from eliminating the loop entirely: {total})");
Console.WriteLine();
Console.WriteLine("Expect the earliest batches to be slower and noisier (Tier 0,");
Console.WriteLine("still warming up / promoting) and later batches to settle into a");
Console.WriteLine("faster, more stable ns/call figure (Tier 1, fully optimized) —");
Console.WriteLine("exact batch numbers vary by machine, load, and CLR version.");

// A small, branch-containing, arithmetic-heavy method — deliberately simple so the
// *tiering effect* dominates the timing signal rather than algorithmic complexity.
[MethodImpl(MethodImplOptions.NoInlining)]
static long HotMethod(int n)
{
    long x = n;
    if ((n & 1) == 0)
    {
        x = x * 3 + 1;
    }
    else
    {
        x = x ^ (x << 2);
    }
    return x % 97;
}
