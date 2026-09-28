# C3 synthetic owner/rearm 구현 설계 초안

- 날짜: 2026-09-28
- 상태: 설계 초안, 구현·실행·수용 근거 아님
- 설계: Sol; 최종 계약·통합 승인: Astra
- 기준: Approved `REQ-M5D7QC3-001..007`, `AC-M5D7QC3-001..010`
- 2026-09-28 Luna P1 보정: initial/Confirm Busy 분리와 typed executor report 추가

## 결론

현재 구조에 새 friend나 역방향 참조를 추가할 필요는 없다. 기존
`HubMenuPresenterV1._state == IntentRetained`와
`HubMenuIntentHandoffOwnerV1._state == RequestTaken` 및 그 증거 필드는 최초
세대의 불변 이력으로 유지한다. 후속 세대는 두 클래스의 별도 활성 슬롯과
추가 전용 이력 노드로만 진행한다. `ProfileNewGameConfirmationV1.Capture`와
`GetAuthenticatedLaunchRootForNewGame`은 그대로 하위 관찰 경계가 된다.

## 권장 필드와 비공개 API

### Q-B: 실제 take 발행 권한

`HubMenuIntentHandoffOwnerV1`에 다음을 추가한다.

- `long _interactionEpoch`와 동일 증거 필드. 최초 세대는 `1`, rearm은
  `checked(_interactionEpoch + 1)`만 허용한다.
- `object _epochToken`과 동일 참조 증거. 숫자 epoch와 함께 묶되 하위
  관찰에는 이 불투명 참조만 전달한다.
- 최초 `_takenRequest`는 절대 덮어쓰지 않는다. 후속 세대는
  `SuccessorRequestSlot`과 append-only `TakenEpochRecord` 연결 목록에
  `epoch/token/request/issuance/presenter/owner/router`를 보존한다.
- Q-B 내부 전용 `IssuanceRecord`는 참조형이며 `int Claimed`를 갖는다.
  외부에서 이름을 알 수 없도록 반환 형식은 `object`로 한다.

구체 경계는 다음 형태가 적합하다.

```csharp
internal bool TryTakeRequest(
    NewGameConfirmationOwnerV1 confirmationOwner,
    InputRouter router,
    out HubMenuIntentRequestV1 request);

internal object ClaimNewGameIssuance(
    NewGameConfirmationOwnerV1 confirmationOwner,
    HubMenuPresenterV1 presenter,
    InputRouter router,
    HubMenuIntentRequestV1 request);

internal object ReserveNewGameSuccessor(
    NewGameConfirmationOwnerV1 confirmationOwner,
    object rearmCapability);

internal object CommitNewGameSuccessor(
    NewGameConfirmationOwnerV1 confirmationOwner,
    object reservedSuccessor,
    object presenterAcknowledgement);
```

첫 오버로드의 실제 take 순서는 고정한다.

1. 기존 `ValidateTopology/ValidateState`와 exact owner/presenter/router를
   참조 동일성으로 검사한다.
2. 실제 live request를 읽고 `NewGame`, 현재 epoch, 기존 handoff receipt를
   검사한다.
3. 지역 변수로 `IssuanceRecord`를 만들고 request 값은 무결성 증거로만
   복제한다.
4. taken proof/history, issuance proof/reference를 게시한 뒤 live request만
   비우고, 최초 세대라면 기존 `_state = RequestTaken`을 마지막에 게시한다.
5. 최종 검증 뒤에만 out request를 대입한다. `ClaimNewGameIssuance`는 등록된
   정확한 issuance를 `Interlocked.CompareExchange`로 한 번만 claim한다.

동일한 Item/Receipt 값을 새로 만든 행은 issuance 참조가 없으므로 실패한다.
기존 `TryTakeRequest(out ...)`는 보존하되 C3 권한을 만들지 않는다.

### C3 owner: 수명주기와 결정 원자성

