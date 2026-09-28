# C2 implementation progress — Approved, not verified

Date: 2026-09-28. Trace: REQ-M5D7QC2-001..007 / AC-M5D7QC2-001..010.

The C1 disk-only predecessor is Verified at reset-source SHA
`90EFBE7A71C20241BD9CF792DE42BFD1B2142BCCB7AD2A146215D62ECC194743`,
after R12 non-process 212/212, R13 actual-process 51/51 and R14 Profile
regression 175/175, with failed/skipped/inconclusive 0 and Luna independent
P0/P1=0. This does not authorize live menu/scene integration.

Astra approved the [C2 bounded contract](../specs/work-contracts/2026-09-28-vd09-m5d7q-c2-memory-cutover.md)
at SHA `5A0CFF6BAF5F8C473D49382F2BD98C4EE27588516409507EC8C33E541B231251`.
Sol and Luna identified proof-CAS ordering, deterministic fault seams, terminal
failure safety and original session-root identity gaps; Astra amended them and
Luna found the contract P1s closed. These were technical clarifications within
the user-approved workflow, not additional user product decisions.

## Actual GPT participation and non-overlapping ownership

- Astra: contract approval, exact API allocation, final acceptance authority,
  source audit and all host Unity execution.
- Sol (`gpt-5.6-sol`): bounded source feasibility and architectural counter-review.
- Terra (`gpt-5.6-terra`), Profile half: only C2 additions in
  ProfileResetDiskTransactionV1.cs and new Profile authority tests/evidence.
- Terra (`gpt-5.6-terra`), Unity half: new C2 coordinator, bounded InputRouter
  terminal lane, DesktopProfileLaunchAdapter root/router witnesses and current
  session/receipt, new PlayMode tests/evidence. No overlap with Profile worker.
- Luna (`gpt-5.6-luna`): independent pre-review; post-implementation and actual
  execution verification still required.

No retired model, automation, Pro-mode claim or paid API was used. Workers do
not launch Unity or accept their own work. Astra owns integration after distinct
independent verification. No C2 test execution/PASS is recorded yet.

## Boundary

C2 is synthetic-only. Confirmation/request ownership, live menu wiring,
interactive destination, scene/media/gameplay, OS settings/audio effects,
ordinary save and archive restore/cleanup are not authorized. A completed C2
receipt will remain UIOnlyBlocked. Existing launch/hub receipts remain history;
the new current-session cell owns the bounded reset state only.

## Approved pre-lease containment clarification

Before implementation integration, Astra added a read-only Profile-owned
pre-lease root/ancestor/lock-path check. The existing lock Acquire can create a
lock before held checks; a root changed to a reparse point must be rejected
before that mutation. Original session-root equality alone is not a containment
proof. No product authority or separate user decision is introduced.
The resulting Approved C2 contract SHA is
`C891C7DA3A4F1B0E4A843C06B069FE2A26A5B7BE1598DCD26E2D124B7205E625`;
the earlier approval SHA above remains historical evidence of initial dispatch.

Luna's bounded pre-review found no new P1 in that clarification. Unlike C1's
explicitly static reparse audit, C2 AC-003/005 requires actual Windows hostile
path cases. Astra explicitly approved the named test-only NTFS junction helper,
with link/target confinement and nonrecursive link-first cleanup. Current C2
Approved SHA is
`BF9F0CB18D6108B2683613075ABFA73C5AD3F82F8A9FDD7ABF17EF8900EAEA73`.
This fixture has not yet executed; no actual reparse PASS is claimed.

## Source-review correction loop — not acceptance

Luna's first Unity source review identified three P1s: result getters did not
reject reflected corruption, the receipt lacked detached exact final evidence,
and failure containment could dereference a substituted foreign router.
Astra additionally found incomplete eligibility, lifecycle/disposal ownership,
terminal forensic-frame preservation, fault seams and post-publication ordering.
These remain implementation corrections, not new product choices. A Terra
hardening task owns only the existing Unity-half allowlist and focused tests.

Astra's Profile audit caught a junction buffer offset/size mismatch before any
execution, an archive-named test that actually mutated Primary, and a deleted
marker retry that performed marker I/O before checking consumed delete authority.
Terra corrected these and expanded the Profile matrix. A FileShare.None marker
case exercises prevalidation failure, not failure inside File.Delete; a distinct
read-permitted/delete-denied sharing case is required for that direct-call claim.
No source-only finding is a Unity PASS. Both halves must be frozen before any
actual Editor compilation/test run; Luna and Astra acceptance remain pending.

## Actual focused execution — Profile half only

- R1 (`artifacts/c2-r1-profile.log`): compilation rejected the missing Profile
  namespace import in InputRouter. Actual Editor logged exit 1; no XML/tests.
  Terra added the missing import. This was not a license failure or a PASS.
- R2 (`artifacts/c2-r2-profile.xml`/`.log`): 29/30 passed, 1 failed, zero
  skipped/inconclusive; actual Editor logged exit 2. The failure proved that
  opening a read-sharing handle before Remove still prevents the production
  exclusive marker reread, so it had not reached File.Delete. Production reads
  were not weakened. Terra moved the hostile handle opening to the existing
  BeforeBarrierDelete checkpoint after validation and attempt latching.
- R3 (`artifacts/c2-r3-profile.xml`/`.log`): **30/30 passed**, zero
  failed/skipped/inconclusive, wrapper exit 0 and actual Editor exit 0.
  XML SHA-256 `C4341E30EE48D7D3EF39A39860A394FF2C81DB0654E214198EC60BD21AC45212`.
  Runtime Profile SHA `59BD9EB9665AF6960FF032B489CDF74602F4D743024B3A29C112CB75A48D2755`;
  focused Profile tests SHA `512C0F5E3A626B5D84FE8A56743622105393A15AF51EE9C635B229F210644B5F`;
  native fixture SHA `543B545439985313BB474BECA97F817D86D7FE87759F3F851256815D9F31140E`.
  AC-M5D7QC2-001/005/006 Profile evidence and the actual reparse portion of
  AC-003 passed. This does not verify Unity memory/input ownership or C2 overall.

