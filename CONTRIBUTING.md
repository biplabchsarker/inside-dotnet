# Contributing

Inside .NET is currently authored by Biplab Sarker (vision, review, approval, repository/publishing management) working with an AI collaborator (drafting, illustration, code samples) — see the working agreement below. External contributions aren't the primary workflow yet, but the standards here apply equally if that changes.

## Working agreement

Every chapter (or foundation milestone) moves through the same five steps:

1. **Plan** — agree on scope: which chapter, which topics from [ROADMAP.md](ROADMAP.md), any real-world insight to fold in.
2. **Create** — draft produced: `article.md`, diagrams, code, and all supporting files per [WRITING_GUIDE.md](WRITING_GUIDE.md).
3. **Review** — reviewed from a Solution Architect's perspective: technical accuracy, whether the Architect's Perspective section actually says something non-obvious, whether the code would survive real code review.
4. **Refine** — corrections and real-world insights get incorporated.
5. **Publish** — merged into the repository, `book.json`/`README.md` status updated, platform variants prepared per [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md).

## Responsibilities

**Project owner (Biplab Sarker):**
- Defines overall vision and roadmap priorities
- Reviews technical accuracy and architectural direction
- Approves content before publication
- Manages the repository and publishing destinations
- Contributes real-world insights and war stories where they strengthen a chapter

**Content collaborator (AI-assisted drafting):**
- Writes chapters to the [WRITING_GUIDE.md](WRITING_GUIDE.md) template
- Creates original diagrams per [IMAGE_GUIDE.md](IMAGE_GUIDE.md) / [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)
- Produces and verifies runnable code samples
- Maintains terminology/style consistency per [STYLE_GUIDE.md](STYLE_GUIDE.md)
- Generates platform-specific variants per [PUBLISHING_GUIDE.md](PUBLISHING_GUIDE.md)
- Flags gaps, inconsistencies, or scope creep for the project owner to decide on

## Quality gate

Nothing is considered complete unless it is:

- **Accurate** — technically correct, verified where verifiable (code runs, referenced APIs exist)
- **Clear** — a reader shouldn't need outside context to follow the chapter
- **Original** — no copied prose, no scraped diagrams, no stock imagery
- **Visual** — the required diagrams exist and pull their weight
- **Runnable** — every code sample builds and runs
- **Architect-level** — the Architect's Perspective section says something a mid-level developer wouldn't already know
- **Consistent** — terminology, tone, and visual style match every previous chapter

## If you're proposing a new chapter or reordering the roadmap

Open the discussion against [ROADMAP.md](ROADMAP.md) first — chapter numbering is phase-ordered (see [ARCHITECTURE.md](ARCHITECTURE.md)), so inserting a chapter after others already exist means renumbering stub folders, which is cheap before they have content and expensive after. Confirm placement before writing.

## License note for contributors

By contributing, you agree your code contributions are licensed under `LICENSE-CODE` (MIT) and written/visual contributions under `LICENSE-CONTENT` (CC-BY-4.0) — see [LICENSE.md](LICENSE.md).
