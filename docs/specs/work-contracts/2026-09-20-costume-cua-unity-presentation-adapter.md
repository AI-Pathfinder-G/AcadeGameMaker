---
status: Approved
---

# Costume CUA Unity presentation adapter

- Date: 2026-09-20
- Status: Approved — Astra 2026-09-27 after Luna final contract pre-gate
  `PASS — P0=0, P1=0, P2=0`; the subsequent implementation remains
  pre-execution blocked until the static-review closure amendment is re-gated
- Owning spec and revision: [Costume file IO and additive Unity presentation adapter](./2026-09-13-costume-io-and-unity-adapter.md), Child B / `REQ-CUA-001..010`
- Parent foundation: approved bounded pure costume/presentation core; real media remains `source_only`
- Assigned by / final authority: Astra
- Implementer: Terra
- Independent verifier: Luna
- Requirement IDs: `REQ-CUA-001..010`
- Acceptance-criterion IDs: `AC-CUA-001..009`
- Rollback point: verified pure costume/presentation core and the independently accepted CIO port

## Bounded outcome

Add a Unity-facing presentation library that:

1. projects the real 36-row Seryeong catalog truthfully as pending/unavailable with `NoAcceptedDefault`;
2. supports transient row highlighting without selecting, unlocking, saving, or binding;
3. validates a complete Accepted portrait/gameplay package before publication;
4. swaps preview and gameplay presentation as one logical unit only after correlated durable selection success; and
5. preserves the actor's completed action, normalized phase, facing, anchor, and tick when visual media changes.

This unit uses synthetic Accepted fixtures only. It does not import or accept real media, compose the Hub Settings/Wardrobe screen, alter InputMode, choose a storage root, or connect to a live gameplay renderer. The eventual wardrobe composition is a later exact scene/prefab contract.

## Truthful wardrobe projection

- The immutable view contains actor/catalog/state revisions, row view models, availability, current ID, transient preview ID, selection eligibility, persistence status, and a typed diagnostic.
- All current real rows remain pending: no portrait/gameplay preview, save request, current selection, or fallback is fabricated.
- Highlighting a row is transient. It may update descriptive text and a pending/locked badge but cannot mutate state bytes or invoke IO/media binding.
- A future accepted row's preview uses the exact same staged media package as gameplay. The right-side canonical idle pose is not a separately substituted concept image.

## Atomic media and completed-snapshot rules

- An authored media package must exactly match one Accepted definition by actor/costume/set IDs, catalog/presentation revisions, canonical manifest/media SHA-256 values, required actions, frame rectangles, point filtering, approved PPU/body profile, pivot/baseline, and live Unity references.
- Validate and stage the entire package before touching a preview or renderer. Missing/stale/foreign/destroyed media, hash or identity mismatch, incomplete actions, or invalid import geometry rejects the whole candidate and retains the prior complete pair.
- The gameplay adapter consumes only a post-commit `CompletedActorSnapshotV1`. Duplicate identical ticks may refresh idempotently; decreasing, conflicting, future/uncommitted, or foreign-actor snapshots reject.
- A swap maps the preserved action and normalized phase to the corresponding new clip/frame. It must not restart at frame zero or change tick, action age, facing, anchor, transform, physics, collision, combat, targeting, cooldowns, abilities, AI, or mechanic phase.
- Unity `Update`, `FixedUpdate`, Animator time, wall clock, and frame rate do not advance or choose simulation action state.

### Canonical package bytes and frame mapping

