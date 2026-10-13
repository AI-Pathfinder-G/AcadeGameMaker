# C4 미결정 A/B/C의 제한 시험 경계 후보

2026-09-29. 솔, 실제 `gpt-6-sol`. 읽기 전용 반대 검토이며 확정 승인·구현·실행이 아니다. 기존 Draft `8339895BC58985997AD55A9577045F99D2EF3C74B1821CAA3E450EFCA6C9E9BE`와 원본 계약은 보존한다. [루나 검수](../verification/2026-09-29-c4-implementation-draft-luna-review.md) SHA `A1CE7289A69C2C719B5067C0CE8D7B9ACCDBB237406D23F03E4C94CAB684F079`의 P1 A/B/C가 이 문서로 폐쇄됐다고 주장하지 않는다. C4 Review·현재 R11 동결·진행 중 Play610을 유지하며 기존 source/QA/Git/network/Unity는 변경·실행하지 않았다.

추적은 `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010` 중 특히 AC003/005/007/008이다. 원본 C4 305~310행은 checkpoint에서 **throw만** 허용한다. 아래 정지·observer·실제 참조 손상은 그보다 넓다. 별도 정확 Approved amendment와 루나 검수 전에는 구현할 수 없다.

## A: 실제 C2 Busy/ManualRepair의 결정적 준비

정상 내부 3인자 executor는 고정 NoOp control로만 실행한다. 시험용 내부 overload/control을 승인한다면 `AfterC1DiskPreparedValidatedBeforeC2Entry` 한 지점에서만 일회성 동기화 정지를 허용하는 후보가 작다. 이 지점은 실제 C1 반환·전체 DiskPrepared 검증·양측 guard 완료 이후, 실제 C2 호출 이전이다. production의 lease-gap에는 새 callback이나 대기를 넣지 않는다.

시험의 별도 worker는 fixture가 이미 알고 있는 **실제 임시 root**에서 원래 root lease를 취득한다. 제어점은 준비 완료 신호를 기다릴 뿐 root/identity/proof/result를 worker에게 발급하지 않는다. 실제 lease 취득 확인 뒤 main executor를 재개하고 C2가 원래 Acquire에서 Busy를 반환하게 한다. worker는 실제 반환·양측 terminal 확인까지 lease를 보유한 뒤 finally에서 해제한다. 결과를 직접 생성하거나 Busy 예외를 checkpoint에서 던져 대신하지 않는다.

근거는 `ProfileResetMemoryCutoverDataV1Tests.cs` 47~58행의 실제 held-root Busy와 63~70행의 **lease 이후 Busy 주입은 Reload**라는 구별이다. 같은 파일 SHA `9F61A4EB2278C8069742BDA56BCF13140E1B717826908D5D6BC64B60B9EEB8A0`. 현재 C2 소스 `60DF...` 149~151행만 원래 Acquire Busy를 반환한다. C4에서는 그 결과가 이미 DiskPrepared 이후이므로 fresh 없이 terminal이어야 한다. 같은 프로세스의 별도 BCL worker만 썼다면 실제 별도 프로세스 검증으로 기록하지 않는다.

ManualRepair의 가장 직접적인 기존 근거는 같은 시험 188~196행이다. 실제 준비 결과 뒤 `profile.operation.lock` 파일을 삭제하고 같은 경로를 디렉터리로 만들었을 때 실제 FinalizeReset이 ManualRepair와 terminal을 요구한다. 후보 시험은 정지 중 그 **실제 임시 경로의 음성 filesystem 조건**을 만들고 원래 C2를 한 번 호출한다. 정상 권한·prepared proof는 손대지 않는다.

실제 marker 손상도 후보지만 같은 결과라고 미리 단정하지 않는다. `ProfileResetDiskTransactionV1.cs` SHA `BE0F...` 539~555행은 held lease에서 실제 marker/전체 준비 행을 읽고 recoverable 실패를 ManualRepair disk-authority 오류로 바꾼다. `ProfileResetMemoryAuthorityV1Tests.cs` 34~43행은 실제 marker를 `{}`로 바꾸는 기존 음성 사례다(SHA `D39DA6EE7A28CBC409A9F2FF07B3CD3992FB2FC2B84EA03160FFAD971A45FD89`). 이 조건을 전체 C4 연결로 옮길 경우 actor가 실제 lease 하에서 임시 marker bytes만 손상하고 해제한 뒤 C2를 재개해야 한다. proof mismatch의 InvalidOperationException과 recoverable marker 읽기 실패는 다르므로 실제 native 결과로 ManualRepair/Reload/throw를 구분한다. 목표 outcome에 맞춰 기대를 느슨하게 바꾸지 않는다.

