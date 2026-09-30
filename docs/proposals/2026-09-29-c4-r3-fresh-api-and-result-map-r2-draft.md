# C4 r3 fresh lower API와 원본 결과 구성 보완 r2 초안

**정정 범위:** C401 원본 SHA `C401ED97EEE7FA43CEB8CB40790AA952863FE721D1DD97FBD98112D4AC56089E`를 보존한다. 원본 C4 184~201행의 5개 outcome·4개 phase를 유지하며 C401의 outcome 합침/phase 축소는 채택하지 않는다. 이 새 r2는 enum·matrix·fresh 조건과 getter 무결성 설명만 정정/구체화한다. lower fresh 4개 callable·Hub 6개 서명·소유권/순서·허용 범위·비재귀 freeze는 그대로다.

2026-09-29. 솔, 실제 `gpt-6-sol`. 상태 **Draft, 구현 권한 없음**. 정확 개정 Draft `ACC8F20DE5D4D4E21F1385C5E204DC2D79BFB76F35CE12F7A439DC3599217738`의 fresh callable/결과 구성 잔여를 아래 예정 계약으로 좁힌다. r3 `F4A6BA518DC75960784AEBC53F77DB0B9107DAA03472D35042BC1D6F9DB75F73`·한정 수용 `178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692`와 기존 모든 문서는 보존한다. 현재 C3 gate·610 실행·C4 Review·source 동결을 유지하며 실제 API 구현·새 승인·Unity/Git/network/QA 실행 사실은 없다.

