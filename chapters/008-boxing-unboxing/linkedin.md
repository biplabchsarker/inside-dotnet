"Boxing is slow" is repeated so often it's become folklore. I measured it instead — and one result surprised me.

Boxing a struct just to call a method through an interface reference, in a tight loop, showed **zero allocated bytes** in a real `BenchmarkDotNet` run on .NET 10. Not "small." Zero — identical, statistically, to calling the same method directly on the struct with no interface involved at all.

The reason: modern RyuJIT does escape analysis. If it can prove a box never leaves the method that creates it — never stored in a field, never returned, never added to a collection — it removes the allocation entirely.

Change one thing — put that same box in an `ArrayList` instead — and the allocation is real: 8x the memory of the `List<int>` equivalent, measured in the same benchmark run.

That's the actual lesson of this chapter: boxing's cost isn't a fixed tax, it's a property of whether the box escapes. `unbox` isn't a conversion either — it's an exact type-match check, which is why `(long)someBoxedInt` throws even though `int` widens to `long` everywhere else in C#.

This is Episode 9 of Inside .NET — full IL mechanics, five diagrams, and three real BenchmarkDotNet result tables, not estimates.

#dotnet #csharp #memorymanagement #softwarearchitecture
