# C4 r3 정확 구현·시험·감사 개정 계약 초안

- 상태: **Draft — 구현·실행 권한 없음**. 날짜 2026-09-29, 솔, 실제 `gpt-6-sol`.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`, 공동 선행 `AC-M5D7QC3-007/008`.
- 원본: [C4 Review 계약](2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md), SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`.
- 설계: [r3](../../proposals/2026-09-29-c4-implementation-contract-ready-r3-draft.md), SHA `F4A6BA518DC75960784AEBC53F77DB0B9107DAA03472D35042BC1D6F9DB75F73`.
- 기술 선택의 [한정 수용](../../approvals/2026-09-29-c4-r3-normative-design-limited-acceptance.md), SHA `178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692`; 루나 규범 설계 SHA `8D1B5B70771F5A9DF2296740976D1A819D081A2193FAC820F3EB82608E501FA5`.

이 문서는 원본과 함께 읽는 정확 개정 **후보**다. 원본·r3·기존 감사·실행 원문은 수정하지 않는다. 현재 C3 필수610과 전체 선행 gate는 미완료이며 C4는 Review다. 아스트라만 C3 독립 선행 수용·아래 잔여 검수 후 별도로 Approved 전환할 수 있다. 이번에 새 계약 문서만 작성했으며 source/QA/Git/network/Unity/compile 변경·실행은 없다. 실제 구현·새 SHA·NUnit count·결과는 아직 없다.

## 정확 runtime 허용 후보

승인 후에도 다음 경로와 명시한 내용만 허용한다. 기존 meta는 수정하지 않는다.

| 경로 | 변경 내용·REQ/AC |
| --- | --- |
| 신규 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` 및 `.cs.meta` | lower internal partial의 executor·actual consume/fault/fresh 원장·typed result·가드 조율·순수 validator·정확 시험 control. REQ001..007/AC001..010 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | partial 선언·기존 실제 issued/Completed 이력에 결속한 C4 소비와 planned birth projection/독립 issuer anchor/issued Thread 참조. 기존 C3 원장·한 번 소비·등록 실패 종료 유지. REQ001/006, AC001/008 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | 원래 launch-established actual pair/root/generation에 등록된 execution/fresh/terminal 가드 절반과 세션 권한 차단. 기존 C2 원본 소유권·receipt 단일 발급 유지. REQ002/004/006, AC002/004/006/008 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 대응 가드·callback/semantic/mode admission 차단·quarantine/OnEnable 재개 방지·원래 스레드 native containment. actions 교체/enable/gameplay 연결은 C2 기존 알고리즘 외 추가하지 않음. REQ002/004/006, AC002/004/006/008 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | 기존 entry가 실제 등록된 정확 guarded pair를 수용하는 최소 분기만. Finalize 알고리즘·lease/proof 재인증·staging/default/memory/barrier/receipt 불변. REQ005, AC005/007 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | 실제 Commit 이후 executor 조율·actual result/fresh 인증·원본 pair closure·fresh 일회용 reciprocal orchestration. REQ001/004/006, AC001/004/008 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | 신규 Hub typed fresh capability/reservation의 private actual CWT·reserve/commit/live 슬롯. 기존 legacy TryTake/최초 taken/issued/epoch/Cancel 역사 유지. 별도 Q-B 개정, REQ004/006, AC004/008/009 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | 같은 actual reservation만 prepare/ack하는 독립 controller/cursor 슬롯과 첫 실제 true frame 폐기. 기존 retained/cursor/transfer/closed/Cancel 역사 유지. 별도 Q-A 개정, REQ004/006, AC004/008/009 |

금지: C1/Profile source·result/proof/issuer 알고리즘, public ABI·asmdef/friend, wrapper·자산/prefab/scene·Packages/ProjectSettings, 정상 identity/root/proof 제조·공개 registry, actual holder, worker의 전체 Profile writer latch, 새 제품 callback/global hook·네트워크. 새 메타 GUID는 승인 후 실제 생성값과 SHA만 동결한다.

## lower 실행·가드·결과 계약

정확 예정 서명은 아래와 같다. 현재 존재하는 API로 표현하지 않는다. 정상 3인자는 fixed NoOp와 기존 두 하위 NoOp만 결속하며 내부 시험 overload만 승인된 control을 전달한다.

```csharp
internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(
    ConfirmedProfileResetRequestV1 confirmed,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router);
internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(
    ConfirmedProfileResetRequestV1 confirmed,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    IProfileResetExecutionTestControlV1 control,
    IProfileResetDiskTestControlV1 diskControl,
    IProfileResetMemoryCutoverControlV1 memoryControl);
internal static void ValidateExecutionDiskRow(ProfileResetDiskResultV1 actual);
internal interface IProfileResetExecutionTestControlV1
{
    void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint);
    void PostDiskPreparedValidatedPreC2();
    void ObserveActualPreparedProofBeforeRowValidation(
        ProfileResetDiskPreparedProofV1 actual);
    void ObserveActualC2ResultBeforeComposition(
        ProfileResetMemoryResultV1 actual);
}
```

실제 C3 Commit→별도 C4 actual consume→양측 guard→실제 C1 Begin 1회→typed row 전체 검증→DiskPrepared일 때만 실제 C2 Finalize 1회→typed composition 순서다. C3 Completed는 C4 Consumed와 다르다. 현재 하위 API는 C1 `Begin(string, ProfileResetConfirmationIdentityV1)`과 control을 받는 3인자 overload(660/664행), C2 `FinalizeReset(string, ProfileResetDiskPreparedProofV1, DesktopProfileLaunchAdapterV1, InputRouter)`와 control을 받는 5인자 overload(97/133행)다. 변경하지 않는다.

가드는 해당 old session의 메뉴/notification/ordinary-save 권한·semantic/callback/메모리 생산을 차단하며 durable barrier를 주장하지 않는다. 독립 Profile 공개 Save 163~183행은 root lease/barrier 경계다. 현재 owned HubUI 활성 세션의 ordinary-save publisher는 runtime 참조에서 확인하지 못했으며 합성 publisher로 채우지 않는다. 실제 C1 Busy/ConfirmationStale의 전체 행이 NoBarrier일 때만 fresh를 허용한다. 실제 DiskPrepared 이후 Busy/ManualRepair/Reload/throw는 양측 영구 terminal이며 Cancel/rearm/fresh를 금지한다. Completed만 동일 actual C2 receipt를 보유한다. 새 결과/handback은 실제 전체 typed row·원본 pair/C4 소비 사건에 private 등록하고 scalar로 재구성하지 않는다. identity/root/proof getter를 추가하지 않는다.

### 스레드 문맥과 불가역 오류

현재 lower 69~78/118~125/301~309/350~383행의 CWT/history에는 Thread가 없다. planned readonly `ConfirmedRequestWitness.BirthThreadProjection`, 실제 원본 RequestLifecycle에 결속된 private readonly `IssuerThreadAnchor`, 실제 RequestIssued 사건의 readonly `IssuerThreadAtIssue`만 발급 당시 `Thread.CurrentThread` 참조로 결속한다. projection과 독립 anchor/issued 비교를 구분한다. 숫자 Thread ID·caller Thread token·새 issuer는 금지한다.

관리 참조의 원본 CWT/issued 조회와 anchor=issued Thread 확인이 먼저다. anchor를 확증하지 못하거나 원래 thread 밖의 최초 호출이면 payload/body/projection/Unity/native 검사·consume/C1/C2 전에 무소비·무변경 프로토콜 거절한다. payload 정상/전체 검증 완료/cleanup 완료로 기록하지 않는다. late consumed/fault도 C4 관리 사건으로 먼저 거절한다. 원래 thread만 complete invariant/pair/root/generation과 단일 projection 손상을 검증하고, fault 사건을 native Disable 전에 private 원장에 영구 기록한 뒤 기존 양측 terminal로 처리한다. consume/guard 이후 모든 예외는 원래 Unity thread에서 permanent fail-stop, finally gate 해제를 유지한다. worker에서 기존 Adapter 589→Router 482~515의 Disable/Dispose 경로를 호출하지 않는다.

단일 projection Thread 손상은 trusted anchor가 정상일 때 원래 thread에서 검출·terminal 처리한다. 단일 anchor/event Thread 손상은 trusted 확인 불가로 원래 thread에서도 관리 거절한다. 기존 State 손상은 원래 CWT/append history 대조와 원래 thread의 fail-closed를 유지한다. 모든 private 필드 동시 공격은 범위 밖이다. unsupported thread에서 미검사 payload 손상을 검출했다고 주장하지 않는다. 이것이 한정 수용된 AC001/008 문맥 규범이며 일반 반사 손상을 무조건 정상화하는 허용이 아니다.

### A/B의 정확 시험 권한

A `PostDiskPreparedValidatedPreC2`는 실제 C1 row 전체 검증·양측 guard 뒤/C2 앞에서 호출당 한 번만 도달한다. 준비10초·worker 보유20초·finally join10초, 각 NUnit180초다. worker는 실제 temp root의 원래 lease로 C2 actual Acquire Busy를 유발하거나 C1 lease 해제 뒤 실제 `profile.operation.lock` 파일을 삭제하고 같은 경로를 디렉터리로 만들어 ManualRepair를 검증한다. marker fault는 채택하지 않는다. main finally의 release 신호·worker 자신의 finally lease 해제·bounded join을 요구한다. cleanup 실패는 실패이며 가짜 Busy/성공으로 바꾸지 않는다. 원래 Unity thread에서 재개한다.

B outcome/state는 실제 Begin readonly struct의 boxed copy에 단일 필드 default/unknown/부적합 closed pair 음성을 주입해 공유 pure validator로 거절하고, 최종 source의 모든 C4 외부 proof/C2 분기가 성공 검증에 의해 지배되는 증거를 결합한다. 실행 중 저장 struct 변조가 아니다. 실제 C1 row→validator→정상 PreparedProof getter→observer actual nested `_root=null`→validator 재검증→성공 시에만 C2 순서다. getter 자체 Validate(449~454행)를 우회하지 않는다. C2 반환→actual object observer `_outcome=default`→mapping 재검증을 요구한다. 각 observer1회, return/ref/out/supplier/global hook/정상 issuer/registry/복구는 금지한다. getter 이전 fault는 미실시다. C1 내부 validator의 nested proof 읽기와 C4 외부 proof 전달 분기를 구분한다.

## 실제 fresh Q-A/Q-B 예정 API와 원본 이력

아래는 예정 Hub 내부 typed API이며 현재 존재하지 않는다. lower 결과/identity/proof를 Q-A/Q-B에 전달하지 않는다. Owner만 실제 lower handback의 원본 pair/owner/committed epoch/C1 NoBarrier/C4 사건을 인증·reserve한 뒤 getter 없는 Hub capability를 private 등록한다.

```csharp
internal void AcceptFreshExecutionHandback(ProfileResetExecutionResultV1 result);
internal bool IsExecutionFreshInProgress(
    HubNewGameExecutionFreshCapabilityV1 capability,
    HubMenuIntentHandoffOwnerV1 handoffOwner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
internal HubNewGameExecutionFreshReservationV1 ReserveExecutionFresh(
    HubNewGameExecutionFreshCapabilityV1 capability,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
internal void CommitExecutionFresh(
    HubNewGameExecutionFreshReservationV1 reservation,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
internal void PrepareExecutionFresh(
    NewGameConfirmationOwnerV1 owner,
    HubNewGameExecutionFreshReservationV1 reservation,
    object nextEpochToken, long nextEpoch);
internal bool MatchesPreparedExecutionFresh(
    NewGameConfirmationOwnerV1 owner,
    HubNewGameExecutionFreshReservationV1 reservation,
    object nextEpochToken, long nextEpoch);
```

순서는 Owner lower reserve→Q-B reserve→Q-A prepare/actual ack→Q-B commit→Owner consume/lower complete다. Q-B commit은 정확 ack와 실제 미완료 reservation을 요구한다. completed/closed history는 live permission이 아니다. 현재 Cancel/Rearm을 재사용하지 않고 old committed epoch/Q-A retained/cursor/transfer/Q-B taken/issued를 append history로 보존하며 신규 live pristine 슬롯만 생성한다. ready factory여도 첫 actual true frame은 폐기하고 다음 정상 선택부터 take를 허용한다. 부분 실패는 원본 pair와 신규 슬롯 모두 closure, handback 복원 금지다. reciprocal complete와 동일 lower proof 전에는 guard 해제/새 UI semantic 발행을 허용하지 않는다.

lower fresh reserve/complete의 private 구현 상세와 typed execution result의 최종 필드 map은 기존 r3의 원래 authority 범위에서 구현자가 제안하여 독립 검수한다. 다만 Owner가 호출할 새 internal lower 서명/타입이 필요하면 구현 전에 이 Draft를 별도로 보완하고 아스트라 승인받는다. 현재 존재하거나 확정된 API로 꾸미지 않는다. 이 잔여 때문에 fresh 전체 허용이 확정됐다고 선언할 수 없다.

## 신규 시험의 정확 경로·class·meta·REQ/AC

각 신규 `.cs`에는 같은 경로의 `.cs.meta`만 동반한다. 기존 fixture/class에 행·도우미를 추가하지 않는다.

| 신규 경로 | class/namespace/assembly |
| --- | --- |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` | `AcadeGameMaker.Tests.EditMode.InputUnity.ProfileResetExecutionBridgeV1Tests`, `AcadeGameMaker.Input.Unity.EditMode.Tests` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` | same namespace `C4ActualExecutionEditFixtureV1`, same assembly |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs` | `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetExecutionBridgePlayModeTests`, `AcadeGameMaker.Input.Unity.PlayMode.Tests` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` | same namespace `C4ActualExecutionPlayFixtureV1`, same assembly |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` | `AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests`, `AcadeGameMaker.Hub.Presentation.Unity.EditMode.Tests` |

assembly-qualified/declaring type/DeclaredOnly/전체 params-byref-return의 정확 binding으로만 Hub 정상 API를 호출한다. 이름 fallback/Reflection.Emit/DynamicProxy/다른 private fixture factory 결합/정상 proof 반사 제조는 금지한다. temp bytes는 정상 encoder·actual launch/capture/confirm/commit 발급만 사용하고 반사 fault는 승인된 단일 필드 음성에 한정한다. Hub Play friend를 추가하지 않으며 실제 device/UI fixture는 InputUnity Play에 둔다.

아래 REQ/AC 숫자는 각각 `REQ-M5D7QC4-nnn`/`AC-M5D7QC4-nnn`의 전체 식별자를 뜻한다. 다중 번호는 아래의 명시 구분을 따른다.

| REQ/AC | 필수 named 행과 증거 |
| --- | --- |
| 001/001 | `AC001_ActualCommittedOpaqueStartsExactlyOnce`, `AC001_UnregisteredCandidateCannotStart`, `AC001_UncommittedActualRequestCannotStart`, `AC001_ActualForeignRootPairGenerationCannotStart`, `AC001_ConsumedAndLateLoserCannotCloseWinner`, `AC001_SingleRegisteredWitnessDamageClosesWithoutBegin` |
| REQ001 · AC001/008 | `AC001_ActualWrongThreadFirstCallRejectsBeforeUnityAndConsume`, `AC001_OriginalBirthThreadExecutesSameRequestAfterWrongThreadReject`, `AC001_ActualBirthProjectionDamageClosesOnOriginalThread`, `AC001_ActualIssuerThreadAnchorDamageRejectsWithoutConsume`, `AC001_ActualIssuerThreadEventDamageRejectsWithoutConsume`, `AC008_ActualConsumedWorkerRejectsBeforeUnityAccess` |
| REQ002 · AC002/006 | `AC002_ActualBothGuardHalvesBlockOldPublication`, `AC002_ThrowAtGuardBoundaryClosesOriginalPair(checkpoint)`, `AC002_QuarantineReleaseCannotReopenPendingGuard`; Play `AC002_RealEnableAndAfterUpdateCannotReleaseExecutionGuard` |
| 003/003 | `AC003_ActualBeginUsesOriginalIdentityOnce`, `AC003_ActualTerminalDiskRowNeverCallsFinalize`, `AC003_ActualBeginBoxedCopyClosedRowValidatorRejectsDefaultUnknown`, `AC003_ProductionValidatorDominatesEveryProofAndFinalizeBranch`; 순수 음성/구조/full bridge 별도 원장 |
| REQ004 · AC004/008 | `AC004_ActualC1BusyIssuesOneFreshHandback`, `AC004_ActualC1StaleIssuesOneFreshHandback`, `AC004_TerminalDiskRowsNeverIssueHandback`, `AC004_FreshPartialFailureClosesBothSlots(checkpoint)`, `AC004_ConsumedOrForeignHandbackCannotRebind`; Play `AC004_ActualBusyFreshCursorDiscardsFirstSubmitThenTakesNewRequest`, `AC004_ActualStaleFreshCursorDiscardsFirstSubmitThenTakesNewRequest` |
| 005/005 | `AC005_ActualDiskPreparedPassesSameProofRootPairOnce`, `AC005_ActualProofPairRootDamageStopsBeforeCutover`, `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize` |
| REQ002/006 · AC006 | `AC006_ActualC1CheckpointThrowPreservesBarrierAndGuard(diskCheckpoint)`, `AC006_ThrowAfterRealBeginBeforeFinalizeKeepsPairClosed`, `AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier`; actual disk barrier와 session guard 구분 |
| REQ004/005 · AC007 | `AC007_ActualC2CompletedCarriesExactReceipt`, `AC007_ActualC2CheckpointFailureIsTerminal(memoryCheckpoint)`, `AC007_ActualC2OutcomeDefaultRejectsComposition`; Play `AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal`, `AC007_ActualC2UnsafeLockPathManualRepairIsTerminal`, `AC007_ActualCompletedReceiptLeavesAllMapsDisabledAndUIOnlyBlocked` |
| REQ001/006 · AC008 | `AC008_ConcurrentActualExecutorHasOneWinner`, `AC008_ConcurrentFreshReserveHasOneWinner`, `AC008_CompletedHistoryCannotAuthorizeSecondExecution`, `AC008_PartialRegistrationExceptionClosesOriginalPair`; Play `AC008_ActualDisableDuringPendingOrFreshClosesOriginalCohort`, `AC008_ActualDestroyAfterExecutionPreservesForensicHistory`, `AC008_ActualOldSubmitCancelReentryRejectsConsumedRequest`, `AC008_ActualFaultRecordedBeforeNativeDisableMakesCancelReentryInert` |
| REQ007 · AC009/010 | 아래 신규 strict audit named 행 및 선택·byte·native/QA 독립 원장 |

Play canceled는 actual Submit held/in-progress를 C2까지 정상 device event로 유지하고 기존 action.canceled listener를 fixture에만 등록한다. 발행별 Presenter 소비/실제 take/intake/confirm/commit을 보존한다. event/no-event를 구분하고 callback에서는 사실만 기록, 바깥에서 assert, finally 원래 thread에서 해제한다. 직접 callback/fake frame/phase 쓰기/새 제품 callback은 금지한다. 동일 consumed request의 실제 재진입은 permanent fault/consume 관리 사건으로 native 전 inert 거절한다. 첫 successor true frame을 추가 빈 발행으로 우회하지 않는다.

## 27개 throw-only와 하위 checkpoint 원장

`BeforeConfirmedConsume`, `AfterConfirmedConsume`, `BeforeAdapterGuard`, `AfterAdapterGuard`, `BeforeRouterGuard`, `AfterRouterGuard`, `BeforeC1Begin`, `AfterC1Begin`, `BeforeC1ResultValidate`, `AfterC1ResultValidate`, `BeforePreparedProofTake`, `AfterPreparedProofTake`, `BeforeC2Finalize`, `AfterC2Finalize`, `BeforeC2ResultMap`, `AfterC2ResultMap`, `BeforeFreshHandbackPublish`, `AfterFreshHandbackPublish`, `BeforeFreshReserve`, `AfterFreshReserve`, `AfterFreshPrepare`, `AfterFreshAcknowledge`, `AfterFreshCommit`, `BeforeFreshConsume`, `AfterFreshConsume`, `BeforeTerminalPublish`, `AfterTerminalPublish`만 C4 일반 throw control에 허용한다.

실제 C1 `ProfileResetDiskCheckpointV1`1..51과 C2 `ProfileResetMemoryCutoverCheckpointV1`1..19는 최종 source의 정확 enum으로 전개하고 기존 control을 한 번만 전달한다. A 정지1개/B observer2개는 일반 throw 권한과 별도로 감사한다. outcome별 planned/reached/not-reached/throw/결과·원본 권한·barrier/guard·호출 수를 기록하며 미도달을 통과로 합산하지 않는다. batch는 고정 rows/id/content/order와 180초를 유지하고 분할 변경은 재승인한다. 현재 NUnit count를 51+19+27 등의 산술로 만들지 않는다.

## Q0/Q-A/Q-B의 정확 감사 보존

현재 Q-B legacy `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs`43행은 exact framework helper 적용 후 body에서 `SceneManager`, `Application.Quit`, `Profile`, `Run`, `Settings`, `Wardrobe`, `CIO`, `CUA`, `InputSystem`, `UnityEvent`, `delegate`를 금지한다. 44행은 Q-A의 `TryTakeRetainedIntent`부터 `private void Awake`까지 `SceneManager`, `Application.Quit`, `new InputRouter`, `new HubMenuPresentationControllerV1`, `UnityEvent`, `delegate`를 금지한다. 이 legacy 파일은 **변경 허용 없음**이다. 신규 fresh 메서드를 옛 transfer 구간에 넣어 충돌하면 금지를 완화하지 않고 설계를 재검토하거나 별도 정확 승인을 받는다.

`Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs`도 변경하지 않는다. 현재 helper18~40행은 정확 five-import header의 `using System.Runtime.CompilerServices;` 하나만 제외하고 나머지 body token을 유지한다. 신규 Q-B body에서 Profile/Run/delegate 등을 허용하지 않는다. blind word boundary/Unicode/namespace 조각/넓은 regex 완화는 금지한다.

`Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubPresentationScopeAuditTests.cs`도 변경하지 않는다. 현재 body는 TMP bytes/build byte NoOp이며 존재하지 않는 transfer 감사를 이 파일에 귀속하지 않는다. 신규 Q-A fresh 감사는 새 `C4ExecutionBridgeStrictAuditTests.cs`에만 둔다.

예외적 변경 후보는 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`뿐이다. 기존 AC011 메서드명/선택을 유지하고 역사 증거9행·불변 current legacy7행을 보존한다. 기존 C2/C2R successor2개를 역사 증거로 보존하고 **변경 Adapter/Router의 current exact C4 pin/provenance**로 분리한다. 구 SHA 또는 새 SHA fallback은 금지한다. 현재92~131행의 current2 비교는 C4 source가 달라지면 승인된 C4 pin과 독립 원문 증거만 비교해야 한다. Git 작업상태 판정·외부 경로·선언 scope·다른 Q0 행은 변경하지 않는다. 옛 C2 source에 현 SHA를 소급 귀속하지 않는다.

신규 Hub strict named 행은 `AC009_LowerExecutorHasNoHubTypeOrPublicAuthority`, `AC009_QBFreshAmendmentKeepsGameplayAndDurableTokensForbidden`, `AC009_QAFreshAmendmentPreservesOriginalTransferAudit`, `AC009_Q0SelectsOnlyExactChangedCurrentSuccessor`, `AC010_PredecessorBodiesAndRequiredSelectionNamesRemainExact`다. 기존 strict body를 추출·복원하면 실제 정확 영역/SHA를 대조하고 신규 fresh 전체 body의 금지/상호 actual 예약/이력 분리도 별도 검사한다. 옛 body 통과만으로 fresh 동작 검수를 면제하지 않는다.

원본 bytes 신규 저장 후보는 `docs/evidence/c4-audit-predecessor/HubUiOnlyQ0ScopeAuditEditModeTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubMenuIntentHandoffEditModeTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubMenuIntentHandoffC3AuditSuccessorTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubPresentationScopeAuditTests.cs.txt`뿐이다. 승인 뒤 exact predecessor SHA와 전체 byte를 저장하며 원본 docs137/기존 evidence는 변경하지 않는다. Q0의 신규 actual current pin 입력 후보 `docs/evidence/c4-audit-current-source-pins.json`은 source freeze 뒤 생성하고 exact SHA/승인 문서 결속을 요구한다. 미수용 source를 accepted로 표시하지 않는다.

## 현재 predecessor와 신규 source/meta 동결

R11 14파일 원장 `artifacts/c3-upper-r11-frozen-source-manifest.json` SHA `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`와 r3의14개 전체 path/SHA 표는 불변 predecessor다. 승인 입력으로 r3에서 정확히 읽는다. Q0 원본 SHA `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4`, Adapter `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`, Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, Q-A scope `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`를 역사 pin으로 보존한다.

새 C4 source/meta 최종 SHA는 구현 뒤 신규 원장에 동결하고 R11 fixed14를 새 source로 덮어쓰지 않는다. 변경 runtime7+신규 runtime/meta+시험 fixture/meta+Q0 audit+신규 도구/evidence의 정확 전체 bytes를 새 input path 목록에 포함한다. 불변 기존 meta도 타입/asset identity 입력으로 유지한다. 허용 밖 source 변경은 실패이며 통과 비교에서 무시하지 않는다.

## 새 QA 프로토콜과 정확 도구 후보

아래 신규 도구만 구현 후보로 둔다. 기존 `artifacts/c3-build-focused-selection.ps1`, `artifacts/c3-build-required-decision-expected-rows.ps1`, `artifacts/c3-verify-decision-rows.ps1`, `artifacts/c3-final-selected-run-r3.ps1`, `artifacts/c3-r11-final-validation-queue.ps1`, `artifacts/c3l-capture-inputs.ps1`, `artifacts/c3l-verify-run.ps1`, `qa/tools/Invoke-UnityQa.ps1`은 변경하지 않는다. 기존 script를 C4 stem으로 개조하지 않는다. old run-r3는 `^c3-...$` stem과 old capture를 사용하므로 C4 runner로 대체할 수 없다.

| 신규 경로 | 정확 역할·계약 |
| --- | --- |
| `artifacts/c4-build-focused-selection.ps1` | 신규 Edit/Play/Hub class의 전체 Test/TestCase/UnityTest에서 exact qualified names·dedupe·anchored selector를 생성한다. bool/int/string/enum의 실제 NUnit 표기를 지원하지 못하면 실패하며 누락시키지 않는다 |
| `artifacts/c4-build-required-checkpoint-rows.ps1` | AC row/throw27/C1의51/C2의19/A/B/C의 actual authority/outcome/phase/ExpectedCase/증거 종류·순서를 생성한다. 미도달 계획도 구분한다 |
| `artifacts/c4-capture-inputs.ps1` | 정확 신규 입력 목록의 source/meta/assets/settings/packages/qa/필요 docs/tool/evidence/selection/checkpoint에 대해 같은 path/hash를 기록한다. 실행 출력/XML/log/queue 진행 자료는 입력에서 제외한다 |
| `artifacts/c4-verify-run.ps1` | 실제 XML full names/results/count·actual native exit/QA return/outer exit·source 전후·신규 freeze를 검증한다. native0/QA0 및 별도의 실제 native 증거가 필수다 |
| `artifacts/c4-verify-required-rows.ps1` | full row의 planned/reached/result/authority/ref/call/guard/barrier/history와 실제 NUnit CaseResult를 결속한다. pure/structure/full bridge를 구분하며 내부 passed만으로 NUnit 실패를 수용하지 않는다 |
| `artifacts/c4-final-selected-run.ps1` | C4 stem만 허용하고 정확 선택+before capture→기존 InvokeUnityQa 한 번→actual exit+after capture→두 verifier 순서다. 기존 결과 덮어쓰기·진행 중 별도 Editor 기동은 금지한다 |
| `artifacts/c4-final-validation-queue.ps1` | 단일 queue/lease로 C4 focused와 기존 불변 선택을 신규 source에서 순차 실행한다. 중단/실패를 성공으로 바꾸지 않고 same inputs를 실행 간 대조한다 |

실제 Unity 실행은 변경하지 않은 `qa/tools/Invoke-UnityQa.ps1`만 runner가 `-ProjectPath/-UnityPath/-TestPlatform/-TestFilter/-ResultsPath/-LogPath/-AwaitSeconds`로 호출한다. 현재6000.6 설정·기존 owned Editor 대기 방식·licensing/network 설정을 변경하지 않는다. `AwaitSeconds`는 관찰 창이며 NUnit180초를 늘리지 않는다. run-r3/capture/Invoke의 현재 SHA는 각각 `B66C0BB96D2DB9240D416C043F4104291E4150D6229154BB3F744E8ECB77230B`, `E95A0E908BFFDBEB22577295384D22D013F2F8488B6EE676395CA26944424CD1`, `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`다.

기존884는 R11 capture 사실이며 C4의 고정 count가 아니다. 기존 source에 승인된 C4 변경/신규 입력을 반영한 정확 새 path 목록을 구현 뒤 동결하고 dedupe 길이로 새 count를 발급한다. 모든 실행에서 같은 paths/hash를 전후·상호 대조한다. 전체 Assets 등 기존 포착 범위를 숨겨 좁히지 않고 기존 capture의 필요한 source/settings/assets/qa와 직접 읽는 docs/resources를 유지하며 C4 입력을 추가한다. 실행 생성 출력은 명시 제외한다. count 차이를 설명하는 path별 추가/제외 사유 원장이 필수다.

생성 후보는 `artifacts/c4-focused-edit-selection-v1.json`, `artifacts/c4-focused-play-selection-v1.json`, `artifacts/c4-focused-hub-selection-v1.json`, `artifacts/c4-required-checkpoint-rows-v1.json`, `artifacts/c4-frozen-source-manifest-v1.json`, `artifacts/c4-frozen-input-paths-v1.json`, `artifacts/c4-predecessor-selection-name-map-v1.json`, `artifacts/c4-final-validation-queue-plan-v1.json`, `artifacts/c4-final-validation-queue-result-v1.json`뿐이다. 각 run은 `artifacts/c4-<승인stem>.xml/.log/-source-before.json/-source-after.json/-native-exit-observation.json/-qa-tool-return.json/-verification.json/-required-row-comparison.json`을 새 불변 출력으로 남긴다. 실제 run stem 목록은 생성 queue plan을 아스트라/루나가 정확 동결한 뒤 실행 전에 확정한다. 미승인 stem과 기존 증거를 덮어쓰지 않는다.

## 실행·수용 gate와 잔여

기존240 Edit/15 Play/562 Edit/51 worker/610 Play의 exact qualified names와 순서를 유지한다. 240의91 matrix+149 remaining 서로소 내역도 원장에 보존하며 새 C4 focused에 흡수·삭제·개명하지 않는다. 최종 method/enum/TestCase source에서 NUnit names/count를 생성하고 selector를 동결한 뒤 실제 XML의 missing/extra/dup/skip/inconclusive/failed를 모두 대조한다. 사례180초·native0·QA0·outer0·row full records·입력 차이0·전체 선택 통과·루나 새 source/실행 독립 P0/P1=0·아스트라 통합 수용을 요구한다. 과거 R11 통과를 변경 source의 통과로 재사용하지 않는다.

잔여는 (1) Owner가 lower fresh reserve/complete를 호출해야 하는 경우의 exact internal 타입/서명·typed result 필드 map, (2) 최종 범위 승인 시 Q0 current C4 pin/provenance 신규 입력과 source freeze 순서의 독립 검수, (3) 새 parser의 실제 NUnit enum/string 표기와 전체 row/selector 지원, (4) 구현 뒤 새 입력 목록/count/run stem 동결이다. 가짜 count/API로 채우지 않는다. 신규 QA 도구 구현·실행도 이 Draft의 승인 뒤에만 가능하다. 현재 C3 gate 미완료·C4 Review를 유지한다.
