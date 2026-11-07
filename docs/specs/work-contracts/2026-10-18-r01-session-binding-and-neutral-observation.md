# R01 현재 세션 승인 연결과 앱 내부 입력 상태 관측

- 상태: Approved — 2026-10-18 아스트라 `gpt-6-astra`의 아래 최종 판정에 한정한 구현·국소 검사 승인. 솔 초안의 후보·미확정 표현은 마지막 절에서 선택한 범위로 확정하며, 새 실제 실행은 최종 바이트와 독립 검수 후 정확 판정을 받는다.
- 사용자 산출물: 사용자의 긴 경로 입력·추가 키 조작 없이 Root가 공개 화면에서 현재 승인 원문을 연결하고, 앱 내부 허용 입력의 현재 수신 상태를 확인한 뒤 관측기를 준비한다. SourceReady=false·R01 실제 플레이·G3/B1 FINAL·CH1 완주 미완료를 유지한다.
- 담당: 아스트라 계약·권한 호환·통합, 솔 한정 설계 및 승인 뒤 구현, 루나 독립 차이·경계 검수, Root 닫힌 외부 승인/연결 자료·공개 UI·원래 프로세스/Job·실제 실행, 사용자 게임 시작과 게임 조작.
- 사용자 지시: 긴 경로 직접 입력과 특이한 키 조작을 다시 요구하지 않는다. 입력 정책이나 이미 선택한 사용자 게임 방침도 재질문하지 않는다. 현재 신원·중립을 추정하거나 검사 없이 true로 발급하지 않는다.
- 조사 범위: 정확 `Chapter1BasicMovementEvidenceWindow.cs`의 Open/OnEnable/OnGUI/실행 경로/strict reader/TryPrepare, 정확 Observer의 입력·중립·Configure 경계, 현재 manager의 Unity 시작 인수, 설치 Input System1.20.0의 명명 API/동기화·Editor update 문서 및 Unity 공식 문서다. 큰 원장·전체 증거 검색·기기/API 호출·실행0이다.

## 확인한 원인과 실제 소비 경계

REQ-R01BIND-001. 현재 Window는 `_executionPath`와 `_message`를 SerializeField로 보존한다. Open과 OnEnable은 현재 실행의 경로/메시지 신원을 새로 결속하거나 자동 초기화하지 않는다. OnGUI는 편집 가능한 경로 칸과 준비 버튼을 노출하며, 준비 버튼은 `_armed` 또는 Play 전환 외에는 경로 부재/현재 승인 미검증만으로 비활성화되지 않는다. 따라서 과거 경로·실패 문구가 다시 보일 수 있고, 파일이 없는 경로도 실제 TryPrepare로 전달될 수 있다. 이것은 직렬화와 현재 UI 분기의 정적 사실이며 특정 GUI 포커스 실패의 플랫폼 원인을 확정하는 근거는 아니다.

TryPrepare는 ReadExecutionApproval→ValidateDiagnosticsApproval 뒤 `diagnosticsPrepared=true`를 설정하고, 그 다음 전체 tuple·정확10·GameView·프로세스·원시/소스/도구·임시 수집기 검사를 진행한다. 파일 읽기 자체의 실패는 코드상 아직 diagnosticsPrepared가 false인 분기지만, 유효 문서 판독 뒤 실패는 `diagnosticsPermanentlyBlocked=true`가 될 수 있다. 모든 준비 실패가 같은 소비 상태라는 설명도, 파일 읽기 실패는 사용자가 계속 눌러도 된다는 설명도 틀리다. 기존 계약의 실제 단회 실패와 원시는 그대로 보존한다.

Observer의 입력 원시 준비는 실제 Play와 보유 GameView 포커스 이후이며, 앱 입력 이벤트도 Play일 때만 기록한다. 따라서 기존 Observer의 과거/미등록 입력을 prePlay 중립 증거로 사용할 수 없다. 새 설계는 미무장 관측·준비·진단의 실제 소비를 건드리지 않는 별도 preflight가 필요하다.

AC-R01BIND-001. 과거 직렬화 상태·현재 세션 연결·실제 준비 시도/실패를 구분한다. 기존 실패 문구를 지우거나 단회/영구 차단을 초기화하지 않는다. 현재 실행에서 이미 소비/실패한 TryPrepare를 새 연결 기능으로 복구하지 않는다.

## 현재 프로세스로 결정되는 정확 연결 원문

REQ-R01BIND-002. 새 연결 자료의 경로 후보는 EvidenceRoot 아래 `r01-execution-binding-p{현재PID}-t{현재UTC시작DateTimeTicks}.json` 하나다. Window가 자기 실제 Process.GetCurrentProcess의 PID와 StartTime.ToUniversalTime().Ticks를 읽어 정확 이름을 계산한다. FILETIME과 혼용하지 않는다. Root가 같은 실제 신원에서 이 파일을 CreateNew·flush/close하고 닫힌 핀을 기록한다. 최신 파일 선택·디렉터리 열거·접두사 추측·과거 세션 경로·환경변수/숨은 IPC·manager 문자열 하드코딩 추가는 0이다.

연결 원문은 닫힌 schema/owner=Root/status=Approved/sessionId/project/현재 Unity exe/version/PID/startTicks/보유 GameView ID/현재 EvidenceWindow UInt64 ID/caseId=R01-A와 executionApproval FilePin을 담는 한정 스키마로 확정해야 한다. 이 원문은 승인 원문 위치와 신원을 묶는 선택 자료이며 finalApproval·입력 중립·진단 권한을 대신 발급하지 않는다. UInt64는 부동소수점 경유 없이 읽는다. actual execution-approval.json이 완성되기 전에 미래 핀을 써 넣지 않는다.

같은 기존 EvidenceWindow에 공개 ‘현재 승인 파일 연결’ 버튼을 두고 Root가 클릭한다. 공개 메뉴를 함께 쓰려면 focusedWindow가 바로 이 보유 객체일 때만 동작시키고 새 GetWindow·대체 객체 검색/생성을 하지 않는다. 사용자 TextField 타이핑 대신 경로·세션·검증 결과를 읽기 전용으로 표시한다. 기존 초기 Open은 실제 포커스였던 원래 GameView를 보유하는 규범을 유지한다.

연결 버튼은 현재 유휴/미무장·정확 원래 창·단회 상태를 확인한 뒤 결정 경로의 자료와 대상 승인 파일을 strict reader/길이 상한/조상 비링크로 읽는다. 대상 원문은 쓰기·삭제 공유 없이 같은 열린 handle에서 길이/바이트/SHA를 얻고 닫힌 연결 핀과 일치시킨다. 파싱과 해시가 서로 다른 읽기의 내용을 섞지 않도록 기존 strict reader에 필요한 한정 pinned-read 경계를 명시한다. 현재 프로세스·프로젝트·버전·session/case·보유 GameView·자기 Window ID·Astra/Root/User 역할과 실제 승인 flags·정확10을 기존 해석 규범으로 확인한다. 경로가 정확 현재 제어폴더의 execution-approval.json인지도 연결 원문의 현재 신원/Root 승인과 대조한다.

성공한 경우에만 현재 실행 전용 경로·길이/SHA·session·UI 객체 tuple을 저장한다. 이 연결 성공은 준비 성공이 아니다. 이후 Prepare 직전에 같은 핀과 신원을 재확인하며 변경/삭제/잠금/신원 불일치이면 준비로 들어가지 않는다. reload/프로세스·원래 객체 소실 때 연결은 무효화되며 지난 직렬화 경로로 자동 복원하지 않는다. 준비/진단 단회 상태는 초기화하지 않는다.

AC-R01BIND-002. 같은 PID 재사용을 시작 ticks로 배제하고, 잘못된 PID/ticks·과거session·다른GameView/Window·링크·중복키·부호/정수정밀도·크기/SHA·파일 교체를 실패 폐쇄적으로 판독한다. 준비 버튼은 이 연결과 현재 닫힌 승인 검증이 끝나기 전 비활성화한다. 사용자 경로 입력·키 질문·최신 파일 탐색을 해결 경로로 사용하지 않는다.

## 선택 검증과 실제 단회 준비의 분리

REQ-R01BIND-003. 연결 preflight는 path/원문/신원 검증과 표시만 한다. TryPrepare·ConfigureForAuthoring·collector 생성·진단 준비·RESERVE·영상·Play·SessionState 단회 쓰기·무장0이다. 파일 부재/미완성/검증 실패는 별도 ‘승인 연결 미완료’ 상태로 기록하고, 실제 TryPrepare 거절과 다른 문구·신원으로 표시한다. 닫힌 원문 준비 전 클릭해 관측기를 소비하지 못하도록 UI와 내부 진입 조건을 모두 둔다.

현재 `_message`·`_firstFailureJson` 등 과거 직렬화 값은 삭제하지 않고 ‘이전 또는 신원 미확정 기록’으로 분리 표시한다. 현재 연결/중립/준비 상태에는 현재 PID/startTicks/session과 결과 출처를 표시한다. 단순 문구 초기화로 복구된 것처럼 보이게 하지 않는다. 현재 영구 차단·기존 collector·실제 준비 실패는 그대로 Root의 원래 STOP/회수 경로다.

닫힌 승인·동적 영상/입력 권위·현재 중립과 실제 기한이 준비되면 Root가 공개 준비 버튼을 한 번 누른다. 기존 ReadExecutionApproval·ValidateDiagnosticsApproval·정확10·핀·실제 프로세스·보유 GameView·원시 부재·전체 소스·임시 collector 단회·UInt64 관문은 생략하지 않는다. preflight가 성공했다고 검사를 캐시 성공으로 대체하지 않는다. 실제 TryPrepare 실패 뒤 preflight 재연결로 재무장하지 않는다.

AC-R01BIND-003. 연결 실패는 실제 준비 소비0이고 실제 준비 시작 후 실패는 기존 차단/부분 원시 보존을 따른다. UI disabled만을 안전 경계로 믿지 않고 준비 함수도 검증된 현재 연결 핀을 요구한다. 기존 실패/단회 값을 지우는 코드가 없어야 한다.

## 설치 API의 확인 사실과 자동 중립의 한계

REQ-R01BIND-004. 설치 `com.unity.inputsystem@7a4e1a2a8194/package.json`은1.20.0이다. public `InputSystem.TrySyncDevice`는 장치의 현재 상태 event를 요청하고 지원되는 경우 다음 input update에 갱신한다. backend/platform 미지원은 false이며 장치가 added가 아니면 예외다. 공개 RequestSyncCommand 설명은 whole/current state event를 요청한다. 실제 이 호스트의 keyboard/mouse 지원·현재 event 도착은 호출하지 않았으므로 미확정이다.

`InputSystem.onAfterUpdate`는 대기 event 처리 후 호출된다. 설치 `InputUpdateType.Editor`는 EditorApplication.update 직전에 발생하며, `InputState.currentUpdateType`은 활성 버퍼 종류를 나타낸다. player/editor 버퍼는 구분된다. 설치 `InputState.updateCount`에는 Editor update 미지원 주석이 있어 이것만으로 Editor 신선도를 증명하지 않는다. `InputState.currentTime`과 input event 시각은 입력 시각 계열이며 QPC 값과 직접 같다고 비교하지 않는다. 각 시계를 원형으로 기록한다.

장치의 공개 native/added/enabled 및 객체 참조를 사용할 수 있다. 하지만 TrySyncDevice=true만으로 event 수신 성공·모든 control 해제·물리 중립을 발급할 수 없다. 동기화 요청과 event에는 여기서 확인한 공개 요청ID 결속이 없으므로 특정 event가 바로 그 명령의 응답이라는 인과를 확정하지 않는다. 단순 `.isPressed=false`는 포커스 손실/reset/disabled/기본 상태일 수 있어 신선 상태의 증거가 아니다.

AC-R01BIND-004. 설치 소스의 존재와 실제 호스트 지원을 구분한다. 전역 OS 키 수집·키 합성·ResetDevice·Disable/EnableDevice·QueueStateEvent/QueueEvent·설정 변경·가상 장치·prePlay Observer/collector 설치·항상true·UInt64 생략은0이다.

## 앱 내부 최신 수신 상태의 한정 후보

REQ-R01BIND-005. 기존 인간 중립 확인을 대체하려면 아스트라가 의미를 **현재 보유 GameView와 동일한 실제 장치에서 앱이 새로 수신한 허용 control 상태의 중립**으로 명시 호환해야 한다. 이 관측은 전세계 모든 키·OS 물리 상태·미래 Play 입력의 보장이 아니다. 사용자에게 경로나 추가 키를 요구하는 대체 경로를 만들지 않는다.

Root의 공개 ‘현재 입력 상태 확인’ 동작은 기존 실제 GameView 객체에 공개 Focus를 요청한 뒤 실제 Application.isFocused 및 focusedWindow==보유 GameView를 확인한다. 새 GameView 검색/생성0이다. 주 스레드의 제한된 Editor 전용 상태기로 장치 원본2(native keyboard/mouse)를 보유하고 added/enabled·참조를 고정한다. 기존 관측 대상 논리8에 대응하는 실제 control9(A/D/Space/좌우Shift/Q/E/마우스좌우)만 읽는다. Q/E·공격은 관측 대상이지 사용 허가가 아니다.

자동 갱신 후보는 요청 이전부터 명명 장치의 입력 event·afterUpdate·장치변경을 제한 구독하고, 실제 요청 InputState.currentTime/QPC·장치ID·원본 참조를 기록한 뒤 TrySyncDevice를 각1회 호출한다. 두 반환이 모두true여야 한다. 요청 뒤 같은 장치ID·원본에서 도착한 full StateEvent만 후보로 삼고, 요청 시각 이전·비유한 시각·미래 또는 판독 불명·필수 control 누락·delta만 있는 결과는 성공으로 보충하지 않는다. 실제 event에서 각 control 값을 읽고 값/knownMask/pressedMask/eventID·time/수신QPC를 한정 자료로 보존한다. 다른 장치나 원시 이벤트 전체 내용은 수집하지 않는다.

두 장치의 필요한 전체 control이 요청 뒤 새로 판독되고, 이후 같은 Editor onAfterUpdate까지 완료된 경우에만 후보 snapshot을 만든다. 그 시점 실제 Editor 버퍼와 event의 허용 control 상태를 대조하고 모든 control 해제·포커스·기기 참조/added/enabled/native·미무장·유휴·원래 객체가 유지됐는지 재확인한다. 자체 afterUpdate 관측 순번과 시각을 기록하며 onEvent 호출만으로 완료했다고 하지 않는다. native/full event라는 표지는 모든 내부 주입 가능성을 배제한 물리 입력 인증으로 과장하지 않는다. 표기는 ‘요청 뒤 수신·앱 내부 최신 상태’다.

유한 관측 후보는 동기화 요청 각1·기존 전체기한 안5초·자료32768바이트·최초 관측1묶음이다. 미지원/예외·full event 미도착·불명/기기 교체·포커스 소실·control 눌림·시간초과·reload/창소실이면 중립 미관측 또는 비중립으로 끝내고 반드시 구독 해제·원시 폐쇄한다. 자동 재요청·장치reset·키 질문·RESERVE0이다. 이5초는 사용자의 응답 제한이 아니라 앱 요청의 관측 예산이다. 실제 플랫폼에서 새 event가 요청 시각으로 식별 불가능하면 성공 기준을 느슨하게 하지 않고 그 한계를 보고한다.

