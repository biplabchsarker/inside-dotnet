// Program.cs — .NET 10 console app
// Demonstrates: (1) compile-time literal interning — identical literals, and
// literals folded from `const` expressions, are the exact same heap object;
// (2) runtime-constructed strings are NOT auto-interned, even when their
// content exactly matches an already-interned literal; (3) string.Intern and
// string.IsInterned, made concrete; (4) Substring's own-reference fast path
// (returning `this` when the requested range is the whole string) versus the
// ordinary case, which always allocates a new string; (5) `new string(char[])`
// never participates in interning automatically; (6) the culture-aware vs.
// ordinal comparison gotcha (the "Turkish I" problem), which is a real,
// production-relevant string-equality bug class distinct from interning.

using System.Globalization;
using System.Text;

Console.WriteLine("=== Inside .NET: Episode 10 — Strings & Interning demo ===");

Console.WriteLine();
Console.WriteLine("--- 1. Literal interning: identical literals are the same object ---");
string literalA = "inside-dotnet";
string literalB = "inside-dotnet";
Console.WriteLine($"  literalA equals literalB (value)?     {literalA.Equals(literalB)}");
Console.WriteLine($"  literalA is literalB (reference)?     {ReferenceEquals(literalA, literalB)}");

Console.WriteLine();
Console.WriteLine("--- 2. `const` expressions fold at compile time, so they intern too ---");
const string constGreeting = "Hello, " + "World"; // folded by the compiler into one literal
Console.WriteLine($"  constGreeting is \"Hello, World\" (reference)? {ReferenceEquals(constGreeting, "Hello, World")}");

Console.WriteLine();
Console.WriteLine("--- 3. Runtime-constructed strings are NOT auto-interned ---");
// "Quokka7Xk" is deliberately a word that appears nowhere else as a literal in
// this file — so the string it produces can't have been interned already by
// some unrelated `ldstr` of the same text elsewhere in the program.
string name = "Quokka7Xk";
string runtimeConcat = BuildGreeting(name); // built at runtime inside the method — never a compile-time literal
string runtimeLiteralEquivalent = "Hello, Quokka7Xk"; // a literal with the SAME content, for comparison
string runtimeBuilder = new StringBuilder("Hello, ").Append(name).ToString();
Console.WriteLine($"  runtimeConcat value:                    \"{runtimeConcat}\"");
Console.WriteLine($"  runtimeConcat equals the literal?        {runtimeConcat.Equals(runtimeLiteralEquivalent)}");
Console.WriteLine($"  runtimeConcat is the literal (ref)?      {ReferenceEquals(runtimeConcat, runtimeLiteralEquivalent)}");
Console.WriteLine($"  runtimeBuilder is runtimeConcat (ref)?   {ReferenceEquals(runtimeBuilder, runtimeConcat)}");

Console.WriteLine();
Console.WriteLine("--- 4. string.Intern and string.IsInterned ---");
// A fresh runtime string built purely from a char array — content matches
// nothing interned yet, so IsInterned genuinely returns null here.
string neverSeenBefore = new string(['Q', 'x', '9', '-', 'r', 'u', 'n', 't', 'i', 'm', 'e']);
Console.WriteLine($"  IsInterned(neverSeenBefore) before interning: {(string.IsInterned(neverSeenBefore) is null ? "null (not interned)" : "found")}");
string internedResult = string.Intern(neverSeenBefore);
Console.WriteLine($"  After string.Intern(neverSeenBefore):");
Console.WriteLine($"    internedResult is neverSeenBefore (ref)?      {ReferenceEquals(internedResult, neverSeenBefore)}");
Console.WriteLine($"    IsInterned(neverSeenBefore) now finds it?     {string.IsInterned(neverSeenBefore) is not null}");
string secondEqualRuntimeString = new string(['Q', 'x', '9', '-', 'r', 'u', 'n', 't', 'i', 'm', 'e']);
Console.WriteLine($"    secondEqualRuntimeString is neverSeenBefore before interning it (ref)? {ReferenceEquals(secondEqualRuntimeString, neverSeenBefore)}");
string secondInterned = string.Intern(secondEqualRuntimeString);
Console.WriteLine($"    After interning it too, both intern results share one object (ref)?   {ReferenceEquals(secondInterned, internedResult)}");

