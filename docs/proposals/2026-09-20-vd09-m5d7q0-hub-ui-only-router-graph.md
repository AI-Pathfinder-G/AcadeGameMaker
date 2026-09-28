# VD-09 M5D7Q0 Hub-UIOnly router graph — Sol design proposal

- Status: Draft; not implementation authority
- Date: 2026-09-20
- Design/counter-review: Sol (`gpt-5.6-sol`)
- Final contract owner and approval authority: Astra
- Intended implementation/review after approval: Terra / Luna
- Dependencies: M5B5, M5D7M, M5D7N, M5D7O and M5D7P-B Verified;
  M5D7P-A must become Verified before implementation begins
- Successor: M5D7Q authored uGUI/TMP hub presentation

## Bounded outcome

Author one reusable `HubRuntimeRoot` prefab whose existing `InputRouter` owns
the prepared `GameInputActions`, enables only the UI map, and publishes the
M5D7P-A semantic frame without requiring Movement, Transfer, Combat, terminal,
or camera objects. Reuse the verified M5D7M launch-adapter and M5D7N handoff
latch lifecycle. Do not author a Canvas, EventSystem, TMP object, menu,
gameplay scene, scene transition, profile mutation, or gameplay graph.

This proposal is deliberately limited to the runtime input/launch root needed
by the later M5D7Q presentation. It is not Astra-approved and cannot authorize
code, test, prefab, or asset implementation. M5D7P-A is currently Approved but
not Verified; Q0 implementation is forbidden until its post-review and Astra
integration advance it to Verified.

## Why the current graph cannot be authored without a code change

The current `InputRouter` serializes `PlayerMovementController`,
`TransferSimulationDriver`, `OrdanBossCombatSimulationDriver`, optional
terminal teardown, and a camera provider. Its initialization requires the
player, transfer, combat, and camera references, verifies their shared player,
and registers the three gameplay consumers. Every fixed commit then obtains
the next tick from the player and prepares, validates, and commits Movement,
Transfer, and Combat, including while the effective mode is `UIOnly`.
`RequestMode` also obtains its tick authority from the player.

Consequently, leaving those fields null does not produce a UI-only router; it
fails initialization. Authoring the complete M5B5 graph behind a hub menu is
also unsafe: movement/combat simulation and terminal state would continue to
exist and could fault or transition behind the presentation. Adding a second
`HubUiInputRouter`, direct UI action callbacks, or an
`InputSystemUIInputModule` would instead violate the verified single generated-
wrapper subscriber, map owner, and M5D7P-A semantic-frame source boundary.

The minimum safe change is therefore one explicit, fail-closed authoring
variant inside the existing `InputRouter`. The default and every existing
serialized object remain the current full-simulation variant.

## Scoped relationship to M5D7M and M5B5

M5D7M `REQ-M5D7M-004` states that prepared hub initialization uses a complete
existing M5B5 Movement/Transfer/Combat/Camera graph and stops rather than
inventing a lighter router. That requirement remains binding for every
historical object/evidence and every current or future `FullSimulation`
variant. Q0 proposes one later exception that applies **only** when the object
is the exact Q0 canonical `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`, passes its
fail-closed validator, and carries the explicit HubUIOnly authoring flag:

- `FullSimulation`: all existing M5B5 graph, preflight, atomic consumer
  transaction, receipt, failure, and regression obligations remain unchanged.
- canonical Q0 `HubUIOnly`: the same `InputRouter` owns the adopted generated
  wrapper and publishes M5D7P-A frames, but no gameplay consumer exists and no
  M5B5 three-consumer/barrier claim is made. A hand-authored lookalike, scene
  object, variant prefab, or any other router cannot claim this exception.

M5D7M reservation, adoption, exact owner/action identity, prepared
initialization, profile publication, notification, and ownership lifecycle are
reused unchanged. M5D7N notification take and exact router/adapter handoff are
also reused unchanged. No historical contract, full graph, state row, test, or
evidence is rewritten or generalized by this exception.

## Proposed closed authoring variant

Add a serialized boolean whose default is false:

```csharp
[SerializeField] private bool _authoredHubUiOnly;
internal bool AuthoredHubUiOnly { get; }
internal void ConfigureHubUiOnlyForAuthoring();
```

A boolean is preferred over a new serialized enum because a missing field on
all existing scenes/prefabs deserializes to false and preserves the current
path. `ConfigureHubUiOnlyForAuthoring` is assignment-only and legal solely on
a pristine, inactive, pre-`Awake`/pre-reservation/pre-initialization object. It
sets the flag and `InputMode.UIOnly`, requires every gameplay/terminal/camera
reference to already be null, and never silently clears a reference. Existing
`ConfigureForAuthoring` remains full-graph-only and rejects a router already
marked HubUIOnly.

