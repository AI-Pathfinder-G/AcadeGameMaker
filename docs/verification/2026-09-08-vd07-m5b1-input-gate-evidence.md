# M5B1 Movement input gate evidence

- Status: Verified
- Requirements: REQ-UX-004/008; REQ-MOV-001/002/007/008
- Contract: [M5B1](../specs/work-contracts/2026-09-08-vd07-m5b1-movement-input-gate.md)

## Impact and scope decision

Terra and Luna independently found that full M5B Unity progression binding lacks the actual InputRouter/InputMode owner, Run/Failure owner, accepted failure provenance and a pre-Transfer transition coordinator. M4B3C teardown at t+1 / -195 occurs after Transfer at -200, so that teardown cannot itself prevent a previously queued Transfer press. M5A remains a value-based progression core, not proof of those missing owners.

Astra therefore selected the concrete Movement command-consumer gate as a prerequisite, not a replacement claim for full M5B. It removes real queued commands and drives existing motor lock physics. This unit does not bind the authored boss graph or automatically lock after boss death. Subsequent work must cover Transfer/Combat queues and a single authoritative input owner before cross-consumer integration, then authoritative Run/profile facts before M5A binding.

## Participation

- Terra: read-only impact; bounded controller implementation.
- Luna: independent failure-mode analysis, contract review and PlayMode test fixture.
- Astra: scope, approval, source review, sequential Unity validation and integration.
- No Ollama, Orca, network outsourcing or scheduled monitoring.

## Results

Contract pre-gate: Luna PASS, P0/P1/P2 = 0. Same-state application must be a true no-op, including preserving a fresh command queued after unlock. Astra approved implementation on 2026-09-08.

Terra delivered the controller-only change. Astra source review confirmed validation precedes queue mutation; same-state returns before clearing; no external directive state, motor algorithm, scene, prefab, assembly, package or project settings were changed by this unit. Scoped diff whitespace check passed.

Unity preflight passed: editor 6000.6.0f1, matching signed Hub/embedded Licensing Client 1.18.3, non-empty entitlement file, no competing project editor or Licensing Client processes. This is preflight evidence, not an executed gameplay test or independent proof of a successful license handshake.

Astra rejected the first test draft before execution: a missing cleanup helper would not compile; the buffered-jump assertion remained airborne and could pass without clearing the buffer; modifier preservation was not asserted; cadence omitted gate results and never retained a locked physics tick. Luna was asked to strengthen the fixture with a landing/no-lock control, concrete modifier assertions, integer cadence grouping, locked ticks and additional owner/replay negatives. Production behavior was not weakened to accommodate tests.

First focused execution `TestResults-Unity-PlayMode-20260908-164511.xml`: 6 passed / 1 failed. AC003's test captured baseline compatibility gravity immediately after the test-only modifier setter, which does not reflect that field until a tick. Root changed setup to the actual transfer modifier application endpoint, checking Lightweight `3.1315f * 0.65f` and subsequent Baseline `3.1315f`. Luna independently accepted that correction; no runtime change was required.

Re-execution `TestResults-Unity-PlayMode-20260908-164623.xml`: 7 passed / 0 failed / 0 skipped, Unity exit 0.

Final sequential Unity runs, with runtime/test writers frozen:

| Execution | Result | SHA-256 |
|---|---|---|
| Focused PlayMode `TestResults-Unity-PlayMode-20260908-164623.xml` | 7/7, skipped 0, exit 0 | `748835F1ACDC4F08AD7CB5C39D0FFA46948FF0EE8EE3A173689C282E578E9D7C` |
| Full EditMode `TestResults-Unity-EditMode-20260908-164658.xml` | 440/440, skipped 0, exit 0 | `3ADB9BC4228A69C3F83A68E5ABFE4185724A444EE98F04D1A2744DC078246649` |
| Full PlayMode `TestResults-Unity-PlayMode-20260908-164737.xml` | 374/374, skipped 0, exit 0 | `58D9C3590320892049A6FBC643AC591D6F562C42ED6542D4E070CAE188CF9EF3` |

814 whole-suite tests passed (the focused 7 are included in the 374, not additional distinct tests). No standalone player build, manual device play or full-game completion is claimed.

## AC mapping

- AC-M5B1-001: `LockDropsExactAndDistantCommandsAndRejectsLockedSubmission` verifies exact/future drop count, locked rejection and no old dash replay; buffered jump separately covered by AC005.
- AC-M5B1-002: `GateOwnerAndTickValidationAreMutationFreeAndIdempotent` verifies null/unregistered/different owner, invalid tick, same-owner registration, repeated lock and test-setter bypass rejection, retaining queued count until a valid lock.
- AC-M5B1-003: `LockedTicksRemainMonotonicAndKeepPlayerActiveAndContinuous` verifies InputLocked ticks, no teleport/deactivation, actual Lightweight preservation and Baseline restoration through the transfer modifier endpoint.
- AC-M5B1-004: `UnlockAcceptsFreshCommandOnceAndNeverReplaysOldFutureCommand` verifies duplicate submission rejection, repeated unlock preserves the fresh command, movement occurs and old future dash never returns.
- AC-M5B1-005: `LockPulseClearsBufferedJumpAndActiveDash` uses an unpulsed control that actually jumps after ground appears; the pulsed case does not. An active dash is cancelled by the pulse. `LockPreservesExternalMovementDirectiveQueueAndReceipt` verifies the separately owned directive is still consumed with its exact identity.
- AC-M5B1-006: `RenderGroupingAt30_60_And144ProducesIdenticalGateTrace` drives integer fixed-step accumulation with actual locked tick 1 and unlock tick 2. Motion, tick, action, dropped count and repeated-unlock result traces match exactly; diagnostic `firstDivergence=none`. Full suites above cover regressions.

Luna independently re-read all three final XML files and hashes, confirmed AC001–006 PASS and P0/P1/P2 = 0. Astra accepts this bounded unit as Verified on 2026-09-08. Existing uncommitted changes and unrelated media were preserved; no commit, push or automatic retry was performed.
