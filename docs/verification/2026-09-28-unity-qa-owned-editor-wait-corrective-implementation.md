# Owned-Editor wait corrective implementation evidence

Date: 2026-09-28  
Scope: REQ-UQW-001..007 / AC-UQW-001..007 corrective QA-tooling slice only.

## Historical evidence retained

The first implementation evidence remains historical and unmodified:
`docs/verification/2026-09-28-unity-qa-owned-editor-wait-implementation.md`
recorded 43 deterministic checks against the earlier hashes. Luna subsequently
identified two P1 gaps in its post-review: incomplete NUnit counters could be
coerced to zero, and the recording checks did not drive the owned-Editor wait
state machine. This record is additive; it does not reclassify that earlier
run as acceptance.

## Corrective changes

- `Get-ClosedQaOutcome` requires every `test-run` counter attribute to be
  present, nonempty, decimal-free, nonnegative integer text before it can
  classify a result as completed.
- Process inventory is complete-or-fail-closed before either preflight or
  discovery makes an ownership decision. Empty candidate sets are normalized
  to an empty array.
- PID attachment compares UTC creation ticks at the common microsecond
  precision of CIM and `Process.StartTime`; it does not use a tolerance window.
- The runner-private recording-port state machine is used by the production
  launch-and-wait path and by deterministic checks. It records one launch,
  no relaunch/kill behavior, discovery, ambiguity, opaque inventory,
  attachment race, progress, timeout, exit, and disposal outcomes.
- Windows argument parsing uses floor semantics for backslashes before quotes.
  Golden vectors are also checked against `CommandLineToArgvW` on Windows.
- A subsequent Astra counterexample correction preserves the contract's
  evidence/NUnit distinction: truncated XML and missing, empty, or malformed
  counter attributes are `UnreadableXmlAfterActualExit`, while readable numeric
  zero/skip/inconsistent counts remain `TestFailure`.
- Full Windows path qualification is checked before normalization. Relative,
  drive-relative, and root-relative declared CIM project/results/log paths,
  and duplicate declarations, make inventory incomplete rather than allowing a
  false "no competing Editor" conclusion.
- A later live-inventory observation separated two scopes: ordinary non-test
  Editors are evaluated only for an absolute, unambiguous project witness at
  preflight, while owned QA discovery validates results/log witnesses only
  after `-runTests` is present. This prevents a non-test secondary Editor's
  relative log from becoming an owned-run inventory failure, without relaxing
  same-project preflight blocking or test-run path validation.
- The full-path predicate is implemented without newer framework-only path
  APIs, retaining compatibility with Windows PowerShell 5.1 while the
  operational self-test host remains PowerShell 7.

## Deterministic execution

Command:

```powershell
pwsh -NoProfile -File qa/tools/Test-UnityQaOwnedEditorWait.ps1
```

Result: `PASS: 104 deterministic owned-Editor wait self-tests.`

The execution used synthetic snapshots, clocks, XML/log text, and recording
ports only. It did not invoke Unity, inspect licensing, query live processes,
launch/kill processes, or produce QA XML/log evidence.

## Frozen inputs for independent review

- Approved contract SHA-256:
  `BCD5ABF85BDAC07662E162FDB85989C14081AC1FCBEE71321598E9900FCC3580`
- Runner SHA-256:
  `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`
- Deterministic test SHA-256:
  `2FABA87A41D70167DACAA25B13808852F1A9BF8C32146E1053E5E14005CDADF2`
- Original-runner snapshot SHA-256:
  `967438285318D6B52B74B7A1137C762D0059CEEC8DA62AD94AEAAC40683B0EFE`

This is implementation evidence only. Independent Luna review and an actual
owned-Editor run remain required before acceptance under AC-UQW-007.
