# Vertical Demo System Contracts

- Status: Review
- Owner and approval: Sol
- Consumers: VD-01 through VD-10
- Last updated: 2026-08-24
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
| Input mode and presentation state | VD-07 Input/UI | all command producers | Authoritative gameplay state |
| Versioned persistent snapshot and atomic file transaction | VD-09 Persistence adapter | VD-04 seed selection, VD-05 restore, VD-06 choice | Live scene objects, active transfer |

소비자는 원본 상태를 직접 수정하지 않고 아래 명령 또는 사건만 사용한다.

## Input order

한 틱의 명령은 다음 순서로 판정한다.

1. VD-07이 현재 `InputMode`로 허용 여부를 판정한다.
2. 유효한 `ChoiceSkillPressed`를 VD-06이 먼저 처리한다.
3. 같은 틱의 `TransferPressed`를 VD-02가 처리한다.
4. 이동·공격·상호작용 명령을 각 소유 시스템이 처리한다.

유효한 선택 기술이 실행되면 같은 틱의 전이 입력을 소비한다. 선택 기술이 실패하면 전이 입력은 계속 처리한다.

## Public contracts

### Transfer target descriptor

`TransferTargetDescriptor`

| Field | Contract |
|---|---|
| `targetId` | 저작된 고유 ordinal 문자열 |
| `kind` | `Box`, `Enemy`, `BossPayload` |
| `aimPoint` | 후보 검색용 대상 중심점; VD-02가 1/100 unit로 양자화해 판정 |
| `baseModifierProfileId` | 대상 소유 시스템이 제공하는 불변 기준 mass/gravity/AI 프로필 ID |
| `isAvailable` | 제거·사망·비노출 중에는 false |

`BossPayload`는 오르단 패턴이 노출한 scripted handle에만 사용하며 보스 본체는 대상이 아니다.

### Transfer commands and events

- `TransferPressed(aimVector, hasAimInput, tick)` — VD-07 발행, VD-02 소비
- `TransferStateChanged(targetId, playerModifierId, targetModifierId, tick)` — VD-02 발행
- `TransferCleared(reason, previousTargetId, tick)` — VD-02 발행

`reason`은 `ManualRecall`, `TargetRemoved`, `RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted` 중 하나다. 정리는 idempotent하며 이미 제거된 대상에는 원본 복원을 요청하지 않고 modifier·참조·표현 상태만 제거한다.

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

활성 런은 저장하거나 이어 하지 않는다. 앱 재시작은 거점의 새 원정 상태로 복구한다. 실제 JSON version, 필드명, 손상 복구는 OD-PLAT-001에서 고정한다.

## Cross-system invariants

- 방 객체 제거 전에 활성 무게 전이를 정리한다.
- 사망·실패 확정 뒤에는 피해, 보상, 선택, 기술 해금을 새로 적용하지 않는다.
- 선택 기술은 이동·전투 공개 계약을 통해 효과를 요청하며 해당 상태를 우회 수정하지 않는다.
- `ResonanceHold`는 VD-06이 요청하지만 속도·중력·AI 정지와 복구는 대상 상태 소유 시스템이 수행한다.
- 저장에는 활성 물리 객체, 활성 전이 참조, 런타임 컴포넌트 참조를 넣지 않는다.
- 계약 순서가 어긋나면 자동 복구를 추측하지 않고 ID·tick·원인을 포함한 계약 오류를 남긴다.

## Remaining approval blockers

- 승인된 기기 범위의 실제 binding과 Input System action map: `OD-PLAT-001`
- 저장 schema version, JSON 필드, 손상 복구 정책: `OD-PLAT-001`

P0 공개 계약과 이동 계약은 해결됐다. 위 P1과 모든 소비 스펙의 일치 검토가 끝나기 전에는 이 문서를 Approved로 전환하지 않는다.