An auxiliary .NET 10 compilation and direct reflection invocation exercised
only 15 parameterized Profile cases; the initial harness did not enumerate
parameterless rows. It is diagnostic evidence only, not a 30-case run or Unity
acceptance. R3 above is the authoritative complete focused execution.

## Further bounded allocation

Astra found the first Unity hardening still lacked the full default/root/final
evidence row, final-result-before-publication order and actual lifecycle fault
matrix. C2 remains Approved/unverified. The two Terra workers are now split
without source overlap: data/coordinator plus new data tests; router/adapter,
actual dynamic PlayMode fixture and Profile final-evidence capability. Astra
approved a private Profile-minted detached final-disk proof within the original
allowlist. No arbitrary strings/booleans may create a completed receipt. No
live menu, input map enable, scene or product destination is added.

## Actual Unity memory/input focused execution — R4/R5

- R4 (`artifacts/c2-r4-playmode.log`): actual Editor compilation failed with
  CS0246 in the new happy-path fixture (missing generated-actions namespace).
  Actual Editor exit 1; no test XML. Terra corrected the fixture import only.
- R5 (`artifacts/c2-r5-playmode.xml`/`.log`): 8/8 passed, failed/skipped 0,
  wrapper exit 0 and actual Editor logged test-run exit 0. XML SHA-256
  `D1DFC3A4B2E94A278E07E83600EB7F8CF9A53C0C604D7D5A1861331875736181`.
  Router SHA `6B3FF2C19408A1E2FE7B07A154D1767934B6DAEA72B945EF149F872260634CB6`;
  Adapter SHA `4FDF71A85D7D5F2DF505EDDA7756DD103A543056848296DDFB8C0CA154A478BE`;
  cutover data SHA `FAD0E29D9AACDC6BD438DB06B38ED3B056C5FFCE816CB477EB3A71D067F6293B`;
  happy fixture SHA `68ED02DD8E4C784C6A12E409F9966BAFDA69C8D226EDCFCDF79476379A39BF29`.
  This supplies one actual launch→C1→C2 success path and seven bounded shape
  checks for AC-M5D7QC2-001/003/004/006/008/009, not complete coverage of those
  ACs. No acceptance or production menu connection is implied.

Astra allocated the next corrections under the same Approved C2 contract:
retain private semantic-history evidence; single-owner candidate handoff and
independent teardown attempts; isolate actual lock Busy from injected faults;
close eligible pre-lease filesystem failures; and revalidate held authority
immediately before terminal entry. Terra implementation and Luna independent
lifecycle review are distinct. C2 remains unverified pending these changes,
the adversarial matrix, frozen-source review and regressions.

## Lifecycle counter-review and further pending work

Luna independently reported original-candidate substitution, coordinated
cohort-witness substitution, and failure-routing through mutable router fields.
Sol supplied a bounded counter-design: private Start-authenticated cohort
registration keyed to both original adapter/router, with shared terminal truth
and immutable original action identities. Duplicated local fields must remain
diagnostics, not authority. These are corrections to REQ-M5D7QC2-003/004/007,
not permission for a general router hot-swap or interactive destination.

Astra requires the candidate/history fault rows to run before the larger
cohort/shared-terminal correction is dispatched. Main source review also found
that candidate disposal was made witness-dependent before the witness existed
on pre-adoption faults. The two Terra workers are coordinating registration
immediately after real candidate creation, before validation/apply/checkpoint
failures. No implementation-only report closes this finding.

Further actual tests are being authored for held-root Busy, post-lease injected
Busy, unsafe lock path, and before/after owned operation/disposal exceptions.
They are pending execution. The final Profile-proof changes after R3 still
require a fresh complete Profile run. Historical R3/R5 PASS is not attributed
to these changing sources.

Luna separately reviewed the frozen Profile final-proof path without running
Unity and reported no additional bounded P0/P1 findings. Before/after hashes
matched: Profile source `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`,
authority tests `D39DA6EE7A28CBC409A9F2FF07B3CD3992FB2FC2B84EA03160FFAD971A45FD89`,
native junction fixture `543B545439985313BB474BECA97F817D86D7FE87759F3F851256815D9F31140E`.
This covers the mint/lease/real-probe/root/correlation/detachment review portion
of AC-M5D7QC2-001/005/006/009 only; fresh execution is still required.

## Expanded focused execution — R6/R7/R8

- R6 PlayMode: 19/19 passed, failed/skipped/inconclusive 0. Actual Editor PID
  43100 completed normally and logged exit 0. The wrapper returned tool exit 1
  early for absent XML; the same Editor continued, with no relaunch or kill.
  Final XML `artifacts/c2-r6-playmode.xml` SHA-256
  `F569B550BD033641C783BEDEBABFBFA1E43162C866C1CF83BF50C2BA76F6512C`.
  Router SHA `58A8481213DABFE2862100B8A93D4577C58E7F65A474B16526296674FBE23C48`;
  Adapter SHA `F75B27153B642916E08D3BCD11F76F4F6C037A2C2DCE4E513D454C7AEE5F65C3`;
  data SHA `D3A6FC5C723AB201E7CB1969BD57C50F373E44E4AA7323FD3DFD4166B2D6B533`;
  operation fixture SHA `B4F580B61008BFF12A1031EFC159D9EB08C54EE9EAC3185407CA5F374F80E228`;
  data fixture SHA `C2F30BE206ECB2024BA2F5F4EAE233FBF843BBF8E9231019C5D390C42E6A7D92`.
  Added actual Busy, injected post-lease Busy, unsafe lock directory, staged
  disposal ordinary/I/O before/after, and old operation before/after rows.
  AC-M5D7QC2-001/002/003/004/008 partial evidence only. Direct containment
  Dispose calls still bypass the operation-control counters; attempt flags
  plus wrapped call counters do not prove the complete disposal matrix.
