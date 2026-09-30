# C3 R11 편집 행렬 결과 독립 검수

검수 범위는 R11 EditMode 결정 분류·전이 행렬과 그 내부 377행 비교 결과다. C3 전체 수용이나 후속 실행 결과를 판정하지 않는다.

## 판정

P0 0건, P1 0건이다. 이 실행에서 선택된 NUnit 사례 91개가 모두 성공했고, XML 내부의 계획·성공 행 754개를 기대 원장과 대조한 결과 377개 ID 각각에 계획 1행과 성공 1행이 존재한다. 비교 원장도 `MatchedRows=377`, `EvidenceMatched=true`, 누락·예상 밖 ID·알 수 없는 상태가 0이다. AC002 분류 173행과 AC004 전이 204행이 각각 결과 및 사례 성공에 연결된다.

## 실제 증거 대조

- `artifacts/c3-r11-edit-matrix-r1.xml` SHA-256 `E90D83041C678955AE3938818BBFD9A5DAAFBCD9614CA24572C34E9BFC950533`: 원시 NUnit XML의 최상위 결과는 Passed, total/passed/failed는 91/91/0, skipped/inconclusive는 0이며 실제 duration은 2690.0793547초다. 모든 91개 테스트 사례 결과가 Passed다.
- `artifacts/c3-r11-edit-matrix-r1.log` SHA-256 `98C1164A2FD8D569E78E218075111B9155E3E0DA556E9A3CF654CD62F3C1AD6A`: 실제 편집기 로그의 종료 줄은 `Test run completed. Exiting with code 0 (Ok). Run completed.`이다. `qa/tools/Invoke-UnityQa.ps1`의 `Get-ClosedQaOutcome`은 실제 프로세스 종료 코드와 완전한 XML 카운터를 함께 확인하고, 종료 코드 0 및 전체 성공일 때만 `Completed`/0으로 분류한다. `verification.json`의 `ActualRunnerExitCode=0`, `Verified=true`와 QA 도구 반환 0이 이 종료 기록과 일치한다.
- `artifacts/c3-r11-edit-matrix-r1-decision-row-comparison.json` SHA-256 `B5FC981638BDF0CD91F3723462380BFC3FBC701D65376DD27AC32236D0B02B74` 및 기대 원장 `artifacts/c3-r11-required-decision-expected-rows.json` SHA-256 `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`: 비교 기록의 기대 ID·사례 매핑을 원시 XML의 각 사례 출력에 담긴 줄 단위 JSON과 다시 대조했다. 377개 ID 모두 계획/성공 쌍이 하나씩 있고, 연결된 NUnit 사례도 성공이다. 과거 R6 실행의 행별 계획·성공 로그는 NUnit 실패 결과로 수용되지 않았지만, 이번 R11 XML은 전체 91개 사례가 성공으로 닫힌 별도 실행이므로 혼동하지 않는다.
- AC002 173행은 실제 세 파일 역할·입력 조합별 분류 결과와 실행 전후 파일 지문이 동일함을 기록한다. 결과에는 `NoConfirmationRequired` 3건, `ConfirmationRequiredMeaningful` 93건, `ConfirmationRequiredAmbiguous` 62건, `Unreadable` 15건이 있으며 모두 기대값과 일치한다.
- AC004 204행은 세 파일 역할별 전이 입력·결과·세대 변화를 기록한다. `FreshDecisionRequired` 180건은 세대가 1 증가하고 새 결정 참조가 생긴다. `Confirmed` 21건은 새 결정 참조가 없고 확인된 원본 참조가 존재한다. `CaptureUnreadable` 3건은 기대된 실패 분류다. 204행 모두 `authorityChecked=true`이고 파일 지문은 각 적용 전이의 기대 상태와 일치한다.
- 시험 본문은 결과를 기록하기 전에 기존 결정·새 결정·표시 상태의 실제 객체 동일성, 새 결정의 단일 세대 증가, 재확인된 원본 캡처의 `MatchesRecapturedIdentity`를 단언한다(시험 파일 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs`, 125–174행). 로그의 `authorityReferences` 값은 객체 해시 기반 추적 표식이므로 그 숫자 자체를 유일한 권한 증명으로 취급하지 않았다. 해당 값은 원시 XML에서 기대된 null/비-null 관계 및 사례 성공을 교차 확인하는 보조 자료다.
- 실행 전후 입력 스냅샷은 각각 884개 항목이며 기록된 차이는 0이다. R11 동결 원장에 든 14개 파일을 현재 바이트와 직접 대조한 차이도 0이다. 검증 원장의 해시는 `5025B9D60B0CDC98D8E9337C52ED0BE0A1E7D106E08B85D75C67EF277FFEA748`, 소스 전후 스냅샷 해시는 각각 `263E855D43C6B92DBCA0351BEE2B01DAD337841181854C82CA9252DA5DA074CF` 및 `72AAF3E52E236E06904D6A9A17BE38B75B71D2CFA1C324C95485BDA689816100`이다.

## 한계

이 판정은 현재 R11 EditMode 행렬과 그 안의 AC002/AC004 377개 행만 수용 가능한 증거로 확인한다. 240개 전체 Edit 선택, PlayMode 15개, 후속 AC001/005/기타 선택 및 필수 회귀 실행은 별도 결과가 필요하다. C3 전체, C4 및 제품 통합은 수용하지 않는다. 이 검수에서는 Unity·컴파일·Git·네트워크 실행을 하지 않았고 기존 증거 파일을 변경하지 않았다.

게시 주석(2026-10-01): 위 시험 소스는 게임 코드의 별도 게시 승인 전까지 이 문서 변경 요청에 포함하지 않는다. 기존 실행·독립 검수의 역사적 판정 범위는 바꾸지 않는다.
