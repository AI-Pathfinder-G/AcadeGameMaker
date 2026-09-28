# VD-09 M5D7Q-C2 Profile authority implementation handoff

- Date: 2026-09-28
- Implementer: Terra (`gpt-5.6-terra`)
- Scope: C1-owned held-lease proof reauthentication, authenticated barrier
  removal, and final disk verification only
- Status: partial implementation handoff — execution and independent verification pending

## Traceability

- `REQ-M5D7QC2-001` / `AC-M5D7QC2-001`: a prepared proof is structurally
  checked and bound to the supplied normalized root and actual held lease. The
  canonical live marker, exact C1 archives, active target-only r0 row and
  barrier-temp absence are checked before the final one-consumer CAS.
- `REQ-M5D7QC2-005` / `AC-M5D7QC2-005`: only the minted lease-reference-bound
  authority can perform one direct BCL marker deletion. The deletion attempt is
  latched before the call; missing, changed, inaccessible or unsafe marker
  states remain reload-required and cannot retry with that authority.
- `REQ-M5D7QC2-006` / `AC-M5D7QC2-006`: final verification requires the
  attempted deletion, exact retained archive and active r0 row, and absence of
  both barrier names. It may mint only the detached immutable
  `ValidatedResetFinalDiskProofV1` after that successful held-lease probe; the
  proof retains no live lease or authority and creates no disk data. A
  transaction-private witness plus weak-table registration is required before
  detached proof validation succeeds; scalar or boolean candidates cannot mint
  final evidence.
- `AC-M5D7QC2-009`: no Unity, UI, scene, save, restore, network, clock or
  random authority was added to this bounded Profile source.

## Delivered files

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileResetMemoryAuthorityV1Tests.cs`

The focused test source currently defines eighteen test methods (including
fifteen parameter rows) covering exact and foreign roots, altered marker/proof
consumption ordering, old-primary-equals-r0 archive semantics, before/after
direct-delete faults, a real exclusive marker lock, missing/directory marker
rows, pre-CAS mutations of every active/barrier-temp/archive row, and
post-delete archive/active/barrier mutations and a positive exact-final row.
Actual root, ancestor, lock-path, marker, and archive NTFS junction cases are
included; the C2 Unity owner half supplies
the later no-staging assertions.

The delete-fault coverage includes a real Windows sharing row: the closed
test-control opens a marker handle sharing `Read` but not `Delete` at the
production `BeforeBarrierDelete` boundary, after all production marker/archive
reauthentication and attempt latching. The unchanged direct `File.Delete` call
then fails with a sharing `IOException`; the authority records that attempt and
rejects a same-authority retry. The control owns and closes only that real test
handle and never supplies an observation, result, or alternate disk operation.
No test has been run by this handoff. The approved test-only NTFS fixture now
creates and verifies real mount-point reparse tags with `CreateFile` and
`DeviceIoControl`; it exercises barrier and archive-directory junction cases
under one owned OS-temp base. Fixture inability is not skipped or passed.
