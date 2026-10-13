# C1C 정상 생산 문맥 시험 접근·관측 호환보정

- 상태: **Review**. `Approved=false`. 구현·실행·통합 허가가 아니다.
- 작성: 솔 `gpt-6.1-sol/ultra`, 2026-10-08. 독립 검토는 별도 루나, 승인은 아스트라다.
- 사용자 동작: 같은 원정에서 실제 선택·동의·확정 저장 뒤 압착 또는 연대 기술을 사용하고, 봉쇄선·마지막 장면·거점 복귀까지 완주한다.
- 산출물·소유: 이번 보정의 단독 쓰기는 이 Review 계약 한 파일뿐이다. 별도 준비 원장·승인·검수 문서를 만들지 않으며 기존 권위·제품·후보·시험·조립 정의·패키지·프로필은 쓰지 않는다.
- 실제 선행: 구현은 아래 범위/인터페이스/저장/실행 배치 미결의 합본 독립 검토·아스트라 명시 승인과 B1 실제 독립 최종 수용·최종 소스 지문·정확 C1C 통합 차집합·유니티 종료 뒤다. 실제 관측형 미결은 그 뒤 별도 승인된 유한 검사 결과로 닫으며 설계 판정과 실제 통과를 구분한다.
- 검사 계획: 기존 5+6+4 선택자, 접근 권한·실제 원본 관측·실패/경계 재현·회귀/유한 실행을 대조한다. 현재 컴파일·유니티·기계 실행·장치·프로필·원격은 모두0이다.
- 종료 조건: 이번에는 이 Review 단일 파일의 쓰기 종료·새 SHA·미결 인계와 Root 소유 반환이다. 제품 종료는 미결 폐쇄·승인된 구현·루나 실제 독립 검증·아스트라 통합 수용까지다.

## 1. 입력 권위와 현재 결함

이번 읽기 시점의 주 계약 `2026-10-03-chapter1-playable-delivery.md`는 SHA `DADCF3DBC93704F3FD63B961E6807DE55C1B664668B3213C3839274472C8D153`이며 §17/22/25/27~30의 현재 승인 계보를 대조했다. 이전 본문과 독립 검수의 SHA `CA1E1194A1C5C059D4868BAE009BB1F619AD14C2BC92CC260390DF2B8D4C5F83`는 당시 §17/22/25/27을 읽은 역사 지문이다. 이전 승인·보고 원문을 현재 지문으로 바꾸지 않는다. C1C Base SHA `03067E1293E35CAC4C850BA0F68D2E0E584522141EC48E6CA2C1F2ADCCFE2725`, UIRecallClosure SHA `80CC2ED411FFC21144A68A159D2478267F4417D10B134F0FFE3735897333952D`, ClosingNeutralRead SHA `670E3F5E1AC99C2CAC0772E7A954BEEDC33A3607E7E41681C38BAEB6AC6F016A`의 Approved 범위는 유지한다. §27~30의 기본 이동·전체35/44 회귀·실제 최종 관문을 최소 컴파일로 대체하지 않는다. 작성 중 소스나 미래 자산 지문을 이 읽기 시점의 문서 지문에서 발급하지 않는다.

활성 Approved 절차 `2026-10-03-play-first-delivery-strategy.md`의 REQ/AC-PROC-011/012를 적용한다. 아래 내용은 M2 보스 이후 연결의 **한 기능 묶음 계획 후보**다. 범위·생산 소유·접근 권한·저장·실행 배치의 미결을 합본으로 판정하며, 범위 안 작은 수정마다 승인·전체 회귀·검수 문서·문서 SHA의 재귀 의존을 새로 만들지 않는다. 기존 단회·첫 실패 중단·필수 AC·B1 최종 선행은 이 절차만으로 완화되지 않는다.

기준은 C1C v5 manifest SHA `1B6A3D25BA148B2EA7C08FBE1E2183A846DA311A549759FBB1A680DA42ECB8D1`의 후보27, 아스트라 준비 SHA `A35E467B752CF01B64FEAD72E1127822591D22113CDDC0BC48A7E7CB2513E005`, 솔 설계 준비 SHA `D0570600C73053104AE20A1DCA5A76C42EBCD318879463F1CDCD65950B7CB9B3`다. 후보27의 지문 불일치0은 실행 수용이 아니다.

루나 사전 보고 SHA `A7275A00F7D1AE8D44B69494794E6E2430B68A58BD718F35AC8CAB860FE17273`는 위 솔 준비에 대한 독립 P0/P1=0/0과 네 미결을 기록했다. 아래 구체 타입·거절 관측·실제 코루틴 실행·프로필 분리 설계를 독립 검수한 보고가 아니다. 특히 이 계약은 정상 확정 뒤 소비된 원본 후보를 **진단으로 읽기**와 그 후보의 **재소비 거절**을 명시 구분한다. 정상 준비 거절 관측의 포함 여부도 후속 합본 검토 대상이다.

보정 전 이 계약 SHA `3537379AB46787D84CABDE6622FA9D067F827CEF4AEAEA1A1BE820BFAB2C38EF`와 준비 원문 SHA `FFCD4DCB947FC20054944182A9E73E8E7A02C3522AB35CB130897535D1503890`에 대한 실제 합본 루나 검수 `c1c-production-context-fixture-contract-review-luna6.v1.json` SHA `4954765F06EF870E754A262B26447930A3400F82B144718A75BACCFF95E94D1C`는 P0/P1=0/0, Review 유지, OD-PCF-001~005 전부 미결을 기록했다. 이 보정본이나 생산 구현을 검수한 것으로 확대하지 않는다.

추가 설계 입력은 아스트라 생산 소유 설계 `c1c-production-context-owner-design-astra.v1.json` SHA `7352A5DBA11504A6004C9BE7B5290A3887828132A7882FDE875FE7637F258FC8`, 선택 저장·같은 원정 재시도 초안 `c1c-choice-save-same-run-retry-contract-draft-astra.v1.md` SHA `EB7328C60196AE47FA5877873979D0A01EDED5E7047B86B8457E139F50E83849`, 그 독립 규범 검수 SHA `91D77382729242A2EDAD1E4547315046EFE09510BEA0D5AAD1AC373E721D76ED`다. 마지막 보고의 P0/P1=0/0은 설계 정합성 검수이며 기존 반대 검토 지적의 정식 폐쇄·Approved·저장 재시도 행동 수용이 아니다. Profile 저장 설계 SHA `3CB5E4D7F2F00DF0A3713526B429B047C0C8422C63D6D05A27B000D78CAD6E57`도 Review 제안이다. 이 입력들의 당시 소스·주 계약 SHA는 각 원문 시점의 역사로 보존한다.

현재 Run EditMode asmdef는 Run/Core만 참조한다. 후보 `Core/ChoiceSkillContracts.cs:16`의 `ChoicePhaseContext` 생성자는 internal이며 정상 발급자는 Transfer.Unity다. 공개 `IChapter1ChoicePhasePort`는 소비 입구이며 발급 source가 아니다. 후보 adapter는 `Chapter1ChoicePhaseAdapter.cs:332`에서 같은 driver/adapter/run/generation 및 실제 pending 문맥을 검증한다. `:319`는 확정 뒤 pending을 비우며 현재 retained reader는 없다. 실제 제품 driver `TransferSimulationDriver.cs:496`와 현재 Input 완료 읽기에는 C1C 원본 context 발급·전달 구현이 아직 없다.

순수 `ChoiceSkillSession.cs:75`는 run·pair·mode를 검사하고 세대/driver/receipt/capture 권위는 상위 생산자가 검사한다. context 생성 접근만 넓혀도 실제 저장·같은 tick Press·예약 피해·물리 접촉을 순수 Run 검사로 바꿀 수 없다. 현재 세 시험 파일은 정확15 선택자와17 Ignore이며 통과·실행0이다.

## 2. 제안 요구·추적

아래 식별자는 **제안 ID**이며 아스트라 채택 전 정식 요구/인수 기준이 아니다. 기존 AC를 삭제·축소하지 않는다.

| 제안 요구 | 제안 인수 기준 | 보존할 기존 추적 |
| --- | --- | --- |
| proposal-REQ-C1C-PCF-001: 동일5 선택자의 실제 생산 실행 접근 | proposal-AC-C1C-PCF-001: 단방향 QA 참조1·실제15 발견·진입/종료·회귀 영향 | REQ-C1C-006, REQ-C1C-UIR-006, AC-C1C-UIR-006 |
| proposal-REQ-C1C-PCF-002: 원본 기술 사실의 현재 완료 읽기 | proposal-AC-C1C-PCF-002: 실제 context/candidate/receipt/capture/view 신원·무변경·오류/옛세대 거절 | REQ-C1C-004, REQ-C1C-UIR-004/005, AC-WT-003/006, AC-C1C-UIR-004/005 |
| proposal-REQ-C1C-PCF-003: 정상·거절·경계의 유한 실제 재현 | proposal-AC-C1C-PCF-003: 아래 다섯 검사별 완료 증거, 미결/미실행은 성공0 | REQ-C1C-006, REQ-CHOICE-006/007, AC-CHOICE-001/002/004, AC-CH1-003/006, AC-WT-001/003/006, AC-UX-004 |

전체 REQ-C1C-002/004/005/006, REQ-C1C-UIR-001~006, AC-CHOICE-001~004, AC-CH1-003~007, AC-C1C-UIR-001~006 및 기존 WT/UX 경계를 유지한다.

§3.2의 재시도 후보는 원래 초안의 proposal-REQ/AC-C1C-SRETRY-001~009 및 REQ-CH1-007/009/010·REQ-CHOICE-002/003/004·AC-CHOICE-002/003/004·AC-CH1-003/006/007·AC-C1C-UIR-001/005/006을 그대로 추적한다. 이 문서가 제안 ID를 정식 채택하거나 실제 AC 통과를 추가하지 않는다. §8의 보정/검수 주기는 기존 REQ/AC-PROC-011/012를 따른다.

## 3. 정확 추가 변경 파일·접근 권한

제품 상대 루트는 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/`다. 아래 정확4는 **PCF 시험 접근·관측 호환보정의 추가 델타 후보**다. 세 C#는 현재 v5 후보에 있고 아직 제품에 통합되지 않았다. PCF4는 QA 파일/참조3과 adapter 진단 읽기1이며, 생산 연결 파일 전체나 선택 저장·재시도4의 다른 집합을 뜻하지 않는다. Closing의 기존4 검사 완성, Driver.context 발급·port 호출·retained capture 및 Sink.completed view를 이번 네 파일만으로 해결했다고 하지 않는다.

| 기존 파일 | 현재 기준 | 한정 후보 델타 |
| --- | --- | --- |
| Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs | v5 SHA `3669DE5D3F2453AB399CCD4D85F23C1485DD3ABB396B9D07CD9C69DC6696B8C3` | 기존5 이름/파일/namespace 유지, UnityTest IEnumerator 진입·정리·결과 전달 |
| Tests/EditMode/Run/AcadeGameMaker.Tests.EditMode.Run.asmdef | 제품 SHA `0499A0C50F6C4254E039F16DF346D6430A5C2079483649AE19DFF6E42703948B` | references에 `AcadeGameMaker.Run.Unity.PlayMode.Tests` 정확1 추가 후보 |
| Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs | v5 SHA `5180DB86C8E310D351379B28BF640CC4CC3897EE2C14D54619B393BFCF16C029` | 기존6 유지, 새 시험 attribute 없는 실행 도우미/장치 수명/실제 코루틴 호스트 |
| Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs | v5 351줄·SHA `2A7AF08F07C2631361BA49A1FD93F4AD45E6A7A95486765D8DFE805F38337DFC` | §5의 원본 사실 보유·읽기·무효화만 |

QA 참조는 EditMode Run→기존 Run.Unity PlayMode 시험의 한 방향이다. 역방향 참조는 현재 없고 제품 조립체는 이 시험 조립체를 참조하지 않으므로 정적 순환0이다. 전체 그래프/플랫폼 실제 컴파일은 미실행이다. includePlatforms, defineConstraints, autoReferenced, TestAssemblies 및 현재 Run/Core 참조는 바꾸지 않는다.

기존 Run의 두 시험 friend, Input.Unity의 Run.Unity.PlayMode.Tests friend 및 기승인 Combat.Unity의 같은 시험 friend를 재사용한다. Base가 승인한 RunUnity 시험의 Run/Transfer.Unity/Combat.Unity/Movement.Unity/Hub.Presentation.Unity 참조 차집합은 기존 공유 통합 선행으로 남긴다. 새 Core/Transfer/Movement/Profile friend, 제품 역참조, 신규 assembly/C#/action 파일은0이다. friend는 전파되지 않는다. EditMode가 Core context 생성자 또는 Transfer Session에 접근하지 않는다.

### 3.1. 기승인 생산 연결과 아직 없는 구현

Base/UIRecallClosure/ClosingNeutralRead가 이미 승인한 생산 행동·정확 소유 입구는 유지한다. 아래 표의 경로는 기존 생산 설계의 연결을 확인하기 위한 **허용 후보 목록**이며 이 Review가 새 쓰기 허가를 발급하지 않는다. 실제 B1 최종본에 대한 정확 차집합에서 기승인18 및 기존 조립체 경계를 다시 대조한다.

| 기존 제품 경로 | 기승인 목적·한정 호출자 | 현재 공백·구현 판정 |
| --- | --- | --- |
| Runtime/Transfer/Unity/TransferSimulationDriver.cs | Base의 `BindChapter1ChoicePortForAuthoring`, 정상 `AdvanceTransferPhase` 안의 context 발급·port Prepare/Validate/Commit; UIRecallClosure의 pending/committed capture 검증과 회수 네 입구. Transfer가 발급·회수를 소유하며 Input adapter만 결속 소비한다 | 실제 `:496` 생산 경로는 아직 C1C context/port 호출0이다. 단일 capture의 원본 보유·최초 receipt 운반 신원과 등록 source의 실제 transaction 결속을 구현해야 한다. §5의 reader로 생산 슬롯을 대신하지 않는다 |
| Runtime/Transfer/TransferSession.cs | 기승인 Prepare/Validate/Recall와 UIRecallClosure의 Cancel. Driver가 자기 private Session의 실제 sealed 후보만 소비 | 호출 연결·준비 취소·소진 검증이 필요하다. 시험에서 Session의 회수 성공이나 pending을 주입하지 않는다 |
| Runtime/Input/Unity/InputRouter.cs | 기존 frozen 입력 transaction의 실제 ChoiceSkill edge/ordinal, 동일 입력 source와 최초 graph 결속 | -210의 실제 입력 순서와 -300 문서 표기 호환이 미결이다. 별도 입력 receipt/소비자나 강제 callback을 추가하지 않는다 |
| Runtime/Input/Unity/Chapter1GameplayInputSink.cs | UIRecallClosure의 `ValidateChapter1CompletedBarrierView`; 자기 router의 이번 +110 완료 원본과 실제 세 생산자 완료만 검증 | 현재 `TryReadCompleted`는 매번 새 값 view를 조합한다. 최초 완료 객체의 보유·검증이 없고 reader도 없다. 반복 boxing한 view를 원본으로 인정하지 않는다 |
| Runtime/Input/Unity/Chapter1EncounterEntry.cs, Runtime/Input/Unity/Chapter1EncounterEvidenceView.cs, Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs | 기존 Flow→Lifetime→router→Driver의 같은 entry/run/generation/firstTick·저장 pair, 비활성 최초 graph 발급과 실제 이전 graph 폐쇄 | 기술 Session·port·환경 결속과 실제 안전 복귀 전달은 기승인 목적의 후속 연결이다. 정상 최초 tick0/교체 actual tick+1을 시험을 위해 바꾸지 않는다 |
| Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs, Runtime/Run/Unity/Chapter1RunView.cs | 원본 UI token·현재 phase·실제 원정과 저장/Session, demo/save/closing 세 receipt의 결속; 표시와 완료는 원래 소유자가 발급 | choice Save 소비·같은 원정 Session 인계·실제 branch Save·Closing 연결은 미완료다. 선택 저장이나 Profile 권한을 Flow 단독 판단으로 발명하지 않는다 |

기승인 API 안의 private 보유 방식·정상 연결 순서·원본 취소 구현처럼 행동/접근권을 넓히지 않는 선택은 아스트라가 기존 Approved와 이 묶음의 정확 차집합에서 판정할 수 있다. 생산 소유 설계의 `internal void ConfigureChapter1ChoiceIdentityForAuthoring(IInputFrameCommitSource source, IChapter1ChoicePhasePort port, object runKey, long generation, SimulationTick firstTick, int choice, int consent, int skill)` 및 `internal bool ValidateChapter1CommittedInputReceipt(SimulationTick tick, object receipt)`는 **새 정확 서명 후보**다. 이를 그대로 추가하려면 생산 발급 chain·호출자·가시성을 같은 묶음에서 아스트라가 명시 채택해야 하며 기존 API가 존재하거나 이미 승인됐다고 쓰지 않는다. 전달된 run/pair 정수와 객체만 같다는 검사는 정상 저장/issuer의 진위를 발급하지 않는다.

### 3.2. 별도 새 권위가 필요한 선택 저장·재시도·완료 저장

선택 저장·같은 원정 재시도 초안의 정확4는 `Runtime/Run/Unity/Chapter1DevelopmentProfile.cs`, `Runtime/Input/Unity/InputRouter.cs`, `Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs`, `Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs`다. adapter/Router 경로가 다른 집합과 겹치므로 파일 수를 더해 전체 변경 수로 주장하지 않는다. 이 네 파일의 재시도 행동은 PCF4·G3 실제 이동·Closing 전용 중립 읽기 승인에서 빌려 오지 않는다.

- Profile은 C1C 기존 정확 변경표에 없던 **기존 파일1의 추가 범위 후보**다. `SaveChapter1ChoiceForRun(int choice, int consent, int skill, long expectedRevision)`와 `RetryChapter1ChoiceForRun(long expectedRevision)`의 실제 원문/같은 후보·개정·공개 보존·저장 경계는 새 승인 대상이다. Profile Core/schema/friend·새 저장 root·V2 변경은0이다.
- Router의 일반 선택 실패 뒤 물리 중립 읽기, adapter의 실패 하한/현재 token, Flow의 최초 Observe 전부터 같은 Profile을 유지하는 저장 단일 소유는 새 생산 행동·접근 경계다. 같은 UIOnly cohort·실제 저장 실패 뒤 전체 bound control 해제→나중 frame/update의 중립→그 뒤 실제 새 Submit/Click edge를 검증한다. Closing 전용 읽기나 G3 관측용 입력을 재시도 proof로 쓰지 않는다.
- 현재 `Chapter1DevelopmentProfile`은 기존 pair/CompletedBranches를 보존하는 출정·실패복귀 입구만 있다. `SaveChapter1CompletedBranchForRun(ProfileBranch branch, long expectedRevision)`는 같은 기존 파일 안의 **추가 완료 저장 후보**이며 재시도4 초안만으로 채택되지 않는다. 실제 성공 RunEnd/demo→한 번의 branch Save→원본 BranchSaveReceipt→Ended/Closing 순서를 기존 C1C 목적에 결속하는 서명·caller·오류 전파를 묶음 승인에서 명시해야 한다. 별도 ChapterCompletionSaving·새 지속 완료 필드는0이다.

이 Review의 현재 추가 제품 작성은0이다. 위 후보를 정확 통합 집합에 넣을 때 파일의 현재/후보 상태·기승인 목적·새 승인 델타를 구분하여 한 소유 계획으로 닫는다.

## 4. 실행 도우미와 도메인 경계

기존 Barrier 시험 파일 안의 제안 공개 서명은 `public static IEnumerator RunProductionContextScenario(Chapter1ContextFixtureCase caseId)`다. 같은 파일의 공개 enum은 `PairSave, RecallBoundary, SolidarityTransfer, ReservationLifetime, AuthorityRejection` 정확5 값이다. 이 서명과 타입은 현재 없고 승인 전 호출0이다. 성공 bool·Save 결과·context를 반환하는 factory가 아니다.

기존 다섯 UnityTest의 최상위 본문은 직접 `yield return new EnterPlayMode()`를 수행하고 진입 후에만 도우미를 호출한다. 정상 종료는 같은 최상위 본문의 ExitPlayMode다. 실패 시 같은 기존 클래스의 UnityTearDown에서 실행 모드를 종료한다. 중첩 도우미/장치 Setup/TearDown은 도메인 재로드·Enter/Exit·컴파일을 수행하지 않는다. 재로드 전 런타임 객체를 만들거나 static에 보유하지 않는다.

로컬 manifest 요청은 test-framework1.6.0, lock의 실제 해결은1.8.0/`source=builtin`이다. 설치 Unity6000.6.0f1 내장 package.json SHA `8C055B04C04E7DD29DDB791618EB1405E6C26BEE0BD7D0AF7B153BD69C2EEFF5`와 현 캐시 package.json SHA `BD9AE99578AA9560DCAE66274FCC0BD1ACDA9C4E549CCDE8E4DCD3150F32BB0D`도1.8.0이다. 자동 다운그레이드/패키지 변경/환경 무효 판정0이다. 실제1.8 Enter/Exit 소스는 진입·종료를 지원하며 `BeforeAfterTestCommandBase.cs:189/281`는 중첩 enumerator의 domain reload 미지원을 명시한다.

EditModeRunner는 Editor 갱신으로 iterator를 소비한다. EnterPlayMode 뒤 중첩 WaitForFixedUpdate가 실제 플레이 코루틴 대기가 된다고 가정하지 않는다. 도우미는 같은 QA 파일 안의 runtime MonoBehaviour 코루틴 호스트에서 실제 장치 시나리오를 실행하고, EditMode 본문은 `yield return null`로 그 실행의 완료/원본 예외를 확인한다. 호스트는 실제 Unity coroutine/WaitForFixedUpdate만 사용하며 FixedUpdate·PlayerLoop·Physics.Simulate·생산 Advance를 직접 호출하지 않는다. 장치 setup은 진입 뒤, teardown은 실제 graph/actions 정리 뒤다.

현재 소스의 정확 순서는 frozen callback batch 게시→`InputRouter`의 `DefaultExecutionOrder(-210)`→`TransferSimulationDriver(-200)`→후보 Barrier Combat(-190)→실제 Movement 완료→기존 +110 완료 관측 권위→Lifetime(+200)→선택 UI 표시(+220)다. **-210은 InputRouter이며 Transfer Driver의 순서는 -200**이다. UIRecallClosure의 `-300 입력 transaction`은 기존 규범 표기이고 실제 -210과의 호환 판정은 아직 없다. 없는 -300 실행자를 기록하거나 실행순서를 고친 것으로 처리하지 않는다. 아스트라는 동일 frozen 입력 선후관계·입력 초기 실패 receipt0·늦은 -200 Commit 실패의 기존 receipt 보존/현재 완료0를 유지하는 한정 문서 호환을 이 묶음에서 명시해야 한다. 실제 제품의 실행 순서를 바꾸려면 별도 행동 영향 판정이 필요하다.

호스트의 완료/예외 필드는 **시험 실행 상태만** 나타낸다. 게임 저장·기술·통과·CH1 완료 권한이 아니다. 모든 단언은 실제 생산 객체에 수행한다. 자식 예외를 원형으로 부모 검사에 전달하고, 로그 억제·성공 bool로 대신하기·실패 후 나머지 사례 진행0이다. 중첩 IEnumerator 예외도 누락하지 않는다. 실제 원본 object/context/candidate/view 단언은 Exit 전에 끝낸다. 재로드 밖에는 스칼라 사실·입력 SHA·검사 ID만 보존하고 live authority를 넘기지 않는다.

실제 호스트 추가/장치 틱 도달·정확15 발견·도메인 재로드·정리 성공은 아직 미입증이다. 이 구체 실행 변경은 새 합본 루나 검토 대상이며 사전 보고의 정적 비순환 판정만으로 닫지 않는다.

## 5. 원본 관측 타입·retained 시점·읽기

현재 없는 아래 서명과 타입을 Review 후보로 고정한다. 위치는 같은 adapter 파일 안이며 Core 생성권을 열지 않는다.

```csharp
internal bool TryReadChapter1CompletedChoiceObservation(
    SimulationTick tick, object completedView,
    out Chapter1ChoiceObservation observation);
