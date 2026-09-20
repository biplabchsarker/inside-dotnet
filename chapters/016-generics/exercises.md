# Hands-On Exercises: Generics Under the Hood

### Exercise 1: Runtime Native Pointer Proof
**Goal:** Verify native code address sharing between reference types and specialization between value types.
1. Create a generic class `Inspect<T>` with a `[MethodImpl(MethodImplOptions.NoInlining)] void Test() { }`.
2. Prepare methods using `RuntimeHelpers.PrepareMethod`.
3. Read the native function pointer via `MethodHandle.GetFunctionPointer()`.
4. Assert that `Inspect<string>` and `Inspect<object>` share the exact same address, while `Inspect<int>` and `Inspect<long>` differ.

---

### Exercise 2: Benchmarking Boxing vs Constrained Generic Structs
**Goal:** Measure the allocation overhead of interface dispatch on structs.
1. Define an interface `IValue` with an `int Get()` method.
2. Implement it on a struct `Counter`.
3. Create two methods:
   - Method A: `int ExecuteNonGeneric(IValue val) => val.Get();`
   - Method B: `int ExecuteGeneric<T>(T val) where T : struct, IValue => val.Get();`
4. Use BenchmarkDotNet to measure memory allocations and execution time.

---

### Challenge: Building a Zero-Allocation Object Pool
**Scenario:** High-performance web servers process millions of requests per second and cannot afford Gen 0 allocations for transient buffers.
**Task:** Build an ultra-fast `GenericStructPool<T>`:
1. Enforce `where T : struct, IClearable`.
2. Support renting and returning via a `ref struct Leased<T>` that implements `IDisposable`.
3. Verify that 1,000,000 rent/return operations produce exactly **0 bytes allocated**!
