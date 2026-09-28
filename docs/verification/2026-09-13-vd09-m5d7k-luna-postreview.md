# VD-09 M5D7K launch preservation sequence — Luna independent post-review

- Contract: [Approved M5D7K](../specs/work-contracts/2026-09-13-vd09-m5d7k-launch-preservation-sequence.md)
- Reviewer: Luna (independent; Astra retains acceptance authority)
- Review scope: current executor, final 11-test fixture, friend declaration, implementation evidence, and supplied XML artifacts
- Review date: 2026-09-13

## Verdict

**PASS — P0=0, P1=0, residual P2=2.** Recommend Astra accept the M5D7K implementation for integration; Astra, not Luna, owns the final status transition.

The executor's sequencing and fail-closed result proof are coherent, and the final execution artifacts are internally consistent. The prior AC-M5D7K-004 blocker was closed by injecting a valid M5D7F `FailedBeforeMove` result with `SourceState.Unreadable` and asserting the primary stop state and no continuation.

## Evidence verification

I independently parsed each XML's NUnit `test-run` counters and recomputed SHA-256:

| Artifact | Result | SHA-256 |
|---|---:|---|
| `artifacts/qa/m5d7k-focused-r5.xml` | 11/11, failed/skipped/inconclusive 0 | `383b28ade519207275a6a1f3eae29cad77f7cbe7b30d21f3fa421a022e2ca27b` |
| `artifacts/qa/m5d7k-m5d7e-regression.xml` | 10/10, failed/skipped/inconclusive 0 | `ede5b5f49b28b9d982c5585e4a8791af2b5a9d90017c1b36269396df6b54d441` |
| `artifacts/qa/m5d7k-m5d7f-regression.xml` | 18/18, failed/skipped/inconclusive 0 | `999f8704368f3325556054b265d4e936897fa087febb9b31656af533d4cdccdc` |
| `artifacts/qa/m5d7k-editmode-full-r2.xml` | 663/663, failed/skipped/inconclusive 0 | `b0c8c9c29278bce6bd1d4638c784dfbb08065b362e3f35c138bc06c70a1ebb56` |
| `artifacts/qa/m5d7k-playmode-full-r2.xml` | 584/584, failed/skipped/inconclusive 0 | `0c2ea8cf42dc53f83e160966b2098f81a87660dfbec42c60f7d7b588687fb64a` |

Current scoped source hashes match the updated implementation evidence:

- Executor: `d2844c5697f58de47933be87b82dc8a45496f7dbe7b17adc3cd01491926fe8fa`
- `AssemblyInfo.cs`: `d210906460567de4d54f364beeabd191b9a6a86391265fdc50881dd69f6fd1d1`
- Executor fixture (updated R2): `176b517b7c4b05e412ded5b36464038bf6995f1db573323cca83411b1a2eaa3f`
- `AssemblyInfo.cs.meta` (independently checked): `5181e13a417a6c8fba47c0f19c7c387c7c29be9c5f951f29f5f830d52330b511`
- Updated implementation evidence: `0559c25863b06f983c84990332e0a9f9d788e72a61d11f25f0879d8c9cc1749a`

Unity 6000.6.0f1 execution is accepted as the supplied evidence for this review; no additional Unity run was performed.

The updated XMLs were independently parsed and rehashed after the unreadable-result fix: focused R5 `11/11` (`383b28ade519207275a6a1f3eae29cad77f7cbe7b30d21f3fa421a022e2ca27b`), full EditMode R2 `663/663` (`b0c8c9c29278bce6bd1d4638c784dfbb08065b362e3f35c138bc06c70a1ebb56`), and full PlayMode R2 `584/584` (`0c2ea8cf42dc53f83e160966b2098f81a87660dfbec42c60f7d7b588687fb64a`). Each has failed/skipped/inconclusive counters equal to zero.

## Implementation audit

