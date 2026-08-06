# Summary — Understanding the CLR

- The CLR is a **managed execution environment**, not just a JIT wrapper: type safety, memory management, structured exception handling, security, thread management, and interop all live inside it.
- The **CTS (Common Type System)** is the single type system every .NET language's compiler maps onto — this is *why* C#, F#, and VB.NET assemblies interoperate with zero glue code.
- The **CLS (Common Language Specification)** is a narrower subset of CTS rules that, when followed, guarantees a public API is safely consumable from *any* CLS-compliant language.
- Every object's header carries a pointer to a **method table** — a per-*type* structure (built once, shared by every instance) containing a **vtable**: fixed-index function-pointer slots for virtual dispatch.
- A **virtual call** follows the method-table pointer and indexes into the vtable at run time (resolves against the *runtime* type); a **non-virtual call** is resolved at JIT time to a direct, potentially inlined address.
- **AppDomains** (.NET Framework: isolation + unloadability) have been replaced in modern .NET by **`AssemblyLoadContext`** — lighter-weight load isolation, with optional collectibility for unloading.
- **BCL** (Base Class Library) is the current term for the foundational `System.*` types; **FCL** (Framework Class Library) is the older, broader historical term — BCL plus everything else shipped in .NET Framework.

**Previous:** [Episode 2 — What Really Happens When You Run a .NET Application?](../001-execution-flow/article.md)
**Next:** [Episode 4 — JIT Compilation Explained](../003-jit-compilation/article.md)
