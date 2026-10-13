# C1-C 선택부터 1챕터 실제 완주까지 — 한정 구현 승인

- 상태: Approved. 아래 한정 승인 기록과 B1 최종지문·충돌 없는 작성·통합/실행 관문을 적용한다. 실제 구현·완주 수용은 별도다.
- 설계 담당: 아스트라. 구현 담당: 별도 테라 `gpt-6.1-sol`. 독립 검수: 루나 `gpt-6-luna`.
- 상위 권위: `2026-10-03-chapter1-playable-delivery.md` §13, §15 포함 원문 SHA `C7ABD46827D4CFE2537E81EDAD616646E998971F7C219D9BFED064582AF21A86`.
- 직접 읽은 설계: 솔 원장 SHA `6E23098CCC2D52FF072A174DB23AEA2448D11A6391B9485F6D447DF11CAB4FA7`, 루나 사전검토 SHA `09744C8FDB444A852D0B61931A7279FE806F828C97F1CD242B123AC05DA614BE`, 현재 권위 재독 v2 SHA `8895E77B32B58EF0E9A887ADBBA047F00F2C4208E742C628BB4AE4768D616E00`.

## 1. 사용자 산출물과 권위

사용자는 출정·네 방·오르단·유담의 명시 선택·실제 상자 전이·선택 기술·봉쇄선 통과·세령 첫 등장·1챕터 마지막 장면·잠긴 2챕터 예고까지 양분기로 완주한다. 본 후보는 B1의 설계·작성 선행조건이 아니다. Q0 인간 승인, 원격 게시, 키·세대, 기존 필수 9회 및 일반 사용자 프로필 권한은 바뀌지 않는다.

REQ-C1C-001은 REQ-CH1-001~010, REQ-CHOICE-001~007, REQ-WT-001/003/005/006/007을 구체화한다. 종료는 같은 최종 소스 빌드에서 루나의 양분기 실제 조작·서사·재시작 검사와 아스트라 통합 수용까지다. 이번 문서 작성은 게임 구현이나 시험 성공이 아니다.

§13.1의 V1 선택·기술·CompletedBranches 저장과 같은 실행 closing receipt를 채택한다. 솔의 V2 및 추가 챕터완료 저장 단계는 역사 제안으로만 남긴다. ChapterCompletionSaving 단계는 만들지 않는다. 성공 분기 저장 후 마지막 대사 완료가 CH1 receipt의 필요조건이며 재시작은 receipt를 복원하지 않는다. 이미 저장된 pair는 선택과 재지급을 생략하지만 실제 봉쇄선과 마지막 장면은 다시 진행해야 한다. V1 필드/인코더/복구/리셋 변경과 Profile friend 추가는0이다.

## 2. 봉쇄선 저작 결정

REQ-C1C-002. 아래 값은 기존 캐논의 기정 사실이 아니라 이번 승인 대상 신규 저작값이다. 좌표는 Barrier prefab 루트의 로컬 XY, 회전0·배율1, 오른쪽 +X/위 +Y, 1유닛은 기존 월드1유닛이다. collider는 회전 없는 BoxCollider2D, 구간은 양끝 포함이다. 플레이어 위치는 기존 player collider의 바닥 중앙을 기준으로 저작하고 기존 몸체 치수·이동 수치는 바꾸지 않는다.

| 물체 | 위치·형상·권위 |
| --- | --- |
| 입구 | 플레이어 바닥 중앙 `(0,0)`; 왼쪽 바닥 X `[-3,1]`, 상면 Y0·두께1 |
| 오른쪽 바닥/출구 | 바닥 X `[4.5,9]`, 상면0·두께1. 출구 trigger 중심 `(7,1.5)`, 크기 `(1,3)`; 실제 player collider 겹침 필요 |
| 틈 | X `(1,4.5)`. 아래 안전 감지선 Y=-2.0; 닫힌 하부 trigger X `[-3,9]`, Y `[-4,-2]`; 적/압착/killY/RunFailed producer 없음 |
| 상자 | 안정 ID `BarrierCounterweight`; 중심 `(4.5,0.5)`, 크기 `(1,1)`, 동적 Rigidbody2D·회전 고정·기본 mass1·gravityScale1. 기존 상자 modifier 그대로 사용. 우측 받침 상면0, X `[4,5]`; 수평 이탈 방지 레일 X4와5의 외측에 두고 LOS mask에서 제외. 중력 반응 검수에서는 동일 prefab 상자를 Y1.5에서 방출하여 실제 낙하를 관측하며 게임 시작 좌표를 바꾸지 않음 |
| 수탈 문 | 안정 ID `BarrierGate-Fractured`; 중심 `(5.1,1.5)`, 크기 `(0.2,3)`. CombatTarget HP2, TransferTarget 아님. 상자 중심에서 문 중심까지 거리 약1.166유닛 |
| 연대 문 | 안정 ID `BarrierGate-ConsentLock`; 같은 문 위치·크기, 피해 불가·TransferTarget 아님. 실제 연대 발판 접촉 publication 뒤 collider 해제 |
| 수탈 다리 | 중심 `(2.75,-0.125)`, 크기 `(3.5,0.25)`; 최초 collider 비활성. 실제 문 HP0의 Combat 완료 publication을 환경 소유자가 소비한 다음 tick에만 collider 활성화. 단순 기술 성공으로 생성하지 않음 |
| 연대 발판 | 성공 tick의 player 바닥 X와 상자 aim X 사이, 상면은 발동 player 바닥 Y, 두께0.25, 길이 `Clamp(abs(targetX-playerX),1.5,6)`; 중심 X는 두 X의 평균. 길이 최솟값 보정은 양끝에 대칭. PlayerOnlyPlatform의 기존 위에서 착지하는 충돌 규칙만 사용 |

