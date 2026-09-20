# Frequently Asked Questions

**Q: Does setting an object to `null` guarantee it will be garbage collected?**
A: No. Setting a variable to `null` only removes *one* reference to the object. If another object (like an event publisher or a static dictionary) still holds a reference to it, it will not be collected.

**Q: Do memory leaks only happen with events?**
A: No. Events are the most common cause, but any long-lived collection (like a `static List<T>` or a `static Dictionary<K, V>`) will cause leaks if you add items to it and never remove them.

**Q: I subscribed to an event on a local button inside a Window. Do I need to unsubscribe?**
A: Usually, no. If the Button and the Window share the exact same lifetime (they are both created and destroyed together), they will become unreachable together. The GC handles isolated "islands" of references perfectly fine. You only need to unsubscribe if the Publisher lives *longer* than the Subscriber.

**Q: Are closures/lambdas dangerous for memory leaks?**
A: Yes, if passed to a long-lived object. When you use `this` inside a lambda, the compiler captures the `this` reference. If you pass that lambda to a static timer or a long-lived service, your entire class instance is pinned in memory.

**Q: How do I find a memory leak in production?**
A: You take a memory dump (using tools like `dotnet-dump`) and open it in Visual Studio or WinDbg. You find the object that should be dead, and you ask the tool for its **GC Root Paths**. The tool will show you exactly which static field or event handler is keeping it alive.
