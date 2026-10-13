# CH1 원정·선택 생산 연결 사전검토

- 검토자: Luna (`gpt-6-luna`)
- 날짜: 2026-10-03
- 대상: `docs/specs/work-contracts/2026-10-03-chapter1-playable-delivery.md`, SHA-256 `A532A07775F9EB1A588A2EB8E759F550A60BE4E7299C4B67FC0FA4809FCFA167`
- 범위: §7 C1-B/C 원정·선택 연결과 §8 시간·체력·입력 보정 후보. C1-B/C는 Review이며 이 문서는 구현 승인이나 제품 검증이 아니다.
- 대조 기준: Approved VD-02, VD-06, `SYSTEM-CONTRACTS.md`, M5A, M5B2, M5B4 및 현재 Movement/Combat/Input/Profile 소스. Unity·제품 실행·Q0·원격은 수행하지 않았다.

## 판정

- **P0: 없음.** 후속 C1-B/C를 C1-A와 구분하고, Run·Combat·Transfer·Choice·Profile의 소유 경계를 새 어댑터 안에 두려는 방향은 기존 계약과 대체로 맞는다.
- **P1: 두 계약 경계를 구현 전에 더 고정해야 한다.** (1) 일반 전투의 새 입력 sink가 실제 `InputRouter` 원자 프레임 준비·검증·커밋에 참여하는 구체 절차가 없다. (2) 압착의 새 age-21 자격은 VD-06 기존 규칙이 아니며 AC-CHOICE-004의 “전이 성립 뒤 기술 한 번”과 플레이어가 이해할 대기 안내 사이의 관계가 명확하지 않다.

## 대조 결과

### tick seed, 승계 체력, 무적

현재 `PlayerMovementController.EnsureMotor()`는 첫 tick을 0으로 생성하고, `CombatSession.Register()`는 최대 체력·등록 무적 길이만 보유한 채 현재 체력은 최대값, 무적 종료 tick은 미설정으로 초기화한다. 따라서 §8.1의 원정 소유자가 exact 다음 tick과 위치를 motor 생성 전에 한 번 결속하는 제안과 §8.2의 첫 Combat 처리 전 생존 체력·이전 완료 snapshot의 전역 무적 종료 tick을 넘기는 제안은 기존 기본 경로를 바꾸지 않는 필요한 후속 보정이다.

후보는 새 graph마다 Movement/Transfer/Combat/Input/Camera의 첫 기대 tick 일치, 중복·타 graph·역행·공백·정수 경계 거절, 미완료 구간의 시간 정지를 요구한다. 체력 seed도 player 1명에 한정하고 기본 `Register`·적/보스 초기 등록·실행 중 치유/부활을 바꾸지 않으며, 만료된 무적을 새로 부여하지 않는다. 이 방향은 SYSTEM의 60Hz 전역 `SimulationTick`과 현재 motor/combat의 exact successor 검사에 합치한다. 이후 승인 구현에서는 진입 seed의 신뢰 출처와 모든 첫 처리 전에 seed가 확정되는 순서를 소스별로 증명해야 한다.

### 실패 입력과 Run 종료 순서

§7.2의 체력 0·KillPlane·LethalCrush 세 source를 각 완료 tick에서 모두 관측해 닫힌 batch로 만들고, 누락/미관측을 false로 대체하지 않으며 Run·graph·tick이 다른 source를 거절하는 설계는 `RunFailureArbitrationSession`의 `AllKnown` presence 요건과 일치한다. 코어는 DTO 값으로 source provenance를 인증하지 않으므로 실제 Unity owner가 세 source 각각을 정확한 graph/run/tick에 묶어야 한다. §7.2의 `RunEndRequested → Transition 잠금 → TransferCleared → RunEndCommitted → 일시 상태 제거 → 보존 필드 저장 → 거점 복귀`는 SYSTEM-CONTRACTS의 Failed run 순서를 보존하며 teardown을 commit보다 앞세우지 않는다. 이 경계를 C1-B의 producer별 정상·음성·동시 원인 검사로 확인할 수 있다.

### 일반 전투 입력 sink와 원자성

Approved M5B4는 `PlayerMovementController`, `TransferSimulationDriver`, `OrdanBossCombatSimulationDriver` 세 참여자의 `PrepareInputFrame`→전체 `ValidateInputFrameCandidate`→동기 `CommitInputFrame` 순서와 전역 receipt의 마지막 게시를 정의한다. 실제 `InputRouter`도 이 세 후보만 조정한다. 일반 `CombatSimulationDriver`에는 현재 legacy `Submit(CombatSimulationInput)` 경로가 있고 M5B4 형태의 prepare/validate/commit 후보가 없다.

