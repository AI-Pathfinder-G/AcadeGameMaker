# C4 제한 구현 계약 준비 r2 초안

**r2 우선 규칙:** 아스트라가 A/B/C의 아래 기술 선택을 다음 Draft에 지정했다. Approved·구현 승인은 아니다. 원본 `8339895BC58985997AD55A9577045F99D2EF3C74B1821CAA3E450EFCA6C9E9BE`와 세 검토 원본은 그대로 보존한다. 기존 경로·fresh API·감사·14개 pin·선택을 아래에 유지하고, 이번 r2의 추가 상태/시험 경계는 끝의 규범 보정 후보에만 한정한다. C3 선행 전체 수용 전제와 C4 Review는 유지한다.

- 날짜·작성자: 2026-09-29, 솔, 실제 `gpt-6-sol`.
- 상태: **Draft — 구현 권한 없음**. 원본 C4는 Review다. 이 문서는 새 승인·구현·실행·결과 발급·게시가 아니다.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`; 공동 최종 검증의 `AC-M5D7QC3-007/008`.
- 근거: [경계 갱신 제안](2026-09-29-c4-r11-frozen-boundary-design-refresh.md) SHA `BAB33568BD479AF46C265F3FCCEC232BBEF339C472B6F9CB1DF9A90E17347F0E`, [아스트라 제한 수용](../approvals/2026-09-29-c4-r11-design-refresh-limited-acceptance.md) SHA `C8D9B5EFD011D4C6014F84B8A62F9F502328E6031ACA417F5CDA66DB7D4A77DB`, 독립 루나 SHA `C9E969C33BF58F8EF401E5D468C6674239BF785BFBB952B936F3E5CE47592AA4`.
- 상위가 보고한 현재 필수 Play 610개 실행의 종료·수용을 추정하지 않는다. C3 선행 전체 수용은 아직 없으며 C4 Review와 현재 소스 동결을 유지한다. 기존 240/15 집중과 562/51/610 필수 실행을 아래 신규 시험으로 대체하지 않는다.
- 이번 작업은 새 문서만 작성했다. 기존 계약·runtime·QA·선택·캡처·Git·네트워크·Unity·컴파일을 변경하거나 실행하지 않았다.

## 승인 전 확정해야 할 구현 경계

실행 순서는 실제 C3 `CommitForExecution` → lower의 별도 실제 소비 → 양측 ResetExecutionPending → 실제 C1 Begin 한 번 → 전체 typed 결과 인증 → DiskPrepared일 때만 실제 C2 FinalizeReset 한 번 → typed 결과 composition이다. C3 Completed 이력과 C4 실제 소비를 구분한다. C1의 lease를 C2로 전용하지 않는다. identity·prepared proof·root·nested result는 private 원본 참조에서만 얻는다.

아래 경로는 **승인 검토용 정확 후보 목록**이다. 승인 전 편집하지 않는다. 경로 접두사 생략 없이 명시한다.

| 경로 | 허용할 최소 변경과 금지 경계 |
| --- | --- |
| 신규 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` 및 `.meta` | 기존 lower를 같은 internal partial로 확장해 private C3 witness에 접근. executor·별도 소비 이력·typed 실행 결과·불투명 fresh handback·상호 가드 조율. Hub 타입 인자/참조·identity/proof getter·외부 registry 접근 없음 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | partial 선언과 실제 Completed 이력에 결속된 별도 C4 소비/실패/handback 경계만. 기존 C3 reserve/complete/closed append history는 유지 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | 별도 execution/fresh/terminal 가드 절반과 정확 등록 pair/root/generation 인증. ordinary save·notification·launch·옛 C3 관찰 차단. 기존 C2 receipt 단일 발급·소유권 보존 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 대응 가드 절반과 callback/semantic 발행 차단. UI quarantine/OnEnable이 pending 억제를 풀지 않도록 함. actions/map 교체·enable 또는 gameplay 연결 없음 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | 필요한 경우에만 기존 136행 및 adapter terminal 진입의 **등록된 정확 guarded pair** 허용 분기. 97행의 실제 4인자 경계·lease/proof 재인증·staging/default/memory/barrier/receipt 알고리즘은 불변 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | 기존 commit 뒤 executor 호출, 실제 issuer 결과 인증, 일회용 fresh orchestration와 정확 reciprocal closure. 정상 완료 loser와 손상 종료 구별 |
| 별도 amendment: `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | 실제 Hub 중립 fresh capability로만 reserve/commit하는 신규 경계와 독립 live 슬롯. legacy TryTake/최초 taken/원래 epoch·issued 이력/Cancel successor를 보존 |
| 별도 amendment: `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | 같은 fresh 예약으로만 prepare/ack하는 독립 live controller/cursor 슬롯. 이전 retained/cursor/transfer/closed 이력 보존. 원래 Q-A authority와 Cancel/Rearm을 완화하지 않음 |

기존 메타는 수정 후보가 아니다. 신규 메타의 GUID는 승인 후 실제 생성값과 SHA를 동결하며 이 초안이 임의 값을 발급하지 않는다. C1/Profile 소스·알고리즘·result/proof, asmdef/friend/public ABI, 자산·prefab·scene·wrapper·Packages·ProjectSettings·네트워크는 금지한다. Q-A/Q-B amendment를 승인하지 않으면 AC004 fresh 경계를 구현할 수 있다고 간주하지 않는다.

## 정확 fresh API와 권한 처리

