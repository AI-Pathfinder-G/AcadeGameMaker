# C1C 물리 중립 읽기 경계 구체화

- 상태: Approved. 아래 아스트라 합본 한정 승인으로 물리 중립 읽기 및 완료·복귀의 정확 내부 API 세 개만 채택한다. 기존 B1 최종 수용·C1C 통합 관문과 구현·실행 수용은 별도다.
- 소유: 기존 InputRouter만 실제 장치를 읽고 기존 Chapter1ChoicePhaseAdapter만 결속된 frame으로 중립을 관측한다. 구현 테라, 독립 대조 루나, 승인 아스트라다. B1 최종 기준 전 공유 쓰기0은 유지한다.

## 원인과 정확 API 후보

현재 UiSemanticFrameV1의 SubmitPressed/ClickPressed는 edge이고 held가 아니다. 후보 adapter의 `if(!edge) _neutral=true`는 계속 누른 입력을 중립으로 오인할 수 있다. 기존 Chapter1HubNeutralReady는 Hub variant에만 속하므로 Closing에서 사용하거나 Hub 가드를 완화하지 않는다.

기존 `InputRouter.cs`에 다음 내부 읽기 서명 하나를 둔다.

```csharp
internal bool TryReadChapter1ClosingControlsNeutral(
    Chapter1ChoicePhaseAdapter source,
    Chapter1ClosingHostEntry entry,
    UiSemanticFrameV1 frame,
    out bool neutral);
```

반환 bool은 읽기 증거 유효성이고 out neutral은 실제 해당 frame의 물리 상태다. 실패 시 neutral=false다. 일반 `Chapter1ClosingControlsNeutral` 속성처럼 문맥 없는 bool을 권한으로 사용하지 않는다. 새로운 receipt/token 타입·public API는 없다.

호출 조건은 현재 정상 Closing variant, 살아 있는 pristine 때 결속한 동일 adapter/router/entry, lifetime의 Issued 동일 entry/router, 같은 endpoint/run/newUiGen, 현재 UIOnly receipt 및 CurrentUiFrame와 전달 frame의 전체 값 일치다. UiSemanticFrameV1은 값 타입이므로 ReferenceEquals를 사용하지 않는다. router private retained publication과 현재 receipt/frameOrdinal/modeEpoch/tick 및 별도 private neutral proof의 receipt를 함께 대조한다. 외부 router/source/복사 entry/이전 frame/미발행/retired/fault에서는 false다. frame의 값 복사만으로 source 권한을 얻지 못한다. 이미 중립을 관측한 옛 frame을 새 대사에서 다시 arm하는 것은 adapter의 소비 ordinal로 차단한다.

## 관측 시점과 소비

새 Closing variant의 기존 단일 입력 transaction이 동작할 때만 UI Submit/Click/Navigate의 실제 물리 상태를 읽어 해당 후보 frame에 결속한다. Submit/Click은 IsPressed와 실제 bound button control의 기존 C1-A 중립 기준(EvaluateMagnitude>0.1이면 held)을 함께 확인하고 Navigate는 bound control의 EvaluateMagnitude>0이면 중립이 아니다. pointer 위치는 제외한다. 새 장치 callback이나 별도 장치 reader를 만들지 않는다. 기존 Hub 경로의 필드와 규칙은 변경하지 않는다.

모든 검증·checked 연산·map 작업이 끝난 성공 transaction에서만 private neutral 값과 그 receipt/frame proof를 함께 보관하고 실제 input receipt를 마지막에 발행한다. 초기값/실패/미발행/세대교체/종료는 proof 무효다. 읽기 API는 장치를 다시 샘플링하거나 clock/중립 latch를 진전시키지 않는다. 현재 frame 이후 장치 변화는 다음 성공 transaction에서 반영한다.

