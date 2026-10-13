# C4 R3 통합 개정 계약 독립 검수

검토 대상은 Draft 계약 SHA-256 FB1BCEB6DC20CA4E5122CB3756A559EC6FEB57858B8F6A6A1E587A85A254F4D1이다. 비교 근거는 기존 C4 Review 계약 3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0, 정확 개정 근거 ACC8F20DE5D4D4E21F1385C5E204DC2D79BFB76F35CE12F7A439DC3599217738, 결과표 r2 B1B5A36757595F9DB993950819A61DB72ECBE44FFBDB369CA45D5C674FED50F6, 규범 선택 한정 수용 178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692 및 현재 R11 소스다.

현재 C3 R11 단계 수용 문서 SHA 92343D3C5AE287B1D4F6B939CFDECD1B6424E6E11B9D29CB6AD4C19F64808F89와 앞선 독립 최종 검수 두 건을 확인했다. C3의 AC-007/008 부분 증거는 C4 공동 최종 gate까지 남고, 이 검토 대상도 Draft다. 이 검수는 C4 구현·실행 권한이나 gate 수용을 부여하지 않는다.

## 대조 결과

C4 요구 001–007, 기준 001–010과 기존 C3 AC-007/008 연결, runtime 8개 경로·시험/fixture/meta 허용 범위, Q0 exact pin, 입력 동결·신규 QA 프로토콜은 상위 설계와 일치한다. C1 ProfileResetDiskTransactionV1.cs 현재 SHA BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2와 C2 ProfileResetMemoryCutoverV1.cs 현재 SHA 60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275는 기존 승인 지문이다. 계약은 두 알고리즘을 바꾸지 않고 정확 C1/C2 호출, 실제 typed row/receipt, C2의 proof 재검증·lease 재취득을 요구한다.

결과 enum의 다섯 outcome과 네 phase는 승인된 r2 결과표와 일치한다. FreshC3Required를 결과 값으로 혼합하지 않고, Busy와 ConfirmationStale를 구별하며, 실제 C1 row 전 예외에서는 정상 PreC1 결과를 발급하지 않는 점도 적절하다. C3 Commit 완료와 C4의 별도 단일 소비, typed result 구성, terminal partial-failure, prior Q-A/Q-B history 보존, 새 C4 source에 대한 별도 manifest·선택·입력 동결·QA 실행 역시 원래 계약 범위를 유지한다.

스레드 규칙은 미검증 payload와 Unity/native 접근 전에 actual CWT 및 독립 issuer-thread anchor를 검사하고, 미지원 문맥은 무소비 프로토콜 거절, 원래 문맥은 전체 무결성 검사 후 손상 시 terminal 처리로 구분한다. worker가 native Disable/Dispose를 수행하지 않으며, fault 기록이 native 정리보다 앞선다는 경계도 맞다. 계획 API의 anchor와 result 관리 조회는 현재 소스에 없는 것으로 명시되어 있고 새 issuer·public registry·Hub 역참조를 허용하지 않는다.

fresh handback은 실제 C1 Busy/NoBarrier 또는 ConfirmationStale/NoBarrier에만 허용되고, C1/C2 결과 및 history projection을 보존하며, Busy와 Stale를 하나의 결과로 접지 않는다. 신규 Q-A live slot을 기존 닫힌 상태보다 먼저 검사하는 제한 분기와 Q-B 신규 private poll/take 분기는 legacy 첫 take·Cancel successor 본문·역사를 되살리거나 덮어쓰지 않는다는 조건과 양립한다. 신규 정상 intent는 실제 reciprocal reservation, 새 epoch 및 cursor, 첫 실제 Submit 폐기 뒤에만 발급하도록 규정되어 있다.

## P1 — 새 C3 소유자 세대 전환 규범 누락

현재 NewGameConfirmationOwnerV1.CommitForExecution은 성공 시 state를 ExecutionCommitted로 설정하고, 현재 필드 검증은 그 state와 모든 projection을 강하게 묶는다. CanBindIssuance는 state가 AwaitingRequest이고 _pendingIssued, _acceptedIssued, _acceptedRequest, _display, _fresh, _decision, _retry, _confirmed, _rearm가 비어 있으며 token/epoch/history가 pristine일 때만 새 요청을 허용한다. 확인한 현재 소스에서 이를 C4 Busy/Stale 뒤 새 epoch의 AwaitingRequest로 전환하는 허용 메서드는 없다. 새 계약의 AcceptFreshExecutionHandback(result)는 서명만 제시하고, 뒤의 Q-A/Q-B API는 새 Owner pending epoch가 이미 존재한다고 전제한다.

따라서 C1 Busy/Stale의 새 cursor·새 handle을 기존 same Owner에게 전달해도, 명시된 계약만으로 Owner가 CanBindIssuance를 만족할 새 generation/token을 만들고 이전 필드를 append-only history로 옮겨 새 pristine active slot을 공개하는 단계가 결정되지 않았다. 기존 owner state를 그대로 두면 실제 TryTakeNewGameForConfirmation의 CanBindIssuance에서 거절되고, 암묵적 초기화는 과거 권한/역사 소거를 허용할 위험이 있다. 이는 제안된 새 Hub slot과 실제 C3 issuance 경로 사이의 필수 연결 누락이다.

필수 보정은 Owner의 fresh accept/commit 전환을 규범화하는 것이다. 실제 Busy/Stale 원본 결과와 lower reservation, 같은 Owner 및 새로운 token/interaction generation을 인증하고, 이전 request/decision/commit 사건을 기존 불변 history에 보존한 채 새로운 active slot을 구성해야 한다. 새 slot은 Q-B 실제 발급 및 Q-A 실제 ack와 lower CompleteFresh 성공 전까지 live pending 권한이 아니어야 한다. 실패·late loser는 원본과 신규 슬롯 모두 terminal이며 legacy Cancel/Rearm/history를 재사용해서는 안 된다. 최소 named 행은 실제 새 epoch issuance와 기존 handle 거절/신규 handle take-confirm 경로를 입증해야 한다. 구현 세부의 임의 반사 권한 제조로 이 단계를 시험해서는 안 된다.

## 판정과 미실시 범위

정적 설계 차단 P0=0, P1=1이다. 신규 Owner fresh 세대 전환 규범을 보정하고 정확 지문에 대해 재검수하기 전에는 이 통합 계약을 구현 승인 대상으로 넘길 수 없다. 나머지 경계는 기존 C3/C4 계약과 일치한다.

현재 소스의 14개 R11 동결, C1/C2 실행 알고리즘 및 Q-A/Q-B 최초 take/history는 이 검수에서 변경하거나 실행하지 않았다. 제안된 8 runtime 경로·신규 시험/fixture와 meta, 신규 Q0 pin 및 C4 전용 QA/입력 동결은 아직 구현·생성·실행되지 않았다. C3 단계 선행 수용은 유지되지만 C3 AC-007/008은 C4 실제 결과/공동 gate 전까지 미완결이다. C4 상태는 Review이며 이 새 개정도 Draft다.
