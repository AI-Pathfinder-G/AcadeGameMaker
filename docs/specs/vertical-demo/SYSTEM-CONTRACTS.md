# Vertical Demo System Contracts

- Status: Approved
- Owner and approval: Sol
- Consumers: VD-01 through VD-10
- Independent contract review: Luna PASS, 2026-08-25
- Approved: 2026-08-25
- Last updated: 2026-09-08 (M4B3C terminal seam and M5B5 Verified opt-in input-phase annotation; Astra approval, Luna PASS)
- Decision basis: [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md)

이 문서는 단위 시스템이 공유하는 상태, 명령, 사건, 생명주기의 유일한 소유자를 정한다. 아래 이름과 필드는 언어 독립 계약이며 실제 C# 타입 이름은 동일 의미를 보존해야 한다.

## Authoritative clock and identity

- 권위 게임플레이 시간은 60 Hz `SimulationTick` 정수다.
- 프레임에서 수집한 입력은 다음 FixedUpdate 틱에 한 번 소비한다.
- 네트워크 시간, wall clock, Unity instance ID, 생성 순서는 결정 키로 사용하지 않는다.
- 공개 ID는 저작된 비어 있지 않은 ordinal 문자열이며 같은 범위에서 고유해야 한다.
- 공개 payload는 ID, enum, 정수와 계약에 명시한 벡터를 사용한다. 벡터 소비자는 판정 전에 계약 단위로 양자화하며 scene object 또는 Component 참조를 담지 않는다.

## State ownership

| State | Sole owner | Read-only consumers | Must not own |
|---|---|---|---|
| Player motion, velocity, grounded and air-action state | VD-01 Movement | Weight transfer, combat, UI | Run result, choice |
| Active transfer and target modifier | VD-02 Weight transfer | Movement, combat, room hazards, UI | Target health, room order |
| Health, damage dedupe, enemy and boss life state | VD-03 Combat | Run, UI | Transfer session, persistent choice |
| Seed, immutable room plan, socket plan, current room | VD-04 Expedition assembly | Run, combat, UI, verification | Player physics, choice |
| Run phase, run result, failure/return transaction | VD-05 Failure and persistence | Combat, UI, verification | Player physics, skill effect |
| Confirmed choice, consent result, granted skill, skill cooldown | VD-06 Choice | Combat, movement, UI, persistence | Run phase, health |
| Input mode, aim camera snapshot and presentation state | VD-07 Input/UI | all command producers, weight transfer | Authoritative gameplay state |
| Versioned persistent snapshot and atomic file transaction | VD-09 Persistence adapter | VD-04 seed selection, VD-05 restore, VD-06 choice | Live scene objects, active transfer |

소비자는 원본 상태를 직접 수정하지 않고 아래 명령 또는 사건만 사용한다.

VD-07 camera presentation state는 `GameplayFollow`, `AuthoredAnchor` mode, current anticipation offset, 12-tick transition, snapped pose와 active room bounds를 포함한다. GameplayFollow는 movement owner의 완료된 player pose를 읽은 뒤 같은 tick 끝에 6×4u dead zone과 속도 threshold 기반 ±3u horizontal·+1.5/-3u vertical offset을 계산한다. offset target band 변화는 현재 값에서 12-tick linear transition을 재시작한다. 처리 순서는 focus offset → dead-zone desired center → axis-independent max 0.5u/tick MoveTowards → room clamp → 각 축 1/18u AwayFromZero snap이다. room entry·respawn·teleport·authored anchor 진입/해제만 same-tick snap을 허용한다. 완성한 `SimulationCameraPoseSnapshot`만 다음 tick mouse aim이 사용한다. mouse position은 camera target input이 아니며 render-only camera pose를 따로 만들지 않는다. AuthoredAnchor는 boss·choice·cutscene의 저작 ID가 있는 경우에만 허용된다.

## Input order

한 틱의 명령은 다음 순서로 판정한다.

1. VD-07이 현재 `InputMode`로 허용 여부를 판정한다.
2. 유효한 `ChoiceSkillPressed`를 VD-06이 먼저 처리한다.
3. 같은 틱의 `TransferPressed`를 VD-02가 처리한다.
4. 이동·공격·상호작용 명령을 각 소유 시스템이 처리한다.

유효한 선택 기술이 실행되면 같은 틱의 전이 입력을 소비한다. 선택 기술이 실패하면 전이 입력은 계속 처리한다.

