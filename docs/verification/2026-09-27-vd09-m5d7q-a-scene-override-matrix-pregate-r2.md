# VD09 M5D7Q-A scene override matrix — Q0 semantic-zero correction R2

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Static validator/test/YAML review only. Unity was not run and no authored asset was modified.

## Exact inputs

| Item | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringValidator.cs` | `477420AF499066D311ADB008AF889D9A90691F2E162045F001326439CCC878CA` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubPresentationAuthoringTests.cs` | `AF15506C9AAC4C4C34F3F55AB5AA78315873A6FF05CC6181DE140DFBE2BEE3DB` |
| `Assets/Scenes/Hub.unity` | `04F3F0EA73C40A964DCE93766CF21AFE3F57DC20912AEAB86ABDADB239BC9C6A` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs` | `6912C422B27819B68C227A84A29BF58D9ADFAE8261D9DD8503F69320F90D004D` |

## Q0 semantic-zero matrix

The validator now treats the Unity-serialized default rows on the Q0 runtime prefab instance as an exact semantic-zero matrix, rather than incorrectly requiring an empty modification array. The allowed matrix is exactly 11 rows:

- source `HubRuntimeRoot` GameObject target, `m_Name=HubRuntimeRoot`, scalar value exact, null object reference;
- source root `Transform` target, `m_LocalPosition.x/y/z=0`;
- source root `Transform` target, `m_LocalRotation.w=1` and `.x/.y/.z=0`;
- source root `Transform` target, `m_LocalEulerAnglesHint.x/y/z=0`.

All ten transform rows have exact source target identity and null object references. The current `Hub.unity` YAML contains these exact 11 rows under the runtime prefab instance (`fileID 679987025`): GameObject source `fileID 1001`, Transform source `fileID 1002`, runtime prefab GUID `5be16abbe0e5e4e418b27c5d053345a1`, and no object-reference values.

`ValidateExactModificationMatrix` requires actual count equal to 11 and every expected row to match exactly once by target identity, property path, scalar value, and object-reference identity. Missing, duplicate, extra, wrong-target, wrong-path, wrong-value, and wrong-reference entries therefore fail closed; no wildcard or broad exemption exists.

## Negative coverage and retained menu matrix

The new Q0 EditMode table covers seven mutations: missing, duplicate, wrong target, wrong path, wrong value, wrong object reference, and extra row. The canonical Q0 matrix must first pass before each mutation is applied.

The existing HubMenuRoot matrix remains independently exact: two presenter seam rows, 19 root-`RectTransform` zero rows, root rotation `w=1`, and root name (`23` rows total), with the prior missing/duplicate/wrong/extra mutation table retained. The validator still requires exact prefab source paths and exact Q0 `InputRouter`/`HubEntryHandoffLatchV1` object references.

The label migration transaction is unchanged (builder SHA above): `BeforeSave`, `AfterSave`, `AfterRefresh`, `AfterReloadValidation`, `BeforeSceneValidation`, and `AfterSceneValidation` injectors remain covered with exact prefab/scene/meta byte and GUID restoration.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The Q0 semantic-zero correction is statically precise and closes the prior false empty-array assumption without relaxing the matrix or changing the menu/label transaction. This is a static pre-gate only; Unity compilation and execution remain the next independent gate.

