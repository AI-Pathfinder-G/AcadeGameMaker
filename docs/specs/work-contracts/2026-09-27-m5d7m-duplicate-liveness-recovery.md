---
status: Approved
---

# M5D7M duplicate-adapter liveness recovery

- Date: 2026-09-27
- Status: Approved for bounded diagnostics; R61/R62 permit only their named
  temporary test instrumentation; runtime implementation remains blocked
- Owner / final authority: Astra
- Architecture counter-review: Sol
- Diagnostic executor: Terra
- Independent reviewer: Luna
- Parent: `2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md`
- Requirements: `REQ-M5D7M-005/006/008`, `AC-M5D7M-005/006/009`

## Trigger and classification

The CUA R11 full PlayMode run produced no XML and consumed one CPU core for
more than 45 minutes after its log stopped at
`DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`.
The partial log is immutable at
`artifacts/unity-results/costume-cua-20260927/costume-cua-r11-full-playmode.log`,
SHA-256
`397B445907A90CB78F32F3DC66DDC6F1B9A3EAF7C4FD7DC26690BDFDDBC4F235`.
No result XML exists. The exact task-created Unity PIDs were stopped only after
livelock classification. The generated `InitTestScene7441...` scene and meta
remain preserved as drift evidence.

The last logged loser reservation exception is expected behavior and is not
evidence of an `InputRouter` loop. Sol's bounded static review identifies the
following winner `Start` receipt/notification validation as the primary
suspect, but this is not yet a production-correctness finding.

## Authorized diagnostic

Run exactly once on Unity 6000.6.0f1, in batch mode and without `-quit`:

- platform: `PlayMode`;
- test filter:
  `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`;
- result XML:
  `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r54-duplicate-liveness.xml`;
- editor log:
  `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r54-duplicate-liveness.log`.

An external monotonic watchdog is mandatory. If a complete XML has not
appeared within 180 seconds after the Unity process starts, record exact PID,
CPU, log length/last-write/tail and then stop only the exact task-created Unity
processes. Do not retry. Preserve the partial log and any generated test scene.
If XML appears, wait for natural exit and record counts, duration and hashes.

## Current allowlist and stop conditions

This first run authorizes no source, test, scene, package, setting, or existing
evidence modification. Only this contract, one independent pre-gate record,
the fresh diagnostic directory/artifacts and one diagnostic evidence record
may be added. Existing R7–R11 and M5D7M evidence is immutable.

Stop after the single run. If it exceeds 180 seconds, a later amendment may
authorize test-only stage markers in
`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs`.
Changing the duplicate scenario, runtime adapter/coordinator, `InputRouter`,
or any assertion is forbidden until the observed stage is independently
reviewed and Astra approves an exact amended allowlist.

## Diagnostic acceptance criteria

- **AC-M5D7M-R1-001:** the exact single filtered test either produces a complete
  XML within 180 seconds or yields a preserved stage-adjacent timeout record;
  no indefinite process remains.
- **AC-M5D7M-R1-002:** no process other than the exact diagnostic Unity process
  is stopped, and no existing artifact is overwritten.
- **AC-M5D7M-R1-003:** source hashes and worktree drift are compared before and
  after; generated test artifacts are named rather than silently removed.
- **AC-M5D7M-R1-004:** the result determines the next gate: isolated pass means
  suite-interaction investigation; isolated timeout means stage-marker
  amendment. It does not itself authorize a behavioral fix.

## Astra approval

Astra approves only the one bounded diagnostic above. No product decision is
required and no runtime or test implementation change is approved.

## Astra R55 class-isolation authorization — 2026-09-27

R54 passed the exact duplicate test naturally in `22.2733782` seconds:
`m5d7m-r54-duplicate-liveness.xml`, SHA-256
`D76894EADADE98329843DC093A1D5C5E74F165FB56E4EF283D3A160F0435A284`,
and `.log`, SHA-256
`34AE43B68B5363589DD6288DCE3FA224352EF7A6AC4D22421E382CF146DAF2C1`.
This rules out an unconditional loop and selects the contract's
suite-interaction branch.

