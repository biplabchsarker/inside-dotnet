# Quiz — GC Generations & the Large Object Heap

1. What's the exact size threshold that routes an allocation to the Large Object Heap instead of Gen 0 — and is it measured in element count or total object size?
2. What does `GC.GetGeneration()` report for an object living on the LOH, and why that number specifically?
3. What mechanism lets a Gen 0 collection find a live reference from a Gen 2 object without rescanning all of Gen 2 — and what part of it does application code never write directly?
4. Is the LOH compacted by every full collection by default? If not, what forces a one-time compaction, and what happens to that setting immediately afterward?
5. Why does POH exist, and what used to happen to a long-held pinned object before .NET 5 introduced it?

<details>
<summary>Answers</summary>

1. 85,000 bytes of total object size (including the object's header, not just its element data) — not element count and not "just the data."
2. `2` — because the LOH is collected together with a full (Gen 2) collection; it's a physically separate segment but not a distinct generation from a collection-counting perspective.
3. The card table and write barrier. The write barrier — inserted automatically by the JIT on every reference-type store that could create a cross-generational reference — is the part application code never writes; it just marks a card-table byte dirty.
4. No, not by default — the LOH is swept but not compacted unless `GCSettings.LargeObjectHeapCompactionMode` is explicitly set to `CompactOnce`, which forces exactly one compacting pass on the next collection that reaches the LOH and then resets itself back to `Default` immediately afterward.
5. POH (Pinned Object Heap, .NET 5+) exists so a long-held pinned allocation has its own segment instead of sitting as an immovable obstacle inside a regular generational segment. Before POH, a long-held pin (via `fixed` or a pinned `GCHandle`) forced the GC to work around it during every compaction of whatever segment it landed in.

</details>
