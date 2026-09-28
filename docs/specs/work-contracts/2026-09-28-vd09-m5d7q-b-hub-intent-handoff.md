---
status: Verified
---

# VD-09 M5D7Q-B hub intent handoff (synthetic-only)

- Date: 2026-09-28
- Status: Verified by Astra on 2026-09-28 (synthetic-only; Approved at execution)
- Owner and approval/integration: Astra
- Bounded design counter-review: Sol
- Intended implementation: Terra
- Independent verification: Luna
- Dependencies: M5D7N, M5D7O, M5D7P-A, M5D7Q0, M5D7Q-A Verified
- Parent requirements: `REQ-UX-004`, `REQ-UX-006`, `REQ-UX-009`, `REQ-PLAT-004`, `REQ-PLAT-006`

## Outcome and non-scope

Transfer the existing Q-A presenter's single retained `HubMenuIntentV1` and
its exact hub-entry receipt to one typed, scene-independent application
boundary. A successful transfer produces a request for one of the four known
menu items, but **does not execute the request**. This is a deliberately
synthetic handoff; it establishes ownership and failure behavior before
product destinations exist.

This unit does not call `SceneManager`, `Application.Quit`, profile create/reset
or save, settings, wardrobe, run start, CIO/CUA, gameplay, or media. It does not
add a destination scene, edit build settings, make a sandbox scene a product
destination, or change the menu's authored layout/copy/input bindings. It does
not claim the 15-minute demo is playable from the menu.

## Contract

The Q-A presenter remains the only owner of the UI semantic frame and menu
controller. It gains one serialized `_handoffOwner` reference to the exact
`HubMenuIntentHandoffOwnerV1` component on its own `HubMenuRoot` instance.
Its sole new transfer API is
`internal bool TryTakeRetainedIntent(HubMenuIntentHandoffOwnerV1 owner, out HubMenuIntentRequestV1 request)`.
The API checks component reference identity against `_handoffOwner`, requires
`IntentRetained`, validates the copied intent and exact `_handoff` against
their Q-A proof fields, and marks a separate consumed-transfer proof before
returning one immutable request. A foreign/null owner or malformed proof
throws and closes the transfer boundary; a second take or take before
retention returns false/default. A failed/closed presenter never transfers.
No generic getter, static accessor, UnityEvent, or callback is added.
Taking the intent leaves menu controls locked; it cannot reactivate Q-A or
consume another input frame.

One additive `HubMenuIntentHandoffOwnerV1` owner on the existing `HubMenuRoot`
receives that pair from the exact scene instance of `HubMenuPresenterV1`.
Its serialized `_presenter` reference and Q-A's `_handoffOwner` reference are
reciprocal; builder and validator prove
`ReferenceEquals(owner.Presenter, presenter)` and
`ReferenceEquals(presenter.HandoffOwner, owner)` on the prefab and scene
instance.
It has `[DefaultExecutionOrder(-170)]`, after the Q-A presenter (`-180`), and
polls at most once in `LateUpdate` of each render frame. An intent produced
by Q-A `FixedUpdate` is eligible on the following `LateUpdate` only after
Q-A's `Update`. If Q-A disables/destroys first, the owner closes without a
request. It has one monotonic lifecycle: `AwaitingIntent -> RequestReady ->
RequestTaken`, or `Failed`/`Closed`. A consumer may take the immutable typed
request once. `HubMenuIntentRequestV1` contains only a known
`HubMenuItemV1` and the exact `HubEntryHandoffReceiptV1` value. Its
constructor, owner receive, and consumer take validate local item, receipt,
and immutable request proofs. Only the presenter transfer compares the
request receipt to Q-A's private `_handoff` and `_handoffProof` for exact
value equality. "Foreign" means not equal to that Q-A handoff. A copied
request/proof mismatch fails closed. There are no scene
names, build indices, paths, delegates, serialized profile bytes, or mutable
Unity references in it. Unknown enum values, receipt mismatch, duplicate
owner, and missing presenter fail closed with no second request.

Teardown is exact: disable/destroy in `AwaitingIntent` closes with no request;
disable/destroy in `RequestReady` discards the untaken request and closes;
disable/destroy in `RequestTaken` closes without reissuing it. A closed/failed
owner never publishes or returns another request. The authored prefab and
runtime topology both require exactly one Q-B owner and exact reciprocal
Q-A/owner references. Duplicate owner is a topology fault before polling.

The handoff owner must not poll or subscribe to input, construct another
controller/cursor/router, or execute an effect. One deterministic frame-bound
poll of the presenter's retained state is allowed in the stated `LateUpdate`;
the requester is not allowed to consume the presenter's menu intent before
Q-A has locked controls. Synthetic tests may exercise the same boundary with
injected, non-effectful values. No global/static service locator is allowed.

## Requirements

