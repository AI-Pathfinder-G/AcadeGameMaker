# C3 existing-boundary patch map

Date: 2026-09-28  
Status: dispatch preparation only — no C3 source/test execution or acceptance

Inputs reviewed: Approved C3, `c3-test-matrix-plan.md`, Q-A/Q-B runtime, and
the C1 observation/capture paths. This map preserves the existing assembly
direction: `Hub.Presentation.Unity -> Input.Unity -> Profile`. No new public
ABI, asmdef, or friend assembly is needed.

| Order | Exact current boundary | Narrow C3 change |
| --- | --- | --- |
| 1 | `Runtime/Profile/ProfileResetDiskTransactionV1.cs`: `CaptureConfirmationIdentity`, `CheckRoot`, `Observe`, and `ProfileRootOperationLockV1` ownership | Add the allowed observation-only held-lease capture that returns C1-owned identity plus three immutable decoded leaf projections from the same reads. Keep `Begin`, marker publication, archive, and every durable C1/C2 path unchanged. |
| 2 | `Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`: original `ResetLaunchCohortWitness`, `TryGetOriginalCohort`, `HasExactResetLaunchCohort`, `IsExactResetRouter` | Add only `GetAuthenticatedLaunchRootForNewGame(InputRouter)`: validate the original active cohort and both root/router witnesses, then return the established normalized root. It must not query environment or touch profile/disk state. |
| 3 | New `Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | Place closed C3 value/result/capability types and the lower profile-observation/confirmed-identity coordinator here. Inputs are adapter/router plus opaque owner/epoch references; it must not reference Hub request/presenter types. |
| 4 | `Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs`: `_request`, `_takenRequest`, proofs, `_state`, `TryTakeRequest`, `Close`/`Fail` | Register private issuance exactly at the real `TryTakeRequest` transition, bound to request, presenter, owner and epoch. Add successor rearm without clearing `_takenRequest` or moving `RequestTaken` backward. |
| 5 | `Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs`: `_cursor`, `_controller`, handoff/intent proofs, `_state`, `TryTakeRetainedIntent`, `Update`, `FixedUpdate`, `Activate`, `Close`/`Fail` | Add owner-authenticated successor rearm: fresh controller and cursor, preserve notice dismissal, quarantine AwaitingBaseline, then discard exactly one validated consecutive `TryAdvance` frame before Ready. Do not use `Interpret`/`Activate` on that frame or reuse old cursor/intent. |
| 6 | New `Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | Own `AcceptNewGame`, initial/retry capture, DecisionRequired exact copy/labels, capability CAS, Confirm/Cancel, re-gate, rearm handshake, and execution-commit invalidation. It is synthetic-only and cannot call C2/destination/map-enable paths. |
| 7 | Existing test assemblies | Add `ProfileNewGameConfirmationV1Tests` under Input.Unity EditMode, `NewGameConfirmationOwnerV1Tests` under HubPresentation EditMode, and `NewGameConfirmationRearmPlayModeTests` under HubPresentation PlayMode, following the approved C3 matrix. |

Implementation sequence: first the C1-owned same-lease projection and
Input.Unity root/closed coordinator boundaries; then Q-B issuance evidence;
then the Hub owner; then Q-A successor quarantine/rearm; finally the focused
matrix in the order AC-001/002, AC-003/004, AC-005/006, AC-007/008, and the
static/evidence gates AC-009/010.

No blocking contract mismatch was found. The current
`CaptureConfirmationIdentity` returns only identity, while C3 requires same
lease identity plus decoded projections; this is the explicitly allowlisted,
observation-only Profile addition, not a C1 durable-algorithm change.
