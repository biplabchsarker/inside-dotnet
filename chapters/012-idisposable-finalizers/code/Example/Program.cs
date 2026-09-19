using System;
using System.IO;

namespace Example;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Inside .NET - Chapter 012: IDisposable & Finalizers");
        Console.WriteLine("Example: The using declaration (C# 8+)");

        // The compiler generates a try/finally block under the hood
        // The stream will be deterministically disposed at the end of the method
        using var stream = new MemoryStream(new byte[100]);
        using var reader = new StreamReader(stream);
        
        string content = reader.ReadToEnd();
        Console.WriteLine("Stream read successfully.");
        
        // reader.Dispose() is called implicitly here
        // stream.Dispose() is called implicitly here
    }
}