각 분기에는 해당 문 하나만 등록한다. Combat roster는 수탈 player·상자·수탈 문 정확3, 연대 player·상자 정확2다. 상자 HP8, 선택 압착 중심 피해4로 생존한다. 상자와 문은 일반 공격/충돌 피해로 봉쇄선을 우회할 수 없고 해당 CombatTarget의 허용 피해종류는 ChoiceSkill뿐이다. 필터는 DamageRequest의 실제 발급원과 예약을 검증하며 피해량을 직접 대입하지 않는다. 유담과 ConsentAnchor-Barrier는 어느 roster에도 없다.

압착 주변 반경은1.5유닛, 예약 대상의 due tick capture 중심을 기준으로 Q1000 좌표 제곱거리 ≤2250000인 승인 roster의 다른 적대 환경대상만 피해2를 받는다. 중심 대상은 피해4 한 번만 받고 주변 목록에서 제외한다. player는 제외한다. 반경 밖·제거·다른 세대는0이다. 중심 대상 제거/사망이면 예약 전체 취소이며 새 활성 대상으로 재조준하지 않는다. 경직60틱은 상자 환경 소유자의 상태이고 사망 문에 복원 동작을 하지 않는다.

**조준 형상 한정 보정:** 루나 원장 `luna-c1c-closure-review.v1.json` SHA `7665F4958F386A06DCF7D04A9EB6E27113C98ACB37948A43B93B4D2916D74422`의 C1C-CLOSE-P1-01을 닫기 위한 신규 저작값이다. `BarrierCounterweight`의 TransferTarget과 `_targetPoseRoot`는 위 상자의 같은 Transform에 결속하며, BoxCollider2D의 로컬 offset은 `(0,0)`, 크기는 `(1,1)`, trigger는 false다. pose root의 초기 Barrier 로컬 위치는 `(4.5,0.5)`, 회전0·배율1이다. 직렬화된 `_localAimPointXQ1000=0`, `_localAimPointYQ1000=0`, `_localAimShapeCenterXQ1000=0`, `_localAimShapeCenterYQ1000=0`, `_localAimShapeHalfExtentXQ1000=500`, `_localAimShapeHalfExtentYQ1000=500`으로 고정한다. Q1000의1000은1유닛이며 모든 로컬 조준값은 Barrier 루트가 아닌 상자의 pose root 기준이다. 실제 TransferObservationBuilder가 쓰는 조준 형상은 축 정렬 타원으로, 이 값에서는 반지름0.5의 원이다. 물리 충돌 사각형으로 조준 포함 판정을 대체하지 않는다. 초기 Barrier 원점이 월드 원점이면 worldAimPoint와 worldAimShapeCenter는 Q1000 `(4500,500)`이고 X/Y 반경은 각각500이다. 다른 Barrier 루트 평행 이동은 실제 캡처 pose를 Q1000으로 양자화한 뒤 로컬값을 checked 덧셈하는 기존 경로를 따른다. 상자가 낙하하면 그 실제 pose에 따라 조준점도 이동하며 최초 좌표를 계속 사용하지 않는다.

저작 builder는 기존 `TransferTarget.ConfigureForAuthoring`의 여섯 정수 인자에 위 값을 순서대로 넣고 실제 collider/pose root/상자 modifier 소유자를 결속한다. validator는 `FreezeAuthoredGeometryForCrossValidation()`에서 여섯 값과 collider/pose root 동일성을 exact 대조한다. 생성 Barrier prefab·meta의 byteLength/SHA와 이 여섯 값 및 실제 직렬화 참조를 저작 원장에 함께 기록하고, runtime 단일 capture의 worldAimPoint/shape를 같은 원장에 결속해 루나가 재대조한다. 신규 meta 형식이나 GUID를 이 문서에서 발명하지 않으며 실제 생성된 prefab/meta가 없으면 저작 수용은 미완료다.

