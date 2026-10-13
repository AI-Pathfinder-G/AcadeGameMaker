# CH1 전체 로컬 체험 전달 계약

- 상태: CH1 전체·C1-C는 Review, §10의 C1-B0 및 §12의 C1-B1 한정 구현은 Approved — 2026-10-03. 사용자 직접 우선순위와 각 제품 단계의 범위를 구별한다.
- 설계·단계 승인·통합: 아스트라 `gpt-6-astra`. 구현: 별도 테라 `gpt-6.1-sol`. 독립 검수: 루나 `gpt-6-luna`. 이후 배정은 ADR-0041을 따른다.
- 최종 목표: 사용자가 현재 소스의 Windows 빌드를 시작해 본편 CH1 `거부하는 사람`을 직접 완주하고 실패 뒤 다시 도전한다.
- 권위: 서사 캐논, 2026-08-24 시놉시스 승인, Approved VD-00~10 및 시스템 계약. NAR-00의 전체 Review 상태를 변경하지 않는다.

## 1. 완주 경계

시작 안내와 탈락 구역·유담의 상황을 알리는 도입 → 거점 출정 → 저작 방의 이동·무게 전이·일반 전투 → 유담의 공동장부와 오르단 압류 동기 → 오르단 격파 → 유담 선택의 확정·원자 저장 → 해당 기술의 비치명 봉쇄선 실제 사용 → 기존 `DemoCompleted`와 세령 첫 등장 → 세령이 아인을 찾으며 도언의 책임을 확인하려는 독립 목적을 드러내는 CH1 마무리 → 한 번의 CH1 완료 기록과 다음 장 예고가 이어져야 한다.

`DemoCompleted`는 재사용하는 기존 구간의 사건이며 그 자체를 CH1 완료로 해석하지 않는다. 별도 챕터 소유자가 기존 종료 영수증과 CH1 마지막 장면 완료를 모두 확인한 뒤 챕터 완료를 한 번 확정한다. CH2 플레이와 승강기 사건·그 후 조건부 동행 성립은 이번 범위 밖이다. 다음 장은 잠긴 예고로 표시하고 미구현 실행 경로로 보내지 않는다. 세령을 CH1 전투 동료·보상으로 넣지 않는다. 아인의 상태를 새로 확정하거나 구출·사망을 만들어 내지 않는다.

수탈은 유담의 거부와 각인 절단·공동장부 능력 상실 및 `압착 판결`, 연대는 범위 있는 동의·소유권 유지 및 `공동 기준면`을 연결한다. 기존 정확 기술 수치·대상·보존 규칙은 VD-06을 그대로 따른다. 정확 대사·장면 저작안은 이 후보의 후속 부록에서 캐논 검수한다.

## 2. 범위와 단계

| 단계 | 제품 산출물 | 승인·실행 관문 | 다음 단계의 실제 의존 |
|---|---|---|---|
| C1-A 첫 연결 | 기존 Ordan 생산 그래프에 입력·HUD·결과·새 그래프 재시도를 연결한 개발용 구간 | [첫 연결 계약](./2026-10-03-first-local-playable-slice.md)의 정확 파일/API/복제 입력/회귀 부록과 Luna 사전 검토 후 별도 Approved | 장치 입력·표시·수명주기의 실제 정상 증거. 이 구간 성공은 CH1 완료가 아님 |
| C1-B 원정 연결 | 거점1, 저작 방 풀6/원정당4, 일반 적2, 이동·전이·전투와 실패/부분 초기화/거점 재출정 | Terra 격차 조사로 기존 Run·Room·Profile 소유자를 지정하고 정확 파일 및 저장 격리 범위 승인 | 정상·실패 원정과 보스 진입 영수증 |
| C1-C 선택·서사 연결 | 유담 양분기·기술2·봉쇄선·세령 추적자·아인을 찾는 목적·CH1 종료 인계 | VD-06 원자 저장과 기존 종료 순서 보존, 챕터 완료 소유 API·대사·자산의 한정 부록 승인 | 동일 빌드에서 양분기 전체 완주 가능 |
| C1-D 사용자 전달 | 현재 소스 빌드·한글 안내·독립 실제 양분기 완주·실패 재도전 | 영향 필수 회귀·빌드 원장·Luna 독립 플레이 후 Astra 한정 수용 | 사용자에게 실행 경로 전달 |

첫 연결 완료를 기다려야만 C1-B/C 설계를 시작하는 직렬 대기는 두지 않는다. 같은 파일 쓰기는 한 명이 소유하며 구현은 해당 단계 Approved 이후다. 데모의 방/적/기술 수량과 12~18분 기준 및 3명 검수 AC는 삭제하지 않는다. CH1 추가 도입·종결 시간은 별도 측정하고 기존 데모 시간으로 섞지 않는다. 전체 데모 수용과 CH1 수용은 각각 필요한 근거를 충족해야 한다.

## 3. 요구사항

- **REQ-CH1-001:** 독립 빌드 시작부터 위 CH1 종료까지 개발 도구·장면 수동 전환·시험 API 없이 연결한다.
- **REQ-CH1-002:** 기존 생산 InputRouter·고정 tick·전이·피해·카메라 소유권을 보존한다. 첫 연결의 임시 결과 종료를 실제 CH1 보스 뒤 선택 전환과 혼동하지 않는다.
- **REQ-CH1-003:** 방 풀6/런4·일반 적2·오르단·거점 및 실패 부분 초기화의 기존 요구를 충족하고 현재 등록부로 집계한다.
- **REQ-CH1-004:** 두 선택 모두 실제 기술 사용으로 봉쇄선을 통과한다. 유담을 전이 대상으로 만들거나 미확정/실패에서 세령을 노출하지 않는다.
- **REQ-CH1-005:** 도입·유담의 거부/동의·선택 결과·세령의 독립 목적·아인을 찾는 목적가 실제 장면/표시로 인과를 전달한다. 점수 게이지로 인간성을 표시하지 않는다.
- **REQ-CH1-006:** 챕터 완료와 다음 장 인계는 명시된 소유자만 확정한다. 동일 이벤트 중복·로드·실패·재도전으로 선택/보상/완료가 중복 지급되지 않는다. CH2 조건부 동행을 앞당겨 확정하지 않는다.
- **REQ-CH1-007:** 개발용 격리 프로필 경로와 선택/실패/재시도 보존 표를 C1-B/C 승인 전에 고정한다. 실제 사용자 프로필·Q0 키·동결 복사본은 변경하지 않는다. C1-A의 비지속 상태를 CH1 저장 구현의 대체로 쓰지 않는다.
- **REQ-CH1-008:** 필요한 표현은 권리·규격·화면 판독 검수를 거친다. 임시 표현은 명시하고 최종 아트로 집계하지 않는다. 세령 첫 등장도 실제 판독 가능한 별도 연출이어야 하며 빈 화면/로그로 대체하지 않는다.
- **REQ-CH1-009:** 각 단계는 변경 REQ·검사 AC·정확 파일과 소유 API·실행 상한을 고정하고 정상 경로부터 시험한다. 기존 필수 회귀를 생략하지 않는다.
- **REQ-CH1-010:** 최종 동일 빌드를 Luna가 실제 입력으로 양분기와 실패 재도전을 확인한다. 코드/도구 시험을 사용자 플레이로 확대하지 않는다. 원격 게시와 Q0 새 세대 권한은 별도다.

## 4. 수용 기준

| AC | 완료 조건 | 추적 |
|---|---|---|
| AC-CH1-001 | 새 실행→거점→원정→보스→선택→봉쇄선→세령→아인을 찾는 목적→CH1 종료의 화면·사건 순서가 하나의 현재 빌드에서 실제 이어짐 | 001/002/005 |
| AC-CH1-002 | 고유 ID로 거점1/풀6/런4/일반 적2/보스1/선택1/기술2 및 원정 실패·재출정 확인 | 003/007 |
| AC-CH1-003 | 수탈·연대 각 한 완주에서 다른 기술/유담 결과, 정확 저장 및 VD-06 네 AC 충족 | 004/005/007 |
| AC-CH1-004 | 실패·미확정·봉쇄선 미통과에서 세령/CH1 완료0; 완료 중복·로드·재도전에서 중복 지급0 | 004/006/007 |
| AC-CH1-005 | 세령 추적자·아인을 찾는 목적와 CH2 예고 경계 독립 서사 검수, 동료/보상 오인0 | 005/006/008 |
| AC-CH1-006 | 정확 입력/빌드 지문·필수 회귀·한글 조작 안내·양분기 독립 실제 플레이·실패 재도전 근거가 모두 연결됨 | 008/009/010 |
| AC-CH1-007 | 원본/Q0 동결 변경0, 실제 사용자 저장 변경0, 구현자와 독립 검수자 분리, 미구현 CH2와 전체 본편 완료 주장0 | 007/009/010 |

## 5. 현재 작업과 승인 전 부록

현재 생산 부품은 존재하나 연속 CH1 구현 완료를 입증하지 않는다. Terra가 별도 실제 격차 보고에서 파일/API별 재사용·부족·소유를 조사한다. 이번 계약 작성은 제품 코드를 변경하지 않는다.

먼저 C1-A의 requester 로컬 opt-in 기본false, 기존 성공 receipt 보존, Input.Unity 불변 표시 값→Hub 표시, 결과 UIOnly에서도 필요한 내부 tick 유지, 완전 Destroy/입력 해제 뒤 새 그래프 경계를 확정한다. 이 작은 구현 묶음을 승인 가능한 상태로 만드는 작업을 CH1 전체 미결 때문에 멈추지 않는다.

C1-B/C는 원정/선택/프로필/챕터 소유 API, 저장·로드 표, 정확 대사/장면·자산 목록, 변경 파일·원문 지문, 집중/필수 회귀 선택과 유한 실행 계획이 필요하다. 이들은 전체 NAR-00 승인 없이 CH1 한정 부록으로 닫는다. Luna 사전 검토 후 Astra가 승인한 단계만 구현하며 Review 상태 자체는 구현 권한이 아니다. Q0 인계·계약·증거는 수정하지 않고 일일 감사는 별도 작업에 맡긴다.

## 6. 환경·실제 검수의 분리

실행 환경은 사용자 승인된 `2026-09-06-unity-6000-6-baseline-update.md`의 Unity6000.6.0f1/revision f7f8ed4d1e24를 적용한다. VD-09의 옛 환경 문구를 이번 목표 변경만으로 다시 바꾸거나 Unity/패키지를 업데이트하지 않는다. manifest/lock의 실제 차이와 후속 승인 호환 근거는 입력 부록에서 결속한다.

RoomPlan4 snapshot×실제 이동3의12회, 선택 양분기 수동 완주 및 기존 분량 검수3명은 자동 시험으로 대체하지 않는다. 사용자에게 먼저 제공할 현재 소스 체험 빌드와 모든 기존 AC를 충족한 최종 수용을 구별한다. 3명 분량 검수 등 미완료 조건은 정확히 남기며 사용자 체험 가능 사실까지 미확인으로 숨기지도 않는다.

실제 조사에서 Run/Choice의 생산 소비자와 방/봉쇄선 자산 연결이 부족하며 기존 ChoiceSkill callback이 아직 진단 경로라는 보고가 있다. C1-B/C의 우선 구현 대상은 새 프레임워크 전체가 아니라 기존 순수 코어의 좁은 생산 소비자와 전투 종료 뒤 별도 gameplay 그래프의 입력·수명 연결이다. 퇴역 보스 router 재활성화 우회는 금지한다. 상세 파일 판정은 테라의 실제 격차 보고와 Luna 대조 후 고정한다.

M5D2의 기존 Verified 순수 코어를 Unity 생산 어댑터나 M5A 연결의 승인으로 확대하지 않는다. C1-B 부록은 Run assembly 접근 권한과 실제 producer 출처, 세 원인의 닫힌 batch 및 accepted failure의 생산 전달을 명시해야 한다. runactive=true 또는 None pair 상수로 실제 상태를 위조하지 않는다. C1-A 결과에 ChapterCompleted/RunEndCommitted 권위를 붙이지 않는다.


### 서사 출처 보정 — Luna 사전검수 반영

설계 열쇠의 움직임을 추적하는 도입 및 아인의 구조용 걸쇠에 열쇠가 응답하는 종결은 반려된 상세 제안서의 비트로 확인됐다. 이전 후보에서 이를 확정 장면처럼 인용한 부분을 철회한다. 이번 필수 범위에는 넣지 않으며 신규 제안 상태로만 남긴다. 새로 채택하려면 별도 캐논·단계 검토가 필요하다. 현재 CH1 필수는 승인 캐논의 유담·오르단·양선택과 VD-06의 세령 추적자 첫 등장, 세령이 아인을 찾고 도언의 책임을 확인하려는 목적이다. 정확 대사는 C1-C 승인 부록에서만 정한다.

## 7. C1-B/C 생산 연결 후속 설계 후보

이 절은 Review다. C1-A 준비/구현에 새 friend·Run 소비·기술 확장을 끼워 넣지 않는다. 근거는 테라61 CH1 영향 보고 SHA `AB54935A65F6A3D166E4ABAFF99BC1DA9D624FAA7E0365F8FCAE69B65D79634E`, 직접 읽은 `PostBossProgressionSession`, `RunFailureArbitrationSession`, Approved VD-05/06이다. C1-A 승인에 이 절의 완성이나 전체 장면 제작을 선행시키지 않는다.

### 7.1 최소 어셈블리와 소유 방향

후속 `AcadeGameMaker.Run.Unity` 한 어셈블리가 기존 Run 순수 코어·Core·Input.Unity·Profile의 불변 관측/저장 결과를 소비한다. `Run/AssemblyInfo.cs`와 `Input/Unity/AssemblyInfo.cs`에 이 정확 친구만 허용하는 변경은 C1-B에서 별도 승인한다. Hub.Presentation.Unity는 Run.Unity의 불변 장면/선택 표시값만 읽고 Run.Unity→Hub 역참조는 금지한다. Combat.Unity의 세션 자체를 Run/UI에 공개하지 않는다. 기존 Input.Unity에서 완료된 전투/입력/정리 값을 투영한다.

후보 파일은 `Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs`(원정/장면 단계), `Chapter1FailureSources.cs`(닫힌 실패 batch), `Chapter1PostBossAdapter.cs`(M5A 입력), `Chapter1PlayableView.cs`(불변 표시), `Runtime/Input/Unity/Chapter1EncounterEvidenceView.cs`(실제 출처 투영)다. 정확 asmdef/메타/시험 파일은 C1-B 승인 부록에서 잠그며 이 이름을 전체 폴더 편집 권한으로 쓰지 않는다. 새 범용 서비스 버스·의존 주입 프레임워크·전체 Run 코어 재작성은 제외한다.

### 7.2 실제 원정과 실패 입력

`Chapter1PlayableFlowOwner`는 원정 시작 시 고유 Run 신원과 실제 시작 tick으로 `RunFailureArbitrationSession.Start`를 호출한다. 각 완료 tick의 체력0·KillPlane·LethalCrush 세 producer가 각각 확인된 true/false를 낸 뒤 닫힌 batch 하나만 `ObserveTick`에 넣는다. 미설치·미관측을 false로 채우지 않는다. 생산 source의 graph 세대·Run 신원·tick이 다르면 거절한다. PresenceMask=AllKnown은 세 관측을 모은 결과이며 하드코딩된 정상 입력이 아니다. 원인 우선순위는 기존 코어의 HealthDepleted→KillPlane→LethalCrush를 보존한다.

인정 실패는 그 세션 snapshot의 AcceptedFailure와 Revision에서 읽는다. 동시 player/boss 사망은 실패가 먼저 M5A에 보이도록 완료 tick의 실패 batch→보스 사망 관측 순서를 고정한다. M5A `ObserveDeath`에는 실제 handoff3 순서·체력·실행 중 원정 상태·저장 검증된 choice/skill pair를 공급한다. `RunActive=true`, `AcceptedFailure=null`, `None/None`을 편의 상수로 넣지 않는다. source 누락은 진단 정지이며 가짜 RunFailed나 성공으로 바꾸지 않는다.

M5A의 TransitionRequested 이후 새 실패 요청을 막는 경계는 소유자가 실제 적용한다. 죽음+1의 실제 teardown/등록제거/전이 baseline/대상비움 영수증을 받은 뒤에만 `CompleteCleanup`을 호출한다. `PrepareChoice`/`PrepareBarrier`는 준비 의도이며 기술 지급·장면 완료·보상 커밋이 아니다. LifecycleSuperseded는 성공 경로를 억제하는 기존 결과를 유지한다.

실패 소비자는 첫 유효 RunEndRequested(Failed)→Transition 입력 잠금→TransferCleared(RunFailed)→RunEndCommitted(Failed)→원정·방·전투 일시 상태 제거→보존 필드 저장→ReturnedToHub(Failed)→새 거점 GameplayEnabled 순서를 소유한다. 자산 파기를 실패 커밋 앞에 두지 않는다. 원정 신원/종료 revision의 중복 입력은 기존 결과만 돌려주고 보상·파기·전환을 반복하지 않는다. 시간 초과·메뉴·로딩 지연을 새로운 게임 실패 원인으로 만들지 않는다. 장면 파기와 입력 폐쇄는 C1-A에서 실증한 순서를 재사용한다.

### 7.3 그래프 교체와 입력 결속

전투 종류를 가짜 보스 alive 상태로 위장하지 않는다. 일반 전투 그래프와 봉쇄선 그래프는 각각 단일 combat/환경 owner와 단일 InputRouter를 갖는다. 보스 그래프의 고정 roster와 퇴역 판정은 유지한다. 후속에 필요한 변경은 Input.Unity 안의 좁은 gameplay 입력 준비/검증/commit 소비 경계이며 일반 적·보스·봉쇄선 중 정확 한 종류를 저작 설정으로 선택한다. 기존 Ordan ConfigureForAuthoring 진입은 기본 경로로 보존한다. 실제 인터페이스 타입/메서드와 transaction 취소 의미는 기존 원자 입력 프레임 계약에 맞춰 C1-B/C 승인 전에 고정한다.

보스 정리→UIOnly 선택→확정 저장 후 이전 그래프 완전 파기→새 봉쇄선 graph 활성 순서다. 원정 신원·확정 pair·실제 생존 체력은 값으로 넘기고 위치는 승인된 다음 입구, 속도0·활성 전이 비움으로 시작한다. 체력을 기본 최대값으로 몰래 회복시키지 않는다. 실패 후 새 원정은 VD-05에 따라 transient를 새로 구성하며 확정 pair는 유지한다. C1-A의 개발용 새 상태 재시도와 이 규칙을 혼동하지 않는다.

### 7.4 선택·원자 저장·완료

선택 화면은 UI 의미 frame의 확인/취소만 소비한다. 미확정 선택 취소는 유담의 거부 상태로 돌아간다. 확정 전 choice/skill/consent의 닫힌 후보를 검증하고 기존 Profile 원자 저장의 성공 결과가 나온 뒤 기술 사용 가능 상태를 게시한다. 저장 실패에서는 성공 표시·기술 grant·봉쇄선 진입을 보류하며 오류/재시도 의도를 명시한다. 기확정 pair는 검증 후 선택 화면을 생략하고 해당 기술만 사용한다. 같은 pair 저장 재시도는 중복 기술/보상 지급을 만들지 않는다.

프로필 위치는 C1-B의 격리 실행 식별과 기존 플랫폼 경로 계약을 대조한 정확 경로로 한정한다. 기존 일반 사용자 프로필·키 저장소를 탐색/초기화하지 않는다. 변경 가능한 필드는 선택/기술/동의·기존 보존 튜토리얼/분기뿐이며 활성 Run·현재 방·체력·위치·전이 대상은 저장하지 않는다. 실제 앱 재시작은 거점의 새 원정 준비 상태다.

봉쇄선 통과는 해당 기술의 실제 결과를 확인한다. 성공은 VD-05의 `TransferCleared → RunEndRequested(Succeeded) → RunEndCommitted → completedBranch` 원자 저장을 한 번 수행하고 VD-06의 세령 첫 등장 순서를 보존한다. 저장/단계 실패에서 컷신과 CH1 완료를 앞당기지 않는다. CH1 완료 표식의 저장 형식이 필요하면 C1-C에서 기존 프로필 버전/호환을 명시 승인하며 임의 JSON 필드를 추가하지 않는다.

### 7.5 두 기술의 실제 입구

새 순수 선택 기술 상태와 Unity 환경 소비자는 선택된 한 기술만 활성화한다. 기존 `ChoiceSkill` callback의 진단 플래그를 콜백 직접 피해로 바꾸지 않는다. 고정 입력 후보의 의미 edge로 받아 ChoiceSkill을 전이보다 먼저 처리한다. 성공하면 같은 tick TransferPressed를 소비하고 실패하면 전이 처리를 계속한다. 입력 mode·aim/LOS·활성 대상·cooldown·graph 세대 검증이 모두 끝난 후보만 commit한다.

- 압착 판결: 실제 활성 전이 대상에36틱 뒤 중심4/주변2 피해·60틱 경직, 전이 회수,300틱 재사용. 대상 제거/장면 교체 때 오래된 예약이 새 세대에 작동하지 않는다.
- 공동 기준면: 발동 tick의 대상/player X 기준150틱 PlayerOnlyPlatform, 길이1.5~6.0·두께0.25,420틱 재사용. 대상 소유자가60틱 ResonanceHold를 적용하며 제거/사망/전환 시 속도 복원 없이 정리한다.
- 봉쇄선: 전이 대상은 `BarrierCounterweight`만이다. 유담/ConsentAnchor는 대상이 아니다. 수탈의 Fractured 파괴·회수와 연대의 발판/ConsentLock 해제가 각각 실제로 통과를 만든다. 비치명 낙하는 입구 복귀이며 RunFailed로 바꾸지 않는다. VD-06의6.0 이내/LOS 및 양경로 통과 시간 차이10초 이내를 검수한다.

### 7.6 영향 검사와 다음 구현 순서

C1-B는 실제 producer 세 원인 각각 정상/음성·동시 원인·같은 tick 보스사망·다른 Run/tick 거절→기존 M5D2/M5A 및 terminal 회귀→실제 실패복귀/새 graph를 먼저 닫는다. C1-C는 실제 선택 저장 정상/실패/기확정→각 기술 작은 표적 정상 입구→쿨다운·지연·제거·전환·두경로 봉쇄선→서사/종료·앱 재시작 순서로 집중 검증한다. 기존 테스트의 합성 입력을 생산 source 연결 성공으로 세지 않는다.

현재 실행 가능한 다음 작업은 C1-A §10.1 원장 준비다. 위 후속 설계는 그 작업이나 C1-A 코드 착수를 지연시키는 추가 선행 문서가 아니다. C1-A 독립 부록이 닫히면 먼저 구현하고, 실제 first tick 결과와 병행해 C1-B/C exact API와 파일 승인 범위를 좁힌다.
## 8. C1-B/C 시간·체력·입력 생산 연결 보정 후보

