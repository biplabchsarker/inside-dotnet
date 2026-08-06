# Summary — Value Types vs Reference Types

- **Value types** (`struct`, `enum`, primitives like `int`/`bool`/`double`) copy their *data* on assignment or when passed to a method.
- **Reference types** (`class`, `interface`, `delegate`, array, `string`) copy the *reference* — both variables end up pointing at the same object.
- **"Value types live on the stack" is imprecise.** They live wherever their containing storage lives: stack for locals, inside the object for class fields, inside the array for array elements, and their own heap block only when boxed.
- **Boxing/unboxing** is what happens when a value type needs to be treated as `object` — a heap allocation plus a full copy each way. Full depth: [Episode 8 — Object Allocation](../007-object-allocation/article.md) and beyond.
- **`readonly struct`** proves immutability to the compiler, enabling defensive-copy elision. **`ref struct`** (e.g. `Span<T>`) is compiler/runtime-enforced stack-only storage — it can never be boxed, fielded in a class, captured in a closure, or used as a generic argument.
- **`record class`** is a reference type with generated value equality; **`record struct`** is a value type with the same generated equality — same equality contract, different copy/storage semantics.
- **Default equality:** value types fall back to reflection-based `ValueType.Equals`; reference types fall back to reference equality (`object.Equals`). Override `Equals`/`GetHashCode` on structs used in hot paths.
- **Mutable structs are a bug magnet** — `foreach`, property getters, and some indexers hand you a *copy*; mutating it silently changes nothing.
- **"Structs are always faster" is a myth.** True only for small, short-lived structs. Large structs copied repeatedly through call layers can cost more than a class's one-time allocation plus pointer copies. `in`/`ref readonly` mitigates the copy cost for large read-only structs.

**Next:** [Episode 8 — Object Allocation](../007-object-allocation/README.md)
