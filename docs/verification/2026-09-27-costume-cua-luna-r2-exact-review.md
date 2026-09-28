# Costume CUA R2 exact-SHA independent pre-review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial re-review only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Prior Luna R1 review SHA-256: `E7F519A759B3DF40876CD3F9969816374A4FA831E330444C36FDDC789AFC06FD`
- Terra evidence reviewed: `docs/verification/2026-09-20-costume-cua-implementation-evidence.md`, SHA-256 `053320024C1E5A096EE76A123EF181E3E5D0D42B18F3D57741C662C32F2E3A89`

## Exact current inputs

| Path | SHA-256 | Lines |
|---|---|---:|
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` | 136 |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` | `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB` | 95 |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` | `F02E78466C7FB2972CFA9AFE68753572586E425E30F3B4B4B70DCED2693E3897` | 308 |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` | 26 |

The unchanged media, presenter test, runtime/test assembly, core, presentation, and CIO baselines match the R1 allowlist. No Unity execution result is being inferred from these hashes.

## Findings

### P0 — 0

No unsafe implementation authority or forbidden production-scope mutation was found in this static review.

### P1 — 4 (still blocking execution)

#### R2-P1-001 — CurrentTupleValid defense is implemented, but the required corruption proof is absent

`CostumeUnityPresentationAdapterV1.cs:83-88` now checks the stored state current ID, binding/media/snapshot actor identity, observed-snapshot equality, exact clip/frame correlation, and package validation. This closes the R1 source-level predicate weakness.

The R2 tests do not inject or assert each required stored-tuple corruption: foreign actor, stored-frame mismatch, state/current mismatch, binding identity/revision/hash mismatch, or snapshot/frame mismatch followed by the required `ReloadRequired` terminal behavior. The predicate is therefore not independently demonstrated against the failure modes that motivated the amendment. Add direct corruption probes and assert status, rejection, unchanged durable/public reference, and terminal no-op behavior.

#### R2-P1-002 — The amendment mutation matrix is not present

`AC_CUA_002_SyntheticPackageUsesExactCanonicalHashesAndBothFacings` mutates only the portrait filter (`F02E...:41-59`), and `AC_CUA_002_OverflowBoundsAndDuplicateClipRectangleAreRejected` covers only a within-clip duplicate (`:132-149`). The current suite has no one-row-per-mutation assertions for costume/actor/source identity, catalog/presentation revisions, binding IDs and hashes, portrait/atlas/clip-map/source-manifest bytes, asset references, PPU, pivot, baseline, normal geometry, exact maximum edge, and the remaining canonical clip-map/package fields. The huge frame is merely constructed and its `X` read (`:146-147`); it is not passed through package validation. Cross-clip rectangle reuse is not explicitly asserted as accepted. Implement the complete approved mutation table, including rejection code and no-publication/no-save expectations for every row, plus the accepted cross-clip case.

#### R2-P1-003 — Transaction/fault matrix still does not prove replacement, projection, and one-reference rules

The pre-commit, uncertain commit, wrong-primary, and initial projection paths are exercised (`F02E...:152-198`), but the replacement test (`:109-128`) only observes `MoveCalls == 2` and a different `Published` object. `MemoryPort.Replace` simply delegates to `MoveNoOverwrite` (`:296-299`), so the test does not prove a `CommittedReplacement` outcome, replacement bytes/backup semantics, old tuple preservation on failure, or replacement-specific projection failure. No test observes the exact single-reference assignment boundary; the source comment is not an assertion. The terminal no-op assertions are present for one projection-failure case but not the complete uncertain/replacement fault matrix. Add explicit `Replace`/outcome counters and assertions for successful replacement, wrong-primary, projection fault after replacement, terminal calls, durable bytes, and exactly one authoritative publication reference.

#### R2-P1-004 — Action/frame and mechanics coverage is still metadata-only/incomplete

`AC_CUA_004_AllActionsBothFacingsAndEndpointAgesPreserveCompletedSnapshot` iterates eight fixture actions, both facings, and ages `0/500/999/1000` (`F02E...:201-222`), but asserts only snapshot fields. It does not assert the expected `FrameFor` result for each required seven-action × two-facing × four-age row. The separate endpoint test uses only a two-frame standalone clip (`:62-72`) and cannot prove the full package matrix's loop/non-loop mapping.

`AC_CUA_005_IsolatedMechanicsAndPhysicsProbeRemainByteForByteUntouched` checks position, scale, gravity, freeze, collider size/enabled, four string tokens, and a constant call count (`:224-244`). It does not cover the amendment's required rotation/body settings, collider identity/geometry, hit/hurt/aim/transfer tokens, stats/cooldowns/abilities, target/phase transitions, AI ownership/counters, or rejected/replacement/projection-fault paths. Expand the probe with independently mutated sentinels and per-operation counters, and assert all required paths leave them unchanged.

### P2 — 0

No new lower-priority issue is recorded; the four P1 gaps are sufficient to block execution.

## Decision

**FAIL / BLOCKED — P0=0, P1=4, P2=0.** The R2 hashes are internally consistent and the adapter's `CurrentTupleValid` source predicate is materially stronger, but the four amendment requirements are not all genuinely closed by exact assertions. Unity execution, Astra restoration to Approved/Verified, and any media or catalog acceptance remain unauthorized. Terra may continue only with the exact test/fixture corrections above; a fresh Luna exact-SHA gate is required afterward.

