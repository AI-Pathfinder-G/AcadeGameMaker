# VD-09 M5D7Q0 Hub-UIOnly router authoring graph

- Status: Verified — Terra implementation, Luna `ACCEPT` (`P0=0`, `P1=0`,
  `P2=0`), and Astra final integration acceptance on 2026-09-23
- Owner and final approval authority: Astra
- Contract design: Sol
- Intended implementer after approval and all gates: Terra
- Independent reviewer: Luna
- Dependencies: M5B5, M5D7M, M5D7N, M5D7O and M5D7P-B Verified;
  **M5D7P-A Verified is a hard implementation prerequisite**
- Parent requirements: `REQ-UX-004`, `REQ-UX-006`, `REQ-UX-009`,
  `REQ-UX-014`, `REQ-PLAT-009`
- Proposed acceptance IDs: `AC-M5D7Q0-001` through `AC-M5D7Q0-012`
- Approval state: Astra approved this contract on 2026-09-20 after confirming
  M5D7P-A Verified and accepting Luna's amended pre-gate PASS
  (`P0=0`, `P1=0`, `P2=0`). Implementation authority is limited to the exact
  allowlist and verification sequence below.

## Purpose and boundary

Create the smallest authorable runtime root that permits the verified desktop
profile launch and handoff lifecycle to remain in `UIOnly` and publish the
M5D7P-A semantic input stream without authoring Movement, Transfer, Combat,
terminal, or camera dependencies. The unit adds one closed HubUIOnly variant
to the existing `InputRouter`, typed authoring seams, a deterministic builder
and fail-closed validator, and one `HubRuntimeRoot` prefab.

The existing router remains the sole generated `GameInputActions` callback and
map owner and the sole `IUiSemanticFrameSourceV1`. Q0 does not add a second
router, `InputSystemUIInputModule`, direct UI callback subscriber, virtual
mouse, Canvas, EventSystem, TMP/uGUI object, menu presentation, scene, scene
transition, profile mutation, run/gameplay state, persistence, or narrative
authority.

M5D7M `REQ-M5D7M-004` and its complete M5B5 graph remain mandatory for every
historical object/evidence and every current or future full-simulation variant.
This later contract proposes one exception that applies only to the exact Q0
canonical `Assets/Prefabs/Hub/HubRuntimeRoot.prefab` with its explicit
HubUIOnly flag and a passing Q0 validator. No hand-authored lookalike, variant
prefab, scene object, or other router may claim the exception. The canonical
variant has no gameplay consumers and therefore makes no M5B5 three-consumer
atomic-barrier claim. M5D7M reservation/adoption/publication/state rows,
M5D7N notification/handoff behavior, FullSimulation, and all past evidence
remain unchanged.

No implementation may begin until both conditions are true:

1. M5D7P-A has independent post-review evidence and Astra has marked it
   `Verified`.
2. Astra has reviewed this Draft, accepted any Luna pre-gate amendments, and
   changed this contract to `Approved`.

## Normative design

### Closed authoring variants

`InputRouter` gains a serialized `_authoredHubUiOnly` flag defaulting to false,
an internal read-only authoring getter, and a pre-lifecycle
`ConfigureHubUiOnlyForAuthoring` seam. Missing serialized data therefore keeps
all existing scenes/prefabs on the current full-simulation path.

The hub authoring seam is legal only before `Awake`, `Start`, reservation,
action allocation, callback registration, initialization, fault, or disposal.
It requires the object to be inactive, all gameplay/terminal/camera references
to already be null, and the authored mode to be set to `UIOnly`. It never
silently clears references. Existing `ConfigureForAuthoring` remains the full-
graph seam and rejects a hub-marked router.

`DesktopProfileLaunchAdapterV1` and `HubEntryHandoffLatchV1` may gain only
typed assignment-only authoring seams and read-only identity getters required
by the builder/validator. These seams operate only in pristine pre-lifecycle
state, assign references only, and may not reserve, prepare, read a profile,
allocate actions, register callbacks, initialize, publish, take a notification,
or create a handoff.

### Prepared initialization

HubUIOnly initialization is legal only through the exact M5D7M prepared path:

`Unreserved → Reserved(owner) → Adopted(owner, actions) →
InitializedPendingPublication(owner) → Initialized`

The hub branch requires exact adopted owner/actions, initial mode `UIOnly`, all
five gameplay/terminal/camera references null, no requester or pending mode
request, pristine hub clock/publication state, Gameplay map disabled, UI map
enabled, and both generated callback sets owned only by this router. It
registers no Movement/Transfer/Combat consumer.

