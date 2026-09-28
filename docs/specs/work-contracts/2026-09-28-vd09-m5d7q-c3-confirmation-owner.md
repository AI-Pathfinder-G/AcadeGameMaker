---
status: Approved
---

# VD-09 M5D7Q-C3 New Game confirmation owner and fresh interaction epoch

- Date: 2026-09-28
- Status: Approved — Astra 2026-09-28; bounded synthetic-only implementation
- Owner and approval: Astra
- Bounded design: Sol; intended implementation: Terra; independent QA: Luna
- Parent: `2026-09-28-vd09-m5d7q-c-new-game-reset.md` (Approved)
- Dependencies: M5D7Q-A and M5D7Q-B Verified; C1 Approved/verified before
  execution integration; C2 Approved but not a destination or interactive hub
- Parent trace: `REQ-M5D7QC-001/002/003/007`,
  `AC-M5D7QC-001/002/006/007`

## Purpose and bounded result

C3 owns one exact Q-B `NewGame` request, the decision whether confirmation is
required, one confirmation/cancellation decision, and the freshness boundary
immediately before C1 Begin. It prevents a retained or taken old request, late
button callback, or stale profile observation from authorizing reset.

C3 is synthetic-only until a later Approved live confirmation-presentation
contract authors the UI graph and binds its controls. This contract defines
typed copy and owner behavior; it creates no prefab, scene, dialog artwork or
live menu wiring. It does not execute C2, accept its receipt, enable any map,
return from `UIOnlyBlocked`, enter a destination, start gameplay, open wardrobe
or Settings, execute Quit, or change costume/media storage.

The successful C3 boundary is one owner-authenticated, single-use
`ConfirmedProfileResetRequestV1` carrying the exact fresh C1 confirmation
identity. A later Approved execution integration must first terminally consume
that request and invalidate every confirm/cancel handle, then invoke C1 Begin.
C3 alone never claims that New Game started or that reset completed.

## Existing behavior preserved

Q-A's `IntentRetained` and Q-B's `RequestTaken` remain immutable history. They
are never cleared, rewritten, or moved backward. Cancellation and stale
re-gating do not unlock those instances. Instead, the exact existing presenter,
handoff owner, confirmation owner and router may create one strictly increasing
interaction epoch. The new epoch owns a fresh controller/request slot and can
accept a new menu selection; the old epoch remains permanently consumed.

This is a narrow New Game decision rearm, not a general hub hot reload. It does
not re-run launch preparation, retake the launch handoff or notification,
replace the router/actions, enable a map, recreate a scene, or reissue an
old request. Continue, Settings and Quit execution remain out of scope.
Only the read-only presentation cursor may be replaced by a fresh cursor using
its existing factory; the old cursor remains forensic history. No cursor/source
API or semantic publication is changed.

## Entry, topology and root identity

The synthetic request/decision coordinator entry and Unity owner are internal
to `AcadeGameMaker.Hub.Presentation.Unity`:

```csharp
NewGameConfirmationStartResultV1 AcceptNewGame(
    HubMenuIntentRequestV1 request,
    DesktopProfileLaunchAdapterV1 sessionOwner,
    InputRouter router,
    NewGameConfirmationOwnerV1 owner);
```

`AcadeGameMaker.Input.Unity` provides only a lower-level profile observation
and confirmed-identity value boundary taking its own launch adapter/router
types and opaque reference-identity owner/epoch tokens. It cannot reference
`HubMenuIntentRequestV1`, a presenter or any Hub.Presentation type. The Hub
owner validates request/presentation topology before calling that boundary.
The existing assembly direction remains Hub.Presentation -> Input.Unity ->
Profile, with no reverse reference or new friend assembly.
The allowlisted launch adapter may expose one internal read-only
`GetAuthenticatedLaunchRootForNewGame(InputRouter)` boundary. It validates the
exact active cohort and both normalized root witnesses before returning the
original root; it creates no directory, lock, observation, receipt or session
state. Eligibility booleans alone cannot provide a caller-free root. Private
field reflection and a fresh environment path lookup are not production APIs.

The request must be exact, unconsumed by C3, and `NewGame`; its handoff receipt
must equal the presenter's immutable launch/handoff history. The owner,
presenter, Q-B handoff owner, launch adapter and router must be the exact active
authored cohort for one interaction epoch. The adapter's launch-established
normalized profile-root witnesses must agree, and that exact root is the only
root C3 may inspect or place in a confirmed request. No caller-supplied root is
accepted.

