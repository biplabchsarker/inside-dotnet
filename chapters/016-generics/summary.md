# Summary: Generics Under the Hood

### The 30-Second Summary

- **Reification over Erasure**: Unlike Java, where generics are erased to `Object` at compile-time, .NET generics are fully preserved in ECMA-335 metadata at runtime. `typeof(T)` works, `new T()` works, and value types avoid boxing.
- **Value Type Specialization**: For every closed value type (`int`, `double`, `DateTime`, or custom struct), RyuJIT generates a dedicated, specialized machine code implementation. Elements are stored packed in contiguous memory with 0 bytes of boxing overhead.
- **Canonical Code Sharing (`__Canon`)**: Because all reference types are represented by an 8-byte pointer on x64, RyuJIT compiles a single, shared canonical implementation (`List<__Canon>`) for all class types, preventing binary bloat while maintaining strict type safety.
- **Static Field Partitioning**: Each closed generic type (`GenericHolder<int>` vs `GenericHolder<string>`) receives its own isolated static memory partition on the High-Frequency Heap and runs its own static constructor.
- **Constrained Devirtualization**: Calling an interface method on a struct through a generic constraint (`where T : struct, IInterface`) allows the JIT to emit direct, inlined calls with zero boxing.
