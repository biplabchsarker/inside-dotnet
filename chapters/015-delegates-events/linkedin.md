# What Really Happens When You Use a C# Delegate? ⚡

Most .NET developers think of delegates as "just function pointers." But in the CLR, a delegate is a heap-allocated managed object with serious architectural implications.

In Episode 16 of Inside .NET, we dissect:
🔍 `System.MulticastDelegate` under the microscope: `_target`, `_methodPtr`, and `_invocationList`.
⚠️ The **Lapsed Listener** bug: why subscribing to a singleton event can silently leak your entire UI view or service graph.
📦 Compiler display classes: how capturing local variables in a lambda quietly allocates heap memory on every call, and how `static` lambdas eliminate it.
🛡️ Resilient event dispatch: why an unhandled exception in one subscriber stops all subsequent listeners, and how to write a fault-tolerant dispatcher.

Read the full deep dive with benchmarks and diagrams on GitHub:
👉 https://github.com/biplabchsarker/inside-dotnet/tree/main/chapters/015-delegates-events

#DotNet #CSharp #SoftwareArchitecture #Performance #MemoryManagement
