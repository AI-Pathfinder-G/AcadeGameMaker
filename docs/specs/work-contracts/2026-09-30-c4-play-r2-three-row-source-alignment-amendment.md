# C4 집중 Play 세 행 출처 정렬 한정 계약

- 상태: **Approved — 아래 시험 기대 자료·증거 결속·재실행만 승인; C4 전체 수용은 실제 결과 검수 전까지 보류**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-005/006/007`, `AC-M5D7QC4-007/008/009/010`. 기존 승인 C4 r4 및 QA r2의 행동·엄격 비교 규칙을 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-play-r2-three-row-source-alignment-design.md`, SHA-256 `AE721318D3A67A983C08594468A6A11B01A790F07DDBDBB2B1D2E7840608361F`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-play-r2-three-row-source-alignment-luna-design-review.md`, SHA-256 `B8C2DB5E57A3FFB479C6D84420D2A1CCDF11509BD14DAF22A3298FFA97E9F942`, P0/P1=0/0.
- 원본 결과: `artifacts/c4-final-validation-queue-result-v10.json`, SHA-256 `741A087D1FD38A2AAB03E9C9A3F608116C72A6623014EC974A8F92E8CC87BB56`. 첫 Edit 139/139 및 행 148/148 통과 후 Play XML 32/32 통과, native/QA/바깥 종료 0, 입력 1052개 전후 동일. Play 필수 행 35개 중 세 행의 기대 증거 불일치로 fail-stop했다. 원본 XML·행 비교·캡처·결과와 다른 역사 자료는 수정하지 않는다.

## 승인 범위

1. `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`의 `ExpectedRowsJson`에서 `play_AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal` 및 `play_AC007_ActualC2UnsafeLockPathManualRepairIsTerminal` 각각 `ExpectedGuard.permanentFaultRecorded`와 `RequiredFacts[guard.permanentFaultRecorded].ExpectedValue`를 `No`에서 `Yes`로 바꾼다. 두 실제 비완료 terminal 경로의 원본 fault 이력과 일치하도록 하는 정렬이다. `play_AC008_ActualFaultRecordedBeforeNativeDisableMakesCancelReentryInert`의 `RequiredFacts[authorityCorrelation.receipt].RequiredCertainty`만 `Observed`에서 `SourceEstablished`로 바꾼다. 값 `Absent`와 실제 pre-C1 결과·영수증 미발급 경계는 유지한다. 정확히 세 행·다섯 leaf 외 185행의 내용, 행 ID·순서·32개 시험 이름·행동·관측 코드를 바꾸지 않는다.
2. 같은 Play fixture의 현재 자기 출처·NoOp 토큰 출처 검사 두 곳만 v12에서 새 소스 원장 v17 참조로 옮긴다. 일반 저장소 역사 검사 v1, 정확 포인터/일곱 토큰 검사, Edit fixture 자체 v16 참조는 유지한다. 제품 Runtime·부모 시험·선택·검증기·캡처기·QA 판정은 수정하지 않는다.
3. 승인된 원본 `artifacts/c4-build-required-checkpoint-rows.ps1`을 동일 상대경로로 복사한 안전한 임시 공간에서 고정 v1 출력 사전 부재를 확인하고 정확 한 번 실행한다. 소스·도구 실제 SHA, 명령·작업 경로·종료 0, 임시 v1 생성물과 새 `artifacts/c4-required-checkpoint-rows-v9.json`의 전체 바이트 동일성, 역사 rows v1의 전후 SHA 동일성, v8 대비 위 다섯 leaf 및 Play fixture `SourceFiles` SHA 한 값 외 차이 0을 검증한다. 188행·원래 ID·순서를 보존한다. 재현 사실을 새 `artifacts/c4-rows-v9-builder-evidence.json`에 `CreateNew` 기록하고 별도 `artifacts/c4-*.log`를 입력으로 추가하지 않는다.
4. 최종 fixture·행 v9·생성 증거 JSON 뒤 `artifacts/c4-frozen-source-manifest-v17.json`과 `artifacts/c4-frozen-input-paths-v17.json`을 `CreateNew` 발급한다. v16의 기존 경로·사유를 보존하면서 변경된 실제 SHA, 본 계약·설계·독립 검수·새 증거를 정확 경로와 SHA로 결속한다. 입력의 모든 직접 파일은 실행 전후 캡처 대상이다. 이전 원장·입력은 역사로 둔다.
5. `artifacts/c4-final-validation-queue.ps1`의 결과 허용 경로 한 줄만 v10에서 `artifacts/c4-final-validation-queue-result-v11.json`로 바꾸며 실제 도구 SHA를 v17에 결속한다. 최종 계획 v13은 9회 선택·순서·시간 제한·필수 행을 보존한다. 기존 출력과 충돌하는 첫 Edit와 두 번째 Play의 stem 및 각 출력 경로를 새 이름으로 발급하고, 나머지 일곱 출력도 실행 직전 부재를 확인한다. 결과 v11과 모든 출력은 `CreateNew`다.

## 수용 조건

- `AC-M5D7QC4-007/008`: 세 실제 Play 관측이 수정한 다섯 leaf와 일치하며 receipt 없음, terminal 가드, 영구 fault 귀속, pre-C1 부재 귀속 및 재진입 금지 의미를 유지한다.
- `AC-M5D7QC4-009/010`: 루나가 fixture/도구의 한정 변경, 생성기 재현, v17 동결과 v13 계획의 실제 지문·출력 부재를 독립 검수한 뒤 큐를 실행한다. 각 실행의 정확 선택·전체 필수 행·원시/QA/바깥 종료·입력 전후 동일성과 9회 전체 fail-stop 결과로만 수용한다. 새 불일치는 원본을 보존하고 별도 분류한다.
