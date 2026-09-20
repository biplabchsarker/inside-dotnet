using System.Runtime.CompilerServices;

Console.WriteLine("=== Chapter 018: Records & Pattern Matching (Advanced) ===\n");

// 1. Record Class vs Record Struct vs Readonly Record Struct
Console.WriteLine("--- 1. Record Class vs Record Struct (Allocation & Mutability) ---");

// Record class (Heap allocated reference type)
var recClass = new PointRecordClass(10, 20);

// Record struct (Stack allocated value type, mutable by default)
var recStruct = new PointRecordStruct(10, 20);
recStruct.X = 15; // Valid because record struct is mutable by default

// Readonly Record struct (Stack allocated value type, completely immutable)
var readOnlyRecStruct = new PointReadonlyRecordStruct(10, 20);
// readOnlyRecStruct.X = 15; // Compile error! CS8852: Init-only property

Console.WriteLine($"Record Class:           {recClass} (IsValueType: {typeof(PointRecordClass).IsValueType})");
Console.WriteLine($"Record Struct:          {recStruct} (IsValueType: {typeof(PointRecordStruct).IsValueType})");
Console.WriteLine($"Readonly Record Struct: {readOnlyRecStruct} (IsValueType: {typeof(PointReadonlyRecordStruct).IsValueType})");

// 2. Deconstruction and Positional Matching
Console.WriteLine("\n--- 2. Deconstruction and Positional Patterns ---");
var (x, y) = recClass;
Console.WriteLine($"Deconstructed Point: x={x}, y={y}");

// 3. Deep Dive: Advanced Pattern Matching
Console.WriteLine("\n--- 3. Advanced Pattern Matching (Relational, List, and Slice Patterns) ---");

// A. Relational and Logical Combinator Patterns
static string CategorizeSpeed(double speedKmH) => speedKmH switch
{
    <= 0 => "Stationary",
    > 0 and <= 50 => "City Speed",
    > 50 and <= 120 => "Highway Speed",
    > 120 and <= 300 => "High-Speed Rail / Autobahn",
    _ => "Aviation / Supersonic"
};

Console.WriteLine($"Speed 45 km/h:  {CategorizeSpeed(45)}");
Console.WriteLine($"Speed 110 km/h: {CategorizeSpeed(110)}");
Console.WriteLine($"Speed 350 km/h: {CategorizeSpeed(350)}");

// B. List and Slice Patterns (C# 11+)
static string EvaluateSequence(int[] numbers) => numbers switch
{
    [] => "Empty sequence",
    [var single] => $"Single element: {single}",
    [1, 2, .. var rest] => $"Starts with 1, 2 followed by {rest.Length} elements",
    [.., var last] when last < 0 => $"Sequence ends with negative number: {last}",
    [var first, .., var last] => $"Multiple elements: first={first}, last={last}",
};

Console.WriteLine($"\nList pattern [1, 2, 3, 4, 5]: {EvaluateSequence(new[] { 1, 2, 3, 4, 5 })}");
Console.WriteLine($"List pattern [99, 100]:        {EvaluateSequence(new[] { 99, 100 })}");
Console.WriteLine($"List pattern [5, 4, -1]:       {EvaluateSequence(new[] { 5, 4, -1 })}");

// 4. EqualityContract and Polymorphic Record Equality
Console.WriteLine("\n--- 4. EqualityContract & Record Inheritance ---");
EmployeeRecord emp1 = new("Grace", "Hopper", 40, "Admiral / Engineering");
PersonRecord per1 = new("Grace", "Hopper", 40);

Console.WriteLine($"emp1: {emp1}");
Console.WriteLine($"per1: {per1}");
Console.WriteLine($"emp1.Equals(per1): {emp1.Equals(per1)} (False: EqualityContract prevents cross-type equality)");

// Declarations
public record PointRecordClass(int X, int Y);
public record struct PointRecordStruct(int X, int Y);
public readonly record struct PointReadonlyRecordStruct(int X, int Y);

public record PersonRecord(string FirstName, string LastName, int Age);

public record EmployeeRecord(string FirstName, string LastName, int Age, string Department)
    : PersonRecord(FirstName, LastName, Age);
