# FAQ — Object Allocation

**Q: Is heap allocation in .NET actually slow?**
A: Not in the common case. Allocating a small object is a size check plus a pointer bump into the current thread's allocation context, followed by writing a small header — close to the cost of a stack pointer move. What's expensive is the *eventual* Gen 0 collection triggered when a thread's allocation context runs out, and that cost scales with the live object graph the GC has to trace, not with allocation count.

**Q: Why does each thread get its own allocation context instead of sharing one pointer?**
A: A single shared pointer would need a global lock taken on every allocation across every thread — the single most frequent operation in a managed program. Per-thread contexts let each thread bump its own pointers with zero coordination; coordination only happens on the rare event of refilling an exhausted context from the shared Gen 0 segment.

**Q: If .NET guarantees new objects are zeroed, does that mean allocation zeroes memory every time?**
A: No. The zeroing work is done when the OS commits fresh pages (which already arrive zeroed) and when the GC reclaims/compacts memory during a collection — not at the moment of each `new`. The guarantee you get is real and unconditional; the cost is relocated to those points rather than paid on the allocation hot path.

**Q: What actually triggers a garbage collection during allocation?**
A: A thread's allocation context running out of room with no unclaimed space left in the shared Gen 0 segment to refill from. It's not a timer, a background thread polling memory pressure, or a fixed allocation count — it's this specific exhaustion event. What the collection itself does is covered in the dedicated GC chapters, not here.

**Q: Why do large objects (≥ 85,000 bytes) get treated differently?**
A: They're routed to the Large Object Heap instead of being bump-allocated alongside small, short-lived objects, because collecting/compacting very large objects on the same schedule as small ones would be wasteful. This chapter only covers the routing decision — the LOH's actual mechanics are [Episode 12 — GC Generations & LOH](../011-gc-generations-loh/article.md).

**Q: Is boxing a completely different allocation mechanism from a normal `new`?**
A: No — boxing allocates through the exact same fast path described in this chapter. The only difference is what happens afterward: the value's field data gets copied into the freshly allocated block. Boxing's specific triggers and costs are covered in [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md).

**Q: Why does calling a virtual method from a constructor cause bugs?**
A: Because construction runs strictly base-to-derived: the base class's constructor body runs to completion (including any virtual calls it makes) before the derived class's field initializers and constructor body run at all. If the base constructor calls a virtual method the derived class overrides, that override executes against an object whose derived-specific fields are still at their zeroed defaults — not whatever the derived constructor was about to set them to.