- R7 Profile: incorrect namespace filter selected zero tests. This is not
  verification, despite the runner's zero-failure result.
- R8 Profile: corrected `AcadeGameMaker.Profile.Tests` filter ran 32/32,
  failed/skipped/inconclusive 0, wrapper and actual Editor exit 0. This is fresh
  execution of the frozen Profile final-proof source/test hashes above,
  including native junction rows. Artifact: `artifacts/c2-r8-profile.xml`.
  XML SHA-256 `CC19E4BAF42DA500F5AA4DB0244937CBEB86385059AFB773E11D4DE4902FB52D`.

Astra dispatched the remaining original-cohort/shared-terminal correction to
a fresh Terra worker and the correlated live-session getter correction to the
data worker, with non-overlapping source ownership. Immutable retained current
cells must reject failed/foreign pairs; completed receipts remain detached.
The six coordinated-reflection/foreign-safety rows and final regressions remain
pending. C2 is still not accepted and no real menu is connected.

## Original ownership clarification — Approved before remaining integration

Astra recorded the existing REQ-M5D7QC2-003/004/007 semantics explicitly in
the Approved C2 contract: staged identity does not transfer disposal ownership;
private one-attempt close authority cannot be inferred from mutable diagnostic
flags; consistent completed fixed ticks are quiescent; corrupt paths contain
only original owners; retained cells revalidate live pairs and receipts remain
detached. Current contract SHA-256
`A23F9B8E978BE24A8BC1E89EDB48ACF9BA3CFE81FC23CB2EF326B396A8CC7A38`.
This is not a new live-menu/scene/save authorization and is not retroactive
acceptance of R6 changing sources. Final tests/review must use the new frozen
source against this clarified contract.

## Shared ownership smoke — R9 and independent rejection gate

R9 (`artifacts/c2-r9-smoke.xml`/`.log`) executed six selected actual rows:
the genuine launch/C1/C2 happy path, retained-cell rejection after an AfterCell
fault, and four staged-disposal ordinary/I/O before/after rows. All 6 passed,
failed/skipped/inconclusive 0; actual Editor logged exit 0. Frozen sources are
the five hashes in Luna's [cohort review](./2026-09-28-vd09-m5d7q-c2-luna-cohort-review.md).
The other newly authored cohort/data rows were not selected by this smoke run.

Luna independently rejected the same frozen cohort source on two P1s:
teardown lambdas still read mutable reset action fields, and private disposal
claims used non-atomic booleans. Astra allocated those runtime corrections to
the cohort Terra worker. The other Terra worker now owns only the existing
PlayMode fixture to correct clean-protocol expectations, use real sibling
cohorts, and implement the remaining post-entry/substitution/resurrection and
atomic replay rows. Its data source and new data fixture remain frozen.
No C2 acceptance is recorded; R9 success does not offset independent P1s.

## R10 freeze and remaining acceptance work

Astra launched focused PlayMode R10 after both Terra owners froze their files.
REQ-M5D7QC2-003/004 corrections capture original operation targets and use
private Interlocked close claims. Luna's fresh read-only review finds the two
prior P1s closed at source level; this is not execution or integration acceptance.

R10 baseline SHA-256:

- Router: `7B8B9FE1EDFC794D6D24CC42F377C6F81554DD19138A87DB550C6C5F86E99881`.
- Adapter: `8C134D6316A95A98913132A31B0B4D0B79BF6BD75F7B5A6688D114A301C24FE0`.
- Coordinator/data: `5274BFC9E27E7F97BDCB41B680E36052A793B51498372B565BBBD24B538CFDC6`.
- Original fixture: `1B1C4018A8A914D04155782E7ACCD4971C5F33E85A4B2A1653A9C7D1DDB11EEF`.
- Data fixture: `DEB2C57D381D8641F2D706C1B381A76BA97AE64BA4AB1BFB0D5CD68899CB01D7`.

Output targets: `artifacts/c2-r10-playmode.xml` and `.log`. Result pending;
no source/test edits are allowed while the actual Editor remains active.
The newly selected cases use two real launch cohorts, clean foreign rejection,
coordinated corruption, post-gate corruption, atomic old-close contention, quiet
completed ticks and reflected terminal replay. REQ/AC-M5D7QC2-003/004/008
remain partial pending actual results.

Terra's read-only inventory and Luna's independent review agree that further
evidence is required: all 19 named cutover checkpoint faults, the missing
owned map/callback operation before/after faults, coordinator-level delete and
final-reprobe failures, and actual restart/resume cases. AC-M5D7QC2-001..010
are not all closed. Profile-only probes do not prove Unity terminal outcomes;
test names and smoke passes do not substitute for the missing matrix.
These are bounded technical checks, not a request for a new user product decision.

## True-restart design gap — AC-M5D7QC2-007 remains open

Sol's read-only counter-review confirms that ordinary preparation rejects a
durable reset barrier, whereas current C2 requires a cohort minted only after
ordinary launch publication. Calling C1 Resume can yield a fresh DiskPrepared
proof but cannot, by itself, establish that required live cohort in a restarted
process. Starting the fixture before creating the barrier would not prove a
restart; fabricated preparation/history or temporary barrier removal is forbidden.

