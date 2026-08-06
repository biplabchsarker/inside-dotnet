# Inside .NET — Episode 6
## Stack vs Heap: Where Your Data Actually Lives

> *Part II — Memory*

---

<!-- Chapter cover: images/005-cover.png, source at diagrams/svg/005-cover.svg (duplicated from assets/brand/chapter-cover-template.svg) -->

### Learning Objectives

By the end of this chapter, you will be able to:
- Explain why the CLR relies on two fundamentally different memory regions — the stack and the managed heap — instead of one general-purpose allocator.
- Correctly predict where a given value type lives (stack-resident local vs. heap-resident field, boxed value, or closure capture) instead of relying on the "value types are always on the stack" shortcut.
- Describe what's actually inside a stack frame and why frame teardown is a single pointer move rather than per-variable cleanup.
- Explain, mechanically, why `StackOverflowException` cannot be caught, and why increasing stack size doesn't fix unbounded recursion.
- Reason about allocation-pressure trade-offs at the architecture level — when struct-heavy, allocation-avoiding designs are worth their added complexity, and when they aren't.

### Real-world Analogy

Picture a professional kitchen.

Each line cook has their own **cutting board** at their station (**the stack** — per-thread, private). It's small, it's fast to work on, and there's a strict rule: whatever you put on it during an order gets cleared the instant that order is plated (**a stack frame is popped the instant its method returns**). You never have to clean someone else's board, and nobody reaches across the kitchen to use yours — it's local, exclusive, and disposable by nature. Grabbing space on your own board costs nothing more than sliding your hand over — you don't ask anyone's permission (**stack allocation is just moving a pointer**).

Now picture the **walk-in pantry** at the back (**the managed heap** — shared across the whole kitchen, i.e., the whole process). Any cook can walk in and grab shelf space for ingredients that need to outlive a single order — stock reductions started this morning that three different dishes will draw from tonight (**objects referenced from multiple places, or that need to outlive the method that created them**). Claiming a new shelf slot is still quick — you just take the next open spot (**heap allocation, in the common case, is also a pointer bump**). But *nobody* individually decides when a pantry item gets thrown out. That's the head chef's job, done on their own schedule, checking what's still referenced by an active ticket before clearing space (**the GC decides when to reclaim heap memory, non-deterministically, based on reachability**). You can't just walk in and un-use a shelf the moment you're done with it the way you clear your own cutting board.

That non-determinism is the whole story: fast, private, auto-cleared vs. shared, flexible, cleaned up on someone else's schedule.

### Problem Statement

A running program needs two fundamentally different memory lifetime patterns, and trying to serve both with one mechanism is a bad trade either way:

- **Deterministic, short-lived, strictly nested lifetimes** — a method's local variables and parameters exist for exactly the duration of that call, nested perfectly inside the caller's lifetime (call `B()` from `A()`, `B`'s locals are always gone before `A`'s). This pattern doesn't need a general-purpose allocator at all — a simple, contiguous, LIFO (last-in-first-out) region is enough, and it can be managed with nothing more than moving one pointer up on entry and back down on exit.
- **Shared, unpredictable, possibly long-lived lifetimes** — an object handed to another thread, stored in a collection, captured by a closure, or returned from a method and used long after that method has returned, doesn't fit a strict nesting pattern at all. Its lifetime is determined by *how many other things still reference it*, which can't be known at compile time or resolved by simple scope exit.

This isn't a C#-specific design choice — it's not even a .NET-specific one. **Every process on every mainstream OS gets a call stack from the hardware/OS** (the CPU has stack-pointer and base-pointer registers dedicated to it; `CALL`/`RET` instructions push/pop return addresses onto it) precisely because pattern #1 is universal to how functions call each other in any language, compiled or interpreted. What .NET adds on top of that OS-level primitive is the **second half**: a managed heap with a garbage collector that automates pattern #2's reclamation problem, so you get the flexibility of shared, unpredictable lifetimes without manually tracking every reference (the way C's `malloc`/`free` or C++'s `new`/`delete` force you to). Without that second half, every "outlives the frame" scenario would have to be solved by hand, per object, which is exactly the class of bug (use-after-free, double-free, leaked native handles) that managed runtimes exist to eliminate.

