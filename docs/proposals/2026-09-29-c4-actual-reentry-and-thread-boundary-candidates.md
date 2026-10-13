# C4 실제 입력 취소 재진입과 최초 실행 스레드 경계 후보

2026-09-29. 솔, 실제 `gpt-6-sol`. 읽기 전용 후보 검토다. C4는 Review이며 A/B/C 세 P1은 미폐쇄다. 기존 계약·초안·소스·QA·실행 증거는 변경하지 않았고 Unity·컴파일·Git·네트워크를 실행하지 않았다. 구현·수용·새 결과 발급 권한은 없다.

추적: `REQ-M5D7QC4-001/002/004/005/007`, `AC-M5D7QC4-001/002/006/007/008/009/010`. 원본 C4 계약 76~84행의 실제 소비·양측 가드·입력 캡처 억제·영수증 역사 보존, 245~247행의 실제 재진입/경합 및 단일 C1/C2 호출을 기준으로 한다. [이전 후보](2026-09-29-c4-fixture-seam-decision-candidates.md) `F0B6897D9481D73107B94D70592A2F091F5557C7DE6AC805FB90E9EDA2D5DB33`와 [루나 검수](../verification/2026-09-29-c4-implementation-draft-luna-review.md) `A1CE7289A69C2C719B5067C0CE8D7B9ACCDBB237406D23F03E4C94CAB684F079`를 보존한다.

## 기존 canceled 이벤트의 실제 도달 후보

설치된 InputSystem `1.20.0`의 `InputActionState.cs` 1159~1178행은 맵 Disable에서 각 action의 `ResetActionState`를 직접 호출한다. 889~929행은 Waiting/Disabled가 아닌 진행 상태를 Canceled로 전환한다. 2699~2724행은 패키지 자체 억제 `m_Suppressed`가 없으면 Canceled 리스너를 호출하고, 2770~2774행은 해당 action의 구독자를 같은 호출 흐름에서 직접 실행한다. `InputAction.cs` 597행의 공개 `canceled` 이벤트를 fixture가 구독하는 경로는 새 제품 callback/testhook가 아니다. 실제 소유 action이 정상 Disable되는 기존 외부 이벤트 경계다.

현재 Router 402~410행은 먼저 실제 terminal 이력과 캡처 억제를 기록한다. 418행은 old Gameplay/UI를 Disable하고, 419~421행에서 Router의 기존 구독자를 제거한다. Router `Accept` 1371~1375행의 terminal/캡처 억제 검사는 **Router 자신의 의미 입력 처리**를 거절한다. 별도의 action 이벤트 구독자까지 패키지 수준에서 억제하는 코드는 아니다. 생성 wrapper 1138/1150행의 구독과 1167/1179행의 제거도 Router 인스턴스 구독자에 한정된다.

따라서 **old UI action이 실제 진행 중이면 C2 terminal 진입의 DisableOldUi 내부에서 같은 스레드 재진입이 가능한 정적 후보**다. 현재 C2 161~163행은 실제 준비·terminal 진입·전달·old Dispose·셀/쌍 갱신·최종 영수증 순으로 진행한다. 초기 C4 가드가 actions를 이미 Disable하면 취소가 더 일찍 발생하므로, 사건 위치는 최종 승인 가드 구현에서 다시 대조해야 한다. 가드에서 사건을 만들려고 Disable을 새로 넣거나 억제 필드를 쓰지 않는다.

Dispose 자체의 취소 사건을 보장하지 않는다. wrapper `Dispose` 804~806행은 `UnityEngine.Object.Destroy(asset)`이며, 패키지 state의 `Destroy` 293~320행은 모니터와 내부 상태를 정리한다. 앞선 정상 Disable이 이미 action을 Disabled로 만들었으면 Dispose에서 두 번째 canceled가 생긴다고 세지 않는다. Dispose 도중의 재진입을 별도 실행 행으로 주장하려면 실제 사건·호출 위치 증거가 필요하다.

## 진행 상태를 만드는 최소 실제 fixture

`GameInput.inputactions` 48행의 Navigate는 Value, 52행의 Submit은 Button이며 둘 다 별도 interaction 문자열이 없다. 정상 device 상태 이벤트와 실제 입력 갱신으로 누름/비영벡터를 유지하여 Started/Performed 상태가 실제로 존재해야 한다. 패키지 내부 상태·phase·callback context를 반사로 만들거나 callback을 직접 호출하지 않는다.

