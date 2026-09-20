Console.WriteLine("=== Chapter 016: Generics Under the Hood (Example) ===\n");

// 1. Full Runtime Type Reification (Contrast with Java's Type Erasure)
Console.WriteLine("--- 1. Reification: Types are Preserved at Runtime ---");
PrintTypeDetails<int>();
PrintTypeDetails<string>();
PrintTypeDetails<DateTime>();

// 2. Static Fields in Generic Classes: Each closed type gets its own static storage!
Console.WriteLine("\n--- 2. Static Field Partitioning ---");
GenericState<int>.Counter = 100;
GenericState<string>.Counter = 500;
GenericState<bool>.Counter = 999;

Console.WriteLine($"GenericState<int>.Counter    = {GenericState<int>.Counter}");
Console.WriteLine($"GenericState<string>.Counter = {GenericState<string>.Counter}");
Console.WriteLine($"GenericState<bool>.Counter   = {GenericState<bool>.Counter}");

// 3. Generic Constraints with Modern .NET Interfaces (INumber<T>)
Console.WriteLine("\n--- 3. Constrained Math with Modern .NET Generic Math ---");
Console.WriteLine($"Add(10, 25)           = {AddNumbers(10, 25)}");
Console.WriteLine($"Add(3.1415, 2.7182)   = {AddNumbers(3.1415, 2.7182)}");

static void PrintTypeDetails<T>()
{
    Type t = typeof(T);
    Console.WriteLine($"Type: {t.FullName}");
    Console.WriteLine($"  IsValueType: {t.IsValueType}, SizeInBytes: {(t.IsValueType ? System.Runtime.CompilerServices.Unsafe.SizeOf<T>() : IntPtr.Size)}");
    Console.WriteLine($"  GUID: {t.GUID}");
}

static T AddNumbers<T>(T a, T b) where T : System.Numerics.INumber<T>
{
    // INumber<T> uses static virtual members in interfaces!
    return a + b;
}

public class GenericState<T>
{
    public static int Counter;
    static GenericState()
    {
        Console.WriteLine($"  [Static ctor] Initialized static storage for closed type: GenericState<{typeof(T).Name}>");
    }
}
