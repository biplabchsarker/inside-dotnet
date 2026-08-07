---
title: Inside .NET — Episode 9: Boxing & Unboxing
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 8 — Object Allocation](../007-object-allocation/article.md).*

```csharp
var original = new Counter(0);
IIncrementable boxed = original; // boxes 'original' right here

boxed.Increment(); // mutates the BOX, not 'original'

Console.WriteLine(original.Value);           // 0
Console.WriteLine(((Counter)boxed).Value);   // 2
```

If that surprises you, this chapter is the mechanism behind why — plus real `BenchmarkDotNet` numbers on what boxing actually costs, including a case where the answer is "nothing at all."

## Boxing rides Episode 8's allocator, unmodified

The `box` IL instruction sizes the allocation (header + the value's own bytes), asks the exact same fast-path allocator from [Episode 8](../007-object-allocation/article.md) for that space — a per-thread bump-pointer, no lock — writes the header (method table pointer, sync block index), then copies the value's bits in. That's the entire mechanism; only the "constructor" (a memcpy instead of user code) differs from a plain `new`.

## Unboxing checks the exact type — it doesn't convert

`unbox` compares the box's method table pointer against the exact type requested. `int` implicitly widens to `long` everywhere else in C#, but `(long)someBoxedInt` throws `InvalidCastException` unconditionally — there's no conversion step involved, just a type-token comparison that fails.

## Three places boxing hides without saying "box"

- `ArrayList`/`Hashtable` — pre-generics collections that only know how to hold `object`.
- `object`/`params object[]` parameters (`string.Format`, `Console.WriteLine("{0}", x)`).
- Assigning a struct to an interface-typed variable — this boxes **once**, at the assignment, not once per call afterward.

## The measured surprise: sometimes it's free

Three ways of calling the same method on the same struct, 100,000 times each, real `BenchmarkDotNet` numbers:

| Method | Mean | Allocated |
|---|---|---|
| Direct call | 21.57 µs | – |
| Generic constraint | 21.62 µs | – |
| Interface reference (boxed) | 21.13 µs | **–** |

The boxed version allocates **zero bytes** — modern RyuJIT's escape analysis proved the box never leaves the method (never stored, returned, or collected) and removed the allocation entirely. Put that same box in an `ArrayList` instead, and it's a real allocation: 4.4x slower, 8x the memory of `List<int>`, measured in the same run. Escape is the variable that decides the cost, not "boxing" as a blanket concept.

## Try it yourself

```bash
cd chapters/008-boxing-unboxing/code/Chapter08.Demo
dotnet run
```

The [demo](code/Chapter08.Demo/Program.cs) proves boxing identity, the exact-type unboxing check, and the mutation gotcha above with real output. The [benchmarks project](code/Chapter08.Benchmarks/Program.cs) backs every number here — `dotnet run -c Release` reproduces them on your own machine.

*Next: [Episode 10 — Strings & Interning](../009-strings-interning/article.md).*
