# VD-09 M5D7P-B Unity UI package baseline — implementation evidence

- Date: 2026-09-20
- Implementer: Terra (`gpt-5.6-terra`)
- Contract: `docs/specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md`
- Status: implementation complete; Luna independent verification **PASS**
  (`P0=0`, `P1=0`, `P2=0`) and Astra integration accepted. The contract is
  `Verified`.

## Scope and preserved baseline

Before implementation, Terra captured the two package files at `2026-09-20
00:36:42` by `Copy-Item -LiteralPath` (one source file per destination) into
the outside-workspace snapshot directory
`C:\Users\me\AppData\Local\Temp\acade-m5d7p-b-20260920`. The retained
originals are `manifest.json.baseline` and `packages-lock.json.baseline`; their
SHA-256 values were then recomputed before the manifest patch and Unity
resolver run:

| File | Frozen SHA-256 |
|---|---|
| `Packages/manifest.json` | `7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25` |
| `Packages/packages-lock.json` | `C0DA832620B8DB22C1E607D3E2638680C29186A871F55FA6FA5B453DD27432A9` |

The pre-existing broad lockfile difference from Git HEAD was preserved as the
approved working-tree baseline. No Git restoration was used.

The final semantic delta from that frozen baseline is exactly:

- manifest: add direct `com.unity.ugui: 2.6.0`; no TMP shim entry;
- lock: add `com.unity.ugui` (`2.6.0`, `builtin`, depth `0`) with exactly
  `ui`, `imgui`, `audio`, `physics2d`, and `physics` module dependencies;
- lock: add built-in `com.unity.modules.audio` at depth `1` and change only
  existing `com.unity.modules.ui` and `com.unity.modules.physics` from depth
  `2` to `1`.

No other lock entry changed relative to the frozen baseline. No runtime, test,
scene, prefab, asset, ProjectSettings, generated input, or unrelated manifest
dependency was changed by this unit.

## Resolver and TMP registration smoke

Unity `6000.6.0f1` ran in batch mode after the manifest change. The fresh log
records package resolution, lockfile modification, and registration of
`com.unity.ugui@2.6.0` from the installed built-in package cache. It reports no
package-resolution error, version substitution, registry override, or
`com.unity.textmeshpro` shim.

The same fresh compilation registered and compiled the auto-referenced
`Unity.TextMeshPro` and `Unity.TextMeshPro.Editor` assemblies, including their
uGUI package `.asmdef` imports. Installed uGUI metadata is version `2.6.0` and
declares the exact five modules above; its `Runtime/TMP/Unity.TextMeshPro.asmdef`
is auto-referenced and its source declares `namespace TMPro`.

| Artifact | SHA-256 |
|---|---|
| `artifacts/unity-results/m5d7p-b-resolution-smoke.log` | `7A9B151C77632B4CFAF196FD157BAEEC6ED0015A0F1E000DFA4EC1BE9155ADE4` |

## Regression evidence

| Run | Result | Evidence |
|---|---|---|
| R01 full EditMode | 688/688 passed; failed/skipped/inconclusive `0/0/0`; duration `875.9722615s` | XML `503E27174523960D5EAE7423F7C3C0D0B6971385D34010DDB08981FCC5453B9E`; log `2F8D05AE37D03A9B16615FA1EBF80152C025BF05E300020AEB5FA5E09E4B1C5C` |
| R04 final full PlayMode | 805/805 passed; failed/skipped/inconclusive `0/0/0`; duration `3303.2216857s` | XML `EBAD69083000D2C67299D0C87A985CD60856460377E3EE1071BDA72B25159CCD`; log `5FE051B4DC63914FB2FCBD3B472CC269D9C0D1AB9A52091CD399EC8A3FE007A0` |

R04 contains the final M5D7P-A source and is also reusable as that unit's full
PlayMode evidence. Its test-run has `failed=0`, `skipped=0`, and
`inconclusive=0`.

### Non-acceptance history

- R02 produced no XML because an unrelated concurrently-added M5D7P-A test
  assembly lacked its `AcadeGameMaker.Core` reference. It was compile-blocked
  and is not acceptance evidence.
- R03 completed at 799/805 with six failures from legacy input/terminal tests
  interacting with the new M5D7P-A UI-proof validation. It is preserved at
  `artifacts/unity-results/m5d7p-b-r03-full-playmode.xml` and is not acceptance
  evidence. The later approved regression amendment and its focused checks
  preceded clean R04.

## Final package hashes

| File | SHA-256 |
|---|---|
| `Packages/manifest.json` | `AD3A5D2329D5A8FD71BE89873C75BA2CAA98B4391558A69D83D81CADFE684E1E` |
| `Packages/packages-lock.json` | `B2B9B1DE4381F3C289837ACD1BDE69013C1F7581511E26861DFDF4F5AE00E550` |

## Acceptance mapping

| Criterion | Terra evidence | Result |
|---|---|---|
| `AC-M5D7PB-001` | Final manifest hash and semantic delta show exactly one direct `com.unity.ugui: 2.6.0` and no `com.unity.textmeshpro`. | Pass |
| `AC-M5D7PB-002` | Final lock hash and semantic delta show uGUI `2.6.0`/`builtin`/depth `0`, its exact five declared dependencies, the permitted audio addition and depth changes, and no shim. | Pass |
| `AC-M5D7PB-003` | Fresh smoke log registers installed built-in uGUI 2.6.0 and compiles auto-referenced `Unity.TextMeshPro` assemblies; installed metadata/source establishes `TMPro`. | Pass |
| `AC-M5D7PB-004` | Fresh package smoke, R01 full EditMode, and R04 full PlayMode all have zero failure/skip/inconclusive; semantic comparison is limited to the approved package graph. Luna P0/P1 review remains pending. | Terra evidence complete; Luna pending |

## GPT participation ledger

| Role | Model | Bounded work | Evidence |
|---|---|---|---|
| Implementation | Terra (`gpt-5.6-terra`) | Package declaration, Unity resolver run, semantic delta inspection, regression execution and evidence assembly. | This document and listed artifacts. |
| Independent verification | Luna (`gpt-5.6-luna`) | Pending; must independently audit AC-M5D7PB-001 through `004` and report P0/P1. | Pending post-review. |