Unity 수직 데모의 fixed-phase 실현은 단일 물리 캡처를 사용한다. `TransferSimulationDriver`는 execution order `-200`에서 exact aim/camera pair가 있는 틱에 한 번만 `Physics2D.SyncTransforms`를 수행하고 tick·aim sample ID·sample tick·camera pose tick 영수증을 발행한 뒤 전이를 완료한다. `CombatSimulationDriver`는 `-190`에서 같은 영수증을 검증하고 추가 sync 없이 공격 관측·기본 공격·단일 피해 batch를 처리한다. `RegularEnemyReactionSimulationDriver`는 `-180`에서 같은 tick의 확정 전이·전투 결과를 받아 `walker`와 `surveyor` 반응 세션을 한 번씩 진행한다. `RegularEnemyBehaviorSimulationDriver`는 `-170`에서 exact `t` 전투 결과와 반응 carried view를 소비하고, 같은 현재 물리 상태에서 추가 sync 없이 명시적 두 적의 Q1000 위치와 authored layer-8 LOS를 walker→surveyor 순서로 관측해 M3B2A를 한 번 진행한다. `RegularEnemyLocomotionSimulationDriver`는 `-160`에서 exact behavior/reaction view를 M3C1 입력으로 풀고, 명시적 Q4096 위치의 환경-only BoxCast를 walker→surveyor, X→Y 순서로 처리해 두 kinematic body의 이동을 예약하고 carried motion view를 발행한다. default-order player movement는 그 뒤 같은 global tick을 처리한다. `RegularEnemyThreatSimulationDriver`는 `+100`에서 player motion과 exact Combat/Behavior/Motion tick `t` 발행을 값으로만 결합해 M3D1을 진행하고, 0..2개의 불변 위협 요청 batch를 Combat tick `t+1`용으로 게시한다. 따라서 순서는 정확히 `-200 → -190 → -180 → -170 → -160 → default → +100`이다. `-160`과 `+100`은 추가 sync를 하지 않으며, `+100`은 Rigidbody/Transform 위치나 physics callback/query로 actor contact를 판정하지 않는다. 전투 tick `t`에서 죽은 대상은 이미 완료된 같은 tick의 전이를 소급해 바꾸지 않으며, co-authored transfer target의 제거는 `t+1` 전이 입력에 한 번 병합한다.

오르단 보스 encounter는 같은 root에서 일반 적 pipeline을 확장하지 않고 대체한다. `-200` Transfer 뒤 정확히 하나의 `OrdanBossCombatSimulationDriver`가 `-190` Combat owner가 되고, `OrdanBossSimulationDriver`가 `-185`에서 두 완료 publication을 소비한다. 명시적 owner list에 regular `CombatSimulationDriver`와 boss owner가 공존하거나 boss owner가 중복되면 session 생성 전에 거부한다. `-185`의 source tick `t` output은 빈 batch를 포함해 정확히 하나의 Combat `t+1` batch와 M4A-owned Transfer exposure forecast를 게시한다. M4B2는 forecast를 다음 `-200` 전에 적용하므로 Seizure Telegraph age 0 입력이 가능하다. 이미 완료된 `-200`은 같은 tick `-190/-185` 사망 때문에 rollback하지 않으며 scripted target cleanup은 후속 Transfer tick에 수행한다. 기존 regular 순서와 의미는 변경하지 않는다.

M4B2의 `OrdanBossTransferExposureScheduler`는 `-180`에서 `-185`가 발행한 정확한 `t+1` forecast만 소비한다. 세 `BossWeight-*` 대상은 boss Transfer session에 한 번 등록되고 sink 가용성과 collider만 임시 노출된다. 일반 노출 종료는 다음 `-200` 입력의 내부 `TransferTargetExposureEnded`로 정리한다. 활성 전이는 기존 `TargetRemoved` clear 결과와 player Baseline 복구를 내지만 registration과 removed-ID 집합은 보존하여 `BossWeight-0,1,2,0` 재사용을 허용한다. 반면 정확한 Defeated handoff는 다음 tick에 세 `TransferTargetRemoved`를 병합하여 영구 제거한다. public `TransferCleared(TargetRemoved)`만으로 permanence를 추론하지 않으며, permanence는 두 내부 입력 중 어느 것이 원인이었는지가 결정한다. lifecycle clear가 같은 tick에서 우선하고, 현재/완료 Transfer phase는 rollback하지 않는다.

M4B3A에서 시작한 저작 보스 graph는 일반 적 graph와 별도인 연결된 단일 prefab instance이며, 현재 정확히 하나의 Transfer owner `-200`, boss Combat owner `-190`, boss core bridge `-185`, payload/audit exposure scheduler `-180`, hostile damage producer `-170`, BalanceAudit pull producer `-165`, default-order player movement owner, read-only encounter handoff adapter `+110`을 사용한다. 따라서 보스 graph의 exact fixed 순서는 `-200 → -190 → -185 → -180 → -170 → -165 → default player movement → +110`이고 regular Combat·reaction·behavior·locomotion·threat owner는 같은 root에 존재하지 않는다. Combat registry는 `ordan,player`, Transfer registry는 `BossAuditBox-0,BossWeight-0,BossWeight-1,BossWeight-2` 순서로 첫 session 전에 한 번 동결한다. `ordan` 본체는 Transfer target이 아니다. Audit box는 처음부터 등록되지만 sink unavailable·collider disabled 상태로 시작하며, M4A 이외의 Unity consumer는 phase/snapshot에서 노출을 재구성하거나 registry에서 제거하지 않는다. `+110` adapter는 현재 tick의 완료된 Transfer·Combat·bridge·scheduler·hostile producer·pull producer publication, exact movement receipt와 serialized graph identity를 검증한 뒤 M4A snapshot·intent·handoff, Combat snapshot, Transfer/exposure 상태와 최소 hostile/pull digest의 방어 복사본만 게시한다. adapter는 full hostile geometry나 full pull directive를 노출하거나 Transform·body·collider·target availability·damage/input/room/reward/run/scene 상태를 바꾸지 않으며, cross-wire·stale·partial publication은 이전 view를 보존한 채 fail-stop한다. M4B3A baseline hierarchy와 M4B3B2/M4B3B3 확장은 각각의 승인된 작업 계약이 소유한다.

