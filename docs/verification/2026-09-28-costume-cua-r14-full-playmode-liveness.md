# CUA R14 full PlayMode liveness result

- Date: 2026-09-28
- Requirement: `REQ-CUA-010` (verification boundary)
- Acceptance criterion: `AC-CUA-009` — **open**
- Executor/monitor: Terra; independent live classification: Luna; stop decision: Astra
- Status: invalid full-suite run, not a test pass or CUA defect finding

R14 was the one authorized host-environment Unity `6000.6.0f1` full PlayMode
run. The editor acquired its license, registered packages, and began tests.
Task-created Unity PID `20968` started at 00:23:25 KST. The preserved partial
log is
`artifacts/unity-results/costume-cua-20260927/costume-cua-r14-full-playmode.log`,
SHA-256 `EB686AA217A17231DF882954B4A3451587A2F4B6A2880E99CEA415C0E1774105`.
No R14 result XML exists.

The log identifies
`DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`
at line `1951` and `InputRouter.cs:231` at line `1942`. Its last write was
00:42:14 KST. During the unchanged-log interval, CPU rose from `1147.20s`
at 00:42:26 to `2778.77s` at 01:09:48. Luna independently recognized the
same test and call site as the R11 full-suite livelock; the R14 contract's
`180` second same-test stale-log plus increasing-CPU stop condition was met.
Astra stopped only PID `20968` at approximately 01:10 KST. A first restricted
stop attempt was denied by the OS; the host-permitted exact-PID stop succeeded.
No retry occurred.

The four CUA source/test hashes remained exactly the R12/R14 contract
baseline. The pre-R14 porcelain status had `495` entries; after the run it
had `498`, with the only additions being Luna's R14 pre-gate document and the
run-generated `Assets/InitTestScene25a382ab-1c88-4067-91f0-88154ab5d434.unity`
plus `.meta`. The scene pair is preserved with prior run evidence. The final
UTF-8 joined porcelain SHA-256 was
`1F166650F92A4A7CF3DAE514840D7906B89BBC8E5B4A786183C77A0D0DA67F50`.

R54 exact-test `1/1`, R55 whole-class `117/117`, and R56 preceding-group
`558/558` runs passed on the same source. R11 and R14 instead stopped during
the same test in unfiltered full-suite order. This is evidence of a
suite-order-dependent test-runner or fixture liveness problem, not proof of a
specific root cause. `AC-CUA-009` still requires a complete full PlayMode XML
with failed/skipped/inconclusive counts all zero. CUA remains `Approved`, not
`Verified`; no real media or wardrobe scene is accepted by this result.
