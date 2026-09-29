# C3 Q-B·owner·presenter successor 일관 단위 구현 체크리스트

- 날짜: 2026-09-29
- 상태: 구현 명세 제안 — Astra 승인 범위 안에서 Terra가 적용, 자체 수용 근거 아님
- 제외: C4 report intake, identity 추출, C1 `Begin`, C2, live UI graph

## 실제 구조와 방향

- asmdef 방향은 `Hub.Presentation.Unity -> Input.Unity -> Profile`이다. Input.Unity에
  Hub 형식이나 friend를 추가하지 않는다.
- 현재 Q-A는 `HubMenuPresenterV1.TryTakeRetainedIntent`, `Update`, `FixedUpdate`,
  `HubMenuPresentationControllerV1.Create/CurrentView/TryDismissNotification/
  TryActivate/TryTakeIntent`를 사용한다.
- cursor는 `UiSemanticFrameCursorV1.Create`, `State`, `TryAdvance`만 사용한다.
  `TryAdvance == false`는 대기이고, 첫 `true`만 유효한 연속 frame이다. skip이나
  foreign source는 예외와 terminal cursor fault다.
- Q-B의 기존 `TryTakeRequest(out HubMenuIntentRequestV1)`, `_takenRequest`와
  `RequestTaken`은 최초 세대 역사로 그대로 둔다.

## 소유 필드와 세대 불변식

### `NewGameConfirmationOwnerV1`

- exact presenter, Q-B, latch의
  `BoundLaunchAdapterForAuthoring/BoundRouterForAuthoring`과 동일한 adapter/router;
- closed owner state와 proof, checked `long interactionEpoch`, fresh reference
  `epochToken`, actual `IssuedNewGameRequestV1` pending 참조와 proof;
- accepted request/issuance, display capture, initial `CaptureRetryWitness`, 단조
  decision generation, `DecisionWitness`, confirmed request, rearm capability;
- 종료 epoch의 append-only history와 현재 epoch slot.

`AwaitingRequest`는 두 형태를 명시적으로 구분한다.

1. **최초 no-history:** epoch 1, 최초 token, history 없음, pending issuance/capture/
   decision/confirmed/rearm 없음. Q-A/Q-B의 기존 live 최초 슬롯만 사용한다.
2. **successor-current-slot pristine:** prior history가 반드시 있고 최초 Q-A retained
   intent와 Q-B `_takenRequest/RequestTaken`도 그대로 존재한다. exact rearm에서
   checked 증가한 epoch와 fresh token을 가진 현재 successor slot만 pending
   issuance/capture/decision/confirmed가 비어 있다. 최초 no-history 검사를 다시
   적용하거나 과거 필드를 지우지 않는다.

### Q-B와 Q-A

- Q-B는 현재 epoch용 `SuccessorRequestSlot`과 `TakenEpochRecord` tail을 추가한다.
  record는 epoch/token, exact rearm 출처, actual handle, request, owner/presenter/router,
  terminal phase를 보존한다.
- presenter는 `SuccessorEpochSlot`에 exact Q-B reservation, epoch/token, fresh
  controller/cursor, focus/hover, retained/consumed successor intent, baseline-pending
  witness와 proof를 둔다. 기존 `_controller/_cursor/_retainedIntent/
  _consumedTransferIntent/_state`는 최초 역사다.

## 내부 API 체크리스트

### 실제 take와 intake

- Q-B `TryTakeNewGameForConfirmation(owner, presenter, router, out issued)`는 보정된
  opaque-handle 설계 순서를 따른다. legacy take 전에 exact cohort/epoch, 최초 또는
  successor pristine, private live NewGame와 immutable handoff receipt를 검증한다.
- clean non-NewGame/foreign/mismatch는 모든 상태 비변경과 lower capture 0회다.
  reflected corruption만 기존 terminal containment를 사용한다.
- actual take 이후 mint/bind fault는 history를 보존하고 Q-B/owner를 닫는다.
- getter 없는 sealed handle의 internal constructor는 미등록 후보만 만든다. 별도
  최상위 private constructor에 Q-B가 접근하는 코드는 금지한다. 실제 발급은 Q-B의
  private `ConditionalWeakTable` 등록과 exact record/owner pending 참조 결속으로만
  인증한다. 미등록 후보는 상태 비변경 거부하며 자동 등록·값 조회를 하지 않는다.
- 최초 take만 기존 `TryTakeRequest`를 호출한다. successor는 현재 슬롯의 live
  request/proof를 전용 take로 소비하고 최초 `RequestTaken`과 taken 필드를 보존한다.
  최초 taken 필드가 비어 있어야 한다는 검사를 successor에 적용하지 않는다.
- owner의 `AcceptNewGame(IssuedNewGameRequestV1 issued, adapter, router)`만 허용한다.
  값-only request entry나 pending handle 자동 검색은 두지 않는다. Q-B one-shot
  consume이 돌려준 request는 데이터 검증 후에만 저장한다.
- 그 뒤에만 승인된 `ProfileNewGameConfirmationV1.Capture`를 호출한다.

### capture·decision

- initial Captured: classification에 따라 `AwaitingDecision` 또는 lower opaque
  confirmed request를 받아 `ConfirmedReady`로 간다.
- initial Busy: 동일 request/issuance/epoch에 묶인 `CaptureRetryWitness` 하나와
  `AwaitingCaptureRetry`; `RetryIntake` CAS 승자만 새 Capture를 호출한다.
- initial Unreadable/예외: retry를 닫고 `TerminalFailure`.
- Confirm은 `DecisionWitness.Pending -> ConfirmInspecting` CAS 승자만 recapture한다.
  Busy만 같은 witness를 Pending으로 복귀시키고 `AwaitingDecision`을 유지한다.
  동일 identity 또는 changed exact-default의 lower confirmed 결과는 capability를
  먼저 소비하고 `ConfirmedReady`; meaningful/ambiguous 변경은 이전 decision을
  닫고 `FreshDecisionRequired`; Unreadable/예외는 terminal이다.
