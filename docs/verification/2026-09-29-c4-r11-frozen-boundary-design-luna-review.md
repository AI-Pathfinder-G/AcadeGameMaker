# C4 R11 동결 경계 설계 독립 검토

읽기 전용 설계 검토다. C4는 계속 Review이며, C3 선행 전체 수용 전 구현하거나 실행할 수 없다.

## 판정

제안의 경계 및 수용 기준 추적에 P0 0건, P1 0건이다. 최신 제안 `docs/proposals/2026-09-29-c4-r11-frozen-boundary-design-refresh.md`의 SHA-256은 `BAB33568BD479AF46C265F3FCCEC232BBEF339C472B6F9CB1DF9A90E17347F0E`다. C1/C2 Verified 범위와 현 소스, 승인된 C3 단계적 수용, C4 Review 기준을 서로 섞지 않는다. 제안은 프로토콜과 API 이름을 제시하지만 실행 반례가 아니고 구현 승인도 아니다.

## 현재 상태와 실제 경계

- C4 계약은 `Review` 상태이며 C1 Verified, C2 Verified 및 C3의 독립 선행 단계 수용과 동결을 전제로 한다. 계약 SHA-256 `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`; ADR-0036 SHA-256 `627D7CE2F948F5A7BBECE2907C772321CBDB450212780D0D281227776D9BB8A3`는 선행 단계 수용과 C3 잔여 AC-007/008·C4 전체 최종 검증을 분리한다.
- C1 계약의 디스크 전용 Verified 지문은 `D03C77478D2EC4214A225E4EA46B993E59C753C6EB883BB0C913491D3068E455`다. 현 `ProfileResetDiskTransactionV1.cs` 지문은 `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`; `Begin`은 실제 identity/root를 검증하고 lease 안에서 identity를 소비한 뒤 파일을 재관찰한다. 변경 검출 `ConfirmationStale`와 최초 lease의 `Busy`는 NoBarrier 행으로 구분된다. 그 외 불확실·장벽 이후 실패를 Busy로 오인하지 말고 terminal 처리해야 한다. C3L 통합 승인 SHA `1000E2752EAB63B496C5DA8491D698A8FE40D492497E039D3F96A725D09B582F`가 현재 C1 후속 소스 지문 연결의 근거로 인용되어 있으며, 과거 Luna 검수의 다른 지문을 현재 바이트로 바꾸어 쓰지 않는다.
- C2 계약 SHA `F76E9AF2B7A9A2E936A8E2253A3E1FDA7E94E164DC0FBCBC08BBF2BAA4D1F5DF`는 Verified이나 합성 범위다. 현 `ProfileResetMemoryCutoverV1.cs` SHA `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`와 수용된 R36 지문이 일치한다. 기존 `FinalizeReset(root, proof, owner, router)` 진입은 실제 exact pair/root 확인 후 lease 획득, proof 재인증, staging과 terminal 경계, 실제 receipt 발급 순서를 유지한다. C1 `DiskPrepared` 뒤 C2 lease 획득 실패는 이미 디스크 장벽 이후이므로 C2 `Busy`라도 C4 전체 실행에서는 terminal이어야 한다. proposal은 C2 proof/receipt 알고리즘 변경을 요구하지 않고, 필요한 경우 exact guard-bound entry만 허용하는 좁은 분기를 조건으로 둔다.
- C3 계약은 Approved 합성 범위(SHA `6B60F520E71B7BC2D1B7EBC9F53C283CA942676FEA02202F796514258F9D5FFB`)다. 현재 lower C3 SHA `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1`에서 C3 `CommitForExecution`은 private 발급 이력을 예약 후 `Completed`로 닫고 같은 불투명 요청을 반환한다. 이는 C3의 완료 이벤트이지 C4가 실제 실행해 발급한 C1/C2 결과가 아니다. 제안은 이 구분과 lower의 현재 private CWT/lifecycle 기반 권한을 보존한다.
- C3 부분수용은 Edit 240 및 Play 15 선택만 각각 제한 수용됐다. Edit 승인 SHA `796D76CF7F9282970B57D91051AB54DFB0284FE470AF321EFA45B8F499CD7929`; Play 승인 SHA `1CF32ED23DC6BA1AC38DDF1F04C4F4309F5DDFAC574DEB1BFDA59A081C31B4AD`다. 제안의 수정된 상태 문단은 이를 각각 부분수용으로 정확히 적고, 필수 562 진행 중 및 51/610 미시작, C3 선행 전체 수용 미완료, AC-007/008 미완료, C4 Review를 유지한다. 부분수용은 C4 준비를 위한 전체 선행 문턱을 충족하지 않는다.