M4B3B1은 M4A가 staged candidate에서만 계산한 정확한 `t+1` audit forecast를 같은 `-180` scheduler가 소비하도록 확장한다. `BossAuditBox-0`은 `BalanceAudit` Telegraph의 마지막 tick이 예고하는 Execute age 0부터 uninterrupted Execute의 마지막 legal input 전까지 하나의 Transfer target으로만 노출되고, payload forecast와 동시에 non-empty일 수 없다. Production authored graph는 명시적인 exact four-ID lane `BossAuditBox-0,BossWeight-0,BossWeight-1,BossWeight-2`만 사용하며, legacy three-payload fixture mode는 typed test seam에서만 선택되어 audit forecast를 무해하게 무시한다. 두 mode는 runtime 상태로 추론하거나 latch 이후 전환하지 않는다. 정상 audit 노출 종료는 registration을 보존하는 temporary end이고 terminal production cleanup은 다음 Transfer input에 네 removal을 한 번에 병합한다. 같은 tick lifecycle이 있으면 기존 lifecycle-first 권위가 removals보다 우선하므로 phase는 lifecycle clear만 처리하고 영구 removal을 주장하지 않는다. M4B3B1은 hostile geometry/damage, pull movement, teardown, room/reward/run/scene 소비를 소유하지 않는다.

M4B3B2는 같은 boss graph의 `-180` 뒤, player movement 전인 `-170`에 정확히 하나의 `OrdanBossHostileDamageProducer`를 추가했다. M4B3B3 이후의 current boss fixed 순서는 위에서 고정한 `-200 → -190 → -185 → -180 → -170 → -165 → default → +110`이며, `-165` 추가는 M4B3B2의 입력·출력이나 소유권을 바꾸지 않는다. hostile producer는 first tick의 seeded pre-simulation player snapshot과 이후 exact `t-1` 완료 movement snapshot, M4A intent/Seizure projection, 현재 Combat·Transfer·scheduler publication, builder가 동결한 Q4096 descriptor만 읽는다. Physics2D query/sync/callback, Transform/Rigidbody 이동, current-t player pose, phase 재구성, health/DamageResult 변경은 금지된다. source tick `t` contact는 Combat의 기존 단일 pending `t+1` batch에서 payload를 보존한 채 0..1개의 `DamageKind.BossPattern` player request로만 원자 append되고, `-190` M1만 다음 tick에 정렬·dedupe·무적·체력·사망을 판정한다. boss/player dead와 현재 Transfer lifecycle은 새 request를 억제하며 이미 처리된 earlier-phase 결과는 rollback하지 않는다. presentation handoff는 disposition과 선택적 request identity를 담은 최소 digest만 복사하고 full geometry observation은 producer-private이다. 정확한 기하 수치, ID/latch 형식과 allowlist는 Approved M4B3B2 작업 계약이 소유한다.

M4B3B3는 `-170` 뒤, default player movement 전인 `-165`에 정확히 하나의 `OrdanBossBalanceAuditPullProducer`를 추가한다. M4A의 immutable BalanceAudit Execute projection만 active timing authority이며 source tick `t`는 movement tick `t+1`용 directive 하나만 예약한다. 마지막 Execute source tick은 새 directive를 예약하지 않아 실제 소비 구간은 age `1..FrozenDuration-1`이다. pull producer만 Movement.Unity의 owner-bound preflight/commit/cancel lane을 사용하고, player movement motor만 current authoritative position에서 authored Ordan center `(28672,4915)`를 향한 최대 `410` Q4096 외부 변위를 계산해 정상 prediction 뒤 합성한 다음 기존 X→Y collision resolution으로 commit한다. public `MovementCommand`/snapshot ABI, velocity, gravity, jump, dash, Transfer modifier와 input-lock state는 pull이 직접 바꾸지 않는다. 같은 tick audit interrupt, boss/player death 또는 exact current `TransferPhasePublication.Input.LifecycleReason`의 `RoomLeaving|RunFailed|Cutscene|DemoCompleted`는 아직 소비되지 않은 current directive를 취소하고 새 directive를 억제하며 completed movement를 rollback하지 않는다. Movement는 성공한 매 tick에 optional consumed directive와 applied delta receipt를 snapshot과 함께 발행한다. `+110`은 producer source tick `t`, queued-next `t+1` identity와 exact movement receipt tick `t`를 서로 다른 필드로 검증·복사하며 missing/stale/cross-wired 입력에서는 이전 view를 보존한다. 세부 정수 sqrt·radial bound·ID/latch·authoring·AC는 Approved M4B3B3 작업 계약이 소유한다.

### M4B3C terminal seam — Approved 2026-09-08

구현 상태: 2026-09-08 Luna 독립 검증 PASS 및 Astra 로컬 통합 승인으로 해당 [작업 계약](../work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)은 Verified다. [최종 증적](../../verification/2026-09-08-vd03-combat-m4b3c-implementation-evidence.md): EditMode 430/430, PlayMode 367/367. 아래 문단은 최초 경계 승인 시점의 기록이며 전체 게임 완료를 뜻하지 않는다.

이 절은 [M4B3C 작업 계약](../work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)의 경계를 규정한다. Luna 독립 사전검토 PASS 후 Astra가 2026-09-08에 승인했다. 구현 및 독립 실행 검증은 별도이며 이 승인은 Verified를 뜻하지 않는다. 기존 Sol 승인 이력은 ADR-0027에 따라 보존한다.

