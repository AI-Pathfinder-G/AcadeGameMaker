# VD-09 M5D7O hub-menu presentation controller — Luna independent contract pre-gate

- Date: 2026-09-14
- Reviewer: Luna (independent)
- Contract: [M5D7O hub-menu presentation controller](../specs/work-contracts/2026-09-14-vd09-m5d7o-hub-menu-presentation-controller.md)
- Compared against: Verified [M5D7N hub-entry handoff latch](../specs/work-contracts/2026-09-13-vd09-m5d7n-hub-entry-handoff-latch.md), approved VD-09 platform quality, approved input/UI feedback, approved 640x360 aspect-frame and UI-scale decisions, `SYSTEM-CONTRACTS.md`, AGENTS.md, and current Input/Profile runtime APIs.
- Scope: read-only contract/API/topology review. No runtime, test, contract, package, scene, prefab, or project-setting file was modified; no Unity run was performed.

## Verdict

**CONDITIONAL — P0=0, P1=2. Do not advance M5D7O to Approved until the two P1 findings are amended and re-reviewed.**

The proposed core boundary is otherwise sound: it keeps scene/uGUI/TMP/input-action ownership in the later authored presentation contract, consumes only the already-transferred M5D7N value, treats Continue as profile continuation rather than active-expedition resume, and keeps notification dismissal and menu intent one-shot and independent. The remaining blockers are contract testability/closure, not a need to widen the implementation boundary.

## P0/P1 findings

### P1-001 — top-right notification anchor is normative but absent from the view API

- **Affected IDs:** `REQ-M5D7O-004`, `AC-M5D7O-004`, `AC-M5D7O-007`; approved UI constraints in `REQ-UX-014` / `AC-UX-013`.
- The contract states that the immutable view carries a **top-right notification anchor**, but `HubMenuViewV1` exposes only `NotificationKind` and `NotificationState`; it has no anchor enum, logical rect, or other immutable anchor token. `AC-M5D7O-004` checks kind/correlation and state only, so a later implementation can satisfy every listed AC while presenting the warning anywhere.
- This is a real ownership seam: M5D7O must not render, but M5D7P needs a testable semantic placement contract. “Top-right policy” in prose is not sufficient for an immutable value or static verification.
- **Required amendment:** add a closed `HubNotificationAnchorV1` (minimum value `TopRight`) or an exact approved logical anchor/rect to `HubMenuViewV1`; require its value in `REQ-M5D7O-004` and assert it in `AC-M5D7O-004`/focused tests. Keep actual Canvas/TMP layout in M5D7P.

### P1-002 — `GetMenuItem(int index)` has no defined index base, range, or invalid-index behavior

- **Affected IDs:** `REQ-M5D7O-003`, `AC-M5D7O-002`, `AC-M5D7O-007`.
- The API uses an integer index while the enum values are `Continue=1` through `Quit=4`, but the contract never says whether valid indices are `0..3` or `1..4`, nor what an out-of-range/illegal index must do. Consequently, “exact fixed order” is not independently reproducible: two implementations can both claim compliance while returning different results for `GetMenuItem(0)` and `GetMenuItem(4)`.
- `ContinueInteractable` is also the only per-item availability value. The prose implies the other three entries are interactable, but this is not an explicit invariant/AC and cannot distinguish a disabled Settings/Quit projection from the intended view.
- **Required amendment:** define one valid index convention and closed invalid-index behavior (fail/throw versus failed state), and state that New Game, Settings, and Quit are interactable in `Ready` while Continue follows `HasPersistedValidProfile`; add the invalid-index and availability assertions to `AC-M5D7O-002`/focused tests.

## Dependency, API, and boundary review