이 절은 실제 솔 `gpt-6.1-sol`의 읽기 전용 반대검토와 아스트라의 원문 대조를 반영한 Review 설계다. C1-A Approved 원문/398개 입력/현재 구현 파일은 변경하지 않는다. 아래 추가 파일과 메서드는 C1-B/C 한정 승인 뒤에만 구현한다.

### 8.1 원정의 연속 tick과 장면 수명

현재 `PlayerMovementController.EnsureMotor()`는 tick0을 생성한다. 이를 원정 그래프 전환에 그대로 쓰면 M5D2의 exact sequential tick과 충돌한다. 격리 후속본에 `ConfigureChapter1EntryForAuthoring(firstTick, position)`을 추가하는 후보로 고정한다. motor 생성·입력 등록·첫 FixedUpdate 전에 단 한 번만 설정하며 기존 비CH1 경로는 tick0 그대로다. 인수는 원정 소유자가 만든 불변 입구 값에서 받고, 소진된 tick·음수·중복 설정·다른 graph 세대를 거절한다. 위치는 승인 입구의 양자화 좌표, 속도는0이며 이를 일반 teleport/중간 reset API로 공개하지 않는다.

원정 소유자는 마지막으로 완성된 권위 tick을 기록하고 새 graph의 첫 tick을 정확 후속값으로 예약한다. 기존 graph가 살아 UIOnly/Transition tick을 처리하는 동안은 그 실제 movement/input 완료 tick을 계속 소비한다. 파기·로드처럼 권위 graph가 없는 구간은 시뮬레이션 시간을 정지하며 벽시계 경과를 가짜 완료 tick으로 채우지 않는다. 한 tick에 두 graph가 실행되거나 이전 graph의 늦은 publication이 새 graph를 진행시키지 못한다. 새 graph를 활성화하기 전에 Movement/Transfer/Combat/입력/카메라 seed와 firstExpectedTick 일치를 한 번 검증한다.

보스 이전 이동·방 전환 중 Active Run의 실패 batch는 실제 연속 tick마다 세 source를 닫는다. 잠긴 전환에서도 source 부재를 정상 false로 대체하지 않는다. 보스 사망 뒤 M5A의 실패 요청 잠금은 명시 별도 terminal 경계이며, 잠긴 후 없는 실패 tick을 합성하거나 실패 prefix를 새 tick0 인스턴스로 교체하지 않는다. 그 뒤는 실제 진행/입력 소유자의 연속 tick으로 봉쇄선까지 진행하고 성공 커밋 또는 기술적 중단을 구별한다. 실패 prefix의 마지막 snapshot을 후속 전체 원정 시간의 유일 시계로 오인하지 않는다.

필수 음성은 다른 Run/graph·중복 활성화·tick 역행/공백·int 경계 소진·생성 후 seed·파기 후 늦은 receipt다. 정상은 서로 다른 세 graph가 전역 연속 tick을 유지하고 로드 지연 자체가 실패나 cooldown 진행을 만들지 않는 경로다.

### 8.2 현재 체력과 잔여 무적의 초기 승계

현재 `CombatSession.Register`는 MaxHealth로 시작한다. 후속 `RegisterCarriedPlayer(registration,currentHealth,invulnerabilityEndTick)`는 **최초 등록·첫 처리 전의 player 한 명**만 받는 좁은 후보 API다. 생존체력1..MaxHealth와 실제 이전 완료 snapshot의 잔여 무적 종료 tick을 검증한다. 이미 만료된 무적은 만료 상태로 정규화하고 새로운 무적 시간을 선물하지 않는다. 기존 Register와 적/보스 MaxHealth 시작은 그대로다. 실행 중 체력 setter·치유·부활 API가 아니다.

원정/graph 신원과 이전 완료 tick의 출처 검증은 Input.Unity의 불변 인계와 Run.Unity 입구 소유자가 담당한다. pure CombatSession은 이 provenance를 스스로 OS/장면에서 찾는다고 주장하지 않는다. 일반 CombatSimulationDriver, OrdanBossCombatSimulationDriver와 새 BarrierCombatSimulationDriver의 session 생성 전에 검증된 player seed를 적용한다. player 사망은 다음 생존 graph로 넘기지 않는다. 기본 공격/피격 예약·적 체력·전이 대상 참조는 승계하지 않고 원래 소유자에서 정리한다. 실패 뒤 새 원정 최대 체력 시작은 VD-05의 별도 초기화이며 방간 체력 승계와 구별한다.

필수 검사: 부분 체력의 regular→boss→barrier 동일 승계, 남은 무적의 정확 전역 만료, HP0/초과/다른Run/중복등록/처리 후 seed 거절, legacy Register 결과 불변. 기존 Combat 순수코어 및 각 driver의 초기 등록·피해·무적 회귀를 추가한다.

### 8.3 일반 전투·봉쇄선의 원자 입력 경계

`Input.Unity/Chapter1GameplayInputSink.cs`는 일반 전투/봉쇄선 두 종류만 대상으로 하는 내부 결속 후보이며 기존 Ordan 설정 경로를 대체하지 않는다. `InputRouter.ConfigureChapter1GameplayForAuthoring`은 정확 한 종류와 BoundPlayer를 초기화 전에 결속한다. sink는 등록·Prepare/Validate/CommitInputFrame과 source/pending/global receipt 비교를 제공한다. `CombatSimulationDriver.Submit` 호출을 감싼 것만으로 원자성을 주장하지 않는다. 실제 phase 진입 전에 pending proof와 전역 InputFrameCommitReceipt·source identity/fault를 검증하고 실패/부분 적용에서는 실행하지 않는다.

새 BarrierCombatSimulationDriver는 일반 적이나 가짜 오르단을 등록하지 않는다. player와 저작된 봉쇄선 피해 가능 대상만 가지며 기존 CombatSession/DamageRequest 판정을 소비한다. 전이 대상의 등록과 피해 대상의 등록은 구별한다. `BarrierCounterweight`의 전이·기술 성공과 문/발판의 실물 결과를 바탕으로만 통과를 게시한다. 인터페이스와 driver의 정확 파일·참조·대상ID·tick순서는 C1-C 구현 승인 표에 함께 고정한다.

### 8.3.1 기존 단일 입력 확정 절차에 편입

§8.3의 구체 입력 편입은 다음 순서로 한정한다. 이 문단은 루나 사전검수의 원자 결속 P1을 보완하는 Review 규범 후보이며 C1-A 구현을 변경하지 않는다.

1. `ConfigureChapter1GameplayForAuthoring`는 기존 Ordan 설정과 상호 배타적으로 일반 전투 또는 봉쇄선 한 종류만 등록한다. `BoundPlayer`는 같은 player 객체여야 하고 등록 소유자는 실제 이 InputRouter 객체 하나다. 원정 키·graph 세대·등록 token·firstExpectedTick을 초기화 때 고정한다. 새 sink가 별도 callback·별도 입력 receipt 또는 두 번째 입력 확정 루프를 소유하지 않는다.
2. 현재 생산 `InputRouter.CommitNextFrame` 안에서 동일 frozen batch로 tick/frameOrdinal/modeEpoch/mode의 receipt 후보를 한 번 만든다. Movement→Transfer→선택된 Combat 순서로 후보를 모두 Prepare하고, 동일 순서로 모두 Validate한 후에만 첫 Commit을 시작한다. 일반/봉쇄선 분기는 기존 Ordan combat 후보 자리만 대체하며 Movement/Transfer/UI 의미 입력을 별도 transaction으로 쪼개지 않는다. tick/ordinal/aim/attack 후속값의 checked 연산과 UI 후보 검증도 첫 Commit 전에 끝낸다.
3. 각 Combat 후보는 발급 driver 객체·등록 router 객체/token·원정/graph·receipt 전체 값·명령·pending 세대를 불변으로 담는다. Validate는 다른 driver/router/graph, 오래된 pending, 중복 후보, tick/receipt 불일치와 기존 fault를 거절한다. Prepare/Validate는 현재 pending·소비 tick·피해·전이 상태를 바꾸지 않는다. `Submit` 호출을 검증으로 사용하지 않는다. Commit은 이미 검증된 후보를 해당 pending 슬롯에 한 번 설치하고 그 동일 receipt 증명을 보관한다.
4. CommitMovement→CommitTransfer→CommitCombat→기존 map 전환→기존 frozen/UI batch 정리→UI publication/proof→전역 `_currentReceipt` 마지막 대입의 순서를 유지한다. generic sink 내부에서는 전역 receipt를 게시하지 않는다. 첫 Commit 이전 실패는 frozen batch를 유지하며, 첫 Commit 이후나 map 실패는 기존 fault latch로 전체 graph의 후속 phase를 막는다. 부분 적용을 정상 rollback했다고 주장하거나 다음 tick에 자동 재시도하지 않는다.
5. 실제 `CombatSimulationDriver`와 새 Barrier driver의 phase 입구는 자기 pending receipt와 등록된 router의 현재 전역 receipt 전체 값 및 등록 객체/token·graph·tick을 대조한다. router fault/미게시/다른 receipt면 피해나 소비를 시작하지 않는다. Movement/Transfer도 기존 동일 전역 receipt 관문을 그대로 사용한다. 따라서 세 pending 설치 중간에 다른 FixedUpdate가 실행되더라도 전역 마지막 게시 전에는 생산 처리를 할 수 없다. Run.Unity는 세 생산 phase가 실제로 완료한 불변 evidence를 받아야 하며 입력 receipt 하나만으로 완료를 추론하지 않는다.
6. 필수 회귀는 기존 Ordan 경로 불변, regular/barrier 정상 각1, 세 Prepare/Validate 지점의 실패 뒤 소비0·batch 보존, 각 Commit 뒤 및 map 전환 실패의 전역 미게시·모든 phase 거절, 다른 driver/router/graph/tick/ordinal/epoch/mode 및 중복 receipt 거절이다. 기존 M5B4/M5B5 입력 실패·원자성 검사를 새 두 combat 분기에도 같은 의미로 적용한다. 원정/선택 입력 구현 승인 표에서 정확 메서드와 시험 ID를 채택하기 전 생산 구현은 하지 않는다.

### 8.4 저장의 실제 결과와 실패 순서

`ProfileProgressionSnapshot.Validate`와 `TryGetCommittedPair`로 실제 pair를 읽고 `ProfileAtomicSaveServiceV1.Save`의 `CommittedFirst` 또는 `CommittedReplacement` 및 정확 저장 revision만 기술/진행 게시 근거로 삼는다. `CommitOutcomeUncertain`은 성공으로 취급하지 않는다. 기존 플랫폼 복구 경로의 실제 읽기·검증이 끝날 때까지 선택/기술/장면 전환을 보류한다. 실패 커밋 뒤 보존 저장이 실패하면 반환 완료/새 출정을 앞당기지 않고 진단 가능한 저장 오류 상태로 남긴다.

SYSTEM-CONTRACTS의 실패 순서가 §7.2보다 우선하며 이 보정본에서 §7.2 자체도 일치시켰다. 성공과 실패의 유효 첫 RunEndRequested는 동일 소유자가 한 번만 커밋한다. source별 중복·동일tick 복합 실패·저장 불확실·앱 재시작을 독립 검사한다.
### 8.5 압착 성공 회수와21틱 규칙 — 한정 결정 후보

VD-02의 성공 전이/회수 뒤21틱 상태 불변과 SYSTEM-CONTRACTS의 Choice 우선순위를 함께 보존하는 후보로 정한다. 압착 판결은 활성 전이의 전환 잠금이 끝나지 않았으면 `ChoiceSkillResult(Cooldown)`으로 실패한다. 기술 cooldown·지연피해 예약·회수는 발생하지 않고 같은 tick TransferPressed는 기존 규칙대로 계속 판정한다. age21 경계부터만 압착 성공 후보가 회수를 포함할 수 있다. 연대는 전이를 유지하므로 이 회수 자격을 추가로 요구하지 않으며 자체420틱 기술 cooldown과 기존 유효 대상 조건을 따른다.

압착 성공은 일반 TransferPressed를 합성하지 않는다. `RecallForChoiceSkill`이라는 좁은 내부 입력을 TransferSession/TransferSimulationDriver의 Choice 우선 phase에 추가하고, 실제 입력 frame receipt·Run/graph 세대·tick·선택 pair·활성 target ID·전이 revision에 결속한다. 외부 UI나 콜백이 임의 true로 호출할 수 없고, 다른 대상/옛 receipt/같은 기술 중복 소비는 거절한다. 사전 검증된 성공 후보를 같은 phase에서 한 번 commit해 player baseline·대상 modifier 제거·active target 비움을 게시하고 새 전환 잠금 종료를 정확 `t+21`로 설정한다.

공개 `TransferCleared` reason을 늘리지 않는 최소 후보는 사용자 기술 입력에 의한 회수를 기존 ManualRecall로 게시하되, 별도의 실제 ChoiceSkillResult와 내부 회수 receipt로 기술 출처를 결속하는 것이다. 전이 이벤트만 보고 피해 예약이나 기술 성공을 추론하지 않는다. enum 추가/옛 이유 의미 변경이 필요한 실제 소비자가 발견되면 해당 변경을 먼저 검토하고 구현에 숨기지 않는다.

압착36틱 지연피해는 성공 시 고정한 대상 ID/graph 세대에 결속된 DamageRequest 예약으로만 처리한다. 회수 후 새로 활성화한 다른 전이 대상을 공격하거나 회수하지 않는다. 대상 제거/장면 전환/실패 정리 뒤 오래된 예약은 취소한다. callback 직접 피해·회수 성공 후 뒤늦게 candidate 검증 실패·기술 성공 없이 cooldown만 시작하는 부분 결과를 금지한다.

필수 경계는 전이 age20/21/22, 기술 재사용299/300, 같은tickChoice+Transfer, 연대의 활성전이 유지, 성공 회수 후 새21틱 잠금, 동일receipt재전송, target제거/graph교체 전후36틱 예약이다. 이 후보는 C1-C의 명시 승인 대상이며 C1-A 입력/전이 파일 수정 권한을 늘리지 않는다.
솔61의 후속 반대검토도 위 최소안을 권고했다. 압착의 전이 잠금 자격은 기존 VD-06에 이미 명시된 규칙이라고 주장하지 않고 **C1-C에서 새로 명시 승인할 보정**이다. 생명주기/대상 제거가 먼저 처리된 뒤 살아 있는 정상 기술만 일반 전이 입력보다 우선한다. 성공 압착은 회수 revision+1·t+21 전환 잠금·t+36 피해 예약·t+300 기술 재사용을 같은 확정 묶음으로 게시한다. 성공 연대는 발판/hold·t+420을 확정한다.

기술 실패의 무효과는 기술 자체에 한정한다. 그 뒤 같은 tick 일반 Transfer가 성공하거나 기존 진단·조준 강조가 갱신되는 정상 동작까지 금지하지 않는다. 각 입력 edge는 한 번 시도하고 Cooldown 실패를 다음 tick에 자동 재시도하지 않는다. 준비/검증 실패에서는 frozen batch를 유지하며 부분 commit 오류가 발생하면 전역 receipt를 게시하지 않고 fault로 멈춘다. AC-WT-003/006·AC-UX-004·AC-CHOICE-004 및 새 CH1 경계 검사를 함께 추적한다.

### 8.6 기술 대기 표시와 한 번 사용의 의미 — Review

압착의 새21틱 발동 자격은 C1-C 승인 보정으로만 채택하며 기존 VD-06의 명시 사실로 소급하지 않는다. UI는 실제 해당 tick의 권위 snapshot에서 `전이 안정화 중`과 남은 tick에 근거한 대기 표시를 보여 주고, 자격·대상·기술 cooldown이 모두 유효하면 `압착 판결 사용 가능`을 표시한다. age20에 누른 입력은 그 tick에 실패하며 기술 cooldown·회수·예약은0이다. age21이 되어도 누른 채 유지한 키를 자동 성공시키지 않는다. `키를 놓고 다시 누르세요` 피드백을 표시하고 age21 이후 발생한 새 의미 edge만 성공 후보로 받는다. 렌더 시간이 아니라 생산 tick으로 자격을 판단한다. 연대에는 이 압착 전용 전환 잠금 대기를 표시하지 않는다.

AC-CHOICE-004의 봉쇄선에서 기술을 한 번 사용한다는 검수는 **유효 자격 이후의 성공 사용 한 번으로 통과 경로가 만들어짐**을 뜻한다. 실패 edge를 성공 횟수에 합치거나 재입력 횟수를 숨기지 않는다. 정상 독립 플레이에서는 대기/가능 표시를 보고 새 edge 한 번으로 각 분기를 통과하며 기존 양경로 시간 차이10초 이내를 유지해야 한다. 별도 경계 검사는 age20 실패→held 상태 age21 자동성공0→release 후 새 edge 성공1, 회수 후 age20/21, 기술 실패 뒤 동시 일반 Transfer의 정상 성공/거절, 표시 snapshot과 실제 자격 일치를 확인한다. 이 보정이 실제 사용성·시간 기준을 만족하지 못하면 AC를 완화하지 않고 설계 검토로 되돌린다. 현재 제품 구현·플레이 승인이나 CH1 수용을 의미하지 않는다.

## 9. C1-B 첫 생산 묶음 승인 후보 — Review

루나 독립 기록 `2026-10-03-chapter1-flow-luna-precheck.md` SHA `EDF53792E787C1458CE34861026554A23607EB67D4787D2DB8048EF4F1BFB449`는 §8의 설계 P1 두 건을 닫았다. 아래는 그 후속 정확 구현 후보이며 아직 Approved가 아니다. C1-A 작성 중인 파일은 해당 구현자의 결과 지문과 독립 검수를 받은 뒤 같은 구현자에게 후속 인계한다. 원본 Assets·Q0를 편집하지 않는다. 격리 프로젝트는 기존 `output/local-playable-slice-r1/project/`를 이어 사용하며 C1-A 결과를 덮어 원래 입력398과 같다고 주장하지 않는다.

### 9.1 먼저 승인할 B0 — 입력·입구 상태·완료 증거

이번 최소 묶음은 regular 생산 입력, graph 전환 시 tick/HP 입구와 실제 완료 snapshot까지다. 실패 세 원인의 생산자와 저장을 함께 미완성 상태로 연결하지 않는다. B1의 실패 처리와 네 방 구성은 B0 증거를 소비하며 CH1 최종 목표에서 제외되는 것이 아니다.

정확 수정 경로는 아래의 `Assets/AcadeGameMaker/Runtime/` 상대 경로다. 기본 경로와 기존 수치는 보존한다.

| 파일 | 허용 API·변경 |
|---|---|
| Input/Unity/InputRouter.cs | `ConfigureChapter1GameplayForAuthoring`의 regular 단일 분기; §8.3.1 등록/Prepare/Validate/Commit 및 최종 receipt 연결. C1-A local opt-in과 기존 Ordan 설정을 동시에 켜면 초기화 거절 |
| Combat/Unity/CombatSimulationDriver.cs | `ValidateCoordinatedInputRegistration`, `RegisterCoordinatedInputRegistration`, `PrepareInputFrame`, `ValidateInputFrameCandidate`, `CommitInputFrame`; 실제 `AdvanceCombatPhase` 앞 pending/global receipt 검사. 기존 비등록 Submit 경로 유지, 등록 뒤 Submit 우회 거절 |
| Movement/Unity/PlayerMovementController.cs | pre-initialization `ConfigureChapter1EntryForAuthoring(firstTick,position)`와 `EnsureMotor`의 선택적 seed 소비. 기본0 유지 |
| Transfer/Unity/TransferSimulationDriver.cs | pre-initialization `ConfigureChapter1EntryForAuthoring(firstTick)`; 새 session과 input 첫 tick 정합, 기본경로 유지. 활성 전이 승계·Choice 회수는 없음 |
| Combat/CombatSession.cs | §8.2 `RegisterCarriedPlayer` 한 메서드; 기존 Register 불변 |
| Combat/Unity/CombatSimulationDriver.cs 및 OrdanBossCombatSimulationDriver.cs | 최초 player 등록 전 `ConfigureChapter1CarriedPlayerForAuthoring(firstTick,currentHealth,invulnerabilityEndTick)` 소비; 기존 적/보스 등록과 수치 불변 |

정확 신규 파일은 `Input/Unity/Chapter1GameplayInputSink.cs`, `Input/Unity/Chapter1EncounterEntry.cs`, `Input/Unity/Chapter1EncounterEvidenceView.cs`와 각 meta다. 새 asmdef나 friend는 B0에 없다. sink의 B0 허용 종류는 regular 하나이며 Barrier 분기는 C1-C 전까지 생성·실행할 수 없다. 후속 Barrier를 위해 비어 있는 성공 구현이나 가짜 Ordan을 넣지 않는다.

`Chapter1EncounterEntry`는 내부 불변 값으로 원정 키·graph 세대·firstTick·position·HP·무적 종료 tick을 묶는다. 등록 소유자 token은 실제 InputRouter 인스턴스가 한 번 생성하는 내부 객체이며 문자열/외부 bool로 대체하지 않는다. graph generation은 같은 원정에서 단조 증가하고 이전 graph 종료 증거가 없으면 새 입구 활성화를 거절한다. 소유 객체 참조는 runtime 등록에만 쓰며 저장하지 않는다. 동등 값의 다른 token, 이전 graph candidate, 초기화 후 seed는 거절한다.

`Chapter1EncounterEvidenceView`는 실제 등록 router가 동일 tick의 Movement/Transfer/Combat 완료 snapshot과 receipt를 대조해 게시한다. 완료 전 조회는 HasCompleted=false이며 player 체력·위치·전이의 정상 false를 대신 만들지 않는다. 세션 참조/수정 API는 노출하지 않는다. 이 view만으로 KillPlane/LethalCrush 미발생이나 실패 batch 완료를 주장하지 않는다.

### 9.2 B0 요구·시험과 유한 실행

#### B0 실제 입구 발급·비활성 구성 소유자 보정

루나가 확인한 입구 발급자 누락을 닫기 위해 B0 신규 허용 후보에 `Input/Unity/Chapter1EncounterLifetimeOwner.cs`와 meta를 추가한다. 이 단일 MonoBehaviour는 graph 바깥에서 최초 원정 키를 한 번 만들고 generation1/firstTick0을 발급한다. 같은 수명 동안 키는 바뀌지 않으며 후속 입구는 실제 이전 완료 view와 아래 파기 증거에서만 만든다. B1 FlowOwner는 이후 이 owner를 합성하여 진행을 지시하고 키를 다시 발급하거나 독자 graph 생성/파기를 하지 않는다. B0에서 RunActive/RunEndCommitted를 만들지 않는다.

정확 내부 입구는 `BeginAuthoredRegularEncounter(prefab)`, `RequestRegularReplacement(prefab)`, `TryGetCompletedEvidence(out view)`다. runtime 호출자는 같은 owner 인스턴스에 결속하며 동시 요청·활성 owner 교체·다른 키/세대 요청을 거절한다. B0 실제 교체는 regular→regular만 허용한다. Ordan HP seed는 초기 등록 집중 검사까지이며 실제 regular→Ordan 전환은 기존 terminal requester와 B1 진행 소유자를 결속한 다음 승인한다. 기존 C1-A presenter와 이 owner를 동일 graph에 붙이지 않는다.

