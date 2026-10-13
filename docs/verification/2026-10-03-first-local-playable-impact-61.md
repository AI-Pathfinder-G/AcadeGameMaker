# 최초 로컬 체험 영향 조사 재확인·차이 — 2026-10-03

- 성격: P0-LOCAL 읽기 조사. 제품 구현·수용·Unity 실행·빌드 권한을 발급하지 않는다.
- 실제 조사자: 테라 역할 `gpt-6.1-sol`. 기존 `gpt-6-sol`의 [조사](./2026-10-03-first-local-playable-impact.md)는 당시 실제 모델의 역사로 보존하며 수정하지 않았다.
- 사용자 동작·산출물: 시작→오르단 전투 조작→성공/실패 표시→새 상태 재시도를 위한 API·파일·검사 경계. 계약 소유자는 아스트라이며 [제품 계약](../specs/work-contracts/2026-10-03-first-local-playable-slice.md)은 이 조사 시점 Review다.
- 실제 선행: Approved [절차 전략](../specs/work-contracts/2026-10-03-play-first-delivery-strategy.md), AGENTS.md·문서 지도·용어·운영 모델·ADR-0037/0038/0039/0040 대조. 제품 구현은 계약 Approved 및 허용 입력/출력 부록 확정 이후다.
- 검사·종료: 소스의 정확 API·가시성·순서·참조와 자산/패키지 실물을 정적 대조하고 아래 차이를 계약 소유자에게 전달한다. 실행 검증0·제품 변경0·원격/키/세대 작업0이다. 일일 감사는 작성하지 않았다.

## 1. 결론과 이전 조사 재확인

기존 오르단 그래프가 가장 작은 후보라는 결론을 유지한다. `OrdanBossEncounterAuthoringBuilder.cs:64~87`은 기존 teardown·router·requester·카메라를 함께 구성한다. `InputRouter.cs:657~674`의 생산 초기화는 플레이어·전이·`OrdanBossCombatSimulationDriver`와 같은 틱/소유 결속을 요구한다. 일반 적 한 유형으로 바꾸는 작업은 이번 한정 연결보다 입력 일반화 범위가 크다.

두 번째 requester 등록은 허용되지 않는다. `InputRouter.cs:187~213`은 한 객체 신원에만 next tick 요청을 허용하며, 기존 requester가 Start에서 먼저 등록한다. `CurrentUiFrame`은 공개된 읽기 출처지만 모드가 UIOnly가 되어야 의미 프레임이 생긴다. 성공 Transition에 머무는 기존 객체만으로 결과 버튼이 작동하지 않는다.

**추천은 후보1인 기존 requester의 명시 로컬 체험 설정에 한정한 실패/후속 UIOnly 확장이다.** 원본은 보존하고 승인된 격리 복제본에서만 구현한다. 기본 설정은 기존 M5D1의 `IsClosed=true`·정확 성공 영수증·단회 전환 의미를 그대로 유지해야 한다. 로컬 설정의 opt-in과 설정 전용 저작 API는 제품 계약이 정확히 승인해야 한다. 새 범용 모드/프로필 프레임워크는 만들지 않는다.

단, 실패 결과 중 **모든 내부 시뮬레이션 정지**까지 이 추천이 증명하지는 않는다. 실패에서는 보스 사망 retirement가 없으므로 UIOnly 프레임 발행에 Movement/Transfer/Combat lane이 계속 필요하다. 최소 계약은 결과 중 사용자 gameplay 입력 차단, 최종 결과 스냅샷 고정, 내부 틱 유지로 한정하거나 별도 retirement 변경을 승인해야 한다. 살아 있는 보스/producer를 임의 disable하여 router의 후보 준비를 깨뜨려서는 안 된다.

## 2. 실제 API·순서·수명

