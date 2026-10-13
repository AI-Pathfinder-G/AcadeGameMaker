# C4 세션 생산 경로와 스레드 종료 경계 검토

2026-09-29. 솔, 실제 `gpt-6-sol`. 실제 정적 호출 경로만 조회했다. 기존 r2 `A549466BBC08B69DFE576AFB391CAAA1473355982771E9C0613BBE940A83EA7A`·원본 Review 계약·세 P1을 보존하며 새 승인·구현·실행·수용을 발급하지 않는다. 소스·QA·기존 문서·Git·네트워크·Unity·컴파일은 변경하거나 실행하지 않았다.

추적은 `REQ-M5D7QC4-001/002/006/007`, `AC-M5D7QC4-001/002/006/008/009/010`이다. 원본 계약 SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0` 76~86행은 **세션 안의 생산과 메모리/입력 안전**을 보호하며 durable barrier를 주장하지 않는다. 따라서 관리 가드가 모든 외부 filesystem writer를 막아야 한다는 요구로 확대하면 안 된다. C1의 실제 durable barrier와 원래 root lease는 별개다.

## 확인한 실제 생산 진입점

아래 경로는 현재 R11 자료다. C4 가드와 birth 필드는 아직 없다. 표의 현재 차단을 신규 C4 관리 latch가 이미 구현됐다는 증거로 쓰지 않는다.

| 실제 경로·줄 | 호출과 현재 차단 | 관리 플래그만으로 충분한지 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` 610~623, 151~152, 517 | `FixedUpdate`→`CommitNextFrame`의 의미 프레임 생산은 reset terminal/cohort input closed에서 반환한다. 현재 frame/receipt getter도 같은 종료를 거절한다 | 기존 C2 종료 조건의 실제 차단 근거다. 새 C4 pending/terminal이 이 admission에 결속돼야 한다. 원본 forensic 필드 삭제는 불필요 |
| 같은 Router 1371~1375, 1010, 189/199 | action callback의 `Accept`는 terminal/cohort closed/캡처 억제/고장으로 거절한다. mode 요청/맵 전환도 종료를 거절한다 | 원래 callback 캡처와 입력 생산 경계에는 플래그/원본 cohort 조회가 있다. InputSystem의 다른 구독자 사건까지 없어진다고 주장하지 않는다 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` 260~264 | `TryTakeNotification`은 adapter terminal 또는 원본 cohort Entered 이상이면 false다 | adapter 안의 미전달 notification만 보호한다. 이미 latch에 복사된 notification을 같은 플래그가 직접 막는 근거는 없다 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` 47~70 | `TryTakeLaunchNotification`은 자신의 cached notification/state를 검사·소비한다 | adapter/router terminal flag의 직접 선행 검사는 이 본문에 없다. C3에서 이미 소비된 상태라 false인 것과 새로운 가드 admission 보장은 다르다 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` 132~147, 347~352 | `FixedUpdate`는 confirmation interaction closed에서 반환하고 그 외 cursor→Router frame→Interpret로 진행한다. `CloseConfirmationInteraction`은 상호작용/후속 슬롯을 닫는다 | 현재 C3 Commit의 실제 closure가 owned presenter를 막는다. adapter/router 플래그 하나만으로 이미 retained intent까지 직접 거절한다는 근거는 없다 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` 120~160, 289 | `LateUpdate`는 상태와 presenter를 통해 실제 intent를 poll한다. `TryTakeRequest`는 자기 상태/원본 request를 소비한다. 상호작용 close는 `Close`로 연결된다 | 직접 Router flag를 읽는 방식이 아니다. C3 Commit의 실제 Q-B closure와 immutable taken 역사 보존이 필요하다 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` 322~347 | 실제 reserve→336행 Q-B/Q-A close→337행 ExecutionCommitted→339행 lower Complete→finally Exit | 정상 C4 입력은 이미 상위 결속이 실제 닫힌 opaque여야 한다. 신규 가드가 이 closure를 대신하거나 옛 epoch를 복구해서는 안 된다 |

owned HubUI cohort의 “메뉴/notification 생산 차단”은 **현재 C3의 상위 실제 close·기존 상태와 새 lower admission의 조합**으로 검증해야 한다. Router 캡처만 멈추고 이미 상위에 보관된 객체를 모두 무효화했다고 주장하면 부족하다. 신규 fresh에는 별도 승인된 실제 reciprocal 완료와 신규 슬롯만 허용한다. 이미 소유자가 가진 forensic receipt/retained 이력을 지우거나 가짜 publisher를 만들어 차단 시험을 대체하지 않는다.

## 일반 저장과 독립 Profile 디스크 경로

실제 runtime 참조를 조회한 범위에서 `ProfileAtomicSaveServiceV1.Save`를 owned HubUI 세션 중에 직접 부르는 제품 publisher는 확인하지 못했다. “없음”은 현재 조회한 runtime C# 참조 범위다. 새 C4 일반 저장 생산 경로가 이미 있거나 가드가 그 경로를 막았다는 증거로 쓰지 않는다.

- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs` 112~113행의 실제 profile port는 `SaveUnderHeldRoot`/`SaveInputRecoveryUnderHeldRoot`를 호출한다. 200~208행은 root lease 하의 preparation, 256~257행은 정상 준비 중 저장 및 barrier 재검사다. Adapter 627행의 정상 preparation port가 coordinator를 호출한다. 이는 launch 준비의 저장 경로이며 현재 활성 HubUI 메뉴가 별도 ordinary-save authority를 발급하는 경로가 아니다. Adapter `Awake` 269~277행은 단일 시작·정확 Router 예약을 먼저 한다.
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` 163~183행의 공개 Save/held-root Save는 **Profile root lease와 durable barrier**를 검사한다. Adapter/Router를 인자로 받거나 해당 플래그를 읽지 않는다. 223~240행의 recovery 저장도 같은 별도 root/barrier 경계다. C4 pending이 durable 이전에 이 독립 lower API 자체를 차단한다고 주장할 수 없다.
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs` 744행은 실제 reset-first 저장을 `SaveResetFirstUnderHeldRoot`로 호출한다. 기존 C1 내부 경로다. 현재 runtime에서 실제 C1 Begin 호출자는 아직 구현되지 않은 C4가 아니며, 기존 시험들은 실제 Begin을 직접 사용한다. Adapter 355행의 실제 Resume는 restart bootstrap이고 in-session C4 신규 실행과 다르다.