owner는 비활성 staging 부모를 먼저 만들고 그 부모 아래 prefab을 인스턴스화한다. 생성된 graph의 activeInHierarchy=false를 확인한 채 entry를 설치하며, 도중 Snapshot/NextExpectedTick/Initialize처럼 motor/session을 만드는 getter를 호출하지 않는다. 모든 seed setter는 초기화 여부를 비생성 필드로 검사하고 이미 Awake/EnsureMotor가 실행됐다면 즉시 거절한다. Movement→Transfer→Combat seed를 설정하고 router 등록 설정과 camera/player 참조를 결속한 다음 비생성 설정 검증으로 firstTick 일치를 확인한다. 마지막에만 staging 부모를 활성화한다. 최초 실제 Awake/Start/FixedUpdate가 같은 seed를 소비한 결과는 첫 완료 view에서 검증하며 설정값 비교를 실행 성공으로 세지 않는다. 실패한 비활성 graph는 파기하며 활성화하지 않는다.

현재 regular prefab에는 입력/카메라가 없으므로 B0 저작 후보에 `Editor/CombatAuthoring/Chapter1RegularEncounterAuthoringBuilder.cs`, `Chapter1RegularEncounterAuthoringValidator.cs`, `Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab`과 meta를 추가한다. 기존 CombatEncounterGraph의 Walker/Surveyor·combat/transfer/반응/행동/이동/위협/인계 구성을 유지한 별도 prefab에 InputRouter와 기존 SimulationCameraDriver를 결속한다. 기존 prefab/builder는 수정하지 않는다. 새 validator는 regular sink 한 개·lifetime owner 바깥 한 개·기존 producer 전부·seed 전 비활성 구성을 확인한다. Editor asmdef의 기존 참조만 사용하며 새 접근이 필요하면 실제 원문 근거로 먼저 좁힌다. 새 카메라 시뮬레이션·일반 적 수치·임시 보스 대체는 없다.

regular router의 유일 mode requester는 이 lifetime owner다. 교체 요청은 기존 RegisterModeRequester/RequestMode로 Transition을 요청하고 그 실제 receipt 및 동일 tick의 세 producer 완료 view를 기다린다. 다음 렌더 단계에서 graph를 비활성화하고 Destroy한다. 그 사이 발생한 마지막 실제 완료 tick을 채택하며 완료하지 않은 tick을 더하지 않는다. 비활성화만으로 파기 완료라고 하지 않는다.

InputRouter의 B0 opt-in 전용 `Chapter1InputLifetime` 읽기 증명을 `Chapter1EncounterEntry.cs`에 정의하고 router가 보유하는 비공개 witness를 통해서만 게시한다. setter·임의 bool constructor는 외부에 두지 않는다. witness의 완료 조건은 실제 OnDestroy 관측, callback 제거, 실제 소유 actions.Dispose 성공이며 예외면 닫힘false다. C1-A LocalPlayableLifetime의 local opt-in 조건을 CH1에 몰래 재사용하지 않고 별도 읽기 API `Chapter1Lifetime`로 제공한다. 기존 경로와 C1-A 판정은 유지한다. owner는 Unity의 이전 graph null과 이 보유 증명의 완료를 모두 확인한 다음에만 generation+1/마지막 완료 tick+1/살아 있는 player HP·무적 종료값을 새 entry로 발급한다. 이것이 predecessor 종료 증거이며 완료 view 하나로 대신하지 않는다. 파기 대기는 최대300 렌더 프레임 또는 단조 시간5초 중 먼저 도달하는 한계로 실패를 표시하고 새 graph를 만들지 않는다.

AC-C1B-ENTRY-001은 실제 비활성 생성→seed→활성화 첫 tick 및 regular 두 graph 교체를 요구한다. 추가 정확 시험은 `Chapter1EntryPlayModeTests.InactiveConstructionSeedsBeforeAwake`, `RejectAlreadyInitializedEntry`, `ReplacementWaitsForDestroyAndActionsDispose`, `RejectForgedOrUnclosedPredecessor`다. 시험은 실제 Unity 수명주기를 사용하고 합성 닫힘 bool로 대체하지 않는다. 이 보정도 Review이며 B0 전체 독립 재검토 뒤 한정 Approved를 발급한다.

| 요구 | 수용 기준 | 정확 신규 시험 클래스/메서드 |
|---|---|---|
| REQ-C1B-INPUT-001 | AC-C1B-INPUT-001: 같은 receipt 정상 확정, 다른 token/graph/receipt 거절 및 부분 commit 후 phase0 | 기존 Input Unity PlayMode 시험 assembly의 `Chapter1RegularInputPlayModeTests.RegularCommitUsesGlobalReceipt`, `RejectForeignRegistration`, `RejectStaleGraph`, `PrepareFailureKeepsBatch`, `PartialCommitBlocksAllPhases` |
| REQ-C1B-ENTRY-001 | AC-C1B-ENTRY-001: firstTick 일치/역행·공백·소진·늦은seed 거절, 이전 graph와 중복 활성화0 | 같은 assembly의 `Chapter1EntryPlayModeTests.SeedsAllProducersAtOneTick`, `RejectLateOrMismatchedSeed`, `RejectOverlappingGeneration` |
| REQ-C1B-HP-001 | AC-C1B-HP-001: regular→Ordan 부분HP·절대무적종료 승계, HP0/초과/중복/처리후 거절, legacy 최대HP 불변 | 기존 Combat EditMode 시험 assembly의 `Chapter1CarriedPlayerTests.PreservesHealthAndInvulnerability`, `RejectsInvalidOrLateCarry`, `LegacyRegistrationUnchanged` |
| REQ-C1B-EVIDENCE-001 | AC-C1B-EVIDENCE-001: 세 실제 완료 전 게시0, 다른 tick 혼합 거절 | `Chapter1RegularInputPlayModeTests.PublishesOnlyCompletedProducerEvidence` |

각 클래스 파일은 해당 기존 시험 폴더에 동명 `.cs`와 meta만 추가한다. 테스트가 새 friend/다른 assembly가 필요하면 구현자가 임의 추가하지 않는다. 기존 M5B4/M5B5 입력 원자성·Movement tick·Transfer 초기화·Combat 등록/무적·Ordan 및 C1-A 집중 시험은 회귀에 포함하며 승인 전 실제 입력 원장과 discovery ID를 고정한다. 시험 숫자를 먼저 꾸미지 않는다.

B0 승인 후 실행 순서는 소스/Luna 사전 대조→동일6000.6.0f1 격리 compile1회15분→신규 집중1회10분→영향 회귀1회30분이다. 종료·배수 각5초는 별도이고 실패 동일 실행 자동 반복0이다. B0만으로 새 배포 빌드·사용자 CH1 완주·원격 게시를 승인하지 않는다. C1-A 실행과 같은 프로젝트를 동시에 열지 않는다.

### 9.3 B1 실패·개발 저장과 C1-C 연결 경계

현재 원본 Movement/Transfer/Combat runtime 검색에서 `KillPlane`/`LethalCrush` 생산 판정기를 확인하지 못했다. 그러므로 B1을 B0와 묶어 AllKnown 상수를 바로 공급하는 구현은 금지한다. B1의 정확 후속 후보는 새 `Run/Unity/Chapter1PlayableFlowOwner.cs`, `Chapter1FailureSources.cs`, `Chapter1RunView.cs`, `AcadeGameMaker.Run.Unity.asmdef`다. Run/AssemblyInfo와 Input/Unity/AssemblyInfo에 Run.Unity friend를 각각 한 개 추가하고 Run.Unity는 Run/Core/Input.Unity/Profile만 참조한다. Input.Unity는 Run.Unity를 참조하지 않는다. 실제 방 경계·압착 생산자의 고정 데이터/완료 API가 정해져야 이 단계가 Approved가 된다.

FlowOwner만 원정 키와 graph 세대, 현재 진행 단계, accepted failure와 Run 종료 커밋을 소유한다. `ObserveCompletedEncounter(view)`는 실제 세 원인 완료 batch를 받은 뒤 RunFailureArbitrationSession.ObserveTick을 호출한다. `CommitFailedRun(intent,transferClearedReceipt)`는 같은 원정의 최초 유효 실패·Transition 확인·실제 RunFailed clear 뒤 한 번만 허용하고 자산 제거는 그 다음이다. 현재 실패 코어가 RunEndCommitted를 구현한 것처럼 부르지 않는다. 기술 오류나 source 누락은 원정 실패 이벤트로 위장하지 않고 진행 차단으로 구별한다.

개발 프로필은 후속 승인 시 `output/local-playable-slice-r1/profile/chapter1-development/` 아래 고정 단일 슬롯만 사용한다. platform 기본 사용자 경로나 기존 저장으로 자동 fallback하지 않는다. 원정 상태·HP·tick·graph token은 저장하지 않는다. 저장 어댑터는 실제 기존 atomic save의 정확 결과와 revision을 소비하고 저장 실패/불확실 동안 ReturnedToHub 또는 선택 기술을 게시하지 않는다. 이 절은 아직 파일 생성·저장 API 호출 권한이 아니다.

C1-C의 `TryCommitChapterCompletion(demoEndReceipt,chapterClosingReceipt,profileCommitReceipt)`는 같은 FlowOwner의 별도 종료 입구 후보다. DemoCompleted만으로 호출 성공하지 않고, 고정 CH1 마지막 장면과 해당 분기의 원자 저장 결과를 함께 확인해야 한다. B0/B1에서는 이 메서드의 성공 stub·완료 bool·CH1 완료 저장 필드를 만들지 않는다. 정확 저장 스키마·완료 receipt·방4선택/자산6·서사/기술 대상은 C1-C 승인 표에서 고정한다.

## 10. C1-B0 한정 승인과 구현 인계 — Approved

실제 아스트라 `gpt-6-astra`는 §9.1~9.2의 첫 생산 묶음을 한정 승인한다. 승인 전임 원문은49677바이트/SHA `358CF33A73D254450EE471F3D101014577C4DD32A8B913D8502893BEBB9BADEB`이며, 루나의 입구/활성화 소유자 독립 재검토 기록은 `docs/verification/2026-10-03-chapter1-flow-luna-precheck.md` 현재 SHA `E294B2EFD10293DD68699DED4E8AE0F0C9B20F29198666DDA6F1BE53A0540F20` 안의 해당 절이다. P0/P1=0은 설계 한정이며 실제 첫 tick/교체 성공이 아니다. 이 절은 §9의 후보·승인 전 문구를 **B0 정확 범위에서만** 대체한다. B1/선택/서사/저장/CH1 종료는 여전히 Review다.

활성화 순서는 독립 검토와 실제 코드에 따라 정확히 다음과 같이 해석한다. 비활성 staging에서 **설정과 seed 저장만** 수행한다. 모든 실제 producer 등록을 활성화 전에 마친다고 해석하지 않는다. 활성화 뒤 기존 Awake가 seeded motor/session과 threat/bridge 피해 lane을 만들고, 기존 InputRouter.Start가 실제 ValidateCoordinatedInputRegistration 전부→RegisterCoordinatedInputOwner 전부를 수행한다. 첫 FixedUpdate 전 이 과정이 끝나야 한다. 기존 메서드명이 존재하는 경우 그대로 재사용하며 §9 표의 `RegisterCoordinatedInputRegistration`은 이 실제 `RegisterCoordinatedInputOwner` 경계로 구현한다. 기존 Awake를 수동 호출하거나 실행 순서를 reflection으로 강제하지 않는다. 생성 getter 선호출/등록 누락/firstTick 불일치는 실행 실패이며 첫 view로 덮지 않는다.

구현자는 별도 테라 `gpt-6.1-sol`다. C1-A 프로젝트를 Unity/빌드/시험이 사용하는 동안 제품 파일을 쓰지 않는다. Root가 해당 소유 실행 종료와 C1-A 현재 소스 원장을 인계하면, 그 원장의 실제 지문을 B0 변경 전 기준으로 기록하고 §9의 정확 파일만 구현한다. C1-A 최초 입력398이나 이전 frozen 원장을 갱신하지 않는다. B0 추가 입력이 기존 격리본에 없으면 승인 목록의 해당 원본 파일·메타만 실제 길이/SHA를 기록해 추가 복제하며 파일 목록을 넓히지 않는다. 새 파일의 해당 부모 meta와 기존 지정 시험 파일/meta 외 와일드카드 권한은 없다. 기존 asmdef/friend 변경은 B0에 없다.

이 승인에는 regular prefab의 두 적/전체 producer 구조를 보존한 별도 저작기·validator·prefab, lifetime owner와 등록/witness, 일반 입력 및 초기 tick/HP 승계와 §9의 집중 시험이 포함된다. B0 실제 교체는 regular→regular이며 보스 승계는 초기 등록 검증까지다. RunFailure AllKnown 입력·실제 방4/풀6·신규 killplane/crush·프로필 파일·Barrier·기술2·챕터 완료를 추가하지 않는다. 이들은 삭제된 목표가 아니라 실제 생산 근거를 고정하는 다음 B1/C 승인 대상이다.

소스 독립 검토 후 §9.2의 compile15분/집중10분/필수 영향 회귀30분 각 단회를 적용한다. 누락 fixture는 정확 파일을 조사해 채택하고 시험을 생략하지 않는다. 실패는 원시·원인·수정·새 검사 목적을 기록하며 자동 동일 재실행하지 않는다. 현재 승인은 구현 인계 가능 판정이며 첫 tick·HP 승계·입력 원자성·그래프 교체·전체 챕터 플레이의 실제 수용은 결과 검수 후에만 한다. C1-A 사용자 체험 빌드 준비를 B0 완료에 새로 종속시키지 않는다. 원본/Q0/키/원격 게시·운영 승인에는 영향이 없다.

### 10.1 B0 별도 산출 경로 보정 — Approved

실제 아스트라는 B0 구현을 C1-A 시험과 분리하기 위해 §9/10의 후속 쓰기 경로만 `output/chapter1-playable-r1/project/`로 변경 승인한다. 증거는 같은 루트 `evidence/`, 준비 원장은 `preparation/`, 향후 승인 빌드는 `build/`다. C1-A 원본/격리 project/실행 파일을 B0 구현자가 수정하지 않는다. 기존 CH1 계약 승인708697 및 C1-A 각 입력·제품·보정 원장의 이력을 보존한다. B1의 Review 상태나 B0 제품 파일 허용 목록은 늘어나지 않는다.

Root는 실제 C1-A 쓰기가 없는 한 시점의 입력/구현/보정 원장을 합친 정확 경로·길이·SHA 목록을 고정한다. 최근 제품 보정과 shader/test 보충 입력을 포함한 실제 존재 원문만 채택하고 현재 작성 중이거나 원장에 없는 파일을 탐색 복제하지 않는다. 테라는 그 고정 목록과 실제 source를 대조해 새 경로에 최초 복제하고 모든 대상 readback 및 원본 재대조가 같을 때만 B0 편집을 시작한다. source가 중간에 바뀌면 해당 checkpoint 복제를 실패로 기록하고 새 고정 원장을 받는다. C1-A 시험이 진행 중이어도 선택된 source가 실제 불변이면 읽기는 가능하다.

복제 대상은 허용 Assets/Packages/ProjectSettings의 고정 파일뿐이다. Library/Temp/Logs/obj/.git/빌드·사용자 저장·기존 evidence·키는 복제하지 않는다. 후속 준비 원장에는 전임 C1-A 원문 지문을 링크하고 역사 증거를 새 실행으로 복제 집계하지 않는다. 메타 GUID와 설정은 그대로 유지한다. B0에 새로 필요한 입력은 §10의 기존 정확 추가 입력 조건을 따른다.

이 새 격리 project에서 B0 소스 구현은 고정 checkpoint 대조가 끝나면 즉시 가능하다. C1-A 최종 검수 완료를 구현의 새 선행으로 삼지 않는다. B0 Unity 실행은 해당 소스/Luna 관문과 기존 유한 상한을 유지하고, C1-A와 동시 Unity 자원 사용은 Root가 분리한다. 마지막 C1-A 보정이 이후 생기면 변경 파일과 의미 차이를 독립 대조해 후속 project에 선택 반영하며 폴더 덮어쓰기로 통합하지 않는다. 최종 CH1은 한 프로젝트·한 현재 빌드의 근거로 수용하고 서로 다른 checkpoint의 성공을 합쳐 수용하지 않는다.

### 10.2 B0 정규 진입 호스트 한정 보정 — Approved

루나 실제 `output/chapter1-playable-r1/evidence/luna-source-precheck.v1.json`은 owner의 정적 수명주기는 확인했으나 builder가 graph만 저장해 실제 initial prefab 결속 입구가 없다는 P1을 기록했다. 실제 아스트라는 다음 보정만 승인한다. 기존 구현 원장 `implementation-files.v1.json` SHA `EDC3F23E88BD94B2371180D38928E1B00AFB9CA9EDDDA0C14651D64708A538A1`과 검수 원문은 보존하고 변경 후 원장을 별도로 남긴다.

추가 파일은 격리 B0 project의 `Assets/Prefabs/Chapter1/Chapter1RegularEncounterHost.prefab`과 해당 `.meta` 두 개뿐이다. 이미 승인한 owner 파일에 `ConfigureInitialPrefabForAuthoring(GameObject inactiveRegularPrefab)`와 읽기 전용 `InitialPrefabForAuthoring`을 추가한다. pristine 비활성 host에서 한 번만 설정하고 활성화/발급 뒤 재설정은 거절한다. 입력 prefab은 inactive·regular router1·owner0 및 기존 validator 구조를 충족해야 한다. builder는 먼저 graph를 저장한 뒤 비활성 host에 owner 하나를 붙여 정확 graph 자산 참조를 설정하고, runtime 시작 시 활성화되도록 host prefab을 저장한다. owner는 graph 밖이며 host 자식에 graph를 미리 생성하지 않는다.

기존 builder/validator 파일에만 host 생성과 `ValidateHost(GameObject host)`를 추가한다. 검사는 owner1·내장 graph/producer0·초기 prefab의 정확 자산 경로·비활성 graph·직렬화 참조 일치를 요구한다. 기존 graph/host 경로가 이미 있으면 자동 덮어쓰지 않는다. 새 C#/scene/asmdef/friend·기존 원본 변경은 없다.

REQ-C1B-ENTRY-001/AC-C1B-ENTRY-001의 정상 시험은 기존 승인 `Chapter1EntryPlayModeTests`에서 저작 host Instantiate→실제 Start→첫 완료 view로 시작한다. 테스트가 owner를 직접 생성하거나 Begin을 호출한 것은 정상 저작 입구의 대체 증거가 아니다. 후속 실제 RequestRegularReplacement→Destroy/actions 정리→generation2를 같은 host로 확인한다. 합성 원자성 시험의 직접 Begin은 제한된 setup으로 별도 표시한다. 정상 host1/잘못된 prefab/중복 설정·중첩 owner 거절을 기존 승인 시험에 추가한다. 한정 구현은 즉시 가능하며 변경 원문 Luna 대조 뒤 기존 compile/집중 단회 상한을 따른다. 실제 실행·첫 tick 성공·B1/CH1 수용은 아직 주장하지 않는다.

## 11. C1-B1 출정·네 방·실패 복귀 연결 — Review

사용자 동작은 거점의 출정→승인 계획의 네 방 이동/일반 전투→오르단 진입, 또는 실제 실패→거점 복귀→새 출정이다. 구현은 테라, 독립 검수는 루나, 승인·통합은 아스트라다. 실제 선행은 B0 입력/수명주기의 정상 증거와 아래 저작 데이터·회귀 입력의 한정 채택이다. 이 절은 아직 구현 승인이 아니며 C1-A/B0 파일을 조사 중 덮지 않는다. 새 관리 문서나 범용 방 프레임워크를 만들지 않는다.

### 11.1 기존 규범과 새 저작 결정의 구별

Approved VD-04의 원문 snapshot 네 개는 실제 `docs/verification/room-plans/101.json`, `202.json`, `303.json`, `404.json`에 존재한다. 순서는 각각 R01/R02/R04/R06, R01/R02/R05/R06, R01/R03/R04/R06, R01/R03/R05/R06이다. 파일의 canonical UTF-8+LF 원문과 SHA256SUMS를 그대로 입력 채택하고 재직렬화하지 않는다. 자동 제안은101→303→202→404, 실제 원자 저장 성공 뒤 RunStarted, 명시 검증 시드는 lastOfferedSeed를 바꾸지 않는다. 계획이 존재한다는 사실은 방 자산/수동 이동 검증이 있다는 뜻이 아니다.

VD-05는 체력0·낙하 한계 통과·Lethal 압착만 실패 원인으로 승인했으며 이 원문에는 보편적인 killY나 압착 거리/시간 수치가 없다. 다음 수치는 **B1 승인 때 채택할 새 저작 규칙**이며 기존 승인 수치라고 소급하지 않는다. 방마다 입구를 로컬(0,0), 필수 바닥 중심 높이0으로 정하고 kill plane은 로컬 Y=-8로 제안한다. 판정은 해당 tick 완료 player 중심 Y가 -8보다 작을 때 true, 같으면 false다. X는 해당 방 경계 내외 모두 적용한다. 카메라/콜라이더 callback·프레임 위치로 판정하지 않는다. 봉쇄선의 비치명 입구 복귀에는 이 실패 판정기를 재사용하지 않는다.

Lethal 압착은 일반 overlap 모두가 아니다. 저작 데이터의 `Lethal=true`인 고정 위험 ID와 tick별 닫힌 압착 영역에 한정한다. B1 최소 저작안은 R05의 선택 경로에 한 개, 필수 경로는 이를 우회한다. 주기180틱에서 경고60틱/닫힘60틱/열림60틱, 닫힘 구간의 고정 로컬 AABB (12,0)~(14,2), 위험 단계 경계는 해당 graph 첫 tick을 기준으로 정수 나머지로 결정한다. 완료된 권위 player 몸체 AABB와 양축 모두 양의 길이로 겹칠 때 true, 경계 접촉만이면 false다. 움직이는 물리 판을 가장하지 않고 표시와 판정이 같은 닫힌 영역·tick을 소비하는 저작 압착 장치다. 수치와 위치는 새 승인 대상이며 임의 실행 중 조정하지 않는다.

### 11.2 정확 파일·자산과 단일 소유

