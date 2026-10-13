# C4 Play R2 동결 결속의 제한 후속안

- 상태: 제안. 기존 한정 승인 범위를 넓히려면 아스트라의 별도 정확 승인이 선행되어야 한다. 구현·실행·수용 증거가 아니다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`. Play 행동의 `AC-M5D7QC4-001..008`과 기존 후속 계약은 그대로 둔다.
- 원본: `docs/specs/work-contracts/2026-09-30-c4-play-r2-current-source-pin-amendment.md`는 Play 선택 v3 불변을 명시한다. 이 제안은 그 문서와 `artifacts/c4-frozen-source-manifest-v12.json`, `artifacts/c4-focused-play-selection-v3.json`, `artifacts/c4-frozen-input-paths-v12.json`을 수정하지 않는다.

## 확인된 두 결속 차이

1. v12 소스 원장의 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` `ChangeAuthority`는 “현재 원장 v11 결속”이라고 쓰지만, 같은 행의 `AllowedChanges.Reason`은 승인된 R2의 v12 결속이다. 실물 SHA `16DEB90F87C0E66A6DFD8C04E9C0DB6EC91F59F0051690853B61FA5C49489B0C`는 v12 `Files`와 같다. 이는 파일 내용이나 시험 동작의 불일치가 아니라 원장 사유의 불일치다.
2. Play 선택 v3의 `ParserEvidence.EnumSources`는 `ProfileResetExecutionBridgeV1.cs`를 옛 SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`로 결속한다. 현재 실물, v12 소스 원장, 188개 행의 v7 열거형 결속은 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. 선택 v3의 32개 정식 이름·고정 선택식·부모 소스 SHA `BC0C22518CE1A267547F72B758160610464E70D053D3A08F01CE2B80A5624BE9`는 그대로다. `artifacts/c4-verify-required-rows.ps1:40`의 `C4-Selection`은 열거형 결속에 `C4-CheckBinding`을 호출하므로 v3을 최종 실행에 그대로 쓰면 실제 현재 소스와 맞지 않는다.

## 새 승인에 한정할 자료

- `artifacts/c4-focused-play-selection-v4.json`: 기존 `artifacts/c4-build-focused-selection.ps1`의 원문 바이트로 실제 재생성한다. 이 도구는 출력명을 v1로 고정하므로, 승인 뒤 필요한 기존 도구·부모·열거형 소스의 같은 바이트를 임시 동일 상대경로 작업 공간에 배치하여 **그 공간 안에서만** `FocusedPlay` 및 v3과 같은 열거형 소스 경로·순서로 실행한다. 임시 생성 v1을 작업 공간의 새 v4 경로로 발급하며 저장소의 역사 v1과 도구 원본은 건드리지 않는다. v3과 필드·배열 순서·값을 구조 비교하여 **Bridge 한 개의 `ParserEvidence.EnumSources.Sha256`만** 현재 SHA로 달라야 한다. 다른 열거형 지문, 32개 이름·순서·선택식·부모 `SourceFiles`·파서 수·플랫폼·종류 등 나머지 모든 필드가 동일해야 한다. 재생성 실패 또는 다른 차이는 승인된 결과가 아니다. v4의 실제 새 파일 SHA를 고정한다. 현행 `artifacts/c4-required-checkpoint-rows-v7.json`의 188개 행 내용·`Order`·`SourceFiles`·열거형 지문은 바꾸지 않는다.
- `artifacts/c4-frozen-source-manifest-v13.json`: v12의 기존 파일·사유·선행 결속을 보존하고 Edit fixture의 `ChangeAuthority` 한 문구만 승인 R2 v12 결속으로 바로잡는다. 새 선택 v4를 `Evidence`와 정확 SHA로 한 번 추가하고 `AllowedChanges`에 `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`의 한정 사유를 연결한다. fixture가 읽는 v12 원장 자체와 **이 제안서, 이 제안서의 루나 독립 검토 문서, 새 Approved 후속 계약 문서** 세 문서를 새 원장에 각각 별도 `Evidence`로 정확 SHA를 고정한다. 각 경로는 유일해야 하며 최종 `Count`는 실제 `Files.Length`로 산출한다. Bridge·Owner·두 fixture·부모 등 기존 소스의 SHA는 바꾸지 않는다.
- `artifacts/c4-frozen-input-paths-v13.json`과 후속 실행 계획: v12의 경로·사유를 보존하면서 선택 v4, 소스 원장 v13, **이 제안서·루나 독립 검토·새 Approved 후속 계약 문서 세 문서**를 각각 직접 입력으로 추가하고 정확 수를 산출한다. 새 계획의 `SourceManifest`는 v13, `InputPathList`는 v13, 집중 Play `Selection`은 v4의 경로와 실제 SHA를 가리킨다. 기존 계획과 출력은 보존하고 새 경로에만 기록한다. 기존 선택 v3도 역사 입력으로 남긴다.

Play fixture `C4ActualExecutionPlayFixtureV1.cs:884,979`와 Edit fixture `C4ActualExecutionEditFixtureV1.cs:1196,1372`는 여전히 v12를 읽는다. v12는 최종 Play fixture SHA `F73BCC9EA0493214FC65B3005862DA5666F6EBEDD759C68CB9910D006D2C7B10`과 Edit fixture SHA `16DEB90F87C0E66A6DFD8C04E9C0DB6EC91F59F0051690853B61FA5C49489B0C`를 이미 고정한다. v13도 이 **동일한 소스 바이트**를 독립 대조하고, 입력 v13은 v12와 v13을 모두 동결한다. fixture를 v13 읽기로 바꾸면 자체 SHA와 원장 발급이 다시 움직이므로 이 후속안에서는 소스 편집을 하지 않는다. `artifacts/c4-verify-required-rows.ps1:41`의 `C4-Manifest`와 `artifacts/c4-final-validation-queue.ps1:6-17`, `artifacts/c4-verify-run.ps1:6-13`은 계획에 지정된 v13 원장의 각 파일 SHA 및 전후 캡처와 선택 결속을 검증한다. 원장의 비어 있지 않은 `ChangeAuthority`만 형식 검사하므로 그 문구의 규범 일치는 별도 독립 정적 검수 항목이다.

검증 순서는 실제 두 런타임·fixture·부모 SHA 재확인 → 기존 도구의 격리 재생성과 v3 대비 전체 구조 비교 → v4의 유일한 열거형 지문 차이 및 32개 이름·선택식·부모 불변 대조 → v13의 Edit 사유·v4/v12/세 문서 증거·기존 파일 SHA 대조 → 입력 v13과 새 계획의 직접 결속 및 전체 파일 전후 동일성 확인 → 새 Play 32건의 실제 원시 종료·행 결과 확인 → 루나 독립 P0/P1 검수 및 승인된 최종 큐다. 이 정적 보정만으로 이전 0/32 실패를 통과로 간주하지 않는다. 일곱 호출 토큰, 제품 코드, QA 판정, 기존 188행의 값과 순서에는 변경 권한이 없다.
