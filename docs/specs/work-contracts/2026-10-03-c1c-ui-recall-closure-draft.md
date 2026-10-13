# C1C 대화 입력·실제 회수 접합 초안

- 상태: Approved. §6의 아스트라 한정 승인으로 §5.1~5.9의 정확 범위만 채택한다. 독립 B1 최종 기준·통합 원장 전 공유 제품 쓰기는 금지하며 실제 구현·실행 수용은 별도다.
- 작성: Root. 현재 호출 모델 `gpt-6.1-sol`을 실제 세션의 2026-10-03T11:47:45.629Z 모델 기록으로 확인했다. 한정 설계 초안이며 아스트라 최종 승인이나 루나 독립 검수는 아니다.
- 목적: 사용자가 선택 기술로 실제 봉쇄선을 통과하고 세령 대화를 끝내는 생산 연결의 두 공백을 닫는다.
- 근거: Approved `2026-10-03-c1c-bounded-closure-candidate.md`, 준비 경로의 `c1c-gap-closure-handoff-terra61.v1.json`과 `shared-api-handoff-terra61.v3.json`.
- 실제 선행: 이 초안의 검토와 C1C 충돌 없는 신규 준비는 B1 완료를 기다리지 않는다. C1C 공유 소스 쓰기는 독립 검수된 B1 최종 기준과 기존 통합 관문 뒤다.
- 종료 조건: 아스트라의 정확 소유·서명·순서 판정, 루나 독립 대조, 해당 구현과 실제 양분기·오류 검사의 증거. 현재 달성하지 않았다.

### 반대 설계 인계 — 미승인

별도 솔 `gpt-6.1-sol`의 `output/chapter1-playable-r1/staging/c1c/ui-recall-counterdesign-sol61.v1.json`은 19,849바이트/SHA `F805BFDB1B07AB6DC2D5EAE6E9253A3044ACDE7E5E57B29F719303BFC8AB3883`다. 기존 성공 계약의 실제 `Cutscene→Ended`와 종료 입력 가드를 유지하려면 종료된 router를 재사용하는 첫 대안을 채택할 수 없다는 반대 검토를 받았다. 권고안은 실제 기존 종료·파기·입력 종료 뒤 새 대화 전용 입력 호스트를 발급하고, 이전 데모 세대와 새 화면 세대를 같은 원정의 소유된 선행 증거로 연결하는 두 번째 대안이다. 이 권고는 아스트라 승인이나 루나 독립 수용이 아니다. 아래 최초 두 대안은 검토 이력으로 보존한다.

반대 설계는 입력 영수증이 -300 단계에서 이미 발행되고 실제 회수 확정은 -200 단계에서 수행된다는 규범 충돌도 드러냈다. 초기 입력 확정 실패의 입력 영수증 미발행과, 늦은 회수 확정 실패의 통합 완료 결과 미발행을 구별해야 한다. 늦은 실패에서 이미 발행한 입력 영수증을 없다고 표기하거나 되돌리지 않는다. 기존 Approved 원문이 해당 늦은 실패에도 입력 영수증 미발행을 요구한다면 아스트라가 정확 호환 보정을 승인하기 전 그 구현을 시작하지 않는다. 생산 소스 변경·실행·완료 수용은 아직 없다.

## 1. 기존 권위와 달라지지 않는 경계

REQ-C1C-002/004/005/006 및 AC-CHOICE-001/002/004, AC-WT-003/006, AC-CH1-003/004/005/006/007에 결속한다. V1 pair·CompletedBranches, 원자 저장 결과, 실제 입력의 단일 소유자, 원문 대사17개, 기술 수치와 전역 미게시 실패 규칙을 유지한다. 신규 지속 챕터 완료 필드·별도 저장 파일·자동 대화 완료는 없다.

현재 주 계약 §20의 B0 검증 수용과 B1 공유 구현은 별도다. Root가 여기서 관측한 B1 파일은 작성 중이며 최종 통합 기준으로 사용할 수 없다. 역사 원장과 앞선 실패를 수정하지 않는다. 현재 주 모델의 확인을 과거 Root 실행 전체의 모델로 소급하지 않는다.

## 2. 입력 모드와 서사 상태를 분리하는 제안

현재 `UiSemanticFrameV1.Validate`와 cursor는 실제 `UIOnly` receipt만 허용하고 InputRouter도 그 모드에서만 UI frame을 발급한다. 따라서 서사상의 Cutscene·Closing·Ended를 같은 이름의 InputMode로 바로 보내면 다음 대사와 거점 버튼이 작동하지 않는다.

제안은 Flow 상태를 `Closing`/`Ended`로 두되, 대화와 완료 화면의 실제 장치 모드는 `UIOnly`로 유지하는 것이다. Gameplay/ChoiceSkill은 잠그고 기존 UI 의미 frame만 소비한다. 기존 `UiSemanticFrameV1` 파일·검증·cursor ABI는 수정하지 않는다. `InputMode.Cutscene` 또는 `InputMode.Ended` receipt를 UIOnly라고 가장하지 않는다.

Flow 상태 추가는 기존 `Run/Unity/Chapter1RunView.cs`의 정확 변경으로 아스트라가 별도 명시해야 한다. 이 파일이 기존18개에 없으면 현재 승인 목록에 포함된 것처럼 처리하지 않는다. 실제 단계 이름과 기존 저장 실패 상태는 B1 최종 원문과 대조해 확정한다.

### 닫아야 하는 수명주기 선택

