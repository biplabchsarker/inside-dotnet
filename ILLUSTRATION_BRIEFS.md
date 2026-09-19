# Illustration Briefs — External Sourcing

This is the spec to hand to whoever produces Inside .NET's five signature illustrations per chapter — a commissioned illustrator, Canva, Figma, or an AI image-generation tool. See [IMAGE_GUIDE.md](IMAGE_GUIDE.md) for how these fit into the overall image system, and [BACKLOG.md](BACKLOG.md) for why this moved from hand-coded SVG to external sourcing (2026-08-08).

Chapters 000-012 currently show hand-coded SVG placeholders in these slots (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md#interim-svg-placeholders-chapters-000-012) — the allowance was extended from 000-008 to 000-010 on 2026-09-09, then to 000-012 on 2026-09-20 as those two chapters were drafted). They stay in place, unedited, until a v2 image from this brief replaces them; `article.md` in each of those chapters already references the standard filenames, so a v2 PNG can be dropped straight in without further edits once produced. Chapter 013 onward has no interim placeholder — for a chapter that hasn't been drafted yet, only the Hero/Concept/Internal/Memory images can be reasonably briefed ahead of time (from the chapter's planned topic); the Performance & Quick Reference image cannot be, since it requires real measured numbers that don't exist until the chapter is written and benchmarked — see each such brief below for exactly what's still open.

## The style brief — applies to all five images, every chapter

- **Background:** light. White (`#FFFFFF`) or near-white (`#F8FAFC`), with a subtle purple/blue gradient wash where it adds depth. No dark blueprint-grid background (that was the pre-2026-08-08 convention).
- **Palette:** .NET purple `#512BD4` and blue `#0078D4` as the primary accents, with their lighter tints for "stack/value type" (blue family) vs. "heap/reference type" (purple family) where the chapter's content maps onto that distinction. Full palette in [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md).
- **Form:** clean, isometric/pseudo-3D shapes (cubes, containers, layered blocks) with soft, subtle drop shadows — not flat 2D icons, not photoreal/painterly rendering. Generous whitespace. One concept per image.
- **Text:** minimal. A title, 2-4 short labels, maybe one short code snippet or one short callout line. These are not infographic posters — a reader should get the point in under three seconds, before reading any surrounding paragraph.
- **No code screenshots, no UI mockups, no stock photography, no scraped/copied artwork.** Everything is original.
- **Consistency:** every chapter's five images should look like they belong to the same book — same palette, same shadow/gradient treatment, same label typography (Segoe UI / Inter for text, Cascadia Code / Fira Code for any code snippet).

## Size and delivery

- **All five images: 1600×900 px (16:9), PNG.** One consistent size across all five and across every chapter — this is deliberate so any external source works from a single template instead of five different specs.
- **Filenames and destinations** (`NNN` = zero-padded chapter number, e.g. `008`):

| # | Image | Filename | Destination |
|---|---|---|---|
| 1 | Hero Cover | `NNN-hero.png` | `chapters/NNN-slug/images/` |
| 2 | Concept Overview | `NNN-concept.png` | `chapters/NNN-slug/diagrams/png/` |
| 3 | Runtime/Internal View | `NNN-internal.png` | `chapters/NNN-slug/diagrams/png/` |
| 4 | Memory/Execution Diagram | `NNN-memory.png` | `chapters/NNN-slug/diagrams/png/` |
| 5 | Performance & Quick Reference | `NNN-performance.png` | `chapters/NNN-slug/diagrams/png/` |

- No editable source file is required back from the external source (unlike the old SVG-first rule) — PNG only. If the source tool produces an editable file too (a Figma/Canva project, an AI tool's prompt), keeping it is a bonus, not a requirement.

## What each of the five images is *for*

1. **Hero Cover** — the chapter's opening visual, top of `article.md`. One central motif (the chapter's core mechanism/concept), 2-4 supporting elements around it, a short tagline. Pure storytelling — no internals yet.
2. **Concept Overview** — placed in **Visual Explanation**. The chapter's central idea, end to end, in one image. This is the "elevator pitch" image.
3. **Runtime/Internal View** — placed in **Under the Hood**. A denser breakdown of the actual internal mechanism — for the reader who wants the real detail.
4. **Memory/Execution Diagram** — placed in **Under the Hood**, alongside #3. A concrete before/after or step-by-step visualization of memory state, data flow, or execution order — "show me, don't just tell me."
5. **Performance & Quick Reference** — placed in **Performance Notes** (and reused at **Summary & Next Chapter**). Real measured numbers from the chapter's actual benchmark data, shown as clean bars or a small table, plus the one-line rule and the single most likely interview question. **Never invent numbers here** — pull them from the chapter's `article.md` Performance Notes section, which is itself sourced from a runnable `BenchmarkDotNet`/`GC.GetAllocatedBytesForCurrentThread` measurement.

---

## Per-chapter content briefs

Each row below is the one-sentence concept to depict — not a full script. The person/tool producing the image should read the referenced chapter's `article.md` for exact terminology and numbers.

### 000 — Welcome to Inside .NET

| Image | Depict |
|---|---|
| Hero Cover | The series' own identity — a single glowing path/thread running through the .NET stack, from source code down to CPU, establishing "this book goes all the way down." |
| Concept Overview | **Learning Journey Roadmap** — the 12-phase structure (Runtime Fundamentals → Memory → C# → Clean Code → Design Patterns → ASP.NET Core → DI → EF Core → Concurrency → Architecture → Production → Capstone) as a single vertical or left-to-right path, current chapter count marked. |
| Runtime/Internal View | **.NET Ecosystem Map** — how the CLR, BCL, ASP.NET Core, EF Core, and the SDK/tooling relate to each other as one labeled diagram, positioning where this series' chapters live within that map. |
| Memory/Execution Diagram | The 13-section chapter template itself, visualized as a labeled strip (cover → objectives → analogy → … → summary), so a reader immediately knows "every chapter has this shape." |
| Performance & Quick Reference | Not benchmark data (no code yet) — instead, a "how to use this book" quick reference: how to navigate, where code samples live, how to self-check with quizzes. |

### 001 — Execution Flow

| Image | Depict |
|---|---|
| Hero Cover | A single line of C# source code transforming, in one visual gesture, into running native code — the whole chapter's promise in one image. |
| Concept Overview | The full pipeline: **C# → Roslyn → IL → CLR (hostfxr/coreclr) → JIT → Native Code → CPU** as a left-to-right chain. |
| Runtime/Internal View | The handoff sequence in more detail: `dotnet run` → `hostfxr` → `coreclr` → assembly loader → type loader → JIT, each as a labeled stage. |
| Memory/Execution Diagram | A timeline contrasting a method's **first call** (through the prestub, JIT compilation, warm-up cost) against **later calls** (straight to cached native code) — the "why is the first call slower" visual. |
| Performance & Quick Reference | Real measured first-call vs. warm-call timing from this chapter's benchmark, plus the one-line rule about JIT warm-up. |

### 002 — The CLR

| Image | Depict |
|---|---|
| Hero Cover | The CLR as a central engine/container, with managed code, memory management, type safety, and cross-language interop as elements orbiting it. |
| Concept Overview | CLR's core responsibilities as one end-to-end image: type safety, memory management (GC), JIT compilation, exception handling, security — as spokes around the CLR core. |
| Runtime/Internal View | **CLR internal architecture** — its major subsystems (class loader, JIT, GC, exception manager, security manager, thread manager) as a labeled cutaway. |
| Memory/Execution Diagram | A virtual method call resolving through a method table/vtable lookup, step by step, contrasted with a non-virtual call's direct dispatch. |
| Performance & Quick Reference | Measured cost difference between a virtual and non-virtual call from this chapter's benchmark, plus the CTS/CLS one-line distinction. |

### 003 — JIT Compilation

| Image | Depict |
|---|---|
| Hero Cover | A method "crystallizing" from IL into native machine code — the JIT's core promise, one clean motif. |
| Concept Overview | **First Call → Prestub → JIT → Native Code → Code Cache → Future Calls** as a left-to-right chain — the mental model the reviewer specifically asked for. |
| Runtime/Internal View | Tiered compilation in detail: Tier 0 (quick, unoptimized) → Tier 1 (fully optimized) → On-Stack Replacement (OSR) as a labeled decision flow, with ReadyToRun and dynamic PGO shown as inputs that change the starting point. |
| Memory/Execution Diagram | The method lifecycle over time — calls 1 through N — showing exactly when Tier 0 compiles, when the transition to Tier 1 happens, and where OSR kicks in mid-loop. |
| Performance & Quick Reference | Measured cold-start/throughput tradeoffs between standard JIT, ReadyToRun, and Native AOT from this chapter's benchmark data. |

### 004 — Assemblies & Metadata

| Image | Depict |
|---|---|
| Hero Cover | An assembly as a self-describing sealed container — code + metadata + manifest bundled as one object. |
| Concept Overview | What makes an assembly "the same" to the CLR — name, version, culture, public key token — as one labeled identity card. |
| Runtime/Internal View | **PE/COFF file structure** — the CLR header and metadata streams/tables as a labeled cutaway of the physical file layout. |
| Memory/Execution Diagram | A `call`/`newobj` IL instruction resolving through a metadata token to a concrete `MethodDesc` — the token-to-method resolution path, step by step. |
| Performance & Quick Reference | The actual assembly resolution algorithm (`deps.json` + probing, scoped per `AssemblyLoadContext`) as a quick-reference decision flow, plus any measured resolution-cost data from the chapter. |

### 005 — Stack vs. Heap

| Image | Depict |
|---|---|
| Hero Cover | Two distinct regions — a fast, ordered stack and a broader, garbage-collected heap — as two contrasting visual containers. |
| Concept Overview | Why the CLR uses two memory regions instead of one general allocator — the stack's LIFO discipline vs. the heap's arbitrary-lifetime objects. |
| Runtime/Internal View | **Stack frame layout** — a single frame's contents (return address, parameters, locals) stacked visually, with a method call pushing a new frame and a return popping it. |
| Memory/Execution Diagram | A recursive call sequence growing the stack frame by frame, contrasted with the moment it overflows — the `StackOverflowException`-can't-be-caught visual. |
| Performance & Quick Reference | Measured frame push/pop cost vs. heap allocation cost from this chapter's benchmark, plus the one-line "why teardown is a single pointer move" rule. |

### 006 — Value Types vs. Reference Types

| Image | Depict |
|---|---|
| Hero Cover | A value (copied whole) beside a reference (a pointer to one shared object) — the chapter's central contrast, one clean image. |
| Concept Overview | Assignment/pass semantics side by side: assigning a value type copies the data; assigning a reference type copies the pointer. |
| Runtime/Internal View | Memory layout differences: a value type inline in a stack frame/array/field vs. a reference type's header + data on the heap. |
| Memory/Execution Diagram | The mutable-struct trap made visual — mutating a struct through a property/indexer that actually operates on a temporary copy, and the value silently not sticking. |
| Performance & Quick Reference | Measured copy cost across `struct`, `readonly struct`, and an equivalent `class` (by value vs. by `in` vs. by reference) from this chapter's real `BenchmarkDotNet` data. |

### 007 — Object Allocation

| Image | Depict |
|---|---|
| Hero Cover | A fresh object materializing on the heap — header + zeroed fields — via a single clean bump-pointer motion. |
| Concept Overview | The allocation fast path: `new SomeClass()` → check allocation context → bump pointer → write header → construct → return reference. |
| Runtime/Internal View | The object header's exact layout (method table pointer + sync block index) sitting in front of a thread's private allocation context, carved from the shared Gen 0 segment. |
| Memory/Execution Diagram | Construction order made visual — base class constructor running first against zeroed derived fields, then control passing to the derived constructor — the "virtual call from a constructor sees uninitialized state" gotcha. |
| Performance & Quick Reference | Measured bytes/time per allocation from this chapter's `GC.GetAllocatedBytesForCurrentThread` data, plus the LOH size-threshold one-line rule. |

### 008 — Boxing & Unboxing

*(Current SVG placeholders already implement this brief in the light-isometric style — see `chapters/008-boxing-unboxing/diagrams/svg/`. Listed here for completeness/consistency and as the reference example for chapters above and below it.)*

| Image | Depict |
|---|---|
| Hero Cover | A value type cube (blue, stack) and a boxed object cube (purple, heap) connected by glowing Boxing/Unboxing arcs, a 3-line code snippet, callouts for "Performance Cost," "Hidden Allocation," "Type Safety." |
| Concept Overview | Boxing Flow (Value Type → Boxing → Reference Object) and Unboxing Flow (Heap Object → Type Check → Copy Value → Stack Variable) as two clean rows. |
| Runtime/Internal View | The box's exact byte layout — Method Table Ptr (8B) + Sync Block Index (8B) + padded value (8B) = 24 bytes, measured. |
| Memory/Execution Diagram | Stack/heap state before boxing (heap empty) vs. after boxing (stack holds a reference, heap holds the real object) — side by side. |
| Performance & Quick Reference | Measured bars: direct/unboxed baseline, box+unbox round trip (5.94×, 24 B/box), `ArrayList` vs `List<int>` (4.38×, 8× memory), and the escape-analysis zero-allocation result (0.98×, 0 B) — plus the `Nullable<T>` boxing interview question. |

### 009 — Strings & Interning

| Image | Depict |
|---|---|
| Hero Cover | Two "Hello, World" strings — one glowing path from a single shared canonical instance (labeled *literal*), one drawn as two separate, disconnected instances despite identical text (labeled *runtime-built*) — the chapter's central contrast in one image. |
| Concept Overview | Two rows: literals flowing into one shared **Intern Pool** icon; runtime-built strings (concatenation, `StringBuilder`, `Substring`) flowing past it into ordinary heap objects, with a dotted "only if you call `string.Intern`" arrow back to the pool. |
| Runtime/Internal View | The `Equals`/`==` decision path as a labeled flowchart: `ReferenceEquals` check first, then length check, then ordinal byte comparison — versus a separate branch for culture-aware comparison routing through a "globalization engine" box. |
| Memory/Execution Diagram | Before/after GC collection, two heaps side by side: one where 200,000 discarded strings return to baseline, one where 200,000 interned strings stay resident — the intern-pool-as-permanent-root visual. |
| Performance & Quick Reference | Measured bars: Ordinal vs. CurrentCulture comparison (25.66×), `Equals` same-reference vs. different-reference (51.74×), `Substring` whole-range vs. partial (23.62×, 152 B/call) — plus the intern-pool memory-retention numbers (57 KB baseline vs. 16.2 MB retained) and the Turkish-I interview question. |

### 010 — Garbage Collection Fundamentals

| Image | Depict |
|---|---|
| Hero Cover | A tidy, glowing heap region with a small "sweep" motion clearing away a few dim, disconnected blocks while a bright, connected cluster (traced from a root icon) stays lit — the chapter's central idea (reachability, not counting) in one image. |
| Concept Overview | Roots (stack/statics/registers/GC-handle icons) with glowing trace-lines reaching into a heap region — reachable objects lit, unreachable ones dimmed — labeled "Mark." |
| Runtime/Internal View | Mark → Sweep → Compact as three side-by-side heap snapshots: marked (some blocks lit), swept (gaps where dim blocks were), compacted (blocks slid together, one clean free region at the end). |
| Memory/Execution Diagram | A small object's promotion path — Gen 0 → Gen 1 → Gen 2 — as three nested regions, with a "survives a collection" arrow moving it one region deeper each time. |
| Performance & Quick Reference | Measured bars: Gen 0 vs. Gen 1 vs. Gen 2 collection cost (5.84× for a full collection), Workstation vs. Server GC throughput (Server measured *slower* for a single-threaded workload — the counter-intuitive result worth calling out), and pre-sized vs. growing `List<T>` (~1.9× faster) — plus the "does `GC.Collect(0)` touch Gen 2?" interview question. |

### 011 — GC Generations & the Large Object Heap

| Image | Depict |
|---|---|
| Hero Cover | Three heap regions in one image, visually distinct in scale and treatment: a small, frequently-swept ephemeral region (Gen 0/Gen 1), a larger long-term Gen 2 region, and a separate Large Object Heap holding a few oversized blocks — the chapter's core point that "not all heap memory is managed the same way." |
| Concept Overview | The segment-budget model end to end: Gen 0 fills its adaptive budget → triggers a Gen 0 collection → survivors promoted to Gen 1 → Gen 1's own adaptive budget → long-term survivors reach Gen 2 — shown as one linear promotion pipeline, with a parallel branch showing a large allocation (total object size ≥ 85,000 bytes) skipping straight to the LOH instead of entering Gen 0 at all. |
| Runtime/Internal View | The card table / write barrier mechanism: a Gen 2 object's field gets updated to reference a newly-allocated Gen 0 object; the write barrier marks the corresponding card-table byte "dirty"; the next Gen 0 collection only rescans dirty cards instead of the entire Gen 2 heap. |
| Memory/Execution Diagram | A size-threshold branch, concretely: allocating a small array (e.g. `new byte[84_975]`, reporting `GC.GetGeneration() == 0`) walks the normal Gen 0 bump-pointer path; allocating one byte more (`new byte[84_976]`, reporting `GC.GetGeneration() == 2`) crosses the 85,000-byte total-object-size threshold and lands directly on the LOH — same `new byte[N]` call, two different destinations, one byte apart, shown side by side. |
| Performance & Quick Reference | Real measured bars, from `chapters/011-gc-generations-loh/article.md`'s Performance Notes: **LOH-fragmented full collection vs. one forced to compact** (`GCSettings.LargeObjectHeapCompactionMode = CompactOnce`) — 52.15 μs (sweep only) vs. 4,527.08 μs (compacting), **87.41×**; **the card-table proxy** — `GC.Collect(0)` cost against a Gen 2 graph with a *fixed* 2,000-object dirty set, growing 40× in total graph size (50,000 → 2,000,000 objects) for under 10× the cost (201.9 μs → 1,973.6 μs) — sub-linear, not perfectly flat, worth a small "noisy measurement" caveat rather than a clean single ratio; **LOH vs. small-object allocation** — close wall-clock times (3.258 ms vs. 3.589 ms) but a stark collection-count gap worth calling out instead (~8 Gen-0-only collections vs. ~31 full Gen 0+1+2 collections per 1,000 runs, for the same ~100 MB total). Likely interview question: "Does the LOH get compacted by default, and what changes that?" |

### 012 — IDisposable & Finalizers

| Image | Depict |
|---|---|
| Hero Cover | A visualization of a pristine managed heap (blue cubes) alongside a chaotic "unmanaged" native memory space (dark, jagged blocks), connected by a bridge labeled "IDisposable". |
| Concept Overview | A flowchart showing the dual-path cleanup pattern: an explicit `Dispose()` call skipping the finalization queue vs. a missed `Dispose()` falling into the finalization queue and causing a delayed, two-pass GC cleanup. |
| Runtime/Internal View | A diagram of the Finalization Queue and F-Reachable Queue internal data structures during a GC cycle, showing how objects with finalizers get promoted to older generations if they aren't explicitly disposed. |
| Memory/Execution Diagram | A side-by-side execution trace of C# 8 `using` declarations (implicit scope boundary) vs. traditional `using` blocks (explicit brackets) mapping down to the underlying `try/finally` IL generation. |
| Performance & Quick Reference | Not yet briefable — wait for benchmark data to show GC cost of finalizers vs. non-finalizers, and the `SafeHandle` advantage. Likely interview question: "What is the difference between Dispose and a finalizer?" |

---

## Notes for whoever produces these

- **Don't fabricate data.** Every number that appears in a Performance & Quick Reference image must trace back to a real measurement already written in that chapter's `article.md` Performance Notes section. If a chapter hasn't been benchmarked yet, ask rather than inventing a plausible-looking number.
- **Terminology must match [STYLE_GUIDE.md](STYLE_GUIDE.md#terminology)** — CLR vs. "the runtime," IL not "bytecode," .NET vs. ".NET Framework," etc. A label that uses the wrong term undermines the whole series' consistency.
- **When in doubt about what a chapter actually says**, read the chapter's `article.md` directly rather than working from this brief's one-line summary alone — this document is a starting point, not a substitute for the source chapter.
