# M5B2 input gate evidence

- Status: Verified
- Contract: [M5B2](../specs/work-contracts/2026-09-08-vd07-m5b2-transfer-combat-input-gates.md)
- Requirements: REQ-UX-004/008, REQ-WT-005/006, REQ-COM-001/004

## Review and implementation

Terra identified mixed input envelopes: clearing the whole queue would lose Transfer removal/exposure/lifecycle work and Combat external damage. Luna identified the terminal exact-empty/future-key boundary and the requirement to stage the entire queue before mutation. Astra's contract preserves system fields and keys, explicitly leaving future-key/nonempty-damage terminal rejection in place. Luna pre-gate PASS (P0/P1=0); the nonblocking bootstrap ambiguity was resolved explicitly before implementation approval: an initialized consumer with no completed phase may lock before its first phase, but Apply must never perform initialization itself.

Terra implemented the two driver sinks and read-only defensive DTO collections. Root rejected the first unlock implementation because it rebuilt the dictionary despite the latch-only contract; Terra corrected it. Registered Transfer input validation now precedes initialization. No runtime simulation algorithm, delivery lane, terminal discard rule, asset, settings or assembly changes are authorized or performed by this unit.

Root added terminal integration tests through the existing manual authored graph and active Transfer fixture: actual queued interactive inputs are stripped before cleanup; valid maintenance completes; retained external damage and future Combat keys continue to reject teardown. Active cleanup tests cover normal TargetRemoved and all four lifecycle reasons with no durable-removal claim for lifecycle supersession. Luna separately implements consumer/mutation/cadence tests and performs independent review.

## Verification

Unity preflight passed: editor 6000.6.0f1 and matching signed Hub/embedded licensing clients 1.18.3, entitlement file present, no competing editor/client. All Unity test runs were sequential with runtime/test writers frozen.

First focused run `TestResults-Unity-PlayMode-20260908-173709.xml`: 42 passed, 4 failed, skipped 0, Unity exit 0. All eight newly added terminal cases passed. Three Combat test setups damaged Ordan at bootstrap, violating the existing requirement for initial full boss health. The external request was corrected to target the player and assert actual health loss (input lock is not invulnerability). One Transfer test expected InvalidOperationException for default input; the registered validation path correctly throws ArgumentNullException before initialization, so its oracle was corrected and the uninitialized-state assertion strengthened. No runtime or bootstrap rule was weakened.

Root added three boundary tests plus stronger existing assertions: true unlock keeps queue identity; null/unregistered/different/unprepared/already-consumed calls reject; source/list mutation cannot change queued damage; duplicate/default/late submissions preserve valid work; fresh post-unlock attack deals actual damage.

| Final execution | Passed / total | SHA-256 |
|---|---|---|
| `TestResults-Unity-PlayMode-20260908-174035.xml` focused consumer/terminal fixtures | 49/49 | `FD8239E83163F6A341279ED4AB1A4872B3259D9780C41600CD5EA55AAE568A8A` |
| `TestResults-Unity-EditMode-20260908-174118.xml` full | 440/440 | `0C53FFE8AB58828D29725D42CB850A3E62BC2E9B9F69B6DEDEDC37063F7DEC90` |
| `TestResults-Unity-PlayMode-20260908-174201.xml` full | 396/396 | `6A1BDDBBE6EEB52D6156C429DF7C9BE39E54E1444EB3CF81C1CA993BB4830203` |

All final runs: failed 0, skipped 0, Unity exit 0. The full suites contain 836 distinct tests; the 49 focused tests are a subset, not extra tests. The previous baseline was 440+374; this unit adds 22 PlayMode cases.

## AC evidence mapping

- AC-M5B2-001: both `LockStripsExactAndFuture...` tests and Transfer `UnlockRetainsStrippedFutureWorkAndAcceptsOnlyFreshInteractiveInput` verify queued exact/future actions disappear; Combat fresh post-unlock attack reduces Ordan health to 57 while retained old input generates no attack.
- AC-M5B2-002: `LockedMixedExposureInputClearsActiveRelationWithoutPressCapture` runs the exact exposure-merge phase; `GatePreservesActiveTerminalMaintenanceAndLifecycle` has five normal/lifecycle cases verifying active clear, original reason, revision 2, ordered terminal IDs, Baseline and correct durable-removal disposition.
- AC-M5B2-003: Combat locked/mixed and exact/future tests run real Transfer→Combat→Bridge phases. Player health drops to 4 from retained external damage while player basic attack is absent. Input lock does not create invulnerability; canonical requests and existing delivery processing remain intact.
- AC-M5B2-004: owner/phase tests in both fixtures cover registration identity, null/unregistered/unprepared calls, initialized bootstrap allowance and already-consumed own phases; same-state and unlock retain queue identity.
- AC-M5B2-005: invalid staged default tests leave prior interactive envelopes intact and allow a subsequent valid path; locked mixed submissions retain system data; source mutation and read-only-list write attempts cannot edit queued work; registered default, duplicate and late input rejection preserve valid state.
- AC-M5B2-006: three `GatedTerminalPreservesExactEmptyDiscardBoundary` cases use the actual manual authored graph. Exact t+1 system cleanup and empty Combat closure succeed; retained external damage or a future input key leaves terminal delivery, frozen outcomes and enabled components unchanged on rejection. Existing terminal cases also pass.
- AC-M5B2-007: both `ThirtySixtyAndOneFortyFourGroupingProduceIdentical...GateTrace` tests use integer fixed-step accumulation at 30/60/144 and actual owner phases. Trace comparisons report `firstDivergence=none`. Full regression suites above pass.

Luna independently confirmed final XML counts/hashes, read the production and test changes, and judged AC001–007 PASS with no remaining P0/P1/P2 findings. Astra accepts this bounded implementation as Verified on 2026-09-08. Scoped whitespace validation passed. Global InputMode/InputRouter coordination and automatic boss-death integration remain subsequent approved work, not completed by these sinks.

This does not claim real-device/InputRouter integration, global mode/cross-consumer atomicity, automatic boss-death input locking, a standalone player build or whole-game completion.

## Ownership

- Terra: production implementation.
- Luna: independent adversarial review, consumer fixtures and final XML/log digest.
- Astra: contract, terminal fixture extensions, source review and final integration.
- Ollama/Orca/automations: not used. Existing uncommitted changes and media preserved. No commit or push.
