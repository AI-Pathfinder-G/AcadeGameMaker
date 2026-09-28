# M5D7Q-A TMP Canvas channel isolation amendment

- Date: 2026-09-27
- Design and counter-review: Sol (`gpt-5.6-sol`)
- Approval authority: Astra
- Status: bounded proposal — not implementation authority and not a contract edit
- Applies to: `REQ-M5D7QA-006/008/009`,
  `AC-M5D7QA-001/002/007/008/009/010`
- Runtime evidence:
  `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-q.log`
- Retry-q generator SHA-256:
  `1B775BF2FB4011FEB2A57FEBAB3FF0F81D40C8A26BBD3AFCD597A0E5EF72F67F`
- Unity/package boundary: Unity `6000.6`, `com.unity.ugui` `2.6.0`

## Decision

Treat TextMeshPro's Canvas vertex-channel mutation as an exact, transient
render prerequisite and isolate it inside every capture-spec execution.

For each of the ten resolution specifications in each of the two same-process
passes, the required lifecycle is exactly:

```text
spec entry baseline:  None (0)
after TMP mesh update: TexCoord1 | Normal | Tangent (25)
immediately at render: TexCoord1 | Normal | Tangent (25)
per-spec finally:      None (0)
```

The exact required render mask is
`AdditionalCanvasShaderChannels.TexCoord1 |
AdditionalCanvasShaderChannels.Normal |
AdditionalCanvasShaderChannels.Tangent`, numeric value `25`. Missing bits and
extra bits both fail. This is not permission for a variable, package-derived
or superset mask.

The generator must reapply the complete `ConfigureCanvas(canvas, camera)`
profile at the start of every spec in pass A and pass B. Resetting only
`additionalShaderChannels` at spec entry is insufficient because it would
leave the other Canvas capture fields dependent on the preceding iteration.
The mask must not be reset to `None` between TMP mesh generation and
`Camera.Render()`: doing so would remove the vertex attributes that TMP has
just declared necessary for the actual SDF render.

The authored Hub Canvas remains `None`. Runtime/game assemblies, authored
assets, visual layout, copy, font policy, input behavior, capture targets,
manifest schema and state-digest semantics are unchanged.

## Retry-q finding

Retry-q deterministically established the following sequence:

1. `640x360.png` entered `AssertLayout` with
   `canvasChannels=None` and passed layout and all six strict TMP observations.
2. `TMP_Text.ForceMeshUpdate(...)` caused uGUI/TMP to OR numeric mask `25`
   into the Canvas. The locked package source does this to supply the
   additional vertex attributes used by TMP.
3. `ConfigureCanvas` had been called only once before the ten-spec loop, and
   neither pass had a per-spec Canvas reset.
4. `1280x720.png` therefore entered its next `AssertLayout` with
   `canvasChannels=TexCoord1, Normal, Tangent` and failed before its render.

This is cross-spec Editor-capture state leakage, not authored Canvas drift and
not a malformed TMP result. The `640x360.png` render necessarily occurred
with the transient mask installed by TMP; the failure arose because that
transient state became the next spec's baseline.

## Exact per-spec transaction

Terra may implement one common per-spec helper used by both pass A and
`RenderPass`. The two passes must not retain separate orderings. That helper
must perform the following phases in order for every spec:

1. Enter a `try/finally` that covers all mutable render state.
2. Call the existing full `ConfigureCanvas(canvas, camera)` before root/oracle
   configuration. Assert the complete Canvas capture profile and exact mask
   `None`; emit the `baseline` observation.
3. Configure the root and safe-frame oracle, then run the unchanged authoring
   and layout assertions. `AssertLayout` continues to require `None` because
   it observes the pre-TMP capture baseline.
4. Create and validate the exact RenderTexture and Camera target in the same
   deterministic order for both passes.
5. Call `Canvas.ForceUpdateCanvases()`, force each required TMP mesh using the
   unchanged strict TMP path, then force canvases again. Assert that the
   Canvas mask equals numeric `25`; emit the `after-tmp` observation. Run the
   unchanged six-path TMP predicate, all with `failures=NONE`.