오르단 death source tick `t`에는 기존 `-200 → -190 → -185 → -180 → -170 → -165 → default → +110` 전체가 먼저 끝나며, `+110` final handoff는 exact `Defeated,RewardRequest,RoomCompletionRequest` triplet과 terminal digest를 보존한다. `-180`이 예약한 exact four-ID removal은 다음 tick `t+1`의 `-200` Transfer에서 처리한다. exact boss terminal lane은 queue consume과 session mutation 전에 removal ID/tick/order, empty exposure-end와 lifecycle precedence를 preflight하고, normal removal effect와 lifecycle-superseded effect를 구분한 internal immutable receipt를 같은 `-200` commit에 게시한다. generic Transfer consume-on-attempt 의미는 바꾸지 않는다.

정확한 cleanup receipt 뒤에만 `OrdanBossTerminalTeardown`이 `-195`에서 실행되어 exact empty `t+1` Combat delivery/input을 닫고 boss Combat·bridge·scheduler·hostile·pull·handoff 및 Ordan target/collider만 quiesce한다. bootstrap과 preterminal `-195`는 no-op이다. `t+1` boss Combat/M1은 진행하지 않고 death tick Combat snapshot은 terminal evidence로 동결한다. encounter root, `Systems`, player, Transfer, Movement, registries와 frozen target references는 deactivate·destroy·reorder하지 않는다. 이 seam은 reward/room/run/choice/save/input/UI/scene 전환을 소비하거나 완료하지 않으며, future room Combat 재개는 별도 Approved owner 계약이 필요하다.

`-190` 전투는 아직 생성되지 않은 같은 tick의 반응을 소급해 읽지 않는다. 대신 직전 성공 반응 tick `t-1`의 불변 carried view를 사용한다. walker의 유효 기본공격 가능 상태는 저작된 기본공격 허용값, M1 처리 전 생존 상태, carried walker 약점 창을 모두 만족할 때만 true다. surveyor와 다른 합법 대상은 반응 fire-lock과 무관하게 저작 허용값과 M1 처리 전 생존 상태만 사용한다. 반응 tick `t`에서 열린 N-tick 창은 전투 `t+1..t+N`에 정확히 N번의 관측 기회를 제공하고, 반응 `t+N`의 만료 발행은 다음 전투 `t+N+1`부터 보인다. 최초 반응 발행 전 walker carried view는 닫힘이다.

각 fixed phase는 자기 상태에 대해서만 원자적이다. `-190`은 M1 변경 전에 `-180` 반응 입력 중 결과 예측이 필요 없는 구조·tick·revision 사전조건을 검사하지만, 이미 성공한 `-200` 전이를 전역 rollback한다고 약속하지 않는다. M1 결과와 최종 생존 상태에 의존하는 정확한 반응 입력은 `-180`에서 두 적 세션 모두 mutation-free 검증한 뒤 함께 진행한다. 결과 의존 검증 실패는 이전의 두 반응 snapshot을 모두 보존하고 이미 확정된 상위 결과를 되돌리지 않은 채 pipeline을 fail-stop한다.

`-170`은 모든 binding, upstream publication presence/tick, bootstrap 또는 `t-1` player snapshot horizon, frozen roster, Q1000와 LOS를 먼저 stage하고 M3B2A의 mutation-free pair preview를 만든다. preview가 walker의 `t+1` recovery intent를 포함하면 M3B1 recovery queue가 그 exact candidate를 받아들일 수 있는지 mutation 없이 먼저 검사한다. 그 뒤에만 M3B2A pair를 commit하고, 이미 preflight된 recovery candidate를 일반 재거부 분기 없이 M3B1에 commit한 다음 carried behavior view를 발행한다. 결과 의존 실패는 M3B2A session·M3B1 recovery queue·이전 carried behavior view를 모두 보존하며, 이미 확정된 `-200/-190/-180` 결과는 되돌리지 않고 pipeline을 fail-stop한다. surveyor shot은 값으로만 carried view에 게시되며 이 단계에서 피해나 denial line을 만들지 않는다. 상위 phase 실패는 같은 tick의 하위 phase 발행을 막는다.

`-160`은 Combat이 동결한 두 BoxCollider2D의 body·root·shape, exact kinematic 설정과 직전 M3C1 snapshot/body Q4096 일치를 먼저 검증한다. 초기 grounded seed는 전체 사전검증 뒤 walker→surveyor 순서의 무조건 support probe로 한 번 동결하고, 런타임 stationary support probe는 직전 grounded·Y 정지·Y 비차단일 때만 수행한다. 모든 query는 `DefaultRaycastLayers`, trigger 제외, 64-hit saturation 거부를 쓰며 body를 임시 이동하지 않는 explicit-origin nonalloc BoxCast다. 이동 cast 거리는 정확히 `(requestedQ4096+82)/4096f`, 법선은 진행 반대 성분을 floor Q4096으로 바꾸어 `2867` 이상일 때만 막고, player·walker·surveyor root를 제외한 환경만 막는다. walker와 surveyor pair resolution 및 M3C1 candidate가 모두 검증된 뒤에만 body 조건을 재검증하고 M3C1을 한 번 commit한 다음 두 `MovePosition`을 예약하고 carried motion view를 발행한다. 일반 거부는 M3C1·body·이전 view를 보존한다. 검증을 소진한 뒤 Unity 내부 `MovePosition` 예외가 발생하면 rollback을 추측하지 않고 view를 발행하지 않은 채 fail-stop한다. dead identity는 query와 이동 예약을 생략한다. reset은 양쪽 behavior가 모두 exact Reset일 때만 선택하며 query 없이 동결된 spawn/ground seed를 복구한다.

