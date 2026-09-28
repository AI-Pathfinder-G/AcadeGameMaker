# VD-09 M5D7Q C3 owner 설계 보정 closure

- 검수자: Luna
- 일자: 2026-09-28
- 설계 SHA-256: `038E568911410202273CE867D85C594B4929248DE025F326F03444D5DA0ABB30`
- focused 시험 using 보정 SHA-256: `03DB99C041D9338D77FA00BC87CFF8FF3C95317884C96A4D87C1A28AECA0B35A`

## 판정

이전 설계 P1 두 건은 문서상 해소되었다. P0=0, P1=0이다. 이 판정은 설계 closure이며 C3 구현·실행·수용이 아니다.

- **typed executor report:** C4가 Review인 동안 실제 report 형식과 발급기를 C3 allowlist에 추가하지 않고, C3에는 future opaque protocol 수신 의미만 둔다. 실제 C1 Begin·report 발급·identity 추출은 향후 Approved C4 executor 소유로 제한된다. report는 실제 consumed request, cohort/epoch, 전체 C1 결과, barrier 가능성, one-shot CAS를 묶고, Busy/ConfirmationStale + NoBarrier만 새 C3 handback을 허용한다. ReloadRequired, ManualRepairRequired, DiskPrepared, unknown/corrupt/exception/barrier 가능성은 terminal close한다. 이는 `AC-M5D7QC3-006/007/008`의 이전 누락을 닫는다.
- **initial/Confirm Busy:** `AwaitingCaptureRetry` + `CaptureRetryWitness`/`RetryIntake()`와 `AwaitingDecision` + 동일 `NewGameDecisionCapabilityV1`/Pending 복귀를 표로 분리했다. initial retry는 same issuance를 유지하고, Confirm Busy는 same capability만 재시도하게 한다. 이는 `AC-M5D7QC3-007`의 이전 lifecycle 누락을 닫는다.

focused 시험은 `AcadeGameMaker.Input` using을 포함한 보정 지문으로 기록되었으며, R1 컴파일 실패 기록은 그대로 보존한다. 현재 R2 실행 중이므로 결과 XML 전에는 시험 통과나 C3L 수용을 주장하지 않는다.