Console.WriteLine();
Console.WriteLine("--- 5. Substring's own-reference fast path ---");
string original = "The quick brown fox";
string wholeRangeSubstring = original.Substring(0, original.Length);
string partialSubstring = original.Substring(4, 5);
Console.WriteLine($"  original.Substring(0, original.Length) is original (ref)? {ReferenceEquals(original, wholeRangeSubstring)}");
Console.WriteLine($"  original.Substring(4, 5) is original (ref)?               {ReferenceEquals(original, partialSubstring)}  -- value: \"{partialSubstring}\"");

Console.WriteLine();
Console.WriteLine("--- 6. `new string(char[])` is never auto-interned ---");
string fromCharArray = new string("inside-dotnet".ToCharArray());
Console.WriteLine($"  fromCharArray equals the literal (value)? {fromCharArray.Equals("inside-dotnet")}");
Console.WriteLine($"  fromCharArray is the literal (reference)? {ReferenceEquals(fromCharArray, "inside-dotnet")}");

Console.WriteLine();
Console.WriteLine("--- 7. Culture-aware comparison is a real correctness risk, not just interning trivia ---");
string protocolToken = "TITLE";
string userInput = "title"; // imagine this arrived from a case-insensitive protocol/file-system lookup
var turkish = CultureInfo.GetCultureInfo("tr-TR");

bool ordinalIgnoreCaseMatch = string.Equals(protocolToken, userInput, StringComparison.OrdinalIgnoreCase);
bool turkishCultureMatch = string.Equals(
    protocolToken.ToLower(turkish),
    userInput.ToLower(turkish),
    StringComparison.Ordinal);
bool invariantMatch = string.Equals(
    protocolToken.ToLowerInvariant(),
    userInput.ToLowerInvariant(),
    StringComparison.Ordinal);

Console.WriteLine($"  \"TITLE\" vs \"title\", StringComparison.OrdinalIgnoreCase:        {ordinalIgnoreCaseMatch}");
Console.WriteLine($"  Both lowered under tr-TR then compared ordinally:               {turkishCultureMatch}");
Console.WriteLine($"  Both lowered invariantly then compared ordinally:               {invariantMatch}");
Console.WriteLine($"  \"I\".ToLower(tr-TR) = \"{"I".ToLower(turkish)}\" (not the ASCII 'i' you'd expect from an invariant/ordinal comparison)");

Console.WriteLine();
Console.WriteLine("--- 8. `==` is value equality (via string.Equals), not reference equality ---");
string valueA = BuildGreeting("Architect");
string valueB = BuildGreeting("Architect");
Console.WriteLine($"  valueA == valueB (operator, value-based)? {valueA == valueB}");
Console.WriteLine($"  ReferenceEquals(valueA, valueB)?          {ReferenceEquals(valueA, valueB)}");

Console.WriteLine();
Console.WriteLine("--- 9. Interning unbounded runtime content is a measurable memory-growth risk ---");
const int uniqueCount = 200_000;

long baselineBytes = ForceFullCollectionAndMeasure();

for (int i = 0; i < uniqueCount; i++)
{
    string discarded = $"never-reused-value-{i}"; // built, used nowhere, eligible for collection immediately
}
long afterDiscardedBytes = ForceFullCollectionAndMeasure();

for (int i = 0; i < uniqueCount; i++)
{
    string.Intern($"never-reused-value-{i}"); // each one is now permanently rooted in the intern pool
}
long afterInternedBytes = ForceFullCollectionAndMeasure();

Console.WriteLine($"  Baseline heap after a full collection:                          {baselineBytes:N0} bytes");
Console.WriteLine($"  After creating+discarding {uniqueCount:N0} unique strings, then collecting: {afterDiscardedBytes:N0} bytes (returns near baseline — ordinary garbage)");
Console.WriteLine($"  After interning {uniqueCount:N0} unique strings, then collecting:            {afterInternedBytes:N0} bytes (does NOT return to baseline — the pool holds every one of them)");

Console.WriteLine();
Console.WriteLine("=== Done ===");

static long ForceFullCollectionAndMeasure()
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    return GC.GetTotalMemory(true);
}

// A non-constant helper so the C# compiler cannot constant-fold this call site —
// the returned string is genuinely built at runtime, on every call.
static string BuildGreeting(string who) => "Hello, " + who;
