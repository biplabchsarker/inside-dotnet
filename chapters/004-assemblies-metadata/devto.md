---
title: "Inside .NET — Episode 5: Assemblies, DLLs & Metadata"
published: false
tags: dotnet, csharp, clr, softwarearchitecture
---

```csharp
Assembly self = Assembly.GetExecutingAssembly();
AssemblyName name = self.GetName();

Console.WriteLine(name.Name);     // "Chapter04.Demo"
Console.WriteLine(name.Version);  // "1.0.0.0"
```

Nothing exotic here — but that `AssemblyName` object is the entire reason "the same DLL" loaded in two different projects can produce two types that are *not* assignable to each other. Let's dig into why.

## The 4-tuple, not the file name

The CLR does not treat an assembly's simple name as its identity. Identity is a 4-tuple: **name, version, culture, public key token**. Change only the version and leave every byte of IL identical, and the CLR treats the result as a completely different identity. Since type identity in the CLR is `(assembly identity, type name)` — never type name alone — a `Contoso.Utils.Widget` from version `1.0.0.0` is not the same type as a `Contoso.Utils.Widget` from version `2.0.0.0`, even with byte-identical source.

This is the root cause behind most "works in one project, throws `FileLoadException`/`MissingMethodException` in another" reports — two dependency chains pinned different versions of the same simple name, and the runtime is doing exactly what it's supposed to do: refusing to treat them as interchangeable.

## What's actually inside the .dll

A .NET assembly is a real Windows PE/COFF file — the same container format native `.exe`/`.dll` files use, borrowed deliberately so decades of OS-level tooling could parse it without knowing anything about .NET. What makes it a *.NET* assembly is a **CLR header** pointing at a **metadata root**, which fans out into streams (`#~`, `#Strings`, `#US`, `#Blob`, `#GUID`) backing the real metadata tables:

- `TypeDef` / `MethodDef` / `FieldDef` — what's *defined* here
- `TypeRef` / `MemberRef` / `AssemblyRef` — what's *referenced* from elsewhere

## Tokens, not names, at every call site

```csharp
Type demoType = typeof(MetadataDemo);
MethodInfo add = demoType.GetMethod("Add")!;
Console.WriteLine($"0x{add.MetadataToken:X8}");
// e.g. 0x06000004
```

That printed hex value is the *literal* 4-byte operand IL uses for a `call` instruction targeting this method — not a separate reflection-only numbering scheme. The top byte says which metadata table to index into (`0x06` = `MethodDef`); the rest is a row number. Resolving it is an O(1) array index, not a string search — which is why IL can reference thousands of members across dozens of assemblies without a per-call name-lookup tax.

## Resolution is an algorithm, not "find a file with a matching name"

When your code first touches a type from a referenced assembly, the CLR binder asks the current `AssemblyLoadContext` to resolve the requested identity. For a normal framework-dependent deployment: consult `deps.json` for the expected version/path, then probe — app base directory, shared framework directories, NuGet package cache — in that order. A name match with a mismatched version does not satisfy the reference. This is also exactly how plugin hosts load two different versions of "the same" dependency side by side: give each plugin its own `AssemblyLoadContext`, and identity resolution never has to reconcile across the boundary.

## Reflection is metadata, queried live

`Assembly`, `Type`, and `MethodInfo` aren't a separate description of your code layered on top — they're a managed API surface over the exact metadata tables the loader and JIT already read. That's the mechanism, and it's also why reflection has a real per-call cost relative to statically resolved code (it's doing at run time what the JIT already resolved once via a token), and why Native AOT's trimming — which removes metadata for types static analysis can't prove reachable — breaks unbounded `Type.GetType("SomeName")` calls by design.

## Try it yourself

The companion demo in [`code/Chapter04.Demo/Program.cs`](code/Chapter04.Demo/Program.cs) reads its own manifest, enumerates its referenced assemblies, lists its defined types and one type's methods with their real metadata tokens, then resolves a token back to a method to prove the mechanism is real rather than merely described. Full breakdown — five diagrams, the architect-level versioning-strategy discussion, and the interview questions — is in the [full article](article.md).

*This is Episode 5 of Inside .NET, closing Part I — The Foundation. Next up: Episode 6 — Stack vs Heap, opening Part II — Memory.*