Astra requested the separate Review-only
`2026-09-28-vd09-m5d7q-c2r-restart-bootstrap.md` contract for a private recovery
reservation and pristine disabled router. Actual adapter Awake precedes router
Awake, so selection/reservation must precede preparation and real Resume/mint
must run only through the later recovery branch after router Awake. Ordinary
Start publication and launch history are not simulated. Luna pre-review and
Astra approval are prerequisites to any implementation. Parent AC-007 remains
open; safe blocked reload is not mislabeled full restart completion.

## R10 actual result — focused partial PASS

The same Editor PID 21688 completed normally with logged test-run exit 0.
The wrapper returned tool exit 1 early for absent XML after its wait; no Editor
relaunch or kill occurred. Final XML records **41/41 Passed**, failed/skipped/
inconclusive 0, duration 927.3176001 seconds. XML SHA-256:
`579C8451AC800CF5BE315B171940FBC264B2EB822F3D67D71F2E791D624AB369`.
All five R10 source/fixture hashes above remained identical after execution.
The Editor and importer have exited; runtime/test allocation may now resume.

This closes execution of the selected AC-M5D7QC2-001/002/003/004/006/008
rows, not those ACs' whole matrices. Independent final integration review,
missing matrix cases, C2R actual-process restart and required regressions remain.
Astra approved C2R at SHA-256
`34A63CDCEB8CC221AF29A8A65D6AC40506C6488CF3DF41E477FD9FCF324D774C`
after Luna's contract pre-review; implementation is now allocated, not accepted.

## Post-R10 implementation allocations

Terra froze the new same-process fault fixture at SHA-256
`16ACFB2885BB6AF2AF0823725852AE49E863856095EFD010B2943B59F91756D3`
and metadata `304E91D4CC8CBA9C49D4366775E2C269C263274474D422BF65889972DB70EDCB`.
It authors 72 fresh-case rows; none has executed yet. The rows include 19 C2
checkpoints, 22 exact owned-operation faults, eight Profile authority boundaries,
seven hostile post-delete disk changes, marker pre-reauth and authorized-delete
changes, two actual junction cases, four final-pair metadata changes and other
cleanup/lock cases. Luna pre-review is allocated before execution.

Astra identified a remaining local diagnostic short-circuit in untransferred
candidate disposal. Terra runtime ownership includes fixing it so only the
private atomic attempt witness authorizes/blocks the close; true/false diagnostic
corruption is covered by the new fixture. Four previously unused new-action
operation seams are being connected to exact captured candidate operations.
Runtime is changing and no new execution or source acceptance is claimed.

The C2R contract now concretely defines same-call authority for the existing
readonly `ProfileResetDiskResultV1` struct, without changing C1: private actual
Resume registration, validated result facts, the exact fresh proof reference
and an opaque one-shot execution witness. Boxed-value reference accidents or
value equality alone are not authority. Current Approved clarification SHA:
`DA1BD70DBBD42DC6817847B63411DEACF18FEC448FEC547A441E91B4C6E66C2D`.
Original allocation approval SHA remains recorded separately above.

## C2R first frozen implementation — not accepted

Terra's first recovery runtime snapshot comprises adapter SHA-256
`01FCB1763BB0F54B5E5B0F3F1D1AF0E1D841192A15B26126DF83FDE1A3CF840B`,
router `7F0977C144CAE91D250835A5F37041EF66659480A557003F71AD06790EB76629`,
and coordinator `641BD580AD5183AA7A321499C368DF5DC3F9ADE97733CB58888289E50B834C24`.
Luna independently reported four P1s: manual-repair classification, repair-blocked
result witness, execution-claim consumption before I/O, and immediate pre-gate
authority/default-binding validation. Additional Astra source hypotheses are
under independent review. No Unity execution or acceptance of this snapshot.

The corrected same-process C2 fault matrix now has 73 rows, SHA-256
`F1EAA0EB7E7E955A1C57C2C230A55FAED3D6FD3AB18169E68333AFA08D0562D9`;
Luna closed the descendant-junction cleanup P1 by source review only. These rows
have not executed. A separate 28-row same-process recovery fixture was authored
and is undergoing cleanup/traceability corrections. It does not constitute
AC-M5D7QC2R-005/006 distinct-process evidence. All C2R ACs remain unverified.

Luna's expanded frozen-runtime review confirms one compile blocker and nine
P1s in total. Astra allocated bounded correction of every recorded finding to
the runtime Terra owner. The additional findings include fail-stop latching
before lease release, Acquire-only Busy classification, no validating Outcome
getter after receipt publication, private recovery routing despite diagnostic
corruption, and closed repair-result synchronization. The StagedRestartBootstrap
getter itself is a raw field read in this snapshot; the validating Outcome
getter, not that raw getter, establishes the post-publication finding.

Read-only host-context preflight on 2026-09-28 passed: pinned Unity 6000.6.0f1,
both signed Licensing Clients 1.18.3 aligned, nonempty entitlement, and no
competing project Editor. This is environment evidence only; no new test ran.

## Corrective review and startup compatibility approval

The first corrective runtime snapshot (adapter
`02E6D526EF00208A8A93BD4A24EC4D62CAD560978036B541306625690D49FF44`,
router `7F0977C144CAE91D250835A5F37041EF66659480A557003F71AD06790EB76629`,
coordinator `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`)
closed the initial compile/containment findings by independent Luna source
review. Luna and Astra then identified a remaining ordinary-startup P1: an
unreserved selection root read followed by a second ordinary root read violates
M5D7M's exact Reserve/single-root sequence and permits A/B root divergence.
No execution or final acceptance occurred.

