# C3 fixture-composition plan

Date: 2026-09-28  
Status: planning only — no C3 implementation, execution, or acceptance claim

This note implements no contract behavior. It narrows the synthetic C3 test
setup described by the Approved C3 contract and the C3 matrix.

## Existing boundaries to retain

- `HubLaunchMatrixContext.Create(HubLaunchScenario)` in
  `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0HandoffMatrixPlayModeTests.cs`
  is the closest real launch-cohort recipe: inactive host, real
  `InputRouter`/`DesktopProfileLaunchAdapterV1`/`HubEntryHandoffLatchV1`, the
  existing authoring seams, `ConfigureForTests`, then Adapter `Start`, Latch
  `Start`/`Update`. Its `CurrentHandoff` is issued by the real latch.
- `HubRuntimeContext.Create()` in
  `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs`
  shows the compact real router baseline: adapter `Start` followed by
  `Router.StepForTests()` yields the real `CurrentReceipt` and
  `CurrentUiFrame` used by `UiSemanticFrameCursorV1`.
- `HubMenuPresenterV1.Update`, `FixedUpdate`, and
  `HubMenuIntentHandoffOwnerV1.LateUpdate` are the only Q-A/Q-B path to use:
  presenter consumes the latch handoff, creates its real cursor, retains the
  intent; Q-B performs the actual `TryTakeRequest` transition. The compact
  `HubMenuIntentHandoffEditModeTests.Fixture.Create` is deliberately *not*
  reusable for C3 acceptance rows because it reflects a handoff/intent state
  instead of issuing the actual Q-A/Q-B transition.

Those helpers are internal to their current test assemblies. The C3 Hub
Presentation tests have no reference to an Input.Unity *test* assembly, and
the Approved allowlist forbids a new friend or asmdef. They therefore cannot
be imported as a shared helper. They are implementation templates only.

## Proposed local composition

Each new C3 fixture file should contain one small disposable
`C3HubCohortFixture`, local to its assembly, rather than reconstructing a
handoff receipt or cursor in individual cases:

1. Create one unique owned temporary root and an inactive runtime host with
   the real router, adapter, and latch; apply the existing authoring seams and
   `DesktopProfileLaunchPreparationPortV1` exactly as the two Q0 contexts do.
2. Compose only real synthetic runtime objects for the Q-A presenter and Q-B
   owner, using the existing serialized authoring-seam topology and its normal
   component/configuration relationships as the provenance template. “Existing
   topology” does **not** mean loading, creating, or changing a Unity scene or
   prefab, nor wiring live UI. Do not build a partial presenter with reflected
   receipt, cursor, controller, intent, or state fields. Configure the
   in-memory authored-seam equivalents, run the actual adapter/latch lifecycle,
   then run the normal presenter update and Q-B late update until an actual
   `NewGame` request is taken. The request, receipt, frame, and cursor must all
   be produced by those real synthetic objects; none may be fabricated or
   copied into fields.
3. Expose only the verified objects and observations needed by a row:
   adapter, router, latch handoff, presenter, Q-B owner, actual taken request,
   current receipt/frame, and owned-root cleanup. The fixture should have one
   `AdvanceRealBaseline()` that calls the real router step and the presenter
   fixed-update path; it returns no fabricated frame.

The C3 owner/EditMode rows may share a fixture only within one uninterrupted
owner lifecycle where the contract specifically permits retry/Confirm/Cancel
sequencing. Every independent intake, foreign/cohort, capture classification,
or fault row starts a new fixture. PlayMode rearm rows retain the old fixture
history, create the successor through the real Q-A/Q-B rearm boundary, then
use `AdvanceRealBaseline()` exactly once for the required discarded frame and
again for the first deliverable successor frame. This avoids repeated deep
receipt construction while preserving a new real cohort for each terminal row.

## Constraints for implementation tests

- The fixture must not substitute a C1 identity, three-leaf classification,
  profile projection, confirmed request, semantic frame, cursor result, or
  Q-B request. Named controls may only throw at Approved checkpoints.
- Do not call `HubMenuIntentHandoffEditModeTests.ValidReceipt`, its reflected
  `Fixture.Create`, `UiSemanticFrameV1Tests.Source`, or field writes to make
  an otherwise valid C3 state. Those are valuable legacy unit probes, not real
  C3 provenance.
- Preserve and compare the original launch/handoff receipt, router
  receipt/frame, maps/actions, and Q-A/Q-B historical rows before and after
  every negative/cancel/rearm row. C3 never retakes launch preparation or
  notification, replaces actions/router, or enables a map.
- The fixture must not invoke C1 Begin, C2, barrier removal, destination,
  scene/gameplay, Settings, Quit, or live confirmation UI wiring. It is only a
  synthetic owner/cohort harness.

## Blocker check

No contract blocker was found. The only composition constraint is assembly
visibility: duplicate the small *real lifecycle composition* locally in each
new C3 test assembly, rather than sharing Q0 test helpers through an
unauthorized test-assembly reference.