internal enum Chapter1ChoiceObservationDisposition
{
    PrepareRejected = 1, ValidationRejected = 2, Committed = 3
}
```

`Chapter1ChoiceObservation`은 adapter 안의 internal sealed 불변 진단 타입이다. 읽기 전용 `Source`(실제 adapter), `Context`(실제 ChoicePhaseContext), `Candidate`(실제 ChoicePhaseCandidate 또는 null), `Result`(실제 ChoicePhaseResult? 또는 null), `Disposition`, `CompletedView`(전달받아 검증한 원본 object) 정확6 속성이다. 생성은 해당 adapter의 읽기 구현만 한다. 정상 context/candidate/save/완료를 발급하지 않으며 외부가 만든 진단 객체를 권한으로 받는 함수도 없다. Context의 실제 Driver/Adapter/GlobalReceipt/Capture/RunKey/Generation/Tick/Revision/Target/Edge/Pair를 값 복사로 대체하지 않는다.

같은 파일의 private source record 하나가 마지막 실제 시도의 context/candidate/result/disposition을 보유한다. 저장 지점은 아래와 같다. 행번호는 현재 v5 앵커이며 후속 구현의 정확 diff가 필수다.

| 앵커 | 한정 신규 관측 변경 |
| --- | --- |
| private pending 선언 부근 :87 | private source record 필드1·불변 내부 자료형 |
| PrepareChoice :272~288 | Current가 실제 pending 신원을 검증한 이후만 이번 context를 기록. Session/환경 준비 거절은 PrepareRejected·result null. 실제 candidate 발급 뒤 Validate 거절은 ValidationRejected·actual candidate·result null |
| CommitChoice :312~324 | 회수→환경→Session 확정이 모두 실제 반환한 후만 Committed 및 실제 반환 result를 기록. Commit 진입 중/예외는 읽기 불가, 기술/전역 완료를 합성하지 않음 |
| CancelPrepared :299~305 | 동일 실제 시도 준비 취소만 거절 사실로 유지. 외부 취소는 유효 관측/pending을 지우지 않음 |
| 새 reader1 | 아래 현재 완료 가드 전부가 참일 때 원본을 담은 불변 진단을 읽음 |
| OnDisable :349 | source record 무효화, retired/technical block은 읽기false |

현재 이 변경의 실제 신규 제품 행수는0이다. 자료형·가드·기존 네 메서드의 관측 표시를 합친 구현 예상은 비공백 약90~120행이며 실행된 구현 지문이 아니다. 후속 솔은 독립 검토 전에 추가/수정 행수와 정확 diff를 계산한다. 관측을 위해 더 많은 소스 소유·서명·필드 또는 별도 Update/FixedUpdate가 필요하면 이 네 파일 범위를 임의 확대하지 않고 아스트라로 돌아간다.

생산 원본의 선행은 adapter 진단 record와 별개다. 현재 `InputFrameCommitReceipt`와 `Chapter1EncounterEvidenceView`는 값 형식이고 Driver의 `_latestCaptureReceipt`도 값 형식 영수증이다. 같은 값을 나중에 boxing해도 최초 object와 같은 신원이 아니다. 기존 Driver가 등록 source의 실제 pending/current receipt를 검증·소비하는 정상 경로에서 **처음 한 번 만든 운반 object를 private 슬롯에 보유**하고 Context와 후속 검증이 그 정확 객체를 계속 사용하게 하는 안이 생산 설계 후보다. source의 실제 transaction·tick·ordinal·mode/run/generation과의 추가 대조가 필요하며 운반 object를 만들었다는 사실만으로 전역 발급 proof가 되지 않는다. 외부/시험 운반 object를 정상 발급 원본으로 받는 입구는0이다.

Driver는 이번 -200에서 얻은 실제 단일 capture를 context/pending에 결속하고 성공 publication 이후에만 같은 capture를 committed 슬롯으로 읽게 해야 한다. Sink는 기존 실제 +110 완료 권위에서 등록된 세 생산자·이번 receipt를 검증한 **최초 완료 view object 하나**를 보유해야 한다. 현재 이 두 슬롯과 완료 view API는 구현되지 않았다. reader가 새 capture/view를 만들거나 매번 `TryReadCompleted` 값 결과를 boxing하여 원본을 대신할 수 없다. late fault·미완료·다음 tick·retirement는 현재 읽기false로 닫고 이전 정상 publication은 역사로만 보존한다.

reader는 out을 null로 시작하고 다음 전부를 내부에서 대조한다.

1. 구성된 같은 source/driver/router/현재 run/generation이며 retired·technical block·router fault가 아니다.
2. 요청 tick은 현재 router의 실제 receipt와 일치하고, retained Context.Tick도 같다. receipt의 원본 운반 object는 실제 Current 검증 때 받은 최초 객체로 보유한다. 값 형식 receipt를 다시 boxing하여 참조가 같다고 꾸미거나 비교하지 않는다. 현재 receipt 소유·값은 기승인 source/완료 view 검증으로 대조하며 공개 값의 일치만으로 발급 proof를 만들지 않는다. 필요한 원본 결속이 실제 생산 구현에 없으면 OD-PCF-002 미해결이다.
3. `Chapter1GameplayInputSink.ValidateChapter1CompletedBarrierView(tick,completedView)`가 자기 router의 이번 +110 원본과 현재 세 생산자 완료·등록/run/generation을 대조한다. 이 API는 기승인 후속 구현 선행이며 현재 존재한다고 하지 않는다.
4. `TransferSimulationDriver.ValidateChapter1CommittedCapture(tick,context.Capture)`가 성공한 이번 -200 원본 capture를 대조한다. 이미 소진된 pending 검증으로 완료 읽기를 대체하지 않는다. capture 재생성·Sync/재포획0이다.
5. source record는 실제 Current 검증 뒤 정상 생산 시도에서 작성했고 Prepared/CommitEntered 같은 미완료 상태가 아니다. Committed는 actual candidate/result가 모두 있어야 하고 Result.Tick/Skill/ordinal은 실제 반환 원본과 같다. 거절 disposition은 result가 없으며 성공·통과로 해석하지 않는다.
6. completedView/owner/run/generation/tick/receipt/capture가 외부·복사·옛 상태이면 false다. 늦은 fault에는 이전 정상 tick의 자료를 이번 완료로 읽지 않는다.

정상 Prepare 거절을 위 현재 완료에 결속하여 **거절 사실만** 읽는 확장은 정확히 새 검토 대상이다. 이것은 루나 사전 보고의 성공 Commit 전용 최소안보다 구체화된 제안이며 독립 재검토 전 채택되지 않는다. held 입력에 새 edge가 없어 생산 context 자체가 없으면 readerfalse이며 기본 view/ordinal/소비0을 읽는다. SaveFailed/SaveUncertain도 기술 context0이다.

읽기는 상태/receipt/통과/pending을 소비·재발급하지 않는다. 현재 완료를 읽을 때는 원래 Commit에서 소비된 candidate를 진단 사실로 포함할 수 있다. 같은 객체로 두 번째 Validate/Commit/회수를 요청하면 거절해야 한다. **소비된 후보가 관측 자료라는 이유로 재소비 가능해지지 않는다.** 다음 tick/다른 view/retirement 뒤의 옛 진단은 정상 읽기0이다. 거절 뒤 같은 tick 일반 Press가 정상 처리되어 Transfer revision이 달라질 수 있으므로 Context의 사전 revision과 최종 결과 revision을 같다고 꾸미지 않는다.

## 6. 정확 선택자·검사별 완료 증거

기존 완전한 선택자15는 아래 정확 namespace/class에 각 표의 method를 붙인 집합이다. 역사 준비 원장의 exactSelectors15와 같은 목록이며 새 TestCase/선택자를 만들지 않는다. 기존 선택자5+6+4=15, 현재 Ignore5+7+5=17곳, 서사17 ID는 서로 다른 집합이다. 현재 세 시험 파일의 실제 실행/통과·Ignore 삭제는 모두0이다. 재료가 없으면 미완료로 남기고 조건 건너뛰기·생성자/계산 결과만으로 수용0이다.

| 실행 모드·기존 파일 | 정확 namespace/class | 선택자/현재 Ignore |
| --- | --- | --- |
| EditMode, Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs | `AcadeGameMaker.Tests.EditMode.Run.Chapter1ChoiceSkillSessionTests` | 5/5 |
| PlayMode, Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs | `AcadeGameMaker.Run.Unity.PlayMode.Tests.Chapter1ChoiceBarrierPlayModeTests` | 6/7 |
| PlayMode, Tests/PlayMode/RunUnity/Chapter1ClosingPersistencePlayModeTests.cs | `AcadeGameMaker.Run.Unity.PlayMode.Tests.Chapter1ClosingPersistencePlayModeTests` | 4/5 |

최상위 EditMode5가 EnterPlayMode를 사용하더라도 선택/실행 모드를 PlayMode로 이동하거나 새 PlayMode5를 추가하지 않는다. 최종 승인된 정상 검사 구현에서 Ignore가 해소되는 행은 기능 묶음의 실제 diff로 검수하며 이 문서 보정은 행을 삭제하지 않는다.

| 기존 다섯 EditMode 검사 | 실제 완료 증거와 유한 사례 |
| --- | --- |
| PairConsentCancelAndSaveGate | 실제 prepared/출정→UI 제안·취소·새 중립/확인→수탈1/4/1·연대2/3/2. 실제 V1 snapshot/Save outcome/stage/기대·확정 revision 및 session/phase를 연결. 실패/불확실 뒤 기술0·봉쇄선0. 두 분기의 같은 저장 기준은 OD-PCF-004가 닫히기 전 완료 주장0 |
| RecallAge20_21_22AndCooldown299_300 | 실제 상자 전이 start tick/revision/capture를 얻고 age20 새 edge 거절·held21 새 발급0·실제 release 후 별도21/22 새 edge 성공을 독립 정상 시작 사례로 관측. 회수20/21·압착299/300은 실제 절대 tick·신규 전이/edge·revision/ordinal로 단언 |
| Solidarity419_420AndSameTickTransfer | 실제 연대 저장/전이/기술→419 거절·420 새 edge 성공, 전이 유지. 같은 실제 장치 update/tick의 기술+Transfer 성공은 일반 Press 소비, 기술 준비 실패는 일반 Press 유지. 같은 receipt/원본 capture와 실제 최종 publication을 기록 |
| Reservation35_36_37Hold59_60Platform149_150 | 실제 예약+35 무피해/+36 DamageRequest 단회 소비/+37 재피해0, hold+59 유지/+60 해제, platform+149 접촉/+150 소멸. 실제 Combat 사망/수명 제거 및 기존 실제 nonlethal retirement→Destroy/actions closure→새 generation에서 옛 예약/발판 효력0. HP·좌표·문·collider/modifier setter0 |
| RejectForeignCopiedStaleConsumedAndOverflow | 먼저 정상 native source의 실제 context/candidate/receipt/capture/completed view를 얻음. 외부/옛 tick·view·retired source/새세대 읽기false와 원본 재소비0. Run candidate 복사/owner/pending 음성은 아래 순수 Run 보조 경계로 분리. checked overflow·late fault는 OD-PCF-003의 실제 유한 원인이 닫히기 전 미완료 |

age21과22 성공은 각 독립 실제 전이에서 시작하며 성공21의 회수 뒤 같은 active target이라고 가정하지 않는다. cooldown 경계도 원래 target 가용성·전이 나이·새 edge 조건을 실제로 충족해야 한다. 사건 전에 장치를 설정하고 실제 callback/receipt tick을 읽는다. tick을 건너뛰거나 과거 view를 보간해35/36 등 누락 관측을 채우지 않는다.

Run의 기존 시험 friend로 **정상 생산 발급 context를 먼저 얻은 뒤** 별도 순수 Session의 실제 Prepare/Validate/Commit/Cancel 원자성을 보조 검사할 수 있다. 그 Session의 정상 pending 후보를 동일 Context/Result/prior 값으로 복사하여 원본 valid 유지·복사 거절/무소비를 검사한다. 이는 순수 Run 소유의 단회성 검사이며 새로운 gameplay session·저장 성공·driver 현재 pending·환경 통과 증거가 아니다. 실제 원정 Session 또는 Core context의 상태를 주입하지 않는다. 정상 생산 거절/수명/Save를 이 보조 결과로 대체하지 않는다. 여러 권위 조건이 함께 틀린 외부 사례를 단일 필드 경계 입증으로 과장하지 않는다.

### 실제 Save 실패의 원인 후보

실제 같은 prepared 개발 root/문서에서 공개 생산 Save를 호출한다. 기존 유효 primary를 `FileAccess.Read/FileShare.Read`로 보유하면 Observe 읽기는 가능하지만 Save의 exclusive primary validation이 실패하는 기존 B1 패턴을 재사용할 수 있다. 예상 실제 분류는 FailedBeforeCommit/ExistingPrimaryValidation이다. 기술 세션 개방0과 원본 파일/revision 및 실제 UI 실패 표시를 대조한다. 파일 잠금 해제 후의 재시도도 기존 새 중립·새 의도 관문을 유지한다.

기존 B1의 정적 잠금 패턴/시험 후보는 같은 Windows 파일 공유 방식의 영향 조사 근거로만 쓴다. B1 Approved §1과 기존 독립/구현 원장에서 Read/Read·Read/None 및 불확실 저장의 실제 OS 결과는 미관측이며, 새 C1C 선택 Save·같은 원정 retry도 실제 저장 결과를 관측하지 않았다. 실제 C1C 결과와 파일 원문은 후속 승인된 배치에서 관측한다. 재시도 초안이 채택되면 최초 Observe 전부터 같은 Flow/Profile의 내부 저장 단일 소유와 외부 writer0을 유지하고, 실제 Primary/ValidCurrentInput의 원문 전체·기대 개정과 첫 batch를 보유한다. 호출별 root lock이나 SHA가 프로세스 간 비교교환을 보장한다고 주장하지 않는다.

FailedBeforeCommit의 승인될 한정 재시도만 같은 retained 후보·같은 expectedRevision을 쓴다. 실패 이후 실제 전체 물리 해제·새 중립·그 뒤 새 edge가 선행이며, temp가 존재하면 공개 StaleTemp 보존의 실제 결과와 재관측을 대조한 뒤 Save1을 호출한다. 기존 primary/previous 교체·소유 상실·미발급/보존 오류면 ready 잠금과 진단으로 닫는다. CommitOutcomeUncertain은 같은 원정 retry0·후속 Observe/Preserve/Save0이며 앱/프로필 복구의 별도 권위를 요구한다. 실패 사전 snapshot은 UI 표시용이고 기술/저장 성공 증거가 아니다. 이 구체 재시도는 §3.2의 미승인 행동 후보이며 기존 중립 문구만으로 실행하지 않는다.

불확실 후보는 실제 출정 저장으로 유효 previous가 있는 상태에서 그 previous를 같은 읽기 공유로 보유하고 생산 choice Save의 실제 `File.Replace(temp,primary,previous)`를 실행하는 방식이다. Save 소스 :329~332는 실제 CommitCall 오류를 CommitOutcomeUncertain으로 분류한다. Windows의 backup 교체/공유 동작이 이 고정 root에서 원하는 실제 오류를 내는지는 **미검증**이다. 이 후보의 예상 결과를 구조체로 주입하거나 precommit 실패를 불확실로 바꾸지 않는다. 예상과 다른 actual outcome이면 원시 보존·해당 사례 실패 후 중단한다. 실제 결과·파일 지문·stage가 있을 때만 이 재현을 채택한다. 새 Profile friend/operation mock/ACL 변경/가짜 JSON/다른 root는0이다.

### 나머지 열 검사와 서사

기존 Barrier6은 AuthoredExtractionOneValidSkillPasses, AuthoredSolidarityOneValidSkillPasses, CounterweightTransferRecallChangesPhysics, RadiusBoundaryAndDamageRoster, NonlethalFallReturnsFreshGraphWithoutRunFailure, AtomicInputFailureAndSingleCapture다. 기존 Closing4는 UiSubmitClickHeldCancelAndDuplicate, BothBranchesCloseOnlyAfterRealPassageAndSave, RejectForgedIncompleteOrUncertainCompletion, RestartPreservesV1PairButNotChapterReceipt다. 각 원래 AC/실제 장치·물리·저장·종단 범위를 전부 유지한다. 이번 helper/reader로 열 검사 실행을 대신하거나 미완성4 Closing을 수용하지 않는다.

기존 Narrative17 ID와 UTF-8 원문/SHA `FAA385CCF3CD30B652FDCE4D8A6F81F2234E337D973D157E6F05D57CB5A3AE3E`를 보존한다. 수탈 S01→SE01→S02→S03→C01, 연대 S01→SS01→S02→S03→C01의 각60 simulation tick·실제 물리 중립/새 edge 뒤만 진행하고, 실제 저장·원본 Closing/CH1 단회 완료 뒤 C02/C03를 표시한다. 재시작은 V1 pair/CompletedBranches만 보존하고 실행 완료 receipt는 복원0이다. 양분기 루나 실제 창 완주·한국어 표시/의미·실패 재도전·시간 차≤10초와 현재 빌드 관문을 유지한다.

## 7. 승인 전 미결과 관문

| 미결 | 현재 사실·필요한 판정 |
| --- | --- |
| OD-PCF-001 실제 실행 접근 | PCF4의 단방향 참조1·최상위 Enter/Exit·runtime coroutine host/예외 수명은 새 한정 승인 대상이다. 정적 순환0·내장1.8 지원은 설계 근거이며 정확15 discovery·고정 tick·재로드·Run 회귀·player 빌드 제외의 실제 통과는 없다. 동일 selector/namespace/모드와 기존 유한 배치를 유지할 정확 구현·검사 범위를 묶음으로 판정한다 |
| OD-PCF-002 생산 발급·완료 읽기 | 기승인 Driver.context/port·recall·capture 및 Sink.completed view 연결은 현재 미구현이다. PCF4에 포함되지 않는다. §3.1의 정상 발급 chain과 최초 receipt boxing/실제 transaction 결속·최초 +110 view 보유가 선행이며, §5의6속성·거절/확정 진단·재소비 거절을 함께 검토한다. 새 서명·접근 경계와 -300 문서/-210 실제 입력 순서의 한정 호환은 아스트라가 같은 묶음에서 명시해야 한다 |
| OD-PCF-003 무주입 실패/overflow | 기존 정상 caller는 최초 tick0, 교체 actual 완료 tick+1이다. `firstTick≤int.MaxValue-600`인 authoring configurer만으로 같은 실제 Flow/issuer를 거쳐 near-max C1C에 유한 도달하지 못한다. 현재 승인된 caller/API로 tickmax의 늦은 Commit 실패를 재현할 수 있다는 근거가 없고, 준비 checked overflow/null은 늦은 확정 fault가 아니다. 실제 recall Commit 연결·유한 정상 실패 원인이 아직 없으므로 이 항목은 미완료다. previous 공유 잠금의 실제 uncertain 분류도 미검증이다. setter/reflection/ForTests·외부 파기 주입·최대 tick까지 장기 반복·가짜 결과로 닫지 않는다 |
| OD-PCF-004 프로필/양분기/재시작 배치 | Base Approved의 수동 보존·교환 절차 자체는 유지한다. 앱/Unity/저장 소유 프로세스 모두 종료 뒤 같은 고정 root의 실제 baseline을 교환해야 하므로 한 live Unity iterator에서 양분기 baseline을 복원할 수 없다. iterator가 자기 Editor를 종료한 뒤 계속 실행한 것으로 기록할 수도 없다. Root가 선택자→baseline→실행번호/기존 예산→실제 종료/새 PID→before/after 판정을 표로 배치하고 아스트라가 검사 순서/자료 결속의 정확 호환을 판정해야 한다. focus 추가 분할/재실행·live reset·미래 재시작 결과 선취는0이다 |
| OD-PCF-005 공유 선행 | B1 actual FINAL·source hash·독립 루나·정확 통합 차집합·같은 project Unity 종료 전 공유 구현0. 현재 최소5조립 컴파일·helper 도구검수는 B1 최종/실제 이동/전체검사/수용을 대체하지 않음 |

미결을 닫지 않은 이 문서의 상태는 Review다. 이 서명/타입을 정식 승인 규범 또는 존재하는 API로 쓰지 않는다. 현재 생산 API와 후보 관측만으로 부족한 부분은 이 기능 묶음의 **구체 후보 델타/실행 배치**에 모아서 판정한다. 미완료를 숨기거나 작은 원인마다 별도 계약을 자동 생성하지 않는다. 새 friend·시험 이동·성공 stub·패키지 변경으로 자동 전환하지 않는다.

OD-PCF-003의 유한 실패 설계 SHA `F362D4A3F876049C7E78C2EEED243B0FF4172936496AF0536C6BC720F7BFE855`와 독립 검수 SHA `5D12319E020292E474F7D01CEBF87E94FAB52F73D68D41F5F68270923FDFBD60`도 미결을 보존한다. 기존 `ConfigureChapter1EntryForAuthoring`의 최대값은2147483047이지만, pristine/inactive 구성과 정상 Flow 발급은 다른 선행이다. 최대 entry 숫자가 허용된다는 사실은 실제 late Commit 원인이 아니다. 새 실패 입구가 필요하면 시험 전용 권한·정확 caller/파일·무소비·유한횟수를 후보로만 명명하고 기존 금지를 우회하지 않는다.

OD-PCF-004에서 `PairConsentCancelAndSaveGate`, `BothBranchesCloseOnlyAfterRealPassageAndSave`, `RestartPreservesV1PairButNotChapterReceipt`는 기존15에 그대로 남는다. Enter/Exit·새 scene·새 graph는 실제 앱 재시작이 아니다. 실제 재시작1/600초의 before/after 원문과 새 PID/시작시각은 외부 Root 수명 제어 및 루나 판독에 결속해야 한다. 나중 실제 창/재시작 결과를 먼저 수행한 집중 검사에서 이미 관측했다고 쓰지 않는다. 두 분기를 연속 실제 프로세스로 수행하는 배치와 기존 집중 각 모드1회를 양립시키는 구체 방법·순서가 아직 정해지지 않았으므로, 선택자 삭제·focus를 두 번 수행·재시작 횟수 확대 없이 아스트라의 한정 호환이 필요하다.

기존 회귀220 입력은 통과 개수가 아니며 현행 통합본 source/test/meta/asmdef·전체 method discovery를 다시 결속한다. QA 참조 영향으로 기존 Run EditMode 전체와 두 관련 시험 조립체·제품 빌드 제외/참조 폐쇄를 누락하지 않는다. 다른 영향 회귀의 생략/증거 재사용은 루나 영향 검토·아스트라 판정 전0이다.

C1C compile1/저작1 각각1800초, 집중 EditMode1/PlayMode1 각각1800초, 회귀 EditMode1/PlayMode1 각각3600초를 유지한다. 양분기 실제 창 각1/각1800초·비치명 재시도 각1/각600초·재시작1/600초도 유지한다. 기존 더 엄격한 단회 제한이 우선하며 새 실행 예산0이다. 첫 실패/상한에서 중단·원시 log/XML/시작종료/프로세스/소스자산 지문 보존·자동 반복0이다.

고정 개발 root 보존, 실제 root 검증/불확실 저장 잠금, 필수9회·Q0 인간 승인·키·세대·원격 관문은 유지한다. 현재 이 문서 작성이 소비한 검사/실행 권한은0이다.

## 8. M2 선택→기술→봉쇄선→세령→종료 기능 묶음 계획 후보

사용자 산출물은 두 승인 분기의 실제 선택/동의·확정 저장→같은 원정 기술→실제 문/발판·봉쇄선→세령 원문 대화→분기 저장·CH1 단회 완료·거점 복귀이며, 정상 실패 뒤 허용된 새 입력 재시도와 실제 재시작 시 V1만 보존되는 동작을 포함한다. PCF는 이 전체의 시험 접근/관측 부분이다. 한 기능 묶음에서 생산 연결·저장·시험 접근의 서로 다른 권위를 분리해 같은 최종 흐름에 결속한다.

| 작업 부분·담당 | 허용 후보 범위·실제 선행 | 제출/판정할 결과 |
| --- | --- | --- |
| 생산 연결 — 한정 솔; 소유/API 판정 아스트라 | §3.1과 Base/UIRecallClosure/ClosingNeutralRead의 기승인 정확 경로/참조. 새 정확 서명·순서 호환은 먼저 묶음 채택. B1 actual FINAL·현재 통합 차집합·같은 project Unity 종료·Root 단일 작성자 인계 | 실제 issuer부터 원본 context/receipt/capture/view까지 연결, 기술 성공/정상 거절/늦은 fault·단회 회수·같은 원정 수명·Closing 완료의 구체 diff. 내부 보정은 이 범위에 누적 |
| 선택 저장/재시도 및 branch 저장 — 같은 기능 묶음의 별도 소유 한정 솔 | §3.2의 Profile1·일반 선택 입력·Flow 소유 보정 채택. 완료 저장 후보는 재시도 승인 밖의 델타로 명시. 같은 파일은 동시 두 작성자가 소유하지 않음 | 같은 실제 prepared root/Flow/Profile/후보/expectedRevision, 실제 Save/보존 원문·새 물리 입력·실패 잠금. Profile core/schema/root/friend0; uncertain 동일 원정 retry0 |
| PCF 시험/관측 — 한정 솔; 독립 검수 루나 | §3 정확4와 QA 참조1·§4 host·§5 reader의 추가 권위 채택. 정상 생산 chain이 먼저 있어야 함 | 기존15/Ignore17/서사17 보존, 실제 관측과 순수 Session 보조 분리, 정상/음성/capture1/수명·현재 완료·재소비 거절. OD003/004 미결이면 해당 AC 미완료 |
| 실행/프로필 배치 — Root; 실제 조작/독립 결과 루나 | 기존 고정 root 보존 승인·단일 저장 소유·정확 프로세스 폐쇄. OD004의 선택자/양분기/재시작 배치와 OD003 유한 원인의 묶음 호환 채택 | 실행번호별 모드·exact selectors·baseline·선행 원문·기존 count/budget·firstfailure·실제 소유 PID/시작종료·원시 결과. 다른 root/일반 profile·live baseline 교환0 |
| M2 독립 제출·최종 통합 — 루나/아스트라 | 안정된 묶음 소스/자산/시험 원문과 실제 입력 지문; 기존 실행 전 독립 차집합 관문·실제 양분기 수용 유지 | 한 묶음의 코드/서사/영향 검수와 필수 회귀·실제 완주·오류/재시작 판독을 모음. 작은 수정별 전체 검수/통합 승인 재발급0. M3 현재 사용자 빌드·9회/Q0/원격은 기존 필수 관문으로 연결 |

이 표는 담당과 델타의 계획 후보이며 현재 제품/스테이징 쓰기나 실행 인계가 아니다. 아스트라가 정상 소유/새 API·저장/시험 접근·실행 배치를 **한 합본 승인 계획**에서 닫은 뒤 범위 안 구현을 계속한다. 서로 다른 소유자가 같은 파일을 고쳐야 하면 Root가 쓰기 종료 후 순차 인계한다. 기존 구현자의 자기 독립 수용 금지는 유지한다.

### 유한 검사와 보정 조건

| 기존 검사/관문 | 유지할 횟수·시간 | 배치상 보존할 내용 |
| --- | --- | --- |
| C1C 컴파일·저작 | 각각1회/1800초 | 같은 최종 통합 입력·자산, 별도 원시 결과 |
| 집중 EditMode·PlayMode | 각 모드1회/1800초 | 총15를 EditMode5·PlayMode10으로 선택; QA host의 Enter/Exit는 새 검사 회수 아님 |
| 필수 회귀 EditMode·PlayMode | 각 모드1회/3600초 | 기존 정확220 입력의 통합본 전체 발견 메서드와 실제 영향 검수; 220은 통과 수 아님 |
| 양분기 실제 창 완주 | 분기별1회/1800초 | 동일 승인 baseline·조작 방식, 실제 한국어/서사·봉쇄선 시간 차≤10초 |
| 비치명 재시도 | 분기별1회/600초 | 실제 파기/actions 폐쇄·새 generation·같은 원정 Session/cooldown 보존 |
| 실제 프로세스 재시작 | 1회/600초 | 종료 전/후 같은 고정 root V1 pair/branch와 새 PID/시작시각, 실행 receipt 미복원 |

위 횟수/상한은 기존 보존값이며 이 문서가 새 실행을 승인하지 않는다. 더 엄격한 기존 단회가 우선한다. 현재 추가 native/컴파일/Unity/프로필 실행과 자동 재시도 예산은0이다. 실패·상한에는 남은 해당 사례를 멈추고 원시를 보존한다. 로그 침묵을 정지 판정으로 쓰지 않는다. 동일 원인의 큰 검사 반복은 원시·최소 재현·변경·새 관측 목적 없이 시작하지 않는다.

승인될 묶음 안의 비실행 정적 대조·구문/참조 점검과 내부 보정은 이어 진행하고 변경/영향을 한 작업 원장에 누적한다. 기존 단회 native 실행이 소진되면 단순 내부 수정이라는 이유로 재호출하지 않는다. 필요한 국소 재실행은 영향 입력·소비자·횟수·상한·정지 조건과 기존 호환을 처음의 실행 계획에 명시하거나 같은 묶음의 한정 판정으로 해결한다. 이 Review는 아직 그런 추가 예산을 발급하지 않는다. 전체 회귀는 안정된 기능 묶음 제출 바이트에서 수행하며 작은 보정마다 반복하지 않는다. 영향 없는 역사 수용 재사용은 루나 영향 검토·아스트라 판정 뒤 그 정확 범위만 기록한다.

### 승인 판단과 문서·지문 결속

기승인 정상 choice/Recall/Closing의 소유 연결, 최초 boxing 객체의 private 보유, 같은 단일 capture/current view의 검증 방식처럼 **승인된 행동·공개 경계를 유지하는 구현 선택**은 아스트라가 기존 Approved 안에서 판정한다. PCF4/QA 참조1/거절 진단, 새 내부 서명, Profile1 및 일반 선택 물리 중립·같은 후보 재시도, 완료 저장 wrapper, 검사 순서/프로세스 배치 호환은 별도의 명시 채택이 필요하지만 이 기능 묶음 한 번의 승인에 모은다. 그것만으로 사용자에게 매 작은 구현 결정을 다시 요청하지 않는다.

사용자의 새 판단이 필요한 실제 범위 변경은 선택/동의의 의미나 캐논, 지속 저장 스키마·일반 사용자 프로필/다른 root 접근, 불확실 저장 뒤 자동 재시도·실패 잠금 해제, 기존 필수15/9회/Q0/세대·키·원격 관문 축소, 기존 승인이 허용한 재실행 범위를 벗어난 사용자 실행/접근 예산 확대 등이다. 기존 계약이 아스트라에게 허용한 한정 재실행·호환 판정은 그 권위를 따르며 작은 구현 선택이나 그 판정마다 사용자 재승인이 필수라고 간주하지 않는다. 현재 제안은 위 실제 범위 변경을 허용하지 않는다. 구현 중 그 경계를 넘는 필요가 생기면 구체 행동·권한 차이를 먼저 제시하며 막힌 경로 밖의 충돌 없는 기승인 작업은 계속한다.

승인 개정의 식별자/정확 범위와 **실제 실행이 소비하는 최종 source/test/asmdef/자산·selector·프로필 baseline·빌드 원문 SHA**를 결속한다. 이번 §1의 SHA는 읽은 원문·역사 계보를 식별하며 보조 문서가 서로의 새 SHA를 요구하는 재귀 승인 사슬을 만들지 않는다. 기존 실제 plan/raw의 정확 핀·단회는 보존하고 필요한 호환을 한 번 명시한다. 미래 생성 prefab/meta·B1 최종 source·미발급 실행/승인 SHA는 null/미발급으로 남긴다. 현재 작성자가 동시 sourceWriter를 해싱하거나 미래 값을 발급하지 않는다.

이번 보정은 Review 단일 파일의 쓰기 종료·Root 반환으로 끝난다. 신규 제품·스테이징·다른 문서/원장·프로필 IO·컴파일·Unity·GUI·native 실행·자식 배정은0이며, OD-PCF-001~005 및 실제 인수 기준의 폐쇄를 주장하지 않는다.

## 9. 실제 저장→원본 세션→새 그래프의 생산 인계 및 유한 실패 사실 보강 후보

이 절은 앞선 Review 접두사를 보존한 추가 보강이다. 상태는 **Review / Approved=false**이며, 아래 서명·자료형·예외는 존재하거나 승인된 구현이 아니다. 같은 M2 묶음의 `proposal-REQ/AC-C1C-PCF-002/003`, REQ-C1C-004/006, REQ-C1C-UIR-004/005, AC-C1C-UIR-004/005/006을 추적한다. `proposal-C1C-PRODUCTION-HANDOFF-A`, `proposal-C1C-RECEIPT-CARRIER-A`, `proposal-C1C-LATE-COMMIT-FAULT-A`는 판정할 후보 이름이며 새 정식 요구·수용 번호를 발급하지 않는다. 현재 작성자는 구현자 조사·초안 작성만 했으며 독립 QA나 OD 폐쇄를 수행하지 않았다.

### 9.1. 실제 코드가 제공하는 것과 제공하지 않는 것

아래 생산 경로는 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/` 기준, 후보 경로는 `output/chapter1-playable-r1/staging/c1c-sol61-v5/Assets/AcadeGameMaker/` 기준이다. v5의 실제27은 소스11·서사 JSON1·meta15이며 공유 생산 수정까지 들어간 통합본이 아니다. 이 절의 행은 읽은 코드의 앵커이지 미래 구현 행수가 아니다.