| 직접 소스 근거 | 확인한 현재 동작 | 제품 계약에 필요한 한정 인터페이스/판정 |
|---|---|---|
| `OrdanBossEncounterHandoffAdapter.cs:16,36,132~148,204~213` | 실행 순서110에서 같은 완료 틱의 전투·전이·보스·피해·끌어당김을 검증한 불변 `LatestPresentationView`를 게시한다. `Combatants`는 `ordan`,`player` 순서다 | Input.Unity 내부의 읽기 전용 로컬 표시 값은 완료 틱·플레이어/보스 현재 체력·전이 상태·표적/예고를 복사한다. UI에는 가변 CombatSession이나 직접 피해 권한을 넘기지 않는다 |
| `CombatSession.cs:26~39` | 불변 `CombatantSnapshot`의 실제 이름은 `CurrentHealth`,`IsDead`,`TargetId`다 | 실패는 정확 player 식별·체력0/IsDead·같은 완료 receipt 틱을 요구한다. 현재 지문에서는 가상 `Health` API를 사용하면 안 된다 |
| `OrdanBossTerminalTransitionRequester.cs:16,119~188` | 순서-211에서 이전 완료 handoff와 router receipt를 읽어 다음 틱 Transition 요청, 그 요청 틱 receipt를 나중에 확인하고 닫는다 | 기존 성공 판정/영수증을 그대로 유지한다. 로컬 설정에서 완료 receipt 다음의 정확 UIOnly 요청·receipt와 불변 결과값을 별도로 게시한다. 오래된 틱·불일치·중복 결과는 거절한다 |
| `InputRouter.cs:720~777` | 순서-210에서 이전 보스 cleanup을 `MatchesRetiredInputGraph`로 확인하면 Combat 후보만 생략한다. Movement/Transfer는 계속 준비·검증·커밋하며 UIOnly 프레임을 생성할 수 있는 구조다 | 성공 cleanup 이후 UIOnly 실제 발행은 집중 실행 증명 대상이다. 실패에서는 Combat 생략 조건이 없으므로 내부 틱을 유지한다 |
| `OrdanBossTerminalTeardown.cs:15,32~40,116~151` | 순서-195의 기존 단회 정리는 정확 death+1 cleanup과 handoff를 확인하고 combat/bridge/scheduler/hostile/pull/handoff/ordan을 disable한다. player/transfer를 보존한다 | 성공에서 이 정리를 생략하지 않는다. result UI는 graph 밖에 두되 router는 기존 graph 안에 두며 재시도 의도 수락까지 생존시킨다 |
| `InputRouter.cs:1196~1251` | full simulation OnDisable은 fault와 callback 제거이며 Actions Dispose는 OnDestroy까지 보류된다. initialized router OnEnable은 예외다 | 같은 router를 재활성화하지 않는다. 재시도 의도/결과를 값으로 복사→old graph inactive→Destroy→실제 파기 완료 관측→new graph 생성/활성 순서를 계약에 고정한다 |
| `InputRouter.cs:237~245,618~620,682~690,946~988` | HubUIOnly는 pristine inactive null-reference graph와 prepared 소유·publication의 전용 상태를 요구한다. 단순 초기 모드 UIOnly를 설정하면 생산 초기화되지 않는다 | 후보2의 새 result router에 기존 HubUIOnly 저작만 복사하는 것은 불충분하다. ForTests나 반사로 준비 상태를 우회하지 않는다 |
| `DesktopProfileLaunchAdapterV1.cs` | 생산 prepared hub 소유는 프로필 준비·원래 cohort·후속 publication과 결속된다 | 후보2는 프로필무접촉 체험에 독립 생산 준비/소유 경계가 더 필요하다. 이 조사에서 그 확장을 승인하거나 구현하지 않는다 |

