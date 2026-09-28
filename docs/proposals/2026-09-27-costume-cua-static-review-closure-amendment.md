# Costume CUA static-review closure amendment

- Date: 2026-09-27
- Bounded counter-design: Sol (`gpt-5.6-sol`)
- Approval authority: Astra
- Status: proposal only — no implementation authority and no Approved-contract edit
- Approved contract SHA-256:
  `8F18474B8D966BCD688ABE3DBFB723872D341868FAD552583E430C081C689B1A`
- Luna static pre-execution review SHA-256:
  `743ECF1AD040044954D89A4FC9746A3A144F68CE4BA7DAC48B958A83AF6B0639`
- Review disposition addressed: `BLOCKED — P0=0, P1=4, P2=2`
- Existing traceability retained: `REQ-CUA-001..010`, `AC-CUA-001..009`

## Bounded decision

All six findings are technically closable inside the existing additive CUA
allowlist without a product decision, pure-core widening, existing-owner edit,
real media, catalog promotion, scene/prefab change or canonical JSON schema
change.

The correction has five rules:

1. `TrySelect` accepts only a media package. It obtains the current completed
   snapshot exclusively from a private slot advanced through the explicit
   instance-bound post-commit observation port.
2. Every admissible first publication and replacement calls the existing pure
   `CostumePresentationServiceV1.TrySwap`; only its exact committed binding is
   put in the prepared immutable tuple.
3. Package validation independently proves definition, binding, payload,
   manifest, clip completeness, geometry and live-reference identity before
   save. The focused suite independently mutates every required dimension.
4. Atlas bounds use widened checked arithmetic, never unchecked `int`
   addition.
5. This synthetic-only slice rejects every repeated rectangle within one
   `(actionId,facing)` clip. That conservative subset represents no authored
   hold, preserves the existing canonical bytes and leaves intentional-hold
   schema support to a later separately approved real-media contract.

No REQ/AC meaning, approved player behavior, canonical frame property order or
hash-input rule changes.

## 1. Exact completed-snapshot authority

### Public surface

The corrected selection surface is exactly:

```text
TrySelect(CostumeUnityMediaPackageV1 media)
```

It has no `CompletedActorSnapshotV1` parameter, token override, setter or
alternate overload. `ObserveCompletedSnapshot` must not remain a public class
method. The adapter exposes only its existing
`ICostumeUnityCompletedSnapshotPortV1 CompletedSnapshots` property, and the
class implements `Observe(CompletedActorSnapshotV1)` explicitly through that
interface.

The adapter owns a private `hasObservedSnapshot` bit and last accepted snapshot
value. Before the first tuple exists, the port may seed that slot. After a
tuple exists, the same port is the only operation that may advance it and
project a later completed frame. `TrySelect` snapshots the private accepted
value into a local before candidate preparation; it cannot accept, manufacture
or advance a caller-provided snapshot.

### Port predicate

An observation is processed in this exact order:

1. if status is `CommitOutcomeUncertain` or `ReloadRequired`, return
   `ReloadRequired` and change nothing;
2. require exact actor ID and facing `left` or `right`;
3. if there is no accepted observation, accept the value as the first
   post-commit observation;
4. reject a lower tick without changing the slot, tuple or projection;
5. for an equal tick, accept only exact identity across tick, actor, action,
   `ActionAgeQ1000`, facing and both anchors; the exact duplicate is an
   idempotent no-op, while any conflict rejects;
6. for a higher tick, if a tuple exists, require the tuple media to contain the
   exact `(ActionId,Facing)` clip and construct the complete next tuple before
   one reference publication; if no tuple exists, update only the private
   observation slot; and
7. on projection failure after a higher-tick tuple publication, retain that
   tuple as authoritative, enter `ReloadRequired`, and permit no later
   mutation.

`TrySelect` returns `Snapshot` before save when no accepted observation exists,
or when the accepted action/facing has no exact candidate clip. Selection does
not consume or modify the observation slot.