격리 project의 신규 runtime 파일은 `Assets/AcadeGameMaker/Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs`, `Chapter1FailureSources.cs`, `Chapter1RunView.cs`, `Chapter1RoomCatalog.cs`, `Chapter1RoomInstance.cs`, `Chapter1DevelopmentProfile.cs`, `AcadeGameMaker.Run.Unity.asmdef`다. Input.Unity에는 `Chapter1HazardObservation.cs` 하나를 추가하고 기존 B0 `Chapter1EncounterEvidenceView.cs`/`Chapter1EncounterLifetimeOwner.cs`/`InputRouter.cs`를 좁게 확장한다. Run.Unity는 Run/Core/Input.Unity/Profile에만 의존한다. Run 및 Input.Unity AssemblyInfo의 Run.Unity friend 각1개, Hub Unity asmdef의 Run.Unity 참조와 Run.Unity의 Hub.Presentation.Unity·Combat.Authoring.Editor·새 Run Unity PlayMode 시험 assembly friend만 추가 후보로 고정한다. 원본 순수 실패/M5A 코어는 재작성하지 않는다.

새 화면은 `Assets/Scenes/Chapter1Playable.unity` 한 개다. graph 밖 거점/진행 UI와 lifetime/flow owner를 유지하며 방 graph만 교체한다. `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/Chapter1ExpeditionPresenter.cs`는 불변 RunView와 기존 UI semantic frame만 읽는다. 신규 저작 파일은 `Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs`, `Chapter1ExpeditionAuthoringValidator.cs`, 새 prefab은 `Assets/Prefabs/Chapter1/Hub.prefab`, `R01.prefab`부터 `R06.prefab`, `OrdanEncounter.prefab`이다. 각 meta와 새 부모 meta만 포함한다. 기존 C1-A scene/prefab을 덮지 않는다.

계획 원문은 `Assets/Chapter1/RoomPlans/101.json`부터 지정 네 개로만 복제한다. catalog의 각 방은 exact ID·socketPlan ID·입출구·camera bounds·필수 경로 점열·최대 gap/벽 높이·적/위험/보상 소켓과 적용된 변주를 직렬화한다. 같은 평면을 이름만 바꾼6개로 완료 처리하지 않는다. R01 입구 안내, R02 낮은 단차 전투, R03 우회 상승, R04 전이 선택 지름길, R05 압착 우회, R06 보스 앞 복합 전투의 서로 다른 저작 배치를 둔다. Walker/Surveyor 두 생산 역할과 기존 regular graph의 결속은 유지하고 스폰 숫자·소켓은 고정 catalog에 기록한다. 정확 좌표 목록/자산 지문은 저작 초안 결과를 승인 전에 채택하며 자동 생성/실행 중 RNG로 대체하지 않는다.

FlowOwner의 `RequestExpedition(int? explicitSeed)`는 현재 Hub·입력 중립·저장 정상에서만 받아 계획 검증/필요 lastOfferedSeed 저장→lifetime 최초 입구 발급→실제 첫 view→RunStarted 순서로 진행한다. `ObserveRoomExit(roomId,generation,tick)`는 실제 완료 player 위치가 해당 출구 영역에 있고 현재 방 적의 완료 snapshot상 생존0일 때만 한 번 확정한다. UI 클릭으로 방 완료를 만들지 않는다. RoomLeaving/실제 TransferCleared(RoomLeaving)→잠긴 입력 receipt→파기 증거→다음 graph의 실제 첫 view→RoomEntered 순서를 유지한다. 네 번째 방 뒤 보스 진입도 같은 생명주기를 사용한다.

Ordan graph의 mode requester는 기존 terminal requester 한 명으로 유지한다. FlowOwner는 별도 requester로 등록하지 않고 requester의 새 `ConfigureChapter1ProgressionForAuthoring` 및 source-bound 실패/진행 요청 입구를 통해 Transition·실제 clear 결과를 받는다. C1-A local opt-in과 CH1 진행 opt-in은 상호 배타적이고 기본false다. 성공 terminal triplet/M5A ObserveDeath는 동일 tick 실패 batch 확정 후에만 진행한다. 이때 RunActive/acceptedFailure/persistedPair는 실제 각 소유 snapshot을 사용하며 상수 대체하지 않는다.

### 11.3 실제 세 원인 닫힘과 실패 종료

`Chapter1HazardObservation`은 bound router/room 객체가 정확 완료 Movement tick에서 낙하와 압착을 각각 계산해 읽기 전용 publication을 만든다. 빈 위험 목록도 catalog 검사 완료 후 실제 루프를 끝냈다는 완료표식과 false를 게시한다. producer 누락·catalog 미검증·tick/graph 불일치는 미완료이며 false가 아니다. health publication은 같은 tick Combat 최종 player HP/IsDead에서만 만든다. 각 publication의 발행 소유 객체/token·원정키·세대·tick·revision을 `Chapter1FailureSources`가 대조한 뒤 관측된 원인으로 presence mask를 계산한다. 세 개 모두 있을 때만 RunFailureArbitrationSession.ObserveTick을 호출한다. 같은 tick 보스 사망은 이 실패 중재 뒤 M5A에 전달한다.

FlowOwner가 accepted intent를 한 번 보유하고 다음 유효 입력 tick에 Transition을 요청한다. 실제 잠금 receipt 뒤 TransferCleared(RunFailed)를 관측하고 `CommitFailedRun`을 한 번 게시한 다음 graph/임시 목록을 제거한다. 파기·actions 정리 완료→실제 보존 snapshot 저장 성공→ReturnedToHub→새 UI 입력 순서다. 저장 실패는 진단 화면에 남고 Returned나 새 출정이 아니다. 이전 원정의 키·방계획·적·예약·HP/위치/전이는 새 원정에 남지 않는다. 재출정은 새 원정 키, generation1, 초기 최대HP와 새 계획이며 기존 확정 pair/튜토리얼/설정은 보존한다.

`Chapter1DevelopmentProfile`은 §9.3의 고정 개발 root만 사용한다. 기존 Profile 공개 load/validation/atomic save API를 그대로 소비하고 API 접근이 부족하면 원문 근거와 정확 friend만 먼저 검토한다. 삭제/새게임 reset으로 오류를 피하지 않는다. 실제 일반 사용자 profile 경로를 읽거나 덮지 않는다. 앱 재시작은 거점으로만 복구하며 활성 원정을 이어 하지 않는다. B1 종료는 보스 뒤 유효 M5A handoff를 C1-C가 받을 수 있는 잠긴 상태까지며 가짜 선택/성공/챕터완료를 게시하지 않는다.

### 11.4 검증·한정 종료 조건

REQ-C1B-ROOM-001/AC-C1B-ROOM-001: 기존 AC-ROOM-001~006에 더해 `Chapter1RoomPlanTests.FixedPlansAndSeedRotation`, `RejectUnvalidatedRoom`, `RoomExitRequiresActualCompletion`로 네 원문/저장 실패/순서/잘못된 소켓을 확인한다. 자동 구조 검사 뒤 각 방 기본 이동 검증에 통과해야만 정규 풀에 편입한다. 미검증 후보를 볼 수 있는 저작 검수 모드와 실제 출정 가능 목록은 구별한다. 네 snapshot×3 수동 이동12회는 유지하며 자동시험으로 대체하지 않는다.

REQ-C1B-FAIL-001/AC-C1B-FAIL-001: `Chapter1FailureSourceTests.HealthFromCompletedCombat`, `KillPlaneBoundary`, `LethalCrushPhaseAndOverlap`, `RejectMissingOrForeignPublication`, `SameTickFailurePrecedesBossDeath`로 실제 원인·Y=-8/직하·닫힘60/120틱 경계·AABB접촉/겹침·AllKnown 위조를 검사한다. 실제 runtime 소유 publication을 사용하고 합성 unit 입력만으로 생산 연결을 수용하지 않는다.

REQ-C1B-RETURN-001/AC-C1B-RETURN-001: `Chapter1ExpeditionPlayModeTests.FailureCommitBeforeDestroy`, `SingleReturnForConcurrentCauses`, `FailedSaveBlocksReturn`, `RestartDoesNotResumeRun`, `FourRoomsThenBossWithCarriedHealth`로 실패3원인 각 경로와 보존/초기화·중복·실제 graph 교체를 확인한다. 시험은 신규 `Tests/PlayMode/RunUnity/` assembly와 위 파일 두 개, 순수 계획/경계 시험은 `Tests/EditMode/Run/`의 위 클래스 파일과 meta를 사용한다. 정확 asmdef 참조는 Run.Unity/Input.Unity/Core 및 기존 시험 지원에만 한정한다.

B1 승인 후 compile1회15분→원인/저장/계획 집중1회15분→필수 Movement/Transfer/Combat/Input/M5A/M5D2/Profile/B0 회귀1회30분→실제 저작 경로 검수 및 수동12회→현소스빌드1회15분 순이다. 실제 플레이는 한 회30분 상한으로 수행자·시드·실패 원인·재출정을 기록한다. 수동12회는 각각 별도 기록이고 미완료를 합격 처리하지 않는다. 검사 선택 ID·입력파일 및 저작 좌표 원문 채택, Luna 사전검토가 B1 Approved 전 남는다. 이 제한은 C1-A/B0 승인 범위를 바꾸지 않는다.

### 11.5 여섯 방의 정확 저작 초안과 입력 폐쇄

아래는 새 B1 저작 좌표의 Review 후보이며 실물 도달성 증거가 아니다. 좌표는 각 방 로컬 world unit, 직사각형 표기는 `(xmin,ymin,xmax,ymax)`다. 입구 포트의 바닥 기준은(0,0), 실제 player 중심 시작은(0,0.81)로 구별한다. 출구 영역은(30,0,32,3), 출구 연결의 다음 방 player 시작은 해당 방 입구다. 좌우 bound는(-4,-10,-3,14)/(35,-10,36,14), camera용 방 영역은(-4,-10,36,14)이며 기존 camera 소유자가 실제 viewport 크기를 고려한 center bounds를 검증한다. 필수 floor 기본재료/충돌층은 승인 regular 환경을 재사용한다.

| ID | 필수 바닥 AABB 목록 | 추가 고정 지형 AABB | 최대 필수 gap/벽 높이 | 기본 Walker/Surveyor 소켓 중심 |
|---|---|---|---|---|
| R01 |(-3,-1,35,0)|없음|0/0|(8,0.81)/(20,0.81)|
| R02 |(-3,-1,35,0)|(10,0,13,1);(21,0,24,1)|0/1|(6,0.81)/(27,0.81)|
| R03 |(-3,-1,35,0)|(8,0,12,1);(12,0,16,2);(16,0,20,1)|0/1|(5,0.81)/(25,0.81)|
| R04 |(-3,-1,12,0);(14,-1,35,0)|(18,1,22,2) 선택 발판|2/0|(7,0.81)/(27,0.81)|
| R05 |(-3,-1,35,0)|(10,2,16,3) 압착 위 우회 발판|0/0|(6,0.81)/(26,0.81)|
| R06 |(-3,-1,10,0);(12,-1,35,0)|(20,0,23,1)|2/1|(5,0.81)/(27,0.81)|

R05는 바닥 경로에서 열린 구간을 기다려 통과하거나 기본 점프로 고정 발판을 넘어간다. 필수 압착 통과를 기술/전이 사용에 의존시키지 않는다. 압착 표시 영역은 §11.1과 정확히 같고 위 발판은 피해 영역 밖이다. R04 선택 발판은 필수 경로에서 제외한다. 죽은 적을 지형 다리로 사용해야만 도달하는 배치를 만들지 않는다. 실제 capsule/collision 결과가 표의 기본 이동 도달성을 만족하지 않으면 좌표 수정안과 실제 검사 후 채택하며 현재 표를 통과 증거로 쓰지 않는다.

socketPlan A는 위 표의 적 두 개를 사용한다. B는 R02/R03/R04/R05에만 존재하며 Walker X를 기본값+1, Surveyor X를 기본값-1로 바꾸고 Y는 해당 기본 바닥의0.81을 유지한다. R01/R06은 A만 존재한다. B1 일반 방에는 상자를 추가하지 않는다. 기존 surveyor/walker 전이 등록2와 player를 포함한 전투 등록3, Walker/Surveyor prefab의 수치·등록역할을 유지한다. 보상 소켓은 각 방(29,0) 하나를 메타에만 선언하고 B1에서는 지급0으로 명시한다. 원정 자원/보상 기능을 가짜 지급하지 않는다.

필수 경로 점열은 각 바닥 중심 경로의 x=0→6→10→14→18→24→31이며 해당 구간 지형 상단을 따라간다. gap 구간은 양끝 이륙/착지, 단차는 각각 상단 waypoint를 포함하도록 validator가 표에서 결정적으로 전개한다. 허용 변주는 지정 A/B 적 좌표뿐이며 지형/적유형/위험 주기 변경은 없다. 정수 격자 snapshot에는 기존 네 JSON 원문을 사용하고 부동소수 지형 메타를 그 canonical 원문에 추가하지 않는다.

의존 입력은 기존 B0 regular 전체 producer·Walker/Surveyor/player·camera·입력·기존 Ordan graph 및 M5A/M5D2/Profile runtime으로 한정 조사한다. 공개 Profile save 결과는 사용 가능하지만 내부 canonical 생성/검증·복구 입구는 현재 Profile friend에 Run.Unity가 없으므로 실제 사용하는 정확 API 목록을 구현 전 대조한다. 필요하다면 Profile AssemblyInfo에 Run.Unity 한 friend만 추가 승인 후보로 제출하며 reflection/공개화 전체 변경은 없다. C1-A/B0 현재 원장과 이 추가 원본의 정확 차집합·raw SHA/길이를 Root가 고정하고 Luna가 확인하는 방식으로 입력을 닫는다. Assets 전체/Library 복제는 없다.

검사 입력은 기존 Run EditMode M5A/M5D2와 Profile 저장/시작복구·입력 등록 시험의 실제 파일/asmdef/meta를 원장으로 한정하고, 위 신규3클래스의 생산 publication 검사를 추가한다. 시험 assembly 이름은 `AcadeGameMaker.Run.Unity.PlayMode.Tests`로 고정하며 새 Run.Unity·Input.Unity의 해당 friend만 필요 범위로 검토한다. 위험 publisher의 양성은 실제 완료 Movement/Combat에서 계산하며 테스트용 true를 제품에서 주입하지 않는다. 방별 자동 구조 검사→후보 한 방 정상 수동 이동→방6 풀 입장→고정계획4×3 검사의 순서를 유지한다. B1 Approved 전 잔여는 이 좌표·Profile 접근의 독립 검토와 정확 추가 입력/시험 원장 채택이며 새 전사 문서 체계가 아니다.

### 11.6 실제 의존 원장·공개 저장 입구·검사 조립체 고정 — Review

테라의 `output/chapter1-playable-r1/evidence/b1-dependency-preparation-terra61.v1.json` SHA `F18BBD8BA000394B1FEDD7E6EA2C1072954DDE2F516FC221249D0E95402756CE`를 이 후보의 정확 입력 목록으로 채택한다. 아스트라는 files95행의 실제 원본 길이/SHA를 전수 재계산하여 불일치0, targetExists=true46행의 실제 격리본 SHA 불일치0을 확인했다. Run/Profile40파일+폴더메타2 외에도 기존 참조4행이 포함된 총46행이며 재사용 범위를42로 잘못 축소하지 않는다. 추가49개는 원장 additionalInputs의 정확 시험44개와 계획 원문5개다. 원장은 작성 당시 계약 BB544 지문을 보존하며 이번 문구 추가로 재작성하지 않는다. 아직 복제나 제품 실행 승인 기록은 아니다.

B1 Approved 시 시험44개는 같은 상대경로에만 복제하고, 계획4개는 §11.2의 `Assets/Chapter1/RoomPlans/` 아래 같은 파일명·동일 원문 바이트로 배치한다. SHA256SUMS는 `preparation/room-plans/SHA256SUMS` 증거 입력으로만 복제하며 Unity에 임의 확장자를 붙여 읽히게 하지 않는다. 새 계획 asset meta와 새 부모 meta는 별도 생성 원장에 기록한다. 기존446 복제 원장·B0 제품 변경·C1-A 추가 보정은 서로 다른 이력이고 단순 합계로 새 고유 입력 수를 선언하지 않는다.

정확 저장 입구는 공개 `ProfileSnapshotV1` 생성/Validate→`ProfileCanonicalEncoder.Encode`→`ProfileCanonicalDecoderV1.Decode`의 문서/Validate, 기존 공개 progression Validate/TryGetCommittedPair다. 시작은 고정 개발 root에 `ProfileLaunchObservationAdapterV1.Observe`→batch.Validate→`ProfileLoadSelectorV1.Select`→plan.Validate/InMemoryDocument를 사용한다. 필요한 복구·초기 저장은 공개 recovery 계획과 공개 `ProfileAtomicSaveServiceV1.Save(directory,document)`를 소비한다. 결과 Validate/Outcome/CommittedRevision 및 계획의 기대 revision이 일치한 CommittedFirst/CommittedReplacement만 진행시킨다. RequiresAtomicSave 계획의 저장을 생략하지 않고 불확실 결과는 실제 재관측·검증 전 진행하지 않는다. 이 공개 입구로 충분하므로 **Profile AssemblyInfo 수정·Run.Unity friend 추가는0**으로 고정하고 앞선 선택적 friend 후보를 철회한다. 내부 UnderHeldRoot/fault port 호출·새 JSON 스키마·보존필드 임의 삭제는 없다.

새 production asmdef 정확 경로는 `Assets/AcadeGameMaker/Runtime/Run/Unity/AcadeGameMaker.Run.Unity.asmdef`이며 이름 `AcadeGameMaker.Run.Unity`, references는 `AcadeGameMaker.Run`, `AcadeGameMaker.Core`, `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Profile` 네 개다. noEngineReferences=false, unsafe/overrideReferences=false이며 Editor 한정이 아니다. 이 assembly의 내부 접근을 위한 신규 `Run/Unity/AssemblyInfo.cs`는 `AcadeGameMaker.Hub.Presentation.Unity`, `AcadeGameMaker.Combat.Authoring.Editor`, `AcadeGameMaker.Run.Unity.PlayMode.Tests` 정확3 friend만 선언한다. 기존 Run/AssemblyInfo와 Input/Unity/AssemblyInfo에 production Run.Unity 각1을 추가하고 Input/Unity에 새 시험 friend1을 추가한다. Hub Unity 및 Combat.Authoring.Editor asmdef에 Run.Unity 참조 각1을 추가한다. 이외 assembly/ref/friend 확대는 없다.

신규 검사 경로는 `Assets/AcadeGameMaker/Tests/PlayMode/RunUnity/` 아래 `AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef`, `Chapter1RoomPlanTests.cs`, `Chapter1FailureSourceTests.cs`, `Chapter1ExpeditionPlayModeTests.cs`와 각 meta 및 RunUnity 폴더 meta다. 이는 §11.4의 새 순수검사 EditMode 배치 후보보다 우선하며 세 신규 클래스를 한 조립체에 둔다. 이름은 `AcadeGameMaker.Run.Unity.PlayMode.Tests`, references는 Run.Unity/Input.Unity/Core/Profile 네 개, optionalUnityReferences TestAssemblies, autoReferenced=false, unsafe/overrideReferences=false, include/excludePlatforms 빈 목록이다. 기존 순수 Run 시험 assembly를 확장하지 않는다. production 객체 조립은 승인 저작물/공개 internal authoring과 기존 friend로만 하며 private reflection을 쓰지 않는다.

선택은 원장 selectedTests의 기존20파일에 있는 실제 시험 전체와 §11.4 신규3클래스의 지정 메서드 전부다. DesktopProfileLaunchAdapterV1Tests의 RouterTests.CreateRig/DestroyAll 의존으로 기존 InputRouterPlayModeTests도 선택한다. 기존 Profile 시험의 메모리/임시 디렉터리 fault fixture는 합성 실패 검사이며 실제 사용자 profile 오류를 유도하지 않는다. 새 production publication 검사는 동일 graph의 실제 Movement/Combat 완료 및 저작 hazard 결과를 받아 health/killplane/crush 각각의 양성·음성·미완료·다른source/tick 거절을 확인한다. InputSystem.TestFramework는 고정된 기존 패키지 그대로다. 발견된 시험 ID/수는 실행 시 기록하며 임의 기대 개수를 만들지 않는다.

이로써 추가 파일/저장 공개 API/검사 선택 후보는 고정됐다. 다음 Luna 독립 검수는 이49 입력·공개 API·새 참조 순환·세 publication 검사와 §11.5 좌표/포트/위험의 정합성을 확인한다. B1의 실제 Approved 전환은 그 결과를 읽어 별도 명시한다. B0/C1-A 승인과 현재 실행은 중단시키지 않으며 이 문단은 추가 제품 코드·Unity·저장·원격 실행을 하지 않는다.

### 11.7 실제 거점 입력과 종단 수용 보정 — Review

독립 검수 `luna-b1-contract-review.v1.json` SHA `242C5CA5DC890CD2648A54D3A1415585DD36778BCF5E3BC9B208B0F6CCC9EF75`의 출정 UI P0와 종단 AC P1을 다음처럼 보완한다. 이어 전임 계약 `DFBC24B1BEB49AF2A95A2811128DB40AD3F815FC98DF28C697A08824DEF58AA7`을 검토한 v2 원문 SHA `E9C66D58EF7CFD61B88575E54802B36E78CE681A94AB15CD7C50258987539A84`의 어셈블리 방향 P1은 아래 같은 파일의 요청 port로 정정한다. 이전 검수 원문은 보존한다. 내부 FlowOwner 호출 성공만으로 사용자의 출정·복귀 성공을 대체하지 않는다.

**REQ-C1B-HUB-001:** 거점은 실제 Hub UIOnly InputRouter와 DesktopProfileLaunchAdapterV1의 기존 prepared lifecycle을 사용한다. ConfigureHubUiOnlyForAuthoring만 호출하면 Start에서 닫히므로 이를 초기화 완료라고 하지 않는다. 정확 추가 후보 파일은 `Input/Unity/Chapter1HubLifetimeOwner.cs`와 meta, 좁은 수정 파일은 기존 `Input/Unity/DesktopProfileLaunchAdapterV1.cs`다. 후자에 `ConfigureChapter1DevelopmentRootForAuthoring(InputRouter router,string exactRoot)` 한 입구만 추가하여 pristine 비활성 상태에서 고정 개발 root를 결속하고 기존 실제 production preparation/Observe/Save 경로를 그대로 사용한다. ForTests나 모의 preparation을 제품에서 호출하지 않는다. 기본 desktop 경로와 reset 권위는 변경하지 않는다. exactRoot는 §9.3 경로를 정규화한 값만 허용하며 상대경로·다른root·링크·사용자 profile fallback은 거절한다. Run.Unity의 friend/참조 방향은 §11.6 그대로다.

