# Inside .NET — Episode 1: Welcome to Inside .NET

*Understanding what really happens under the hood — a visual journey from developer to solution architect.*

You've probably written thousands of lines of C#. You know how to spin up a Web API, wire up dependency injection, query a database with EF Core, and ship it to production. That's real skill — but it's also the part of .NET that's *visible*.

Underneath every `dotnet run`, the runtime is compiling your code twice, deciding where your objects live, cleaning up memory you never explicitly freed, and juggling thousands of logical threads on a handful of physical cores. Most of the time this machinery is invisible — until a service leaks memory in production at 2 AM, or a DI misconfiguration corrupts data across requests, or a hot path GCs itself into double-digit latency.

**Inside .NET** is a new series that opens the hood. Not a tutorial on *how to type the code* — a guided tour of *why the code behaves the way it does*.

## The analogy

Millions of people drive a car skillfully without knowing what a camshaft does — until the car stutters on a hill or won't start in the cold. At that point, "I know how to drive" isn't enough anymore. .NET developers are in the same position: you know how to drive the framework. This series is about the engine.

## What the series covers

Eleven parts, fifty-plus chapters — Foundation, Memory, C# & Clean Code, Design Patterns, Dependency Injection, ASP.NET Core, Concurrency, Entity Framework Core, Architecture, Cloud & Production, and Becoming an Architect. Each chapter builds on the last, moving from "what is the CLR" all the way to "how do you design a microservices system for high availability."

## The format

Every chapter follows the same shape: a real-world analogy, the problem being solved, original diagrams built specifically for this series, the internal .NET mechanics, production-quality C# code, common mistakes, performance considerations, senior-level interview questions, key takeaways, and a bridge to the next chapter.

## Why this, why now

There's no shortage of .NET tutorials. There's a real shortage of content that treats the runtime's internals as a first-class, teachable subject — connected to real production failures and real interview questions, not just documentation.

Episode 2 opens with the foundation everything else depends on: what actually happens, step by step, from `dotnet run` to your first line of code executing.

*Next: [Episode 2 — What Really Happens When You Run a .NET Application?](../001-execution-flow/medium.md)*
