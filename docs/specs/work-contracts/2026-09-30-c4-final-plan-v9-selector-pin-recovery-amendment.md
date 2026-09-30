# C4 최종 계획의 편집·허브 선택 지문 한정 복구 계약

- 상태: **Approved — 아래 증거 발급과 실행 전 결속 검증만 승인; 실제 9회 결과·통합 수용은 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`; 기존 행동 `AC-M5D7QC4-001..008` 및 선행 승인 계약은 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-final-plan-v9-selector-pin-recovery-design.md`, SHA-256 `3A3740BDB35C8A5F01DC7A795B67F1955605667A6F84782B489D07DD4D7602B5`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-final-plan-v9-selector-pin-recovery-luna-final-review.md`, SHA-256 `13DAC3354181B00F3E8685EE495CE02DF6099F0FF2F7D2D3B1F2699488671BB7`, P0/P1=0/0.
- 차단 근거: `docs/verification/2026-09-30-c4-final-queue-plan-v9-luna-review.md`, SHA-256 `D9054862EFD8FE9A39B1705289A7179232E159DF13EED08D0C0144E16058E1C5`, P0/P1=0/1.

최종 계획 v9에서 집중 Edit 선택 v5와 집중 Hub 선택 v2의 `ParserEvidence.EnumSources` Bridge SHA만 옛 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`다. 현행 실물·원장 v13·기대 행 v7은 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. 검증기는 이 SHA를 실물과 대조하므로 v9 실행은 시작하지 않는다. 원본 선택·계획·원장·입력·이전 출력은 불변으로 보존한다.

## 허용 발급과 한계

1. 기존 `c4-build-focused-selection.ps1`를 필요한 같은 바이트 소스·도구와 함께 Edit용과 Hub용 **서로 다른** 안전한 임시 동일 상대경로 공간에서 각각 정확히 한 번 실행한다. 각 공간의 고정 v1 출력은 사전 부재해야 한다. 생성 v1을 새 `artifacts/c4-focused-edit-selection-v6.json`, `artifacts/c4-focused-hub-selection-v3.json`으로 `CreateNew` 발급한다. 각 이전 선택과 전체 JSON 구조·배열 순서를 대조해 해당 Bridge 열거형 SHA 한 값 외 차이가 0이어야 한다. 이름·순서·선택식·부모·다른 열거형 및 파서 근거·플랫폼·종류는 불변이다. 저장소의 기존 v1/v5/v2는 전후 SHA가 같아야 한다.
2. 각 재생성마다 별도 `CreateNew` 기계 판독 증거와 로그를 발급한다. 경로는 `artifacts/c4-focused-edit-selection-v6-builder-evidence.json`, `artifacts/c4-focused-edit-selection-v6-builder.log`, `artifacts/c4-focused-hub-selection-v3-builder-evidence.json`, `artifacts/c4-focused-hub-selection-v3-builder.log`다. 각각 임시 v1 사전 부재, 복사 소스·도구 SHA, 실제 명령·작업 경로·종료값, 생성물 SHA·새 선택 전체 바이트/구조 일치, 이전 선택 대비 유일 SHA 차이, 저장소 역사 v1 전후 SHA, 로그 SHA를 기록한다. 임시 공간 정리는 확인된 작업 공간 경로에만 수행한다.
3. 네 증거가 완성된 뒤 `artifacts/c4-frozen-source-manifest-v14.json`을 `CreateNew` 발급한다. v13의 기존 파일·순서·SHA·종류·권한 사유를 보존하고 새 선택 둘·증거 넷·이 승인 계약·설계·독립 검토·원장 v13을 각각 유일한 경로와 실제 SHA의 `Evidence`로 추가한다. 신규 항목은 `AllowedChanges`에 정확 경로·요구사항·사유를 기록한다. 원장 파일 수는 실제 길이와 같아야 한다. 제품·fixture·부모·기대 행의 SHA는 바꾸지 않는다.
4. 새 입력 목록 v14는 v13의 1029개 경로·사유를 보존하고 v14 원장·새 선택 둘·재생성 증거 넷·이 계약·설계·독립 검토를 직접 입력으로 추가한다. v12·v13·v14 원장이 모두 입력에 결속되어야 한다. 두 fixture는 v12를 계속 읽으며 v13/v14는 같은 최종 fixture 바이트를 독립 대조한다. 소스·QA 도구·기대 행 188개를 수정하지 않는다.
5. 계획 v10은 source 원장 v14, 입력 v14, 기대 행 v7, 집중 Edit v6·Play v4·Hub v3을 정확 SHA로 결속한다. 나머지 여섯 선택과 9회 순서·예상 수 `139/32/5/91/149/15/562/51/610`, 필수 행과 QA 규칙은 그대로다. 9개 선택 모두의 계획 SHA·소스/열거형 실물 SHA·이름 수·배열·정확 선택식, 모든 예정 출력의 유일성과 부재, 새 계획·문서의 직접 결속을 실행 전에 검사한다. 이미 존재하는 출력은 재사용하지 않고 필요한 stem을 새로 발급한다. 큐 결과는 별도 `CreateNew` 경로에 기록하며 원시 종료·QA 반환·행·입력 전후를 남긴다.

루나는 두 생성 증거의 실제 실행과 전체 차이, v14 원장·입력의 모든 보존/증분, 계획 v10의 9개 live pin과 출력 부재를 독립 정적 검수한다. 그 뒤에만 최종 큐를 실행한다. 앞선 Play R3의 32/32 별도 진단은 외부 반환 JSON의 독립 증거가 없으므로 최종 수용을 대체하지 않는다. 실패한 실행과 모든 원본 증거는 보존하고 새 결함은 별도 분류한다.
