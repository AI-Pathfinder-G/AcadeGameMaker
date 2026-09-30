# C4 r4 정확 구현·시험·감사 통합 개정 계약

- 상태: **Approved — 제한 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-29, 아스트라, 실제 `gpt-6-astra`. 설계 작성자는 솔, 실제 `gpt-6-sol`.
- 승인 근거: [C4 r4 구현 계약 승인](../../approvals/2026-09-29-c4-r4-implementation-contract-approval.md). 독립 검수 원문은 `2026-09-29-c4-r4-exact-implementation-amendment-reviewed-draft.md` SHA `EF54AE99EEF6E482C267C949A0C1B53709FF19611E9791300E07764EBD9256C0`로 보존한다. 아래 초안 작성 시점의 Draft·미승인 문장은 그 시점의 이력이며 현재 구현 권한은 이 상태와 승인 기록을 따른다. 동작·서명·범위·시험 요구는 검수 원문과 같다. 실제 Unity 실행은 최종 소스·도구·입력·선택·계획 동결과 독립 사전 검수 후 아스트라가 별도 배분한다.
- 소유 범위: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`; 공동 최종 수용 `AC-M5D7QC3-007/008`.
- 이 문서는 C4 개정 범위의 규범 소유 문서다. 아스트라의 Approved 전환 전에는 구현하지 않는다. 제안·검토 문서는 근거와 역사이며 이 문서의 규범 권한을 위임받지 않는다. 원본 C4의 개정하지 않은 동작도 아래 요구·기준으로 직접 보존한다. 원본 Review 문서의 상태나 문장은 이번 작성으로 바꾸지 않는다. 아스트라 승인 때 원본의 적용 관계를 별도로 기록한다.
- 통합 선행 근거: 원본 C4 SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`; 정확 개정 초안 `ACC8F20DE5D4D4E21F1385C5E204DC2D79BFB76F35CE12F7A439DC3599217738`; 결과 구성 r2 `B1B5A36757595F9DB993950819A61DB72ECBE44FFBDB369CA45D5C674FED50F6`; 규범 선택 한정 수용 `178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692`. 이 문서 본문에 서명·행·허용 범위를 통합하며 이전 문서를 수정하지 않는다.


r4는 루나 검수 SHA `450B52AA9CE7332EA8CA480E22B8B4933FB99BAC9CAF07DEFF208ED4A9B54849`가 지적한 same Owner fresh 세대 전환 P1의 설계 보정이다. r3 원문 SHA `FB1BCEB6DC20CA4E5122CB3756A559EC6FEB57858B8F6A6A1E587A85A254F4D1`은 검수 원본으로 보존한다. 상태는 Draft이며 아래 규범을 실제 구현·P1 폐쇄·Approved로 주장하지 않는다. QA 제안 SHA `67271D88CEBBDD4A688F36F7FB766B19DA20FEE060BECD7122A33AC1B9E9E26A`의 기술 선택은 별도 소유 Draft `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md` 및 이 문서 QA 부록에 직접 고정한다.
## 현재 선행 수용과 권한

[C3 R11 실행 직전 통합 수용](../../approvals/2026-09-29-c3-r11-pre-c4-integration-acceptance.md) SHA `92343D3C5AE287B1D4F6B939CFDECD1B6424E6E11B9D29CB6AD4C19F64808F89`에서 C4 선행 단계 gate가 완료됐다. Edit240은 matrix91과 나머지149의 서로소 실제 통과이며, Play15·필수 Edit562·worker51·Play610도 실제 전체 통과했다. native/QA/outer 종료값0, 이름 차이·중복·실패·skip·inconclusive0, 각 입력884의 전후 및 실행 간 SHA 차이0, 내부377행의 planned/pass/matched377·failed0을 보존한다. 내부 행은 추가 NUnit 사례가 아니다. 독립 최종 루나 SHA `0DF2C42F361B5676FAE13EF11F031BAF3BC4278EB33098CF866857E766AEFC6A`, 필수 회귀 루나 SHA `C6CBD31A22CC28C542CC7D7071FF1741ECA959FA03D37A65465D97889CD12220`는 P0/P1=0이다.

위 C3 선행 수용은 C3 전체 Verified나 게임 코드 게시 승인이 아니다. C3 AC007/008은 실제 C4 결과 출처와 공동 최종 gate에서만 닫힌다. 현재 C4 개정 구현의 소유 계약은 이 Approved 문서와 Approved QA 증거 규약이다. 원본 C4 Review와 이전 Draft는 이력이며 현재 구현 권한을 대신하지 않는다. 과거 문서의 610 대기 문장도 역사다. 새 C4 source에서 집중·필수 회귀를 다시 수행해야 하며 과거 통과를 재사용하지 않는다.

## 요구와 수용 기준

| 요구 | 보존할 규범 |
| --- | --- |
| REQ-M5D7QC4-001 | 실제 C3 execution commit이 모든 decision/rearm 권한을 폐쇄한 뒤에만 그 정확한 confirmed 객체를 C4가 한 번 소비한다 |
| REQ-M5D7QC4-002 | C1 이전에 정확 pair/root/generation 실행 가드를 설정하며 예외·durable barrier가 옛 세션 writer 권한을 되살리지 못한다 |
| REQ-M5D7QC4-003 | 원래 identity로 실제 C1 Begin 한 번만 호출하고 전체 typed 결과를 스칼라 재구성 없이 보관한다 |
| REQ-M5D7QC4-004 | 검증된 C1 Busy/NoBarrier 또는 ConfirmationStale/NoBarrier만 fresh C3를 허용한다. 불확실하거나 barrier 가능성이 있는 행·예외는 terminal이며 Cancel/rearm/retry를 금지한다 |
| REQ-M5D7QC4-005 | 동일 actual DiskPrepared proof/root/pair로 C2 Finalize 한 번만 호출하며 전체 C1/C2 상관을 보존한다 |
| REQ-M5D7QC4-006 | C1/C2 사이 lease 공백에서 disk는 durable barrier, 옛 memory/input 권한은 세션 가드로 보호한다. lease나 옛 profile 권한을 재사용하지 않는다 |
| REQ-M5D7QC4-007 | UI 목적지·scene·map enable·gameplay·media·ordinary save·archive cleanup 및 다른 제품 권한을 추가하지 않는다 |

| 수용 기준 | 필수 규범 |
| --- | --- |
| AC-M5D7QC4-001 | exact/duplicate/consumed/foreign root·pair·generation/default/반사 손상 및 C3 미commit 행은 유효 commit 전 C1 0회를 증명한다. 지원하지 않는 스레드는 아래 관리 문맥 거절 규범을 따르며 payload 검증 완료를 주장하지 않는다 |
| AC-M5D7QC4-002 | 양측 가드 전·사이·뒤 fault에서 semantic/menu/notification/save 발행0과 부분 진입의 영구 폐쇄를 증명한다 |
| AC-M5D7QC4-003 | 원래 identity로 실제 Begin 1회. outcome/state의 actual Begin boxed-copy 순수 음성과 모든 proof/C2 분기 지배 증거를 함께 요구하며 실제 중첩 proof·C2 객체 손상은 별도 전체 실행 증거다. 스칼라 대체·손상 행은 C2로 진행할 수 없다 |
| AC-M5D7QC4-004 | 실제 C1 Busy와 Stale만 fresh handback/cursor 기준선을 만든다. 소비된 request/identity 재시도 금지, Manual/Reload/throw의 fresh 금지 |
| AC-M5D7QC4-005 | 동일 proof/root/pair로 C2 1회. foreign/replaced/consumed proof·pair/root drift는 C2 변이 전 terminal |
| AC-M5D7QC4-006 | 모든 실제 C1 durable checkpoint·결과 반환·C2 진입 앞/내 fault에서 barrier의 disk 차단과 가드의 옛 memory/input 차단을 구분해 증명한다 |
| AC-M5D7QC4-007 | 실제 C2 Completed/Busy/Reload/Manual/throw 행에서 Completed만 같은 receipt를 보유하며 DiskPrepared 이후 모든 비완료는 terminal·Cancel/rearm/retry 금지 |
| AC-M5D7QC4-008 | 실제 disable/destroy/재진입/경합에서 C1/C2 한 번·proof 중복 소비 없음·forensic 이력 불변. unsupported thread 및 trusted anchor 손상은 아래 제한 문맥 규범을 따른다 |
| AC-M5D7QC4-009 | lower는 Hub 타입/public ABI/asmdef/friend를 추가하지 않는다. UI/prefab/scene/map enable/gameplay/run/wardrobe/media/Settings/Quit/save/archive/network/clock/random 권한 없음 |
| AC-M5D7QC4-010 | 새 focused와 필수 회귀에서 failed/skipped/inconclusive0, 정확 names·full rows·동일 입력·actual native/QA/outer0, 루나 독립 P0/P1=0 뒤 아스트라 통합 수용 |

