// Program.cs — .NET 10 console app
// Demonstrates: (1) a struct as a local vs. the same struct as a field of a
// class instance, observed via heap growth; (2) why unbounded recursion is
// not safely demoable (explained, not executed).

Console.WriteLine("=== Inside .NET: Episode 6 — Stack vs Heap demo ===");

// ---------------------------------------------------------------------------
// Part 1: a struct (value type) as a LOCAL variable.
// 'point' lives in this method's stack frame. Copying it copies the whole
// 12 bytes right there on the stack — no heap allocation happens at all.
// ---------------------------------------------------------------------------
Point3D point = new Point3D(1, 2, 3);
Point3D copy = point; // full value copy, stack-to-stack, no heap involved
copy.X = 99;
Console.WriteLine($"\n[Local struct] point.X={point.X} (unchanged), copy.X={copy.X}");

long before = GC.GetTotalMemory(forceFullCollection: true);

// Allocating many *locals* of a struct: no lasting heap growth, because each
// one is stack-resident for the duration of one loop iteration and never
// escapes into anything that outlives the loop.
long sink = 0;
for (int i = 0; i < 1_000_000; i++)
{
    Point3D transient = new Point3D(i, i, i);
    sink += transient.X + transient.Y; // touch it so the JIT can't optimize it away entirely
}

long afterLocals = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"[Struct locals]      heap before: {before,12:N0} bytes | after: {afterLocals,12:N0} bytes | delta: {afterLocals - before,12:N0} bytes  (sink={sink})");

// ---------------------------------------------------------------------------
// Part 2: the SAME struct as a FIELD of a class instance.
// Now each Point3D lives INSIDE a heap object (PointHolder). The struct
// itself didn't change — where it lives changed, because its container did.
// ---------------------------------------------------------------------------
var holders = new List<PointHolder>(capacity: 1_000_000);
long beforeHeap = GC.GetTotalMemory(forceFullCollection: true);

for (int i = 0; i < 1_000_000; i++)
{
    holders.Add(new PointHolder(new Point3D(i, i, i))); // PointHolder is a class -> heap
}

long afterHeap = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"[Struct-in-class]    heap before: {beforeHeap,12:N0} bytes | after: {afterHeap,12:N0} bytes | delta: {afterHeap - beforeHeap,12:N0} bytes");
Console.WriteLine($"Held {holders.Count:N0} PointHolder instances, each carrying a Point3D field inline.");

// Keep 'holders' referenced until here so the GC can't collect it mid-measurement.
Console.WriteLine($"Sample check: holders[500000].Point.X = {holders[500_000].Point.X}");

// ---------------------------------------------------------------------------
// Part 3: boxing — a third way a value type ends up on the heap.
// ---------------------------------------------------------------------------
Point3D boxedSource = new Point3D(7, 8, 9);
object boxed = boxedSource; // boxing: allocates a heap object wrapping the struct
Console.WriteLine($"\n[Boxing] boxed object type: {boxed.GetType()} — this Point3D now lives on the heap, wrapped.");

// ---------------------------------------------------------------------------
// Part 4: StackOverflowException — explained, never triggered.
//
// Uncontrolled recursion exhausts the current thread's stack (default ~1MB
// on Windows for the main thread). When the CPU/OS detects the guard page
// at the end of the stack has been hit, the CLR cannot run a catch handler
// or even its own exception-dispatch code, because doing so requires MORE
// stack space than exists. The process is torn down immediately by the
// runtime — StackOverflowException cannot be caught by user code (see
// article.md for the full mechanism). Uncomment the two lines below ONLY
// in a disposable process/terminal if you want to see it happen for real —
// it WILL crash this process with no chance to catch anything.
// ---------------------------------------------------------------------------
// static void RecurseForever(int depth) => RecurseForever(depth + 1);
// RecurseForever(0);

Console.WriteLine("\nDone. (Uncontrolled recursion demo intentionally left disabled — see comment above.)");

// A value type: when declared as a local, it's stack-resident.
// When it's a field of a class, it becomes part of that class's heap layout.
struct Point3D
{
    public int X;
    public int Y;
    public int Z;

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

// A reference type (class). Its Point3D field is laid out INLINE inside
// this object's heap block — it is not a separate heap allocation, and it
// is not "on the stack" just because it's a struct.
class PointHolder
{
    public Point3D Point;

    public PointHolder(Point3D point) => Point = point;
}
