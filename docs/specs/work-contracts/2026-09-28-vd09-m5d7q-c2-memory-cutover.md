---
status: Verified
---

# VD-09 M5D7Q-C2 reset memory cutover and receipt

- Date: 2026-09-28
- Status: Verified — Astra 2026-09-28; bounded synthetic-only implementation
- Owner and approval: Astra
- Bounded design: Sol; intended implementation: Terra; independent QA: Luna
- Parent: `2026-09-28-vd09-m5d7q-c-new-game-reset.md`
- Required predecessor: C1 reset disk transaction Approved and verified for its
  implementation scope; C1 disk-only gates closed on 2026-09-28
- Parent trace: `REQ-M5D7QC-004/005/006/007`,
  `AC-M5D7QC-003/004/005/006/007`

## Purpose and boundary

C2 is the bounded Unity memory-finalization half of an already confirmed reset.
It accepts a C1 disk-prepared proof, reacquires the actual profile root lease,
reauthenticates that proof against the live canonical barrier, complete archive,
and exact default revision `0` disk row, stages fresh disabled default input
actions and the exact default profile snapshots, terminally gates the current
session, applies the new memory state, removes the barrier, reprobes disk, and
only then publishes one reset receipt.

C2 does not request or decide confirmation, perform C1 archival/default save,
load an archive, retry an uncertain operation in-session, save a profile, show
UI, transition a scene, start gameplay/run, enable a gameplay map, change OS
window/audio devices, or claim New Game destination acceptance. Its successful
receipt says only that disk and the bounded session memory agree on exact r0.

The future confirmed-request owner must consume/invalidate the NewGame request
and confirm/cancel callbacks before invoking C1/C2. Once a durable barrier was
published, Cancel is no longer a legal operation. C2 has no pending-request
owner and must not be wired to Q-B until that owner has a separately Approved
contract. A terminal reset result is not permission to return to the old menu.

## Existing-runtime compatibility decision

`DesktopProfileLaunchReceiptV1` and `HubEntryHandoffReceiptV1` are immutable
launch-history evidence and remain unchanged. They are not rewritten to look as
if the application originally launched from the reset profile. C2 adds a
separate current-session profile cell owned by `DesktopProfileLaunchAdapterV1`.
After a successful reset, consumers that need current profile state must use
that cell through a later explicitly approved integration; existing handoff
receipt consumers retain historical launch meaning.

The current initialized `InputRouter` has no legal action replacement state.
C2 therefore requires one narrow owner-authenticated terminal cutover lane on
the existing HubUIOnly prepared lifecycle. This lane is not a second launch and
cannot return to the pre-reset actions. No general hot-swap API is authorized.

## Entry objects and ownership

The coordinator entry is internal to `AcadeGameMaker.Input.Unity`:

```csharp
ProfileResetMemoryResultV1 FinalizeReset(
    string root,
    ProfileResetDiskPreparedProofV1 diskProof,
    DesktopProfileLaunchAdapterV1 sessionOwner,
    InputRouter router);
```

Arguments are non-null; root uses the existing normalized non-root rules. The
adapter and router must be the exact active authored pair, the adapter must have
one published valid desktop launch receipt, and the router must be initialized,
unfaulted HubUIOnly owned by that adapter. No gameplay-enabled or transition
router, pending launch publication, foreign owner, duplicate adapter, disabled
object, or already terminal/reset session is eligible.

The adapter retains an independent router identity witness established during
its original successful launch, in addition to its mutable serialized binding.
Clean argument/eligibility/protocol rejection preserves the session. Detection
of reflected lifecycle corruption may invoke the existing fail-closed
containment; it must not be mislabeled Busy or Completed.

