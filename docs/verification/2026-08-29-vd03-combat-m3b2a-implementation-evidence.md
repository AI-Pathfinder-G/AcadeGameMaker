# VD-03 M3B2A Regular Enemy Behavior Core — Terra Implementation Evidence

- Date: 2026-08-29
- Contract: [VD-03 M3B2A](../specs/work-contracts/2026-08-29-vd03-combat-m3b2a-regular-enemy-behavior-core.md)
- Implementation owner: Terra
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Status: implementation, Unity EditMode verification, and Luna independent review complete — PASS with no remaining P0/P1.

## Implemented boundary

- `RegularEnemyBehaviorSession` is an internal, no-engine-reference pair session. It stages/validates walker and surveyor from exact M1/M3A inputs and commits both publications, ordinals and next tick together. Live walker M3A multiplier is revalidated exactly as `IsHeavy ? 1500 : 1000`.
- Walker behavior owns only `Approach → DashTelegraph(18) → DashActive(12) → Recovery(40|60)`, guard projection, the latched dash direction and one immutable `walker` recovery intent for the following tick. It neither submits the candidate to M3B1 nor performs a hit.
- Surveyor behavior owns only `Relocate(48) → FireTelegraph(30) → Fire(1) → Cooldown(60)`, the latched fire target, ordinalled immutable shot intent and M3A-owned fire-lock/forced-descent suppression projection.
- Exact reset/death handling clears local behavior state only. M1/M3A/M3B1/VD-02 sources, Unity adapters, public ABI, settings, scenes and prefabs were not changed.

## Focused verification prepared

| Check | Result | Acceptance mapping |
|---|---:|---|
| New `RegularEnemyBehaviorSessionTests` | PASS: 15 focused cases covering exact 18/12/40/60 and 48/30/1/60 boundaries, candidate/shot ordinals, guard, suppression/residual lock, role-local death/reset, stale/wrong-role/default/invalid/malformed-reset M3A inputs, bidirectional Heavy/multiplier mismatch, pair atomicity, Q1000 extrema, overflow, and 30/60/144 integer render-accumulator replay | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |
| `git diff --check` | PASS | implementation hygiene for `AC-COM-003`, `AC-WT-005` |
| Static source scan of new core | PASS: no Unity/physics, public declarations, floating point, time, RNG or discovery references | contract boundary; `AC-COM-003` |
| Unity Combat EditMode / full project EditMode | PASS: 124/124 passed, 0 failed, 0 skipped, 0 inconclusive in 2.5094845 s. Result: `TestResults/2026-08-29-m3b2a-editmode-124-pass.xml`; SHA-256 `82a81190854183fef195705c37a918f4825afe92cd884772fbfea498aceeadc8` | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |
| Luna independent staged-milestone review | PASS: exact five-file allowlist, cached diff hygiene, source boundary, focused cases and exported XML/hash verified; no remaining P0/P1 | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |

## Verification correction record

- The first full EditMode run reported one M3B2A test failure because `SurveyorSuppressionHasPriorityAndResidualLockRestartsFullRelocate` jumped directly from tick 2 to tick 76 although the session contract requires exact consecutive ticks. The test now processes ticks 3 through 75 and verifies suppression at each tick before checking the tick-76 release.
- That failed run left Unity's native safety state repeatedly asserting `Access version should be odd when acquiring lock`, which cascaded into 16 unrelated legacy failures and stalled recompilation. The affected Unity editor process was restarted without accepting the unrelated URP material-upgrade prompt; the clean rerun passed all 124 EditMode tests.

## Ollama utilization record

| Lane | Outcome | Terra screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted clone/stage/validate-both/commit and focused boundary/atomic/suppression/ordinal/reset/horizon test patterns. Rejected concrete pseudocode that invented shared phase/interfaces, coupled enemy deaths, owned Q1000 position, emitted at the wrong boundary, used forbidden multipliers, skipped reset publication or swallowed overflow. |
| GLM 5.2 | used and accepted in part | Accepted the contract-forming death/reset, stale-view, overflow and residual-lock failure classes. Rejected any concurrent-phase/global-rollback inference, per the Approved contract record. |
| MiniMax M3 | used and accepted in part | Accepted the engine-free replay/fixture direction and exact intent identity coverage. Rejected `uint64` time, death-to-recovery and compressed-tick proposals, per the Approved contract record. |

No cloud lane received repository text, local paths, credentials, personal data or secrets. Cloud output did not alter public ABI or approval authority.

## Integration allowlist and working-tree separation

- The M3B2A staged change set contains only the runtime core, its Unity metadata, the focused EditMode tests, their metadata, and this evidence record.
- `Assets/Scenes/MovementSandbox.unity` and `ProjectSettings/SceneTemplateSettings.json` were already modified or untracked before the M3B2A Unity verification restart. They are user-owned, outside this contract, and remain unstaged.
- Reopening Unity generated project-setting churn. `ProjectSettings/PackageManagerSettings.asset` was removed again; the remaining `ProjectSettings/URPProjectSettings.asset` diff is an unstaged terminal-newline-only difference and is excluded from M3B2A integration. The URP material-upgrade prompt was dismissed without approval.

## Deferred integration risk

- The added behavior core is intentionally not bound to Unity: a later Approved M3B2B adapter must convert the immutable recovery intent to M3B1's Unity-internal candidate and bind movement/shot outputs without changing this core's phase ownership.
