# C3 R7 실제 Submit 실패의 최소 관측 제안

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007, AC-M5D7QC3-006/009/010의 제한 원인 판별 제안이다. 소스·환경·QA 도구 수정, Unity·컴파일·Git 실행을 수행하지 않았다. 기존 R6/R7 실패와 기존 설계는 보존한다. 아래 관측의 구현·실행은 아스트라의 제한 승인 후 별도 단계이다.

## 현재 증거

`artifacts/c3-r7-edit-submit-r1.xml` SHA-256 `33C3B8CB4DA6965751EBA5B3DE499041199D1BA4BAB371D89FBA47BF79644F63`은 정확한 단일 시험 실패를 기록한다. 실제 10.610006초, `NewGameConfirmationOwnerV1Tests.cs:426`의 실제 `SubmitPressed=true` 주장은 false이다. 검증 JSON은 이름 차이/중복 0, 입력 884개의 전후 지문 차이 0이며 통과가 아니다. QA 반환은 5이고 부모가 확인한 실제 편집기 종료는 2이다. 기존 정상 종료 분류를 재설계하거나 별도 배열 관측을 추가하지 않는다.

실행 전 원장의 Edit 시험 SHA `96B763452BF8A70B8C2232E9B93449D2576C871971A16A9C9D8D9A25CA1D7CE2`, Play 시험 SHA `20B7EA2340402149B20DD0D7080FD4DFA1AFF400E701C6BE2050370E001BA3A4`는 현재 재조회와 일치한다. R7에서는 412행의 pre-successor released 입력 갱신이 존재한다. 따라서 그 갱신만 추가한 보정이 실패를 해결하지 못했다는 실제 증거이다. 같은 패치를 반복하지 않는다.

## 실제 경로 차이와 남은 후보

Edit fixture 700–725행은 비활성 호스트 구성→활성화→필요한 Awake 명시 호출→adapter/latch Start·Update→Router Step→Presenter Update 후 실제 `Presenter.Activate` API로 초기 의도를 만든다. 이 API는 정상 Q-A/Q-B 발급 경로이지만 실제 키 입력의 action callback 작동까지 증명하지 않는다. 키보드는 fixture 반환 뒤 409행에서 등록된다. 이후 released 갱신→실제 take/intake→Cancel/Rearm→Enter 갱신→Router Step에서 실패한다.

Play fixture 320행은 키보드를 먼저 등록한 뒤 호스트를 활성화한다. 322–326행의 정상 launch/latch/Step 경로 후 `SelectInitialNewGame` 411–417행은 실제 Down 또는 Enter→release→Enter→release 발행과 Presenter 처리를 수행한다. 334행 `Publish`는 큐 입력→`InputSystem.Update()`→`StepForTests()`이다. Play는 초기 실제 Submit 발행 자체를 415행에서 요구한다. 이 차이는 키보드 등록 순서와 실제 입력 처리 환경을 함께 포함하므로 등록 순서만 원인으로 확정할 수 없다.

Router `Awake` 519–525행은 진입과 준비 무결성을 검사한다. 준비된 실제 launch의 `InitializePreparedHub` 321–336행이 330행 `RegisterCallbacks`, 332행 `Initialize`를 호출하여 준비된 UI-only 초기화를 수행한다. `OnEnable` 1130행은 종료/재활성화 금지 검사만 수행하며 맵 활성화나 콜백 등록을 하지 않는다. 따라서 사적 `OnEnable` 강제 호출은 근거 있는 입력 초기화 보정이 아니다. UI 활성화 격리는 1014–1045행, action 인증은 1371–1375행, `OnSubmit`→`MarkUiEdge`는 1470–1476행이며 실제 `_uiSubmitPressed`만 761–769행의 프레임 후보에 포함된다.

`Assets/GameInput.inputactions`의 실제 맵명은 `UI`이다. 52행의 Submit과 71–73행의 `<Keyboard>/enter`, `<Keyboard>/space`, `<Gamepad>/buttonSouth`가 존재한다. 맵명이 HubUI라는 전제나 Enter 바인딩 누락으로 설명하지 않는다. 실제 launch가 채택한 action의 override·mask·device·control 해석 상태는 아직 기록되지 않았다.