Sol supplied bounded counter-design. Luna independently pre-reviewed the
startup compatibility appendix at Review SHA-256
`1B546FD7C7D670543A945A8A1BEACB5534334DFC841EA39D4E65D6C0FB00DECA`
with P0/P1=0. Astra approved that appendix at current C2R SHA-256
`4BDC6506EE023FDFE66938F3F0E3BAF09DF733D379BD3DC3FC36467E7A6FFCE9`:
exact actionless reservation before one root lookup, same normalized root for
selection/preparation, irreversible private recovery promotion, preserved
getter-thrown failures, and an explicit closed-repair override for unsafe
returned-root rows only. Terra runtime and separate test owners are implementing
the correction; source review and execution are still required.

The four-phase external-process fixture was strengthened after Luna's evidence
P1: real canonical non-default primary seed before C1 Begin, actual archived
old bytes, exact-r0 current-cell/root/actions/generation agreement, and no old
state/receipt resurrection. Current fixture SHA-256
`17BEBBC75A0FF83C4F39C04DE104DF4172D23F6915C9E40E61556CF27E4A8B8E`;
it remains execution-pending. Required non-process regressions explicitly
exclude that externally driven fixture, whose phases must execute separately
under Main's exact-PID control. This is not an unfiltered full-suite pass, a
Skip/Ignore waiver, or process-AC closure.

## R11 actual compile failure — no executed tests

After runtime source-review P0/P1=0 (not a compiler-pass claim), Main launched
the frozen source in actual Unity with a focused non-process filter.
`artifacts/c2-r11-playmode.log` reports CS0266 at adapter line 326: C1's existing
`ProfileResetDiskResultV1.TargetByteLength` is `long?`, while the private recovery
witness stores `long`. The Editor logged exit code 1 and no Editor remained.
No test result or passing row is claimed. The wrapper may still finish its
missing-XML wait independently of that actual compiler exit; no active Editor
was killed or relaunched. R11 evidence is preserved and will not be overwritten.

Astra allocated only the adapter's validated nullable-length extraction to
Terra before any private recovery mint. C1 types and contracts stay unchanged.
Fresh compile/execution and independent correction review are pending.

## R12 fresh focused run — active, not a result

Terra's adapter-only nullable correction SHA-256 is
`AE31A8AA54B6A2466F3B275632B9A6BF275D44CCEDF6F547BCDFEB14E2A03FAC`.
Luna independently reviewed the correction as source-only, preserving the
actual R11 compiler failure. Router SHA-256 is
`DCB078169A6AE63F97EBDDF6E68F9601519953F4481C4053996A87177243F2E5`;
coordinator remains `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.
Bootstrap fixture is `6EFF2E1BD33E0CC9A6C622452090C039A13AB3582945789898BB6961CC84CAFF`
(7 direct tests + 26 TestCase rows = 33 rows, not 11 execution rows).
Process fixture is `BD690FE231E13E1785332F514896FB077069B7043C5854F92A1D674EFA531B66`.

Main launched R12 at 13:14:46 +09:00. Host-context inventory identified the
actual Editor PID 45232 for `artifacts/c2-r12-playmode.log`; no command line
or credentials were emitted. Its log records successful script compilation
and test-run initialization. It remains active, with no final XML at the time
of this entry. All source/test workers are frozen during the run. Selected
rows exercise original C2 happy path, production adapter recovery happy path,
ordinary absence/single-root/Busy/manual selection, exact old preparation
trace, and hostile descendant-junction cleanup. No pass or AC closure yet.

## R12 actual result — 6/10, corrective work required

The same Editor PID 45232 finished with logged test exit 2. XML is
`artifacts/c2-r12-playmode.xml`, SHA-256
`EA897686F1D0EF856F1E3B870A3D0009029E82E44C23B89902FC080FDD07152B`:
10 total, 6 passed, 4 failed, skipped/inconclusive 0, duration 72.5861606 s.
The wrapper reported UnityExitCode 0 and tool exit 1; the actual Editor's
logged test exit 2 and XML failure counters are authoritative. No successful
suite or AC closure is claimed.

Passed rows include original C2 actual happy path, production Adapter Awake
selection/Start Resume/C2R happy path, absent ordinary startup, single-root
A/B safety, actual held-lease BusyBlocked and descendant-junction cleanup.
The production recovery row is real executed evidence, unlike isolated
direct-router promotion tests; distinct-process criteria remain pending.

The existing M5D7M preparation-trace test reached Start but failed because C2
unconditionally attempted to mint a Hub-only reset cohort for a historical
non-Hub router whose Hub proof fields are correctly null. Broadening those
proof checks is not authorized. Luna is reviewing narrow Hub-only recovery/
reset-cohort scope while preserving the historical other-router launch.

Three repair-selection rows failed in test cleanup: fixture Dispose removed
the issued-root registry entry, then the outer finally called Cleanup again.
Terra owns a confined, issued-root-only idempotence correction; foreign or
unissued cleanup remains prohibited. No runtime repair failure is inferred
solely from that cleanup exception. R12 source and result evidence are retained.

## R13 Hub-only correction — fresh execution pending

Astra approved the authored-Hub-only narrowing of REQ-M5D7QC2R-001/003/005
and AC-M5D7QC2R-001/003/007/008 at contract SHA-256
`2BB53A16B9E7D9FA6B9A8C50CA928BEFEF353C137E0DD97C03BF2FE430112E74`.
Historical non-Hub startup keeps its original failure and receipt semantics;
no Hub proof check was relaxed. Terra restored the original unsafe-root
assertions and added explicit getter-thrown exception rows. Separate production
Hub tests exercise the approved unsafe-returned-root repair classification.
The fixture retains an immutable ever-issued root registry so repeated cleanup
of the same owned root is idempotent, without accepting any foreign root.

Frozen R13 sources: adapter
`C9D2CAE724113BEF186EC82FC5761367534F21A0ADAF7D3437765050279746E8`, router
`66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, coordinator
`60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`, bootstrap fixture
`B74F01633BFFFEEC9BE3FAF4E53B451F85C2D21A448896A2E12AEB059ABE263E`
(37 rows). Main launched fresh `artifacts/c2-r13-playmode.xml/.log` against
these sources, including the restored legacy failure matrix and original C2
happy path. No result or criterion closure is claimed before the actual XML.
Independent source review is being reconciled against these exact hashes;
an assertion based on an older snapshot cannot establish a current defect.

