# R35 omitted worker provenance — Luna forward review

Date: 2026-09-28  
Scope: read-only evidence-boundary review while R35 is executing. No Unity,
source, test, specification, manifest, or runtime change.

## Finding

The R35 before-manifest covered the current AcadeGameMaker code/configuration
closure but did not include the two external child-worker inputs used by
`ProfileResetDiskProcessV1Tests`:

- `qa/fixtures/ProfileResetCrashWorker/Program.cs`
- `qa/fixtures/ProfileResetCrashWorker/ProfileResetCrashWorker.csproj`

The EditMode fixture is the only repository test reference to that worker
project, and it contains exactly `51` internal worker cases. The worker
project is package-free and is built/invoked only by that fixture. No additional
R35 cases are implicated by this omission.

## Required bounded closure

R35 must finish unchanged. Its non-worker result may contribute only the exact
`562` current names obtained by removing all `51`
`ProfileResetDiskProcessV1Tests` names, and only after the full shared `667`
file manifest is confirmed unchanged and the R35 XML name set is exact.

After R35's Editor is gone, one fresh R36 run of exactly the `51` worker-case
names is sufficient to close the omitted provenance. R36 must capture a fresh
complete manifest including the 667 shared files, both worker inputs and any
direct worker build configuration, then require exact before/after equality,
zero failed/skipped/inconclusive cases, and exact worker-name equality. The
final current partition is then `562 + 51 = 613` distinct names; no old 592
rows or historical 202-file digest may be reused.

## Verdict

Current status: **P0=0; P1=1 evidence-boundary blocker**, limited to the
omitted worker provenance. A bounded R36 as specified is sufficient; no other
affected cases require a fresh rerun based on repository references. Once its
manifest and exact 51-case XML pass, the temporary P1 can close without
changing runtime authority, acceptance criteria, or user decisions.

Astra may preserve the existing `AC-M5D7QC2-010` /
`AC-M5D7QC2R-007/008` criteria and approve this evidence-repair sequence; no
contract amendment or product decision is needed. Final C2/C2R integration
remains Astra's authority.

## R36 pre-gate replay

Independent check of `artifacts/c2-r36-selection-preflight.json`:

- SHA-256: `B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3`.
- R35 expected names: `613`; R36 names: `51`; R36 is an exact subset and its
  complement is `562`, with overlap `0`.
- Positive selector
  `^AcadeGameMaker\.Profile\.Tests\.ProfileResetDiskProcessV1Tests\.`
  selects exactly the `51` worker names, no other R35 name, and no fixture
  parent node.
- Worker directory non-bin/obj files are exactly `Program.cs` and
  `ProfileResetCrashWorker.csproj`; applicable repository build-config scan is
  empty; observed SDK is `10.0.401`.
- Fresh before/after complete manifest is required and remains open until the
  actual R36 run.

This confirms the bounded R36 plan with **P0=0; temporary evidence P1=1**.
R35 must finish unchanged before R36; no old-592 or historical-202 digest
reuse is permitted.