추적은 REQ001→AC001/008, REQ002→AC002/006, REQ003→AC003, REQ004→AC004/007, REQ005→AC005/007, REQ006→AC006/008, REQ007→AC009/010이다. 아래 A/B/C 및 Q-A/Q-B/Q0 개정은 이 요구의 정확 제한 개정이며 제품 권한 확장이 아니다. 구현 중 제품 범위 확대가 필요하면 사용자 결정이 필요하다. 기술 충돌·구현 불가능·검수 실패는 아스트라에게 보고하고 해당 구현을 중지한다.

## 정확 runtime 허용 범위

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

lower fresh의 네 서명·결과 구성·현재 권한과 역사 분리는 이 문서의 뒤 절에서 직접 규정한다. 추가 overload·권한 getter·스칼라 권한을 구현자 재량으로 만들지 않는다.

## 신규 시험의 정확 경로·class·meta·REQ/AC

각 신규 `.cs`에는 같은 경로의 `.cs.meta`만 동반한다. 기존 fixture/class에 행·도우미를 추가하지 않는다.

| 신규 경로 | class/namespace/assembly |
| --- | --- |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` | `AcadeGameMaker.Tests.EditMode.InputUnity.ProfileResetExecutionBridgeV1Tests`, `AcadeGameMaker.Input.Unity.EditMode.Tests` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` | 같은 이름공간 `C4ActualExecutionEditFixtureV1`, 같은 조립 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs` | `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetExecutionBridgePlayModeTests`, `AcadeGameMaker.Input.Unity.PlayMode.Tests` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` | 같은 이름공간 `C4ActualExecutionPlayFixtureV1`, 같은 조립 |
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

