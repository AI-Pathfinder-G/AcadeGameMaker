# C3 R7 Submit 최소 관측 제안 r2

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007 및 AC-M5D7QC3-006/009/010을 추적한다. 원본 제안 `2026-09-29-c3-r7-edit-submit-minimal-observation.md` SHA-256 `EECF51248D3C0295AA4FA329E0B1EABDB2F467DB57D2EF7015928560FE9BD5E2`와 루나 검수 `2026-09-29-c3-r7-edit-submit-minimal-observation-luna-review.md` SHA-256 `C21D229C74B61A92BC777AAEDF5A1E80C7979B47378A678CDF67647464CE17EA`를 보존한다. 이 r2는 게으른 해석·추가 제품 검증을 제거한 첫 진단 설계이다. 구현·Unity·컴파일·Git 실행과 기존 자료 수정은 없다.

## 정확한 범위

기존 R7 단일 AC006의 실제 입력 큐·Update·take/intake·Cancel/Rearm·Step·기존 assertion을 그대로 유지한다. 입력이나 발행 프레임을 추가하지 않는다. action/asset/map의 모든 추가 getter, `WasPerformedThisFrame`, controls/bindings, 상위 상태/결속 getter와 추가 `CurrentReceipt`/`CurrentUiFrame` 조회는 제외한다. 기존 assertion의 `CurrentUiFrame` 조회만 그대로 남긴다. 진단은 제품 권한·등록부·callback·상태를 쓰지 않는다.

최소 관측은 다음 다섯 시점이며 모든 시점에 같은 항목만 기록한다.

1. fixture 반환 후 기존 키보드 등록 직후, 원래 released Update 전.
2. 원래 released Update 직후.
3. 원래 Rearm 반환 직후.
4. 원래 Enter Update 직후이면서 원래 Step 전.
5. 원래 Step 직후이면서 기존 assertion 전.

허용 항목은 `Application.isPlaying`, `Application.isFocused`, `InputSystem.settings.updateMode`, `InputState.currentUpdateType`, 등록한 원래 keyboard의 `deviceId`·`added`, 아래 Router 여섯 필드의 값이다. `currentUpdateType`은 **Update 직후 관측된 최근 갱신 종류**이다. 특정 호출의 종류를 증명하는 특별한 callback 관측이나 각 event 처리 종류라고 과장하지 않는다.

| 정확 선언 타입 | 필드 | 정확 타입 |
|---|---|---|
| `AcadeGameMaker.Input.Unity.InputRouter` | `_callbackOrdinal` | `System.Int64` |
| 같은 타입 | `_uiSubmitPressed` | `System.Boolean` |
| 같은 타입 | `_captureSuppressed` | `System.Boolean` |
| 같은 타입 | `_uiEnableQuarantinePending` | `System.Boolean` |
| 같은 타입 | `_uiCallbacksRegistered` | `System.Boolean` |
| 같은 타입 | `_mode` | `AcadeGameMaker.Core.InputMode` |

여섯 필드는 실제 Router 40/45/61/94/144/147행에 선언되어 있다. 기존 Edit fixture `Field`(802–808행)는 고정 `Fields` 사전에 없는 이름을 거절한다. 이 여섯 항목을 기존 사전의 다른 필드로 대체하거나 이름 단독 fallback을 쓰지 않는다. 관측용으로 이 여섯 필드만 정확한 `FieldInfo`를 사전 준비한다. 선언 타입은 `typeof(InputRouter)`, 조립은 `AcadeGameMaker.Input.Unity`, 공개 타입 이름과 실제 조립을 검증한다. `Instance | NonPublic | DeclaredOnly`, 정확 `DeclaringType`, 이름, 필드 타입, 비정적 여부를 모두 대조한다. 값 읽기는 `GetValue`뿐이며 setter/메서드 실행은 없다.

사전 바인딩 검사는 실제 fixture 생성 이전에 수행한다. 불일치면 진단 사용 불가를 별도로 기록하고 진단을 비활성화하며 원래 시험 본문과 assertion은 그대로 수행한다. 각 관측도 보호된 시험 전용 기록 구간에 두어 진단 오류를 별도로 남기고 원래 경로를 계속한다. 진단 오류를 원래 제품 오류로 바꾸거나 원래 assertion 예외를 잡아 통과시키지 않는다. 어떤 진단 오류도 성공한 판별로 집계하지 않는다. `_actionsClosed`/`_faulted` 등 추가 필드는 첫 판별에 필요하지 않아 포함하지 않는다.

