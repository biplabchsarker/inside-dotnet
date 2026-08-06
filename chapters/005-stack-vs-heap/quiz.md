# Self-Check Quiz — Stack vs Heap: Where Your Data Actually Lives

1. Is the call stack a .NET-specific mechanism, or does it exist independently of any managed runtime? Explain why.
2. Give a concrete example where a value type (`struct`) ends up stored on the heap, not the stack.
3. What three things are typically found in a method's stack frame?
4. Why can't a `StackOverflowException` be caught with a normal `try`/`catch`?
5. In the common case, is heap allocation in .NET actually slow compared to stack allocation? What's the real source of heap-related cost?

<details>
<summary>Answers</summary>

1. It's independent of any managed runtime — every process on every mainstream OS gets a call stack from CPU/OS mechanisms (dedicated stack-pointer registers, `CALL`/`RET` instructions). Any language, managed or not (C, C++, Rust, Go), relies on the same underlying mechanism. .NET adds the managed heap and GC on top of it, not the stack itself.
2. A struct that is a field of a class instance (e.g., `class Order { public decimal Total; }`) — `Total` is laid out inline as part of the `Order` object's memory block on the heap. Boxing a struct into an `object`, or capturing a struct in a lambda closure, are two other mechanisms that move it to the heap.
3. The return address, the method's parameters (or references to them), and its value-type local variables — plus JIT/runtime bookkeeping for exception-handling regions.
4. Because handling any exception — including running a `catch` block or the CLR's own exception-dispatch/unwinding logic — requires pushing more stack frames to do that work, and by definition there's no stack space left once an overflow is detected. The CLR terminates the process immediately instead of attempting unsafe recovery.
5. No — in the common case heap allocation is also just a pointer bump (bump allocation into the current Gen 0 segment), comparably cheap to a stack pointer move. The real cost isn't the allocation call itself; it's the *non-deterministic* collection pause that happens later when a generation fills up and the GC has to trace and reclaim memory.

</details>
