# Executive Summary: Reflection & Expression Trees Under the Hood

## Key Takeaways

1. **PE Metadata Architecture**: Assemblies store class, method, and field definitions in structured ECMA-335 relational tables (`TypeDef`, `MethodDef`, `FieldDef`). `System.Type` navigates these tokens at runtime.
2. **Reflection Inefficiencies**: `MethodInfo.Invoke` cannot be inlined by RyuJIT and incurs up to 145× latency compared to direct calls due to runtime accessibility checks, argument validation, `object[]` array allocation, and primitive boxing.
3. **Expression Tree Representation**: `Expression<TDelegate>` models code as an immutable Abstract Syntax Tree (AST) in memory, allowing frameworks like Entity Framework Core to inspect and translate C# queries into SQL.
4. **JIT Compilation to Native Delegates**: Invoking `.Compile()` on an AST emits CIL opcodes into a `DynamicMethod`, which RyuJIT compiles into native machine code. Subsequent calls execute in ~1.25 ns with zero heap allocations.
5. **Caching Is Mandatory**: The compilation step takes ~0.3 ms. Compiled delegates must be cached (typically in `static readonly` fields of closed generic helper types) to achieve hot-path throughput.
6. **Native AOT Shift**: Under Native AOT, runtime dynamic emission (`Reflection.Emit`, `Expression.Compile`) is unsupported. Modern .NET architectures favor compile-time **Roslyn Source Generators**.
