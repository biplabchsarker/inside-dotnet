using System.Reflection;

Console.WriteLine("=== Chapter 015: Delegates & Events (Advanced) ===\n");

// 1. Multicast Delegate Invocation List and Exception Pitfall
Console.WriteLine("1. Multicast Delegate Chain and Exception Behavior");

Action multiDelegate = () => Console.WriteLine("  Handler 1 succeeded");
multiDelegate += () => throw new InvalidOperationException("Handler 2 failed abruptly!");
multiDelegate += () => Console.WriteLine("  Handler 3 succeeded");

Console.WriteLine("Direct invocation multiDelegate():");
try
{
    multiDelegate();
}
catch (Exception ex)
{
    Console.WriteLine($"  CAUGHT: {ex.Message} (Notice Handler 3 NEVER ran!)");
}

Console.WriteLine("\nSafe Invocation via GetInvocationList():");
var exceptions = new List<Exception>();
foreach (var singleDelegate in multiDelegate.GetInvocationList())
{
    try
    {
        singleDelegate.DynamicInvoke();
    }
    catch (TargetInvocationException tie)
    {
        exceptions.Add(tie.InnerException ?? tie);
        Console.WriteLine($"  Safely trapped exception: {tie.InnerException?.Message}");
    }
}
Console.WriteLine($"Total subscribers evaluated: {multiDelegate.GetInvocationList().Length}, Exceptions trapped: {exceptions.Count}");

// 2. Closure Decompilation & Heap Allocation Mechanics
Console.WriteLine("\n2. Closures Under the Hood (Compiler-Generated Display Classes)");

int multiplier = 42;
Func<int, int> closingLambda = x => x * multiplier;

Console.WriteLine($"Result: {closingLambda(2)}");
Console.WriteLine($"Target object type: {closingLambda.Target?.GetType().FullName}");
Console.WriteLine($"Target object is compiler-generated? {closingLambda.Target?.GetType().Name.Contains("DisplayClass")}");

// Inspecting fields of the display class
if (closingLambda.Target != null)
{
    var fields = closingLambda.Target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    foreach (var field in fields)
    {
        Console.WriteLine($"  Captured Field on DisplayClass: '{field.Name}' = {field.GetValue(closingLambda.Target)}");
    }
}

// 3. Static Lambdas in modern C# prevent accidental allocations
// Func<int, int> staticLambda = static x => x * multiplier; // Error CS8829: A static local function or lambda cannot reference 'multiplier'
Func<int, int> staticLambda = static x => x * 10;
Console.WriteLine($"Static lambda Target: {(staticLambda.Target == null ? "null (NO HEAP ALLOCATION)" : staticLambda.Target.GetType().Name)}");

// 4. Thread-Safe Event Accessors using Interlocked.CompareExchange
Console.WriteLine("\n3. Thread-Safe Custom Event Accessors (Interlocked Pattern)");
var bus = new ThreadSafeEventBus();
EventHandler<string> handlerA = (s, msg) => Console.WriteLine($"  A received: {msg}");
bus.MessagePublished += handlerA;
bus.Publish("Hello from ThreadSafeEventBus!");
bus.MessagePublished -= handlerA;
bus.Publish("This should not be printed.");

public class ThreadSafeEventBus
{
    private EventHandler<string>? _messagePublished;

    // Under the hood, this is exactly what the C# compiler emits for field-like events!
    public event EventHandler<string>? MessagePublished
    {
        add
        {
            EventHandler<string>? current = _messagePublished;
            while (true)
            {
                EventHandler<string>? updated = (EventHandler<string>?)Delegate.Combine(current, value);
                EventHandler<string>? prev = Interlocked.CompareExchange(ref _messagePublished, updated, current);
                if (ReferenceEquals(prev, current)) break;
                current = prev;
            }
        }
        remove
        {
            EventHandler<string>? current = _messagePublished;
            while (true)
            {
                EventHandler<string>? updated = (EventHandler<string>?)Delegate.Remove(current, value);
                EventHandler<string>? prev = Interlocked.CompareExchange(ref _messagePublished, updated, current);
                if (ReferenceEquals(prev, current)) break;
                current = prev;
            }
        }
    }

    public void Publish(string message)
    {
        // Safe snapshot
        var handler = _messagePublished;
        handler?.Invoke(this, message);
    }
}