A HubUIOnly router reaching `Start` in `Unreserved` faults before legacy action
allocation or callback registration. `InitializeForTests` rejects HubUIOnly;
focused tests use the real adapter lifecycle. Existing full-simulation
initialization and legacy fallback remain byte- and behavior-compatible.

### Hub frame transaction

HubUIOnly owns a private checked next-tick value and exact proof. Its first
successful receipt is `(tick=0, frameOrdinal=0, modeEpoch=0, mode=UIOnly)`.
Tick and ordinal increase together by exactly one for each successful fixed
commit; epoch remains zero. The tick is local input-frame identity only and is
never persisted or compared with another router or gameplay simulation.

Each hub commit performs this order:

1. Validate the hub/full discriminator, exact null-reference matrix, prepared
   owner/action/callback identity, previous receipt/frame proof, maps, mode,
   epoch, and absence of requester/pending request.
2. Construct a candidate receipt from the local tick and ordinal.
3. Construct and validate the M5D7P-A semantic frame from a frozen pending UI
   batch.
4. Precompute checked next tick/ordinal and all diagnostics.
5. After all fallible work, consume successful transient UI state, retain only
   same-epoch Navigate/Point current state, assign next clock/frame/proof, and
   publish the authoritative receipt last.

It performs no gameplay candidate, consumer, barrier, camera, terminal, or ID
operation. Any failure preserves the prior good receipt/frame and the pending
batch not yet successfully consumed, disables both maps, prevents further
input consumption, and permits no retry.

Before `ConfirmPreparedHubPublication`, every failure after an exact reservation
uses the existing owner-bound `FailPreparedHubLaunch` path. Before adoption the
adapter retains and disposes its candidate according to M5D7M; after adoption
the router closes its owned candidate exactly once. There is no receipt or
notification publication, no fallback, and both maps remain disabled.

After successful confirmation and publication, a hub runtime fault keeps the
router in the existing M5D7M-compatible `Initialized` terminal-fault row. It
immediately disables both maps, invalidates the semantic source, and prevents
all further callback consumption/commit. It does not eagerly unsubscribe or
dispose the adopted wrapper. The standard router `OnDisable`/`OnDestroy` owner
teardown later closes that exact wrapper exactly once. A repeated fault,
Disable then Destroy, or Destroy after Disable cannot double-close it. If this
policy cannot be represented by the existing M5D7M state rows and closure
semantics, implementation stops for Astra rather than adding a new row.

### Mode and lifecycle boundary

HubUIOnly cannot register a mode requester or accept a mode request. Both APIs
reject before any player dereference and without mutation. HubUIOnly remains
`UIOnly`, epoch `0`, until its root is destroyed/unloaded. A destination scene
owns a separate ordinary full-simulation router; Q0 does not transition scenes
or transfer a router instance.

The expected lifecycle order is adapter `-220`, router `-210`, latch `-190`:
adapter reserve/prepare/adopt in `Awake`; router allocation-free lifecycle
entry; adapter prepared initialization and publication confirmation in
`Start`; first hub frame in router `FixedUpdate`; exact M5D7N handoff in latch
`Update`. Router-added-first and adapter-added-first authoring permutations
within the same inactive cohort must converge to that fixed execution order;
component insertion order is not evidence of a router-first `Awake` callback.

### Canonical prefab

`Assets/Prefabs/Hub/HubRuntimeRoot.prefab` has one active root GameObject and
exactly these components: `Transform`, HubUIOnly `InputRouter`,
`DesktopProfileLaunchAdapterV1`, and `HubEntryHandoffLatchV1`. Adapter and latch
reference the exact sibling router, and latch references the exact sibling
adapter. All gameplay/terminal/camera fields are null.

The prefab has no child, Camera, Canvas, EventSystem, UI/TMP component,
presentation controller, gameplay component, requester, or transition
executor. Q0 owns no scene. M5D7Q later composes this connected prefab with a
separate menu presentation prefab in the hub scene.

### Fail-closed validation

The builder creates or replaces only the canonical prefab and is deterministic
and idempotent, including preservation of the existing folder and prefab
`.meta` GUIDs on a second run. The validator is observational: it reports and rejects drift
but never repairs a candidate. It checks exact root/component count and order,
same-GameObject identities, hub flag/mode, null-reference matrix, no forbidden
component/child, no prefab override in its canonical asset, and no missing
script. It must use typed authoring seams, not private-field-name strings.

