# VD-03 M4B3B2 Ordan Hostile Geometry Implementation Evidence

- Result: PASS / Verified
- Date: 2026-09-07
- Independent reviewer: Luna (`P0=0`, `P1=0`, `P2=0`)
- Contract: [M4B3B2 Ordan Hostile Geometry and Next-tick Player Damage](../specs/work-contracts/2026-09-06-vd03-combat-m4b3b2-ordan-hostile-geometry.md)
- Requirements: `REQ-COM-002`, `REQ-COM-003`, `REQ-COM-004`; affected `REQ-WT-003`, `REQ-WT-005`
- Acceptance evidence: `AC-M4B3B2-001` through `AC-M4B3B2-008`

## Implemented boundary

- M4A publishes an immutable Seizure trajectory projection only during Execute, with exact stage/acceleration duration validation and preview/commit equality.
- One engine-free `OrdanBossHostileDamageProducer` runs at `-170`, after Transfer, Combat, bridge and exposure scheduling and before player movement. It reads only completed value publications and a tick-0 seed or exact prior-tick movement snapshot.
- The complete Q4096 arena, player, Ordan, Debt, Seizure, Audit and three-occluder descriptor is immutable, defensively exposed for validation, and checked by the authoring validator.
- Closed integer AABB/annulus contact and checked rational slab occlusion create at most one exact `t+1` player `DamageRequest`; Combat remains the sole health, ordering, dedupe, invulnerability and death owner.
- The Combat pending batch permits one producer-private atomic append, including an empty append. Malformed IDs/fields, wrong horizon, foreign producer, forged candidate, stale batch and second append preserve the prior batch and reject.
- The `+110` handoff copies only the immutable hostile digest. Full geometry observations stay producer-private and are used only by focused verification and cadence comparison.

## Verification results

| Run | Result | Acceptance IDs | SHA-256 |
|---|---:|---|---|
| Focused EditMode `OrdanBossSessionTests` + `OrdanBossHostileGeometryTests` | 49/49 | `AC-M4B3B2-001`, `002`, `003`, `007` | `BA4B9FB735A30B76C843EEE5EDAD91A7F57C935B9962DE410B225031082337D3` |
| Focused authored PlayMode `OrdanBossEncounterAuthoredGraphPlayModeTests` | 10/10 | `AC-M4B3B2-003` through `008` | `8338482DA0BB6BC97761C344A31ABFB549D57CE3F94540EA50398A98BA950E69` |
| Full EditMode regression | 397/397 | regression support for `AC-M4B3B2-001` through `008` | `05684F62DDE4376E4D6BACB6B9554F5BFD6E6BC7BE1169631AD933A64C9483E0` |
| Full PlayMode regression | 330/330 | regression support for `AC-M4B3B2-001` through `008` | `D96281C589B81DE2E1E7F44493C747F91A837908872B438D97ACB3A2496BA9F6` |

The authored cadence test uses fresh graphs and the exact 30/60/144 render grouping over 500 fixed ticks. Matching-tick signatures include producer geometry, hostile digest, pending delivery batch, canonical Combat results, both health values, M4A forecasts, exposure outcomes, lifecycle/terminal state and stable IDs. No first-divergent tick was observed.

## Static and asset checks

- The hostile producer contains zero `Physics2D`, cast/raycast/overlap, collision/trigger callback, Transform, Rigidbody or movement calls.
- Execution order remains `-200 Transfer -> -190 Combat -> -185 bridge -> -180 exposure scheduler -> -170 hostile producer -> default player movement -> +110 handoff`.
- Scoped `git diff --check` passes for the implementation allowlist.
- The approved builder-generated boss prefab and sandbox scene contain exactly one producer on `Systems`; the validator checks its complete descriptor and binding graph.

## Review corrections

Luna's first implementation review found three P1 issues: exact projection duration was not constructor-validated, the authoring validator checked only part of the geometry descriptor, and the focused Play/cadence matrix omitted several hostile paths and private observations. The implementation added the exact duration gate, full immutable descriptor validation, normal/accelerated Seizure contact, uninterrupted/interrupted Audit cases, death/lifecycle/terminal and four-ID independence cases, append-owner rejection cases, and geometry/pending-batch cadence fields. Luna's re-review closed all three findings and returned `PASS`, `P0=0`, `P1=0`, `P2=0`.

## Ollama utilization record

| Lane | Outcome | GPT screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted owner-order, quantization, occlusion drift, duplicate identity and lifecycle/terminal failure modes. Host-engine ID recycling was rejected as outside the authored-ID contract. |
| Kimi K3 | used and rejected | Its generic geometry proposal invented a 1/16 grid, substep sweep and fractional interpolation. Terra/Sol replaced it with the Approved Q4096 and rational-slab contract. |
| MiniMax M3 | used and accepted in part | Retained table-driven fixtures, explicit oracle outputs and first-divergent-tick reporting; rejected PRNG, wall-clock, visual/audio and invented-origin fields. |

No cloud model received repository files, local paths, credentials, personal data or secrets. Luna screened verification evidence and Sol alone accepted integration.

## Unclaimed work

BalanceAudit pull movement remains M4B3B3. Reward/room consumption, encounter teardown, scene transition and persistence remain M4B3C or later. M4B3B2 does not claim those behaviors.
