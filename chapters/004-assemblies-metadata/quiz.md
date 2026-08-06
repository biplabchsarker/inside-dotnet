# Self-Check Quiz — Assemblies, DLLs & Metadata

1. What is the actual 4-tuple that constitutes an assembly's identity to the CLR?
2. What is a metadata token, and why does IL use one instead of embedding a fully qualified name at each reference?
3. Name two metadata tables that describe things *defined* in an assembly, and two that describe things *referenced* from elsewhere.
4. In order, what does the CLR consult and probe to resolve a referenced assembly in a standard framework-dependent deployment?
5. Does strong naming protect or encrypt an assembly's IL code? What does it actually guarantee?

<details>
<summary>Answers</summary>

1. Simple name, version, culture, and public key token (empty/absent if not strong-named).
2. A metadata token is a 4-byte value whose top byte identifies a metadata table and remaining bytes index a row in it; IL uses tokens instead of names because resolving a token is an O(1) indexed lookup, and the name/signature is stored once in the metadata streams rather than repeated at every call site.
3. Defined here: `TypeDef`, `MethodDef` (also `FieldDef`). Referenced elsewhere: `TypeRef`, `MemberRef` (also `AssemblyRef` for the owning assembly's identity).
4. `deps.json` (for the expected version/path), then probing in order: the application's base directory, the shared framework directories, and the NuGet package cache — all scoped to the current `AssemblyLoadContext`.
5. It protects neither — it does not encrypt or obfuscate IL. It signs a hash of the assembly's metadata with a private key, proving the assembly hasn't been altered since signing, and lets the public key token participate in assembly identity so two publishers can reuse the same simple name without collision.

</details>
