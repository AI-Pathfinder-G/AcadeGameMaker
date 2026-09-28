# M5D7M R60 handoff-only separation — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent diagnostic pre-gate only; no Unity execution
- Approved contract SHA-256: `FA0493BFF09C97EC7FE388EDF08D5E74E51D848372F6BBF4906E3E7B8F265F68`

## Checks

1. R59 evidence is preserved: partial log SHA-256
   `EC3E3DEEAAEA344B8EA7CFD393F2FEF6D2789DDC2CA1399AFDEA7DE714485923`, no
   XML, D4/InputRouter liveness timeout, and preserved temporary scene pair.
   R59 is a selection-arm diagnostic only; `AC-CUA-009` remains open.
2. R60 is Approved for exactly one Unity `6000.6.0f1` host-environment
   PlayMode diagnostic without `-quit`, using the R58 selection plus only
   `HubUiOnlyQ0HandoffMatrixPlayModeTests`. The
   `HubUiOnlyQ0RemainingRuntimePlayModeTests` arm is explicitly omitted.
   Expected selection is `990` tests (`985+5`); the filter is a selection set
   and does not guarantee fixture order.
3. Fresh outputs
   `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r60-handoff-only.xml`
   and `.log` are absent. No Unity, crash-handler, or package-manager process
   is present at the gate. No version probe is authorized.
4. The four CUA source/test hashes remain unchanged: media
   `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`,
   adapter
   `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`,
   adapter tests
   `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, and
   presenter tests
   `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
5. The pre-amendment porcelain baseline is `507` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `2A78E5F2E7DC823CDFAFA3F042EDBBC47EC370F70E7DB29843DF6F729AC61D31`.
   Existing dirty state is preserved and excluded from R60 output scope.
6. The global watchdog is `1900` seconds. The exact-D4 unchanged-log plus
   increasing-CPU watchdog remains `180` seconds. On trigger, only the exact
   task-created Unity process tree may be stopped; no retry or version probe is
   permitted. Partial logs and generated scene pairs must be preserved.
   Compile/authentication failure, missing XML after natural exit, nonzero
   failed/skipped/inconclusive counts, source drift, or unexpected workspace
   mutation is a hard stop.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R60 is procedurally ready for the one approved
handoff-matrix-only diagnostic. A timeout supports this selection arm only; a
pass rejects this arm as sufficient. Neither outcome closes `AC-CUA-009`,
replaces the unfiltered full PlayMode XML, or authorizes a code/test fix.