## 제안 설계와 AC 추적

lower 입력은 C3가 실제 발급한 불투명 요청, adapter/router와 C1 identity를 lower의 private witness에서 확인해야 한다. proposed executor는 Input.Unity에 두어야 하며 Hub 타입을 하위 어셈블리로 역참조하지 않는다. 실제 asmdef 방향은 Hub.Presentation.Unity가 Input.Unity를 참조하고 Input.Unity에는 Hub.Presentation 참조가 없음을 보인다. 제안의 partial lower 선언은 기존 Input.Unity 경계 안에 머물고 신규 메서드는 내부 API 후보로 한정된다. public ABI, asmdef, friend 관계 또는 C1/Profile 알고리즘·결과·proof 변경을 요구하지 않는다.

제안은 C3 Completed 이력을 유지하면서 별도 소비 기록과 실행 가드를 두고, 정확한 C1 Busy/ConfirmationStale·NoBarrier에서만 fresh handback을 허용한다. 소비된 요청/identity의 재사용을 금지하며, C1 DiskPrepared 이후의 C2 비완료·예외는 terminal 처리한다. Busy/Stale handback을 기존 Cancel/Rearm으로 가장하지 않고, immutable history를 보존하는 새 Hub 중립 capability/reservation 및 Q-A/Q-B 상호 acknowledgment 절차로 분리한다. 이는 실행증거가 아닌 설계 제안이며 새로운 정상 권한 발급은 Astra 승인 전 허용되지 않는다.

AC-M5D7QC4-001..010을 개별 확인했다. 001의 실제 same-request 소비·foreign/stale/double-consume 거절, 002/006의 양쪽 가드와 C1-C2 lease 틈, 003의 단일 실제 C1 Begin과 원 identity, 004의 C1 NoBarrier만 허용하는 fresh handback, 005의 동일 root/proof/pair로 한 번의 C2 호출, 007의 정확 receipt와 모든 post-barrier 실패 폐쇄, 008의 재진입·경쟁·불변 이력, 009의 권한·어셈블리·API 경계, 010의 최종 선택·필수 회귀가 설계 행과 대응한다. 007/008은 실행증거로 폐쇄되지 않았고 C3 AC-007/008도 여전히 열려 있다.

## 남은 승인 경계와 한계

기존 Q-B 감사 테스트는 현재 Hub 소스에서 `Profile`, durable 호출, gameplay 및 delegate 계열 토큰을 금지하고 엄격한 허용 import 규칙을 검사한다. C4가 Q-A/Q-B source를 amendment로 바꾸는 경우 기존 감사 테스트의 편집 허용과 새 strict audit successor도 별도로 승인해야 한다. 제안은 이 조건을 명시하며, 허용되지 않으면 fresh handback 구현 가능성을 단정하지 않는다. Q0 현재 소스 감사 핀도 Adapter/Router가 실제 변경되는 경우에만 좁은 amendment를 요구한다.

정적 설계에서 실행 반례는 관찰하지 않았다. Fresh-C3 handback, 이중 가드의 원자적 결속, 예외별 terminal 폐쇄, actual Busy/Stale와 barrier 이후 구분은 향후 Approved 구현과 실제 시험으로 확인해야 한다. 이 검토는 Git·네트워크·Unity·컴파일을 실행하지 않았고, 현재 필수 회귀 562의 진행 결과를 추정하지 않는다. C4 Review 유지 및 C3 선행 전체 미수용은 변함없다.
