# Exercises

### Exercise 1: Implement the Standard Dispose Pattern
Create a class called `TempFileWrapper` that manages a temporary file. 
- It should generate a random filename and open it using a raw unmanaged handle (`IntPtr` or simulated).
- Implement the standard `Dispose(bool disposing)` pattern.
- Ensure that if the consumer forgets to call `Dispose()`, the finalizer will clean up the temporary file on disk.

### Exercise 2: The SafeHandle Upgrade
Refactor your `TempFileWrapper` from Exercise 1. 
- Replace the raw `IntPtr` and finalizer with a `SafeFileHandle` from `Microsoft.Win32.SafeHandles`.
- Notice how you can delete the finalizer (`~TempFileWrapper`) completely! 

---

## Challenge

Look at the following code snippet. Predict what will happen when this program runs, and why.

```csharp
class BadFinalizer
{
    ~BadFinalizer()
    {
        Console.WriteLine("Finalizing...");
        throw new InvalidOperationException("Oops!");
    }
}

class Program
{
    static void Main()
    {
        CreateObject();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("Done.");
    }

    static void CreateObject()
    {
        var b = new BadFinalizer();
    }
}
```

<details>
<summary>Challenge Answer</summary>

The program will print "Finalizing..." and then **immediately crash the entire process**. 
When an exception is thrown inside a finalizer, it goes unhandled on the Finalizer Thread. In modern .NET, an unhandled exception on the Finalizer Thread is fatal and terminates the process immediately. "Done." will never be printed.
</details>