설치된 `Library/PackageCache/com.unity.inputsystem@7a4e1a2a8194/InputSystem/Runtime/`도 읽었다. `InputSystem.cs:2783–2785`의 기본 Update는 manager Update이며 `InputManager.cs:1975–1977`은 기본 갱신 종류를 선택한다. 같은 파일 242–243행은 player 갱신의 EditMode 특수 설정이 없고 게임 비실행 또는 비집중 상태이면 Editor 갱신을 선택한다. `Actions/InputActionState.cs:1503–1504`는 Editor 갱신의 action control-change 처리를 거절하고 1405–1410행도 초기 상태 검사를 제외한다. 그러므로 정상 Edit 갱신으로 키보드 값은 변해도 action callback이 발생하지 않을 별도 정적 경로가 있다. 실제 R7의 갱신 종류와 해당 설정을 관측하지 않았으므로 이 경로를 확정 원인으로 판정하지 않는다.

## 다음 단일 시험에 필요한 관측만

기존 Edit AC006 시험과 해당 fixture 내부에 시험 전용 읽기 기록만 추가한다. 최소 시점은 fixture 반환 직후, 키보드 등록 직후, released Update 직후, Cancel 직후, Rearm 직후, Enter Update 직후이면서 Step 이전, Step 직후이다. 새 successor 이후에는 빈 프레임을 추가 발행하지 않는다. 각 기록은 같은 실제 객체 참조에 결속되어야 하며 실패 전 기록을 XML 출력에 남긴다.

| 관측 경계 | 정확 읽기 항목 | 원인 판별 |
|---|---|---|
| 갱신 환경 | `Application.isPlaying`, `Application.isFocused`, `InputSystem.settings.updateMode`, `InputState.currentUpdateType`(각 실제 Update 직후), 설정/장치 목록의 읽기 값 | Editor action 처리 제외와 player 갱신을 분리한다. 내부 패키지 설정을 대입하지 않는다. |
| 실제 키보드 | 등록한 keyboard 참조/deviceId·added·enabled·현재 Enter 값, `Keyboard.current` 참조, 등록 장치 ID | 큐 이벤트가 장치 상태에 반영됐는지, 다른 현재 키보드인지 구분한다. |
| 실제 채택 actions | Router `_actions` 원본 참조와 asset 참조, UI map 이름·enabled·bindingMask·devices, Submit action 참조/id·enabled·phase·bindingMask·bindings의 path/effectivePath/override와 controls의 장치/경로, `WasPerformedThisFrame()` 읽기 | 정상 Enter 지원과 실제 해석·동작 상태를 분리한다. 전체 assets JSON을 반복 덤프할 필요는 없다. |
| Router callback 인증 | 정확 선언 `InputRouter`의 `_mode`, `_captureSuppressed`, `_uiEnableQuarantinePending`, `_uiEnableQuarantineSubscribed`, `_uiCallbacksRegistered`, `_gameplayCallbacksRegistered`, `_uiCallbacksProof`, `_callbackOrdinal`, `_uiSubmitPressed`, `_faulted`, `_disposed`, `_actionsClosed`, `_resetCutoverTerminal`, `_preparedLaunchState` | 맵 비활성/미등록/격리 유지/폐쇄와 실제 callback 발생 전후를 구분한다. 모두 읽기 전용이다. |
| 실제 발행 | Step 전후 `CurrentReceipt`, `CurrentUiFrame`의 값과 실제 mode/tick/Submit, `_frameOrdinal` 및 `_uiSubmitPressed` | callback은 발생했으나 Step의 snapshot/commit에서 소실됐는지 구분한다. getter 검사 오류는 그대로 실패로 기록한다. |
| 실제 상위 결속 | owner/presenter/handoff 상태, 원래 `_retainedIntent`/`_takenRequest` 참조, successor Cursor·BaselinePending·Phase의 실제 값, launch/latch 원래 영수증 참조 | 입력 이전 폐쇄·세대 혼동과 입력 전달 실패를 구분한다. 권한을 제조하지 않는다. |

