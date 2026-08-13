# References — Strings & Interning

- [String.Intern(String) method — API reference](https://learn.microsoft.com/dotnet/api/system.string.intern)
- [String.IsInterned(String) method — API reference](https://learn.microsoft.com/dotnet/api/system.string.isinterned)
- [String.Substring — API reference](https://learn.microsoft.com/dotnet/api/system.string.substring)
- [Best practices for comparing strings in .NET](https://learn.microsoft.com/dotnet/standard/base-types/best-practices-strings)
- [StringComparison enum — API reference](https://learn.microsoft.com/dotnet/api/system.stringcomparison)
- [Use ordinal string comparison — CA1309](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1309)
- [Specify StringComparison for correctness — CA1310](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1310)
- [XmlNameTable class — API reference](https://learn.microsoft.com/dotnet/api/system.xml.xmlnametable)
- [GC.GetTotalMemory — API reference](https://learn.microsoft.com/dotnet/api/system.gc.gettotalmemory)
- [dotnet-counters — measuring GC heap size / Gen 2 size](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters)
- [BenchmarkDotNet — MemoryDiagnoser](https://benchmarkdotnet.org/articles/configs/diagnosers.html)
- [dotnet/runtime — coreclr string interning and comparison implementation](https://github.com/dotnet/runtime)
- [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/article.md) — why strings' value-based equality is notable for a reference type
- [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md) — the `ReferenceEquals`-vs-value-equality distinction applied here to strings
- [Episode 8 — Object Allocation](../007-object-allocation/article.md) — the allocator every uninterned string still allocates through
