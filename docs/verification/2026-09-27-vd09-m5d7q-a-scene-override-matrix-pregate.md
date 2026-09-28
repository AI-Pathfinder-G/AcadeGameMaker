# VD09 M5D7Q-A scene override matrix — Luna static pre-gate

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Validator and focused test review only. Unity was not run and no authored asset was modified.

## Exact inputs

| Item | SHA-256 |
|---|---|
| `HubPresentationAuthoringValidator.cs` | `BD8E8B8F4110183D30B2E8A0B1AFFBBEBA9FCA9615C267036D2BE78676E17CF2` |
| `HubPresentationAuthoringTests.cs` | `E300488598703666847DA8019EC070860265C90861989801C2B0AC5D8189BCF3` |
| `HubMenuResolutionPlayModeTests.cs` | `9BA028A069D656A36594E12F2D33FB3C0A18D368528BB59AE0EADE4EF893F66C` |
| `HubPresentationAuthoringBuilder.cs` (label migration transaction) | `6912C422B27819B68C227A84A29BF58D9ADFAE8261D9DD8503F69320F90D004D` |
| `Assets/Scenes/Hub.unity` | `04F3F0EA73C40A964DCE93766CF21AFE3F57DC20912AEAB86ABDADB239BC9C6A` |
| Approved work contract `2026-09-23-vd09-m5d7q-a-authored-hub-shell.md` | `AEBBF84F8EB08FC345894BDFF09816428915543488C165E66AD12DB37E514456` |

## Matrix verification

`ValidateSceneInstanceOverrideMatrix` admits exactly 23 `HubMenuRoot` property-modification rows:

- two presenter rows targeting the prefab-source `HubMenuPresenterV1` (`_latch` and `_router`), with empty scalar values and object references equal to the exact Q0 scene-instance `HubEntryHandoffLatchV1` and `InputRouter` components;
- nineteen `RectTransform` rows targeting the prefab-source root `RectTransform`, all with null object references and value `0`: `m_Pivot.x/y`, `m_AnchorMax.x/y`, `m_AnchorMin.x/y`, `m_SizeDelta.x/y`, `m_LocalPosition.x/y/z`, `m_LocalRotation.x/y/z`, `m_AnchoredPosition.x/y`, and `m_LocalEulerAnglesHint.x/y/z`;
- one additional source-root `RectTransform` `m_LocalRotation.w=1` row with a null object reference;
- one source-root `HubMenuRoot` GameObject `m_Name=HubMenuRoot` row with a null object reference.

The validator requires the actual modification count to equal 23 and requires every expected row to match exactly once by target identity, property path, scalar value, and object-reference identity. Therefore missing, duplicate, extra, foreign-target, wrong-path, wrong-value, and wrong-reference rows fail closed; there is no wildcard or broad exemption. The source Q0 `HubRuntimeRoot` instance is independently required to have zero property modifications.

The authored scene YAML independently shows the same 23 rows: two exact Q0 links, the nineteen zero-valued root-transform paths, `m_LocalRotation.w=1`, and the root name. The Q0 prefab source is not modified.

## Negative and regression coverage

The EditMode test table exercises 13 independent mutations: missing/wrong/duplicate router, missing/wrong/duplicate latch, wrong target, wrong path, wrong presenter value, extra root transform, wrong root-transform value, wrong root name, and wrong root object reference. A canonical scene is first required to pass the matrix, then each mutation must throw and the original modification set is restored.

The PlayMode resolution test checks the actual prefab and scene instance, including the current four Korean menu labels, centered local geometry, overflow/character-count assertions, and exact scene prefab identity. The existing builder transaction remains unchanged: six injected failure points cover `BeforeSave`, `AfterSave`, `AfterRefresh`, `AfterReloadValidation`, `BeforeSceneValidation`, and `AfterSceneValidation`, with exact prefab/scene/meta byte and GUID restoration.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The scene-override correction is statically bounded and fail-closed. This is a static pre-gate only; it does not claim Unity compilation or test execution success. The next independent build/runtime run may proceed.