| 실제 앵커 | 보강 사실과 경계 |
| --- | --- |
| 생산 `Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs:318~327` | 현재 보스 정리 뒤 `LockedPostBoss`까지 진행하며 선택 저장·봉쇄선 인계는 구현되어 있지 않다. 실제 Profile과 run key는 Flow의 private 소유다. |
| 후보 `Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs:257~269` | 현재 `CompleteChoiceSave`는 같은 endpoint·saving UI token·저장 개정을 확인하고 원본 `ChoiceSkillSession`을 만든다. 이 메서드에 전달되는 값의 검사만으로 실제 파일 Save 호출·정상 prepared Profile을 발급하지 않는다. |
| 같은 후보 `:119~127`, `:130~152` | 원본 세션의 불투명 읽기와 비활성 새 adapter의 Configure/BindUiPort/BindRunSkillSession이 있다. Flow가 같은 세션을 먼저 보관하고 Lifetime이 같은 객체를 넘기는 실제 인계가 아직 없다. |
| 생산 `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs:159~193`, `:205~223` | 기존 방/보스 생성은 비활성 staging에 구성 후 활성화한다. 이전 graph 파기와 actions 폐쇄를 실제로 기다린 뒤 완료 tick+1·generation+1을 발급한다. 새 Choice→Barrier 분기와 저장/세션 인계 검증은 현재 없다. |
| 생산 `Runtime/Transfer/Unity/TransferSimulationDriver.cs:127~137`, `:496~552` | coordinated 입력 source 등록과 실제 pending/current receipt 소비가 있다. C1C 저장 신원 구성·문맥 발급·회수 확정·현재 carrier 전달은 없다. |

