# Unity QA owned-Editor wait — Terra implementation evidence

Date: 2026-09-28  
Status: implementation evidence only — independent Luna review and Astra acceptance pending

## Scope and trace

Implemented the Approved bounded QA-tool contract
`2026-09-28-unity-qa-owned-editor-wait.md` for **REQ-UQW-001..007** and
deterministic **AC-UQW-001..006** coverage.

Changed only:

- `qa/tools/Invoke-UnityQa.ps1`
- `qa/tools/Test-UnityQaOwnedEditorWait.ps1`

The preserved pre-change runner snapshot remains
`artifacts/uqw-before-runner.ps1.snapshot` SHA-256
`967438285318D6B52B74B7A1137C762D0059CEEC8DA62AD94AEAAC40683B0EFE`.
It is historical evidence for the active R22 invocation and was not edited.

## Deterministic execution

Executed only:

```powershell
pwsh -NoProfile -File qa/tools/Test-UnityQaOwnedEditorWait.ps1
```

Result:

```text
PASS: 43 deterministic owned-Editor wait self-tests.
```

The self-tests dot-source the exact runner parsing/selection/classification
functions and use synthetic argv, process snapshots, clocks, XML/log strings,
and a synthetic secret. They do not launch Unity, inspect a real license,
query live processes, write a result/log file, kill/restart a process, or
exercise product code.

Covered rows include Windows argument round-trip (spaces, quotes, trailing
backslash, and a flag-like filter value), case/separator exact path matching,
prefix collision, exact pinned executable, worker exclusion, duplicate
required flags, duplicate `-runTests`, one/duplicate owned candidates,
pre-existing project detection, discovery/progress boundaries, early XML,
actual nonzero/unknown exit, zero/skip/inconsistent counters, missing/truncated
XML, licensing versus transient refused-channel diagnostics, and private
secret-bearing command-line inspection.

## Frozen implementation hashes

- `qa/tools/Invoke-UnityQa.ps1`:
  `627281FC07DCDDCE28B1DC1E1AAA297A8A0D5D8A2652ABAD97B1805D5E14B2EA`
- `qa/tools/Test-UnityQaOwnedEditorWait.ps1`:
  `B6148D6BB33D6EE1F5B6F67AD35557FB3237DA73BD9C6D6E66708B01FAE03A3E`

`git diff --check` reported no whitespace errors for the two tool files. No
live end-to-end runner result, product verification, or acceptance is claimed.
