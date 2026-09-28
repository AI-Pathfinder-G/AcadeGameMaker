---
status: Verified
---

# VD-03 M4B3B3 오르단 BalanceAudit 결정론적 견인 작업 계약

- Owner: Terra
- Contract authority and integration: Sol
- Independent verification: Luna
- Date: 2026-09-07
- Parent specifications: `VD-01`, `VD-03`, `SYSTEM-CONTRACTS`
- Requirements: `REQ-MOV-001`, `REQ-MOV-002`, `REQ-MOV-004`, `REQ-MOV-005`, `REQ-MOV-006`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`, affected `REQ-WT-003`, `REQ-WT-006`

## Goal and boundary

`BalanceAudit` Execute 동안 플레이어를 오르단 쪽으로 지속 견인하는 결정론적 이동 효과를 추가한다. M4A가 보스 패턴과 시점을, Movement가 위치·속도·충돌을 계속 단독 소유한다. 이 단위는 보스나 플레이어의 체력, Transfer 상태, 입력, 방/런 생명주기, 카메라, UI, VFX, 오디오를 소유하지 않는다.

M4B3B3는 M4B3B1의 감사 상자 노출과 M4B3B2의 shockwave 피해를 변경하지 않는다. `BalanceAuditPull`은 피해나 경직이 아니며 새 `DamageRequest`를 만들지 않는다. 기존에 완료된 이동 틱은 어떠한 중단 사건으로도 되돌리지 않는다.

## Sol decisions frozen for review

### Meaning and authored values

- 견인 앵커는 감사 상자가 아니라 M4B3B2와 동일한 오르단 authored center `(28672,4915)` Q4096이다. `BossAuditBox-0`은 패턴 중단용 Transfer 표적일 뿐 이동 앵커가 아니다.
- 견인은 한 번의 속도 충격이 아니라 M4A가 명시한 활성 `BalanceAudit Execute` 구간의 연속적인 외부 변위다.
- 한 견인 이동 틱의 최대 이동 길이는 정확히 `410` Q4096이다. 이는 `410*60/4096 = 6.005859375u/s` 상당이며, 기본 달리기 `8u/s`보다 낮고 대시 `20u/s`보다 낮다.
- 플레이어와 앵커의 현재 authoritative 위치 차이가 `410` 이하이면 정확히 앵커까지만 이동해 overshoot하지 않는다. 두 위치가 같으면 합법적인 영벡터를 소비하고 위치를 바꾸지 않는다.
- 런타임은 Transform, Rigidbody, Collider 또는 Physics query에서 앵커·방향·힘을 다시 읽지 않는다. Builder가 exact integer descriptor를 기록하고 validator가 이를 확인한다.

### M4A projection and active window

- 기존 age-0 `BalanceAuditPull` intent ID `ordan.audit-pull/{attackOrdinal:D19}`는 패턴 시작 의미와 상관관계의 유일한 one-shot authority로 유지한다.
- M4A 출력에 immutable nullable `BalanceAuditPullProjection`을 추가한다. 최종 committed snapshot이 `BalanceAudit/Execute`인 모든 틱에만 존재하며 `SourceTick`, `AttackOrdinal`, `ElapsedTick`, `FrozenDuration`을 가진다. 다른 패턴/phase, Vulnerable, Recovery, Defeated에는 없다.
- projection의 `ElapsedTick`은 `0..FrozenDuration-1`이고 snapshot `PhaseAge`와 같아야 한다. `FrozenDuration`은 M4A의 stage-frozen 표 `90/82/75` 중 하나이며 같은 attack ordinal 동안 바뀌지 않는다.
- 이 `90/82/75`는 기존 M4A `Duration(BalanceAudit,stage,Execute)`의 `90`, `ceil(90*100/110)=82`, `ceil(90*100/120)=75`와 정확히 일치한다. projection은 그 이미 frozen된 phase end에서 duration을 읽을 뿐 stage timing을 바꾸거나 다시 계산하지 않는다.
- 같은 source tick에 감사 상자 Transfer edge가 성공해 M4A가 `BalanceAuditPull` 다음 `BalanceAuditInterrupt` intent를 내더라도 최종 snapshot은 Vulnerable이고 projection은 없다. 따라서 age-0 interrupt는 견인을 시작하지 않는다.
- 마지막 Execute source tick에는 projection이 있지만 새 다음-tick directive를 만들지 않는다. 따라서 정상 패턴의 실제 견인 소비 구간은 Execute age `1..FrozenDuration-1`, 정확히 `FrozenDuration-1` movement ticks다.

### Exact phase and tick horizon

1. `-200` Transfer가 tick `t`를 완료한다.
2. `-190` Combat가 tick `t`를 완료한다.
3. `-185` Ordan M4A bridge가 tick `t`의 immutable output/projection을 발행한다.
4. `-180` exposure scheduler와 `-170` hostile producer는 기존 계약대로 동작한다.
5. 새 `OrdanBossBalanceAuditPullProducer`가 `[DefaultExecutionOrder(-165)]`에서 exact current publications를 검증하고 Movement-owned directive에 대해 preflight/commit 또는 cancel을 수행한다.
6. default-order `PlayerMovementController`만 tick `t`의 directive를 소비하고 운동 예측·cast-first collision resolution·`MovePosition`을 완료한다.
7. `+110` handoff는 immutable 결과만 읽는다.

- source tick `t`의 projection은 `ElapsedTick < FrozenDuration-1`일 때만 delivery movement tick `t+1` directive 하나를 예약할 수 있다. 같은 source tick의 movement `t`에는 영향을 주지 않는다.
- tick `0`은 movement `0`을 변경하지 않고 합법적인 age-0 projection이면 movement `1`만 예약한다.
- directive는 선언된 exact movement tick에만 소비된다. stale, duplicate, skipped, wrong horizon, wrong ordinal/ID, cross-wired owner 또는 두 번째 producer는 전체 작업을 mutation 없이 reject한다.
- directive ID는 invariant-culture ASCII `ordan.audit-pull/{attackOrdinal:D19}/{deliveryTick:D10}`이고 96자 이하다. latch key는 `(attackOrdinal,deliveryTick)`이며 성공한 commit 뒤에만 기록한다.

### Movement ownership and composition

- Movement core의 새 `internal readonly struct ExternalMovementDirective`는 exact `MovementTick`, stable `DirectiveId`, `AnchorXQ4096`, `AnchorYQ4096`, `MaximumStepQ4096`만 운반한다. public `MovementCommand`와 `PlayerMotionSnapshot` ABI에는 필드를 추가하지 않는다. Movement.Unity의 opaque internal candidate가 producer-private owner identity와 preflight fingerprint를 별도로 묶는다.
- `PlayerMovementController`는 at most one pending directive를 future exact tick key로 보관한다. 생산자는 primitive descriptor를 controller의 owner-bound mutation-free preflight에 제출하고 반환된 opaque candidate만 commit한다. Combat.Unity는 Movement core internal value를 직접 만들거나 motor state, command queue 또는 Transform을 바꾸지 않는다.
- default movement tick 시작 시 controller는 exact directive를 먼저 peek하고 motor의 pure Q4096 preflight로 current authoritative position 기준 delta를 계산한다. 이 단계의 잘못된 descriptor·sqrt·곱/합 overflow는 command 또는 directive를 제거하기 전에 reject한다. 그 뒤에만 기존 규칙대로 command를 소비하고 motor prediction을 시작하며, 성공한 motor commit 뒤 directive를 제거하고 receipt를 발행한다. prediction이 시작된 뒤의 비-pull 기존 이동 실패는 VD-01의 기존 consume-on-attempt 의미를 유지하고 M4B3B3가 새로운 rollback 보장을 만들지 않는다. directive가 없으면 기존 경로와 bit-identical해야 한다.
- motor의 exact seam은 mutation-free `PreflightExternalMovementDirective(directive)`가 opaque `ExternalMovementDeltaCandidate`를 만들고, `BeginTick(..., candidate|null)`가 current `NextExpectedTick`, directive fingerprint와 preflight 당시 motor position을 다시 대조한 뒤에만 사용하도록 한다. 정상 이동 분기마다 별도 외부 호출을 만들지 않고 motor 내부의 단일 stage helper가 ordinary predicted position에 preflighted delta를 한 번 합성한다.
- `PlayerMovementMotor`는 기존 command, Transfer modifier, input-lock, 접지, 점프/벽점프, 대시, 수평 가감속, 중력, 벽슬라이드 cap을 먼저 계산하고 정상 predicted position을 만든다. 그 후 directive의 외부 변위를 predicted position에 checked-add한다. velocity, remainders, facing, dash/jump timers, modifier와 action state는 견인 때문에 직접 바뀌지 않는다.
- controller의 기존 X-then-Y cast-first resolution이 정상 이동과 견인을 합친 단일 start-to-predicted path를 해결한다. 견인은 벽, 바닥, 천장을 통과하지 않으며 새 query/sync/callback을 만들지 않는다. 충돌로 막힌 축은 기존 `CommitTick` 규칙대로 velocity/remainder를 처리한다.
- 견인은 active dash에도 합산된다. 따라서 dash timing/velocity는 유지되고, 실제 이동 경로만 오르단 방향으로 최대 `410`만큼 휘며 합쳐진 경로 전체가 충돌 해석을 받는다.
- 견인은 jump, jump release, wall jump, gravity, wall slide 및 Baseline/Lightweight 계산 뒤 위치에 합산되므로 해당 속도 규칙과 Transfer modifier를 덮어쓰지 않는다.
- `InputLocked`는 플레이어 입력만 잠그므로 살아 있는 정상 전투 중의 견인은 계속 적용된다. lifecycle/death suppression은 별도 우선순위를 가진다.

### Exact Q4096 vector math

- motor의 current authoritative position에서 authored anchor를 향한 `dx`, `dy`를 signed `Int64`로 checked-subtract한다.
- `distanceSquared = checked(dx*dx + dy*dy)`를 계산한다. 합이나 곱 overflow는 command와 directive 소비 전 pure preflight에서 reject하고 직전 snapshot, pending directive와 command queue를 보존한다.
- `stepSquared = checked(410L*410L)`를 먼저 고정한다. `distanceSquared <= stepSquared`이면 `(dx,dy)`를 그대로 사용하므로 앵커를 넘지 않는다.
- 그보다 멀면 `distanceCeil = CeilIntegerSqrt(distanceSquared)`를 사용한다. 이는 `floor(IntegerSqrt)`가 정확한 제곱이면 그 값, 아니면 checked `floor+1`이다. 부동소수점, `Math.Sqrt`, Unity vector normalization과 근삿값 테이블은 금지한다.
- 각 성분은 `sign(component) * floor(abs(component)*410/distanceCeil)`이다. 절댓값 곱과 양수 나눗셈을 checked `Int64`로 수행하므로 zero 방향 대칭을 보존하고 반올림 때문에 반경을 넘지 않는다. 결과는 반드시 `resultX²+resultY² <= stepSquared`를 재검사한다.
- `distanceSquared == 0`이면 `(0,0)`이다. postcondition 실패, 산술 overflow, 음수 distance, 잘못된 step/anchor는 saturation, nearest rounding 또는 fallback cardinal을 사용하지 않고 atomic reject한다.
- preflight는 current coordinate와 delta의 checked-add도 검증한다. 이후 기존 normal prediction 결과와 외부 변위를 더한 final predicted coordinate가 `Int32`를 벗어나는 극단 입력은 기존 movement attempt 실패 의미를 따르되 부분 축 snapshot이나 receipt는 발행하지 않는다.

### Interrupt, death and lifecycle precedence

- current tick M4A output에 `BalanceAuditInterrupt`가 있거나 final snapshot이 더 이상 `BalanceAudit/Execute`가 아니면 producer는 exact current movement tick에 아직 pending인 같은 attack directive를 먼저 취소하고 새 directive를 예약하지 않는다.
- final-tick interrupt는 pending current pull과 shockwave 모두 억제한다. age-0 interrupt는 pull intent가 trace에 남아도 directive를 만들지 않는다.
- current Combat가 Ordan dead 또는 player dead를 보고하면 pending current directive를 취소하고 새 directive를 만들지 않는다. Combat가 이미 처리한 피해와 이전 movement snapshot은 rollback하지 않는다.
- current tick의 exact `TransferPhasePublication`이 존재하고 `Input.LifecycleReason`이 `RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted` 중 하나이면 pending current directive를 취소하고 새 directive를 만들지 않는다. `ManualRecall`과 `TargetRemoved`는 lifecycle input이 아니며 이 분기에 들어오지 않는다. missing, null 또는 stale publication을 suppression으로 추론하지 않고 graph validation 실패로 처리한다. M4B3B3는 Movement reset, Transfer clear, room completion 또는 teardown을 수행하지 않는다.
- producer는 첫 Ordan defeat 후 terminal-latch하고 empty terminal outcome을 한 번 발행한다. 이후 advance는 reject하고 마지막 terminal publication을 보존한다. 새 encounter는 M4B3C가 새 graph를 만들 때 새 producer를 사용한다.
- 취소는 producer-private exact owner token, directive ID, movement tick과 attack ordinal이 모두 맞을 때만 성공한다. 이미 motor가 commit한 tick은 취소하거나 다시 계산할 수 없다.
- prior source `t-1`이 예약한 current movement tick `t` directive만 `-165` current cancellation 대상이다. 같은 `-165` advance에서 cancellation commit이 성공한 뒤에만 no-next-queue outcome을 발행하며, 두 번째 cancel, 이미 소비된 directive 또는 그 뒤의 forged commit은 prior queue/outcome을 보존하고 fail-stop한다. Unity fixed phase는 단일 스레드 순서로 이 transaction을 실행하며 비동기 race를 가정하지 않는다.

### Outcome and presentation handoff

- producer의 immutable per-tick outcome은 source tick, terminal, disposition, attack ordinal, elapsed/duration, optional canceled-current directive ID, optional expected-current-consumption directive ID, optional queued-next directive ID/delivery tick을 서로 다른 필드로 담는다.
- disposition은 `NoProjection`, `Queued`, `FinalExecuteNoQueue`, `ZeroDistancePending`, `Interrupted`, `SuppressedBossDead`, `SuppressedPlayerDead`, `SuppressedLifecycle` 중 하나다. `ZeroDistancePending`은 producer가 아니라 motor 소비 결과에서만 확정되므로 producer outcome에서는 queued directive가 정상적으로 남는다.
- Movement는 모든 성공 movement tick마다 exact tick, `HadDirective`, optional consumed directive ID, applied delta X/Y를 담은 immutable internal receipt를 snapshot과 같은 commit 경계에서 발행한다. no-directive tick은 `HadDirective=false`, empty ID, `(0,0)`을 명시한다. 실패한 prediction은 receipt도 교체하지 않는다.
- `+110` encounter handoff에는 full directive나 수학 내부가 아니라 source tick `t`, producer disposition, optional newly queued directive ID/delivery `t+1`, 그리고 Movement tick `t`에서 소비된 prior-source directive receipt를 복사한 최소 digest를 추가한다. handoff는 producer outcome의 expected-current-consumption ID와 exact movement receipt ID를 일치시킨다. cancel/suppression이면 receipt는 no-directive여야 한다. receipt가 missing/stale이거나 ID/empty 규칙이 다르면 applied delta를 추론하지 않고 prior view를 보존한 채 fail-stop한다. queued-next와 consumed-current는 서로 다른 optional identity로 명명하고 혼합하지 않는다.

### Authoring and validation

- `OrdanBossEncounterAuthoringBuilder`만 existing `Systems` object에 producer를 정확히 하나 추가하고 player, Transfer, Combat, M4A bridge, exposure scheduler, hostile producer와 handoff를 명시적으로 연결한다. producer 자체의 private serialized integers가 exact anchor/step descriptor를 소유한다.
- validator는 producer 수 `1`, exact execution order `-165`, same graph identities, `Anchor=(28672,4915)`, `MaximumStep=410`, expanded Systems component set과 handoff binding을 read-only 검증한다. controller의 unique producer registration은 비직렬화 런타임 identity이므로 validator가 prefab load 중 등록을 일으키지 않으며, controller가 두 번째 owner를 fail-closed하고 focused PlayMode가 이를 검증한다.
- 새 GameObject, hierarchy child, layer, tag, collider, Rigidbody, renderer, scene/project setting은 추가하지 않는다.

## Requirements and acceptance evidence

- `AC-M4B3B3-001`: focused EditMode proves projection presence, exact `90/82/75` duration, age/ordinal/source identity, age-0 and final-tick rules, source `t` to movement `t+1`, tick-0 bootstrap, strict stale/skipped/duplicate/cross-wire rejection.
- `AC-M4B3B3-002`: focused EditMode proves anchor/step descriptor, ceil integer sqrt, toward-zero component division, exact-anchor zero, radial postcondition, no overshoot, axis/diagonal symmetry, `(410,1)` and `(600,600)` counterexamples, `Int64` square/add and `Int32` coordinate overflow rejection.
- `AC-M4B3B3-003`: focused EditMode proves one private preflight/commit/cancel/consume lane, defensive immutable values, commit-only latch and state preservation on forged candidate, second owner or partial mutation.
- `AC-M4B3B3-004`: focused motor tests prove exact composition with neutral/move/jump/release/dash/wall-slide/wall-jump/InputLocked, unchanged velocity/timers/modifier, and bit-identical no-directive behavior.
- `AC-M4B3B3-005`: focused PlayMode proves combined X-then-Y collision resolution against ground/wall/ceiling, no tunneling, no direct producer movement and same-tick snapshot/receipt atomicity.
- `AC-M4B3B3-006`: focused PlayMode proves Baseline and Lightweight gravity remain exact while identical pull delta is added after ordinary prediction; Transfer apply/clear ordering does not create a second movement state.
- `AC-M4B3B3-007`: focused PlayMode proves uninterrupted age `1..duration-1` pulls, age-0 interrupt suppression, later interrupt immediate current-pending cancellation, final shockwave/interrupt priority and later attack ordinal reuse.
- `AC-M4B3B3-008`: focused PlayMode proves boss/player death, Transfer lifecycle suppression, terminal latch, already-committed non-rollback and no M4B3B1/B2 ownership changes.
- `AC-M4B3B3-009`: fresh authored 30/60/144 render cadence traces match projection, directive, cancel, Movement snapshot/receipt, Combat/Transfer, M4B3B1/B2 and handoff digests at identical simulation ticks.
- `AC-M4B3B3-010`: builder/validator, static owner/query guards, focused/full EditMode and PlayMode regressions prove protected `AC-MOV-001..006`, M4A, M4B3B1 and M4B3B2 behavior remains green.

## Implementation allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs`
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossBalanceAuditPullProducer.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/MovementContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/PlayerMovementMotor.cs`
- `Assets/AcadeGameMaker/Runtime/Movement/AssemblyInfo.cs` — internal directive/receipt를 `Combat.Unity`와 해당 EditMode 검증 어셈블리에만 노출하는 friend-assembly 경계
- `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs`
- optional new internal-only `Assets/AcadeGameMaker/Runtime/Movement/Unity/ExternalMovementDirectiveCandidate.cs` and `.meta`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs`
- generated `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` and `Assets/Scenes/OrdanBossEncounterSandbox.unity`
- focused M4A, Movement, pull producer, authored graph and handoff tests under existing test assemblies, including an explicit owner/query static guard test
- this contract, its pre-gate/implementation evidence, `docs/README.md`, `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`

