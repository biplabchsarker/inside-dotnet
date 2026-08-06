"Value types go on the stack, reference types go on the heap." You've said this in an interview. So have I. It's also incomplete enough to be wrong.

Here's the part that gets skipped: a struct's storage location follows its CONTAINER's lifetime, not its own type category.

→ A struct declared as a local variable? Usually stack. Fine.
→ A struct that's a FIELD of a class instance? It lives on the heap, laid out inline as part of that object. `class Order { public decimal Total; }` — Total is on the heap with the rest of Order.
→ A struct that's BOXED (assigned to `object`, passed where boxing is implicit)? Heap, wrapped.
→ A struct CAPTURED by a lambda closure? Heap, hoisted into a compiler-generated closure class.

The real rule: something lives on the stack only if its lifetime is strictly nested inside a method call. Everything with shared or outliving-the-frame lifetime goes to the heap — value type or not.

And this isn't even a C# quirk. Every process on every OS gets a call stack from the CPU/hardware itself — dedicated stack-pointer registers, CALL/RET pushing and popping return addresses. What .NET specifically adds on top is the managed heap + GC: automated reclamation for the lifetimes that don't fit the stack's strict LIFO nesting.

One more myth-buster while we're here: you cannot catch a StackOverflowException. Not because Microsoft didn't bother — by the time it's detected, there's no stack space left to run your catch block, or even the CLR's own exception machinery. The process terminates immediately, by design.

This is Episode 6 of "Inside .NET" — opening Part II: Memory. Full breakdown with diagrams (including one that shows exactly where a struct field sits inside a heap object) and runnable code in the comments.

#dotnet #csharp #memorymanagement #softwarearchitecture #garbagecollection