§8.3은 새 `Chapter1GameplayInputSink`의 prepare/validate/commit 및 `InputFrameCommitReceipt`·source/fault 대조를 요구하지만, 이 새 일반 전투 참여자가 InputRouter의 단일 원자 transaction에 어떻게 들어가는지, 전역 receipt가 그 입력을 포함함을 어떻게 증명하는지, 모든 검증이 끝나기 전 기존 또는 새 consumer mutation을 어떻게 막는지 정하지 않는다. 단지 receipt를 읽는 것만으로 sink의 combat 입력이 같은 프레임에서 commit됐다는 보증은 생기지 않는다.

**필수 보정:** C1-B/C 구현 승인표에서 일반 전투의 정확 owner와 InputRouter 조정 순서를 고정하고, 새 일반 전투 후보가 M5B4의 전역 검증·단일 commit·마지막 receipt 원칙에 어떻게 참여하는지 명시해야 한다. interactive-only 입력과 system-owned DamageRequest/maintenance lane의 분리, source identity, 실패 시 무변경 보장, 기존 legacy `Submit` 경로 비변경을 검사 항목으로 둬야 한다. M5B4 Verified 규범을 암묵적으로 넓히거나 Ordan sink를 일반 전투 sink라고 재명명해서는 안 된다.

### Profile 저장과 선택 확정

§7.4/§8.4는 snapshot을 검증하고 저장소의 `CommittedFirst` 또는 `CommittedReplacement` 및 정확 revision을 확인한 뒤에만 grant/진행을 게시하며, `CommitOutcomeUncertain`에서는 성공·봉쇄선 진입을 보류한다. 이는 VD-06의 원자 저장 후 기술 지급과 SYSTEM의 `profile.json` atomic replace·불확실 결과 비성공 규칙에 부합한다. 활성 Run/현재 방/체력/위치/전이 상태를 저장하지 않고 기확정 pair는 재지급하지 않는 경계도 적절하다. 구현 계약에는 격리된 profile root의 정확 소유자와 저장 결과가 가리키는 검증된 snapshot revision을 결속해야 한다.

### 압착의 21틱 자격과 AC-CHOICE-004

VD-02는 전이·회수 뒤 21 고정 tick 동안 전이 상태 변경을 금지하고, 잠금 age 21부터 같은 Transfer 입력의 회수를 허용한다. VD-06은 별도로 `ChoiceSkill`의 `GameplayEnabled` 자격과 기술별 cooldown을 정하지만 압착이 Transfer 잠금 기간에 실패해야 한다는 별도 자격은 정하지 않는다. §8.5는 이를 기존 VD-06 규칙이라 하지 않고 C1-C에서 새로 승인할 보정이라고 정확히 구분했으며 age20/21/22와 cooldown 경계를 테스트에 넣었다. 실패 edge가 자동 재시도되지 않고 실패가 기술 cooldown·회수·피해 예약을 만들지 않는 점도 명확하다.

다만 VD-06의 AC-CHOICE-004는 제목이 “봉쇄선의 즉시 사용”이고, “전이를 성립시킨 뒤 ChoiceSkill을 한 번 사용”해 두 경로가 통과한다고 적는다. §8.5의 age21 이전 `Cooldown` 거절은 해당 AC에 내재한 기존 규칙이 아니다. 21 tick은 60Hz에서 0.35초지만, 현재 후보에는 플레이어가 기술을 아직 사용할 수 없다는 가시적 안내나 언제 다시 눌러야 하는지 판독 규칙이 없다. 그래서 사용자가 전이 직후 눌렀다가 거절된 뒤 다시 누르는 흐름은 “한 번 사용” 검증과 모호하게 충돌할 수 있다.

**필수 보정:** C1-C 승인 범위에서 age21 자격을 새 규칙으로 명시하고, AC-CHOICE-004를 구현/검증할 때는 age20 실패가 아무 기술 효과도 내지 않음과 age21 이상 새 입력 edge의 성공을 구별해야 한다. 플레이어가 잠금 종료를 알 수 있는 표시/피드백 또는 계약상 대기 절차를 지정하고, 기술 사용 입력 한 번의 의미를 “성공 입력”인지 “모든 시도”인지 고정해야 한다. 승인된 VD-06을 직접 바꾸려면 별도 규범 보정 승인이 필요하다. 이 보정 전에는 §8.5를 VD-06의 선행 승인처럼 취급하지 말고 C1-C 완료 판정에서 AC-CHOICE-004를 무조건 통과시켜서는 안 된다. 0.35초 추가 대기는 AC의 10초 경로 차이 제한 안에 들어갈 수 있지만, 그것만으로 안내·입력 의미의 모호성이 해소되지는 않는다.

