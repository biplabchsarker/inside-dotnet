---
title: "Inside .NET: Why Your C# App is Leaking Memory"
published: false
tags: dotnet, csharp, architecture, performance
---

It's a common myth that you can't have memory leaks in C# because of the Garbage Collector. 

The truth is, the GC only collects **unreachable** objects, not **unused** objects. If a static field, long-lived singleton, or background thread can trace a path to your object, it will never be collected.

In **Inside .NET Chapter 013**, we dive deep into the internals of Managed Memory Leaks.

In our benchmarks, simply forgetting to type `-=` (unsubscribe) on an event handler caused nearly a gigabyte of short-lived objects to be promoted directly into Generation 2, resulting in a massive permanent memory leak!

We cover:
- The structure of `MulticastDelegate` and why it holds strong references.
- How to trace a GC Root path.
- The Weak Event Pattern (`WeakReference<T>`) for production systems.

Read the full chapter in the open-source book here: [Link to Chapter 013]
