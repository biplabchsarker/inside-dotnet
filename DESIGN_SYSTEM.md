# Design System

The visual rules every diagram, cover, and banner in this project must follow. If `IMAGE_GUIDE.md` says "consistent branding," this document is what "consistent" means, precisely.

## Color palette

| Role | Name | Hex | Usage |
|---|---|---|---|
| Primary | .NET Purple | `#512BD4` | Primary brand color, headers, key nodes in diagrams |
| Primary Dark | Deep Purple | `#3B1E94` | Hover/emphasis states, dark-mode primary |
| Secondary | Azure Blue | `#0078D4` | Secondary accent, "runtime/infrastructure" concepts (CLR, cloud, network) |
| Secondary Dark | Deep Blue | `#005A9E` | Emphasis/dark-mode secondary |
| Accent | Violet | `#68217A` | Tertiary accent, used sparingly for contrast nodes |
| Success / Correct | Green | `#107C10` | "Do this" callouts, correct-pattern highlights |
| Warning / Mistake | Amber | `#D83B01` | Common-mistake callouts, anti-pattern highlights |
| Neutral Ink | Slate | `#1B1B1F` | Body text, diagram labels |
| Neutral Mid | Gray | `#6E6E76` | Secondary labels, captions |
| Neutral Line | Light Gray | `#D8D8DE` | Diagram borders, dividers |
| Background | White | `#FFFFFF` | Canvas background (light mode) |
| Background Dark | Near-black | `#0F0F13` | Canvas background (dark mode) |

Gradients are built from adjacent palette entries only (e.g. `#512BD4 → #0078D4`), at a maximum of 2 stops, applied to large fills (covers, banners) — never on small diagram nodes or text.

## Typography

| Role | Font stack | Weight |
|---|---|---|
| Headings | `Segoe UI, Inter, sans-serif` | 600–700 |
| Body | `Segoe UI, Inter, sans-serif` | 400 |
| Code / diagram labels | `Cascadia Code, Fira Code, monospace` | 400–500 |

- Diagram label size: minimum 14px equivalent at final render size — never so small it survives only at full zoom.
- Chapter covers use the heading font at large scale for the episode number + title, body font for the subtitle.

## Spacing

- Base unit: **8px**. All diagram padding, margins, and node spacing are multiples of 8 (8, 16, 24, 32, 48, 64).
- Diagram nodes: minimum 16px internal padding around label text.
- Chapter covers: 64px safe margin from any edge for text/logo elements.

## Iconography

- Line-icon style, 2px stroke weight at 1x scale, rounded joins, no fill except for solid "state" indicators (e.g. a filled dot for "active" vs. outline for "idle").
- Reusable component icons (Server, Database, CLR, GC, JIT, Memory, Stack, Heap, Thread, CPU, Cloud, Azure, Docker, Kubernetes, etc.) are defined once in `assets/components/` — see [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) — and reused everywhere rather than redrawn per chapter.

## Shadows & depth

- Flat design by default — no drop shadows on diagram nodes.
- Isometric illustrations (chapter covers, hero banners) may use a single soft shadow (`0 8px 24px rgba(81, 43, 212, 0.12)`) to lift the primary subject off the background — nowhere else.

## Diagram rules

- **Flowcharts / architecture diagrams:** Mermaid `flowchart`, left-to-right or top-to-bottom, node fills alternate between Primary and Secondary at 10% opacity tints, borders at full palette strength.
- **Sequence diagrams:** Mermaid `sequenceDiagram`, participants ordered left-to-right in the order they're introduced in the article's prose.
- **Timelines:** Mermaid `gantt`, always labeled "illustrative, not to scale" unless the timing is measured and reproducible.
- **Isometric illustrations:** reserved for chapter covers and Part-opener banners — never for in-article technical diagrams, where precision matters more than visual flair.
- Every diagram gets a light-mode version; dark-mode is optional but, when produced, must reuse the same palette's dark-mode row (see table above) rather than a simple color invert.

## Applying this system

- Mermaid diagrams: set `%%{init: {'theme':'base', 'themeVariables': {...}}}%%` using the hex values above where Mermaid's theming supports it; otherwise style via subgraph/node classes.
- SVG diagrams: define the palette as a `<style>` block of named classes (`.primary`, `.secondary`, `.accent`, `.warning`, etc.) at the top of the file so all diagrams share literal, copy-pasteable class names.
