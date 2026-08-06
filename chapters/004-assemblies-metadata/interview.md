# Interview Questions — Assemblies, DLLs & Metadata

**Q1: What, precisely, makes two assemblies "the same" to the CLR — and what doesn't?**
A: Identity is the 4-tuple: simple name, version, culture, and public key token (if strong-named). Two assemblies matching on simple name alone but differing in any of the other three are different identities — the CLR does not treat them as interchangeable, will not silently substitute one for the other, and a type from one is not assignable to the "same" type from the other. File path, file name, or byte-for-byte content similarity is irrelevant to identity.

**Q2: How does IL reference a method defined in another assembly without embedding its full name at every call site?**
A: Via a metadata token — a 4-byte value where the top byte identifies the metadata table (e.g., `MemberRef` for an externally-defined member) and the rest is a row index. That row references a `TypeRef` (which assembly/type it belongs to) plus a name and signature stored once in the `#Strings`/`#Blob` streams. Resolving the token is an indexed table lookup, not a string search, and the name/signature is stored exactly once regardless of how many call sites reference it.

**Q3: Walk through what happens when your code references an assembly that isn't loaded yet.**
A: The CLR binder asks the current `AssemblyLoadContext` to resolve the requested identity (name, version, culture, public key token). For the default framework-dependent case, it consults the app's `deps.json` for the expected version and path, then probes the app base directory, the shared framework directories, and the NuGet package cache in order. The first file whose actual on-disk manifest identity matches the request is loaded into that `AssemblyLoadContext`; a name match with a mismatched version/key does not satisfy the reference.

**Q4: Why would you deliberately load two different versions of the same-named assembly in one process, and how?**
A: Plugin-style hosting, where independently-built plugins may depend on different versions of a shared library and you can't force them to agree. You load each plugin (and its dependency closure) into its own custom `AssemblyLoadContext` instead of the shared `Default` context — each ALC resolves its own references independently, so two ALCs can each hold a different version of "the same" assembly name without the loader treating that as a conflict, because identity resolution is scoped per-ALC.

**Q5: Is strong naming a security feature?**
A: Not in the way people often assume. It proves the assembly's metadata hasn't changed since it was signed with a given private key, and it lets the public key token participate in identity so two publishers can use the same simple name without collision. It does not vouch for the trustworthiness of the code inside, isn't a sandboxing mechanism, and isn't equivalent to code-signing/Authenticode, which addresses a different trust question (who published this file) at the OS/publisher level rather than the CLR-identity level.
