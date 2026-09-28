# VD-03 M3B2B Regular Enemy Behavior Unity Bridge — Terra Implementation Evidence

- Date: 2026-08-29
- Contract: [VD-03 M3B2B](../specs/work-contracts/2026-08-29-vd03-combat-m3b2b-behavior-unity-bridge.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Status: PASS — Unity reruns and Luna independent verification complete; approved for Sol integration.

## Implemented boundary

- New internal `RegularEnemyBehaviorSimulationDriver` runs at `-170`, reads only explicit player/combat/reaction bindings and Combat-owned ordinal-sorted frozen roster, stages Q1000/LOS in walker→surveyor order, then publishes only an immutable behavior pair value. Missing, stale or future result-dependent upstream publications are rejected before first-session creation or any later behavior/queue/carried-view mutation.
- M3B2A now has a mutation-free `PreviewNext`. The bridge previews, validates, preflights a `t+1` recovery conversion, commits the behavior session once, commits the prepared M3B1 queue entry, then replaces the carried behavior view.
- Combat exposes presence separately from its latest outcome plus a frozen-roster read seam. M3B1 exposes a production carried-view seam and a preflight/prepared-commit recovery handoff.
- No body movement, new synchronization, damage request, projectile/denial line, component public API, scene, prefab or project-setting change was made.

## Focused verification prepared

| Check | Result | Acceptance mapping |
|---|---:|---|
| Full project PlayMode | PASS: 73/73, 0 failed, 0 skipped, 0.152431 s. `TestResults/2026-08-29-m3b2b-playmode-final.xml`; SHA-256 `7c80d395bc12558cb319d04f7ab0fa889a7bbe74250101d18fd18e1ce26efce1`. Coverage includes `-170` ordering; bootstrap/t-1 horizon; missing/stale/future M1/M3A preservation; exact reset/restart; non-`t+1` recovery rejection; queue-before-movement; active shared-aim single sync; authored LOS; and behavior-inclusive 30/60/144 replay. | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |
| Full project EditMode regression | PASS: 124/124, 0 failed, 0 skipped, 1.6702611 s. `TestResults/2026-08-29-m3b2b-editmode-final.xml`; SHA-256 `0e09fda7934f062220dc2c84e092fa5732280a20df75d731e0acf0c1a1db648c`. | `AC-COM-003`, affected `AC-WT-005` |
| `git diff --check` and new-file whitespace checks | PASS | `AC-COM-003`, `AC-WT-005` hygiene |
| Static source scan | PASS: new bridge has no public declaration, `Physics2D.SyncTransforms`, body movement, damage/projectile API, time/RNG or discovery API | contract boundary; `AC-COM-003` |

## Correction and rerun record

- Sol source review found one compile-time internal method-name mismatch in the legacy `SubmitRecovery` wrapper and corrected it to call the approved prepared-commit seam. Unity then compiled all runtime and test assemblies successfully.
- The first PlayMode run completed 69/70. The sole failure was a test oracle error: the authored default rig places walker and player at the same Q1000 X with open LOS, so M3B2A correctly publishes `DashTelegraph` on the first tick rather than `Approach`. Sol corrected only that expected phase.
- Luna then found four P1 gaps: first-failure session creation, reset integration, post-success upstream preservation, and active shared-aim/LOS coverage, plus an ordinal-roster ambiguity. Terra deferred session creation until exact M1/M3A/observation preflight succeeds, added the requested bridge cases, and validates ordinal roster order. The earlier rerun counts and hashes were discarded; only the final reruns above are evidence.
- Sol corrected the new LOS integration test setup so its bootstrap tick remains in `Approach`: walker begins outside dash range, then moves into range on the active shared-aim tick. This lets the test distinguish open self-root exclusion from a third-party layer-8 blocker without carrying a prior `DashTelegraph` state.
- The corrected implementation and test set passed the final full PlayMode and EditMode reruns recorded above.

## Ollama utilization record

| Lane | Outcome | Terra screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted explicit references, stage/validation and stale/duplicate guard patterns. Rejected its phase reorder, pure-planner Transform ownership, reset/clock changes and publish-before-queue design. |
| GLM 5.2 | used and accepted in part | Accepted contract-forming spatial baseline, lifecycle and atomic-handoff risk classes. A post-implementation gap sweep confirmed tick-order, no-extra-sync and exact-`t+1` mutation targets already covered by tests/source scan. Rejected null snapshot (value struct), role unfreezing/third enemy (frozen fixed pair), mutable publication (readonly value types), and its claim that physics-query LOS is invalid; the Approved bridge intentionally reuses the authored physics LOS evaluator. |
| MiniMax M3 | failed and replaced | The contract records two empty bounded fixture results. GPT-authored focused preview/queue and render-grouping coverage replaces them. |

## Remaining risks

- The bridge deliberately publishes only intents. Movement, contact damage, projectile/denial-line lifetime and presentation remain for later Approved work.
- Luna independently reviewed the exact seven-file staged allowlist and reported PASS with no P0, P1 or P2 findings. Luna verified atomic failure paths, phase order, tick horizons, frozen roster, shared-aim/LOS behavior, reset semantics, runtime boundaries, XML counts and both recorded hashes, and explicitly authorized Sol integration.
