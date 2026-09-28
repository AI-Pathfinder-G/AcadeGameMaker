# VD09 M5D7Q-A TMP label-layout correction — implementation pre-gate R4

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static/compile-only recheck)
Scope: Recheck only the CS0104 `Object` ambiguity fix reported by `builder-pass-f` before `executeMethod`. No Unity invocation, asset mutation, or runtime claim was made.

## Inputs and exact hashes

| Item | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubPresentationAuthoringTests.cs` (current) | `71087281070DF4F4EF2C7251E4A1FDCFFB593FBF9B47050511B1D2741369BD09` |
| Same test file (prior R3 PASS) | `9574A3D60E026E337FA21B152EDF4FE927C3127FC08531BB41FD91127EA3780A` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs` (unchanged builder) | `6912C422B27819B68C227A84A29BF58D9ADFAE8261D9DD8503F69320F90D004D` |

## Narrow change check

The current test source explicitly qualifies the four Unity object operations implicated by CS0104:

- `UnityEngine.Object.Instantiate(...)` in the independent label-mutation test (line 71).
- `UnityEngine.Object.DestroyImmediate(clone)` in that test's `finally` block (line 78).
- `UnityEngine.Object.Instantiate(...)` in the complete legacy-profile test (line 85).
- `UnityEngine.Object.DestroyImmediate(clone)` in that test's `finally` block (line 93).

The file also imports both `System` and `UnityEngine`; explicit qualification removes the `System.Object`/`UnityEngine.Object` ambiguity without changing clone, validation, cleanup, mutation, or assertion semantics. No bare `Object` remains at these operation sites. The builder hash is unchanged from R3.

## Retained R3 gates

The prior R3 implementation gates remain in scope and are unchanged by this compatibility-only amendment: exact prefab/scene/meta rollback and hash/GUID proof; one-save/no-broad-save migration boundary; all six failure-injection stages including `BeforeSceneValidation` and `AfterSceneValidation`; validator and generator coverage; and REQ/AC traceability for `REQ-M5D7QA-009` and `AC-M5D7QA-002/010`.

## Verdict

**PASS — P0=0, P1=0, P2=0 for this narrow compile-only recheck.**

This result authorizes the compile-fix delta to proceed to the next build/Unity execution gate. It is not evidence that Unity compilation, `builder-pass-f`, runtime migration, or final acceptance has succeeded; those require the independent build/runtime gate.

