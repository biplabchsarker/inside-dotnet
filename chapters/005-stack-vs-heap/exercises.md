# Exercises — Stack vs Heap: Where Your Data Actually Lives

1. **Prove the container rule, not the type rule** — Starting from [`code/Chapter05.Demo/Program.cs`](code/Chapter05.Demo/Program.cs), add a third measurement: allocate 1,000,000 `Point3D` values as elements of a `Point3D[]` array (a value-type array, allocated once) versus 1,000,000 `Point3D` values as elements of a `List<object>` (each one boxed on `Add`). Print the heap delta for both using the same `GC.GetTotalMemory(forceFullCollection: true)` before/after pattern already in the sample. Confirm: the array case shows one allocation for the whole array (the structs are inline inside it), while the boxed-list case shows growth proportional to the element count. Explain in a sentence why the array case is the exception that swallows a million structs into a single heap allocation.

2. **Feel the copy cost of a large struct** — Define a `readonly struct BigVector4x4` containing 16 `double` fields (128 bytes) with a constructor and an `Add` method that returns a new `BigVector4x4`. Write a loop that calls `Add` 10,000,000 times passing the struct **by value**, and a second version where the method takes the parameter `in BigVector4x4` instead. Time both with `Stopwatch`. Confirm the `in`-parameter version is measurably faster, and explain why in terms of what's actually being copied on each call — this is the "Struct size matters for stack pressure too" point from Performance Notes, made concrete.

3. **Stretch — trigger and diagnose a real `StackOverflowException`** — In a **disposable** console project (not the chapter's own demo — this deliberately crashes the process), write the two commented-out lines from `Program.cs` (`RecurseForever`) for real, and run it. Observe that the process terminates immediately with no catchable exception and no graceful message. Then run the same program under `dotnet-dump` or the Visual Studio debugger's "break on unhandled exception," attach *before* the crash, and walk the call stack to see how many frames deep it got before hitting the guard page. Write down what you observe versus what you expected — most people expect a large, specific number; the actual depth depends on frame size, which varies by method, not a fixed constant.

4. **Try to break `Span<T>`'s stack-only rule on purpose** — Write a method that does `Span<byte> buf = stackalloc byte[64];` and then try, one at a time: (a) storing `buf` in a field of a class, (b) returning `buf` from the method, (c) capturing `buf` in a lambda that's stored for later use. Confirm the compiler rejects all three at compile time, then read the actual compiler error for each — they name the `ref struct` escape rule directly. This is the mechanism, not just the claim, behind "you can't hold a stack buffer past the frame it came from."

## Challenge

**Predict the output before running it.**

```csharp
Span<int> Make()
{
    Span<int> local = stackalloc int[4];
    local[0] = 42;
    return local; // does this compile?
}
```

Before running (or even compiling) this: will it compile, or will the compiler reject it? If it's rejected, write down the exact reason in your own words — in terms of stack frame lifetime, not just "the compiler said no." Then try changing `Span<int>` to `int[]` (a heap-allocated array) with the same body pattern and confirm that version compiles fine — and explain why the exact same-looking code is legal for one and illegal for the other.
