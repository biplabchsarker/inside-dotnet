# Image Guide

All images in this project are original. No stock images, no images scraped from the internet, no AI-generated "art" masquerading as illustration — every diagram is purpose-built for Inside .NET.

## The five signature illustrations — every chapter (v2, 2026-08-08)

Beyond the in-article Mermaid diagrams (which stay — they're fast, versionable, and correct), every chapter earns **five signature illustrations** that carry the "stop scrolling" weight documentation diagrams can't. As of 2026-08-08 these are **externally sourced** (commissioned illustrator, Canva/Figma, or an AI image-generation tool) rather than hand-coded SVG — see "Why externally sourced" below. Every chapter's exact content brief (what each image should actually depict) lives in [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md); this section defines the reusable *type* and *style*, not per-chapter content.

1. **Hero Cover** (`images/NNN-hero.png`) — the chapter's opening visual. A central motif (the chapter's core concept, rendered as a clean isometric/3D object or mechanism) with 2-4 supporting elements flowing toward or around it. **No code, no screenshots, no UI mockups, minimal text — pure storytelling about the concept.** Distinct from `chapter-cover-template.svg` (the Part label + episode number + title navigational cover, which stays hand-coded SVG) — the Hero Cover is the illustrative companion that runs alongside it at the top of `article.md`.
2. **Concept Overview** (`diagrams/png/NNN-concept.png`) — one clean image explaining the chapter's central idea end-to-end (e.g. Source → Compiler → IL → CLR → JIT → CPU, or a before/after transformation). Generous spacing, minimal text. This is the image a reader screenshots and shares.
3. **Runtime/Internal View** (`diagrams/png/NNN-internal.png`) — a clean breakdown of the chapter's internal structure (e.g. CLR's subsystems as a labeled cutaway). Denser than the Concept Overview — for the reader who wants the mechanism, not the elevator pitch. (Renamed from "Deep-Dive Illustration" — same role.)
4. **Memory/Execution Diagram** (`diagrams/png/NNN-memory.png`) — new in v2. Visualizes data flow, memory state, or execution order concretely (e.g. stack/heap contents before vs. after an operation, a call stack unwinding, a timeline of what runs when). This is the image that answers "show me, don't just tell me" for whatever the chapter's central mechanism actually does to memory or control flow.
5. **Performance & Quick Reference** (`diagrams/png/NNN-performance.png`) — new in v2, merges the old "Cheat Sheet" and a dedicated performance-comparison image into one: real measured numbers (never fabricated — pull from the chapter's actual `BenchmarkDotNet`/`dotnet-counters` data) shown as clean horizontal bars or a small table, alongside the one-line rule and the single most likely interview question. This is the image a reader saves for review before an interview.

All five are referenced inline in `article.md`: Hero Cover near the top (Learning Objectives area), Concept Overview in **Visual Explanation**, Runtime/Internal View and Memory/Execution Diagram both in **Under the Hood**, Performance & Quick Reference in **Performance Notes** (and reused at **Summary & Next Chapter** in place of the old standalone Cheat Sheet).

### Why externally sourced (v2 change)

Two rounds of review feedback asked for progressively more polished illustrations. A hand-coded SVG technique (flat isometric cubes, gradient shading, blur-based soft shadows — prototyped in `chapters/008-boxing-unboxing/diagrams/svg/`) closed some of the gap, but the project owner's review of that prototype ("close, needs iteration") plus the decision to source images externally going forward means: **new chapters should not wait on further SVG iteration.** Use [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) as the spec handed to whichever external source produces the final PNGs — commissioned illustrator, Canva, Figma, or an AI image-generation tool. See [BACKLOG.md](BACKLOG.md) for the full decision history.

Existing hand-coded SVGs for Chapters 000-017 remain in place as interim placeholders until externally-sourced replacements land — they are not being iterated on further as SVG.

## "Microsoft Learn quality" — what that means in practice

This is not a request to copy Microsoft's visual identity — it's a request to match its *level of craft*. Concretely:

- **Soft gradients** — 2-stop linear or radial gradients from the palette (see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)), used for depth, never as decoration alone.
- **Subtle shadows / glow** — an SVG `<filter>` with `feGaussianBlur` behind a hero illustration's central motif, at low opacity, to make it read as the focal point without becoming garish.
- **Consistent spacing** — 8px base unit throughout (per DESIGN_SYSTEM.md), applied to hero illustrations too, not just diagrams.
- **Consistent typography** — the same heading/body font stack everywhere, same weight for the same semantic role across every chapter.
- **Delivered as PNG, brief-driven** — the five signature illustrations are commissioned/generated externally against [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) and delivered as PNG directly; there is no SVG source to maintain for these five specifically (in-article Mermaid diagrams and any one-off technical diagrams still follow the SVG-first rule below).

