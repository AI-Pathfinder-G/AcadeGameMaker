# Costume CUA final aggregate static pre-execution gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: independent static gate only; Unity was not run and no implementation file was modified.
- Approved contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`

## Exact allowlist/hash check

All supplied exact hashes match the current tree:

| Surface | Current SHA-256 |
|---|---|
| Runtime asmdef | `F4953F55EB97B770FA3DE150D3412F5AAB63B40DF0DA02DCAFAAC1A7742AB70D` |
| Runtime asmdef meta | `76A81F1EB8E2ADFD301020C9E8DF2F342826F919CF5DC2F73421FD3F0915C171` |
| Media package | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` |
| Media package meta | `5C04B4DD35436FC5C2DBA498BE036E39BA3F99FACB19252AAD536EA7423B0C8D` |
| Presentation adapter | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| Presentation adapter meta | `69E7D17B614ACAF4358535CFA3E957C2B8653794C7978F929BB78896FA6197AD` |
| View presenter | `B2BB6CFB83EF2812756ED837850C13E6135B0EFD53AE01DAB74FC196580E7587` |
| View presenter meta | `52B45E061F90817F0035749DA25A4AE000784F7866270C5A0D793CAF66AC7972` |
| PlayMode test asmdef | `65BCEC274D49AE7F3EF00F8161ED2518D6F3236DD7E3B9842936D9D8A3632D86` |
| PlayMode test asmdef meta | `1E1344F1B37134D60D05F8A75F808862D5DDB85B8E86095119432463638608F0` |
| Adapter tests | `51CF23D69FF8C4AFD78174191ECA78A31B01D47BD0765D6F279A142B96CA0DCF` |
| Adapter tests meta | `521C6E381E6138139B7D7730DE33E4589BB1E595FA1E74FF078C480C30875FC4` |
| Presenter tests | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |
| Presenter tests meta | `AD858ECF62AD1F5FE2D36D49B569835BB759676F07A08627ADB5010DBF86A2AB` |

The test asmdef path is the contract path `AcadeGameMaker.Presentation.Unity.PlayMode.Tests.asmdef`; the exact assembly name is `AcadeGameMaker.Presentation.Unity.PlayMode.Tests`. The runtime asmdef references only `AcadeGameMaker.Costumes`, `AcadeGameMaker.Costumes.IO`, and `AcadeGameMaker.Presentation`. No production file references movement/combat/input/simulation/physics assemblies.

## Closure evidence

The four former R3 findings are independently closed and their evidence files remain byte-stable:

- R3-P1-001 current tuple: `docs/verification/2026-09-27-costume-cua-luna-r4a-current-tuple-review.md`, SHA `A734F9BD3EF0FEA37160869694B565AD25AA6BB4C6FC022C5A86512209AF67BC`.
- R3-P1-002 package mutation matrix: `docs/verification/2026-09-27-costume-cua-luna-r4b4-aggregate-closure.md`, SHA `5A9AC313E3EDE99CA01DA23481E7DA6783CD88D09B341B4785FCD620077ACFCD`.
- R3-P1-003 durable replacement/projection: `docs/verification/2026-09-27-costume-cua-luna-r5-final-r3-p1-003-closure.md`, SHA `9C36FD813ACE148885943A9824F9D53C9FFD859C4BE99633771F59060833395E`.
- R3-P1-004 mechanics isolation: `docs/verification/2026-09-27-costume-cua-luna-r6-1-r3-p1-004-closure.md`, SHA `AAF12C5857E847DD2A3F53D5FBE30FF7AE2930EFBBB75F82A67276175CAB61F3`.

Together these cover the complete package mutation table, exact tuple corruption/terminal behavior, durable first/replacement/uncertain/projection boundaries, five mechanics paths, and the seven-action/two-facing/four-age matrix. REQ-CUA-001..010 and AC-CUA-001..009 remain unchanged and traceable to the approved contract.

## Static scope and compile plausibility

- No `Update`, `FixedUpdate`, `Animator`, engine time, input, global lookup, scene lookup, renderer discovery, network route, or forbidden mechanics assembly reference exists in the production allowlist files.
- Unity physics types occur only in the isolated test-only mechanics probe required by AC-CUA-005; the probe is not passed to adapter/projection/CIO.
- The R6.1 identity and Rigidbody2D/Collider oracle covers the approved source-of-truth settings and excludes only documented physics-step transients and Unity 6 legacy aliases.
- The current C# surface is plausible for Unity 6000.6/C#9: the test asmdef has the expected PlayMode test assembly name, references the approved pure/runtime assemblies, and uses no unsafe or external precompiled references.
- All positive media remains synthetic; all real Seryeong rows remain pending. No scene, prefab, catalog, real media, package, ProjectSettings, or gameplay authority is promoted.

## Execution readiness and still-missing approval inputs

Static implementation status is **PASS — P0=0, P1=0, P2=0**. However, the execution gate is **BLOCKED procedurally** because the approved proposal explicitly requires Astra to approve previously unused artifact stems before execution. Neither the contract nor proposal fixes an exact Unity `-testFilter` command or result-stem set.

Known assembly name:

`AcadeGameMaker.Presentation.Unity.PlayMode.Tests`

The focused filter still requires Astra's exact command-level approval. A safe candidate is the PresentationUnity test namespace/class cohort, but it must not be treated as authorized until recorded. Full EditMode and full PlayMode are specified as fresh unfiltered pairs.

The following stems are currently unused in `artifacts/` and are suggested, not authorized:

- `costume-cua-r7-focused-playmode` (`.xml` + `.log`)
- `costume-cua-r7-full-editmode` (`.xml` + `.log`)
- `costume-cua-r7-full-playmode` (`.xml` + `.log`)

Astra must approve the exact focused filter/command and these (or replacement) stems before Unity execution. Any source/test/meta hash drift, nonzero failed/skipped/inconclusive count, missing pair, changed allowlist, real-media promotion, or stale artifact invalidates the gate.

## Decision

**STATIC PASS / EXECUTION BLOCKED pending Astra's artifact-stem and exact-filter approval.** No product decision is required; the remaining blocker is a contract-required execution-record decision, not an implementation P1/P2.
