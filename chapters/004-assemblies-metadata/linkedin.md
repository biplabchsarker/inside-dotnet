Ever wondered why loading "the same DLL" in two different projects can produce two completely different types that aren't assignable to each other?

Here's the mechanism: an assembly's identity to the CLR is NOT its file name. It's a 4-tuple — name, version, culture, and public key token. Change the version number and leave everything else byte-identical, and the CLR treats it as an entirely different identity. Type identity is (assembly identity, type name), not type name alone — which is exactly why "the same" type from two different assembly versions can't be assigned to each other.

Under the hood:
→ A .NET assembly is a real Windows PE/COFF file (borrowed format, parsed the same way cross-platform) with a CLR header pointing at metadata tables.
→ IL never embeds full type/method names at call sites — it embeds compact 4-byte metadata tokens that index directly into tables like TypeDef, MethodDef, and MemberRef. A footnote number, not a full citation, every time.
→ Assembly resolution isn't "find a file with a matching name" — it's deps.json plus an ordered probing algorithm (app directory → shared framework → NuGet cache), scoped per AssemblyLoadContext, which is also how plugin hosts load two versions of the same dependency side by side without collision.
→ Reflection (Type, MethodInfo, Assembly) isn't a separate description of your code — it's a run-time query surface over the exact same metadata tables the loader and JIT already consumed.

This is Episode 5 of Inside .NET — the chapter that explains why "DLL Hell" was ever a real problem, and exactly which mechanism modern .NET uses to prevent it. Full breakdown with diagrams and runnable code in the comments.

#dotnet #csharp #clr #softwarearchitecture
