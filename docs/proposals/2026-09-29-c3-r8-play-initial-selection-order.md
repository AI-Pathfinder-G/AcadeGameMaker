# C3 R8 Play 최초 선택 순서의 제한 진단·보정 제안

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007 및 AC-M5D7QC3-001/006/009/010을 추적한다. 실제 소스/XML 읽기와 이 제안 작성만 수행했다. 구현·시험 실행·제품 계약 변경은 하지 않았다. 아스트라 승인 전 수정하지 않는다.

## 실제 실패 위치

`artifacts/c3-r8-play-probes-r1.xml` SHA-256 `E8C6AFA445998AB0AD397D0DFA1436A9DAE6DEACCBD2588C53C93FB1D67D8556`을 직접 재조회했다. AC001은 7.630223초, AC006은 7.369897초에 모두 `Fixture.Take:440`의 `TryTake=true` assertion이 실패한다. 최초 실제 Submit=true와 RequestReady 확인 이후의 실패이며, 정상 비활성화나 새 즉시 Ready cursor 경계에는 아직 도달하지 않았다. 따라서 시험 위치 이전이 이들 경계의 실행 성공을 해결했다는 증거가 아니다. 실제 편집기 종료 2·QA 반환 5·2개 실패·이름/중복 차이 0·입력 884개 전후 차이 0인 실행 결과를 보존한다.

## 정확 인증 경로와 release 가설의 한계

`HubMenuIntentHandoffOwnerV1.TryTakeNewGameForConfirmation:172–218`은 게이트 획득 후 실제 C3 상태를 검증하고 다음 경계에서 false를 반환한다.

1. 180행: 정확 owner/presenter/router 결속, `CanBindIssuance`, 현재 presenter 결속.
2. 181–183행: 현재 실제 live request와 RequestReady 단계.
3. 184–185행: 원본 request 검증, **Item=NewGame**, `MatchesImmutableHandoff`.
4. 최초 경로 190행: 원래 `TryTakeRequest`가 실제 request를 한 번 가져옴.

`CanBindIssuance`는 Owner 135–138행의 실제 epoch/상태/빈 슬롯/역사 조건을 확인한다. `MatchesConfirmationCohort`는 Presenter 225–227행의 원래 참조 결속이다. `MatchesImmutableHandoff` 229–235행은 원래 `HubEntryHandoffReceiptV1`을 보존된 `_handoff`와 비교한다. request의 `_receipt` 타입도 같은 launch 영수증이다(Q-B 12–23행). 이것은 새 `CurrentUiFrame`의 입력 발행 영수증과 다른 계약이다. 현재 코드에서 마지막 release 발행만으로 이 원래 영수증의 동일성이 바뀐다는 경로는 확인하지 못했다.

실제 opaque handle 발급은 위 검증 뒤 200–208행에서만 일어난다. `AcceptNewGame` 146–164행은 발급된 동일 handle의 정상 CWT 분류를 앞뒤로 확인하고 내부 Validate/Consume/관찰을 수행한다. 최초 두 실패는 그 전에 발생했다. 현재 두 실행만으로 Owner intake·Cancel·새 입력 세대 또는 제품 무결성 오류를 주장하지 않는다.

## 최초 Down이 무시되는 정적 경로

현재 Play `SelectInitialNewGame` 425–432행은 비회복 경로에서 첫 `Publish(Key.DownArrow)`→release→Enter→RequestReady→release를 수행한다. `Fixture.Create(false)`는 초기 primary default의 정상 persisted launch이며, launch 이후 previous default를 추가한다. 컨트롤러 소스 `HubMenuPresentationControllerV1.cs:33`의 정상 persisted 초기 focus는 Continue이다. Presenter 153–154행은 실제 NavigateChanged가 있어야 Down으로 focus를 옮기고, 162–165행은 실제 Submit에서 현재 focus를 활성화한다.

Router `InitializeHubUiOnly` 598–605행은 정상 UI-only 초기화에서 `SwitchMaps`를 호출한다. 1014–1045행은 UI enable 격리를 설정하여 첫 실제 입력 갱신의 callback들을 무시하고 그 갱신 끝에 해제한다. 현재 fixture는 키보드 등록/호스트 활성화/실제 launch/Router Step을 하지만 최초 선택 이전 별도 `InputSystem.Update()`는 없다. 따라서 첫 Down이 활성화 격리에서 무시되고, 이후 Enter는 정상 발행되어 **Continue request도 RequestReady를 만족하지만 NewGame take는 false**가 되는 소스 경로가 있다.

