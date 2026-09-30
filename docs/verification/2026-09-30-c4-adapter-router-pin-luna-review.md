# C4 Adapter·Router 두 파일 지문 및 경계 독립 검토

- 검토자: GPT-6 Luna
- Adapter `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`: SHA-256 `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E`.
- Router `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`: SHA-256 `542FB7DCCC92B8B59DDDBE4A77741BBEFEEE65546F315F5BB2AA3FA8D22B57F5`.
- 대조 계약: C4 r4 `docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md`, SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`; fresh checkpoint amendment `docs/specs/work-contracts/2026-09-29-c4-fresh-checkpoint-accessibility-amendment.md`, SHA-256 `7C4BE9991171CEEAA44EE7C615B20BF76982AB66552F76F0F2F111BFAC6D28BF`.
- 범위: 지정된 두 원본 파일의 제한 정적 검토만 수행했다. 컴파일·Unity 실행, 전체 runtime/fixture 수용은 하지 않았다.

## 판정

P0=0, P1=0. 두 대상 파일은 요청된 pin과 일치한다. 실제 C4 guard 적용·notification 및 session/receipt 조회·Router frame commit/입력 차단이 r4의 관리 원본·생명주기 조건과 양립한다. 이 판정은 두 파일의 제한 정적 범위에만 적용한다.

## 확인한 경계

Adapter `ApplyExecutionGuard`는 Adapter 원본 cohort CWT에서 router와 소유자를 확인하고 guard context를 검증한 뒤 Enter/CompleteFresh/Close 상태를 기록한다. C2가 이미 완료된 정상 세션에 대한 Close는 이전 guard가 이미 닫혔거나 C2 cohort/reset lifecycle이 Completed가 아닌 경우에만 `LatchResetCutoverFailure`로 분기한다. 따라서 정상 C2 Completed 상태를 C4 정상 종료가 실패로 덮지 않는다. Guard half는 이전 receipt/frame 증거를 보존한다.

Router도 `ApplyExecutionGuard` 전에 실제 원본 context를 확인하고, 변경된 blocked/closed half와 mutable 필드가 다르면 fail한다. Enter 때 기존 receipt/frame을 저장하며 Close 때까지 기록을 지우지 않는다. Adapter/Router 각자 `RecordExecutionLifecycleFault`를 `OnDisable`/`OnDestroy`의 native 처리보다 먼저 호출한다. Router `FixedUpdate`, `StepForTests`, `RequestMode`, 입력 callback 및 의미 상태 getter는 guard/terminal 상태를 확인해 추가 commit, mode 전이, 원시 입력 반영을 차단한다. Guard가 살아 있는 동안 public receipt/frame 조회만 검증된 원본 history로 읽고, 닫힌/terminal 상태는 의미 상태를 노출하지 않는다.

Adapter의 `CurrentResetSession`은 session의 전체 `Validate`를 수행하는 live getter다. Faulted cleanup 경로가 이 getter에 의존하지 않는 점은 정상 C2 Completed 보존과 오류 격리를 함께 지키며, 별도 세대 증거 검토의 faulted immutable witness 경로와도 충돌하지 않는다. Adapter receipt 발행은 실제 reset pair 검증과 receipt 자체 `Validate` 뒤 terminal phase 및 receipt를 최종 게시한다.

## 한계

이 검토는 두 pin이 바이트상 일치하고 위 코드 경로가 승인 계약과 정적으로 양립하는지만 확인했다. 실제 Unity lifecycle·guard 경쟁이나 전체 C4 승인·수용은 검증하지 않았다. 이 기록을 근거로 다른 변경 파일 또는 전체 runtime의 수용 범위를 넓히지 않는다.
