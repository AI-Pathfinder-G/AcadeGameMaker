# C4 Edit·Hub 선택 지문 복구 구현 독립 정적 검토

검토 기준 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-final-plan-v9-selector-pin-recovery-amendment.md`의 SHA-256은 `D5FF6370CCAE9E65A5EDE32FC51E79F99FD155A618157AA1AAD84730ACCD6190`이다. Edit 선택 v6 `artifacts/c4-focused-edit-selection-v6.json`은 `A4D19A936CDE9974BA85FB946F6C2B571FF316F34C4B1192F7DB223C033B6F57`, 근거 JSON은 `3D8C1121E66FF86C62E2FB35ED5C75440B61841CD8BF1D3B2CF712E9BF4C15F7`, 로그는 `BC21AB73E764EF30CE23B7345B3B097459F1E05BFED586879A0D98F97882F949`이다. Hub 선택 v3 `artifacts/c4-focused-hub-selection-v3.json`은 `51262774D336EFAC859EB3631A9C488BC624BE09437C7AD640B556CEC7848389`, 근거 JSON은 `2F812FAEB7E6BD4A2DF8BA7AB7C753FDEB692EF9A072346F3B0BD777A366E4C6`, 로그는 `FADC7CB5FB0EB47C7DC2BC89F1B2DE4748E3CB84B0CAD6059853D3F9A29EFF28`이다.

각 근거는 서로 다른 격리 임시 작업 디렉터리와 각 고정 v1 출력의 사전 부재를 기록한다. 두 실행 모두 원본 생성기와 검증기, 부모, Bridge·디스크·메모리 열거형 소스의 복사 지문이 현재 실제 파일과 일치하고, 호출은 각각 `FocusedEdit` 및 `FocusedHub`, 프로세스 종료 코드는 0이다. 임시 생성물 SHA는 해당 새 선택 파일의 전체 SHA와 각각 일치한다. 각 로그의 지문도 근거 JSON과 현재 파일에 일치하며 호출과 종료 0을 기록한다. 과거 v1/v2/v5 선택의 실행 전후 지문은 변하지 않았고 현재 파일 지문도 기록값과 같다.

선택 v5→v6 비교에서 달라진 JSON 값은 `ParserEvidence.EnumSources[0].Sha256` 하나뿐이며 이름 수는 139다. Hub v2→v3에서도 같은 한 값만 달라지고 이름 수는 5다. 두 새 SHA는 현재 Bridge `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`와 일치한다. 두 선택의 다른 열거형 지문, 이름과 순서, 고정 선택식, 부모 `SourceFiles`, 파서 정보 및 나머지 전체 구조의 차이는 없다. 모든 근거의 복사 소스 여섯 개와 역사 선택 항목을 현재 파일과 독립 대조했으며 불일치는 0건이다.

## 판정

P0 0건, P1 0건이다. 이번 검토 범위인 승인 계약의 1·2항 결과는 충족한다. source manifest v14, input v14, 계획 v10은 아직 발급되지 않았으며 검토 대상도 아니므로, 이 결과는 최종 결속·Unity 실행·C4 수용을 뜻하지 않는다. 생성기와 Unity는 이번 검토에서 실행하지 않았다.
