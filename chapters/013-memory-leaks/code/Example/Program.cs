using System;

namespace Example;

public class NotificationService
{
    // A static publisher lives for the lifetime of the application
    public static event EventHandler<string>? NotificationReceived;
    
    public static void Fire(string msg) => NotificationReceived?.Invoke(null, msg);
}

public class UserDashboard
{
    private readonly string _dashboardId;
    
    // Simulate a heavy UI component (10MB)
    private readonly byte[] _heavyUiData = new byte[10 * 1024 * 1024];

    public UserDashboard(string id)
    {
        _dashboardId = id;
        
        // THE LEAK: Subscribing to a static event
        // The NotificationService now holds a strong reference to this dashboard instance.
        NotificationService.NotificationReceived += OnNotification;
        
        Console.WriteLine($"Dashboard {_dashboardId} created.");
    }

    private void OnNotification(object? sender, string message)
    {
        Console.WriteLine($"Dashboard {_dashboardId} received: {message}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Simulating User Login/Logout cycle...\n");
        
        for (int i = 1; i <= 5; i++)
        {
            var dashboard = new UserDashboard(i.ToString());
            
            // User immediately logs out. The dashboard variable goes out of scope.
            dashboard = null;
        }

        Console.WriteLine("\nForcing Garbage Collection...");
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Even though the dashboards went out of scope, they are still alive!
        Console.WriteLine("\nFiring Notification:");
        NotificationService.Fire("System Update Available");
        
        Console.WriteLine("\nBecause the static event holds a reference to them, all 5 dashboards are still in memory (50MB leaked)!");
    }
}
