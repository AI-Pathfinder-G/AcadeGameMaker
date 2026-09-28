# M5B3 device action asset evidence

- Status: Verified; Luna independent PASS and Astra integration approval, 2026-09-08
- Contract: [M5B3](../specs/work-contracts/2026-09-08-vd07-m5b3-device-action-asset.md)
- Requirements: REQ-UX-001/006/007/009, REQ-PLAT-002/007

## Ownership and preflight

Astra approved the bounded contract after Terra confirmed the installed official importer/code-generator route and Luna passed the independent pre-gate. Terra owns the asset/importer metadata/runtime assembly/bootstrap edit; Luna owns independent virtual-device fixtures and review; Astra runs sequential Unity and accepts integration. No installed package, project setting, scene, prefab or media changes belong to this unit.

The installed Input System importer generates GameInputActions directly from Assets/GameInput.inputactions into the runtime Input assembly. The generated wrapper is not authored manually. This unit does not claim production InputMode exclusivity, next-tick InputRouter routing or physical XInput hardware verification.

Unity preflight passed for editor 6000.6.0f1, matching signed Hub/embedded licensing clients 1.18.3, an existing entitlement file and no competing project editor/client. No license restoration or new installation was needed.

Root's pre-import static check rejected the first source draft: 50 map/action/binding identifiers had an eleven-character final UUID segment. Terra was instructed to correct these unaccepted source identifiers before first import and validate all IDs as GUIDs, with uniqueness checks. JSON parse alone was insufficient; no accepted asset identity was replaced.

## Execution

The corrected source has 61 valid, unique map/action/binding UUIDs and four valid distinct meta GUIDs. The asset GUID is `7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f`; the generated wrapper GUID is `a0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5`.

First test launch `Unity-QA-PlayMode-20260908-180521.log` stopped at compilation: the fixture referenced the wrapper before Unity generated it. No test result exists for that attempt. Root temporarily gated the new test assembly for first import, ran the official importer successfully (`Unity-M5B3-FirstImport.log`, exit 0), then restored unconditional test compilation. The generated wrapper came from Unity, not a placeholder or handwritten replacement. Initial six focused tests subsequently passed. Root strengthened per-binding evidence with independent button/direction cases; final results follow below.

Unity replaced the orphan wrapper meta during first import; root restored the reserved wrapper GUID after generation and confirmed it remained stable through the final full PlayMode run. No scene references existed to the temporary generated GUID.

| Final result | Passed/total | SHA-256 |
|---|---|---|
| TestResults-Unity-PlayMode-20260908-180802.xml (focused) | 39/39 | B87DA225F31C426167AA997A92CA6C3E327D2408EC2C444C9D5FCAF97589D108 |
| TestResults-Unity-EditMode-20260908-180858.xml (full) | 440/440 | 49FBD26F1EA7BAD6E700BABE45535A3AAA24EE622C2E1F3F37097F3110D864EC |
| TestResults-Unity-PlayMode-20260908-180944.xml (full) | 435/435 | E89FD85D64C4E8EF326FF4F0E360F5F9E62BFA67739CFF0C543847C031C70CBB |

All final runs: failed 0, skipped 0, Unity exit 0. There are 875 full-suite cases; the focused 39 are a subset, not additional tests. This unit adds 39 PlayMode cases to the previous 836-test baseline.

Luna independently verified the results/hashes and reviewed AC-M5B3-001..005: PASS, P0/P1/P2 = 0. The asset has exactly two maps, 16 actions and 43 bindings, all 61 source IDs valid and unique. The wrapper matches source and official regeneration; individual button/direction checks prevent one working scheme from masking a broken alias. Bootstrap and prior gameplay regressions remain green. Astra accepts this bounded unit. Actual device-to-simulation routing, camera, authoritative InputMode and physical hardware acceptance remain subsequent work, not claims of this result.
