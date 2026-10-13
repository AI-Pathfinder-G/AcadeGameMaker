# 1챕터 체험 플레이 영향 조사 — 2026-10-03

- 실제 작업자: 테라 역할 `gpt-6.1-sol`. 최신 사용자 목표는 보스 단독 실험이 아니라 1챕터 체험 플레이 가능한 전달물이다.
- 범위: 읽기 전용 의존/빈자리 조사. 제품 구현·격리 프로젝트 복제·Unity·빌드·Q0·원격 실행0. 전략·계약·원본 소스·기존 증거·감사 파일을 수정하지 않았다.
- 산출물·동작: 1챕터 시작→진행→오르단→선택/기술→봉쇄선→서사/결과, 실패→거점 복귀→새 원정을 잇기 위한 실제 소유·API·최소 제작 순서.
- 담당: 계약·캐논·최종 통합 아스트라, 승인 후 구현 테라, 사전/사후 독립 검수 루나. 현재 제품 구현 승인은 별도 계약의 Approved 및 정확 파일/실행 부록 확정 뒤다.
- 조사 입력: 문서 지도·CONTEXT·운영 모델, 게임/서사 캐논, Approved VD-00~10 및 SYSTEM-CONTRACTS, 본편 NAR-00/관계 후속의 현재 상태, CH1 시놉시스 승인, 현 Runtime/Editor/장면/프리팹. 검사와 종료는 직접 근거와 영향별 REQ/AC를 새 문서에 연결해 계약 작성자에게 전달하는 것이다.

## 1. 1챕터·데모·한정 체험의 정확한 구분

[CH1 방향 승인](../approvals/2026-08-24-character-first-eight-chapter-synopsis-approval.md)과 [시놉시스 1장](../proposals/2026-08-24-character-first-story-reboot-sol-draft.md)의 제목은 **거부하는 사람**이다. 도언은 분산했던 설계 열쇠 하나가 강제로 움직인 감각을 따라 탈락 구역에 들어간다. 유담의 공동장부를 압류하는 오르단과 싸운 뒤 첫 선택을 한다. 수탈에서는 유담의 거부를 무시해 압착 판결을 얻고 공동장부 조작 능력 상실과 도언 각인의 번짐을 남긴다. 연대에서는 범위 있는 동의·소유권을 지켜 공동 기준면을 얻으며 오르단의 증원 시간을 허용한다. 두 경로 모두 구역을 빠져나온 뒤 세령이 활을 겨눈 독립 추적자로 처음 나타난다. 아인이 사라진 날 도언이 마지막 동행자였다는 이유와 아인의 구조용 걸쇠에 설계 열쇠가 응답하는 훅이 다음 장으로 이어진다.

이 승인은 인물·관계·8챕터 **방향**의 승인이다. exact 대사·보스 수치·Unity 구성은 승인 비범위이며 [NAR-00](../specs/full-game-narrative/00-spec-index.md)와 [관계 후속](../specs/full-game-narrative/01-relationship-ending-and-retry.md)은 현재 Review다. CH1의 새 한정 제품 계약은 Approved VD-06을 보존하며 필요한 구간·대사·전이·기록·종료 의미를 구체화해야 한다. 전체 NAR-00의 미해결 후반 규칙을 상수나 가상 상태로 채우지 않는다.

[VD-00](../specs/vertical-demo/00-spec-index.md)의 수직 데모는 전체 챕터를 명시 비범위로 두면서 거점1·방풀6/런당4·일반 적2·오르단1·선택1·두 기술·실패/부분 초기화·세령 첫 등장과 12~18분을 고정한다. CH1 캐논과 겹치는 사건이 많아도 **VD-00 완료=본편 CH1 전체 완료**는 아니다. 한정 CH1 체험의 등록 구간과 제외 콘텐츠를 아스트라가 정하고, 데모 AC-SCOPE-001..004 및 본편 Review 요구를 축소하거나 완료로 소급하지 않아야 한다. 이전 [한 전투 조사](./2026-10-03-first-local-playable-impact-61.md)는 입력·HUD·재시도 의존 근거로만 재사용한다.

## 2. 현재 존재하는 부품과 실제 미연결

