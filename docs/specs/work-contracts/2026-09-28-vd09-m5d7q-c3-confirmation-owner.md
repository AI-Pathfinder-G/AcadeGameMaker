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
    IssuedNewGameRequestV1 issuedRequest,
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

2026-09-29 아스트라 승인 내부 진입점 구체화: 실제 등록된 불투명
`IssuedNewGameRequestV1` 참조만 입력으로 받는다. 요청 값만 받거나 값으로 대기
권한을 찾는 진입점은 금지한다. 내부 생성자는 미인증 후보만 만들며 비공개
등록부와 정확한 Q-B·소유자·세대 결속만 발급을 증명한다. 정상 구성·항목·영수증·
초기 상태 불일치는 실제 take 전에 상태 변경과 관찰·root 접근 없이 거절한다.
최초 무이력 조건을 후속 세대에 재적용하지 않고 새 현재 슬롯만 검증하며 과거
이력을 보존한다. 실제 take 이후 발급·결속 실패는 이력 복구 없이 양쪽을
종료한다. REQ-M5D7QC3-001/005 및 AC-M5D7QC3-001/005/006의 구체화이며 C4
승인이나 과거 요청 값 변경이 아니다. 정확한 설계·독립 폐쇄는 2026-09-29
Q-B 일관 단위 구현 승인에 기록한다.

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

### 아스트라 단계적 수용 보정 — Approved, 2026-09-29

[ADR-0036](../../adr/0036-staged-confirmation-and-execution-acceptance.md)에 따라 C3의 실제 Q-B 발급, 관찰, 결정, 취소, 새 입력 세대와 실행 직전 권한 폐쇄를 선행 단계로 구현·독립 검증한다. AC-M5D7QC3-001..006/009와 AC-M5D7QC3-010의 해당 집중·필수 편집 모드 및 실행 모드 회귀, 루나 P0/P1=0과 아스트라 수용이 필요하다. C3L 관찰 단위의 제한된 회귀 결과로 새 소유자·재무장 검증을 대신할 수 없다.

AC-M5D7QC3-007의 관찰 Busy와 AC-M5D7QC3-008의 C3 커밋 구간은 실행 연결 전 부분 증거로만 기록한다. 두 전체 기준은 Open/Not Verified로 유지하며 부분 PASS·독립 Verified로 표시하지 않는다. C3 전체 Verified도 아직 기록할 수 없다.

실제 C1 결과 발급·보고 수용은 별도로 Approved가 된 C4만 구현할 수 있다. 그 후 남은 AC-M5D7QC3-007/008과 AC-M5D7QC4-001..010 전체를 실제 출처 및 현재 소스의 집중·필수 회귀로 공동 검증하고 루나 독립 검수와 아스트라 최종 수용을 받아야 한다. 반사 호출을 통한 정상 권한 발급, 합성 결과 발급기, 결과 스칼라 대체는 허용하지 않는다. 제품 동작·허용 목록·C1/C2 실행 금지는 그대로 유지한다.

승인 근거는 [독립 보정 검수](../../verification/2026-09-29-vd09-m5d7q-c3-c4-acceptance-cycle-resolution-luna-closure.md)와 제안서 SHA-256 `A8D35076BADAD0E897C9E27F15055956A3B13D96AE222BE81953129ECB4660B3`이다. 이 보정은 수용 순서의 승인이지 실제 코드 수용 또는 C4 계약 승인 자체가 아니다.

### 아스트라 Q-B 프레임워크 가져오기 감사 후속 보정 — Approved, 2026-09-29

`REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/009/010`과 기존 `AC-M5D7QB-004`에 한정하여 [솔 후속 유지안](../../proposals/2026-09-29-c3-qb-framework-import-audit-successor.md) SHA-256 `4AB34DE39D4648E291A88CA0068FCCA68275E49D772165A1BD9DE8744642E18B` 및 [루나 독립 설계 검수](../../verification/2026-09-29-c3-qb-framework-import-audit-luna-design-review.md) P0/P1=0을 근거로 다음 시험 유지 범위만 허용한다.

