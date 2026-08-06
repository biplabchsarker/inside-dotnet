# Chapter 007 — Code Sample

`Chapter07.Demo` is a minimal .NET 10 console app demonstrating:
- The allocation fast path's measured throughput, via `GC.GetAllocatedBytesForCurrentThread()` before/after allocating five million small objects
- Construction order — a base class's constructor body runs to completion before the derived class's constructor body starts
- The classic gotcha: a `virtual` method called from a base constructor observes the derived class's fields at their zeroed default, because the derived constructor hasn't run yet

## Run it

```bash
cd Chapter07.Demo
dotnet run
```

## Expected output

Exact timings/byte counts will vary by machine, but the shape is stable:

```
=== Inside .NET: Episode 8 — Object Allocation demo ===

--- 1. Allocation fast-path throughput ---
Allocated 5,000,000 AuditRecord instances in 152.1 ms
Bytes allocated (this thread): 160,000,104  (32.0 bytes/instance)
Approx. time per allocation:   30.4 ns  (sink=12499997500000)

--- 2. Construction order: base before derived ---
  BaseWithLogging: constructor running
  DerivedWithLogging: constructor running

--- 3. Virtual call from base constructor sees uninitialized derived state ---
  RiskyBase ctor sees Describe() = "(uninitialized)"
After full construction, risky.Describe() = "ready"

=== Done ===
```

The line to focus on is `RiskyBase ctor sees Describe() = "(uninitialized)"` — that's `RiskyDerived`'s override of `Describe()` running while `RiskyBase`'s constructor is still executing, before `RiskyDerived`'s own constructor has assigned `_label`. The final line, printed after construction completes, correctly shows `"ready"` — proving the field genuinely was still unset during the base constructor's call, not just theoretically.
