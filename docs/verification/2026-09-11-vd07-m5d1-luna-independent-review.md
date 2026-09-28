# VD-07 M5D1 오르단 플레이어블 터미널 전환 — Luna 독립 검증

- 검증자: Luna (`gpt-5.6-luna`), 독립 검토
- 검증일: 2026-09-11
- 기준 계약: `docs/specs/work-contracts/2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md`
- 구현 인계: `docs/verification/2026-09-11-vd07-m5d1-terra-handoff.md`
- 판정: **PASS 권고 / Astra 최종 통합 판정 대기** (R13 독립 검증 완료; 이 보고서는 계약을 `Verified`로 표기하지 않음)

## 결론

정적 구조와 authored asset은 계약의 주요 shape를 보존한다. requester는 `-211`, router는 `-210`이고, `Systems`의 router/requester와 별도 `Camera` child의 연결, 초기 `GameplayEnabled`, PPU18/640x360/Windowbox/UpscaleRenderTexture/Point/orthographic10 프로필이 파일 수준에서 확인된다. R13 실행 증거까지 독립적으로 대조했으며 P0/P1/P2 결함은 발견하지 않았다.

R13의 requester PlayMode 9/9, authored EditMode 22/22, 기존 실패 fixture 1/1, 전체 EditMode 483/483, 전체 PlayMode 576/576 XML은 모두 `Passed`이며 failed/skipped/inconclusive가 0이다. 로그는 Unity `6000.6.0f1` 실행과 license entitlement 해소 후 결과 저장을 보여준다. R12에서 runtime clone을 위해 제거한 root-name gate는 계약이 `EncounterKey`를 validation/receipt 상수로만 정의한다는 경계에 부합한다. runtime은 대신 명시 참조, 동일 root, bound player/combat/transfer/handoff를 fail-closed로 검증하고, authored validation은 여전히 정확한 `OrdanBossEncounterGraph` 이름을 요구한다. R13은 legacy manual fixture가 새 camera/router/requester를 수동 phase 전에 disable하도록 제한적으로 보정했으며, runtime authority를 넓히지 않는다.

## P0/P1/P2

### P0

없음.

### P1-001 — requester 초기화 시 Transfer graph identity를 검증하지 않음 — **R1 해결, R13 실행 확인**

R1에서 `BoundTransferForAuthoring`를 읽어 null, requester와 동일 root, 동일 player binding을 확인하도록 보강했다. foreign-root Transfer fixture가 초기화를 예외로 거부하고 router/requester 상태를 보존하는지 검사하며, R13 requester XML에서 해당 경로를 포함한 9개가 모두 통과했다. runtime clone은 이름이 아니라 동일 graph binding을 검증하고, authored validation은 exact root name을 유지한다.

영향 AC: `AC-M5D1-001`, `AC-M5D1-002`, `AC-M5D1-003` (해결 및 실행 확인).

### P1-002 — AC-M5D1-004 요구 실패 모드에 대한 독립 테스트 공백 — **R1 해결, R13 실행 확인**

R1은 전용 requester 테스트를 추가했다: router commit 전 duplicate observation, disable→reenable, source tick `long.MaxValue` overflow, router fault를 각각 검사하고, request/pending receipt/retry/forged completion의 부재를 assertion한다. 기존 conflicting pending assertion과 함께 계약 경로가 의미 있게 덮이며, R13의 9/9 requester XML에서 실제 assertion 결과가 모두 통과했다.

영향 AC: `AC-M5D1-004` (해결 및 실행 확인).

### P2

없음. R13 이후 미해결 P2는 없다.

## AC별 상태

