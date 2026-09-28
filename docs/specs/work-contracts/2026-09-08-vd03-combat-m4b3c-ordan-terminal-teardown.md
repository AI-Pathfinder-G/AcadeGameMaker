---
status: Verified
---

# VD-03 M4B3C 오르단 terminal cleanup·boss-only teardown 작업 계약

- Owning specs: `VD-02`, `VD-03`, `SYSTEM-CONTRACTS`
- Assigned by / final authority: Astra
- Complex-design draft: Sol (`gpt-5.6-sol`, high reasoning, standard mode)
- Implementer: Terra
- Independent pre/post verifier: Luna
- Date: 2026-09-08
- Verified: 2026-09-08 — Luna independent verification PASS; Astra local integration acceptance. [Final evidence](../../verification/2026-09-08-vd03-combat-m4b3c-implementation-evidence.md) covers `AC-M4B3C-001` through `009`; full EditMode 430/430 and PlayMode 367/367 pass.
- Requirements: `REQ-COM-004`, `REQ-WT-003`, `REQ-WT-005`; protected `REQ-MOV-001`, `REQ-COM-003`, `REQ-COM-006`
- Acceptance criteria: `AC-M4B3C-001` through `AC-M4B3C-009`; parent `AC-COM-003`

## Goal and smallest safe closure

M4B3B3까지 검증된 오르단 graph는 보스 사망 source tick `t`의 결과와 최종 presentation handoff를 모두 발행한 뒤, `-180` scheduler가 네 Transfer 대상의 제거를 `t+1`에 예약하고 terminal-latch한다. 이 단위는 그 예약이 `t+1`의 `-200` Transfer owner에서 실제로 처리되었음을 별도 effect receipt로 증명하고, 곧이어 정확히 `-195`에서 보스 전용 실행기만 정지한다.

이 closure가 필요한 이유는 기존 terminal scheduler가 다음 advance를 거부하고, death bridge가 Combat `t+1`용 빈 delivery를 하나 남기기 때문이다. `-195`는 임의의 새 gameplay phase가 아니다. `-200` cleanup 완료 뒤이며 `-190` boss Combat 재진입 전인 유일한 현재 seam이다. teardown은 `t+1` Combat publication을 만들지 않고, 사망 tick `t`의 Combat·bridge·handoff publication을 그대로 보존한다.

이 단위는 보상 요청을 소비하거나 전달하지 않는다. 방 완료, 보상 지급, 입력 잠금/해제, choice 진입, room/run/scene 전환, 저장, UI, 카메라, VFX, 오디오도 소유하지 않는다. `Defeated → RewardRequest → RoomCompletionRequest` triplet은 teardown 자격 증거일 뿐이며 세 사건의 실제 소비는 후속 상위 생명주기 단위다.

## Frozen source truth and tick order

### Death source tick `t`

1. `-200` Transfer와 `-190` boss Combat가 tick `t`를 완료한다.
2. `-185` `OrdanBossSimulationDriver`만 M4A를 commit하고 정확한 ordinal triplet `Defeated`, `RewardRequest`, `RoomCompletionRequest`를 모두 source tick `t`로 한 번 발행한다.
3. `-180` `OrdanBossTransferExposureScheduler`는 위 exact triplet과 빈 payload/audit forecast를 검증하고, `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2` 순서의 네 `TransferTargetRemoved(t+1)`를 기존 exact terminal lane으로 한 번 예약한다. 모든 audit/payload sink availability와 collider는 false가 되고 scheduler가 terminal-latch한다.
4. `-170` hostile producer와 `-165` pull producer는 boss-dead terminal outcome만 발행한다. 새 hostile request나 pull directive를 만들지 않는다.
5. default-order player Movement는 tick `t`를 끝내며 이미 완료한 이전 상태를 rollback하지 않는다.
6. `+110` handoff는 tick `t`의 exact 완료 publication과 triplet, terminal scheduler·hostile·pull digest를 검증한 최종 immutable view를 반드시 먼저 게시한다.

### Cleanup and teardown tick `t+1`

