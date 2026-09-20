# Roadmap

**Status:** 🚧 Foundation Phase (v1.0)

This is the master plan: what gets built, in what order, and who's responsible for what at each phase. See [book.json](book.json) for the machine-readable version of the chapter list and status.

## Phase 0 — Project Foundation *(current)*

**Goal:** build the platform before building content.

**Repository**
- [x] Finalize folder structure
- [x] README
- [x] ROADMAP
- [x] PHILOSOPHY
- [x] STYLE GUIDE
- [x] WRITING GUIDE
- [x] IMAGE GUIDE
- [x] ARCHITECTURE
- [x] DESIGN SYSTEM
- [x] CONTRIBUTING
- [x] LICENSE (dual: MIT code / CC BY 4.0 content)
- [x] CHANGELOG

**Branding**
- [x] Logo (v1, hand-coded SVG)
- [x] Hero Cover
- [x] Chapter Cover template
- [x] Social Banner
- [x] Color Palette
- [x] Typography
- [x] Icons (component library v1 — 10 seed components)

**Publishing**
- [x] GitHub (`github.com/biplabchsarker/inside-dotnet`)
- [ ] LinkedIn (per-chapter, ongoing)
- [ ] Medium (per-chapter, ongoing)
- [ ] Dev.to (per-chapter, ongoing)
- [ ] Website (`docs/`, planned for v6.0)
- [ ] PDF (compiled handbook, planned for v6.0)
- [ ] PowerPoint (per-chapter, as warranted)

**Your work:** review and approve the project foundation; decide the branding direction.
**Our work:** build the documentation standards, design system, chapter template, and produce original visual assets. *(Done for v1 — see this document's checkboxes above.)*

## Phase 1 — Runtime Fundamentals

Chapters 001–004. Topics: What is .NET, CLR, CTS, CLS, Assemblies, IL, JIT, Native AOT, Runtime startup.

## Phase 2 — Memory

Chapters 005–013. Topics: Stack, Heap, Value Types, Reference Types, Object Allocation, Boxing/Unboxing, String Interning, GC, LOH, Memory Leaks, IDisposable.

## Phase 3 — C#

Chapters 014–018. Topics: OOP fundamentals, Delegates & Events, Generics, Reflection & Expression Trees, Records & Pattern Matching.

## Phase 4 — Clean Code

Chapters 019–022. Topics: SOLID, DRY/KISS/YAGNI, Composition vs. Inheritance, Separation of Concerns & Refactoring.

## Phase 5 — Design Patterns

Chapters 023–026. Topics: Creational patterns (Singleton, Factory, Builder), Structural patterns (Adapter, Decorator, Facade), Behavioral patterns (Strategy, Observer), Repository & Unit of Work.

## Phase 6 — ASP.NET Core

Chapters 027–032. Topics: Kestrel & Request Pipeline, Middleware, Routing & Model Binding, Filters, Authentication & Authorization, Minimal APIs vs. Controllers.

## Phase 7 — Dependency Injection

Chapters 033–035. Topics: IoC, Service Provider, Singleton/Scoped/Transient lifetimes, DI container internals, Options Pattern.

## Phase 8 — EF Core

Chapters 036–040. Topics: DbContext lifecycle, Change Tracking, LINQ execution, Transactions & Migrations, Performance.

## Phase 9 — Concurrency

Chapters 041–045. Topics: Thread vs. Task, Thread Pool, async/await, Synchronization Context & Cancellation, Parallel Programming.

## Phase 10 — Architecture

Chapters 046–051. Topics: Clean Architecture, Onion/Hexagonal Architecture, CQRS & MediatR, DDD, Event-Driven Architecture & Microservices, Modular Monolith.

## Phase 11 — Production

Chapters 052–056. Topics: Docker & Kubernetes, Azure/Cloud deployment, Caching & Redis, Observability & OpenTelemetry, Performance Tuning & Security.

## Capstone — Becoming an Architect

Chapters 057–058. Topics: Becoming an Architect (scalability, HA, system design, decision records), Real-World Case Studies.

## GitHub Milestones

| Milestone | Scope |
|---|---|
| v1.0 | Foundation (this phase) |
| v1.1 | Branding v2 (if/when real illustration work is commissioned) |
| v2.0 | Runtime chapters (Phase 1) — **done** |
| v3.0 | Memory chapters (Phase 2) — **done** |
| v4.0 | C#, Clean Code, Design Patterns, DI, ASP.NET Core (Phases 3–7) — **in progress** |
| v5.0 | EF Core, Concurrency, Architecture, Production (Phases 8–11) |
| v6.0 | Website + compiled book (PDF/eBook) + automation (`scripts/build.ps1`) |

## Every chapter checklist

See [WRITING_GUIDE.md](WRITING_GUIDE.md) for the full Definition of Done. Summary: chapter cover, learning objectives, analogy, problem statement, visual explanation, internal implementation, four-tier code progression, best practices, anti-patterns, performance notes, three-tier Architect's Perspective, interview questions, quiz, exercises, references, summary, next-chapter preview.

## Working agreement

See [CONTRIBUTING.md](CONTRIBUTING.md) — Plan → Create → Review → Refine → Publish, repeated per chapter or milestone.

## The standard every chapter must meet

Every chapter should answer, demonstrably:

Why? → How? → What happens internally? → When should I use it? → When shouldn't I? → How does Microsoft implement it? → How does it scale? → How does an architect think about it?

If a chapter doesn't answer all eight, it's not complete — see [WRITING_GUIDE.md](WRITING_GUIDE.md#the-eight-questions-every-chapter-must-answer).

## Ultimate goal

The best open-source .NET learning resource — a visual, open-source learning platform where developers don't just learn APIs, they understand how .NET works internally. Our benchmark is not other GitHub repositories — it's Microsoft Learn, the .NET Runtime documentation, Martin Fowler, Microsoft Press, O'Reilly, Manning, and JetBrains Guides — with stronger visuals and a more deliberate learning journey. Eventually: `inside-dotnet.dev` (or GitHub Pages), with search, dark mode, navigation, syntax highlighting, and versioning, and a one-command build (`scripts/build.ps1`) that generates LinkedIn, Medium, Dev.to, Hashnode, GitHub Pages, PDF, PowerPoint, and eBook output from one Markdown source per chapter.
