# VD-05 M5D2 활성 런·실패 판정 코어 — Terra 구현 증적

- 상태: **Implemented — Luna 독립 검증 및 Astra 통합 수용 대기**
- 기준 계약: [M5D2 Run failure arbitration core](../specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md)
- 계약 사전검토: [Luna second-pass PASS](./2026-09-11-vd05-m5d2-contract-pregate.md)
- 구현자: Terra
- 수용 권한: Astra. 이 문서는 구현자 자기 수용이나 실제 run source/Unity 연결을 주장하지 않는다.

## 구현 경계

`RunFailureArbitrationSession`은 새 Core-only Run owner다. 한 instance-local key의 `NotStarted → Active → Failed` failure prefix, exact sequential signed-32 `SimulationTick`, separate checked signed-64 revision, closed three-cause batch, immutable one-shot `RunEndRequested(Failed)` intent와 replay fingerprint만 소유한다.

Unity component, actual Combat/KillPlane/LethalCrush source provenance, M5A adapter, `RunEndCommitted`, cleanup/return/success, profile/choice/skill state, input/scene/asset authority는 구현하지 않았다.

## REQ / AC 구현 매핑

| 요구사항 / 수용 기준 | 구현 및 전용 테스트 근거 |
|---|---|
| REQ-M5D2-001 / AC-M5D2-001 | `RunFailurePhase`, immutable `RunFailureSnapshot`, instance-local start fingerprint; `AC001_StartBoundariesDuplicatesConflictsAndIndependentInstancesAreExact` covers start ticks `0` and `int.MaxValue-1`, duplicate/conflict, invalid horizons, and independent sessions. |
| REQ-M5D2-002 / AC-M5D2-002 | `RunFailureTickInput` requires `PresenceMask=AllKnown`, subset-checked triggered mask and exact active tick; `AC002_SequentialNoFailureTicksReplaysAndChangedReplaysRetainFingerprints` covers no-failure progression, duplicate, changed replay, stale and skip. |
| REQ-M5D2-003 / AC-M5D2-003 | `OrderedObserved`, `AcceptedRunFailure`, `RunEndRequested`, and `RunFailureIntent` create an exact failed envelope and correlation metadata; `AC003_EachSingleCausePublishesOneCompleteImmutableFailureEnvelope` covers all three single causes and defensive immutability. |
| REQ-M5D2-003 / AC-M5D2-004 | `AC004_AllMultiCauseMasksUseCanonicalOrderingAndFirstCause` covers all three 2-bit masks and `AllKnown`, asserting canonical observed ordering and first-cause tie-break without cause-specific gameplay behavior. |
| REQ-M5D2-004 / AC-M5D2-005 | start/last-active/terminal fingerprints are separate; `AC005_TerminalReplaysAreDuplicatesAndAllOtherPostFailureInputsAreAtomicErrors` proves terminal one-shot replay and mutation-free changed/post-terminal rejection. |
| REQ-M5D2-004 / AC-M5D2-006 | normalized constructor-bypass revalidation, partial/default/unknown masks, invalid key/ticks, signed-32 successor overflow and reflected signed-64 revision-overflow atomicity are covered by the two `AC006_*` tests. |
| REQ-M5D2-005 / AC-M5D2-007 | `AC007_CoreOnlySourceUsesSeparateCheckedSuccessorsAndNoRuntimeAuthority` checks Core-only assembly references, the two checked successors, exact failed request construction, and forbidden runtime authority tokens. |
| AC-M5D2-008 support | `AC008_ThirtySixtyAndOneFortyFourGroupingTracesAreExact` produces equal 30/60/144 grouping traces with `firstDivergence=none`; it is deterministic grouping evidence, not a performance claim. |

## Changed files

- `Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs`
- `Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Run/RunFailureArbitrationSessionTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Run/RunFailureArbitrationSessionTests.cs.meta`
- `docs/specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md`
- `docs/verification/2026-09-11-vd05-m5d2-implementation-evidence.md`
- `docs/README.md`

No asmdef, AssemblyInfo, existing runtime/test, asset, scene, package, or project-setting file was changed for M5D2.

## Static verification performed

- Scoped `git diff --check`: PASS.
- Source review: `SimulationTick` successor is `checked(normalized.Tick.Value + 1)`; revision successor is `checked(_revision + 1)`; their types remain `int` and `long` respectively.
- Source review: intent construction is exact `new RunEndRequested(RunResult.Failed, accepted.Cause, accepted.Tick)`; public result/observed collections are copied to read-only arrays.
- Source review: no `UnityEngine`, IO, network, wall-clock, RNG, scene, InputRouter, profile, or callback authority token in the new runtime source.
- The local standalone C# compiler is unavailable, so Terra makes no compile/test-pass claim. Unity execution is intentionally deferred to root's authorized runner.

## Source SHA-256

| File | SHA-256 |
|---|---|
| `RunFailureArbitrationSession.cs` | `047C6F5573C367D417FD9BC8EDB163EBB2930DC494333FB07CFF002A3D5F902B` |
| `RunFailureArbitrationSession.cs.meta` | `D67C991514D54F78F1441F70B4DCD46C6B85EC89A45FA830FBA167D23364690D` |
| `RunFailureArbitrationSessionTests.cs` | `2063E9B60A3C9B2497F192CDFE9BF03587AE55B4F3F04EE71ABB271294911952` |
| `RunFailureArbitrationSessionTests.cs.meta` | `C05AE0D038C0B551AA530449530FF65DE846B3EAC1C5C87CDCCE308167E8162A` |

## R1 focused-compile correction

- Focused Unity compilation identified CS0051 only: the NUnit-discoverable public AC003 test method exposed internal `RunFailureCauseMask` and `RunFailureCause` parameter types.
- AC003 now accepts public primitive `int` literals and immediately casts them to the internal types inside the method. Runtime visibility, assembly topology, and runtime source are unchanged.
- This correction was checked by source inspection only; no Unity invocation was performed in this turn.

## R2 root execution evidence

- Licensing preflight: PASS on Unity `6000.6.0f1`.
- Focused EditMode: `11/11` passed; fail/skip/inconclusive `0`; XML SHA-256 `50C4BCD36FDBD527F31F503C85C6C20F30584A34CA3A96A38AC6FE7A19F2DDB7`.
- Full EditMode: `494/494` passed; fail/skip/inconclusive `0`; XML SHA-256 `9B373826DF085D1E05B0DD5026F0BE5161D53319C7F005B0AD5E310D53814866`.
- Full PlayMode: `576/576` passed; fail/skip/inconclusive `0`; XML SHA-256 `637779ABFF24D22823A03C2B058E907C1B8F1890403FD265C11324E829082584`.
- QA catalog coverage: `13/68`; QA self-test: `7` passed.
- These are root-provided execution results. They do not constitute Terra self-acceptance; Luna independent review and Astra integration acceptance remain pending.

## Execution handoff

Terra did not invoke Unity for this task. Root's licensing-authorized execution is recorded in R2 above. Luna must independently review the runtime/test boundary, malformed/overflow and replay counterexamples, full envelope, and 30/60/144 trace before Astra decides integration acceptance.
