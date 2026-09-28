# C2/C2R required regression plan — Luna pre-execution

Date: 2026-09-28. Runtime and tests are frozen for R12; this is a plan, not
execution evidence or acceptance.

## Required non-process partition

Use three runner partitions to avoid duplicate matrix execution. This is runner
isolation only, not a product/spec waiver. Across R21+R22+R23, reconcile the
fully qualified XML test-case names and counts; the union must exclude only
`ProfileResetRestartProcessV1Tests`, with no omitted case, Skip/Ignore, or new
assembly. Record source before/after hashes for every partition and require
zero failed, skipped, and inconclusive rows in each XML.

- **R21:** all `ProfileResetMemoryCutoverFaultMatrixV1Tests` rows (the complete
  73-row matrix), and no duplicate execution elsewhere.
- **R22:** PlayMode union of
  `AcadeGameMaker.Tests.PlayMode.InputUnity`,
  `AcadeGameMaker.Input.Unity.PlayMode.Tests`, and
  `AcadeGameMaker.Tests.PlayMode.HubPresentation`, excluding the external
  process class and the already-owned matrix class.
- **R23:** EditMode union of `AcadeGameMaker.Profile.Tests`,
  `AcadeGameMaker.Tests.EditMode.Profile`,
  `AcadeGameMaker.Tests.EditMode.InputUnity`, and
  `AcadeGameMaker.Tests.EditMode.HubPresentation`.

No contract criterion requires these partitions to run in one Unity invocation;
the required condition is the aggregate focused/required regression result.

### R23 namespace inventory correction

The complete R23 selector must match both legacy and newer Profile fixtures:

`^(AcadeGameMaker\.Profile\.Tests|AcadeGameMaker\.Tests\.EditMode\.(Profile|InputUnity|HubPresentation))`

This includes the existing 15-fixture `AcadeGameMaker.Tests.EditMode.Profile`
surface as well as `AcadeGameMaker.Profile.Tests`. Report discovered and
executed fully qualified case names for every matched file; a zero-discovery
namespace is a runner/setup failure, not an empty pass. No case is waived.

Run these focused non-process groups in the existing assembly:

1. `ProfileResetRestartBootstrapV1Tests` (all 33 rows: ordinary absent-root
   trace, production restart happy path, Busy/repair, promotion, reflected
   state, all restart checkpoints, and owned-operation faults). The direct
   promotion rows are router-transition unit coverage; they are not by
   themselves proof of production Adapter Awake/private-CWT minting. The
   production happy-path row must close that narrow evidence gap with exact
   receipt, maps-disabled, no ordinary Prepare/UTC, current-cell/generation,
   and private witness assertions.
2. R21 owns `ProfileResetMemoryCutoverFaultMatrixV1Tests` (all 73 rows: 66
   parameterized plus 7 direct). This must be rerun despite the unchanged
   `ProfileResetMemoryCutoverV1.cs` hash: Adapter/Router integration changed,
   and the final gate requires fresh zero failed/skipped/inconclusive evidence.
3. `DesktopProfileLaunchAdapterV1Tests`, including the focused legacy root
   getter rows for InvalidOperationException, IOException,
   UnauthorizedAccessException, and ArgumentException, plus the strict
   Reserve → root → UTC → Prepare trace, token ownership, teardown, and
   ordinary publication rows.
4. R22/R23 include the existing `InputUnity`/Hub focused legacy tests covering router Awake,
   Start, Update, FixedUpdate, action ownership, callback teardown, and
   mutable/reflected-state containment. Reuse R8/R10 only as historical
   comparison; do not treat them as final evidence.

For each group require actual XML with zero failed, skipped, and inconclusive
rows, and record source hashes. Any compile failure is P0 and stops acceptance.

## External process phases

