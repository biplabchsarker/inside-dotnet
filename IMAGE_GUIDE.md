# Image Guide

All images in this project are original. No stock images, no images scraped from the internet, no AI-generated "art" masquerading as illustration — every diagram is purpose-built for Inside .NET.

## The four signature illustrations — every chapter

Beyond the in-article Mermaid diagrams (which stay — they're fast, versionable, and correct), every chapter earns **four signature illustrations** that carry the "stop scrolling" weight documentation diagrams can't:

1. **Hero Cover** (16:9, `diagrams/svg/NNN-hero.svg` → `images/NNN-hero.png`) — the chapter's opening visual. Cinematic *in composition*, not in rendering technique: a central motif (the chapter's core concept, rendered as a glowing/blueprint engine, container, or mechanism), supporting elements flowing toward or around it, on a blueprint-grid or soft-gradient background. **No code, no screenshots, no UI mockups — pure storytelling about the concept.** This is distinct from the existing `chapter-cover-template.svg` (Part label + episode number + title, used for the series-wide navigational cover) — the Hero Cover is the illustrative companion that runs alongside or instead of it at the top of `article.md`.
2. **Concept Illustration** (`diagrams/svg/NNN-concept.svg`) — one clean image explaining the chapter's central idea end-to-end (e.g. Source → Compiler → IL → CLR → JIT → CPU). Beautiful icons from the [component library](VISUAL_LANGUAGE.md), generous spacing, minimal text. This is the image a reader screenshots and shares.
3. **Deep-Dive Illustration** (`diagrams/svg/NNN-deepdive.svg`) — an engineering-blueprint-style breakdown of the chapter's internal structure (e.g. CLR's subsystems as a labeled cutaway). Denser than the Concept Illustration — this is for the reader who wants the detail, not the elevator pitch.
4. **Cheat Sheet** (`diagrams/svg/NNN-cheatsheet.svg` → `diagrams/png/NNN-cheatsheet.png`) — a one-page, skimmable summary of the chapter's key facts: the core rule stated in one line, 3–5 "remember this" bullets, and the one interview question most likely to come up. This is the image a reader saves for review before an interview, not the one they read the chapter through — dense but scannable, not prose.

All four live under `diagrams/svg/`, get exported to `diagrams/png/` (except the Hero Cover's PNG, which — like the navigational chapter cover — lives in `images/`, since both are "the chapter's face"), and are referenced inline in `article.md`: Hero Cover near the top (Learning Objectives area), Concept Illustration in **Visual Explanation**, Deep-Dive Illustration in **Under the Hood**, Cheat Sheet in **Summary & Next Chapter**.

## "Microsoft Learn quality" — what that means in practice

This is not a request to copy Microsoft's visual identity — it's a request to match its *level of craft*. Concretely:

- **Soft gradients** — 2-stop linear or radial gradients from the palette (see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)), used for depth, never as decoration alone.
- **Subtle shadows / glow** — an SVG `<filter>` with `feGaussianBlur` behind a hero illustration's central motif, at low opacity, to make it read as the focal point without becoming garish.
- **Consistent spacing** — 8px base unit throughout (per DESIGN_SYSTEM.md), applied to hero illustrations too, not just diagrams.
- **Consistent typography** — the same heading/body font stack everywhere, same weight for the same semantic role across every chapter.
- **SVG first, PNG automatic** — every illustration is authored as SVG, then rasterized with `scripts/Svg2Png` (see below) — never a PNG without its source alongside it.

**Honest ceiling (still true, revisit before asking for more "cinematic"):** hand-coded SVG (this project's actual toolset) reaches "engineering blueprint / isometric vector / modern infographic" quality convincingly — soft gradients, glow filters, blueprint grids, consistent iconography. It does not reach photoreal, painterly, or literally-animated illustration — that requires a commissioned illustrator or an AI image-generation tool, neither of which is in scope here (see [BRAND_GUIDE.md](BRAND_GUIDE.md) for the same caveat applied to brand assets). Repeated requests to make illustrations "more cinematic/iconic/memorable" within this same toolset hit diminishing returns once the gradient/glow/grid/iconography treatment is already applied consistently — the next real jump in visual quality requires a different tool, not another SVG iteration. This is tracked as a deliberate backlog item in [BACKLOG.md](BACKLOG.md), not something to keep re-attempting with the same hand-coded approach.

## Visual style

- **Background:** clean white or a deep blueprint gradient (see DESIGN_SYSTEM.md), with subtle grid lines for hero/deep-dive illustrations where they reinforce the "engineering blueprint" register
- **Palette:** .NET blue + purple accents — see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) for exact values
- **Form:** clean vector / isometric technical diagrams — minimal text, self-explanatory without reading the surrounding article
- **Consistency:** every diagram should look like it belongs in the same book — same stroke widths, same corner radii, same label typography, same icon style (see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) for the reusable component library)

