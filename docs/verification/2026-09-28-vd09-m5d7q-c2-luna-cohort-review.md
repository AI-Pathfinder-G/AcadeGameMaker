# M5D7Q-C2 Luna cohort/terminal review

Date: 2026-09-28  
Reviewer: Luna, bounded independent read-only review  
Scope: frozen C2 Unity runtime and focused PlayMode fixtures; no Unity execution and no source edits. Main-lane R9 smoke was active; historical R6/R8 partial records are not current acceptance evidence.

Baseline source hashes (SHA-256, unchanged by this review):

- `InputRouter.cs`: `D2A8B8396793D81CCB4D2F12E26137E29E6E1F79919B64D07513915BE55538FB`
- `DesktopProfileLaunchAdapterV1.cs`: `35035543CC73A5BF55E7617E93770847EA942E37EA3DC763218185BD0A60F815`
- `ProfileResetMemoryCutoverV1.cs`: `5274BFC9E27E7F97BDCB41B680E36052A793B51498372B565BBBD24B538CFDC6`
- `ProfileResetMemoryCutoverV1Tests.cs`: `932E9DE8258176B89EC6594ED91D8A97FE71F5FC617AD1BD5DD8F23BBC6846D0`
- `ProfileResetMemoryCutoverDataV1Tests.cs`: `DEB2C57D381D8641F2D706C1B381A76BA97AE64BA4AB1BFB0D5CD68899CB01D7`

## Findings

### P1 — CWT ownership does not make teardown operation targets immutable

`InputRouter.cs:351-356,388-395` and `:372-378` invoke map/callback/dispose operations through mutable `_resetOldActions`/`_resetNewActions`. The CWT/cohort references protect claim and attempt bookkeeping, but the operation lambdas still dereference mutable fields. A checkpoint/control mutation between eligibility and the operation can redirect `Disable`, callback removal, or `Dispose` to a foreign wrapper. The same issue exists in the entry-failure helper (`DisposeClaimedResetCandidateOnce`) after candidate claim. Local immutable originals must be the only operation targets; CWT flags alone are insufficient.

Mapping: **REQ-M5D7QC2-003/004**, **AC-M5D7QC2-003/004**.

### P1 — CWT exact-once disposal flags are non-atomic

`DesktopProfileLaunchAdapterV1.cs:118-123` reads and writes `OldDisposeAttempted`/`NewDisposeAttempted` as ordinary booleans. Two teardown paths can both observe `false` and both claim the close attempt before either write becomes visible, allowing duplicate disposal. The exact-once witness needs an atomic compare-and-exchange (or an equivalent serialized ownership gate) before any external operation.

Mapping: **REQ-M5D7QC2-004**, **AC-M5D7QC2-004**.

## Bounded observations

The current private CWT cohort cross-checks correctly reject ordinary sibling/router/root substitutions, and terminal getters consult both local state and the cohort registry. Candidate registration/claim is now adapter-owned and preclaim disposal uses the registered candidate. These observations do not offset the two teardown-target/atomicity findings above.

No acceptance score or closure claim is made. Hashes were captured again after review and remained identical to the baseline values above.