**Honest ceiling this replaces:** hand-coded SVG (this project's original toolset) reaches "engineering blueprint / isometric vector / modern infographic" quality — soft gradients, glow filters, blueprint grids, consistent iconography — but not photoreal, painterly, or true 3D-rendered illustration. A light-isometric SVG technique (prototyped in `chapters/008-boxing-unboxing/diagrams/svg/`) closed part of that gap but was judged "close, needs iteration" — see [BACKLOG.md](BACKLOG.md) for the full history. The five signature illustrations now go external instead of chasing further SVG iteration; hand-coded SVG remains the right tool for in-article Mermaid/technical diagrams and any one-off illustration where external sourcing isn't warranted.

## Visual style

- **Background:** light — white or near-white (`#FFFFFF`/`#F8FAFC`) with subtle purple/blue gradient washes, per the Chapter 8 hero prototype. (This replaces the earlier dark-blueprint-grid hero convention used in Chapters 000-007 — see [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) for the full style brief handed to external sources.)
- **Palette:** .NET blue + purple accents — see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) for exact values
- **Form:** clean, isometric/pseudo-3D shapes with soft shadows and generous whitespace — one concept per image, minimal text, self-explanatory without reading the surrounding article
- **Consistency:** every illustration should look like it belongs in the same book — same palette, same shadow/gradient treatment, same label typography, same icon style (see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) for the reusable flat-icon component library, still used for in-article diagrams)

## Format rules

- **All five signature illustrations:** 1600×900 (16:9), PNG, one consistent aspect ratio across all five so any external source (illustrator, Canva, Figma, AI image-gen) works from one template — see [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) for the exact brief per chapter.
- **In-article technical diagrams:** authored as Mermaid first (fast, versionable, renders natively on GitHub) whenever Mermaid's shapes are expressive enough
- **Beyond Mermaid's range** (detailed architecture diagrams, anything needing precise custom shapes not worth an external commission): author as hand-coded SVG, or Draw.io (`.drawio`), or PlantUML (`.puml`) for sequence/class diagrams
- **SVG stays the source format for anything hand-coded** — always keep the editable source, never ship a PNG without its source alongside it, for anything that isn't one of the five externally-sourced illustrations

## Folder structure (per chapter)

```
diagrams/
├── mermaid/    .mmd source files, one per in-article technical diagram, numbered to match article.md order
├── drawio/     .drawio source files (editable in diagrams.net / VS Code draw.io extension)
├── plantuml/   .puml source files
├── svg/        hand-coded SVG only — NNN-cover.svg (navigational cover), plus any one-off technical diagrams. The five signature illustrations no longer live here once externally sourced (see below).
└── png/        PNG export of hand-coded SVGs in this folder, PLUS the externally-sourced NNN-concept.png / NNN-internal.png / NNN-memory.png / NNN-performance.png
```

## Naming

- `diagrams/mermaid/01-execution-flow.mmd`, etc. — numbered to match the order technical diagrams appear in `article.md`
- Navigational chapter cover (stays hand-coded SVG): `diagrams/svg/NNN-cover.svg` (source) → `images/NNN-cover.png` (export)
- Hero Cover (externally sourced): delivered directly as `images/NNN-hero.png` — no SVG source expected
- Concept Overview (externally sourced): delivered directly as `diagrams/png/NNN-concept.png`
- Runtime/Internal View (externally sourced): delivered directly as `diagrams/png/NNN-internal.png`
- Memory/Execution Diagram (externally sourced): delivered directly as `diagrams/png/NNN-memory.png`
- Performance & Quick Reference (externally sourced): delivered directly as `diagrams/png/NNN-performance.png`
- Component library assets live in `assets/components/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) — still used for in-article Mermaid/SVG diagrams
- Reusable diagram/illustration starting points live in `assets/templates/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md#diagram-templates)

