# VD-09 M5D7Q0 Hub-UIOnly router graph — Terra implementation evidence

- Date: 2026-09-20
- Implementer: Terra (`gpt-5.6-terra`)
- Contract: `docs/specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md`
- Status: Verified. Terra implementation and current-source Editor gates are
  complete after the deterministic authoring repair; Luna independently
  returned `ACCEPT` (`P0=0`, `P1=0`, `P2=0`) and Astra accepted integration on
  2026-09-23. Full PlayMode 943/943 predates the final Editor-only revision and
  is reused under Luna's documented impact ruling, not claimed as rerun.

## Bounded implementation record

- `REQ-M5D7Q0-001..005`: `InputRouter` adds a false-default HubUIOnly
  discriminator and a prepared-lifecycle-only, zero-consumer UI branch. It
  retains one generated wrapper/callback owner and does not dereference player,
  transfer, combat, terminal, or camera dependencies in that branch.
- `REQ-M5D7Q0-003` / `AC-M5D7Q0-003,007`: the branch owns local tick and frame
  ordinal successors with independent exact proofs; it constructs the semantic
  frame before consuming a pending UI batch and publishes the receipt last.
- `REQ-M5D7Q0-006,008` / `AC-M5D7Q0-005,007,012`: hub command rejection occurs
  before gameplay dereference. Pre-confirmation graph corruption closes through
  the existing prepared failure row; post-publication corruption retains
  forensic receipt/frame fields but invalidates reads, disables maps, and
  defers exact adopted-action closure until hub Disable/Destroy.
- `REQ-M5D7Q0-009` / `AC-M5D7Q0-008`: the authoring builder leaves an already
  valid canonical asset untouched. Its repair path writes one fixed LF UTF-8
  YAML graph with stable local IDs, synchronously imports it, and then invokes
  the observational validator. The validator requires the exact asset path,
  root/component order, enabled sibling cohort, typed identities, and null
  graph matrix.

## Authoritative Q0 current path and hash manifest

The original pre-gate workspace baseline was already dirty and cannot be
reconstructed from `HEAD` without attributing unrelated user/Unity work to
Q0. The following explicit manifest is therefore the authoritative current Q0
delta record, derived from the Approved allowlist and the actual staged Q0
inventory. The Q0 scope audit consumes these exact path/hash rows; it does not
substitute a synthetic before/after sample or a folder wildcard.

| Changed source path | Current SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `6C1BBAA1F9AB4000F492233E96AC69D6B93DD5B32378F58C9EC92F13C4EA8341` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs` | `D1768821D6B635AD1B515FCEB326BC58C2F92E1551D640B2BAA268F9983181F1` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | `3A3110407949A42FA172EBF0A14F4508364FF1C1D6B739B3EF62246912F08A3D` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs` | `1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs` | `E633D2FEB8C0D51894E76B9067306B7E483B670AC391CBB55C8B449960FBBF1E` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef` | `22B3D54327009D66AA04ADD30ACEF1629F6AD7F8A8CBF465C667A42AB9865A9A` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs` | `67F8C19E30E0351CB725493E7FB3D794E6EBB5786EC8851660B3525AEA9F1320` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs` | `3E91A2741EC5BA106DE5703167D52ACAD2AA930BAFD54C0F00368246AA41CD5E` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs` | `C88C27BC316192925F652017F4251E13EED86AE5F200358B48FF8A4714BE6067` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef` | `1ED81F65C60ACA26C6BEDE2C76DE2B81A3A08FD1D22544E58EC41AF76677BA72` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs` | `E853B2A777B86E9F70AF4B06FED7427DB7A9DF0710A1BD3F3B311EAD3F1D122D` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` | `23BE8E126687609321C7E26968ECC2F552C2AD8AE96E6F1FDDA2A04A5E30A8F8` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyInputRouterPlayModeTests.cs` | `2CCBF5CE34A541E6012CCB48086353356D71D766C5BED68C43018AA1BE0FFE1F` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0CoverageBPlayModeTests.cs` | `8EEDFA700F94B0101A1C4AF89A5D26905C5A8F95B42781935D6A6BD3832328A5` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0HandoffMatrixPlayModeTests.cs` | `1AF86650F070896A77D33453E76C346ECAA04CAEDAB62D5E192214097CFA6667` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs` | `6899830A32D452F8C82BC18697E1B9744C2C050EAC7A89A856DE6240EB9C5D1F` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0TestSupport.cs` | `404B0E2D123EC9430A152D602DF302BB943EB1B772133519E62CCB1A0FCFD09B` |

The exact Q0 metadata paths are `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`,
the four individually named `HubAuthoring` source metadata files, `Assets/Prefabs/Hub.meta`,
`Assets/Prefabs/Hub/HubRuntimeRoot.prefab.meta`, and the seven individually named
focused-Q0 test metadata files. Documentation is restricted to this evidence,
the matching contract, proposal, and pre-gate record. No scene, package,
generated-input, profile, gameplay, or wildcard metadata path is admitted.

