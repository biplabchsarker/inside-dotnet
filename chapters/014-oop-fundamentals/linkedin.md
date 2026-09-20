# LinkedIn Post

Does `virtual` actually cost anything in C#? We measured it. 📊

A direct method call: 13.11 ms (50M calls).
The exact same call, made virtual: 20.55 ms. **1.57× slower.**
Through an interface instead: 20.82 ms. **1.59× slower.**

Now seal the class so the JIT can *prove* no override could exist: **13.66 ms — within 4% of the direct call.** The JIT devirtualized it back to almost nothing.

In **Inside .NET Chapter 014**, we go past "polymorphism is nice" and into exactly *why* it costs what it costs: every object carries a hidden Method Table pointer, and a virtual call follows that pointer — never the declared type of the reference calling it. That's also why `override` always wins through any reference type, while `new` (method hiding) can silently disappear through a base-typed one.

We also found — and verified directly, because folklore gets this wrong constantly — exactly which part of a derived object is and isn't safe to touch from a virtual call made inside a base constructor. (Hint: it's not what most blog posts say.)

Read the full deep-dive for the Method Table internals, the override-vs-new gotcha, and the constructor trap most people describe slightly wrong.

[Link to Chapter]

#dotnet #csharp #oop #softwareengineering #performance
