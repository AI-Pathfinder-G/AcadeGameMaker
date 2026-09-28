# M5D7Q-A TMP menu-label layout correction — Luna pre-gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: proposal-only independent pre-gate; no Unity execution and no implementation edit
- Proposal: `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-label-layout-correction-addendum.md`
- Proposal SHA-256: `749CBF21D147304C6C5B992AA5B720681C80BC9B3EDD76B2E737D4A47A102353`
- Review contract: `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- Review contract SHA-256: `9FE5BF0B8688CBDF87DDBB631A8D0BDC47DA131614332D568275DA829C8D5621`
- Runtime cause evidence: `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-o.log`
- Baseline generator SHA-256: `6368164FC84F1AA7B0E0F4A4C4F9C49A11E88E4DD829A07ACA00B01D7471E6CD`

## Verdict

**PASS — P0=0, P1=0, P2=0.** Terra may implement the bounded correction only
after Astra restores the contract to `Approved`. Astra may approve this
technical delta without a new user product decision; the Korean copy, typeface,
font size, button geometry, center, interaction and runtime semantics remain
unchanged. This pre-gate is not implementation acceptance or `Verified` status.

## Independent checks

### Cause and minimal geometry

- Retry-o provides a concrete real-Unity cause: the exact Continue label has
  preferred height `20.28` in a `168x20` local rect and reports
  `OVERFLOW_FLAG,OVERFLOW_INDEX`, while its actual SDF mesh remains contained.
  The strict TMP predicate is correctly retained; no waiver or tolerance is
  introduced.
- The all-four correction `(12,6,168,20)` -> `(12,5,168,22)` preserves the
  exact `(96,16)` label center inside unchanged `192x32` buttons. It is the
  smallest larger even-height integer profile that preserves the integer center
  and gives deterministic margin `5` on both vertical sides.
- The proposal uniformly applies the profile to Continue, NewGame, Settings and
  Quit, while explicitly freezing button/hit geometry, copy/font/wrapping/
  alignment/material/state cues, menu spacing, SafeFrame and notification paths.

### Bounded migration and validation

- Builder migration is exact all-four legacy-profile only, in memory, atomic at
  the prefab path, with full candidate validation and reload validation. New
  canonical input is a no-op; mixed/partial/foreign/unrelated drift fails
  closed rather than being repaired.
- Validator additions cover exact paths, anchors/pivot, position/size/center,
  scale/rotation/margins, font/copy/size/wrapping/alignment/material and
  prefab-instance override policy. The proposed positive, independent negative
  mutation, no-op, rollback and same/fresh-process matrices cover the failure
  boundaries without granting general repair authority.
- Scene reserialization is conditional and scoped: unchanged inherited scene
  bytes remain the required result when Unity does not need a canonical rewrite.
  `.meta` GUIDs and all unrelated runtime/art/package/project assets are
  forbidden to change.

### Evidence, rollback and traceability

- The exact new evidence names are enumerated and contract-allowlisted:
  `scope-before`, builder pass/fresh layout logs, focused and full layout
  suites, four regression layout suites, `resolution-capture-retry-p.log`,
  `scope-after`, and `final-manifest`. Existing retry-a through retry-o and
  prior evidence remain immutable.
- Pre-save bytes/hashes/GUIDs are retained; candidate failure writes no asset;
  save/reload or test failure restores only the exact bounded pre-change files;
  capture failures retain prior final output through existing `.tmp/.prev`
  rollback. No adaptive height growth, font shrink, predicate relaxation or
  unrelated regeneration is authorized.
- REQ/AC mapping is narrow and preserved: geometry to
  `REQ-M5D7QA-006`/`AC-M5D7QA-007`, deterministic migration/rollback to
  `REQ-M5D7QA-009`/`AC-M5D7QA-002,010`, and strict capture proof to
  `REQ-M5D7QA-006,009`/`AC-M5D7QA-007,010`. No IDs or mappings change.

## Gate state

The contract is intentionally `Review` for this addendum. Astra approval must
precede Terra's bounded implementation, followed by Luna's exact-source gate,
Terra's allowlisted evidence runs, and Luna's independent post-review.
