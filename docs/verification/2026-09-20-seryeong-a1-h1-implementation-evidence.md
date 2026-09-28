# Seryeong A1/H1 first-media offline staging — implementation evidence

- Date: 2026-09-20
- Contract: `docs/specs/work-contracts/2026-09-20-seryeong-a1-h1-first-media-package.md`
- Implementer: Terra
- Scope: approved offline tooling and non-wearable staging only
- Current implementation disposition: `PendingMissingLeftManual32pxAndLunaReview` — **not** `AcceptedForVerticalDemoMedia`

## Scope and dirty-worktree boundary

The workspace was already dirty at implementation start (`368` porcelain entries).
The contract's three generated target paths did not exist at the scoped baseline:
`tools/seryeong_first_media/`,
`images/sprites/seryeong-appearance-v3/production/a1-h1-v1/`, and
`images/sprites/seryeong-appearance-v3/manifest/a1-h1-package-v1.json`.
This implementation changed only those targets plus this evidence and the
minimum `docs/README.md` index link. No `Assets/`, `.meta`, Unity catalog,
scene, prefab, runtime, importer, addressable, or APV3 inventory file was
changed by this unit.

## P0 path-boundary correction

The initial offline tool exposed configurable output arguments and used a broad
directory cleanup. Before integration, that surface was removed.

- The public production `build()` / `validate()` APIs and CLI accept no output
  path. They permit only the exact Approved production stage and manifest.
- `build_for_test()` / `validate_for_test()` are explicitly test-only and
  permit only `stage` and `manifest.json` beneath a fresh direct child of the
  system temp root named `seryeong-first-media-test-*`.
- The stage has a deterministic ownership marker
  `.seryeong-first-media-owned-v1` (SHA-256
  `6b3d1b6f3a9e890dd1caa40b0c9bc72b0969a44c2a06e8eaffbd6cdbae345ecf`).
  Rebuilds inspect this marker and the exact known stage inventory, then unlink
  only named regular generated files. There is no recursive deletion call.
- Reparse/symlink components, ROOT or pinned-source ancestors, unknown stage
  entries, unknown pending payloads, and unowned manifests reject before a
  write. The package manifest now carries the same producer marker.
- **Luna P1 follow-up:** production and test manifest preflight now both
  require the marker, the exact canonical schema, and a regular single-link
  file. Stage inventory, destination parents, manifest ownership/link count,
  and canonical package fields are all checked before `_prepare_stage()` can
  unlink a generated file. A failed preflight therefore leaves the complete
  stage file list and bytes unchanged.
- The validator now fixes and checks package identity (`A1/H1`, portrait and
  gameplay set IDs, revision `1`, exact production stage root), non-claim,
  pending rights, APV3 `0`, `NoAcceptedDefault`/`defaultGrant=false`, and the
  incomplete atlas/clip-map fields in addition to source and stage hashes.
- **Luna re-review follow-up:** `pending/stage-manifest.json` and
  `pending/right-idle-diagnostic.json` are now exact-semantic documents, not
  merely hash containers. The preflight reconstructs the expected diagnostic
  pixels from the pinned v4 source, validates its action/source/preview/missing
  left/not-produced/operation/generated-hash meaning, then validates every
  stage identity, requirement ID, revision, rights, APV3/default, payload,
  blocker, and generated-file field. This happens before a rebuild can unlink
  an owned payload; recomputing the outer package hash cannot bypass it.
- **Luna final type-safety follow-up:** all package, stage, and diagnostic
  comparisons now recurse through exact JSON types. `true` is not `1`, `false`
  is not `0`, and `1.0` is not integer `1`; objects, arrays, strings and null
  must also have the exact expected JSON type and shape.

| Boundary implementation file | SHA-256 |
|---|---|
| `tools/seryeong_first_media/seryeong_first_media.py` | `3d258104a60919357021d42673f446d7ac447d2b4322dfc302890417f103a04f` |
| `tools/seryeong_first_media/test_seryeong_first_media.py` | `ad66ab591e2ea82e1960330a356e63da54a182aa64ca1eacbc146b9509289bbd` |
| `tools/seryeong_first_media/README.md` | `a63132b8eaccd7b1fe0a2bf2ec5efaa2c7645e72e087cffd9084571e7e2d46cc` |

## REQ-SM1 implementation record

- **REQ-SM1-001/006/008/010:** `tools/seryeong_first_media/seryeong_first_media.py`
  fixes all six source roles and SHA-256 pins; it rejects any hash or A1/H1
  provenance drift before staging. It uses raw frame `(row=0,column=0)` only
  as the exact preview identity reference.
