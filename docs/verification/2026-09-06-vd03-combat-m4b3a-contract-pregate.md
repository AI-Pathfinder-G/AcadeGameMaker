# VD-03 M4B3A Ordan Authored Boss Graph Contract Pre-gate

- Result: PASS
- Reviewer: Luna
- Contract owner and approval: Sol
- Date: 2026-09-06
- Reviewed contract: [M4B3A Ordan Authored Boss Graph](../specs/work-contracts/2026-09-06-vd03-combat-m4b3a-ordan-authored-graph.md)

## Final finding

Luna's final independent read-only review found `P0=0`, `P1=0`, and `P2=0`. Sol accepts the result and approves implementation only inside the reviewed M4B3A allowlist.

## Corrections closed before PASS

- Replaced the contradictory unpacked-prefab wording with one exact connected prefab instance and zero override sets.
- Froze serialized component order for Ordan, audit target, payload targets, Systems, Environment, and all environment collider children.
- Added scheduler `BoundBridge` and `BoundTransferDriver` read-only identity seams and an explicit cross-wire rejection test.
- Required direct proof that the audit target is registered before the first Transfer session while unavailable and collider-disabled, and is neither treated as a payload nor removed by M4B2.
- Required assignment-only mutation tests for every new `ConfigureForAuthoring` seam.
- Defined synthetic 30/60/144 render groupings and the complete immutable view comparison field set.
- Fixed exact focused EditMode and PlayMode test paths.
- Added the M4B3A authored-graph, initial audit state, phase order, and read-only handoff amendment to `SYSTEM-CONTRACTS.md`.

## Authorization boundary

This PASS authorizes the fresh authored graph, audit sink, read-only handoff view, typed assignment seams, validator, builder, and focused tests only. It does not authorize audit exposure, hostile geometry or damage, pull movement, teardown, scene/build integration, reward or room consumption, choice transition, input changes, or final art/audio. Those remain M4B3B or later work.

## Implementation-discovered allowlist clarification

Before Unity or builder execution, Terra correctly stopped because the existing payload sink exposed only a test-labeled assignment caller. Sol clarified the already-approved typed-authoring rule by allowlisting an additive `OrdanBossPayloadTransferModifierSink.ConfigureForAuthoring` caller over the same private assignment implementation. It may not change runtime exposure, application, revision, or validation semantics. This closes the builder's production authoring path without permitting `ConfigureForTests` in production code or `SerializedObject` field injection.

The first successful compile then exposed a Unity nesting fact before any valid scene artifact was accepted: placing the read-only player prefab creates serialized root-name/local-pose modifications in the containing boss prefab. Sol narrowed the prior impossible nested-player zero-override statement to the exact deterministic name, position, identity rotation, and zero Euler-hint properties produced by the builder. All other nested-player property/object-reference and structural overrides remain forbidden.

Builder validation then confirmed the same Unity serialization rule for a root prefab instantiated into an otherwise empty scene: Unity records the unchanged root name, zero local position, identity rotation, and zero Euler hint. Sol therefore replaced the impossible scene-level zero-property requirement with that exact deterministic default set while preserving connected source identity and zero object-reference or structural changes. This matches the already Verified regular encounter scene's serialization shape and does not authorize gameplay or hierarchy drift.

After the user environment moved from Unity 6000.3 to 6000.6, the outer-scene `PrefabUtility` query collapsed redundant identity/zero entries belonging to the nested player even though the prefab YAML and live authored pose remained exact. Sol treats only absent redundant defaults as equivalent: the validator still proves the connected original source and direct pose, checks every surfaced nested modification against the narrow allowlist, and rejects any foreign serialized or structural override.

Implementation review also exposed a wording conflict with the already Verified M4B2 terminal latch. M4B3A now requires the exact death-tick handoff triplet and preservation of the last immutable terminal view while later scheduler/adapter attempts fail-stop. It does not require or permit the adapter to synthesize later empty publications. M4A remains the evidence owner for later empty core handoffs, and M4B3B/later lifecycle integration owns teardown.
