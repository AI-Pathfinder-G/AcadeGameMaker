# M5D7Q-C1 disk transaction — pre-implementation gate

Date: 2026-09-28. Scope: Approved C1 disk half only; no implementation or AC pass claimed.

## Actual ownership

- Sol (`gpt-5.6-sol`): bounded contract draft and counter-review.
- Luna (`gpt-5.6-luna`): independent pre-review and re-review.
- Astra: resolved design findings, approved bounded contract, allocated Terra.
- Terra (`gpt-5.6-terra`): implementation assigned after approval, not accepted yet.

No Ollama/Spark/paid API call, retry, schedule or Pro-mode execution occurred.

## Findings and disposition

Luna reviewed `AC-M5D7QC1-001..009`. Initial review found P0=0,
P1=3 and P2=2. Astra resolved them before approval:

1. `AC-003/004/007`: resume first proves old destination completeness; while
   incomplete, strict source/destination rules apply. Only complete old archive
   permits new active Primary/Temp interpretation. New Primary uses the same
   pathname as old Primary; demanding that pathname be absent at DiskPrepared
   would reject every successful reset. Luna accepted this corrected two-pass
   rule. Uncooperative external reinsertion is explicitly outside the model.
2. `AC-006/007`: corrupt new Primary is preserved under a separate fixed
   primary directory before rebuilding. Temp and Primary collisions, even
   identical bytes, block without delete/overwrite/suffix guessing.
3. `AC-008`: exact fixture path is `qa/fixtures/ProfileResetRootLockHolder.ps1`,
   test-only temporary roots and exact child-PID cleanup, bounded lifetime.
4. `AC-001`: confirmation generation means length/hash of actual bytes plus
   decoding those same bytes; private immutable root-bound identity is consumed
   only after root ownership.
5. `AC-005/009`: reset capability binds live lease, root, marker and target;
   each use revalidates disk. Reflection tests cover malformed invariants,
   not an unrestricted CLR attacker.

Sol independently recommended collision fail-closed, two-pass archive proof,
atomic identity consumption after ownership and acceptance of CommittedFirst
only. Luna re-read the amended document and reported no remaining P0/P1.
Astra set Approved after that gate. AC IDs in this record are review coverage,
not executed-test PASS statuses.
Approved contract SHA-256 at implementation allocation:
`D42276E3F07B5C66AC9390D0C25DB37F77F3847346A14D8FD5419FEEBF6F6478`.

## Unity environment readiness

Read-only host-context `Invoke-UnityQa.ps1 -PreflightOnly` exited 0:
Editor 6000.6.0f1 matches the project, embedded/Hub clients 1.18.3 are signed
and aligned, entitlement is nonempty, and no competing project Editor exists.
No Unity test was launched by this preflight; no test pass is inferred.

## Remaining gate

Terra code/tests, Luna independent source review, actual Unity XML and separate-
process evidence are required before acceptance. C1 deliberately retains the
barrier at DiskPrepared; memory cutover, barrier removal, confirmation UI and
scene transition remain outside this delivery.
