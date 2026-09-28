# VD-09 M5D7I input-binding-override apply adapter — Luna independent post-review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [Approved M5D7I input-binding-override apply adapter](../specs/work-contracts/2026-09-13-vd09-m5d7i-input-binding-override-apply-adapter.md)
- Pre-gate: [Luna R2 pre-gate](2026-09-13-vd09-m5d7i-contract-pregate.md), PASS P0=0/P1=0 with residual P2=3
- Scope: independent source, test, allowlist, evidence, and XML review. No runtime, test, contract, evidence, asset, or README files were modified; no Unity test was run by Luna.

## Verdict

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra accept M5D7I for integration; do not mark the contract Verified here.**

The implementation stays within the approved candidate-only detection boundary. It uses the pinned Input System 1.20.0 APIs, performs the required canonical round-trip, rejects warning-only stale IDs as mismatch, and never references or mutates `InputRouter`. The final focused and full-suite evidence is clean. The residual P2 notes are caller-origin/disposal boundaries and do not block this isolated adapter.

## AC review

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7I-001 | **PASS** | Focused PlayMode `AC001_EmptySentinelRetainsDisabledGeneratedDefaults` uses a real `GameInputActions`; result is `DefaultsReady/None/Retain`, the only port call is preflight `Save`, and the asset remains disabled/empty. |
| AC-M5D7I-002 | **PASS** | `AC002_RealGeneratedOverrideRoundTripsCanonicalMeaning` creates a known current override on one generated wrapper, applies it to a second fresh wrapper, and verifies exact M5D5 canonical text with `OverridesApplied/None/Retain`. |
| AC-M5D7I-003 | **PASS** | `AC003_UnknownBindingIdIsWarningOnlyMismatch` starts from a real Input System binding envelope, replaces only the GUID with a valid unknown ID, expects the package warning, and obtains `RecoveryRequired/RoundTripMismatch/Discard`. |
| AC-M5D7I-004 | **PASS** | The per-instance test port injects all six closed recoverable exception types at Load and post-load Save. The asserted sequences are empty `Save`, Load failure `Save,Load`, and post-load paths `Save,Load,Save`, with no retry and discard after Load. |
| AC-M5D7I-005 | **PASS** | Metadata/default/reflection-invalid snapshots, null/disposed/enabled/already-overridden candidates, null port, and non-empty preflight proof fail before Load. The runtime validates input and candidate before using the port. |
| AC-M5D7I-006 | **PASS** | Unexpected `NotSupportedException` and fatal `OutOfMemoryException` escape from both dependency stages; fresh subsequent calls succeed. The source catch filter excludes all other exceptions, including the named fatal types. |
| AC-M5D7I-007 | **PASS** | The five legal result rows are constructed and validated. Reflection tampering of outcome, failure, disposition, requested/verified proof, and all getter paths fails closed; unavailable verified proof throws only for LoadFailed/RoundTripFailed. |
| AC-M5D7I-008 | **PASS** | Static test/source review confirms one production Load with `true`, one post-load Save, M5D5 ordinal comparison, no direct `RemoveAllBindingOverrides`, no router/file/time/RNG/persistence authority, and only Profile references added to the two asmdefs. The package-internal removal is correctly induced only by `Load(..., true)`. |
| AC-M5D7I-009 | **PASS** | Focused PlayMode is 8/8, full EditMode 647/647, and full PlayMode 584/584; each XML reports zero failed, skipped, and inconclusive tests. |

## Source and boundary findings

### Candidate and lifecycle

`ValidateCandidate` rejects null-like/disposed assets, enabled assets, and any enabled map before the port's preflight Save. A non-empty preflight proof is caller misuse, and the adapter performs no Load. After Load starts, every ordinary recovery result carries `CandidateDisposition=Discard`; the adapter does not dispose or clean up the caller-owned object. The corrected PlayMode fixture disables the intentionally enabled map before disposal, eliminating the earlier generated-wrapper finalizer warning.

The runtime does not authenticate historical asset origin or prove that a deliberately reflection-bypassed caller did not pass a live wrapper. This remains the documented caller-owned “fresh generated candidate” precondition and the residual P2 boundary; adding generated-wrapper identity state or a second binding authority would exceed the contract.

### Exact package behavior and exception boundary

The checked-in package source defines `SaveBindingOverridesAsJson(this IInputActionCollection2)` and `LoadBindingOverridesFromJson(this IInputActionCollection2, string, bool)`, and explicitly supports generated wrappers. With `removeExisting:true`, the package internally invokes `RemoveAllBindingOverrides` before applying entries. The adapter calls neither that cleanup method nor any retry; its BCL port makes exactly the mandated Load call and the permitted Save calls. Unknown IDs only log a warning in the package and are caught by the saved-output comparison.