The normalized profile root from original launch Awake is retained as a pending
candidate and promoted only on successful original Start publication into two
independent immutable session-root witnesses. Together with the router witness
and published historical launch receipt they prove one launch cohort. Supplied
C2 root must match both witnesses by the existing platform-consistent normalized
path equality before lock acquisition, proof inspection/consumption, staging or
mutation. Foreign-root mismatch is clean nonmutating protocol rejection; a
default/unknown/reflection-corrupt witness invokes fail-closed containment.
Neither historical launch receipt nor hub receipt is amended to carry the root.

Before acquiring the root lock, the coordinator calls the Profile-owned internal
`ValidateMemoryResetRootBeforeLease(root)` read-only containment check. It reuses
C1's root/ancestor/lock-path reparse checks; it never creates a directory or lock.
A changed reparse root must not let Acquire follow a foreign directory and write
its lock before held-lease reauthentication can reject it. Held checks repeat
containment under the actual lease; pre-lease validation is not durable authority.

`ProfileResetDiskPreparedProofV1` remains C1-owned, immutable and one-consumer.
C2 does not trust its copied scalars. It acquires
`ProfileRootOperationLockV1`, then calls a C1-owned internal
`ReauthenticatePreparedUnderHeldRoot(lease, root, proof)` capability. That
capability validates the live lease/root and reopens and validates:

- the exact canonical marker bytes and transaction ID bound by the proof;
- every marker-declared archive destination and absence of every old active
  identity according to C1's two-pass rules;
- exact approved default canonical bytes at `profile.json`, revision `0`;
- absent `profile.prev.json` and `profile.tmp.json`;
- absent `profile.reset.tmp.json` and present exact `profile.reset.json`.

Ordering is normative: first validate the proof's immutable payload and root,
then every live disk row above without mutation, then perform the proof CAS as
the final reauthentication step and mint authority. A failed live check leaves
the proof unconsumed. A duplicate/CAS loser is protocol rejection and cannot
mint authority. No stale or forged proof is consumed to make it look accepted.
The authority, cutover capability, current-session cell and reset receipt all
bind the exact launch-established normalized root as well as transaction identity.

It returns an unforgeable, lease-bound `ValidatedResetMemoryAuthorityV1` usable
only while that same live lease is held. Failure consumes no disk authority,
does not stage/apply memory, and returns `ReloadRequired` or
`ManualRepairRequired` according to C1's closed classification. A process
restart may call C1 Resume on a disk-prepared marker to mint a new one-shot
prepared proof; C2 never reconstructs or guesses one.

Busy acquisition waits at most five seconds and returns non-mutating `Busy`.
The root lease is held through reauthentication, staging, session gating,
memory application, barrier removal, final reprobe and receipt publication.

## Exact staged state

After reauthentication and before any live mutation, C2 obtains
`ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument` and requires
its canonical bytes to equal the authority target. It stages:

- one new `GameInputActions`, initially disabled on every map;
- one exact `ProfileBindingOverrideApplyAdapterV1.Apply` of the target input;
- target `ProfileSettingsSnapshot`, `ProfileTutorialSnapshot`, and
  `ProfileProgressionSnapshot` (`ProfileRevision == 0`);
- the complete target `ProfileCanonicalDocumentV1` as the current-session
  profile value.

The binding result must be exact `DefaultsReady`, failure `None`, disposition
`Retain`; the input override must be empty and current-compatible. Any creation,
apply, validation, or staging failure disposes only the staged actions exactly
once, leaves the prior profile value and action ownership unchanged, retains
the barrier, and returns terminal `ReloadRequired`. Before releasing ownership
the adapter/router must latch the fail-stop reset gate and disable maps,
callbacks, request publication and ordinary-save authority. This gate is a
failure-safety mutation, not successful profile/action publication. There is no
fallback to interactive existing actions, Cancel, or the old menu and no
receipt. A failure in the gate itself must retain terminal fault authority;
it cannot yield a success-like usable session. Pre-entry argument/protocol
rejection does not claim an authorized reset result.

Settings/tutorial/progression application in C2 means atomic publication of
these validated immutable snapshots in the session profile cell. This contract
does not invent a window, mixer, tutorial presenter, or gameplay progression
side-effect adapter. Such side effects require later contracts.