초기 player transfer origin과 위 고정 target aim point는 실제 prefab의 기존 origin 규칙으로 거리≤6.0 및 열린 LOS가 되어야 한다. 저작 validator가 실제 양자화 거리와 ray 결과를 검사하고 어긋나면 자산을 거절한다. 문은 상자 뒤에 있어 초기 LOS를 가리지 않는다. 주변피해 계산과 전이 거리 판정은 서로 다른 규칙이다. 기존 `CounterweightTransferRecallChangesPhysics` 검사에 위 여섯 저작값·초기 world 조준점·낙하 후 pose 동행을 함께 단언한다. 기존 AC-WT-006 회귀의 포인터/스틱·거리·LOS 경계와 포함 판정은 유지한다.

연대 발판은 `[t,t+150)` 동안 존재하고 상자 hold는 `[t,t+60)`이다. 발판이 끝나도 이미 접촉으로 열린 문은 같은 graph에서 열린 상태를 유지한다. 통과에는 실제 기술 결과, 해당 환경 결과, 실제 출구 겹침, 같은 tick의 완료 Movement/Transfer/Combat view가 모두 필요하다. 잠긴 문이나 틈을 이동 기술로 우회해 출구에 닿아도 기술·환경 결과 없이는 성공0이다.

REQ-C1C-003. 안전 trigger는 +110 완료 view의 실제 player bounds가 Y=-2 이하인 첫 tick에만 비치명 복귀 요청을 만든다. Input 측 어댑터가 실제 view와 router에 결속한 일회성 요청을 FlowOwner에 전달한다. 다음 생산 입력 receipt의 Transition→기존 RoomLeaving 정리→기존 graph 파기/actions 종료→새 generation Barrier graph의 입구 `(0,0)` 구성→GameplayEnabled 순서로 복귀한다. 위치 setter/teleport/HP reset을 추가하지 않는다. B0의 실제 lifetime·HP/남은 무적·연속 tick 승계를 재사용하고 HP는 감소하지 않는다. 선택 pair와 기술 cooldown의 절대 종료 tick은 원정 소유 ChoiceSkillSession에 유지한다. 예약/발판/문/hold와 활성 전이는 정리하고 새 graph는 닫힌 초기 상태다. 실패·RunEnd·보상·저장·세령 노출은0이다. 기술 대기 후 다시 시도할 수 있다.

## 3. 입력·권한·단일 확정 API

REQ-C1C-004. 기존 GameInputActions의 ChoiceSkill action과 InputRouter.OnChoiceSkill을 재사용한다. 새 action asset·InputSystem callback·-205 FixedUpdate·별도 입력 receipt는0이다. §8.3.1·§8.5·§8.6의 frozen batch, 전체 Prepare→전체 Validate→전체 Commit→map→전역 receipt 마지막 게시를 그대로 적용한다. -200 기존 capture 한 번 안에서 lifecycle/exposure/removal→기술→일반 Press, -190 기존 Combat 권위, Movement 완료 뒤 +110 읽기만 유지한다.

아래는 생성할 API의 정확 서명이며 현재 구현이 존재한다는 주장이 아니다. bool 반환은 검증 결과이고 권한 payload가 아니다. 불변 token의 외부 공개 생성자는 없다. owner private pending 슬롯과 객체 동일성을 모든 소비점에서 재대조한다.

