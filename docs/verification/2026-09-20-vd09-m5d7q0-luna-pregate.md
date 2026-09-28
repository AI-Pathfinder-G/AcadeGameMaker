# VD-09 M5D7Q0 Hub-UIOnly router graph — Luna pre-gate

- Review date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`)
- Reviewed artifacts: the M5D7Q0 work contract and Sol design proposal
- Review type: independent contract/design review; no implementation tests run
- Result: **PASS — P0=0, P1=0, P2=0**

## Review outcome

The amended contract closes all five P1 and all three P2 findings from Luna's
first pre-gate. No new blocking or residual design finding was identified.
The bounded HubUIOnly variant, ownership lifecycle, proof boundary, prefab
authoring/validation boundary, fault preservation, and acceptance coverage are
now sufficiently specific for Astra's contract decision.

| Prior finding | Closure in amended contract/proposal |
|---|---|
| M5D7M full-graph requirement conflicted with the proposed lightweight hub variant | The exception is restricted to the exact canonical `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`, its explicit flag, and a passing validator. `REQ-M5D7Q0-007` preserves all other M5D7M objects, FullSimulation variants, and historical evidence. |
| Post-publication fault closure conflicted with M5D7M's prepared-state rows | The contract keeps the router in the existing faulted `Initialized` row, disables maps and semantic consumption immediately, and defers exact-once wrapper closure to normal Disable/Destroy teardown. `AC-M5D7Q0-012` verifies the behavior and that no new M5D7M state row is needed. |
| Fault-stage source was missing from the implementation allowlist | `InputRouterDiagnostic.cs` is now allowlisted for appending `HubUiOnly` without renumbering existing values. |
| Unity metadata was missing for the new authored paths | The allowlist includes `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`, `Assets/Prefabs/Hub.meta`, and `HubRuntimeRoot.prefab.meta`; `AC-M5D7Q0-008` checks prefab GUID preservation and `AC-M5D7Q0-011` constrains the exact metadata scope. |
| Prepared-failure and local-clock proof coverage was underspecified | `AC-M5D7Q0-002` injects each prepared lifecycle failure boundary. `AC-M5D7Q0-003` defines the no-publication `0/0` baseline and first `0/0` receipt with `1/1` successor. `AC-M5D7Q0-007` separately mutates each proof/successor field and overflows tick and ordinal. |
| FPS comparison did not fix callback timing | `AC-M5D7Q0-003` injects the same callback script before the same numbered fixed boundaries at each render FPS. |
| Domain-reload expectation was ambiguous | `AC-M5D7Q0-009` defines reload coverage as teardown and execution of a fresh session, with no runtime state carried across. |
| Builder identity stability was not explicit | The contract and `AC-M5D7Q0-008` require stable prefab serialization and preservation of folder/prefab metadata GUIDs on the second builder run. |

## Scope and regression review

- `REQ-M5D7Q0-001..010` and `AC-M5D7Q0-001..012` maintain the existing
  `InputRouter` as the sole wrapper callback owner, map owner, and semantic
  frame source. The closed false-by-default discriminator, five-reference
  null matrix, prepared owner/action identity, map state, requester state, and
  UI-only transaction are covered.
- The prepared lifecycle retains M5D7M's owner-bound reservation, adoption,
  confirmation, and prepublication failure path. Failure injection distinguishes
  each ownership transfer boundary and verifies fallback suppression and
  exact closure responsibility.
- The local clock is separated from gameplay time. The contract binds the
  initial empty publication state, first successful receipt/frame/proof, exact
  successor, checked overflow, and fixed-boundary replay conditions.
- FullSimulation remains the default for missing serialized data and retains
  existing initialization/commit behavior. `AC-M5D7Q0-001` and
  `AC-M5D7Q0-010` require hybrid rejection and direct M5B5/M5D7M/M5D7N/M5D7P-A
  plus full EditMode/PlayMode regression evidence.
- Typed assignment-only seams, exact sibling identity, observational
  no-repair prefab validation, component-order/forbidden-object negative tests,
  and allowlist scope checks are explicit.
- `REQ-M5D7Q0-008`, `AC-M5D7Q0-007`, and `AC-M5D7Q0-012` preserve forensic
  receipts, frames, and pending input on fault; distinguish preconfirmation
  closure from postpublication terminal containment; and prevent retries or
  double closure.

## Separate implementation and approval gates

This PASS accepts the contract's design readiness only. **M5D7P-A must still
reach `Verified` before Q0 implementation**, as required by `REQ-M5D7Q0-011`
and the verification sequence. **Astra's explicit approval is also still
outstanding**; the contract remains Draft and grants no implementation
authority. Neither gate changes the pre-gate finding counts above.