### R13 actual result — 38/52, no acceptance

Actual Editor PID 12228 finished with logged test exit 2; no Editor remained.
XML SHA-256 `A9753FD7F5C5059381D4ED91B6AAB1FDC72DBEB520A378B60DE31C03790B199F`
records 52 total, 38 passed, 14 failed, skipped/inconclusive 0, duration
83.9884688 s. The wrapper's launcher exit 0 is not the test exit.
The restored legacy trace and 12 environment/preparation failure rows passed,
as did the original C2 happy path, production recovery happy path, four Hub
unsafe-returned-root rows and the three previous repair-selection failures.

Thirteen checkpoint rows reached a test assertion comparing FileAttributes
zero to integer zero; the XML prints Expected 0/But 0 but the types differ.
One reflected-diagnostic corruption row invoked Router Start and received the
designed invariant exception; teardown also logged it. Terra is correcting
only the focused fixture, while Luna independently evaluates the exact failure
and containment assertions. These are failed executions, not a passing matrix
or permission to suppress arbitrary errors. Fresh execution remains required.

Luna's `2026-09-28-vd09-m5d7q-c2r-r13-luna-pregate.md` withdraws its preceding
snapshot warning after absolute-path hash verification and finds no new runtime
P0/P1 in the frozen current source. This source review is not suite acceptance.

### R14 corrective bootstrap execution — pending

Terra corrected the enum-mask assertion representation and explicitly asserted
the wrapped malformed-state rejection, retaining the no-actions, UTC/Prepare,
receipt and fallback prohibitions. One exact expected teardown invariant log
documents deliberate reflected corruption; no general log suppression exists.
The process fixture received the equivalent typed enum-mask correction only.
Bootstrap SHA-256 is
`41A9F04F76C9771CB44FC1A35A0A3823952101C8B4ACF28AB4E575268A67E320`;
process fixture SHA-256 is
`F612D9E042D2748BA2D23E2C35CCCA693C1225EA0356A8A9AF41F9D2E0545AD4`.
Runtime hashes remain unchanged. Luna's independent corrective pre-gate
`2026-09-28-vd09-m5d7q-c2r-r13-terra-correction-luna-pregate.md`
reports source P0=0/P1=0. Main launched the 37-row bootstrap-only R14 run
with fresh XML/log paths; no executed pass is claimed yet.

### R14 actual bootstrap pass; process gates still open

Actual Editor PID 37956 logged exit 0 and terminated. R14 XML SHA-256
`5B2099E3AA12D8804E0647B573B1E560BD53B5DD48CADD9BF8A2A64E59CA13F5`
records 37/37 passed, failed/skipped/inconclusive 0, duration 27.8096041 s.
The corrected fixture hash and runtime snapshot were unchanged. This closes
the failed bootstrap rerun, not all C2/C2R ACs. Main next launched R15's actual
Prepare phase against a new uniquely owned OS-temp process base. Separate
Resume, deliberate death-after-delete, ordinary-after-death, fault matrix and
required regression execution remain pending.

### Required regression runner partition — Astra technical decision

Luna independently reviewed the R21/R22/R23 partition in
`2026-09-28-vd09-m5d7q-c2r-required-regression-plan.md`; Astra approves it as
runner scheduling only. R21 executes every fault-matrix row. R22 executes the
remaining actual PlayMode InputUnity/HubPresentation namespaces, omitting that
already-owned matrix only from the remainder invocation. R23 executes all
actual Profile/InputUnity/HubPresentation EditMode namespaces. The aggregate
excludes only the externally orchestrated process fixture, whose separate
evidence is preserved. Full-name XML case reconciliation and unchanged source
hashes are mandatory. No Skip, AC waiver, unfiltered full-suite claim or single
invocation requirement is invented; no matrix case is omitted from the union.

### R21 actual fault-matrix result — 72/73, final root-drift rejection failed

The unchanged frozen runtime/fixture completed all 73 selected rows in actual
Editor PID 26536. Its log reports test exit 2 and the Editor terminated.
`artifacts/c2-r21-faultmatrix.xml` SHA-256
`6C361431CBD48E12F9BAB1AAAFA506C436BFDB471DA4A395A7A107D97CB1FDE8`
records 72 passed, 1 failed, skipped/inconclusive 0, duration 1806.1421165 s.
The earlier wrapper missing-XML tool exit happened while the same Editor was
active; it is not the authoritative final test outcome.

The failed row is
`AC_M5D7QC2_004_006_AfterFinalProbePairMetadataMismatchCannotStageReceipt("root")`:
after the real final disk probe, the control changes the adapter's independent
launch-root witness to a foreign path. Expected ReloadRequired, actual Completed.
No test assertion is waived. Astra allocated cause analysis and a proposed
minimal runtime root-agreement correction to Terra, with independent Luna
evidence/source review. C1, actual foreign filesystem state and historical
receipt values may not be altered. C2/C2R acceptance and the final regression
gate remain open; fresh execution is required after any approved correction.

