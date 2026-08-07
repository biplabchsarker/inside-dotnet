# Writing Guide

This is the canonical chapter template and completion checklist. A chapter is not "draft complete" until every item below is checked. See [`chapters/_template/`](chapters/_template/) for a ready-to-copy skeleton.

## Chapter file layout

```
chapters/NNN-topic-slug/
├── README.md          chapter-local index (links to every file below)
├── article.md         the full canonical chapter (source of truth)
├── linkedin.md        LinkedIn-optimized rewrite
├── medium.md          Medium-optimized rewrite
├── devto.md           Dev.to-optimized rewrite
├── summary.md         TL;DR / key takeaways
├── faq.md             frequently asked questions
├── interview.md       senior-level interview Q&A
├── quiz.md            self-check quiz (with collapsible answers)
├── exercises.md        hands-on exercises (see below)
├── references.md      further reading
├── code/               complete, runnable code sample(s)
├── images/             exported chapter cover + any rendered images
└── diagrams/
    ├── mermaid/        .mmd source (rendered inline in article.md too)
    ├── drawio/         .drawio source for anything beyond Mermaid's expressive range
    ├── plantuml/       .puml source for sequence/class diagrams where PlantUML fits better
    ├── svg/             hand-authored or exported SVG (final, styled per DESIGN_SYSTEM.md)
    └── png/             PNG export of every SVG, for platforms that don't render SVG
```

## `article.md` structure — the 13-section template

1. **Chapter cover** — reference to `images/NNN-cover.png`, matching [BRAND_GUIDE.md](BRAND_GUIDE.md), immediately followed by the Hero Cover illustration (`images/NNN-hero.png`)
2. **Learning Objectives** — 3–5 bullet points, phrased as "By the end of this chapter, you will be able to…"
3. **Real-world Analogy** — opens with a **storytelling hook**: 2–4 sentences grounding the concept in a concrete, high-stakes scenario (a real system — banking, healthcare, high-frequency trading, a production incident — not an abstract example), *then* the analogy itself. The analogy is fresh (never reused from another chapter) and survives being pushed on.
4. **Problem Statement** — why does this concept exist; what breaks or gets harder without it
5. **Visual Explanation** — the Concept Overview illustration, then 3–5 original diagrams (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md))
6. **Under the Hood** — the Runtime/Internal View illustration and the Memory/Execution Diagram, then the actual CLR/BCL/framework mechanics, precisely and correctly
7. **Code Example** — see the four-tier code progression below
8. **Performance Notes** — the Performance & Quick Reference illustration (real measured numbers, never fabricated), cost, measurement approach, and any relevant benchmark data
9. **Common Mistakes / Anti-Patterns** — the ones that actually show up in code review, not trivia
10. **Architect's Perspective** — three-tier (see below); each tier opens with a sharp, italicized one-line question before its explanation (e.g. *"Is this interface earning its dispatch cost, or is it decoration?"*) — this is a signature feature of the series, not decoration
11. **Interview Questions** — 4–6 senior-level Q&A tied directly to this chapter
12. **Quiz** — 5 questions with collapsible answers
13. **Summary & Next Chapter** — key takeaways, a bridge to the next episode, a "Where you are in the journey" footer (a fenced ASCII box showing `Previous episode → ▶ This episode ◀ you are here → Next episode`), and a **Related Chapters** line linking substantively-connected chapters beyond just previous/next (a forward reference this chapter deferred to, a chapter that reuses a concept introduced here, etc.) — verify target chapter numbers against `book.json` before linking, don't rely on memory of what a chapter "should" be numbered

> **v2 image standard (2026-08-08):** the illustration set is now five images (Hero Cover, Concept Overview, Runtime/Internal View, Memory/Execution Diagram, Performance & Quick Reference), externally sourced against [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md) rather than hand-coded SVG. This supersedes the four-illustration structure (Hero/Concept/Deep-Dive/Cheat Sheet) used in Chapters 000-008 — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md) for the full rationale and [BACKLOG.md](BACKLOG.md) for the decision history. Chapters 000-008 are not yet retrofitted.

`exercises.md` and `references.md` are separate files but are considered part of the required chapter output. `exercises.md` additionally requires a **Challenge** section: a short, concrete "predict the output before running it" code snippet distinct from `quiz.md`'s conceptual questions — it should test a specific, nameable misconception this chapter addresses.

## Code example — four-tier progression

Every chapter's `code/` folder should progress through up to four tiers (not every chapter needs all four — use judgment, but at minimum Example + one deeper tier):

1. **Example** — the simplest correct illustration of the concept
2. **Advanced Example** — a more realistic variation (edge cases, composition with other features)
3. **Performance Example** — a benchmark or timing comparison that makes the chapter's performance claims concrete
4. **Production Example** — how this looks inside a realistic, larger codebase (error handling, DI, logging, tests where relevant)

## Architect's Perspective — three tiers

Every chapter's Architect's Perspective section must move through three altitudes, explicitly labeled:

- **Developer Perspective** — how do I use this correctly, today, in the code I'm writing
- **Senior Perspective** — when does this choice matter, what does it trade off against alternatives, what will bite a team in code review
- **Architect Perspective** — how does this decision propagate across a system: scalability, maintainability, team conventions, migration cost, and how Microsoft itself implements/recommends it internally where relevant

## The eight questions every chapter must answer

If a chapter doesn't answer all eight, it is not complete:

1. **Why?** — why does this concept/feature exist
2. **How?** — how do you use it correctly
3. **What happens internally?** — the actual runtime/framework mechanics
4. **When should I use it?**
5. **When shouldn't I?**
6. **How does Microsoft implement it?** — tie back to the actual BCL/runtime/framework source where feasible
7. **How does it scale?**
8. **How does an architect think about it?** — ties directly to the Architect's Perspective section

## Definition of done

A chapter is complete only when:

- [ ] All 13 `article.md` sections are present and non-stub, including the storytelling hook and the three Architect's Perspective lead-questions
- [ ] `linkedin.md`, `medium.md`, `devto.md`, `summary.md`, `faq.md`, `interview.md`, `quiz.md`, `exercises.md`, `references.md` all exist with real content
- [ ] `exercises.md` includes a Challenge section
- [ ] `code/` builds and runs cleanly (`dotnet run`), verified, not assumed
- [ ] At least 3 diagrams exist as both inline Mermaid (in `article.md`) and standalone source files under `diagrams/`
- [ ] The chapter's five signature illustrations exist (Hero Cover, Concept Overview, Runtime/Internal View, Memory/Execution Diagram, Performance & Quick Reference — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md) and [`ILLUSTRATION_BRIEFS.md`](ILLUSTRATION_BRIEFS.md)), sourced externally as 1600×900 PNG, and referenced inline in `article.md`
- [ ] The journey footer at the end of `article.md` links the correct previous/next chapters, and includes a Related Chapters line
- [ ] All eight questions (above) are demonstrably answered somewhere in the chapter
- [ ] Terminology matches [STYLE_GUIDE.md](STYLE_GUIDE.md#terminology)
- [ ] `book.json`'s `status` map is updated to `"done"` for this chapter
- [ ] Root `README.md` status table is updated
