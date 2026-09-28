# VD-09 M5D7N hub-entry handoff latch

- Status: Verified
- Owner: Astra
- Architecture counter-review: Sol
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7M Verified
- Parent requirements: `REQ-PLAT-009`, `REQ-PLAT-010`, `REQ-PLAT-011`
- Proposed acceptance IDs: `AC-M5D7N-001` through `AC-M5D7N-009`
- Astra approval: Approved on 2026-09-13 after dependency M5D7M reached Verified and Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=1`). The residual P2 treats pre-lifecycle authored cohort membership as a declared supported-topology precondition rather than claiming historical runtime provenance for late-added components.
- Astra integration: Verified on 2026-09-14 after focused M5D7N `69/69`, direct M5D7M `117/117`, full EditMode `669/669`, and full PlayMode `803/803` passed with failed/skipped/inconclusive `0`; Luna final post-review reported `P0=0`, `P1=0`, and the inherited non-blocking topology-scope `P2=1`.

## Purpose and boundary

`HubEntryHandoffLatchV1` consumes the published M5D7M launch receipt at the first `Update` after its lifecycle `Start`, verifies the exact bound `InputRouter` remains active, non-faulted, and in `UIOnly`, and publishes one immutable handoff for a later authored hub UI owner. It may take M5D7M's optional typed launch notification once and re-expose that payload once.

This unit does not find, load, create, or activate scenes or UI; choose menu order, initial focus, notification placement, duration, dismissal, localization, TMP, or EventSystem; modify packages, prefabs, art, profile, persistence, router mode/maps, run, gameplay, or narrative state; or retry any source operation. Persistence/preservation pending is a valid handoff, not a launch rejection.

## API and topology

```csharp
[DefaultExecutionOrder(-190)]
[DisallowMultipleComponent]
internal sealed class HubEntryHandoffLatchV1 : MonoBehaviour
{
    [SerializeField] private DesktopProfileLaunchAdapterV1 _launchAdapter;
    [SerializeField] private InputRouter _router;

