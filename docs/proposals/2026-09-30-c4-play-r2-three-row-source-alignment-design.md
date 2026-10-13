# C4 Play 세 행의 실제 종료·증거 귀속 보정안

- 상태: 제안. 아스트라의 한정 승인 전 구현·실행·수용 권한이 없다.
- 추적: `REQ-M5D7QC4-005/006/007`, `AC-M5D7QC4-007/008/009/010`.
- 원본 실패: `artifacts/c4-final-validation-queue-result-v10.json` SHA-256 `741A087D1FD38A2AAB03E9C9A3F608116C72A6623014EC974A8F92E8CC87BB56`은 첫 Edit 139/139 및 행 148/148 검증 뒤 `c4-r2-focused-play`에서 멈췄다. Play XML은 32/32 통과, 실제 native·QA·바깥 종료는 모두 0, 입력 1052개 전후 차이는 0이다. 행 비교는 35개 planned/reached/passed, failed 0, `SourceMatched=true`, `EvidenceMatched=false`이며 아래 세 ID만 불일치다. 기존 XML·행 비교·결과와 첫 Edit 실행 출력은 역사로 보존한다.

## 세 행의 실제 의미

| 행 | 현재 기대·실제 차이 | 제한 보정 근거 |
| --- | --- | --- |
| `play_AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal` | `ExpectedGuard.permanentFaultRecorded=No`와 canonical `RequiredFacts` 같은 값 `No`; 실제 두 값 `Yes` | 실제 C2 Busy는 C4에서 `ReloadRequired/C2Invoked`로 terminal 종료된다. `ProfileResetExecutionBridgeV1.cs:311,533-544`의 원래 쌍 종료와 Adapter/Router `ApplyExecutionGuard(Close)`의 비완료 lifecycle latch가 `RecordExecutionLifecycleFault`→`RecordExecutionFault`에 도달한다. fixture `ObserveActualGuard:800-802`는 원본 `FaultHistory`·`Head/Failure`로 `Yes`를 읽는다. 영수증 없음·장벽 유지·가드 폐쇄·fresh 금지는 유지된다. |
| `play_AC007_ActualC2UnsafeLockPathManualRepairIsTerminal` | 위와 같은 예상 `No` 두 곳, 실제 `Yes` | 실제 C2 `ManualRepairRequired/C2Invoked`도 같은 비완료 terminal 경로다. 승인 C4 AC007의 “DiskPrepared 이후 모든 비완료는 terminal”과 일치하며 `No`로 바꾸어 제품 fault 기록을 지우지 않는다. |
| `play_AC008_ActualFaultRecordedBeforeNativeDisableMakesCancelReentryInert` | `RequiredFacts.authorityCorrelation.receipt`의 값은 양쪽 `Absent`, 기대 확실성 `Observed`, 실제 `SourceEstablished` | `ProfileResetExecutionBridgePlayModeTests.cs:194-230`은 `AfterRouterGuard`에서 실제 예외·동일 재진입 거절을 확인하고 `ObservePreC1Failure`를 호출한다. fixture `C4ActualExecutionPlayFixtureV1.cs:493-513`은 실제 `_resetReceipt=null`을 확인한 뒤 C1 이전·C2 미호출 소스 경계와 함께 영수증 부재를 `SourceEstablished`로 기록한다. 이는 실제 발급되지 않은 영수증의 구조적 부재 근거이며 새 영수증이나 정상 권한 관측을 주장하지 않는다. |

QA r2 규약은 `ExpectedGuard`와 canonical `RequiredFacts`를 실제 nested 값·사실과 각각 정확 대조한다. 따라서 첫 두 행은 각각 `ExpectedGuard.permanentFaultRecorded`와 `RequiredFacts[guard.permanentFaultRecorded].ExpectedValue`를 **함께 `No→Yes`**, 세 번째 행은 `RequiredFacts[authorityCorrelation.receipt].RequiredCertainty`만 **`Observed→SourceEstablished`**로 바꾼다. 총 세 행·다섯 leaf만 변경하고 나머지 185행의 내용·ID·`Order`, 35개 Play 행 ID, 32개 시험 이름·순서·기대 행동은 불변이다. 제품 fault를 정상화하거나 검증기 확실성 판정을 완화하지 않는다.

## 권한 범위와 비재귀 동결

