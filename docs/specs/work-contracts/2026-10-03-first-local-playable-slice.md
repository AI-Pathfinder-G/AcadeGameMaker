# 최초 로컬 플레이 구간 통합 계약

- 상태: Approved — 2026-10-03 실제 아스트라의 C1-A 한정 제품 승인. 정확 범위와 단계별 실행 조건은 최신 §11이 우선한다. CH1 전체·Q0·원격 승인은 아니다.
- 설계·계약·통합 수용: 실제 아스트라 `gpt-6-astra`.
- 향후 구현·영향 조사: 테라 역할 `gpt-6.1-sol` — 2026-10-03 사용자의 최신 모델 통일 지시를 따른다. 독립 사전·사후 검수: 루나 `gpt-6-luna`. 기존 `gpt-6-sol`의 읽기 조사 사실은 당시 모델의 역사로 보존한다.
- 작업판: ADR-0042 및 [CH1 전체 체험 계약](./2026-10-03-chapter1-playable-delivery.md)의 C1-A/P1-CH1-BASE. 이 한 전투는 입력·표시·그래프 교체 기반의 개발 검증이며 최종 사용자 목표는 CH1 완주다.
- 상위 권위: ADR-0037/0039, Approved VD-00~10 및 해당 부품 계약. 전체 데모·본편의 수량과 기존 AC는 변경하지 않는다.
- 현재 단계: 원문398과 루나 사전 검수를 채택하여 정확 격리 복제·C1-A 구현을 시작할 수 있다. 실제 제품 구현·Unity·빌드·플레이 결과는 아직 없으며 단계별 관문을 따른다.

## 1. 사용자에게 전달할 한 구간

Windows 로컬 실행 파일을 시작하면 조작 안내와 함께 한 장면에서 플레이어를 움직이고, 점프·대시·조준·무게 전이/회수·기본 공격으로 기존 오르단 전투를 경험한다. 플레이어와 보스 체력, 전이 상태, 성공 또는 실패를 화면에서 확인하고, 결과 화면의 재시도로 같은 구간을 완전히 새로운 상태에서 다시 시작한다. 종료는 정상적인 창 종료를 지원한다.

이는 최초 로컬 체험이며 데모 성공 사건 `DemoCompleted`, 원정 완료, 보상 지급, 인간성 선택, 세령 등장, 저장 진행도를 발생시키지 않는다. 전체 데모의 거점1·저작방6/런4·일반 적2·선택1/기술2·12~18분과 본편 CH1~CH8는 그대로 남는다. 이번 한 구간을 챕터 완주나 전체 데모 완료로 표시하지 않는다.

첫 전투는 일반 적 한 종류를 새로 연결하는 대신 **현재 오르단 그래프 한 개**를 재사용하는 설계 후보로 정한다. 적의 수가 아니라 승인된 입력·카메라·전이·전투 소유 경계의 변경량을 최소화한다. 오르단 체력60과 기존 패턴·피해·이동 수치를 낮춰 편의를 만드는 변경은 포함하지 않는다. 실제 독립 플레이가 불가능한 결함이나 조작 문제가 드러나면 원인을 기록하고 해당 범위만 재설계한다.

## 2. 현재 근거와 재사용 경계

