# M5C2 fixed-room Unity camera evidence

- Status: Verified — Astra accepts local integration after Luna independent final AC001..005/code/XML/hash review PASS, P0/P1/P2=0, 2026-09-08.
- Contract: [Approved M5C2](../specs/work-contracts/2026-09-08-vd07-m5c2-fixed-room-camera-driver.md)
- Authority/integration: Astra; runtime and Editor helper implementation: distinct Terra workers; independent runtime tests/review: Luna; actual InputRouter integration fixture: Astra, reviewed by Luna.
- Baseline: M5C1 Verified, full EditMode474 + PlayMode539 =1013.

## Pre-gate

Terra implementability and Luna independent contract review passed after two corrections. Installed URP resolves to17.6.0 in package-lock and package.json while manifest retains its17.3.0 request; this unit changes neither package file and does not claim an installed17.3 source. The private serialized PPC filter has no public runtime API, so a bounded Editor SerializedObject helper applies/checks Point while the runtime validates only public profile values. Public profile/binding/rotation/scale/Z drift is checked before each advance; private filter drift is not claimed as runtime coverage.

The unsupported viewport policy continues the pure core at every completed Movement tick but publishes no camera DTO or new Transform until supported output returns. This prevents permanent tick misalignment while preserving the approved minimum640x360; it does not introduce a smaller supported resolution or gameplay hard-snap event. Bootstrap publishes no seed DTO. Luna final pre-gate: PASS, P0/P1/P2=0.

## Execution and review

Root serialized all Unity runs with source writers frozen. Initial compilation failed because PixelPerfectCamera belongs to the nested Unity.RenderPipelines.Universal.2D.Runtime assembly; the targeted assemblies now explicitly reference it. No package was installed or changed. The first executed21-case run exposed two fixture errors: releasing a RenderTexture while still bound to Camera, and expecting a queued Transfer payload for a genuinely empty neutral frame. Both fixture oracles were corrected; no production behavior was relaxed. Subsequent21 and final28 cases passed.

Root review hardened exact reference binding, initialization fault handling, mutation-free phase rejection, exact rotation/scale checks and authoring preconditions. Luna independently reviewed these changes and root's additional nonzero pose, serialized-reference drift and actual re-enable cases. AC-M5C2-001..005: PASS with no remaining runtime or coverage blocker.

| Run | Result | Evidence file | SHA-256 |
|---|---|---|---|
| Focused Camera + actual InputRouter | 28/28 | TestResults-Unity-PlayMode-20260908-195415.xml | E6594C556AF75CF937E7E596B23D145B539F76B7260AD4B10A9FD75101A308FC |
| Focused Editor profile | 6/6 | TestResults-Unity-EditMode-20260908-195243.xml | 3994EAF6E9E8A4D3A58F1BC6AA28D69F2DEDDC25ED11BF327DB2F20FE76BF6F6 |
| Full EditMode | 480/480 | TestResults-Unity-EditMode-20260908-195459.xml | C09707E35FE469B0A8AFD909A4B85BEE578C9724B280920CABDCD1871FA348CA |
| Full PlayMode | 567/567 | TestResults-Unity-PlayMode-20260908-195550.xml | D23B1BE35D90EFC1C4AD488510150AEF5098E6B6F4BC768A9E1114B589794373 |

All zero failures/skips, Unity exit0. Full total1047 includes the focused cases. AC001 covers genuine completed Movement→camera→next-input pairing and no seed; AC002 supported/odd viewport rectangles plus6 Editor profile cases; AC003 nonzero pixel18 Transform versus Q100 and no render feedback; AC004 continuous gap/current-tick recovery/no delayed attack; AC005 initialization/binding/profile/failure-stage/disable-reenable closure.

Package hashes unchanged across execution: manifest 7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25; lock C0DA832620B8DB22C1E607D3E2638680C29186A871F55FA6FA5B453DD27432A9. Scoped whitespace check passed. Preserve all pre-existing dirty worktree changes.

RenderTexture dimension fixtures do not establish GPU image quality. No playable scene, anchor, Run transition, pixel-art image certification or private filter runtime enforcement is claimed.
