# Quiz — Object Allocation

1. What two things does the CLR write into an object's header at allocation time, and where were those concepts first introduced in this series?
2. Why doesn't allocating heavily from two threads at once serialize on a shared lock in the common case?
3. Is memory zeroed at the moment of each `new`? If not, when and where does the zeroing actually happen?
4. In what order do field initializers and constructor bodies run across a base/derived class chain?
5. What size threshold routes an allocation to the Large Object Heap instead of the normal path, and which chapter covers what happens after that routing decision?

<details>
<summary>Answers</summary>

1. A method table pointer (identifying the object's exact runtime type) and a sync block index (historically the indirection point for `lock`/`Monitor` and default hashing) — both introduced in [Episode 3 — The CLR](../002-clr/article.md) and recapped here as the payload the allocator writes.
2. Because each thread allocates from its own private allocation context (a small slice of the shared Gen 0 segment) — bumping that context's own pointer requires no coordination with any other thread. Coordination is only needed when a context is exhausted and a thread needs to claim more shared space.
3. No — freshly committed OS pages arrive already zeroed, and pages reclaimed by a collection are re-zeroed by the GC during collection/compaction, not at the moment of allocation. The guarantee is real; the work is relocated off the allocation hot path.
4. Base class first — its field initializers, then its constructor body — followed by the derived class's field initializers, then its constructor body. This is enforced by the compiler implicitly chaining to the base constructor unless you explicitly chain elsewhere with `: base(...)` or `: this(...)`.
5. Roughly 85,000 bytes. What happens on the Large Object Heap side of that decision is [Episode 12 — GC Generations & LOH](../011-gc-generations-loh/article.md), deliberately not this chapter.

</details>