예외적 변경 범위는 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`뿐이다. 기존 AC011 메서드명/선택을 유지하고 역사 증거9행·불변 current legacy7행을 보존한다. 기존 C2/C2R successor2개를 역사 증거로 보존하고 **변경 Adapter/Router의 current exact C4 pin/provenance**로 분리한다. 구 SHA 또는 새 SHA fallback은 금지한다. 현재92~131행의 current2 비교는 C4 source가 달라지면 승인된 C4 pin과 독립 원문 증거만 비교해야 한다. Git 작업상태 판정·외부 경로·선언 scope·다른 Q0 행은 변경하지 않는다. 옛 C2 source에 현 SHA를 소급 귀속하지 않는다.

신규 Hub strict named 행은 `AC009_LowerExecutorHasNoHubTypeOrPublicAuthority`, `AC009_QBFreshAmendmentKeepsGameplayAndDurableTokensForbidden`, `AC009_QAFreshAmendmentPreservesOriginalTransferAudit`, `AC009_Q0SelectsOnlyExactChangedCurrentSuccessor`, `AC010_PredecessorBodiesAndRequiredSelectionNamesRemainExact`다. 기존 strict body를 추출·복원하면 실제 정확 영역/SHA를 대조하고 신규 fresh 전체 body의 금지/상호 actual 예약/이력 분리도 별도 검사한다. 옛 body 통과만으로 fresh 동작 검수를 면제하지 않는다.

원본 bytes 신규 저장 범위는 `docs/evidence/c4-audit-predecessor/HubUiOnlyQ0ScopeAuditEditModeTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubMenuIntentHandoffEditModeTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubMenuIntentHandoffC3AuditSuccessorTests.cs.txt`, `docs/evidence/c4-audit-predecessor/HubPresentationScopeAuditTests.cs.txt`뿐이다. 승인 뒤 exact predecessor SHA와 전체 byte를 저장하며 원본 docs137/기존 evidence는 변경하지 않는다. Q0의 신규 actual current pin 입력 범위 `docs/evidence/c4-audit-current-source-pins.json`은 source freeze 뒤 생성하고 exact SHA/승인 문서 결속을 요구한다. 미수용 source를 accepted로 표시하지 않는다.

## 현재 predecessor와 신규 source/meta 동결

R11 14파일 원장 `artifacts/c3-upper-r11-frozen-source-manifest.json` SHA `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`는 불변 선행 원장이다. 이 문서의 아래 표에 14개 전체 경로와 SHA를 직접 보존한다. Q0 원본 SHA `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4`, Adapter `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`, Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, Q-A scope `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`를 역사 pin으로 보존한다.

새 C4 source/meta 최종 SHA는 구현 뒤 신규 원장에 동결하고 R11 fixed14를 새 source로 덮어쓰지 않는다. 변경 runtime7+신규 runtime/meta+시험 fixture/meta+Q0 audit+신규 도구/evidence의 정확 전체 bytes를 새 input path 목록에 포함한다. 불변 기존 meta도 타입/asset identity 입력으로 유지한다. 허용 밖 source 변경은 실패이며 통과 비교에서 무시하지 않는다.

## 새 QA 프로토콜과 정확 도구 범위

아래 신규 도구만 구현 범위로 둔다. 기존 `artifacts/c3-build-focused-selection.ps1`, `artifacts/c3-build-required-decision-expected-rows.ps1`, `artifacts/c3-verify-decision-rows.ps1`, `artifacts/c3-final-selected-run-r3.ps1`, `artifacts/c3-r11-final-validation-queue.ps1`, `artifacts/c3l-capture-inputs.ps1`, `artifacts/c3l-verify-run.ps1`, `qa/tools/Invoke-UnityQa.ps1`은 변경하지 않는다. 기존 script를 C4 stem으로 개조하지 않는다. old run-r3는 `^c3-...$` stem과 old capture를 사용하므로 C4 runner로 대체할 수 없다.

| 신규 경로 | 정확 역할·계약 |
| --- | --- |
| `artifacts/c4-build-focused-selection.ps1` | 신규 Edit/Play/Hub class의 전체 Test/TestCase/UnityTest에서 exact qualified names·dedupe·anchored selector를 생성한다. bool/int/string/enum의 실제 NUnit 표기를 지원하지 못하면 실패하며 누락시키지 않는다 |
| `artifacts/c4-build-required-checkpoint-rows.ps1` | AC row/throw27/C1의51/C2의19/A/B/C의 actual authority/outcome/phase/ExpectedCase/증거 종류·순서를 생성한다. 미도달 계획도 구분한다 |
| `artifacts/c4-capture-inputs.ps1` | 정확 신규 입력 목록의 source/meta/assets/settings/packages/qa/필요 docs/tool/evidence/selection/checkpoint에 대해 같은 path/hash를 기록한다. 실행 출력/XML/log/queue 진행 자료는 입력에서 제외한다 |
| `artifacts/c4-verify-run.ps1` | 실제 XML full names/results/count·actual native exit/QA return/outer exit·source 전후·신규 freeze를 검증한다. native0/QA0 및 별도의 실제 native 증거가 필수다 |
| `artifacts/c4-verify-required-rows.ps1` | full row의 planned/reached/result/authority/ref/call/guard/barrier/history와 실제 NUnit CaseResult를 결속한다. pure/structure/full bridge를 구분하며 내부 passed만으로 NUnit 실패를 수용하지 않는다 |
| `artifacts/c4-final-selected-run.ps1` | C4 stem만 허용하고 정확 선택+before capture→기존 InvokeUnityQa 한 번→actual exit+after capture→stdout partial 반환 순서다. 큐가 실제 runner exit 관측→최종 QaReturn CreateNew 작성→두 verifier 호출을 수행한다. 기존 결과 덮어쓰기·진행 중 별도 Editor 기동은 금지한다 |
| `artifacts/c4-final-validation-queue.ps1` | 단일 queue/lease로 C4 focused와 기존 불변 선택을 신규 source에서 순차 실행한다. 중단/실패를 성공으로 바꾸지 않고 same inputs를 실행 간 대조한다 |

실제 Unity 실행은 변경하지 않은 `qa/tools/Invoke-UnityQa.ps1`만 runner가 `-ProjectPath/-UnityPath/-TestPlatform/-TestFilter/-ResultsPath/-LogPath/-AwaitSeconds`로 호출한다. 현재6000.6 설정·기존 owned Editor 대기 방식·licensing/network 설정을 변경하지 않는다. `AwaitSeconds`는 관찰 창이며 NUnit180초를 늘리지 않는다. run-r3/capture/Invoke의 현재 SHA는 각각 `B66C0BB96D2DB9240D416C043F4104291E4150D6229154BB3F744E8ECB77230B`, `E95A0E908BFFDBEB22577295384D22D013F2F8488B6EE676395CA26944424CD1`, `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`다.

기존884는 R11 capture 사실이며 C4의 고정 count가 아니다. 기존 source에 승인된 C4 변경/신규 입력을 반영한 정확 새 path 목록을 구현 뒤 동결하고 dedupe 길이로 새 count를 발급한다. 모든 실행에서 같은 paths/hash를 전후·상호 대조한다. 전체 Assets 등 기존 포착 범위를 숨겨 좁히지 않고 기존 capture의 필요한 source/settings/assets/qa와 직접 읽는 docs/resources를 유지하며 C4 입력을 추가한다. 실행 생성 출력은 명시 제외한다. count 차이를 설명하는 path별 추가/제외 사유 원장이 필수다.

생성 범위는 `artifacts/c4-focused-edit-selection-v1.json`, `artifacts/c4-focused-play-selection-v1.json`, `artifacts/c4-focused-hub-selection-v1.json`, `artifacts/c4-required-checkpoint-rows-v1.json`, `artifacts/c4-frozen-source-manifest-v1.json`, `artifacts/c4-frozen-input-paths-v1.json`, `artifacts/c4-predecessor-selection-name-map-v1.json`, `artifacts/c4-final-validation-queue-plan-v1.json`, `artifacts/c4-final-validation-queue-result-v1.json`뿐이다. 각 run은 `artifacts/c4-<승인stem>.xml/.log/-source-before.json/-source-after.json/-native-exit-observation.json/-qa-tool-return.json/-verification.json/-required-row-comparison.json`을 새 불변 출력으로 남긴다. 실제 run stem 목록은 생성 queue plan을 아스트라/루나가 정확 동결한 뒤 실행 전에 확정한다. 미승인 stem과 기존 증거를 덮어쓰지 않는다.

## 실행·수용 gate와 잔여

기존240 Edit/15 Play/562 Edit/51 worker/610 Play의 exact qualified names와 순서를 유지한다. 240의91 matrix+149 remaining 서로소 내역도 원장에 보존하며 새 C4 focused에 흡수·삭제·개명하지 않는다. 최종 method/enum/TestCase source에서 NUnit names/count를 생성하고 selector를 동결한 뒤 실제 XML의 missing/extra/dup/skip/inconclusive/failed를 모두 대조한다. 사례180초·native0·QA0·outer0·row full records·입력 차이0·전체 선택 통과·루나 새 source/실행 독립 P0/P1=0·아스트라 통합 수용을 요구한다. 과거 R11 통과를 변경 source의 통과로 재사용하지 않는다.


## actual 기존 연결과 소유 범위

현재 lower `ProfileNewGameConfirmationV1.cs` SHA `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1`은 132행 `IssuedConfirmedRequests` CWT와 69~78행 witness의 실제 Adapter/Router/Owner/Epoch/InteractionGeneration/DecisionGeneration/Root/SourceResult/SourceWitness를 연결한다. 301~309행 실제 발급과 350~383행 RequestIssued/MintIssued/CWT history는 원본 증거다. `NewGameConfirmationOwnerV1`이 현재 lower Complete에 전달하는 `this`, `_epochToken`, `_epoch`, `_generation`은 같은 원본 Owner/Epoch/InteractionGeneration/DecisionGeneration 연결이며 숫자만으로 권한을 발급하지 않는다.

현재 C3 `Completed`는 상위 reserve/commit 완료다. 새 C4 consume는 그 same opaque의 실제 executor 단일 소비 사건이다. fresh handback의 Available/Reserved/Completed/Closed는 다시 별도 사건이다. 어느 하나를 다른 단계의 소모 플래그로 재사용하지 않는다. old C3 완료/폐쇄·원래 epoch·taken/cursor 이력은 지우지 않는다.

lower는 Hub 타입·presenter·Q-B 객체를 참조하지 않는다. lower가 받는 `object ownerToken`은 기존 witness.Owner와 같은 **원래 Owner 인스턴스**뿐이고 epoch token도 기존 actual association 또는 같은 Owner가 단일 fresh 작업에서 만든 다음 token 참조뿐이다. 이 값은 caller proof나 identity가 아니며 실제 결과/예약 원본과의 참조 일치 조건이다. Q-A/Q-B/presenter 참조와 다음 Hub capability/reservation은 Hub private CWT가 소유한다. lower execution generation과 C2 memory generation, Hub interaction epoch/decision generation은 서로 다른 원장 값이다.

## 예정 결과 타입과 접근 가능한 구성

`ProfileResetExecutionResultV1`은 internal sealed actual-registered 결과다. getter는 아래 세 개만 제공한다. 정상 생성자는 lower private issuer 경로에 한정하며 unregistered candidate는 거절한다. 생성자의 구현 위치가 nested/private factory 또는 internal unregistered candidate 방식인지는 사적 선택이다.

```csharp
internal enum ProfileResetExecutionOutcomeV1
{ Completed=1, Busy=2, ConfirmationStale=3, ReloadRequired=4, ManualRepairRequired=5 }
internal enum ProfileResetExecutionPhaseV1
{ PreC1=1, C1Returned=2, C2Invoked=3, Completed=4 }
internal sealed class ProfileResetExecutionResultV1
{
    internal ProfileResetExecutionOutcomeV1 Outcome { get; }
    internal ProfileResetExecutionPhaseV1 Phase { get; }
    internal long ExecutionGeneration { get; }
    internal void Validate();
}
internal sealed class ProfileResetFreshC3HandbackV1 { }
internal sealed class ProfileResetFreshExecutionReservationV1 { }
```

두 opaque 타입에는 getter·공개 필드·소비 bool·root/owner/epoch/identity/proof 추출 API가 없다. handback은 execution result의 사적 구성으로만 보관하며 Hub는 result 전체를 lower에 다시 전달한다. reservation만 아래 reserve의 반환으로 Owner에 전달한다. C1 row/C2 result/C2 receipt를 result getter로 노출하지 않는다. 따라서 Hub가 결과에서 C1 identity/prepared proof/root를 역추출하는 경로가 없다.

enum의 값0/default 및 정의 밖 값은 모두 invalid다. `FreshC3Required`는 가드/생명주기 상태이며 execution outcome이나 phase에 넣지 않는다. 실제 C1 Busy와 ConfirmationStale는 서로 다른 Outcome getter 값·원본 typed row·실행 행으로 보존한다. `PreC1`은 원본 closed phase 구성원으로 유지하지만 actual typed C1 row를 받기 전 예외에서는 정상 result를 발급하지 않는다. 해당 단계의 fault/trace에 PreC1을 기록할 수 있을 뿐, 이 matrix에 정상 PreC1 반환 행을 만들지 않는다.

| 구성 | contract-visible 경계 | 사적 원본 구성과 검증 |
| --- | --- | --- |
| Outcome/Phase/ExecutionGeneration | 위 getter만. generation은 진단/원장 상관 값이며 권한이 아님 | CWT 원본의 exact 값과 projection을 대조. generation은 actual C4 consume 때 등록된 양수이며 caller 공급 금지 |
| provenance | 공개/internal getter 없음 | same actual confirmed request/witness·C3 Completed 원본·C4 consume 사건·원래 owner/epoch/pair/root·birth anchor·가드 사건을 private 원장으로 연결 |
| C1Row | getter 없음 | 실제 Begin 전체 `ProfileResetDiskResultV1` 값 그대로 보관. 재구성·대체 없음; 원본 값과 typed validator/전체 구성을 검증 |
| C2Result | getter 없음 | 실제 Finalize 반환 `ProfileResetMemoryResultV1` 동일 참조 또는 null. 원래 CWT/Validate와 outcome를 대조 |
| C2Receipt | getter 없음 | 실제 Completed의 `ProfileResetMemoryReceiptV1` 동일 참조 또는 null. 현재 C2 receipt는 79행 sealed class이며 기존 Validate/실제 final proof를 대조 |
| FreshOpaque | getter 없음 | actual C1 Busy/NoBarrier 또는 ConfirmationStale/NoBarrier 행에만 actual registered `ProfileResetFreshC3HandbackV1`; 그 외 null. 같은 실행 결과/C4 원본에만 결속 |

사적 필드의 구체 이름·CWT 보조 class 이름은 구현 선택이지만 위 접근성·closed matrix·원본 연결·단일 필드 projection 손상 탐지·append-only 사건은 계약이다. 결과 getter는 관리 원본/무결성 검증을 수행하되 이미 닫힌 pair에 현재 live Unity 결속을 다시 요구하지 않는다. forensic 결과 읽기가 새 authority 소비나 native cleanup을 일으키지 않는다. 실제 temp 실행의 forensic 관측은 승인된 정확 반사 읽기 또는 source 근거로 하며 새로운 getter를 추가하지 않는다.

**모든 getter와 Validate의 전체 검증 경계:** actual result CWT 등록과 동일 C4 consume/단일 result-issued 사건·원본 confirmed witness/C3 Completed 연결을 먼저 확인한다. 전체 enum/default/unknown·위 nullable matrix·same actual adapter/router/owner 및 정규화된 원본 root correlation·execution generation/guard 사건을 함께 검증한다. C1Row 전체 `Validate`와 actual C1 발급 상관을 검사하고, C2Result가 있으면 전체 원본 Validate/outcome를, receipt가 있으면 same actual C2 receipt/원래 root/해당 C2 세대·최종 correlation을 검사한다. fresh가 있으면 같은 actual handback/result/pair/owner/root/epoch 원본 연결도 검사한다. private projection과 원본 사건 불일치·foreign/중복 발급·반사 손상은 throw다. 동일 정상 forensic 결과를 여러 번 읽는 것은 중복 발급이 아니며 별도 live authority도 아니다.

C4 ExecutionGeneration과 C2 receipt의 memory generation은 서로 다른 원장이다. 숫자가 서로 같다는 검사로 권한을 만들지 않고 각각 실제 consume 사건과 실제 C2 원본 correlation에 결속한다. 닫힌 forensic 결과의 검증에서 현재 살아 있는 Unity 결속/현재 payload 조회를 새로 요구하지 않는다. 위 관리/typed 검증을 현재 pair 활성 상태 검사로 대체하지 않는다.

### closed nullable matrix

각 반환 결과에는 actual C1Row가 반드시 있다. 불가역 소비 이후 예외로 실제 typed row를 받지 못하면 결과를 만들지 않고 영구 fault 사건 기록·양측 종료 후 예외를 유지한다. 사전 외부 문맥 거절은 이 내부 fault 경계에 포함하지 않는다. 모든 사적 nullable 구성은 다음과 같다.

| execution Outcome/Phase | 실제 C1 row | 실제 C2Result | C2Receipt | FreshOpaque |
| --- | --- | --- | --- | --- |
| Busy/C1Returned | actual Busy/NoBarrier | null | null | 같은 실제 결과에 등록된 객체 1개 |
| ConfirmationStale/C1Returned | actual ConfirmationStale/NoBarrier | null | null | 같은 실제 결과에 등록된 객체 1개 |
| Completed/Completed | DiskPrepared/DiskPrepared | actual Completed | same actual receipt, nonnull | null |
| ReloadRequired/C2Invoked | DiskPrepared/DiskPrepared | actual Busy 또는 ReloadRequired | null | null |
| ManualRepairRequired/C2Invoked | DiskPrepared/DiskPrepared | actual ManualRepairRequired | null | null |
| ReloadRequired/C1Returned | actual ReloadRequired/DefaultCommitUncertain | null | null | null |
| ManualRepairRequired/C1Returned | actual ManualRepairRequired/ManualRepairRequired | null | null | null |

C2 Busy를 execution ReloadRequired로 조합하는 것은 fresh를 허용하지 않는 terminal 안내 분류다. 사적 C2 원본 Busy를 Reload로 바꾸거나 C2가 Reload를 반환했다고 보고하지 않는다. C1/C2 row·receipt·generation은 각 실제 원장 값으로 분리한다. 표 밖 조합·default/unknown enum·scalar 불일치·unregistered 결과는 거절한다. 손상된 실제 C2 object를 조합해 정상 결과를 발급하지 않는다.

matrix는 결과 발급 당시의 불변 구성이다. handback이 나중에 Reserved/Completed/Closed가 되어도 원래 result의 Busy/C1Returned 또는 ConfirmationStale/C1Returned와 same opaque 이력을 지우지 않는다. 결과 getter가 그 역사 값을 읽을 수 있다는 사실은 live authority가 아니다. Reserve/Validate/Complete는 별도 현재 handback/예약 사건을 검사한다. Completed 결과의 C2 receipt도 forensic evidence이며 현재 session을 다시 실행하거나 UI를 여는 권한이 아니다.

## Owner→lower 정확 fresh callable

메서드는 lower `ProfileNewGameConfirmationV1` internal partial에 둔다. 모든 parameter는 by-value이며 ref/out은 없다. 추가 overload/tuple/scalar 권한 반환은 금지한다.

```csharp
internal static ProfileResetFreshExecutionReservationV1 ReserveFreshExecution(
    ProfileResetExecutionResultV1 result,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken, object committedEpochToken,
    long committedInteractionGeneration, long committedDecisionGeneration,
    object nextEpochToken, long nextInteractionGeneration);