| AC | 상태 | 독립 근거 |
|---|---|---|
| `AC-M5D1-001` | **PASS 권고** | authored structure/validator focused EditMode `22/22` 및 full EditMode `483/483` PASS; exact component/reference/order mutation assertions 포함. |
| `AC-M5D1-002` | **PASS 권고** | requester focused PlayMode `9/9` 및 full PlayMode `576/576` PASS; normal no-op, incomplete/conflict/stale/cross-wire paths 실행 확인. |
| `AC-M5D1-003` | **PASS 권고** | focused requester exact death path PASS; `Transition@t+1`, epoch +1, locked empty Movement/Transfer/Combat payload assertions 통과. |
| `AC-M5D1-004` | **PASS 권고** | focused requester `9/9` PASS; duplicate, disable→reenable, overflow, router-fault, conflicting request 및 immutable completion assertions 통과. |
| `AC-M5D1-005` | **PASS 권고** | authored camera/profile EditMode와 completed-camera/unsupported-output PlayMode coverage가 full suites에서 PASS; focused authoring `22/22`도 PASS. |
| `AC-M5D1-006` | **PASS 권고** | focused requester `9/9`, focused authoring `22/22`, former failure `1/1`, full EditMode `483/483`, full PlayMode `576/576`; 모두 failed/skipped/inconclusive `0`. |

## 실행·정적 증거

- `qa/tools/Test-QaCatalog.ps1`: PASS — 13 scenarios / 68 AC.
- `qa/tools/Test-QaCatalog.ps1 -SelfTest`: PASS — 7 mutation checks.
- R13 focused requester: `artifacts/m5d1-20260911-reauth/requester-playmode-unsandboxed-r12.xml` — `9/9`, failed/skipped/inconclusive `0`.
- R13 focused authoring: `artifacts/m5d1-20260911-reauth/authoring-editmode-unsandboxed-r2.xml` — `22/22`, failed/skipped/inconclusive `0`.
- R13 former failure: `artifacts/m5d1-20260911-reauth/audit-exposure-playmode-r13.xml` — `1/1`, failed/skipped/inconclusive `0`.
- R13 full EditMode: `artifacts/m5d1-20260911-reauth/full-editmode-unsandboxed.xml` — `483/483`, failed/skipped/inconclusive `0`.
- R13 full PlayMode: `artifacts/m5d1-20260911-reauth/full-playmode-unsandboxed-r3.xml` — `576/576`, failed/skipped/inconclusive `0`.
- Unity logs: `6000.6.0f1`; licensing client connected, entitlement details resolved, and all five XML results were saved after execution.
- `git diff --check` on scoped M5D1 files: PASS.
- R13 source review: PASS for compile plausibility, authority/tick/lifecycle behavior, exact authored-vs-runtime identity boundary, and M5D1 allowlist; no runtime implementation change in the R13 legacy-fixture correction.
- Current source hashes: requester `46CF17CAE4046FF9BCA7E06D5AED40666D56D6BF7AEA96380FAEC971DE297D32`; requester tests `D483FBBB753CC697CD652E402CBDBDC16B82B2A89855854C652B52C011645B17`; legacy audit test `139058F187EC807B7F0791ED71C4BCC9439B48566390C65D7A9823D279463344`; prefab `4444E2417FE8C24446BAF4044CD28F738341082C9E45CFA58F9561FAEED2B139`; scene `12C30280E922F661C4B6AF4D31810537EC39ACC71461770408CE4AAC2999999F`.
- XML SHA-256: requester `576E33BC1E4ADFFFDC605661F339602D316DBAD44D85E19A206F9B8CEBA06861`; authoring `0FD455BACBF9F96F82AB2D2A61F3D865BCE50174960BF8093FCC71337DA7A569`; former failure `7FFD177EF23D49563A98E2F5CCE1E023ACA24A7372053A5B5077314D45586EF9`; full EditMode `AF4F81E124D4B90A9FE34F58F8D53FDE8899C1534D960545C3F9C0F7CEED524A`; full PlayMode `67BD473C6994B338D1CC9FE303F9591ACA114C52C1689E9D264434D87380BD40`.

## Required follow-up before Astra acceptance

1. Astra to perform the final integration/contract status decision using this independent PASS recommendation.
2. This report intentionally does not mark the contract `Verified`; no further Luna test run is required for the reported R13 evidence.