The existing request is a value row, not an issuer-authenticated epoch token.
C3 intake therefore also requires a private one-shot Q-B issuance capability
bound to the retained exact request, actual handoff owner, presenter and epoch.
Matching Item/Receipt values alone cannot authenticate a new epoch. This is
additive private evidence in the allowlisted handoff-owner source; historical
request values and historical launch/handoff receipts are not rewritten.

Foreign, duplicate, default, unknown, reflected-corrupt, disabled, closed,
terminal-reset, C2-gated or cross-epoch rows fail closed. A clean protocol
rejection has no profile/map mutation. Reflected lifecycle corruption may use
the existing terminal containment and is never presented as a cancellation,
Busy, or successful confirmation.

## Fresh three-leaf observation and decision

C3 calls C1's observation-only `CaptureConfirmationIdentity` against the exact
launch root. The capture uses the actual profile root lease and barrier check
and freshly observes Primary, Previous and Temp. C3 never reconstructs an
identity from launch receipt fields, timestamps, filenames or UI values.

One narrow C1-owned observation API captures the identity and decoded immutable
leaf projections from the same reads under the same actual root lease. Its
projections bind each role, raw-byte length/hash, classification and decoded
document; it validates projection/identity agreement at every getter. A
separate read outside that lease is not classification evidence. C3 compares
these projections, not revision/hash guesses or a second filename read.

Every present readable valid candidate is compared field-for-field with
`ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument`, including
settings, empty binding override, tutorial and all progression fields except
the bookkeeping profile revision. Revision remains part of the fresh identity
comparison, but never alone establishes meaningful progress. Classification is:

- `NoConfirmationRequired` only when Primary is valid and all product fields
  equal the approved default and Previous and Temp are absent, or when all
  three are absent. A revision-only difference still passes through C1; it is
  not permission for a durable no-op or a non-r0 final profile;
- `ConfirmationRequiredMeaningful` when any valid candidate in any role differs
  from any approved default product field, regardless of selected source or
  revision;
- `ConfirmationRequiredAmbiguous` when any present readable leaf is invalid,
  unsupported, or input-recovery-required, or when Previous/Temp is present
  even if its bytes equal the default;
- `CaptureBusy` when lease acquisition times out without observation/mutation;
- `CaptureUnreadable` when any role, root, barrier or safe-path fact cannot be
  authoritatively read. This is terminal for the current epoch and is never
  converted to “no progress,” default, or a confirmation prompt.

Decision precedence is CaptureUnreadable, meaningful, ambiguous, then
NoConfirmationRequired. Both meaningful and ambiguous require the same prompt;
precedence never silently discards an ambiguous leaf. The private observation
evidence retains all three classifications for freshness validation.

The captured identity is immutable, defensively evidenced, bound to the root
and epoch, and hidden from UI callbacks. Exact-default/no-leaf detection does
not skip C1: confirmation may be bypassed, but the same fresh identity must be
terminally handed to the execution boundary so C1 decides the durable result.

If initial capture is Busy, C3 retains sole ownership of the already-taken
exact Q-B request in this same epoch. An explicit intake retry performs a new
capture without another Q-B take, request recreation or menu rearm. A concurrent
or reentrant retry is rejected without changing the retained request. Teardown
or CaptureUnreadable closes it; neither is reported as Cancel.

## Exact user-facing decision copy

When either confirmation-required classification is reached, the future live
presentation must display this exact Korean copy:

> 새 게임을 시작하면 현재 단일 프로필의 진행 상황, 설정, 입력 설정,
> 튜토리얼 확인 상태가 초기화됩니다. 기존 프로필 파일은 자동 복구에
> 사용되지 않는 수동 복구용 자료로 계속 보관됩니다. 의상 및 외형 자료는
> 별도 저장 영역이므로 변경되지 않습니다. 계속하시겠습니까?

Exact action labels are `초기화하고 새 게임` and `취소`. The copy does not
promise security deletion, archive cleanup, automatic restoration, immediate
gameplay, a scene transition, or a successful reset. Costume/appearance is
stated unchanged because the Approved parent excludes costume/media storage
from the single-profile transaction; changing that product boundary requires a
new user decision and parent amendment.