Astra authorizes one next diagnostic run, still with no source changes,
retries or `-quit`: PlayMode filter
`AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests`
on Unity 6000.6.0f1, writing fresh
`m5d7m-r55-adapter-class.xml/.log` under the same diagnostic directory.
Use a 1,200-second external monotonic watchdog. Historical accepted class
runs completed in about 946–985 seconds; therefore this bound permits the
known workload while detecting the R11 liveness failure. Apply the same exact
PID-only stop, artifact preservation, drift accounting and no-retry rules as
R54. A pass selects contamination outside this class; a timeout selects a
test-only stage-marker amendment. Neither result authorizes implementation.

## Astra R56 pre-class interaction authorization — 2026-09-27

R55 passed all `117` tests in the target class in `982.0166498` seconds:
`m5d7m-r55-adapter-class.xml`, SHA-256
`9B664706F9CC0814C34169673EF6751747F300FC4DD0BC6FD1283944F84E2EB8`,
and `.log`, SHA-256
`0D1551BE80E49CCB2FC5AD2AF57F482CFF3106BFEF03331AE1DC2611623FAB2B`.
This selects contamination outside the class.

The R11 log reached the target class immediately after Camera, Combat and
pure Input PlayMode tests. Astra authorizes one interaction-preserving run
using these semicolon-separated regex group filters, in this single orderless
Unity test-runner selection:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests`

Run once in PlayMode on Unity 6000.6.0f1, without `-quit`, and write fresh
`m5d7m-r56-preclass-interaction.xml/.log` under the same diagnostic directory.
Use a 1,500-second watchdog: historical component durations total less than
1,000 seconds, leaving a substantial diagnostic margin without permitting the
45-minute R11 livelock. Apply the same exact PID-only stop, preservation,
drift and no-retry rules. Source changes remain forbidden. A timeout selects
cross-assembly state contamination; a pass rejects this specific prefix and
requires a later broader-order diagnostic. Neither result authorizes a fix.

## Astra R57 HubPresentation interaction authorization — 2026-09-28

R56 passed `558/558` in `936.1186442s` with failed/skipped/inconclusive all
zero. Its XML SHA-256 is
`68FD2FF1C697FA2B0DA58BC515CE6B165FDFAA3FF53F601C18C39A7E292E4DA7`.
The later R14 unfiltered full PlayMode run independently reproduced the R11
liveness condition at the same D4 duplicate-adapter test and
`InputRouter.cs:231`, with stale log and increasing CPU. Its preserved log
SHA-256 is
`EB686AA217A17231DF882954B4A3451587A2F4B6A2880E99CEA415C0E1774105`;
no XML exists. The outcome and exact-PID stop are documented in
`docs/verification/2026-09-28-costume-cua-r14-full-playmode-liveness.md`,
SHA-256 `70E869F5A8150FC38BF6BFB23C87E60D3086BDE9239643ECBA0980FD01B36CD6`.
No production or test source changed.

In the previously successful full-suite fixture order, the target M5D7M
class follows Camera, Combat, three `HubPresentation` fixtures (four tests),
and pure Input. R56 selected every one of those groups except
`HubPresentation`. Sol's bounded counter-review identifies adding that
single group as the smallest available interaction test. The test filter is
an orderless selection set; it does not command Unity to run fixtures in a
specified order. Therefore the result may support only a group-level
interaction hypothesis, not blame a specific Hub test or production owner.

Subject to Luna's independent procedural pre-gate, Astra authorizes exactly
one diagnostic Unity `6000.6.0f1` PlayMode run in the host environment, with
no `-quit`, using this semicolon-separated regex filter:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests`