추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`. 아래 이름·서명·타입·getter 범위는 **planned**이며 구현자는 임의 인터페이스로 나누지 않는다. 이 보완의 독립 검수와 아스트라 exact amendment 승인 전에는 구현하지 않는다.

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

각 반환 결과에는 actual C1Row가 반드시 있다. 예외로 실제 typed row를 받지 못하면 결과를 만들지 않고 영구 fault 사건 기록·양측 종료 후 예외를 유지한다. 모든 사적 nullable 구성은 다음과 같다.

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

`Validate`는 current 미완료 actual reservation만 true다. 정상 foreign/unregistered/완료/closed/늦은 조회는 false이며 완료 이력을 live permission으로 사용하지 않는다. 등록된 원본 projection 손상은 원래 thread에서 무결성 오류·원본 pair 종료이며 unsupported thread는 r3의 사전 무소비 거절을 따른다. 이 함수는 consume/guard release·발행·native callback을 수행하지 않는다.

`Complete`는 actual same reservation/pair/owner/next token/generation과 미완료 사건을 gate 안에서 재검증한다. 정상 이미 completed/closed loser는 비변경 거절한다. 성공은 lower handback 완료 사건을 영구 기록하고 **그 같은 private 완료 증거**로만 기존 exact Adapter/Router execution guard의 fresh 결속을 완료한다. 반환은 void다. 외부 callable guard release나 새 activation proof getter를 제공하지 않는다. pair 절반 결속 실패·기록/등록 실패는 Completed로 보고하지 않고 양측 영구 종료한다.

`Close`의 reservation은 reserve 이전/중간 실패에서 null일 수 있다. null이어도 actual result/handback의 원본 owner/pair를 먼저 인증하며 새 authority를 만들지 않는다. 등록됐던 reservation이면 같은 result 소속이어야 한다. 원본 사건에 Closed/fault를 선기록하고 양측 guard를 종료한다. caller의 손상된 pair projection으로 foreign action을 정리하지 않고 원본 등록 pair를 사용한다. 이미 종료된 동일 작업은 native 정리를 중복하지 않는다. actual foreign 문맥은 clean 거절한다. Hub 슬롯 종료는 lower가 Hub를 호출하는 방식이 아니라 Owner의 같은 catch/finally 경로가 수행한다.

## Hub 6개 서명과 게이트 순서의 결합

기존 예정 `AcceptFreshExecutionHandback`, `IsExecutionFreshInProgress`, Q-B `ReserveExecutionFresh`/`CommitExecutionFresh`, Q-A `PrepareExecutionFresh`/`MatchesPreparedExecutionFresh`의 서명은 정확 개정 Draft 그대로 유지한다. lower reservation은 Owner private 상태에만 보관하고 Q-A/Q-B에는 getter 없는 Hub capability/reservation만 전달한다. Hub CWT는 same actual lower reservation의 사적 연계와 owner/presenter/Q-B/router/next token/epoch를 기록한다. lower에는 presenter/Q-B object조차 전달하지 않는다.

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

## 검증·잔여

새 closed matrix 각 행·getter 단일 projection 손상·foreign result/reservation·same opaque 두 번째 reserve/complete·late consumed·partial registry 실패·원래 thread fault/native canceled·foreign next token/epoch·완료 전 live permission 없음은 신규 focused 원장에 추가하고 actual fixture로만 준비한다. Q-A/Q-B acknowledgment가 lower Complete 호출을 지배하는 구조 보고와 validator가 proof/C2를 지배하는 보고를 별도로 요구한다. 기존240/15/562/51/610의 names/180초/native0/QA0/outer0/full rows/독립 루나/아스트라 수용은 그대로다.

typed field 접근 범위·4개 fresh callable·matrix·token/세대 소유·freeze 순서는 이 보완의 확정 **설계 후보**다. 단일 출처 검수에서 lower bearer 예약만으로 Hub 단계가 완료됐다고 인증하는 것과 private Owner source 지배 증거의 조합이 원래 상호 소유권 요구를 충족하는지 확인해야 한다. lower에 Hub proof를 새로 넣거나 역참조하여 이를 우회하지 않는다. 실제 가드 pair의 부분 완료/종료 순서와 parser/NUnit/새 input count는 구현 후 독립 증거가 필요하다. 아직 전체 구현 계약 Approved·C3 gate 완료·실행 수용을 주장하지 않는다.

## C401 대비 정확 의미 차이와 추가 대조

- `FreshC3Required=1, Completed=2, ReloadRequired=3, ManualRepairRequired=4`를 원본 closed outcome인 `Completed/Busy/ConfirmationStale/ReloadRequired/ManualRepairRequired`로 정정했다. FreshC3Required는 가드/생명주기 상태로만 남긴다. enum 수치값은 위 planned 선언이며 default0을 배제한다.
- 축소 phase `NoBarrierFresh/DiskPreparedCutover/TerminalC1Outcome`를 원본 `PreC1/C1Returned/C2Invoked/Completed`로 정정했다. C1 Busy/Stale/terminal은 C1Returned, 실제 C2 비완료는 C2Invoked, 실제 완료는 Completed다. 예외는 정상 typed 결과로 바꾸지 않는다.
- 합쳐졌던 C1 Busy/Stale fresh 행을 별도 두 행으로 분리하고 Reserve/FreshOpaque 조건을 그 두 actual 원본에 한정했다. 결과 getter의 whole enum/nullability/pair/root/generation/nested validation을 명시했다.
- C2 actual Busy→terminal C4 ReloadRequired 조합은 유지한다. 이는 원본 C4 153~158행의 post-DiskPrepared Busy/Reload/Manual 매핑과 일치하며 C1 Busy/fresh와 합치지 않는다.
- 현재 r3와 ACC8은 실제 C1 Busy/Stale를 각각 원본으로 보존하고 FreshC3Required를 가드 상태로 사용한다. C401의 두 enum 변경을 승인한 근거는 없으므로 이번 정정은 그 변경을 원본으로 복귀시키는 것이다. getter 비노출 선택은 원본의 prepared proof 비유출을 보존하며 원본 closed output의 전체 correlation 검증 의무를 위와 같이 유지한다. 나머지 lower bearer/Owner 지배 경계는 여전히 독립 설계 검수 대상이고 이 정정만으로 수용됐다고 주장하지 않는다.