internal static bool ValidateFreshExecutionReservation(
    ProfileResetFreshExecutionReservationV1 reservation,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken, object nextEpochToken, long nextInteractionGeneration);
internal static void CompleteFreshExecutionReservation(
    ProfileResetFreshExecutionReservationV1 reservation,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken, object nextEpochToken, long nextInteractionGeneration);
internal static void CloseFreshExecution(
    ProfileResetExecutionResultV1 result,
    ProfileResetFreshExecutionReservationV1 reservation,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken);
```

`Reserve`은 actual Busy/C1Returned 또는 ConfirmationStale/C1Returned 결과·원본 committed C3 연결·실제 C4 consume·원래 owner/epoch/generation/pair/root·원래 thread·pending guard를 인증한다. `nextInteractionGeneration`은 원래 interaction generation+1이고 checked overflow는 terminal 오류다. `nextEpochToken`은 nonnull·old token과 다른 참조·원본 이력에 이미 사용되지 않은 같은 Owner 작업의 실제 신규 token이어야 한다. 사적 result/handback 원본 없이 값만으로 예약을 만들지 못한다. 반환 opaque는 해당 다음 token/generation/pair/owner/result/handback에 actual CWT 등록한다. 이 등록값을 getter로 되돌려주지 않는다.

`Validate`는 current 미완료 actual reservation만 true다. 정상 foreign/unregistered/완료/closed/늦은 조회는 false이며 완료 이력을 live permission으로 사용하지 않는다. 등록된 원본 projection 손상은 원래 thread에서 영구 관리 fault 사건을 기록하고 throw한다. 이 관리 validation 함수 자체는 consume/guard release·발행·native callback·Disable/Dispose를 수행하지 않는다. 원래 thread의 Owner/lower orchestration catch가 처음 보관한 immutable original pair로 양측 native closure를 수행한다. unsupported 최초 문맥 거절은 무소비·무변경 프로토콜 거절이며 fault 사건이 아니다.

`Complete`는 actual same reservation/pair/owner/next token/generation과 미완료 사건을 gate 안에서 재검증한다. 정상 이미 completed/closed loser는 비변경 거절한다. 성공은 lower handback 완료 사건을 영구 기록하고 **그 같은 private 완료 증거**로만 기존 exact Adapter/Router execution guard의 fresh 결속을 완료한다. 반환은 void다. 외부 callable guard release나 새 activation proof getter를 제공하지 않는다. pair 절반 결속 실패·기록/등록 실패는 Completed로 보고하지 않고 양측 영구 종료한다.

`Close`의 reservation은 reserve 이전/중간 실패에서 null일 수 있다. null이어도 actual result/handback의 원본 owner/pair를 먼저 인증하며 새 authority를 만들지 않는다. 등록됐던 reservation이면 같은 result 소속이어야 한다. 원본 사건에 Closed/fault를 선기록하고 양측 guard를 종료한다. caller의 손상된 pair projection으로 foreign action을 정리하지 않고 원본 등록 pair를 사용한다. 이미 종료된 동일 작업은 native 정리를 중복하지 않는다. actual foreign 문맥은 clean 거절한다. Hub 슬롯 종료는 lower가 Hub를 호출하는 방식이 아니라 Owner의 같은 catch/finally 경로가 수행한다.

## Hub 6개 서명과 게이트 순서의 결합

이 문서 앞 절의 여섯 Hub 서명을 그대로 적용한다. lower reservation은 Owner private 상태에만 보관하고 Q-A/Q-B에는 getter 없는 Hub capability/reservation만 전달한다. Hub CWT는 same actual lower reservation의 사적 연계와 owner/presenter/Q-B/router/next token/epoch를 기록한다. lower에는 presenter/Q-B object조차 전달하지 않는다.

순서는 Owner gate→원본 operation context 인증→lower Reserve→Owner의 Hub capability 등록→Q-B Reserve→Q-A Prepare→실제 Matches acknowledgment→Q-B Commit→Owner의 same Hub capability 단일 Consume/새 epoch 슬롯 결속→lower Complete→성공 상태 공개다. Owner의 새 슬롯은 lower Complete 전까지 외부 live permission이 아니다. lower가 Hub acknowledgment를 독립적으로 읽었다고 주장하지 않는다. 실제 Hub private orchestration의 ack/consume가 lower Complete 호출을 지배한다는 source 증거가 필수다. lower는 자신이 발급한 예약의 bearer authority와 원본 owner/pair/next token을 인증하고 Hub private 단계의 실행 사실은 해당 source 경계로 증명한다.

외부/native call 도중 gate를 유지한 채 무한 기다리는 registry lock을 만들지 않는다. lower 예약/완료 원장은 짧은 관리 lock/CAS로 선행 기록하며 native closure 단계는 영구 fault 사건 뒤 원래 Unity thread에서 수행한다. 실제 same-request canceled 재진입은 consumed/fault 관리 사건부터 읽어 inert 거절하고 winner를 Terminal catch로 다시 닫지 않는다. 정상 publication 재개는 완료된 same reciprocal 결속 뒤의 원래 Router 경로만 사용하며 map enable/교체·추가 빈 frame을 만들지 않는다. 신규 cursor의 첫 actual true frame 폐기를 유지한다.

partial failure의 Owner catch는 처음 인증한 original result/reservation/pair/owner를 immutable local operation context로 보관하여 lower Close를 호출하고 Q-A/Q-B 신규 슬롯도 닫는다. 다음 token/epoch와 original scalar 이력을 복구하여 재시도하지 않는다. 위 context는 이미 실제 발급된 참조의 보관이며 새 정상 issuer가 아니다. 정상 늦은 loser는 이 terminal catch 밖에서 거절하며 항상 Owner gate finally 해제다.

## Q0 pin과 input/tool의 비재귀 동결 순서

1. 승인 후 runtime·신규 시험/fixture·신규 QA 도구·감사 predecessor bytes를 먼저 최종화한다. source에 자신의 최종 SHA를 넣지 않는다.
2. Q0 current pin 입력 `docs/evidence/c4-audit-current-source-pins.json`에는 **변경 Adapter/Router 두 파일**의 exact SHA·원본 predecessor·변경 승인/검수 근거만 넣는다. Q0 파일 자신의 SHA·이 JSON 자신의 SHA·최종 source/input manifest SHA를 넣지 않는다. source freeze나 실제 수용이 끝났다는 상태도 미리 넣지 않는다.
3. Q0는 그 exact pin resource SHA/승인 provenance에만 결속하도록 한정 보정한다. Q0를 최종화한 뒤 source/meta manifest를 생성한다. source manifest는 최종 runtime/test/meta/Q0 및 변경 허용 사실을 포함하며 manifest 자신의 byte hash를 자기 행에 넣지 않는다. 외부 원장/검수 문서가 그 manifest SHA를 보관한다.
4. 최종 source로 선택/행 원장을 생성하고 parser 대조 후 input path list를 만든다. path list는 필요한 입력의 path만 정의하며 자신이나 run output을 입력으로 넣지 않는다. source manifest·pin JSON·선택·행·새 도구·직접 읽는 문서 자원의 목록을 유지하고 기존884에서의 추가/제외 사유를 보존한다. count는 dedupe된 실제 입력 수이며 현재 발급하지 않는다.
5. queue plan은 source manifest/selection/row/input path list의 외부 SHA를 결속한다. queue plan·queue result·각 before/after capture·XML/log/native/QA/row comparison·실행 후 review/수용 문서는 run input에서 제외한다. runner는 queue plan의 고정 SHA를 시작/각 실행 경계에서 별도로 검사한다. 따라서 input capture가 자신이나 결과를 재귀로 hash하지 않는다.
6. 각 run은 같은 고정 input paths의 현재 SHA를 before/after/상호 대조하며 새 C4 source에 R11 fixed14/884를 재사용하지 않는다. 실행 중 tool/doc/source를 바꾸면 실패다. 새 승인/검수 문서를 실행 뒤 발급하면 같은 run의 사전 입력인 척 하지 않는다. 새 근거를 실제 input으로 읽어야 한다면 새 버전 freeze와 새 실행을 요구한다.

Q0 current pin resource에 어떤 승인/정적 검수 문서를 직접 읽을지는 source freeze 전 고정한다. 그 문서는 자신이 참조할 최종 manifest SHA를 아직 포함할 수 없으므로 구현 변경 승인·두 runtime의 정확 바이트 정적 검수까지만 주장한다. 이후 전체 source/run 수용은 별도의 사후 근거로 분리한다. 이렇게 source→pin→Q0→manifest→selection/rows→input list→queue→run/result의 비순환 순서를 유지한다.


## R11 정확 선행 14개 바이트

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


## C1/C2의 실제 경계와 바이트 출처

C1 `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`의 현재 SHA `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`는 C3L 통합 수용 문서의 17행과 연결한다. C3L 수용 문서 SHA `1000E2752EAB63B496C5DA8491D698A8FE40D492497E039D3F96A725D09B582F`를 보존한다. 원본 C1 disk 계약 SHA `D03C77478D2EC4214A225E4EA46B993E59C753C6EB883BB0C913491D3068E455`와 과거 검수 SHA `A16DF015F805F73507B65378497F13DF02B637F711D53F4793FE5212B8EA4B00`를 현 바이트 수용으로 소급하지 않는다. 실제 C1 result는 445행 readonly struct이고 PreparedProof는 257행 sealed class다.

C2 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs`의 현재 SHA `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`는 `artifacts/c2-r36-source-before.json` 808~809행에 연결한다. 해당 원장 SHA `2CAB80CC20A01165778905F67161866906F439C6E5AAFC5E316D1813628F5EA4`, C2 Verified 계약 SHA `F76E9AF2B7A9A2E936A8E2253A3E1FDA7E94E164DC0FBCBC08BBF2BAA4D1F5DF` 503~523행 및 독립 C2/C2R 최종 검수 SHA `3EE81482D43D5039E3D412EF3C38400F6A7E4903541DCDE29D77E7BD3A733A8E` 17~27/43~60행을 보존한다. 이 수용은 합성 memory/input 범위이며 새 C4 실행의 검증을 대신하지 않는다. C2 result는 36행 sealed class, receipt는 79행 sealed class다. FinalizeReset의 정확 4/5인자 서명·proof 재인증·lease 재취득·staging/default/memory/barrier/receipt 단일 발급은 유지한다.