`AC-M5D7Q0-011` now reads the actual deterministic Git porcelain inventory
(changed and untracked paths) and emits every row as either an exact named Q0
path or `PREEXISTING_DECLARED_EXTERNALLY_OR_UNATTRIBUTABLE`. It admits no Q0
folder/prefix/glob inference. The first-turn dirty-worktree inventory was
captured outside this repository session, so a cryptographically authoritative
pre-Q0 baseline cannot be reconstructed from `HEAD`; the latter classification
therefore records the current non-Q0 paths without fabricating a clean-baseline
or before/after claim.

## Static checks, execution record, and pending verification

- `git diff --check` was clean for the M5D7Q0 allowlist. A pre-existing,
  unrelated trailing-space warning in `ProjectSettings/ProjectSettings.asset`
  was observed and not changed.
- Canonical builder/idempotence/observational-validator (post-compatibility
  repair): `HubRuntimeAuthoringBuilderEditModeTests`, 29/29 passed, duration
  `3.8373859`, run 2026-09-22 KST. Result XML SHA-256
  `66AA350863B39E01A052DD33BFF1BCB349CB360E6E24EE6355CA2669740D1054`;
  log SHA-256 `0CF8FC5A72FB5EBD83931FAF284503DD989470CF840DA870FA6A198ED442E25F`.
  This run generated/rebuilt the canonical `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`
  (SHA-256 `F5B0B347AFFF52821594B449B4184464C9DFE426ADABEF2D42B5F73C7CEC5720`)
  and observed its stable metadata (`HubRuntimeRoot.prefab.meta` SHA-256
  `B4C188F55CD7DA32649008AB5FA84CC434C04FF10BF64F1AB7C31071063D86AE`),
  parent `Hub.meta` (`772772C20B2021B4094ACD0600624F13714D7EB3F8E7E8BB93FF0FBC5F1F6749`),
  and `HubAuthoring.meta` (`A3539C6B6B0594852E4C779DF110B0E20B092A108FB8FDCB4429EC798DF6B5FC`).
- Scope audit/idempotence guard (post-compatibility repair):
  `HubUiOnlyQ0ScopeAuditEditModeTests`, 4/4 passed, duration `0.469402`,
  run 2026-09-22 KST. Result XML SHA-256
  `7E2106EB8170B76AAB310B446055B87D9185DBAFAC76156130B602AFA3FF0F60`;
  log SHA-256 `290BD64D3BB2C0B0D3AECFD4CC3D1DC34E65D417024EFCBF5E7D5330D501C2F5`.
- Focused Q0 PlayMode completed before the compatibility repair: router
  38/38, duration `400.3398361`, XML `0D140EB41DBA0D8B5C1F78A559781347823F234D572BCB1D1887BE26C73D0793`,
  log `019C45CDEC96D4B65B7716026CF743A2A9A956B1DA7A8AC4CD409D67531F3CAE`;
  Coverage B 27/27, duration `544.6480092`, XML
  `2EB3BF7A5D820200030B6CD7990543760D83761F3590AC77E0114B86B7FD5CFA`,
  log `7963C3CE7D89D60C1DADF988511A7F14165823A1A9BDA1A907D55DD0F66E5E9F`;
  handoff matrix 5/5, duration `335.8243712`, XML
  `D4DBE7EC4845FA9528457A47633F20E0A28C54E38C0BAB8FC03F22EFBAE618BE`,
  log `A9264EB3C0FDF9B38219EA8134E8CE938CFAD58A69E0C71249E56EC5F8C1FFF9`;
  remaining runtime 18/18, duration `365.0642638`, XML
  `C39CFA2DFBBFB4A7CADC4A672BD09A9CDE747CE5624E0FB6298DD7B851886420`,
  log `0A19551195D8A69DE1F098BFA00BF198AA5CCFC7C8A4816225DD1F3FDB0539B3`.
  These are useful evidence for REQ-M5D7Q0-001..009 / AC-M5D7Q0-003,005,007,008,012,
  but do not replace the required independent current-state Luna rerun.
- Direct regression checks completed before the compatibility repair: M5B5
  `InputRouterPlayModeTests`, 3/3, duration `0.1698399`, XML
  `6AC3BF514D5332BDD9EB041104B8C352C82BF0DCBCEFC7F364CBCC95838E88FF`,
  log `A401036A584198584141CCE2CCA8E7CF68B055EDCFD1F67A42772850E594D102`;
  M5D7P-A `UiSemanticFrameRouterPlayModeTests`, 52/52, duration `0.3372591`,
  XML `3E7DA8C389A8E62B9D4B438C76A9F8A666E52994CC303A86B52D412CF23D1405`,
  log `B28B7EF5901EEEB0EEA4F992C6EDB381300051D74BC1BC7BE4070DA5A50BC903`.
- M5D7M filter diagnostic r1 used the obsolete namespace
  `AcadeGameMaker.Tests.PlayMode.InputUnity.DesktopProfileLaunchAdapterV1Tests`
  and found zero tests. Its XML (`BD71555E84DA6422AEBAB4B8F1E6CB96B3D5C335A3FBFE4686B45F85E071A4A4`)
  and log (`F8A7AF22B5966A9866B2578E73F9392E073E519D20D6BFA0152A8A512E0CBD12`)
  are explicitly invalid diagnostics and are not pass evidence.