반대 근거는 정지 자체가 현재 throw-only 범위를 넘고, 잘못된 handshake가 180초 timeout·lease 누수·테스트 간 교착을 만들 수 있다는 점이다. 정확 하나의 phase·호출당 1회·원래 thread에서 재개·bounded wait·timeout 시 양측 terminal 및 worker 정리·두 가드의 실제 완료·worker 준비 실패 시 C2 미호출을 계약에 넣어야 한다. 최초 C1 Busy/Stale에 이 정지를 사용하거나 lower 호출을 반복하지 않는다. 대안인 C2 단독 기존 시험+구조 검토는 전체 bridge AC007 실행 행을 대체하지 않는다. 무작위 lease 경쟁은 결정적인 준비를 증명하지 못한다.

## B: C1은 readonly struct이므로 observer의 한계를 먼저 고정

실제 C1 `ProfileResetDiskResultV1`은 445행의 **internal readonly struct**이며 447행에 `_outcome`, `_state`, `_proof` 등이 있다. by-value `void ObserveActualC1ResultOnce(ProfileResetDiskResultV1 actual)`는 executor 저장값을 수정할 참조가 아니다. boxed copy의 상위 필드를 반사로 바꿔도 원래 executor local의 enum/default 상태는 바뀌지 않는다. 이를 실제 저장 결과 손상으로 보고해서는 안 된다. getter 검사만 하는 observer도 현재 throw-only보다 넓어 별도 승인 대상이다.

유효하게 좁힐 수 있는 후보는 다음과 같다.

| 후보 | 실제 효과·권한 경계 | 평가 |
| --- | --- | --- |
| C1 by-value 관측만 | 실제 반환 row의 읽기와 호출/상관 관측. return/ref/out/supplier 없음 | A의 phase/실제 결과 확인에 도움. top-level C1 손상 실행 행을 닫지 못함 |
| 실제 C1 PreparedProof 참조의 한 필드 손상 | proof는 257행 sealed class, 259행 `_root` 등 private 필드가 있다. 실제 반환된 row의 same proof 객체에 `_root=null` 음성 주입 후보. 저장된 struct의 같은 참조 대상은 손상됨 | AC005의 **실제 nested proof 손상** 후보. 새 proof/정상 root 발급·복구·TryConsume·registry 변경은 금지. root scalar 대체와 달리 정상 권한을 새로 만들지 않지만 강한 음성 접근이므로 exact type/field/값·반사 binding·도달 시점을 별도 승인해야 함 |
| 실제 C2 반환 참조의 한 필드 손상 | C2 `ProfileResetMemoryResultV1`은 36행 sealed class이고 37행의 `_outcome`은 CWT 원본과 대조된다. 실제 반환 객체에 `_outcome=default` 음성 주입 후 composition 거절 | AC007/출처·mapping 음성 후보. actual receipt/registry를 생성하거나 교체하지 않음. C2가 이미 실행된 이후이므로 C2 진입 전 손상 행으로 기록하지 않음 |
| C1 struct 전체를 반환/대체하거나 writable slot/ref 노출 | fake/default/scalar 조립값으로 executor 입력을 바꾸는 경계 | 배제. observer라는 이름으로 결과 대체 권한을 숨기거나 원래 금지를 우회함 |

별도 승인 후보 observer는 정확 실제 C1 반환 직후 validation 이전 또는 실제 C2 반환 직후 mapping 이전에 각 1회만 호출한다. control이 주는 값은 없고 production은 NoOp다. 시험이 관측한 실제 객체는 같은 invocation에만 한정하고 정상 발급·다른 실행 재사용·static/global hook·identity 추출을 하지 않는다. B의 proof 음성행에서는 exact `_root=null` 하나만, C2 음성행에서는 `_outcome=default` 하나만 허용한다. 같은 객체의 복구·두 필드 동시 변경·새 proof·issuer 삽입은 제외한다.

PreparedProof를 전달하면 시험이 strong internal 권한 객체를 보게 되는 부담은 남는다. 더 작은 대안은 통제된 **negative-only 동작 enum**으로 bridge가 해당 actual proof의 정확 한 필드 손상만 수행하는 시험 overload지만, 그것도 product 소스 안에 반사 손상 코드를 넣는 별도 seam이므로 자동으로 더 안전하거나 승인됐다고 판단하지 않는다. 정상 3인자 executor에 그런 branch를 추가하지 않는다.