adapter는 Closing일 때 위 API 유효성 및 neutral=true가 관측된 새 frame에서만 중립을 arm한다. 최소한 Submit/Click/Cancel edge가 있는 frame에서는 neutral arm0이고 그 frame을 다음 요청의 release 증거로 재사용하지 않는다. 중립 frame 뒤 더 높은 frameOrdinal의 새 Submit/Click edge만 허용한다. held frame에서 edge가 없다는 이유로 arm하지 않는다. 첫 화면과 대사별 새 중립·59/60틱 경계는 기존 §5.3대로다. Cancel에 새 동작을 추가하지 않는다.

## 검사와 인계

REQ-C1C-UIR-002/003 및 REQ-C1C-004, AC-C1C-UIR-002/003, AC-CHOICE-001/002에 결속한다. 기존 `Chapter1ClosingPersistencePlayModeTests.UiSubmitClickHeldCancelAndDuplicate`에서 활성화 전 Submit/Click/Navigate held→edge없는 frame에도 중립false→실제 release frame neutraltrue→그 이후 새 edge 단회, 동일 frame release/edge 거절·옛 proof/다른 source/entry 거절을 확인한다. 대사 넘어갈 때 held 유지와59틱 거절 후60틱 held 자동진행0도 포함한다. 기존 15선택자와 검사 파일을 유지하고 제품 시험용 setter·상태 주입·UiSemanticFrame ABI 변경0이다.

판정은 이미 승인한 물리 중립 동작과 읽기 소유자의 정확 서명 구체화다. 루나가 위 실제 입력 시점/소유/검사 연결을 대조한 뒤 아스트라가 한정 채택하면 신규 후보 서명에 연결할 수 있다. 공유 InputRouter 구현은 기존 B1 최종/통합 관문 뒤다. 이 문서의 제품 쓰기·Unity·시험·프로필·Q0·원격은0이다.

## 완료·복귀 호출 흐름 합본 보정 후보

이 절은 테라 `staging/c1c/shared-api-handoff-terra61.v4.json`이 남긴 두 API 공백을 같은 Closing 행동/소유 묶음으로 닫는다. 앞선 물리 중립 Review에 대한 루나 P0/P1=0 보고 `evidence/c1c-closing-neutral-read-review-luna6.v1.json` SHA `B88771866600FADF3F18C09E133311083A71EC88226F8937156F80700D40F015`는 그 좁은 검토로 보존한다. 아래 완료/복귀 보충을 이미 독립 검수했다고 주장하지 않으며 합본 최종 대조 후 한 번 승인한다.

기존 `Chapter1ChoicePhaseAdapter.cs`의 adapter와 같은 파일의 `IChapter1ChoiceUiPort`에 정확 두 서명을 추가하는 후보다.

```csharp
// Chapter1ChoicePhaseAdapter
internal void CompleteClosingFromCommittedReceipt(
    IChapter1ChoiceUiPort endpoint,
    Chapter1ClosingHostEntry entry,
    object completion);

// IChapter1ChoiceUiPort: 기존 FlowOwner가 구현
bool ValidateCommittedChapter1Completion(
    Chapter1ChoicePhaseAdapter source,
    Chapter1ClosingHostEntry entry,
    object completion);
```

Input은 Run.Unity를 참조하지 않으며 object를 자체 완료 권한으로 취급하지 않는다. FlowOwner의 interface 구현은 실제 내부 Chapter1CompletionReceipt 형식·자기 CurrentChapter1CompletionReceipt와 동일 객체·자기 원정/분기·retained 원본 demo/save·소비된 실제 closing receipt/port·현재 issued entry/새 UI generation과 해당 source/router 결속을 모두 대조한다. 아직 완료를 발급하지 않았으면 false다. 재시작 값 복사·bool setter·다른 owner/entry/router/receipt는 false다. 검증은 발급·저장·소비·상태 진전을 하지 않는 읽기만이다.

