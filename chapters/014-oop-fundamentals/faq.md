# Frequently Asked Questions — OOP Fundamentals

**Q: Does C# support multiple inheritance of classes?**
A: No — a class can inherit from exactly one base class, which is precisely why a class has one single vtable and interfaces need their own separate Interface Method Table. A class *can* implement multiple interfaces, which is where C# gets its multiple-inheritance-of-behavior flexibility from instead.

**Q: If `virtual` costs more than a direct call, should I avoid it entirely?**
A: No — avoid it where nothing will ever override it. Where polymorphism buys real flexibility (a plugin point, a DI-swappable strategy, genuine "is-a" extensibility), the ~57-59% overhead measured in this chapter is almost always worth paying. The point isn't "never virtual," it's "virtual on purpose, not out of habit."

**Q: Is `sealed` only about performance?**
A: No — `sealed`'s primary purpose is a design statement: "this class is not meant to be extended." The devirtualization benefit this chapter measured is a genuine side effect, not the main reason to reach for it.

**Q: Why did `new` even get added to C# if it's this easy to misuse?**
A: It exists for a narrow, legitimate case: a base class you don't control adds a method with the same name and signature as one you already wrote, and you need to avoid an accidental collision without breaking your existing (non-virtual) method's behavior. It is not a substitute for `override`, and the compiler warns (`CS0114`) if you write a method that hides a base member without either `override` or `new` — read that warning; don't suppress it reflexively.

**Q: What's the difference between hiding a method and hiding a field or property?**
A: Mechanically the same rule applies — `new` on a field or property also resolves by the reference's declared (compile-time) type, not the object's actual type — but fields and properties aren't virtual by default at all in C#, so this scenario is rarer in practice than the method-hiding gotcha this chapter focuses on.

**Q: Does boxing a struct that implements an interface use the same Interface Method Table dispatch?**
A: Yes, once boxed — a boxed struct becomes a heap object with its own Method Table, and a call through the interface reference dispatches through that boxed copy's Interface Method Table exactly like a class would. See [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md) for the boxing mechanics themselves.