The runtime's recoverable filter is exactly the approved set (`ArgumentException`, `InvalidOperationException`, `FormatException`, `NullReferenceException`, `IndexOutOfRangeException`, `UnityException`) and is scoped around dependency operations. M5D5 parse/validation exceptions map only to `RoundTripFailed`; adapter validation and all other unexpected/fatal exceptions escape. The test seam is supplied through an internal overload and the injected port is per-call/instance-owned; no test port or candidate is retained by the adapter.

### Result proof and authority

`ProfileBindingOverrideApplyResultV1` is a readonly value with strings only. Every getter invokes `Validate()`. The implementation enforces exact enum ranges, canonical requested proof, the empty default row, equal non-empty applied proof, valid unequal mismatch proof (including empty), and null verified proof only for LoadFailed/RoundTripFailed; unavailable proof's getter throws after validation. No candidate, exception, callback, profile, path, collection, persistence, or recovery-plan data is stored.

The runtime assembly references Profile only in addition to existing Input/Unity dependencies. The test assembly adds only Profile. `GameInputActions.cs`, the authored input asset, InputRouter, package pin, ProjectSettings, and other allowlisted read-only files are unchanged by this unit. No live map is enabled, no callback is registered, and no router action collection is reachable from the adapter source.

## Residual P2 notes

- **P2-001 — caller-origin proof:** structural generated-wrapper/candidate checks cannot authenticate historical construction or distinguish a reflection-forged live wrapper. This is an intentional caller precondition and not an M5D7I authority claim.
- **P2-002 — discard lifecycle:** `Discard` is a typed ownership handoff, not physical poisoning. A caller must dispose every candidate after ordinary failure and after a fatal exception; the adapter correctly avoids cleanup or mutation outside Input System's mandated Load behavior.
- **P2-003 — production/test port distinction:** production uses a stateless singleton BCL port, while deterministic injected ports are instance-scoped through the internal overload. The source has no mutable static test state; future changes must preserve this distinction.

## Independently recomputed hashes

| File | SHA-256 |
|---|---|
| Approved contract | `6d31da4dbb4a04eb521067e179578ddcf6fc6816e5f37319fed4c5a0898dd066` |
| R2 pre-gate | `0fd1b8cb1ce78a07170e301c0fe3210b750f618dbd8937e408772e3bb2d8cca0` |
| Runtime adapter | `92572bb90723c213194d3c9dd221c113bd135fcfd2f5ad091e9b27f4e9a473e1` |
| Runtime adapter meta | `30658173d410337dbd2fe36d1002d7a4418d056d7ae344cb2b0694d1061287f1` |
| Runtime asmdef | `cd3e47dd94bccb9e537ca7af4c6029fc8327ea0154c90ec7b6fd305218fe4c0e` |
| PlayMode tests | `30654c5dd8555babe616ac484b4a3bf84bf34ca7b9ced774e1efdd72967c5cf4` |
| PlayMode test meta | `37e9373654891cf44188b55ac2baf601ad7d78c1d5185769408347c43a9e5203` |
| PlayMode test asmdef | `5f79096ca7294d07c77af7aa8612b439114148c415fe53a8b07dd19207b6595f` |
| Implementation evidence | `af364de867bd9571136576a2e1b8e8cc5dee3674c5b5194fd5b9608ec1f61eaa` |
| Focused PlayMode XML r4 | `d4b5abf53ca9f2e7c31223857ea900fe308331a21b92f8ca2b6f197fdcf2ba1c` |
| Full EditMode XML r2 | `00281b4e3ab0846e2883454ffe12b8990cb60adad95b52a4a9c3a20d0749c8bb` |
| Full PlayMode XML r2 | `e96b442744fed80cd3d8b5289c4ab13d208ea96e0123bdaacb3e1dc6b9c2c063` |

XML parsing independently confirmed: focused `8/8`, full EditMode `647/647`, full PlayMode `584/584`; all have result `Passed`, failed `0`, skipped `0`, and inconclusive `0`.

## Final recommendation

**PASS — P0=0, P1=0, residual P2=3.** Astra may accept M5D7I for integration. This evidence supports only isolated binding-override application detection on a caller-owned candidate; it does not claim launch recovery, persistence, InputRouter mutation, repair planning, notification, or gameplay entry.