## 제한

이 사전검토는 문서와 현재 소스 표면을 읽어 호환성 후보를 평가했다. 새 API 구현, assembly 접근성 완성, producer들의 실제 graph 결속, atomic save 소비자, 21틱 UI 판독, 시험 실행·실제 플레이는 검증하지 않았다. 두 P1의 계약 보정 및 Astra 승인 전에는 C1-B/C 구현·통합 수용으로 확대할 수 없다. 이는 이미 허용된 C1-A 첫 연결 구현과 무관하다.

## §8.3.1 / §8.6 보정 재검토 (2026-10-03)

대상 계약을 정확 경로에서 다시 읽었다. 전체 SHA-256 `17628DA56F0E32C5B9BAD25545B6571F4573BADACCED6494339E63A4F0CD57C6`이다. 이번 검토는 앞서 기록한 두 P1에 대한 설계 델타만 평가한다.

- **원자 프레임 P1: 설계 수준에서 닫힘.** §8.3.1은 동일 `InputRouter.CommitNextFrame`의 frozen batch와 receipt 후보를 사용하고, Movement→Transfer→선택 Combat의 전체 Prepare 및 전체 Validate 뒤 첫 Commit을 시작하도록 했다. 일반/봉쇄선은 Ordan 후보 자리를 상호 배타적으로 대체하며, map/UI publication과 전역 receipt 마지막 대입의 기존 순서를 보존한다. Combat phase는 등록 owner/token·router·graph·tick 및 receipt 전체 값을 대조하고, 첫 Commit 전 실패의 입력 보존과 이후 실패의 fault/phase 거절도 명시한다. 기존 M5B4/M5B5 의미를 새 분기에도 검사하도록 했다. 따라서 이전의 “새 sink가 실제 단일 transaction에 어떻게 참여하는지 불명확”했던 설계 P1은 보정됐다. 실제 InputRouter·Combat source 수정, owner 등록과 receipt 대조, 세 실패 지점과 Commit/map 실패 검사는 구현 승인표의 정확 파일/API/시험 ID로 잠그기 전까지 검증되지 않은 잔여다.
- **압착 21틱/AC-CHOICE-004 P1: 설계 수준에서 닫힘.** §8.6은 age20 입력 실패·무효과, held 입력의 age21 자동 성공 금지, 키 해제 후 새 edge 성공을 명시하고 실제 생산 tick 기반 대기·가능 표시를 요구한다. AC-CHOICE-004의 “한 번”을 유효 자격 이후의 성공 사용 한 번으로 해석하고, 실패 edge는 성공 횟수에 합산하지 않으며 독립 플레이에서 새 edge 한 번으로 통과하도록 구분했다. 10초 내 양경로 제한을 유지하고, 이 사용성/시간 기준이 실제 플레이에서 성립하지 않으면 AC를 완화하지 않고 설계로 되돌리도록 했다. 새 age21 자격은 C1-C의 신규 승인 규칙이며 VD-06의 기존 자격이라고 주장하지 않는다. 따라서 설계상의 입력·대기 모호성은 해소됐으나 실제 표시와 입력 판독은 미검증이다.

### 남은 승인·검증 경계

보정 후 이 두 설계 P1은 **닫힘**, P0은 계속 0이다. 이는 현재 `Review` 문서의 설계 후보 판정이지 C1-B/C 제품 승인이나 Verified M5B4/M5B5 수정 승인으로 확대되지 않는다. 구현 시작 전 Astra 승인 패키지에서 exact changed-file/asmdef/meta allowlist, generic Combat·Barrier·Run producer의 API/owner/token/graph 결속, Profile 격리 저장 root와 committed revision, CH1 완료 소유 API 및 저장·재시작 경계, 각 REQ/AC와 시험 ID, 영향 회귀와 유한 실행 상한을 고정해야 한다. 구현 후에는 source-bound transaction/실패 source와 Profile 결과의 정적 대조, 필수 시험, age20/21 표시·재입력 경계, 실제 양경로에서 대기/사용 가능 표시와 10초 차이, 실제 첫 플레이를 별도 증거로 검증해야 한다. 이번 델타 검토에서 코드, Unity, Q0, 원격 실행은 하지 않았다.

## §9 B0 후보의 입구 발급·활성화 경계 사전검토 (2026-10-03)

대상 계약 SHA-256 `DC180CAAAA40F7232C7A4B45FDFD920AD9DB35DFB9B09B69BC06E50324AF0D36`의 §9 B0만 좁게 읽고 현재 구현 표면과 대조했다.

