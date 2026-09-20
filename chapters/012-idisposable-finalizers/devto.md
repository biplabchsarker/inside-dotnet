---
title: "Inside .NET: The Massive Cost of C# Finalizers"
published: false
tags: dotnet, csharp, architecture, performance
---

If you've written C# for any length of time, you've used the `using` statement. But what is it actually doing under the hood? And what happens when you *forget* to use it?

In **Inside .NET Chapter 012**, we dive deep into the internals of `IDisposable` and finalizers.

Here is the one-line rule you need to know:
> Adding a finalizer guarantees an object will survive Gen 0, get promoted to Gen 1, and stall the Finalizer Thread — avoid them unless strictly necessary.

In our benchmarks, simply having an empty finalizer made garbage collection **tens of times slower** (47.65×-73.53× across two measured runs) for those objects! 

We also cover:
- The dual-path cleanup pattern (`Dispose` vs Finalization).
- How the **Finalization Queue** and **F-Reachable Queue** work inside the CLR.
- Why the modern best practice is to avoid finalizers entirely and use `SafeHandle`.

Read the full chapter in the open-source book here: [Link to Chapter 012]
