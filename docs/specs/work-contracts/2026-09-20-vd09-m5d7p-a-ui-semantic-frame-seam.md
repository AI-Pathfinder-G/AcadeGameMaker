# VD-09 M5D7P-A UI semantic frame seam

- Status: Verified
- Owner and final approval authority: Astra
- Design/counter-review: Sol
- Intended implementer after approval: Terra
- Independent reviewer: Luna
- Dependencies: M5B5 Verified, M5D7N Verified, M5D7O Verified
- Parent requirements: `REQ-UX-004`, `REQ-UX-006`, `REQ-UX-009`, `REQ-UX-013`
- Proposed acceptance IDs: `AC-M5D7PA-001` through `AC-M5D7PA-008`
- Source proposal: `docs/proposals/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md`
- Astra approval: Approved on 2026-09-20 after Luna's amended independent
  pre-gate reported PASS (`P0=0`, `P1=0`, residual `P2=1`). The residual P2
  keeps M5D7O typed-notification/correlation continuity as a mandatory
  downstream authored-presentation gate; it does not block this input seam.
- Astra integration acceptance: Verified on 2026-09-20 after Terra's final
  implementation evidence and Luna's independent post-review reported
  `P0=0`, `P1=0`, `P2=2`, with focused `5/5` EditMode and `52/52` PlayMode,
  direct M5B5 `3/3` and M5D7O `19/19`, and full `693/693` EditMode and
  `855/855` PlayMode passing with zero failed, skipped, or inconclusive tests.
  The two P2 boundaries remain nonblocking and are carried forward exactly as
  recorded in the Luna post-review.

## Purpose and boundary

Extend the one existing `InputRouter` so the six UI actions it already owns in
`UIOnly` publish one immutable semantic frame tied to the exact successful
`InputFrameCommitReceipt`. This is a technical prerequisite for authored hub
presentation. It does not add a second action subscriber, lightweight router,
`InputSystemUIInputModule`, virtual mouse, action-map owner, scene, Canvas,
uGUI/TMP dependency, safe-frame hit test, focus policy, menu effect,
localization, or notification reconstruction.

The source is latest-value only. A future presentation owner must advance the
identity-bound cursor once per fixed step at an execution order strictly after
the router's `-210` commit. Render-`Update`-only consumption is forbidden:
multiple fixed commits can occur between render frames and an edge would be
unrecoverable. The cursor intentionally establishes one baseline before it can
deliver input; the later authored owner must keep controls non-interactive
until that baseline has been observed.

## API under review

```csharp
internal interface IUiSemanticFrameSourceV1 : IInputFrameCommitSource
{
    UiSemanticFrameV1? CurrentUiFrame { get; }
}

internal readonly struct UiSemanticFrameV1
{
    internal InputFrameCommitReceipt Receipt { get; }
    internal int NavigateXQ4096 { get; }
    internal int NavigateYQ4096 { get; }
    internal bool NavigateChanged { get; }
    internal bool HasPoint { get; }
    internal int PointXActualPx { get; }
    internal int PointYActualPx { get; }
    internal bool PointChanged { get; }
    internal bool ClickPressed { get; }
    internal bool HasClickPoint { get; }
    internal int ClickPointXActualPx { get; }
    internal int ClickPointYActualPx { get; }
    internal long ScrollXQ4096 { get; }
    internal long ScrollYQ4096 { get; }
    internal bool SubmitPressed { get; }
    internal bool CancelPressed { get; }
    internal void Validate();
}

internal enum UiSemanticFrameCursorStateV1
{
    AwaitingBaseline = 1,
    Ready = 2,
    Closed = 3,
    Failed = 4
}

internal sealed class UiSemanticFrameCursorV1
{
    internal static UiSemanticFrameCursorV1 Create(
        IUiSemanticFrameSourceV1 source);
    internal UiSemanticFrameCursorStateV1 State { get; }
    internal bool TryAdvance(
        IUiSemanticFrameSourceV1 source,
        out UiSemanticFrameV1 frame);
}
```

The cursor stores the exact source reference. A different source cannot
substitute even if it exposes value-equal receipts and frames. The seam is
read-only and grants no source reset, dequeue, map, action, or menu authority.