| 사용자 구간 | 존재하는 직접 근거 | 미연결/미발견 구현 | 추적 요구·검사 |
|---|---|---|---|
| 시작·거점 출정 | `Assets/Scenes/Hub.unity`, Hub.Presentation.Unity의 화면/의도 소유자, Input.Unity의 프로필 launch 및 Profile 원자 저장/복구 부품 | 현 활성 빌드 장면은 Bootstrap 단독. Hub intent가 실제 원정 장면/RunStarted로 이어지는 Runtime `SceneManager/LoadScene/RunStarted` 소비 경로는 검색 미발견. CH1 도입·출정/이어가기의 실제 목적지 없음 | REQ-SCOPE-001, REQ-RUN-001/006, REQ-PLAT 해당 저장/빌드; AC-SCOPE-001, AC-PLAT-004/005 |
| 탈락 구역 진행 | 이동·전이 코어/Unity와 MovementSandbox. 일반 적 Walker/Surveyor의 행동·이동·위협·전투 연결 및 CombatEncounterSandbox | Runtime에 방/원정 조립 소유 폴더·소스 없음. Assets의 R01/R06/RoomPlanSnapshot 자산·공유 입출구·CH1 지형/사건 배치 검색 미발견. 일반 적 graph에 생산 InputRouter/카메라 결속 없음 | REQ-MOV-001..005, REQ-WT-001..007, REQ-COM-001/002, REQ-ROOM-001..007; AC-MOV-003, AC-ROOM-001..006, AC-COM-001 |
| 오르단 전투 | OrdanBossEncounterSandbox, OrdanBossEncounterGraph, CombatEncounterPlayer. 기존 저작기가 InputRouter·카메라·전이·기본공격·보스 피해/패턴·handoff·terminal을 실제 직렬화 결속 | 실제 CH1 진행에서 보스로 들어오는 수명주기/장면 전이 없음. HUD·피격/조준/전이 판독 및 최종 actor 렌더 연결 부족 | REQ-COM-003/004/006, REQ-UX-001..006, REQ-WT-005; AC-COM-002/003/004, AC-UX-001/003/005 |
| 보스 뒤 진행 판정 | `Runtime/Run/PostBossProgressionSession.cs`의 ObserveDeath/CompleteCleanup, Verified M5A. exact 죽음·실패·pair·cleanup에서 PrepareChoice/PrepareBarrier 의도를 만든다 | Runtime/Editor에서 이 코어를 호출하는 생산 소비자 검색 미발견. 의도는 장면 준비/선택 확정/기술부여/지급/완료가 아님 | REQ-COM-004, REQ-RUN-001/004, REQ-CHOICE-004/005; AC-M5A-001..009와 부모 AC-CHOICE-003 부분 근거 |
| 유담 선택·기록 | ProfileProgressionSnapshot의 선택/동의/기술 pair 및 canonical encoder/decoder·원자 저장 부품 | Choice/Narrative 런타임 소유자·선택 화면·확인/취소·ChoiceCommitted·기술 grant·선택 직후 save 생산 경로 없음. 프로필 필드 존재만으로 선택 구현이라 세지 않는다 | REQ-CHOICE-001..005, REQ-RUN-003/006; AC-CHOICE-001/002/003, AC-RUN-002/004 |
| 두 기술과 봉쇄선 | GameInput.inputactions/GameInputActions에 ChoiceSkill 액션, DamageContracts에 ChoiceSkill 분류. 전이/Combat 피해 코어는 재사용 후보 | `InputRouter.cs:1492`는 ChoiceSkill을 `_unboundGameplay=true`로만 기록. 선택 기술 처리·쿨다운·압착 지연피해/경직·발판/ResonanceHold 없음. Assets의 BarrierCounterweight/ConsentAnchor/BarrierGate 검색 미발견 | REQ-CHOICE-006/007, REQ-UX-004, REQ-WT-001/005; AC-CHOICE-004, AC-UX-004 |
| 세령 첫 등장·CH1 결과 | 캐논·VD-06의 사건 순서, Profile completedBranches 필드 | DemoCompleted/RunEndCommitted 성공·전이정리·완료 저장·서사 장면·세령 등장·Ended 소비자 없음. CH1 걸쇠/다음장 훅 표시 없음 | REQ-SCOPE-001/002, REQ-CHOICE-005, REQ-RUN-001/006; AC-SCOPE-001/002, AC-CHOICE-003, AC-RUN-004 |
| 실패·복귀·재시도 | `Runtime/Run/RunFailureArbitrationSession.cs` Verified M5D2. 실제 보스/player 체력 snapshot. 전이 lifecycle clear 코어 | 코어는 NotStarted→Active→Failed 및 요청까지. Unity source adapter·KillPlane/LethalCrush producer·RunEndCommitted·Returned·거점 복귀/새 RunStarted 없음 | REQ-RUN-001..006, REQ-WT-005; AC-RUN-001..004, AC-WT-004 |

미발견 판정은 현 `Assets/AcadeGameMaker/Runtime`, `Editor`, Assets 장면/프리팹/asset/json/cs 검색 범위의 결과다. 이름만 다른 외부 파일이나 미래 구현까지 부재로 단정하지 않는다. 현재 Scenes는 Bootstrap·Hub·MovementSandbox·CombatEncounterSandbox·OrdanBossEncounterSandbox이며 연속 CH1 장면은 확인하지 못했다. 이 조사에서 플레이를 실행하지 않았다.

## 3. 실제 구현 전에 닫을 핵심 경계

### 입력 소유와 그래프 전환

기존 InputRouter 생산 결속은 OrdanBossCombatSimulationDriver에 묶여 있다. 일반 적 CombatSimulationDriver는 그대로 주입할 수 없다. **CH1이 일반 적 진행을 포함하면** 작은 명시 gameplay 입력 sink 공통 경계 또는 일반 적 전용 한정 입력 결속을 Approved 부록에서 정해야 한다. 각 graph에는 한 InputRouter·한 gameplay Combat owner만 두며 보스/regular owner 공존을 허용하지 않는다. 기존 규범 고정 phase와 원자 프레임 receipt를 보존한다.

