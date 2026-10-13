# C4 Play 단일 호출 검사의 현재 소스 결속 보정안

- 상태: 제안. 구현·실행·수용 권한이 아니다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`. 기존 C4 Play 사례의 `AC-M5D7QC4-001..008` 증거 의미도 유지한다.
- 실제 자료: `artifacts/c4-play-r2-diagnostic.xml`은 Play 32건 모두 실패했다. 공통 선행 실패는 `C4ActualExecutionPlayFixtureV1.cs:991`의 소스 지문 검사로, 예상 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`, 실제 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. 따라서 뒤의 제품 행이 통과하거나 실패했는지는 이 진단으로 판정하지 않는다.

## 정확한 차이

`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs:976-1003`의 `AssertFrozenNoOpSingleCallSource`는 역사 원장 `artifacts/c4-frozen-source-manifest-v1.json`을 읽고 Bridge와 Owner의 옛 지문을 각각 고정한다. 현재 실물과 `artifacts/c4-frozen-source-manifest-v11.json`의 정확한 `Runtime` 결속은 아래와 같다.

| 소스 | v1 | 현재 실물·v11 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` | `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB` | `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7` | 같은 지문 |

같은 fixture의 `FrozenNoSameCaseSavePointers`는 이미 v11을 읽고 부모·fixture 실물 지문을 확인한다(`:879-922`). `AssertFrozenOrdinaryWriterSource`의 v1 결속(`:821-849`)은 변경되지 않은 두 Profile 소스를 대상으로 하는 별도 역사 검사다. 이 제안은 그 검사를 이동하거나 느슨하게 하지 않는다.

## 한정 보정 계약 후보

1. 변경 가능한 제품·시험 소스는 **Play fixture 한 파일**이다. `AssertFrozenNoOpSingleCallSource`가 최종 현재 원장 한 개에서 두 `Runtime` 경로를 각각 정확히 한 번 찾아 `Kind`, 원장 지문, 실제 파일 바이트 지문을 모두 대조하게 한다. 현 원장 v11은 두 런타임 지문과 일치하지만 fixture 수정 뒤 자기 지문이 달라지므로, 최종 실행에는 새 후속 원장 v12를 발행하고 이 함수와 `FrozenNoSameCaseSavePointers`의 현재 fixture 결속을 함께 v12로 맞춘다. v1·v11 원본은 수정하지 않는다. 후속 원장은 fixture 편집 종료 후 그 최종 바이트 지문을 담아 비재귀적으로 동결한다.
2. `AssertFrozenNoOpSingleCallSource`의 `return ExecuteConfirmedReset(...NoOp...)` 정확 포함 검사와 Bridge의 C1·C2·두 관찰 호출 토큰 각각 **정확 한 번**, Owner의 실행·fresh reserve·fresh complete 호출 토큰 각각 **정확 한 번** 검사를 그대로 유지한다. 지문 비교 삭제, 부분 문자열 지문 허용, 단일 호출 수 완화, 검사 건너뛰기는 허용하지 않는다. 해당 토큰이 현재 실제 소스와 달라졌다면 별도 설계·승인 없이 검사를 고쳐 통과시키지 않는다.
3. 신규 증거 경로는 `artifacts/c4-frozen-source-manifest-v12.json`, 이어지는 현재 입력 경로·캡처·실행 계획·결과의 새 버전으로 한정한다. `artifacts/c4-input-path-list-v1.json` 및 현재 `artifacts/c4-frozen-input-paths-v11.json`에는 fixture와 역사·현재 원장이 입력으로 잡혀 있다. 후속 입력 목록에는 v12를 더하고, 기존 경로·1017개 입력의 동일성 주장과 새 목록의 개수는 구분한다. 필요하면 목록·캡처·계획의 새 버전을 발행하되 원본 및 실행 출력을 덮어쓰지 않는다. 시험 부모·선택의 32개 정확 이름과 기대 행·의미는 바꾸지 않는다.

검증 순서는 (가) 수정 전 두 실물 SHA와 v11 결속 재확인, (나) fixture의 두 현재 원장 참조와 단일 호출 토큰 정적 검토, (다) fixture 최종 SHA를 포함한 v12 및 새 입력·선택·기대 행의 원장 지문 동결, (라) 입력 전후 동일성·32개 이름·실제 종료코드·전체 행을 갖춘 새 Play 진단, (마) 루나 독립 검수 후 기존 필수 선택을 포함한 승인된 최종 큐 재실행이다. 현재 0/32와 과거 19/32는 역사로 남긴다. 제품 소스, Q0, C1/C2, 생성 입력, QA 판정 규칙, 원래 시험 이름·기대값을 이 보정으로 변경하지 않는다.
