# M5D7Q-C production launch root-lease span — 부분 검증

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md` — Approved, full implementation pending
- Implementation: Terra (`gpt-5.6-terra`)
- Counter-review: Sol (`gpt-5.6-sol`)
- Independent postreview: Luna (`gpt-5.6-luna`), bounded source scope P0/P1/P2=0
- Integration: Astra (`gpt-6-astra`), bounded lease-span slice accepted

## Scope and traceability

`REQ-M5D7QC-006/007`, partial `AC-M5D7QC-004/007`: production `ProfileLaunchPreparationCoordinatorV1.Prepare` owns one OS root lease through observation, selection, preservation, binding apply/repair, optional save, prepared-token construction and failure cleanup. The production profile port uses held APIs with the actual live, normalized-root lease; wrong-root and disposed leases are rejected. Standalone public Observe/Save remain independently guarded. Internal injected overloads are fault-test seams. Lock metadata is never deleted on release.

This closes the ordinary launch observation/preservation/save interleaving gap recorded in the earlier foundation evidence. No reset marker publication, old-profile archive, reset crash resume, new-default commit, reset memory cutover, receipt, confirmation UI or scene effect is implemented. A real separate-process contention test and the full Q-C crash matrix remain pending; same-process OS-handle contention is not that evidence.

## Final host-context execution

| Run | Result | XML SHA-256 | Log SHA-256 |
|---|---:|---|---|
| `artifacts/m5d7qc-launch-lease-r3.xml` / `.log` | PlayMode 36/36; failed/skipped/inconclusive 0 | `A4BE38DABD89949E2E65A035D71B160438FBA43326DD3D96959FD4CB724AE26A` | `CF09C08C70B360B819719CE174D6D68C1992F4F8E23424F0222B0573C6D1DDC8` |
| `artifacts/m5d7qc-profile-regression-r3.xml` / `.log` | EditMode 175/175; failed/skipped/inconclusive 0 | `4EF4FC00A293A368C19DB1E5B6A0A35C88B8C1FB7ADADEEAB1A2C7FDF33D84BE` | `33E620D673ED3375528D0AAA653543D20EE8975E8B5B24D490278152508DE503` |

R1 did not reach test execution because the new PlayMode test missed a closing parenthesis. R2 did not reach tests because the internal Profile lease was not visible to the exact PlayMode test assembly. Terra fixed the syntax and added a narrow friend-assembly entry without making the lease public; both failed logs remain preserved. Neither is counted as a test pass. R3 produced the XML above and exited naturally; no Unity process remained.

Luna independently reviewed the final source and 36/36 PlayMode XML and found P0/P1/P2=0 in this bounded slice. The later 175/175 Profile regression was verified by Astra from its XML. Q-C is **not Verified** as a whole.

## Source fingerprints

| Source | SHA-256 |
|---|---|
| `Runtime/Profile/ProfileNewGameResetServiceV1.cs` | `2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098` |
| `Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `E24BD9DBDC9AF43C311F47508A1516EB3D638C6E5AD4605808FC90427B959498` |
| `Runtime/Profile/ProfileLaunchObservationAdapterV1.cs` | `1AF63C8C9691C9AB1980B18AAA762A77CCBFEB09FBA0C9F9704874750DF5EF0A` |
| `Runtime/Profile/AssemblyInfo.cs` | `9CAC7929FDEEA6CB76AB092CE656D06B78E90E6FB03672290F3D1ACAC07CBF30` |
| `Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs` | `8FA81A8E0082F5E05EAB8200BB28B020F038DAC66BBC33B6026E3A4906E15C00` |
| `Tests/PlayMode/InputUnity/ProfileLaunchPreparationCoordinatorV1Tests.cs` | `11A6D2951DB5A3C8F22C53577A7282E84A23C07B8A8E4F559DFCE9405E537B7B` |
| `Tests/EditMode/Profile/ProfileResetBarrierInterceptionV1Tests.cs` | `38AE4DA7D8786D3A4697A1688CF107A4A71E61573910F8A4099A55C2C3B6C3CA` |

Paths in this table are beneath `Assets/AcadeGameMaker/`. Astra approved and integrated the bounded contract; Sol counter-reviewed ownership/deadlock risks; Terra implemented and corrected test fixtures; Luna independently reviewed implementation and partial AC evidence. No non-GPT model calls were made.
