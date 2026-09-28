# Costume CUA R4-B1 validation-row narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 21 definition/package/binding mutation rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `FCD28E6E9B1BF99F196BC9D0A25A5A598C4A82B1B8F64C923355200D2ADF5CE4`
- Terra evidence reviewed: `docs/verification/2026-09-20-costume-cua-implementation-evidence.md`, SHA-256 `D60EA0014884E4036C9E93B6D5BC01A7D3ACEEEF6651649E566F0F71F9C87F11`

## Row enumeration and construction

`NonJsonValidationMutations` contains exactly 21 rows (`FCD28...:17–18`):

- definition identity/status: actor, costume, portrait set, gameplay set, pending, rejected;
- package state: synthetic flag, catalog revision, presentation revision; and
- binding: actor, costume, portrait set, atlas set, catalog revision, presentation revision, portrait hash, atlas hash, clip-map hash, missing action, extra action, empty action set.

Each row is constructed/mutated independently in `AC_CUA_002_ConstructibleNonJsonMutationRejectsWithoutTouchingPublishedPair` (`:19–37`). Definition rows build a distinct `CostumeDefinitionV1`; package rows mutate the package backing fields; binding rows build a distinct `CostumePresentationBindingV1`. Each row calls `media.Validate(definition, binding)` and asserts a non-`None` rejection.

## Closure gap

The row construction and direct package-level rejection are real, but the required adapter preservation boundary is not fully exercised:

- No row calls `adapter.TrySelect` with a mutated package/binding candidate. The only operation after mutation is direct `media.Validate` (`:34`), so the test does not prove the adapter's pre-save selection rejection path, zero-save behavior, or zero-projection behavior for these candidates.
- The old tuple reference, `MoveCalls`, projection count, `Ready` status, and current ID are asserted (`:35`), but no transient highlight is established before the mutation and no `TransientPreviewId`/highlight value is asserted afterward. Thus highlight preservation is not proven.
- The status/current assertions are only post-validation snapshots; they do not establish unchanged full view state or old portrait/gameplay pair under an attempted adapter selection.

The true first-publication control is created by `PublishedAdapter`, but it does not repair the missing mutated-candidate `TrySelect` boundary or highlight assertion for these 21 rows.

## Decision

**OPEN — P1 remains for R4-B1.** The 21 rows are enumerated and genuinely constructible, and direct `Validate` rejection is demonstrated. The row requirement is not closed because the requested adapter-level rejection/no-save/no-projection and highlight-preservation assertions are absent. Add a valid pre-highlight, route each applicable mutated candidate through `TrySelect` (or document an explicit definition-only validation boundary with equivalent adapter proof), assert complete old view/tuple/status/highlight invariants, then request a fresh narrow Luna review. Other R4-B categories remain intentionally unreviewed in this document.

