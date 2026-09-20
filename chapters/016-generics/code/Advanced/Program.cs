using System.Reflection;
using System.Runtime.CompilerServices;

Console.WriteLine("=== Chapter 016: Generics Under the Hood (Advanced) ===\n");

// 1. Proving Code Sharing (Reference Types) vs Specialization (Value Types)
Console.WriteLine("--- 1. JIT Native Code Pointers ---");

// Force JIT compilation of closed methods
RuntimeHelpers.PrepareMethod(typeof(GenericWorker<int>).GetMethod("DoWork")!.MethodHandle);
RuntimeHelpers.PrepareMethod(typeof(GenericWorker<double>).GetMethod("DoWork")!.MethodHandle);
RuntimeHelpers.PrepareMethod(typeof(GenericWorker<string>).GetMethod("DoWork")!.MethodHandle);
RuntimeHelpers.PrepareMethod(typeof(GenericWorker<object>).GetMethod("DoWork")!.MethodHandle);
RuntimeHelpers.PrepareMethod(typeof(GenericWorker<Uri>).GetMethod("DoWork")!.MethodHandle);

IntPtr ptrInt    = typeof(GenericWorker<int>).GetMethod("DoWork")!.MethodHandle.GetFunctionPointer();
IntPtr ptrDouble = typeof(GenericWorker<double>).GetMethod("DoWork")!.MethodHandle.GetFunctionPointer();
IntPtr ptrString = typeof(GenericWorker<string>).GetMethod("DoWork")!.MethodHandle.GetFunctionPointer();
IntPtr ptrObject = typeof(GenericWorker<object>).GetMethod("DoWork")!.MethodHandle.GetFunctionPointer();
IntPtr ptrUri    = typeof(GenericWorker<Uri>).GetMethod("DoWork")!.MethodHandle.GetFunctionPointer();

Console.WriteLine($"[Value Type]     GenericWorker<int>.DoWork:    0x{ptrInt:X}");
Console.WriteLine($"[Value Type]     GenericWorker<double>.DoWork: 0x{ptrDouble:X}");
Console.WriteLine($"[Reference Type] GenericWorker<string>.DoWork: 0x{ptrString:X}");
Console.WriteLine($"[Reference Type] GenericWorker<object>.DoWork: 0x{ptrObject:X}");
Console.WriteLine($"[Reference Type] GenericWorker<Uri>.DoWork:    0x{ptrUri:X}");

Console.WriteLine($"\nNotice: Reference types share the exact same code pointer (Canon)? {ptrString == ptrObject && ptrString == ptrUri}");
Console.WriteLine($"Notice: Value types have distinct specialized pointers? {ptrInt != ptrDouble}");

// 2. Covariance and Contravariance
Console.WriteLine("\n--- 2. Covariance (out) and Contravariance (in) ---");

// Covariance: IEnumerable<out T> preserves the inheritance hierarchy
IEnumerable<Dog> dogs = new List<Dog> { new Dog("Buddy"), new Dog("Rex") };
IEnumerable<Animal> animals = dogs; // Allowed because of 'out' variance!
foreach (var a in animals) Console.WriteLine($"  Animal name: {a.Name}");

// Contravariance: IComparer<in T> allows using a broader comparer for derived types
IComparer<Animal> animalComparer = new AnimalComparer();
IComparer<Dog> dogComparer = animalComparer; // Allowed because of 'in' variance!
Console.WriteLine($"  dogComparer compares dogs using general Animal comparer: {dogComparer.Compare(new Dog("Alpha"), new Dog("Beta")) < 0}");

// 3. Constrained Generic Devirtualization
Console.WriteLine("\n--- 3. Constrained Generic Devirtualization ---");
RunBenchmark(new StructProcessor());

static void RunBenchmark<T>(T processor) where T : struct, IProcessor
{
    // Because T is constrained to a struct, the JIT emits a DIRECT call (no boxing, no vtable dispatch)!
    processor.Process();
}

public class GenericWorker<T>
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void DoWork() => Console.WriteLine(typeof(T).Name);
}

public class Animal
{
    public string Name { get; }
    public Animal(string name) => Name = name;
}

public class Dog : Animal
{
    public Dog(string name) : base(name) { }
}

public class AnimalComparer : IComparer<Animal>
{
    public int Compare(Animal? x, Animal? y) => string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
}

public interface IProcessor
{
    void Process();
}

public struct StructProcessor : IProcessor
{
    public void Process() => Console.WriteLine("  StructProcessor executed directly with ZERO boxing!");
}
