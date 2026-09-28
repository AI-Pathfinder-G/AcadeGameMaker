# M4B3C implementation and terminal verification evidence

- Date: 2026-09-08
- Status: **Verified** — Luna independent verification PASS; Astra final integration acceptance 2026-09-08. Initial checkpoint history is retained.
- Contract: [Approved M4B3C](../specs/work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)
- Requirements: `REQ-COM-004`, `REQ-WT-003`, `REQ-WT-005`; protected Movement/Combat behavior unchanged.
- Astra: approval, integration review, sequential Unity execution and evidence capture.
- Sol: bounded contract design and architectural counter-review, standard high reasoning.
- Terra: runtime, authoring and authored PlayMode fixture implementation.
- Luna: independent EditMode contract tests, pre-gate and checkpoint coverage review. Astra reviewed the new tests; Terra did not independently accept its own runtime.
- Ollama: all assignments recalled under ADR-0027; no invocation, probe, retry or automation. Pro mode was not invoked or claimed.

## Implemented

The exact boss terminal lane preflights its removal envelope before consuming input and publishes an internal cleanup-effect receipt. Normal removal and lifecycle supersession are distinct. Boss Combat accepts only an issuer/owner-bound exact empty next-tick discard candidate and revalidates queue shape before commit. The new `-195` coordinator preserves the final death-t handoff, stages its receipt before mutation and disables only the approved boss set. Builder/validator bind the new component explicitly; the generated prefab and sandbox scene were refreshed.

Astra review corrected candidate cross-instance/queue-change defenses, cleanup revision/lifecycle proof, receipt staging, exact collider/registry bindings and hidden-collider/unavailable-sink drift rejection. Sol's proposal to prohibit all buffered aim on the terminal tick was rejected: existing normal input and generic capture-failure semantics remain unchanged. The coordinator does not implement rewards, choice, room transition or future Combat ownership.

## Executed results

Unity 6000.6.0f1 normal-user licensing preflight passed. Runs were sequential, with source writes frozen during execution. All final runs below have Unity exit 0 and zero skipped tests.

| Run | Passed / failed | Evidence | SHA-256 |
|---|---|---|---|
| Focused EditMode (new contract + authoring) | 25 / 0 | `TestResults-Unity-EditMode-20260908-125635.xml` | `5AFD77A631B1775F0F7C9BA4DE24F7FF6EB3EB1ADE413F42BA66D3B9978888CD` |
| Focused authored PlayMode | 14 / 0 | `TestResults-Unity-PlayMode-20260908-125736.xml` | `49B3AE92E55FF6EA4BD3078048937208791F7EEDA74A58EA42DD6FE2274C02A7` |
| Full EditMode | 424 / 0 | `TestResults-Unity-EditMode-20260908-125820.xml` | `86CA24011363F7D11F1F1E74F298526D4260915E683DC5965837A1A54AAEF9B4` |
| Full PlayMode, after legacy fixture amendment | 339 / 0 | `TestResults-Unity-PlayMode-20260908-130135.xml` | `54A3C1930FB1DFE1AE854968489DD3C92F49647F4E2BD3B944CCED97B834DB62` |

The XML/logs are local ignored evidence. Authoring execution completed successfully in `Unity-M4B3C-Authoring-20260908.log`. Prefab changes add one Systems component and its explicit bindings; the scene retains one prefab instance (generated instance file ID changed). Only generated blank-name whitespace was normalized after regeneration; no semantic asset changes were hand-authored.

Earlier failures are not hidden: the first authoring pass had 17/19 passes before asset regeneration; the new EditMode fixture initially had internal-access compile errors, then named-scene setup failures. Full PlayMode initially returned 338/339 because the legacy handoff test manually disabled scheduler/handoff before automatic teardown. An Astra-approved test-only allowlist amendment removed that obsolete intervention and added automatic teardown receipt assertions; production guards were not relaxed. See [resume baseline](./2026-09-08-resume-baseline.md).

## Initial checkpoint coverage and remaining work (historical)

These are partial evidence mappings, not completion of each AC:

