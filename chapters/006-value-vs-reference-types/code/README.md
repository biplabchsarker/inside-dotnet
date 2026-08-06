# Chapter 006 — Code Sample

`Chapter06.Demo` is a minimal .NET 10 console app demonstrating:
- Value type copy semantics (mutating a copy leaves the original untouched)
- Reference type sharing (mutating through one reference is visible through every other reference to the same object)
- The mutable-struct gotcha — the compiler blocks the obvious `foreach` mutation (`CS1654`), but mutating a struct copy returned by a method/property is a silent no-op
- `readonly struct` passed with `in` to avoid copying a large struct across a call boundary
- `record class` vs `record struct` — identical generated value equality, different copy/storage semantics underneath

## Run it

```bash
cd Chapter06.Demo
dotnet run
```

## Expected output

```
=== Inside .NET: Episode 7 demo — Value Types vs Reference Types ===

--- 1. Value type: copy by value ---
a = (1, 2)
b = (99, 2)

--- 2. Reference type: copy by reference ---
c1.Name = Bob
c2.Name = Bob
ReferenceEquals(c1, c2) = True

--- 3. Mutable struct gotcha ---
After mutating a copy returned by a method:
  (1, 1)
  (2, 2)
  (3, 3)
After explicit index rewrite:
  (999, 1)
  (999, 2)
  (999, 3)

--- 4. readonly struct passed by `in` ---
Sum via in-parameter (no copy): 36

--- 5. record class vs record struct ---
rc1 == rc2                 : True
ReferenceEquals(rc1, rc2)   : False
rs1 == rs2                 : False
rs1                        : PointRS { X = 5, Y = 6 }
rs2                        : PointRS { X = 42, Y = 6 }

=== Done ===
```

To confirm the guarded mistake for yourself, uncomment the `foreach (MutablePoint p in points) { p.X = 999; }` line in `Program.cs` — it fails to compile with `CS1654: Cannot modify members of 'p' because it is a 'foreach iteration variable'`.
