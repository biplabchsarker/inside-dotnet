# Quiz

1. **What determines if an object can be garbage collected?**
<details>
<summary>Answer</summary>
Reachability. If the GC can trace a path from a GC Root (like a static variable or active stack frame) to the object, it is considered reachable and cannot be collected.
</details>

2. **Why does an event subscription create a strong reference?**
<details>
<summary>Answer</summary>
Because C# events are backed by delegates. A delegate instance holds both a `Method` pointer and a `Target` pointer. The `Target` pointer is a strong reference to the instance of the object that owns the method.
</details>

3. **Does setting an object to `null` guarantee it gets collected?**
<details>
<summary>Answer</summary>
No. Setting a variable to `null` only severs that specific reference. If the object subscribed to a static event, the static event still holds a reference to it.
</details>

4. **What happens to objects that are leaked?**
<details>
<summary>Answer</summary>
Because they survive Gen 0 and Gen 1 collections, they are eventually promoted to Generation 2. They will sit in Gen 2 permanently, consuming memory until the application crashes with an `OutOfMemoryException`.
</details>

5. **What does `WeakReference<T>.TryGetTarget()` do?**
<details>
<summary>Answer</summary>
It attempts to retrieve the referenced object. If the GC has already collected the object, it returns `false` and sets the `out` parameter to `null`. If the object is still alive, it returns `true` and provides a strong reference to it.
</details>
