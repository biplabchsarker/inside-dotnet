# LinkedIn Post

Is it possible to have a memory leak in a managed language like C#?

Absolutely. 🚨

The .NET Garbage Collector is incredible, but it only collects *unreachable* objects. It cannot collect *unused* objects.

In **Inside .NET Chapter 013**, we dive into the "Lapsed Listener" problem. If a short-lived UI component subscribes to a long-lived static event, the UI component will **never** be garbage collected unless you explicitly unsubscribe with `-=`. 

We wrote a benchmark to prove it: 
❌ Forgetting to unsubscribe: **977 MB of RAM permanently leaked.**
✅ Using a Safe pattern: **0 MB retained.**

Read the full deep-dive to learn how to track down GC Roots, trace memory through Gen 2, and use `WeakReference` to build foolproof event aggregators.

[Link to Chapter]

#dotnet #csharp #memoryleaks #garbagecollection #softwareengineering
