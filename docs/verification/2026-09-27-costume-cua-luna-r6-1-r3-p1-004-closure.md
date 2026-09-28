# Costume CUA R6.1 independent closure review — R3-P1-004

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Current CUA contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Production adapter SHA-256: `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- R6.1 test SHA-256: `51CF23D69FF8C4AFD78174191ECA78A31B01D47BD0765D6F279A142B96CA0DCF`

## Common mechanics oracle

`MechanicsProbeSnapshot.Capture` and `AssertMechanicsProbeExact` now form one common before/after oracle used by all five R6 paths. It asserts:

- `GameObject`, `Transform`, `Rigidbody2D`, and `BoxCollider2D` identity with `SameAs`;
- owner/component/attached relationships, including body and collider transforms, collider `attachedRigidbody`, attached-collider count/query count, and every returned attached-collider identity;
- transform position, rotation and scale;
- the requested Rigidbody2D source-of-truth settings: body type, simulated, auto-mass, full-kinematic contacts, gravity, mass, linear/angular damping, local/world center of mass, inertia, body position/rotation, velocities, constraints, freeze-rotation value and constraint bit, collision mode, interpolation, sleep mode, include/exclude layers, and shared material identity;
- collider enabled/trigger/offset/size/material;
- hit/hurt/aim/transfer references and tokens;
- attack/defense/HP/cooldown/ability set;
- target reference/token, phase, AI state, simulation counter and frame counter.

The five tests call the same oracle after first selection, replacement, accepted observation, rejected observation, and projection failure. The probe is private and is never passed to the adapter, projection or CIO.

## Exclusion rationale

The source comment explicitly excludes `IsSleeping`/`IsAwake` and accumulated force/torque because they are physics-step transients; these tests are synchronous and do not advance simulation. It also excludes Unity 6 legacy aliases (`velocity`, `drag`, `angularDrag`, `isKinematic`) because the oracle captures their source-of-truth properties (`linearVelocity`, `linearDamping`, `angularDamping`, `bodyType`). This is a valid non-duplicative boundary for the requested configuration/identity invariant and does not weaken the adapter isolation claim.

The prior seven-action × two-facing × `{0,500,999,1000}` completed-snapshot matrix remains intact in the same test file.

## Decision

**CLOSED / PASS — P0=0, P1=0, P2=0 for R3-P1-004.** The former identity and complete-body-settings gap is closed statically. This is not Unity execution evidence; broader execution and release approval remain separate Astra decisions.
