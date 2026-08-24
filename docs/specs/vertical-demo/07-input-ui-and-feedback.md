# [VD-07] Input, UI, and Feedback

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

의미 입력, 입력 모드 gate, 키보드·XInput 지원 범위와 전이·런·선택의 최소 판독 피드백을 포함한다.

## Non-scope

최종 UI 아트, 접근성 전체 기능과 게임플레이 권위 상태의 직접 변경은 포함하지 않는다.

## Contract

이동·전투·무게 전이·상호작용의 의미 기반 입력과 플레이어가 현재 상태·실패 이유·선택 결과를 이해하는 최소 피드백을 정의한다. 키보드와 XInput 게임패드를 모두 지원하고 마우스는 UI·조준에 사용할 수 있으며, 구체 action map과 실제 바인딩은 플랫폼 계약에 둔다.

입력 계층은 `com.unity.inputsystem` 1.20.0만 사용한다. 단일 `GameInput.inputactions`에서 생성한 C# wrapper를 `InputRouter`가 직접 구독하며 `PlayerInput.SendMessage`, 구형 Input Manager와 `Both` 모드는 사용하지 않는다. action map은 `Gameplay`와 `UI` 두 개뿐이다.

Gameplay는 `포인터 조준형 횡스크롤 액션`을 따른다. 키보드·마우스는 WASD 이동, Space 점프, Shift 대시, E 상호작용, Q 선택 기술, 좌클릭 공격, 우클릭 전이를 사용하고 JKL 전투·클릭 이동을 사용하지 않는다. 게임패드는 왼쪽/오른쪽 스틱 이동·조준, RT 공격, LT 전이, RB 선택 기술, A 점프, B 대시, X 상호작용을 사용한다.

UI는 `Navigate`, `Point`, `Click`, `ScrollWheel`, `Submit`, `Cancel`을 사용한다. `Navigate`는 WASD·방향키와 gamepad D-pad·왼쪽 스틱, `Point`·`Click`·`ScrollWheel`은 mouse, `Submit`은 Enter·Space와 gamepad A, `Cancel`은 Esc와 gamepad B에 연결한다. `Pause`는 Gameplay의 Esc와 gamepad Start이며 `UIOnly`로 전환한다. UIOnly에서는 focus navigation을 사용하고 gamepad virtual mouse cursor는 만들지 않는다. Gameplay와 UI map은 동시에 활성화하지 않는다.

UI layout coordinate는 640×360 logical safe frame이며 pixel frame·icon과 hit rect는 gameplay integer scale·offset을 따른다. final-output SDF text는 같은 logical layout에 결합되고 minimum/standard/heading 12/14/18px, interactive area 24×24px, edge margin 12px minimum을 사용한다. 수직 데모에는 UI scale setting이 없다.

runtime rebind는 keyboard의 Move composite 네 방향과 모든 Gameplay button action, mouse의 Attack·Transfer button, gamepad의 Gameplay button action에 허용한다. gamepad Move·Aim stick control과 mouse Point axis는 고정한다. UI Navigate·Submit·Cancel과 Pause의 Esc·Start 기본 binding은 안전 복구 경로이므로 제거하거나 덮어쓸 수 없다. Pause에는 추가 binding만 허용한다. stick 축 반전은 binding override가 아닌 별도 설정이다.

같은 control scheme의 Gameplay map 안에서 exact duplicate binding은 허용하지 않는다. 이미 사용 중인 control을 지정하면 현재 action과 교환할지 확인하고, 승인 시 두 binding을 원자적으로 맞바꾸며 취소 시 어느 쪽도 바꾸지 않는다. 자동 삭제·무통지 덮어쓰기는 금지한다. Gameplay와 UI map은 상호 배타적이므로 map 사이의 동일 control은 충돌이 아니다. protected UI·Pause 기본 binding과의 교환은 거부한다. chord·multi-key binding은 수직 데모에서 지원하지 않는다.

