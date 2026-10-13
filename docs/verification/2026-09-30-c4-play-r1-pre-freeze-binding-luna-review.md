# C4 Play R1 복구 결속 자료 독립 정적 검토

- 적용 승인: `docs/specs/work-contracts/2026-09-30-c4-play-r1-bounded-recovery-amendment.md`, SHA-256 `2F07A599BCA19A515F9FD2644124D23480676B8FC74A5F84207C729BCD0DFC94`.
- Play 선택 v3: `artifacts/c4-focused-play-selection-v3.json`, SHA-256 `F572205953FCCD7EC50C1D6E586B8F84F036B22D939182B1022AC071C8E8E8EE`.
- 필수 행 v6: `artifacts/c4-required-checkpoint-rows-v6.json`, SHA-256 `4829FA125773C24514042D6C2911C1FEEDD62F81AEE65E06C545158674740144`.
- 큐 도구: `artifacts/c4-final-validation-queue.ps1`, SHA-256 `6D3F435E2655A8420252B9E8B2D8A550106E6CCB9792659ECA7DB7A4FF928279`.

## 판정

정적 결속 검토에서 P0 0건, P1 0건이다. 이는 선택·예상 행·큐 도구의 해당 지문 대조에 한정되며 동결 발급, 실행 준비 완료 또는 C4 수용 판정이 아니다.

## 확인 사항

Play 선택 v3는 v2와 비교해 예상 개수 32, 정식 시험 이름의 전체 목록과 순서, 선택식 및 플랫폼이 같다. 유일한 차이는 Play 부모 파일 지문으로, 이전 `1CDA66FB944D17489D6003AED24FAB665E4979C74ABAD242D81C7914133DDAA6`에서 현재 부모 `BC0C22518CE1A267547F72B758160610464E70D053D3A08F01CE2B80A5624BE9`로 바뀌었다.

필수 행 v6는 188행이다. v5와 `SourceFiles` 및 `EnumSources`를 제외한 전체 구조·행 내용을 정규화 비교했을 때 동일했다. `SourceFiles` 변경 세 건은 Edit fixture `A9677499FC8528656387CBA8926C030589A7E7522162C2031F90DAE4E1902F5B`에서 현재 `56AF5A3789CA0D010FBE0EA49331E8CAD0D1E267BC51955786B7789AE9A15DA9`, Play fixture `30330B3276F61F9686D9743CD7BFCA20B5CE6A5FFDC2E0994F8847949FAB1A0E`에서 현재 `215A813596E2C19B8FBAA49B070CC30A4AA448A057F618E0ECF5ADA34198D6E3`, Strict 시험은 기존 `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713` 그대로다. 세 현재 시험 파일의 실제 지문은 행 자료와 일치한다. `EnumSources`에서는 실행 브리지 한 건만 이전 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`에서 현재 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`로 바뀌었고, Disk transaction 및 memory cutover 지문은 유지됐다.

큐 도구는 결과 경로를 `artifacts/c4-final-validation-queue-result-v8.json`으로 고정한다. 이미 있는 결과 파일을 거절하고 프로젝트별 mutex와 fail-stop 실행 정책을 유지하며, 실패 시 `WholeAccepted=false` 결과를 만들고 중단하는 경로가 남아 있다. 현재 결과 v7은 보존되어 있다.

## 실행 전제

Play fixture는 `artifacts/c4-frozen-source-manifest-v11.json`을 참조하도록 바뀌었지만 v11 manifest는 아직 발급되지 않았다. 새 입력 목록·실행 계획·큐 결과 v8도 아직 없다. 따라서 이 자료만으로 실행을 시작하거나 이전 R1 실패 13건을 해결된 것으로 취급할 수 없다. Unity나 컴파일은 실행하지 않았다.
