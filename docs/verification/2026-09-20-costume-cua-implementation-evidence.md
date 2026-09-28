# Costume CUA Unity presentation adapter — Terra implementation evidence

- Date: 2026-09-27
- Implementer: Terra (`gpt-5.6-terra`)
- Contract: `docs/specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md`
- Approved contract SHA-256: `8F18474B8D966BCD688ABE3DBFB723872D341868FAD552583E430C081C689B1A`
- Luna final contract pre-gate SHA-256: `AC90107EFB11D75D2D15BEB71906F69A8A80A28B9D246B4596CD5F69B5F9C822`
- Status: **Verified for the Approved synthetic-only CUA contract; `AC-CUA-009: PASS` by Astra after Luna's independent R15 review.** Earlier pending language below is historical; the final acceptance addendum controls.

## Static-review correction baseline — 2026-09-27

The Approved static-review closure amendment is contract SHA-256
`600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`;
Sol's bounded proposal is `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
and Luna's amendment re-gate is
`A33EC5EC8B4C24CC2A1B0B6101E7BF9B9E9F69F33E416DF8EA1F5B5372850175`.
Before this correction the four permitted replacement files were:

| Path | Pre-correction SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `E77192DDF0DD12F758652277AB48DD23F1CA25779890C5357D19DA7840F22865` |
| `CostumeUnityPresentationAdapterV1.cs` | `AD12C248D4978DDAECE072BB169C68FF407F940CD1CF6C3321DA150265813127` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `668D6C5E1CC6F6E3D5634B985DEFBC0A7283BD22D31117A5DFBAEA4411D2CDAC` |
| `CostumeUnityViewPresenterV1Tests.cs` | `E9FD6D7DAA4F1E792F3B7D27752DAFE0EC2C09B3D4F9AA664FE9944F98784913` |

The correction changes only those four files and this allowed evidence record.
It removes selection's snapshot argument, uses the explicit port's private
observation slot, invokes pure `TrySwap` for bootstrap/replacement, strengthens
independent package validation, and rejects widened-bound/within-clip duplicate
geometry. Unity remains forbidden until Luna reviews the replacement hashes.

| Corrected path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` |
| `CostumeUnityPresentationAdapterV1.cs` | `F954AB1830F965481B49C8F4CD26ED5D55FBB84BEEC7A0AE37FF98E60014F25C` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `387F780F25892D23C3B1346D6E92215E73073523A5EC49BBC40F70B68E4BFBAB` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

## R3 correction ledger (static only)

Luna R3 review SHA-256 `52A1EC4221B411124A5B63D7093DCD4D3913AB91203CF7AC72EE7EAFE6E835CB`
identified remaining assertion gaps. The allowed R3 delta keeps the four-file
boundary: current tuples now correlate adapter/media/binding/snapshot actor and
the exact stored deterministic frame; test fixture replacement uses true primary/
backup promotion; action matrix covers the seven contract actions, both facings
and `0/500/999/1000`; and synthetic wrong-primary/projection/terminal and
mechanics-probe assertions remain isolated. This is a coverage ledger, not a
claim that Unity compiled or ran.

## R4-A current-tuple corruption coverage (static only)

R4-A adds 23 independently named `TestCaseSource` corruption rows, each run
through both selection and higher-tick observation for **46 exact cases**:
foreign binding/media/snapshot actor; absent/different state current; binding
costume/set/revision/hash/action-vocabulary fields; and all eight stored frame
scalars. Each case replaces only the private tuple in a valid-first-publish
fixture, requires `ReloadRequired`, preserves that malformed tuple reference,
and asserts save/replace/projection/current/highlight stability plus later
terminal blocking. The existing true replacement test remains the valid control.
No Unity execution is represented by this ledger.

Static correction checks: both untouched asmdefs parse as JSON; the old
snapshot-bearing selection overload is absent; the adapter contains both pure
`TrySwap` call sites; forbidden scene/global/time/renderer/mechanics API scan is
clear; and scoped `git diff --check` is clean. These are static checks only,
not Unity compilation or test results.

## R2 static-correction baseline — 2026-09-27

Luna's corrected-implementation review SHA-256
`E7F519A759B3DF40876CD3F9969816374A4FA831E330444C36FDDC789AFC06FD`
retained Unity execution block while requiring current-tuple/frame correlation,
full negative validation, concrete CIO fault and terminal matrices, and action/
facing/mechanics probes. The R2 pre-change source hashes were Media
`7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79`,
Adapter `F954AB1830F965481B49C8F4CD26ED5D55FBB84BEEC7A0AE37FF98E60014F25C`,
adapter tests `387F780F25892D23C3B1346D6E92215E73073523A5EC49BBC40F70B68E4BFBAB`,
and presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
R2 adds only the approved static correction/test coverage; it records no Unity
run, test outcome, catalog promotion, or media acceptance.

| R2 corrected path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` |
| `CostumeUnityPresentationAdapterV1.cs` | `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `F02E78466C7FB2972CFA9AFE68753572586E425E30F3B4B4B70DCED2693E3897` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

## Allowlist baseline and impact

The baseline public seams are intentionally read-only for this child:

| Path | SHA-256 | Role |
|---|---|---|
| `Assets/AcadeGameMaker/Runtime/Costumes/CostumeCoreV1.cs` | `9149AD9E4D806F02811C0D85F5E1DB044E6380C70B5409C4929CA09913EF8178` | 36 pending catalog and immutable selection state |
| `Assets/AcadeGameMaker/Runtime/Presentation/CostumePresentationV1.cs` | `600077CAE9B792451A37CC977FC19632E7AE2309428978CED4552937CB6C27DA` | immutable binding and completed snapshot seam |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` | `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E` | exact synchronous CIO save boundary |

