# Seryeong A1/H1 first-media offline staging — Luna independent review

- Date: 2026-09-20
- Scope: Approved `2026-09-20-seryeong-a1-h1-first-media-package.md`, Terra's offline tool, pending outputs, package manifest, and implementation evidence
- Method: static adversarial inspection and engine-free probes only; no code/media edits and no Unity execution
- Disposition: **P0=0, P1=2, P2=0. Do not accept the packer/validator yet. The staged media itself remains explicitly pending.**

## Findings

### P1 — Existing manifest is not safely ownership-checked before mutation

`_build()` calls `_prepare_stage()` before `_prepare_manifest()`. A test-root probe built the five pending stage files, changed the sibling manifest marker to a foreign value, then called the test build: it rejected the manifest only after deleting all five pending files (`5 → 0`). In production mode, `_prepare_manifest(test_only=False)` checks only `packageId`, not `producerMarker`; a package-ID-only manifest was accepted by the helper probe. This conflicts with the README/evidence claim that unowned manifests reject before writes.

`write_json()` also truncates the existing package manifest in place. A temporary hard-link probe (`nlink=2`) showed that writing the approved manifest path overwrote the linked peer file. The result is output-path spill outside the allowlisted path, despite the lexical target path being fixed.

**Trace:** `REQ-SM1-005/010`, `AC-SM1-006/010`.

**Minimum correction:** complete all stage and manifest ownership checks before unlinking anything; require the exact producer marker for production manifests; publish the manifest by creating a fresh file and atomically replacing the authorized leaf (or explicitly reject multiply-linked files). Add adversarial tests for foreign/missing markers, preservation of the prior stage on rejection, and hard-linked manifest targets.

### P1 — Validator accepts identity/revision corruption after rebinding its hash

On isolated temporary builds, I changed `stage-manifest.json` to `mediaRevision=99` or `portraitSetId="foreign.portrait"`, recomputed the package's `stageManifestSha256`, and ran `validate_for_test()`. Both malformed packages were accepted. Changing the package manifest's `stageRoot` to `outside/root` was also accepted. The current validator checks selected identifiers and payload hashes, but does not verify these normative revision/set/root fields against the fixed A1/H1 package.

**Trace:** `REQ-SM1-005/010`, `AC-SM1-006`.

**Minimum correction:** validate the complete canonical identity/revision/geometry/path schema, including expected portrait/gameplay set IDs, `mediaRevision=1`, and exact stage root; add mutation tests that update dependent hashes so the test proves semantic validation rather than merely a stale digest rejection.

## Independent checks

- Bundled Python engine-free suite: **4/4 passed**. The production `validate()` path also passed read-only.
- Actual Windows directory-junction probe was rejected by `_assert_no_reparse_chain`. The test suite's mocked reparse-point probe also rejects; the regular symlink creation attempt was unavailable in this environment, so no symlink-specific OS result is claimed.
- The tool has no production output-path arguments; its CLI accepts only `build` or `validate`. No recursive deletion call exists. It checks the ownership marker and exact stage inventory; unknown stage and pending payloads fail closed. The unit test confirmed an unknown stage file survives rejection.
- All six pinned sources passed `require_pins()`. Current tool, test, README, four pending diagnostic outputs, stage manifest, and package manifest hashes match Terra's evidence.
- Current package is `PendingMissingLeftAndRightsAttestation`, rights are `PendingUserAttestation`, `missingLeft=true`, `completeAtlas=false`, and `clipMap=null`. The stage contains only the right-facing diagnostic, preview diagnostic, and manifests. No accepted output is claimed.
- The A1/H1 tool writes only to its fixed staging root and manifest; tests used isolated system-temp roots. The script has no Unity or `Assets/` write path. The current APV3 inventory still reports `acceptedCombinationCount=0`; no Unity/catalog/APV3 mutation was made by this review.

## AC-SM1 disposition

