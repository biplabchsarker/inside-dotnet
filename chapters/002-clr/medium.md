# Inside .NET — Episode 3: Understanding the CLR

*Part I — The Foundation*

Episode 2 treated the CLR as a black box: it loads assemblies, hands methods to the JIT, and provides some runtime services underneath. That's true, but it undersells what the CLR actually is — a full managed execution environment, closer to a virtual machine than a runtime library bolted onto a compiler.

## The air traffic control analogy

Think of the CLR as air traffic control, not the planes. Airlines — C#, F#, VB.NET — all file flight plans in the same standardized format (CTS-conformant IL). Every aircraft broadcasts a standardized transponder record (every object carries a method-table pointer describing its type). ATC doesn't re-verify an aircraft's whole design on every radio call — it looks up a known profile and applies the right procedure (a vtable lookup resolves a virtual call without re-inspecting the whole type). And ATC enforces the same safety rules on every flight, regardless of airline — the CLR enforces type safety and memory safety uniformly, regardless of which language emitted the IL.

## Why this needs to exist at all

Three real problems collapse into "why have a runtime like this":

- **Cross-language interop** — without one shared type system, calling F# from C# would need hand-written adapters, the way calling between genuinely different platforms always has.
- **Memory and type safety without manual bookkeeping** — a raw native binary has no concept of "this pointer is a live `Order` object." The CLR tracks exact types and live references, which is what makes GC, safe casts, and bounds-checked arrays possible at all.
- **Fast, correct dynamic dispatch** — deciding which override runs, on every polymorphic call, has to be both correct and cheap. The method table / vtable design is the CLR's answer.

## CTS and CLS — not the same thing

The **CTS (Common Type System)** is the one type system every .NET language's compiler maps onto. A C# `class`, an F# reference type, and a VB.NET `Class` are literally the same construct in metadata once compiled — there's no language tag. That's why a C# project can reference an F# library and call into it directly.

The **CLS (Common Language Specification)** is narrower: a subset of CTS rules that, if your public API follows them, guarantees it's consumable from *any* CLS-compliant language — not just the one you wrote it in. This still matters directly the moment you publish a NuGet package meant for multi-language consumption.

## What a loaded type actually looks like in memory

When the type loader resolves a type for the first time, it builds a **method table**: a per-*type* structure holding a pointer back to metadata, size/layout info, a pointer to the parent's method table, interface maps, and a **vtable** — function-pointer slots for every virtual method, at a fixed index shared by every override in the hierarchy.

Every object instance carries, as the first word(s) of its header, a pointer to *that one shared table* — not a copy of its type's behavior. A million `Order` instances share one method table; each instance is just field data plus a pointer.

## Virtual dispatch vs. non-virtual dispatch

A `callvirt` on a virtual method resolves against the **runtime type**, not the declared type: follow the object's method-table pointer, index into the fixed vtable slot, jump to whatever compiled address sits there — decided at run time, every call. That indirection is small but real, and it blocks inlining, which is often the bigger cost.

A non-virtual call — static, sealed, or otherwise unoverridable — is resolved to a direct address at JIT time, and the JIT is free to inline it away entirely. Modern RyuJIT also does *guarded devirtualization*: speculatively inlining a virtual call site that's almost always hit by one concrete type, with a fallback check — you get most of the win without giving up polymorphism.

## AppDomains, then and now

.NET Framework isolated code within a process using **AppDomains** — independently unloadable, each with its own static state. .NET Core dropped multi-AppDomain support entirely: one non-unloadable AppDomain per process, always. In its place, **`AssemblyLoadContext`** provides load isolation — including loading multiple versions of the same assembly side by side — and, if collectible, can be unloaded. This is the actual mechanism behind every modern .NET plugin system.

## BCL vs. FCL — get this one right

The **BCL (Base Class Library)** is the foundational, always-present `System.*` surface — `Object`, `String`, core collections, `System.IO` basics. The **FCL (Framework Class Library)** was the older, broader term for BCL plus everything else — ASP.NET, WinForms, WPF — shipped in .NET Framework. "BCL" is the term in current use; "FCL" reads as dated outside of historical context.

## Try it yourself

The [companion code sample](code/Chapter02.Demo/Program.cs) demonstrates method-table sharing across instances of the same type, virtual-vs-non-virtual call resolution, and live inspection of `AssemblyLoadContext.Default`.

*Next: [Episode 4 — JIT Compilation Explained](../003-jit-compilation/medium.md)*
