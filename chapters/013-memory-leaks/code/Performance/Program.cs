using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Performance;

public class EventPublisher
{
    public event EventHandler SomethingHappened;
    
    public void FireEvent()
    {
        SomethingHappened?.Invoke(this, EventArgs.Empty);
    }
}

public class LeakySubscriber
{
    private byte[] _data = new byte[1024 * 100]; // 100 KB payload
    
    public LeakySubscriber(EventPublisher publisher)
    {
        // LEAK: Subscribing to a long-lived publisher without ever unsubscribing
        publisher.SomethingHappened += OnSomethingHappened;
    }

    private void OnSomethingHappened(object sender, EventArgs e)
    {
        // Do nothing
    }
}

public class SafeSubscriber
{
    private byte[] _data = new byte[1024 * 100]; // 100 KB payload
    
    public void Subscribe(EventPublisher publisher)
    {
        publisher.SomethingHappened += OnSomethingHappened;
    }
    
    public void Unsubscribe(EventPublisher publisher)
    {
        publisher.SomethingHappened -= OnSomethingHappened;
    }

    private void OnSomethingHappened(object sender, EventArgs e)
    {
        // Do nothing
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Memory Leak Benchmark ---");
        
        RunSafeScenario();
        RunLeakyScenario();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void RunSafeScenario()
    {
        Console.WriteLine("\nRunning Safe Scenario (Proper Unsubscribe)...");
        var publisher = new EventPublisher();
        
        long initialMemory = GC.GetTotalMemory(true);
        
        for (int i = 0; i < 10000; i++)
        {
            var subscriber = new SafeSubscriber();
            subscriber.Subscribe(publisher);
            
            // ... use it ...
            
            subscriber.Unsubscribe(publisher); // Clean up!
        }
        
        long finalMemory = GC.GetTotalMemory(true);
        Console.WriteLine($"Objects Created: 10,000 (approx {10000 * 100 / 1024} MB)");
        Console.WriteLine($"Memory retained after GC: {(finalMemory - initialMemory) / 1024.0 / 1024.0:F2} MB");
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void RunLeakyScenario()
    {
        Console.WriteLine("\nRunning Leaky Scenario (Missing Unsubscribe)...");
        var publisher = new EventPublisher();
        
        long initialMemory = GC.GetTotalMemory(true);
        
        for (int i = 0; i < 10000; i++)
        {
            var subscriber = new LeakySubscriber(publisher);
            
            // ... use it ...
            
            // Forgot to unsubscribe! The publisher now holds a strong reference to the subscriber.
        }
        
        long finalMemory = GC.GetTotalMemory(true);
        Console.WriteLine($"Objects Created: 10,000 (approx {10000 * 100 / 1024} MB)");
        Console.WriteLine($"Memory retained after GC: {(finalMemory - initialMemory) / 1024.0 / 1024.0:F2} MB");
        
        // Prevent publisher from being collected before this point
        GC.KeepAlive(publisher);
    }
}