| Criterion | Luna disposition | Reason / remaining gate |
|---|---|---|
| `AC-SM1-001` | **Pending — not accepted** | Only a partial right-facing diagnostic exists, not the 13-frame final package or contact-sheet identity audit. |
| `AC-SM1-002` | **Partial** | The 128×128 RGBA preview and 512×512 4× diagnostic are produced; manual 64px body measurement, final 32px atlas, and 640×360 stage review do not exist. |
| `AC-SM1-003` | **Fail-closed staging pass; final AC pending** | Missing left is explicitly 0/6; no complete two-facing 12-frame matrix or clip map is emitted. |
| `AC-SM1-004` | **Pending** | Left-facing authoring and native direction/landmark review are absent. |
| `AC-SM1-005` | **Pending** | No final 32px frame/pivot/continuity review exists. |
| `AC-SM1-006` | **FAIL — P1** | Deterministic temporary builds and basic corruption tests pass, but semantic revision/set/path corruption probes were accepted; manifest ownership/write-boundary probes also fail. |
| `AC-SM1-007` | **Pending** | Rights chain is correctly left at `PendingUserAttestation`; acceptance is prohibited until the project owner's attestation is recorded. |
| `AC-SM1-008` | **Staging boundary pass** | APV3 remains 0/432, package/catalog truth is pending, excluded media is not used, and this tool/test path made no Unity/catalog/APV3 changes. |
| `AC-SM1-009` | **Not run — separate future contract** | Unity import/publication is explicitly out of scope and remains blocked by CUA/CIO/M5D7Q0 and the default/wearability decision. |
| `AC-SM1-010` | **Partial** | Terra evidence and this independent review exist; Astra's final disposition remains required. |

## What Astra may accept now

Astra may accept the **bounded existence of pending, non-addressable diagnostic artifacts** and the truthful claims that source pins match, missing-left coverage is blocked, rights remain pending, and APV3/runtime catalog were not promoted. Astra should **not accept the packer/validator as production-safe** until both P1 findings are corrected and independently re-probed. No media may be called `AcceptedForVerticalDemoMedia`; AC-SM1-001..005 and AC-SM1-007 remain open, and AC-SM1-009 is a later import gate.

## 2026-09-20 dated re-review

- Reviewer: Luna (independent post-fix review)
- Scope: Approved contract, `tools/seryeong_first_media/**`, pending production stage and package manifest, Terra implementation evidence, and this review's prior findings
- Method: direct engine-free suite, actual production read-only validation, isolated production/test marker and semantic-corruption probes, SHA-256 recheck; no code or media edits and no Unity launch
- Disposition: **P0=0, P1=1, P2=1. The output-boundary P1 is closed; the semantic stage-manifest P1 remains. Keep the media pending and non-wearable.**

### Prior P1 follow-up

1. **Output ownership and preflight — CLOSED.** Directly exercised test and production build/validate paths with both a missing `producerMarker` and a poisoned marker. All eight calls rejected before mutation; the complete stage file inventory and bytes were unchanged. The direct 7-test suite also passed the hard-link case with the linked peer bytes and stage snapshot unchanged. This closes the prior manifest-ordering, production-marker, and hard-link spill finding (`REQ-SM1-005/010`, `AC-SM1-006/010`).
2. **Canonical semantic validation — OPEN, P1.** The outer package manifest now rejects mutation of `mediaRevision`, both set IDs, `stageRoot`, `packageId`, `disposition`, rights, APV3 count, catalog disposition/default grant, `completeAtlas`, and `clipMap`. However, changing the inner `stage-manifest.json` and recalculating the outer `stageManifestSha256` still passes `validate_for_test()`: `mediaRevision=99`, foreign portrait/gameplay set IDs, APV3 count `1`, `runtimeCatalogDisposition="Accepted"`, and non-null `payload.clipMap` were all accepted. The inner or outer `schemaVersion=true` is accepted too (`true == 1` in Python). `payload.completeAtlas=true` and disposition/rights escalation do reject. The validator therefore does not yet enforce the complete typed canonical schema or all stage-manifest identity/revision/claim fields, contrary to the prior finding's minimum correction and `AC-SM1-006`.

   **Minimum correction:** validate the exact stage-manifest key set, exact types, and every normative identity/revision/rights/APV3/catalog/default/atlas/clip-map value; use type-strict checks for integer fields. Add re-hashed inner-manifest mutation tests so rejection cannot be explained by a stale outer digest. Keep the no-mutation assertion for every rejected build/validation path. Trace: `REQ-SM1-005/009/010`, `AC-SM1-006/008/010`.

