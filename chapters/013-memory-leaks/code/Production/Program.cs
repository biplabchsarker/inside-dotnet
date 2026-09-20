using System;
using System.Collections.Generic;

namespace Production;

// PRODUCTION: A generic Weak Event Manager
// This allows subscribers to be collected by the GC even if they forget to unsubscribe.
public class WeakEventManager<TEventArgs>
{
    private readonly List<WeakReference<EventHandler<TEventArgs>>> _listeners = new();

    public void Subscribe(EventHandler<TEventArgs> listener)
    {
        _listeners.Add(new WeakReference<EventHandler<TEventArgs>>(listener));
    }

    public void Fire(object sender, TEventArgs e)
    {
        // Iterate backwards so we can safely remove dead references
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            var weakRef = _listeners[i];
            if (weakRef.TryGetTarget(out var listener))
            {
                // Target is still alive, invoke it
                listener(sender, e);
            }
            else
            {
                // Target was garbage collected! Clean up the dead reference.
                _listeners.RemoveAt(i);
            }
        }
    }
}

public class NotificationService
{
    // Use the WeakEventManager instead of a standard C# event
    public static readonly WeakEventManager<string> NotificationReceived = new();
    
    public static void Fire(string msg) => NotificationReceived.Fire(null, msg);
}

public class ForgetfulSubscriber
{
    private readonly string _name;
    private readonly byte[] _heavyUiData = new byte[10 * 1024 * 1024];

    public ForgetfulSubscriber(string name)
    {
        _name = name;
        NotificationService.NotificationReceived.Subscribe(OnNotification);
        Console.WriteLine($"Subscriber {_name} created.");
    }

    private void OnNotification(object? sender, string message)
    {
        Console.WriteLine($"Subscriber {_name} received: {message}");
    }
    
    // NOTE: We intentionally "forget" to unsubscribe here. No IDisposable.
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Simulating Production WeakEvent pattern...\n");
        
        var s1 = new ForgetfulSubscriber("One");
        var s2 = new ForgetfulSubscriber("Two");

        Console.WriteLine("\nFiring Notification (Both Alive):");
        NotificationService.Fire("Hello!");

        // Simulate going out of scope
        s1 = null; 
        s2 = null;

        Console.WriteLine("\nForcing Garbage Collection...");
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("\nFiring Notification (Both Dead):");
        NotificationService.Fire("Hello again!");
        
        Console.WriteLine("\nNo output means the WeakEventManager correctly allowed the GC to collect the forgetful subscribers!");
    }
}
