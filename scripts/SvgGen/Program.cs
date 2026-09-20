using System.Text;

class Program
{
    static void Main()
    {
        string outDir = @"../../chapters/014-oop-fundamentals/diagrams/svg";

        File.WriteAllText(Path.Combine(outDir, "014-hero.svg"), GenerateHero());
        File.WriteAllText(Path.Combine(outDir, "014-concept.svg"), GenerateConcept());
        File.WriteAllText(Path.Combine(outDir, "014-internal.svg"), GenerateInternal());
        File.WriteAllText(Path.Combine(outDir, "014-memory.svg"), GenerateMemory());

        Console.WriteLine("SVGs generated.");
    }

    static string GetDefs() => @"
  <defs>
    <linearGradient id=""bgWash"" x1=""0%"" y1=""0%"" x2=""100%"" y2=""100%"">
      <stop offset=""0%"" stop-color=""#FFFFFF""/>
      <stop offset=""55%"" stop-color=""#F8FAFC""/>
      <stop offset=""100%"" stop-color=""#F3F0FC""/>
    </linearGradient>
    <filter id=""softShadow"" x=""-50%"" y=""-50%"" width=""200%"" height=""200%"">
      <feGaussianBlur stdDeviation=""10"" result=""blur""/>
      <feOffset dy=""8"" in=""blur"" result=""offsetBlur""/>
      <feComponentTransfer in=""offsetBlur"" result=""shadow"">
        <feFuncA type=""linear"" slope=""0.22""/>
      </feComponentTransfer>
      <feMerge>
        <feMergeNode in=""shadow""/>
        <feMergeNode in=""SourceGraphic""/>
      </feMerge>
    </filter>
    <marker id=""arrow"" markerWidth=""10"" markerHeight=""10"" refX=""9"" refY=""5"" orient=""auto"">
      <path d=""M0,1 L9,5 L0,9 Z"" fill=""#9CA3AF""/>
    </marker>
  </defs>";

    // cx,cy is the LEFT point of the box's top face. w/d/h are size multipliers.
    static string DrawBox(int cx, int cy, double w, double d, double h, string cTop, string cLeft, string cRight)
    {
        double ux = 55;
        double uy = 27.5;

        double tLeftX = cx, tLeftY = cy;
        double tBottomX = cx + w * ux, tBottomY = cy + w * uy;
        double tRightX = cx + (w + d) * ux, tRightY = cy + (w - d) * uy;
        double tTopX = cx + d * ux, tTopY = cy - d * uy;

        double bLeftX = tLeftX, bLeftY = tLeftY + h * 60;
        double bBottomX = tBottomX, bBottomY = tBottomY + h * 60;
        double bRightX = tRightX, bRightY = tRightY + h * 60;

        return $@"
  <g filter=""url(#softShadow)"">
    <polygon points=""{tLeftX},{tLeftY} {tBottomX},{tBottomY} {tRightX},{tRightY} {tTopX},{tTopY}"" fill=""{cTop}""/>
    <polygon points=""{tLeftX},{tLeftY} {tBottomX},{tBottomY} {bBottomX},{bBottomY} {bLeftX},{bLeftY}"" fill=""{cLeft}""/>
    <polygon points=""{tBottomX},{tBottomY} {tRightX},{tRightY} {bRightX},{bRightY} {bBottomX},{bBottomY}"" fill=""{cRight}""/>
  </g>";
    }

    static string GenerateHero()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">PART III — C# · EPISODE 15</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">OOP Fundamentals</text>

  <!-- Encapsulation: a sealed box with a lock -->
  {DrawBox(220, 480, 2, 2, 2, "#DBEAFE", "#60A5FA", "#2563EB")}
  <g transform=""translate(320,470)"">
    <rect x=""-18"" y=""-6"" width=""36"" height=""28"" rx=""6"" fill=""#FFFFFF"" stroke=""#2563EB"" stroke-width=""3""/>
    <path d=""M -10,-6 L -10,-18 A 10,10 0 0 1 10,-18 L 10,-6"" fill=""none"" stroke=""#2563EB"" stroke-width=""4""/>
  </g>
  <text x=""340"" y=""650"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Encapsulation</text>

  <!-- Inheritance: a parent box with a smaller child box stacked, connected by a line -->
  {DrawBox(650, 380, 2.5, 2.5, 1.5, "#DDD6FE", "#8B5CF6", "#512BD4")}
  {DrawBox(700, 560, 1.5, 1.5, 1.2, "#EDE9FE", "#A78BFA", "#7C3AED")}
  <path d=""M 780 470 L 780 545"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <text x=""790"" y=""700"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Inheritance</text>

  <!-- Polymorphism: one box, three arrows fanning to three different behaviors -->
  {DrawBox(1150, 420, 1.8, 1.8, 1.5, "#F3F4F6", "#9CA3AF", "#4B5563")}
  <path d=""M 1260 430 L 1360 350"" stroke=""#0078D4"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 1260 460 L 1380 460"" stroke=""#0078D4"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 1260 490 L 1360 570"" stroke=""#0078D4"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <text x=""1240"" y=""700"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#4B5563"" text-anchor=""middle"">Polymorphism</text>

  <text x=""800"" y=""800"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">One reference type, many actual behaviors</text>
</svg>";
    }

