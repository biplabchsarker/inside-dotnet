using System.Reflection;

Console.WriteLine("=== Chapter 015: Delegates & Events (Production) ===\n");

// -------------------------------------------------------------
// Scenario 1: The Lapsed Listener Memory Leak vs WeakEventSource
// -------------------------------------------------------------
Console.WriteLine("--- 1. WeakEventSource (Preventing Lapsed Listener Leaks) ---");

var globalEventHub = new GlobalEventHub();

void CreateAndRegisterTemporarySubscriber()
{
    var subscriber = new ShortLivedWindow("TemporaryDialog_1");
    // Register using WeakEventSource instead of strong event
    globalEventHub.WeakBroadcast.Subscribe(subscriber, (target, sender, msg) => target.OnBroadcastReceived(sender, msg));
    Console.WriteLine("  ShortLivedWindow registered with WeakEventSource.");
}

CreateAndRegisterTemporarySubscriber();

// Force garbage collection
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

Console.WriteLine("  Garbage collection triggered. Raising broadcast:");
globalEventHub.WeakBroadcast.Raise(globalEventHub, "System notification after GC");

// -------------------------------------------------------------
// Scenario 2: Resilient SafeMulticastDispatcher
// -------------------------------------------------------------
Console.WriteLine("\n--- 2. Resilient SafeMulticastDispatcher (Fault Isolation) ---");

var dispatcher = new SafeMulticastDispatcher<OrderProcessedEventArgs>();

dispatcher.Subscribe((sender, args) => Console.WriteLine($"  [AuditService] Logged order {args.OrderId}"));
dispatcher.Subscribe((sender, args) => throw new HttpRequestException("CRM webhook failed with HTTP 503"));
dispatcher.Subscribe((sender, args) => Console.WriteLine($"  [EmailService] Sent receipt for order {args.OrderId}"));

try
{
    Console.WriteLine("  Dispatching event across 3 subscribers (one will fail)...");
    dispatcher.Dispatch(null, new OrderProcessedEventArgs(9901, 149.99m));
}
catch (AggregateException ex)
{
    Console.WriteLine($"  Dispatcher caught aggregate error: {ex.InnerExceptions.Count} exception(s).");
    foreach (var inner in ex.InnerExceptions)
    {
        Console.WriteLine($"    - Handled error: {inner.Message}");
    }
}
Console.WriteLine("  Notice both AuditService AND EmailService ran, despite the CRM webhook failure!");

// -------------------------------------------------------------
// Type Definitions
// -------------------------------------------------------------

public class OrderProcessedEventArgs : EventArgs
{
    public int OrderId { get; }
    public decimal Amount { get; }
    public OrderProcessedEventArgs(int orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }
}

public class ShortLivedWindow
{
    public string Name { get; }
    public ShortLivedWindow(string name) => Name = name;

    public void OnBroadcastReceived(object? sender, string message)
    {
        Console.WriteLine($"    [{Name}] received: {message}");
    }

    ~ShortLivedWindow()
    {
        Console.WriteLine($"    >>> Finalizer: {Name} was collected by GC! No memory leak! <<<");
    }
}

public class GlobalEventHub
{
    public WeakEventSource<string> WeakBroadcast { get; } = new();
}

/// <summary>
/// A memory-leak safe event source that holds only weak references to listener instances.
/// If a listener goes out of scope, it will be naturally collected by the GC.
/// </summary>
public class WeakEventSource<TEventArgs>
{
    private readonly List<IWeakSubscription> _subscriptions = new();
    private readonly object _lock = new();

    public void Subscribe<TTarget>(TTarget target, Action<TTarget, object?, TEventArgs> handler)
        where TTarget : class
    {
        lock (_lock)
        {
            _subscriptions.Add(new WeakSubscription<TTarget>(target, handler));
        }
    }

    public void Raise(object? sender, TEventArgs args)
    {
        List<IWeakSubscription> toExecute;
        lock (_lock)
        {
            // Prune dead references while taking a snapshot
            _subscriptions.RemoveAll(s => !s.IsAlive);
            toExecute = new List<IWeakSubscription>(_subscriptions);
        }

        foreach (var sub in toExecute)
        {
            sub.Invoke(sender, args);
        }
    }

    private interface IWeakSubscription
    {
        bool IsAlive { get; }
        void Invoke(object? sender, TEventArgs args);
    }

    private class WeakSubscription<TTarget> : IWeakSubscription where TTarget : class
    {
        private readonly WeakReference<TTarget> _weakTarget;
        private readonly Action<TTarget, object?, TEventArgs> _handler;

        public WeakSubscription(TTarget target, Action<TTarget, object?, TEventArgs> handler)
        {
            _weakTarget = new WeakReference<TTarget>(target);
            _handler = handler;
        }

        public bool IsAlive => _weakTarget.TryGetTarget(out _);

        public void Invoke(object? sender, TEventArgs args)
        {
            if (_weakTarget.TryGetTarget(out var target))
            {
                _handler(target, sender, args);
            }
        }
    }
}

/// <summary>
/// A resilient dispatcher that invokes all registered handlers sequentially,
/// aggregating any exceptions into an AggregateException without aborting execution of remaining handlers.
/// </summary>
public class SafeMulticastDispatcher<TEventArgs>
{
    private EventHandler<TEventArgs>? _handlers;
    private readonly object _lock = new();

    public void Subscribe(EventHandler<TEventArgs> handler)
    {
        lock (_lock)
        {
            _handlers = (EventHandler<TEventArgs>?)Delegate.Combine(_handlers, handler);
        }
    }

    public void Unsubscribe(EventHandler<TEventArgs> handler)
    {
        lock (_lock)
        {
            _handlers = (EventHandler<TEventArgs>?)Delegate.Remove(_handlers, handler);
        }
    }

    public void Dispatch(object? sender, TEventArgs args)
    {
        EventHandler<TEventArgs>? snapshot;
        lock (_lock)
        {
            snapshot = _handlers;
        }

        if (snapshot == null) return;

        var invocationList = snapshot.GetInvocationList();
        List<Exception>? exceptions = null;

        foreach (var handlerDelegate in invocationList)
        {
            try
            {
                ((EventHandler<TEventArgs>)handlerDelegate).Invoke(sender, args);
            }
            catch (Exception ex)
            {
                exceptions ??= new List<Exception>();
                exceptions.Add(ex);
            }
        }

        if (exceptions != null && exceptions.Count > 0)
        {
            throw new AggregateException("One or more event handlers threw exceptions during dispatch.", exceptions);
        }
    }
}
