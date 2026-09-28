---
status: Implemented
---

# Seryeong appearance v3 inventory implementation handoff

- Date: 2026-09-13
- Implementer: Terra
- Contract: `2026-09-13-seryeong-appearance-combinations-v3` (Approved)
- Scope: source-only inventory, coverage recipes, and local reference viewer. No image generation, extraction, atlas packing, Unity/runtime, or narrative-state work occurred.

## Delivered behavior

`tools/seryeong_appearance_v3/inventory.py` reads the existing `costumes-v2` inventory and writes source-only manifests:

- `inventory.json` contains the canonical 36 costumes, stable `h1..h12`, and exactly 432 ordered `seryeong.appearance.<costume>.<hair>.v3` identities.
- Each hair style binds to one of the two hashed labeled reference sheets. The figures use the approved label-excluding `[column * 512, row * 512, 512, 460]` rectangles. These are full-composition references, explicitly not hair masks.
- The only source pointers are for `a1`, `a2`, `c2`, and `d3` × `h1..h12`. Their hair mapping state is `unmapped`; no pointer asserts an extraction or composed output.
- `coverage.json` supplies all 25 required named actions for each pair. Non-lovers shop absence is one `declarative_absence` staging record per pair (`absenceIsNotSprite: true`, `output: null`), avoiding a fake transparent PNG. Every other action has left and right authored-facing requirements. Standard actions require six frames; relationship and lovers-shop actions require eight.
- The manifests hold `acceptedCombinationCount: 0`, `source_only` review state, and `production_pending` readiness. Native frame records have not been created.
- `viewer.html` is self-contained for `file:` use: it embeds its inventory metadata, performs no fetch or CDN access, filters by costume/hair/context, and shows CSS crops from the original relative reference files. It states that it is a reference-pair view, not a composited final portrait.
- `candidate-media.json` separately indexes actual local diagnostic media: twelve broad-fringe source diagnostics, three A1 hair movement candidates, the H1 relationship source, and the H1 shop source. All remain source-only except the shop source, which is visibly `rejected` for `RejectedHairRow2`; its replacement shop v2 and prelove result are marked pending roots. Candidate source count is never used as a completion count. The viewer plays available GIF motion previews and links whole PNGs. It links the 64-body contact atlas rather than presenting a 32/64 motion control because no 64-body motion GIF exists.
- The non-lovers absence gate is limited to the external shop scene with `seryeongPresent: false` and `doeonSoloVisit: true`. Prelove accompanied shop actions explicitly require `seryeongPresent: true`; this metadata has no runtime or narrative effect.
- `build_relationship_diagnostic.py` writes the corrected A1/H1 group-preserving relationship, shop v2, and reaction diagnostics at `native/a1/h1/relationship-diagnostic-v2/`. It uses the existing broad flood-mask classifier, then keeps qualifying actual connected components inside each source cell instead of rectangular expanded crops; separately connected partners are retained together while neighboring fragments are excluded. Each action-family row has one fixed 64-body scale based only on its manually proposed, unreviewed Seryeong crown-to-sole height; it never scales to the full group-row width. The prior v1 diagnostic is preserved and recorded as `RejectedScale`. Outputs are separate `6×1` 64-body row atlases and row GIFs, avoiding an empty fourth row. Shop row 2 is explicitly rejected as a changed pose, never relabelled as `arm_link`. All candidates retain `0` accepted combinations.

## Verification evidence

Executed with the bundled Python runtime:

```text
python -m unittest tools/seryeong_appearance_v3/test_inventory.py -v
Ran 6 tests — OK
```

The checks cover `AC-APV3-001` and `AC-APV3-008`: 36 costumes, 12 hairs, 432 unique identities, exactly 48 first-slice identities, zero accepted combinations, and current reference-hash validity. They also cover `AC-APV3-002` and `AC-APV3-006`: 25 unique action definitions, every required appearance/action/facing matrix key, exactly 432 declarative non-sprite absence records, the external-only absence gate, and prelove accompanied-shop presence. Deliberately duplicated hair IDs and a removed required action both fail closed. Every candidate-media link is checked to resolve locally, while its accepted count remains zero.

`AC-APV3-003` through `AC-APV3-007`, `AC-APV3-009`, and `AC-APV3-010` remain unverified because no authored sprite outputs, flattened review, build receipt, independent Luna review, or final integration acceptance exists. Repository truth remains `0/432` Accepted.