Main must execute each phase in a distinct Unity process with the required
environment and owned temp base: `Prepare`, `Resume`, `DeleteGate`, then
`OrdinaryAfterDelete`. Main alone spawns/kills and validates PID/evidence;
DeleteGate’s parent termination is not an NUnit pass. AC-M5D7QC2R-005/006 close
only after phase-specific evidence proves fresh static state, non-default
archive seed, exact r0/current-cell/generation agreement, barrier removal,
disabled maps, one receipt, and ordinary no-receipt/no-resurrection after the
death boundary.

No user decision, new model, Ollama route, source edit, or Unity call is
authorized by this plan.

## Current execution clarification after R24 — Astra

The earlier 73-row/33-row descriptions above preserve the original plan.
The final matrix now contains 74 rows, bootstrap 37 rows, and data two
additional root-witness rows. R21 (72/73) and R24 (41/45) are failed historical
evidence, not reused as final passes. R31 reruns the focused 45 cases after
Luna-reviewed history-equality corrections. R25 owns the complete fresh
74-row matrix; R22 and R23 retain their namespace partition above. The final
PlayMode union excludes only the externally orchestrated process fixture,
never a matrix case. Fresh process phases R26–R30 use separate new owned
temp bases and actual distinct Editor PIDs. Run numbering is an evidence
identifier, not chronological order; recorded timestamps determine chronology.
All accepted partitions must use the same frozen runtime/test source hashes
and zero failed/skipped/inconclusive results before independent final review.

## R23 test-only successor partition — Astra after Luna pre-gate

Actual R23 is 595/597 with two failures, never a passing run. Preserve its
XML unchanged. Luna's R32 post-patch pre-gate verifies the two-file correction
and unchanged 202-source dependency fingerprint. Reuse only the 592 unique
passing cases outside the original AC006 aggregate and all four Q0 audit
cases. R32 executes all 17 replacement controller cases plus all four Q0
cases, using generated names:

`HubMenuPresentationControllerV1Tests\.AC006_(NestedValueBackings|Ready_|Pending_|Consumed_)|HubUiOnlyQ0ScopeAuditEditModeTests`

Final EditMode evidence is an explicitly versioned 592+21 partition, conditional
on zero failed/skipped/inconclusive R32 and unchanged relevant sources/helpers.
It is not an unfiltered full-suite claim. The original AC006 assertion mapping
must remain complete, with no timeout increase or skipped probe. R25/R31 and
fresh process evidence remain valid only for their unchanged frozen sources.

## R22 baseline reconciliation — Luna (pre-XML)

The historical unfiltered PlayMode baseline contains 446 unique qualified
case names across the three current non-process namespace surfaces:

- `AcadeGameMaker.Tests.PlayMode.InputUnity`: the legacy InputRouter,
  camera/failure/terminal, preparation, binding, cutover-data, and ordinary
  profile tests under `Tests/PlayMode/InputUnity`.
- `AcadeGameMaker.Input.Unity.PlayMode.Tests`: DesktopProfileLaunchAdapter,
  HubEntryHandoffLatch, HubUiOnly Q0 coverage/handoff/remaining tests, and
  the corresponding support file under the same directory.
- `AcadeGameMaker.Tests.PlayMode.HubPresentation`: the four Hub menu
  presenter/intent/resolution/failure-teardown files under
  `Tests/PlayMode/HubPresentation`.

Fresh R22 XML must retain all 446 baseline full names, add the current
non-process C2/C2R rows, and contain zero failures, skips, or inconclusive
rows. `ProfileResetRestartProcessV1Tests` is the sole external-process
fixture excluded from this PlayMode partition; the 74-row
`ProfileResetMemoryCutoverFaultMatrixV1Tests` is excluded from R22 because
R25 owns it. Therefore R22 + R25 reconciliation excludes only the external
process class, with no silent namespace/file/test-case omission. No count is
accepted until the fresh XML is inspected by full qualified name.

## Resumed R34 positive-selector execution — Astra

The user lifted the temporary pause. Actual R22 was 610/614 with four
external-process environment failures: its namespace regex admitted excluded
fixtures through matching ancestor nodes. Preserve that failed XML unchanged.
Terra traced the installed groupNames/FullNameFilter/NUnit Pass implementation;
Luna independently reviewed it in
`2026-09-28-vd09-m5d7q-c2r-r22-selector-luna-pregate.md`, P0=0/P1=0.