`artifacts/c4-build-required-checkpoint-rows.ps1:10`은 Edit/Play/Hub 시험 소스의 `ExpectedRowsJson`을 직접 파싱해 행 원장을 만든다. 따라서 행 v9만 별도로 고치고 Play fixture를 불변으로 두면 원천 정의와 어긋난다. 승인할 수 있는 최소 시험 소스 변경은 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`의 **위 세 행·다섯 leaf를 포함한 `ExpectedRowsJson`**과 현재 자기 결속 두 곳뿐이다. 이 fixture의 동결 원장 참조 전체는 일반 저장소 역사 검사 v1(`:828`)과 현재 포인터 검사 v12(`:884`), NoOp 일곱 호출 토큰 검사 v12(`:979`)의 세 곳이다. 일반 저장소 v1은 유지하고 뒤 두 참조만 최종 원장 v17로 옮긴다. 포인터의 정확 선언·본문·같은 사례 비저장 확인과 NoOp의 Bridge 네 토큰·Owner 세 토큰 각각 한 번 검사는 그대로 유지한다. Edit fixture는 자체 현재 원장 v16을 계속 읽고, v17에서도 같은 실제 SHA로 대조한다. Play 부모 소스·정식 32개 이름·선택 v4의 `SourceFiles`와 선택식은 불변이다.

Play fixture 편집을 끝낸 뒤 최종 바이트 SHA를 포함하는 `artifacts/c4-frozen-source-manifest-v17.json`을 새로 발급한다. fixture는 v17의 **경로 문자열만** 포함하므로 원장은 그 최종 fixture 바이트를 비재귀적으로 고정한다. 이전 v12·v16 원장과 원래 fixture 바이트는 역사로 보존한다. 새 `artifacts/c4-required-checkpoint-rows-v9.json`은 승인된 기존 행 생성기를 현재 최종 Edit/Play/Hub 소스와 별도의 안전한 동일 상대경로 임시 공간에서 실제 실행해 발급한다. 필요한 모든 소스·도구를 실제 동일 SHA로 복사하고 생성기의 고정 v1 출력이 그 공간에서 **사전 부재**임을 확인한 뒤 정확히 한 번 실행한다. 임시 생성 v1의 SHA와 새 rows v9의 전체 바이트가 같아야 하며 저장소의 역사 rows v1 전후 SHA는 불변이어야 한다. v8과 전체 구조를 대조하여 위 다섯 leaf 및 `SourceFiles`의 Play fixture SHA 한 값 외 차이가 있으면 실패한다. 새 행의 `Count=188`, 원본 순서·ID·수용 기준, Edit fixture `SourceFiles` SHA를 보존한다. 정확 명령·작업 디렉터리·실제 종료코드 0, 복사 소스·도구 SHA, 임시 v1 사전 부재와 생성물 SHA, v9 바이트 동일성, v8 대비 유일한 여섯 차이, 역사 v1 전후 SHA 및 표준 출력·오류 출력의 요약을 **새 `artifacts/c4-rows-v9-builder-evidence.json` 한 파일에 `CreateNew`로** 기록한다. `artifacts/c4-*.log`는 입력으로 금지되므로 별도 생성 로그 파일을 만들지 않는다.

행 v9와 위 생성 증거 JSON을 모두 `CreateNew`로 발급한 **다음** v17 소스 원장을 만든다. v17은 변경 Play fixture, 행 v9, `artifacts/c4-rows-v9-builder-evidence.json`의 정확 경로·실제 SHA를 `Evidence`와 한정 `AllowedChanges`에 결속하고, 이 제안·루나 독립 검토·새 Approved 계약의 실제 SHA와 승인 사유도 정확 결속한다. 새 입력 v17은 기존 입력 경로를 보존하면서 v17 원장·행 v9·생성 증거 JSON·세 문서를 직접 포함하고 전후 캡처로 각 SHA를 대조하며 새 도구 바이트가 있으면 함께 고정한다. `artifacts/c4-final-validation-queue.ps1`의 결과 허용 경로 한 줄만 `artifacts/c4-final-validation-queue-result-v11.json`로 옮기고 실제 SHA를 원장·입력·계획에 반영한다. 계획 v13의 첫 Edit와 두 번째 Play는 이미 발급된 v12 계획의 출력 8개·7개와 충돌하지 않는 **각각 새 stem 및 해당 여덟 출력 경로**를 쓴다. 나머지 일곱 실행의 출력은 현재 부재하나 실행 직전 다시 확인하며 하나라도 생기면 새 계획을 발급한다. 아홉 선택과 180초 제한·기대 이름·188행 의미는 유지한다.

검증 순서는 제품 terminal/fault 경로와 receipt 부재 귀속 독립 정적 검수 → Play fixture 세 행·다섯 leaf 및 두 v17 참조만 변경됐는지 대조 → 실제 행 생성과 v8 대비 전체 구조 차이 검사 → v17 소스·입력·계획 및 아홉 선택 실물 지문 검사 → 새 출력 부재 확인 → 실제 native/QA/바깥 종료, 35개 Play 행과 188개 전체 기대, 입력 전후 동일성 및 루나 독립 검수다. 제품 소스·Edit fixture·시험 부모·선택·캡처·검증 도구의 판정은 변경하지 않는다. 새 실행이 다른 불일치를 드러내면 별도로 분류하고 멈춘다.
