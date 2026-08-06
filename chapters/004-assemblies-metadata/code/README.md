# Chapter 004 Demo — Assemblies, DLLs & Metadata

## Run it

```bash
cd code/Chapter04.Demo
dotnet run
```

Requires the .NET 10 SDK.

## What it does

`Program.cs` uses `System.Reflection` to inspect the executing assembly's own manifest and metadata, entirely at run time:

1. **Assembly identity** — prints the simple name, version, culture, public key token (if strong-named), full identity string, and on-disk location, read from `Assembly.GetName()` / `Assembly.FullName`.
2. **Referenced assemblies** — enumerates the `AssemblyRef` table via `Assembly.GetReferencedAssemblies()`, listing every dependency's name and version.
3. **Defined types** — enumerates the `TypeDef` table via `Assembly.GetTypes()`, printing each type's full name and metadata token.
4. **Methods on a type** — enumerates `MethodDef` rows for a small demo type (`MetadataDemo`) via `Type.GetMethods()`, printing each method's metadata token alongside its declaring type's token.
5. **Token resolution round-trip** — takes one method's `MetadataToken` and resolves it back to a `MethodBase` via `Module.ResolveMethod(int)`, demonstrating that the token is a stable, reusable handle — the same mechanism IL itself uses at `call`/`newobj` sites.

## Expected output shape

Exact version numbers, tokens, and the referenced-assembly list will vary by SDK version and build configuration, but the shape is:

```
=== Inside .NET: Episode 5 demo — Assembly manifest & metadata ===

-- Assembly identity (the manifest, decoded) --
Simple name     : Chapter04.Demo
Version         : 1.0.0.0
Culture         : neutral
Public key token: (none — not strong-named)
Full identity   : Chapter04.Demo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
On-disk location: <path>\Chapter04.Demo.dll

-- Referenced assemblies (AssemblyRef table) --
  System.Linq                    v...
  System.Private.CoreLib         v...
  System.Runtime                 v...
  (N referenced assemblies total)

-- Types defined in this assembly (TypeDef table) --
  <Program>$  [token: 0x02000002]
  MetadataDemo  [token: 0x02000003]
  ...

-- Methods on MetadataDemo (MethodDef table), with tokens --
  Add                  token=0x06000... declaring type token=0x02000...
  Describe             token=0x06000... declaring type token=0x02000...
  NoOp                 token=0x06000... declaring type token=0x02000...

-- Resolving a metadata token back to a member --
Token 0x06000... resolves to: Add
Same member as original lookup: True
```
