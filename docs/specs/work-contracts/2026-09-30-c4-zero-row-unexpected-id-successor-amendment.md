# C4 0행 예상 밖 ID 기록 후속 한정 계약

- 상태: **Approved — 예상 밖 ID 보고 경계와 후속 증거 발급만 승인; Unity 전체 수용은 새 실제 결과 전까지 보류**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`. 직전 승인 계약 `2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md`를 이 한정 경계에서만 보충한다.
- 설계: `docs/proposals/2026-09-30-c4-zero-row-unexpected-id-successor-design.md`, SHA-256 `A210F85E64D6A00C60A9E1922D1033672F2BCE4B4D77F9F5A082092834971959`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-zero-row-unexpected-id-successor-luna-review.md`, SHA-256 `6E6575454B4F33CBB49BC15191AC5EFA4A39BBD844816BDA0FB1A1CC97405F3A`, P0/P1=0/0.
- 독립 진단: `docs/verification/2026-09-30-c4-zero-row-unexpected-record-luna-diagnostic.md`, SHA-256 `D345BAAC3832A9FEF3B58A15049433F7FACDDDF1E0222BC489CFE8AC6CC0A7AB`, P1 1건.
- 회귀 원본: `artifacts/c4-zero-row-regression-r1-evidence.json`, SHA-256 `8D7A0B55F037E4694980861D76305F7D78F403993B4EBD7149566863A7910E17`. 일곱 사례 중 0행 정상, 형식 오류 음성, 비영 행 정상·누락·불일치 음성, Matrix91 377행은 기대대로 동작했다. 정상 형식의 예상 밖 C4 행은 `c4-verify-required-rows.ps1:347`의 빈 `$rows.Id` 속성 접근 예외로 종료 1만 남기고 `UnexpectedIds` 보고서를 만들지 못했다. r1 입력·출력과 이전 큐 결과는 역사로 보존한다.

## 승인 범위

1. `artifacts/c4-verify-required-rows.ps1:347`의 `$rows.Id` 접근만 빈 선택에서도 길이 0인 명시적 ID 배열 열거로 바꾼다. 기존 대소문자 구별 `-cnotin`, 중복 제거 및 예상 밖 ID 수집 순서는 보존한다. 직전 승인에 따른 `:343`의 분기별 `$rows=@(...)`, 전체 원장·계획·소스 결속, 비영 행의 terminal/facts/certainty 검사, malformed 기록의 `InvalidIds`, `Matrix91` 별도 C3 검증은 수정하지 않는다. 제품·시험 fixture·선택·기대 행·다른 도구는 변경하지 않는다.
2. 새 회귀 r2는 새 출력 경로에서 원본 불변 XML·계획·행 원장을 사용한다. 0행 정상은 `Rows=[]`, 모든 행 카운트 및 누락·예상 밖·형식 오류 0, `SourceMatched=true`, `EvidenceMatched=true`, 종료 0이어야 한다. 정상 형식의 예상 밖 C4 기록은 `UnexpectedIds`에 그 ID를 기록하고 `EvidenceMatched=false`, 종료 1이어야 한다. 형식 오류는 `InvalidIds`에 기록하고 종료 1이어야 한다. 비영 Play의 35행 정상·누락·중첩 값 불일치 음성, Matrix91 C3 377행 정상도 유지한다. 실제 명령·입출력 SHA·종료·판정은 새 증거에 기록하고 루나가 독립 검수한다.
3. 직전 계약에서 승인한 결과 경로 v12 도구 변경은 그대로 두고 더 수정하지 않는다. 회귀가 닫힌 뒤 최종 행 검증기와 큐 도구 실제 SHA, 본 계약·설계·검수·회귀 증거를 새 v18 소스 원장·입력 목록에 결속한다. 이전 v17 원장·입력의 경로·사유·불변 지문을 보존하고 새 파일만 `CreateNew` 발급한다. 계획 v14는 기존 첫 다섯 실행의 새 stem·출력과 뒤 네 실행의 부재 확인을 포함해 직전 계약의 순서·사례 수·행 배분·잠금·중단 규칙을 유지한다.

`AC-M5D7QC4-009/010`의 최종 판정은 루나의 r2 회귀·v18 동결·v14 계획 독립 검수 뒤, 실제 아홉 묶음의 정확 선택·원시/QA/바깥 종료·전체 필수 행·입력 전후 동일성 결과로만 한다. 새 실패는 원본을 보존하고 별도 분류한다.