기존 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs`의 해당 권한 감사 메서드에서 Q-B 소스 입력 한 군데에 엄격한 후속 도우미를 적용한다. 수정 전 전체 바이트 지문 `FC7DD9F902C9783036137CE68423D43A28C00DE187DA125FDB8C41CDCF79B421`을 `.txt` 이력 증거로 보존한다. 기존 금지 배열·반복 검사 본문·메서드 이름·개수·모든 행동과 presenter/prefab/request 검사는 보존한다. 새 `HubMenuIntentHandoffC3AuditSuccessorTests.cs`와 `.meta`에 도우미와 별도 반례 행을 두며 기존 562개 선택 이름과 신규 집중 선택을 분리한다.

현재 승인된 평범한 가져오기 선언과 공백만 있는 파일 머리에서 정확한 `using System.Runtime.CompilerServices;` 물리 줄 한 개만 구별한다. 나머지 전체 텍스트는 원래 부분 문자열 금지 검사를 그대로 받는다. 범용 토큰·주석·이름공간 제거, 유니코드 표기·별칭·전처리·공백 정규화로 검사 우회, 본문에서 같은 프레임워크 형식 사용을 감추는 처리는 금지한다. 신규 반례는 모든 기존 금지 토큰, 중복·다른 위치·변형 선언을 거부해야 한다. 실제 등록부는 Q-B의 비공개 약한 참조 등록부로 유지하며 런타임·조립·공개 권한을 변경하지 않는다.

새 정확 지문에서 독립 구현 검수와 집중 및 기존 562개를 포함한 필수 회귀가 필요하다. 이전 감사 본문 또는 이전 회귀를 현재 통과로 재사용하지 않는다. 이 보정은 시험 유지의 승인이고 C3/C4 수용이 아니다.

### 아스트라 실제 임시 루트 실행 모드 시험 위치 보정 — Approved, 2026-09-29

[솔 시험 위치 보정안](../../proposals/2026-09-29-c3-playmode-fixture-placement.md) SHA-256 `BAB0264267FA79583C57AD121FC83E28DEE28E2C99DBCA2C7183C9C1868EAF99`와 [루나 후속 독립 폐쇄](../../verification/2026-09-29-c3-playmode-fixture-placement-luna-design-review.md)의 P0/P1=0을 근거로 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs`와 `.meta` 두 신규 경로만 시험 허용 목록에 추가한다. 기존 Input.Unity.PlayMode.Tests 조립에서 직접 구현한 임시 루트 환경과 실제 준비 포트를 사용한다. 조립·friend·제품 접근 권한·실제 저장 경로를 변경하지 않는다.

상위 형식은 정확한 조립 한정 이름·선언 형식·전체 매개변수와 반환 서명을 확인한 정상 메서드 호출로 연결한다. 실제 반환 불투명 객체의 참조 동일성을 보존한다. 이름 단독 검색·유사 오버로드 대체·다른 시험의 비공개 발급기 결합·증표/영수증/등록부/정상 상태 반사 제조는 금지한다. 준비·실제 메뉴 선택·발급·확인·취소·재무장을 실제 입력 및 생애 경로로 수행한다. 해석 실패나 호출 예외는 원인을 보존해 시험 실패로 드러낸다.

신규 실행 모드 집중 선택과 전체 예상 이름을 최종 소스에서 고정하고 실제 결과 이름·중복·누락·종료 코드·실패/건너뜀/판정 불가·전후 지문을 대조한다. 기존 536개와 결함 행 74개 이름 및 회귀 선택은 그대로 보존해 별도로 실행한다. 기존 결과나 설계 통과를 신규 실행 증거로 재사용하지 않는다. 추적은 `REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/006/009/010`이다. 런타임 변경·C3 전체·C4 수용 승인이 아니다.

### 아스트라 분류·새 결정·재진입 증거 해석 — Approved, 2026-09-29

AC-M5D7QC3-002/004의 [필수 시험 행렬](../../proposals/2026-09-29-c3-required-decision-test-matrix.md) 지문 `B213890A1204B72BDBE9B6263154341A6668F95C7BBC3D224EBAB8D15D41599D`와 [독립 설계 검수](../../verification/2026-09-29-c3-required-decision-test-matrix-luna-design-review.md)를 근거로 승인 본문의 exact-default 직접 확인과 AC004의 세대 문구를 다음과 같이 구체화한다. 모든 파일 변경은 fresh actual capture의 전체 identity로 재검증한다. 의미/모호 분류가 새 prompt를 요구하면 `FreshDecisionRequired` 후 `OpenFreshDecision`으로 결정 세대를 정확히 한 번 증가시키고 구 권한을 거부한다. 새 분류가 `NoConfirmationRequired`이면 승인된 별도 직접 `Confirmed` 경로로 fresh capture에 결속하고 새 prompt·결정 권한·결정 세대를 제조하지 않는다. 세 파일 각각의 직접 기본값 전이를 별도 실제 시험으로 입증한다. 이는 기존 동작의 해석이며 단순 변경 무시나 stale identity 사용을 허용하지 않는다.