| 현재 부품 | 직접 확인한 연결·권위 | 이번 사용과 남은 연결 |
|---|---|---|
| `Assets/Scenes/OrdanBossEncounterSandbox.unity`, `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` | 기존 저작 그래프 후보. 실제 인스턴스의 참조·렌더·실행 가능성은 새 검증 대상 | 한 장면의 전투 원본으로 격리 복제. 원본 자산 수정 금지 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs` | 플레이어·전이·보스 전투·스케줄러·피해·handoff·카메라·입력·terminal 연결 | 기존 구성·실행 순서를 보존하고 새 체험 장면·화면 연결은 별도 빌더가 담당 |
| `Runtime/Input/Unity/InputRouter.cs` | 유일한 장치 callback 소유자. `ConfigureForAuthoring`은 `OrdanBossCombatSimulationDriver`를 받음 | 기본 키보드/마우스와 기존 XInput 경로 유지. 새 직접 장치 callback 금지 |
| `Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs` | `LatestPresentationView`의 완료 tick·체력·사망·handoff 읽기 | 체력 표시와 실패 판정의 읽기 출처. UI가 체력이나 피해를 직접 수정하지 않음 |
| `Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs` | 보스 사망의 정확 triplet과 입력 전환 완료 영수증. 단일 mode requester 등록 | 성공 판정은 완료 영수증에 결속. 기존 보상/방 완료 요청을 실제 원정 성공으로 소비하지 않음 |
| `Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs` | 보스 사망 뒤 실제 전이·전투 정리 경계 | 새 성공 화면이 기존 정리를 건너뛰지 않음 |
| `Runtime/Input/Unity` 어셈블리 | 이미 `Combat.Unity` 내부 읽기 권한이 있음 | §9의 불변 표시값을 이 기존 어셈블리 안에 추가. §10.5의 정확 Editor 친구 외 새 friend·반사·시험 전용 API 우회 금지 |
| `CombatEncounterGraph`의 일반 적 구성 | Walker+Surveyor 두 역할, 생산 InputRouter/카메라 직접 연결 없음 | 이번 최초 체험에서 제외. 단일 적 구성을 위해 기존 역할 계약이나 router를 일반화하지 않음 |

표의 `Runtime` 경로는 `Assets/AcadeGameMaker/Runtime` 아래다. 현재 Unity 버전 파일은 `6000.6.0f1`이다. 실제 프로젝트 복제 때 패키지·설정·자산 GUID·글꼴·렌더러의 고정 입력 원장을 만들고 검사한다. 파일 존재만으로 현재 빌드·플레이 통과를 주장하지 않는다.

## 3. 권위·파일 소유와 격리

구현 승인 시 사용할 제안 루트는 `output/local-playable-slice-r1/project/`, 빌드 출력은 `output/local-playable-slice-r1/build/`, 증거는 `output/local-playable-slice-r1/evidence/`다. 이 경로는 기존 Q0 prep/run/evidence와 분리한다. 원본 프로젝트의 읽기 복제만 허용하고 `Library`, `Temp`, 실행 출력, 개인 프로필, `.git` 및 Q0 복제 루트를 소스처럼 재귀 복사하지 않는다. 정확 복제 입력은 Terra의 의존 목록과 Luna 검토 뒤 승인 부록에 고정한다.

| 소유 | 허용할 후보 경로·범위 | 금지·승인 전 조건 |
|---|---|---|
| Astra | 이 계약과 전략 문서의 P0/P1 현재 작업 행 | Q0 계약·동결 소스·인계·증거 수정 금지 |
| Terra | 격리 프로젝트 `Assets/AcadeGameMaker/Runtime/Input/Unity/LocalPlayableSlice*.cs` 및 `.meta` | 새 장면 수명주기·읽기 표시·재시도만. 정확 파일명과 소유 API는 §9에서 확정 |
| Terra | 격리 프로젝트 `Assets/AcadeGameMaker/Editor/CombatAuthoring/LocalPlayableSliceAuthoring*.cs`, 빌드 도구 및 `.meta` | 기존 그래프 빌더·validator를 활용. §9의 Editor asmdef 두 참조만 한정 후보. §10.5의 정확 Editor 친구 외 AssemblyInfo friend 확장 금지 |
| Terra | 격리 프로젝트 `Assets/Scenes/LocalPlayableSlice.unity`, `Assets/Prefabs/LocalPlayableSlice/`, `Assets/LocalPlayableSlice/Presentation/`와 메타 | 기존 장면/프리팹을 덮어쓰지 않음. 정확 생성 자산 목록·참조를 검증 |
| Terra | 격리 프로젝트의 집중 EditMode/PlayMode 시험 및 명시 빌드 설정 | 기존 시험을 삭제·무력화하거나 성공으로 바꾸지 않음. 설정의 정확 변경 키를 승인 부록에 고정 |
| Luna | 독립 검수 기록·화면 관측·실제 빌드 입력 기록 | 구현자의 자기 검수를 독립 수용으로 표시하지 않음 |

기존 소스 변경이 반드시 필요하면 격리 복제본의 정확 파일·메서드·변경 이유·기존 수용 영향만 승인 부록에 추가한다. `InputRouter`·terminal requester·asmdef를 위 와일드카드가 암묵적으로 허용하지 않는다. 원본으로의 역반영은 이 계약의 자동 단계가 아니며 후속 통합 판정 대상이다.

### 로컬 실행과 Q0의 관계

ADR-0038 및 Q0 계약은 해당 원격 게시·정확 검증 큐의 권위다. 일반 제품의 로컬 설계 전체를 금지하는 것으로 확대하지 않는다. 반대로 이 계약은 Q0의 인간 승인·키·세대·서명·9회1654·원격 공개문서2의 관문을 대신하지 않는다. 별도 격리 체험은 실제 생산 게임 입력과 Unity 수명주기를 사용하며 Q0 검사기 또는 신뢰 자식의 입력을 위조하지 않는다.

현재 직접 확인한 이동·전투 FixedUpdate 경로에는 Q0 운영 권위 직접 참조가 없다. 다만 라이선스·패키지·렌더러·소스/API 및 기존 필수 회귀 의존은 실제 조사로 확정한다. 게시 완료는 설계의 선행이 아니며, 로컬 빌드/플레이 승인은 이 제품 계약의 정확 실행 범위로 별도 명시해야 한다.

## 4. 행동·상태 요구사항

- **REQ-LPS-001 — 시작:** 독립 실행 파일의 기본 시작 장면은 이번 구간이다. 사용자가 Editor 메뉴나 시험 helper를 호출하지 않아도 안내와 조작 가능한 장면에 도달한다. 기존 Bootstrap/Hub의 프로필 초기화 흐름을 임의로 호출하지 않는다.
- **REQ-LPS-002 — 생산 입력:** 기존 `GameInput.inputactions`·생성 wrapper·InputRouter를 사용한다. 이동·점프·대시·공통 조준·전이/회수·기본 공격은 실제 사용자 입력에서 기존 소비 경로로 전달된다. `ForTests` 호출·반사·직접 피해 주입·자동 승리 입력은 실제 플레이 증거에서 금지한다.
- **REQ-LPS-003 — 한 전투:** 기존 오르단 그래프의 등록·tick 순서·패턴·전이 노출·피해·무적·terminal 정리를 보존한다. 두 번째 시뮬레이션 시계나 체력 저장소를 UI에 만들지 않는다.
- **REQ-LPS-004 — 판독:** 플레이어/보스/전이 대상/안전 지형/위험을 형태와 표시로 구별한다. 플레이어와 보스의 현재/최대 체력, 전이 상태, 조작 안내가 실제 원천과 일치한다. 누락 참조·글리프·범위 초과 값을 숨기지 않고 초기화 실패로 보고한다.
- **REQ-LPS-005 — 결과:** 새 체험 수명주기는 `진입 중 → 조작 중 → 성공 또는 실패 → 재시작 중`을 소유한다. 성공은 기존 보스 terminal 완료 영수증 이후, 실패는 완료된 presentation의 player 사망 이후 각각 한 번만 확정한다. 같은 확정 tick에 둘 다 참이면 실패를 우선한다. 이는 체험 결과 규칙이며 데모 분기/보상 규칙을 변경하지 않는다.
- **REQ-LPS-006 — 단일 입력 소유:** 결과·재시작 중에는 실제 gameplay 입력이 새로운 공격·전이·이동을 만들지 않는다. 기존 mode requester와 새 수명주기 사이의 소유권을 §9의 단일 소유 설계로 확정한다. 두 소유자의 중복 등록·강제 우회·기존 receipt 위조를 금지한다.
- **REQ-LPS-007 — 새 상태 재시도:** 결과 화면의 재시도는 이전 그래프와 callback·전이·카메라·피해 예약을 폐쇄하고 같은 저작 그래프를 새 인스턴스로 시작한다. 사망한 graph의 부분 reset이나 죽은 적 부활 주입을 금지한다. 플레이어/보스 체력·위치·전이·대시·공격 상태·결과 latch는 새 시작 상태이며 누른 채 이어진 입력은 새 명시 입력 전 발동하지 않는다.

  현재 full-simulation InputRouter의 `OnDisable`은 fault latch와 callback 제거를 수행하지만 모든 action Dispose 완료와 같지 않다. 비활성화한 기존 router를 다시 활성화하지 않는다. 폐기 방식은 `OnDestroy`와 실제 구독/입력 폐쇄를 확인한 뒤 새 그래프를 생성해야 하며, `SetActive(false)`만으로 재시도 준비 완료를 주장하지 않는다.
- **REQ-LPS-008 — 비지속 체험:** 실제 사용자 저장·설정·키 저장소·네트워크·원격을 사용하지 않는다. 결과는 로컬 비지속 상태다. 새 실행은 새 체험이다. 기존 profile 파일 생성·변경이 없음을 실행 전후 확인한다.
- **REQ-LPS-009 — 표시 자산 권위:** 재사용 미디어는 실제 파일·권리·규격·검수 범위가 확인된 것만 포함한다. 아래 한정 임시 표현은 이 계약에서 명시 수용하기 전까지 적용하지 않는다. 미수용 세령/도언 원화나 시안을 자동으로 게임 외형에 넣지 않는다.
- **REQ-LPS-010 — 현재 소스 빌드:** Unity6000.6.0f1 및 고정 패키지/설정으로 Windows x64 빌드를 만든다. 실제 포함 소스·장면·자산·버전/빌드 설정의 원문 지문과 빌드 산출물 지문, 명령·로그·종료를 결속한다. 이전 초기 exe를 새 전달물로 사용하지 않는다.
- **REQ-LPS-011 — 독립 실제 플레이:** Luna가 그 빌드를 실제 실행하고 장치 입력·화면·성공·실패·재시도를 독립 확인한다. 자동 집중 검사나 녹화만으로 실제 입력 플레이를 대체하지 않는다. 실행 도구가 실제 장치 입력을 제공하지 못하면 미검증으로 남겨 실행 가능한 검수 경로를 정한다.
- **REQ-LPS-012 — 근거·종료:** 구현자는 각 변경에 REQ를, 검수자는 결과에 AC를 기록한다. 정상 집중 경로 후 필수 회귀·현재 빌드·독립 플레이가 모두 닫혀야 이 체험을 수용한다. 반복·실패는 전략 REQ-PROC-003..007을 따르고 계약상 단회 제한을 지킨다.

## 5. 임시 표현의 한정 승인 제안

최종 인물·보스 아트 패키지는 이 체험의 자동 선행으로 삼지 않는다. 기존 승인 미디어를 먼저 조사하고 사용할 수 없는 역할에만 자체 생성 단색 기하 도형·선·한글 역할/상태 표시를 제안한다. 외부 생성 API·새 원화 제작·의상 확장은 포함하지 않는다. 제목과 안내에 “로컬 조작 체험 · 임시 표현”을 표시하며 실제 도언/오르단 외형의 완성·채택으로 주장하지 않는다.

임시 기하는 플레이어·보스·전이 물체를 서로 다른 실루엣/테두리와 역할 이름으로 구별하고 실제 collider/피해 범위를 가리지 않아야 한다. 의미색·전이 판독·조준은 기존 VD-07/08과 충돌하지 않아야 한다. 기본 한글 글꼴은 기존 승인 자산의 라이선스·실제 글리프·shader 의존을 확인하고, 새 문자열의 누락 글리프를 집중 확인한다. 흑백/색각 의존만으로 상태를 구별하지 않는다.

임시 표현은 이번 독립 플레이의 식별·판독 조건을 통과하는 범위만 수용한다. 아트 캐논·최종 스프라이트·의상 제작 점수를 올리지 않는다. 정식 아트 교체는 별도 규격/시각 검수와 입력·피해 가시성 회귀 후 종료하며, 이번 계약은 임시 표현의 영구 제품 채택을 승인하지 않는다.

## 6. 비순환 의존과 검사 계획

`실제 부품/권위 조사 → 단일 소유/API·파일·미디어·검사 부록 확정 → Luna 사전 검토 → Astra Approved → 격리 입력 동결 → 구현 → 실제 정상 입구 집중 검사 → 영향 필수 회귀 → 현재 소스 빌드 → Luna 실제 플레이 → Astra 한정 수용`의 순서다.

Q0-R4 인간 승인 대기는 위 제품 조사와 별도 가지다. 제품 그래프가 실제로 Q0 권위를 요구한다는 근거가 새로 발견되면 해당 호출/계약을 기록하고 분리 설계를 검토한다. 존재하지 않는 공개 권위나 미래 검수 결과를 준비 입력으로 요구하지 않는다. 독립 최종 수용을 구현 시작의 전제로 두지 않는다.

| 변경·행동 | 집중 검사 | 필수 회귀·영향 경계 | 재사용 가능한 근거 |
|---|---|---|---|
| 시작·직렬화 그래프·필수 자산 | 실제 시작 장면→참조→입력 초기화→첫 완료 tick→HUD 연결. 장면0/1,누락/중복1개를 작은 경계로 확인 | 실제 사용 Ordan 저작 그래프·입력 초기화·카메라·표시 관련 기존 검사 | byte가 같은 기존 구성/수치의 역사 검수는 범위 설명. 새 장면 통과 대신 사용하지 않음 |
| 단일 수명주기·결과·재시도 | 성공·실패·동일 tick 경합·재시도 연속2회·누른 입력·구독 중복/오래된 요청 거절 | 기존 terminal triplet/teardown, 모드 lock·receipt, 전이 종료, 공격/피해 순서 관련 영향 시험 전부 | 미변경 순수 코어 증거는 정확 바이트·환경 비교와 Luna 동의 후만 사용 |
| HUD·임시 표시 | 체력 원천 일치, 상태0/1/복수,한글 글리프·겹침·식별·필수 해상도 | VD-07/08의 실제 사용하는 레이아웃·조준 판독 범위 | 원화 수용과 폰트 권리 근거만. 새 화면 시각 검수는 새로 수행 |
| 복제·빌드 | 전체 복제 입력 누락/중복·GUID·asmdef·packages·설정, 실제 생산 빌드 입구와 활성 장면 | 빌드에 포함된 새/변경 소스의 컴파일 및 사용 그래프 검사; 정확 목록은 §9에서 고정 | 기존 Q0 지원4나 역사 C3/C4의9회는 이번 빌드·플레이 증거가 아님 |
| 실제 전달 | 현재 빌드 새 시작→조작·전투→성공/실패→재시도, 로그·저장 불변 | 독립 키보드/마우스 경로 필수. 기존 게임패드 경로의 검사 가능성과 미검증 범위를 부록에 명시 | 새 실행 입력/원시/화면만 이 체험의 플레이 수용 근거 |

현재 표와 §9는 검사 범위를 정한다. 정확 기존 시험 이름·선택·개수·필수 전체 회귀 여부·소스 지문·시간 상한·실행 소유자는 Terra 영향 조사와 Luna 검토 후 §9 부록에서 고정한다. 기존 계약의 필수 회귀나 Q0의9회를 이 문서로 생략하지 않는다. 관련 없음이 확인된 Q0 전용 회귀를 이유 없이 새 제품 검사에 중복 추가하지도 않는다.

## 7. 수용 기준

| AC | 재현 가능한 완료 조건 | 추적 |
|---|---|---|
| AC-LPS-001 | 새 실행에서 별도 Editor 명령 없이 정확 시작 장면·조작 안내·생산 입력 첫 tick이 준비되고 플레이어를 움직인다. 누락 그래프는 명시 실패하며 불완전 조작을 허용하지 않는다 | REQ-LPS-001/002 |
| AC-LPS-002 | 실제 입력으로 좌우 이동·점프·대시·조준·전이 성공/회수·기본 공격이 기존 경로를 통과하고 화면 피드백과 일치한다. 직접 피해/ForTests 주입0,중복 callback0 | REQ-LPS-002/003/004 |
| AC-LPS-003 | 플레이어와 오르단 체력 표시가 완료된 동일 출처와 일치한다. 실제 전투에서 피격/체력감소를 관측하고 보스 종료 영수증 이후에만 성공,player 사망 뒤에만 실패가 각각 한 번 발생한다 | REQ-LPS-003..006 |
| AC-LPS-004 | 성공과 실패 각각에서 재시도 후 새 그래프·초기 상태로 진입한다. 연속2회 재시도에서 오래된 손상/전이/모드요청·중복구독0,누른 채 입력의 자동발동0 | REQ-LPS-006/007 |
| AC-LPS-005 | 결과/재시작 중 gameplay 입력이 차단되고 허용 UI 동작만 소비된다. 완료/실패 경합은 규정 우선순위로 한 번 결정되며 기존 terminal 정리를 생략하지 않는다 | REQ-LPS-005/006/007 |
| AC-LPS-006 | 자산의 역할·출처·권리·임시 표기와 실제 글리프/구분성이 독립 검수된다. 필수 해상도에서 조작/체력/결과가 잘리거나 겹치지 않으며 최종 아트 채택으로 표시하지 않는다 | REQ-LPS-004/009 |
| AC-LPS-007 | 정확 원문 입력·활성 장면·설정과 Windows x64 빌드 산출물 지문·로그·native 종료가 결속된다. 필수 집중/회귀 누락0이며 이전 exe 재사용0 | REQ-LPS-010/012 |
| AC-LPS-008 | Luna가 동일 빌드의 실제 사용자 입력 성공·실패·재시도와 처리되지 않은 예외/누락참조0을 확인한다. 자동 시험과 실제 플레이를 구분하고 실제 profile/키/원격 변경0을 검증한다 | REQ-LPS-008/010/011 |
| AC-LPS-009 | 원본/Q0 동결 자료의 변경0,허용 목록 밖 제품변경0,독립 검수 P0/P1=0,시작 안내와 한계가 전달된다. 전체 데모/본편 완료를 주장하지 않는다 | REQ-LPS-008..012 |

## 8. 전달물과 종료 조건

현재 소스가 들어간 로컬 빌드 폴더, 한글 시작 안내(실행 위치·기본 조작·목표·재시도·임시 표현·제외 범위), 입력/산출 지문 원장, 집중·필수 회귀 원시, Luna의 실제 플레이 근거와 Astra 한정 수용을 함께 전달한다. 새 증거 때문에 기존 실패를 삭제하거나 성공으로 바꾸지 않는다.

완료 시 구현 감사의 기존 inventory-v1에서 실제 달라진 단위만 재평가한다. 임시 형태는 최종 아트 점수에 더하지 않고 한 장면 실제 연결은 해당 한정 단위에만 기록한다. 문서/도구/시험 수를 제품 전체 분모로 사용하지 않는다. 감사 보고 작성 소유는 현재 별도 감사 작업에 있으며 이 계약 작성자가 해당 파일을 덮어쓰지 않는다.

## 9. 첫 연결의 확정 설계와 승인 부록

2026-10-03 실제 테라 `gpt-6.1-sol`의 영향 조사 `docs/verification/2026-10-03-first-local-playable-impact-61.md`(SHA F8102229061739AEF60DFBA24A07A74559850BE10E4DFACCFBA503CC2B66962C)를 반영한 설계다. 아직 Review이며 아래 정확 실행 부록과 Luna 검토가 구현 관문이다.

### 9.1 입력·결과·그래프 소유

기존 `OrdanBossTerminalTransitionRequester`에 직렬화된 로컬 체험 opt-in을 기본 false로 추가한다. false 경로는 기존 `IsClosed`·완료 receipt·단일 Transition 동작을 보존한다. true에서만 완료된 handoff의 player 사망을 결과로 읽고, 기존 유일 mode requester가 다음 정확 tick Transition 및 이후 UIOnly receipt를 검증한다. 두 번째 requester와 장치 callback은 만들지 않는다. 같은 완료 tick의 플레이어/보스 사망은 표시 결과를 실패로 고정하되 기존 보스 triplet/teardown은 그대로 처리한다.

실패 결과에서 전체 시뮬레이션 정지를 요구하지 않는다. UIOnly 발행에 필요한 내부 Movement/Transfer/Combat tick은 유지하고 gameplay 입력을 차단한다. HUD/result는 결과 확정 때 불변 snapshot으로 고정한다. 성공 퇴역 뒤 같은 보스 router를 GameplayEnabled로 되돌리지 않는다. 결과를 시각적으로만 덮고 실제 입력을 허용하는 구현은 거절한다.

Input.Unity의 새 불변 표시값 API를 Hub.Presentation.Unity가 기존 friend 경계로 읽는다. Combat 상태나 체력 쓰기를 Hub에 공개하지 않는다. UI 의미 프레임을 고정 tick마다 한 번 소비하고 렌더 단계는 마지막 불변 표시만 그린다. 별도 UI 장치 구독·Unity 버튼의 독립 InputSystem 소비는 금지한다.

상위 장면 수명 소유자는 결과/UIOnly 영수증 이후 재시도 의도를 한 번 받아 이전 그래프를 비활성화하고 Destroy한다. OnDestroy에 따른 action disposal·callback 해제와 실제 객체 파기를 확인하기 전 새 그래프를 만들지 않는다. 새 그래프의 로컬 시작 장벽은 기존 InputRouter 입력 읽기 경로에서 관련 버튼 release 및 이동/조준 축의 정의된 neutral을 확인한 뒤 연다. 누른 채 입력을 새 press로 소비하지 않는다. 새 장치 callback·직접 InputSystem polling 우회 없이 이를 검증할 수 없는 경우 해당 좁은 API를 다시 검토한다.

이 그래프 교체 소유권은 CH1의 향후 전투→선택→봉쇄선 그래프 교체에 재사용한다. 이번 개발용 성공→재시도는 CH1 성공 종료 권위가 아니다. 퇴역한 보스 그래프를 되살려 ChoiceSkill을 연결하지 않으며 CH1 후속 gameplay 그래프와 Run/Choice 소비 API는 C1-B/C에서 별도 승인한다.

### 9.2 정확 제품 파일 후보

모두 격리 프로젝트 안의 상대경로다. 다음 목록 외 변경은 자동 허용되지 않는다. 메타는 대응 파일과 함께 소유한다.

| 파일 | 한정 변경/API |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs` | 기본false opt-in, 기존 성공 receipt 보존, 로컬 결과/UIOnly 승계 및 불변 표시값 게시 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | opt-in 시작 release/neutral 장벽과 파기 완료 확인에 필요한 좁은 읽기 상태. 기존 false 경로·맵·callback 소유 불변 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/LocalPlayableSliceView.cs` | 완료 tick/체력/전이/결과의 불변 값. 수치 변경 권한 없음 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/LocalPlayableSlicePresenter.cs` | UI 의미 프레임 소비·TMP 표시·그래프 교체 수명 소유. Combat 직접 참조 금지 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/LocalPlayableSliceAuthoringBuilder.cs` | 기존 고정 graph prefab을 읽기 재사용, 새 장면/표시/참조 구성 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/LocalPlayableSliceBuild.cs` | 고정 장면의 Windows x64 빌드와 원문 입력/출력 원장 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/AcadeGameMaker.Combat.Authoring.Editor.asmdef` | `Unity.TextMeshPro`, `AcadeGameMaker.Hub.Presentation.Unity` 두 참조만 추가. runtime 역참조 없음; §10.5의 정확 Editor 친구만 예외 |
| `Assets/Scenes/LocalPlayableSlice.unity`, `Assets/Prefabs/LocalPlayableSlice/LocalPlayableSliceGraph.prefab` | 새 자산. 기존 Ordan root 이름/참조 검증 유지, 원본 자산 덮어쓰기 없음 |

순환은 Editor→Hub→Input→Combat 방향으로 차단한다. Input→Hub 참조를 만들지 않는다. 위 미확정 실제 폴더 위치·추가 재료 자산·시험 파일은 구현 전에 원문 기반 입력 부록에서 고정한다. 이름이 비슷하다는 이유로 새 assembly를 만들지 않는다.

### 9.3 집중·필수 회귀

새 집중 시험은 기본false 기존 성공, opt-in 성공/실패/동시 사망, Transition/UIOnly 정확 tick, 결과 latch, UI 의미 프레임 1회 소비, held release/neutral, Destroy 전에 새 생성 거절, 연속 재시도2회와 장면 종료를 포함한다. 정상 실제 저작 그래프 초기화→첫 tick→표시를 음성/대규모 회귀보다 먼저 확인한다.

기존 필수 영향 클래스는 `OrdanBossTerminalTransitionRequesterPlayModeTests`, `InputRouterTerminalPlayModeTests`, `OrdanBossTerminalTeardownPlayModeTests`, `OrdanBossEncounterHandoffAdapterPlayModeTests`, `InputRouterPlayModeTests`, `InputRouterFailureBoundaryPlayModeTests`, `InputRouterAdversarialPlayModeTests`, `OrdanBossEncounterAuthoredGraphPlayModeTests`, `OrdanBossCombatInputGatePlayModeTests`, `InputRouterActualCameraPlayModeTests`, `SimulationCameraDriverPlayModeTests`, `OrdanBossEncounterAuthoringTests`다. 이동/전이와 상위 계약의 추가 필수 선택은 영향 부록에서 확정하며 이름만 같은 시험을 검증 완료로 세지 않는다. 기존 requester/router 변경은 과거 통과로 대체하지 않는다.

### 9.4 즉시 닫을 실행 부록

Terra는 제품 변경 없이 실제 복제 입력 경로/길이/SHA/GUID 의존, 위 정확 폴더·메서드·새 시험명, 기존 필수 회귀와 실행 선택, Packages/ProjectSettings의 허용 키, 렌더러/폰트/TMP의 실제 참조, 유한 실행 상한과 프로필 불변 경계를 제출한다. 신규 입력 부록을 미래 빌드 결과에 의존시키지 않는다. Luna가 이를 사전 검수하면 Astra가 C1-A만 Approved로 전환할 수 있으며 CH1 전체 미결이 이 작은 구현을 막지 않는다.

실제 화면 입력 도구는 Root가 초기화/창 목록 조회 가능성을 확인했다고 보고했다. 이는 플레이 수단의 준비이며 게임창 조작·빌드·AC 충족은 아니다. 실제 독립 플레이는 승인 후 동일 빌드에서 별도로 수행한다.
## 10. C1-A 첫 묶음의 준비 승인과 실행 후보

### 10.1 원문 원장 작성만 Approved

2026-10-03 실제 아스트라가 **이 항목의 읽기 준비만 Approved**로 한정한다. 제품 계약 전체와 §10.2~10.4는 Review다. 담당은 별도 테라 `gpt-6.1-sol`, 독립 대조는 루나, 통합은 아스트라다. 이는 사용자 위임의 CH1 준비 범위이며 복제·제품 편집·Unity·빌드·장치 입력·키·원격 실행 권한이 아니다.

입력은 `docs/verification/2026-10-03-chapter1-playable-impact-61.md` SHA `AB54935A65F6A3D166E4ABAFF99BC1DA9D624FAA7E0365F8FCAE69B65D79634E`의 §7 `정확 경로` 목록356개다. 이 목록만 추출해 원본을 읽고, 상대경로/바이트길이/SHA256/메타 GUID/직접 로컬 참조/패키지 참조·미해결 사유를 원장에 넣는다. 제품 파일을 복사하지 않는다. 356은 존재 후보 수이지 참조 폐쇄 검증 완료 수가 아니다.

허용 출력은 `output/local-playable-slice-r1/preparation/source-inputs.v1.json`, `test-inputs.v1.json`, `dependency-review.v1.json`의 새 파일 세 개다. 기존 경로가 있으면 덮어쓰지 않고 판정 요청한다. 첫 원장은 §7 목록만, 시험 원장은 §9.3의 실제 시험 클래스와 그 helper/asmdef/메타/fixture 의존을 직접 소스 참조로 추적한 후보만 담는다. 폴더 전체나 저장소 재귀 복제를 허용하지 않는다. 세 원장에는 조사 보고 SHA·이번 계약 원문 SHA·원본 루트·조회 시각·중복/누락/링크 검사 결과와 미해결 참조를 넣는다. SHA는 원문 바이트 기준이며 정규화하지 않는다. 패키지 GUID 로드·Unity 가져오기 성공을 읽기만으로 주장하지 않는다. 작업은 로컬 읽기와 세 JSON 작성으로 끝나고 자식 제품 프로세스를 만들지 않는다.

루나가 실제 파일·집합·길이·SHA와 의존 범위를 대조한 뒤 아스트라가 정확 입력 원장을 채택한다. 미해결 참조는 원인별로 닫거나 유한 가져오기 진단의 관측 대상으로 명시해야 하며, 실패 시 임의 패키지 설치·renderer 삭제·환경 업데이트를 하지 않는다. 이 준비 단계 완료를 제품 AC 충족으로 세지 않는다.

### 10.2 정확 API와 변경 메서드 후보

§9.2 파일 목록에 아래 API·책임을 고정한다. 새 Run.Unity·Run/Input AssemblyInfo friend 변경·Run 코어·ChoiceSkill 구현은 C1-A에서 제외한다. CH1 진행과 저장 소비자는 C1-B/C에서 추가 승인한다.

- `OrdanBossTerminalTransitionRequester.ConfigureLocalPlayableForAuthoring(bool enabled)`는 초기화 전 한 번만 설정한다. 기존 `ConfigureForAuthoring`, `InitializeAtRuntime`, 생산 `FixedUpdate`→`Advance`, `ObserveRequestedReceipt`의 기본false 의미를 보존한다. true 전용 결과/UIOnly 가지를 이 메서드들에 연결하고 `LatestLocalPlayableView`만 읽기 공개한다. 기존 `LatestCompletionReceipt`를 실패 성공으로 조작하지 않는다.
- `LocalPlayableSliceView.cs`는 불변 `LocalPlayableSliceView`와 `LocalPlayableSliceResult` 및 별도 읽기 전용 `LocalPlayableInputLifetime`을 소유한다. 값은 graph 세대, 실제 완료 tick, player/boss 현재/최대 체력, 전이 상태, 결과, UIOnly 승인 tick을 갖는다. 표시값에는 Unity 객체·쓰기 세션·가짜 RunActive/ChapterCompleted가 없다. lifetime의 폐쇄 상태는 실제 router OnDestroy/CloseActionsOnce 경로만 게시하며 외부 setter가 없다.
- `InputRouter.ConfigureLocalPlayableForAuthoring(bool requireNeutralStart)`는 초기화 전만 허용한다. `Start`, 실제 고정 tick 입력 후보 작성, `ClearModeEpochBatch`, `OnDisable`, `OnDestroy`, `CloseActionsOnce` 중 로컬 장벽/실제 폐쇄 기록에 필요한 부분만 변경한다. 맵 정의·기존 callback 등록 수·Hub 프로필 실행 가드·기존 ForTests API는 변경하지 않는다. `LocalPlayableInputLifetime` 읽기 반환은 파기 후에도 소유자가 확인할 수 있는 별도 값 소유 핸들이며 Unity 파기 객체 접근으로 대신하지 않는다.
- 시작 장벽은 router가 소유한 기존 action 상태를 동일 입력 갱신 경계에서 읽는다. 모든 디지털 gameplay/UI 동작이 해제되고 Move/Navigate 및 gamepad 조준의 처리된 벡터가 중립인 완료 갱신을 확인한 뒤, 그 갱신의 edge를 버리고 다음 새 입력만 허용한다. 마우스 화면 좌표 자체가 원점이어야 한다는 조건은 두지 않는다. 실제 누른 채 재시작·게임패드 축·마우스 클릭의 음성으로 확인한다. 새로운 장치 callback이나 다른 시스템의 직접 polling은 금지한다.
- `LocalPlayableSlicePresenter.ConfigureForAuthoring(...)`는 graph prefab·requester·router·명시 Regular/Bold TMP 자산·화면 참조를 받는다. 고정 tick에서 기존 UI 의미 프레임을 정확 한 번 소비하고 `LateUpdate`는 불변 view만 렌더한다. `RequestRetry()`는 승인된 UI frame에서만 의도를 만들며 `RestartAfterDisposal()`은 이전 root 파기 및 lifetime 폐쇄를 확인한 다음 새 prefab 한 개를 생성한다. requester가 아닌 presenter가 모드 전환을 요청하지 않는다. 파기/폐쇄 미확인은 새 그래프를 만들지 않는 명시 실패다.
- `LocalPlayableSliceAuthoringBuilder.Build()`는 고정 Ordan prefab을 읽어 새 prefab/scene을 생성하고 refs·root 이름·opt-in·카메라·TMP를 결속한다. 기존 Ordan Build()는 고정 Sandbox/Graph 경로를 쓰므로 호출하지 않으며 private BuildGraph를 우회하지 않는다. `LocalPlayableSliceBuild.BuildWindows()`는 그 한 장면만 명시 BuildPlayer 인자로 선택한다. 별도 PlayerSettings·품질·입력·패키지 변경은 허용하지 않는다. `EditorBuildSettings.asset`의 활성 장면 목록만 격리본에서 이번 장면1개로 설정하는 후보이며 원본 설정은 보존한다.

신규 집중 시험 경로는 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/LocalPlayableSliceInputPlayModeTests.cs`, `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/LocalPlayableSliceLifecyclePlayModeTests.cs`, `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/LocalPlayableSliceAuthoringTests.cs`와 각각 메타다. 기존 시험 assembly의 접근 범위를 그대로 사용한다. Hub 시험은 공개 Unity 수명과 저작 완료 자산/화면을 관측하며 Input 내부 타입 직접 호출을 가정하지 않는다. 참조 추가가 필요한 경우 정확 asmdef/참조 한 항목을 입력 부록에서 승인하며 새 friend를 임의 추가하지 않는다. 새 임시 표시 자산은 별도 외부 이미지 없이 builder가 생성하는 단색 geometry와 기존 명시 폰트만 사용한다. 생성 material 등의 추가 저장 파일은 승인 목록에 없으면 메모리 객체로 두거나 정확 경로를 사전에 보완한다.

