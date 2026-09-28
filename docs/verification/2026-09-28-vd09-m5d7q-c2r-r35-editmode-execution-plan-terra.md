# C2/C2R R35 complete current EditMode partition — Terra plan

Date: 2026-09-28  
Status: planning only; no Unity execution, source/test/spec/asmdef edit, Skip,
NUnit-timeout, or acceptance decision is authorized by this record.

## Purpose and non-reuse boundary

R35 is one future fresh execution of the complete *current* C2/C2R EditMode
dependency partition for `AC-M5D7QC2-010` and
`AC-M5D7QC2R-007/008`. It replaces any proposed final reliance on the old
592-row R23 reuse: the historical 202-file serialized dependency digest cannot
currently be reconstructed independently. Matching four runtime anchors or a
case count is not a substitute for a fresh complete run.

The preserved inputs establish the expected current-name manifest, but do not
make R35 a pass:

| Input | Use | Count |
| --- | --- | ---: |
| `artifacts/c2-r23-final-editmode.xml` | retained passed names after removing five superseded rows | 592 |
| `artifacts/c2-r32-editmode-corrective.xml` | current replacement names | 21 |
| Exact union | future R35 required qualified test-case names | **613** |

The sets are disjoint (`0` shared full names): `592 + 21 = 613`. R23 is
historical `597` total / `595` passed / `2` failed; the five superseded names
are one old aggregate failure and four old Q0 rows (three passed, one failed).
They are not selected in R35 and are not rewritten. The 21 current rows are
17 generated controller cases and four current Q0 audit cases.

## Required complete inventory

R35 must cover all actual current classes in the R23 C2/C2R dependency
partition: all `AcadeGameMaker.Profile.Tests` variants, all
`AcadeGameMaker.Tests.EditMode.Profile` variants, the intended
`AcadeGameMaker.Tests.EditMode.InputUnity` fixtures, and the three
`AcadeGameMaker.Tests.EditMode.HubPresentation` fixtures. The retained R23
inventory has 29 whole-fixture cohorts plus 18 non-superseded controller rows;
R32 contributes 17 controller rows and four Q0 rows.

The 29 whole-fixture cohorts are:

```text
AcadeGameMaker.Profile.Tests.ProfileResetDiskAdversarialV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskCheckpointV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskFaultV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskProcessV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskProofV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskResultV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskSaveFaultV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetDiskTransactionV1Tests
AcadeGameMaker.Profile.Tests.ProfileResetMemoryAuthorityV1Tests
AcadeGameMaker.Tests.EditMode.HubPresentation.HubMenuIntentHandoffEditModeTests
AcadeGameMaker.Tests.EditMode.HubPresentation.HubPresentationAuthoringTests
AcadeGameMaker.Tests.EditMode.HubPresentation.HubPresentationScopeAuditTests
AcadeGameMaker.Tests.EditMode.InputUnity.HubRuntimeAuthoringBuilderEditModeTests
AcadeGameMaker.Tests.EditMode.InputUnity.UiSemanticFrameV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileAtomicSaveServiceV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileBindingApplyFailureRecoveryPlannerV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileBindingOverridesJsonTests
AcadeGameMaker.Tests.EditMode.Profile.ProfileCanonicalDecoderV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileCanonicalEncoderTests
AcadeGameMaker.Tests.EditMode.Profile.ProfileInputRecoveryAtomicSaveServiceV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileInputSnapshotTests
AcadeGameMaker.Tests.EditMode.Profile.ProfileLaunchObservationAdapterV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileLaunchPreservationExecutorV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileLoadSelectorV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileProgressionSnapshotTests
AcadeGameMaker.Tests.EditMode.Profile.ProfileQuarantineServiceV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileRecoveryPlannerV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileResetBarrierInterceptionV1Tests
AcadeGameMaker.Tests.EditMode.Profile.ProfileSettingsTutorialSnapshotsTests
```

`ProfileResetDiskProcessV1Tests` is deliberately included: all its 51
EditMode operation-worker cases execute inside the normal test invocation. It
does not authorize, replace, or manually orchestrate the separate PlayMode
external-process fixture/phases.

