# Costume CUA R3-P1-002 aggregate mutation-matrix review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static aggregate ruling only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Sol proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Current adapter-test SHA-256: `2D82B0F2415F61B3B634699152DD19A8E43116C7D732D0937F000C3414A0333A`
- Runtime media SHA-256: `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79`
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`

## Closed sub-gates

The current test file has distinct, real mutation arrays and adapter-bound assertions for:

- B1a: 23 current-tuple corruption rows, including valid control, terminal `ReloadRequired`, and no-save/no-projection preservation (`8BFF4B00DEAD6F9179B474B01E1355D24B21FC8E4B0344C3752AD6544DC2E33C`).
- B1b1: 12 payload/reference/import/profile rows with exact validation categories and B1a preservation (`A3AE5341F0B9D1254ADA761FF7D79F1DC38CE54594F47B811F9336B85562AAB2`).
- B1b2: 23 constructor/geometry/pivot/baseline/duplicate rows, including widened-edge rejection, exact-edge acceptance, and cross-clip reuse (`608649781756BD1045ABDC67D6F7567486F96BF682C2C772DE7B7DF1F775782E`).
- B2: 34 syntactically real clip-map byte mutations, hash rebound to reach `Manifest`, exact rejection, B1a preservation, and canonical positive control (`56DDA793397DCF6AAAB2FCB3D79FE78AAB8971E834492FB20E02F0CFA0BDAD69`).
- B3: 28 syntactically real source-manifest byte mutations, independent payload-hash preservation, exact rejection, B1a preservation, and canonical positive control (`7EF8082515E047C833A3EF56B592D12E0DFC6A29D873BFDC828F5BBA585856CA`).

These sub-gates close the previously broad identity, revisions, binding, payload, reference, import/profile, frame-value, clip-map, and source-manifest families. The current test explicitly reaches `Validate` and `TrySelect` for the relevant constructible rows; no Unity result is inferred.

## Exact remaining rows

The complete Sol table is not yet closed. The following approved rows are absent from the current test matrix:

1. `clipPropertyOrder`: mutate the nested clip-object property order (`actionId,facing,loop,frames`) while leaving the values unchanged. `R4B2ClipMapByteMutations` has top-level order examples (`reorderSchemaActor`, `reorderActorCostume`, `reorderPpuCell`) but no nested clip-object reorder.
2. `framePropertyOrder`: mutate the nested frame-object property order (`x,y,width,height,pivotXQ1000,pivotYQ1000,baselineY,durationTicks`) while leaving the values unchanged. `frameOrderSwap` swaps frame array elements; it is not a frame-property-order mutation.
3. `foreignPackage`: submit a package from a foreign actor/set as an independent lifetime/staleness candidate. Existing identity rows mutate fields on the fixture candidate and do not provide this lifetime row.
4. `stalePackage`: submit a candidate made stale by the relevant catalog/state revision transition and assert the exact rejection/preservation boundary. The current arrays cover revision mismatches but do not name or exercise this stale-candidate path.
5. `previouslyRejectedCandidate`: retry a candidate that has already been rejected and assert the required unchanged/terminal behavior. No current test row or helper names this path.
6. `missingSnapshotAction`: observe a completed snapshot whose `ActionId` is absent from the candidate package's action vocabulary, then assert rejection and preservation. Current snapshot tests cover actor/monotonicity/age and the valid seven-action matrix, not an unknown action.

The proposal's “candidate whose reference is destroyed before save” portion is covered by the existing `destroyedPortraitBeforeTrySelect` and `destroyedAtlasBeforeTrySelect` B1b1 rows; it is not counted as an additional gap here.

The canonical contract also explicitly fixes nested clip and frame property order, so treating only top-level order or frame-array order as representative would under-cover the required byte-level boundary.

## Decision

**OPEN / BLOCKED — P0=0, P1=6, P2=0.** R3-P1-002 is not CLOSED. Terra must add the six exact rows above, each with a real mutation/fixture and the applicable validation, `TrySelect`, no-CIO/no-projection, tuple/view/status/counter preservation oracle (plus a valid control where applicable). Astra may not restore Approved/Verified or authorize the Unity execution gate on this aggregate evidence. No product decision is needed for these test-only closures.
