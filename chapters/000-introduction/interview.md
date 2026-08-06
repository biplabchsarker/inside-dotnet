# Interview Questions — Welcome to Inside .NET

**Q1: What's the difference between knowing how to use a framework and understanding it internally? Give an example of when that difference matters in production.**
A: Using a framework means knowing its API surface — how to call it correctly. Understanding it internally means knowing *why* it behaves the way it does under load, failure, or edge cases. Example: a developer who knows DI syntax can register a `DbContext` as a singleton and have it compile and even work in dev. A developer who understands the DI container's lifetime model knows that's a production-breaking mistake because `DbContext` is not thread-safe and a singleton is shared across concurrent requests.

**Q2: Why do senior engineering interviews frequently ask about GC, the CLR, or thread pool internals instead of just asking candidates to write code?**
A: Because writing correct code under a spec is a mid-level skill; diagnosing *why* correct-looking code misbehaves in production (memory growth, latency spikes, deadlocks) requires a mental model of the runtime. Interviewers use these questions as a proxy for "can this person operate independently when something goes wrong in a system they didn't build."

**Q3: How would you explain the value of "internals knowledge" to a junior developer who just wants to ship features?**
A: Frame it as leverage, not homework: understanding the runtime doesn't slow feature delivery, it shortens debugging time, prevents entire classes of production incidents, and is exactly what separates a developer who ships features from an architect who's trusted to make system-level decisions.
