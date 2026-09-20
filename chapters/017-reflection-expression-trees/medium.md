# Inside .NET: Reflection & Expression Trees Under the Hood

### Why Reflection is Slow and How Expression Trees Achieve Zero-Overhead Execution

*By Biplab Sarker*

Every .NET developer has used reflection to inspect object properties or dynamically invoke methods. But what really happens behind `MethodInfo.Invoke`? Why does it take 50 nanoseconds when a direct call takes less than half a nanosecond?

And how do frameworks like **Entity Framework Core**, **Dapper**, and **AutoMapper** dynamically manipulate types without grinding your web servers to a halt?

In this episode of **Inside .NET**, we tear down the Common Language Runtime (CLR) metaprogramming pipeline:
- The structure of PE/COFF metadata tables (`TypeDef`, `MethodDef`).
- The 4 hidden costs of runtime late binding (security checks, argument arrays, boxing, and dispatch stubs).
- The architecture of Expression Tree Abstract Syntax Trees (ASTs).
- How RyuJIT compiles dynamic CIL opcodes into hardware-native assembly instructions.

Read the full chapter with interactive diagrams and benchmark results on GitHub:
👉 [Inside .NET Repository](https://github.com/biplabchsarker/inside-dotnet)
