# C4 Play 집중 실행 13건의 제한 복구 설계

- 상태: 제안. 이 문서는 구현·계약 승인·실행 수용이 아니다.
- 원시 증거: `artifacts/c4-focused-play.xml` SHA-256 `92B143BEC6A26D89169CB07A5E1D8F9396362A3E647ED38EF532011CC92A0D7E`의 32건 중 통과 19건, 실패 13건. 앞선 Edit 실행의 통과는 이 Play 실행을 통과로 바꾸지 않는다.
- 기준: 승인된 `docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md` SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md` SHA-256 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`.

## 실패의 정확 분리

| 묶음 | 수 | 실제 경계와 판단 |
| --- | ---: | --- |
| `Expected -1 / 843` | 11 | `C4ActualExecutionPlayFixtureV1.cs:899–903`의 부분 문자열 검색이 `ObserveActualBarrier`의 3인자 전달 오버로드(844행)와 4인자 실제 관찰 오버로드(846행)를 모두 찾는다. 이미 바이트 지문 확인 뒤 발생한 시험 포인터 결함이다. 결과·장벽 검증을 완화하지 않는다. |
| 반복 취소의 `DecisionRequired` 기대, 실제 `Confirmed` | 1 | `ProfileResetExecutionBridgePlayModeTests.cs:247,273`은 기본 `profile.json` 하나로 만든 `Create()`를 사용한다. `C4ActualExecutionPlayFixtureV1.cs:108–114,146–150`의 기본 세 leaf 설정은 요청되지 않았다. C3 분류(`ProfileNewGameConfirmationV1.cs:442–452`)는 현재 단일 기본 leaf를 `NoConfirmationRequired`로 보고, Owner `InspectInitial`(247–265)은 실제 `Confirmed`를 반환한다. 프롬프트가 필요한 시험 자료 설정 오류이며 취소 요구나 제품 결과를 바꾸지 않는다. |
| fresh 결과 뒤 실제 비활성화에서 Adapter terminal 기대, 실제 false | 1 | `ProfileResetExecutionBridgePlayModeTests.cs:135–165`의 fresh=true 행은 실제 Busy 결과와 Router 비활성화를 확인한 뒤 `Adapter.IsResetCutoverTerminal`에서 실패했다. Router `OnDisable`(1196–1214)은 lower 오류 사건만 기록한 다음 C2 terminal이 아직 없으면 Hub 단독 `LatchFault/CloseActionsOnce`로 진행한다. lower `RecordExecutionLifecycleFault`(`ProfileResetExecutionBridgeV1.cs:371–380`)도 관리 오류 기록에서 멈춘다. 실제 원래 pair의 Adapter closure 부재는 REQ-M5D7QC4-006 / AC-M5D7QC4-008의 양측 영구 폐쇄 결손 후보이며 시험 기대를 false로 낮출 수 없다. |

첫 두 묶음은 서로 다른 시험 설정·포인터 오류다. 세 번째는 앞 두 실패를 통과시켜도 남는 별도 제품 전이 결손이다. 현재 XML은 정확한 제품 보정 후 결과를 증명하지 않는다.

## 한정 보정 계약 후보

1. Play fixture의 포인터 찾기는 **4인자** `ObserveActualBarrier(EvidenceRow, ProfileResetExecutionResultV1, RootLeaseWorker, ConfirmedProfileResetRequestV1)` 선언 하나와 그 본문으로 한정한다. 부모 사례 선언행도 문자열 리터럴 포함 검색 대신 실제 선언 시작을 고정한다. 원래 소스 SHA·정확 선언행·읽기행 결속과 `NoSameCaseSaveObservation`을 모두 유지한다. 이 단계의 허용 후보는 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`뿐이다.
2. 반복 취소 부모 사례 한 건은 `Create(true,true)`로 **처음부터 실제 기본 세 leaf**를 둔다. 초기 fixture가 실제 캡처·확인·커밋을 정상 수행한 뒤 C1 Busy·fresh 경로에 들어간다. 이전 이력·첫 실제 커서 프레임 폐기·새 불투명 요청·취소/재무장 세대·마지막 실제 C1/C2 검증 및 정확 `DecisionRequired` 기대를 유지한다. `Create(false,true)`는 커밋을 생략하므로 허용하지 않는다. 허용 후보는 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs`의 해당 초기화 한 곳뿐이다.
3. fresh=true 비활성화는 새 제품 콜백이나 권한 없이 이미 발급된 lower `ExecutionRecord`의 원래 pair로 닫는다. `RecordExecutionLifecycleFault`는 현재와 같이 `GuardedAdapters` 또는 `GuardedRouters`의 실제 등록 참조와 `Thread.CurrentThread == e.Thread`를 먼저 확인하고, `e.Gate` 안에서 영구 오류 사건을 기록한다. **잠금을 푼 뒤** `!e.NativeClosed`인 실제 pair에 기존 `CloseOriginalExecution(e)`을 실행한다. 이 함수는 `NativeFaultClosed`/`NativeClosed`/`GuardOperation=Close`를 외부 `ApplyExecutionGuard` 전에 기록하고 Adapter와 Router 양쪽에 `Close`를 적용한다(`ProfileResetExecutionBridgeV1.cs:527–543`). 따라서 `LatchResetCutoverFailure`의 재귀 오류 통지는 같은 원장에 기록만 하고 추가 native 처분을 시작하지 않아야 한다. 원래 Unity 스레드가 아닌 경우는 현행 예외 거절과 native 0회를 유지한다. 새 세션, C1/C2 호출, proof 소비, handback 재발행, 조립·공개 API 변화는 금지한다.

세 번째 후보의 허용 파일은 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`와 위 Play 시험 파일의 정확 관련 행뿐이다. `InputRouter.cs`의 일반 `OnDisable`, generated `GameInputActions.cs`, Adapter·C1·C2·Q-A/Q-B·자산·설정은 이 복구에 변경하지 않는다. lower 이력의 영구 오류를 native보다 먼저 기록하고, Busy 결과의 forensic `Validate`와 실패한 fresh 재사용 거절을 검증해야 한다. `ExecuteConfirmedReset`의 기존 `catch { FaultOriginalExecution(execution); throw; }`는 정상 closure에서 원래 예외를 재던진다. closure 자체의 예외는 성공으로 삼키지 않으며 별도 cleanup 실패로 남긴다. 재진입·부분 가드·이미 native 종료된 C2 완료 행에서 이 규칙이 원래 예외와 양측 폐쇄를 보존하는지는 구현 전 독립 검수와 제한 실행으로 확인할 기술 조건이다.

새 승인 후 세 소스의 정확 SHA를 동결하고, 이름·순서·개수가 같은 Play 선택과 예상 행의 SourceFiles, C4 소스 원장과 입력 목록을 새 버전으로 발행한다. 옛 실행 XML·원장과 C3/Q0 역사 지문은 보존한다. 11 포인터 행, 반복 취소 1행, fresh 비활성화 1행의 실제 재실행에서 원래 예외·양측 terminal·권한 비재사용·C1/C2 단일 호출·cleanup·영수증과 이력을 모두 대조한 뒤 나머지 필수 선택으로 진행한다. 실제 실행 전에는 13건의 통과나 C4 전체 수용을 주장하지 않는다.
