# Costume CUA R4-B1a corrected validation-row narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow review of the 21 definition/package/binding mutation rows only; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `BD510339DDBBE00C86838648AE4571B00BAB8AE780B0D8632450AFEAFFD5CA39`
- Prior R4-B1 evidence SHA-256: `4B6EAE9ECE55F7D811BB3282010BCE0A9E4F5C6BDBD36BB86C541B7FC3A457E8`

## What is now closed

The same 21 rows remain explicitly enumerated (`BD510...:17–18`). The corrected test starts with a valid first publication, selects the second synthetic package/definition, mutates one requested definition/package/binding field, replaces the corresponding catalog definition using a fresh fixture, calls direct `media.Validate`, and then calls the real `adapter.TrySelect(media)` (`:20–37`). This closes the prior gap where only `Validate` was called.

The row covers definition actor/costume/portrait-set/gameplay-set/status, synthetic flag, package catalog/presentation revisions, and binding actor/costume/sets/revisions/hashes/action vocabulary. The adapter-level attempt is required to return non-`None`, and the test asserts no additional move/save, no additional projection, `Ready` status, unchanged current ID, and the same `Published` tuple reference (`:34–37`).

## Remaining gap in the requested proof

The requested “full views/highlight/current/status, old tuple+media+binding refs” proof is not complete:

- No pre-mutation `Highlight` call establishes a non-empty transient preview, and no row asserts `TransientPreviewId`/highlight preservation after the rejected `TrySelect`.
- Only `SnapshotView().CurrentId` is compared. State revision, actor/current view fields, availability, row contents and the complete before/after view are not compared.
- `Published` is asserted `SameAs(old)`, but `old.Media` and `old.Binding` are not independently asserted `SameAs` their pre-mutation references. The test mutates the candidate `PackageTwo`/binding, not the old published package, so this is a missing explicit invariant assertion rather than evidence of observed mutation.
- The test does not assert the exact rejection category for each row or the candidate media/definition pair's source/reference identity after the failed attempt.

## Decision

**OPEN — P1 remains for R4-B1a.** The 21 rows now reach the actual adapter `TrySelect` path and demonstrate no additional save/projection plus tuple/status/current preservation, but the requested full view/highlight and explicit old media/binding reference assertions are still absent. Add those assertions on every row and request another narrow Luna review. Other R4-B categories remain outside this document.