6. Immediately before `Camera.Render()`, assert the mask is still exactly
   `25` and emit the `render` observation. Render, read back and validate the
   PNG under the existing render predicates.
7. In the per-spec `finally`, detach the Camera target, restore the captured
   `RenderTexture.active` and `GL.sRGBWrite` values, destroy only current
   temporary objects, restore the complete Canvas capture baseline through
   the common Canvas configuration path, assert exact mask `None`, and emit
   the `finally` observation. The `finally` observation is mandatory on both
   success and injected failure paths.

The next spec may start only after the preceding `finally` assertion succeeds.
The pass-B helper call must use the same code path and phase order as pass A.
After all specs, the existing outer `View.Restore` remains responsible for
restoring the invocation-original authored/editor Canvas and other view state,
then proving exact equality. The per-spec reset to the capture baseline does
not replace or weaken that outer restoration.

If a failure occurs while restoring the per-spec baseline, that restoration
failure must be retained alongside the primary exception and the invocation
must fail. It must never be hidden by the original error or by publication
cleanup.

## Exact observations and failure codes

Each of the four phases for each of ten specs in each of two passes must emit
one machine-parseable marker, for exactly 80 successful markers:

```text
M5D7QA_CANVAS_CHANNEL_OBSERVATION|pass=<A|B>|file=<capture.png>|phase=<baseline|after-tmp|render|finally>|mask=<0|25>|names=<None|TexCoord1,Normal,Tangent>|requiredMask=<0|25>|missingMask=<decimal>|extraMask=<decimal>|failures=<NONE|sorted-codes>
```

`names` uses the exact comma-separated order shown above, without incidental
spaces. Numeric fields are authoritative. The predicate is:

```text
missingMask = requiredMask & ~actualMask
extraMask   = actualMask & ~requiredMask
pass        = actualMask == requiredMask
```

The implementation must reject missing and extra bits independently and then
enforce exact equality. In particular, a mask containing required `25` plus
`TexCoord2` or `TexCoord3` fails; accepting `HasFlag` or a superset is
forbidden. The sorted failure vocabulary is:

- `BASELINE_MISSING_BITS`, `BASELINE_EXTRA_BITS`;
- `AFTER_TMP_MISSING_BITS`, `AFTER_TMP_EXTRA_BITS`;
- `RENDER_MISSING_BITS`, `RENDER_EXTRA_BITS`;
- `FINALLY_MISSING_BITS`, `FINALLY_EXTRA_BITS`;
- `CANVAS_PROFILE_DRIFT` for any non-channel field in the complete configured
  Canvas profile.

Any non-`NONE` marker is followed by an exception that includes pass, file,
phase, actual mask, required mask, missing mask and extra mask. The first
failure stops the capture before staging or publication. A package behavior
change from exact `25` is a fail-closed compatibility finding; this amendment
does not authorize adapting the expected mask.

## Focused deterministic tests

Before retry-r, Astra must admit and Terra must produce the previously unused
immutable result pair
`focused-editmode-canvas-channel-retry-r.xml/.log`. The focused Unity Editor
suite must use a real `Canvas` and real project TMP objects/package code, not a
mocked mask setter, and prove:

1. full `ConfigureCanvas` establishes the complete baseline and exact `None`;
2. the actual TMP force-mesh path changes `None` to exact numeric `25`;
3. the render-boundary predicate accepts exact `25`, and per-spec cleanup
   returns it to exact `None`;
4. repeated representative specs and both pass labels use one common helper
   and cannot inherit a prior spec's mask;
5. independent missing-bit mutations for `TexCoord1`, `Normal` and `Tangent`
   are rejected with the exact phase-specific code;
6. independent extra-bit mutations, including `TexCoord2` and `TexCoord3`,
   are rejected even when all required bits are present;
7. a non-channel Canvas-profile mutation is rejected as
   `CANVAS_PROFILE_DRIFT`;
8. fault injection immediately after TMP update, immediately before render,
   immediately after render and during readback/PNG validation still executes
   per-spec cleanup, yields exact `None`, then allows outer `View.Restore` to
   prove invocation-original state equality;
9. a deliberately failing cleanup is reported without suppressing the primary
   fault; and
