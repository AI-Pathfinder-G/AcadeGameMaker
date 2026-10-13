# C4 0행 예상 밖 기록 회귀 진단

현재 검토한 검증기는 `artifacts/c4-verify-required-rows.ps1` SHA-256 `68FFA7ABD658332794B70F623F740D23CD06181C54587ECE3745B8FE711E8838`이다. 실제 회귀 자료를 읽어 확인했으며 도구·시험 파일은 수정하거나 실행하지 않았다.

## 회귀 자료

- 0행 양성 `c4-zero-row-regression-r1-zero-positive.json`(SHA-256 `91211F693811ED043C17C4D52278C584AB2D160CE1A9CAE369A7B41E3325A23B`)은 행 0개, 예상 밖/잘못된 행 0개, `SourceMatched=true`, `EvidenceMatched=true`를 기록한다.
- 잘못된 형식 행 음성 `c4-zero-row-regression-r1-malformed-result.json`(SHA-256 `4C4CABD94E64ED37C6CD0138C0073D131A1AA3123691461772A680D8B43BCA69`)은 `InvalidIds` 1개, `EvidenceMatched=false`를 보고한다.
- 정상 형식의 예상 밖 C4 행을 넣은 `c4-zero-row-regression-r1-unexpected.xml`(SHA-256 `2D2637CEA8E7A7B849EB21FD0BC2428D4B2D05E7E2AC1CC1493A93CCC0FDE656`)에는 `play_AC002_GuardedHistoryReadDoesNotPublishAndFreshCursorUsesActualPair` 기록이 있다. 대응 결과 JSON은 발급되지 않았다.
- 비영 Play 35행 양성은 35/35 일치했다. 누락 행 및 값 불일치 음성은 각각 1건을 잡아 `EvidenceMatched=false`로 보고했다. Matrix91 특례는 377행 일치, `EvidenceMatched=true`다.

## 결함과 최소 보정

0행 양성과 잘못된 형식 행 처리는 기대대로 작동한다. 그러나 검증기 347행의 `$rows.Id`는 0개 선택에서 빈 컬렉션 속성으로 평가되며, `Set-StrictMode -Version Latest`에서 예상 밖 정상 기록을 분류하는 대신 예외가 발생한다. 따라서 현재 예상 밖 행 음성은 종료값 1만 보이고, 계약이 요구하는 `UnexpectedIds` 및 `EvidenceMatched=false` 보고서를 만들지 못한다.

P1은 예상 밖 행을 조용히 받아들이는 권한 우회가 아니라, 음성 판정의 구조화된 결과 발급이 누락되는 회귀 검증기 결함이다. 최소 보정은 선택된 행 ID를 빈 경우에도 안전한 컬렉션으로 먼저 물질화하거나 정확 비교용 집합을 만들고, 관측 ID마다 그 집합에 없는 항목을 `UnexpectedIds`에 추가하는 것이다. 빈 예상 집합에서 정상 형식 C4 기록은 반드시 예상 밖으로 보고하고, `EvidenceMatched=false`로 기록한 뒤 종료값 1을 반환해야 한다. 잘못된 형식 행은 기존처럼 `InvalidIds`로 보고하고, 비영 행·Matrix91 검사는 그대로 유지해야 한다.

## 판정

P0 0, P1 1. 이 결함은 AC-M5D7QC4-009/010의 예상 밖 행 음성 회귀가 판독 가능한 결과를 남기는 것을 막는다. 현재 구현은 오류 종료로 fail-closed하지만 승인된 structured negative evidence를 충족하지 않는다. 실제 제품 동작 결함이나 전체 C4 수용 여부를 뜻하지 않는다.
