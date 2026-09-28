# M5D7Q-A clean-bootstrap implementation static recheck R7

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: exact source review only; no Unity launch, execution, or edit
- Generator SHA-256: `1DD41BB2AA187703D5035FD81D58C1B5E427D0FAB72C1019CB319F7DDD75F9DD`
- Prior diagnostic PASS SHA-256: `5561A418B351EC2AC4CC39C66772F467AABC5B256D021AE7AE30A29FEB1AB101`

## Verdict

**BLOCKED — P0=0, P1=1, P2=0.** The explicit format/type assertions are
plausible and do not relax the required depth-24 profile, but the new exact
surface-support preflight is too late for the approved contract.

## P1 finding

**CB-IMPL-P1-RT-001 — explicit depth/color support is checked after Hub
opening and temporary-output mutation.** `ValidateCanonical()` (before temp
directory creation and `OpenScene`) checks only the legacy
`SupportsRenderTextureFormat(RenderTextureFormat.ARGB32)` surface. The new
`SystemInfo.IsFormatSupported(R8G8B8A8_SRGB, FormatUsage.Render)` and
`SystemInfo.IsFormatSupported(D24_UNorm_S8_UInt, FormatUsage.Render)` checks
are inside `NewRt()`, which is reached only after `Directory.CreateDirectory`
and `EditorSceneManager.OpenScene`.

The capture proposal/contract require an unsupported graphics surface to fail
before opening Hub. A device can support the legacy ARGB32 query while not
supporting the exact D24_UNorm_S8_UInt descriptor, so the current ordering does
not prove that requirement. Add the exact color/depth support checks to the
pre-open canonical preflight (retaining the `NewRt()` checks as defense), then
recompute the source SHA and re-gate.

## Closed checks

- `GraphicsFormat.R8G8B8A8_SRGB` plus `D24_UNorm_S8_UInt` is a plausible Unity
  6000.6/C#9 API choice. Descriptor fields are explicit and actual descriptor,
  `r.format`, `r.depth`, size, MSAA and creation assertions remain strict.
- The existing ARGB32/depth-24/sRGB semantics are not relaxed; the issue is
  only preflight timing and coverage of the exact depth format.
- RT diagnostic requested/actual marker, bootstrap snapshot/classification,
  terminal markers, no-save/process boundary, TMP/pass A-B, GUID/hash, and
  staged publish/rollback logic remain present.

This is an implementation gate finding, not a product decision. Unity
compilation and runtime evidence were not executed.