| 소유 파일 | 서명·소비 규칙 |
| --- | --- |
| `Input/Unity/Chapter1ChoicePhaseAdapter.cs` | `BindUiPort(IChapter1ChoiceUiPort endpoint)`, `SubmitUiIntent(UiSemanticFrameV1 frame, string intent)`, `ValidatePendingUi(IChapter1ChoiceUiPort endpoint, Chapter1ChoiceUiToken token)`, `ConsumePendingUi(IChapter1ChoiceUiPort endpoint, Chapter1ChoiceUiToken token)`; 내부 UI interface·sealed token을 같은 파일에 정의 |
| 같은 파일의 interface | `void HandleChoiceUiIntent(Chapter1ChoicePhaseAdapter source, Chapter1ChoiceUiToken token)`; Run.Unity FlowOwner가 구현. 실제 router/UI frame과 현재 phase를 adapter가 검증한 후에만 token 발급 |
| `Core/ChoiceSkillContracts.cs` | 공개 `IChapter1ChoicePhasePort`의 `ChoicePhaseCandidate PrepareChoice(ChoicePhaseContext context)`, `bool ValidateChoice(ChoicePhaseCandidate candidate)`, `ChoicePhaseResult CommitChoice(ChoicePhaseCandidate candidate)`, `void CancelChoice(ChoicePhaseCandidate candidate)`; adapter가 단일 구현·발급자, Transfer driver만 결속 소비자 |
| `Transfer/Unity/TransferSimulationDriver.cs` | `BindChapter1ChoicePortForAuthoring(IChapter1ChoicePhasePort port)`는 pristine 비활성 때 한 번. 기존 AdvanceTransferPhase 내부에서 context 발급·port prepare/validate/commit. Run.Unity 참조 없음 |
| `Transfer/TransferSession.cs` | 내부 `PrepareRecallForChoiceSkill(SimulationTick tick, int expectedRevision, string targetId)`, 반환 sealed `ChoiceRecallCandidate`; `bool ValidateRecallForChoiceSkill(ChoiceRecallCandidate candidate)`; `void RecallForChoiceSkill(ChoiceRecallCandidate candidate)`; 후보는 Session가 생성하고 Transfer driver만 소비 |
| `Run/ChoiceSkillSession.cs` | 내부 `ChoiceSkillCandidate Prepare(ChoicePhaseContext context)`, `bool Validate(ChoiceSkillCandidate candidate)`, `ChoicePhaseResult Commit(ChoiceSkillCandidate candidate)`, `void Cancel(ChoiceSkillCandidate candidate)`; ChoiceSkillCandidate는 같은 Run 파일의 내부 sealed 타입이며 Session이 발급한다; 전이/피해를 직접 변경하지 않고 pair·cooldown·기술 ordinal 및 pending만 소유 |
| `Combat/Unity/Chapter1ChoiceEnvironmentOwner.cs` | `PrepareCompressionReservation(ChoicePhaseContext context)`, `PrepareReferencePlane(ChoicePhaseContext context)`→sealed `ChoiceEnvironmentCandidate`; `Validate(ChoiceEnvironmentCandidate candidate)`, `Commit(ChoiceEnvironmentCandidate candidate)`, `Cancel(ChoiceEnvironmentCandidate candidate)`; 실제 포트 접근은 Input adapter로 제한 |
| `Run/Unity/Chapter1PlayableFlowOwner.cs` | `TryCommitChapterCompletion(Chapter1DemoEndReceipt demoEndReceipt, Chapter1ClosingReceipt chapterClosingReceipt, Chapter1BranchSaveReceipt profileCommitReceipt)`; 세 불변 receipt는 같은 파일/CompletionReceipt 파일의 내부 타입. FlowOwner가 demo/save receipt를 실제 성공원장에서 발급·보관하며 ClosingPresenter 발급 receipt는 결속 presenter의 pending 객체와 대조 |

Core context는 상위 assembly 타입을 참조하지 않는다. 객체 신원은 object 참조, pair는 검증된 선택/동의/기술의 정수 값, tick은 SimulationTick, target ID는 string으로 읽기 전용 보관한다. 원래 상위 타입 검증은 발급자와 소비자에서 수행한다. Core context는 발급 Transfer driver·결속 adapter 객체, 기존 실제 전역 receipt 객체, 단일 capture 객체, run key·graph generation·tick·Transfer revision·활성 target ID·choice edge ordinal·읽기 전용 선택 pair·mode를 담는다. 생성권은 Transfer.Unity이고 adapter는 이 context가 결속 driver의 이번 pending context인지를 공개 읽기 검증 `ValidatePendingChoiceContext(ChoicePhaseContext context)`로 확인한다. Core의 ChoicePhaseCandidate는 adapter가 발급하고 context 객체와 불투명 object proof handle로 session pending, recall 후보(압착만), 환경 후보를 담는다. 각 proof의 구체 타입은 원래 소유 assembly에 남기고 Core에서 참조하지 않는다. 공개 값 객체를 복제해도 driver/session/environment의 pending 객체 검증을 통과하지 못한다. 모든 ordinal/revision/tick 연산은 checked이며 overflow는 회수 전 거절이다.

UI token은 adapter/router/endpoint 객체, run·generation, frameOrdinal/modeEpoch/tick/mode, intent, 양수 request ordinal과 현재 선택 제안 상태를 담는다. 기존 640×360 safe frame, 최소24×24 hit rect, actual-pixel 변환·Navigate/Submit/Click을 재사용한다. intent는 `chapter1.choice-extraction`, `chapter1.choice-solidarity`, `chapter1.choice-confirm`, `chapter1.choice-cancel`, `chapter1.choice-save-retry`, `chapter1.closing-next`, `chapter1.completed-return` 일곱 개로 닫는다. 취소는 확정 전 거부 상태 복귀이며 저장 시작 뒤 취소0. 수탈/연대 버튼은 제안만, confirm에서 새 edge로 확정한다. 연대 제안에서만 OffersScopedResonance를 표시한다. 같은 frame의 Click+Submit은 하나만 소비하며 held/옛 frame/잘못된 phase/복사·외부 owner/재진입은0이다. consume 잠금을 먼저 고정하고 저장을 호출하며 실패 재시도는 새 neutral와 새 edge를 요구한다.

압착은 전이 age21 이후 성공, recall revision+1·t+21 lock·t+36 예약·t+300 cooldown을 한 묶음으로 확정한다. 연대는 t+150 발판·t+60 hold·t+420 cooldown이며 전이 유지다. 모든 환경·예약 가용성과 checked 연산 검증을 회수 전에 끝낸다. 성공 기술은 같은 tick TransferPressed를 소비하며 실패 기술은 일반 Press 판정을 계속한다. Prepare/Validate 실패는 소비0, Commit 실패는 전역 미게시·TechnicalBlocked이며 부분 성공을 정상 진행으로 처리하지 않는다.

