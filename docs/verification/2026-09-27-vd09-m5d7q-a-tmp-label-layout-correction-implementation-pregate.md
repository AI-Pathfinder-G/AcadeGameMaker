# M5D7Q-A TMP menu-label layout correction — Luna implementation gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: exact source review only; no Unity launch, execution, or implementation edit
- Contract SHA-256: `CD86E1435FA1E5432DECCAB711F282198641D363997BCDCBA0966E93EF864D78`
- Proposal SHA-256: `749CBF21D147304C6C5B992AA5B720681C80BC9B3EDD76B2E737D4A47A102353`
- Builder SHA-256: `F4FE2429525CDE61418693D7EFEC43B0752B638E1F99F64E12308EF0922D9E94`
- Validator SHA-256: `BFF0EABA039F36F1EF2D5DD5916E0DE8BD05F48697079814F2B7D1FA6CBC794F`
- Generator SHA-256: `42018A79470DA04388045D06CC999580D6EB604AED7A4053B987CD31EE7E7696`
- Authoring tests SHA-256: `C83E3C0F8DE42E760E8C213FA19D2F6D953DE6EC54773459A07E015C8BAC49E9`
- Scope tests SHA-256: `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`
- Resolution tests SHA-256: `769314F30379EE5B2AA9533BB2204126DFAD7119B0D849B372A38C325FB3CF86`
- Prefab state: intentionally legacy; not changed or executed by this gate

## Verdict

**BLOCKED — P0=0, P1=1, P2=1.** The exact source identities are correct, but
the implementation does not yet meet the required atomic migration/rollback
contract and lacks a supplied rollback probe. No `builder-pass-f` execution is
authorized from this gate.

## P1 finding — migration is not atomic or rollback-safe

`MigrateExactLegacyMenuLabelsOrValidate()` first mutates the loaded canonical
prefab in place (all four label RectTransforms), then calls
`PrefabUtility.SavePrefabAsset`, `AssetDatabase.SaveAssets`, and `Refresh`.
There is no pre-invocation byte/SHA/GUID snapshot, no candidate clone or
transaction boundary, and no catch/finally restoration of the original prefab
bytes if validation, save, refresh, reload, or the later `ValidateSceneAsset`
call fails. `Build()` validates the scene only after the prefab has already
been saved, so a scene failure can leave the new prefab persisted.

This violates the proposal's deterministic migration rule: record exact
before bytes, validate/save/reload atomically, and restore only the exact
pre-invocation prefab/scene bytes on any post-save failure. It also leaves an
in-memory dirty candidate on pre-save validation failure. Terra must add the
bounded snapshot/candidate/rollback path before implementation can pass.

## P2 finding — rollback probe is not present in supplied tests

The supplied authoring/scope/resolution test files cover canonical geometry,
legacy/mixed/independent mutation rejection, byte no-op and scene/prefab TMP
checks, but contain no test that injects migration save/reload/post-save
failure and proves exact prefab/scene byte/GUID restoration. The required
`focused-editmode-layout` rollback evidence may be produced by a separately
admitted fixture, but no such implementation/test is present in the supplied
file set and it must be added or explicitly evidenced before the gate is
re-run.

## Closed checks

- Builder uses the exact shared `12,5,168,22` profile; new canonical input is
  intended to no-op, while legacy validation requires all four exact legacy
  labels and rejects mixed profiles.
- Validator covers exact paths, anchors/pivot, geometry/center, scale/rotation,
  margin, font/copy/size/wrapping/alignment/material and scene override policy.
- Capture generator asserts all four new logical rects and fingerprints the
  builder/validator/generator/prefab/scene/test proposal surfaces. Existing
  TMP, RT, bootstrap, no-save, pass A/B, GUID/hash and capture rollback logic
  remains present.
- No obvious C#9 API issue was found in the inspected changes; Unity
  compilation and runtime execution were not performed.

The finding is technical only; no new user product decision is implicated.
