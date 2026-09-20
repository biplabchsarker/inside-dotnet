using System.Reflection;

Console.WriteLine("=== Chapter 018: Records & Pattern Matching (Example) ===\n");

// 1. Positional Record Declaration & Synthesized Value Equality
Console.WriteLine("--- 1. Record Value Equality vs Class Reference Equality ---");

var p1 = new PersonRecord("Ada", "Lovelace", 36);
var p2 = new PersonRecord("Ada", "Lovelace", 36);
var c1 = new PersonClass("Ada", "Lovelace", 36);
var c2 = new PersonClass("Ada", "Lovelace", 36);

Console.WriteLine($"Record p1 == p2: {p1 == p2} (Value-based equality synthesized by Roslyn)");
Console.WriteLine($"Record p1.Equals(p2): {p1.Equals(p2)}");
Console.WriteLine($"Record HashCodes: p1={p1.GetHashCode()}, p2={p2.GetHashCode()} (Identical)");
Console.WriteLine($"Class  c1 == c2: {c1 == c2} (Reference-based equality; false!)");
Console.WriteLine($"Record ToString: {p1}");

// 2. Inspecting Synthesized Members via Reflection
Console.WriteLine("\n--- 2. Inspecting Roslyn-Synthesized Record Members ---");
Type recordType = typeof(PersonRecord);
Console.WriteLine($"Type: {recordType.Name}");
Console.WriteLine($"Implements IEquatable<PersonRecord>: {typeof(IEquatable<PersonRecord>).IsAssignableFrom(recordType)}");

Console.WriteLine("Synthesized Methods:");
foreach (var method in recordType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
{
    Console.WriteLine($"  - {method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
}

// 3. Non-Destructive Mutation via `with` Expression
Console.WriteLine("\n--- 3. Non-Destructive Mutation via `with` ---");
var p3 = p1 with { Age = 37 };
Console.WriteLine($"Original p1: {p1}");
Console.WriteLine($"Cloned   p3: {p3}");
Console.WriteLine($"ReferenceEquals(p1, p3): {ReferenceEquals(p1, p3)} (Fresh heap instance)");

// 4. Basic Pattern Matching with Records
Console.WriteLine("\n--- 4. Pattern Matching with Type & Property Patterns ---");
object[] entities = { p1, p3, new OrderRecord(5001, 199.99m), "string token" };

foreach (var entity in entities)
{
    string description = entity switch
    {
        PersonRecord { Age: >= 37 } senior => $"Senior Engineer: {senior.FirstName} (Age {senior.Age})",
        PersonRecord person => $"Team Member: {person.FirstName} {person.LastName}",
        OrderRecord { Amount: > 100m } bigOrder => $"High-Value Order #{bigOrder.Id} ({bigOrder.Amount:C})",
        OrderRecord order => $"Standard Order #{order.Id} ({order.Amount:C})",
        _ => "Unknown Entity"
    };
    Console.WriteLine($"  {description}");
}

// Declarations
public record PersonRecord(string FirstName, string LastName, int Age);

public class PersonClass
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public PersonClass(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}

public record OrderRecord(int Id, decimal Amount);