Only the contract allowlist may change: the new `Presentation/Unity` runtime
assembly and three source files, the isolated `PresentationUnity` PlayMode test
assembly and two test files (with metas), this evidence record, and the minimum
documentation links. No existing source, catalog row, asset, scene, prefab,
real media, package, ProjectSettings file, persistence-root selection, InputMode,
or live renderer owner is authorized. Positive fixtures are synthetic byte arrays
and temporary `Texture2D` objects only; they do not import or promote real media.

## Implemented requirement coverage (execution pending)

- `REQ-CUA-001/002`: immutable 36-row pending view and transient highlight only;
  the focused PlayMode fixture asserts the real catalog's exact 36 pending rows,
  `NoAcceptedDefault`, unchanged state revision/current ID, no tuple, and only a
  transient highlighted ID.
- `REQ-CUA-003/004/008`: canonical synthetic package bytes, hashes, exact
  clip/facing/geometry/filter/PPU/pivot/baseline/reference validation, then one
  immutable tuple publication. The source manifest canonically binds portrait and
  gameplay-set IDs, all three hashes, sorted required actions, closed `left/right`
  facings, and schema version. The fixture constructs only temporary synthetic
  textures and byte arrays; it proves canonical acceptance, filtering rejection,
  and no real catalog/media claim.
- `REQ-CUA-005/006/007`: explicit instance-bound post-commit snapshot port, exact
  loop/non-loop endpoint mapping, visual-only retained state, and direct synchronous
  `CostumeFileAdapterV1.Save` outcome handling (`FailedBeforeCommit`,
  `CommitOutcomeUncertain`, `ReloadRequired`). The post-save authoritative change
  is one assignment of the complete immutable tuple; state and tick are read from
  that tuple rather than published via companion component assignments. The focused
  synthetic CIO-port tests cover successful durable correlation, pre-commit failure,
  commit uncertainty, and no publication on either failure.
- `REQ-CUA-009/010`: no global lookup, scene lookup, renderer discovery, or
  mechanics connection; static/isolated tests assert the explicit port boundary,
  injected projection, absence of static mutable owner fields, and the exact
  additive allowlist.

## Static implementation audit — 2026-09-27

- `git diff --check` reports no whitespace error in the CUA allowlist. Existing
  unrelated workspace warnings are not part of this child.
- Both new asmdef JSON files parse successfully. The runtime assembly references
  only the existing Costumes, Costumes.IO, and Presentation seams; the focused
  test assembly references only those plus the new runtime assembly.
- Source scan of the new runtime and tests finds no scene/global lookup,
  `Update`/`FixedUpdate`, Animator/time, renderer owner, or mechanics API use.
  It has no persistence delegate/receipt injection: the only persistence member is
  the constructor-injected concrete `CostumeFileAdapterV1` and the only save call is
  synchronous `Save(staged)`.
