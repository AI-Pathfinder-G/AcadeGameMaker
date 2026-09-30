# C4 0행 예상 밖 ID 보고 구현 정적 검토

검토 기준은 Approved 계약 `docs/specs/work-contracts/2026-09-30-c4-zero-row-unexpected-id-successor-amendment.md`(SHA-256 `7EFA4CCCE58378125531FB47E13840C527411C75793EEFE59B511D521CCF11F9`)이다. 선행 진단 `docs/verification/2026-09-30-c4-zero-row-unexpected-record-luna-diagnostic.md`(SHA-256 `D345BAAC3832A9FEF3B58A15049433F7FACDDDF1E0222BC489CFE8AC6CC0A7AB`) 및 0행 배열 복구 계약과 대조했다.

`artifacts/c4-verify-required-rows.ps1`의 현재 SHA-256은 `A947C9C6D51C94D177F8A8268B4930607C1669232602B0F4C1C6E960FA16D9DC`다. 347행은 `$rows.Id` 열거를 명시적 `@($rows|ForEach-Object{$_.Id})` 배열로 대체하고, 기존 case-sensitive `-cnotin` 및 `Select-Object -Unique` 흐름을 보존한다. 그 표현만 이전 `$rows.Id`로 역치환하면 선행 SHA-256 `68FFA7ABD658332794B70F623F740D23CD06181C54587ECE3745B8FE711E8838`가 재현된다. 따라서 변경은 승인된 한 표현에 한정되고 빈 행 집합에서도 ID 비교 대상은 길이 0 배열이다. `:343`의 두 분기, malformed의 `InvalidIds`, 비영 행 strict 비교와 Matrix91 경로에는 변경이 없다.

큐 도구 `artifacts/c4-final-validation-queue.ps1`은 SHA-256 `13484AAA0C5CAF60FFA53BB674D02B3F79B0BEF3055BDC91950C63F9D44455E7`로 유지된다. 검증기 PowerShell 구문 분석 오류는 0건이며 결과 v12는 아직 없다. 회귀 재실행은 하지 않았다.

## 판정

P0 0, P1 0. 승인된 예상 밖 ID 보고 경계의 정적 변경은 정확하다. 실제 r2 음성 보고서 발급 및 v18·계획 v14의 동결 검수는 남아 있으며 이 문서는 AC 실행 수용을 뜻하지 않는다.