For `NoConfirmationRequired`, no destructive-confirmation UI is required, but
the coordinator moves directly to `ConfirmedReady` and returns outcome
`Confirmed` with classification evidence `NoConfirmationRequired` and the same
single-use confirmed request. Classification and decision outcome are distinct
closed fields; the classifier alone never carries a request. This does not call
C1 or report reset completion.

## Decision capability and one-shot callbacks

The prompt publication returns one private `NewGameDecisionCapabilityV1` bound
by reference identity to owner, presenter, Q-B owner, adapter, router, root,
interaction epoch, exact request and captured C1 identity. Confirm and Cancel
accept only that capability and its exact generation. Default, copied,
foreign-owner, old-epoch, already-used and reflection-corrupt capabilities
throw and cannot change state.

The private decision state is closed to Pending, ConfirmInspecting, Consumed
and Closed. Confirm first atomically reserves Pending -> ConfirmInspecting;
Cancel may atomically consume only Pending. While inspection is reserved,
concurrent/reentrant Confirm or Cancel is rejected nonmutating, never queued.
Only an actual capture Busy releases ConfirmInspecting -> Pending and retains
the exact capability for an explicit retry. Every other outcome atomically
consumes/closes the reserved decision and invalidates both callbacks before
publication. Unexpected inspection failure closes it, never releases it.
Duplicate, late and reentrant callbacks cannot recreate a request or cross an epoch.
Disable/destroy closes the decision without treating it as Cancel.

Confirm first recaptures a new C1 identity under the root lease and compares
the complete three-leaf identity with the displayed identity:

- exact equality publishes one `ConfirmedProfileResetRequestV1` and closes the
  decision epoch;
- a newer meaningful or ambiguous identity publishes no confirmed request and
  enters `FreshDecisionRequired`; a new prompt capability may be created only
  from that newer identity in a new decision generation;
- newly exact-default state may follow the no-confirmation path, but still uses
  the new identity;
- Busy leaves no winner published and allows only an explicit retry of the
  same confirm decision with the same still-live capability;
- unreadable/barrier/manual-repair evidence terminally closes the epoch with no
  request and no mutation.

No stale displayed identity may be overwritten, silently replaced, or passed
to C1. A C1 `ConfirmationStale` returned later at execution means no barrier was
published: the executor reports the typed stale result to C3, the attempted
confirmed request remains consumed history, and C3 must capture and open a new
decision/interaction epoch. It never retries C1 with the consumed identity.

## Cancellation and fresh interaction epoch

Cancel changes no profile byte, confirmation identity, launch/current-session
profile, setting, binding, tutorial/progression snapshot, action collection,
map enable state, semantic-frame publication, handoff/launch receipt, scene or
run state. It consumes only the decision capability and records `Cancelled`.

Because Q-A and Q-B are monotonic, cancellation recovery uses this bounded
handshake:

1. C3 issues one `HubNewGameRearmCapabilityV1` bound to the cancelled epoch and
   exact presenter/Q-B owner/router cohort.
2. Q-B records the old request as consumed and advances its independent epoch
   counter; it never clears `_takenRequest` history or republishes it.
3. Q-A validates the same successor, creates a fresh presentation-controller
   intent slot from its already validated immutable handoff and current notice
   state, clears only transient focus/hover/pending input for the new epoch, and
   prepares the successor with `NewGame` focus without yet entering Ready.
   If the existing notice was dismissed, the fresh controller uses its existing
   dismissal method before publication to preserve that notice state; it does
   not retake or republish a notification. Ready is published only after step 4.
4. A fresh read-only cursor is created with the existing factory against the
   exact unchanged router. Q-A quarantines the successor in AwaitingBaseline
   and deliberately discards the next consecutive frame obtained by TryAdvance
   before making the new intent slot Ready. The old cursor is retained only as
   history. Already-published frames and edges from the cancelled epoch cannot
   activate anything in the new epoch. A skipped tick or malformed frame closes
   the rearm rather than resetting a failed cursor.
   A separate owner-bound successor-baseline-pending witness overrides the
   presenter's ordinary `_cursor.State == Ready` promotion. The factory's
   immediate Ready value is not the successor's readiness proof. While this
   witness is pending, Update cannot promote Ready and FixedUpdate may only
   TryAdvance the exact fresh cursor; false means stay quarantined, true means
   validate and discard that frame, then publish Ready and consume the witness.
   No Interpret/Activate call is legal on this discarded frame. Disable/destroy,
   a foreign cursor, skipped tick, or corrupted witness closes all successors.