## 최종 동결·독립 검증

구현 뒤 exact signatures·closed matrix 전 행·getter 단일 손상·foreign result/reservation·same opaque 두 번째 reserve/complete·late consumed·partial registry 실패·원래 thread fault/native canceled·다른 next token/epoch·완료 전 live permission 없음의 실제 원장을 동결한다. pure validator/구조/전체 실행 증거를 구분한다. Hub ack/consume→lower Complete 및 validator→모든 외부 proof/C2 분기의 지배 구조를 별도 독립 검수한다. 실제 source/meta/tool/selector/checkpoint/입력 수와 NUnit count는 생성·parser 대조 시점에만 발급한다.

C3 AC007/008과 C4 AC001..010의 공동 final gate는 새 실제 C1/C2 결과·완료 receipt·fresh 후속세대·실제 경합/생명주기·focused 및 기존 불변 선택 전부와 루나 독립 P0/P1=0·아스트라 통합 수용을 요구한다. 새 입력에서 한 실행이라도 source/input 차이가 있거나 native/QA/outer 비영이면 수용하지 않는다. 기술 설계의 과거 한정 수용은 이 새 문서의 승인이나 실제 구현 수용을 대신하지 않는다.

## 승인 전 독립 검토 대상

필드명·private factory·CWT 보조 class는 사적 구현 선택이다. 메서드/type/property/행/검증 순서/허용 경로는 이 문서의 계약이며 선택적 구현 분기가 아니다. 아래 새 Owner 조율·guard 상호 호출 서명은 통합 과정의 구체화이므로 아스트라 승인 전에 독립 루나 검수를 요구한다. 새 normal issuer·공개 registry·Hub 역참조·worker native fail-stop·actual holder를 허용하지 않는다. 구현 뒤 실제 parser/NUnit 이름/count·source 지배·부분 완료 폐쇄 증거는 아직 없으며 생성·실행 전 동결할 사항이다. 현재 어떤 C4 구현·QA 실행·최종 수용도 발급하지 않는다.
## 기존 commit 전용 경계와 별도 Owner 실행 진입