10. all six existing strict TMP observations remain `failures=NONE` when the
    exact render mask is present.

The XML result must have zero failed, skipped and inconclusive tests. The log
must record the exact source SHA, Unity/package version, test filter and all
transition/fault observations. A source change after this suite invalidates
the gate and requires a fresh focused run under a newly approved evidence
stem.

No new PlayMode suite is required for this Editor-only capture-state
correction. Existing relevant full/focused/regression evidence remains
immutable and valid unless Luna identifies an actual impacted assertion.

## Capture evidence and publication

All prior results, especially `resolution-capture-retry-q.log`, are immutable
failure evidence and must not be overwritten, deleted or reclassified.

After the focused suite and Luna pre-gate pass at the exact generator/test
SHAs, the next and only admitted capture-attempt stem is:

```text
artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-r.log
```

Retry-r must record all 80 ordered channel observations, all 120 strict TMP
observations (six paths x ten specs x two passes) with `failures=NONE`, the
unchanged render/layout markers, same-process equality, exact authored/source
hashes, restoration equality, atomic publication result and process exit `0`.

Only after retry-r succeeds may the existing authorized clean-process proof
stem `resolution-capture-fresh.log` be used. It must run from the same accepted
source/authored hashes and prove exact byte equality of all ten PNGs and the
manifest against retry-r's published final, as well as the same channel
lifecycle and restoration. Neither log may be reused after a source or
authored-hash change. If retry-r fails, preserve it and stop; another capture
stem requires a new Astra amendment.

The manifest schema, field order and state-digest preimage remain byte/
semantically unchanged. Channel observations are log evidence only and are
not added to the manifest. PNG hashes are calculated from the actual render
and may be accepted only through retry-r/fresh equality; historical byte
identity is not presumed. The existing `.tmp`/`.prev`/final transaction and
rollback rules remain unchanged.

## Failure, restoration and rollback

- Preflight, baseline, after-TMP, render-mask or focused-test failure stops
  before staging. Remove only the current `.tmp`; do not alter a valid final.
- A post-stage failure restores `.prev -> final` exactly under the approved
  publication transaction. Never publish a partial set or a manifest whose
  PNG hashes were not produced by the accepted render.
- Every exception path must attempt the per-spec `None` reset and the outer
  invocation-original restoration. The final process result is failure if
  either restoration does not prove equality.
- No scene, prefab, font, material, ProjectSettings, package, `.meta` or
  runtime asset may be saved to make the test pass. Authored dependency and
  GUID hashes must equal their pre-invocation values.
- Rollback of an implementation candidate removes only the bounded generator
  and focused-test delta, restores their recorded pre-change bytes and leaves
  retry-q and all newly generated failure evidence immutable.
- No adaptive retry, channel-superset acceptance, TMP bypass, font/material
  substitution, `AssertLayout` relaxation or render-before-assertion is
  authorized.

## Traceability, approval and product decision

| Technical delta | Existing requirement | Existing acceptance criteria |
|---|---|---|
| isolate transient channel state across resolution specs while preserving exact layout/render behavior | `REQ-M5D7QA-006` | `AC-M5D7QA-007` |
| fail closed on missing/extra bits and restore on every fault boundary | `REQ-M5D7QA-008` | `AC-M5D7QA-009`, `AC-M5D7QA-010` |
| deterministic Editor-only generator/test/evidence delta with authored hash equality | `REQ-M5D7QA-009` | `AC-M5D7QA-001`, `AC-M5D7QA-002`, `AC-M5D7QA-008` |

No requirement, acceptance criterion, traceability mapping or product
semantics changes. This amendment only makes the approved capture generator
compatible with the locked uGUI/TMP render lifecycle while retaining an exact
authored/pre-spec baseline and exact fail-closed diagnostics.

No user product decision is required. Astra must approve this bounded
amendment and any exact result-name allowlist before Terra implements it. Luna
must independently pre-gate the implementation/source SHAs and the complete
test/filter/evidence plan before retry-r, then post-review the focused XML/log,
retry-r, `resolution-capture-fresh.log`, manifest/PNG equality, restoration
markers and final changed-file allowlist before Astra integrates.