- The corrected M5D7M r2 filter identified two regressions: its expected
  disposed-held-action containment did not throw/contain (`REQ-M5D7Q0-010`,
  `AC-M5D7Q0-001`), and its pre-UI-registration injected callback fault did
  not execute the historical cleanup observable (`REQ-M5D7Q0-010`,
  `AC-M5D7Q0-001`). It failed 2/117 (115 passed), duration `1023.7978933`,
  XML `B8D175CEB04726F4670D3EE8889B3708B305E87F73C9F88120EDDDCCA249BAC4`,
  log `C18A2361C6DE474686EA006A1FAFF72B7B25E40A70CAFE29558DDC9C4E867D23`.
  The allowlisted `InputRouter` repair restores malformed already-disposed
  containment via `ValidatePreparedInvariantOrClose()` and restricts the
  legacy synthetic unsubscribe witness to the non-Hub injected fault stage;
  the HubUIOnly branch remains witness-driven.
- Corrected M5D7M r3 (post-compatibility repair):
  `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests`,
  117/117 passed, failed/skipped/inconclusive 0, duration `925.9375703`,
  completed 2026-09-22 00:47:19 KST. Result XML SHA-256
  `EE24DE4B1965E8D850A3FBD484598DFB3E8C9D3E84F1D206B8A9DA9E2F431566`;
  log SHA-256 `E427FBACBB25AFCAB1C783699F6C7B38B47AB1F040312BBF97643AC125EE3601`.
- M5D7N r1 was launched once after the valid M5D7M r3 pass, but Unity exited
  `198` before test discovery/result creation at 2026-09-22 00:48 KST. The
  log records unavailable access token, no matching free entitlement, and
  `No valid Unity Editor license found. Please activate your license.` Log
  SHA-256 `BE8A4C8D16EDEF2EEE9A450431A2DAE1B88B9E5ECCC60193CD97FC0D039BF470`.
  No XML was produced, no retry occurred, and this is a Unity authentication/
  license blocker rather than a test result.
- After the user's Unity re-login report, one smallest non-destructive
  `-batchmode -nographics -quit` entitlement probe ran on 2026-09-22
  05:17--05:21 KST. It did not establish a healthy entitlement:
  `LicenseClient-me` was repeatedly refused, each reconnection timed out after
  60 seconds, and Unity reported the licensing connection was lost. Because
  this probe also used `-quit` and reported unavailable
  `com.unity.editor.headless`, it is an invalid entitlement proof and does not
  establish that `-nographics` alone caused the failure. The probe was bounded
  and its exact process was stopped at 05:21:14 KST. Probe log SHA-256
  `C30135D7682BFA067FBA2C9E1572DD83B03FD70ACCCD9B03F399AF349FE8FB10`.
- To distinguish that probe from a valid test invocation, M5D7N r2 was then
  launched once with the exact successful M5D7M r3 shape (`-accept-apiupdate`,
  `-batchmode`, `-nographics`, explicit project path, and PlayMode test
  runner), using the correct `HubEntryHandoffLatchV1Tests` namespace/filter.
  It again reached a refused `LicenseClient-me`, 60-second initialization
  timeout, and lost-connection reconnection before test discovery. It produced
  no result XML and its single owned Unity process was bounded/stopped at
  05:28:14 KST; no further retry or suite invocation occurred. M5D7N r2 log
  SHA-256 `AF84A82503910580CC8F006A2F1D4B845E9EAF8F76D7C7876EB941E0950D6DEA`.
- Following a controlled restart of only Unity Hub and Unity Licensing Client,
  the fresh Hub record reported a successful licensing-pipe connection and
  activated Unity Personal/SameMachine entitlement. M5D7N r3 was then launched
  once with the same known-valid M5D7M test-run shape and correct filter.
  Despite that Hub state, the batch Editor again recorded refused
  `LicenseClient-me`, a 60-second initialization timeout, and a lost licensing
  connection before test discovery. It produced no XML and its single owned
  Unity process was bounded/stopped at 05:32:09 KST; no full suite began.
  M5D7N r3 log SHA-256
  `58974CE54B9C49010834CFC152776CB730D9791DA23792343173E59D418676DC`.
- A real GUI project initialization then completed with Unity Personal
  entitlement resolution and an updated access token before its initialization
  Editors were closed. M5D7N r4 was launched once afterward with the same
  known-valid M5D7M test-run shape and correct filter. The batch Editor still
  reached refused `LicenseClient-me`, 60-second initialization timeout, and
  lost connection before test discovery. It produced no XML and its single
  owned Unity process was bounded/stopped at 05:37:11 KST; no full suite began.
  M5D7N r4 log SHA-256
  `1592D79BCE627E64A0200A708913D40CB6CC1AB1D5C1F1771A138D0528E52F23`.
- Final materially distinct diagnostic: M5D7N r5 retained the same explicit
  project path, batch test runner, PlayMode, correct filter, result and log
  arguments, but removed only `-nographics` (and retained no `-quit`) to test
  the graphics-context batch path. It still recorded refused
  `LicenseClient-me`, a 60-second initialization timeout, and lost connection
  before test discovery. It produced no XML and its single owned Unity process
  was bounded/stopped at 05:40:15 KST. Automated Unity retries are now closed;
  no full suite began. M5D7N r5 log SHA-256
  `1B2AC9CCF82BAE4AEC99FACEC22EA8960EE14CB08D455A68A9905FF7278FCBC6`.
