# Hands-On Exercises: Delegates & Events

### Exercise 1: Exploring Delegate Reflection
**Goal:** Write a small utility method `void DumpDelegate(Delegate del)` that inspects any given delegate and prints:
1. Target object type (or "Static Method" if null).
2. Method name and parameter types.
3. Number of subscribers in the invocation list.
4. The memory address of `_methodPtr`.

---

### Exercise 2: Closure Allocation Profiling
**Goal:** Prove that capturing local variables causes heap allocation using BenchmarkDotNet or `GC.GetAllocatedBytesForCurrentThread()`.
1. Write a loop that executes 1,000,000 iterations calling a method that accepts `Func<int, int>`.
2. Case A: Pass a static lambda `static x => x + 1`.
3. Case B: Pass a capturing lambda `x => x + localVariable`.
4. Measure the difference in allocated bytes.

---

### Challenge: Resilient Asynchronous Event Dispatcher
**Scenario:** Standard C# events are synchronous: when an event is raised, every subscriber executes on the caller's thread sequentially.
**Task:** Build an `AsyncEventDispatcher<TEventArgs>` that:
1. Supports asynchronous event handlers (`Func<object?, TEventArgs, Task>`).
2. Invokes all subscribers concurrently using `Task.WhenAll` or sequentially.
3. Safely aggregates any exceptions thrown by asynchronous subscribers without leaving unobserved task exceptions.
4. Allows cancellation via a `CancellationToken`.
