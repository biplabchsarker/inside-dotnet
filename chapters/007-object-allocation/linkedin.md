"Never call a virtual method from a constructor" is advice every C# developer has heard. Here's the mechanism that makes it dangerous, not just a style rule.

When `new SomeClass()` runs, the CLR allocates zeroed, headered memory FIRST, then runs constructors base-to-derived. If a base constructor calls a virtual method the derived class overrides, that override runs while the derived class's own fields are still at their zeroed default — because the derived constructor hasn't executed yet.

The allocation side of this is worth knowing too. `new` is deliberately cheap: bump a pointer in the current thread's own private allocation context, write a small header (method table pointer + sync block index), done. No search, no lock, no per-field zeroing at that moment — the memory arrived pre-zeroed. Each thread gets its own allocation context specifically so heavy concurrent allocation doesn't serialize on a global lock.

The cost shows up later, not at allocation time: when a thread's context runs out of room, that's what triggers a Gen 0 collection — not a timer, not a periodic check.

This is Episode 8 of Inside .NET — full mechanics, diagrams, and a runnable demo that proves the base-constructor gotcha with real output instead of just asserting it.

#dotnet #csharp #memorymanagement #softwarearchitecture