## Capture and quantization

- **Navigate:** an accepted `performed` callback reads the processed `Vector2`;
  an accepted `canceled` callback supplies zero. Both components must be
  finite before any mutation. Clamp each component to `[-1,1]`, then round
  `component * 4096` with `MidpointRounding.AwayFromZero`. The current value
  persists only within the same UI mode epoch. `NavigateChanged` is sticky for
  the pending fixed frame when any accepted callback changes the current
  value, even if a later callback restores the frame-start value.
- **Point:** an accepted non-canceled `performed` callback rounds each finite
  component to checked signed `int`, AwayFromZero. Coordinates remain signed
  actual output pixels and are not clamped to the output, gameplay rect, or
  640x360 logical frame. `HasPoint` distinguishes an unobserved point;
  `PointChanged` is sticky under the same rule as Navigate.
- **Click:** coalesce at most one accepted `performed` edge per pending frame;
  the first wins. Read position from the exact bound `Mouse` device's position
  control during that same callback, never from `Mouse.current`, a later Point
  callback, a timestamp, or a synthesized/virtual pointer. Quantize by the
  Point rule, update the current Point with the same sample, and retain an
  immutable click-time copy. `HasClickPoint` is exactly equal to
  `ClickPressed`; a missing, non-finite, or unrepresentable click point faults
  before pending mutation.
- **ScrollWheel:** each accepted non-canceled `performed` callback converts
  each finite component to checked signed-64
  `Round(component * 4096, AwayFromZero)`, then checked-adds it to the pending
  frame aggregate. Opposing samples may net to zero. Scroll is not clamped to
  `[-4096,4096]`.
- **Submit/Cancel:** each accepted `performed` edge coalesces independently to
  one boolean. A later canceled callback does not erase the edge.
- `started`, irrelevant `canceled`, wrong action/map, disabled device, wrong
  mode, and capture-suppressed callbacks make no semantic change. Once a live
  callback identifies the exact expected `UI.Click` action and UI map, its
  control must be that callback device's exact `Mouse.leftButton`; an
  incompatible control or non-`Mouse` device is malformed production binding,
  not an ignorable foreign callback, and faults before mutation. Position is
  read from that same `Mouse.position` control during the callback.

All conversions, callback-ordinal increments, and aggregate additions use
local checked temporaries. NaN, infinity, overflow, malformed expected action
or bound click control latches an exact appended `UiCapture` router fault
before changing pending UI state. The fault preserves the prior global
receipt/frame and all earlier valid pending facts, disables consumption through
the existing monotonic `IsFaulted` boundary, and provides no reset/reopen path.

## Commit and lifecycle contract

1. Before consumer mutation, build a fully validated UI candidate from the
   proposed receipt and frozen UI batch. A candidate exists only when the
   proposed mode is `UIOnly`; entering a new UI epoch produces a deliberately
   empty candidate with no retained point, navigation, edge, or scroll.
2. Existing Movement/Transfer/Combat prepare, validate, commit, and map switch
   remain authoritative and retain their exact order. No UI publication or UI
   pending-batch consumption occurs during those fallible steps.
3. In the existing non-throwing final assignment block, clear only the
   successfully consumed transient UI edge/change/scroll fields, retain
   Navigate and Point current values only for an unchanged UI epoch, set
   `CurrentUiFrame` to the candidate or null for a non-UI receipt, copy the
   exact receipt into an independent private publication proof in every mode,
   then publish that exact `CurrentReceipt` last. No fallible operation or
   callback occurs between these assignments.
4. A preparation/validation rejection preserves consumers, maps, identifiers,
   pending UI facts, `CurrentUiFrame`, and `CurrentReceipt`. Failure after the
   first local consumer commit or during map switching follows M5B5: latch the
   router fault, retain the previous global frame/receipt and unconsumed UI
   batch, and forbid retry or consumption.
5. Successful exit from UI clears all retained and pending UI state and
   publishes null UI frame with the new non-UI receipt. Successful re-entry is
   a new epoch with an empty UI candidate. Initialization starts clear.
   Disable/destroy retains forensic values but `IsFaulted` makes them
   unusable. No lifecycle reset exists.
