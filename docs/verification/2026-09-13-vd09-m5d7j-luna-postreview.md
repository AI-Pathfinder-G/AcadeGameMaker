# VD-09 M5D7J binding-apply-failure recovery transform — Luna independent post-review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [Approved M5D7J binding-apply-failure recovery transform](../specs/work-contracts/2026-09-13-vd09-m5d7j-binding-apply-failure-recovery-transform.md)
- Pre-gate: [Luna independent contract pre-gate](2026-09-13-vd09-m5d7j-contract-pregate.md), PASS P0=0/P1=0/residual P2=3
- Scope: independent source, test, authority, allowlist, regression, evidence, and XML review. No runtime, test, contract, evidence, or README files were modified; no Unity test was run by Luna.

## Verdict

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra accept M5D7J for integration; Luna does not mark the contract Verified.**

The implementation adds the two approved public planner entries and two private proof branches while preserving the existing M5D7C plan API and reason numeric values. The new transform accepts only a fully validated current/non-empty binding snapshot, resets only input to the approved current empty default, and advances the source revision exactly once for both primary and previous semantics. The clean focused/regression/full-suite evidence supports the claimed engine-free boundary.

## AC review

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7J-001 | **PASS** | `PlanPrimaryBindingApplyFailure` accepts a revalidated current/non-empty source and outputs current-empty input, exact `InputRepair`, `r→r+1`, preserved settings/tutorial/progression, and defensive snapshot independence. |
| AC-M5D7J-002 | **PASS** | `PlanPreviousBindingApplyFailure` outputs exact `PreviousPromotion|InputRepair` at the same single increment. The test also calls existing `PlanPreviousPromotion` and proves that it still preserves the non-empty override, ruling out accidental replacement/double-increment behavior. |
| AC-M5D7J-003 | **PASS** | New tests cover revisions `0`, `37`, and `long.MaxValue-1` for both methods, reject `long.MaxValue` before output, and prove a following valid call succeeds. |
| AC-M5D7J-004 | **PASS** | Both entries reject current-empty, metadata recovery, binding recovery, invalid, unsupported, default, and reflection-invalid decode values with the specified `ArgumentException`/inner-exception or overflow boundary. |
| AC-M5D7J-005 | **PASS** | New proof kinds, original non-empty current input, canonical binding identity, preserved settings/tutorial/progression, result document, revision, reason, and all public getters are revalidated. Reflection mutations fail through `Validate()` and every getter. |
| AC-M5D7J-006 | **PASS** | Existing reason values remain `1/2/4`; the unchanged M5D7C focused regression is 10/10, and the full EditMode suite is 652/652. Existing planner paths retain their prior proof behavior. |
| AC-M5D7J-007 | **PASS** | Static review finds no Unity, M5D7I, Input System, IO, file/path, Save, clock, RNG, callback, or persistence authority. Runtime scope is the existing Profile planner file; tests are one new EditMode fixture. |
| AC-M5D7J-008 | **PASS** | M5D7J focused EditMode is 5/5, M5D7C regression is 10/10, full EditMode is 652/652, and full PlayMode is 584/584; each XML has zero failed/skipped/inconclusive tests. |

## Source review

### Applicability and transformation

`PlanBindingApplyFailure` first calls `source.Validate()` and wraps malformed/default/reflection-invalid values as `ArgumentException` with the original `InvalidOperationException` inner exception. It then requires exactly `ValidCurrentInput` and rejects an empty override before reading the source revision. Metadata and binding decoder-recovery classifications are not accepted by this new path and remain delegated to M5D7C.

The primary and previous branches share one arithmetic path: source revision is read once, `long.MaxValue` is rejected, `WithRevision` creates exactly `r+1`, and one document is encoded with current empty input. The reason is selected directly (`InputRepair` or `PreviousPromotion|InputRepair`); no existing previous-promotion plan is composed, so no `r+2` can arise.

### Proof and defensive state

The plan now stores `_proofBindingOverride` plus the original `_proofInput` for proof kinds 5 and 6. `Validate()` requires the proof input to be current and non-empty, requires the canonical text identity to match, and requires the result input to be current empty. Settings, tutorial, and progression proofs are validated and compared; progression comparison intentionally excludes only the expected revision change. The existing snapshot constructors/getters clone arrays, so source and result collections are not shared mutable authority.

