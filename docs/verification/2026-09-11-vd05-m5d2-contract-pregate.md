# VD-05 M5D2 활성 런·실패 판정 코어 — Luna 계약 사전 게이트

- 검토자: Luna (`gpt-5.6-luna`), 독립 계약 검토
- 검토일: 2026-09-12 (second pass; first pass 2026-09-11)
- 기준 계약: `docs/specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md`
- 비교 문서: VD-05, SYSTEM-CONTRACTS, M5A, M5D1 및 현재 `AcadeGameMaker.Run` runtime
- 판정: **PASS / P0=0, P1=0** (second-pass contract pre-gate)
- 이 문서는 구현 승인이나 `Approved`/`Verified` 전환이 아니다.

## 결론

M5D2의 핵심 경계는 타당하다. M5A Unity 연결보다 VD-05 실패 권위를 먼저 세우고, 실제 Combat/KillPlane/LethalCrush source provenance를 후속 adapter 책임으로 남기며, Unity/IO/RNG/profile/scene을 범위에서 제외한 점은 M5A·M5D1·SYSTEM-CONTRACTS와 일치한다. 동시 실패의 canonical tie-break, sequential tick, mutation-free validation, replay fingerprint, checked successor, Core-only allowlist/friend 구조도 방향이 맞다.

첫 번째 검토에서 제기한 세 P1은 개정 계약에서 명시적으로 닫혔다. 이 문서는 여전히 구현 승인이나 `Approved`/`Verified` 전환이 아니지만, 개정 문서 자체에 대한 Luna second pre-gate는 PASS다.

## Second-pass verdict — 2026-09-12

P0/P1은 없다. 개정 계약은 다음을 모두 닫았다.

| Former finding / review dimension | Second-pass result |
|---|---|
| P1-001 tick width/horizon | **Resolved.** `SimulationTick` is explicitly canonical signed-32 `int`, matching current Core and VD-01; `StartTick` `0..int.MaxValue-1` and checked tick successor are therefore representable. `Revision` is explicitly separate checked signed-64 `long`, never caller-injected or implicitly converted. |
| P1-002 closed-batch/default detection | **Resolved.** `PresenceMask` is required and must equal `AllKnown`; `TriggeredMask` is independent, subset-checked, and unknown bits are rejected. Default `0/0`, all partial masks, unknown bits, and constructor-bypass values are explicitly rejected without false-coercion or mutation. |
| P1-003 RunEndRequested envelope | **Resolved.** `RunFailureIntent.Request` is field-for-field `RunEndRequested(Failed, canonicalCause, acceptedTick)` and carries immutable key, post-commit source revision, exact masks, and ordered observed causes. Consumers are explicitly forbidden to recompose the request. |
| Parent lifecycle mapping | **PASS.** The contract explicitly owns only the `NotStarted → Active → Failed` failure prefix and failure-request dedupe; `Returned`, `RunEndCommitted`, success, and full `AC-RUN-003` remain out of scope. |
| Replay fingerprints / atomicity | **PASS.** Start, prior active no-failure, and terminal fingerprints have defined duplicate/conflict behavior; invalid attempts and both checked successor overflows preserve state/fingerprints and publish no partial result or intent. |
| Source provenance | **PASS.** DTO equality is expressly not authentication; actual owner/graph/tick/publication verification remains a future adapter responsibility, with no invented source provenance. |
| Allowlist / friend access | **PASS.** The allowlist remains limited to the new Core-only Run session, its EditMode tests, and listed documents; existing Run asmdef, `noEngineReferences`, and single Run-test friend remain unchanged. |

### Acceptance-criteria pre-gate status

`AC-M5D2-001` through `AC-M5D2-008`: **PASS recommendation at contract level.** The revised contract now gives each criterion an implementable, testable boundary: signed-32 tick edges and separate signed-64 revision (`001/002/006/007`), exact presence/trigger masks and malformed matrix (`002/004/006`), complete immutable intent-to-snapshot comparison (`003/004/005`), deterministic cause ordering (`004`), replay retention and no-publication overflow atomicity (`005/006`), Core-only/static allowlist checks (`007`), and independent full/grouping evidence requirements (`008`). This is a pre-gate recommendation only; implementation evidence and Astra’s final `Approved`/`Verified` decision remain required.

## P0

없음.

## First-pass P1 findings — resolved by the 2026-09-12 revision

### P1-001 — SimulationTick 폭과 StartTick horizon 불일치 (first-pass; RESOLVED)

> Historical first-pass finding. The revised contract makes signed-32 `SimulationTick` canonical and separates the signed-64 internal revision.

M5D2는 `RunStartInput.StartTick`을 `0..int.MaxValue-1`로 제한하면서 `RunFailureSnapshot`의 revision만 checked signed-64-bit로 명시한다. 검토 시점의 실제 `Assets/AcadeGameMaker/Runtime/Core/SimulationTick.cs`는 `SimulationTick.Value`를 signed `int`로 보유하고, VD-01 계약도 `SimulationTick`을 signed 32-bit wrapper로 정의한다. 반면 상위 설계 전제가 signed-64 SimulationTick이라면 현재 StartTick 범위는 조용한 domain truncation이다.

Astra가 canonical tick domain을 하나로 결정해야 한다. signed-64가 정본이면 StartTick/NextExpectedTick/successor와 overflow 기준을 `SimulationTick` 전체 범위에 맞춰야 한다. signed-32가 정본이면 M5D2에 그 사실과 `int.MaxValue-1` horizon의 이유를 명시하고 signed-64 표현을 revision에만 한정해야 한다. 구현자가 암묵 변환하거나 horizon을 임의로 좁히면 안 된다.

