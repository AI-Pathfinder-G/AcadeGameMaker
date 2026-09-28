# Owned-Editor wait corrective post-review — Luna

Date: 2026-09-28  
Scope: independent review of the frozen runner/self-test correction and one deterministic self-test invocation. No Unity, license, live-CIM, production-runner launch, process kill, Assets, or acceptance.

## Evidence

- Runner `qa/tools/Invoke-UnityQa.ps1` SHA-256:
  `270E6383F86D6051148554C7BF6916D581CA83FE80AC7A7AC707C5FEE2CD8479`.
- Self-test `qa/tools/Test-UnityQaOwnedEditorWait.ps1` SHA-256:
  `3CE584F4FE8763DC48FE67D5500F4DBFDF5B93FA7F1731ACD839E262619E1856`.
- Corrective evidence SHA-256:
  `29B35E2B32B08E0E84FA6E5AADAFC710038AB29C1A0B0B06B901315AD4DADB9E`.
- Frozen approved contract SHA-256:
  `BCD5ABF85BDAC07662E162FDB85989C14081AC1FCBEE71321598E9900FCC3580`.
- Command: `pwsh -NoProfile -File qa/tools/Test-UnityQaOwnedEditorWait.ps1`.
- Result: `PASS: 91 deterministic owned-Editor wait self-tests.`

## Independent verdict

P0: 0. P1: 0 in the corrective snapshot. The previously reported gaps are
closed: all five XML counter attributes must be present and decimal-free
nonnegative integers; odd backslash-before-quote parsing is floor-corrected and
checked against Windows `CommandLineToArgvW`; zero candidates are normalized;
partial CIM records fail closed; and creation identity uses common UTC
microsecond ticks (the observed active-R22 CIM/Process delta was 0.5 microsecond,
within normalization; one-microsecond and one-millisecond changes reject).

The exact production launch-and-wait state machine is driven through recording
ports by the self-test: one launch, one attach/dispose, no kill/relaunch,
30-second progress, bounded live timeout, attachment race, ambiguity,
observation failure, and zero-candidate discovery are all exercised without
Unity or live process APIs.

## AC-UQW map

- **001:** native argv round trips, exact path/flag matching, duplicate/worker/
  prefix collisions, and ownership candidates pass.
- **002:** early XML, unknown/bad exit, attach race, PID/creation identity and
  no-XML-alone-pass cases pass.
- **003:** recording-port clock proves bounded discovery/await, compact progress,
  exactly one launch, and no kill/relaunch.
- **004:** valid pass, nonzero/zero/inconsistent/missing/malformed counters,
  ambiguity and zero discovery fail closed.
- **005:** license failure, transient refusal, missing/unreadable XML and
  incomplete observation remain distinct.
- **006:** 91 deterministic tests use no Unity, license, live CIM, dependency,
  or production bypass.
- **007:** hashes and this independent result are recorded; a real invocation
  of this exact runner remains required before acceptance.

No remaining P0/P1 finding. This is a Luna verification record, not approval;
the active R22 Editor and product evidence remain untouched.

## Main review correction — two remaining P1s

The preceding zero-P1 statement is superseded by this bounded correction;
source and contract remain frozen.

- **P1-1, malformed XML classification:**
  `Get-ClosedQaOutcome $true 0 $true '<broken' ''` currently returns
  `TestFailure`/exit 5. The approved closed policy requires unreadable XML
  after actual exit to remain the distinct `UnreadableXmlAfterActualExit`
  evidence failure (exit 4), not a completed-test failure classification.
  The deterministic assertion must retain that distinction rather than waive
  it as a test failure.
- **P1-2, relative CIM argv path:**
  `ConvertTo-NormalizedWindowsPath '..\\AcadeGameMaker' 'candidate'` calls
  `GetFullPath` before checking rootedness and can turn a relative process
  argument into the current QA working-directory absolute path. Explicit user
  output paths may be made absolute by `New-OutputPath`; discovered CIM
  command-line witnesses must instead reject non-rooted/malformed project,
  results, or log values and classify inventory as unavailable/preflight
  blocked, never normalize them into a false exact match.

The prior empty-candidate strict-mode concern is closed: production paths and
the self-test now normalize zero candidates to an empty collection. No new P0
finding was identified. Acceptance remains blocked pending these two narrow
runner/test corrections and fresh independent verification.

## Final follow-up compatibility finding

The two prior P1s are closed in the current snapshot: malformed/empty/decimal
counter rows now classify as `UnreadableXmlAfterActualExit`, and relative,
drive-relative, and root-relative CIM witness paths are rejected before
normalization. The self-test passes 97 rows and covers both fixes.

One additional P1 remains for the stated PowerShell compatibility surface:
`ConvertTo-NormalizedWindowsPath` now calls
`[IO.Path]::IsPathFullyQualified`. That API is available on modern .NET/pwsh
but is not part of the Windows PowerShell 5.1/.NET Framework `System.IO.Path`
surface. Thus the runner can fail before launch under Windows PowerShell 5.1,
despite the `pwsh` self-test passing. The rooted-path check needs a
5.1-compatible implementation (while preserving rejection of `C:relative`
and `\root-relative`) plus a genuine 5.1 syntax/API smoke check. No P0 was
identified; acceptance remains pending this compatibility correction and fresh
independent verification.
