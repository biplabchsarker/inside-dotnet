# Frequently Asked Questions: Reflection & Expression Trees

### Q: Is reflection ever acceptable in production?
**A:** Yes, on the **cold path**. Startup dependency injection container registration, plugin discovery, assembly scanning, and configuration binding run once when the application boots. The 10–50 ms total cost is imperceptible to users. Reflection becomes catastrophic only when placed on the **hot path** (e.g., inside HTTP request handlers, deserialization loops, or per-message processing).

### Q: Why not use `dynamic` instead of Expression Trees?
**A:** The C# `dynamic` keyword relies on the Dynamic Language Runtime (DLR) call-site caching mechanism. While faster than naive reflection due to polymorphic inline caching, `dynamic` still introduces boxing overhead for value types, incurs call-site lookup overhead, eliminates compile-time type safety, and complicates diagnostics. Compiled Expression Trees give you strong typing, compile-time safety, zero boxing, and direct delegate execution.

### Q: How should I store compiled expression delegates?
**A:** The most efficient strategy is **static generic caching**:
```csharp
private static class Cache<TSource, TTarget>
{
    public static readonly Func<TSource, TTarget> Mapper = BuildMapper();
}
```
This guarantees zero dictionary lookup overhead, zero thread locking, and single-instruction memory dereference upon invocation.

### Q: Does `Expression.Compile(preferInterpretation: true)` help?
**A:** Yes, in scenarios where an expression will only be executed 1 to 5 times (e.g., in scripting or one-off workflows). Passing `preferInterpretation: true` skips the expensive RyuJIT CIL emission step and executes via an internal light-weight interpreter. However, for hot paths executed thousands of times, standard JIT compilation is far superior.

### Q: Can I use Expression Trees with private properties or fields?
**A:** Yes. Unlike compile-time C# syntax which enforces accessibility, Expression Tree factory methods (`Expression.Property`, `Expression.Field`) accept any `PropertyInfo` or `FieldInfo`, including private members retrieved with `BindingFlags.NonPublic | BindingFlags.Instance`.