### 10.3 실행 선택·상한 후보

원장 독립 대조와 제품 Approved 전에는 아래를 실행하지 않는다. 승인 뒤에도 Terra 구현→Luna 소스 사전 대조→Root 실제 소유 실행/원시 보존→Luna 독립 결과→Astra 수용을 구분한다. 실제 GUI 독립 입력은 Luna가 담당하며 Root는 창/기기 조작 지원을 할 수 있지만 수행자를 바꾸어 기록하지 않는다.

1. 원문 입력 복제 및 지문 재대조 후, Unity6000.6.0f1의 격리 가져오기/컴파일 한 번: 15분 상한. 패키지/renderer/font 미해결 실제 결과를 먼저 확인하며 성공 없이 큰 시험으로 넘어가지 않는다.
2. `LocalPlayableSliceAuthoringTests`와 실제 저작 장면 첫 tick 정상 검사 한 번: 5분 상한. 이것이 정상 입구다.
3. 새 Input/Lifecycle 클래스의 집중 경계 묶음 한 번: 10분 상한. 기본false·성공·실패·동시 사망·UI 잠금·held input·완전 파기·재시도2회·누락참조를 포함한다.
4. §9.3 기존 클래스 전부와 변경 영향 이동/전이 시험을 입력 원장에서 정확 클래스/시험 ID로 채택한 회귀 묶음: 한 번 30분 상한. 실제 discovery 결과의 선택 ID/수를 기록하며 미리 임의 개수를 만들지 않는다. 필수 fixture/시험이 누락되면 통과로 종료하지 않는다.
5. 같은 소스의 Windows x64 Mono 빌드 한 번: 15분 상한. 현재 실행 파일 지문과 포함 scene/입력 원장 결속 후 실제 첫 연결 플레이를 최대30분 관측한다. 별도 CH1 완주로 보고하지 않는다.

