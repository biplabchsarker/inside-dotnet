# Chapter Quiz: Reflection & Expression Trees Under the Hood

### Q1: What physical file structure in a .NET assembly holds the list of all defined classes and their metadata flags?
- A) The PE Manifest Header
- B) The `TypeDef` metadata table (0x02)
- C) The RyuJIT code heap
- D) The Assembly Dependency Cache

<details>
<summary>Answer</summary>

**B.** ECMA-335 partition II defines metadata tables; table 0x02 is the `TypeDef` table containing type names, namespaces, visibility attributes, and extends pointers.
</details>

---

### Q2: Why does `MethodInfo.Invoke` allocate memory on the managed heap even when calling a method that returns `void`?
- A) The CLR allocates a hidden stack frame on the LOH.
- B) The arguments must be passed in an `object[]` array, and any value-type arguments must be boxed onto the heap.
- C) It creates an internal thread-local storage object.
- D) RyuJIT re-allocates the method's native instructions.

<details>
<summary>Answer</summary>

**B.** `MethodInfo.Invoke(object? obj, object?[]? parameters)` requires an `object[]` reference array, and any primitive/value types (such as `int`, `double`, or `decimal`) are boxed into heap objects.
</details>

---

### Q3: What is the primary difference between `Func<int, bool>` and `Expression<Func<int, bool>>`?
- A) `Func<int, bool>` executes in user space, while `Expression` runs in kernel space.
- B) `Func<int, bool>` is compiled executable CIL bytecode; `Expression<Func<int, bool>>` is an Abstract Syntax Tree data structure representing the logic.
- C) `Expression` is limited to mathematical operations only.
- D) `Func` cannot accept value types.

<details>
<summary>Answer</summary>

**B.** A delegate is executable code; an Expression Tree is an immutable tree data structure of nodes (`LambdaExpression`, `BinaryExpression`, etc.) that can be inspected, rewritten, or compiled at runtime.
</details>

---

### Q4: How does calling `.Compile()` on an `Expression<TDelegate>` execute dynamic code at native speeds?
- A) It interprets the nodes line-by-line via a virtual machine.
- B) It compiles the AST nodes into raw CIL instructions inside a `DynamicMethod`, which RyuJIT then compiles into native CPU instructions.
- C) It calls the C# compiler (csc.exe) as an external process.
- D) It invokes `MethodInfo.Invoke` repeatedly behind the scenes.

<details>
<summary>Answer</summary>

**B.** The Expression Tree compiler uses `System.Reflection.Emit.DynamicMethod` to generate CIL opcodes on the fly, which RyuJIT compiles directly to native assembly.
</details>

---

### Q5: How does the invocation latency of a compiled expression delegate compare to a direct C# method call?
- A) Orders of magnitude slower
- B) Close, but measurably slower — both far faster than reflection
- C) 10× faster because of optimization
- D) Milliseconds per invocation

<details>
<summary>Answer</summary>

**B.** A compiled delegate executes native JIT machine code accessed via a direct pointer — a few nanoseconds, and far closer to a direct call than to reflection (tens of nanoseconds), though not perfectly identical to it. See this chapter's Performance Notes for this session's actual measured numbers.
</details>

---

### Q6: What happens if you call `myExpression.Compile()` inside a high-frequency loop of 1,000,000 iterations?
- A) The CLR automatically caches the result on the second iteration.
- B) Performance collapses because compiling an Expression Tree is a real, non-trivial cost paid again on every single iteration.
- C) The loop is unrolled automatically by RyuJIT.
- D) An `InvalidOperationException` is thrown.

<details>
<summary>Answer</summary>

**B.** `.Compile()` is an expensive one-time factory operation. Calling it inside a loop results in catastrophic latency and memory allocation. Always cache compiled delegates.
</details>

---

### Q7: How does `Delegate.CreateDelegate` compare to compiling an Expression Tree for known method signatures?
- A) It is slower than reflection.
- B) It creates an open or closed delegate directly from a `MethodInfo` without AST construction or CIL emission, running at native call speed with negligible setup time.
- C) It is only supported for private methods.
- D) It requires a Roslyn compiler reference.

<details>
<summary>Answer</summary>

**B.** `Delegate.CreateDelegate` directly binds a method pointer to a delegate signature, bypassing AST overhead entirely.
</details>

---

### Q8: What role does `ExpressionVisitor` play in LINQ providers like Entity Framework Core?
- A) It renders SQL diagrams in the IDE.
- B) It traverses the AST nodes recursively, translating member expressions, binary operations, and constants into target-specific syntax like SQL.
- C) It checks database user permissions.
- D) It compiles C# code to WebAssembly.

<details>
<summary>Answer</summary>

**B.** `ExpressionVisitor` uses the Visitor pattern to inspect and translate each expression node into relational SQL statements.
</details>

---

### Q9: Why does `PublishAot=true` (Native AOT) pose challenges for `Expression.Compile()`?
- A) Native AOT forbids LINQ queries.
- B) Native AOT strips the JIT from the runtime; because `Expression.Compile()` generates new CIL opcodes at runtime that require JIT compilation, it is unsupported or restricted.
- C) Native AOT only supports C.
- D) Native AOT requires all expressions to be marked with `[AotSafe]`.

<details>
<summary>Answer</summary>

**B.** Native AOT compiles everything ahead-of-time. Runtime dynamic code generation (`Reflection.Emit`, `DynamicMethod`) cannot compile without a runtime JIT.
</details>

---

### Q10: What modern C# feature is recommended to replace runtime reflection and dynamic expression compilation for Native AOT apps?
- A) C# Dynamic Keyword (`dynamic`)
- B) Roslyn C# Source Generators
- C) Unsafe pointer arithmetic
- D) COM Interop

<details>
<summary>Answer</summary>

**B.** Roslyn Source Generators run at compile-time during build, generating typed C# code for serialization, mapping, and dependency injection with zero runtime overhead and full Native AOT compatibility.
</details>