추천 전이표는 `GameplayEnabled → Transition(성공 정확 death+1, 실패 완료틱+1) → UIOnly(전환 receipt 다음 정확 틱) → 재시도 의도 1회 수락 → 이전 그래프 파기 대기 → 새 GameplayEnabled 그래프`다. 동일 완료 틱에서 player 사망과 보스 격파가 겹칠 때 최종 사용자 결과의 우선순위는 아스트라가 계약에 고정하며, 성공 관련 기존 terminal triplet/cleanup은 우선순위와 무관하게 보존한다. 변경 후 실행 전에 정확 실패 완료 틱·Transition/UIOnly 동안 handoff 계속 게시 여부를 작은 집중 검사로 확인한다.

입력을 누른 채 재시도할 때 새 Input System map 활성화가 held-state 이벤트를 재발행할 수 있으므로, 새 그래프 활성화만으로 자동발동0을 주장하지 않는다. 계약의 승인 범위에 로컬 신규 입력의 최초 release/neutral 장벽과 그 결과를 넣거나 실제 집중 검증으로 기존 경로의 충족을 입증해야 한다. 직접 장치 callback을 추가하는 방식은 제외한다.

## 3. 가시성·어셈블리 경계

- `Combat.Unity/AssemblyInfo.cs`는 Input.Unity·Combat.Authoring.Editor·해당 시험에 내부 접근을 허용한다. Hub.Presentation.Unity에는 허용하지 않는다.
- `Input.Unity/AssemblyInfo.cs`는 Hub.Presentation.Unity와 Combat.Authoring.Editor에 이미 내부 접근을 허용한다. 따라서 `Input.Unity`에 한정 불변 표시/결과 값과 저작 설정 API를 두고 requester가 복사하면 새 친구 선언 없이 UI로 전달할 수 있다.
- `Input.Unity.asmdef → Combat.Unity → Combat/Movement/Transfer/Core`; `Hub.Presentation.Unity.asmdef → Input.Unity/Input/Core/Profile/TMP`이 현재 방향이다. Hub.Presentation.Unity가 requester의 불변 값만 소비하면 Combat.Unity 참조를 추가하지 않는다. Input.Unity가 UI 어셈블리를 참조하면 순환이므로 금지한다.
- `Camera.Unity.asmdef → Input.Unity/Movement/Camera/Core/URP`. Input.Unity는 직렬화된 카메라 provider를 사용하므로 Camera.Unity를 역참조하지 않는다. 이 구조를 유지한다.
- 기존 `Combat.Authoring.Editor.asmdef`는 TMP/Hub.Presentation.Unity를 참조하지 않는다. 별도 체험 저작기를 이 기존 어셈블리에 넣는 경우 그 복제 asmdef의 명시 TMP/Hub.Presentation.Unity 참조를 허용 목록에 넣어야 한다. Editor-only → runtime 방향이므로 순환은 없다. 별도 새 저작 어셈블리를 택한다면 내부 저작 API 친구 추가가 생기므로 변경량을 비교해 승인한다.

제안 새 API 이름/파일명은 계약 부록 확정 대상이다. 필요한 내용은 한정 저작 설정, 완료 틱 불변 HUD 값, 불변 결과/모드 증거, 수명주기 소유자의 단회 재시도 의도 소비다. UI는 `CurrentUiFrame`의 실제 틱/epoch/출처를 검사하며 일반 EventSystem 장치 모듈이나 직접 InputAction 등록을 재시도 권위로 삼지 않는다.

## 4. 최소 빌드 입력·표현의 실제 권위