## Format rules

- **Chapter covers (navigational) and Hero Covers (illustrative):** 16:9 aspect ratio, SVG source + PNG export
- **In-article technical diagrams:** authored as Mermaid first (fast, versionable, renders natively on GitHub) whenever Mermaid's shapes are expressive enough
- **Beyond Mermaid's range** (Hero/Concept/Deep-Dive illustrations, detailed architecture diagrams, anything needing precise custom shapes, gradients, or glow): author as hand-coded SVG, or Draw.io (`.drawio`) if a visual editor is more practical, or PlantUML (`.puml`) for sequence/class diagrams where its layout engine fits better than Mermaid's
- **SVG is the preferred final source format** — always keep the editable source, never ship a PNG without its source alongside it

## Folder structure (per chapter)

```
diagrams/
├── mermaid/    .mmd source files, one per in-article technical diagram, numbered to match article.md order
├── drawio/     .drawio source files (editable in diagrams.net / VS Code draw.io extension)
├── plantuml/   .puml source files
├── svg/        final, styled SVG — NNN-cover.svg (navigational), NNN-hero.svg, NNN-concept.svg, NNN-deepdive.svg, NNN-cheatsheet.svg, plus any other one-off illustrations
└── png/        PNG export of every SVG in this folder except NNN-cover.svg / NNN-hero.svg (those export to ../images/ — see Naming below)
```

## Naming

- `diagrams/mermaid/01-execution-flow.mmd`, etc. — numbered to match the order technical diagrams appear in `article.md`
- Navigational chapter cover: `diagrams/svg/NNN-cover.svg` (source) → `images/NNN-cover.png` (export)
- Hero Cover: `diagrams/svg/NNN-hero.svg` (source) → `images/NNN-hero.png` (export)
- Concept Illustration: `diagrams/svg/NNN-concept.svg` → `diagrams/png/NNN-concept.png`
- Deep-Dive Illustration: `diagrams/svg/NNN-deepdive.svg` → `diagrams/png/NNN-deepdive.png`
- Cheat Sheet: `diagrams/svg/NNN-cheatsheet.svg` → `diagrams/png/NNN-cheatsheet.png`
- Component library assets live in `assets/components/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) — and are referenced/composed into chapter illustrations rather than redrawn per chapter
- Reusable diagram/illustration starting points live in `assets/templates/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md#diagram-templates)

## Export pipeline

`scripts/Svg2Png` is a small .NET tool (Svg.Skia) that rasterizes SVG → PNG, since this environment has no system SVG rasterizer. Run it after authoring or editing any SVG:

```bash
cd scripts/Svg2Png
dotnet run -- ../../chapters/NNN-slug/diagrams/svg 2.0
```

See [`scripts/Svg2Png/README.md`](scripts/Svg2Png/README.md) for the full path-resolution convention and more examples. This is a manual step today (run it before considering a chapter's images done); full `article.md`→every-platform automation is tracked separately in [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md).

## Dark mode

Dark-mode variants are optional per diagram but encouraged for anything published to the website (`docs/`). When produced, suffix with `-dark`: `02-clr-architecture-dark.svg`.

## Checklist — every diagram

- [ ] Original artwork (not copied/adapted from any external source)
- [ ] Editable source committed (`.mmd`, `.drawio`, `.puml`, or `.svg` — never a PNG only)
- [ ] Final PNG export generated via `scripts/Svg2Png` and committed alongside the SVG
- [ ] Uses only the approved palette, typography, and stroke rules from [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)
- [ ] Reuses component-library icons from `assets/components/` where the concept already has one, instead of redrawing it
- [ ] Self-explanatory: a reader should get the gist from the diagram alone, before reading the surrounding paragraph

## Checklist — every chapter's four signature illustrations

- [ ] Hero Cover exists, no code/screenshots/UI mockups, central motif + supporting elements + blueprint/gradient background
- [ ] Concept Illustration exists, end-to-end idea in one image, component-library icons, minimal text
- [ ] Deep-Dive Illustration exists, denser engineering-blueprint breakdown of internal structure
- [ ] Cheat Sheet exists: one-line core rule, 3–5 "remember this" bullets, one likely interview question
- [ ] All four referenced inline in `article.md` at the sections named in [The four signature illustrations](#the-four-signature-illustrations--every-chapter)
- [ ] All four exported to PNG via `scripts/Svg2Png`