- `AC-M4B3C-001`, `005`: manual terminal flow and the amended automatic handoff fixture prove death-t triplet, next-t cleanup and receipt publication. Broader bootstrap/preterminal no-op and ordering-negative matrices remain.
- `AC-M4B3C-002`, `003`: normal inactive cleanup and malformed wrong-ID envelope are exercised. Active audit/payload and all four lifecycle reasons, exact durable membership/revision negative cases and post-commit mismatch still require dedicated tests.
- `AC-M4B3C-004`, `008`: empty fingerprint, append-committed empty rejection, future input/delivery, foreign owner/cross-instance and repeated candidate rejection are exercised. Actual non-empty payload/hostile batches, candidate queue changes after preflight, wrong/missing handoff, input-only proof, partial-disabled graph and second coordinator advance remain.
- `AC-M4B3C-006`, `007`: exact disabled-set assertions, runtime destructive-operation guard, automatic receipt and two post-teardown Transfer/Movement pairs are exercised. Explicit player/body/collider/registry identity, frozen death-t Combat snapshot and drift-negative assertions remain incomplete.
- `AC-M4B3C-009`: focused and full regressions pass. The existing nonterminal 30/60/144 cadence tests do not replace new terminal-cadence evidence. A terminal cadence matrix and final evidence digest remain mandatory.

Next bounded work is to finish the missing terminal/active/lifecycle/negative/cadence tests, address independent findings, then request Astra final acceptance. The contract remains `Approved`, not `Verified`. No Git commit, remote wiki publication, gameplay completion or final integration is claimed.

## Continuation: detailed terminal verification — 2026-09-08

Requirements remain `REQ-COM-004`, `REQ-WT-003`, `REQ-WT-005`; protected `REQ-MOV-001`, `REQ-COM-003`, `REQ-COM-006` remain unchanged. Terra implemented the active/lifecycle, prepared-terminal negative and synthetic-cadence tests. Luna implemented independent EditMode tests and reviewed current runtime/Terra tests. Astra reviewed the tests, corrected coverage gaps, ran Unity sequentially and owns the acceptance decision. No Ollama, Orca, remote publishing, automation or Pro invocation occurred.

Runtime hardening in this continuation rejects stale due cleanup reservations before normal input consumption; requires normal durable cleanup effect; validates the exact Ordan identity and same-Systems owner bindings, including the bound hostile producer; and reuses the existing handoff graph validator for exact reference identity. Normal Transfer consume-on-attempt behavior was not broadened or changed. The new tests independently inspect session registration absence and actual durable membership, not merely requested IDs or a receipt flag.

### Executed continuation results

Unity 6000.6.0f1; licensing preflight PASS. Each successful run below exited 0 with zero skipped tests. Source writers were frozen for execution. The final change after full EditMode was only the PlayMode assertion helper's non-generic collection iteration; the final full PlayMode compiled and exercised that change.

| Run | Passed / failed | XML | SHA-256 |
|---|---|---|---|
| Focused EditMode, authored fixture corrected | 11 / 0 | `TestResults-Unity-EditMode-20260908-133336.xml` | `4E005E3B56A94609108EB2F4B29C1ED45746538C8C18CE6AF3147F980AE1A7DD` |
| Focused PlayMode, terminal + authored + handoff | 45 / 0 | `TestResults-Unity-PlayMode-20260908-132852.xml` | `3FA3C737E948D1061B2F7A67E657076D20387C629BCDA188E2EFD920BFEAF153` |
| Final full EditMode, including exact-empty success test | 430 / 0 | `TestResults-Unity-EditMode-20260908-133629.xml` | `2279E36595832EDD7616B363BEC7F41F582F81B3ACFA42629D36AB6E8E4823D6` |
| Final full PlayMode, direct effect assertions corrected | 367 / 0 | `TestResults-Unity-PlayMode-20260908-133826.xml` | `B0ABBBA22BFE31E0BDDC920CE19127F5DE21BB30F74EC19E412168527F4181E9` |

Intermediate failures were test defects and remain in local logs: `132309` and `132628` EditMode compilations failed on internal-type access; focused EditMode `132751` and full EditMode `133108` failed two authored fixtures because EditMode did not execute production Awake registration. Explicit bridge, scheduler and pull initialization fixed the fixture without relaxing runtime guards. Full PlayMode `133659` returned 356/367 because a new assertion cast HashSet to non-generic ICollection; iterating IEnumerable fixed all 11 cases. These failed runs are not counted as successful evidence.

### Final acceptance mapping

Test files: `OrdanBossTerminalTeardownContractTests` (EditMode), `OrdanBossTerminalTeardownPlayModeTests` (PlayMode), and existing `OrdanBossEncounterAuthoredGraphPlayModeTests` / authoring / handoff suites. Each case's source comment cites the corresponding AC.

