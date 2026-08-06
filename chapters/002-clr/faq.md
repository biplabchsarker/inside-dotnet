# FAQ — Understanding the CLR

**Q: Is the CLR the same thing as a virtual machine, like the JVM?**
A: Functionally, yes in the sense that both are managed execution environments sitting between portable intermediate code and the real CPU, providing memory management, type safety, and a common execution model. They differ in details (the CLR's multi-language CTS design vs. the JVM's original single-language-first design, verification model, GC implementations), but the "managed execution environment" framing is accurate for both.

**Q: If C#, F#, and VB.NET all compile to CTS-conformant IL, does that mean I can inherit a C# class from F#?**
A: Yes — that's precisely the point of the CTS. A class, interface, or abstract type defined in one CTS-conformant language is just a CTS type once compiled; any other CTS-conformant language's compiler can consume, extend, or implement it without special-casing which language originally produced it.

**Q: Does every object really carry a pointer to its method table, even a simple `int`?**
A: A boxed `int` does — boxing promotes a value type onto the heap specifically by wrapping it in an object header (method table pointer + sync block) plus the value's data. An *unboxed* `int` living on the stack or inline in another object has no object header at all; it's just the raw bits, which is part of why value types avoid this overhead when they don't need reference semantics.

**Q: Can I still use AppDomains in .NET 8/9/10?**
A: The `AppDomain` API surface still compiles for source compatibility, but modern .NET only ever runs one, non-unloadable AppDomain per process — calls like `AppDomain.CreateDomain` don't give you real isolation or unloadability anymore. `AssemblyLoadContext` is the actual mechanism for load isolation and unloading in current .NET.

**Q: Is "FCL" ever the technically correct term to use instead of "BCL"?**
A: Only when you're specifically talking about the full .NET Framework-era library surface (BCL + ASP.NET + WinForms + WPF, etc.) as a historical whole. For the foundational `System.*` types on any current .NET version, "BCL" is the accurate and currently-used term.