중립 snapshot은 같은 세션·원래 프로세스/장치/창·입력 시각에만 유효하다. 추가 control event·기기 변경·포커스/Play전환·reload는 snapshot을 무효화한다. Root가 승인 자료를 작성하거나 EvidenceWindow를 클릭하는 포커스 변화가 있으므로, 오래된 결과로 inputNeutralObserved=true를 재사용하지 않는다. 설계 승인 시 **준비 직전과 최종 Play 인계에서 공개 Focus→최신상태 갱신을 어느 유한 횟수로 소비할지**, 관측창 클릭과 GameView 복귀 사이에 중립을 어떻게 재판독할지를 한 상태기로 확정해야 한다. 현 초안은 그 정책을 몰래 자동 갱신 무제한으로 만들지 않는다.

실행 승인 파일의 inputNeutralObserved는 실제 닫힌 관측 원문 핀과 위 유효성·갱신 규칙을 아스트라가 한 묶음에서 수용한 뒤에만 발급한다. 현재 파일 스키마·기존 인간 관문과 충돌하는 지점은 정확 호환 조항으로 닫고, 옛 승인·사용자 응답·준비 단회·미관측 입력 원시는 바꾸지 않는다. 최초 Play 뒤 Observer는 기존 방식으로 새 실제 player 버퍼·장치·GameView·Unity 수신 원시와 UInt64 의무를 검증한다. Editor snapshot을 실제 플레이 입력 원시로 대신 쓰지 않는다.

AC-R01BIND-005. backend미지원·도착없음·장치 교체/disabled/reset·focus손실·pressed·full/delta·요청전시각·중복/불명 event와 준비 클릭 뒤 snapshot 무효화를 실패 폐쇄적으로 검수한다. 실제 호출 전에는 자동 중립이 이 호스트에서 가능하다고 단정하지 않는다. 안전하게 끝나는 미지원 분기를 사용자 경로/키 요구의 재발로 대체하지 않는다.

## 승인에 필요한 정확 범위와 종료 조건

REQ-R01BIND-006. 이번 요청은 Draft1뿐이다. 구현 승인 후보는 Window 한 파일의 공개 연결/읽기전용 표시·현재 연결 preflight/버튼가드·현재/역사 메시지 분리·Editor 입력 snapshot 상태기다. 기존 Observer 변경·prePlay collector 설치는 제안하지 않는다. 패키지/API 가용성과 Editor assembly 참조의 정확 컴파일 범위를 승인 전에 닫고, 별도 helper/asmdef 변경이 필요하면 숨겨 추가하지 않는다.

실제 Window 소스가 바뀌므로 기존590 checkpoint와 manager의 정확 source/tool 핀을 그대로 실행할 수 없다. 기존 동결 checkpoint를 수정하지 않고 동일590 경로집합에서 승인된 Window 변경행의 새 길이/SHA를 결속한 새 checkpoint와 별도 manager 개정을 하나의 호환 범위로 닫아야 한다. meta·다른589·추가설정1 불변, 실제 컴파일/도메인 reload·새 Window 핀·원래 미무장 읽기·독립 검수 요구를 유지한다. ‘UI만 수정’이라는 이유로 소스 검사를 우회하지 않는다.

아스트라 판정 요청은 연결자료의 정확 파일명/닫힌 스키마·단일 작성자, 명시된 Window/필요 시 helper 경로, source/checkpoint/manager 정확핀 호환, snapshot의 앱 내부 의미·최종 갱신 시점/횟수·원시 소유, unsupported 안전종료를 한 묶음으로 고정하는 것이다. 그 뒤 한정 구현1·국소 컴파일/구문/결정적 경계검사1·같은 범위 보정1/재검사1·루나 독립1을 제안한다. 부재/과거신원/핀변경/실제단회 소비 경계와 입력 상태기의 의미 있는 실패 분기를 검사하며 전체 게임 회귀·새 진단 도구 사슬은 만들지 않는다.

새 실제 UI 수용1은 새 소스·핀·호스트·정확 실행 승인을 닫은 뒤이며 그 전에는0이다. 인간 관문 때문에 막히면 소진/미완료를 남기고 원래 STOP/회수한다. 제품 완주와 기존 필수 AC는 별도다. 종료는 사용자의 긴 경로/특이키 요구 없이 연결 가능 여부·앱 내부 최신 control 상태의 관측/미관측·단회 준비 미소비/소비·실제 원시·남은 결정으로 보고한다.

AC-R01BIND-006. 이 초안 작성 단계의 제품 코드/빌드/프로세스/입력 API 호출0을 확인한다. 승인 없는 live hotpatch·리플렉션·숨은 자동입력·서명/핀 우회·과거단회 해제·큰 원장/광범위 검색0이다. 사용자 직접 입력 반복을 해결책으로 다시 제시하지 않는다.

## 읽기 근거

- 현재 프로젝트 `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs`: SerializeField24, Open121, OnEnable147, OnGUI345, TryPrepare437, strict reader1227, 실행승인 투영1392.
- 현재 프로젝트 `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs`: Configure178, Play 이후 입력 준비217, event 수신278.
- 설치1.20.0 `InputSystem/Runtime/InputSystem.cs`: TrySyncDevice1791·onAfterUpdate2830; `Devices/Commands/RequestSyncCommand.cs`; `InputUpdateType.cs`; `State/InputState.cs`; `Devices/InputDevice.cs`; 설치 `Documentation~/background-behavior.md`.
- [Unity 공식 InputSystem API](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/api/UnityEngine.InputSystem.InputSystem.html) — 현재 게시본은1.20.1이므로 설치1.20.0 직접 소스와 구분한다.
- [Unity 공식 기기·배경 포커스 설명](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/background-behavior.md) — develop의 현재 문서를 설치판 직접 증거로 대체하지 않는다.

## 아스트라 최종 판정 — 한 묶음 구현 승인

REQ/AC-R01BIND-001..006을 다음 범위로 확정한다. 사용자 최신 지시에 따라 과거 인간의 경로 입력·반복 중립 답변은 새 실행의 선행조건에서 제외한다. 앱 동의·사용자의 실제 게임 시작/조작·기존 UInt64 진단·실제 준비 단회·원래 감독 기한은 유지한다. 아래 자동 관측은 앱 내부 수신 상태의 증거이며 물리 입력 전체의 인증이 아니다.

### 소유와 변경 범위

