---
status: Verified
---

# Unity QA owned-Editor evidence wait

- Date: 2026-09-28
- Status: Verified — Astra 2026-09-28; bounded QA tooling only
- Approval/integration: Astra; implementation: Terra; independent review: Luna
- Parent: Approved VD-09 platform/quality, REQ-PLAT-004/005 and AC-PLAT-002
- Design: docs/verification/2026-09-28-unity-qa-launcher-wait-design.md

## Bounded purpose

Correct the QA wrapper's observed early-launcher-return false missing-XML
classification. R32's wrapper returned before its actual Editor completed
21/21; R22's actual Editor remains running after its wrapper returned. These
historical facts remain unchanged. No NUnit timeout, filter, assertion, product
criterion, current running Editor, runtime/configuration or license is changed.
This slice can be implemented outside Assets while R22 owns its frozen source.
Existing R22 evidence continues to identify the original runner and actual
Editor; a runner patch never retroactively changes that execution.

## Ownership and launch

Retain existing pinned-version/signature/license/path preflight and refusal to
overwrite results/logs. Process inventory unavailable or a pre-existing exact
project Editor blocks launch, rather than guessing that the project is free.
Inspect command lines privately; print no raw command line, token or license
contents. Compare parsed argument values, not substring matches, for exact
normalized project/results/log paths and the pinned executable. Windows path
comparison is case-insensitive, normalized and separator-aware. Prefix paths,
substring flags and argument-like quoted values cannot match.

Start the launcher hidden, retaining its process handle and ancillary exit
status. Discover/attach the actual run Editor by pinned executable and exact
`-projectPath`, `-testResults`, `-logFile` values; require `-runTests`. Exclude
asset workers and competing Editors. Bind PID plus creation time/handle so PID
reuse is not authority. The launcher may itself be the actual Editor if it
matches; it is not automatically assumed to be so. Duplicate matching
processes or unavailable inventory are ownership failures, never selection of
an arbitrary process. If a matched Editor exits before its exit status can be
observed, report incomplete evidence, not success based on XML alone.

Construct one validated argument-token list, quote each token using Windows
process argument rules, then pass the resulting safely encoded argument line
to Start-Process. Do not rely on ArgumentList array joining to preserve spaces
or quoting. Deterministic golden-token/round-trip tests cover spaces, trailing
backslashes, quoted strings, flag-like values and duplicate flags; malformed
or unsupported path/argument values reject before launch. No diagnostic prints
the encoded argument line. This changes encoding, not the test filter meaning.

Use a bounded discovery grace (30 seconds), followed by configurable
`AwaitSeconds` (default 7200, valid 60..21600), measured with a monotonic elapsed
clock. Polls/progress permit outer-tool yielding; compact progress is emitted
at most every 30 seconds. Never kill or restart a process, create a schedule,
or retry launch. At the deadline, a live owned Editor is
`AwaitTimeoutRunning`; explain that it was left running and evidence is
incomplete. This is not NUnit failure or missing XML after exit.
The public AwaitSeconds parameter rejects zero and out-of-range values; any
internal immediate-deadline deterministic case still cannot pass empty XML.

## Closed result policy

Only actual owned Editor exit plus a readable NUnit XML `test-run` with
nonzero total, all passed, zero failed/skipped/inconclusive and actual exit 0
is success. XML appearing before process exit is insufficient. Missing or
invalid counters, inconsistent totals, zero discovery, truncated XML and
unknown actual exit status cannot succeed. Launcher status remains diagnostic.

Separate classifications: `NoOwnedActualEditor`,
`AmbiguousActualEditorOwnership`, `OwnershipObservationUnavailable`,
`AwaitTimeoutRunning`, `ActualExitUnobserved`,
`MissingXmlAfterActualExit`, `UnreadableXmlAfterActualExit`,
`LicenseOrEntitlementFailure`, `TestFailure`, and `Completed`.
Inspect existing bounded licensing patterns only without usable results after
actual exit; the known transient refused-channel log must not override a valid
completed pass. A live Editor never becomes a license/NUnit failure merely
because output is late. Preserve existing nonzero CLI failure semantics and
describe evidence/environment/incomplete failures distinctly from NUnit
failure. Exact numeric exit codes may remain 0 success, 3 environment,
4 incomplete/evidence, 5 completed failed test execution.

## Requirements

- **REQ-UQW-001:** Retain safe preflight, exact fresh output paths and no
  competing same-project launch.
- **REQ-UQW-002:** Authenticate and retain the exact actual Editor, not an
  unrelated launcher/worker/prefix collision/PID reuse.
- **REQ-UQW-003:** Await bounded actual completion with compact progress;
  never kill/relaunch or label a live wait timeout as test failure.
- **REQ-UQW-004:** Require actual exit and complete nonzero zero-failure XML
  before success; preserve NUnit criteria without timeout/filter waivers.
- **REQ-UQW-005:** Distinguish environment, ownership, incomplete observation,
  missing/unreadable XML and completed test failure without credential output.
