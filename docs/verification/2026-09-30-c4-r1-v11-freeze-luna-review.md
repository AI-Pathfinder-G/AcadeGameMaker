# C4 R1 진단 전 동결 자료 독립 정적 검토

- 승인 경계: `docs/specs/work-contracts/2026-09-30-c4-play-r1-bounded-recovery-amendment.md`, SHA-256 `2F07A599BCA19A515F9FD2644124D23480676B8FC74A5F84207C729BCD0DFC94`.
- source manifest v11: `artifacts/c4-frozen-source-manifest-v11.json`, SHA-256 `36B4655904C898176B79655E0243005B997A607FFBB32D1FF85E789552C85EFB`.
- input list v11: `artifacts/c4-frozen-input-paths-v11.json`, SHA-256 `84216F8B717F2D897B0093B77874502DC293556832D8577A26FCC814B013BCE0`.
- 참조 결속: Play selection v3 `F572205953FCCD7EC50C1D6E586B8F84F036B22D939182B1022AC071C8E8E8EE`; required rows v6 `4829FA125773C24514042D6C2911C1FEEDD62F81AEE65E06C545158674740144`; 입력 안의 GPT 참여 원장 `23085FFE1965391849BAD920C0C7DBBB92789F1FABFD05870302AF55126BFCC2`.

## 판정

이번 범위의 P0 0건, P1 0건이다. v11 원장과 입력 목록은 실제 파일 및 승인된 R1 제한 복구 변경에 맞는다. 이 단계는 비수용 Play 진단 실행의 동결 전제 검토만 다루며 실행 계획, 큐, Unity 실행 또는 C4 통합 수용은 검토하지 않았다.

## 대조 결과

v10 manifest의 123개 경로·순서는 v11의 앞 123개에 그대로 유지됐다. 기존 항목 중 SHA가 바뀐 것은 6개이며, 신규 증거 경로 8개가 추가돼 v11은 131개다. 모든 131 경로가 존재하고 현재 바이트 SHA가 manifest와 일치했다. 여섯 SHA 변경은 승인된 실행 브리지, Edit/Play fixture, Play 부모, 큐 도구 및 GPT 참여 원장이다. 새 증거 8개는 승인 계약·제안·독립 검토·선택 v3·행 v6로 한정된다. 여섯 변경 경로와 여덟 신규 경로 모두 manifest의 허용 변경 원장에 등재돼 있다. 허용 변경 항목 수는 56이다.

Play fixture의 현재 SHA `215A813596E2C19B8FBAA49B070CC30A4AA448A057F618E0ECF5ADA34198D6E3`, Play 부모 `BC0C22518CE1A267547F72B758160610464E70D053D3A08F01CE2B80A5624BE9`, 실행 브리지 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`가 selection/required rows와 일치한다. Edit fixture `56AF5A3789CA0D010FBE0EA49331E8CAD0D1E267BC51955786B7789AE9A15DA9`와 Strict 시험 `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713`도 rows v6의 source 참조와 일치한다. 바뀐 fixture는 자기 참조를 원장 v11로 갱신했고, 행 파일은 각 소스 참조 외의 기존 188개 행을 보존한다.

v11 input은 1,017개 경로와 1,017개 이유를 갖는다. v10의 1,008개 경로와 각 이유가 모두 유지되고, 승인 근거와 현재 source/selection/rows 원장 등 직접 입력 9개가 더해졌다. 전체 경로는 `Sort-Object -CaseSensitive` 순서와 일치하며 중복·누락 파일·빈 이유가 없다. 53개 기존 파일이 경로 목록에서 빠져 보이는 것은 `qa/results/` 출력 제외에 해당하며, capture 도구가 별도로 제외하는 `__pycache__` 파일 한 건도 입력 범위 누락으로 세지 않았다. `ExcludedOutputPatterns`와 predecessor capture는 v10과 같고, 현재 queue/XML/log/before·after/검증 산출물은 목록에 없다. 목록은 자기 자신을 포함하지 않으며, fixture가 직접 읽는 manifest v11과 현재 GPT 참여 원장은 입력에 포함된다. 전체 source tree에서 승인된 출력·캐시 제외 외의 누락은 확인되지 않았다.

## 제한

manifest v11의 131개 실물 SHA와 input v11의 1,017개 파일 존재·정렬·사유를 검증했지만, 새 plan·queue 및 실행 산출물은 이 검토 대상에 없다. 따라서 이 검토는 Play 진단 실행을 허가하거나 이전 13개 실패를 해소하지 않는다. Unity 및 컴파일은 실행하지 않았다.