    static string GenerateConcept()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">CONCEPT OVERVIEW</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">Same Declared Type, Different Actual Behavior</text>

  <!-- List<Animal> container -->
  <rect x=""120"" y=""250"" width=""360"" height=""580"" rx=""16"" fill=""#F3F4F6"" stroke=""#9CA3AF"" stroke-width=""2""/>
  <text x=""300"" y=""290"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""17"" font-weight=""700"" fill=""#374151"" text-anchor=""middle"">List&lt;Animal&gt;</text>

  {DrawBox(200, 380, 1.1, 1.1, 0.8, "#DBEAFE", "#60A5FA", "#2563EB")}
  <text x=""300"" y=""475"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Dog</text>

  {DrawBox(200, 550, 1.1, 1.1, 0.8, "#DDD6FE", "#8B5CF6", "#512BD4")}
  <text x=""300"" y=""645"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Cat</text>

  {DrawBox(200, 720, 1.1, 1.1, 0.8, "#F3F4F6", "#9CA3AF", "#4B5563")}
  <text x=""300"" y=""815"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#4B5563"" text-anchor=""middle"">Animal</text>

  <text x=""300"" y=""220"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""15"" fill=""#6E6E76"" text-anchor=""middle"">every element stored and accessed as 'Animal'</text>

  <!-- Loop calling Speak() -->
  <path d=""M 500 430 L 640 430"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 500 600 L 640 600"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 500 770 L 640 770"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <text x=""570"" y=""410"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""14"" fill=""#6E6E76"" text-anchor=""middle"">.Speak()</text>

  <!-- Results -->
  <rect x=""660"" y=""390"" width=""420"" height=""80"" rx=""12"" fill=""#DBEAFE"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""870"" y=""438"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""17"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Dog.Speak() runs</text>

  <rect x=""660"" y=""560"" width=""420"" height=""80"" rx=""12"" fill=""#DDD6FE"" stroke=""#512BD4"" stroke-width=""2""/>
  <text x=""870"" y=""608"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""17"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Cat.Speak() runs</text>

  <rect x=""660"" y=""730"" width=""420"" height=""80"" rx=""12"" fill=""#F3F4F6"" stroke=""#4B5563"" stroke-width=""2""/>
  <text x=""870"" y=""778"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""17"" font-weight=""700"" fill=""#4B5563"" text-anchor=""middle"">Animal.Speak() runs</text>

