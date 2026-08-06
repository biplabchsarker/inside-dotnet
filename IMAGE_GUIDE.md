# Image Guide

All images in this project are original. No stock images, no images scraped from the internet, no AI-generated imagery masquerading as illustration — every diagram is purpose-built for Inside .NET.

## Visual style

- **Background:** clean white, with subtle gradients where they add depth (never for decoration alone)
- **Palette:** .NET blue + purple accents — see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) for exact values
- **Form:** clean vector / isometric technical diagrams — minimal text, self-explanatory without reading the surrounding article
- **Consistency:** every diagram should look like it belongs in the same book — same stroke widths, same corner radii, same label typography, same icon style (see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) for the reusable component library)

## Format rules

- **Chapter covers:** 16:9 aspect ratio, SVG source + PNG export
- **In-article diagrams:** authored as Mermaid first (fast, versionable, renders natively on GitHub) whenever Mermaid's shapes are expressive enough
- **Beyond Mermaid's range** (detailed architecture diagrams, isometric illustrations, anything needing precise custom shapes): author as hand-coded SVG, or Draw.io (`.drawio`) if a visual editor is more practical, or PlantUML (`.puml`) for sequence/class diagrams where its layout engine fits better than Mermaid's
- **SVG is the preferred final source format** — always keep the editable source, never ship a PNG without its source alongside it

## Folder structure (per chapter)

```
diagrams/
├── mermaid/    .mmd source files, one per diagram, numbered to match article.md order
├── drawio/     .drawio source files (editable in diagrams.net / VS Code draw.io extension)
├── plantuml/   .puml source files
├── svg/        final, styled SVG — this is what gets embedded/exported
└── png/        PNG export of every SVG, generated at 2x resolution for retina displays
```

## Naming

- `diagrams/mermaid/01-execution-flow.mmd`, `diagrams/svg/02-clr-architecture.svg`, etc. — numbered to match the order diagrams appear in `article.md`
- Chapter covers: `images/NNN-cover.svg` (source) and `images/NNN-cover.png` (export)
- Component library assets (reusable icons — server, database, CLR, GC, JIT, memory, stack, heap, thread, CPU, cloud, Azure, Docker, Kubernetes, etc.) live in `assets/components/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) — and are referenced/composed into chapter diagrams rather than redrawn per chapter

## Dark mode

Dark-mode variants are optional per diagram but encouraged for anything published to the website (`docs/`). When produced, suffix with `-dark`: `02-clr-architecture-dark.svg`.

## Checklist — every diagram

- [ ] Original artwork (not copied/adapted from any external source)
- [ ] Editable source committed (`.mmd`, `.drawio`, or `.puml` — never SVG/PNG only)
- [ ] Final PNG export committed alongside the SVG
- [ ] Uses only the approved palette, typography, and stroke rules from [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)
- [ ] Reuses component-library icons from `assets/components/` where the concept already has one, instead of redrawing it
- [ ] Self-explanatory: a reader should get the gist from the diagram alone, before reading the surrounding paragraph
