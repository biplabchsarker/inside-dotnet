# Chapter 006 — Struct vs Class Copy-Cost Benchmark

Real `BenchmarkDotNet` numbers backing this chapter's Performance Notes claim that large-struct-by-value copying is measurably more expensive than passing by `in` or using an equivalent reference type.

## Run it

```bash
cd Chapter06.Benchmarks
dotnet run -c Release
```

Takes roughly 2 minutes (BenchmarkDotNet runs warm-up + multiple iterations per method). Results vary by hardware — the *relative* gap between "large struct by value" and "large struct by `in`" is the point, not the absolute nanosecond figures.

## What it measures

Four call patterns, same underlying data:
- A 16-byte struct passed by value (baseline)
- A 256-byte struct passed by value
- The same 256-byte struct passed by `in` (readonly reference, no copy)
- An equivalent 256-byte reference type passed by reference (always one pointer copy)

See the results table in [`../../article.md`](../../article.md#performance-notes) for the measured numbers from this run.
