# VD-03 M3B2B Contract Pre-Gate — Luna PASS

- Date: 2026-08-29
- Contract: [VD-03 M3B2B](../specs/work-contracts/2026-08-29-vd03-combat-m3b2b-behavior-unity-bridge.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Independent verifier: Luna
- Final result: PASS — no remaining P0/P1

## Review history

Terra's read-only feasibility pass found two P1 contract gaps: the first behavior tick needed an explicit player bootstrap-snapshot rule, and a default tick-0 `CombatSimulationOutcome` could not prove that M1 had actually published. Sol specified `Snapshot.Tick=t0` only at the initial expected tick and `Snapshot.Tick=t-1` thereafter, then required separate combat-publication presence plus combat-owned frozen-roster and production reaction-view seams.

Luna's first independent pass confirmed those corrections and found one remaining P1: the Approved system contract still ended its fixed phase at `-180 -> default`. Sol synchronized the authority document with the new `-170` behavior phase and its two-owner handoff.

The final pass confirmed:

- exact `-200 -> -190 -> -180 -> -170 -> default` ordering and no second physics synchronization;
- current-physics Q1000/LOS staging, literal role ordering and combat-owned frozen roster;
- explicit initial-tick snapshot semantics and successful M1 publication presence;
- mutation-free behavior preview followed by M3B1 recovery preflight, M3B2A commit, guaranteed queue commit and carried publication;
- result-dependent failure preserves M3B2A, the M3B1 recovery queue and prior carried view while retaining already successful upstream publications and stopping the pipeline;
- death, reset, future `t+1` recovery, ordinal reuse and surveyor value-only shot ownership remain aligned with verified M3B1/M3B2A;
- actual motion, collision, damage, denial-line lifetime, presentation, scene and project settings remain out of scope;
- Kimi/GLM/MiniMax outcomes are explicit, screened and non-authoritative.

## Approval

Sol accepts Luna's final PASS and changes the M3B2B contract from `Review` to `Approved`. Terra may implement only the frozen allowed files and behavior. M3B2B remains integration substrate and does not by itself close full `AC-COM-001`, `AC-COM-003`, `AC-WT-002` or `AC-WT-005`.