- Unity compilation and tests were intentionally **not run** under this task's
  authority. Therefore AC-CUA-001..009 have implementation coverage only and
  await Luna's independent static gate followed by authorized focused/full runs.

No Unity test result, media acceptance, catalog promotion, or Verified claim is
recorded until an authorized Luna-reviewed execution gate.

## Addendum — authorized Unity execution history through R14 (2026-09-28)

This additive addendum supersedes the opening status only. The static ledgers
above remain historical records of their respective pre-execution gates.

`REQ-CUA-001..010` retain the bounded synthetic-only implementation scope.
`AC-CUA-009` remains **open**: it requires one complete authorized full
PlayMode result XML with `failed=0`, `skipped=0`, and `inconclusive=0`.
Accordingly, none of the runs below accepts real media, promotes a catalog row,
or establishes a live wardrobe scene.

- R11 focused PlayMode passed `195/195` with failed/skipped/inconclusive all
  zero. Result XML SHA-256:
  `0485ADB16440963B553A6C4D48191D47F1C4C736624B3402D6D9632414740F19`.
- R11 full EditMode passed `800/800` with failed/skipped/inconclusive all zero.
  Result XML SHA-256:
  `D4E18A69084A2E0140E19D503C734AC7DD68C30AE1FF723D5BD13CC7E3A38C7C`.
- R11 full PlayMode and R14 full PlayMode both reached the identical
  `DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`
  suite-order liveness condition and produced no result XML. They are invalid
  attempts, not passes and not CUA product-defect findings.
- R12 exited abnormally without a result XML; R13 was blocked before test
  startup by the Unity licensing/package authentication environment. Neither
  run supplies an `AC-CUA-009` result.
- R14 acquired its license, registered packages, and began the full suite, but
  its log became stale at the same D4 test while CPU continued increasing. The
  contract's 180-second exact-test watchdog was met and Astra stopped only the
  task-created Unity PID. The preserved evidence is
  [R14 full PlayMode liveness result](2026-09-28-costume-cua-r14-full-playmode-liveness.md),
  SHA-256 `70E869F5A8150FC38BF6BFB23C87E60D3086BDE9239643ECBA0980FD01B36CD6`.

The current execution-baseline hashes remain unchanged:

| Path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44` |
| `CostumeUnityPresentationAdapterV1.cs` | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

## Astra acceptance addendum — R15 full PlayMode (2026-09-28)

The R15 uninstrumented, unfiltered Unity `6000.6.0f1` full PlayMode run
exited naturally with `1142/1142` passed, failed/skipped/inconclusive
`0/0/0`, duration `4890.2643413s`. The XML contains `47` fixtures, including
the complete CUA `PresentationUnity` selection `195/195`. Its XML SHA-256 is
`4EFEACC7447BD272CB361E41F91CEE18B120FFB5973AC4BE501DC4EE24A8F196`;
the log SHA-256 is
`7B0527F0C934437D50C997C59B5691C670CA03770D75D17C6491EAB0DAFA05B0`.
The [R15 execution evidence](2026-09-28-costume-cua-r15-full-playmode-evidence.md)
records PID, inventory, source hashes, and drift. Luna's
[independent postreview](2026-09-28-costume-cua-r15-full-playmode-luna-postreview.md)
(SHA-256 `287F3A0E76C4C1D2613C507E9B65B0F85BB18E4E6C75FE665FBB569C66133BEA`)
confirmed the R15 result and the preserved R11 focused PlayMode `195/195`
and full EditMode `800/800` results, all with zero failed/skipped/inconclusive.
All four CUA execution-baseline hashes remained unchanged.

As final authority, Astra accepts **`AC-CUA-009: PASS`** and the CUA
synthetic-only Unity presentation-adapter implementation as verified against
the Approved contract. The older R11/R14 no-XML attempts and the R15
reclassification remain factual history; they do not supersede this complete
R15 XML. This acceptance does **not** accept real Seryeong media, promote any
of the 36 pending catalog rows, establish a live wardrobe scene, or make a
visual-quality claim. Those remain separate downstream gates.