- **M5D7N receipt/notification correlation:** feasible. `HubEntryHandoffReceiptV1.Validate()` and `HubEntryHandoffReceiptV1.SameReceipt(...)` provide the receipt proof boundary; `ProfileLaunchNotificationV1.FromReceipt(...)` derives the expected typed kind. M5D7O can validate the optional value once and keep no live adapter/router reference. It must not call the M5D7N take a second time, consistent with `REQ-M5D7N-005` and `AC-M5D7N-003`.
- **Valid persisted-profile predicate:** the Primary/Previous-only rule is consistent with `ProfileLaunchSource` and `REQ-M5D7O-002`. Default remains New Game even when default bootstrap is successfully saved, while a Primary/Previous source remains a confirmed profile even when the launch receipt records pending/uncertain persistence. This matches the M5D7N safe-handoff rule and `REQ-PLAT-006`/`AC-PLAT-004` (active expedition state is never resumed).
- **Manual dismissal:** Click/Submit-only dismissal and the absence of timer, navigation, cancel, lifecycle, or intent-take dismissal paths are closed and compatible with `REQ-UX-009`, `REQ-M5D7O-005`, and the approved UI feedback contract. M5D7P must map only the adopted UI semantic actions and must retain a visible warning until one of those two sources is received.
- **Intent one-shot:** `Ready → IntentPending → IntentConsumed` is sufficient for one exact immutable intent; disabled Continue must remain a no-op. The contract should make the invalid-menu-enum result follow the same fail-closed policy as invalid dismissal enums, but this is a clarification rather than a new authority.
- **Reflection/mutation AC feasibility:** the view and intent are value types containing closed primitives; notification/handoff values can be revalidated at the controller boundary. The requested reflection matrix is implementable if Terra retains private proof copies and validates before every getter/command. It must not rely on `ValueType.Equals` alone for a future deep mutable field.
- **EditMode fixture construction:** no test-only production factory is required. A friend test assembly can call the existing internal `ProfileLaunchPreparationCoordinatorV1.Prepare(...)`, construct a valid `DesktopProfileLaunchReceiptV1` through the existing prepared-launch path, call `WithHubProof()`, construct `HubEntryHandoffReceiptV1`, and derive notification with `ProfileLaunchNotificationV1.FromReceipt(...)`. Failed-save, preservation-denied, and commit-uncertain cases require injected profile/actions ports already supported by the preparation coordinator. The new EditMode asmdef must explicitly reference the assemblies it uses (`AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Profile`, and the Unity InputSystem/test assemblies as applicable); no runtime factory or package change belongs in M5D7O.
- **uGUI/TMP/package boundary:** current manifest has no direct `com.unity.ugui` or `com.unity.textmeshpro` dependency. This is acceptable for M5D7O because the controller is engine-free and the contract forbids package/UI changes. M5D7P remains blocked until compatible package versions and the UI semantic-input seam are separately Approved, as its stop conditions state.
- **Input ownership:** `SYSTEM-CONTRACTS.md` defines UI `Navigate`, `Point`, `Click`, `ScrollWheel`, `Submit`, and `Cancel`, mutually exclusive from Gameplay, with no virtual mouse. M5D7O correctly does not enable maps, own `InputSystemUIInputModule`, or mutate `InputRouter`; M5D7P must use the existing M5B5 graph and the adopted semantic seam.

## Residual P2 notes

- **P2-001 — fixture topology is not specified in the allowlist:** valid receipts are constructible through existing internal coordinator seams, but the contract should name the required EditMode asmdef references and an instance-scoped injected-port fixture so implementation does not add a production factory.
- **P2-002 — invalid command enum outcomes need one shared rule:** `TryDismissNotification` explicitly mentions invalid enum testing, while `TryActivate` does not. Define whether either invalid enum throws/latches `Failed` or returns false without mutation; do not leave this to implementation choice.
- **P2-003 — view projection does not expose a notification correlation token:** creation-time validation is adequate for this bounded core, but M5D7P must not infer prose or receipt identity from `NotificationKind`; it should retain the controller-owned notification state and use the exact typed dismissal seam only.

