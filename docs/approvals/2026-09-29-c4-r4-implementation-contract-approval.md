# C4 r4 구현 계약 승인

- 판정: **Approved — 제한 구현 권한**. 실제 실행·통합 수용은 미완료다.
- 승인자: 아스트라, 실제 `gpt-6-astra`, 2026-09-29.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`, 공동 `AC-M5D7QC3-007/008`.

사용자의 연속 후속 구현 지시에 따라 두 소유 규격을 Approved로 전환한다. 소유 규격은 [C4 r4 정확 개정 계약](../specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md)과 [C4 QA 증거 프로토콜 r2](../specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md)이다. 원본 C4 Review SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`는 역사로 보존하고 이 두 승인 규격이 개정 구현 범위의 현재 규범을 소유한다. 원본의 요구·수용 기준과 제한은 새 소유 본문에서 보존했으며 제안 문서에 규범 권한을 위임하지 않는다.

C3 선행 수용 SHA `92343D3C5AE287B1D4F6B939CFDECD1B6424E6E11B9D29CB6AD4C19F64808F89`를 확인했다. C3 전체와 AC007/008, C4의 실제 결과는 아직 수용하지 않는다.

독립 검수 원문 지문은 r4 `EF54AE99EEF6E482C267C949A0C1B53709FF19611E9791300E07764EBD9256C0`, QA r2 `F7B07E26C45C9B0DCD4CAE6915BAF4D0D8DD5CBE30E0D1D5D3878C060A2674D0`다. 상태 전환 전에 각각 reviewed-draft 문서로 정확 바이트를 보존했다. 루나 실제 `gpt-6-luna`의 [r4 검수](../verification/2026-09-29-c4-r4-consolidated-contract-luna-review.md) SHA `1D4083A48C044ED868BA672B1645D8F1FCCB63AB7F0488D69522574B1822BB2F`, [QA r2 검수](../verification/2026-09-29-c4-qa-evidence-protocol-r2-luna-review.md) SHA `BF145A1D4609943FC8EC91C795B2DA3A6734145B1C66151133F86AADD2002E1B`는 각 정적 P0/P1=0/0이다. 아스트라도 상태 전환·이력·공개 순서와 정확 중첩 증거 비교 규범을 대조했다. 이전 P1과 원본·실패 검수는 이력으로 보존한다.

테라 구현 역할의 `gpt-6-sol`은 허용된 8개 실행 경로, 신규 시험·fixture 5개와 대응 meta, Q0 감사의 정확 current pin 보정만 구현한다. 별도 테라 `gpt-6-sol`은 명시된 새 검증 도구 7개만 구현한다. 원래 C1 알고리즘, C2의 최소 eligibility 분기 외 알고리즘, public ABI·asmdef·friend·기존 시험 본문·기존 QA 도구·자산·장면·설정은 변경하지 않는다. 파일별 소유자를 나누고 동일 최종 행 스키마를 사용한다. 루나는 구현자와 별도로 독립 검증하며 아스트라가 통합한다.

구현 뒤 실제 source/meta/tool/선택/행/입력/계획을 새 지문으로 동결하고 source·프로토콜 독립 검수에서 P0/P1=0을 요구한다. 실제 Unity 실행은 이 조건 이후 별도 배분한다. 이전 R11 14개·입력884·시험 통과는 새 C4 검증을 대신하지 않는다. 실제 집중 및 불변 선행 선택 전부의 결과·내부 행·부모 NUnit·세 독립 종료값·동일 입력을 검증한 뒤에만 공동 최종 수용을 판단한다.

이번 기술 승인은 실제 화면·장면·제품 범위 확대, 게임 코드 원격 게시 또는 초기 저장소의 미승인 자산·설정 게시 승인이 아니다. 변경 요청 #3와 기존 위키는 이미 게시된 과거 범위이며 새 계약·구현을 게시했다고 주장하지 않는다. 기술 충돌은 아스트라가 정확 보정·독립 검수로 처리하고 실제 제품 결정이 필요한 경우에만 사용자에게 요청한다.
