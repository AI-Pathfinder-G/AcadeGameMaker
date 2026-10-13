# C4 최종 큐의 0행 선행 선택 검증기 복구 제안

- 상태: 제안. 아스트라 한정 승인 전 도구 수정·Unity 재실행·수용 권한이 없다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`; 기존 C3 149·562·51·610 선택과 C4 188행의 의미를 유지한다.
- 실패 원본: `artifacts/c4-final-validation-queue-result-v11.json` SHA-256 `D19C57E5B76C5BCBD95864E2F02695355251E2F9E71A6EB66F52F3F3A4DC9152`는 4개 실행을 기록하고 `c4-edit-remaining149`에서 중단했다. 이 선택의 XML 149/149 통과, 실제 native·QA·바깥 종료 0, 입력 1060개 전후 동일은 보존한다. 행 검증 진단은 `artifacts/c4-verify-required-rows.ps1:344`에서 `$rows.Count`가 없는 값이라는 오류를 재현했다. 앞선 네 실행의 수용 여부를 이 오류로 소급 바꾸지 않는다.

## 좁은 원인

계획의 `Remaining149.RequiredRowIds=[]`다. `artifacts/c4-verify-required-rows.ps1:337`은 계획의 빈 목록을 전달하고, `:343`의 `$rows=if(...) {@(...)} else {...}`는 빈 결과가 PowerShell 할당에서 배열로 유지되지 않아 `$null`이 된다. 이어 `:344`의 `.Count` 접근이 행 내용 판정 전에 실패한다. `C4-LegacyRun`은 `Matrix91`에만 적용된다(`:63`); Remaining149와 다른 0행 선행 선택은 현재 C4 행 검증기를 거친다. 이 실패는 제품 동작 실패나 149건의 NUnit 실패가 아니다.

## 한정 수정 계약 후보

1. 변경 도구는 `artifacts/c4-verify-required-rows.ps1` 한 파일이다. `:343`의 할당을 명시적인 두 분기로 바꿔 각 분기에서 `$rows=@(...)`를 직접 할당하고 빈 선택도 **실제 길이 0 배열**로 유지한다. `RequiredRowIds`를 받지 않은 비계획 호출은 기존 전체 기대 행 선택을 그대로 유지한다. `:344`의 정확 행 개수 검사, 비영 행의 `C4-MatchTerminal`·source·facts·nested·순서 검사, `C4-RunSelection`과 `Matrix91` 특례는 손대지 않는다.
2. 0행 계획에서는 정확히 `Rows=[]`, `Planned/Reached/Passed/Failed/NotReached=0`, `RequiredMissingIds=[]`, `UnexpectedIds=[]`, `InvalidIds=[]`, `SourceMatched=true`, `EvidenceMatched=true`여야 통과한다. XML의 아무 C4 행 기록이라도 예상 밖 ID로 잡고, 형식 불량 행은 `InvalidIds`로 잡아 실패한다. 빈 행을 이유로 예상 밖 기록을 무시하거나 `EvidenceMatched` 조건을 우회하지 않는다. 부모 XML의 전체 149개 정확 이름·통과·실제 종료·입력 동일성은 별도 `c4-verify-run.ps1`의 기존 판정 그대로 유지한다. 비영 행 계획은 기존 필수 ID 누락·중복·상태/값/확실성 불일치에 계속 실패해야 한다.
3. Unity 전에 도구만의 제한 회귀를 수행한다. 승인 후 복제한 불변 Remaining149 XML/계획/원장을 사용한 0행 양성, 임시 XML의 예상 밖 C4 행 하나·형식 불량 행 하나의 각각 음성, 필수 ID가 있는 기존 집중 행의 정확 일치 양성 및 누락·불일치 음성, `Matrix91`의 기존 특례를 분리해 확인한다. 임시 진단·회귀 결과는 새 경로에만 기록하고 원래 XML·행 비교·결과 v11은 수정하지 않는다. 이 검사만으로 최종 큐를 통과했다고 주장하지 않는다.

새 `artifacts/c4-frozen-source-manifest-v18.json`과 입력 v18은 두 변경 도구의 최종 실제 SHA·한정 승인 근거 및 이 제안/루나 검토/승인 문서를 정확 결속한다. 두 번째 도구 변경은 `artifacts/c4-final-validation-queue.ps1`의 결과 경로 허용 한 줄을 `artifacts/c4-final-validation-queue-result-v12.json`로 옮기는 것뿐이며 `CreateNew`·중단·잠금·종료 관찰은 유지한다. 계획 v14는 v18 원장·입력·두 도구 SHA를 결속한다. 첫 다섯 실행은 v13 계획에서 출력이 생겼으므로 각각 새 stem 및 여덟 새 출력 경로를 써 원본을 보존한다. 뒤 네 실행은 현재 출력 부재를 실행 직전 다시 확인하며, 하나라도 생기면 계획을 새로 발급한다. 아홉 선택의 정확 이름·순서·기대 수 139/32/5/91/149/15/562/51/610과 C4 188행·180초 제한은 불변이다.

루나는 배열 타입/0행 표면, 예상 밖·형식 오류 음성, 비영 행 엄격성, 변경 도구 두 줄의 범위와 실제 SHA를 독립 검수한다. 그 뒤 새 계획의 아홉 선택 live 지문·출력 부재·전후 입력을 확인하고 승인된 순차 큐에서 native/QA/바깥 0과 전체 행을 검증한다. source fixture·런타임·선택·기대 행·캡처/실행 검증기 판정은 바꾸지 않는다.