따라서 원본 “ordinary-save production 거절”은 owned in-session producer가 새 가드 아래 새로운 저장 권한을 내지 못한다는 요구로 추적해야 한다. 현재 그런 producer가 없다면 실제 부재·정적 호출 경계를 기록하고, Profile 공개 Save를 임의 owned producer라고 부르거나 synthetic 허용/거절을 제조하지 않는다. 이후 producer가 추가되면 그 실제 entry가 가드에 결속돼야 한다. 외부 프로그램의 `File.WriteAllBytes`까지 막는 기능은 이 계약이 아니다. r2에서 “모든 writer”를 이 범위 구분 없이 독립 lower 전체로 확장한 가능성 검토는 보완이 필요하며 원본 계약 변경 없이 새 결론으로 수용하지 않는다.

## 최소 스레드 대안과 반대 검토

후보 정책은 다음 순서다. 실제 C3 private birth와 **독립 actual issuer-thread anchor**를 관리 참조로 먼저 조회한다. 두 Thread 객체 참조는 아직 없는 planned 상태이며 기존 actual CWT/issued 요청에만 결속한다. 지원하지 않는 다른 스레드의 최초 호출은 body/projection/Unity 검증·소비 이전에 비변경 프로토콜 거절한다. 이 거절은 payload 무결성 검사 완료·유효 권한 인정·Busy/Stale/Completed가 아니다. scalar thread ID나 caller가 만든 identity/proof를 인증하지 않는다.

원래 Unity 스레드의 정상 entry에서만 실제 body/projection·전체 preflight를 검증하고, 등록된 원본의 실제 손상은 기존 fail-closed 의미를 유지한다. 원본 actual pair의 **permanent fault 사건을 private 실행 이력에 먼저 기록**하고 native Disable/Dispose로 넘어간다. 정상 Canceled 구독자의 동일 요청 재진입은 그 사건과 실제 소비 원장을 먼저 읽어 inert 거절한다. C3 Completed와 C4 consumed/fault를 구분하며 gate 뒤 재인증·finally 해제·정상 늦은 loser 비변경 원칙도 유지한다.