서명은 수용된 경계 제안의 후보를 따른다. lower의 `ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed, DesktopProfileLaunchAdapterV1 adapter, InputRouter router)`와 getter 없는 `ProfileResetFreshC3HandbackV1`은 Input.Unity 소유다. Owner의 `AcceptFreshExecutionHandback(ProfileResetExecutionResultV1 result)`만 정상 lower handback을 소비할 수 있다. C1 전체 행의 `Busy/NoBarrier` 또는 `ConfirmationStale/NoBarrier`가 private executor 등록과 정확히 일치해야 한다.

Hub에는 `HubNewGameExecutionFreshCapabilityV1`/`HubNewGameExecutionFreshReservationV1`만 전달한다. Q-B `ReserveExecutionFresh(capability, owner, presenter, router, nextEpochToken, nextEpoch)` → Q-A `PrepareExecutionFresh(owner, reservation, nextEpochToken, nextEpoch)` → `MatchesPreparedExecutionFresh` → Q-B `CommitExecutionFresh` → Owner consume 및 lower complete 순서다. 모든 인자는 실제 같은 참조이며 정확 서명 전체는 BAB335 제안의 코드 블록을 승인 입력으로 동결한다. 그 블록의 `IsExecutionFreshInProgress`와 private 미완료 예약의 일치가 live permission이며 완료 이력은 permission이 아니다.

실제 registry 등록·소비·완료 사건은 기존 lower lifecycle gate 및 append-only 실행 이력에 결속한다. gate 획득 뒤 같은 actual pending과 미소비 상태를 다시 확인한다. 정상 경쟁 loser는 비변경 거절, 등록된 원본 증거의 반사 손상은 내부 무결성 실패와 양측 terminal이다. 여러 가변 필드의 순차 대입만으로 인증하지 않는다. old committed epoch와 closed Q-A/Q-B는 역사로 남고 신규 슬롯만 pristine하게 생성한다. 신규 cursor ready 여부와 무관하게 **첫 실제 true 프레임을 폐기**한 뒤 다음 정상 선택부터 take를 허용한다. fresh prepare/ack/commit/consume의 부분 실패는 원본 pair와 신규 슬롯을 모두 닫으며 handback을 재사용하지 않는다. 새 UI 발행 허용은 reciprocal 완료 proof에 결속한다.

## 새 focused 시험과 실제 권한 fixture의 정확 경로

모든 아래 `.cs`에는 같은 경로의 `.cs.meta`만 동반한다. 기존 시험 class·private fixture에 행이나 도우미를 추가하지 않는다. 현재 실제 asmdef/friend를 유지한다.

