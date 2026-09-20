# Hands-on Exercises: Reflection & Expression Trees

## Exercise 1: Fast Dynamic Property Setter

**Objective**: Traditional `PropertyInfo.SetValue(instance, value)` boxes value types and takes ~45 ns per call. Build a generic factory `Action<TTarget, TProperty> CreateSetter<TTarget, TProperty>(string propertyName)` using Expression Trees.

**Requirements**:
1. Accept the property name as a string.
2. Construct two `ParameterExpression` nodes: one for `TTarget` and one for `TProperty`.
3. Create a `BinaryExpression` using `Expression.Assign(Expression.Property(target, propertyName), value)`.
4. Return the compiled delegate `Action<TTarget, TProperty>`.
5. Verify in a test harness that the setter correctly modifies the target object and generates 0 bytes of heap allocation on hot-path calls.

---

## Exercise 2: LINQ Expression Tree Filter Combiner

**Objective**: In enterprise applications, users frequently construct dynamic search criteria. Write an extension method `Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)` that merges two predicates with a logical `AND`.

**Gotcha to Solve**: If you simply call `Expression.AndAlso(left.Body, right.Body)`, the parameter in `right.Body` points to a distinct `ParameterExpression` reference than `left.Body`, throwing an invalid parameter exception upon invocation.

**Requirements**:
1. Create a custom `ExpressionVisitor` that replaces references to `right.Parameters[0]` with `left.Parameters[0]`.
2. Merge the rewritten body with `left.Body` using `Expression.AndAlso`.
3. Wrap the resulting expression in a `LambdaExpression` using `left.Parameters`.
4. Test with an in-memory collection of orders or products.

---

## Exercise 3: Zero-Allocation Struct Property Reader

**Objective**: When dealing with structs, passing by value causes copies, while passing to non-generic reflection causes boxing. Write a factory that takes a struct by reference (`ref TStruct`) and retrieves a property value with zero allocations using `Expression.Lambda`.
