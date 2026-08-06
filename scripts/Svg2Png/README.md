# Svg2Png

A small .NET tool (using [Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia)) that batch-rasterizes SVG source to PNG, since this environment has no system SVG rasterizer (no ImageMagick/Inkscape/rsvg-convert). This is the "SVG first, export PNG automatically" half of [IMAGE_GUIDE.md](../../IMAGE_GUIDE.md).

## Usage

```bash
cd scripts/Svg2Png
dotnet run -- <file-or-directory> [scale]
```

- `<file-or-directory>` — a single `.svg` file, or a directory to convert recursively.
- `[scale]` — render scale multiplier (default `2.0`, i.e. 2x for retina). Chapter covers (1600×900 source) typically use `2.0`; small icons in `assets/components/` can use a higher multiplier since the source is only `64×64`.

## Output path convention

The tool infers where the PNG belongs from the source path, so you don't pass an output path:

| Source pattern | Output |
|---|---|
| `chapters/NNN-slug/diagrams/svg/NNN-cover.svg` | `chapters/NNN-slug/images/NNN-cover.png` |
| `chapters/NNN-slug/diagrams/svg/*.svg` (anything else) | `chapters/NNN-slug/diagrams/png/*.png` |
| `assets/brand/*.svg`, `assets/components/*.svg`, `assets/templates/*.svg` | sibling `png/` subfolder |

## Examples

```bash
# One chapter's illustrations
dotnet run -- ../../chapters/002-clr/diagrams/svg 2.0

# Every SVG in the whole repo
dotnet run -- ../.. 2.0

# The component icon library, at higher resolution since source is 64x64
dotnet run -- ../../assets/components 8.0
```

## Notes

- Re-run after editing any SVG — PNGs are not auto-regenerated on save, this is a manual (or CI-scriptable) step, matching the manual-until-automated posture described in [PUBLISHING_GUIDE.md](../../PUBLISHING_GUIDE.md).
- If a chapter's `diagrams/svg/` has no cover file (`*-cover.svg`), only that check is skipped — everything else still converts.