## Session terminal gate and irreversible boundary

Before changing any live profile/actions reference, the adapter enters
`ResetCutoverTerminal` and the router enters its owner-authenticated reset
cutover lane. This is the irreversible in-session boundary:

- adapter ordinary profile save access, menu intent production, notification
  production and repeat reset entry are refused;
- router UI/gameplay maps are disabled, its callbacks are unregistered, and no
  semantic frame or mode request can be published;
- the prior `GameInputActions` remains owned by the router until the staged
  candidate and session state have passed all validation, but it can never be
  re-enabled or saved after the terminal gate;
- hub handoff/history receipts remain immutable evidence but confer no authority
  to resume interaction after the gate.

Any already-published request is inert history after the terminal gate; its
future consumer must recheck the current adapter/router authority before any
effect. The future request owner must atomically invalidate retained/queued
NewGame requests and late confirm/cancel callbacks before reset entry. C2 does
not mutate a request object's historical value and does not pretend disabled
input alone invalidates an already-taken object. Until that owner contract is
Approved and integrated, C2 is synthetic-only and cannot be wired to live menu
requests. Late callbacks cannot reopen the terminal adapter/router lifecycle.

Entering the gate returns one internal `ResetCutoverCapabilityV1` bound by
reference identity to the adapter, router, old actions, staged actions and C1
memory authority. It cannot be constructed by callers or reused. If terminal
gate entry fails before maps/callbacks are disabled, staged actions are disposed
and prior profile/action references remain only as unusable forensic ownership.
After successful reauthentication and proof consumption, every unsuccessful exit
latches terminal failure in both owners before lease release, even if teardown
failed before the maps or callbacks were disabled. Either owner's terminal bit
or shared generation witness makes both sides noninteractive and self-containing.
No old menu, input, save or in-session retry is legal. Reauthentication failure
after accepting an otherwise eligible entry also latches this failure gate; the
unconsumed proof may be observed on restart, not retried in the terminal session.
Only clean pre-entry rejection or nonmutating Busy preserves interaction.

## Pair-atomic memory publication

Under the terminal gate, one router method consumes the capability and replaces
the old actions with the staged disabled actions. It validates all maps disabled
before and after transfer, never enables UI or gameplay, never publishes an
input receipt, and closes/disposes the old actions exactly once only after the
new reference is owned. The adapter then publishes one immutable
`CurrentSessionProfileV1` containing the exact target document and copied
settings/input/tutorial/progression plus reference proof for the router-owned
new actions.

The adapter cell and router validate one shared cutover generation and object
identity at every getter. Publication is considered applied only when both
validate together. There is no observable successful row with new profile/old
actions or old profile/new actions. Any exception after terminal entry closes
both old and staged candidates as applicable, retains forensic fields, marks
adapter and router terminal/faulted, retains the barrier, returns
`ReloadRequired`, and emits no receipt. The session cannot save, return to menu,
process input, start gameplay, retry C2, or remove the barrier.

This is fail-stop pair-atomicity, not rollback atomicity: after the terminal
boundary, failure may leave partial in-memory forensic state, but none of it is
usable and old state can never become a writer again.

Exactly-once disposal means one invocation/ownership-close attempt, witnessed
before the external Dispose call. If Dispose throws, disposal success is not
claimed: the pair is Faulted and no receipt can be published. Existing
best-effort teardown that swallows errors cannot prove a successful C2 row.
Terminal input getters reject access to old published semantic frames/receipts;
those values are retained only as forensic history, never as current authority.

## Barrier removal and final validation

Only after exact pair publication validates does C2 ask the C1/Profile owner to
remove `profile.reset.json` under the same lease and exact authenticated marker
authority. Removal uses no rename/backup/fallback. A missing-before-call,
changed, unreadable, directory/reparse-valued marker, delete exception, or
uncertain post-delete observation is terminal `ReloadRequired`; the session
remains gated and no receipt is produced. C2 never guesses that an exception
means removal succeeded.

