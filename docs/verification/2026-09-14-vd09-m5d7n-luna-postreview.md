# VD-09 M5D7N hub-entry handoff latch — Luna independent post-review

- Contract: [Approved M5D7N](../specs/work-contracts/2026-09-13-vd09-m5d7n-hub-entry-handoff-latch.md)
- Reviewer: Luna (independent; Astra retains acceptance and status authority)
- Review date: 2026-09-14
- Scope: approved contract, M5D7N runtime/test source, M5D7M dependency source, implementation evidence, and R22f–R26 XML artifacts

## Verdict

**PASS — P0=0, P1=0, inherited P2=1.** Astra has marked M5D7N Verified; this report records independent confirmation and does not change contract status.

The final source is consistent with the approved lifecycle, ownership, fail-closed state machine, exact notification transfer, and no-authority boundary. The final focused, dependency-regression, full EditMode, and full PlayMode executions are independently consistent and contain no failed, skipped, or inconclusive tests.

The inherited P2 is the approved topology-scope note: the implementation supports the declared pre-lifecycle authored cohort and makes no historical provenance claim for late-added components. It is not an implementation blocker.

## Independently verified evidence

Each XML was parsed from its NUnit `test-run` element and SHA-256 was recomputed:

| Run | Artifact | Result | SHA-256 | Duration |
|---|---|---:|---|---:|
| R22f | `artifacts/unity-results/m5d7n-r22f-topology-lifecycle.xml` | 3/3; failed/skipped/inconclusive 0 | `61E5ED3E4D5AD6ECE413D111C6D663344F351D5D3F9629857CE3C977B1345698` | 63.255993 s |
| R23 | `artifacts/unity-results/m5d7n-r23-focused.xml` | 69/69; failed/skipped/inconclusive 0 | `E6CCC45CF7AAE63AAAF34144B3E8267F92B71B27983BE38BFFE750DE0826F6EE` | 2384.9165272 s |
| R24 | `artifacts/unity-results/m5d7n-r24-m5d7m-regression.xml` | 117/117; failed/skipped/inconclusive 0 | `1EA09E6C7E3BB7FB4A4CECC0148783C70823E1A1A4F221BFDC70DBF5565AACBB` | 972.2019979 s |
| R25 | `artifacts/unity-results/m5d7n-r25-full-editmode.xml` | 669/669; failed/skipped/inconclusive 0 | `D683F52C461D481B6D044B9057C1CC9C060847E88EFF5885621698313F714106` | 9.837388 s |
| R26 | `artifacts/unity-results/m5d7n-r26-full-playmode.xml` | 803/803; failed/skipped/inconclusive 0 | `713121E90AC66951086150641D58E3322CE1C349AD777A906E08A66018D4AF07` | 3344.1593748 s |

Unity logs identify version `6000.6.0f1`; the final evidence records successful licensing preflight. Earlier licensing failures and R21's superseded fixture failure are explicitly retained as historical diagnostics, not counted as final results.

Current source hashes:

- M5D7N runtime `HubEntryHandoffLatchV1.cs`: `96E38AE8BF0CC8B99A4533DD2AED8A78C98D21F4334359744EEC34B9CD01038A`
- M5D7N focused tests: `9A96ABD3985785FBDE6FAFD2A825B44AD1D5B3AE3F3AFDB61C47DDF4655C1B2E`
- M5D7N implementation evidence: `5BE4CA65918090F6F5008254A3407DA9ACF3582B39A9C748CE6EE8655768A0AD`
- M5D7M `InputRouter.cs`: `1ABA6873EDD3F90050B07B57A61F609F22A98EDCB629269A62E2E8D8D2F84E69`
- M5D7M `DesktopProfileLaunchAdapterV1.cs`: `C1B27EE416315666FA2E7673433735CC78B2DA410D5438D98B48C947075E5C0F`

The M5D7M runtime hashes match the previously reviewed dependency sources; no M5D7M runtime change is present in this review scope.

## Implementation findings

- `Awake` and `Start` validate authored topology and commit only the specified lifecycle states. First `Update` validates the source receipt, exact bound live router, expected notification, and one authorized source take before publication.
- A successful source take is copied into both retained and owned proof fields before comparison. Unexpected, missing, mismatched, or later validation failures cannot silently discard a transferred payload.
- Publication stages notification/state and `_handoffProof`, then assigns `_handoff` last. No fallible operation follows that assignment. Later updates are terminal no-ops, and local notification take is single-shot.
- `ValidateState` closes Pristine, AwaitingStart, AwaitingFirstUpdate, all three Published states, Failed, and ClosedBeforePublication using presence/history and exact ValueType proof equality. Handoff and notification reflection rows cover their backing fields and all new cached proof/history fields.
- The cache optimization is bounded by deep M5D7M validation before transfer and exact copied ValueType equality afterward. Neither M5D7N source nor tests add scene, persistence, router mutation, action ownership, retry, logging, network, RNG, or other forbidden authority.
- R21's inactive-host expectation was corrected in the fixture: actual same-host deactivation reaches `OnDisable` and `ClosedBeforePublication`; external/disabled/inactive-router cases remain fail-closed exception rows. R22f and R23 both pass.

## Acceptance matrix

| Criterion | Status | Independent basis |
|---|---|---|
| AC-M5D7N-001 | PASS | R23 real coordinator scenario table covers clean primary, default, previous, decode repair, and binding-apply repair with exact handoff facts. |
| AC-M5D7N-002 | PASS | R23 covers preservation denial, save failure, commit uncertainty, and preservation-only pending receipts while retaining typed evidence. |
| AC-M5D7N-003 | PASS | R23 covers all notification kinds, expected-none, prior-consumed, unexpected, kind/correlation mismatch, and second local take. |
| AC-M5D7N-004 | PASS | Four authored creation permutations and R22f/R23 lifecycle execution prove convergence only at first Update. |
| AC-M5D7N-005 | PASS | R23 invalid-entry, topology, inactive/disabled, router-state, receipt, and reflection fail-closed rows pass. |
| AC-M5D7N-006 | PASS | R23 pre/post publication disable/destroy and no-source-consumption assertions pass. |
| AC-M5D7N-007 | PASS | R23 duplicate Update, reactivation, duplicate take, and postpublication source no-reread assertions pass. |
| AC-M5D7N-008 | PASS | R23 handoff/state/notification/cached-proof reflection tests and static forbidden-authority scans pass. |
| AC-M5D7N-009 | PASS | R22f, R23, R24, R25, and R26 all report result Passed with failed/skipped/inconclusive counters equal to zero. |

## Findings and recommendation

No new P0, P1, or P2 implementation findings remain. The only residual is the inherited, explicitly accepted P2 topology-scope note recorded by the Approved contract.

**Recommendation:** Astra's Verified status is supported by this independent review. Luna does not independently perform the status transition.