기존 오르단 requester가 modeRequester를 독점하므로 새 챕터 소유자가 두 번째로 등록할 수 없다. [이전 조사](./2026-10-03-first-local-playable-impact-61.md)의 로컬 opt-in/불변 표시/후속 UIOnly 후보는 재사용 가능하지만, CH1에서는 **보스 격파를 최종 성공 결과로 표시하면 안 된다**. 같은 requester 소유를 보존해 진행 소유자의 값 기반 의도를 실행하거나 하나의 승인된 mode 소유 경계로 결속해야 한다.

보스 cleanup 후 `InputRouter.cs:720~727`은 `_combatRetired` 또는 `HasCompletedBossDeath`에서 GameplayEnabled를 거절한다. `UIOnly→GameplayEnabled` 요청만으로 봉쇄선 게임플레이를 다시 열 수 없다. 또한 보스 Transfer registry는 audit/세 payload의 정확 four-ID 구조여서 BarrierCounterweight를 슬쩍 추가할 수 없다. **후속 봉쇄선 graph**는 새 전이 대상/선택 기술/입력 결속과 장면 수명주기를 가져야 한다. empty-combat 사용을 기존 보스 드라이버의 가짜 alive 상태로 만드는 방식은 제외한다.

graph 간 넘기는 것은 확정 프로필/진행의 불변 값과 승인된 입구 초기 상태다. 이전 router disable만으로 Actions disposal이 끝나지 않으며 OnDestroy가 필요하다. 이전 graph 완전 파기→입력 소유 폐쇄 관측→새 graph 활성화를 지킨다. 보스 이후 player 체력 보존·입구 위치·velocity 초기화·전이 clear 등 exact 넘김은 계약에 적어야 하며 새로운 graph 기본 체력으로 덮어써서는 안 된다. 실패 재시도에서는 원정 transient 상태 제거·보존 pair 유지·held-input 중립 장벽을 별도로 검증한다.

### 진행·실패·저장 소유

M5A에 `RunActive=true`,`AcceptedFailure=null`,`pair=None/None`을 상수로 넣지 않는다. M5D2 Start/ObserveTick의 실제 Run 신원·완료 틱·닫힌 실패 batch와 검증된 Profile pair를 생산 adapter가 공급해야 한다. player 동시 사망 또는 이미 수락된 실패에서는 M5A가 성공 의도를 Suppressed로 만드는 기존 규칙을 보존한다. 성공의 다음 틱 잠금 뒤 새 실패 요청을 차단하는 규범도 실제 소유자가 실행해야 한다.

`Run/AssemblyInfo.cs`는 현재 EditMode.Run 시험에만 내부 접근을 허용한다. 새 Unity 진행 adapter가 M5A/M5D2를 읽으려면 정확 새 어셈블리 이름/friend 추가를 승인해야 한다. Combat.Unity의 handoff/cleanup은 Input.Unity·Combat.Authoring.Editor 등 기존 친구에게만 열려 있다. 예를 들어 Input.Unity에서 불변 증거를 복사하고 새 Run.Unity가 Input.Unity/Run/Profile/Core를 소비하는 방향은 후보이나 현재 asmdef/friend가 이미 준비된 것으로 보지 않는다. Input.Unity→Run.Unity 역참조로 순환을 만들지 않는다.

이전 단일 전투 체험의 프로필무접촉 가정은 CH1에 그대로 적용할 수 없다. VD-06은 선택/기술의 원자 저장과 기존 확정 pair의 선택 생략을 요구하며 VD-05는 실패 뒤 보존을 요구한다. 저장 실패에서는 기술/분기 완료 성공을 먼저 게시하지 않는다. 기존 일반 사용자 프로필을 피하려는 격리 경로가 필요하다면 VD-09의 persistentDataPath 계약과 현재 플랫폼 구성에 맞는 **정확한 승인 경계**를 정한다. Q0 키/서명/570입력과는 별개이며 임의 NewGame reset을 시도하지 않는다.

### 선택 기술·표현

ChoiceSkill은 전이보다 먼저 처리해 성공 시 같은 틱 TransferPressed를 소비하고 실패 시 전이를 계속 처리해야 한다. 기존 router의 불변 atomic frame 후보 경계에 Choice 의미를 추가하는 범위는 승인해야 한다. callback에서 직접 피해/발판/선택 상태를 변경하는 우회는 제외한다.

압착 판결은 36틱 지연·중심4/주변2 피해·60틱 경직·전이회수·300틱 재사용, 공동 기준면은 150틱 PlayerOnlyPlatform·길이1.5~6.0/두께0.25·420틱 재사용과 대상의 60틱 ResonanceHold를 실제 게임에서 수행해야 한다. `BarrierCounterweight`만 전이 대상이고 유담/ConsentAnchor는 대상이 아니다. 봉쇄선 낙하는 실패 대신 입구 복귀다. 단순 선택 화면에서 문 열기 또는 기술명 자막만으로 AC-CHOICE-004를 충족하지 않는다.

