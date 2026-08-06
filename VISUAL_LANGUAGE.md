# Visual Language

Every image in this project must look like it belongs in the same book — chapter 1 or chapter 58. The mechanism that guarantees this is a **reusable component library**, not individual artistic judgment applied fresh each time. Think LEGO bricks: a fixed set of pre-designed pieces that every chapter composes into its own diagram, rather than everyone drawing their own version of "a server."

## Why a component library, not one-off illustrations

- **Consistency for free.** If "CLR" always looks like the same icon, readers build a visual vocabulary across chapters instead of re-learning symbols every time.
- **Speed.** Composing a diagram from existing pieces is faster than designing one from scratch, and it scales to 58 chapters.
- **Maintainability.** Fix or restyle one component once, and every diagram that uses it benefits — instead of hunting through 50 chapters' worth of bespoke art.

## Component catalog (v1)

Each entry below is a planned reusable icon/symbol, defined once under `assets/components/` and referenced from any chapter's diagrams. This catalog grows as new chapters need new concepts — add to it deliberately, don't let one-off icons proliferate outside it.

| Category | Components |
|---|---|
| Runtime | CLR, GC, JIT, Assembly, Metadata, Type Loader |
| Memory | Stack, Heap, Object, Boxed Value, LOH |
| Concurrency | Thread, Thread Pool, Task, CPU Core |
| Infrastructure | Server, Database, Cache, Message Queue, Load Balancer |
| Cloud | Cloud (generic), Azure, Docker, Kubernetes, Container |
| Data flow | Request, Response, Pipeline stage, Arrow (sync), Arrow (async, dashed) |
| Status | Success (check), Warning (triangle), Error (x), Info (circle-i) |

## Component format

- Authored as standalone SVG under `assets/components/<name>.svg`, using only the palette and stroke rules from [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md).
- Each component is a self-contained `<symbol>`-style SVG with a defined viewBox (default `0 0 64 64`) so it can be dropped into a larger diagram at any scale without redrawing.
- Named consistently: lowercase-kebab, matching the catalog table above (`clr.svg`, `gc.svg`, `thread-pool.svg`, `kubernetes.svg`).

## Using components in a chapter diagram

- **Mermaid diagrams** can't embed arbitrary SVG symbols directly — for chapters using Mermaid, reference the component visually by naming the node after the same catalog term and matching its color to that component's designated palette role, so the *concept* stays visually consistent even if the literal SVG isn't reusable inside Mermaid's renderer.
- **Hand-authored SVG diagrams** (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md)) should `<use>`-reference the component symbol directly from `assets/components/`, rather than redrawing the shape inline.

## Governance

- Before drawing a new icon for a concept, check the catalog above — if it exists, reuse it.
- If a chapter genuinely needs a new reusable concept (e.g. "Redis", "OpenTelemetry", "Health Check"), add it to the catalog table in this document *and* to `assets/components/` in the same change — don't let it live as a one-off inside a single chapter's `diagrams/svg/` folder.
- Component visuals are versioned like code: a breaking restyle of an existing component (e.g. changing what "CLR" looks like) is a `VISUAL_LANGUAGE` version bump, documented in [CHANGELOG.md](CHANGELOG.md), and ideally applied retroactively to chapters that already used the old version.
