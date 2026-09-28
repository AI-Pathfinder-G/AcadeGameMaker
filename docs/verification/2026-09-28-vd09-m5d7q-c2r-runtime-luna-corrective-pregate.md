# C2R corrective runtime pre-gate — Luna independent review

Date: 2026-09-28  
Authority: Approved C2R contract `DA1BD70DBBD42DC6817847B63411DEACF18FEC448FEC547A441E91B4C6E66C2D`; original C2 specification and M5D7M root/UTC ordering remain normative.  
Scope: frozen source review only. No Unity execution, acceptance, or source change by Luna.

## Frozen inputs

| Input | SHA-256 |
|---|---|
| `DesktopProfileLaunchAdapterV1.cs` | `02E6D526EF00208A8A93BD4A24EC4D62CAD560978036B541306625690D49FF44` |
| `InputRouter.cs` | `7F0977C144CAE91D250835A5F37041EF66659480A557003F71AD06790EB76629` |
| `ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` |
| `ProfileResetRestartBootstrapV1Tests.cs` | `03BF6A1C6DA36567D80CA200F0026981BEC0EEE494355605A0EF454F6D1432EB` |
| `ProfileResetRestartProcessV1Tests.cs` | `2CF21A5B3B7965A7E937662EBA1B0BA10D8D329186BA304571650846C5B334F0` |
| `ProfileResetMemoryCutoverFaultMatrixV1Tests.cs` | `F1EAA0EB7E7E955A1C57C2C230A55FAED3D6FD3AB18169E68333AFA08D0562D9` |
| `qa/fixtures/C2RRestart/README.md` (runner isolation amendment) | `9953B6D80915BA8DE053C950F194791002CE943FEBF47FE694D6C37184A06B83` |

## Findings

### P1 — first root read and ordinary root read violate the existing ordering and single-root contract

`DesktopProfileLaunchAdapterV1.cs:243–251` performs C2R root validation and barrier probing before any reservation. If no barrier is found, `:276–280` then reserves the ordinary launch and calls `_environment.GetPersistentDataPath()` again. A mutable/test environment can therefore return root A for the safety decision and root B for `Prepare`; the second root can be different, unsafe, or contain a committed barrier that was never selected. This conflicts with the original M5D7M `REQ-M5D7M-001` / `AC-M5D7M-001` Reserve → root → UTC → Prepare trace (including first root read while Reserved) and the C2R ordinary-lane/root-scope requirements (`AC-M5D7QC2R-001`, `AC-M5D7QC2R-007`).

This is a source P1 gate, not an execution result. A bounded design direction for Astra is an exact, actionless `ReservePreparedHubLaunch` before the single root capture; an absent-barrier path may remain ordinary, while a committed/unsafe result is one-way promoted to the private recovery lane only while the reserved cohort is pristine. The ordinary path must reuse the captured root and must not allocate actions, receipt, or a fake cohort during selection. No implementation is accepted by this review.

### P1 — process evidence does not yet prove the non-default old-state and session-agreement claims

`ProfileResetRestartProcessV1Tests.cs:21–38` prepares only the default C1 transaction, then asserts exact-r0 bytes and receipt/barrier effects in Resume. It does not seed a non-default canonical primary/archive state, and does not explicitly assert the resumed cell's current-cell, generation, and snapshot-byte agreement. Therefore the four-process fixture is structurally safe and its missing/wrong environment behavior is correctly a failure, but it cannot by itself close the strongest `AC-M5D7QC2R-005`/`AC-M5D7QC2R-006` old-progress-resurrection and session-agreement claims. Add a canonical non-default seed and explicit cell/generation/snapshot assertions, or narrow the claimed AC evidence. This is a verification/evidence P1, not a runtime acceptance.

The amended process README's isolation decision is sound: the four phases require external process control, each phase must run separately, the process fixture is explicitly excluded from the required non-process regression filter, and DeleteGate is parent-terminated rather than a falsely passing/missing-XML NUnit result. No Skip/Ignore/extra assembly or environment guard weakening was observed.

## Previously reported corrective closures confirmed by source review

No remaining P0 was found in this frozen snapshot. The prior compile blocker (`recoveryBeforeResume`), post-acquire Busy classification, lease-scope terminal latch, private recovery execution CAS before I/O, exact held-lease/binding validation, repair-witness/result synchronization, raw post-publication result handoff, mutable-state ordinary fallthrough guard, and staged-candidate cleanup result overwrite are addressed in the reviewed source. These are source-review observations only; Unity execution remains pending.

## Gate disposition

Implementation remains **not approvable** until the root reservation/single-capture issue is resolved against the approved ordering and the process fixture evidence gap is either strengthened or explicitly narrowed. The frozen source and fixture hashes above are the before/after record for this review; no runtime/test execution was performed by Luna.
