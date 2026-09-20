# Chapter 013 — Memory Leaks in a Managed World

Part II — Memory

| File | Purpose |
|---|---|
| [article.md](article.md) | Full canonical chapter (13-section template; 3 diagrams, mechanics, code, Architect's Perspective, interview Q&A, quiz) |
| [summary.md](summary.md) | TL;DR |
| [faq.md](faq.md) | FAQ |
| [interview.md](interview.md) | Interview Q&A |
| [quiz.md](quiz.md) | Self-check quiz |
| [exercises.md](exercises.md) | Hands-on exercises + Challenge |
| [references.md](references.md) | Further reading |
| [linkedin.md](linkedin.md) | LinkedIn-optimized version |
| [medium.md](medium.md) | Medium-optimized version |
| [devto.md](devto.md) | Dev.to-optimized version |
| [code/](code/) | Four .NET 10 projects: `Example` (the Lapsed Listener leak), `Advanced` (fixing it with `IDisposable` and explicit `-=`), `Production` (a `WeakEventManager` built on `WeakReference<T>`), and `Performance` (real measured memory retained by a safe vs. leaky subscriber after a forced GC) |
| [diagrams/mermaid/](diagrams/mermaid/) | Standalone Mermaid source for all 3 diagrams (also embedded inline in article.md) |
| [diagrams/svg/](diagrams/svg/) | Signature-illustration SVG sources (light-isometric interim placeholders; see [IMAGE_GUIDE.md](../../IMAGE_GUIDE.md)) |

**Previous:** [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/README.md)
**Next:** Episode 15 — OOP Fundamentals *(not yet written; opens Part III — C#)*