Base Approved §4는 Input.Unity→Run 참조1, Run의 Input.Unity friend1, Core의 Input.Unity/Transfer.Unity friend2, Input.Unity의 Run.Unity/Run.Unity.PlayMode.Tests friend2 등을 이미 정확히 승인했다. 현재 제품에 아직 없는 기승인 차집합의 통합과 **그 목록 밖의 신규 friend 추가**를 구별한다. 이번 후보는 기승인 참조/접근 허용만 재사용하며 신규 friend·Run.Unity 역참조·Core→Run 참조·시험 assembly로의 제품 참조는0이다. 기승인 차집합을 실제 적용·컴파일했다고 주장하지 않는다.

### 9.2. 실제 Save와 세션을 보유하는 Flow의 한정 경계

`proposal-C1C-PRODUCTION-HANDOFF-A`는 새 선택을 실제 저장하는 경로에 다음 순서를 제안한다.

1. Flow가 실제 현재 Choice adapter의 원본 UI 요청을 기존 endpoint로 받아 원래 prepared Profile·같은 run key·현재 generation·현재 source·확인된 선택/동의에 결속한다. §3.2의 실제 선택 Save wrapper 후보가 채택·구현된 뒤에만 그 공개 저장 호출을 한다. 이 절이 Profile의 추가 권위를 먼저 발급하지 않는다.
2. 실제 호출이 반환한 `ProfileAtomicSaveResultV1`, 기대 revision, 저장 후 같은 `_profile.Snapshot`의 원본 progression/pair, 원본 UI 요청의 신원을 Flow의 private 기록에 보관한다. 결과 구조체를 다시 조립하거나 외부가 제공한 성공 값으로 대체하지 않는다. 원문 pair는 실제 저장 후 snapshot에서 얻은 선택/동의/기술이며 UI 표시나 전달 정수로 재발급하지 않는다. `IsExactCommit`·result/snapshot/pair의 실제 Validate를 함께 적용하고, outcome은 `CommittedFirst` 또는 `CommittedReplacement`, stage는 `None`, committed/snapshot revision은 같은 expectedRevision이어야 한다.
3. 같은 실제 결과로 기존 `CompleteChoiceSave`가 정상 반환한 뒤 `RunSkillSessionForHandoff`의 **그 원본 객체**를 Flow가 보관한다. 여기서 세션을 다시 만들지 않는다. Flow의 private 세션은 실제 `_runKey` 원본을 소유한 객체이며 선택/동의/기술도 위 원문 pair와 같다. 저장 값·pair 정수·run 문자열 값 일치만으로 정상 issuer를 발급하지 않는다.
4. Flow가 private 원본 저장 기록의 불투명 운반 객체 하나를 만든다. 이 객체는 같은 실제 호출·원래 Profile·현재 UI 요청·expected/committed revision·실제 outcome/stage·원문 pair와 세션을 보유한 Flow 기록의 신원이다. Input이나 Transfer가 이 객체의 외형을 해독하여 저장 진위를 추정하지 않는다. Flow의 pending 검증만 그 원래 기록과 대조한다. 새로운 저장·파일 읽기·Profile friend를 보태는 객체가 아니다.
5. Lifetime의 같은 pending 인계가 이전 Choice 그래프의 정상 전이 완료·실제 파기·actions 폐쇄를 기다린다. 최종 Transition 완료 원문이 생긴 뒤에만 다음 generation/firstTick을 정한다. 그래프 비활성화·새 scene·UI 표시만으로 폐쇄 성공을 선언하지 않는다.
6. Lifetime은 새 비활성 Barrier 그래프에 같은 run key, 새 generation, 실제 완료 tick+1을 발급하고 adapter의 `ConfigureForAuthoring`→`BindUiPort`→`BindRunSkillSessionForAuthoring`에 원래 endpoint와 원본 세션을 넘긴다. 이어 Driver의 기존 `BindChapter1ChoicePortForAuthoring`와 §9.4의 새 구성 후보를 호출하고 기존 router 등록을 마친 뒤 단회 인계를 소비·활성화한다.

비치명 낙하로 새 Barrier 그래프를 만들 때 Flow의 같은 세션·cooldown·skill ordinal을 승계한다. 새 graph generation과 첫 tick은 Lifetime의 실제 완료 원문에서만 나온다. 세션 재생성·cooldown 감소·같은 인계 재사용은0이며 각 교체는 새 pending 인계 객체 하나를 요구한다. 이 교체의 요청 원인은 현재 원본 source의 실제 비치명 낙하/bounds·완료 view이며 Flow의 private 인계 기록에서 최초 Choice Save 요청과 구별한다. 원래 저장 기록은 보존하되 새 Save나 새 UI 확정을 발행했다고 꾸미지 않는다. 이미 V1 pair가 저장된 정상 재시작 경로는 Base가 승인한 선택/재지급 생략을 보존한다. 그 경로는 실제 launch/prepared Profile의 원문을 Flow가 소유하는 별도 출처로 구별하며 이번 실행에서 새 Choice Save가 반환했다고 꾸미지 않는다. 새 선택 저장 인계의 성공 증거로 그 경로를 대신하지 않는다. 아래 신규 Save 인계 후보만으로 이 별도 경로의 전체 생산 연결까지 폐쇄됐다고 주장하지 않는다.

### 9.3. 정확 인계 자료형·호출자·검증 포트 후보

아래는 기존 후보/기승인 생산 파일 안에 추가할 **새 정확 서명 권위 후보**다. private 저장 기록·원본 보유·비활성 구성 순서 자체는 기승인 행동의 구현 선택이지만 이 서명을 추가하는 권위는 아스트라가 같은 M2 묶음에서 명시 채택해야 한다. 새 C# 파일이나 assembly를 만들지 않는다.

`Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs` 안에 `internal sealed class Chapter1ChoiceBarrierHandoff`를 두는 후보다. 불변 속성은 `Owner`, `Endpoint`, `Predecessor`, `PredecessorRouter`, `RunKey`, `PredecessorGeneration`, `LastPredecessorCompletedTick`, `RunSession`, `CommittedChoiceSave`, `Choice`, `Consent`, `Skill` 정확12다. 생성자 후보는 다음과 같다.

```csharp
internal Chapter1ChoiceBarrierHandoff(
    Chapter1EncounterLifetimeOwner owner, IChapter1ChoiceUiPort endpoint,
    Chapter1ChoicePhaseAdapter predecessor, InputRouter predecessorRouter,
    object runKey, long predecessorGeneration, SimulationTick lastPredecessorCompletedTick,
    object runSession, object committedChoiceSave, int choice, int consent, int skill);
```

실제 생성 caller는 `Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs`의 private 인계 경로 하나다. 기존 Run.Unity→Input.Unity 참조와 Input.Unity의 기승인 friend로 접근한다. 생성자가 값 유효성을 확인해도 token 생성 자체는 authority가 아니다. 같은 Flow와 Lifetime이 각각 보유한 현재 pending 객체의 동일 신원 검증을 통과해야 한다. `LastPredecessorCompletedTick`은 최초 선택 인계에서는 원본 UI 요청의 실제 완료 tick, 비치명 교체에서는 현재 원본 낙하 요청에 결속한 실제 완료 tick이다. Flow의 private 원인 구분과 같은 source 검증이 둘을 구별한다. 최종 전이 완료 tick 또는 새 firstTick으로 가장하지 않는다. 최종 전이 tick/receipt와 실제 closure는 Lifetime의 기존 private predecessor/closure 기록에서 따로 결속한다.

같은 `Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs`의 기존 internal `IChapter1ChoiceUiPort`에 다음 **세 메서드**를 추가하는 후보다. 구현자는 실제 Flow, caller는 같은 Lifetime뿐이다.

```csharp
bool ValidatePendingChapter1ChoiceBarrierHandoff(
    Chapter1EncounterLifetimeOwner owner, Chapter1ChoiceBarrierHandoff handoff);
void ConsumePendingChapter1ChoiceBarrierHandoff(
    Chapter1EncounterLifetimeOwner owner, Chapter1ChoiceBarrierHandoff handoff);
void RejectPendingChapter1ChoiceBarrierHandoff(
    Chapter1EncounterLifetimeOwner owner, Chapter1ChoiceBarrierHandoff handoff);
```

같은 `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs`에는 다음 **두 메서드**를 추가하는 후보다. 요청 caller는 Flow의 같은 endpoint이며, 검증은 Flow/동일 Lifetime의 정상 구성 경로만 사용한다.

```csharp
internal void RequestChapter1ChoiceBarrierReplacement(
    GameObject inactiveBarrierPrefab, IChapter1ChoiceUiPort endpoint,
    Chapter1ChoiceBarrierHandoff handoff);
internal bool ValidatePendingChapter1ChoiceBarrierHandoff(
    Chapter1ChoiceBarrierHandoff handoff);
```

검증/소비의 정확 경계는 다음과 같다.

- Flow는 `ReferenceEquals`로 자기 현재 pending handoff·endpoint·Lifetime·원본 세션·private 저장 운반 객체·원본 run key를 모두 확인한다. 실제 원래 Profile/Save/UI 기록과 outcome/stage/expected/committed revision/원문 pair를 대조하고 동일 세션의 run 소유도 확인한다. 구조체 값 복사, 같은 pair, 문자열 값 일치만으로 pending을 새로 등록하지 않는다.
- Lifetime은 같은 pending 객체·실제 현재 requester/endpoint·Choice adapter/router·현재 run/generation과 실제 완료 tick을 확인한 뒤 정상 기존 입력 권위를 통해 다음 실제 tick의 Transition을 요청한다. 숫자 tick을 강제 실행하지 않는다. 처음 요청에서는 현재 이전 graph, 폐쇄 대기 이후에는 보유한 같은 predecessor/closure 원문을 확인하므로 파기된 Unity 객체의 관리 참조를 새 객체로 바꾸지 않는다.
- 이전 graph의 실제 Transition receipt·후속 완료·등록 해제·파기·actions 폐쇄가 확인되기 전 새 Barrier 구성/발급0이다. 새 entry는 같은 Lifetime이 checked generation+1·최종 실제 완료 tick+1에서 만든다. token 생성 때 계산한 가상 미래 tick을 쓰지 않는다.
- 양측 pending은 요청→폐쇄 대기→비활성 구성/기존 등록→단회 소비→Issued 또는 Rejected로만 진행한다. 검증은 무소비다. Flow의 Consume은 자기 검증과 Lifetime의 현재 비활성 구성/등록 완료 검증 뒤 같은 객체를 한 번만 소진한다. Lifetime도 같은 객체의 소진을 기록한 뒤에만 활성화한다. 복사/외부/다른 run·generation·session·Save proof/미폐쇄·재소비는 거절한다.
- 실패/기술 봉쇄/원정 종료는 해당 pending을 Rejected로 닫고 현재 권위를 무효화한다. Reject는 같은 소유의 현재 pending만 닫으며 다른 정상 pending을 지우지 않는다. 소비 후 활성화 실패에도 인계를 되돌려 재사용하지 않는다. 생성 중 새 graph는 비활성 폐쇄하고 현재 실패를 보존한다. 원래 성공 Save/세션/이전 publication을 지우거나 저장을 되돌리지 않는다. 자동 재시도0이다.

이 포트는 Input.Unity의 타입과 불투명 객체만 사용한다. Input.Unity는 Flow의 구체 Run.Unity 타입을 참조하지 않고, Flow가 이미 참조하는 Input.Unity 포트를 구현한다. Transfer.Unity로 원본 Session이나 Profile 구체 타입을 보내지 않는다. Core/Transfer에 UI 또는 저장 객체 해독 권한을 추가하지 않는다. 기승인 friend는 조립체 단위이므로 정확 caller의 한정은 제출 diff의 호출 지점 대조까지 필요하며 선언만으로 기계적으로 제한됐다고 쓰지 않는다.

### 9.4. Driver 새 서명 총3과 등록 전/후의 두 단계

`Runtime/Transfer/Unity/TransferSimulationDriver.cs`의 **새 정확 서명 후보는 아래 총3개**다. 첫2는 §3.1의 후보이며, 제3은 +110에 최초 운반 객체를 넘길 필요를 드러낸 추가 후보다. 기존 존재/Approved API로 표기하지 않는다.

```csharp
internal void ConfigureChapter1ChoiceIdentityForAuthoring(
    IInputFrameCommitSource source, IChapter1ChoicePhasePort port,
    object runKey, long generation, SimulationTick firstTick,
    int choice, int consent, int skill);
internal bool ValidateChapter1CommittedInputReceipt(
    SimulationTick tick, object receipt);
internal object ReadChapter1CommittedInputReceipt(SimulationTick tick);
```

Configure caller는 §9.3 검증을 통과한 **같은 Lifetime의 새 비활성 Barrier 구성 경로** 하나다. source는 그 새 graph의 실제 router, port는 기존 Bind에 결속한 그 adapter 원본이며 run/generation/firstTick은 Lifetime이 발급한 entry, pair는 검증된 Flow 원문에서만 온다. Driver가 전달 정수만 검사하여 실제 저장을 인증하는 API가 아니다. 전달 숫자/객체의 정확성에 더해 정상 caller의 인계 검증이 생산 권위의 선행이다.

첫 구성은 비활성·pristine·미등록에서 한 번이다. 이때 `_coordinatedInputSource`는 아직 없으므로 등록 완료를 요구하며 호출 순서를 뒤집지 않는다. Driver는 기대 source/port/run/generation/firstTick/pair의 원본을 private 보관하고 실제 등록 전 상태로 남는다. 이후 기존 `RegisterCoordinatedInputOwner(owner,source,firstTick)`가 기존 Validate의 초기화·player next tick·owner/source 동일성·충돌 가드와 함께 **저장해 둔 source/firstTick 및 결속 port 원본**을 대조한다. owner/source는 같은 router여야 한다. Lifetime은 이 실제 등록이 완료된 경우만 인계를 소비·활성화한다. 등록 전 FixedUpdate·다른 owner/source/tick·반복 Configure·교체 run/pair는 기술 거절이며 자동 초기화로 우회하지 않는다.

기존 `BindChapter1ChoicePortForAuthoring`는 여전히 기승인 최초 결속 API다. 새 구성/검증2개를 넣는 것만으로 Flow의 저장 진위·Lifetime 인계·원본 세션·두 단계 등록·+110 운반·회수/완료 읽기가 해결되지 않는다. 원래 제안의 `ExpectedDriverReceiptIsOriginal` 같은 bool을 계산했다는 사실도 carrier가 전달됐다는 증거가 아니다. 제3 읽기 후보는 실제 성공 publication과 같은 tick의 private carrier가 있을 때 **그 정확 객체**를 반환하며, 없거나 현재 fault/retired/다른 tick이면 null이다. 소비·새 boxing·context 생성·저장 진위 발급은0이다. 새 Driver3의 사용자는 기존 Input.Unity friend를 통해 접근하며 Transfer→Input/Run.Unity 역참조나 새 friend는0이다.

### 9.5. 단 한 번 만든 receipt carrier와 현재 슬롯의 무효화

`proposal-C1C-RECEIPT-CARRIER-A`는 실제 `ConsumeCoordinatedReceipt`가 기존 pending/current/tick/ordinal/mode epoch/gate 검증을 통과한 직후, `_pendingCoordinatedReceipt`를 비우기 전에 그 실제 값 하나를 **한 번만 boxing**하여 현재 private carrier에 보유하는 생산 구현 선택이다. 같은 source의 실제 정상 transaction과 §9.4의 현재 run/generation 결속이 선행이며 외부가 만든 object나 receipt를 등록받지 않는다. run/generation/source/firstTick 경계도 같은 private 구성/등록 원본에서 검증한다.