- `PortraitHash` is lowercase SHA-256 over the exact accepted portrait PNG file bytes. `AtlasHash` is lowercase SHA-256 over the exact accepted gameplay-atlas PNG file bytes. `ClipMapHash` is lowercase SHA-256 over the exact UTF-8 bytes of a canonical clip-map JSON document.
- Canonical clip-map JSON uses UTF-8 without BOM or insignificant whitespace, ordinal property order `schemaVersion,actorId,costumeId,gameplaySetId,ppu,cellWidth,cellHeight,pivotXQ1000,pivotYQ1000,baselineY,clips`. Clips sort by ordinal `actionId`, then closed facing order `left,right`. Each clip contains properties in exact order `actionId,facing,loop,frames`; `facing` is exactly `left` or `right`. Duplicate `(actionId,facing)` pairs reject. Each frame object contains integer properties in exact order `x,y,width,height,pivotXQ1000,pivotYQ1000,baselineY,durationTicks`; `width`, `height`, and `durationTicks` are positive and widened checked `long` right/top calculations must remain inside the atlas. This synthetic-only slice rejects every repeated `(x,y,width,height)` rectangle within one clip; cross-clip reuse remains allowed. Intentional holds require a later schema-v2 product slice. Unknown/duplicate properties and noncanonical bytes reject.
- The package source-manifest bytes bind the three payloads by recording the exact actor/costume/set IDs, revisions, the three lowercase hashes, required-action IDs, required-facing IDs, and clip-map schema version. Its own canonical UTF-8 bytes are retained as evidence; it is not substituted for any of the three catalog hashes.
- Pure `CostumePresentationBindingV1.ClipIds` continues to carry action vocabulary only. Before binding publication, the Unity package validator requires every package-required action to contain both `(actionId,left)` and `(actionId,right)` clip-map rows. At presentation time it resolves the completed snapshot's exact `ActionId` plus exact `Facing` to that tuple. No compound action string is injected into the pure binding, and facing never changes simulation action identity.
- For a clip with `N>0` ordered frames, non-looping `ActionAgeQ1000` maps to `min(N-1, floor(ActionAgeQ1000*N/1000))`. Looping clips use `floor(ActionAgeQ1000*N/1000)` for `0..999` and map exact `1000` to frame `0`. Frame duration metadata is validated but cannot advance simulation or override the completed snapshot phase.

## Selection and CIO handoff

- Prepare selection only for an Accepted and unlocked row whose view says selection is eligible.
- Stage exact next state, canonical bytes, view, and media candidate, then construct one immutable adapter-owned `PublishedCostumePresentationV1` tuple containing state/current ID, pure binding, portrait reference, gameplay clip map, and all correlated revisions. Every field and Unity reference is validated before persistence.
- Call the exact injected concrete `CostumeFileAdapterV1.Save(stagedNextState)` synchronously. Correlation means that same call receives the same immutable staged state and returns `CommittedFirst` or `CommittedReplacement` with `ExpectedRevision == stagedNextState.Revision`; the CUA does not accept a detached receipt, asynchronous callback, user-supplied result, or alternate save delegate. The concrete CIO adapter owns exact byte encoding/reopen verification, so no nonexistent digest/token is inferred from its receipt.
- After exact save success, publish only by one non-throwing reference assignment from the old immutable tuple to the already-complete new tuple. All read surfaces first snapshot that one reference; no callback or renderer may observe or execute between component assignments because there are no component assignments.
- Live preview/render projections read the published tuple on their next ordinary presentation pass. A projection failure cannot revert durable state or the authoritative tuple; it enters terminal `ReloadRequired`, disables mutation, and may retain the last successfully drawn pixels only as non-authoritative display until fresh CIO recovery. It never claims rollback or the old durable selection.
- `FailedBeforeCommit` leaves old state and media authoritative.
- `CommitOutcomeUncertain` enters terminal `ReloadRequired`, blocks further mutation, claims no durable winner, and retains the last complete visual only as non-authoritative display until fresh CIO recovery.

## Exact allowlist

Only these new paths and minimum status links may change:

- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/AcadeGameMaker.Presentation.Unity.asmdef` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityViewPresenterV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/AcadeGameMaker.Presentation.Unity.PlayMode.Tests.asmdef` and `.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` and `.meta`
- `docs/verification/2026-09-20-costume-cua-contract-pregate.md`
- `docs/verification/2026-09-20-costume-cua-implementation-evidence.md`
- `docs/verification/2026-09-20-costume-cua-luna-independent-review.md`
- this contract and the minimum status/link hunk in `docs/README.md`

No existing source/test/asmdef, costume catalog row, scene, prefab, real media, Profile/Run/movement/combat/transfer/input/camera file, package, or ProjectSetting may change. Tests use isolated temporary objects and destroy only objects they create.

## Requirements

- **REQ-CUA-001:** Expose exact pending/locked/unlocked/current/unavailable states and report the 36 real Seryeong rows as pending with `NoAcceptedDefault`.
- **REQ-CUA-002:** Transient preview navigation cannot grant, select, save, bind, or disguise placeholder media as an outfit.
- **REQ-CUA-003:** Validate all package identities, revisions, canonical hashes, portrait, complete clips, geometry/settings, and pure binding before publication.
- **REQ-CUA-004:** Portrait/preview and gameplay media publish atomically; any failure retains the prior complete pair.
- **REQ-CUA-005:** Consume only completed snapshots and preserve tick/action/phase/facing/anchor across swap without owning simulation time.
- **REQ-CUA-006:** Presentation is visual-only and cannot alter mechanics, physics, collision, combat, targeting, stats, abilities, AI, or phase.
- **REQ-CUA-007:** Publish selection only after exact correlated durable save; uncertain commit enters closed `ReloadRequired` behavior.
- **REQ-CUA-008:** Synthetic Accepted fixtures never promote or claim acceptance for real media/catalog rows.
- **REQ-CUA-009:** Use explicit instance-bound ports; do not discover or modify existing menus, scenes, prefabs, InputMode, or renderer owners.
- **REQ-CUA-010:** Stay inside the exact additive allowlist.