## 4. 정확 파일·조립체 경계

제품 대상 루트는 `output/chapter1-playable-r1/project/` 하나다. 상위 §13.2 및 솔 원장의 exactProposedChanges 18개 경로, 신규 시험3개, Choice/Barrier/Closing prefab·Chapter1Closing.json·기존 Chapter1Playable scene만 후보에 포함한다. 추가 UI token/receipt/interface는 위 기존 후보 파일에 넣으며 파일 수를 임의 확장하지 않는다. 모든 신규 파일의 meta, 실제 새 부모 폴더 meta를 원장에 포함한다. 기존 meta는 보존한다.

필요 조립체 변경은 다음 집합으로 닫는다. 기존 항목이면 중복 추가0이며 B1 최종 기준과 차집합으로만 적용한다.

- Input.Unity asmdef: `AcadeGameMaker.Run` 참조1. Run.Unity 역참조0.
- Core AssemblyInfo: `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Transfer.Unity` friend2. Core context 생성은 Transfer.Unity, candidate 생성은 Input.Unity에 한정하며 Core→Run 참조0.
- Run AssemblyInfo: `AcadeGameMaker.Input.Unity` friend1. 순수 Run은 Core의 공개 불변 계약만 사용한다.
- Input.Unity AssemblyInfo: `AcadeGameMaker.Run.Unity`, `AcadeGameMaker.Run.Unity.PlayMode.Tests` friend2. 기존 Hub.Presentation.Unity friend 재사용.
- Combat.Unity AssemblyInfo: `AcadeGameMaker.Run.Unity.PlayMode.Tests` friend1. Input.Unity 기존 friend 재사용.
- RunUnity PlayMode 시험 asmdef: `AcadeGameMaker.Run`, `AcadeGameMaker.Transfer.Unity`, `AcadeGameMaker.Combat.Unity`, `AcadeGameMaker.Movement.Unity`, `AcadeGameMaker.Hub.Presentation.Unity` 참조5를 기존 참조에 추가. 시험은 실제 scene 생산 입력을 사용하며 내부 성공 발급 우회 없음.
- Hub.Presentation.Unity의 B1 Run.Unity 참조와 Run.Unity의 기존 Hub.Presentation.Unity friend 재사용. 새 assembly·Profile friend·Transfer friend·Movement friend는0.

B1 최종지문은 설계 완료 선행이 아니다. 제품 통합 전 Root가 독립 검수된 B1/B0 입력, 위 모든 수정 파일·meta·asmdef의 byteLength/SHA, 누락 신규 경로, 정확 변경 소유자·차집합을 `c1c-integration-base.v1.json`으로 고정하고 루나가 대조해야 한다. 없는 파일의 SHA를 만들거나 작성 중 B1을 최종 기준으로 표시하지 않는다. 지문 불일치는 통합을 멈추되 충돌 없는 설계는 계속할 수 있다. 현재 후보는 이 실행 관문을 충족했다고 주장하지 않는다.

## 5. 서사 원문과 완료

REQ-C1C-005. 아래 문장은 현재 캐논의 유담 거부·범위 동의·장부 조작 능력 상실, 세령의 아인 탐색·도언 책임 추적을 구체화하는 승인 대상 원문이다. 캐논의 사건을 변경하거나 CH2 협력 확정을 앞당기지 않는다. 승인 후 저작 JSON은 아래 ID/순서를 그대로 UTF-8로 옮기고 byteLength/SHA를 저작 원장과 closing receipt에 고정한다. 자산이 아직 없으므로 현재 SHA를 발명하지 않는다.