현재 Owner SHA `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043`의 322~348행은 아래 commit 전용 서명이다. 원래 실제 lower Reserve→모든 상위 권한/epoch/Q-A/Q-B 폐쇄→same opaque Complete→finally Exit 순서와 반환 객체를 보존한다.

```csharp
// 기존 서명: C1/C2 호출을 추가하지 않는다.
internal ConfirmedProfileResetRequestV1 CommitForExecution(
    ConfirmedProfileResetRequestV1 request);
// 신규 예정 Owner 조율 진입: 이미 실제 commit된 반환 객체만 받는다.
internal ProfileResetExecutionResultV1 ExecuteCommittedReset(
    ConfirmedProfileResetRequestV1 confirmed);
// 신규 예정 lower 관리 조회: ProfileNewGameConfirmationV1 internal partial.
internal static bool IsExecutionAvailable(
    ConfirmedProfileResetRequestV1 confirmed,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken);
internal static bool IsExecutionFaulted(
    ConfirmedProfileResetRequestV1 confirmed,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken);
```

정상 호출자는 실제 Confirm 반환→기존 CommitForExecution 1회→그 반환을 ExecuteCommittedReset에 명시 전달한다. 기존 commit-only 호출의 효과와 시험은 그대로다. CommitForExecution에 자동 C1 실행·새 결과 반환·실행 getter를 넣지 않는다. lower 3인자 ExecuteConfirmedReset도 actual C3 commit 이후의 명시 호출이며 C3 commit을 대신하거나 내부에서 재호출하지 않는다.

두 관리 조회는 실제 발급 CWT/독립 thread anchor/원래 Owner·adapter/router 참조/C3 Completed/C4 사건만 읽으며 Unity 객체의 속성·body projection·root·receipt·actions를 읽거나 변경하지 않는다. IsExecutionAvailable은 확증된 원래 thread의 아직 C4 미소비 original association만 true, foreign/unregistered/unsupported/untrusted/소비·fault·종료는 false다. IsExecutionFaulted는 정확 original association에 이미 기록된 C4 permanent fault만 true이며 새로운 payload 검증·fault 기록·native cleanup을 만들지 않는다. 둘 다 getter로 proof/root/identity를 돌려주지 않는다. 관리 기록 손상으로 확증하지 못하면 false이고 실행 권한을 만들지 않는다.

Owner 진입도 먼저 관리 IsExecutionAvailable을 조회한다. false인 최초 worker·소비된 늦은 loser는 Owner Validate/gameObject/Commit 호출 전에 비변경 거절한다. 지원하는 원래 thread에서만 Owner gate를 획득하고 동일 관리 availability 및 same `_confirmed`/ExecutionCommitted 원본 문맥을 다시 확인한다. 정상 늦은 loser 거절은 terminal catch 밖이다. 원래 thread의 Owner 전체 무결성 Validate는 내부 오류 경계 안에서 수행한다. lower Execute는 별도 원장 gate에서 한 번 소비한다. Owner gate는 lower 호출까지 보유하되 canceled 재진입은 관리 consumed 조회에서 먼저 거절되며 registry lock을 native 구간에 걸쳐 보유하지 않는다. 항상 finally Exit한다.

정상 lower 결과를 전체 Validate한 뒤 gate를 해제한다. Busy 또는 Stale일 때만 같은 result를 기존 예정 AcceptFreshExecutionHandback으로 한 번 전달한다. 이 전달은 자체 Owner gate와 아래 fresh 순서를 사용하므로 gate를 중첩 획득하지 않는다. 해당 결과부터 fresh 종료까지 execution guard는 유지된다. terminal 정상 결과는 원래 thread에서 Owner Terminal 및 이미 닫힌 Q-A/Q-B를 폐쇄 상태로 유지한다. Completed 결과도 기존 old UI 권한을 재개하지 않는다. 새 필드가 필요해도 원본 commit 이력과 receipt를 덮어쓰지 않는다.

lower 예외에서 IsExecutionFaulted가 true인 **이 same 원본 실행**만 Owner terminal로 처리한다. lower 내부에서 불가역 소비/guard 뒤 예외는 fault를 native 종료보다 먼저 기록하고 원래 양측을 영구 폐쇄한다. 프로토콜 거절·다른 실행 승자의 정상 consumed 기록을 fault로 바꾸지 않는다. Owner 내부 무결성 실패는 원래 thread의 양측 terminal 경계를 따른다. 위 두 관리 조회와 Owner 조율은 신규 planned 서명이며 현재 소스에 존재한다고 주장하지 않는다. 독립 검수는 availability 재인증·lower 단일 소비·정상 loser catch 제외·fault 조회가 winner를 다시 닫지 않는 구조를 확인해야 한다.

## lower private 가드와 Adapter/Router의 정확 상호 호출

private 필드명과 원장 자료구조는 구현 선택이지만 상호 호출은 다음 예정 서명으로 고정한다. 모두 Input.Unity internal이며 Hub 역참조·public callable·등록부 getter가 없다.

```csharp
internal sealed class ProfileResetExecutionGuardContextV1 { }
internal enum ProfileResetExecutionGuardOperationV1
{ Enter=1, CompleteFresh=2, Close=3 }
// DesktopProfileLaunchAdapterV1 및 InputRouter에 각각 같은 서명 1개.
internal void ApplyExecutionGuard(
    ProfileResetExecutionGuardContextV1 context,
    ProfileResetExecutionGuardOperationV1 operation);
// ProfileNewGameConfirmationV1 internal partial의 관리 인증.
internal static void ValidateExecutionGuardContext(
    ProfileResetExecutionGuardContextV1 context,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    ProfileResetExecutionGuardOperationV1 operation);
internal static bool IsExecutionGuardedPair(
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router);
```

context는 getter 없는 실제 lower private CWT 발급 객체다. confirmed/C4 consume·정확 original pair/root/세대/원래 thread·현재 guard 작업 사건에 사적으로 결속한다. caller의 operation enum·context 후보만으로 phase를 바꾸지 않는다. lower가 실제 소비 사건 뒤 Enter 작업을 선등록하고 각 원본 half에 Apply를 호출한다. 각 half는 자신의 현재 상대 pair와 context를 동일 ValidateExecutionGuardContext로 인증한 뒤 해당 half만 설정한다. Close는 실제 original fault/terminal 사건, CompleteFresh는 same lower reservation 완료·Hub acknowledgment/consume 지배 뒤의 작업 사건을 요구한다. default/unknown operation·foreign context·phase 불일치·duplicate 완료·단일 손상을 정상 전환으로 인정하지 않는다.

IsExecutionGuardedPair는 해당 pair의 실제 C4 consumed/등록된 guard 상태와 양측 완료·미fault를 관리 원장으로 확인하는 C2 eligibility 보조다. 실제 C2 proof/root 재인증을 대체하지 않으며 guard 중인 아무 pair를 허용하지 않는다. C2는 원래 소유권 검증과 exact pair 검사 후 이 등록된 eligibility에만 제한 분기한다. Close/fault 뒤에는 false다. false를 실제 C2 Busy/Reload/Manual로 제조하지 않는다. native Disable/Dispose는 원래 thread에서만 기존 경로로 수행한다. worker의 독립 Profile writer 일괄 차단은 제공하지 않는다.

lower만 actual 등록과 전환 사건을 만들며 Apply나 Validate 호출자는 새로운 issuer가 아니다. 양측 Enter·CompleteFresh의 절반 실패는 원래 fault 기록→양측 Close 순서이며 guard를 반쯤 성공으로 발행하지 않는다. closed forensic 결과 읽기와 현재 guard eligibility는 분리한다. 이 추가 상호 호출은 계획 API로서 본 통합 Draft 승인 전 독립 검수 대상이다. 새로운 scope/공개 권한을 구현 편의로 늘리지 않는다.

## fresh 이후 정상 Q-A/Q-B 발급 경로의 정확 적용

현재 Q-A 69~96행 TryTakeRetainedIntent 및 Q-B 144~164행 TryTakeRequest는 최초 retained/taken 경로다. 이 두 메서드의 본문·최초 `_takenRequest`·원래 transfer 역사와 legacy 감사 추출 영역은 그대로 보존한다. 현재 Q-A 107/134행은 `_confirmationInteractionClosed`로 정상 Update/FixedUpdate를 차단하고, Q-B 115행은 기존 Cancel successor만 별도 Poll한다. C4 fresh는 이 옛 닫힘을 false로 되돌리거나 Cancel successor를 권한으로 재사용해서는 동작할 수 없다.

