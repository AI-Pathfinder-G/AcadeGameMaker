# Costume CUA R4-B1b1 media-boundary narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 12 payload/reference/import/PPU rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `DEDA6DC48B04CBE72C308BFC0E6D1CB098F5D37B740C8313831C5C11CC45A7D0`

## Rows and named boundaries

`R4B1b1Mutations` enumerates exactly 12 rows (`DEDA...:18–22`): portrait bytes, atlas bytes, clip-map bytes, null portrait, null atlas, destroyed portrait/atlas before validation, destroyed portrait/atlas before adapter selection, portrait filter drift, atlas filter drift, and PPU mismatch.

The test constructs a fresh candidate package for every row (`:53–83`). Byte mutations change one source byte; null mutations pass a null Unity reference; destroyed mutations destroy the exact candidate reference at the named pre-validate or pre-`TrySelect` boundary; filter mutations change Point to Bilinear; PPU changes the package/profile and matching fixture definition. Constructor admission is not counted as validation evidence.

Expected validation categories are exact and match the implementation boundary (`:369–375`): payload bytes → `Hash`, filter drift → `Filter`, PPU → `Ppu`, and null/destroyed references → `Reference`.

## Adapter and preservation proof

For all constructible rows, the test creates a fresh published adapter, sets a non-empty highlight, captures the complete `DeepViewSnapshot`, old `Published`/media/binding values and move/replace/projection/status counters, calls real `TrySelect(candidate)`, and requires adapter `Package` rejection (`:88–102`). `AssertB1aPublishedPairPreserved` verifies old tuple identity, old media reference, old binding value, all counters, status and the full view including transient highlight/current/rows (`:405–415`, `:468–472`).

The two `destroyed...BeforeValidate` rows intentionally stop at the pre-validation boundary after asserting the exact `Reference` result (`:84–86`); they do not claim an adapter operation occurred. The two `destroyed...BeforeTrySelect` rows proceed through `TrySelect` and use the same preservation oracle. This is the correct distinction between validation-before-use and adapter-boundary tests.

## Decision

**CLOSED for R4-B1b1.** All 12 rows are real, independently mutated, category-checked at the named boundary, and—where an adapter call is admissible—covered by the B1a no-save/no-projection/full-view/old-pair preservation oracle. This narrow result does not close other R4-B categories or authorize Unity execution by itself.

