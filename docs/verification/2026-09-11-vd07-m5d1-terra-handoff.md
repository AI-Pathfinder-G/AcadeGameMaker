# M5D1 오르단 플레이어블 터미널 전환 — Terra 구현 인계

- 상태: **Implemented — Luna 독립 검증 및 Astra 통합 수용 대기**
- 기준 계약: `docs/specs/work-contracts/2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md`
- 구현자: Terra
- 수용 권한: Astra (구현자 자기 수용 금지)

## R1 bounded correction — Luna P1-001 / P1-002 대응

- `ReferencesShareExactAuthoredGraph()`는 이제 router의 bound Transfer가 null이 아니고, requester와 같은 `OrdanBossEncounterGraph` root에 속하며, 동일 player를 bind함까지 확인한다. 이 read-only getter 검증은 InputRouter mode/frame/map 동작을 변경하지 않는다.
- requester fixture는 foreign Transfer cross-wire 초기화 거부와 원상 보존을 직접 검사한다. 또 duplicate `AdvanceForTests`가 router `-210` commit 전 재요청/가짜 완료를 만들지 않는 경로, disable→reenable, source tick `long.MaxValue` overflow, router fault를 각각 검증해 request/retry/forged completion이 없음을 assertion 한다.
- duplicate observation은 requested tick보다 이전의 authoritative receipt를 단순 대기로 처리한다. 따라서 duplicate callback/test seam이 `-210` 전 receipt를 mismatch로 오인해 requester를 조기 종료하지 않으며, 이후 exact `Transition@t+1`만 completion proof가 될 수 있다.
- 이 R1은 M5D1 allowlist 안의 requester runtime, dedicated requester PlayMode test 및 Terra handoff에만 한정된다. **Luna 재검토 대기**.

## R2 compile-scope correction — reauthorized Unity compiler findings

