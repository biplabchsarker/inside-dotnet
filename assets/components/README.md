# Component Library

Reusable diagram icons, per [VISUAL_LANGUAGE.md](../../VISUAL_LANGUAGE.md). Each SVG is a self-contained `0 0 64 64` viewBox symbol using only [DESIGN_SYSTEM.md](../../DESIGN_SYSTEM.md) colors, ready to `<use>` in hand-authored SVG diagrams, or to reference by name/color when naming Mermaid nodes.

## Seeded in v1

| File | Concept |
|---|---|
| `clr.svg` | CLR / runtime engine |
| `gc.svg` | Garbage Collector |
| `stack.svg` | Call stack |
| `heap.svg` | Managed heap |
| `thread.svg` | Thread |
| `cpu.svg` | CPU core |
| `server.svg` | Server / host |
| `database.svg` | Database |
| `cloud.svg` | Cloud (generic) |
| `docker.svg` | Container / Docker |

## Adding a new component

1. Check this table first — reuse before redrawing.
2. Author at `0 0 64 64`, 2px stroke, rounded joins, palette colors only.
3. Add a row to this table AND to the catalog table in `VISUAL_LANGUAGE.md` in the same change.
