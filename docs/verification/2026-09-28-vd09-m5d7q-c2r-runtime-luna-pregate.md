# M5D7Q-C2R frozen runtime Luna pre-gate

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Scope: independent read-only review of the frozen C2R runtime against the
Approved C2R contract and Approved C2. No Unity execution, source edits, or
acceptance were performed.

## Frozen hashes

| File | Prior C2 baseline | Current C2R snapshot |
|---|---|---|
| `DesktopProfileLaunchAdapterV1.cs` | `8C134D6316A95A98913132A31B0B4D0B79BF6BD75F7B5A6688D114A301C24FE0` | `01FCB1763BB0F54B5E5B0F3F1D1AF0E1D841192A15B26126DF83FDE1A3CF840B` |
| `InputRouter.cs` | `7B8B9FE1EDFC794D6D24CC42F377C6F81554DD19138A87DB550C6C5F86E99881` | `7F0977C144CAE91D250835A5F37041EF66659480A557003F71AD06790EB76629` |
| `ProfileResetMemoryCutoverV1.cs` | `5274BFC9E27E7F97BDCB41B680E36052A793B51498372B565BBBD24B538CFDC6` | `641BD580AD5183AA7A321499C368DF5DC3F9ADE97733CB58888289E50B834C24` |
| frozen matrix fixture | `F1EAA0EB7E7E955A1C57C2C230A55FAED3D6FD3AB18169E68333AFA08D0562D` | unchanged |

Approved C2R contract: `DA1BD70DBBD42DC6817847B63411DEACF18FEC448FEC547A441E91B4C6E66C2D`.

## P0 compile blocker

`DesktopProfileLaunchAdapterV1.cs:303-309` declares
`recoveryBeforeResume` but lines `307-308` dereference an undeclared
`recovery`. The frozen three-file runtime cannot compile until this identifier
is corrected. No Unity compilation was run by this review.

## P1 findings

### P1 — recovery catch releases the lease before terminal failure latch

`ProfileResetMemoryCutoverV1.cs:106-117` scopes the root lease inside the
`try`, while the catch at `:119-120` calls `owner.LatchResetCutoverFailure()`
only after the `using` scope has disposed the lease. After recovery has crossed
the accepted boundary, this reverses the required terminal-latch-before-lease-
release ordering.

Mapping: `REQ-M5D7QC2R-003/004`, `AC-M5D7QC2R-003/004`.

### P1 — post-acquire Busy is structurally reclassified as Busy

The catch at `ProfileResetMemoryCutoverV1.cs:119` wraps the whole recovery
flow, including post-acquire staging/terminal work. Any
`ProfileRootOperationBlockedExceptionV1` raised after the lease is held is
returned as `ProfileResetMemoryOutcomeV1.Busy`, allowing a nonterminal result
after recovery has begun. C2R requires `BusyBlocked` only for pre-recovery
lease contention; post-lease failures must be terminal `ReloadRequired` or
`ManualRepairRequired`.

Mapping: `REQ-M5D7QC2R-004/005`, `AC-M5D7QC2R-004`.

### P1 — receipt publication is followed by fallible getter work in Adapter.Start

`ProfileResetMemoryCutoverV1.cs:116` publishes the receipt as the final
authoritative assignment inside the coordinator. After it returns,
`DesktopProfileLaunchAdapterV1.cs:309-311` evaluates
`memoryResult.Outcome` and `memoryResult.StagedRestartBootstrap`; both invoke
validation getters and can throw after publication. This violates the
no-fallible-operation-after-publication rule and can leave a completed receipt
with an unrecorded/uncaught bootstrap state.

Mapping: `REQ-M5D7QC2R-004`, `AC-M5D7QC2R-005/006`.

### P1 — mutable restart state can route Start toward the ordinary body

`DesktopProfileLaunchAdapterV1.cs:298-316` branches solely on mutable
`_restartBootstrap`. If reflection changes that field to `None` while a private
recovery CWT witness still exists, `Start` attempts the ordinary launch body
instead of rejecting the corrupt recovery lifecycle. The ordinary body will
normally fail closed on missing adoption, but the required CWT-backed
diagnostic-corruption rejection must happen before any ordinary-path attempt.