현재 후보 ClosingPresenter는 UI token.Generation과 DemoEndReceipt.Generation의 일치를 요구한다. 봉쇄선 graph를 파기하고 별도 closing router를 새 세대로 만들 경우 이 비교는 그대로 성립하지 않는다. 아스트라는 다음 두 방식 중 생산 계약과 기존 종료 기준에 맞는 한 방식을 확정해야 한다.

1. 기존 C1-A 결과 화면처럼 정리된 기존 graph의 router만 UIOnly로 유지한다. 실제 DemoCompleted 정리·Run 성공·branch 저장의 기존 순서를 먼저 충족하고 게임 생산자는 다시 활성화하지 않는다. Closing prefab은 중복 router 없이 표시·입력 port만 연결한다. 이후 거점 복귀에서 실제 graph 파기/actions 종료 뒤 새 거점을 만든다. 동일 router/세대 검증을 유지할 수 있다.
2. 기존 종료 계약이 그보다 먼저 graph 파기를 요구한다면 실제 파기·actions 종료 뒤 새 closing UI host를 구성한다. 이전 demo 세대와 현재 closing 세대를 각각 보관하고 실제 정리 영수증·같은 run의 새 host 입구를 결속한다. 세대 숫자를 같게 꾸미거나 기존 demo receipt를 새로 발급하지 않는다. 이 경우 정확 host 구성·파일·모드 requester·세대 전달 API를 승인에 추가해야 한다.

이 초안은 어느 방식을 이미 Approved라고 선택하지 않는다. 기존 종료 기준을 확인하지 않은 채 graph를 유지하거나 새 host를 발명해서는 안 된다. 분기 저장과 DemoCompleted의 순서도 기존 주 계약 §13.2/§17 및 C1C 계약을 따른다.

### 순환 참조 없는 표시 포트 제안

Run.Unity가 Hub.Presentation.Unity를 참조하지 않도록 아래 내부 interface를 기존 `Chapter1ChoicePhaseAdapter.cs`에 둔다. 양쪽은 기존 Input.Unity 참조와 friend를 사용한다. 추가 제품 파일이나 신규 friend를 만들지 않는다.

```csharp
internal interface IChapter1ClosingViewPort
{
    void BeginFromCommittedRun(object demo, object branchSave);
    bool AdvanceFromPendingUi(Chapter1ChoicePhaseAdapter source,
        Chapter1ChoiceUiToken token);
    bool OwnsPendingClosingReceipt(object receipt);
    void ConsumePendingClosingReceipt(object receipt);
}
```

불투명 object는 권한을 부여하지 않는다. ClosingPresenter는 실제 Run.Unity 내부 receipt 타입으로 대조하고, FlowOwner는 저작 때 한 번 결속한 포트와 자체 retained demo/save를 대조한다. 서명은 초안이며 기존 후보의 형식별 메서드를 이 interface의 명시 구현으로 연결하는 정확 최종 형태를 승인자가 결정한다.

FlowOwner의 제안된 읽기·완료 API는 다음이다.

```csharp
bool ValidatePendingChapter1ClosingStart(object presenter,
    Chapter1DemoEndReceipt demo, Chapter1BranchSaveReceipt save);
Chapter1CompletionReceipt CurrentChapter1CompletionReceipt { get; }
```

기존 `TryCommitChapterCompletion(demo,closing,save)`는 유지한다. 같은 FlowOwner/run/분기, 실제 성공 원장과 branch 저장의 Outcome·기대 revision·CommittedRevision, 결속 presenter의 retained closing 객체·원문 SHA·재생 ordinal·마지막 새 edge를 모두 대조한다. 생성자 호출·public 값 복사·bool 반환 하나만으로 완료할 수 없다. 소비 준비를 전부 검사한 뒤 presenter pending을 한 번 소비하고 현재 실행 완료를 한 번 발급한다. 재진입·불확실 저장·불완전 마지막 대사·다른 세대는 완료0이다.

closing-next는 기존 adapter의 private pending UI token 하나로만 전달한다. FlowOwner는 포트로 전달하기 전에 같은 token을 소비하지 않는다. Presenter는 실제 endpoint를 검증하고 표시 시작부터60 simulation tick 이상 및 새 중립·새 edge를 확인한 후 한 번 소비한다. 마지막 C01 이후에도 FlowOwner의 실제 completion을 확인해야 C02/C03·거점 버튼을 표시한다. 현재 실행의 완료를 재시작에서 복원하지 않는다.

## 3. 실제 회수 후보의 driver 연결 제안

순수 Transfer의 Session과 ChoiceRecallCandidate는 원래 assembly 내부에 남긴다. Input adapter가 직접 Session을 호출하거나 신규 Transfer friend를 추가하지 않는다. 현재 Transfer.Unity는 이미 Input.Unity friend를 선언하므로 아래 내부 driver bridge를 사용할 수 있다.

```csharp
object PrepareChapter1RecallForChoiceSkill(ChoicePhaseContext context);
bool ValidateChapter1RecallForChoiceSkill(ChoicePhaseContext context,
    object recallProof);
void CommitChapter1RecallForChoiceSkill(ChoicePhaseContext context,
    object recallProof);
void CancelChapter1RecallForChoiceSkill(ChoicePhaseContext context,
    object recallProof);
```

Session의 기승인 Prepare/Validate/Recall 세 API는 보존한다. 취소는 추가 내부 `CancelRecallForChoiceSkill(ChoiceRecallCandidate candidate)`를 같은 `TransferSession.cs`에 명시하는 안이다. 최종 정확 서명은 아스트라 판정 대상이며 현재 존재하는 API로 주장하지 않는다.

