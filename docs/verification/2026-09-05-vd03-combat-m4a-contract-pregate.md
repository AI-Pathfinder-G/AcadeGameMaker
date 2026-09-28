# VD-03 M4A Ordan Boss Core Contract Pre-gate

- Date: 2026-09-05
- Contract: `docs/specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md`
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-005`, `REQ-WT-006`
- Partial acceptance targets: `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-005`
- Final Luna result: **PASS — P0=0, P1=0, P2=0**
- Sol decision: **Approved for M4A implementation**

## Review history

Luna's first pass reported four P1 precision gaps and two P2 scope/normalization gaps. Sol froze the Stage 1 bases and exact Stage 2/3 integer ceilings, M1 lethal-clamp and `TargetDead` handling, explicit `Baseline -> Heavy` edges with global and per-target revision commit rules, exact intent/request IDs, checked tick/ordinal horizons, normalized snapshots, and engine-free evidence scope.

The second pass found one remaining contradiction between Telegraph-only external handle exposure and Execute-time identity retention, plus two P2 shape gaps. Sol separated Transfer eligibility from internal trajectory correlation, froze local target/state/revision discriminants, and specified the first and repeated-Defeated input/output shapes.

The third pass found one stale sentence that still described the retained Execute identity as unavailable. Sol replaced it with the same external-exposure/internal-correlation distinction. Luna's final pass found no remaining P0, P1 or P2 issue.

## Accepted contract boundaries

- M4A is one internal, engine-free deterministic state machine in the existing Combat assembly.
- Combat alone owns health, damage ordering, dedupe and death; Transfer alone owns active transfer and revisions.
- Ordan's body is not a Transfer target. Telegraph exposes one rotating `BossWeight-0..2` target; Execute retains its ID internally only.
- Vulnerability is a pattern-suspending safe attack window without a damage multiplier.
- Payload self-damage is an exact amount-8 next-tick Combat request with aligned-result validation.
- No Unity, scene, prefab, public API, input, UI, room, reward grant, run or project-setting change is allowed.

## Ollama screening

- Kimi K3: **used and accepted in part** for typed immutable phase decomposition and preview/commit test ideas; authority leaks and guessed thresholds were rejected.
- GLM 5.2: **used and accepted in part** for ambiguity, death, one-shot and mutation risks; incorrect ownership assumptions were rejected.
- MiniMax M3: **failed and replaced** after an empty response without quota/auth/network/model error; Terra and Luna supplied fixture analysis.

No cloud model received repository content, local paths, credentials, personal data or secrets.

## Gate meaning

This PASS authorizes Terra to implement only the approved M4A allowlist. It is not Unity execution evidence and does not close `AC-COM-002`, `AC-COM-003`, `AC-COM-004` or `AC-WT-005` by itself.
