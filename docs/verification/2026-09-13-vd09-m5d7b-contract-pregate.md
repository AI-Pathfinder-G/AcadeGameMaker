# VD-09 M5D7B canonical decoder — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7B profile v1 canonical decoder](../specs/work-contracts/2026-09-13-vd09-m5d7b-profile-v1-canonical-decoder.md)
- Compared against: Approved VD-09, SYSTEM-CONTRACTS, ADR-0031, and Verified M5D3–M5D7A APIs/contracts
- Verdict: **Second pre-gate PASS — P0=0, P1=0; Astra may proceed to approval after implementation-gate clarifications**
- Scope: contract/design review only. No contract or implementation was modified and no Unity run was performed.

## Second pre-gate re-review

The revised contract closes P1-001. It now validates the outer envelope using the raw `bindingOverridesJson` string before invoking M5D5. An exact outer canonical/hash match followed by malformed or noncanonical inner binding text returns `ValidBindingRecoveryRequired` with an immutable `ProfileInputRecoveryProjectionV1` containing only settings, tutorial, progression, and source revision; no document is exposed. Metadata mismatch remains a separate `ValidInputMetadataRecoveryRequired` document classification, and combined binding/metadata failures carry the corresponding reason bits. A canonical inner binding continues through M5D6 and M5D7A to produce a document.

The revised API declares exact property types and availability, explicitly makes `InputRecoveryReason` a closed flags domain, fixes failure precedence, classifies representable negative outer schema versions as unsupported, makes duplicate schema invalid before unsupported classification, and defines root-depth 64 accepted / 65 `MalformedJson` behavior. The exception containment boundary now distinguishes data exceptions from fatal runtime exceptions. AC-M5D7B-002, -004, -005, -006, -007, and -009 cover the repaired recovery, precedence, depth, strict UTF-8, getter, and containment matrix.

## P1 finding

### P1-001 — RESOLVED: malformed/noncanonical inner override recovery projection

VD-09 says a non-empty `input.bindingOverridesJson` is parsed and canonicalized before being stored, and explicitly classifies parse/canonicalization failure as a binding partial-recovery target. VD-09 AC-PLAT-009 correspondingly requires settings, tutorial, and progression to survive while only the input block is replaced.

The first draft mapped every M5D5 constructor/canonicalization rejection to `Invalid/SchemaViolation`. The revised step 5 first validates an outer reconstruction that preserves the raw binding string, and revised step 6 routes only the inner failure to a typed recovery projection. The public result now exposes no `Document` for that classification while preserving validated non-input state and source revision.

The revised cases are now implementable:

1. A structurally valid outer profile whose override string is noncanonical, for example `{"b":2,"a":1}`, is first checked as raw outer text and then returns `ValidBindingRecoveryRequired` with reason `BindingOverridesMalformedOrNonCanonical` and no document.
2. A structurally valid outer profile whose override string is malformed, for example `{bad}`, follows the same binding-recovery path after exact outer hash validation; settings/tutorial/progression/source revision remain available through the projection.

This is now distinct from `ValidInputMetadataRecoveryRequired`: metadata mismatch can return an exact document, while malformed/noncanonical inner binding cannot. Neither recovery result implies normal save/current compatibility or actual Input System application.

## Current P0/P1 findings

None found. The unsupported-outer-schema rule, no-migration/no-IO boundary, strict exact-byte validation, and authenticity disclaimer are consistent with VD-09 and SYSTEM-CONTRACTS. P1-001 is resolved by the revised typed projection and precedence.

## Remaining P2 findings and required test clarifications

- Add explicit cases for an inner decomposed/unpaired text value: outer NFC/surrogate rejection must retain the documented first-step precedence, while safely representable malformed/noncanonical inner JSON reaches binding recovery. This prevents ambiguity between outer-string invalidity and inner-JCS recovery.
- Keep failure precedence executable in tests for unsupported schema with malformed ignored fields, duplicate escaped `schemaVersion`, integrity shape errors, negative outer versions, and the distinction between `SchemaViolation` and `CanonicalOrIntegrityMismatch`.
- Test the closed `ProfileInputRecoveryReason` flags domain and reflection-bypassed result/projection values with unknown bits, plus every getter outside its availability state. Verify all listed parser/crypto data exceptions become typed results while only a null argument throws; preserve the specified root-depth 64/65 and strict UTF-8 no-replacement cases.

## Positive pre-gate checks

- M5D7A exact reconstruction is the correct integrity strategy: parse a v1 candidate, rebuild through the Verified encoder, compare payload-only lowercase SHA-256 and then full final bytes, and reject alternate escaping, whitespace, reorder, or hash-only repairs.
- M5D6 asset/schema mismatch handling is correctly separated from outer schema support. An exact canonical mismatch document can be returned as `ValidInputRecoveryRequired` with nonzero flags while preserving settings/tutorial/progression and without encoding derived compatibility metadata.
- The proposed no-IO/path/selection/quarantine/revision-mutation/input-apply boundary is honest and consistent with SYSTEM-CONTRACTS. The allowlist is implementable as one runtime decoder file, one EditMode test file, matching metas, and the listed evidence/index documents only.

## AC design status

AC-M5D7B-001..009 now describe a viable exact parser/validator, typed binding-recovery projection, metadata mismatch classification, and strict failure matrix. The remaining P2 cases are implementation-gate precision, not contract blockers.

## Recommendation

Recommend Astra proceed toward approval: P0/P1 are now zero. Keep the remaining P2 edge cases as mandatory implementation tests and preserve the split between exact document recovery, binding-recovery projection, and later IO/atomic save/input-apply ownership.