`NewGameConfirmationOwnerV1`은 합성 전용 `MonoBehaviour`로 두고
presenter, Q-B owner, launch adapter, router의 정확한 참조를 보유한다.
`AcceptNewGame(request, adapter, router, this)`는 먼저 Q-B issuance를 claim한
뒤에만 하위 capture를 호출한다.

필수 소유 필드는 다음과 같다.

- 계약의 닫힌 `NewGameConfirmationOwnerStateV1`과 동일 증거
- 현재 `request`, Q-B `issuance`, 숫자 interaction epoch, `epochToken`
- `ProfileNewGameCaptureResultV1 _displayCapture`
- 단조 증가 `long _decisionGeneration`
- `DecisionWitness _decision`, `object _rearmCapability`
- 단일 `ConfirmedProfileResetRequestV1 _confirmedRequest`
- 종료된 결정/요청/rearm의 append-only 이력
- initial Busy 전용 `CaptureRetryWitness`와 실행 후속 보고 이력

`DecisionWitness`는 owner/cohort/root/epoch/request/capture/generation을 readonly로
묶고 `int Phase`만 원자 변경한다. 단계는 `Pending=0`,
`ConfirmInspecting=1`, `Consumed=2`, `Closed=3`이다.

- Confirm: `Pending -> ConfirmInspecting` CAS가 이긴 호출만 recapture한다.
- Cancel: `Pending -> Consumed` CAS가 이긴 호출만 `Cancelled`를 기록한다.
- capture Busy만 동일 witness를 `ConfirmInspecting -> Pending`으로 되돌린다.
- 나머지 결과와 예외는 먼저 `Consumed/Closed`로 만들고 callback 권한을
  무효화한 뒤 결과를 게시한다. reentrant/동시 호출은 대기열에 넣지 않는다.
- 변경된 meaningful/ambiguous capture는 `FreshDecisionRequired`에 보존하고,
  별도 `OpenFreshDecision()`만 checked generation을 증가시켜 새 capability를
  만든다. 표시된 identity를 제자리 교체하지 않는다.
- 변경된 exact-default capture는 그 새 capture로 바로 Confirmed를 발행한다.

initial capture Busy와 Confirm recapture Busy는 같은 상태나 재시도 API를 쓰지
않는다.

| 발생 위치 | owner 상태 | live 권한 | 명시적 재시도 | Busy 출력 |
| --- | --- | --- | --- | --- |
| `AcceptNewGame` 최초 capture | `AwaitingCaptureRetry` | 같은 Q-B issuance/request/epoch에 묶인 `CaptureRetryWitness` 하나 | `RetryIntake()` | identity, decision capability, confirmed request 없이 private retry 소유권만 보존 |
| `Confirm` recapture | `AwaitingDecision` | 기존 `DecisionWitness`와 **동일한** `NewGameDecisionCapabilityV1`; phase만 다시 `Pending` | 같은 capability로 `Confirm(capability)` 재호출 | 새 capability/generation/request 없이 같은 Pending decision만 보존 |

`RetryIntake()`는 `AwaitingCaptureRetry -> Inspecting` CAS를 이긴 호출 하나만
실행한다. 다시 Busy면 같은 witness로 `AwaitingCaptureRetry`에 복귀한다. capture
성공이면 retry witness를 먼저 소비하고 `AwaitingDecision` 또는
`ConfirmedReady`를 게시하며, Unreadable이나 예외면 witness를 닫고
`TerminalFailure`로 간다. 이 상태에서는 Confirm/Cancel/rearm이 모두 불법이다.

Confirm은 `DecisionWitness.Pending -> ConfirmInspecting` CAS를 이긴 호출만
실행한다. Busy면 정확히 같은 witness를 `Pending`으로 복귀시키고 owner는 계속
`AwaitingDecision`이다. 따라서 Busy 뒤에는 Cancel 또는 같은 capability의 명시적
Confirm 재시도만 가능하다. 다른 동시/reentrant 호출은 상태를 바꾸지 않고
거부한다. Busy 복귀 CAS 자체가 실패하거나 capture/검증에서 예외가 나면 같은
decision을 닫고 `TerminalFailure`로 간다. 어떤 예외도 Busy 출력이나 새 재시도
권한을 만들지 않는다.