| ID/경로 | 화자와 정확 표시 문장 |
| --- | --- |
| Y01 공통 | 유담: 이 장부에는 마흔일곱 사람의 허락이 담겨 있어. 네게 소유권을 넘기지는 않겠어. |
| Y02 공통 | 도언: 네 선택을 남길지, 네 각인을 끊어 힘을 가져갈지 결정해야 하는군. |
| YE01 수탈 제안 | 표시: 수탈 — 유담의 거부를 무시하고 각인을 절단합니다. |
| YS01 연대 제안 | 유담: 봉쇄선을 지나는 데 필요한 공명만 빌려주겠어. 장부와 선택은 내게 남겨 둬. |
| YS02 연대 제안 | 표시: 연대 — 유담이 허락한 범위의 공명만 빌립니다. |
| YE02 수탈 확정 뒤 | 유담: 내 허락 없이 끊었구나. 이제 나는 공동장부를 다룰 수 없어. |
| YE03 수탈 확정 뒤 | 도언: 내가 네 거부를 꺾었다. 그 책임을 남에게 돌리지는 않겠어. |
| YS03 연대 확정 뒤 | 도언: 네 각인은 네 것이야. 허락한 범위 안에서만 쓰겠다. |
| YS04 연대 확정 뒤 | 유담: 그 약속이 지켜지는지 보겠어. |
| S01 성공·공통 | 세령: 도언. 나는 아인을 찾고 있어. 언니에게 네가 무엇을 맡겼는지도 알아야겠어. |
| SE01 수탈 | 세령: 유담의 거부를 무시했군. 네 사정만 듣고 넘어가지는 않겠어. |
| SS01 연대 | 세령: 유담에게 선택을 남겼군. 하지만 그것만으로 네 책임이 사라지지는 않아. |
| S02 공통 | 도언: 내가 설명하지 못한 위험이 있었어. 아인을 찾고, 내가 한 일에 답하겠다. |
| S03 공통 | 세령: 네 말을 믿을지는 내가 판단해. 지금은 네가 남긴 흔적부터 확인하겠어. |
| C01 공통 | 표시: 봉쇄선 너머로 길이 이어집니다. 도언은 아인을 찾고 자신의 책임을 확인하기 위해 걸음을 옮깁니다. |
| C02 공통 | 표시: 1챕터 완료 |
| C03 공통 | 표시: 2챕터 — 아직 열리지 않은 길 |

Y01→Y02→제안→새 confirm→해당 결과 대사→ChoiceCommitted→pair 원자 저장 성공→기술 부여·Barrier 진입 순서다. 저장 실패는 `저장에 실패했습니다. 다시 시도해 주세요`, 불확실은 `저장 결과를 확인하고 있습니다`로 표시하며 기술0·봉쇄선0이다. 실패 재시도는 확인된 공개 복구 결과 뒤 새 의도만 받는다. 확정 연출이 저장 성공을 뜻하지 않는다.

실제 봉쇄선 통과→Cutscene·DemoCompleted 전이정리→RunEndRequested(Succeeded)→단일 RunEndCommitted→CompletedBranches 실제 저장→DemoCompleted→S01→SE01 또는 SS01→S02→S03→C01 순서다. 각 줄은 최소60 simulation tick 표시 후 새로운 closing-next UI 의미 edge로만 진행하고 중립 이전 held 입력은 진행하지 않는다. Cutscene에서 gameplay와 ChoiceSkill은 잠기며 UI advance만 기존 semantic frame으로 받는다. 스킵·자동 완료 없음. 마지막 C01의 유효 다음 edge에서 presenter가 원문 SHA·분기·재생 ordinal 전부의 완료 receipt를 한 번 발급한다. FlowOwner가 실제 demo/save/presenter pending 세 객체를 검증한 뒤 CH1 receipt를 발급하고 Ended에서 C02/C03 및 거점 버튼을 표시한다. C02를 미리 보여 완료를 암시하지 않는다. CH2 scene load0·동행 확정0·아인 상태 새 확정0이다.

## 6. 검사 메서드·회귀·유한 실행

REQ-C1C-006. 새 fixture 파일은 상위 §13.3 세 파일이며 아래 메서드는 승인 후 작성할 정확 이름이다. 내부 함수 직접 호출 시험과 실제 저작 입력 시험을 구별한다.

