# Exercises — GC Generations & the Large Object Heap

1. **Find your own exact LOH crossover for a type other than `byte`.** Starting from the size-threshold section of [`code/Chapter11.Demo/Program.cs`](code/Chapter11.Demo/Program.cs), write a small loop that allocates `new int[N]` (4 bytes per element, plus the same ~24-byte array header) for increasing `N`, printing `GC.GetGeneration()` for each, until you find the exact `N` where it flips from `0` to `2`. Is the byte-size crossover point the same 85,000 you'd expect, even though the *element count* where it happens is completely different from the `byte[]` case in this chapter? Explain why in one sentence.

2. **Reproduce LOH fragmentation, then measure how much a `CompactOnce` actually saves you.** Starting from `RunLohFragmentationDemo` in [`code/Chapter11.Demo/Program.cs`](code/Chapter11.Demo/Program.cs), change `ObjectSize` from 200,000 to 1,000,000 and `ObjectCount` from 200 to 40, re-run, and record the `FragmentationAfterBytes` before and after `CompactOnce`. Then time the compacting `GC.Collect()` call itself with a `Stopwatch`. Does the compaction's wall-clock cost scale with the total live LOH bytes being moved, roughly matching what `Chapter11.Benchmarks`' `LohFragmentationCompactionBenchmarks` measured at a different size?

3. **Test whether the card table's benefit actually depends on the dirty set staying small.** Starting from `CardTableProxyBenchmarks` in [`code/Chapter11.Benchmarks/Program.cs`](code/Chapter11.Benchmarks/Program.cs), change `DirtyObjectCount` from 2,000 to a value equal to the full `Gen2GraphSize` for each parameterized run (i.e., dirty every object in the graph, not just a fixed subset), and re-run with `dotnet run -c Release`. Does the `CollectGen0WithFixedDirtySet` cost now scale with `Gen2GraphSize` where it didn't before? What does that tell you about what the card table actually saves you from — the *size of Gen 2*, or the *size of what changed in Gen 2*?

4. **Stretch — watch `LOH Size` and fragmentation on a real workload with `dotnet-counters`.** Write a small console loop that allocates and drops 200,000-byte arrays continuously for 30+ seconds (similar to this chapter's `RunLohFragmentationDemo`, but without ever compacting), and attach `dotnet-counters monitor --counters System.Runtime` to the running process. Record the steady-state `LOH Size`. Then add a periodic `GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce` followed by a `GC.Collect()` every few seconds and re-run — confirm the pattern in `LOH Size` changes, and connect what you see back to this chapter's "Under the Hood" explanation of why compaction isn't the default.

## Challenge

**Predict the output before running it.**

```csharp
var a = new byte[84_975];
var b = new byte[84_976];

Console.WriteLine($"a: {GC.GetGeneration(a)}");
Console.WriteLine($"b: {GC.GetGeneration(b)}");
```

Both arrays are declared one byte apart in size. Most people's first instinct is that both print the same generation — after all, 84,975 and 84,976 bytes are practically identical amounts of data, and nothing about the code *looks* like it should branch differently between them.

Before running it: will `a` and `b` report the same generation, or different ones? If different, which one is `0` and which is `2`, and — in terms of the *total object size* the CLR actually compares against the 85,000-byte LOH threshold, not just the array's visible element count — explain exactly why one byte is enough to flip the answer.
