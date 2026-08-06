# Chapter 001 — Code Sample

`Chapter01.Demo` is a minimal .NET 10 console app demonstrating:
- Inspecting live CLR/runtime info (`Environment.Version`, `RuntimeInformation.OSDescription`)
- Reading assembly metadata via `System.Reflection`
- Observing JIT warm-up cost by timing a method's first vs. second call

## Run it

```bash
cd Chapter01.Demo
dotnet run
```

Expected output shape (numbers vary by machine):
```
=== Inside .NET: Episode 2 demo ===
CLR version   : 10.0.8
OS description: Microsoft Windows 10.0.26200
Assembly      : Chapter01.Demo, Version=1.0.0.0, ...
Location      : ...\Chapter01.Demo.dll
First call    : 12.437 ms (includes JIT compile)
Second call   : 0.041 ms (native code already cached)
```

To inspect the actual IL emitted for `Fibonacci`, paste the method into [sharplab.io](https://sharplab.io) or run `ilspycmd Chapter01.Demo.dll` after building.
