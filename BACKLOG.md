# Backlog

Deferred items from review feedback — things worth doing, deliberately not done yet, so they don't get lost. Check this before starting a new phase of work; move an item to [CHANGELOG.md](CHANGELOG.md) when it ships.

## Chapter 009's five illustrations not yet sourced (2026-08-13)

Chapter 009 (Strings & Interning) was written in full — all 13 `article.md` sections, both `code/` projects (`Chapter09.Demo`, `Chapter09.Benchmarks`) built and run for real numbers, all 5 diagrams, and every companion file — but no image-generation/commissioning tool was available in the session that wrote it, so its five signature illustrations don't exist yet, not even as an interim SVG placeholder (unlike Chapters 000-008, which all have *something* in these slots). Its content brief is ready in [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md#009--strings--interning) for whoever sources them.

**Status convention introduced by this gap:** `book.json`'s `status` map now uses `"drafted (text/code/diagrams complete, images pending external sourcing)"` for a chapter in this exact state, distinct from `"done"` (everything in [`WRITING_GUIDE.md`](WRITING_GUIDE.md)'s Definition of Done, images included) and `"planned"` (not started). Update Chapter 009's status to `"done"` once its five PNGs land in `images/`/`diagrams/png/` per the brief — don't leave it at `"drafted"` indefinitely, and don't reuse `"drafted"` for a chapter that's missing anything besides images.

## Illustration quality ceiling (2026-08-07, updated 2026-08-08)

Two rounds of review feedback asked for progressively more "cinematic / iconic / memorable" illustrations. The original hand-coded SVG toolset (dark blueprint-grid gradients, glow filters — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md)) hit diminishing returns within that specific technique.

**2026-08-08 update:** prototyped a second SVG technique for Chapter 008 — light background, isometric-projection cubes (layered flat polygons with per-face gradient shading to fake 3D depth), blur-based soft shadows, generous whitespace, minimal text. This is a genuine step up in polish (see `chapters/008-boxing-unboxing/diagrams/svg/008-hero.svg` and siblings) and directly answers a review request for a "Microsoft Learn cover, not a poster full of text." It is still hand-coded flat/isometric SVG, not true 3D rendering — no raytraced shadows, no painterly shading. That ceiling is unchanged from before; only the *technique within the ceiling* improved.

**Decided, 2026-08-08:**
1. **Style verdict: "close, needs iteration."** The isometric-cube prototype is a real improvement but doesn't clear the bar on its own — going forward, the five signature illustrations are **sourced externally** (commissioned illustrator, Canva, Figma, or an AI image-generation tool) rather than iterated further as hand-coded SVG.
2. **Image count: expand to 5** — Hero Cover, Concept Overview, Runtime/Internal View, Memory/Execution Diagram, Performance & Quick Reference (replaces the 4-illustration structure; Cheat Sheet's role — one-line rule, key bullets, interview question — merges into Performance & Quick Reference rather than disappearing). `WRITING_GUIDE.md`, `IMAGE_GUIDE.md` updated for all future chapters.
3. **Retrofit timing: not yet.** Chapters 000-007 keep their existing (dark-blueprint, 4-illustration) SVGs as interim placeholders. Chapter 008 keeps its light-isometric SVG prototype as an interim placeholder too, pending its own externally-sourced replacement. No chapter's actual image files are being redone right now — only the standard going forward.

**Next step:** [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) — the external-sourcing spec (style brief, 1600×900 PNG sizing, per-chapter content briefs for 000-008) — is ready to hand to whichever external source produces the actual images. Canva/Figma MCP connectors are available in this environment but need the project owner to authorize them via claude.ai connector settings first, if that's the chosen route.

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