모든 사적 읽기는 기존 정확 assembly/declaring type/필드 타입 바인딩으로 수행한다. 제품 메서드·필드 대입, CWT 등록, 등록부 변경, 입력/영수증/프레임 반사 제조, 직접 `OnSubmit`/`OnEnable`/action callback 호출은 금지한다. 읽기용 구조를 제품에 추가하거나 제품 delegate seam을 만들지 않는다. 필드 읽기는 원래 상태를 재구성하는 권한으로 쓰지 않는다.

판별 기준은 다음과 같다. Enter 장치 값은 true이나 실제 갱신이 Editor이고 action performed/Router ordinal 변화가 없으면 패키지의 Editor action 제외 경로와 일치한다. player 갱신인데 Submit controls가 등록 키보드를 포함하지 않으면 mask/device/override 해석을 조사한다. action은 performed인데 ordinal 변화가 없으면 Router의 callback 등록 및 인증 상태를 조사한다. ordinal과 pending Submit가 변했는데 Step 후 프레임이 false라면 발행 후보/commit 경로를 제품 결함 후보로 조사한다. 억제/폐쇄 상태가 유지되면 실제 launch와 Cancel/Rearm 전후 차이를 근거로 판단한다. 어떤 결과도 실행 전에 단정하지 않는다.

실제 갱신 환경이 원인으로 확인되면 후속 승인에서 기존 실제 Play AC006 증거의 적용과 Edit 사례의 정당한 위치를 검토할 수 있다. 현재는 사례 이동·행 삭제·조건 완화·시간 증가·사적 패키지 flag 변경·시험 장치 환경 재설정 패치를 제안 실행하지 않는다. 먼저 같은 단일 AC006의 제한 관측으로 원인을 판별한다. 377행 행렬 및 나머지 비싼 집중/회귀 실행은 이 진단에 포함하지 않는다.

### 공개 갱신 API와 시험 환경의 접근성 제약

첫 진단의 최우선 분기는 실제 `InputState.currentUpdateType`이 Editor인지 Dynamic/Fixed인지이다. 한 번의 단독 관측 실행으로 위 표의 각 시점을 기록한다. Editor이고 action 미발생이면 입력 시험 환경의 문제를 우선 분리하고 현재 경로를 반복 실행하지 않는다. Dynamic/Fixed이면 action·Router pending·발행의 세 경계를 순서대로 판별한다. 기록이 부족하면 부족한 관측을 보고하며 근거 없는 제품 패치를 하지 않는다.

명시적 `InputSystem.Update(InputUpdateType.Dynamic)`을 정상 공개 API로 사용하는 안은 설치된 버전에서는 적용할 수 없다. 정확 소스 `InputSystem.cs:2783`의 무인자 Update는 public이지만 2788행의 종류 지정 오버로드는 **internal**이다. `InputManager.cs:3225–3226`은 정상 비실행 Edit 환경에서 player 갱신 자체를 거절할 수도 있다. 따라서 명시적 Dynamic 호출이 편집 시험의 callback을 작동시킨다는 현재 공개 API 근거는 없으며, 반사 호출이나 friend/assembly 추가로 우회하지 않는다. 이후 설치된 정확 공개 API가 별도로 확인되는 경우에만 해당 제안을 다시 검토할 수 있다.

기존 Play 시험은 단순한 활성화 순서뿐 아니라 22행에서 `InputTestFixture`를 상속한다. 설치된 `Tests/TestFixture/InputTestFixture.cs:92–118`의 정상 Setup은 시험 runtime 생성, 원래 입력 상태 저장/분리와 player처럼 동작하는 시험 환경을 구성하며 정상 TearDown이 복구한다. 이는 제품 콜백이나 권한 제조가 아니지만 기존 Edit 환경과 다른 실제 입력 시험 전제이다. Edit asmdef는 `Unity.InputSystem.TestFramework`를 참조하지 않고 Play asmdef는 이미 참조한다. 현재 제한 제안에서 Edit에 이를 추가하거나 전역 `InputSettings`/`ProjectSettings`를 바꾸지 않는다. 이 차이를 실제 관측 결과와 함께 후속 시험 위치 계약에 반영해야 한다. 현재 Play의 실제 성공을 재사용하거나 주장하지 않는다.