adapter 입구는 정상 Closing variant/source, `_endpoint` 동일성, 구성 때 받은 동일 entry, lifetime의 issued owner/router, 현재 phase Closing, 아직 completion 수신0을 먼저 확인하고 위 endpoint 검증을 호출한다. 실패는 모든 상태를 유지한 채 거절한다. 전 조건 성공 뒤 실제 completion 원본을 한 번 보관하고 adapter phase를 Ended로 바꾸며 pending UI를 비우고 중립을 해제한다. 그 시점 현재 발행 frameOrdinal을 복귀의 하한으로 보관한다. 같은 완료 재전송·다른 완료 재전송은 거절하고 이전 상태를 덮지 않는다.

전체 순서는 실제 새 UI frame/물리중립→새 closing-next→Presenter 단회 토큰 소비/다섯째 C01 완료→원본 pending ClosingReceipt→FlowOwner의 기존 TryCommitChapterCompletion 원자 단회 발급→별도 후속 표시 연결로 CompleteClosingFromCommittedReceipt→C02/C03·복귀 표시→그 완료 연결 이후 더 높은 frameOrdinal의 실제 물리 중립→더 높은 새 edge의 completed-return→토큰 단회 소비→lifetime retirement→실제 Destroy/actions closure→기존 새 Hub 순서다. 마지막 대사의 같은 Submit/Click을 거점 복귀로 재사용하지 않는다.

기존 CH1 발급 transaction의 ‘pending closing 소비 후 실패 가능한 외부 호출 없음’은 유지한다. adapter 통지는 receipt를 발급하는 그 원자 구간의 일부가 아니라 완료된 발급 뒤 별도 UI 연결이다. 이 후속 연결이 실패하면 이미 발행된 CH1 receipt를 취소/재발급하지 않고 진단 정지·복귀 token0으로 남는다. 통지 실패를 정상 거점 도착으로 표시하지 않는다. 완료 발급 자체는 실제 대사와 저장의 원본 증거에만 의존한다.

completed-return의 ValidatePendingUi는 Ended·자기 retained completion과 endpoint의 현재 완료 검증·같은 entry/router·새 물리 중립/새 edge·현재 frame을 모두 재대조한다. FlowOwner가 해당 token을 기존 ConsumePendingUi로 먼저 한 번 소비한 뒤 이미 승인된 RequestChapter1ClosingHostRetirement(endpoint,entry)를 호출한다. 입력 mode는 계속 UIOnly이며 이전 demo router의 Ended guard나 신규 입력 mode 전환을 추가하지 않는다. 거점 활성화/Returned는 실제 host 정리 후만이다.

REQ-C1C-UIR-001/002/003 및 REQ-C1C-005/006, AC-C1C-UIR-001/002/003, AC-CH1-003/004/007에 연결한다. 기존 세 검사 `UiSubmitClickHeldCancelAndDuplicate`, `BothBranchesCloseOnlyAfterRealPassageAndSave`, `RejectForgedIncompleteOrUncertainCompletion`에서 원본 완료 전달1·복사/외부/미발급/중복0·최종 edge 복귀 재사용0·완료통지 실패 진단·새 중립/edge 후 실제 파기/거점 순서를 함께 확인한다. `RestartPreservesV1PairButNotChapterReceipt`는 retained completion/entry/Ended 표시의 재시작 복원0을 유지한다. 기존15선택자와 파일 내 보강이며 새 시험 파일이나 성공 stub은 없다.

정확 수정 후보 파일은 이미 승인한 InputRouter, Chapter1ChoicePhaseAdapter, Chapter1PlayableFlowOwner 및 기존 ClosingPresenter의 연결과 기존 시험 파일뿐이다. 새 C#/assembly/friend/프로필/semantic ABI는0이다. 물리 중립과 완료복귀 두 공백은 합본 독립 검수/승인 한 번으로 관리하고 테라 v4의 다른 미완료 구현·fixture/공유 연결을 완료됐다고 부르지 않는다. B1 최종·C1C 통합 관문 전 공유 제품 쓰기는 계속0이다.