최소 표현은 승인된 임시 기하/한글 역할·상태 표시와 기존 검증 폰트로 시작할 수 있다. 도언·유담·오르단·세령·일반 적·전이 대상·위험·봉쇄선의 역할/경계가 읽혀야 한다. 유담을 죽이거나 세령을 전투 동료/선택 보상으로 배치하지 않는다. 최종 아트/의상·초상·애니메이션의 수용으로 세지 않는다. 실제 대사/표현은 새 CH1 계약에서 아스트라 캐논 검토와 루나 독립 판독 검수를 받아야 한다.

## 4. 가장 빠른 실제 구현 순서 제안

아래는 **같은 1챕터 계약 안의 구현/집중 확인 순서**다. 각 부품마다 별도 승인 문서를 만드는 권고가 아니며 완료 목표는 끝까지 연속 플레이 가능한 전달물이다.

1. **연속 수명주기 뼈대부터:** 승인 exact CH1 구간·시작/복귀 화면·챕터 진행 소유·단일 입력 소유·graph 교체·run failure source 연결을 한 묶음으로 만든다. 기존 오르단 graph/입력/HUD를 먼저 실제 생산 시작에서 연결해 첫 틱·실패복귀·새 원정 입력까지 작은 정상 경로를 확인한다. 이전 로컬 조사에서 재사용할 부분은 이 기반이다.
2. **진행 구간을 실제 조작으로 채움:** 승인한 CH1 도입/저작 동선·전이 퍼즐/일반 적을 붙이고, 끝 출구에서 오르단으로 들어가며 lifecycle 전이 정리를 검증한다. 데모 전체를 수용하려는 범위이면 방6·네 snapshot/런4·일반 적2를 그대로 채운다. 한정 CH1 체험이 다른 수량이면 별도 구간/AC로 구별하고 기존 데모 수량을 바꾸지 않는다.
3. **보스→선택→봉쇄선을 한 연결로:** M5D2 실제 snapshot→M5A exact death/cleanup→UIOnly 선택→원자 저장→기술부여→새 봉쇄선 gameplay graph를 연결한다. 초기에 두 기술의 작은 실제 표적/발판 정상 경로를 먼저 확인해 입력/전이 순서 결함을 전체 회귀 전에 찾는다. 두 분기는 함께 구현한다.
4. **결과·서사·복귀 경계 완성:** 봉쇄선 실사용 뒤에만 DemoCompleted 관련 규범 성공 commit/완료 저장/세령 추적자/Ended를 실행한다. CH1 다음장 훅과 이번 체험 종료 의미를 표시하고 실패/재시작은 확정 선택·기술을 유지한 새 원정으로 돌아가게 한다. 보스 격파 직후 성공 화면을 넣지 않는다.
5. **한 현재 소스 빌드와 독립 플레이:** 집중 정상/음성→영향 필수 회귀→정확 입력/활성 장면 Windows 빌드→루나의 시작부터 두 선택·봉쇄선·서사 및 실패복귀/재시도 실제 입력 플레이→아스트라 수용으로 닫는다. 코드/장면 조각 완료를 CH1 전달 완료로 보고하지 않는다.

루나는 각 연결의 사전 실패 모드를 함께 검토하며, 변경 없는 부품의 같은 바이트 근거 재사용은 영향 분석 후 판단한다. 전체 데모를 수용할 때 VD-10의 네시드×두분기8경로·보스3쌍·방12회·첫플레이3명/시간 측정은 유지한다. CH1 한정 계약의 집중 경로가 그 전부를 완료한 것으로 주장하지 않는다.

## 5. 검사·빌드 선행과 재사용 한계

| 변경 묶음 | 집중 관측 | 필수 영향 회귀/독립 근거 | 재사용 경계 |
|---|---|---|---|
| 출정·진행·graph 전환 | production 시작/first tick·입출구·전이 clear·어셈블리/GUID·old wrapper closure→new graph·held 입력 | InputRouter/이동/전이/regular/boss authored graph와 camera 관련 현재 시험, 실제 동선 완주 | 이전 코어의 수치 검수는 같은 바이트/환경만. 새 장면/방/입력 일반화는 신규 검증 |
| 실패·보스 진행 | 닫힌 동일 틱 실패 batch·동시 사망·M5A suppression·exact death+1 cleanup/receipt·실패1회/복귀1회 | M5D2·M5A·M5D1·teardown·전이 lifecycle 및 새 생산 adapter 시험 | 순수코어 기존 검수는 adapter의 source identity·장면·복귀를 증명하지 않음 |
| 선택·저장·두 기술 | 확인/취소·pair1회·save 실패·기확정 생략·ChoiceSkill 선행·쿨다운/hold·봉쇄선 두경로 | Profile encoder/decoder/원자 저장 영향 회귀, VD-06 두분기 PlayMode·Luna 실제 기술 사용 | 필드/serializer 검수만 재사용 후보. 선택/스킬/봉쇄선은 현재 신규 구현 |
| 완료·서사·재시작 | 봉쇄선 전 세령0·완료 후 추적자1회·실패억제·저장/ended 순서·앱 재시작 transient0 | AC-CHOICE-003, AC-RUN-001..004·캐논 독립 검토·두경로 연속 실행 | 영상 파일 존재/시놉시스 승인은 runtime 컷신/챕터 완주 증거가 아님 |
| 표현·빌드 | HUD/전이/위험/역할 판독·한글 glyph·safe frame·현재 장면/소스 지문·Windows 실행 | REQ-ART-002..005/008..015, 해당 AC-ART/UX, AC-PLAT-002/004/005와 새 계약 AC | 폰트 권리/동일byte만 후보. 신규 문구/화면·현재 exe는 새 검수 |

