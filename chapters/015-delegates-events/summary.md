# Summary: Delegates & Events Under the Hood

### The 30-Second Summary

- **Delegates are Managed Objects**: In C#, a delegate is not a raw C-style function pointer. It is an instance of `System.MulticastDelegate` allocated on the managed heap. It holds a reference to a target object (`_target`) and a native code pointer (`_methodPtr`).
- **Single-cast vs. Multicast**: When multiple delegates are chained with `+=`, the runtime creates a new `MulticastDelegate` whose `_invocationList` points to an array of delegates. Calling the delegate iterates this list synchronously.
- **Exception Trap**: If subscriber #2 throws an unhandled exception, subscriber #3 will **never execute**. Production event publishers should iterate `GetInvocationList()` and handle exceptions per listener.
- **Closures Allocate**: When an anonymous method or lambda captures an outer variable, the compiler generates a hidden `<>c__DisplayClass` on the heap. In tight loops or hot paths, this creates GC pressure. Use `static` lambdas to guarantee zero allocations.
- **The Lapsed Listener Leak**: Because `_target` holds a strong reference to the subscribing object, subscribing to a long-lived publisher prevents the subscriber from ever being collected by the GC.
- **Events are Encapsulation Gateways**: An `event` is simply a pair of compiler-generated `add` and `remove` methods (using `Interlocked.CompareExchange` for thread safety) wrapping a private delegate field.