HubLifetimeOwner는 같은 `Chapter1HubLifetimeOwner.cs` 안의 내부 `IChapter1HubRequestPort`에 pristine 비활성 상태에서 `BindRequestPort(IChapter1HubRequestPort endpoint)`로 한 번 결속된다. Run.Unity의 Chapter1PlayableFlowOwner가 이 interface를 구현하고 외부 저작 소유자가 주입한다. Input.Unity는 이 interface만 알고 Run.Unity의 타입을 참조하지 않는다. 새 assembly 참조/friend/별도 callback 소유자는 없다. 비활성 Hub prefab 구성→router/adapter authoring→활성화→실제 준비 receipt와 UIOnly receipt를 확인한다. 출정 때 기존 hub의 actions/callback 실제 닫힘과 파기를 확인한 뒤 regular graph를 활성화한다. 다시 돌아올 때 old hub/router를 재활성화하지 않고 같은 prefab의 새 instance를 만든다. gameplay graph와 hub의 입력 actions가 동시에 활성화되지 않는다. UIOnly 초기 진입은 현재 held Submit/Click/Navigate가 중립으로 돌아온 완성 입력 프레임까지 출정 버튼을 비활성화한다. mouse pointer 위치 자체는 중립0을 요구하지 않는다. C1-A의 승인 neutral 의미를 재사용하고 새 InputSystem 직접 콜백을 추가하지 않는다.

Chapter1ExpeditionPresenter가 표시하는 정확 intent ID는 `chapter1.start-expedition`와 `chapter1.return-to-hub`다. 시작 화면 기본 focus는 출정 하나이며 버튼 hit rect는 기존640×360 safe frame에서 최소24×24, 클릭 좌표는 기존 actual-pixel→safe-frame 변환을 사용한다. 기존 UI Submit의 키보드/게임패드 경로와 UI Click의 포인터 경로를 모두 받는다. 같은 의미 frame에서 click과 submit이 함께 오면 intent1개, cursor로 frameOrdinal/modeEpoch/tick을 한 번만 소비한다. presenter는 실제 UI frame과 선택 intent를 HubLifetimeOwner의 `SubmitUiIntent(frame,intent)`에 전달한다. owner는 자신의 실제 router/준비 receipt·neutral·현재 frame과 대조한 뒤 아래 token을 발급해 port의 `HandleHubRequest(Chapter1HubLifetimeOwner source,Chapter1HubRequestToken token)`만 호출한다. FlowOwner 내부의 실제 RequestExpedition 호출은 Run.Unity endpoint의 검증을 통과한 뒤에만 일어난다. bool나 문자열만으로 외부 호출을 통과시키지 않는다.

같은 파일의 내부 sealed 불변 `Chapter1HubRequestToken`은 발급 Hub owner 객체, 실제 InputRouter 객체, endpoint 객체, hub generation, 양수 request ordinal, 실제 준비 receipt 식별값, UI receipt의 tick/frameOrdinal/modeEpoch/mode, 정확 intent 및 nullable 검증 seed를 가진다. 값 복사만으로 권한을 만들지 않는다. owner는 실제 발급한 token 객체를 비공개 pending 슬롯에 보관하고 `ValidatePendingRequest(endpoint,token)`으로 객체 동일성·source·현재 lifecycle·실제 frame/준비 증거를 재대조한다. endpoint는 자신이 결속한 source/활성 hub generation·예상 phase·마지막 소비 ordinal과 이 검증을 모두 확인한다. token을 endpoint가 한 번 소비하도록 owner의 `ConsumePendingRequest(endpoint,token)`을 호출하고, 중복/재진입 방지 상태를 먼저 고정한 뒤 저장/출정 의도를 처리한다. 저장 실패도 같은 token을 재사용하지 않으며 새 사용자 edge만 새 token을 만든다. 이 token은 입력이나 RunStarted의 별도 receipt가 아니며 신규 callback을 등록하지 않는다. 정상 입력 receipt의 소유자는 기존 InputRouter 그대로다.

거절 검사는 기존 `Chapter1ExpeditionPlayModeTests`에 `RejectForeignHubEndpointOrRouter`, `RejectCopiedStaleOrConsumedHubToken`, `RejectHubRequestInWrongPhase`, `RejectReentrantRequestBeforePersistence`, `CannotRebindActiveHubPort`를 추가한다. 값은 같지만 미발급 객체, 다른 endpoint/owner/router·세대·receipt, pending 제거 후 재전송, 저장 실패 후 같은 token 재전송 모두 실제 저장/RunStarted 호출0을 확인한다. Run.Unity→Input.Unity만 참조하고 반대 참조가 없는 asmdef 검사도 함께 한다. 제품에 테스트용 token 발급 우회를 추가하지 않는다.

출정은 요청 수락 뒤 버튼 잠금→lastOfferedSeed의 실제 저장 Outcome/revision 검증→hub 입력 닫힘→첫 graph view→RunStarted 순서다. 저장 실패는 `저장에 실패하여 출정하지 못했습니다`를 표시하며 RunStarted0, 새 seed 진행0이다. 새로운 neutral 이후 사용자가 새 edge로 다시 요청할 수 있지만 자동 재시도하지 않는다. 입력이 잠긴 동안 이전 제출을 큐에 쌓지 않는다.

실패 처리는 §11.3의 자동 권위 순서를 유지한다. 실패 commit→graph 파기→보존 저장 성공→ReturnedToHub가 끝나면 새 거점 UI에서 실패 이유와 `출정`을 표시한다. 정리 중의 `거점으로` 버튼은 진행 상태 확인용 잠금이며 실제 파기/저장을 앞당기지 못한다. 정리/저장 완료 후 결과 확인 화면의 `chapter1.return-to-hub` intent를 Submit 또는 Click으로 한 번 받아 거점 기본 화면을 표시할 수 있지만 ReturnedToHub를 다시 발행하지 않는다. 실패 후 누른 채인 키는 neutral 전까지 결과 확인/즉시 재출정을 일으키지 않는다. 저장 실패 상태에는 성공 복귀 버튼을 활성화하지 않는다.

**AC-C1B-HUB-001:** 실제 저작 Chapter1Playable scene에서 prepared Hub→실제 InputSystem 장치 이벤트→생산 callback→UI semantic frame→정확 intent→공개 개발 저장→RunStarted/첫 graph view를 관측한다. `Chapter1ExpeditionPlayModeTests.AuthoredHubSubmitStartsExpedition`, `AuthoredHubClickStartsExpedition`, `HeldSubmitDoesNotStartOrRestart`, `ClickAndSubmitProduceOneRequest`, `SaveFailureShowsErrorWithoutRunStarted`를 추가한다. InputSystem 테스트 장치 입력은 합성 장치라는 한계를 기록하지만 FlowOwner 직접 호출·frame 객체 직접 생성으로 UI AC를 통과시키지 않는다. 별도 독립 실제 창 키보드/포인터 플레이를 후속 수용 근거로 남긴다.

**REQ-C1B-E2E-001/AC-C1B-E2E-001:** `Chapter1ExpeditionPlayModeTests.AuthoredHubThroughFourRoomsToOrdan`은 고정 명시 seed101과 실제 검증 풀6에서 계획4를 선택한다. 같은 원정키 아래 각 방의 입력 receipt→Movement/Transfer/Combat 완료→적 최종 사망 snapshot→실제 player 출구 위치→RoomLeaving/clear→Destroy/dispose→후속 generation/RoomEntered를 순서대로 연결하고 최종 Ordan 첫 완료view와 승계HP를 확인한다. 테스트가 적HP setter/직접피해/출구 이벤트 호출/phase 완료 bool로 진행을 건너뛰면 종단 AC 증거가 아니다. 실제 게임 입력으로 이동·공격하며 동일 경로의 원문 계획SHA와 방문ID4를 기록한다. 제한시간 도달은 미완료이고 자동 방 건너뛰기를 하지 않는다.

**AC-C1B-E2E-002:** `Chapter1ExpeditionPlayModeTests.AuthoredFailureReturnsAndAllowsFreshStart`는 실제 저작 장면에서 세 원인 각각을 독립 실행한다. 적의 실제 공격으로 HP0, 실제 이동으로 killY 하회, R05의 실제 닫힌 Lethal 영역으로 진입하여 publication을 발생시킨다. 같은 원정키/세대/tick과 발행owner3/closed batch→accepted failure→Transition receipt→RunFailed clear→단일 commit→파기/actions종료→공개 Save의 기대 revision과 동일한 CommittedRevision→ReturnedToHub→새 UI neutral→사용자 새 Submit/Click→새 원정키를 한 trace로 검증한다. 세 원인 동시·보스 같은 tick 사망 우선순위는 기존 집중 생산 publication 시험도 함께 유지하며 정상 종단을 그 합성 setup으로 대체하지 않는다.

검증 메뉴는 VD-04의 명시 seed 요청을 실제 UI로 제공한다. 일반 출정과 구별되는 `chapter1.validation-seed-101/202/303/404` 네 고정 항목을 Navigate/Submit 또는 Click으로 선택하고 같은 frame/token 관문을 거쳐 RequestExpedition(선택값)을 호출한다. 이 경로는 lastOfferedSeed를 변경하지 않는다. 종단 정상은 검증101, 압착 실패는 R05가 있는 검증202를 실제 메뉴로 선택하며 테스트가 FlowOwner에 seed를 직접 주입하지 않는다. 기본 사용자 출정은 null 자동 순환 그대로다. 검증 항목을 런타임 임의 정수 입력/미등록 방 선택으로 확장하지 않는다.

**AC-C1B-E2E-003:** `Chapter1ExpeditionPlayModeTests.AuthoredReturnPreservesDevelopmentProfile`는 실제 고정 개발 root의 보존 snapshot 전후 동일필드와 새 저장 revision, 원정 중 임시 상태의 제거, 앱 재시작 후 Hub만 존재를 검증한다. 테스트용 개발 root의 원문은 실행 전후 보존하고 사용자 profile은 읽지 않는다. 실패/불확실 저장의 종단 음성은 `AuthoredSaveUncertainDoesNotPublishReturned`로 별도 실행하며 정상 실제디스크 trace와 fault fixture의 출처를 구별한다.

추가 파일은 기존 승인후보의 Chapter1ExpeditionPlayModeTests 안 메서드이며 새 시험조립체/패키지 없음. 종단4회(정상101 한 번, 실패원인3 각각 한 번)는 각30분·전체120분 상한, 마지막 실제 창 키보드/포인터 확인은 각30분 상한으로 단계 실행한다. 처음 실패한 경로는 원시 보존 후 원인/최소보정 관문으로 돌아가며 자동 반복하지 않는다. 이는 기존 compile/집중/회귀30분을 늘리는 것이 아니라 별도 사용자동작 종단 검증이며 수동 roomPlan4×3 이동요구도 대체하지 않는다. 종료 조건은 위 연속 trace·독립 실제 조작·개발저장 원문 결속이 모두 있을 때 B1 출정/실패복귀만 수용하는 것이다. Ordan 다음 선택·봉쇄선·CH1 완료는 C1-C까지 여전히 미수용이다.

## 12. C1-B1 한정 구현 승인·선택 통합 — Approved

실제 아스트라 `gpt-6-astra`는 전임 계약 SHA `03878A57212177563F83E01B83B03E6720EB2AA60511DEC3271A2A56BAF3DF66`과 루나 독립 v3 SHA `3A13D8233B3623AC0034AABDFDBB1EBEAEDEB7DF9FD002EC3EBD5C27DF01BC65`의 P0/P1=0 판정을 직접 읽어 §11.1~11.7의 **B1 정확 파일·좌표·생산 입력/실패·개발저장·검사**를 한정 승인한다. 이 절은 해당 §11의 Review/승인 전 문구보다 B1 범위에서 우선한다. C1-C 선택·기술·봉쇄선·서사·CH1 완료는 여전히 Review다. B1은 사용자가 출정하여 네 방과 보스에 도달하고 실제 실패 후 거점에서 재출정하는 연결이며 전체 CH1 완주 수용이 아니다.

### 준비·소스 착수와 실행 선행

별도 테라 `gpt-6.1-sol`가 고정 `output/chapter1-playable-r1/project/` 안에서 §11의 정확 목록만 구현한다. 준비 원장 F18B의49개 추가 입력을 지정 매핑으로 복제하고 길이/SHA/readback·메타 생성/기존 동일46 재사용을 별도 기록할 수 있다. 기존 원장과 원본은 불변이다. Profile friend0, Run.Unity와 Input.Unity의 단방향 참조 및 source-bound port를 유지한다. 지정 신규/수정 파일 외 파일이나 새로운 권한은 자동 추가하지 않는다.

B0 실행 성공은 B1 설계·준비 복제·충돌 없는 소스 작성의 선행이 아니다. 다만 같은 격리 project를 Unity가 읽는 동안 Assets/Packages/ProjectSettings를 쓰지 않는다. Root가 실행 종료와 B0 현재 변경 지문을 인계하면 B1 구현자는 그 지문을 기준으로 후속 수정한다. 실행 중에도 evidence/preparation의 읽기 목록 작성은 가능하며 제품 파일을 바꾼 것으로 기록하지 않는다. B0와 공유하는 InputRouter/lifetime/evidence·저작 연결은 한 구현자가 소유하며 B0 원문과 B1 변경을 단계별 원장으로 구별한다.

실제 B1 compile/저작·종단 실행 관문에는 B0 compile, 저작 host의 실제 Start→첫 완료 view, regular 교체의 파기/actions 완료·generation2 및 필수 입력/seed/HP 검사가 통과한 근거가 필요하다. B0가 실패하면 해당 연결의 원인을 먼저 고치며 B1 합성 성공으로 덮지 않는다. B1 소스 독립 검토와 §11.4/11.7의 유한 실행 상한·원시 보존·실패 단회 제한을 유지한다. 방 후보의 실제 기본이동 검수 전 정규풀 편입은 금지하며 실물 검수용 저작 장면으로 먼저 조사한다. 실행 수용은 같은 최종 소스·계획·저장·실제 사용자 입력 trace에 한정한다.

### C1-A 시험 한 파일 선택 통합 확정

제안 `c1a-input-test-integration-proposal.v1.json` SHA `05767CD4A0B6B609C4EA030C7795AEFC75E93B9C5139057D4E799921C19DBCDB`와 루나 조건부 검수 `luna-c1a-input-test-integration-review.v1.json` SHA `5C5557015D4A5FB2F04002FD7D8B0C41AD6FB02E61F9D2515B8E5A225F7F1CDB`를 채택한다. 아스트라는 실제 대상14612B/SHA `F590C37EE6838921F82FAF8E09D2BEFB0DAE40DDDBDAEFD5469BA35017C3BA23`, 제안16446B/SHA `91802FAEF7A7C74EBBBB30F279107D4E617B5700C83D2B807D9140628FD0F25D`를 재대조했다.