그 carrier가 실제 발급 `ChoicePhaseContext.GlobalReceipt`에 그대로 들어간다. 정상 -200 성공 publication 뒤 제3 읽기로 반환되는 객체도 같으며, +110 Sink는 그 객체를 전달받아 제2 검증과 자기 실제 router receipt/현재 세 생산자 완료를 함께 확인한 후 최초 완료 view 슬롯에 보유한다. 이 완료 슬롯에는 view object와 그 carrier를 함께 보관하여 adapter 완료 읽기의 `Context.GlobalReceipt`와 `ReferenceEquals`를 대조한다. 값 receipt에서 다시 boxing한 객체, 매번 재구성한 완료 view 또는 공개 값의 같음을 최초 원본 신원으로 쓰지 않는다. 생산 Configure/Bind/포트 소비와 이 경로의 직접 호출자만 실제 객체를 넘기며 QA가 carrier를 생산 슬롯에 넣는 입구는0이다.

다음 tick 진입·fault·retire는 현재 carrier/context/capture/완료 읽기 슬롯만 무효화한다. 이미 Router가 발행한 receipt, 실제 input publication, 과거 성공 publication, 실제 일부 Commit 변화는 보존한다. late fault 뒤 제3 읽기 null·완료 reader false와 **이미 발행된 실제 Router receipt 보존**을 동시에 요구한다. 현재 읽기 권위의 소멸을 원본 receipt 미발행이나 역사 삭제로 바꾸지 않는다. Sync/capture1, 기존 입력 준비/확정의 소비 순서와 일반 Press 우선순위는 별도 기승인 그대로다.

### 9.6. OD-PCF-003의 산술·도달·늦은 실패 사실 보정

현재 정상 최초 tick은 Lifetime `:45`의0, 교체는 `:223`의 실제 완료 tick+1이다. 기존 구성의 `firstTick≤int.MaxValue-600`은 inactive/pristine 저작 상한이며 정상 Flow가 해당 최대값에서 시작할 수 있다는 권위가 아니다. 고정 예산·tick 강제0·상태/가짜 context/가짜 Save/모의 operation 주입0 안에서 **-200 late Commit failure를 만드는 합법적인 유한 정상 원인을 확인하지 못했다**. 실제 제품 Recall의 Prepare/Validate/Commit/Cancel 구현도 아직 없으므로 이를 전체 생산 경로의 실패 불가능 증명으로 확대하지 않는다.

F362 유한 설계의 `int.MaxValue-600`에서181tick 뒤 skill2의 `t+420`, 301tick 뒤 skill1의 `t+300`이 넘친다는 계산은 각각의 산술 제안일 뿐이다. 후보 `Runtime/Combat/Unity/BarrierCombatSimulationDriver.cs:126`은 매 tick HP 소비 전에 `checked(tick.Value+600)`을 수행한다. 전체 실제 cohort는 첫 다음 tick `int.MaxValue-599`에서 이 Combat 연산이 먼저 넘친다. 따라서 near-max 시작 caller를 새로 허용해도181/301tick 뒤의 목표 경계까지 정상 cohort가 도달한다는 근거는 없다. tick0·고정 예산·Combat 선행 상한을 그대로 둔 채 그 제안을 실제 유한 실패 원인으로 채택하지 않는다.

후보 `Runtime/Run/ChoiceSkillSession.cs:29~53`의 checked 경계는 Prepare에서 overflow를 null로 거절하고, `:60~68`의 Commit은 재검증 뒤 이미 준비한 값을 대입한다. 후보 adapter `:290`의 Prepare overflow/null·보조 Session의 산술 시험·`Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs:55`의 `Compression(int.MaxValue)`는 동적 late Commit 실패가 아니다. 실제 Profile Save의 FailedBeforeCommit/CommitOutcomeUncertain도 Barrier/context 진입 전 저장 관문이며 기술 Commit의 I/O 실패 원인으로 대신할 수 없다.

계약 판정에서는 다음 두 전제를 명시적으로 구별해야 한다. 첫째, 기존 무주입/정상 tick0/유한 예산 아래 확인 가능한 실제 정상 성공·거절·수명과 순수 산술 보조를 수행하되 생산 checked overflow 또는 late fault의 합법 원인이 없으면 해당 AC는 **미완료**로 남긴다. 둘째, AC-C1C-UIR-005의 동적 늦은 실패 관측을 이 묶음에서 반드시 수행하려면 아래처럼 정상 context를 대체하지 않는 **한정 시험 fault 권한 예외**를 아스트라가 정확히 채택해야 한다. 이는 현재 금지의 자동 해석이나 기존 Approved 안의 private 보유 선택이 아니다. 사용자 선택/캐논/저장 의미를 바꾸는 새 제품 결정으로 확대하지 않으며 현재 구현·실행 허용은0이다.

### 9.7. 단회 늦은 Commit fault의 닫힌 기술 예외 후보

`proposal-C1C-LATE-COMMIT-FAULT-A`는 다음 조건을 모두 채택하는 경우만 검토 가능한 **후보**다. 채택·컴파일·동적 수용·호출은 현재0이다. 자연 발생 실패의 재현으로 기록하지 않으며 생산 checked 산술의 overflow를 입증한 것으로도 쓰지 않는다.

| 정확 경계 | 제안 |
| --- | --- |
| 시험 파일/최상위 caller | 기존 `Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs`의 `RejectForeignCopiedStaleConsumedAndOverflow` 선택자 하나가 §4의 같은 `RunProductionContextScenario(AuthorityRejection)`를 호출한다. 새 selector/추가 Play/새 시험 파일은0이다. |
| 실제 내부 caller | 기존 `Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs`의 위 시나리오 도우미 하나만 아래 arm 후보를 호출한다. 제품의 Flow/Lifetime/UI/Editor 저작자는 호출하지 않는다. EditMode에서 adapter/Core 발급 내부로 직접 접근하지 않는다. |
| 제품 파일/추가 서명 | 기존 `Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs` 안의 `internal bool ArmChapter1LateCommitFaultForQa(ChoicePhaseContext completedContext, object completedView)` 정확1이다. §3의 PCF4 관측만 권한 밖인 추가 델타로 명시 채택해야 하며 새 friend/assembly/C# 파일은0이다. |
| 편입 조건 | 해당 필드·arm·고정 throw 지점은 `UNITY_EDITOR && UNITY_INCLUDE_TESTS` 조건 아래만 두는 후보다. 실제 기존 Input.Unity 컴파일에서 이 조건이 성립하고 같은 QA 호출자가 접근 가능한지는 미검증이다. 조건이 성립하지 않으면 이 후보는 미실행/차단이며 defineConstraints/asmdef/패키지/별도 빌드 루트를 자동 변경하지 않는다. Player에 arm/throw 코드가 없는 정확 제출 바이트와 빌드 제외 검증이 필수다. |
| 정상 원본 선행 | 실제 같은 source/driver/run/generation/session의 정상 발급 context와 같은 tick +110 완료 view를 먼저 얻는다. arm은 adapter의 실제 native retained context와 전달 객체가 같고 Sink의 current view 검증·Driver의 carrier/capture 검증을 통과한 경우만 받는다. 순수 Session·외부 진단 객체·값 복제·가짜 Save로 arm하지 않는다. 정상 문맥의 Prepare 거절 사실을 사용할 경우 성공 candidate/Commit 증거라고 쓰지 않는다. |
| 단회 범위 | arm은 같은 fixture의 실제 원정/adapter/session 하나에 최초1회, 성공 반환 뒤 재arm0이다. 다음 실제 압착 Commit의 전체 Validate가 참인 경우만 소진한다. 실제 입력의 새 edge·정상 target·실제 회수 pending이 선행이다. arm은 tick·상태·context·receipt·결과를 주입하지 않으며 일반 플레이의 다음 tick을 기다린다. 기존 선택자/집중 배치 예산이 만료되거나 source 교체/retire/다른 run·generation/첫 fault이면 소멸하고 다른 graph로 승계하지 않는다. |
| 정확 throw 위치 | 후보 adapter `CommitChoice`의 `:317` 실제 압착 `CommitChapter1RecallForChoiceSkill`이 정상 반환한 직후, `:318` 실제 environment Commit 앞이다. Commit 진입 `:314`의 전체 Validate 성공과 같은 현재 context/candidate/원본 세션을 보유한 경로여야 한다. 이미 소진된 회수 proof를 다시 Validate/Consume하지 않는다. arm을 한 번 소진한 뒤 고정 `new InvalidOperationException("C1C_QA_LATE_COMMIT_ONCE")` 하나를 낸다. 임의 delegate/operation/Save result/예외 반환값을 받지 않는다. 실제 Recall가 먼저 실패하면 그 원시 실패만 기록하고 후보 throw가 실행됐다고 쓰지 않는다. |
| 입증할 동작 | 이미 -210에 발행된 원본 Router receipt와 실제 회수 일부 효과를 보존한다. adapter/Driver/Sink는 TechnicalBlocked로 같은 tick 일반 Press·Transfer/Combat/Movement/+110 완료·passage/CH1 결과를0으로 닫는다. 현재 carrier/reader는 무효화하되 이미 발행된 receipt/역사 publication은 지우지 않는다. rollback·자동 재시도·fault를 Prepare/null로 바꾸기는0이다. |
| 실패 판정 | 정상 원본이나 압착 Commit/회수 경계에 유한 도달하지 못함, 조건부 코드 미편입, arm 거절, 다른 예외/throw 미소진, 후속 phase 진행, receipt 소실이면 해당 사례 미완료/실패로 원시 보존 후 중단한다. 추가 Play·입력 배속·강제 tick·fake context·외부 물체 파기·상태 setter·reflection·모의 operation으로 전환하지 않는다. |

이 후보의 arm은 **시험 예외 요청만** 보유한다. context/candidate/완료 view/저장 진위를 새로 발급하거나 재소비하는 권위가 아니다. 성공 arm 전에 실제 원본 검증이 필요하고, 실제 throw 지점은 정상 생산 경로의 Validate/회수 반환 뒤로 닫는다. 과거 관측 객체가 다음 tick의 정상 권위를 계속 갖는다고 해석하지 않는다. 정확 한 caller와 조건부 player 제외를 제출 diff에서 검증해야 하며 internal이라는 말만으로 생산 권한 유출0을 단정하지 않는다.

이 한 위치는 실제 Recall 일부 효과 뒤의 동적 AC-C1C-UIR-005 봉쇄/receipt 보존을 겨냥한다. 모든 Recall/Environment/Session 실패 위치, 전체 생산 checked overflow, 입력 transaction 초기 Commit/map 실패까지 시험했다고 확대하지 않는다. 특히 원래 `AtomicInputFailureAndSingleCapture`는 별도의 입력 receipt0 의무다. 후속 제안이 그 범위를 요구하면 이 단1 예외를 늘려 호출하지 않고 같은 M2 묶음의 정확 델타와 유한 예산을 먼저 판정한다. 현재 생산 checked overflow 도달 미입증과 전체 OD-PCF-003 미완료는 그대로다.

### 9.8. 이 보강의 종료와 보존 관문

기승인 private 구현 선택과 위 새 handoff 포트·Driver 총3·조건부 QA arm1의 명시 채택 필요를 구분했다. 문서에 서명을 적었다는 사실은 저장→원본 Session→Lifetime→Driver→Context/+110 신원의 생산 폐쇄가 아니다. **PCF4만으로 생산 전부 해결0, OD-PCF-002/003 수용0**이다.

OD-PCF-001의 실행 접근/조건부 편입·discovery, OD-PCF-004의 동일 V1 양분기 baseline/실제 프로세스 재시작 배치, OD-PCF-005의 B1 actual FINAL/독립 검수/정확 공유 통합 차집합을 유지한다. 기존15 selector·Ignore17·서사17은 미실행이며 삭제/통과 승격0이다. B1 actual FINAL 전 공유 제품 통합0, 현재 source/후보/툴/다른 문서/원장/프로필·실행·컴파일·Unity·GUI·자식 배정0이다. 이 Review 한 파일의 보강을 닫아 Root로 반환하며 작은 설명 보정 때문에 동결 Approved 원문 수정·새 검수보고·전체 문서 재해싱·문서 SHA의 재귀 결속을 요구하지 않는다.

## 10. 늦은 확정 오류의 동기 봉쇄·조건부 관측·기존 pair 출처 보강 후보

이번 보강은 솔 `gpt-6.1-sol/ultra`의 후속 기술 설계다. 기존 전체79187바이트/SHA `DBFF9C7EC883BCB48B092CEBD6EE070CFCB4446C63ADC1BB4122F3103DFCB3B6`를 보존하고 이 절만 추가한다. **Review / Approved=false**, OD-PCF-001~005 및 동적 인수 기준 미완료를 유지한다. §9의 정상 저장/세션→Lifetime 폐쇄→새 비활성 구성→기존 Register 원본 source 대조→단회 carrier는 후보로 유지하되, 그 서명만으로 늦은 오류의 실제 봉쇄가 구현됐다고 해석하지 않는다. `proposal-C1C-SYNCHRONOUS-LATE-BLOCK-A`, `proposal-C1C-SYNCHRONOUS-FAULT-EVIDENCE-A`, `proposal-C1C-AUTHENTICATED-PERSISTED-PAIR-A`를 같은 M2의 판정 후보로 명명한다. REQ/AC-C1C-UIR-004/005/006 및 proposal-REQ/AC-C1C-PCF-002/003을 추적하며 독립 QA·승인·제품 실행은0이다.

### 10.1. 실제 가드와 현재 연결 공백

| 읽은 실제 코드 | 판단 |
| --- | --- |
| v5 adapter `Chapter1ChoicePhaseAdapter.cs:314~324` | Commit 진입의 전체 Validate 이후 회수→환경→Session 순서다. catch는 자기 `_technicalBlocked`만 세운다. 같은 router의 `IsFaulted`를 세우는 호출은 없으므로 현재 후속 phase 봉쇄는 미구현이다. |
| 생산 Driver `TransferSimulationDriver.cs:140`, `:496~531` | FixedUpdate는 Advance를 직접 호출한다. 아직 C1C port 호출 자체가 없으며 늦은 예외 이후의 지속 봉쇄도 없다. 정상 기술 Commit 오류를 Prepare 거절/일반 Press로 처리하면 안 된다. |
| 생산 Router `InputRouter.cs:1428~1451`, `:934`, `:1724` | private `LatchFault`는 `_faulted`를 먼저 세우고 두 action map을 최선으로 닫는다. `_currentReceipt`를 지우지 않는다. 이후 입력 transaction과 callback 수용은 fault로 닫힌다. C1C adapter에서 이 private 경계로 들어가는 승인된 입구는 현재 없다. |
| 생산 Movement `PlayerMovementController.cs:498~517`, `:551~566` | 실제 source `IsFaulted` 검사는 command 소비·BeginTick·CommitTick·MovePosition 앞이다. Router를 같은 호출 안에서 fault로 만들면 후속 이동은 이 기존 가드에서 막힌다. adapter 자체 bool만으로는 막히지 않는다. |
| v5 Combat `BarrierCombatSimulationDriver.cs:102~112` | source `IsFaulted`를 실제 capture 읽기·HP Process 앞에서 검사한다. 정상 반환/새 publication 없이 자기 fault로 닫힐 수 있으나 현재 adapter에서 source fault가 전달되지 않는다. |
| 생산 Sink `Chapter1GameplayInputSink.cs:27~30`, Router `:205~207` | source fault이면 현재 완료 읽기는 false다. 오래된 publication을 현재 완료로 재사용하지 않는다. |
| 생산 Lifetime `Chapter1EncounterLifetimeOwner.cs:130~157`, `:373~374` | source fault는 Fail로 들어가 graph 비활성/파기를 수행한다. 관측을 위해 이 폐쇄를 무기한 늦추는 기능은 현재 없다. |
| 생산 Flow `Chapter1PlayableFlowOwner.cs:234`, `:358~365` | 수명 실패는 기술 진단 경로다. 실제 수용 실패 토큰·RunFailed 정리 receipt를 검증하는 게임 실패/저장/거점복귀 경로와 같지 않다. 현재 C1C Barrier 진행 자체가 미연결인 점도 유지한다. |

Unity가 한 컴포넌트에서 예외를 기록했다는 사실만으로 뒤의 다른 FixedUpdate가 자동 취소된다고 가정하지 않는다. 필요한 것은 -200의 같은 동기 호출 안에서 **등록된 실제 입력 source를 fault로 바꾸는 것**이다. 기존 Core `IInputFrameCommitSource`는 CurrentReceipt/IsFaulted 읽기만 제공한다. 이를 공개 fault setter로 확대하거나 Transfer→Input 역참조를 만드는 대신, 이미 같은 Input.Unity에 있는 adapter와 Router 사이를 좁게 연결하는 후보를 아래에 둔다.

### 10.2. 최소 동기 입구2와 Driver의 private 종료

항상 편입할 생산 동기 경계의 **새 정확 internal 메서드는 총2개**다. §9.4의 Driver 새 서명 총3과 별개이며, 새 Core 공개 interface·friend·assembly·C# 파일은0이다.

```csharp
// Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs
internal bool OwnsEnteredChapter1ChoiceCommit(ChoicePhaseCandidate candidate);

// Runtime/Input/Unity/InputRouter.cs
internal void LatchChapter1ChoiceLateCommitFault(
    Chapter1ChoicePhaseAdapter source, ChoicePhaseCandidate enteredCandidate);
```

adapter는 `:314`의 전체 Validate가 성공한 직후, 첫 회수/환경 변경 전에 같은 candidate/context/session/router/driver/run/generation/carrier를 private Commit 진입 슬롯에 고정한다. 이 슬롯은 실제 Commit의 try/catch/finally 동안만 존재한다. `OwnsEnteredChapter1ChoiceCommit`는 그 **이미 수용한 원본 객체와 진입 기록의 신원만** 읽는다. `_technicalBlocked`가 세워진 catch에서도 같은 진입을 확인할 수 있어야 한다. 새 native context를 발급하거나, Session/환경/Recall의 현재 pending을 다시 Validate/Consume하는 함수가 아니다. 정상 반환 또는 봉쇄 완료 뒤 finally에서 슬롯을 닫으며 외부/복사/옛 candidate는 false다.

Router의 새 입구 caller는 그 adapter의 Commit catch 하나다. 실제 `source.Router`가 자기 자신이고 Context.Driver가 자기 결속 Transfer이며 원본 candidate가 위 Commit 진입 슬롯에 있다는 소유 검증으로 입구를 한정한다. 이 기록은 진입 당시 `Current`와 Driver pending 검증이 실제로 성공한 문맥이다. **회수가 이미 소진된 뒤 `ValidateChoice`/Recall Validate를 재호출하여 봉쇄 허가를 판정하지 않는다.** 현재 완료용 `ValidateChapter1CommittedInputReceipt`도 늦은 실패의 봉쇄 입구 검증으로 쓰지 않는다.

