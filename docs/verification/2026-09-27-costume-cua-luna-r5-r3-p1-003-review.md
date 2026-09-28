# Costume CUA R5 narrow review — R3-P1-003 replacement boundary

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Current adapter-test SHA-256: `EF14FC9A8C0046B249A3F5E7A14EAF9BED911466E33D4A05359E29E6F9A992CD`
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Runtime CIO SHA-256: `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E`

## Five named R5 tests

1. `AC_CUA_003_R5_FirstSaveMoveSeesNoPublicationThenProjectionSeesCompleteDurableTuple` observes `Published == null` during the first move, then the complete new tuple at projection and decodes the primary revision/current ID.
2. `AC_CUA_003_R5_ReplacementReplaceBoundaryRetainsOldTupleThenProjectsOneCompleteNewTuple` observes the complete old tuple during `BeforeReplace`, asserts one `Replace` trace, observes one complete new tuple at projection, and decodes primary/previous revision/current-ID fields.
3. `AC_CUA_003_R5_ReplacementProjectionFaultRetainsDurableNewTupleAndClosesMutation` proves durable new tuple authority after a replacement projection throw, retains old drawn pixels as non-authoritative, enters `ReloadRequired`, and blocks selection/observation/highlight without counter or byte changes.
4. `AC_CUA_006_R5_ReplacementSameRevisionDifferentPrimaryBytesClosesWithoutPublicationOrRetry` injects a same-revision/different-state primary, expects `CommitOutcomeUncertain`, preserves the old tuple, and proves no retry/publication/projection after terminal closure.
5. `AC_CUA_006_R5_AdapterPublicSurfaceHasNoReceiptResultDelegateOrAlternateSaveSeam` rejects public receipt/result/delegate/alternate-save seams and requires the single public `TrySelect(CostumeUnityMediaPackageV1)` entry point.

## Closed portions

- The replacement path is real: the fixture starts with an existing primary, `MemoryPort.Replace` is invoked once, and the commit trace is `Move, Replace`.
- The adapter's unchanged source calls the concrete synchronous `_cio.Save(staged)` and only publishes when the result is `CommittedFirst` or `CommittedReplacement` with the staged revision. The test's accepted replacement plus the real `Replace` path therefore exercises the intended branch without introducing a detached receipt seam.
- `BeforeReplace` proves the old tuple/reference and nested state/media/binding/snapshot/frame are still authoritative before atomic replacement; projection observes the new complete tuple after the single `_published = candidate` assignment.
- Projection-fault semantics are closed: durable new tuple/state remain authoritative, old pixels remain only in `LastSuccessful`, status becomes `ReloadRequired`, and all three mutation entry points are terminal no-ops.
- Wrong-primary uncertainty and no-retry semantics are closed: the same-revision altered primary yields `CommitOutcomeUncertain`, leaves the old tuple published, and later `TrySelect`, `Observe`, and `Highlight` do not change bytes, counters, or projection count.
- The public surface check is closed for the stated forbidden seams.

## Remaining P1

**R3-P1-003 remains OPEN (P1=1).** The replacement tests decode only `Revision` and `Current("fixture")` through `AssertDurableState`; they do not assert the exact canonical primary bytes equal the staged replacement state and the exact previous bytes equal the pre-replacement primary. They also do not assert that the temp role is missing after successful first/replacement commit. `MemoryPort.Replace` removes the temp in its implementation, but an implementation detail is not an assertion in these R5 tests.

The tests likewise do not observe the concrete `CostumeFileSaveResultV1` value directly. `TrySelect == None` plus `ReplaceCalls == 1` and the accepted adapter branch makes `CommittedReplacement`/expected-revision logically implied by the current source, but the contract asks for explicit success-result and expected-revision proof. A minimal closure should capture pre-replacement primary bytes, compare primary and previous byte-for-byte after success, assert temp absence, and add a narrow CIO-result/expected-revision observation that does not add a CUA receipt seam.

## Decision

**OPEN / BLOCKED — P0=0, P1=1, P2=0.** R5 closes projection-fault authority, wrong-primary uncertainty/no-retry, one-reference observation boundaries, and the public-surface prohibition, but it does not yet close the exact durable replacement byte/role/result proof required by R3-P1-003. Unity execution and Approved/Verified restoration remain unauthorized for this finding.