1. `-200` `TransferSimulationDriver`가 예약된 exact four-ID batch를 처리한다. 활성 보스 대상이 있었다면 정확히 하나의 `TransferCleared(TargetRemoved)`와 player Baseline 복구를 낸다.
2. 새 `OrdanBossTerminalTeardown`은 `[DefaultExecutionOrder(-195)]`에서만 실행한다. exact Transfer cleanup effect, death tick `t`의 final handoff, exact empty boss Combat delivery `t+1`, graph identity와 아직 활성인 teardown 대상 전부를 mutation-free preflight한다.
3. preflight 성공 뒤에만 Combat의 exact empty `t+1` delivery를 discard하고 아래 boss-only component/collider set을 정지한 뒤 immutable teardown receipt를 게시하고 자신도 정지한다.
4. `-190`, `-185`, `-180`, `-170`, `-165`, `+110`은 tick `t+1`에 다시 실행하지 않는다. 따라서 stale boss publication을 갱신하거나 scheduler terminal 예외를 만들지 않는다.
5. `TransferSimulationDriver`, `PlayerMovementController`, player GameObject와 공용 registries는 살아 있으며 후속 tick을 독립적으로 진행할 수 있다.

`-195` teardown이 현재 Transfer publication을 지우거나 바꾸는 것은 금지한다. death tick Combat·bridge·handoff publication도 지우거나 `t+1`로 위조하지 않는다. 새 teardown receipt는 `DeathSourceTick=t`, `CleanupTick=t+1`을 서로 다른 필드로 가진다.

Coordinator는 preterminal normal tick에서도 `-195` callback을 받는다. scheduler가 아직 terminal이 아니면 mutation-free no-op하고 receipt를 발행하지 않는다. bootstrap에서 handoff가 아직 없는 상태도 합법적인 no-op다. 이 경로는 death triplet, cleanup receipt 또는 Combat terminal delivery를 요구하지 않으며 어떤 queue/component/collider도 만지지 않는다. scheduler terminal을 관측한 첫 tick에만 아래 terminal preflight로 진입한다.

Boss Combat owner는 teardown 뒤 더 진행하지 않는다. 따라서 보스와 플레이어의 마지막 Combat snapshot은 death tick `t`의 immutable terminal evidence로 동결되며, 살아 있는 player `CombatTarget` component가 곧바로 새 M1 health owner가 되는 것은 아니다. 후속 room/choice combat 재개는 별도 Approved 계약이 player health를 명시적으로 넘기거나 새 owner를 세워야 한다. M4B3C는 이후 player damage, invulnerability 또는 Combat tick 연속성을 주장하지 않는다.

## Transfer cleanup effect receipt

`LatestCompletedInputRemovalIds`는 입력 요청 목록일 뿐 removal 효과의 증거가 아니다. Terra는 `TransferSimulationDriver`가 `-200` commit과 같은 경계에서만 게시하는 internal immutable `BossTerminalTransferCleanupReceipt`를 추가한다.

Receipt는 다음 값을 가진다.

- `DeathSourceTick`, `CleanupTick`
- exact defensive-copied requested IDs 네 개
- `Disposition`: `Removed` 또는 `LifecycleSuperseded`
- nullable exact lifecycle reason
- cleanup 전 active boss target ID 또는 empty
- cleanup 후 `ActiveTargetId`, `PlayerState`, `Revision`
- active boss target이 있었다면 disposition과 일치하는 exact `TransferCleared` proof, 아니면 null
- 네 ID가 Transfer session registration에서 실제로 사라졌는지 나타내는 committed effect proof

Normal `Removed`는 input lifecycle이 null이고, 네 exact ID가 Transfer session registration에는 모두 **없고** durable removed-ID set에는 모두 **존재한** 뒤에만 게시한다. 후속 capture는 frozen Unity registry를 유지하되 이 네 ID를 후보로 되살리지 못한다. active boss target이 있었다면 clear reason은 `TargetRemoved`, previous ID와 tick은 exact, revision은 정확히 `+1`, player는 Baseline/empty active여야 한다. active target이 없었다면 removal result는 null이고 revision을 억지로 증가시키지 않는다.

`RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted`가 같은 cleanup tick을 소유하면 기존 lifecycle-first 규칙이 네 permanent removal 요청보다 우선한다. 이때 receipt는 `LifecycleSuperseded(reason)`이고 permanent removal effect를 주장하지 않는다. active boss target이 있었다면 optional clear proof의 reason은 해당 exact lifecycle reason이고 `TargetRemoved`여서는 안 된다. active target이 없었다면 lifecycle clear proof는 null일 수 있다. Transfer session 전체 cleanup과 player Baseline은 기존 lifecycle 결과로 검증하며 네 ID의 durable removed-ID membership은 false다. M4B3C는 lifecycle을 생성하거나 재시도하지 않고, boss-only executors를 quiesce할 수만 있다. 이 분기는 전체 제품의 boss-to-choice 전환이나 계속 플레이 가능성을 증명하지 않는다.

Receipt는 exact four-ID terminal request가 없거나, input에 exposure end가 섞였거나, ID/tick/order가 다르거나, session/durable set/post snapshot이 disposition과 맞지 않으면 게시하지 않는다. public Transfer API와 `TransferSessionSnapshot` ABI는 바꾸지 않는다.

### Terminal-lane preflight before `-200` mutation

위 guarantee는 generic Transfer attempt의 사후 검사로 만들지 않는다. `MergeExactBossTerminalRemovals`가 연 exact terminal lane에 한해서, `TransferSimulationDriver`는 cleanup tick `t+1`의 queue를 `ConsumeOrEmpty`와 `TransferSession.Process` 전에 mutation-free preflight한다.

- queue에는 exact ordered four-ID removal batch가 한 번만 있어야 하며 모든 item tick은 `t+1`이다.
- exposure-end 목록은 empty여야 하고 foreign/duplicate removal, wrong tick/order와 두 번째 terminal reservation은 거부한다.
- lifecycle은 null 또는 기존 네 lifecycle reason 중 정확히 하나여야 한다. preflight가 null이면 expected disposition은 `Removed`, non-null이면 `LifecycleSuperseded(exact reason)`으로 동결한다. lifecycle을 제거하거나 removal로 재분류하지 않는다.
- invalid terminal envelope는 queue consume, TransferSession/sink/player mutation과 receipt publication 전에 거부한다. 해당 terminal input은 소비하지 않는다.
- 이 강화는 exact boss terminal lane에만 적용한다. 일반 Transfer input의 기존 consume-on-attempt와 실패 의미는 바꾸지 않는다.

Valid preflight 뒤 `-200` commit은 기존 순서로 진행하며 receipt는 committed state와 frozen expectation을 대조한다. 이 사후 대조가 실패하면 이미 commit된 Transfer 결과와 publication은 보존하고 rollback하지 않는다. teardown/Combat discard/component disable은 시작하지 않고 예외로 fail-stop하며, emergency root disable이나 추측 복구를 하지 않는다.

## Exact boss-only teardown set

성공 commit은 아래만 변경한다.

- enabled `false`: `OrdanBossCombatSimulationDriver`, `OrdanBossSimulationDriver`, `OrdanBossTransferExposureScheduler`, `OrdanBossHostileDamageProducer`, `OrdanBossBalanceAuditPullProducer`, `OrdanBossEncounterHandoffAdapter`, authored Ordan의 `CombatTarget`
- collider enabled `false`: authored Ordan body collider와 네 `BossAuditBox/BossWeight` target collider
- exact empty `t+1` boss Combat delivery discard
- 새 teardown component의 immutable latest receipt 게시 후 자기 자신 enabled `false`

다음은 변경하거나 파괴하지 않는다.

- encounter root, `Systems`, player GameObject의 active state
- `PlayerMovementController`, player Rigidbody2D/collider/CombatTarget, player transform·health·motion snapshot
- `TransferSimulationDriver`, `TransferTargetRegistry`, `CombatTargetRegistry`
- 네 `TransferTarget`과 modifier sink component, authored Rigidbody2D, serialized registry reference와 ordinal order
- death tick `t`의 Combat/bridge/handoff values

