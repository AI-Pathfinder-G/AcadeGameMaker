# C4 0행 예상 밖 ID 후속 설계 검토

검토한 제안 `docs/proposals/2026-09-30-c4-zero-row-unexpected-id-successor-design.md`의 SHA-256은 `A210F85E64D6A00C60A9E1922D1033672F2BCE4B4D77F9F5A082092834971959`다. 근거 진단 `docs/verification/2026-09-30-c4-zero-row-unexpected-record-luna-diagnostic.md`(SHA-256 `D345BAAC3832A9FEF3B58A15049433F7FACDDDF1E0222BC489CFE8AC6CC0A7AB`), r1 증거 `artifacts/c4-zero-row-regression-r1-evidence.json`(SHA-256 `8D7A0B55F037E4694980861D76305F7D78F403993B4EBD7149566863A7910E17`), Approved 1차 계약 `docs/specs/work-contracts/2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md`(SHA-256 `94E8B2F782C70DFE00E3299690B590BDDCBF181F20B68240DAADA8292252C59D`)와 대조했다.

## 검토 결과

원인과 제안된 변경 범위가 일치한다. 빈 예상 행에서 `$rows.Id`를 직접 열거하는 347행이 StrictMode 예외를 내어 정상 형식의 예상 밖 ID를 보고하기 전에 중단한다. 제안은 그 ID 열거만 명시적 배열로 바꾸고 기존 대소문자 구분 `-cnotin`, 관측 순서에 따른 `Select-Object -Unique`, `UnexpectedIds` 보고를 유지한다. 빈 배열이어도 예상 밖 ID 비교가 가능하며, 유효한 예상 밖 행은 보고서를 만들고 `EvidenceMatched=false` 및 종료값 1로 거절해야 한다. 잘못된 형식 행의 `InvalidIds` 경로도 그대로다.

비영 행의 선택 ID·terminal 상태·값·확실성·지문 검증, 전체 188행 원장 검사, `Matrix91`의 기존 비교기 분기, 큐·캡처·실행 검증기는 변경하지 않는다고 명시했다. r2 회귀는 0행 양성, malformed 및 정상 형식 예상 밖 음성, 비영 Play 35행 양성·누락·불일치 음성, Matrix91 377행 양성을 새 출력 경로에서 재검증하도록 구성되어 있다. 이는 첫 회귀에서 실제로 확인된 성공·실패 조건과 맞는다.

v18 원장·입력 및 계획 v14 발급은 검증기 지문과 r2 결과를 독립 확인한 뒤 진행하고, 이전 결과를 덮지 않도록 이미 출력이 있는 실행의 새 출력 경로와 뒤 실행의 사전 부재 확인을 선행 계약에 따른 조건으로 둔다. 제안은 구현 승인이나 큐 전체 수용으로 확대하지 않는다.

## 판정

P0 0, P1 0. 설계는 남은 StrictMode 진단 결손만 수정하며 행 비교의 엄격성이나 기존 회귀 의미를 낮추지 않는다. 구현, r2 회귀 실행, v18/계획 v14 바이트 검수는 이 문서의 범위 밖이다.
