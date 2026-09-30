# C4 편집 제어 접근성 독립 검토

검토 대상은 `docs/specs/work-contracts/2026-09-29-c4-edit-control-accessibility-amendment.md`, SHA-256 `24C8308D7DB1DBBB9D6184B005E48EE7EDE07ADA0F692249F03AF675BFDC5DDA`이다. 선행 승인 R4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D` 및 QA R2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`를 대조했다.

판정: 정적 설계 범위 P0 0, P1 0. 편집 시험 조립이 Profile 내부 타입을 직접 참조할 수 없다는 문제는 실제 friend 선언과 일치한다. `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs`의 friend는 `AcadeGameMaker.Profile.EditMode.Tests`, `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Input.Unity.PlayMode.Tests`이며 `AcadeGameMaker.Input.Unity.EditMode.Tests`는 없다. 해당 편집 시험 asmdef도 Profile 조립을 참조하지만, 참조만으로 internal 접근 권한이 생기지는 않는다.

제안된 `ProfileResetExecutionTestControlFactoryV1.Create`는 이미 허용된 `ProfileResetExecutionBridgeV1.cs`의 내부 경계에 한정되고, R4의 기존 `IProfileResetExecutionTestControlV1` 네 메서드와 인자 형식에 대응한다. 내부 구현이 실제 checkpoint를 그대로 보내고 disk proof 및 C2 result 객체를 `Action<object>`에 전달하면 두 대상은 실제 클래스 인스턴스 참조를 유지할 수 있다. 새 proof/result를 만들거나 결과를 반환받아 실행 흐름을 바꾸지 않는다는 제약도 명시되어 있다. 정상 3인자 경로가 기존 고정 NoOp에 남고, 시험만 이미 승인된 제어 오버로드를 반사로 호출하므로 공개 ABI·friend·조립 정의를 넓히지 않는다.

C1의 기존 `ProfileResetDiskTestControlFactoryV1.Create(Action<ProfileResetDiskCheckpointV1>)`는 같은 프로젝트 패턴의 근거다. 편집 fixture가 정확한 실제 enum 타입을 reflection으로 가져오고, 자신의 비공개 일반 메서드를 해당 enum으로 닫아 `Delegate.CreateDelegate`로 `Action<실제 enum>`에 결속하는 방식은 표준 반사 사용이며 `Reflection.Emit`이나 임의 enum 변환을 요구하지 않는다. 메서드의 선언 조립, 전체 매개변수와 반환 형식, 정적/선언 전용 범위를 모두 검사하고 대체 overload를 허용하지 않는 제안 조건은 적절하다.

이 판정은 새 factory 서명의 접근성 및 권한 경계가 정적으로 일관된다는 뜻이다. 해당 factory는 아직 Draft이며 실제 구현, C# 컴파일, Unity 시험 실행은 확인되지 않았다. 아스트라의 별도 승인 전에는 추가 API 구현 권한이 없다. 소스·QA·Unity·Git을 수정하거나 실행하지 않았다.
