# Exercises — OOP Fundamentals

### Exercise 1: Build a small shape hierarchy

Create an abstract `Shape` class with an abstract `Area()` method and a virtual `Describe()` method that prints the shape's name and area. Implement `Circle` and `Rectangle`, each overriding `Area()`. Store several shapes in a `List<Shape>` and call `Describe()` on each through the base type — confirm each one reports its own correct area.

### Exercise 2: Reproduce the `override` vs. `new` gotcha yourself

Write your own `BaseType`/`DerivedOverride`/`DerivedHiding` pair (or reuse the ones from [`code/Advanced/`](code/Advanced/)), but this time store both derived instances in a `List<BaseType>` and call the method through the list. Predict, then verify, which version runs for each — and explain why in your own words before checking [`code/Advanced/Program.cs`](code/Advanced/Program.cs).

### Exercise 3: Measure a devirtualized call yourself

Take [`code/Performance/Program.cs`](code/Performance/Program.cs) and add a fifth benchmark: a virtual call where the field's *declared* type is the sealed class directly (not `Base`) but the underlying method is `virtual` rather than `sealed override`. Run it and compare the result to `SealedVirtualCall` — does declaring the field as the sealed type alone produce the same devirtualization, or does the method also need `sealed override`?

---

## Challenge

Predict exactly what this program prints — *before* running it.

```csharp
var d = new Derived();

class Base
{
    public Base()
    {
        Initialize();
    }

    protected virtual void Initialize() => Console.WriteLine("Base.Initialize");
}

class Derived : Base
{
    private string _name; // no field initializer — assigned in the constructor BODY below

    public Derived()
    {
        _name = "Derived";
    }

    protected override void Initialize() => Console.WriteLine($"Derived.Initialize, name = '{_name}'");
}
```

<details>
<summary>Challenge Answer</summary>

It prints:

```
Derived.Initialize, name = ''
```

Not `'Derived'`. `Base`'s constructor calls `Initialize()`, which is virtual, so it dispatches to `Derived.Initialize` — the object's Method Table already points at `Derived`, even while `Base`'s constructor is still running. But `_name = "Derived"` is a statement inside `Derived`'s constructor **body**, and that body only starts running *after* `Base`'s constructor returns. At the moment `Initialize()` actually runs, `_name` is still `null` (printed as `''` by the interpolated string) — but written this way, so it doesn't throw.

**The subtlety worth internalizing:** it's specifically the constructor *body* that hasn't run yet — a field **initializer** (`private string _name = "Derived";` written directly on the declaration) actually *does* run before the base constructor is invoked, so that variant of this same program would print `'Derived'`, not `''`. Verify both forms yourself in [`code/Advanced/`](code/Advanced/) — this is the kind of "everyone assumes it works one way, verify before you trust it" detail this book insists on checking rather than repeating from memory.
</details>