## Acceptance criteria

- **AC-CUA-001 (REQ-CUA-001/002):** isolated PlayMode view shows exactly 36 pending rows and `NoAcceptedDefault`; highlighting changes no state bytes and invokes no save/media operation.
- **AC-CUA-002 (REQ-CUA-003/004/008):** synthetic fixtures independently corrupt every identity, revision, hash, manifest payload, portrait, required action/facing pair, duplicate/unknown facing, rectangle, filtering, PPU, pivot, baseline, and reference lifetime; every case preserves the old complete pair.
- **AC-CUA-003 (REQ-CUA-004):** observation hooks around save success and publication never see a new preview/portrait paired with old gameplay media or the reverse. The prepared immutable tuple publishes through one reference assignment; an injected post-save projection failure enters `ReloadRequired` while the durable/new tuple remains authoritative and old pixels are labeled non-authoritative. Foreign/stale/consumed candidates reject.
- **AC-CUA-004 (REQ-CUA-005):** valid and invalid snapshot matrices cover idle, locomotion, airborne, landing, bow draw/release/recovery and both exact facings; valid swaps preserve completed state, resolve `(ActionId,Facing)` without changing pure action identity, and use the exact non-loop/loop endpoint mapping above without accidental frame-zero restart. Missing/unknown facing, decreasing, conflicting duplicate, future/uncommitted, and foreign-actor snapshots reject at the explicit post-commit observation port.
- **AC-CUA-005 (REQ-CUA-006):** before/after evidence proves all mechanic/physics identities and simulation-owner call counts unchanged.
- **AC-CUA-006 (REQ-CUA-007):** the exact synchronous concrete CIO call publishes once only when outcome and `ExpectedRevision` match. Wrong/stale revision rejects; tests prove same-revision/different-state cannot be supplied through a detached result because no detached receipt/delegate API exists. Pre-commit failure retains old state/media; uncertain commit or post-save projection failure closes mutation and requires recovery under the rules above.
- **AC-CUA-007 (REQ-CUA-009/010):** static and runtime probes prove explicit ports, no global lookup or existing owner mutation, and allowlist-only additions.
- **AC-CUA-008 (REQ-CUA-008):** every positive package is labeled synthetic and the real 36 rows remain pending.
- **AC-CUA-009 (all):** focused and full EditMode/PlayMode pass with failed/skipped/inconclusive `0`; Luna independently verifies and Astra decides acceptance.

## Dependencies and stop conditions

Do not approve implementation until CIO `AC-CIO-007` and M5D7Q0 verification are complete and Luna has re-reviewed these amendments against the actual public surface. Stop if the single immutable publication cannot be implemented without a fallible post-save construction step, a pure/public correlation is missing, a test needs real media, a snapshot is not demonstrably post-commit, or connection requires any unnamed existing owner change. PPU/internal-render choice is intentionally deferred because this slice uses synthetic packages; real media import remains separately blocked.

## Participation

Sol supplied the presentation split, truthful unavailable-state behavior, atomic-swap rules, and wardrobe interaction boundary. Astra authored this Review child. Terra/Luna participation will be recorded only after actual gated work.

## Astra approval — 2026-09-27

CIO, M5D7Q0, and M5D7Q-A are `Verified`. Luna independently re-reviewed this
exact amended contract at `PASS — P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-contract-final-pregate.md`, SHA-256
`AC90107EFB11D75D2D15BEB71906F69A8A80A28B9D246B4596CD5F69B5F9C822`.
Astra approves implementation inside the exact additive allowlist. Positive
media fixtures remain synthetic; all 36 real Seryeong rows remain pending,
and this approval does not authorize a scene, wardrobe screen, live renderer
connection, real-media import, catalog promotion, or gameplay authority.

## Static-review closure amendment — 2026-09-27

