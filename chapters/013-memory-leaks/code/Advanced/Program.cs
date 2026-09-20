using System;

namespace Advanced;

public class NotificationService
{
    public static event EventHandler<string>? NotificationReceived;
    public static void Fire(string msg) => NotificationReceived?.Invoke(null, msg);
}

// ADVANCED: Implement IDisposable to break the strong reference chain
public class SafeUserDashboard : IDisposable
{
    private readonly string _dashboardId;
    private readonly byte[] _heavyUiData = new byte[10 * 1024 * 1024]; // 10MB

    public SafeUserDashboard(string id)
    {
        _dashboardId = id;
        NotificationService.NotificationReceived += OnNotification;
        Console.WriteLine($"Dashboard {_dashboardId} created.");
    }

    private void OnNotification(object? sender, string message)
    {
        Console.WriteLine($"Dashboard {_dashboardId} received: {message}");
    }

    public void Dispose()
    {
        // Break the reference chain so the GC can collect this object
        NotificationService.NotificationReceived -= OnNotification;
        Console.WriteLine($"Dashboard {_dashboardId} unsubscribed and disposed.");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Simulating Safe User Login/Logout cycle...\n");
        
        for (int i = 1; i <= 5; i++)
        {
            // Using statement automatically calls Dispose() when it goes out of scope
            using (var dashboard = new SafeUserDashboard(i.ToString()))
            {
                // User is actively looking at the dashboard
            } 
            // Dispose() is called here
        }

        Console.WriteLine("\nForcing Garbage Collection...");
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("\nFiring Notification:");
        NotificationService.Fire("System Update Available");
        
        Console.WriteLine("\nNo output means no dashboards were leaked! Memory is safely freed.");
    }
}