  <text x=""1250"" y=""540"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-weight=""700"" fill=""#1B1B1F"" text-anchor=""middle"">The reference type never</text>
  <text x=""1250"" y=""574"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-weight=""700"" fill=""#1B1B1F"" text-anchor=""middle"">changes. The behavior does.</text>
  <text x=""1250"" y=""620"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">That's polymorphism — decided by</text>
  <text x=""1250"" y=""646"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">the OBJECT, not the variable.</text>
</svg>";
    }

    static string GenerateInternal()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">RUNTIME / INTERNAL VIEW</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">The Method Table (vtable) Lookup</text>

  <!-- Dog instance on the heap -->
  <rect x=""100"" y=""260"" width=""320"" height=""160"" rx=""14"" fill=""#DBEAFE"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""260"" y=""295"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Dog instance (on the heap)</text>
  <rect x=""120"" y=""310"" width=""280"" height=""34"" rx=""6"" fill=""#2563EB""/>
  <text x=""260"" y=""333"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13"" font-weight=""700"" fill=""#FFFFFF"" text-anchor=""middle"">Method Table pointer (offset 0)</text>
  <text x=""260"" y=""385"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13"" fill=""#0B3A66"" text-anchor=""middle"">Name = &quot;Rex&quot;  (instance fields)</text>

  <path d=""M 260 344 L 260 460"" stroke=""#2563EB"" stroke-width=""3"" marker-end=""url(#arrow)""/>

  <!-- Dog's Method Table / vtable -->
  <rect x=""100"" y=""470"" width=""320"" height=""280"" rx=""14"" fill=""#FFFFFF"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""260"" y=""505"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Dog's Method Table</text>
  <rect x=""120"" y=""520"" width=""280"" height=""36"" rx=""6"" fill=""#F3F4F6"" stroke=""#9CA3AF""/>
  <text x=""260"" y=""543"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""12.5"" fill=""#374151"" text-anchor=""middle"">slot 0: ToString -> Object.ToString</text>
  <rect x=""120"" y=""562"" width=""280"" height=""36"" rx=""6"" fill=""#FEF3C7"" stroke=""#D97706"" stroke-width=""2""/>
  <text x=""260"" y=""585"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""12.5"" font-weight=""700"" fill=""#92400E"" text-anchor=""middle"">slot 1: Speak -&gt; Dog.Speak</text>
  <rect x=""120"" y=""604"" width=""280"" height=""36"" rx=""6"" fill=""#F3F4F6"" stroke=""#9CA3AF""/>
  <text x=""260"" y=""627"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""12.5"" fill=""#374151"" text-anchor=""middle"">slot 2: Equals -> Object.Equals</text>
  <text x=""260"" y=""670"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""12.5"" fill=""#6E6E76"" text-anchor=""middle"">Dog's override replaced ONLY slot 1</text>

  <!-- Call site -->
  <rect x=""560"" y=""260"" width=""420"" height=""110"" rx=""14"" fill=""#F3F4F6"" stroke=""#4B5563"" stroke-width=""2""/>
  <text x=""770"" y=""300"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""16"" font-weight=""700"" fill=""#1B1B1F"" text-anchor=""middle"">Animal animal = dogInstance;</text>
  <text x=""770"" y=""335"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""16"" font-weight=""700"" fill=""#1B1B1F"" text-anchor=""middle"">animal.Speak();  // callvirt</text>

  <path d=""M 770 372 L 400 490"" stroke=""#D97706"" stroke-width=""3"" stroke-dasharray=""6,4"" marker-end=""url(#arrow)""/>
  <text x=""620"" y=""420"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" fill=""#92400E"" text-anchor=""middle"">1. read Method Table ptr, index slot 1</text>

  <path d=""M 780 580 L 990 580"" stroke=""#D97706"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <text x=""880"" y=""565"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" fill=""#92400E"" text-anchor=""middle"">2. jump here</text>

  <!-- Dog.Speak code -->
  <rect x=""1000"" y=""500"" width=""420"" height=""160"" rx=""14"" fill=""#FEF3C7"" stroke=""#D97706"" stroke-width=""2""/>
  <text x=""1210"" y=""540"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#92400E"" text-anchor=""middle"">Dog.Speak()</text>
  <text x=""1210"" y=""575"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13"" fill=""#78350F"" text-anchor=""middle"">Console.WriteLine(</text>
  <text x=""1210"" y=""600"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13"" fill=""#78350F"" text-anchor=""middle"">  $&quot;{{Name}} says: Woof!&quot;);</text>

  <text x=""800"" y=""800"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""17"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">A direct (non-virtual) call skips both dashed steps — the address is fixed at compile time</text>
