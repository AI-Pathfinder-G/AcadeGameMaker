# Costume CUA R14 full PlayMode — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent procedural pre-gate only; no Unity execution
- Contract SHA-256: `B77E9912C02C0AA472FBFD1C4D7B55F7FDC55408CD99E4AE68D865AFC033FA46`
- R13 license-blocker evidence: `docs/verification/2026-09-28-costume-cua-r13-license-blocker.md`

## Checks

1. The final R14 authorization is consistent with the R13 disposition: exactly
   one Unity `6000.6.0f1` full PlayMode run in an unrestricted host command
   environment, with `-runTests -testPlatform PlayMode`, no filter, no assembly
   restriction, and no `-quit`. The permission boundary changes only process
   launch environment; it does not authorize source, package, ProjectSettings,
   test, or product changes. No retry is authorized after R14 failure.
2. The R13 authentication evidence is preserved and correctly treated as an
   environment blocker, not a CUA finding. Its invalid-run log has no result
   XML; no R14 stem exists yet under
   `artifacts/unity-results/costume-cua-20260927/`.
3. No Unity, UnityCrashHandler64, or UnityPackageManager process is present at
   this gate. The pre-existing Licensing Client is not a task-created process
   and remains outside the stop scope.
4. The four required CUA source/test hashes match the R12 execution baseline:
   - media `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
   - adapter `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
   - adapter tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
   - presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`
5. The R14 fresh outputs are exactly
   `costume-cua-r14-full-playmode.xml` and
   `costume-cua-r14-full-playmode.log`; both are absent before launch.
   Existing R7–R13 artifacts and R11/R12 temporary scene evidence remain
   immutable.
6. The immediate pre-R14 porcelain baseline is `495` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `4F2A7CF05FFA50EE5A0A3CF347B525C5FE7ABD2F420E1A08D00F3C06D91DEF7D`.
7. The global watchdog is `6600` seconds. The M5D7M duplicate-test stale-log
   watchdog remains `180` seconds with increasing Unity CPU. Independently,
   repeated license-pipe refusal plus 60-second timeout and zero registered
   packages before test startup is an authentication hard stop once confirmed
   for two cycles; only the exact task-created editor process may then be
   stopped and all evidence must be preserved.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R14 is procedurally ready for the one authorized
full PlayMode execution, provided the attached command is launched through the
reviewed unrestricted host environment and the exact Unity PID is recorded.
This is not a test-result or production-correctness PASS. Any R14 failure,
missing XML after exit, compile failure, nonzero failed/skipped/inconclusive
count, source/hash drift, or unexpected workspace mutation is a hard stop.