| 새 파일 | class·namespace·조립과 범위 |
| --- | --- |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` | `AcadeGameMaker.Tests.EditMode.InputUnity.ProfileResetExecutionBridgeV1Tests`; 기존 Input.Unity.EditMode.Tests. 실제 임시 launch·C3 opaque 발급과 lower 인증/가드/typed result·fault 행 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` | 같은 namespace의 `C4ActualExecutionEditFixtureV1`; 독립 fixture. 정상 environment/prepare/launch API와 정확 reflection binding으로 Hub 호출. 이전 fixture private factory와 결합하지 않음 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs` | `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetExecutionBridgePlayModeTests`; 기존 Input.Unity.PlayMode.Tests. 실제 InputTestFixture·keyboard·activation·출판/소비와 fresh/lifecycle/완료 후 차단 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` | 같은 namespace의 `C4ActualExecutionPlayFixtureV1`; 실제 device 이벤트·정상 Update·실제 host lifecycle을 사용하는 독립 fixture |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` | `AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests`; 기존 Hub.Presentation.Unity.EditMode.Tests. source/API·정확 감사 successor·기존 역사/선택 보존 검사만 |

실제 Hub 양성 입력은 Input.Unity.PlayMode.Tests에 둔다. Hub Play 조립은 Input.Unity의 environment/preparation friend가 아니므로 그 조립에서 정상 권한을 반사 대입으로 만들지 않는다. Hub 타입의 정상 호출은 assembly-qualified type, 정확 declaring type, DeclaredOnly, 전체 parameter/byref/return map으로 인증한 reflection만 사용한다. 이름 단독 fallback·DynamicProxy·Reflection.Emit·상위 private 상태 대입은 금지한다. 실제 임시 root의 bytes는 정상 encoder를 사용하고 실제 owner/adapter/router 발급만 정상 권한으로 취급한다. 정상 관측을 이유로 lazy action/receipt getter를 불필요하게 반복하지 않는다.

## AC별 named 행 초안

아래 이름은 새 class 안의 후보 method 이름이다. 양성/음성은 실제 API 또는 허용된 단일 손상/throw로 구분한다. 아직 소스 선언·NUnit 정식 names·예상 사례 수는 생성되지 않았다. `ACnn`은 전부 `AC-M5D7QC4-nnn`을 뜻한다.

| AC·REQ | Input Edit 후보 이름과 실제 행 |
| --- | --- |
| 001·001 | `AC001_ActualCommittedOpaqueStartsExactlyOnce`; `AC001_UnregisteredCandidateCannotStart`; `AC001_UncommittedActualRequestCannotStart`; `AC001_ActualForeignRootPairGenerationCannotStart`; `AC001_ConsumedAndLateLoserCannotCloseWinner`; `AC001_SingleRegisteredWitnessDamageClosesWithoutBegin`. foreign 행은 실제 다른 cohort/root/epoch 원본이며 scalar로 권한을 만들지 않음 |
| 002·002 | `AC002_ActualBothGuardHalvesBlockOldPublication`; `AC002_ThrowAtGuardBoundaryClosesOriginalPair(checkpoint)`; `AC002_QuarantineReleaseCannotReopenPendingGuard`. checkpoint별 계획/관측·각 publisher 결과·원본 pair 폐쇄를 요구 |
| 003·003 | `AC003_ActualBeginUsesOriginalIdentityOnce`; `AC003_ActualTerminalDiskRowNeverCallsFinalize`; `AC003_ActualIssuedResultSingleFieldDamageRejectsGetter`; `AC003_ScalarOrUnregisteredResultCannotBeExecutionAuthority`. 결과 손상은 실제 발급 후 getter 음성 검증이며 실행 전 손상을 재현했다는 주장과 구별 |
| 004·004 | `AC004_ActualC1BusyIssuesOneFreshHandback`; `AC004_ActualC1StaleIssuesOneFreshHandback`; `AC004_TerminalDiskRowsNeverIssueHandback`; `AC004_FreshPartialFailureClosesBothSlots(checkpoint)`; `AC004_ConsumedOrForeignHandbackCannotRebind` |
| 005·005 | `AC005_ActualDiskPreparedPassesSameProofRootPairOnce`; `AC005_ActualProofPairRootDamageStopsBeforeCutover`. actual result/proof는 실제 Begin 산출물뿐이며 손상은 음성 주입. 전체 실행 경로에 주입 가능한 지점은 아래 미결정에 따름 |
| 006·002/006 | `AC006_ActualC1CheckpointThrowPreservesBarrierAndGuard(diskCheckpoint)`; `AC006_ThrowAfterRealBeginBeforeFinalizeKeepsPairClosed`; `AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier`. 실제 C1의 51개 checkpoint 모두 별도 사례 후보, marker 유무로 result를 추정하지 않음 |
| 007·004/005 | `AC007_ActualC2CompletedCarriesExactReceipt`; `AC007_ActualC2CheckpointFailureIsTerminal(memoryCheckpoint)`; `AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal`; `AC007_ActualC2ManualRepairHasNoReceiptOrFreshAuthority`. 마지막 두 전체 연결 행의 결정적 fixture는 승인 전 미결정 |
| 008·001/006 | `AC008_ConcurrentActualExecutorHasOneWinner`; `AC008_ConcurrentFreshReserveHasOneWinner`; `AC008_CompletedHistoryCannotAuthorizeSecondExecution`; `AC008_PartialRegistrationExceptionClosesOriginalPair`. 실제 경쟁과 지연창 구조 증거를 분리 |

| Play 후보 이름 | AC·정상 흐름/음성 관측 |
| --- | --- |
| `AC004_ActualBusyFreshCursorDiscardsFirstSubmitThenTakesNewRequest` | 004/008: 실제 C1 Busy→fresh reciprocal→첫 실제 Submit=true 폐기→다음 실제 take·새 decision. 이전 opaque/decision/cancel/rearm/handback은 거절 |
| `AC004_ActualStaleFreshCursorDiscardsFirstSubmitThenTakesNewRequest` | 004/008: 정상 encoded leaf 변경으로 실제 C1 Stale, 위와 동일 새 세대·이력·최초 frame 검증 |
| `AC002_RealEnableAndAfterUpdateCannotReleaseExecutionGuard` | 002/006: 실제 UI 갱신·정상 callback/lifecycle과 publisher 차단. private callback 직접 호출 금지 |
| `AC007_ActualCompletedReceiptLeavesAllMapsDisabledAndUIOnlyBlocked` | 007/009: 실제 C2 receipt·default memory·actions 소유권·양측 receipt·디스크 barrier 최종 행과 차단 유지. 장면/Run 진입 없음 |
| `AC008_ActualDisableDuringPendingOrFreshClosesOriginalCohort` | 008: 정상 Behaviour 비활성화, 실제 콜백·전체 폐쇄·늦은 호출 거절·history 보존 |
| `AC008_ActualDestroyAfterExecutionPreservesForensicHistory` | 008: 실제 destroy와 단일 소유 action 정리, foreign action disposal/이중 C1/C2 없음 |

| Hub Edit 후보 이름 | AC·strict 검수 |
| --- | --- |
| `AC009_LowerExecutorHasNoHubTypeOrPublicAuthority` | lower→Profile 방향·ABI/friend/asmdef 불변·private registry/identity/proof 비노출 |
| `AC009_QBFreshAmendmentKeepsGameplayAndDurableTokensForbidden` | Q-B의 Profile/Run/gameplay/Settings/delegate 금지 및 정확 using 예외 유지 |
| `AC009_QAFreshAmendmentPreservesOriginalTransferAudit` | Q-A 기존 transfer 범위·원래 source/body 이력과 fresh 슬롯 상호 인증 |
| `AC009_Q0SelectsOnlyExactChangedCurrentSuccessor` | 실제 바뀐 Adapter/Router만 엄격 현재 successor; old-or-new 허용 없음 |
| `AC010_PredecessorBodiesAndRequiredSelectionNamesRemainExact` | 원래 감사/fixture source 기록 및 기존 240/15/562/51/610 정식 names·원장 보존. source 변경에 맞춘 새 증거와 과거 증거를 분리 |

양성 call count·identity/proof 참조는 실제 issuer와 실행 trace의 정상 읽기 및 구조 근거로 대조한다. private witness를 읽어 정상 권한을 조립하지 않는다. C1/C2 결과를 `new`, scalar tuple, 반환 대체 delegate 또는 registry 삽입으로 만들지 않는다. 실제 발급 객체의 단일 필드 손상은 음성 getter/무결성 시험에 한정한다. 손상 뒤 정상처럼 복구·재등록하지 않는다.

## throw-only checkpoint 목록

아래 27개 일반 checkpoint는 **throw-only**로 보존한다. r2에서 별도 승인 후보로 좁힌 단일 정지와 두 음성 observer는 뒤의 기술 선택 표에 한정하며 일반 checkpoint 권한으로 확대하지 않는다. 신규 C4 내부 control은 생산 경로의 no-op과 시험 경로를 구분한다. checkpoint는 결과를 반환하거나 identity/root/pair/proof/receipt를 교체하지 않는다. 전역 가변 hook·UI callback seam·gateMonitor·권한 대입 통로를 만들지 않는다.

고정 후보는 `BeforeConfirmedConsume`, `AfterConfirmedConsume`, `BeforeAdapterGuard`, `AfterAdapterGuard`, `BeforeRouterGuard`, `AfterRouterGuard`, `BeforeC1Begin`, `AfterC1Begin`, `BeforeC1ResultValidate`, `AfterC1ResultValidate`, `BeforePreparedProofTake`, `AfterPreparedProofTake`, `BeforeC2Finalize`, `AfterC2Finalize`, `BeforeC2ResultMap`, `AfterC2ResultMap`, `BeforeFreshHandbackPublish`, `AfterFreshHandbackPublish`, `BeforeFreshReserve`, `AfterFreshReserve`, `AfterFreshPrepare`, `AfterFreshAcknowledge`, `AfterFreshCommit`, `BeforeFreshConsume`, `AfterFreshConsume`, `BeforeTerminalPublish`, `AfterTerminalPublish`다. 도달하지 않은 checkpoint를 통과로 집계하지 않으며 outcome별 실제 도달 원장을 승인 전에 고정한다.

기존 실제 C1 control의 `ProfileResetDiskCheckpointV1` 1..51(149~165행)와 실제 C2 control의 `ProfileResetMemoryCutoverCheckpointV1` 1..19(27행)를 정확 enum 이름별로 전개한다. C4→실제 Begin/Finalize의 시험 overload는 기존 실제 하위 구현에 control만 전달하며 1회 호출을 유지한다. C2 `InvokeOperation`은 원래 exact-owned operation을 1회 수행하고 필요 시 그 전/후 throw만 허용하며 skip/대체/foreign action을 수행하지 않는다. 실제 durable 이전 throw라도 검증된 C1 Busy/Stale 행 없이 C4를 fresh로 돌리지 않는다. throw는 손상/불확실 경계이며 C1이 실제 반환한 closed typed 행과 구별한다.

## 이전 초안의 outcome 검증 흐름·미결정 이력

아래 A/B/C는 원본 833989 초안의 역사로 보존한 미결정 설명이다. r2의 현재 기술 선택은 이 문서 끝의 명시적인 A/B/C 규범 보정 후보가 우선한다. 원본의 정지 금지·throw-only 한계가 별도 개정 없이 해소됐다는 뜻이 아니다.

1. 실제 temp root의 정상 profile bytes·launch/auth·C3 capture/confirm/commit으로 opaque를 발급한다. C1 Busy는 그 뒤 별도 실제 root lease를 보유한 채 executor를 호출한다. real Stale은 commit 뒤 정상 encoder로 실제 leaf를 바꾼 후 실행한다. 각 결과는 원래 C1 typed result 전체·NoBarrier·mutation 관측·private 실행 provenance를 함께 검증한다.
2. DiskPrepared/Completed 양성은 변경하지 않은 실제 C3 opaque로 실제 Begin→실제 Finalize 전체를 수행한다. 원래 proof 같은 참조와 root/pair, 실제 default disk/memory, barrier 최종 proof, 같은 C2 receipt를 대조한다. receipt에 없는 관찰 대상을 합성하지 않는다.
3. terminal 결과는 실제 filesystem 제약과 기존 하위 throw-only control이 **실제 생성한 typed result**만 사용한다. C1의 정확 closed result로 C2 0회, 실제 C2 비완료로 fresh 0회·양측 terminal을 확인한다. C4 own throw는 C2 typed outcome 시험을 대신하지 않는다.
4. **미결정 A:** 전체 C1→C2 연결 안의 C2 Acquire Busy를 결정적으로 얻는 실제 lease 경쟁 준비와 ManualRepair 조건의 좁은 실제 filesystem fixture를 아직 동결하지 않았다. 별도 actual actor가 루트 lease/파일을 정상 취급하는 안을 검토하되 control checkpoint에서 lease/파일을 조작하거나 fake Busy/ManualRepair를 반환하지 않는다. 기존 C2 단독의 실제 Busy/ManualRepair 행을 C4 intake에 대신 주입해서 통과시키지 않는다. 결정적 권한·경합 준비를 루나가 확인한 뒤 정확 fixture 경로/정식 사례를 승인해야 한다. 미해결이면 C4 전체 Approved 준비 완료로 선언하지 않는다.
5. **미결정 B:** C1 typed row/proof의 실제 생성 직후 C4 validation 이전 손상을 throw-only 조건 안에서 관측하는 실행 fixture는 아직 확정하지 않았다. 우선 실제 결과 getter 손상 음성 + production validation-before-proof/C2 구조 증거를 구별한다. AC003/005의 요구를 대체했다고 단정하지 않고 아스트라/루나가 충분성 또는 별도 좁은 허용을 결정한다. 정상 권한 제조·결과 대체 seam은 허용하지 않는다.
6. **미결정 C:** callback-free 동기 executor의 same-thread reentry와 지연 pre-auth 경쟁 창의 실행 재현 가능 범위. 실제 도달 가능한 정상 callback/경쟁만 실행하고 나머지는 source·gate 재인증·finally·반복 거절의 구조 증거로 명시한다. 새로운 callable 통로나 테스트용 대기 gate를 만들지 않는다.

## Q0/Q-A/Q-B 감사의 별도 허용 후보

원본 C4 runtime 허용 목록은 감사 시험 수정 권한이 아니다. 각각 아스트라의 별도 narrow amendment를 필요로 한다.

- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`: Adapter/Router가 실제 달라질 때 현재 exact successor 선택만. 기존 9개 역사·7개 legacy-current·2개 C2/C2R provenance는 그대로 보존한다. 현재 Adapter `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`, Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` pin과 원본 감사 SHA `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4`를 predecessor로 보존. 역사 provenance와 현재 파일 검증을 분리하며 새 current는 exact SHA 하나만 선택한다. 기존 test method/정식 names를 추가·삭제하지 않는다. Git 작업상태 전제도 이 amendment로 몰래 바꾸지 않는다.
- `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs`: 필요한 경우 기존 43~44행 strict source-body 검사에 대한 **정확 provenance maintenance만** 별도 허용 검토. 기존 금지 token/import 규칙·조건·method names는 유지한다. 새로운 execution 동작을 허용하려고 금지 단어/범위를 제거하지 않는다.
- `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs`: 기존 C3 import exception/body 복원 helper와 시험은 그대로 보존하는 predecessor이며 수정 후보가 아니다. 새 `C4ExecutionBridgeStrictAuditTests.cs`에서 C4 successor를 별도 검증한다.
- `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubPresentationScopeAuditTests.cs`: 현재 SHA `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`는 predecessor로 보존하며 source-controlled TMP 입력의 불변 검사는 변경하지 않는다. Q-A transfer 변경의 strict successor는 신규 C4 감사 class에 둔다.
- 신규 증거 후보 `docs/evidence/c4-audit-predecessor/HubUiOnlyQ0ScopeAuditEditModeTests.cs.txt`, `HubMenuIntentHandoffEditModeTests.cs.txt`, `HubMenuIntentHandoffC3AuditSuccessorTests.cs.txt`, `HubPresentationScopeAuditTests.cs.txt`: 승인 후 실제 predecessor bytes를 exact SHA로 저장할 경로만 제시한다. 이번에 파일을 만들거나 역사 원문을 바꾸지 않았다.

Q-B `using System.Runtime.CompilerServices;`의 기존 정확 plumbing 제외만 유지한다. blind 단어 경계·namespace 조각·Unicode 숨김·넓은 regex 예외를 만들지 않는다. Q-A controller/cursor 생성은 기존 승인된 fresh 경계에서만 허용하며 legacy transfer 구간의 금지 검사는 보존한다. 실제 unchanged 파일에 새 successor를 발급하지 않는다.

## 현재 14파일 predecessor 동결

원장 `artifacts/c3-upper-r11-frozen-source-manifest.json` SHA `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`의 정확 predecessor다. 시험 중 갱신하지 않는다.

| 경로 | SHA-256 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs` | `F80D57376F3845E16BB3D347CFB3834ACE8FCCB53F539376D880A844A90F9581` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs.meta` | `4DDE703DFCF2E048D9737FF57EED9E0CA1192109046871AFE31C437400F13AD2` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs.meta` | `05F240AFB899893F47C94798D05D65BD97F5982120AD8EE086CBFBD27C1C482B` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs` | `57E7EFB17307BFB156D338F7BDCE25104E3192748FD6C159F7BB7B4A7483DAD6` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` | `A0A1928CE83A22D324F785476B38DCDE595B71E53122139F10535C5B3AF6F9D6` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs.meta` | `AC62F31C59BC1FA79BC0999BFCE6D6F7965AE6C3F2E2061DE8B96587A2C6B611` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs` | `1B0F8D0919770CD1FBD06F908EC23E370A18E911EC5AAC09938FAF0BFDA174E6` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs.meta` | `F1FFFA239E271C037AC818FF4C03B698DC3B07B561F8024887F4E80677A2FEFB` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` | `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs.meta` | `1F20AD5E096121FE7D60FF605474376F4F7BF4786DED820C455916B584A40D6A` |