</svg>";
    }

    static string GenerateMemory()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">MEMORY / EXECUTION DIAGRAM</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">override vs. new (Method Hiding)</text>

  <!-- Left: override -->
  <rect x=""120"" y=""220"" width=""650"" height=""560"" rx=""16"" fill=""#EFF6FF"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""445"" y=""265"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">override — ONE vtable slot</text>

  <rect x=""170"" y=""300"" width=""250"" height=""70"" rx=""10"" fill=""#FFFFFF"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""295"" y=""340"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13.5"" fill=""#0B3A66"" text-anchor=""middle"">BaseType overridingAsBase</text>

  <rect x=""450"" y=""300"" width=""270"" height=""70"" rx=""10"" fill=""#FFFFFF"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""585"" y=""340"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13.5"" fill=""#0B3A66"" text-anchor=""middle"">DerivedOverride overriding</text>

  <path d=""M 295 372 L 445 460"" stroke=""#2563EB"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 585 372 L 445 460"" stroke=""#2563EB"" stroke-width=""3"" marker-end=""url(#arrow)""/>

  <rect x=""320"" y=""470"" width=""250"" height=""70"" rx=""10"" fill=""#2563EB""/>
  <text x=""445"" y=""510"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#FFFFFF"" text-anchor=""middle"">the SAME object</text>

  <path d=""M 445 545 L 445 610"" stroke=""#2563EB"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <rect x=""270"" y=""620"" width=""350"" height=""80"" rx=""10"" fill=""#DBEAFE"" stroke=""#2563EB"" stroke-width=""2""/>
  <text x=""445"" y=""655"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Both calls resolve through</text>
  <text x=""445"" y=""680"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">the object's OWN vtable slot</text>

  <text x=""445"" y=""740"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""15"" font-weight=""700"" fill=""#107C10"" text-anchor=""middle"">Always: &quot;DerivedOverride.Greet&quot;</text>

  <!-- Right: hiding -->
  <rect x=""830"" y=""220"" width=""650"" height=""560"" rx=""16"" fill=""#FEF2F2"" stroke=""#D83B01"" stroke-width=""2""/>
  <text x=""1155"" y=""265"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-weight=""700"" fill=""#7C2D12"" text-anchor=""middle"">new — TWO unrelated methods</text>

  <rect x=""880"" y=""300"" width=""250"" height=""70"" rx=""10"" fill=""#FFFFFF"" stroke=""#D83B01"" stroke-width=""2""/>
  <text x=""1005"" y=""340"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13.5"" fill=""#7C2D12"" text-anchor=""middle"">BaseType hidingAsBase</text>

  <rect x=""1160"" y=""300"" width=""250"" height=""70"" rx=""10"" fill=""#FFFFFF"" stroke=""#D83B01"" stroke-width=""2""/>
  <text x=""1285"" y=""340"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""13.5"" fill=""#7C2D12"" text-anchor=""middle"">DerivedHiding hiding</text>

  <path d=""M 1005 372 L 1005 460"" stroke=""#D83B01"" stroke-width=""3"" marker-end=""url(#arrow)""/>
  <path d=""M 1285 372 L 1285 460"" stroke=""#D83B01"" stroke-width=""3"" marker-end=""url(#arrow)""/>

  <rect x=""880"" y=""470"" width=""250"" height=""80"" rx=""10"" fill=""#F3F4F6"" stroke=""#9CA3AF"" stroke-width=""2""/>
  <text x=""1005"" y=""500"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" font-weight=""700"" fill=""#374151"" text-anchor=""middle"">resolved at COMPILE TIME</text>
  <text x=""1005"" y=""522"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" font-weight=""700"" fill=""#374151"" text-anchor=""middle"">from the BaseType reference</text>

  <rect x=""1160"" y=""470"" width=""250"" height=""80"" rx=""10"" fill=""#FEE2E2"" stroke=""#D83B01"" stroke-width=""2""/>
  <text x=""1285"" y=""500"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" font-weight=""700"" fill=""#7C2D12"" text-anchor=""middle"">resolved at COMPILE TIME</text>
  <text x=""1285"" y=""522"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""13"" font-weight=""700"" fill=""#7C2D12"" text-anchor=""middle"">from the DerivedHiding reference</text>

  <text x=""1005"" y=""600"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""14"" font-weight=""700"" fill=""#374151"" text-anchor=""middle"">&quot;BaseType.Greet&quot;</text>
  <text x=""1285"" y=""600"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""14"" font-weight=""700"" fill=""#7C2D12"" text-anchor=""middle"">&quot;DerivedHiding.Greet&quot;</text>

  <text x=""1155"" y=""740"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""15"" font-weight=""700"" fill=""#D83B01"" text-anchor=""middle"">Same object — TWO different answers</text>
</svg>";
    }
}