## Export pipeline

`scripts/Svg2Png` is a small .NET tool (Svg.Skia) that rasterizes SVG → PNG, since this environment has no system SVG rasterizer. It's still used for the navigational chapter cover and any hand-coded technical diagrams:

```bash
cd scripts/Svg2Png
dotnet run -- ../../chapters/NNN-slug/diagrams/svg 2.0
```

See [`scripts/Svg2Png/README.md`](scripts/Svg2Png/README.md) for the full path-resolution convention. The five signature illustrations skip this pipeline entirely — they arrive as finished PNGs from whichever external source produced them, dropped straight into `images/` or `diagrams/png/` at the filenames above.

## Dark mode

Dark-mode variants are optional per diagram but encouraged for anything published to the website (`docs/`). When produced, suffix with `-dark`: `02-clr-architecture-dark.svg`.

## Checklist — every diagram

- [ ] Original artwork (not copied/adapted from any external source)
- [ ] Editable source committed (`.mmd`, `.drawio`, `.puml`, or `.svg` — never a PNG only)
- [ ] Final PNG export generated via `scripts/Svg2Png` and committed alongside the SVG
- [ ] Uses only the approved palette, typography, and stroke rules from [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)
- [ ] Reuses component-library icons from `assets/components/` where the concept already has one, instead of redrawing it
- [ ] Self-explanatory: a reader should get the gist from the diagram alone, before reading the surrounding paragraph

## Checklist — every chapter's five signature illustrations

- [ ] Content brief for this chapter exists in [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) before commissioning/generating anything
- [ ] Hero Cover exists, no code/screenshots/UI mockups, central motif + supporting elements, light background
- [ ] Concept Overview exists, end-to-end idea in one image, minimal text
- [ ] Runtime/Internal View exists, denser breakdown of internal structure
- [ ] Memory/Execution Diagram exists, shows a concrete before/after or step-by-step data/execution state
- [ ] Performance & Quick Reference exists: real measured numbers (never fabricated), one-line rule, one likely interview question
- [ ] All five referenced inline in `article.md` at the sections named in [The five signature illustrations](#the-five-signature-illustrations--every-chapter-v2-2026-08-08)
- [ ] All five are 1600×900 PNG, delivered by the external source, no placeholder text left in

## Interim SVG placeholders (Chapters 000-017)

Chapters 000-017 currently use hand-coded SVG for their signature illustrations (the four-illustration structure, dark-hero style for 000-007; the light-isometric technique for 008-017, five images each). These stay in place as placeholders — not deleted, not further iterated as SVG — until externally-sourced v2 replacements land per [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md). Treat any SVG-authoring instruction elsewhere in this doc as inapplicable to those placeholders going forward.

**2026-09-09 update:** the allowance was extended from "Chapters 000-008" to include 009 and 010 — those two chapters had shipped with only a navigational cover (no illustrations at all, unlike 000-008 which had *something* in every slot), and the project owner decided closing that specific gap with the same light-isometric technique was worth it rather than leaving two chapters fully bare while the external pipeline remains unbuilt.

**2026-09-20 update:** the same reasoning was applied again as Chapters 011 through 017 were drafted in sequence — each shipped with the same light-isometric technique across all five signature illustrations as a newly-drafted chapter (not a gap being closed retroactively), since the external sourcing pipeline still isn't wired up. This does not reopen SVG iteration as the long-term plan — see [BACKLOG.md](BACKLOG.md) for the decision history — it only means *future* chapters (018 onward) should default to waiting for external sourcing rather than assuming another SVG extension, unless the same "fully bare" condition recurs and gets a similar explicit decision.
