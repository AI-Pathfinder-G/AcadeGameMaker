# Sprite source review — 2026-09-09

Scope: REQ-SPR-002..006/009/010, source material only. This evidence does not accept native atlases or runtime integration.

- Astra: scope, image generation, source saving, integration.
- Sol: bounded contract draft and blocker corrections.
- Terra: offline tooling, metadata tests, raw gallery. No image processing executed.
- Luna: independent visual inspection of raw 01–18, pre-gate review and raw-gallery source review.

## Findings from Luna visual review

Character identities and specified solo/relationship actions are readable in raw01–12. B3 NPC and B6 boss landmarks are identifiable. Raw05 initially has five groups per row; raw12 has a missing reel-row figure and a cable crossing a neighboring cell. Both originals are preserved and replacement v2 sources were generated with six figures/groups per row (Astra visual check; further extraction review pending).

Raw13–18 preserve the adopted Ordan, Taegan, Chaeryun, Mujin, Ragen and creature identities. Taegan and Ragen have long weapons crossing nominal cell territory. Source09 uses uneven row boundaries. These require reviewed extraction boundaries or regeneration, never blind regular-grid slicing.

Raw02/03/04/06/08/09/12/13/14/16/18 contain some detached hearts, surprise marks, dust, arcs or chips. They cannot be included in a clean character atlas without reviewed separation. Fixed shop tables and the creature beam also require stable placement checks.

## Disposition

- AC-SPR-001: pending native packing, alpha, pivot, palette and silhouette checks.
- AC-SPR-002: source identity review performed; final atlas coverage and provenance acceptance pending.
- AC-SPR-003: source solo/pair actions readable; native blush readability and secondary settling remain pending continuous playback.
- AC-SPR-004: pending image-processing permission and deterministic pack verification.
- AC-SPR-005: raw gallery only; native/4x/stage animation evidence pending.
- AC-SPR-006: metadata unit tests reported 3/3 by Terra; complete pixel fault-fixture verification pending.

Luna identified that the first gallery implementation used `fetch` and would not work reliably via local `file://`. Terra was requested to embed the plan so double-click viewing needs no HTTP server. This is a delivery defect, not a game change.

All planned raw sources and their revisions are saved under `images/sprites/production-v1`. Script image processing awaits the explicit user answer requested in this conversation. No Unity files were changed for this package and no playable-asset completion is claimed.

## Viewer source verification

- `viewer.html` now embeds the 18 gallery jobs directly and contains no `fetch()` or other external metadata load, so it is compatible with direct `file://` opening in principle.
- The viewer selects `raw/05-lovers-shop-v2.png` and `raw/12-seoha-boss-v2.png` for the revised sources; all other image paths remain local relative paths.
- `.raw` images use `width:100%; height:auto; image-rendering:pixelated`, with an overflow-safe wrapper and mobile layout rule. This is source validation only; no browser, runtime, or native atlas acceptance was performed.