B0 project의 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/LocalPlayableSliceInputPlayModeTests.cs` 한 파일만 위 C1-A 제안 원문으로 선택 통합할 수 있다. 실제 교체 직전 before가 다르면 멈추고 충돌을 검토한다. 이전 원문/원장과 복사 후 지문은 별도 증거로 보존하며 meta/runtime/asmdef/asset은 이 작업에서 수정하지 않는다. 같은 project Unity 실행 중 교체하지 않는다. 과거 C1-A 위험2·회귀243 결과는 역사이며 B0 통합본의 실행 결과로 재집계하지 않는다.

C1-A Windows 빌드 성공과 설정 직렬화 델타·출력202 지문은 별도 근거다. 게임창 제어 승인 시간초과로 실제 GUI 플레이 관측은0이며 AC-LPS 실제 플레이/CH1 완료를 수용하지 않는다. 이번 B1 승인·시험 통합은 그 미검증을 해소했다고 주장하지 않으며 Q0·키·원격·사용자 일반 프로필 권한을 늘리지 않는다.

## 13. C1-C 실제 완주 연결의 한정 승인표 후보 — Review

이 절은 §12 승인 원문 SHA `E03515E572E64200F98770955369AF07B865D865E15D47C58BB68119361A26F2` 뒤의 후속 설계다. 솔 `gpt-6.1-sol`의 읽기 설계 원장 `output/chapter1-playable-r1/evidence/c1c-bounded-design-sol61.v1.json` SHA `6E23098CCC2D52FF072A174DB23AEA2448D11A6391B9485F6D447DF11CAB4FA7`을 아스트라가 읽었다. 그 원장의 미구현 API·신규 수치·저장 제안은 기존 승인이나 실제 성공 증거가 아니다. B1 구현의 추가 선행조건으로 삼지 않으며 C1-C 제품 쓰기 권한도 아직 부여하지 않는다.

### 13.1 완료 저장 범위의 최소 결정 후보

현재 사용자 목표인 실제 CH1 완주에는 기존 V1의 선택·기술·completedBranch 원자 저장과, 같은 실행에서의 별도 CH1 마지막 장면 완료 증거를 결속한다. 지속하는 `completedChapter` 필드는 이번 최소 C1-C 묶음에 추가하지 않는 안을 채택한다. §7/§9의 조건부 저장 형식 검토를 무조건적인 V2 이행 요구로 해석하지 않는다. `CompletedBranches`는 기존 DemoCompleted 분기 저장이며 CH1 완료로 이름을 바꾸지 않는다. 튜토리얼 ID나 별도 무검증 저장 파일에 챕터 완료를 숨기지 않는다.

`TryCommitChapterCompletion(demoEndReceipt,chapterClosingReceipt,profileCommitReceipt)`의 마지막 인자는 기존 성공 분기의 실제 원자 저장 결과다. 같은 FlowOwner/run/분기·기대 revision 및 실제 CommittedRevision이 결속되어야 한다. CH1 receipt는 그 실행의 실제 closing 완료 뒤 한 번 발급한다. 재시작은 저장된 pair/branch만 복원하며 그 정보만으로 closing을 재생 완료하거나 CH1 완료 receipt를 만들지 않는다. 현재 실행의 CH1 완주 증거와 재시작 후 지속 완료 기능을 구별한다. 별도 지속 완료 기능을 요구할 때만 V2 문서·복구·하향 호환의 정확 파일을 별도 승인한다. 이는 CH1의 실제 마지막 장면이나 양분기 완주를 생략하는 결정이 아니다.

### 13.2 구현 승인 전 고정할 생산 경계

다음 경로의 앞부분은 `Assets/AcadeGameMaker/Runtime/`다. 모든 신규 파일의 meta와 정확 시험 접근은 최종 차집합에 포함하고 B1 최종 원문을 인계받은 동일 제품 구현자가 수정한다.

| 묶음 | 정확 파일/API 후보와 소유 | 승인 전 닫을 검사·종료 조건 |
| --- | --- | --- |
| 선택 입력·원자 처리 | 기존 `Input/Unity/InputRouter.cs`, `Chapter1GameplayInputSink.cs`, `Chapter1EncounterEntry.cs`, `Chapter1EncounterEvidenceView.cs`; 신규 `Core/ChoiceSkillContracts.cs`, `Run/ChoiceSkillSession.cs`, `Input/Unity/Chapter1ChoicePhaseAdapter.cs` | 기존 frozen batch에 실제 ChoiceSkill edge/ordinal을 포함하고 전역 receipt를 마지막에 게시한다. 새 콜백/별도 입력 receipt 소유자0. 동일 객체 pending 권한·run/generation/tick·실제 저장 pair를 검증한다. Prepare/Validate 실패는 무소비, 부분 Commit 실패는 전역 미게시·정지다. |
| 전이·기술 단계 | 기존 `Transfer/TransferSession.cs`, `Transfer/Unity/TransferSimulationDriver.cs`; `PrepareRecallForChoiceSkill/ValidateRecallForChoiceSkill/RecallForChoiceSkill`, `BindChapter1ChoicePortForAuthoring` 후보 | 별도 -205 실행자를 만들지 않고 기존 -200의 단일 capture 안에서 lifecycle/removal→기술→일반 Press 순서다. §8.6 age20 실패/새 edge age21 성공·가시 대기, 회수 후21틱·압착300/연대420·동시 Transfer 성공 소비/실패 계속을 검사한다. 회수 전 예약/발판/checked 계산까지 검증한다. |
| 봉쇄선의 실물 결과 | 신규 `Combat/Unity/BarrierCombatSimulationDriver.cs`, `Chapter1ChoiceEnvironmentOwner.cs`; 기존 lifetime/evidence의 Barrier 분기 | -190 CombatSession의 DamageRequest만 사용한다. 압착36틱/중심4·주변2/경직60, 연대 발판150틱/길이1.5~6/두께0.25/hold60을 유지한다. 실제 대상/문 HP·주변 반경·좌표·충돌/낙하 복귀 입구는 다음 정확 저작표와 검수 전 구현하지 않는다. 통과 bool나 직접 HP 변경으로 대체하지 않는다. |
| 선택 UI·세령·CH1 마침 | 기존 `Run/Unity/Chapter1PlayableFlowOwner.cs`; 신규 `HubPresentation/Unity/Chapter1ChoicePresenter.cs`, `Chapter1ClosingPresenter.cs`, `Run/Unity/Chapter1CompletionReceipt.cs` | 기존 UI semantic frame/source-bound port 방식으로 한 의도를 받는다. M5A 정리→선택 저장→기술→실제 봉쇄선→Run 성공 commit·branch 저장→DemoCompleted→세령→closing→CH1 receipt 순서다. 정확 대사·재생완료 기준을 캐논 검수하고 CH2 장면/확정 동행/아인 상태를 추가하지 않는다. |
| 저작·검증 연결 | 기존 `Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs`와 Validator; 신규 `Assets/Prefabs/Chapter1/{Choice,Barrier,Closing}.prefab`, `Assets/Chapter1/Narrative/Chapter1Closing.json`; 기존 Chapter1Playable scene 차집합 | 기존 shader/font·소유 정리·입력 중립을 재사용한다. 두 선택의 같은 시작 조건에서 각각 한 번의 유효 기술로 실제 통과, 시간 차10초 이하와 양분기 마지막 장면을 검증한다. 메타/부모 폴더 및 B1 원문 차집합을 고정한다. |

참조 후보는 `Run.Unity → Input.Unity → Run → Core` 순방향뿐이다. Input.Unity의 Run 참조1, Run의 Input.Unity friend1, Core의 Input.Unity/Transfer.Unity friend2는 정확 타입 접근 검사 뒤 C1-C 승인표에서만 허용한다. Input.Unity→Run.Unity 역참조, Profile friend, 반사·범용 공개 상태 setter는0이다. 기존 B1 Hub port를 다른 receipt 소유자로 복제하지 않는다.

### 13.3 검증 후보와 다음 승인 관문

신규 시험 후보는 `Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs`, `Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs`, `Chapter1ClosingPersistencePlayModeTests.cs`와 각각 meta다. 새 시험 assembly는 만들지 않고 기존 정확 참조/친구로 접근 가능한지 먼저 확인한다. 각 시험은 REQ-CHOICE-001~007, AC-CHOICE-001~004, AC-WT-003/006, AC-UX-004 및 AC-CH1-003~007을 연결한다.

필수 관측은 실제 callback→동일 global receipt→기술·전이·전투 완료→실물 통과→성공 저장 Outcome/revision→DemoCompleted→closing→CH1 완료다. 저장 실패/불확실·다른 owner/복사 receipt/옛 세대/다른 분기/closing 미완료에서 CH1 완료0을 확인한다. 합성 fault/테스트 장치는 실제 창 양분기 조작을 대체하지 않는다. 재시작 검사는 V1 pair/branch 보존과 CH1 실행 receipt 미복원을 확인한다.

다음 승인에는 정확 봉쇄선 좌표/대상/수치, UI 의도 token과 전이 port의 발급 객체/소비 API, 실제 서사 원문, B1 최종 원문 차집합, 시험 메서드·필수 회귀 선택·단회 유한 상한을 고정한다. 이들은 통상 설계 권위로 정리하며 새로운 사람 질문이나 전체8챕터 문서 승격을 선행시키지 않는다. 그 원문에 대한 루나 사전 독립검수와 아스트라 한정 Approved 뒤에만 C1-C 제품 구현을 시작한다.

## 14. B1 후보 작성 경로·B0 보존 그래프 호스트 보정 — Approved

아스트라는 같은 승인 제품 행동의 충돌 없는 구현과 최초 저작 실패의 최소 복구만 다음처럼 승인한다. 전임 문서 SHA는 `5013AA7A10D1CB828747F02B00BB981E0F7E0C93B2DC9548C094D9C5641917CC`다. §13 C1-C는 여전히 Review이며 이 절이 그 구현 권한을 만들지 않는다.

**B1 후보 경로:** 별도 테라 `gpt-6.1-sol`는 `output/chapter1-playable-r1/staging/b1/` 아래에 §11/12에서 승인한 신규 Runtime/Tests/Editor 파일과 meta만 정확 프로젝트 상대경로로 작성할 수 있다. 이 경로는 Unity 프로젝트가 아니며 Assets 자동 import·실행·빌드·게시를 하지 않는다. B0 실행 동안 기존 project와 공유 파일에는 쓰지 않는다. 후보별 길이/SHA와 최종 project 상대경로를 원장에 남기고 루나 독립 원문 검수 뒤, 실행 종료·현재 공유 파일 인계 지문을 확인하여 기존 제품 구현자가 하나의 project로 통합한다. 신규 후보의 대상이 이미 있으면 덮어쓰지 않고 변경 소유자에게 인계한다. 새 템플릿·패키지·범용 프레임워크나 추가 허용 파일은 없다. 후보 작성은 제품 실행·수용·완료로 집계하지 않는다.

**B0 최초 저작 복구:** 실제 보존 graph는 `Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab` 31532바이트/SHA `B16B013217C342805509DCF5C34BFD1B43A3EBA1F16DBDCE386BA465AFA9A2DC`, meta154바이트/SHA `C2F76DD8A3E5B58D599FF9B8B2C8C0A7D3C22958446B46A8A9C7A0F05E679DAF`다. 최초 저장 시 prefab 루트 이름이 `Chapter1RegularEncounter`인 반면 새 validator가 scene 이름 `CombatEncounterGraph`만 받아 ValidateHost가 실패했다. 아스트라는 실제 두 원문 지문과 host/meta 부재를 읽어 확인했다. 최초 실패 원문/출력은 보존한다.

기존 Chapter1RegularEncounterAuthoringValidator는 persistent asset이면 정확 PrefabPath와 이름 `Chapter1RegularEncounter`, scene instance이면 유효 scene과 이름 `CombatEncounterGraph`를 구별한다. 비활성·producer/router/owner 개수·실제 관계 검증과 기존 Regular validator는 그대로 둔다. 런타임 이름이나 상태를 이 오류 때문에 바꾸지 않는다.

기존 Chapter1RegularEncounterAuthoringBuilder에 `BuildHostFromPreservedGraph()` 한정 진입을 추가한다. 위 graph/meta의 실제 원문을 실행 전후 대조하고 host/meta 모두 없을 때만 이름 있는 임시 scene에서 보존 graph를 읽기 검증하여 승인된 host 두 파일만 생성한다. graph 삭제·재저장·재생성, 동일 Build 재호출·덮어쓰기는 금지한다. 저장 호출은 새 host에 한정하고 무관한 dirty asset을 전체 저장하지 않는다. 새 C#/assembly/friend/자산 경로는 없다.

REQ-C1B-ENTRY-001/AC-C1B-ENTRY-001에 결속하여 테라의 두 기존 파일 최소 수정 원장→루나 독립 델타 대조→기존 상한 내 단회 host 후속 저작 순서로 진행한다. 실제 실행은 Root가 소유한다. 종료 조건은 graph/meta 전후 동일, 새 host/직렬화 참조 exact, ValidateHost 통과다. 이것만으로 실제 host Start·첫 tick·generation2를 수용하지 않으며 해당 B0 검사는 이어서 별도로 수행한다. 같은 project Unity 실행 중 소스 수정은 하지 않는다.

## 15. 일반 방 등록 보존·초기 tick 호환 보정 — Approved

§14까지의 전임 SHA `C86C3981176030AB838084722C50D30DF983FC2BFD3163227B2A528CCBF159CF`를 보존한다. 실제 아스트라는 VD-04 REQ-ROOM-002/004/007과 고정 snapshot, VD-02 REQ-WT-001/007·AC-WT-001, VD-06 AC-CHOICE-004 및 실제 regular graph/handoff 검증 소스를 대조해 다음 최소 보정을 승인한다.

**B1 일반 방 상자0:** VD-04는 방마다 상자를 요구하지 않는다. 각 방의 적 두 종류·위험·보상 소켓과 기본 이동 경로는 유지하며 A/B 변주는 적 좌표만 사용하도록 §11.5 본문을 정정했다. 기존 regular registry의 surveyor/walker 정확2, modifier sink2, player를 포함한 CombatTarget3 검사를 완화하지 않는다. 상자를 넣기 위해 기존 RegularEnemyEncounterAuthoringValidator나 TransferSimulationDriver의 regular 등록/완료 handoff를 확장하는 권한은 없다. 원문 snapshot4는 변경하지 않는다. 이는 CH1의 상자 전이 기능을 삭제하는 것이 아니다. 기존 보스 상자 경계는 보존하고 C1-C의 실제 BarrierCounterweight에서 상자 전이/회수·물리 반응과 두 봉쇄선 AC를 따로 검증해야 최종 CH1을 수용한다. B1 증거를 상자 AC 통과로 재사용하지 않는다.

**B0 보스 최초 tick 호환:** 격리 project의 기존 `Runtime/Combat/Unity/OrdanBossHostileDamageProducer.cs`, `OrdanBossBalanceAuditPullProducer.cs`, `OrdanBossEncounterHandoffAdapter.cs` 세 파일을 추가 수정 허용한다. 앞 두 producer의 최초 tick0 특례를 실제 결속 player의 `Chapter1ConfiguredFirstTick` 값(미설정은0)에만 적용하고 그 tick의 Snapshot.Tick/NextExpectedTick 모두 일치해야 한다. 그 이후 prior snapshot=tick-1 규칙은 유지한다. Handoff는 첫 expectedPublicationTick만 같은 실제 player seed 또는0에서 시작하고 다음은 항상 마지막 발행 tick+1, 실제 Movement 완료 후 NextExpectedTick=tick+1 및 모든 upstream 같은 tick 검사를 유지한다. 외부 int·상수20·ForTests 값으로 최초 tick을 대신하지 않는다. 피해·패턴·등록·수치·terminal 권위 변경은0이다.

테라는 기존 승인 B0 시험 안에서 기본 tick0, 실제 pre-Awake seed20의 최초·다음21, 틀린 snapshot/다른 player/누락·중복 tick 거절을 집중 대조한다. 동일 실패 원문을 보존하고 루나 소스 독립 델타 뒤 기존 유한 실행 관문으로 진행한다. regular 교체 두 UnityTest가 render frame60 동안 실제 FixedUpdate를 기다리지 못한 관측은 실제 fixed 단계와 render 파기 단계를 기다리도록 시험만 보정할 수 있다. 기존 유한 wall-clock/반복 상한을 유지하고 완료view·destroy/dispose·generation2를 직접 대입하거나 실제 성공으로 미리 표기하지 않는다.

**B1 시험 참조:** 신규 승인 `Tests/PlayMode/RunUnity/AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef`의 기존 네 runtime 참조에 `Unity.InputSystem`, `Unity.InputSystem.TestFramework` 두 개만 추가한다. 현재 InputUnity PlayMode 시험의 실제 동일 패키지 참조를 재사용하며 패키지 설치/버전변경·제품 runtime 참조·friend 추가0이다. 실제 InputSystem 장치 이벤트를 사용하는 §11.7 시험에 한정하고 FlowOwner 직접 호출로 사용자 UI 검증을 대체하지 않는다.

## 16. B0 기존 인계 시험의 생성 순서·예외 정리 한정 보정 — Approved

실제 아스트라는 §15까지 전임 원문 SHA `C7ABD46827D4CFE2537E81EDAD616646E998971F7C219D9BFED064582AF21A86`, 테라 읽기 진단 `output/chapter1-playable-r1/evidence/b0-r2-handoff-fixture-diagnosis-terra61.v1.json` SHA `76630EBE649A61C486D84EF75DC9EC21B0F7628F827CE1267F6B9B3AA9A8C37E`, 실제 시험 원문과 집중 r2 XML을 직접 대조하여 다음 시험 한 파일 변경만 승인한다. REQ-C1B-ENTRY-001, AC-C1B-ENTRY-001 및 AC-COM-003/004에 결속한다. 사용자 산출물은 기존 첫 tick·상태 승계·실제 그래프 교체 검사의 신뢰 가능한 재개이며 전체 CH1 제품 목표나 기존 AC를 축소·확장하지 않는다.

관측된 집중 r2는20개 중12통과·8실패다. 원시 XML SHA `32106DBFA4E569B873BC21A0CC48ABF6E42E1979E323DEE7819B06F3F54C55D0`, log SHA `111C697908AB3B28FFBE79AC03A15D63296A233768B4F09B869C7EDE09B83CB3`를 보존한다. 첫 실패는 `InvalidHandoffPublicationsAreAtomic`의 SimulationCameraDriver.Start→Initialize에서 late initialization 거절이며 뒤의 일반방7개에서 중복 collider key가 관측됐다. 실제 시험은 FixedUpdate 대기에서 재개한 뒤 duplicateRoot/skippedRoot를 생성하고, 예외 정리 목록에서 이 두 객체가 빠져 있다. 생성 시점에 따른 Start/FixedUpdate 경쟁 및 잔존 객체가 후속 실패를 유발했다는 설명은 현재 **원인 가설**이다. 원시 stack은 해당 clone을 식별하지 않으므로 확정 원인으로 쓰지 않는다.

같은 현재 소스의 새 단독 일반방 검사 XML `unity-b0-single-regular-contamination-probe-r1.xml` SHA `1752612A48FA598B6D61CD92A1E06AB5351998CD0AE2B98D7DB4D92BC549DED7`은1/1, 새 실제 교체·seed20/21 검사 XML `unity-b0-production-carry-replacement-probe-r1.xml` SHA `98FFAB2C525A5C04489758E0F555B79AACDFAE5D3DA285C8BC8CE58D7971C44A`은2/2 통과다. 이 근거는 실패가 모든 새 생산 경로에서 재현되지는 않는다는 관측이며 B0 전체 수용이나 기존 r2 실패의 삭제 근거가 아니다.

**정확 수정 범위:** 별도 테라 `gpt-6.1-sol`는 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterHandoffAdapterPlayModeTests.cs`의 `InvalidHandoffPublicationsAreAtomic` 한 메서드만 수정한다. 변경 전 파일 SHA는 `E430CB33E4080BF7906A6CCF4AD3ADD3C3160C8EFC23C8EF4438DA0392730B15`이며 불일치하면 통합을 멈춘다. 원본 Assets 사본·meta·다른 시험 메서드·assembly·runtime·자산 변경은0이다. 같은 project를 Unity가 읽는 동안에는 이 시험 파일도 쓰지 않는다.

허용 델타는 다섯 소유 변수 `root/firstFailureRoot/foreignRoot/duplicateRoot/skippedRoot`를 try 밖에서 null로 선언하고, 다섯 Instantiate 전부를 try 안의 최초 yield 이전에 배치하는 것이다. 기존 first/foreign 오염 주입·duplicate/skipped 검사 블록·WaitForFixedUpdate 개수·publication 불변 및 원자 거절 기대를 유지한다. 기존의 성공 경로 끝 두 객체 Destroy 호출을 finally 정리로 합치고, finally에서 다섯 객체 각각 null 여부를 검사하여 SetActive(false) 후 DestroyImmediate로 시험 소유 객체를 모두 수거한다. 기존 역사 reflection 호출은 그대로이며 새 reflection·직접 Movement 진전·카메라 초기화 우회·예외를 숨기는 LogAssert 추가·기대 완화는0이다. SimulationCameraDriver의 snapshot.Tick/NextExpectedTick guard는 변경하지 않는다.

이 정리는 시험 실패에도 다음 시험을 오염시키지 않기 위한 장치다. DestroyImmediate로 생산 Destroy/actions dispose가 검증됐다고 주장하지 않는다. 실제 제품의 graph 교체·actions 종료·generation2·dispose 수용은 기존 생산 경로 검사에서 별도로 유지한다.

**검수·실행 관문:** 테라는 전후 원문·길이/SHA·정확 diff와 요구/AC 연결을 남기고 루나가 위 범위·생성 시점·다섯 객체 예외 정리·기대 불변을 독립 대조한다. 그 통과 뒤 Root만 단일 `InvalidHandoffPublicationsAreAtomic`을 새 실행에서1회 재현한다(10분 상한). 통과하면 같은 승인 시험 집합의 일반방 검사를 이어 확인하고, 기존 승인 신규 B0 집중 집합 전체를 새 실행1회10분으로 확인한 뒤 필수 영향 회귀1회30분으로 진행한다. 회귀를 EditMode/PlayMode로 나눌 때 각각 한 번이며 합산30분을 늘리지 않는다. 각 실행 전 최종 시험파일/meta/asmdef 및 정확 method 선택 지문을 고정하고 루나가 §9.2의 M5B4/M5B5·Movement tick·Transfer 초기화·Combat 등록/무적·Ordan·C1A 영향 필수검사 누락0을 대조한다. 현행 fixture 조사 원장은 선택 준비이며 과거 C1A243개 성공이나 단독3개 성공으로 필수 회귀를 대체하지 않는다.

첫 실패에서 해당 실행을 중단하고 동일 자동 반복0이다. XML/log·프로세스 시작/종료·현재 source/asset SHA를 보존하며, 같은 late initialization 또는 중복 key가 남으면 clone instance·scene 잔존과 최소 순서 관측을 먼저 수행하도록 다음 한정 관문으로 돌아간다. 생성 이동이 충분하다고 미리 단정하거나 runtime guard 완화로 진행하지 않는다. 기존 종료·배수 각5초 및 무로그만으로 중단 판정하지 않는 규칙도 유지한다. 종료 조건은 한정 소스 델타 독립 수용과 단일 재현·신규 집중·필수 회귀의 실제 결과 확보이며, C1C 승인·전체 CH1 완주·Q0·일반 프로필·키·원격 게시 변경 권한은0이다.

## 17. C1-C 선택·기술·봉쇄선·마지막 장면 한정 구현 — Approved

§16까지 전임 SHA `E316350C049FA9B5C26A249B462E76CE30CFAFDE3F3800AA5B84397259070F6A`를 보존한다. 실제 아스트라는 [C1C 한정 구현 승인](./2026-10-03-c1c-bounded-closure-candidate.md) SHA `03067E1293E35CAC4C850BA0F68D2E0E584522141EC48E6CA2C1F2ADCCFE2725`의 §1~7을 현재 C1-C 권위로 채택한다. 독립 검수에 투입한 전임 원문은 `F111E119F4BEDA54DB88F7925E451BE9E4BD8E8D4AD8CEB95F81BAD8AA66DB05`이고 승인 변경은 상태·승인 기록뿐이다. 루나 전체 정적 검수 v1 SHA `7665F4958F386A06DCF7D04A9EB6E27113C98ACB37948A43B93B4D2916D74422`와 마지막 조준 형상 재검수 v2 SHA `1EB492FC0E3D4E16FAF961FF47A6C63F4682A3533AC233FB1E2072D6B2BAA65A`를 합하여 기존 P0=0·남은 P1=0인 설계 범위만 승인한다. 실제 양분기 검수·빌드·실행 성공 판정이 아니다.

§13 및 앞선 C1-C Review 표현은 위 정확 승인 범위에서 이 절이 대체한다. 기존 V1 pair/CompletedBranches와 같은 실행 closing receipt, 실제 BarrierCounterweight의 상자 AC·정확 기하/조준/피해/발판/비치명 복귀, 단일 생산 입력·권한 포트, 캐논 원문·완료 순서, 정확 파일/참조/검사·회귀220개 입력 원장·단회 상한·검사 전용 개발 저장 보존 절차를 유지한다. 솔 V2 제안은 역사이며 새 스키마·지속 CH1 완료 필드·일반 사용자 프로필 접근은0이다.

별도 테라는 승인된 충돌 없는 신규 파일 작성·준비를 진행할 수 있고, B1/B0 공유 파일 수정과 실제 project 통합 전에 독립 검수된 최종 기준지문·변경 차집합을 고정한다. B1 미완료를 모든 C1-C 설계/준비의 선행으로 삼지 않으며 같은 project Unity 실행 중에는 제품을 쓰지 않는다. 루나 구현 델타 검수→한정 compile/저작·집중/필수회귀→실제 창 양분기 완주·시간 차·서사·재시작→아스트라 통합 수용 관문은 모두 남는다. 양분기 전체 CH1 목표·필수9회·Q0 인간 승인·키·원격 게시 관문은 그대로다.

## 18. B0 정확 영향 회귀 선택·B1 보스 입구 보존 — Approved

전임 SHA `2AD3FA09FAC75FDBAC54A0A1928A7CACBB531AE84E440ADBD25E6FC4DBDEFF87` 뒤 한정 보정이다. 실제 아스트라는 현재22개 시험 census와 `output/local-playable-slice-r1/evidence/unity-regression-r1-start.json`의 과거 선택을 직접 비교하고, B0 회귀 선택 원장（`../../../output/chapter1-playable-r1/preparation/b0-regression-selection-astra.v1.json`；로컬 보존 자료） SHA `7500B340B7294BF52C5E6525BC64A7EE29465AA99397BAFA2956FF41174F958B`에 정확 선택과 입력 지문을 기록했다. 과거 C1A243개는 일부 클래스의 지정 메서드를 포함한 당시 선택의 성공이며 현재 B0 전체 회귀 결과가 아니다.

현재22개 전부의 전체 메서드를 유지하고 다음3개 전체 클래스를 추가한 정확25개를 §9.2/§16 영향 회귀로 승인한다: `AcadeGameMaker.Tests.EditMode.Combat.CombatSessionTests`, `AcadeGameMaker.Tests.PlayMode.Movement.PlayerMovementInputFrameCandidatePlayModeTests`, `AcadeGameMaker.Tests.PlayMode.TransferUnity.TransferInputFrameCandidatePlayModeTests`. 앞 클래스는 공통 Combat 등록·최대HP·중복 피해·44/45틱 무적·실패 원자성을, 뒤 둘은 실제 기존 AC-M5B4의 소비자 Prepare/Validate/Commit 경계를 검사한다. 새 carry3개와 M5B5 InputRouter 검사만으로 이 누락을 대신하지 않는다. 기존22개에는 M5B5 전체, Ordan Combat의 M5B4 조정, Movement tick, Transfer 초기화/입력 잠금, Ordan/terminal, C1A 및 새 B0 집중 클래스가 포함된다. 실제 발견 시험 개수는 사전 추정하지 않는다.

정확 추가 복제는 원장 `exactCopyInputs9`의 세 `.cs`와 각 `.meta`, 원본 `Tests/EditMode/Combat/AcadeGameMaker.Combat.EditMode.Tests.asmdef` 및 meta, `Tests/EditMode/Combat.meta`의9개뿐이다. 모두 원본 바이트/SHA 그대로 지정 project 상대경로에 복제한다. 기존 Combat runtime이 이미 같은 시험 assembly friend를 선언하므로 runtime/friend/패키지 수정0, 기존 Movement/Transfer 시험 assembly 재사용이다. 원장의 관측에서9개 목적지는 모두 부재다. 실제 복제 전 목적지 부재와 기존 GUID 충돌0을 확인하며 다른 바이트가 있으면 덮지 않는다. 이 절은 누락 시험 입력의 정확 복제를 허용하지만 실제 복제나 컴파일 성공을 주장하지 않는다. Root가 Unity 종료를 확인한 후에만 별도 테라가 복제하고 전후 원장을 남긴다.

루나는 복제9개 exact SHA/meta·시험 의존·실제25개 선택에 필수 AC 누락이 없는지 독립 대조한다. 보조 입력이 더 필요하다는 실제 컴파일 근거가 나오면 정확 경로를 먼저 결정하고 임의 추가·검사 제외0이다. 실행 직전22개 기존 파일 중 §16 보정 및 새 음성시험이 반영된 최종 SHA와 모든 method discovery ID를 새 실행 원장에 고정한다. 이 정적 선택 원장 자체는 discovery가 아니다.

§16 단일 재현→신규 집중 성공 뒤 Root만 회귀를 실행한다. EditMode1회·PlayMode1회, 합산1800초와 종료·배수 각5초를 유지하며 플랫폼별30분으로 늘리지 않는다. 실패·상한 도달 시 원시 보존 및 동일 자동 반복0이다. 이 선택에 과거 C1A 성공, 다른 복사본 성공, Root 단독3개 성공을 현재 실행 성공으로 재사용하지 않는다. 같은 최종 입력의 직전 집중 결과는 원인 조사 근거로 링크할 수 있으나 이번25개 전체 회귀에서 메서드를 빼는 근거로 쓰지 않는다. 기존 필수9회·게시 검사를 이 한 번으로 대체하지 않는다.

**B1 공유 API 인계:** 조사한 staging `Chapter1PlayableFlowOwner.cs`의 `RequestChapter1BossReplacement`는 `(0,0.81)`을 전달하지만 원본 OrdanBossEncounterAuthoringBuilder의 player 저작값과 현재 OrdanBossEncounterGraph prefab의 중첩 player override는 정확 `(-7,0.81,0)`이다. prefab의 `(7,1.2,0)`은 Ordan 보스 자신의 위치이며 플레이어 입구가 아니다. 소스/prefab/B1 후보 원문 SHA는 위 선택 원장의 `sourceEvidence`에 고정했다. B1 구현자는 기존 허용 FlowOwner와 공유 lifetime API의 실제 인계에서 플레이어 저작 입구 `(-7,0.81)`을 보존하고 일반방 `(0,0.81)`을 보스에 재사용하지 않는다. prefab/root가 평행 이동한 경우 실제 graph 루트 기준으로 변환하며 world 상수를 강제로 덮지 않는다. 새 graph는 기존 비활성 seed 경로에서만 위치·속도0을 구성하고 실제 HP·무적·연속 tick 승계 및 boss 원위치는 유지한다. FlowOwner 호출·lifetime 소비·저작 validator의 동일 입구 대조를 B1 변경 원장과 루나 델타 검수에 포함한다. 이 결정으로 기존 prefab/builder·카메라/전투 수치·상자 배치를 바꿀 권한은 없다. 현재 이 문서 작업은 공유 project 소스 쓰기0·Unity 실행0이다.

