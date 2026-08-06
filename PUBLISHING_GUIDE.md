# Publishing Guide

One Markdown source. Many platforms. `article.md` is the canonical version of every chapter — everything else is a derived adaptation of it, never an independently-authored fork that can drift out of sync.

```
article.md (source of truth)
   ├─→ linkedin.md    (LinkedIn article/post)
   ├─→ medium.md      (Medium article)
   ├─→ devto.md        (Dev.to article)
   ├─→ website          (docs/ static site page)
   ├─→ PDF               (compiled handbook chapter)
   └─→ PowerPoint        (slide deck, when a chapter warrants one)
```

## Per-platform conventions

### LinkedIn (`linkedin.md`)
- Hook in the first two lines — LinkedIn truncates aggressively before "see more."
- Short paragraphs, generous line breaks, arrows (`→`) for sequences instead of nested bullet lists (LinkedIn's renderer handles simple line breaks better than markdown lists).
- Ends with 3–5 relevant hashtags (`#dotnet #csharp #softwarearchitecture` + chapter-specific tags).
- Length target: 200–400 words — enough to deliver one real idea, short enough to be read in-feed.

### Medium (`medium.md`)
- Long-form, closer to `article.md` in depth but rewritten for Medium's reading rhythm (shorter paragraphs than a technical doc, more subheadings).
- Keep the same analogy and diagrams reference as `article.md` — don't introduce a different analogy per platform, that breaks series consistency.
- Ends with a "Next" link to the following chapter's `medium.md` (not `article.md`) so a reader who found the series on Medium stays on Medium.

### Dev.to (`devto.md`)
- Similar to Medium in depth, but Dev.to's audience skews more toward "show me the code faster" — lead with a slightly shorter narrative wind-up before the first code block compared to the Medium version.
- Use Dev.to's native code-fence syntax highlighting (` ```csharp `) and its `{% embed %}` conventions where relevant (e.g. embedding the GitHub code sample link).

### Website (`docs/`)
- Rendered close to verbatim from `article.md` — the website is the "full depth, no compromises" version, matching the source most closely of all platforms.
- Gets search, dark mode, and chapter navigation (see [ROADMAP.md](ROADMAP.md) — `inside-dotnet.dev` / GitHub Pages).

### PDF (handbook)
- Compiled from `article.md` across a full Part (or the full book at v6.0), with chapter covers (`images/NNN-cover.png`) inserted as section breaks.
- Table of contents auto-generated from chapter titles + Learning Objectives.

### PowerPoint (`slides/`)
- Not every chapter needs a deck — produce one when a chapter's diagrams are strong enough to carry a talk (internal team session, meetup, conference lightning talk).
- One diagram (or diagram cluster) per slide; Architect's Perspective section maps naturally to a "So what does this mean for your system?" closing slide.

## Publishing checklist — per chapter

- [ ] `linkedin.md` written, hook-tested (would the first two lines make you click "see more"?)
- [ ] `medium.md` written, links forward to next chapter's `medium.md`
- [ ] `devto.md` written
- [ ] Website page generated/updated (once `docs/` exists)
- [ ] Downloadable code sample zip/link verified (points at `code/` in the repo)
- [ ] Slide deck added if warranted

## Automation roadmap

`scripts/build.ps1` is the target end-state: one command, run against a chapter folder, that regenerates every platform variant from `article.md` plus a small front-matter block (title, part, episode number, next-chapter link). This is a **Phase-later** goal — see [ARCHITECTURE.md](ARCHITECTURE.md#publishing-pipeline-current-vs-planned) and [ROADMAP.md](ROADMAP.md) milestone v6.0. Until it exists, every variant above is written by hand, but written *to* this guide's conventions, so automating the transformation later is mechanical rather than a rewrite.
