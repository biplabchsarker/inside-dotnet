# Exercises — Object Allocation

1. **Measure allocation-size sensitivity** — Starting from [`code/Chapter07.Demo/Program.cs`](code/Chapter07.Demo/Program.cs), add a second `AuditRecord`-like type with several extra `long` fields (make it noticeably larger — say, 8 extra fields instead of 2 total). Re-run the same before/after `GC.GetAllocatedBytesForCurrentThread()` measurement for 5,000,000 instances of the larger type. Confirm bytes-per-instance scales roughly with the added field size, and discuss in a sentence why a larger object size means more frequent allocation-context refills at the same allocation *rate* (same number of objects/second), even though each individual allocation is still "just a bump."

2. **Prove the zeroing guarantee empirically, not just by trusting the claim** — Write a class with several `int`, `bool`, and reference-typed fields but no constructor that assigns them. Allocate an instance with `new` and immediately print every field. Confirm they read as `0`, `false`, and `null` respectively — then explain, in your own words, why this chapter says the zeroing work did *not* happen at the moment your `new` executed, even though the guarantee clearly held.

3. **Break the base-constructor gotcha the other direction** — Starting from `RiskyBase`/`RiskyDerived` in `Program.cs`, fix the gotcha two different ways: (a) make `Describe()` non-virtual and have `RiskyBase`'s constructor call a private helper instead, and (b) keep `Describe()` virtual but move the field assignment into a field initializer (`private readonly string _label = "default";`) instead of the constructor body, then explain why fix (b) only *partially* helps — it changes what the base constructor's call to `Describe()` sees, but does it fully solve the general problem for every possible derived class? Justify your answer.

4. **Stretch — observe an allocation-context refill under real load** — Using `dotnet-counters monitor -- <your process>` (or `dotnet-counters monitor --process-id <pid>` attached to a running instance of `Chapter07.Demo` modified to loop the allocation phase indefinitely), watch `Allocation Rate` and `# of Gen 0 Collections` while the loop runs. Correlate a rise in allocation rate with a corresponding rise in Gen 0 collection frequency, and write down what you observe about the relationship — is it linear, and does it match this chapter's claim that context exhaustion (not a timer) is the trigger?

## Challenge

**Predict the output before running it.**

```csharp
abstract class Shape
{
    protected Shape()
    {
        Console.WriteLine($"Area at construction time: {Area()}");
    }

    public abstract double Area();
}

sealed class Square : Shape
{
    private readonly double _side;

    public Square(double side)
    {
        _side = side;
    }

    public override double Area() => _side * _side;
}

var square = new Square(5.0);
Console.WriteLine($"Area after construction: {square.Area()}");
```

Before running this: what does the first line print, and what does the second line print? Most people expect both lines to print `25`. Write down, in terms of allocation and construction order — not just "the compiler does something weird" — exactly why the first line prints what it prints, using this chapter's base-before-derived mechanics to justify your answer field by field.
