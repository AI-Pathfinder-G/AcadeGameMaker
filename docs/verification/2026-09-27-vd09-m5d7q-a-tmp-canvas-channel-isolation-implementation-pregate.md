# VD09 M5D7Q-A TMP Canvas channel isolation — Luna implementation pre-gate

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Static review only. Unity was not run and no authored/production asset was modified.

## Exact approved inputs

| Item | SHA-256 |
|---|---|
| Approved contract `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md` | `5206546D4140C38A567598D99BF859034ED806EDC062B466ABDD49DF2AE06F75` |
| Approved proposal `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-canvas-channel-isolation-amendment.md` | `34D05E3E92668D97EB916C38FE770F1EB16AA80E22435C9ADCB3A479D916E600` |
| Generator | `8DB852A5D6CAF1DA4883ED3D27DEB909CA5B877DA4D6BA10246E9CAE934E0C90` |
| Focused EditMode tests | `D7A41D0B6F4B6F703D02BC6CD0DF5316DFB95B338128A749928D2CAAA0A13208` |

## Static contract check

- `CaptureSpec` is the sole per-spec helper. Pass A calls it directly for all ten `Specs`; `RenderPass` calls the same helper for pass B. No duplicate phase ordering exists.
- Each spec enters with full `ConfigureCanvas` and exact mask `0`, runs the unchanged Oracle/layout path, forces the real TMP meshes, requires exact mask `25` (`TexCoord1 | Normal | Tangent`) after TMP and immediately before render, then always restores the full Canvas profile and exact mask `0` in `finally`. This yields 20 specs × 4 phases = 80 channel observations.
- `AssertCanvasChannels` independently computes missing and extra bits, rejects both, enforces exact equality and the full capture profile, emits deterministic phase/pass/file markers, and uses the required ordered names for mask `25`. The failure vocabulary and sorted failure list match the approved amendment.
- Six strict TMP paths remain in `AssertTmp`, including static font/material identity, Normal wrapping, overflow/index/character checks, finite mesh vertices, vertex order, and local-rect containment. No TMP bypass or `AssertLayout` relaxation is present.
- The per-spec transaction preserves camera/RT/readback/render globals and aggregates a primary failure with a cleanup failure. Outer capture restoration, dependency hashes/GUID, same-process A/B byte comparison, canonical manifest, `.tmp`/`.prev` atomic publish, rollback, and final validation remain in place.

## Focused test coverage

The focused tests use real project prefab/TMP objects and a real Canvas. They cover representative A/B specs, the exact 0→25→25→0 probe lifecycle, independent missing bits (TexCoord1/Normal/Tangent), independent extra bits (TexCoord2/TexCoord3), non-channel profile drift, faults after TMP/before render/after render/readback, cleanup failure, and primary+cleanup aggregation. Tests also confirm recovery after the aggregate fault and exact `None` after successful cleanup.

## Scope and verdict

The amendment is limited to transient capture Canvas state and diagnostics. Authored Canvas policy, layout, TMP/font/material policy, renderer/output, manifest/state digest, restoration and publication semantics remain unchanged. No new product decision is required.

**PASS — P0=0, P1=0, P2=0.**

This is a static implementation pre-gate; focused Unity execution and capture evidence are still required before integration.

