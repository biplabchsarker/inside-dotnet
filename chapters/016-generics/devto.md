---
title: Inside .NET: The Runtime Secrets of C# Generics
published: false
tags: dotnet, csharp, architecture, performance
---

# Inside .NET — Episode 17: Generics Under the Hood

Ever wondered why `List<int>` is lightning fast compared to `ArrayList`, but `List<string>` and `List<object>` don't bloat your binary size?

In this episode of **Inside .NET**, we inspect the runtime internals of generics:

- **Reification vs. Erasure**: Why .NET modified the runtime and Java didn't.
- **`__Canon`**: The hidden CLR canonical type that powers code sharing.
- **Constrained Devirtualization**: Invoking interface methods on structs without boxing.
- **Static Field Partitioning**: Building ultra-fast 0.3 ns type metadata caches.

Read the canonical guide with runnable benchmarks on GitHub:
👉 [Inside .NET Episode 17: Generics Under the Hood](https://github.com/biplabchsarker/inside-dotnet/tree/main/chapters/016-generics)