| 입력 범주 | 현재 실물과 권위 | 승인 부록의 최소 처리 |
|---|---|---|
| 공통/기능 소스 | Core, Movement, Transfer, Combat, Input, Camera 및 각 Unity 연결·asmdef·메타. Input.Unity는 Profile 및 프로필 준비/회복 소스를 컴파일 참조한다 | 전투 화면에서 프로필 기능을 호출하지 않더라도 Profile 소스를 멋대로 제외하면 컴파일 의존이 깨진다. asmdef 참조와 타입 참조 폐쇄를 먼저 고정한다 |
| 입력 | `Assets/GameInput.inputactions`·메타, `Runtime/Input/GameInputActions.cs`·메타, InputSystem 패키지 | 기존 실제 입력 자산/생성 wrapper를 그대로 복제한다. 새 callback/직접 ForTests를 생산 경로로 만들지 않는다 |
| 그래프·장면 | 기존 오르단 프리팹·메타, CombatEncounterPlayer 프리팹·메타, 오르단 실험장·메타와 해당 소스 GUID | 원본을 보존하고 새 한정 시작 장면/프리팹을 승인된 output 프로젝트에 작성한다. 원본 EditorBuildSettings는 Bootstrap 단독 활성 상태다. 새 체험 빌드는 새 설정/명시 장면 입력을 쓴다 |
| 카메라·렌더 | 픽셀 카메라·URP 참조. Camera.Unity는 URP Runtime/2D Runtime을 참조한다 | 기존 카메라 수치/프로필과 렌더 설정·renderer 자산/GUID 폐쇄가 필요하다. 과거 renderer 진단을 새 화면 판독/플레이 증거로 대체하지 않는다 |
| UI·검증 글꼴 | `Assets/UI/Fonts/Hub/`의 NotoSansCJKkr Regular/Bold OTF·SDF·각 메타·OFL-1.1. Hub.Presentation.Unity는 Unity.TextMeshPro 참조 | 기존 폰트 수용은 정확 바이트·권리의 후보 근거다. 새 문자열의 글리프·SDF 소재/shader·출력 해상도는 다시 검사한다. 필요 폴더 메타도 정확 목록에 포함한다 |
| 임시 기하 | 현재 저작기에는 최종 SpriteRenderer/Animator 연결이 없으며 물리 객체 배치가 있다 | 승인된 역할별 자체 기하+한글 식별만 사용한다. collider/공격범위를 가리지 않고 최종 아트/의상 수용으로 세지 않는다. 새 외부 생성/다운로드를 호출하지 않는다 |
| 패키지·설정 | Unity6000.6.0f1. manifest InputSystem1.20.0/URP17.3.0/TestFramework1.6.0/UGUI2.6.0; lock 실제 URP17.6.0/TestFramework1.8.0 | 기존 [버전 권위 대조](./2026-10-01-package-version-authority-crosscheck.md)의 승인 차이를 유지한다. 발견만으로 버전을 올리지 않는다. manifest·lock·ProjectVersion 및 실제 사용 ProjectSettings를 원문 입력으로 기록한다 |
| 폴더·자료 | 자산은 파일 본문뿐 아니라 부모 폴더 메타·GUID·asmdef와 패키지 소유 관계를 갖는다 | 파일 단위의 읽기 입력 목록과 신규/복제 수정 허용 목록을 구별한다. Library/Temp/Obj/Logs/기존 exe를 근거 없이 제품 입력으로 복사하지 않는다 |

위 표는 최소 **범주**이며 exact path 허용 목록이 아니다. 실제 격리 프로젝트의 범위·입력 파일 목록/지문·신규 파일명·수정 복제 파일·출력 경로·실행 시간 상한은 아스트라가 제품 계약 부록에서 확정해야 한다. 이 조사에서는 output 디렉터리/프로젝트를 생성하지 않았다.

## 5. 집중·필수 회귀·같은 바이트 증거 재사용