driver는 그 실행 단계의 실제 retained context·단일 capture·입력 receipt·결속 adapter·run·generation·tick·revision·active target·새 choice edge ordinal을 확인한 경우에만 Session의 실제 후보를 준비한다. 불투명 proof는 driver의 private pending wrapper 객체이며 원래 Session candidate와 그 context 객체를 보관한다. 새 object·복사·다른 driver/context·이전 tick·제거 대상·다른 graph는 검증에 실패한다. 아직 발행되지 않은 현재 context를 외부 값만으로 만들어 수용하지 않는다.

Session은 준비 때 다음 revision과21tick lock 계산을 checked로 끝내고 현재 target/등록/sink와 revision을 retained 후보에 결속한다. Validate는 같은 Session pending 객체 및 아직 같은 실제 상태를 검사한다. Cancel은 pending만 해제하며 modifier·전이·lock·revision을 바꾸지 않는다. 생명주기·노출 종료·제거·새 context가 오면 관련 pending을 취소한다.

한 capture 내부의 기존 lifecycle→exposure/removal 우선순위를 유지한 뒤 기술과 일반 Press를 처리한다. 전체 기술 Session·실제 환경·회수 후보의 Prepare와 Validate, 모든 checked 연산·예약 가능성 확인이 끝난 뒤만 회수를 소비한다. 압착은 driver만 실제 Session 회수를 소비하고 환경→기술 결과를 확정한다. 연대는 회수 없이 실제 환경과 기술 결과를 확정한다. 성공한 기술만 같은 tick의 일반 TransferPressed를 소비한다. Prepare/Validate 실패는 부분 pending을 취소하고 일반 Press 판정을 계속한다. Commit 실패는 전역 완료 미게시·TechnicalBlocked·후속 Press0이며 부분 성공으로 정상 진행하지 않는다.

이 bridge는 skill 결과나 실제 통과·저장 권한을 발급하지 않는다. driver가 Session을 소비했다는 증거와 environment가 실제 예약/발판을 확정했다는 증거를 각각 유지하고 기존 전체 결과 경계에서 대조한다. 가짜 회수 성공 객체·modifier 직접 설정·HP 직접 설정으로 시험을 통과시키지 않는다.

## 4. 정확 소유와 검사 인계

기존 승인 경로인 `TransferSession.cs`, `TransferSimulationDriver.cs`, `Chapter1ChoicePhaseAdapter.cs`, `ChoiceSkillContracts.cs`, `ChoiceSkillSession.cs`, `Chapter1PlayableFlowOwner.cs`, `Chapter1ClosingPresenter.cs`, `Chapter1CompletionReceipt.cs` 안의 관련 변경만 검토한다. `Chapter1RunView.cs` 및 수명주기 방식에 따른 추가 기존 파일은 승인자가 정확 차집합으로 명시해야 한다. 승인 목록 밖 파일·assembly·friend를 암묵적으로 늘리지 않는다. B1이 이미 만든 부모 meta는 기존 GUID와 원문을 재사용한다.

현재 신규 시험15개는 모두 실제 생산 구성이 미완료여서 건너뜀 상태다. 다음 구현은 기존 승인 시험 파일3개와15개 이름을 보존하면서 실제 저작 graph·InputSystem 장치 입력·실제 Save 결과·실제 capture/후보·실제 closing port를 구성해야 한다. 특히 다음을 확인한다.

- age20 실패와 새 edge21 성공, held21 실패, 같은 tick 기술 성공과 일반 Press 단회 소비.
- copied/foreign/removed/stale context·proof의 무소비 거절과 취소 뒤 상태 불변.
- 전체 준비 실패 시 일반 Press 유지, commit 실패 시 전역 완료 미게시.
- UIOnly 실제 receipt와 대화 phase 구분, 첫 중립·동일 Click+Submit 단회·표시59/60tick 경계·마지막 새 edge.
- 양분기 실제 봉쇄선 통과·실제 분기 저장 뒤 원문 대화 완료, DemoCompleted만인 경우 완료0.
- 선택한 수명주기 방식의 실제 router/세대·정리·파기 및 거점 복귀 근거.
- 재시작 시 V1 pair/branch만 복원하고 closing·현재 실행 완료는 미복원.

합성 후보 음성과 실제 정상 생산 경로를 구분하고, 앞선 B0 성공이나 건너뜀 시험을 새 경로의 수용으로 쓰지 않는다. 기존 C1C 유한 compile·저작·집중·필수 회귀·실제 양분기 플레이 관문과 Q0/9회/원격 게시 경계는 유지한다.

## 5. 아스트라 한정 판정 후보 — 독립 검수 전 Review 유지

이 절은 앞선 두 대안 가운데 **기존 종료 뒤 새 Closing UI host**만 선택한다. §2의 기존 router 유지안은 채택하지 않는다. 이 절의 정확 보정은 루나 독립 대조와 아스트라의 명시 Approved 전환 뒤에만 규범 효력을 가진다. 기존 Approved 문서의 원문·역사 지문은 그대로 둔다. 제품 구현·실행·독립 수용을 이 문서 작성으로 주장하지 않는다.

사용자 동작은 실제 봉쇄선 통과 후 새 입력으로 다섯 대사를 읽고 거점으로 돌아가는 것, 산출물은 그 경로의 저작·현재 소스 빌드·독립 양분기 증거다. 테라는 구현, 루나는 독립 검수, 아스트라는 승인·통합을 맡는다. 공유 쓰기의 실제 선행은 독립 검수된 B1 최종 입력과 C1C 통합 기준 원장이다. 게시 완료는 선행이 아니다.

### 5.1 성공·파기·새 입력의 정확 순서

