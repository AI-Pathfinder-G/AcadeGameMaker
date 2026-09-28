# Costume CUA — Luna execution procedural gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

This is a static, independent gate of Astra's execution-authorization clause. It does not run Unity, alter implementation, or accept any result artifact.

## Inputs

- Contract: `docs/specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md`
- Contract SHA-256: `638B7A124A091359395B1D34AD4D248C6710C92D791B5B3333B9573398D75A86`
- Prior aggregate static gate: `docs/verification/2026-09-27-costume-cua-luna-final-aggregate-static-gate.md`
- Prior aggregate gate SHA-256: `1AE41A6DC119BDFBB027D46EF146FBEEEC47B84D8BC887E4A34E958DD66C18D5`

## Checks

The authorization clause is internally complete and exact:

1. Unity version is `6000.6.0f1`; the three runs are serial and explicitly omit `-quit`.
2. Focused PlayMode is restricted to assembly `AcadeGameMaker.Presentation.Unity.PlayMode.Tests` and filter `AcadeGameMaker.Tests.PlayMode.PresentationUnity`.
3. Full EditMode is unfiltered and has no assembly restriction.
4. Full PlayMode is unfiltered and has no assembly restriction.
5. The fresh output root is exactly `artifacts/unity-results/costume-cua-20260927/`.
6. Each run has an exact immutable stem with both required files: `.xml` results and `.log` editor log:
   - `costume-cua-r7-focused-playmode`
   - `costume-cua-r7-full-editmode`
   - `costume-cua-r7-full-playmode`
7. The clause defines hard stops for a pre-existing target, compile error, missing XML, nonzero failed/skipped/inconclusive count, unexpected project mutation, or source/hash drift.
8. The target directory was absent at this gate (`Test-Path` false), and no matching R7 stem was found under `artifacts/` (0 matches for each stem). Therefore no prior result can be mistaken for a fresh run.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the procedural execution gate.**

The contract now provides sufficient exact authorization for the three serial runs. This PASS is not a test-result PASS and does not permit bypassing any hard stop. Unity execution remains for the authorized executor; this Luna review intentionally did not execute it.

No implementation or production asset was modified.

