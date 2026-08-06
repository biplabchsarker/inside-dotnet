// Program.cs — .NET 10 console app
// Demonstrates: (1) measuring the allocation fast path's throughput via
// GC.GetAllocatedBytesForCurrentThread(); (2) base-before-derived construction
// order, observed directly; (3) the classic "virtual call from a base
// constructor sees uninitialized derived state" gotcha, made concrete instead
// of just described.

Console.WriteLine("=== Inside .NET: Episode 8 — Object Allocation demo ===");

// ---------------------------------------------------------------------------
// Part 1: measure the allocation fast path.
// GC.GetAllocatedBytesForCurrentThread() reads the *current thread's*
// allocation counter directly — no forced collection, no snapshot of the
// whole heap, just "how many bytes has this thread bumped its allocation
// pointer past." That makes it a faithful way to observe the fast path
// itself, not the GC's behavior around it.
// ---------------------------------------------------------------------------
const int iterations = 5_000_000;

long before = GC.GetAllocatedBytesForCurrentThread();
var sw = System.Diagnostics.Stopwatch.StartNew();

long sink = 0;
for (int i = 0; i < iterations; i++)
{
    var record = new AuditRecord(i, DateTime.UtcNow.Ticks);
    sink += record.Id; // touch it so the JIT can't optimize the allocation away
}

sw.Stop();
long after = GC.GetAllocatedBytesForCurrentThread();

long totalBytes = after - before;
double bytesPerAlloc = (double)totalBytes / iterations;
double nsPerAlloc = sw.Elapsed.TotalMilliseconds * 1_000_000 / iterations;

Console.WriteLine($"\n--- 1. Allocation fast-path throughput ---");
Console.WriteLine($"Allocated {iterations:N0} AuditRecord instances in {sw.Elapsed.TotalMilliseconds:N1} ms");
Console.WriteLine($"Bytes allocated (this thread): {totalBytes:N0}  ({bytesPerAlloc:N1} bytes/instance)");
Console.WriteLine($"Approx. time per allocation:   {nsPerAlloc:N1} ns  (sink={sink})");

// ---------------------------------------------------------------------------
// Part 2: construction order — base runs before derived, always.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 2. Construction order: base before derived ---");
_ = new DerivedWithLogging();

// ---------------------------------------------------------------------------
// Part 3: the gotcha, made concrete — a virtual call from a base constructor
// observes the DERIVED class's fields at their zeroed default, because the
// derived constructor hasn't run yet when the base constructor's body does.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 3. Virtual call from base constructor sees uninitialized derived state ---");
var risky = new RiskyDerived(label: "ready");
Console.WriteLine($"After full construction, risky.Describe() = \"{risky.Describe()}\"");

Console.WriteLine("\n=== Done ===");

// A small, deliberately plain reference type — no inheritance, no interfaces —
// so Part 1's measurement reflects the allocation mechanism itself, not any
// virtual-dispatch or interface overhead layered on top of it.
sealed class AuditRecord
{
    public int Id { get; }
    public long TimestampTicks { get; }

    public AuditRecord(int id, long timestampTicks)
    {
        Id = id;
        TimestampTicks = timestampTicks;
    }
}

// Demonstrates the ORDER of construction: base's constructor body runs to
// completion before derived's constructor body starts.
class BaseWithLogging
{
    public BaseWithLogging()
    {
        Console.WriteLine("  BaseWithLogging: constructor running");
    }
}

sealed class DerivedWithLogging : BaseWithLogging
{
    public DerivedWithLogging()
    {
        Console.WriteLine("  DerivedWithLogging: constructor running");
    }
}

// The gotcha: RiskyBase's constructor calls a virtual method. At that point,
// RiskyDerived's '_label' field is still at its zeroed default (null) — the
// derived constructor's field initializer and body haven't run yet.
abstract class RiskyBase
{
    protected RiskyBase()
    {
        // Calling a virtual method from a constructor, on 'this', is legal —
        // and dangerous, because 'this' is not fully constructed yet.
        Console.WriteLine($"  RiskyBase ctor sees Describe() = \"{Describe()}\"");
    }

    public abstract string Describe();
}

sealed class RiskyDerived : RiskyBase
{
    private readonly string _label;

    public RiskyDerived(string label)
    {
        // By the time THIS line runs, RiskyBase's constructor (and its call
        // to Describe(), above) has already completed. '_label' was still
        // null/default during that call — this assignment happens after.
        _label = label;
    }

    // Overrides the base's virtual method. When called from RiskyBase's
    // constructor, '_label' has not been assigned yet — it reads as its
    // zeroed default (null for a string), not "ready".
    public override string Describe() => _label ?? "(uninitialized)";
}
