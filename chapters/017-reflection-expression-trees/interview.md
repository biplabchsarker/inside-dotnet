# Deep Interview Preparation: Reflection & Expression Trees

### 1. "Can you walk me through the lifecycle of an ECMA-335 metadata token from C# source code to `System.Type`?"
**Key Concepts to Hit:**
- The Roslyn compiler writes type definitions into PE headers with tokens structured as a 1-byte table index followed by a 3-byte RID (Row Identifier). E.g., `0x02000010` is table `0x02` (`TypeDef`), row 16.
- The OS PE loader maps the binary into memory; the CLR Class Loader parses the metadata streams (`#~`, `#Strings`, `#Blob`, `#GUID`).
- The CLR creates an unmanaged internal `MethodTable` structure in native process memory and assigns it a `TypeHandle`.
- In managed code, `typeof(T)` or `instance.GetType()` returns a managed `RuntimeType` instance that holds a native pointer to this `MethodTable`.

---

### 2. "Why is `MethodInfo.Invoke` orders of magnitude slower than a regular C# method call?"
**Key Concepts to Hit:**
- **Lack of JIT Inlining**: The JIT cannot inline a late-bound reflection call because the target address is resolved at runtime.
- **Security and Accessibility Checks**: On every call, the CLR checks caller permissions and visibility flags (`public`, `internal`, `private`).
- **Signature & Parameter Validation**: Verifies that the number and types of arguments in `object[]` match the method signature.
- **Boxing & Heap Allocation**: Any value-type parameters must be boxed into individual heap objects, and an `object[]` array must be allocated to hold them.
- **Indirect Dispatch Stub**: Execution is routed through an internal C++ stub (`InvokeMethodFast`) that marshals parameters into the architecture-specific calling convention stack frame.

---

### 3. "How does `Expression<Func<T, bool>>` differ from `Func<T, bool>`, and how does the CLR compile it into machine code?"
**Key Concepts to Hit:**
- `Func<T, bool>` is a compiled delegate holding an unmanaged code pointer (`_methodPtr`) to executable native machine instructions.
- `Expression<Func<T, bool>>` is an Abstract Syntax Tree (AST) composed of interconnected node objects (`LambdaExpression`, `BinaryExpression`, `ParameterExpression`, `ConstantExpression`).
- Calling `.Compile()` instantiates `Expression.Compiler.LambdaCompiler`, which uses `System.Reflection.Emit.DynamicMethod` to generate CIL opcodes on the Dynamic Code Heap.
- RyuJIT compiles those opcodes into native machine instructions and returns a delegate pointing directly to that code.

---

### 4. "How does Entity Framework Core use Expression Trees to generate SQL?"
**Key Concepts to Hit:**
- In LINQ to Entities (`IQueryable<T>`), queries are captured as Expression Trees rather than delegates.
- EF Core passes the tree through a series of `ExpressionVisitor` implementations.
- Nodes are translated into an internal Relational Model:
  - `MemberExpression` (`p.Price`) -> Column expression (`[p].[Price]`)
  - `BinaryExpression.GreaterThan` -> SQL `>` operator
  - `ConstantExpression` -> SQL parameter (`@__price_0`)
- The SQL generator builds a text query and parameters, sent to the database via ADO.NET.

---

### 5. "How does Native AOT affect reflection and dynamic code generation?"
**Key Concepts to Hit:**
- Native AOT compiles C# directly to machine code ahead-of-time and strips the JIT compiler.
- APIs that rely on runtime JIT code generation (`Reflection.Emit`, `Expression.Compile()`, `Assembly.Load(byte[])`) fail or throw runtime exceptions.
- Metadata is aggressively trimmed; types not referenced statically may have their metadata stripped unless preserved via `[DynamicDependency]` or trimming descriptors.
- Solution: Transition from runtime reflection/compilation to **Roslyn Source Generators**, which perform metaprogramming and code generation at build time.
