# C4 최종 큐의 Unity 진입 전 입력 충돌 복구 제안

- 상태: 제안. 구현·실행·수용 권한이 아니다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`.
- 원본: `artifacts/c4-final-validation-queue-result-v8.json` SHA-256 `573480005473F747DC41283E1D9F14C0A4F650E786737A81F87F692962F7031E`은 `Runs=[]`, 중단점 `c4-r7-focused-edit`를 기록한다. XML·입력 전 캡처가 발급되기 전 `artifacts/c4-capture-inputs.ps1:8`에서 실패했다. v10 계획과 v14 소스·입력 원장, 두 builder 로그·증거, 기존 출력은 수정하지 않는다.

## 원인과 필수 결속 조건

v14 입력의 `Paths`에는 `artifacts/c4-focused-edit-selection-v6-builder.log`와 `artifacts/c4-focused-hub-selection-v3-builder.log`가 들어 있다. 그러나 `artifacts/c4-capture-inputs.ps1:8`은 `artifacts/c4-*.log`를 입력에서 거절하고 v14의 `ExcludedOutputPatterns`도 같은 경로를 제외한다. 로그 둘은 v14 소스 원장 `Files`에 각각 `Kind=Evidence`와 정확 SHA로 한 행씩, `AllowedChanges`에 각각 한 행씩 있으며 v14 입력 `ReasonMap`에도 각각 `Added` 한 행씩 기록돼 있다. 역사 원장·입력과 실제 로그를 그대로 유지한다.

단순히 v15 입력에서 로그 둘만 빼면 충분하지 않다. `artifacts/c4-verify-run.ps1:13`은 실행 계획의 소스 원장 `Files` **모두**가 전후 입력 캡처에도 정확 경로·SHA로 존재할 것을 요구한다. 따라서 실제 실행용 v15 소스 원장의 `Files`와 `AllowedChanges`에서도 로그 두 경로를 제외해야 한다. v14 원장 자체를 v15의 `Evidence` 및 v15 입력에 정확 SHA로 고정하면, 두 로그의 역사 지문은 v14 내부에서 검증 가능한 형태로 남는다. v15 `Files`의 비로그 증거 JSON 두 건은 그대로 유지한다.

또한 `artifacts/c4-capture-inputs.ps1:9-10`의 `ReasonMap`은 고정된 C3 선행 캡처의 경로와 현재 `Paths` 합집합에 대해서만 정확 한 행을 허용한다. 두 로그는 v14에서 처음 추가됐고 고정 선행 캡처에는 없다. 그러므로 v15 `Paths`와 `ReasonMap`에서 **로그 두 경로와 두 사유 행을 모두 제거**해야 한다. `Removed` 두 행을 넣으면 오히려 검증이 실패한다. **v14→v15 제거 사유는 새 Approved 후속 계약에 정확 두 경로를 명시하고, v14 소스 원장과 v14 입력 목록 양쪽의 실제 SHA를 후속 원장에 결속**한다. `ReasonMap`의 의미를 바꾸거나 캡처·검증 도구를 완화하지 않는다.

## 한정 보정과 발급 순서

1. 새 `artifacts/c4-frozen-source-manifest-v15.json`은 v14의 기존 실제 소스·시험·선택·증거 SHA를 보존하되, 두 builder 로그를 `Files`와 `AllowedChanges`에서 제외한다. v14 소스 원장과 v14 입력 목록 자체를 각각 정확 SHA의 `Evidence`로 추가해 제거 전 결속을 보존한다. `artifacts/c4-final-validation-queue.ps1`의 승인된 새 바이트와 근거도 `Tool`·`AllowedChanges`로 고정한다. 새 승인 계약·이 제안·루나 독립 검토를 각각 정확 SHA의 증거로 결속한다. 모든 `Files`가 v15 입력에 존재하도록 대조하고 실제 `Count`를 산출한다.
2. 새 `artifacts/c4-frozen-input-paths-v15.json`은 v14의 허용 입력 경로를 유지하되 두 `builder.log`만 `Paths`에서 제거하고 두 비로그 builder 증거 JSON은 유지한다. v14 원장·v14 입력 목록, v15 원장, 변경된 큐 도구, 새 승인·설계·검토 문서를 직접 입력에 넣는다. 선행 캡처 기준 `ReasonMap`을 정확 재산출하면서 로그 두 사유 행도 제외한다. 로그 제거 이유는 위 계약의 명시 문장과 v14 원장·입력 결속으로 추적한다. `ExcludedOutputPatterns`와 캡처·검증 규칙은 바꾸지 않는다.
3. `artifacts/c4-final-validation-queue.ps1:11`의 정확 결과 경로 조건 한 줄만 `artifacts/c4-final-validation-queue-result-v9.json`을 허용하도록 바꾼다. 기존 v8 결과 파일과 결과 작성의 `CreateNew`·중단·상호 배제·실제 종료 관찰은 유지한다. 새 도구 SHA를 v15 원장·입력 및 새 계획의 도구 결속에 반영한다. 다른 QA 도구는 변경하지 않는다.
4. 새 `artifacts/c4-final-validation-queue-plan-v11.json`은 `SourceManifest=v15`, `InputPathList=v15`, 바뀐 큐 도구 SHA와 새 결과 v9 경로에 결속한다. 9개 실행의 선택·139/32/5/91/149/15/562/51/610 정식 이름과 순서, 188행, 180초 제한, 기대 결과, 기존 stem은 바꾸지 않는다. 결과 v9와 첫 실행의 XML·전후 캡처·로그·종료·행 출력이 **현재 부재**인지 실행 직전 다시 확인하고, 하나라도 있으면 새 stem/결과 버전의 별도 계약을 요구한다. 기존 v8 실패 결과와 v10 계획을 덮어쓰지 않는다.

검증 순서는 v14 로그·원장 역사 SHA 확인 → v15 `Files`와 입력 경로의 정확 포함 관계 및 두 로그 배제 → 수정 한 줄과 새 도구 SHA 독립 검수 → 새 계획의 아홉 선택 실물 지문·이름·기대 행·출력 부재 사전 점검 → 전후 캡처 동일성·실제 원시/QA/바깥 종료 0·전체 행·루나 독립 검수다. 현재 실패는 Unity 실행이나 제품 결함 증거로 분류하지 않는다. 제품 코드·fixture·선택·행·캡처 및 실행 검증의 판정은 변경하지 않는다.
