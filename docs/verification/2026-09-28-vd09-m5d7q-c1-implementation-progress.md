# M5D7Q-C1 — disk transaction implementation progress

Date: 2026-09-28. Status: partially implemented and focused-tested; NOT Verified
and NOT integrated into the live menu or launch path.

## Outcome

Terra added the engine-free reset transaction, canonical hashed barrier,
fresh one-shot confirmation identity, byte-preserving archive, destination-first
restart inference, interrupted-file preservation and exact default r0 first
save through the existing atomic save core. DiskPrepared retains the barrier
and grants no reset-success, memory, scene or ordinary save authority.

The new service is internal and has no production caller. No real profile was
reset, no archive was restored or erased, and no license/session was changed.
`ProfileNewGameResetServiceV1.cs` remains at its accepted foundation hash
`2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098`.

## Final tested source fingerprints

| File | SHA-256 |
|---|---|
| Runtime/Profile/ProfileResetDiskTransactionV1.cs | `4485DF37480B6032FF64BC7B47707F1CB08F9621F324F06B1DB86AAFAE29BC5D` |
| Runtime/Profile/ProfileAtomicSaveServiceV1.cs | `E954333B3F04A38E3ABEAE98BA17333745DDB93B1062DEA53518AFB92DAD2F95` |
| Tests/EditMode/Profile/ProfileResetDiskTransactionV1Tests.cs | `AAF9C05B61F612A48EBC72CEF990D576AB24C1AA5785CBFBBB76D793EF21C862` |
| qa/fixtures/ProfileResetRootLockHolder.ps1 | `F896F707A1C53074E7E324B382E1FCB4C0E93EEDD7CFADF573126E0B59F632E5` |

The first three paths are beneath `Assets/AcadeGameMaker/`. Added sources/tests
also have Unity metadata. There was no package, scene, prefab, ordinary root-
lease behavior, input or costume implementation change in the C1 delivery.

## Actual execution evidence

Main ran the pinned Unity 6000.6.0f1 through the existing QA wrapper in the
normal Windows licensing session, sequentially with the source frozen:

- `artifacts/c1-r7-focused.xml`: **19/19 passed**, failed/skipped/inconclusive 0.
- `artifacts/c1-r7-profile-regression.xml`: **175/175 passed**, failed/skipped/
  inconclusive 0. Existing profile encoding/decoding/save/recovery/guard tests.
- Matching `.log` files and wrapper exit 0 support both runs.

Focused XML SHA-256:
`F06517FD3B973617D140FB5F3761C35C49470DFA6062F91771995658D4F13176`.
Final profile-regression XML SHA-256:
`3E5E3FA0CDEA3762724C6913B085D2FAA87779806C731627673A7A2DA6225304`.
Approved-at-execution contract SHA-256 (unchanged):
`D42276E3F07B5C66AC9390D0C25DB37F77F3847346A14D8FD5419FEEBF6F6478`.

Earlier draft compilation failed (R2 four CS0819 errors, R3 one remaining
CS0819); those logs are retained and have no acceptance XML. Subsequent R4
11/11, R5 14/14 and R6 18/18 focused passes were intermediate revisions, not
the final tested source. R5 profile regression also passed 175/175.

The attempted unfiltered R4 EditMode run did not produce XML; logs stopped
updating after Hub capture-test/import messages. Main validated and stopped
only its QA Editor PID 46064 and confirmed child import-worker PIDs 34552 and
46836. The log is retained. This cancelled run is NOT full-regression PASS
and does not prove a root cause or a new product defect.

## AC disposition — partial, not whole-criterion closure

- `AC-M5D7QC1-001`: partial evidence for stale/single-use identities and
  held-root Busy followed by reuse of the unconsumed identity.
- `AC-M5D7QC1-002/004`: partial evidence for malformed markers, readable invalid
  byte preservation, exact default generation and prepared restart.
- `AC-M5D7QC1-005`: partial execution/static evidence for delegation to the
  existing first-save core and ordinary barrier guards; broader capability
  provenance/adversarial matrix remains open.
- `AC-M5D7QC1-006/007`: partial evidence for exact Primary plus interrupted Temp
  preservation and destination-first classification; full fault/crash/collision
  and corrupt-new-Primary matrices remain open.
- `AC-M5D7QC1-009`: partial evidence for default/malformed output, scalar/root/
  role mutations and coupled proof/archive mutation; broader matrix open.
- `AC-M5D7QC1-003/008`: NOT closed. Full stage-specific fault/crash tests and
  actual separate-process contention/restart tests still need implementation.
  A fixture file and same-process Busy test do not satisfy AC-008.

Luna's [R7 independent partial review](./2026-09-28-vd09-m5d7q-c1-luna-partial-review.md)
found no remaining P0/P1 within the focused scope. Astra retains this as a
tested WIP implementation, not full C1 acceptance or parent Q-C completion.

## GPT participation ledger

- Sol (`gpt-5.6-sol`): contract draft and bounded counter-review.
- Terra (`gpt-5.6-terra`): implementation and R1–R7 corrections/test fixtures.
- Luna (`gpt-5.6-luna`): independent pre/post review and actual final XML review;
  earlier wrong-executable/path/accessibility claims were withdrawn explicitly.
- Astra: contract approval, corrective direction, host test execution, evidence
  audit and final WIP disposition. Implementation was not self-accepted.

No Ollama, Spark, recurring job, separately billed API or Pro-mode execution.

## Next work

Complete injectable IO/fault/crash and real child-process test matrices before
full C1 acceptance. Then separately scope memory/settings/input cutover,
barrier removal and one-shot success receipt under ownership; confirmation UI
and destination connection remain later Approved work. No new user product
decision is required for the already-approved manual-only archival policy.
