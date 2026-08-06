# Architecture

How this repository itself is organized, how a chapter moves from idea to published, and what automation exists or is planned.

## Repository layout

```
inside-dotnet/
├── README.md                 project landing page, status table
├── PHILOSOPHY.md              why this project exists, core principles
├── ROADMAP.md                 phase-by-phase plan, chapter list, milestones
├── STYLE_GUIDE.md             tone, code conventions, naming, terminology
├── WRITING_GUIDE.md           chapter template + definition of done
├── IMAGE_GUIDE.md             diagram/image rules and folder conventions
├── DESIGN_SYSTEM.md           colors, typography, spacing, diagram rules
├── VISUAL_LANGUAGE.md         reusable component library concept + catalog
├── BRAND_GUIDE.md             logo/cover/banner usage
├── ARCHITECTURE.md            this file
├── PUBLISHING_GUIDE.md        one-markdown-to-many-platforms pipeline
├── CONTRIBUTING.md            how to propose/write a chapter
├── CHANGELOG.md               dated log of structural/foundation changes
├── BACKLOG.md                  deferred review feedback, future-chapter candidates, known gaps
├── LICENSE.md                 explains the dual-license split
├── LICENSE-CODE               MIT (applies to every code/ folder)
├── LICENSE-CONTENT            CC-BY-4.0 (applies to articles, diagrams, prose)
├── book.json                  machine-readable phase/chapter/status metadata
│
├── chapters/
│   ├── _template/              copy this to start a new chapter (mirrors WRITING_GUIDE.md)
│   └── NNN-topic-slug/         one folder per chapter — see WRITING_GUIDE.md for its internal layout
│
├── assets/
│   ├── brand/                  logo, hero cover, chapter-cover template, social banner
│   └── components/              reusable diagram icon library (see VISUAL_LANGUAGE.md)
│
├── linkedin/  medium/  devto/  website/  ebook/     per-platform compiled bundles (see PUBLISHING_GUIDE.md)
├── docs/                        optional static-site source (MkDocs/Docusaurus/Astro), for the eventual inside-dotnet.dev
├── slides/                      per-chapter presentation decks
└── scripts/                     automation (build.ps1 etc.) — see below
```

## Chapter lifecycle

1. **Plan** — the chapter's slot in `book.json`'s phase/chapter list is confirmed (topic, position, dependencies on prior chapters).
2. **Draft** — `article.md` is written to the full 13-section template ([WRITING_GUIDE.md](WRITING_GUIDE.md)), diagrams are authored, code is written and verified with `dotnet run`.
3. **Review** — technical accuracy and architectural framing are reviewed (see [CONTRIBUTING.md](CONTRIBUTING.md) working agreement).
4. **Refine** — corrections/insights are incorporated.
5. **Complete supporting files** — `linkedin.md`, `medium.md`, `devto.md`, `summary.md`, `faq.md`, `interview.md`, `quiz.md`, `exercises.md`, `references.md` are all filled in against `article.md` as source of truth.
6. **Status update** — `book.json`'s `status` map and the root `README.md` status table are both updated to `done`.
7. **Publish** — platform-specific variants are pushed out per [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md).
8. **Commit & push** — every completed chapter (or batch of chapters) is committed with a descriptive message and pushed to `main`.

A chapter never skips steps 2–6 partially — see the Definition of Done checklist in [WRITING_GUIDE.md](WRITING_GUIDE.md).

## Publishing pipeline (current vs. planned)

**Current (manual):** `article.md` is the source of truth; `linkedin.md`/`medium.md`/`devto.md` are hand-written adaptations, not auto-generated.

**Planned (`scripts/build.ps1`):** a single command that reads `article.md` plus chapter metadata and emits the LinkedIn, Medium, Dev.to, Hashnode, GitHub Pages, PDF, PowerPoint, and eBook variants automatically. This is explicitly a **later-phase automation goal** (see [ROADMAP.md](ROADMAP.md) milestone v6.0) — until then, platform variants are written by hand against the template conventions in [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md), so that when automation does arrive, existing variants already conform to the format it will expect.

## `book.json` as the machine-readable source of truth

`book.json` tracks:
- `phases[]` — the 12-phase structure (Phase 0 – Phase 11), each with its chapter slug list
- `status{}` — per-chapter status (`"planned"` | `"draft"` | `"done"`)
- Series metadata (title, author, license, repository URL, visual identity pointers)

Any tooling built later (site generators, automation scripts, progress dashboards) should read from `book.json` rather than re-deriving structure from folder names.

## Design decisions worth recording here

- **Diagrams-as-code first:** Mermaid is the default diagram format because it's versionable, diffable, and renders natively on GitHub — hand-authored SVG/Draw.io/PlantUML are used only where Mermaid's shape vocabulary is insufficient (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md)).
- **Dual licensing:** code and written content have different reuse expectations (see [LICENSE.md](LICENSE.md)) — this is a deliberate split, not an oversight.
- **.NET 10 target:** chapters target the current LTS installed in the working environment; earlier planning referenced .NET 9, which is superseded (see [STYLE_GUIDE.md](STYLE_GUIDE.md)).
- **Chapter numbering is phase-ordered, not creation-ordered:** chapter slugs (`NNN-slug`) reflect final reading order per [ROADMAP.md](ROADMAP.md), which may not match the order chapters were drafted in — stub folders get renumbered as the roadmap solidifies, before they contain real content (renumbering a written chapter is disruptive and should be avoided once `status` is `done`).