허용된 두 runtime에서 **신규 execution-fresh 슬롯이 있으면 먼저 그 actual live 슬롯을 검증·처리하고, 없으면 원래 경로를 그대로 수행하는 분기**를 추가한다. Q-A Update/FixedUpdate의 신규 분기는 원래 closed 플래그 검사 앞, Q-B LateUpdate의 신규 분기는 기존 successor 앞에 둔다. 새 helper는 Q-A의 TryTakeRetainedIntent부터 private Awake까지 감사 영역 밖에 배치한다. 신규 body도 같은 금지 토큰과 신규 엄격 감사 대상이다. 기존 `_confirmationInteractionClosed`/controller/cursor/retained/consumedTransfer/Cancel successor는 폐쇄 역사로 그대로 남는다. 새 slot 자체의 phase·closed·cursor baseline·한 번 transfer 사건만 live permission이다.

Q-A 신규 exact callable은 다음 하나이며 앞의 여섯 fresh 준비/결속 서명을 바꾸지 않는다.

```csharp
internal bool TryTakeExecutionFreshIntent(
    NewGameConfirmationOwnerV1 owner,
    HubNewGameExecutionFreshReservationV1 reservation,
    object nextEpochToken, long nextEpoch,
    out HubMenuIntentRequestV1 request);
```

이 정상 intent 전달은 actual committed Hub 예약·reciprocal owner/Q-B/presenter/router·same next token/epoch·current fresh slot·첫 true frame 폐기 완료·actual retained intent와 미transfer 사건을 인증한다. old retained나 scalar intent를 자동 선택하지 않는다. 정상 아직 미ready/이미transfer는 false이며 초기 output은 default다. 등록된 슬롯 손상은 원래 thread에서 신규 및 원본 pair를 폐쇄한다. 전달 request는 기존 immutable launch handoff receipt를 보존하고 실제 새 cursor가 생성한 의도로만 만든다. lower identity/proof/result를 받지 않는다.

Q-B 신규 private fresh poll은 이 메서드만 한 번 호출해 새 슬롯의 Live/Proof/RequestReady를 기록한다. 기존 TryTakeNewGameForConfirmation(172~216행)에 신규 actual fresh 슬롯을 고르는 제한 분기를 추가하되, 최초 경로와 Cancel successor 경로의 원래 검증·take 역사를 보존한다. 새 슬롯은 Live.Item=NewGame·immutable handoff·same Owner pending epoch 인증 후만 실제 IssuedNewGameRequestV1를 한 번 등록한다. TakenEpochRecord/Issuances 기존 CWT와 같은 actual handle→Owner.BindIssued/HasPendingIssued 원본 연결을 사용하며 이전 record를 Previous로 남긴다. 새 슬롯의 taken/issued 역사를 원래 `_takenRequest`에 대입하지 않는다. fresh 완료 전에는 발급할 수 없다.

MatchesConfirmationCohort 및 Owner의 같은 cohort/CanBindIssuance 검사에는 해당 **완료된 새 슬롯의 실제 reciprocal association**만 인정하는 신규 분기를 둔다. 옛 closed/Completed 역사를 살아 있는 cohort로 인정하지 않는다. fresh slot의 영구 closed는 신규 분기를 차단하고 원래 닫힌 슬롯으로 fallback하지 않는다. 동일한 take/intake/Confirm/Commit 경로가 새 handle을 처리하되 oldEpoch handle은 clean 거절, 새 handle만 새 decision generation을 만든다. 정상 Take 이후 오류는 기존 양측 terminal 원칙이다.

신규 named 행 `AC004_FreshSlotUsesActualNewIntentWithoutLegacyTakeReset`, `AC004_OldCommittedHandleRejectsAfterFreshWhileNewHandleSucceeds`, `AC004_FreshCloseNeverFallsBackToOldOrCancelSlot`, `AC009_ExecutionFreshTransferKeepsLegacyBodiesExact`를 새 focused 원장에 포함한다. 기존240/15/562/51/610의 이름·선택·원래 본문 감사는 변하지 않는다. 새 7번째 Hub callable과 슬롯 분기 위치는 실제 정적 차이에서 필요한 planned 개정이며 한정 수용된 여섯 서명의 변경으로 숨기지 않는다. 독립 루나가 소유권·legacy 감사·신규 분기의 비우회성을 확인한 뒤 아스트라가 이 통합 계약을 승인해야 한다.
## r4 same Owner fresh 세대 전환의 필수 규범

이 절은 앞 절의 AcceptFreshExecutionHandback과 fresh 순서를 구체화하며 암묵적 reset을 금지한다. 현재 Owner SHA `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043`에서 필드48~61행·EpochHistory71~80행·CanBindIssuance135~139행·InspectInitial188~206행·Rearm292~320행·Commit322~348행·Validate354~359행을 실제 확인했다. 현재 Commit은 ExecutionCommitted를 설정하며 새 발급은 AwaitingRequest와 빈 active 필드를 요구한다. 기존 Rearm은 Cancel 권한만 처리하므로 Busy/Stale fresh에 재사용하지 않는다. 현재 존재하는 fresh 전환 메서드가 있다고 주장하지 않는다.

### 상태·token·세대·역사의 구분

Owner enum에는 신규 예정 `ExecutionFreshPreparing=12`만 끝에 추가한다. 기존 AwaitingRequest1부터 Closed11까지 숫자·기존 정상 흐름은 바꾸지 않는다. 이는 Owner 내부 준비 상태이고 execution outcome/phase의 구성원이 아니다. 같은 Owner의 ExecutionCommitted→ExecutionFreshPreparing→AwaitingRequest 전환은 아래 실제 fresh 작업의 단일 완료로만 허용한다. 준비 중 모든 Take/Bind/Accept/Confirm/Cancel/Rearm/Commit의 정상 새 권한 발급은 금지한다. 기존 enum 범위 비교로 새 상태를 우연히 허용하지 않는다.

| 값/슬롯 | 준비 중 규칙 | 성공 공개 후 규칙 |
| --- | --- | --- |
| Owner·cohort·adapter/router | 기존 actual 원본 그대로 | 같은 참조 유지, 다른 Owner를 새로 만들지 않음 |
| interaction epoch/token | old active 값과 proof 유지, next는 사적 pending context에만 보관 | checked old epoch+1과 실제 신규 nonnull token을 active/proof에 같은 값으로 결속 |
| decision generation | old 양수 `_generation`을 그대로 기준으로 보관 | 0으로 reset하거나 fresh 전환만으로 증가시키지 않음. 다음 실제 성공 Capture에서 기존199행의 checked +1, 이후 OpenFreshDecision262행의 정상 증가만 허용 |
| state/proof | ExecutionFreshPreparing와 일치하는 proof, 실제 pending context·operation gate·old commit 인증 | 모든 reciprocal/lower 완료 및 신규 pristine 슬롯 뒤 AwaitingRequest를 마지막 공개 표지로 설정 |
| old pending/accepted/capture/decision/retry/confirmed/rearm | 작업의 immutable old context·역사 후보에 원래 참조/값을 모두 보관. active 값을 임의 삭제하지 않음 | 아래 전체 old snapshot을 append-only 이력에 연결한 뒤 새 active만 비움 |
| C3 Completed/C4 consumed/result/handback | 같은 원본의 영구 발급·소비 이력 유지 | 복구·다시 Available·다시 commit 금지, 결과 getter는 forensic 행을 유지 |
| Q-A/Q-B | 원래 retained/cursor/taken/issued/closed·Cancel successor 유지, 신규 pending fresh 슬롯만 준비 | 완료된 신규 슬롯만 live, 옛 닫힌 값·taken을 reset하지 않음 |

새 역사 사건은 기존 `_history`를 Previous로 유지하고 old token/epoch/decision generation/accepted issued/request/capture·fresh/decision/retry/confirmed/rearm·actual C3 commit/C4 consume/result와 이번 fresh reservation/completion을 사적 immutable 원본 참조로 연결한다. 기존 EpochHistory의 Cancel/Rearm 사건을 수정하거나 C4 사건을 가짜 Rearm capability로 채우지 않는다. 동일 역사 연속성에서 old epoch를 기록하는 C4 종류의 새 노드를 추가하고 CanBindIssuance의 `_history.Epoch==nextEpoch-1`/old Token!=new Token 조건을 충족시킨다. 종류 구분·사적 필드명은 구현 선택이지만 전체 원본 상관·검증·이전 노드 불변은 계약이다. old record의 Rearm 유무를 새 fresh 권한으로 해석하지 않는다.

