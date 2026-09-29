# C3 하위 CAS 생애 기록 구현 독립 정적 검수

- 검수자·실제 호출 모델: Luna, `gpt-6-luna`
- 날짜: 2026-09-29
- 범위: 고정 런타임·시험 소스 정적 검수. Unity/컴파일 실행 없음. 대상 외 파일은 수정하지 않음.
- 런타임 SHA-256: `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` — 지정 지문과 재계산값 일치
- 시험 SHA-256: `F5EFF1F9414F1DFC6BC3ECBB4A2B66592AEC61416DADD2902A59F695C699C4D3` — 지정 지문과 재계산값 일치
- 기준 설계: `docs/proposals/2026-09-29-c3-lower-single-field-cas-integrity.md`, SHA-256 `D23ED6F90FE58198A4C5778133083463CA4715BB72CF87DD377C3CDC1A6C6754`

## 판정

P0=0, P1=0. 실제 result/request witness를 key로 하는 비공개 CWT 생애 기록과 불변 사건 체인이 CAS 정수와 별도로 actual result/request/reservation/resolution 참조를 결속한다. mint는 `Fresh → MintAttempt → MintIssued`, 요청은 `RequestIssued → ReserveAttempt → Reserved → Completed`로 실제 객체 쌍을 연결한다. 인증된 요청에서 정수와 event head가 다르면 종료 이력이 고정되고, 단일 `RequestMintState`/`State` 반사 복구로 두 번째 요청·재예약·재완료가 열리지 않는다.

예약/완료 동시 승자 패자는 잠금 뒤 현재 이력을 확인해 정상 중복으로 거부되고 승자의 head를 변경하지 않는다. mint 동시 패자도 이미 `MintIssued`인 상태에서 별도 종료 처리 없이 거부된다. 인증 전에 거부된 foreign 문맥은 lifecycle 잠금과 종료 경로에 들어가지 않아 기존 권한을 소비하지 않는다. 승인된 C3 owner가 callback 재진입·동시 Confirm/Cancel/retry를 자체 CAS로 먼저 차단하므로 이 하위 잠금은 상위 단일 승자 규칙을 대신하지 않는다. 실제 launch-root accessor는 동기 검증이며 사용자 callback을 호출하지 않는다.

예외 containment는 사전 생성 `FailureClosed` 참조를 대입해 추가 사건 할당 없이 수행한다. 실패 전 `Head`를 `ClosedHistory`로 보존하며, 예약 등록 중 실패하면 `ReserveAttempt`가 실제 예약/증명 쌍을 이미 품은 뒤 두 번째 CWT 결속을 수행한다. 두 번째 등록이 실패해도 첫 실제 등록과 쌍은 history에 남고 요청은 닫힌다. mint 등록 실패도 이미 생성한 request/witness 쌍을 실패 전에 사건에 연결하고, request/mint 생애를 종료한다. 시험은 이 중간 등록 실패 경계를 반사 기반 고장 주입으로 직접 다룬다.

시험 소스는 `[TestCase]` 55행과 `[Test]` 18개, 합계 73개의 예정 실행 행을 가진다. request mint/state 단일 필드 복구, 알 수 없는 값, 중간 mint/reservation 등록 실패, foreign 문맥 비소모, 동시 패자의 대기 진입·주 스레드 승자 후 회수와 오류 확인이 존재한다. 참조하는 lifecycle/CWT/이벤트/증명 필드와 생성자, `MemberwiseClone`, `ThreadState.WaitSleepJoin` 및 10초 `Join` helper 이름은 소스에서 확인했다. 이는 정적 행 수이며 컴파일 또는 통과 결과가 아니다.

## 수용 경계

독립 정적 구현 검수에서 P0/P1은 없다. 승인된 두 하위 파일과 기존 lower API 경계 안에 머물며 Hub 역참조, C1/C2 호출, C4 보고·identity 추출 또는 새 assembly/friend/public API를 추가하지 않는다. 이 검수는 실행 증거가 아니며 73개 시험은 아직 미실행이다. 새 상위 owner/Q-B/presenter 일관 단위와 마지막 동결 소스의 집중·필수 회귀 및 별도 독립 검수가 남아 있다. AC-M5D7QC3-007/008 전체, C3 전체 및 C4는 수용하지 않는다.
