# Owned-Editor wait final post-review — Luna

Date: 2026-09-28  
Scope: independent review of the frozen runner/self-test and one deterministic self-test invocation. No Unity, live CIM, license, production launch, process kill, Assets, or acceptance.

## Evidence

- Runner SHA-256: `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`.
- Self-test SHA-256: `2FABA87A41D70167DACAA25B13808852F1A9BF8C32146E1053E5E14005CDADF2`.
- Command: `pwsh -NoProfile -File qa/tools/Test-UnityQaOwnedEditorWait.ps1`.
- Result: `PASS: 104 deterministic owned-Editor wait self-tests.`
- The historical 43/91/97 results remain unaccepted records; they are not
  combined with this fresh result.
- Windows PowerShell 5.1 compatibility was not verified: the optional attempt
  was stopped by host execution policy before script load. No policy bypass or
  change was made; current authorized operational runtime is pwsh.

## Verdict

P0: 0. P1: 0 in this frozen patch. The previous ownership issue is closed:
ordinary same-project non-test Editors block preflight even with relative logs,
foreign/non-test secondaries do not poison owned-candidate discovery, and a
`-runTests` candidate with relative/drive-relative/root-relative declared paths
fails closed. Unknown/duplicate project declarations remain fail-closed.

The runner uses a Windows PowerShell 5.1-compatible rooted-path predicate,
strict complete XML counters and unreadable-XML classification, common UTC
microsecond creation identity, and the exact recording-port production wait
state machine. Native argv/quoting, ambiguous/empty/opaque inventory, attach
race, bounded clock/progress, one launch/no kill-relaunch, license distinction,
and valid XML with bad actual exit are covered by the 104 checks.

## AC-UQW map

- **001:** exact argv/path/flag, worker, duplicate, foreign, and ownership
  matrices pass.
- **002:** launcher/Editor separation, early XML, exit/attach/PID identity and
  relative witness rejection pass.
- **003:** bounded discovery/await, 30-second progress, one launch, and no
  termination/relaunch pass through recording ports.
- **004:** complete nonzero counters, malformed/empty/decimal counters, zero
  discovery, ambiguity and inconsistent totals fail closed.
- **005:** license, transient refusal, missing/unreadable XML and unknown
  observation outcomes remain distinct.
- **006:** deterministic tests use no Unity/license/live CIM or production
  bypass and introduce no dependency.
- **007:** exact hashes and this independent result are recorded; a future
  real invocation remains required before acceptance.

No remaining P0/P1 finding. This is a Luna review, not Astra approval or live
end-to-end product evidence.