Mapping: `REQ-M5D7QC2R-001/003/005`, `AC-M5D7QC2R-002/003`.

### P1 — RepairBlocked Start does not synchronize the private result witness

`DesktopProfileLaunchAdapterV1.cs:300` closes a `RepairBlocked` startup and
returns without assigning `RestartRecoveryWitness.Result`. When a temporary-
barrier path created a witness at `:263`, the later `RestartBootstrapResult`
getter at `:401-409` compares that null witness result with `_restartResult` and
throws. The closed repair result must remain readable after Start/close.

Mapping: `REQ-M5D7QC2R-001/005`, `AC-M5D7QC2R-001/004`.

### P1 — unsafe/ambiguous recovery selection and held-authority failures collapse to ReloadRequired

`DesktopProfileLaunchAdapterV1.cs:243-257` converts only `IOException` and
`UnauthorizedAccessException` from root/barrier selection into the closed
repair reservation. Malformed/invalid root exceptions from the Profile
containment helper (for example `ArgumentException`) escape `Awake` instead of
producing the required closed repair result. More importantly,
`ProfileResetMemoryCutoverV1.cs:108-120` catches all restart pre-lease and
reauthentication failures as `ReloadRequired`; it does not preserve the C2
`ManualRepairRequired` classification for unsafe root, lock-path, reparse, or
manual-repair disk authority cases.

Mapping: `REQ-M5D7QC2R-001/004/005`, `AC-M5D7QC2R-001/004`.

### P1 — repair-blocked bootstrap has no private recovery witness but its getter requires one

In `DesktopProfileLaunchAdapterV1.cs:249-257`, root/selection failure creates a
`RepairBlocked` result and calls `ReserveRecoveryBlockedNoActions` without
registering a `RestartRecoveryWitness`. `RestartBootstrapResult` at
`:401-409` requires a private recovery witness for every non-
`OrdinaryLaunchRequired` result. Therefore a valid repair-blocked result can
throw as malformed when read, violating closed typed-result behavior.

Mapping: `REQ-M5D7QC2R-001/005`, `AC-M5D7QC2R-001/002/004`.

### P1 — recovery execution has no one-shot CAS before reauthentication I/O

`DesktopProfileLaunchAdapterV1.cs:303-309` guards only the adapter `Start`
Resume invocation. `FinalizeRestartRecovery` checks `Minted == 1` at
`ProfileResetMemoryCutoverV1.cs:103-111`, but does not atomically consume a
private execution witness before root validation, lease acquisition, or proof
reauthentication. A duplicate private entry can therefore repeat recovery I/O
and race until the proof CAS rejects one caller. This does not permit a second
receipt, but it fails the contract's duplicate-bootstrap rejection boundary.

Mapping: `REQ-M5D7QC2R-002/004`, `AC-M5D7QC2R-002/004`.

### P1 — held authority and exact binding evidence are not revalidated before recovery terminal entry

`ProfileResetMemoryCutoverV1.cs:111-114` reauthenticates once, applies binding,
then enters `AdoptResetCandidateForTerminal`/`EnterRestartRecoveryTerminal`
without the same immediate `authority.ValidateForHeldLease` used by ordinary C2
at `:152`. Recovery also calls only `binding.Validate()` at `:113`; the explicit
`DefaultsReady`/`None`/`Retain`/empty-overrides checks occur later through the
session-cell path, after terminal entry and action transfer. This weakens the
pre-gate exact-default and held-lease boundary under fault/interleaving seams.

Mapping: `REQ-M5D7QC2R-003/004`, `AC-M5D7QC2R-003/004`.

## Reviewed sound areas

The frozen source captures original router/control/action references for
operation delegates; recovery has no fabricated old-action disposal claim;
CWT witnesses and CAS flags prevent local reflection booleans from rearming
ownership; adapter `-220` Awake precedes router `-210` Awake/Start; BCL root,
ancestor, lock-path, and both barrier probes occur before ordinary preparation;
`Busy` maps to `BusyBlocked`; receipt/bootstrap construction precedes the final
authoritative receipt assignment; and router recovery remains actionless and
quiescent until the private recovery path. These observations do not offset the
P1s above, and C2R remains unaccepted pending correction and independent
runtime execution.
