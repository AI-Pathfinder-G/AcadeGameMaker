# M5C1 camera follow core evidence

- Status: Verified — Astra accepts local integration after Luna independent runtime/AC/XML review PASS, P0/P1/P2=0, 2026-09-08.
- Contract: [Approved M5C1](../specs/work-contracts/2026-09-08-vd07-m5c1-camera-follow-core.md)
- Authority/integration: Astra; runtime: Terra; independent test/review: Luna
- Baseline: M5B5 Verified, EditMode 440/440 + PlayMode 539/539 = 979.

Terra implementability review and Luna independent contract pre-gate passed with no P0/P1/P2 findings. The contract explicitly chooses target-change age1 and the last committed interpolation offset, superseding the less precise age-zero wording in the design proposal. It keeps exact Q73728 arithmetic through final pixel18 snapping and the existing Q100 projection boundary.

The runtime is confined to a new engine-free Camera assembly referencing Core and Movement. Astra's initial static review found the checked staging, independent axes, final snap order and signed rounding consistent with the Approved contract. Luna's independent fixture, runtime review and final XML/hash inspection subsequently passed as recorded below.

The unit does not implement a Unity camera, completed camera DTO publication, viewport/rendering, anchors, room/respawn/teleport lifecycle, camera-driven input integration or playable scene. Those remain later work, not implied by core acceptance.

## Executed verification

Root serialized Unity 6000.6.0f1 runs with all source writers frozen. Licensing preflight passed; no authentication or environment changes were necessary. Luna authored the independent fixture and reviewed the runtime against AC-M5C1-001 through 005. Astra corrected a test-oracle assumption before execution: a freely moving camera centered at pixel18 +/-100 would follow a player at zero, so the projection sweep now uses fixed equal bounds. No runtime behavior was weakened.

| Run | Result | Evidence file | SHA-256 |
|---|---|---|---|
| Focused AC-M5C1-001..005 | 34/34 | TestResults-Unity-EditMode-20260908-193629.xml | F346E8DE28157505F13950702A044993EE356FD3C25EAC12F14A1E42E7E1A8C9 |
| Full EditMode | 474/474 | TestResults-Unity-EditMode-20260908-193658.xml | E4B1460515B09B9001C4EAB781457DE6421BA00BB540AFF6563D8034F6DA79EB |
| Full PlayMode | 539/539 | TestResults-Unity-PlayMode-20260908-193801.xml | 1DBF9FF7D758C28FD73C5C2422C45A0C67FE4CAC6B06C3A95DACF39ED94B10B0 |

All runs: zero failed, zero skipped, Unity exit0. Full-suite total1013; the focused34 are included, not added again. AC001 threshold/retarget/full vertical transition; AC002 inclusive/fractional dead-zone and independent caps; AC003 bounds; AC004 signed projection/overflow/rejected tick preservation; AC005 scripted render-group replay and regression passed. Scripted grouping is not measured physical FPS or visual camera acceptance.