Every public plan getter calls `Validate()`. The new tests mutate proof kind, reason, source/result revision, proof input, canonical binding identity, preserved settings/tutorial/progression, and result document, then assert `Validate()` and every getter fail closed. Existing default/current-repair/current-promotion branches explicitly pass a null binding proof and reject any injected non-null proof.

### Authority and compatibility

The Profile runtime remains engine-free and contains no Unity, M5D7I, Input System, file/path, save, clock, RNG, callback, or persistence dependency. The two new methods are additive public API; existing `ProfileRecoveryPlanReason` values remain unchanged and existing planner entry behavior is unchanged under the M5D3 focused regression. No asmdef, package, project-setting, input, scene, or prefab changes are in the M5D7J allowlist.

The transform's lack of actual-failure authentication is explicit and correct: a bare decode result cannot prove which selected file or candidate produced M5D7I's failure, so the later launch owner must make that matching assertion. M5D7J does not overclaim this provenance and does not add a second save path. Both output plans can later be passed to ordinary M5D7D save after selector-owned preservation/quarantine preconditions: primary current uses existing-primary replacement; previous current uses the post-selection primary state and ordinary first-save/replacement behavior.

## Residual P2 notes

- **P2-001 — external failure assertion:** source/file/candidate provenance is caller-owned and unauthenticated by this engine-free transform. This is intentionally documented and must remain a launch-coordinator precondition.
- **P2-002 — private proof-kind values:** proof kinds 5 and 6 are private integers rather than persisted/public enum values. This is safe; future tests should verify closed behavior rather than depend on private numeric values.
- **P2-003 — downstream save sequencing:** M5D7J returns only an in-memory plan. The launch owner still must resolve preservation/quarantine and invoke ordinary M5D7D persistence; this review found no need for a special M5D7J save path.

## Independently recomputed hashes

| File/artifact | SHA-256 |
|---|---|
| Approved contract | `b85e4a4b23932d1d346f94363d40eb9008b7fc78ef94c2b9c31917f306895f98` |
| Luna pre-gate | `0d3ea5e0799064c542f796df2311019129c51fb7e59f3daaa3aeecc2853f3ec7` |
| Runtime planner | `e72fe64417385170391e2b7ee6dc7740e448ac1828dc77bff7a603db235f56f8` |
| M5D7J tests | `d7b2bb12686cafc0ac5caed5483b01240c27d309401cd50d49f526b02ea43505` |
| M5D7J test meta | `351e39332fab391efb44a66fd093aac020cb45294d96a951536a3a5e1f456c6c` |
| Implementation evidence | `9309aba3e7bdd76a6f7a53a0d7156d155c867fe7464c461148dbeed335c3525c` |
| M5D7J focused XML | `d98487558b6f6912412e12b0b81c7601b5debbad7c97392fc8c5b734f6dc04bb` |
| M5D7C regression XML | `50a085a180d7efd7a8567ae00e0749be87c7697104596caafa07c654a88dbedf` |
| Full EditMode XML | `98f568a924705be95d8822347f31578e0924fc93bb0f17e907e05da3c0ac91c6` |
| Full PlayMode XML | `380927491dd1460694076ad1b2055637e79c473122b95df67d3f2520e2b6255e` |

The M5D7C regression XML hash above is shown exactly as independently computed in the artifact check; all values match the supplied evidence anchors. XML parsing confirmed:

- M5D7J focused: `5/5`, result Passed, failed/skipped/inconclusive `0/0/0`.
- M5D7C regression: `10/10`, result Passed, failed/skipped/inconclusive `0/0/0`.
- Full EditMode: `652/652`, result Passed, failed/skipped/inconclusive `0/0/0`.
- Full PlayMode: `584/584`, result Passed, failed/skipped/inconclusive `0/0/0`.

## Final recommendation

**PASS — P0=0, P1=0, residual P2=3.** Astra may accept M5D7J for integration. This review accepts only the pure transformation; it does not claim that M5D7I actually failed, that a source was selected, that persistence/quarantine ran, or that launch recovery completed.
