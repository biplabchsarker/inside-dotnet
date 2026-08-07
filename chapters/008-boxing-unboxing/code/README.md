# Chapter 008 — Code Samples

Two projects, matching the Example/Advanced/Performance tiers described in `article.md`.

## `Chapter08.Demo` (Example + Advanced)

Demonstrates:
- Boxing identity — two boxes of the same value are two distinct heap objects (`ReferenceEquals` proves it)
- Unboxing's exact-type requirement — unboxing to a compatible-but-different type throws `InvalidCastException`
- `ArrayList` (boxes every element) vs `List<int>` (doesn't), measured directly with `GC.GetAllocatedBytesForCurrentThread()`
- Boxing triggered by interface dispatch on a struct, and how a generic constraint avoids it
- The classic gotcha: mutating a boxed struct through an interface reference mutates the box, not your original variable

```bash
cd Chapter08.Demo
dotnet run
```

### Expected output

Exact byte counts may vary slightly by machine/runtime patch, but the shape is stable:

```
=== Inside .NET: Episode 9 — Boxing & Unboxing demo ===

--- 1. Boxing identity: same value, different boxes ---
  boxedA equals boxedB (value)?     True
  boxedA is boxedB (reference)?     False
  Mutating 'number' after boxing doesn't touch either box:
  number=99, boxedA=42, boxedB=42

--- 2. Unboxing requires the exact original type ---
  Unboxing to the original type 'int' works: 7
  Unboxing to 'long' threw: Unable to cast object of type 'System.Int32' to type 'System.Int64'.

--- 3. ArrayList (boxes) vs List<int> (doesn't) — measured allocation ---
  ArrayList.Add x1,000,000:  32,000,072 bytes allocated
  List<int>.Add x1,000,000:  4,000,056 bytes allocated
  Ratio (ArrayList / List<int>): 8.0x

--- 4. Interface dispatch boxes a struct; generic constraints don't ---
  Direct call, no boxing:            counter.Value = 5
  Through IIncrementable (boxed):     boxedCounter.Value = 5
  Through a generic constraint:      DescribeWithoutBoxing(counter) = 5

--- 5. Mutating a boxed struct through an interface doesn't touch your variable ---
  original.Value (unchanged):  0
  ((Counter)boxed).Value (the box's state): 2

=== Done ===
```

## `Chapter08.Benchmarks` (Performance)

Real `BenchmarkDotNet` numbers backing this chapter's Performance Notes claims — no estimates, no "should be roughly."

```bash
cd Chapter08.Benchmarks
dotnet run -c Release
```

Takes roughly 2 minutes (three benchmark classes, each with its own warm-up + iterations).

### What it measures

1. **`BoxingArithmeticBenchmarks`** — summing 100,000 `int`s directly vs. boxing and unboxing each one on every iteration (a literal box+unbox round trip, isolated from any container).
2. **`CollectionBoxingBenchmarks`** — `List<int>.Add` + sum vs. `ArrayList.Add` + sum over 100,000 items, same shape, only the container differs.
3. **`InterfaceDispatchBenchmarks`** — the same struct's `Increment()` called directly, through a generic constraint, and through an `IIncrementable` interface reference, 100,000 times each.

See the results tables in [`../../article.md`](../../article.md#performance-notes) for the measured numbers from this run, including the (deliberately surprising) zero-allocation result in benchmark 3 — explained there, not just reported.
