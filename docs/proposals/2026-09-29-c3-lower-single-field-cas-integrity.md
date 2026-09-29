# C3 하위 단일 소비 필드 롤백 방지 보정안

- 날짜: 2026-09-29
- 상태: 제안 — 아스트라 승인 전 구현·수용 권한 없음
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/006`, `AC-M5D7QC3-001/008`의 하위 부분 증거
- 기준 검수: [후속 정적 검수](../verification/2026-09-29-c3-lower-gpt6-successor-static-review.md)
- 런타임 기준 SHA-256: `19E142269AD34C94D5B01DD7F4EB603200E1639AFE99F7CCCAA4A1BBCE802114`
- 시험 기준 SHA-256: `79DC9982D74D7FAB470CDFE30C5C6CB35D1F7CFD05F6F62169FE5C7C7ADBC32E`

## 제한된 해결 범위

`ResultWitness.RequestMintState` 또는 `ConfirmedRequestWitness.State` 하나만 이전
정수로 복구해도 실제 발급·예약·완료·종료 이력과 불일치하여 새 권한을 얻지 못하게
한다. 기존 `SourceResult/SourceWitness` 결속과 요청 인증 후 예약 전 검증 실패의
종료 처리는 보존한다. 모든 비공개 필드·등록부를 동시에 변조하는 공격 방어로
확대하지 않는다. 스칼라 두 개를 차례로 쓰고 두 값의 동등성만 검사하지 않는다.

## 최소 비공개 구조

각 실제 result witness와 request witness에 별도 비공개 생애 기록을 결속한다.
`ConditionalWeakTable<ResultWitness, MintLifecycle>` 및
`ConditionalWeakTable<ConfirmedRequestWitness, RequestLifecycle>`의 키는 실제
witness 참조다. 기록은 최초 witness 등록과 함께 생성하며 caller에게 반환하지
않는다. 뒤늦은 조회 실패에 `GetOrCreateValue`로 초기 기록을 제조하지 않는다.

각 기록의 최소 필드는 readonly 전용 잠금 객체 `Gate`, readonly 원본 witness
참조, 현재 불변 사건 참조 `Head`, 종료 전 이력 보존용 `ClosedHistory`, readonly
사전 생성 종료 사건 `FailureClosed`다. `Head/ClosedHistory`는 비공개이며 잠금
안에서만 기록한다. CAS 정수와 별도인 실제 사건을 보관하는 등록부다.
사건은 readonly 종류·원본 result/witness·실제 request/witness·실제 reservation/
witness 참조와 이전 사건 참조를 갖는다. 아직 존재하지 않는 쌍은 null이며, 종류별
필수 쌍을 명확히 검증한다. 기존 capture/root/owner/epoch를 새 값으로 재구성하지
않고 기존 원본 witness와 참조 동일성으로 연결한다.

- 발급 기록: `Fresh -> MintAttempt -> Issued`, 실패하면 `Closed`.
- 요청 기록: `Issued -> Reserved -> Completed`, 완료 전 실패/명시 종료는 `Closed`.
- `Completed/Closed`는 영구 종료이며 초기 사건으로 돌아가지 않는다.
- `FailureClosed`는 기록 생성 때 미리 할당한 불변 종료 사건이다. 중간 할당·등록
  예외에서는 추가 할당 없이 현재 `Head`를 `ClosedHistory`에 보존하고 `Head`를
  이 사건으로 전진시킨다. 이미 종료된 기록은 덮어쓰지 않는다. 기록 생성 자체가
  실패하면 원래 witness/결과를 게시하지 않는다.

mint 정수 0은 `Fresh`에서만, 1은 발급 시도/발급 이력이 있을 때만, 2는 종료에서만
유효하다. 요청 정수 0/1/2/3은 각각 `Issued/Reserved/Completed/Closed`에 대응한다.
경계 진입 시 두 증거를 함께 검사하고 불일치는 인증된 기록을 종료한 뒤 거부한다.
완료 이력에서는 정수를 복구해도 완료 사건을 보존하며 모든 후속 권한을 거부한다.
기록과 정수 중 하나만 맞는 경우를 성공으로 간주하지 않는다.

## 순서와 동시성

1. 기존 실제 CWT 원본 및 exact adapter/router/owner/epoch/generation 인증을 먼저
   수행한다. 인증 실패는 기록 잠금·정수 변경·종료 처리에 들어가지 않는다. 외부
   문맥 거부로 실제 요청이나 발급 권한을 소모하지 않는다.
2. 인증된 mint는 mint `Gate`, 요청 예약/완료/종료는 request `Gate` 하나를 잡는다.
   같은 권한의 사건 검증·CAS·등록·사건 전진·게시를 하나의 잠금 구간으로 묶는다.
   여러 기록 잠금을 중첩하지 않는다. 원본 mint 발급 이력은 request 발급 전에
   확정하여 request 검증에서는 불변 발급 사건 참조를 읽는다.
3. 정상 mint는 `Fresh`와 정수 0을 확인한 뒤 불변 `MintAttempt`를 기록하고 CAS
   `0 -> 1` 승자를 확인한다. 실제 request와 request witness/생애 기록을 등록하고
   실제 resolution 등록을 끝낸 뒤 이 실제 쌍들을 묶은 `Issued` 사건을 기록한다.
   출처 결속과 사건 일치를 확인한 뒤에만 resolution을 게시한다. CAS/할당/등록/
   resolution 게시 준비 실패는 mint를 종료하고 부분 등록된 request도 종료한다.
   부분 등록 request를 외부에 반환하지 않으며 동일 capture 재발급은 금지한다.
4. 예약은 기존 active-root/source 검증 및 `Issued`와 정수 0의 일치를 검사한 뒤
   CAS `0 -> 1`, actual reservation 등록, `Reserved` 사건 기록을 완료하고 게시한다.
   사건은 exact request/witness 및 exact reservation/witness를 함께 보존한다.
   중간 실패는 사전 생성 종료 사건으로 닫고 예약 객체를 반환하지 않는다.
5. 완료는 기존 인증과 exact reservation CWT 쌍, `Reserved` 사건의 같은 쌍 및
   정수 1을 확인한다. source/root 검증 뒤 CAS `1 -> 2`와 `Completed` 사건 기록을
   끝내고 동일 opaque request를 반환한다. 실패 시 종료하며 예약으로 돌아가지
   않는다. 완료 기록 뒤의 단순 중복 호출은 완료 이력을 보존하며 거부한다.
6. 명시 종료와 인증 후 실패도 같은 request `Gate`에서 처리한다. 완료/종료 이력이
   있으면 새 권한을 만들지 않는다. 그 외에는 현재 이력을 보존하고 종료 사건과
   정수 3을 함께 게시한다. 예외 경로에서는 사전 생성 사건을 사용한다.

잠금은 두 증거의 중간 갱신을 정상 동시 호출이 관찰하지 못하게 하며, CAS는 기존
단일 승자 전이를 유지한다. 정상 동시 mint/reserve/complete의 패자는 상태에 맞는
중복 거부로 끝나고 **다른 호출의 정상 승자 사건을 종료하지 않는다**. 데이터/
출처 훼손·정수/사건 불일치·승자 경로의 예외만 인증 후 종료 처리한다. 잠금 안의
root 검증에는 사용자 콜백을 추가하지 않고, 기존 어댑터 검증의 재진입 가능성이
있다면 동일 기록의 진행 중 호출을 거부해 중간 상태에 재진입하지 않게 한다.

## 필수 반례와 시험 이름

아래 이름은 실제 실행 결과가 아니라 구현 후 집중 검증 요구다. 단일 정수만 반사로
바꾸고 다른 원본 증거를 그대로 두며, 거부 뒤 그 정수를 정상 값으로 복구해도
권한이 되살아나지 않음을 확인한다.

- `AC001_MintedCaptureStateRollbackCannotIssueSecondRequest`: 발급 후 mint 0 복구,
  두 번째 발급 거부와 기존 실제 request 쌍 보존.
- `AC001_FailedMintStateRollbackCannotRetrySameCapture`: 등록/게시 준비 실패 후 mint
  0 복구, 재발급 거부와 부분 request 종료.
- `AC008_ReservedStateRollbackCannotReserveAgain`: 예약 후 State 0 복구, 새 예약
  거부, 원 예약도 불일치 종료 후 완료 금지.
- `AC008_CompletedStateRollbackCannotReserveOrCompleteAgain`: 완료 후 State 0 및 1
  각각 복구, 재예약·재완료 모두 거부하며 완료 사건 보존.
- `AC008_ClosedStateRollbackCannotReserveOrComplete`: 발급 상태와 예약 상태 각각에서
  종료 후 State 0/1 복구, 재예약·기존 예약 완료 거부.
- `AC008_SingleStateInvalidValueClosesWithoutRepairResurrection`: 알 수 없는 정수로
  변경 후 거부, 다시 이전 정수 복구해도 종료 유지.
- `AC001_ConcurrentMintHasOneWinnerAndLoserDoesNotCloseWinner`
- `AC008_ConcurrentReserveAndCompleteHaveOneWinnerAndPreserveHistory`
- `AC008_IntermediateRegistrationFailureLeavesTerminalActualPair`
- `AC008_ForeignAuthenticationDoesNotConsumeActualLifecycle`: 외부 문맥 거부 뒤 실제
  권한 정상 1회 성공. 기존 출처/프로젝션/루트 단일 필드 훼손 시험도 유지한다.

등록 장애 검증은 비공개 등록 단계의 실제 실패를 기존 시험 접근 방식으로 유도하고
정상 권한을 제조하는 공개 factory나 합성 발급기를 추가하지 않는다. 시험 편의를
위해 제품 API·등록부·proof·identity를 노출하지 않는다.

## 허용 파일과 수용 제한

아스트라의 별도 제한 승인 뒤 구현 허용 파일은 다음 두 개뿐이다.

- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs`

공개 ABI, asmdef, friend, assembly 방향, Hub 형식 역참조, C1/C2 호출, C4 결과/
보고/identity 추출 권한은 추가하지 않는다. 이 제안은 소스 변경이나 Unity 실행을
수행하지 않았다. 하위 부분 검증이며 `AC-M5D7QC3-007/008` 전체, C3 전체 및 C4
수용을 선언하지 않는다. 구현 후 새 정확한 지문에서 집중 시험과 루나 독립 검수,
아스트라 승인을 받아야 한다.