Write fresh immutable outputs only to
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r57-hub-interaction.xml`
and `.log`; both must be absent at pre-gate. Record exact task-created PID,
CPU, log length/timestamp, XML counts/duration and artifact hashes. Use an
external `1500` second monotonic watchdog. If the log is unchanged at the D4
duplicate-adapter test for at least `180` seconds while CPU increases, stop
only the exact task-created Unity process tree and preserve the partial log.
No retry is authorized. An authentication/compile error, missing XML after
natural exit, nonzero failed/skipped/inconclusive, source hash drift, or
unexpected workspace mutation is a hard stop. Run-generated temporary test
scene pairs must be identified and preserved, never silently removed.

This diagnostic changes no source, tests, packages, ProjectSettings, scenes,
or prior evidence. The pre-R57 porcelain status contains `499` entries with
UTF-8 joined SHA-256
`1B5A62B0874465B786FAF32F7FCDBD45D264C0E2D990CFCF80106AFFED45ACC4`.
The CUA source/test hashes are the exact four listed in its R12/R14
authorization. A complete pass would reject this one added-group hypothesis
and require broader order/lifecycle diagnostics; the same D4 timeout would
support a `HubPresentation` group interaction candidate. Neither outcome
authorizes a code fix or closes `AC-CUA-009`.

## Astra R58 low-cost suffix selection authorization — 2026-09-28

R57 completed naturally: `562/562` passed, failed/skipped/inconclusive
`0/0/0`, duration `941.6734618s`. Its XML SHA-256 is
`440F7D7A36C10F78D9FFA5D8878DF7E016361FD01B2C733ED9564AE89AD27A8B`
and log SHA-256 is
`6B9C025ED81628C429BA90345F553CBBBE8A3B3D94C1BBAB02E165FE2911D8A9`.
The diagnostic evidence is
`docs/verification/2026-09-28-m5d7m-r57-hub-interaction-evidence.md`,
SHA-256 `DA97F6750461F2F0BAB66FC7BD7686EAE838BDF1EFBEF217D6D6E4A7CBCFFB5A`.
R57's first 21 fixture suites matched the prior successful full PlayMode
prefix exactly, including the target D4 test. Adding `HubPresentation`
therefore did not reproduce the full-suite liveness failure.

The remaining prior full-suite suffix contains five costly
`AcadeGameMaker.Input.Unity.PlayMode.Tests` sibling fixtures (indices
21–25, 157 tests, summed historical duration `4096.504097s`) and 19
low-cost `InputUnity`, `Movement`, and `TransferUnity` fixtures (indices
26–44, 228 tests, summed historical duration approximately `5.60s`). The
current CUA `PresentationUnity` suite adds 195 tests whose focused run took
approximately `0.324s`. Sol counter-reviewed a cost-aware selection:
include every low-cost suffix group and CUA alongside the R57 prefix,
leaving only the five costly siblings unselected. This tests the selection
set and runner/discovery lifecycle, not causal execution order. A filtered
pass may narrow the remaining selection-set difference only after the
result XML confirms the intended fixture/test inventory.

Subject to Luna's independent procedural pre-gate, Astra authorizes exactly
one host-environment Unity `6000.6.0f1` PlayMode diagnostic with no `-quit`
and this semicolon-separated regex test filter:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests;^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Movement\.;^AcadeGameMaker\.Tests\.PlayMode\.TransferUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.PresentationUnity\.`

