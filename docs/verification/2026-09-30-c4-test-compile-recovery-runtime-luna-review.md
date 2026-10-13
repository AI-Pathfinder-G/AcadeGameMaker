# C4 시험 컴파일 복구 정적 검수

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-test-compile-recovery-amendment.md` — SHA-256 `EF6B0966836FEA1896AF8BA8313D052F8C24E43F93DCD92D27B392377399B4CB`.
- 검수 대상 SHA-256: Edit fixture `0900831635B02D1F9CE0177A9FA8587C8CFFA2CD3E2D984B394825FA604CDB89`, Edit 시험 `9F051A4A1F5BE4418CF1BD6E9CA1477B9C3933AEB645FF7FABEC4F655EB6F17E`, Play fixture `23E91AF7F4DB54A2937E51E4BC1FEDDE677C702477591115D9562CF05B7A29B2`, Play 시험 `1CDA66FB944D17489D6003AED24FAB665E4979C74ABAD242D81C7914133DDAA6`, 큐 `FCF914CAAAC1DB7D50B2CD944E58514C0FBDCE83F1E8AB46E673CD00AE293476`.
- 대조한 규범: Approved QA r2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, Approved C4 r4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- 판정 범위: 정적 코드 검수만. Unity·컴파일·큐 실행은 하지 않았다. P0 0건, P1 0건.

네 대상 파일의 지문은 요청된 값과 일치한다. Edit 및 Play fixture는 기존 `AcadeGameMaker.Input.GameInputActions` 이름공간을 가져온다. Edit fixture의 원인 검사는 세 기존 값 `WrongThread`, `IssuerThreadAnchor`, `IssuerThreadAtIssue`를 모두 유지한다. Edit/Play 시험의 두 결과 검사는 각각 원래 두 열거형 값만 허용한다. Edit fixture의 `HasObserved`는 사전에서 받은 `object`가 기대한 `Dictionary<string,object>`일 때만 후속 값과 관측 확실성을 검사하고, 타입이 다르면 거짓을 반환해 닫힌다.

Play fixture의 `C4_EXPECTED_ROWS_BEGIN/END`와 188행 공유 `ExpectedRowsJson`은 소스에 유지되어 있고 검수 과정에서 수정되지 않았다. 시험 메서드·사례 이름과 매개변수도 새 보정으로 바뀌지 않았다. 이 정적 확인은 앞선 실패 로그의 컴파일 원인을 보정하는 데 필요한 범위와 맞지만, 새 컴파일 성공이나 테스트 결과를 증명하지 않는다.

큐 변경은 고정 결과 경로를 `artifacts/c4-final-validation-queue-result-v4.json`으로 한정한다. 계획 바인딩 확인, 동결 소스 검사, 단일 프로젝트 잠금, 하위 종료값/표준오류 수집 및 실패 시 중단 흐름이 남아 있다. 따라서 이전 v1/v2/v3 결과와 실패 실행 증거를 덮어쓰지 않는 설계다. 새 선택·행 원장·manifest·입력 목록·plan의 지문 및 실제 Unity 재실행은 이번 검수 범위 밖이며, 새 실행 수용은 아직 주장할 수 없다.