- Reboot-resumed current-source verification was authorized with persistent
  result root `artifacts/unity-results/m5d7q0-20260922-reboot/`. Its baseline
  records 33 exact Q0 source/prefab/meta/test hashes and a dirty-worktree
  inventory commitment (374 porcelain records; it makes no clean-baseline
  assertion): baseline SHA-256
  `7B85E6F4C341FA27E522493232ABEB806DEA4302EA4A6C302F631918CCC8DEC0`.
  The first required current-source gate, focused EditMode builder/validator,
  was launched once after reboot but blocked before test discovery on the same
  `LicenseClient-me` refusal, 60-second initialization timeout, and lost
  connection. No XML was generated; its owned Unity process was stopped at
  07:23:07 KST. The preserved log SHA-256 is
  `5536C44158279FBA73A7E91A375AFC2AC28946B198016CBB08F9FC5F335536CA`.
  The final manifest records this stop, all not-run gates, unchanged current
  33-path Q0 hash commitment, and unchanged dirty inventory: SHA-256
  `920191635F9900E3D987663BFD5CBE97B8A144ED5FD7E439374EB87ED2236941`.
- Astra then authorized one additional preservation-safe GUI result stem so the
  prior batch log remained intact. The GUI test-run omitted `-batchmode`,
  `-nographics`, and `-quit`, but retained the same explicit project, EditMode,
  builder filter, XML, and log arguments. It reached Direct3D graphics
  initialization but still failed before test discovery with refused
  `LicenseClient-me`, 60-second initialization timeout, and lost connection.
  No XML was generated; its owned Unity PID was stopped at 07:28:08 KST. GUI
  log SHA-256 `476A333031C527D8FF2491A04E80AD25F2C01DC11AE656E1EA706E6F0E1A69A9`.
  All automated Unity runner paths authorized for this resumed run are closed;
  `final-manifest.json` was updated to preserve both launch shapes and hashes.
- A final isolation run closed Unity Hub and the Hub-owned licensing client so
  the Editor could launch its bundled `Unity.Licensing.Client.exe`. The Editor
  launched PID 1996 from its 6000.6.0f1 licensing-client path, connected to
  both `LicenseClient-me` channels, and reported version `1.18.3+d7ffd15`.
  It then failed before test discovery because the access token was unavailable
  and entitlement lookup returned 404 with zero matching free entitlements;
  Unity exited `198`. No XML was generated. Preserved bundled-client log
  SHA-256 `05C27CF425E96BFA931A9CCAFDB1930845632E81F605D6A0C9E43F5F73CB14DE`.
  This distinguishes the remaining blocker as unavailable Editor entitlement,
  not Hub IPC, project code, or test failure.
- After a later Hub recheck reported three entitlement groups, free-tier
  eligibility, and repeated Unity Personal/SameMachine activation, the
  separately authorized reactivated builder gate ran once on current source.
  The batch Editor nevertheless again recorded refused `LicenseClient-me`, a
  60-second initialization timeout, and lost connection before test discovery.
  No XML was generated; the owned Unity process was stopped at 23:08:10 KST.
  Preserved reactivated-run log SHA-256
  `05B0E40F0DCA3C356F7692CABD4394ED5B08B323C1F9D8768532B68504C7527C`.
  This did not create a functional test failure and no subsequent gate may run
  under the required ordered sequence.
- Unity Hub 3.21.3 was then repaired through its official signed installer
  (installer exit `0`; installer SHA-256
  `ee8e5ae4153201b87b3813c96b00f1f0397d1558597a941a9ea9fe538f52e5a3`),
  followed by fresh free-tier and Unity Personal/SameMachine activation. The
  separately authorized repaired builder run still recorded refused
  `LicenseClient-me`, a 60-second initialization timeout, and lost connection
  before test discovery. No XML was generated; the owned Unity process was
  stopped at 23:18:29 KST. Preserved repaired-run log SHA-256
  `994019C7CEDA51B7F6A1E3AA47989B341AFBDFAC9207A326AADE69323D06E38E`.
  Per the stop condition, no Editor reinstall or further automated runner
  workaround was attempted.
- Official Unity CLI `1.0.0-beta.8` subsequently reported healthy auth/license
  diagnostics, so the approved `unity.exe test` route was tried once with the
  current-source EditMode builder filter, persistent XML output path, and a
  300-second timeout. It passed a Hub session to the Editor; the Editor then
  attempted to launch a licensing client but reported that the global client
  mutex was already owned. No XML was emitted, and the CLI/Editor processes
  exited at the configured 300-second timeout. The CLI has no log-file option,
  so the preceding repaired batch log remains preserved; the wrapper detached
  from the invoking shell, making its numeric exit code unobserved rather than
  inferred. This is a runner timeout before test discovery, not a functional
  test failure; no retry or Editor reinstall followed.
