# C4 런타임 8개 파일 독립 사전 검토

- 검토 모델: `gpt-6-luna`.
- 범위: 승인된 R4 및 후속 접근성·발급 세대 규칙과 현재 런타임 8개 파일의 정적 대조.
- 한계: Unity, 컴파일, QA 도구 또는 전체 실행은 하지 않았다. 이 문서는 바이트 동결·실행 수용·C4 수용이 아니다.
- 판정: 런타임 정적 구현에서 P0 0건, P1 0건. 단, 아래 QA 증거 규칙 공백은 별도 P1이며 네 예외 행의 실행 전 폐쇄를 막는다.

## 확인한 런타임 지문

| 파일 | SHA-256 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `8248404F3F237936954979F3FEC2848E9F419CF0AA2A781F732D5CA379FF9809` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` | `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `542FB7DCCC92B8B59DDDBE4A77741BBEFEEE65546F315F5BB2AA3FA8D22B57F5` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `55B8C19F88B07C5888128C86BF05A743D747AB206E6D44942110ABE0AAFDD38B` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `D891961CF4E006F2E58B3516C7AFE1B5872B49CFA26DC5E7A83AFB35D458A3F6` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | `6B1D1FB6EDDAFD29ACD3A7A326D56E0B6AD98093ABE2DCBC9F1965BD634B6A59` |

대조한 규범은 R4 계약 `docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md` SHA `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, QA r2 `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md` SHA `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, 발급 세대 개정 `docs/specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md` SHA `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`이다.

## 정적 대조 결과

브리지의 3인자 실행 경로는 내부 고정 NoOp 제어만 사용한다. 별도 시험 제어 연결은 내부 팩터리이며 정상 발급 권한을 만들지 않는다. 실제 실행은 원본 confirmed의 관리 기록과 원래 스레드·owner·adapter·router를 먼저 확인하고, execution CWT 기록과 consume 이력을 등록한 다음 가드→C1→typed 결과 검증→필요한 경우 C2→typed 조합 순서를 따른다. 결과/영수증·세대는 실제 CWT와 원본 기록에 결속되며 scalar 값으로 권한을 복구하지 않는다. 예상된 시험 관측용 메서드 외에 public 제품 API, `InternalsVisibleTo`, 어셈블리 정의 변경은 발견하지 못했다.

Adapter와 Router의 양쪽 가드는 실제 pair·thread·관리 원장에 결속되어 있으며 오류 기록은 native 종료보다 먼저 이뤄진다. C2 변경은 정확 guarded pair만 기존 자격 분기의 예외로 인정하는 제한 분기이고, C2의 기존 lease/proof 재검증·stage·barrier·receipt 흐름을 대체하지 않는다. 원본 C1 알고리즘 변경은 확인되지 않았다. Q-B는 불변 발급 이력에서 자기 issuer·thread·최신 epoch의 실제 기록을 선택하고 Q-A는 원래 예약에 결속된 슬롯을 선택한다. 종료·foreign 투영을 기존 authority로 복원하는 경로는 이 정적 추적에서 확인되지 않았다.

완료된 C2 결과 뒤 실제 `OnDisable`/`OnDestroy`가 발생하면 lifecycle fault 메타데이터가 별도 사건으로 추가될 수 있다. 이는 정상 완료 중 `CloseOriginalExecution`이 오류를 만들지 않는 것과 구분된다. `ResultRecord`의 outcome/phase/returned 기록은 불변이고 `result.Validate()`는 실제 결과 CWT 연결을 계속 검사한다. AC008의 `AC008_ActualDestroyAfterExecutionPreservesForensicHistory`는 destroy 뒤 fault 상태와 별개로 완료 결과·세대가 유지됨을 요구하며, 추가 FaultHistory 사건을 금지한다고 쓰지 않는다. 따라서 이 동작은 승인된 이력 보존과 충돌한다고 판정하지 않았다.

## P1 — C2 미호출 예외 행의 memory 세대 표현 규칙 부재

현재 QA r2는 `Unobserved` 세대를 `PureValidator`/`Structure` 또는 실제 body·payload·consume 전 문맥 거절에 한정한다. 후속 발급 세대 개정은 `IssuedWithoutResult`를 executionGeneration에만 추가했고 `ReceiptWithoutExecutionResult`는 C1과 C2 호출이 각각 1회인 경우로 제한한다.

브리지의 실제 순서는 C1 Begin 후 `AfterC1Begin`, `BeforeC1ResultValidate`, `AfterC1ResultValidate`를 거친다. `DiskPrepared`이면 `BeforePreparedProofTake`에서 proof를 가져오기 전에 예외가 날 수 있다. 네 경로는 이미 execution record와 consume이 발급된 뒤지만 아직 결과 레코드가 없다. 요청된 행은 executionGeneration을 `IssuedWithoutResult`, 결과 outcome/phase를 null, `checked=false`로 기록하고, proof는 Unknown, prepared observer는 미도달·호출 0으로 표시한다. 이 부분은 발급 세대 개정과 정합한다.

그러나 이 행들의 C2는 아직 호출되지 않았고 memory receipt/세대도 이 C4 실행에서 관측되지 않았다. 이를 `memoryGeneration=Unobserved`로 표현하면 QA r2의 허용 조건을 위반한다. `Absent`로 두는 것도 실제 null 발급 부재 관측 없이 사용하면 규약 위반이다. C2 호출 수 0과 `BeforePreparedProofTake`의 미도달을 source-established로 기록하는 현재 계약 문구만으로는 generation rule을 확장할 권한이 생기지 않는다.

필요한 최소 조치는 QA 규범에 이 정확한 FullBridge 실패 경로만을 위한 규칙을 승인하는 것이다. 예를 들어 `C2NotInvokedAfterIssuedExecution` 규칙이 `executionGeneration=IssuedWithoutResult`, `memoryGeneration=null/Unknown`, `checked=false`, `c2Finalize=0`, prepared observer 미도달·0, 실제 해당 checkpoint throw를 요구하도록 고정할 수 있다. 새로운 정상 행·런타임 observer·C2 권한을 추가하지 않고, 4개 경계 이름과 exact call-count/fact 조건을 사전 고정해야 한다. 승인 전 이 4행의 세대 증거는 수용 불가다.

## 범위

이 검토는 소스 구조와 승인 문서 간 정합성만 판단했다. 새 fixture, QA verifier, 전체 최종 manifest와 실제 Unity 실행은 검토·실행하지 않았다. 실제 수명주기와 취소·경합 증거가 수행됐다고 주장하지 않으며 전체 C4 수용을 의미하지 않는다.