Gameplay의 mouse pointer는 별도 OS cursor 위에 겹치는 장식이 아니라 총구의 화면 공간 조준점을 나타내는 gameplay reticle이다. 유효 대상이 선택되면 reticle이 즉시 커지고 선택 대상에 1px `#F7FFFC` 형광 외곽선을 표시해 `조준 포착`을 알린다. 조준 포착은 공격 조준 상태이며 무게 전이 가능 여부와 분리한다. reticle 내부 ring은 청록 연속선=`전이 가능`, 주황 연속선=`Cooldown`, 적색 단절선=`거리 또는 LOS 차단`으로 나타낸다. 활성 전이 대상은 안쪽 `#20E0D0`·바깥 `#F7FFFC`의 지속 2px 이중 외곽선을 사용한다. UIOnly에서는 gameplay reticle과 대상 외곽선을 숨기고 일반 UI cursor를 표시한다. reticle의 정확한 확대율·발광 강도·점멸/보간 값은 `OD-ART-001`에서 고정한다.

### Inputs

`InputRouter`가 버퍼링한 `AimSample`, `Transfer`, `ChoiceSkill`, 이동·공격·상호작용, 전이·런·선택·입력 모드 사건, 실패 이유와 피드백 제한 틱.

### Outputs

후보 외곽선·연결선, 활성·회수·실패·쿨다운·선택 결과 피드백과 `InputModeChanged` 사건을 낸다.

### Owned state

`InputRouter`, 권위 입력 모드 gate, `Gameplay`/`UI` map 활성 상태, 피드백 표시와 중복 억제를 소유한다. 전이·체력·선택·런의 게임플레이 권위 상태, 사용자 binding override와 저장 필드는 소유하지 않는다.

### Invariants

- `GameplayEnabled`, `UIOnly`, `Cutscene`, `Transition`, `Ended`의 모드에 따라 입력 표현과 허용 상태를 일관되게 갱신한다.
- 같은 틱의 유효 `ChoiceSkill`과 `Transfer`는 ChoiceSkill을 먼저 처리한다. 수탈 성공은 Transfer를 소비하고, 연대 성공은 Transfer를 무시하며, 실패한 ChoiceSkill은 Transfer를 막지 않는다.
- 실제 기기 callback은 gameplay state를 직접 변경하지 않고 의미 명령을 버퍼에 기록하며, 각 명령은 다음 `SimulationTick`에서 최대 한 번 소비된다.
- map 전환은 `InputMode` 소유 경로만 수행한다. 장면·기능 코드는 개별 action 또는 map을 임의로 enable/disable하지 않는다.
- 기기 연결·분리와 마지막 사용 control scheme 변화는 표시용 상태만 바꾸며 시뮬레이션 결과를 바꾸지 않는다.
- 기본 공격과 전이는 같은 포인터·스틱 조준 의도를 사용한다. 선택 기술은 현재 활성 전이 대상을 사용하므로 별도 조준을 요구하지 않는다.
- 마우스는 마지막 정수 screen pixel을 보관하고 다음 SimulationTick 시작에 직전 완료 tick의 `SimulationCameraPoseSnapshot`으로 변환한다. gamepad stick과 변환된 mouse 방향은 정규화 뒤 각 성분을 `Round(component×4096, AwayFromZero)`하고 -4096~4096으로 clamp한다.
- Gameplay에서 mouse pointer 위치는 총구 조준 reticle로 표시하고 유효 포착 시 reticle 확대와 대상 형광 외곽선을 함께 표시한다. UIOnly는 gameplay reticle·대상 외곽선을 숨기고 일반 UI cursor를 사용한다. Cutscene·Transition·Ended 전환은 모든 gameplay pointer 피드백을 숨긴다.
- 조준 포착은 공격 조준을 뜻하며 전이 가능 여부와 독립적으로 유지한다. 전이 상태는 reticle 내부 ring의 색과 선 형태로, 활성 전이는 대상의 지속 이중 외곽선으로 별도 표현한다.
- Gameplay와 UI map은 동시에 활성화하지 않으며 gamepad 오른쪽 스틱은 OS cursor를 움직이지 않는다. UIOnly는 focus navigation만 사용하고 virtual mouse를 만들지 않는다.
- UIOnly 최상위 일시정지 화면의 `Cancel`은 GameplayEnabled로 복귀하고, 하위 화면의 `Cancel`은 상위 화면으로 한 단계 돌아간다.
- mouse actual pixel은 중앙 gameplay rectangle 기준으로 조준 좌표를 만든다. pointer가 letterbox/pillarbox에 있으면 조준 방향만 가장 가까운 gameplay edge로 clamp하고 포착·Attack·Transfer mouse press는 생성하지 않는다. UI element는 safe frame 밖에 없으며 여백 Click은 아무 action도 실행하지 않는다.