어떤 GameObject에도 `SetActive(false)`를 호출하지 않고 `Destroy`/`DestroyImmediate`를 사용하지 않는다. encounter root에 player가 포함되어 있으므로 root 또는 `Systems` 비활성화는 계약 위반이다. Ordan Rigidbody2D의 `simulated`, body type, velocity와 transform도 바꾸지 않는다. 등록 객체를 없애지 않고 collider와 tick-producing component만 정지하여 frozen registries가 null reference가 되지 않게 한다.

## Combat terminal delivery closure

Death bridge는 기존 계약에 따라 source `t`에서 Combat `t+1` delivery를 정확히 하나 commit한다. M4B3C는 `OrdanBossCombatSimulationDriver`에 teardown-owner-bound mutation-free preflight와 one-shot discard seam을 추가한다.

- batch는 `SourceTick=t`, `DeliveryTick=t+1`, payload request null, hostile request count 0, `HostileAppendCommitted=false`여야 한다. 이 네 값 전체가 exact empty delivery fingerprint다.
- optional `t+1` `OrdanBossCombatInput`은 `Tick=t+1`, `Camera=null`, `Aim=null`, `Press=null`, `ExternalDamageRequests.Count=0`인 exact empty shape만 허용한다. 이 exact empty input이 있으면 empty delivery와 같은 closure에서만 제거할 수 있다. attack, aim, press, camera 또는 external damage가 하나라도 있으면 자동 폐기하지 않고 전체 teardown을 mutation 없이 거부한다.
- key가 `t+1`보다 큰 어떤 Combat input 또는 delivery가 이미 queue에 있으면 teardown을 거부한다. 성공 closure는 exact empty `t+1` input/delivery 외의 key를 읽어 소비하거나 변경하지 않는다.
- preflight candidate는 teardown owner identity, source/delivery tick, empty-batch fingerprint를 묶는다. exact candidate commit 뒤에만 delivery를 제거한다.
- duplicate/forged/cross-wired discard, non-empty delivery, missing delivery, wrong horizon은 기존 queue와 모든 component enabled state를 보존한 채 fail-stop한다.
- M1 health, canonical damage trace, death Combat outcome은 다시 계산하지 않는다.

## Teardown receipt and failure semantics

`OrdanBossTerminalTeardownReceipt`는 Unity object reference 없이 다음을 방어 복사한다.

- terminal-consumption key: authored ASCII encounter key `OrdanBossEncounterGraph`와 `DeathSourceTick=t`
- `DeathSourceTick=t`, `CleanupTick=t+1`
- exact ordered handoff kinds
- Transfer cleanup disposition과 optional lifecycle reason
- optional `TransferCleared` identity/revision
- discarded empty Combat delivery identity
- disabled component semantic IDs와 disabled collider authored IDs
- `PlayerPreserved=true`, `Terminal=true`

모든 외부 값과 component/collider 상태를 preflight한 뒤에만 commit한다. 검증 실패 시 Combat delivery, components, colliders, prior Transfer/handoff publications와 이전 teardown receipt를 모두 보존하고 예외로 fail-stop한다. 성공 commit은 one-shot이며 이후 direct advance는 거부한다. Unity enabled/collider assignment는 preflight 뒤 정해진 값 쓰기만 하며 callback, physics query/sync 또는 scene discovery를 호출하지 않는다.

같은 `(OrdanBossEncounterGraph,t)` key는 해당 coordinator instance에서 한 번만 수락한다. owner identity와 latch는 instance-local이며 static/global registry를 만들지 않으므로, 독립된 두 authored graph가 같은 source tick을 사용해도 서로 중복으로 오인하지 않는다. `Removed` disposition만 normal cleanup success를 뜻한다. `LifecycleSuperseded`는 더 강한 lifecycle이 Transfer removal을 대신 처리했다는 quiescence 기록일 뿐이며 reward/room/choice 성공 경로를 열거나 M4B3C 완료를 주장하지 않는다.

`PlayerPreserved`는 이 단위가 player를 deactivate/destroy/mutate하지 않았다는 뜻일 뿐이다. 입력 모드, choice 준비, room completion 또는 제품 수준의 자유 이동을 의미하지 않는다.

## Builder and validator

