# M5D7Q-A TMP menu-label layout correction — Luna implementation recheck R2

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: exact source review only; no Unity launch, execution, or implementation edit
- Contract SHA-256: `CD86E1435FA1E5432DECCAB711F282198641D363997BCDCBA0966E93EF864D78`
- Proposal SHA-256: `749CBF21D147304C6C5B992AA5B720681C80BC9B3EDD76B2E737D4A47A102353`
- Builder SHA-256: `001D6B24B605EDABA837D11389E4732E365C4086CF288C4DBB89D88A313BD4A4`
- Validator SHA-256: `4499F560F762FF7B369D5C1BF2D4FE4BCC8BB8C0467BCCEB8068DD3A49E9A441`
- Generator SHA-256: `42018A79470DA04388045D06CC999580D6EB604AED7A4053B987CD31EE7E7696`
- Authoring tests SHA-256: `642625C19415516EFE41AC9E3A4F2ACA97B59396F7062CE1FB714CC35CF83E02`
- Scope tests SHA-256: `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`
- Resolution tests SHA-256: `769314F30379EE5B2AA9533BB2204126DFAD7119B0D849B372A38C325FB3CF86`
- Prefab/scene state: intentionally legacy; not changed or executed by this gate

## Verdict

**BLOCKED — P0=0, P1=1, P2=1.** The previous migration finding is mostly
closed, but exact meta-byte restoration and one injected failure point remain
unproven/incorrect. `builder-pass-f` remains unauthorized.

## P1 finding — rollback captures meta but does not restore it

`LabelMigrationSnapshot.Take()` correctly captures prefab, scene, both `.meta`
byte arrays, hashes and GUIDs. However `RestoreAndAssert()` writes only the
prefab and scene bytes before refresh, then merely checks the captured meta
hashes/GUIDs. If a post-save/refresh failure changes either `.meta`, rollback
does not restore it; it reports rollback failure instead of restoring the exact
pre-invocation state required by the proposal. The same gap applies to a
`SaveAsPrefabAsset` failure after Unity has touched metadata.

The rollback path must write both captured meta byte arrays (with the existing
GUID/hash assertions retained), refresh/reload, and then prove all four byte
sets and GUIDs. Aggregate-failure handling and candidate unload are otherwise
present.

## P2 finding — one actual injector boundary is not covered

The builder exposes six failure points, including `BeforeSceneValidation`, and
the migration invokes all six. The supplied authoring test cases exercise
`BeforeSave`, `AfterSave`, `AfterRefresh`, `AfterReloadValidation`, and
`AfterSceneValidation`, but omit `BeforeSceneValidation`. Add that test case or
provide equivalent allowlisted injected evidence before re-gating.

## Closed checks

- Candidate migration now uses `LoadPrefabContents`, validates exact all-four
  legacy geometry, mutates only the candidate, validates new geometry, performs
  one `SaveAsPrefabAsset`, unloads, refreshes, reload-validates, validates the
  Hub scene, and restores on injected/post-save failures.
- The canonical path is a no-op; mixed/partial/foreign profiles fail closed.
  Broad `AssetDatabase.SaveAssets` was removed from the migration path.
- Builder/validator/generator/test hashes match the submitted identities. The
  generator asserts all four `12,5,168,22` rects and fingerprints the bounded
  implementation/evidence surfaces.
- No product/runtime scope, GUID identity policy, TMP predicate, capture
  rollback, or C#9 plausibility issue beyond the findings above was found.
