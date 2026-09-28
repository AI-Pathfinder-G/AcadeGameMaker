# VD-09 M5D7Q-A Terra implementation evidence — final Luna/Astra review pending

- Date: 2026-09-23
- Implementer: Terra (`gpt-5.6-terra`)
- Contract: `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- Status: **Blocked before Unity compilation, TMP static-atlas generation, authored asset creation, and test execution. This is not Verified and is not an acceptance record.**

## REQ-M5D7QA-007 source-font gate

The only candidate source bytes were independently read from the approved
temporary isolation root
`C:\Users\me\AppData\Local\Temp\acade-m5d7qa-font-20260923\extracted`.
The copied repository bytes match `AST-UI-FONT-001` exactly:

| Asset | Bytes | SHA-256 |
|---|---:|---|
| `NotoSansCJKkr-Regular.otf` | 16,433,112 | `6BCB2A0703AA137E874FC2DFFA85F6C21BA9A67FA329E81B8C801663AF7E992A` |
| `NotoSansCJKkr-Bold.otf` | 16,997,996 | `26D0C6748500A0444844280B308F5B62C7AE92AC6C6AC88148E502DD211EB52A` |
| `OFL-1.1.txt` (exact distribution `LICENSE`) | 4,301 | `6A73F9541C2DE74158C0E7CF6B0A58EF774F5A780BF191F2D7EC9CC53EFE2BF2` |

This satisfies the source-byte part of `AC-M5D7QA-008`; it does **not** supply
the required Unity import GUID, SDF, embedded atlas, or material values.

## Blocking evidence

The first required Unity invocation used Unity `6000.6.0f1` and wrote only the
admitted `font-package-smoke.log` result stem. It could not complete initial
license authentication. Its recorded facts are:

- `Connection to channel LicenseClient-me refused`;
- `Timed-out after 60.02s, waiting for Licensing to initialize`;
- `Timed-out after 60.02s, waiting for channel: "LicenseClient-me"`;
- `Licensing initialization failed after 74.83s`; and
- a subsequent reconnect again reported `Connection to channel LicenseClient-me refused`.

The full raw evidence is
`artifacts/unity-results/m5d7qa-20260923/font-package-smoke.log`.
Per the M5D7Q-A stop condition, Terra did not retry, change the font/atlas
contract, run a filtered substitute suite, or create out-of-allowlist result
files.

## Final static-atlas stop condition

After Astra established an elevated Unity invocation with a successful licensing
handshake, the `builder-pass-a` stage compiled the M5D7Q-A assemblies and
imported both exact OTF files. Static SDF creation then stopped before any SDF
asset was written. The exact failure is:

```text
NullReferenceException: Object reference not set to an instance of an object
at TMPro.TMP_Settings.get_clearDynamicDataOnBuild()
at TMPro.TMP_FontAsset.CreateFontAssetInstance(...)
at AcadeGameMaker.Hub.Authoring.Editor.HubPresentationAuthoringBuilder.EnsureSdf(...)
```

The project has no initialized TMP settings object. Creating a default TMP
resource/settings asset, using reflection to mutate TMP internals, or swapping
to a dynamic/default/fallback font is forbidden by the exact allowlist and
`REQ-M5D7QA-007`. Contract §Approval gate expressly makes failure to generate
one 1024×1024 atlas per selected face a stop condition rather than authority to
change source or atlas settings. Therefore no additional builder attempt,
scene/prefab generation, SDF artifact, or test run was made.

### Authorized single retry — 08:16 KST

After Astra confirmed no Editor was running and restarted the stale Unity Hub
and Licensing Client, one identical `font-package-smoke` command was
authorized. The fresh Unity Editor log again recorded
`Connection to channel LicenseClient-me refused`, followed by
`Licensing is not yet initialized`. This is the same authentication/channel
condition; no third launch or workaround was attempted.

### User-authorized privilege-context retry — 12:37 KST

After the user explicitly approved restarting the normal Windows-user Unity
Hub session, Astra reported a successful Hub handshake, access-token receipt,
activation response `207`, and one entitlement. The one subsequently authorized
identical Editor smoke command nevertheless again recorded
`Connection to channel LicenseClient-me refused` and then
`Timed-out after 60.01s, waiting for Licensing to initialize`. It did not reach
project compilation, font import, static-atlas creation, or any test. This is
the final permitted retry; no further recovery loop was run.

## Acceptance-criterion disposition

| AC | Disposition |
|---|---|
| `AC-M5D7QA-001` to `AC-M5D7QA-007` | Not run: Unity editor was unavailable before authored assets and PlayMode validation. |
| `AC-M5D7QA-008` | Source hashes pass; Unity SDF/atlas/material/GUID audit not run. |
| `AC-M5D7QA-009` | Not run: failure/teardown matrix requires Unity execution. |
| `AC-M5D7QA-010` | Not run: no focused, dependency-regression, or full XML-backed suites were permitted after the licensing stop. |

## Luna-ready handoff

Not ready for Luna acceptance. Luna may audit the source-byte hashes and the
license-authentication block, but independent post-review must wait for a
healthy Unity licensing client, successful static atlas generation, exact
authoring validation, and the complete required test sequence. Astra alone
retains approval/integration authority.

## Resume evidence — TMP static-SDF shader prerequisite — 2026-09-27

- Implementer: Terra (`gpt-5.6-terra`)
- Contract SHA-256: `DB4315900D5BC02955BB2979257713B369788CE687A8ED66C4A5E5DC7DEA2019`
- Scope: `REQ-M5D7QA-007`, `REQ-M5D7QA-009`; blocking `AC-M5D7QA-008`
- Status: **Blocked. Not Verified.** This section supersedes only the earlier
  statement that Unity compilation was unavailable; it does not alter the
  original licensing facts.

The elevated Unity `6000.6.0f1` batch runs now completed licensing handshakes
and compiled the M5D7Q-A runtime/editor/test assemblies. The source-controlled
canonical `Assets/UI/Resources/TMP Settings.asset` passed its typed precondition
through the public `Resources.Load<TMP_Settings>("TMP Settings")` path and was
not modified. Both exact Noto OTF assets imported. No SDF, atlas, material,
`HubMenuRoot.prefab`, or `Hub.unity` asset was written.

The builder then deterministically stopped at
`TMP_FontAsset.CreateFontAssetInstance` before static SDF creation:

```text
ArgumentNullException: Value cannot be null. Parameter name: shader
at UnityEngine.Material..ctor(UnityEngine.Shader shader)
at TMPro.TMP_FontAsset.CreateFontAssetInstance(...)
at AcadeGameMaker.Hub.Authoring.Editor.HubPresentationAuthoringBuilder.EnsureSdf(...)
```

The first recorded run is `builder-pass-b.log`; the fresh-process confirmation
is `builder-fresh-process.log`. The confirmation omitted forced `-nographics`
and reached the identical exception, so the fault is not a NullGfx-only
artifact. Neither run imported or created TMP Essential Resources, a shader
asset, a fallback/default font, or any out-of-allowlist file.

Read-only uGUI 2.6.0 package-source inspection establishes the contract gap:

- `Runtime/TMP/TMP_FontAsset.cs:663` constructs the SDF material from
  `ShaderUtilities.ShaderRef_MobileSDF`.
- `Runtime/TMP/TMP_ShaderUtilities.cs:146-156` resolves that value only by
  `Shader.Find("TextMeshPro/Mobile/Distance Field")`.
- The package cache contains the needed runtime resource surface only through
  `Package Resources/TMP Essential Resources.unitypackage`; its importer calls
  `AssetPackage.Package.Import` in
  `Runtime/TMP/TMP_PackageResourceImporter.cs:20-68`. The only directly visible
  `.shader` file outside that package is the editor-only
  `Editor Resources/Shaders/TMP_SDF Internal Editor.shader`, which is not the
  requested mobile distance-field shader.

The Approved allowlist expressly forbids TMP Essential Resources and permits
only the canonical TMP Settings resource. Importing that package or admitting a
shader/material asset would expand the contract. Terra therefore made no
workaround and paused before `AC-M5D7QA-001` through `AC-M5D7QA-010` execution.
Sol/Astra must decide whether to amend the contract with a bounded, auditable
shader provenance path; Luna post-review remains pending.

## Resume evidence — fixed-atlas glyph-capacity stop — 2026-09-27

- Implementer: Terra (`gpt-5.6-terra`)
- Contract SHA-256: `692A15E859A1B81068392C5C30A70A70EF272DDF37B90DD1F1632DBB402326F5`
- Scope: `REQ-M5D7QA-007`, `REQ-M5D7QA-009`; blocking
  `AC-M5D7QA-008` and therefore `AC-M5D7QA-010`
- Status: **Blocked. Not Verified.**

After Astra approved the narrow Mobile SDF amendment, Terra added exactly the
two permitted byte-locked uGUI shader files, their four fixed metas, and the
exact uGUI notice. All canonical source prerequisites remained unchanged:

| Asset | SHA-256 |
|---|---|
| `Assets/UI/Resources/TMP Settings.asset` | `71A6F2477A384F5D12D27AC22F6C30449ED87AA21FD0906FB119442FEAEECBD4` |
| `TMP_SDF-Mobile.shader` | `44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4` |
| `TMPro_Properties.cginc` | `66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90` |
| `third_party/unity-ugui-2.6.0-LICENSE.md` | `3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6` |

The elevated Unity `6000.6.0f1` `builder-pass-b` completed the immutable
shader preflight, found the canonical `TextMeshPro/Mobile/Distance Field`
shader, and entered the static font path. It then rejected the contract's
exact 138-glyph population under the fixed settings (sampling point size 90,
padding 9, `SDFAA`, one 1024×1024 atlas, multi-atlas false):

```text
InvalidOperationException: Static glyph population failed: 하항했
at HubPresentationAuthoringBuilder.EnsureSdf (...:52)
```

`하`, `항`, and `했` are the final three required, sorted Hangul code points
in `AST-UI-FONT-001`; none may be removed or substituted. The command and full
stack are recorded in
`artifacts/unity-results/m5d7qa-20260923/builder-pass-b.log`.

The failed Unity call temporarily created only
`Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset` and its `.meta`.
Contract amendment cleanup authority permits removal of an SDF target created
by that same failed invocation. Terra verified those exact two paths and
removed only them. No SDF, atlas, material, `HubMenuRoot.prefab`, or
`Hub.unity` remains; no Essential Resources import, glyph-inventory change,
font-setting change, second shader, or unrelated-path mutation was made.

This is a new contract/product blocker: the approved single-atlas parameters
cannot currently populate all required glyphs through the specified public TMP
path. Terra stopped without retrying or changing glyph inventory, sampling
size, padding, render mode, atlas size, or multi-atlas policy. Sol/Astra must
provide a new bounded amendment before authoring or test execution resumes;
Luna post-review remains pending.

## 2026-09-27 resumed implementation / verification record

- Scope: `REQ-M5D7QA-002`, `REQ-M5D7QA-007`, `REQ-M5D7QA-008`, `REQ-M5D7QA-009`;
  `AC-M5D7QA-002`, `AC-M5D7QA-008`, `AC-M5D7QA-010`.
- Status: **Blocked. Not Verified.**

The Approved 2048 amendment was implemented with pair-in-memory validation,
Static-state serialized source-GUID validation, and persistent atlas/material
subassets. `builder-pass-e.log` completed successfully; both roots contain the
font root, Alpha8 atlas, and material subassets after save/refresh/reload.
The focused retries passed: EditMode `3/3` and PlayMode `3/3`, each with zero
failed/skipped/inconclusive. Earlier failed/recovered logs remain immutable.

Direct M5D7O verification initially appeared unable to begin test execution. Both the initial
`regression-m5d7o.log` and contract-authorized warm-cache
`regression-m5d7o-retry.log` froze at Unity Test Framework global
`Unity.PerformanceTesting.Editor.TestRunBuilder` `IPrebuildSetup`, before an
XML result or any fixture output. Each was observed for seven minutes with
the same-run headless Editor and its asset worker responsive and CPU-advancing,
then only those two processes were terminated. No package, project-setting, or
prebuild bypass was applied. Historical timing then established that the
fixture is silent for about 14--15 minutes. The approved 25-minute recovery
`regression-m5d7o-longrun.xml` completed in 914 seconds with `19/19` passed
and failed/skipped/inconclusive `0`; the seven-minute terminations were
undersized timeouts, not proven hangs. That historical record is retained; the
following final record supersedes only its then-pending execution disposition.

## Final Terra implementation and execution record — 2026-09-27

- Implementer: Terra (`gpt-5.6-terra`)
- Approved contract SHA-256: `5206546D4140C38A567598D99BF859034ED806EDC062B466ABDD49DF2AE06F75`
- Status: **Implemented evidence complete; final Luna/Astra review pending. This
  record does not change the Approved contract to Verified.**
- Canonical package: [`final-manifest.json`](../../artifacts/unity-results/m5d7qa-20260923/final-manifest.json),
  [`scope-before.json`](../../artifacts/unity-results/m5d7qa-20260923/scope-before.json), and
  [`scope-after.json`](../../artifacts/unity-results/m5d7qa-20260923/scope-after.json).

Final generator SHA: `8DB852A5D6CAF1DA4883ED3D27DEB909CA5B877DA4D6BA10246E9CAE934E0C90`.
Final focused authoring-test SHA: `D7A41D0B6F4B6F703D02BC6CD0DF5316DFB95B338128A749928D2CAAA0A13208`.
The Canvas-channel amendment SHA is `34D05E3E92668D97EB916C38FE770F1EB16AA80E22435C9ADCB3A479D916E600`.
Luna passed the exact-SHA implementation pre-gate (`7FB1FA46534F013F574B037CE4467CA0F5418F4C6B7555F781EF52AA89AE0CAF`),
focused evidence review (`CCE4D82D2F2F540D4E43029ED3A973D03B8512048A345DB32102994C0E18B776`),
and retry-r capture review (`44C53F9820E18E6A2BED23E8729D48D263801298C038ECEF4F933CC4B1DE2681`).

### REQ / AC evidence map

| Requirement / AC | Terra evidence and disposition |
|---|---|
| `REQ-M5D7QA-001`, `REQ-M5D7QA-009`; `AC-M5D7QA-001`, `AC-M5D7QA-002` | Scoped builder/validator transaction and exact scene/prefab/GUID evidence are in the final manifest. `HubMenuRoot.prefab` SHA is `66BC499B334FE3EC3F62ECCB5171425907C381B5EE5F78F5BC7081EE47D28740`; `Hub.unity` remains `04F3F0EA73C40A964DCE93766CF21AFE3F57DC20912AEAB86ABDADB239BC9C6A`. |
| `REQ-M5D7QA-002`–`005`; `AC-M5D7QA-003`–`006` | Final focused layout EditMode `51/51`, focused PlayMode `4/4`, M5D7O `19/19`, M5D7P-A `5/5`, M5D7Q0 `4/4`, and M5D7N long-run `69/69` all passed with failed/skipped/inconclusive zero. |
| `REQ-M5D7QA-006`; `AC-M5D7QA-007` | retry-r records the real-scene independent-oracle capture: exactly 80 Canvas channel observations, every A/B × ten spec sequence `None(0) → TexCoord1,Normal,Tangent(25) → 25 → None(0)`, all no-failure. |
| `REQ-M5D7QA-007`; `AC-M5D7QA-008`, `AC-M5D7QA-010` | Static SDF evidence is retained from the approved 2048 flow. Final full suites passed: EditMode `788/788`, PlayMode `947/947`, each failed/skipped/inconclusive zero. |
| `REQ-M5D7QA-008`, `REQ-M5D7QA-009`; `AC-M5D7QA-009`, `AC-M5D7QA-010` | Channel focused EditMode passed `61/61`; independent missing/extra bits, Canvas profile drift, post-TMP/readback faults, and primary-plus-cleanup aggregation restore `None(0)`. Both capture processes record 120 TMP observations, all `failures=NONE`, and terminal/global restoration. |

### Capture publication and process-boundary proof

`resolution-capture-retry-r.log` SHA-256 is `7B3456857989B9BBDB6A73DA179136F4E71958D7B9A058F6527D07AE8B4551E9`.
It exited successfully, emitted `M5D7QA_SAME_PROCESS_CAPTURE|passes=2|png=10/10|manifest=true|equal=true`,
and completed clean-bootstrap restoration with `persistentHashesEqual=true|globalsRestored=true`.
The final set has exactly ten PNGs plus canonical UTF-8/LF `manifest.json` SHA-256
`374AE7CA4169BB2C08FE08E9F18DC2E657B78773A4ECE57A33808984CD9E80C2`; no `.tmp` or `.prev` remains.

The fresh process wrote `resolution-capture-fresh.log` SHA-256
`92071AE7CE7EA8A280440C74EB4A3A7CCA96E4FC3854199638A1B57426F4495E` and also exited successfully.
Its 80/120 observations were all no-failure; StagePublish checked the existing final, and all ten PNG plus manifest bytes remained identical to retry-r.

### Immutable failure history and GPT participation ledger

Earlier license, shader, atlas-capacity, Unity-6000 compatibility, camera/RT, layout, and PlayMode-fixture attempts are preserved as immutable evidence, not accepted results. retry-p and retry-q remain diagnostic failures; retry-r is the first admitted capture success. The full list is in `final-manifest.json`; no prior log or XML was overwritten.

| Participant | Actual role and model | Concrete output / independence boundary |
|---|---|---|
| Astra | Orchestrator, approver, integrator — `gpt-6-astra` | Approved bounded amendments, execution order, result stems, and final-review handoff; did not self-accept Terra implementation. |
| Sol | Bounded contract/counter-review contributor | Supplied historical design/amendment counter-review when requested; has no final approval authority. |
| Terra | Implementation/execution owner — `gpt-5.6-terra` | Implemented the permitted tools/tests, ran admitted Unity commands, preserved logs, and assembled this record. |
| Luna | Independent QA owner — `gpt-5.6-luna` | Independently pre-gated exact source SHA and reviewed channel focused/retry-r capture evidence; final post-review remains pending. |

## Final Luna postreview and Astra integration — 2026-09-27

Luna's independent [final postreview](2026-09-27-vd09-m5d7q-a-final-luna-postreview.md)
SHA-256 `B73F2AA693244B141244EE2F34EB12780DB059674BE057A07F5ED191019F3E67`
returned **PASS — P0=0, P1=0, P2=0**. Its direct
[retry-r visual review](2026-09-27-vd09-m5d7q-a-resolution-capture-retry-r-visual-review.md)
SHA-256 `F257E092DDD3A9B30AA856DCCF466D9C4AAB4BE66FED5598019E4023302B07D5`
also returned P0/P1/P2 zero.

Astra then performed final integration and promoted the contract to **Verified**.
The execution evidence above was generated under the then-Approved contract SHA
`5206546D4140C38A567598D99BF859034ED806EDC062B466ABDD49DF2AE06F75`;
the current integrated Verified contract SHA is
`91C1B9E5A1F1294C14DFFB63623914FDA58BB9BD7C0EB1E3220C691F531B8D32`.
This status update records Astra's authority decision; it does not revise the
historical execution inputs, Unity logs, captures, or Terra's independent-ownership boundary.
