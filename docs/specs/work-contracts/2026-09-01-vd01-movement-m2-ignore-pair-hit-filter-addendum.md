# Work Contract Addendum: VD-01 M2 Ignored-Pair Cast-Hit Filtering

- Status: Approved — Sol, after Luna pre-gate PASS (`P0=0`, `P1=0`), 2026-09-01
- Owner and integration authority: Sol
- Unit design and implementation: Terra
- Independent verification: Luna
- Owning contract: `docs/specs/work-contracts/2026-08-25-vd01-movement-sandbox.md`
- Triggering dependent contract: `docs/specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md`
- Requirements: `REQ-MOV-001`, `REQ-MOV-004`, `REQ-MOV-006`; affected `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`
- Partial acceptance evidence only: `AC-MOV-001`, `AC-MOV-004`, `AC-MOV-005`; affected `AC-COM-001`, `AC-COM-003`

## Problem and narrow decision

Unity 6000.3.21f1 reports `Physics2D.GetIgnoreCollision(playerCollider, enemyCollider)==true` after exact pair mutation, but the existing attached `CapsuleCollider2D.Cast` with `ContactFilter2D.NoFilter()` still returns that enemy. The verified `PlayerMovementController` consequently selects the ignored enemy as ground/wall/dash/axis-resolution truth. M3E1A measured this in both regular-enemy directions and the environment-preservation path; production pair-ignore integration is stopped.

Movement owns conversion of raw Unity cast hits into authoritative movement contacts and collision resolution. This addendum therefore permits one exact behavior change inside the existing hit-selection loop: after rejecting a null collider, the attached player collider itself and triggers, reject a hit when `Physics2D.GetIgnoreCollision(_collider, hit.collider)` is true. The existing caller predicate, Q4096 distance conversion, stable ordering, cast count, buffer, directions, distances, origins, temporary query pose and synchronization behavior remain unchanged.

This is an explicit raw-result filter, not an assumption that Unity's Cast honors pair-ignore state. It applies uniformly to the existing downward ground probe, left/right wall probes, dash obstruction probe and X/Y resolution casts because all use `FindBestHitAt`. It creates no second motion, contact, health, death or lifecycle authority.

## Exact implementation contract

`PlayerMovementController.FindBestHitAt` keeps the exact existing sequence:

1. construct the current `ContactFilter2D`, call `NoFilter()`, then set `useTriggers=false`;
2. retain the existing query-only temporary pose and existing `Physics2D.SyncTransforms` calls only when the origin differs;
3. issue the existing attached-collider Cast into the existing 32-entry buffer with the same direction and distance;
4. for each raw hit in returned order, skip null, the attached collider and triggers exactly as before;
5. then call `Physics2D.GetIgnoreCollision(_collider, hit.collider)` exactly once for that surviving raw hit and skip it when true;
6. then apply the existing caller predicate and unchanged Q4096/stable-authored-ID candidate ordering.

No `IgnoreCollision` mutation occurs in Movement. M3E1's later approved `+110` handoff remains the only owner allowed to change the two exact player/regular-enemy pair flags, after the player's completed source tick so the new filter first affects movement at `t+1`. Movement simply observes current pair truth while selecting a hit. Returning a pair to false makes that collider eligible on the next Movement query; M3E1 itself permits no same-instance post-death restoration.

The raw cast result count must remain below 32 in every M3E1 authored and replay fixture so this addendum never claims that filtering can recover a nonignored hit truncated by Unity. This addendum does not change the frozen M2 buffer or invent a new saturation failure policy. The later M3E1 B0/B1 authoring validator and evidence must prove the exact graph is unsaturated for every exercised probe/resolution direction.

## Preserved invariants

- no public type/member, serialized field, assembly reference, execution order, project setting, layer or collision-matrix change;
- no collider disable/destroy, trigger conversion, body unsimulation, root deactivation or teleport fallback;
- no new cast, overlap, raycast, callback, `SyncTransforms`, wall-clock, render-delta or RNG dependency;
- ignored colliders remain geometrically unchanged and available to Combat, Transfer, Reaction, Behavior, Locomotion and Threat validation;
- nonignored actors and authored environment retain identical blocking, Q4096 quantization and stable ordering;
- identical fixed-tick inputs and pair-state transitions produce identical complete logical traces under 30/60/144 render grouping;
- protected user files remain unchanged and unstaged.

## Required evidence

Focused PlayMode evidence must use the real `PlayerMovementController`, its exact attached vertical capsule and nontrigger kinematic walker/surveyor boxes:

1. baseline raw and production paths include each selected role in separate fresh fixtures;
2. exact pair true leaves the collider in a direct raw Cast oracle but removes it from production Movement contact/resolution, proving the new filter rather than engine behavior;
3. the opposite nonignored enemy and environment wall/ground remain eligible and blocking;
4. returning the exact pair false restores eligibility on the next query, while destroying the fixture and creating new collider references starts false;
5. ground, wall, dash obstruction and X/Y resolution each exercise the shared filter without adding another query or sync;
6. ignored-state M3C2/M3D2 geometry revalidation and immutable publications remain successful and identical to a nonignored control where pair state is not part of those views;
7. every direct raw Cast in the M3E1 fixtures reports count below 32;
8. a nonvacuous trace changes pair state at the same logical tick and proves tick-by-tick equality under 30/60/144 render grouping;
9. source scans confirm only the allowlisted filter branch changed and no forbidden surface appeared;
10. focused Movement, focused M3E1A, relevant M3C2/M3D2, full EditMode and full PlayMode suites pass with XML counts, failures/skips and SHA-256 recorded against the cited AC IDs.

## Allowlist

- `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs`: the single ignored-pair raw-hit filter only;
- focused existing or new Movement/CombatUnity PlayMode tests and `.meta` files;
- this addendum, its pre-gate, implementation evidence and documentation index.

## Denylist and stop conditions

No M3E1 scene/prefab, `+110`, lifecycle, sink or production pair mutation may be implemented under this addendum. No change to M1 motor, command consumption, Q4096 math, skin/probe/normal constants, cast buffer, cast overload, filter mask, predicate semantics, stable ordering, collider geometry, public ABI, assemblies, packages or project settings is permitted.

Stop and report to Sol if the explicit `GetIgnoreCollision` filter does not make all Movement query paths respect the exact pair, if raw buffer saturation occurs in the authored fixture, if a nonignored/environment hit changes, if another physics query/sync or public/cross-assembly seam appears necessary, or if any existing Movement/M3C2/M3D2 regression fails.

## Ollama utilization record

| Lane | Outcome | Sol screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Retained the minimal post-Cast `GetIgnoreCollision` filter and selected/nonignored/environment/order test decomposition. Rejected the inaccurate claim that Cast already respects the collision matrix, invented 600-frame scope and incomplete lifecycle/atomicity wording. |
| GLM 5.2 | used and accepted in part | Retained ground/wall/dash/axis, external-toggle, fresh-instance, environment and cadence mutation classes. Rejected unsourced one-way-platform, multiplayer push, substep and presentation assumptions outside the frozen project. |
| MiniMax M3 | failed and replaced | Two bounded non-thinking requests returned empty final content while consuming their entire output in hidden thinking; no quota/rate/auth/model/network error occurred. Terra supplies fixtures inside this contract and Luna reviews them; no empty output is adopted. |

No cloud model received repository text, local paths, credentials, personal data or secrets. Ollama output has no decision, approval, merge or canon authority.

## Approval gate

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this addendum to `Approved`. Passing this addendum reopens only the M3E1A collision feasibility retry; M3E1 B0/B1 remain gated by the staged M3E1 contract.