기존 소스의 빌드 입력은 Core/Movement/Transfer/Combat/Input/Camera/Profile/HubPresentation/Run의 실제 asmdef·메타·타입 폐쇄와 신규 CH1 runtime/authoring/UI 경계다. 패키지/설정은 Unity6000.6.0f1, InputSystem1.20.0, 현 URP/UGUI/TMP와 [기존 버전 권위](./2026-10-01-package-version-authority-crosscheck.md)를 유지한다. 새 시작/진행/봉쇄선/서사 장면, 프리팹·폴더 메타·GUID·글꼴/shader·renderer·입력 wrapper까지 exact 파일 목록으로 고정한다. 현재 Bootstrap 단독 빌드 설정·초기 exe로 새 CH1 빌드를 주장하지 않는다.

Q0-R4 인간 답변·동결 소스·기존9회·키/원격 게시 관문은 그대로 별도다. 새 격리 입력/출력 경로와 수정 복제본 exact 목록·실행 상한은 아스트라가 작성 중인 제품 계약에 승인해야 한다. 이번 조사에서는 복제/실행을 시작하지 않았다. 같은 오류 두 번이면 큰 검사 반복을 멈추고 최소 재현·원시·원인/변경·새 관측 목적을 정리한다. 단회 계약은 첫 실패부터 따른다.

## 6. 직접 읽은 핵심 바이트

| 원문 | SHA-256 |
|---|---|
| VD-06 | `7AF402A72497A42A11B98324C7E346AC38DA89FF2FE2B87BDA2FAF6F3F02F672` |
| NAR-00 Review | `A34CC30E435DEFF7D2A9938E47EEDE0196A177838D8FBE35E270A7ACBF1E22B3` |
| PostBossProgressionSession.cs | `DCDF3CB209C937329FD7424096B95FED30016ED7ACFDCBFB7D238A784A0F1A2B` |
| RunFailureArbitrationSession.cs | `047C6F5573C367D417FD9BC8EDB163EBB2930DC494333FB07CFF002A3D5F902B` |

조사 완료는 제품 계약 설계의 근거 전달만 뜻한다. 구현 완료율·실제 CH1 플레이 가능·전체 데모/본편 완료를 주장하지 않는다. 일일08시 감사는 별도 감사 작업의 소유이며 이 문서에 진척 퍼센트를 새로 산정하지 않았다.

## 7. 아스트라 검토용 제한 원문 읽기·복제 입력 후보

아래는 현재 존재를 확인한 정확 경로 목록이다. 원본 전체 재귀 복제 지시가 아니라 지정 runtime/authoring 어셈블리 폐쇄와 필요한 자산만 열거한 **승인 후보**다. 원문 입력 본문·메타는 같은 상대 경로로 보존하고 각 복제 시각 SHA를 새 계약 입력 원장에 고정한다. 목록 밖 추가는 발견 근거와 계약 허용이 필요하다. Q0 동결570 목록을 대신 쓰지 않으며 Library/Temp/Obj/Logs/기존 빌드·대형 미디어·Costumes/Presentation 어셈블리는 포함하지 않는다. 일반 적/Hub 자산은 CH1 계약에서 실제 사용하도록 정한 경로만 유지하고 불사용 경로는 승인 부록에서 명시 제외할 수 있다.

현재 어셈블리 참조 폐쇄는 이 목록의 Runtime 9범주와 Editor 3범주 안에서 local assembly를 해결한다. 외부 assembly는 InputSystem·URP Runtime/2D Runtime·TMP이며 manifest/lock와 설치된 Unity의 실제 패키지 소유를 사용한다. renderer/global settings의 패키지 GUID와 기존 미해석 참조는 이 목록만으로 의존 폐쇄가 증명된 것이 아니다. 승인된 격리 가져오기 사전 검증에서 정확 참조 로드를 확인하되 과거 일곱 참조의 미입증을 지우지 않는다. 폰트 SDF shader GUID는 `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader`와 메타를 함께 포함하며 TMP Settings의 default font는 null이므로 UI에 Regular/Bold를 명시 결속한다.

### 정확 수정·신규 파일 제안 — 승인 전

다음은 첫 구현 묶음에 필요한 **경로 제안**이며 아직 파일을 생성하지 않았다. 해당 경로는 §7 목록의 원본이 아니라 승인될 격리 프로젝트 안의 상대 경로다. 후속 방/Choice 코어·봉쇄선 효과의 전체 exact 목록은 CH1 한정 설계와 함께 승인해야 하므로 첫 묶음 목록으로 챕터 전체 구현 권한을 주장하지 않는다.