`+100`은 `Awake`에서 명시적 binding을 검증하고 첫 `-190 FixedUpdate` 전에 Combat-owned 위협 전달 lane을 한 번 등록한다. 등록된 첫 Combat tick만 bootstrap으로 prior batch가 없고 이후 매 tick은 빈 batch를 포함한 exact 하나를 요구한다. Combat은 기존 입력을 legacy consume-on-attempt 규칙으로 선택한 뒤 위협 batch를 제거하지 않고 검증하며, 나머지 모든 preflight까지 성공한 뒤에만 위협 batch를 한 번 소비해 external damage와 선택적 기본공격 요청에 병합한다. M1만 최종 정렬·dedupe·체력·무적·사망을 판정한다. reset tick도 pending batch의 존재를 요구해 소비하되 그 위협은 M1 reset 전에 버리고, 같은 `+100` pair transaction이 다음 tick용 empty batch를 게시한다. source tick `t` 위협은 결코 같은 tick 전투에 들어가지 않고 정확히 `t+1`에만 전달된다.

실제 기기 입력은 Input System 1.20.0의 단일 `GameInput.inputactions`와 생성 C# wrapper를 통해 VD-07 `InputRouter`에만 들어온다. callback은 의미 명령 버퍼에 기록하고 다음 `SimulationTick`에서 위 순서로 한 번 소비한다. map은 `Gameplay`와 `UI`뿐이며 `InputMode` 전환만 map 활성 상태를 바꾼다.

M5B5로 검증된 보스 그래프의 opt-in 입력 연결은 `-210 InputRouter → -200 Transfer → -195 terminal teardown(해당 시) → -190 Combat → 기존 보스 후속 phase → Movement` 순서를 사용한다. InputRouter가 세 sink의 입력 후보를 모두 적용하고 map 전환까지 성공한 뒤에만 동일한 `InputFrameCommitReceipt`를 마지막으로 게시한다. 각 sink는 자기 pending proof와 이 전역 proof 및 source identity/fault 상태를 검사한 뒤에만 진행하며 기존 비연결 그래프의 계약은 바뀌지 않는다. 종료 정리는 locked cleanup receipt의 Combat proof를 폐쇄하고, 정확한 종료 증적 이후에만 입력 연결이 Combat을 제외한다. 이 경계는 자동 Run 성공·보상 지급·메뉴 구현을 뜻하지 않는다. 상세 권위는 [Verified M5B5](../work-contracts/2026-09-08-vd07-m5b5-device-input-router.md)다.

`UI` map은 `Navigate`, `Point`, `Click`, `ScrollWheel`, `Submit`, `Cancel`을 가진다. Navigate는 WASD·방향키와 XInput D-pad·왼쪽 스틱, Point·Click·ScrollWheel은 mouse, Submit은 Enter·Space와 gamepad A, Cancel은 Esc와 gamepad B다. Gameplay의 `Pause`는 Esc와 gamepad Start다. 두 map은 상호 배타적이며 gamepad virtual mouse는 사용하지 않는다.

runtime binding override는 keyboard Move composite와 Gameplay button, mouse Attack·Transfer button, gamepad Gameplay button에만 허용한다. gamepad Move·Aim stick과 mouse Point axis는 override 대상이 아니다. UI Navigate·Submit·Cancel과 Pause의 Esc·Start 기본 binding은 제거·대체할 수 없는 안전 경로이며 Pause는 추가 binding만 받을 수 있다. stick 축 반전은 binding override 밖의 설정 값이다.

같은 control scheme의 Gameplay map에서 사용 중인 control을 새 action에 지정하면 교환 확인을 요구한다. 승인 transaction은 두 binding을 함께 교환하고 실패 시 둘 다 원래 값으로 rollback한다. 취소는 무변경이다. exact duplicate, 자동 삭제, 무통지 overwrite, protected binding과의 교환, chord·multi-key override는 금지한다. 상호 배타적인 Gameplay/UI map 사이의 동일 control은 허용한다.

## Public contracts

### Transfer target descriptor

`TransferTargetDescriptor`

| Field | Contract |
|---|---|
| `targetId` | 저작된 고유 ordinal 문자열 |
| `kind` | `Box`, `Enemy`, `BossPayload` |
| `aimPoint` | 후보 검색용 대상 중심점; VD-02가 1/1000 unit(Q1000)로 양자화해 판정 |
| `aimShapeCenter` | 직접 포인터 hit 검사용 저작 local-space 중심; 1/1000 unit(Q1000) |
| `aimShapeHalfExtents` | 직접 포인터 hit 검사용 저작 local-space 타원 반지름; 각 축 1/1000 unit(Q1000) |
| `baseModifierProfileId` | 대상 소유 시스템이 제공하는 불변 기준 mass/gravity/AI 프로필 ID |
| `isAvailable` | 제거·사망·비노출 중에는 false |

`BossPayload`는 오르단 패턴이 노출한 scripted handle에만 사용하며 보스 본체는 대상이 아니다.

Q1000은 `AC-WT-006`의 5.999/6.000/6.001u 거리 경계를 보존하기 위한 대상 계약 정밀도다. 카메라 pose의 `positionQ100`은 presentation 계약 그대로 유지하며 대상 거리 판정에는 사용하지 않는다.

### Transfer commands and events

