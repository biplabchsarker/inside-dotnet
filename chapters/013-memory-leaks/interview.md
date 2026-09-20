# Senior Interview Questions

### 1. Can a managed application in .NET actually leak memory?
**Answer:**
Yes. While the Garbage Collector prevents *unmanaged* leaks (forgetting to free pointers), it cannot prevent *managed* leaks. A managed leak occurs when an object is functionally unused by the application, but is still reachable by the GC through a strong reference chain. Because it is reachable, the GC will never collect it.

### 2. What is the most common cause of a managed memory leak?
**Answer:**
The Lapsed Listener (or Event Handler) problem. If a short-lived subscriber object (like a UI view) attaches an event handler to a long-lived publisher (like a static service or singleton) using `+=`, the publisher holds a strong reference to the subscriber via the underlying `MulticastDelegate`. If the subscriber does not call `-=` before it goes out of scope, it remains pinned in memory forever.

### 3. How do you fix a Lapsed Listener leak?
**Answer:**
The standard fix is for the subscriber to implement `IDisposable`. In the `Dispose()` method, the subscriber should call `-=` on the event. The creator of the subscriber is then responsible for calling `Dispose()` (often via a `using` block) when the object is no longer needed. Alternatively, for complex UI architectures, you can use a Weak Event pattern (via `WeakReference<T>`).

### 4. What is a WeakReference and when should you use it?
**Answer:**
A `WeakReference` holds a pointer to an object, but it does *not* prevent the Garbage Collector from collecting that object. When you try to access the object via `TryGetTarget`, it will return the object if it is still alive, or `null` if it has been collected. It is used heavily in caching and event aggregators to prevent memory leaks when subscribers forget to unsubscribe.

### 5. If a Button subscribes to an event on the Form it belongs to, is that a memory leak?
**Answer:**
No. If the Publisher (the Form) and the Subscriber (the Button) have the exact same lifetime, they will go out of scope together. The GC is smart enough to collect isolated "islands" of objects that reference each other, as long as neither object is reachable from a static GC Root.