`NewGameDecisionCapabilityV1`과 `HubNewGameRearmCapabilityV1`은 외부 필드가 없는
불투명 참조형으로 두고, owner가 가진 `ConditionalWeakTable`의 readonly witness와
일치할 때만 유효하게 한다. 기본값, 미등록 생성, 복제된 값, 다른 owner/epoch,
소비 완료 행은 모두 상태 변경 전에 거부한다.

### Q-A: 후속 controller/cursor 슬롯

`HubMenuPresenterV1`에는 기존 `_controller/_cursor/_retainedIntent/
_consumedTransferIntent/_state`를 수정하지 않는 별도 `SuccessorEpochSlot`을 둔다.
이 슬롯은 exact Q-B 예약 토큰, epoch/token, fresh controller/cursor,
focus/hover, 단계와 증거를 묶는다. 종료 슬롯은 append-only 이력으로 이동한다.

```csharp
internal object PrepareNewGameSuccessor(
    NewGameConfirmationOwnerV1 confirmationOwner,
    HubMenuIntentHandoffOwnerV1 handoffOwner,
    InputRouter router,
    object reservedSuccessor);

internal void CloseNewGameInteraction(
    NewGameConfirmationOwnerV1 confirmationOwner,
    object epochToken);
```

`PrepareNewGameSuccessor`의 검사·게시 순서는 다음과 같다.

1. exact same GameObject Q-A/Q-B, 기존 `_router`, `_latch`의 bound adapter/router,
   현재 `_handoff`, owner와 예약 epoch를 참조 동일성으로 검사한다.
2. 기존 controller의 notice 상태를 읽고, 불변 `_handoff/_notification`으로 새
   controller를 만든다. 기존 notice가 Dismissed면 새 controller에 기존
   dismissal 메서드를 한 번 적용한다.
3. `UiSemanticFrameCursorV1.Create(_router)`로 fresh cursor를 만들고 focus는
   `NewGame`, hover는 null로 설정한다.
4. old controller/cursor/intent 슬롯을 이력에 연결한 뒤
   `SuccessorBaselinePendingWitness`를 마지막에 게시한다. factory가 즉시
   `Ready`여도 presenter Ready로 보지 않는다.
5. exact acknowledgement를 Q-B에 돌려주며, Q-B commit 뒤에만 후속
   `AwaitingIntent` 슬롯이 활성화된다.

`Update`는 successor pending 분기를 기존 `AwaitingBaseline -> Ready` 승격보다
먼저 처리하고 반환한다. `FixedUpdate`도 successor pending을 기존
`Ready/IntentRetained` 분기보다 먼저 처리한다. 이때 허용되는 호출은 exact
fresh cursor의 `TryAdvance(exactRouter, out frame)` 하나뿐이다. `false`는 계속
격리, `true`는 연속성이 이미 검증된 첫 프레임을 해석 없이 폐기한 뒤 witness를
소비하고 successor Ready를 게시한다. 그 프레임에서는 `Interpret`, `Activate`,
hit test, notice dismissal을 호출하지 않는다. 예외, skip, foreign cursor/router,
증거 훼손, disable/destroy는 모든 후속 슬롯을 terminal close한다.

후속 `Activate/TryTakeRetainedIntent/LateUpdate`는 현재 successor slot만 사용한다.
기존 최초 intent/request 필드는 계속 immutable history이며, 반복 Cancel도 새
slot과 새 epoch record를 추가할 뿐 과거 값을 지우거나 덮어쓰지 않는다.

## intake, Cancel/rearm, 실행 commit 순서

intake는 다음 순서로 실패 전 상태 변경을 최소화한다.

1. owner lifecycle과 exact authored cohort를 검사한다.
2. request를 검증하고 Q-B의 실제 issuance를 claim한다.
3. presenter가 request receipt와 자신의 immutable `_handoff`가 같고 latch의
   adapter/router가 전달 인자와 같은지 내부 메서드로 검사한다.