- `SimulationCameraPoseSnapshot(cameraPoseTick, positionQ100, orthoSizeQ1000, rotationQ10, viewportWidth, viewportHeight, gameplayRectX, gameplayRectY, gameplayRectWidth, gameplayRectHeight, integerScale)` — VD-07 발행
- `AimSample(sampleId, source, screenPixel|null, aimVectorQ4096, isPointerInsideGameplayRect|null, cameraPoseTick, sampleTick)` — VD-07 발행
- `TransferPressed(aimSampleId|null, tick)` — VD-07 발행, VD-02 소비
- `TransferStateChanged(targetId, playerModifierId, targetModifierId, tick, revision)` — VD-02 발행
- `TransferCleared(reason, previousTargetId, tick, revision)` — VD-02 발행

`integerScale=floor(min(viewportWidth/640, viewportHeight/360))`이고 minimum은 1이다. `gameplayRectWidth=640×integerScale`, `gameplayRectHeight=360×integerScale`, `gameplayRectX=floor((viewportWidth-gameplayRectWidth)/2)`, `gameplayRectY=floor((viewportHeight-gameplayRectHeight)/2)`이며 좌표 원점은 viewport bottom-left다. 홀수 잔여 pixel은 right/top bar가 하나 더 가진다.

`source`는 `MousePointer`, `GamepadStick`이다. mouse actual pixel을 gameplay rectangle edge에 clamp한 뒤 rect-local 좌표를 `Round(localX×1919/(gameplayRectWidth-1), AwayFromZero)`, `Round(localY×1079/(gameplayRectHeight-1), AwayFromZero)`로 바꾸고 0..1919, 0..1079에 clamp한 값이 1920×1080 normalized aim-grid `screenPixel`이다. `isPointerInsideGameplayRect`는 clamp 전 actual pixel이 rectangle 안이면 true, 여백이면 false이며 gamepad에서는 null이다. false인 mouse sample은 direction만 제공하고 target acquisition과 mouse press command를 만들지 않는다. gameplay world logical canvas는 640×360이며 output baseline 2560×1440에서 nearest-neighbor 4배 확대한다. 24 normalized aim pixels는 이 output에서 32 pixels다. `sampleTick`은 AimSample을 생성해 소비하는 SimulationTick이며 `cameraPoseTick=sampleTick-1`은 직전 완료 tick의 유일한 pose snapshot을 가리킨다. render smoothing camera는 aim 변환에 사용하지 않는다.

`aimVectorQ4096`은 입력 벡터를 정규화한 뒤 각 성분을 `Clamp(Round(component ×4096, AwayFromZero), -4096, 4096)`로 만든 정수 pair다. 양자화 결과가 `(0,0)`이면 invalid다. gamepad 원본 magnitude는 제곱값으로 비교하며 `magnitude² ≥0.04`일 때만 새 sample이다. 유효 조준이 한 번도 없으면 `aimSampleId`는 null이며 전이는 `InvalidTarget`이다.

gamepad `angleKey`는 원시 stick 값이 아니라 `aimVectorQ4096`을 dequantize한 방향과 player-to-aimPoint 방향 사이의 각도를 사용한다.

`reason`은 `ManualRecall`, `TargetRemoved`, `RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted` 중 하나다. 정리는 idempotent하며 이미 제거된 대상에는 원본 복원을 요청하지 않고 modifier·참조·표현 상태만 제거한다.

### Basic attack command

- `BasicAttackPressed(attackSequenceId, aimSampleId, tick)` — VD-07 발행, VD-03 소비

`attackSequenceId`와 `aimSampleId`는 비음수 signed-64 정수다. attack sequence는 encounter 동안 유일한 의미 입력 ID이며 render frame·Unity instance ID에서 만들지 않는다. `aimSampleId`는 같은 tick에 소비되는 정확한 `AimSample.SampleId`와 일치해야 하며 nullable·facing fallback 공격은 없다. 공격은 위 입력 순서의 4단계에서 처리되고 ChoiceSkill·Transfer의 선행 결과를 되돌리지 않는다. 세부 명중·cooldown은 Approved VD-03 작업 계약이 소유한다.

### Damage contracts

- `DamageRequest(requestId, sourceId, targetId, amount, kind, tick)` — 전투 외 시스템도 요청할 수 있으나 VD-03만 판정
- `DamageResult(requestId, targetId, appliedAmount, result, tick)` — VD-03 발행

`result`는 `Applied`, `Invulnerable`, `Duplicate`, `TargetDead`, `Invalid`다. VD-03은 `requestId`를 한 번만 수락하고 같은 틱 요청은 request ID ordinal 오름차순으로 처리한다. 사망 확정 뒤 남은 요청은 `TargetDead`다.

### Room plan contract

`RoomPlanSnapshot(seed, selectionRuleVersion, orderedRoomIds, socketPlanIds, sha256)`는 VD-04가 소유한다.

- 지원 seed는 101, 202, 303, 404다.
- snapshot은 [기준 파일](../../verification/room-plans/)과 SHA-256이 일치해야 한다.
- 신규 프로필의 `lastOfferedSeed`는 null이며 자동 순환은 `101 → 303 → 202 → 404`다.
- VD-09가 다음 `lastOfferedSeed`를 원자 저장한 뒤에만 VD-05가 `RunStarted`를 확정한다.

### Run commands and events

- `RunStartRequested(requestedSeed|null, tick)`
- `RunStarted(seed, roomPlanHash, tick)`
- `RoomEntered(roomId, socketPlanId, tick)`
- `RoomLeaving(roomId, reason, tick)`
- `RunEndRequested(result, cause, tick)`
- `RunEndCommitted(result, cause, tick)`
- `PersistentSnapshotRequested(reason, tick)`
- `ReturnedToHub(previousResult, tick)`

