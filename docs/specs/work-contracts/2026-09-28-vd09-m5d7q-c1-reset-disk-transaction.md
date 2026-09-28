---
status: Verified
---

# VD-09 M5D7Q-C1 confirmed reset disk transaction

- Date: 2026-09-28
- Status: Verified — Astra 2026-09-28; disk-only scope, not live menu integration
- Owner and approval: Astra
- Bounded design: Sol; intended implementation: Terra; independent verification: Luna
- Parent: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md`
- Parent trace: `REQ-M5D7QC-003/004/006/007`, `AC-M5D7QC-002/003/004/005/007`
- Dependencies: M5D7A/B/D/G/L/M Verified; M5D7Q-C launch root-lease slice partially verified

## Purpose and boundary

C1 is the engine-free, already-confirmed disk half of New Game reset. It owns
fresh identity validation, durable barrier publication, byte-for-byte archival
of the three automatic candidates, crash-idempotent resume, and creation of the
exact approved default v1 document at revision `0` through the existing M5D7D
save core. It stops with the valid barrier retained and a typed disk-prepared
proof. A later contract owns memory/settings/input application, barrier removal,
success receipt, menu or scene behavior.

C1 has no confirmation UI, request consumption, confirmation decision, memory
mutation, live input/map mutation, notification, scene/run authority, success
receipt, archive cleanup/restore, or power-loss guarantee for directory
metadata. `DiskPrepared` is not reset success and ordinary load/save remains
blocked while the barrier exists.

## Fixed paths and containment

The normalized non-root absolute profile root and its live
`ProfileRootOperationLockV1` lease use the already verified rules. C1 accepts
no caller-supplied leaf name or archive path. The only active names are:

- `profile.operation.lock`
- `profile.reset.tmp.json`
- `profile.reset.json`
- `profile.json`, `profile.prev.json`, `profile.tmp.json`

One transaction archive is
`recovery/new-game/<transactionId>/`. Its only old-leaf filenames are
`primary.bin`, `previous.bin`, and `temp.bin`. Interrupted new-default bytes use
`interrupted-default-temp/<sha256>-<byteLength>.bin` or
`interrupted-default-primary/<sha256>-<byteLength>.bin`, according to their
active source role. The hash is lowercase 64-hex and length is invariant
unsigned decimal without leading zero except `0`. No other file is created by
C1.
Both interrupted-file destinations are non-overwriting: any existing
destination, even byte-identical, requires manual repair; no deletion or suffix
guessing is permitted.

`transactionId` is exactly 32 lowercase hexadecimal characters generated from
16 cryptographically random bytes. It contains no separator, dot, whitespace,
or alternate representation. Before every archive operation, the root,
`recovery`, `new-game`, transaction directory and destination parent must be
opened/inspected without following a reparse point; every full path must remain
beneath the normalized root using `OrdinalIgnoreCase` boundary comparison.
Existing transaction directory, destination collision, reparse point, or
unprovable containment fails closed. Resume uses only the ID authenticated by
the barrier and may find its existing directory.

## Fresh confirmation identity

The caller supplies an immutable `ProfileResetConfirmationIdentityV1` created
by the future confirmation owner from one fresh observation under no mutation.
It contains exactly three role records in Primary, Previous, Temp order. Each
record is either `Missing`, or `Present(classification, byteLength, sha256,
decodedRevision)`. Classification is one of `ValidCurrentInput`,
`ValidInputRecoveryRequired`, `UnsupportedSchema`, or `Invalid` and is the
existing decoder projection; `decodedRevision` is a nonnegative integer only
for the two valid classifications and is otherwise JSON `null`. SHA-256 is
lowercase 64-hex over the exact file bytes and byte length is nonnegative.
Unreadable is not an identity and confirmation cannot authorize it.

After acquiring the root lease, C1 observes all three leaves afresh using
exclusive complete reads. Every role must exactly match the supplied presence,
classification, length, hash and decoded revision: fresh bytes must match
length/hash and decoding those same bytes must match classification/revision.
Generation here means this byte fingerprint, not filesystem timestamps.
Any change returns `ConfirmationStale` before creating an
archive or barrier. This identity is single-use process state: a second begin
call is rejected. Resume never accepts or needs a confirmation identity; the
committed marker is its sole authority.
The engine-free internal APIs are `CaptureConfirmationIdentity(root)`,
`Begin(root, identity)` and `Resume(root)`. Capture owns a root lease, rejects
barriers, and creates no reset metadata. It is an observation, not UI/user
consent; only a future confirmed-consent owner may invoke Begin in production.
Identity constructors are private, records defensively copied, and identity
includes normalized root. Begin acquires the lease before atomic single-use
consumption; Busy leaves identity unconsumed, while stale or read failure after
consumption requires recapture/reconfirmation. Resume takes no identity.

## Canonical barrier v1

The barrier is UTF-8 without BOM, newline, insignificant whitespace, duplicate
keys, escaping alternatives, or extra fields. It is exactly this property
order, with role objects ordered Primary, Previous, Temp:

```json
{"format":"AcadeGameMaker.ProfileResetBarrier","version":1,"transactionId":"<32-lower-hex>","target":{"profileRevision":0,"byteLength":<n>,"sha256":"<64-lower-hex>"},"oldLeaves":[{"role":"Primary","fileName":"profile.json","presence":"Missing","classification":null,"byteLength":null,"sha256":null,"decodedRevision":null},{"role":"Previous","fileName":"profile.prev.json","presence":"Missing","classification":null,"byteLength":null,"sha256":null,"decodedRevision":null},{"role":"Temp","fileName":"profile.tmp.json","presence":"Missing","classification":null,"byteLength":null,"sha256":null,"decodedRevision":null}],"bodySha256":"<64-lower-hex>"}
```

For a present leaf, `presence` is `Present` and the four identity fields use
the exact values above. For a missing leaf all four are JSON `null`.
`target` is `ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument`
only; its revision must be `0`, length is its exact canonical file byte length,
and hash is over those exact bytes. `bodySha256` is SHA-256 over the canonical
UTF-8 bytes of the same object with the entire final
`,"bodySha256":"..."` member omitted. This definition, not object serializer
defaults, is normative.

The marker decoder requires exact field set/order/spelling/types, canonical
number and string forms, format/version, closed roles/classifications,
role/file-name pairing, cross-field nullability, transaction/path rules,
target equality to the current approved default planner, and body hash. Any
unknown, malformed, noncanonical, unreadable, oversized, directory-valued or
unsupported barrier is terminal `ManualRepairRequired`; it is never absent and
never falls through to ordinary Previous recovery. Maximum marker size is
8192 bytes; a larger readable marker is malformed.

## Publication and mutation boundary

Begin creates the unique archive directory, writes the complete marker bytes
to `profile.reset.tmp.json` with exclusive create, write-through, full write,
`Flush(true)`, close and exclusive re-read validation, then publishes it with
same-directory no-overwrite `File.Move` to `profile.reset.json`. The committed
barrier is reopened and validated before any active leaf move. Any temp-name
collision blocks Begin; it is never promoted. Any failure before a validated
committed barrier leaves all three active leaves authoritative and unchanged;
an empty transaction directory and barrier temp are inert.

Once the committed barrier exists, old active filenames are never restored.
Only validated resume toward exact default is allowed. A move is
source-to-fixed-destination on the same volume, followed by exclusive reopen
and exact length/hash verification. For every originally present old leaf:

- matching source only: move is pending;
- matching destination only and source absent: move completed;
- both source and destination, neither, mismatched/unreadable source or
  destination, or destination collision: `ManualRepairRequired`.

For an originally missing leaf, both source and its destination must remain
absent until old-leaf archival is complete; unexpected presence is manual
repair. Readable invalid/unsupported old leaves are preserved exactly like
valid leaves. An unreadable old leaf prevents Begin; on resume it blocks.
Archive order is Temp, Primary, Previous. Ordering is normative only for new
work; disk identities, not a mutable phase counter, decide restart state.

The marker identity owns old/default interpretation. Therefore old Primary
bytes equal to the target default are still old until `primary.bin` is proved
and the active source is absent. An active target-equal `profile.json` is a new
reset product only after all marker-declared old leaves are proved archived.
Byte equality alone never advances that boundary.
Resume uses two passes. First inspect every declared destination. If any old
archive is incomplete, apply the strict old source/destination matrix to all
roles before moving, and prohibit new-product interpretation. If all declared
destinations are exact (and originally missing destinations absent), the old
archive is complete; then and only then classify active Primary/Temp as reset
products. Active Previous must be absent. Complete archive plus new Primary
does not violate the incomplete-archive both-copies rule, even when old Primary
equaled the target. Uncooperative external tools reinserting old bytes after
archival are outside the cooperative root-lock/process-crash model; identical
bytes cannot prove their writer and no timestamp inference is used.

## Exact first save and interrupted temp

After every declared old destination is verified and all three active leaves
are absent, C1 calls one new internal reset-only capability:

```csharp
ProfileAtomicSaveResultV1 SaveResetFirstUnderHeldRoot(
    ProfileRootOperationLockV1 lease,
    ValidatedProfileResetBarrierV1 barrier,
    string root,
    ProfileCanonicalDocumentV1 exactDefault);
