# Chapter 003 — Code Sample

## Run it

```bash
cd Chapter03.Demo
dotnet run -c Release
```

Run in `Release` configuration — `Debug` builds disable several JIT optimizations globally and will mask the Tier 0 → Tier 1 effect this demo is trying to surface.

To compare against tiering disabled (every method compiles straight to Tier-1-quality code on first call, no promotion step):

```bash
# bash / Linux / macOS
DOTNET_TieredCompilation=0 dotnet run -c Release

# PowerShell
$env:DOTNET_TieredCompilation = "0"; dotnet run -c Release
```

## Expected output shape

```
=== Inside .NET: Episode 4 demo — Tiered Compilation in action ===

-- RuntimeFeature flags --
IsDynamicCodeSupported : True
IsDynamicCodeCompiled  : True

-- Observing Tier 0 -> Tier 1 promotion via batch timing --
...
 Batch |      Calls | Elapsed (ms) |    ns/call
----------------------------------------------
     1 |    2000000 |       12.xxx |       6.xx
     2 |    2000000 |        8.xxx |       4.xx
     ...
    12 |    2000000 |        6.xxx |       3.xx

(checksum, to keep the JIT from eliminating the loop entirely: <some number>)
```

The exact numbers are machine- and load-dependent — what matters is the *shape*: the first one or two batches tend to be higher and noisier than the later ones, which settle into a lower, more stable per-call time as the hot method gets promoted from Tier 0 to Tier 1. Running with `DOTNET_TieredCompilation=0` set should flatten that curve — every batch starts closer to the steady-state number since there's no promotion step to wait for.
