# Chapter 013: Memory Leaks

**TL;DR**
The Garbage Collector only collects *unreachable* objects, not *unused* objects. If a long-lived publisher holds a strong event subscription to a short-lived object, the short-lived object is permanently reachable and will never be collected, resulting in an `OutOfMemoryException`.

## The Core Concept
Managed memory leaks are almost entirely a problem of unintentional strong reference chains. The most common culprit is the **Lapsed Listener** pattern: subscribing to a static or long-lived event using `+=` and forgetting to unsubscribe with `-=`.

## The Mechanics
When you subscribe to an event, the compiler translates this to `Delegate.Combine`. The underlying `MulticastDelegate` holds a `Target` pointer to the subscriber instance. Because the publisher holds the delegate, it implicitly holds a strong reference to the subscriber.

If the publisher is static, it acts as a GC Root. The GC traces from the Root to the Publisher to the Delegate to the Subscriber. The path is unbroken, so the Subscriber is promoted to Gen 2 and lives forever.

## Weak Events
To solve systemic leak issues, architectures often employ the **Weak Event Pattern**. A `WeakReference<T>` holds a pointer to an object but does not prevent the GC from collecting it. When the GC runs, the `WeakReference` simply returns `null` if the object was collected.

## The One-Line Rule
> The GC collects unreachable objects, not unused objects. If a static field, long-lived singleton, or background thread can trace a path to your object, it will never be collected.
