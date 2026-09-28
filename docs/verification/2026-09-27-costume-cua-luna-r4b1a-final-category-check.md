# Costume CUA R4-B1a final category narrow check

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: final narrow check of the four incorrect adapter-category expectations from R4-B1a closure review #3; Unity was not run and no implementation file was modified.
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`
- Adapter tests SHA-256: `9075D55327F9638D135AC0A5DC88943CDBC3D84329D36E989BAE3CF48D5D3198`
- Prior closure review SHA-256: `D73AA0DCB5659E34FBF67414B7A21D9E5BB1FA6279B8EC80B426FB239FB44A3E`

## Check

`ExpectedAdapter` now maps exactly these six catalog-selection failures to `Selection`:

- `definitionPending`
- `definitionRejected`
- `definitionActor`
- `definitionCostume`
- `bindingActor`
- `bindingCostume`

All other 15 rows map to `Package`. This matches the fixture mutation mechanics: the six rows either make the selected costume unavailable/non-accepted or replace its catalog ID/actor, while the remaining identity-preserving, synthetic, revision, hash and action-vocabulary rows retain selection eligibility and fail package validation. The 21-row enumeration, exact package rejection oracle, pre-highlight, deep view snapshot, old tuple/media/binding assertions, counters and status assertions remain present at `9075...:17–41`, `:350–356`.

## Decision

**CLOSED for R4-B1a.** The four previously incorrect expected adapter categories are corrected without relaxing the row or preservation assertions. This narrow result does not close the other known R4-B categories or authorize Unity execution by itself.

