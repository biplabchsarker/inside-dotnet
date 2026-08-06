# Inside .NET — Episode 5: Assemblies, DLLs & Metadata

*Part I — The Foundation*

Most developers treat a `.dll` as "the compiled version of my project" and move on. That's not wrong, but it skips the part that actually matters architecturally: an assembly is the CLR's unit of *versioning* and *type identity* — not a file extension.

## The book analogy

A book's ISBN is a globally unique identifier — no other book, not even a different edition of the same book, reuses it (an assembly's identity: name + version + culture + public key token). The table of contents lets you jump directly to a chapter by name instead of scanning the whole book (metadata tables — TypeDef, MethodDef — indexed, not linearly searched). The bibliography lists every book this one cites precisely enough to fetch the *exact* edition (the AssemblyRef table, pinned to a specific version). A footnote marker doesn't repeat the full citation inline — it's a compact pointer resolved on demand (a metadata token embedded in IL).

## What's actually inside the file

A .NET assembly is a valid Windows PE/COFF file — the same container format native `.exe`/`.dll` files use, reused deliberately so decades of OS-level tooling could recognize its basic shape. What makes it a *.NET* assembly is a CLR header pointing at a metadata root, which fans out into streams (`#~`, `#Strings`, `#US`, `#Blob`) backing the actual metadata tables: `TypeDef`/`MethodDef`/`FieldDef` for what's defined here, `TypeRef`/`MemberRef`/`AssemblyRef` for what's referenced elsewhere.

## Tokens, not names

Every `call` or `newobj` in IL doesn't embed a name — it embeds a 4-byte metadata token. The top byte says which table; the rest is a row index. Resolving it is an indexed lookup, not a string search, which is why IL can reference thousands of members across dozens of assemblies without paying a name-resolution cost at every use.

## Identity is a 4-tuple, not a file name

Two assemblies named `Contoso.Utils`, differing only in version, are entirely unrelated identities to the CLR. A type from one isn't assignable to the "same" type from the other — because type identity is `(assembly identity, type name)`, never type name alone. This is the actual mechanism behind most "works in one project, throws `FileLoadException` in another" bugs.

## How a reference actually gets resolved

The CLR binder asks the current `AssemblyLoadContext` to resolve a requested identity. For a standard deployment, that means consulting `deps.json`, then probing — app directory, shared framework, NuGet cache — in order, until a file's actual on-disk identity matches the request. A name match with a mismatched version doesn't count. Plugin hosts exploit this by giving each plugin its own `AssemblyLoadContext`, letting two versions of "the same" dependency coexist without collision.

## Reflection is metadata, queried live

`Assembly`, `Type`, and `MethodInfo` aren't a separate description of your code — they're a managed API over the exact metadata tables the loader and JIT already read. `Type.MetadataToken` is the same token IL uses internally. This is also why reflection has a real per-call cost relative to statically resolved code, and why Native AOT's trimming — which removes metadata for types static analysis can't prove reachable — breaks unbounded reflection by design, not by accident.

## Try it yourself

The [companion code sample](code/Chapter04.Demo/Program.cs) reads the executing assembly's own manifest — name, version, referenced assemblies — enumerates its types and one type's methods with their metadata tokens, then resolves a token back to a method to prove the mechanism is real, not just described.

*Next: [Episode 6 — Stack vs Heap](../005-stack-vs-heap/README.md)*
