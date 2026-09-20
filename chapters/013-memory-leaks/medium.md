# Medium Post: Memory Leaks in a Managed World

A fundamental misconception about .NET is that the Garbage Collector prevents memory leaks. It doesn't. 

While the GC prevents *unmanaged* leaks (forgetting to free pointers), it is powerless against *managed* leaks. A managed leak happens when you hold a strong reference to an object that you no longer need. 

In **Inside .NET Chapter 013**, we tear down the mechanics of the "Lapsed Listener" pattern. We cover:
- How the `MulticastDelegate` structure secretly pins your objects in memory.
- Why static events are so dangerous.
- How objects get promoted to Gen 2 and crash your application with an `OutOfMemoryException`.
- How to architect robust systems using the Weak Event Pattern (`WeakReference<T>`).

Stop assuming the GC will clean up after you. Learn how reachability really works.

Read the full chapter here: [Link]