## Selector formulation and preflight

Use the same installed Unity Test Framework constraint found for R22:
`-testFilter` means a `groupNames` regex whose NUnit `Pass` can match an
ancestor. Do not use a broad namespace selector or a negative-lookahead
exclusion.

Construct one positive leaf selector from these three branches:

```text
^(?:(?:<Regex.Escape(each of the 29 whole fixture names) joined by |>)\.|AcadeGameMaker\.Tests\.EditMode\.InputUnity\.HubMenuPresentationControllerV1Tests\.(?!AC006_AllControllerAndNestedProofBackingsFailClosed$).*|AcadeGameMaker\.Tests\.EditMode\.InputUnity\.HubUiOnlyQ0ScopeAuditEditModeTests\.)
```

The terminal fixture dot is required. It prevents a namespace or fixture node
from satisfying the positive branch: only a leaf or parameterized-method
descendant can match. The controller branch admits all current controller rows
except the old exact aggregate name, including the 17 generated
`AC006_NestedValueBackings`, `AC006_Ready_*`, `AC006_Pending_*`, and
`AC006_Consumed_*` cases. The Q0 branch admits the four current annotated
methods, whose full names must equal the R32 set. This formulation is safe
under ancestor/descendant `Pass`; the post-discovery name checks below are
mandatory rather than inferred from the regex.

Before the run, Main must generate a fresh sorted expected-name manifest:

1. take the 592 R23 **passed** full names excluding exactly
   `HubMenuPresentationControllerV1Tests.AC006_AllControllerAndNestedProofBackingsFailClosed`
   and all four R23 `HubUiOnlyQ0ScopeAuditEditModeTests` names;
2. union the 21 distinct R32 full names;
3. require cardinality `613`, overlap `0`, no retained superseded name, all
   17 current controller names, all four current Q0 names, and all 51
   `ProfileResetDiskProcessV1Tests` names;
4. record the exact selector and source-before manifest; after execution,
   require the XML's sorted full-name set to equal this manifest with no
   missing or extra name.

This preflight detects both a stale replacement source and any selector-driven
extra cohort before a result is interpreted. It does not claim a discovery-only
Unity feature or manufacture a historical digest.

## Fresh source proof

Main must capture an explicit current complete dependency inventory immediately
before R35 and confirm byte-for-byte path/hash equality immediately after it.
It must include all R35 selected test sources, their metas and asmdefs, and
the actual runtime/shared-helper dependency closure used by those tests. The
new manifest is the R35 proof; it must not be presented as equality to the
unreconstructed historical 202-file digest.

At planning time the following current anchors are fixed and must be present
in that manifest:

| Path | Current SHA-256 |
| --- | --- |
| `Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20` |
| `Runtime/Input/Unity/InputRouter.cs` | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` |
| `Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` |
| `Runtime/Profile/ProfileResetDiskTransactionV1.cs` | `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4` |
| `Tests/EditMode/InputUnity/HubMenuPresentationControllerV1Tests.cs` | `40AA974BF8407A6FD2AAE57830256F04599AF1A835F708999BDC10F332061699` |
| `Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` | `12B98E06919CDC2AFFBD9E380BF2B4546C00D3BA86A8F86E5D9EDF59FFE331FD` |

Paths in this table are relative to `Assets/AcadeGameMaker/`.

## Execution and gates

After Astra approves this plan and Luna independently approves its selector,
name-set, source-inventory, and failure-mode preflight, Main alone may invoke
the verified runner once in EditMode. It must retain normal NUnit settings:
no Skip/Ignore, no timeout change, no test/source/asmdef edit, no manual
external process execution, and no claim based on a pending/missing XML.

Only actual XML containing exactly 613 expected names with zero failed,
skipped, and inconclusive cases, unchanged before/after fresh source manifest,
and Luna's independent post-execution review can contribute to
`AC-M5D7QC2-010` / `AC-M5D7QC2R-007/008`. Astra alone decides integration.