- **판정: P0 없음, P1 있음. B0만으로 실제 graph 입구 연속·교체는 아직 실행 가능하게 닫히지 않았다.** §9.1은 `Chapter1EncounterEntry` 값과 실제 InputRouter 등록 token을 구분하고, graph generation 단조성 및 이전 graph 종료 증거가 없을 때 새 입구를 거절하며, 동일 tick Movement/Transfer/Combat 완료 view를 완료 전 게시하지 않는다는 방향은 안전하다. 그러나 `Chapter1EncounterEntry`의 실생산 발급자, 최초 RunKey와 graph generation 소유자, 다음 graph를 활성화하는 owner, 이전 graph 종료를 증명하는 정확 receipt/API가 지정되지 않았다. 단일 `InputRouter` token은 router 등록 인스턴스를 고정할 뿐 Run/graph 수명주기 권위를 만들지 않는다. `Chapter1EncounterEvidenceView`는 같은 tick의 완료 snapshot/receipt를 모으는 view이며 graph teardown·비활성화 완료 영수증으로 정의되지 않았다. B1에서 `Chapter1PlayableFlowOwner`가 향후 Run 상태를 소유한다고 했지만 B0는 먼저 실제 graph 전환 seed/carry와 종료 증거를 요구한다. 현재 문구만으로는 해당 owner를 만들지 않고 B0의 생산 전환을 구현할 수 없다.

- **실제 seed 순서가 중요한 이유:** 현재 `PlayerMovementController.Awake()`는 `EnsureMotor()`를 곧바로 호출하며 이는 `PlayerMovementMotor(SimulationTick(0))`를 만든다. `InputRouter.Initialize()`도 `_player.NextExpectedTick`를 먼저 읽는데 이 속성 역시 motor가 없으면 `EnsureMotor()`를 호출한다. Combat 초기화도 player의 다음 tick을 읽어 session을 만든다. 따라서 새 firstTick/HP entry를 사용하려면 모든 활성화 경로에서 entry를 먼저 수신·검증하고 seed를 설치한 뒤 해당 컴포넌트를 활성화해야 한다. §9는 pre-initialization API와 `EnsureMotor`의 선택적 seed 소비를 제안하지만, 비활성 graph 생성→entry 검증/seed→router/Transfer/Combat 등록→동일 firstTick 검증→첫 활성화 순서를 소유하는 구성요소나 기존 graph의 종료 증거가 없다. 잘못된 순서면 seed 전 tick0 생성으로 기존 invariant를 깨뜨릴 수 있다.

- **필수 보정:** Astra는 (a) 실제 Run/graph owner 또는 이에 준하는 한정 B0 issuer를 정확히 지정해 RunKey·generation·이전 graph 종료 증거에서 Entry를 발급하고, (b) 신규 graph의 비활성 생성부터 entry seed 설치, router/세 consumer 등록, 마지막 활성화까지의 순서와 단일 owner를 고정해야 한다. 종료 증거는 완료 view와 별도로 실제 이전 graph가 quiesce/deactivate됐음을 입증해야 하며, 그 증거 전 중복/겹침 활성화를 거절해야 한다. 만약 그 owner/receipt를 B0에 포함할 수 없다면 B0의 범위를 초기 단일 graph의 입력/증거에 한정하고 실제 graph 전환·seed/carry를 그 owner가 승인된 뒤 수행하도록 단계 순서를 바꾸어야 한다. 단순히 입력값을 임의 생성하거나 fixture token/합성 종료 bool로 채워서는 안 된다.

- **접근성:** 예상 시험 assembly는 기존 내부 접근 경계로 접근 가능해 보인다. Input Unity PlayMode 시험은 Input/Movement/Transfer/Combat.Unity 내부 friend 연결을 사용하고, `CombatSession` 및 드라이버 EditMode 시험은 현재 Combat·Combat.Unity EditMode friend 경계 안에 있다. §9가 새 asmdef/friend를 요구하지 않는 조건은 소스의 현재 `InternalsVisibleTo` 선언과 모순되지 않는다. 이는 정적 접근성 검토이며 Unity compile/discovery 결과는 아니다.

정리하면 B0의 input sink/receipt, API 경로, firstTick/HP seed 대상과 시험 경계는 정확 후보로 잘게 나뉘었으나 실제 Entry issuer 및 predecessor 종료/activation owner가 빠져 있다. 위 owner 경계가 고정되기 전에는 B0의 실제 graph 전환 수용을 승인해서는 안 된다. 기존 §8.3.1/§8.6 설계 판정, C1-A 고정 검수, 제품/Unity/Q0/원격 상태는 변경하지 않는다.

