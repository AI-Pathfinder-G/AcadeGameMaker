# C3 취소의 효과 호출 경로 — 솔 구조 증거

- 날짜: 2026-09-29
- 작성자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-003/005/006/007`, `AC-M5D7QC3-003/005/006/009/010`
- 근거: [제한 구현 승인](../approvals/2026-09-29-c3-intake-cancel-and-gate-correction-approval.md),
  [취소 필수 증거 제안](../proposals/2026-09-29-c3-intake-and-cancel-required-evidence.md)
  SHA-256 `D55936D54119282834909ED4C13C3CA683C981984D9541AAA138290B01BCBE2C`.
- 상태: 최종 R5 동결 소스의 독립 구조 증거로 갱신. 통합 원장
  `artifacts/c3-upper-r5-frozen-source-manifest.json` SHA-256
  `B9DC34D615ECE99775C927FB434D88BB53F0B2F9E356BF8FECB2EB2D4483C9EE`와
  현재 실제 14개 파일의 지문이 전부 일치했다. 코드·시험 변경/Unity 실행
  및 실제 snapshot 시험 결과가 아니다. 아래 줄은 이 정확 R5에 적용한다.

## 실제 조회 자료

| 파일 | 조회 시점 SHA-256 |
| --- | --- |
| `Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043` |
| `Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5` |
| `Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900` |
| `Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` |
| `Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` | `1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C` |

파일 경로의 공통 접두사는 `Assets/AcadeGameMaker/`이다. 기존 Confirm 콜백
보고서 `2026-09-29-c3-confirm-callback-reachability-sol-evidence.md`의
`AFBDC066B55B36A16590A793EE30FC7EE75142A85B0570042DC46428EB3D89D5`는
그 당시 소스의 역사로 보존하며 이 문서가 갱신하지 않는다.

## 정상 실제 Cancel의 닫힌 호출 범위

owner `:269–288`의 정상 취소는 아래 순서만 가진다.

| 실제 호출/줄 | 실제 효과와 다음 호출 |
| --- | --- |
| `AuthenticateDecision` `:215–218`, 호출 `:271/:274` | 기존 `Decisions` CWT 조회, 실제 owner와 Used 확인. Enter 뒤 같은 실제 witness를 재확인하며, 정상 소비 거부는 내부 Terminal catch 밖이다 |
| `Enter` `:359`, 호출 `:271` | `_operation` 0→1 CAS. 다른 reset 게이트나 입력 모드를 변경하지 않음 |
| `Validate` `:354–358`, 호출 `:277`; `ValidateLiveDecision` `:220–221`, 호출 `:278` | 원본 Cohort·같은 GameObject·역할/state/epoch/token 증거, 현재 presenter 결속, 실제 current decision/token/epoch/generation/display/issued·기대 상태 검사 |
| decision 예약 `:279–280` | 실제 witness.State 0→2 CAS, Used=true, 현재 `_decision=null` |
| rearm 발급 `:281–282`, 생성자 `:115–116` | 빈 불투명 rearm 객체와 실제 역할·원본 epoch 참조의 RearmWitness 생성, 기존 `Rearms` CWT에 등록. 새로운 게임 실행/프로필 reset 권한 발급이 아님 |
| `SetState` `:362`, 호출 `:283` | owner `_state`와 `_stateProof`를 `Cancelled`로 함께 기록 |
| `Result` `:349–352`, 호출 `:284` | 취소 결과와 원본 rearm 참조만 담고 기존 `Results` CWT에 대조 행 등록. 결과 생성자 `:22–23`은 값/참조 대입만 수행 |
| `Exit` `:361`, 바깥 finally `:288` | `_operation=0` 기록. 정상 지연 거부와 내부 성공/실패 모두 해제하며 Enter 실패는 이 finally에 진입하지 않음 |

정상 취소 경로에는 `InspectInitial`, lower `Capture/Resolve`,
`CommitForExecution`, `Rearm`, Q-B successor reserve/commit, presenter 준비나
렌더 호출이 없다. Cancel은 재무장 권한을 돌려주지만 Rearm을 호출하지 않는다.
Cancel 이후 실제 새 epoch/controller/cursor와 첫 frame 처리는 별도 Rearm
작업의 효과이므로 같은 취소 직전·직후 snapshot에 섞지 않는다.

위 닫힌 실제 호출 경로에는 C1 Begin/reset 파일 쓰기·보관·교체, C2 메모리
cutover, adapter/router reset·입력 map 전환, SceneManager 장면 전환,
게임 실행 진입을 호출하는 단계가 없다. 부재 판단은 파일 전체의 금지 문자열
검색만이 아니라 실제 Cancel 몸체와 호출된 생성자/보조 몸체의 대조에 근거한다.
이 결론은 정상 취소의 제품 호출 범위이며 실제 snapshot 실행을 대신하지 않는다.

새 현재 결속 검사는 presenter `:225–228`의 기존 참조 비교와 latch
`HubEntryHandoffLatchV1 :31–32`의 필드 getter 및 owner `:132`의
`MatchesLaunchAdapter` 참조 비교만 호출한다. 여기서 adapter/root 환경 조회,
입력 asset/map 활성 변경이나 UI 렌더는 수행하지 않는다. owner `:356`의
기존 무결성 검사는 모든 상태에 유지하고 `:357`의 현재 결속 요구만
`_state <= Rearming`인 살아 있는 여덟 상태에 적용한다.
`ExecutionCommitted/TerminalFailure/Closed`의 정상 getter `:124`도 기존
무결성 검사를 수행하되 이미 닫힌 현재 UI 결속을 요구하지 않는다.

## 오류 폐쇄는 정상 취소 보존 판정과 분리

현재 owner `:286`에서 내부 실패가 발생하면 `Terminal :370–378`을 호출한다.
`InvalidateCallbacks :363–368`는 실제 decision/retry/rearm 증거를 폐쇄하고
현재 슬롯을 비운다. Q-B `CloseConfirmationInteraction :289`→`Close :299–304`는
요청 슬롯을 비우고 소유 상태만 닫는다. presenter `:347–353`은 현재 상호작용
폐쇄 표식, successor phase/hover, EventSystem 선택 해제 및 button 비활성을
수행한다. 이 오류 폐쇄는 정상 Cancel에서 호출되지 않으며, 정상 취소에
UI 선택/활성 변화가 있다고 기록하지 않는다.

Terminal의 `_confirmed != null && _configured` 분기 `:372–373`는 lower
`CloseExecutionCommit :269–280`을 호출할 수 있다. lower 경로는
`GetConfirmedRequestWitness :395–400`의 원본 CWT/역할/세대 확인,
`GetRequestLifecycle :350–354`, 해당 lifecycle의 lock,
`ValidateRequestHistory :356–375`, `CloseAuthenticatedFailure :377–386`이다.
기존 완료/폐쇄 이력 검사와 사전 구성된 폐쇄 사건 연결·witness.State=3만
수행하며 C1 Begin, 파일 capture, root 조회, reset 또는 메모리 cutover를
호출하지 않는다. 정상 AwaitingDecision 취소에는 confirmed request가 없지만,
오류 경로의 이 분기도 대조했다.

presenter 오류 폐쇄의 Unity UI 호출은 엔진의 선택/그래픽 알림이 발생할 수
있다. 여기서 임의로 연결된 외부 사용자 코드의 모든 효과까지 없다고 단정하지
않는다. 확인한 제품 몸체에는 장면/게임 실행/reset 호출이 없다는 제한 구조
증거다. 실제 fixture의 연결과 관찰 값은 독립 시험이 대조해야 한다.

## 실제 메모리 대상이 없는 합성 시험의 한계

승인된 실제 임시 환경의 같은 Cancel 직전·직후 비교는 세 파일의 존재/bytes,
현재 실제 adapter/router reset 필드, 실제 입력 자산·map·활성·바인딩, 양측
영수증·taken/retained/epoch 이력 및 실제 SceneManager/fixture 관찰 값을
복사하여 수행해야 한다. 현재 메모리 셀이 null이면 **부재 보존**만 비교한다.
getter로 메모리 셀을 만들거나 가짜 전역 게임 상태·session을 생성하지 않는다.

따라서 파일/입력/실제 관찰 대상 보존 시험과 위 호출 구조를 합친 C3 합성
범위의 AC005 증거이며 실제 진행 중인 게임 세션의 메모리·전역 실행 상태를
전수 보존했다고 주장할 수 없다. 실제 메모리 대상이 존재하는 fixture에서는
그 실제 대상의 기존 값과 세대를 별도로 복사해야 한다. 이 문서는 시험을
실행하지 않았으므로 각 대상의 보존 통과나 C3 전체 수용을 주장하지 않는다.
최종 R5의 일곱 진입점 재인증/catch/finally 및 Confirm 콜백 구조는
`2026-09-29-c3-r5-confirm-callback-and-gate-sol-evidence.md`에서 별도로
대조한다. 기존 컴파일 원장의 실제 Exit=0 기록은 참고 자료로 읽었으나
독립 재컴파일/Unity 시험 결과로 재사용하지 않는다.
