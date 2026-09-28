# M5D7Q-C 잠금·초기화 표식 차단 기반 — 부분 검증

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md` (`Approved`, SHA-256 `BC18B92B5E1355A55AE9E49FEB85326F277E0CC842D444627985F0796C7404FA`)
- Implementation: Terra (`gpt-5.6-terra`)
- Independent postreview: Luna (`gpt-5.6-luna`), P0=0, P1=0, P2=1
- Integration: Astra (`gpt-6-astra`), **bounded foundation accepted; Q-C remains incomplete**

## Accepted boundary

`REQ-M5D7QC-006/007`, partial `AC-M5D7QC-004/007`: ordinary public profile observation, save and input-recovery save acquire an OS-exclusive `profile.operation.lock` and reject any present or unreadable `profile.reset.json` before ordinary candidate selection or write. A directory at the barrier name is terminal. Lock acquisition times out to Busy without profile mutation. The three-argument injected save and injected observation overloads remain internal fault-test seams, not production entry points.

No confirmed-New-Game reset, archive movement, marker publication, crash resume, default first-save, in-memory cutover, receipt, confirmation UI or destination is implemented. The current code does not publish the barrier. The normal launch coordinator still needs a single ownership span across observation, preservation and save before a reset coordinator can safely operate concurrently. A same-process lock test is not proof of separate-process contention.

Follow-up: [production launch lease-span evidence](./2026-09-28-vd09-m5d7q-c-launch-lease-evidence.md) closes that coordinator ownership gap. The fingerprints and counts below preserve this earlier slice's exact source state; they are not the latest source fingerprints.

| Source | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs` | `D7D2D14B4DB2E220AAFCB604205EB31A120F792E2EACD5D5E1A0D6B722D5AFC8` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `8493BB047BB89144B4FE1BE89FC7C92EA7678B175D0A776E8F5C91359C53B640` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchObservationAdapterV1.cs` | `FBA100CF96A0B96DA24E561092904129D42C815F097C7C3E975D634BEED7FEB2` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileResetBarrierInterceptionV1Tests.cs` | `CE8AB207F6668BCA6A1F01DBE96F4BAB9F743735CC8CF0EA9692AB211294F56B` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs` | `DACEDD7AEC5E5D71746A10DC7CAC92E229513829E6AB4645A4EEB80924BF51DB` |

## Execution and fault history

The initial sandbox Unity run never reached test execution: the Licensing Client timed out/reconnected, leaving no XML. Its task-owned Unity PID 22356 was confirmed and stopped after the log ceased progressing; `Unity-M5D7QC-Barrier.log` is preserved (SHA-256 `FB5E52C0A3F0B00FB8A26EB49A623AB8964E98777CEFC25077E868E53C6F3C0A`). The first two host attempts included `-quit`, so they exited after import/compilation without XML; neither is a test pass. The first proper host run produced `4 total / 1 passed / 3 failed`: the three failures came from an invalid test-only seed `123` before the barrier assertions. Terra corrected only that fixture. The first broader regression then produced `173 total / 172 passed / 1 failed` because the historical exact-leaf test did not allow the newly approved lock metadata. The M5D7D contract was narrowly amended and its assertion updated to preserve exact active data-leaf checking.

| Final host-context run | Result | XML SHA-256 | Log SHA-256 |
|---|---:|---|---|
| `artifacts/m5d7qc-barrier-host-r4.xml` / `.log` | EditMode 4/4, zero failed/skipped/inconclusive | `4DD8A46DBF06F19268E1ADB0277F6EF2404201002F2A57631EDDCC9E815F770E` | `C0E517A0CB254C85B1C63D19608E08C1B05154D5E26159914DFF2C48E787E75D` |
| `artifacts/m5d7qc-profile-regression-r2.xml` / `.log` | EditMode 173/173, zero failed/skipped/inconclusive | `3611B7497B19A3CF2214FA1FFC484764F0278CC54D9F12016BDD75B74AD6C678` | `6D3314634C93574887EDC9BAB9A473AF5D589EE7AA4B09CD10F85ABEE523EAEA` |
| `artifacts/m5d7qc-launch-coordinator-r1.xml` / `.log` | PlayMode 33/33, zero failed/skipped/inconclusive | `B07E7D41164D89374B24944B13FD496E6898216D4B8CA988A7FDD3AEE43DE8C0` | `569F27634579E7661DF0A76482DD5DD933026397F054CF02AD74AF8AB88FB7BE` |

Luna's independent final postreview found no P0/P1 in this bounded foundation. One P2 remains: cross-process contention is not yet tested with an actual second process. `AC-M5D7QC-002/003/005/006` and the remaining transaction/crash portions of `AC-M5D7QC-004/007` are **not verified**.

## GPT participation

| Owner | Work | Acceptance boundary |
|---|---|---|
| Astra | user decision, ADR-0035, Approved Q-C/VD-09 and M5D7D amendments, host execution and partial integration | No full reset acceptance |
| Sol | bounded marker/archive/crash-resume design | Reviewed by Astra and Luna |
| Terra | lock/barrier foundation and deterministic tests | Independently reviewed by Luna |
| Luna | adversarial pre/post review and AC digest | P0=0, P1=0; cross-process P2 open |