## §9.2 입구 소유 보정 재검토 (2026-10-03)

대상 계약 `docs/specs/work-contracts/2026-10-03-chapter1-playable-delivery.md`의 정확 경로 원문은 49,677바이트, SHA-256 `358CF33A73D254450EE471F3D101014577C4DD32A8B913D8502893BEBB9BADEB`이다. 아래 판정은 §9.2 델타만 대상으로 한다.

- **P0 없음, 이전 입구 발급·활성화 P1은 설계 수준에서 닫힘.** 새 `Chapter1EncounterLifetimeOwner`가 최초 RunKey·generation·firstTick과 regular 후속 입구의 단일 발급자가 되고, B1은 키를 재발급하지 않고 이 owner를 합성한다. 비활성 staging 아래 인스턴스화 후 motor/session 생성 getter를 피하며 Movement→Transfer→Combat seed와 router/camera 설정을 먼저 결속하고, 마지막에 활성화한다. 실제 교체는 Transition receipt와 동일 tick 완료 증거 뒤에 이전 graph를 파기하고, Unity-null과 router가 가진 `OnDestroy`·callback 제거·실제 `actions.Dispose` witness를 모두 확인해야 다음 세대를 발급한다. 완료 view 하나나 단순 비활성화만으로 종료를 대신하지 않는다. 이로써 앞 절에서 요구한 issuer, seed-before-Awake, predecessor 종료 및 중복 활성화 방지 소유 경계가 구체화됐다.
- 현재 소스의 시작 순서와도 설계상 양립한다. `PlayerMovementController.Awake`는 motor를 만들고, `RegularEnemyThreatSimulationDriver.Awake`는 player의 `NextExpectedTick`을 읽어 위협 등록을 수행하므로, inactive staging 중 seed를 먼저 넣어야 한다. `InputRouter.Awake`/`Start`의 기존 검증·초기화와 producer의 Awake 등록은 활성화 뒤 Unity 수명주기에서 일어난다. §9.2는 이를 실행 성공으로 간주하지 않고 실제 첫 완료 view와 정해진 PlayMode 시험으로 확인하도록 한다. 따라서 사전 설정이 성공했다는 정적 확인을 등록/첫 tick 성공으로 확대하지 않는다.
- 실제 regular→regular만 B0에 포함하고 regular→Ordan 전환 및 RunActive/RunEndCommitted 권위는 후속 경계로 남겼다. §9.2의 HP 승계 PlayMode 표에 regular→Ordan 사례가 보이더라도, 본문은 이를 초기 등록 집중 검사와 구별하고 실제 graph 전환은 승인된 terminal requester와 B1 소유자 결속 뒤로 미룬다. 이 제한을 유지하는 한 모순에 따른 P1은 아니다.
- 이 판정은 설계 누락 보정 수용이다. B0 전체 계약은 여전히 Review이며 구현·컴파일·Unity 수명주기 시험·실제 graph 교체는 확인하지 않았다. 정확 source/API 구현, 순서 시험, cleanup 실패의 fail-closed 동작 및 회귀는 별도 승인/검증 관문이다. 제품 실행·Q0·원격은 수행하지 않았다.

## C1-A 컴파일 실패 한 줄 보정 대조 (2026-10-03)

기존 단회 실패 로그 `output/local-playable-slice-r1/evidence/unity-import-compile-r1.log`의 컴파일 진단은 `LocalPlayableSlicePresenter.cs(120,43)`에서 `InputMode` 이름을 찾지 못한 `CS0103`이다. `InputMode`는 `AcadeGameMaker.Core` 네임스페이스에 선언되어 있으며, 격리 Hub Unity asmdef에는 이미 `AcadeGameMaker.Core` 참조가 있다. 보정 파일 `output/local-playable-slice-r1/project/Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/LocalPlayableSlicePresenter.cs`에 `using AcadeGameMaker.Core;`가 추가된 것을 직접 확인했다. 현재 원문은 16,277바이트/SHA-256 `D989E3F00DD11FAC0959E0E131EA219BD1217DFDA211AEC0FA38923C303BD3C5`이고 `output/local-playable-slice-r1/evidence/implementation-compile-delta.v1.json`의 after 지문과 일치한다. 이 보정은 관측된 미해결 식별자와 기존 참조에 맞는 한 줄 import이며, asmdef 및 원본 변경은 없다.

**판정:** 수정 범위의 정적 대조는 통과했으며 Root가 승인된 단회 컴파일 재시도를 진행할 수 있다. 이번 검토자가 재컴파일한 것은 아니며, 성공 여부는 새 실제 결과가 나오기 전까지 미확인이다.