## Pre-gate AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7O-001 | **PASS with P2 fixture note** | Source predicate is closed over Primary/Previous/Default and does not consult save outcome; valid receipt scenarios are constructible via existing internal coordinator seams. |
| AC-M5D7O-002 | **BLOCKED by P1-002** | Menu values are closed, but index base/range and non-Continue availability are underspecified. |
| AC-M5D7O-003 | **PASS by contract** | One exact intent and one take are defined; invalid-enum behavior should be aligned as P2-002. |
| AC-M5D7O-004 | **BLOCKED by P1-001** | Correlation/kind is closed, but the required top-right anchor has no immutable API value or assertion. |
| AC-M5D7O-005 | **PASS with clarification** | Click/Submit-only manual dismissal is explicit; invalid-enum outcome should share the fail-closed rule. |
| AC-M5D7O-006 | **PASS by contract; implementation pending** | Private proof validation can cover value/state/correlation mutation without Unity dependencies. |
| AC-M5D7O-007 | **PASS by boundary** | Allowlist and no-authority clauses exclude Unity UI/scene, package, IO, profile mutation, input-map, run, timer, network, RNG, logging, and Quit calls. |
| AC-M5D7O-008 | **BLOCKED — implementation gate** | No implementation or execution evidence exists; focused/full EditMode and PlayMode plus independent post-review remain required. |

## Final recommendation

**CONDITIONAL — P0=0, P1=2, P2=3.** Amend P1-001 and P1-002, then request a narrow re-review. Once both are closed, the remaining M5D7O boundary is suitable for Astra approval; no user product decision is required. M5D7P should remain a separate contract and must not be pulled into this core implementation.

## Amendment review — 2026-09-14

The contract was re-read after the requested amendments. The parent traceability row now correctly names the UI contracts `REQ-UX-009` and `REQ-UX-014` (alongside the applicable platform requirements).

- **P1-001 closed:** `HubNotificationAnchorV1.TopRight` is now a closed enum and `HubMenuViewV1.NotificationAnchor` exposes it as immutable view data. The projection states that it remains `TopRight` even when a notification is absent, and `AC-M5D7O-004` requires the exact value. This closes the semantic M5D7O→M5D7P placement seam without adding rendering authority.
- **P1-002 closed:** `GetMenuItem` is explicitly zero-based (`0..3`) with `ArgumentOutOfRangeException` for negative/4-or-greater indices and no controller mutation. `IsInteractable` defines Continue from profile validity and New Game/Settings/Quit as interactable in `Ready`; unknown items throw and latch policy is explicit. `REQ-M5D7O-003` and `AC-M5D7O-002` now state the same rule.
- **P2-001 closed:** the allowlist now requires direct EditMode references to `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Profile`, `AcadeGameMaker.Input`, `Unity.InputSystem`, and used test assemblies; the fixture must use the existing instance-scoped injected-port preparation path, with no production factory or static environment. This is sufficient to construct valid receipts and all persistence-outcome variants through existing internal seams.
- **P2-002 closed:** unknown activation and dismissal enums both latch `Failed` and throw `ArgumentOutOfRangeException`; invalid index handling is also explicit and non-mutating. The command boundary is now consistent and testable.

No new P0/P1 issue was introduced. The remaining correlation caution is non-blocking: M5D7O validates the notification against the handoff at creation, while M5D7P must retain the controller-owned typed value and must not infer receipt identity or prose from `NotificationKind` alone.

### Amended pre-gate status

| AC | Independent status | Amendment basis |
|---|---|---|
| AC-M5D7O-001 | **PASS** | Primary/Previous/Default predicate and injected receipt construction remain closed. |
| AC-M5D7O-002 | **PASS** | Zero-based indexing, exact order, invalid range, and availability are now exact. |
| AC-M5D7O-003 | **PASS** | One-shot activation/take remains unchanged. |
| AC-M5D7O-004 | **PASS** | Exact notification correlation and immutable `TopRight` anchor are now asserted. |
| AC-M5D7O-005 | **PASS** | Click/Submit-only dismissal and invalid enum failure are explicit. |
| AC-M5D7O-006 | **PASS by contract; implementation pending** | Reflection-proof requirement remains implementable with private proof copies. |
| AC-M5D7O-007 | **PASS** | Scope and no-authority boundary remain unchanged. |
| AC-M5D7O-008 | **BLOCKED — implementation gate** | Execution evidence is correctly deferred until Terra implementation and Luna post-review. |

## Final amended recommendation

**PASS — P0=0, P1=0, residual P2=1.** The two blocking findings and the requested fixture/enum/traceability clarifications are closed. Astra may advance M5D7O to `Approved`; this recommendation covers only the engine-free controller contract and does not approve M5D7P authored scene, uGUI/TMP package, or input-module work.
