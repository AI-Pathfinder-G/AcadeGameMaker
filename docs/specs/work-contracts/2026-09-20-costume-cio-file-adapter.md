---
status: Verified
---

# Costume CIO file adapter

- Date: 2026-09-20
- Status: Verified — Astra final integration on 2026-09-27 after Luna closed
  `AC-CIO-007` at `PASS — P0=0, P1=0, P2=0`
- Owning spec and revision: [Costume file IO and additive Unity presentation adapter](./2026-09-13-costume-io-and-unity-adapter.md), Child A / `REQ-CIO-001..008`
- Parent foundation: [Approved costume presentation and sprite-production contract](./2026-09-13-costume-presentation-and-sprite-production.md), bounded pure-core implementation accepted for child development
- Assigned by / final authority: Astra
- Implementer: Terra
- Independent verifier: Luna
- Requirement IDs: `REQ-CIO-001..008`
- Acceptance-criterion IDs: `AC-CIO-001..007`
- Rollback point: current pure `AcadeGameMaker.Costumes` implementation and its bounded Luna evidence

## Bounded outcome

Add one costume-only filesystem adapter that persists and recovers canonical `CostumeStateV1` bytes. It owns exactly three fixed filenames below one caller-supplied, normalized, absolute, non-root directory:

- `costume-state-v1.json`
- `costume-state-v1.previous.json`
- `costume-state-v1.temp.json`

It does not choose the eventual application root, connect a menu, import media, change a renderer, grant an outfit, or edit Profile/Run authority. The current real Seryeong catalog remains all `ProductionPending`; `NoAcceptedDefault` is a successful read-only unavailable result and performs no write.

## Public contract and invariants

- Consume the existing public `CostumeCanonicalCodecV1`, `CostumeRecoveryInputV1`, `CostumeRecoverySelectorV1`, `CostumeRecoveryPlanV1`, and immutable `CostumeStateV1` APIs without widening or editing them.
- Reject null, empty, relative, filesystem-root, traversal, alternate-name, and role-aliasing inputs before filesystem mutation.
- Observe each role at most once. Preserve `Missing`, `Read`, and `ReadFailed` as distinct typed outcomes with defensive byte copies.
- Delegate byte validity and deterministic recovery to the pure selector. Never merge candidates or bootstrap over an unread role when authority remains uncertain.
- Save only canonical bytes correlated to the exact expected state and revision. The fixed transaction is: validate; write exact temp; flush managed and durable buffers; atomically move/replace; reopen primary; require byte equality and successful decode.
- Distinguish `CommittedFirst`, `CommittedReplacement`, `FailedBeforeCommit`, and `CommitOutcomeUncertain`. Once move/replace begins, failure never causes retry, cleanup, overwrite, promotion, rollback claim, or success claim.
- Do not quarantine/delete, discover roots, use clocks/RNG/network/Unity APIs, or call Profile/Run services.

### Approved Windows durability and path profile

- This child targets the approved Windows desktop vertical-demo profile on a local NTFS volume only. Other platforms and filesystems fail closed and require a separate amendment.
- The caller supplies an already-existing directory. Normalize it once with `Path.GetFullPath`; trailing separators normalize away except for the rejected filesystem root. Reject UNC/network paths, drive roots, non-NTFS volumes, and any existing root/ancestor or owned role that has `FileAttributes.ReparsePoint`.
- Derive all three role paths internally from the normalized directory and fixed filenames. Compare canonical role paths with `StringComparer.OrdinalIgnoreCase`; reject aliasing, escape, and any path whose parent is not the exact normalized directory. Callers never provide a role filename.
- Use `FileStream.Flush(true)` for the durable temp flush. Use `File.Replace(temp, primary, previous)` for replacement and a no-overwrite same-volume `File.Move(temp, primary)` for the first commit. If the runtime cannot prove these primitives and same-volume local-NTFS conditions, return a typed pre-commit failure.
- Keep `Missing`/`Read`/`ReadFailed` dispositions outside `CostumeRecoveryInputV1`. When an unread role could change a selector result that treats null as absent, return typed load failure instead of publishing bootstrap/`NoAcceptedDefault`; a separately proven valid higher-authority candidate may still be selected according to the documented precedence.

## Exact allowlist

Only these paths may be added or minimally changed:

- `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs` and `.meta`
- `docs/verification/2026-09-20-costume-cio-contract-pregate.md`
- `docs/verification/2026-09-20-costume-cio-implementation-evidence.md`
- `docs/verification/2026-09-20-costume-cio-luna-independent-review.md`
- the minimum status/link hunk in `docs/README.md`
- this contract's status/front matter and approval record

No existing runtime source, test, asmdef, Profile/Run file, scene, prefab, asset/media, package manifest, or ProjectSetting may change. If an existing public API is insufficient, implementation stops for an Astra-approved amendment.