- After Astra stopped and restarted only Hub and Hub Licensing Client, the
  fresh single client (PID 27116) was reported CLI-auth fresh with a valid
  Personal ULF/assignment. The official CLI route nevertheless reproduced the
  same pre-discovery failure: the Editor refused the existing
  `LicenseClient-me` channel, attempted a second client (PID 12348), and
  reported failure to acquire the global `Unity-LicenseClient-me` mutex because
  another client was already running. No XML was generated; CLI and Editor
  again exited after the configured 300-second limit. The wrapper detached, so
  the numeric exit code is unobserved rather than inferred. This classifies the
  remaining failure precisely as global licensing-client mutex/channel startup
  contention, not a functional test result; no third CLI call followed.
- In the final Hubless CLI-only isolation, all Unity Hub, Hub Licensing Client,
  and Unity Editor processes were verified absent before one official CLI
  builder/validator invocation. The Editor found no existing
  `LicenseClient-me` channel, launched its bundled client (PID `34312`) from
  `C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client.exe`,
  and connected successfully on both licensing channels (client version
  `1.18.3+d7ffd15`). It then reported unavailable access token and an
  entitlement lookup HTTP `404` with zero entitlement groups and zero matching
  free entitlements, followed by `No valid Unity Editor license found` before
  test discovery. The CLI completed with exit `198` and
  `EDITOR_LICENSING_FAILURE`; no XML was generated, and no Unity/licensing
  process remained after completion. The CLI exposes no log-file option, so
  the preserved repaired batch log was not overwritten. This bypasses the
  prior mutex contention but confirms missing Editor entitlement in the
  CLI-only process context; no retry or code/test change was made.
- An official forced repair then installed a separately signed Editor at
  `C:/Program Files/Unity/Hub/Editor/6000.6.0f1-x86_64/Editor/Unity.exe` and
  Hub registered that copy as its default. One official CLI builder/validator
  invocation explicitly selected that new path with `--editor-path`, while the
  fresh Hub Licensing Client PID `32464` remained present. The new Editor
  (`6000.6.0f1 (f7f8ed4d1e24)`) again refused `LicenseClient-me`, attempted to
  launch bundled Licensing Client PID `23748`, and recorded failure to acquire
  global mutex `Unity-LicenseClient-me`. After the approved 60-second bounded
  diagnostic produced neither test discovery nor XML, only that invocation's
  CLI PID `10508` and Editor PID `9072` were stopped; Hub's Licensing Client
  PID `32464` remained running. The CLI exposes no log-file option, so the
  existing repaired batch log remains unchanged (SHA-256
  `994019C7CEDA51B7F6A1E3AA47989B341AFBDFAC9207A326AADE69323D06E38E`).
  This single new-Editor attempt has no observable process exit code because it
  was deliberately bounded; it is a pre-discovery licensing mutex failure, not
  a functional test result. No retry, code change, or test change was made.
- Astra then authorized one final Hub-mediated launch shape. Official Unity CLI
  `open` explicitly selected the parallel Editor, forwarded the complete
  builder/validator EditMode runner arguments as one `--args` string, and
  returned exit `0`; its dedicated `focused-edit-builder-hubopen.log` was
  created (SHA-256
  `7C13BB71CE0E84FF54131AB1D4E4A2FB7C7EC2086F61E6E23C1F6A37C4C1D2BE`).
  Thus Hub accepted and relayed the open request. The resulting Editor PID
  `26584` still refused `LicenseClient-me`, timed out twice for 60 seconds,
  launched bundled clients PIDs `35380` and `20804`, and reported unavailable
  `com.unity.editor.headless`; no test discovery or XML occurred. At the
  approved 180-second limit, only this invocation's new-Editor processes
  (Editor `26584` and verified same-install helpers `1748`, `22092`, `34712`,
  `35744`, `3040`, and `32976`) were stopped; PID `32976` was the same-install
  DotNetSdk `dotnet.exe` helper found during cleanup verification. Hub
  Licensing Client PID `32464` was
  preserved. The Editor exit code is unobserved because that isolated run was
  intentionally bounded. This is a licensing initialization/mutex failure,
  not a test result; no retry, code change, or test change followed.
- The execution-boundary diagnosis was then confirmed: when the official Unity
  CLI test runner and explicit parallel Editor were launched once in the
  elevated normal-user environment shared with the healthy Hub Licensing Client
  (PID `32464`), the current-source
  `HubRuntimeAuthoringBuilderEditModeTests` gate completed normally. Its result
  XML SHA-256 is `54A102CD0F200009D7DEE36B113C48F0658A0ED91FBD0ECE25CA13EF476AB5D8`:
  29/29 passed, with zero failed, skipped, or inconclusive, in `6.3697531`
  seconds. This covers the idempotence and observational validator assertions
  for REQ-M5D7Q0-009 / AC-M5D7Q0-008. The CLI test wrapper has no log-file
  option and detached after Editor completion, so no numeric wrapper exit code
  is claimed; the result XML and exited launch processes are the evidence.
  The builder produced only one tracked delta: canonical
  `Assets/Prefabs/Hub/HubRuntimeRoot.prefab` changed from historical baseline
  SHA-256 `F5B0B347AFFF52821594B449B4184464C9DFE426ADABEF2D42B5F73C7CEC5720`
  to `6599B82D5DA4E8ADCE6C7934147508E81BBBAFC36649A1D327779A0428724E91`.
  Astra explicitly approved this as the allowed current-source canonical
  builder output. Its prefab meta SHA-256 remains
  `B4C188F55CD7DA32649008AB5FA84CC434C04FF10BF64F1AB7C31071063D86AE`,
  and the other 32 tracked Q0 paths still match the immutable baseline. The
  historical baseline manifest was not modified.
