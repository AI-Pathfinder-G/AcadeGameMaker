# C3/C4 실행 보고 수용 순환 해소안

- 날짜: 2026-09-29
- 상태: 제안 — Astra 승인 전에는 계약 또는 구현 권한이 아님
- 범위: C3 합성 소유자와 C4 실제 실행 보고 사이의 검증 배분만 다룬다.

## 판정

현재 문구대로는 최종 수용에 순환 의존이 있다. Approved C3의
`AC-M5D7QC3-007/008`은 실제 C1 실행 결과와 장벽 가능성에 근거한 typed executor
report 행을 요구한다. 그러나 승인된 C3 구현 범위는 C1 `Begin`, 실행 보고 형식과
발급기를 금지하고, 그 형식과 발급기는 아직 Review인 C4에서만 추가하도록
고정했다. 동시에 C4는 C3가 독립 검증되어야 C4 intake가 가능하다고 요구한다.
따라서 C3 전체 Verified를 C4 구현의 선행 조건으로 두면 실제 report 양성 행을
어느 단계에서도 최초로 만들 수 없다.

## C1 호출 없이 검증 가능한 실제 C3 경계

C3 합성 시험은 대체 결과를 만들지 않고 다음 실제 생산 경계를 실행할 수 있다.

1. 실제 Q-B take에서 발급된 private issuance를 C3가 한 번만 소비한다.
2. `ProfileNewGameConfirmationV1.Capture`의 실제 관찰 lease를 통해 초기 Busy,
   Unreadable, 분류와 Confirm 재관찰 Busy/변경/동일 행을 실행한다.
3. Confirm/Cancel CAS, 초기 capture retry와 Confirm retry의 분리, 취소 rearm,
   단조 epoch, 새 cursor 첫 연속 frame 폐기를 실행한다.
4. `ConfirmedProfileResetRequestV1`의 실제 lower 발급과
   `CommitForExecution`을 실행하여 `ConfirmedReady -> ExecutionCommitted`,
   confirmed-publication 소비, confirm/cancel/retry/rearm 무효화가 opaque request를
   executor에 반환하기 전에 끝나는지 검증한다.
5. 커밋 전·도중의 허용된 결정적 fault와 커밋 후 teardown에서 권한이 복구되지
   않는지 검증한다. 이는 `AC-M5D7QC3-008`의 C3 소유 구간이다.

이 범위로는 실제 C1 `Busy/NoBarrier`, `ConfirmationStale/NoBarrier`,
`ReloadRequired`, `ManualRepairRequired`, `DiskPrepared` 또는 장벽 가능성 증거가
생기지 않는다. 따라서 이 결과를 받아 fresh epoch 또는 terminal close를 만드는
`ReportExecution` 양성 행은 검증할 수 없다.

## reflection mint 판정

private 생성자를 reflection으로 호출한 값은 issuer의
`ConditionalWeakTable`에 등록되지 않으므로 위조 값이며 음성 거부 시험에만 쓸 수
있다. private `Issue` 또는 미래 C4 mint를 reflection으로 직접 호출해 등록된 값을
만들더라도 실제 consumed confirmed request, guard, C1 반환, barrier proof를 거친
발급 경로가 아니다. 이는 테스트가 생산 issuer의 제어 흐름을 우회해 권한을
제조한 것이므로 `AC-M5D7QC3-007`의 실제 provenance 양성 증거로 인정할 수 없다.
reflection은 backing-field 변조와 forged 값의 fail-closed 음성 시험에만 사용한다.

## 권고하는 단계적 게이트

별도 Astra 승인이 필요하다. 제품 동작이나 결과 행을 완화하지 않고 수용 순서만
다음처럼 명시한다.

1. **C3 합성 선행 게이트:** C3의 실제 Q-B issuance, 관찰, 결정, rearm,
   execution-commit 구간을 구현·검증한다. `AC-M5D7QC3-001..006/009`와
   `AC-M5D7QC3-007`의 관찰 Busy 행, `AC-M5D7QC3-008`의 C3 커밋 구간을
   검증한다. 다만 007/008 결과는 **실행 연결 전 부분 증거
   (`pre-C4 evidence only`)**로만 기록하며 두 AC 전체 상태는 각각
   **Open / Not Verified**로 유지한다. 부분 행을 PASS, 수용 또는 독립 Verified로
   기록할 수 없다.
   `AC-M5D7QC3-010`에 따라 이 범위의 집중 시험과 필요한 회귀를 실패·건너뜀·
   판정보류 0으로 실행하고, Luna가 독립적으로 P0=0/P1=0을 확인한 뒤 Astra가
   이 선행 게이트만 수용해야 한다. C3L EditMode 562개와 worker 51개는 계획된
   필수 회귀 범위이며 현재 R4는 진행 중이고 R5는 미실행이다. 완료된 수용 증거나
   선행 PASS로 기록할 수 없다. 새 C3 owner의 EditMode 수명주기·동시성 행과 실제
   rearm/cursor quarantine의 필요한 PlayMode 회귀는 별도로 실행해야 하며 C3L
   결과로 대체할 수 없다. 이를 C3 전체 Verified라고 기록하지 않는다.
2. **C4 선행 조건 보정:** C4의 “C3 independently verified before C4 intake”를
   위 합성 선행 게이트의 독립 수용과 C3 source/API freeze로 바꾼다. C4는 여전히
   Astra 승인 전 구현할 수 없다.
3. **실제 통합:** Approved C4만 실제 C1 `Begin`, guard, typed report issuer와 C3
   report intake를 구현한다. C3의 consumed opaque request와 exact owner/epoch/
   generation을 그대로 사용한다.
4. **공동 최종 게이트:** 실제 C1 결과로 Busy/Stale fresh handback과 모든
   possible-barrier terminal close를 검증하여 남은 `AC-M5D7QC3-007/008`과
   C4 `AC-M5D7QC4-001..010` 전체를 함께 닫는다. 여기에는 009의 정적/API·권한
   경계 검수와 010의 현재 소스 통합 집중 시험 및 필요한 EditMode/PlayMode 회귀가
   반드시 포함되며, 001..008이나 과거 결과로 대체할 수 없다.
   `AC-M5D7QC3-010`의 현재 소스 집중 시험과 필요한 회귀도 함께
   실패·건너뜀·판정보류 0이어야 하고, Luna가 통합 결과를 독립 검수하여
   P0=0/P1=0을 기록한 뒤 Astra가 최종 수용해야 한다. 이 게이트에서 실제 C1
   결과와 C4 연결 증거가 갖춰진 뒤에만 C3 007/008을 닫고 C3 전체 Verified와
   C4 수용을 기록한다.

별도 합성 report issuer, scalar 결과 seam, caller boolean, reflection mint, C1
결과 복제는 추가하지 않는다. C3 승인 범위에서 C1을 직접 호출하는 시험도
허용하지 않는다. 이 제안은 어떤 집중·회귀 시험이나 독립 검수 결과도 완화하지
않으며 C4를 Approved로 바꾸거나 구현 권한을 부여하지 않는다. C4 승인과 구현은
기존 계약에 따라 별도의 Astra 결정이 있어야 한다.
