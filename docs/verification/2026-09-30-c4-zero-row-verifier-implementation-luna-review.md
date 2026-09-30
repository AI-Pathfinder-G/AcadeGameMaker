# C4 0행 검증기 복구 구현 정적 검토

대조 기준은 Approved 계약 `docs/specs/work-contracts/2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md`(SHA-256 `94E8B2F782C70DFE00E3299690B590BDDCBF181F20B68240DAADA8292252C59D`)이다. 변경된 도구 두 개의 현재 바이트, 파서 진단, 이전 지문으로의 역치환을 읽기 전용으로 확인했다.

## 대조 결과

- `artifacts/c4-verify-required-rows.ps1` 현재 SHA-256은 `68FFA7ABD658332794B70F623F740D23CD06181C54587ECE3745B8FE711E8838`이다. 기존 한 줄의 조건부 배열 식을 `RequiredRowIds` 존재 여부에 따른 두 분기의 명시적 `$rows=@(...)` 대입으로 바꿨다. 해당 줄을 이전 식으로 역치환하면 이전 SHA-256 `74F26AC5BFD34A2ABD78814B981844CB2A9CACCADED1062973FB22665BB4CD12`가 재현된다. 따라서 변경은 승인된 한 줄에 한정된다.
- 이 변경은 빈 행 목록을 실제 길이 0 배열로 유지한다. 주변의 계획·선택·원장·XML 지문 결속, 예상 행 전체의 형식·고유성·순서 확인, 선택 ID 수 확인, XML 파싱, 예상 밖/잘못된 행 거절, 비어 있지 않은 행의 정확 비교 및 `Matrix91` 분기는 그대로다. 다만 이 정적 검수는 새 빈 행 양성·예상 밖/형식 오류 음성 회귀를 실행하지 않았다.
- `artifacts/c4-final-validation-queue.ps1` 현재 SHA-256은 `13484AAA0C5CAF60FFA53BB674D02B3F79B0BEF3055BDC91950C63F9D44455E7`이다. 11행의 허용 결과 경로만 v11에서 `artifacts/c4-final-validation-queue-result-v12.json`으로 바뀌었다. 경로를 되돌리면 이전 SHA-256 `60BA312E15CF8F0823AA47F281F789BC1D8DB64C47939EED06AACBEBC6E64446`가 재현된다.
- 두 PowerShell 파일의 구문 분석 오류는 0건이다. 이전 실패 결과 `artifacts/c4-final-validation-queue-result-v11.json`은 보존되어 있고, 새 결과 v12는 아직 없다.

## 판정

P0 0, P1 0. 변경 바이트는 승인 범위와 일치한다. 이는 도구의 정적 변경 검수만 뜻한다. 0행/비영 행 회귀, 새 동결 자료, 계획 v14 및 실제 큐 실행의 검증은 남아 있으며 이 기록은 AC-M5D7QC4-009/010의 실행 수용이나 C4 전체 수용을 주장하지 않는다.
