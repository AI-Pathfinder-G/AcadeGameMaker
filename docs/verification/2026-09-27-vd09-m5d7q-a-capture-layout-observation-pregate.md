# VD09 M5D7Q-A layout-observation marker — Luna static pre-gate

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Diagnostic-only review. Unity was not run and no implementation behavior was changed.

## Exact input

`Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`

SHA-256: `1B775BF2FB4011FEB2A57FEBAB3FF0F81D40C8A26BBD3AFCD597A0E5EF72F67F`

## Findings

- `M5D7QA_LAYOUT_OBSERVATION` is emitted by `LogLayoutObservation` at the first line of `AssertLayout`, immediately before the existing actual-scene layout predicate. It performs no assignment, render call, output write, publish, rollback, or state restoration.
- The marker records the already-applied Oracle dimensions/scale/safe rect plus Canvas/root/SafeFrame/backdrop observations. The existing `Require(...)` predicate remains unchanged after the log call; `RenderPass`, PNG/manifest generation, same-process comparison, publish, and final restoration paths are unchanged.
- Float serialization uses invariant round-trip `"R"` formatting through `N`/`Pair`/`Triple`/`Box`; integer fields use invariant formatting. Boolean fields use the deterministic lowercase `true`/`false` helper. The marker contains no `GetInstanceID`/instance-ID field or other process-local identity.
- The same assertion path is used for the normal and repeat passes; the marker is observational in both and does not alter determinism or output bytes.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The amendment is diagnostic-only and preserves predicate, rendering, output, rollback, and restoration semantics.

