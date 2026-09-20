---
title: Inside .NET: Demystifying C# Delegates, Events, and Closures
published: false
tags: dotnet, csharp, architecture, performance
---

# Inside .NET — Episode 16: Delegates & Events Under the Hood

Ever wondered what the CLR actually allocates when you write `Action action = () => DoSomething();`?

In this episode of **Inside .NET**, we tear down the illusion of simple "function pointers" and examine the true runtime mechanics:

- `System.MulticastDelegate` heap structure: `_target` vs `_methodPtr`.
- Why `+=` creates a new delegate instead of mutating the existing one.
- How lambdas create hidden `<>c__DisplayClass` heap instances.
- How to write leak-free weak event publishers.

Check out the full canonical guide and runnable code on GitHub:
👉 [Inside .NET Episode 16: Delegates & Events](https://github.com/biplabchsarker/inside-dotnet/tree/main/chapters/015-delegates-events)