추가 근거 runtime C1 `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, C2 `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`와 정확 수용 연결은 BAB335 제안의 근거표를 따른다. 이 둘을 R11의 14개에 들어 있다고 표현하지 않는다.

## 실행 전 선택·예상 수 동결 절차

Approved 전환은 C3 독립 선행 수용, 위 미결정 A/B/C의 기술 판정, 정확 runtime/test/audit 허용 목록과 루나 설계 P0/P1=0 이후 아스트라만 수행한다. 그 뒤 구현자가 최종 코드 선언을 완료하면 아래를 실행 전에 별도 새 원장으로 동결한다.

1. `[Test]`/`[UnityTest]`/모든 `[TestCase]`의 실제 namespace/class/method/인자 형식·escaping을 읽어 **정식 NUnit names 전체**를 생성한다. 위 후보 이름·51/19 enum 전개·27 C4 checkpoint의 실제 도달 행을 순서·REQ/AC·양성/음성·outcome·권한·180초 제한과 결속한다. TestCase 문자열/정수/enum의 parser 지원을 실제 확인한다. 이 문서의 method 수를 실제 사례 수로 쓰지 않는다.
2. 각 class anchored selector를 exact escaped names의 합집합으로 동결한다. 신규 Edit/Play/Hub 선택·예상 수·실제 source/meta SHA·checkpoint 도달 원장을 `artifacts/c4-focused-edit-selection-v1.json`, `artifacts/c4-focused-play-selection-v1.json`, `artifacts/c4-required-checkpoint-rows-v1.json`, `artifacts/c4-frozen-source-manifest-v1.json`으로 분리할 후보를 제안한다. 이번에 생성하지 않는다. 승인 시 정확 파일 목록과 도구 경로를 별도 명시해야 하며 기존 생성기/validator를 무단 수정하지 않는다.
3. 예상 count는 중복 제거 뒤 exact names 배열의 길이와 일치해야 한다. 신규 focused와 기존 240/15/562/51/610의 교집합을 검사하고 기존 names·순서를 그대로 보존한다. 선택되지 않은 기존 시험을 새 이름으로 보고하거나 내부 checkpoint 행을 NUnit 통과 수에 합산하지 않는다.
4. 최종 각 사례의 실제 XML Passed와 실제 checkpoint 계획/관측을 결속한다. 누락/extra/중복/실패/skip/inconclusive와 native/QA/outer 종료, source/meta/필수 입력 전후 지문을 대조한다. 180초를 늘리지 않는다. 내부 여러 checkpoint를 한 expensive case에 몰아 시간 초과를 숨기지 않는다. 실제 행별 비용이 한도를 넘으면 원래 행/내용/순서를 유지한 별도 고정 분할을 다시 승인받는다.
5. 새 소스의 필수 회귀와 C3 AC007/008+C4 AC001..010 공동 최종 독립 검수는 그대로 요구한다. 기존 C3/선행 결과를 후속 소스의 통과로 재사용하지 않는다. 루나 독립 검수와 아스트라 통합 수용 전 전체 Verified로 기록하지 않는다.

현재는 exact test 경로와 named 행의 계약 초안까지다. 정상 결과 발급과 제품 실행 권한은 열지 않았으며, 미결정과 집중 이름/count는 승인 검토를 기다린다.

## r2 기술 선택·규범 보정 후보

이 절은 앞의 역사적 A/B/C 미결정을 구체화하는 **Draft 개정 문장**이다. 원본 Review 계약 305~310행의 throw-only와 AC003/005/008의 충분성을 별도 정확 규범 검수한 뒤에만 Approved 전환을 검토할 수 있다. 실제 실행·반례·P1 전체 폐쇄를 발급하지 않는다.

보존한 검토는 루나 `docs/verification/2026-09-29-c4-implementation-draft-luna-review.md` SHA `A1CE7289A69C2C719B5067C0CE8D7B9ACCDBB237406D23F03E4C94CAB684F079`, 솔 `docs/proposals/2026-09-29-c4-fixture-seam-decision-candidates.md` SHA `F0B6897D9481D73107B94D70592A2F091F5557C7DE6AC805FB90E9EDA2D5DB33`, 솔 `docs/proposals/2026-09-29-c4-actual-reentry-and-thread-boundary-candidates.md` SHA `F42A96EE86AD400588D2A1F36FDAAF7628C57AEACC8CFD30E390CC1AA3B11223`다.

### A: 단일 실제 C1→C2 준비 경계

정상 내부 3인자 `ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed, DesktopProfileLaunchAdapterV1 adapter, InputRouter router)`는 고정 NoOp만 사용한다. 새 내부 시험 overload/control에만 `PostDiskPreparedValidatedPreC2` 한 정지 지점을 둔다. 실제 C1 반환 전체와 DiskPrepared/원본 proof/pair/root를 검증한 뒤, 두 실제 가드가 결속된 상태이며 실제 C2 진입 전이어야 한다. 일반 27개 checkpoint와 기존 C1/C2 throw-only 권한은 넓히지 않는다.

한 invocation당 한 번만 동기화 정지한다. bounded wait는 원래 180초 제한 안에서 timeout 및 실제 작업 여유를 남기며 구체 시간은 승인 시 고정한다. fixture worker 준비 완료 뒤 executor는 원래 스레드에서 재개한다. 취득 실패·timeout은 정상 Busy로 제조하지 않고 terminal 오류 및 C2 미호출로 기록한다. 모든 경로에서 finally worker 신호·lease 해제·join과 정상 fixture 정리를 수행한다. 재진입 callback에서 worker cleanup을 실행하지 않는다.

허용 실제 준비는 두 가지뿐이다. `AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal`은 독립 worker가 실제 임시 root의 기존 lease를 취득하여 C2 실제 Acquire가 Busy를 반환할 때까지 보유한다. `AC007_ActualC2UnsafeLockPathManualRepairIsTerminal`은 실제 임시 `profile.operation.lock` 파일을 삭제하고 같은 경로를 디렉터리로 만든 후 원래 C2를 실행한다. marker 손상 대안은 이번에 채택하지 않는다. C1/C2 typed 결과 대체·control의 가짜 Busy/ManualRepair 반환은 금지한다.

근거는 기존 `ProfileResetMemoryCutoverDataV1Tests.cs` SHA `9F61A4EB2278C8069742BDA56BCF13140E1B717826908D5D6BC64B60B9EEB8A0`의 47~58행 실제 held lease, 63~70행 lease 이후 주입 Busy는 Reload인 구별, 188~196행 실제 unsafe lock 조건이다. 현재 C2 SHA `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`의 149~151행만 실제 Acquire Busy를 반환한다. 새 전체 bridge에서 양측 terminal·fresh 0회·receipt 없음·원본 proof/역사 보존을 별도 실제 결과로 확인해야 한다.

### B: 순수 검증과 실제 참조 손상 증거 분리

production 공유 closed-row validator는 실제 C1 typed row의 전체 outcome/state/허용 조합을 순수하게 검사한다. 발급·소비·상태 변경·C1/C2 호출은 하지 않는다. 실제 Begin 반환값의 boxed copy에만 top-level default/unknown/부적합 scalar 조합의 음성을 주입하여 같은 validator가 거절하는지 검사한다. 정확 필드/값은 승인된 AC003 행에서 고정하며 정상 값을 새 권한으로 만들지 않는다. 실제 실행 저장값을 바꾼 full bridge 시험이라고 표현하지 않는다.

production의 모든 proof 접근·C2 분기는 위 검증 성공에 의해 지배되어야 하고 검증 뒤 row 교체가 없어야 한다. 실제 source 줄·SHA의 지배 구조 보고서를 별도 증거로 요구한다. 순수 음성행과 지배 구조의 조합이 AC003을 충족하는지 아스트라/루나의 **정확 규범 검수**가 남는다. 실제 full bridge top-level 손상을 요구하는 해석이라면 현재 선택으로 P1 B를 닫을 수 없다. actual-holder 대안은 채택하지 않는다.

추가 내부 test-only observer는 두 actual 참조에만 각 1회 음성 손상을 허용하는 별도 amendment 후보다. 실제 C1 PreparedProof는 sealed class이며 C1 결과는 readonly struct다(C1 SHA `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, 257~259/445~447행). C1 검증 전 observer가 실제 nested PreparedProof의 `_root=null` 하나만 주입한다. 실제 C2 반환 후 mapping 전 observer가 actual `ProfileResetMemoryResultV1`의 `_outcome=default` 하나만 주입한다(C2 36~37행 원본 CWT와 대조). 정확 declaring type/field/type/허용값만 결속한다.

