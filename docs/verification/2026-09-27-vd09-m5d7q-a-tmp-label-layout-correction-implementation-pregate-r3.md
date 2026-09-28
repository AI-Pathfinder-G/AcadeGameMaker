# M5D7Q-A TMP menu-label layout correction — Luna implementation recheck R3

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: exact source review only; no Unity launch, execution, or implementation edit
- Contract SHA-256: `CD86E1435FA1E5432DECCAB711F282198641D363997BCDCBA0966E93EF864D78`
- Proposal SHA-256: `749CBF21D147304C6C5B992AA5B720681C80BC9B3EDD76B2E737D4A47A102353`
- Builder SHA-256: `6912C422B27819B68C227A84A29BF58D9ADFAE8261D9DD8503F69320F90D004D`
- Validator SHA-256: `4499F560F762FF7B369D5C1BF2D4FE4BCC8BB8C0467BCCEB8068DD3A49E9A441`
- Generator SHA-256: `42018A79470DA04388045D06CC999580D6EB604AED7A4053B987CD31EE7E7696`
- Authoring tests SHA-256: `9574A3D60E026E337FA21B152EDF4FE927C3127FC08531BB41FD91127EA3780A`
- Scope tests SHA-256: `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`
- Resolution tests SHA-256: `769314F30379EE5B2AA9533BB2204126DFAD7119B0D849B372A38C325FB3CF86`
- Prefab/scene state: intentionally legacy; not changed or executed by this gate

## Verdict

**PASS — P0=0, P1=0, P2=0 (static implementation gate).** Terra may proceed to
the allowlisted `builder-pass-f` execution. This PASS is limited to source
review and is not runtime evidence, post-review, or `Verified` acceptance.

## Closed migration and rollback checks

- `LabelMigrationSnapshot` captures prefab/scene bodies, both `.meta` byte
  arrays, their SHA-256 values, and both AssetDatabase GUIDs. On every
  post-save failure it writes all four captured files, refreshes/reloads, and
  proves all four hashes plus both GUIDs. Aggregate rollback failures preserve
  both the original and rollback exceptions.
- The candidate boundary uses `LoadPrefabContents`; all-four legacy geometry
  and full legacy surface are validated before mutation. The candidate is
  validated at the new profile before one `SaveAsPrefabAsset`; it is unloaded,
  refreshed, reload-validated, scene-validated, and meta/GUID-validated.
  Canonical input remains a no-op; mixed/partial profiles fail closed.
- No broad `AssetDatabase.SaveAssets` is used by the migration path. Existing
  migration failure injection covers all six stages: `BeforeSave`, `AfterSave`,
  `AfterRefresh`, `AfterReloadValidation`, `BeforeSceneValidation`, and
  `AfterSceneValidation`. The authoring test snapshots all four files and
  verifies exact bytes/GUIDs and restored legacy validity for each stage.

## Other checks

- Validator covers exact four paths, legacy/canonical geometry, center,
  anchors/pivot, scale/rotation, margins, copy/font/size/wrapping/alignment/
  material, button containment, and scene prefab override policy.
- Generator asserts all four `12,5,168,22` logical rects and fingerprints the
  bounded implementation/evidence surfaces. Existing TMP/RT/bootstrap,
  no-save, pass A/B, capture hash/GUID and staged rollback gates remain.
- No C#9/API blocker or product/runtime scope expansion was found statically.
