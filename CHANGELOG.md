# Changelog

All notable structural, foundation, and roadmap changes to this project. Individual chapter publication is tracked in `book.json`'s `status` map and the README status table, not duplicated here — this log is for changes to the *project itself*.

## [1.4.0] — 2026-08-14 — Chapter 010 drafted

### Added
- Chapter 010 (Garbage Collection Fundamentals) written in full: `article.md`'s 13 sections, all companion files, 5 Mermaid diagrams, and two .NET 10 code projects — `Chapter10.Demo` and `Chapter10.Benchmarks` (real `BenchmarkDotNet` measurements: Workstation vs. Server GC throughput, per-generation forced-collection cost — a full collection measured 5.84× a Gen 0 collection — and pre-sized vs. growing `List<T>`, ~1.9× faster)
- A genuinely surprising fact verified empirically for this chapter (not assumed from folklore): nulling a local variable and calling `GC.Collect()` from the *same still-executing method frame* does not reliably free the object — confirmed three separate ways (`WeakReference`, `GC.GetTotalMemory`, repeated-call JIT-tiering check) before being written up
- Chapter 010's content brief added to `ILLUSTRATION_BRIEFS.md`, ready for external sourcing; status set to `"drafted"` per the convention introduced for Chapter 009

### Fixed
- Chapter 009's README "Next" link no longer says "(not yet written)" now that Chapter 010 exists

### Known gap
- Chapter 010 has no signature illustrations yet (same gap as Chapter 009 — no image-generation/commissioning tool available this session). Tracked in `BACKLOG.md`.

## [1.3.0] — 2026-08-13 — Chapter 009 drafted; new "drafted" status convention

### Added
- Chapter 009 (Strings & Interning) written in full: `article.md`'s 13 sections, all companion files, 5 Mermaid diagrams, and two .NET 10 code projects — `Chapter09.Demo` and `Chapter09.Benchmarks` (real `BenchmarkDotNet`/`GC.GetTotalMemory` measurements: ordinal-vs-culture comparison cost, the `Equals` reference-equality fast path, `Substring`'s self-reference fast path, and a measured ~16 MB intern-pool memory-retention proof)
- Chapter 009's content brief added to `ILLUSTRATION_BRIEFS.md`, ready for external sourcing
- New `book.json` status value, `"drafted (...)"`, for a chapter whose text/code/diagrams are complete but whose five signature illustrations aren't sourced yet — distinct from `"done"` and `"planned"`; see `BACKLOG.md` for the exact convention and when to promote it to `"done"`

### Known gap
- Chapter 009 has no signature illustrations at all yet (not even an interim SVG placeholder, unlike Chapters 000-008) — no image-generation/commissioning tool was available when it was written. Tracked in `BACKLOG.md`.

## [1.2.0] — 2026-08-07 — Cheat Sheets, cross-linking, real benchmarks, backlog

### Added
- Fourth signature illustration: **Cheat Sheet** (one-line rule + bullets + one interview question) — `assets/templates/cheat-sheet.svg`, built for Chapters 000-007, documented in `IMAGE_GUIDE.md`/`VISUAL_LANGUAGE.md`
- **Related Chapters** line added to every chapter's journey footer (000-007), pointing to substantively-connected chapters beyond just previous/next
- Real `BenchmarkDotNet` numbers in Chapter 006's Performance Notes (`code/Chapter06.Benchmarks/`) — large-struct-by-value vs. by-`in` vs. equivalent class, measured, not assumed
- `BACKLOG.md` — deferred review feedback (illustration-quality ceiling, future runtime-internals depth, additional chapter candidates, known content-bug pattern), so review feedback that can't be applied immediately doesn't just get lost
- Version badge + phase-status table at the top of `README.md`

### Fixed
- Chapter 001 referenced a nonexistent "Episode 9 — Native AOT" chapter (Native AOT is covered inside Episode 4 / chapter 003) — corrected to point there
- Chapter 006 linked boxing's deep-dive to the wrong chapter (007 instead of 008) — corrected

### Changed
- `WRITING_GUIDE.md`'s Definition of Done now requires all four signature illustrations (was three) and a Related-Chapters entry in the journey footer

## [1.0.0] — 2026-08-06 — Foundation

### Added
- Full governance doc set: `PHILOSOPHY.md`, `STYLE_GUIDE.md`, `WRITING_GUIDE.md`, `IMAGE_GUIDE.md`, `ARCHITECTURE.md`, `DESIGN_SYSTEM.md`, `VISUAL_LANGUAGE.md`, `BRAND_GUIDE.md`, `CONTRIBUTING.md`, `PUBLISHING_GUIDE.md`, `ROADMAP.md`, this `CHANGELOG.md`
- Dual license: `LICENSE-CODE` (MIT) + `LICENSE-CONTENT` (CC BY 4.0), explained in `LICENSE.md`
- Brand v1: hand-coded SVG logo, hero cover, chapter-cover template, social banner (`assets/brand/`)
- Component icon library v1: 10 seed components (`assets/components/`)
- `chapters/_template/` — copyable chapter skeleton matching `WRITING_GUIDE.md`
- Expanded roadmap: Phase 0 (Foundation) through Phase 11 (Production) plus a capstone, 58 planned chapters total (up from the original ~50-chapter draft), inserting a dedicated Phase 3 — C# (OOP, delegates/events, generics, reflection/expression trees, records/pattern matching) and reordering ASP.NET Core ahead of Dependency Injection to match the finalized roadmap

### Changed
- Repository renamed: `biplabchsarker/.Net_Journey` → `biplabchsarker/inside-dotnet` (GitHub auto-redirects the old URL); local folder renamed to match
- Chapter numbering renumbered across all not-yet-written stub folders to align with the finalized 12-phase roadmap (chapters 000–006, already written, kept their existing numbers — Phase 1 and the start of Phase 2 already matched the new plan)
- Diagram folder structure expanded per chapter from a flat `diagrams/` to `diagrams/{mermaid,drawio,plantuml,svg,png}/`
- `code/` target framework standard confirmed as **.NET 10** (superseding earlier .NET 9 references — the working environment's installed SDK is .NET 10)

### Chapters
- 000–006 written previously (Welcome, Execution Flow, CLR, JIT Compilation, Assemblies/Metadata, Stack vs Heap, Value vs Reference Types) — retrofit to the finalized `WRITING_GUIDE.md` template tracked as part of this milestone

## [0.1.0] — 2026-08-06 — Initial scaffold

### Added
- Initial repository scaffold: `book.json`, `chapters/`, `assets/`, per-platform output folders
- Chapters 000 (Welcome to Inside .NET) and 001 (What Really Happens When You Run a .NET Application?) written in full
- Local git repository initialized
