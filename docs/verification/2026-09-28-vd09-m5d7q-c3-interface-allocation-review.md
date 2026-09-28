# VD-09 M5D7Q-C3 interface and allocation review

- Date: 2026-09-28
- Reviewer: Sol (`gpt-5.6-sol`), bounded interface counter-design
- Authority: advisory only; this note does not amend the Approved C3 contract,
  approve implementation, or override Astra
- Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md`
- Scope: `REQ-M5D7QC3-001..007`; implementation evidence remains pending

## Finding

No contract blocker was found. The Approved C3 behavior fits the existing
assembly direction `Hub.Presentation.Unity -> Input.Unity -> Profile` and the
existing friend boundaries. It does not require a public ABI, reverse reference,
new friend assembly, asmdef change, scene/prefab change, C1 mutation change, or
C2 authority.

This review is allocation guidance only. Future changes to
`ProfileResetDiskTransactionV1.cs` and `DesktopProfileLaunchAdapterV1.cs` must
wait until C2 has completed independent verification and Astra has accepted and
frozen the C2 source. This preserves the C3 delivery gate and the Approved
allowlist (`AC-M5D7QC3-009/010`).

## Interface and file authority map

### Profile/C1: same-read observation proof

`ProfileResetDiskTransactionV1.cs` should remain the owner of root lease,
barrier checking, the three active leaf reads, decoding, and confirmation
identity. Add one internal observation-only capture result that contains:

- the existing single-use `ProfileResetConfirmationIdentityV1`;
- exactly three immutable projections in Primary, Previous, Temp order;
- for each projection, exact role/name, presence, raw-byte length/hash,
  classification and decoded revision;
- only for `ValidCurrentInput`, the decoded immutable
  `ProfileCanonicalDocumentV1` produced by that same read and decode.

The identity leaf and its projection must be built together from the same byte
array while the same actual root lease is held. Every getter must validate their
agreement. No second read, filename probe, launch-receipt reconstruction, or
caller-supplied projection is classification evidence. `Begin`, `Resume`,
barrier publication, archive handling, and durable algorithms remain unchanged.
This supports `REQ-M5D7QC3-002/004` and `AC-M5D7QC3-002/004`.

### Input.Unity: engine-facing confirmation core

The new `ProfileNewGameConfirmationV1.cs` should own the lower-level internal
bridge from exact launch identity to C1 observation. Its entry takes only
Input.Unity types plus opaque reference-identity owner and epoch tokens. It:

1. authenticates the exact launch adapter/router/root;
2. invokes the real C1 same-lease capture;
3. retains the private identity/projection evidence;
4. classifies the three leaves without exposing documents or identity to UI.

For every `ValidCurrentInput` document, compare product fields explicitly with
`ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument`: settings,
input asset/schema and empty binding override, tutorial confirmations, and all
progression fields. Exclude only bookkeeping profile revision. Do not depend on
revision, selected source, filenames, canonical-byte equality, or incidental
struct equality. Apply the Approved precedence: unreadable first, then any
meaningful valid leaf, then ambiguity (including any present Previous/Temp or
input-recovery/invalid/unsupported leaf), otherwise exact-default/no-leaf.
This is the bounded core for `REQ-M5D7QC3-001/002/004` and
`AC-M5D7QC3-001/002/004`.

`DesktopProfileLaunchAdapterV1.cs` needs only the Approved read-only
`GetAuthenticatedLaunchRootForNewGame(InputRouter)` seam. It returns the
original normalized root after validating the exact active OriginalLaunch
cohort, both root witnesses, exact router, and nonterminal/non-C2-gated state.
It performs no environment lookup, lease, observation, state creation, or
receipt publication.

### Hub confirmation owner: lifecycle and capabilities

The new `NewGameConfirmationOwnerV1.cs` should be the sole C3 lifecycle/CAS
owner. It owns request intake, Busy capture retry, decision generations,
Confirm/Cancel exclusion, confirm recapture comparison, confirmed-request
publication, cancellation rearm, and execution-commit invalidation. Private
capabilities must bind by reference identity to the exact C3 owner, presenter,
Q-B owner, adapter, router, root, interaction epoch, request and captured C1
identity. Presentation results contain no identity or document.

This owner must invalidate decision/retry/rearm authority before an execution
owner can call C1 Begin. It grants no C2, map, destination, scene, gameplay,
wardrobe, Settings or Quit authority (`REQ-M5D7QC3-003/004/006/007`,
`AC-M5D7QC3-003/007/008/009`).

### Q-B: actual-take issuance and monotonic history

`HubMenuIntentHandoffOwnerV1.cs` must retain `_takenRequest` as immutable
history and add a strictly increasing interaction epoch plus a private one-shot
issuance record. Issuance must occur at the actual Q-B NewGame take/claim
boundary and bind the exact handoff owner, presenter, C3 owner, taken request and
epoch. A later lookup based only on equal `HubMenuIntentRequestV1` values is not
authentication.

Cancellation rearm advances the epoch and prepares a fresh request slot. It
never clears, rewrites, retakes or republishes the old request. Duplicate,
foreign, partial or reused capabilities close rather than roll back. This is
required by `REQ-M5D7QC3-001/005` and `AC-M5D7QC3-001/005/006`.

### Q-A presenter: successor quarantine, not history reuse

`HubMenuPresenterV1.cs` must retain the old intent/controller/cursor as forensic
history. Rearm creates a fresh controller from the unchanged validated handoff
and preserved notice state, creates a fresh cursor with the existing factory,
sets NewGame focus, and records an owner-bound successor-baseline-pending
witness.

While that witness is pending, `Update` cannot promote the successor to Ready,
even if the cursor factory immediately reports Ready. `FixedUpdate` may only
advance the exact fresh cursor. The first consecutive frame is validated and
discarded without `Interpret` or `Activate`; only then is the witness consumed
and Ready published. False advance remains quarantined. Foreign cursor,
skipped/malformed frame, disable/destroy, reflected witness corruption or
partial Q-A/Q-B successor disagreement closes all successors with no rollback
or old-edge replay (`REQ-M5D7QC3-005`, `AC-M5D7QC3-005/006`).

## Allocation and stop boundary

The minimal seams are: C1 same-read capture; Input.Unity capture/recapture;
Q-B actual-take issuance plus epoch advance; and presenter successor preparation
plus baseline completion. Tests should map failures to the cited
`AC-M5D7QC3-*` rows and preserve zero profile/map/history mutation on rejection.

Stop for Astra if implementation requires a copied-value issuer check, a second
profile read for classification, reuse/clearing of Q-A or Q-B history, ordinary
cursor Ready as successor readiness proof, public ABI/friend/asmdef expansion,
or any C1/C2 durable-behavior change. Astra alone approves implementation and
final integration; Luna independently verifies it.