최종 신규 active 슬롯은 `_pendingIssued`, `_acceptedIssued`, `_acceptedRequest`, `_display`, `_fresh`, `_decision`, `_retry`, `_confirmed`, `_rearm`가 모두 비어 있다. 기존 configured/cohort는 유지한다. `_generation`은 old 값, epoch/token 및 각각 proof는 실제 next 값, `_history`는 같은 old epoch를 보존한 신규 노드다. 이 조건으로 기존 CanBindIssuance의 새 epoch pristine 요구를 충족해야 한다. 비우기 전에 모든 old 값과 위 사건이 실제 역사에 보관됐음을 확인하며 scalar 복사만으로 C3 commit 권한을 새로 만들지 않는다.

### 단일 gate와 정확 공개 순서

1. 정상 public 제품 경로가 아닌 예정 internal AcceptFreshExecutionHandback(result)는 lower 실제 result 원본·Busy/Stale/NoBarrier·same Owner/pair/old epoch·C4 소비·원래 thread를 사전 인증한다. 이미 정상 완료/세대교체된 동일 result와 foreign candidate는 비변경 거절한다. Owner Enter를 획득한 뒤 동일 실제 결과의 미완료 handback과 old ExecutionCommitted association을 다시 인증한다. 늦은 정상 loser 거절은 terminal catch 밖이며 finally Exit한다.
2. 내부 무결성 검증 후 immutable old operation context·checked next epoch·같은 Owner가 만든 실제 next token·빈 next active 슬롯·신규 history 후보를 준비한다. token은 old 및 모든 이력 token과 다른 참조다. allocation/overflow/무결성 오류는 내부 terminal이며 정상 전환으로 되돌리지 않는다. ExecutionFreshPreparing와 그 proof를 설정하고 pending context를 실제 등록한다. 기존 active 원본을 다음 단계의 auth 자료로 보존한다.
3. lower ReserveFreshExecution(result, same pair, this, old token/interaction/decision generation, next token/interaction)→Hub capability 등록→Q-B Reserve→Q-A Prepare→같은 reservation의 actual Matches ack→Q-B Commit→Owner의 same Hub capability 한 번 소비 순서를 유지한다. IsExecutionFreshInProgress는 **ExecutionFreshPreparing + operation1 + actual pending context + 미완료 원본 association**에서만 true다. 기존 Rearming/IsRearmInProgress와 섞지 않는다.
4. next active 슬롯과 새 history 후보는 아직 사적 준비 자료다. CanBindIssuance/HasPendingIssued는 이 상태에서 false이며 Q-B도 새 issued handle을 만들지 않는다. pending 슬롯을 actor의 current epoch인 척 읽거나 기존 token/proof를 부분 변경하여 권한을 발행하지 않는다.
5. same lower CompleteFreshExecution을 한 번 호출한다. 실제 Hub ack/commit/Owner consume가 이 호출을 지배한다. lower·양측 guard 완료는 동일 원본 예약에 결속한 관리 작업이며 이 구간에서 Unity/event/action enable·추가 publication을 수행하지 않는다. 정상 UI 생산의 재개는 다음 최종 공개와 gate 해제 뒤의 원래 Unity 경로에 한정한다.
6. 이미 준비한 immutable history 후보를 `_history`에 append하고 next active token/epoch/proof·빈 권한 필드·유지된 decision generation을 결속한다. 같은 actual completed fresh Q-A/Q-B association을 Owner의 신규 active 슬롯과 reciprocal로 인증한다. **AwaitingRequest/state proof를 마지막 비예외 공개 표지로 설정**한다. 아직 이 표지 전에는 새 슬롯을 live permission으로 읽지 않는다. 마지막 공개 전의 모든 throwing 검증·자료 생성은 앞 단계에서 수행한다. finally operation을 해제한다.
7. 첫 실제 true cursor frame 폐기 후 다음 정상 선택에서 Q-B가 새 handle을 실제 발급하고 Owner의 BindIssued/HasPendingIssued→AcceptNewGame→정상 Capture/Confirm→새 Commit을 수행한다. first actual Capture의 generation은 old generation+1이며 consumed old confirmed/decision/issued는 새 active 슬롯이나 pending으로 재선택되지 않는다.

원래 닫힌 상태의 Validate getter는 유지한다. Validate/IsNormallyEnded/CanBindIssuance는 새 Preparing를 명시적으로 구분한다. Preparing는 private pending context와 immutable old commit/history를 검증하되 아직 미완료 Q-A의 live cohort를 정상 pending으로 요구하지 않는다. 기존 Awaiting/Inspecting/Decision/Cancelled/Rearming의 live cohort 요구는 유지하고, 새 AwaitingRequest에서는 완료된 fresh reciprocal association을 요구한다. 기존 ExecutionCommitted/TerminalFailure/Closed는 원본 역할·무결성 검증을 유지하고 현재 live 결속을 요구하지 않는다. 새 enum을 추가했다는 이유로 `state<=Rearming` 또는 종료 상태 범위 비교를 광범위 완화하지 않는다.

이 단일 작업에서 등록/Prepare/ack/Commit/consume/lower Complete/최종 공개 전 실패는 original context를 사용해 lower Close·Owner terminal·Q-A/Q-B 원본 및 신규 슬롯 closure를 수행한다. 이미 lower 완료됐더라도 Owner 공개 전 실패는 fresh 성공으로 기록하지 않는다. 이전 history/consumed 사건은 보존하며 old token/권한을 복원하거나 Cancel로 대체하지 않는다. 이는 실제 winning 작업의 부분 실패 경계다. 이미 정상 완료된 동일 handback의 늦은 loser·foreign 호출까지 성공한 winner를 terminal로 닫는 규칙으로 확대하지 않는다.

### 필수 실제 행과 증거

새 focused 원장에 `AC004_ActualBusySameOwnerPublishesPristineNextEpoch`, `AC004_ActualStaleSameOwnerPublishesPristineNextEpoch`, `AC004_PreparingOwnerCannotBindOrIssueUntilReciprocalComplete`, `AC004_NewActualHandleConfirmsWithNextDecisionGeneration`, `AC008_FreshOwnerHistoryPreservesOldCommitAndAllCapabilityReferences`, `AC008_ConcurrentSameHandbackHasOneOwnerTransition`, `AC008_FreshOwnerPartialPublicationFailureClosesBothCohorts`를 추가한다. 위 API로 실제 temp root/C1 Busy 또는 Stale를 만들고 정상 actual issuance/intake/confirm으로만 준비한다. old handle 비소비 거절·new handle 성공·token 참조 변경/epoch+1/generation 유지 후 실제 Capture+1·pristine 전체 필드·old append history·단일 완료·late loser 비변경·원본/신규 부분 실패 terminal을 full row로 기록한다. 제품 권한/epoch/proof 반사 제조나 기존 fixture 행 추가는 금지한다. 선언된 27개 checkpoint 범위를 넘는 새 throw seam을 임의로 만들지 않으며 기존 fresh checkpoint의 실제 위치가 이 공개 순서를 검증하는지 독립 source 대조한다.

## 규범 QA 부록

정확 증거 객체와 실행·선택·행·검증 스키마의 소유 Draft는 [C4 QA 증거 프로토콜](2026-09-29-c4-qa-evidence-protocol.md)이다. 두 소유 규격이 모두 Approved가 되기 전에는 신규 QA 도구 구현·실행을 하지 않는다. 제안 원문은 역사일 뿐 실행 규범을 보완하는 숨은 입력으로 쓰지 않는다.

- 단일 프로젝트 queue가 정규화 절대 projectPath의 SHA256으로 `Local\AcadeGameMaker-C4-<SHA256>` named mutex를 큐가 단독 소유한다. runner는 같은 잠금을 중복 획득하지 않는다. `Global\` 및 별도 잠금 파일은 금지하고 abandoned mutex는 실패다. 살아 있는 Editor·증거 미완료 뒤 다음 실행 금지는 유지한다.
- runner는 partial QA-return만 stdout 단일 JSON으로 반환한다. QA child의 WriteHost/stdout/stderr는 별도 포착하며 runner stdout으로 전달하지 않는다. queue가 실제 runner Process.ExitCode를 종료 뒤 관측하고 최종 QaReturn을 CreateNew로 정확 한 번 쓴 후 최종 verifier를 호출한다. self exit 예측·미관측0·동일 파일 덮어쓰기는 금지한다.
- 실제 기존 NUnit XML과 최종 source로 확증된 enum/escaping 표기만 지원하며 미지원 선언은 실행 전 실패다. fixture source 담당자가 실제 row emitter와 expected ledger의 동일 schema를 함께 발행하며 이름·개수는 최종 소스/parser 대조 때 동결한다. 377 역사 행·기존 이름/선택/180초·actual native/QA/outer0·독립 검수 요구는 그대로다.