- **REQ-M5D7QB-001:** Q-A's retained intent and its correlated hub-entry
  receipt transfer atomically to exactly one bound handoff owner, no earlier
  than `IntentRetained`.
- **REQ-M5D7QB-002:** The transfer and downstream request are each one-shot;
  duplicates, late takes, unknown values, and malformed or foreign receipts
  do not publish a request or unlock menu controls.
- **REQ-M5D7QB-003:** The request is immutable and typed, representing exactly
  one of `Continue`, `NewGame`, `Settings`, or `Quit`, without effect execution
  or a destination string/index.
- **REQ-M5D7QB-004:** Missing/duplicate authoring, failure, and teardown are
  terminal and fail closed; a late frame or consumer cannot resurrect an
  intent or produce a second request.
- **REQ-M5D7QB-005:** Q-A, Q0, profile, run, costume, scene, build settings,
  package, and gameplay behavior remain unchanged outside the explicitly
  admitted presenter handoff and HubMenuRoot composition delta.

## Acceptance criteria

- **AC-M5D7QB-001:** Given each of the four accepted Q-A menu selections in
  synthetic tests, when the bound owner observes `IntentRetained`, then it
  receives exactly one exact correlated typed request, and Q-A remains locked.
- **AC-M5D7QB-002:** Given a second take, pre-retention take, unknown enum,
  malformed/foreign receipt, or an unbound/duplicate owner, when transfer is
  attempted, then no new request is published and no effect is invoked.
- **AC-M5D7QB-003:** Given request publication, when the consumer takes it
  twice or the component is disabled/destroyed in each of `AwaitingIntent`,
  `RequestReady`, and `RequestTaken`, then the above teardown table holds,
  at most one request leaves the boundary, and no late frame reopens it.
- **AC-M5D7QB-004:** Static scope inspection and authored-prefab tests prove
  absence of scene/profile/run/settings/wardrobe/CIO/CUA/gameplay effects,
  second input owner, destination strings/indices, and unintended Q-A/Q0 or
  asset changes. Reflection-corruption tests cover owner binding, consumed
  transfer proof, request proof and receipt correlation. Focused Q-A and Q-B
  regressions pass with failed/skipped/inconclusive zero.

## Traceability

`REQ-M5D7QB-001 -> AC-M5D7QB-001/002`; `REQ-M5D7QB-002 ->
AC-M5D7QB-002/003`; `REQ-M5D7QB-003 -> AC-M5D7QB-001/004`;
`REQ-M5D7QB-004 -> AC-M5D7QB-002/003`; `REQ-M5D7QB-005 -> AC-M5D7QB-004`.

## Implementation gate and follow-on

Luna pre-reviewed the exact Q-A extension, ownership, testability, and scope
at `PASS — P0=0, P1=0`; Astra approved this bounded synthetic contract on
2026-09-28. An actual effect executor requires a separate contract that first
fixes the product destination and the meaning of New Game, Settings, and Quit.
`Continue` means profile continuation from the hub, never restoration of an
active expedition; this document does not alter that approved meaning.

Allowed implementation surface after approval is exactly:

- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs`
  (only handoff-owner field, configuration, guarded transfer/proof, and
  lifecycle closure; no layout, copy, or input interpretation changes);
- new `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs`
  and `.meta` (the request type may live in the same source);
- `Assets/Prefabs/Hub/HubMenuRoot.prefab` (one component and reciprocal
  serialized references only);
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs`
  and `HubPresentationAuthoringValidator.cs` (only the exact one-owner
  composition and validation; existing visual/capture behavior fixed);
- new `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs`
  and `.meta`, new
  `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/HubMenuIntentHandoffPlayModeTests.cs`
  and `.meta`, and necessary direct additions to existing Q-A presenter or
  authoring tests;
- this contract, its focused verification evidence, and navigation entries.

No assembly definition, Q0 prefab, scene, ProjectSettings, package, profile,
run, costume, gameplay, font, shader, atlas, or unrelated file may change.

## Final integration — 2026-09-28

Terra implemented the exact bounded handoff. The first sandbox Unity attempt
did not reach tests because of license/package initialization and was
preserved as a separate failure record. Four later Unity `6000.6.0f1`
host-context runs exited naturally: Q-B focused EditMode `5/5`, Q-B focused
PlayMode `4/4`, direct HubPresentation EditMode `68/68`, and direct
HubPresentation PlayMode `8/8`, all failed/skipped/inconclusive `0`.

Luna independently reviewed `AC-M5D7QB-001..004` and reported
`PASS — P0=0, P1=0, P2=1` in
`docs/verification/2026-09-28-vd09-m5d7q-b-luna-postreview.md`. The sole P2
is the absence of an injected malformed serialized two-owner prefab test;
authored exact-one, `DisallowMultipleComponent`, validator, and runtime
fail-closed guards are present. Astra accepts this bounded synthetic-only
implementation as `Verified`. No actual menu effect or scene destination is
approved by this closure.