5. C3 consumes the rearm capability. Reuse, partial successor disagreement or
   any fault closes all participating owners; there is no rollback to the old
   epoch and no replay of its request.

Rearm is legal only before C1 publishes a durable barrier. Immediately before
an execution owner calls C1 Begin, it consumes the confirmed request and C3
atomically enters `ExecutionCommitted`, invalidating confirm, cancel, retry and
rearm capabilities. After that point Cancel is illegal. If C1 reports Busy or
`ConfirmationStale` before barrier publication, only the typed executor-report
path may create a new C3 epoch. `ReloadRequired` or `ManualRepairRequired`, or
any known/potential barrier publication, closes interaction terminally: no old
menu, cancel, same-session retry or rearm is allowed.

## Closed states and outputs

`NewGameConfirmationOwnerStateV1` is closed to `AwaitingRequest`, `Inspecting`,
`AwaitingCaptureRetry`,
`AwaitingDecision`, `FreshDecisionRequired`, `ConfirmedReady`, `Cancelled`,
`Rearming`, `ExecutionCommitted`, `TerminalFailure`, and `Closed`.

`NewGameObservationClassificationV1` is closed to `NoConfirmationRequired`,
`ConfirmationRequiredMeaningful`, and `ConfirmationRequiredAmbiguous`.
`NewGameConfirmationOutcomeV1` is closed to `DecisionRequired`, `Confirmed`, `Cancelled`,
`FreshDecisionRequired`, `Busy`,
`CaptureUnreadable`, `ReloadRequired`, and `ManualRepairRequired`. Only
`Confirmed` carries the one-shot confirmed request. Default/unknown enums,
impossible field combinations, mismatched duplicate proofs and reflected rows
throw. Failure results never carry a profile document, identity or success-like
request.

Only DecisionRequired carries the private live decision capability and exact
prompt/label values, with either confirmation-required classification. It
carries no captured identity/document or execution request. Busy from initial
capture requires AwaitingCaptureRetry with the retained issuer-authenticated
request; Busy from Confirm requires AwaitingDecision with the same Pending
capability. Neither grants a new Q-B take or input epoch.

Only Confirmed carries the request, both for explicit consent and the separately
evidenced no-confirmation classification. Busy carries only private retry
ownership, never an identity or confirmed request in the public/presentation row.

The confirmed request grants only permission to attempt C1 Begin with its exact
identity after execution commit. It grants no C2, barrier-removal, memory,
receipt, UI, scene, gameplay, save, notification, wardrobe, Settings or Quit
authority.

## Requirements

- **REQ-M5D7QC3-001:** Consume one exact Q-B NewGame request into one
  owner/root/epoch-bound confirmation lifecycle without executing an effect.
- **REQ-M5D7QC3-002:** Freshly inspect all three active leaves and require
  confirmation for every meaningful or ambiguous prior-state row.
- **REQ-M5D7QC3-003:** Publish exact bounded Korean copy and enforce one-shot,
  mutually exclusive, owner-authenticated Confirm/Cancel callbacks.
- **REQ-M5D7QC3-004:** Revalidate the complete C1 identity at Confirm and re-gate
  changed state without stale overwrite or same-identity retry.
- **REQ-M5D7QC3-005:** Make Cancel byte/profile/map neutral and rearm only a
  fresh interaction epoch without reversing or replaying Q-A/Q-B history.
- **REQ-M5D7QC3-006:** Invalidate all decision/rearm authority before C1 Begin;
  prohibit Cancel and old-menu interaction after any durable barrier boundary.
- **REQ-M5D7QC3-007:** Remain synthetic-only and grant no C2/destination, scene,
  gameplay, wardrobe, Settings, Quit, live UI-art or unrelated authority.

## Acceptance criteria

- **AC-M5D7QC3-001:** Exact NewGame, non-NewGame, duplicate, foreign receipt,
  owner/router/root/topology/epoch mismatch and reflection-corrupt matrices
  prove one exact request intake and fail-closed containment.
- **AC-M5D7QC3-002:** Primary/Previous/Temp permutations of missing, exact
  default, every single non-default field, input-recovery-required, invalid,
  unsupported and unreadable rows prove the closed decision classification;
  filename, selected source and revision alone never decide it.
