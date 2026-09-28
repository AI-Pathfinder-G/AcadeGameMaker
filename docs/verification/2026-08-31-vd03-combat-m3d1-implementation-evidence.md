# VD-03 M3D1 Deterministic Regular-Enemy Threat Core — Implementation Evidence

- Date: 2026-08-31
- Contract: [VD-03 M3D1](../specs/work-contracts/2026-08-31-vd03-combat-m3d1-regular-enemy-threat-core.md)
- Status: PASS — Terra implementation and Luna independent verification complete; approved for Sol integration
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`

## Implemented boundary

The engine-free `RegularEnemyThreatSession` now converts exact M3B2A behavior and M3C1 motion pairs into immutable next-tick threat candidates. Walker contact uses exact relative swept AABB intersection over closed `[0,1]` with widened signed arithmetic and `BigInteger` rational comparison. The Surveyor owns one vertical `0.5u` denial strip for exact `[spawnTick, spawnTick+30)` lifetime, including post-death persistence, and tests the player's closed swept X interval.

The session owns previous logical centers, three death latches, encounter-local shot ordinal, active-line state, latest immutable snapshot and next expected tick. Preview is mutation-free; commit recomputes and compares the complete snapshot and every request before replacing state. Reset clears line, latches, ordinal and sweep history without inventing a cross-encounter center.

`DamageKind.EnemyRanged=6` is the only public ABI extension. Existing enum values, M1 sort/dedupe/health/invulnerability and result semantics are unchanged. Requests use exact string IDs and target Combat tick `sourceTick+1`; no queue or Unity integration is introduced in M3D1.

## Acceptance trace

| Evidence | Requirement / acceptance trace |
|---|---|
| Static, boundary, near-miss, single/opposed movement, diagonal, parallel and extreme-coordinate swept contact | `REQ-COM-001`, `REQ-COM-002`; partial `AC-COM-001` |
| Every alive walker phase, sustained per-tick contact IDs and dead/reset suppression | `REQ-COM-005`; partial `AC-COM-001`, `AC-COM-003` |
| Line spawn/tick-29/tick-30, strip boundary/miss/crossing and simultaneous request order | `REQ-COM-002`, `REQ-COM-005`; partial `AC-COM-001`, `AC-COM-003` |
| Exact Fire snapshot/intent ordinal relation and full malformed publication matrix | `REQ-COM-004`; partial `AC-COM-003` |
| Suppressed/Heavy/dead post-shot persistence, repeated deaths and resurrection rejection | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |
| Reset from active line/nonzero ordinal/all latches and post-reset static reseed | `REQ-COM-004`; partial `AC-COM-003` |
| Delivery and deadline overflow, forged snapshot/request candidate and complete state preservation | `REQ-COM-004`; partial `AC-COM-003` |
| Mixed `EnemyRanged` canonical M1 ordering and complete echoed result fields | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |
| 30/60/144 logical grouping equality and defensive request-copy isolation | `REQ-COM-004`; partial `AC-COM-003` |

## Verification results

| Run | Result | Result artifact SHA-256 |
|---|---:|---|
| Focused `RegularEnemyThreatSessionTests` EditMode | 28/28 passed | `9da639a777e68e6cbfce1757a0d5bf3697d5e93c8d4e70157e80772c7f736a57` |
| Combat EditMode regression | 63/63 passed | `f9a94aab3686381b337e0d22ae598cdb6c762f8f16940850d8fcee1f1e894c8b` |
| Full project EditMode final | 179/179 passed, 0 skipped, 1.6397506 s | `adb4df82e8cea4b5dd7c69831d523081461ffdc26f79de9b245c3b158cd4e0eb` |

The final full XML contains all 28 Threat cases. Luna independently recomputed its count, duration and hash. `git diff --check` passed. Source review found no Unity reference, float/double/decimal simulation, wall-clock/RNG, discovery API or mutable collection escape.

## Equal-X compatibility addendum execution

M3D2 executable integration proved that approved M3B2A can publish walker direction `0` at exact equal X and approved M3C1 carries that value through `DashTelegraph` and `DashActive`. Under the separately approved addendum, M3D1 now accepts the complete upstream `-1/0/1` domain in only those two phase checks. Existing static/swept geometry remains the sole contact authority and `±2` still rejects atomically.

The expanded focused M3D1 suite passed 31/31 with SHA-256 `e877db172698a2faf046d7fa5070ee089aab048cf113deb7d825576acb79eb62`. Full project EditMode passed 185/185 with SHA-256 `f9a167d7ffa74cd6b3a511f9330a6a7d7f664df7f5fea7910dae5ca070173650`. Luna independently returned PASS with P0 `0`, P1 `0`, P2 `0` for the compatibility change and M3D2 integration.

## Luna correction record

Luna's first implementation pass found no runtime defect but rejected integration because the original 20 focused tests did not directly prove the entire Approved evidence matrix. Terra added eight focused cases covering mixed enum ordering/result echoes, sustained contact IDs, the complete shot mutation matrix with full state preservation, both Suppressed shapes, repeated death/resurrection, meaningful reset clearing, the exhausted source horizon and forged request content.

After the suite reached 28 focused and 179 total EditMode cases, Luna re-reviewed the code, test assertions and final XML and returned PASS with P0 `0`, P1 `0`, P2 `0`.

## Ollama utilization ledger

| Lane | Outcome | GPT screening disposition |
|---|---|---|
| GLM 5.2 | used and accepted in part | Retained spawn/death ambiguity, next-tick latency, invulnerability-boundary, simultaneous-threat and reset-leak mutations. Rejected assigning M1 or Movement authority to M3D1. |
| Kimi K3 | used and accepted in part | Retained immutable candidate, preflight/commit, stable identity and explicit lifetime ideas. Rejected unsigned ticks, numeric IDs, X-only walker contact, one-tick line and queue ownership. |
| MiniMax M3 | failed and replaced | Returned empty non-thinking text with no quota/rate error. Terra implemented the repeatable matrix and Luna independently reviewed it; no empty output was adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output had no approval or integration authority.

## Scope and preservation

Changed implementation files are limited to:

- `Assets/AcadeGameMaker/Runtime/Combat/DamageContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyThreatSession.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/CombatSessionTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyThreatSessionTests.cs` and `.meta`
- this evidence document and documentation index

Pre-existing user-owned scene/project-setting changes remain outside this unit and were neither staged nor reverted. M3D2A still owns Combat delivery queuing; M3D2B still owns the `+100` Unity bridge and presentation remains later work.
