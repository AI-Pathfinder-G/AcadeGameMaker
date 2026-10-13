# 첫 로컬 플레이 전달 영향 조사 — 2026-10-03

> 읽기 전용 영향 조사. 제품 구현·수용 계약이 아니다. 테라가 기존 소스·장면·프리팹·입력·미디어와 승인 절차를 대조했다. 루나의 독립 검수와 아스트라의 제품 계약 승인은 별도다. 이 조사에서 Unity 실행·빌드·게임 프로세스·키·원격 호출은 하지 않았다.

## 판정과 사용자 동작

**가장 작은 기존 실행 그래프 후보는 일반 적 두 유형 그래프보다 오르단 단일 보스 실험장이다.** 오르단 저작기（`../../Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`；로컬 보존 자료）가 플레이어·전이·기본공격·보스 피해·카메라·실제 입력·종료 요청을 기존 프리팹（`../../Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab`；로컬 보존 자료）에 결속한다. 반면 일반 적 저작기（`../../Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringBuilder.cs`；로컬 보존 자료）는 Walker와 Surveyor **두 역할을 고정**하며 입력 라우터와 카메라가 없다. 입력 라우터（`../../Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`；로컬 보존 자료）의 저작 연결 대상도 `OrdanBossCombatSimulationDriver`다. 따라서 Walker 한 유형을 요구하면 라우터 일반화·두 역할 그래프 변경이 추가된다. 적의 수가 적다는 이유만으로 구현 범위가 작아지지 않는다.

제안하는 한정 사용자 흐름은 **로컬 실행 파일 → 오르단 실험 구간 → 이동·점프·대시·전이·기본공격 → 보스 격파 또는 플레이어 체력 0 → 화면 결과 → 같은 구간 새 상태로 재시도**다. 이는 VD-00 전체 데모의 거점1·방6/런4·적2·선택/기술2·세령 등장·12~18분 수용이 아니다. 보스 한 전투를 첫 체험으로 선택할지는 아스트라의 별도 제품 계약에서 확정해야 한다. 현재 장면을 실행해 실제 재미나 완주를 확인한 사실은 없다.

## 정확한 재사용 경계

| 사용자 동작·자산 | 현재 파일·실제 구조 | 재사용과 빈자리 | 기존 제품 추적 |
|---|---|---|---|
| 이동·점프·대시·지형 | PlayerMovementMotor（`../../Assets/AcadeGameMaker/Runtime/Movement/PlayerMovementMotor.cs`；로컬 보존 자료）, PlayerMovementController（`../../Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs`；로컬 보존 자료）, 플레이어 프리팹（`../../Assets/Prefabs/Player/CombatEncounterPlayer.prefab`；로컬 보존 자료）, 오르단 그래프의 Ground/LeftBound/RightBound | 코어·Rigidbody2D/Collider2D·실험장 배치 실재. 사용자용 조작 안내와 결과 이후 잠금·초기화는 미연결 | REQ-MOV-001~004, AC-MOV-001/002/004 |
| 조준·전이·회수 | TransferSession（`../../Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs`；로컬 보존 자료）, TransferSimulationDriver（`../../Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`；로컬 보존 자료）, 오르단 그래프의 `BossAuditBox-0`·`BossWeight-0..2` | 입력 라우터/전이 레지스트리/대상이 그래프에 결속됨. 전이 대상의 식별 표현·튜토리얼 표시는 별도 | REQ-WT-001~007, AC-WT-001~005 |
| 기본 공격·체력·피격 | BasicAttackSession（`../../Assets/AcadeGameMaker/Runtime/Combat/BasicAttackSession.cs`；로컬 보존 자료）, CombatSession（`../../Assets/AcadeGameMaker/Runtime/Combat/CombatSession.cs`；로컬 보존 자료）, OrdanBossCombatSimulationDriver（`../../Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs`；로컬 보존 자료）, CombatTarget（`../../Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTarget.cs`；로컬 보존 자료） | 보스/플레이어 체력과 사망을 코어가 계산한다. 게임 화면 체력·명중·피격·실패 표시가 없음. `CombatSession.ResetEncounter`는 완료 틱 후 명시 재등록 계약이지 종료된 보스 그래프의 즉시 재시도 API가 아님 | REQ-COM-001/003/004/006, AC-COM-001~004 |
| 실제 입력·카메라 | GameInput.inputactions（`../../Assets/GameInput.inputactions`；로컬 보존 자료）, InputRouter（`../../Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`；로컬 보존 자료）, SimulationCameraDriver（`../../Assets/AcadeGameMaker/Runtime/Camera/Unity/SimulationCameraDriver.cs`；로컬 보존 자료） | 오르단 프리팹은 GameplayEnabled 입력·픽셀 카메라를 저작 결속한다. 일반 적 그래프에는 이 결속이 없음. 새 Input System 직접 콜백/시험용 진입으로 우회하지 않음 | REQ-UX-001/004, AC-UX-001/004; AC-M5B5-001~008 |
| 격파 신호·잠금 | OrdanBossEncounterHandoffAdapter（`../../Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs`；로컬 보존 자료）의 틱별 `LatestPresentationView`는 `Combatants`와 보스 완료 triplet을 제공한다. OrdanBossTerminalTransitionRequester（`../../Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs`；로컬 보존 자료）는 정확 완료를 보고 입력을 `Transition`으로 잠근다. OrdanBossTerminalTeardown（`../../Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs`；로컬 보존 자료）은 기존 보스 정리 소유자다. | 이 requester는 **Run·보상·장면 전환 권위가 아니다**. 성공 결과 화면·실패 판정·다음 UI 모드·재시도 소유자는 없다. 보스 격파 경로의 기존 완료 증거를 사용하고 새 중복 완료/보상 주체를 만들지 않아야 함 | REQ-COM-003/004/006, AC-COM-002/004; REQ-SCOPE-001/002는 전체 데모로 확대 금지 |
| 시작 장면·현재 빌드 | OrdanBossEncounterSandbox（`../../Assets/Scenes/OrdanBossEncounterSandbox.unity`；로컬 보존 자료） 실재. EditorBuildSettings（`../../ProjectSettings/EditorBuildSettings.asset`；로컬 보존 자료）는 `Bootstrap.unity`만 활성. [별도 전달 감사](./2026-10-03-astra-independent-delivery-audit.md)의 실행 파일 SHA `C8B0D73D…275C6FB2`는 초기 환경 빌드와 동일 | 로컬 시작→실험장 목적지/활성 장면 및 현재 소스 빌드가 결손. 기존 파일 존재를 새 플레이 빌드로 간주 불가 | REQ-PLAT-001/004/005, AC-PLAT-002/005 |