상한 도달은 해당 실행 실패/미완료를 보존하고 소유한 정확 프로세스만 정리한다. 상한 외 종료·배수 정리는 각5초로 구별하고 재귀 kill·무기한 기다림을 금지한다. 실패 뒤 자동 같은 실행 반복은 하지 않는다. 원인과 최소 보정의 집중 검사를 검토한 뒤 재개하며 사용자 재질문 대신 기존 기술 위임 범위에서 Astra가 판단한다. Q0 실행·서명·원격 게시에 이 상한이나 권한을 적용하지 않는다.

### 10.4 다음 실제 배정

지금 허용되는 다음 작업은 §10.1의 원장 세 개 작성뿐이다. 준비 결과를 이 계약에 SHA로 결속하고 Luna가 경로/참조/시험/API 후보를 검토하면, 첫 구현 범위를 Approved로 전환한다. CH1 전체의 방·선택·저장 상세 설계는 병행하되 이 첫 묶음 승인과 순환시키지 않는다.
### 10.5 실제 가시성·시험 입력의 한정 보정

현재 `Runtime/HubPresentation/Unity/AssemblyInfo.cs`는 Hub.Authoring.Editor와 Hub 자체 시험 assembly에만 내부 접근을 허용한다. Combat.Authoring.Editor asmdef 참조만 추가해서는 새 internal presenter의 authoring API를 호출할 수 없다. 이에 제품 Review 후보의 정확 수정 파일에 `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AssemblyInfo.cs`를 추가하고 `InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")` 한 항목만 허용한다. 기존 Input→Combat 및 Hub→Input 내부 읽기 관계는 그대로다. Run.Unity friend나 새 공개 runtime API를 허용하지 않는다. 이는 §3/9의 일반 friend 금지에 대한 근거 있는 단일 Editor 예외이며 Luna 확인 후 제품 승인에 포함한다.

