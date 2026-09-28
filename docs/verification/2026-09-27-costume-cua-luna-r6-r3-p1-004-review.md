# Costume CUA R6 independent mechanics review — R3-P1-004

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Current CUA contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Production adapter SHA-256: `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- Current test SHA-256: `C081E135EBE4FBF240575CB89401652060CDF07CDB87D96F4CDBF690E2434761`

## R6 path coverage

The current test source contains five individually named `AC_CUA_004_R6` paths, and the probe is never passed to the adapter, projection, or CIO:

- first selection success;
- replacement success;
- accepted durable observation;
- rejected observation;
- post-durable projection fault.

Each path captures a `MechanicsProbeSnapshot` before the relevant operation and invokes `AssertMechanicsProbeExact` after the operation. The equality helper compares transform position, rotation and local scale; Rigidbody body type, simulated flag, gravity, mass, linear/angular velocity, constraints, collision mode, interpolation and sleep mode; collider enabled/trigger/offset/size/material; hit/hurt/aim/transfer references and tokens; attack/defense/HP/cooldown/ability set; target reference/token, phase, AI state, simulation counter and frame counter. The five tests preserve the relevant valid/rejected/fault adapter results and projection count where applicable.

The earlier 7-action × 2-facing × `{0,500,999,1000}` completed-snapshot matrix remains present and asserts every mapped frame field against an independently calculated frame.

## Remaining P1

**R3-P1-004 remains OPEN (P1=1).** The probe does not directly capture or assert identity for the Transform, `Rigidbody2D`, or `BoxCollider2D` objects. The proposal and AC-CUA-005 require identity preservation, not only value preservation. `MechanicsProbeSnapshot` has no `TransformReference`, `BodyReference`, or `ColliderReference` fields, and `AssertMechanicsProbeExact` cannot detect replacement of any of those objects.

The probe also does not establish a complete Rigidbody2D settings snapshot. It configures and checks a selected subset (body type, simulated, gravity, mass, velocities, constraints, collision mode, interpolation and sleep mode), but does not snapshot the remaining configured/identity-bearing body settings required by the “complete motion/body settings” clause. This is a concrete coverage gap even though the adapter currently has no probe reference.

Collider material identity is checked, and collider scalar geometry is checked; hit/hurt/aim/transfer/target references and tokens plus stats/abilities/phase/AI and simulation/frame counters are covered. No adapter-to-probe authority path was found.

## Decision

**OPEN / BLOCKED — P0=0, P1=1, P2=0 for R3-P1-004.** The five path structure and existing action/frame matrix are sound, but the mechanics contract is not fully proven until object identities and the complete Rigidbody2D settings/identity snapshot are added to the before/after oracle. Unity execution and Verified restoration remain unauthorized for this finding.