### Direct verification and hashes

- Bundled Python engine-free suite: **7/7 passed** (`AC-SM1-002/003/006/008/010`). It confirms current tests for deterministic rebuilds, package-field rejection, marker preflight, hard-link peer preservation, and path boundaries; it does not cover the re-hashed inner-manifest cases above.
- Actual production `validate`: exit 0 with `PENDING_FAIL_CLOSED: staging is valid but intentionally non-acceptable.` It was read-only (`AC-SM1-003/006/008/010`).
- Independent production/test marker probes: missing and poisoned marker rejected on build and validate with the entire stage inventory and bytes unchanged (`AC-SM1-006/010`).
- All six Approved source pins match the contract. Every current staging output and package manifest matches Terra's recorded output hashes:

| File | Rechecked SHA-256 |
|---|---|
| `.seryeong-first-media-owned-v1` | `6b3d1b6f3a9e890dd1caa40b0c9bc72b0969a44c2a06e8eaffbd6cdbae345ecf` |
| `pending/right-idle-frame0-preview-diagnostic.png` | `588f3a3a7c2e5e63fdf3a12061b451c934f00a096ffc3af6c89d153d574454d2` |
| `pending/right-idle-frame0-preview-diagnostic-4x.png` | `cb88f3fa79f3730282bdaa23b57a032fc06e2b7cecbc8a8e9d05c2194257d0de` |
| `pending/right-idle-row-diagnostic.png` | `8ae22705cb2a0b16aa611c422e357ae24ea80ba4f68a4b26a3d86426c5f0ed7b` |
| `pending/right-idle-diagnostic.json` | `ff03a909368a1a39a5d1e9eb7dd95b5856158f45427685a3bd58ec4b65eec6ae` |
| `pending/stage-manifest.json` | `972c55f03c6ac15796549061e8904466849efa4db6465c32dda02090251590ff` |
| `manifest/a1-h1-package-v1.json` | `359c15f2c12cb539751d9baf258a2b89a0b2c66ac859d2ce043b2c2aecef0da0` |

- **P2 — Terra evidence has a stale implementation hash.** Evidence records `seryeong_first_media.py` as `e6c105a4ba680798f0cbeaf9185abe72c7a24070941a01f7bced902449aba30f`; the current file hashes to `ba9c7739fb83dc10730778ed8500d935626986d56a9a70f3585e69cc10b691fb`. Test-file, README, staging-output, package-manifest, and source-pin hashes match their recorded/current values. Refresh the evidence digest after the final code correction.
- APV3 `manifest/inventory.json` remains `acceptedCombinationCount=0` (`0/432`). `CostumeCatalogV1.SeryeongInventoryPending()` still constructs all 36 rows as `ProductionPending` with `defaultGrant=false`, preserving runtime `NoAcceptedDefault`. Rights remain `PendingUserAttestation`. This review launched no Unity process and wrote no `Assets/`, catalog, APV3, or production-stage files (`AC-SM1-007/008`).

### Updated AC-SM1 disposition

