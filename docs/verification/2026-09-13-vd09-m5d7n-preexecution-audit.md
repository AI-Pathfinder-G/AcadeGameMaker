# VD-09 M5D7N pre-execution independent audit

- Reviewer: Luna
- Date: 2026-09-13
- Scope: frozen M5D7N runtime/test source and existing execution diagnostics
- Verdict: static pre-execution gate PASS (`P0=0`, `P1=0`)
- This is not a runtime acceptance or contract Verified decision. AC-M5D7N-009 remains pending executable Unity evidence.

## Identity and deterministic checks

| Artifact | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` | `70C60E94EC561E09E2E7B64ABA820DA2A3F78EC4F61309F2039ABB28985BCC21` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs.meta` | `ED26E027C7129E2D53F7D588841BAA77D20CD7829043CBF703995869018F57FA` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubEntryHandoffLatchV1Tests.cs` | `C741B34479DE15FC037319B24A8DF5B7A39900538DA48A21F73E348C72913A79` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubEntryHandoffLatchV1Tests.cs.meta` | `BF4F7413B8BB5C5D5FCF776A1D98CDA07120DAEAC7FDB6208A4490A2840BF73C` |
| unchanged M5D7M `InputRouter.cs` | `1ABA6873EDD3F90050B07B57A61F609F22A98EDCB629269A62E2E8D8D2F84E69` |
| unchanged M5D7M `DesktopProfileLaunchAdapterV1.cs` | `C1B27EE416315666FA2E7673433735CC78B2DA410D5438D98B48C947075E5C0F` |
| unchanged M5D7M `AcadeGameMaker.Input.Unity.asmdef` | `CD3E47DD94BCCB9E537CA7AF4C6029FC8327EA0154C90EC7B6FD305218FE4C0E` |

The static source scan found the required immutable receipt/notification references and none of the forbidden scene, UI, router-mode, coordinator, persistence, file/IO, logging, network, RNG, or action-authority markers. The test source is syntactically plausible by inspection: the scenario parameter is internal, the authored fixture uses existing friend/assembly boundaries, and the new cases use existing Unity/NUnit APIs.

## Acceptance readiness

| Criterion | Static result | Independent basis |
|---|---|---|
| AC-M5D7N-001 | Ready | Nine real coordinator scenarios cover clean primary, default, previous, decode repair, binding repair, and pending outcomes; handoff/receipt validation remains fail-closed. |
| AC-M5D7N-002 | Ready | Preservation denial, save failure, commit uncertainty, and preservation-only rows assert accepted handoff and pending/preservation evidence. |
| AC-M5D7N-003 | Ready | Expected-present/none, prior-consumed, unexpected, kind, and correlation adversaries exercise single source take and terminal no-retry behavior. |
| AC-M5D7N-004 | Ready | Four authored creation orders cover same/separate router placement and both component creation orders; state converges at first Update. |
| AC-M5D7N-005 | Ready | Invalid-entry rows include actual adapter-bound inactive router, foreign references, disabled cohort, faulted/non-UI router, missing/malformed receipt, and reflected illegal state. Hostile reflection fields are restored before teardown. |
| AC-M5D7N-006 | Ready | Before/after publication Disable/Destroy rows preserve the specified close/retract behavior. |
| AC-M5D7N-007 | Ready | Reactivation uses actual `enabled=false/true` dispatch and duplicate Update/take assertions. |
| AC-M5D7N-008 | Ready | Receipt/state/history/correlation reflection cases and forbidden-authority static scan are present. |
| AC-M5D7N-009 | Pending | No XML was produced; executable focused/full Unity results are still required. |

## Findings

No P0 or P1 finding remains in the frozen source/test snapshot.

Residual P2: the deterministic scenario assertions emphasize the externally material handoff fields and rely on the nested receipt `Validate`/proof equality for some less-visible projection fields; a future strengthening pass could compare every nested save/binding field against the scenario oracle directly. This does not block the static gate because the production handoff copies and revalidates the complete M5D7M receipt.

The only execution diagnostic is infrastructure-related: `artifacts/unity-results/m5d7n-focused.log` contains Unity 6000.6.0f1 Licensing Client channel loss/timeouts and no XML result. It is not evidence of a M5D7N code failure, and no repeated license attempt was made.
