# Medium Post: IDisposable & Finalizers

If you've written C# for any length of time, you've used the `using` statement. But what is it actually doing under the hood? And more importantly, what happens when you *forget* to use it?

In **Inside .NET Chapter 012**, we tear down the magic of resource cleanup in .NET. We cover:
- The dual-path cleanup pattern (`Dispose` vs Finalization).
- How the **Finalization Queue** and **F-Reachable Queue** work inside the CLR.
- Why an object with a finalizer takes **tens of times longer** (47.65×-73.53× across two measured runs) to collect than a normal object (backed by real `BenchmarkDotNet` data).
- Why the modern best practice is to avoid finalizers entirely and use `SafeHandle`.

Stop blindly adding `~MyClass()` "just to be safe." Learn what the GC is actually doing.

Read the full chapter here: [Link]
