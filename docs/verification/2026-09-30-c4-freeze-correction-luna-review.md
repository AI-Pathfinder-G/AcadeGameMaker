# C4 동결 보정 초안 독립 검토

- 검토 범위: `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-draft.md`
- 검토 대상 SHA-256: `E1362A223A55608CECE8BB88BE2FC06D6240F21094EF6F90DDEBDE1BBBA300A0`
- 대조 기준: Approved C4 r4 `docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md` SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, QA r2 및 현재 v1 원장·입력 목록.
- 판정: P0 0, P1 1. 초안 상태이며 구현·실행 승인이나 C4 수용 판정이 아니다.

초안은 v1 원장을 보존하면서 v2를 실행 권위 원장으로 분리하고, 기존 fixture는 v1의 `Files`만 읽게 한다. v1/v2의 `Files` 값과 순서가 일치하는지 실행 전 검증하고 양쪽 원장을 입력 목록에 포함한다는 구조는 R4와 양립한다. 이는 fixture 소스 변경이나 기존 102개 파일의 재동결을 요구하지 않으면서 v1의 누락된 변경 권한을 실행 승인으로 오인하지 않게 한다. 입력 목록에 v1과 v2를 모두 넣고, 실행 전후마다 같은 경로·해시를 대조하는 조건도 적절하다.

초안이 새 선행 지도와 입력 목록의 이름을 R4에 맞춘 점도 올바르다. R4가 지정한 이름은 `artifacts/c4-predecessor-selection-name-map-v1.json` 및 `artifacts/c4-frozen-input-paths-v1.json`이다. 현재 준비 목록의 `artifacts/c4-predecessor-selection-map.json`, `artifacts/c4-input-path-list-v1.json`은 권위 입력으로 재사용하지 않고 이력으로 보존한다. 새 입력 목록은 기존 884개 경로를 유지하고, v1/v2·정확한 새 지도·직접 읽는 도구와 근거를 포함하며 자기 자신·계획·실행 출력을 제외해야 한다. 이전 입력 목록이나 지도는 새 생성·검증 절차가 실제로 읽는 경우에만 직접 입력으로 포함하고, 그렇지 않으면 준비 이력으로만 남기는 해석이 최소 범위다.

남은 P1은 v2 `AllowedChanges`를 경로별로 다시 만들 때 해결되어야 한다. 현재 v1은 모든 허용 경로에 같은 요구사항 다섯 개만 붙여 `REQ-M5D7QC4-003`과 `REQ-M5D7QC4-005`를 빠뜨렸다. R4는 실제 C1/C2 연결 경로도 변경 허용 범위에 둔다. v2는 변경 가능한 경로별로 실제 근거가 있는 REQ만 기록하고, 특히 C1/C2 실행 연결 경로가 003/005 중 어느 요구사항에 의해 허용되는지 정확히 명시해야 한다. 파일 경로별 변경 권한을 포괄적인 동일 목록으로 복제하면 이 결손이 남는다. Q0 감사 파일을 제한적으로 변경 가능 목록에 둔 점은 R4의 별도 Q0 pin 변경 승인에 포함되므로 추가 결함으로 보지 않았다.

P1을 닫으려면 v2 발급 뒤 v1/v2의 바이트 SHA와 `Files` 전체 값·순서 동일성, 변경 경로별 `AllowedChanges`와 Approved 근거, 새 두 파일명의 정확성, 새 입력 목록의 기존 884 경로 보존 및 추가·제외 사유를 독립 확인해야 한다. 이후 plan 발급은 별도 승인 gate로 남는다. Unity·컴파일·큐 실행은 이 검토에서 수행하지 않았다.
