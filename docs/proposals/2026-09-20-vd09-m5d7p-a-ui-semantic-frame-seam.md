# VD-09 M5D7P-A UI semantic frame seam — Sol contract proposal

- Status: Draft; not implementation authority
- Date: 2026-09-20
- Design/counter-review: Sol (`gpt-5.6-sol`)
- Final contract owner and approval authority: Astra
- Intended implementation/review after approval: Terra / Luna
- Parents: Verified M5B5, Verified M5D7N, Verified M5D7O, `REQ-UX-004`, `REQ-UX-006`, `REQ-UX-009`

## Bounded outcome

Extend the one existing `InputRouter` so the six UI actions it already owns in
`UIOnly` publish one immutable semantic frame tied to the exact successful
`InputFrameCommitReceipt`. Do not add a second action subscriber, lightweight
router, `InputSystemUIInputModule`, virtual mouse, action-map owner, scene,
Canvas, uGUI/TMP dependency, menu effect, or notification reconstruction.

The publisher remains latest-value only. To prevent a render-rate `Update`
consumer from silently skipping fixed frames, this unit also defines a small
engine-free identity-bound cursor. The future M5D7P presentation owner must call
that cursor once per fixed step after the router (router is `-210`; the future
owner must use an exact later order). The first live UI publication is a
baseline and is not replayed as input; only exact consecutive publications are
delivered once.

## Proposed API

