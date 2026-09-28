# VD-03 M4A 환수관 오르단 결정론적 보스 코어 구현 증적

- Date: 2026-09-05
- Contract: `docs/specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md`
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-005`, `REQ-WT-006`
- Acceptance criteria: partial `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-005`
- Result: **PASS / Verified — Luna `P0=0`, `P1=0`, `P2=0`; Sol integration approved**

## Implemented allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs.meta`

The unit is internal and engine-free. It adds no public ABI, Unity-object identity, physics query, frame-time source, RNG, scene, prefab, project-setting, reward grant, room mutation, or save/run mutation.

## Behavioral evidence

- `AC-COM-002`: 28 focused tests pin the four-slot cursor, cyclic payload handles, all Stage 1/2/3 normal timing rows, accelerated Stage 2/3 timing, exclusive phase ends, strict health thresholds, delayed promotion, DebtLine one-shots, Audit pull/shockwave ordering, and deterministic replay.
- `AC-COM-003`: focused tests pin preview purity, recompute-on-commit, forged/stale candidate rejection, immutable output copies, exact clocking, revision non-consumption on rejection, malformed input atomicity, overflow, death precedence, normalized repeated death, and one-shot future handoffs.
- `AC-COM-004`: focused tests pin Telegraph-only payload transfer acceptance, exact request ABI, next-tick aligned-result validation, lethal clamp/`TargetDead` behavior, 120-tick payload vulnerability, 90-tick Audit interruption vulnerability, and hostile-intent suppression.
- `AC-WT-005`: transfer edges use explicit target IDs, exact `Baseline -> Heavy` edges, and strictly increasing global and per-target revisions without changing Transfer authority.

## Unity verification

Unity `6000.3.21f1` ran with a valid Unity Personal entitlement. The Unity 6 test runner was invoked without `-quit` so it could own shutdown after writing NUnit XML.

| Gate | Total | Passed | Failed | Skipped | Result | XML SHA-256 |
|---|---:|---:|---:|---:|---|---|
| M4A focused EditMode | 28 | 28 | 0 | 0 | Passed | `3CBA4959DB12A2E91644C49931ED5980563BD280F657EA2D2CD47D3E75F18051` |
| Combat EditMode regression | 200 | 200 | 0 | 0 | Passed | `346223673832BA9065F5084DC9B25C8F390FCA3D90874A6A0DE8F7FF0213AC7E` |
| Full EditMode regression | 317 | 317 | 0 | 0 | Passed | `02B6B7032E02F823F8F5F84A671C6B8734EF9531061ADE8890945C788559DD4F` |
| Full PlayMode regression | 288 | 288 | 0 | 0 | Passed | `EDDA621BAE33AE3A01B8AA3E0DCE45675AD812AFE82726857F19032628D58826` |

The first compile attempt exposed two integration defects: a stale mutable-candidate state reference and an incorrect `DamageRequest` tick property name. Sol corrected both before the passing run. A later expanded test initially encoded accelerated payload completion as direct Recovery; the frozen contract requires an aligned next-tick result and vulnerability, so the test was corrected and the complete ladder reran green.

## Ollama utilization record

- Kimi K3: **used and accepted in part**. Immutable phase decomposition and preview/commit test ideas informed the unit; guessed thresholds and authority leaks were rejected by Terra/Sol screening.
- GLM 5.2 pre-pass: **used and accepted in part**. Stage-promotion, phase-boundary, death, one-shot, and mutation risks were accepted; incorrect ownership premises were rejected.
- GLM 5.2 post-pass: **used and accepted in part**. Pending-result/death coincidence and malformed-input atomicity were retained as coverage checks. Mixed simultaneous-edge and deferred-edge premises that cannot exist in the frozen single-edge input ABI were rejected.
- MiniMax M3: **failed and replaced**. Its validation-tool proposal returned an empty response without quota, authentication, network, or model-availability error. Terra implemented the deterministic fixtures and Luna reviewed their required coverage instead.
- Luna: independently found one P1 test-coverage gap after implementation. Terra added the missing timing, request-shape, revision, pending-result, repeated-tick, and overflow cases; Sol corrected one faulty test expectation found by execution.

Luna's final independent review found no remaining P0, P1, or P2 issue and authorized the M4A contract to transition to `Verified`. This remains partial evidence for the parent acceptance criteria; M4B still owns the Unity bridge and authored encounter proof.

No cloud model received repository contents, local paths, credentials, personal data, or secrets. No Ollama output was adopted without GPT screening.

## Workspace safety

The pre-existing user-owned changes in `ProjectSettings/URPProjectSettings.asset` and `ProjectSettings/SceneTemplateSettings.json` were not staged, reverted, or edited. Test XML and Unity logs are execution artifacts and are not part of the implementation allowlist.