## 아스트라 합본 한정 승인 — Approved

2026-10-07 아스트라 `gpt-6-astra`는 루나 `gpt-6-luna`의 합본 독립 규범 검토 `output/chapter1-playable-r1/evidence/c1c-closing-input-completion-review-luna6.v1.json` SHA `8B88FFD677D8407AC3EAC48D19C414A9B2600DEAC7520A1952A59848F220F774`를 직접 읽고 P0=0/P1=0인 이 문서의 물리 중립 읽기와 완료·복귀 호출 흐름 전체를 한 묶음으로 한정 승인한다. 검수 원문 SHA `9BD3C48C901C5478E7EAADB18A7496F02A7F773CED18F71267BC0AB6981847C7`, 준비 원장 SHA `D395C8A1C13158E63C31C5B527DAD01DE2300D077967EE931FAA9D2B7F1ED037`, 공유 인계 v4 SHA `8A41D8C75713257143A40D48B8879C67B4AE0BE2E4A57ADCECD324A7CD88B455`를 당시 증거로 보존한다. 앞선 중립 단독 루나 검토 SHA `B88771866600FADF3F18C09E133311083A71EC88226F8937156F80700D40F015`를 완료·복귀 합본 검수로 확대하지 않는다. 앞선 Review·후보·승인 전 표시는 작성 당시 이력이며 이 절이 정확 승인 범위에서만 대체한다.

승인하는 추가 서명은 기존 `InputRouter.cs`의 `internal bool TryReadChapter1ClosingControlsNeutral(Chapter1ChoicePhaseAdapter source, Chapter1ClosingHostEntry entry, UiSemanticFrameV1 frame, out bool neutral)`, 기존 `Chapter1ChoicePhaseAdapter.cs`의 `internal void CompleteClosingFromCommittedReceipt(IChapter1ChoiceUiPort endpoint, Chapter1ClosingHostEntry entry, object completion)`, 같은 파일의 기존 `IChapter1ChoiceUiPort`에 두고 FlowOwner가 구현하는 `bool ValidateCommittedChapter1Completion(Chapter1ChoicePhaseAdapter source, Chapter1ClosingHostEntry entry, object completion)` 정확 세 개다. 새 파일·assembly·friend·저장 스키마·semantic frame ABI·완료 발급자·입력 장치 소유자를 추가하지 않는다. B1 §24의 별도 QA 접근 예외를 이 승인으로 확대하지 않는다.

물리 중립은 새 Closing variant의 기존 단일 입력 transaction에서 실제 Submit/Click의 IsPressed 및 bound button control 크기와 Navigate control 크기를 관측하고, 성공한 실제 UIOnly receipt와 frame 전체 값·private publication proof·같은 issued entry/router/source에 결속한다. 읽기는 재샘플·tick/ordinal·중립 latch 진전0이며 현재 frame 이후 장치 변화는 다음 성공 transaction에서만 반영한다. 무edge·값 복사·이전 frame·다른 source/entry는 새 권한이 아니다. 첫 화면과 매 대사에서 새 물리 중립 이후 더 높은 frameOrdinal의 새 Submit/Click만 허용하고, edge가 있는 frame 자체를 중립으로 다시 사용하지 않는다. 59틱 거절 후 held60틱 자동 진행0·동시 Click/Submit 단회·Cancel 새 동작0을 유지한다.

