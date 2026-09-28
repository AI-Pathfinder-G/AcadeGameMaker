# VD-09 M5D7P-A UI semantic frame seam — Luna independent pre-gate

- Review date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`), independent contract reviewer
- Contract: [M5D7P-A UI semantic frame seam](../specs/work-contracts/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md)
- Compared against: `AGENTS.md`, ADR-0032, `docs/agent-operating-model.md`, Verified M5B5/M5D7N/M5D7O contracts and evidence, `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`, `InputRouter`, `InputRouterDiagnostic`, `GameInput.inputactions`, generated `GameInputActions.cs`, and existing M5B5 InputRouter PlayMode tests.
- Review type: read-only contract/API/topology/traceability review. No tests were run and no implementation or contract file was modified.

## Initial pre-amendment verdict (superseded below)

**CONDITIONAL — P0=0, P1=2, P2=2. Do not advance M5D7P-A to Approved until P1-001 and P1-002 are amended and independently re-read.**

The design fits the verified ownership model: one existing `InputRouter` remains the sole generated-wrapper subscriber and map owner; UI facts stay buffered until the same successful global receipt; the consumer uses fixed-step, identity-bound receipt traversal; no UI package, scene, second subscriber, or notification authority is introduced. M5B5 already establishes the critical transaction order and old-receipt/pending-batch preservation semantics. The first-frame baseline is compatible with launch readiness when presentation controls remain non-interactive until `Ready`.

## P0/P1 findings

### P1-001 — non-UI publication has no specified independent CurrentReceipt proof

- **Affected IDs:** `REQ-M5D7PA-006`, `AC-M5D7PA-007`; inherited `REQ-UX-004`/`REQ-UX-006` receipt-authority boundary.
- The contract requires reflection corruption of router publication state to fail at a source getter or commit boundary. It defines a UI publication as a matching `CurrentUiFrame.Receipt`/`CurrentReceipt` pair, but defines a healthy non-UI publication only as `CurrentUiFrame == null`. In that state, changing `_currentReceipt` through reflection to a different well-formed receipt with the same non-UI mode has no specified comparison value. `InputFrameCommitReceipt` is immutable at its API, but its backing fields can still be altered reflectively; it has no `Validate()` proof of its own.
- A non-UI source getter therefore could return a forged tick/ordinal/epoch while satisfying the stated “frame absent” relation. That weakens the same global proof used by the M5B5 consumers.
- **Required amendment:** require an independent retained exact receipt proof (or an equivalently closed invariant that detects any altered receipt field) in every mode. Define that it is updated before `CurrentReceipt` in the non-throwing publication block, with `CurrentReceipt` remaining the last authoritative assignment. Add an AC-M5D7PA-007 case that replaces a non-UI current receipt with a different but well-formed same-mode receipt and requires the next receipt getter/commit to throw, latch `UiCapture`, and preserve the prior publication.

### P1-002 — expected Click control fault handling conflicts with ignored wrong-device callbacks

- **Affected IDs:** `REQ-M5D7PA-002`, `REQ-M5D7PA-006`; `AC-M5D7PA-002`, `AC-M5D7PA-007`.
- The contract says wrong action/map/device callbacks make no semantic change, while also requiring a malformed expected action or bound click control to fault. It does not define whether an invocation from the exact `UI.Click` action and UI map whose bound control is not the approved mouse-left control is an ignored wrong-device callback or a malformed binding that must latch `UiCapture`. If that case is silently ignored, malformed production bindings are indistinguishable from irrelevant callbacks; if accepted, the required same-device point may not be available.
- The checked-in action asset currently resolves `UI.Click` only to `<Mouse>/leftButton` and `UI.Point` to `<Mouse>/position`, so the intended runtime path is feasible. `CallbackContext.control.device` gives the exact device that triggered the accepted Click, whose `Mouse.position` can be read during that callback; neither `Mouse.current` nor a second subscriber is needed. The ambiguity concerns malformed/runtime-overridden bindings and the required failure behavior.
- **Required amendment:** distinguish foreign callbacks (wrong action/map, or otherwise not the accepted expected callback) from an expected `UI.Click` callback with a malformed control/device. Specify the exact accepted click control (`Mouse.leftButton`) and that its position is read from that same `Mouse` instance in that callback. Specify that an expected Click action with an incompatible bound control faults before any pending-field mutation. Add tests for two Mouse devices (proving a later/global current pointer cannot replace the click device's point), later Point movement, and the malformed expected Click control.

## Acceptance-criterion review

| Criterion | Review result | Basis / remaining condition |
|---|---|---|
| AC-M5D7PA-001 | **PASS by contract** | Processed Navigate performed/canceled rules, clamping, Q4096 rounding, and sticky same-epoch change semantics are explicit and compatible with the existing Value action/composites. Tests must cover multiple callbacks that return to the frame-start value. |
| AC-M5D7PA-002 | **BLOCKED — P1-002** | Signed unclamped Point and same-device click-time point are defined; the exact malformed expected Click control versus ignorable wrong-device callback case is not. |
| AC-M5D7PA-003 | **PASS by contract** | Scroll checked aggregation, opposing deltas, and independent Submit/Cancel edge coalescing are closed. Performed-then-canceled must preserve each edge. |
| AC-M5D7PA-004 | **PASS by contract** | Non-UI capture suppression, empty UI-entry epoch, same-epoch current-value retention, successful-exit clearing, and `_unboundUi` suppression match M5B5 map/lifecycle rules. |
| AC-M5D7PA-005 | **PASS by contract** | Candidate preparation precedes consumer mutation; pending UI state and old publication are retained through precommit rejection and terminal partial-commit/map faults, consistent with M5B5. Tests must cover each existing prepare/validate/commit/map failure injection boundary and distinguish retryable precommit failures from terminal partial commits. |
| AC-M5D7PA-006 | **PASS with P2 clarification** | Exact source identity, duplicate, consecutive tick/ordinal, mode epoch, exit, skip/replay, and fault outcomes are defined. The first publication while `AwaitingBaseline` is specified only when it is UI; see P2-002 for an otherwise-unreachable non-UI-first publication. |
| AC-M5D7PA-007 | **BLOCKED — P1-001/P1-002** | Fail-closed intent is explicit, but the non-UI receipt proof and malformed expected Click-control behavior need the amendments above. The implementation test matrix should enumerate each frame/router/cursor proof field and verify a Failed latch. |
| AC-M5D7PA-008 | **PASS as planned gate** | Focused EditMode/PlayMode, direct M5B5/M5D7O regressions, and full EditMode/PlayMode are specified; execution evidence is naturally pending implementation. The existing EditMode and PlayMode assemblies already reference the test/runtime assemblies needed, and `InputTestFixture` supports real generated-wrapper device tests. |

## Focused cross-checks

### Baseline and launch readiness

The deliberate first-frame discard does not conflict with M5D7N/M5D7O readiness as written. M5D7N accepts only the exact live, non-faulted `UIOnly` router and M5D7O owns menu intent separately. If a coherent UI pair exists at cursor creation, that pair is stored as the baseline and the cursor is immediately `Ready`; if there is no receipt yet, the first coherent UI frame is the baseline and is not delivered. The explicit requirement to keep controls non-interactive until `Ready` prevents a pre-baseline click from activating menu intent. This intentionally adds at most the wait for the first UI publication when no current pair exists.

### Click-time device access and asset/wrapper boundary

`Assets/GameInput.inputactions` defines a single `UI.Click` binding at `<Mouse>/leftButton` and a `UI.Point` binding at `<Mouse>/position`; the generated wrapper forwards the exact action's `CallbackContext` to `InputRouter.OnClick`. Reading `Mouse.position` through `context.control.device` on that accepted callback is technically available and preserves same-device identity. The input asset and generated wrapper hashes match the Verified M5B5 evidence (`8E23A5A3AE1D514596A1FC8C61017F2740664904741D5A95383074A06E840E76` and `730BE8CAD941CD4567BBEA9B844FE33673FEA49842E28CB53C2F088D3F528C89`); both remain correctly excluded from the allowlist.

### Atomic receipt publication and failure retention

M5B5 stages the exact receipt, computes checked successors before mutation, prepares and validates all three sink candidates, commits them in order, switches maps, consumes frozen input only after success, and publishes `CurrentReceipt` last. Its verified behavior preserves the prior global receipt and unconsumed batch on ordinary precommit failure, and faults after partial local commit or map failure without claiming rollback. M5D7P-A's candidate-before-mutation and UI-frame-before-final-receipt sequence can extend that same boundary without changing M5B5 ownership. The new frame, proof, and pending UI fields must all be prepared in locals before any publication or pending clear.

### Cursor, mode exit, and fault latching

The `-210` router order and a strictly later fixed-step consumer avoid render-frame edge loss. Exact source reference plus checked `tick + 1`, `frameOrdinal + 1`, unchanged UI epoch for delivery, and a single epoch increment/no-frame receipt for clean exit are consistent with M5B5's monotonic receipt rules. A faulted source cannot authorize an old frame because the cursor checks `IsFaulted` before delivery. Capture faults must use the appended `UiCapture` diagnostic stage, preserve old publication and already-valid pending facts, and never expose a retry/reset path.

### M5D7O typed-notification P2 transfer

The M5D7O residual requires the authored owner to retain the controller-owned typed notification and receipt correlation; it must not reconstruct identity or prose from `NotificationKind`. M5D7P-A keeps notification/controller/menu types outside this seam and states that Click/Submit are input facts only, so this boundary is preserved. The downstream M5D7P contract and tests still need to prove that one lifecycle owner carries the transferred payload and that a visible warning is not lost on a scene transition.

## P2 notes

- **P2-001 — downstream M5D7P ownership:** the M5D7O typed notification/correlation warning is correctly carried into `REQ-M5D7PA-008`; verify concrete payload retention and visible-warning continuity in the authored-presentation contract.
- **P2-002 — first non-UI publication while awaiting baseline:** `Create` closes if a current non-UI receipt exists, but `AwaitingBaseline` describes only no-publication and first-UI-publication outcomes. Specify whether an initial non-UI receipt after cursor creation closes the cursor or is ignored. M5D7N's `UIOnly` precondition makes this path unexpected for the Hub owner, so it is not an approval blocker.

## Original required action (superseded below)

Amend the M5D7P-A contract for P1-001 and P1-002, then request a narrow Luna re-review. **Do not mark this contract Approved until both are closed.** No new user product decision is required. The proposed allowlist is otherwise bounded and matches the existing Unity assemblies; implementation must remain limited to its listed router/diagnostic additions, new semantic frame/cursor and focused tests, and documentation entries.

## Amendment re-review — 2026-09-20

- Reviewer: Luna (`gpt-5.6-luna`), independent narrow contract re-review.
- Scope: Astra's amendments to the receipt-proof invariant, exact expected Click control handling, and first non-UI baseline behavior; rechecked the amended normative clauses and AC-M5D7PA-002/-006/-007/-008 against the prior source/API/topology findings above.
- No source, implementation, contract, or test files were changed; no tests were run because this is a pre-implementation contract gate.

### Final verdict

**PASS — P0=0, P1=0, P2=1. Recommend Astra advance M5D7P-A to Approved.** The original P1-001 and P1-002 are closed, and P2-002 is closed by explicit cursor behavior and AC coverage. Approval remains Astra's authority; this report is a recommendation, not an approval-state mutation. The earlier conditional verdict and required-action section above are preserved as history and are superseded by this amendment re-review.

### Amendment findings

- **P1-001 — closed.** The amended invariant now requires three coherent publication states: before publication, frame/proof/receipt are all absent; for UI, frame, independent proof, and current receipt are present and exactly equal; for non-UI, the frame is absent while proof and current receipt are present and equal. The non-throwing publication block stages the frame, copies the exact receipt into the independent proof, and assigns `CurrentReceipt` last. Getter/commit validation latches `UiCapture` on corruption without normalizing forensic state. AC-M5D7PA-007 explicitly exercises replacing a non-UI receipt with a different, well-formed same-mode receipt and requires fail-closed behavior.
- **P1-002 — closed.** The amended text separates irrelevant callbacks (wrong action/map, disabled device, wrong mode, suppression, etc.) from a live callback identified as the exact expected `UI.Click` action/map. The latter must use that callback control's own device and exact `Mouse.leftButton`; an incompatible control or non-Mouse device faults before pending mutation. The point is read from that same Mouse during the callback. AC-M5D7PA-002 now requires two-Mouse coverage and verifies that an exact expected Click using any other control faults before mutation. This is consistent with the checked-in single `<Mouse>/leftButton` binding and avoids `Mouse.current` or later-point substitution.
- **P2-002 — closed.** `AwaitingBaseline` now has a deterministic first non-UI publication outcome: transition to `Closed`, return false/default, and deliver nothing. AC-M5D7PA-006 explicitly covers it. This matches the existing `Create` behavior for a non-UI current receipt and does not conflict with the downstream Hub's UI-only readiness precondition.
- **No new P0/P1 found.** The publication sequence has a single specified non-throwing assignment block with the authoritative receipt last; the proof/frame/receipt presence matrix is closed for UI, non-UI, and prepublication. The expected-click malformed-control fault is pre-mutation while unrelated callbacks remain ignorable. The added cases have direct AC traceability and stay within the existing router/cursor seam and allowlist. M5B5's preserved-publication and pending-batch failure boundary remains compatible with the amendments.

### Final AC disposition

| Criterion | Amendment re-review disposition |
|---|---|
| AC-M5D7PA-001 | PASS by contract; unchanged by amendments. |
| AC-M5D7PA-002 | PASS by contract; same-device callback point, two-Mouse proof, and malformed expected control fault are now explicit and testable. |
| AC-M5D7PA-003 | PASS by contract; unchanged by amendments. |
| AC-M5D7PA-004 | PASS by contract; unchanged by amendments. |
| AC-M5D7PA-005 | PASS by contract; receipt-last non-throwing publication extends the established M5B5 boundary. |
| AC-M5D7PA-006 | PASS by contract; first non-UI publication while awaiting baseline now explicitly closes without delivery. |
| AC-M5D7PA-007 | PASS by contract; exact independent proof covers every mode and the forged same-mode non-UI receipt case. |
| AC-M5D7PA-008 | PASS as a future execution gate; focused and full suites must still report zero failures, skips, and inconclusive results before implementation verification passes. |

### Residual P2 and implementation gate

- **P2-001 — downstream M5D7P typed-notification/correlation continuity.** M5D7P-A correctly excludes notification/controller ownership and preserves the M5D7O boundary through `REQ-M5D7PA-008`; the authored-presentation contract must still prove retention of the controller-owned typed payload/correlation and continuity of a visible warning across scene transitions. This is a downstream verification item, not an M5D7P-A approval blocker.
- Implementation remains gated on Astra recording `Approved`. Once approved, Terra's implementation tests and Luna's independent verification must demonstrate the AC-M5D7PA-008 zero failure/skip/inconclusive execution requirement; this pre-gate pass is not runtime evidence.

## Compile-allowlist amendment re-review — 2026-09-20

- Reviewer: Luna (`gpt-5.6-luna`), narrow independent amendment review.
- Scope: the single proposed `AcadeGameMaker.Core` direct reference in the existing Input.Unity EditMode test asmdef; checked the amended allowlist, that asmdef, the corresponding PlayMode asmdef, Core type ownership, the cited compile-preflight log, and the current EditMode test source.
- No contract, asmdef, or code was changed by this review. No new compile was run.

### Amendment disposition

The direct reference is a safe, test-only dependency and does not add production authority. `InputFrameCommitReceipt` and `IInputFrameCommitSource` are declared by `AcadeGameMaker.Core`; the EditMode test assembly already references `AcadeGameMaker.Input.Unity`, which in turn references Core, but Unity asmdef dependencies are not transitive for compile-time name resolution. The recorded preflight log (`artifacts/unity-results/m5d7pa-r01-focused-editmode.log`) shows CS0234/CS0012/CS0246 in `UiSemanticFrameV1Tests.cs` for the missing Core namespace/interface/receipt. For that logged source snapshot, the single direct reference is the minimal asmdef fix. The PlayMode test asmdef already has the same direct Core reference.

There is one current-tree qualification: the present `UiSemanticFrameV1Tests.cs` uses reflection/`Assembly.Load` to obtain Core types and has no compile-time `using AcadeGameMaker.Core` or typed `InputFrameCommitReceipt`/`IInputFrameCommitSource` usage. That is different from the source snapshot recorded in the failed compile log. Thus the amendment is minimal and necessary for the logged typed-test version, but is not technically necessary for the current reflection-based file as it stands. Keep the one-reference allowance only if Terra is retaining/restoring typed test usage; otherwise it is an unnecessary test-assembly edge. Either choice remains test-only and does not warrant a production dependency.

### Final disposition and P0/P1/P2

**Amendment design: acceptable; P0=0, P1=1, P2=3 overall (two amendment-specific P2 notes plus the carried P2-001). The recorded approval decision may stand for the bounded test-only dependency, but implementation must remain paused until the contract's authoritative status is reconciled.**

- **P1-003 — approval-state field conflicts with approval record.** The contract front matter still says `Status: Review`, while its final review record says “Astra approved” and “Terra may implement.” The project rule permits implementation only when the spec status itself is `Approved`. This blocks relying on the approval in the current document state; Astra must reconcile the status field and approval record before implementation resumes. This is a document authorization inconsistency, not a problem with the Core reference itself.
- **P2-003 — generic asmdef exclusion should state the exact exception.** The allowlist now names precisely `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef` and restricts the edit to one direct Core reference, but the following exclusion still says broadly that “asmdefs” are outside the allowlist. The specific entry can reasonably be read as an exception, and the addition is bounded; tighten the exclusion to “all other asmdefs” at the next contract edit to avoid interpretation drift.
- **P2-004 — dependency necessity depends on test-source version.** The failed preflight proves the reference is needed for the typed test snapshot, but current test source uses reflection instead. The implementer should keep source and allowlist intent aligned; no further reference or assembly change is authorized by this amendment.
- Existing **P2-001** (downstream M5D7P typed-notification/correlation continuity) remains carried forward and is not changed by this amendment.

The only permitted dependency edit, if the typed test source is used, remains the one `AcadeGameMaker.Core` entry in the named EditMode asmdef. No runtime asmdef, production source, package, action asset, generated wrapper, or other test assembly is included. This re-review does not authorize or perform that edit.

## Correction — compile-allowlist amendment, fresh-disk re-review — 2026-09-20

The preceding compile-amendment disposition was based on a stale contract snapshot. I re-read the contract from disk and correct the resulting findings here; this section supersedes P1-003 and the open P2-003/P2-004 conclusions above.

- The current contract front matter is `Status: Approved`, and the approval record is present immediately below it. **P1-003 does not exist in the current contract.** Its earlier claim that the status remained `Review` was incorrect and is withdrawn.
- The allowlist now says `all other asmdefs` are excluded, leaving the one explicitly named EditMode asmdef as the sole permitted exception. **P2-003 is closed.** The reference remains a single direct `AcadeGameMaker.Core` test-assembly dependency; it adds no production assembly dependency, runtime authority, or action/map ownership.
- The review record now explicitly constrains the dependency to strongly typed receipt/interface tests and says the temporary reflection-only workaround is not acceptable final source. **P2-004 is closed at the contract/amendment level.** The current `UiSemanticFrameV1Tests.cs` on disk still reflects Core types, so implementation must restore/retain the strongly typed tests and apply the exact allowlisted reference before that code is eligible for final verification. That implementation alignment is outstanding work, not a defect in this reviewed amendment or an additional P2 finding.
- The failed compile log still evidences CS0234/CS0012/CS0246 for the typed test snapshot, so the one direct reference is the minimal fix for the accepted final test design. The existing PlayMode test asmdef already directly references Core.

### Corrected final disposition

**PASS — P0=0, P1=0, P2=1 overall. Maintain `Approved` and allow implementation to resume within the exact allowlist.** The only carried P2 is P2-001, the mandatory downstream M5D7P typed-notification/receipt-correlation continuity gate. Terra must not treat the present reflection-only temporary test file as final; restore/retain strongly typed tests with the one approved EditMode Core reference, then complete the specified test runs. This review changes no contract, asmdef, or code and runs no compile/tests.

## Full-PlayMode R03 test-fixture amendment re-review — 2026-09-20

- Reviewer: Luna (`gpt-5.6-luna`), narrow independent test-allowlist re-review.
- Evidence inspected: current Approved contract and exact two test-file allowlist entries; R03 XML; `InputRouter.ValidateUiPublicationOrFault`; the five `ActualRouterSourceMismatchCannotAuthorizeAnyPendingConsumer` variants; the terminal overflow evidence-rewrite helper and assertions; the focused semantic-frame tests.
- No contract, source, tests, or asmdefs were changed by this review. No test was rerun.

### R03 failure disposition

`artifacts/unity-results/m5d7p-b-r03-full-playmode.xml` reports **805 total, 799 passed, 6 failed, 0 skipped, 0 inconclusive**. All six failures map exactly to the two amendment-scoped legacy fixtures:

- Five parameterized receipt-mismatch cases fail only when the old fixture restores `_currentReceipt` and expects the next `AdvanceFrame` to recover. Under the now-required router invariant, changing `_currentReceipt` alone disagrees with `_uiPublicationProof`; the next getter/consumer boundary latches monotonic `UiCapture`. Restoring one reflected field must not clear that latch. Updating these cases to assert `UiCapture`, retain the existing sink immutability/pending-receipt assertions, and prove no recovery is therefore a contract correction, not a weakened regression. Their well-formed same-mode single-field receipt variants remain the AC-M5D7PA-007 forged-receipt coverage; no fixture may update the proof in these corruption cases.
- The terminal overflow fixture deliberately synthesizes a valid non-UI `CurrentReceipt` at `int.MaxValue` so a downstream checked successor overflows. Setting only `_currentReceipt` now fails earlier at the publication proof boundary, before the requester's overflow path runs. Setting `_uiPublicationProof` to the identical receipt in this one intentionally coherent fixture preserves the router's non-UI invariant and keeps the downstream overflow assertions unchanged. This does not weaken the separate single-field corruption tests.

### Amendment assessment

The two allowlist additions are exact and minimal: (1) the five corrupt-receipt cases may change only their terminal-fault/no-recovery expectation while still proving all sinks remain untouched, and (2) the terminal overflow helper may update the proof alongside its deliberately rewritten receipt, leaving product/runtime code and overflow assertions unchanged. The distinction between a one-field corruption test and a deliberately coherent boundary-value fixture is explicit and consistent with the independent-proof model. No product scope, production authority, or runtime permission expands. The R03 failures do not indicate a package or runtime regression; they expose stale fixture assumptions. However, AC-M5D7PA-008 remains unverified until a subsequent required run reports zero failures, skips, and inconclusive tests.

### Final disposition

**PASS — P0=0, P1=0, P2=1 overall. Maintain `Approved`; implementation may continue with only these two test-fixture changes.** The sole carried P2 remains downstream typed-notification/receipt-correlation continuity. Do not mark the implementation `Verified` or AC-M5D7PA-008 passed from R03; rerun the required focused and full suites after the changes and require zero failure/skip/inconclusive results. This is a contract/allowlist review only, not acceptance of the failing R03 execution.

At this re-review's fresh-disk check, `UiSemanticFrameV1Tests.cs` is now strongly typed with `using AcadeGameMaker.Core`, and the named EditMode test asmdef contains the single direct Core reference. This supersedes the earlier temporary-reflection-state note under the compile-allowlist correction; that prior snapshot is retained only as review history.