추가 두 대안도 확정하지 않고 비교한다.

1. **실제 값의 boxed copy와 production 공통 순수 validator:** 실제 Begin에서 얻은 struct의 boxed copy에 `_outcome`/`_state`의 default·unknown 같은 음성 변조를 가하고, 생산 경로도 사용하는 순수 closed-row validator에 그 값을 전달해 거절을 검사한다. validator는 발급·소비·C1/C2 호출·상태 변경 없이 실제 typed row의 불변식을 검사해야 한다. 생산 C2 분기와 proof 접근의 모든 경로가 그 검증 성공에 의해 지배되고 이후 검증값을 교체하지 않는지 별도 source 근거로 증명한다. 권한 노출·신규 runtime 상태가 가장 작지만 **executor의 실제 local 손상 실행 증거는 아니다**. top-level 행은 pure-validator 음성+분기 지배 구조, nested proof는 실제 full bridge 손상으로 원장을 나눠야 한다. AC003이 이 조합을 허용하는지 정확 기술 판정이 필요하다. 새 성공 결과 발급기나 fake C1 공급자로 확대하지 않는다.
2. **실제 일회용 private result-holder:** bridge가 실제 Begin 반환 struct 전체를 scalar 재구성 없이 private holder에 보관하고 그 저장 행을 검증·분기에 사용한다. 원래 발급 참조·invocation과 forensic typed 값은 별도 private 불변 이력에 보존한다. 시험용 opaque actual-holder observer는 그 **실제 등록된 holder**만 한 번 받으며 return/ref/out·교체값 공급자는 없다. 정확 holder의 저장 projection 필드를 `default`로 손상하는 범위라면 executor가 실제 사용하는 행의 거절과 C2 0회를 full bridge에서 관측할 수 있다. 반면 unknown/scalar별 음성을 하려면 boxed copy 변경 뒤 실제 저장 필드에 다시 쓰는 추가 fault 동작까지 정확 승인해야 하며 단순 readonly 관측이라고 부르지 않는다. 이 손상은 정상 C1 결과의 대체 권한이 아니라 승인된 **negative stored-projection fault**로만 한정할 수 있는 후보다. 불변 원본과 stored projection 불일치를 성공/Busy/Stale로 재분류하지 않고 terminal 처리한다.

두 번째는 실제 저장 경계를 시험할 수 있지만 holder·CWT/불변 사건·관측 지점·반사 저장 손상 범위를 새로 늘린다. identity/root/proof를 holder getter로 노출하지 않고, 시험에는 정확 손상 대상 필드 외 전체 원장을 읽거나 재등록하는 권한을 주지 않아야 한다. holder에서 원본을 다시 꺼내 권한을 제조·되살리거나 observer가 임의 결과를 공급하는 방식은 금지한다. 기존 세 P1을 닫기 위한 최소성은 첫 대안보다 낮으며, 확정 후 exact class/field/값·private 등록·오류 containment·production NoOp·후속 audit를 별도 Approved amendment에 넣어야 한다. 이 문서는 새 holder나 observer를 승인하지 않는다.

최소성 우선 후보는 **C1 top-level default/unknown/scalar substitution의 실제 값 boxed-copy pure-validator 음성과 C4 validation-before-proof/C2 구조 증거를 분리**하고 nested proof/C2 reference 손상만 exact observer amendment로 좁히는 안이다. AC003이 full bridge 저장 손상까지 요구한다면 두 번째 holder 대안의 더 큰 상태·시험 seam을 별도 판단해야 한다. 어느 조합도 현재 승인됐거나 P1 B 전체가 닫혔다고 주장하지 않는다. C1/Profile의 struct를 class로 바꾸거나 반환 algorithm을 변경하는 대안은 금지한다.

## C: 소비 후 늦은 worker와 도달 불가 callback을 구분

A의 guard 이후 정지 중 실제 worker가 **동일 이미 소비된 opaque request와 같은 pair**로 정상 3인자 executor를 늦게 호출하는 후보는 가능하다. 다만 신규 executor는 private issued/C4 consumed history를 먼저 확인하고, 해당 clean loser를 adapter/router의 Unity native 상태 조회 **이전**에 거절해야 한다. 현재 lower 396~400행의 CWT/ReferenceEquals 인증과 350~361행의 private history는 그 순서의 기반이며, 408~412행의 authenticated-root 호출은 adapter로 넘어가므로 consumed loser 거절 뒤에 둬야 한다.

