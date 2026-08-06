---
title: Inside .NET, Episode 3 — What a Method Table Actually Is
published: false
tags: dotnet, csharp, clr, softwarearchitecture
series: Inside .NET
---

Every managed object you've ever created has a pointer in it you never wrote — sitting right in the object header, pointing at a **method table**. That one pointer is the reason virtual dispatch, polymorphism, and cross-language interop in .NET all work the way they do. Let's look at it directly.

## The 30-second version

```csharp
var order1 = new OrderClass { OrderId = 1 };
var order2 = new OrderClass { OrderId = 2 };

RuntimeTypeHandle h1 = order1.GetType().TypeHandle;
RuntimeTypeHandle h2 = order2.GetType().TypeHandle;

Console.WriteLine(h1.Value == h2.Value); // True
```

Both instances point at the exact same method table. Not a copy — the same one. A million `OrderClass` instances share one table; each instance on the heap is just field data plus a header carrying a pointer to it. That's why polymorphism doesn't cost memory per instance.

## Why this exists

Three problems collapse into "why does .NET need a runtime like the CLR at all":

- **Cross-language interop** — C#, F#, and VB.NET all compile down to the same **CTS (Common Type System)**. A C# `class` and an F# reference type are literally the same construct in metadata once compiled — no language tag, no adapter layer needed to call between them.
- **Type/memory safety without manual bookkeeping** — a raw native binary has no concept of "this pointer is a live `Order` object." The CLR tracks exact types and live references, which is what makes GC, safe casts, and bounds-checked arrays possible at all.
- **Fast, correct dynamic dispatch** — deciding which override runs, on every polymorphic call, has to be both correct and cheap. The method table / vtable design is the answer.

## What actually happens on a virtual call

```csharp
Base b = new Derived();
Console.WriteLine(b.Describe()); // "Derived.Describe (vtable slot overridden)"
```

`b`'s *declared* type is `Base`. Its *runtime* type is `Derived`. When `callvirt` fires:

1. Follow the object's method-table pointer (the runtime type's table, not the declared type's).
2. Index into that table's fixed vtable slot for `Describe`.
3. Jump to whatever compiled address is sitting there.

That's decided at run time, every single call. It's small overhead, but it's real, and — more importantly — it blocks inlining, which is frequently the bigger cost in a hot loop.

A non-virtual call (`static`, `sealed`, or otherwise unoverridable) skips all of it: the target address is fixed at JIT time, and the JIT is free to inline it away entirely.

```csharp
const int iterations = 200_000_000;
IShape[] shapes = { new Circle(), new Square(), new Circle(), new Square() };

var swVirtual = Stopwatch.StartNew();
double totalVirtual = 0;
for (int i = 0; i < iterations; i++)
    totalVirtual += shapes[i & 3].Area(); // virtual dispatch every call
swVirtual.Stop();

var swDirect = Stopwatch.StartNew();
double totalDirect = 0;
var square = new Square();
for (int i = 0; i < iterations; i++)
    totalDirect += square.DirectArea(); // sealed, non-virtual, inlinable
swDirect.Stop();
```

Run the [full sample](code/Chapter02.Demo/Program.cs) yourself with `dotnet run` — the timing gap is real, and it's exactly why "seal what isn't a designed extension point" is a legitimate, low-risk perf habit, not premature optimization.

Modern RyuJIT also does **guarded devirtualization**: if a virtual call site is observed to almost always hit one concrete type, the JIT emits a fast direct path with a type-check guard and a slow-path fallback. You get most of the inlining win without giving up polymorphism — automatically, no code change required.

## AppDomains are gone. AssemblyLoadContext is what you actually have now

.NET Framework isolated code with **AppDomains** — multiple per process, each independently unloadable. .NET Core dropped that entirely: one non-unloadable AppDomain per process, always. `AppDomain.CreateDomain` still compiles for source compatibility, but it buys you nothing.

The real mechanism today is **`AssemblyLoadContext` (ALC)**:

```csharp
AssemblyLoadContext defaultAlc = AssemblyLoadContext.Default;
Console.WriteLine(defaultAlc.Name);          // "Default"
Console.WriteLine(defaultAlc.IsCollectible); // False
```

A *collectible* ALC can load isolated (even differently-versioned) copies of an assembly and later unload them. This is literally how MSBuild task isolation and every real .NET plugin system works today — not process-per-plugin, not AppDomains.

## Where this actually matters at the architecture level

If your codebase wraps every class in an interface "for testability," you're paying the vtable-indirection cost pervasively for a payoff you may not need — reserve interfaces for real seams: testing boundaries, things that vary by deployment, plugin contracts. And if you're designing a plugin or extension system, that's mechanically an `AssemblyLoadContext` design problem: how many contexts, collectible or not, how you handle a dependency loaded at different versions by different plugins at once.

## Try it yourself

Full runnable demo, diagrams, quiz, and interview questions in the [repo](code/Chapter02.Demo/Program.cs) — Episode 3 of **Inside .NET**, a from-scratch series on .NET runtime internals for people who already know the syntax and want to know what's underneath it.

*Next up: Episode 4 goes inside RyuJIT itself — tiered compilation, IL-to-native translation, and how to read the JIT's own diagnostic output.*
