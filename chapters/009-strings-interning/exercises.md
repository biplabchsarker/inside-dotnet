# Exercises — Strings & Interning

1. **Watch the intern pool actually grow.** Starting from the memory-retention section in [`code/Chapter09.Demo/Program.cs`](code/Chapter09.Demo/Program.cs), change `uniqueCount` from 200,000 to 500,000 and then to 1,000,000. Re-run with `dotnet run -c Release` and record the "after interning" heap size at each value. Confirm the growth is roughly linear in the number of unique strings interned, and explain in a sentence why that's exactly what you'd expect from a table that never releases entries.

2. **Prove `string.Intern`'s exact return semantics, not just the summary of them.** Build two separate strings with identical content via two different runtime constructions (e.g., one via `string.Concat`, one via `StringBuilder.ToString()`), confirm they're not `ReferenceEquals` to each other, then call `string.Intern` on the first one and afterward on the second one. Confirm both calls return the *same* reference — specifically, the first string's — and explain why interning the second one doesn't add a new pool entry.

3. **Measure whether the `Equals` reference-equality fast path still matters for short strings.** Starting from `ReferenceEqualityFastPathBenchmarks` in [`code/Chapter09.Benchmarks/Program.cs`](code/Chapter09.Benchmarks/Program.cs), change the string length from 500 characters down to 5, and re-run. Record the new ratio between `EqualsAgainstSameReference` and `EqualsAgainstEqualButDifferentReference`, and explain — in terms of what the fast path actually skips — why the ratio shrinks (but doesn't disappear) as the string gets shorter.

4. **Stretch — watch an interning leak happen live with `dotnet-counters`.** Write a small ASP.NET Core minimal API (or a long-running console loop) that interns each incoming request's correlation ID (a new GUID per call) instead of just storing it normally. Run it under load with a simple loop or `curl` script hitting it repeatedly, and attach `dotnet-counters monitor --counters System.Runtime` to the running process. Confirm `GC Heap Size` and `Gen 2 Size` climb steadily with no plateau, then stop interning the GUIDs and confirm the growth stops — this is the exact production diagnostic workflow described in this chapter's Performance Notes, applied to a leak you built yourself.

## Challenge

**Predict the output before running it.**

```csharp
string a = "cat" + "dog";
string b = "catdog";
Console.WriteLine(ReferenceEquals(a, b));

string x = "cat";
string y = "dog";
string c = x + y;
string d = "catdog";
Console.WriteLine(ReferenceEquals(c, d));
```

Before running this: do both lines print the same value? Most people expect either both `True` (since `"cat" + "dog"` and `x + y` look like the same operation, just with the pieces held in variables) or both `False` (since concatenation "obviously" allocates). Write down, in terms of *when* the concatenation actually happens — compile time versus runtime — why the first line and the second line give different answers, even though `a` and `c` end up holding identical text either way.