```csharp
internal interface IUiSemanticFrameSourceV1 : IInputFrameCommitSource
{
    UiSemanticFrameV1? CurrentUiFrame { get; }
}

internal readonly struct UiSemanticFrameV1
{
    internal InputFrameCommitReceipt Receipt { get; } // Mode == UIOnly
    internal int NavigateXQ4096 { get; }               // -4096..4096
    internal int NavigateYQ4096 { get; }
    internal bool NavigateChanged { get; }
    internal bool HasPoint { get; }
    internal int PointXActualPx { get; }                // signed, unclamped
    internal int PointYActualPx { get; }
    internal bool PointChanged { get; }
    internal bool ClickPressed { get; }
    internal bool HasClickPoint { get; }                // == ClickPressed
    internal int ClickPointXActualPx { get; }           // signed, unclamped
    internal int ClickPointYActualPx { get; }
    internal long ScrollXQ4096 { get; }                 // frame aggregate
    internal long ScrollYQ4096 { get; }
    internal bool SubmitPressed { get; }
    internal bool CancelPressed { get; }
    internal void Validate();
}

internal enum UiSemanticFrameCursorStateV1
{
    AwaitingBaseline = 1, Ready = 2, Closed = 3, Failed = 4
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

The cursor stores the exact source reference. Value-equal receipts from a
different object never substitute for source identity.

## Capture and quantization

- `Navigate`: on accepted `performed`, read the processed `Vector2`; on
  accepted `canceled`, use zero. Reject either non-finite component before any
  mutation. Clamp each finite component to `[-1,1]`, then
  `Round(component * 4096, AwayFromZero)`. The current value persists only in
  the same UI mode epoch. `NavigateChanged` is sticky for the pending frame if
  any accepted callback changes the current value, even if later callbacks
  return it to the frame-start value.
- `Point`: on accepted non-canceled `performed`, round each finite component to
  a checked signed `int` with `AwayFromZero`; do not clamp to the gameplay rect,
  640x360 frame, or output bounds. `HasPoint` distinguishes the unobserved state.
  `PointChanged` is sticky under the same rule as navigation.
- `Click`: coalesce at most one performed edge per frame. The first accepted
  click wins and cannot be replaced. Capture the exact bound `Mouse` device's
  `position` control in that callback, quantize it by the Point rule, and update
  the current Point with the same sample before recording the click-time copy.
  Do not use `Mouse.current`, timestamps, a synthesized point, or a virtual
  mouse. A click without a finite representable point faults before mutation.
- `ScrollWheel`: for every accepted performed callback, convert each finite
  component to `Round(component * 4096, AwayFromZero)` as checked `long`, then
  checked-add it to the pending frame aggregate. Opposing deltas may net to
  zero. No arbitrary `[-4096,4096]` clamp is applied to scroll magnitude.
- `Submit` and `Cancel`: coalesce performed edges independently to one boolean
  each. Performed then canceled in one interval preserves the edge.
- `started`, irrelevant `canceled`, wrong action/map/device, disabled device,
  wrong mode, and capture-suppressed callbacks make no semantic change.

All conversion and addition uses local temporaries. NaN, infinity, checked
pixel/scroll/callback overflow, or a malformed bound click control latches a
new exact UI-capture router fault before changing any pending UI field. It
preserves the prior receipt/frame and the already valid pending batch; the
faulted source is never consumable.

## Commit and lifecycle semantics

1. Before consumer mutation, build a candidate `UiSemanticFrameV1` from the
   proposed receipt and frozen UI batch. It exists only when the proposed mode
   is `UIOnly`; entering a new UI epoch uses an empty/zero, no-point candidate.
2. Existing Movement/Transfer/Combat prepare, validate, commit and map switch
   remain authoritative. No UI publication or pending consumption occurs yet.
3. In the existing non-throwing final assignment block, clear the successfully
   consumed UI transient flags/scroll, retain Navigate/Point current values only
   for the same UI epoch, set `CurrentUiFrame` to the candidate (or null for a
   non-UI mode), and assign the exact `CurrentReceipt` last. Every externally
   valid UI frame therefore carries a receipt equal to the source's current
   receipt.
4. Any pre-commit rejection preserves consumers, maps, IDs, pending UI batch,
   `CurrentUiFrame`, and `CurrentReceipt`. Any post-local-commit/map failure
   follows M5B5: latch fault, retain the previous global frame/receipt and
   unconsumed batch, and permit no later UI consumption or retry.
5. Successful mode exit clears all retained/pending UI state and publishes no
   UI frame for the new non-UI receipt. Successful re-entry begins a new empty
   epoch. Initialization begins clear. There is no reset/reopen API. Faulted
   disable/destroy preserves forensic state as required by M5B5, but the source
   is unusable because `IsFaulted` is true.
6. The former `_unboundUi` diagnostic becomes false for these six now-bound
   actions; it does not remain a parallel input channel.

## Consecutive owner consumption

`UiSemanticFrameCursorV1.Create` binds one exact source without reading actions
or maps. `TryAdvance` checks source identity and `IsFaulted` before any cursor or
menu mutation.

- Before baseline, absent receipt/frame returns false. The first exact
  `UIOnly` receipt/frame pair is recorded as baseline and returns false, so an
  edge captured before owner readiness is never replayed.
- The same exact receipt after baseline is a harmless duplicate poll and
  returns false.
- Exact `tick + 1`, `frameOrdinal + 1`, unchanged UI mode epoch, `UIOnly`, and
  equal frame/source receipt delivers the frame once and advances the cursor.
- An exact next receipt that leaves `UIOnly`, increments mode epoch once, and
  has no UI frame closes the cursor without delivering a frame.
- A foreign source, faulted source, missing/mismatched frame, lower/replayed
  ordinal, ordinal or tick skip, overflow successor, unexplained mode-epoch
  change, UI frame in a non-UI mode, or reopening after `Closed` latches cursor
  `Failed` and throws before downstream state changes.

The future presentation owner must perform this advance every fixed step after
the router. Reading only in render `Update` is a stop condition because several
fixed commits can occur between render frames and a latest-only source cannot
recover a skipped edge.

## Requirements

- **REQ-M5D7PA-001:** retain the existing `InputRouter` as the sole generated-
  wrapper and map owner and expose UI input only as a receipt-bound immutable
  semantic frame.
- **REQ-M5D7PA-002:** publish exact Q4096 Navigate current/change, signed actual-
  pixel Point current/change, first Click plus its callback-time point, checked
  Q4096 Scroll aggregate, and Submit/Cancel edges.
- **REQ-M5D7PA-003:** publish/consume the semantic frame atomically with only a
  successful `UIOnly` `InputFrameCommitReceipt`; preserve old publication and
  pending input on failure.
- **REQ-M5D7PA-004:** clear semantic state on successful mode-epoch/lifecycle
  boundaries without allowing held/old edges to cross epochs or faulted state
  to replay.
- **REQ-M5D7PA-005:** enforce exact source identity and one-time consecutive
  cursor consumption with baseline, duplicate, skip, replay, foreign, mode-exit
  and fault behavior closed above.
- **REQ-M5D7PA-006:** reject non-finite, overflow, malformed action/control and
  reflected immutable/cursor corruption without normalization or partial
  pending mutation.
- **REQ-M5D7PA-007:** add no scene, Canvas, EventSystem, uGUI/TMP/package,
  `InputSystemUIInputModule`, virtual-mouse, localization, notification,
  controller, menu-effect, profile, run, gameplay or persistence authority.
- **REQ-M5D7PA-008:** preserve M5D7O's controller-owned typed notification and
  correlation boundary; a UI Click/Submit frame is only an input fact and does
  not itself dismiss, reconstruct or localize a notification.

## Acceptance criteria

- **AC-M5D7PA-001:** keyboard/gamepad Navigate performed/canceled and multiple
  within-frame changes yield the exact clamped Q4096 current value and sticky
  change bit at the next successful UI receipt.
- **AC-M5D7PA-002:** negative/off-window/fractional Point and Click samples round
  exactly; click retains the first edge and its same-device callback-time point
  even if the current point later changes.
- **AC-M5D7PA-003:** multiple positive/negative Scroll samples checked-aggregate
  exactly; Submit/Cancel and performed-then-canceled edges coalesce once.
- **AC-M5D7PA-004:** gameplay/locked modes accept no UI semantics; entry publishes
  an empty epoch frame, same-epoch current values persist, successful exit clears
  them, and no virtual mouse or second map owner exists.
- **AC-M5D7PA-005:** every injected prepare/validate/commit/map failure preserves
  previous UI frame/receipt and pending batch; partial-commit faults cannot be
  consumed or retried.
- **AC-M5D7PA-006:** baseline and exact consecutive delivery work once; duplicate
  polls are false/no-op; skip, replay, foreign/equal-value source, mismatched
  receipt, fault, epoch jump and reopen fail before a consumer effect.
- **AC-M5D7PA-007:** NaN/infinity, pixel/scroll/ordinal overflow and reflection
  mutation of every frame/cursor proof field fail closed at the next boundary.
- **AC-M5D7PA-008:** M5B5 direct regressions, focused M5D7P-A PlayMode/EditMode,
  M5D7O direct regression, and full EditMode/PlayMode finish with
  failure/skip/inconclusive zero; Luna reports P0=0/P1=0.

## Proposed allowlist

- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs`
- new `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs` and `.meta`
- new focused EditMode cursor/value tests in the existing Input.Unity EditMode
  test assembly, and new focused PlayMode router tests in the existing
  Input.Unity PlayMode test assembly, with `.meta` files
