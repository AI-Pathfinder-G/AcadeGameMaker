# C3 R7 실제 Submit 검증의 동등 PlayMode 이전 제안

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007 및 AC-M5D7QC3-006/009/010을 추적한다. 실제 읽기 검토와 제안 작성만 수행했다. 아스트라의 제한 승인 전 구현하거나 실행하지 않는다. 기존 실패·관측·선택 원장은 역사로 보존한다.

## 실제 근거와 한계

`artifacts/c3-r7-submit-observation-r1.xml`을 재조회했으며 SHA-256은 `9675F13A7E8E0485CB96072BE76F57A189D260BFF358E390A5FE70AFA65ECDB3`이다. 실제 14.455652초, 단일 AC006 실패이며 관측 후 원래 Submit assertion이 실패했다. 검증 JSON은 이름 차이 0, 입력 884개 전후 차이 0이다. QA 반환 5와 부모가 확인한 실제 편집기 종료 2를 유지한다. 관측의 성공을 원래 시험 통과로 바꾸지 않는다.

XML의 다섯 관측은 모두 `LatestUpdateType=Editor`, `IsPlaying=false`, `IsFocused=false`, UI callbacks 등록 true, `_callbackOrdinal=0`, pending Submit false이다. 첫 관측의 suppression/quarantine은 true이고 released 갱신 후 false로 해제되어 유지된다. 진단 오류는 0이다. 따라서 억제가 released 후에도 계속됐다는 설명과는 다르며 설치된 패키지의 Editor action 제외 경로와 일치한다. 장치 Enter 값과 action 처리 자체는 관측하지 않았으므로 단일 원인을 확정하지 않는다. 제품 runtime 수정 근거로 사용하지 않는다.

## 동일 구성과 가장 작은 시험 위치 변경

현재 Edit 시험의 `Create(true)` 인자는 **prompt**이다. 처음 primary 파일은 정상 default이며 launch 완료 후 previous에 정상 default를 추가한다. 회복 입력이나 notice를 만드는 인자가 아니다. 현재 Play `Fixture.Create(false)` 인자는 **recovered**이고, false이면 동일한 primary default를 사용한다. 327행에서 launch·latch·최초 발행 완료 후 previous default를 이미 추가한다. 따라서 두 경로 모두 정상 primary default→정상 launch→previous default 추가→DecisionRequired 조건이며 회복 notice 경로를 새로 넣을 필요가 없다.

Edit는 정상 Presenter.Activate API로 최초 의도를 만든다. Play는 기존 `SelectInitialNewGame`의 실제 Down→release→Enter→release 입력으로 최초 실제 Q-A 의도와 Q-B request를 만든다. Play fixture 320행의 키보드 등록은 호스트 활성화 이전이며 클래스 22행은 기존 `InputTestFixture`를 상속한다. 기존 TestFramework 조립 참조와 정상 시험 runtime을 사용한다. 이 변경은 초기 발급을 실제 입력 경로로 강화하며 원래 opaque 권한을 제조하거나 presenter/value만으로 pending을 고르지 않는다.

기존 Play `AC006_ImmediateReadyFactoryDiscardsActualSubmitBeforeNewTake`(81–100행)가 이미 fresh cursor Ready, 무발행 BaselinePending, 첫 실제 Submit 폐기와 다음 actual take를 검증한다. 같은 runtime `PrepareNewGameSuccessor`(Presenter 237–246행)의 정상 `UiSemanticFrameCursorV1.Create`를 사용하며 특수 factory나 다른 immediate-ready 권한을 만들지 않는다. 그러므로 중복 신규 사례를 만들지 않고 이 기존 사례에 Edit의 누락 검증 전체를 옮기는 안을 우선 제안한다. Edit의 AC006 하나와 해당 시험에만 사용된 `SubmitObservation` helper/호출만 제거한다. assertion 삭제로 coverage를 줄이는 변경이 아니라 아래 동등 검증의 이전이다.

## 보존할 전체 검증과 입력 순서

| 경계 | 기존 검증과 이전할 검증 |
|---|---|
| 원래 실제 권한 | 실제 초기 request take→AcceptNewGame→DecisionCapability, Cancel→실제 RearmCapability→Rearm. 값/행으로 실제 handle을 선택하지 않는다. 원래 cursor·retained intent·최초 taken request를 저장한다. |
| 새 커서 무발행 | successor cursor의 실제 State=Ready; Presenter Update/Fixed만 수행해도 BaselinePending=true. 새 successor 생성 후 첫 Submit 이전에 `Publish()` 등 빈 프레임을 추가하지 않는다. |
| 첫 실제 Submit | 정상 `Publish(Key.Enter)` 후 원래 `CurrentUiFrame.Value.SubmitPressed=true` assertion을 유지한다. Presenter Fixed/Late 이후 BaselinePending=false, successor Phase=Ready, Retained=null, actual TryTake=false 및 out handle=null을 요구한다. |
| 원래 역사 | 첫 프레임 폐기 이후 원래 `_cursor` 참조 동일, `_retainedIntent` 값 동일, `_takenRequest` 값 동일, Q-B State=RequestTaken을 모두 옮긴다. 역사 슬롯을 대입/교체하지 않는다. 새 `_successor.Cursor`와 원래 `_cursor`를 혼동하지 않는다. |
| 다음 정상 선택 | 첫 프레임 폐기 후에만 실제 release Publish→Presenter Fixed/Late→Enter Publish를 수행한다. 두 번째 실제 Submit=true를 명시하고 Presenter Fixed/Late→실제 take→실제 intake 성공을 요구한다. 결과 Outcome=DecisionRequired, 새 실제 DecisionCapability는 이전 capability와 다른 참조임을 모두 요구한다. |