- The core `AcadeGameMaker.Combat` assembly, which owns `OrdanBossHandoffKind` and `OrdanBossHandoff`, grants `AcadeGameMaker.Input.Unity` only the read access required for the immutable terminal triplet used by requester evidence validation. No Combat mutation seam was introduced. (The Unity adapter assembly's pre-existing requester friend remains unchanged.)
- The existing authored Combat PlayMode test assembly now explicitly references `AcadeGameMaker.Camera.Unity` and `AcadeGameMaker.Input.Unity`; its two corresponding assemblies grant that one test assembly access to the pre-existing internal authored component types it already reads. These are compile-only test visibility adjustments, not runtime behavior changes.
- The single reauthorized focused PlayMode attempt reached actual compiler execution and exposed the core-assembly ownership correction above; no XML was produced. Per the one-attempt bound, a second Unity invocation is deferred to Luna. **Implementation corrected; Luna re-review pending.**

## R3 exact compile correction — second reauthorized compiler report

- `Combat.Authoring.Editor` now references the `AcadeGameMaker.Camera` core assembly in addition to its existing Camera.Unity and URP runtime/2D references; this resolves `CameraCenterBounds` and the existing `PixelPerfectCamera` authoring use without package changes.
- Requester overflow coverage now uses the actual `SimulationTick` maximum (`int.MaxValue`). The two Transfer phase assertions now call the void test seam and inspect its immutable `LatestPhasePublication` read instead of treating the seam as a return value.
- The core Combat assembly additionally grants only the dedicated Input.Unity PlayMode test assembly the existing internal handoff value types needed for its adversarial overflow fixture.
- No Unity invocation was run for R3, as directed. **Implementation corrected; Luna re-review pending.**

## R4 exact compile correction — remaining validator visibility and URP type resolution

- Added the existing Universal Render Pipeline namespace import to `OrdanBossEncounterAuthoringValidator`, matching the builder's compiled PixelPerfectCamera reference path.
- Added the narrowly scoped Camera core friend declaration for `AcadeGameMaker.Combat.Authoring.Editor`, which permits that validator to inspect the internal fixed-camera bounds value without expanding runtime access.
- The Camera core assembly reference and existing URP assembly references were already present in the authoring editor asmdef; no other reference or behavior changes were made.
- No Unity invocation was run for R4, as directed. **Implementation corrected; Luna re-review pending.**

## R5 exact compile correction — authoring-test Camera namespace ambiguity

- Qualified the two authoring-test component references as `UnityEngine.Camera`; the imported `AcadeGameMaker.Camera` namespace otherwise shadows the Unity component type.
- No authored graph, runtime behavior, or assembly reference changed.
- No Unity invocation was run for R5, as directed. **Implementation corrected; Luna re-review pending.**

## R6 focused-test fixture correction — authored root identity

- Focused requester execution compiled and ran: one test passed and six failed solely because `Instantiate(asset)` appends `(Clone)` to the root name, which correctly fails the production authored-root identity gate.
- `CreateRig` now restores the cloned fixture root name to the exact `OrdanBossEncounterGraph` authored key before any component discovery or initialization. This is test-fixture setup only; it does not relax production validation or alter the foreign-transfer cross-wire fixture.
- No Unity invocation was run for R6, as directed. **Implementation corrected; Luna re-review pending.**

## R7 focused-test correction — locked coordinated empty frame

- The R6 focused result reached behavior: six tests passed; the remaining assertion incorrectly expected no queued Transfer/Combat entries immediately after the router commits locked `Transition@t+1`.
- The test now proves the intended coordinated receipt semantics: Transfer has the exact `requestedTick` envelope while player commands remain empty. Combat terminal-retirement behavior is clarified by R10 below.
- Runtime receipt handling remains unchanged; the reserved Transfer input is intentionally consumed by its ordered `-200` phase.
- No Unity invocation was run for R7, as directed. **Implementation corrected; Luna re-review pending.**

## R8 exact compile correction — IReadOnlyList assertions

- Replaced the requester fixture's `Is.Empty` collection matchers with explicit `.Count == 0` assertions for Transfer removals/exposure ends and Combat external damage requests, matching the Unity-bundled NUnit assertion API.
- No test behavior or runtime code changed.
- No Unity invocation was run for R8, as directed. **Implementation corrected; Luna re-review pending.**

## R9 focused-test correction — terminal cleanup merged with locked frame

- The R9 focused requester result was six passes and one assertion failure: the `Transition@t+1` Transfer input correctly retained the four cleanup removals that terminal teardown had already reserved at death tick `t`.
- The requester fixture now distinguishes Transfer gameplay input from the teardown-owned system payload. It proves camera, aim, press, lifecycle, and exposure ends are empty; proves the exact ordinal cleanup list `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2` is retained with `requestedTick`; then runs Transfer's ordered `-200` phase and proves that exact queue entry is consumed. Retired Combat receives no new frame.
- This aligns REQ/AC-M5D1-003's locked interactive frame with the pre-existing terminal teardown authority. No runtime behavior changed.
- No Unity invocation was run for R9, as directed. **Implementation corrected; Luna re-review pending.**

## R10 focused-test correction — retired Combat receives no frame

- The R10 focused result was six passes and one assertion failure because terminal teardown had already retired Combat before InputRouter prepared `Transition@t+1`.
- The exact-death fixture now proves the stronger intended behavior: Transfer retains its exact teardown cleanup envelope, while the Combat input queue remains empty because no Combat candidate or commit is issued after retirement.
- No runtime behavior changed. No Unity invocation was run for R10, as directed. **Implementation corrected; Luna re-review pending.**

## R11 focused-EditMode correction — precise direct-input authority guard

- Focused requester PlayMode is now **PASS 7/7**. Focused authored EditMode reached 21/22 and exposed a static-source guard false positive: the broad `Input.` token matched the required `using AcadeGameMaker.Input.Unity` namespace for authored `InputRouter` validation.
- The negative-source check now rejects direct Unity input authority precisely: `UnityEngine.Input.`, `Input.Get`, `GetKey`, and `GetMouse`. The nearby no-discovery, no-render-time, no-physics-query, no-scene-load, and no-mutation guards remain unchanged.
- This permits only the required typed router namespace; it does not permit direct device polling. No runtime behavior changed, and no Unity invocation was run for R11. **Implementation corrected; Luna re-review pending.**

## R12 full-PlayMode cascade correction — runtime clone identity versus authored identity

- Full EditMode is **PASS 483/483**. Full PlayMode reported 404 pass / 170 fail. The dominant cascade begins when existing tests instantiate the approved prefab as `OrdanBossEncounterGraph(Clone)`: requester `Start` used the authored-root name proof and threw before registration, leaving deferred objects whose subsequent phases generated collateral errors such as duplicate stable collider keys.
- The requester now has two non-overlapping read-only proofs. Explicit authoring validation and `InitializeForTests` still require the exact authored root name; runtime `Start` validates the same root/player/router/transfer/combat/handoff bindings without treating Unity's clone suffix as a cross-wire. Completion receipts retain the fixed `OrdanBossEncounterGraph` key.
- A malformed runtime graph now closes requester startup without an exception, registration, pending request, retry, or completion receipt. Dedicated PlayMode coverage proves both a valid unrenamed clone's no-op startup and a foreign-Transfer clone's safe fail-closed startup; the existing explicit authoring/test-init cross-wire rejection remains strict.
- Review of the XML's remaining `TransitionRequired` logs found eleven regular-enemy test cases and five Ordan cases after the original unhandled requester-start failures. They are downstream residual logs from the failed/deferred fixture cleanup, not a basis to alter InputRouter runtime behavior outside this contract. The next bounded full rerun must confirm they disappear with the requester-start cascade.
- No Unity invocation was run for R12, as directed. **Implementation corrected; Luna re-review pending.**

## R13 legacy manual-phase fixture adaptation — disable new authored auto drivers

- R12 focused requester PlayMode is **PASS 9/9**. The next full PlayMode run reached 575/576, with only `OrdanBossAuditExposurePlayModeTests.AuthoredGraphExposesAuditAtBalanceAuditExecuteHorizonOnly` failing.
- That legacy fixture manually disables and steps the pre-existing player/Transfer/Combat lanes, then yields. It now explicitly retrieves, asserts, and disables the newly authored `SimulationCameraDriver`, `InputRouter`, and terminal requester before manual stepping. This prevents their lifecycle callbacks from observing an intentionally disabled player or injecting concurrent frame ownership.
- The same helper is used by every manual phase setup in the fixture. Assembly references already existed; only the necessary Camera/Input namespace imports were added. No runtime behavior changed.
- No Unity invocation was run for R13, as directed. **Implementation corrected; Luna re-review pending.**

## R14 final execution evidence — all suites green, independent acceptance pending

- Reauthorized Unity `6000.6.0f1` execution is green: focused requester PlayMode **9/9**, focused authored EditMode **22/22**, focused legacy audit exposure PlayMode **1/1**, full EditMode **483/483**, and full PlayMode **576/576**; every report has zero failed, skipped, and inconclusive cases.
- This supersedes the earlier licensing-blocked execution note in this handoff. It is Terra implementation evidence, not Terra self-acceptance; Luna independent re-review and Astra integration acceptance remain required.

## 구현 범위

- `OrdanBossTerminalTransitionRequester`는 `DefaultExecutionOrder(-211)`의 유일한 sandbox-local mode requester다. 동일 authored graph의 terminal handoff ordered triplet, completed boss-death outcome, `GameplayEnabled@t` router receipt 및 `Player.NextExpectedTick == t+1`을 모두 확인할 때만 `Transition@t+1`을 요청한다.
- router의 `Transition@t+1` immutable receipt를 다음 관찰에서 대조한 뒤 source/requested/committed tick, mode epoch, `OrdanBossEncounterGraph` key를 포함한 completion receipt 하나를 게시하고 닫힌다. incomplete/stale/cross-wire/overflow/conflict/fault 경로는 요청 또는 completion을 만들지 않고 fail-closed한다.
- builder/validator는 한 `Systems`의 exact router/requester 및 별도 `Camera` child (`Camera`, `PixelPerfectCamera`, `SimulationCameraDriver`)를 검증한다. authored initial mode는 `GameplayEnabled`; camera는 fixed center `(0,81)` pixel18과 inclusive singleton bounds, PPU 18 / 640×360 / Windowbox / UpscaleRenderTexture / Point / orthographic 10 profile이다. builder가 정의한 deterministic serialized graph와 file-ID reference closure는 정적으로 확인했고, R14의 reauthorized focused/full execution이 이를 회귀 검증했다.
- `OrdanBossEncounterAuthoringTests`에 missing router, router-camera cross-wire, camera profile drift 구조 변이를 추가했다. 새 requester PlayMode fixture는 real authored combat/handoff/router graph에서 normal no-op, exact death `t → t+1`, locked empty payloads, completion receipt 및 conflicting pending mode fail-closed를 다룬다.

## REQ / AC 추적

| 요구사항 | 구현·테스트 근거 | 상태 |
|---|---|---|
| REQ-M5D1-001 / AC-M5D1-001 | builder/validator exact component order and bindings; focused authored EditMode 22/22; full EditMode 483/483 | 실행 PASS; Luna/Astra 수용 대기 |
| REQ-M5D1-002 / AC-M5D1-002 | requester evidence gate, clone/cross-wire no-authority, no-op/conflicting evidence; focused requester PlayMode 9/9 | 실행 PASS; Luna/Astra 수용 대기 |
| REQ-M5D1-003 / AC-M5D1-003 | exact `Transition@t+1`, epoch +1, locked Transfer cleanup envelope/retired Combat assertions; focused requester PlayMode 9/9 | 실행 PASS; Luna/Astra 수용 대기 |
| REQ-M5D1-004 / AC-M5D1-004 | immutable completion receipt, duplicate/disable-reenable/overflow/router-fault paths; focused requester PlayMode 9/9 | 실행 PASS; Luna/Astra 수용 대기 |
| REQ-M5D1-005 / AC-M5D1-005 | authored fixed camera/profile validator; focused authored EditMode 22/22 and full suites | 실행 PASS; Luna/Astra 수용 대기 |
| REQ-M5D1-006 / AC-M5D1-006 | all focused reports plus full EditMode 483/483 and PlayMode 576/576, zero fail/skip | 실행 PASS; Luna/Astra 수용 대기 |

## 최종 실행 기록

- Unity: `6000.6.0f1` (project-pinned, reauthorized execution).
- Earlier licensing-blocked logs are historical and superseded by the following XML evidence.

| 범위 | 결과 | XML SHA-256 |
|---|---:|---|
| Focused requester PlayMode | 9/9 pass, 0 fail/skip | `requester-playmode-unsandboxed-r12.xml` — `576E33BC1E4ADFFFDC605661F339602D316DBAD44D85E19A206F9B8CEBA06861` |
| Focused authored EditMode | 22/22 pass, 0 fail/skip | `authoring-editmode-unsandboxed-r2.xml` — `0FD455BACBF9F96F82AB2D2A61F3D865BCE50174960BF8093FCC71337DA7A569` |
| Focused legacy audit exposure PlayMode | 1/1 pass, 0 fail/skip | `audit-exposure-playmode-r13.xml` — `7FFD177EF23D49563A98E2F5CCE1E023ACA24A7372053A5B5077314D45586EF9` |
| Full EditMode | 483/483 pass, 0 fail/skip | `full-editmode-unsandboxed.xml` — `AF4F81E124D4B90A9FE34F58F8D53FDE8899C1534D960545C3F9C0F7CEED524A` |
| Full PlayMode | 576/576 pass, 0 fail/skip | `full-playmode-unsandboxed-r3.xml` — `67BD473C6994B338D1CC9FE303F9591ACA114C52C1689E9D264434D87380BD40` |

- Static hygiene: scoped M5D1 `git diff --check` passed. The historical workspace-wide warning for pre-existing `ProjectSettings/ProjectSettings.asset` remains outside this contract's allowlist.

## Luna handoff checklist

1. Independently inspect the green focused/full XML hashes and zero fail/skip totals above.
2. Review requester one-shot/disable-reenable/overflow/conflicting-requester and clone/cross-wire safe-start coverage, plus generated prefab/scene component and child order.
3. Record AC-tagged independent findings and submit them to Astra. This document does not claim `Verified` or final integration acceptance.
