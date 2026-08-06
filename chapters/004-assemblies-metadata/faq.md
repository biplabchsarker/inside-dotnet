# FAQ — Assemblies, DLLs & Metadata

**Q: Is a "DLL" the same thing as an "assembly"?**
A: Almost always, in practice, yes — but the terms aren't strictly synonymous. A DLL is a file extension/container convention; an assembly is the CLR's logical unit of identity and versioning. A single assembly can in principle span multiple files via `.netmodule` linking, though this is vanishingly rare in real-world .NET code. A native (non-.NET) DLL is a DLL but not an assembly at all — it has no CLR header or metadata.

**Q: What actually breaks if two loaded assemblies have the same simple name but different versions?**
A: Nothing breaks automatically — the CLR treats them as two entirely separate identities and can load both, each in its own `AssemblyLoadContext` if the hosting code sets that up (plugin scenarios rely on exactly this). What breaks is *your* code if it assumes a type from one version is assignable to or interchangeable with the "same" type from the other — it isn't, because type identity includes assembly identity.

**Q: Does strong naming encrypt or protect my code?**
A: No. It signs a hash of the assembly's metadata with a private key so tampering after signing is detectable, and it lets the public key token participate in identity. It does nothing to obfuscate, encrypt, or protect the IL itself — IL in a strong-named assembly is exactly as readable with a decompiler as IL in an unsigned one.

**Q: Why does `Type.GetType("Namespace.Type")` sometimes return `null` even though the type clearly exists?**
A: Without an assembly-qualified name, `Type.GetType` only searches the calling assembly and `mscorlib`/core libraries by default — it does not search every loaded assembly. If the type lives elsewhere, you need the assembly-qualified form (`"Namespace.Type, AssemblyName"`) with an identity string that actually matches what's loaded, or you need to search `Assembly.GetTypes()` across the assemblies you expect it in yourself.

**Q: Can I inspect an assembly's metadata without writing a reflection-based C# program?**
A: Yes — tools like `ildasm`, `dotnet-ildasm`, ILSpy, or the `System.Reflection.Metadata` low-level API let you read metadata tables directly without loading the assembly into an executing process at all, which is also how tools like Roslyn analyzers and trimmers inspect assemblies without running them.
