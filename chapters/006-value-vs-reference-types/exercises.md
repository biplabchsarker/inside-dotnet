# Exercises — Value Types vs Reference Types

These build directly on [`code/Chapter06.Demo/Program.cs`](code/Chapter06.Demo/Program.cs). Do them in a scratch console project (or extend the demo project itself) so you can run and observe the actual output, not just reason about it on paper.

## Exercise 1 — Reproduce and fix the mutable-struct trap

Write a mutable `struct Money { public decimal Amount; public string Currency; }` and a `List<Money>` with a few entries. Write a method `ApplyDiscount(Money m, decimal percent)` that mutates `m.Amount` and returns nothing (`void`).

1. Call it in a `foreach` loop over the list and confirm — by printing the list before and after — that nothing changed.
2. Explain in a comment *why* nothing changed, referencing which copy got mutated.
3. Fix it two different ways: (a) make `ApplyDiscount` return a new `Money` and have the caller write it back by index, and (b) convert `Money` to a `readonly struct` with an `init`-only `Amount` and confirm the original mutating method now fails to compile.

## Exercise 2 — Measure the struct-size crossover point

Using `BenchmarkDotNet` (or a manual `Stopwatch` loop of a few million iterations if you don't want to add the dependency), benchmark passing a struct by value versus passing the same data as a `class` through a call chain of 3–4 nested method calls, for three struct sizes: 2 fields (16 bytes), 8 fields (64 bytes), and 32 fields (256 bytes).

1. Plot or tabulate the results.
2. Identify the approximate size at which the struct's pass-by-value cost overtakes the class's allocation-plus-pointer-copy cost in your environment.
3. Compare your result against the "roughly 16 bytes" rule of thumb given in the chapter's Performance Notes section — do your numbers support a different threshold, and if so, what does that tell you about relying on rules of thumb versus measuring?

## Exercise 3 — Design a domain value object, then defend the choice

Pick a real value from a domain you know (e.g. `Money`, `DateRange`, `GeoCoordinate`, `Percentage`). Implement it twice: once as a `readonly struct` (or `record struct`) and once as an immutable `record class`.

1. Write a short design note (a few sentences, in a comment block or a scratch `NOTES.md`) arguing which one your team should standardize on for this specific value, referencing its typical size and how often it's likely to be copied through a call chain versus stored in collections.
2. Now assume the value grows — add 6 more fields to simulate scope creep. Re-run your reasoning. Does your recommendation change, and at what point?

This exercise is deliberately open-ended — the goal is practicing the Architect Perspective judgment call from the chapter, not arriving at one "correct" answer.

## Challenge

**Predict the output before running it.**

```csharp
struct Counter { public int Value; }

var list = new List<Counter> { new Counter { Value = 1 } };
var item = list[0];
item.Value = 99;

Console.WriteLine(list[0].Value);   // A

var arr = new Counter[] { new Counter { Value = 1 } };
arr[0].Value = 99;

Console.WriteLine(arr[0].Value);    // B
```

`A` and `B` look like the same operation on two different collection types. They print different results. Write down what you predict for each, and *why the collection type changes the answer* — specifically, what `list[0]` returns versus what `arr[0]` returns as an addressable location — before running it. This is the mutable-struct trap from this chapter, wearing a slightly different disguise.
