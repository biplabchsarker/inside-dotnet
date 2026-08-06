# Changelog

All notable structural, foundation, and roadmap changes to this project. Individual chapter publication is tracked in `book.json`'s `status` map and the README status table, not duplicated here — this log is for changes to the *project itself*.

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
