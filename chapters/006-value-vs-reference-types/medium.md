# Inside .NET — Episode 7: Value Types vs Reference Types

*Part II — Memory*

You write `b = a;`. What actually gets copied depends entirely on the type of `a` — and getting this wrong is the root cause of half the "I mutated one variable and another one changed too" (or the opposite) bugs in C# codebases.

## The photocopy and the claim ticket

Hand a colleague a photocopy of a contract (a value type, copied by value): they can annotate it freely, your original page is untouched — they never had your page, just an independent copy of its content. Hand a colleague a claim ticket to a shared locker (a reference type, copied by reference): you both hold tickets, but there's only one locker. If they rearrange what's inside, you see it too the next time you open your locker — because a ticket is a pointer to shared storage, not the storage itself.

## The rule, precisely

- **Value types** (`struct`, `enum`, `int`, `bool`, `double`, `DateTime`) copy their field data on assignment and on pass-by-value.
- **Reference types** (`class`, `interface`, `delegate`, array, `string`) copy the reference — both variables end up pointing at the same object.

## "Value types live on the stack" is imprecise

The accurate rule: a value type lives wherever its *containing storage* lives.

- Local variable → typically the stack.
- Field of a class → inside that class instance, wherever the instance is — almost always the heap.
- Array element → inside the array's storage, and arrays are heap objects.
- Boxed value → its own new heap allocation, created specifically to give the value an object identity.

## Boxing, briefly

When a value type needs to be treated as `object` — added to a non-generic collection, passed where an interface is expected, cast explicitly — the CLR allocates a heap block, copies the value's data into it, and prefixes it with a normal object header. Unboxing copies the data back out. Every box and unbox is a full-field copy plus, for boxing, a fresh allocation. Full depth is reserved for [Episode 8 — Object Allocation](../007-object-allocation/article.md) and the dedicated boxing chapter — here it's just the bridge between the two worlds.

## `readonly struct` and `ref struct`

`readonly struct` proves to the compiler that every field is immutable, letting it skip defensive copies it would otherwise insert before calling members through `in` parameters. `ref struct` — the category `Span<T>` belongs to — is enforced stack-only storage: it can never be boxed, fielded in an ordinary class, captured by a closure, or used as a generic type argument, because it may wrap a reference into stack memory that must never outlive its frame.

## Records: the modern confusion point

`record class` is a *reference type* with compiler-generated value equality. `record struct` is a *value type* with the same generated equality. Both give you member-wise `Equals`, `GetHashCode`, `==`, and `with`-expressions — the equality contract is identical. The copy and storage semantics are not: assigning a `record class` copies a reference; assigning a `record struct` copies the data.

## The mutable struct trap

```csharp
foreach (var p in points) { p.X = 5; }
```

This compiles. It does nothing. `p` is a fresh copy assigned each iteration and discarded at the end of it — the real elements in `points` are never touched. The fix is making structs immutable by default (`readonly struct`) so the mistake can't compile in the first place.

## The "structs are always faster" myth

Small, short-lived, rarely-copied structs beat heap allocation and GC pressure — a real win. But a large struct copied through several method layers pays a full-field `memcpy` at every call boundary, which can outweigh a class's one-time allocation plus cheap pointer copies everywhere else. `in` / `ref readonly` exists to pass large read-only structs by reference instead, avoiding the copy while keeping the immutability guarantee.

## Try it yourself

The [companion demo](code/Chapter06.Demo/Program.cs) shows value-copy vs reference-sharing side by side, a safe reproduction of the mutable-struct-in-a-`foreach` gotcha, and a `record class` vs `record struct` equality comparison.

*Next: [Episode 8 — Object Allocation](../007-object-allocation/article.md)*