- **REQ-UQW-006:** Keep deterministic tool tests independent of real Unity,
  licenses and product runtime, with no hidden test-only production bypass.
- **REQ-UQW-007:** Preserve frozen R22 sources and historical evidence; change
  only the runner, its bounded tool test and current evidence/navigation.

## Acceptance criteria

- **AC-UQW-001:** Exact/prefix/quoted-path/case/separator/flag collision and
  worker matrices select only the intended process; duplicate/unavailable
  inventory and pre-existing project cases do not launch.
- **AC-UQW-002:** Early launcher return, Editor-late XML, XML-before-exit,
  actual nonzero/unknown exit, and PID-reuse cases prove actual ownership and
  exit are required, with no inferred pass.
- **AC-UQW-003:** Deterministic clocks prove discovery bound, live await timeout,
  progress intervals and exactly one launch, zero kill/relaunch/schedule calls.
- **AC-UQW-004:** Valid pass, zero discovery, fail/skip/inconclusive, inconsistent
  or absent counters and malformed XML prove the complete closed policy.
- **AC-UQW-005:** License failure versus transient channel refusal, missing XML
  after exit and unreadable XML prove distinct classifications. Synthetic
  command-line secrets never appear in diagnostic/progress output.
- **AC-UQW-006:** Deterministic PowerShell tests run without Unity/license
  access and pass all cases; source review finds no production bypass, process
  kill, new dependency or runtime/Assets/configuration change.
- **AC-UQW-007:** Luna independently verifies exact code/test hashes and
  results with P0=0/P1=0 before Astra accepts. No live end-to-end success is
  claimed without a later actual invocation of this exact runner.

Trace: 001→AC001; 002→AC001/002; 003→AC003; 004→AC002/004;
005→AC005; 006→AC006; 007→AC006/007.

## Allowlist after approval

Only `qa/tools/Invoke-UnityQa.ps1`, one new
`qa/tools/Test-UnityQaOwnedEditorWait.ps1`, this contract's status/evidence and
minimal docs navigation/participation ledger may change. Extract small
runner-owned parsing/classification/wait functions for deterministic tests;
tests may load those exact function definitions without running preflight or
using a public bypass that would skip production ownership checks. Test
snapshots/clock/launcher ports are for tool tests, not product identities or
fake Unity acceptance evidence. Production calls real process/clock/file APIs.

No Assets, runtime, NUnit tests/filters/timeouts, Packages, ProjectSettings,
credentials/license state, dependencies, unrelated script or historical
evidence changes. Do not run additional Unity while the current project
Editor is active. If reliable process attachment needs broader permissions,
dependencies, credential output or process termination, stop for Astra; no
guessing, silent retry or false success is permitted.

## Delivery gate

Review is not implementation authority. Luna reviews failure modes; Astra
alone changes to Approved and allocates the allowlist. Terra implements with
REQ-UQW references, Luna verifies AC-UQW independently, Astra integrates. A
later real QA run is separately evidenced; deterministic tooling acceptance
does not establish product acceptance or alter ongoing C2/C2R results.

### Astra approval — 2026-09-28

Luna reviewed amended Review SHA
`97956C258ABE23D08A5F83FDEE423A85E21EDFCB63DCEB8614FBE59EC6D51AD8`
and design SHA
`D0033AB6D8E26C01D229C67DA1FC3452ADCB2638AE87B5ABCDA2F0795CC667D0`
with all seven ACs mapped and P0/P1=0 after the two clarity corrections.
Astra approves exactly the two QA tool files and associated current evidence.
Terra may implement outside Assets while R22 remains frozen, but may run only
deterministic tool tests, never a second Unity instance or license operation.
Luna must independently verify before integration. The current R22 run uses
the original wrapper; its early return and eventual actual result remain
separate historical facts. No new user product decision is introduced.

### Astra integration acceptance — 2026-09-28

Approved-at-execution contract SHA-256:
`BCD5ABF85BDAC07662E162FDB85989C14081AC1FCBEE71321598E9900FCC3580`.
The exact accepted runner is
`A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`,
with deterministic test
`2FABA87A41D70167DACAA25B13808852F1A9BF8C32146E1053E5E14005CDADF2`.
Main and Luna independently ran all 104 deterministic checks. Main R33
actually invoked this runner and completed 32/32 existing focused EditMode
tests with actual exit 0, no skips/inconclusive and no remaining Unity process.
Luna independently verified exact XML/log/source hashes and AC-UQW-001..007
with P0=0/P1=0 in
`2026-09-28-unity-qa-owned-editor-wait-luna-r33-actual-review.md`.

Astra accepts only the QA ownership/wait tool scope. This does not accept
C2/C2R, a full regression suite or any live menu/scene/gameplay behavior.
Historical failed R22 and unaccepted 43/91/97-check snapshots remain intact.
No PID absent from R33 output is invented, and Windows PowerShell 5.1 runtime
compatibility remains unverified. Current operational runtime is pwsh.
