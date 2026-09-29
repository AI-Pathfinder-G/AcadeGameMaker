# C3 R7 Submit 최소 관측 r2 독립 폐쇄 검토

검토 대상 정확 SHA-256: `7DBDF90C6251718F6730512068132D10D76F6C3DDCB976A55BF801E820675536`.

**P0 0, P1 0.** r1의 차단 사유는 폐쇄됐다. r2는 `WasPerformedThisFrame()`, `controls`, `bindings`, action/asset/map getter, 상위 상태 getter 및 추가 `CurrentUiFrame`/`CurrentReceipt` 조회를 제거했다. `InputDevice.enabled`와 Enter `isPressed`도 제외해 runtime query/값 캐시 경유를 피한다. 남은 항목은 정적 환경 값과 장치 id/added, 정확 선언형에서 미리 결속한 Router 필드의 값 읽기다. 이 검토는 제안 설계의 정적 수용성만 판단하며 구현 승인이나 실행 수용이 아니다.

실제 소스와의 대조 결과 제안에 적힌 Router 필드명·타입은 일치한다: `InputRouter._callbackOrdinal` (`System.Int64`), `_uiSubmitPressed`, `_captureSuppressed`, `_uiEnableQuarantinePending`, `_uiCallbacksRegistered` (`System.Boolean`), `_mode` (`AcadeGameMaker.Core.InputMode`). `InputRouter` 실제 선언은 `AcadeGameMaker.Input.Unity` 네임스페이스의 `AcadeGameMaker.Input.Unity` 어셈블리이며 `InputMode`는 `AcadeGameMaker.Core`에 선언되어 있다. 각 필드가 선언형에서 비정적임을 사전 검증한 뒤 `FieldInfo.GetValue`만 쓰고, 불일치·진단 예외 시 기록상 진단을 비활성화해 기존 시험 본문과 assertion을 수행한다는 설계는 최소권한 읽기와 맞는다. 기존 fixture의 필드 whitelist를 이름 단독 fallback으로 우회하지 않는 조건도 적절하다.

다섯 시점은 기존 단일 시험의 입력 큐·무인자 Update·Take/Cancel/Rearm·Step 순서를 그대로 관측한다. 관측으로 입력, 권한, callback, 프레임을 주입하거나 변경하지 않는다. `InputState.currentUpdateType`은 갱신 직후 관측된 최근 종류로만 기록하고 특정 호출 종류를 직접 증명한다고 표현하지 않는다. 기존 `CurrentUiFrame` assertion만 보존하여 추가 getter 호출이 제품 검증 경로를 새로 열지 않는다. 장치 Enter 값과 Action 내부 상태를 읽지 않으므로 원인 판별은 Router callback 수신 여부와 상태에 제한된다는 한계도 명시돼 있다.

실행 전 남은 확인은 새 기록 코드의 정확 반사 결속 및 오류 분리, NUnit 결과 XML 보존이다. 이는 구현 지문에서 검증할 조건이며 현재 설계의 차단 결함은 아니다. R7의 실제 Submit 실패는 그대로 열린 상태다. 이 읽기 전용 검토에서 코드 변경·컴파일·Unity 실행은 없었다.
