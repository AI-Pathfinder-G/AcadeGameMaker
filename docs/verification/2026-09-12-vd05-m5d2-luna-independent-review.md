# VD-05 M5D2 활성 런·실패 판정 코어 — Luna 독립 구현 검증

- 검토자: Luna (`gpt-5.6-luna`)
- 검토일: 2026-09-12
- 기준 계약: [VD-05 M5D2 run failure arbitration core](../specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md)
- 구현 증적: [Terra implementation evidence](./2026-09-11-vd05-m5d2-implementation-evidence.md)
- 판정: **PASS recommendation / P0=0, P1=0, P2=0**
- 이 문서는 계약의 `Verified` 전환이 아니며, Astra가 최종 통합·수용 권한을 가진다.

## 결론

독립 source/test review와 제공된 실행 증적에서 M5D2의 승인된 Core-only failure prefix 구현을 막는 P0/P1/P2는 발견되지 않았다. 구현은 `NotStarted → Active → Failed`와 instance-local replay dedupe만 소유하고, M5A/Unity/source producer/profile/choice/return/success 권위를 만들지 않는다. Astra에 **AC-M5D2-001..008 PASS 통합 권고**한다.

이는 실제 Combat·KillPlane·LethalCrush source provenance, M5A adapter, playable run lifecycle의 검증이 아니다. 계약의 명시된 비범위와 동일하다.

## Evidence independently checked

- Focused EditMode: `11/11`, failed/skipped/inconclusive `0`, XML SHA-256 `50C4BCD36FDBD527F31F503C85C6C20F30584A34CA3A96A38AC6FE7A19F2DDB7`.
- Full EditMode: `494/494`, failed/skipped/inconclusive `0`, XML SHA-256 `9B373826DF085D1E05B0DD5026F0BE5161D53319C7F005B0AD5E310D53814866`.
- Full PlayMode: `576/576`, failed/skipped/inconclusive `0`, XML SHA-256 `637779ABFF24D22823A03C2B058E907C1B8F1890403FD265C11324E829082584`.
- Unity `6000.6.0f1`; the execution logs show entitlement resolution and license update succeeded.
- Focused XML contains the eleven M5D2 tests, including all AC test methods and the `firstDivergence=none` 30/60/144 grouping trace. The supplied QA context independently parses as 13 catalog scenarios covering 68 AC; the recorded validator self-test is 7/7 PASS.
- Runtime SHA-256: `047C6F5573C367D417FD9BC8EDB163EBB2930DC494333FB07CFF002A3D5F902B`.
- Test SHA-256: `2063E9B60A3C9B2497F192CDFE9BF03587AE55B4F3F04EE71ABB271294911952`.

## AC status

| Acceptance criterion | Independent result | Review basis |
|---|---|---|
| AC-M5D2-001 | **PASS** | `Start` at `0` and `int.MaxValue-1`, exact duplicate/conflict behavior, invalid start horizon, and independent sessions are tested and source-consistent. |
| AC-M5D2-002 | **PASS** | Sequential `AllKnown/None` progression increments next tick and revision once; latest no-failure fingerprint duplicate, changed replay, stale, and skip remain mutation-free. |
| AC-M5D2-003 | **PASS** | Single causes produce `Failed`, `RunActive=false`, one intent, exact `(Failed,cause,tick)`, post-commit source revision, masks, and ordered observed set. Snapshot/intent correlation is source-consistent. |
| AC-M5D2-004 | **PASS** | All seven non-empty masks (`1,2,4,3,5,6,7`) are covered; canonical `HealthDepleted → KillPlane → LethalCrush` ordering is independent of enumeration order and emits one intent. |
| AC-M5D2-005 | **PASS** | Terminal fingerprint is retained for identical later replay; duplicate is empty and state-preserving, while changed terminal inputs and later ticks fail without mutation. |
| AC-M5D2-006 | **PASS** | Default, six partial presence masks, unknown presence/trigger bits, invalid key/ticks, constructor-bypass values, signed-32 successor overflow, and reflected `long` revision overflow are rejected before publication. A subsequent valid input remains usable. |
| AC-M5D2-007 | **PASS** | Source uses separate `int` tick and `long` revision with explicit checked successors; output cause lists/intent lists are defensive read-only copies; Run assembly remains Core-only/no-engine and contains no new gameplay authority. |
| AC-M5D2-008 | **PASS** | Focused, full EditMode, and full PlayMode XML all have zero failure/skip/inconclusive; 30/60/144 grouping traces are equal with no divergence. |

## Adversarial source/test review

### Replay and atomicity

`_startFingerprint`, `_lastActiveFingerprint`, and `_terminalFingerprint` are separate. The latest accepted no-failure input is the only active replay fingerprint; a same-tick identical input is `Duplicate`, while a changed same-tick input conflicts. The terminal fingerprint is checked first and remains replayable after failure; all changed or subsequent inputs are rejected. Invalid normalization, tick successor overflow, revision overflow, result construction, and intent construction occur before state assignment, so the retained state/fingerprint cannot be partially advanced.

### Masks and constructor-bypass behavior

`Normalize` reconstructs both input structs through their validating constructors at the session boundary. `PresenceMask` must equal `AllKnown`; `TriggeredMask` must contain only known bits and is not used as presence. The tests exercise default, partial, unknown, and reflection-rewritten constructor-bypass inputs. No missing flag is coerced to `false`.

### Boundary ticks and revision

`StartTick=int.MaxValue-1` accepts one observation and publishes `NextExpectedTick=int.MaxValue`; an observation at `int.MaxValue` throws before publication. The source uses explicit `checked(normalized.Tick.Value + 1)` and `checked(_revision + 1)`. Revision has no input property and the reflected `long.MaxValue` counterexample proves no overflow publication.

### Envelope and immutability

The failure path constructs `RunEndRequested(RunResult.Failed, accepted.Cause, accepted.Tick)` and then places it in `RunFailureIntent` with the same run key, post-commit revision, exact masks, and canonical observed causes. `AcceptedRunFailure`, `RunFailureIntent`, and `RunFailureResult` each copy collection values into `Array.AsReadOnly` wrappers. Tests reject mutation of the returned intent collection and accepted observed-cause collection; the shared copy helper is also used for intent observed causes.

### Authority and allowlist

The runtime has no Unity/IO/network/wall-clock/RNG/callback/scene/profile/input authority and references only `AcadeGameMaker.Core`. The Run asmdef retains `noEngineReferences=true` and Core-only references; `AssemblyInfo.cs` grants friendship only to `AcadeGameMaker.Tests.EditMode.Run`, whose Editor-only asmdef references Run/Core. The test-only CS0051 correction changes only a public NUnit parameter signature to primitive values and does not expand runtime visibility or authority.

## Findings

### P0

None.

### P1

None.

### P2

None. The tests are intentionally bounded to the approved pure-core contract; missing source-owner authentication and Unity/M5A integration are documented non-scope, not findings against M5D2.

## Final recommendation

Recommend Astra integrate M5D2 as **PASS for AC-M5D2-001..008** and retain the contract’s status distinction: this evidence verifies only the bounded Core failure owner. Do not mark the contract `Verified` from this report; do not infer source provenance, failure cleanup, persistence, playable scene flow, M5A connection, or the full VD-05 lifecycle.
