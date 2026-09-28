# VD-03 M3A Regular Enemy Reaction Core Evidence

- Date: 2026-08-28
- Contract: [VD-03 M3A](../specs/work-contracts/2026-08-28-vd03-combat-m3a-regular-enemy-reaction-core.md)
- Baseline: `5acff45`
- Implementation: Terra
- Independent verification: Luna
- Integration decision: Sol PASS
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`

## Outcome

M3A is verified as an internal, engine-free deterministic state machine. It adds no public type, Unity reference, project setting, scene or asset and does not alter M1, M2A, M2B or VD-02's transfer-modifier authority.

The implementation fixes these observable boundaries:

- walker natural and Heavy-impact shield openings are exclusive 30/90-tick windows;
- walker Heavy affects only the local recovery projection `1500`, with dead state neutralized to `1000`;
- surveyor alive Heavy edges force descent and start/extend an exclusive 75-tick fire lock;
- raw Heavy and historical end ticks survive directive suppression until cleanup/reset as specified;
- transfer revisions accept only `1..int.MaxValue`, allow gaps, and reject duplicate, older and no-op edges atomically;
- death, baseline cleanup and encounter reset retain one global exact-tick clock;
- reset clears encounter-local revision/tombstone/window/death state only after the VD-02 baseline clear;
- all proposed horizons and next ticks use checked arithmetic before commit.

## Verification results

| Check | Result | Acceptance mapping |
|---|---:|---|
| Luna independent runtime/test compilation | PASS | `AC-COM-003`, `AC-WT-005` |
| Luna direct NUnit execution | **15/15 PASS** | `AC-COM-001`, `AC-COM-003`, `AC-WT-002`, `AC-WT-005` |
| Terra direct NUnit execution | **15/15 PASS** | corroborating evidence |
| 30/60/144 render-group replay trace | exact match | `AC-COM-001`, `AC-COM-003` |
| new public surface / Unity reference scan | none | contract boundary |
| `.meta` GUID duplicate scan | none | import safety |
| `git diff --check` | PASS | integration hygiene |
| Unity 6.3 LTS Test Runner full EditMode | **108/108 PASS** | `AC-COM-001`, `AC-COM-003`, `AC-WT-002`, `AC-WT-005` |
| Unity 6.3 LTS official XML focused M3A subset | **15/15 PASS** | `AC-COM-001`, `AC-COM-003`, `AC-WT-002`, `AC-WT-005` |

The focused matrix covers constructor snapshot absence, exact tick/target failure, revision bounds/gaps/no-ops, 30/90 window boundaries and overlap, event grammar/tombstones, 75-tick surveyor lock and later Heavy extension, walker and surveyor raw/effective death behavior, dead-tick horizon suppression, transfer cleanup, walker and surveyor reset behavior, reset revision reuse, first-state and populated-state overflow atomicity, reset overflow and replay determinism.

## Unity regression closure

On 2026-08-29, a Unity Hub-launched authenticated Unity 6.3 LTS editor ran the official Unity Test Runner and completed the full EditMode suite at **108/108 PASS**, including all **15/15** `RegularEnemyReactionSessionTests` cases.

The exported Unity XML is `TestResults/2026-08-29-editmode-108-pass.xml`: start `2026-08-28 15:05:50Z` (`2026-08-29 00:05:50 KST`), result `Passed`, total `108`, passed `108`, failed `0`, skipped `0`, duration `2.3464072s`, SHA-256 `D8C8DFACF78AC7B1B94FB0EF30DFC50F25C50D0D9CE2F3B31A9DA304C7440B0A`. This closes the prior code-198 environment obligation for M3A. Direct non-Hub batch launch remains incompatible with the concurrently installed Hub licensing-client protocol, so this evidence was produced through the authenticated Hub/editor Test Runner path; no assertion or compilation failure was observed within this Unity EditMode run.

## Ollama utilization record

| Lane | Outcome | Screening and replacement |
|---|---|---|
| Kimi K3 | used and accepted in part | Its generic stage-before-commit and lifecycle-guard structure informed the bounded implementation. Public types, equality-threshold behavior and reset semantics were rejected and Terra rewrote all code from the Approved Sol contract. |
| GLM 5.2 | used and accepted in part | Pre-pass ordering/death/reset/dedupe concerns and post-pass revision/death/reset/atomic mutation scenarios were retained. Threshold-event interpretations that contradicted exclusive duration windows were rejected. |
| MiniMax M3 | failed and replaced | No usable final fixture was returned because output budget was consumed by uncontrolled reasoning. There was no HTTP 429 or quota failure. Terra authored the matrix and Luna independently reviewed it. |

No Ollama model received repository content, local paths, credentials, personal information or secrets. No Ollama output had approval or integration authority.

## Deferred boundary

M3A does not construct M2A observations, arbitrate same-tick M1 results, run Unity AI, or reserve execution order. M3B must separately approve and verify effective attackability, ordinal-first Heavy-impact arbitration, M1 final-alive input, transfer-edge delivery and any later `-180` phase/SYSTEM-CONTRACTS update.