4. adapter의 read-only root 인증을 통과한다.
5. 그 뒤에만 `ProfileNewGameConfirmationV1.Capture(adapter, router, owner,
   epochToken)`을 호출한다. Busy는 같은 issuance를 유지하고 take/rearm하지 않는다.

Cancel/rearm은 `Cancelled` 이후 명시 호출로만 진행한다.

1. Cancel winner가 issuance/epoch/cohort에 묶인 rearm capability를 만든다.
2. owner가 `Rearming`으로 전이하고 Q-B가 checked successor epoch를 먼저
   예약한다. 이 시점 이후 실패는 epoch를 되돌리지 않는다.
3. Q-B가 Q-A preparation을 동기 호출하고 exact acknowledgement를 받는다.
4. Q-B가 orthogonal successor를 `AwaitingIntent`로 commit한다.
5. owner가 acknowledgement를 검증하고 rearm capability를 소비한 뒤
   `AwaitingRequest`로 간다. 부분 실패는 세 참여자를 닫고 old epoch로 복귀하지 않는다.

`ConfirmedProfileResetRequestV1`은 Hub가 아니라 **Input.Unity 소유**의 getter가
없는 internal sealed 참조형으로 둔다. 이는 현재 assembly 방향과 C4 Review의
opaque boundary를 따른다. 하위 `ProfileNewGameConfirmationV1`만 확인에 사용한
capture에서 이를 발행하며, 해당 형식의 `ConditionalWeakTable` witness는 exact
adapter/router/root, opaque Hub owner/epoch token, interaction/decision generation,
capture와 consumed CAS를 묶는다. Hub는 이를 보관·반환할 수 있을 뿐 C1 identity,
root 또는 document를 꺼내거나 교체할 수 없다. 직접 생성자나 같은 값은 권한이
아니다.

```csharp
internal ConfirmedProfileResetRequestV1 CommitForExecution(
    ConfirmedProfileResetRequestV1 request);
```

이 메서드는 owner `ConfirmedReady -> ExecutionCommitted`,
decision/retry/rearm 무효화, Q-A/Q-B interaction close를 모두 마친 뒤 C3의
confirmed-publication CAS를 소비한다. 마지막으로 lower witness를 committed로
표시하고 동일 opaque request를 executor에 반환한다. 이 표시는 executor 소비와
구분된다. lower request의 실제 one-consumer CAS는 executor가 guard 진입 뒤 C1
직전에 한 번만 수행한다.
실제 identity 추출은 향후 Approved C4의 Input.Unity executor 내부에서만
`ProfileNewGameConfirmationV1`의 one-consumer 검증과 함께 일어난다. 중간 실패 시
request를 반환하지 않아 C1 Begin 호출 권한이 생기지 않는다. C3 자체는 C1
Begin이나 C2를 호출하지 않는다.

## typed executor report와 실행 후 상태

현재 C1의 실제 형식은 Profile-owned internal `ProfileResetDiskResultV1`이다.
그 `Validate()`는 `Busy/ConfirmationStale + NoBarrier`,
`ReloadRequired + DefaultCommitUncertain`, `ManualRepairRequired +
ManualRepairRequired`, `DiskPrepared + DiskPrepared`만 허용한다. 그러나 Profile은
Hub에 friend를 제공하지 않으므로 Hub가 이 struct나 scalar outcome을 직접 받거나
재구성해서는 안 된다.

향후 Approved C4 executor가 실제 C1 `Begin` 반환 직후 전체 result를 검증하고,
그 호출에서만 Input.Unity-owned `ProfileResetExecutionReportV1`을 발행한다. 이
report는 getter 없는 불투명 참조형이고, executor의 `ConditionalWeakTable`
witness가 다음을 readonly로 묶는다.