    internal HubEntryHandoffReceiptV1? CurrentHandoff { get; }
    internal bool TryTakeLaunchNotification(out ProfileLaunchNotificationV1 notification);
}
```

The latch and adapter must be enabled on the same active `GameObject` before that lifecycle begins. The serialized router must be reference-equal to `launchAdapter.BoundRouter` and must remain enabled on an active object. No registry, scene search, static service locator, or execution-order correctness dependency is allowed. The already-published M5D7M receipt proves completion of any separately hosted router initialization.

`HubEntryHandoffReceiptV1` is immutable and fail-validating. It contains the exact `DesktopProfileLaunchReceiptV1`, snapshot `InputMode.UIOnly`, `GameplayEnabled=false`, `UiEnabled=true`, and `HubEntryAccepted=true`. Every getter revalidates the entire closed matrix and receipt correlation. It contains no live router, path, clock, exception, callback, Unity object, scene, or prose.

## Lifecycle and state

Closed states are `Pristine`, `AwaitingStart`, `AwaitingFirstUpdate`, `PublishedNoNotice`, `PublishedNoticePending`, `PublishedNoticeConsumed`, `Failed`, and `ClosedBeforePublication`.

1. `Awake` enters once from `Pristine`, validates non-null, enabled, active, same-object adapter topology and exact router identity, and commits `AwaitingStart`. It does not read a receipt or notification.
2. `Start` enters once only from `AwaitingStart` and commits `AwaitingFirstUpdate`. It does not touch the source. Because the latch and adapter are enabled on the same active object before lifecycle entry, Unity invokes both `Start` callbacks before the latch's first `Update`; no relative `Start` order or execution-order attribute is used for correctness.
3. The first `Update` accepts only `AwaitingFirstUpdate` and attempts exactly once. It obtains and validates `CurrentReceipt`; confirms the adapter's bound router is the exact serialized router; and confirms that router remains enabled, active, non-faulted, and `EffectiveMode == UIOnly`. `GameplayEnabled=false` and `UiEnabled=true` in the handoff are the exact immutable M5D7M publication snapshot, not a second live action-map read. M5D7N does not expose or inspect the adopted actions collection.
4. Before consuming the source notification, it derives the exact expected nullable notification from the validated receipt using `ProfileLaunchNotificationV1.FromReceipt`. It calls `TryTakeNotification` exactly once. Expected-none must pair with `false/default`; expected-present must pair with `true` and an exactly correlated payload of the derived kind. A missing expected notification, unexpected notification, kind mismatch, or receipt-correlation mismatch fails terminally without publishing a handoff. Any payload successfully returned by that one authorized take is thereafter latch-owned even if a subsequent local validation fails; the latch never retries, restores, or reconsumes source state.
5. It constructs and validates the handoff and optional local notification entirely before publication. It stores the optional notification first and assigns `CurrentHandoff` last by a non-throwing assignment. No fallible operation follows publication.
6. Later `Update` calls do nothing. A notification is returned once; later calls return false and default without changing handoff, adapter, router, profile, or persistence.

Publication implies first-update completion, no local fault, exact adapter/router identity, a valid source receipt, and one exact notification state: absent, pending, or consumed. `Failed` and `ClosedBeforePublication` are terminal and cannot retry after reactivation. Disable/destroy before publication consumes no source notification. Disable/destroy after publication does not retract the handoff or alter M5D7M/router state.

Receipt absence/corruption, foreign or faulted/non-UI-only router, invalid topology/lifecycle, unexpected/missing notification, or invalid notification fails closed without a handoff. The latch owns only its lifecycle, immutable handoff, and any successfully returned local notification. Apart from the single authorized notification take, it never closes or mutates M5D7M, the router, actions, maps, profile, or persistence. The original protocol/programmer exception propagates and is not replaced by cleanup.

## Requirements

- **REQ-M5D7N-001:** accept only a same-GameObject M5D7M adapter and the exact router bound by that adapter.
- **REQ-M5D7N-002:** attempt source receipt acceptance once at the first `Update` after `Start`, independently of component `Awake`/`Start` ordering.
- **REQ-M5D7N-003:** publish only when the receipt is valid, its immutable publication snapshot states gameplay disabled/UI enabled, and the exact live router is active, non-faulted, and `UIOnly`.
- **REQ-M5D7N-004:** publish one immutable fail-validating handoff only after all fallible validation, using the handoff assignment as the final operation.
- **REQ-M5D7N-005:** derive the expected optional notification from the validated receipt, require exact presence/kind/correlation at the one authorized M5D7M take, and expose a successfully transferred payload to the later owner at most once.
- **REQ-M5D7N-006:** accept preservation denial, save failure, and uncertain commit receipts as safe pending hub handoffs.
- **REQ-M5D7N-007:** fail closed on lifecycle, identity, receipt, router, or notification violations without publishing or retrying; a successfully returned notification is the sole authorized source-ownership transfer and is retained locally.
- **REQ-M5D7N-008:** add no scene, visible UI, focus, menu, rendering, package, art, input mutation, profile/save/retry, run, gameplay, narrative, network, RNG, or logging authority.

## Acceptance criteria

- **AC-M5D7N-001:** clean primary, default bootstrap, previous promotion, decode repair, and binding-apply repair each publish one exact UI-only handoff.
- **AC-M5D7N-002:** preservation denial, save failure, and commit uncertainty each publish `HubEntryAccepted=true` while retaining exact pending evidence.
- **AC-M5D7N-003:** all three notification kinds and true notification absence transfer exactly; prior-consumed expected notification, unexpected notification, kind mismatch, and receipt-correlation mismatch fail terminally; the second local take returns false/default.
- **AC-M5D7N-004:** supported adapter/latch `Awake` and `Start` permutations prove `Pristine → AwaitingStart → AwaitingFirstUpdate` and converge only at the first `Update`, with one attempt and no subsequent source read.
- **AC-M5D7N-005:** null/malformed receipt, foreign/faulted/non-UI-only router, wrong GameObject, disabled/inactive cohort, and reflected illegal state publish nothing and fail terminally.
- **AC-M5D7N-006:** prepublication disable/destroy consumes no source notification; postpublication disable/destroy changes neither handoff nor router/source state.
- **AC-M5D7N-007:** duplicate `Update`, reactivation, and duplicate notification take cannot republish, retry, or reconsume.
- **AC-M5D7N-008:** reflection mutation of each handoff/state/notification correlation field fails validation, and static scope checks find every forbidden authority absent.
- **AC-M5D7N-009:** focused M5D7N, direct M5D7M, full EditMode, and full PlayMode finish with failed/skipped/inconclusive zero; Luna reports P0=0/P1=0.

## Proposed implementation allowlist

- new `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` and `.meta`
- new `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubEntryHandoffLatchV1Tests.cs` and `.meta`
- this contract, M5D7N pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry after approval

No M5D7M/M5D7L/InputRouter source, asmdef, generated actions/input asset, scene, prefab, package, ProjectSettings, visible UI, Profile runtime, gameplay consumer, or unrelated file may change.

## Stop and rollback conditions

Stop before approval if M5D7M is not Verified; a handoff requires scene discovery, UI construction, router mutation, retry/persistence authority, or a file outside the allowlist; first-Update ordering cannot be proven without execution-order dependence; notification correlation cannot be retained; or a valid pending receipt would be rejected. Rollback removes only the new latch/tests/evidence and its documentation entry.

## Participation record

- Sol supplied the bounded architecture, lifecycle/state matrix, requirements, acceptance criteria, and user-decision audit on 2026-09-13.
- Astra converted the recommendation into this Draft and retains approval/integration authority.
- Luna completed the amended-contract pre-gate with PASS (`P0=0`, `P1=0`, `P2=1`) on 2026-09-13. Terra is assigned only after this Astra approval; Luna remains the independent post-reviewer.
- No Ollama model or external cloud prompt participated.

## User-decision audit

No user decision is required for this code-only latch. The later visible hub UI contract must stop for menu order, initial focus, notification placement/duration/dismissal, UI technology, and scene/prefab topology decisions.
