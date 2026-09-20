# Exercises

### Exercise 1: Finding the Leak
Create a simple Console App. 
- Create a `static event Action OnTick`.
- Inside a `while` loop, create a new instance of a class that subscribes to `OnTick` but does not unsubscribe.
- Force a `GC.Collect()` inside the loop and print out `GC.GetTotalMemory(true)`. 
- Watch the memory grow over time. 

### Exercise 2: Fixing the Leak
Modify Exercise 1.
- Make the class implement `IDisposable`.
- Unsubscribe from `OnTick` inside the `Dispose` method.
- Call `.Dispose()` inside the loop before creating the next object.
- Run the program and observe that the memory stays flat.

---

## Challenge

Look at the following code snippet. Will `DataProcessor` be garbage collected after `Process()` finishes executing? Why or why not?

```csharp
public class ApplicationTimer
{
    public static event EventHandler Tick;
}

public class DataProcessor
{
    public DataProcessor()
    {
        ApplicationTimer.Tick += (s, e) => Console.WriteLine("Tick!");
    }
}

public class Program
{
    public static void Process()
    {
        var processor = new DataProcessor();
    }
}
```

<details>
<summary>Challenge Answer</summary>

**No, it will not be collected.**

Even though `processor` goes out of scope when `Process()` returns, the constructor of `DataProcessor` subscribes to a `static` event. The lambda expression `(s, e) => Console.WriteLine("Tick!")` is an instance method (it belongs to the `DataProcessor` instance) because it was declared inside it. 

Therefore, `ApplicationTimer.Tick` holds a strong reference to the `DataProcessor` instance. It will live in memory forever.
</details>