- 실제 소비된 원래 `ConfirmedProfileResetRequestV1` 참조와 consumed 증거
- exact adapter/router/root, opaque C3 owner/epoch token 및 execution generation
- 실제 C1 반환 `ProfileResetDiskResultV1` 전체와 그 exact Outcome/DurableState
- C1 전 실행 guard 및 barrier 가능성 분류
- one-shot report CAS와, Busy/Stale일 때만 존재하는 fresh-C3 handback 증표

값 enum, caller boolean, 파일 재탐색, 예외 종류나 복제된 C1 struct는 report를
발행할 수 없다. 이 형식과 발행기는 C4가 아직 Review이므로 **이 C3 구현
allowlist로 미리 추가하지 않는다**. C3 owner에는 다음 intake 의미만 고정하고,
구체 lower 형식 추가는 C4 승인 뒤에 한다.

```csharp
internal NewGameConfirmationOutcomeV1 ReportExecution(
    ProfileResetExecutionReportV1 report);
```

검사·전이 순서는 다음과 같다.

1. owner가 정확히 `ExecutionCommitted`인지, report가 이 owner/epoch와 원래
   consumed confirmed request 및 execution generation에 묶였는지 검사한다.
2. lower issuer 등록과 report one-shot CAS를 검사한다. 실패는 terminal close이며
   outcome scalar만 보고 복구하지 않는다.
3. 실제 C1 `Busy/NoBarrier` 또는 `ConfirmationStale/NoBarrier`인 경우에만
   fresh-C3 handback을 소비한다. old confirmed request/identity는 이미 소비된
   이력으로 남고 다시 Begin이나 capture에 쓰지 않는다.
4. handback으로 Q-B checked successor epoch와 Q-A fresh cursor quarantine을
   수행한 뒤 `AwaitingRequest`를 게시한다. 새 Q-B NewGame 실제 take/issuance가
   들어온 다음 `AcceptNewGame`이 **새 capture**를 수행한다. report 자체가 old
   request를 재시도하거나 identity를 교체하지 않는다.
5. `ReloadRequired`, `ManualRepairRequired`, `DiskPrepared`, C2가 호출된 행,
   default/unknown/corrupt/exception 및 barrier가 게시됐거나 게시 가능성을 배제할
   수 없는 모든 행은 `TerminalFailure`로 닫는다. Cancel, rearm, old-menu 상호작용,
   same-session retry는 영구 금지한다.

ReportExecution의 동시/reentrant 호출은 report CAS 단일 승자만 허용한다. 승자가
fresh successor를 게시하기 전 실패하면 epoch/history는 되돌리지 않고 C3/Q-A/Q-B를
모두 닫는다. terminal 결과를 받은 뒤 teardown이나 다른 report가 Busy/Stale
복구를 만들 수 없다.

## 구현 정지 조건과 검증 초점

새 public ABI, friend/asmdef, reverse reference, prefab/scene, map enable, action
교체, C1 Begin/C2 호출은 필요하지 않으며 허용되지 않는다. 현재 계약과 코드
사이의 추가 차단 충돌은 발견하지 못했다. 다만 구현자가 기존
`IntentRetained/RequestTaken` 또는 최초 retained/taken 필드를 재사용·초기화해야
한다면 즉시 Astra에 중단 보고해야 한다.

최소 검증은 `AC-M5D7QC3-001/003/005/006/008/009`에 맞춰 실제 take가 아닌
동일값 request 거부, Confirm/Cancel 경쟁 단일 승자, 반복 Cancel의 checked epoch,
factory 즉시 Ready 격리, 첫 true frame 폐기, 부분 rearm terminal close, execution
commit 이전 권한 무효화를 직접 증명해야 한다. `AC-M5D7QC3-007/008`에는 initial
Busy와 Confirm Busy의 상태/API/capability 분리, report provenance, exact
Busy/Stale NoBarrier 단일 복구, old confirmed request 재사용 금지, 모든
possible-barrier terminal closure를 추가한다. C3L 관찰 시험이 독립 수용되기
전에는 이 설계로 C3 구현 또는 런타임 수용을 주장할 수 없다.
