<p align="center"><img src="assets/brand/logo.svg" alt="Inside .NET" width="420"/></p>

# Inside .NET
### Understanding What Really Happens Under the Hood
*A Visual Journey from Developer to Solution Architect — by Biplab Sarker*

**Repo:** https://github.com/biplabchsarker/inside-dotnet
**Status:** 🚧 Foundation Phase (v1.0)
**License:** Dual — [MIT](LICENSE-CODE) for code, [CC BY 4.0](LICENSE-CONTENT) for written content ([details](LICENSE.md))

Most developers know how to write code. Fewer understand what the .NET runtime is actually doing behind the scenes. **Inside .NET** is a chapter-by-chapter series that bridges that gap using original diagrams, real-world analogies, internal runtime explanations, practical C# examples, performance notes, common mistakes, and interview insights.

Every chapter builds on the previous one, taking readers from developer fundamentals to solution architecture. Our benchmark isn't other GitHub repos — it's Microsoft Learn, the .NET Runtime docs, Martin Fowler, Microsoft Press, O'Reilly, Manning, and JetBrains Guides, with stronger visuals and a more deliberate learning journey. See [ROADMAP.md](ROADMAP.md) for the full vision.

## Who this is for
Junior & mid-level developers · Senior engineers · Software architects · Technical leads · Engineering managers · Students preparing for interviews.

## Project documentation

| Doc | Covers |
|---|---|
| [PHILOSOPHY.md](PHILOSOPHY.md) | Why this project exists, core principles |
| [ROADMAP.md](ROADMAP.md) | Phase-by-phase plan, milestones, chapter list |
| [STYLE_GUIDE.md](STYLE_GUIDE.md) | Tone, code conventions, naming, terminology |
| [WRITING_GUIDE.md](WRITING_GUIDE.md) | Chapter template + definition of done |
| [IMAGE_GUIDE.md](IMAGE_GUIDE.md) | Diagram/image rules and folder conventions |
| [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) | Colors, typography, spacing, diagram rules |
| [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md) | Reusable component icon library |
| [BRAND_GUIDE.md](BRAND_GUIDE.md) | Logo, cover, and banner usage |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Repo layout, chapter lifecycle, automation |
| [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md) | One markdown → LinkedIn/Medium/Dev.to/PDF/etc. |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Working agreement, responsibilities, quality gate |
| [CHANGELOG.md](CHANGELOG.md) | Dated log of structural/foundation changes |
| [LICENSE.md](LICENSE.md) | Dual license explained |

## How this repo is organized

```
inside-dotnet
├── README.md, PHILOSOPHY.md, ROADMAP.md, STYLE_GUIDE.md, WRITING_GUIDE.md,
│   IMAGE_GUIDE.md, DESIGN_SYSTEM.md, VISUAL_LANGUAGE.md, BRAND_GUIDE.md,
│   ARCHITECTURE.md, PUBLISHING_GUIDE.md, CONTRIBUTING.md, CHANGELOG.md,
│   LICENSE.md, LICENSE-CODE, LICENSE-CONTENT      governance docs (see table above)
├── book.json                                        machine-readable phase/chapter/status metadata
│
├── chapters/
│   ├── _template/                                    copy this to start a new chapter
│   └── NNN-slug/                                      one folder per chapter, e.g. 005-stack-vs-heap
│       ├── README.md        chapter-local index
│       ├── article.md       the full, canonical chapter (source of truth)
│       ├── linkedin.md      LinkedIn-optimized rewrite
│       ├── medium.md        Medium-optimized rewrite
│       ├── devto.md         Dev.to-optimized rewrite
│       ├── summary.md       TL;DR / key takeaways
│       ├── faq.md           frequently asked questions
│       ├── interview.md     senior-level interview Q&A
│       ├── quiz.md          self-check quiz
│       ├── exercises.md     hands-on exercises
│       ├── references.md    further reading
│       ├── code/            complete, runnable .NET 10 / C# examples
│       ├── images/          exported chapter cover + rendered images
│       └── diagrams/        mermaid/ drawio/ plantuml/ svg/ png/ — see IMAGE_GUIDE.md
│
├── assets/
│   ├── brand/                 logo, hero cover, chapter-cover template, social banner
│   └── components/            reusable diagram icon library — see VISUAL_LANGUAGE.md
│
├── linkedin/  medium/  devto/  website/  ebook/    per-platform compiled bundles
├── docs/                    optional static-site source, for the eventual inside-dotnet.dev
├── slides/                  per-chapter presentation decks
└── scripts/                 automation (build.ps1, planned) — see ARCHITECTURE.md
```

