# VD09 M5D7Q-A resolution capture retry-r independent review

Date: 2026-09-27 (Asia/Seoul)  
Reviewer: Luna (independent evidence gate)  
Scope: evidence-only review; no Unity execution and no implementation edits.

## Inputs and identity

| Item | SHA-256 / result |
|---|---|
| Capture log `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-r.log` | `7B3456857989B9BBDB6A73DA179136F4E71958D7B9A058F6527D07AE8B4551E9` |
| Final manifest `artifacts/unity-results/m5d7qa-20260923/resolution-captures/manifest.json` | `374AE7CA4169BB2C08FE08E9F18DC2E657B78773A4ECE57A33808984CD9E80C2` |
| Generator `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs` | `8DB852A5D6CAF1DA4883ED3D27DEB909CA5B877DA4D6BA10246E9CAE934E0C90` |
| Focused tests `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubPresentationAuthoringTests.cs` | `D7A41D0B6F4B6F703D02BC6CD0DF5316DFB95B338128A749928D2CAAA0A13208` |
| Contract token in the log | `5206546D4140C38A567598D99BF859034ED806EDC062B466ABDD49DF2AE06F75` |

The command line is the required terminal capture invocation (`-batchmode -quit -projectPath … -executeMethod AcadeGameMaker.Hub.Authoring.Editor.HubPresentationCaptureGenerator.Capture`) and carries the expected contract, generator, and test hashes. The log ends with Unity batchmode exit code `0`.

## Independent evidence checks

- Canvas channel markers: exactly 80. Ordered pass A then B, ten files each, with each file in the exact `baseline → after-tmp → render → finally` sequence. Every tuple matched the required masks: `0 → 25 → 25 → 0`, names `None → TexCoord1,Normal,Tangent → TexCoord1,Normal,Tangent → None`, missing/extra masks `0`, and `failures=NONE`.
- TMP markers: exactly 120 (six exact paths × ten targets × two passes). Each six-entry block retained the expected path order; each path occurred 20 times and every marker had `failures=NONE`. The six paths are the four menu labels plus notification dismiss/message.
- Camera/layout/render observations: 20 each; all observation records have no non-`NONE` failure state. The render records retain the requested/actual target dimensions and the contract-required color/depth profile.
- Same-process proof: one `M5D7QA_SAME_PROCESS_CAPTURE|passes=2|png=10/10|manifest=true|equal=true` marker.
- Terminal preflight and completion: the log records one clean unsaved bootstrap classification, accepted `PROCESS_BOUNDARY_NO_SAVE`, and completion with `hubLoaded=true`, `hubDirty=false`, `persistentHashesEqual=true`, `globalsRestored=true`. The process has one explicit scene snapshot; no dirty scene or restoration failure was reported. This terminal mode correctly reports `initialSceneRestored=false` and `zeroSceneRestored=false` as a process-boundary fact rather than claiming an impossible empty-setup restoration.
- Published set: the directory contains exactly the 11-file allowlist (10 PNG + `manifest.json`), with no extra files. Sibling `resolution-captures.tmp` and `resolution-captures.prev` paths are absent, and no `.tmp`/`.prev` entries exist inside the final directory.
- Manifest: UTF-8 no-BOM, two-space/LF canonical bytes are established by the expected manifest SHA; top-level keys are exactly `schemaVersion,captures`, schema version is `1`, and there are ten ordered captures. Each capture has the expected schema fields, a 64-hex state digest and PNG digest, and `assertionPassed=true`.
- PNGs: every manifest PNG digest equals the independently computed SHA-256. PNG signatures and IHDR dimensions match the manifest for all ten outputs: 640×360, 1280×720, 1920×1080, 2560×1440, 1366×768, 3440×1440, 720×1280, 639×359, and the two 1280×720 quarantine frames. The below-minimum capture is correctly `supported=false`/suppressed; quarantine frame 0/1 suppression state differs as expected.
- Authored immutability: the generator’s exact dependency-hash set and fixed-GUID checks are present at preflight, before publication, and after restoration. The completion marker reports `persistentHashesEqual=true`; no authored-dependency drift, GUID failure, rollback failure, or capture assertion failure appears in the log. The generator and focused-test source hashes match the expected identities above.

Unity licensing diagnostics include an access-token warning, but entitlement resolution succeeded and the capture completed with exit code 0; this is not a capture gate failure.

## Gate verdict

**PASS — P0=0, P1=0, P2=0 for the retry-r evidence gate.** `resolution-capture-fresh.log` is authorized to run as the next separate clean-process gate. This verdict does not accept fresh-process byte identity or visual quality; those remain pending and must be reviewed from the fresh log/set before final acceptance.

