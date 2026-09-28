# VD-03 M4B3B1 Ordan Audit Forecast and Exposure Implementation Evidence

- Result: PASS / Verified
- Date: 2026-09-06
- Independent reviewer: Luna (`P0=0`, `P1=0`; final documentation-only `P2` closed before integration)
- Contract: [M4B3B1 Ordan Audit Forecast and Exposure](../specs/work-contracts/2026-09-06-vd03-combat-m4b3b1-ordan-audit-exposure.md)
- Requirements: `REQ-COM-003`, `REQ-COM-004`, `REQ-WT-003`, `REQ-WT-005`
- Acceptance evidence: partial `AC-COM-002`, `AC-COM-003`, `AC-WT-003`, `AC-WT-005`

## Implemented boundary

- M4A now publishes an immutable exact-`t+1` audit forecast calculated only from its staged candidate. Payload and audit forecasts cannot both be exposed.
- The existing `-180` scheduler has explicit serialized legacy three-payload and authored-production four-target modes. Production binds the authored audit target, sink and collider and uses the same exposure-end owner.
- Normal audit withdrawal remains temporary and reusable. Terminal production scheduling performs one four-ID removal merge while preserving the existing three-payload scheduler outcome and a separate audit-only value outcome.
- Transfer keeps an internal defensive copy of the latest completed input's requested removal IDs. Same-tick lifecycle precedence clears active Transfer through the lifecycle path and does not promote skipped removals into durable descriptor exclusions.
- The bridge, presentation handoff, builder and validator carry or prove the additive audit values and exact authored identities without phase inference, physics authority, damage, movement or teardown.

## Verification results

| Run | Result | Acceptance IDs | SHA-256 |
|---|---:|---|---|
| Focused EditMode `OrdanBossAuditExposureTests` | 4/4 | `AC-COM-002`, `AC-COM-003`, `AC-WT-005` | `7BD1C9220918D7BC8F91D253A4D1E7375E7EEF09D4A9CCC3D4884FD2E882F881` |
| Focused PlayMode `OrdanBossAuditExposurePlayModeTests` | 9/9 | `AC-COM-002`, `AC-COM-003`, `AC-WT-005` | `0C904B740DE655E74939FF93E60B524532366BC75E5A49994E67B94C3449F1E9` |
| Full EditMode regression | 378/378 | regression support for the listed partial ACs | `B7DFF6C1328CD20A44AE9286556CD977DD62DB952A6FA1A38057F1BF44DA0A10` |
| Full PlayMode regression | 322/322 | regression support for the listed partial ACs | `CA9B757DAE37AE09C6CFF7EF8C90C96EE7625E1103542F187C2D47203B1A3F37` |

The first sandboxed Unity launch produced no XML because its Windows security boundary could not reach Unity Licensing IPC; it is environment evidence, not a failed test. The accepted runs above were executed in the normal Windows user context. Iteration exposed inaccessible internal-type references, premature test lifecycle execution, missing first-tick bootstrap, real target-range setup, interrupt-time temporary-end timing, and one stale overflow assertion. All were corrected before the recorded green runs.

The final focused matrix additionally proves explicit legacy/production mode and fail-closed registration, authoring rejection of a serialized legacy mode, active-audit terminal cleanup, all four supported lifecycle-first cases, requested-versus-processed removal separation, foreign prequeued-removal rejection, temporary clear with same-ID reuse, audit handoff defensive copies, and the audit-inclusive 30/60/144 cadence signature through tick 500.

## Static and asset checks

- Scoped `git diff --check` passes for the M4B3B1 allowlist. A repository-wide check still reports pre-existing trailing whitespace in the unrelated dirty `ProjectSettings/ProjectSettings.asset`; M4B3B1 does not modify or stage that file.
- The approved builder regenerated the boss prefab and scene, and the authored validator ran inside the builder before both saves.
- Protected baseline hashes remain exact:
  - `ProjectSettings/URPProjectSettings.asset`: `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7`
  - `ProjectSettings/SceneTemplateSettings.json`: `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A`
  - `Assets/Scenes/MovementSandbox.unity`: `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`

## Ollama utilization record

| Lane | Outcome | GPT screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted stale forecast, hidden collider, movement authority, teardown order and terminal-latch risks; Sol rejected rollback semantics. |
| Kimi K3 | used and rejected | The proposal invented time, ID, fixed-point and movement contracts outside this unit; Terra replaced it with the approved local implementation. |
| MiniMax M3 | used and rejected | The fixture proposal invented random, polygon, rendering, GC/audio and visual-oracle scope; Luna/Sol retained only contract-owned deterministic checks. |

No cloud model received repository files, local paths, credentials, personal data or secrets.

## Unclaimed work

This remains partial evidence. Hostile geometry and player damage are M4B3B2, pull movement is M4B3B3, and terminal teardown/lifecycle consumption is M4B3C. M4B3B1 does not close `AC-COM-002` or `AC-COM-004` system-wide.

## Independent review

Luna's final re-review found the earlier authoring-mode and acceptance-coverage blockers closed. The reviewer confirmed the explicit production-mode validator, exact four-ID terminal preflight, foreign queued-removal rejection, active/inactive terminal paths, four lifecycle-first cases, requested-versus-processed separation, legacy/production registration matrix, temporary clear and same-ID reuse, audit handoff defensive copies, and audit-inclusive 500-tick cadence traces. Final implementation severity is `P0=0`, `P1=0`; the sole `P2` was this evidence page's stale counters/status and is closed above.
