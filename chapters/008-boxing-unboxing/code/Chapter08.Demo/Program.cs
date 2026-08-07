// Program.cs — .NET 10 console app
// Demonstrates: (1) boxing identity — every box is its own heap object, even for
// the same value; (2) unboxing's exact-type requirement, made concrete via the
// exception it throws when violated; (3) the ArrayList-vs-List<int> boxing gap,
// measured directly with GC.GetAllocatedBytesForCurrentThread(); (4) boxing
// triggered by interface dispatch on a struct, and how a generic constraint
// avoids it; (5) the classic "mutating a boxed struct through an interface
// doesn't mutate your original variable" gotcha, made concrete instead of just
// described.

Console.WriteLine("=== Inside .NET: Episode 9 — Boxing & Unboxing demo ===");

// ---------------------------------------------------------------------------
// Part 1: boxing identity — two boxes of the same value are two distinct
// heap objects. object.ReferenceEquals proves it; there is no interning for
// boxed value types the way there is for small strings.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 1. Boxing identity: same value, different boxes ---");

int number = 42;
object boxedA = number;
object boxedB = number;

Console.WriteLine($"  boxedA equals boxedB (value)?     {boxedA.Equals(boxedB)}");
Console.WriteLine($"  boxedA is boxedB (reference)?     {ReferenceEquals(boxedA, boxedB)}");
Console.WriteLine($"  Mutating 'number' after boxing doesn't touch either box:");
number = 99;
Console.WriteLine($"  number={number}, boxedA={boxedA}, boxedB={boxedB}");

// ---------------------------------------------------------------------------
// Part 2: unboxing demands the exact boxed type, not merely a compatible one.
// Unboxing is a type check followed by a copy — the check is what throws here.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 2. Unboxing requires the exact original type ---");

object boxedInt = 7;
int okUnbox = (int)boxedInt; // exact match: int -> object -> int. Fine.
Console.WriteLine($"  Unboxing to the original type 'int' works: {okUnbox}");

try
{
    long mismatched = (long)boxedInt; // int and long are different types; no implicit widening during unboxing.
    Console.WriteLine($"  Unreachable: {mismatched}");
}
catch (InvalidCastException ex)
{
    Console.WriteLine($"  Unboxing to 'long' threw: {ex.Message}");
}

// ---------------------------------------------------------------------------
// Part 3: ArrayList boxes every value type it stores; List<int> doesn't.
// Measured via the same per-thread allocation counter used in Episode 8 —
// this isolates the allocator's contribution, not wall-clock time.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 3. ArrayList (boxes) vs List<int> (doesn't) — measured allocation ---");

const int itemCount = 1_000_000;

long beforeArrayList = GC.GetAllocatedBytesForCurrentThread();
var arrayList = new System.Collections.ArrayList(itemCount);
for (int i = 0; i < itemCount; i++)
{
    arrayList.Add(i); // each 'i' is boxed here — ArrayList only knows how to store object.
}
long afterArrayList = GC.GetAllocatedBytesForCurrentThread();

long beforeList = GC.GetAllocatedBytesForCurrentThread();
var typedList = new List<int>(itemCount);
for (int i = 0; i < itemCount; i++)
{
    typedList.Add(i); // no boxing — List<int> stores the int inline in its backing array.
}
long afterList = GC.GetAllocatedBytesForCurrentThread();

long arrayListBytes = afterArrayList - beforeArrayList;
long listBytes = afterList - beforeList;

Console.WriteLine($"  ArrayList.Add x{itemCount:N0}:  {arrayListBytes:N0} bytes allocated");
Console.WriteLine($"  List<int>.Add x{itemCount:N0}:  {listBytes:N0} bytes allocated");
Console.WriteLine($"  Ratio (ArrayList / List<int>): {(double)arrayListBytes / listBytes:N1}x");

// ---------------------------------------------------------------------------
// Part 4: calling an interface member through the interface type boxes a
// struct; calling the same member directly, or through a generic constraint,
// does not. Same struct, same method, different call shape.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 4. Interface dispatch boxes a struct; generic constraints don't ---");

var counter = new Counter(5);

Console.WriteLine($"  Direct call, no boxing:            counter.Value = {counter.Value}");

IIncrementable boxedCounter = counter; // <- this assignment is where the box happens.
Console.WriteLine($"  Through IIncrementable (boxed):     boxedCounter.Value = {boxedCounter.Value}");

int viaGeneric = DescribeWithoutBoxing(counter); // generic constraint dispatches on the concrete struct type — no box.
Console.WriteLine($"  Through a generic constraint:      DescribeWithoutBoxing(counter) = {viaGeneric}");

// ---------------------------------------------------------------------------
// Part 5: the gotcha — calling a mutating method through a boxed reference
// mutates the BOX, not the original struct variable. The box and the
// variable are, after boxing, two completely separate copies.
// ---------------------------------------------------------------------------
Console.WriteLine("\n--- 5. Mutating a boxed struct through an interface doesn't touch your variable ---");

var original = new Counter(0);
IIncrementable boxed = original; // boxes 'original' into a new heap object right here.

boxed.Increment(); // mutates the fields INSIDE THE BOX. 'original' is untouched.
boxed.Increment();

Console.WriteLine($"  original.Value (unchanged):  {original.Value}");
Console.WriteLine($"  ((Counter)boxed).Value (the box's state): {((Counter)boxed).Value}");

Console.WriteLine("\n=== Done ===");

// A generic method constrained to IIncrementable dispatches through the JIT's
// per-value-type specialization (Episode 16 territory) instead of an
// interface reference — the struct is passed by value into the generic
// instantiation, no heap box required.
static int DescribeWithoutBoxing<T>(T incrementable) where T : IIncrementable
    => incrementable.Value;

// A small mutable struct, deliberately not readonly, so Part 5's gotcha is
// reachable: readonly structs can still be boxed, but a readonly struct's
// interface methods can't mutate anything, which would hide the point here.
interface IIncrementable
{
    int Value { get; }
    void Increment();
}

struct Counter(int startingValue) : IIncrementable
{
    private int _value = startingValue;

    public int Value => _value;

    public void Increment() => _value++;
}
