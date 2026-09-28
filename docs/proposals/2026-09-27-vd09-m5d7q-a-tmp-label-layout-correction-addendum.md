# M5D7Q-A TMP menu-label layout correction addendum

- Date: 2026-09-27
- Design and counter-review: Sol (`gpt-5.6-sol`)
- Approval authority: Astra
- Status: proposal — not implementation authority and not a contract edit
- Applies to: `REQ-M5D7QA-006/009`, `AC-M5D7QA-002/007/010`
- Runtime evidence:
  `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-o.log`
- Retry-o generator SHA-256:
  `6368164FC84F1AA7B0E0F4A4C4F9C49A11E88E4DD829A07ACA00B01D7471E6CD`
- Parent technical proposal:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-capture-runtime-blocker-amendment.md`

## Decision

Correct only the four menu-label RectTransforms from `(x=12, y=6, w=168,
h=20)` to `(x=12, y=5, w=168, h=22)` inside their unchanged `192x32`
buttons.

This is the minimum measured correction. It preserves each label rect center:

```text
old center = (12 + 168/2, 6 + 20/2) = (96, 16)
new center = (12 + 168/2, 5 + 22/2) = (96, 16)
```

The button bounds, hit rectangle, copy, font, font size, wrapping, alignment,
material, focus/hover/disabled visuals, menu spacing, safe frame and every
runtime/input semantic remain unchanged. The new label rect stays wholly inside
the button with left/right margins `12` and bottom/top margins `5`.

No font shrink, copy change, preferred-size bypass, overflow-flag waiver,
fallback, auto-size, clipping, masking or tolerance is allowed.

## Measured cause

Retry-o reached the real Unity 6000.6 capture path, passed terminal bootstrap
classification and exact Camera/RenderTexture predicates, then emitted:

```text
path=SafeFrame/MenuPanel/Continue/Label
rect=0,0,168,20
preferred=51.52,20.28
textBounds=0,-0.272001266,51.518055,20
meshBounds=0.490972251,2.47423387,49.92118,15.5336094
overflow=true
firstOverflow=0
failures=OVERFLOW_FLAG,OVERFLOW_INDEX
```

The actual SDF mesh is safely inside the local rect, but TMP's own vertical
layout requirement is `20.28`, exceeding the authored height `20` by `0.28`.
The accepted no-overflow predicate therefore correctly stopped before a PNG.
It must not be relaxed.

All four primary labels use the same Regular face, font size `14`, zero margin,
Normal wrapping and exact `168x20` geometry. The correction applies uniformly
to Continue, NewGame, Settings and Quit so the authored menu does not gain a
label-specific geometry exception. The proposed height `22` provides `1.72`
logical pixels beyond the observed preferred height while preserving the exact
visual center. Actual pass status remains a Unity acceptance result, not a
paper assumption.

## Exact authored geometry

The only changed authored values are:

| Exact path under `HubMenuRoot` | Old rect | New rect |
|---|---|---|
| `SafeFrame/MenuPanel/Continue/Label` | `12,6,168,20` | `12,5,168,22` |
| `SafeFrame/MenuPanel/NewGame/Label` | `12,6,168,20` | `12,5,168,22` |
| `SafeFrame/MenuPanel/Settings/Label` | `12,6,168,20` | `12,5,168,22` |
| `SafeFrame/MenuPanel/Quit/Label` | `12,6,168,20` | `12,5,168,22` |

For all four, anchors and pivot remain exact bottom-left `(0,0)`, local scale
remains `(1,1,1)`, rotation remains identity and margin remains zero. Existing
horizontal and vertical alignment values remain unchanged; the validator must
assert them rather than relying on package defaults.

The following are explicitly unchanged:

- button rects `192x32` at their current four positions;
- `Label` width `168`, x `12`, center `(96,16)` and font size `14`;
- four Korean strings and all notification TMP geometry/copy;
- SafeFrame/MenuPanel geometry, hit rectangles and 24/12 minima;
- focus rail, border and disabled strike geometry;
- capture target list, oracle, manifest schema and authoring-state token.

## Bounded implementation surface

After Astra approval, Terra may change only the following implementation and
evidence surfaces for this correction:

- `HubPresentationAuthoringBuilder.cs`: change the four shared label creation
  arguments from `12,6,168,20` to `12,5,168,22`; add a bounded migration from
  the exact legacy four-label profile to the new profile.
- `HubPresentationAuthoringValidator.cs`: validate all four exact paths,
  anchors, pivot, position, size, scale, rotation, alignment, font/copy and
  zero margins. The exact legacy profile and partial/mixed migrations fail.
- `HubPresentationCaptureGenerator.cs`: assert the four new logical label
  rects before every TMP observation. The existing strict local-space
  mesh/rect/overflow/glyph predicate is unchanged.
- `HubPresentationAuthoringTests.cs` and
  `HubPresentationScopeAuditTests.cs`: positive, no-op and independent geometry
  mutation coverage.
- `HubMenuResolutionPlayModeTests.cs`: actual prefab and scene-instance
  effective geometry plus TMP overflow/mesh/copy checks. Other PlayMode test
  sources change only if an exact existing assertion embeds the old rect.
- `Assets/Prefabs/Hub/HubMenuRoot.prefab`: the four RectTransform position/
  height values only.
- `Assets/Scenes/Hub.unity`: only if Unity must reserialize the canonical
  prefab instance to retain zero overrides. If the scene continues to inherit
  the corrected prefab with identical bytes, unchanged scene bytes are the
  required result, not a reason to force a rewrite.
- this addendum, the owning contract/index/evidence hunks authorized by Astra,
  and the exact result stems below.

All `.meta` GUIDs remain unchanged. Runtime assemblies, Q0, font/SDF/atlas/
material/TMP Settings/shader/license assets, packages, ProjectSettings, input,
profile, persistence, costume, gameplay and notification assets are forbidden.

### Deterministic migration rule

The builder remains a no-op for the new canonical profile. It may migrate only
when all four labels exactly match the legacy profile and the rest of the
prefab/scene already passes the approved validator surface.

1. Record exact before bytes/SHA-256 for builder, validator, tests, generator,
   prefab, scene and all their metas before the migration run.
2. Load the prefab through Unity's prefab authoring API, change the four
   RectTransforms together in memory, validate the complete candidate and
   save the prefab once at its existing path.
3. Revalidate the canonical Hub scene as exactly two prefab roots, Q0 with zero
   overrides and HubMenuRoot with zero unauthorized overrides. Save the scene
   only if Unity requires a canonical reserialization; otherwise do not touch
   it.
4. Reload prefab and scene from disk and validate all four new label rects.
5. A mixed profile, one-to-three legacy labels, any unrelated drift or any
   save/reload validation failure is fail-closed; it is not repaired.
6. Same-process second builder invocation and fresh-process invocation must be
   byte no-ops for prefab, scene and all metas.

The migration is one bounded approved canonical update, not general repair
authority.

## Validator and test matrix

### Positive assertions

- all four exact label paths are present once;
- each has `anchoredPosition=(12,5)`, `sizeDelta=(168,22)`, bottom-left
  anchors/pivot, unit scale, identity rotation and zero margins;
- rect center is exactly `(96,16)` in its button and the rect stays within
  `[0,192] x [0,32]`;
- buttons, hit rectangles, copy/font/size/wrapping/alignment/material and state
  cue geometry remain byte/structurally unchanged apart from the four rects;
- scene inherits the prefab result with no label override;
- valid builder runs are same-process and fresh-process byte no-ops.

### Independent negative mutations

Reject each of the following without repair:

- old `y=6,h=20` on all four or any one label;
- `y=5,h=20`, `y=6,h=22`, `y=4,h=22`, `y=5,h=21` or `y=5,h=23`;
- x/width drift, non-bottom-left anchors/pivot, non-unit scale or rotation;
- only one, two or three labels migrated;
- center drift despite sufficient height;
- button/hit rect, copy, font size, wrapping, alignment, margin or material
  drift;
- prefab-instance override in `Hub.unity` used to hide an invalid prefab.

### TMP capture acceptance

For every target and both same-process render passes, all six exact TMP paths
must emit observations with `failures=NONE`. In particular, all four menu
labels must report:

- local rect `0,0,168,22`;
- `overflow=false` and `firstOverflow=-1`;
- exact copy, Regular Static font, size `14`, zero margin and canonical
  material/shader;
- exact character/visible/non-space counts;
- finite mesh bounds wholly inside the local rect.

Dismiss and Message retain their existing exact rects/sizes/copy rules and
must also report `failures=NONE`. Any failure stops before publication; the
new height is not accepted merely because Continue passes once.

## Exact execution and evidence sequence

The owning contract must admit the following unused result stems before they
are produced. Existing retry-a through retry-o and all other prior evidence
remain immutable.

1. `scope-before.json`: snapshot the approved pre-change allowlist and hashes.
2. `builder-pass-f.log`: execute the exact four-label migration, reload and
   validate the prefab/scene, then execute the same-process no-op proof.
3. `builder-fresh-process-layout.log`: fresh Unity process builder no-op and
   exact post-migration byte/GUID proof.
4. `focused-editmode-layout.xml/.log`: exact geometry positives, independent
   mutation matrix, migration gating, same-process no-op and rollback probes;
   failed/skipped/inconclusive all zero.
5. `focused-playmode-layout.xml/.log`: actual prefab/Hub scene geometry,
   centered visuals, TMP no-overflow and unchanged hit/input/notice behavior;
   failed/skipped/inconclusive all zero.
6. `regression-m5d7o-layout.xml/.log`,
   `regression-m5d7pa-layout.xml/.log`,
   `regression-m5d7q0-layout.xml/.log`, and
   `regression-m5d7n-layout.xml/.log`: unchanged controller, semantic cursor,
   router and handoff boundaries; failed/skipped/inconclusive all zero.
7. `full-editmode-layout.xml/.log` and `full-playmode-layout.xml/.log`: fresh,
   unfiltered full suites after the authored migration; failed/skipped/
   inconclusive all zero.
8. `resolution-capture-retry-p.log`: corrected source SHA, exact terminal
   bootstrap markers, all six TMP observations `failures=NONE` for every one
   of ten targets in pass A and pass B, same-process PNG/manifest equality,
   authored hash equality, atomic publication and exit `0`.
9. `resolution-capture-fresh.log`: separate clean Unity process at the exact
   same source/authored SHA; prior-final equality for all ten PNGs and
   manifest, all TMP observations clean, hash equality and exit `0`.
10. `scope-after.json` and `final-manifest.json`: record the exact changed-file
    set, source/authored hashes, XML counts, capture PNG/manifest hashes and
    immutable prior blocker evidence.

The state-digest preimage and semantic token remain unchanged because button,
copy, focus, notice and interaction state are unchanged. PNG hashes are
recomputed from the corrected actual scene. Byte identity with any historical
unaccepted PNG is neither assumed nor required. `resolution-capture-retry-p`
alone does not satisfy fresh-process proof or final acceptance.

## Failure and rollback

- Before migration, retain exact byte snapshots of the existing prefab/scene
  and all files authorized for change; do not alter `.meta` files or GUIDs.
- If candidate validation fails before save, destroy the in-memory candidate
  and write no authored asset.
- If save/reload or any post-save validation fails, restore only the exact
  pre-invocation prefab/scene bytes at their same paths, refresh, prove the
  original hashes/GUIDs and stop. Do not regenerate fonts or Q0.
- If focused or regression suites fail, preserve their logs/XML as failure
  evidence, revert the bounded implementation/authored delta to the recorded
  pre-change bytes and do not run capture.
- Capture preflight/TMP/render failures remove only current `.tmp`; an existing
  valid final remains untouched. Post-stage failures restore `.prev -> final`
  exactly under the existing capture rollback policy.
- A rollback failure, mixed old/new label profile, authored hash drift,
  changed button center/hit rect, any TMP failure code, capture byte drift or
  nonzero required run is a stop condition. No adaptive height growth beyond
  `22`, font shrink or predicate relaxation is authorized.
- Prior logs, especially `resolution-capture-retry-o.log`, are never
  overwritten, deleted or reinterpreted as success.

Rollback removes only this proposed layout delta from builder/validator/
generator/tests/prefab/scene and restores their recorded prior bytes. It does
not remove immutable evidence logs and does not touch runtime, fonts, packages,
ProjectSettings or unrelated assets.

## Traceability and user decision

| Delta | Requirement | Acceptance criteria |
|---|---|---|
| four-label rect correction with unchanged visual center | `REQ-M5D7QA-006` | `AC-M5D7QA-007` |
| deterministic migration, validation, bytes/GUIDs and rollback | `REQ-M5D7QA-009` | `AC-M5D7QA-002`, `AC-M5D7QA-010` |
| strict TMP observations and same/fresh capture proof | `REQ-M5D7QA-006/009` | `AC-M5D7QA-007/010` |

No REQ/AC ID or traceability mapping changes. `REQ-M5D7QA-007` font policy,
`REQ-M5D7QA-008` fail-closed runtime boundaries and all runtime/input/menu
effect requirements remain unchanged.

No new user product decision is required. The correction preserves the exact
user-selected Korean copy, Noto Sans CJK KR face, font size, button geometry,
visual center and behavior. It is a technical authored-layout correction for
the already-approved no-overflow requirement. Astra must approve this bounded
contract delta and Luna must pre-gate the exact implementation before Terra
runs `builder-pass-f`.
