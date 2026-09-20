# Frequently Asked Questions: Generics Under the Hood

### 1. Does having 50 different `List<T>` types cause code bloat in .NET?
Only if they are value types with different memory layouts! If you have `List<string>`, `List<Customer>`, `List<Order>`, and 47 other reference types, RyuJIT compiles **only one** native code body (`List<__Canon>`). They share the same code segment. Only value types (`List<int>`, `List<double>`) produce separate assembly bodies because their physical machine layouts differ.

### 2. Why can't I create a new instance of a generic type (`new T()`) without a constraint?
At the CIL level, the compiler must emit a constructor call (`newobj`). If `T` is unconstrained, the compiler has no guarantee that `T` possesses a public parameterless constructor (for example, `string` does not have a public parameterless constructor). Specifying `where T : new()` tells the compiler to verify and emit the appropriate instantiation code.

### 3. What is the performance cost of `typeof(T)` inside a generic method?
In modern .NET with RyuJIT, `typeof(T)` for a closed generic type is recognized as an intrinsic and evaluated at JIT-compile time. It compiles down to a direct pointer read from the `MethodTable` handle, executing in approximately 0.3 nanoseconds with zero allocations.

### 4. How does `INumber<T>` in modern .NET work without virtual dispatch overhead?
C# 11 introduced static virtual members in interfaces. When you write `T Add<T>(T a, T b) where T : INumber<T> => a + b;`, the JIT directly inlines the specialized addition instruction (e.g. CPU `add` instruction for integers) based on `T`'s static interface implementation.