The port call itself is the post-commit authority boundary. This isolated
library has no authorized simulation clock or owner query from which it could
independently infer that a dishonest port caller submitted a future value.
Accordingly, “future/uncommitted” at this boundary means a value not delivered
through the post-commit port: it cannot enter `TrySelect` at all. The focused
static/runtime probe must prove that absence. The later live-owner integration
contract must prove that its single injected owner calls the port only after
tick commit. Adding a clock, owner lookup, detached receipt or user-created
observation token here is forbidden.

This is a precision clarification of the already approved explicit-port model,
not a weakening of the post-commit rule.

## 2. Mandatory pure swap gate and publication tuple

After selection preparation/commit and before CIO save, the adapter must
perform all of the following while no external observation callback runs:

1. call pure `TryCreateBinding` for the proposed package action vocabulary;
2. independently validate the whole media package against both the exact
   Accepted definition and proposed pure binding;
3. resolve the accepted snapshot's exact `(ActionId,Facing)` clip and frame;
4. validate the current published tuple/state correlation when one exists;
5. call pure `CostumePresentationServiceV1.TrySwap`; and
6. construct the complete immutable `PublishedCostumePresentationV1` from the
   returned `committed` binding, staged state, media, accepted snapshot and
   frame.

For a replacement, `TrySwap` receives the current published binding as
`current` and the newly created binding as `proposed`. The adapter requires a
`None` result and exact structural equality between `committed` and `proposed`
across every scalar/hash/revision field and the ordinal clip-ID sequence.
Foreign/stale/incomplete current or proposed bindings, a missing current
snapshot action, or any different committed value rejects before save and
preserves the old tuple by exact reference.

Before that call, an existing current tuple must itself satisfy all of these:
its state current ID for the actor equals its binding costume ID; binding,
media and actor identities/revisions/hashes/action vocabulary agree exactly;
`media.Validate(currentDefinition, currentBinding)` returns `None`; its Unity
references are still live; and its snapshot equals the adapter's last accepted
observation. A proposed-candidate fault preserves this known-good tuple. A
fault in the current tuple cannot truthfully preserve a complete old pair, so
it enters `ReloadRequired` without save or projection. The same current-tuple
check precedes publication of a later observed snapshot.

### First synthetic publication

The pure API has no nullable-current overload and must not be widened. A first
publication is therefore admitted only when all of these are true:

- no published tuple exists;
- the authoritative initial state has no current costume for the actor;
- a port-accepted snapshot exists; and
- the complete proposed package/binding already passed all validation.

Only in that state, call `TrySwap(catalog, proposed, proposed, snapshot, out
committed)` as a reflexive bootstrap validation. This is not a claim that a
prior binding exists. It invokes the unchanged pure identity/catalog/hash/
current-action gate and must return an exact copy of `proposed`. An adapter
constructed with a nonempty durable current ID but no matching published tuple
cannot use this rule; it rejects and requires fresh recovery rather than
inventing current media.

Every potentially throwing or rejecting operation, including exact tuple and
frame construction, completes before `_cio.Save(stagedNextState)`. Exact save
success is still only `CommittedFirst` or `CommittedReplacement` with
`ExpectedRevision == stagedNextState.Revision`. Publication remains one
non-throwing `_published = candidate` reference assignment. Projection occurs
afterward and reads the same reference. No portrait, atlas, clip map, state,
binding, snapshot or frame companion assignment is permitted.

The initial and replacement tests must observe only these boundaries:

```text
during concrete Save: old complete tuple (or null for first save)
after successful Save, at projection: exact complete new tuple
```

There is no callback point between successful Save return and the one reference
assignment.

## 3. Independent package-validation matrix

`CostumeUnityMediaPackageV1.Validate` must not assume that the caller just
created the binding. It independently compares the supplied binding to the
package and Accepted definition, and requires its ordinal `ClipIds` to equal
the package's distinct ordinal action set exactly. Each action has exactly one
left and one right clip. Package validation remains side-effect free.

Each row below is one independent mutation from a known-valid synthetic
candidate. A rejection asserts: no CIO call, no projection call, exact
`ReferenceEquals(oldPublished, adapter.Published)`, unchanged view state/
current ID/status, and unchanged old portrait/gameplay pair.

