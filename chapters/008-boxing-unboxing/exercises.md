# Exercises — Boxing & Unboxing

1. **Force the escape and watch the allocation reappear.** Starting from `InterfaceDispatchBenchmarks` in [`code/Chapter08.Benchmarks/Program.cs`](code/Chapter08.Benchmarks/Program.cs), add a new benchmark method that stores the boxed `IIncrementable` in an instance field (or a `List<IIncrementable>`) before looping `Increment()` calls, instead of keeping it as a purely local variable. Re-run with `dotnet run -c Release` and confirm the `Allocated` column is no longer `-` — explain, in a sentence, why storing the reference in a field is enough to defeat the escape analysis that eliminated the allocation in `InterfaceReferenceCalls`.

2. **Prove the `Nullable<T>` boxing rules empirically, not just by trusting the claim.** Write a small program that boxes an `int?` with `HasValue == true` and a separate `int?` with `HasValue == false`. For each, print `boxed.GetType()` (or check `boxed is int`) and `boxed == null`. Confirm the non-null case reports `System.Int32` (not `System.Nullable<System.Int32>`) and the null case really does equal `null`. Explain why this matters for code that does `if (someBoxedNullable is int x)` pattern matching.

3. **Break the mutation gotcha a different way — struct copies inside collections.** Create a `List<Counter>` (using this chapter's `Counter` struct) with a few elements, then write a `foreach (var c in list) c.Increment();` loop followed by printing the list's contents. Confirm the list is unchanged, and explain why — in terms of what `foreach` actually hands you on each iteration — this is the *same* underlying "structs are copied" rule from Episode 7, distinct from (but related to) this chapter's boxed-mutation gotcha where the mutation target actually was shared state (the box), not a copy.

4. **Stretch — measure `string.Format` vs. an interpolated string with `BenchmarkDotNet`.** Add a `[MemoryDiagnoser]` benchmark class comparing `string.Format("{0}", someInt)` against `$"{someInt}"` (assigned directly to a `string`), for a value-typed argument such as `int` or `DateTime`. Run it and confirm the allocation difference described in "Under the Hood" #8 — then try a reference-typed argument (a `string`) in both forms and confirm the gap disappears, since there's nothing to box either way.

## Challenge

**Predict the output before running it.**

```csharp
object boxed = 5;
object boxedAgain = boxed;

Console.WriteLine(ReferenceEquals(boxed, boxedAgain));

int first = 5;
int second = 5;
object boxedFirst = first;
object boxedSecond = second;

Console.WriteLine(ReferenceEquals(boxedFirst, boxedSecond));
```

Before running this: what do the two lines print, and are they the same? Most people expect either both `True` or both `False`. Write down, in terms of exactly when a `box` operation happens — not just "boxing is involved somewhere" — why the first line's result differs from the second's, and identify precisely which line of code in this snippet performs an actual boxing allocation and which lines perform an ordinary reference copy.
