# Self-Check Quiz — Value Types vs Reference Types

1. When you assign one variable of a reference type to another (`b = a;`), what gets copied?
2. Is it accurate to say value types always live on the stack? If not, give one counter-example.
3. Why does `foreach (var p in points) { p.X = 5; }` compile without error but fail to actually change anything, when `points` is a `List<StructPoint>`?
4. What's the difference in storage/copy semantics between `record class` and `record struct`, given that both generate value-based equality?
5. What does a `struct` get for `Equals`/`GetHashCode` if you don't override them, and why might you override them anyway even though the default is already correct?

<details>
<summary>Answers</summary>

1. Only the reference itself (a pointer-sized value) — both variables end up pointing at the same underlying object; none of the object's data is duplicated.
2. No. A value type lives wherever its containing storage lives — e.g. a value type that's a field of a class lives inside that (usually heap-allocated) object, and a value type that's an array element lives inside the array's heap-allocated storage. Only local variables typically place a value type on the stack.
3. `p` is the `foreach` iteration variable — a fresh copy of each struct element assigned at the start of every loop iteration. Setting `p.X = 5` mutates that copy, which is discarded when the next iteration begins; the original elements inside `points` are never touched.
4. Both generate the same member-wise `Equals`/`GetHashCode`/`==`/`ToString()`/`with` support. The difference is the underlying type category: `record class` is a reference type (heap-allocated, copied by reference on assignment), while `record struct` is a value type (copied by value, stored per normal struct placement rules).
5. It falls back to `ValueType.Equals`, which uses reflection to compare fields one by one — correct, but measurably slower than a hand-written comparison. You'd override them anyway for performance in equality-heavy code paths, such as using the struct as a dictionary key or in a `HashSet<T>`.

</details>