- Before any port call, the executor rejects null ports, non-zero-offset UTC, relative/root paths, invalid observation/selection values, and non-identical public selection semantics. Root normalization matches the M5D7F boundary.
- The execution loop forwards the observation candidates and selected intents in `Temp → Primary → Previous` order, skips `None`, calls each required role once, and stops immediately on `FailedBeforeMove` or `MoveOutcomeUncertain`.
- `ProfileLaunchPreservationResultV1.Validate()` closes the three-role state machine: resolved states continue, later required roles become `NotAttempted`, later `None` remains `NotRequired`, the attempted prefix and stop role must agree, and `MayPersist` is true only for a fully resolved prefix. Every public getter calls `Validate()`.
- Returned M5D7F results are validated and their role/reason are matched to the request. Unexpected and fatal exceptions are not caught or converted into persistence permission.
- Static review found one production delegation to `ProfileQuarantineServiceV1.Preserve`, no save/observation/Unity/path-source/clock authority, and the only new friend declaration is the exact `InternalsVisibleTo("AcadeGameMaker.Profile.EditMode.Tests")` line.

## Acceptance matrix

| Acceptance criterion | Luna status | Basis |
|---|---|---|
| AC-M5D7K-001 | PASS | No-intent result, no port call, all `NotRequired`, `MayPersist=true`. |
| AC-M5D7K-002 | PASS | Exact root/UTC/candidate capture, ordered calls, resolved mixed outcomes. |
| AC-M5D7K-003 | PASS | Both stop outcomes at each required position plus explicit later-`None` row and exact state assertions. |
| AC-M5D7K-004 | PASS (P1-001 resolved) | Readable files prove exact names/hash8 and source removal; deterministic branch now returns a valid M5D7F `FailedBeforeMove` result with `SourceState.Unreadable` and asserts the exact primary stop/no-continuation state. |
| AC-M5D7K-005 | PASS with P2 note | Defaults, path/UTC, semantic mismatch, and reflected selection corruption are zero-call failures; observation-candidate reflection is not separately exercised. |
| AC-M5D7K-006 | PASS | Role/reason mismatch, default/corrupt result, unexpected and fatal representatives stop at one call; subsequent valid execution succeeds. |
| AC-M5D7K-007 | PASS | Every result backing field is reflection-mutated and getters fail closed; stop-prefix rows are asserted. |
| AC-M5D7K-008 | PASS | One stateless production delegation, injected instance port, and forbidden-authority source scan. |
| AC-M5D7K-009 | PASS | All supplied focused/regression/full XMLs are green with no failed/skipped/inconclusive tests; Luna recheck is P0=0/P1=0. |

## Findings

### P1-001 — AC004 unreadable result was not initially exercised (RESOLVED)

In the first review, `AC004_UnreadableIntentIsForwardedDeterministically` constructed `ProfileLoadCandidateV1.Unreadable(Primary)` but configured the port to return `SourceMissing`. The fixture's `MakeResult` represented `FailedBeforeMove` with `SourceState.Present` and did not prove an exact unreadable result.

Terra/Astra's bounded fix now injects `FailedBeforeMove` with `SourceState.Unreadable`, captures the returned result, and asserts `PrimaryState=FailedBeforeMove`, `StoppedRole=Primary`, later `NotRequired` states, and one call. The final focused R5 and full-suite artifacts are green. This P1 is closed.

### P2-001 — Observation candidate reflection is not a distinct test

AC005 names invalid/reflection-corrupted observation candidates. The fixture tests a default observation batch and reflected selection/result fields, but does not mutate a private candidate field inside an otherwise valid observation and assert a zero-call rejection. The production `Validate()` path is appropriately fail-closed; this is a coverage gap rather than a source defect.

### P2-002 — Candidate equality assertion is intentionally shallow

The forwarding assertion checks role, kind, classification, and unsupported schema version, but does not compare every nested canonical-document field/byte array. The runtime passes the observation candidate directly, and the contract explicitly treats separately produced plans with identical public semantics as equivalent, so no concrete runtime failure was found. A full byte-level candidate assertion would make the exact-forwarding proof stronger.

## Recheck after P1-001 remediation

The new unreadable branch and reflected-observation-candidate zero-call case are present in the final fixture. `ProfileLaunchPreservationExecutorV1.cs` is unchanged from the previously reviewed runtime hash; only the allowlisted test fixture changed. The deterministic result is protocol-valid under M5D7F's result invariants and is mapped to the correct fail-closed K state.

No P0 or P1 findings remain. The two residual P2 observations above do not demonstrate a runtime violation: the source forwards the exact observation candidate object and the contract intentionally defines selection identity by public semantics rather than historical private proof.

## Recommendation

The remediation is sufficient for Luna's independent gate. Astra may integrate after its normal review and status decision; this report does not mark the contract Verified.
