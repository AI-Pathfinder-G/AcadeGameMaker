# C4 0행 선택의 예상 밖 행 보고 후속안

- 상태: 제안. 1단계 Approved 계약과 r1 회귀 출력은 불변이며, 아스트라의 별도 한정 승인 전 구현·Unity 실행·수용 권한이 없다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행 계약: `docs/specs/work-contracts/2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md` SHA-256 `94E8B2F782C70DFE00E3299690B590BDDCBF181F20B68240DAADA8292252C59D`. 현재 검증기 `artifacts/c4-verify-required-rows.ps1` SHA-256 `68FFA7ABD658332794B70F623F740D23CD06181C54587ECE3745B8FE711E8838`은 빈 `RequiredRowIds`를 배열로 보존한다.

## 남은 정확 실패

Terra의 r1 제한 회귀에서 Remaining149의 0행 양성은 종료 0, `Rows=[]`, `EvidenceMatched=true`였다. 형식 불량 C4 행은 종료 1·`InvalidIds` 1건의 보고서를 남겼다. Play 35/35 양성과 필수 행 누락·값 불일치 음성, Matrix91의 기존 377/377도 통과했다. 그러나 **형식이 올바른 예상 밖 C4 행**은 `artifacts/c4-verify-required-rows.ps1:347`의 `$rows.Id`가 빈 배열에 대한 `StrictMode` 속성 접근에서 예외를 내어 종료 1이면서도 `UnexpectedIds` 보고서를 남기지 못했다. 이는 예상 밖 행을 수용한 증거가 아니라, 예정된 음성 보고 경로의 결손이다. r1 XML·JSON·진단 원본은 보존한다.

## 가장 작은 후속 계약 후보

수정 파일은 `artifacts/c4-verify-required-rows.ps1` 하나다. `:347`의 `$rows.Id` 속성 열거만 빈 집합에서도 안전한 명시적 ID 배열, 예를 들어 `$expectedIds=@($rows|ForEach-Object {$_.Id})`, 로 치환하고 기존 `-cnotin`의 정확 대소문자 비교 및 예상 밖 ID 유일화·원래 순서를 그대로 유지한다. 0행이면 모든 유효 C4 기록의 ID가 `UnexpectedIds`에 들어가고 `Rows=[]`, `EvidenceMatched=false`, 실제 종료 1인 보고서를 `CreateNew`로 남겨야 한다. 잘못된 형식은 계속 `InvalidIds`와 종료 1이다. 비영 행의 필수 ID·상태·값·확실성·source/manifest/XML 결속, 전체 188행 원장 검사, `Matrix91` 특례, `C4-LegacyRun`, 큐·캡처·실행 검증기의 판정은 변경하지 않는다. 예상 밖 행을 무시하거나 0행을 자동 성공으로 처리하지 않는다.

후속 r2 회귀는 새 경로에만 기록한다. 0행 원본 양성, 형식 불량 음성, **정상 형식 예상 밖 행의 `UnexpectedIds` 정확 1건·`InvalidIds=[]`·`Rows=[]`·`EvidenceMatched=false`·종료 1**, Play 35행 양성 및 누락·불일치 음성, Matrix91 377행 양성을 다시 확인한다. 특히 비영 행에서 기대 ID의 대소문자 차이·예상 밖 ID가 원래처럼 거절되는지도 검사한다. 새 검증기 지문과 각 r2 결과를 루나가 독립 대조한 뒤에만 v18 소스 원장·입력 v18과 계획 v14를 발급한다. 결과 경로 v12를 허용하는 현재 큐 도구 한 줄은 그대로 유지한다. 제품·fixture·선택·기대 행과 이전 r1 회귀·실행 결과는 수정하지 않는다.
