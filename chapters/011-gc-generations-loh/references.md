# References — GC Generations & the Large Object Heap

- [Fundamentals of garbage collection — .NET](https://learn.microsoft.com/dotnet/standard/garbage-collection/fundamentals)
- [Large object heap — .NET](https://learn.microsoft.com/dotnet/standard/garbage-collection/large-object-heap)
- [GCSettings.LargeObjectHeapCompactionMode property — API reference](https://learn.microsoft.com/dotnet/api/system.runtime.gcsettings.largeobjectheapcompactionmode)
- [GC.GetGCMemoryInfo method and GCMemoryInfo/GCGenerationInfo structs — API reference](https://learn.microsoft.com/dotnet/api/system.gc.getgcmemoryinfo)
- [GC.AllocateArray<T> method (Pinned Object Heap) — API reference](https://learn.microsoft.com/dotnet/api/system.gc.allocatearray)
- [Runtime config options for garbage collection (GCgen0size, GCHeapHardLimit)](https://learn.microsoft.com/dotnet/core/runtime-config/garbage-collector)
- [.NET 8 GC region-based heap design — release notes](https://learn.microsoft.com/dotnet/core/compatibility/gc/8.0/regions)
- [dotnet-counters — LOH Size, Gen 0/1/2 Size, % Time in GC](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters)
- [dotnet/runtime — coreclr GC source and design docs (`src/coreclr/gc/`)](https://github.com/dotnet/runtime)
- [BenchmarkDotNet — MemoryDiagnoser and per-generation GC columns](https://benchmarkdotnet.org/articles/configs/diagnosers.html)
- [Episode 11 — Garbage Collection Fundamentals](../010-garbage-collection/article.md) — the roots/mark-sweep-compact/generational-hypothesis fundamentals this chapter deepens
- [Episode 8 — Object Allocation](../007-object-allocation/article.md) — the bump-pointer allocation fast path whose budget this chapter explains
- [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/article.md) — what happens when GC reclamation alone isn't enough
