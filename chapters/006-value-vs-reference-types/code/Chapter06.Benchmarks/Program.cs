using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<StructVsClassBenchmarks>();

[MemoryDiagnoser]
public class StructVsClassBenchmarks
{
    // Small struct (16 bytes) - the "cheap to copy" case.
    public readonly struct SmallStruct
    {
        public readonly double X, Y;
        public SmallStruct(double x, double y) { X = x; Y = y; }
        public double Sum() => X + Y;
    }

    // Large struct (256 bytes) - copying this by value is the expensive case.
    public readonly struct LargeStruct
    {
        public readonly double A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P,
                                Q, R, S, T, U, V, W, X2, Y2, Z2, AA, BB, CC, DD, EE, FF; // 32 doubles = 256 bytes
        public LargeStruct(double seed) { A = B = C = D = E = F = G = H = I = J = K = L = M = N = O = P
            = Q = R = S = T = U = V = W = X2 = Y2 = Z2 = AA = BB = CC = DD = EE = FF = seed; }
        public double Sum() => A + B + C + D + E + F + G + H + I + J + K + L + M + N + O + P;
    }

    // Equivalent reference type - one pointer copy regardless of size.
    public sealed class LargeClass
    {
        public double A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P,
                      Q, R, S, T, U, V, W, X2, Y2, Z2, AA, BB, CC, DD, EE, FF;
        public LargeClass(double seed) { A = B = C = D = E = F = G = H = I = J = K = L = M = N = O = P
            = Q = R = S = T = U = V = W = X2 = Y2 = Z2 = AA = BB = CC = DD = EE = FF = seed; }
        public double Sum() => A + B + C + D + E + F + G + H + I + J + K + L + M + N + O + P;
    }

    readonly SmallStruct _small = new(1, 2);
    readonly LargeStruct _large = new(1.5);
    readonly LargeClass _largeClass = new(1.5);

    [Benchmark(Baseline = true)]
    public double PassSmallStructByValue() => SumSmallByValue(_small);

    [Benchmark]
    public double PassLargeStructByValue() => SumLargeByValue(_large);

    [Benchmark]
    public double PassLargeStructByIn() => SumLargeByIn(in _large);

    [Benchmark]
    public double PassLargeClassByReference() => SumClass(_largeClass);

    [MethodImpl(MethodImplOptions.NoInlining)]
    static double SumSmallByValue(SmallStruct s) => s.Sum();

    [MethodImpl(MethodImplOptions.NoInlining)]
    static double SumLargeByValue(LargeStruct s) => s.Sum();

    [MethodImpl(MethodImplOptions.NoInlining)]
    static double SumLargeByIn(in LargeStruct s) => s.Sum();

    [MethodImpl(MethodImplOptions.NoInlining)]
    static double SumClass(LargeClass c) => c.Sum();
}