완료는 같은 현재 원정·분기·retained 원본 demo/save·실제 저장 결과와 기대/확정 revision·원래 DemoGeneration·새 UiGeneration·현재 issued entry/router·결속 Closing port가 소비한 원본 closing receipt를 검증한 FlowOwner의 현재 원본 Chapter1CompletionReceipt에만 결속한다. Input은 Run.Unity를 참조하지 않고 object를 완료 권한으로 해석하지 않는다. 완료 검증은 읽기만이다. 기존 원자 발급의 checked 준비→pending closing 단회 소비→실패 가능한 외부 호출 없는 자체 CH1 receipt 게시를 유지한다. adapter 통지는 그 원자 구간이 끝난 뒤 별도 연결이며 실패하면 원본 receipt를 보존하고 진단 정지·복귀 token0으로 남는다. 취소·재발급·자동 재시도·정상 거점 도착 표시는0이다.

정상 후속 연결은 실제 원본 완료를 한 번 보관한 뒤 adapter Closing→Ended, pending 비우기·중립 해제·현재 frameOrdinal 하한 고정, 그 하한보다 높은 frame의 새 물리 중립, 다시 더 높은 frame의 새 completed-return edge 순서다. 마지막 C01 edge의 복귀 재사용과 완료 중복 전달은0이다. Ended는 표시·adapter 상태이며 입력은 계속 새 Closing variant의 UIOnly다. completed-return은 같은 현재 완료·entry/router·현재 frame을 재검증하고 FlowOwner가 기존 ConsumePendingUi로 단회 소비한 뒤 기존 RequestChapter1ClosingHostRetirement를 요청한다. 실제 graph Destroy·입력 map 비활성·callback 해제·actions dispose 및 closure 근거가 끝난 뒤에만 기존 새 Hub 생성과 Returned를 허용한다. 재시작은 V1 pair/CompletedBranches만 보존하고 completion/closing/entry·Ended 표시를 복원하지 않는다.

REQ-C1C-UIR-001/002/003 및 REQ-C1C-004/005/006, AC-C1C-UIR-001/002/003 및 AC-CHOICE-001/002, AC-CH1-003/004/007에 연결한다. 기존 Chapter1ClosingPersistencePlayModeTests의 UiSubmitClickHeldCancelAndDuplicate, BothBranchesCloseOnlyAfterRealPassageAndSave, RejectForgedIncompleteOrUncertainCompletion, RestartPreservesV1PairButNotChapterReceipt 안에 위 정상·음성·재시작 관측을 보강한다. 기존 전체 15선택자·필수 회귀220 입력의 현행 재결속·유한 실행·동일 최종 소스의 루나 양분기 실제 완주·실패 재도전·현재 빌드 관문을 유지한다. Assert.Ignore로 끝나는 미완성 생산 시험은 통과나 수용0이다.

정확 제품 수정은 기존 승인 InputRouter, Chapter1ChoicePhaseAdapter와 그 내부 interface, Chapter1PlayableFlowOwner, Chapter1ClosingPresenter 및 기존 시험 파일 범위다. 기존 성공/회수/수명주기 계약의 나머지 범위는 주 계약 §17/§22와 기승인 문서를 그대로 따른다. 충돌 없는 준비와 공유 제품 통합은 구별한다. 공유 제품 쓰기는 B1 독립 최종 수용·최종 source/meta/asmdef 지문 및 c1c-integration-base.v1.json의 정확 차집합·소유와 루나 대조, 해당 project Unity 종료 뒤다. B1 최소 컴파일 통과는 이 최종 관문을 대신하지 않는다.

이후 구현 담당은 ADR-0043에 따른 솔 `gpt-6.1-sol`, 독립 검수는 루나 `gpt-6-luna`, 최종 통합은 아스트라다. 테라라는 과거 역할·파일명·실제 모델 표기는 역사로 보존하며 신규 테라 배정은 없다. 이번 승인은 설계와 정확 API 구체화에 한정하고 구현 완료·시험 성공·독립 통합·CH1 완주를 수용하지 않는다. 승인 작성의 제품·스테이징 소스 쓰기·Unity·컴파일·시험·프로필·Q0·원격 실행은0이며 기존 인간 승인·키·세대·9회·게시 관문은 그대로다.
