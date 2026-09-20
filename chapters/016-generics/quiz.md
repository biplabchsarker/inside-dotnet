# Quiz: Generics Under the Hood

### Question 1
What does "reified generics" in .NET mean?
- A) Generic types are converted to `System.Object` by the C# compiler.
- B) Generics only run on 64-bit systems.
- C) Type parameters are fully preserved at runtime in metadata and executable machine code.
- D) Generics require runtime dynamic reflection to execute methods.

<details>
<summary>Answer</summary>

**C)** Reification means type arguments remain first-class runtime citizens in metadata and JIT compilation, allowing full runtime type inspection and native code specialization.
</details>

---

### Question 2
Why do `List<string>` and `List<object>` share the same native JIT machine code?
- A) The C# compiler merges them into dynamic objects.
- B) Both types are reference types represented by identical 8-byte pointers, allowing the CLR to share a single canonical implementation (`__Canon`) to prevent code bloat.
- C) Strings inherit directly from object without any custom methods.
- D) The JIT only compiles generics once per assembly.

<details>
<summary>Answer</summary>

**B)** Because all reference types have identical pointer representation on a given architecture, RyuJIT shares a single native code implementation (`__Canon`) across all reference types.
</details>

---

### Question 3
What happens to static fields in a generic class `MyClass<T>`?
- A) One shared static field exists for all instantiations of `MyClass<T>`.
- B) Each closed type (e.g. `MyClass<int>` vs `MyClass<string>`) has its own independent static storage slot and its own static constructor.
- C) Static fields are disallowed in generic classes.
- D) Static fields are only created for value types.

<details>
<summary>Answer</summary>

**B)** Closed generic types are distinct runtime types with their own independent static memory allocations.
</details>

---

### Question 4
How does the constraint `where T : struct, IResettable` prevent boxing when calling `item.Reset()`?
- A) It moves the struct to the Large Object Heap.
- B) The compiler emits the `constrained.` prefix, allowing the JIT to devirtualize and emit a direct call to the struct's method without allocating a heap box.
- C) It caches the struct in a thread-local static field.
- D) It casts the struct to `Span<T>`.

<details>
<summary>Answer</summary>

**B)** The `constrained.` CIL prefix signals to the JIT that the target is a concrete value type, allowing it to bypass virtual dispatch and invoke the struct's method directly in-place.
</details>
