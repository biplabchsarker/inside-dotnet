# Inside .NET
### Understanding What Really Happens Under the Hood
*A Visual Journey from Developer to Solution Architect — by Biplab Sarker*

Most developers know how to write code. Fewer understand what the .NET runtime is actually doing behind the scenes. **Inside .NET** is a chapter-by-chapter series that bridges that gap using original diagrams, real-world analogies, internal runtime explanations, practical C# examples, performance notes, common mistakes, and interview insights.

Every chapter builds on the previous one, taking readers from developer fundamentals to solution architecture.

## Who this is for
Junior & mid-level developers · Senior engineers · Software architects · Technical leads · Engineering managers · Students preparing for interviews.

## How this repo is organized

```
.Net_Journey
├── README.md              this file
├── book.json               series metadata: parts, chapter list, status, visual identity
│
├── chapters/               one folder per chapter (numbered, e.g. 005-stack-vs-heap)
│   └── NNN-slug/
│       ├── README.md        chapter-local index
│       ├── article.md       the full, canonical chapter (long-form)
│       ├── linkedin.md      LinkedIn-optimized rewrite (short, hook-driven)
│       ├── medium.md        Medium-optimized rewrite
│       ├── summary.md       TL;DR / key takeaways
│       ├── faq.md           frequently asked questions
│       ├── interview.md     senior-level interview Q&A for this topic
│       ├── references.md    further reading
│       ├── quiz.md          self-check quiz
│       ├── code/            complete, runnable .NET 10 / C# examples
│       ├── images/          exported PNG/SVG renders for publishing
│       └── diagrams/        source diagrams (Mermaid / SVG / PlantUML / drawio)
│
├── assets/                  series-wide visual identity assets (banners, icons, shared diagrams)
├── code/                    cross-chapter or shared solution-level code (if any)
├── linkedin/  medium/  devto/  website/  ebook/    per-platform compiled bundles
├── docs/                    optional MkDocs/Docusaurus site source
├── slides/                  per-chapter presentation decks
└── scripts/                 automation (generate.ps1 etc.) to produce platform variants from article.md
```

## Chapter template
Every chapter follows the same structure so the series reads like one cohesive book, not a pile of blog posts:

1. Introduction
2. A real-world analogy
3. The problem being solved
4. Original visual explanation (diagrams)
5. Internal .NET mechanics
6. C# implementation
7. Common mistakes
8. Performance considerations
9. Interview questions
10. Key takeaways
11. What's next in the series

## Visual identity
- Clean white background, subtle gradients
- Blue/purple accents inspired by .NET branding (`#512BD4`, `#0078D4`)
- Isometric and flat technical diagrams, minimal text, self-explanatory
- No stock images — every diagram is purpose-built for this series (Mermaid/SVG source lives in each chapter's `diagrams/` folder)

## Roadmap
See [`book.json`](book.json) for the full Table of Contents (Parts I–XI, 50+ chapters) and per-chapter status.

## Status
| Chapter | Title | Status |
|---|---|---|
| [000](chapters/000-introduction) | Welcome to Inside .NET | Draft |
| [001](chapters/001-execution-flow) | What Really Happens When You Run a .NET Application? | Draft |

More chapters land incrementally — each one fully written, diagrammed, and coded before moving to the next.