## Chapter template

Every chapter follows the same 13-section structure so the series reads like one cohesive book, not a pile of blog posts — see [WRITING_GUIDE.md](WRITING_GUIDE.md) for the full definition of done:

1. Chapter cover
2. Learning Objectives
3. Real-world Analogy
4. Problem Statement
5. Visual Explanation
6. Under the Hood
7. Code Example (Example → Advanced → Performance → Production)
8. Performance Notes
9. Common Mistakes / Anti-Patterns
10. Architect's Perspective (Developer → Senior → Architect)
11. Interview Questions
12. Quiz
13. Summary & Next Chapter

Every chapter must also answer: **Why? → How? → What happens internally? → When should I use it? → When shouldn't I? → How does Microsoft implement it? → How does it scale? → How does an architect think about it?**

## Visual identity

- Clean white background, subtle gradients
- .NET purple/blue accents (`#512BD4`, `#0078D4`) — full palette in [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)
- Isometric and flat technical diagrams, minimal text, self-explanatory
- No stock images — every diagram is purpose-built for this series, composed from the reusable [component library](assets/components/) where possible

## Roadmap

See [ROADMAP.md](ROADMAP.md) for the full 12-phase plan and [`book.json`](book.json) for the machine-readable chapter list and status (58 planned chapters across Phases 0–11 plus a capstone).

## Status

| Phase | Chapters | Status |
|---|---|---|
| 0 — Project Foundation | — | ✅ Done (v1.0) |
| 1 — Runtime Fundamentals | [001](chapters/001-execution-flow)–[004](chapters/004-assemblies-metadata) | ✅ Done |
| 2 — Memory | [005](chapters/005-stack-vs-heap)–[006](chapters/006-value-vs-reference-types) done, 007–013 planned | 🚧 In progress |
| 3 — C# | 014–018 | ⏳ Planned |
| 4 — Clean Code | 019–022 | ⏳ Planned |
| 5 — Design Patterns | 023–026 | ⏳ Planned |
| 6 — ASP.NET Core | 027–032 | ⏳ Planned |
| 7 — Dependency Injection | 033–035 | ⏳ Planned |
| 8 — Entity Framework Core | 036–040 | ⏳ Planned |
| 9 — Concurrency | 041–045 | ⏳ Planned |
| 10 — Architecture | 046–051 | ⏳ Planned |
| 11 — Production | 052–056 | ⏳ Planned |
| Capstone — Becoming an Architect | 057–058 | ⏳ Planned |

**Written so far:** [000](chapters/000-introduction) Welcome to Inside .NET · [001](chapters/001-execution-flow) Execution Flow · [002](chapters/002-clr) Understanding the CLR · [003](chapters/003-jit-compilation) JIT Compilation Explained · [004](chapters/004-assemblies-metadata) Assemblies, DLLs & Metadata · [005](chapters/005-stack-vs-heap) Stack vs Heap · [006](chapters/006-value-vs-reference-types) Value Types vs Reference Types

More chapters land incrementally in continuous batches — each one fully written, diagrammed, coded, and verified before moving to the next. See [CONTRIBUTING.md](CONTRIBUTING.md) for the working agreement.