- The next ordered current-source gate,
  `HubUiOnlyQ0ScopeAuditEditModeTests`, was also run once through official
  Unity CLI in that same normal-user execution boundary. It returned CLI exit
  `0`; result XML SHA-256
  `B3C8E55727659591FEEDAC8E38C2014698946B9BBA28E90F24DE14CCDD4CDDDA`
  records 4/4 passed, zero failed/skipped/inconclusive, duration `0.7896079`
  seconds. This is the focused AC-M5D7Q0-011 static-scope result supporting
  REQ-M5D7Q0-009. CLI exposes no log-file option,
  so no `focused-edit-scope.log` was emitted; the XML, explicit CLI exit, and
  filter are preserved in the final manifest. No subsequent PlayMode gate has
  begun.
- The following ordered focused PlayMode router gate,
  `HubUiOnlyInputRouterPlayModeTests`, was launched exactly once through the
  same normal-user boundary with its full contract filter. It reached runtime
  test activity but produced no result XML before the configured 300-second
  CLI timeout; the CLI and both same-install Unity processes had exited by
  00:26:32 KST. `Logs/Editor.log` records expected negative-path topology
  exceptions during the fixture's corruption tests, but no test-run completion
  or result emission. Because the official CLI test runner exposes no log-file
  option, no dedicated `focused-play-router.log` was created. This is an
  uncounted timeout without a report—not evidence of a specific functional test
  failure. Astra compared it with the prior valid `400.3398361`-second router
  duration and approved one bounded correction: the same filter, Editor, and
  normal-user boundary were rerun once with a 900-second timeout. The valid
  rerun result XML SHA-256 is
  `C09597488B68A1E8911B3A4CB1E52A41316BF9B66CCA5C2F269F04BC43BD83D4`:
  38/38 passed, zero failed/skipped/inconclusive, in `405.0191937` seconds.
  It covers AC-M5D7Q0-002, -003, -005, -006, -007, -009, and -012 across
  REQ-M5D7Q0-001 through -008 as applicable to the router fixture. The CLI
  wrapper detached after successful Editor completion, so its numeric exit is
  not inferred; the XML and exited launch processes are the result evidence.
  No code/test change was made, and no subsequent gate has begun.
- The next ordered focused Coverage B PlayMode gate,
  `HubUiOnlyQ0CoverageBPlayModeTests`, was run once through the same
  normal-user boundary under its Astra-authorized 1200-second envelope.
  Result XML SHA-256
  `E55336D9F7C7FFBBB41684E17CD989C70DF3A8352D645C38A0C1083C9F791801`
  records 27/27 passed, zero failed/skipped/inconclusive, in `572.2354192`
  seconds. It observes AC-M5D7Q0-007 and -009 for their applicable
  REQ-M5D7Q0 router invariants. The CLI wrapper detached after successful
  Editor completion and exposes no log-file option; the result XML and exited
  processes are the preserved evidence. No subsequent gate has begun.
- The following Handoff Matrix PlayMode gate,
  `HubUiOnlyQ0HandoffMatrixPlayModeTests`, was run once in the same boundary
  under its 900-second envelope. Result XML SHA-256
  `E7A66E68FE84425FC0B0649897DBBB773017690473C64255F27678D0DB758829`
  records 5/5 passed, zero failed/skipped/inconclusive, in `343.4638143`
  seconds. This covers REQ-M5D7Q0-007 / AC-M5D7Q0-006. The CLI runner exposes
  no log-file option and detached after successful Editor completion; the XML
  and exited processes are preserved evidence. The next ordered Remaining
  Runtime gate has not yet started.
- The ordered Remaining Runtime PlayMode gate,
  `HubUiOnlyQ0RemainingRuntimePlayModeTests`, was then run once in the same
  boundary under its 900-second envelope. Result XML SHA-256
  `6D4F82DD6AA56A6CC5E9705F4E0A0DBD8EFC752C1E153C2FCCC4757B6EC26BED`
  records 18/18 passed, zero failed/skipped/inconclusive, in `372.0641391`
  seconds. It observes REQ-M5D7Q0-001 through -008 as applicable and
  AC-M5D7Q0-001, -003, -004, -005, -009, and -012. The CLI runner exposes no
  log-file option and detached after successful Editor completion; the XML and
  exited processes are preserved evidence.
- Direct M5B5 `InputRouterPlayModeTests` then ran once through the same
  normal-user boundary with CLI exit `0`. Result XML SHA-256
  `1FBC31B5BE6FAF467D8090396D0059B3769C372C6431CAA469B4D405C2B7B660`
  records 3/3 passed, zero failed/skipped/inconclusive, in `0.1709869`
  seconds. This preserves the M5B5 full-simulation regression prerequisite
  required by AC-M5D7Q0-010. No dedicated log was emitted by the CLI runner.