## Ollama utilization record

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted interrupt-over-pull priority, strict exact-tick expiry, zero-distance handling, movement-owner collision, cadence and immutability cases. Rejected saturating arithmetic, a guessed `Wall > Dash > Pull` rule, tick-rate-as-render-rate wording and a stale-snapshot direction source. |
| Kimi K3 | used and accepted in part | Accepted immutable directive, preflight/commit/consume, additive displacement and same-tick cancellation concepts. Rejected invented public types/IDs, command rewriting, mutable single-pending state, guessed constants and treating zero-distance as invalid. |
| MiniMax M3 | used and accepted in part | Accepted table-driven fixtures, first-divergent-tick diagnostics, replay identity, symmetry, overflow, collision and cadence cases. Rejected Q12 tick encoding, invented range/contact commands, half-tick metamorphism, attack-order commutativity, dash immunity and apply-then-nullify death semantics. |

All prompts were abstract and non-sensitive. No cloud model received repository text, local paths, credentials, personal data, secrets or approval authority.

## Stop conditions

Stop and return to Sol if implementation needs a public Movement API, direct producer Transform/Rigidbody mutation, dynamic solver force, a second movement state/clock, current-tick rollback, Unity-derived pull direction, new physics sync/query/callback authority, M1 health or damage changes, Transfer lifecycle/removal changes, M4B3B1 four-ID removal changes, M4B3B2 geometry/damage changes, room/run teardown, project settings or final presentation assets.

## Approval gate

Luna independently rechecked the frozen values, radial math, ownership, lifecycle and handoff seams and reported `PASS`, `P0=0`, `P1=0`, `P2=0` on 2026-09-07. Sol approves implementation inside this exact allowlist and stop boundary. Approval does not claim implementation evidence or close any acceptance criterion.

Sol amendment, 2026-09-07: public Movement ABI를 넓히지 않고 internal receipt type을 `Combat.Unity` handoff와 전용 EditMode 검증에서 읽기 위해 `Runtime/Movement/AssemblyInfo.cs`를 위 allowlist에 추가했다. 이 변경은 새 런타임 authority나 public API를 만들지 않는다.

Sol clarification, 2026-09-07: authoring validator는 read-only 원칙상 prefab load 중 비직렬화 producer 등록을 실행하거나 추론하지 않는다. exact-one component/binding은 validator가, second-owner rejection은 controller와 focused PlayMode가 각각 검증한다.
