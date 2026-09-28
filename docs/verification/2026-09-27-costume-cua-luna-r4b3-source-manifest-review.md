# Costume CUA R4-B3 source-manifest mutation narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 28 source-manifest canonical-byte mutation rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `2D82B0F2415F61B3B634699152DD19A8E43116C7D732D0937F000C3414A0333A`

## Row enumeration and mutation integrity

`R4B3SourceManifestByteMutations` contains exactly 28 rows (`2D82...:24–31`): UTF-8 BOM/whitespace; property reorder/unknown/duplicate; identity, revision and hash fields; required action/facing list missing/extra/unsorted/duplicate variants; and schema version.

`MutateSourceManifestBytes` performs exact byte/string changes for every named row (`:644–680`). Each replacement is guarded by `ReplaceExactOnce`/`ReplaceQuotedFieldValue`, which throws if the canonical sequence or field is absent (`:728–743`), and the test asserts the candidate manifest hash differs from the canonical manifest hash (`:242–247`). These are therefore real mutations rather than no-op labels.

## Exact validation and adapter boundary

The candidate retains canonical portrait, atlas and clip-map payload hashes and the binding is bound to those canonical hashes (`:245–252`). Each row then asserts `Validate == CostumeUnityPackageRejectionV1.Manifest`, replaces the catalog definition, calls real `adapter.TrySelect(candidate) == Package`, and applies the B1a full preservation oracle for old tuple/media/binding, all view/highlight/current/status fields, and move/replace/projection counts (`:253–260`, helper `:780–789`). This proves the mutation reaches source-manifest canonical-byte equality rather than failing early on a payload hash.

## Canonical positive control

`AC_CUA_002_R4_B3_CanonicalSourceManifestControlValidatesAndPublishesReplacement` uses unchanged canonical manifest bytes, asserts `Validate == None`, calls actual replacement through `TrySelect`, and verifies a new published tuple, candidate media identity, expected current ID, two moves, one replace, and two projections (`:264–280`).

## Decision

**CLOSED for R4-B3.** All 28 required source-manifest mutations are no-op-guarded and hash-proven, reach exact `Manifest`, pass through adapter rejection with B1a preservation, and have a canonical positive replacement control. This narrow result does not close other R4-B categories or authorize Unity execution by itself.