After a non-throwing removal, C2 freshly reprobes under the same lease:

- both barrier names absent;
- Primary exact canonical target r0;
- Previous and Temp absent;
- complete exact C1 archive still present;
- current-session document/snapshots equal the target;
- router owns the new actions, all maps remain disabled, no callbacks or input
  receipt are published, and the old actions are closed exactly once;
- adapter/router remain terminally gated.

Any mismatch is `ReloadRequired`, with no receipt and no attempt to recreate the
barrier. Before releasing ownership, the adapter first latches its terminal
failure state so no old or partial state can save after lease release.

## One-shot receipt and output

Only the exact final row may create `ProfileResetMemoryReceiptV1`. It binds the
transaction ID, marker hash, target canonical hash/length/revision `0`, archive
proof digest, exact session document, default binding-apply evidence, router
ownership/generation proof, barrier-absence reprobe and `UIOnlyBlocked` session
disposition. It is immutable, defensively copied, validates every getter, and
may be taken exactly once from the adapter. Publication is the last in-memory
write before lease release.

The receipt does not enable UI, gameplay or a scene. A later destination/menu
contract must explicitly accept it and decide whether/how to create a fresh
interactive lifecycle. Rejection cannot undo the durable reset.

`ProfileResetMemoryOutcomeV1` is closed to `Completed`, `Busy`,
`ReloadRequired`, and `ManualRepairRequired`. Only `Completed` carries the
receipt and exact final evidence. `Busy` proves no staged/live mutation.
Failures carry no receipt or success-like document. Default, unknown and
reflection-corrupted result/capability/cell/receipt rows throw.

## Requirements

- **REQ-M5D7QC2-001:** Reacquire the actual root lease and consume/revalidate one
  C1 proof against the live marker, full archive and exact target-only r0 disk
  state before staging or memory mutation.
- **REQ-M5D7QC2-002:** Stage one fresh fully disabled default action collection
  and the exact planner-default profile/settings/input/tutorial/progression;
  pre-gate failure leaves live memory unchanged and retains the barrier.
- **REQ-M5D7QC2-003:** Enter an owner-authenticated terminal session gate before
  live apply so old actions/profile can never write or resume after the boundary.
- **REQ-M5D7QC2-004:** Apply router action ownership and current-session profile
  as one fail-stop pair, disposing the old collection exactly once and exposing
  no usable partial row.
- **REQ-M5D7QC2-005:** Remove only the authenticated barrier after exact memory
  apply; removal fault or uncertainty remains terminal with no receipt/retry.
- **REQ-M5D7QC2-006:** Reprobe barrier absence, disk/archive exactness and memory
  agreement under the same lease before publishing one receipt and releasing.
- **REQ-M5D7QC2-007:** Preserve launch/hub receipts as history and grant no UI,
  scene, gameplay, save, notification or product-destination authority.

## Acceptance criteria

- **AC-M5D7QC2-001:** Valid, consumed, foreign-root, forged, mutated, stale and
  reflection-corrupt C1 proofs plus live marker/archive/active-file mutations
  prove lease-held reauthentication and exact-once consumption.
- **AC-M5D7QC2-002:** Binding candidate creation/apply/disposal fault matrices
  prove exact defaults, all maps disabled, exact one apply, no live profile or
  action-ownership publication before the gate, and barrier retention plus
  terminal failure gating on every staging failure.
- **AC-M5D7QC2-003:** Every adapter/router eligibility and terminal-gate fault
  point proves no post-boundary input/menu/save publication and no old-state
  resurrection; clean eligibility/protocol rejection before successful
  reauthentication preserves the prior session exactly. Reflected lifecycle
  corruption may invoke existing containment, never a success/Busy result.
  Exact/foreign roots and individual reflected root/router-witness mutations
  prove launch-cohort binding before lock acquisition or proof consumption.