새 Hub lifecycle 시험이 사용할 현 asmdef가 원장39에 없으므로 읽기 준비 승인 범위에 `output/local-playable-slice-r1/preparation/test-inputs.supplement.v1.json` 새 파일 한 개를 추가한다. 입력은 다음 네 원본이며 실제 SHA/길이/직접 assembly 참조와 source356/test39의 중복 여부를 기록한다. 원본·복제·제품 변경은 여전히 금지다.

- `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/AcadeGameMaker.Hub.Presentation.Unity.PlayMode.Tests.asmdef`
- 위 파일의 `.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AssemblyInfo.cs`

복제 입력의 최종 합집합은 source356/test39/추가원장의 고유 경로로 정하며 중복 경로는 같은 원문 SHA일 때 한 번만 복제한다. 새 테스트는 저작 완료 새 자산을 사용하고, 테스트 중 추가 내부 friend나 reflection 접근이 필요하면 자동 우회하지 않는다. 추가 원장/Luna 대조가 끝나기 전 제품 Approved를 발급하지 않는다.
### 10.6 기존 그래프 저작 경계 보존

실제 기존 Ordan builder의 `BuildGraph`는 private이고 `Build()`는 고정 기존 Sandbox/Graph 자산을 쓴다. 새 builder는 이를 호출해 기존 자산을 덮지 않는다. 승인 입력의 기존 prefab을 로드해 새 자산으로 복제하는 저작만 사용한다. 원래 root의9자식·actor leaf·component 순서·nested player 원본·물리/참조를 보존한다. 임시 renderer와 UI는 graph 바깥의 별도 표현 root에서 transform/불변 상태를 관찰하며 기존 player/target에 component나 child를 붙이지 않는다.

