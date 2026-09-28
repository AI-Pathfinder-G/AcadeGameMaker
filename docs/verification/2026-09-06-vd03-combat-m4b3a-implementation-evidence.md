# VD-03 M4B3A Ordan Authored Boss Graph Implementation Evidence

- Result: **PASS / Verified**
- Contract: [M4B3A Ordan Authored Boss Graph](../specs/work-contracts/2026-09-06-vd03-combat-m4b3a-ordan-authored-graph.md)
- Integration authority: Sol
- Unit implementation and test expansion: Terra, screened and integrated by Sol
- Independent verification: Luna, `P0=0`, `P1=0`, `P2=0`
- Unity: `6000.6.0f1`
- Date: 2026-09-06

## Implemented result

M4B3A now provides one isolated, connected Ordan boss prefab and standalone scene. The graph reuses the approved player prefab, contains the exact Ordan/audit/payload/environment hierarchy, pre-registers the exact Combat and Transfer rosters, and activates the verified `-200 → -190 → -185 → -180 → +110` chain. The audit box remains dormant. The `+110` adapter publishes a Unity-reference-free immutable presentation view only after exact same-tick publications and same-graph identities converge.

The adapter rejects foreign roots, foreign registries, wrong roster objects, wrong audit/payload collider ownership, partial first publication, duplicate ticks, skipped ticks, stale/cross-wired inputs, and failed publication without replacing the last accepted view. Death preserves the exact ordered `Defeated`, `RewardRequest`, `RoomCompletionRequest` triplet and terminal payload removals. In accordance with Verified M4B2, later scheduler/adapter attempts reject and the last terminal view remains unchanged; no post-terminal view is synthesized.

## Requirement and acceptance evidence

- `REQ-COM-001`, `AC-COM-002`: exact `ordan,player` Combat roster, exact audit/payload Transfer roster, Ordan basic-attack eligibility, payload-only scripted exposure, and first `BossWeight-0` Telegraph availability are verified.
- `REQ-COM-003`, `AC-COM-003`: the exact phase chain, DebtLine-to-SeizureWeight progression, immutable copies, structural mutation matrix, foreign/cross-wired/stale/duplicate/skipped rejection, atomic failed publication, death triplet, terminal removals, and terminal-view preservation are verified.
- `REQ-COM-004`, `AC-COM-004`: one owner per fixed phase, exact execution-order attributes, same-Systems and same-root identity convergence, first tick `0`, exact `+1` publication continuity, and absence of room/reward/scene/input consumers are verified.
- `REQ-COM-006`: authored Ordan identity, health `60`, collision profile, and verified boss stack activation are verified.
- `REQ-WT-003`, `REQ-WT-005`: exact target kind/profile/owner/collider identities, four initially hidden Transfer targets, dormant audit revision behavior, payload exposure, and terminal removals are verified.
- Synthetic cadence evidence uses three independently instantiated authored graphs. Each synthetic render frame samples once: 30 Hz advances two fixed ticks, 60 Hz advances one, and 144 Hz uses a 5/12 `0/1` pattern. Full immutable-view signatures match field-for-field at every matching simulation tick, including unchanged zero-step observations.

## Final test evidence

| Suite | Result | SHA-256 |
|---|---:|---|
| `TestResults-M4B3A-Focused-Edit9.xml` | 16/16 passed | `E1118F0E2E0B9C8DE9CF11A054839BD144FC9D351BF9FF650344D0D685F8DE3B` |
| `TestResults-M4B3A-Graph-Play8.xml` | 4/4 passed | `037C536439B1C97FF1F0CF3BFF73201C03919301280F21044667648A6D60C84F` |
| `TestResults-M4B3A-Handoff-Play4.xml` | 3/3 passed | `2C7E94D87E9202E12EB3E8AF802F4EC36A7E984EE3897F23E03898341A16018A` |
| `TestResults-M4B3A-Full-Edit4.xml` | 372/372 passed | `D5911F619D02395656D6D5CDFCADE899A8EB6900615485DD218338D8E6D5FFDB` |
| `TestResults-M4B3A-Full-Play4.xml` | 312/312 passed | `96FD5D104AFFDE69809B33FDA495123F3F3E20DCE0A1838145108D14A5B37558` |

All final suites report zero failures and zero skipped tests. Earlier compile-error, strict Unity 6000.6 prefab-query, incomplete-coverage, and superseded focused/full XML files remain local execution history only and are not acceptance evidence.

## Independent review closure

Luna's first implementation review rejected verification because root identity, monotonic publication, detailed mutations, actual phase tracing, cadence sampling, static authority checks, and implementation evidence were incomplete. Sol and Terra closed each finding. Luna's final read-only review found `P0=0`, `P1=0`, and `P2=0` and approved M4B3A code and acceptance coverage.

## Ollama utilization record

| Lane | Outcome | Screening result |
|---|---|---|
| Kimi K3 | used and rejected | Invented unrelated schemas and fixed-point types outside the frozen contract; no code or schema was adopted. |
| GLM 5.2 | used and accepted | Adversarial tick-order, body-transfer contamination, duplicate damage, prefab drift, hidden collider, teardown, and presentation-authority scenarios informed the final matrix after Sol normalization. |
| MiniMax M3 | used and rejected | Proposed useful validator categories, but wrong names, roster counts, tags, schemas, and phase values were rejected; no proposal code was integrated. |

All Ollama work used abstract non-sensitive packets. Terra or Luna screened every result, and Sol alone integrated the accepted conclusions.

## Scope boundary and safety

M4B3A does not implement audit exposure, hostile hit geometry, damage delivery, pull movement, reward/room consumption, scene transition, run integration, teardown, or final art/audio. Those remain M4B3B or later work.

The protected pre-existing files stayed byte-identical throughout final verification:

- `ProjectSettings/URPProjectSettings.asset`: `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7`
- `ProjectSettings/SceneTemplateSettings.json`: `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A`
- `Assets/Scenes/MovementSandbox.unity`: `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`

Unrelated Unity baseline, licensing, BGM, video, QA, and local log/XML changes were not adopted as M4B3A implementation scope.
