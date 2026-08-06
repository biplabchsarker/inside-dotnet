# Self-Check Quiz — What Really Happens When You Run a .NET Application?

1. What does the C# compiler (Roslyn) actually produce — machine code or something else?
2. What triggers the JIT to compile a specific method?
3. What happens to a method's native code after it's compiled — is it recompiled on every call?
4. Name two runtime services the CLR provides continuously while your managed code runs.
5. What's the key tradeoff Native AOT makes compared to the standard JIT model?

<details>
<summary>Answers</summary>

1. IL (Intermediate Language) plus metadata — not machine code.
2. The method being called for the first time during that process's execution.
3. No — the JIT compiles it once, caches the native code in memory, and every subsequent call reuses that cached code for the life of the process.
4. Any two of: garbage collection, exception handling, security/code access, threading/thread pool scheduling.
5. Faster startup and smaller memory footprint, at the cost of losing runtime features that depend on IL still being present at run time (heavy reflection, runtime codegen).

</details>
