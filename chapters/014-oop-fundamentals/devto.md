---
title: OOP Fundamentals — What override, new, and virtual Actually Cost in .NET
published: false
tags: dotnet, csharp, oop, performance
---

## The four pillars, minus the hand-waving

Encapsulation, inheritance, polymorphism, abstraction — everyone can define them. Fewer people can say exactly what happens inside the CLR when you call a virtual method, or why `override` and `new` (method hiding) can make the *same object* behave two different ways.

Chapter 014 of **Inside .NET** covers both, with real numbers:

| Call kind | Measured cost |
|---|---|
| Direct (non-virtual) | 1.00× (baseline) |
| Sealed virtual (devirtualized) | 1.04× |
| Virtual (real vtable lookup) | 1.57× |
| Interface call | 1.59× |

Sealing a class doesn't just document intent — it lets the JIT *prove* no override can exist and skip the vtable lookup entirely. We measured it: the sealed case came back within 4% of a genuinely direct call.

We also nailed down a subtlety about calling virtual members from constructors that a lot of write-ups get slightly wrong: a derived class's field *initializers* have already run by the time a base constructor's virtual call reaches them — it's specifically anything the derived constructor's own *body* was going to do that hasn't happened yet.

Full write-up, runnable benchmarks, and the override/new gotcha demonstrated live: [Link to Chapter]
