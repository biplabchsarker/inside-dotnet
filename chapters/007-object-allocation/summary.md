# Summary — Object Allocation

**TL;DR:** `new SomeClass()`'s fast path is a bump-pointer allocation into the current thread's private allocation context, followed by writing an object header (method table pointer + sync block index) into memory that was already zeroed before you ever saw it. Constructors then run base-to-derived. Objects >= 85,000 bytes and boxed values ride the same mechanism but are covered in their own chapters.

**Key takeaways:**

- Allocation in the common case is a size check plus a pointer bump -- no search, no free list, no lock. This is the mechanism behind Episode 6's "heap allocation is also just a pointer bump" claim.
- Each thread owns its own **allocation context** (a small private slice of the shared Gen 0 segment) specifically so allocation doesn't serialize on a global lock in the common case.
- **Zeroing is a guarantee, not a per-allocation step.** New objects always start at their default field values, but the zeroing work happens when pages are committed or reclaimed by the GC -- not at the moment of `new`.
- A thread's allocation context running out -- not a timer, not periodic polling -- is what triggers a Gen 0 collection. This chapter stops at that trigger; the collection itself is Episode 10/11's territory.
- Objects >= 85,000 bytes go to the Large Object Heap ([Episode 12](../011-gc-generations-loh/article.md)); boxing rides this same allocator as one more trigger of it ([Episode 9](../008-boxing-unboxing/article.md)) -- neither is duplicated here.
- Construction runs **base class first, then derived**, against already-allocated, already-headered, already-zeroed memory. A `virtual`/`abstract` call made from a base constructor executes against an object whose derived-specific fields are still at their zeroed default -- a real, recurring bug source, demonstrated directly in this chapter's code sample.
