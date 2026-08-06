# Inside .NET — Episode 6: Stack vs Heap: Where Your Data Actually Lives

*Part II — Memory*

Part I traced how source code becomes running native instructions. Part II asks a different question: once code is executing, where does the data it operates on actually live? Every allocation in a .NET program lands in one of exactly two places — the stack or the heap — and getting this distinction right is the foundation for everything else in memory management: allocation cost, boxing, GC generations, memory leaks.

## The kitchen analogy

Each cook has their own cutting board (the stack — per-thread, private, cleared the instant an order is plated). The walk-in pantry (the managed heap) is shared by the whole kitchen — any cook can grab shelf space for ingredients that need to outlive one order, but nobody individually decides when a pantry item gets thrown out. That's the head chef's job (the GC), done on their own schedule, checking what's still needed before clearing space.

## Two lifetime patterns, two mechanisms

A running program needs both a deterministic, strictly-nested lifetime pattern (method locals — perfect for a simple LIFO region managed by moving one pointer) and a shared, unpredictable lifetime pattern (objects referenced from multiple places, or that outlive their creating method — which needs an allocator plus a way to know when nothing references something anymore).

This split isn't a C# invention. Every process on every mainstream OS gets a call stack from the CPU/OS directly — dedicated stack-pointer registers, `CALL`/`RET` instructions that push and pop return addresses. What .NET adds on top is the *managed heap plus garbage collector*: automated reclamation for the lifetimes that don't fit the stack's nesting.

## What's actually in a stack frame

Return address, parameters, value-type locals, and exception-handling bookkeeping. Frame teardown on return isn't per-variable cleanup — it's one pointer restore, which is why stack allocation and deallocation are both essentially free.

## The myth that needs killing

"Value types go on the stack, reference types go on the heap" is close enough to be dangerous. It only holds for local variables of value types. It breaks the moment the container's lifetime differs:

- A struct **field on a class instance** lives on the heap, laid out inline as part of that object.
- A **boxed** struct lives on the heap, wrapped.
- A struct **captured by a closure** gets hoisted into a heap-allocated compiler-generated class.

The real rule is about lifetime and container, not type category.

## Why you can't catch a stack overflow

By the time the CPU/OS detects the guard page at the end of a thread's stack has been hit, there's no stack space left to run a `catch` block — or even the CLR's own exception-dispatch machinery, which needs stack space too. Since .NET 2.0, the CLR's answer is to terminate the process immediately rather than attempt unsafe recovery.

## Try it yourself

The [companion code sample](code/Chapter05.Demo/Program.cs) allocates a million structs as plain locals (negligible heap growth) and then a million of the *same* struct as a field of a class (measurable heap growth via `GC.GetTotalMemory`) — same type, different container, different memory story.

*Next: [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/medium.md)*
