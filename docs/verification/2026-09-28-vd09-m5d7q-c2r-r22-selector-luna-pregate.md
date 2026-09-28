# C2/C2R R22 selector diagnostic — Luna independent pre-gate

Date: 2026-09-28  
Scope: read-only independent replay of Terra's selector diagnostic against the
preserved R22 NUnit XML and installed Unity Test Framework source. No Unity,
CIM, licensing, source, test, filter, timeout, or specification changes.

## Inputs

- Discovery XML: `artifacts/c2-r22-final-playmode.xml`
- Discovery XML SHA-256:
  `43D4FE3ABDC861E7CE70F0B2E6355D3AD801C47A7453F5901EFC905C9EA08FFB`
- Astra/Main-generated preflight (using Terra's 23-fixture proposal):
  `artifacts/c2-r34-selection-preflight.json`
- Frozen C2/C2R source dependency digest retained by the R22 pause record:
  `7D712A7326C121AE2DA439E16368968A7A3A716E7CAF1E0E444E18FC4C9CBD1E`

The four frozen runtime anchors independently match their recorded hashes:
`DesktopProfileLaunchAdapterV1.cs` `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`,
`InputRouter.cs` `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`,
`ProfileResetMemoryCutoverV1.cs` `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`,
and `ProfileResetDiskTransactionV1.cs`
`168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`.
The historical 202-file serialization digest was not independently
reconstructed from the available current inventory; therefore this pre-gate
does not claim historical whole-inventory equality. A fresh explicit manifest
must accompany R34 and any later 592-row reuse decision.

The local implementation paths independently inspected were the installed
`SettingsBuilder.cs`, `RuntimeTestRunnerFilter.cs`, `FullNameFilter.cs`, and
`CompositeWorkItem.cs`. They confirm that `-testFilter` populates
`groupNames`, that group names become regex full-name filters, and that child
work items are admitted through NUnit `Pass` while traversing suites. A
namespace-level positive match therefore admits excluded fixture descendants;
negative lookaheads do not repair that ancestor match.

## Exact replay

I extracted the 23 fixture names from Terra's diagnostic, generated the
`^(?:<Regex.Escape(each)>...)\.` selector, and applied it to every qualified
leaf in the frozen XML. The resulting selector is 1,889 characters. Independent
results:

| Check | Result |
| --- | ---: |
| XML qualified leaves | 614 |
| Positive selected leaves | 536 |
| Selected fixture cohorts | 23 |
| `AcadeGameMaker.Tests.PlayMode.InputUnity` actual/selected | 329 / 251 |
| `AcadeGameMaker.Input.Unity.PlayMode.Tests` actual/selected | 277 / 277 |
| `AcadeGameMaker.Tests.PlayMode.HubPresentation` actual/selected | 8 / 8 |
| Selected R25 matrix cases | 0 |
| Selected external-process cases | 0 |
| Reconciled historical baseline | 446; missing 0 |

The generated selector is exactly equal to the JSON `Selector`; sorted
qualified selected names are equal with `536` local and `536` JSON entries,
with zero missing and zero extra names. The JSON discovery hash equals the
verified XML hash. The arithmetic is therefore `614 - 74 - 4 = 536`, with
the corrected InputUnity target `329 - 74 - 4 = 251`.

The terminal dot is required. It prevents a fixture node itself from matching,
so only method/parameterized descendants match; an excluded fixture has no
matching self, parent, or descendant under this positive leaf-prefix selector.

## Verdict and safe next step

**Luna pre-gate: PASS, P0=0, P1=0** for selector construction and frozen-XML
replay under `AC-M5D7QC2-010` and `AC-M5D7QC2R-007/008`'s partition boundary.

The exact selector is safe to use for one unchanged normal R34 PlayMode
invocation, with the runner's ordinary observation bound only (proposed
`AwaitSeconds 10800` is a wait bound, not a NUnit test timeout). Before
accepting any R34 result, inspect its full qualified-name set against this
536-name preflight list and require zero failed, skipped, and inconclusive
rows. Persist the selector and sorted list beside that run.

This pre-gate does not execute or accept R34, does not reclassify R22's
`614/610/4` failure, and does not close C2/C2R or any live product/UI gate.
R25 remains the sole owner of the 74 matrix cases; the four external-process
cases remain outside R34 and require their separately authorized phases.