- `OrdanBossEncounterAuthoringBuilder`만 기존 `Systems`에 `OrdanBossTerminalTeardown`을 정확히 하나 추가하고 Transfer, boss Combat, bridge, scheduler, hostile, pull, handoff, Ordan target/collider와 네 target collider를 명시적으로 연결한다.
- validator는 exact-one teardown component, execution order `-195`, same-root identities, exact downstream set, collider IDs/order, existing `-200 → -190 → -185 → -180 → -170 → -165 → default → +110` values와 new conditional terminal seam을 read-only 검증한다.
- validator는 teardown을 실행하거나 component를 끄거나 delivery를 discard하지 않는다.
- 새 GameObject, hierarchy child, layer, tag, renderer, collider, Rigidbody, project/scene setting은 추가하지 않는다.

## Requirements and acceptance evidence

- `AC-M4B3C-001`: focused EditMode가 exact death triplet source `t`, scheduler exact four-ID `t+1` reservation, hostile/pull terminal suppression과 `+110` final handoff 선행을 증명한다.
- `AC-M4B3C-002`: focused Transfer tests가 normal cleanup의 request/effect 구분, exact registration/durable removal, active audit/payload 각각의 single `TargetRemoved` clear·Baseline·revision과 inactive no-clear/no-revision을 증명한다.
- `AC-M4B3C-003`: focused Transfer tests가 lifecycle-first `LifecycleSuperseded` receipt, permanent-removal 비주장과 exact lifecycle clear reason을 증명한다. boss terminal lane의 malformed/mixed/stale queue는 `-200` consume/session mutation 전에 거부되어 receipt와 teardown mutation을 만들지 않는다. valid preflight 뒤 receipt-only mismatch는 committed `-200` 결과를 보존하고 추가 teardown mutation 없이 fail-stop하며 rollback을 주장하지 않는다. 일반 Transfer consume-on-attempt 의미는 bit-identical하다.
- `AC-M4B3C-004`: focused EditMode가 `HostileAppendCommitted=false`를 포함한 exact empty Combat `t+1` delivery preflight/discard, non-empty/missing/stale/forged/duplicate/cross-owner 거부와 death Combat outcome 보존을 증명한다.
- `AC-M4B3C-005`: authored PlayMode가 bootstrap과 모든 preterminal `-195` callback의 mutation-free no-op, `-200 cleanup → -195 teardown` 순서, tick `t+1` downstream 미실행, instance-local one-shot receipt와 final handoff tick `t` 보존을 증명한다.
- `AC-M4B3C-006`: authored PlayMode가 exact boss component/collider set만 꺼지고 root/Systems/player/Transfer/Movement/registries와 serialized references가 보존됨을 증명한다. `SetActive`/Destroy/scene load/unload 금지 static guard는 새 `OrdanBossTerminalTeardown` runtime 파일과 M4B3C가 추가한 runtime hunks만 검사한다. 기존 authoring builder의 합법적인 asset rebuild 코드는 검색 범위에서 제외하고 validator가 결과 graph를 별도로 검사한다.
- `AC-M4B3C-007`: authored PlayMode가 teardown 뒤 최소 두 개의 Movement tick과 Transfer tick을 더 진행하여 player snapshot tick 연속성, 동일 player GameObject/body/collider, Baseline 및 boss target 재선택 불가를 증명한다. boss Combat owner와 M1은 더 진행하지 않고 death tick player/boss Combat snapshot이 그대로 동결됨도 증명한다. 이는 sandbox local survival만 증명하며 future room Combat 재개를 뜻하지 않는다.
- `AC-M4B3C-008`: wrong handoff order/tick, missing final handoff, cleanup effect 없는 input-only proof, wrong graph/root/collider, pre-disabled partial graph, second teardown에서 preflight가 아무 상태도 바꾸지 않고 fail-stop함을 증명한다.
- `AC-M4B3C-009`: builder/validator, 30/60/144 cadence terminal traces, focused/full EditMode·PlayMode가 parent `AC-COM-003`, M4A, M4B1~M4B3B3와 Transfer/Movement regressions을 유지함을 증명한다.