### Visual Explanation

#### 1. Stack frame push/pop across nested calls

```mermaid
sequenceDiagram
    participant OS as OS Thread Stack
    participant A as Main()
    participant B as ProcessOrder()
    participant C as CalculateTotal()

    A->>OS: push frame for Main (locals, return addr)
    A->>B: call ProcessOrder(order)
    OS->>OS: push frame for ProcessOrder<br/>(param 'order' ref, locals)
    B->>C: call CalculateTotal(items)
    OS->>OS: push frame for CalculateTotal<br/>(param 'items' ref, local 'sum')
    C->>OS: return sum
    OS->>OS: pop CalculateTotal frame<br/>(stack pointer moves back up)
    B->>OS: return
    OS->>OS: pop ProcessOrder frame
    A->>OS: Main continues, then returns
    OS->>OS: pop Main frame
```

Every frame is destroyed the instant its method returns, in strict reverse order of creation — that's the LIFO discipline that makes stack management a single pointer move instead of a search for free space.

#### 2. Stack vs. heap memory layout in a process

```mermaid
flowchart TB
    subgraph PROC["One .NET Process"]
        direction TB
        subgraph T1["Thread 1 Stack (~1MB default, private)"]
            F1["Frame: Main()"]
            F2["Frame: ProcessOrder()"]
            F3["Frame: CalculateTotal()"]
            F3 --> F2 --> F1
        end
        subgraph T2["Thread 2 Stack (~1MB default, private)"]
            F4["Frame: HandleRequest()"]
        end
        subgraph HEAP["Managed Heap (shared, GC-managed)"]
            O1["Order instance"]
            O2["List&lt;OrderLine&gt; instance"]
            O3["Customer instance"]
        end
    end
    F2 -. "reference field points into" .-> O1
    F4 -. "reference field points into" .-> O1
    O1 -->|field: Lines| O2
    O1 -->|field: Customer| O3
```

Two threads, two independent stacks — but one shared heap. Both threads can hold references to the *same* heap object simultaneously; only the heap needs a garbage collector, because only the heap has this many-to-one reachability problem.

#### 3. A value type living *inside* a heap object

```mermaid
flowchart LR
    subgraph STACK["Stack frame: CreateOrder()"]
        R["local variable 'order'<br/>(a reference, 8 bytes)"]
    end
    subgraph HEAPOBJ["Heap: the Order object"]
        HDR["Object header<br/>(method table ptr, sync block)"]
        F1["CustomerId: int  ← value type, INSIDE the object"]
        F2["Total: decimal  ← value type, INSIDE the object"]
        F3["Lines: reference → points elsewhere on the heap"]
    end
    R -->|"points to"| HDR
```

This is the diagram that kills the myth: `CustomerId` and `Total` are value types (`int`, `decimal`), but they are **not on the stack**. They're laid out contiguously as part of the `Order` object's memory block on the heap, because that's where the object containing them lives. Only the *reference* to the whole object (`order`) sits on the stack.

#### 4. Allocation cost comparison

```mermaid
flowchart LR
    subgraph SA["Stack allocation"]
        S1["Move stack pointer down<br/>by frame size"] --> S2["Done — O(1),<br/>no search, no lock"]
    end
    subgraph HA["Heap allocation (Gen 0, common case)"]
        H1["Bump allocation pointer<br/>in current segment"] --> H2["Done — also O(1)<br/>*if there's room*"]
        H2 -.->|"segment full"| H3["Trigger GC<br/>(non-deterministic pause)"]
    end
```

#### 5. StackOverflowException: why it can't be caught

```mermaid
flowchart TB
    A["Recursive call N"] --> B["Recursive call N+1"]
    B --> C["Recursive call N+2"]
    C --> D["Stack pointer approaches<br/>guard page at stack limit"]
    D --> E["CPU/OS raises hardware<br/>guard-page fault"]
    E --> F{"Is there stack space left<br/>to run exception machinery?"}
    F -->|No| G["CLR fails fast:<br/>process terminates immediately"]
```

