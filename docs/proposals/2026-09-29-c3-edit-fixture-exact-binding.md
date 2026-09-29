# C3 편집 모드 실제 cohort fixture의 정확한 참조 결속

- 날짜: 2026-09-29
- 상태: 제한 fixture 보정 설계 제안 — 소스 수정·실행·승인 없음
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/002/004/007`, `AC-M5D7QC3-001/002/004/009/010`
- 대상은 `Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` 안의
  `NewGameActualCohortFixtureV1.Call/Read/Field` 및 같은 파일의 직접 이름 단독 조회다.
  제품 API·assembly·friend·권한 생성은 추가하지 않는다.

## 최소 helper 규칙

PlayMode fixture의 정확한 형식/서명 결속을 따른다. 모든 형식은 assembly-qualified
이름으로 해석하고 실제 FullName/assembly 이름을 대조한다. Hub 형식은
`AcadeGameMaker.Hub.Presentation.Unity`, lower 형식은
`AcadeGameMaker.Input.Unity` 조립의 같은 namespace에 있다. UI 예외는
`UnityEngine.EventSystems.EventSystem, UnityEngine.UI` 및
`TMPro.TMP_Text, Unity.TextMeshPro`다. UnityEngine의 GameObject/RectTransform과
직접 참조 가능한 lower 형식은 `typeof`로 얻고 명시 형식과 대조한다.

`Call`은 (정확한 receiver 형식, member 이름)으로 아래 고정 표의 Signature를
선택한다. 호출 인수의 현재 GetType이나 개수로 overload를 추정하지 않는다.
`DeclaredOnly|Instance|NonPublic`, 정확한 declaringType, parameter 순서/형식과
by-ref, 반환 형식, non-generic/non-optional 조건으로 MethodInfo를 검증한다.
표에 없는 호출은 실패한다. static/상속/name-only fallback은 없다. null 인수도
표의 선언 형식을 사용한다. actual opaque 결과는 정확한 형식/null 및 원래 참조를
확인하고 재구성하지 않는다. 해석·호출 실패는 원인을 보존한 시험 실패로 드러낸다.

기호: P=HubMenuPresenterV1, Q=HubMenuIntentHandoffOwnerV1,
O=NewGameConfirmationOwnerV1, A=DesktopProfileLaunchAdapterV1,
R=InputRouter, L=HubEntryHandoffLatchV1, H=IssuedNewGameRequestV1,
Z=NewGameConfirmationStartResultV1, D=NewGameDecisionCapabilityV1,
T=NewGameCaptureRetryCapabilityV1, E=HubNewGameRearmCapabilityV1,
S=HubNewGameSuccessorReservationV1, C=ConfirmedProfileResetRequestV1.

## 현재 제품 메서드 호출 표

| declaringType | member와 정확한 인수 | 반환 |
| --- | --- | --- |
| P | ConfigureForAuthoring(R,L,HubSafeFrameScalerV1,EventSystem,HubMenuButtonViewV1[],RectTransform,GameObject,TMP_Text,TMP_Text) | void |
| O | ConfigureForAuthoring(P,Q,A,R) | void |
| R,A,L,P,Q | Awake() — 각 해당 형식에 별도 선언한 메서드 | void |
| A,L | Start() — 각 해당 형식에 별도 선언한 메서드 | void |
| L,P | Update() — 각 해당 형식에 별도 선언한 메서드 | void |
| P | FixedUpdate(), Activate(HubMenuItemV1), ActivateSuccessor(HubMenuItemV1) | void |
| Q | LateUpdate() | void |
| Q | TryTakeNewGameForConfirmation(O,P,R,out H) | bool |
| O | AcceptNewGame(H,A,R), RetryIntake(T), Confirm(D), Cancel(D), OpenFreshDecision() | Z |
| O | Rearm(E), SetFaultForTests(string), OnDisable() | void |
| O | CommitForExecution(C) | C |
| Q | MatchesSuccessorReservation(S,O,P,R,object,long) | bool |
| Q | CommitNewGameSuccessor(S,O,P,R,object,long) | void |
| P | PrepareNewGameSuccessor(O,S,object,long) | void |

out H는 `H.MakeByRefType()`으로 조회하고 실제 parameter.IsOut까지 확인한다.
fixture가 이미 직접 호출하는 ConfigureHubUiOnlyForAuthoring/ConfigureForTests/
StepForTests 등 형식 있는 lower API는 반사로 다시 바꾸지 않는다. lifecycle 호출은
현재 편집 fixture의 실제 초기화 순서를 유지하며 진짜 반환 영수증을 사용한다.

기존 copied-row 음성 행의 직접 `GetMethod("AcceptNewGame",...)`도 같은 정확한
H/A/R→Z MethodInfo를 얻도록 제한한다. 그 음성 행은 잘못된 **데이터 row**를
넘겨 실제 reflection binder의 ArgumentException을 검증하는 목적이므로 원래
예외 증거를 보존한다. 양성 wrapper가 wrong argument를 미리 거부한 결과로 그
음성 행을 바꾸거나, row를 H로 변환하는 fallback을 만들지 않는다.

## 현재 읽기 property 표

`Read`는 exact receiver/declaringType와 property 이름·정확 반환 형식·인수 없는
nonpublic instance getter를 검증한다. 실제 getter를 호출하므로 제품의 자체
Validate가 실행된다. Field 읽기로 getter를 대체하지 않는다.

| declaringType | property | 반환 |
| --- | --- | --- |
| Z | Outcome | NewGameConfirmationOutcomeV1 |
| Z | DecisionCapability, RetryCapability, RearmCapability | 각각 D,T,E |
| Z | ConfirmedRequest | C |
| Z | Prompt, ConfirmLabel, CancelLabel | string |
| O | State | NewGameConfirmationOwnerStateV1 |
| Q | State | HubMenuIntentHandoffOwnerStateV1 |
| P | State | HubPresenterStateV1 |
| HubMenuIntentRequestV1 | Item, Receipt | HubMenuItemV1, HubEntryHandoffReceiptV1 |
| UiSemanticFrameCursorV1 | State | UiSemanticFrameCursorStateV1 |

이 표는 현재 Read 호출을 전부 포함한다. matrix에 새 Classification 등 읽기가
필요하면 실제 선언을 확인하고 별도 exact 표 행으로 추가한다. property 대신
이름이 비슷한 field/method로 넘어가지 않는다.

## 현재 읽기 field 표

`Field`는 증거 읽기 전용이다. exact declaringType/name/FieldType 및 instance,
`DeclaredOnly`와 public/nonpublic 조건을 검사한다. 내부 중첩 형식도 정확한
outer+중첩 full name과 assembly를 먼저 검증한다. 아래 internal readonly 중첩
필드는 NonPublic 범위다. SetValue 또는 serialization overwrite는 추가하지 않는다.

| declaringType | 현재 읽는 field: 정확한 FieldType |
| --- | --- |
| A | _awakeAttempted: bool |
| L | _state: HubEntryHandoffStateV1 |
| O | _operation: int, _epoch: long, _history: O+EpochHistory, _pendingIssued/_acceptedIssued: H, _decision: D, _rearm: E, _display: ProfileNewGameCaptureResultV1 |
| Q | _request/_takenRequest: Nullable<HubMenuIntentRequestV1>, _epochs: Q+TakenEpochRecord, _successor: Q+SuccessorRequestSlot |
| P | _cursor: UiSemanticFrameCursorV1, _retainedIntent: Nullable<HubMenuIntentV1>, _successor: P+SuccessorEpochSlot, _confirmationInteractionClosed: bool |
| P+SuccessorEpochSlot | Cursor: UiSemanticFrameCursorV1, BaselinePending: bool, Phase: HubPresenterStateV1, Retained: Nullable<HubMenuIntentV1>, Reservation: S, Token: object, Epoch: long |
| O+EpochHistory | Previous: O+EpochHistory |

Nullable field의 GetValue 결과는 null 또는 실제 값의 boxed object이므로 반환물의
GetType을 Nullable 형식으로 필수 비교하지 않는다. FieldInfo.FieldType은 정확한
Nullable 형식과 대조하고, null이 아닌 실제 값은 기저 struct 형식과 대조한다.

현재 목록에 private inherited field는 없다. 향후 필요하면 정확한 base declaringType을
별도 표에 명시하고 해당 field의 읽기 증거를 검증한다. 기저 형식을 순회해 처음
발견한 같은 이름의 field를 채택하는 방식은 금지한다.

## 검증 범위와 기존 동작 보존

참조 조회는 실제 제품 호출과 증거 읽기에만 사용하며 권한을 제조하지 않는다.
알 수 없는 member, 잘못된 declaration/return, by-ref 불일치 및 null에 근거한
모호한 추정은 거부해야 한다. 정상 actual opaque 경로는 1회 성공해야 한다.
현재 시험 이름·개수·기존 기대 예외를 유지하고 fixture 구현 변경 뒤 새 소스
지문과 실행 증거를 동결한다. 이전 132행 등의 실행 결과를 변경된 fixture의
검증으로 재사용하지 않는다.