- **AC-M5D7QC2-004:** Faults before/after action transfer, old callback removal,
  old disposal, session-cell publication and cross-proof validation prove exact
  ownership/disposal and fail-stop pair semantics without rollback claims.
- **AC-M5D7QC2-005:** Marker changed/missing/locked/directory/reparse cases and
  faults before/during/after delete prove no false success, no barrier rebuild,
  no same-session retry and terminal gating before lease release.
- **AC-M5D7QC2-006:** Final reprobe mutations of every active/archive/barrier and
  memory field prove receipt publication is last, exact once, and impossible
  unless durable r0 and bounded memory agree.
- **AC-M5D7QC2-007:** Restart from C1 `DiskPrepared` can mint a fresh prepared
  proof and complete C2; restart after barrier absence follows ordinary exact-r0
  launch and cannot replay the lost C2 receipt or resurrect old progress.
- **AC-M5D7QC2-008:** Independent tests prove existing launch/handoff receipts
  remain byte/value-identical historical evidence while the new session cell
  alone owns current reset state.
- **AC-M5D7QC2-009:** Static/API review proves no UI/scene/gameplay transition,
  map enable, save, archive restore/cleanup, notification, OS settings/audio
  side effect, public hot-swap API, network, clock or random authority.
- **AC-M5D7QC2-010:** Focused and required regression suites have zero failed,
  skipped and inconclusive tests, and Luna reports P0=0/P1=0 before Astra
  integration.

Traceability: `001 -> AC-001/007`; `002 -> AC-002`; `003 -> AC-003/004`;
`004 -> AC-004/008`; `005 -> AC-005`; `006 -> AC-006/007`;
`007 -> AC-008/009/010`. All AC references use the full
`AC-M5D7QC2-*` identifier.

## Exact implementation allowlist

After Astra changes this contract to Approved, implementation may:

- add `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs`
  and `.meta`;
- modify `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`
  only for the read-only pre-lease containment check, held-lease prepared-proof
  reauthentication and authenticated barrier removal/final-reprobe capabilities;
- modify `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` only for the
  owner-bound terminal reset cutover lane described here;
- modify
  `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`
  only for launch-established router/root identity witnesses, the terminal gate,
  current-session cell and one-shot reset receipt;
- add focused EditMode Profile tests and PlayMode Input.Unity tests, each with
  `.meta`, plus C2 evidence and one minimal `docs/README.md` link.

Existing `InternalsVisibleTo("AcadeGameMaker.Input.Unity")` already permits the
required Profile-to-Input.Unity internal boundary. No asmdef, friend assembly,
generated actions, input asset, hub latch/presenter, scene, prefab, Packages,
ProjectSettings or unrelated source modification is authorized.

### Deterministic verification seams

The allowlisted sources may add internal closed-enum checkpoint/control seams
for proof reauthentication, candidate creation and binding application, terminal
bit entry, map disabling/quarantine cancellation, each callback removal, action
transfer, old Dispose invocation/return, session-cell publication, pair
validation, authenticated barrier deletion, every final disk/archive/memory
reprobe, and receipt staging/publication. Production uses a no-op control.
Tests operate on real GameInputActions and the same BCL paths/operations as
production; no alternate save/reset algorithm, global mutable hook, public ABI,
friend/asmdef change or copied test implementation is permitted.
Controls may only observe/throw at named boundaries, never substitute candidates,
actions, binding results, disk observations or operation return values. Narrow
operation delegates wrap only Disable/RemoveCallbacks/Dispose on the exact owned
real object; production invokes each real operation exactly once. Construction,
binding apply and BCL deletion stay direct calls with before/after checkpoints.

Before/after checkpoints are not falsely claimed as a fault inside a BCL call.
Real sharing/access failures exercise delete-call failures; before/after seams
exercise boundary faults, including an exception after actual successful delete.
Faults during map/callback/Dispose operations require a narrow internal operation
delegate that calls the real operation and can throw before/after that call;
production delegates perform it exactly once. No successful teardown is claimed
when the operation throws. Evidence distinguishes each operation and boundary.

