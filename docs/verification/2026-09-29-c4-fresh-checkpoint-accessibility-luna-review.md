# C4 fresh checkpoint 접근성 개정 독립 정적 검토

검토 대상 최종 Draft SHA-256은 `D8E0B90432CE0CE9900AABC281F3DEE05BC12475C11194631D66E7677318C988`이다. Approved 선행은 R4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, QA R2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, 시험 제어 접근성 개정 `c4-edit-control-accessibility-amendment.md`이다. 후자는 실제 구현 승인 상태이며 compile/Unity 미검증이다.

판정: 런타임 개정의 정적 설계 차단 P0 0, P1 0. 초안은 검토 중 제기된 세 연결 결손을 각자의 실제 권한 경계 안에서 해결한다.

첫째, `ConfirmedRequestWitness`는 현재 `InteractionGeneration`/`DecisionGeneration` 투영을 가지고 있고 `RequestLifecycle`은 actual request mint 시점에 같은 두 수를 다시 고정할 위치가 있다. `IssueConfirmedFromFreshCapture`가 witness와 lifecycle을 연속 생성하고 두 CWT에 발행 전 결속한다. 두 readonly 발행값을 lifecycle에서 보존해 전체 검증 때 witness 숫자와 대조한다는 보정은 단일 projection 교체를 실제 mint값으로 검출하며 정상 발행 경로를 새로 만들지 않는다.

둘째, lower의 실제 실행 반환은 `ExecutionResults`와 `FreshHandbacks` CWT에 결속되어 있고, `TryFreshResult`는 동일 결과·Owner·pair·anchor thread·미 fault/미 closed를 확인한다. 제안된 `IsFreshExecutionAvailable`은 이 actual registry에서 Busy/Stale NoBarrier 미예약·미완료 행을 비소비로 확인하는 한정 조회다. 이를 Owner의 최초·진입 직후 사전 검사로 사용하고, true 이후에만 catch 안에서 full validation/reserve를 수행하며, 같은 Owner의 original committed context에서 6인자 시험 lower가 발급한 실제 반환을 허용하는 흐름은 `_executionResult` 필드만을 발급 근거로 삼던 막힘을 해결한다. 이 조회는 새 결과, 보관 setter, 소비·reserve를 발급하지 않는다.

셋째, 정상 Completed C2 이후 lower는 `CloseOriginalExecution`으로 C4 guard를 닫고 Adapter/Router의 `ApplyExecutionGuard(Close)`가 기존 C2 lifecycle과 Completed cohort를 확인한다. `LatchResetCutoverFailure`는 adapter를 Faulted, router를 faulted로 바꾸므로 정상 Completed guard close와 실제 실패·비완료 fault를 구분하라는 개정은 필수이며, R4 typed 결과·receipt를 보존하도록 명시되어 현재 guard 구조와 맞는다.

다섯 fresh checkpoint는 기존 enum 지점과 일치하며, lower의 출처 CWT·reservation 순서와 Owner 실제 Prepare/ack/Commit/consume 호출의 source 지배를 별도 검증한다. 새 bool 조회와 checkpoint 통지 서명은 내부 관리 결속에 한정되고 Hub 역조회·권한 객체 반환·normal issuer를 추가하지 않는다. guarded 역사 getter와 19개 C2 시험 배치는 이미 승인된 경계의 별도 한정 변경으로 명시되어 있다.

근거로 읽은 당시 source 지문은 `ProfileNewGameConfirmationV1.cs` `A8557B9BF28B4922158E9DA36A177A11A6A1B75CD51EF8A372FC5CD2B32906C9`, `ProfileResetExecutionBridgeV1.cs` `E1CDD8AC8B4807BD1DCC1B2AF9CA6739BC75A6CCEF570BC6391A2B3F9E16CEB4`, `NewGameConfirmationOwnerV1.cs` `7B6FD0CD3B4B22B3B751EF5AB9DA9A0BE0A450C12B76099745C31F793EBBA59D`, `DesktopProfileLaunchAdapterV1.cs` `C1336B3185852924D94479ED1D1FF8201EF7C886F929DD7851BC110982712435`, `InputRouter.cs` `CE79DA6CCEB70317F96EFF4F7C3BF6835058656FFC6058EAADB9148586FF0A8F`다.

이것은 수정 초안의 규범 적합성 검토다. 초안은 Approved 전환 전 구현 권한이 없으며 새 C4 source freeze, C4/QA 도구의 exact-row 검증, C# 컴파일 및 Unity 실행은 확인하지 않았다. 따라서 런타임 구현 완료나 시험 통과를 뜻하지 않는다. 소스·QA·Unity·Git을 수정하거나 실행하지 않았다.