6. These six actions cease to set `_unboundUi`; the legacy diagnostic field may
   remain for compatibility but must be false for their callbacks.

`CurrentUiFrame` and `CurrentReceipt` getters validate their closed
relationship and the independent exact receipt proof. In a healthy UI
publication the frame, proof, and current receipt are present and exactly
equal; in a healthy non-UI publication the frame is absent while proof and
current receipt remain present and exactly equal; before first publication all
three are absent. Reflected mutation of any receipt field, proof field, frame,
or presence row throws and latches `UiCapture` at the next source getter or
commit boundary. It does not normalize or replace the corrupted forensic
values.

## Cursor and consecutive consumption

`Create` requires a non-null, non-faulted source and validates its current
receipt/frame pair without actions, maps, or menu state. If a coherent current
pair already exists, it is stored as the non-delivered baseline and the cursor
starts `Ready`. If no global receipt exists, the cursor starts
`AwaitingBaseline`; the first coherent publication becomes the non-delivered
baseline. A non-UI receipt at creation produces `Closed`. Any malformed initial
pair fails creation.

`TryAdvance` first validates cursor integrity, exact source identity,
`source.IsFaulted`, and source receipt/frame coherence before changing cursor
state or exposing a frame.

- In `AwaitingBaseline`, no publication returns false unchanged. The first
  coherent UI publication is recorded as baseline, moves to `Ready`, and
  returns false/default. No edge captured before readiness is replayed. If the
  first coherent publication is non-UI with no UI frame, the cursor moves to
  `Closed` and returns false/default.
- In `Ready`, the same exact receipt is a duplicate poll and returns
  false/default without mutation.
- Exact checked `tick + 1`, `frameOrdinal + 1`, unchanged UI mode epoch,
  `UIOnly`, and a frame whose embedded receipt equals the source receipt
  deliver that frame exactly once and advance the cursor.
- The exact next receipt that leaves `UIOnly`, advances the mode epoch by one,
  and exposes no UI frame moves to `Closed` without delivery.
- Foreign/equal-value source, faulted source, missing or mismatched frame,
  lower/replayed receipt, ordinal or tick skip, successor overflow,
  unexplained epoch change, UI frame under non-UI mode, or a call after
  `Closed` latches `Failed` and throws before downstream state can change.

A cursor is only a proof reader; later M5D7P authored-presentation validation
must ensure that exactly one lifecycle owner creates and advances it. That
later owner must not expose interactive controls until the cursor is `Ready`.

## Requirements

- **REQ-M5D7PA-001:** retain the existing `InputRouter` as the sole generated-
  wrapper subscriber and map owner, and expose UI input only as a
  receipt-bound immutable semantic frame.
- **REQ-M5D7PA-002:** publish exact Q4096 Navigate current/change, signed actual-
  pixel Point current/change, first Click plus its same-device callback-time
  point, checked Q4096 Scroll aggregate, and Submit/Cancel edges.
- **REQ-M5D7PA-003:** publish and consume the semantic frame atomically only
  with a successful `UIOnly` global receipt; preserve previous publication and
  pending input on every failure boundary.
- **REQ-M5D7PA-004:** clear semantic state across successful mode-epoch and
  lifecycle boundaries so held/old values and edges cannot cross epochs or
  replay from a faulted source.
- **REQ-M5D7PA-005:** enforce exact source identity, deliberate baseline, and
  one-time consecutive cursor consumption with closed duplicate, skip, replay,
  foreign, mode-exit, and fault behavior.
- **REQ-M5D7PA-006:** reject non-finite values, overflow, malformed
  action/control and reflected immutable/router/cursor corruption without
  normalization or partial pending mutation.
- **REQ-M5D7PA-007:** add no scene, Canvas, EventSystem, uGUI/TMP/package,
  `InputSystemUIInputModule`, virtual-mouse, localization, notification,
  controller, menu-effect, profile, run, gameplay, or persistence authority.
- **REQ-M5D7PA-008:** preserve M5D7O's controller-owned typed notification and
  receipt-correlation boundary; a UI Click/Submit frame is an input fact only
  and cannot itself dismiss, reconstruct, or localize a notification.

## Acceptance criteria

