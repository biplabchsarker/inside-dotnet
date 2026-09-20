# Senior Interview Questions — OOP Fundamentals

### 1. What's the difference between `override` and hiding a method with `new`?
**Answer:**
`override` replaces the base method's own vtable slot — every caller reaches the derived implementation, regardless of the reference's declared type, because dispatch is resolved from the *object* at run time. `new` declares an entirely separate, non-virtual method resolved by the *compiler*, at compile time, from the reference's *declared* type — the same object can produce two different answers depending only on which type of reference calls it.

### 2. What actually happens inside the CLR during a virtual method call?
**Answer:**
The `callvirt` IL instruction null-checks the reference, reads the object's Method Table pointer (a hidden field at the start of every object), indexes into the correct vtable slot for that method, and makes an indirect jump to whatever address is stored there. A direct (non-virtual) call skips all of that — its target address is fixed at compile time.

### 3. Why can an interface method call cost slightly more than a class virtual call?
**Answer:**
A class has exactly one vtable (single inheritance), but it can implement any number of interfaces, so interface dispatch goes through a separate Interface Method Table rather than a straightforward vtable-slot index — one additional layer of indirection on top of what a class virtual call already does. Measured directly in this chapter: interface calls came in essentially tied with virtual calls, a hair slower.

### 4. What is devirtualization, and when can the JIT do it?
**Answer:**
Devirtualization is the JIT proving that a particular call site can only ever reach one possible method implementation, and replacing the indirect vtable jump with a direct call (sometimes inlining it outright). It's guaranteed when the target type is `sealed` (or the method is `sealed override`) — there is provably no further override that could exist. This chapter measured a sealed virtual call within ~4% of a genuinely direct call, confirming it actually happens, not just that it's theoretically possible.

### 5. Why is calling a virtual member from a constructor dangerous?
**Answer:**
An object's Method Table pointer is set to its most-derived type *before* any constructor body runs. If a base constructor calls a virtual method, a derived override runs — but the derived constructor's own *body* hasn't executed yet, so anything it was responsible for setting up isn't there. The precise, verified nuance: field **initializers** (assigned inline on the field declaration) already ran by this point and are safe; it's specifically logic inside the constructor body that isn't ready yet.

### 6. What's the difference between `abstract` and `virtual`?
**Answer:**
`virtual` provides a default implementation that a derived class *may* override. `abstract` provides no implementation at all and *forces* every concrete derived class to supply one — and a class containing any abstract member must itself be declared `abstract`, meaning it can never be instantiated directly.

### 7. Why does encapsulation matter if the CLR doesn't enforce it at run time the way it enforces type safety?
**Answer:**
Encapsulation (private fields, controlled access through methods/properties) is a *compile-time* discipline enforced by the C# compiler and the CLR's field-accessibility metadata — not a run-time GC-style guarantee. Its value is protecting a class's own invariants (e.g. "balance never goes negative") from being bypassed by code outside the class, which is exactly as real a bug-prevention mechanism as type safety, just checked at a different stage.