- **AC-M5D7QC3-003:** Golden Korean copy and exact labels, plus default/forged/
  copied/late/concurrent/reentrant Confirm/Cancel tests, prove one winner and no
  hidden authority in presentation values.
- **AC-M5D7QC3-004:** Every leaf changed between display and Confirm, including
  exact-default transitions, additions/removals and ambiguous bytes, proves
  full fresh identity comparison and a new decision generation with no stale
  identity passed to C1.
- **AC-M5D7QC3-005:** Cancellation snapshots prove identical profile bytes,
  memory snapshots, actions/maps, receipts and scene/run state; a fresh epoch
  can accept one new selection while the old Q-A intent and Q-B request remain
  immutable, unavailable and unreplayed.
- **AC-M5D7QC3-006:** Frame/callback faults around rearm prove a fresh cursor
  baseline, no cancelled-epoch edge delivery, monotonic generations and
  terminal closure on partial successor disagreement.
- **AC-M5D7QC3-007:** Busy before Begin permits only bounded retry/re-gate;
  ConfirmationStale consumes the attempted execution request and creates no
  barrier; ReloadRequired, ManualRepairRequired and every possible post-barrier
  row forbid Cancel, rearm, old-menu interaction and same-session retry.
- **AC-M5D7QC3-008:** Execution-commit fault points prove confirm/cancel/retry/
  rearm authority is invalid before C1 can publish a barrier and cannot be
  recreated by exceptions or teardown.
- **AC-M5D7QC3-009:** Static/API review proves no C2 call, barrier removal,
  destination/scene/gameplay/map enable, save outside C1, notification,
  wardrobe/costume mutation, Settings/Quit effect, UI asset generation,
  network, clock or random authority.
- **AC-M5D7QC3-010:** Focused and required regression suites have zero failed,
  skipped and inconclusive tests; Luna reports P0=0/P1=0 before Astra accepts.

Traceability: `001 -> AC-001`; `002 -> AC-002/004`; `003 -> AC-003`;
`004 -> AC-004/007`; `005 -> AC-005/006`; `006 -> AC-007/008`;
`007 -> AC-009/010`. All identifiers use the full `AC-M5D7QC3-*` prefix.

## Proposed implementation allowlist after approval

After Astra changes this contract to Approved, implementation may only:

- add `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs`
  and `.meta` for the engine-facing, Profile-friend coordinator/value core;
- add
  `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs`
  and `.meta` for the synthetic Unity owner;
- modify `HubMenuPresenterV1.cs` only for the generation-bound NewGame rearm,
  fresh controller/cursor baseline and immutable old-intent evidence;
- modify `HubMenuIntentHandoffOwnerV1.cs` only for the reciprocal generation
  rearm and immutable old-request evidence;
- modify `DesktopProfileLaunchAdapterV1.cs` only for the internal read-only
  original-root getter described above, after C2-owned edits are frozen;
- modify `ProfileResetDiskTransactionV1.cs` only if a narrow observation-only
  identity comparison/projection is required to keep three-leaf interpretation
  C1-owned; it may not change Begin, durable mutation or C1/C2 authority;
- add only the internal observation acquisition/provenance lane to
  `ProfileNewGameResetServiceV1.cs` under Approved
  `2026-09-28-vd09-m5d7q-c3l-observation-lease-provenance.md`; preserve original
  Acquire, capture, Begin/Resume and durable bodies unchanged;
- add focused Input.Unity EditMode and Hub.Presentation.Unity EditMode/PlayMode
  synthetic tests, `.meta`, evidence, and one minimal `docs/README.md` link.
- after C2/C2R final independent acceptance, conditionally modify only
  `Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` for the exact
  C3 Adapter successor evidence gate below. This is evidence maintenance, not
  permission to weaken historical or current-file assertions.

No asmdef/friend, launch receipt, generated action/input asset, Q0 prefab,
scene, menu prefab/art/TMP asset, Packages, ProjectSettings, costume/media,
save algorithm, C1/C2 durable behavior or unrelated source change is allowed.
The existing Profile -> Input.Unity and Input.Unity -> Hub.Presentation.Unity
friend boundaries are sufficient.

Internal deterministic controls may throw only at named intake, capture,
decision-CAS, recapture, request-publication, execution-commit and rearm
boundaries. They may not substitute identities, leaf classifications, requests,
frames or results. Production calls the real C1 observation and existing real
presenter/router objects; there is no alternate filesystem or input path.

## Stop conditions and unresolved decisions

