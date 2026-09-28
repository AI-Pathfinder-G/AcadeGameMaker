---
status: Verified
---

# VD-05 M5D2 활성 런·실패 판정 코어 작업 계약

- Date: 2026-09-11
- Owning Approved specs: [VD-05](../vertical-demo/05-failure-and-persistence.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Related Approved specs: [VD-03](../vertical-demo/03-combat-and-enemies.md), [VD-06](../vertical-demo/06-humanity-choice-and-narrative.md), [VD-09](../vertical-demo/09-platform-and-quality.md)
- Decision basis: ADR-0013, ADR-0018, ADR-0031; [승인된 성공 장면 순서](../../approvals/2026-08-25-p1-demo-success-scene-flow-approval.md)
- Prerequisites: [M5A Verified](./2026-09-08-vd05-m5a-post-boss-progression-core.md), [M4B3C Verified](./2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md), [M5D1 Verified](./2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md)
- Assigned by / final authority: Astra
- Complex-design draft: Sol; implementation: Terra; independent pre/post verification: Luna
- Parent requirements: `REQ-RUN-001` **failure prefix only**, `REQ-RUN-004` failure-request dedupe only, `REQ-RUN-005`; protected `REQ-RUN-002`, `REQ-RUN-003`, `REQ-RUN-006`, `REQ-COM-004`, `REQ-CHOICE-004`, `REQ-PLAT-006`. 이 단위는 parent `AC-RUN-003` 또는 전체 Run lifecycle을 완료하지 않는다.
- Unit requirements: `REQ-M5D2-001` through `REQ-M5D2-006`
- Unit acceptance criteria: `AC-M5D2-001` through `AC-M5D2-008`
- Approval state: **Approved — Astra, 2026-09-12.** Luna second independent pre-gate PASS (`P0=0`, `P1=0`) after resolving the signed-32 tick/signed-64 revision boundary, presence-bearing failure batch, and complete `RunEndRequested(Failed,cause,tick)` envelope. Terra may implement only the exact allowlist below; this approval does not authorize Unity/source adapters or M5A scene binding.
- Implementation state: **Verified — Astra, 2026-09-12.** Terra implemented the exact allowlist, root execution passed focused EditMode `11/11`, full EditMode `494/494`, and full PlayMode `576/576` with failure/skip/inconclusive `0`, and Luna independently verified `AC-M5D2-001..008` with `P0=0`, `P1=0`, `P2=0`. See [implementation evidence](../../verification/2026-09-11-vd05-m5d2-implementation-evidence.md) and [Luna independent review](../../verification/2026-09-12-vd05-m5d2-luna-independent-review.md). This verifies only the bounded Core failure owner; it does not claim a Unity/source adapter, M5A binding, persistence, or the complete VD-05 lifecycle.

## 결정: M5A Unity 연결보다 실패 권위가 먼저다

현재 오르단 graph에는 다음의 실제 불변 증거가 있다.

- 사망 source tick `t`의 Combat player/boss snapshot과 `Defeated → RewardRequest → RoomCompletionRequest` handoff
- M5D1의 실제 `Transition@t+1` InputRouter commit receipt
- M4B3C의 `BossTerminalTransferCleanupReceipt`와 boss-only teardown receipt

그러나 `PostBossProgressionSession.ObserveDeath`가 요구하는 `RunActive`, 이미 수락된 실패, 검증된 persisted choice/skill pair 중 앞의 둘을 소유하는 runtime은 없다. `Assets/AcadeGameMaker/Runtime/Run`에는 M5A의 순수 progression session만 있고 VD-05 Run phase·failure owner는 없다. `KillPlane`·`LethalCrush`의 실제 producer도 아직 없다. 또한 승인된 profile schema는 존재하지만 profile load/validation 또는 VD-06 confirmed choice/granted skill runtime owner는 아직 없다.

따라서 M5D2에서 실제 M5A adapter를 만들면 `RunActive=true`, `AcceptedFailure=null`, `None/None`을 저작값이나 상수로 위조하게 된다. 이 계약은 그 연결을 승인하지 않는다. 가장 좁은 선행 단위로 engine-free active-run/failure arbitration owner를 만든다. persisted pair owner와 실제 source-bound Unity adapter는 각각 후속 Approved 계약이어야 한다.

## 목표와 범위

한 `RunFailureArbitrationSession`은 정확히 한 runtime run instance의 `NotStarted → Active → Failed` 경계와 실패 중복 제거를 소유한다. 매 authoritative `SimulationTick`의 세 승인된 실패 후보를 하나의 닫힌 batch로 판정하고, 실패가 있으면 정확히 하나의 accepted failure와 `RunEndRequested(Failed)` intent를 발행한다. 정상 active snapshot은 이후 M5A adapter가 `RunActive`와 accepted-failure 사실을 값으로 읽을 수 있는 유일한 VD-05 근거가 된다.

이 단위는 Unity를 참조하지 않는 순수 코어다. scene component, callback, physics query, wall clock, RNG, 파일 IO, 네트워크, 정적 전역 registry를 사용하지 않는다.

### 포함

- instance-local run identity와 `NotStarted`, `Active`, `Failed` phase
- 명시적 run start, exact sequential tick 판정, read-only snapshot
- 승인된 실패 원인 `HealthDepleted`, `KillPlane`, `LethalCrush`의 닫힌 동일-tick batch
- 동시 실패의 결정론적 단일 accepted failure, one-shot `RunEndRequested(Failed)` intent
- exact duplicate, conflict, stale/skip, malformed/default input, tick overflow 처리
- 불변 입력·결과·snapshot과 mutation-free validation

### 비범위

- Unity adapter와 오르단 graph/builder/validator/prefab/scene 변경
- 실제 Combat/KillPlane/LethalCrush producer 구현 또는 binding
- M5A `ObserveDeath`/`CompleteCleanup` 호출과 M4B3C/M5D1 receipt 정규화
- `RunEndCommitted`, `Returned`, 거점 로드, 새 run 시작 UI, 성공 `Succeeded`, `DemoCompleted`
- reward 지급/소비, room-plan 진행, choice UI/확정, skill grant/cooldown, barrier, heroine scene
- profile load/save/schema/atomic IO, persisted pair 기본값 추측, active run 저장/재개
- Transfer lifecycle 발행, 입력 모드 변경, player health·motion 변경

## 승인 대상 API와 소유 상태

구현 이름은 아래 의미를 보존한다.

- `RunFailurePhase`: `NotStarted`, `Active`, `Failed`
- `RunFailureCause`: `HealthDepleted`, `KillPlane`, `LethalCrush`
- `[Flags] RunFailureCauseMask`: `None=0`, `HealthDepleted=1`, `KillPlane=2`, `LethalCrush=4`, `AllKnown=7`
- `RunFailureDisposition`: `Applied`, `Duplicate`
- `RunResult`: 이 단위가 생성할 수 있는 값은 `Failed`뿐이며 parent SYSTEM-CONTRACTS의 result 의미를 그대로 보존
- `RunEndRequested`: exact `Result=Failed`, canonical `Cause`, accepted `Tick`; SYSTEM-CONTRACTS의 `(result, cause, tick)` payload와 field-for-field 일치
- `RunStartInput`: non-empty validated `RunInstanceKey`, signed-32 `SimulationTick StartTick` in `0..int.MaxValue-1`
- `RunFailureTickInput`: 같은 `RunInstanceKey`, exact signed-32 `SimulationTick Tick`, required `PresenceMask`, separate `TriggeredMask`
- `AcceptedRunFailure`: canonical cause, exact accepted tick, exact `PresenceMask=AllKnown`, triggered mask와 defensive-copied canonical ordered observed-cause set
- `RunFailureIntent`: exact `RunEndRequested Request`와 local correlation용 `RunInstanceKey`, post-commit `SourceRevision`, exact `PresenceMask`, `TriggeredMask`, defensive-copied ordered observed-cause set
- `RunFailureSnapshot`: phase, `RunActive == (phase == Active)`, key, start tick, next expected signed-32 tick when Active, optional accepted failure, signed-64 `long Revision`
- `RunFailureResult`: disposition, immutable snapshot, immutable newly emitted intent batch
- `RunFailureArbitrationSession.Start(input)` and `ObserveTick(input)` return `RunFailureResult`; read-only `Snapshot` has no tick-advancing or redelivery side effect

한 session은 정확히 한 run instance다. 다른 run을 위해 새 instance를 만들며 Reset/rebind/restart API는 없다. `RunInstanceKey`는 1..96자의 authored ordinal ASCII `[A-Za-z0-9._:/-]`만 허용한다. 이 값은 저장 ID나 보안 capability가 아니며 다른 session의 같은 key를 전역 중복으로 취급하지 않는다.

`SimulationTick.Value`의 canonical domain은 현재 Core와 VD-01이 정의한 signed 32-bit `int`다. M5D2가 이 값을 signed 64-bit로 넓히거나 암묵 변환하지 않는다. 모든 Active `ObserveTick(t)`은 state/result publication 전에 signed-32 `checked(t+1)`을 성공시켜야 하므로 `StartTick`의 합법 범위는 정확히 `0..int.MaxValue-1`이다. `Revision`만 별도의 signed-64 `long`이며 tick이 아니다.

초기 snapshot은 `NotStarted`, `Revision=0`, accepted failure 없음이다. 유효한 `Start`는 key와 `StartTick`을 한 번 동결하고 `Active`, `NextExpectedTick=StartTick`, `Revision=1`을 만든다. Applied `ObserveTick`마다 `Revision=checked(previousRevision+1)`을 결과 생성 전에 stage한다. Revision은 caller input이나 rehydration 값이 아니며 API로 음수 또는 임의 값을 주입할 수 없다. checked revision 증가가 overflow하면 tick input, replay fingerprint, snapshot과 intent를 모두 보존한 채 throw한다. Start는 run asset, seed, room, profile 또는 scene이 실제로 열렸다고 주장하지 않는다. 실제 run-start source binding은 후속 Unity 계약의 책임이다.

## 닫힌 실패 batch와 결정 순서

`ObserveTick`은 `Active`에서 `Tick == NextExpectedTick`인 경우에만 허용한다. `PresenceMask`는 정확히 `AllKnown(0b111)`이어야 한다. `TriggeredMask`는 `None..AllKnown`의 부분집합이어야 하며 unknown bit를 포함할 수 없다. 따라서 명시적 정상 tick은 `PresenceMask=AllKnown, TriggeredMask=None`이고, default `0/0`, partial presence `001/010/100/011/101/110`, unknown presence bit, known presence와 unknown triggered bit는 모두 constructor-bypass 재검증에서도 거부된다. 누락을 false로 보정하지 않는다. 모든 validation, signed-32 tick successor, signed-64 revision successor와 완전한 result/intent 할당을 state publication 전에 완료한다.

1. `TriggeredMask=None`이면 no-failure `Applied` 결과를 내고 intent 없이 `Active`를 유지하며 `NextExpectedTick=t+1`, revision `+1`을 commit한다.
2. `TriggeredMask`가 non-empty이면 exact ordered observed set을 `HealthDepleted → KillPlane → LethalCrush` 순서로 동결한다.
3. accepted diagnostic cause는 그 ordered set의 첫 원인이다. 이는 동일-tick 입력 도착 순서에 좌우되지 않는 결정 tie-break이며 이 단위에서 보상·연출·복귀 방식의 차이를 만들지 않는다.
4. staged post-commit revision `r+1`과 accepted failure에서 exact `RunEndRequested(Failed, canonicalCause, t)`를 만든다. 이를 `RunFailureIntent`가 run key, source revision `r+1`, exact presence/triggered masks와 ordered observed set과 함께 방어 복사해 완전히 동결한다.
5. 위 payload와 field-for-field 일치하는 snapshot을 `Failed`, `RunActive=false`, revision `r+1`로 한 번 commit하고 `RunFailureIntent` 하나만 발행한다. 이후 새 tick은 수락하지 않는다.

Intent consumer는 cause/tick/key/revision/observed set을 snapshot이나 input에서 다시 조립하지 않는다. `Request.Result`, `Request.Cause`, `Request.Tick`은 SYSTEM-CONTRACTS의 exact `RunEndRequested` payload이고, 나머지 intent fields는 이 instance 안의 source snapshot을 대조하기 위한 local correlation metadata다. `RunInstanceKey`와 `SourceRevision`은 persistent profile 또는 cross-run global identity가 아니며 parent public event를 확장하지 않는다. canonical cause는 진단 tie-break일 뿐 원인별 보상·연출·복귀 의미를 만들지 않는다.

`HealthDepleted` triggered bit는 미래 adapter가 Combat owner의 같은 tick 최종 player health/dead 사실에서만 만들 수 있다. `KillPlane`와 `LethalCrush` triggered bit도 각각 미래 Approved hazard owner publication에서만 만들 수 있다. 이 순수 DTO는 owner reference를 인증하지 못한다. 따라서 unit test의 equal copied value는 source provenance 보장이 아니며, 실제 adapter는 명시적으로 bound owner identity, same run graph, exact tick과 publication completeness를 검증해야 한다.

M5A에 전달할 때는 반드시 해당 death source tick `t`의 Run tick을 먼저 닫는다. M5D2 snapshot이 `Active`이면 `RunActive=true`, accepted failure null이고, `Failed`이면 `RunActive=false`와 exact accepted failure를 함께 전달한다. caller가 둘을 독립적으로 조립하거나 player-dead Combat 사실을 재분류할 수 없다. 이 전달은 본 계약의 구현 범위가 아니다.

## 실패·중복·충돌·overflow 동작

- 동일 `RunStartInput` replay는 `Duplicate`이고 revision/intent를 바꾸지 않는다. key 또는 tick이 다른 두 번째 Start는 conflict이며 mutation-free throw다.
- 직전 accepted no-failure tick batch의 key/tick/presence/triggered mask가 byte-semantic identical한 replay는 `Duplicate`이고 `NextExpectedTick`, revision, intent를 바꾸지 않는다. 같은 tick의 어느 mask 변경도 conflict다.
- terminal failure batch의 key/tick/presence/triggered mask가 identical한 replay는 어느 후속 시점에도 `Duplicate`이며 새 intent를 내지 않는다. 같은 terminal tick의 mask 변경과 실패 뒤 새 tick은 mutation-free error다.
- `Tick < NextExpectedTick-1`은 stale, `Tick > NextExpectedTick`은 skip/future error다. session은 빠진 tick을 합성하거나 current tick으로 재라벨하지 않는다.
- invalid key, unknown enum/mask bit, default outer input, partial/missing presence, negative tick, inconsistent constructor-bypass value, signed-32 tick overflow와 signed-64 internal revision overflow는 state와 retained replay fingerprint를 변경하지 않는다. revision은 caller input이 아니므로 `negative revision input` 경로는 존재하지 않는다. 실패한 시도 뒤 exact valid input은 여전히 수락 가능해야 한다.
- signed-32 `SimulationTick` successor 계산은 checked다. `t+1`을 표현할 수 없는 active observation은 triggered mask와 관계없이 commit 전에 거부하며 Run failure나 success로 바꾸지 않는다. signed-64 revision successor도 별도로 checked하며 둘을 같은 수치 domain으로 취급하지 않는다.
- public result/snapshot/observed-cause collections are defensive copies. 반환값을 변조해 session을 바꿀 수 없다.
- callbacks, consumer acknowledgement, automatic retry, exception swallowing, rollback of already completed external systems는 없다.

## 요구사항

- `REQ-M5D2-001` — 한 session은 signed-32 tick을 쓰는 한 run key의 `NotStarted → Active → Failed` prefix와 `RunActive`를 소유하고 reset/rebind/static dedupe 없이 signed-64 revision의 immutable snapshot을 제공한다.
- `REQ-M5D2-002` — 각 active tick의 세 승인 실패 원인을 `PresenceMask=AllKnown`과 별도 `TriggeredMask`의 exact sequential closed batch로 판정하며 default·partial·unknown presence나 stale·future·skipped tick을 합성하지 않는다.
- `REQ-M5D2-003` — non-empty 동일-tick triggered mask는 고정 원인 순서로 exact observed set과 accepted failure 하나를 만들고, exact `RunEndRequested(Failed,cause,tick)`와 run key/source revision/masks/ordered set을 동결한 intent 하나만 발행한다.
- `REQ-M5D2-004` — key/tick/presence/triggered mask exact replay는 진단용 Duplicate이며 changed replay, second key, post-failure tick과 malformed/default/partial/unknown/overflow 입력은 mutation-free로 거부한다. tick `int`와 internal revision `long` 증가는 각각 checked다.
- `REQ-M5D2-005` — M5D2는 `REQ-RUN-001`의 failure prefix와 failure-request dedupe만 소유하며 external source provenance, `RunEndCommitted`/failure cleanup/return, success, profile/choice, M5A/Unity 연결을 주장하거나 수행하지 않는다.
- `REQ-M5D2-006` — 구현은 기존 Core-only Run assembly 경계를 유지하고 UnityEngine, IO, RNG, network, wall clock, callback 또는 기존 public ABI/asset/project setting을 변경하지 않는다.

## 수용 기준

- `AC-M5D2-001` — fresh session의 `StartTick=0`, `int.MaxValue-1` exact Start가 각각 revision 1의 Active snapshot과 exact signed-32 next tick을 한 번 만들며 identical Start는 Duplicate, negative/`int.MaxValue` Start와 changed/second Start는 mutation-free error다. 두 독립 session은 같은 key/tick을 공유해도 간섭하지 않는다.
- `AC-M5D2-002` — `PresenceMask=AllKnown, TriggeredMask=None`인 연속 tick들이 intent 없이 Active를 유지하고 signed-32 next tick과 signed-64 revision을 각각 정확히 1 증가시킨다. 직전 key/tick/masks identical replay는 Duplicate이며 stale/skip/mask-changed replay는 state와 retained fingerprint를 바꾸지 않는다.
- `AC-M5D2-003` — AllKnown presence 아래 세 단일 triggered bit 각각은 exact cause/tick/mask/ordered set을 가진 Failed snapshot을 만들고 RunActive를 false로 한다. 유일한 intent의 `Request`는 exact `(Failed,cause,tick)`이며 run key, post-commit source revision, masks와 ordered set이 snapshot과 field-for-field 일치한다.
- `AC-M5D2-004` — AllKnown presence 아래 모든 2-bit와 3-bit triggered 조합은 도착/열거 순서와 무관하게 고정 ordered set과 첫 canonical accepted cause를 만든다. exact RunEndRequested payload와 correlation metadata를 가진 intent는 하나뿐이고 원인별 다른 gameplay 의미는 없다.
- `AC-M5D2-005` — terminal key/tick/masks identical replay는 Duplicate/empty new intents이고 changed presence/triggered mask 또는 후속 tick은 error다. first accepted failure, snapshot revision과 완전한 intent payload/fingerprint는 보존된다.
- `AC-M5D2-006` — default `PresenceMask=None`, 여섯 partial known-presence mask, unknown presence/triggered bit, invalid key, negative/stale/future/skipped tick과 signed-32 successor overflow는 partial snapshot/intent를 만들지 않으며 retained Start/last-active/terminal fingerprint와 이어지는 valid input을 보존한다. revision은 caller input이 아님을 API/static test로 확인하고 internal `long` 증가는 checked이며 overflow 전에 publication이 없음을 검토한다.
- `AC-M5D2-007` — 정적 검사가 signed-32 `SimulationTick`/signed-64 internal revision 분리, 두 checked successor, complete immutable RunEndRequested envelope, Run runtime assembly의 Core-only/no-engine 경계와 UnityEngine/IO/RNG/network/wall-clock/callback/scene/input/profile 접근 부재, 기존 public ABI·asset·project setting 무변경을 증명한다.
- `AC-M5D2-008` — Luna가 `AC-M5D2-001..007` mapping, focused EditMode와 전체 EditMode/PlayMode zero failure/skip, AllKnown presence를 포함한 동일 tick script의 30/60/144 grouping exact snapshot/intent trace, XML SHA-256와 first divergence 또는 none을 독립 검증한다. grouping 검사는 render 성능 주장이 아니다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Run/RunFailureArbitrationSessionTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-11-vd05-m5d2-contract-pregate.md`
- later `docs/verification/2026-09-11-vd05-m5d2-implementation-evidence.md`
- `docs/README.md` minimum index/status link

기존 `AcadeGameMaker.Run.asmdef`, `AssemblyInfo.cs`와 Run EditMode test asmdef는 이미 필요한 `Core` reference와 test friend를 가지므로 변경 금지다. 그 밖의 runtime, tests, asmdef, prefab, scene, builder, validator, assets/media, Packages, ProjectSettings와 기존 verification 기록도 금지다.

## 검증 계획과 증거

1. Luna second pre-gate가 이 수정된 Review 문서의 parent REQ prefix mapping, signed-32 tick/signed-64 revision 분리, presence-bearing masks, complete RunEndRequested envelope, 동시 실패 tie-break, replay/overflow와 future adapter provenance 경계를 반례 중심으로 검토한다.
2. Astra가 필요한 수정을 통합하고 이 문서만 `Approved`로 전환한 뒤 Terra가 allowlist만 구현한다.
3. Terra evidence는 `REQ-M5D2-*`별 코드·테스트 mapping, `0`, `int.MaxValue-1`, `int.MaxValue` tick 경계, default/partial/unknown mask matrix, complete intent-to-snapshot field comparison, Start/last-active/terminal fingerprint retention, focused EditMode XML, full EditMode/PlayMode XML, Unity version, exact command, changed-file list, source/XML SHA-256와 미실행 항목을 기록한다.
4. Luna는 구현자와 독립적으로 `AC-M5D2-*`별 source/test review, malformed/default/partial/unknown mask와 tick/revision overflow counterexamples, complete RunEndRequested payload, simultaneous-cause matrix, deterministic grouping trace와 전체 회귀를 검증한다.
5. Astra만 최종 통합과 `Verified` 상태를 결정한다. pure core PASS를 실제 run start/failure source 또는 playable boss-to-choice 연결로 보고하지 않는다.

## 후속 통합 순서와 중단 조건

권장 순서는 `M5D1 Verified → M5D2 Run failure owner → persisted profile snapshot owner → actual source-bound M5A adapter → choice/barrier/scene owners`다. 후속 adapter는 M4B3C/M5D1 actual receipts, M5D2 same-tick snapshot과 VD-09/VD-06 validated persisted pair를 모두 갖기 전에는 승인할 수 없다. M5A의 `TransitionRequested` intent는 이미 M5D1이 commit한 exact transition receipt와 대조하는 값일 뿐, 두 번째 mode request가 되어서는 안 된다.

다음 중 하나가 필요하면 즉시 멈추고 Astra에 반환한다.

- 승인된 세 원인 밖의 새 failure cause 또는 원인별 보상·연출·복귀 차이
- 동시 실패의 canonical diagnostic priority를 플레이어 표시나 영구 기록 의미로 사용하려는 요구
- actual KillPlane/LethalCrush source, scene/run bootstrap, return transaction 또는 success owner 구현
- M5A, Combat, Transfer, InputRouter, profile schema/IO, choice/skill runtime 또는 public ABI 변경
- allowlist 밖 파일, Unity component, asset/scene/project setting 또는 외부 호출 필요

## 롤백과 참여 기록

Rollback point는 M5D1 Verified 상태다. 되돌리기는 승인 시 새 M5D2 runtime/test 파일과 이 계약·후속 증적·docs index hunk만 대상으로 하며 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다. commit, remote publication, asset deletion은 암시되지 않는다.

이 계약은 Sol의 bounded complex-design 초안과 Luna의 두 차례 독립 pre-gate를 거쳐 Astra가 Approved로 전환했다. Terra 구현과 Luna 구현 후 독립 검증은 아직 수행되지 않았다. 외부 모델, Ollama, 자동화, network call과 Pro 실행은 사용하거나 주장하지 않는다.
