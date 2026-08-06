# Exercises — Understanding the CLR

These build directly on the [companion code sample](code/Chapter02.Demo/Program.cs). Do them in order — each one uses a fact established by the previous one.

## Exercise 1 — Prove method-table sharing yourself

Starting from the demo's `OrderClass`, create 10,000 instances in a loop and, for each one, capture `GetType().TypeHandle.Value`. Store the distinct values in a `HashSet<IntPtr>`.

- What do you expect the set's `Count` to be, and why?
- Now do the same for 10,000 instances split across two *different* classes (`OrderClass` and a new `InvoiceClass` with the same shape). What's the set's `Count` now, and what does that tell you about what a method table pointer actually identifies — the type, or the instance?

## Exercise 2 — Measure devirtualization, don't assume it

The chapter's demo uses `Stopwatch` for an illustrative timing comparison. Replace it with a proper [BenchmarkDotNet](https://benchmarkdotnet.org/) benchmark class with two `[Benchmark]` methods: one calling `IShape.Area()` through an array of mixed `Circle`/`Square` instances (as in the demo), and one calling `Square.DirectArea()` directly on a single `sealed` instance.

- Run it in `Release` configuration. Report the actual ns/op difference — is it in the range the chapter implies (small but real), or did you see something bigger/smaller?
- Add a third benchmark: call `Area()` through the interface, but on an array containing *only* `Square` instances (monomorphic call site). Does RyuJIT's guarded devirtualization narrow the gap versus the mixed-type case? What does that tell you about when "virtual is basically free" is actually a defensible claim?

## Exercise 3 — Build a minimal collectible plugin loader

Write a small console app that:

1. Defines an `IPlugin` interface with a single `string Describe()` method.
2. Compiles (or references, if you'd rather keep it simple) a second assembly containing one class implementing `IPlugin`.
3. Loads that second assembly into a **collectible** `AssemblyLoadContext`, instantiates the plugin via reflection, calls `Describe()`, then unloads the ALC (`Unload()` + drop all references + `GC.Collect()`/`GC.WaitForPendingFinalizers()` in a loop until the ALC reports unloaded).

Confirm the ALC actually unloads (hint: keep a `WeakReference` to the `AssemblyLoadContext` itself and poll `IsAlive`). What's the one thing you have to be careful about with references held by the host app that would prevent the unload from ever completing?

---

**Previous:** [Episode 2 — What Really Happens When You Run a .NET Application?](../001-execution-flow/article.md)
**Next:** [Episode 4 — JIT Compilation Explained](../003-jit-compilation/article.md)