AC-M5D7QC3-003의 재진입은 [실제 도달 가능한 증거 경계](../../proposals/2026-09-29-c3-reentrant-evidence-boundary.md) 지문 `DFF389EBE18B29EC5B6DC790FD8D73060A505CBEA3E99FCE17CA5161D62A3E79`에 따라 정상 재무장 렌더의 실제 그래픽 콜백에서 같은 스레드의 중첩 Confirm/Cancel/Rearm 시도를 실행하고 비변경 거부 및 외부 정상 작업의 단일 완료를 확인한다. 정상 Confirm 예약 중 외부 콜백 경계가 없는 경우 정확한 동결 소스의 호출 사슬·줄·지문으로 도달 불가를 함께 검수한다. 이 자료는 live decision operation-gate 재진입 실행 증거가 아니며 동시·중복·늦은 호출 실행을 대체하지 않는다. 합성 제품 콜백 통로·권한/상태 대입을 추가해 도달 불가능한 실행을 제조하지 않는다. callback 발생이 실제 관찰되지 않으면 시험 실패다.

실제 상위 결정 권한의 null/default, 같은 형식 미등록 후보, 실제 복제본, 다른 실제 소유자의 권한, 구 세대 권한 거부와 새 권한 성공을 Confirm/Cancel 각각 검증한다. 거절 후 원래 실제 권한이 정상 한 번 성공하는 비소모와 파일·슬롯·세대 보존이 필요하다. 기존 하위 복제 시험이나 기존 codec 회귀로 대체하지 않는다. 기존 신규 집중 시험과 임시 UI 표시만 수정할 수 있으며 새 조립·friend·제품 콜백 통로·공개 API·실제 장면·자산 변경과 C1/C2 실행을 승인하지 않는다. 독립 검수와 최종 실제 실행은 여전히 필수다.

### 아스트라 진입·취소 증거와 늦은 경쟁 보정 — Approved, 2026-09-29

[제한 구현 승인](../../approvals/2026-09-29-c3-intake-cancel-and-gate-correction-approval.md)에 따라 기존 소유자와 필요할 때 기존 Q-B 발급 이력 읽기 보조, 신규 집중 시험만 보정한다. 외부 실제 권한 인증·게이트 획득 후 재인증·내부 무결성 검증을 분리한다. 이미 정상 완료된 호출의 늦은 거절은 정상 승자를 닫지 않으며, 아직 미소모인 실제 권한의 증거 손상은 전체 폐쇄한다. 일곱 진입점의 게이트 해제와 종료 상태 조회, 살아 있는 현재 입력 연결 검증을 유지한다. 정확한 지연 창은 구조 검수이며 실제 재현했다고 주장하지 않는다.

AC001의 단일 증거 손상은 승인 문서에 명시한 네 필드의 음성 주입만 허용하며 정상 권한 제조는 금지한다. AC005는 같은 합성 실행 묶음의 실제 자료·현재 메모리/reset 대상 또는 부재·입력/영수증/장면·실행 관찰값의 취소 전후 동등성과 효과 부재 구조를 검증한다. 존재하지 않는 전역 메모리·실제 게임 세션을 만들거나 그 전체 보존을 시험했다고 확대하지 않는다. 180초 제한과 필수 실행·독립 검수 요건은 유지하며 C3 전체 및 C4 수용 승인이 아니다.

### 아스트라 실제 집중 시험 실행 구성 보정 — Approved, 2026-09-29

[제한 시험 보정 승인](../../approvals/2026-09-29-c3-r6-focused-test-correction-approval.md)에 따라 기존 신규 편집·실행 모드 시험과 정확 행 매핑 도구만 보정한다. AC002의 173개 분류 행을 8개씩 22개 사례로, AC004의 역할별 68개 전이 행을 3개씩 23개 사례로 나누어 총 91개 NUnit 사례로 수행한다. 원래 377개 행의 ID·내용·역할·순서·기대 결과·권한/세대·원본 참조·전후 파일 검증은 유지한다. 해당 정확 사례가 Passed인 경우에만 하위 행을 수용하며 180초 제한을 늘리지 않는다.

