Console.WriteLine("=== Inside .NET: Episode 7 demo — Value Types vs Reference Types ===");

// ---------------------------------------------------------------------------
// 1. Value type copy semantics: mutating a copy never touches the original.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 1. Value type: copy by value ---");

Point a = new(1, 2);
Point b = a;          // full field-by-field copy into b
b.X = 99;

Console.WriteLine($"a = {a}");   // (1, 2) — untouched
Console.WriteLine($"b = {b}");   // (99, 2) — independent copy

// ---------------------------------------------------------------------------
// 2. Reference type sharing: mutating through one reference is visible
//    through every other reference to the same object.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 2. Reference type: copy by reference ---");

Customer c1 = new() { Name = "Alice" };
Customer c2 = c1;     // c2 now points at the SAME object as c1
c2.Name = "Bob";

Console.WriteLine($"c1.Name = {c1.Name}"); // Bob — same object, seen through c1 too
Console.WriteLine($"c2.Name = {c2.Name}"); // Bob
Console.WriteLine($"ReferenceEquals(c1, c2) = {ReferenceEquals(c1, c2)}"); // True

// ---------------------------------------------------------------------------
// 3. The mutable-struct gotcha, demonstrated safely.
//    The compiler blocks the most obvious mistake outright: a `foreach`
//    iteration variable is implicitly read-only, so `p.X = 999;` below
//    would fail with CS1654 ("Cannot modify members of 'p' because it is
//    a 'foreach iteration variable'") if uncommented — the SAFE outcome.
//    The genuinely dangerous version compiles cleanly: grab a struct
//    through anything that hands back a COPY (a method return, a
//    property getter) and mutate that copy — nothing observable changes.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 3. Mutable struct gotcha ---");

List<MutablePoint> points = new()
{
    new MutablePoint { X = 1, Y = 1 },
    new MutablePoint { X = 2, Y = 2 },
    new MutablePoint { X = 3, Y = 3 },
};

// foreach (MutablePoint p in points) { p.X = 999; } // CS1654 — does not compile

MutablePoint firstCopy = GetFirst(points); // returns a COPY of points[0]
firstCopy.X = 999;                          // mutates only the local copy

Console.WriteLine("After mutating a copy returned by a method:");
foreach (MutablePoint p in points)
{
    Console.WriteLine($"  {p}"); // still X=1,2,3 — the list was never touched
}

// The correct way to actually mutate elements in place: rewrite by index.
for (int i = 0; i < points.Count; i++)
{
    MutablePoint copy = points[i]; // still a copy...
    copy.X = 999;
    points[i] = copy;              // ...but explicitly written back
}

Console.WriteLine("After explicit index rewrite:");
foreach (MutablePoint p in points)
{
    Console.WriteLine($"  {p}"); // now X=999 for all
}

// ---------------------------------------------------------------------------
// 4. readonly struct + in parameter: avoid copying a large struct
//    across a call boundary while keeping it immutable.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 4. readonly struct passed by `in` ---");

LargeStruct big = new(1, 2, 3, 4, 5, 6, 7, 8);
Console.WriteLine($"Sum via in-parameter (no copy): {SumByIn(in big)}");

// ---------------------------------------------------------------------------
// 5. record class vs record struct: same generated equality contract,
//    different copy/storage semantics underneath.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 5. record class vs record struct ---");

PointRC rc1 = new(5, 6);
PointRC rc2 = new(5, 6);
Console.WriteLine($"rc1 == rc2                 : {rc1 == rc2}");                 // True (value equality)
Console.WriteLine($"ReferenceEquals(rc1, rc2)   : {ReferenceEquals(rc1, rc2)}");  // False (different objects)

PointRS rs1 = new(5, 6);
PointRS rs2 = rs1; // full value copy, independent instance
rs2 = rs2 with { X = 42 };
Console.WriteLine($"rs1 == rs2                 : {rs1 == rs2}");                 // False (different values now)
Console.WriteLine($"rs1                        : {rs1}");                        // unchanged
Console.WriteLine($"rs2                        : {rs2}");                        // X = 42

Console.WriteLine();
Console.WriteLine("=== Done ===");

static double SumByIn(in LargeStruct s) => s.Sum();
static MutablePoint GetFirst(List<MutablePoint> list) => list[0]; // returns a copy

// ===========================================================================
// Type definitions
// ===========================================================================

// A plain value type — copying it copies X and Y.
struct Point
{
    public int X, Y;
    public Point(int x, int y) { X = x; Y = y; }
    public override string ToString() => $"({X}, {Y})";
}

// A plain reference type — copying a variable of this type copies the reference.
class Customer
{
    public string Name = "";
}

// A deliberately mutable struct, used only to demonstrate the foreach gotcha.
// In real code, prefer `readonly struct` to make this class of mistake a
// compile error instead of a silent no-op.
struct MutablePoint
{
    public int X, Y;
    public override string ToString() => $"({X}, {Y})";
}

// A larger, immutable struct — a realistic candidate for `in` passing
// instead of pass-by-value, to avoid repeatedly copying all 8 fields.
readonly struct LargeStruct
{
    private readonly double _a, _b, _c, _d, _e, _f, _g, _h;

    public LargeStruct(double a, double b, double c, double d, double e, double f, double g, double h)
    {
        _a = a; _b = b; _c = c; _d = d; _e = e; _f = f; _g = g; _h = h;
    }

    public double Sum() => _a + _b + _c + _d + _e + _f + _g + _h;
}

// record class: reference type, generated value equality.
record class PointRC(int X, int Y);

// record struct: value type, generated value equality.
record struct PointRS(int X, int Y);