이 실제 소유를 확인한 뒤 Router는 `_faulted`를 첫 비가역 봉쇄 표시로 세운다. 이후 진단의 tick/receipt 대조나 map 종료 오류가 봉쇄를 취소하지 않게 한다. 기존 private `LatchFault(InputRouterFaultStage.Disabled)`를 재사용하는 후보다. `Disabled`는 봉쇄된 입력 상태의 진단이지 -210 transaction의 최초 실패 시점이 아니며, 실제 -200 원인은 원래 예외와 Commit 진입 문맥으로 구별한다. 기존 enum 번호/진단 구조를 바꾸거나 이미 발행된 receipt를 초기 receipt0로 분류하지 않는다. action Disable/진단 읽기 중 추가 오류는 최선의 종료 사실로 남기되 원래 Commit 예외를 대체하지 않는다. `_faulted`가 이미 true이므로 map 종료가 실패해도 기존 callback 수용 가드는 닫힌다.

adapter catch는 자기 기술 봉쇄→아래 조건부 실제 실패 관측→Router 동기 봉쇄→원래 예외 재전파 순서다. 관측 오류에도 Router 봉쇄를 반드시 수행한다. 이 입구에 실제 소유가 아닌 요청은 새 정상 source를 fault로 만들 권한이 없으며 거절된다. 정상 production catch가 소유 가드를 통과하지 못하는 제출 바이트라면 봉쇄 AC는 미완료다. internal이라는 이유만으로 정확 caller 한정이 입증됐다고 쓰지 않고 제출 diff의 호출 지점을 대조한다.

Driver는 기존 Advance 안의 실제 port Commit 호출을 private try/catch로 감싼다. 예외에서는 자기 C1C 기술 봉쇄를 고정하고 **일반 Press/Session.Process 후속 처리·최종 reflection·새 Transfer publication 이전에** 원래 예외를 재전파한다. prepare/validate의 정상 null/false만 일반 Press로 이어질 수 있다. 해당 tick의 current carrier/context/capture/완료 슬롯은 무효화하되 이미 발행된 Router receipt·이미 수행한 Recall·이전 publication은 지우지 않는다. Driver에는 공개 fault API나 추가 새 서명4를 만들지 않는다.

같은 source fault가 먼저 보이므로 기존 Combat/Movement 가드는 HP Process와 motor/MovePosition 전에 닫힌다. 최소 후보에서는 Movement/Combat의 FixedUpdate를 새로 고치지 않고 이 기존 거절을 재사용한다. Unity callback 진입과 그 가드의 예외 기록은 있을 수 있지만 command·receipt 소비/HP Process/motor tick/MovePosition/새 완료 publication은0이어야 한다. 해당 예외를 정상 phase 성공으로 계산하지 않고 QA가 지정 late 예외와 뒤의 봉쇄 진단을 구별한다. Combat은 기존 catch에서 자체 fault가 되고 Lifetime은 같은 수명 경계에서 graph를 닫는다. Driver의 private 기술 봉쇄는 자기 후속 Advance 전에 확인하며 새 성공 outcome으로 대체하지 않는다. 실제 `Chapter1RegularInputPlayModeTests.PartialCommitBlocksAllPhases:46~55`가 요구하는 직접 호출 거절·초기 receipt0·tick 무진전을 약화하지 않는다. 새 Movement/Combat setter·fault interface·FixedUpdate·PlayerLoop 제어·강제 실행은0이다. 불필요한 반복 callback을 피하려고 기존 private FixedUpdate의 반환 행동까지 바꿔야 한다면 그 추가 영향은 이 최소안 밖의 같은 M2 기술 델타로 명시 판정한다.

### 10.3. 기존 Lifetime 폐쇄를 보존하는 동기 실패 관측 후보

진단 편의를 위해 기술 오류가 난 graph를 살아 있게 보존하는 방식을 우선 채택하지 않는다. UIRecallClosure §5.6의 TechnicalBlocked는 해당 tick/후속 phase/Press/완료0·부분 변화의 사실 보존·자동 재시도0이지, 현재 graph의 파기를 무기한 보류하거나 사용자에게 새 대사/게임 상태를 만드는 권위가 아니다. 현재 Lifetime Fail/Destroy 및 Flow의 기술 진단 전이를 유지하면서 **그 폐쇄 전에 같은 catch에서 실제 값을 읽어 둔다**. 실제 객체 파기는 rollback이 아니며, 확보한 실패 기록도 부분 변화가 정상 완료됐다는 권위가 아니다.

조건부 진단의 **추가 정확 internal 읽기 서명은1개**다. §9의 정상 완료 reader와 구분하고 같은 adapter 파일 안에만 둔다.

```csharp
internal bool TryReadChapter1LateCommitFaultObservation(
    out Chapter1ChoiceLateCommitFaultObservation observation);
```

`Chapter1ChoiceLateCommitFaultObservation`은 같은 파일의 internal sealed 불변 역사 진단 후보다. 속성은 `Source`, `Context`, `Candidate`, `InputReceipt`, `TransferSnapshot`, `MotionSnapshot`, `NextExpectedMovementTick`, `BoxMass`, `BoxGravityScale`, `SkillOrdinal`, `EnvironmentHasActualSkill`, `Error`, `EvidenceError` 정확13이다. Context/Candidate는 위 실제 Commit 진입 원본, Error는 원래 예외다. Context.GlobalReceipt는 그 원본 carrier를 그대로 보유한다. 실패 기록은 실제 reader만 읽고 어떤 생산 API도 authority/pending/저장 성공으로 받지 않는다. 이것을 정상6속성 완료 진단이나 현재 capture reader의 성공으로 반환하지 않는다.

adapter는 회수 반환 뒤 예외가 난 같은 catch에서 원본 진입 tuple과 Router의 실제 `_currentReceipt`에 해당하는 값, Transfer의 실제 snapshot, 이미 존재하는 motor의 마지막 완료 snapshot/next tick, 등록된 상자의 실제 mass/gravityScale, 원본 세션 ordinal과 환경의 actual-skill 여부를 읽는다. Driver의 기존 `SnapshotForTests`/BoundRegistry, registry의 실제 TargetId/TargetCollider/TargetOwnerSink, Movement의 `TryReadExistingMotionForAuthoring`, 환경의 `HasActualSkill` 같은 기승인 읽기 경계를 재사용하는 후보다. 먼저 `TryReadExistingMotionForAuthoring`으로 같은 player의 이미 존재하는 motor를 확인하며 false이면 `EnsureMotor`를 경유하는 Snapshot/NextExpectedTick/Driver SnapshotForTests를 호출하지 않는다. 같은 동기 호출 안에서 존재가 확인된 정상 native Session/motor만 읽고, 없으면 미관측으로 닫는다. motor 초기화·재capture·Sync·물리 진행·세션 재생성은0이다. 실제 등록 상자의 collider에 결속한 body를 원본 저작/정상 전이 관측과 대조하며, 이름이 같은 임의 body를 읽어 정상 일부 효과라고 쓰지 않는다.

관측값은 nullable로 두고 실패한 읽기는 실제 EvidenceError/미관측으로 남긴다. null/default를 실제 0이나 baseline으로 채우지 않는다. 회수 후 revision/target/lock, 실제 상자 물리 값은 회수 전 정상 원본과 대조하며 결과는 **부분 효과의 사실**이다. 원래 InputReceipt와 Context.GlobalReceipt의 값·단회 carrier 신원은 이 동기 경계에서 대조하고, 봉쇄 뒤 current reader를 다시 살려 증명하지 않는다. Body/sink 결속이나 Snapshot 읽기가 입증되지 않으면 해당 부분 효과 AC는 미완료다.

필드/자료형/읽기/관측 호출은 arm/고정 throw와 같은 `UNITY_EDITOR && UNITY_INCLUDE_TESTS` 조건 아래로 한정한다. 조건부 관측 실패는 원래 exception과 동기 봉쇄를 방해하지 않는다. OnDisable/Destroy는 현재 완료 record를 무효화하되 확보한 별도 역사 진단을 새 정상 권위로 만들지 않는다. QA는 사전에 얻은 실제 adapter의 관리 참조와 그 불변 진단만 읽는다. 폐쇄 후 Unity native property를 재읽거나 다른 graph의 현재 관측으로 가장하지 않는다. 이 방식의 실제 관측·폐쇄 후 읽기·Player 제외는 미실행이다.

대안인 Lifetime 파기 지연은 현재 source fault→Fail/Destroy 전이를 바꾼다. 그것이 필요해지면 정확 늦은 C1C 표시의 읽기 서명, 지연 상태/종료 시점, 그래프/actions/원정 소유, 실제 RunFailed/저장/복귀와의 분리 및 유한 종료를 같은 M2에서 아스트라가 명시 판정해야 한다. 이번 후보에는 그 새 읽기표지나 지연 분기를 넣지 않으며 사용자 제품 선택이 추가 승인됐다고 해석하지 않는다. 기술 오류를 실제 수용 실패 토큰/RunFailed clear/SaveAfterFailure 없이 게임 실패 저장·새 Hub 복귀로 바꾸지도 않는다.

### 10.4. 단회 fault caller와 정상 문맥의 유한 접근 후보

§9.7의 조건부 단회 예외를 기존 AC-C1C-UIR-005의 정확 선택자와 결속하기 위해 **late arm의 caller를 한 곳으로 재지정하는 추가 기술 후보**를 명명한다. 앞선 §9 본문은 역사 제안으로 보존하며 이 차이를 자동 Approved로 취급하지 않는다.

- 최상위 late fault 선택자는 기존 `Chapter1ChoiceBarrierPlayModeTests.AtomicInputFailureAndSingleCapture` 하나다. 같은 QA 파일의 private `RunLateCommitFaultScenario` 도우미 한 곳만 실제 arm을 호출한다. 기존 PlayMode 집중1 안에서 실행하며 새 selector·추가 Play·공개 enum 값·새 실행 파일은0이다.
- 기존 `Chapter1ChoiceSkillSessionTests.RejectForeignCopiedStaleConsumedAndOverflow`와 공개 `RunProductionContextScenario(AuthorityRejection)`는 실제 정상 문맥/권한 거절·순수 Session 보조를 담당하며 late arm을 호출하지 않는다. 두 선택자가 각자 한 번 throw하여 단1을 두 번으로 늘리지 않는다.
- arm은 §9의 정확1 서명을 유지하고 등록된 정상 extraction 1/4/1 원정 하나에서 최초1회만 허용한다. 정상 source/Session/Profile·원문 pair가 먼저 있어야 한다. 현재 root가 solidarity pair이면 숫자를 바꾸거나 fake Save로 extraction을 만들지 않는다. 그 사례의 합법 baseline/실행 위치가 정해지지 않았으면 미실행이다.
- 실제 상자 전이를 정상 입력으로 시작한 뒤 age1~20의 실제 새 ChoiceSkill edge에서 발급된 정상 context와 PrepareRejected·실제 +110 완료 원본을 먼저 확보하는 유한 경로 후보를 쓴다. 이 경로는 실제 age20 거절 의무와 맞으며 성공 기술을 먼저 써서 상자를 제거한 다음 동일 상자를 재사용하는 제안이 아니다. 생산 Driver가 현재 실제 edge/등록/활성 target/원본 receipt·단일 capture에서 문맥을 발급하고 Session.Prepare가 기존 age 경계를 거절하는 순서가 실제 선행이다. Driver가 age<21을 문맥 발급 전에 막는 구현이라면 이 선행 문맥은 얻을 수 없고 이 후보는 차단이며 QA가 별도로 문맥을 만들지 않는다. 그 생산 순서의 정확 판정도 같은 M2에 결속한다. 실제 helper가 그 시간 창을 놓치거나 문맥/완료를 얻지 못하면 해당 사례를 닫는다. tick/state를 되돌리거나 자동 graph 재생성으로 창을 반복하지 않는다.
- 같은 current 원본을 검증한 뒤 arm하고 실제 물리 해제·새 edge와 정상 tick의 age≥21을 기다린다. 현재 등록/target/cooldown/전체 Prepare/Validate가 통과한 첫 압착 Commit만 소진한다. 정상 `CommitChapter1RecallForChoiceSkill` 반환 직후, environment Commit 앞에서 고정 `C1C_QA_LATE_COMMIT_ONCE` 예외를 낸다. 소진된 회수 proof 재검증·추가 Recall·environment/Session Commit·일반 Press는0이다.
- 실제 입력·camera/aim·상자 가용성·프레임 간 age 창·완료 reader 편입을 아직 실행하지 않았다. 위 경로는 정상 생산 발급을 사용한 유한 일정 후보이며 도달 통과가 아니다. 기존 고정 예산 안에서 목표가 안 나오면 미완료/원시 보존 후 중단한다. 장기 반복·배속·강제 tick·Physics.Simulate·Advance 직접 호출·외부 파기·모의 operation으로 전환하지 않는다.

한 late fault 기록 안에서 입력 receipt/원본 carrier 보존, 회수 부분 값, 원래 예외, Router fault/입력 차단, Driver/Combat/Movement tick 무진전·새 publication0, current capture/완료 reader false, passage/CH1 결과0, 기존 Lifetime 폐쇄와 기술 진단을 대조한다. 앞선 동기 실패 기록은 역사 증거로 쓰고 폐쇄 뒤 현재 완료를 합성하지 않는다. arm을 소진하지 못한 실제 Recall 오류나 관측 실패를 지정 throw 성공으로 계산하지 않는다.

같은 `AtomicInputFailureAndSingleCapture`의 **초기 입력 transaction 실패 receipt0** 의무는 별도다. 이 late fault에서 이미 발행된 receipt가 남았다는 결과로 초기 실패를 대체하지 않는다. production checked overflow의 실제 정상 유한 도달도 여전히 미입증이며 고정 throw의 예외 종류를 바꿔 그 산술을 증명하지 않는다. 이 caller 재지정/부분 증거 결속/단1 범위는 아스트라의 같은 M2 기술 호환 판정 전 호출0이며 전체 선택자 통과를 선취하지 않는다.

### 10.5. 실제 조건부 편입·시험 접근·Player 제외의 근거와 한계

현재 제품 캐시 `Library/Bee/artifacts/1900b0aE.dag/AcadeGameMaker.Input.Unity.rsp`는41447바이트/SHA `C3CA82DFF74CB8E0BD069F24F705A73B8353FAFF8979A0A9F29A2E4FB3FABAAC`, 최종 쓰기 UTC `2026-10-07T16:11:14.1408510Z`다. `:44`에 `UNITY_INCLUDE_TESTS`, `:133`에 `UNITY_EDITOR`가 실제 define으로 들어 있다. 같은 캐시 Run.Unity.PlayMode.Tests.rsp의 `:44/:133`에도 두 define이 있다. **따라서 읽은 Editor 컴파일 입력에서는 두 조건이 성립한다.** 그러나 해당 Input rsp의 source 목록에는 C1C adapter가 없으며 이는 과거 입력이다. 미래 adapter/arm/실패 진단의 실제 편입·컴파일·런타임 접근이 이미 통과했다고 해석하지 않는다.

실제 Input.Unity `AssemblyInfo.cs`에는 Run.Unity.PlayMode.Tests friend가 있고, 그 시험 asmdef는 Input.Unity를 이미 참조한다. 동일 QA 파일의 helper에서 internal arm/진단 reader에 접근하는 정적 경계는 존재한다. EditMode Run은 QA 단방향 참조1 후보를 통해 공개 도우미만 호출하고 내부 arm을 직접 호출하지 않는다. 새 friend·제품의 QA 참조·Core 생성권·Profile 내부 접근은0이다. 실제 selector discovery/Enter/Exit/조건부 메서드 편입은 후속 승인된 같은 제출 바이트에서 확인해야 한다.

arm/고정 throw/실패 진단과 그 QA 호출 지점은 모두 같은 두 조건으로 둘러싼다. 조건이 성립하지 않으면 해당 후보 시험을 실행 가능/통과로 보고하지 않으며 define/asmdef/패키지를 고쳐 조건을 강제로 만들지 않는다. 항상 필요한 생산 동기 봉쇄2와 Driver의 private 오류 종료는 이 시험 조건 밖에 둔다. 시험 코드가 없다는 이유로 실제 production Commit 오류의 봉쇄를 제거하지 않는다.

일반 Player는 `UNITY_EDITOR`가 없으므로 위 조건부 코드·호출·자료형이 함께 제외되는 설계다. 실제 QA assembly의 `optionalUnityReferences=TestAssemblies`와 정상 빌드 제외를 대조한다. 설치 test-framework의 `UnityEditor.TestRunner/TestBuildAssemblyFilter.cs:16~20`은 IncludeTestAssemblies 또는 playModeTestRunnerEnabled 여부로 시험 관련 assembly의 제외를 구별하며, PlayerLauncher `:242`는 시험 Player 빌드에 IncludeTestAssemblies를 추가한다. **시험 Player 빌드와 사용자 전달 Player를 혼동하지 않는다.** 일반 빌드에서 시험 옵션0·QA 역참조0·arm/throw/진단 타입 부재를 같은 실제 빌드 입력/출력으로 확인해야 하며 현재 Player 빌드/CLR 로딩/메서드 발견은0이다. 캐시 define 두 줄만으로 그 부재를 수용하지 않는다.

### 10.6. 이미 저장된 pair의 실제 재시작 출처와 인계 차이

`proposal-C1C-AUTHENTICATED-PERSISTED-PAIR-A`는 새 Choice Save 경로와 **원래 저장된 pair로 시작한 새 실제 원정**의 출처를 분리하는 후보다. 생산 `Chapter1DevelopmentProfile.cs:15~28`은 실제 prepared Hub receipt, 인증된 고정 root, 실제 Observe/Select 원문과 FinalDocument 바이트 일치를 확인한 뒤 같은 Profile의 문서를 보유한다. Flow `:125`의 이 객체 생성과 최초 snapshot을 private 출처로 보관하고 실제 출정 Save가 V1 pair를 보존한 현재 snapshot/revision과 대조한다. `ObserveBossDeath:308~311`은 그 실제 progression의 TryGetCommittedPair에서 PostBoss pair를 얻으며, PostBossProgressionSession은 실제 cleanup 뒤 BarrierPreparationRequired를 선택한다. 현재 Flow는 그 뒤 LockedPostBoss에서 멈추므로 실제 Barrier 인계는 미구현이다.

Flow의 private 출처 구분은 `CurrentRunChoiceSave`와 `AuthenticatedExistingPair` 두 가지 후보다. 전자는 §9의 실제 Choice Save outcome/stage/expected/committed revision을 갖는다. 후자는 같은 prepared Profile의 원문 pair·그 실제 출정 보존 Save·현재 revision·run key·실제 보스 cleanup을 보유하며 **이번 실행 Choice Save 결과는 null/미발급**이다. 출정 Save 결과를 Choice Save 결과로 이름만 바꾸지 않는다. 결과 구조체를 새로 만들어 성공으로 채우거나 공개 pair값만 전달하여 발급하지 않는다. 별도 Profile public API/스키마/root/friend는0이다.

