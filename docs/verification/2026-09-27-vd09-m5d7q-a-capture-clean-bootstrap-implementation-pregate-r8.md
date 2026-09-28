# M5D7Q-A clean-bootstrap implementation static recheck R8

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow exact render-support preflight recheck; no Unity launch,
  execution, or edit
- Generator SHA-256: `18FA31363366275B0CCE2701D9E6EF36B0C9E5D98A2648A6C6E30955514DFC1F`

## Verdict

**PASS — P0=0, P1=0, P2=0 (narrow static recheck).**

`ValidateCanonical()` now checks non-Null graphics, exact
`R8G8B8A8_SRGB` render support, and exact `D24_UNorm_S8_UInt` render support
after `State.Take()` but before temporary-directory creation and
`OpenScene(Hub)`. The same checks remain defensively duplicated in `NewRt()`.

`NewRt()` retains strict descriptor and actual assertions: exact graphics and
depth-stencil formats, width/height, legacy ARGB32 view, 24-bit depth, MSAA=1,
and the fixed mip/dimension/random-write profile. No relaxation was found.

The prior bootstrap snapshot/classification and terminal gates, RT diagnostic,
TMP/pass A-B, canonical manifest, GUID/hash, no-save, and staged
publication/rollback logic remain present. No Save/SetDirty/CloseScene call is
present. This PASS is limited to the exact source hash and does not claim
Unity compilation, runtime evidence, or final `Verified` acceptance.
