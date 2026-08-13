# References — Garbage Collection Fundamentals

- [Fundamentals of garbage collection — .NET](https://learn.microsoft.com/dotnet/standard/garbage-collection/fundamentals)
- [Garbage collection and performance — workstation vs. server GC, concurrent GC](https://learn.microsoft.com/dotnet/standard/garbage-collection/performance)
- [Runtime config options for garbage collection (ServerGarbageCollection, ConcurrentGarbageCollection)](https://learn.microsoft.com/dotnet/core/runtime-config/garbage-collector)
- [WeakReference class — API reference (short vs. long weak references)](https://learn.microsoft.com/dotnet/api/system.weakreference)
- [GC.CollectionCount(Int32) method — API reference](https://learn.microsoft.com/dotnet/api/system.gc.collectioncount)
- [GC.GetGeneration method — API reference](https://learn.microsoft.com/dotnet/api/system.gc.getgeneration)
- [Object.Finalize / finalizers — .NET](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-finalize-and-dispose)
- [.NET and containers — GC heap count and cgroup awareness](https://learn.microsoft.com/dotnet/core/docker/building-net-docker-images)
- [dotnet-counters — % Time in GC, Gen 0/1/2 Size](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters)
- [BenchmarkDotNet — configuring jobs (WithGcServer, WithGcConcurrent)](https://benchmarkdotnet.org/articles/configs/jobs.html)
- [dotnet/runtime — coreclr GC source (`src/coreclr/gc/`)](https://github.com/dotnet/runtime)
- [Episode 8 — Object Allocation](../007-object-allocation/article.md) — the bump-pointer allocator this chapter's compaction phase keeps viable
- [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md) — the escape-analysis-avoided allocations this chapter's GC never has to reclaim
- [Episode 13 — IDisposable & Finalizers](../012-idisposable-finalizers/article.md) — the full treatment of the finalization lifecycle introduced here
