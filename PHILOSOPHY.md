# Philosophy

> We don't teach syntax. We teach understanding.

## Why this project exists

Most .NET content falls into two camps: tutorials that teach *what to type* without explaining *why it works*, or official documentation that's accurate but written for lookup, not for learning. **Inside .NET** is the missing middle layer — a structured, visual, progressively-building explanation of what the runtime, framework, and architecture patterns actually do, connected to real production code and real interview questions.

Our benchmark is not other GitHub repositories. Our benchmark is Microsoft Learn, the .NET Runtime documentation, Martin Fowler, Microsoft Press, O'Reilly, Manning, and JetBrains Guides — but with stronger visuals and a more deliberate learning journey.

## Core principles

1. **Explain why before how.** A mechanism without a reason is trivia. Every topic opens with the problem it solves before it shows the solution.
2. **Prefer visuals over long paragraphs.** If a diagram can carry the idea, it should — original artwork, not decoration.
3. **Every concept is grounded in a real-world analogy** that holds up under scrutiny (not a cute metaphor that breaks the moment someone pushes on it).
4. **Every concept connects to internal .NET mechanics** — the actual CLR/BCL/runtime behavior, not folklore.
5. **Every concept ships with production-ready code**, not toy snippets that fall apart outside a demo.
6. **Every concept documents its common mistakes and performance cost.** Understanding a mechanism includes understanding what it costs and how it fails.
7. **Every concept is viewed from an architect's altitude**, not just a developer's — see [Architect's Perspective](WRITING_GUIDE.md#architects-perspective) in the Writing Guide.
8. **All diagrams are original**, created specifically for this project. No stock images, no scraped diagrams, no unlicensed assets.
9. **Consistency beats novelty.** A reader should be able to tell an Inside .NET chapter apart from any other .NET content by its structure and visual language alone, chapter 1 or chapter 58.
10. **Nothing ships half-finished.** A chapter that skips the quiz, the interview questions, or the architect's perspective is not "mostly done" — it's not done.

## What we are not

- Not a syntax tutorial. We assume the reader can already write C#.
- Not a documentation mirror. If Microsoft Learn already explains something well at reference-depth, we link to it rather than restate it — our value is the narrative and the visual model, not duplicating the API reference.
- Not a marketing piece for .NET. Trade-offs get stated plainly, including where .NET's design costs something (startup latency, GC pauses, allocation pressure).

## Source of truth

This document, together with [ROADMAP.md](ROADMAP.md), [STYLE_GUIDE.md](STYLE_GUIDE.md), [WRITING_GUIDE.md](WRITING_GUIDE.md), [IMAGE_GUIDE.md](IMAGE_GUIDE.md), [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md), [VISUAL_LANGUAGE.md](VISUAL_LANGUAGE.md), [BRAND_GUIDE.md](BRAND_GUIDE.md), [ARCHITECTURE.md](ARCHITECTURE.md), and [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md), is the canonical foundation for the project. Any chapter, diagram, or asset that conflicts with these documents is wrong, not the documents.