### §18 복제 담당·확인 순서 편차의 현재 입력 한정 수용 — Approved

실제 아스트라는 `b0-regression-copy-root.v1.json`, `b0-regression-guid-check-root.v1.json` 및 `b0-regression-selection-review-luna6.v1.json`을 직접 읽었다. Root가 지정9개를 복제했고 사전 GUID 검사를 누락한 것은 §18 절차 편차로 보존한다. 테라 수행으로 이름을 소급하거나 사후 검사를 사전 검사라고 바꾸지 않는다. 독립 루나는 기존22개와 추가3개 선택·원본/복제9개 SHA·참조 폐쇄·필수 AC를 대조했고, 실제249개 project meta 중 신규5개 GUID가 각각1개·충돌0임을 독립 확인했다. runtime 수정0과 정확 복제 바이트가 확인된 현 입력은 재복제 없이 수용한다. 담당과 순서의 일탈을 일반 허용으로 확대하지 않는다.

이 판정은 루나의 기술 준비 P0=0 및 절차 P1 한 건에 대한 아스트라의 명시 처리이며 해당 보류만 해소한다. Root는 §16 단일 재현 및 신규 집중 성공의 실제 원시 증거와 현재 입력 지문을 결속한 뒤 §18의 정확25개 전체 회귀를 EditMode/PlayMode 각1회·합산1800초 안에서 실행할 수 있다. method discovery·최종 지문 고정·첫 실패 원시 보존·자동 반복0·필수9회 불변을 유지한다. 실제 회귀 통과나 B0 전체 수용을 미리 선언하지 않으며 B1 공유 API 설계 완료는 이 회귀의 선행조건이 아니다. 앞으로의 복제는 원래 담당·사전 확인 순서를 준수한다.

## 19. B1 보스 구성·실패 승인 출처의 공유 API 한정 확정 — Approved

전임 계약 SHA `756935FDAE11B28DA2C8CE6E2BBB6DC2DDB9EF1D27CA3933480D8511CC5C194A` 뒤, 실제 아스트라는 [B1 공유 API 인계](./2026-10-03-b1-shared-api-closure.md) SHA `2870B88BFED397A8280D7729E47F5622E5C01203339DFDB07BAC0EF975268E9F` 및 실제 소스 읽기 원장 `output/chapter1-playable-r1/preparation/b1-shared-api-closure-astra.v1.json` SHA `5449FFA9621327E663F296ACF5562D580D1D92BA7D21D2FC4C0C04C1C886FB0F`의 정확 범위를 기존 B1 구현 승인에 결속한다.

미정이던 `ConfigureChapter1ProgressionForAuthoring`은 실제 lifetime/entry/port/teardown의 비활성 단회 결속으로 닫는다. 문자열 run/generation/tick만 받는 실패 전환 후보는 실제 세 발행 원문 객체와 중재 결과를 결속한 `Chapter1AcceptedFailureToken` 및 양쪽 pending 검증·단회 소비로 대체한다. 보스 graph의 requester는 기존 Ordan 종료 요청자 하나이며 lifetime은 그 입력 권위를 중복 취득하지 않는다. +200 실제 완료 관측→+210 세 원인 실패 중재 우선→다음 tick 기존 Transition/정리 순서, 기존 player 입구 `(-7,0.81)`와 HP·무적·tick 승계를 유지한다. 해당 문서의 정확 기존 파일/서명/시험 외 제품 파일·권한을 늘리지 않는다.

테라는 Root의 실제 B0 최종 입력 지문과 독립 검수 인계를 받은 뒤 공유 파일을 수정한다. B0 최종478개 원장은 루나가 별도 검수할 증거이며 본 설계 승인으로 그 검수를 대신하지 않는다. 구현 후 루나가 신규 실패 token의 실제 발급/소비·복제/타owner/옛세대 거절·동일 tick 실패우선·실제 cleanup과 legacy 불변을 독립 확인하고 기존 유한 compile/집중/회귀/종단 관문으로 진행한다. 이 문서 작성의 제품 소스 쓰기·Unity 실행은0이며 B0 전체 수용·B1 완료·C1C 완주·전체 CH1 완료를 주장하지 않는다.

## 20. B0 검증 범위 한정 수용·B1 기승인 접근성 명확화 — Approved

전임 주 계약 SHA `026C3C594D1BCBBAD190D8937AAA8A81FDDBA3DE6DF81A1AB9A657468B939219`를 보존한다. 실제 아스트라는 `output/chapter1-playable-r1/evidence/b0-final-execution-review-luna6.v1.json` SHA `92E8498E30C4F2C88988885F26360E3C081BAC798886F1082CB8076F3AD88E40`의 독립 P0/P1=0과 원시 결과 대조를 직접 읽고, B0 집중23/23 및 정확25개 클래스 영향 회귀299/299·실패/건너뜀0의 **해당 검증 범위만** 수용한다. EditMode40·PlayMode259의 개별 경과 합산 상한72.6230483초가 계약1800초 이내임을 확인했다. 집중·회귀별 source와 원시 XML 결속을 유지하고 다른 미검증 행동을 이 숫자에 포함하지 않는다.

B1 공유 구현 기준은 `output/chapter1-playable-r1/evidence/b0-final-source-checkpoint-root.v1.json` SHA `2EECB8659FB07548BE304AAA9BCCF3155A0EE530E75802853FD51B5214B19AC9`의 Assets/Packages/ProjectSettings 정확478개다. 루나가 현재 파일 전수의 누락0·길이/SHA 불일치0과 Unity 종료를 독립 대조했다. 이 원장과 과거 실패/r2·보정·복제 편차 기록은 불변으로 남긴다. 테라는 공유 파일 쓰기 직전 해당 파일의 기준 SHA를 재확인하고 B1 변경 차집합을 별도 기록하며, 이 기준을 B1 구현 후에도 현재값인 것처럼 재사용하지 않는다. B0 전체 게임 수용·B1 종단·CH1 완주·새 배포/게시 수용은 아니다.

루나의 B1 접근성 검수 `b1-shared-api-closure-review-luna6.v1.json` SHA `9EE023D07D6EC569022F900A924EB85B29684C578DA6CC44D4DD854DF497BAB8`는 현재 B0 Input.Unity에 Run.Unity friend가 없다는 사실을 정확히 관측했다. 다만 §11.6의 “기존 Run/AssemblyInfo와 Input/Unity/AssemblyInfo에 production Run.Unity 각1을 추가하고 Input/Unity에 새 시험 friend1을 추가한다”는 범위는 이미 §12 Approved다. §19 인계의 “신규 friend0”을 이 기승인 변경까지 금지하는 것으로 해석하지 않는다. 해당 표현은 기승인 집합 외 확장0이며 아직 통합하지 않은 기존 승인2개를 없애지 않는다.

[B1 공유 API 인계 보정본](./2026-10-03-b1-shared-api-closure.md) SHA `4C55BF58011376F68E70E042C3FDDDCC62736386FA34A03327C1E6B42C396011`을 현재 권위로 채택한다. Input/Unity/AssemblyInfo에 `AcadeGameMaker.Run.Unity` 및 `AcadeGameMaker.Run.Unity.PlayMode.Tests` friend2, Run/AssemblyInfo에 Run.Unity friend1, Run.Unity의 Input.Unity 참조를 기존 승인 그대로 함께 구현하면 internal token/생성자의 컴파일 접근이 성립한다. Input.Unity→Run.Unity 역참조·public token·새 권한 stub·다른 friend는0이다. 이전 인계 SHA `2870B88B…268E9F`와 실제 소스 읽기 원장은 역사로 보존한다. 루나는 이 정확 보정에 한하여 접근성 P0를 재대조한 뒤 별도 테라의 기준478 결속 공유 구현으로 인계한다. 이후 실제 컴파일과 pending/출처 음성 검사는 여전히 필요하다.

## 21. B1 실제 lifecycle clear 읽기 투영 한정 보정 — Approved

전임 주 계약 SHA `E30B748FE51FE7C74F7493FA2B64367EBB02E3E356A7DAF9A271A3BC9D797183` 뒤, 아스트라는 [B1 lifecycle 접근 보정](./2026-10-03-b1-lifecycle-clear-projection-review.md) SHA `688A90FC7F848C116C4BECB3C8C8766370379FE7CD26C7B387B24884D9A0A9A7`의 정확 두 파일 설계를 승인한다. 직접 읽은 루나 독립 설계 검토 SHA `E9AE54633A1CF722B1A5A5564A8EEFB0DCDDA03B812D31869715845E389F2086`는 P0=0/P1=1이며 유일 P1은 AC-C1B-EVIDENCE-001 추적 누락이다. 해당 AC를 기존 REQ-C1B-EVIDENCE-001 및 실제 publication/receipt/tick 검사에 명시한 문서 보정으로 아스트라가 닫는다. 루나 원문과 Review 당시 SHA를 보존하고 독립 재검수 P1=0을 발명하지 않는다.

제품 루트 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/`의 `Runtime/Transfer/Unity/TransferSimulationDriver.cs`에서 기존 TransferPhasePublication에 `internal TransferCleared? LifecycleClear => Outcome.RemovalResult.HasValue ? Outcome.RemovalResult.Value.Cleared : null;` 읽기 투영 하나를 추가한다. `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs`의 ValidateLifecycle 소비점 하나가 이를 읽는다. 실제 기존 phase·tick·lifecycle reason·Transition·현재 router receipt 동일성·Baseline·ActiveTargetId 없음·이전 target/clear 존재·PreviousTargetId·clear Reason/Tick의 모든 검사를 유지한다. null을 성공 bool/기본 clear로 바꾸지 않는다. 같은 실제 Outcome을 읽을 뿐 새 발급권이 아니며 public TransferCleared 값 타입과 기존 Transfer.Unity friend를 사용한다. 순수 Transfer·friend·assembly 참조·신규 파일 변경은0이다.

REQ-C1B-EVIDENCE-001/ROOM-001/FAIL-001/RETURN-001, AC-C1B-EVIDENCE-001/INPUT-001/E2E-001/E2E-002/ROOM-001/FAIL-001/RETURN-001에 결속한다. 실제568 바이트 기준 및 변경 전 두 파일 지문을 확인한 뒤 별도 테라가 두 파일만 보정하고 루나가 델타를 독립 검수한다. 그 후 Root만 새 이름의 최소 컴파일을 한 번 수행한다. 기존 실패 원시 결과 `Library/b1-minimal-csc-r1/result.json` SHA `8AA8C4CE26253B19245AE153E93389F0932DAD1B69FADAD90FCF91A504735050`를 변경하지 않는다. 같은 오류 자동 반복0, 후속 조립체 결과·입력 지문·원시 결과 독립 검수를 요구한다.

기존568 소스 검토의 접근 경계 P0는 아직 제품 보정·새 컴파일·독립 검수가 끝나지 않아 미해결이다. 집중31과 기존 필수 회귀·종단의 미실행 상태를 유지하고 컴파일 성공만으로 AC 통과나 B1 전체 수용을 선언하지 않는다. 검사 선택자·유효 clear와 누락/외부/다른 target·reason·tick 검사는 한정 계약 그대로 유지한다. C1C 초안의 Review와 공유 쓰기 관문, Q0·프로필·키·9회·원격 경계는 별도다. 이번 문서 작업의 제품 쓰기·컴파일·Unity·시험 실행은0이다.

## 22. C1C 종료 입력·실제 회수 접합 한정 승인 — Approved

전임 주 계약 SHA `3D3F120FBA19B377AAD69E992BAEC31A39A90839B0A979C216C41784ED846E07` 뒤, 아스트라는 [C1C 종료 입력·회수 접합](./2026-10-03-c1c-ui-recall-closure-draft.md) SHA `80CC2ED411FFC21144A68A159D2478267F4417D10B134F0FFE3735897333952D`의 §5.1~5.9와 §6을 정확 한정 권위로 채택한다. 직접 읽은 루나 독립 검토 SHA `7A4D46A876371169CE980573BC19638CDCF66C200F7A63C0753484F34348D3B5`의 P0/P1=0, 검수 초안 SHA `0411221DB2A89B6DA3B0658D40CB69A7AD6E4C49E8DF948AC42F64ABBA252EBB`, 준비 v2 SHA `9979AD8951F8CD04EB4AC1AEEE28AFAB6A0178D3489ED2CA2DDD083BAE49E35B`를 보존한다. 설계 승인이지 제품 구현·플레이 수용은 아니다.

기존 실제 Cutscene→DemoCompleted clear→Run 성공→원자 branch save→Ended 순서와 terminal guard 뒤 실제 graph 파기/actions closure를 확인하고 새 Closing UIOnly host를 발급한다. 현재 HubUiOnly의 profile 준비 경로를 단순 재사용하지 않는다. oldDemoGen/newUiGen·같은 원정의 원본 demo/save/entry·별도 UI tick·중립/새 edge·60틱을 결속한다. driver4 회수 API와 Session Cancel은 private Session/실제 context·capture·revision·checked 준비·단회 소비를 유지한다. 정확 추가 기존 파일은 `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs`와 `Runtime/Run/Unity/Chapter1RunView.cs` 둘이며 나머지는 기존 승인 파일/Closing 경로·15검사·220입력 안이다. Closing prefab은 아직 미저작인 기승인 미래 산출물이다.

주 계약 §8.6/§13.2 및 기존 C1C §3의 미게시 규칙은 위 문서 §5.6대로 한정 호환 보정한다. -300 input transaction 오류는 기존 input receipt0, -200 회수/환경/기술 commit의 늦은 오류는 이미 발행한 input receipt를 사실대로 보존하면서 해당 tick Choice/Transfer 완료·후속 Combat/Movement·통합 +110 view/통과/CH1 완료0 및 TechnicalBlocked·후속 Press0이다. 이전 정상 역사값이나 부분 변경을 지우거나 rollback했다고 주장하지 않는다. 기존 입력 실패 AC와 성공 순서를 줄이지 않으며 REQ/AC-C1C-UIR-001~006 및 AC-CH1-003~007·AC-CHOICE-001~004·AC-WT-003/006을 함께 추적한다.

충돌 없는 신규 후보 준비는 계속할 수 있으나 공유 project 쓰기는 B1 r2 시험 CS0012 해결·별도 독립 최종 수용/지문과 C1C 통합 기준 원장·루나 대조 뒤다. B1 시험 참조 보정 Review의 승인/실행 권한은 이 절과 별개다. 테라 구현·루나 독립 검수·기존 유한 검사/필수 회귀·같은 최종 빌드 양분기 실제 완주와 실패 재도전 관문을 유지한다. 신규 C#/assembly/action/friend·지속 완료 저장·프로필·Q0·키·9회·원격 권한 확대0이다. 이번 승인 작성의 제품 쓰기·Unity·시험 실행은0이다.

## 23. B1 시험 조립체 Run 직접 참조·실패 묶음 최소 재확인 — Approved

전임 주 계약 SHA `2730E3516D27B9418AE1D9C890B54C892173C7B82718FDA40B04F9C8EE1A87CF` 뒤 아스트라는 [B1 시험 Run 참조 보정](./2026-10-03-b1-test-run-reference-review.md) SHA `64F159DF03262262D6A04C746FF38A7DEF768F3007115DDD8268622887FB1313`를 정확 한정 승인한다. 루나 독립 검토 SHA `4EABDE87E2B82707AC9CB4D4BE9BC28AE384AEBA35C3D4B1D93746F49CF0420F`의 P0/P1=0과 현재568·준비8·재사용468·컴파일 자원10 지문 일치를 직접 읽었다. 이전 두 실패·r2 여섯 성공 컴파일을 범위별 역사로 보존한다.

§11.6의 시험 참조 집합에 `AcadeGameMaker.Run` 직접 참조1을 추가한다. 정확 변경 파일은 project의 `Assets/AcadeGameMaker/Tests/PlayMode/RunUnity/AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef` 하나다. `Chapter1FailureSourceTests.RejectStaleOrAllFalseFailureBatch`의 두 CloseCompleted 호출은 내부 RunFailureTickInput 결과를 폐기하며 생성·멤버 접근을 하지 않는다. 참조는 메타데이터 해석만 제공하고 내부 접근권을 늘리지 않는다. 기존 모든 assertions·31 선택·생산/시험 소스·meta/GUID와 나머지 asmdef 값은 유지한다. 새 friend/public factory/ForTests/반사·시험 삭제·Ignore는0이다.

REQ-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001 및 AC-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001/E2E-001/E2E-002를 추적한다. 테라 후보/적용 바이트→루나 정확 델타 검수→Root 새 시험 response 차분과 실행 직전 지문 대조→실패 시험 조립체 최소 컴파일1회→결과 독립 검수 순서다. 새 response는 r2 대비 승인 Run DLL 참조1·새 출력 경로만 바꾼다. 내부 접근 오류가 남으면 실패 기록과 원인을 먼저 보존하고 자동 반복하지 않는다.

성공6 재사용 원장 SHA `5C6C12B3C2D0CC9765374CC4A829D6C02AEEEBAF56C4EB59AF83152E11026E35`와 컴파일 자원 원장 SHA `DF946905A47CC3C724FC503F5AA677FB9BEAD6E8BF22263A8980A4470148FDC9`의468/10 입력이 실행 직전에도 같을 때만 성공6을 재컴파일 없이 한정 재사용한다. 원장 내부의 당시 미수용 false는 보존하고 현재 승인 기록으로 연결한다. 변경/누락이 있으면 재사용을 멈추고 정확 영향을 판정한다. 큰 Unity 실행 권한은 없다.

기존 생산 접근 오류에 대한 §21 보정·r2 성공6과 이번 시험 CS0012를 구별한다. 신규31·필수 회귀·종단은 미실행이고 최소 컴파일 통과는 runtime AC나 전체 B1 수용이 아니다. C1C 충돌 없는 준비는 기승인 범위에서 계속하되 공유 제품 쓰기는 B1 최종 독립 기준과 C1C 통합 관문 뒤다. 이번 문서 작업의 제품 쓰기·컴파일·Unity·시험·프로필·Q0·원격0이다.

## 24. B1 내부 반환형식의 정확 시험 접근 예외 — Approved

전임 주 계약 SHA `698CB46B9D40543B1EA7A58A6923A1DA7533171E328C2EBF7FFE90A127787E7F` 뒤 아스트라는 [B1 내부 반환 접근 보정](./2026-10-03-b1-test-return-access-review.md) SHA `7BB1C47EB05875E8D311C707A7154C5F659A66CB620D1381F977CC28DE547EBE`를 정확 한정 승인한다. 직접 읽은 루나 독립 검토 SHA `302B69CB6575AF1DF82ADA3F701C31C1A553A1704F167A0349CB7F7CF479B348`의 P0/P1=0, 현재568v3 및468/10 원시 입력 일치를 근거로 한다. §23에서 반환값 폐기와 참조1이면 충분하다고 한 추론은 r3 CS0122로 반증됐다. 그 원문과 모든 실패를 소급 수정하지 않는다.

제품 루트 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs`의 변경 전199바이트/SHA `2BDE9F85BCA41C5EE4AC73B611461DFA0D744FE507CA4AF7C7D52B4495B11BC9`를 대조하고 `[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")]` 한 줄만 추가한다. 기존 §11.6/§23 및 해당 한정 계약의 ‘새 friend0’에 대한 정확 예외다. 이는 시험 assembly 전체의 순수 Run 내부 접근이라는 실제 QA 권한 확대다. 다른 시험/생산 assembly에 확대하지 않으며 생산 함수/반환형식/FlowOwner·세 시험/assertions·meta/asmdef를 바꾸지 않는다. 공개 factory/ForTests/반사/시험삭제/Ignore0이다.

REQ-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001 및 AC-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001/E2E-001/E2E-002를 유지한다. 테라 정확1파일→루나 델타 독립 대조→Root 새 Run+실패 시험 두 조립체 최소 진단 한 묶음 한 번→결과 독립 검수 순서다. 새 Run/ref DLL을 만들고 시험 response는 그 산출물을 참조한다. 기존 성공5만 입력·자원·identity 동일성 및 메타데이터 변경 범위 검수 뒤 조건부 재사용한다. 새 연결 원장을 작성하고 역사468/10 원장을 현재 Run 지문으로 덮지 않는다.

첫 실패에서 후속 컴파일을 중단하며 반복·큰 Unity·성공5 재컴파일0이다. §21 생산 보정/r2 성공6, §23 직접 참조/r3 실패, 이번 QA 접근 예외를 서로 다른 증거로 남긴다. 31검사·필수 회귀·종단은 아직 미실행이고 최소 컴파일은 runtime/B1 전체 수용이 아니다. C1C 공유 제품 쓰기는 독립 B1 최종 기준과 통합 관문 뒤다. C1C 합본 입력/완료 API Review는 별도 독립 검수 중이며 이 절이 승인하지 않는다. 이번 문서 작업의 제품 쓰기·컴파일·Unity·시험·프로필·Q0·원격0이다.

## 25. C1C 물리 중립·원본 완료·새 입력 복귀 합본 한정 승인 — Approved

전임 주 계약 SHA `63A4C5472B68B8D5DA5D1D54804DE47EBF8845A71AA323891B32F2A290FA51D6` 뒤 아스트라는 [C1C 물리 중립·완료·복귀 합본](./2026-10-03-c1c-closing-neutral-read-review.md)의 합본 한정 승인 절을 채택한다. 직접 읽은 루나 합본 독립 검토 SHA `8B88FFD677D8407AC3EAC48D19C414A9B2600DEAC7520A1952A59848F220F774`의 P0/P1=0/0, 검수 원문 SHA `9BD3C48C901C5478E7EAADB18A7496F02A7F773CED18F71267BC0AB6981847C7`, 준비 원장 SHA `D395C8A1C13158E63C31C5B527DAD01DE2300D077967EE931FAA9D2B7F1ED037`를 보존한다. 앞선 중립 단독 검토를 합본 검수로 소급하지 않는다. §17/§22의 기승인 Closing 행동과 파일 소유 안에서 정확 내부 API 세 개를 닫는 설계 승인이다.

InputRouter의 실제 단일 UI transaction에 결속된 물리 중립 읽기, 현재 같은 source/router/issued entry·receipt/frame 전체 값·private proof, 읽기 재샘플/시계/latch 진전0을 유지한다. 실제 원본 CH1 receipt의 후속 통지만 adapter Closing→Ended를 허용한다. FlowOwner는 현재 원정/분기·retained demo/save·확정 revision·소비된 원본 closing/port·원래 demo 세대와 새 UI 세대·issued entry/router를 검증하며 Input→Run.Unity 참조0이다. 원자 발급의 pending 소비 후 외부 실패 가능 호출0은 보존하고 adapter 통지는 발급 완료 뒤에 수행한다. 통지 실패는 receipt 보존·진단 정지·복귀 token0이며 취소/재발급0이다.

