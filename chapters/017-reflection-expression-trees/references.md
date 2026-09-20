# Academic & Official References: Reflection & Expression Trees

1. **ECMA-335 International Standard**
   - *Common Language Infrastructure (CLI) Partition II: Metadata Definition and Semantics*.
   - Specifies physical metadata formats, streams (`#Strings`, `#Blob`, `#GUID`, `#~`), and token tables (`TypeDef`, `MethodDef`, `FieldDef`).
   - https://www.ecma-international.org/publications-and-standards/standards/ecma-335/

2. **Microsoft .NET Runtime (dotnet/runtime)**
   - Source code: `System.Reflection.Emit.DynamicMethod` and `Expression.Compiler.LambdaCompiler`.
   - https://github.com/dotnet/runtime/tree/main/src/libraries/System.Linq.Expressions

3. **Entity Framework Core Query Pipeline**
   - *How EF Core Translates Expression Trees to SQL*.
   - https://github.com/dotnet/efcore/tree/main/src/EFCore/Query

4. **Pro C# 10 with .NET 6** (Andrew Troelsen, Phil Japikse)
   - Chapter on Reflection, Dynamic Types, and Late Binding Mechanics.

5. **BenchmarkDotNet Execution**
   - Measured benchmarks for `MethodInfo.Invoke`, `Delegate.CreateDelegate`, and `Expression.Compile()` on .NET 10.0 (x64 RyuJIT).
