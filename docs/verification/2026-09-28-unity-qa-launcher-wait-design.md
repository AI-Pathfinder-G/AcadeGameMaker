# Unity QA launcher wait design

Date: 2026-09-28  
Status: design only — no runner implementation, Unity execution, or acceptance claim

## Problem and trace

`qa/tools/Invoke-UnityQa.ps1` currently invokes `Unity.exe` directly, receives a
launcher return, and then waits only 120 seconds for XML. Unity 6000.6 can
leave the actual project Editor running after that launcher return. The wrapper
then emits “no test-results XML” even though the actual Editor later produces
the evidence. This was observed in the preserved C2/C2R records; it is not an
NUnit result.

This future QA-tool correction supports the evidence reliability needed by
**REQ-PLAT-004/005** and **AC-PLAT-002**. Existing runner comments naming
REQ-PLAT-012..014 / AC-PLAT-010..012 are legacy trace, not identifiers added
to the current platform contract. The bounded work contract's REQ-UQW/AC-UQW
identifiers govern this tool correction. It changes no product quality criteria.

## Proposed launcher ownership model

1. Keep the existing pinned-editor, license, project-path, and pre-existing
   same-project Editor checks. Do not kill, reuse, or start alongside an
   existing Editor.
2. Allocate the exact new result and log paths before launch as today. Start
   the launcher with `Start-Process -PassThru`; its process result is recorded
   as *launcher only*, never as the test result.
3. After launch, discover an actual Editor only by a private process inventory
   predicate requiring all of: the pinned Unity executable, the normalized
   exact project path, the exact requested `-testResults` path, and the exact
   requested `-logFile` path. Retain its PID and attach a process handle for
   its eventual exit code. Never print raw command lines; diagnostics may show
   only PID, executable basename, and the already-approved project/result/log
   paths.
4. If zero matching Editors appear during the bounded discovery interval, report
   `NoOwnedActualEditor` separately from XML absence. If more than one matches,
   report `AmbiguousActualEditorOwnership`; do not select or terminate either.
5. Wait for the owned actual Editor, not the launcher. A configurable,
   explicitly bounded `-AwaitSeconds` (for example 60..21600) replaces the
   silent 120-second XML-only wait. While it is alive, emit periodic compact
   progress (`PID alive; XML pending/present`) so an outer tool can yield
   progress without declaring a test failure.
6. Do not classify success when XML appears early. Success requires a readable
   NUnit XML document, the owned actual Editor exit code `0`, and the runner's
   strict closed policy: nonzero total, passed accounts for every row, zero
   failed/skipped/inconclusive, consistent complete counters. Empty/unreadable
   evidence cannot pass. The launcher exit code is ancillary diagnostics only.
   Public AwaitSeconds rejects zero; an internal immediate-deadline tool test
   never converts zero evidence to success. Encode a validated argument-token
   list with Windows-safe quoting, not naive ArgumentList array joining; golden
   round-trip tests preserve spaces/backslashes/quotes and exact flags. Reject
   malformed/unsupported paths before launch and never print argument lines.

## Closed outcome classification

| Condition | Classification | Meaning |
| --- | --- | --- |
| Owned Editor still alive at `AwaitSeconds` | `AwaitTimeoutRunning` | Incomplete observation, not a missing-XML or NUnit failure; no process is killed. |
| Owned Editor exits; XML absent | `MissingXmlAfterActualExit` | Failed QA execution evidence, with actual PID/exit code. |
| Owned Editor exits; log matches licensing/entitlement failure and XML is absent/unusable | `LicenseOrEntitlementFailure` | Environment failure, not a test failure. |
| Owned Editor exits; XML unreadable | `UnreadableXmlAfterActualExit` | Failed QA execution evidence. |
| Owned Editor exits; valid XML and nonzero actual exit or existing failing counters | existing test failure classification | Preserve current NUnit judgement. |
| Owned Editor exits `0`; valid nonzero XML has consistent counters and every row passed, zero fail/skip/inconclusive | success | Authoritative run result. |

The tool must also handle a race where a discovered PID ends before a process
handle can provide an exit code: classify it as an incomplete ownership/exit
observation, never as success merely because XML happens to exist.

## Bounded implementation and deterministic tests

No implementation is authorized by this note. A future Approved work contract
may allow only:

- `qa/tools/Invoke-UnityQa.ps1`;
- one deterministic PowerShell test file under `qa/tools/` for the extracted
  process-selection and outcome-classification functions; and
- QA evidence under `docs/verification/`.

The tests use synthetic process snapshots, clocks, XML/log fixtures, and a
recording launcher; they never launch Unity or inspect/alter a real license.
They must prove launcher-early/Editor-late success, bounded live timeout,
missing XML after actual exit, licensing classification, ambiguous ownership,
pre-existing competing-project rejection, and that diagnostic output omits a
synthetic command-line secret. They also prove result/log path matching is
exact and that no branch kills a process.

Excluded: runtime, Assets, Packages, ProjectSettings, license state, Unity
configuration, test filters, timeout relaxation of NUnit itself, credential
logging, process termination, and any change to existing historical evidence.

## Approval gate

This design is not an approval to edit the runner. Astra must first approve a
bounded QA-tool work contract; Terra may then implement the allowlisted files,
Luna independently verifies the deterministic tool tests and runner behavior,
and Astra alone accepts the evidence.