REQ-C1C-UIR-001은 REQ-C1C-005/006을 구체화한다. 같은 run의 실제 봉쇄선 완료view → 기존 유일 requester인 EncounterLifetimeOwner의 Cutscene 요청과 실제 receipt → 실제 TransferCleared(DemoCompleted) → RunEndRequested/Committed(Succeeded) → 실제 CompletedBranches 원자 저장 성공 → 같은 requester의 Cutscene→Ended 실제 receipt → 마지막 실제 완료 tick 보존 → graph 비활성화 및 Destroy → graph Unity null과 closure witness의 Destroyed/Closed 모두 확인 → 새 Closing host 순서다. FlowOwner를 두 번째 mode requester로 등록하지 않는다. 저장 실패/불확실이면 Ended·host 발급·완료 모두 금지하고 기존 진단 상태로 남는다.

SYSTEM-CONTRACTS.md의 Successful demo 6단계와 InputRouter의 Ended terminal guard는 그대로 유지한다. 상위 주 계약 §13.2/§13.3의 축약 순서 중 성공 저장 뒤 DemoCompleted라고 적힌 부분은 §17이 채택한 C1C 및 이 정확 순서로 대조한다. 승인 시 이 명시 호환 보정을 함께 기록하며 그 축약을 새로운 역순 실행 권한으로 사용하지 않는다.

새 host는 기승인 미래 산출물 `Assets/Prefabs/Chapter1/Closing.prefab`을 저작·검증한 뒤 그 새 instance로 구성한다. 이번 확인에서 해당 제품 경로의 prefab은 없으며 이미 존재하거나 저작·실행됐다는 뜻이 아니다. 기존 HubUiOnly 초기화를 호출하지 않는다. 이전 router/actions를 재활성화하거나 기존 receipt의 generation을 바꾸지 않는다. 동일 lifetime owner가 실제 파기 근거와 FlowOwner의 retained demo/save를 모두 검증한 뒤 private pending entry 한 개를 발급한다. entry에는 owner/endpoint/run/원래 demo/save 객체/oldDemoGen/실제 Ended receipt·마지막 완료 tick/실제 closure 근거/newUiGen/firstUiTick을 읽기 전용으로 보관한다. `newUiGen=checked(oldDemoGen+1)`, `firstUiTick=checked(lastCompletedTick+1)`이다. 파기/로딩 대기 중 tick을 추가하지 않는다.

### 5.2 정확 입구·소유 API와 상태

다음 서명은 모두 내부 접근이며 기존 파일 안에 둔다. `Chapter1ClosingHostEntry`는 `Chapter1ChoicePhaseAdapter.cs` 내부 sealed 타입이다. 생성자는 일반 호출 권한이 아니며 모든 소비는 실제 owner pending/issued 동일 객체 검증을 다시 요구한다.

| 소유 | 정확 추가 서명 |
| --- | --- |
| EncounterLifetimeOwner | `void RequestChapter1SuccessfulClose(IChapter1ChoiceUiPort endpoint, object passage)` |
| EncounterLifetimeOwner | `void CommitChapter1SuccessfulRun(IChapter1ChoiceUiPort endpoint, object demo, object branchSave)` |
| EncounterLifetimeOwner | `Chapter1ClosingHostEntry PrepareChapter1ClosingHostEntry(IChapter1ChoiceUiPort endpoint, object demo, object branchSave)` |
| EncounterLifetimeOwner | `bool ValidatePendingChapter1ClosingHostEntry(Chapter1ClosingHostEntry entry)` |
| EncounterLifetimeOwner | `void ConsumeChapter1ClosingHostEntry(Chapter1ClosingHostEntry entry, InputRouter router)` |
| EncounterLifetimeOwner | `bool OwnsChapter1ClosingHost(Chapter1ClosingHostEntry entry, InputRouter router)` |
| EncounterLifetimeOwner | `void CancelChapter1ClosingHostEntry(Chapter1ClosingHostEntry entry)` |
| EncounterLifetimeOwner | `void RequestChapter1ClosingHostRetirement(IChapter1ChoiceUiPort endpoint, Chapter1ClosingHostEntry entry)` |
| IChapter1ChoiceUiPort | `bool ValidatePendingChapter1SuccessfulClose(object passage)` |
| IChapter1ChoiceUiPort | `bool ValidatePendingChapter1SuccessfulRun(object demo, object branchSave)` |
| IChapter1ChoiceUiPort | `bool ValidatePendingClosingHostEntry(Chapter1ClosingHostEntry entry)` |
| InputRouter | `void ConfigureChapter1ClosingUiOnlyForAuthoring(Chapter1ClosingHostEntry entry)` |
| Chapter1ChoicePhaseAdapter | `void ConfigureClosingHostForAuthoring(InputRouter router, Chapter1ClosingHostEntry entry)` |
| FlowOwner | `void BindClosingViewPort(IChapter1ClosingViewPort port)` |
| FlowOwner | `bool ValidatePendingChapter1ClosingStart(object presenter, Chapter1DemoEndReceipt demo, Chapter1BranchSaveReceipt save)` |
| FlowOwner | `Chapter1CompletionReceipt CurrentChapter1CompletionReceipt { get; }` |

입구의 opaque passage/demo/save는 Input assembly에서 Run 타입을 참조하지 않기 위한 운반값이다. 결속 FlowOwner가 실제 발급 owner·pending 동일 객체·run/분기/세대·성공 commit 및 Save Outcome/기대 revision/CommittedRevision을 검증한다. lifetime은 그 bool만 믿지 않고 실제 자기 router/완료view/clear/Ended/closure 원장을 함께 확인한다. RequestSuccessfulClose는 현재 Barrier의 실제 통과 pending에만 단회 허용하고 CommitSuccessfulRun은 실제 DemoCompleted clear 이후에만 허용한다. Prepare는 실제 Ended와 파기 완료 전 null이며 상태를 진전시키지 않는다.

