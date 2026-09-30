# C4 최종 큐의 실행 전 입력 충돌 한정 복구 계약

- 상태: **Approved — 아래 실행 도구 한 줄과 증거 결속만 승인; 실제 큐 재실행·통합 수용은 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`.
- 설계: `docs/proposals/2026-09-30-c4-final-queue-pre-unity-capture-recovery-design.md`, SHA-256 `15E770FBF6231849CC41C89E7E02BFCDA51FF5D6831F3CEA6A3E0462CCF52ACD`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-pre-unity-capture-recovery-luna-r2-review.md`, SHA-256 `5C7DEEC694B5ED51E19D98107149D371A3FA72BE6AE931663E87407FE982DD77`, P0/P1=0/0. 최초 지적은 `docs/verification/2026-09-30-c4-pre-unity-capture-recovery-luna-design-review.md`, SHA-256 `03CAC8194F89FD6B8E58BD4FD109505E1DBA9480C865459A9A34DEB4F68293A7`에 보존한다.

계획 v10의 큐 결과 v8은 SHA-256 `573480005473F747DC41283E1D9F14C0A4F650E786737A81F87F692962F7031E`, `Runs=[]`, 중단점 `c4-r7-focused-edit`다. 첫 입력 캡처가 `artifacts/c4-capture-inputs.ps1:8`에서 두 `artifacts/c4-*-builder.log` 경로를 출력으로 분류해 거절했다. Unity·XML·입력 전 캡처가 실행/발급되기 전 실패했으므로 제품 결과로 해석하지 않는다. 원본 v8 결과, v10 계획, v14 원장·입력과 로그·증거를 보존한다.

## 허용 수정과 동결

1. `artifacts/c4-final-validation-queue.ps1`의 결과 경로 비교 한 줄만 새 `artifacts/c4-final-validation-queue-result-v9.json`을 허용하도록 바꾼다. 원본 v8을 덮어쓰지 않으며 `CreateNew`, 단일 프로젝트 잠금, fail-stop, 실제 원시·QA·바깥 종료 기록, 행·입력 검증을 바꾸지 않는다. 다른 QA·캡처·검증 도구는 수정하지 않는다.
2. 실행용 source manifest v15의 `Files`와 `AllowedChanges`에서 정확히 `artifacts/c4-focused-edit-selection-v6-builder.log`, `artifacts/c4-focused-hub-selection-v3-builder.log` 두 경로만 제외한다. v14의 나머지 경로·지문·종류·사유를 보존한다. v14 원장과 v14 입력 목록 원본 자체를 실제 SHA의 `Evidence`로 v15 원장과 입력에 추가해 역사 로그 두 개의 `Files`/`AllowedChanges`/`ReasonMap` 지문과 제거 근거를 추적 가능하게 유지한다. 수정한 큐 도구는 `Tool`과 허용 변경 사유로 결속한다. 새 승인 계약·설계·독립 검토를 각각 정확 SHA의 증거로 포함한다.
3. 실행용 입력 목록 v15의 `Paths`와 `ReasonMap`에서 같은 두 로그 경로·행을 모두 제외한다. 두 비로그 builder 증거 JSON과 나머지 v14 경로·사유는 보존한다. 로그는 고정 선행 캡처 884개에 없으므로 `Removed` 행을 추가하지 않는다. 대신 이 계약과 v14 원장·입력 결속이 v14→v15 제외 사유를 소유한다. v15 원장·변경 큐 도구·이 계약·설계·독립 검토를 직접 입력으로 추가한다. `ExcludedOutputPatterns`, 캡처·검증 판정, 입력 범위 검사는 유지한다. v15 `Files`의 모든 경로는 v15 `Paths`에 정확히 포함돼야 한다.
4. 계획 v11은 source manifest v15, input v15, 변경 큐 도구의 실제 SHA와 새 결과 v9에 결속한다. 기존 9개 실행의 선택·정식 이름·순서·예상 수 `139/32/5/91/149/15/562/51/610`, 188행·필수 행·180초 제한·9개 stem과 예정 출력은 변하지 않는다. 첫 실행의 모든 출력과 결과 v9가 실행 직전 비어 있어야 하고, 하나라도 있으면 새 경로를 승인받아 발급한다.

루나는 결과 경로 조건 한 줄 외 도구 바이트가 같은지, v14→v15 두 로그 제외와 역사 원장 결속, 현재 입력·소스의 포함 관계, 9개 선택의 실물 지문·출력 부재를 독립 검수한다. 이후에만 새 큐를 실행한다. `AC-M5D7QC4-009/010`의 실제 전후 동일성, 원시·QA·바깥 종료, 전체 행은 새 결과로 판정한다. 실패는 원본과 분리해 기록하며 과거 32/32 비수용 진단 또는 큐 v8 실패를 소급 수용하지 않는다.