`result`는 `Succeeded`, `Failed`; 실패 `cause`는 `HealthDepleted`, `KillPlane`, `LethalCrush`다. 첫 유효 `RunEndRequested`만 commit되고 이후 요청은 진단 가능한 duplicate다.

### Choice and skill contracts

- `ConsentStateChanged(previous, next, reason, tick)` — VD-06 발행
- `ChoiceCommitted(choice, consentState, grantedSkill, tick)` — VD-06 발행
- `ChoiceSkillPressed(tick)` — VD-07 발행, VD-06 소비
- `ChoiceSkillResult(skill, result, targetId|null, cooldownEndTick|null, tick)` — VD-06 발행

`choice`는 `Extraction`, `Solidarity`; `grantedSkill`은 `CompressionVerdict`, `CommonReferencePlane`이다. 기술 실패 `result`는 `NoActiveTransfer`, `Cooldown`, `InputLocked`, `InvalidTarget` 중 하나다. 성공 결과만 cooldown을 시작한다.

### Input mode contract

- `InputModeChanged(previous, next, reason, tick)` — VD-07 발행
- mode는 `GameplayEnabled`, `UIOnly`, `Cutscene`, `Transition`, `Ended`다.

UI는 모드와 권위 상태를 표현할 뿐 선택, 체력, 전이, 런 결과를 직접 변경하지 않는다.

UI presentation 좌표는 640×360 logical safe frame이며 gameplay rectangle의 integer scale과 offset으로 output에 배치한다. pixel frame·icon은 point scale, text는 logical 12/14/18px role을 final-output SDF로 렌더한다. interactive hit rect는 최소 24×24 logical px이고 frame edge margin은 12 logical px다. UI scale은 profile setting이 아니다.

Gameplay mouse pointer는 총구의 화면 공간 조준 reticle이다. 유효 공격 조준 대상이 선택되면 reticle 확대와 해당 대상의 형광 외곽선이 함께 활성화된다. 이 조준 포착은 전이 가능 여부와 독립적이다. reticle 내부 ring은 청록 연속선=`TransferReady`, 주황 연속선=`Cooldown`, 적색 단절선=`RangeOrLineOfSightBlocked`를 나타내며 활성 전이 대상은 지속 이중 외곽선을 사용한다. `UIOnly`는 gameplay reticle과 target outline을 제거하고 일반 UI cursor를 표시하며 `Cutscene`, `Transition`, `Ended`와 letterbox/pillarbox pointer는 gameplay reticle·ring·target outline을 같은 tick에 숨긴다.

target outline priority는 `ActiveTransfer > AimAcquired > Normal`이다. Normal actor·major NPC·핵심 충돌 실루엣은 8방향 1px W01 `#090D12`, AimAcquired는 1px S02 `#F7FFFC`, ActiveTransfer는 안쪽 1px S01 `#20E0D0`와 바깥 1px S02의 지속 이중선으로 대체한다. outline은 640×360 logical pixel에 고정하고 점멸·subpixel sampling·occluder 투시를 허용하지 않는다.

위 semantic hue family는 11색 accent palette에서만 공급하며 ordinary world decoration에 쓰지 않는다. TransferReady cyan, AimAcquired·ActiveTransfer 공통 white, Cooldown amber, Blocked red, Extraction 2색, Solidarity 2색, heroine identity 3색을 사용한다. Solidarity ivory와 heroine warm ivory는 공유하지 않으며 heroine identity hue를 reward·interactable indicator로 재사용하지 않는다. lighting은 role hue family를 다른 state hue로 바꾸지 않는다.

renderer light ownership은 BackDecor·WorldGeometry·Actors·GameplaySemanticFX·UI·FrontOccluder로 분리한다. world global은 BackDecor·WorldGeometry·FrontOccluder, actor global은 Actors만 소유하고 environment local은 승인된 world layer와 제한된 Actors에만 적용한다. GameplaySemanticFX·UI는 unlit이며 semantic core pixel의 승인 HEX를 보존한다. gameplay preset의 Hub/Expedition/Boss world·actor intensity 하한은 각각 0.85/1.00, 0.75/0.95, 0.65/0.90이고 cutscene override는 control 반환 전에 해당 preset으로 복구한다.

## Lifecycle order

### Room transition

1. `InputModeChanged(GameplayEnabled, Transition)`
2. `RoomLeaving`
3. `TransferCleared(RoomLeaving)`
4. 방 객체 제거
5. 다음 방 객체 준비와 `RoomEntered`
6. `InputModeChanged(Transition, GameplayEnabled)`

### Failed run

1. 첫 유효 `RunEndRequested(Failed, cause)`
2. `InputModeChanged(*, Transition)`
3. `TransferCleared(RunFailed)`
4. `RunEndCommitted(Failed, cause)`
5. 원정·방·전투 일시 상태 제거
6. `PersistentSnapshotRequested(FailureReturn)` — 보존 필드만 기록
7. `ReturnedToHub(Failed)`
8. `InputModeChanged(Transition, GameplayEnabled)`

### Boss to choice to barrier