Receipt staging/validation can fault before publication. The authoritative
receipt assignment is last with no subsequent fallible operation; a test must
not invent an after-publication exception that contradicts this atomic row.

### Approved final-evidence mint clarification — Astra 2026-09-28

The Profile-owned final-reprobe capability may additionally return an internal
`ValidatedResetFinalDiskProofV1`, minted only after its real successful held-lease
probe. The existing void verifier may remain as a compatibility wrapper. The
proof copies normalized root, transaction/marker/target/archive fingerprints,
exact target bytes/revision and both barrier-absence facts. Its private mint
witness is Profile-owned, not a caller-supplied boolean; default, foreign,
unknown or reflected-corrupt evidence is rejected. Observational getters
perform no disk I/O and retain no live lease/authority. This is detached final
evidence, not permission to delete, save, restore or probe a different root.
The Unity receipt factory requires this proof and the exact pair/default-binding
evidence; arbitrary scalar strings cannot mint a completed receipt. All receipt
and result construction/validation occurs before the final adapter assignment.
This clarifies the already-required final receipt row within the existing
Profile/Unity allowlist and introduces no product authority.

For AC-M5D7QC2-003/005 actual Windows reparse evidence, the focused Profile
test allowlist explicitly includes
`Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileResetMemoryJunctionFixtureV1.cs`
and `.meta`. This test-only helper may use CreateFile/DeviceIoControl to create
and verify NTFS junctions, without subprocess, admin elevation or installation.
Every link and target is confined to one uniquely owned, validated OS-temp base.
Links are removed first as nonrecursive links; before any recursive cleanup the
exact resolved base is verified to remain that test-owned descendant. Targets'
sentinel bytes and absence of foreign lock/barrier mutation are asserted.
Fixture inability is a failed/unverified gate, never Ignore/Skip or a fabricated
positive. This helper supplies hostile filesystem state only, not a copied
production reset algorithm, and has no production reference or callable API.

## Stop and rollback

Stop for Astra before implementation if C1 does not expose a stable one-shot
prepared proof, if exact archive reauthentication cannot remain C1-owned, if
the existing Profile friend boundary is insufficient, or if a correct result
requires mutating launch/hub receipts, enabling a map, applying OS/audio
settings, changing a scene/prefab, adding a general router hot-swap, saving,
or widening public ABI. A requirement to keep the current menu interactive
during/after C2 is a new product/lifecycle decision and is not inferred.

Rollback removes only the new C2 source/tests/docs and reverts C2-owned hunks
in the three allowlisted existing sources. It never deletes/recreates a durable
barrier, archive, active profile, C1 evidence, or unrelated dirty work. A
runtime transaction that crossed the terminal gate is not rolled back by source
rollback; its disk state remains governed by C1/C2 recovery rules.

## Known technical gaps for Astra review

1. No current runtime type owns a mutable session profile. The proposed
   adapter-owned `CurrentSessionProfileV1` is new bounded authority; consumers
   are deliberately not migrated in C2.
2. `InputRouter` supports one prepared launch and terminal failure, but no
   initialized action replacement. The proposed terminal reset lane is a real
   lifecycle extension and needs Luna pre-review of callback/map teardown.
3. No runtime applies profile window/audio settings or tutorial/progression to
   live subsystems. C2 can truthfully publish their immutable session snapshots
   only. Product side effects need later contracts.
4. Existing hub handoff/menu objects may remain visually alive after the gate,
   but C2 must make their input/intent authority unusable. Re-enabling a fresh
   interactive hub after receipt is intentionally deferred.
5. Crash after barrier deletion but before receipt cannot recreate the receipt;
   restart observes ordinary exact r0. This is safe durable behavior, but a
   later UX contract must decide how to communicate that reset completion.

