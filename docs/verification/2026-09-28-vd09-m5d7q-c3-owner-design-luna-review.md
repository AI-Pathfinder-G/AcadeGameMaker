# VD-09 M5D7Q C3 owner 설계 독립 대조

- 검수자: Luna
- 일자: 2026-09-28
- 대조 대상: `docs/proposals/2026-09-28-c3-owner-implementation-design.md`, Approved C3 계약, 현재 Hub/Q-B 파일
- 범위: 설계·계약 정합성만 검토. 코드·시험·Unity 실행 없음.

## 판정

설계는 실제 Hub 파일에 아직 구현되지 않았고, 구현 착수·수용 근거가 아니다. 구조상 immutable 최초 `IntentRetained`/`RequestTaken`, 실제 private Q-B issuance, owner/epoch 참조 증거, copied value intake 거부, successor cursor 첫 실제 frame 폐기, commit 후 `IdentityFor`, 역방향 friend 금지는 Approved 계약과 정합하다. P0는 없다.

**P1=2, Astra 보강 또는 명시적 설계 보정 필요**

1. **typed executor report 경계가 빠져 있다.** Approved C3 계약은 C1 Begin 직전 confirmed request를 소비한 뒤, barrier 전에 C1이 `Busy` 또는 `ConfirmationStale`을 반환하면 typed executor-report 경로만 새 C3 epoch를 만들 수 있고, `ReloadRequired`/`ManualRepairRequired` 또는 barrier 가능성은 terminal close해야 한다. 설계에는 `CommitForExecution`과 C1을 호출하지 않는다는 경계만 있고, executor outcome 타입·report API·barrier 전/후 상태 전이·stale request 소비 규칙이 없다. 이 누락은 AC-M5D7QC3-006/007/008과 REQ-M5D7QC3-006의 구현 해석을 열어 둔다.
2. **Confirm Busy와 initial Busy의 lifecycle 구분이 명시되지 않았다.** 설계는 Confirm의 `ConfirmInspecting -> Pending` 복귀와 동일 issuance 보존을 설명하지만, 계약이 요구하는 initial Busy=`AwaitingCaptureRetry`와 Confirm Busy=`AwaitingDecision`의 서로 다른 출력·재시도 소유권을 정의하지 않는다. 같은 Pending capability를 유지하되 두 경로의 owner state, 허용 callback, output row, 재시도 경계를 별도로 고정해야 AC-M5D7QC3-007을 충족한다.

## 정합한 항목

- Q-B의 기존 `_takenRequest`와 Q-A의 기존 intent 슬롯을 덮어쓰거나 되돌리지 않고 successor/append-only 이력으로 분리한다.
- issuance는 opaque reference와 one-shot claim으로 묶고, 같은 Item/Receipt를 복사한 value row·foreign owner/router·다른 epoch를 거부한다. 구현 시 request의 모든 값과 issuance에 묶인 exact row를 비교해야 한다.
- successor cursor가 즉시 `Ready`여도 pending witness가 우선하며, 첫 `TryAdvance`의 실제 frame은 검증 후 해석·활성화 없이 폐기한다.
- 실행 commit은 request CAS, owner `ExecutionCommitted`, callback/rearm 무효화를 먼저 수행하고 그 뒤 `IdentityFor`를 호출한다.
- 제안된 참조 방향은 Hub.Presentation → Input.Unity → Profile을 유지하며 새 friend/asmdef나 역방향 참조를 만들지 않는다.

현재 Hub 파일에는 위 C3 API와 successor 슬롯이 아직 존재하지 않는다. 따라서 이 문서는 구현 승인이나 수용 선언이 아니며, Astra가 두 P1을 계약 보강 또는 구현 설계에 명시한 뒤 Terra 구현과 Luna 독립 검증을 진행해야 한다.