By the time the guard page is hit, there is — by definition — no more room on that thread's stack. Running a `catch` handler, or even the CLR's own exception-dispatch bookkeeping, requires pushing *more* frames onto the very stack that just ran out. There's nowhere left to put them.

*(Standalone Mermaid sources for all five diagrams live under [`diagrams/mermaid/`](diagrams/mermaid/), numbered to match the order above, per [IMAGE_GUIDE.md](../../IMAGE_GUIDE.md).)*

### Under the Hood

1. **The stack is an OS/hardware construct, not a CLR invention.** When Windows creates a thread, it reserves a contiguous region of virtual memory for that thread's stack — **1 MB by default for the main thread** on Windows (configurable via the linker/`ulimit`-equivalent or, for a CLR thread, via `Thread`'s constructor overload that takes `maxStackSize`). The CPU has dedicated registers for this — on x64, `RSP` (stack pointer) and `RBP`/frame pointer conventions — and the `CALL` instruction automatically pushes a return address before jumping; `RET` pops it back off. The CLR doesn't manage this region with a garbage collector; it just uses the mechanism the OS and CPU already provide.

2. **What's actually in a stack frame.** When a method is called, its frame typically contains: the **return address** (where execution resumes in the caller after this method returns), the **incoming parameters** (or references to them, depending on calling convention and JIT decisions), **local variables of value types** declared in the method body, and bookkeeping the JIT needs for exception handling regions (`try`/`catch` scope tables) and, in debug builds, frame pointers for debugger walkability. Reference-typed locals and parameters also live in the frame — but what's stored there is only the **reference** (a pointer-sized value), not the object itself.

3. **Frame teardown is just moving the pointer back.** On method return, the JIT-generated code doesn't zero out or individually "free" each local — it simply restores the stack pointer to where it was before the call, effectively saying "everything above this address is fair game to overwrite on the next call." This is why stack allocation/deallocation is essentially free: no allocator bookkeeping, no fragmentation, no search for free blocks.

4. **The managed heap is what the CLR adds on top.** At process start, the CLR reserves a set of memory segments for the managed heap (organized into GC generations — the subject of [Episode 12 — GC Generations & LOH](../011-gc-generations-loh/article.md)). When you `new` a reference type, the runtime doesn't scan for a free slot the way a general-purpose native allocator might — in the common case it does a **bump allocation**: check if the current generation-0 segment has enough contiguous free space, and if so, hand out the next address and move the "next free" pointer forward. That's why heap *allocation* is nearly as cheap as stack allocation in the fast path.

5. **Heap deallocation is where the two diverge completely.** There is no `RET`-equivalent that frees a heap object. An object becomes eligible for collection only when the GC determines, during a collection cycle, that nothing reachable from any thread's stack, static field, or GC handle still references it. *When* that determination happens is not tied to any line of your code — it's tied to allocation pressure, generation thresholds, and (rarely) explicit `GC.Collect()` calls. This is the deterministic-vs-non-deterministic split that defines the whole chapter.

6. **Object layout on the heap.** A heap object isn't just "your fields." It has an **object header** (a method table pointer used for virtual dispatch and type identity, plus a sync block index historically used for `lock`/`Monitor`), followed by its fields laid out contiguously — value-type fields inlined directly into that block, reference-type fields stored as pointers to *other* heap blocks. This is exactly what Diagram #3 shows, and it's the mechanical reason the "value types = stack" rule breaks down.

7. **How this ties back to the .NET runtime source.** The CLR's GC (`coreclr`'s `gc.cpp`) and the JIT's stack-frame layout logic are both part of the runtime, not the BCL — you can read the actual allocation fast-path (`GCHeap::Alloc` and friends) in the [`dotnet/runtime`](https://github.com/dotnet/runtime) repository. Microsoft's own internal guidance for high-throughput BCL types (e.g. `Span<T>`, `ValueTuple`, `ValueTask`) is built directly on this mechanic: those types are structs specifically so hot paths in the framework itself avoid heap allocation.