- this eventual work contract, pre-gate/evidence/post-review, and minimal indexes

`GameInput.inputactions`, generated `GameInputActions.cs`, asmdefs, M5D7O/M5D7N
runtime, scenes, prefabs, packages/manifest/lock, ProjectSettings, UI assets,
profile/persistence/run/gameplay/narrative sources, and unrelated files are not
in the allowlist.

## Stop and rollback

Stop before approval if the implementation needs a second callback subscriber,
action/map enable authority outside `InputRouter`, a queue to hide missed owner
ticks, render-`Update`-only consumption, `InputSystemUIInputModule`, virtual
mouse, `Mouse.current`, frame timestamps, scene/package/UI changes, safe-frame
hit testing, notification prose/correlation reconstruction, or a menu effect.
Stop on any inability to preserve M5B5's old receipt/batch under injected
failure, or if M5D7O/M5D7N direct regression changes.

Rollback removes the new value/cursor and focused tests, and reverts only the
bounded router/diagnostic additions and documentation entries. It must not
rewrite the input asset, generated wrapper, M5D7O/M5D7N, or historical evidence.

## Counter-review findings

- **P0 resolved by contract:** a latest-only source plus render `Update` consumer
  can skip fixed frames and lose edges. Exact post-router fixed-step cursor
  consumption is mandatory.
- **P0 resolved by contract:** `InputSystemUIInputModule` or a second subscriber
  would compete with the verified router for adopted actions and maps.
- **P1 resolved by contract:** click position comes from the exact bound mouse
  device at the click callback, not from a later Point or global mouse.
- **P1 resolved by contract:** UI entry/exit is an epoch boundary; no old current
  value, edge or scroll crosses it.
- **P1 resolved by contract:** scroll is an unclamped checked Q4096 aggregate;
  Navigate alone uses normalized `[-4096,4096]` components.
- **Residual P2:** the future authored presenter still must define safe-frame
  hit testing, focus movement, hover/focus precedence and cursor rendering. None
  belongs in this seam.
- **Residual P2:** the legacy diagnostic name `UnboundUi` may remain for ABI/test
  stability but must always be false for the six now-bound actions.

No user product decision is required for this technical prerequisite. This
proposal is not Approved; Astra must decide the final API, IDs, allowlist and
status before Terra may implement it.
