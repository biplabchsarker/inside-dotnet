using System.Text;

class Program
{
    static void Main()
    {
        string outDir = @"../../chapters/011-gc-generations-loh/diagrams/svg";
        
        File.WriteAllText(Path.Combine(outDir, "011-hero.svg"), GenerateHero());
        File.WriteAllText(Path.Combine(outDir, "011-concept.svg"), GenerateConcept());
        File.WriteAllText(Path.Combine(outDir, "011-internal.svg"), GenerateInternal());
        File.WriteAllText(Path.Combine(outDir, "011-memory.svg"), GenerateMemory());
        
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

    static string DrawCube(int cx, int cy, int w, int d, int h, string cTop, string cLeft, string cRight)
    {
        // Isometric projection: W is width in +x/-y direction, D is depth in -x/-y direction.
        // For simplicity, standard dx=w, dy=w/2.
        int topY = cy - (w/2 + d/2);
        int leftX = cx - d;
        int rightX = cx + w;
        int bottomY = cy + h; // actually the vertical drop is h
        
        int p0x = cx, p0y = cy - (w/2 + d/2); // top
        int p1x = cx + w, p1y = cy - d/2 + w/2 - (w/2); // right
        int p2x = cx, p2y = cy; // center
        int p3x = cx - d, p3y = cy - w/2 + d/2 - (d/2); // left

        // actual simple projection:
        int dx = 55; int dy = 27; // base unit
        // scale by w and d (which are multipliers)
        int px0 = cx;
        int py0 = cy - (w * dy + d * dy);
        int px1 = cx + w * dx;
        int py1 = cy - d * dy;
        int px2 = cx + (w - d) * dx;
        int py2 = cy;
        int px3 = cx - d * dx;
        int py3 = cy - w * dy;
        
        // standard simple cube
        p0x = cx; p0y = cy - dy * (w+d);
        p1x = cx + dx * w; p1y = cy - dy * (w-d);
        p2x = cx; p2y = cy + dy * (w+d); // wait, math is simpler if we just use explicit points
        
        // Let's use simple parameters: cx, cy is the BOTTOM center of the TOP face.
        // w is size along bottom-right, d is size along bottom-left, h is height.
        int tCx = cx;
        int tCy = cy;
        int rightX_ = cx + w;
        int rightY_ = cy - w/2;
        int topX_ = cx + w - d;
        int topY_ = cy - (w/2 + d/2);
        int leftX_ = cx - d;
        int leftY_ = cy - d/2;
        
        // using 55 and 27 as units
        int pTopX = cx;
        int pTopY = cy - (w * 27) - (d * 27);
        int pRightX = cx + w * 55;
        int pRightY = cy - (w * 27) + (d * 27);
        int pBottomX = cx + (w-d)*55;
        int pBottomY = cy + (w+d)*27;
        
        // Just use exact relative points. w=width units, d=depth units, h=height units
        int uX = 55;
        int uY = 27;
        
        int top_x = cx;
        int top_y = cy - (w * uY) - (d * uY);
        int right_x = cx + w * uX;
        int right_y = cy - (w * uY) + (d * uY);
        int bottom_x = cx + (w * uX) - (d * uX);
        int bottom_y = cy + (w * uY) + (d * uY); // Wait, this math is confusing. Let's just do it exactly like 010.

        return $@"
  <g filter=""url(#softShadow)"">
    <polygon points=""{cx},{cy-27*w} {cx+55*w},{cy} {cx},{cy+27*w} {cx-55*w},{cy}"" fill=""{cTop}""/>
    <polygon points=""{cx-55*w},{cy} {cx},{cy+27*w} {cx},{cy+27*w+60*h} {cx-55*w},{cy+60*h}"" fill=""{cLeft}""/>
    <polygon points=""{cx},{cy+27*w} {cx+55*w},{cy} {cx+55*w},{cy+60*h} {cx},{cy+27*w+60*h}"" fill=""{cRight}""/>
  </g>";
    }

    // A better DrawCube allowing independent width and depth
    static string DrawBox(int cx, int cy, double w, double d, double h, string cTop, string cLeft, string cRight)
    {
        double ux = 55;
        double uy = 27.5; // let's use 27.5 for exact 2:1 iso
        
        double pTopX = cx;
        double pTopY = cy - (w * uy + d * uy);
        double pRightX = cx + w * ux;
        double pRightY = cy - (w * uy - d * uy);
        double pBottomX = cx + (w - d) * ux;
        double pBottomY = cy + (w + d) * uy;
        double pLeftX = cx - d * ux;
        double pLeftY = cy + (w * uy - d * uy);

        // adjust cy so cx, cy is the top-center
        // wait, let's just make cx, cy the left point of the top face.
        double tLeftX = cx;
        double tLeftY = cy;
        double tBottomX = cx + w * ux;
        double tBottomY = cy + w * uy;
        double tRightX = cx + (w + d) * ux;
        double tRightY = cy + (w - d) * uy;
        double tTopX = cx + d * ux;
        double tTopY = cy - d * uy;

        double bLeftX = tLeftX;
        double bLeftY = tLeftY + h * 60;
        double bBottomX = tBottomX;
        double bBottomY = tBottomY + h * 60;
        double bRightX = tRightX;
        double bRightY = tRightY + h * 60;

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
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">PART II — MEMORY · EPISODE 12</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">GC Generations &amp; the Large Object Heap</text>

  <!-- Gen 0/1 (Blue) -->
  {DrawBox(300, 500, 1.5, 1.5, 1.5, ""#DBEAFE"", ""#60A5FA"", ""#2563EB"")}
  <text x=""380"" y=""650"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Ephemeral (Gen 0/1)</text>

  <!-- Gen 2 (Purple) -->
  {DrawBox(700, 400, 3, 3, 2, ""#DDD6FE"", ""#8B5CF6"", ""#512BD4"")}
  <text x=""865"" y=""650"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Gen 2 (Long-term)</text>

  <!-- LOH (Grey/Green) -->
  {DrawBox(1200, 450, 4, 1.5, 1, ""#F3F4F6"", ""#9CA3AF"", ""#4B5563"")}
  <text x=""1310"" y=""650"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#4B5563"" text-anchor=""middle"">Large Object Heap</text>

  <text x=""800"" y=""800"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""20"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">Not all heap memory is managed the same way</text>
</svg>";
    }

    static string GenerateConcept()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">CONCEPT OVERVIEW</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">The Segment-Budget Pipeline</text>

  {DrawBox(200, 400, 1.5, 1.5, 1, ""#DBEAFE"", ""#60A5FA"", ""#2563EB"")}
  <text x=""280"" y=""500"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Gen 0</text>
  
  <path d=""M 380 430 L 460 430"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>

  {DrawBox(500, 400, 2, 2, 1, ""#DBEAFE"", ""#60A5FA"", ""#2563EB"")}
  <text x=""610"" y=""500"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">Gen 1</text>

  <path d=""M 730 430 L 810 430"" stroke=""#9CA3AF"" stroke-width=""3"" marker-end=""url(#arrow)""/>

  {DrawBox(850, 350, 3, 3, 1.5, ""#DDD6FE"", ""#8B5CF6"", ""#512BD4"")}
  <text x=""1015"" y=""500"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Gen 2</text>

  <!-- LOH branch -->
  <path d=""M 280 550 L 280 650 L 460 650"" stroke=""#9CA3AF"" stroke-width=""3"" stroke-dasharray=""5,5"" marker-end=""url(#arrow)""/>
  <text x=""370"" y=""640"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" fill=""#6E6E76"" text-anchor=""middle"">>= 85,000 bytes</text>

  {DrawBox(500, 600, 4, 1.5, 1, ""#F3F4F6"", ""#9CA3AF"", ""#4B5563"")}
  <text x=""610"" y=""700"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#4B5563"" text-anchor=""middle"">Large Object Heap</text>

</svg>";
    }

    static string GenerateInternal()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">RUNTIME / INTERNAL VIEW</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">The Card Table &amp; Write Barrier</text>

  <!-- Gen 2 Object -->
  {DrawBox(300, 350, 2, 2, 2, ""#DDD6FE"", ""#8B5CF6"", ""#512BD4"")}
  <text x=""410"" y=""530"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#2E1065"" text-anchor=""middle"">Gen 2 Object</text>

  <path d=""M 520 400 L 720 400"" stroke=""#0078D4"" stroke-width=""4"" marker-end=""url(#arrow)""/>

  <!-- Gen 0 Object -->
  {DrawBox(750, 350, 1, 1, 1, ""#DBEAFE"", ""#60A5FA"", ""#2563EB"")}
  <text x=""805"" y=""460"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""700"" fill=""#0B3A66"" text-anchor=""middle"">New Gen 0 Object</text>

  <!-- Card Table -->
  <rect x=""300"" y=""650"" width=""400"" height=""60"" rx=""4"" fill=""#F3F4F6"" stroke=""#9CA3AF"" stroke-width=""2""/>
  <rect x=""400"" y=""650"" width=""100"" height=""60"" fill=""#D83B01""/>
  <text x=""450"" y=""685"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" font-weight=""700"" fill=""#FFFFFF"" text-anchor=""middle"">DIRTY</text>
  <text x=""600"" y=""685"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" fill=""#9CA3AF"" text-anchor=""middle"">clean cards</text>

  <path d=""M 410 550 L 450 630"" stroke=""#D83B01"" stroke-width=""2"" stroke-dasharray=""4,4"" marker-end=""url(#arrow)""/>
  <text x=""500"" y=""800"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""18"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">Write barrier flags the card table — Gen 0 collection skips clean cards entirely</text>
</svg>";
    }

    static string GenerateMemory()
    {
        return $@"<svg viewBox=""0 0 1600 900"" xmlns=""http://www.w3.org/2000/svg"">
{GetDefs()}
  <rect width=""1600"" height=""900"" fill=""url(#bgWash)""/>
  <text x=""100"" y=""90"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""16"" font-weight=""600"" fill=""#6E6E76"" letter-spacing=""3"">MEMORY / EXECUTION DIAGRAM</text>
  <text x=""100"" y=""130"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""34"" font-weight=""700"" fill=""#1B1B1F"">The LOH Size Threshold</text>

  <!-- Left: Gen 0 -->
  <rect x=""250"" y=""300"" width=""450"" height=""120"" rx=""14"" fill=""#EFF6FF"" stroke=""#0078D4"" stroke-width=""2""/>
  <text x=""475"" y=""350"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""18"" font-weight=""600"" fill=""#0B3A66"" text-anchor=""middle"">new byte[84_975]</text>
  <text x=""475"" y=""380"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" fill=""#0B3A66"" text-anchor=""middle"">Total: 84,999 bytes. Gen 0.</text>
  {DrawBox(420, 450, 1, 1, 1, ""#DBEAFE"", ""#60A5FA"", ""#2563EB"")}

  <!-- Right: LOH -->
  <rect x=""900"" y=""300"" width=""450"" height=""120"" rx=""14"" fill=""#F3F4F6"" stroke=""#9CA3AF"" stroke-width=""2""/>
  <text x=""1125"" y=""350"" font-family=""Cascadia Code, Fira Code, monospace"" font-size=""18"" font-weight=""600"" fill=""#4B5563"" text-anchor=""middle"">new byte[84_976]</text>
  <text x=""1125"" y=""380"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""14"" fill=""#4B5563"" text-anchor=""middle"">Total: 85,000 bytes. LOH.</text>
  {DrawBox(1020, 450, 3, 1, 1, ""#F3F4F6"", ""#9CA3AF"", ""#4B5563"")}

  <text x=""800"" y=""750"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""24"" font-weight=""700"" fill=""#1B1B1F"" text-anchor=""middle"">1 Byte Difference = Different Destination</text>
  <text x=""800"" y=""790"" font-family=""Segoe UI, Inter, sans-serif"" font-size=""18"" font-style=""italic"" fill=""#6E6E76"" text-anchor=""middle"">Array header overhead is included in the threshold check</text>
</svg>";
    }
}