### Code Example

**Example** — struct-as-local vs. struct-as-class-field vs. boxing, observed via measured heap growth:

```csharp
// Program.cs — .NET 10 console app
// Demonstrates: (1) a struct as a local vs. the same struct as a field of a
// class instance, observed via heap growth; (2) why unbounded recursion is
// not safely demoable (explained, not executed).

Console.WriteLine("=== Inside .NET: Episode 6 — Stack vs Heap demo ===");

// ---------------------------------------------------------------------------
// Part 1: a struct (value type) as a LOCAL variable.
// 'point' lives in this method's stack frame. Copying it copies the whole
// 12 bytes right there on the stack — no heap allocation happens at all.
// ---------------------------------------------------------------------------
Point3D point = new Point3D(1, 2, 3);
Point3D copy = point; // full value copy, stack-to-stack, no heap involved
copy.X = 99;
Console.WriteLine($"\n[Local struct] point.X={point.X} (unchanged), copy.X={copy.X}");

long before = GC.GetTotalMemory(forceFullCollection: true);

// Allocating many *locals* of a struct: no lasting heap growth, because each
// one is stack-resident for the duration of one loop iteration and never
// escapes into anything that outlives the loop.
long sink = 0;
for (int i = 0; i < 1_000_000; i++)
{
    Point3D transient = new Point3D(i, i, i);
    sink += transient.X + transient.Y; // touch it so the JIT can't optimize it away entirely
}

long afterLocals = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"[Struct locals]      heap before: {before,12:N0} bytes | after: {afterLocals,12:N0} bytes | delta: {afterLocals - before,12:N0} bytes  (sink={sink})");

// ---------------------------------------------------------------------------
// Part 2: the SAME struct as a FIELD of a class instance.
// Now each Point3D lives INSIDE a heap object (PointHolder). The struct
// itself didn't change — where it lives changed, because its container did.
// ---------------------------------------------------------------------------
var holders = new List<PointHolder>(capacity: 1_000_000);
long beforeHeap = GC.GetTotalMemory(forceFullCollection: true);

for (int i = 0; i < 1_000_000; i++)
{
    holders.Add(new PointHolder(new Point3D(i, i, i))); // PointHolder is a class -> heap
}

long afterHeap = GC.GetTotalMemory(forceFullCollection: true);
Console.WriteLine($"[Struct-in-class]    heap before: {beforeHeap,12:N0} bytes | after: {afterHeap,12:N0} bytes | delta: {afterHeap - beforeHeap,12:N0} bytes");
Console.WriteLine($"Held {holders.Count:N0} PointHolder instances, each carrying a Point3D field inline.");

// Keep 'holders' referenced until here so the GC can't collect it mid-measurement.
Console.WriteLine($"Sample check: holders[500000].Point.X = {holders[500_000].Point.X}");

// ---------------------------------------------------------------------------
// Part 3: boxing — a third way a value type ends up on the heap.
// ---------------------------------------------------------------------------
Point3D boxedSource = new Point3D(7, 8, 9);
object boxed = boxedSource; // boxing: allocates a heap object wrapping the struct
Console.WriteLine($"\n[Boxing] boxed object type: {boxed.GetType()} — this Point3D now lives on the heap, wrapped.");

// ---------------------------------------------------------------------------
// Part 4: StackOverflowException — explained, never triggered.
//
// Uncontrolled recursion exhausts the current thread's stack (default ~1MB
// on Windows for the main thread). When the CPU/OS detects the guard page
// at the end of the stack has been hit, the CLR cannot run a catch handler
// or even its own exception-dispatch code, because doing so requires MORE
// stack space than exists. The process is torn down immediately by the
// runtime — StackOverflowException cannot be caught by user code (see
// the "Under the Hood" section above for the full mechanism). Uncomment the
// two lines below ONLY in a disposable process/terminal if you want to see
// it happen for real — it WILL crash this process with no chance to catch
// anything.
// ---------------------------------------------------------------------------
// static void RecurseForever(int depth) => RecurseForever(depth + 1);
// RecurseForever(0);

Console.WriteLine("\nDone. (Uncontrolled recursion demo intentionally left disabled — see comment above.)");

// A value type: when declared as a local, it's stack-resident.
// When it's a field of a class, it becomes part of that class's heap layout.
struct Point3D
{
    public int X;
    public int Y;
    public int Z;

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

// A reference type (class). Its Point3D field is laid out INLINE inside
// this object's heap block — it is not a separate heap allocation, and it
// is not "on the stack" just because it's a struct.
class PointHolder
{
    public Point3D Point;

    public PointHolder(Point3D point) => Point = point;
}
```