A new `InputRouterFaultStage.HubUiOnly`, if required, is appended without
renumbering existing members. M5D7P-A callback/frame corruption retains
`UiCapture`; hub graph, lifecycle, map, clock, and matrix failures use
`HubUiOnly`. Reflection mutation, hybrid references, map drift, owner/action/
callback drift, illegal mode/epoch, and checked successor overflow are terminal
and are not normalized.

Failure-injection evidence distinguishes Reserve, Prepare, Take, Adopt,
callback registration, initialization, proof/receipt/notification staging, and
confirmation. The stable post-initialization/pre-first-fixed state has local
next tick and ordinal `0/0` while input `CurrentReceipt`, `CurrentUiFrame`, and
independent UI publication proof are all absent. The desktop launch receipt is
not an input-frame publication. The first successful fixed commit alone
publishes input tick/ordinal `0/0`, with exact frame/receipt/proof equality and
checked successor `1/1`.

## Requirements

- **REQ-M5D7Q0-001:** preserve the existing `InputRouter` as the only generated-
  wrapper subscriber, map owner, and UI semantic-frame source while adding one
  explicit HubUIOnly authoring variant whose serialized default is false.
- **REQ-M5D7Q0-002:** initialize HubUIOnly only through the exact M5D7M prepared
  owner/action lifecycle and fault before any legacy allocation when that
  lifecycle is absent.
- **REQ-M5D7Q0-003:** publish the local tick, frame ordinal, zero epoch,
  M5D7P-A semantic frame, independent proof, and authoritative receipt as one
  fail-closed fixed transaction.
- **REQ-M5D7Q0-004:** keep all player, movement, transfer, combat, terminal, and
  camera dependencies absent and execute no gameplay consumer or barrier in
  HubUIOnly.
- **REQ-M5D7Q0-005:** keep Gameplay disabled, UI enabled, and callbacks/actions
  under the exact adopted router ownership without a second subscriber or UI
  input module.
- **REQ-M5D7Q0-006:** reject mode requester registration, mode requests, epoch
  changes, and every non-`UIOnly` hub state before mutation.
- **REQ-M5D7Q0-007:** reuse M5D7M launch publication and M5D7N notification/
  handoff correlation without changing their receipt meaning or historical
  evidence; the `REQ-M5D7M-004` exception applies only to the validator-passing
  canonical Q0 prefab and never to FullSimulation or any historical object.
- **REQ-M5D7Q0-008:** fault terminally on hybrid graph, map/callback/action drift,
  reflection corruption, invalid lifecycle, or checked clock overflow while
  preserving the last good publication and unconsumed pending input; use
  `FailPreparedHubLaunch` before confirmation, and after publication close maps/
  consumption immediately but defer exactly-once action-owner closure to
  standard Disable/Destroy.
- **REQ-M5D7Q0-009:** deterministically build and observationally validate one
  exact `HubRuntimeRoot` prefab with typed assignment-only authoring seams.
- **REQ-M5D7Q0-010:** preserve full-simulation M5B5/M5D7M behavior and introduce
  no scene, Canvas, EventSystem, TMP/uGUI presentation, transition, profile,
  persistence, run, gameplay, or narrative authority.
- **REQ-M5D7Q0-011:** prohibit implementation until M5D7P-A is Verified and this
  contract is explicitly Approved by Astra.

## Acceptance criteria

- **AC-M5D7Q0-001:** existing full-simulation fixtures and authored prefabs
  deserialize with the hub flag false, retain their current initialization and
  commits, and reject every hybrid full/hub matrix without partial mutation.
- **AC-M5D7Q0-002:** independent failure injection at Reserve, environment/
  Prepare, Take, Adopt, callback registration, initialization, initialization
  proof, receipt/notification staging, and confirmation proves exact candidate
  ownership and closure count at each boundary. Every failure suppresses
  fallback, desktop/input receipt, notification, and both maps. A failure after
  reservation but before confirmation uses `FailPreparedHubLaunch`; a failure
  before adoption leaves candidate disposal with the adapter, and a failure
  after adoption closes through the router exactly once. Missing, disabled,
  duplicate, or foreign adapters cannot allocate fallback or close a legitimate
  owner's candidate.