| 변경 경계 | 집중 검사 후보 | 필수 영향 회귀 후보 | 동일 바이트 재사용 한계 |
|---|---|---|---|
| requester 로컬 opt-in/결과 | 정확 성공 triplet→Transition→cleanup→UIOnly, 실패 완료 player snapshot→Transition→UIOnly, 동시 사망, 결과 중복/오래된 틱, 기본 opt-in=false | `OrdanBossTerminalTransitionRequesterPlayModeTests`, `InputRouterTerminalPlayModeTests`, `OrdanBossTerminalTeardownPlayModeTests`, `OrdanBossEncounterHandoffAdapterPlayModeTests` | 기본 M5D1 의미/검사는 유지한다. 복제 requester가 바뀌면 과거 requester 통과를 현재 통과로 재사용하지 않는다 |
| 결과 UI·router 프레임 | 실제 UI frame click/submit→단회 재시도, stale epoch/held 입력/중복 클릭, gameplay 잠금과 최종 결과 고정 | `InputRouterPlayModeTests`, `InputRouterFailureBoundaryPlayModeTests`, `InputRouterAdversarialPlayModeTests`, UI semantic 관련 기존 검사와 신규 한정 표시 검사 | 기존 Hub C3/C4/Q0 9회는 새 UI·재시도 증거가 아니다. 기존 계약이 요구한 회귀는 그대로 유지한다 |
| graph destroy→새 graph | production Start/FixedUpdate, old OnDisable callback 제거와 destroy 후 disposal, 새 wrapper/틱/체력/전이/카메라/공격 순번, 연속2회·누른 채 입력 | `OrdanBossEncounterAuthoredGraphPlayModeTests`, `OrdanBossCombatInputGatePlayModeTests`, `InputRouterActualCameraPlayModeTests`, `SimulationCameraDriverPlayModeTests`와 이동/전이 영향 검사 | 순수 코어 수치/알고리즘의 byte와 실행환경이 같고 영향없음을 Luna가 판단한 부분만 후보. 신규 수명주기를 대신하지 않는다 |
| 새 저작·표현·빌드 | 실제 장면 첫 틱·HUD, 누락/중복 GUID/컴포넌트, 한글/역할 구분·해상도, production build 활성 장면·새 산출 지문 | `OrdanBossEncounterAuthoringTests`와 새 체험 저작 검증·컴파일/사용 패키지 회귀; 빌드 동일 산출물 실제 독립 플레이 | 글꼴 권리/바이트와 미변경 자산 근거만 범위 재사용. 새 장면/새 문자열/빌드/실제 플레이는 신규 증거 필수 |

이는 실행 선택안이며 필수 시험의 확정/실행 완료가 아니다. 정확 시험 선택·개수·기존 상위 계약별 필수 전체 회귀·상한·수행자는 Luna 사전 검토와 Astra Approved 부록에서 고정한다. 제품 AC-LPS-001..009와 절차 AC-PROC-002/003/005/009를 추적하며 현재 실행 AC 충족은 주장하지 않는다. 같은 실패 두 번이면 원시·최소 재현·변경/새 관측 목적을 정리하고 큰 반복을 멈춘다. 단회 계약은 첫 실패부터 제한한다.

## 6. 재확인 바이트

| 읽은 원문 | SHA-256 |
|---|---|
| 기존 영향 조사 | `D59EE22448C7A4274284ED84AD2376509B5CED5DF01D44E8FC92DDE9FFEBBBC4` |
| InputRouter.cs | `5A210D7ED80DE86C9D3092865E7F4712C00C65B84372E479018BF43956F6695F` |
| OrdanBossTerminalTransitionRequester.cs | `46CF17CAE4046FF9BCA7E06D5AED40666D56D6BF7AEA96380FAEC971DE297D32` |
| OrdanBossEncounterHandoffAdapter.cs | `8F57C7123031629EB30FBBA0D16261ECF213D28B267358BDCCE1E617642A9761` |
| OrdanBossTerminalTeardown.cs | `D737A32490403AAEC4332DC90808ED3F0D4B49A0FA4BD03E4D332CB8F6D23B1E` |

조사는 위 원문과 프로젝트 입력을 읽고 새 문서만 작성했다. 원본 제품/Q0 동결 소스·계약·인계·증거·감사 파일은 이 작업에서 수정하지 않았다. Q0-R4 운영 인간 답변 대기는 그대로 별도이며 제품 구현0·실제 플레이0이다.