- **REQ-SM1-002/004/007:** the tool reproduces a `128×128` transparent,
  64px-source **diagnostic** preview and nearest-neighbor `4x` diagnostic from
  the existing `source_only` v4 sheet. The preview records the required
  baseline/pivot `(64,104)` / `y=104`; alpha bounds are recorded for later
  manual review. It makes no new matte, automatic 32px downscale, or runtime
  setting change.
- **REQ-SM1-003:** the deterministic right-only row audit reports six source
  frames. `idle.left` is recorded as
  `FailClosedMissingProductionInput` with zero available frames. No mirror,
  duplicate, fabricated frame, full atlas, or clip map is emitted.
- **REQ-SM1-005/009:** both manifests bind the generated diagnostic hashes and
  all source hashes while retaining `nonWearable=true`, `nonAddressable=true`,
  APV3 `0`, `NoAcceptedDefault`, and `PendingUserAttestation`.

## Pinned source immutability check

All initial/final source hashes matched the Approved contract:

| Role | SHA-256 |
|---|---|
| raw movement | `a2939b925999aaf9634513862456e75912cb40fb4d638b2fe6584d28d3448dc6` |
| raw provenance | `a68fce849cd740aaaed246de61c48ad024a780a65134e2b75eb765b1ea824766` |
| A1 reference | `951310f8cb41f126e192ea6de74ab8b297a5f95d18be158d8d98503775da5cc1` |
| H1 reference | `da047fa24b06cfcb7eb6c3516a3e6b17145bf3cb954be416c8e2685f8b96b60d` |
| reviewed calibration | `414aaac2d134ff86c2ccfa645517ce606722a3b6f13ccfaac49cbb32b60735f9` |
| source-only v4 diagnostic | `2564dd72375b2a4e9c4980cb9ac5d36cedd52db3df6509c6e338e43ed9a63b41` |

## Generated pending staging hashes

| File | SHA-256 |
|---|---|
| `production/a1-h1-v1/pending/right-idle-frame0-preview-diagnostic.png` | `588f3a3a7c2e5e63fdf3a12061b451c934f00a096ffc3af6c89d153d574454d2` |
| `production/a1-h1-v1/pending/right-idle-frame0-preview-diagnostic-4x.png` | `cb88f3fa79f3730282bdaa23b57a032fc06e2b7cecbc8a8e9d05c2194257d0de` |
| `production/a1-h1-v1/pending/right-idle-row-diagnostic.png` | `8ae22705cb2a0b16aa611c422e357ae24ea80ba4f68a4b26a3d86426c5f0ed7b` |
| `production/a1-h1-v1/pending/right-idle-diagnostic.json` | `ff03a909368a1a39a5d1e9eb7dd95b5856158f45427685a3bd58ec4b65eec6ae` |
| `production/a1-h1-v1/pending/stage-manifest.json` | `1e87cb7a21d3c27a75a0749f1c4ea4443428039629df336dde3c3e7267b6a019` |
| `manifest/a1-h1-package-v1.json` | `f2beab184ee6dd23ca4243fe68aa597cbe557d81403cedc22033428cf825c3e8` |

## Engine-free verification

Executed with the bundled Python runtime; Unity was not launched.

1. In-memory `compile()` for the packer and tests — passed without creating a
   workspace `__pycache__` output.
2. `unittest tools/seryeong_first_media/test_seryeong_first_media.py` — **9/9 passed**.
   - **AC-SM1-006:** two clean temporary builds had identical hashes and all
     pinned source hashes matched before/after.
   - **AC-SM1-003/008:** pending schema carries `left=0`, `right=6`, no atlas
     payload, and an explicit missing-left fail-closed state.
   - **AC-SM1-002:** preview is verified as transparent `RGBA 128×128`.
   - **AC-SM1-006:** altered complete-atlas flag, unsupported schema version,
     and corrupted preview bytes all reject.
   - **AC-SM1-006/010:** production-path substitution, system-temp-root use,
     manifest escape, unknown stage payload, and mocked reparse/symlink
     traversal all reject without deleting the unknown payload.
   - **AC-SM1-005/006/010:** producer-marker corruption and a real NTFS
     hard-linked manifest reject before stage cleanup; peer bytes and the full
     stage snapshot remain unchanged. Media revision, portrait set, stage
     root, rights, APV3/default, and non-claim mutations all reject even after
     JSON bytes are rewritten.
   - **AC-SM1-005/006:** stage `schema`/REQ IDs/A1-H1 sets/revision/APV3/
     default/payload/blockers and diagnostic source/frame/preview/missing-left/
     operations/generated-map mutations all reject after the stage and package
     hashes are deliberately rebound. The adversarial stage snapshot remains
     byte-identical after failed validation and failed rebuild.
   - **AC-SM1-005/006:** package, stage and diagnostic `bool`/`int`/`float`
     alias attempts reject after all applicable hashes are rebound; the stage
     snapshot remains unchanged after failed validation and rebuild.
