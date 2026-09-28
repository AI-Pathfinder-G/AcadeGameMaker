# Costume CUA R3 exact-SHA independent pre-review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Prior Luna R2 review SHA-256: `52A1EC4221B411124A5B63D7093DCD4D3913AB91203CF7AC72EE7EAFE6E835CB`
- Terra evidence reviewed: `docs/verification/2026-09-20-costume-cua-implementation-evidence.md`, SHA-256 `2BAD44994105EF612AC7AFB45ABD339BB2F66DCFD12C36480382C5568628BD7D`

## Exact current inputs

| Path | SHA-256 | Lines |
|---|---|---:|
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` | 136 |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` | `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB` | 95 |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` | `05B65640A29032D3451BB58130742D6D52DC5FA264580AFE34E600822E932A0F` | 318 |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` | 26 |

The media and adapter remain unchanged from R2. The presenter test and the previously approved runtime/test seams remain unchanged. No Unity result or compilation result is inferred from these hashes.

## Findings

### P0 — 0

No forbidden production-scope mutation or unsafe authority was found in the exact-SHA static review.

### P1 — 4 (still blocking execution)

#### R3-P1-001 — Current-tuple corruption cases are still not directly asserted

The adapter source still has the strengthened `CurrentTupleValid` checks at lines 83–88: actor identity, state current ID, observed-snapshot equality, exact clip/frame correlation, and package validation. This is a source-level improvement over R1.

However, the test file has no direct mutation/injection of the stored published tuple or its nested values for foreign actor, stored-frame mismatch, state/current mismatch, binding identity/revision/hash mismatch, or snapshot/frame mismatch. It therefore does not prove that each corruption reaches `ReloadRequired` before save/projection and then blocks later selection, observation, and highlight. The amendment explicitly requires those negative cases; the evidence ledger cannot substitute for assertions.

#### R3-P1-002 — Complete package mutation matrix is still absent

The new test file still contains only the filter mutation (`:41–59`), within-clip duplicate rejection (`:133–150`), and constructible `Int32.MaxValue` frame read (`:147–148`). It does not enumerate/assert the approved rows for definition/package identity, all catalog/presentation revisions, each binding ID/hash/action-set mutation, portrait/atlas/clip-map/source-manifest byte and canonical-format mutations, null/destroyed references, PPU, pivot, baseline, ordinary and exact-edge geometry, or stale/foreign/rejected candidates.

The maximum frame is not passed through `Validate`, so the test does not prove `Geometry` rejection for `x=Int32.MaxValue,width=1`, `x=Int32.MaxValue-1,width=Int32.MaxValue`, and analogous Y cases. Cross-clip rectangle reuse remains unasserted as an accepted case. The source-level bounds change is not a substitute for the required independent mutation rows and no-save/no-projection/reference-preservation assertions.

#### R3-P1-003 — Replacement bookkeeping improved, but durable/outcome and replacement-projection boundaries remain unproven

The replacement test now asserts `ReplaceCalls == 1` (`:109–130`), and `MemoryPort.Replace` now writes a backup and primary candidate (`:292–308`). This demonstrates that the fixture can enter a replacement-shaped port call, but it does not assert the actual primary bytes, backup bytes, source cleanup, or the concrete CIO result `CommittedReplacement` and expected revision. The test only observes adapter-level success and the new published object.

There is still no replacement-specific projection-fault test. The projection fault at `:175–200` is first publication only. There is also no boundary observer proving the old complete tuple during `Save` and exactly one authoritative reference assignment after durable success, nor a complete replacement fault/terminal matrix proving old tuple/reference and save/projection counters remain unchanged for every pre-save and uncertain path. The amendment requires these exact boundaries.

#### R3-P1-004 — Mechanics/physics probe remains incomplete; frame matrix is now present but not sufficient to close AC-CUA-005

The action test now correctly enumerates the seven required actions, both facings, and ages `0/500/999/1000` (`:202–226`), and asserts every frame field against an independently calculated expected index. This closes the prior frame-matrix omission.

The mechanics test (`:228–248`) still covers only Transform position/scale, Rigidbody gravity/freeze, collider size/enabled, four string tokens, and a constant call count. It omits Transform rotation, Rigidbody identity and remaining body settings, collider identity/geometry, hit/hurt/aim/transfer geometry, stats, cooldowns, abilities, target/phase/AI tokens and simulation-owner counters. It also exercises only first selection plus one accepted observation, not replacement, rejected observation, or projection failure, and does not recursively compare all sentinels/counters after each path. AC-CUA-005 therefore remains open.

### P2 — 0

No separate P2 issue is recorded; the four P1 findings are sufficient to block execution.

## Decision

**FAIL / BLOCKED — P0=0, P1=4, P2=0.** R3 closes the prior frame-matrix gap and improves replacement fixture bookkeeping, but it does not close the required mutation, tuple-corruption, durable replacement/outcome, replacement-projection, or complete mechanics proofs. Astra may not authorize Unity execution or restore Approved/Verified on this evidence. Terra may continue only with the exact allowlisted test/fixture additions above, followed by a fresh Luna exact-SHA gate.

