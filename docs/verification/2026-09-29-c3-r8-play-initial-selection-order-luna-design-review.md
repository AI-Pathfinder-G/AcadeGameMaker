# C3 R8 Play 최초 선택 순서 설계 독립 검토

검토 대상 제안 SHA-256 `DE355A7010232DE4DD1CD5975E7EA9C830A26C591A2199C062D1C1636209F9E1`을 실제 XML과 구현 경로에 대조했다. **P0 0, P1 0.** 제안은 take 실패를 제품 결함으로 단정하지 않고, 최초 UI 선택의 실제 항목을 확인하면서 격리 해제용 중립 입력 갱신을 successor 경계와 분리한다. 승인 전 설계상 차단 결함은 없다.

정적 호출 흐름은 가설과 맞는다. Router의 UI map 활성화는 `_captureSuppressed`와 quarantine을 설정하고, 첫 실제 `InputSystem` after-update에서 barrier를 해제한다. Play fixture에는 초기 Down 이전에 별도 입력 갱신이 없다. Presenter의 초기 focus는 persisted-valid이면 Continue, 아니면 NewGame이며 실제 NavigateChanged가 있어야 Down으로 focus를 바꾼다. 이어진 Enter는 현재 focus 항목을 요청한다. Q-B의 `TryTakeNewGameForConfirmation`은 실제 live request의 `Item == NewGame`을 take 전에 검사하므로, 첫 Down이 격리 중 무시된 경우 `RequestReady`와 실제 Submit은 참이면서 NewGame take만 거절될 수 있다. 다만 XML에는 최초 Navigate 프레임이나 실제 request Item이 기록되지 않아 이는 정적 후보일 뿐이다.

제안된 `Publish()` 중립 입력은 기존 UI 입력 API(정상 키 상태 이벤트→InputSystem 갱신→Router Step)를 사용하고 최초 선택 전에만 수행한다. 이 한 번의 프레임은 새 intent나 Q-B 권한을 만들지 않는다. 기존 Down→release→Enter 선택 순서는 보존되고, 실제 첫 Enter에서 `NavigateChanged` 및 음수 Y를 확인하며, 이후 실제 Q-B request의 item을 읽어 NewGame인지 검증한다. take 실패 전에 판단할 자료를 추가하되 Take/Accept 정상 API와 불투명 핸들 경로는 그대로다. 이 초기 barrier는 Rearm 이후 successor의 첫 true Submit 앞에 빈 발행을 추가하는 것과 다른 위치이며, 후자의 AC006 핵심은 보존된다.

Q-B `_request`의 실제 타입은 `HubMenuIntentRequestV1?`; struct에는 `_item`과 `_itemProof`가 있고 `Item` 속성은 Validate를 수행한다. 구현은 먼저 nullable field 자체의 정확 선언·`Nullable<HubMenuIntentRequestV1>` 타입을 확인해야 한다. `FieldInfo.GetValue` 결과가 null인지 확인한 뒤 비어 있지 않은 nullable의 boxed 값이 정확 `HubMenuIntentRequestV1`인지 검증하고, 그 struct 선언형에서 비정적 `HubMenuItemV1 _item` 필드를 정확 결속해 읽어야 한다. 이름 단독 field fallback이나 validating `Item` getter 대신 이 제한 raw read를 쓴다. 이는 새 권한을 제조하지 않는다. 원본 항목이 NewGame으로 관측돼도 take 불가 원인은 아직 결속·phase·다른 gate일 수 있으므로 원인 확정으로 확대하면 안 된다. NavigateChanged/Y raw 값은 실제 Router가 발행한 현재 UI frame의 기존 공개 조회를 이용한 정상 assertion으로 검증한다. 이는 진단용 read-only helper 조회가 아니며 기존 실제 Submit assertion과 같은 검증 경로다. 추가 action getter/영수증 getter나 helper 전반의 일반화는 불필요하다.

공용 `SelectInitialNewGame`은 현재 11개 시험 본문에서 사용된다. 중립 발행을 그 helper에 넣으면 회복 notice 경로를 포함해 이 초기화 흐름을 공유하는 시험도 달라질 수 있다. 제안이 회복 경로를 별도 한계로 적은 점은 적절하다. 구현 수용 때는 두 실패 probe만으로 끝내지 말고 분할된 Play remaining 13개 이름도 정확히 실행해 영향이 없음을 확인해야 한다. 기존 240 Edit/15 Play 명칭, 377행과 91개 matrix, 180초 제한, 제품 소스·API·settings·asmdef 범위는 그대로 유지한다.

현재 시험은 R8 첫 take 단계에서 실패했으며 제안은 실행되지 않았다. 첫 정상 NewGame take, disabled-owner 후속, 재무장 successor 첫 Submit 폐기, 다음 정상 선택은 모두 실제 실행으로 확인해야 한다. 이번 검토는 설계 정적 대조이며 source 변경·Unity/컴파일 실행은 없다.