M5A의 [보스 이후 진행 코어 계약](../work-contracts/2026-09-08-vd05-m5a-post-boss-progression-core.md)은 2026-09-08 Luna 사전 게이트 PASS 뒤 Astra가 승인했다. 순수 Run 코어는 closed death-tick batch에서 Transition 요청을 만들고, exact t+1 정상 정리·teardown 이후에만 미지급 보상 요청 기록과 boss-segment 완료 및 선택/봉쇄선 준비 의도를 한 번 낸다. 플레이어 동시 사망·이미 수락된 실패·비활성 Run과 lifecycle-superseded cleanup은 성공 의도를 억제한다. None/None 저장 pair는 선택 준비, 승인된 두 choice/skill pair는 봉쇄선 준비다. 이 단계는 실제 입력 잠금, 방 이동, 보상 지급, 저장 또는 아래 성공 흐름의 완료가 아니며, Unity owner 연결은 후속 계약이 필요하다.

M5A 순수 코어 구현은 같은 날 Luna 독립 검증과 Astra 통합 승인으로 Verified다. [증적](../../verification/2026-09-08-vd05-m5a-implementation-evidence.md): focused 10/10, full EditMode 440/440, PlayMode 367/367. 기존 아래의 실제 장면·입력·저장 순서는 변경하지 않는다.

1. `R06Completed`에서 전이를 정리하고 보스 segment를 연다.
2. `BossStarted`는 활성 전이 없는 `GameplayEnabled`로 시작한다.
3. `BossDefeated` 뒤 입력과 실패 요청을 잠그고 전이를 정리한다.
4. `ChoicePresented`는 `UIOnly`, 확정 연출은 `Cutscene`이다.
5. `ChoiceCommitted` 뒤 선택·기술 snapshot을 원자 저장한다.
6. `BarrierStarted`에서 `BarrierCounterweight` 전이를 허용하고 `GameplayEnabled`로 전환한다.
7. 봉쇄선 낙하는 실패가 아니라 segment 입구 복귀다.

### Successful demo

1. `InputModeChanged(*, Cutscene)`
2. `TransferCleared(DemoCompleted)`
3. `RunEndRequested(Succeeded, DemoCompleted)`
4. `RunEndCommitted(Succeeded, DemoCompleted)`
5. `PersistentSnapshotRequested(CompletedBranch)`와 원자 저장
6. `InputModeChanged(Cutscene, Ended)`

## Persistence boundary fixed by P0

| State | Failure/restart |
|---|---|
| 위치, 속도, 체력, 활성 전이 | 초기화/제거 |
| 방 계획, 현재 방, 적·위험·보상, 원정 자산 | 초기화/제거 |
| 설정, 입력 바인딩, 튜토리얼 확인 | 보존 |
| 확정 선택, 해금 기술, 완료 분기 기록 | 승인된 commit 뒤 보존 |

활성 런은 저장하거나 이어 하지 않는다. 앱 재시작은 거점의 새 원정 상태로 복구한다. 영구 상태는 VD-09의 schemaVersion 1 단일 `profile.json`에 settings·binding override·tutorial·확정 progression만 canonical JSON으로 기록하고 payload hash를 검증한다. 알 수 없는 field를 추측해 gameplay 상태로 채택하지 않는다.

저장은 `Application.persistentDataPath`의 `profile.tmp.json`을 전체 기록·storage flush·close·재검증한 뒤에만 `profile.json`으로 atomic replace한다. Windows 구현은 `File.Replace(temp, primary, previous, true)`이며 기존 previous가 있으면 교체 전 primary byte로 대체한다. 최초 저장은 검증된 temp를 같은 directory에서 move한다. replace 전 실패는 기존 primary·previous를 변경하지 않는다. replace 호출 오류는 성공을 발행하지 않고 세 파일을 재검증하며 최소 하나의 직전 정상 snapshot을 primary 또는 previous에 보존한다.

load는 valid primary, valid previous, default 순서만 사용하며 stale temp는 revision과 무관하게 로드하지 않는다. primary가 valid여도 invalid previous를 포함한 비정상 파일은 `recovery/`에 보존한다. previous `r` 복구와 binding 부분 복구는 result revision `r+1`, uncommitted default는 source `-1`에서 첫 result `0`이다. binding asset ID·schema·override 적용만 실패하면 input block만 기본값으로 복구하고 다른 profile state는 유지한다. 모든 복구는 atomic save를 요청하며 launch당 한 번 비차단 알림을 낸다.

## Cross-system invariants

- 방 객체 제거 전에 활성 무게 전이를 정리한다.
- 사망·실패 확정 뒤에는 피해, 보상, 선택, 기술 해금을 새로 적용하지 않는다.
- 선택 기술은 이동·전투 공개 계약을 통해 효과를 요청하며 해당 상태를 우회 수정하지 않는다.
- `ResonanceHold`는 VD-06이 요청하지만 속도·중력·AI 정지와 복구는 대상 상태 소유 시스템이 수행한다.
- 저장에는 활성 물리 객체, 활성 전이 참조, 런타임 컴포넌트 참조를 넣지 않는다.
- 계약 순서가 어긋나면 자동 복구를 추측하지 않고 ID·tick·원인을 포함한 계약 오류를 남긴다.

## Approval state

이 문서의 P0·P1 기능 결정과 소비 스펙 정합성 검토는 완료되어 Approved다. M4B2 temporary-exposure, M4B3B1 audit-exposure, M4B3B2 hostile-geometry amendments는 각각의 2026-09-06 Luna 사전 게이트와 Sol 승인을 거쳐 효력을 얻는다. 구현 중 새 정밀도 모순이 발견되면 기존 요구를 추측해 완화하지 않고 승인된 작업 계약에서 보정 근거와 새 경계를 기록한다.