Luna's independent `2026-09-28-vd09-m5d7q-c2-r21-luna-digest.md` confirms
the root-drift failure as P1, not a test expectation defect. Terra identified
that the final pair check compares the private root to itself but omits the
independent launch-root mirrors. Astra approved only an adapter-local,
terminal-safe check of immutable original-cohort owner/router/root/receipt
against their live diagnostic mirrors, including both root witnesses. No
prepared-launch action validator, C1, router or coordinator change is needed.
The existing final pair check must contain the exact original cohort and return
ReloadRequired before receipt staging. Retained current-cell getters must
reject the mismatch; detached immutable receipt/history is not rewritten.

Separate Terra test ownership extends the unchanged rejection policy to the
second root witness and adds post-completion retained-cell tests. This increases
the matrix from 73 to 74 rows; the earlier actual 72/73 result remains intact.
New source review, fresh matrix and required regressions remain pending.

### Root-drift corrective snapshot and R24 focused execution

Terra changed only the adapter runtime; router and coordinator are unchanged.
Adapter SHA-256 is
`0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`.
Separate Terra test ownership froze the now-74-row matrix at
`7FCBF75770677402BCE018CF38CA9437DB68DD613BCEC474E173E019B3AD071A`
and data fixture at
`8D220F9D86B587884327BED55ED6953DDA89D210B14AD27A68335DE856BF294D`.
The two post-completion rows retain the actual current cell before root A/B
mutation and verify both its direct live getters and adapter getters reject,
while historical receipt values and detached final evidence stay unchanged.

Luna's `2026-09-28-vd09-m5d7q-c2-r21-final-correction-luna-pregate.md`
reports corrective source P0=0/P1=0. Main launched R24 focused execution:
37 bootstrap rows, 5 final-pair mismatch rows, 2 new retained-cell rows and
the original C2 happy path (45 expected). No result is claimed yet. Fresh
74-row matrix, required regression partitions and repeated actual process
phases against this final adapter hash remain required before acceptance.

### R24 actual result — historical getter expectations corrected before rerun

Actual Editor PID 49464 exited 2. R24 XML records 41/45 passed, four failed,
zero skipped/inconclusive, duration 333.3807241 s; SHA-256
`A16F92F95F9C973ACC9B875378F2697C2447A846EAF5814AA97AE70979C4EF5C`.
The wrapper's earlier missing-XML result was not the final Editor outcome.
Luna independently classified all four failures in
`2026-09-28-vd09-m5d7q-c2-r24-luna-digest.md`: root A/B live-cell rejection
works, but the matrix/data assertions incorrectly require the immutable
historical `CurrentReceipt` getter to throw. The Approved C2 history contract
requires that receipt to remain readable and unchanged. Astra authorizes only
exact historical-equality test expectations; live/retained cell rejection,
terminal containment, no new receipt, disabled maps and notification checks
remain mandatory. Runtime remains unchanged. The failed R24 is preserved;
fresh focused 45-row, full 74-row matrix and required regressions remain gates.

### R31 actual focused rerun — 45/45 on corrected historical expectations

Actual Editor PID 49260 (EditorInfo telemetry) completed and exited 0; no
Editor remained before the next phase. XML
`artifacts/c2-r31-root-history-corrective.xml` SHA-256
`510821792AB9FF66FA7FE65D01F85AF7711F0475AB0CFC64EC803986FE3558CE`
records 45/45, failed/skipped/inconclusive 0, duration 319.4986741 s.
Matrix SHA is `1AD60243FB3902E52CC5BD9D05C5A4C4DBEC7ADE1060979CAA79659EA32A85E4`;
data SHA is `9F61A4EB2278C8069742BDA56BCF13140E1B717826908D5D6BC64B60B9EEB8A0`.
Runtime adapter remains `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`.
Luna independently approved the test-only correction before execution. The
early wrapper missing-XML exit is preserved, not confused with actual failure.
Fresh process R26–R30 phases followed; R25 full 74-row matrix is running.
Required R22/R23 regressions and independent final integration are still pending.

### R25 actual complete fault matrix — 74/74

Actual Editor PID 52284 exited 0 and all Editor processes ended before the
next run. XML `artifacts/c2-r25-final-faultmatrix.xml` SHA-256
`FD9B0E7B1FE60ABDBADAB2FF4C0AAD309B5635D4D4188264DA4BB8E43292D396`
records 74/74, failed/skipped/inconclusive 0, duration 1966.6145073 s.
Main rechecked adapter/router/cutover/C1/matrix/data hashes: unchanged from
the final R31 snapshot. The early wrapper missing-XML exit happened while the
actual Editor was running and is not the final outcome. All matrix rows were
executed; neither R21's failure nor R24's failures are overwritten.
R23 executes the complete actual Profile/InputUnity/HubPresentation EditMode
namespace union next, including both legacy and newer Profile namespaces;
R22 remaining non-process PlayMode follows. This order is runner scheduling,
not a behavior/coverage change. Final independent review and acceptance wait
for both required partitions.

### R23 actual EditMode regression — 595/597, two independent test gates

Actual Editor PID 17868 exited 2 and is gone. XML
`artifacts/c2-r23-final-editmode.xml` SHA-256
`9A9309CF1B5208EE478782D58F0DF18663E38637792B55A4A04E6FF501E5CCC6`
records 595 passed, two failed, skipped/inconclusive 0, duration
1474.5806043 s. All four runtime source hashes remain the final frozen values.
The two failures are the single aggregated controller/nested-proof mutation
case exceeding NUnit's 180000 ms timeout, and Q0's exact historical router
fingerprint differing from the Approved C2/C2R successor. The latter had been
independently identified as a stale audit expectation before execution; the
actual failure is now recorded, not inferred. No runtime correctness failure
or passing result is inferred from the timeout. Both gates remain open.