이미 저장된 pair의 새 원정 세션은 Flow가 그 정상 출처와 실제 보스 cleanup을 검증한 뒤 같은 `_runKey`로 `ChoiceSkillSession`을 최초1회 만드는 private 구현 후보다. 선택 UI/기술 재지급/새 Choice Save는 생략한다. 새 원정의 초기 세션이며 이전 실행의 ordinal/cooldown/완료 receipt를 복원하는 것이 아니다. 만들어 둔 같은 세션을 Barrier 및 그 원정의 비치명 교체에 계속 전달한다. private 출처 기록의 신원·실제 Profile·현재 run·원문 pair를 기존 Flow pending 검증 포트에서 대조한다. Run.Unity의 기존 Run friend를 재사용하며 새 공개 세션 factory는0이다.

§9의 handoff는 Choice adapter를 predecessor로 받으므로, Choice UI를 생략하는 이 경로에 가짜 adapter를 만들면 안 된다. 동일 handoff 타입에 **추가 불변 속성1** `PredecessorBossPublication`과 **추가 internal 생성자 overload1**을 제안한다. 기존 정상 선택 생성자에서는 이 속성이 null이다.

```csharp
// Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs 안의 같은 handoff 타입
internal Chapter1ChoiceBarrierHandoff(
    Chapter1EncounterLifetimeOwner owner, IChapter1ChoiceUiPort endpoint,
    Chapter1BossPublication predecessorBoss, InputRouter predecessorRouter,
    object runKey, long predecessorGeneration, SimulationTick lastPredecessorCompletedTick,
    object runSession, object persistedPairSource, int choice, int consent, int skill);
```

이 overload에서는 `Predecessor`가 null이고 `PredecessorBossPublication`은 Lifetime이 실제 소유한 현재 보스 publication 원본이다. `CommittedChoiceSave`라는 앞선 불투명 속성의 저장 출처는 위 `AuthenticatedExistingPair` 기록이며 이번 Choice Save 성공을 뜻하지 않는다는 사실을 Flow의 private 출처 판별로 검증한다. 속성 이름의 일반화가 필요하면 실제 구현 전에 같은 M2 서명 델타로 판정하며 지금 접두사 서명을 이미 바뀐 승인 API로 쓰지 않는다.

Flow/Lifetime의 §9 pending 포트는 이 두 출처를 혼합하지 않는다. 기존 pair 경로는 원본 Profile/세션, 같은 run/generation/router와 Lifetime.OwnsChapter1BossPublication, 실제 cleanup/등록 정리/완료 원문을 확인한다. 마지막 실제 완료 tick은 그 predecessor에서 얻고 이후 Transition·파기·actions 폐쇄가 끝난 actual tick+1/new generation에서만 새 비활성 Barrier를 구성한다. source가 현재일 때 검증·보유한 원본 보스 publication과 이후 같은 closure 기록을 함께 대조하며 파기 뒤 새 발급으로 대체하지 않는다. Choice 출처에서 null predecessor 또는 기존 pair 출처에서 가짜 UI token/Save/adapter는 거절한다. 원래 두 단계 Driver 등록/단회 소비는 동일하며 이를 이미 구현·수용했다고 주장하지 않는다.

### 10.7. 동일15·실행 배치·종료 관문의 보존

위 후보는 동기 봉쇄의 메서드2, 조건부 실패 진단 reader1/자료형1, 기존 handoff 속성1/overload1 및 같은 기존 파일의 private 연결만 추가 명명했다. Driver 새 서명 총3·조건부 arm1은 §9대로 구분한다. Lifetime 파기 지연 입구·새 friend·assembly·C# 파일·제품 저장/대사/재시도 의미 변경은0이다. 필요한 실제 채택은 기존 M2의 정확 기술 호환 판정이며 구현/실행 권한이 아니다.

기존 EditMode 집중1/1800초, PlayMode 집중1/1800초, 실제 재시작1/600초와 동일15 이름을 보존한다. `PairConsentCancelAndSaveGate`와 `BothBranchesCloseOnlyAfterRealPassageAndSave`는 각각 한 selector 안에서 양분기 행동을 요구한다. 한 live iterator의 baseline 교환·자기 프로세스 종료 뒤 계속 실행은 불가하며, 두 모드 경계에 분기 이름만 배정해도 한 selector의 양분기/시간 검증이 충족되지 않는다. **OD-PCF-004의 실제 양분기/재시작 시행 배치는 미결**이다. 추가 분할·회수 확대·역사 증거 재사용이 필요하면 같은 M2에서 정확 실행/자료 결속을 기술 판정하되 사용자에게 새 제품 선택을 자동 요구하는 문제로 확대하지 않는다. 현재 이를 수행하거나 통과로 계산하지 않는다.

조건부 시험/생산 원본 편입과 유한 도달이 미실행인 OD-PCF-001/002/003, 위 OD004, B1 actual FINAL·독립 검수·정확 공유 통합 차집합의 OD005를 유지한다. 초기 receipt0·production checked overflow·기존15/Ignore17/서사17의 실제 증거는 별도 미완료다. 새로운 제품/스테이징/다른 문서/도구/원장/보고파일 쓰기·컴파일·Unity·GUI·CLR 로딩·저장·실행·자식 배정0, 자기 구현 독립 수용0이다. 이번 종료는 이 Review 한 파일의 접두사 보존 확인·새 길이/SHA·쓰기 닫힘 및 Root 소유 반환뿐이다.

## 11. 세 독립 지적의 최소 보정과 양분기 실행 배치 후보

이 절은 Root 실제 `gpt-6.1-sol/medium`의 동일 M2 기술 보정 초안이다. 닫힌 기존 전체107119바이트/SHA `FDE39A34ADEC9AACD8696E1D8B37E6AEFD46C653F166291EC249827C8B229540`를 바이트 그대로 보존한다. 아스트라 `gpt-6-astra/ultra`와 루나 `gpt-6-luna/max`의 읽기 반대판독은 각각 P0=0/P1=3이며, 아래가 그 세 지적에 대응하는 후보이다. **Review / Approved=false**다. 기존 REQ/AC-C1C-UIR-004/005/006 및 proposal-REQ/AC-C1C-PCF-002/003의 의미와 OD-PCF-001~005 미완료를 유지한다. 새 제품·스테이징·도구·실행·프로필 쓰기는0이다.

### 11.1. 모든 부가 호출보다 먼저 봉쇄하고 원래 예외를 유지

§10.1의 private `LatchFault` 설명은 전체 함수에서 fault가 첫 실행이라는 뜻이 아니다. 실제 함수는 먼저 `CancelUiEnableQuarantine()`를 호출하고, action owner 해석과 마지막 tick/map 진단 getter도 예외를 낼 수 있다. 따라서 §10.2의 좁은 새 입구는 검증된 실제 Commit 진입 소유를 확인한 뒤 **그 어떤 Cancel/action/getter/관측 호출보다 먼저** `_faulted=true`와 `_faultStage=Disabled`를 고정해야 한다. 소유 검증은 소진한 Recall/Session의 현재 Validate를 재호출하지 않는 진입 tuple 신원 검증이며, 외부/옛 candidate가 정상 source를 봉쇄하는 권한은0이다.

이후 기존 `LatchFault(Disabled)`를 최선으로 호출한다. 해당 함수의 Cancel/owner 해석/Disable/마지막 getter가 실패해도 이미 세운 `_faulted`는 유지하고 새 입구의 finally가 `_captureSuppressed=false`를 보장한다. 새 입구의 부가 예외는 adapter의 별도 중첩 catch에서 관측 오류로만 보유한다. adapter의 바깥 원래 Commit catch는 부가 오류 때문에 빠져나가지 않고 원래 예외를 `throw;`로 다시 전달하며, 마지막 finally에서 Commit 진입 슬롯을 닫는다. 원래 private `LatchFault`의 일반 caller 행동과 기존 enum/진단 구조를 이번 보정으로 바꾸지 않는다.

adapter catch의 정확 순서는 자기 기술 봉쇄 → 위 Router 동기 봉쇄 시도 → 별도 조건부 역사 관측 → 원래 예외 재전파다. 관측 getter가 실패하거나 일부 값이 없더라도 이미 세운 source fault와 Driver의 private 후속 종료를 취소하지 않는다. 정상 production catch에서 진입 소유 검증이 실패하여 같은 실제 source의 fault를 보장하지 못하면 봉쇄 AC는 미완료다. 관측 오류나 지정 throw만 보고 봉쇄 성공으로 판정하지 않는다. Driver는 일반 Press/Process/publication 이전에 원래 오류를 재전파하며, 같은 source의 기존 Movement/Combat/Sink 가드는 유지한다.

원래 Router receipt 값은 **전체 Validate가 성공한 Commit 진입에서, 첫 회수/환경 변경 전에** 실제 현재 원본으로 읽어 값 복사한다. 같은 진입 Context의 원래 carrier 참조를 함께 보유한다. 봉쇄 후 current reader가 false인 상태에서 새 receipt/carrier를 발급하거나 다시 boxing하여 채우지 않는다. 실제 역사 receipt와 현재 reader 무효화를 동시에 확인한다. 관측이 action 종료 뒤 이루어지는 동안 graph가 먼저 파기되면 해당 native 값은 미관측으로 남기고, 파기를 지연하거나 사후 getter로 복구하지 않는다.

### 11.2. 정확13 속성의 자료형과 파기 이후 읽기 제한

같은 adapter 파일의 조건부 `Chapter1ChoiceLateCommitFaultObservation`은 get-only 속성13을 다음 자료형으로 한정한다. 별도 자료형·공개 속성·메서드는 추가하지 않는다. 참조값의 저장은 깊은 불변성이나 현재 생산 권위라는 주장이 아니다.

| 속성 | 정확 후보 자료형 | 의미와 허용 읽기 |
| --- | --- | --- |
| Source | Chapter1ChoicePhaseAdapter | 진입 원본 관리 참조. 폐쇄 뒤 `ReferenceEquals` 신원 대조만 한다. |
| Context | ChoicePhaseContext | 실제 발급 진입 원본. 원래 carrier 참조와 결속하며 재발급/소비0이다. |
| Candidate | ChoicePhaseCandidate | 전체 Validate를 통과한 원본 candidate. 신원만 보존하고 사후 현재 Validate0이다. |
| InputReceipt | InputFrameCommitReceipt? | 진입에서 확보한 실제 readonly struct 값 복사. 미확보는 null이다. |
| TransferSnapshot | TransferSessionSnapshot? | 같은 동기 catch의 실제 readonly struct 값 복사. Session/motor 존재가 입증되지 않으면 null이다. |
| MotionSnapshot | PlayerMotionSnapshot? | 같은 player의 기존 motor에서 읽은 실제 readonly struct 값 복사. 생성/진행0이다. |
| NextExpectedMovementTick | SimulationTick? | 기존 motor의 실제 값 복사. 미관측을 default tick0으로 채우지 않는다. |
| BoxMass | float? | 그 원본 등록 collider/body의 실제 mass 값 복사. 등록 body 신원 불명은 null이다. |
| BoxGravityScale | float? | 같은 body의 실제 gravityScale 값 복사. |
| SkillOrdinal | int? | 같은 실제 세션에서 확보한 값 복사. 미관측은 null이다. |
| EnvironmentHasActualSkill | bool? | 같은 실제 환경의 관측값. 미관측을 false로 바꾸지 않는다. |
| Error | System.Exception | 바깥 catch의 원래 객체 참조 그대로. 원래 caught 객체와 `ReferenceEquals` 대조만 하며 사후 stack/message/getter 읽기로 불변 증거를 주장하지 않는다. |
| EvidenceError | string | 부가 읽기/봉쇄 오류 또는 미관측 이유를 동기 경계에서 닫은 불변 문자열. 모두 관측되면 null, 비어 있는 성공 문자열로 실패를 숨기지 않는다. 길이 최대2048문자, 초과 시 잘림 표지를 포함한다. |

`InputFrameCommitReceipt`, `TransferSessionSnapshot`, `PlayerMotionSnapshot`, `SimulationTick`은 현재 실제 Core/Transfer/Movement 값 자료형을 그대로 사용한다. 새 snapshot/receipt 생성으로 baseline을 조립하지 않는다. 먼저 `TryReadExistingMotionForAuthoring`의 같은 player/motor 존재 확인이 성공해야만 기존 실제 Snapshot/NextExpectedTick/Driver SnapshotForTests를 읽는다. 존재 확인 실패이면 Driver의 default snapshot을 실제 관측으로 기록하지 않는다. 관측을 위해 EnsureMotor/Sync/capture/Advance/Physics 실행0이다.

Unity 객체의 폐쇄 후 `== null`, native property, 이름/상태/body getter를 호출하지 않는다. Source/원본 등록 객체의 참조는 폐쇄 전에 확보한 신원만 대조한다. Error는 원래 예외 신원이며 원래 예외 재전파 증거는 실제 원시 실행 로그와 함께 확인한다. 이 record 하나로 이후 phase 무진전·입력 차단·완료0을 추론하지 않는다. 실제 기존 가드 거절, 현재 reader false, 역사 publication/receipt 보존, actual Lifetime 폐쇄/기술 진단을 별도 실제 관측과 결속해야 한다. 일부 값이나 폐쇄 이후 관측이 없으면 해당 AC는 미완료다. 정상 완료6속성 record와 현재 생산 reader로 이 역사를 반환하지 않는다.

### 11.3. 두 저장 출처의 닫힌 분기와 원본 재검증

Flow의 기존 private 저장 운반 객체에는 **private 닫힌 출처 구분**을 둔다. 값은 `CurrentRunChoiceSave`와 `AuthenticatedExistingPair` 두 가지뿐이다. 공개 setter·외부 object 등록·원문 pair 숫자로 출처 추정·Profile public API·스키마 변경0이다. 운반 객체는 원래 Profile, 실제 Save 결과/원문/revision, 같은 원정 key, 원래 세션과 실제 predecessor를 변경 불가능하게 보유한다. 이전 정상 기록을 복사한 외부 object를 현재 pending으로 발급하지 않는다.

| 출처 | 정확한 predecessor와 저장 증명 |
| --- | --- |
| CurrentRunChoiceSave | 실제 현재 Choice adapter 원본만 있고 BossPublication은 null이다. 같은 Profile의 실제 Choice Save outcome/stage/expected/committed revision 및 최종 원문 pair가 성공 결속되고, 그 Save에서 유지한 원래 RunSession을 사용한다. |
| AuthenticatedExistingPair | Choice predecessor는 null이며 Lifetime이 실제 소유한 BossPublication 원본만 있다. 원래 prepared Profile/인증 root의 보유 원문 pair와 실제 출정 보존 Save/현재 revision/같은 run key, 실제 보스 cleanup을 확인한다. 이번 실행 Choice Save 결과는 null이다. 새 원정의 원래 Session을 최초1회 생성하여 비치명 교체에도 그대로 전달한다. |

private Flow 생성 caller는 생성자 호출 **직전** 위 같은 Profile/실제 Save/원문 pair/revision/run/session/predecessor 검증을 수행한다. handoff의 두 정확 생성자는 null/혼합 predecessor, 누락 source/session/run/owner/endpoint 및 부정한 세대/tick 구조를 거절하고 자기 overload에 대응하는 private 분기 표시를 닫는다. 생성자만으로 Input.Unity가 Flow의 private 저장 객체를 해독하거나 Profile 진위를 독립 인증한다고 주장하지 않는다. 실제 Profile/Save 진위는 이를 소유한 Flow의 사전 검증과 아래 기존 pending 포트 재검증으로 증명한다. 이미 제안한 handoff 속성13과 overload2 이외의 공개/internal 속성·생성자·포트 추가0이다.

Flow는 현재 pending 생성·검증·소비에서 자기 닫힌 출처와 handoff 분기, 같은 원본 Profile/Save 운반 객체/RunSession/run key/predecessor를 매번 신원 대조하고 실제 expected/committed revision·현재 보유 원문 pair를 확인한다. Choice 출처에 BossPublication/기존 pair 기록 또는 기존 pair 출처에 Choice adapter/가짜 Choice Save가 들어오면 거절한다. Lifetime은 같은 endpoint의 기존 `ValidatePendingChapter1ChoiceBarrierHandoff` 포트의 참 결과와 자기 현재 pending/원래 requester/router/run/generation/actual completed tick 및 해당 실제 predecessor 소유를 함께 확인한다. Flow 포트의 참 하나만으로 Lifetime 원본 소유를 대체하지 않는다.

폐쇄 전에는 현재 predecessor 검증을 수행하고, 폐쇄 후에는 이미 보유한 같은 predecessor/closure 원문 신원·실제 Transition receipt·등록 해제/actions 폐쇄·단회 소비를 대조한다. 파기한 Unity 객체의 native getter나 새 BossPublication으로 그 기록을 대체하지 않는다. 아직 현재 저장 원문/revision이나 original closure를 확인하지 못하면 요청/구성/활성화0이다. 새 generation/firstTick은 실제 마지막 완료 뒤에만 정하고 미리 계산한 미래값으로 생성하지 않는다. 거절/활성화 실패는 current pending만 닫으며 성공 Save/기존 역사·원래 세션을 rollback하지 않는다.

### 11.4. OD-PCF-004의 집중 횟수 보정 후보와 현재 한계

루나 `gpt-6-luna/max`의 읽기 판독은 실제 v5의 Edit5·Barrier Play6·Closing4, 기존15 선택자의 실제 수용0/15를 확인했다. Ignore 호출17 중 추가2는 prefab/narrative 가드의 대체 경로이며 새로운 선택자가 아니다. 앞선 Ignore 이력으로 양분기/실제 재시작을 닫지 않는다. 고정 profile root와 Approved Base의 original/baseline/extraction/solidarity 보존 경로를 유지한다.

`PairConsentCancelAndSaveGate`와 `BothBranchesCloseOnlyAfterRealPassageAndSave`를 각각 새 Unity 프로세스의 분기 A/B 두 조건부 행에 배치하는 후보를 명명한다. 각 모드의 집중 invocation은 **1→2**이므로 아스트라의 정확 기술 호환 판정이 선행한다. 모드별 누적 wall-clock은 기존1800초를 유지한다. 첫 행은 해당 모드의 나머지 선택자를 각각1회 포함하고, 두 번째 행은 그 양분기 선택자만 재실행한다. 선택자 추가/이동·통과 복사·추가 자동 재시도0이다. 넓은 회귀는 각 모드1회/3600초 그대로 두며 집중 양분기 증거를 대신하지 않는다.

각 행은 실제 branch 조건, 기존 selector, 해당 고정 root의 실제 입력 저장 원문 SHA, 종료 후 최종 XML 경로/원문 SHA, 정수 종료 코드, PID/시작/종료, 소유 프로세스 소멸과 출력 회수를 결속한다. Root는 먼저 original과 실제 공개 경로에서 만든 baseline을 보존한다. 분기 A의 Edit/Play 결과를 정상 종료·최종 XML 파싱 뒤 닫고 그 실제 저장 root를 보존한 다음에만 baseline을 복원한다. 새 프로세스로 분기 B를 실행하고 똑같이 닫는다. 끝에 original을 동일 원문 SHA로 복원한다. live Editor/Player/save writer가 하나라도 남거나 폐쇄를 미관측한 상태에서는 baseline 교환0이다. 실패/누적 상한 도달 시 해당 사례를 종료하고 재시도0이며, 여러 XML을 사후 편집·병합하여 하나의 정상 실행처럼 만들지 않는다.

