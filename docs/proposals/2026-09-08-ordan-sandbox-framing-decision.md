# Ordan playable sandbox — framing decision

- Status: Resolved by user, 2026-09-08; exact implementation still requires an Approved work contract
- Decision: OD-M5D1-001
- Resolution: Early bosses including Ordan use a single-screen arena; middle encounters expand, the final boss uses multi-phase giant/device combat, and the true boss is a narrow post-seal duel. See ADR-0028 and boss-encounter-direction canon. The alternatives below remain the historical proposal, not an open decision.
- Predecessors: M5B5, M5C1, M5C2 Verified. Latest full regression1047/1047.
- Assessment: Terra read-only integration analysis; Astra scope/decision boundary.

## Evidence and next outcome

The existing Approved M4B3A arena authors a24u ground span, side walls centered at x=-12/+12 and fixed player/boss/weight positions. Approved VD-08 fixes the camera frame at approximately35.56x20u (18PPU,640x360,ortho10), irrespective of supported output resolution. A camera cannot both show only the existing arena and retain that fixed scale without surrounding scenery. Do not silently enlarge the battlefield or change zoom/crop to hide this mismatch.

The next bounded integration should connect actual InputRouter + Camera to the authored sandbox and safely request Transition before the first post-death input frame. A terminal-only owner at-211 can read completed boss-death authority and request the next exact mode before router-210; this does not fabricate Run state, rewards or scene progression. Existing M5A cannot yet be connected by inventing RunActive, failure state or persisted Choice/Skill defaults; those owners remain later dependencies.

## User-visible choice

Recommended: preserve the existing combat geometry and distances. Compose a single-screen boss arena, with non-playable surrounding machinery/background occupying the excess camera frame. Fixed room bounds and initial composition can then be authored under a new Approved contract; exact coordinates/Z and wiring are Astra-owned technical details, not separate user questions.

Alternative: enlarge the playable arena so camera tracking is meaningful. This changes movement distances, boss approach/avoidance and obstacle layout and requires renewed combat/layout acceptance; it is not merely a camera setting.

The existing graph is a simulation/authoring sandbox, not a visually accepted playable scene. Device/camera integration alone does not supply missing visible art, HUD, menu, Run, choice or reward behavior. No scene/prefab or combat-layout change has been made under this proposal.