두 실패 실행에는 request.Item 또는 최초 NavigateChanged가 기록되어 있지 않으므로 실제 Item=Continue를 확정하지 않는다. 이 경로는 현재 입력 순서와 충분히 구체적인 정적 후보이며, 광범위 제품 패치나 수많은 getter 관측은 필요하지 않다.

## 제한 fixture 보정과 필요한 확인

가장 작은 후속 승인안은 같은 Play fixture의 **최초 선택 전** 정상 `Publish()` 한 번으로 released 장치 상태와 활성화 격리 갱신을 처리하고, 기존 비회복 Down→release→Enter 순서를 그대로 수행하는 것이다. 정상 장치 상태 이벤트→패키지 Update→Router 실제 Step이며 직접 action callback이나 가짜 프레임이 아니다. 이 중립 발행은 최초 epoch의 입력 준비이며 Cancel/Rearm 이후가 아니다. 새 successor의 첫 실제 Submit 이전에는 여전히 어떤 추가 빈 발행도 넣지 않는다.

정적 후보를 숨기지 않도록 최초 비회복 Down의 실제 기존 `CurrentUiFrame`에서 `NavigateChanged`와 음수 `NavigateYQ4096`를 정상 assertion으로 요구하고, Enter/Presenter/Late 직후 RequestReady에 더해 실제 원래 request의 `_item=NewGame`을 요구한다. 후자는 Q-B의 `_request: Nullable<HubMenuIntentRequestV1>`를 정확 declaring type/field type으로 읽고 값이 있는 boxed 원본 struct의 `_item: HubMenuItemV1`를 읽기만 한다. 사적 슬롯 대입·등록·복사 후 재발급은 금지한다. 필요하면 같은 두 raw 값과 Presenter `_focus: Nullable<HubMenuItemV1>`를 기존 입력 순서 실행에서 먼저 기록하는 단독 진단안을 선택할 수 있다. 게으른 action 조회나 추가 영수증 getter는 넣지 않는다.

원래 request가 NewGame인데도 take가 false이면 이 선택 후보로 해결됐다고 주장하지 않고 180행의 결속/CanBind와 실제 phase/live의 제한 raw 자료를 별도 승인 아래 검토한다. 최초 Down 실제 Navigate가 없으면 입력 격리/발행 관측을 근거로 보고한다. 원래 입력과 권한을 바꾸어 실패를 숨기지 않는다.

마지막 초기 release는 현재 take 인증에서 요구하는 불변 launch 영수증을 교체하지 않으므로 이번 후보 때문에 시점을 옮길 이유는 없다. 최초 Take→Accept 이후이며 Rearm 이전으로 옮기는 순서도 원칙상 입력 준비 경계가 될 수 있으나, 현재 AC001은 take 후 비활성화하며 Accept하지 않는 사례여서 공용 helper에서 무조건 Accept를 넣을 수 없다. 그 이동을 별도 보정으로 섞지 않는다. 새 첫 Submit을 위해 필요한 released 상태는 기존 초기 release에서 정상 확보하고, successor 이후 첫 true 프레임 폐기 검증은 그대로 유지한다.

공용 helper의 회복 경로에는 notice dismiss의 최초 Enter도 첫 격리에 소비될 수 있다는 동일 정적 후보가 있다. 다만 이번 실제 실패는 `Create(false)` 두 사례뿐이다. 회복 경로 통과를 주장하지 않고 제한 중립 갱신을 공용 최초 준비로 둘 경우 해당 기존 경로의 정확 영향 시험을 후속 검증 대상으로 명시한다. 다른 입력/권한 행이나 기대 결과를 변경하지 않는다.

## 범위와 지문

구현 승인 후보는 기존 Play Owner 시험 파일의 `SelectInitialNewGame` 초기 준비와 제한 실제 선택 검증뿐이다. 새 runtime·settings·asmdef·friend·API·자산·상태 대입·사적 수명주기 호출은 없다. 기존 Edit 240/Play 15 선택 이름과 377행/91개 행렬을 유지하며 180초 시험 제한을 올리지 않는다. 기존 R8 실패와 R7 관측을 보존한다. 실행·독립 검수·통합 수용은 별도 단계이다.

- 현재 Play 시험: `FC431157BEBF276EDA2C36ACF7682E2C3144A999AF39A18375D492B1D0D116A2`.
- Q-B 소스: `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5`.
- Presenter 소스: `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900`.
