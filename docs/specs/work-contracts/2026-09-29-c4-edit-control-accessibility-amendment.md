# C4 편집 모드 시험 제어 접근성 한정 개정

- 상태: **Approved — 정확 시험 제어 연결 구현 승인, 실제 검증 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-29. 루나 [독립 검수](../../verification/2026-09-29-c4-edit-control-accessibility-luna-review.md) SHA `110726B562565FF89879671FAD6B7D25A125E8EA66E63B6D3119FC9BCA0BCAE8`의 정적 P0/P1=0/0과 실제 friend 목록을 확인했다. 검수 원문 SHA `24C8308D7DB1DBBB9D6184B005E48EE7EDE07ADA0F692249F03AF675BFDC5DDA`는 `2026-09-29-c4-edit-control-accessibility-reviewed-draft.md`로 보존한다. 아래 초안 작성 시점의 미승인 문장은 당시 이력이며 현재 추가 구현 권한은 이 상태를 따른다. 실제 compile·Unity 실행·통합 수용은 미완료다. 원래 r4와 QA r2는 변경하지 않는다.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-29.
- 추적: `REQ-M5D7QC4-003/005/006/007`, `AC-M5D7QC4-002/003/005/006/007/009/010`.
- 선행 소유: Approved [r4 실행 계약](2026-09-29-c4-r4-exact-implementation-amendment.md) SHA `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, [QA r2 규약](2026-09-29-c4-qa-evidence-protocol.md) SHA `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`. 두 원문은 변경하지 않는다.

## 실제 접근 경계와 문제

`Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs`는 `AcadeGameMaker.Profile.EditMode.Tests`, `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Input.Unity.PlayMode.Tests`만 friend로 선언한다. 신규 편집 fixture의 `AcadeGameMaker.Input.Unity.EditMode.Tests`에는 Profile internal 타입 접근을 허용하지 않는다. asmdef의 Profile 참조 자체가 friend 접근을 대신하지 않는다. r4의 제어 인터페이스는 실제 internal PreparedProof를 observer 매개변수로 사용하므로 편집 fixture가 이를 직접 구현하려면 접근성 문제가 발생한다.

friend·asmdef·public ABI·Profile 원본·정상 권한 발급기를 바꾸지 않는다. 이 개정은 이미 승인된 네 시험 제어 지점에 정확히 같은 실제 객체를 전달하는 내부 시험 제어 생성 경계만 추가한다. 새로운 제품 동작·시험 지점·observer·가짜 결과·복구 경로를 만들지 않는다. 독립 구현 가능성 확인 및 루나 검수 후 아스트라가 승인한다.

## 정확 추가 API와 효과

위치는 이미 승인된 신규 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` 내부다. 새 파일·조립·메타 경로는 추가하지 않는다.

```csharp
internal static class ProfileResetExecutionTestControlFactoryV1
{
    internal static IProfileResetExecutionTestControlV1 Create(
        Action<ProfileResetExecutionCheckpointV1> checkpoint,
        Action postDiskPreparedValidatedPreC2,
        Action<object> actualPreparedProof,
        Action<object> actualC2Result);
}
```

모든 인자는 nonnull이며 null은 관리 코드에서 생성 전에 거절한다. 반환은 private 구현의 새 시험 제어 객체다. 생성은 현재 요청·pair·root·identity·proof·result·registry·CWT·Unity/native를 읽거나 변경하지 않으며 발급/소비/실행을 하지 않는다. private 구현은 기존 인터페이스 네 메서드에서 각각 해당 callback을 한 번 전달한다. checkpoint는 실제 enum 그대로, 정지 지점은 인자 없이, 두 observer는 실제 typed 객체를 동일 참조로 object에 담아 전달한다. 새 객체·복사·boxing으로 정상 proof/result를 대체하거나 callback 반환값을 실행 결과로 사용하지 않는다. 예외는 원래 제어 지점으로 그대로 전파한다.

제어 전달 위치·순서·호출 횟수·A의 실제 worker 경합·B의 실제 단일 필드 손상과 재검증·27/51/19 checkpoint 요구는 r4 그대로다. factory 자체는 지점에 도달했다는 증거가 아니다. 실제 실행 행과 부모 NUnit 및 독립 소스 검수로 도달·횟수·원본 객체 상관을 증명한다. factory에는 다른 overload·설정 getter·정상 권한 반환·holder·등록부·전역 hook을 추가하지 않는다.

정상 3인자 실행은 기존 fixed NoOp만 사용하며 이 factory나 외부 callback을 사용하지 않는다. 시험 fixture만 명시 6인자 제어 overload에 반환 제어를 전달한다. observer는 기존 허용 손상만 수행하며 실제 proof/result를 보관해 다른 정상 작업을 발급하거나 원래 실행을 우회하지 않는다. factory는 정상 요청·identity·proof의 발급기가 아니다.

## 편집 fixture의 정확 기존 C1 제어 결속

Profile의 기존 `ProfileResetDiskTestControlFactoryV1.Create(Action<ProfileResetDiskCheckpointV1>)`는 수정하지 않는다. 편집 fixture는 실제 Profile 조립의 정확 타입·DeclaredOnly static 메서드·전체 인자와 반환 형식을 반사로 확인하고 호출한다. enum 타입은 그 정확 실제 메타데이터에서만 얻는다. fixture의 private 일반 generic 수신 메서드를 그 enum으로 MakeGenericMethod하고 `Delegate.CreateDelegate`로 정확 `Action<실제 enum>`에 결속하는 표준 반사 경로를 사용할 수 있다. 반환은 원래 기존 C1 제어이며 새 정상 권한이 아니다. checkpoint 정수는 실제 enum의 이름/정의 값/원장과 대조하고 다른 enum·미정 값·다른 메서드로 fallback하지 않는다.

Reflection.Emit·DynamicProxy·이름만으로 메서드 선택·다른 fixture의 private factory·normal identity/proof 제조·friend 확대는 금지한다. 편집 fixture의 순수 C1 결과 검사도 실제 반환 boxed copy와 승인된 validator의 정확 반사 결속으로만 수행한다. 기존 정상 타입 접근이 가능한 실행 모드 fixture는 기존 제어 인터페이스를 직접 구현할 수 있다. 시험 사례 이름·파일 범위·행 스키마·NUnit180초·선행 선택·실제 종료 관측 요구는 변경하지 않는다.

## 독립 검수와 수용

이 추가 서명과 실제 객체 전달 경계의 접근성·권한 비확장·NoOp 보존을 루나가 독립 검수한 뒤 아스트라가 Approved로 전환한다. 구현 이후에는 factory 생성만으로 실제 실행이나 observer를 발급했다고 주장하지 않는지, 각 callback이 원래 승인 지점의 동일 객체에만 결속하는지 소스 및 실제 행을 검증한다. 실제 C4 소스 동결에는 이 API와 사용하는 fixture 및 개정 계약을 함께 포함한다. 이 문서 작성으로 추가 API 구현·Unity 실행·공동 최종 수용을 발급하지 않는다. 기존 Approved r4의 독립 구현 작업은 계속할 수 있다.