정상 비활성화는 실제 실행 모드의 정상 수명주기 콜백으로 검증한다. 즉시 준비 커서 시험의 released 입력 갱신은 새 successor 생성 이전에만 수행하며, 새 세대의 첫 실제 Submit=true 발행을 버리는 조건과 다음 정상 선택·take 성공을 유지한다. 콜백 직접 호출·제품 수명주기 변경·가짜 입력·조건 완화·행 삭제·기존 회귀 선택 변경은 금지한다. 실제 R6 실패와 원시 377개 행 출력을 보존하고 새 최종 소스의 실행·독립 검수 전에는 기존 P1을 닫지 않는다. 런타임·조립·자산·설정·Q0 pin 및 전체 C3/C4 수용 상태는 그대로 유지한다.

### 아스트라 실제 Submit 최소 관측 — Approved, 2026-09-29

[제한 관측 승인](../../approvals/2026-09-29-c3-r7-submit-observation-approval.md)에 따라 기존 편집 모드 AC006 시험의 다섯 경계에서 정적 환경·원래 장치 ID/등록 여부·정확한 Router 원시 필드 여섯 개만 기록한다. 기존 입력·갱신·프레임·권한·assertion과 180초 제한을 보존한다. 지연 해석 getter나 제품 상태 쓰기는 허용하지 않는다. 독립 구현 검수 후 단일 관측 실행 한 번의 실제 결과를 남기며 이전 실패를 통과로 전환하거나 원인·C3/C4 수용을 단정하지 않는다.

### 아스트라 실제 Submit 동등 시험 이전 — Approved, 2026-09-29

[제한 이전 승인](../../approvals/2026-09-29-c3-submit-playmode-transfer-approval.md)에 따라 기존 편집 AC006과 전용 관측 helper를 제거하고 기존 실행 모드 즉시 Ready 사례에 원래 검증 전체를 옮긴다. 동일한 정상 primary/previous 구성과 실제 입력 발급·불투명 권한을 사용한다. 첫 true 발행 폐기와 다음 true 정상 선택, 역사·상태 검증을 유지한다. 377행·91분할·180초 제한과 제품/조립/설정/기존 필수 회귀는 불변이다. 원시 실패·관측·원본 바이트를 보존하며 최종 정확 선택과 소스에서 독립 검수 및 실제 실행 전 수용을 주장하지 않는다.

### 아스트라 최초 실제 입력 선택 준비 — Approved, 2026-09-29

[제한 입력 준비 승인](../../approvals/2026-09-29-c3-play-initial-selection-preparation-approval.md)에 따라 기존 실행 시험의 최초 SelectInitialNewGame 전에 정상 중립 발행 한 번으로 맵 활성화 격리 갱신을 처리하고, 실제 방향 입력과 원본 NewGame 요청을 검증한다. 재무장 이후 첫 실제 Submit 앞에는 빈 발행을 추가하지 않는다. 기존 240/15 이름·377행/91분할·180초 제한과 제품/조립/설정/필수 회귀는 유지한다. 독립 검수 후 공용 helper의 전체 실행 시험 영향과 최종 소스 필수 검증을 수행하며 기존 실패를 소급 수용하지 않는다.

### 아스트라 최초 중립 프레임 정상 소비 — Approved, 2026-09-29

[제한 소비 승인](../../approvals/2026-09-29-c3-initial-neutral-cursor-consumption-approval.md)에 따라 최초 선택 준비의 중립 Publish 바로 뒤에 정상 PresenterFixed 한 번만 추가하여 발행과 커서 소비를 대응시킨다. 추가 입력·상태 대입·cursor reset 없이 모든 검증·시간 제한과 successor 첫 실제 Submit 조건을 유지한다. 원시 실패·원본 바이트를 보존하고 독립 검수 및 최종 실제 실행 전 수용을 주장하지 않는다.

### 아스트라 취소 관찰 타입 이름 보정 — Approved, 2026-09-29

[제한 타입 보정 승인](../../approvals/2026-09-29-c3-cancel-snapshot-type-correction-approval.md)에 따라 취소 관찰 도우미의 이동 제어기 이름공간 문자열 한 곳만 실제 선언으로 정정한다. 조립 이름·필드·엄격한 타입 검사·전후 전체 비교·시간 제한은 유지한다. 원본 실패와 바이트를 보존하고 독립 검수 후 최종 실행 시험 15개 전체를 수행하며 통과·C3/C4 수용을 사전에 주장하지 않는다.
