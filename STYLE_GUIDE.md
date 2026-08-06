# Style Guide

## Tone

- **Professional** — this is written for senior engineers and architects, not beginners. No condescension, no filler enthusiasm.
- **Friendly** — direct and readable, not academic or dry. Contractions are fine. Sentences should sound like a knowledgeable colleague explaining something, not a spec document.
- **Architect-level** — assume fluency in C# and general software engineering. Never explain what a `for` loop is. Do explain what the CLR does with your `for` loop.
- **Story-driven** — every chapter opens with a concrete scenario or analogy before it opens with a definition.

## Voice rules

- Explain mechanisms, not marketing. If a .NET feature has a real cost or limitation, say so.
- Use "you" to address the reader directly; avoid "one might consider."
- Prefer active voice: "The JIT compiles the method" not "The method is compiled by the JIT."
- Short paragraphs. If a paragraph needs a second `but` or `however`, consider splitting it.
- Technical terms are introduced once, defined precisely, and used consistently afterward — see the [Terminology](#terminology) section below.

## Code

- **Target framework: .NET 10** (the current LTS at the time of writing) unless a chapter is specifically about version-specific behavior, in which case the version under discussion is called out explicitly in the article text.
  - Note: earlier project drafts referenced .NET 9 as the target — this was superseded once .NET 10 became the working environment's installed SDK. .NET 9 is out of the picture; do not target it for new chapters.
- Every code sample must be **production-quality**: nullable-reference-aware, no unhandled exceptions in the happy path, no dead code, no toy shortcuts that wouldn't survive code review.
- Every code sample must **actually build and run** — verified with `dotnet run` before a chapter is marked done, not assumed.
- Prefer modern C# idioms (top-level statements, file-scoped namespaces, primary constructors, collection expressions) where they don't obscure the point being demonstrated — but never at the expense of clarity for a reader unfamiliar with a very new syntax feature; introduce/explain unfamiliar syntax briefly if you use it.
- Comments in code explain **why**, not what — the same rule that governs prose applies to code.

## Naming conventions

- **Chapters:** `NNN-topic-slug` (three-digit, zero-padded, kebab-case) — e.g. `019-solid-principles`. Chapter `000` is reserved for the series introduction.
- **Diagrams:** `NN-diagram-name.ext` within a chapter's `diagrams/<format>/` folder — e.g. `diagrams/mermaid/01-execution-flow.mmd`, `diagrams/svg/02-clr-architecture.svg`.
- **Chapter covers:** `NNN-cover.svg` (source) and `NNN-cover.png` (export) at 16:9.
- **Code projects:** `ChapterNNN.Demo` (PascalCase, matching the chapter number) — e.g. `Chapter019.Demo`.
- **Phases:** referred to as "Phase 0" through "Phase 11" in planning docs; chapters are grouped into Parts (`Part I` – `Part XII`) in reader-facing content — see [ROADMAP.md](ROADMAP.md) for the mapping.

## Terminology

Use these terms precisely and consistently across all chapters — inconsistent terminology is the fastest way to make a 58-chapter series feel like it wasn't written as one project:

| Term | Use it for | Don't confuse it with |
|---|---|---|
| CLR | The Common Language Runtime as a whole (the engine) | "the runtime" used loosely to mean the SDK or `dotnet` CLI |
| BCL | Base Class Library (`System.*` fundamental types) | FCL (broader, includes ASP.NET Core/EF Core — avoid using "FCL" at all unless a chapter is specifically discussing the distinction) |
| IL | Intermediate Language (the compiled-but-not-native form) | "bytecode" (avoid — it invites a false equivalence with the JVM that we don't need) |
| Assembly | The unit of deployment/versioning/identity (a `.dll`/`.exe`) | "package" (a NuGet package can contain multiple assemblies) |
| Managed code | Code executing under CLR type-safety/GC | "safe code" (a different, narrower C# keyword-level concept) |
| .NET | The current, cross-platform, open-source platform (.NET 5+) | ".NET Framework" (the legacy, Windows-only, pre-.NET-5 platform) — always disambiguate on first mention in a chapter if the distinction matters |

## Formatting

- Headings use sentence case ("What really happens", not "What Really Happens") **except** chapter titles and episode titles, which use title case to match the series branding.
- Mermaid diagrams are embedded directly in `article.md` for readability AND saved as standalone source files under `diagrams/mermaid/` for reuse — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md).
- Inline code uses backticks; file paths and folder names also use backticks.
- Cross-references to other chapters always use the format `[Episode N — Title](../NNN-slug/article.md)`.
