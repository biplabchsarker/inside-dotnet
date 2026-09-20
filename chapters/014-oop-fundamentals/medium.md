# Medium Article Draft — OOP Fundamentals, Under the Hood

Every C# developer learns the four pillars of OOP — encapsulation, inheritance, polymorphism, abstraction — early on. Far fewer ever see what they actually compile down to inside the CLR. This chapter closes that gap.

You'll learn:

- What a virtual method call (`callvirt`) actually does at run time: a null check, a Method Table pointer read, a vtable slot index, and an indirect jump — and why each of those steps costs something.
- Why `override` and hiding a method with `new` can make the *exact same object* answer two different ways, depending only on the declared type of whoever's calling it — a gotcha that's easy to introduce and easy to miss in review.
- The measured cost: a virtual call ran **57% slower** than a direct call in our benchmark, and an interface call **59% slower** — but sealing the class let the JIT devirtualize it back to within **4%** of a direct call.
- A constructor-safety nuance we verified directly rather than repeating from memory: calling a virtual member from a base constructor sees a derived class's field *initializers* just fine — it's specifically logic inside the derived constructor's *body* that hasn't run yet. Most explanations of this gotcha get that distinction wrong.

Every number here comes from a real, runnable `BenchmarkDotNet` project you can clone and re-run yourself — nothing in this chapter is asserted from memory.

[Continue reading →]
