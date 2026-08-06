# Inside .NET — Episode 0: Welcome to Inside .NET

*Understanding what really happens under the hood — a visual journey from developer to solution architect.*

You've probably written thousands of lines of C#. You spin up a Web API, wire up dependency injection, query a database with EF Core, and ship it. That's real skill — but it's also the part of .NET that's *visible*.

Underneath every `dotnet run`, the runtime is compiling your code twice, deciding where your objects live, cleaning up memory you never explicitly freed, and juggling thousands of logical threads on a handful of physical cores. Most of the time this machinery is invisible — until a service leaks memory in production at 2 AM, or a DI misconfiguration corrupts data across requests, or a hot path GCs itself into double-digit latency.

**Inside .NET** is a new series that opens the hood. Not "here's how to type the code" — a guided tour of *why the code behaves the way it does*.

There's no code sample in this episode — it's the roadmap. Episode 2 is where the first real runtime mechanism, and the first runnable C#, shows up. If you're the type who wants to see the code before committing to a series, this is your signal to bookmark and come back for [Episode 2](../001-execution-flow/article.md).

## The analogy

Millions of people drive a car skillfully without knowing what a camshaft does — until the car stutters on a hill or won't start in the cold. At that point, "I know how to drive" isn't enough anymore. .NET developers are in the same position: you know how to drive the framework. This series is about the engine.

## What the series covers

Eleven parts, fifty-plus chapters:

| Part | Focus |
|---|---|
| I — Foundation | CLR, compilation pipeline, assemblies, startup |
| II — Memory | Stack, heap, GC, boxing, LOH, leaks |
| III — C# & Clean Code | OOP, SOLID, DRY/KISS/YAGNI |
| IV — Design Patterns | Creational, structural, behavioral, repository/UoW |
| V — Dependency Injection | Lifetimes, container internals |
| VI — ASP.NET Core | Pipeline, middleware, routing, filters |
| VII — Concurrency | Threads, tasks, async/await, sync context |
| VIII — EF Core | DbContext lifecycle, change tracking, LINQ, performance |
| IX — Architecture | Clean/Onion/Hexagonal, CQRS, DDD, microservices |
| X — Cloud & Production | Containers, observability, caching, messaging |
| XI — Becoming an Architect | Scalability, HA, system design, case studies |

Each part builds on the last — Foundation isn't optional reading, it's the mechanism every later chapter is a more detailed view of.

## The format, every chapter

Cover, learning objectives, a fresh real-world analogy, the problem being solved, original diagrams, the actual runtime mechanics ("Under the Hood"), a four-tier code progression (Example → Advanced → Performance → Production) once code applies, common mistakes that actually show up in code review, a three-tier Architect's Perspective (Developer / Senior / Architect), senior-level interview Q&A, a self-check quiz, and a bridge to the next chapter.

## Why this, why now

There's no shortage of .NET tutorials. There's a real shortage of content that treats the runtime's internals as a first-class, teachable subject — connected to real production failures and real interview questions, not just reference documentation.

Episode 2 opens with the foundation everything else depends on: what actually happens, step by step, from `dotnet run` to your first line of code executing — IL, the CLR, JIT compilation, and the runtime services running underneath your process the whole time.

Follow along if you want to go from "I know how to drive .NET" to "I know how the engine works."

*Next: [Episode 2 — What Really Happens When You Run a .NET Application?](../001-execution-flow/article.md)*
