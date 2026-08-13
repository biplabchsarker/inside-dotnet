I turned on Server GC expecting a free performance win. Measured it instead — it made things slightly worse.

On a 22-logical-processor machine, a single thread allocating 2,000,000 small objects ran in 9.998 ms under Workstation GC and 10.934 ms under Server GC. Same allocation total, same collection behavior — Server GC was just slower. Not a fluke: Server GC creates one heap *per core* specifically so multiple threads allocating concurrently can parallelize. With exactly one allocating thread, that parallelism has nothing to apply to, and the extra heaps' overhead shows up with nothing to offset it. ASP.NET Core defaults to Server GC because it genuinely has many concurrent requests. Your single-threaded batch job probably doesn't.

Two more things I measured instead of assuming:

A full garbage collection costs **5.84x** a Gen 0 collection, against the identical live object graph. That's the real, quantified payoff of the generational hypothesis — most collections never pay this price, because most objects die young.

And here's the one that actually surprised me: nulling a local variable and calling `GC.Collect()` *in the same still-executing method* does NOT reliably free the object. A `WeakReference` proved it directly — still alive, right after the null and the collect. Only after that method *returns*, and a fresh collection runs from the caller, does it actually go. The CLR's real guarantee is "collectible once the holding frame is gone," not "collectible the instant you reassign a variable."

This is Episode 11 of Inside .NET — roots, mark/sweep/compact, generational promotion, and three real BenchmarkDotNet result tables, not estimates.

#dotnet #csharp #memorymanagement #softwarearchitecture
