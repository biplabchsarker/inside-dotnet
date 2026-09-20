# Frequently Asked Questions: Delegates & Events

### 1. What is the difference between `Action<T>`, `Func<T, TResult>`, and a custom `delegate`?
`Action` and `Func` are simply pre-defined generic delegates provided by the BCL in `System`. Functionally, there is zero difference between writing:
```csharp
public delegate int Transformer(int x);
```
and using `Func<int, int>`. Use custom delegate types only when you need domain-specific parameter names or specific API documentation.

### 2. Why are field-like events implemented with `Interlocked.CompareExchange`?
When you declare `public event Action? OnChange;`, the C# compiler generates:
```csharp
private Action? _onChange;
public event Action? OnChange
{
    add => InterlockedCompareExchangeLoop(ref _onChange, Delegate.Combine);
    remove => InterlockedCompareExchangeLoop(ref _onChange, Delegate.Remove);
}
```
Because delegates are immutable, subscribing creates a new delegate object. `Interlocked.CompareExchange` performs an atomic lock-free CAS (Compare-And-Swap) update to ensure multiple threads subscribing or unsubscribing simultaneously do not overwrite each other.

### 3. What does `event?.Invoke(this, args)` actually compile to?
It compiles to:
```csharp
var temp = this.myEvent; // Local reference snapshot (thread-safe against concurrent unsubscription)
if (temp != null)
{
    temp.Invoke(this, args);
}
```
This protects against the race condition where another thread sets `myEvent = null` between a null check and the invocation.

### 4. How can I pass parameters to a background task without allocating a closure?
Instead of capturing local variables in a lambda:
```csharp
// Allocates DisplayClass on heap:
int id = 42;
ThreadPool.QueueUserWorkItem(_ => Process(id));
```
Use the state parameter overload:
```csharp
// Zero closure allocation:
int id = 42;
ThreadPool.QueueUserWorkItem(static state => Process((int)state!), id);
```
In modern .NET, APIs like `ThreadPool.QueueUserWorkItem` accept a state object to avoid closure allocations entirely.