observer는 값 return·ref/out·supplier·결과 교체·global hook·normal proof issuer·registry 추가/복구를 제공하지 않는다. by-value C1 전달은 실제 struct 저장값 변경 통로가 아니며 nested 동일 참조만 손상한다. C2 observer 행은 C2 실행 후 손상이며 C2 미호출 증거로 세지 않는다. 정상 3인자는 fixed NoOp이고 결과/proof 복구·재사용도 금지한다.

| 새 Edit named 행 | 증거·기대와 한계 |
| --- | --- |
| `AC003_ActualBeginBoxedCopyClosedRowValidatorRejectsDefaultUnknown` | 실제 Begin row의 복사값만 음성 변조. 순수 validator 거절과 source 지배 구조의 분리 원장; full bridge 저장 손상 아님 |
| `AC003_ProductionValidatorDominatesEveryProofAndFinalizeBranch` | 최종 source의 모든 경로/예외 경계 정적 검수. NUnit 행만으로 의미 지배 증명을 대체하지 않음 |
| `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize` | 실제 참조 `_root=null` 단일 손상→terminal·C2 0회·fresh 0회. 정상 권한 제조 없음 |
| `AC007_ActualC2OutcomeDefaultRejectsComposition` | 실제 C2 객체 `_outcome=default`→mapping 무결성 거절·terminal. 같은 실제 receipt를 정상 성공으로 노출하지 않음 |

