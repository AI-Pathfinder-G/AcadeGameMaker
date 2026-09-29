# C3 R7 Submit 최소 관측 설계 독립 검토

검토 대상 제안 SHA-256: `EECF51248D3C0295AA4FA329E0B1EABDB2F467DB57D2EF7015928560FE9BD5E2`.

## 판정

**P0 0, P1 1.** 문제를 좁히려는 다섯 관측 경계와 제품 입력 경로·권한·프레임을 바꾸지 않는다는 원칙은 적절하다. 다만 현재 표의 `InputAction.WasPerformedThisFrame()`, `controls`, `bindings`를 “읽기 전용” 관측이라고 승인할 수 없다. 설치 패키지 `InputAction.cs`에서 `WasPerformedThisFrame()`은 `GetOrCreateActionMap()`을 호출하고, `controls`는 바인딩 해석을 수행하며, `bindings`는 첫 호출에 목록 추출·할당을 할 수 있다고 명시한다. 관측이 기존 실패보다 먼저 예외나 초기화·해석을 일으켜 원래 경로를 바꾸거나 실패를 가릴 수 있다. 구현 승인 전 이 항목들을 최소 진단 집합에서 제거해야 한다.

## 소스 대조

실제 실패 시험은 `NewGameConfirmationOwnerV1Tests.cs`의 `AC006_ImmediateReadyCursorDiscardsActualSubmitFrameWithoutActivation`이며 Enter 상태 이벤트와 무인자 `InputSystem.Update()` 직후 `f.Step()`을 호출하고, `CurrentUiFrame.Value.SubmitPressed`를 검사한다. 따라서 기존 시험 경로상 갱신 직후와 Step 전후 관측은 분기상 유용하다. 시험/fixture, Edit assembly, 공개 API, 설정을 바꾸거나 추가 프레임을 넣을 필요는 없다.

`InputState.currentUpdateType`은 설치된 패키지 `InputState.cs`의 공개 정적 읽기 속성이다. 다만 `InputSystem.Update()` 반환 후 이를 읽으면 “그 직전 호출이 처리한 종류”인지 보장되는 패키지 계약을 별도 확인해야 한다. 이는 최근 갱신 종류의 관측으로 표시하고, 정확한 호출 종류를 보장한다고 과장하지 않는다. `Application.isPlaying`과 `isFocused`, 등록 키보드의 `deviceId`, `added`, `enabled`, Enter 값은 읽을 수 있다. 장치 전체 목록을 덤프할 필요는 없다.

`InputRouter.cs`에는 제안의 주요 내부 필드가 실제 존재한다: `_actions`, `_captureSuppressed`, `_uiEnableQuarantinePending`, `_uiEnableQuarantineSubscribed`, `_faulted`, `_frameOrdinal`, `_mode`, `_gameplayCallbacksRegistered`, `_uiCallbacksRegistered`, `_uiCallbacksProof`, `_actionsClosed`, `_resetCutoverTerminal`, `_uiSubmitPressed`, `_callbackOrdinal`. 단, 열거된 값을 전부 반복 기록할 이유는 약하다. `_uiCallbacksProof`는 등록 참조 자체를 증명하는 표식이지 콜백 실행 증거가 아니며, `_callbackOrdinal` 및 `_uiSubmitPressed`의 전후 변화가 실제 Router 콜백 수신 여부를 더 직접적으로 구분한다.

`CurrentUiFrame`과 `CurrentReceipt`는 단순 backing-field 조회가 아니다. getter가 종료 상태 및 게시 무결성을 검증하므로 추가로 호출하면 예외 또는 제품 fault 경로가 생길 수 있다. 기존 시험의 `CurrentUiFrame` 확인은 유지하되, 별도 진단을 위해 불필요하게 반복 호출하지 않는다. Step 직후 원래 assertion이 보존되도록 진단은 assertion 전에 실패하지 않는 수동 필드/정적 값 읽기로 제한하거나, 진단 실패를 원래 assertion 결과와 분리해 출력하는 별도 보호 구조를 설계해야 한다.

## 최소 보정안과 한계

승인 후 구현한다면 요청된 다섯 경계( fixture 직후, released 갱신 직후, Rearm 직후, Enter 갱신 직후 Step 전, Step 후)만 기록한다. 각 경계에서 `isPlaying`/`isFocused`, 갱신 후 `currentUpdateType`, 등록 키보드의 장치 식별 및 Enter 값, Router `_callbackOrdinal`/`_uiSubmitPressed`/`_captureSuppressed`/`_uiEnableQuarantinePending`/`_uiCallbacksRegistered`/`_actionsClosed`/`_faulted`를 읽는다. fixture 반환 시점의 키보드 등록 전 관측은 해당 시점에 키보드가 없을 수 있으므로 null/부재를 정상 기록한다. Enter 직전·직후의 callback ordinal과 pending Submit 변화로 입력 수신을 확인하고, 기존 Step 뒤 assertion만으로 발행 프레임을 평가한다.

`InputAction.phase` 등 추가 상태가 정말 필요하면 해당 설치 버전의 getter가 기존 상태를 만들거나 바꾸지 않는다는 소스 검증을 먼저 하고 별도 승인받는다. `controls`, `bindings`, `WasPerformedThisFrame()` 호출은 이 설계의 최소 진단에서 제외한다. 따라서 이 안은 callback 전 단계의 action 자체 상태를 직접 확정하지 못한다. Router 입력 수신이 없었다는 사실까지만 판별하며, 그 원인을 binding resolution까지 세분하려면 별도 검증된 관측 수단이 필요하다.

승인된 비공개 필드의 정확한 타입/선언형식/읽기 실패 처리, 기록 API가 NUnit 결과 XML에 남는지, 다섯 경계 로그가 성공 여부를 바꾸지 않는지는 구현 지문에서 재검수해야 한다. 기존 Edit 시험의 실제 실패 원인, package update 종류의 런타임 값, 실제 성공 여부는 이 정적 검토로 확정하지 않는다. Unity/컴파일 실행과 코드 변경은 없었다.