| Acceptance criterion | Executed evidence and review |
|---|---|
| `AC-M4B3C-001` | `AuthoredGraphDeathChainPublishesExactTerminalEvidence`: ordered source phases, exact death triplet, four-ID next-tick reservation, hostile/pull suppression and final handoff. |
| `AC-M4B3C-002` | Active audit and payload cleanup plus inactive normal cleanup: exact clear owner/tick/revision, Baseline, actual registration absence and exact durable four-ID membership. |
| `AC-M4B3C-003` | Four active/inactive lifecycle reasons, no durable membership on lifecycle, malformed mixed/wrong-ID/order/duplicate envelopes, duplicate/stale reservation preservation, and wrong-removal-tick constructor rejection. Receipt-only mismatch is explicitly a **helper-level** fault injection after valid commit, not a full production-phase re-execution; publication-before-receipt ordering and absence of rollback were also inspected in runtime. Existing generic Transfer regressions pass. |
| `AC-M4B3C-004` | Exact-empty optional input and delivery successful discard; actual payload/hostile/aim/press/external damage, missing/future/stale/forged/cross-owner/repeated candidates and changed queues rejected. Authored tests separately prove the death snapshot remains frozen. |
| `AC-M4B3C-005` | Bootstrap-before-first-tick and preterminal no-op, automatic next-tick teardown, independent same-tick graphs, repeated entry rejection; no subsequent boss publication. |
| `AC-M4B3C-006` | Exact boss component/collider set disabled; root/Systems/player/Transfer/registries and bound identities preserved. Static destructive-operation scan is scoped to the new runtime file and added runtime hunks; authoring rebuild operations are excluded. Builder/validator regression passes. |
| `AC-M4B3C-007` | Two additional player/Transfer ticks, same player body/collider, Baseline, removed-target press cannot reacquire, actual registration absence, and final Combat/handoff values unchanged after continuation. |
| `AC-M4B3C-008` | Single-mutation prepared graph cases: missing/wrong-order/wrong-tick handoff, input-only effect, same-root foreign owner, wrong collider, partial-disabled graph, target collider/sink drift; queues/publications are preserved. Cross-owner tests and exact-root binding review complement these cases. |
| `AC-M4B3C-009` | Full 430 EditMode + 367 PlayMode regressions, authoring and previous boss/Transfer/Movement suites, plus independent synthetic 30/60/144 terminal traces below. |

### Terminal trace digest (`AC-M4B3C-009`)

The cadence test varies synthetic render-frame fixed-step grouping over ticks 0–7, including death at 4, cleanup/teardown at 5, and continuation at 6/7. It compares the raw final presentation signature as well as tick-keyed terminal entries. This is deterministic synthetic-cadence coverage, **not a real-time performance/FPS benchmark**.

- First divergent terminal tick: none across 30/60/144 groupings.
- Final handoff diagnostic FNV-1a hash: `B1536B6F` (not a cryptographic integrity claim; XML integrity uses SHA-256 above).
- Cleanup: death 4, cleanup 5, LifecycleSuperseded false, DurableRemovalCommitted true, no active target, Baseline, revision 0, no clear; requested IDs exactly `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2`.
- Disabled components: `combat, bridge, scheduler, hostile, pull, handoff, ordan`; coordinator disables itself after receipt publication.
- Disabled colliders: `ordan, BossAuditBox-0, BossWeight-0, BossWeight-1, BossWeight-2`.
- Post-teardown player/Transfer ticks: `6/6`, `7/7`; cleanup-tick player snapshot 5. Full trace is emitted in the XML test output.

The scope remains local sandbox survival after boss death. Reward consumption, room/run completion transitions, future Combat ownership and the full game are not implemented or accepted by this evidence. No Git commit or remote wiki publication is claimed.

### Final decision

Luna independently read the final XML/logs, confirmed both SHA-256 digests and the terminal trace, and reported `AC-M4B3C-001` through `009` PASS with no P0/P1/P2 blocker. The AC003 helper-level limitation remains explicit above. Astra accepts the reviewed local integration under the unchanged Approved behavior contract on 2026-09-08 and promotes this work contract to `Verified`. This decision supersedes the historical initial-checkpoint outstanding list, not its recorded results.
