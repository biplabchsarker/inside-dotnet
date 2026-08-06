using System.Reflection;

Console.WriteLine("=== Inside .NET: Episode 2 demo ===");

// 1. Prove we are running as *managed* code with a live CLR behind us.
Console.WriteLine($"CLR version   : {Environment.Version}");
Console.WriteLine($"OS description: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");

// 2. Inspect this assembly's metadata — the manifest the loader parsed at startup.
Assembly self = Assembly.GetExecutingAssembly();
Console.WriteLine($"Assembly      : {self.FullName}");
Console.WriteLine($"Location      : {self.Location}");

// 3. Observe JIT warm-up: the first call compiles the method, later calls reuse native code.
TimeSpan first = Time(() => Fibonacci(28));
TimeSpan second = Time(() => Fibonacci(28));
Console.WriteLine($"First call    : {first.TotalMilliseconds:F3} ms (includes JIT compile)");
Console.WriteLine($"Second call   : {second.TotalMilliseconds:F3} ms (native code already cached)");

static TimeSpan Time(Action action)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    action();
    sw.Stop();
    return sw.Elapsed;
}

static long Fibonacci(int n) => n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
