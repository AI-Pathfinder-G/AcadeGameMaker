# Costume CUA — Luna R9 helper compile-fix pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Static-only review of the R9 test-source helper correction. Unity was not run and implementation files were not modified.

## Inputs and hashes

- R9 failure log: `artifacts/unity-results/costume-cua-20260927/costume-cua-r9-focused-playmode.log`
- R9 failure-log SHA-256: `E44FA7EBB1E4F2F1D807FA8F5418D23622C6D130388AE81D99669574BB0D061C`
- Corrected adapter-test SHA-256: `F60EDF4B483F0C108FAE7B11FAF3A8FAC87CFCE581974D3ED5C425E9C5964887`
- R7/R8 failure logs remain preserved with their previously recorded hashes.

## Findings

1. The preserved R9 compiler output identifies exactly three missing test helpers/usages: `SnapshotEqual`, `FrameEqual`, and `ActionIds(media)`. The corrected test adds them as private test-only helpers; no production source or public surface changes.
2. `SnapshotEqual` compares exactly the seven `CompletedActorSnapshotV1` values: `Tick`, `ActorId`, `ActionId`, `ActionAgeQ1000`, `Facing`, `AnchorX`, and `AnchorY`.
3. `FrameEqual` compares exactly the eight `CostumeUnityFrameV1` values: `X`, `Y`, `Width`, `Height`, `PivotXQ1000`, `PivotYQ1000`, `BaselineY`, and `DurationTicks`.
4. `ActionIds(CostumeUnityMediaPackageV1 media)` enumerates `media.Clips`, inserts action IDs into `SortedSet<string>(StringComparer.Ordinal)`, and copies the unique ordinal-sorted set to the result. This is the correct media-derived binding list and does not change the existing parameterless `ActionIds()` canonical fixture list, which remains unchanged.
5. C# 9/Unity compilation is plausible: all helpers are private static methods in the containing test class; `IReadOnlyList` is enumerable, `SortedSet<T>.CopyTo` is available, and required `System`/generic imports already exist.
6. The corrected source still contains the prior R3–R6 coverage, including durable replacement/outcome, projection-fault and uncertain-primary terminal behavior, current-tuple corruption, mechanics-isolation, and the complete action/facing/endpoint matrix. No semantic gap or production regression is visible from this test-only correction.

## Verdict

**PASS — P0=0, P1=0, P2=0 for this narrow static correction gate.**

The helper correction may proceed to a fresh compile/test attempt. The R9 stem was consumed by the compile failure and must not be overwritten; a new R10 authorization and fresh focused/full stems are required. This review does not authorize R10 execution.

