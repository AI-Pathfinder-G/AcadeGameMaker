# C4 집중 Edit 현재 소스 지문 한정 복구 계약

- 상태: **Approved — 지정한 시험 보조 코드·증거 결속만 승인; 실제 재실행·통합 수용은 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`; 기존 `AC-M5D7QC4-001..008`의 행동·행 의미를 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-edit-r7-current-source-pin-recovery-design.md`, SHA-256 `8C4F98A6E3B1711D993F8DB692A893954C6A8AE9F42D7C45DCD818B5B37C733A`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-edit-r7-current-source-pin-luna-design-review.md`, SHA-256 `7710B4E25FC0A53A52C7D84153568FE654BA6B5369EF0AF2CAE3F12660723F4B`, P0/P1=0/0.
- 실제 결과 검수: `docs/verification/2026-09-30-c4-r7-focused-edit-first-run-luna-diagnostic.md`, SHA-256 `E26E67C0105CAA5C8B32C243ABFA7A7CB94C23A11CF2C1CCA5DF0EA6A6675282`, P0/P1=0/1.

계획 v11의 첫 실행 `c4-r7-focused-edit`는 Unity XML 139개 중 5개 통과·134개 실패, 원시 종료 2, 입력 v15 1044개 전후 차이 0이다. 결과 v9 SHA-256 `DD5B6E5F348A6CF05DCEEA116E745AD705725086CB59968F949BD20CDC7D47DF`는 `Runs=[]`와 첫 실행 fail-stop을 기록했다. 133건은 Edit fixture `AssertFrozenNoOpSingleCallSource`, 1건은 `AssertFrozenBusySource`에서 역사 Bridge 기대 SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`와 현재 실제·원장 SHA `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`가 달라 중단됐다. 뒤의 제품 동작은 이 실패로 판정하지 않는다. XML·로그·캡처·원시 종료·결과 원본을 보존한다.

## 허용 구현

1. 변경 가능한 시험 소스는 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` 한 파일이다. `AssertFrozenBusySource`와 `AssertFrozenNoOpSingleCallSource`는 현재 실행용 후속 source manifest v16을 읽고 정확 경로 유일성·`Kind=Runtime`·고정 예상 SHA·원장 SHA·실제 파일 바이트 SHA를 계속 대조한다. 두 함수의 Bridge 기대 SHA만 위 현행 SHA로 바꾼다. Busy의 다른 두 Runtime SHA와 `FileShare.None`·Busy 예외·C1/Begin/fresh/checkpoint 순서 검증, NoOp의 Owner SHA·표현식·Bridge 네 호출/Owner 세 호출의 각각 정확 한 번 검사는 유지한다. 검사를 삭제·완화·건너뛰지 않는다.
2. 같은 Edit fixture의 현재 부모·자기 파일 출처 검사 두 곳은 v12→v16 원장 참조만 바꾼다. 최종 Edit fixture 바이트 SHA를 v16 원장에 고정해 비재귀적으로 검증한다. 별도의 `AssertFrozenOrdinaryWriterSource` 역사 v1 검사와 그 두 Profile 소스 SHA는 유지한다. Play fixture는 v12 참조를 유지하고 새 원장도 동일 Play fixture SHA를 확인한다. 다른 Runtime, 부모, 선택, QA·캡처·검증기는 수정하지 않는다.
3. 기대 행 v8은 188개 행의 값·식별자·`Order`·출처 의미를 그대로 유지하고 `SourceFiles`의 Edit fixture SHA 한 값만 최종 실제 SHA로 바꿔 `CreateNew` 발급한다. 집중 Edit 선택 v6의 부모·139개 이름·선택식·열거형 현행 지문, 집중 Play v4·Hub v3 및 나머지 여섯 선택은 그대로다. source manifest v16은 Edit fixture, 행 v8, 승인 계약·설계·두 독립 검토의 정확 SHA와 변경 사유를 고정한다. 입력 v16은 v15의 허용 경로·사유를 보존하고 새 원장·행·문서를 직접 입력으로 더한다. 기존 v1/v12/v15 원장과 출력은 수정하지 않는다.
4. `artifacts/c4-final-validation-queue.ps1`의 결과 경로 비교 한 줄만 v9→새 `artifacts/c4-final-validation-queue-result-v10.json`으로 바꾸며 `CreateNew`·잠금·fail-stop·원시/QA/바깥 종료·행 검증은 유지한다. 변경 도구 SHA를 v16 원장·입력·계획 v12에 결속한다. 계획 v12의 첫 Edit 실행은 새 stem과 여덟 새 출력 경로를 사용하고, 나머지 여덟 실행은 모든 출력이 부재하는 경우에만 기존 stem을 유지한다. 전체 9회 순서·사례 수 `139/32/5/91/149/15/562/51/610`와 180초 제한·필수 행을 보존한다.

루나는 Edit fixture 두 출처 검사와 자기 지문, 별도 역사 검사 보존, 결과 경로 도구 한 줄, 행의 유일 SHA 변경, v16 원장·입력 및 계획의 실제 결속을 독립 정적 검수한다. 그 뒤에만 큐를 재실행한다. 실제 `AC-M5D7QC4-009/010` 판정에는 입력 전후 동일성·전체 행·원시/QA/바깥 종료를 모두 요구한다. 새 실패가 나타나면 원본을 보존하고 별도 분류한다.