| Criterion | Re-review disposition | Evidence / remaining gate |
|---|---|---|
| `AC-SM1-001` | **Pending** | No completed 13-frame package or final identity/contact-sheet audit. |
| `AC-SM1-002` | **Partial** | Diagnostic preview geometry validates; final atlas/body review and 640×360 stage review remain absent. |
| `AC-SM1-003` | **Fail-closed staging pass; final AC pending** | Missing left is explicit; no full atlas/clip map is emitted. |
| `AC-SM1-004` | **Pending** | Separately authored left row and native motion/landmark review do not exist. |
| `AC-SM1-005` | **Pending** | Final 32px frame, pivot, and continuity review do not exist. |
| `AC-SM1-006` | **FAIL — P1** | Outer package mutation rejects, but re-hashed inner identity/revision/APV3/clip-map mutations and boolean schema versions pass. |
| `AC-SM1-007` | **Pending** | Rights remain `PendingUserAttestation`. |
| `AC-SM1-008` | **Staging boundary pass** | APV3 remains `0/432`; runtime catalog stays pending with `NoAcceptedDefault`; no Unity/catalog/APV3 mutation occurred during this review. |
| `AC-SM1-009` | **Not run — separate future contract** | Unity import/publication remains outside this offline slice. |
| `AC-SM1-010` | **Partial; completion blocked by P1** | Output boundary and source pins pass; the independent review still has a P1 and Astra disposition remains required. |

### Current Astra acceptance boundary

Astra may accept only the **bounded existence and current pending/non-addressable disposition of the diagnostics**, plus the verified output ownership boundary and unchanged source pins. Astra should **not accept the packer/validator as production-safe** until the inner-manifest semantic P1 is fixed and independently re-probed. The media is not `AcceptedForVerticalDemoMedia`; rights attestation, separately authored left-facing frames, and final visual/package ACs remain open. APV3 stays `0/432`; runtime catalog remains `NoAcceptedDefault`.

## 2026-09-20 final dated addendum

