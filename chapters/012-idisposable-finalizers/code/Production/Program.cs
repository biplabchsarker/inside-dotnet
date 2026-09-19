using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Production;

// Using SafeHandle avoids the need for a finalizer entirely!
public class ModernResourceWrapper : IDisposable
{
    // SafeFileHandle handles the finalization and OS interop safely
    private readonly SafeFileHandle _handle;
    private bool _disposed;

    public ModernResourceWrapper(string path)
    {
        Console.WriteLine($"Opening handle to {path} using SafeFileHandle...");
        _handle = File.OpenHandle(path);
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        Console.WriteLine("Disposing SafeFileHandle...");
        // Just dispose the managed object. No finalizer needed in this class!
        _handle?.Dispose();
        _disposed = true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        string tempFile = Path.GetTempFileName();
        
        using (var resource = new ModernResourceWrapper(tempFile))
        {
            Console.WriteLine("Resource is open and safe.");
        }
        
        File.Delete(tempFile);
    }
}
