using Svg.Skia;

if (args.Length < 1)
{
    Console.WriteLine("Usage: svg2png <file-or-directory> [scale] ");
    Console.WriteLine("  If a directory is given, every .svg under it is converted recursively,");
    Console.WriteLine("  writing alongside a diagrams/png/ or images/ sibling based on the source path convention.");
    return 1;
}

var target = args[0];
var scale = args.Length > 1 && float.TryParse(args[1], out var s) ? s : 2.0f;

var files = Directory.Exists(target)
    ? Directory.GetFiles(target, "*.svg", SearchOption.AllDirectories)
    : [target];

int converted = 0;
foreach (var svgPath in files)
{
    var outPath = ResolvePngPath(svgPath);
    Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

    using var svg = new SKSvg();
    svg.Load(svgPath);
    var picture = svg.Picture;
    if (picture is null)
    {
        Console.WriteLine($"SKIP (failed to parse): {svgPath}");
        continue;
    }

    var width = (int)(picture.CullRect.Width * scale);
    var height = (int)(picture.CullRect.Height * scale);
    using var bitmap = picture.ToBitmap(SkiaSharp.SKColor.Empty, scale, scale, SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Premul, SkiaSharp.SKColorSpace.CreateSrgb());
    if (bitmap is null)
    {
        Console.WriteLine($"SKIP (render failed): {svgPath}");
        continue;
    }

    using var data = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
    using var stream = File.OpenWrite(outPath);
    data.SaveTo(stream);

    Console.WriteLine($"{svgPath} -> {outPath} ({width}x{height})");
    converted++;
}

Console.WriteLine($"Converted {converted}/{files.Length} file(s).");
return 0;

// diagrams/svg/NNN-cover.svg -> images/NNN-cover.png (chapter covers)
// diagrams/svg/*.svg (other illustrations)     -> diagrams/png/*.png
// assets/brand/*.svg                            -> assets/brand/png/*.png
// assets/components/*.svg                       -> assets/components/png/*.png
// assets/templates/*.svg                        -> assets/templates/png/*.png
static string ResolvePngPath(string svgPath)
{
    var full = Path.GetFullPath(svgPath);
    var fileName = Path.GetFileNameWithoutExtension(full) + ".png";
    var dir = Path.GetDirectoryName(full)!;

    if (dir.Replace('\\', '/').EndsWith("diagrams/svg") && Path.GetFileName(full).EndsWith("-cover.svg"))
    {
        // chapter cover: sibling ../../images/
        var chapterRoot = Directory.GetParent(dir)!.Parent!.FullName;
        return Path.Combine(chapterRoot, "images", fileName);
    }

    if (dir.Replace('\\', '/').EndsWith("diagrams/svg"))
    {
        var diagramsRoot = Directory.GetParent(dir)!.FullName;
        return Path.Combine(diagramsRoot, "png", fileName);
    }

    // assets/* : write to a sibling png/ folder
    return Path.Combine(dir, "png", fileName);
}
