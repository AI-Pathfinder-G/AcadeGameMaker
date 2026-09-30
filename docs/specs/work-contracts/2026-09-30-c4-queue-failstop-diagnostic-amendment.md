# C4 실패 큐 결과 보존과 재기동 진단

- 상태: **Approved — 큐 진단의 한정 수정만 승인, Unity 재실행·통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 검수 `docs/verification/2026-09-30-c4-queue-failstop-diagnostic-luna-review.md` SHA-256 `1E150E942438585ED1B5742F7D7ED99652D3090D3E8329D6B38E9E55C52E7490`, 초안 SHA-256 `DAFF2AD060A47C572E194FCF9615FEF1AE94960FAE7CC0FCFD881B91F0EBC80A`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4·QA r2 및 이후 동결·인자 보존 보정. 제품 소스·시험·선택·행은 바꾸지 않는다.

계획 v2 SHA `7690DBC504D04908931C47C7E7CA3FD4FF3D26814085E630703DBF321D319797`의 큐는 첫 focused Edit 하위 실행기에서 표준출력 JSON이 비어 있어 중단했다. `artifacts/c4-final-validation-queue-result-v1.json`은 실패 원인·멈춘 stem과 `Runs:[]`, `WholeAccepted:false`를 `CreateNew`로 기록했다. 실제 72개 run 출력과 Unity 프로세스는 없었다. 이 결과는 실패의 불변 증거이며 덮어쓰지 않는다. 하위 실행기의 표준오류는 현 큐가 읽고 버리므로, 정확한 자식 오류는 이 결과만으로 확인할 수 없다.

재기동 때 최상위 PowerShell의 `-File` 인자에는 큐 스크립트의 **정규화된 절대 경로**를 사용한다. 이전 호출에 사용된 `.\artifacts\c4-final-validation-queue.ps1`은 하위 실행기의 `C4-Normal`이 요구하는 절대 경로 형식과 다르다. 이는 관찰된 빈 JSON의 가능한 원인이며, 실제 자식 표준오류가 없으므로 확정 사실로 보고하지 않는다. 상대 경로를 허용하도록 소유권 검사를 완화하지 않는다.

아스트라는 Terra에게 `artifacts/c4-final-validation-queue.ps1` 한 파일만 보정하도록 배분한다. 큐 명령에는 한정 `-ResultPath` 인자를 추가하고, 이번 새 출력은 정확 `artifacts/c4-final-validation-queue-result-v2.json`로 고정한다. 기존 result-v1 존재 검사를 이 v2 경로 검사로 바꾸고, 최종 결과도 그 경로에 `CreateNew` 한 번만 작성한다. 결과 schema·`PlanBinding`·`SourceManifestBinding`·`InputListBinding`·`Runs`·실패중단 규칙과 각 run의 8개 출력 경로는 바꾸지 않는다. 하위 실행기 표준오류를 내부에서 보관하되, stdout JSON이 비었거나 하위 종료값이 0이 아니면 JSON 파싱 전에 그 실제 종료값과 오류를 `FailureReason`으로 기록한다. 비어 있는 stdout을 성공 JSON으로 만들지 않는다. 표준오류가 비어 있어도 실패를 그대로 유지한다. 임의 결과 경로, 기존 v1 덮어쓰기, 실패 뒤 다음 실행은 허용하지 않는다.

변경 뒤 `artifacts/c4-frozen-source-manifest-v5.json`을 `FileMode.CreateNew`로 발급한다. v4 Files 102개에서 큐 스크립트의 SHA 한 개만 바꾸고, 기존 큐 `AllowedChanges` 행의 `Reason`에 이 승인된 `AC-M5D7QC4-010` 한정 진단·v2 결과 근거를 추가한다. 다른 24행과 `RequirementIds`는 유지한다. v1~v4 소스 원장은 보존한다. `artifacts/c4-frozen-input-paths-v5.json`은 기존 v4 입력 973개·이유를 모두 유지하고 v5 소스 원장 및 본 최종 Approved 계약을 더해 975개로 만든다. 소스 원장 v1을 포함한 직접 입력, 기존 884개, `Sort-Object -CaseSensitive`, 자기/결과/실행 출력 제외를 유지한다.

새 `artifacts/c4-final-validation-queue-plan-v3.json`은 계획 v2의 9개 run·선택·행·잠금·72개 출력 경로를 그대로 보존하고 source/input 원장 및 큐 도구 FileBinding 세 곳만 v5 값으로 바꾸어 `CreateNew`로 발급한다. 계획 v1/v2와 실패 result-v1은 보존한다. Luna가 새 큐 도구의 실패 표면, v5 원장·입력, 계획 v3을 독립 검수하고 아스트라가 별도 배분한 뒤에만 **절대 경로**의 큐를 다시 시작한다. 새 result-v2가 실패해도 덮어쓰거나 성공으로 바꾸지 않는다.
