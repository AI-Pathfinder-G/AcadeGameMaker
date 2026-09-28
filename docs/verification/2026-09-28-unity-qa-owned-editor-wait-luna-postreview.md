# Owned-Editor wait post-review — Luna

Date: 2026-09-28  
Scope: read-only review of the two allowlisted QA files; deterministic self-test only. No Unity, license, production-runner invocation, process launch/kill, Assets, or acceptance.

## Frozen evidence

- Contract SHA-256: `BCD5ABF85BDAC07662E162FDB85989C14081AC1FCBEE71321598E9900FCC3580`.
- Runner `qa/tools/Invoke-UnityQa.ps1`: `627281FC07DCDDCE28B1DC1E1AAA297A8A0D5D8A2652ABAD97B1805D5E14B2EA`.
- Deterministic self-test `qa/tools/Test-UnityQaOwnedEditorWait.ps1`: `B6148D6BB33D6EE1F5B6F67AD35557FB3237DA73BD9C6D6E66708B01FAE03A3E`.
- Pre-change runner snapshot: `artifacts/uqw-before-runner.ps1.snapshot`, SHA-256 `967438285318D6B52B74B7A1137C762D0059CEEC8DA62AD94AEAAC40683B0EFE`.
- Authorized command result: `PASS: 43 deterministic owned-Editor wait self-tests.`

## Findings

P0: none.

P1-1 — incomplete XML counters can pass. `Get-ClosedQaOutcome` casts absent
`failed`, `skipped`, or `inconclusive` attributes to zero. For example,
`<test-run total="1" passed="1"/>` satisfies the current consistency and
zero-counter checks and returns `Completed`, although the contract requires
complete counters. Require presence and valid numeric representation of all
four counters before success; add a deterministic missing-attribute row.

P1-2 — production wait-loop behavior is not exercised by the 43 self-tests.
The self-test covers parser/selector/classifier helpers and boundary predicates,
but does not drive `Invoke-OwnedEditorWait` with an injected deterministic
clock/process/launcher. Consequently discovery attach race, live timeout,
progress cadence, and exactly-one-launch/no-relaunch behavior are not directly
proven by the tool test. Add a runner-owned seam or deterministic harness that
executes the production wait state machine without Unity or real process APIs;
retain the no-kill/no-relaunch assertions.

## AC map

- **AC-UQW-001:** 43-row self-test covers encoded argv, exact paths,
  prefix/worker/duplicate collisions, pinned executable, and ownership
  ambiguity. The production snapshot uses PID plus creation time/handle.
- **AC-UQW-002:** helper classification covers early XML, actual exit,
  unknown exit, and attachment race; the production-loop test gap is P1-2.
- **AC-UQW-003:** discovery/progress boundary predicates pass, but full
  production-loop timing/launch assertions remain P1-2.
- **AC-UQW-004:** pass, zero/skip/inconsistent counters, absent/truncated XML
  are covered; missing-counter completeness is P1-1.
- **AC-UQW-005:** license failure, transient refusal, missing and unreadable
  XML are distinct; valid XML with bad actual exit is a test failure.
- **AC-UQW-006:** self-test ran under `pwsh -NoProfile` with no Unity/license/
  process launch; no production bypass or dependency change observed.
- **AC-UQW-007:** hashes and this independent result are recorded; live
  end-to-end evidence remains pending.

No approval or implementation acceptance is claimed. The active R22 Editor was
not touched.

## Additional independent counterexamples

The following concrete cases expand, rather than replace, P1-1/P1-2:

- Empty `failed`, `skipped`, or `inconclusive` attributes are converted to
  zero and can yield `Completed`; these fields must be present and strictly
  numeric.
- `ConvertFrom-WindowsArgumentLine` uses `[int]($slashes/2)`, which rounds
  rather than floors in PowerShell. A token containing three backslashes before
  a quote therefore decodes incorrectly (encoded/decode length mismatch), so
  the golden argv suite must include odd backslash runs immediately before
  quotes and verify native Windows round-trip semantics.
- An empty `Get-OwnedEditorCandidates` result can become a null pipeline value;
  under `Set-StrictMode -Version Latest`, dereferencing `.Count` can throw
  instead of producing an unambiguous zero-candidate ownership outcome. The
  production wait path must normalize empty results to an actual empty array.
- `Get-UnityProcessSnapshots` projects incomplete CIM records to empty
  executable/command-line/creation fields. Those records are then silently
  non-matches, allowing preflight to proceed as if inventory were clear rather
  than classifying inventory as unavailable. Incomplete process inventory must
  fail closed.
- `Attach-OwnedEditor` compares CIM creation time with `Process.StartTime` at
  a one-millisecond tolerance. CIM/provider timestamp precision and conversion
  can differ for the same process; add a deterministic same-process precision
  case while retaining PID-reuse rejection, using a documented robust identity
  comparison rather than a flaky boundary.

These remain P1 until the allocated runner-only correction is independently
re-read and tested; no fixes were applied in this review.