[최초 수치 감사02](./implementation-audits/2026-10-03-02.md)는 확인된 소스 47단위 중 이동·입력·기본공격·전이·오르단의 여러 부분을 50점의 제한 구현으로, `CombatSession`의 피해·체력0 코어를 정확 수용 단위 100점으로 구별한다. 같은 감사는 플레이어·오르단 물리 그래프를 제한 50점, 실제 캐릭터/적 렌더·동작을 0점, 최신 장면 목적지와 완주를 미구현으로 기록한다. 이 점수를 완성 플레이 비율로 더하지 않는다.

## 단일 입력 소유자와 결과·재시도 경계

`InputRouter.RegisterModeRequester`는 두 번째 소유자를 거절하며 기존 `OrdanBossTerminalTransitionRequester`가 이미 등록한다. 새 결과/실패 소유자가 별도로 등록하거나 Input System 콜백을 직접 받아서는 안 된다. 최소 영향 후보는 **기존 requester를 한정 확장**해 성공의 정확 `Defeated/RewardRequest/RoomCompletionRequest` 검증과 `Transition` 잠금을 보존하고, 별도 실패는 동일 handoff의 `player.IsDead` 값으로 식별하며, 같은 requester가 결과 UI를 위한 `UIOnly` 전환을 후속 정확 틱에서 소유하는 것이다. 이 경로는 아직 실행되지 않았고 `InputRouter`가 보스 사망·combat retirement 이후 `UIOnly`에서 틱과 UI 프레임을 계속 게시하는지 집중 검증이 필요하다.

`Combat.Unity`의 내부 handoff/체력 값은 기존 AssemblyInfo（`../../Assets/AcadeGameMaker/Runtime/Combat/Unity/AssemblyInfo.cs`；로컬 보존 자료）가 `Input.Unity`와 저작기 어셈블리에만 열어 둔다. Input.Unity의 AssemblyInfo（`../../Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`；로컬 보존 자료）는 이미 `Hub.Presentation.Unity`와 저작기에 친구 접근을 허용한다. 따라서 기존 친구 범위 안에서 단일 requester가 **값만** 결과로 게시하고, 한정 결과 화면이 `InputRouter.CurrentUiFrame`을 받아 재시도 의도를 소비하는 구성은 검토 가능하다. 새 친구 추가·반사·`ForTests` 런타임 호출·숨은 상태 직접 변경은 제안하지 않는다. `Input.Unity`에는 현재 TMP 어셈블리 참조가 없고 Hub.Presentation.Unity asmdef（`../../Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AcadeGameMaker.Hub.Presentation.Unity.asmdef`；로컬 보존 자료）에 TMP 참조가 있으므로 화면 표시는 그 소유 범위가 맞다.

