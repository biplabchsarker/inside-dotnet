# Inside .NET: What Really Happens Under the Hood of C# Delegates & Events

Delegates and events are cornerstones of the .NET ecosystem. Whether wiring up GUI button clicks, subscribing to domain events, or writing LINQ queries, developers rely on them daily.

Yet underneath their elegant C# syntax lies a complex CLR machinery involving heap objects, dynamic invocation chains, native function pointers, and compiler-generated display classes.

### In this chapter:
1. **The MulticastDelegate Object**: What fields does the CLR store on the managed heap?
2. **The Lapsed Listener Trap**: Why does subscribing with `+=` hold objects alive indefinitely in the Garbage Collector?
3. **Closures and GC Spikes**: How Roslyn decompiles variable captures, and how `static` lambdas solve the problem.
4. **Resilient Multicast Dispatch**: Why default events fail when one subscriber throws, and how production engines handle it.

Check out the full chapter in the Inside .NET repository:
[Read Chapter 015 on GitHub](https://github.com/biplabchsarker/inside-dotnet/tree/main/chapters/015-delegates-events)