Run it with `dotnet run` in [`code/Chapter05.Demo/`](code/Chapter05.Demo/) — the struct-as-local loop shows little to no heap growth, while the struct-as-class-field loop shows clear, measurable heap growth for the exact same struct type and the exact same iteration count. The difference is entirely about the *container*, not the *field's type*. This chapter's demo stays at the Example tier deliberately — [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md) and [Episode 12 — GC Generations & LOH](../011-gc-generations-loh/article.md) build Advanced/Performance-tier benchmarks (`BenchmarkDotNet`, allocation profiling) directly on top of the mechanics established here.

### Performance Notes

- **Stack allocation has effectively zero marginal cost** beyond the instructions to move the stack pointer — there's no allocator to call, nothing to search, no possibility of fragmentation. This is why hot-path code that avoids unnecessary heap allocations (using `struct`, `Span<T>`, `stackalloc`, or `readonly ref struct` types like `ReadOnlySpan<T>`) can meaningfully reduce GC pressure in allocation-sensitive code.
- **`stackalloc`** lets you explicitly allocate a buffer on the stack (typically wrapped in a `Span<T>`) for scenarios like parsing or formatting where you want a scratch buffer that never touches the GC — but it's bounded by available stack space, so it's for small, bounded buffers only, never unbounded-size data.
- **Heap allocation triggering a Gen 0 collection is the actual cost people mean when they say "GC is slow."** The allocation itself is fast; the pause that happens when a generation fills up and needs collecting is where latency shows up, and it scales with the number of *live* objects the GC has to trace, not the number of allocations made — which is why churn-heavy code (allocate-and-immediately-discard patterns) is a classic optimization target even though each individual allocation was "cheap."
- **Struct size matters for stack pressure too, not just heap pressure.** A large struct copied by value on every method call (parameters, returns) can itself become a performance problem — copying 200 bytes on every call adds up — which is why guidance on struct design (generally keep them small, ideally ≤ 16 bytes, or pass by `in`/`ref` when larger) exists independently of the stack/heap question.
- **Deep, unbounded recursion is a stack-*size* problem, not solvable by GC tuning** — it's the one memory exhaustion scenario entirely outside the garbage collector's domain, since the stack isn't GC-managed at all.
- **Measuring this yourself:** `GC.GetTotalMemory(forceFullCollection: true)` (used in the code sample above) is a quick, honest signal for "did this loop grow the heap," but for anything beyond a teaching demo, reach for `dotnet-counters` (`# of Gen 0 Collections`, `Allocation Rate`) or `dotnet-trace` with the GC provider — both give you real allocation-rate and pause data instead of a single before/after snapshot.

### Common Mistakes / Anti-Patterns