## Requirements

- **REQ-CIO-001:** IO owns only the three fixed costume filenames below the validated caller-supplied directory and never reads/writes Profile or Run storage.
- **REQ-CIO-002:** Each role is observed at most once into defensive immutable evidence; missing, read failure, invalid bytes, and valid bytes remain distinct.
- **REQ-CIO-003:** Load delegates recovery to the pure core, never merges candidates, and never bootstraps over uncertain unread authority.
- **REQ-CIO-004:** Save follows the exact durable-temp, atomic move/replace, primary-reopen verification order and never increments state revision.
- **REQ-CIO-005:** Pre-commit and uncertain-commit failures remain distinct; uncertain commit never triggers retry, cleanup, promotion, false rollback, or false success.
- **REQ-CIO-006:** `NoAcceptedDefault` is read-only unavailable and causes no save, grant, unlock, selection, binding, or media operation.
- **REQ-CIO-007:** This unit does not quarantine/delete, choose a composition root, modify pure APIs, use Unity, or reuse Profile authority.
- **REQ-CIO-008:** Implementation stays inside the exact allowlist.

## Acceptance criteria

- **AC-CIO-001 (REQ-CIO-001/008):** tests reject invalid roots and role aliasing, prove every opened path is one of the three exact owned names under an isolated root, and static diff proves allowlist compliance.
- **AC-CIO-002 (REQ-CIO-002/003):** missing/read-failed/corrupt/valid permutations prove one observation per role, defensive bytes, deterministic precedence, no merge, and no bootstrap when unread data leaves authority uncertain.
- **AC-CIO-003 (REQ-CIO-004):** first-save and replacement traces prove exact stage order, durable temp flush, atomic operation, primary reopen, byte equality, decode, and unchanged state revision.
- **AC-CIO-004 (REQ-CIO-005):** a fault injected at every stage proves the correct failure boundary, no automatic retry/cleanup/promotion, and no success without reopened-primary equality.
- **AC-CIO-005 (REQ-CIO-006):** all-pending catalog with missing/corrupt/valid-empty state yields `NoAcceptedDefault`, zero writes, and no fabricated current/unlock/binding.
- **AC-CIO-006 (REQ-CIO-007):** static review finds no Profile/Run service, delete/quarantine, root discovery, Unity API, mutable static port, clock, RNG, network, or existing-core edit.
- **AC-CIO-007 (all):** focused and full EditMode plus full PlayMode pass with failed/skipped/inconclusive `0`; Luna independently verifies evidence and Astra decides acceptance.

## Required evidence and order

1. Luna pre-gate records contract consistency, public seam sufficiency, failure boundaries, and allowlist safety. **Complete:** PASS with P2 clarifications on 2026-09-20.
2. Astra changes this contract to `Approved` with an approval record. **Complete:** approved on 2026-09-20 with the Windows durability/path profile above.
3. Terra writes a local impact brief, implementation, tests, and evidence citing every `REQ-CIO-*` and `AC-CIO-*` ID.
4. Luna independently runs adversarial and regression verification; Terra may not accept its own work.
5. Astra integrates only after Luna reports no unresolved P0/P1.

The later Unity presentation adapter, real media acceptance/import, hub wardrobe composition, input routing, and in-play sprite swap are separate gates. This contract alone never claims a player-visible wardrobe.

## Participation record

- Sol supplied the bounded child split, atomic persistence constraints, and later wardrobe composition boundaries.
- Astra authored this implementation contract and retains approval/integration authority.
- Terra and Luna participation will be recorded in the named evidence files after their actual work.

## Astra approval record

Astra accepts Luna's pre-gate with `P0=0`, `P1=0`, and the three P2 clarifications incorporated normatively above. Terra may implement only the CIO child and exact allowlist. This approval does not authorize CUA, wardrobe composition, real-media import, catalog promotion, or an in-play sprite swap.

## Astra final integration — 2026-09-27

The reviewed CIO runtime, test, and asmdef hashes remain byte-identical to
Luna's 2026-09-20 corrected implementation review. The later Unity 6000.6
complete-suite evidence passes 788/788 EditMode and 947/947 PlayMode with
failed/skipped/inconclusive all zero; the EditMode result contains all nine
`CostumeFileAdapterV1Tests`, each passed. Luna independently confirmed that
the former sole P1 was incomplete full-suite execution, not an implementation
defect, and closed `AC-CIO-007` at `PASS — P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cio-ac007-closure-review.md`, SHA-256
`A0839B317261192EB48F505AF09BDC6DFFCE6898A7901B2E4530FA1C36467A91`.
Astra accepts `REQ-CIO-001..008` and `AC-CIO-001..007` and marks this contract
`Verified`. The exclusions above remain unchanged.
