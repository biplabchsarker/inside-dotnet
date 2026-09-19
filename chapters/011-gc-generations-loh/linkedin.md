I found the exact byte where an allocation silently jumps onto the Large Object Heap. It's not where you'd guess.

`new byte[84_975]` → Gen 0, the normal path.
`new byte[84_976]` → already on the LOH.

One byte flips it, because the CLR compares the object's *total* size — data plus a ~24-byte array header — against the documented 85,000-byte threshold, not the element count you actually typed.

This is Episode 12 of Inside .NET, the direct follow-up to last week's GC fundamentals chapter. Three things I measured instead of assuming:

Allocating ~100 MB as 1,000 large (LOH) objects vs. ~62,500 small (Gen 0) ones came out close on wall-clock time — close enough that a repeat run flipped which one "won." But the collection counts didn't budge: the LOH approach triggered ~31 full (Gen 0+1+2) collections per 1,000 runs, against ~8 Gen-0-only collections for the small-object approach. Same bytes, same rough speed, completely different GC pause profile.

Forcing the Large Object Heap to actually compact (`GCSettings.LargeObjectHeapCompactionMode = CompactOnce`) cost 87x a plain sweep of the same fragmented heap. That's exactly why it isn't the default — and exactly why it's a real, useful tool for the one time you actually need it.

And the card table — the mechanism that lets a cheap Gen 0 collection ignore a huge Gen 2 heap instead of rescanning it every time. No public API reads its dirty bits directly, so I built a proxy: hold the *dirtied* subset of a Gen 2 graph fixed at 2,000 objects while growing the total graph 40x, from 50,000 to 2,000,000. Collection cost grew under 10x, not 40x — sub-linear, not flat, but the point holds: the size of what *changed* in Gen 2 is what matters, not the size of Gen 2 itself.

Real BenchmarkDotNet numbers, real `GC.GetGCMemoryInfo()` output, no estimates — including the runs where the numbers didn't cooperate with a clean story, reported anyway.

#dotnet #csharp #memorymanagement #softwarearchitecture