3. Default production build and validation — passed with the expected message:
   `PENDING_FAIL_CLOSED`.

## Acceptance status and remaining gates

This is implementation evidence only; Terra does not accept its own work.

- **AC-SM1-001/002/005:** partially prepared by source/geometry diagnostics;
  human native, 4x, animation, overlay, and 640×360 review remains for a
  complete package.
- **AC-SM1-003/004:** intentionally unsatisfied until six separately authored
  and manually corrected `idle.left` frames exist.
- **AC-SM1-007:** intentionally unsatisfied while the rights chain remains
  `PendingUserAttestation`.
- **AC-SM1-009/010:** later Unity import/publication is out of scope; Luna
  independent review and Astra's final disposition remain required.

The pending files must remain unreachable and non-wearable until all of those
gates are closed.

## 2026-09-21 A1 default and rights-state synchronization

This bounded update implements the user-approved state records in ADR-0034 and
the project-use authorization evidence. It is not a media-acceptance decision
and does not replace Luna's independent review.

- **REQ-SM1-005/006/009/010:** The fixed offline tool now requires the exact
  rights-chain evidence document and SHA-256
  `d079325198338e9d171750336add34c69f7130fa9bd81747d82bcfd5ee31236b`.
  Both stage and package manifests record `rightsDisposition=ProjectUseAuthorized`.
  The obsolete user-attestation blocker is removed; no third-party material is
  newly introduced.
- **REQ-SM1-005/009:** Both manifests record
  `resolvedInitialDefaultCostumeId=seryeong.costume.a1.v1`, while retaining
  `runtimeCatalogDefaultGrant=false`, `runtimeCatalogDisposition=NoAcceptedDefault`,
  `acceptedAppearanceCombinationCount=0`, `nonWearable=true`, and
  `nonAddressable=true`. This records a future Unity-publication choice only.
- **REQ-SM1-006/008/010:** The two failed image-generation attempts are retained
  only in the existing quarantine and are hash-pinned by the validator:

  | Candidate | SHA-256 | Rejection record |
  |---|---|---|
  | `quarantine/left-idle-generated-v1.png` | `2b0292ef464f8eb76d3b4a42ebb73e4b57db955bd4e41387d8adeaf1503bde40` | Baked checkerboard; `2172×724` 24bpp RGB with no alpha; not the required `384×128` 6×2/64px production geometry and lacks an auditable six-frame facing timeline. |
  | `quarantine/left-idle-generated-v2.png` | `24565d1656344fa7b3de6cd9de3b2570287b98caa58e30fbc0314626a136e505` | Baked checkerboard; `2172×724` 24bpp RGB with no alpha; wrong production geometry/scale and weak, non-contractual frame distinction rather than an ordered, separately authored `idle.left` row. |

  Neither file contributes pixels, provenance credit, acceptance credit, or a
  runtime asset. The validator rejects unexpected, missing, linked, or
  hash-drifted quarantined candidates. No raster pixels were edited.

### 2026-09-21 deterministic verification

- In-memory Python compilation passed.
- Engine-free suite: **10/10 passed** (`AC-SM1-002/003/006/008/010`), including
  the new quarantined-candidate hash-drift/no-mutation case.
- Production `validate → build → validate` each returned
  `PENDING_FAIL_CLOSED`. The three generated diagnostic PNG SHA-256 values were
  byte-identical before and after the state rebuild.
- The current stage-manifest SHA-256 is
  `d515309eb0d04d1f07c02e81c1080e6d9dfe4b792736eca17b4343cb2017410d`;
  the package-manifest SHA-256 is
  `b2ff1f9139a4450e1d77e633103fb35ebc35a6b5a73dc4a14b22bd26a4f7d0c8`.
- Boundary implementation hashes: module
  `0affae2d2dfbb533b934643575d4f305f79e8a07d42f70f030fc61486162cbfc`,
  tests `0b4c8e8f388ebe90ad19c5017e4f0454fe9282927ad0997f978d4ec4e805e29f`, README
  `2e0feafa3700a1462471abbda0b14e1f387c325162649b5c71758ff819e5a1c1`.

The prior text that says rights were pending is historical 2026-09-20 review
context. The current unresolved gates are only separately authored left-facing
frames, manual 32px correction/review, Luna's independent post-change review,
and Astra disposition; Unity publication remains a separate later contract.
