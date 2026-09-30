# C4 공통 검사 함수 불러오기 인자 보존

- 상태: **Approved — 네 호출자의 한정 수정만 승인, Unity 재실행·통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 검수 `docs/verification/2026-09-30-c4-library-parameter-preservation-luna-review.md` SHA-256 `71EC63A42131C9FD0D590640AE0BDD507EDBF5720081B35CCFF919D1BBA281E2`, 초안 SHA-256 `A0B85274AC75C7A46E5354313598727249B2474051AF22B8302C11A38BD86A8C`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4·QA r2 및 동결/입력 경로 보정. 제품 소스·시험 행·선택·Unity 설정은 변경하지 않는다.

계획 v1 SHA `ABB1E1A18CEDFBDB12681262F78B0F778E89C9A7324DA77B36B8D84E4874F7D0`은 독립 사전 검수를 통과했으나 첫 큐 호출은 `c4-final-validation-queue.ps1` 11행에서 `계획 지문 불일치`로 종료했다. 이때 실제 계획 지문은 그대로였다. 공통 `c4-verify-required-rows.ps1 -LibraryOnly`를 점으로 불러오면 PowerShell이 그 스크립트의 미지정 매개변수 `$PlanPath`, `$PlanSha256`, `$OutputPath`를 호출자 범위에서 빈 값으로 다시 결속하는 것이 원인이다. 작은 재현에서 `$PlanSha256='ABC'`가 불러오기 뒤 빈 값이 됐다. 이 실패는 mutex·입력 포착·Unity 기동보다 앞서 발생했고 queue result, XML/log, 72개 run 출력은 아직 없다. 계획 v1은 불변 실패 이력으로 보존한다.

아스트라는 Terra에게 공통 함수 파일을 다시 바꾸지 않고 다음 정확 네 호출자의 **불러오기 전후 인자 보존**만 구현하도록 배분한다: `artifacts/c4-final-validation-queue.ps1`은 `$PlanPath/$PlanSha256`, `artifacts/c4-final-selected-run.ps1`은 `$PlanPath/$PlanSha256`, `artifacts/c4-verify-run.ps1`은 `$PlanPath/$PlanSha256`, `artifacts/c4-capture-inputs.ps1`은 `$OutputPath`를 보존한다. 기존 `-LibraryOnly`·함수·계획 및 실행 검증 순서는 그대로 유지한다. `c4-verify-required-rows.ps1`의 단독 행 검증 매개변수와 나머지 세 생성 도구는 변경하지 않는다. 각 호출자에 대해 점 불러오기 뒤 원래 값이 보존됨을 실행 없이 결정적으로 확인하고, 변경 구간 외 바이트가 같음을 검증한다.

변경 뒤 `artifacts/c4-frozen-source-manifest-v4.json`을 `FileMode.CreateNew`로 발급한다. v3의 Files 102개 경로·순서에서 위 네 도구의 SHA만 바꾸고, `AllowedChanges`에 이 네 도구의 `REQ-M5D7QC4-007` 및 `AC-M5D7QC4-009/010` 근거를 더해 25행으로 만든다. 이전 v1/v2/v3 원장과 계획 v1은 불변 보존한다. fixture의 v1 원장 조회는 바뀌지 않은 C1/C2/Bridge/시험 파일 결속에만 쓰고, 최종 계획·runner·verifier의 권위 원장은 v4로 한다.

`artifacts/c4-frozen-input-paths-v4.json`은 입력 목록 v3의 971개 경로·이유를 모두 유지하고 새 v4 소스 원장과 본 최종 Approved 계약을 `Added`로 포함해 973개로 발급한다. `Sort-Object -CaseSensitive` 순서, 이전 884개, 소스 원장 v1 직접 입력, 출력·자기 경로 제외를 유지한다. 이전 입력 목록 v1/v2/v3과 계획 v1은 새 실행에서 읽지 않으므로 입력 경로에 넣지 않는다.

새 계획 `artifacts/c4-final-validation-queue-plan-v2.json`은 이전 계획 v1의 9회 순서·선택·행 ID·명령행 길이·출력 경로·잠금을 보존하고 `SourceManifest`와 `InputPathList`, 변경된 도구 네 FileBinding만 새 SHA로 교체해 `FileMode.CreateNew`로 발급한다. 같은 72개 출력과 `artifacts/c4-final-validation-queue-result-v1.json`은 모두 미발급 상태여야 하며 기존 계획을 덮어쓰지 않는다. 새 계획을 루나가 독립 사전 검수하고 아스트라가 별도 배분한 뒤에만 큐를 다시 시작한다. 큐는 기존처럼 처음 실패에서 중단하고 실제 종료값과 불확실성을 덮어쓰지 않는다. 어느 도구도 `WholeAccepted`를 true로 바꾸지 않는다.
