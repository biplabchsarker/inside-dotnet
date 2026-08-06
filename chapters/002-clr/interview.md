# Interview Questions — Understanding the CLR

**Q1: What, precisely, does the CLR do — beyond "it runs .NET code"?**
A: It's a managed execution environment providing: type safety enforcement, memory management (GC), structured cross-language exception handling, a code-access security model, thread/thread-pool management, and managed/unmanaged interop marshaling — on top of loading assemblies and JIT-compiling IL. "Runs the code" undersells it; it defines the rules under which that code is allowed to run at all.

**Q2: What's the difference between the CTS and the CLS, and why do both need to exist?**
A: The CTS (Common Type System) is the single, complete type system every .NET language's compiler maps its constructs onto — it's what makes a C# class and an F# type the same kind of thing at the metadata level. The CLS (Common Language Specification) is a narrower subset of CTS rules that, if a public API follows them, guarantees that API is consumable from *any* CLS-compliant language, not just the one that authored it. The CTS makes cross-language *execution* possible; the CLS makes cross-language *public API design* safe.

**Q3: How does the CLR resolve a virtual method call at run time, and why is a non-virtual call cheaper?**
A: Every object carries a pointer, in its header, to its type's method table — a structure built once per type containing a vtable: an array of function-pointer slots, one per virtual method, at a fixed index shared by every override in the hierarchy. A virtual call follows that pointer, indexes into the correct slot for the *runtime* type, and jumps to whatever address is there — decided at run time. A non-virtual call (static, sealed, or a method with no override possible) has its target address fixed at JIT time, so it's a direct call the JIT can also choose to inline — no indirection, and no barrier to inlining.

**Q4: Why did .NET Core drop multi-AppDomain support, and what replaced the functionality people actually used AppDomains for?**
A: Most real-world AppDomain usage boiled down to two needs: isolating static state between logically separate units of code, and being able to unload code without restarting the process. `AssemblyLoadContext` addresses both more cheaply — you can load isolated or even duplicate/differently-versioned assemblies into separate ALCs, and a *collectible* ALC can be unloaded — without carrying the heavier cross-domain marshaling and security-boundary machinery AppDomains required, most of which modern .NET's threat model doesn't rely on anyway.

**Q5: What's the actual difference between the BCL and the FCL, and which term should you use today?**
A: The BCL (Base Class Library) is the minimal, always-present set of foundational types — `System.Object`, `String`, core collections, `System.IO` basics. The FCL (Framework Class Library) was the older, broader term covering the BCL plus everything else Microsoft shipped on top of it (ASP.NET, WinForms, WPF) in the .NET Framework era. Current Microsoft documentation and the community use "BCL"; "FCL" is a legacy term you'll see in older material or hear as interview trivia, not in active current usage.