기존 Play의 `SelectInitialNewGame` 마지막 release는 **Rearm 이전**이다. 이는 새 첫 프레임 폐기 조건을 회피하는 빈 발행이 아니다. Cancel/Rearm 이후 첫 actual publish가 Enter라는 순서를 고정한다. 새 첫 true 프레임을 false로 바꾸거나 무시하고 다음 true만 검증하지 않는다. 단순 값 주입, 직접 action callback, 가짜 프레임, 사적 수명주기 호출을 추가하지 않는다.

Play fixture 419–434행의 API는 정확 assembly-qualified runtime 타입·declaring type·full params/byref·return 검증과 실제 opaque 반환 타입 확인을 사용한다. `AcceptNewGame(IssuedNewGameRequestV1, DesktopProfileLaunchAdapterV1, InputRouter) -> NewGameConfirmationStartResultV1`, `TryTakeNewGameForConfirmation(owner,presenter,router,out IssuedNewGameRequestV1) -> bool`, `Cancel(NewGameDecisionCapabilityV1) -> NewGameConfirmationStartResultV1`, `Rearm(HubNewGameRearmCapabilityV1) -> void`를 그대로 사용한다. Outcome 추가 검증은 실제 ResultType의 `get_Outcome() -> NewGameConfirmationOutcomeV1`에 정확 결속한다. 기존 `Decision`은 실제 `get_DecisionCapability()`에 결속하며 새 결과를 forged/default 값으로 대체하지 않는다.

이전되는 읽기 assertions의 정확 필드는 Presenter의 `_cursor: UiSemanticFrameCursorV1`, `_retainedIntent: Nullable<HubMenuIntentV1>` 및 Q-B의 `_takenRequest: Nullable<HubMenuIntentRequestV1>`이다. 정확 선언 타입과 타입은 기존 runtime 선언을 다시 대조해 동결한다. successor의 Cursor/BaselinePending/Phase/Retained는 기존 Play 읽기와 같은 실제 slot에서 읽는다. 기존 `SnapshotField`의 선언/타입 검증을 활용하거나 이 필드에 한정한 정확 읽기를 사용하며 이름 단독 fallback이나 사적 상태 대입을 추가하지 않는다. 기존 Play 전체 읽기 도우미를 범용 재설계하지 않는다.

## 선택과 승인 범위

우선안은 현재 Edit 241개에서 해당 한 사례를 이전하여 **잠정 Edit 240개·Play 15개**이다. 기존 Play 사례의 이름은 유지하고 검증을 강화하므로 Play 16개가 필요하지 않다. 구현 후 정확 선언 파서와 XML 이름 대조로 재집계하기 전 이 수치를 확정 실행 결과로 쓰지 않는다. 독립 검수에서 동등성이 깨지는 구체적 구성 차이가 확인되는 경우에만 새 별도 Play 1개를 두는 240/16안을 별도 승인으로 검토한다.

377개 decision 행의 ID·내용·순서·역할·결과 및 91개 고정 행렬 분할은 변경하지 않는다. 해당 행 매핑/검증기도 이 이동 때문에 변경하지 않는다. 기존 180초 NUnit 제한을 유지한다. 이번 bounded amendment는 기존 신규 Edit/Play Owner 시험 두 파일과 새 정확 집중 선택 원장에 한정한다. 관측 helper 제거는 실패 기록을 지우는 것이 아니며 이전 원본 소스 지문·XML·진단 기록은 그대로 보존한다. runtime·InputSettings·ProjectSettings·asmdef·friend·공개 API·자산·기존 회귀 선택은 불변이다.

이 설계로 시험 성공을 미리 주장하지 않는다. 후속 실행은 승인된 강화 Play 사례와 필요한 정확 선택의 실제 증거를 남기며 실제 종료·실패/건너뜀/판정보류·이름/중복·전후 입력 지문을 대조해야 한다. 기존 실패를 소급 수용하거나 C3 전체/다른 costly 선택의 수용으로 확장하지 않는다.

## 읽은 자료의 지문

- 현재 관측 포함 Edit 시험: `DF9FE80D362457ED8DEBB03556C644F8D669ADF03C20CBE74BFD5A695B691B3C`.
- 현재 Play 시험: `20B7EA2340402149B20DD0D7080FD4DFA1AFF400E701C6BE2050370E001BA3A4`.
- 실제 관측 검증 JSON: `EE964F06EC07582F2B2619612AB8C8097D980F98C475C3CEE3D02B8130E123D8`.
- 실제 관측 QA 반환 JSON: `A8B6E934AA81A3E1C8E7FBF15ABF10D8024492E7BB0B1ABC98FAA01EA1987C30`.