기존 Play fixture `SelectInitialNewGame` 425~442행은 중립→Down→release→Enter→release를 정상 발행·소비한다. 마지막 release를 그대로 사용하면 Submit/Navigate가 C2 시점에도 진행 중이라는 전제가 없다. 그 원본을 바꾸지 않고 신규 독립 C4 fixture에서만 다음 후보를 검토한다.

- 가장 작은 양성 후보는 초기 실제 NewGame 선택의 Enter를 유지한 채 실제 take→intake→Confirm→Commit→정상 C4 실행으로 이어가는 경로다. 후속 release/추가 의미 프레임을 발행하지 않는다. 선택 전에 필요한 중립·Down·release의 정확 발행/Presenter 소비는 유지한다. Presenter `CloseConfirmationInteraction` 347~352행은 상호작용을 닫지만 입력 맵을 Disable하지 않는다. 실제 Confirm/Commit 후에도 원래 action이 진행 중인지 정상 읽기로 확인해야 하며 현재 코드로 실행 성공을 주장하지 않는다.
- Navigate 유지 후보는 실제 Down과 Enter의 device 조합을 정상 입력 갱신으로 유지해야 한다. 불필요한 입력 행과 커서 발행이 늘므로 Submit 단독 후보보다 최소성이 낮다. 최초 선택/현재 cursor의 정확 계약을 깨뜨리며 상태를 맞추는 방식은 배제한다.
- release된 행·이미 Disabled된 행·패키지 억제 행·진행 상태가 없는 행은 canceled 재진입 실행으로 집계하지 않는다. 정상 단일 실행 및 callback 없는 구간의 구조 증거로 분리한다.

fixture는 실제 old owned action 참조를 정상 소유 준비 경로 또는 정확 선언의 읽기 전용 관측으로 얻고 `canceled += listener`를 한 번 등록한다. listener는 동일 이미 C4 소비된 opaque request·원래 adapter/router만 정상 executor로 재호출한다. 새 권한·root·generation·결과를 만들지 않는다. C3 Commit 완료 이력과 C4 실행 소비 이력은 다르므로, C3 lower의 Completed 행 하나만으로 모든 C4 실행을 거절해서는 안 된다. 신규 C4 실행 원장의 실제 단일 소비 사건을 구분해야 한다.

재진입 거절은 catch로 시험이 기록하고 callback 밖에서 검사한다. 패키지 2771행의 안전한 리스너 호출은 구독자 예외를 격리할 수 있으므로 callback 내부 NUnit 예외만으로 실패를 입증하지 않는다. listener에서 Disable/Dispose/새 입력 갱신/cleanup을 재호출하지 않고 finally에서 원래 구독자를 제거한다. 거절 결과·발생 횟수·실제 같은 관리 스레드·원래 request 참조는 읽기 증거다. 취소 이벤트 callback 안에서 같은 action state를 파괴하지 않는다(패키지 295행의 처리 중 파괴 금지 조건).

후보 `AC008_ActualOldSubmitCancelReentryRejectsConsumedRequest`는 실제 사건 1회 이상과 정상 winner의 C1/C2 각 1회, 두 가드 유지, clean loser 비변경 거절, proof 중복 소비 없음, 최종 typed 결과/영수증 및 원래 forensic frame/receipt 불변을 각각 관측한다. 자연적으로 발생할 수 있는 추가 사건을 제품값으로 막지 않으며 일회성 fixture 관측은 재호출 시도 한 번만 한다. consumed 거절은 Unity 생존/맵/현재 영수증 조회보다 앞선 기존 관리 참조·private 이력 확인으로 수행하여 정상 winner를 terminal catch로 닫지 않는다. C2 정상 성공에 이미 포함되는 terminal 전이는 loser가 만든 변경으로 세지 않는다.

이 후보는 실제 기존 이벤트를 관찰하므로 새 제품 callback을 넣는 이전 정지 seam보다 권한이 작다. 다만 기존 focused fixture에 새 행을 붙이지 않고 신규 정확 C4 Play 경로/선택과 180초 범위를 승인해야 한다. 실제 실행 결과 없이 AC008 전체 충족이나 세 P1 폐쇄를 주장하지 않는다. callback 없는 C1 구간과 pre-auth 지연 worker는 별도 구조/경합 범위다.

## 최초 실행의 private 관리 스레드 출생 증거 후보

현재 lower `ConfirmedRequestWitness`와 RequestIssued 이력에는 관리 스레드 출생 증거가 없다. 이 문서는 존재한다고 주장하지 않는다. C4의 최초 유효 실행은 adapter/root 인증 시 Unity 객체 조회에 들어가기 전에 원래 스레드인지 판단할 수 있어야 한다. 현재 lower 396~400행의 CWT·관리 참조 인증 다음, 408~412행의 adapter root 호출 전이 그 경계다.