Use only fresh
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r58-cheap-suffix.xml`
and `.log`, absent at pre-gate. The expected selection is approximately
`985` tests (`562+228+195`); exact fixture/class names and counts must be
checked from the result XML before interpreting any pass. Use a `1500` second
external monotonic watchdog, allowing more than nine minutes above the
observed `947.6s` normal-cost estimate. The exact-D4 unchanged-log `180`
second plus increasing-CPU watchdog remains active. Stop only the exact
task-created Unity process tree on a watchdog trigger. No retry is allowed.
Preserve any partial log and run-generated temporary scene pair. Compile or
authentication failure, missing XML after normal exit, nonzero
failed/skipped/inconclusive, source hash drift, or unexpected workspace
mutation is a hard stop.

R58 changes no source/test/package/ProjectSettings/scene or existing
evidence. The pre-R58 porcelain status has `501` entries with UTF-8 joined
SHA-256
`BB400A740F2B5FFAF00FFA242A837F4F3E2BA71FF4D4E711968D7D3B4F3C1269`.
The four CUA source/test hashes in its R12/R14 authorization remain the
execution baseline. An R58 pass is diagnostic only and does not replace the
unfiltered full PlayMode XML required by `AC-CUA-009`.

## Astra R59 two-cheap-sibling selection authorization — 2026-09-28

R58 completed naturally: `985/985` passed, failed/skipped/inconclusive
`0/0/0`, duration `944.3292552s`. XML SHA-256 is
`5E9047F3AB928BB2EEF1B309F24AE9CC4D0D33CDC5FB77D3022DB8205D2FC889`;
log SHA-256 is
`B2B12FD6C41F303196FE4663586783767C5777813419DD20B0D1D4068F4EE81E`.
The evidence is `docs/verification/2026-09-28-m5d7m-r58-cheap-suffix-evidence.md`
(SHA-256 `806E9D0F99DDDDF2866F92487A25ECF52B0C389707D7595189FE224259E3429E`).
Luna independently confirmed XML inventory, hashes, D4 natural completion,
and open `AC-CUA-009`.

Five costly `AcadeGameMaker.Input.Unity.PlayMode.Tests` sibling fixtures
remain outside R58. Sol's bounded cost review recommends the two shortest:
`HubUiOnlyQ0HandoffMatrixPlayModeTests` (historical 5 tests, `340.19s`) and
`HubUiOnlyQ0RemainingRuntimePlayModeTests` (18 tests, `374.69s`). This is a
selection-set diagnostic, not an execution-order command or causal
attribution. Expected selection is `1008` tests (`985+5+18`), subject to
exact XML inventory confirmation. Observed-duration estimate is about
`1659s` before startup and variance.

Subject to Luna's independent procedural pre-gate, Astra authorizes exactly
one host-environment Unity `6000.6.0f1` PlayMode diagnostic with no `-quit`,
using the R58 semicolon-separated filter plus these two exact fixture arms:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests;^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Movement\.;^AcadeGameMaker\.Tests\.PlayMode\.TransferUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.PresentationUnity\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.HubUiOnlyQ0HandoffMatrixPlayModeTests;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.HubUiOnlyQ0RemainingRuntimePlayModeTests`

Write only fresh, absent
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r59-two-cheap-siblings.xml`
and `.log`. Use an external `2200` second monotonic global watchdog and
retain the separate exact-D4 unchanged-log `180` second plus increasing-CPU
watchdog. On trigger, stop only the exact task-created Unity process tree.
No retry is authorized. Record task-created PID, CPU/log observations, XML
counts, fixture inventory, duration, hashes, and workspace drift. Preserve
partial logs and any run-generated temporary test-scene pair. Compile or
authentication failure, missing XML after natural exit, nonzero
failed/skipped/inconclusive, source hash drift, or unexpected workspace
mutation is a hard stop.

R59 changes no source, test, package, ProjectSettings, scene, or previous
evidence. The pre-amendment porcelain status had `503` entries with UTF-8
joined SHA-256
`E87E864BE80DC8A2297C164BAA390CC57AE14B6123884052F44C6378522A5CDC`.
The four CUA source/test hashes in the R12/R14 authorization remain the
execution baseline. A D4 liveness failure implicates this two-fixture
*selection arm* only and requires narrower diagnosis before any fix; a pass
means only that this arm is insufficient to reproduce the failure, not that
the other three are individually causal. Neither outcome closes `AC-CUA-009`
or replaces its unfiltered full PlayMode XML requirement.

## Astra R60 handoff-matrix-only separation authorization — 2026-09-28

R59 reproduced the D4 liveness timeout with the R58 selection plus two
additional Input.Unity sibling fixtures. The partial log SHA-256 is
`A5583325AB96178D92F7FB4F5413F3644802856E78EAD93CEA87814F596567CF`;
no XML was produced. The exact D4 log was unchanged for more than `207s`
while main Unity CPU rose, and the task-created process tree alone was
stopped. The preserved evidence is
`docs/verification/2026-09-28-m5d7m-r59-two-cheap-siblings-evidence.md`
(SHA-256 `EC3E3DEEAAEA344B8EA7CFD393F2FEF6D2789DDC2CA1399AFDEA7DE714485923`).
Luna independently confirmed the D4/InputRouter evidence, no XML, source
baseline, and preserved temporary scene pair. R59 identifies only a
two-fixture *selection-arm* interaction, not a specific culprit.

The cheaper one-fixture separation is to add only
`AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0HandoffMatrixPlayModeTests`
to the R58 selection. Its historical five tests took `340.19s`, giving an
observed-duration estimate near `1285s` plus startup/variance. Expected
selection is `990` tests (`985+5`), subject to exact XML inventory. The
filter is a selection set; it cannot guarantee fixture execution order.

Subject to Luna's independent procedural pre-gate, Astra authorizes exactly
one host-environment Unity `6000.6.0f1` PlayMode diagnostic with no `-quit`,
using this semicolon-separated regex filter:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests;^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Movement\.;^AcadeGameMaker\.Tests\.PlayMode\.TransferUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.PresentationUnity\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.HubUiOnlyQ0HandoffMatrixPlayModeTests`