각 테스트 이름과 결과는 위 AC ID를 직접 인용한다. Luna 증적은 first-divergent tick, final handoff hash, cleanup receipt, disabled-set digest, post-teardown player/Transfer tick을 포함한다.

## Exact implementation allowlist

Runtime:

- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs` — final-view exact read seam만 허용
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs` and `.meta`

Authoring assets:

- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs`
- generated `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab`
- generated `Assets/Scenes/OrdanBossEncounterSandbox.unity`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossTerminalTeardownContractTests.cs` and `.meta`
- new `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossTerminalTeardownPlayModeTests.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossEncounterAuthoringTests.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterAuthoredGraphPlayModeTests.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterHandoffAdapterPlayModeTests.cs` — existing death fixture must let the new automatic teardown own shutdown instead of pre-disabling scheduler/handoff; retain terminal handoff assertions.
- `Assets/AcadeGameMaker/Tests/PlayMode/TransferUnity/TransferSimulationDriverPlayModeTests.cs`

Evidence/documentation:

- this contract
- new `docs/verification/2026-09-08-vd03-combat-m4b3c-contract-pregate.md`
- new `docs/verification/2026-09-08-vd03-combat-m4b3c-implementation-evidence.md`
- `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`
- `docs/README.md`

All other files and components are forbidden. In particular Movement runtime, player prefab, combat/transfer public contracts, reward/room/run/save/input/UI/camera/presentation assets, project settings and media are outside the allowlist.

## Rollback point

- Read-only baseline commit: `309f2204cf19a321ae74c92f3be0e3fc94e3499e`
- The worktree already contains unrelated user changes and untracked media. Do not reset, clean, delete or overwrite them.
- Rollback means removing only new M4B3C files and inversely applying only the M4B3C hunks in allowlisted files. `git reset --hard`, broad checkout and asset deletion are forbidden.

## Required ownership and evidence

- Terra: local impact note, implementation, builder/validator, fixtures/tests, generated prefab/scene diff and REQ-tagged implementation evidence.
- Luna pre-gate: adversarial review of request-vs-effect proof, lifecycle supersession, final-handoff-before-teardown, exact disabled set, player/root preservation, delivery discard and failure atomicity.
- Luna post-gate: independent focused/full tests, cadence trace, static destructive-action guard, diff/asset inspection and AC-tagged evidence digest. Terra cannot accept its own work.
- Sol: this bounded complex-design draft and architecture counter-review only. Sol has no final approval or integration authority under ADR-0027.
- Astra: contract approval, conflict resolution and final integration.
- Ollama: recalled by ADR-0027; no call, probe, retry, schedule or utilization ledger entry.

Current subagent controls exposed model and reasoning effort but no Pro-mode control. This draft used actual Sol high reasoning in standard mode; it does not claim Pro and authorizes no separately billed API route.

## Stop conditions

Stop and return to Astra if implementation needs any of the following:

- root/Systems/player GameObject activation change, object destruction or scene transition
- reward, room completion, run, choice, save, input mode, UI/camera/VFX/audio behavior
- public Transfer/Movement/Combat ABI change
- `t+1` boss Combat advance or death outcome recomputation
- non-empty pending Combat delivery/input discard
- lifecycle precedence change or permanent-removal claim on `LifecycleSuperseded`
- Transform/Rigidbody motion, physics query/sync/callback, new collider/body or project setting
- registry removal/reordering/nulling, target/sink destruction or re-registration
- broader component disable set than the exact list above

## Approval gate

Approved by Astra on 2026-09-08 after Luna independent contract pre-gate PASS (P0=0, P1=0, P2=0). Terra may implement only the exact allowlist and acceptance scope above. Implementation and independent runtime verification remain outstanding; this is not a Verified claim. Sol supplied the standard-mode design draft, not Pro execution or final approval.

Astra approved a test-only allowlist amendment on the same date after full PlayMode exposed the legacy death fixture pre-disabling scheduler/handoff before the new automatic coordinator. The amendment preserves production behavior and existing assertions; it permits updating only that fixture's shutdown ownership and adding terminal receipt assertions (`AC-M4B3C-005`, `AC-M4B3C-009`).
