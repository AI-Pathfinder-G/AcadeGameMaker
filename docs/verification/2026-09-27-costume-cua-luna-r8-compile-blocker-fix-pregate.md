# Costume CUA — Luna R8 compile-blocker fix pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Static-only review of the R8 compile failure and the bounded test-source correction. Unity was not run and no implementation file was modified by this review.

## Inputs and hashes

- R8 failure log: `artifacts/unity-results/costume-cua-20260927/costume-cua-r8-focused-playmode.log`
- R8 failure-log SHA-256: `A759B4E6D0C49321F79C5AB40BEE023BC5F753E2E50E9AC4223A38806743C579`
- R7 preserved failure-log SHA-256: `2E48CCACE390FE16B3F327497D6BA1DD6133071E3A04583CD9CEE8AC3E1AE14A`
- Corrected adapter test SHA-256: `8E93A8293ABD0E79913628E74A9FDC54E57D37C20022D053308037136358CF99`
- Media SHA-256 unchanged: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`

## Findings

1. The preserved R8 compiler output contains only the expected unresolved-type errors in the adapter test: `CostumePresentationBindingV1` and `CompletedActorSnapshotV1`. Both are declared in `AcadeGameMaker.Presentation.CostumePresentationV1.cs` under namespace `AcadeGameMaker.Presentation`.
2. The corrected test adds exactly `using AcadeGameMaker.Presentation;`. The source has no competing declarations for either type under the project, so the import resolves the seven reported references without ambiguity.
3. The change is test-source namespace qualification only. It does not alter runtime assemblies, public APIs, adapter behavior, persistence, projection, or mechanics authority.
4. Static inspection confirms the prior R3–R6 coverage remains in the corrected test source: durable first-save/replacement and outcome assertions, projection-fault and uncertain-primary terminal paths, current-tuple corruption matrices, pure frame mapping, and the complete `7 actions × 2 facings × 4 endpoint ages` plus mechanics-isolation tests.
5. R7 and R8 logs are both present with the supplied immutable hashes. R8 is therefore consumed by a compile failure and cannot be reused as a successful or fresh retry result.

## Verdict

**PASS — P0=0, P1=0, P2=0 for this narrow static correction gate.**

The using-only correction is sufficient to address the reported R8 missing-type blocker, subject to a fresh compile/test run. A new R9 execution authorization and fresh stems are required; the existing R8 stem must not be overwritten. Recommended exact R9 stem set is `costume-cua-r9-focused-playmode`, `costume-cua-r9-full-editmode`, and `costume-cua-r9-full-playmode`, each with `.xml` and `.log`, under the existing evidence directory. This review does not authorize those runs.