## Requirements

- **REQ-UX-001:** 모든 필수 행동은 하나의 명명된 입력 액션에 연결되고 중복 충돌이 없어야 한다.
- **REQ-UX-002:** 전이 후보, 성공, 실패, 활성 상태, 회수를 서로 구분할 수 있어야 한다.
- **REQ-UX-003:** 체력/실패 위험, 원정 결과, 선택 결과, 새 기술을 플레이 진행을 멈추지 않고 확인할 수 있어야 한다.
- **REQ-UX-004:** 컷신·메뉴·장면 전환의 입력 잠금과 복구가 일관되어야 한다.
- **REQ-UX-005:** 전이 후보·활성·회수·차단·쿨다운과 `NoActiveTransfer`, `Cooldown`, `InputLocked`, `InvalidTarget` 실패 이유를 서로 구분해 표시해야 한다.
- **REQ-UX-006:** Input System 1.20.0의 생성 C# wrapper와 단일 `InputRouter`를 사용해 `Gameplay`/`UI` 두 map의 callback을 다음 60Hz tick 의미 명령으로 한 번만 변환해야 한다.
- **REQ-UX-007:** Gameplay는 승인된 키보드·마우스와 XInput 역할 배치를 사용하고 JKL 전투·클릭 이동 없이 포인터·오른쪽 스틱 조준을 공격과 전이에 공통 적용해야 한다.
- **REQ-UX-008:** `InputRouter`는 정수 mouse pixel 또는 `magnitude²≥0.04`의 오른쪽 스틱과 직전 `SimulationCameraPoseSnapshot`을 Q4096 `AimSample`로 만들어 다음 SimulationTick에 한 번 제공하고, 모드·생명주기 경계에서 aim·press buffer를 결정적으로 정리해야 한다.
- **REQ-UX-009:** UI는 승인된 Navigate·Point·Click·ScrollWheel·Submit·Cancel·Pause binding, focus navigation과 map 상호 배제를 사용하고 gamepad virtual mouse를 만들지 않아야 한다.
- **REQ-UX-010:** Gameplay의 mouse pointer는 총구 조준 reticle로 표시하고 유효 대상 포착 시 reticle 확대와 대상 형광 외곽선을 함께 표시하며, 조준 포착을 전이 가능·Cooldown·거리/LOS 차단·활성 전이 상태와 서로 구분하고 UIOnly와 잠긴 모드에서는 gameplay 조준 피드백을 제거해야 한다.
- **REQ-UX-011:** runtime rebind는 keyboard Move와 Gameplay button, mouse Attack·Transfer, gamepad Gameplay button에만 허용하고 gamepad Move·Aim stick, mouse Point axis와 안전 UI·Pause 기본 binding을 보호하며 stick 축 반전은 별도 설정으로 처리해야 한다.
- **REQ-UX-012:** 같은 control scheme의 Gameplay binding 충돌은 확인 뒤 원자 교환하거나 취소 시 무변경으로 처리하고 중복·자동 삭제·무통지 덮어쓰기와 protected binding 교환을 금지하며 map 사이 공유는 허용해야 한다.
- **REQ-UX-013:** mouse aim은 중앙 16:9 gameplay rectangle을 기준으로 변환하고 여백에서는 edge-clamped aim direction만 유지하며 target acquisition·Attack·Transfer·UI action을 발생시키지 않아야 한다.
- **REQ-UX-014:** HUD·menu는 640×360 logical safe frame, 12/14/18px SDF text와 최소 24×24px hit area·12px edge margin을 사용하고 gameplay rectangle의 integer scale 밖에 interactive UI를 두지 않아야 한다.