entry 상태는 Pending→Issued→Retired 또는 Pending→Cancelled뿐이다. 비활성 pristine router/adapter 결속과 양측 검증을 모두 끝낸 뒤 owner가 Consume으로 동일 router에 한 번 Issued 전환하고 그 이후에만 actions를 활성화한다. runtime에서는 Pending 검증을 재호출하는 대신 OwnsChapter1ClosingHost로 Issued 동일 entry/router를 대조한다. 외부/복사 entry Cancel은 유효 pending을 지우지 않는다. 구성 실패는 actions 닫기·실제 파기·진단 정지이며 자동 새 entry 재발급0이다.

완료 후 새 사용자 completed-return token을 실제 adapter가 한 번 소비한 뒤만 retirement 요청을 한다. 새 host도 실제 Destroy 및 actions/callback closure를 끝내고 기존 Hub lifetime의 새 instance 진입으로 넘긴다. 돌아가기 요청만으로 ReturnedToHub를 게시하지 않는다. 창 종료·상위 owner 파기에서도 두 세대의 pending/issued entry와 callbacks를 무효화한다.

### 5.3 Closing 전용 엄격 행렬과 시계

REQ-C1C-UIR-002. InputRouter 내부의 별도 Closing variant는 아래 정확 행렬을 적용한다. 기존 Hub 준비·복구·리셋 가드를 완화하지 않으며 공통 private UI frame transaction만 재사용할 수 있다.

| 단계 | 필수 | 거절 |
| --- | --- | --- |
| 구성 | 비활성 pristine·actions/callback 없음·유효 pending entry·고정 adapter/endpoint | Hub/local/regular/Ordan variant 혼합, profile prepared/recovery/reset 예약, gameplay producer 참조, 재결속 |
| 활성화 | Issued entry·같은 새 router·UIOnly 초기모드·기존 GameInputActions UI callback 단일 소유 | Gameplay/ChoiceSkill map 활성, 별도 callback/action, profile I/O·launch 준비 |
| 실행 | 같은 owner/run/newUiGen·유효 issued entry·checked UI tick/ordinal·실제 UIOnly receipt | mode requester 등록/RequestMode, gameplay·전이·전투 생산 phase, 옛 source/frame·중복 frame |
| 종료/오류 | UI/Gameplay map 비활성·callbacks 제거·actions dispose·closure witness·pending 무효화 | 재활성화·Ended 해제·다음 화면 선활성화 |

기존 InputRouter FixedUpdate의 단일 입력 확정 경로에서만 새 UI 시계를 진전시킨다. gameplay producer를 빈 대역으로 만들어 시계를 돌리지 않는다. firstUiTick부터 성공한 실제 UI receipt당 한 tick이며 벽시계/렌더 프레임/ForTests 증가로 대사 시간을 채우지 않는다. -300 입력 Prepare/Validate/Commit/map 오류면 해당 UI receipt0·진단 정지다. UIOnly 전용 경로는 gameplay +110 완료view를 만들지 않는다.

첫 실제 물리 Submit/Click/Navigate 중립 프레임 이후만 버튼을 연다. pointer 위치0은 요구하지 않는다. 각 대사 표시 시작 UI tick에서 60틱 이상과 그 대사에 대한 새 중립·새 edge가 있어야 넘긴다. 59틱 edge는 거절하고 held를60틱에 재사용하지 않는다. 마지막 C01도 같은 조건을 요구한다. C02/C03와 복귀 버튼은 실제 current completion 발급 뒤만 표시한다.

RunView의 기존 enum 값은 보존하고 `Closing`과 `Ended` 두 표시 단계를 끝에 추가한다. 이 값은 입력 모드나 성공 권한이 아니다. 실제 Ended receipt/파기 대기에는 기존 잠금 표시를 유지하고 첫 유효 새 UI receipt와 presenter 시작 뒤 Closing, CH1 receipt 발급 뒤 Ended로 표시한다. 기존 Diagnostic은 기술·저장 실패 표시로 유지한다.

### 5.4 완료 포트와 두 세대의 결속

REQ-C1C-UIR-003. §2의 IChapter1ClosingViewPort 네 서명을 채택한다. BeginFromCommittedRun의 demo/save는 retained 원본이어야 하며 presenter는 구성 때 받은 entry와 실제 FlowOwner 시작 검증을 대조한다. closing-next token은 FlowOwner가 미리 소비하지 않고 결속 port로 전달한다. Presenter가 endpoint/source/entry/newUiGen/새 frame·edge/60tick을 검증해 단회 소비한다.

ClosingReceipt는 원래 DemoGeneration, 새 UiGeneration, 같은 HostEntry 객체를 별도 보관한다. UI token.Generation은 UiGeneration에만 비교하고 demo receipt.Generation은 DemoGeneration에만 비교한다. 원래 세대와 새 세대가 같다는 검사로 바꾸지 않는다. FlowOwner는 자기 run/branch/demo/save/entry, 원문 SHA·정확 대사 ordinal·마지막 새 edge, 결속 presenter pending을 모두 확인한다. 모든 checked 계산과 완료 객체 준비를 소비 전에 끝내며 port pending 소비 뒤에는 실패 가능한 외부 호출 없이 자체 CH1 receipt 한 개만 게시한다. 재시작에서 entry/closing/CH1 receipt를 복원하지 않는다.

### 5.5 실제 회수 연결의 확정 후보

