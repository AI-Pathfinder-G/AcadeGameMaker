# VD-03 M3B1 Reaction Unity Adapter — Terra Implementation Evidence

- Date: 2026-08-29
- Contract: [VD-03 M3B1](../specs/work-contracts/2026-08-29-vd03-combat-m3b1-reaction-unity-adapter.md)
- Implementation owner: Terra
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`
- Status: Verified and approved for integration by Sol after Luna independent review.

## Implemented boundary

- `TransferSimulationDriver` publishes its last successful immutable exact-tick outcome only after transfer processing and final player-modifier reflection, together with a camera/device-free consumed-intent summary.
- `CombatSimulationDriver` requires the explicit reaction-driver binding, invokes its structural preflight before M2A/M1 mutation, rejects any non-first-tick missing/stale carried view, uses the carried `t-1` walker view to form attackability, and retains a private complete-tuple sorted request trace aligned by index with M1 results.
- `RegularEnemyReactionSimulationDriver` runs at `-180`, accepts only explicit bindings, validates the frozen `walker` (`9/0/true`) and `surveyor` (`6/0/true`) co-authored roster, projects exact transfer edges, arbitrates ordinal-first applied walker `HeavyImpact`, and invokes `ValidateNext` on both M3A sessions before either commits.
- Recovery candidates are future-only, unique by both tick and encounter cycle ID, and tombstoned when consumed, including same-tick impact/death suppression. Reset is restricted to the exact idle transfer/combat tick and clears adapter queues only after both M3A resets validate and commit.
- `RegularEnemyReactionSession.ValidateNext` replays the same checked state-machine logic on an isolated clone, so successful validation has no observable mutation.
- Repair pass: recovery cycle IDs are encounter-unique across queued, consumed and tombstoned states; high-tick tests seed the required completed `t-1` player pose through the existing motor test seam; surveyor baseline recall keeps its exclusive residual fire-lock end.
- GUI-failure repair: same-tick `TargetRemoved → apply` and manual recall tests now wait for the existing VD-02 apply lock expiry at tick 22. This preserves, rather than alters, VD-02 lock semantics while proving the M3B1 projection/residual-lock requirements.

## Focused verification performed

| Check | Result | Acceptance mapping |
|---|---:|---|
| `git diff --check` | PASS | implementation hygiene for `AC-COM-003`, `AC-WT-005` |
| Source scan: new reaction adapter has no public declarations, discovery API, time/RNG, or physics sync | PASS | contract boundary; `AC-COM-003` |
| Source scan: the only `Physics2D.SyncTransforms` in the touched phase drivers remains transfer `-200` | PASS | affected `AC-WT-005` |
| Focused M3B1 tests added: `ValidateNextMatchesProcessWithoutChangingObservableState`, carried `t-1` gate, recovery cycle uniqueness, exact 30/90 carried walker windows, surveyor fire-lock/basic-attack separation, duplicate-ID canonical provenance/arbitration, reset non-idle/current-recovery/raw-Heavy/stale-outcome rejection, startup binding/roster rejection, same-tick walker-clear/surveyor-apply revisions, live high-tick overflow, and high-tick Invulnerable/TargetDead/lethal suppression | PASS as part of the full suites below | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |
| Full Unity PlayMode suite, Unity 6000.3.21f1 | PASS — 65/65, 0 failed, 0 skipped; [result XML](../../TestResults/2026-08-29-m3b1-playmode-65-pass.xml), SHA-256 `b8d679d609fe95b2285bd7368c9858490b44aeea03b1ccc090e06937ce6b54b8` | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |
| Full Unity EditMode suite, Unity 6000.3.21f1 | PASS — 109/109, 0 failed, 0 skipped; [result XML](../../TestResults/2026-08-29-m3b1-editmode-109-pass.xml), SHA-256 `c8402c62ea64ce353de85ce816610b60f587c750ed13324446f4cf5027e8c6c8` | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005` |

Both result files were exported from Unity Test Runner after restarting the editor so the latest source and test assemblies were compiled. The counts and suite result were independently parsed from the exported XML, and the hashes above identify the exact evidence files.

## Ollama utilization record

| Lane | Outcome | Terra screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted Sol-screened validate-both-before-process, recovery tombstones, and index-aligned canonical trace. Rejected its invented orchestrator/reset enum and phase reordering. |
| GLM 5.2 | used and accepted in part | Post-implementation redacted scenario-gap sweep completed. Sol retained the warning that a combined test helper could mask the `-190/-180` boundary and added an explicit combat-only seam/test. Sol rejected unsupported claims about Unity fixed-tick skipping, `readonly struct` reset corruption, main-thread concurrency and static tombstone leakage. |
| MiniMax M3 | used and accepted in part | Accepted its test cases for same-tick clear+apply, busy reset, duplicate IDs and render replay. Rejected silent Ignore for missing/stale state, nonfatal alignment mismatch, altered revision rules and broad tombstone intervals. |

No cloud lane received repository text, local paths, credentials, personal data or secrets. Their material did not change public ABI, approval or integration authority.

## Remaining verification risks

- Focused high-tick coverage now proves phase-local overflow for a live walker `HeavyImpact` at `int.MaxValue-89` and a live surveyor Heavy edge at `int.MaxValue-74`, plus duplicate, Invulnerable, TargetDead and lethal suppression paths at upstream-admitted ticks. The Invulnerable case opens player invulnerability at `int.MaxValue-91` and confirms it at `int.MaxValue-90`; both remain below M2B's `int.MaxValue-45` common 45-tick horizon. A live 30-tick recovery overflow would require tick `int.MaxValue-29`, but M2B's Approved common `max(21, 45)` horizon correctly rejects every combat tick above `int.MaxValue-45` before M1; this test is mathematically precluded without weakening that upstream contract.
- The full Unity PlayMode and EditMode suites pass. Scene/prefab wiring remains a later integration concern because M3B1 deliberately changes no scene or prefab assets.
- The new adapter requires the serialized private component binding graph. Scene/prefab authoring remains forbidden here, so later integration must wire and independently verify these references.
- M3B1 deliberately adds no M3B2 locomotion, recovery production, surveyor firing, collision sampling or presentation behavior.

## Luna static review

Luna independently reviewed the complete production and test diff and reported final PASS with no P0/P1 defect. Luna directly parsed both exported XML files and confirmed PlayMode 65/65 and EditMode 109/109, including the combat-only `-190/-180` seam, carried `t-1`, exact 30/90-tick windows, duplicate-ID arbitration, reset/overflow/death/transfer-edge cases, and 30/60/144 render replay. Luna also confirmed the stated hashes, requirement/acceptance mapping, all three Ollama utilization outcomes, no new public ABI, and no extra physics synchronization. Sol accepts this evidence and marks M3B1 Verified.
