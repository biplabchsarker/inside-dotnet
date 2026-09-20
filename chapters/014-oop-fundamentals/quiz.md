# Quiz — OOP Fundamentals

1. Why can the same `List<Animal>`, iterated with the exact same loop, print three different things for three different elements?
2. If `DerivedHiding hiding = new(); BaseType asBase = hiding;`, does `asBase.Greet()` run `DerivedHiding`'s or `BaseType`'s version — and why?
3. What extra step does an interface method call perform that a class virtual call doesn't?
4. What has to be true about a type for the JIT to devirtualize a call to it?
5. Why does calling a virtual method from inside a constructor risk operating on uninitialized state — and specifically what kind of state is and isn't safe at that point?

<details>
<summary>Answers</summary>

1. Because a virtual call is dispatched through the *object's own* Method Table pointer, not the declared type of the list/reference. Each element's actual (run-time) type determines which override runs, even though every element is stored and accessed as `Animal`.
2. `BaseType`'s version (`"BaseType.Greet"`). `new` is resolved at compile time from the reference's *declared* type — `asBase` is declared as `BaseType`, so the compiler binds that call to `BaseType.Greet`, never reaching the hiding method at all.
3. It goes through a separate Interface Method Table rather than a class's single vtable, because one class can implement many interfaces but has only one (single-inheritance) vtable — that's one more layer of indirection than a class virtual call.
4. The target type (or the specific method) must be provably `sealed` — no further override can exist, so the JIT can replace the indirect vtable jump with a direct call.
5. Because the object's Method Table pointer is already set to the most-derived type before any constructor body runs. A virtual call from a base constructor reaches a derived override, but the derived class's own constructor *body* hasn't executed yet — so anything that body was going to set up isn't there. Field initializers (assigned inline on the declaration) already ran by this point and ARE safe to rely on; it's specifically constructor-body logic that isn't.

</details>
