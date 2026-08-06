# Brand Guide

## Status: v1 — placeholder identity (navigational cover), v2 — illustration system (Hero/Concept/Deep-Dive)

The wordmark, hero cover, and navigational chapter-cover template below are the **v1 brand identity** — simple, hand-coded SVG. Since then, **v2** added a per-chapter *illustration* system — Hero Cover, Concept Illustration, Deep-Dive Illustration — with genuinely more visual craft (gradients, glow filters, blueprint grids) — see [IMAGE_GUIDE.md](IMAGE_GUIDE.md#the-three-signature-illustrations--every-chapter) and the reusable base at [`assets/templates/hero-illustration.svg`](assets/templates/hero-illustration.svg). Both are hand-coded SVG — no illustrated/isometric *artwork* in the commissioned-illustrator or AI-image-gen sense, because neither is in scope here. Treat everything in this document as upgradeable, not final.

## Assets

| Asset | Path | Purpose |
|---|---|---|
| Logo (wordmark) | [`assets/brand/logo.svg`](assets/brand/logo.svg) | Primary mark, used in README header, website nav, slide decks |
| Logo (mark only, no text) | [`assets/brand/logo-mark.svg`](assets/brand/logo-mark.svg) | Favicon-scale usage, social profile picture |
| Hero cover | [`assets/brand/hero-cover.svg`](assets/brand/hero-cover.svg) | Repository social preview, top of README, series-level banner |
| Chapter cover template | [`assets/brand/chapter-cover-template.svg`](assets/brand/chapter-cover-template.svg) | Starting point for every `chapters/NNN-slug/images/NNN-cover.svg` — duplicate and edit the episode number/title/part label |
| Social banner | [`assets/brand/social-banner.svg`](assets/brand/social-banner.svg) | LinkedIn/Medium/Dev.to post header image (1200×630) |

## Logo usage

- Minimum clear space around the logo: 24px (see [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md) spacing unit — 3× base unit).
- On white/light backgrounds, use the full-color logo. On dark backgrounds, use the same mark — it's designed to work on both since it doesn't rely on a light-only background fill.
- Never stretch, recolor outside the defined palette, or add effects (drop shadows, outlines) not already part of the source SVG.
- Do not recreate the wordmark in a different font — always use the SVG source as the single source of truth.

## Color

See [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md#color-palette) for the full palette. Brand-specific usage:

- Primary brand color: `#512BD4` (.NET Purple)
- Logo gradient (where used): `#512BD4 → #0078D4`, left to right

## Chapter covers

Every chapter's cover follows the same layout, defined in `chapter-cover-template.svg`:

- 16:9 aspect ratio
- Top-left: Part label (e.g. "PART I — THE FOUNDATION")
- Center: Episode number (large) + chapter title (below it, smaller)
- Bottom-right: series wordmark (small, from `logo-mark.svg`)
- Background: primary gradient, consistent across all covers — chapters are visually distinguished by title text only, not by a unique background per chapter (this is deliberate: a shelf of chapter covers should read as one series, not 58 different designs)

## Social banners

The social banner (`social-banner.svg`, exported to PNG at 1200×630 for platform compatibility) uses the hero cover's composition at social-card proportions, with the episode-specific title swapped in per post.

## What's explicitly out of scope for v1

- Illustrated/isometric character art or scene illustrations
- Animated logo/intro
- Print-specific brand collateral (business cards, letterhead)

These would be reasonable v2 additions once real design tooling or a commissioned illustrator is involved — track as a backlog item, not a blocker for content work.
