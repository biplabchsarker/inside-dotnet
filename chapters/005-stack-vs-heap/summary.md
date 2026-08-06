# Summary — Stack vs Heap: Where Your Data Actually Lives

- The **stack** is per-thread, LIFO, OS/CPU-provided (not a CLR invention) — default ~1MB on Windows for the main thread. Holds call frames: return addresses, parameters, and value-type locals. Allocation and deallocation are both just pointer moves — effectively free.
- The **managed heap** is shared across all threads in a process, holds reference-type instances, and is what the CLR adds on top of the OS-level stack. Allocation is (usually) also a pointer bump; **deallocation is not deterministic** — it happens whenever the GC decides to collect.
- Every process on every mainstream OS gets a call stack from hardware/OS mechanisms (`CALL`/`RET`, stack-pointer registers) — this predates and exists outside of .NET entirely.
- **The "value types = stack, reference types = heap" rule is an oversimplification.** A struct field on a class instance lives on the heap as part of that object. A boxed struct lives on the heap. A struct captured by a closure lives on the heap. Location follows lifetime/container, not type category.
- **`StackOverflowException` can't be caught** — by the time it's detected, there's no stack space left to run a handler or the CLR's own exception machinery, so the process is terminated immediately instead.
- A stack frame contains: return address, parameters, value-type locals, and exception-handling region bookkeeping. Frame teardown on return is just restoring the stack pointer — no per-variable cleanup.

**Next:** [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/article.md)