기존 `OrdanBossEncounterAuthoringValidator.Validate(root,false)`의 구조 검증을 재사용한다. `true`는 원래 prefab 경로·scene override를 강제하므로 새 prefab에 그대로 통과했다고 주장하지 않는다. 새 `Assets/AcadeGameMaker/Editor/CombatAuthoring/LocalPlayableSliceAuthoringValidator.cs`와 meta를 정확 신규 파일에 추가한다. 이는 기존 구조 검증 뒤 새 prefab의 정확 경로·opt-in 두 곳·외부 표현 root·카메라/TMP·장면 하나만 검사하며 원래 validator를 느슨하게 변경하지 않는다. runtime 그래프는 원래 이름을 유지하고 로컬 validator의 출력은 C1-A만 증명한다.

Luna 최종 정적 대조에서 requester의 기존 메서드명 AdvanceOneFixedTick이 존재하지 않음을 확인하여 실제 생산 FixedUpdate→Advance로 정정했다. AdvanceForTests를 제품 입구로 사용하지 않는다. 준비 원문398에는 변경이 없다.

## 11. C1-A 한정 제품 승인 — 2026-10-03

실제 아스트라 `gpt-6-astra`는 루나 `gpt-6-luna`의 독립 사전검수 및 현재 원문 재대조를 근거로 이 첫 연결 계약을 **C1-A 범위 Approved**로 전환한다. 검토 대상 계약은40512바이트/SHA `A118940957925E9011EE6CE63938DFFA172B315AD67D58D7654BF6FCB17E6AC3`, 독립 기록은 `docs/verification/2026-10-03-chapter1-playable-luna-precheck.md` SHA `F9288D1D12DBDEBED435C6BC8F559CA5E407B114A807EBAF4100056EA6578218`이다. 이는 본 승인 문단을 넣기 전 지문이며 자기 지문을 주장하지 않는다. 메서드 오기·Editor 접근·기존 저작기 쓰기 경계는 수정 후 P0/P1=0이다.

이 절은 앞선 §3/9/10의 후보·Review·준비만 허용 문구 중 **아래 확정 C1-A 범위에 한해** 후속 승인으로 우선한다. CH1 전체 계약과 C1-B/C 후속 설계는 계속 Review다. Q0 동결·인간 승인·원격 게시·키·운영큐 권한은 변하지 않는다.

### 채택 입력

`output/local-playable-slice-r1/preparation/`의 다음 네 원문을 채택한다. 앞선 DAD8 및30C2 계약 지문은 각각 작성 당시 준비 승인 원문이며 후속 상태 전환을 이유로 원장을 다시 쓰지 않는다.