- `OpenFreshDecision`만 checked decision generation을 증가시키며 새 capability를
  만든다. display capture를 제자리 덮어쓰지 않는다.
- Cancel은 Pending -> Consumed CAS 승자 하나만 `Cancelled`와 exact rearm
  capability를 만든다. ConfirmInspecting, Busy intake, confirmed, terminal 상태에서는
  Cancel을 허용하지 않는다.

### cancel successor와 cursor baseline

1. owner `Cancelled -> Rearming`; Q-B가 exact rearm capability를 소비하지 않은 채
   checked successor epoch/fresh token을 예약한다.
2. presenter `PrepareNewGameSuccessor`는 exact Q-B reservation/cohort와 기존 immutable
   handoff/notification을 검증하고 `HubMenuPresentationControllerV1.Create`로 fresh
   controller를 만든다. 이전 view가 Dismissed면 fresh controller에 기존
   `TryDismissNotification`을 한 번 적용한다.
3. `UiSemanticFrameCursorV1.Create(exactRouter)`로 fresh cursor를 만들고 focus는
   NewGame, hover는 null로 둔다. factory가 즉시 Ready여도 successor는
   `BaselinePending`이다. old controller/cursor/intent는 history로 연결한다.
4. Q-B가 presenter acknowledgement를 검증해 successor `AwaitingIntent`를 commit한
   뒤 owner가 rearm capability를 소비하고 successor-current-slot pristine
   `AwaitingRequest`를 게시한다. 중간 fault는 세 참여자를 닫고 epoch를 되돌리지
   않는다.
5. presenter `Update`와 `FixedUpdate`는 successor baseline-pending 분기를 기존
   AwaitingBaseline/Ready/IntentRetained 분기보다 먼저 처리하고 반환한다.
   `FixedUpdate`는 exact fresh cursor의 `TryAdvance(exactRouter, out frame)`만 호출한다.
   false면 격리를 유지한다. 첫 true frame은 `Validate` 후 **폐기**하고
   Interpret/Activate/hit test/notice dismissal을 전혀 호출하지 않은 뒤 witness를
   소비해 successor Ready를 게시한다. skip, malformed frame, foreign cursor/router,
   disable/destroy는 모든 current successor를 terminal close한다.
6. 이후 fresh controller의 실제 NewGame activation만 successor retained intent를
   만들고, Q-B의 successor take가 fresh opaque handle/append-only record를 만든다.

### commit·종료

- `CommitForExecution(confirmedRequest)`는 `ConfirmedReady`에서만 lower
  `ReserveExecutionCommit`을 얻고, confirm/cancel/retry/rearm과 Q-A/Q-B current
  interaction을 모두 닫아 owner를 `ExecutionCommitted`로 만든 뒤 lower
  `CompleteExecutionCommit`을 호출한다. 그 뒤 동일 opaque request만 반환한다.
- reserve 이후 fault는 owner/Q-A/Q-B/lower를 terminal close하고 confirmed request를
  복구하지 않는다. disable/destroy도 Cancel이나 rearm을 제조하지 않는다.
- 이 단위에는 `IdentityForExecution`, executor consumption, C1/C2 호출,
  `ReportExecution` 형식·mint·intake를 추가하지 않는다.

## 집중 synthetic fixture 이름과 필수 행

### EditMode `NewGameConfirmationOwnerV1Tests`

- `AC001_ActualQBTakeOpaqueHandleIsConsumedOnce`
- `AC001_CopiedEqualRequestCannotConsumePendingActualHandle`
- `AC001_UnregisteredCandidateCannotConsumePendingActualHandle`
- `AC001_FirstAwaitingRequestRequiresNoHistory`
- `AC001_SuccessorAwaitingRequestKeepsOldHistoryAndPristineCurrentSlot`
- `AC002_InitialBusyRetryRetainsOneIssuanceWithoutRetake`
- `AC003_ConfirmCancelRaceHasExactlyOneWinner`
- `AC004_ConfirmRecaptureEqualChangedDefaultAndChangedPromptRows`
- `AC007_InitialBusyAndConfirmBusyKeepDistinctCapabilities`
- `AC008_CommitClosesAllAuthorityBeforeLowerComplete`
- `AC008_CommitFaultAndTeardownNeverRestoreConfirmedRequest`

### PlayMode `NewGameConfirmationRearmPlayModeTests`

- `AC005_CancelRearmPreservesInitialPresenterAndQBTakenHistory`
- `AC005_RepeatedCancelAppendsCheckedEpochRecords`
- `AC006_ImmediateReadyFactoryStillDiscardsFirstTrueSuccessorFrame`
- `AC006_AwaitingBaselineFalseFramesRemainQuarantinedThenDiscardFirstTrue`
- `AC006_DiscardedFrameCannotActivateDismissOrReachQBTake`
- `AC006_SkippedMalformedForeignAndPartialSuccessorFaultsCloseAllOwners`
- `AC006_DismissedNoticeStateIsPreservedWithoutRetakingNotification`
- `AC009_CoherentUnitHasNoReverseReferenceC1C2SceneMapOrExecutionReportAuthority`

양성 fixture는 실제 Q-A activation, Q-B `LateUpdate`, actual opaque handle과 real
cursor publication을 사용한다. 음성 copied-row 행은 actual handle consume 전후를
검사해 거부가 pending 권한을 소모하지 않았음을 증명한다. lower 단위 결과만으로
Q-B/owner/rearm 또는 전체 C3 권한을 통과 처리하지 않는다.
