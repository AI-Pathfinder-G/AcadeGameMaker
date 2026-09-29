# C3 실제 발급 intake와 같은 취소의 필수 보존 증거

- 날짜: 2026-09-29
- 상태: 제한 시험·최소 보정 제안 — 실행·결함 실증·구현 승인 아님
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/009/010`
- 기준: [상위 r3 검수](../verification/2026-09-29-c3-upper-r3-luna-runtime-review.md).
- 실제 조회 owner SHA-256 `4723C8CE75022BE00911D7E5BEBCD4BE349D7902BD3442BDA0642AA1EDE37742`,
  Q-B `9B9AAFA1A0898E27FAADC6CAC389838E170D1527A177893060DF5397723C34E7`.
- 기존 신규 Edit/Play 시험·실제 임시 환경·180초 제한만 사용한다. 코드·Unity 수정/
  실행, 새 registry/assembly/API 및 정상 권한 제조는 수행하지 않았다.

## 실제 권한을 보존하는 최소 intake 행

두 독립 실제 cohort A/B는 서로 다른 임시 root, 정상 preparation/latch/Q-A/Q-B와
실제 opaque handle을 가진다. 각 행을 독립 fixture로 분리하고 baseline의 정상
발급 결과를 보관한다. clean mismatch는 정상 실제 권한을 뒤이어 1회 소비해
거부가 원본 권한을 소모하지 않았음을 확인한다.

| 최소 행 | 실제 조작/호출 | 기대 증거 |
| --- | --- | --- |
| OwnerBeforeTake | A Q-B의 C3 take에 B owner만 전달 | false, A live request/proof·taken/history·양쪽 pending/display 비변경; A 정상 take/intake 1회 성공 |
| PresenterBeforeTake | A Q-B take에 B presenter만 전달 | 같은 비소모와 A 정상 성공 |
| RouterBeforeTake | A Q-B take에 B router만 전달 | 같은 비소모와 A 정상 성공 |
| OwnerAfterTake | 실제 A handle을 B owner intake에 전달 | 정상 foreign 거부, A pending/Consumed/history 비변경; A 정상 intake 성공 |
| RouterAfterTake | 실제 A handle/A owner에 B router만 전달 | 정상 foreign 거부 후 A 정상 intake 성공 |
| RootAdapterAfterTake | 실제 A handle/A owner에 다른 실제 root의 B adapter 전달, A router 유지 | adapter 불일치로 foreign 거부 후 A 정상 intake 성공; caller root 입력을 추가하지 않음 |
| RootWitnessCorruption | 실제 A take 후 A adapter `_launchRootWitnessB` 한 필드만 B의 실제 임시 root로 변경 | 실제 A intake의 lower root 조회가 원본 cohort/root 불일치로 실패, owner/Q-A/Q-B 전체 폐쇄; display/confirmed 없음 |
| OldEpochAfterRearm | 실제 A intake/Cancel/Rearm·첫 frame 폐기 완료 후 새 actual take, old handle을 같은 owner에 먼저 전달 | old 거부, 새 pending/handle 비소모, 새 actual handle 1회 성공; 최초 taken/retained/과거 epoch 역사 보존 |
| ActualTopologyAfterTake | 정상 A take 후 presenter의 기존 ConfigureForAuthoring에 B router/latch와 동일 A UI 객체를 제공 | 정상 API가 만든 현재 구조 불일치. intake는 관찰 전에 terminal close해야 함. 현재 구현의 폐쇄 여부는 아래 별도 정적 후보로 기록 |
| TakenProofCorruption | 실제 A take 후 Q-B `_takenRequestProof` 한 필드만 null | 실제 A intake winner의 ValidateState 실패로 owner/Q-A/Q-B 종료, display/confirmed 없음, 최초 실제 taken/history는 보존 |
| EpochProofCorruption | 실제 A take 후 owner `_epochProof` 한 필드만 현재 epoch+1 | 실제 A intake winner의 Validate 실패로 같은 전체 폐쇄; 세대 재활성화 없음 |
| StateProofCorruption | 실제 A take 후 owner `_stateProof` 한 필드만 Inspecting | 현재 선검사/폐쇄 결손 후보. 요구 기대는 인증된 actual intake 이후 전체 폐쇄이며 아래 최소 보정 판단 전 실행 통과로 기대하지 않음 |

음성 proof 주입은 위 정확한 단일 필드 한 개씩만 바꾸고, 실제 runtime 경계를
호출한다. callback/registry/receipt/capture를 정상 값으로 만들어 등록하지 않는다.
정상 launch/take는 반사로 제조하지 않는다. 잘못된 root의 독립 adapter 행은
**foreign adapter 거부**의 실제 증거이며 같은 adapter의 내부 root witness 훼손
증거라고 과장하지 않는다. RootWitnessCorruption은 기존 두 root witness 중 정확한
한 필드의 불일치를 actual root getter에 전달하는 별도 음성 행이다. 이 행 역시
단일 손상 오류 주입이며 새 root 또는 원본 권한의 정상 값을 제조하지 않는다.

duplicate component는 DisallowMultipleComponent가 실제 AddComponent를 차단할
수 있다. 엔진이 중복을 생성하지 않으면 그 결과는 구성 차단 증거일 뿐 upper
intake topology mismatch 실행 증거가 아니다. 복제된 컴포넌트를 강제 주입하거나
로그를 억제해 실제 생성했다고 주장하지 않는다. 위 normal authoring API 행은
구조 불일치를 실제로 만드는 대안이며 원래 UI 참조는 읽어서 그대로 전달한다.

## 런타임 최소 보정 후보 — 시험과 분리

Owner `:147`의 AcceptNewGame 선검사는 `HasPendingIssued`를 사용한다. `:143`의
HasPendingIssued는 `_state == _stateProof`까지 검사한다. `_stateProof`만 잘못된
실제 pending handle은 선검사에서 foreign로 거부되어 `:152` Validate와 `:158`
Terminal에 들어가지 않는 정적 경로다. 반면 `_epochProof`는 이 선검사에 없으므로
정상 actual handle 인증 뒤 Validate에서 실패해 Terminal로 이어진다. 실행으로
재현한 결함은 아니며 source상 확인된 분기 차이만 보고한다.

필요한 최소 방향은 AcceptNewGame의 **foreign/duplicate/old handle 참조 인증**과
인증된 현재 pending의 **lifecycle/proof 무결성 검사**를 분리하는 것이다. exact
adapter/router/configured/pending handle 참조의 clean 인증은 상태 proof 훼손으로
foreign가 되지 않게 하고, 이후 실제 operation winner의 try 안에서 현재 Validate를
호출한다. Q-B의 기존 registry/record 양쪽 pending 인증도 보존한다. 다른 API의
HasPendingIssued를 일괄 느슨하게 만들거나 임의 후보를 actual handle로 등록하지
않는다. foreign 거부 후 원본 handle 성공 행이 함께 통과해야 한다.

구조 행 역시 별도 후보다. 현재 owner `:318` Validate는 저장된 Cohort와 같은
GameObject/Q-A↔Q-B를 확인하지만 presenter의 **현재 router/latch**를 재대조하지
않는다. 기존 presenter ConfigureForAuthoring은 현재 router/latch를 정상 API로
바꿀 수 있고, intake의 `:153` consume/immutable handoff 비교만으로는 이 현재
구조를 확인한다고 볼 수 없다. 필요한 최소 방향은 인증 후 Validate에서 기존
`presenter.MatchesConfirmationCohort(owner,Q-B,router)`의 현재 exact adapter/latch/
router 결속을 확인하는 것이다. 이는 저장된 root를 새 환경에서 조회하거나 권한을
추가하는 보정이 아니다. 런타임 수정이 필요하면 아스트라의 별도 제한 승인과 루나
검수 후 수행하며 이 시험 제안으로 자동 승인하지 않는다.

이 현재 살아 있는 결속 요구는 상태별로 명시 제한한다. `AwaitingRequest`,
`Inspecting`, `AwaitingCaptureRetry`, `AwaitingDecision`, `FreshDecisionRequired`,
`ConfirmedReady`, `Cancelled`, `Rearming`에서는 기존
`MatchesConfirmationCohort`가 반드시 참이어야 한다. `ExecutionCommitted`,
`TerminalFailure`, `Closed`에서는 정상 `CloseConfirmationInteraction`이
presenter의 현재 결속을 닫아 이 조회가 거짓일 수 있으므로 **현재 살아 있는
결속 요구만** 제외한다. 종료 상태에서도 원본 Cohort의 presenter/Q-B/adapter/
router 참조, 같은 GameObject·Q-A↔Q-B 역할 연결, state/stateProof 범위와 일치,
epoch/epochProof 및 token/tokenProof의 기존 무결성 검사는 모두 유지한다.
종료 상태를 통째로 Validate에서 제외하거나 현재 결속 검사를 무조건 요구하여
정상 종료 뒤 State 등 읽기 getter의 검증을 깨뜨리지 않는다. 기존 `:318`
무결성 조건을 보존하고, 위 살아 있는 상태에만 현재 결속 조건을 더한다.
종료 상태의 정상 getter 조회와 살아 있는 상태의 실제 구조 음성 행을 별개로
검증해야 한다.

## 같은 실제 Cancel 전후의 완전한 관찰 snapshot

실제 prompt/decision이 존재하는 하나의 cohort에서 Cancel 직전과 반환 직후,
**중간 Step/Update/Rearm 없이** 아래 같은 관찰값을 복사하고 비교한다. 별도
cohort의 여러 취소 증거를 합쳐 한 번의 완전 snapshot이라고 주장하지 않는다.

| 관찰 범위 | 실제 자료/대조 |
| --- | --- |
| 세 파일 | Primary/Previous/Temp 각각 존재·정확 bytes·길이·SHA-256. 초기 M/D/invalid 세 자료 또는 M/D/Ø 배치를 두 행으로 나눠 present 및 absent 보존을 모두 확인 |
| 관찰/결정 | 실제 display capture 참조, 기존 request/issued/epoch 및 최초 Q-A retained/Q-B taken·history 참조. decision은 소비됨, Cancelled와 실제 rearm capability 발급만 허용 |
| adapter 메모리/reset | 읽기 `_currentSession`, `_resetLifecycle`, `_resetCutoverTerminal`, `_resetGeneration`, `_resetReceipt`, `_untransferredCandidate`, `_restartBootstrap/_restartResult` 및 실제 launch root/receipt 원본. null/default이면 absence를 그대로 기록 |
| router 메모리/reset | 실제 `_actions`, `_resetOldActions/_resetNewActions`, `_resetCutoverLifecycle/_resetCutoverTerminal/_resetCutoverGeneration`, `_mode`, `_modeEpoch`, `_captureSuppressed`, fault/disposed flags의 읽기 대조 |
| 입력 자산/맵 | 같은 GameInputActions 및 `.asset`, 실제 Gameplay.Get()/UI.Get() map 참조, map/action별 enabled, 현재 binding effectivePath/overridePath·mask. Cancel 전후 자산/맵 교체·활성 전환·설정 변경 없음 |
| 양측 영수증 | adapter CurrentReceipt와 launch receipt witness, router CurrentReceipt, latch CurrentHandoff/notice 역사, presenter immutable handoff/notification의 값과 참조. getter가 요구하는 실제 API 검증도 실행 |
| 장면 | SceneManager의 실제 sceneCount, 각 loaded scene의 handle/path/name/isLoaded, active scene handle, 해당 cohort 실제 GameObject scene 및 객체 참조/활성 상태 |
| 실행 상태 | 이 fixture가 실제 보유한 router mode/frame receipt와 terminal teardown/camera/gameplay driver 참조·활성 상태를 기록. 확인용 Hub fixture에서 RunSession/목적지 인스턴스를 만들지 않음 |

모든 private snapshot은 exact declaringType/FieldType로 읽기만 한다. null인
CurrentSessionProfileV1을 getter 호출로 억지로 생성하거나 fake 글로벌 snapshot을
만들지 않는다. 실제 current cell이 있으면 그 기존 API의 canonical document,
settings/input/tutorial/progression 및 generation을 복사해 비교한다. 현재 순수 Hub
launch fixture에는 reset current cell이 없을 수 있으므로 absence 보존+실제 파일
스냅샷+입력 값 보존 및 **C3 Cancel에 C1/C2/reset/scene/run 호출이 없는 구조**를
합쳐 한정 증거로 기록한다. 존재하지 않는 전역 run/메모리를 전수 관찰했다거나
실제 gameplay session 보존 시험을 했다고 주장하지 않는다. AC005 전체 해석은
아스트라가 이 실제 synthetic 범위 증거와 한계를 확인해야 한다.

정상 Rearm은 Cancel snapshot **이후 별도 구간**에서 검사한다. 파일/메모리/
maps/receipt/scene 상태는 그대로이고 epoch와 새 controller/cursor/current 슬롯의
변화 및 과거 append만 기대한다. 취소 자체에 새 epoch가 이미 append된다고
잘못 기대하지 않는다. 기존 Play 실제 입력 fixture의 신규 취소 snapshot 행과
Edit intake 음성 행에만 추가하며 180초 제한, 정확한 selector/expected names,
실행 전후 source SHA, actual exit 및 실패/건너뜀/판정보류/중복·누락 대조를 유지한다.
이번 문서에는 실제 시험 결과가 없다.

## 인증과 진입 사이에서 늦어진 경쟁 호출의 추가 경계

추적은 `REQ-M5D7QC3-001/003/005/007`, `AC-M5D7QC3-001/003/005/009/010`이다.
위 두 런타임 지문을 다시 조회하여 그대로임을 확인했다. 다음은 현재 소스의
정적 반례 후보이며, 해당 실행 순서를 재현했다는 보고가 아니다.

| 현재 owner 경계 | 실제 현재 줄 | 늦어진 호출이 정상 승자를 손상할 후보 |
| --- | --- | --- |
| Confirm/Cancel | 인증 205, 진입 212/248, 검증 215/251, witness CAS 216/252, 전체 폐쇄 catch 230/259 | 두 호출이 같은 실제 decision의 앞선 인증에 성공한다. A가 결정 소비와 상태 전이를 마치고 Exit한다. B가 뒤늦게 Enter에 성공하면 오래된 witness CAS 실패 또는 이미 바뀐 상태에 따른 내부 실패가 정상 A의 결과를 Terminal로 닫을 수 있다 |
| RetryIntake | 앞선 인증 163, 진입 164, 검증 168, CAS 169, 전체 폐쇄 175 | A가 재관찰을 마치고 retry를 닫은 뒤 B가 이전 인증 결과로 진입하면 Closed/current retry를 다시 검사하지 않는다. Inspecting은 A의 finally에서 0으로 복구되어 B의 CAS가 성공할 수도 있어, 재관찰과 상태 전이를 다시 수행할 후보도 있다 |
| Rearm | 앞선 인증 268, 진입 269, CAS 273, 새 epoch 전환 282, 폐쇄 286 | A가 새 epoch를 완성한 뒤 B의 이전 witness Consumed CAS가 실패하면 새 epoch까지 Terminal로 닫을 수 있다 |
| AcceptNewGame | 앞선 인증 148, 진입 149, Validate 152, Q-B consume 153, 폐쇄 158 | A가 실제 handle을 소비하고 관찰 결과를 발급한 뒤 B가 들어오면 Q-B의 exact pending 인증 거부가 owner catch 안에 들어가 정상 A 결과를 닫을 수 있다. Q-B의 앞선 인증은 224, Consumed CAS는 226이다 |
| OpenFreshDecision/CommitForExecution | 앞선 조건 235/291, 진입 236/292, 실제 작업 240/296, 폐쇄 243/307 | 같은 앞선 조건을 통과한 호출이 뒤늦게 들어오는 동일 구조다. 전자는 이미 null인 fresh를 재사용할 수 있고, 후자는 이미 커밋된 lower request의 예약 실패를 Terminal로 처리할 수 있다 |

`Enter` 320은 `_operation`의 0→1 CAS이며 `Exit` 321은 다시 0을 쓴다.
따라서 게이트가 1인 동안에만 겹친 호출을 거부하는 현재 경합 시험으로는
**앞선 인증을 통과한 호출이 다음 0 구간에 진입하는 순서**를 설명할 수 없다.
앞선 인증 결과 자체를 예약으로 취급하지 않는다.

### 최소 보정 순서와 거부 분류

새 토큰·등록부·외부 호출 통로를 만들지 않고 owner의 위 일곱 진입점에 같은
순서를 적용한다. 최초 외부 인증은 실제 CWT 등록 객체와 변경 불가능한
Owner/Issuer/Presenter/Router/Token/epoch 등 해당 호출의 원본 문맥·참조를
확인한다. `_stateProof`, `_epochProof`, taken proof 등 무결성 증거는 이
외부 인증에서 비교하지 않는다. 따라서 등록되지 않은 후보나 다른 실제
소유자/라우터/adapter의 호출은 게이트·소비·전체 폐쇄 없이 거부한다.

1. 앞선 외부 인증에서 실제 원본 객체와 witness를 잡는다. 이때의 생존 판단은
   빠른 거부일 뿐, 내부 작업을 허가하는 최종 판단이 아니다.
2. Enter에 성공한 뒤, 동일 capability/handle과 **동일 실제 witness 참조**를
   다시 인증한다. 현재 epoch/current pending 및 실제 완료·소비 이력을
   함께 대조한다. 정상 완료에 따른 old/used/consumed/대체된 권한이면 이
   지점에서 비변경 거부한다. 이 거부는 Terminal catch 바깥에 둔다.
3. 여전히 실제 미소모 권한이면 내부 Terminal catch 안에서 Validate와
   현재 슬롯·예상 상태·양측 연결을 확인하고 기존 CAS/consume/작업을 수행한다.
   실제 pending의 proof 손상은 여기서 전체 폐쇄한다. 게이트 보유 중이고
   재인증을 통과했는데 CAS가 실패하면 정상 경쟁 패자로 간주하지 않는다.
4. 2의 비변경 거부와 3의 성공/실패 모두 바깥 finally에서 반드시 Exit한다.
   Enter 자체 실패는 그 finally에 들어가지 않아 타 호출의 게이트를 해제하지 않는다.

형태를 보여 주는 의사 코드는 다음과 같다. 새 제품 메서드 이름이나 공개
인터페이스를 요구하는 코드가 아니며, Terra가 기존 메서드 안에서 구현한다.

```text
original = authenticateExternal(actualArgument, actualContext)
Enter()
try:
    recheckSameActualAndRejectCompletedOutsideTerminalCatch(original)
    try:
        Validate()
        validateLivePendingSlotsAndExpectedState(original)
        existingReserveConsumeAndWork(original)
    catch:
        existingWholeClosure()
        throw