## 장치 읽기의 추가 축소 근거

설치된 `InputDevice.cs:301,355`의 `added`·`deviceId`는 기존 인덱스/ID 값 읽기이다. 반면 `enabled` 202–214행은 `QueryEnabledStateFromRuntime` 635–655행으로 이어져 최초 runtime command와 device flag 캐시 쓰기를 할 수 있으므로 제외한다. `Keyboard.enterKey` 1311행과 인덱서 2368–2377행 자체는 기존 control 참조 읽기이지만 `ButtonControl.isPressed` 198–204행은 `InputControl.value` 1320–1339행의 가공·캐시 쓰기를 경유할 수 있다. 따라서 `enter.isPressed`도 첫 진단에서 제외한다. 원래 키 이벤트가 실제 장치 값에 반영됐는지는 이 축소 관측으로 확인하지 못한다.

`InputSystem.cs:2857–2859`의 settings getter는 기존 manager settings 참조 읽기이고 `InputSettings.cs:55`의 updateMode getter는 기존 값 읽기이다. `InputState.cs:27`은 `InputUpdate.s_LatestUpdateType` 읽기이다. 정적 환경 값은 읽기만 하며 설정 대입이나 패키지 feature 변경을 하지 않는다.

## 판별과 한계

관측된 최근 종류가 Editor이고 Enter Update 전후 `_callbackOrdinal`/`_uiSubmitPressed` 변화가 없으면 설치 패키지의 Editor action 제외 경로와 **일치한다**고만 보고한다. 실제 장치 상태 반영이나 단일 원인 확정은 아니다. 패키지 `InputManager.cs:242–243,1975–1977`, `InputActionState.cs:1503–1504`가 그 정적 근거이다. Dynamic/Fixed이고 콜백 등록이 false이거나 억제/격리 값이 유지되면 해당 Router 상태의 실제 관측으로 범위를 좁힌다. 등록·억제가 정상처럼 보여도 action 처리와 장치 값은 관측하지 않았으므로 제품 결함이나 바인딩 결함을 확정하지 않는다. pending Submit 변화가 있는데 원래 assertion이 실패하면 발행 경계의 추가 검토가 필요하다고 보고한다.

설치본 `InputSystem.cs:2788`의 종류 지정 Update는 internal이며 공개 우회안이 아니다. Owner Edit 클래스는 `InputTestFixture`를 상속하지 않고 Edit asmdef에는 TestFramework 참조가 없다. 기존 Play는 해당 정상 상속과 참조를 갖지만 그 사실만으로 실제 통과를 주장하지 않는다. 새 friend·조립 참조·internal Update 반사 호출·전역 InputSettings/ProjectSettings 변경·직접 callback·가짜 프레임·억제 필드 대입은 금지한다.

이 설계의 후속 실행 범위는 같은 단일 시험의 원인 판별 한 번뿐이다. 반복 released 갱신이나 행렬/나머지 비싼 집중 선택 실행은 포함하지 않는다. 관측을 성공적으로 남겨도 기존 R7 AC006 실패가 통과로 전환되지 않는다. 추가 장치/action 관측 또는 시험 위치 보정은 실제 첫 결과를 근거로 별도 승인한다.

## 지문

R7 XML `33C3B8CB4DA6965751EBA5B3DE499041199D1BA4BAB371D89FBA47BF79644F63`, 실행/현재 Edit 소스 `96B763452BF8A70B8C2232E9B93449D2576C871971A16A9C9D8D9A25CA1D7CE2`를 보존한다. 아래 설치 패키지 경로는 `Library/PackageCache/com.unity.inputsystem@7a4e1a2a8194/InputSystem/Runtime/` 기준이다.

- `Devices/InputDevice.cs`: `0218D44FA6DC8029E41DB72CF41F5F91615C972205423AFBCE4C4776CB091015`.
- `Controls/ButtonControl.cs`: `656A77F2E3095DCEAA204660A295AD3AE813350B91F5C6574379A4A5F172146F`.
- `Controls/InputControl.cs`: `AC3E961052B79BC5CA23550AB46012372E3003D24F90F30B8AF1AD8111E9E4D6`.

다른 패키지 호출 경로의 정확 지문은 보존된 원본 제안에 있다. 설치 패키지 읽기 근거는 게시 후보나 권한 범위 추가가 아니다. 원인 확정·시험 통과·통합 수용은 아직 아니다.
