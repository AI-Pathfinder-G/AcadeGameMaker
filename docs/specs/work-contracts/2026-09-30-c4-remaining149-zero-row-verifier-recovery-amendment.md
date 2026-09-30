# C4 선행 회귀 0행 검증기 한정 복구 계약

- 상태: **Approved — 아래 검증 도구·증거 결속·재실행만 승인; C4 전체 수용은 새 실제 결과 검수 전까지 보류**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`. 기존 C3 선택과 C4 188행의 의미를 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-remaining149-zero-row-verifier-recovery-design.md`, SHA-256 `A0E10085565633488FCE2AE72CEDF68B784FFDEA13BC6CFB0EF8F13F427929D9`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-remaining149-zero-row-recovery-luna-design-review.md`, SHA-256 `114CDAAD4F04C7A388C3F1E8AE69A2DF62902C1CD55B5B4B73136FEA86B17C4C`, P0/P1=0/0.
- 독립 실패 진단: `docs/verification/2026-09-30-c4-r9-edit-remaining149-row-verifier-luna-diagnostic.md`, SHA-256 `C8318C2EE61D8A611E495ED77207F44DF674226D2EC72BB21BCA81BE2443A7F5`, P1 1건.
- 원본 큐 결과: `artifacts/c4-final-validation-queue-result-v11.json`, SHA-256 `D19C57E5B76C5BCBD95864E2F02695355251E2F9E71A6EB66F52F3F3A4DC9152`. 앞의 네 실행이 통과하고 `c4-edit-remaining149`에서 fail-stop했다. 해당 XML은 149/149 통과, native/QA/바깥 종료 0, 입력 1060개 전후 동일이다. 행 비교기는 계획의 0개 필수 행을 `$null`로 풀어 `.Count` 접근에서 중단했다. 원본 결과·XML·캡처·앞선 실행 출력과 잘못된 C3 진단 결과는 수정하지 않는다. C3 0/377 진단을 C4 수용 근거로 사용하지 않는다.

## 승인 범위

1. `artifacts/c4-verify-required-rows.ps1`의 0행 선택 할당 한 곳만 명시적 두 분기의 `$rows=@(...)` 대입으로 바꿔 빈 `RequiredRowIds`를 실제 길이 0 배열로 유지한다. 계획 없이 호출하는 기존 전체 행 선택은 유지한다. 예상 원장 188행 전체 형식·ID·순서 검증, 계획/소스/XML 결속, 필수 ID 개수, 관측 XML 파싱, 예상 밖·형식 오류 탐지, 비영 행의 terminal·facts·certainty 엄격 비교와 `Matrix91`의 별도 C3 377행 검증은 변경하지 않는다.
2. 0행 계획의 양성 결과는 `Rows=[]`, 모든 도달·통과·실패·미도달 카운트 0, 누락·예상 밖·형식 오류 목록 비어 있음, `SourceMatched=true`, `EvidenceMatched=true`를 모두 요구한다. 예상 밖 C4 행이나 형식 불량 행 하나라도 있으면 실패해야 한다. 부모 XML의 정식 시험 149개 정확 선택·통과, 실제 종료값, 입력 동일성은 기존 실행 검증기가 별도로 계속 요구한다. 비영 행의 누락·불일치 음성도 유지한다.
3. Unity 재실행 전에 원본 불변 XML·계획·행 원장 복사본으로 0행 양성을 검증하고, 임시 XML의 예상 밖 C4 행·형식 오류 각각 음성, 비영 행 양성·누락/불일치 음성, `Matrix91` C3 특례를 별도로 검증한다. 진단·회귀 출력은 새 경로에 기록하고 원본 C4 실행 출력은 변경하지 않는다. 루나가 변경 바이트와 각 판정을 독립 검수한다.
4. `artifacts/c4-final-validation-queue.ps1`의 결과 허용 경로 한 줄만 v11에서 새 `artifacts/c4-final-validation-queue-result-v12.json`로 옮긴다. 두 도구의 최종 실제 SHA와 본 계약·설계·독립 검토·새 회귀 증거를 새 `artifacts/c4-frozen-source-manifest-v18.json` 및 `artifacts/c4-frozen-input-paths-v18.json`에 결속한다. v17의 경로·사유·불변 지문과 모든 역사 결과를 보존하고 두 새 원장·입력은 `CreateNew`로 발급한다.
5. 새 계획 v14는 v18 원장·입력·두 도구 SHA를 결속한다. v13에서 이미 출력이 생긴 첫 다섯 실행은 각각 새 stem 및 여덟 새 출력 경로를 쓴다. 뒤 네 실행도 모든 출력 부재를 다시 확인한 경우에만 기존 stem을 유지한다. 아홉 선택의 순서·정식 이름·사례 수 `139/32/5/91/149/15/562/51/610`, 행 배분, 180초 제한, 잠금·중단·원시/QA/바깥 종료 판정은 유지한다. 결과 v12 및 72개 출력 경로는 발급 전 부재를 확인한다.

## 수용 조건

- `AC-M5D7QC4-009/010`: 0행 양성/예상 밖·형식 오류 음성, 비영 행 양성/누락·불일치 음성, `Matrix91` 377행 특례를 먼저 독립 검수한다. v18 파일별 지문·입력 포괄성·계획 v14 선택과 출력 부재를 확인한다.
- `AC-M5D7QC4-010`, 공동 `AC-M5D7QC3-007/008`: 실제 큐 아홉 실행의 정확 시험 개수·이름, native/QA/바깥 종료 0, 모든 필수 행, 각 실행과 실행 사이 1060개 입력 전후 동일성을 모두 충족할 때만 전체 수용한다. 새 불일치가 나면 원본을 보존하고 별도 분류한다.