최종 대사 edge는 거점 복귀로 재사용하지 않는다. 완료 통지 때 고정한 frame 하한 뒤 새 물리 중립·그보다 높은 새 edge의 단회 completed-return, 기존 token 소비·retirement 요청, 실제 graph 파기/actions closure 뒤 기존 새 Hub 생성 순서를 지킨다. 입력은 UIOnly를 유지하고 과거 demo router의 Ended guard를 해제하지 않는다. 완료/closing/entry와 Ended 표시는 재시작 복원0이다. 기존 V1 pair/CompletedBranches 및 서사·기술·성공6단계·실패 규범은 그대로다.

REQ-C1C-UIR-001/002/003, REQ-C1C-004/005/006 및 AC-C1C-UIR-001/002/003, AC-CHOICE-001/002, AC-CH1-003/004/007을 추적한다. 기존 ClosingPersistence 네 선택자를 보강하고 전체15선택자·220입력 재결속·유한 검사/필수 회귀·같은 최종 소스의 루나 양분기 실제 완주·실패 재도전·현재 빌드를 유지한다. 준비 v4의 미완성 생산 fixture와 Assert.Ignore를 성공으로 인정하지 않는다.

정확 허용 파일/API의 구체화 외 신규 C#/assembly/friend/프로필/semantic ABI/지속 완료 필드 권한은0이다. §24의 B1 QA friend 예외는 별도다. C1C 공유 쓰기는 B1 독립 최종 수용/기준지문과 c1c-integration-base.v1.json의 정확 차집합·소유·루나 대조 및 같은 project Unity 종료 뒤다. 최소 컴파일은 runtime/B1 최종 수용이 아니며 게시 완료를 로컬 작업의 추가 선행으로 만들지 않는다. 솔 구현·루나 독립 검수·아스트라 통합을 유지한다. 이번 문서 작업은 구현·실행·완주 수용이 아니며 제품·스테이징 소스·Unity·컴파일·시험·프로필·Q0·원격0이다.

## 26. B1 출정 저장 실패의 새 중립·새 거점·별도 출정 한정 승인 — Approved

전임 주 계약 SHA `0255576B517AC315A52D9C042A5FF6C1128C946688EA0D77005D96004C2BA3E9`의 §25까지 원문을 보존하고 아스트라는 [B1 출정 저장 재확인](./2026-10-07-b1-expedition-save-retry.md) §1~8을 정확 한정 채택한다. 검수 원문 SHA `0E8F2ECAFB49090811627D1498F1A7B17573E437725730B752690CB58D4D2163`, 솔 준비 v2 SHA `92822F8666D0641F74640CDA8D54856309045F4EBA76661D1445B9042ABB1847`, 루나 전체 사전검토 v1 SHA `51C6E9C84A1E849767084F1AE4754CEB96B9457B3BDEDEE8D6E94A7455E635EC`의 P0=0/P1=1과 P1-001을 닫은 좁은 v2 SHA `F90D3DD29E62836AF659233E02E7C5EBF40FD8901888E59B444E28EFB07AFF8B`의 남은 P0/P1=0/0을 연결한다. v2 단독 전체검수나 실제 구현·실행 수용이 아니다.

인증된 고정 개발 root의 실제 검증된 Unreadable batch와 실제 유효 FailedBeforeCommit만 복구 가능한 출정 실패로 분류한다. 저장 미발급과 실제 결과/FailureStage/기대·확정 revision을 구별하며 불확실·기술·권위·catalog·생성자·prepared·revision 불일치·SavingReturn·이번 시도 시작 이후 오류는 Diagnostic을 유지한다. 실제 소비 token 단회 재무장→새 물리 중립/update/frame→새 retry edge→기존 Hub Destroy/actions closure→새 generation/prepared→새 중립 및 별도 새 출정 edge→실제 저장·첫 완료 view·RunStarted 순서를 적용한다. 실패 token/seed 자동 재실행·과거 중립 재사용·원문 초기화·자동 출정/복귀는0이다.

정확 변경은 FlowOwner/RunView/HubLifetimeOwner/InputRouter/ExpeditionPresenter/기존 ExpeditionPlayModeTests 여섯 파일뿐이다. REQ-C1B-SRETRY-001~008, AC-C1B-SRETRY-001~007과 기존 HUB/INPUT/RETURN/E2E/EVIDENCE·CH1 기준을 보존한다. 원래31에 정확 신규4를 더한35 집중1회·합산900초, 필수44 전체 클래스 EditMode/PlayMode 각1회·합산1800초, 방 기본 이동 선행·별도 수동12·종단4·실제 창·현소스빌드·9회·Q0/키/세대/원격 관문을 유지한다. 기존31의 잘못된 저장 실패 Diagnostic 기대만 정확 실패 상태 관측으로 바꾸며 이름·기존 차단 assertion을 삭제하지 않는다. 새 파일/조립체/참조/friend/action/callback·Profile 구현·모의 저장·반사·일반 사용자 저장 권한은0이다.

제품 쓰기는 G1/G2 r1 환경 실패의 루나 원시 검수·현재568 재결속·실행 종료와 Root→솔 여섯 파일 소유 인계 뒤다. UPM 초기화/IPC 실패를 소스 컴파일 실패로 부르거나 소비된 가져오기/저작을 자동 반복하지 않는다. 환경의 새 실행 맥락 최소 진단은 별도이며 본 제품 계약이 Unity 큰 재실행을 승인하지 않는다. 별도 실행 순서 Review는 아직 독립 판정 대상이다. 솔 구현·루나 독립 검수·아스트라 통합을 유지하며 이번 승인 작성은 제품·스테이징·Unity·컴파일·시험·프로필·원격0이다.

## 27. B1 방별 기본 이동 수용→집중검사 실행 순서 한정 승인 — Approved

전임 주 계약 §26까지 SHA `6DAADA449D01E0015CE2245C96A0FB94ADC3A8A3FE427E7B6B0F2E393385BCB1`의 원문과 §25까지 SHA `0255576B517AC315A52D9C042A5FF6C1128C946688EA0D77005D96004C2BA3E9`의 접두사를 보존한다. 아스트라는 [B1 실행 순서 보정](./2026-10-07-b1-runtime-gate-order.md) §1~5의 REQ-C1B-ORDER-001 / AC-C1B-ORDER-001을 한정 채택한다. 검수 원문 SHA `9699ED81462049417E96F6EFF45C8ED333B5E5CB01CD6559BFB7CA2E723B198A`, 루나 독립 검토 SHA `59324086AB194CDB264CC5B3377209BD92565264C698337CA6597F457A08467C`의 P0/P1=0/0 및 준비 v1 SHA `23202CC6D89159B2F11066D154B14CDE284F4CAFF62C467122F0D4F1E9491643`를 결속한다. §26 작성 당시 Review였던 순서 보정을 이 절에서 승인하며 당시 원문을 덮지 않는다.

실제 Select→Validate(true)는 정규 출정 전 여섯 방 모두의 기본 이동 수용을 요구한다. §11.4의 요약 순서는 G1/G2 가져오기·컴파일·구조유효 후보9 저작→G3 여섯 방/지원 A/B 실제 이동·독립 증거 수용·기존 API 자산 메타 결속→G4 집중검사→G5 필수 회귀→G6 별도 수동12→기존 종단/실제 창/현재 빌드로 명시한다. G3만 G4 앞으로 배치하며 §11.4/§11.5/§12와 VD-04의 풀 입장 금지, REQ-C1B-ROOM-001/REQ-ROOM-005/007 및 관련 AC·검수/수용 책임·실제 가드를 유지한다.

ORDER 보고의 검수 범위는 **기존31의 순서·선택·상한 보존**이다. §26은 별도 승인 계약 SHA `4662E2A1FBF33E6DC4AFBC33376F3B59A9A5214E05C81D5356A7C8B5933F788F`, 루나 전체 v1 SHA `51C6E9C84A1E849767084F1AE4754CEB96B9457B3BDEDEE8D6E94A7455E635EC`와 P1-001 폐쇄의 좁은 v2 SHA `F90D3DD29E62836AF659233E02E7C5EBF40FD8901888E59B444E28EFB07AFF8B`를 근거로 **기존31+승인 신규4=35**를 승인했다. 두 독립 근거를 합쳐 **G3 실제 수용 뒤 전체35를 같은 집중1회·합산900초**에 검사한다. 순서 보정은 승인35 선택을 삭제·축소하거나 추가900초를 주는 권한이 아니며 ORDER 원문을35 검수로 소급 표기하지 않는다. §26의 정확 여섯 파일·신규4·기존 시험 보정 범위를 그대로 적용한다.

G1/G2 단일 Unity 실행·합산900초, G3 회당1800초/첫 실패, G5 전체44 클래스 EditMode/PlayMode 각각1회·합산1800초와 종료/출력 각5초, G6 snapshot4×3 별도 수동12·회당1800초를 유지한다. G7 정상1/실패3종 각1·각1800초/합산7200초·별도 실제 창 키보드/포인터 각1800초, G8 빌드1회900초 및9회·Q0/키/세대/프로필/원격 관문도 유지한다. G3 실제 증거와 자산 수용 전후 지문을 G4의 현재 source/test/자산/meta/asmdef·발견 ID에 다시 결속한다. 가짜 수용·가드 완화·G3/수동12 중복집계·자동 반복·상한 증액0이다.

r1 원시 독립 검토 SHA `B12F9DE7EC3B71FECC4D20CE7B34DAB981751D2657AFF7F20E229D25D81FAC8C`는 UPM IPC 초기화 단계31.502초·exit1·시간초과 아님, source568 전후·당시 현재 불변, 후보9/meta0을 확인한다. 소스 컴파일 실패로 분류하지 않으며 G1/G2 완료0·현재 G3 수용0·G4 검사0이다. 소비된 G1/G2를 이 승인으로 반복하지 않는다. 환경 원인/맥락 진단과 새 실행 결정은 별도이고 환경 실패를 §26 제품 설계를 막는 새 일반 승인으로 확대하지 않는다. Root 소유 인계 SHA `1313AB89A11B7325B9C2D91566EDBF42D8166FAC9843EE8ACCAFE18F82F65260` 뒤 솔의 여섯 파일 구현·새 v5·쓰기 종료·루나 델타를 구별한다. C1C 공유 통합은 B1 독립 최종 수용·통합 관문 뒤이며 이 승인 담당의 제품·스테이징·Unity·컴파일·시험·수동 플레이·빌드·프로필·Q0·원격0이다.


## 28. B1 실제 최소5 수용 및 새 Unity 실행 맥락 단회 한정 승인 — Approved

전임 §27까지150379바이트/SHA `CA1E1194A1C5C059D4868BAE009BB1F619AD14C2BC92CC260390DF2B8D4C5F83`를 그대로 보존하고 아스트라 `gpt-6-astra`·`ultra`는 [새 실행 맥락 단회 재관측](./2026-10-08-b1-unity-context-reobservation.md) §6의 한정 승인을 채택한다. 검수 원문 SHA `3450AB7549418997135051D8EB11D28472F940167E109EB6A0D074F8CF5786F7`, 규범 루나 v2 SHA `47D2B7F27C8F24F37785B846A242DF588CC83C6B74A28068F01A824FF884C1B4` 및 최종 제어 도구 루나 v4 SHA `D6BF4896A778C5CF69D22194317ACDA2C87077769C46420D4D7FB4743F308B5B`의 정확 검토 범위를 보존한다. 과거 Review와 선행 미발급 표시는 작성 당시 이력이다.

현재568 원장 `A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41`의 여섯 소스 정적 제한 수용 `212F79D6CEC9AFF70BF66CB229732C8863018805F5050FBD737357A743ECC155`과 실제 최소 컴파일 수용을 구별한다. r5 null 종료코드 결과/소진을 보존하고, r6 v3 계획 `D8ED7D14476CF334B3E6D38A5ED5F1A2CE259D372AFC0D02D3756A5E1876DC53`·계획 검수 `8596AFF042FD2A2C3B27A0D46D99CB43A3CFE4112A60A7975F6F6D9152CFA508`·정확 실행 승인 `755C87A1D536A999B30863EBF76FE1B1EE1E6EBA579F0F65BF8BDF21FF923AD7`를 거친 실제5 결과 `113336B69DCBF8A013EA38A4ABCCB4065C38CFEFFBCF4B1AE0EEC013B8D4CA43`와 독립 결과 검수 `4E5C954240D23176F127B4AE506F0596C6C2D02C89CF41560A6FFA1F851D5C70`의 P0/P1=0/0을 직접 결속한다. 다섯 native 정수exit0·timeoutfalse·관리오류0·소유종료·DLL/ref10·현재568 불변만 한정 수용한다. 추가 최소 컴파일이나 실제 시험·이동·빌드 수용은0이다.

Root에게만 같은 Unity6000.6.0f1·기존 Chapter1ExpeditionAuthoringBuilder.Build·검수 helper SHA `13434CE716B56E3CD13E4098F6C689C0B9862973E7E630206E16571CA600CD70`와 준비 v4 `12685528D5E5E37AFD8038354545822676F7EA8B4FCC72DA32F4DFB5C4036DE1`로 새 실행 맥락의 가져오기·자체 컴파일·scene1/prefab8 저작 **새1회·합산900초**를 한정 허용한다. 실제 Root20핀 인계와 아스트라19명명핀/최상위7SHA, 현재 Approved 계약 및 본 주 계약의 실제 지문, 현재568·9후보/meta18 부재·새 원시·쓰기/실행 폐쇄를 실행 직전에 대조하고 `require_escalated` 정식 도구 심사를 거친다. 심사 거절 우회0, 종료 확인/출력 회수 각5초, 첫 실패 정지, 정확 원본568 변경/삭제0 및 후보18 외 소스 추가0이다. 실패/부분 산출을 보존하며 큰 실행 반복·분할·외부 UPM/IPC 주입·설정/보안/설치/캐시/인증 조작0이다.

REQ-C1B-CTX-001~006·REQ-CH1-009/010 및 AC-C1B-CTX-001~006·AC-CH1-006/007을 채택한다. r1은 UPM 초기화 실패로 소비된 역사이며 새 시도가 그 이력을 초기화하지 않는다. 실제 Unity/G1/G2는 아직 미수행·미수용이며 새 원시/정확 후보의 루나 독립 검토 뒤 한정 판정한다. §27의 G3 실제 기본 이동→집중31+4=35/900초→전체44/1800초→수동12→종단4/실제 창→현재소스빌드, 기존9회·Q0·프로필·키/세대·원격 및 B1 독립 최종 이후 C1C 공유 통합을 모두 유지한다. 이 승인 작성자의 제품·스테이징·Unity·native·시험·프로필·원격 실행은0이다.

## 29. B1 저작 장면 한 줄 순서·로그 도구 준비의 한정 승인 — Approved

전임 §28까지153755바이트/SHA BCD60C45866854C1DA7D1903607DB981E063104900D2A1DC7D763B51C109E152를 정확 접두사로 보존하고 아스트라 gpt-6-astra/ultra는 [장면·로그 한정 복구](./2026-10-08-b1-authoring-scene-and-log-recovery.md) §7의 **제품1파일 구현·도구/검사 계획 준비만** 채택한다. 검수 원문 1D2E70D1FA8FE2277F31A2E1E88308AB6E38EBFBFD904BFB8481C5F344906A36, 준비 4E07249F4A9E5116E0B429252D063C2DF3ED71E145E1670DFBF286AC72E25463, 루나 규범 검수 E248F163A4E0806FB503B25B1B3A8130BE951C6158157F8C38926C4F6B699E39의 신규 P0/P1=0/0을 결속한다. 승인된 현 계약 SHA는 10CF2D4BD5DB53499A22698721CA9E4293EA644733BCB60DF41DEA807C410E34다. 기존 §28 실행 승인은 r2로 소진됐으며 이를 재사용하거나 실패 원문을 바꾸지 않는다.

현재568v5 A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41 중 정확 Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs의 Build에서 기존 NewScene/name 한 문장을 입력 null 검사 뒤·regular Validate 바로 앞으로 이동한다. 최종 원인 7606C69CF0B7BA097335AE6DFEC09A8C026022E8DD8140E0D7D64E10FCBC4536를 근거로 하며 기존567·원본 graph/meta·검증 조건·저장 경로·API/friend/메뉴·런타임/입력/저장/프로필을 보존한다. Root→솔 정확 단일 쓰기 인계 뒤 구현하고 새568/정확 델타·루나 독립 검수·아스트라 소스 한정 판정을 거친다.

Root의 별도 r3 로그 도구/계획 준비와 새 Editor1 최소 계획 준비는 가능하나 **새 native/Unity 실행 권한은0**이다. 최소 영향 원장 EF19562D844B26F331CBF2E1DC8929FEE56AF3F77F31AA3A87AAF8F8E9629D6E에 따라 기존 r6 Editor RSP 출력 두 경로만 바꾸고 기존 입력/참조를 독립 검토한다. r6 실제5와 새 최소1은 구별하며 다른4/전체5 반복0이다. 직접 소비자 Combat.Unity.EditMode.Tests의 변경 후 컴파일/시험 수용으로 확대하지 않는다. 최소1회60초와 새 Unity1회900초는 각각 정확 계획/도구/입력·독립 검수 및 별도 실행 승인 이후의 후보이며 이번에 발급하지 않는다.

r2 원시7/end 부재/native exit=null/도구 exit1·로그 return1과 소진을 보존한다. r2 독립 검수 216322C5BDA0394C61ED3D1C3480E694A5870ADFBC5F981B0B471C9F3CE82E3F의 로그 직렬화 P1-001은 새 helper 독립 검수 전 미폐쇄다. 네 변수명 치환 재현 성공을 실제 Unity 종료 성공으로 대신하지 않는다. 미래 source/결과/helper 검수 SHA는 null로 유지하고 정확20핀·승인19핀/최상위7SHA 직접 결속과 검수 원문/Approved 지문 분리를 지킨다.

REQ/AC-C1B-ASL-001~006, 기존 §26~28 및 G3 실제 이동→35/900초→44/1800초→별도 수동12→종단4/실제 창→현소스빌드, 필수9회·Q0·키/세대·프로필·원격 및 B1 실제 독립 FINAL 전 C1C 공유 통합0은 그대로다. 실행·B1 FINAL·CH1 수용0이며 승인 작성자의 제품·스테이징·GUI·native/Unity·시험·프로필·원격0이다. 현재 담당은 솔 구현, 루나 독립 검수, 아스트라 승인·통합, Root 실행 소유다.


## 30. B1 R3 G1/G2 한정 수용과 기본 이동 관측 정확7 구현 승인 — Approved

전임 §29까지157058바이트/SHA `EFA3020DB138D3434C6660A8092418B34BA76C40D6B7D9C4F6680CE5AF8258EB`의 전체 바이트 접두사를 보존한다. 실제 아스트라 `gpt-6-astra/ultra`는 [기본 이동 관측·사후 수용 계약](./2026-10-08-b1-basic-movement-observation-and-acceptance.md) §10의 세 정책 및 REQ/AC-C1B-G3OBS-001~012를 **정확7파일 구현·검사 계획 준비 한정**으로 채택한다. 계약 SHA `39C64FF77AA6F8B89EAEAD430C0BAE5B19E780F9842182F37CDA61B29E8E222F`, 솔 v3 원문 `F2425B91B5B77932650D3DB3D60B439759B7B002D51A0DE3EBA574DE0B907113`, 실제 루나/max 규범 검수 `A5C98C1F0760307E10F184CFF0796CEF9A76490F846D9D4CDF6EEDB3E93D8FCC`의 P0/P1=0/0 및 같은 초안의 추가 루나 검수 `C4CA211C1AD811684D97944ECA13A830306E7E5BF525169DB372C21FD78499E7`·P0/P1=0/0을 결속한다.

R3 실제 Root `require_escalated` 세션22335·도구exit0·native정수0·22.2815407초·관측 프로세스 폐쇄·관리오류0, 원래568 변경삭제0·후보9/meta9·현재586은 Root 원문 `AEE7E010A7AE028967EBF0ADFD9216E12C902188C0969819DD43DFB6A376D2D1`, 루나 실제 결과 `C8DEE976F72616BD103897E8009B47202836C8C123071209A9561DFF6369F985` 및 별도 직렬화 구조 검수 `9ADE6B8C7124D1FBF5B0B95F78FC3C5708BBB743B10AEFE4035B44DF27A3C678`로 확인했다. 아스트라 한정 수용 `C02BB01C6E58708E519D8CF3595218A1459B2AFA510714F8850A80D96E3F4DF0`은 G1 가져오기·컴파일과 G2 후보 소스·직렬화 구조만 수용한다. Build 반환 완료 문자열은 미관측 추론으로, helper false/null은 원문대로 남긴다. 실제 이동·Play·시험·빌드·B1 FINAL·CH1 수용0이다.

후속 현재 기준은 실제 source-after 179017바이트/SHA `532DB8F433AF9779022FDEFF5FBA1A0476CCDCA90FD660026DD223B5C72A8D94`의586 행이다. 내부 C1D5는 이전568 계보이며 현재 기준이 아니다. Root 정확7 단일 소유 인계 뒤 솔은 InputRouter/PlayerMovementController/Chapter1HazardObservation reader3와 신규 Runtime 조건부 수집기 C#/meta2·Editor 창 C#/meta2만 구현한다. 기존583 불변·기존3 delta·신규4 및 실제 완료 원장을 독립 대조한다. 미래590을 현재 파일 수로 쓰지 않는다.

정책은 Root GUI1/Play10·각1800초/전체18000초와 유한 원시, Play 전 임시 수집기1·실제 host참조1·Start/FixedUpdate210/정리 단일 writer, 이후 실제 방별 수용 뒤 prefab6 한정 소유 이전이다. 현재 native/컴파일/Unity/GUI/Play/방 수용기록 실행 권한0이며 각각 코드·도구·원시·실제 능력 및 정확 입력을 결속한 별도 실행 승인 뒤다. 기존 Runtime 생산 callback·게임 상태·Profile·Builder/validator·asmdef/friend·기존 meta 변경0이다. 기존 메모리 프로필 가드와 assembly 참조는 보존하며 새 프로필 파일 IO/검사 호출을 추가하지 않는다.

§26~29의 G3→집중35/900초→회귀44/1800초→별도 수동12→종단4/실제 창→현재 소스 빌드, 필수9회·Q0 인간 승인·키/세대·프로필·원격 및 B1 독립 FINAL 전 C1C 공유 통합0을 모두 유지한다. 역사 실행·동결 원문과 ASL10CF는 변경하지 않는다. 현재 담당은 솔 구현, 루나 독립 검수, 아스트라 승인·통합, Root 단일 실행 소유다. 이번 승인 작성자의 제품·스테이징·프로필·native/Unity·GUI·시험·빌드 실행0이다.
