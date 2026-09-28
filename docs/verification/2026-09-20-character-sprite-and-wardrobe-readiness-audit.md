# Character sprite and wardrobe readiness audit — 2026-09-20

- Reviewer: Luna, independent read-only inventory audit
- Requested outcome: identify which authored character sprites can truthfully be imported and bound to the planned wardrobe/in-play presentation
- Result: **NOT READY for real-media binding; framework work may proceed behind separate Approved gates**

## Evidence-backed inventory result

- Seryeong accepted combinations: **0 / 432**. The appearance manifest marks the inventory `source_only`; the costume catalog therefore has no Accepted default and correctly resolves to `NoAcceptedDefault`.
- No character raster or `TextureImporter` metadata exists below `Assets/`. Character material remains below `images/` as source art, raw sheets, diagnostics, or rejected revisions.
- Native movement diagnostics cover A1/A2/C2/D3 × H1–H12 and V/T/L/B × H1–H5. Combat, village, relationship, and shop coverage is partial and source-only; it is not a complete two-facing gameplay action package.
- The 18 `production-v1/raw` sheets and story-character boards are concept/source material, not accepted atlases.
- Known exclusions remain excluded: the changed-pose shop hair row, relationship v1 rejected scale, and other manifest-marked rejected revisions.

Primary machine-readable inventories:

- `images/sprites/seryeong-appearance-v3/manifest/inventory.json`
- `images/sprites/seryeong-appearance-v3/manifest/v8-inventory.json`
- `images/sprites/seryeong-appearance-v3/manifest/candidate-media.json`

## Import and presentation risks

- The 64px profile uses 128×128 cells with source top-left baseline/pivot `(64,104)`, equivalent to Unity normalized bottom-left pivot `(0.5, 0.1875)`. Center pivots or guessed slicing would move the feet and break cross-animation alignment.
- At 18 PPU a 64px body occupies about 3.56 world units. A proposed 36 PPU preserves the earlier approximately 1.78-unit body scale, but 36 PPU is not yet approved. Raising the internal render target to 1280×720 could preserve more displayed detail but has separate camera, pixel-perfect, pointer, and performance implications.
- Any future import must use point filtering, exact authored cells, explicit per-sprite pivots/anchors, complete required actions, and atomic portrait/gameplay-package validation.
- Flattened relationship/shop art cannot provide independently movable character layers.
- Provenance prompts and source hashes exist, but the reviewed manifests do not contain explicit author/license/use-right fields. Do not call these media license-cleared until a separate rights record exists.

## Decision and gate consequences

1. Do not copy source-only files into `Assets`, mark them Accepted, expose them as wearable, or bind them to the live renderer.
2. The CIO persistence adapter and synthetic-fixture Unity presentation framework may proceed only through their separate Approved contracts.
3. Real-media work requires a bounded acceptance/import contract naming the exact first outfit package, required actions/facings, hashes, pivot, PPU, and license/provenance record.
4. The eventual wardrobe must label unavailable rows as pending/locked and must never substitute a placeholder as the selected outfit.
5. Astra must obtain the 36 PPU versus 1280×720 internal-render decision before approving the first real 64px gameplay-media import. That decision does not block persistence or unavailable-state framework implementation.

## Traceability

- `AC-APV3-008`: no gameplay import while combinations remain unaccepted.
- `AC-COST-002/005/009`: truthful no-default state, binding completeness, and no pending-media promotion.
- `AC-AP64-001..004`, `AC-SPR-001/006/009`, `AC-APV3-003/005`: cell geometry, filtering, pivot/baseline, action completeness, and presentation constraints.

No repository media, Unity asset, catalog status, source, test, scene, prefab, package, or project setting was changed by this audit.

