# Costume CUA R4-B4 and R3-P1-002 aggregate closure review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: static/adversarial review only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Sol proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Current adapter-test SHA-256: `DB5119043CC7FC13692D57C361CB246475467B72FC4C0116AEF927F830156136`
- Runtime media SHA-256: `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79`
- Runtime adapter SHA-256: `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB`

## R4-B4 row-by-row review

The exact `R4B4Cases` array contains six named rows and each row is independently dispatched by `AC_CUA_002_R4_B4_ExactCandidateBoundaryPreservesPublishedPair`:

| Row | Static result | Evidence |
|---|---|---|
| `clipPropertyOrder` | CLOSED | `MutateB4ClipMapBytes` performs a guarded exact nested clip-object reorder (`actionId,facing,loop,frames` → `facing,actionId,loop,frames`). The candidate rebinds the mutated hash, `Validate` must reach `Manifest`, then `TrySelect` reaches `Package`; B1a preserves the old tuple/view/counters. A no-op would fail the required `Manifest` result. |
| `framePropertyOrder` | CLOSED | Performs a guarded exact nested frame-object reorder (`x,y,width,height` → `y,x,width,height`), rebinds the hash, requires `Manifest`, and applies the same adapter/B1a preservation path. |
| `foreignPackage` | CLOSED | `CreateForeignPackage` creates a complete, independently valid package with foreign actor/costume/set identity. It first proves `Validate == None`, then real `TrySelect` returns `Selection` with the B1a no-mutation/preservation oracle. |
| `stalePackage` | CLOSED | `CreateStalePackage` uses a prior catalog revision while preserving otherwise canonical bytes and validates independently. The adapter then rejects the stale candidate as `Package`; B1a proves no save/projection or published-pair mutation. |
| `previouslyRejectedCandidate` | CLOSED | A real manifest-invalid candidate is first rejected by `Validate`, then submitted twice to the adapter. Both calls return `Package`, and the complete B1a oracle runs after each call, proving repeat rejection is side-effect free. |
| `missingSnapshotAction` | CLOSED | The candidate is valid but omits `bowdraw`; a `bowdraw` snapshot is observed against the already published valid package, then candidate selection returns `Snapshot` before commit. B1a proves old pair/view/status/counters remain unchanged. |

The helper `ReplaceExactOnce` rejects a missing canonical sequence, while each property-order row's required `Manifest` result makes an accidental no-op fail. The foreign/stale candidates are independently constructible and validated before adapter rejection; no Unity behavior is inferred.

## Aggregate R3-P1-002 ruling

R4-B4 closes the six exact rows previously identified as missing by the aggregate review:

- nested clip-object property order;
- nested frame-object property order;
- foreign package;
- stale package;
- previously rejected candidate;
- missing snapshot action.

Together with the already closed B1a (`8BFF4B00DEAD6F9179B474B01E1355D24B21FC8E4B0344C3752AD6544DC2E33C`), B1b1 (`A3AE5341F0B9D1254ADA761FF7D79F1DC38CE54594F47B811F9336B85562AAB2`), B1b2 (`608649781756BD1045ABDC67D6F7567486F96BF682C2C772DE7B7DF1F775782E`), B2 (`56DDA793397DCF6AAAB2FCB3D79FE78AAB8971E834492FB20E02F0CFA0BDAD69`), and B3 (`7EF8082515E047C833A3EF56B592D12E0DFC6A29D873BFDC828F5BBA585856CA`) evidence, the Sol proposal's complete package mutation table is statically enumerated and covered by real mutation/fixture paths and the applicable rejection/preservation assertions.

## Decision

**CLOSED / PASS for R4-B4 and aggregate R3-P1-002 — P0=0, P1=0, P2=0 within this narrow scope.** The package mutation matrix no longer has the six previously identified static gaps. This result does not by itself close R3-P1-003/R3-P1-004, does not constitute Unity execution evidence, and does not authorize a broader product or release decision.