```

It validates the live lease/root and the still-present exact barrier, requires
the exact barrier target/default planner document, and requires Primary and
Previous absent. It then delegates to the existing M5D7D first-save core. It
does not call `ThrowIfPresent`; this unforgeable barrier capability is the only
barrier-present save exception. Public save, ordinary held save, input-repair
save, load, and launch preparation remain forbidden by the barrier.
The capability has a private constructor and is minted only through the reset
coordinator's exact marker validation. It binds the actual live lease,
normalized root, transaction ID, exact marker bytes/hash and target bytes/hash.
Every use revalidates these against disk; disposed/wrong lease, altered root,
marker or target and malformed/reflection-corrupted capabilities fail closed.
Reflection tests exercise malformed invariants, not protection against an
unrestricted CLR attacker rewriting every private member consistently.

Before each first-save attempt, a present `profile.tmp.json` is never decoded
or loaded. It is exclusively read, hashed, and moved without overwrite to its
derived interrupted-default Temp filename, then reopened and verified. If the
destination exists, even with identical bytes, return `ManualRepairRequired`
without deleting or overwriting either copy. Collision, unreadable temp,
failed move, or unprovable absence blocks. Thus every
interrupted default temp remains preserved while retries can be idempotent.

`CommittedFirst` is accepted only with exact target bytes/revision in Primary
and Previous/Temp absent. `FailedBeforeCommit` returns `ReloadRequired` with
the barrier retained; a later process may resume. `CommitOutcomeUncertain`
returns terminal `ReloadRequired`, performs no in-session retry, and leaves
disk inference to a later resume. On resume, complete archive plus exact target
Primary and absent Previous/Temp is already disk-prepared. A corrupt,
invalid, unsupported or mismatching readable active Primary is moved without
overwrite to `interrupted-default-primary/<sha256>-<length>.bin`, reopened and
verified, and rebuilt only after its active absence is proved. This is allowed
only after complete old-archive proof, never during old archival. Primary bytes
are never confused with Temp bytes or inferred from decoder classification.
Unreadable bytes block. Old archive proof remains mandatory before any rebuild.

## State machine and typed output

The closed durable states inferred under ownership are:

1. `NoBarrier`: ordinary behavior; Begin may fresh-validate confirmation.
2. `BarrierPublished`: valid marker; no old destination yet required complete.
3. `ArchivingOldLeaves`: strict per-role source/destination matrix in progress.
4. `OldLeavesArchived`: all declared archives exact; active three absent.
5. `DefaultCommitUncertain`: archive complete and active files are not yet an
   exact target-only row.
6. `DiskPrepared`: archive complete, Primary exact target, Previous/Temp
   absent, valid barrier retained.
7. `ManualRepairRequired`: barrier or disk identity cannot prove one state.

No phase is persisted separately and no timestamp/order guess is allowed.
`ProfileResetDiskOutcomeV1` is closed to `DiskPrepared`, `ConfirmationStale`,
`Busy`, `ReloadRequired`, and `ManualRepairRequired`. Its result contains the
outcome, transaction ID only when authenticated, inferred durable state, exact
target length/hash/revision, three old archive proof states, and active
Primary/Previous/Temp states. All fields validate as one closed matrix;
default/unknown/reflection-corrupted combinations throw. Only `DiskPrepared`
contains `ProfileResetDiskPreparedProofV1`, binding normalized-root identity,
transaction ID, exact canonical marker bytes/hash, exact target bytes/hash and
revision `0`, complete archive fingerprints, and active target-only reprobe.
It is process-local, immutable/defensively copied, one-consumer, and grants no
ordinary save, memory, receipt, UI, or barrier-removal authority.

C1 always releases the OS handle after returning/throwing. The durable barrier,
not a held handle, blocks other sessions afterward. Busy waits at most five
seconds and mutates nothing. Recoverable filesystem failures map only to the
typed fail-closed outcomes above; programmer/protocol/fatal exceptions escape
after ownership cleanup and never produce a proof.

## Requirements

- **REQ-M5D7QC1-001:** Validate a fresh, exact, single-use confirmation identity
  under the cross-process root lease before any archive or barrier mutation.
- **REQ-M5D7QC1-002:** Encode, publish and decode the exact canonical hashed v1
  barrier before moving any active leaf; malformed/unreadable barrier always
  blocks ordinary recovery.
- **REQ-M5D7QC1-003:** Archive every marker-declared old leaf byte-for-byte to
  its fixed manual-only destination with strict crash-idempotent identity rules.
- **REQ-M5D7QC1-004:** After complete archive proof, create only the exact
  approved default r0 through the existing save core and the narrow reset-only
  held-root capability.
- **REQ-M5D7QC1-005:** Preserve interrupted new-default bytes safely and infer
  resume state from authenticated identities without confusing identical old
  default bytes with the reset product.
- **REQ-M5D7QC1-006:** Return a closed disk-only outcome; `DiskPrepared` retains
  the barrier and grants no memory cutover, success receipt, UI, scene, or
  ordinary persistence authority.
- **REQ-M5D7QC1-007:** Restrict all paths, names, containment, collisions,
  retries and mutations to this contract and fail closed on uncertainty.

## Acceptance criteria

- **AC-M5D7QC1-001:** Exact/mismatching presence, classification, length, hash,
  revision and generation matrices prove fresh confirmation and zero mutation
  on stale/unreadable input.
- **AC-M5D7QC1-002:** Golden bytes plus single-field/order/type/case/number/hash,
  oversize, directory, access and unknown-version mutations prove canonical
  marker rejection and that ordinary load/save never reaches Previous.
- **AC-M5D7QC1-003:** Fault/crash injection before and after every marker write,
  flush, close, move, reopen and every old-leaf move/reopen proves the strict
  source/destination matrix, invalid-byte preservation, and no old resurrection.
- **AC-M5D7QC1-004:** Missing/valid/invalid/unsupported/unreadable combinations
  for all three leaves, including exact-default old Primary plus present
  Previous/Temp, prove complete archive before target creation.
- **AC-M5D7QC1-005:** Existing save-core first-save tests plus reset capability
  tests prove exact planner r0, no Previous, barrier authentication, rejection
  by every ordinary held/public writer, and no duplicate persistence algorithm.
- **AC-M5D7QC1-006:** Crashes and faults around default temp/write/flush/commit/
  postcheck preserve temp or Primary bytes, perform no same-session uncertain
  retry, and converge only to `DiskPrepared` or a fail-closed repair outcome.
- **AC-M5D7QC1-007:** Identical old-default and target bytes, repeated identical
  interrupted temps, hash-name collision, both/neither old copies, corrupt new
  Primary, and unreadable archive adversaries cannot advance by byte ambiguity.
- **AC-M5D7QC1-008:** Real separate-process contention at every durable state
  proves five-second non-mutating Busy behavior and restart inference after the
  crashed handle releases.
- **AC-M5D7QC1-009:** Output/proof mutation and reflection tests prove the closed
  matrix and that no barrier removal, memory apply, receipt, scene/UI, network,
  clock-based inference, archive load, restore, cleanup, or unrelated path is
  reachable.

Traceability: `001 -> AC-001`; `002 -> AC-002/003`; `003 -> AC-003/004`;
`004 -> AC-004/005/006`; `005 -> AC-006/007`; `006 -> AC-008/009`;
`007 -> AC-002/003/007/008/009`. All AC IDs above carry the
`AC-M5D7QC1-` prefix.

## Implementation allowlist

After Astra changes this contract to Approved, implementation may:

- add one engine-free reset disk transaction source and `.meta` beneath
  `Assets/AcadeGameMaker/Runtime/Profile/`;
- modify `ProfileNewGameResetServiceV1.cs` only for the validated marker model,
  decoder and reset coordinator integration;
- modify `ProfileAtomicSaveServiceV1.cs` only for the internal reset-only
  held-root first-save entry that delegates to its existing core;
- add focused EditMode tests and `.meta` beneath
  `Assets/AcadeGameMaker/Tests/EditMode/Profile/`;
- add only `qa/fixtures/ProfileResetRootLockHolder.ps1` as separate-process
  fixture. A test starts Windows PowerShell with fixed arguments and a unique
  test-owned temporary root, holds the exact lock with FileShare.None, emits
  ready/release signals outside the profile directory, and uses bounded
  lifetime. Cleanup targets only its exact child PID and verified temporary
  directory. The fixture rejects non-temporary roots and accepts no production
  profile path;
- add C1 implementation/verification evidence and one minimal README link.

No asmdef/friend change, Input.Unity change, existing ordinary launch behavior,
scene, prefab, input asset, ProjectSettings, Packages, costume/media storage, or
other save slot is authorized.

## Stop and rollback

Stop for Astra if implementation requires barrier removal, memory/input apply,
receipt/UI/scene authority, a second save algorithm, archive overwrite, copy-
delete fallback for old leaves, relaxed ordinary barrier checks, symlink/reparse
following, new filenames, ABI widening, or an unspecified separate-process
fixture path. An uncertain commit is never retried in the same session.

Rollback removes only C1-added files/docs and reverts only C1-owned hunks in
the two allowlisted existing sources. It never deletes transaction archives,
barriers, active profile bytes, unrelated dirty work, or historical evidence.

## Delivery gate

Luna pre-review cites every `AC-M5D7QC1-*`; Astra alone may set Approved. Terra
then implements with `REQ-M5D7QC1-*` citations. Luna independently verifies
the exact source/evidence and Astra alone integrates. Sol drafted the contract;
Luna independently reviewed AC-001..009 and Astra's two-pass/collision/private-
capability amendments, finding no remaining P0/P1. Astra approved this exact
bounded disk-only scope on 2026-09-28; approval does not imply implementation,
verification, UI connection or full parent Q-C completion.

Astra accepted the frozen disk-only implementation after R12 212/212,
R13 actual child-process 51/51 and R14 Profile regression 175/175, all with
zero failed/skipped/inconclusive, and Luna P0=0/P1=0 independent review.
See [continuation evidence](../../verification/2026-09-28-vd09-m5d7q-c1-continuation.md)
and [final Luna review](../../verification/2026-09-28-vd09-m5d7q-c1-final-luna-review.md).
R13's premature outer-wrapper timeout is retained separately from its successful
actual Unity execution. Reparse containment is source-reviewed; no dedicated
Windows reparse fixture execution is claimed. The Approved-at-execution
contract SHA was `D42276E3F07B5C66AC9390D0C25DB37F77F3847346A14D8FD5419FEEBF6F6478`.
No memory apply, barrier removal, live UI or scene authority is added by this status.
