# Costume CUA R5-final independent closure review — R3-P1-003

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Current CUA contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Adapter SHA-256: `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- Adapter-test SHA-256: `D10814120B1D4A98520114CA46DDA0A02194FC643DFF86889A64D90E422920DF`
- Concrete CIO SHA-256: `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E`

## Exact closure checks

The five named R5 tests now directly close the former P1:

- First save computes the exact staged canonical bytes, proves no publication during `Move`, asserts primary byte equality and temp `Missing`, and reflectively checks the private concrete result as `CommittedFirst`, `PrimaryDecode`, with the staged revision.
- Replacement captures the pre-replacement primary bytes, computes the exact staged canonical bytes, observes the old complete tuple in `BeforeReplace`, then asserts one `Replace`, primary byte equality, previous byte-for-byte equality with the captured old primary, temp `Missing`, and private result `CommittedReplacement`/`PrimaryDecode`/staged revision.
- Replacement projection fault proves the durable new tuple remains authoritative while old drawn pixels remain non-authoritative, status becomes `ReloadRequired`, primary/previous bytes remain exact, temp is missing, and the concrete result remains the successful replacement result. Subsequent `TrySelect`, `Observe`, and `Highlight` are terminal no-ops with unchanged bytes/counters/projection count.
- Same-revision different-primary corruption proves `CommitOutcomeUncertain` at `PrimaryEquality`, no publication or projection, the reopened primary differs from staged canonical bytes, temp is missing, and the private result carries the uncertain outcome/stage/expected revision. Subsequent mutation attempts do not retry or change files/counters.
- Public-surface inspection confirms the last-save result exists only in private fields; no public result property/method, receipt parameter, delegate, or alternate save seam is exposed.

The adapter stores the exact result from the same synchronous `_cio.Save(staged)` call before applying the single authoritative `_published = candidate` assignment. The tests inspect that private field without adding caller authority or a detached result seam. The unchanged CIO performs the replacement, reopen, byte-equality, decode, and expected-revision checks that produce the asserted result.

## Finding status

The sole prior R5 P1 required canonical staged-primary bytes, exact old-primary backup bytes, temp cleanup, exact `CostumeFileSaveResultV1` outcome/stage/revision, projection-fault durable authority, wrong-primary `PrimaryEquality` uncertainty/no-retry, and public-surface isolation. All are now directly asserted by the current test source and the unchanged concrete CIO/adapter path.

## Decision

**CLOSED / PASS — P0=0, P1=0, P2=0 for R3-P1-003.** The previous R5 finding is closed. This is a static closure only: it does not constitute Unity execution evidence and does not independently close R3-P1-004 or authorize a release decision.