- Reviewer: Luna (independent final re-review of Terra's semantic-validation correction)
- Disposition: **P0=0, P1=1, P2=0.** The previous stale-evidence-hash P2 is closed. Value-level inner semantic and pixel corruption now rejects after hash rebinding. One strict-schema P1 remains: Python equality accepts boolean/float substitutes for integer fields.

### Re-review results

- Direct bundled-Python engine-free suite: **8/8 passed** (`AC-SM1-002/003/006/008/010`). Actual production read-only `validate` returned `PENDING_FAIL_CLOSED` (`AC-SM1-003/006/008/010`).
- Independently exercised **66** inner stage-manifest/diagnostic mutation cases. This covered every expected field, field addition/removal, source/frame/preview/missing-left/action/operation/generated-map values, each of the three diagnostic PNGs' pixels, and changed inner hashes followed by a recomputed package `stageManifestSha256`. The **60 mutations with differing canonical values or pixels** rejected in both validation and rebuild, preserving the stage snapshot byte-for-byte (`AC-SM1-005/006/010`); six JSON numeric type aliases are detailed below and remain accepted.
- **P1 — canonical JSON types are still not exact.** `_assert_exact_object()` and `_assert_package_static()` rely on Python equality, under which `true == 1`, `false == 0`, and `1.0 == 1`. Re-hashed probes accepted inner stage `schemaVersion=true|1.0`, `mediaRevision=true`, and `acceptedAppearanceCombinationCount=false`; diagnostic `schemaVersion=true|1.0` also passed. Outer package probes accepted equivalent boolean/float forms for schema version, media revision, APV3 count, boolean claims, atlas flag, and missing-left marker. Validation returned successfully without changing the stage, while rebuilds of the accepted inner mutations proceeded and rewrote the staging files. These values have the same Python comparison result but are invalid types for the canonical JSON schema. Trace: `REQ-SM1-005/010`, `AC-SM1-006/010`.

  **Minimum correction:** type-check canonical integer fields as integers excluding booleans, require boolean fields to be actual booleans, and add `true`, `false`, and integral-float alias probes for package, stage, and diagnostic schemas. Rejection must continue to preserve stage inventory and bytes.
- Outer ownership protections remain closed: missing and poisoned producer markers reject on production/test build/validate with stage snapshots unchanged; hard-linked manifests reject on build and validate with both stage bytes and the peer unchanged (`AC-SM1-006/010`).
- Rechecked all six source pins, stage files, package manifest, tool, test, and README hashes; each matches both its approved pin or current Terra evidence entry. The earlier module-hash discrepancy is resolved. APV3 remains `0/432`, the runtime catalog remains all-pending with `NoAcceptedDefault`, and rights remain `PendingUserAttestation` (`AC-SM1-007/008`). No Unity process was launched and no Unity/catalog/APV3/production file was changed during this review.

### Final AC-SM1 disposition

| Criterion | Final re-review disposition | Evidence / remaining gate |
|---|---|---|
| `AC-SM1-001` | **Pending** | Final 13-frame media and identity/contact-sheet review do not exist. |
| `AC-SM1-002` | **Partial** | Diagnostic preview and pixels match the pinned source; completed atlas, manual body review, and 640×360 stage review remain absent. |
| `AC-SM1-003` | **Fail-closed staging pass; final AC pending** | Missing left remains explicit; no complete atlas/clip map is emitted. |
| `AC-SM1-004` | **Pending** | Separately authored left frames and native continuity review remain absent. |
| `AC-SM1-005` | **Pending** | Final 32px frame, pivot, and continuity review remain absent. |
| `AC-SM1-006` | **FAIL — P1** | Rebound value/pixel mutations reject, but JSON bool/float type aliases pass. |
| `AC-SM1-007` | **Pending** | Rights attestation is unresolved. |
| `AC-SM1-008` | **Staging boundary pass** | APV3 `0/432`; runtime catalog pending/`NoAcceptedDefault`; no Unity/catalog/APV3 mutation. |
| `AC-SM1-009` | **Not run — separate future contract** | Unity import/publication remains outside this offline slice. |
| `AC-SM1-010` | **Partial; completion blocked by P1** | Path/ownership and source-pin checks pass; strict typed-schema validation and Astra disposition remain. |

### Current Astra acceptance boundary

Astra may accept the **existence, deterministic source-derived pixels, ownership protections, and pending/non-addressable disposition of the current offline diagnostics**. The packer/validator does not yet meet full `AC-SM1-006` because numeric JSON types are not strict. No media is `AcceptedForVerticalDemoMedia`; rights, left-facing production frames, final visual ACs, and the later Unity publication gate remain outstanding.

## 2026-09-20 final close-out addendum

- Reviewer: Luna (independent close-out after strict JSON type correction)
- Disposition: **P0=0, P1=0, P2=0.** The previous bool/int/float type-smuggling P1 is closed. The implementation tool and validator are acceptable to Astra for the contract's bounded offline, fail-closed pending-tooling scope; this does not accept the media package.

### Direct close-out evidence

- Direct engine-free suite: **9/9 passed** (`AC-SM1-002/003/006/008/010`). Actual production read-only validation returned `PENDING_FAIL_CLOSED` (`AC-SM1-003/006/008/010`).
- Independently exercised **28 bool/int/float type-smuggling variants** across outer package, stage, and diagnostic JSON. For inner mutations, recalculated both the stage document's dependent generated hash where applicable and the outer `stageManifestSha256`. Every variant was rejected by both `validate_for_test()` and `build_for_test()`; all stage inventories and bytes stayed unchanged (`AC-SM1-005/006/010`).
- Rechecked semantic and pixel regressions: a re-hashed stage revision change, diagnostic operation mutation, and preview pixel mutation all reject validation; semantic rebuild rejects before cleanup. The rebuilt package hashes do not bypass deterministic pixel comparison (`AC-SM1-005/006`).
- Rechecked ownership regressions: missing and poisoned outer producer markers reject both validation and rebuild with stage snapshots unchanged; hard-linked manifests reject both operations with stage bytes and the linked peer unchanged (`AC-SM1-006/010`).
- Recalculated all six source pins and staging-output/package hashes; all match the Approved contract or Terra evidence. The updated implementation evidence digests also match current files: module `3d258104a60919357021d42673f446d7ac447d2b4322dfc302890417f103a04f`, tests `ad66ab591e2ea82e1960330a356e63da54a182aa64ca1eacbc146b9509289bbd`, README `a63132b8eaccd7b1fe0a2bf2ec5efaa2c7645e72e087cffd9084571e7e2d46cc` (`AC-SM1-006/010`).
- APV3 remains `0/432`; runtime catalog remains all `ProductionPending` with `NoAcceptedDefault`; rights remain `PendingUserAttestation`. No Unity process was launched, and this review did not modify Unity, catalog, APV3, or production staging files (`AC-SM1-007/008`).

### Final AC-SM1 disposition

| Criterion | Close-out disposition | Evidence / remaining gate |
|---|---|---|
| `AC-SM1-001` | **Pending** | The final 13-frame media and identity/contact-sheet audit do not exist. |
| `AC-SM1-002` | **Partial** | Deterministic diagnostic geometry/pixels validate; complete gameplay atlas and 640×360 stage review remain absent. |
| `AC-SM1-003` | **Fail-closed staging pass; final AC pending** | The missing left row remains explicit; no complete two-facing atlas or clip map is emitted. |
| `AC-SM1-004` | **Pending** | Six separately authored and reviewed left-facing frames remain outstanding. |
| `AC-SM1-005` | **Pending** | Manual 32px frame, pivot, landmark, and continuity review remains outstanding. |
| `AC-SM1-006` | **PASS — offline tooling integrity** | Deterministic outputs, strict typed semantics, hash rebinding rejection, and no-mutation failure behavior passed. |
| `AC-SM1-007` | **Pending** | User rights attestation remains required. |
| `AC-SM1-008` | **PASS — offline staging boundary** | APV3 is `0/432`; runtime catalog is pending/`NoAcceptedDefault`; Unity/catalog/APV3 are unchanged. |
| `AC-SM1-009` | **Not run — separate future contract** | Unity import/publication is a later gate after CUA/CIO/M5D7Q0 verification and the default/wearability decision. |
| `AC-SM1-010` | **Tooling review pass; final project disposition pending** | Luna reports no P0/P1/P2 on the offline tool; Astra still records integration disposition. |

### Astra acceptance boundary

For the reviewed 2026-09-20 bytes, Astra could accept `tools/seryeong_first_media/**` and its hash-pinned outputs as **bounded offline pending tooling** under the Approved contract. The media was then non-wearable and non-addressable, not `AcceptedForVerticalDemoMedia`. The 2026-09-21 status note below supersedes only the current rights/default state; it does not alter this review's evidence.

## 2026-09-21 status note — not an independent re-review

This historical Luna review predates the project owner's later
`ProjectUseAuthorized` clarification and ADR-0034. Its findings remain factual
for the reviewed 2026-09-20 bytes, but they are not the current rights/default
state. The current Terra evidence records the replacement pending disposition,
the rights-chain hash, A1's future-default decision, and quarantined candidate
hashes. Luna must independently re-review that exact changed scope before any
AC-SM1 status beyond the tooling's prior bounded acceptance is claimed.

## 2026-09-21 independent post-review — A1 default and rights synchronization

- Reviewer: Luna
- Scope: the bounded A1/H1 rights/default synchronization, staging manifests,
  quarantined image candidates, offline tool and tests, and current index status.
- Disposition: **P0=0, P1=0, P2=0** for the reviewed synchronization and
  fail-closed tooling. This does not accept the media package or authorize
  Unity publication.

### Findings and evidence

- `ProjectUseAuthorized` is consistently recorded for the A1/H1 project-use
  scope in the rights chain, package manifest, stage manifest, validator, and
  README. The evidence cites the owner's clarification that these images were
  generated for this project and authorizes their same-project use and
  modification. It expressly says no separate Unity permission is required
  or represented; no Unity-specific rights requirement is claimed.
- The amended rights chain now includes explicit creator/provider,
  model/tool/version, generation/acquisition dates, terms evidence,
  project-use and modification permission, attribution, and restriction
  fields. The model/version is transparently `historical-unavailable`; the
  preserved prompts, references, paths, dates, and hashes satisfy the
  contract's exception without guessing. The recorded authorization remains
  project-scoped and makes no claim that Unity requires separate permission.
- ADR-0034 and both manifests record `seryeong.costume.a1.v1` as the resolved
  future initial default. The manifests still state
  `runtimeCatalogDefaultGrant=false`, `NoAcceptedDefault`, APV3 accepted count
  `0`, `nonWearable=true`, and `nonAddressable=true`. The current A1 choice is
  not represented as current wearability or catalog acceptance.
- The two quarantined image candidates match their pinned SHA-256 values:
  v1 `2b0292ef464f8eb76d3b4a42ebb73e4b57db955bd4e41387d8adeaf1503bde40` and
  v2 `24565d1656344fa7b3de6cd9de3b2570287b98caa58e30fbc0314626a136e505`.
  Both are `2172×724` 24-bit RGB images, so neither has alpha or the required
  `384×128` gameplay-row geometry. The tool hashes and rejects drift in the
  quarantine inventory; neither candidate is used as package pixels.
- The amended rights-chain SHA-256 is
  `d079325198338e9d171750336add34c69f7130fa9bd81747d82bcfd5ee31236b`; the
  stage-manifest SHA-256 is
  `d515309eb0d04d1f07c02e81c1080e6d9dfe4b792736eca17b4343cb2017410d`; the
  package-manifest SHA-256 is
  `b2ff1f9139a4450e1d77e633103fb35ebc35a6b5a73dc4a14b22bd26a4f7d0c8`; and
  the validator/tool SHA-256 is
  `0affae2d2dfbb533b934643575d4f305f79e8a07d42f70f030fc61486162cbfc`.
  All match Terra's supplied pins. The package still has zero left-facing
  frames, no complete atlas, and no clip map.
- Engine-free suite: **10/10 passed**. Read-only production validation
  returned `PENDING_FAIL_CLOSED`. No build or staging write was run during
  this verification.
- The staging tool exposes no Unity, catalog, importer, addressable, or APV3
  write path. Its production targets remain limited to the approved offline
  stage and manifest. APV3 remains `0/432`; runtime catalog code continues to
  describe all Seryeong costumes as `ProductionPending` with no accepted
  default. No media acceptance or Unity publication is claimed by this review.

### Current AC-SM1 disposition for this bounded review

| Criterion | Disposition | Evidence / remaining gate |
|---|---|---|
| `AC-SM1-006` | **PASS — offline tooling integrity** | 10/10 engine-free tests; production validator fails closed; hashes and strict pending semantics match. |
| `AC-SM1-007` | **PASS — project-use authorization recorded** | Rights chain covers the pinned A1/H1 sources and evidence hashes; the amended fields and explicit project authorization satisfy the rights-evidence gate. |
| `AC-SM1-008` | **PASS — offline staging boundary** | APV3 `0/432`, runtime catalog `NoAcceptedDefault`, package remains non-wearable/non-addressable, and the staging tool has no Unity/catalog/APV3 publication path. |

All other package acceptance criteria retain their prior pending or partial
dispositions. Six separately authored left-facing frames, manual 32px
correction and review, complete package evidence, and Astra disposition remain
open. The historical 2026-09-20 findings above remain as history; this section
is the current independent review of the 2026-09-21 rights/default state.
