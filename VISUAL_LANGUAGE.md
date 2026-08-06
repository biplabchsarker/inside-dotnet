# Visual Language

Every image in this project must look like it belongs in the same book — chapter 1 or chapter 58. The mechanism that guarantees this is a **reusable component library**, not individual artistic judgment applied fresh each time. Think LEGO bricks: a fixed set of pre-designed pieces that every chapter composes into its own diagram, rather than everyone drawing their own version of "a server."

## Why a component library, not one-off illustrations

- **Consistency for free.** If "CLR" always looks like the same icon, readers build a visual vocabulary across chapters instead of re-learning symbols every time.
- **Speed.** Composing a diagram from existing pieces is faster than designing one from scratch, and it scales to 58 chapters.
- **Maintainability.** Fix or restyle one component once, and every diagram that uses it benefits — instead of hunting through 50 chapters' worth of bespoke art.

## Component catalog

Each entry below is a reusable icon/symbol under `assets/components/` — ✅ built, or ⏳ catalogued but not yet drawn (drawn on demand, the first time a chapter actually needs it — see [Governance](#governance)). This catalog is deliberately grown incrementally rather than front-loaded to ~100 speculative icons: build the ones the roadmap's next several chapters need, add more as work reaches them.

| Category | Components |
|---|---|
| Runtime | ✅ CLR, ✅ GC, ⏳ JIT, ⏳ Roslyn, ⏳ IL, ⏳ Assembly, ⏳ Metadata/Manifest, ⏳ Type Loader, ⏳ CTS, ⏳ CLS, ⏳ AssemblyLoadContext, ⏳ Exception Handler, ⏳ Security |
| Memory | ✅ Stack, ✅ Heap, ⏳ Object, ⏳ Boxed Value, ⏳ LOH, ⏳ Pinned Object, ⏳ Finalizer, ⏳ Weak Reference |
| Concurrency | ✅ Thread, ✅ CPU Core, ⏳ Thread Pool, ⏳ Task, ⏳ Await, ⏳ Cancellation Token, ⏳ Synchronization Context |
| C# language | ⏳ Delegate, ⏳ Event, ⏳ Generic, ⏳ Reflection, ⏳ Expression Tree, ⏳ Record, ⏳ Pattern Match |
| Patterns & DI | ⏳ Singleton, ⏳ Factory, ⏳ Builder, ⏳ Strategy, ⏳ Observer, ⏳ Adapter, ⏳ Decorator, ⏳ Facade, ⏳ Repository, ⏳ Unit of Work, ⏳ DI Container, ⏳ Service Provider, ⏳ Options Pattern |
| ASP.NET Core / EF Core | ⏳ Middleware, ⏳ Controller, ⏳ Minimal API, ⏳ Kestrel, ⏳ DbContext, ⏳ LINQ, ⏳ Change Tracker, ⏳ Migration |
| Infrastructure | ✅ Server, ✅ Database, ⏳ Cache, ⏳ Redis, ⏳ Message Queue, ⏳ RabbitMQ, ⏳ Load Balancer, ⏳ SQL Server, ⏳ Postgres, ⏳ MongoDB |
| Cloud | ✅ Cloud (generic), ✅ Docker, ⏳ Azure, ⏳ Kubernetes, ⏳ Container, ⏳ Microservice, ⏳ OpenTelemetry |
| Data flow | ⏳ Request, ⏳ Response, ⏳ Pipeline stage, ⏳ Arrow (sync), ⏳ Arrow (async, dashed) |
| Status | ⏳ Success (check), ⏳ Warning (triangle), ⏳ Error (x), ⏳ Info (circle-i) |

Roughly 100 entries at full maturity across the whole 58-chapter roadmap — see [`assets/components/README.md`](assets/components/README.md) for the live built/pending list, kept in sync with this table.

## Diagram templates

`assets/templates/` holds reusable *starting points* — parameterized SVG skeletons (with `<!-- EDIT -->`-marked regions, same convention as `assets/brand/chapter-cover-template.svg`) for the recurring diagram *shapes* every chapter reaches for, so authoring a new chapter's illustrations means editing a template, not designing from a blank canvas:

| Template | Use it for |
|---|---|
| `hero-illustration.svg` | Base frame for a chapter's Hero Cover — blueprint grid + radial glow + central-motif slot (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md#the-four-signature-illustrations--every-chapter)) |
| `cheat-sheet.svg` | Base frame for a chapter's Cheat Sheet — one-line rule + bullet list + one interview question, dense and scannable |
| `timeline.svg` | Sequential/chronological flows (execution timelines, GC generations over time) |
| `architecture.svg` | Boxes-and-connections system diagrams |
| `flowchart.svg` | Decision/branching logic |
| `comparison.svg` | Side-by-side two-option comparisons (stack vs heap, value vs reference) |
| `memory-layout.svg` | Byte/field-level memory diagrams |
| `lifecycle.svg` | State-machine-shaped lifecycles (DI lifetimes, DbContext lifecycle) |
| `decision-tree.svg` | "When should I use X vs Y" branching guidance |
| `checklist.svg` | Definition-of-done / best-practices visual checklists |
| `interview-summary.svg` | End-of-chapter interview-question visual recap |
| `performance-comparison.svg` | Benchmark/timing bar comparisons |

Grown the same way as the component catalog: build the templates the next few chapters actually need first (this batch ships the first several), add the rest incrementally.

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
