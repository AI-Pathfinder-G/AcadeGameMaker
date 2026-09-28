# R35 worker-fixture closure correction — Terra plan

Date: 2026-09-28  
Status: read-only follow-up plan. R35 is already running and must finish
unchanged; this record neither executes nor reclassifies it.

## Discovered manifest gap

The frozen R35 before-manifest omits the external local build inputs used by
the selected EditMode fixture
`AcadeGameMaker.Profile.Tests.ProfileResetDiskProcessV1Tests`. Its `BuildWorker`
step compiles the local crash worker, so the fixture's entire 51-case cohort
depends on that worker input closure. Treat all **51** rows as affected; do not
attempt a case-by-case waiver or infer that only the row visibly named for a
worker build is dependent.

Known direct source inputs are:

```text
qa/fixtures/ProfileResetCrashWorker/ProfileResetCrashWorker.csproj
qa/fixtures/ProfileResetCrashWorker/Program.cs
```

The project is a plain SDK `net10.0` project with no project/package references
and an explicitly empty `RestoreSources` property. Generated `bin/` and `obj/` output is not a
source input and must not enter a before/after equality manifest.

Before the follow-up, Main must also enumerate, from the worker directory up
through the repository root, any existing repository-local:

```text
global.json
Directory.Build.props
Directory.Build.targets
Directory.Packages.props
NuGet.Config
nuget.config
```

Record every discovered path and hash; record an explicit empty result when
none apply. Do not capture user-profile NuGet configuration, SDK installation
contents, credentials, license state, package caches, or other private host
data. Record the resolved SDK/version as execution provenance separately from
the repository source closure. This makes the actual local build inputs
auditable without claiming a guessed historical configuration state.

## R35 factual boundary and fresh focused follow-up

R35's 667-file manifest did not contain the worker closure before execution.
Consequently, even if its XML reports 613 zero-failure cases, it remains a
factual execution record but cannot by itself provide a complete fresh
dependency-manifest proof for these 51 rows. Do not alter, delete, or relabel
that record.

After R35 has ended and its XML/exit are inspected, run one fresh worker-only
EditMode follow-up with a new complete before/after manifest containing:

- the 51-fixture test/runtime/asmdef dependency closure;
- the two worker source inputs above;
- every applicable repository-local MSBuild/NuGet/global configuration path;
- the exact selector and sorted expected 51-name manifest.

Use the ancestor-safe positive leaf prefix:

```text
^AcadeGameMaker\.Profile\.Tests\.ProfileResetDiskProcessV1Tests\.
```

The final dot prevents a namespace or fixture ancestor from matching. Require
exactly 51 qualified test-case names, zero failed/skipped/inconclusive, no
missing/extra name, and byte-identical before/after closure paths. The run uses
the fixture's ordinary internal worker behavior only; it does not authorize
the separate PlayMode external-process phases.

## Reconciliation and decision

No full 613-case rerun is required solely for this manifest omission. The
approved regression model permits distinct partitions, so the following fresh
versioned union is sufficient **only if each stated condition is met**:

| Partition | Required condition | Count |
| --- | --- | ---: |
| R35 non-worker rows | Actual R35 XML has the exact current 613-name set; removing the worker fixture leaves 562 names with zero failed/skipped/inconclusive, all covered by R35's frozen before/after manifest. | 562 |
| Follow-up worker-only rows | New exact 51-name XML has zero failed/skipped/inconclusive and its complete fresh closure is unchanged before/after. | 51 |
| Reconciled current partition | Qualified-name sets are disjoint and their union is exact. | **613** |

If R35 has any non-worker failure, missing/extra name, or source-manifest
change, this focused repair cannot close the partition and Astra must decide a
broader rerun. If the 51-case selector or fresh closure is not exact, stop
rather than weakening the criteria.

This correction preserves the zero-failure and independent-evidence gates of
`AC-M5D7QC2-010` and `AC-M5D7QC2R-007/008`. Luna must independently review the
actual R35 split, the follow-up closure/XML, and the 562+51 reconciliation;
Astra alone decides whether it contributes to integration.
