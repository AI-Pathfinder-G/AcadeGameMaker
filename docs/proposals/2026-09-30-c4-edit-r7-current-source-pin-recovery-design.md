# C4 집중 Edit 첫 실행의 현재 소스 지문 복구 제안

- 상태: 제안. 아스트라의 한정 승인 전 구현·실행·수용 권한이 없다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`; 기존 `AC-M5D7QC4-001..008` 행동·증거 의미는 유지한다.
- 실제 실패: `artifacts/c4-final-validation-queue-result-v9.json` SHA-256 `DD5B6E5F348A6CF05DCEEA116E745AD705725086CB59968F949BD20CDC7D47DF`는 `Runs=[]`, 중단점 `c4-r7-focused-edit`다. 해당 XML은 139건 중 5건 통과·134건 실패, 실제 Unity 종료 2, 입력 v15의 1044개 전후 차이 0을 보인다. 133건은 Edit fixture `AssertFrozenNoOpSingleCallSource`의 옛 Bridge SHA, 1건은 `AssertFrozenBusySource`의 같은 옛 SHA에서 실패했다. 실패 이후의 제품 결과는 이 XML만으로 판정하지 않는다. 기존 Play R3 32/32 결과도 새 Edit 결과로 대체하지 않는다.

## 실제 fixture 참조 범위

`Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`의 동결 원장 참조는 다섯 곳이다. `AssertFrozenBusySource`의 `:837`은 v1과 Bridge 예상 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`를 쓴다(`:846`). `AssertFrozenNoOpSingleCallSource`의 `:1332-1336`도 v1과 같은 옛 Bridge SHA를 쓴다. `FrozenNoSameCaseSavePointers`와 부모/원본 관찰 함수의 `:1196,1372`는 fixture 자신의 현재 바이트를 검사하며 v12를 읽는다. 나머지 `AssertFrozenOrdinaryWriterSource`의 `:1135`는 현재도 변하지 않은 Profile 두 소스에 대한 별도 v1 역사 결속이다. 파일 전체 원장 경로 검색 결과 이 다섯 곳 외의 숨은 원장 리터럴은 없다.

현재 `ProfileResetExecutionBridgeV1.cs` 실제 SHA와 현행 원장은 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. `AssertFrozenBusySource`의 나머지 두 실제 Runtime SHA는 `ProfileNewGameResetServiceV1.cs`=`23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`, `ProfileResetDiskTransactionV1.cs`=`BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`다. 단일 호출 검사의 Owner SHA는 `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`로 보존한다. 일반 저장소 검사의 두 실제 SHA도 v1과 같다.

## 승인할 수 있는 최소 수정

1. 바뀌는 시험 소스는 위 **Edit fixture 한 파일**뿐이다. Busy와 단일 호출 검사는 v16 후속 소스 원장을 읽고 각 대상의 정확한 경로 유일성, `Kind=Runtime`, 고정 예상 SHA, 원장 SHA, 실제 파일 바이트 SHA를 모두 대조한다. 두 검사에서 Bridge 예상값만 현행 `52E00...4F32A`로 바꾸고 Busy의 나머지 두 값과 Owner 값을 유지한다. Busy의 `FileShare.None`·Busy 예외·C1 결과·`Begin`→fresh branch→checkpoint 순서 검사와 단일 호출의 NoOp 표현식·Bridge 네 토큰·Owner 세 토큰의 **각각 정확 한 번** 검사는 삭제하거나 완화하지 않는다. `AssertFrozenOrdinaryWriterSource`의 v1 검사와 모든 토큰도 유지한다.
2. 같은 Edit fixture의 `:1196,1372` 현재 부모·fixture 결속 참조를 v12에서 v16으로 옮긴다. 수정 종료 뒤 최종 Edit fixture SHA를 v16 원장에 동결한다. Play fixture는 기존 v12를 계속 읽고 그 실제 SHA를 v16 원장도 동일하게 확인한다. v12/v15 원장·원래 시험·Runtime 소스는 불변 이력이다. fixture의 단순 리터럴 교체 외 문장 추가·삭제로 source pointer 줄이 바뀌면 새 검수 없이 진행하지 않는다.
3. 188행의 값·식별자·`Order`·출처 뜻을 유지한 행 v8은 `SourceFiles`의 Edit fixture SHA **한 값만** 새 실제 값으로 바꾼다. 기존 집중 Edit 선택 v6은 부모 소스·139개 이름·순서·선택식·열거형 현재 SHA가 불변이므로 유지한다. 집중 Hub v3·Play v4와 선행 여섯 선택도 불변이다. v16 소스 원장은 변경 Edit fixture/행 v8의 실제 SHA와 이 설계·독립 검토·새 Approved 계약을 정확 증거로 결속한다. 입력 v16은 이전 허용 입력을 보존하고 v16 원장·행 v8·세 문서를 더해 새 source/input 지문을 발급한다. 새 계획 v12는 그 지문과 현재 도구를 결속하며 아홉 선택·기대값·180초 제한은 유지한다.
4. `artifacts/c4-final-validation-queue.ps1`의 결과 경로 검사 한 줄만 기존 v9 대신 `artifacts/c4-final-validation-queue-result-v10.json`을 허용한다. 도구의 새 실제 SHA는 v16 원장·입력·계획 v12에 고정한다. 계획 첫 Edit 실행은 원래 `c4-r7-focused-edit`의 XML·로그·전후 캡처 등 이미 존재하는 다섯 출력을 보존하도록 **새 stem과 그에 따른 여덟 출력 경로**를 사용한다. 나머지 여덟 실행의 기존 출력은 현재 부재하지만 실행 직전 다시 확인한다. 하나라도 존재하면 별도 새 stem/계획이 필요하며 덮어쓰지 않는다. 결과 v9도 불변으로 남긴다.

검증 순서는 다섯 fixture 원장 참조와 네 변경 위치·유지되는 v1 검사 독립 대조 → 두 런타임 및 fixture 실제 바이트 SHA와 v16 원장 대조 → Busy 경계·NoOp 일곱 토큰의 정적 재검수 → 행 v8의 유일한 Edit SHA 차이와 188행 내용·순서 동일성 확인 → 아홉 선택·도구·출력 부재의 계획 사전 검수 → 입력 전후 동일성, 실제 원시·QA·바깥 종료 및 전체 행의 승인된 최종 큐다. 캡처·검증·QA 판정 도구, 제품 코드·선택·행 기대값은 변경하지 않는다. 새 실패가 드러나면 별도 분류하고 이 사전 지문 실패를 제품 통과로 간주하지 않는다.
