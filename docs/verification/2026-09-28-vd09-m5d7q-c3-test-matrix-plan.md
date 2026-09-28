# VD-09 M5D7Q-C3 test matrix plan

Date: 2026-09-28  
Status: planning only — no implementation, execution, or acceptance claim  
Inputs: Approved C3 contract and Sol's interface/allocation review.

## Test allocation and fixtures

Keep C1 leaf reads and the observation proof in Profile/Input.Unity tests; test
the synthetic owner, Q-A/Q-B epoch handshake, and cursor quarantine in
`AcadeGameMaker.Hub.Presentation.Unity` tests. Proposed namespaces/fixtures:

- `AcadeGameMaker.Input.Unity.EditMode.Tests` (existing InputUnity convention): `ProfileNewGameConfirmationV1Tests`;
  same-lease capture classification, authenticated adapter-root boundary, closed
  result/value validation, and C1 control checkpoints.
- `AcadeGameMaker.Tests.EditMode.HubPresentation` (existing HubPresentation convention): 
  `NewGameConfirmationOwnerV1Tests`; intake, private capability/CAS, confirm,
  cancel, retry, execution-commit, and Q-B actual-take issuance.
- `AcadeGameMaker.Tests.PlayMode.HubPresentation`:
  `NewGameConfirmationRearmPlayModeTests`; Q-A/Q-B successor agreement,
  baseline-frame discard, callbacks and disable/destroy containment. These are
  synthetic owners only: no prefab, scene, map enable, C2, or C1 Begin.

Every fixture uses exact validated real objects plus deterministic throw-only
controls at the contract's named boundaries. It must not substitute an identity,
leaf classification, request, frame, or result. Each negative row snapshots
profile bytes, actions/maps, receipts, notification, scene/run state, and
historic Q-A/Q-B entries before/after.

## C1 observation/classification matrix

For each role `Primary`, `Previous`, `Temp`, test Missing, valid exact-default,
ValidInputRecoveryRequired, Invalid, Unsupported, and Unreadable. Cover the
three-leaf cross-product by a compact presence/classification matrix: (1) all
missing; (2) Primary-default/others-missing; (3) every role singly present
default; (4) Primary-default plus Previous or Temp present; (5) one unreadable
role with the other two each Missing/default/meaningful; (6) invalid,
unsupported, and input-recovery-required separately in every role; (7) every
role addition/removal/change between display and Confirm. Assert precedence:
Unreadable > meaningful-valid > ambiguous-present > exact-default.

For every `ValidCurrentInput` role, run one row per product field below against
the planner's exact default document; the profile revision is deliberately not a
meaningful field, but is changed in a separate fresh-identity row.

| Group | Single non-default fields |
| --- | --- |
| Settings | WindowMode; MasterVolumeQ1000; MusicVolumeQ1000; SfxVolumeQ1000; GamepadAimInvertX; GamepadAimInvertY |
| Input (current-compatible) | BindingOverridesJson (non-empty canonical override) |
| Tutorial | ConfirmedIds (one canonical ID) |
| Progression | LastOfferedSeed; ConsentState; CompletedBranches; committed choice/skill pair |

`ActionsAssetId` and `BindingSchemaVersion` mismatch rows are **not**
ValidCurrentInput product mutations: exercise each as
`ValidInputMetadataRecoveryRequired` and therefore ambiguous. Malformed or
non-canonical binding override is likewise an input-recovery/ambiguous row, not
a meaningful valid-document row. The valid progression fixtures must respect
the closed choice/skill invariant: LastOfferedSeed-only, ConsentState-only, and
CompletedBranches-only can differ from default; CommittedChoice and GrantedSkill
must be one correlated exact pair fixture (Extraction/CompressionVerdict or
Solidarity/CommonReferencePlane), not impossible literal single-field rows.
Each valid-product fixture must classify `ConfirmationRequiredMeaningful`
regardless of role or selected source. A revision-only document is
`NoConfirmationRequired` but still must produce a distinct fresh identity.
Previous/Temp presence is ambiguous even when their bytes are exact default.
Confirm recapture repeats the role/field matrix: equal identity -> Confirmed;
changed meaningful or ambiguous -> FreshDecisionRequired; changed exact-default
-> fresh Confirmed; Busy -> same pending capability; unreadable -> terminal no
request.

## Lifecycle, history, and concurrency matrix

- Intake: exact NewGame succeeds once; non-NewGame/default/duplicate/taken,
  foreign receipt/presenter/Q-B owner/adapter/router/root, disabled, terminal
  reset, cross-epoch, and reflected rows reject without mutation. Include the
  private Q-B issuance record: copied/equal request values never authenticate;
  issuance occurs only at the actual take and is single-use.
- Busy: initial `CaptureBusy` retains exactly one Q-B issuance; retry performs
  a new capture without take/rearm. Concurrent/reentrant retry rejects. Busy at
  Confirm restores only the original pending capability, never a new one.
- Decision CAS: default/copied/foreign/old-generation capabilities; two
  Confirm threads; Confirm-versus-Cancel; callback after ConfirmInspecting;
  late callback; and disable/destroy. Exactly one winner, monotonically closed
  history, and no public result exposes identity/document on a failure.
- Cancel/rearm: byte/memory/action/map/receipt/scene snapshots unchanged;
  Q-B old `_takenRequest` and Q-A old intent/cursor remain immutable. Assert
  generation strictly increases, one fresh NewGame selection only, preserved
  notice state, and a fresh cursor's first consecutive frame is validated and
  discarded (no Interpret/Activate). False advance stays quarantined; skipped,
  malformed, foreign, partial-successor, lifecycle/reflection faults close.
- Execution boundary: before C1 Begin, consume confirmed request and invalidate
  Confirm/Cancel/retry/rearm. `ConfirmationStale` records consumed history then
  requires a new epoch; Busy is typed pre-barrier only; Reload/Manual and all
  possible post-barrier rows forbid old-menu callbacks and rearm.

## AC evidence map

| AC | Focused proof |
| --- | --- |
| AC-001 | exact intake/topology/epoch/Q-B private issuance rejection matrix |
| AC-002 | three-leaf × product-field matrix and precedence |
| AC-003 | golden Korean copy/labels plus Confirm/Cancel capability CAS |
| AC-004 | display-to-confirm full-identity mutation matrix |
| AC-005 | cancel neutrality and immutable Q-A/Q-B history/new epoch |
| AC-006 | cursor baseline discard, monotonic epochs, fault closure |
| AC-007 | Busy/Stale vs terminal post-barrier execution report matrix |
| AC-008 | execution-commit boundary fault points and authority invalidation |
| AC-009 | static forbidden-authority/API scan and synthetic-only assertions |
| AC-010 | focused + required suite run ledger; Luna independent P0/P1 review |

Stop for Astra if any test requires a second classification read, C1 durable
algorithm change, mutable/copy-value request issuance, history reuse, map enable,
public ABI/friend/asmdef, or live UI assets. This plan does not mark C3 or C2
Verified.