현재 Owner Edit 클래스 20행에는 `InputTestFixture` 상속이 없고 프로젝트 `Tests/EditMode`의 실제 C# 검색에서도 그 상속/사용 근거를 찾지 못했다. 같은 이름의 다른 도메인 시험 `Press` 보조 함수는 입력 패키지 사용 증거가 아니다. 따라서 기존 Edit 상속 helper를 그대로 사용 가능한 것으로 추정하지 않는다. 패키지의 공개 `Press`/`Release`(482–490행)는 공개 `Set`(571행)을 거쳐 정상 장치 상태 이벤트를 만든다. `Set` 585–613행은 control 값을 이벤트에 쓰고 `QueueEvent` 후 무인자 `InputSystem.Update()`를 호출한다. 정상 `[Test]`에서는 이 기본 갱신이고 `[UnityTest]`에서는 이벤트를 큐에만 둔다. Editor assembly의 `[UnityTest]`는 578–582행에서 명시적으로 지원하지 않는다. 패키지 Setup 109–113행이 생성/채택한 `InputTestRuntime`은 그 소스 408행에서 `isInPlayMode=true`를 기본으로 가지므로 정상 시험 runtime에서 기본 player 갱신을 선택할 수 있다. 이는 장치 이벤트 처리이며 직접 action callback 또는 가짜 제품 프레임 주입과 구별된다.

그러나 현재 Edit 조립 참조와 수명주기에는 그 시험 runtime이 없다. 현재 범위에서 helper 이름만 바꾸는 것은 구현 가능한 해결안이 아니다. 관측으로 Editor 제외 경로가 확인되면 이미 TestFramework 참조와 정상 상속이 있는 Play 시험 위치를 사용하는 안이 조립 변경 없는 가용 후속 후보이다. Edit에 새 참조·상속을 추가하는 안은 별도 범위 승인 없이는 구현하지 않는다. 프로젝트의 전체 입력 환경을 직접 재설정하거나 패키지 내부 Update를 반사 호출하지 않는다.

## 확인 자료 지문

- R7 검증 JSON: `4F34E364923E97AAECF8AD15B6DFA75D85F5B57619690EE584A488A5FD7EAF3E`.
- R7 QA 반환 JSON: `E6888CF7A754C21C6E4ACEC6DFB10A3FA2A0D79115DCDD48E32C29CF7CB9F57A`.
- 패키지 `InputManager.cs`: `3D0A05B08DAE53BC753F4B9E56619F60D616311347064C2DD5883B18252C1CED`.
- 패키지 `Actions/InputActionState.cs`: `F75867163F293FFBAA4829154C55DC95A5EE4A72BD36623849C427D810D350A7`.
- 패키지 `InputSystem.cs`: `A6A5D937E423BE1143A4A11D963C39D597298E5A064B99BB4EC8DDFFB34B6D90`.
- 패키지 `Tests/TestFixture/InputTestFixture.cs`: `1D3496E92F6EAF53ED51B8499FFE4659AB9502F77691E562B33784A1FDAC9765`.
- 패키지 `Tests/TestFixture/InputTestRuntime.cs`: `FF8F6D7E70AA4707E54482E2FC4BDECCFF50BBD38A088A2C6BF31044F96E5440`.

패키지 경로와 지문은 설치된 구현의 읽기 근거이며 게시 후보나 승인 범위를 추가하지 않는다. 이 제안은 원인 판별 계획이며 실제 원인 확정·제품 수정·시험 통과·C3 전체 수용 증거가 아니다.
