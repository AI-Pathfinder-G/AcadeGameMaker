# M5D7Q-A capture-runtime blocker amendment pre-gate

Date: 2026-09-27  
Reviewer: Luna (independent narrow pre-gate)  
Scope: proposal and Review-contract audit only; no Unity launch, implementation edit, or capture execution.

## Review identity

- Proposal: `docs/proposals/2026-09-27-vd09-m5d7q-a-capture-runtime-blocker-amendment.md`
- Proposal SHA-256: `2E2ADA39EAC1283ABD53A2C6F86C3BEB9EE05EAB9CFC7EBD5253A5C56F5F4F70`
- Review contract SHA-256: `8C6DAD8825A8BB161EA739495303BC64EF86000E7A7E792FA217A17E1FDBA65E`
- Implementation SHA named by the proposal: `5B26C2D99BEC3C4A6984848532271AC60E098DEBB1F7B9E6516930A3E08940AE`
- Direct blocker evidence: `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-e.log`

## Independent checks

### TMP local-space predicate and diagnostics

The amendment separates invariant-culture diagnostic observations from the
authoritative predicate. It uses each TMP component's own `RectTransform.rect`
as the sole coordinate system, checks finite rect/measurement values and exact
zero margins, requires both TMP overflow indicators to be clear, verifies exact
source-character records and canonical font/material ownership, and proves all
four mesh vertices of every non-space character are actual mesh vertices inside
the same local rect with no epsilon. Empty Message is the only empty-mesh case.
The fixed observation line records preferred size and text bounds without
misusing either as a quad-containment oracle; sorted failure codes preserve
actionable diagnostics before assertion. No copy/font/size/layout/atlas change
is authorized by a failure.

### Terminal empty-setup boundary

The two modes are explicit. Non-empty `SceneSetup` retains exact restoration and
equality. Empty setup is accepted only in batchmode with exactly one `-batchmode`,
`-quit`, and the exact `-executeMethod` token, no `-runTests`, zero loaded scenes,
and invalid active scene, before Hub or output mutation. The terminal mode does
not claim impossible zero-scene restoration; it records
`zeroSceneRestored=false`, leaves only the canonical clean unsaved Hub in memory,
and relies on immediate process exit as the isolation boundary. This is a clear
and auditable distinction from ordinary Editor-session restoration.

### No-save, state, and dependency proof

The terminal success conditions require canonical active Hub, clean scene, no
Save/SetDirty APIs, unchanged authored dependency hashes, restored globals/view,
and fixed accepted/complete markers. The proposal retains temporary-object
detachment/destruction and output rollback rules. It explicitly forbids treating
an incomplete set or rollback failure as accepted evidence.

### Same-process and fresh-process determinism

For terminal mode, the proposal correctly specializes same-process repeatability
to two independent ten-spec render/assert/encode passes inside one entrypoint,
reapplying the authoring state and comparing all PNG and manifest bytes before
publication. It does not falsely claim two public invocations when zero-scene
restoration is impossible. A separate fresh Unity process remains mandatory and
must match the final set byte-for-byte.

### Evidence, rollback, and traceability

The amendment preserves the ten-PNG/manifest allowlist and adds only the named
`resolution-capture-retry-f.log` evidence stem; the contract already carries the
fresh-process stem. Failure before publication removes only this invocation's
temporary set; post-stage failures restore the prior final or absence state;
rollback failure is aggregated and nonzero. The proposal maps the local TMP,
terminal session, immutability, restoration, and determinism changes to
`REQ-M5D7QA-006/008/009` and `AC-M5D7QA-007/009/010` without changing product or
runtime semantics.

## Decision gate

No user product decision is required: copy, font, layout, interaction, runtime
behavior, capture list, and manifest schema remain unchanged. Astra must still
approve the bounded contract delta before Terra implements it; Luna must then
pre-gate the exact corrected source and post-review the two Unity runs. Any
actual TMP mesh/layout failure that would require product-facing changes remains
outside this amendment and requires a separate Astra contract decision.

## Verdict

**PASS — P0=0, P1=0, P2=0 (proposal pre-gate).**

This PASS authorizes no implementation by itself and does not accept the failed
retry-e log or any historical capture set.

