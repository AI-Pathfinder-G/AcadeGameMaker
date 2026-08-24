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
| Input mode, aim camera snapshot and presentation state | VD-07 Input/UI | all command producers, weight transfer | Authoritative gameplay state |
| Versioned persistent snapshot and atomic file transaction | VD-09 Persistence adapter | VD-04 seed selection, VD-05 restore, VD-06 choice | Live scene objects, active transfer |

소비자는 원본 상태를 직접 수정하지 않고 아래 명령 또는 사건만 사용한다.

VD-07 camera presentation state는 `GameplayFollow`, `AuthoredAnchor` mode, snapped pose와 active room bounds를 포함한다. GameplayFollow는 movement owner의 완료된 player pose를 읽은 뒤 같은 tick 끝에 fixed zoom target을 계산하고 1/18 unit로 snap해 `SimulationCameraPoseSnapshot`을 발행한다. 다음 tick mouse aim만 이 snapshot을 사용한다. mouse position은 camera target input이 아니며 render-only camera pose를 따로 만들지 않는다. AuthoredAnchor는 boss·choice·cutscene의 저작 ID가 있는 경우에만 허용된다.

## Input order

한 틱의 명령은 다음 순서로 판정한다.

1. VD-07이 현재 `InputMode`로 허용 여부를 판정한다.
2. 유효한 `ChoiceSkillPressed`를 VD-06이 먼저 처리한다.
3. 같은 틱의 `TransferPressed`를 VD-02가 처리한다.
4. 이동·공격·상호작용 명령을 각 소유 시스템이 처리한다.

유효한 선택 기술이 실행되면 같은 틱의 전이 입력을 소비한다. 선택 기술이 실패하면 전이 입력은 계속 처리한다.

실제 기기 입력은 Input System 1.20.0의 단일 `GameInput.inputactions`와 생성 C# wrapper를 통해 VD-07 `InputRouter`에만 들어온다. callback은 의미 명령 버퍼에 기록하고 다음 `SimulationTick`에서 위 순서로 한 번 소비한다. map은 `Gameplay`와 `UI`뿐이며 `InputMode` 전환만 map 활성 상태를 바꾼다.

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
| `aimPoint` | 후보 검색용 대상 중심점; VD-02가 1/100 unit로 양자화해 판정 |
| `aimShapeCenter` | 직접 포인터 hit 검사용 저작 local-space 중심; 1/100 unit |
| `aimShapeHalfExtents` | 직접 포인터 hit 검사용 저작 local-space 타원 반지름; 각 축 1/100 unit |
| `baseModifierProfileId` | 대상 소유 시스템이 제공하는 불변 기준 mass/gravity/AI 프로필 ID |
| `isAvailable` | 제거·사망·비노출 중에는 false |

`BossPayload`는 오르단 패턴이 노출한 scripted handle에만 사용하며 보스 본체는 대상이 아니다.

### Transfer commands and events

- `SimulationCameraPoseSnapshot(cameraPoseTick, positionQ100, orthoSizeQ1000, rotationQ10, viewportWidth, viewportHeight, gameplayRectX, gameplayRectY, gameplayRectWidth, gameplayRectHeight, integerScale)` — VD-07 발행
- `AimSample(sampleId, source, screenPixel|null, aimVectorQ4096, isPointerInsideGameplayRect|null, cameraPoseTick, sampleTick)` — VD-07 발행
- `TransferPressed(aimSampleId|null, tick)` — VD-07 발행, VD-02 소비
- `TransferStateChanged(targetId, playerModifierId, targetModifierId, tick)` — VD-02 발행
- `TransferCleared(reason, previousTargetId, tick)` — VD-02 발행

`integerScale=floor(min(viewportWidth/640, viewportHeight/360))`이고 minimum은 1이다. `gameplayRectWidth=640×integerScale`, `gameplayRectHeight=360×integerScale`, `gameplayRectX=floor((viewportWidth-gameplayRectWidth)/2)`, `gameplayRectY=floor((viewportHeight-gameplayRectHeight)/2)`이며 좌표 원점은 viewport bottom-left다. 홀수 잔여 pixel은 right/top bar가 하나 더 가진다.

`source`는 `MousePointer`, `GamepadStick`이다. mouse actual pixel을 gameplay rectangle edge에 clamp한 뒤 rect-local 좌표를 `Round(localX×1919/(gameplayRectWidth-1), AwayFromZero)`, `Round(localY×1079/(gameplayRectHeight-1), AwayFromZero)`로 바꾸고 0..1919, 0..1079에 clamp한 값이 1920×1080 normalized aim-grid `screenPixel`이다. `isPointerInsideGameplayRect`는 clamp 전 actual pixel이 rectangle 안이면 true, 여백이면 false이며 gamepad에서는 null이다. false인 mouse sample은 direction만 제공하고 target acquisition과 mouse press command를 만들지 않는다. gameplay world logical canvas는 640×360이며 output baseline 2560×1440에서 nearest-neighbor 4배 확대한다. 24 normalized aim pixels는 이 output에서 32 pixels다. `sampleTick`은 AimSample을 생성해 소비하는 SimulationTick이며 `cameraPoseTick=sampleTick-1`은 직전 완료 tick의 유일한 pose snapshot을 가리킨다. render smoothing camera는 aim 변환에 사용하지 않는다.

`aimVectorQ4096`은 입력 벡터를 정규화한 뒤 각 성분을 `Clamp(Round(component ×4096, AwayFromZero), -4096, 4096)`로 만든 정수 pair다. 양자화 결과가 `(0,0)`이면 invalid다. gamepad 원본 magnitude는 제곱값으로 비교하며 `magnitude² ≥0.04`일 때만 새 sample이다. 유효 조준이 한 번도 없으면 `aimSampleId`는 null이며 전이는 `InvalidTarget`이다.

gamepad `angleKey`는 원시 stick 값이 아니라 `aimVectorQ4096`을 dequantize한 방향과 player-to-aimPoint 방향 사이의 각도를 사용한다.

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

Gameplay mouse pointer는 총구의 화면 공간 조준 reticle이다. 유효 공격 조준 대상이 선택되면 reticle 확대와 해당 대상의 형광 외곽선이 함께 활성화된다. 이 조준 포착은 전이 가능 여부와 독립적이다. reticle 내부 ring은 청록 연속선=`TransferReady`, 주황 연속선=`Cooldown`, 적색 단절선=`RangeOrLineOfSightBlocked`를 나타내며 활성 전이 대상은 지속 이중 외곽선을 사용한다. `UIOnly`는 gameplay reticle과 target outline을 제거하고 일반 UI cursor를 표시하며 `Cutscene`, `Transition`, `Ended`는 gameplay pointer feedback을 표시하지 않는다.

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

## Remaining approval blockers

`OD-PLAT-001`은 해결됐다. `OD-ART-001`, `OD-SCENE-001`과 모든 소비 스펙의 Luna 일치 검토가 끝나기 전에는 이 문서를 Approved로 전환하지 않는다.
