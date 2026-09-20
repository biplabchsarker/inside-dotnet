# Quiz: Delegates & Events

### Question 1
What is the base class of all delegate types in C#?
- A) `System.Object`
- B) `System.Delegate`
- C) `System.MulticastDelegate`
- D) `System.Action`

<details>
<summary>Answer</summary>

**C) `System.MulticastDelegate`** — while `MulticastDelegate` derives from `System.Delegate`, all custom and built-in delegates in C# directly inherit from `System.MulticastDelegate`.
</details>

---

### Question 2
What happens if you combine two delegates returning an integer (`Func<int>`) using `+=` and then invoke the resulting delegate?
- A) Both methods run, and the sum of both return values is returned.
- B) A compile-time error occurs because delegates with return values cannot be multicast.
- C) Both methods run sequentially, but only the return value of the **last** delegate is returned.
- D) Only the first delegate runs.

<details>
<summary>Answer</summary>

**C)** Multicast delegates execute each method in order, but the return value of `Invoke()` is overwritten by each subscriber; only the final subscriber's result is returned to the caller.
</details>

---

### Question 3
What does marking a lambda as `static` (e.g., `static x => x * 2`) do?
- A) Forces the delegate to run on a background thread.
- B) Makes the method thread-safe using a lock.
- C) Prevents the lambda from capturing any enclosing state, guaranteeing zero heap allocations for closures.
- D) Binds the delegate to a static class.

<details>
<summary>Answer</summary>

**C)** The `static` modifier on anonymous functions causes Roslyn to emit an error if any local variable or `this` reference is captured, ensuring no display class instance is allocated on the heap.
</details>

---

### Question 4
Why is `event?.Invoke(this, EventArgs.Empty)` thread-safe compared to `if (event != null) event(this, EventArgs.Empty);`?
- A) It uses a hidden `Monitor.Enter` lock.
- B) The null-conditional operator copies the delegate reference into a local stack variable before testing for null, preventing another thread from nulling the field between the check and call.
- C) It uses `Interlocked.Increment`.
- D) It executes inside the CLR's synchronization context.

<details>
<summary>Answer</summary>

**B)** In C#, `x?.Invoke()` evaluates `x` once into a compiler-generated local temporary. Even if another thread sets the field to null concurrently, the local temporary on the stack remains valid.
</details>