REQ-C1C-UIR-004. §3의 내부 driver 네 서명 및 Session의 `internal void CancelRecallForChoiceSkill(ChoiceRecallCandidate candidate)`를 그대로 채택한다. 기존 Session Prepare/Validate/Recall 세 서명은 유지한다. Session은 private으로 남고 새 Transfer friend는 없다.

실제 -200 단일 capture에서 lifecycle→exposure/removal→aim을 먼저 처리하고, 이번 driver가 보유한 context/receipt/capture/adapter/run/generation/tick/revision/active target/choice edge ordinal과 같은 객체만 준비한다. proof는 driver의 private sealed wrapper이며 그 private Session의 실제 pending candidate를 보관한다. adapter는 wrapper를 해석하거나 Session을 호출하지 않는다. 새 context·target 제거/노출 종료·graph 종료는 Session과 wrapper pending을 함께 취소한다. 외부 proof 취소로 현재 유효 후보를 지우지 않는다.

모든 Session/환경/회수 후보와 actual target/sink 등록, age≥21·새 edge·pair/cooldown, checked revision+1/t+21/t+36/t+300 및 연대 t+60/t+150/t+420, 예약 가용성을 회수 전에 준비·검증한다. 회수는 driver만 단회 소비하고 환경→기술 결과 확정으로 이어진다. Prepare/Validate 거절은 후보만 취소하고 modifier/revision/lock/cooldown 소비0이며 일반 Press를 계속한다. 성공 기술만 같은 tick Press를 소비한다. 이미 확정에 들어간 오류는 일반 Press로 복귀하지 않는다.

### 5.6 규범 호환 보정의 정확 범위

REQ-C1C-UIR-005. 승인 대상 보정은 주 계약 §8.6 끝의 ‘부분 commit 오류가 발생하면 전역 receipt를 게시하지 않고 fault로 멈춘다’, §13.2 입력 행의 ‘부분 Commit 실패는 전역 미게시·정지’, C1C Approved §3 끝의 ‘Commit 실패는 전역 미게시·TechnicalBlocked’의 C1C 기술 적용 범위다. 이 절 승인 전 이 세 문구를 이미 보정됐다고 사용하지 않는다.

1. **-300 입력 transaction**: 기존 전체 Prepare→Validate→Commit→map→global input receipt 마지막 게시와 실패 receipt0를 그대로 유지한다. frozen batch/재진입/기존 AC-C1B-INPUT-001 및 PartialCommitBlocksAllPhases를 약화하지 않는다.
2. **-200 기술 Prepare/Validate**: 이미 실제 발행된 input receipt는 보존한다. 기술 효과0이며 일반 Press는 기존 규칙대로 계속할 수 있다.
3. **-200 회수·환경·기술 Commit의 늦은 오류**: 발행된 input receipt 및 이미 발생한 부분 물리/Session 변경은 사실대로 보존한다. 해당 tick의 Choice 성공 결과, Transfer 완료 publication, 뒤따르는 Combat/Movement 실행 및 통합 완료 publication, +110 완료view/통과/CH1 receipt는0이다. TechnicalBlocked로 닫고 후속 Press0·자동 재시도0이다. rollback했다고 주장하지 않는다. 이전 정상 tick의 역사 publication은 지우지 않으며 현재 tick 완료로 재사용하지 않는다.

이는 늦은 실패를 성공으로 수용하는 완화가 아니라 실제 발행 시점에 맞춘 실패 관측 보정이다. 기존 입력 실패 AC와 기존 Successful demo 순서는 보존하고 아래 AC-C1C-UIR-005를 추가한다. 늦은 오류에서 Movement/Combat 후속 실행이0임을 현재 생산 경로에서 보장하지 못하면 구현은 미완료다.

### 5.7 정확 파일 차집합·검사·종료 관문