Luna's first implementation review correctly blocked Unity execution at
`P0=0, P1=4, P2=2`. Astra accepts Sol's bounded closure proposal
`docs/proposals/2026-09-27-costume-cua-static-review-closure-amendment.md`,
SHA-256
`C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`,
as normative for the corrected implementation. It changes no REQ/AC mapping,
canonical JSON property order, schema version, real-media status, or product
decision.

The correction must remove every selection overload that accepts a snapshot;
the explicit instance-bound completed-snapshot port is the sole writer of a
private accepted-observation slot. First publication uses the proposal's
reflexive pure `TrySwap(proposed, proposed, snapshot, out committed)` bootstrap;
replacement uses current/proposed and only the exact committed binding enters
the complete immutable tuple. Every construction/validation/swap step occurs
before the concrete synchronous CIO save, followed only by one non-throwing
reference assignment and then projection. Current-tuple corruption enters
`ReloadRequired`; failed-before-commit preserves the old authoritative tuple;
uncertain commit claims no winner; projection failure retains the durable new
tuple and enters `ReloadRequired`.

Terra may correct only the media-package source, presentation-adapter source,
and their two existing PlayMode test files plus the already allowlisted
evidence. The presenter, asmdefs/metas, pure core, CIO, catalog, scene/prefab,
media, packages, ProjectSettings, and existing owners remain byte-identical.
The focused suite must independently cover the full mutation table, checked
integer bounds, within-clip duplicate rejection, first/replacement pure swap,
save/projection terminal faults, port-only snapshot/action/facing matrices,
and mechanics/physics identity/call-count preservation described by the
proposal. Unity execution remains forbidden until Luna independently re-gates
the corrected exact source/test hashes with no unresolved P0/P1/P2.

## Astra execution authorization — 2026-09-27

Luna's final aggregate static gate passed the exact corrected implementation
at `P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-luna-final-aggregate-static-gate.md`,
SHA-256
`1AE41A6DC119BDFBB027D46EF146FBEEEC47B84D8BC887E4A34E958DD66C18D5`.
Astra authorizes the following three Unity 6000.6.0f1 runs, serially and
without `-quit`. No other Unity run is authorized by this section.

1. Focused PlayMode: `-runTests -testPlatform PlayMode -assemblyNames
   AcadeGameMaker.Presentation.Unity.PlayMode.Tests -testFilter
   AcadeGameMaker.Tests.PlayMode.PresentationUnity`.
2. Full EditMode: `-runTests -testPlatform EditMode` with no filter or
   assembly restriction.
3. Full PlayMode: `-runTests -testPlatform PlayMode` with no filter or
   assembly restriction.

Fresh immutable outputs must be written below
`artifacts/unity-results/costume-cua-20260927/` using exactly these stems:

- `costume-cua-r7-focused-playmode` (`.xml` results and `.log` editor log);
- `costume-cua-r7-full-editmode` (`.xml` results and `.log` editor log); and
- `costume-cua-r7-full-playmode` (`.xml` results and `.log` editor log).

The directory was absent immediately before this authorization. A pre-existing
target, compile error, missing result XML, nonzero failed/skipped/inconclusive
count, unexpected project mutation, or source/hash drift is a hard stop. The
result artifacts are evidence outputs and do not expand production scope.

## Astra R8 retry authorization — 2026-09-27

