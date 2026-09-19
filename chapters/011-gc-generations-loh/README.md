# Chapter 011 — GC Generations & the Large Object Heap

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
| [code/](code/) | Two .NET 10 projects: `Chapter11.Demo` (segment/heap growth via `GC.GetGCMemoryInfo()`, the exact LOH size-threshold crossover, a card-table proxy measurement, LOH fragmentation + `CompactOnce`, and the Pinned Object Heap) and `Chapter11.Benchmarks` (real `BenchmarkDotNet` measurements: LOH vs. equivalent-total-size small-object allocation, fragmented vs. compacted full-collection cost, and the card-table proxy across Gen 2 graph sizes with a fixed dirty set) |
| [diagrams/mermaid/](diagrams/mermaid/) | Standalone Mermaid source for all 5 diagrams (also embedded inline in article.md) |
| [diagrams/svg/011-cover.svg](diagrams/svg/011-cover.svg) | Chapter cover source (edited from `assets/brand/chapter-cover-template.svg`) |

**Previous:** [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/README.md)
**Next:** [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/README.md) *(not yet written)*