결과 후 재시도는 내부 `CombatSession.ResetEncounter` 단독 호출이 아니라 **구간 그래프 전체를 종료·파기한 뒤 새 장면 또는 새 그래프 인스턴스로 다시 진입**하는 별도 한정 소유 경계가 필요하다. 이것이 플레이어·오르단 체력, 틱, 공격 순번, 전이 대상, 입력 라우터, 카메라, terminal requester를 새 세대로 만든다. 실제 `SceneManager` 전환·UI 선택·중복 재시도 차단·파기 순서와 새 입력 활성화는 아직 증명되지 않았으므로 제품 계약의 집중 검사에 넣는다. 현재 `Hub` 메뉴의 저장 초기화/C3·C4 실행 연결을 첫 구간의 묵시적 시작 경로로 복사하지 않는다.

## 미디어와 식별 표현

| 필요 표시 | 실제 현재 증거 | P1 계약에서 닫을 경계 |
|---|---|---|
| 플레이어·보스·전이 대상·지형 | 오르단 그래프에 물리 객체·콜라이더·픽셀 카메라가 있다. 프리팹 조사와 [감사02](./implementation-audits/2026-10-03-02.md)는 최종 플레이어/보스 렌더러·애니메이션 연결을 확인하지 못했다. 도언/오르단 원화·스프라이트 원본 시안은 최종 게임용 패키지 수용이 아니다. | 캐논 색·18 PPU·권리·방향/동작을 충족한 최소 실제 표현을 우선. 불가하면 식별용 **임시 기하 표시**의 용도·색/문구·종료 조건을 별도 제품 계약에 명시 승인하고 최종 아트 점수에 합산하지 않음 |
| 조작·체력·피격·성공/실패·재시도 | 허브 한국어 Regular/Bold SDF 글꼴（`../../Assets/UI/Fonts/Hub/`；로컬 보존 자료）은 감사02에서 현재 지문 일치·제한 단위 독립 수용. 같은 글꼴을 결과 화면에 쓸 수 있는 후보이나 새 문구/해상도/입력 화면은 미검증. | 최소 안내와 체력 변화·명중/피격·결과·재시도 버튼을 실제 장면에서 읽히게 하고, UIOnly 의미 프레임과 화면 반응을 함께 시험. 기존 허브 화면 전체를 가져오지 않음 |
| 음향 | 최초 체험의 필수 식별성·반응에 필요한 범위 아직 확정 전 | 사용 여부와 실제 장면 연결·권리를 P1 계약에서 판단; 재생 없음을 완성 음향으로 주장하지 않음 |

## 제안 파일 소유와 비순환 의존

