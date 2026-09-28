# C2/C2R R22 selector diagnostic — Terra

Date: 2026-09-28  
Scope: read-only local diagnosis of the completed R22 XML and the installed
Unity Test Framework implementation. No Unity invocation, source/spec/asmdef
change, timeout/Skip change, or acceptance decision was made here.

## Result

R22's command-line `-testFilter` is mapped to `Filter.groupNames`, not to an
exact leaf-name filter. The logged expression was:

```text
^(?!.*ProfileResetRestartProcessV1Tests)(?!.*ProfileResetMemoryCutoverFaultMatrixV1Tests)(AcadeGameMaker\.Tests\.PlayMode\.InputUnity|AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests|AcadeGameMaker\.Tests\.PlayMode\.HubPresentation)
```

It does not safely exclude a fixture beneath a matched namespace. The actual
NUnit `TestFilter.Pass` algorithm accepts a node when it matches itself, a
parent, or a descendant. Both excluded fixtures fail the expression themselves,
but their parent `AcadeGameMaker.Tests.PlayMode.InputUnity` matches it. Unity
then calls `_childFilter.Pass(test)` while recursively creating work items. The
parent match therefore admits the fixture and its children. This explains the
actual XML, not merely PowerShell-regex behavior.

Local implementation basis:

- `SettingsBuilder.cs:41-70` registers `-testFilter` and assigns it only to
  `groupNames`.
- `RuntimeTestRunnerFilter.cs:24-31, 46-65` turns `groupNames` into a regex
  `FullNameFilter`; `FullNameFilter.cs:12-15` evaluates a node's full name.
- The installed `nunit.framework.dll` `TestFilter.Pass` contains the
  `Match(test) || MatchParent(test) || MatchDescendant(test)` path; Unity's
  `CompositeWorkItem.cs:228-248` uses `Pass` for every suite child.

## Actual inventory and required R22 target

Source artifact: `artifacts/c2-r22-final-playmode.xml`, SHA-256
`43D4FE3ABDC861E7CE70F0B2E6355D3AD801C47A7453F5901EFC905C9EA08FFB`.

| Set | Qualified cases |
| --- | ---: |
| Actual R22 | 614 = 610 passed + 4 failed |
| R25-owned `ProfileResetMemoryCutoverFaultMatrixV1Tests` | 74 |
| External `ProfileResetRestartProcessV1Tests` | 4 |
| Required R22 positive remainder | **536** |
| Historical baseline retained by that remainder | **446** |
| Current non-matrix additions | **90** |

All 614 cases are in the three R22 namespace surfaces. The **actual** split is
`InputUnity=329`, `Input.Unity.PlayMode.Tests=277`, and `HubPresentation=8`.
InputUnity contains both excluded fixtures, so the **target** split is
`InputUnity=251` after subtracting its 74 matrix and four external-process
cases, with `Input.Unity.PlayMode.Tests=277` and `HubPresentation=8` unchanged.
The 536-case target is therefore exactly `614 - 74 - 4`, preserves the
reconciled 446 historical names, and leaves R25 as sole owner of all 74 matrix
names.

This is the necessary selector correction for the zero-failure gate in
`AC-M5D7QC2-010` and the static/execution boundary in
`AC-M5D7QC2R-007` / `AC-M5D7QC2R-008`; it is not proof that those ACs pass.

## Safe command-line selection

Use one *positive leaf-prefix* `-testFilter`, constructed from exactly these
23 fixture full names, with `^` at the beginning and `\.` immediately after
the alternation:

```text
AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests
AcadeGameMaker.Input.Unity.PlayMode.Tests.HubEntryHandoffLatchV1Tests
AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyInputRouterPlayModeTests
AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0CoverageBPlayModeTests
AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0HandoffMatrixPlayModeTests
AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0RemainingRuntimePlayModeTests
AcadeGameMaker.Tests.PlayMode.HubPresentation.HubMenuFailureTeardownPlayModeTests
AcadeGameMaker.Tests.PlayMode.HubPresentation.HubMenuIntentHandoffPlayModeTests
AcadeGameMaker.Tests.PlayMode.HubPresentation.HubMenuPresenterPlayModeTests
AcadeGameMaker.Tests.PlayMode.HubPresentation.HubMenuResolutionPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterActualCameraPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterAdversarialPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterAimPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterFailureBoundaryPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterTerminalPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.OrdanBossTerminalTransitionRequesterPlayModeTests
AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileBindingOverrideApplyAdapterV1Tests
AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileLaunchPreparationCoordinatorV1Tests
AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetMemoryCutoverDataV1Tests
AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetMemoryCutoverV1Tests
AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartBootstrapV1Tests
AcadeGameMaker.Tests.PlayMode.InputUnity.UiSemanticFrameRouterPlayModeTests
```

Form the selector as `^(?:<Regex.Escape(each full fixture name) joined by |>)\.`.
For the frozen R22 inventory the resulting selector is 1,889 characters.
The final dot is essential: it prevents a fixture node itself from matching.
Only its method/parameterized descendants match; an excluded fixture has no
matching self, parent, or descendant, so `TestFilter.Pass` cannot admit it.

Local replay against every R22 XML test-case full name gives:

```text
positive leaf matches: 536
selected from all 614: 536
selected matrix cases: 0
selected external-process cases: 0
```

This is robust to the installed ancestor/descendant `Pass` semantics, unlike a
namespace inclusion plus negative lookaheads. It should be regenerated from a
fresh discovery inventory if the frozen source fingerprint changes; do not
silently add fixtures.

## CLI capability finding and cheap preflight

The installed command-line parser supports `-testFilter` / `-editorTestsFilter`
and `-assemblyNames`; it has no command-line `-testNames` or discovery/list
option. `Filter.testNames` exists for direct `TestRunnerApi` use but is not set
by `SettingsBuilder`. `-orderedTestListFile` controls ordering, not selection.
`-assemblyNames` is an AND filter and cannot remove the two fixtures from the
shared `AcadeGameMaker.Input.Unity.PlayMode.Tests` assembly, so it is not a
safe R22 partition mechanism by itself.

Before a long Unity regression, Main can perform the following no-Unity,
read-only preflight using the preserved XML as the discovery corpus:

1. Build the 23-fixture positive selector above using `Regex.Escape`.
2. Apply the installed `Pass` rule to the XML hierarchy, not only to leaf
   strings: match self, walk parents, and for suites test descendants.
3. Require exactly 536 selected leaves, 23 selected fixture cohorts, 0 matrix,
   0 process, and inclusion of the already reconciled 446 historical full
   names. Persist the generated selector and selected sorted full-name list
   beside the next run's evidence.
4. Only if those invariants hold, run the unchanged normal PlayMode command
   with that one selector. Inspect the new XML's qualified-name set against the
   preflight list before claiming a result.

There is no CLI discovery-only run to substitute for step 2. A direct API
enumerator could provide an additional discovery check, but it would require a
separately authorized editor utility; it is unnecessary while the unchanged
R22 XML remains the frozen discovery corpus.

## Authority

This is Terra implementation/support diagnostic evidence only. It does not
accept the R22 result or C2/C2R. Luna must independently verify the next actual
partition and Astra alone may integrate it.