finally:
    Exit()
```

외부 재인증을 단순히 기존 조건 전체의 복사로 만들면 안 된다. 특히
`HasPendingIssued` 143의 `_state == _stateProof`를 그대로 재사용하면
`StateProofCorruption`이 정상 외부 거부로 남는다. **실제 미소모 발급과
현재 슬롯 손상**을 **실제 정상 소비로 끝난 발급**과 구분해야 한다.
실제 등록된 동일 문맥의 현재 epoch 발급이 아직 미소모인데 owner/Q-B pending
슬롯 또는 기대 상태가 어긋나면 내부 무결성 오류로 전체 폐쇄한다. 정상 완료
이력이 확인된 오래된 권한이면 현재 proof 검사에 들어가지 않고 거부하여,
지연된 패자가 완료된 정상 작업이나 후속 새 epoch를 닫지 못하게 한다.

Accept에는 필요할 경우 기존 Q-B `Issuances` CWT의 정확 IssuanceWitness,
그 readonly Record/Handle/Token/Epoch 연결과 `Consumed`를 **읽기만 하는
내부 보조 판정** 하나를 최소 범위로 허용하는 안을 제안한다. 원본 issuer/
owner/presenter/router를 모두 명시 전달하고 실제 record가 현재 또는 기존
append 이력의 동일 record인지 확인한다. 미등록/외부 객체, 동일 문맥의
이미 소비된 과거 발급, 동일 문맥의 미소모 현재 발급을 구별한다. 후자의
슬롯 이상은 내부 Validate로 넘긴다. caller가 pending을 자동 선택하거나
handle·등록부·증거 객체·식별자·권한 상태를 읽어 받는 API를 만들지 않는다.
이 보조 판정은 등록·소비·역사 변경을 하지 않으며 Q-B consume의 기존
proof 검사와 단일 Consumed CAS를 대체하지 않는다. 동일 record/token의
소비 완료와 새 epoch의 append 이력을 유지한다.

Decision/Retry/Rearm은 이미 있는 실제 CWT witness의 Used/Closed/Consumed와
원본 epoch/current 참조를 이용한다. 정상 완료 이력으로 설명되지 않는
미소모 actual witness의 슬롯 불일치를 외부 후보와 함께 거부하지 않는다.
Busy는 결정 State 및 retry Inspecting을 0으로 돌려 현재 권한을 유지하므로,
늦게 진입한 호출이 재인증 뒤 정상 bounded retry가 되는 것은 허용된다.
완료·취소·새 epoch로 넘어간 권한의 재사용은 허용하지 않는다.
OpenFreshDecision은 앞서 잡은 실제 `_fresh` 참조와 현재 동일 참조/대기
상태를 재검사하고, Commit은 동일 `_confirmed` actual request와 준비
상태를 재검사한다. 이미 완료된 작업의 늦은 거부를 lower 예약 catch 안에
넣지 않는다. 이 두 관련 진입점을 남겨 동일 창을 유지하지 않는다.

허용 변경 후보는 기존 owner 런타임과, 위 읽기 보조가 필요할 때의 기존 Q-B
런타임, 기존 허용된 신규 집중 시험 파일에 한정한다. presenter/asmdef/friend/
public ABI/assembly 방향, lower 계약·권한 발급, 신규 seam은 변경하지 않는다.
이는 아스트라의 제한 구현 승인과 루나의 독립 설계·소스 검수 전 제안이다.

### 실행 증거와 구조 보장의 분리

기존 실제 권한의 정상 중복·늦은 호출 및 정상 두 작업 경합 행에서 정상
승자의 결과·pending·confirmed·새 epoch가 살아 있고 이후 허용된 실제 다음
작업이 성공함을 확인한다. Confirm/Cancel 혼합, Retry 완료 후 동일 retry,
Rearm 완료 후 old rearm, intake 완료 후 동일 issued, fresh 열기/commit 완료
후 같은 호출을 각각 기록한다. Busy 뒤 같은 실제 cap의 정상 재시도도 남긴다.
게이트가 1인 동안 거부된 경합과 작업 완료 뒤 거부된 늦은 호출은 다른 행으로
분리한다. 단일 `_stateProof` 음성 손상 actual intake의 전체 폐쇄 행을 반드시
유지하여 이 수정이 무결성 실패를 비변경 거부로 숨기지 않음을 검증한다.

정상 외부 두 스레드를 동시에 시작한 경합이 해당 창의 정확한 지연 위치를
확정하지 못하면 그 한계를 원장에 적는다. 반복 성공만으로 이 순서를 재현했다고
주장하지 않는다. 현재 허용 throwpoint는 예외만 던지며 앞선 인증과 Enter
사이에 대기하거나 외부 callback을 실행하는 통로가 아니다. 이를 임의의
대기 delegate로 바꾸거나 반사 gate monitor를 추가해 결정적 일정을 제조하지
않는다. 추가 seam 없이 위 정상 실행 증거와, 일곱 진입점 전부의 post-Enter
재인증·catch 배치·항상 Exit·완료 이력 우선 거부를 정확 최종 소스 줄/SHA로
검수하는 구조 보장을 합친다. 정확 창의 결정적 실행 증거를 대신했다는
표현은 쓰지 않는다. 기존 180초 제한/정확 선택 이름/실제 종료·건너뜀·실패와
전후 지문 원장은 그대로 적용하며 AC003 전체 판정은 루나·아스트라가 맡는다.