HubUIOnly owns a private local next tick and its independent proof. The first
successful publication uses tick `0`, frame ordinal `0`, mode epoch `0`, and
`UIOnly`. Tick and ordinal increase together exactly once per successful fixed
commit. This tick is an instance-local input-frame identity only; it is not
persisted, transferred to another scene, or compared with a gameplay router.

## Initialization and fixed-commit behavior

`Initialize` selects exactly one branch before reading any gameplay reference.

The existing full branch is unchanged. The hub branch is legal only through
`InitializePreparedHub` with the exact adopted owner/actions. It requires:

- prepared state `Adopted` and exact owner/action identity;
- authored initial and effective mode `UIOnly`;
- player, transfer, combat, terminal, and camera references all null;
- no registered requester or pending mode request;
- pristine hub tick/ordinal/proof and publication state;
- Gameplay map disabled and UI map enabled after initialization.

It registers no gameplay consumer. Both generated callback sets may remain
registered on the one adopted wrapper because the router is already their sole
owner, while the Gameplay map remains disabled.

`InputRouter.Start` must detect HubUIOnly plus `Unreserved` before legacy action
allocation and enter a terminal fault. Thus a missing launch adapter cannot
quietly create a second lifecycle. `InitializeForTests` rejects HubUIOnly; hub
tests exercise the actual adapter reservation/adoption path.

For each HubUIOnly fixed commit:

1. Validate the authoring flag, exact null-reference matrix, prepared owner,
   action/callback ownership, prior publication proof, and map state.
2. Require `UIOnly`, epoch `0`, and no requester or pending request.
3. Create a candidate receipt from local tick and frame ordinal.
4. Build and validate the M5D7P-A semantic frame from the frozen pending UI
   batch.
5. Precompute checked tick/ordinal successors and diagnostics.
6. Only after all fallible work, consume transient UI edges/scroll, retain the
   same-epoch Navigate/Point state, assign successors and frame/proof, and
   publish the authoritative receipt last.

No Movement, Transfer, Combat, camera, terminal, gameplay ID, or gameplay
candidate path is touched. A failure preserves the last good frame/receipt and
the not-yet-successfully-consumed pending UI batch, disables both maps, stops
all further input consumption, and cannot retry.

Action lifetime follows the existing M5D7M state matrix rather than inventing
a Q0 shortcut. Before `ConfirmPreparedHubPublication`, any failure uses the
existing exact-owner `FailPreparedHubLaunch` path: the router enters prepared
`Failed`, suppresses fallback/publication/maps, and closes the owned candidate
exactly once. After successful confirmation/publication, an `Initialized`
runtime fault immediately closes maps and semantic consumption but retains the
exact adopted wrapper under router ownership; standard router
`OnDisable`/`OnDestroy` performs the one unsubscribe/dispose closure exactly
once. Q0 must not add a new M5D7M stable state row to close actions eagerly.

## Mode, lifecycle, and handoff order

HubUIOnly never registers a mode requester and never changes from `UIOnly`.
`RegisterModeRequester` and `RequestMode` reject before player dereference and
without mutation. The later hub presenter locks its controls after publishing
one menu intent; a separate scene executor may unload the hub. The gameplay
scene receives its own ordinary full-simulation router. Q0 does not own that
executor or scene transition.

Expected lifecycle:

| Unity boundary | Order | Required action |
|---|---:|---|
| `Awake` | `-220` | launch adapter reserves, prepares, and adopts actions |
| `Awake` | `-210` | router records lifecycle entry without allocation |
| `Awake` | `-190` | handoff latch validates exact adapter/router identity |
| `Start` | `-220` | adapter calls `InitializePreparedHub` and confirms publication |
| `Start` | `-210` | initialized router is a no-op |
| `Start` | `-190` | latch waits for first `Update` |
| `FixedUpdate` | `-210` | router publishes the first hub receipt/frame |
| `Update` | `-190` | latch takes the exact notification and publishes handoff |

Correctness must retain M5D7M's router-first/adapter-first `Awake` permutation
coverage and must not rely on undocumented component list order.

## Authoring topology

Q0 owns one prefab only, plus its stable Unity metadata:

```text
Assets/Prefabs/Hub/HubRuntimeRoot.prefab
Assets/Prefabs/Hub.meta
Assets/Prefabs/Hub/HubRuntimeRoot.prefab.meta
HubRuntimeRoot
├─ Transform
├─ InputRouter
│  ├─ authoredHubUiOnly = true
│  ├─ initialMode = UIOnly
│  └─ player/transfer/combat/terminal/camera = null
├─ DesktopProfileLaunchAdapterV1
│  └─ router = the exact sibling InputRouter
└─ HubEntryHandoffLatchV1
   ├─ launchAdapter = the exact sibling adapter
   └─ router = the exact sibling InputRouter
```

