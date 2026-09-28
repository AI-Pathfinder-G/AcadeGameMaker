# M5D7Q-A clean-bootstrap implementation static recheck R9

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow format-overload and camera-diagnostic review; no Unity launch,
  execution, or edit
- Generator SHA-256: `DEDE3FEFF150794399DDF0765072CA9D2A8EB4A92475E5FB4839805058D4D80A`
- Prior PASS generator SHA-256: `18FA31363366275B0CCE2701D9E6EF36B0C9E5D98A2648A6C6E30955514DFC1F`

## Verdict

**BLOCKED — P0=0, P1=1, P2=0.**

## P1 finding

**CB-IMPL-P1-CAM-001 — camera diagnostic is not deterministic.**

The new `M5D7QA_CAMERA_TARGET_OBSERVATION` marker includes
`targetTextureId` and `requestedTargetId` from `GetInstanceID()`. Unity object
instance IDs are process/allocation dependent and can differ between pass A/B
and fresh Unity processes. They therefore cannot be part of a deterministic
camera observation. Replace them with stable fields (for example
`targetEqualsRequested`, requested/actual dimensions, and fixed target
presence), or explicitly remove the IDs from the deterministic marker while
retaining any non-contract diagnostic elsewhere. Then re-gate the new SHA.

## Closed checks

- All four `IsFormatSupported` calls now use the intended
  `GraphicsFormatUsage.Render` overload; exact color/depth preflight remains
  before temporary output and Hub opening.
- The camera marker is emitted before the unchanged `pixelRect`/target
  predicate and its requested/actual dimensions, rects, target equality, and
  fixed invariant formatting are otherwise appropriate.
- No RT/camera pass/fail relaxation, bootstrap classification change, terminal
  marker change, TMP/pass A-B change, save/SetDirty/CloseScene call, hash/GUID
  change, or rollback change was found.

This is a deterministic-evidence gate finding only and does not introduce a
user product decision.
