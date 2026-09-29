# C3 상위 세 경계의 제한 건축 반대 검토

- 날짜: 2026-09-29
- 검토자·실제 모델: 솔, `gpt-6-sol`
- 범위: 후속 세대 handshake, 첫 실제 cursor 프레임 폐기, 실행 직전 상호 폐쇄의
  읽기 전용 정적 검토. 시험/Unity 실행과 소스 변경 없음.
- manifest SHA-256: `0849AE5B55FDD448175A3534CB2A6E67F36102257FDFA8FEA27C0D39DAEF1175`
- Q-B SHA-256: `D8DDF69750D6BA4931CEA450EE637A29BF3D04C12EB760A8CA51249CA81A5A6E`
- presenter SHA-256: `827A7219F29DB5A9DEEE6A160319E0FB3F965CFBF16412A8BAAFEA37F2AEF74E`
- owner SHA-256: `4723C8CE75022BE00911D7E5BEBCD4BE349D7902BD3442BDA0642AA1EDE37742`
- 위 네 지문을 실제 파일에서 대조했고 모두 지정값과 일치했다.
- 기준: Approved C3, 실제 발급 설계 `28926347FAA0E247726177578C3B3A79358993F9ED0FC61C9C5886563C5F5CD3`,
  일관 단위 계획 `E68280CD03EA28B3F0CC9B60530FD88762BB27BBE24B966BAD2810D02DF1DC59`.

## 판정과 구체적 반례

제한 범위 P0=0/P1=1이다. 정상 최상위 재무장 순서는 실제 쌍을 결속하지만, 소비가
끝난 실제 예약의 늦은 presenter prepare 재전달을 차단하지 않는다.

`NewGameConfirmationOwnerV1.cs:276`의 Q-B 예약, `:277`의 presenter 준비,
`:279`의 Q-B commit, `:280`의 이전 epoch 역사 추가, `:281`의 rearm Used와
`:282`의 새 epoch/token 게시 순서는 정상이다. 실패는 `:286`에서 Terminal을 호출하고,
`:323`의 callback 무효화 및 `:337`의 Q-A/Q-B 폐쇄로 이어진다. Q-A `Previous`
슬롯 연결과 Q-B `TakenEpochRecord` 연결도 최초 retained/taken 역사를 덮어쓰지 않는다.

그러나 Q-B `HubMenuIntentHandoffOwnerV1.cs:249`의 `MatchesSuccessorReservation`은
현재 슬롯과 일치하는 **이미 Committed인 예약**도 true로 취급한다. 뒤의 owner
`IsActualRearm`은 `NewGameConfirmationOwnerV1.cs:263`에서 원본 등록/참조/숫자만
검사하고 Used·현재 Rearming·진행 중 operation을 검사하지 않는다. 실제 진행 중
검사는 별도 `:265`에 있지만 presenter 준비가 사용하는 이 경로에는 적용되지 않는다.

반례는 실제 첫 Cancel/Rearm을 완료하고 후속 세대의 실제 NewGame을 retain·Q-B로
이관하여 presenter 후속 슬롯이 `IntentRetained/Transferred=true`인 시점에, 그 첫
handshake의 **동일 actual reservation/token/epoch**를 늦은 호출로 다시
`HubMenuPresenterV1.PrepareNewGameSuccessor`에 전달하는 것이다. 새 권한 제조나
private 필드 변경은 필요 없다. `HubMenuPresenterV1.cs:239`의 이전 슬롯 조건을
만족하고 위 역사 예약 검사가 true이므로 `:242`의 fresh controller와 `:244`의
fresh cursor를 만든 뒤 `:246`에서 같은 epoch의 새 준비 슬롯을 설치한다.
owner가 해당 세대의 `AwaitingDecision/ConfirmedReady`에 있어도 prepare 경계는
이를 거부하지 않는다. 소비된 handshake가 새 준비 권한으로 재사용되는 문제다.

이로 인해 Q-B는 이미 taken인 슬롯, owner는 실제 요청/결정, Q-A는 새
AwaitingBaseline 슬롯로 갈라질 수 있다. 그 뒤 정당한 Cancel/Rearm도 Q-A 이전
슬롯의 `IntentRetained/Transferred` 조건을 만족하지 못해 종료될 수 있다.
이는 정적 경로 반례이며 실제 실행 재현을 수행하지 않았다.

최소 보정 방향은 Q-B 역사 예약 조회와 **미commit pending 예약의 prepare 권한**을
분리하는 것이다. 준비 경계에는 exact pending reservation, 미commit, 실제
`owner.IsRearmInProgress`의 old cohort 증거를 요구한다. Q-B commit과 ownerconsume
후 같은 reservation의 prepare 재전달은 비변경 거부해야 한다. 정상 poll/이력 대조에
필요한 committed 예약 참조는 보존한다. 기존 함수를 일괄 강화해 정상 역사 조회를
깨뜨리지 말고 준비 전용 live 경계를 적용해야 한다. 반례 시험은 actual handshake
결과를 보존한 뒤 재전달하여 Q-A controller/cursor/epoch/history와 Q-B/owner가
변하지 않고 원래 정상 결정이 유지되는지 확인해야 한다.

추적: `REQ-M5D7QC3-005`, `AC-M5D7QC3-005/006`의 예약 재사용·후속 세대 일관성.

## 첫 실제 프레임 폐기 — 추가 반례 없음

presenter `:107`/`:134`는 후속 슬롯 분기를 최초 cursor 승격보다 먼저 처리한다.
후속 생성자 `:48`/`:52`가 BaselinePending/AwaitingBaseline을 초기화하고,
`:275` 이후 Update는 factory Ready를 승격 근거로 사용하지 않는다. `:290`의
TryAdvance false는 대기이며 `:291`에서 실제 frame을 검증한다. 첫 true는
`:292` 분기에서 baseline을 소비하고 Ready로 전진한 뒤 `:297`에서 반환하여
`:300`의 Interpret를 호출하지 않는다. 활성화 `:330`과 버튼 표시 `:341`도
BaselinePending을 차단한다. frame/cursor 훼손 예외는 `:303`에서 Q-A/Q-B/owner
폐쇄 `:354`로 연결된다. 이 경계에서는 제한 정적 검토로 새 반례를 찾지 못했다.
추적: `REQ-M5D7QC3-005`, `AC-M5D7QC3-006`.

## 실행 직전 상호 폐쇄 — 추가 반례 없음

owner `:291`은 현재 ConfirmedReady와 exact opaque request를 요구하고 `:292`의
operation guard를 얻는다. `:296`의 lower reserve 뒤 `:298`에서 decision/retry/rearm/
pending authority를 닫고 `:299`에서 Q-A/Q-B interaction을 닫는다. `:300`의
ExecutionCommitted가 게시된 뒤 `:302`가 같은 request/reservation/owner/epoch/
generation으로 lower complete를 호출한다. request를 재생성하거나 identity를
추출하지 않는다. reserve 후 예외는 `:306`에서 lower close를 시도하고 `:307`에서
상위 Terminal을 호출한다. 최초 영구 taken/retained 역사 값은 권한 재활성화에
쓰이지 않는다. 이 경계에서는 제한 정적 검토로 새 반례를 찾지 못했다.
추적: `REQ-M5D7QC3-006`, `AC-M5D7QC3-008`의 실행 연결 전 부분 범위.

이 보고서는 루나 전체 독립 검수나 실제 집중/필수 회귀, 아스트라 통합 수용을
대체하지 않는다. C3 전체 AC-007/008 및 C4 수용을 선언하지 않으며, 모든 private
필드 동시 변조 방어로 범위를 확대하지 않았다.
