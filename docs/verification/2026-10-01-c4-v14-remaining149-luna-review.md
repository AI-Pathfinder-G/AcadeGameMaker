# C4 v14 다섯 번째 실행 독립 중간 검수

검수 대상은 `c4-r2-edit-remaining149` 한 실행이다. 계획은 `artifacts/c4-final-validation-queue-plan-v14.json` SHA-256 `8478870AD578FBC2E1D9CCC72895B78FF2756B535F65808D99BC77D77E1DFE6C`에 결속되어 있다. 이는 부분 결과이며 전체 큐 수용 판정이 아니다.

원시 XML은 149개 시험 모두 통과했고 실패·건너뜀·미확정은 0이다. XML의 고유 전체 이름 149개는 선택 `artifacts/c3-r11-edit-remaining-selection.json`(SHA-256 `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF`)의 이름과 정확히 일치한다. 누락·초과·중복은 없다. XML SHA-256은 `1D8B60981830F832615814D668374E88DA7F845A73C308AD239A7358072A190E`이며 검증 보고서의 XML 결속 지문과 일치한다.

필수 행 비교는 C4 행 0개가 예상된 이 분할에서 `Planned/Reached/Passed/Failed = 0/0/0/0`, 누락·예상 밖·무효 ID 0, `SourceMatched=true`, `EvidenceMatched=true`를 보고한다. 현재 검증기는 `artifacts/c4-verify-required-rows.ps1`의 SHA-256 `A947C9C6D51C94D177F8A8268B4930607C1669232602B0F4C1C6E960FA16D9DC`이며, `RequiredRowIds` 분기에서 `$rows=@(...)`로 빈 배열을 유지한다. 행 비교 결과 SHA-256은 `F89F7179D6399E11A74ACD16426873D99E9BDDA0B60314A6541C7B4F6B2E2584`다.

검증 보고서는 계획, 선택, source manifest v18, input paths v18, XML, native 종료 관측, QA 반환, 행 비교에 각각 연결되어 있다. manifest 지문은 `637CB631D8F25F630139F26A2B4AEA552D64F8E64EBA7526AAB3825079BC4569`, 입력 목록 지문은 `69DE040ACA02BB02F454B45D09AF418E942278E395E366E54BFFE60AA54F0ABC`다. 입력 차이와 동결 소스 차이는 각각 0이다. 실제 native 종료 코드, QA 도구 반환 코드, 외부 종료 코드는 모두 0이다. 검증의 `Verified=true`, `WholeAccepted=false`를 확인했다.

이 결과는 AC-M5D7QC4-009/010 및 AC-M5D7QC3-007/008과 관련된 이 분할의 제한 증거로만 기록한다. 다른 실행 및 전체 큐 판정은 포함하지 않는다. P0 0건, P1 0건.
