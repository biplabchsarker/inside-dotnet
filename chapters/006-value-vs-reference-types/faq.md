# FAQ — Value Types vs Reference Types

**Q: Are value types always on the stack and reference types always on the heap?**
A: No. Reference types are always allocated as heap objects (with a rare, invisible-to-you JIT stack-allocation optimization in a few narrow escape-analysis cases). Value types are stored wherever their containing context is — a local variable's stack frame, a class field inside that heap object, an array element inside the array's heap block. Only boxing gives a value type its own independent heap allocation.

**Q: If I pass a struct to a method, is it always copied?**
A: By default, yes — pass-by-value copies the struct's fields into the parameter. Passing with `in`, `ref`, or `out` avoids the copy by passing a reference to the original storage instead, with `in` additionally enforcing (at compile time) that the callee can't mutate through it.

**Q: What's the actual difference between `record class` and `record struct`?**
A: Both get compiler-generated member-wise equality (`Equals`, `GetHashCode`, `==`), `ToString()`, and `with`-expression support. The difference is everything a plain `class` vs `struct` difference implies: `record class` is a reference type (heap-allocated, copied by reference), `record struct` is a value type (copied by value, stored per normal struct placement rules).

**Q: Why would I ever override `Equals`/`GetHashCode` on a struct if it already gets member-wise equality for free?**
A: The "free" default (`ValueType.Equals`) uses reflection to walk fields at run time, which is meaningfully slower than a hand-written or compiler-generated (`record struct`) comparison. The behavior is identical either way for correctness — the override exists purely for performance in equality-heavy code paths (dictionary keys, `Distinct()`, `HashSet<T>`).

**Q: Why can't a `Span<T>` be stored as a field in a normal class or captured in a lambda?**
A: Because `Span<T>` is a `ref struct`, which the runtime forbids from ever reaching the heap — it can wrap a reference to stack memory, and if an instance escaped into a heap object or a captured closure, it could outlive the stack frame it points into, producing a dangling reference the GC has no way to detect or protect against.