| 원장 | 바이트 | SHA256 |
|---|---:|---|
| source-inputs.v1.json |405602|6A31561637D4A2E77D6371B716591C81D47924FCEF6BD340101FE055E279F7FF|
| test-inputs.v1.json |73839|3FEC47F46B7973959C041351A6B1CBA8ECFAFBE134E753280703211A644FD100|
| dependency-review.v1.json |262478|C991BF8E23007D92D6543EF1F9465EB972D094C2626BD4DCCE7E6B99475510CF|
| test-inputs.supplement.v1.json |4330|5D750121A4152DE7C412E3B9FF385BD9033C6DC6C84E9BA900B527E25191ADEE|

source356/test39/supplement4 중 동일 원문 중복1을 합친 **398 고유 파일**을 정확 격리 project 루트에 복제할 수 있다. 아스트라는 전수 실제 길이/SHA 및 중복 일치를 재대조했고 루나는 독립 대조했다. 복제 전과 직후 지문·메타를 확인하고 링크/누락/변경이 있으면 중단한다. 원본/Q0는 수정하지 않는다. 기존 Library/Temp/출력/사용자 저장/키·저장소 Git 복제는 제외한다.

### 지금 허용되는 구현

별도 테라 `gpt-6.1-sol`에게 §9.2와 §10.2/10.5/10.6의 **정확 파일·메서드·신규 시험·메타**만 격리본에서 구현하도록 승인한다. 파일 목록이 겹치면 후속 §10의 좁은 보정이 우선한다. Hub의 정확 Editor friend1개 및 Editor의 정확 두 assembly 참조, 격리 EditorBuildSettings의 시작 장면1개만 허용한다. 새 파일의 필요한 부모 폴더 메타는 새 폴더에 한해 만들고 생성 원장에 기록한다. 광범위한 Assets/Presentation 와일드카드가 추가 제품 파일 권한을 주지 않는다.

허용 증거 쓰기는 `output/local-playable-slice-r1/evidence/`, 빌드는 같은 루트의 `build/`다. 복제/허용 변경/메타/실행 원장과 로그는 이 경계 안에 남긴다. 새 원장 작성 도구가 필요하면 그 자체의 범위·출처를 기록하며 제품 바이트를 우회 변경하지 않는다. 실제 gameplay 수치·기존 prefab/scene·기존 Ordan builder/validator 본문·Run 코어·새Run.Unity/friend·ChoiceSkill·프로필/네트워크·Q0·외부 API는 이 승인에 없다.

### 실행 단계와 미입증 경계

구현 후 실제 변경 소스·정적 참조·시험 선택을 루나가 검토하면 §10.3의15분 가져오기/컴파일 한 번으로 진행할 수 있다. 미해석GUID212는 외부 패키지로 모두 판정된 것이 아니다. 실제 로드에서 package/local/missing을 구별하고 필요한 graph/font/renderer 참조가 누락되면 첫 tick·회귀·빌드 진행을 막는다. 패키지 요청/잠금은 보존하며 자동 업데이트·누락 renderer 삭제로 통과시키지 않는다.

가져오기 성공 후 정상 저작 입구5분→집중10분→정확 선택 회귀30분→빌드15분→독립 실제 플레이최대30분의 순차 조건을 유지한다. 기존12클래스 및 신규3클래스의 discovery/fixture 의존과 영향 이동·전이 필수 시험은 큰 회귀 전에 확정하며 부족한 시험을 생략해 성공 처리하지 않는다. 새 원본 입력이 실제로 필요하면 정확 부가 경로/지문만 검토·채택한다. 이 잔여 검사 선택은 지금의 제한된 제품 구현을 막는 새 전체 설계 선행이 아니다. 각 실패는 원시를 보존하고 자동 동일 재실행하지 않는다.

Windows Mono 모듈 두 종류의 존재는 Root의 읽기 관측이며 현재 빌드 성공이 아니다. 이번 승인은 구현·단계별 검사 권한이며 AC-LPS 완료, 사용자 플레이 성공, 데모/CH1 전체 완주 또는 원격 게시 수용이 아니다. 실제 후속 검수와 결과로만 수용한다.

## 12. C1-A 한글 메모리 글꼴 한정 보정 — Approved

실제 아스트라는 승인 전임 SHA `9150DDA73EF79E3CE1C597D8A4946D77899DFA266B288DD6CC0302E41CD67401`의 입력398·제품 범위·검사 순서를 보존하며 다음 보정만 승인한다. 기존 정적 SDF의 문자138개는 새 한국어 표시를 모두 포함하지 않고, 실제 `m_SourceFontFile`은 null이다. GUID 문자열만으로 런타임 원본 Font가 빌드에 포함됐다고 주장하지 않는다.

REQ-LPS-FONT-001: 이미 채택한 `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf`와 `NotoSansCJKkr-Bold.otf`를 각각 명시적 `UnityEngine.Font` 직렬화 참조로 새 presenter에 전달한다. 기존 Regular/Bold SDF와 그 material/shader 참조도 보존한다. builder의 기존 ConfigureForAuthoring 호출과 presenter의 해당 API에 Font 두 인수만 추가한다. validator는 정확 자산 경로·Font 비null·기존 OTF의 includeFontData=1·기존 shader `TextMeshPro/Mobile/Distance Field`의 정확 참조를 검사한다. 원본 OTF/meta/SDF/shader나 패키지를 수정하지 않는다.

REQ-LPS-FONT-002: presenter가 초기화 때 각 Font로 `TMP_FontAsset.CreateFontAsset`의 공개 Font 오버로드를 호출해 Dynamic 글꼴 두 개만 메모리에 소유한다. sampling90/padding9/SDFAA/1024×1024/다중 atlas false로 고정하고, 생성 실패나 표시 문자 누락이면 시작 실패로 닫는다. 실제 표시 문자열 전체와 숫자·구두점·표시 가능한 전이 대상 이름의 합집합을 `TryAddCharacters`로 사전 확인한다. 고정 크기에 부족하면 자동 확대/OS 폰트 대체하지 않고 실제 누락 결과를 보존한다. 새 저장 폰트 자산·외부 다운로드·Resources 전체 로드·전역 TMP 기본값 변경은 없다.

현재 패키지의 생성 API는 새 material과 atlas를 만들고, `TMP_FontAsset.OnDestroy`가 자기 atlas와 material을 파기한다. 생성 material의 shader가 직렬화한 승인 shader와 같은지 확인하고 이 생성 material을 그대로 소유·사용한다. 별도 기존 material 복제나 기존 SDF atlas 참조를 대입할 필요가 없다. 생성된 atlas 크기·gradient scale을 기존 정적 material 값으로 덮지 않는다. 따라서 원본 material/atlas는 공유 읽기이며 파기 대상이 아니다.

REQ-LPS-FONT-003: runtime 글꼴은 graph가 아니라 presenter 수명에 속한다. graph 재시도마다 재생성하지 않는다. presenter 파기 때 TMP 참조를 해제한 뒤 생성 FontAsset 두 개를 각 한 번 Destroy하여 패키지의 자원 정리를 사용한다. 별도 atlas/material 이중 파기를 하지 않는다. 첫 글꼴만 생성된 뒤 두 번째 실패한 부분 초기화도 같은 경로로 정리한다. 원본 Font/SDF/material/shader는 Destroy하지 않는다.

정확 수정 범위는 이미 승인한 `LocalPlayableSlicePresenter.cs`, `LocalPlayableSliceAuthoringBuilder.cs`, `LocalPlayableSliceAuthoringValidator.cs` 및 기존 승인 신규 Authoring/Lifecycle 시험 파일의 글꼴 검사뿐이다. 새 assembly/friend/파일·게임 수치·입력 소유 변경은 없다. AC-LPS-FONT-001은 실제 build 포함 Font2와 모든 표시 문자 누락0, AC-LPS-FONT-002는 재시도2회 뒤 글꼴 인스턴스 동일 및 presenter 파기/부분 실패 후 소유 자원 해제·원본 생존이다. 소스 검토와 기존 단계별 실행에서 검증하며 지금 glyph 렌더·빌드 성공을 주장하지 않는다.