| 정확 fixture.method | 필수 내용/AC |
| --- | --- |
| Chapter1ChoiceSkillSessionTests.PairConsentCancelAndSaveGate | 두 pair·동의·취소·성공/실패/불확실 저장, AC-CHOICE-001/002, AC-CH1-003/006 |
| Chapter1ChoiceSkillSessionTests.RecallAge20_21_22AndCooldown299_300 | age20 실패·held21 실패·새 edge21/22, 회수후20/21, 압착299/300, AC-WT-001/003, AC-UX-004 |
| Chapter1ChoiceSkillSessionTests.Solidarity419_420AndSameTickTransfer | 연대419/420·전이 유지·동시 기술 성공 소비/실패 일반처리, AC-CHOICE-004 |
| Chapter1ChoiceSkillSessionTests.Reservation35_36_37Hold59_60Platform149_150 | 모든 경계 및 대상 제거·죽음·세대 변경 취소, AC-CHOICE-004 및 REQ-CHOICE-006/007 |
| Chapter1ChoiceSkillSessionTests.RejectForeignCopiedStaleConsumedAndOverflow | owner/run/generation/receipt/context/ordinal·복사·중복·checked overflow 무소비, AC-WT-003/006 |
| Chapter1ChoiceBarrierPlayModeTests.AuthoredExtractionOneValidSkillPasses | 실제 scene 장치입력→상자→압착→문 피해2·HP0→다리→출구 연속 trace, AC-CHOICE-004, AC-CH1-004 |
| Chapter1ChoiceBarrierPlayModeTests.AuthoredSolidarityOneValidSkillPasses | 실제 상자→기술→발판 접촉→문 해제→출구, AC-CHOICE-004 |
| Chapter1ChoiceBarrierPlayModeTests.CounterweightTransferRecallChangesPhysics | 동일 prefab 실제 낙하 기본/전이/회수 비교 및 modifier exact, AC-WT-001/003/006, REQ-WT-003/007. B1 상자0 증거 재사용 금지 |
| Chapter1ChoiceBarrierPlayModeTests.RadiusBoundaryAndDamageRoster | 중심 단회4·주변1.499/1.500/1.501 경계2/2/0·player 제외·다른 세대0, REQ-CHOICE-006 |
| Chapter1ChoiceBarrierPlayModeTests.NonlethalFallReturnsFreshGraphWithoutRunFailure | 실제 낙하→receipt-bound 정리→새 generation/입구·HP/무적·cooldown 보존, AC-CH1-005 |
| Chapter1ChoiceBarrierPlayModeTests.AtomicInputFailureAndSingleCapture | 각 Prepare/Validate 무소비·Commit/map 실패 전역0·후속phase0·Sync/capture1, AC-WT-003/006 |
| Chapter1ClosingPersistencePlayModeTests.UiSubmitClickHeldCancelAndDuplicate | 실제 장치 callback과 semantic frame·동일frame1·취소·저장실패 새 edge, AC-CHOICE-001/002 |
| Chapter1ClosingPersistencePlayModeTests.BothBranchesCloseOnlyAfterRealPassageAndSave | 두 branch 실제 생산 성공 trace·정확 대사와 closing receipt1·CH2잠금, AC-CHOICE-001/003, AC-CH1-003/004/007 |
| Chapter1ClosingPersistencePlayModeTests.RejectForgedIncompleteOrUncertainCompletion | 복사/다른 owner/분기/세대·미완료/불확실 저장·DemoCompleted만인 경우 CH1완료0, AC-CH1-006/007 |
| Chapter1ClosingPersistencePlayModeTests.RestartPreservesV1PairButNotChapterReceipt | 실제 개발 저장·재시작, 저장된 pair/branch 보존·closing 미복원·일반 프로필 접근0, AC-CH1-006 |

회귀 선택은 첨부 `2026-10-03-c1c-regression-inputs.v1.json`의 정확 파일 목록 전체 메서드다. 원본 Assets의 현 바이트는 읽기 기준이며 실행본 기준이라고 혼동하지 않는다. 범위는 Transfer 전부, Combat 및 CombatUnity 전부, Run 전부, InputUnity 전부, Profile 전부와 기존 C1A/B0/B1 시험이다. 넓은 필수집합을 임의 축소하지 않는다. 실제 실행 전 통합본의 모든 선택 파일/meta/asmdef·선택된 실행 method 전체 이름·fixture SHA를 실행 manifest로 재결속하고 원본과 차이가 있는 승인 델타를 루나가 대조한다. 원본 기준 지문만으로 B1 현행 회귀 통과를 주장하지 않는다.

실행 소유자는 Root다. 단회 compile1·저작1 각각30분, 집중 EditMode1 및 PlayMode1 각각30분, 회귀 EditMode1 및 PlayMode1 각각60분으로 닫는다. 기존 더 엄격한 해당 단회 제한이 있으면 그 제한이 우선한다. 정상 양분기 실제 창 완주는 분기별1회·각30분, 비치명 재시도 관측은 양분기 각1회·각10분, 재시작1회·10분이다. 봉쇄선 통과시간은 각 graph GameplayEnabled 첫 receipt부터 실제 출구 publication까지 측정하고 같은 조작 방식·동일 기준 상태에서 차이≤10초다. 장치별 기존 회귀와 필수9회는 이 횟수로 대체하지 않는다.

첫 실패/상한 도달에서 해당 실행을 멈추고 자동 재실행0, 원시 log/XML·시작/종료·프로세스·입력/자산/빌드 지문·실패 마지막 trace를 보존한다. 무로그만으로 정지 판정0. 최소 재현·원인/변경·새 관측 목적과 아스트라의 기존 계약상 재실행 관문을 통과한 뒤에만 후속 실행한다. 실제 창 두 분기는 루나가 독립 조작·한국어 글리프·대기/실패 안내·시간 차·서사 의미를 검수한다. 합성 장치/fault 시험이 이를 대체하지 않는다.

### 두 분기의 동일 저장 기준을 만드는 검사 전용 보존 절차

기존 고정 개발 root의 pair는 실행 중 덮거나 지우지 않는다. 양분기 검수만을 위해 다음 오프라인 보존 절차를 이번 후보의 명시 승인 대상으로 둔다. 앱·Unity·저장 프로세스가 모두 종료되고 잠금이 없으며 경로가 정규화된 workspace 내부의 정확 개발 root이고 링크가 아닐 때만 Root가 수행한다. 일반 사용자 프로필은 읽기0이다.