| Boundary | Independent negative cases |
|---|---|
| Definition/package identity | actor, costume, portrait-set and gameplay-set ID, pending/rejected/unknown definition, synthetic flag false |
| Revisions | package catalog revision, package presentation revision, binding catalog revision, binding presentation revision |
| Binding identity | actor, costume, portrait set, gameplay atlas set, each of portrait/atlas/clip-map hash, missing action, extra action, empty/incomplete binding |
| Payload hashes | mutate portrait bytes only, atlas bytes only, clip-map bytes only; verify each lowercase definition/binding hash independently |
| Clip-map canonical bytes | BOM, whitespace, property reorder, unknown property, duplicate property, wrong schema version, actor/costume/set/PPU/cell/pivot/baseline value, unsorted clips, duplicate `(action,facing)`, missing action, missing left, missing right, unknown facing, frame reorder or frame-field mutation |
| Source manifest | mutate each actor/costume/portrait-set/gameplay-set ID, catalog/presentation revision, each of three hashes, required-action list/order, required-facing list/order, clip-map schema version, plus BOM/whitespace/property order/unknown/duplicate property |
| Unity references | null portrait, null atlas, destroyed portrait, destroyed atlas; test each immediately before validation and clean up only fixture-owned objects |
| Import/profile | portrait filter, atlas filter, PPU |
| Frame values | negative x/y, zero/negative width, height or duration at construction; out-of-atlas x/y/right/top; pivot X, pivot Y and baseline independently |
| Lifetime/staleness | foreign package, stale package, previously rejected candidate, missing snapshot action and candidate whose reference is destroyed before save |

Constructor rejections and `Validate` rejections are recorded separately; a
constructor exception is not counted as proof that an independently
constructible package mutation is rejected.

### Checked atlas bounds

For every frame, calculate:

```text
right = checked((long)x + (long)width)
top   = checked((long)y + (long)height)
valid = x >= 0 && y >= 0 && width > 0 && height > 0
        && right <= atlas.width && top <= atlas.height
```

Do not evaluate `x + width` or `y + height` as `int`, even temporarily. Tests
must include `x=Int32.MaxValue,width=1`,
`x=Int32.MaxValue-1,width=Int32.MaxValue`, and the analogous Y cases, as well
as ordinary one-pixel overrun and exact-edge acceptance. Any overflow or bound
failure returns `Geometry`; it never escapes as success or mutates old state.

### Duplicate rectangles and canonical schema

For this synthetic-only slice, within each single `(actionId,facing)` clip,
the rectangle key `(x,y,width,height)` must be unique across all frame indexes.
Adjacent and nonadjacent duplicates reject as `Clips`, even when duration,
pivot or baseline differs. Identical rectangle keys in different clips remain
allowed because they do not express a repeated frame in one authored clip.

This needs no `hold` property and therefore changes none of:

- top-level, clip or frame property order;
- schema version `1`;
- clip-map/source-manifest canonical encoding; or
- the three catalog hash definitions.

If intentional holds become necessary for real media later, the least invasive
future schema is a separately approved version `2` adding an exact sorted,
unique zero-based `heldFrameIndexes` array after `frames` in each clip. Each
listed index would have to be greater than zero and repeat the immediately
preceding rectangle; every repeated rectangle would have to be declared and
nonconsecutive undeclared reuse would reject. Frame objects would retain their
existing property order, while `ClipMapHash` and the source manifest's
`clipMapSchemaVersion` would necessarily change. Version `2` is not authorized
by this amendment and is not needed to close the current review.

## 4. Replacement, projection and terminal-state invariants

The focused fixture must contain two Accepted synthetic definitions/packages
for one actor so it can prove both first commit and true replacement. A
replacement must preserve the last port-accepted tick, action,
`ActionAgeQ1000`, facing and anchors, call pure `TrySwap`, obtain
`CommittedReplacement`, and publish one new complete tuple. The old tuple
object and its nested immutable values remain unchanged.

Exact outcome rules are:

| Boundary | Authoritative tuple and status |
|---|---|
| validation, preparation or pure-swap rejection | old tuple exact reference; no save/projection; prior status unchanged |
| `FailedBeforeCommit` | old tuple remains authoritative; status `FailedBeforeCommit`; no projection |
| `CommitOutcomeUncertain` | no tuple assignment; no durable winner is claimed; retained old tuple/pixels are display-only under terminal `CommitOutcomeUncertain` |
| exact durable success | one assignment to the prepared new tuple, then projection |
| projection throws after durable success | new tuple/state remain authoritative; old drawn pixels are non-authoritative; status `ReloadRequired` |

`CommitOutcomeUncertain` and `ReloadRequired` share one closed mutation
predicate. `TrySelect`, `CompletedSnapshots.Observe` and `Highlight` all return
`ReloadRequired` without changing highlight, snapshot slot, tuple, view state,
save count or projection count. Reads remain available and expose the terminal
status. This slice has no in-place recovery method: recovery means discarding
the adapter and constructing a fresh instance only after fresh CIO load by the
later integration owner.

Wrong/stale save revision evidence must respect the concrete CIO boundary. No
test-only detached receipt or alternate save delegate may be introduced.
Tests instead prove that:

- the exact state reference/value staged by selection is the one synchronously
  passed to the concrete `CostumeFileAdapterV1.Save`;
- first and replacement success return the same expected revision and publish;
- a port that makes reopened primary bytes differ, including same revision
  with different state or a wrong revision, yields the CIO's non-success/
  uncertainty result and no CUA publication; and
- reflection/static inspection finds no receipt parameter, result setter,
  delegate or alternative save method on the CUA surface.

This demonstrates the stale/wrong result cannot be injected rather than
creating the forbidden seam solely to test it.

## 5. Snapshot/action and visual-only matrices

The action fixture contains exact action IDs for idle, locomotion, airborne,
landing, bow draw, bow release and bow recovery, with both `left` and `right`
clips. For each action/facing pair, tests observe through
`adapter.CompletedSnapshots.Observe`, then select/swap and assert exact
preservation of tick, action, `ActionAgeQ1000`, facing and anchors plus the
contract's loop/non-loop `0`, interior, `999` and `1000` frame mapping.

Independent negative cases cover:

- selection before the first port observation;
- reflection/static proof that no selection overload accepts a snapshot;
- foreign actor and unknown facing at the port;
- decreasing tick;
- exact duplicate tick as a no-op;
- same-tick conflicts in action, age, facing, anchor X and anchor Y one at a
  time;
- higher-tick action/facing absent from the current media;
- proposed package missing the accepted current action, which must fail pure
  `TrySwap`;
- foreign/stale current/proposed pure binding pairs; and
- destroyed or otherwise drifted current tuple enters terminal recovery rather
  than claiming old-pair preservation; and
- observations and selections after each terminal status.

The `future/uncommitted` negative is the no-bypass proof described above: a
snapshot not observed through the post-commit port has no route into
selection. Tests must not claim the adapter can compare against a simulation
clock it does not own.

For `AC-CUA-005`, create an isolated temporary mechanics probe containing
Transform identity/position/rotation/scale, Rigidbody2D identity and complete
motion/body settings, collider identity/enabled/geometry, hit/hurt/aim/
transfer geometry tokens, stats, cooldowns, abilities, action/target/phase/AI
tokens and simulation-owner call counters. Capture exact before values and
references, run first selection, replacement, accepted observation, rejected
observation and projection failure, then recursively compare every field and
call counter. The adapter is never given the probe. Static inspection also
rejects `Update`, `FixedUpdate`, Animator/time/input/global lookup and mechanics
assembly references.

## Exact implementation and test delta

After Astra approval and Luna re-review authorization, Terra may change only:

- `CostumeUnityMediaPackageV1.cs`: independent binding/completeness checks,
  widened checked bounds and within-clip duplicate-rectangle rejection;
- `CostumeUnityPresentationAdapterV1.cs`: private accepted-observation slot,
  port-only snapshot ingestion, snapshot-free `TrySelect`, mandatory pure
  `TrySwap`, bootstrap rule, exact tuple comparison/publication and common
  terminal mutation guard;
- `CostumeUnityPresentationAdapterV1Tests.cs`: two-package replacement fixture,
  full parameterized validation/save/snapshot/action/mechanics/fault matrices;
