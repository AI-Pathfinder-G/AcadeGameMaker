# VD-03 M3C1 Deterministic Regular-Enemy Locomotion Core — Terra Implementation Evidence

- Date: 2026-08-30
- Contract: [VD-03 M3C1](../specs/work-contracts/2026-08-30-vd03-combat-m3c1-enemy-locomotion-core.md)
- Status: PASS — Terra implementation and Luna independent verification complete; approved for Sol integration
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`

## Implemented boundary

`RegularEnemyLocomotionSession` is an engine-free Combat-internal Q4096 owner for the literal walker/surveyor pair. It provides mutation-free `PreviewNext`, recomputed immutable resolved candidates, field-for-field candidate validation and one pair-atomic commit. Encounter reset has a separate collision-free preflight/commit transaction. No Unity type, physics query, body movement, damage request, denial-line lifecycle, public ABI, floating-point simulation, time source, RNG or dynamic discovery was introduced.

The fixed math follows the Approved order: Heavy Q1000 multipliers are rounded away from zero without retained multiplier remainder; acceleration and position alone retain canonical signed `/60` remainders; updated vertical velocity precedes position integration; fall-speed clamp clears gravity remainder only. Both roles read the prior committed pair, and collision resolutions must echo the complete prediction identity and remain inside the integer sweep envelope.

## Acceptance trace

| Evidence | Requirement / acceptance trace |
|---|---|
| Exact first/consecutive/stale/future/default/terminal tick tests | `REQ-COM-004`; partial `AC-COM-003` |
| 60-tick Approach, 12-tick DashActive and 48-tick Relocate displacement/remainder tests | `REQ-COM-001`, `REQ-COM-002`; partial `AC-COM-001` |
| Heavy grounded `9216`, airborne `3226`, gravity `180224`, apply/clear and fall-clamp tests | `REQ-COM-002`, `REQ-COM-005`; affected `AC-WT-002`, `AC-WT-005` |
| Complete reachable walker and surveyor phase/intent/reaction rows plus invalid mutations | `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-001`, `AC-COM-003` |
| Preview/candidate equality, forged candidate and malformed second-role atomicity | `REQ-COM-004`; partial `AC-COM-003` |
| Sweep envelope, truncation flag, stationary support, downward block and axis-specific cleanup | `REQ-COM-001`, `REQ-COM-004`; partial `AC-COM-001`, `AC-COM-003` |
| Death identity, repeated inert death, resurrection rejection and exact spawn/seed reset | `REQ-COM-004`; partial `AC-COM-003` |
| Checked position boundary, inactive-axis avoidance and 30/60/144 trace equality | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |

M3C1 remains partial substrate. Unity cast/support truth and actual body movement remain M3C2; contact damage and surveyor denial lines remain later M3D contracts. This evidence does not close any listed acceptance criterion by itself.

## Verification results

| Run | Result | Duration | Result artifact SHA-256 |
|---|---:|---:|---|
| Focused `RegularEnemyLocomotionSessionTests` EditMode | 27/27 passed | 0.0427417 s | `ee7d19a08e49f7d10cab034a08243183dcb631be7a8393713c54e2701f50d6d3` |
| Combat EditMode regression | 58/58 passed | 0.067461 s | `382c77202fea607e2481e64caeff777c5d51ca2d84e8130eb833a4a2d0849fb3` |
| Full project EditMode regression | 151/151 passed | 1.6688556 s | `2330ef7688e3f0e77c71929d77c51e16bcf6ab20b5690084b0c55852c57ecd53` |

Result files are local test artifacts under `TestResults/` and are not part of the implementation allowlist. `git diff --check` passed. A source scan of the new runtime file found no `UnityEngine`, public surface, floating point, wall-clock/RNG, damage or dynamic-discovery dependency. The Combat assembly retains `noEngineReferences: true`.

## Luna P1 correction record

Luna's first implementation pass found no execution or hash failure but rejected integration for two evidence/correctness gaps. Reset preflight checked only phase, movement and direction, so a forged reset could retain non-reset alive/end/guard/telegraph/latch/ordinal or pair-intent fields and still clear locomotion state. It now requires the exact complete M3B2A reset pair shape before either role mutates, with field and intent mutations proving full pair preservation.

The tests now also submit an exact terminal `t=int.MaxValue` malformed input, clear a previously nonzero X remainder through aligned zero-direction DashActive, compare every carried motion field after failed transactions, and reject opposite/excess/unmoved-axis travel, duplicate role identity and invalid stationary-support variants. The three final reruns above supersede all earlier M3C1 XML counts and hashes.

Luna independently re-reviewed the exact five-file staged allowlist and all three final XML artifacts. Luna confirmed complete reset-shape validation and failure preservation, terminal tick, nonzero-remainder zero dash, collision/support mutation coverage, fixed math, pair atomicity and forbidden-surface boundaries, and reported PASS with no P0, P1 or P2 findings. Sol integration is authorized.

## Ollama utilization ledger

No additional cloud-model call was made during implementation; the Approved contract froze the following screened outcomes and instructed Terra to record them unchanged.

| Lane | Outcome | Terra screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Retained the two-stage pair transaction, checked fixed-point carries and atomic boundary-test ideas. Physics casts, invented public types and failure-time zero publication remain rejected. |
| GLM 5.2 | used and accepted in part | Retained drift, half-commit, ordering, multiplier and render-group mutation cases. Inter-enemy current-state dependency and Q4096 bit-shift multiplier remain rejected. |
| MiniMax M3 | failed and replaced | The empty non-thinking response supplied no usable fixture. Terra implemented the Approved repeatable matrix directly; no retry or model output was adopted. |

No cloud model received repository text, local paths, credentials, personal data or secrets. These outcomes confer no approval or integration authority.

## Scope and preservation

Changed implementation files are limited to the contract allowlist:

- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyLocomotionSession.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyLocomotionSessionTests.cs` and `.meta`
- this evidence document

Pre-existing user-owned scene/project-setting changes remain outside this unit and were neither staged nor reverted. No stop condition was reached.
