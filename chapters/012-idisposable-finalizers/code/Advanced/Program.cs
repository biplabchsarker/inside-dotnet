using System;
using System.IO;

namespace Advanced;

// The Standard Dispose Pattern
public class UnmanagedResourceWrapper : IDisposable
{
    private IntPtr _unmanagedHandle; // Fake unmanaged handle
    private FileStream _managedResource; // Managed resource
    private bool _disposed = false;

    public UnmanagedResourceWrapper()
    {
        Console.WriteLine("Allocating resources...");
        _unmanagedHandle = new IntPtr(12345);
        
        // In a real scenario, this would be a real file.
        // We use MemoryStream here just to mock a managed resource.
    }

    // Deterministic cleanup called by the consumer
    public void Dispose()
    {
        Dispose(disposing: true);
        
        // Tell the GC: "I already cleaned this up, remove it from the Finalization Queue!"
        GC.SuppressFinalize(this); 
        Console.WriteLine("Dispose() called explicitly. Finalization suppressed.");
    }

    // Non-deterministic cleanup called by the Finalizer Thread
    ~UnmanagedResourceWrapper()
    {
        Console.WriteLine("Finalizer called. Object was not properly disposed!");
        Dispose(disposing: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // We are being called explicitly. Safe to touch managed objects.
            Console.WriteLine("Disposing managed resources...");
            _managedResource?.Dispose();
        }

        // Always release unmanaged resources
        if (_unmanagedHandle != IntPtr.Zero)
        {
            Console.WriteLine($"Releasing unmanaged handle {_unmanagedHandle}...");
            _unmanagedHandle = IntPtr.Zero;
        }

        _disposed = true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Correct Usage ---");
        using (var wrapper = new UnmanagedResourceWrapper())
        {
            // Use it...
        }
        
        Console.WriteLine("\n--- Incorrect Usage (Relying on Finalizer) ---");
        CreateOrphan();
        
        // Force a GC to make the finalizer run
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
    
    static void CreateOrphan()
    {
        var wrapper = new UnmanagedResourceWrapper();
        // Forgot to call Dispose!
    }
}