1. `output/local-playable-slice-r1/profile/chapter1-development/`의 기존 전체 바이트·메타 목록을 보존 원장에 기록하고 `output/chapter1-playable-r1/evidence/c1c-profile-preservation/original/`로 디렉터리 자체를 이동한다. 목적지가 이미 있으면 중단하며 덮어쓰기·삭제0이다. 최초 부재도 원장으로 기록한다.
2. 같은 고정 root가 부재인 상태에서 실제 공개 launch 경로로 새 유효 V1을 만들고 앱을 닫는다. 이 미확정 baseline 전체를 `c1c-profile-preservation/baseline/`에 복사하고 지문을 대조한다. 미확정/동의 거부/기술없음/완료분기없음을 공개 decoder로 검증한다. 무검증 JSON 작성과 reset 호출0이다.
3. 수탈 실제 완주·재시작 관측 후 전체 개발 root를 `c1c-profile-preservation/extraction/`으로 이동·보존한다. 비어 있는 정확 고정 root에 baseline을 복사해 지문을 대조한 뒤 연대 완주·재시작을 수행한다. 설정·튜토리얼·seed도 같은 baseline으로 시작하고 검증101 UI를 사용한다.
4. 연대 종료 후 개발 root를 `c1c-profile-preservation/solidarity/`로 이동하여 원시 바이트를 보존하고, 1의 original이 있었으면 동일 고정 root로 이동 복원하여 전체 SHA 일치를 확인한다. 최초 부재였으면 root는 부재로 돌려놓고 새 실행 결과는 보존 디렉터리에 남긴다. 복원 실패는 검사 차단이며 저장 성공으로 숨기지 않는다.

이 절차는 원정 오류 복구나 사용자 게임 기능이 아니며 수동 양분기 검사 데이터의 보존·교환만 허용한다. 각 파일의 이동 전후 absolute path·byteLength/SHA·프로세스 종료 증거를 원장에 기록하고 루나가 baseline 동일성을 확인한다. 개발 root 오염·불확실 커밋이 있으면 복구 판정 전 교환하지 않는다. 해당 보존 절차 승인 전 양분기 fresh 조건이 충족됐다고 주장하지 않는다.

B0의 현재22개 fixture 조사와 새 음성시험3개의 변경은 별도 B0 실행 선택 관문이다. 본 C1C 후보는 그 실행이나 검수 결과를 미리 승인하지 않는다. B1 보스 입구는 기존 Ordan 저작 위치를 유지하며 일반방 `(0,0.81)` 좌표를 보스에 확장 적용할 권한은 없다. 실제 B1 최종 소스에서 이 경계도 통합 전 대조한다.

## 7. 다음 관문

실제 아스트라는 검수 전임 원문 SHA `F111E119F4BEDA54DB88F7925E451BE9E4BD8E8D4AD8CEB95F81BAD8AA66DB05`, 루나 전체 정적 검수 v1 SHA `7665F4958F386A06DCF7D04A9EB6E27113C98ACB37948A43B93B4D2916D74422`의 P0=0/P1=1과 조준 형상 단일항목 재검수 v2 SHA `1EB492FC0E3D4E16FAF961FF47A6C63F4682A3533AC233FB1E2072D6B2BAA65A`의 해당 P1 폐쇄·추가 P0/P1=0을 직접 읽고 이 문서 §1~6의 정확 범위를 한정 Approved로 승인한다. 신규 저작값·압착 age21 보정·대사 원문·V1 보존 결정·두 분기 검사 전용 저장 보존 교환·정확 파일/API/참조·검사 방법과 상한이 승인 범위다. 전체 검수를 v2 하나가 수행한 것으로 확대하지 않으며 실행 증거는 아직 별도다.

회귀 원장 `2026-10-03-c1c-regression-inputs.v1.json` SHA `5299C917DD21556CD80D03E09A71BDC3FEB99A8092E31F43B351B5930F527FC0`의 정확220개 읽기 기준 입력을 채택한다. 원장의 Review 표시는 작성 당시 이력으로 보존하며 해당 선택의 현재 승인 권위는 이 절이다. 현행 통합본 입력·method 선택 재결속을 대신하지 않는다. 이전 본문의 승인 대상·후보라는 표현은 이 정확 범위에 한하여 이번 승인으로 해소한다.

별도 테라는 충돌 없는 신규 파일 작성과 명시 범위의 준비부터 진행할 수 있다. B1/B0 공유 파일 수정·실제 project 통합 전에 §4의 독립 검수된 최종 기준지문과 차집합 관문이 필수이며, Unity가 같은 project를 읽는 동안 제품 쓰기0이다. 통합 직전 B1 실제 기준지문, 구현 뒤 생성 자산·서사 JSON 지문, 실행 뒤 독립 실제 양분기 플레이 증거는 시점별 필수 산출물이며 현재 존재하는 것처럼 표기하지 않는다. 코드 자기수용·실행 성공·전체 CH1 완주·새 배포/원격 게시 승인은 이 설계 승인에 포함하지 않는다.