Use only fresh, absent
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r60-handoff-only.xml`
and `.log`. An external `1900` second monotonic global watchdog and the
separate exact-D4 unchanged-log `180` second plus increasing-CPU watchdog
apply. Stop only the exact task-created Unity process tree on trigger; no
retry and no additional Unity version probe. Record PID, CPU/log observations,
XML counts/inventory/duration if complete, hashes, workspace drift, and
run-generated scene pairs. Preserve all partial output and scenes. Compile
or authentication failure, missing XML after natural exit, nonzero
failed/skipped/inconclusive, source hash drift, or unexpected workspace
mutation is a hard stop.

The pre-amendment porcelain had `507` entries with UTF-8 joined SHA-256
`2A78E5F2E7DC823CDFAFA3F042EDBBC47EC370F70E7DB29843DF6F729AC61D31`.
R60 changes no source/test/package/ProjectSettings/scene or prior evidence;
the four CUA hashes in the R12/R14 authorization remain baseline. A D4
timeout would support the handoff-matrix *selection arm* as sufficient in
this environment, not prove the fixture or production code is defective. A
pass would mean this one arm is insufficient, leaving the remaining-runtime
arm or combination to investigate. No result here closes `AC-CUA-009` or
replaces its unfiltered full PlayMode XML requirement.

## Astra R61 D4 test-only stage-marker authorization — 2026-09-28

R60 selected the R58 set plus only
`HubUiOnlyQ0HandoffMatrixPlayModeTests` and reproduced the exact D4 liveness
timeout. Its partial log SHA-256 is
`C8EDCDBC87675B56FD7B5EFCF3DFDBB6290AC39E044A28A26503A52824EC5DB6`;
no XML exists. The log was unchanged for approximately `201s` at D4 while
main CPU rose; only the exact task-created Unity process tree was stopped.
The preserved evidence is
`docs/verification/2026-09-28-m5d7m-r60-handoff-only-evidence.md`, SHA-256
`626718C48B66E6632EEBAEC57DF9FC2BBC2BA9FB9129E59BC1EB91A5666BC7CF`.
Luna independently confirmed the partial log, preserved scene pair, source
baseline, and open `AC-CUA-009`. The R58 selection passed; therefore
HandoffMatrix membership is sufficient to reproduce the observed timeout
*in this selection environment*, but this does not identify a defective
fixture or runtime component.

Sol's bounded review found that HandoffMatrix's eager `TestCaseSource`
enumerates five scenario DTOs without creating Unity objects, InputSystem
subscriptions, directories, or shared owner state; its test body and cleanup
cannot run before the earlier D4 stall. The cheapest next discriminator is
stage localization inside D4, not another uninstrumented partition.

**REQ-M5D7M-R61-001:** Terra may add only deterministic `Debug.Log` stage
markers prefixed `M5D7M-R61` in the single D4 test method and its
`CreateDuplicateAdapter` helper in
`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs`.
Markers must bracket `LogAssert.Expect`, duplicate-context creation,
`parent.SetActive(true)` including its caught exception path, winner `Start`,
receipt/assertion phase, and `finally` disposal. Do not change assertions,
control flow, catches, scenario data, or runtime code. The pre-change test
source SHA-256 is
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
Luna must independently review the exact diff before execution. Markers are
temporary; they must be removed with an exact patch after evidence is
captured, restoring this source hash, and Luna must verify restoration.

**REQ-M5D7M-R61-002:** After Luna's implementation pre-gate, run exactly
one host-environment Unity `6000.6.0f1` PlayMode diagnostic with no `-quit`
and the exact R60 filter:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests;^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Movement\.;^AcadeGameMaker\.Tests\.PlayMode\.TransferUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.PresentationUnity\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.HubUiOnlyQ0HandoffMatrixPlayModeTests`

