# M5D7Q-C2 Luna R10 pre-gate closure review

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Scope: independent read-only source review of the frozen C2 original-cohort,
teardown, terminal-containment, and retained-session-cell implementation. No
source edits, Unity execution, or acceptance claim were made by this review.

R10 focused Unity execution was active in the main lane when this review was
recorded and remains pending. The prior cohort review's two P1 findings are
closed by source inspection only; this note does not replace independent
runtime verification.

## Frozen source hashes

Prior cohort-review baseline and current closure snapshot:

| Artifact | Prior review baseline | Current frozen snapshot |
|---|---|---|
| `InputRouter.cs` | `D2A8B8396793D81CCB4D2F12E26137E29E6E1F79919B64D07513915BE55538FB` | `7B8B9FE1EDFC794D6D24CC42F377C6F81554DD19138A87DB550C6C5F86E99881` |
| `DesktopProfileLaunchAdapterV1.cs` | `35035543CC73A5BF55E7617E93770847EA942E37EA3DC763218185BD0A60F815` | `8C134D6316A95A98913132A31B0B4D0B79BF6BD75F7B5A6688D114A301C24FE0` |
| `ProfileResetMemoryCutoverV1.cs` | `5274BFC9E27E7F97BDCB41B680E36052A793B51498372B565BBBD24B538CFDC6` | `5274BFC9E27E7F97BDCB41B680E36052A793B51498372B565BBBD24B538CFDC6` |
| C2 PlayMode fixture | `932E9DE8258176B89EC6594ED91D8A97FE71F5FC617AD1BD5DD8F23BBC6846D0` | `1B1C4018A8A914D04155782E7ACCD4971C5F33E85A4B2A1653A9C7D1DDB11EEF` |
| C2 data fixture | not in prior cohort note | `DEB2C57D381D8641F2D706C1B381A76BA97AE64BA4AB1BFB0D5CD68899CB01D7` |

## Prior P1 closure

### P1 closure — immutable original teardown targets

The prior finding mapped to `REQ-M5D7QC2-003/004` and
`AC-M5D7QC2-003/004`: teardown delegates previously dereferenced mutable reset
action fields. The current router captures and validates the CWT-owned original
old/new action pair, then invokes disable, callback removal, and disposal on
those local immutable references. Entry-failure candidate disposal likewise
uses the registered original candidate. Source review finds no remaining
mutable-field redirection in these C2-owned operations.

### P1 closure — atomic exact-once disposal claim

The prior finding mapped to `REQ-M5D7QC2-004` and `AC-M5D7QC2-004`: disposal
attempt flags were non-atomic. The current adapter uses private CWT witness
words with `Interlocked.CompareExchange` before old/new disposal invocation;
local boolean fields remain diagnostic only. Source review finds the duplicate
claim race closed.

These are source-review closures only. They are not runtime PASS results.

## Remaining explicit AC matrix gaps

- `AC-M5D7QC2-002`: no complete executed creation/apply/binding/staged-disposal fault matrix.
- `AC-M5D7QC2-003`: terminal-gate coverage remains incomplete for every map-disable and callback-removal fault point.
- `AC-M5D7QC2-004`: explicit callback-removal, `DisposeNew`, transfer-boundary, session-cell-publication, and cross-proof rows remain required.
- `AC-M5D7QC2-006`: no complete executed final-reprobe mutation matrix covering every memory and disk field.
- `AC-M5D7QC2-007`: restart-after-`DiskPrepared` and restart-after-barrier-absence evidence remains absent.
- `AC-M5D7QC2-009`: static scope is bounded, but runtime regression evidence remains pending.
- `AC-M5D7QC2-010`: R10 focused/regression results and independent Luna P0/P1 disposition remain pending.

The retained current-session cell's getter path revalidates adapter/router CWT
identity, root, action identity, and generation. Completed receipt evidence is
detached from the live lease/capability. No P0/P1 is raised by this source
review, and no implementation or integration acceptance is granted.