### C: 기존 실제 canceled와 관리 스레드 출생 증거

신규 독립 Play fixture가 실제 owned UI Submit을 정상 device 이벤트로 누르고 C2 Disable까지 held/in-progress 조건을 입증한다. 실제 `canceled` 이벤트 구독자를 fixture에만 등록하고 동일 이미 C4 소비된 confirmed request·원래 pair로 정상 3인자 executor 재호출을 한 번 시도한다. 새 제품 callback·testhook·phase 대입·callback 직접 호출·가짜 frame을 만들지 않는다. 원래 발행별 Presenter 소비·opaque intake/Confirm/Commit을 유지한다.

InputSystem1.20.0의 `InputActionState.cs` SHA `F75867163F293FFBAA4829154C55DC95A5EE4A72BD36623849C427D810D350A7` 1178→889~929→2724→2771은 진행 상태의 실제 Disable 취소 전달 경계다. Router 현재 SHA `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` 410행 캡처 억제/418행 old Disable/419~421행 구독 제거 및 1373행 자신의 callback 거절과 구분한다. 기존 Play 마지막 release를 유지한 채 진행 상태를 주장하지 않는다. 새 가드 구현이 먼저 Disable하면 실제 발생 지점을 다시 원장화하며 C2 사건으로 허위 집계하지 않는다.