Use only fresh, absent
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r61-d4-stage-markers.xml`
and `.log`. Expected inventory is `990` tests if complete. Use a `1900`
second external monotonic global watchdog and the separate exact-D4
unchanged-log `180` second plus increasing-CPU watchdog. The latter must
assess the last *stage marker or D4 exception* rather than requiring the
unmodified D4 log line. Stop only the exact task-created process tree on a
watchdog trigger. No retry or version probe. Record stage sequence, PID,
CPU/log observations, XML counts/inventory/duration if complete, hashes,
workspace drift, and run-generated scene pairs. Preserve partial output and
scenes; no existing artifacts may be overwritten. Compile/auth failure,
missing XML after natural exit, nonzero failed/skipped/inconclusive, source
drift beyond the approved marker diff, or unexpected workspace mutation is
a hard stop.

**AC-M5D7M-R61-001:** Luna confirms the marker-only diff preserves D4
behavior, source baseline elsewhere, exact filter, and fresh output stem.
**AC-M5D7M-R61-002:** The one run records the last reached marker or a
complete 990-test XML; timeout and stop obey both watchdogs without retry.
**AC-M5D7M-R61-003:** Evidence cites `REQ-M5D7M-R61-001/002`, records
artifact hashes and drift, and makes only a stage-localization inference.
**AC-M5D7M-R61-004:** After evidence, the temporary marker lines are
removed, the exact original test-source SHA-256 is restored, and Luna
independently verifies restoration. This does not authorize a behavioral
fix or close `AC-CUA-009`.

The pre-amendment porcelain had `511` entries with UTF-8 joined SHA-256
`FA25B260BDCED6919281C2D51759F3752C882A2A6D3E41BA94BFAC3B95339F15`.
All four CUA source/test hashes in the R12/R14 authorization remain the
baseline. Only the one named M5D7M test source, this contract, the independent
pre-gate/evidence records, and fresh R61 outputs are on the allowlist.

## Astra R62 runner-boundary observer authorization — 2026-09-28

R61 reached all D4 stage markers through `dispose:after`, then produced no
further log or XML while CPU rose until the exact-D4 post-marker watchdog
stopped only its task-created process tree. Its partial log SHA-256 is
`060B1753603C0E8221C4B5B4CD659DEB88BF834FBD009BBF5D26F3C7615B3E78`.
The preserved evidence is
`docs/verification/2026-09-28-m5d7m-r61-d4-stage-markers-evidence.md`
(SHA-256 `D8D51E55A7EE8138346E912D23E689FE5DDEF6505C81C5293DBF2F3D0E8DB179`).
Luna independently verified the stage sequence and restoration of the
unmodified D4 test source to SHA-256
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
The method body completed, but runner-level test completion and subsequent
fixture transition remain unobserved. An R54/R55/R61 log comparison and
the pinned Unity Test Framework `1.8.0` source leave `LogScope` validation,
work-item completion, and next-child transition unresolved. R61 is not a
pass for `AC-CUA-009`.

The pinned framework publicly supports assembly-level `ITestRunCallback`
registration via `TestRunCallbackAttribute`. Its `TestFinished` callback is
issued from `UnityWorkItem.WorkItemComplete` after result completion and
before parent-child continuation. Sol recommends a tiny filtered observer
instead of an external debugger or another unobserved selection run.

**REQ-M5D7M-R62-001:** Terra may add exactly one temporary test-only source
`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/M5D7MR62RunBoundaryObserver.cs`
and its Unity `.meta` in the existing Input.Unity PlayMode test assembly.
The source may contain only the documented assembly registration and a
sealed `ITestRunCallback` implementation. It must emit compact deterministic
`Debug.Log` markers prefixed `M5D7M-R62` for D4 `TestStarted` and
`TestFinished` (including result state), and HandoffMatrix fixture/case
`TestStarted`/`TestFinished`. Other callbacks must be no-op. No reflection,
threads, waits, filesystem writes, product calls, result mutation, exception
throwing, runtime/test assertion edits, package edits, or Unity settings
changes. Luna must independently review the exact source and `.meta` before
execution. The observer is temporary and must be removed with `apply_patch`
after evidence capture and independently verified absent; all previously
existing source hashes must remain unchanged.

**REQ-M5D7M-R62-002:** After Luna's implementation pre-gate, run exactly
one host-environment Unity `6000.6.0f1` PlayMode diagnostic with no `-quit`,
using the exact R60 selection filter:

`^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.HubPresentation\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests;^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Movement\.;^AcadeGameMaker\.Tests\.PlayMode\.TransferUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.PresentationUnity\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.HubUiOnlyQ0HandoffMatrixPlayModeTests`

Use only fresh, absent
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r62-run-boundary.xml`
and `.log`. Expected complete inventory is `990` tests. Use a `1900`
second external monotonic global watchdog and a separate `180` second
unchanged-log plus increasing-CPU watchdog after the exact D4 expected
exception or an `M5D7M-R62` D4/Handoff boundary marker. Stop only the exact
task-created process tree; no retry or version probe. Record callback
sequence, PID, CPU/log observations, XML counts/inventory/duration if
complete, artifact hashes, workspace drift, and run-generated scenes.
Preserve all partial outputs and generated scenes. Compile/auth failure,
missing XML after natural exit, nonzero failed/skipped/inconclusive, source
drift beyond the approved observer, or unexpected workspace mutation is a
hard stop.