- **AC-M5D7Q0-003:** successful hub receipts begin at tick/ordinal `0/0`, remain
  epoch `0`/`UIOnly`, and advance both counters exactly once per successful
  fixed commit. Immediately after initialization and before the first fixed
  commit, local next tick/ordinal are `0/0` while input receipt/frame/proof are
  all absent; the first commit publishes exact equal receipt/frame/proof at
  `0/0` and checked successor `1/1`. Under 30, 60, and 144 render FPS, the same
  callback script injected before the same numbered fixed boundaries yields
  the same receipt/frame sequence; only render opportunities differ.
- **AC-M5D7Q0-004:** static inspection, prefab validation, and runtime spies prove
  zero gameplay/camera references, components, candidate calls, commits, IDs,
  requester registration, and second input subscription in HubUIOnly.
- **AC-M5D7Q0-005:** before and after initialization, requester registration and
  mode request attempts throw/fault as specified before player access and
  leave mode, epoch, receipt, frame, and pending input unchanged; the healthy
  pre-call map state is unchanged for a rejected public command, while a
  reflected requester or pending-request corruption at the next invariant
  boundary terminally disables both maps.
- **AC-M5D7Q0-006:** Primary, Previous, and Default profile scenarios plus every
  typed notification kind pass through the real adapter/coordinator/latch with
  exact router identity and exactly one M5D7N handoff; no Q0 API dismisses or
  localizes the notification.
- **AC-M5D7Q0-007:** separate fixtures mutate the hub flag, each map, requester,
  pending request, receipt, frame, independent proof, hub next-tick successor,
  and frame-ordinal successor; separate fixtures overflow tick and ordinal.
  Each next getter/commit boundary detects the exact corruption, preserves the
  prior forensic receipt/frame and unconsumed batch, disables both maps, and
  cannot retry, recover, or relabel the batch. Owner/action/callback mismatch
  and every full/hub hybrid reference are covered independently.
- **AC-M5D7Q0-008:** the builder run twice produces the same canonical prefab;
  the second run preserves the exact `Assets/Prefabs/Hub.meta` and
  `HubRuntimeRoot.prefab.meta` GUIDs and introduces no unrelated serialization
  delta;
  individual mutations of component count/order, sibling identity, hub flag,
  mode, each forbidden reference/component/child, missing script, and prefab
  override are independently rejected without auto-repair.
- **AC-M5D7Q0-009:** router-added-first and adapter-added-first construction of
  one inactive cohort both converge to the fixed adapter `-220` → router `-210`
  → latch `-190` `Awake` order; every legal `Start` ordering, disable/destroy
  before publication, disable/destroy after receipt, and domain reload preserve
  the exact M5D7M ownership/closure outcomes. Domain reload is tested only as
  teardown of the first session followed by creation and execution of a fresh
  session; no runtime state survives or is restored across reload.
- **AC-M5D7Q0-010:** focused Q0 EditMode/PlayMode, direct M5B5, M5D7M, M5D7N,
  and M5D7P-A regressions, then full EditMode and full PlayMode, finish with
  failure/skip/inconclusive zero; Luna reports `P0=0` and `P1=0`.
- **AC-M5D7Q0-011:** because the captured pre-run workspace was already dirty,
  the scope audit must not claim a clean repository-wide before/after boundary.
  Instead it verifies the immutable captured inventory commitment plus the
  exact 33-path Q0 baseline/current hash manifest, identifies every non-Q0
  path as pre-existing-or-unattributable without admitting it to Q0, and proves
  no Q0 scene/UI/package/generated-input/profile/gameplay path, new action
  subscriber, or second frame source. Evidence records separate Terra
  implementation and Luna verification ownership. The audit treats
  `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`,
  `Assets/Prefabs/Hub.meta`, and `HubRuntimeRoot.prefab.meta` as exact
  allowlisted Q0 files and no other new folder metadata as allowed.
- **AC-M5D7Q0-012:** one injected runtime fault after a successfully published
  hub input frame immediately disables both maps and all further input
  consumption while leaving the exact adopted actions owned and not disposed;
  one subsequent Disable/Destroy teardown unsubscribes/disposes that candidate
  exactly once, repeated fault/Disable/Destroy does not close twice, and no new
  receipt/frame/handoff is published. The same fixture proves this requires no
  new M5D7M stable state row.

### Traceability