| 한 묶음/담당 | 변경 가능성이 있는 정확 파일 범위 | 현재 상태·소유 경계 | 필요한 사전/집중 검사 |
|---|---|---|---|
| 제품 계약 — 아스트라, 루나 사전 검수 | 한정 P1 Approved 계약·REQ/AC·허용 파일 목록 | **미승인 구현**. 시작·결과·재시도 동작과 임시/최종 미디어 권한 먼저 고정 | 그래프·입력·미디어·빌드 의존의 DAG 순환0 |
| 단일 결과/모드 소유 — 테라 | `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs`와 새 한정 결과 값/검사 파일, 해당 PlayMode 시험 | 기존 성공/teardown 의미 유지, 실패/후속 UIOnly만 같은 requester에 추가. 두 번째 requester 금지 | 성공/실패/동시 사망·중복 요청·틱/모드/receipt 정확성 |
| 결과 화면·재시도 — 테라 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/`의 한정 화면 소유, 새 장면/프리팹·저작기/검증기 후보 | 기존 Hub 저장 초기화 권위와 분리. 화면 입력은 기존 UI semantic frame만 소비 | 결과 한 번, 재시도 한 번, 이전 그래프 파기·새 그래프 신원/틱/체력 |
| 표현 — 테라 제작, 루나 시각 검수, 아스트라 수용 | 최소 플레이어·오르단·전이/지형·UI 자료와 메타·저작 프리팹; 실제 파일은 계약에서 열거 | 기존 원화 시안은 최종 패키지 아님. 임시 기하는 명시 승인 전 사용 불가 | 출처/권리·규격·식별·방향/동작·실제 화면 |
| 시작·빌드 — 테라, 루나 실행 검수 | 한정 시작 장면·`ProjectSettings/EditorBuildSettings.asset`·빌드 수집 경로 후보 | 현재 Bootstrap 단독 설정과 초기 exe는 불변 역사. 새 로컬 프로젝트/빌드 출력은 별도 승인된 경로에서 | 현재 소스/장면 지문, 활성 장면, 빌드 종료·실제 입력/성공/실패/재시도 |

예상 선행은 **P0 계약 승인과 최소 미디어 권한 → 기존 그래프 보존 하 결과/재시도 연결 → 국소 실행 검사 → 정확 소스 빌드 → 루나 독립 실제 플레이 → 아스트라 한정 수용**이다. Q0의 동결 복사본/키/9회/원격 공개는 이 흐름의 묵시적 선행이 아니다. 로컬 통합은 원래 저장소·동결 Q0 파일을 직접 고치지 않는 **별도 승인된 격리 프로젝트/작업 사본**에서 하도록 계약에 소유 경로를 명시해야 한다. 이 조사는 그 사본을 만들지 않았고, 별도 로컬 빌드 실행 권한도 발급하지 않는다. 원격 게임 코드 게시에는 여전히 [ADR-0038](../adr/0038-forward-provenance-for-initial-code-publication.md)과 기존 Q0 계약의 다른 권위가 필요하다. P1 계약의 파일 목록이 Q0 570 입력/동결 경로와 겹친다면 충돌·해시 전이를 명시하고 아스트라가 분리해야 한다.

## 영향별 검증 계획 — 현재 실행 0

| 변경 행동 | 영향 REQ/AC | 집중 정상·음성 | 필수 회귀·재사용 경계 | 최종 판정 |
|---|---|---|---|---|
| 시작→실제 입력·이동/전이/공격 | REQ-PROC-001/002, AC-PROC-001/002; REQ-MOV/WT/COM/UX 해당 AC | 현재 입력 자산/카메라/그래프의 실제 한 틱·연속 틱, 잠금·대상 없음·중복 입력 | 기존 코어/실험장 정확 소스 증거는 영향없는 부분만 루나가 재사용 판정. 변경 파일 집중 PlayMode 필수 | 사용자가 빌드 시작부터 조작 |
| 플레이어 사망·오르단 격파→결과 | REQ-COM-004/006, AC-COM-002/003/004; AC-PROC-009 | 성공 triplet/terminal receipt, 실패 player 체력0, 동시/중복·오래된 틱·두 번째 requester 거절 | 기존 terminal/teardown·router 회귀; 모의 publication만으로 실제 화면 성공 주장 금지 | 실제 표시와 입력 잠금 |
| UIOnly→재시도 | REQ-UX-004, AC-UX-004; AC-PROC-009 | 실제 UI frame 선택, 두 번 누름/전환 중 입력, 이전 그래프 완전 파기·새 틱/체력/전이/입력 | Hub C3/C4는 별도 권위. 새 장면 경로의 집중 PlayMode 및 실패 정리 | 같은 구간 새 상태 진입 |
| 미디어·현재 빌드 | REQ-ART-002/003/005/008/012, AC-ART-001/002/005/009; REQ-PLAT-001/004/005, AC-PLAT-002/005; REQ-PROC-009 | 미디어 권리/식별·상태별 재생, 빌드 포함 장면/지문·Windows 시작/성공/실패/재시도 | 기존 글꼴 정확 바이트 수용은 재사용 후보; 신규 문구·표시는 새 시각 검수. 전체 VD-00 완료를 주장하지 않음 | 루나 독립 실제 플레이, 아스트라 수용 |

`REQ-PROC-005/AC-PROC-004`: 같은 원인 두 번이면 원시 보존·게임/도구/환경 분류·최소 재현·변경 예상 결과를 기록하고 큰 회귀를 멈춘다. 단회 계약이면 첫 실패부터 지킨다. `REQ-PROC-008/AC-PROC-007`: 최신 상태는 승인된 전략의 단일 작업판에 유지하고 이 조사 보고를 새로운 운영 상태표로 쓰지 않는다. `REQ-PROC-009/AC-PROC-008`: 최소 기본 표현만 먼저 수용하고 의상/후반 콘텐츠를 최초 체험의 자동 선행으로 묶지 않는다. `AC-PROC-009`는 실제 빌드·독립 입력/성공/실패/재시도 전에는 미충족이다.

이 조사 이후의 결정은 한정 체험의 전투 대상을 오르단으로 확정할지, 기존 requester 확장과 결과 UI/재시도의 파일 소유·표현 권한을 어떻게 Approved 계약에 적을지다. 이 보고는 제품 구현이나 새 Unity 실행을 시작하지 않았다.
