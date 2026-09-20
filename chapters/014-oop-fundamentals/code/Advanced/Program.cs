// Program.cs — .NET 10 console app
// Demonstrates the classic override-vs-new (method hiding) gotcha, and how the
// CLR resolves each one completely differently: override is resolved from the
// OBJECT's own method table at the call site (late-bound); `new` is resolved
// from the COMPILE-TIME (static) type of the reference (early-bound) — the
// same call site can print two different things depending only on which
// keyword the derived class used, with zero change to the calling code.

Console.WriteLine("=== Inside .NET: Episode 15 — override vs. new (method hiding) ===\n");

DerivedOverride overriding = new();
BaseType overridingAsBase = overriding;

Console.WriteLine("--- override: same output through either reference type ---");
Console.WriteLine($"Called through DerivedOverride reference: ");
overriding.Greet();
Console.WriteLine($"Called through BaseType reference (same object): ");
overridingAsBase.Greet();

Console.WriteLine();

DerivedHiding hiding = new();
BaseType hidingAsBase = hiding;

Console.WriteLine("--- new (hiding): output CHANGES depending on the reference's declared type ---");
Console.WriteLine($"Called through DerivedHiding reference: ");
hiding.Greet();
Console.WriteLine($"Called through BaseType reference (same object): ");
hidingAsBase.Greet();

Console.WriteLine();
Console.WriteLine("Same underlying object in both cases. The `override` version is dispatched");
Console.WriteLine("through the object's own method table (virtual, run-time bound), so it can");
Console.WriteLine("never be 'un-overridden' by the reference type used to call it. The `new`");
Console.WriteLine("version is an entirely separate, non-virtual method resolved at COMPILE TIME");
Console.WriteLine("from whatever the reference is DECLARED as — hence the different output above.");

class BaseType
{
    public virtual void Greet() => Console.WriteLine("  BaseType.Greet (virtual)");
}

class DerivedOverride : BaseType
{
    // Replaces BaseType's vtable slot for Greet. Every caller, regardless of
    // the declared reference type, ends up here for an instance of this class.
    public override void Greet() => Console.WriteLine("  DerivedOverride.Greet (override)");
}

class DerivedHiding : BaseType
{
    // `new` does NOT touch BaseType's vtable slot at all — it declares a brand
    // new, unrelated, non-virtual method that happens to share a name. Calling
    // it through a BaseType-typed reference never reaches this method; the
    // compiler binds that call to BaseType.Greet at COMPILE time instead.
    public new void Greet() => Console.WriteLine("  DerivedHiding.Greet (new / hiding)");
}