Stop for Astra if the exact three-leaf decision cannot remain C1-owned, if
rearm requires clearing/reusing Q-A/Q-B historical slots, if cancellation
requires replacing actions or enabling a map, if any path can call C1 before
decision invalidation, or if implementation needs a public ABI, friend/asmdef,
scene/prefab/art or parent-behavior change.

No new user product decision is required for the technical epoch/rearm design,
the exact reset fields, manual-only indefinite archive, or separate unchanged
costume storage: those follow the Approved parent and ADR-0035. A user decision
is required only if product intent changes to delete archives, reset costume or
other storage, allow Cancel after barrier publication, silently proceed on
ambiguous/unreadable data, or promise a live destination/return-to-menu flow in
this slice.

## Delivery gate

Luna pre-review must cite every `AC-M5D7QC3-*`, verify the Q-A/Q-B monotonic
history and fresh-frame boundary, and distinguish pre-barrier Busy/Stale from
post-barrier terminal results. Astra alone may change Review to Approved. Terra
then implements with `REQ-M5D7QC3-*` citations; Luna independently verifies;
Astra alone accepts integration. The approval below governs implementation.

### Astra approval — 2026-09-28

Sol drafted the bounded contract. Astra corrected the reverse assembly
dependency, same-lease three-leaf projection, revision-independent product-field
classification, Busy reservation/retry ownership, classification/outcome split,
private Q-B epoch issuance, notice preservation and fresh cursor quarantine.
Luna independently reviewed the amended contract at Review SHA
`423DE7B2ABCBB0267768EB596E9230840B289E0FDD1D257F5005944A62ABA999`
and found the remaining successor-baseline design P1 closed. Its pre-gate
record maps all ten ACs and explicitly claims no runtime acceptance.

Astra approves only the exact allowlisted C3 implementation and synthetic tests.
C2 and C3 are not yet Verified; live confirmation/menu graph, C1/C2 execution
wiring, interactive destination, scene/gameplay and costume/media remain
excluded. Existing C2 source must be frozen before any C3-owned changes to its
Profile/launch boundary are allocated. No new user product decision is
introduced by these technical lifecycle safeguards.

### Astra bounded Q0 successor amendment — Approved, 2026-09-28

The prior Approved C3 execution snapshot was SHA-256
`34D7A5FE0D8DCD67F224EABD8CCEE7168E39D5AB4F6B110F4A7024A9D1A116C6`.
Luna's forward review `2026-09-28-vd09-m5d7q-c3-q0-successor-maintenance-review.md`
identified the necessary strict evidence succession after the allowlisted
read-only Adapter accessor changes its source fingerprint. Astra approves only
this conditional test-maintenance lane under AC-M5D7QC3-009/010.

C2/C2R must first be independently accepted on the unchanged current source.
Terra may then implement the C3 accessor, but may not edit the Q0 test until
Luna independently reviews the exact resulting Adapter source and Astra records
separate approval of its precise SHA-256 and C3 contract/evidence provenance.
Only then may one C3 successor row become the strict current Adapter assertion.
Preserve all nine historical rows, seven strict legacy-current rows, and the
accepted C2/C2R Adapter `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`
as immutable historical evidence, rather than testing that predecessor hash
against the changed current source. Router
`66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`
remains the sole strict current Router assertion.

No old-or-new fallback, hash range, alternate current-file alias, relaxed
assertion, history rewrite, runtime authority, new assembly or product change
is permitted. Actual source namespaces must drive subsequent test selection;
the established Input.Unity EditMode namespace is
`AcadeGameMaker.Tests.EditMode.InputUnity`. A passing predecessor suite is not
evidence for an unexecuted C3 successor.

### Astra typed observation-lease clarification — Approved, 2026-09-28

C3 Approved predecessor SHA-256:
`8B17E87DB947F08095EBE8525E9F50D7C5D6F1187804441F99BAF4AE551D9BD2`.
Terra and Luna independently found that legacy Acquire collapses unavailable
root/access/security failures into Busy and cannot prove C3's genuine timeout
rule. Approved C3L supplies only the observation acquisition lane and exact
typed provenance; this closes the design blocker without changing parent
AC-M5D7QC3-002/004/007/009/010 or existing durable behavior. The other C3
allowlist, synthetic-only restrictions and Q0 exact-source successor gate
remain unchanged. C3/C3L acceptance requires actual independent verification.