현재 Adapter 589행 `LatchResetCutoverFailure`→Router 482~515행은 실제 Disable/RemoveCallbacks/Dispose를 수행한다. 이를 worker의 관리-only containment라고 부르지 않는다. 위 대안은 worker에서 그 함수를 호출하지 않는 **entry 제한 후보**다. 관리 전단에서 Unity 객체의 overloaded null 비교·생존 getter·CurrentReceipt/현재 frame·actions 접근도 하지 않는다. 실제 native cleanup을 하지 않은 worker 거절에는 “cleanup 완료”를 기록하지 않는다.

반대 검토는 다음과 같다.

1. 원본 60~70행은 실행 전 전체 invariants와 pair/root/세대를 요구한다. 다른 스레드는 실행 가능 entry가 아니므로 먼저 프로토콜 거절하고 아무 authority도 인정하지 않는 해석은 가능하지만, 그 해석을 계약이 이미 승인했다고 주장할 수 없다. 원래 스레드의 진입에는 전체 검증을 그대로 요구한다.
2. 원본 72~74행은 clean pre-entry rejection의 비소비·비변경과 reflection corruption의 기존 fail-closed 가능성을 구분한다. 손상 payload를 wrong-thread에서 검사하지 않으면 그 시점의 손상을 **발견·종료하지 않는다**. 실제 손상 반례를 clean 검증 통과라고 표현하면 기준 완화다. “지원하지 않는 문맥의 거절에서는 payload 검사/종료 의무가 없고 원래 문맥에서 엄격 검증한다”는 별도 정확 규범 판정이 필요하다.
3. birth projection/issuer anchor 자체 불일치가 관리 전단에서 확인된다면 정상 wrong-thread와 같다고 지워서는 안 된다. 원본 pair 관리 이력에 오류를 기록하되 native 작업을 원래 스레드로 한정하는 정책 또는 지원하지 않는 문맥의 무검증 거절 정책 중 어느 의미를 요구하는지 미결정이다. 원본 anchor에 대한 반사 손상을 유효 birth로 재등록·복구하지 않는다.
4. worker가 authority를 인정/소비하지 않고 즉시 돌아오는 정책이면 **거절된 worker마다 모든 owned producer를 terminal latch해야 하는지**는 원본 세션 가드 요구와 별도 문제다. 미실행을 이유로 성공한 실행의 가드 의무를 없애지 않고, 잘못된 스레드 호출 하나로 정상 winner 전체를 닫는 의미도 임의 추가하지 않는다. r2의 관리-only 모든 writer latch 의무를 이 정책으로 자동 폐쇄하지 않는다.
5. 원래 스레드에서 fault 사건을 먼저 기록하는 순서는 실제 C2 old-action canceled 재진입 후보에 유효한 구조 보장이다. 그 사건이 모든 관련 C4 entry를 지배하는지 최종 source로 확인하고, 실제 canceled 이벤트가 없으면 구조 근거로만 남긴다. private 이력 기록 예외·부분 등록 실패·native cleanup 예외의 원본 pair containment도 별도 검수한다.

최소 후보 시험은 같은 actual request의 wrong-thread 무소비 거절→원래 스레드 정상 실행, actual consumed worker 거절, 원래 스레드의 승인된 단일 projection 손상→fault 이력 선기록→실제 canceled 재진입 inert→실제 native cleanup이다. 미도달 cleanup·callback 없는 구간·pre-auth 지연 순서를 실행 완료로 집계하지 않는다. 새 publisher나 제품 callback 없이 신규 C4 focused fixture에서만 준비하며 기존 240/15/562/51/610 선택과 180초를 보존한다.

## 지문과 남은 규범 판정

| 추가 직접 조회 자료 | SHA-256 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `80DDBF925C74A8A81DD3B3950F25B3843D76B81F62ED942F3F3585CFB52B3F87` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs` | `8FA81A8E0082F5E05EAB8200BB28B020F038DAC66BBC33B6026E3A4906E15C00` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` | `1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C` |

Adapter/Router/C3 lower/Q-A/Q-B/Owner/C1의 정확 현재 지문은 r2의 14개 pin·추가 runtime 표를 따른다. 이 문서는 바이트를 바꾸지 않았다. 남은 것은 unsupported-thread 사전 거절과 payload 반사 손상 fail-closed 요구의 정확 해석, birth/anchor 손상 처리, 실제 owned producer별 신규 가드 admission 증거다. 독립 Profile writer·외부 filesystem 보호로 확대하지 않으며 C4 Review·세 P1 미폐쇄·C3 선행 gate를 유지한다.
