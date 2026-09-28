# VD-09 M5D7H Primary Input-Recovery Atomic Save — Terra implementation evidence

Status: Verified — Astra accepted the revision-proof remediation after Luna R2 PASS (P0=0, P1=0; residual P2=3), focused M5D7H 25/25, unchanged M5D7D 29/29, full EditMode 647/647, and full PlayMode 576/576. This evidence verifies only primary input-recovery atomic replacement, not launch coordination or actual Input System override application.

## Scope and traceability

- Approved contract: [M5D7H](../specs/work-contracts/2026-09-13-vd09-m5d7h-primary-input-recovery-atomic-save.md); [Luna second pre-gate PASS](2026-09-13-vd09-m5d7h-contract-pregate.md).
- Only `ProfileAtomicSaveServiceV1.cs` was modified; only this new focused fixture/meta and this evidence record were added. M5D7D’s existing test file and M5D3–M5D7G sources/tests remain unchanged.

| Requirement / AC | Implementation evidence |
| --- | --- |
| REQ-M5D7H-001..002; AC-001..004 | Internal primary/decoded recovery-proof validation, M5D7C plan derivation before lease/I/O, and fresh semantic plan identity checks before exactly one replacement. |
| REQ-M5D7H-003..004; AC-005..007 | Existing write-through temp/validate port sequence, injected directory/write/flush/close/temp/source-read precommit faults, replace/postcommit uncertainty faults, replace-only recovery commit, old primary byte capture/post-proof, and separate typed result matrix. |
| REQ-M5D7H-005; AC-008..009 | The existing M5D7D normalized case-insensitive ref-counted registry and file-operation port are reused; no second lock/port is introduced. |
| REQ-M5D7H-006..007; AC-010 | No public ABI change or recovery public API; ordinary `Save` remains additive-only and static focused checks cover authority boundaries. |

## Verification state

Terra performed static allowlist/diff review and did not run Unity from the sandbox. Astra's final deterministic focused/regression/full results are recorded below and were independently accepted by [Luna R2](2026-09-13-vd09-m5d7h-luna-postreview.md).

## Focused compile diagnostic

The first Astra focused invocation did not produce NUnit XML because the M5D7H fixture had a `CS1026` missing-parenthesis error in its deeply nested different-root `Task.Run` document construction. The fixture now uses named local documents for both ordinary and different-root tasks; runtime was not changed. This is diagnostic history only, not a test result.

## Post-review result-proof remediation

Luna identified that a reflection-forged positive committed revision could satisfy the original recovery-success row. The internal M5D7H result now retains an immutable expected-result-revision proof: success requires the returned revision to equal that proof, while precommit and uncertainty rows require both revision fields to be `-1`. The focused fixture mutates the returned success revision to both `-1` and positive `999`, asserting `Validate()` and the internal getter fail closed. Ordinary M5D7D public result behavior is unchanged. The fresh R3/R2 artifacts below supersede the earlier R1 anchors; Luna R2 re-review remains pending.

## Earlier R1 deterministic-result provenance

| Item | Result / SHA-256 |
| --- | --- |
| `ProfileAtomicSaveServiceV1.cs` | `e7a0203c76e1e4a9aa41dda06ace9dbe63f8a50e51781e166e60aee0b23d6592` |
| `ProfileInputRecoveryAtomicSaveServiceV1Tests.cs` | `082fb000ebe436d4dce4353fb8e0d1f30901682d7d6099161f6dfc9ded6a4b51` |
| `ProfileInputRecoveryAtomicSaveServiceV1Tests.cs.meta` | `cb0431ce7e282125dbe4e78d8f30285212bf9fe85c0522798b708d4ed675d5e3` |
| Focused M5D7H EditMode | 25/25 passed; `artifacts/m5d7h-20260913/focused-editmode-r2.xml`; `a20ee0660ea87b688cc27e4e68655b901953b16ca4304452d51b7b36ccb5a981` |
| Unchanged M5D7D focused regression | 29/29 passed; `artifacts/m5d7h-20260913/m5d7d-focused-regression.xml`; `62c511b2298d71296421884dee01e41f7e535ad807e95f840d9b17970c68643d` |
| Full EditMode | 647/647 passed; `artifacts/m5d7h-20260913/full-editmode.xml`; `569b36ed08b82944b87a1a4d2f6cc62088b1034f6524894c449acf21b468900f` |
| Full PlayMode | 576/576 passed; `artifacts/m5d7h-20260913/full-playmode.xml`; `accc789c7b234640d847ff7cb773b7bec6f434083b200b28e95806a3f358b960` |

## Current revision-proof scoped hashes and deterministic results

| Item | Result / SHA-256 |
| --- | --- |
| `ProfileAtomicSaveServiceV1.cs` | `0a6926e65bd5aea177fbdaf4c0768232ed209ea0fe713cc0ee22220a5aa3b894` |
| `ProfileInputRecoveryAtomicSaveServiceV1Tests.cs` | `c601560ce285b9774f64800ececbfabed3140ab6d9795a424b43bef78ba76f06` |
| `ProfileInputRecoveryAtomicSaveServiceV1Tests.cs.meta` | `cb0431ce7e282125dbe4e78d8f30285212bf9fe85c0522798b708d4ed675d5e3` |
| Focused M5D7H EditMode R3 | 25/25 passed; `artifacts/m5d7h-20260913/focused-editmode-r3.xml`; `1a75d03a576e9a3052dbf93c7b1f5c5bbc0dfce9cdca9f79538ad46588f49543` |
| Unchanged M5D7D focused regression R2 | 29/29 passed; `artifacts/m5d7h-20260913/m5d7d-focused-regression-r2.xml`; `786e9396434d3c864e9e143e83ac40b00bb0bfba46938f2f8cfdf157b7d118e8` |
| Full EditMode R2 | 647/647 passed; `artifacts/m5d7h-20260913/full-editmode-r2.xml`; `7aa5da79f66123749dba37dde04779ce31e09bc98ded243808cfe6ef65b0fafb` |
| Full PlayMode R2 | 576/576 passed; `artifacts/m5d7h-20260913/full-playmode-r2.xml`; `0d1605cb59078f43543cdb76290b6b8ccc6d8a961b6069536ca99955a0b9d0d0` |
