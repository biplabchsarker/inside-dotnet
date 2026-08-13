# Chapter 009 — Strings & Interning

Part II — Memory

| File | Purpose |
|---|---|
| [article.md](article.md) | Full canonical chapter (13-section template; 5 diagrams, mechanics, code, Architect's Perspective, interview Q&A, quiz) |
| [summary.md](summary.md) | TL;DR |
| [faq.md](faq.md) | FAQ |
| [interview.md](interview.md) | Interview Q&A |
| [quiz.md](quiz.md) | Self-check quiz |
| [exercises.md](exercises.md) | Hands-on exercises + Challenge |
| [references.md](references.md) | Further reading |
| [linkedin.md](linkedin.md) | LinkedIn-optimized version |
| [medium.md](medium.md) | Medium-optimized version |
| [devto.md](devto.md) | Dev.to-optimized version |
| [code/](code/) | Two .NET 10 projects: `Chapter09.Demo` (literal vs. runtime interning, `string.Intern`/`IsInterned`, `Substring`'s self-reference fast path, the Turkish-I culture gotcha, and a measured intern-pool memory-retention proof) and `Chapter09.Benchmarks` (real `BenchmarkDotNet` measurements: ordinal vs. culture-aware comparison cost, the `Equals` reference-equality fast path, `Substring` allocation) |
| [diagrams/mermaid/](diagrams/mermaid/) | Standalone Mermaid source for all 5 diagrams (also embedded inline in article.md) |
| [diagrams/svg/009-cover.svg](diagrams/svg/009-cover.svg) | Chapter cover source (edited from `assets/brand/chapter-cover-template.svg`) |

**Previous:** [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/README.md)
**Next:** [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/README.md)