## Delivery gate

Luna pre-review must cite every `AC-M5D7QC2-*` and explicitly assess the five
technical gaps. Astra alone may change Review to Approved. Terra then implements
with `REQ-M5D7QC2-*` citations; Luna independently verifies exact evidence; Astra
alone accepts integration. Runtime edits require Approved status and closure of
the predecessor gate; both are recorded in the approval below.

### Astra approval — 2026-09-28

#### Approved implementation clarification — original ownership witnesses

REQ-M5D7QC2-003/004/007 and AC-M5D7QC2-003/004/008 require original
cohort/action identity and irreversible terminal/disposition authority, not
equality among mutable duplicated fields. A private registration keyed to both
the actual launch adapter and router may hold that authority within the existing
allowlisted sources. It is staged only after real launch proof validation and
becomes eligible only after real launch publication. No caller-provided root,
receipt, action reference, generation or diagnostic flag alone can mint it.

Recording a staged candidate's identity is not transfer of disposal ownership.
Before the router's exact claim, the adapter/coordinator owns its single close
attempt; after claim, the router owns it. Original old actions are recorded at
launch, not learned from potentially corrupted reset fields. Private close
attempt authority is minted immediately before the owned external Dispose
invocation; observing a mutable `DisposeAttempted=true` field cannot mint that
authority. A reflected false flag cannot authorize a second invocation.

Both owners' interactive and lifecycle paths consult the same irreversible
authority. Normal fixed ticks in a consistent completed `UIOnlyBlocked` state
are quiescent, not new failures. Corrupt diagnostic/cohort disagreement closes
the original cohort only; clean foreign argument rejection remains nonmutating.
An already-retained current-session cell revalidates the exact live pair at
every getter, while completed receipt evidence remains detached and readable
without a lease or live pair. These clarify existing bounded fail-stop behavior;
they add no interactive destination, public ABI, friend, scene or save authority.

C1's frozen disk-only implementation is Verified after R12/R13/R14 and Luna's
independent final review. Sol supplied source feasibility/counter-review and
Luna reviewed all ten ACs and the five bounded technical gaps. Astra amended
the proof ordering, failure gate, deterministic verification seams, disposal
semantics, historical-frame authority and original session-root binding.
Luna found the remaining contract P1s closed; no product decision is added.

Astra approves the exact allowlisted C2 implementation and tests above.
This paragraph supersedes the draft-only delivery wording, not the behavioral
contract. C2 remains synthetic-only and unverified: no real menu request wiring,
interactive destination, scene, media, gameplay or general action hot-swap is
authorized. Terra implements, Luna independently verifies, Astra integrates.

### Astra final integration — Verified, 2026-09-28

Approved execution snapshot SHA-256:
`A23F9B8E978BE24A8BC1E89EDB48ACF9BA3CFE81FC23CB2EF326B396A8CC7A38`.
Astra accepts the frozen C2/C2R source after Luna's final independent
`2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md`, P0=0/P1=0.
All AC-M5D7QC2-001 through -010 close for this bounded scope, including genuine
restart AC-007 through C2R's separate-process evidence.

Current regression evidence is fresh R35 non-worker 562 plus R36 worker 51
(613 distinct, exact names), and fresh R34 PlayMode 536 plus disjoint R25
matrix 74 (610 distinct), all failed/skipped/inconclusive 0. R31 corrective
45, R32 corrective 21 and R33 authority 32 provide focused evidence, not extra
distinct regression totals. Shared manifests and source fingerprints are
unchanged. R26–R30 establish actual process phases; R29's deliberate death is
not an NUnit pass. Historical R22/R23 failures and unreproduced historical
202-file provenance remain preserved, with no old-592 pass reuse.

Verified does not authorize live menu wiring, confirmation UI, interactive
destination, map enable, scene, gameplay, wardrobe or costume-storage changes.
Later C3/C4 changes require their own Approved boundaries and fresh verification.