- 솔 단독 제품 변경은 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs`와 같은 폴더 `AcadeGameMaker.Combat.Authoring.Editor.asmdef` 두 파일이다. 후자는 실제 확인된 참조 누락을 해결하는 `Unity.InputSystem` 참조 한 항목 추가만 허용한다. 공개 API의 안전한 시그니처를 사용하며 unsafe 설정·별도 helper·Observer·패키지 변경은 없다.
- 솔은 새 `output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v17.json`을 작성한다. v16의 정확590 경로집합과 메타데이터 의미를 보존하고 위 두 파일의 실제 길이/SHA 및 새 산출물 식별만 갱신한다. 다른588과 추가 설정1은 불변이다. v16 원문은 덮어쓰지 않는다.
- 솔은 새 `output/chapter1-playable-r1/tools/chapter1-r01-user-play-entry-manager.v11.ps1`을 작성한다. v10 대비 제어 접두사 r9→r10, 체크포인트 v16→v17 및 두 검증 위치의 실제 길이/SHA만 갱신한다. 영상 C++v4/r7 실행파일·Job·명령 감독·기한·정리 동작은 불변이다. 위 새 두 경로는 승인 전 부재를 확인했다.
- Root는 실행 원문·연결 파일·사례 디렉터리·현재 소스 집합 대조와 실제 GUI를 소유한다. 솔 코드와 체크포인트 작성 종료 뒤만 Root가 최종 핀을 사용한다. 루나는 전체 묶음의 독립 검수를 맡는다.
- Root의 정적 준비에는 최종 v17/manager11 핀 및 실제591 집합 대조, 호스트 Editor 부재 확인, 존재하는 0바이트 잠금의 배타 읽기·비링크·목표 부재 확인 뒤 `UnityLockfile.r01-user-play-entry-20261018-r10.preserved`로 가역 보존1, 새 제어폴더 `r01-user-play-entry-20261018-r10`과 startup 원문1 작성을 포함한다. 실행 전 후보 핀 보정1은 이전 원문을 별도 보존한 뒤 가능하다. startup은 수동 질문 단계가 이 계약으로 대체됐음을 명시하며 미래 동적 신원·중립·컴파일 성공을 발급하지 않는다. 새 시작은 최종 핀의 별도 정확 판정 전0이다.

### 연결 파일과 소비 경계

연결 JSON의 정확 키는 `schema`, `owner`, `status`, `sessionId`, `project`, `unityExecutable`, `unityVersion`, `unityPid`, `unityStartTicks`, `gameViewInstanceId`, `evidenceWindowInstanceId`, `caseId`, `executionApproval`의13개다. schema는 `r01-execution-binding.v1`, owner/status는 Root/Approved, caseId는 R01-A다. FilePin은 기존 `path/length/sha256`이다. 결정 파일명은 REQ-002의 PID/UTC 시작 ticks 이름이며 최대32768바이트, Root CreateNew 한 번이다. 실행 승인 핀은 실제 닫힌 원문만 쓴다. 실행 승인 경로는 EvidenceRoot의 정확 현재 sessionId 제어폴더 아래 `execution-approval.json`이어야 하며 sessionId에 경로 구분자·상위 이동을 허용하지 않는다. 바인딩의 sessionId와 실행 승인 내부 sessionId도 같아야 한다.

연결 버튼은 닫힌 자료 작성 후 Root 한 번, 실제 준비 미소비 상태의 자료 보정 뒤 추가 한 번까지만 허용한다. 부재/미완성/검증 실패는 연결 미완료이고 진단/실제 준비의 소비가 아니다. 자동 재탐색·폴링·준비 재시도는 없다. 공개 추가 메뉴는 만들지 않는다. 준비 함수 자체가 현재 연결 검증을 요구하며 기존 TryPrepare를 우회하는 진입을 두지 않는다.

### 두 번의 관측과 준비 상태기

1. RESERVE 전 Root가 공개 입력 상태 확인을 한 번 요청한다. 원래 GameView로 Focus 후 REQ-005의 새 앱 상태 관측을 수행한다. 성공한 초기 관측만 닫힌 원문으로 Root가 수용한다. 파일명은 EvidenceRoot 아래 `r01-neutral-p{PID}-t{UTC시작ticks}-initial.json`이다. 초기 파일은 아직 미래 session 승인을 갖지 않으므로 sessionId를 발명하지 않고 실제 프로세스·창·장치 tuple로 결속한다.
2. 초기 성공 뒤 기존 Native 창 관측·사례 폴더11·RESERVE·영상 준비/최종 권위 절차를 진행한다. Root는 실제 초기 관측 핀과 현재 신원을 외부 정확 승인에 결속하고 실행 승인 및 연결 원문을 닫는다. 기존 실행 승인의 `inputNeutralObserved=true`는 초기 관측 사실과 **아래 준비 직전 재관측을 반드시 통과한다는 조건**을 함께 뜻한다. 과거 snapshot의 현재 유효성 또는 미래 재관측 성공을 주장하지 않는다. 기존 strict 실행 승인 스키마에는 임의 키를 추가하지 않는다.
3. Root의 공개 준비 요청 한 번은 즉시 TryPrepare를 호출하지 않는다. Window는 요청을 소비하고 원래 GameView Focus→새 관측 두 번째를 상태기로 수행한다. 두 번째 성공 뒤 동일 실행 승인 핀·프로세스·객체·유휴·기한을 재확인하고 같은 주 스레드 연속 흐름에서 기존 TryPrepare를 한 번 호출한다. 재관측 중 준비 버튼은 비활성이다. Focus 이동을 위해 사용자 키/버튼 해제를 요구하지 않는다.
4. 두 번째 결과는 같은 이름의 `-prepare.json`으로 Window가 CreateNew 한 번 기록하며, 실제 연결 sessionId와 실행 승인 SHA를 포함한다. snapshot 성공과 원문 폐쇄 사이 또는 실제 준비 진입 전에 장치/포커스/허용 control 상태 변화가 감지되면 성공을 재사용하지 않는다. 기존 소스/파일 검증 이후 실제 collector 구성 직전에도 현재 GameView·장치·허용 control·관측 무효화 여부를 재확인한다. 실패하면 기존 실제 준비 소비 위치에 맞게 실패를 보존하고 Root STOP으로 끝낸다.
5. 각 snapshot은 각 장치 TrySyncDevice 한 번, 최대5초·원문32768바이트다. 전체 합계 각 장치2회/총4회이며 무한 재갱신·세 번째 관측은 없다. 이벤트 구독은 활성 관측/준비 감시 동안만 유지하며 완료·실패·reload·창 종료에서 해제한다. 이벤트 원문 전체나 다른 키는 저장하지 않는다. 첫 snapshot 뒤 UI 포커스 변경은 최초 성공의 역사만 남기며 두 번째 관측을 대체하지 못한다. 준비 이후 Play 입력은 기존 Observer가 검증한다.

두 관측 파일은 schema=`r01-editor-neutral-observation.v1`, stage, 실제 PID/시작ticks/창ID, 장치ID와 native/added/enabled 결과, 요청별 반환·입력시간·QPC, 명명 control9의 알려짐/눌림 상태와 실제 값, 해당 full event의 ID/시간/수신QPC, Editor afterUpdate 자체 순번/시각, 최종 포커스·현재 버퍼 대조·성공/실패 이유·폐쇄결과만 담는다. initial의 sessionId/approvalSHA는 null이며 prepare에서만 실제 값을 넣는다. UInt64와 QPC를 부동소수점으로 변환하지 않는다. 수신 event의 Input 시계와 QPC의 값을 직접 비교하지 않는다.

미지원·눌림·새 full event 미도착·모호한 시각·장치/포커스 소실은 이번 관측 실패로 종료한다. 장치 재요청이나 사용자 경로/키 질문으로 돌아가지 않는다. 해당 실제 원시를 바탕으로 후속 개발 대안을 정하며 이 승인으로 자동 재실행하지 않는다.

### 유한 검사와 실제 실행의 연결

솔 구현1, 관리자의 구문 검사1, 의미 있는 결정적 경계검사 묶음1, 해당 범위 보정1과 영향 재검사1을 승인한다. 검사 자료/스크립트/결과는 새 `output/chapter1-playable-r1/preparation/r01-session-binding-validation/` 안에만 작성하며 제품 경로에 시험용 코드를 넣지 않는다. 최소 실패 사례는 과거 신원·핀 교체·미완성 연결·진단 소비 전후 구분·중복 준비·동기화 미지원·full/delta·요청 이전/미래 시각·장치/포커스 교체·눌림·두 번째 snapshot 실패다. 모의 결과를 실제 장치 지원 증거로 보고하지 않는다.

새 소스는 실제 Unity Csc가 변경된 Editor assembly를 컴파일하고 reload한 로그 및 새 DLL 길이/SHA에 결속해야 한다. 기존 cached DLL 또는 newCsc=0 증분 템플릿을 수용 근거로 재사용하지 않는다. 국소 컴파일1과 컴파일 오류의 범위 내 보정 후 재컴파일1까지 이 묶음 예산이며, 실제 Unity 시작은 새 입력 핀·호스트 확인 뒤 정확 승인으로 실행한다. 성공한 같은 세션에서 공개 연결/두 관측/단회 준비의 실제 수용1로 이어갈 수 있도록 승인자료를 묶고 별도 선행 진단 세션은 만들지 않는다.

루나 독립 차이·경계·컴파일 증거 검수1, 허용 보정 발생시 영향 재판독1, 아스트라 최종 통합1을 둔다. 기존 영상/감독 정적 수용은 영향 밖으로 재사용한다. 전체 게임 회귀·반복 계약 사본·문서 지문 재귀는 추가하지 않는다. 새 실제 실행 전 최종 파일 핀을 이 승인과 결속한다. 지금 승인은 제품 변경과 한정 검사이며 장치 호출/Unity 시작/게임 입력의 즉시 실행 승인이 아니다.

승인 참여: 아스트라 `gpt-6-astra`, 도구 기본 추론 설정. 솔 초안 전체, 실제 Editor asmdef의 참조 목록, manager v10의 체크포인트 검증 두 위치와 새 경로 부재를 직접 판독했다. r9 종료는 Root의 닫힌 원시 요약에 근거하며 새 동적 성공은 아직 없다. 이 묶음 완료 기준은 사용자 수동 경로·추가 키 질문 없이 실제 연결과 앱 상태 관측/단회 준비의 결과를 검증 가능하게 만드는 것이다. CH1 완주와 SourceReady 승격은 포함하지 않는다.

### 독립 검수 P1 한정 보정 — 동일 바이트 판독

Luna의 최초 독립 검수 P0=0/P1=1을 수신했다. Astra는 `ReadLockedApproval`이 원래 handle을 유지해 쓰기/삭제 공유를 막고 길이/SHA를 확인하지만 `ReadExecutionApproval(path,...)`로 다시 여는 코드를 직접 확인했다. 실제 파일 교체가 발생했다고 주장하지 않으며, REQ/AC-R01BIND-002의 동일 읽기 바이트 파싱 요건은 미충족으로 유지한다. 현재 r10 실행은0이다.

기존 내부 보정1과 별도로 이 P1에만 솔 추가 보정1·영향 재검사1을 승인한다. 읽은 `byte[]`를 기존 엄격 JSON 검증과 실행 승인 투영에 직접 넘기도록 Window 안에서 공통 파서/오버로드를 분리한다. UTF-8·중복키·닫힌 스키마·정수·크기 검사를 줄이지 않고, 파일 경로가 필요한 검사는 원래 경로에서 유지한다. 다른 파일 판독 호출의 의미와 단회/중립/권위는 바꾸지 않는다. 같은 원문에 대해 해시 확인 뒤 경로 재열기로 파싱하는 흐름이 없어졌는지와 엄격 거절 분기를 영향 검사한다.

수정된 Window 행만 v17에서 갱신하고 manager11의 해당 체크포인트 핀 및 Root의 미소비 startup/명령 핀을 결속한다. 기존 후보 원문은 별도 보존하며 허용된 startup 보정1을 사용한다. 변경 계약 핀도 현재 개정으로 갱신한다. Luna 영향 재판독1을 받아 잔여 P0/P1=0인 뒤 실제 시작을 정확 판정한다. 추가 전체 회귀·새 계약 사본·새 Unity 실행·장치 관측 예산은 발급하지 않는다.

## 2026-10-19 KST 재개 — 검증된 제품 유지와 새 실행 준비

r10은 외부 보안창 사용자 처리 대기 중 기존 capture 기한2026-10-18T15:10:02.631Z에 도달했다. Astra는 startup.jsonl 마지막 owned-stop 원문에서 case cleanup reserve, STOP QPC1955194722289, 영상exit1/Unity3758145814·원래 process/Job/event/Root handle 정리·양 pipe EOF/flush/close 완료를 직접 확인했다. readerStatus=Running은 종료 기록 순간의 값이고 살아 있는 관리자를 뜻하지 않는다. 도구 session41804 exit1은 Root 실제 관측이다. 자동 연결·두 관측·단회 준비의 성공 및 독립 통합은 보존하되 만료 권위는 재사용하지 않는다.

REQ/AC-R01BIND-006의 같은 사용자 행동·소유 범위에서 다음 정적 준비를 Approved로 확정한다.

- Sol 단독 새 manager.v12.ps1은 기존 manager.v11.ps1 43768/4597A14DA116D607540B9DF85DE98B02A3BB098657AF7731800E8F8D8284DC9F의 제어 접두사 r01-user-play-entry-20261018-r10→r01-user-play-entry-20261018-r11 한 곳만 교체한다. 새 경로는 동일 tools 폴더이며 승인 전 부재를 확인했다. 역치환 전체동일·구문1, 해당 작성 오류 보정1/재검사1, Luna 독립 차이 판독1이다. 제품590·v17·영상 C++/exe 변경0이며 기존60경계검사·제품 독립 검수는 영향 밖으로 재사용한다.
- Root는 r10 종료 요약 신규1과 닫힌 원시 핀/호스트 부재를 보존한다. 새 시작 전에 보안창 부재를 실제 읽기 전용 관측으로 확인한다. 현재590+추가1/591집합·v17·DLL 핀·host Editor 부재를 확인하고, 있으면0바이트 잠금을 배타 읽기/비링크/보존 목표 부재 확인 뒤 정확 UnityLockfile.r01-user-play-entry-20261018-r11.preserved로 가역 보존1한다. 새 r11 제어폴더1/startup 후보1을 작성한다. 미소비 핀 보정1은 이전 후보 별도 보존 후 허용한다. 정확 사례 base는 b1-unity-g3-owned-20261018-r11이다.
- 실제 시작은 최종 manager/startup/명령 핀과 Luna 차이 결과 뒤 별도 정확 판정한다. 새 프로세스 신원·전체18000/cleanup17990·새 사례1800 시계를 발급하며 r10 원점/입력/중립/실행 권위는 복제하지 않는다. 이는 종료된 사례 연장이 아닌 사용자 목표를 위한 새 명시 실행이다.
- 제품이 동일하므로 r10의 실제 새 Csc·DLL167424/437084871561B3859859208611DA4ECEB579F2D801A66F043E601D76B4F9DBD8와 Luna 컴파일 수용을 재사용한다. 새 실행에서는 현재 source590+extra1 일치, 실제 증분 빌드 종료0/reload/적재/컴파일 오류0와 같은 DLL 핀을 확인하면 충분하다. 새 Csc가 없다는 사실을 명시하며 새 컴파일로 꾸미지 않는다. 파일/assembly가 달라지면 이 재사용 조건은 성립하지 않는다.
- 후속 순서는 같은 R01 미무장 읽기1→initial 새 관측1→NativeREAD1/폴더11/RESERVE1→동적 영상 준비·최종 공개→새 실행 승인/결정 binding→prepare 새 관측1/TryPrepare1→가시화/사용자 시작 인계다. 각 동적 정확 핀 판정은 유지하되 새로운 제품 설계/선행 진단 반복은 없다. 이번 프로세스의 장치각2/단회·5초32768 한도와 실패시 원래STOP은 그대로다.

사용자 경로·키·중립 질문0, 보안 권한 대리0, 임의 기한 연장0이다. 외부 보안창이 다시 생기면 권한 지침을 따르고 원래 시계를 보존한다. 실제 새 플레이·진단·완주 성공은 아직 없으며 SourceReady=false다. 이 절 승인자는 Astra gpt-6-astra이고 기존 원문/실패/소진/성공의 실제 범위를 유지한다.
## 2026-10-19 오전 사용자 재개 — r12 한정 준비 승인

사용자의 ‘다시 진행해’에 따라 r11 만료 원문을 보존하고 새 실행을 준비한다. Astra가 r11 owned-stop을 직접 판독했으며 STOP QPC1990596347997, case cleanup reserve, Unity3758145814/video1·원래 프로세스/Job/event/Root handle 폐쇄, stdout20976/stderr0·EOF/flushclose를 확인했다. readerStatus=Running은 종료 당시 기록이다. 공개 연결/prepare/Play/진단0은 Root 관측이며 r11 권위는 소진됐다.

같은 REQ/AC-R01BIND-006 묶음의 정적 범위를 다음과 같이 Approved로 배정한다. 새 제품 변경이나 영상 도구 재진단은 없다.

- Root: r11 종료 요약 신규1, 닫힌 원시 핀·호스트 원PID부재/Editor부재 확인. 현재 소스590+추가1/591와 v17·기존 검수 DLL 일치 대조. 보안창 부재를 실제 읽기 전용으로 확인. 현재0바이트 잠금이 있으면 배타읽기·비링크·목표부재를 확인한 뒤 UnityLockfile.r01-user-play-entry-20261018-r12.preserved로 가역 보존1. 새 control r01-user-play-entry-20261018-r12 디렉터리1/startup 후보1, 미소비 후보 핀 보정1은 원본 별도 보존 후 허용한다.
- Sol: tools/chapter1-r01-user-play-entry-manager.v13.ps1 단독 작성. 기준 v12 43768/384CCE9DFF584C4D10B2BB6FB065BFC2B950A4DD2BDE2C08E8289EEB40F060D5에서 r01-user-play-entry-20261018-r11→r01-user-play-entry-20261018-r12 제어 접두사 한 곳만 교체. 역치환 전체동일·구문1·작성 보정1/영향 재검사1. 제품590·v17·영상 코드/실행파일 변경0.
- Luna: r11 종료 원시와 manager13 단일 차이 독립 판독1을 하나로 묶는다. 추가 제품 전체 회귀 없이 기존60경계검사·컴파일·r10 자동연결/두 관측/준비 통합을 영향 밖 재사용한다. Astra는 최종 startup/명령·파일 핀과 이 결과 뒤 실제 시작1을 정확 판정한다. 현재 시작0이다.

r12 실제 시작 이후 허용 절차는 같은 source/DLL·이번 증분 빌드/reload 확인→R01/Game/Evidence/미무장읽기1→새 initial1→예약 직전 보안창 부재 및 실제 화면 배치 확인→NativeREAD1/정확 새 사례 base b1-unity-g3-owned-20261018-r12 및10폴더/RESERVE1→동적 영상/새 실행승인/새binding→새prepare1/단회준비→사용자 시작 인계다. 실제 단계별 동적 핀 판정과 원래18000/17990/1800 기한·실패STOP을 유지한다. 종료된 r11 시계·중립·입력·권위를 재사용하지 않는다.

가시화는 예약 전에 같은 EvidenceWindow 제목줄 이동1로 먼저 확인할 수 있다. 최신 화면에서 main과 연결/준비 버튼 모두 접근 가능한 위치를 선택하며 도킹/리사이즈/새창/닫기0이다. 버튼이 보이지 않게 이동했다면 준비 소비 전에 같은 제목줄 원위치 복원1만 허용하고 추가 GUI 실험은 하지 않는다. 이 배치로 main과 준비 동작을 모두 접근할 수 없으면 예약을 시작하지 않고 구체 상태를 보고한다. 기존 r11의 ‘준비 뒤 이동1’과 중복 소비하지 않는다. 보안창이 있으면 그 화면을 우회 조작하지 않는다. 런타임 UI 도구 초기화는 해당 skill의 정상 절차를 사용하며 새 도구/API 우회0이다.

사용자 수동경로·키·중립 질문0, 보안 권한 대리0, 제품 및 필수진단 가드 삭제0이다. 앱 내부 관측 성공과 현재 사용자 게임 시작/완주는 구분하고 SourceReady=false를 유지한다.
## R01PLACE 최초 창 배치 제품 보정 — Approved

Astra는 실제 초안 `preparation/r01-window-placement-design-sol61.v1.md` 전체를 판독하고 REQ/AC-R01PLACE-001..003의 기본안을 이 계약에 통합 승인한다. 최초 요청의 output 하위 경로는 실제 파일 위치가 아니었으며 루트 preparation 원문을 근거로 한다. 공개 EditorWindow.position이 docked 창을 분리할 수 있다는 공식 문서도 확인했다(https://docs.unity3d.com/6000.0/Documentation/ScriptReference/EditorWindow-position.html). 이번 최초 배치에서만 그 공개 동작을 의도적으로 허용한다. 실제 OS 배치 성공은 별도 관측한다.

솔 단독 변경은 현재 Window164020/D7C1484CD1B7688F73C81D4463142CF39FC12B9E43D3CDBD07B540298F980472의 최초 Open 배치와 같은 파일의 순수 사각형 판정 함수다. 초기 원래 GameView 포커스에서 main 사각형과 pixelsPerPoint를 읽고 기존 armed/보유객체 충돌 검사를 유지한다. 신규 보유 확정 한 번에만 같은 Window의 maximized=false 및 minSize/maxSize/position을 설정한다. 이미 보유된 재진입·OnEnable·update에서 재배치0이며 추가 GetWindow/새창/닫기/리플렉션/OS·AX 우회0이다. 배치 실패도 보유 참조나 이전 단회 상태를 지우지 않는다.

현재 Root 관측 화면2560×1392의 단일 원점(0,0) 프로필만 사용한다. 화면 픽셀900×1100을 실제 배율로 논리 환산, main 오른쪽+32픽셀·y64픽셀·화면여백16픽셀로 후보를 계산한다. 실제 main/API 사각형·배율·크기는 유한 양수이며 화면 내·main 비중첩이어야 한다. 지원 배율은 이번1 또는1.25, 논리높이820 이상이다. 배율/좌표계 불명·범위 밖이면 초기 관측/예약 전에 배치 미완료로 끝낸다. min/max 설정은 같은 계산 크기로 고정하고 position 재판독으로 적용값을 확인한다. 설정 순서는 maximized=false→같은 min/max→position→재판독1로 고정하며 자동 재적용하지 않는다. main 기하·GameView 레이아웃은 변경하지 않는다. 화면 경계의 OS 장식 여백과 실제 버튼 가시성은 Root 화면으로 추가 확인한다. 속성 일치만으로 성공하지 않는다.

OnGUI 재배열·스크롤 변경 대안은 이번에 선택하지 않는다. 최초 메뉴 뒤 Root 실제 화면1에서 main·제목·연결/initial/prepare/cleanup/진단 버튼이 보이고 접근 가능한지 확인한다. disabled 버튼을 시험 누르지 않는다. 실패하면 원래STOP이며 drag/최소화/Raise/도킹 재시험0이다. 성공한 같은 세션에서만 기존 initial/영상/연결/prepare 절차로 이어간다. 사용자 수동 경로·키 질문0이다.

솔은 v17의590 중 Window 한 행만 변경한 새 `output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v18.json`을 작성한다. 다른589와 추가 설정1·asmdef·Observer·영상 v4/r7은 불변이다. 새 tools/chapter1-r01-user-play-entry-manager.v14.ps1은 v13 기준 제어 접두사 r12→r13, checkpoint v17→v18 및 두 검증 위치의 실제 길이/SHA만 갱신한다. 이전 원문은 보존한다. Root는 새 r13 제어폴더/startup1·현재591대조·호스트Editor부재·보안창부재 확인·존재하는0바이트 잠금의 정확 r13.preserved 가역 보존1을 소유한다. 미소비 startup 보정1은 이전 후보 보존 후 허용한다.

검사는 구현1·정적/관리자 파서1·실제 순수 기하함수 결정적 fixture1, 범위 내 보정1/영향 재검사1, Luna 차이/경계 독립1과 보정시 영향 재판독1로 묶는다. fixture는 초안의 정상/공간부족/배율1·1.25·1.5·2/비유한/음수/경계/중첩/재진입·armed를 포함한다. 시험 산출물은 `preparation/r01-window-placement-validation/`에 한정한다. 모의 기하는 OS 성공 증거가 아니다. 제품 변경에 따라 새 실제 Csc 종료0/reload/새DLL핀/현재591를 요구하며 cached DLL로 대체하지 않는다. 실제 컴파일1·컴파일 오류 범위내 보정 후 재컴파일1은 기존 감독과 정확 시작 승인 안에서 수행한다. 예비 진단 세션·전체 회귀·새 계약 사본은 없다.

새 실행은 최종 핀과 Luna 정적 결과 뒤 별도 정확 판정한다. r12 종료·초기관측 성공·도구오류는 보존하고 어떤 권위도 부활시키지 않는다. 계약·진단·전체/사례 기한·동적 파일 승인·사용자 실제게임 정책은 유지한다. Astra gpt-6-astra가 이 유한 구현 묶음을 승인하며 SourceReady=false·CH1완주 미완료는 그대로다.
## R01PLACE main 내부 최초 도킹 대안 — Approved 선택

Astra는 보완 초안 `preparation/r01-window-placement-diagnostic-sol61.v1.md`15386/9C2DD255DD7EA6A76B52F7E59FF767A53E8788709E869C3ABC3D6753422D56D9와 현재 OnDisable·직렬화 보유 필드를 직접 판독했다. 독립 popup의 도구 접근은 미입증이므로 그 배치 진단-only 재실행은 선택하지 않는다. 설치 IL에서 기존창을 즉시 반환하는 GetWindow 경로를 확인한 Sol 근거에 따라 공개 CreateWindow의 최초 도킹과 동작 상단 배치를 선택한다. 아래는 기존 새창/자기타입 조회/OnGUI 재배열 금지의 정확 한정 호환이며 그 밖의 금지는 유지한다.

최초 메뉴에서 실제 focused GameView 객체·ID·position 및 main position을 먼저 보유한다. 공개 Resources.FindObjectsOfTypeAll<자기타입>() 조회1로0/1/다수를 분류한다.0이면 공개 CreateWindow<This>(제목,typeof(UnityEditor.SceneView))1을 사용한다.1개가 이미 현재 보유 원본을 가진 경우 기존 동일원본 재진입 규범만 적용하고 닫기/재도킹/새 생성0이다.2개 이상·불명 상태는 실패한다. 내부 GameView/Inspector 타입 검색·리플렉션은0이다.

정확 새 프로세스 실행이 승인된 최초 진입에서, 유일 창이 미무장·collector없음·보유 GameView ref와 id 모두없음·receivedExecutionSha없음·initial/prepare/실제준비/미무장읽기/진단/적용 미시작·미소비·활성관측없음·현재연결없음을 모두 확인할 때만 미사용 복원창 Close1을 허용한다. SessionState의 기존 미무장 소비 표지도 false여야 한다. 직렬화된 ref/id가 남아 있으면 역사로 추정해 지우지 않고 이 분기를 거절한다. Close 후 기존 객체 소실과 자기타입 남은0을 공개 조회1로 확인한 뒤 CreateWindow1을 허용한다. 닫기 실패·잔존창은 즉시 실패하며 반복하지 않는다. 현재 OnDisable은 이 조건에서 FinishNeutral/Seal을 호출하지 않고 구독 해제만 수행한다. 별도 OnDestroy 추가0이다.

신규 창은 fixed popup position/min/max/maximized 쓰기를 하지 않는다. _placementReady는 기존 검사에서 제거하지 않고, 새 docked=true·main전후 정확동일·보유 GameView position전후 정확동일·원본ID/Unity객체참조 동일·관측창ID유효·유한양수 pane·논리 높이370이상 조건을 모두 만족한 경우에만 true로 설정한다. 폭도 상단 필수 동작의 결정적 레이아웃 검사에서 충분한 최소폭을 산출·검증하고 결과에 기록한다. 실패는 false 유지 및 원시1/예외이며 자동 위치보정·재도킹·popup대체0이다. main 전후/게임창 전후/실제pane/배율/ID·참조/각조건을 불변형식으로 성공·실패 각각 Open당 한 줄 기록한다. 과거 r13 target Equals 실패는 그대로 보존하며 새도킹에서는 근거없는 허용오차를 도입하지 않는다.

OnGUI는 짧은 현재 상태 및 연결·initial·case선택·prepare·cleanup·진단 동작을 상단370논리높이 안에 표시하고 상세 원시/긴 경로·역사 메시지는 하단 스크롤에 둔다. 기존 모든 버튼·핸들러·활성조건·상태기·단회 검사를 유지하며 성공/실패나 권위 정보를 삭제하지 않는다. Root가 실제 main 안의 Scene/Game 옆 관측탭과 필수 동작 접근성을 확인하기 전 initial/RESERVE0이다. 공개 관측탭 선택과 기존 GameView Focus에 따른 탭 전환을 허용하되 메뉴 재Open/새창 찾기0이다. 실제 탭전환으로 GameView 원본/기하가 변하면 실패한다.

솔 제품 변경은 Window 한 파일의 최초Open/분류·배치판정·원시/해당 OnGUI 표시 블록이다. asmdef·Observer·영상·패키지·다른589 및 extra1 불변. 새 checkpoint `b1-final-source-checkpoint-sol61.v19.json`은 v18의 Window 한 행만 갱신하고, 새 manager.v15.ps1은 v14의 제어 r13→r14·v18→v19 및 두 체크포인트 검증 핀만 변경한다. 이전 원문은 덮어쓰지 않는다. Root는 새 r14 control/startup1·현재소스/호스트/보안창 확인·0바이트잠금 exact r14.preserved 보존1을 소유하며 미소비 후보 보정1은 이전 원문 보존 후 허용한다.

기존 미소비 배치 보정1을 이 선택 구현에 사용한다. 국소 정적/파서/레이아웃·분류 fixture1, Luna 독립1을 묶으며 독립 지적의 이 범위 보정1/영향 재검사·재판독1까지 처음에 포함한다. fixture는 창0/미사용1/보유1/다수·모든소비플래그·Close잔존·도킹실패·원본/기하변경·부족pane·상단모든동작 포함을 다룬다. 산출물은 `preparation/r01-window-placement-validation/` 안이다. 새 실제 Csc1/새DLL/reload와 최초Open 실제UI1은 최종핀·독립검수 뒤 정확 실행 승인으로 진행한다. 컴파일 오류 한정보정/재컴파일1 외 선행 진단 세션·전체회귀0이다. 성공한 같은 세션에서 기존 자동연결·두관측·영상·단회준비로 이어가며 실패는 원래STOP한다.

Astra gpt-6-astra가 이 선택을 승인한다. 특정 공개 API의 존재가 실제 도킹 성공을 뜻하지 않는다. 사용자 경로·키 질문/자동게임입력0, 기존 기한/동적핀/UInt64단회/SourceReadyfalse를 유지한다.

## 2026-10-19 r14 만료 보존 및 r15 정적 준비 — Approved

Astra는 r14 owned-stop 및 closure-summary.json4546/BDE1B47A9BE67EC03A77149018FDEE9D83F866F3ABB16FE0517C0EB81E716E35를 직접 판독했다. STOP2265515226430, case cleanup reserve, Unity3758145814/video1, 원래 process/Job/event/Root handle 폐쇄와 양 pipe EOF/flush/close를 확인했다. readerStatus=Running은 종료 기록 순간의 상태다. 부분 영상114023158바이트는 보존하되 해독·연속 플레이 수용은 없다. 연결/prepare/Play/진단0이며 늦은 보안창 처리 응답은 만료 권위를 부활시키지 않는다. 원PID3 및 Editor부재와 보안창 부재는 Root의 이번 실제 관측으로 구분한다.

REQ/AC-R01BIND-006 및 R01PLACE-001..003의 같은 사용자 행동·소유 범위에서 다음 한정 정적 준비를 승인한다.

- Sol은 tools/chapter1-r01-user-play-entry-manager.v16.ps1을 단독 작성한다. 기준 manager.v15.ps1 43768/BD922EA5484052B6DA4D55651EADF7AFF23C09E473B7B3F067B07A2F9A536AB8의 제어 접두사 r01-user-play-entry-20261018-r14→r01-user-play-entry-20261018-r15 한 곳만 바꾼다. 역치환 전체동일·구문검사1·작성 보정1/영향 재검사1을 허용한다. Window·v19·나머지 제품·영상 C++/exe 변경과 재빌드는0이다.
- Root는 r14 동결 종료 증거와 부분 파일을 보존한다. 현재 v19의590행과 추가 설정1/591집합·DLL 핀, 호스트 Editor부재·보안창부재를 확인한다. 현재0바이트 잠금이 있으면 배타읽기·비링크·목표부재 확인 후 정확 UnityLockfile.r01-user-play-entry-20261018-r15.preserved로 가역 보존1한다. 새 control r01-user-play-entry-20261018-r15 디렉터리1/startup 후보1을 작성하고, 미소비 후보 핀 보정1은 이전 원문 별도 보존 후 허용한다. 새 사례 base b1-unity-g3-owned-20261018-r15는 예약 준비 전 부재를 확인한다.
- Luna는 r14 종료와 manager16 단일 차이 독립 판독1을 묶는다. 기존75경계검사와 제품 독립 검수는 영향 밖 재사용하며 전체 회귀를 추가하지 않는다. Astra는 최종 manager/startup/명령 핀과 잔여 P0/P1 없는 결과 뒤 실제 시작1을 별도 정확 판정한다. 이 절은 실제 시작 권위가 아니다.

제품과 소스가 동일할 때 r14 실제 새 Csc 성공·reload 및 DLL172032/5CC9F9FC5DCE923003002E3C8AADC120B6B74479AF5B5B78DAFC7AA9A385F069를 재사용한다. 새 실행에서는 현재591 일치, 실제 증분 빌드 종료0/reload/적재·컴파일 오류0와 동일 DLL 핀을 확인한다. 새 Csc가 없으면 그 사실을 기록하며 과거 Csc를 이번 실행으로 표기하지 않는다. 소스나 DLL이 다르면 이 재사용은 성립하지 않는다.

새 실행의 순서는 R01/Game/최초 도킹→실제 main 내부 탭·모든 동작 접근 및 원본/기하 확인→미무장 읽기1→새 initial1→예약 직전 보안창 부재 확인→NativeREAD1/새 사례 base와10폴더/RESERVE1→실제 동적 영상 준비·최종 공개→새 실행승인·binding→연결1/새 prepare 관측1/단회 TryPrepare1→GameView 및 사용자 시작 인계다. 실제 동적 핀 판정과 기존 전체18000/cleanup17990·사례1800·영상1790초, 실패STOP, 최초 Play 뒤 Pause/UInt64 관문을 유지한다. 같은 main 내부 탭 선택 외 popup 이동·Raise·재Open 실험은 하지 않는다.

r14 실제 도킹·초기관측 성공은 기능 근거로 보존하지만 새 중립·프로세스·기한·준비 성공으로 재사용하지 않는다. 사용자 수동경로·키·중립 질문0, 보안권한 대리0, 임의 시계 연장0이다. 실제 prepare와 플레이 완료는 아직 미수용이며 SourceReady=false를 유지한다. 승인자 Astra gpt-6-astra.

## R01PLAY 최초 Play 초기화·영상 실패 원시 보강 — Approved

Astra는 Sol 분석 preparation/r01-first-play-failure-analysis-sol61.v1.md10560/4100F96AB6BDD37135924AA7346639392F410DF8609BF03E846B63AD3EBB774B 전체, 실제 seal 및 원래 STOP을 판독했다. r15 최초 Play의 bridge·writer 미준비와 target scene/name 또는 hierarchy 가드 예외, 별도 영상 시각 비단조 실패를 보존한다. configuration 기본값은 Event(includeConfiguration=false)의 null 직렬화 결과이므로 구성 소실 증거로 사용하지 않는다. 실제 false 항목과 영상 거절 시각쌍은 미관측이며 두 실패의 인과를 단정하지 않는다. 준비 성공 통합은 준비 단계에만 남는다.

### REQ/AC-R01PLAY-001: 동일 승인 bridge와 최초 입력 경계

Sol은 준비할 때 승인된 EditorWindow의 Unity 객체 참조를 Observer에 직렬화하여 같은 객체를 복원할 수 있게 한다. Runtime은 Editor 타입을 참조하지 않고 기존 인터페이스로만 결속한다. 복원된 참조·현재 PID/시작시각·session/case·승인파일 동일 바이트 핀·GameView 참조/ID·host·source/configuration을 검증한 뒤 동일 bridge를 복원하고 실제 writer와 구독을 준비하는 단일 권위 경로를 둔다. 기존 Window OnEnable/EnteredPlayMode의 중복 Bind와 경합하지 않으며 논리 준비 단회와 관리 객체 복원1을 구분한다. 최초 실제 Play 입력 경계부터 관측해야 한다. 경계 후 준비를 기다리거나 입력/tick을 건너뛰고 연속0으로 표시하는 변경은 금지한다. 경계까지 준비 불가면 정확 실패한다. 관측 시작 전 검사와 입력 등록의 구체 순서는 실제 설치 Unity 수명에 맞춰 구현하며 소스 모형 검사를 실제 Unity 성공으로 간주하지 않는다.

기존 초기/prepare 새 관측·게임 포커스/장치 검증·같은 승인바이트·단회·Pause 범위·정리·기한을 유지한다. 보존 참조가 없어지거나 다른 tuple/창이면 자동 탐색 대체·새 설치하지 않는다. Window collector 재발견은 승인된 현재 session/host의 유일 객체에만 허용한다. 실패·소비·불완전 상태를 재로드로 초기화하지 않는다.

### REQ/AC-R01PLAY-002: 예상 Play 전환과 종료 분리

준비 성공 뒤 최초 사용자 Play 전환에서만 정상 reload를 한 번 허용한다. 예상 전환 표지와 현재 승인 신원을 보존하고 정상 중단/복원/실제 경계 활성 단계를 명시한다. 정상 reload의 OnDisable은 실패 봉인으로 오인하지 않되 실제 창 닫힘·Play 종료·재컴파일·가져오기·중복 reload·모호한 전이는 기존 실패/봉인으로 닫는다. 한 전환 중 구독 등록/해제와 writer 상태를 실제 값으로 기록하며 이전 등록을 없었던 것으로 계산하지 않는다. 정상 복원의 새 구독은 이전 구독 해제가 확인된 경우1만 허용하고, 첫 Play 입력 이후에는 이 복원 권위를 사용할 수 없다. 실파일 재열기·덮어쓰기·입력 관측 손실을 성공으로 숨기지 않는다.

### REQ/AC-R01PLAY-003: graph 활성 전 장면 귀속

현재 Chapter1EncounterLifetimeOwner의 staging 생성 경로는 활성화 전에 승인된 host.gameObject.scene으로 명시 이동하고 host/staging/target 장면 유효성·실제 이름/handle/path 및 target hierarchy를 검증한다. active scene 의존을 제거하되 CombatTarget·TransferTarget 공통 이름/계층 가드는 유지한다. 준비 시 정확 미저장 review 이름과 동일 host/scene 객체 신원을 직렬화해 보존한다. Play 전환에서 그 동일 미저장 scene 이름만 비었고 다른 모든 결속이 일치할 때에만 보존된 정확 이름 복원1을 허용한다. 비어 있지 않은 다른 이름·다른 scene·저장된 scene·host 불일치에는 적용하지 않는다. graph 활성 전에 복원 결과를 재검증한다. 임의 이름 주입·scene 저장/생성 대체·공통 target 가드 삭제·reaction/behavior의 후속 실패 무시는0이다.

### REQ/AC-R01PLAY-004: 유한 원시와 영상 독립 범위

수명 전환·bridge 검증·writer 준비·구독·최초 경계 및 graph 활성 직전 scene/name/hierarchy 값을 전이당1, 실행 전체 최대16행/행8192바이트로 기록한다. 실패 원시는 실제 false 항목을 구별하되 기존 비밀정보/게임입력 범위 밖 수집은 없다. 기록이 한도를 넘으면 성공으로 대체하지 않고 실패한다. 영상은 새 tools/b1-window-video-capture.v5.cpp와 b1-window-video-capture-r8.exe로 분리한다. v4의 비단조 판정 실패 직전에 현재/직전 frameTime·수신QPC·acceptedQPC·기존 콜백/제출 순서값만 최대1행/8192바이트로 추가한다. 기존 stdout 총한도와 strictguard·시각/PTS·상하한·수명·정리·기한은 모두 유지한다. timestamp 합성/오프셋/중복 폐기·무조건 무시·새 허용오차0이다. 다음 실제 실행에서도 영상이 실패하면 그 실행은 원래STOP이며 성공으로 우회하지 않는다.

### 소유·산출물·유한 검사

Sol 단독 제품 변경은 Assets/AcadeGameMaker 아래 Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs, Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs, Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs 및 정확 이름 보존 전달에 필요한 경우에만 Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs다. InputRouter·CombatTarget·TransferTarget·캐논·패키지·asmdef·다른 소스는 불변이다. 새 checkpoint evidence/b1-final-source-checkpoint-sol61.v20.json은 v19의 실제 변경 행만 갱신하고590행·추가설정1/591집합을 유지한다. 새 manager.v17.ps1은 v16 기준 r15→r16 제어 접두사·v19→v20 및 실제 checkpoint 핀·영상 v5/r8 경로/핀만 갱신한다. 기존 원문은 덮어쓰지 않는다.

구현1·국소 결정적 검사1·범위 보정1/영향 재검사1·Luna 독립1, 독립 P1에 한정한 추가 보정1/영향 재판독1을 처음부터 포함한다. 국소 검사는 실제 판정 함수의 동일/다른 tuple·소비·부분구독·reload1/2·writer실패·포커스/장치소실·종료, 빈이름/계층·다른 host/active scene·정확복원/거절, 영상 strictguard 유지와 원시1한도를 포함한다. 산출물은 preparation/r01-first-play-validation/ 아래로 한정한다. 전체 기능 회귀와 문서 사본 반복은 없다.

영상 빌드1은 기존 검수된 MSVC 명령/의존성의 소스·출력 경로만 바꾸어 compile1→성공시link1로 수행한다. project/Library/b1-video-cpp-build-r8/의 obj/dependencies/link-inputs.rsp/compile-plan.json/link-plan.json 및 각 stdout/stderr 로그를 신규 부수 산출물로 허용한다. 한정 보정 뒤 재빌드1까지 허용하며 기존 출력 덮어쓰기0이다. 제품 실제 Csc1/reload/새DLL핀 및 같은 새 세션의 최초 Play 전환1은 최종 파일핀·독립 검수 후 별도 정확 시작 승인으로 수행한다. 컴파일 오류의 한정 보정/재컴파일1 외 동적 실패 자동재시도0이다.

Root는 r15 종료 원문/부분영상 보존·현재 source/host/보안창 확인을 소유한다. 새 r16 control/startup1·존재하는0바이트 잠금 exact r16.preserved 가역 보존1을 준비할 수 있고 미소비 후보 보정1은 원본 별도 보존 후 허용한다. 새 사례 base는 b1-unity-g3-owned-20261018-r16이다. 실제 실행·새 초기/prepare·영상·사용자 Play·최초Pause/UInt64는 기존 동적 핀/단회/전체18000·사례1800 규범을 유지한다. 사용자 수동경로/키 질문·Root 게임입력0, SourceReady=false다. 이 부록은 구현 승인이지 실제 새 실행 성공 또는 시작 권위가 아니다. 승인자 Astra gpt-6-astra.
### R01PLAY 영상 빌드 기록 보정의 한정 해석

Root/Sol 보고에 따라 r8 최초 compile 프로세스1의 obj4993724·dependencies64077·stdout76224·stderr0은 보존한다. 환경사전의 계획 JSON 직렬화 오류로 종료코드 숫자가 보존되지 않았으므로 컴파일 성공으로 수용하지 않는다. link0/실행파일0 상태이며 영상 실제 실행은 없다.

기존 범위 보정·재빌드1 예산 안에서 환경사전을 일반 직렬화 객체로 변환하는 기록 보정과 compile 재실행1을 허용한다. 같은 project/Library/b1-video-cpp-build-r8/ 아래 신규 capture.correction.obj, compile-dependencies.correction.json, compile.correction.stdout.log, compile.correction.stderr.log 및 아직 없는 compile-plan.json을 사용한다. 이전 산출물은 덮어쓰지 않는다. 실제 compile 종료코드와 원래 프로세스 회수를 먼저 확실히 보존하고 성공0일 때만 보정 obj를 명시한 기존 최초 link1을 진행한다. link 계획과 최종 exe 핀은 실제 입력으로 결속한다. 이는 추가 재빌드 예산 발급이 아니며 기록 실패를 성공으로 전환하지 않는다. 제품 변경 범위·정적 검사·Luna 독립·실제 실행 승인 조건은 그대로다. 승인자 Astra gpt-6-astra.
### R01PLAY 영상 자기 소스 경로 P1 보정 — Approved

r16 CAPTURE 최초1은 source-contract-path 거절로 소진됐다. Astra가 v5.cpp의 kSource가 v4.cpp를 가리키는 실제 상수와 원래 owned-stop2375666031398/Unity3758145814/video1·process/Job/pipe 정리를 직접 확인했다. 제출 Prepare의 v5 경로는 의도한 새 소스와 일치했으며 엄격 경로 검사를 제거할 사유가 아니다. 이번 독립 검수에서 누락된 P1으로 기록하고 과거 정적 수용의 한계를 보존한다. r16의 실제 새 Csc·도킹·initial 성공을 유지하지만 collector/prepare/Play 성공은 없다.

Sol은 기존 독립 P1 추가 보정1을 이 수정에 사용한다. 새 tools/b1-window-video-capture.v6.cpp는 소진된 v5를 복사해 kSource 절대값을 자기 v6.cpp 경로로 한 곳만 바꾼다. 역치환 전체동일과 실제 파일경로/kSource/예정 Prepare.source/manager source 경로의 동일성을 결정적 검사1로 확인한다. 기존 시각 실패쌍 원시·엄격가드·수명·한도는 불변이다. v5/r8 및 기존 빌드 기록은 덮어쓰지 않는다.

이 동적 P1의 교정 증명을 위한 추가 최소 빌드1을 명시 승인한다. project/Library/b1-video-cpp-build-r9/에 새 obj/dependencies/link-inputs.rsp/compile-plan.json/link-plan.json과 compile/link stdout·stderr 로그를 작성한다. 기존 검수된 빌드 설정으로 compile1→종료0 확인시 link1, 새 tools/b1-window-video-capture-r9.exe를 만든다. 이번 추가 빌드 실패시 자동 재빌드0이다. 실제 종료코드·원래 프로세스 회수를 기록하고 출력 existence만으로 성공을 주장하지 않는다.

새 manager.v18.ps1은 v17의 제어 r16→r17·영상 v5/r8→v6/r9 및 실제 새 소스/실행파일 핀만 갱신한다. 제품3행/v20/새 Editor·Runtime DLL은 불변이다. Luna가 정확 소스경로 일치·역치환·manager 변경·빌드 실제 종료0을 영향 재판독1하며 전체67검사는 영향 밖으로 재사용한다. Root는 r16 종료 증거1/핀/host부재를 보존하고 새 r17 control/startup1·현재591/동일DLL·보안창부재·존재하는0바이트잠금 exact r17.preserved 보존1을 준비한다. 미소비 후보 핀 보정1은 이전 원문 보존 후 허용한다.

새 사례 base는 b1-unity-g3-owned-20261018-r17이다. 실제 시작은 최종핀과 Luna 결과 뒤 별도 정확 승인한다. 동일 v20/EditorDLL174080/9E55326258D7F6678BBD016E024FC8F866A52CF128939805DBBF322E7366614D 및 RuntimeDLL294400/8576FC2D85371C4447A18CC91196D995697CD71B10AEF3F3C4C7CD2A6B723409가 현재 소스와 일치할 때 r16 실제 Csc를 재사용할 수 있다. 새 실행의 증분 종료0/reload/오류0/현재핀은 별도로 확인한다. Play 전환 복구는 여전히 미관측이며 새 initial/prepare와 동적 핀·원래기한·단회·실패STOP을 유지한다. 과거 실패 재시도나 권위 부활이 아니다. 승인자 Astra gpt-6-astra.
## R01PLAY 재로드 선행 단계·카메라·동일시각 정책 — Approved

Astra는 preparation/r01-first-play-reload-analysis-sol61.v1.md13686/15DAE0EBE09A3EB75163AD98364B320EFC61CD2B8FCAC93D5825062129D10C65 전체와 r17 closure5929/911A7C3B81AF1ABEFD77285A38EE65CC484235AEF72536EAE8BB88F4CCCA5B72를 직접 판독했다. 예상 phase1 뒤 phase2/owner/bound 복원실패·samples0, 조기 loadedfalse 후 정상 scene/target 값, 카메라 초기조건 실패, 영상 current==previous2384640808944를 분리한다. 후속 정상 scene 값은 앞선 입력 실패를 취소하지 않는다. 영상 동시각은 픽셀 동일 증거가 아니다. 과거 준비 수용·실패·종료·부분영상은 보존한다.

공식 sceneLoaded 설명은 OnEnable 뒤 Start 전 순서이며 첫 Input보다 앞선 보장은 아니다(https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html). SystemRelativeTime 문서는 compositor의 QPC 기반 시각을 정의하나 서로 다른 반환 frame의 고유 timestamp 보장을 명시하지 않는다(https://learn.microsoft.com/en-us/uwp/api/windows.graphics.capture.direct3d11captureframe.systemrelativetime?view=winrt-26100). 다음 정책의 유한 상한은 API의 경험적 보장이 아니라 이번 명시 승인 예산이다.

### REQ/AC-R01PLAY-005: 명시 중단 완료와 sceneLoaded 복원

기존 정확 Window의 ExitingEditMode에서 승인 신원/현재상태 재검증→Expect→명시 Suspend를 한 번 수행한다. writer 미개방·기존 두 구독 해제·phase2/sub1/unsub1의 실제 완료를 직렬화하고 OnDisable은 중복 중단하지 않는다. phase1을 완료로 간주하거나 소비 상태를 초기화하지 않는다. 실패는 기존 봉인/STOP이다.

같은 원래 Window가 공개 sceneLoaded를 한 번 등록하여 자기 승인바이트·PID/시작시각·GameView 참조/ID·자기 ID를 검증한다. 전달된 정확 승인 scene의 session/host/collector 유일성을 확인한 뒤 그 Window가 복원 인터페이스를 직접 제공한다. scene clone이 잃은 외부 owner 참조의 자연 복원을 가정하지 않는다. 다른 Window 탐색/생성·collector 재설치·승인 교체는0이다. 명시 중단 완료 상태와 현재권위가 모두 유효한 경우 관리 객체 복원1만 허용한다.

Lifetime의 조기 OnEnable은 아직 loadedfalse인 정상 생성단계에서 이름 복원을 시도하지 않는다. 정확 sceneLoaded에서 기존 host/handle/path/이름 검증·같은 미저장 scene의 빈이름 정확복원1을 수행하고 graph 활성 전 재검증한다. 비빈 다른 이름·다른scene·저장scene·host불일치는 실패한다. Runtime Start에 이르렀는데 선행 완료가 없으면 기다리지 않고 실패한다.

실제 Play 첫 Input 경계 선행감시는 전환 재개 초기부터 유지한다. sceneLoaded/bridge/writer 준비 전에 경계가 오면 정확 실패한다. 입력을 지나친 뒤 최초처럼 기록하거나 지연 재시도·폴링으로 성공을 기다리지 않는다. 성공/실패/Close에서 scene/assembly/Input 구독을 실제 해제하고 등록·해제·복원수를 보존한다. 기존 입력범위·단회·Pause/UInt64·시계·source/승인핀 검사는 유지한다.

### REQ/AC-R01PLAY-006: 실제 활성화 직후 카메라 초기화

Lifetime은 graph 활성 전에 정확 camera/player/설정 firstTick의 결속을 준비하고 SetActive(true)가 반환한 직후 첫 FixedUpdate 전에 생산용 bootstrap1을 호출한다. CameraDriver는 정확 graph/host/player/firstTick 및 기존 활성·snapshot.Tick==NextExpectedTick 초기조건을 검증하고 기존 Initialize를 수행한다. Camera.Start는 이미 완료된 초기화를 반복하지 않는다. 최초 호출 전에 tick이 진전됐거나 반복 bootstrap이면 실제값을 기록하고 실패한다. 완료 pose를 합성하거나 tick을 되돌리지 않는다. bootstrap은 available/완료 publication을 만들지 않으며 실제 완료 Movement tick 소비는 기존 FixedUpdate에서만 한다. ForTests 경로 호출·기존 초기조건 삭제0이다.

### REQ/AC-R01PLAY-007: 정확 동일시각의 유한 제외

새 영상 정책은 이전 유효 source frame timestamp와 정확히 같은 timestamp에만 적용한다. 원래 HWND/프로세스신원·크기/형식·STOP/Closed·기한·시계하한 및 실제 수용시각 상한 검사를 모두 통과한 뒤 연속 제외 최대2회, 전체 capture 제외 최대32회 안에서만 equal-time-excluded로 분류한다. 각각3번째/33번째는 즉시 실패한다. 엄격히 큰 source timestamp를 정상 수용할 때만 연속계수를0으로 되돌리며 전체계수는 초기화하지 않는다. 역행은 즉시 실패한다.

제외 frame은 encoder WriteSample에 넘기지 않고 기존 frame/surface를 즉시 해제한다. 직전 source timestamp·마지막 제출 timestamp·PTS·원점·기한을 합성하거나 갱신하지 않는다. 정상 제출 PTS는 계속 엄격 증가한다. 각 제외의 현재/직전 원시시각·수신/수용QPC·실제 횟수·사유·검사결과를 최대32행/행8192바이트로 기존 stdout 한도 안에 기록한다. 원시 기록 실패·한도초과는 실패한다. 종료 원시에 제외 총수/최대연속수/내용동일 미검증/해당구간 내용연속성 미관측을 명시한다. 이 계수는 기존 frame-rate policySkipped와 구분한다.

제외는 처리 정책상 계속 진행할 수 있다는 뜻이며 픽셀동일·누락0·완전연속성 수용이 아니다. 제외가 있는 최종 영상은 독립 검수가 구간/원시를 확인하기 전 연속플레이 증거로 수용하지 않는다. 실패한 r17에 소급 적용하지 않는다. 무제한 또는 조건 없는 중복무시·픽셀 추가복사/비교·timestamp합성·역행완화0이다.

### 구현 소유·핀·검사 예산

Sol 단독 제품 파일은 기존 Window/Observer/Chapter1EncounterLifetimeOwner와 Runtime/Camera/Unity/SimulationCameraDriver.cs까지 정확4개다. builder·InputRouter·Movement·Target·다른586행·패키지/asmdef는 불변이다. 새 evidence/b1-final-source-checkpoint-sol61.v21.json은 v20의 실제 변경행만 갱신하며590+추가1/591집합을 유지한다. 새 tools/b1-window-video-capture.v7.cpp는 v6에서 위 정책/원시 및 자기 kSource v7 경로만 변경하고 새 b1-window-video-capture-r10.exe를 빌드한다. manager.v19.ps1은 v18에서 r17→r18·v20→v21·영상v6/r9→v7/r10 및 실제 핀만 갱신한다. 실제파일/kSource/Prepare.source/manager/compile입력 일치 검사는 필수다.

구현1·국소 결정적 경계/순서검사1·범위보정1/영향재검사1·Luna 독립1, 독립 P1 한정 보정1/영향재판독1을 묶는다. 벡터는 suspend미완료/완료·잘못된권위·sceneLoaded순서/누락/중복·첫Input선행·writer실패·창참조소실·pristine/진전tick/반복카메라·동시각연속2/3·전체32/33·역행·시계/신원/크기/기한/로그실패다. 산출물은 preparation/r01-first-play-validation/의 신규 명명 파일로 보존한다. 기존67검사의 영향 밖 부분은 재사용하되 새 순수검사를 실제 Unity 순서 보장으로 주장하지 않는다.

native 빌드1은 기존 검수설정의 compile1→성공0시link1이며 보정 뒤 재빌드1까지 허용한다. project/Library/b1-video-cpp-build-r10/ 아래 obj/dependencies/link-inputs.rsp/compile-plan.json/link-plan.json/각stdout·stderr를 신규 작성한다. 보정은 .correction 접미의 별도 파일로 최초출력을 보존한다. 실제 종료값·회수를 기록한다. 제품 새 Csc1/새Editor·RuntimeDLL/reload와 최초Play전환1은 최종핀·Luna 뒤 별도 정확시작으로 수행한다. 컴파일오류 한정보정/재컴파일1 외 실제 실패 자동재시도0이다.

Root는 r17 종료원문·host부재를 보존하고 새 r18 control/startup1·현재591/보안창/Editor부재·존재하는0바이트잠금 exact r18.preserved 보존1을 준비한다. 미소비 후보보정1은 원본 별도보존 후 허용한다. caseBase b1-unity-g3-owned-20261018-r18의 새 신원·동적핀·초기/prepare·원래18000/17990/1800/1790·사용자최초Play/Root최초Pause/UInt64를 유지한다. 수동경로키질문·Root게임입력0, SourceReady=false다. 이 승인은 구현 범위이며 실제 재생복구·영상수용·새실행 권위를 선취하지 않는다. 승인자 Astra gpt-6-astra.
R01PLAY-006의 생산용 호출 경계는 Input.Unity에서 Camera.Unity 직접 참조를 추가하지 않는다. Chapter1EncounterLifetimeOwner.cs 안의 internal 인터페이스를 CameraDriver가 기존 friend 접근으로 구현하고, Lifetime은 기존 InputRouter.BoundCameraProviderForAuthoring 객체를 정확 캐스트하여 호출한다. 동일 graph의 owner/player·설정 firstTick·단회 검증을 인터페이스 호출 전후 유지한다. asmdef·InputRouter·기존 IInputCameraSnapshotProvider 변경은0이며 다른 카메라 검색/대체0이다.

R01PLAY-005의 최초 경계는 실제 Play&&!Pause의 설정된 player input update 경계다. Editor 준비 관측 update를 게임 첫 경계로 취급하지 않는다. 구현 전 현재 프로젝트 설정과 설치 InputSystem의 updateType 분기를 한정 읽어 정확 player모드·구분값을 결속한다. 첫 실제 player경계는 준비미완료면 실패하며 건너뛰지 않는다. r17의 실제 실패 updateType은 미관측이므로 Editor update가 원인이라고 단정하지 않는다. 알 수 없는 갱신형식·설정은 임의 무시하지 않고 실패한다. 관측 원시에 실제 updateType·설정값을 포함한다.
### R01PLAY-005 실제 입력 갱신형식 결속 보완

Root는 프로젝트 InputSettings asset/설정 대입 부재와 EditorBuildSettings.m_configObjects 비어있음·activeInputHandler1, 설치 InputSettings.cs774의 기본 Dynamic 및 InputManager.OnBeforeUpdate의 현재형식 설정 후 listener 호출을 한정 판독했다. Astra는 설치 Runtime/InputSettings.cs의 기본값, Runtime/State/InputState.cs27의 공개 InputState.currentUpdateType, Runtime/InputUpdateType.cs의 형식 의미를 직접 확인했다. 정적 기본값은 현재 실행값 증명이 아니다.

준비·전환중단·복원에서 실제 InputSystem.settings.updateMode가 ProcessEventsInDynamicUpdate인지 읽고 고정/원시에 기록한다. 다른 모드·변경은 실패하며 설정을 자동 변경하지 않는다. 실제 onBeforeUpdate에서 공개 InputState.currentUpdateType을 읽어 판정한다. Play&&!Pause의 Dynamic은 최초 실제 player 경계이며 준비미완료면 즉시 실패한다. Editor는 별도 편집기 관측으로 분류하여 player 경계 성공/소비로 세지 않는다. None은 설치 설명상 실제 갱신이 없는 재설정 단계로 별도 기록하며 player 입력 성공으로 세지 않는다. 이 둘을 제외한 다른/복합/알수없는 형식은 기본적으로 실패한다.

BeforeRender는 정확 보유 keyboard/mouse의 실제 updateBeforeRender가 모두false이고 추가 지원입력 장치가 없다는 검증이 있는 경우에만 비대상 장치 단계로 분류할 수 있다. 그렇지 않으면 지원범위 밖으로 실패한다. Fixed/Manual은 고정 Dynamic 정책과 불일치하므로 실패한다. 실제 첫 Dynamic 이전의 모든 전환 표지와 첫 각형식 관측을 기존 전체16행/행8192바이트 한도 안에서 기록한다. 반복 Editor/None은 계수만 누적하며 무한 로그를 만들지 않는다. 이 구분은 실제 player 첫경계를 지나쳐도 된다는 권위가 아니며 r17의 미관측 updateType 원인을 소급 확정하지 않는다. 승인자 Astra gpt-6-astra.
## 2026-10-21 사용자 재개: r19 정적 준비와 Csc 재사용 — Approved

Astra는 r18 startup.jsonl의 기한 실패 case cleanup reserve와 owned-stop2418978767946을 직접 판독했다. Unity3758145814/video1·원래 프로세스/Job/이벤트 핸들 폐쇄 및 양 파이프 EOF/flush/폐쇄가 기록됐다. readerStatus Running은 정리 당시 값이며 현재 생존 증거가 아니다. Root는 닫힌 원시 핀·부분영상·실제 Play/진단 유무·호스트 원PID 부재를 별도 종료요약으로 보존한다. r18 준비 성공은 역사로 유지하고 만료 권위를 부활시키지 않는다.

REQ/AC-R01PLAY-005..007 및 R01BIND/PLACE의 동작은 변경하지 않는다. Sol 단독으로 새 tools/chapter1-r01-user-play-entry-manager.v20.ps1을 만들되 검수된 v19 43769/729398E4FDC4CA74C95C38CB6BD99CC829B3AC39B4097DFF94FFAAC23A5AC63B의 제어 접두사 r01-user-play-entry-20261018-r18 한 곳만 r01-user-play-entry-20261021-r19로 교체한다. 역치환 전체동일·파서1, 표기 보정1/영향 재검사1까지다. 제품 v21/590행 및 추가1·CPPv7/r10exe·영상 정책·입력/수명/카메라 가드·시계는 불변이며 재빌드0이다. Luna는 r18 종료요약과 이 단일 차이·불변 핀을 묶어 독립 검수1한다. 기존86검사와 코드 검수는 영향 밖으로 재사용하며 실제 Play 성공으로 확대하지 않는다.

Root 정적 범위는 현재590 전량 길이/SHA·실제591 집합/추가설정1·세 DLL·정식 호스트 Editor/원PID 부재·보안창 부재 확인, 새 evidence/r01-user-play-entry-20261021-r19 폴더1과 startup 후보1 작성이다. 존재하는 정확0바이트 Temp/UnityLockfile은 배타읽기 종료·정확 경로/조상 비링크·목표 부재를 확인한 뒤 UnityLockfile.r01-user-play-entry-20261021-r19.preserved로 가역 이름변경1할 수 있다. 비어있지 않은 잠금이나 장면 백업은 이번 범위로 삭제/이동하지 않는다. 실제 발견 정보를 보존하고 별도 한정 판단한다. 새 사례 base b1-unity-g3-owned-20261021-r19는 예약 전 부재를 유지한다. 미소비 startup 핀 보정1은 이전 원문을 별도 CreateNew 보존한 뒤만 허용한다. 기존 r18 파일은 수정하지 않는다.

r18 compile2514/029586113E0B8E6F97C8561CDC5D059FF903EC97239851E75A55897FE13DB50B의 실제 새 Csc는 현재 v21 소스와 DLL이 정확히 같은 경우에만 재사용한다. Editor175616/D1C0C666BAD434EA0C3B596EDEE1A49217FFD9B15780DC0645FF10E3D599DB9D, Input297984/D412FDF3034123DB456175AC285DAA79333919F9F0EAB84F5E038E4C223FBE30, Camera17408/F2032F578556083178535B3771C5C7CE5A9AAB57647D5FDF59E2AE6688BDDA22를 각각 대조한다. 새 실행에서는 실제 증분 빌드 종료0·reload·오류0·현재 DLL 핀·590/591을 별도로 관측한다. 새 Csc가 없으면 없다고 기록하며 과거 새 Csc를 이번 실행으로 표기하지 않는다. 불일치는 이 재사용 조건을 충족하지 않는다.

정확 시작은 최종 startup/명령/도구인수 핀과 Luna 결과, 직전 현재 상태 확인 뒤 Astra 별도 판정한다. 이 부록은 정적 준비만 승인하며 Unity/영상/GUI 준비 동작을 지금 실행할 권위가 아니다. 새 실행의 initial/prepare는 새로 관측하고 동적 신원·핸들·권위·원래 전체18000/cleanup17990/사례1800/영상1790·첫 사용자 Play/Root 최초 Pause·UInt64 단회를 유지한다. 사용자 경로/키 질문·게임입력 주입·자동 실패 재시도0, SourceReady=false·CH1완주 미수용이다. 승인자 Astra gpt-6-astra.
## R01PLAY 최초 표본 일시정지·진단 대기 호환 — Approved (2026-10-21)

Astra는 preparation/r01-first-play-death-analysis-sol61.v1.md9503/3B5F6E0F3B348357246C946433B153A01B3471E91F4CDCCE219FB47ABD01187C 전체와 실제 r19 봉인/말미표본·종료 기록을 판독했다. 359표본·tick0부터358·체력5에서0·마지막 Health.triggeredtrue와 KillPlane/LethalCrushfalse를 보존한다. 무입력 대기와 활성 적 피해 경로는 절차 충돌의 근거지만 특정 적이 모든 피해를 주었다고 확정하지 않는다. 카메라는 호출 전 준비 원시와 실제 tick0/카메라 오류없음까지만 확인됐으며 완료 초기화 원시를 발급하지 않는다. r19 사망 봉인·진단 미소비·원래 STOP/회수·영상0은 유지한다.

### REQ/AC-R01PLAY-008: 첫 유효 표본 뒤 동기 단회 일시정지

기존 Root 최초 GUI Pause1을 현재 실행 승인에 결속된 원래 Window의 자동 Pause1로 대체한다. 사용자 최초 Play는 그대로이며 키/경로/추가 통지 대기로 이 정지 시점을 결정하지 않는다. 정확 R01-A·현재 승인/프로세스/시작시각·Window/GameView·host/collector·source핀, 최초 Dynamic/bridge/writer 선행이 검증된 경우만 허용한다. 첫 tick0 표본 직렬화·Append·Flush 성공과 기존 위험 가드 통과, health>0/사망false/모든 위험false/live&&!sealed가 필수다. 위험한 첫 표본은 기존 실패이며 Pause로 성공 전환하지 않는다.

Observer의 기존 위험검사 직후 동일 IReceivedInputContext 경계로 동기 요청한다. Window는 요청자와 runKey/generation/tick0·샘플수1·Flush 성공·정확 원래 객체 및 잠긴 동일 승인 바이트를 검증하고 실제 Play&&!Pause에서 EditorApplication.isPaused=true를 딱 한 번 호출한다. 비동기 지연/폴링/자동 재요청0이다. 호출 전후 시각·현재 tick/health·Flush/실제 paused 상태·소비 횟수/사유를 유한 원시로 기록한다. setter 반환과 실제 pausedtrue가 모두 필요하며 참조/핀불일치·이미 Pause·중복·setter/기록 오류는 실패한다. Runtime의 UnityEditor 직접 참조는 추가하지 않는다.

Pause 요청이 진행 중인 물리 단계나 이미 예약된 다음 생산자 호출을 선제 차단한다고 주장하지 않는다. 정지 요청부터 진단 수용 뒤 명시 재개까지 별도 대기 상태를 유지한다. 추가 완료 tick/위험/봉인/live 소실이 관측되면 정확 원시 후 실패로 닫는다. 정상 재개 이후의 정상 tick은 이 대기 가드로 거절하지 않도록 명시 재개 전이와 원래 세대/승인 검증을 구현한다. 생산자 tick을 지우거나 건너뛰거나 되돌리지 않는다. 실제 효과는 새 단회 실행에서 표본수/원시와 Root GUI 대조로만 수용한다.

### REQ/AC-R01PLAY-009: 정지 검증·단회 진단·명시 재개

Root는 자동 Pause 원시와 실제 같은 main/GameView의 일시정지 상태를 한 번 대조하며 수동 Pause 토글을 추가로 누르지 않는다. 실제 Play&&Pause·live&&!sealed·tick0 표본1·현재 승인/원본/기한/오류없음을 확인한 뒤 기존 정확 고정path·5초32768바이트1회 UInt64를 수행한다. 기존 진단의 단회·핀·폐쇄·정수 의미와 다른 필수 AC는 유지한다. 진단 성공은 게임 입력/이동/완주 수용이 아니다.

Root의 닫힌 진단 원시·GUI 대조와 독립 수용 및 Astra 해당 실행 판정 후에만 같은 Window가 진단 성공/현재 승인/원래 객체·시계/대기 상태를 재검증해 명시 재개 전이1을 허용한다. 구현에는 이 조건부 재개 경로를 포함하되 실제 재개 동작 권위는 진단 뒤 별도 정확 판정한다. 추가 사용자 경로/키/중립 질문은0이며 실제 게임 조작은 사용자다. 미승인 외부 unpause/진단 전 재개·추가 tick·실패/만료면 STOP이고 재시도0이다. 정지 중 전체/사례/영상 시계는 계속 흐른다. 표본이나 입력을 합성·누락 보충하지 않는다.

### 소유·정확 경로·유한 예산

Sol 단독 제품 변경은 project/Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs와 Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs 두 파일 및 Observer 안 기존 인터페이스에 필요한 항목뿐이다. Window 기존 공개 동작 영역에 필요한 상태/진단 뒤 조건부 재개 동작을 한정 추가할 수 있으나 현재 최소창/도킹/원본참조·기존 버튼 접근을 유지한다. Lifetime/Camera/InputRouter/Combat/Threat/Movement/NPC/prefab·체력·적 공격·무적·위험/봉인 가드·영상v7/r10·패키지/asmdef는 불변이다.

새 evidence/b1-final-source-checkpoint-sol61.v22.json은 v21 실제 변경2행만 갱신하고590+추가1/591을 유지한다. 새 tools/chapter1-r01-user-play-entry-manager.v21.ps1은 v20에서 제어 r01-user-play-entry-20261021-r19→r01-user-play-entry-20261021-r20 및 checkpoint v21→v22/실제 핀만 갱신한다. 새 사례 base는 b1-unity-g3-owned-20261021-r20이다. 영상 재빌드0이다. 기존 소진 산출물은 덮어쓰지 않는다.

구현1·순수 판정/순서 국소검사1·범위보정1/영향재검사1·Luna 독립1, 독립 P1 보정1/영향재판독1까지 같은 묶음이다. 벡터는 Flush실패·위험 첫표본·다른권위/객체·중복/이미정지·setter실패·추가tick·live/봉인·진단실패/성공·미승인재개/정상재개·만료/정리다. 실제 엔진 정지 효과는 순수검사로 증명하지 않는다. 국소기록은 preparation/r01-first-sample-pause-validation/ 신규 명명 파일로 보존한다. 새 Editor/Input 실제 Csc1·각 새 DLL/reload·오류0를 요구하며 Camera 동일핀 유지 확인한다. 컴파일 오류 한정보정/재컴파일1 외 동적 실패 자동 재실행0이다. 실제 최초 Play1과 자동 Pause1은 정적검수/최종핀 후 별도 정확 시작 판정이다.

Root는 r19 closure6973/38AB3C14C82A3E505BF082B2233A46E44B2DEC1E3C785DFA27E88B0302875CD9 및 원시를 보존하고 현재591/호스트/보안창을 확인한다. r20 control폴더1/startup후보1·현재 정확0바이트 잠금의 배타읽기/비링크/목표부재 후 exact r20.preserved 가역 이름변경1을 준비할 수 있다. 미소비 startup 보정1은 이전원문 별도보존 후만 허용한다. 장면 백업 발견은 별도 실제대상 판정이며 삭제/복원 권위가 아니다. SourceReady=false·CH1완주 미수용·게임입력주입0이며 과거 최초Play/사망/만료 실패를 취소하지 않는다. 승인자 Astra gpt-6-astra.
R01PLAY-008/009의 실행 승인 스키마 호환은 다음 네 필드로 제한한다: firstSamplePauseAuthorizedNow=true, firstSamplePauseCaseId="R01-A", firstSamplePauseInvocationLimit=1, resumeAfterUInt64AcceptedAuthorizedNow=true. 마지막 true는 진단 전 재개 권위가 아니라 닫힌 진단 성공·독립 수용·Astra 실제 실행 판정 뒤 공개 재개 요청1을 검증할 조건부 범위다. 기존 execution-approval의 엄격 파서/허용키·투영·동일바이트 핀 검사에 네 필드를 명시 추가하고 누락·형식오류·다른사례/한도는 거절한다. 기존 동결 실행승인에는 소급 적용하지 않는다. startup의 기존 스키마/기본진단false는 유지하며 새 실제 execution만 이 권위를 발급한다. 현재 계약 핀은 기존 toolObservation.bindingContract에 결속해 문서 지문 재귀 의존을 추가하지 않는다.

원시는 적어도 phase, pauseRequested, pauseInvocationCount, actualIsPaused 전후, firstTick, lastCompletedTick, sampleCount, flushSucceeded, diagnosticsAccepted, resumeRequested와 승인 SHA/원래 runKey·generation을 기록한다. 단계는 첫표본 대기→Pause 요청 소비→실제 Pause 확인/진단 대기→진단 수용 후 명시 재개 소비로 구분한다. 최대8전이·행8192바이트를 기존 사건 한도 안에 보존하며 반복 폴링 로그0이다. 진단 수용 전 사용자/외부 unpause는 다음 공개 pauseStateChanged 또는 첫 실제 입력/완료tick 감시에서 정확 실패로 닫는다. OS 입력 자체를 막았다고 주장하지 않고 재개 효과를 성공으로 처리하지 않는다. 정리 때 구독을 해제한다. 진단 뒤 재개는 Root가 같은 Window 공개 동작1을 정확 판정 후 수행하며 게임 키 입력은 사용자만 한다. 원래 승인파일을 재개 시 덮어쓰지 않는다. 승인자 Astra gpt-6-astra.
## 2026-10-21 보안창 처리 후 r21 정적 재개 — Approved

Astra는 r20 first-failure overall cleanup reserve4157852722680과 owned-stop4157857706638 원문을 직접 판독했다. Unity3758145814·원래 process/Job/이벤트/Root 핸들 폐쇄, videoHandle없음·stdout/stderr null은 영상 미시작 상태다. tool51684 회수exit1과 현재 원PID/Editor 부재는 Root 실제 관측으로 종료요약에 구분한다. 준비 동작·RESERVE·영상·collector·Play/진단0과 compile 성공을 분리하며 r20 권위/시계/소진은 보존한다.

Sol 단독 새 tools/chapter1-r01-user-play-entry-manager.v22.ps1은 검수 manager21 43769/8941053BF749C42CFF76AC4455CC61DC56441357A8BE3E87CFBD1E575E5FFAB9의 제어 접두사 r01-user-play-entry-20261021-r20→r01-user-play-entry-20261021-r21 한 곳만 교체한다. 역치환 전체동일·파서1·표기보정1/영향재검사1을 허용한다. 제품v22·590행+추가1/591·영상v7/r10·4필드/자동Pause/진단/조건부재개/원래시계는 불변이며 제품수정/영상재빌드0이다. Luna는 r20 종료 및 단일변경·불변핀을 묶어 독립검수1한다. 기존58검사와 코드검수는 영향 밖 재사용이며 자동Pause 성공으로 승격하지 않는다.

Root는 현재590 전량/실제591/추가설정1·세DLL·호스트Editor/원PID부재·실제보안창부재를 확인하고 새 evidence/r01-user-play-entry-20261021-r21 폴더1/startup후보1을 작성한다. 정확0바이트 Temp/UnityLockfile이 있으면 배타읽기 종료·경로/조상비링크·목표부재 뒤 UnityLockfile.r01-user-play-entry-20261021-r21.preserved로 가역 이름변경1할 수 있다. 백업폴더는 현재 부재를 확인하며 새발견을 자동삭제/이동하지 않는다. 새 사례 base b1-unity-g3-owned-20261021-r21는 예약 전 부재다. 미소비 후보핀 보정1은 이전원문 별도보존 뒤만 허용한다.

r20 compile2630/92E248A1A7D895EB597463444C7AFEA914B515C09985A7C4D7D77791D7A8688E의 실제 세 Csc는 현재 v22 소스와 Editor183808/39BD16CE644AE5B4E79E98FDB35117CBC5581273992850FF4634F7E0DE05C6CA, Input304128/FEF1404E588B1622741EEA05526FDA123B3873B92631481BFFF0B3C18AF804AE, Camera17408/1DAFABAE0C02E7171F0E43A99E1CA58F02336D4989B221983B4BF0087F8CF670이 정확히 동일할 때만 재사용한다. r18 Camera 옛F203 핀 조건을 재도입하지 않는다. 새 실행의 실제 증분 종료0/reload/오류0·Csc 유무·현재 세 핀은 새 관측으로 결속한다. 불일치는 추정 재사용하지 않는다.

정확 시작은 최종 startup/명령/도구인수 핀·Luna·직전 현재상태 뒤 별도 판정한다. 이 부록은 정적 준비이며 실제 시작/GUI/영상 권위가 아니다. 새 initial/prepare·동적 신원·4필드 결속·최초 사용자Play 뒤 자동Pause/실제 정지검증·UInt64 단회·진단수용 뒤 정확재개1과 전체18000/cleanup17990/사례1800/영상1790·실패STOP·자동재시도0을 유지한다. 수동 최초Pause 토글·게임입력주입·사용자경로키질문0, SourceReady=false다. 승인자 Astra gpt-6-astra.
## 2026-10-21 r21 실패 보정 묶음 — 첫 Dynamic 생산 게이트·영상 시간 예산 — Approved

승인자 Astra/gpt-6-astra. 직접 판독한 Sol 분석10645/172DC5BDEF713FAEC1585E463A1DC22D8E4EE042E34B68D6CD8B750D1533732C와 추가 설계11684/BD3A30279BD08EEE33788B40D8B8039F988241ECE89BE5F6CEC77DF723744186를 근거로 한다. r21 closure7912/F8E189B0F9411D7D153AF60DF768D6813791BF60196C06638435C6453B15F738 및 모든 실패 원문은 보존한다. 사용자 산출물은 실제 첫 입력을 보존한 tick0 자동 정지·필수 진단을 거쳐 직접 이동할 수 있는 준비다. 현재 준비/실행 성공을 선취하지 않는다.

### REQ-R01PLAY-010 / AC-R01PLAY-010: 첫 실제 Dynamic과 생산 시작

Sol 단독 제품 변경 범위는 Runtime/Input/Unity의 Chapter1EncounterLifetimeOwner.cs, Chapter1BasicMovementTickObserver.cs, InputRouter.cs 및 Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs 네 파일이다. 경로 기준은 output/chapter1-playable-r1/project/Assets/AcadeGameMaker다. Camera 소스·asmdef·입력 설정·적/체력/위험/이동·장면/프리팹은 변경하지 않는다. 이 범위는 기존 두 파일 제한을 이번 원인 보정에 한해 대체한다.

정상 sceneLoaded의 원래 Window/bridge/writer 준비가 완료된 뒤에만 같은 Lifetime이 정확 inactive/pristine 그래프의 기존 액션 wrapper·콜백·Gameplay map을 단회 사전 준비한다. Router의 정상 Start 공통 액션 준비를 분리한 내부 생산 경계이며 일반 초기화 검사를 무조건 완화하지 않는다. 정확 소유·비활성·미Awake/미Start·미등록/미실패·미발행 상태와 wrapper/map/등록 증거를 요구한다. prepared는 initialized/완료 tick이 아니다. 모터·소비자 전체 초기화를 inactive 상태에서 호출하지 않는다.

Start 요청과 최초 실제 Dynamic 검증은 별도 사실이다. 실제 Dynamic onBefore에서 이미 준비된 원래 액션/기기/승인/등록과 원래 bridge를 검증한다. 요청과 검증이 갖춰지면 해당 callback 또는 원래 정상 수명 지점에서 graph 활성화1을 소비한다. 동일 Dynamic의 onAfter 실제 서수를 대조해 입력 처리 종료를 별도로 기록한다. 정상 Awake/Start는 정확 준비 증거만 재사용하고 기존 모터/소비자 초기화와 Camera Prepare→활성화→Complete 순서를 유지한다. 새 wrapper/중복 등록/재Enable/캐시 Clear·입력 합성·수동 Start/시험 초기화/InputSystem.Update는 금지한다. 최초 실제 사건·현재 눌림·짧은 press/release는 같은 실제 콜백으로 보존해야 하며 before 검증을 사건 처리 완료로 부르지 않는다.

첫 생산 Fixed 전 정상 Start 완료·동일 Dynamic 종료·원래 생산 신원·pristine tick0가 모두 성립해야 한다. 성립하지 않은 활성 그래프를 조용히 진행시키거나 완료 tick을 버리지 않고 실패한다. 그래프 활성화 전의 정확 사전 준비 대기만 Observer의 fixedCalls 증가/역할 검사 앞에서 분리한다. 일반 graph 소실·이미 생산 진전·잘못된 부분 활성은 대기 분기로 흡수하지 않는다. 실제 첫 Dynamic 감시와 기존 FirstPauseAllowed의 boundary 필수조건을 유지하며 Fixed에서 이를 true로 만들지 않는다.

대기는 writer 준비의 실제 시각/프레임에서 시작하여 5초 또는300프레임 먼저 도달한 때 실패한다. 기존 전체/사례 기한도 유지하며 Window update와 실제 callback에서 검사한다. 원점 재설정·무기한 대기0. 요청/검증/종료/소비/실패의 유한 전이만 기록하고 매 프레임 로그는 금지한다. 실패·종료 시 사전 준비 map/등록/부분 graph를 정확 소유로 해제한다. 실제 원시에는 요청·before/after 서수·wrapper/map 동일성·Start 완료·활성화 횟수·첫 tick과 첫 Dynamic을 구분한다. first sample Append/Flush·위험 검사 뒤 자동 Pause1 및 추가 tick/진단 전 재개 실패 가드는 유지한다.

AC는 Start→Dynamic과 Dynamic→Start, 첫 Fixed 선행의 미생산 대기, before/after 불일치, 최초 실제 사건/현재 눌림/짧은 press-release 보존, 중복/다른 소유, 5초/300프레임/원기한 경계, 부분 실패 정리, 이미 진전된 tick 거절, 정상 Start proof 재사용/재Enable·캐시초기화0, Camera pristine·최초 tick0/Pause 순서를 국소 검증한다. 실제 엔진 수명과 입력 전달은 정적 검사 통과만으로 수용하지 않는다.

### REQ-R01PLAY-011 / AC-R01PLAY-011: 미발행 시작 식별자

Window의 기존 엄격 UInt64 판독에서 pristine start의 generation0/lastTick-1/fixedCall0/samples0 및 기존 모든 start 조건에만 runKey의 명시 null 또는 길이0 문자열을 허용한다. 필드 누락·다른 타입·공백·발행된 키는 이 예외로 받지 않는다. 일반 사건/표본 키·정수·중복키·스키마는 불변이다. 실제 r21 빈 문자열과 null 양쪽 및 모든 거절 벡터를 검사한다. 미소비 진단의 잠재 충돌 보정이며 과거 실제 진단 실패라고 기록하지 않는다.

### REQ-R01VIDEO-012 / AC-R01VIDEO-012: 유한 동일시각 정책과 출력 예산

새 tools/b1-window-video-capture.v8.cpp는 v7을 보존한 후 이 정책만 변경한다. 새 b1-window-video-capture-r11.exe를 빌드한다. source 자기경로 상수·manager·실제 계획·compile 입력 일치 검사를 필수로 한다. 원래 캡처1790초/절대기한·STOP/신원/형식/기하/하한/실제 수용상한/역행 가드·PTS/제출 정책·정리 수명은 유지한다.

총 동일시각32 제한은 이번 명시 정책으로 대체한다. 연속2는 유지한다. 실제 수신 QPC 기준 초당16건·최대 저장16건·최초16건의 토큰 예산을 사용한다. 부동소수점 누적 허용은 금지하고 주파수/정수 나머지·곱셈 오버플로·QPC 역행/이상·기한을 검사한다. 재충전 기준과 원점은 변경/재설정하지 않는다. 1790초의 이론 총상한28656을 별도 검사하며, 전체 source 수신 상한429600도 별도 검사한다. 이는 240Hz 성능 보장이 아닌 새 유한 운영 상한이다. 부족/초과는 실패하며 실제 다음 실행 통과를 보장하지 않는다.

처음16건 상세 각1024바이트, 이후5초 구간 집계 및 종료 잔여를 합해 최대359행 각1024바이트다. 빈 구간 반복출력은 하지 않고 고정 크기 집계만 사용한다. 구간 최초/말미 실제 쌍·수신/수용 QPC·계수·최대연속·기간·세부 시각 미보존 수를 기록한다. 같은 그림/연속성은 미검증으로 명시한다. 실패 직전 실제 쌍·수신/수용QPC·검사 전 전체/연속/토큰 상태는 한 행8192바이트로 별도 예약한다. 그 값이 없는 과거33번째 쌍은 추론으로 남긴다.

stdout 배분 상한은 상세16384+집계367616+기존 체크포인트262144+기타 준비/정리/실패131072+마지막 실패8192=785408바이트다. 실제 UTF8 개행 포함 종류별 바이트와 전체1048576을 모두 검사한다. 기록 생략·무제한 문자열/행 축적0, 출력 실패/상한 초과는 실패다. 제외마다 모든 안전검사/해제는 유지하며 lastSource/lastSubmitted/PTS는 바꾸지 않는다. source/unique/equal/policySkipped/submitted/unclassified 계수와 관측 공백·내용 미검증을 분리한다.

AC는 토큰 시작/재충전/소수 잔여/저장상한/부족·정확 경계/역행/오버플로, 연속2/3·총28656/28657·source429600/429601, 16상세와 집계/종료 잔여/359한도, 각 출력 상한과 실패행 예약, PTS 불변·즉시 해제·계수 보존을 검증한다. 기존 안전·정리·출력상한 검사를 영향 밖 재사용하고 변경된 출력 분류 경로는 직접 검사한다.

### 단일 소유·산출물·유한 검사·후속 실행

Sol 구현1·국소 검사1·범위 내 보정1/영향 재검사1, Luna 독립 묶음검수1 및 독립 P1 한정 보정1/재판독1을 허용한다. 필요한 결정적 검사 보조 파일은 preparation/r01-first-dynamic-gate-and-video-budget-check-sol61.v1.ps1 및 같은 기본명의 결과.json에 한정한다. 기존 검사58건은 영향 밖만 재사용한다. 구현 검사는 자기 수용이 아니다.

소스 체크포인트 새 v23과 manager.v23.ps1을 생성한다. manager는 r01-user-play-entry-20261021-r22 접두사·v23 소스핀·v8/r11 영상핀만 갱신하고 나머지 감독을 유지한다. 기존 v22/v7/r10/manager22는 덮어쓰지 않는다. 새 Library/b1-video-cpp-build-r11의 obj/의존목록/rsp/compile·link 계획/표준출력·오류 로그를 허용한다. 빌드1은 compile1→성공 시link1이며 보정 재빌드1은 기존 출력 별도 보존 후 .correction 산출물로 한다. 실제 종료/exit코드/명령·환경을 기록하며 미확정을 성공으로 메우지 않는다.

Root는 Sol 쓰기 종료 뒤 현재590행·추가1/591과 새 네 행·불변586, hostEditor0·보안창부재를 확인하고 새 r22 control1/startup후보1을 만든다. 정확0바이트 잠금은 배타읽기 종료·조상 비링크·목표부재 후 r22.preserved로 가역보존1을 허용한다. 새 발견 백업은 자동삭제/이동하지 않는다. 미소비 후보 보정1은 이전 원문을 별도 보존한 뒤 한다. 새 사례 base는 예약 전 부재로 유지한다.

최초 실제 시작/Play는 최종핀·Luna·Astra 정확 판정 뒤 각1이다. 새 Editor/Input 및 실제 의존 Csc 모두 종료0/reload/CS오류0·실제 DLL핀으로 결속하며 Camera 소스 불변이 DLL 불변을 뜻하지 않는다. 이후 최초 실제 Dynamic·정상 생산·tick0 자동Pause/GUI·live&&!sealed·기존 UInt64 단회·독립수용/Astra 별도 재개 판정을 유지한다. 실패 자동재실행0·큰 원장 접근0·사용자 경로/키/중립 반복질문0·SourceReadyfalse·CH1완주 미수용이다. 이번 부록은 구현·정적 준비 권위이며 실제 실행 성공이나 사전 진단 수용이 아니다.
## 2026-10-21 r22 입력 경로·관측 비용 보정 및 새 진단 경로 — Approved

승인자 Astra/gpt-6-astra. Sol 보고서 preparation/r01-resume-input-route-analysis-sol61.v1.md 11723/E1113B304A9ABCA5EEA348E9519217FA81D2138D926DFA366DEE82F24ACD5F1D를 직접 판독했다. r22 실제 첫 Dynamic/자동Pause/진단/재개 성공과 이후 입력 수신0·359표본 사망 실패를 분리 보존한다. read16.30초와 재개~봉인16.51초의 관계는 강한 성능 근거지만 파일 조회만의 총비용이나 입력0의 유일 원인으로 확정하지 않는다. 사용자 행동 부재를 추정하지 않는다. 산출물은 검사 강도를 유지한 관측 비용 절감과 원래 입력 경로의 실제 가시성, 새 정확 진단 뒤 사용자 직접 플레이 준비다.

### REQ-R01PLAY-012 / AC-R01PLAY-012: 동일 빈도 메타데이터 스냅샷

Sol 단독 제품 범위는 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs 및 InputRouter.cs, Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs 세 파일이다. Lifetime/Camera/입력 설정/프리팹/적·체력·위험·이동/영상v8-r11은 불변이다. 기존 네 파일 중 Lifetime 변경 권위를 이번 묶음으로 재사용하지 않는다.

Observer는 시작의 원래 전체 SHA 확인과 정확 경로 결속 뒤 590개 FileInfo를 생성·캐시한다. 기존 매 CheckSourceMetadata 호출마다 모든590개에 Refresh 정확1회 후 같은 스냅샷의 Exists/Length/LastWriteTimeUtc를 비교한다. 경로/개수·검사 빈도·기준 길이와 수정시각·첫/끝 SHA·소실/예외 실패는 유지한다. stale 스냅샷 재사용·부분 검사·주기 완화·실패 무시0. 캐시 배열/기준값은 현재 승인된 파일 순서에 고정하며 다른 세션/재로드 원본을 재사용하지 않는다. 물리 간격·게임시계·생산 tick을 수정하지 않는다.

새 sample 필드는 sourceMetadataMilliseconds 하나로 정하고 기존 readMilliseconds 내부의 실제 메타데이터 구간만 Stopwatch로 계측한다. 유한 비음수 실제값이어야 하며 기존 readMilliseconds/previousAppendFlushMilliseconds 의미는 유지한다. Window의 엄격 표본 키/숫자 투영에 이 필드만 정확 추가한다. 첫/끝 SHA 검사 자체의 타이밍이나 강도를 약화하지 않는다. AC는 정확590/Refresh1회/동일스냅샷·배열경로 결속·예외/소실/길이/시각 변화 실패·새필드의 누락/비유한/음수/중복 거절을 검사한다. 비용1/3 또는 실제 입력 해결은 사전 수용하지 않는다.

### REQ-R01PLAY-013 / AC-R01PLAY-013: 원래 입력 경로 유한 관측

Observer/Window/Router의 내부 읽기 경계로 원래 두 장치와 원래 wrapper/map/소유만 관측한다. events에 새 kind=input-route-observation을 추가하되 최대4행·각 UTF8개행 포함8192바이트·기존 전체 사건 상한 안에서만 쓴다. 단계는 resume-before, first-dynamic-after-resume, first-native-event-after-resume, failure-or-seal 각각 최대1이다. 오지 않은 사건의 행을 합성하지 않는다. 실패와 봉인이 겹쳐도 마지막 단계1회만 기록한다. 고정 카운터/최종시각만 유지하고 반복 로그/사건별 문자열 축적0이다.

고정 원시 스키마는 schemaVersion/kind/stage/sessionId/caseId/approvalSHA256/runKey/generation/observedStopwatchTicks/stopwatchFrequency/frameCount/updateType/updateCount/route로 제한한다. route의 코드상 고정 구조에 공개 Application 포커스/보유GameView 포커스/Play/Pause, 누적Dynamic/Editor 갱신 수, 원래keyboard/mouse의 deviceId·동일참조·added·enabled, wrapper/map/등록proof 동일성·Gameplay/UI map enabled·mode·initialized/Start완료·fault/disposed/closed/captureSuppressed, 원래 두 장치의 onEvent진입 및 갱신종류별/State·Delta/허용control판독 전후 계수·마지막실제수신시각을 담는다. 모든 키/타입을 구현에서 명시 고정하고 국소검사와 Luna가 확인한다. Window의 UInt64 사건 판독은 이 정확 kind/스키마/4단계·현재신원·유한행 제한만 호환하며 임의 사건을 건너뛰지 않는다.

onEvent 계수는 원래 두 장치에 한해 Observer의 Dynamic 필터 앞에서 증가시킨다. InputManager/backend 필터 전이나 OS입력을 관측한 것으로 표현하지 않는다. 추가 입력값/다른 장치/전역키 수집0, 실제 값은 기존 허용control 원문만 유지한다. 실제enabledfalse·wrapper/map 불일치면 원인을 남기고 재개/진행을 거절하며 Enable/Sync/Reset/입력재생으로 복구하지 않는다. 해제/봉인 뒤 원본이 없으면 관측불가를 명시하고 true로 채우지 않는다. 입력 경로 읽기가 기존 콜백·캐시·지도 활성 상태를 바꾸지 않아야 한다.

AC는 단계별1회/전체4회/행·전체바이트 경계, 현재신원/원본, device disabled·wrapper/map변경 거절, 필터 전후 계수 분리·다른장치 무수집, 사건 미도달시 행 미생성·카운터 의미, 조회의 부작용0·부분실패 보존을 검증한다. 현재 입력 수신0 원인은 실제 새 관측 전 미확정이다.

### REQ-R01PLAY-014 / AC-R01PLAY-014: 새 정확 UInt64 단회 권위

r22에서 소비된 evidence/b1-basic-movement-uint64-runtime-r1.json 11783/522BB4DCC9E68436F543F8CE768ECC9F22228B72FA5DC99AB52F715568CC8F44는 그대로 보존한다. 이번 변경 소스의 다음 정확 실행은 evidence/b1-basic-movement-uint64-runtime-r2.json 한 경로만 허용한다. Window 고정상수·Root 실행승인 및 사례별진단경로·같은 창/native권위·소스핀을 r2로 함께 결속한다. 임의경로·r1 덮어쓰기/삭제/재무장·새 실패 자동재진단0이다. 기존의 전역 r1 고정경로 요구는 이번 새 진단에 한해 r2로 대체하며 기존 실제 진단 성공은 역사로 유지한다.

r2도 R01-A만 true/5초32768바이트1회·정확 PID/ticks/창/승인·실제 첫tick0 자동Pause/live미봉인·진단전표본1·폐쇄원시와 창최종상태·Luna독립/Astra수용 뒤 공개재개1을 모두 유지한다. 파서의 영향 밖 벡터 근거는 재사용할 수 있으나 새 live진단을 대신하지 않는다. r2가 이미 존재하거나 다른 실행에서 소비됐다면 중단한다. 새 route/표본 투영 호환과 r1불변/r2정확일치·다른경로거절을 국소검사한다.

### 소유·산출물·검사 예산과 실행 경계

Sol 구현1/국소검사1/범위보정1·영향재검사1, Luna 독립묶음1·독립P1한정보정1/재판독1을 허용한다. 검사는 preparation/r01-resume-input-route-check-sol61.v1.ps1 및 같은 기본명의 결과.json에 한정한다. 변경 전 세 원문은 preparation/r01-resume-input-route-v23-baseline/ 아래 원래 Assets상대경로로 CreateNew·닫힌핀 대조 각1 후 동결한다. 변경본을 기준원본으로 표시하지 않는다.

새 source checkpoint v24 및 tools/chapter1-r01-user-play-entry-manager.v24.ps1을 작성한다. manager23을 기준으로 r01-user-play-entry-20261021-r23 접두사·새체크포인트핀만 갱신하고 영상v8/r11·감독/기한은 유지한다. 새 native빌드0. 역치환/허용핀차이·파서검사1과 제품 변경3행/불변587·extra1/전체591을 묶어 검수한다. 기존106검사는 영향 밖만 재사용한다. 테스트 통과는 실제입력/성능수용이 아니다.

Root는 Sol 쓰기 종료 뒤 현재590전량/591·호스트Editor0·현재보안창부재를 확인하고 새r23 control1/startup후보1을 만든다. 정확0바이트잠금은 배타읽기 종료·저장소경계/비링크/목표부재 후 r23.preserved로 가역보존1, 미소비후보보정1은 이전원문별도보존 뒤 허용한다. 새발견 장면백업은 무단삭제/이동하지 않는다. 새 사례base는 예약 전 부재로 유지한다. 기존 실패·진단·원문을 그대로 보존한다.

실제 최초시작/사용자Play는 최종핀·독립검수·Astra정확판정 뒤 각1이다. 새 Editor/Input 및 실제 의존 Csc종료0·reload/CS오류0·실제DLL핀·현재소스로 새 결속한다. 최초Dynamic→tick0자동Pause→새r2진단·독립수용→정확재개 뒤 실제입력경로/표본비용/화면을 관측한다. 성능 한계와 수신 미관측을 숨기지 않는다. 기한연장·무변경실패재실행·입력주입·사용자키/경로/중립추가요구0·SourceReadyfalse·CH1완주 미수용이다. 이번 부록은 구현/정적준비 승인이고 실제 실행이나 진단 사전수용이 아니다.