# VD-03 M3C2 Regular-Enemy Locomotion Unity Bridge — Implementation Evidence

- Date: 2026-08-30
- Contract: [VD-03 M3C2](../specs/work-contracts/2026-08-30-vd03-combat-m3c2-enemy-locomotion-unity-bridge.md)
- Status: PASS — Terra implementation and Luna independent verification complete; approved for Sol integration
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`

## Implemented boundary

`RegularEnemyLocomotionSimulationDriver` now executes at `-160` after behavior and before default-order player movement. It consumes the exact Combat-owned walker/surveyor roster and exact current behavior/reaction publications, stages unconditional initialization support probes, resolves explicit-position nonalloc environment BoxCasts in walker→surveyor and X→Y order, validates the complete M3C1 pair candidate, commits once, schedules alive bodies with `MovePosition`, and publishes one immutable `CarriedEnemyMotionView`.

The bridge validates the frozen kinematic Rigidbody2D and BoxCollider2D form before preview and immediately before commit. Ordinary validation/query failures preserve the M3C1 session, body positions and prior carried view. Dead identities issue neither queries nor movement calls. Paired Reset enters M3C1 reset preflight, restores frozen spawn/ground seeds without queries, then schedules both spawn positions. Actor contact, damage, denial lines, animation and presentation remain outside M3C2.

## Acceptance trace

| Evidence | Requirement / acceptance trace |
|---|---|
| Exact `-200/-190/-180/-170/-160/default`, zero/nonzero bootstrap and exact upstream horizon failures | `REQ-COM-001`, `REQ-COM-004`; partial `AC-COM-001`, `AC-COM-003` |
| Frozen roster/binding and complete Rigidbody2D/BoxCollider2D authoring mutation matrix | `REQ-COM-004`; partial `AC-COM-003` |
| Wall, ceiling, landing, support, ledge, corner, trigger and all actor-root exclusion cases | `REQ-COM-001`, `REQ-COM-004`; partial `AC-COM-001`, `AC-COM-003` |
| Stable equal-distance selection, duplicate identity, nonfinite hit, invalid normal and 64-hit saturation | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |
| Walker-success/surveyor-failure preservation and body drift rejection | `REQ-COM-004`; partial `AC-COM-003` |
| 60-tick Approach, 12-tick DashActive, 48-tick Relocate and Heavy grounded/airborne traces | `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-005`; affected `AC-WT-002`, `AC-WT-005` |
| Death/repeated-dead, resurrection rejection, query-free reset and terminal tick | `REQ-COM-004`; partial `AC-COM-003` |
| Real `MovePosition → Physics2D.Simulate → next -160` equality and 30/60/144 grouping equivalence | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |

Exact M3C1 arithmetic, candidate equality and remainder semantics continue to be proven by the verified M3C1 EditMode suite; the M3C2 tests exercise their Unity query/body handoff rather than duplicating the pure-core proof.

## Verification results

| Run | Result | Duration | Result artifact SHA-256 |
|---|---:|---:|---|
| Focused `RegularEnemyLocomotionSimulationDriverPlayModeTests` | 55/55 passed | 0.1383978 s | `d9d24874422802c5ca09c8dd3ee5863f459552b9b7b5e4d7878da7445381353f` |
| Combat Unity PlayMode regression | 97/97 passed | — | `866604ec67d043159fba98950c1bcc18b02a64574e1d755d67d6b4103ad15c10` |
| Full project PlayMode regression | 128/128 passed | — | `9baa0e54e77ee0dd4b5dcabe4c21654cbae43a43d3b63879afaaea2e042d86af` |
| Full project EditMode regression | 151/151 passed | — | `813b21a69e901e5e4f350aa8f844fcb04e270dbe0a2f56155d4725dae34da992` |

Unity clears earlier `Temp/` result files on later project runs; the final PlayMode XML contains all 97 CombatUnity cases and all 55 focused M3C2 cases. Luna independently recomputed the surviving full PlayMode and EditMode hashes and confirmed zero failed or skipped cases. `git diff --check` passed.

Source review found no added `Physics2D.SyncTransforms`, temporary Transform/body repositioning for queries, force/velocity locomotion, collision callback, time/RNG, damage or denial-line dependency. Test mutation/counter seams are internal, nonserialized and inert in production.

## Correction and independent review record

The first focused six-case fixture incorrectly expected Approach movement while the walker was already inside the exact DashTelegraph range; Terra corrected the fixture with an explicit moving control and environment-wall control. Luna then rejected integration because six cases did not satisfy the Approved evidence matrix. Terra expanded the suite to 55 focused cases.

The expanded run exposed three issues. Two were fixture problems: nonzero bootstrap lacked the approved player snapshot horizon, and Unity did not retain the attempted kinematic angular-velocity mutation. The fixtures now build the exact bootstrap motor state and test the factored angular invariant deterministically. The third was a runtime defect: generic upstream phase validation excluded the paired Reset enum before reset selection. The validator now admits paired Reset, while mixed/invalid reset shapes still reject through `SelectInput` and M3C1 preflight.

After correction, Luna inspected the runtime, internal test seams, complete focused matrix and final XMLs. Luna returned PASS with P0 `0`, P1 `0`, P2 `0` and recommended Sol integration.

## Ollama utilization ledger

The Approved contract froze these screened outcomes before Terra implementation. No additional cloud call was needed after the contract boundary was fixed.

| Lane | Outcome | GPT screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Terra retained explicit-position X→Y nonalloc casts, pair-wide preflight and focused boundary-fixture ideas. Invented public interfaces, instance-ID keys, post-commit support probing and failure-time identity publication remained rejected. |
| GLM 5.2 | used and accepted in part | Sol/Terra retained saturation, X-resolved Y origin, half-commit, drift, stable-order and render-group mutation cases. Frame-delta injection, implicit rollback and sibling collision assumptions remained rejected. |
| MiniMax M3 | failed and replaced | Its bounded non-thinking fixture request returned empty output without a quota/rate signal. Terra implemented the repeatable validation matrix and Luna reviewed it; no empty output was adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output had no approval or integration authority.

## Scope and preservation

Changed implementation files are limited to the Approved allowlist:

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyLocomotionSimulationDriver.cs` and `.meta`
- narrow internal read-only seams in `CombatSimulationDriver.cs` and `RegularEnemyBehaviorSimulationDriver.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/RegularEnemyLocomotionSimulationDriverPlayModeTests.cs` and `.meta`
- this evidence document and documentation index

Pre-existing user-owned scene/project-setting changes remain outside this unit and were neither staged nor reverted. No stop condition was reached.
