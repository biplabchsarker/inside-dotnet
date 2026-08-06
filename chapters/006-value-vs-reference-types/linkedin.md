"Structs are stack, classes are heap" is the sentence I hear most often about C# — and it's wrong in a way that causes real bugs.

Here's the precise rule: value types (struct, enum, int, bool, double...) copy their DATA on assignment. Reference types (class, string, array, delegate) copy the REFERENCE — both variables end up pointing at the same object.

Where a value type is physically stored is a completely separate question from that copy behavior. A local `int` sits on the stack. But a struct that's a FIELD of a class lives inside that class instance — on the heap, if the instance is heap-allocated. A struct inside an array lives inside the array's own heap block. The only time a value type gets its own independent heap allocation is when it's boxed.

This is where the classic mutable-struct bug comes from:

foreach (var p in points) { p.X = 5; }

This compiles cleanly. It changes nothing. `p` is a fresh copy every iteration, thrown away at the end of the loop. The fix: make structs `readonly struct` wherever you can, so the mistake fails to COMPILE instead of failing silently at runtime.

Also worth internalizing: "structs are always faster" is a myth. Small structs, copied rarely, beat heap allocation. Large structs, copied repeatedly through call layers, can cost MORE in aggregate memcpy time than a class's one-time allocation plus pointer copies everywhere else. `in` / `ref readonly` exists specifically to pass large read-only structs without paying that copy cost.

This is Episode 7 of Inside .NET — full breakdown with diagrams, the record class vs record struct distinction, and runnable code in the comments.

#dotnet #csharp #memorymanagement #softwarearchitecture