worker 경로의 null/identity 비교에는 UnityEngine.Object의 overloaded `==`/`!=`를 피하고 관리 참조 `ReferenceEquals`만 사용한다. 같은 실제 witness의 이미 소비된 사건은 정상 loser이며 winner를 Terminal로 닫지 않는다. 최초 미소비 요청의 정상 실행은 원래 Unity thread에서만 수행한다. 이 후보로 임의 worker가 valid request를 처음 실행해도 안전하다는 범위를 주장하지 않는다.

`AC008_ActualConsumedRequestWorkerRejectsBeforeUnityAccess` 후보는 main 소비/guard 완료→worker actual call→비변경 거절→main 재개→C1/C2 원래 1회 및 결과/이력 보존을 관측한다. actor가 새 객체·generation·proof를 만들어 호출하지 않는다. 동시에 별도 callback을 product에 넣어 same-thread 재진입을 제조하지 않는다.

이 실행은 **post-consumption 늦은 호출**만 증명한다. pre-auth→gate 사이 지연 창의 모든 순서나 callback-free 동기 구간의 same-thread callback을 재현했다는 증거가 아니다. 그 범위는 gate 획득 후 actual pending 재인증, normal consumed loser의 비변경 거절, 무결성 오류의 양측 폐쇄, finally/gate 해제의 실제 source 구조로 분리한다. 원본 AC008 해석이 이 조합을 허용하는지 아스트라·루나의 기술 판정이 남는다. same-thread 재진입이 실행 가능한 실제 외부 호출 지점이 없다면 구조 한계로 남기며 test callback·gateMonitor로 만들어 통과시키지 않는다.

## 최소 amendment 경로와 남은 결정

원본 C4 305~310행의 throw-only 예외를 개정하려면 다음 정확 경계를 별도로 Approved에 적어야 한다. 기존 Draft 파일이나 허용 목록을 이번에 수정하지 않았다.

- 신규 예정 runtime `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`: 정상 3인자 고정 NoOp, 시험 내부 overload/control, 정확 1개 A 정지 지점·2개 B observer 지점·실제 호출 1회 보존·C의 consumed-before-Unity 거절. C1/Profile·asmdef/friend/public ABI 변경 없음.
- 신규 예정 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` 및 `.meta`: B의 관측 한계/실제 nested 손상·getter/구조 구별.
- 신규 예정 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs`, `C4ActualExecutionPlayFixtureV1.cs` 및 각각 `.meta`: A의 실제 root worker·filesystem 음성 조건과 C의 실제 늦은 worker. fixture 내부의 독립 BCL worker를 우선 검토한다. 실제 별도 프로세스 worker가 필요하면 새 worker 파일/실행 경로를 다시 정확 승인하며 임의 외부 실행을 추가하지 않는다.
- 신규 예정 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` 및 `.meta`: production NoOp 고정·observer 반환/대체 없음·권한/ABI·본문 금지 유지와 실행/구조 원장의 구분.

현재 named 사례의 정식 NUnit names/count를 아직 발급하지 않는다. 승인 후 checkpoint 도달 원장·실제 선언·선택을 새 버전으로 동결하고 기존 240/15/562/51/610 및 180초 조건은 보존한다. A는 좁은 정지 amendment 없이는 결정적인 전체 bridge 행이 부족하고, B는 struct top-level의 실제 손상을 readonly observer로 해결하지 못하며, C는 post-consumption 실행과 나머지 구조 해석을 구분해야 한다. 세 P1의 전체 폐쇄·C3 선행 수용·C4 Approved/Verified는 이번 결과가 아니다.

## 대조한 실제 소스 지문

위 줄 번호는 아래 현재 파일의 실제 정적 조회 기준이다. 시험 실행 결과를 새로 발급하지 않았다.

| 파일 | SHA-256 | 직접 대조 경계 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs` | `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2` | 257~259행 실제 proof 참조, 445~447행 readonly struct, 539~555행 실제 재인증 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` | 36~37행 실제 결과와 원본 등록, 145~175행 실제 lease·결과·종료 처리 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` | 350~361행 실제 이력, 396~412행 관리 참조 인증과 Unity 경계 |

원본 Review 계약은 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md`이며 위 제안은 305~310행의 기존 throw-only 허용을 그대로 승인받았다고 해석하지 않는다. 수용된 경계 제안 `BAB33568BD479AF46C265F3FCCEC232BBEF339C472B6F9CB1DF9A90E17347F0E`와 제한 수용 `C8D9B5EFD011D4C6014F84B8A62F9F502328E6031ACA417F5CDA66DB7D4A77DB`도 이번 새 시험 경계의 구현 승인으로 확장하지 않는다.