- **"Value types go on the stack, reference types go on the heap."** This is the single most repeated — and most incomplete — statement about .NET memory, and this chapter exists partly to correct it precisely:
  - A value type declared as a **local variable in a method** typically lives on the stack (subject to JIT decisions — see "escape analysis" below).
  - A value type that is a **field of a class instance** lives wherever that class instance lives — which is the **heap**, laid out inline as part of the object (Diagram #3).
  - A value type that is **boxed** (assigned to an `object`/interface-typed variable, or passed where boxing is implicit) gets copied into a **new heap allocation** that wraps it — the subject of [Episode 9 — Boxing & Unboxing](../008-boxing-unboxing/article.md).
  - A value type **captured by a lambda or local function that becomes a closure** gets hoisted by the compiler into a compiler-generated **closure class instance on the heap**, because the delegate carrying it may outlive the method that declared it.
  - The correct rule is about **lifetime and reachability**, not about type category: something lives on the stack only if its lifetime is strictly bounded by, and nested inside, a method call's lifetime. Everything with unpredictable, shared, or outliving-the-frame lifetime ends up on the heap, value type or not.
- **Assuming every local variable that "looks stack-like" actually stays on the stack.** The JIT can, and does, decide to spill certain locals or promote them differently based on optimization and escape analysis; conversely, `stackalloc` and `Span<T>` let you *explicitly* request true stack memory for scenarios where you want to guarantee it — a deliberate, opt-in tool, not the default behavior of `struct`.
- **Believing you can `catch (StackOverflowException)` and recover.** You cannot, by design, since .NET 2.0's behavior change: the CLR terminates the process immediately rather than attempting to run user handler code with no stack left to run it safely.
- **Assuming a bigger stack "fixes" deep recursion algorithmically.** Increasing thread stack size (`new Thread(ThreadStart, maxStackSize)`) raises the ceiling, but unbounded or accidentally-infinite recursion (a missing base case, a cycle in recursive data) will still eventually overflow it — the fix is almost always converting to iteration or adding the missing termination condition, not allocating more stack.
- **Thinking heap allocation is "slow" and stack allocation is "fast" as an absolute, universal rule.** In the *common case* both are pointer bumps and comparably cheap; heap allocation becomes comparatively expensive only when it triggers a collection, when the object is large enough to go on the Large Object Heap, or under heavy allocation pressure across many threads contending for allocation contexts.

### Architect's Perspective

**Developer Perspective**

Day to day, this chapter reduces to one habit: know what your container is before you reason about where a value lives. A `struct` local in a tight loop costs you nothing extra; the same `struct` as a class field means every instance of that class carries it on the heap. If you're writing a hot parsing/formatting path, reach for `Span<T>`/`stackalloc` deliberately rather than hoping the JIT keeps things on the stack for you — it's an opt-in tool, not an assumption you get for free. And never write a `catch (StackOverflowException)` — it's dead code that will never execute as written; fix the recursion instead.

**Senior Perspective**

This is where code review actually earns its keep. The moment someone puts a `struct` on a hot path *because* "structs avoid the heap," check what actually contains it — if it's a field on a class, a member of a `List<T>` of reference types, or gets boxed anywhere on that path (passed to an `object`-typed API, or into a non-generic collection), the "avoid the heap" premise silently stopped being true and you've added copy overhead for nothing. The trade-off that actually matters at this altitude: struct-heavy designs trade heap/GC pressure for copy pressure, and copy pressure is invisible in a profiler's allocation view — it shows up as raw CPU time instead, which means a team optimizing purely by watching Gen 0 counts can walk right past a large-struct-copied-on-every-call regression. This is also where "increase stack size" requests in PRs should get pushed back on — it's treating a symptom (recursion depth) instead of the cause (missing base case or an accidental cycle).

**Architect Perspective**

At system scale, the stack/heap distinction becomes an allocation-pressure design decision, and the right call depends entirely on throughput and GC sensitivity, not on a blanket "structs are faster" rule. A high-throughput service — a serialization hot path, a real-time pricing engine, a network protocol parser processing millions of messages a second — genuinely justifies a struct-heavy, allocation-avoiding data model: every avoided heap allocation is one less object the GC has to trace, and at that volume, Gen 0 collection frequency is a first-order latency concern (this is precisely why `System.Text.Json`'s `Utf8JsonReader`, `Span<T>`, and `ValueTask` exist in the BCL itself — Microsoft made the same trade-off internally for the same reason). But that same optimization is actively harmful in a typical CRUD service or internal line-of-business API processing dozens of requests per second: structs bring value semantics (accidental copies, defensive-copy bugs, no polymorphism without boxing) that cost a team real defect and onboarding time, for GC savings that were never the bottleneck in the first place. The architect's job is to identify *where* allocation pressure is actually a measured problem (via `dotnet-counters`/production telemetry, not intuition) and confine the added complexity of struct-heavy design to that boundary — not let it leak into the rest of the codebase as a stylistic default. This is also the layer where "how does Microsoft do it" is a genuinely useful data point: the BCL's own struct usage (`Span<T>`, `ValueTuple`, `DateTime`, `Guid`) consistently targets exactly this profile — small, copy-cheap, used on paths where allocation avoidance measurably matters — not "everything should be a struct."

### Interview Questions

**Q1: Is it true that value types always live on the stack? Give a concrete counterexample.**
A: No. A value type's storage location follows the lifetime of its *container*, not its own type category. A `struct` field on a `class` instance is laid out inline as part of that class instance's memory block, which lives on the heap — so if you have `class Order { public decimal Total; }`, every `Order` instance's `Total` field lives on the heap alongside the rest of that `Order` object, even though `decimal` is a value type. Boxing and closure capture are two more mechanisms that move a value type onto the heap.

**Q2: Walk through exactly what happens on the stack when method `A` calls method `B`, and what happens when `B` returns.**
A: Before the call, the current stack pointer marks the top of `A`'s frame. The `CALL` instruction pushes the return address (the instruction in `A` to resume at) and control transfers into `B`. The JIT-generated prologue for `B` then reserves space by moving the stack pointer further, for `B`'s parameters/locals. When `B` executes `RET` (via its epilogue), the stack pointer is restored to where it was right after the return address was popped, and execution resumes in `A` at that address. No individual local variable is "freed" — the entire region is simply marked reusable by moving the pointer back.

**Q3: Why does the CLR terminate the process on `StackOverflowException` instead of letting you handle it like other exceptions?**
A: Handling any exception — including running a `catch` block or even the CLR's exception-dispatch and stack-unwinding logic itself — requires pushing additional stack frames to do that work. A stack overflow, by definition, means there's no more stack space available on that thread. Attempting to run recovery code in that state risks corrupting adjacent memory or crashing in an unpredictable, unsafe way. Since .NET 2.0, the documented and enforced behavior is immediate process termination rather than attempting unsafe recovery — `catch (StackOverflowException)` is explicitly ineffective by design.

**Q4: Why is the call stack described as an OS/CPU-level mechanism rather than something the CLR invented?**
A: Every process on every mainstream operating system gets a call stack because function calling itself is a hardware-level concept — CPUs have dedicated stack-pointer registers (e.g., `RSP` on x64) and instructions (`CALL`/`RET`) that automatically push/pop return addresses. The OS allocates and protects that memory region (with guard pages) when it creates a thread. C, C++, Rust, and Go programs all rely on this same mechanism with no managed runtime involved. What the CLR contributes specifically is the layer *above* the stack: the managed heap and the garbage collector, which automate reclaiming memory for objects whose lifetime doesn't fit the stack's strict LIFO nesting.

**Q5: If a struct is generally cheaper to allocate than a class because it avoids heap allocation and GC involvement, when would a large struct actually hurt performance?**
A: Structs are copied by value on assignment, on being passed as a parameter (unless passed by `ref`/`in`), and on being returned. A large struct (well beyond pointer-size) copied repeatedly through call chains or loops accumulates real CPU/memory-bandwidth cost that a reference type wouldn't incur (a reference type only ever copies a small pointer). This is why struct design guidance generally recommends keeping structs small (rule of thumb: comparable to or smaller than 16 bytes) or passing larger ones by `in`/`ref` — the "no heap, no GC" benefit of a struct can be outweighed by copy cost if the struct is large and copied often.

**Q6: "Value types go on the stack, reference types go on the heap." Is this statement correct? Justify your answer with an example.**
A: It's an oversimplification that holds only for local variables of value types declared directly in a method body — and even then only in the common case. It breaks down as soon as the value type's *container* has a different lifetime: a `struct` field on a `class` instance is laid out inline as part of that class instance's memory block on the heap (e.g., `class Order { public decimal Total; }` — every `Order`'s `Total` lives on the heap with the rest of the object). Boxing a struct into an `object` allocates a heap wrapper around it. A struct captured by a lambda that becomes a closure gets hoisted into a compiler-generated heap-allocated closure class. The accurate rule: storage location follows the lifetime and location of the containing scope/object, not the type's value/reference category.

### Quiz

1. Is the call stack a .NET-specific mechanism, or does it exist independently of any managed runtime? Explain why.
2. Give a concrete example where a value type (`struct`) ends up stored on the heap, not the stack.
3. What three things are typically found in a method's stack frame?
4. Why can't a `StackOverflowException` be caught with a normal `try`/`catch`?
5. In the common case, is heap allocation in .NET actually slow compared to stack allocation? What's the real source of heap-related cost?

<details>
<summary>Answers</summary>

1. It's independent of any managed runtime — every process on every mainstream OS gets a call stack from CPU/OS mechanisms (dedicated stack-pointer registers, `CALL`/`RET` instructions). Any language, managed or not (C, C++, Rust, Go), relies on the same underlying mechanism. .NET adds the managed heap and GC on top of it, not the stack itself.
2. A struct that is a field of a class instance (e.g., `class Order { public decimal Total; }`) — `Total` is laid out inline as part of the `Order` object's memory block on the heap. Boxing a struct into an `object`, or capturing a struct in a lambda closure, are two other mechanisms that move it to the heap.
3. The return address, the method's parameters (or references to them), and its value-type local variables — plus JIT/runtime bookkeeping for exception-handling regions.
4. Because handling any exception — including running a `catch` block or the CLR's own exception-dispatch/unwinding logic — requires pushing more stack frames to do that work, and by definition there's no stack space left once an overflow is detected. The CLR terminates the process immediately instead of attempting unsafe recovery.
5. No — in the common case heap allocation is also just a pointer bump (bump allocation into the current Gen 0 segment), comparably cheap to a stack pointer move. The real cost isn't the allocation call itself; it's the *non-deterministic* collection pause that happens later when a generation fills up and the GC has to trace and reclaim memory.

</details>

### Summary & Next Chapter

**Key takeaways:**

- The **stack** is per-thread, LIFO, and OS/CPU-provided — not a .NET invention. Its allocation and deallocation are essentially free because they're just pointer moves, and it holds call frames: locals, parameters, and return addresses.
- The **managed heap** is what .NET adds on top: shared across a process's threads, used for objects with unpredictable or outliving-the-frame lifetimes, allocated cheaply (bump allocation) but reclaimed *non-deterministically* by the GC.
- **"Value types on stack, reference types on heap" is an oversimplification.** A value type's location is determined by the lifetime and location of its container — a struct field in a class instance lives on the heap; a boxed struct lives on the heap; a struct captured by a closure lives on the heap.
- **`StackOverflowException` cannot be caught** because by the time it's detected, there's no stack space left to run any handler, including the CLR's own — the runtime terminates the process instead.
- **At architecture scale, this is an allocation-pressure trade-off, not a style preference** — struct-heavy, allocation-avoiding designs earn their complexity in high-throughput, GC-sensitive paths, and cost more than they save everywhere else.
- This distinction isn't cosmetic trivia — it's the foundation for everything else in Part II: value/reference semantics (Episode 7), object allocation (Episode 8), boxing (Episode 9), GC generations (Episode 12), and memory leaks (Episode 14) are all specific consequences of this stack/heap split.

**What's next:** [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/article.md) builds directly on this foundation: now that you know *where* memory lives, the next question is *how* assignment, copying, and equality behave differently depending on whether you're holding a value or a reference to one.

---

**Previous:** [Episode 5 — Assemblies, DLLs & Metadata](../004-assemblies-metadata/article.md)
**Next:** [Episode 7 — Value Types vs Reference Types](../006-value-vs-reference-types/article.md)
