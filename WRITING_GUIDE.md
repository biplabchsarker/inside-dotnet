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

1. **Chapter cover** — reference to `images/NNN-cover.png`, matching [BRAND_GUIDE.md](BRAND_GUIDE.md)
2. **Learning Objectives** — 3–5 bullet points, phrased as "By the end of this chapter, you will be able to…"
3. **Real-world Analogy** — a fresh analogy (never reused from another chapter), that survives being pushed on
4. **Problem Statement** — why does this concept exist; what breaks or gets harder without it
5. **Visual Explanation** — 3–5 original diagrams (see [IMAGE_GUIDE.md](IMAGE_GUIDE.md))
6. **Under the Hood** — the actual CLR/BCL/framework mechanics, precisely and correctly
7. **Code Example** — see the four-tier code progression below
8. **Performance Notes** — cost, measurement approach, and any relevant benchmark data
9. **Common Mistakes / Anti-Patterns** — the ones that actually show up in code review, not trivia
10. **Architect's Perspective** — three-tier (see below)
11. **Interview Questions** — 4–6 senior-level Q&A tied directly to this chapter
12. **Quiz** — 5 questions with collapsible answers
13. **Summary & Next Chapter** — key takeaways plus a bridge to the next episode

`exercises.md` and `references.md` are separate files but are considered part of the required chapter output.

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

- [ ] All 13 `article.md` sections are present and non-stub
- [ ] `linkedin.md`, `medium.md`, `devto.md`, `summary.md`, `faq.md`, `interview.md`, `quiz.md`, `exercises.md`, `references.md` all exist with real content
- [ ] `code/` builds and runs cleanly (`dotnet run`), verified, not assumed
- [ ] At least 3 diagrams exist as both inline Mermaid (in `article.md`) and standalone source files under `diagrams/`
- [ ] A chapter cover exists under `images/NNN-cover.png` (+ SVG source under `diagrams/svg/`)
- [ ] All eight questions (above) are demonstrably answered somewhere in the chapter
- [ ] Terminology matches [STYLE_GUIDE.md](STYLE_GUIDE.md#terminology)
- [ ] `book.json`'s `status` map is updated to `"done"` for this chapter
- [ ] Root `README.md` status table is updated