Astra requests independent review of test-only corrections: preserve every
original value/controller corruption assertion in bounded separate rows,
without increasing timeout or skipping a case; preserve all nine Q0 historical
manifest rows while pinning only the two authorized current successors and
keeping the other seven current fingerprints strict. Shared helpers/runtime
must not change. Any selective evidence reuse requires independent exact
coverage/source/dependency review; the failed R23 remains historical and is
never presented as a passing suite.

### R23 correction authorization — Astra, bounded tests only

Luna's `2026-09-28-vd09-m5d7q-c2r-r23-luna-pregate.md` independently
classifies both failures and reviews complete assertion preservation. Astra
authorizes only the existing controller test's 17-row decomposition (one full
value-field group, twelve Ready, two Pending and two Consumed fields), and the
existing Q0 audit's exact two current-successor fingerprints with unchanged
nine-row historical evidence and other seven strict current fingerprints.
No timeout increase, Skip, runtime change or historical rewrite is authorized.
Terra implements; Luna compares the preserved original source snapshots before
fresh R32 executes all 17 rows and all four Q0 audit rows. R23's unaffected
592 passing rows may be reused only after the independent source/helper/
dependency comparison and fully-qualified coverage reconciliation. The failed
R23 and its timed-out incomplete aggregate are not accepted evidence.

Original test snapshots are `artifacts/c2-r23-before-controller-tests.cs.snapshot`
SHA `218EB71BDD168C59A9DBF03082A37F31741A94AE1A360D62BA4D15C89099FA5F`
and `artifacts/c2-r23-before-q0-audit.cs.snapshot`
SHA `23BE8E126687609321C7E26968ECC2F552C2AD8AE96E6F1FDDA2A04A5E30A8F8`.
Main captured 202 other affected runtime/test/meta/asmdef files, excluding only
those two test source files (not their meta files). Ordinal path plus SHA-256
lines, LF-joined UTF-8 without BOM, have aggregate SHA
`7D712A7326C121AE2DA439E16368968A7A3A716E7CAF1E0E444E18FC4C9CBD1E`.
Post-correction equality is mandatory; accepted runtime/C2 matrix/process
evidence must retain its original frozen relevant-source hashes.

### R23 test-only correction frozen — independent review pending

Terra changed only the two authorized EditMode sources. Controller SHA-256 is
`40AA974BF8407A6FD2AAE57830256F04599AF1A835F708999BDC10F332061699`;
Q0 audit SHA-256 is
`12B98E06919CDC2AFFBD9E380BF2B4546C00D3BA86A8F86E5D9EDF59FFE331FD`.
Main compared both original snapshots: only the bounded AC006 decomposition
and Q0 two-successor audit changed, with shared helpers otherwise preserved.
The other 202-source aggregate remains exactly
`7D712A7326C121AE2DA439E16368968A7A3A716E7CAF1E0E444E18FC4C9CBD1E`.
Luna must independently approve coverage/dependency reuse before R32. The
generated AC006 case names replace the old aggregate method's display name;
the runner must select all 17 generated names plus all four Q0 audit cases.
This is not execution evidence, acceptance, or a passing R23 claim.

### R32 actual corrective execution — 21/21, EditMode partition 613

Actual Editor PID 46300 completed with exit 0 and all Editor processes ended.
XML `artifacts/c2-r32-editmode-corrective.xml` SHA-256
`B06D406CEE0AF1D7D8985A6C5416F1CAA85A6D08B6248DFD603A4D8A9B709D65`
records 21/21, failed/skipped/inconclusive 0, duration 694.6409837 s.
All 17 controller generated cases and four Q0 audit cases were discovered and
executed. Main rechecked the unchanged 202-source aggregate; Luna independently
reconciled 592 retained unique passed R23 rows plus 21 fresh unique R32 rows:
613 accepted versioned EditMode cases, not an unfiltered full-suite claim.
The five superseded R23 cases, including its failed aggregate, are excluded
only from the final accepted partition, never removed from historical XML.
The wrapper's early missing-XML result is preserved and is not actual failure.
R22 now runs the three-namespace remaining PlayMode partition with only the
external process class and separately owned R25 matrix excluded in this
invocation. Final C2/C2R acceptance remains pending R22 and independent review.

### R22 actual execution and user pause — final gate remains open

The exclusion statement immediately above describes the intended partition,
not the actual discovered selection. The preserved original runner's regex
did not exclude either the external-process fixture or the matrix fixture.
Actual R22 XML `artifacts/c2-r22-final-playmode.xml` SHA-256
`43D4FE3ABDC861E7CE70F0B2E6355D3AD801C47A7453F5901EFC905C9EA08FFB`
records 614 total, 610 passed, four failed, zero skipped/inconclusive, duration
7939.9749898 seconds. The log explicitly records actual Editor exit 2 (Failed),
and no Unity Editor remains. This is an actual failed run, distinct from the
old wrapper's earlier missing-XML observation; neither is relabelled success.

All four failed cases are `ProfileResetRestartProcessV1Tests` rejecting absent
externally supplied phase environment. The ordinary regression invocation
incorrectly included them. R26–R30 remain separate phase evidence, not a waiver
or a means of changing this XML's failures. All 446 historical scoped baseline
qualified names are present. All 74 R25 matrix names were also repeated and
passed in R22; they must not be counted twice. The unchanged 202-source digest
remains `7D712A7326C121AE2DA439E16368968A7A3A716E7CAF1E0E444E18FC4C9CBD1E`.

AC-M5D7QC2-010 and AC-M5D7QC2R-007/008 final zero-failure gates remain open.
Record the runner partition/setup defect, preserve the 610 passing-case
evidence and all original artifacts, and obtain a correctly discovered
zero-failure regression invocation after explicit user resume. Do not weaken
the process tests, introduce Skip, or infer product runtime failure from their
missing environment. The user directed completion of this batch then pause:
no rerun, C3 implementation, C4 approval or live UI/scene work is started.