영향: `REQ-M5D2-001/002/004`, `AC-M5D2-001/002/006/008`.

### P1-002 — bool 세 개로는 “누락을 false로 보정하지 않음”을 표현할 수 없음 (first-pass; RESOLVED)

> Historical first-pass finding. The revised contract adds required exact `PresenceMask=AllKnown` independently of `TriggeredMask` and mandates constructor-bypass revalidation.

`RunFailureTickInput`은 세 원인의 `triggered flag`를 bool로 받지만 계약은 세 flag가 모두 존재해야 하며 누락을 false로 보정하지 않는다고 요구한다. valid key/tick과 함께 역직렬화·reflection·default construction으로 일부 flag가 빠진 입력이 들어오면 bool의 `false`와 명시적 false를 구별할 수 없다. “default outer input”의 key 검사만으로는 이 closed-batch invariant를 증명하지 못한다.

세 값의 presence를 nullable bool/명시적 presence mask/closed constructor 등으로 표현하고, default·missing·unknown 입력이 state와 replay fingerprint를 바꾸지 않는 경로를 계약 API와 AC에 고정해야 한다. 명시적 세 bool 인자만 허용한다는 뜻이라면 “missing”의 정의와 constructor-bypass 방어를 명문화해야 한다.

영향: `REQ-M5D2-002/004`, `AC-M5D2-002/006`.

### P1-003 — RunEndRequested intent payload가 SYSTEM-CONTRACTS와 닫히지 않음 (first-pass; RESOLVED)

> Historical first-pass finding. The revised contract embeds the exact request and immutable correlation fields in `RunFailureIntent` and forbids consumer recomposition.

SYSTEM-CONTRACTS의 `RunEndRequested`는 `result, cause, tick`을 갖지만 M5D2는 `RunFailureIntentKind.RunEndRequestedFailed`라는 종류명만 정의한다. `AcceptedRunFailure`가 snapshot에 존재하더라도 downstream owner가 intent만 소비할 때 cause, accepted tick, ordered observed-cause set, run key/revision을 어떻게 원자적으로 연결하는지 계약상 명확하지 않다. 이를 caller가 재조립하면 동일-tick closed batch와 provenance 경계가 다시 열린다.

intent 자체에 최소 failure payload와 source snapshot identity를 포함하거나, intent가 exact immutable snapshot을 필수로 참조·검증한다는 값을 명시해야 한다. canonical cause는 진단 tie-break일 뿐 보상·연출·복귀 의미를 만들지 않는다는 현재 제한은 유지해야 한다.

영향: `REQ-RUN-001/004`, `REQ-M5D2-003/005`, `AC-M5D2-003/004/005`.

## First-pass P2 / 경계 명확화 — retained as implementation cautions

- M5D2의 parent mapping이 개정 문서에 “failure prefix only”로 명시되어 이 경계는 해소됐다. 구현·통합 보고서에서도 `REQ-RUN-001` 전체와 VD-05 `AC-RUN-003`을 완료했다고 과장하지 않아야 한다.
- Start, 직전 accepted no-failure, terminal failure fingerprint의 retention semantics와 invalid-attempt 불변성이 개정 문서의 replay 규칙 및 AC에 유지되어 있다. 구현 시 해당 보존을 테스트로 증명해야 한다.

## 차원별 판정

| 검토 차원 | 판정 | 근거 |
|---|---|---|
| Tick domain / horizon | **PASS** | Revised contract canonically fixes signed-32 `SimulationTick` and reserves checked signed-64 `long` for internal revision. |
| Closed batch / default detection | **PASS** | Exact `PresenceMask=AllKnown` is independent of `TriggeredMask`; default, partial, unknown, and constructor-bypass values are rejected without coercion. |
| Replay retention | **PASS (implementation evidence required)** | Start/last-active/terminal fingerprints, identical replay, changed replay, and invalid mutation-free retention are specified and mapped to ACs. |
| Atomicity / overflow | **PASS (implementation evidence required)** | Validation, both checked successors, complete result/intent allocation, and no publication on overflow precede state publication. |
| Deterministic grouping / tie-break | PASS | exact sequential tick과 `HealthDepleted → KillPlane → LethalCrush` 순서가 render grouping/도착 순서와 분리된다. |
| Source provenance | PASS | pure DTO가 인증 capability가 아님을 명시하고, bound owner/same graph/exact tick 검증을 후속 adapter에 남겼다. |
| Allowlist / friend access | PASS | 새 Run runtime/test 파일만 허용하고 기존 Run asmdef·AssemblyInfo·friend를 변경 금지한다. 현재 Run asmdef는 Core-only/no-engine이며 friend는 Run EditMode test에 한정된다. |

## First-pass remediation conditions — closed by revision; implementation still gated

1. **Closed:** the contract fixes the signed-32 tick / signed-64 revision split and exact horizon.
2. **Closed:** presence-bearing masks and default/partial/unknown rejection are explicit and mapped to ACs.
3. **Closed:** the exact `RunEndRequested` payload and immutable intent linkage match SYSTEM-CONTRACTS.
4. Implementation remains gated on Astra changing this contract to `Approved`; Unity execution, M5A adapter, and scene binding remain outside this contract.
