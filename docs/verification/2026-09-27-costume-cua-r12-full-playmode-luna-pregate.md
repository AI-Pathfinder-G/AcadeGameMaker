# Costume CUA R12 full-PlayMode liveness retry — Luna pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Independent static procedural pre-gate only. Unity was not run; source, tests, scenes, artifacts, and other implementation files were not modified.

## Contract and evidence

- Contract SHA-256: `06463AB8E3507EC84CC5EE850B3BD671439D8E67ED2FC5DEE58A97EB93ADB0C2`
- R11 focused: 195/195, XML `0485ADB16440963B553A6C4D48191D47F1C4C736624B3402D6D9632414740F19`, log `36B510E577231DC3E8E757063BB3C6FC96291F090F7334B66FB271990FE4E91C`
- R11 full EditMode: 800/800, XML `D4E18A69084A2E0140E19D503C734AC7DD68C30AE1FF723D5BD13CC7E3A38C7C`, log `86251F0306CC575C86F617E9313CE0740FDB3507290863F4014E0EB990FD2DF0`
- R11 full PlayMode partial log: `397B445907A90CB78F32F3DC66DDC6F1B9A3EAF7C4FD7DC26690BDFDDBC4F235`; no XML exists
- R54: 1/1, XML `D76894EADADE98329843DC093A1D5C5E74F165FB56E4EF283D3A160F0435A284`, log `34AE43B68B5363589DD6288DCE3FA224352EF7A6AC4D22421E382CF146DAF2C1`
- R55: 117/117 in 982.0166498s, XML `9B664706F9CC0814C34169673EF6751747F300FC4DD0BC6FD1283944F84E2EB8`, log `0D1551BE80E49CCB2FC5AD2AF57F482CFF3106BFEF03331AE1DC2611623FAB2B`
- R56: 558/558 in 936.1186442s, XML `68FD2FF1C697FA2B0DA58BC515CE6B165FDFAA3FF53F601C18C39A7E292E4DA7`, log `F86781E60227487BBC6011A323EA5BEB7577B71A98BC8047F888185DA55A882A`

## Checks

1. Current production/test hashes match the contract exactly: Media `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`, Adapter `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`, Adapter tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, ViewPresenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
2. R12 authorizes exactly one Unity `6000.6.0f1` full PlayMode run with `-runTests -testPlatform PlayMode`, no filter, no assembly restriction, and no `-quit`. Its only fresh outputs are `costume-cua-r12-full-playmode.xml` and `.log` under `artifacts/unity-results/costume-cua-20260927/`; both are currently absent.
3. The prior `InitTestScene7441...unity` and `.meta` baseline remains present and is not silently removable. Earlier artifacts are preserved; the R12 paths introduce no overwrite ambiguity.
4. The 6,600-second global watchdog is explicit. A log stale for at least 180 seconds at the same M5D7M duplicate-adapter test while Unity still consumes CPU is classified as the same runner livelock; only the exact task-created process tree may be stopped, evidence must be preserved, and no retry is allowed.
5. Compile error, missing XML after normal exit, any nonzero failed/skipped/inconclusive count, source/test hash drift, or unexpected workspace mutation is a hard stop. The contract records the pre-run porcelain baseline and joined-status digest for drift comparison.
6. The contract explicitly classifies R11 as a transient runner liveness failure after R54–R56 isolated passes and authorizes no source/test correction or implementation behavior change.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the R12 procedural pre-gate.**

R12 is procedurally ready for the single authorized full-PlayMode run, subject to the exact hard stops and baseline preservation. This is not an execution or production-correctness PASS.

