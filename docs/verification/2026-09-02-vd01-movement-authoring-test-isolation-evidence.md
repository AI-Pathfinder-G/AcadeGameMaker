# VD-01 Movement Authoring Test Isolation Evidence

- Date: 2026-09-02
- Status: Verified — Luna PASS (`P0=0`, `P1=0`, `P2=0`)
- Owning contract: `docs/specs/work-contracts/2026-08-25-vd01-movement-sandbox.md`
- Requirements: `REQ-MOV-001`, `REQ-MOV-002`, `REQ-MOV-005`
- Acceptance evidence: `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-005`

## Cause and user adjudication

The legacy EditMode authoring test called the production `MovementSandboxAuthoringBuilder.Build()` entry and regenerated `Assets/Scenes/MovementSandbox.unity` during every full EditMode run. The approved scene geometry remained the same, but Unity regenerated local file IDs and root order. The pre-run SHA-256 `6843D9FFCC74E553FF337167A42E397842AC305A4100584D25E04E70B4A7D4E9` changed to `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`.

The prior exact bytes were absent from Git history, reflog, stash, Unity backup directories and workspace hash search; restoring HEAD would have discarded the preexisting working change. The user explicitly approved the regenerated current scene as the new protected baseline on 2026-09-02.

## Correction

- The production menu and internal `Build()` entry remain fixed to the approved production scene and prefab paths.
- An internal `BuildForTests` entry accepts only `.unity` and `.prefab` paths beneath one literal temporary test root. It rejects production destinations, parent traversal, backslashes, sibling-prefix paths and wrong extensions before asset creation.
- Test mode uses the shared builder core but does not call global `AssetDatabase.SaveAssets()`.
- The EditMode test hashes the production scene and prefab, builds and validates only disposable assets, closes the temporary scene and deletes exactly the temporary root in `finally`, then requires both production hashes to remain identical.
- No runtime code, gameplay behavior, project setting or public surface changed.

## Executable evidence

| Run | Result | XML SHA-256 | Protected result |
|---|---:|---|---|
| Movement authoring isolation focus | 3/3 passed, 0 failed, 0 skipped | `9167B43749B6FD48B750C0ADB9200BE96684FAD19D1F060464992C205D2558C2` | scene remained `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`; prefab remained `B31D45569AFBC852AF6A92EBA33885CD4182776B98DDA345940F9D60862A2C68`; temporary root absent |
| Full EditMode with isolation | 265/265 passed, 0 failed, 0 skipped | `50624AC92D3C358225482ED3C853E1585BB46B57BA46437B16FA999BBF7D6305` | same scene and prefab hashes; temporary root absent |

Direct compilation passed for the Movement Authoring Editor assembly (`673E1165E5531ACEC3F4E7A2A3A2A830731E839F02BB8C7BF5379BDA0E17F704`) and Movement EditMode Tests (`0BED83A83D76F55EA75A463F5149D7B2BC5A95106AE5E4A78C36EFB12A6AF3FE`).

## Ollama utilization record

- **Kimi K3 — used and accepted in part:** its bounded review reinforced directory-boundary, traversal, production-path, cleanup and hash-oracle cases. Suggestions that depended on invented asset types or broader canonicalization APIs were rejected.
- **GLM 5.2 — used and accepted in part:** its adversarial matrix reinforced sibling-prefix, separator, extension, exception cleanup and stale-temp cases. Suggestions requiring new public APIs or project-wide state were rejected.
- **MiniMax M3 — failed and replaced:** the bounded fixture request returned an empty final response after consuming its output budget, without quota, authentication, network or model-availability error. Terra's focused fixtures and Luna review replace it.

No cloud model received repository content, local paths, credentials, personal data or decision authority.

## Independent review

Luna verified the fail-closed path boundary, disposable-root cleanup, production hash oracle and both executable runs. The final result is `P0=0`, `P1=0`, `P2=0`; the user-approved regenerated Movement scene is the protected baseline for subsequent work.