Astra authorizes one unchanged PlayMode R34 invocation using the exact positive
23-fixture leaf-prefix selector and sorted 536-case list in
`artifacts/c2-r34-selection-preflight.json`. It selects no matrix or external
process case; all 446 historical scoped names remain present. R25 continues
to own the 74 matrix cases. This corrects execution selection, not product
criteria, a Skip, an assertion, or any NUnit timeout. AwaitSeconds 10800 is
the QA observation bound only. Main alone invokes the Verified UQW runner.
Actual fresh XML must match all 536 preflight names exactly and have zero
failed/skipped/inconclusive before acceptance is considered.

Fresh explicit 204-file source/meta/asmdef inventory is stored in
`artifacts/c2-r34-source-before.json`; compare every path/hash after execution.
The historical 202-file aggregate serialization has not been reconstructed
from its surviving description. Do not assert current equality to that old
digest merely from its count or four unchanged runtime anchors. The historical
R32/592+21 review remains factual history. Before final C2/C2R integration,
either reproduce its original dependency evidence exactly or run the current
complete approved EditMode partition fresh and reconcile all qualified names.
No C3 source edit occurs while this frozen regression owns the baseline.

## R35 fresh complete EditMode execution approval — Astra

Terra's R35 execution plan and Luna's independent R35 pre-gate are accepted
for execution planning only (P0=0/P1=0). After R34's owned Editor has exited,
Main may execute one unchanged EditMode invocation using the exact selector
and 613 expected qualified names in `artifacts/c2-r35-selection-preflight.json`.
This includes all 51 ordinary internal process-worker cases; it does not
manually invoke the four external PlayMode process phases.

Immediately before R35, capture the complete current code/meta/asmdef dependency
inventory and relevant configuration/fixture assets. Require byte-for-byte
path/hash equality afterward, exact fresh XML name-set equality, zero failed,
skipped or inconclusive cases, actual owned-Editor exit, and Luna's independent
actual-result review for `AC-M5D7QC2-010` and `AC-M5D7QC2R-007/008`.
The old 592 passes and unreconstructed historical 202-file digest are not
reused as final evidence. No source/test, Skip, assertion, NUnit-timeout or
runtime contract change is authorized by this execution approval.

## R36 worker-provenance closure approval — Astra

Main identified two worker-build inputs missing from the preserved R35 before
manifest. Terra's follow-up plan and Luna's independent provenance review
confirm exactly 51 affected internal worker cases, with one temporary P1
evidence blocker. R35 must finish unchanged; its factual XML is never rewritten.
Main scanned the worker-directory ancestry within the repository: no global,
Directory.Build/Packages or NuGet config was found. Observed SDK is 10.0.401;
repeat the configuration scan and SDK observation immediately before R36.

Astra approves one R36 EditMode invocation only after R35's Editor exits.
Use the positive fixture-leaf selector and exact 51 names in
`artifacts/c2-r36-selection-preflight.json`; preserve ordinary internal worker
behavior, NUnit settings and all source files. Capture a fresh complete
manifest containing all 667 shared files, both worker inputs and every
discovered repository-local build config. Require before/after path/hash
equality, zero failures/skips/inconclusive, and exact 51-name equality.

Only if R35 has the exact current 613-name set and its shared 667-file manifest
is unchanged may its 562 non-worker names be retained. Combine those with the
fresh 51 R36 names: overlap 0, exact current union 613. Independently verify
shared-file identity across both runs and obtain Luna's actual-result review
before closing `AC-M5D7QC2-010` / `AC-M5D7QC2R-007/008`. R35's original 51
worker rows are not reused for input-provenance closure, nor are the old 592
passes or historical 202-file digest. Non-worker failure, changed shared inputs
or missing/extra names require a new technical gate, not a weakened criterion.
This is additional evidence repair only, not a runtime/contract/product change.