- `CostumeUnityViewPresenterV1Tests.cs`: exact public-surface, explicit-port,
  no-global/no-mechanics and terminal-highlight probes; and
- the already allowlisted implementation evidence and independent-review
  records after actual results exist.

`CostumeUnityViewPresenterV1.cs`, both asmdefs/metas, pure costume/presentation
core, CIO, catalog, scenes, prefabs, media, packages, ProjectSettings and all
existing owners should remain byte-identical. Any need to change one is a stop
condition and requires Astra to revise the allowlist before work continues.

The focused PlayMode test set must expose individually named tests for:

1. port-only first snapshot and snapshot-free selection API;
2. bootstrap and replacement `TrySwap` gates;
3. the complete package mutation table;
4. checked maximum-integer bounds and exact-edge geometry;
5. duplicate rectangles within a clip rejected and cross-clip reuse accepted;
6. first/replacement save observation and exact one-reference publication;
7. pre-commit, uncertain-commit, wrong-primary and post-save projection faults;
8. terminal mutation blocking and durable/new authority after projection
   failure;
9. the complete action/facing/frame and monotonic/duplicate/conflict matrix;
10. exact mechanics/physics-owner invariants; and
11. 36 real pending rows, synthetic labels and unchanged highlight-only rules.

Failed, skipped and inconclusive counts must all be zero. After Luna statically
rechecks the exact replacement source/test/meta/asmdef hashes and reports all
P1/P2 closed, run one focused PlayMode result pair followed by fresh unfiltered
full EditMode and full PlayMode result pairs. Astra must approve the exact
previously unused artifact stems before execution. A source/test hash change
invalidates the static gate and all subsequent results.

## Failure, rollback and evidence policy

- Record exact pre-change bytes/SHA-256 for the four authorized source/test
  files and unchanged asmdefs/metas before correction.
- A candidate failure restores only those four files to their recorded bytes;
  do not reset or overwrite unrelated working-tree changes.
- Every test owns a unique validated non-root temporary CIO directory and only
  its own temporary Unity objects. On failure, retain immutable logs/XML but
  remove only fixture-owned disposable data after recording the failure.
- A package/pure-swap/snapshot rejection must occur before save. A
  `FailedBeforeCommit` changes no tuple. An uncertain commit never retries,
  cleans up, promotes or claims rollback. A projection failure never reverts
  the durable/new tuple.
- Any partial portrait/gameplay publication, missing `TrySwap` call path,
  public snapshot-selection bypass, accepted overflow/duplicate, changed
  canonical bytes, mechanics drift, nonzero required test count, or
  allowlist/hash drift is a hard stop.
- The Luna BLOCKED review and every failed static/test artifact remain
  immutable. They are never overwritten or reclassified as success.
- No Unity execution is authorized until Luna re-reviews the exact corrected
  hashes and Astra accepts that pre-execution gate. No implementation evidence
  may say `Verified`; Luna recommends and Astra decides.

## Traceability and decision need

| Closure | Existing requirements | Existing acceptance criteria |
|---|---|---|
| port-only completed observation and monotonic/conflict rules | `REQ-CUA-005/009` | `AC-CUA-004/007` |
| mandatory pure swap and immutable publication | `REQ-CUA-004/005/007` | `AC-CUA-003/004/006` |
| identity/hash/manifest/geometry/reference matrix | `REQ-CUA-003/004/008` | `AC-CUA-002/003/008` |
| replacement, save/projection faults and terminal closure | `REQ-CUA-004/007` | `AC-CUA-003/006` |
| visual-only mechanics/physics preservation | `REQ-CUA-006` | `AC-CUA-005` |
| allowlist and full evidence gate | `REQ-CUA-010` | `AC-CUA-007/009` |

No REQ/AC ID or mapping changes. No user product decision is required. Rejecting
duplicate rectangles is the conservative behavior already permitted by the
contract and is sufficient for synthetic fixtures; it neither chooses a real
animation cadence nor changes a real asset. Supporting authored holds or
connecting a live completed-snapshot owner remains a later separately approved
product/integration slice.

Astra approval is still required before Terra implements this correction, and
Luna must independently close the exact replacement hashes before any Unity
execution.
