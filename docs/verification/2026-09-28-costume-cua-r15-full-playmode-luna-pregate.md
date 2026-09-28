# Costume CUA R15 full PlayMode Luna pre-gate

- Date: 2026-09-28
- Reviewer: Luna (independent QA)
- Scope: procedural pre-gate only; Unity was not executed.

## Contract and preserved evidence

The current CUA contract SHA-256 is
`CDCEA3D6CBFC808BD5EAE6AA2953B10629EDB48A4713E16B3ACEC5D1DE7BD98F`.
Its R15 amendment authorizes exactly one uninstrumented, unfiltered Unity
`6000.6.0f1` PlayMode run, with no `-testFilter`, assembly restriction,
temporary observer, or `-quit`. The former D4 180-second stale-log/CPU stop
is explicitly withdrawn for this run.

R62 evidence is preserved and independently valid: XML SHA-256
`31D3552DF1B972F0F215C9E4120E907DAF3DD757B8FD8B478ABC111A2412E57B`, with
`990/990` passed and zero failed/skipped/inconclusive; the observer source and
meta are absent. The original D4 source SHA is restored to
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.

## Fresh-run and baseline checks

The required R15 stems are absent:

- `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.xml`
- `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.log`

No Unity, UnityCrashHandler64, or UnityPackageManager process is present.
The immediate pre-amendment porcelain is `517` entries with joined SHA-256
`3A49F1EC7D2D2D0C4E3577E97DC9E1E585EC924093763E6A6E461A7321B1AFB4`, matching
the R15 contract baseline. The four CUA execution hashes remain unchanged:

- media `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
- adapter `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- adapter tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
- view-presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`

The expected complete inventory is approximately `1142` (`985+157`) and must
be confirmed from the unfiltered XML. Use only one external monotonic `7200s`
global watchdog. Early stop is permitted only for the specified license/
package-startup two-cycle failure, compile failure, process exit without XML,
nonzero failed/skipped/inconclusive result, source drift, unexpected mutation,
or the global cap; unchanged log/CPU alone is not a stop condition. A global
cap stop must target only the exact task-created Unity process tree, preserve
partial output/scenes, and never retry.

## Verdict

**AC-CUA-R15-001 procedural pre-gate: PASS.** The exact no-filter,
no-observer, no-quit run and 7200-second global-only watchdog are internally
consistent with the approved amendment; fresh stems, process absence, R62
evidence, source hashes, and porcelain baseline pass. License/compile hard
stops are explicit.

**P0: 0; P1: 0; P2: 0.** This authorizes only the bounded R15 procedure. It
does not claim a result, does not satisfy `AC-CUA-R15-002/003`, and leaves
parent `AC-CUA-009` open until the unfiltered XML is independently verified.
