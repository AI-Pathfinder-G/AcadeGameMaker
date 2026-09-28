# VD-03 M4B3B3 Ordan BalanceAudit Pull Implementation Evidence

- Result: PASS / Verified
- Date: 2026-09-07
- Independent reviewer: Luna (`P0=0`, `P1=0`, `P2=0`)
- Contract: [M4B3B3 Ordan BalanceAudit deterministic pull](../specs/work-contracts/2026-09-07-vd03-combat-m4b3b3-ordan-audit-pull.md)
- Requirements: `REQ-MOV-001`, `REQ-MOV-002`, `REQ-MOV-004`, `REQ-MOV-005`, `REQ-MOV-006`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-003`, `REQ-WT-006`
- Acceptance evidence: `AC-M4B3B3-001` through `AC-M4B3B3-010`

## Implemented boundary

- M4A publishes immutable `BalanceAuditPullProjection` values only for committed `BalanceAudit/Execute` ticks, retaining exact source tick, attack ordinal, phase age and frozen `90/82/75` duration.
- One authored `OrdanBossBalanceAuditPullProducer` runs at `-165`. Source tick `t` may queue only movement tick `t+1`; tick-zero bootstrap, duplicate/stale/skipped publication and wrong-horizon state fail closed.
- Movement owns the internal directive, checked Q4096 ceil-integer-sqrt math, toward-zero radial scaling, ordinary-motion-first composition, X-then-Y collision resolution, snapshot and same-commit receipt. Public `MovementCommand` and `PlayerMotionSnapshot` ABI remain unchanged.
- Same-tick BalanceAudit interrupt, boss/player death and the four approved lifecycle reasons cancel an unconsumed current directive before Movement and suppress the next directive. Completed prior movement is never rolled back.
- The `+110` handoff validates the exact producer outcome and current movement receipt, then separates consumed-current, canceled-current and queued-next identities in an immutable minimal digest.
- Builder-generated prefab/scene assets serialize exactly one producer on `Systems` with anchor `(28672,4915)` and maximum step `410`; the read-only validator checks component order, bindings, constants and handoff wiring.

## Verification results

| Run | Result | Acceptance IDs | SHA-256 |
|---|---:|---|---|
| Focused EditMode `OrdanBossSessionTests` | 42/42 | `AC-M4B3B3-001`, `007` | `3A96C5E74BA777207A4A502C337D590C216322E92AB89D09DC7361A19A26FB19` |
| Focused EditMode `PlayerMovementMotorTests` | 32/32 | `AC-M4B3B3-002`, `003`, `004`, `006` | `158E6634D3D98A0D5AA958E17A7B9B297FCBF0166D6E7C497A81AA4237C54DEA` |
| Focused PlayMode `PlayerMovementControllerPlayModeTests` | 20/20 | `AC-M4B3B3-003`, `005`, `006`, `007` | `E7B75D934CAFE4FF1B5753864FF41D34D87F48A5999F3A6C2A3E33BD599D8F18` |
| Focused authored PlayMode `OrdanBossEncounterAuthoredGraphPlayModeTests` | 13/13 | `AC-M4B3B3-001`, `007`, `008`, `009` | `2EA95EDE5C38E4A04B538321DA3752AFFE4D2E2E648832409EDA8DF8190DEB3F` |
| Full EditMode regression | 418/418 | regression support for `AC-M4B3B3-001` through `010` | `E32AA2193D3050677EEBF81092085ADCE91F2E04F1E1084848A32268C39E3A43` |
| Full PlayMode regression | 338/338 | regression support for `AC-M4B3B3-001` through `010` | `0CDAC6436A1725B72465E80C01B4129B133256407AD11047EBAAA528B8E16E0A` |

The authored cadence replay uses independent graphs at 30/60/144 render groupings. Matching simulation-tick signatures include the complete `PlayerMotionSnapshot`, Movement receipt-derived pull digest, queued-next identity, Combat/Transfer publications, M4A projections, M4B3B1 exposure state, M4B3B2 hostile digest and stable authored IDs. No first-divergent tick was observed.

## Focused behavior evidence

- Projection tests cover all frozen durations, exact age/source/ordinal identity, age-zero and final rules, preview/commit equality and age-zero interrupt suppression.
- Integer math tests cover exact anchor, near-axis `(410,1)`, diagonal `(600,600)`, mirrored axes, radial bound, checked square overflow and final predicted-coordinate overflow without snapshot publication.
- Nine branch cases cover neutral, move, jump, release, dash, wall-slide, wall-jump, `InputLocked` and `Lightweight`; pull changes position only and leaves velocity/state evolution equal to ordinary motion.
- PlayMode covers owner/forged/duplicate commit rejection, ground plus combined wall/ceiling collision, no overlap/tunneling, atomic receipt preservation on failed due tick, Baseline/Lightweight modifier reflection and clear, and later attack-ordinal lane reuse.
- Authored graph tests cover all 90 Execute ticks, exactly 89 consumed pulls, queue/consume separation, later interrupt cancellation, prior-commit non-rollback, boss/player death, lifecycle suppression, terminal latch, duplicate and skipped bootstrap rejection.

## Static and asset checks

- The pull producer contains no Transform/Rigidbody/Physics query, force, velocity or `MovePosition` authority.
- Exact fixed order is `-200 Transfer -> -190 Combat -> -185 bridge -> -180 scheduler -> -170 hostile -> -165 pull -> default Movement -> +110 handoff`.
- Scoped `git diff --check` passes for every M4B3B3 implementation, test, asset and documentation file.
- The public Movement ABI remains unchanged. `Movement/AssemblyInfo.cs` grants only the internal receipt/directive boundary to `Combat.Unity` and its focused EditMode verifier, as recorded by the Sol allowlist amendment.

## Review corrections

Terra's first final pass reported one P1 coverage gap and prefab trailing whitespace. Luna independently reported duplicate source-tick acceptance plus missing cadence snapshot and focused branch/collision/lifecycle evidence. Sol accepted those findings: the producer now enforces tick-zero bootstrap and strictly consecutive publications; tests now cover nine motor branches, overflow, combined collision, receipt failure preservation, the complete 90-tick window, suppression/terminal/non-rollback, modifier ordering, later ordinal reuse and full Movement cadence state. The generated prefab whitespace was removed with exact component context. Terra's re-review and Luna's final re-review both returned `PASS`, `P0=0`, `P1=0`, `P2=0`.

## Ollama utilization record

| Lane | Outcome | GPT screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Retained interrupt priority, strict tick/lifecycle behavior, zero-distance, cadence and immutability cases; rejected saturation and guessed precedence. |
| Kimi K3 | used and accepted in part | Retained immutable directive and preflight/commit/consume concepts; rejected public API expansion, command rewriting, guessed constants and invalidating zero distance. Terra/Sol rewrote the local implementation. |
| MiniMax M3 | used and accepted in part | Retained table fixtures, symmetry, collision and first-divergent-tick diagnostics; rejected half-tick/random/apply-then-nullify proposals. |

No cloud model received repository files, local paths, credentials, personal data or secrets. Terra and Luna screened the work; Sol alone accepted integration.

## Unclaimed work

M4B3B3 does not add VFX, audio, camera behavior, UI, room completion, rewards, teardown, persistence or scene transition. Those remain later units.