The prefab contains no Camera, Canvas, EventSystem, TMP/uGUI component,
movement, transfer, combat, terminal requester, mode requester, presentation
controller, or scene-transition component. M5D7Q later owns `Hub.unity` and a
separate `HubMenuRoot` UI prefab; it composes connected prefab instances rather
than Q0 pre-owning a scene that the presentation unit must rewrite.

The builder should use typed, pre-lifecycle, assignment-only seams on
`DesktopProfileLaunchAdapterV1` and `HubEntryHandoffLatchV1`. It must not use
`SerializedObject.FindProperty` against private field names. Those seams set
references only and perform no profile read, reservation, input allocation,
initialization, notification, or handoff operation.

## Fail-closed proof boundary

Add `InputRouterFaultStage.HubUiOnly` only at the end of the enum so existing
numeric values remain stable. Existing callback/frame corruption continues to
use `UiCapture`. Hub authoring/lifecycle/map/tick failures use `HubUiOnly`.
`LatchFault` chooses the hub local next tick when the hub variant is active;
the full path remains unchanged.

The hub null-reference and state matrix is checked at authoring validation,
`Awake`, `Start`, prepared initialization, fixed commit, source getters, and
teardown. Reflection mutation, hybrid full/hub references, map drift, callback
or action identity drift, illegal mode/epoch, or checked tick/ordinal overflow
is terminal and is never normalized.

Failure injection must cover every prepared boundary separately: Reserve,
environment/Prepare, Take, Adopt, callback registration, initialization,
proof/receipt/notification staging, and confirmation. Each case proves which
side owns a candidate, fallback suppression, both-map closure, absence of
desktop/input publication, and the appropriate existing M5D7M closure path.
The healthy initialized state has hub next tick/ordinal `0/0` while the input
receipt, UI frame, and independent publication proof are all absent; only the
first successful fixed commit publishes `0/0` and stages successor `1/1`.

Adversarial coverage mutates the receipt, frame, independent proof, each
successor, hub flag, each map state, requester identity, and pending request
individually. Tick and ordinal overflow are separate fixtures. For the 30, 60,
and 144 render-FPS comparison, the exact same callback script is injected
before the exact same numbered fixed boundaries; only render opportunities
differ. Domain-reload coverage means teardown of one session followed by
creation and execution of a fresh session, not preservation of runtime state
through reload. Two builder runs must preserve both canonical prefab bytes/
serialization and the folder/prefab `.meta` GUIDs.

Do not extend `PreparedHubInitializationProofV1` or the desktop profile receipt
solely to encode the graph kind. The graph kind belongs to Q0 and is proven by
the router's closed authoring property, runtime invariant, and prefab validator.
If Luna demonstrates that this cannot prove an acceptance criterion without
trusting mutable state, stop for Astra rather than silently widen M5D7M data.

## Conflict and risk analysis

- **P0 — competing input owner:** a second router, UI module, or direct action
  subscription conflicts with M5B5/M5D7P-A. Resolved by extending only the
  existing router.
- **P0 — hidden gameplay:** satisfying M5D7M literally with an invisible full
  graph permits gameplay simulation behind the menu. Resolved by an explicit
  zero-consumer variant rather than disabled-looking gameplay components.
- **P0 — legacy fallback:** a missing adapter could allocate actions in
  `Start`. Resolved by early HubUIOnly rejection before allocation.
- **P1 — source tick:** the hub has no player tick. Resolved by an isolated,
  checked local frame tick tied one-to-one to M5D7P-A ordinal and never reused
  as simulation time.
- **P1 — authoring private fields:** string-based serialized injection is
  brittle. Resolved by typed assignment-only authoring seams.
- **P1 — historical proof meaning:** adding graph kind to M5D7M receipts could
  retroactively widen their meaning. Resolved by Q0-owned proof and validator.
- **P1 — post-publication action closure:** eager disposal on an Initialized
  fault would conflict with M5D7M's stable state matrix. Resolved by immediate
  map/consumption closure and standard Disable/Destroy owner closure; requiring
  a new M5D7M row is a stop condition.
- **P2 — actual UI topology:** Canvas scaling, EventSystem ownership, TMP
  consumers, focus/hover/click, notification dismissal, and accessibility
  remain M5D7Q responsibilities, not Q0.

## Recommendation and open gate

Adopt the single-router HubUIOnly variant and prefab-only topology above. Luna's
first Q0 pre-gate reported `P0=0`, `P1=5`, `P2=3`; this revision narrows the
M5D7M exception to the canonical prefab, aligns fault teardown with the existing
state matrix, and specifies the missing lifecycle, mutation, timing, reload,
and GUID evidence. No new user product decision is required. Astra must still
approve the amended API/allowlist after Luna re-review. Terra must not implement
Q0 while M5D7P-A remains anything other than Verified.