`AC008_ActualOldSubmitCancelReentryRejectsConsumedRequest`는 실제 same-thread canceled 도달·늦은 clean 거절·두 가드 유지·C1/C2 각 1회·proof 중복 소비 없음·receipt/frame forensic 이력 보존을 확인한다. callback에서 실패/거절을 기록하고 바깥에서 assertion한다. Dispose나 release 후 event 없는 구간은 별도 구조 증거이며 same-thread 실행으로 합산하지 않는다. pre-auth 지연 worker의 모든 순서도 이 행으로 대체하지 않는다.

첫 C4 실행의 다른 스레드 문제는 새로운 **planned 상태**로 한정한다. lower 실제 C3 발급 witness에 `Thread.CurrentThread` 객체 참조와 별도 actual issued-event birth anchor를 추가하는 후보다. 현재 그런 필드는 없다. 숫자 thread ID·caller token·새 root/proof issuer를 만들지 않는다. 기존 실제 CWT/pair/issued 이력의 관리 참조 인증과 birth witness/anchor 일치 확인 뒤, 원래 스레드만 최초 Unity preflight와 실제 소비를 허용한다. 이미 C4 소비된 요청은 관리 원장을 먼저 읽어 비변경 거절한다. 정상 wrong-thread는 Unity 조회/소비/가드/C1/C2 전에 clean 거절하며 원래 스레드의 뒤이은 정상 실행을 보존한다. gate 획득 뒤 같은 actual pending/birth/미소비 인증을 재확인한다.

