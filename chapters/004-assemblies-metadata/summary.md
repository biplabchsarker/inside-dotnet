# Summary — Assemblies, DLLs & Metadata

- An assembly is the CLR's unit of **deployment, versioning, and type identity** — identity is the 4-tuple **(simple name, version, culture, public key token)**, not the file name alone.
- A .NET assembly is a valid **PE/COFF file** (the Windows executable format, parsed the same way cross-platform) with a **CLR header** pointing at a metadata root and, for exes, the entry point token.
- The **assembly manifest** holds the assembly's own identity plus the **`AssemblyRef`** table listing every dependency's identity — the machine-readable "bibliography."
- **Metadata tables** (`TypeDef`, `MethodDef`, `FieldDef` for what's defined here; `TypeRef`, `MemberRef` for what's referenced elsewhere) replace embedded names with **metadata tokens** — compact, indexed handles IL uses at every `call`/`newobj` site.
- **Strong naming** signs the assembly's metadata hash and embeds a public key token, disambiguating identity between publishers — it is *not* a security/sandboxing feature.
- **Assembly resolution** is a defined algorithm: `deps.json` plus ordered probing (app directory → shared framework → NuGet cache), scoped per-`AssemblyLoadContext` — not best-effort file finding.
- **`System.Reflection`** (`Assembly`, `Type`, `MethodInfo`) queries the exact same metadata tables the loader and JIT already use — powerful, but with a real per-call cost relative to statically resolved code.

**Next:** [Episode 6 — Stack vs Heap](../005-stack-vs-heap/README.md)
