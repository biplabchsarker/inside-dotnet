---
title: Inside .NET — Episode 7: Value Types vs Reference Types
published: false
tags: dotnet, csharp, memorymanagement, softwarearchitecture
series: Inside .NET
canonical_url:
---

*Part II — Memory. Previous: [Episode 6 — Stack vs Heap](../005-stack-vs-heap/article.md).*

```csharp
foreach (var p in points) { p.X = 5; }
```

Compiles cleanly. Changes nothing. If you've never hit this, you will — and if you have, this chapter is the "why," precisely.

## The one-line rule

- **Value types** (`struct`, `enum`, `int`, `bool`, `double`, `DateTime`) copy their field data on assignment and on pass-by-value.
- **Reference types** (`class`, `interface`, `delegate`, array, `string`) copy the reference — both variables end up pointing at the same object.

That's it. Everything else in this chapter is a consequence of that one distinction.

## Why `foreach` loop didn't work

`p` in `foreach (var p in points)` is a fresh **copy** of each element, assigned at the start of every iteration. `p.X = 5` mutates that copy. The loop discards it at the end of the iteration. The actual elements in `points` were never touched — not corrupted, not partially updated, just completely unaffected.

The compiler *does* catch the most naive version of this: iteration variables are implicitly read-only, so the line above actually fails with `CS1654` if `p` is declared `MutablePoint p` in certain contexts. But there are enough access paths that return a copy silently — a property getter, a method return — that the mistake routinely survives to production in a different shape:

```csharp
myControl.Location.X = 10;  // compiles in some contexts, mutates a throwaway copy
```

The fix, in both cases: make the struct `readonly struct`, so any attempted mutation through a copy fails to compile instead of failing silently at runtime.

## "Value types live on the stack" — not quite

The precise rule: **a value type lives wherever its containing storage lives.**

- Local variable → stack frame.
- Field of a `class` → inside that heap-allocated object.
- Array element → inside the array's own heap block (arrays are reference types).
- Boxed value → its own fresh heap allocation, created specifically to give the value an object identity.

Only the first case is actually "on the stack." The other three put value-type data on the heap, just not as an independently-addressable object with its own header.

## Boxing is where value types get expensive

Storing an `int` in an `ArrayList`, passing a `struct` where `object` is expected, or invoking a non-overridden interface member on a struct through its interface type — all three box: a heap allocation plus a full-field copy, every single time. `List<int>` exists specifically so this never has to happen for the common case. (Full mechanics — object headers, method tables, the JIT's `unbox`/`unbox.any` — get their own treatment in [Episode 8 — Object Allocation](../007-object-allocation/article.md).)

## `readonly struct` and `ref struct`

`readonly struct` proves to the compiler every field is immutable — which means it can skip a defensive copy it would otherwise insert before letting you call a member through an `in` parameter. `ref struct` — the category `Span<T>` belongs to — is enforced *stack-only* storage: it can never be boxed, fielded in an ordinary class, captured in a lambda, or used as a generic type argument, because it may wrap a reference into stack memory that must never outlive the frame it points into.

## `record class` vs `record struct`

Both generate the same member-wise `Equals`, `GetHashCode`, `==`, and `with`-expression support. The difference is everything the plain `class`/`struct` difference already implies:

```csharp
record class PointRC(int X, int Y);   // reference type — copied by reference
record struct PointRS(int X, int Y);  // value type — copied by value

PointRC rc1 = new(1, 2);
PointRC rc2 = new(1, 2);
rc1 == rc2;                    // True — value equality
ReferenceEquals(rc1, rc2);     // False — different objects
```

People remember the equality part correctly and forget the storage part — which is exactly where "isn't `record` supposed to be immutable and value-like?" confusion comes from. `record` alone means `record class`: a reference type.

## The myth: "structs are always faster"

True only for small, short-lived structs that avoid boxing and repeated copying. A struct with 20 fields, passed by value through 4 layers of method calls, pays a full 20-field `memcpy` at *every* call boundary — that can add up to more total copying than a class's one-time heap allocation plus cheap pointer copies everywhere else. `in` / `ref readonly` exists specifically to pass large `readonly struct` values by reference without giving up immutability.

## Where this actually matters at scale

If your team is building domain value objects (`Money`, `DateRange`, `GeoCoordinate`) at any real scale, this stops being a per-file judgment call. Small, copy-heavy values are `readonly struct`/`record struct` candidates; larger or more complex ones are better as immutable `record class`. Standardizing this as a team convention — and backing it with an analyzer rule that flags mutable public struct fields — is cheaper than relying on every reviewer catching the mutable-struct trap by eye. It's also exactly how the .NET runtime team treats its own BCL types: `Vector2/3/4`, `TimeSpan`, `DateOnly`, `Guid` are all `readonly struct`s, by design guideline, not by accident.

## Try it yourself

The full runnable demo — value-copy vs. reference-sharing, the guarded `foreach` mistake (`CS1654`), `in`-parameter passing, and `record class` vs `record struct` equality — is in [`code/Chapter06.Demo/Program.cs`](code/Chapter06.Demo/Program.cs):

```bash
cd chapters/006-value-vs-reference-types/code/Chapter06.Demo
dotnet run
```

*Next: [Episode 8 — Object Allocation](../007-object-allocation/article.md).*
