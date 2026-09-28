# C2R startup-compatibility runtime pre-gate — Luna review

Date: 2026-09-28  
Scope: frozen runtime and legacy-test review against the approved startup-
compatibility appendix (`4BDC...FFCE9`), source review only. No Unity execution
or source edits by Luna.

## Frozen hashes

| Input | SHA-256 |
|---|---|
| `DesktopProfileLaunchAdapterV1.cs` | `CE7343EE54FB641168919DB51CFFFBBA36944C0936A856679FAB788D09B0D3FC` |
| `InputRouter.cs` | `022729F8A82DEC467C196151ECCCE7C9AF7DE667F3D9B393FB7E997B2AE01936` |
| `ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` |
| `DesktopProfileLaunchAdapterV1Tests.cs` | `A94E193AD03213223EF0C734A78C2C2CF56783895A7C4DC6E604C7F81539E0D1` |
| `ProfileResetRestartProcessV1Tests.cs` | `17BEBBC75A0FF83C4F39C04DE104DF4172D23F6915C9E40E61556CF27E4A8B8E` |
| `qa/fixtures/C2RRestart/README.md` | `20BAC2F0BE0DD3E706357D3DD57A7D98D7D8A26DFA57414A03730E43F4872B3F` |

## Findings

### P1 — root-getter exceptions are misclassified by the new selection catch

`DesktopProfileLaunchAdapterV1.cs:249–269` places the environment root getter
inside the same `try` that catches `IOException`,
`UnauthorizedAccessException`, and `ArgumentException` as returned-root
selection failures. The approved appendix explicitly preserves the historical
behavior for **any exception thrown by the getter before it returns a value**:
propagate/log the original exception, close only the exact reservation, and do
not convert it to ManualRepair. Only an actually returned invalid/unsafe value
may take the C2R ManualRepair promotion.

This is an implementation P1 against the appendix's getter-throw rule and
`AC-M5D7QC2R-001/002/007`; it is not a P0. The current legacy row covers only
`InvalidOperationException` (`DesktopProfileLaunchAdapterV1Tests.cs:434–464`),
so IOException/UnauthorizedAccessException/ArgumentException getter seams are
not protected. The corrective shape should capture the getter result outside
the classification catch, then classify only validation/probing failures after
a value has been returned, while preserving original exception identity.

### P1 — process old-progress evidence remains revision-only

The strengthened process fixture now includes non-default bytes, generation,
and cell assertions, but its non-default seed is revision-only. It does not
include a canonical committed choice/offer, granted skill, or completed branch.
Therefore it can support a narrower “exact revision/r0 and session agreement”
claim, but not the strongest old-progress-resurrection claim in
`AC-M5D7QC2R-005/006` without at least one valid non-default progression fact.
This is an evidence-gap P1, not a runtime defect.

### P1 — closed repair promotion retains a null root that the lifecycle invariant rejects

`InputRouter.cs:264–270` and `:276–280` intentionally set `_recoveryRoot=null`
for a returned-root repair classification. `CloseRecoveryReservation` then
transitions `RepairBlocked` to `Closed` (`:287–289`), while
`ValidatePreparedInvariant` requires a non-empty root for every recovery state
except `RepairBlocked` (`:1275–1280`). Thus Start/Update/teardown validation
after a readable ManualRepair result can treat the already-closed repair row as
malformed and latch an additional failure. This violates the appendix's
requirement that readable closed ManualRepair remain quiescent and teardown
close only the exact owner. It is a source P1; the fix must distinguish the
private unknown-root repair witness from reflected/corrupt `None` state, rather
than broadly relaxing the invariant.

## Closed review points

Reservation now precedes the single root read; the captured root is reused for
ordinary preparation; promotion is one-way and exact-pristine; ordinary
mutators are revoked; private recovery CWT identity is authoritative; and
teardown does not rearm or fall back. The prior compile, lease, one-shot CAS,
post-publication getter, cleanup, and mutable-state fallthrough findings remain
closed by source review. Bootstrap tests were still changing and were not
frozen or reviewed here.

## Gate disposition

No P0 found. Implementation is **not pre-gate closed**: fix and add focused
getter-throw and closed-repair lifecycle rows for the P1s above, and either seed one canonical non-default
progression fact or narrow the process AC claim. No execution or acceptance was
performed by Luna.
