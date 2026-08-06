# Backlog

Deferred items from review feedback — things worth doing, deliberately not done yet, so they don't get lost. Check this before starting a new phase of work; move an item to [CHANGELOG.md](CHANGELOG.md) when it ships.

## Illustration quality ceiling (2026-08-07)

Two rounds of review feedback have asked for progressively more "cinematic / iconic / memorable" illustrations. The current hand-coded SVG toolset (gradients, glow filters, blueprint grids, consistent iconography — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md)) has been pushed about as far as it goes; further requests in this direction hit diminishing returns with the same technique.

**What would actually move the needle:** a commissioned illustrator, or an AI image-generation tool integrated into the workflow — neither is available in this environment today.

**Decision needed from the project owner, not to be re-attempted with more SVG iteration:** either (a) accept the current hand-coded SVG ceiling as "good enough — engineering blueprint / vector infographic quality" and stop asking for more cinematic treatment, or (b) source/commission real illustration work for the four signature images (Hero, Concept, Deep-Dive, Cheat Sheet) per chapter, at which point this project's SVG originals become the *layout spec* a real illustrator or image-gen tool works from, not the final artifact.

## Future runtime-internals depth (for chapters not yet written)

Review feedback asked for these specific internals to be covered "where they add value." They belong in chapters that don't exist yet — don't try to shoehorn them into Chapters 000-007:

- **Card table, write barriers, ephemeral GC segments** — belong in [Episode 11 — GC Generations & LOH](chapters/011-gc-generations-loh/) (chapter not yet written)
- **Deeper GC segment/generation mechanics** beyond what Episode 8 (Object Allocation) already forward-references — same chapter
- **Boxing cost benchmarks (real numbers, not just claims)** — belongs in [Episode 9 — Boxing & Unboxing](chapters/008-boxing-unboxing/) (chapter not yet written)
- **LINQ vs. loops, string concatenation approaches** — no dedicated chapter currently scheduled for either; consider whether these belong as a Performance Notes addition to an existing future chapter (e.g. LINQ execution, Episode 39) or need their own slot. Flag for a roadmap discussion, don't just insert them ad hoc.

## Additional chapter candidates raised in review (2026-08-07)

The reviewer suggested these topics "before ASP.NET Core." Cross-checked against the current 59-chapter roadmap ([ROADMAP.md](ROADMAP.md), [book.json](book.json)) — most already have a home; a few genuinely don't:

| Topic | Status |
|---|---|
| Metadata | Covered inside Episode 5 (004-assemblies-metadata), not standalone |
| Reflection | Already scheduled standalone — Episode 18 (017-reflection-expression-trees) |
| AssemblyLoadContext | Covered inside Episodes 3 & 5 (002-clr, 004-assemblies-metadata), not standalone |
| Type Loader | Covered inside Episode 3 (002-clr), not standalone |
| Thread Pool | Already scheduled standalone — Episode 43 (042-thread-pool) |
| Async/Await internals | Already scheduled standalone — Episode 44 (043-async-await) |
| SynchronizationContext | Already scheduled standalone — Episode 45 (044-synchronization-context) |
| `Span<T>` / `Memory<T>` | Partially covered (Episodes 6-7); no standalone chapter |
| `ref struct` | Partially covered (Episode 7); no standalone chapter |
| `Unsafe` | **Not covered anywhere in the current roadmap** — genuine gap |
| Native AOT | Covered inside Episode 4 (003-jit-compilation), not standalone |
| PGO | Covered inside Episode 4 (003-jit-compilation), not standalone |
| **Exception Handling** | **Not covered as a standalone chapter** — touched briefly in Episode 3; genuine gap worth a roadmap discussion |

**Action when revisiting the roadmap:** `Unsafe` and standalone `Exception Handling` are the two real gaps. Everything else already has a home — don't re-add it as a new chapter, that would duplicate content. Decide deliberately whether `Unsafe` and `Exception Handling` deserve their own chapters or stay as sections within existing ones before touching chapter numbering again (renumbering is cheap for empty stub folders, expensive once they have content).

## Known content bugs fixed in passing (2026-08-07)

Found while doing the Related-Chapters cross-linking pass — noting here so the pattern (verify chapter-number cross-references against the actual roadmap before trusting them) is remembered:
- Chapter 001 referenced a phantom "Episode 9 — Native AOT" chapter that doesn't exist (Native AOT is covered inside Episode 4/chapter 003) — fixed.
- Chapter 006 linked boxing's "deep dive" to Episode 8 (chapter 007, Object Allocation) instead of Episode 9 (chapter 008, Boxing & Unboxing) — fixed.

If a future review or a fresh session adds more cross-references, double-check the target chapter number against `book.json`'s actual chapter list, not against memory of what the number "should" be.

## Deferred from the "Inside .NET Companion" idea (2026-08-06)

Labs / Projects / Exercises / Architecture Challenges / Interview Challenges as a dedicated section. Decision already made for *when* this gets built: new top-level `companion/` folder in this same repo, not a separate repo. Not started — no chapters have been retrofitted with cross-links into it, because it doesn't exist yet.

## Deferred from the "visual journey map" idea (2026-08-06)

A lit-up progress path across all 59 chapters. Decision already made for *when* this gets built: static SVG/Mermaid embedded in `ROADMAP.md` and `README.md`, not an interactive artifact. Not started.