The R7 focused run stopped during compilation before tests because
`CostumeUnityClipV1.CheckId` was unresolved. Its immutable failure log is
`artifacts/unity-results/costume-cua-20260927/costume-cua-r7-focused-playmode.log`,
SHA-256
`2E48CCACE390FE16B3F327497D6BA1DD6133071E3A04583CD9CEE8AC3E1AE14A`;
no R7 result XML exists and no R7 full suite ran. Luna passed the bounded
compile correction at `P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-luna-compile-blocker-fix-pregate.md`,
SHA-256
`EBCB6922F61D721BB0DE125112B7F8C3CC12CB62827BF1BCBC19FD61704E8517`.
The corrected media source SHA-256 is
`A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
and the corrected adapter-test SHA-256 is
`9C1C28CBC650E955BE64FFC0778F7BDB6129F56BA762D48049F0D6D47435CDF9`;
all other aggregate-gate hashes remain unchanged.

The unused R7 full-suite stems are withdrawn. Astra authorizes the same exact
three commands and order defined above, still on Unity 6000.6.0f1 and without
`-quit`, using only these fresh stems under the existing evidence directory:

- `costume-cua-r8-focused-playmode` (`.xml` and `.log`);
- `costume-cua-r8-full-editmode` (`.xml` and `.log`); and
- `costume-cua-r8-full-playmode` (`.xml` and `.log`).

The R7 failure log must not be changed or removed. The same hard-stop rules
apply independently to every R8 run.

## Astra R9 retry authorization — 2026-09-27

The R8 focused run also stopped before tests because its test source lacked
the `AcadeGameMaker.Presentation` namespace import. Its immutable log is
`artifacts/unity-results/costume-cua-20260927/costume-cua-r8-focused-playmode.log`,
SHA-256
`A759B4E6D0C49321F79C5AB40BEE023BC5F753E2E50E9AC4223A38806743C579`;
no R8 result XML exists and no R8 full suite ran. Luna passed the one-line
test-only correction at `P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-luna-r8-compile-blocker-fix-pregate.md`,
SHA-256
`6720EE8674A6F83ADCE25756CFCD0F88329328737174A1A703860CFA6E501DF0`.
The corrected adapter-test SHA-256 is
`8E93A8293ABD0E79913628E74A9FDC54E57D37C20022D053308037136358CF99`;
the corrected media SHA and all other aggregate hashes remain unchanged.

The unused R8 full-suite stems are withdrawn. Astra authorizes the same exact
three commands, order, Unity version and no-`-quit` behavior using only these
fresh stems under the existing evidence directory:

- `costume-cua-r9-focused-playmode` (`.xml` and `.log`);
- `costume-cua-r9-full-editmode` (`.xml` and `.log`); and
- `costume-cua-r9-full-playmode` (`.xml` and `.log`).

Both R7 and R8 failure logs must remain immutable. The same hard-stop rules
apply independently to every R9 run.

## Astra R10 retry authorization — 2026-09-27

The R9 focused run stopped before tests because three private test helpers were
absent. Its immutable log is
`artifacts/unity-results/costume-cua-20260927/costume-cua-r9-focused-playmode.log`,
SHA-256
`E44FA7EBB1E4F2F1D807FA8F5418D23622C6D130388AE81D99669574BB0D061C`;
no R9 result XML exists and no R9 full suite ran. Luna passed the test-only
helper correction at `P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-luna-r9-helper-compile-fix-pregate.md`,
SHA-256
`E54D96D94F41B3F1E809FA630BB3C0197638F79D1B4090CC2F6850962FF96CE6`.
The corrected adapter-test SHA-256 is
`F60EDF4B483F0C108FAE7B11FAF3A8FAC87CFCE581974D3ED5C425E9C5964887`;
all other current implementation hashes remain unchanged.

The unused R9 full-suite stems are withdrawn. Astra authorizes the same exact
three commands, order, Unity version and no-`-quit` behavior using only these
fresh stems under the existing evidence directory:

- `costume-cua-r10-focused-playmode` (`.xml` and `.log`);
- `costume-cua-r10-full-editmode` (`.xml` and `.log`); and
- `costume-cua-r10-full-playmode` (`.xml` and `.log`).

The R7, R8 and R9 failure logs must remain immutable. The same hard-stop rules
apply independently to every R10 run.

## Astra R11 retry authorization — 2026-09-27

The R10 focused suite compiled and ran `195` tests, with `185` passed and `10`
failed in the NonJson test oracle. Its immutable artifacts are
`costume-cua-r10-focused-playmode.xml`, SHA-256
`C2AFA45FE75BDF9CC1E8A9515C0117AC822C9D5B5E4DB6E9F108C2771BAB7DBD`,
and `costume-cua-r10-focused-playmode.log`, SHA-256
`2D2C9E45A5275333AD20686FA56C51A8057C97B7CD2F37050954086848B309A0`,
under the authorized evidence directory. No R10 full suite ran. Luna passed
the corrected test boundary and count-preservation oracle at
`P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cua-luna-r10-nonjson-p1-closure.md`,
SHA-256
`FA6808A28796406CBEDA8E131779B8056E79F4C2B586B0D727172EC9AAF42F0F`.
The corrected adapter-test SHA-256 is
`16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`;
all other current implementation hashes remain unchanged.

The unused R10 full-suite stems are withdrawn. Astra authorizes the same exact
three commands, order, Unity version and no-`-quit` behavior using only these
fresh stems under the existing evidence directory:

- `costume-cua-r11-focused-playmode` (`.xml` and `.log`);
- `costume-cua-r11-full-editmode` (`.xml` and `.log`); and
- `costume-cua-r11-full-playmode` (`.xml` and `.log`).

All R7 through R10 artifacts must remain immutable. The same hard-stop rules
apply independently to every R11 run.

## Astra R12 full-PlayMode liveness retry authorization — 2026-09-27

The R11 focused PlayMode suite passed `195/195` with failed, skipped and
inconclusive counts all `0`; its XML SHA-256 is
`0485ADB16440963B553A6C4D48191D47F1C4C736624B3402D6D9632414740F19`
and its log SHA-256 is
`36B510E577231DC3E8E757063BB3C6FC96291F090F7334B66FB271990FE4E91C`.
The R11 full EditMode suite passed `800/800` with failed, skipped and
inconclusive counts all `0`; its XML SHA-256 is
`D4E18A69084A2E0140E19D503C734AC7DD68C30AE1FF723D5BD13CC7E3A38C7C`
and its log SHA-256 is
`86251F0306CC575C86F617E9313CE0740FDB3507290863F4014E0EB990FD2DF0`.
Those results remain valid because the implementation and tests have not
changed.

The R11 full PlayMode run produced no result XML. It stopped making log
progress for more than `31` minutes while retaining sustained single-core
CPU use at
`DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`.
Only the exact task-created Unity process tree was stopped after this was
classified as a test-runner livelock. Its preserved partial log SHA-256 is
`397B445907A90CB78F32F3DC66DDC6F1B9A3EAF7C4FD7DC26690BDFDDBC4F235`.
The run-created
`Assets/InitTestScene7441ab4c-7296-4c24-a9be-2a434f6bb15e.unity` and `.meta`
are preserved as drift evidence and form part of the pre-R12 baseline; they
must not be silently removed or modified.

The Approved diagnostic-only child contract
`docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
then established all of the following without source or workspace drift:

- R54 exact suspect test: `1/1` passed in `22.2733782s`; XML SHA-256
  `D76894EADADE98329843DC093A1D5C5E74F165FB56E4EF283D3A160F0435A284`,
  log SHA-256
  `34AE43B68B5363589DD6288DCE3FA224352EF7A6AC4D22421E382CF146DAF2C1`.
- R55 complete `DesktopProfileLaunchAdapterV1Tests` class: `117/117` passed
  in `982.0166498s`; XML SHA-256
  `9B664706F9CC0814C34169673EF6751747F300FC4DD0BC6FD1283944F84E2EB8`,
  log SHA-256
  `0D1551BE80E49CCB2FC5AD2AF57F482CFF3106BFEF03331AE1DC2611623FAB2B`.
- R56 the exact preceding Camera, Combat and pure Input groups plus the full
  M5D7M class: `558/558` passed in `936.1186442s`; XML SHA-256
  `68FD2FF1C697FA2B0DA58BC515CE6B165FDFAA3FF53F601C18C39A7E292E4DA7`,
  log SHA-256
  `F86781E60227487BBC6011A323EA5BEB7577B71A98BC8047F888185DA55A882A`.

Astra therefore classifies the R11 stop as a non-reproduced transient Unity
test-runner liveness failure, not evidence of a deterministic production-code
defect. No source or test correction is authorized. The exact current hashes
remain:

- `CostumeUnityMediaPackageV1.cs`:
  `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`;
- `CostumeUnityPresentationAdapterV1.cs`:
  `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`;
- `CostumeUnityPresentationAdapterV1Tests.cs`:
  `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`;
  and