## 13. 기존 셰이더 include 입력 두 개 보완 — Approved

실제 아스트라는 §12 승인 전임 SHA `493EA6167158C723D498A33388D8DF4C42873E0E9D56F436CAF08D2DB1087D3E`에서 다음 원본 두 개의 **동일 바이트 추가 복제만** 승인한다. r3 가져오기 로그 SHA `5B9C804BE3E8BD9FF89DC8C2C83416CEC2FB25995F8453CA5D442A50D5B08560`에서 스크립트 compile exit0과 별개로 필수 font shader의 include 누락이 발견됐다. 기존 `TMP_SDF-Mobile.shader` 95행은 상대 `TMPro_Properties.cginc`를 요구한다. 원본 include를 읽고 재귀 include0 및 다음 원문 SHA를 직접 확인했다. 패키지 원문과 바이트 동일하다고 주장하거나 새 package를 가져오지 않는다.

| 정확 원본/격리본 상대경로 | 바이트 | SHA256 |
|---|---:|---|
| Assets/UI/Shaders/Hub/TMPro_Properties.cginc |2707|66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90|
| Assets/UI/Shaders/Hub/TMPro_Properties.cginc.meta |204|E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8|

REQ-LPS-FONT-004/AC-LPS-FONT-003: 기존398 준비 원장과 제품 구현 원장은 불변 보존하고, `output/local-playable-slice-r1/preparation/shader-include-inputs.supplement.v1.json`에 실제 두 원본/복제 길이·SHA·기존 집합과 중복0을 기록한다. 실제 입력 합집합은400이며 기존398 기록을 소급 수정하지 않는다. Root의 동일바이트 복제와 보완원장 작성을 허용하고 Luna가 원본/격리본과 shader 상대 include의 해소를 독립 대조한다. 원본·shader 본문·패키지·API·셰이더 수치는 수정하지 않는다.

독립 대조 뒤 기존 고정 Unity 환경으로 해당 격리 project 재가져오기/compile 한 번15분을 허용한다. 같은 원인 자동 재시도는 없고 실제 shader 진단을 확인해 필수 오류0이어야 다음 저작/시험/빌드로 진행한다. 스크립트 exit0만으로 shader 성공을 판정하지 않는다. 종료·배수 각5초는 기존 제한을 유지한다. 이 보완은 동일 글꼴 표시 위험의 누락 입력 폐쇄이며 새 제품 기능·Q0·원격·사용자 저장 권한이 아니다.

## 14. 필수 이동·전이 영향 회귀 입력 보완 — Approved

실제 아스트라는 전임 계약 SHA `29B341F98A3A137D6658146AB2615581E35F7AE29533AEBD7709FEE4BB3381DC`에 다음14개 기존 원본의 동일바이트 복제만 추가 승인한다. 원본 실제 길이/SHA와 지정9메서드 존재를 직접 대조했다. 아래 상대경로 기준은 `Assets/AcadeGameMaker/Tests/PlayMode/`다. 새 friend·제품 API·시험 본문 변경이나 동적 fixture 추가는 없다.

| 상대경로 | 바이트 | SHA256 |
|---|---:|---|
|Movement/PlayerMovementControllerPlayModeTests.cs|33629|5762DF8E7FA980328FB600B1F25404F53DA238F0C7619A74DCB730D95951C267|
|Movement/PlayerMovementControllerPlayModeTests.cs.meta|60|8CEA19BC1FDA3E23C4AA6AB9B5E82BD89F0B94E864A849965A02508E51B5A5F3|
|Movement/PlayerMovementInputGatePlayModeTests.cs|16569|792C71B4A025E4F761D94193C858724C7465494BF6407E12F3F670B9B5A94097|
|Movement/PlayerMovementInputGatePlayModeTests.cs.meta|60|5452292935A794135D707CB91F621D25F687134769238AC526A9554EA9A8B977|
|TransferUnity/TransferSimulationDriverPlayModeTests.cs|21771|5F934941897FC67C58FC07046CBD5B069C20167A8808EAA17748904B1A384E39|
|TransferUnity/TransferSimulationDriverPlayModeTests.cs.meta|60|76C904922C87CE27A06A7560A3CCCBA427D6100182C97D95AE6E349C82A5099A|
|TransferUnity/TransferInputGatePlayModeTests.cs|20380|555CDA491CFA1AD7A5FF8559888429F2748E9EF53A5835785DBFD7FBE3CA85B1|
|TransferUnity/TransferInputGatePlayModeTests.cs.meta|60|DFEC7D6ECE92561801C517A2692E52A148F3BD279BB9DEC24A924523435F1C3B|
|Movement/AcadeGameMaker.Movement.PlayMode.Tests.asmdef|541|0890FA3F34DF84A315EBF8C737CF5DC987EAECBB8010272CE207235397FDC80C|
|Movement/AcadeGameMaker.Movement.PlayMode.Tests.asmdef.meta|60|5EAF98BA8661B1590022DFFAA740609DF251FBAECEEFEE96007B4AB6740D777A|
|TransferUnity/AcadeGameMaker.Transfer.Unity.PlayMode.Tests.asmdef|612|D76A37D0B2F94FE0FCD431B7E9522D901C99F0E61E1487DBECF5D39E884C2ED5|
|TransferUnity/AcadeGameMaker.Transfer.Unity.PlayMode.Tests.asmdef.meta|166|1FF0782B6F55061239DF6D8B1968DA0594DCB6BD9BF8E864A5572088959EBC31|
|Movement.meta|172|F352792A9B76D06E5A561DB104B2C98F0496274BBF7C5E1282D5F8FE662055ED|
|TransferUnity.meta|172|F8E55A722602929CD85CC6CF1C92AE2C594594EB29D97180C1BE25E00AA5FF24|

REQ-LPS-REG-001/AC-LPS-REG-001: Root는 격리 project의 동일경로에14개 원문을 복제하고 `preparation/movement-transfer-test-inputs.supplement.v1.json`에 전후 원문 지문·중복을 기록한다. 기존398/400 입력 및 구현 원장은 불변이다. 중복0 확인 시 실제 고유 입력은414다. 누락·변경·다른 기존파일 덮어쓰기는 승인하지 않는다. Luna 독립 원문 대조 후 기존 큰 회귀 묶음에 아래9메서드를 포함한다.

- PlayerMovementInputGatePlayModeTests: `LockedTicksRemainMonotonicAndKeepPlayerActiveAndContinuous`, `UnlockAcceptsFreshCommandOnceAndNeverReplaysOldFutureCommand`, `LockPulseClearsBufferedJumpAndActiveDash`.
- TransferSimulationDriverPlayModeTests: `DriverCapturesOnceAndReflectsLightweightExactlyOnce`, `LifecycleClearsActiveOwnerAndConsumesSameTickPress`, `ThirtySixtyAndOneFortyFourRenderGroupsReplayIdenticalDriverOutcomes`.
- TransferInputGatePlayModeTests: `LockStripsExactAndFutureInteractiveFieldsButPreservesRemovalAndKeys`, `UnlockRetainsStrippedFutureWorkAndAcceptsOnlyFreshInteractiveInput`, `LockedMixedSubmissionCopiesMaintenanceBeforeCanonicalization`.

PlayerMovementControllerPlayModeTests의 입력도 포함하며 정상 physics 영향 선택은 실제 발견 목록과 함께 후속 고정한다. 이번9개를 전체 이동 회귀 통과라고 주장하지 않는다. 기존12클래스+신규3클래스 및 다른 필수 영향 검사를 대체하거나 생략하지 않는다. 큰 검사 단회30분 및 기존 집중·정상 저작 선행 조건은 그대로다. 원문 복제는 즉시 가능하지만 검사 성공·선택 발견·현재 제품 수용은 실제 결과로만 기록한다. Q0·원격·키·사용자 프로필 권한은 변하지 않는다.
