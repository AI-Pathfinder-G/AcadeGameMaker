# Costume CUA R4-B2 clip-map mutation narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 34 canonical clip-map byte mutation rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `C07F3491CA2A215A9F50CF95D00EC844FCD841BBAA1EF3B6A0196F74B011EC78`

## Row enumeration

`R4B2ClipMapByteMutations` contains exactly 34 named rows (`C07F...:23–29`): UTF-8 BOM/leading/trailing/interior whitespace; schema/actor-costume/PPU-cell property reorder; unknown/duplicate property; schema, identity, PPU and geometry-profile values; unsorted/duplicate/missing/unknown clip entries; and frame order/field mutations.

`MutateClipMapBytes` is not a no-op helper. Each branch either adds/removes/reorders bytes or performs a checked exact substring replacement, and missing canonical substrings throw (`:590–631`, `:636–640`). The two removal branches remove actual objects from the canonical `clips` array; frame/clip substitutions target existing canonical sequences.

## Hash rebinding and validation boundary

Each row constructs a candidate whose clip-map bytes are mutated, then recomputes both the definition and binding `ClipMapHash` from those mutated bytes (`:193–203`, `:583–587`). The test explicitly asserts:

- mutated clip-map hash differs from the original;
- candidate hash equals definition hash;
- binding hash equals definition hash; and
- `Validate` returns exact `CostumeUnityPackageRejectionV1.Manifest`.

Thus the rows cannot pass merely by an early hash mismatch; they reach canonical manifest/byte equality validation. Every row then replaces the catalog definition, invokes real `adapter.TrySelect(candidate)`, expects adapter `Package` rejection, and applies the B1a full old tuple/media/binding, view/highlight/current/status and save/replace/projection preservation oracle (`:204–212`, `:677–686`).

## Canonical positive control

`AC_CUA_002_R4_B2_CanonicalClipMapControlValidatesAndPublishesReplacement` supplies the unchanged canonical bytes, asserts `Validate == None`, and performs an actual replacement through `TrySelect` (`:215–231`). It confirms a new published tuple, candidate media identity, expected state current ID, `MoveCalls == 2`, `ReplaceCalls == 1`, projection count `2`, and `Ready` status.

## Decision

**CLOSED for R4-B2.** All 34 mutations are syntactically real, hash-rebound to reach `Manifest`, exactly categorized, routed through adapter rejection with the B1a preservation oracle, and paired with a canonical positive replacement. This narrow result does not close other R4-B categories or authorize Unity execution by itself.