**AC-M5D7M-R62-001:** Luna confirms observer-only source/meta, exact
filter, fresh stem, baseline preservation, and no prohibited side effects.
**AC-M5D7M-R62-002:** The one run distinguishes no D4 `TestFinished`, D4
finished without Handoff start, Handoff started, or complete 990-test XML,
without indefinite task processes or retry. Missing events are interpreted
only within the observed log and may remain timing-sensitive.
**AC-M5D7M-R62-003:** Evidence cites `REQ-M5D7M-R62-001/002`, preserves
hashes/drift, and makes no product-defect or parent acceptance claim.
**AC-M5D7M-R62-004:** After evidence capture, Terra removes only the two
temporary observer files; Luna confirms absence and baseline source hashes.
No result here closes `AC-CUA-009` or replaces an unfiltered full PlayMode
XML with zero failed/skipped/inconclusive.

The pre-amendment porcelain had `515` entries with UTF-8 joined SHA-256
`338B161612F44494020339811BAA91A85B475C6EAE234CA7A97D38B30C0AF8EE`.
The temporary observer source/meta, this contract, Luna pre-gate and
verification evidence, and the fresh R62 outputs are the entire R62
allowlist; existing dirty state and generated scene pairs remain preserved.

## Astra diagnostic closure — 2026-09-28

The R62 observer selection produced a complete `990/990` PlayMode XML with
zero failed/skipped/inconclusive, D4 `TestFinished Passed`, and the
HandoffMatrix fixture plus all five cases `Passed`. The temporary observer
source/meta were removed, and Luna independently confirmed restoration of
the D4 source SHA-256
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
The later uninstrumented, unfiltered CUA R15 full PlayMode run exited
naturally after `4890.2643413s` with `1142/1142` passed and zero
failed/skipped/inconclusive (XML SHA-256
`4EFEACC7447BD272CB361E41F91CEE18B120FFB5973AC4BE501DC4EE24A8F196`).
Luna independently reviewed that full result, and Astra accepted parent
`AC-CUA-009` in
`docs/verification/2026-09-20-costume-cua-implementation-evidence.md`.

R15's log advanced after approximately 42 minutes of silence following the
D4 expected exception, reaching the same later exception site observed in
the previous successful full-suite log. The earlier R11/R14/R59/R60/R61
watchdog stops remain factual preserved records, but **no current M5D7M
livelock or product defect is established**. Their liveness classification
is superseded by the complete R15 evidence; the short stale-log threshold
was unsuitable for this suite. This diagnostic child is closed with no
runtime or test-behavior fix and no authorization for further retries.