`AC001_ActualWrongThreadFirstCallRejectsBeforeUnityAndConsume`와 `AC001_OriginalBirthThreadExecutesSameRequestAfterWrongThreadReject`는 실제 같은 opaque·원래 pair를 사용한다. `AC008_ActualConsumedWorkerRejectsBeforeUnityAccess`는 post-consumption 실제 늦은 worker만 증명한다. 정상 첫 실행 worker와 late consumed worker를 혼동하지 않는다. thread 참조의 단일 음성 projection 손상은 정상 wrong-thread와 다른 무결성 오류이며 fake issuer·anchor 복구는 금지한다.

### 관리 전용 terminal latch: 구현 전 미결정

규범 보정 후보는 다음과 같다. **등록된 원본 witness/projection 무결성이 손상되면 actual original pair의 관리 전용 terminal latch가 즉시 모든 해당 writer/capture의 권한을 차단한다. worker에서는 Unity native 조회·actions Disable/Dispose를 호출하지 않는다. native cleanup은 원래 Unity 스레드의 정상 경로에서만 수행하며 실제 미실행 cleanup은 미실행으로 기록한다.** 단순 bool 두 개 순차 쓰기로 양측 차단을 인증하지 않고 실제 pair에 결속된 private monotonic latch의 atomic terminal 사건을 두 가드·writer·callback admission이 읽어야 한다.

현재 기능을 재사용했다고 주장할 수 없다. Adapter 589행 `LatchResetCutoverFailure`는 Router로 넘어가며, Router 482~515행은 502~515행에서 실제 Disable/RemoveCallbacks/Dispose를 수행한다. 따라서 기존 FailStop을 worker에서 호출하는 안은 배제한다. 해당 source SHA는 위 Router 및 Adapter predecessor `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`다.

현재 허용 Adapter/Router 범위 안에서 새 private 관리 latch·원본 pair anchor·native-cleanup pending 및 모든 기존 admission의 선행 검사를 만들 수 있는지는 **미결정**이다. 특히 기존 C1/Profile 직접 writer가 그 latch를 모르므로 “모든 writer”를 bridge 경로의 writer만으로 축소하지 않는다. 실제 issued root에 대한 기존 관리 root lease/barrier·입구 차단만으로 즉시 전체 차단을 증명할 수 없으면 C1/Profile 금지를 넓혀 구현하지 말고 이 요구를 차단으로 남긴다. 원래 스레드로 cleanup을 자동 예약하는 새 전역/Unity dispatcher 권한도 만들지 않는다. 실제 atomic 원본 pair 결속·메모리 가시성·각 writer/capture 도달 경계를 설계 검수하기 전 구현 준비 완료로 선언하지 않는다.

신규 `AC001_ActualBirthProjectionDamageLatchesManagedOriginalPair` 후보는 actual issued witness의 승인된 단일 필드 음성만 사용하고, worker의 Unity/native cleanup 0회·실제 original pair writer/capture 차단·원래 스레드 cleanup의 실제 수행 여부를 분리한다. 필요한 경로가 허용 밖이면 실행 행을 만들어 통과시키지 않는다. C3 기존 단일 필드 원본/append history 보장을 보존하고 모든 private 필드 동시 공격 범위로 확대하지 않는다.

## r2 정확 변경 후보 범위와 수용 전 잔여

기존 8개 runtime 경로와 신규 5개 시험/fixture 및 각 `.meta` 후보를 그대로 유지한다. 추가 허용 내용은 다음 표만이며 아직 승인되지 않았다.

| 기존 정확 후보 경로 | r2 추가 내용 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` 및 신규 `.meta` | fixed NoOp 3인자·내부 control·단일 A 정지·순수 row validator·정확 B observer·C4 actual consume 원장·관리 인증/종료 조율 후보 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | 실제 발급 witness/issued anchor의 private birth 참조·C3 원본 역사 유지. 새 정상 callable issuer 없음 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`, `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 원래 가드 후보 내에서 관리-only atomic original-pair 차단의 가능성 검토와 원래 스레드 cleanup 분리. 불가능하면 미결정 유지 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs`, `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` 및 각각 신규 `.meta` | B 분리 원장 및 birth/late managed 검사. 원본 fixture 미변경 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs`, `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` 및 각각 신규 `.meta` | A 실제 worker/unsafe lock·C 실제 held Submit/canceled·실제 cleanup 증거. 새 제품 seam 없음 |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` 및 신규 `.meta` | exact control/observer NoOp·무발급/무대체·관리-only 경계·Q0/Q-A/Q-B successor 검수 |

Q0/Q-A/Q-B 및 기존 C3 birth/history의 정확 amendment는 앞의 허용 후보와 별도 규범 승인으로 남는다. 원래 14 pin·Q0 현재 pin·감사 원문·240/15/562/51/610 정식 이름/필수 실행·180초는 보존한다. 새 named 행은 최종 선언 후 source/meta SHA·정식 NUnit 이름·예상 수·실제 사건/구조 구분 원장으로 동결하며 이번에는 정식 이름이나 실제 count를 발급하지 않는다.

남은 핵심은 AC003의 순수 boxed-copy 음성+production 지배 구조 조합에 대한 규범 검수, A 정지/B 참조 observer/C3 birth 상태의 exact amendment, 그리고 허용 Adapter/Router만으로 모든 writer/capture를 즉시 막는 관리-only containment의 가능성이다. 이 잔여와 C3 선행 whole gate를 통과하기 전 C4 Approved·구현 Ready·P1 전체 폐쇄가 아니다.
