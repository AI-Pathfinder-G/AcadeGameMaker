# VD-03 M4B1 Ordan Unity Combat Bridge Contract Pre-gate

- Date: 2026-09-05
- Contract: `docs/specs/work-contracts/2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md`
- Status: **PASS — P0=0, P1=0, P2=0**
- Reviewer: Luna

## Gate focus

The pre-gate must verify single Combat ownership, exact `-200/-190/-185` ordering, mandatory empty-or-singleton next-tick delivery, M1 result pass-through, relevant Transfer edge preservation, age-0 exposure forecast, death/cleanup behavior, immutable handoffs, allowlist isolation and truthful partial-AC language.

## Pre-review inputs

- Terra recommended splitting runtime delivery, Transfer/trajectory, and authored encounter/presentation into M4B1/M4B2/M4B3.
- Terra identified that the bridge cannot reconstruct age-0 exposure or the private next payload ordinal from an M4A snapshot. Sol resolves it with an additive M4A-owned prior-tick immutable exposure forecast and explicitly rejects cross-phase rollback.
- GLM highlighted identity exposure, incomplete publication, tick drift, partial initialization, callback authority and consumer mutation.
- Kimi's immutable staging and queue concepts were accepted only after removing invented fields and authority leaks.
- MiniMax returned an empty response; no quota/auth/network/model error was reported, so Luna must independently cover its validation-fixture role.

This pre-gate authorizes Sol to approve only the contract's exact M4B1 allowlist. It is not implementation or Unity execution evidence.

## Review history

Luna's first pass reported four P1 gaps: exact Transfer modifier mapping, executable boss/regular Combat-owner exclusion, Transfer capture-receipt validation, and first-use atomic adoption. Sol added all four plus exact forecast persistence cases and a per-encounter SYSTEM-CONTRACTS amendment. Luna's second pass found no P0/P1 issue and one P2 wording gap; Sol replaced the shortened request-ID order wording with M1's complete `(RequestId, SourceId, TargetId, Kind, Amount)` key.

Terra separately found that first-tick and later-tick atomicity could not truthfully be described as one global transaction because M1 has no rollback API. Sol froze a private provisional Combat result for bootstrap and the existing phase-local no-rollback rule for later ticks. Luna must confirm this final correction before approval.

Luna's final pass confirmed the provisional bootstrap, later phase-local no-rollback behavior, exact M1 sort key and all earlier corrections with `P0=0`, `P1=0`, `P2=0`. Sol approved the contract on 2026-09-05.
