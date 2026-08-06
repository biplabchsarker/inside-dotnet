# Chapter 002 — Code Sample

`Chapter02.Demo` is a minimal .NET 10 console app demonstrating:
- CTS in action: value types vs. reference types reported via reflection
- Method-table sharing: two instances of the same type share one `RuntimeTypeHandle`
- Virtual dispatch resolving against the runtime type vs. non-virtual dispatch that never varies
- An illustrative timing comparison between virtual (interface) dispatch and a direct, inlinable call
- Live inspection of `AssemblyLoadContext.Default` — the modern replacement for AppDomains

## Run it

```bash
cd Chapter02.Demo
dotnet run
```

Expected output shape (numbers vary by machine):
```
=== Inside .NET: Episode 3 demo — CLR internals ===

-- CTS: value types vs. reference types --
Int32          IsValueType=True   BaseType=System.ValueType
OrderStruct    IsValueType=True   BaseType=System.ValueType
OrderClass     IsValueType=False  BaseType=System.Object

-- Method table sharing across instances --
order1 type handle == order2 type handle : True
(Both instances point at the SAME method table; only field data differs.)

-- Virtual dispatch: declared type vs. runtime type --
Declared type: Base, Runtime type: Derived
b.Describe() -> "Derived.Describe (vtable slot overridden)"  (vtable slot resolves to Derived's override)
nv.Describe() -> "NonVirtualBase.Describe (not virtual — no vtable involved)"  (non-virtual: always Base's method, no override possible)

-- Virtual vs non-virtual call cost (illustrative) --
Virtual (interface) dispatch :    412 ms  (sum=2400000000)
Non-virtual / inlinable call :     98 ms  (sum=1600000000)

-- AssemblyLoadContext (modern AppDomain replacement) --
Default ALC name     : Default
Is collectible        : False
Loaded assemblies (first 5):
  - System.Private.CoreLib
  - Chapter02.Demo
  - ...
```

The virtual-vs-non-virtual timing gap will vary significantly by machine and JIT tiering state — the point isn't the exact numbers, it's that a measurable gap exists and is explainable by the mechanism described in [article.md](../article.md).