- Direct M5D7M `DesktopProfileLaunchAdapterV1Tests` then passed current-source
  verification in the same boundary: 117/117 passed, zero failed/skipped/
  inconclusive, duration `961.8582484` seconds; XML SHA-256
  `990EA1501BE406A5A2B99ED189DB047F9DCF38CF39AA8C13E4D53F6C16DC28CE`.
- Direct M5D7N `HubEntryHandoffLatchV1Tests` was launched once with its
  approved 1800-second envelope but exited without result XML. No retry was
  made and direct M5D7P-A was not started, per stop-on-first-failure.
- Astra subsequently approved one bounded, product-code-free test-harness
  repair for M5D7N. To retain AC-M5D7N-001..004 scenario coverage across the
  PlayMode domain-reload boundary, `HubEntryHandoffLatchV1Tests.cs` replaces
  the nine mutable `LaunchScenario` objects formerly retained by NUnit
  `TestCaseSource` with nine named scalar-string `TestCase` values. The test
  body immediately resolves each key through a fail-closed switch factory;
  all nine factories, assertion bodies, scenario order, and named-case intent
  remain unchanged. This is test-only scope, with no product runtime delta:
  reconstructed pre-patch SHA-256 `9A96ABD3985785FBDE6FAFD2A825B44AD1D5B3AE3F3AFDB61C47DDF4655C1B2E`,
  current SHA-256 `A3BF6AB9DCB72A6E3B543FA8ED93B3FA9D1DDCA80A4CD2312FDEDBF22EFB04CF`.
  Static inspection confirms nine named cases and nine factory rows with no
  remaining `TestCaseSource` or `ScenarioTable`; `git diff --check` reports
  only the pre-existing unrelated trailing whitespace in
  `ProjectSettings/ProjectSettings.asset`.
- The single authorized post-repair M5D7N direct-Editor rerun used the new
  6000.6.0f1 Editor, no `-quit`, and the escalated normal-user boundary from
  02:35:17 through 03:05:38 KST. Unity resolved its entitlement and rebuilt
  the patched PlayMode test assembly, but it emitted neither test discovery
  nor result XML before the full 1800-second envelope. Only the owned Editor
  PID `38624` and its exact child helpers were stopped; Hub was untouched.
  The preserved log SHA-256 is
  `92FF1FDCA8BCD4FF670312327C060409F0F6579FED79DEB2069072C4991972E9`;
  the overwritten prior no-quit diagnostic log SHA-256 was
  `D9A2CCF480B56D697B499BEAA38EEA6BC6E7123F7BE5B5CF2ECD666459B33178`.
  This is still an uncounted runner timeout rather than a functional M5D7N
  failure. Per stop-on-first-failure, direct M5D7P-A, both full suites, and
  Luna review did not start.
- Astra rejected the scalar-case hypothesis because this rerun reached the
  same pre-discovery 1800-second timeout as the original form. The test-only
  patch was therefore reverted with `apply_patch`, restoring the exact
  `TestCaseSource`/`ScenarioTable` form and source SHA-256
  `9A96ABD3985785FBDE6FAFD2A825B44AD1D5B3AE3F3AFDB61C47DDF4655C1B2E`.
  No Unity rerun followed the reversion. This confirms that mutable scenario
  serialization was not causal; product code remains untouched.
- Astra then authorized one distinct diagnostic stem, `direct-m5d7n-smoke`,
  against the restored original source. The direct no-quit Editor ran the
  exact `DefaultReceipt_FirstUpdatePublishesOneUiOnlyHandoffAndOneNotification`
  method once in the escalated normal-user boundary under a 600-second
  envelope. Its XML SHA-256
  `563FE9A58EFE0780D1A9002C1D9985FAD5EBE6A388FDAA496FA75AC077D01562`
  records 1/1 passed, zero failed/skipped/inconclusive, duration `49.4838167`
  seconds; log SHA-256 is
  `1C44C49ADEC462E3EB72A43870D453C75CFA8A70BD6D9EACD4455A2344FC9E42`.
  The process exited with no owned Unity process remaining. This demonstrates
  that the original class is discoverable and executable, so the unresolved
  full-class M5D7N result is a slow-suite envelope issue rather than a global
  runner/discovery blocker. It does not replace the required class-level gate;
  a 3600-second full-class decision remains with Astra. Direct M5D7P-A and
  both full suites remain unstarted.
- Astra approved one 5400-second full-class rerun after the smoke result. The
  restored original `HubEntryHandoffLatchV1Tests` completed in `2399.6245332`
  seconds: 69/69 passed with zero failed/skipped/inconclusive. XML SHA-256 is
  `A4984C6BDBB007C4C58C3D44144F798F54182607646D30B2C77E08EB713C6434`;
  direct log SHA-256 is
  `443F33F5124A438B88F92DF16D9AAF746B5BFE98D6B4B02A3461F1CCF751C534`.
  This proves the prior 1800-second envelope was insufficient, not a runner
  blocker. The immediately following direct M5D7P-A
  `UiSemanticFrameRouterPlayModeTests` also passed 52/52, zero failed/skipped/
  inconclusive, in `0.339335` seconds; XML SHA-256
  `A95C5FF97732E10651888388E45ABE8FAFFB9ADDF89AE5601800C91CEE9E700B`,
  log SHA-256 `520533568776391DED9EC93DC3F6BDB5EAC22F5749FA71DBCE37C4EBF8EDAFB0`.