| 작업 | 정확 상대 경로 제안 | 이유/가시성 |
|---|---|---|
| 기존 복제본 수정 | `Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs` | 신규 Run.Unity에 M5A/M5D2 내부 접근 허용. 기존 순수코어 본문은 변경하지 않음 |
| 기존 복제본 수정 | `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs` | 신규 Run.Unity에 불변 입력/terminal 증거·설정 접근. 기존 UI 친구 범위 유지 |
| 기존 복제본 수정 | `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs` | 명시 CH1 설정에서 단일 requester 소유 유지·진행 UIOnly 연결. 기본 M5D1 의미 유지 |
| 신규 | `Assets/AcadeGameMaker/Runtime/Run/Unity/AcadeGameMaker.Run.Unity.asmdef`, `AssemblyInfo.cs`, `Chapter1PlayableFlowOwner.cs`, `Chapter1PlayableView.cs` | 참조는 Run/Core/Input.Unity/Profile, UI로 넘기는 불변 View와 실제 완료/실패 source adapter. Combat.Unity 직접 접근 없이 Input.Unity 값 투영으로 결속 |
| 신규 | `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1EncounterEvidenceView.cs` | handoff/cleanup/player와 실제 router receipt의 완료 틱/신원 검증·불변 값 투영. M5A 값을 상수로 위조하지 않음 |
| 신규 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/Chapter1PlayablePresenter.cs` | 기존 TMP/UI semantic frame 경로로 안내/HUD/선택/결과 표시. 새 직접 device callback 없음 |
| 기존 복제본 수정 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AcadeGameMaker.Hub.Presentation.Unity.asmdef` | UI→Run.Unity 읽기 참조 추가. Run.Unity→UI 역참조 금지 |
| 신규 | `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1PlayableAuthoringBuilder.cs`, `Chapter1PlayableAuthoringValidator.cs` | 기존 보스 builder 원본을 덮어쓰지 않고 한정 새 장면 저작·정적 결속 검사 |
| 기존 복제본 수정 | `Assets/AcadeGameMaker/Editor/CombatAuthoring/AcadeGameMaker.Combat.Authoring.Editor.asmdef` | Run.Unity/Hub.Presentation.Unity/TMP 명시 참조가 필요한 한정 저작기 의존. Run.Unity AssemblyInfo에 이 Editor 친구를 열거 |
| 신규 장면/프리팹 | `Assets/Scenes/Chapter1Playable.unity`, `Assets/Prefabs/Combat/Chapter1PlayableRoot.prefab` | 기존 실험장 원본 수정 없이 생산 시작/소유를 결속. 지형/후속 graph exact 자산은 챕터 설계 승인 부록에서 추가 |
| 기존 복제본 수정 | `ProjectSettings/EditorBuildSettings.asset` | 신규 시작 장면 활성. 원본 Bootstrap 설정 보존 |
| 신규 집중 검사 | `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/Chapter1EncounterEvidencePlayModeTests.cs`, `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/Chapter1PlayableFlowPlayModeTests.cs` | 실제 Start/FixedUpdate·source identity·UI/파기/재시도 정상과 음성. 필요한 test asmdef Run.Unity 참조·친구를 정확 승인 |

위 신규 Assets 파일에는 각 동일 경로의 `.meta`, 신규 `Runtime/Run/Unity.meta`가 필요하다. 해당 부모 폴더의 현 메타는 아래 목록에 있고, 새 폴더를 추가하면 exact 메타도 계약에 추가한다. InputRouter 일반 적/봉쇄선 sink와 ChoiceSkill 의미프레임 확장은 이 첫 묶음 수정표에 아직 포함하지 않았다. 그 연결을 생략한 채 CH1 완료를 선언하지 않는다.

### 정확 패키지·폰트·렌더 경로와 해석 경계

| 범주 | 정확 경로 | 직접 확인한 의미 |
|---|---|---|
| 패키지 요청/잠금 | `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings/ProjectVersion.txt` | 요청/실제 URP·시험 버전 차이는 기존 권위 보존. 새 버전/원격 route 추가 없음 |
| 렌더 진입 설정 | `ProjectSettings/GraphicsSettings.asset`, `ProjectSettings/QualitySettings.asset`, `ProjectSettings/URPProjectSettings.asset` | Quality의 pipeline GUID가 `Assets/Settings/UniversalRP.asset`를 참조하고 Graphics의 global settings GUID가 아래 자산을 참조함 |
| 렌더 자산 | `Assets/Settings/UniversalRP.asset`, `Renderer2D.asset`, `UniversalRenderPipelineGlobalSettings.asset`, `DefaultVolumeProfile.asset` 및 각각 `.meta` | renderer/global/profile 및 각 패키지 소유 참조를 보존. 패키지 GUID 로드 폐쇄는 별도 검증이며 과거 미해석 근거를 자동 해소하지 않음 |
| 폰트 원본/권리 | `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf`, `NotoSansCJKkr-Bold.otf`, `OFL-1.1.txt` 및 각각 `.meta` | 권리/원본 보존. 임시 표현에서도 새 문자열 glyph 검사 필요 |
| 폰트 실제 렌더 | `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset`, `NotoSansCJKkr-Bold-SDF.asset` 및 각각 `.meta` | 기존 SDF atlas/material 내장과 해당 shader를 명시 복제 |
| TMP 자원/shader | `Assets/UI/Resources/TMP Settings.asset`, `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader` 및 각각 `.meta` | default font null. 한정 UI의 명시 font 참조와 shader GUID를 유지 |