REQ-C1C-UIR-006. 제품 루트 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/` 아래 기존 승인 파일만 사용하되 추가 기존 파일은 `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs`와 `Runtime/Run/Unity/Chapter1RunView.cs` 정확 두 개다. 신규 C#·assembly·action asset·friend는 추가하지 않는다. 기존 승인 InputRouter/ChoicePhaseAdapter/EncounterEntry/EncounterEvidenceView/Chapter1GameplayInputSink, TransferSession/TransferSimulationDriver, ChoiceSkillContracts/ChoiceSkillSession, FlowOwner/CompletionReceipt/ClosingPresenter, 기존 builder/validator와 승인 시험3파일 안에서만 연결한다. 입력 sink는 현재 tick 통합 완료·실패 gating의 기존 승인 소유자이며 Transfer driver가 +110 view의 발급 권위를 가져가지 않는다. Closing prefab/기존 scene/기존 원문 JSON을 재사용한다. 기존 부모 meta는 B1 독립 최종 원문을 보존한다.

C1C 통합 원장에는 두 추가 파일을 포함한 모든 수정 전/후 byteLength/SHA·소유자·REQ/AC·기존 B1 최종 기준 차집합을 기록한다. 이번 읽기 지문은 작성 중 파일의 최종 승인 지문이 아니다. 루나 검수와 Approved 전환, Root의 B1 최종 기준 인계 전 공유 쓰기는0이다.

| 추가 인수 기준 | 기존 정확 검사 선택자 연결·필수 관측 |
| --- | --- |
| AC-C1C-UIR-001 → REQ-C1C-UIR-001 | `Chapter1ClosingPersistencePlayModeTests.BothBranchesCloseOnlyAfterRealPassageAndSave`: 실제6단계·Destroy/closure·actions 비중첩·새 세대/시계·양분기 |
| AC-C1C-UIR-002 → REQ-C1C-UIR-002 | `Chapter1ClosingPersistencePlayModeTests.UiSubmitClickHeldCancelAndDuplicate`: 엄격 variant 혼합거절·물리 중립·59/60tick·held·동시Click/Submit 단회 |
| AC-C1C-UIR-003 → REQ-C1C-UIR-003 | `Chapter1ClosingPersistencePlayModeTests.RejectForgedIncompleteOrUncertainCompletion`, `RestartPreservesV1PairButNotChapterReceipt`: 두 세대·원본 pending·불확실 저장·미복원 |
| AC-C1C-UIR-004 → REQ-C1C-UIR-004 | `Chapter1ChoiceSkillSessionTests.RecallAge20_21_22AndCooldown299_300`, `Solidarity419_420AndSameTickTransfer`, `RejectForeignCopiedStaleConsumedAndOverflow`: 실제 proof/취소·checked·일반Press |
| AC-C1C-UIR-005 → REQ-C1C-UIR-005 | `Chapter1ChoiceBarrierPlayModeTests.AtomicInputFailureAndSingleCapture`: -300 receipt0와 -200 input receipt 보존/통합완료0를 별도 fault에서 기록·후속phase0 |
| AC-C1C-UIR-006 → REQ-C1C-UIR-006 | 아래 정확15개·기존 승인 회귀220 입력 재결속·소스/자산 원장·독립 대조 |

기존 나머지 일곱 선택자 `Chapter1ChoiceSkillSessionTests.PairConsentCancelAndSaveGate`, `Reservation35_36_37Hold59_60Platform149_150`, `Chapter1ChoiceBarrierPlayModeTests.AuthoredExtractionOneValidSkillPasses`, `AuthoredSolidarityOneValidSkillPasses`, `CounterweightTransferRecallChangesPhysics`, `RadiusBoundaryAndDamageRoster`, `NonlethalFallReturnsFreshGraphWithoutRunFailure`도 유지한다. 짧은 메서드명은 바로 앞 동일 클래스에 속한다. 총15개를 보고서에 완전한 이름으로 고정한다. 실제 생산 fixture 없이 Ignore로 끝나는 시험은 통과0이다.

기존 유한 compile·저작·집중·회귀·양분기 실제 플레이와 실패 재도전·현재 빌드 관문을 그대로 적용한다. 첫 실패와 반복 제한도 유지한다. 이 한정 문서의 종료는 독립 설계 대조와 명시 승인까지이며 제품 종료는 같은 최종 소스의 루나 실제 증거·아스트라 통합 수용까지다. Q0 인간 승인·9회·키·원격·일반 사용자 프로필 권한은 추가하지 않는다.

### 5.8 단일 capture와 완료view의 읽기 API

공유 인계 v3의 미연결 읽기 API는 아래 정확 소유로 한정한다. 이들도 위 Review·독립 검수·승인 관문에 포함한다.

- `TransferSimulationDriver`: `public bool ValidatePendingChoiceContext(ChoicePhaseContext context)`는 기존 승인 서명이다. `internal bool ValidatePendingChapter1Capture(SimulationTick tick, object capture)`, `internal object ReadChapter1CommittedCapture(SimulationTick tick)`, `internal bool ValidateChapter1CommittedCapture(SimulationTick tick, object capture)`, `internal bool IsChapter1ChoiceTargetAvailable(string targetId, long generation, object capture)`를 추가 후보로 고정한다.
- pending 검증은 현재 -200 실제 context/capture 객체에만 참이다. committed 읽기는 성공한 이번 -200 완료 publication과 같은 tick/capture만 반환하고 미완료·늦은 fault·다른 tick이면 null이다. committed 검증은 이미 소진된 pending context 검증으로 대체하지 않는다. target 가용성은 해당 due capture의 실제 등록·노출·제거·세대와 대조한다. capture를 새로 만들거나 두 번째 Sync/capture를 수행하지 않는다.
- `Chapter1GameplayInputSink`: `internal bool ValidateChapter1CompletedBarrierView(SimulationTick tick, object completedView)`를 둔다. 자기 router가 실제로 발급한 동일 tick의 +110 완료view 객체·등록·generation·run과 세 producer 완료를 재대조한다. Transfer driver에는 이 메서드를 추가하지 않는다. Input adapter가 결속 sink를 통해 호출한다. 이 bool은 새 통과 권한이 아니며 실제 통과 pending은 검증된 view/환경 결과/출구 겹침에 별도로 결속한다.

REQ-C1C-004 및 REQ-C1C-UIR-004/005/006, AC-WT-003/006 및 AC-C1C-UIR-004/005/006에 추적한다. late fault에서 capture나 옛 완료view가 정상 값으로 읽히지 않는 검사는 기존 `AtomicInputFailureAndSingleCapture`와 `RejectForeignCopiedStaleConsumedAndOverflow`에 포함한다.

### 5.9 추적 및 지문 시점 보정 — Review 유지

상위 AC-CH1-003은 수탈·연대 각각의 다른 기술/유담 결과·정확 저장·VD-06 네 AC를 요구한다. AC-CH1-004는 실패/미확정/미통과에서 세령·CH1 완료0 및 중복/로드/재도전의 중복 지급0을 요구한다. 이 둘은 본 접합과 직접 관련되어 §1 및 새 준비 원장의 추적 목록에 명시한다. 새 제품 동작이나 기준 변경은 없다.

- `Chapter1ChoiceSkillSessionTests.PairConsentCancelAndSaveGate`는 기존 AC-CH1-003/006 연결을 유지한다.
- `Chapter1ChoiceBarrierPlayModeTests.AuthoredExtractionOneValidSkillPasses`와 `AuthoredSolidarityOneValidSkillPasses`, `Chapter1ClosingPersistencePlayModeTests.BothBranchesCloseOnlyAfterRealPassageAndSave`는 AC-CH1-003/004를 함께 추적한다. 두 분기 정상 완주와 실패/미통과의 미노출은 별도 관측으로 남긴다.
- `Chapter1ClosingPersistencePlayModeTests.RejectForgedIncompleteOrUncertainCompletion`과 `RestartPreservesV1PairButNotChapterReceipt`는 AC-CH1-004의 미확정/저장 불확실/불완전·중복/로드/재도전 차단을 연결한다. 기존 AC-CHOICE-003 및 AC-CHOICE-001~004 전체 보존 의무도 유지한다. 서사 인과/양분기 실제 플레이를 정적 검사로 대체하지 않는다.

`preparation/c1c-ui-recall-closure-astra.v1.json`은 작성 당시 여덟 문서/인계 지문을 기록한 역사 원장으로 보존한다. 이후 주 계약 §21 추가와 B1 접근 보정의 Approved 전환으로 두 당시 지문이 현재값과 달라졌다. 이는 당시 파일을 다시 검증해 성공했다고 소급할 근거가 아니다. 새 `c1c-ui-recall-closure-astra.v2.json`은 이번 보정 초안 및 현재 권위 문서 지문, 기존 인계의 당시 관측 성격을 구분한다. 기존 솔/테라 인계 안의 제품 지문은 각 작성 시점의 관측이며 B1 현재 또는 최종 수용 지문으로 승격하지 않는다.

Closing prefab은 아직 저작되지 않은 기승인 미래 산출물이며 위의 재사용 표현은 기승인 경로·설계 범위를 뜻한다. 파일 존재·자산 SHA·실제 instance/입력/완주 증거를 만들지 않는다. B1 변경 후568 원장은 별도 검수 대상이고 B1 최종 수용 및 C1C 통합 기준/승인 관문 전 공유 쓰기0이다. 이번 수정 뒤에도 본 문서 상태는 Review이며 루나 최종 독립 검토와 아스트라 명시 승인이 필요하다.
## 6. 아스트라 한정 승인 — Approved

2026-10-03 아스트라는 루나 독립 규범 검토 `output/chapter1-playable-r1/evidence/c1c-ui-recall-closure-review-luna6.v1.json` SHA `7A4D46A876371169CE980573BC19638CDCF66C200F7A63C0753484F34348D3B5`를 직접 읽고 P0=0/P1=0인 정확 §5.1~5.9를 승인한다. 검토 원문 SHA `0411221DB2A89B6DA3B0658D40CB69A7AD6E4C49E8DF948AC42F64ABBA252EBB`와 준비 v2 SHA `9979AD8951F8CD04EB4AC1AEEE28AFAB6A0178D3489ED2CA2DDD083BAE49E35B`를 역사로 보존한다. 상태·승인 기록 이외 구현 규범은 검수 원문 그대로다. 앞선 Review/미승인/승인 전 표현은 작성 당시 이력이며 이 절이 정확 승인 범위에서만 대체한다. §1~4의 두 대안·미정 서명은 §5의 확정 후보와 충돌하면 §5를 따른다.

승인은 기존 성공6단계와 Ended guard를 보존한 새 Closing UIOnly host, 동일 원정의 실제 종료/파기/actions closure·oldDemoGen/newUiGen·UI 시계·entry 단회권한, private Session 회수의 driver4API/Session Cancel·실제 capture·checked 전체 준비, 정확 포트·읽기 API·두 추가 기존파일 및 기존15검사/220입력에 한정한다. Closing prefab은 기승인 경로의 미저작 미래 산출물이다. 새 C#·assembly·action/friend·저장 스키마·지속 CH1 완료 필드를 추가하지 않는다.

§5.6의 규범 호환 보정을 같은 묶음으로 명시 승인한다. 주 계약 §8.6/§13.2 및 C1C 한정 계약 §3의 포괄적인 commit 실패 미게시 문구는 C1C의 -300 입력 확정 실패에는 기존 input receipt0를 유지하고, -200 회수/환경/기술의 늦은 오류에는 실제 이미 발행한 input receipt를 보존하되 현재 tick의 통합 완료와 후속 phase/통과/CH1 결과0으로 적용한다. 기존 원문을 조용히 재해석하거나 늦은 실패의 input receipt0·rollback을 주장하지 않는다. AC-C1B-INPUT-001과 기존 실패 검사·성공6단계를 유지하고 AC-C1C-UIR-005를 추가 검증한다.

이 설계 승인과 충돌 없는 신규 후보 준비는 B1 완료를 기다리지 않는다. 공유 project 쓰기는 별도 B1 r2 시험 CS0012의 해결·독립 최종 수용/지문과 `c1c-integration-base.v1.json`의 정확 차집합·소유·독립 대조 뒤다. B1 현재568이나 성공6 컴파일을 최종 제품 수용으로 간주하지 않는다. B1 시험 참조 Review는 별도이며 본 C1C 승인으로 그 변경이나 재컴파일을 허용하지 않는다.

테라 구현→루나 독립 소스·생산 fixture 검수→기존 유한 compile/저작/집중/필수 회귀→같은 최종 소스의 실제 양분기·실패 재도전·현재 빌드→아스트라 통합 수용을 유지한다. 새 승인 작성은 제품 쓰기·Unity·시험·프로필·Q0·원격0이며 CH1 완주나 빌드 수용이 아니다.
