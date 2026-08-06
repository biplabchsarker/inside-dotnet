Every .NET object has a hidden field you never declared.

It's a pointer, sitting in the object header, right before your actual data — pointing to something called a method table.

That one pointer is *why*:
→ A million instances of the same class don't each carry a copy of its methods — they all share ONE method table.
→ Calling an overridden method actually works — the CLR follows that pointer, looks up a fixed vtable slot, and jumps to whatever override is really there. At run time. Every single call.
→ A non-virtual call skips all of that — it's resolved at JIT time, straight to an address, and the JIT can even inline it away completely.

This is also why C#, F#, and VB.NET can call each other's code with zero adapter layers: they all compile down to the SAME type system — the CTS (Common Type System). It's not "similar." It's identical at the metadata level. The CLR doesn't know or care which language produced the IL it's running.

And the isolation model changed too:
→ .NET Framework used AppDomains — full isolation, independently unloadable, multiple per process.
→ Modern .NET dropped that. One AppDomain per process, period. Load isolation and unloading now happen through AssemblyLoadContext instead — lighter weight, and it's the real mechanism behind every plugin system you've used in .NET Core/5+.

This is Episode 3 of Inside .NET — going one level deeper than "the CLR runs your code." Full breakdown with diagrams and runnable code (including a live method-table-sharing check and a virtual-vs-non-virtual timing comparison) in the comments.

#dotnet #csharp #clr #softwarearchitecture
