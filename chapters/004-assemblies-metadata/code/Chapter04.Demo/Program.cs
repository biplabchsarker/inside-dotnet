using System.Reflection;

Console.WriteLine("=== Inside .NET: Episode 5 demo — Assembly manifest & metadata ===");
Console.WriteLine();

Assembly self = Assembly.GetExecutingAssembly();

// 1. Assembly identity — the 4-tuple the CLR actually treats as "this assembly."
AssemblyName name = self.GetName();
Console.WriteLine("-- Assembly identity (the manifest, decoded) --");
Console.WriteLine($"Simple name     : {name.Name}");
Console.WriteLine($"Version         : {name.Version}");
Console.WriteLine($"Culture         : {(string.IsNullOrEmpty(name.CultureName) ? "neutral" : name.CultureName)}");

byte[]? pkt = name.GetPublicKeyToken();
string pktDisplay = (pkt is { Length: > 0 })
    ? Convert.ToHexString(pkt).ToLowerInvariant()
    : "(none — not strong-named)";
Console.WriteLine($"Public key token: {pktDisplay}");
Console.WriteLine($"Full identity   : {self.FullName}");
Console.WriteLine($"On-disk location: {self.Location}");
Console.WriteLine();

// 2. AssemblyRef table — every assembly THIS one depends on, each with its own
//    identity. This is reading the manifest's dependency list directly.
Console.WriteLine("-- Referenced assemblies (AssemblyRef table) --");
AssemblyName[] refs = self.GetReferencedAssemblies();
foreach (AssemblyName r in refs.OrderBy(r => r.Name))
{
    Console.WriteLine($"  {r.Name,-30} v{r.Version}");
}
Console.WriteLine($"  ({refs.Length} referenced assemblies total)");
Console.WriteLine();

// 3. TypeDef table — every type actually defined in this assembly.
Console.WriteLine("-- Types defined in this assembly (TypeDef table) --");
Type[] types = self.GetTypes();
foreach (Type t in types)
{
    Console.WriteLine($"  {t.FullName}  [token: 0x{t.MetadataToken:X8}]");
}
Console.WriteLine();

// 4. MethodDef rows for one specific type, plus their metadata tokens — the exact
//    integers IL uses internally (call/newobj operands) instead of names.
Type demoType = typeof(MetadataDemo);
Console.WriteLine($"-- Methods on {demoType.Name} (MethodDef table), with tokens --");
MethodInfo[] methods = demoType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
foreach (MethodInfo m in methods)
{
    Console.WriteLine($"  {m.Name,-20} token=0x{m.MetadataToken:X8}  declaring type token=0x{m.DeclaringType!.MetadataToken:X8}");
}
Console.WriteLine();

// 5. Prove metadata tokens are stable, compact handles: resolving the SAME token
//    twice against this module returns the SAME MethodBase, no name lookup needed.
Module module = demoType.Module;
int firstToken = methods[0].MetadataToken;
MethodBase resolvedAgain = module.ResolveMethod(firstToken)
    ?? throw new InvalidOperationException("Token did not resolve — should be impossible for a token we just read.");
Console.WriteLine("-- Resolving a metadata token back to a member --");
Console.WriteLine($"Token 0x{firstToken:X8} resolves to: {resolvedAgain.Name}");
Console.WriteLine($"Same member as original lookup: {methods[0].Name == resolvedAgain.Name}");

// A small type purely so the demo has something concrete to enumerate members of.
static class MetadataDemo
{
    public static int Add(int a, int b) => a + b;
    public static string Describe(object o) => o.ToString() ?? "(null)";
    public static void NoOp() { }
}
