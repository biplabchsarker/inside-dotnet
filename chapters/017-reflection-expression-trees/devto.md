---
title: "Inside .NET: Reflection & Expression Trees Under the Hood"
published: true
description: "Why MethodInfo.Invoke is 140x slower than direct calls, and how compiled Expression Trees achieve near-direct native speed with zero allocations."
tags: dotnet, csharp, performance, programming
cover_image: https://raw.githubusercontent.com/biplabchsarker/inside-dotnet/main/chapters/017-reflection-expression-trees/images/017-hero.png
---

## Summary

In modern enterprise applications, dynamic data access is essential—whether mapping DTOs, building database queries, or serializing payloads. But calling `MethodInfo.Invoke` on the hot path introduces massive GC pressure and latency spikes.

In this deep dive, we explore:
1. **ECMA-335 Metadata Architecture**: How TypeDef and MethodDef tokens are resolved by the CLR.
2. **The 140x Reflection Penalty**: Measuring parameter array allocations, primitive boxing, and security checks.
3. **Expression Tree ASTs**: Inspecting and rewriting immutable syntax trees.
4. **DynamicMethod & RyuJIT Emission**: Compiling ASTs into raw machine code that executes in 1.25 ns with zero allocations.

Read the full chapter in the repository: [Inside .NET — Chapter 017](https://github.com/biplabchsarker/inside-dotnet)