- The required full EditMode suite then completed through the official Unity
  CLI in the escalated normal-user boundary: 735/735 passed, zero failed/
  skipped/inconclusive, duration `917.2217652` seconds. Its XML SHA-256 is
  `29A3CF4162E42AEC3739BA774BFD0BAED6C2119B52964A9A1DC8C1C87061F63C`.
  Full PlayMode has not started; its timeout recommendation remains an Astra
  decision.
- The approved full PlayMode suite completed once under its 7200-second
  envelope: 943/943 passed, zero failed/skipped/inconclusive, duration
  `4997.7963525` seconds. XML SHA-256 is
  `E28011588D077077FF629C7651E06DB6ACDCFA7493AB40DA0E90FE6BF0293A78`.
  Post-suite 33-path Q0 scope audit finds one delta only: canonical
  `HubRuntimeRoot.prefab` is now
  `2422E270366EB26A0294D6D84DD208DEC3F1F4ECBEF116B3BF9ADD9AC526C59F`,
  differing from both the immutable baseline and the previously Astra-approved
  builder output. This is a residual scope/acceptance blocker requiring Astra
  decision; Terra does not self-accept it.
- Astra-approved deterministic YAML repair replaces process-random prefab
  saves for REQ-M5D7Q0-009 / AC-M5D7Q0-008. The valid branch is no-op; repair
  writes fixed LF UTF-8 bytes, synchronously imports, and validates. The
  focused builder suite passed 31/31 in two independent Unity processes; the
  process-B duration was `1.8371698`, XML SHA-256
  `0AD583AA8066B864187A3AA82C55FE53ECB9FC0849D139F64C7A28814A4A2F4E`,
  canonical prefab SHA-256 `B1CFD731A4E177C37B59CEA0813E3B51E44DD10BA96AE38A1B15F15012438EEE`,
  and meta SHA-256 `B4C188F55CD7DA32649008AB5FA84CC434C04FF10BF64F1AB7C31071063D86AE`.
- Final current-source Editor evidence: the post-amendment scope run completed
  4/4 passed, zero failures, duration `0.3514881`; artifact
  `artifacts/unity-results/m5d7q0-20260922-reboot/scope-after-deterministic-fix-final.xml`
  SHA-256 `CC20E761D4ACD43D59ACBB8069EE060563BE87C2CD19E59D836A054F002D0733`.
  The deterministic-fix full EditMode suite completed 737/737 passed, zero
  failed/skipped/inconclusive, duration `896.1148112`; artifact
  `artifacts/unity-results/m5d7q0-20260922-reboot/full-editmode-deterministic-fix.xml` SHA-256
  `DEDFFC470FA86A40ACD6321154AF57D75FB5231F852641F2FBC4A6DE1B1A5B4D`.
  Canonical prefab/meta remained respectively
  `B1CFD731A4E177C37B59CEA0813E3B51E44DD10BA96AE38A1B15F15012438EEE` and
  `B4C188F55CD7DA32649008AB5FA84CC434C04FF10BF64F1AB7C31071063D86AE`.
  The changed code is Editor-only; runtime and PlayMode sources are unchanged.
  Luna/Astra determine whether the historical full PlayMode result suffices.
- Consequently the current-source full EditMode requirement is complete.
  Luna's ruling on reuse versus rerun of the pre-revision 943/943 PlayMode
  result, and Luna's independent review, remain pending. No acceptance
  criterion is marked accepted by this evidence.
- Luna's first final review reported stale source/result state in the historical
  `final-manifest.json`. The superseding
  `artifacts/unity-results/m5d7q0-20260922-reboot/final-manifest-v2.json`
  records all 33 Q0 baseline/current hash rows, every retained passing XML with
  exact counts and provenance, and null log fields where no same-invocation log
  exists. Its SHA-256 is
  `B7D525428BE895E19BC42B77D260059833204386331C481716EDEA9D51B7BEEA`.
  The historical manifest remains immutable evidence and is explicitly
  superseded, not silently rewritten.

## Astra final integration

Astra accepts the implementation on the amended AC-M5D7Q0-011 evidence
boundary. Luna independently verified the 33-path v2 manifest, final scope
`4/4`, deterministic full EditMode `737/737`, and the bounded reuse of full
PlayMode `943/943`, with `P0=0`, `P1=0`, and `P2=0`. This record does not claim
that the originally dirty repository was clean and does not admit any unrelated
workspace path into Q0 scope.

## GPT participation ledger

| Role | Model | Recorded state |
|---|---|---|
| Implementation and focused test authoring | Terra (`gpt-5.6-terra`) | complete; current-source evidence recorded |
| Independent verification | Luna (`gpt-5.6-luna`) | `ACCEPT`, `P0=0`, `P1=0`, `P2=0` |
| Contract/integration authority | Astra (`gpt-6-astra`) | Verified; final integration accepted 2026-09-23 |