## Acceptance criteria

### AC-UX-001 — 전이 상태 판독

- **Given** 전이 불가·가능·활성 상태가 각각 있고
- **When** 같은 대상에 접근하고 조작하면
- **Then** 검수자는 화면 피드백만으로 세 상태와 실패 이유를 구분한다.

### AC-UX-002 — 입력 복구

- **Given** 컷신, 메뉴, 방 전환 중 하나가 입력을 잠그고
- **When** 해당 상태가 정상 또는 취소로 끝나면
- **Then** 이전 입력이 고착되지 않고 플레이 입력이 한 번만 복구된다.

### AC-UX-003 — 필수 상태와 결과 판독

- **Given** 정상 플레이, 실패 직전, 원정 종료, 선택 완료, 기술 획득 상태가 각각 있고
- **When** 검수자가 화면과 오디오 피드백을 확인하면
- **Then** 체력/실패 위험, 원정 결과, 선택 결과, 새 기술을 서로 혼동하지 않고 식별한다.

### AC-UX-004 — ChoiceSkill과 전이 입력 순서

- **Given** 활성 전이와 양쪽 ChoiceSkill, 그리고 UIOnly·Cutscene·Ended 상태가 있고
- **When** 같은 틱의 ChoiceSkill·Transfer 또는 잠긴 상태의 ChoiceSkill을 입력하면
- **Then** 성공 수탈은 전이를 한 번 회수하고 성공 연대는 유지하며, 실패한 ChoiceSkill은 Transfer를 허용하고 잠긴 상태는 `InputLocked` 피드백만 보인다.

### AC-UX-005 — 입력 구조와 단일 소비

- **Given** Input System package·Player Settings·`GameInput.inputactions`·생성 wrapper와 입력 기록이 있고
- **When** 정적 계약 검사와 같은 callback 기록의 PlayMode 재생을 수행하면
- **Then** package는 1.20.0, Active Input Handling은 Input System only, map은 `Gameplay`/`UI`뿐이고 `PlayerInput.SendMessage`·구형 Input Manager 참조가 없으며 각 의미 명령은 다음 SimulationTick에서 정확히 한 번 소비된다.

### AC-UX-006 — 포인터 조준 역할 배치

- **Given** 키보드·마우스와 XInput 기본 binding 및 같은 전투 검수 장면이 있고
- **When** 이동 중 공격·전이·선택 기술을 각 control scheme으로 실행하면
- **Then** 키보드·마우스는 왼손 이동·플랫폼 조작과 오른손 조준·전투로 분리되고 게임패드는 두 스틱·trigger/bumper로 같은 의미 결과를 내며 JKL 전투와 클릭 이동 binding이 없다.

### AC-UX-007 — 조준 샘플과 카메라 독립성

- **Given** 같은 mouse pixel·stick script와 같은 simulation camera 기록이 있고
- **When** 30/60/144 render FPS에서 각각 재생하면
- **Then** 매 tick의 cameraPoseTick, `AimSample` source·screenPixel·Q4096 vector와 소비 tick이 exact match하며 `(0,0)` sample이 없고 mode 전환 이전 press가 Gameplay 복귀 뒤 다시 발동하지 않는다.

### AC-UX-008 — UI 기본 조작과 map 상호 배제

- **Given** keyboard/mouse와 XInput 기본 binding, 일시정지 최상위·하위 UI가 있고
- **When** 각 control scheme으로 메뉴 이동·확인·취소·일시정지·복귀를 수행하면
- **Then** 승인된 binding으로 같은 의미 결과를 내고 Gameplay/UI map은 한 번에 하나만 활성화되며 gamepad virtual mouse가 생성되지 않는다.