후보는 실제 C3 요청 발급 301행에서 `Thread.CurrentThread`의 관리 참조를 private readonly birth witness에 기록하고 실제 RequestIssued 불변 사건에도 같은 참조를 결속하는 것이다. 현재 C3 실제 발급 경로가 정상 Unity 스레드라는 전제는 별도 구조·실행 증거로 보존한다. 숫자 `ManagedThreadId`만 공개/전달하여 인증하거나 재구성하지 않는다. 하나의 필드 변조를 그대로 정상 birth로 믿지 않도록 기존 원본 CWT·실제 issued 사건과 일치 검증하며 모든 private 필드 동시 변조 방어로 확대하지 않는다.

신규 C4 관리 전단은 실제 issued request/pair 확인→이미 C4 소비된 요청의 clean 거절→미소비 원본 birth 참조 확인→같은 스레드일 때만 Unity preflight→게이트 후 동일 실제 미소비 권한 재인증→실제 한 번 소비로 제한한다. 정상 다른 스레드의 최초 호출은 Unity 조회·권한 소비·가드·C1/C2 전의 비변경 거절이어야 한다. 그 뒤 원래 스레드의 동일 실제 request 정상 실행이 가능한 양성 짝을 둔다. birth 증거 손상과 정상 wrong-thread를 혼동하지 않는다. 손상 시 전체 종료를 wrong-thread에서 Unity API로 실행하는 정책은 정하지 않았으며 원래 스레드 containment와 관리 전단 오류 정책의 정확 판정이 남는다.

수정 후보는 기존 허용 lower `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs`의 실제 발급 witness/issued 사건과 신규 `ProfileResetExecutionBridgeV1.cs`의 private 인증뿐이다. public ABI·caller thread 토큰·새 issuer·asmdef/friend·global hook는 없다. 신규 상태를 추가하며 기존 C3 승인 이력/단일 필드 무결성/strict audit에 영향을 주므로 C3 전체 선행 수용과 별도 exact Approved amendment·독립 검수가 필요하다. 현재 source 수정 권한으로 해석하지 않는다.

반대 근거는 request가 원래 스레드에서 발급됐다는 사실만으로 arbitrary thread 전체 안전성이 생기지 않는다는 점이다. 최초 실행이 다른 스레드로 정상 이전되어야 하는 요구가 있다면 이 후보는 그 계약과 충돌한다. 현재 제안은 그러한 제품 요구를 새로 만들지 않고 원래 Unity 실행 경계만 제한한다. 이미 소비된 worker 경로와 실제 same-thread canceled 경로는 birth 증거 없이도 관리 원장의 선행 거절로 설계 가능하며 최초 worker 실행 문제와 분리한다. birth 손상 오류 정책을 확정하기 전 이 후보를 구현 Ready로 취급하지 않는다.

## 조회한 지문

| 자료 | SHA-256 | 근거 줄 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` | 402~421, 1371~1375 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` | 161~163 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` | 243~259, 301, 396~412 |
| `Assets/AcadeGameMaker/Runtime/Input/GameInputActions.cs` | `730BE8CAD941CD4567BBEA9B844FE33673FEA49842E28CB53C2F088D3F528C89` | 804~806, 1089/1105, 1138/1150, 1167/1179 |
| `Assets/GameInput.inputactions` | `8E23A5A3AE1D514596A1FC8C61017F2740664904741D5A95383074A06E840E76` | 48/52 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` | `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23` | 425~442 |
| `Library/PackageCache/com.unity.inputsystem@7a4e1a2a8194/InputSystem/Runtime/Actions/InputActionState.cs` | `F75867163F293FFBAA4829154C55DC95A5EE4A72BD36623849C427D810D350A7` | 293~320, 889~929, 1159~1178, 2699~2724, 2770~2774 |
| 같은 설치 패키지의 `package.json` | `954681EBD82BA9A9C19FF20C81D947C849C684A40707DF48E7BF6A5B88F3D524` | 4행 `1.20.0` |

패키지 자료는 설치 환경의 정적 근거이며 새 저장소 게시 후보나 승인된 source 변경이 아니다. 최초 스레드 증거와 이벤트 fixture 모두 후보일 뿐이다. 기존 240/15/562/51/610의 사례·선택·필수 실행은 대체하지 않는다.
