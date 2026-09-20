# Summary — OOP Fundamentals

**TL;DR**
Every object carries a hidden Method Table pointer, and a virtual call is dispatched through *that* pointer — never through the declared type of the reference calling it. That single mechanism is why `override` always wins through any reference type, why `new` (hiding) can silently "disappear" through a base reference, and why virtual/interface calls cost measurably more (57-59% in this chapter's benchmark) than a direct call — a cost `sealed` lets the JIT erase almost entirely.

## The Core Concept

Encapsulation, inheritance, and polymorphism aren't just language keywords — they map to concrete CLR mechanics: a private field is enforced by the compiler, not the runtime; inheritance copies and extends a vtable; polymorphism is that vtable being indexed by the *object's own* type at the moment of the call, not by whatever type the calling code happens to have declared.

## override vs. new — the classic gotcha

- **`override`** replaces the base method's own vtable slot. Every caller reaches it, through any reference type, because dispatch is resolved from the object at run time.
- **`new` (hiding)** declares an unrelated, non-virtual method resolved at *compile time* from the reference's *declared* type. The same object can answer two different ways depending only on which reference type made the call.

## Measured dispatch cost

| Call kind | Relative cost |
|---|---|
| Direct (non-virtual) | 1.00× (baseline) |
| Sealed virtual (devirtualized) | 1.04× |
| Virtual (real vtable lookup) | 1.57× |
| Interface (interface method table) | 1.59× |

## The One-Line Rule

> A virtual call is dispatched through the OBJECT's own Method Table — never the declared type of the reference calling it. `sealed` lets the JIT prove that lookup was never going to matter, and skip it entirely.
