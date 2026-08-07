# Inside .NET — Episode 9: Boxing & Unboxing

*Part II — Memory*

A value type and a reference type can't both ride the same conveyor — one has no header, no heap identity, nothing a uniform `object` reference can point at. Boxing is the CLR's way of building that value type a temporary heap identity on demand. This chapter is the full mechanism, plus real numbers on what it actually costs — including a case where it costs nothing at all.

## Boxing is Episode 8's allocator, one more time

Nothing new gets invented for boxing. The `box` IL instruction sizes the allocation (object header plus the value's own bytes), asks the same fast-path allocator from [Episode 8](../007-object-allocation/article.md) for that space — a per-thread bump-pointer, no lock — writes the header (method table pointer, sync block index), and copies the value's bits in. The only difference from a plain `new SomeClass()` is that the "constructor" here is a single memcpy instead of user code.

## Unboxing is a type check, not a conversion

This is the detail most explanations skip: `unbox` doesn't ask "can this be converted to what I want." It compares the box's method table pointer against the *exact* type requested. `int` widens to `long` everywhere else in C# — but `(long)someBoxedInt` throws `InvalidCastException`, every time, because there's no conversion step being consulted at all. A mismatch is a mismatch, full stop.

## Where boxing hides without saying "box"

Three shapes are worth recognizing on sight:

- Storing a value type in `ArrayList` or `Hashtable` — these predate generics and only know how to hold `object`.
- Passing a value type to an `object` parameter, including a `params object[]` overload (`string.Format`, `Console.WriteLine("{0}", x)`).
- Assigning a struct to a variable typed as an interface it implements.

That third one has a nuance worth knowing precisely: the box happens **once**, at the assignment — every call through that interface variable afterward dispatches against the same box, it doesn't re-box per call.

## The escape-analysis surprise

Here's where this chapter earns its keep instead of repeating received wisdom. I benchmarked three ways of calling the same method on the same struct 100,000 times: directly, through a generic constraint, and through a boxed interface reference.

| Method | Mean | Allocated |
|---|---|---|
| Direct call | 21.57 µs | – |
| Generic constraint | 21.62 µs | – |
| **Interface reference (boxed)** | 21.13 µs | **–** |

Zero. The boxed version allocates nothing, measurably indistinguishable from the two boxing-free alternatives. Modern RyuJIT proved the box never escapes the method — it's never stored, returned, or placed in a collection — and eliminated the allocation entirely.

Compare that to `ArrayList` vs `List<int>` in the same benchmark run, where the box genuinely does escape (it gets stored in the list's backing array): 4.4x slower, 8x the memory. Same language feature, opposite outcome, because escape is the variable that actually matters — not "boxing" as an undifferentiated concept.

## The mutation gotcha, made concrete

```csharp
var original = new Counter(0);
IIncrementable boxed = original; // boxes 'original' right here

boxed.Increment(); // mutates the BOX, not 'original'

Console.WriteLine(original.Value);           // 0
Console.WriteLine(((Counter)boxed).Value);   // 2
```

The moment a struct is boxed, it and the variable it came from are two independent copies. `boxed.Increment()` mutates the box's own state — `original` never sees it. This is the same "structs copy" rule from Episode 7, just easy to lose track of when the copy is sitting behind a reference.

## Try it yourself

The [companion demo](code/Chapter08.Demo/Program.cs) walks through boxing identity, the exact-type unboxing check, the `ArrayList`/`List<int>` allocation gap, and this mutation gotcha with real printed output. The [benchmarks project](code/Chapter08.Benchmarks/Program.cs) is the source of every number in this article — run it yourself with `dotnet run -c Release`.

*Next: [Episode 10 — Strings & Interning](../009-strings-interning/article.md).*
