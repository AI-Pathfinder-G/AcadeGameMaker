# C4 Router 컴파일 복구 구현 독립 정적 검수

검토 기준은 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-router-compile-recovery-amendment.md` SHA-256 `E77E006AB8FCF302F1B5EC6994E8791CE7444CC810FB24F44AC5971C0255B9DD`이다. 대상 `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`의 검토 시 SHA-256은 `E51E2A7903A4BB1C38F3F45306B98C2117ACFE1E7DA2127F334A8D16E707BF18`이다. 비교 기준으로 전달된 이전 핀은 `542FB7DCCC92B8B59DDDBE4A77741BBEFEEE65546F315F5BB2AA3FA8D22B57F5`이다.

## 판정

정적 검수에서 P0 0건, P1 0건이다. 승인된 두 호출부는 기존 원소유자를 형식 확인한 뒤 그대로 넘기며, 대체 어댑터나 새 권한을 만들지 않는다. 이 판정은 해당 Router 바이트의 제한 정적 검수이며 컴파일·Unity 실행 또는 전체 C4 수용을 뜻하지 않는다.

## 호출부와 실패 경계

`InputRouter.ApplyExecutionGuard`에서 `_preparedLaunchOwner`를 `DesktopProfileLaunchAdapterV1 originalAdapter`로 패턴 확인하고, 확인된 동일 참조를 `ValidateExecutionGuardContext`에 전달한다(현재 파일 542–544행). 형식이 맞지 않으면 예외가 먼저 발생하므로 가드 상태 변경이나 프레임 검증으로 진행하지 않는다. 기존 함수는 context의 등록 여부, 어댑터와 라우터의 참조 동일성, 신뢰된 실행 원본, 현재 스레드, 가드·연산 일치를 재확인한다.

`TryReadExecutionHistory`는 이미 등록된 가드 절반이 차단 상태이고 닫히지 않은 경우에만 확인을 계속한다. 이어 동일 필드에서 확인한 어댑터를 `IsExecutionGuardedPair`에 전달하고, 짝이 유효하지 않으면 예외로 닫는다(571–580행). 이 인증보다 먼저 영수증·프레임·증명 필드를 읽지 않는다. 기존 쌍 검사는 어댑터/라우터의 실제 CWT 기록 일치, 스레드, 원본 실행 신뢰성, 소비 기록, fault/native 종료 상태와 진입 상태를 확인한다. 어댑터 형식이 아니면 short-circuit로 거절된다.

`DesktopProfileLaunchAdapterV1`은 `sealed` 클래스다. 따라서 형식 패턴은 의도치 않은 파생 대체를 허용하지 않으며, 두 호출 모두 필드의 원래 객체를 직접 사용한다. 확인된 변경은 전달된 컴파일 오류 두 지점에 한정되어 승인 계약의 원소유자 동일성 및 fail-closed 요구와 일치한다. 다른 Router 호출부나 공개 API, C1/C2·Profile 알고리즘 및 가드 전이는 이번 검토에서 변경 대상으로 사용되지 않는다.

## 범위와 한계

현재 파일과 승인 계약의 SHA를 직접 재계산하고 관련 메서드 및 호출 대상의 선언을 읽었다. 이전 SHA에 해당하는 Router 원본 바이트는 현재 검토 자료에 없어 파일 전체의 과거-현재 바이트 차이를 독립 재생성하지는 못했다. 따라서 “전체 파일에서 변경 바이트가 오직 두 군데”라는 암호학적 diff 주장은 하지 않는다. 다만 승인된 두 호출 경계의 현재 동작과 주변 전이·검증 순서를 확인했으며, 정적 검토 범위에서 승인 외 동작 변경을 발견하지 못했다.

컴파일 및 Unity 재실행은 하지 않았다. 이전 실제 실행의 컴파일 실패 기록은 보존된 실패 증거로 남으며, 이 정적 검수만으로 그 오류가 실제 빌드에서 해소됐다고 판정하지 않는다. 새 pin resource·Q0 감사·원장·계획의 검수도 이 문서의 범위 밖이다.
