# Component Library

Reusable diagram icons, per [VISUAL_LANGUAGE.md](../../VISUAL_LANGUAGE.md). Each SVG is a self-contained `0 0 64 64` viewBox symbol using only [DESIGN_SYSTEM.md](../../DESIGN_SYSTEM.md) colors, ready to `<use>` in hand-authored SVG diagrams, or to reference by name/color when naming Mermaid nodes.

## Built (v2 — 32 components)

| File | Concept |
|---|---|
| `clr.svg` | CLR / runtime engine |
| `gc.svg` | Garbage Collector |
| `jit.svg` | JIT compiler |
| `roslyn.svg` | Roslyn (C# compiler) |
| `il.svg` | IL (Intermediate Language) |
| `assembly.svg` | Assembly |
| `metadata.svg` | Metadata / manifest |
| `type-loader.svg` | Type Loader |
| `cts.svg` | Common Type System |
| `cls.svg` | Common Language Specification |
| `assembly-load-context.svg` | AssemblyLoadContext |
| `exception-handler.svg` | Exception handling |
| `security.svg` | Security / code access |
| `stack.svg` | Call stack |
| `heap.svg` | Managed heap |
| `object.svg` | Heap object |
| `boxed-value.svg` | Boxed value type |
| `loh.svg` | Large Object Heap |
| `pinned-object.svg` | Pinned object |
| `finalizer.svg` | Finalizer |
| `weak-reference.svg` | Weak reference |
| `thread.svg` | Thread |
| `thread-pool.svg` | Thread pool |
| `task.svg` | Task |
| `await.svg` | async/await |
| `cancellation-token.svg` | CancellationToken |
| `synchronization-context.svg` | SynchronizationContext |
| `cpu.svg` | CPU core |
| `server.svg` | Server / host |
| `database.svg` | Database |
| `cloud.svg` | Cloud (generic) |
| `docker.svg` | Container / Docker |

## Pending (⏳ in VISUAL_LANGUAGE.md — draw on demand)

C# language (Delegate, Event, Generic, Reflection, Expression Tree, Record, Pattern Match), Patterns & DI (Singleton, Factory, Builder, Strategy, Observer, Adapter, Decorator, Facade, Repository, Unit of Work, DI Container, Service Provider, Options Pattern), ASP.NET Core / EF Core (Middleware, Controller, Minimal API, Kestrel, DbContext, LINQ, Change Tracker, Migration), Infrastructure (Cache, Redis, Message Queue, RabbitMQ, Load Balancer, SQL Server, Postgres, MongoDB), Cloud (Azure, Kubernetes, Container, Microservice, OpenTelemetry), Data flow (Request, Response, Pipeline stage, Arrows), Status (Success, Warning, Error, Info).

## Adding a new component

1. Check this table first — reuse before redrawing.
2. Author at `0 0 64 64`, 2px stroke, rounded joins, palette colors only.
3. Add a row to this table AND move it from ⏳ to ✅ in `VISUAL_LANGUAGE.md`'s catalog table, in the same change.