### AC-UX-009 — 포인터 포착 피드백

- **Given** 유효 후보가 없는 지점, 공격 조준 대상, 전이 가능·Cooldown·거리/LOS 차단·활성 전이 대상, UIOnly와 입력 잠금 상태가 있고
- **When** mouse pointer가 각 상태를 순서대로 통과하면
- **Then** Gameplay에서는 pointer 위치에 조준 reticle이 보이고 조준 포착에서 reticle 확대와 1px `#F7FFFC` 단일선이 유지되며, 내부 ring은 전이 가능=청록 연속선·Cooldown=주황 연속선·거리/LOS 차단=적색 단절선, 활성 전이는 안쪽 `#20E0D0`·바깥 `#F7FFFC`의 지속 2px 이중선으로 서로 구분되고 UIOnly·잠긴 모드에서는 gameplay 조준 피드백이 남지 않는다.

### AC-UX-010 — 재지정 허용 범위와 안전 입력

- **Given** keyboard/mouse와 XInput의 기본 binding 및 runtime rebind 화면이 있고
- **When** 허용된 Gameplay 입력과 고정된 axis·UI·Pause 기본 입력을 각각 재지정 또는 제거하려 하면
- **Then** keyboard Move·Gameplay button, mouse Attack·Transfer와 gamepad Gameplay button은 재지정할 수 있고 Move·Aim stick과 mouse Point axis는 고정되며 UI Navigate·Submit·Cancel과 Pause Esc·Start 기본 binding은 항상 남고 stick 반전은 별도 설정으로만 바뀐다.

### AC-UX-011 — binding 충돌의 확인 교환

- **Given** 같은 Gameplay control scheme에서 서로 다른 두 action, 다른 map의 action, protected binding과 chord 입력 후보가 있고
- **When** 이미 사용 중인 control로 rebind를 시도해 교환 승인·취소를 각각 수행하면
- **Then** 승인 시 같은 scheme의 두 Gameplay binding만 원자적으로 교환되고 취소 시 둘 다 유지되며 map 사이 동일 control은 허용되고 protected binding·chord는 거부되며 exact duplicate가 남지 않는다.

### AC-UX-012 — 화면 여백 포인터 차단

- **Given** letterbox와 pillarbox가 있는 viewport, gameplay edge 안팎의 mouse pixel과 target이 있고
- **When** pointer 이동과 Attack·Transfer·Click을 재생하면
- **Then** gameplay 안에서는 기존 target 계약이 동작하고 여백에서는 aim direction만 nearest edge로 clamp되며 target ID와 Attack·Transfer·UI Click command가 생성되지 않는다.

### AC-UX-013 — UI 논리 좌표와 hit area

- **Given** keyboard/mouse와 gamepad focus로 조작하는 HUD·menu 및 지원 viewport가 있고
- **When** minimum font·button·edge placement를 각 integer scale에서 검사·조작하면
- **Then** text·hit area·margin은 승인 최소값보다 작지 않고 같은 logical element가 같은 safe-frame 좌표에 있으며 bar 영역 Click·focus target과 UI scale option이 없다.

## Verification

package manifest·Player Settings·생성 wrapper·의미 입력 맵 정적 검사, 키보드·XInput 동등 동작 재생, 상태별 스크린 캡처, 고정 틱 입력 순서·잠금/복구 PlayMode 테스트, 짧은 이해도 관찰로 검증한다. runtime override 저장·복구는 VD-09의 해결된 플랫폼 계약을 따르며 조준 피드백의 정확한 시각 수치는 `OD-ART-001`에서 고정한다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [ADR-0019](../../adr/0019-pointer-aimed-sidescroller-controls.md), [VD-01](./01-player-movement.md), [VD-02](./02-weight-transfer.md), [VD-06](./06-humanity-choice-and-narrative.md)
