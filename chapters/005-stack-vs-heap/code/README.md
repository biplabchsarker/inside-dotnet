# Chapter 05 Demo — Stack vs Heap

## Run it

```bash
cd Chapter05.Demo
dotnet run
```

Requires the .NET 10 SDK (`dotnet --version` should report a `10.x` SDK).

## What it demonstrates

1. **A struct as a local variable** — `Point3D` is created and copied purely on the stack. `GC.GetTotalMemory` before/after a million-iteration loop of transient struct locals shows little to no lasting heap growth.
2. **The same struct as a field of a class instance** — `PointHolder` wraps a `Point3D`. Creating a million `PointHolder` instances shows clear, measurable heap growth for the *same* struct type used in Part 1 — proving that a value type's storage location depends on its container, not its type category.
3. **Boxing** — assigning a `Point3D` to an `object` allocates a heap wrapper around it, a third mechanism (besides "field of a class") that puts a value type on the heap.
4. **StackOverflowException** — the recursive trigger is left commented out in `Program.cs` on purpose. Uncommenting it and running will crash the process immediately and unrecoverably (by design — see the chapter's article.md for why it can't be caught). Only uncomment it in a disposable terminal/process if you want to observe it directly.

## Expected output shape

```
=== Inside .NET: Episode 6 — Stack vs Heap demo ===

[Local struct] point.X=1 (unchanged), copy.X=99
[Struct locals]      heap before:      XXX,XXX bytes | after:      XXX,XXX bytes | delta:       XX,XXX bytes  (sink=...)
[Struct-in-class]    heap before:      XXX,XXX bytes | after:   XX,XXX,XXX bytes | delta:   XX,XXX,XXX bytes
Held 1,000,000 PointHolder instances, each carrying a Point3D field inline.
Sample check: holders[500000].Point.X = 500000

[Boxing] boxed object type: Point3D — this Point3D now lives on the heap, wrapped.

Done. (Uncontrolled recursion demo intentionally left disabled — see comment above.)
```

The exact byte counts vary by run and machine, but the pattern is consistent: the struct-locals delta stays small/flat, while the struct-in-class delta grows by roughly (object header + field sizes) × 1,000,000 — direct, observable evidence that the struct's heap presence comes from its container, not from being a struct.
