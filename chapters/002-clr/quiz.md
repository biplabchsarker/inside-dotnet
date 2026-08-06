# Self-Check Quiz — Understanding the CLR

1. Name three responsibilities of the CLR beyond "compiling and running IL."
2. What is the CTS, and why is it the reason C# and F# assemblies can call each other with no adapter code?
3. What does every object carry in its header that makes virtual dispatch possible, and what does that pointer refer to?
4. Why is a non-virtual method call cheaper than a virtual one, mechanically?
5. What replaced AppDomains as the load-isolation mechanism in modern .NET, and what capability does it add that a single AppDomain-per-process model lacks?

<details>
<summary>Answers</summary>

1. Any three of: type safety enforcement, memory management (GC), structured exception handling, security/code-access checks, thread management, managed/unmanaged interop marshaling.
2. The Common Type System is the single type system every .NET language's compiler maps its own constructs onto — a class or type from any CTS-conformant language becomes the same kind of CTS construct in metadata, so the CLR (and any other language) doesn't need to know or care which language originally produced it.
3. A pointer to the type's **method table** — a structure built once per type (shared by every instance of that type) containing type metadata and a vtable of function-pointer slots for virtual methods.
4. A non-virtual call's target address is fixed at JIT time, so it's a direct call with no table lookup — and the JIT can additionally choose to inline it. A virtual call must follow the method-table pointer and index into the vtable at run time, which is resolved against the runtime type and blocks inlining.
5. `AssemblyLoadContext` (ALC). It adds the ability to load isolated or even multiple different versions of the same assembly into separate contexts within one process, and — if the ALC is collectible — unload that code later, which a single non-unloadable AppDomain-per-process model in modern .NET cannot do on its own.

</details>