아래 356개는 현재 존재하는 exact 후보 경로를 정렬한 목록이다. 시험 fixture/전체 회귀 입력은 제품 복제 원문과 구별해 기존 필수 검사의 실제 선택 후 별도 정확 목록으로 추가해야 한다. 이 목록은 시험 입력 생략 승인이나 전체 현재 프로젝트 수용이 아니다.

### 정확 경로

- `Assets/AcadeGameMaker.meta`
- `Assets/AcadeGameMaker/Editor.meta`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring.meta`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/AcadeGameMaker.Camera.Authoring.Editor.asmdef`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/AcadeGameMaker.Camera.Authoring.Editor.asmdef.meta`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/SimulationCameraProfileAuthoring.cs`
- `Assets/AcadeGameMaker/Editor/CameraAuthoring/SimulationCameraProfileAuthoring.cs.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/AcadeGameMaker.Combat.Authoring.Editor.asmdef`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/AcadeGameMaker.Combat.Authoring.Editor.asmdef.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringBuilder.cs.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringValidator.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringValidator.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringValidator.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringValidator.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs.meta`
- `Assets/AcadeGameMaker/Runtime.meta`
- `Assets/AcadeGameMaker/Runtime/Camera.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/AcadeGameMaker.Camera.asmdef`
- `Assets/AcadeGameMaker/Runtime/Camera/AcadeGameMaker.Camera.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/CameraFollowContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/CameraFollowContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/SimulationCameraSession.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/SimulationCameraSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/AcadeGameMaker.Camera.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/AcadeGameMaker.Camera.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/SimulationCameraDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/SimulationCameraDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/SimulationCameraDriverDiagnostic.cs`
- `Assets/AcadeGameMaker/Runtime/Camera/Unity/SimulationCameraDriverDiagnostic.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/AcadeGameMaker.Combat.asmdef`
- `Assets/AcadeGameMaker/Runtime/Combat/AcadeGameMaker.Combat.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/BasicAttackSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/BasicAttackSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/CombatSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/CombatSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/DamageContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/DamageContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyBehaviorSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyBehaviorSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyEncounterLifecycleSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyEncounterLifecycleSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyLocomotionSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyLocomotionSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyReactionSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyReactionSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyThreatSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyThreatSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/AcadeGameMaker.Combat.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/AcadeGameMaker.Combat.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatObservationBuilder.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatObservationBuilder.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTarget.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTarget.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTargetRegistry.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTargetRegistry.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossAuditTransferModifierSink.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossAuditTransferModifierSink.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossBalanceAuditPullProducer.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossBalanceAuditPullProducer.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossHostileDamageProducer.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossHostileDamageProducer.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossPayloadTransferModifierSink.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossPayloadTransferModifierSink.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyBehaviorSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyBehaviorSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyLocomotionSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyLocomotionSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyReactionSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyReactionSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatDelivery.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatDelivery.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Core.meta`
- `Assets/AcadeGameMaker/Runtime/Core/AcadeGameMaker.Core.asmdef`
- `Assets/AcadeGameMaker/Runtime/Core/AcadeGameMaker.Core.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Core/AimContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Core/AimContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Core/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Core/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Core/BasicAttackContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Core/BasicAttackContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Core/InputFrameCommitContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Core/InputFrameCommitContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Core/SimulationTick.cs`
- `Assets/AcadeGameMaker/Runtime/Core/SimulationTick.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AcadeGameMaker.Hub.Presentation.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AcadeGameMaker.Hub.Presentation.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuButtonViewV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuButtonViewV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuCopyV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuCopyV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubSafeFrameScalerV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubSafeFrameScalerV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input.meta`
- `Assets/AcadeGameMaker/Runtime/Input/AcadeGameMaker.Input.asmdef`
- `Assets/AcadeGameMaker/Runtime/Input/AcadeGameMaker.Input.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Input/GameInputActions.cs`
- `Assets/AcadeGameMaker/Runtime/Input/GameInputActions.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AcadeGameMaker.Input.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AcadeGameMaker.Input.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubMenuPresentationControllerV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubMenuPresentationControllerV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputCameraSnapshotProvider.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputCameraSnapshotProvider.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileBindingOverrideApplyAdapterV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileBindingOverrideApplyAdapterV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/AcadeGameMaker.Movement.asmdef`
- `Assets/AcadeGameMaker/Runtime/Movement/AcadeGameMaker.Movement.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/FixedQ4096.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/FixedQ4096.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/MovementCollisionResolution.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/MovementCollisionResolution.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/MovementContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/MovementContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/PlayerMovementMotor.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/PlayerMovementMotor.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/AcadeGameMaker.Movement.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/AcadeGameMaker.Movement.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/AcadeGameMaker.Profile.asmdef`
- `Assets/AcadeGameMaker/Runtime/Profile/AcadeGameMaker.Profile.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileBindingOverridesJson.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileBindingOverridesJson.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileInputSnapshot.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileInputSnapshot.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchObservationAdapterV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchObservationAdapterV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchPreservationExecutorV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchPreservationExecutorV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLoadSelectorV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLoadSelectorV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileProgressionSnapshot.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileProgressionSnapshot.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileSettingsTutorialSnapshots.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileSettingsTutorialSnapshots.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Run.meta`
- `Assets/AcadeGameMaker/Runtime/Run/AcadeGameMaker.Run.asmdef`
- `Assets/AcadeGameMaker/Runtime/Run/AcadeGameMaker.Run.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Run/PostBossProgressionSession.cs`
- `Assets/AcadeGameMaker/Runtime/Run/PostBossProgressionSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs`
- `Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/AcadeGameMaker.Transfer.asmdef`
- `Assets/AcadeGameMaker/Runtime/Transfer/AcadeGameMaker.Transfer.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferContracts.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSelector.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSelector.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/AcadeGameMaker.Transfer.Unity.asmdef`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/AcadeGameMaker.Transfer.Unity.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/AssemblyInfo.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/AssemblyInfo.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/RegularEnemyTransferModifierSink.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/RegularEnemyTransferModifierSink.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferFixedMath.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferFixedMath.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferLineOfSight.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferLineOfSight.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferObservationBuilder.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferObservationBuilder.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSandboxOwnerSinks.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSandboxOwnerSinks.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferTarget.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferTarget.cs.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferTargetRegistry.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferTargetRegistry.cs.meta`
- `Assets/GameInput.inputactions`
- `Assets/GameInput.inputactions.meta`
- `Assets/Prefabs.meta`
- `Assets/Prefabs/Combat.meta`
- `Assets/Prefabs/Combat/CombatEncounterGraph.prefab`
- `Assets/Prefabs/Combat/CombatEncounterGraph.prefab.meta`
- `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab`
- `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab.meta`
- `Assets/Prefabs/Enemies.meta`
- `Assets/Prefabs/Enemies/Surveyor.prefab`
- `Assets/Prefabs/Enemies/Surveyor.prefab.meta`
- `Assets/Prefabs/Enemies/Walker.prefab`
- `Assets/Prefabs/Enemies/Walker.prefab.meta`
- `Assets/Prefabs/Hub.meta`
- `Assets/Prefabs/Hub/HubMenuRoot.prefab`
- `Assets/Prefabs/Hub/HubMenuRoot.prefab.meta`
- `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`
- `Assets/Prefabs/Hub/HubRuntimeRoot.prefab.meta`
- `Assets/Prefabs/Player.meta`
- `Assets/Prefabs/Player/CombatEncounterPlayer.prefab`
- `Assets/Prefabs/Player/CombatEncounterPlayer.prefab.meta`
- `Assets/Scenes.meta`
- `Assets/Scenes/CombatEncounterSandbox.unity`
- `Assets/Scenes/CombatEncounterSandbox.unity.meta`
- `Assets/Scenes/Hub.unity`
- `Assets/Scenes/Hub.unity.meta`
- `Assets/Scenes/OrdanBossEncounterSandbox.unity`
- `Assets/Scenes/OrdanBossEncounterSandbox.unity.meta`
- `Assets/Settings.meta`
- `Assets/Settings/DefaultVolumeProfile.asset`
- `Assets/Settings/DefaultVolumeProfile.asset.meta`
- `Assets/Settings/Renderer2D.asset`
- `Assets/Settings/Renderer2D.asset.meta`
- `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`
- `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset.meta`
- `Assets/Settings/UniversalRP.asset`
- `Assets/Settings/UniversalRP.asset.meta`
- `Assets/UI.meta`
- `Assets/UI/Fonts.meta`
- `Assets/UI/Fonts/Hub.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf.meta`
- `Assets/UI/Fonts/Hub/OFL-1.1.txt`
- `Assets/UI/Fonts/Hub/OFL-1.1.txt.meta`
- `Assets/UI/Resources.meta`
- `Assets/UI/Resources/TMP Settings.asset`
- `Assets/UI/Resources/TMP Settings.asset.meta`
- `Assets/UI/Shaders.meta`
- `Assets/UI/Shaders/Hub.meta`
- `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader`
- `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader.meta`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/AudioManager.asset`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/EditorSettings.asset`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/InputManager.asset`
- `ProjectSettings/Physics2DSettings.asset`
- `ProjectSettings/PhysicsCoreProjectSettings2D.asset`
- `ProjectSettings/ProjectSettings.asset`
- `ProjectSettings/ProjectVersion.txt`
- `ProjectSettings/QualitySettings.asset`
- `ProjectSettings/TagManager.asset`
- `ProjectSettings/TimeManager.asset`
- `ProjectSettings/UnityConnectSettings.asset`
- `ProjectSettings/URPProjectSettings.asset`