- `CostumeUnityViewPresenterV1Tests.cs`:
  `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.

Subject to a fresh Luna procedural pre-gate, Astra authorizes exactly one
Unity 6000.6.0f1 full PlayMode run with `-runTests -testPlatform PlayMode`, no
filter, no assembly restriction and no `-quit`. Its only fresh outputs are:

- `artifacts/unity-results/costume-cua-20260927/costume-cua-r12-full-playmode.xml`;
  and
- `artifacts/unity-results/costume-cua-20260927/costume-cua-r12-full-playmode.log`.

Both paths must be absent at the pre-gate and all earlier artifacts must remain
immutable. The global watchdog is `6600` seconds. If the log remains unchanged
for at least `180` seconds at the same M5D7M duplicate-adapter test while Unity
continues consuming CPU, classify the attempt as the same runner livelock,
stop only the exact task-created process tree, preserve all evidence, and do
not retry. A compile error, missing XML after normal exit, any nonzero failed,
skipped or inconclusive count, source/test hash drift, or any new unexpected
workspace mutation is a hard stop. The pre-run workspace baseline contains
`489` porcelain-status entries; its observed UTF-8 joined status SHA-256 is
`B7767138F675CCFB56B4C32B1CE0738B8BBEFC341907538238C15442C9745984`.

## Astra R13 full-PlayMode completion authorization — 2026-09-28

R12 ran once but exited before result XML creation. Its immutable log SHA-256
is `666A74FA760407553FB1186022FEDB77258EBC4CDDE06E9C6995C5323E2EA906`.
The interruption and independent Luna forensic assessment are recorded in
`docs/verification/2026-09-28-costume-cua-r12-full-playmode-interruption.md`,
SHA-256 `C908FAD6F9A57A00ACEABEB68BCB78331C82983012509B0DB8FDA5CCB0D4714B`.
The editor and worker logs show an import-worker transport disconnect but do
not establish its cause. R12 is invalid for `AC-CUA-009`; no CUA product
defect or source correction is inferred. Its created
`Assets/InitTestScene4c3f7ede-c74c-45a8-8c34-93f19204e4bd.unity` and `.meta`
are preserved in the baseline alongside the R11 scene pair.

Subject to an independent Luna procedural pre-gate, Astra authorizes exactly
one further full PlayMode execution in Unity `6000.6.0f1`. Use the same
`-batchmode -projectPath <repository> -runTests -testPlatform PlayMode
-testResults <R13 XML> -logFile <R13 log>` command, with no `-quit`, filter,
or assembly restriction. The fresh output paths are exactly
`artifacts/unity-results/costume-cua-20260927/costume-cua-r13-full-playmode.xml`
and `.log`, both absent before this authorization. Keep the command attached
to an observable command session until it exits; record the exact Unity PID.
No other Unity run or retry is authorized by this section.

Apply a global `6600` second watchdog. Only if the log remains unchanged at
the same M5D7M duplicate-adapter test for at least `180` seconds while Unity
CPU increases, classify the R11-type livelock and stop the exact task-created
process tree. If the process exits without XML, preserve the partial log and
stop. The same applies to a compile failure, any failed/skipped/inconclusive
test, source hash drift, or unexpected workspace mutation. Do not infer a
pass from a partial log or from R11 focused/EditMode results. Existing
artifacts and temporary test scenes are immutable evidence. The immediate
pre-R13 porcelain baseline contains `493` entries with UTF-8 joined SHA-256
`FFC171B3E06CB54C852681A798F327E50946BD68270FF9DAFCFE80A20ACF0462`.
The four CUA source/test hashes listed in the R12 authorization remain the
required execution baseline.

## Astra R14 licensing-boundary execution authorization — 2026-09-28

R13 ran once in the restricted command environment and could not start tests.
The Unity editor repeatedly failed to connect to the already-running
Licensing Client named pipe, then Package Manager registered zero packages.
The invalid run is recorded in
`docs/verification/2026-09-28-costume-cua-r13-license-blocker.md`, SHA-256
`33A8C22DEFB1318BB58CE3581B311A9A639D6131083FB2EF4C79E6E094F3393D`.
Its immutable log SHA-256 is
`A0123D069C423090587199B865FBD11966FF97D3A11ABE9F7CDC675A5481B925`.
No tests started and R13 has no XML. Luna independently classified this as
an environment/authentication blocker, not a CUA code finding. Astra stopped
only the exact task-created editor PID `32628`; the pre-existing Unity Hub
and Licensing Client remain running.

Subject to independent Luna procedural pre-gate and host permission review,
Astra authorizes exactly one Unity `6000.6.0f1` full PlayMode run in an
unrestricted command environment so the editor can reach the Licensing Client
named pipe. The command remains `-batchmode -projectPath <repository>
-runTests -testPlatform PlayMode -testResults <R14 XML> -logFile <R14 log>`,
without `-quit`, filter, or assembly restriction. The only fresh target stems
are `artifacts/unity-results/costume-cua-20260927/costume-cua-r14-full-playmode.xml`
and `.log`, absent before authorization. Maintain an attached command session
and record the exact Unity PID. This permission changes only the execution
environment, not the tests, source, packages, ProjectSettings, or product
scope. Do not retry after any R14 failure.

The global watchdog remains `6600` seconds. The M5D7M duplicate-test
same-log `180` second plus increasing-CPU watchdog remains active. Repeated
license pipe refusal, 60-second timeout, and zero registered packages before
test startup is an authentication hard stop once confirmed for at least two
cycles; stop only the task-created editor process and preserve evidence. Any
missing XML after exit, compile failure, nonzero failed/skipped/inconclusive
count, source hash drift, or new unexpected workspace mutation is also a hard
stop. Existing R7–R13 artifacts and the R11/R12 temporary scene pairs remain
immutable. The immediate pre-R14 porcelain baseline has `495` entries and
UTF-8 joined SHA-256
`4F2A7CF05FFA50EE5A0A3CF347B525C5FE7ABD2F420E1A08D00F3C06D91DEF7D`.
The four CUA source/test hashes in R12 remain the required baseline.

## Astra R15 unfiltered full-PlayMode completion authorization — 2026-09-28

The R14 host run was stopped after approximately 26 minutes when its log
remained at the M5D7M duplicate-adapter test for more than `180` seconds
while CPU rose. The R11 run had earlier been stopped after approximately
45 minutes. Subsequent approved diagnostics changed the interpretation of
that silence: R58's broad filtered selection passed `985/985` in
`944.3292552s`, and R62's R58-plus-HandoffMatrix selection passed `990/990`
in `1293.7224278s` with zero failed/skipped/inconclusive and natural exit.
The R62 XML SHA-256 is
`31D3552DF1B972F0F215C9E4120E907DAF3DD757B8FD8B478ABC111A2412E57B`;
its evidence is
`docs/verification/2026-09-28-m5d7m-r62-run-boundary-evidence.md`
(SHA-256 `15B39A7135B88BEA39E69977EE97DE19FE9518364AC02CC6E82F43380921E7C2`).
R62's temporary observer recorded D4 `TestFinished Passed`, then a quiet
interval of approximately `169` seconds before the HandoffMatrix fixture
started; all five HandoffMatrix cases subsequently passed. This interval
is only `11` seconds below the previous `180`-second stale-log stop threshold.
The observer has been removed, with its source/meta absent and the original
D4 test SHA-256 restored to
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
Luna independently confirmed restoration and the R62 result.

Sol and Luna therefore classify the earlier R11/R14 and M5D7M R59/R60/R61
stops as **watchdog-terminated, indeterminate**, not proven liveness defects.
This is a revised inference from new evidence, not a rewrite of their factual
logs or stop records. Historical successful unfiltered PlayMode took
`5097.83s` (about 85 minutes). The current full estimate is about `5041s`
from R58's `944.33s` plus the five historically costly omitted fixtures'
`4096.5s`, before startup and variance. Both R11 and R14 were stopped
before that normal full-suite duration.

**REQ-CUA-R15-001:** Subject to Luna's independent procedural pre-gate and
host permission review, Astra authorizes exactly one uninstrumented,
unfiltered Unity `6000.6.0f1` full PlayMode execution in the host environment.
Use `-batchmode -projectPath <repository> -runTests -testPlatform PlayMode
-testResults <R15 XML> -logFile <R15 log>` with no `-quit`, `-testFilter`,
assembly restriction, or temporary observer. Fresh, absent outputs are
`artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.xml`
and `.log`. Record the exact task-created Unity PID/tree. The expected
complete test inventory is approximately `1142` (`985+157`), subject to
actual XML confirmation.

**REQ-CUA-R15-002:** Use a single external monotonic `7200` second global
watchdog. The former D4 same-log `180` second watchdog is explicitly
withdrawn for R15. No unchanged-log or increasing-CPU observation alone
authorizes an early stop. If the global cap fires, stop only the exact
task-created Unity process tree, preserve the partial log and generated
scene pairs, and do not retry. An independently confirmed license/package
startup failure (the R14 two-cycle rule), compile failure, process exit
without XML, nonzero failed/skipped/inconclusive count, source hash drift,
or unexpected workspace mutation is a hard stop. Existing artifacts and
dirty state are immutable; do not clean or reset them.

**AC-CUA-R15-001:** One exact unfiltered, uninstrumented PlayMode run either
produces a complete XML and natural exit, or a bounded factual failure
record; no task process remains indefinitely and no retry occurs.
**AC-CUA-R15-002:** The complete XML, if produced, has expected fixture/test
inventory and failed/skipped/inconclusive `0`; hashes and workspace drift
are recorded. A partial log cannot satisfy `AC-CUA-009`.
**AC-CUA-R15-003:** Luna independently verifies the result and source
baseline; Astra alone decides whether the parent `AC-CUA-009` is met, using
the already-preserved focused PlayMode and full EditMode evidence as well.

The immediate pre-amendment porcelain has `517` entries, UTF-8 joined
SHA-256 `3A49F1EC7D2D2D0C4E3577E97DC9E1E585EC924093763E6A6E461A7321B1AFB4`.
The four CUA source/test hashes in the R12/R14 authorization remain the
execution baseline. This R15 amendment supersedes earlier stale-log stop
rules only for the one R15 run; it authorizes no source/test/product edits.
