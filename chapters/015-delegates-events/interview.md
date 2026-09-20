# Senior & Architect Interview Questions: Delegates & Events

### Q1: Walk me through the internal memory representation of a C# Delegate. What fields exist on the object in the heap?
**Answer:**
A delegate in .NET inherits from `System.MulticastDelegate` (which inherits from `System.Delegate` and `System.Object`). On a 64-bit architecture, each delegate instance has:
1. Standard Object Header: 8-byte SyncBlock index + 8-byte `MethodTable*`.
2. `_target`: An 8-byte managed object reference to the instance on which the method should be called (or `null` if the method is static).
3. `_methodPtr`: An 8-byte unmanaged pointer directly referencing the JIT-compiled native code machine instructions (or a thunk/precode stub).
4. `_methodBase`: A reference to reflection metadata (`MethodInfo`).
5. `_invocationList`: A reference to an `object[]` or child `Delegate` when multiple callbacks are chained together via `+=`.
6. `_invocationCount`: Number of callbacks in the chain.

---

### Q2: What is the "Lapsed Listener" problem and how do you diagnose and resolve it in enterprise .NET applications?
**Answer:**
The Lapsed Listener problem occurs when a short-lived subscriber registers an event handler with a long-lived publisher (such as a static event, a singleton service, or an application host). Because the publisher's delegate holds a strong reference in its `_target` field to the subscriber instance, the Garbage Collector sees a direct reference path from a GC Root (the singleton publisher) to the subscriber. As a result, the subscriber cannot be collected, leaking itself and its entire sub-graph of dependencies.

**Diagnosis:**
Memory profiler snapshots (dotnet-dump, Visual Studio Diagnostics, or JetBrains dotMemory) showing multiple instances of transient view models or services retained by `System.EventHandler` or `System.Action` references.

**Resolution:**
1. Implement `IDisposable` on the subscriber and explicitly unsubscribe with `-=`.
2. Use a Weak Event Pattern (`WeakReference<T>` or `WeakEventManager`).
3. Refactor in-process event handlers into scoped mediator notifications (e.g., MediatR or channel-based pub/sub).

---

### Q3: Why does C# produce a hidden class when a lambda captures an outer variable? What is this class called, and how does it affect memory?
**Answer:**
When a lambda or anonymous method references a local variable or parameter outside its own scope, the variable is "captured in a closure." Because the local variable lives on the thread call stack, it will cease to exist when the outer method returns. However, the delegate might be invoked long after the method returns.

To preserve the captured state, the Roslyn compiler generates a hidden display class (named `<>c__DisplayClassN_M`). It moves ("hoists") the local variable from the stack into an instance field on this display class. The lambda itself becomes an instance method on this display class.
Each time the enclosing method runs, `new <>c__DisplayClass()` is allocated on the managed heap. In high-frequency code paths, this produces continuous Gen0 garbage collections. Using `static` lambdas (`static x => x + 1`) ensures the compiler disallows state capture, preventing display class instantiation.