실제 분기 창2회/각1800초와 비치명 retry2회/각600초는 집중 행을 대체하지 않고 동일 immutable baseline에서 별도 실행·실제 원문으로 보존한다. 실제 restart1회/600초는 Root가 외부 수명을 제어한 공개 앱의 새 PID/시작시각과 V1 보존/완료 receipt 미복원을 확인해야 한다. 시험이 자기 Editor를 종료한 뒤 이어 실행·최종 XML을 쓰는 방식은 불가하다. 살아 있는 Test Runner와 재시작 대상의 분리/동일 고정 root writer 소유/기존 selector 접근이 실제로 가능한 유한 배치는 아직 확정하지 못했으며 OD004 미완료다.

위 집중 횟수 보정은 Review 후보이다. OD003의 정상tick0·유한 예산에서 생산 checked overflow/자연 late failure의 미입증을 닫지 않으며 지정 단회 fault와 초기 receipt0 의무도 각각 별개다. B1 actual FINAL 이전은 설계/영향 조사만 허용하고 공유 제품 통합0을 유지한다. 보정 뒤 독립 반대판독과 아스트라의 정확 범위 승인, 구현 소유 인계가 선행하지 않으면 제품/도구 구현·실행0이다. 기존 Q0 인간 승인·키/세대·필수 회귀/9회·원격 게시 관문은 유지한다.
### 11.5. OD004 배치 반대판독의 추가 보정 — Review

아스트라 `gpt-6-astra/ultra`가 닫힌107119바이트까지와 Approved Base/실제 v5를 읽어 확인한 결과, §11.4의 **첫 행에 나머지 전부/두 번째 행에 두 선택자만**은 불충분하다. 그 문구는 검토 중인 이전 후보로 보존하며 실행 계획이나 승인이 아니다. `Reservation35_36_37Hold59_60Platform149_150`도 양분기이며, 연대 전용 선택자를 수탈 행에 억지 배치하지 않는다.

| 모드 | 수탈 A의 최소 배치 후보 | 연대 B의 최소 배치 후보 |
| --- | --- | --- |
| Edit | PairConsentCancelAndSaveGate의 A 부분, RecallAge20_21_22AndCooldown299_300, Reservation35_36_37Hold59_60Platform149_150의 압착 부분, RejectForeignCopiedStaleConsumedAndOverflow | PairConsentCancelAndSaveGate의 B 부분, Solidarity419_420AndSameTickTransfer, Reservation35_36_37Hold59_60Platform149_150의 연대 부분 |
| Play | AuthoredExtractionOneValidSkillPasses, CounterweightTransferRecallChangesPhysics, RadiusBoundaryAndDamageRoster, AtomicInputFailureAndSingleCapture, BothBranchesCloseOnlyAfterRealPassageAndSave의 A 부분 | AuthoredSolidarityOneValidSkillPasses, BothBranchesCloseOnlyAfterRealPassageAndSave의 B 부분 |

Edit는 기존 이름5를 그대로 두되 최소 실제 호출7이다. Play는 기존 이름10을 그대로 두되 closing 양분기를 포함하면 최소11호출이며, NonlethalFallReturnsFreshGraphWithoutRunFailure를 실제 양분기로 요구하면 추가 한 호출이 필요하다. UiSubmitClickHeldCancelAndDuplicate, RejectForgedIncompleteOrUncertainCompletion, NonlethalFallReturnsFreshGraphWithoutRunFailure, RestartPreservesV1PairButNotChapterReceipt의 branch와 실제 입력 저장 상태/소비 순서는 아직 명명하지 못했다. 임의 순서 또는 자동 시험 실행 순서에 맡기지 않는다. 분기 창/비치명 retry와 기존 집중 selector가 같은 행동을 요구해도 원래 각 관측 권리를 대신하거나 중복을 숨기지 않는다.

실제 선택 Save 성공 뒤의 같은 profile은 새 원정에서 Choice UI를 생략하므로 pristine choice로 반복 사용할 수 없다. CommitOutcomeUncertain은 현재 ready 잠금/후속 Observe·Save0이며 새 graph만 만들어 정상 진행을 재개하지 않는다. 정상·FailedBeforeCommit·CommitOutcomeUncertain을 어느 기존 selector와 실제 원정/저장 순서에서 소비할지, uncertain을 마지막에 둔 뒤 필요한 실제 공개 복구 시작을 어느 남은 누적 예산에 포함할지 확정이 필요하다. 그 과정이 두 프로세스보다 많은 시작을 요구하면 실제 수를 같은 M2 호환 델타로 드러내고 승인받아야 하며 두 시작에 포함됐다고 기록하지 않는다. 모드별1800초는 시작/가져오기/검사/정리·필요한 baseline 교환을 포함해 계속 계수한다. 첫 실패이면 후속 B 시작0이다.

재시작 selector의 최소 검토 후보는 같은 PlayMode 이름을 UnityTest iterator와 같은 파일의 private 관측 helper로 바꾸고, 저장/게임을 소유하지 않는 실제 Test Runner는 살아 있게 두는 방식이다. Root가 별도의 **동일 최종 C1C 소스 실제 standalone 앱**만 종료·새로 시작한다. 기존 B1 G8 산출물은 C1C를 포함하지 않아 그 후보의 선행 빌드로 재사용하지 못한다. Run in Player도 별도 시험 Player 작성/실행을 요구하므로 미승인 예비 빌드0의 우회로가 아니다. 같은 project의 다른 Editor/새 project/새 profile root를 만들지 않는다.

관측 후보는 실제 이전 앱 PID/StartTicks/EXE/보유 handle와 원래 V1/완료 증거 → 실제 종료/출력 회수 → 같은 고정 root 교환0·옛 저장 writer handle0 → 동일 EXE의 새 PID/StartTicks → 공개 시작의 실제 pair/branch 보존·선택 재지급 생략·completion receipt 미복원을 결속한 뒤 iterator 정상 반환과 최종 XML 폐쇄를 요구한다. 단1 재시작600초와 집중 Play 누적1800초를 동시에 계수한다. Player 내부 receipt를 읽는 승인된 경로가 현재 없으므로 공개 UI/실제 저장·코드 불복원 검수의 정확 증거 범위를 별도 확정해야 한다. 새 파일 생성이나 코드 추론만으로 actual receipt 미복원 성공을 만들지 않는다.

현재 OD004 폐쇄/구현/새 build·앱 시작 권한은0이다. 필요한 선행은 실제 source 최종 빌드의 순서·횟수, 허용된 재시작 관측 수단, 고정 root writer 소유 및 정상/불확실 저장 흐름의 유한 배치에 대한 같은 M2/M3 기술 판정이다. 새로운 사용자 제품 선택은 발견하지 못했다. B1 actual FINAL 이전의 설계/호환 준비와 이후의 공유 구현/최종 실행을 분리한다. §11.1~11.3의 세 P1 보정은 별도 독립 구조 판독 중이며, 그 폐쇄를 OD004 실행 수용으로 확대하지 않는다.
## 12. 생산 연결과 사례별 실행 배치 합본 제안 — Review

이 절은 2026-10-13 아스트라 `gpt-6-astra`, 추론 설정 `도구 기본값, override 없음`의 설계 정리다. 기존126278바이트/SHA256 `724ABE52B3496A589F8346E4720A7DBFE5D1310E24F0C8564B91B0CEF4832D1D`를 접두사 그대로 보존한다. 상태는 **Review / Approved=false**다. 기존 솔·아스트라 권고와 Root의 사례 배치 제안을 한곳에 모으는 데 기술상 차단은 없으나, 아래 미결을 구현·시험 권한으로 바꾸지 않는다. 과거 실제 모델·설정·실패·미완료 판정은 변경하지 않는다.

### 12.1. 사용자 행동·생산 소유·범위와 선행

목표 행동은 보스 뒤 실제 선택 저장, 같은 원정의 기술 구간 진입과 비치명 복귀, 실제 통과·분기 저장 뒤 마지막 대사 확정까지의 연결이다. 생산 공백은 다음 네 묶음으로 관리한다.

| 공백 | 생산 소유와 연결 후보 |
| --- | --- |
| 보스 정리 뒤 LockedPostBoss에서 멈춤 | Flow가 실제 선택 Save를 소유하고 원본 RunSession을 최초 발급한다. RunView는 원래 발급 사실을 표시하며 성공을 대신 생성하지 않는다. |
| 원본 세션·다음 세대 인계 부재 | Lifetime이 원래 세션과 predecessor 폐쇄 증거를 보유하고 실제 등록 해제·actions 폐쇄 뒤에만 새 Barrier 세대/실제 완료 tick+1을 인계한다. |
| 실제 기술 포트 소비 부재 | Driver의 AdvanceTransferPhase가 같은 tick의 원본 context/receipt/capture에서 Prepare/Validate/Commit을 수행하고 일반 Press와 중복 소비하지 않는다. TransferSession과 Adapter/Router의 원본 소유를 연결한다. |
| 실제 성공과 저장·마지막 대사 단절 | Flow의 실제 성공, 같은 원정의 분기 Save와 원본 receipt, Closing 마지막 대사 commit을 순서대로 결속한다. 저장 불확실을 완료로 승격하지 않는다. |

추가 생산 연결 검토 대상11은 기존 제품 루트의 Chapter1PlayableFlowOwner, Chapter1RunView, Chapter1DevelopmentProfile, InputRouter, Chapter1GameplayInputSink, Chapter1EncounterEntry, Chapter1EncounterEvidenceView, Chapter1EncounterLifetimeOwner, Chapter1ChoicePhaseAdapter, TransferSimulationDriver, TransferSession이다. 정확 경로는 §3.1/3.2의 기존 파일을 따른다. 이는 새 파일11 또는 승인된 수정11이 아니다. 기존 후보27, 기존 공유 통합18, 이 추가 연결11은 겹침을 포함하는 서로 다른 집합이다. B1 최종 지문을 기준으로 경로별 교집합·차집합·기승인 행동·새 행동·실제 소유자를 한 번에 닫기 전 수량을 더하거나 공유 원문을 덮지 않는다.

Driver 새 서명3은 §9.4, Lifetime/Flow pending·handoff는 §9.3 및 §10.6/11.3, Adapter/Router 동기 입구2는 §10.2의 후보다. SaveChapter1CompletedBranchForRun과 선택 저장·재시도 추가 서명도 §3.2의 Review 미결이다. 선언이 문서에 있다는 사실을 기존 API 또는 Approved 근거로 쓰지 않는다. 늦은 오류에서는 §11.1의 자기 봉쇄→Router 동기 봉쇄→조건부 관측→원래 예외 재전파 순서가 앞선 불충분한 설명보다 우선하며, 소진한 proof 재검증·봉쇄 전 getter·관측 성공으로 실제 봉쇄 대체를 금한다.

추적은 proposal-REQ/AC-C1C-PCF-001~003, proposal-REQ/AC-C1C-SRETRY-001~009와 §2의 기존 REQ/AC 연결을 유지한다. 담당은 솔6.1의 한정 설계·구현 후보, 루나6의 독립 검수, 아스트라6의 정확 묶음 승인·통합이다. 이 절의 실제 작업은 설계 문서1 정리뿐이다. B1 actual FINAL, 최종 소스 핀, 정확 차집합과 독립 검토, Unity 종료, 공유 파일 단일 소유 인계가 완료되기 전 공유 구현0이다. r8의 컴파일 결과나 한정 수용을 B1 actual FINAL로 대신하지 않는다.

### 12.2. 기존 선택자15의 제안 호출 배치

아래 Edit11/Play14는 **제안 호출수**이며 승인된 프로세스 시작 수·성공 수·최소 충분성 증명이 아니다. §11.5의 미결을 구체화하는 후보로만 둔다. A는 수탈, B는 연대다. 원래 Edit5/Play10의 선택자15 이름·소속은 유지한다. 같은 이름의 여러 행을 한 통과 결과로 복사하지 않는다.

| 행 | 기존 선택자 | 제안 조건 |
| --- | --- | --- |
| E1 | PairConsentCancelAndSaveGate | A 정상 |
| E2 | RecallAge20_21_22AndCooldown299_300 | A |
| E3 | Reservation35_36_37Hold59_60Platform149_150 | A |
| E4 | PairConsentCancelAndSaveGate | A 확정 전 실패·새 중립 이후 재시도 |
| E5 | PairConsentCancelAndSaveGate | A 불확실 잠금 |
| E6 | PairConsentCancelAndSaveGate | B 정상 |
| E7 | Solidarity419_420AndSameTickTransfer | B |
| E8 | Reservation35_36_37Hold59_60Platform149_150 | B |
| E9 | PairConsentCancelAndSaveGate | B 확정 전 실패·새 중립 이후 재시도 |
| E10 | PairConsentCancelAndSaveGate | B 불확실 잠금 |
| E11 | RejectForeignCopiedStaleConsumedAndOverflow | A |
| P1 | UiSubmitClickHeldCancelAndDuplicate | A |
| P2 | AuthoredExtractionOneValidSkillPasses | A |
| P3 | CounterweightTransferRecallChangesPhysics | A |
| P4 | RadiusBoundaryAndDamageRoster | A |
| P5 | AtomicInputFailureAndSingleCapture | A 초기 실패 |
| P6 | AtomicInputFailureAndSingleCapture | A 늦은 오류·별도 새 원정 |
| P7 | NonlethalFallReturnsFreshGraphWithoutRunFailure | A |
| P8 | BothBranchesCloseOnlyAfterRealPassageAndSave | A |
| P9 | RejectForgedIncompleteOrUncertainCompletion | A 원본·위조 거절 |
| P10 | RejectForgedIncompleteOrUncertainCompletion | A 실제 분기 저장 불확실 |
| P11 | AuthoredSolidarityOneValidSkillPasses | B |
| P12 | NonlethalFallReturnsFreshGraphWithoutRunFailure | B |
| P13 | BothBranchesCloseOnlyAfterRealPassageAndSave | B |
| P14 | RestartPreservesV1PairButNotChapterReceipt | 동일 최종 C1C 앱의 B 실제 완료 상태에서 앱만 종료·재시작1, 독립 시험 실행기는 생존 |

P6의 늦은 오류는 §9.7/10/11의 정확 시험 예외 후보를 채택하고 정상 생산 문맥으로 도달해야만 실행할 수 있다. E11의 보조 산술과 P6을 실제 생산 checked overflow 증명으로 쓰지 않는다. 확정 전 실패·불확실 저장의 실제 합법 원인과 접근 방식도 미결이며, 표가 임의 오류·성공·Save 결과 주입 권한을 발급하지 않는다.

### 12.3. 사례 격리·저장 폐쇄·유한 예산의 미결

각 행은 원본 저장 기준, 같은 고정 root의 허용 보존 경로, branch, selector, 실제 원정 key·세대·세션, 시작/종료·최종 XML·원시 로그·저장 지문과 소유 종료를 별도로 결속해야 한다. 사례별 기준 원문과 보존 경로는 아직 미승인이다. 앞 행의 원정 key, 세대, Session/cooldown, pending, receipt, capture, 저장 writer, 중립/새 edge를 다음 행의 권위로 재사용하지 않는다. 같은 사례 안의 승인될 비치명 복귀는 원래 세션을 유지하는 별개 의무다. 단회 소비·실패 뒤의 권위를 재발급하거나 graph만 바꾸어 불확실 저장 잠금을 풀지 않는다.

기준 저장 교환은 해당 Editor/Player와 모든 저장 writer의 실제 폐쇄·출력 회수를 확인한 뒤에만 가능하다. 성공 Save 이후의 profile을 pristine choice 기준으로 재사용하지 않는다. 원래 저장 원문 보존·종료 후 복원·지문 대조를 유지하며, 실패 상태를 덮거나 XML을 사후 병합하여 성공을 만들지 않는다. 사례 실패 시 뒤 행을 자동 시작하지 않는다.

모드별 집중 전체1800초는 시작·가져오기·시험·정리·필요한 기준 교환을 모두 포함하는 기존 제약이다. Edit11/Play14를 그 안에 실제 배치할 수 있는지, 필요한 정확 프로세스 수·행별 시간·폐쇄 방식은 미결이다. 여러 호출을 두 시작으로 간주하거나 무제한 재시도를 넣지 않는다. 넓은 회귀 모드별1회/3600초, 분기 창2회/각1800초, 비치명 재시도2회/각600초와 실제 재시작1회/600초의 기존 관측 의무도 유지한다. 겹치는 행동만으로 별도 의무를 대신하지 않으며 P14에는 집중 Play 누적1800초와 재시작600초를 동시에 계수한다.

P14는 같은 최종 C1C 소스의 실제 standalone 앱이 선행해야 한다. B1의 옛 빌드는 재사용하지 않는다. 시험 실행기는 생존하고 앱만 원래 handle/PID/시작 신원으로 종료·회수한 뒤 같은 EXE를 새로 시작한다. 고정 root의 writer 중복0, 새 PID/시작, V1 pair 보존·선택 재지급 생략·완료 receipt 미복원의 정확 외부 관측 경로가 필요하다. 현재 내부 receipt의 외부 관측 근거는 미결이다. M2 시험과 M3 최종 빌드·재시작의 선행 순서, 추가 빌드/시작 수량은 같은 기술 묶음에서 닫아야 하며 이 절은 새 빌드·시험 Player·probe를 허용하지 않는다.

### 12.4. 다음 종료 조건과 현재 권한

검사는 기존 Chapter1ChoiceSkillSessionTests, Chapter1ChoiceBarrierPlayModeTests, 기존 Closing 시험의 세 파일과 EditMode Run asmdef1 안에 배치하는 후보로 한정한다. 새 검증 도구·새 시험 파일0이며 실제 수정 권한도 아직0이다. 다음 설계 종료 조건은 정확 서명·caller·소유·기존 승인 대비 차집합, 각 사례의 합법 기준과 입력/오류 도달, 실제 유한 프로세스·시간 배치, 저장 writer 폐쇄, M2/M3 순서 및 관측 경로를 독립 판독 가능한 한 묶음으로 확정하는 것이다. OD-PCF-001~005와 미충족 AC는 닫지 않는다.

현재 발견된 항목은 기술 설계 미결이며 새 사용자 제품 선택은 없다. 실제 앱 동의·예산·접근 범위를 넘는 선택이 필요해지면 그 항목만 보류하고 기존 인간 관문을 유지한다. 이 Review 정리로 구현 승인·통합 승인·실제 시험·새 probe·GUI·프로필 쓰기·새 승인 원문·새 보고 사본은 발생하지 않는다. 종료 산출물은 이 파일의 접두사 보존·최종 길이/SHA 확인과 Root에 대한 단독 쓰기 소유 반환이다.
