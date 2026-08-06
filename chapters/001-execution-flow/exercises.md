# Exercises — What Really Happens When You Run a .NET Application?

1. **Time JIT warm-up at different depths.** Run the included [demo](code/Chapter01.Demo/Program.cs) as-is, then modify the `Fibonacci` call depth (try `20`, `28`, `35`) and re-run with `dotnet run`. Record the first-call vs. second-call timings at each depth. Does the *gap* between first and second call grow with `n`, stay roughly constant, or shrink? What does that tell you about what the JIT compilation cost actually scales with (the method's IL size and complexity) versus what it doesn't scale with (how much work the method does once compiled)?

2. **Read the IL your C# compiled to.** Paste the `Fibonacci` method from `Program.cs` into [sharplab.io](https://sharplab.io), select "IL" as the output, and identify which IL opcodes (`ldarg.0`, `call`, `add`, `ble.s`, etc.) correspond to which parts of the C# source (the parameter access, the recursive calls, the comparison, the addition). Confirm for yourself that nothing in the IL is CPU-specific — there's no x64 or ARM64 instruction anywhere in it.

3. **Stretch — compare JIT-based startup to Native AOT startup.** Publish the demo project two ways: `dotnet publish -c Release` (standard JIT-based apphost) and `dotnet publish -c Release -r win-x64 --self-contained -p:PublishAot=true` (Native AOT — requires the Native AOT workload; see [Native AOT deployment overview](https://learn.microsoft.com/dotnet/core/deploying/native-aot/) if the build fails on missing prerequisites). Time `Measure-Command { .\Chapter01.Demo.exe }` (PowerShell) for each published output a few times and compare. Which parts of this chapter's pipeline does the Native AOT build skip entirely, and does the measured startup gap match what you'd expect from that?

## Challenge

**Predict the output before running it.**

```csharp
static long Fibonacci(int n) => n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);

TimeSpan t1 = Time(() => Fibonacci(30));
TimeSpan t2 = Time(() => Fibonacci(30));
TimeSpan t3 = Time(() => Fibonacci(5));   // note: much smaller n
Console.WriteLine(t1 > t2);               // A
Console.WriteLine(t3 < t2);               // B
```

Before running it: will `A` print `True` or `False`? Will `B`? Write down your answer and *why*, then run it. If you got `B` wrong, you predicted based on JIT warm-up alone and forgot the method is already warm by the third call — the real cost difference between `t2` and `t3` at that point is almost entirely `Fibonacci(30)` doing exponentially more recursive work than `Fibonacci(5)`, not compilation.
