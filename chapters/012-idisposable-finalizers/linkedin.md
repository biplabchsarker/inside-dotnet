# LinkedIn Post

Ever wondered why the senior dev on your team gets nervous when they see a finalizer (`~MyClass`) in C#? 🚨

The .NET Garbage Collector handles managed memory perfectly, but unmanaged resources (file handles, network sockets) are a different beast. 

I just published **Chapter 012 of Inside .NET: IDisposable & Finalizers**, and we benchmarked exactly what a finalizer costs:
👉 An object with an empty finalizer takes **tens of times longer** (47.65×-73.53× across two measured runs) to collect than a normal object.
👉 Finalizers force objects to survive Gen 0 and get promoted to Gen 1.
👉 They stall the dedicated Finalizer Thread.

Read the full deep-dive into the Finalization Queue, the F-Reachable Queue, and why you should almost always prefer `SafeHandle` over a raw finalizer today.

[Link to Chapter]

#dotnet #csharp #softwareengineering #performance #garbagecollection
