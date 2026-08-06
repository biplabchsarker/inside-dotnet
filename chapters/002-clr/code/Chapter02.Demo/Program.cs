// Program.cs — .NET 10 console app
using System.Reflection;
using System.Runtime.Loader;

Console.WriteLine("=== Inside .NET: Episode 3 demo — CLR internals ===");

// 1. CTS in action: value type vs. reference type, from the SAME type system,
//    regardless of which keyword you used to declare them.
Console.WriteLine();
Console.WriteLine("-- CTS: value types vs. reference types --");
PrintTypeKind(typeof(int));         // value type -> System.ValueType
PrintTypeKind(typeof(OrderStruct)); // value type
PrintTypeKind(typeof(OrderClass));  // reference type -> System.Object

static void PrintTypeKind(Type t)
{
    Console.WriteLine($"{t.Name,-14} IsValueType={t.IsValueType,-6} BaseType={t.BaseType?.FullName}");
}

// 2. Method table identity: every instance of the same runtime type shares
//    the exact same type handle (the managed-code proxy for "method table pointer").
Console.WriteLine();
Console.WriteLine("-- Method table sharing across instances --");
var order1 = new OrderClass { OrderId = 1 };
var order2 = new OrderClass { OrderId = 2 };
RuntimeTypeHandle h1 = order1.GetType().TypeHandle;
RuntimeTypeHandle h2 = order2.GetType().TypeHandle;
Console.WriteLine($"order1 type handle == order2 type handle : {h1.Value == h2.Value}");
Console.WriteLine("(Both instances point at the SAME method table; only field data differs.)");

// 3. Virtual dispatch resolves against the RUNTIME type, not the declared/static type.
Console.WriteLine();
Console.WriteLine("-- Virtual dispatch: declared type vs. runtime type --");
Base b = new Derived();               // declared type: Base, runtime type: Derived
Console.WriteLine($"Declared type: {typeof(Base).Name}, Runtime type: {b.GetType().Name}");
Console.WriteLine($"b.Describe() -> \"{b.Describe()}\"  (vtable slot resolves to Derived's override)");

NonVirtualBase nv = new Derived2();
Console.WriteLine($"nv.Describe() -> \"{nv.Describe()}\"  (non-virtual: always Base's method, no override possible)");

// 4. Timing difference between a megamorphic virtual call path and a
//    direct/non-virtual call path — illustrative, not a rigorous benchmark.
Console.WriteLine();
Console.WriteLine("-- Virtual vs non-virtual call cost (illustrative) --");
const int iterations = 200_000_000;
IShape[] shapes = { new Circle(), new Square(), new Circle(), new Square() };

var swVirtual = System.Diagnostics.Stopwatch.StartNew();
double totalVirtual = 0;
for (int i = 0; i < iterations; i++)
    totalVirtual += shapes[i & 3].Area(); // interface (virtual) dispatch every call
swVirtual.Stop();

var swDirect = System.Diagnostics.Stopwatch.StartNew();
double totalDirect = 0;
var square = new Square();
for (int i = 0; i < iterations; i++)
    totalDirect += square.DirectArea(); // sealed type, non-virtual, JIT can inline
swDirect.Stop();

Console.WriteLine($"Virtual (interface) dispatch : {swVirtual.ElapsedMilliseconds,6} ms  (sum={totalVirtual:F0})");
Console.WriteLine($"Non-virtual / inlinable call : {swDirect.ElapsedMilliseconds,6} ms  (sum={totalDirect:F0})");

// 5. AssemblyLoadContext: the modern replacement for AppDomain-based isolation.
Console.WriteLine();
Console.WriteLine("-- AssemblyLoadContext (modern AppDomain replacement) --");
AssemblyLoadContext defaultAlc = AssemblyLoadContext.Default;
Console.WriteLine($"Default ALC name      : {defaultAlc.Name}");
Console.WriteLine($"Is collectible         : {defaultAlc.IsCollectible}");
Console.WriteLine("Loaded assemblies (first 5):");
foreach (var asm in defaultAlc.Assemblies.Take(5))
    Console.WriteLine($"  - {asm.GetName().Name}");

// ---- Supporting types ----

#pragma warning disable CS0649 // OrderId is only used to demonstrate CTS value-type layout, never read
struct OrderStruct { public int OrderId; }
#pragma warning restore CS0649
class OrderClass { public int OrderId; }

class Base
{
    public virtual string Describe() => "Base.Describe (should be overridden)";
}
class Derived : Base
{
    public override string Describe() => "Derived.Describe (vtable slot overridden)";
}

class NonVirtualBase
{
    public string Describe() => "NonVirtualBase.Describe (not virtual — no vtable involved)";
}
class Derived2 : NonVirtualBase { }

interface IShape { double Area(); }
sealed class Circle : IShape { public double Area() => Math.PI * 2 * 2; }
sealed class Square : IShape
{
    public double Area() => 4 * 4;
    public double DirectArea() => 4 * 4; // called directly on a concrete sealed type
}