- **AC-M5D7PA-001:** keyboard/gamepad Navigate performed/canceled and multiple
  within-frame changes yield exact clamped Q4096 current values and sticky
  change bits at the next successful same-epoch UI receipt.
- **AC-M5D7PA-002:** negative, off-window, and fractional Point/Click samples
  round exactly; the first click keeps its same-device callback-time point even
  when current Point later changes; two-Mouse coverage proves that a global or
  later pointer cannot substitute; an exact expected Click callback from any
  control other than that device's `Mouse.leftButton` faults before mutation.
- **AC-M5D7PA-003:** multiple signed Scroll samples checked-aggregate exactly;
  Submit/Cancel and performed-then-canceled edges coalesce once.
- **AC-M5D7PA-004:** non-UI modes accept no UI semantics; UI entry publishes an
  empty epoch frame; same-epoch current values persist; successful exit clears
  values and transients; the six callbacks no longer mark unbound UI.
- **AC-M5D7PA-005:** each injected prepare, validate, local-commit, and map-
  switch failure preserves the previous UI frame/receipt and pending batch;
  partial-commit faults cannot be consumed or retried.
- **AC-M5D7PA-006:** existing-pair and absent-pair baseline cases, exact
  consecutive delivery, duplicate poll, clean UI exit, skip, replay,
  foreign/equal-value source, mismatch, source fault, epoch jump, and call
  after close produce the exact cursor outcomes before a consumer effect; a
  first non-UI publication while awaiting baseline closes without delivery.
- **AC-M5D7PA-007:** NaN/infinity, pixel/scroll/callback/receipt-successor
  overflow, malformed click control, and reflection mutation of every
  frame/router/cursor proof field fail closed at the next boundary. This
  includes replacing a non-UI current receipt with a different well-formed
  same-mode receipt: the independent proof detects it, latches `UiCapture`,
  and no new publication is created.
- **AC-M5D7PA-008:** focused M5D7P-A EditMode/PlayMode, direct M5B5 and M5D7O
  regressions, full EditMode, and full PlayMode all finish with
  failure/skip/inconclusive zero; Luna reports `P0=0` and `P1=0`.

### Traceability

| Requirement | Acceptance evidence |
|---|---|
| `REQ-M5D7PA-001` | `AC-M5D7PA-004`, `AC-M5D7PA-008` |
| `REQ-M5D7PA-002` | `AC-M5D7PA-001`, `AC-M5D7PA-002`, `AC-M5D7PA-003` |
| `REQ-M5D7PA-003` | `AC-M5D7PA-005`, `AC-M5D7PA-008` |
| `REQ-M5D7PA-004` | `AC-M5D7PA-004`, `AC-M5D7PA-006` |
| `REQ-M5D7PA-005` | `AC-M5D7PA-006`, `AC-M5D7PA-007` |
| `REQ-M5D7PA-006` | `AC-M5D7PA-005`, `AC-M5D7PA-007` |
| `REQ-M5D7PA-007` | `AC-M5D7PA-004`, `AC-M5D7PA-008` |
| `REQ-M5D7PA-008` | `AC-M5D7PA-008` plus Luna static review |

## Proposed implementation allowlist

- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs`
- new `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef`, limited to adding the one direct `AcadeGameMaker.Core` reference required by frame/cursor tests that name `InputFrameCommitReceipt` and `IInputFrameCommitSource`
- new focused EditMode frame/cursor tests in the existing Input.Unity EditMode
  test assembly and new focused PlayMode router tests in the existing
  Input.Unity PlayMode test assembly, including `.meta` files
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/InputRouterFailureBoundaryPlayModeTests.cs`, limited to updating the five reflection-forged `_currentReceipt` cases to assert the new monotonic `UiCapture` fault and no recovery after a single-field proof mismatch; sink immutability assertions remain mandatory
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/OrdanBossTerminalTransitionRequesterPlayModeTests.cs`, limited to making the intentional coherent overflow fixture set `_uiPublicationProof` to the same receipt whenever its helper sets `_currentReceipt`; the production overflow assertions remain unchanged
- this contract, its Luna pre-gate/implementation/post-review evidence, and
  minimal documentation indexes

`Assets/GameInput.inputactions`, generated `GameInputActions.cs`, all other asmdefs,
M5D7O/M5D7N runtime, scenes, prefabs, `Packages/manifest.json`,
`Packages/packages-lock.json`, ProjectSettings, UI assets,
profile/persistence/run/gameplay/narrative sources, and unrelated files are
outside the allowlist.

## Stop and rollback conditions

Stop before approval or implementation if the solution needs a second callback
subscriber, action/map authority outside `InputRouter`, a queue to conceal
missed owner ticks, render-`Update`-only consumption,
`InputSystemUIInputModule`, virtual mouse, `Mouse.current`, timestamp ordering,
scene/package/UI changes, safe-frame hit testing, notification prose or
correlation reconstruction, a menu effect, or a new user product decision.
For this stop condition, "callback subscriber" means an additional generated
action-map callback consumer or semantic input-capture owner. A one-shot
`InputSystem.onAfterUpdate` lifecycle hook owned by the same `InputRouter` is
permitted solely to release the UI-map enable quarantine after that input
update; it must not read, stage, publish, or consume any semantic input and
must be removed on release, disable, destroy, close, or fault.
Stop on any inability to preserve M5B5's prior receipt/batch under injected
failure, or if direct M5B5/M5D7O regression changes.

Rollback removes the new value/cursor and focused tests and reverts only the
bounded router/diagnostic additions and documentation entries. It must not
rewrite the input asset, generated wrapper, M5D7O/M5D7N, or historical
evidence.

## Review record and open gate

- Sol identified two P0 design risks and closed them in this Review contract:
  latest-only render consumption could lose edges, and a second UI input owner
  would conflict with the verified adopted-action boundary.
- Astra retained Sol's exact-device click rule, mode-epoch clearing, checked
  scroll aggregation, and fail-closed receipt binding. Astra additionally
  made baseline readiness observable and explicitly forbade later interactive
  controls before that readiness.
- Luna's first pre-gate reported `P0=0`, `P1=2`, `P2=2`. Astra added an
  independent exact receipt proof for every publication mode, made the exact
  expected Click callback require that same device's `Mouse.leftButton`, added
  two-Mouse/malformed-control tests, and closed the previously unspecified
  first non-UI publication while awaiting baseline. These amendments require a
  narrow independent re-review before approval.
- Implementation compile preflight showed that the existing M5D7O EditMode
  assembly references `AcadeGameMaker.Input.Unity` but not the Core assembly
  that declares the semantic frame's receipt/interface types. Astra amended
  the allowlist to permit exactly one direct `AcadeGameMaker.Core` reference in
  that existing test asmdef. No production dependency, assembly split, or
  runtime authority changes; the PlayMode test assembly already has the exact
  Core reference. Luna confirmed that this is the minimal safe test-only edit
  for the typed test source and does not expand production authority. Terra
  must retain/restore the strongly typed tests if it applies the reference;
  the temporary reflection-only workaround is not the accepted final source.
- The first final-source full PlayMode run exposed six legacy reflection-fixture
  conflicts. Five M5B5 mismatch cases changed only `_currentReceipt` and then
  expected to restore that field and resume; the approved independent proof
  now correctly turns that single-field forgery into a monotonic `UiCapture`
  fault. Their minimum valid update is to retain all sink no-mutation checks,
  assert the fault/proof failure, and forbid recovery. One terminal-overflow
  fixture intentionally constructs a coherent overflow receipt row; its helper
  must now update the exact publication proof together with the current
  receipt so it continues testing overflow rather than accidental corruption.
  Astra added only those two exact test files and those exact expectation/helper
  edits to the allowlist; the focused AC007 single-field-forgery test remains
  the independent proof that mismatched receipt mutation fails closed. Luna
  must confirm this regression-fixture amendment before the edits are applied.
- Residual P2 for the later authored presenter: exact safe-frame hit testing,
  focus/hover precedence, cursor rendering, one-owner authoring validation,
  and Korean-capable font assets remain outside this seam.

No user product decision is required. Luna closed both initial P1 findings and
the first-non-UI baseline ambiguity; Astra approved this bounded input seam on
2026-09-20. Terra may implement only the allowlist. Luna must independently
verify the implementation and all required regression runs before Astra may
mark it `Verified`.
