# C2R final startup/runtime pre-gate — Luna review

Date: 2026-09-28  
Scope: frozen source/static review before Main execution. No Unity execution,
source edits, or acceptance by Luna.

## Frozen hashes

| Input | SHA-256 |
|---|---|
| `DesktopProfileLaunchAdapterV1.cs` | `AE1964A609E51FB67C0483241477D15DE8C8248B1CAD50588AC63CFF7D09F602` |
| `InputRouter.cs` | `DCB078169A6AE63F97EBDDF6E68F9601519953F4481C4053996A87177243F2E5` |
| `ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` |
| `DesktopProfileLaunchAdapterV1Tests.cs` | `63792A73E0EC3DD16BCF7AE4A72D40ECC3471E864155B9896788A9EB97E6C9B6` |
| `ProfileResetRestartBootstrapV1Tests.cs` | `6EFF2E1BD33E0CC9A6C622452090C039A13AB3582945789898BB6961CC84CAFF` |
| `ProfileResetRestartProcessV1Tests.cs` | `BD690FE231E13E1785332F514896FB077069B7043C5854F92A1D674EFA531B66` |
| `qa/fixtures/C2RRestart/README.md` | `4BED15CBDFCEAAC9A496921A2ECDA58E36344D621B14127FCDA0FE9C9BE7818C` |

## Result

Contract/runtime pre-gate P0: **0**. Implementation defects P1: **0**;
evidence-gap P1: **1** (direct promotion rows do not traverse production Awake).

The reviewed correction now captures the environment root outside the
classification-only catch, so all root-getter exception types retain the
historical failure path while invalid values and post-return filesystem
classification take private ManualRepair. The closed unknown-root repair case
is narrowly recognized through the private recovery witness/result, preserving
quiescent teardown without broadly accepting reflected or corrupt `None` state.

Focused source rows cover all four getter exception types, direct promotion
before/after router Awake, foreign/duplicate/corrupt promotion, actionless
ownership, and actual generation reflection. The process fixture seeds legal
non-default progression, archives and verifies the old bytes, then asserts
exact r0 bytes, current-cell generation and action agreement, barrier removal,
disabled maps, and one receipt.

The required runner treatment remains sound: the four process entries are
excluded from the normal non-process regression filter and executed separately
by Main, with no Skip/Ignore/new assembly or weakened environment guard.

One evidence limitation remains P1 for AC-001/002: the direct promotion rows
exercise the real router transition, but construct a pristine harness and call
the internal promotion method directly; they do not obtain the adapter's
private recovery CWT through an actual Awake barrier-selection path. They are
valid transition-unit coverage, not proof of the complete production mint.
Main's actual focused execution must provide that missing production-path
evidence. This is a Luna pre-gate record only; Astra acceptance remains
pending.

## R11 compiler correction (append-only update)

R11 Unity compilation found a real P0 compile blocker: the nullable
`TargetByteLength` was assigned directly to the witness `long` field at the
recovery mint boundary. No tests ran; the editor exited with failure. Terra's
narrow adapter-only correction now validates `HasValue` and `> 0`, then assigns
`.Value` exactly once before witness mutation and one-shot mint:

`DesktopProfileLaunchAdapterV1.cs:326–329`

- pre-correction adapter SHA: `AE1964A609E51FB67C0483241477D15DE8C8248B1CAD50588AC63CFF7D09F602`
- corrected adapter SHA: `AE31A8AA54B6A2466F3B275632B9A6BF275D44CCEDF6F547BCDFEB14E2A03FAC`

This closes the identified compile defect by source inspection, but is not a
compiler-pass claim. R12 must provide fresh compile and focused execution
evidence. The production Awake/private-CWT evidence P1 remains open until that
actual path is exercised; the earlier source-only hashes and conclusions are
preserved above.
