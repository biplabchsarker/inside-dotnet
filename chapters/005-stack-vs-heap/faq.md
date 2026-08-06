# FAQ — Stack vs Heap: Where Your Data Actually Lives

**Q: If a struct field on a class always lives on the heap, is there ever a downside to using a struct instead of a class for that field?**
A: Yes — you lose the independent-identity and reference-sharing semantics of a class, and every read/write copies the whole struct by value unless you're careful with `ref`/`in`. The heap-vs-stack question is orthogonal to the value-vs-reference-semantics question, which is exactly what [Episode 7](../006-value-vs-reference-types/article.md) covers next.

**Q: Can you increase the default ~1MB stack size to avoid `StackOverflowException`?**
A: Yes, via `new Thread(threadStart, maxStackSizeBytes)` for a specific thread (the main thread's size is typically set via the executable's linker settings or `ulimit`-equivalent on other platforms). This raises the ceiling but doesn't fix unbounded recursion — it just takes longer to hit the same wall.

**Q: Does every heap allocation trigger a garbage collection?**
A: No. Most allocations just bump a pointer inside the current generation-0 segment. A collection is only triggered when that segment (or another generation, or the Large Object Heap) doesn't have enough contiguous free space for the next allocation, or when explicitly requested.

**Q: Is `stackalloc` the same as declaring a local struct?**
A: No — `stackalloc` explicitly reserves a block of stack memory for an array-like buffer (usually wrapped in `Span<T>`) and is opt-in, used for scratch buffers you know are small and bounded. A local struct variable may or may not literally stay on the stack depending on JIT decisions (escape analysis, capturing) — `stackalloc` is a guarantee, a struct local is a convention that usually holds.

**Q: Why does the managed heap need generations (Gen 0/1/2) if allocation is just a pointer bump?**
A: Generations are about making *collection* cheap, not allocation. Most objects die young, so the GC concentrates its work on the smallest, most recently allocated region (Gen 0) most of the time, rather than scanning the entire heap on every collection. That's covered in depth in [Episode 11 — GC Generations & LOH](../010-gc-generations-loh/article.md).
