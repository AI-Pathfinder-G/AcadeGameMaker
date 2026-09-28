# VD-09 M5D7D atomic profile save adapter — Terra implementation evidence

- Status: **Verified — Astra accepted after Luna R2 PASS (P0=0, P1=0)**
- Contract: [M5D7D atomic profile save adapter](../specs/work-contracts/2026-09-13-vd09-m5d7d-atomic-profile-save-adapter.md)
- Dependencies: M5D7A-M5D7C Verified
- Independent review: [Luna M5D7D implementation review](2026-09-13-vd09-m5d7d-luna-independent-review.md)

## REQ / AC mapping

| Requirement / acceptance | Implementation and focused coverage |
|---|---|
| REQ-M5D7D-001 / AC-M5D7D-001..003 | The BCL adapter validates the supplied current-compatible canonical document before leasing, writes only the canonical file bytes with create/exclusive/write-through/full-write/flush-to-storage/close, then rereads and decodes before any commit call. Real isolated-directory tests cover first save, replacement, and deterministic directory/write/flush/close/temp-read failure stages. |
| REQ-M5D7D-002 / AC-M5D7D-002,005,009 | The adapter owns only exact sibling leaves `profile.json`, `profile.prev.json`, and `profile.tmp.json`. Existing primary is exclusive-read and current-valid before the one `File.Replace(temp, primary, previous, true)` call; first save uses same-directory `File.Move`. There is no primary pre-delete, copy/delete, rollback, retry, or fallback path. |
| REQ-M5D7D-003 / AC-M5D7D-003,004 | Pre-commit failures return typed `FailedBeforeCommit`; a thrown commit or failed postcheck independently probes primary/previous/temp and returns `CommitOutcomeUncertain` with revision `-1`. |
| REQ-M5D7D-004 / AC-M5D7D-002..005 | The internal file-operation port exposes directory/open/write/flush/close/read/replace/move boundaries for deterministic EditMode fault and order tests. It is not a public gameplay API. |
| REQ-M5D7D-005 / AC-M5D7D-006..008 | Fully-qualified non-root path validation precedes lock/IO; ordinal-ignore-case leases retain active and waiting callers; result getters revalidate a closed outcome/stage/state/revision matrix. |
| REQ-M5D7D-006 / AC-M5D7D-009,010 | The adapter is engine-free and has no profile transformation, load selection, quarantine, recovery retry, input application, Unity path, notification, clock, RNG, or network authority. |

## Scoped implementation files

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs.meta`

| File | SHA-256 |
|---|---|
| `ProfileAtomicSaveServiceV1.cs` | `32114257682BB19D8A48F15D9460B7E2437239ED23EDC8EA651A95026415A0FC` |
| `ProfileAtomicSaveServiceV1.cs.meta` | `DE020BF9FBB5059399796460931AD619A782561EF8828AECE5F8743A495A1C80` |
| `ProfileAtomicSaveServiceV1Tests.cs` | `B7D562800E0ADA31869EB731A85FE2EE7FDA851E66B0C3613484AC1B54F821DF` |
| `ProfileAtomicSaveServiceV1Tests.cs.meta` | `B35C122B3514FFFC6F2BA62B9E85329ED2147A68255CC4B42652DEAE2D2DE3E0` |

## Root execution

The initial focused run exposed a test-harness-only `DispatchProxy` construction error (sealed proxy base), followed by one missing concurrency release fixture. Terra corrected only the allowlisted test file. Luna's first post-review then found that `File.Exists` collapsed access errors into missing and that broad catches swallowed unexpected/fatal exceptions. Terra replaced the boolean check with typed `File.GetAttributes` presence probing and restricted stage catches to recoverable filesystem/security exceptions. Unexpected and fatal exceptions now escape while the lease still releases. Astra reran the final evidence set on Unity `6000.6.0f1`; all licensing and competing-process preflights passed.

| Run | Result | NUnit XML | SHA-256 |
|---|---:|---|---|
| Focused EditMode `ProfileAtomicSaveServiceV1Tests` | 29 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7d-20260913/focused-editmode-r5.xml` | `0043AB07363460360E2464D9250F06A3F50AA8A95EC04AD589DBC216CA417845` |
| Full EditMode | 584 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7d-20260913/full-editmode-r2.xml` | `0AA3D77392361507B3ABAA49F6741E89A8A6DD0FAB00D2C63B9BAA75E75E4903` |
| Full PlayMode | 576 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7d-20260913/full-playmode-r2.xml` | `8B8098E0E4B183A177DB2DA7CE305F5E77776D65BB54BF46E803C5B10EFDFE84` |

The focused suite exercises real isolated-directory first-save and Windows replacement paths, exact byte preservation, write-through/`Flush(true)` production source constraints, pre/post-commit injected faults, inaccessible-presence classification, unexpected/fatal exception propagation, typed uncertain-state probes, path/result invariants, alias serialization, lease cleanup, and different-directory concurrency. The earlier failed/interrupted attempts and pre-P1 XMLs remain in `artifacts/m5d7d-20260913/` as diagnostic history and are not acceptance evidence.
