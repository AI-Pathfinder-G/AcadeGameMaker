# Costume CUA R4-B1a closure review #3

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow closure review of the 21 definition/package/binding validation rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `E1D604221AD6D366AB9F7EAD545DA89215E5790AF3F2AA231CE6EFA3162F7D6D`
- Prior R4-B1a evidence SHA-256: `737946CE106AA5219152C9F5716DA4F0BC9096253B481E65926681A53CEB00A0`

## Coverage that is now present

All 21 rows remain enumerated and each fresh fixture now:

- establishes a valid first publication;
- establishes a non-empty highlight before mutation;
- mutates the candidate definition/package/binding;
- calls `Validate` with an exact expected `CostumeUnityPackageRejectionV1`;
- replaces the corresponding catalog row and calls the real adapter `TrySelect`;
- calls `DeepViewSnapshot` across catalog/state/actor/current/transient/availability/status and every row scalar; and
- asserts the old `Published` object, old media reference, old binding value, move/projection counts and `Ready` status remain unchanged (`E1D6...:20–42`, `:350–356`).

This closes the previously missing highlight, deep-view, media/binding, and adapter-boundary assertions in the test source.

## Exact-category defect

The adapter rejection oracle is not correct for all 21 rows. `ExpectedAdapter` returns `Selection` only for `definitionPending` and `definitionRejected`, and returns `Package` for every other row (`E1D6...:350–351`). However, these four mutations replace the catalog entry for `fixture.costume.synthetic2.v1` with a definition whose costume ID no longer matches the selected media's ID:

- `definitionActor` creates actor `other` and costume `other.costume.synthetic.v1`;
- `definitionCostume` creates costume `fixture.costume.other.v1`;
- `bindingActor` changes binding actor and costume, then `DefinitionFromBinding` installs the changed costume ID;
- `bindingCostume` changes the binding costume to `fixture.costume.unknown.v1`, then installs that changed ID.

`CostumeSelectionServiceV1.Classify` returns `UnknownCostume`/selection rejection when `catalog.Find(media.CostumeId)` no longer finds the selected ID. Therefore `adapter.TrySelect(media)` must return `CostumeUnityAdapterRejectionV1.Selection` for those four rows, not `Package`. The current exact expected category assertion would fail on execution, so the category matrix is not closed.

The remaining category mapping is statically consistent: pending/rejected definitions map to selection rejection; identity-preserving set mutations and synthetic/revision/binding/hash/action mutations reach package rejection. This does not cure the four incorrect expected rows.

## Decision

**OPEN — P1 remains for R4-B1a.** View/highlight/reference/persistence preservation coverage is now present, but the exact adapter rejection oracle is wrong for four rows. Correct `ExpectedAdapter` (or preserve the selected catalog ID while changing only the intended identity field), then request another narrow Luna review. Other R4-B categories remain outside this document.

