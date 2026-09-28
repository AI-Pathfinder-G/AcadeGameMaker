# Costume CUA — Luna compile-blocker fix pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Static-only review of the compile-blocker correction. Unity was not run and no implementation file was modified by this review. The prior compiler-failure log remains immutable evidence.

## Inputs and hashes

- Failure log SHA-256: `2E48CCACE390FE16B3F327497D6BA1DD6133071E3A04583CD9CEE8AC3E1AE14A`
- Corrected media source `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs`: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
- Corrected adapter tests `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs`: `9C1C28CBC650E955BE64FFC0778F7BDB6129F56BA762D48049F0D6D47435CDF9`
- All other implementation/test hashes are unchanged from the aggregate static gate `1AE41A6DC119BDFBB027D46EF146FBEEEC47B84D8BC887E4A34E958DD66C18D5`.

## Findings

1. The preserved failure is a single CS0103 at `CostumeUnityMediaPackageV1.cs:31`: `CostumeUnityClipV1` called `CheckId` before that class defined the helper. The corrected source now defines `private static void CheckId(string value) { if (String.IsNullOrEmpty(value)) throw new ArgumentException(); }` inside `CostumeUnityClipV1`.
2. This helper is private, static, and byte-for-byte semantically identical to the existing sibling Unity-package helper in `CostumeUnityMediaPackageV1` (the package-level `CheckId` at line 133). It does not widen visibility, alter public API, introduce state, or change the package validation path. It is intentionally not a replacement for the stricter domain-level canonical-ID validator; no domain authority is exposed or bypassed.
3. The corrected file has the required `System` import, and the declaration is valid C# 9 lexical scope for the constructor call. The method's `String.IsNullOrEmpty` and `ArgumentException` references resolve without new dependencies.
4. The adapter test source directly exercises the regression boundary: `AC_CUA_002_InvalidSyntheticClipGeometryCannotBecomeAValidFrame` asserts `ArgumentException` for both `new CostumeUnityClipV1(null, ...)` and `new CostumeUnityClipV1(String.Empty, ...)`, in addition to the invalid-facing case. Existing null/empty clip collection coverage and the complete R3/R4/R5/R6 suites remain present in the corrected test source, including durable replacement, uncertain-primary, projection-fault, current-tuple, mechanics-isolation, and `7 actions × 2 facings × 4 ages` checks.
5. No evidence supports a regression to the previously closed R3 findings. This is a compile-only helper correction plus direct null/empty action regression coverage; it does not change adapter production code or the closed persistence/mechanics behavior.

## Verdict

**PASS — P0=0, P1=0, P2=0 for this narrow static gate.**

The corrected implementation may proceed to a fresh Unity compile/test attempt. The prior R7 stem consumed by the failed compile is not reusable: the retry must publish to a new, explicitly authorized fresh stem and preserve the failure log above. This PASS is not a Unity execution/result PASS.

