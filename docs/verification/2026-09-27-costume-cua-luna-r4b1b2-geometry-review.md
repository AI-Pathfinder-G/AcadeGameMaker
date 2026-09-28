# Costume CUA R4-B1b2 geometry-boundary narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 23 geometry/constructor/overflow/pivot/baseline/duplicate rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `2341AC1E81D03E6DE2B2A41BDECEC46407704B4C9DC33F192360081263245497`

## Row accounting

The test source enumerates exactly 23 rows (`2341...:19–26`):

- 8 constructor-invalid scalars: negative X/Y, zero/negative width/height, zero/negative duration;
- 13 constructible rejected packages: ordinary X/Y/right/top overrun, both X and both Y maximum-integer overflow-bound cases, pivot X/Y, baseline, adjacent duplicate and non-adjacent duplicate within one clip; and
- 2 accepted packages: exact right/top edge and cross-clip rectangle reuse.

## Rejection and preservation

Constructor rows assert the exact `ArgumentOutOfRangeException` at construction (`:118–132`). The 13 constructible rows create independent clip/package/definition/binding candidates, call `Validate`, and assert the exact category—`Geometry`, `Pivot`, `Baseline`, or `Clips`—through `ExpectedB1b2Validation` (`:134–159`, `:480–485`). Each then calls real `adapter.TrySelect(candidate)` and applies the B1a preservation oracle: old tuple/media/binding, full view/highlight/current/status, save/replace/projection counts unchanged.

The four maximum-integer rows pass actual `CostumeUnityFrameV1` instances through package `Validate`; they are not merely constructed and inspected. The ordinary overrun and exact-edge rows use a 4×2 atlas with the expected one-pixel boundary values.

## Positive boundary behavior

`exactRightTopEdge` and `crossClipRectangleReuse` both construct valid packages and call `Validate == None` followed by actual `adapter.TrySelect(candidate) == None` (`:161–184`). The test asserts a new published tuple, the candidate media reference, replacement `MoveCalls == 2`, `ReplaceCalls == 1`, projection count `2`, and `Ready` status. The cross-clip case reuses the same rectangle in `idle/left` and `locomotion/left` while retaining distinct in-clip rectangles, so the positive rule is tested rather than inferred from within-clip rejection.

## Decision

**CLOSED for R4-B1b2.** All 23 rows, exact rejection categories, B1a negative preservation, exact-edge acceptance and cross-clip positive replacement are statically covered. This narrow result does not close other R4-B categories or authorize Unity execution by itself.