| Requirement | Acceptance evidence |
|---|---|
| `REQ-M5D7Q0-001` | `AC-M5D7Q0-001`, `AC-M5D7Q0-004`, `AC-M5D7Q0-010` |
| `REQ-M5D7Q0-002` | `AC-M5D7Q0-002`, `AC-M5D7Q0-009` |
| `REQ-M5D7Q0-003` | `AC-M5D7Q0-003`, `AC-M5D7Q0-007` |
| `REQ-M5D7Q0-004` | `AC-M5D7Q0-004`, `AC-M5D7Q0-011` |
| `REQ-M5D7Q0-005` | `AC-M5D7Q0-002`, `AC-M5D7Q0-004`, `AC-M5D7Q0-007`, `AC-M5D7Q0-012` |
| `REQ-M5D7Q0-006` | `AC-M5D7Q0-005`, `AC-M5D7Q0-007` |
| `REQ-M5D7Q0-007` | `AC-M5D7Q0-006`, `AC-M5D7Q0-009`, `AC-M5D7Q0-010` |
| `REQ-M5D7Q0-008` | `AC-M5D7Q0-002`, `AC-M5D7Q0-007`, `AC-M5D7Q0-009`, `AC-M5D7Q0-012` |
| `REQ-M5D7Q0-009` | `AC-M5D7Q0-008`, `AC-M5D7Q0-011` |
| `REQ-M5D7Q0-010` | `AC-M5D7Q0-001`, `AC-M5D7Q0-004`, `AC-M5D7Q0-010`, `AC-M5D7Q0-011` |
| `REQ-M5D7Q0-011` | approval and dependency status review before any implementation commit |

## Proposed implementation allowlist

Runtime modifications:

- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs`, limited
  to appending the exact HubUIOnly diagnostic without renumbering existing values
- `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`,
  limited to the typed assignment-only authoring seam
- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs`,
  limited to typed assignment-only authoring seam/read-only identity
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`, limited to exact
  authoring/test friend-assembly entries required by this unit

New authoring files and their `.meta` files:

- `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`, limited to the Unity-
  generated folder metadata for this exact authoring subtree
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs`
- `Assets/Prefabs/Hub.meta`
- `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`
- `Assets/Prefabs/Hub/HubRuntimeRoot.prefab.meta`

Tests:

- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef`, limited to the one Hub Authoring Editor reference
- new focused Q0 EditMode tests under
  `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/`
- new focused Q0 PlayMode tests under
  `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/`
- test `.meta` files

Documentation/evidence:

- this contract and its proposal
- one Luna pre-gate, one Terra implementation evidence report, one Luna
  post-review report, the GPT participation ledger entry, and minimal indexes
- exact Luna post-review path
  `docs/verification/2026-09-22-vd09-m5d7q0-luna-independent-review.md`
  and its final deterministic-repair successor
  `docs/verification/2026-09-23-vd09-m5d7q0-luna-independent-review.md`

Persistent Unity execution evidence (Astra amendment, 2026-09-22):

- exact root `artifacts/unity-results/m5d7q0-20260922-reboot/`;
- `baseline-manifest.json`, historical `final-manifest.json`, and superseding
  `final-manifest-v2.json`;
- the following exact result stems, each limited to one `.xml` and one `.log`:
  `focused-edit-builder`, `focused-edit-builder-gui`,
  `focused-edit-builder-bundled`, `focused-edit-builder-reactivated`,
  `focused-edit-builder-repaired`, `focused-edit-builder-hubopen`,
  `focused-edit-scope`, `focused-play-router`,
  `focused-play-coverageb`, `focused-play-handoff`,
  `focused-play-remaining`, `direct-m5b5`, `direct-m5d7m`,
  `direct-m5d7n`, `direct-m5d7n-smoke`, `direct-m5d7pa`, `full-editmode`,
  `full-playmode`, `focused-edit-builder-process-b`,
  `scope-after-deterministic-fix`, `scope-after-deterministic-fix-r2`,
  `scope-after-deterministic-fix-final`, and
  `full-editmode-deterministic-fix`.

The baseline manifest records hashes before the resumed verification run; the
final manifest records all result/log hashes, counts, timestamps, exact Unity
arguments with secrets omitted, and the post-run allowlisted-source hashes.
No other file below this root is admitted. These artifacts are verification
evidence, not runtime/product scope. The static audit compares the two
manifests and the exact named Q0 source paths; it must not relabel arbitrary
dirty-worktree paths as pre-existing proof.

Explicitly outside the allowlist are `Assets/GameInput.inputactions`, generated
`GameInputActions.cs`, all other runtime/asmdef/test sources, `Packages/`,
`ProjectSettings/`, scenes, Canvas/EventSystem/TMP/font/menu assets, M5D7O
runtime, profile/persistence/run/gameplay/narrative sources, existing gameplay
prefabs, and unrelated files.

## Required verification sequence

1. Confirm M5D7P-A is `Verified`; otherwise stop without implementation.
2. Luna pre-gates this exact Draft or Astra-amended successor and reports all
   P0/P1 findings.
3. Astra resolves findings and changes the contract to `Approved`.
4. Terra records a clean allowlist baseline, implements only the approved
   files, and runs focused Q0 plus direct dependency regressions.
5. Terra runs full EditMode and PlayMode with XML-backed counts and zero
   failure/skip/inconclusive.
6. Luna independently reruns focused, direct, full, authoring-negative, and
   static-scope checks and maps evidence to every `AC-M5D7Q0-*`.
7. Astra alone decides integration and `Verified` status.

## Stop and rollback conditions

Stop before implementation if M5D7P-A is not Verified or this contract is not
Approved by Astra. Stop during review or implementation if the solution needs:

- a second generated-action subscriber, router, map owner, or semantic-frame
  source;
- `InputSystemUIInputModule`, virtual mouse, `Mouse.current`, polling, or a
  render-`Update` input queue;
- hidden Movement/Transfer/Combat/Camera objects in the hub;
- changes to generated input, M5D7M/M5D7N receipt schemas, profile/persistence,
  M5D7O behavior, scene transition, gameplay, or narrative;
- a new or altered M5D7M stable state row, or eager post-publication adopted-
  action disposal instead of standard Disable/Destroy closure;
- a Q0-owned scene, Canvas, EventSystem, UI/TMP/font object, presentation
  controller, or menu effect;
- runtime private-field reflection/string injection or a validator that repairs
  drift;
- transfer of a hub router instance into a gameplay scene;
- a user-facing product decision not already fixed by approved UX/canon;
- any inability to preserve full-simulation regressions or the prior semantic
  frame/receipt/pending batch at a fault boundary;
- any file outside the approved allowlist.

Rollback removes the Q0 prefab, authoring assembly, focused tests, friend/test
references, and bounded HubUIOnly router/seam additions. It restores the
existing full-simulation sources and documentation index without changing
M5D7P-A, generated input, packages, M5D7M/N/O, scenes, or historical evidence.

## Draft review record and open gate

- Sol determined that the current serialized graph cannot be made UI-only by
  authoring alone because initialization, fixed commit, and mode request paths
  dereference gameplay/player authority.
- Sol rejected both an invisible complete M5B5 hub graph and a second UI router
  as violations of safety or verified single ownership.
- The recommended minimal change is one false-by-default HubUIOnly branch in
  the existing router, a local frame tick, typed assignment-only authoring
  seams, and a prefab-only builder/validator boundary.
- Luna's first Q0 pre-gate reported `P0=0`, `P1=5`, `P2=3`. This amended Draft
  confines the M5D7M exception to the canonical prefab, aligns prepublication
  and post-publication closure with existing M5D7M rows, expands exact failure/
  corruption coverage, and fixes fixed-boundary, reload, and `.meta` evidence.
  It remains subject to Luna re-review and Astra approval.
- No user product decision is open. Astra must still approve the exact API,
  scoped M5D7M exception, allowlist, and acceptance evidence after Luna's
  pre-gate.
- Historical gate resolution: M5D7P-A is Verified and Astra approved this
  contract on 2026-09-20. The 2026-09-22 amendment adds only exact persistent
  verification-output paths and does not expand runtime or product authority.
- 2026-09-23 Astra evidence amendment: the original AC-011 clean before/after
  wording was impossible to prove from the honestly captured 374-record dirty
  inventory. Astra replaced it with the exact committed-inventory and 33-path
  Q0 hash boundary above, without broadening product-code scope, and admitted
  the final Luna successor report as documentation evidence. This amendment
  does not relabel or approve any unrelated dirty-worktree path.
- 2026-09-23 final integration: Astra accepts REQ-M5D7Q0-001..011 and
  AC-M5D7Q0-001..012 on the amended evidence boundary. Luna independently
  verified the complete v2 manifest and returned `ACCEPT` with no remaining
  findings. Final evidence includes deterministic builder process A/B `31/31`,
  amended scope `4/4`, full EditMode `737/737`, and impact-approved reuse of
  full PlayMode `943/943`.
