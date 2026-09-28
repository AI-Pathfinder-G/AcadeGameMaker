# Kimi K3 VD-01 M2 Adapter Design Review

- Date: 2026-08-25
- Draft source: Ollama Cloud `kimi-k3:cloud`, bounded non-secret architecture prompt
- Screening authority: Sol
- Status: Screened; selected ideas only, draft has no approval authority
- Requirements: `REQ-MOV-001~004`, `REQ-MOV-006~010`
- Acceptance criteria: draft input toward `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-004`, `AC-MOV-005`, `AC-MOV-006`

## Accepted input

- Prefer kinematic casts over dynamic-force simulation for a deterministic motor.
- Keep the adapter as a translation and validation layer, not an independent simulation owner.
- Cover tunneling, corner contacts, wall-ID stability, dash obstruction, and replay equality in PlayMode tests.

## Rejected input

- Rejected intentional one-frame divergence between motor snapshot and visual/body position. It violates the frozen sole-authority and immutable-payload invariants.
- Rejected Unity instance IDs and cached instance-derived IDs. Stable authored IDs are required.
- Rejected multiple `Advance` calls as collision substeps inside one fixed tick. One authoritative `SimulationTick` must advance exactly once.
- Rejected render-frame hysteresis for grounded state. The adapter must emit an already validated current-tick grounded signal.
- Rejected a test that expected the snapshot to retain an unclamped intended position while the body was clamped.

## Sol integration decision

M2 uses a kinematic, axis-separated cast adapter and an internal same-tick collision-resolution commit owned by the motor. This preserves the public interface while ensuring the final `PlayerMotionSnapshot` and Rigidbody target are identical. The exact rules are frozen in the M2 section of the VD-01 work contract.

## Screened implementation-skeleton follow-up

Sol supplied the frozen M2 rules back to Kimi for a bounded file-by-file skeleton. The follow-up correctly repeated the requested public-controller/internal-test-seam split and proposed useful ground, ceiling, left/right wall, blocked-dash, stable-ID, malformed-command, equality, and render-rate replay test categories.

The proposed code itself was rejected. It used wrong repository paths and namespaces, invented `Q4096Vec2`, mutable contact fields, `SimulationTick.Current`, and a dash-schedule object that do not exist, constructed the pure motor with Unity references, placed collision casts inside the motor, and duplicated tick ownership between controller and motor. Terra may use only the screened test decomposition, not the code skeleton.
