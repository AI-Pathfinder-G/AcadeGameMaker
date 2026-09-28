# M5D1 — 오르단 플레이어블 입력·카메라 연결과 터미널 전환 잠금

- Status: **Verified — Astra, 2026-09-11.** Unity 인증·entitlement 정상화를 확인한 뒤 Terra의 R13 구현을 Unity `6000.6.0f1`에서 집중 requester PlayMode `9/9`, 집중 authored EditMode `22/22`, 이전 실패 회귀 `1/1`, 전체 EditMode `483/483`, 전체 PlayMode `576/576`으로 검증했다. 모든 결과는 failure/skip/inconclusive `0`이며, Luna 독립 검토에서 미해결 P0/P1/P2 없이 AC-M5D1-001~006 전부 PASS 권고했다. Astra가 해당 증거와 권한 경계를 최종 수용한다. See [Terra handoff](../../verification/2026-09-11-vd07-m5d1-terra-handoff.md) and [Luna independent review](../../verification/2026-09-11-vd07-m5d1-luna-independent-review.md).
- Authority: Astra; implementation: Terra; independent verification: Luna
- Prerequisites: M4B3C, M5B5, M5C2 Verified; OD-M5D1-001 resolved by ADR-0028.
- Parents: VD-07 REQ-UX-004/006/007/008/013; VD-03 REQ-COM-004/006; ADR-0028/0031.

## 결과와 경계

기존 `OrdanBossEncounterSandbox`를 실제 `InputRouter`와 `SimulationCameraDriver`가 연결된 한 화면 플레이어블 샌드박스로 만든다. 오르단 사망 권위가 완료된 소스 틱 `t` 다음의 첫 입력 틱 `t+1`에서, 새 단일 요청자가 `InputRouter(-210)`보다 먼저 `Transition`을 요청해 이동·전송·공격 입력을 원자적으로 잠근다.

이 작업은 런 성공/실패, 보상, 선택지, 호감도, 프로필, 저장, 메뉴, HUD, 컷신 또는 장면 이동을 판정하지 않는다. M5A 진행 코어를 연결하지 않으며, 전환 모드는 오직 전투 종료 뒤 추가 입력을 막는 샌드박스 로컬 상태다. 전투 수치·패턴·기하·플레이어 이동 알고리즘과 기존 카메라 코어는 변경하지 않는다. 렌더러·스프라이트·애니메이션·오디오·신규 패키지도 범위 밖이다.

## 승인된 런타임 계약

`AcadeGameMaker.Input.Unity`에 internal sealed `OrdanBossTerminalTransitionRequester`를 하나 추가한다. 실행 순서는 정확히 `DefaultExecutionOrder(-211)`이며, 전역 탐색·정적 폴백·장면 권위 없이 명시 직렬화 참조만 사용한다. 동일 컴포넌트 중복을 금지한다.

요청자는 다음의 exact graph만 바인딩한다: `InputRouter`, `PlayerMovementController`, `OrdanBossCombatSimulationDriver`, `OrdanBossEncounterHandoffAdapter`. 필요하면 검증 강화를 위해 기존 scheduler/terminal teardown을 추가로 바인딩할 수 있으나 새 gameplay authority를 만들 수 없다. 초기화 전 assignment-only `ConfigureForAuthoring`, 동일 생산 경로를 호출하는 좁은 테스트 초기화/스텝 seam, immutable 최신 상태/receipt 읽기만 허용한다. 초기화 시 자신을 router의 유일 mode requester로 등록하고 이후 재구성·재등록·재부팅을 거부한다.

정상 전투 중에는 아무 요청도 하지 않는다. 소스 틱 `t`의 다음 조건이 모두 같은 encounter graph와 tick을 증명할 때에만 정확히 한 번 `RequestMode(this, InputMode.Transition, stable-nonempty-reason, t+1)`을 호출한다.

- handoff의 최신 presentation view가 `IsTerminal`, ordered exact triplet `Defeated → RewardRequest → RoomCompletionRequest`, 각 handoff tick `t`, terminal snapshot과 고정 stable-id roster를 함께 증명한다.
- combat의 completed boss-death 권위가 같은 `t`와 일치한다.
- router의 직전 authoritative `CurrentReceipt`가 `GameplayEnabled@t`다.
- player의 `NextExpectedTick`과 요청 tick이 정확히 `t+1`이다.
- 모든 명시 바인딩은 동일 player/combat/handoff graph다.

부분·누락·교차 배선·stale/future/overflow tick·부트스트랩 전 death·이미 다른 requester/pending mode가 있는 경우에는 추정하거나 보정하지 않고 요청 전 fail-closed한다. 요청자는 `InputRouter` 내부 버퍼·액션맵·consumer gate를 직접 바꾸지 않는다. `-210` router가 기존 M5B5 원자 commit으로 mode-changing locked frame `t+1`을 커밋하고, 그 receipt가 마지막 권위다. 따라서 `t+1`의 Movement/Transfer/Combat payload는 비어 있고 gameplay map은 비활성화되며 mode epoch은 정확히 1 증가한다.

요청 뒤에는 router의 authoritative `Transition@t+1` receipt를 다음 관찰 기회에 대조해 immutable 완료 receipt를 한 번 게시하고 닫힌다. 불일치·router fault·재요청·재활성화로 새 성공을 만들 수 없다. 완료 receipt는 최소 source death tick, requested/committed tick, mode/epoch과 고정 encounter key `OrdanBossEncounterGraph`를 포함하거나 같은 사실을 손실 없이 증명해야 한다. 이 key는 검증용 상수일 뿐 새 장면/Run identity 권위가 아니다. 이는 샌드박스 입력 잠금 증거일 뿐 Run 성공 증거가 아니다.

정확한 실행 순서의 핵심은 `-211 requester → -210 InputRouter → -200 Transfer cleanup → -195 terminal teardown → -190 이하 retired combat graph → default Movement → +10 Camera → +110 handoff`다. 첫 post-death 입력 프레임은 이 순서 안에서 차단되어야 한다. 기존 terminal teardown 및 camera가 같은 틱에 만드는 권위를 선행 증거로 오용하지 않는다.

## 승인된 저작 연결

기존 단일 connected `OrdanBossEncounterGraph` prefab instance 원칙을 유지한다. builder가 prefab 내부에 정확히 한 개의 실제 `InputRouter`, requester, orthographic `Camera`, `PixelPerfectCamera`, `SimulationCameraDriver`를 명시적으로 만들고 서로와 기존 player/transfer/combat/teardown/handoff에 연결한다. 카메라는 별도 명명 child로 두어 encounter actor/system transform을 움직이지 않게 하며, validator와 구조 테스트가 그 정확한 child 순서·컴포넌트 순서·참조를 읽기 전용으로 검증한다.

초기 input mode는 명시적으로 `GameplayEnabled`이며 builder 설정과 validator assertion 양쪽에서 고정한다. 이를 위해 `InputRouter`에 assignment가 아닌 read-only authored binding/initial-mode getter만 추가할 수 있고 runtime mode 동작은 변경하지 않는다. 카메라는 M5C2 profile인 PPU18, reference 640×360, Windowbox, UpscaleRenderTexture, Point, orthographic size10, rotation0, unit scale, finite z를 사용한다. 한 화면 오르단 전장은 중심 `(world 0, 4.5)`에 고정한다: pixel18 center `(0,81)`, inclusive bounds도 X `0..0`, Y `81..81`. 기준 출력은 2560×1440, 최소 지원 gameplay viewport는 640×360이며 기존 M5C2의 unsupported-output gap 정책을 그대로 따른다. 기존 24u 전투 기하와 공격 판정은 바꾸지 않는다.

builder는 새 scene/prefab만 결정적으로 재생성하고 기존처럼 저장 전후 validation을 거친다. validator는 탐색·수선·초기화를 하지 않으며 missing/duplicate/reordered/renamed/foreign component, camera/profile drift, cross-wire, wrong initial mode/order를 각각 거부한다. 기존 nested player와 scene prefab override 정책은 유지한다.

## 요구사항과 수용 기준

- REQ-M5D1-001 / AC-M5D1-001: 실제 authored scene/prefab이 실제 router+camera+requester와 동일 graph로 exact 연결되고 router authored initial mode가 `GameplayEnabled`임을 getter와 validator로 증명한다. builder 재실행이 byte-stable하며 validator가 어떠한 owner도 초기화하지 않은 채 이를 승인한다. 독립 구조 변이들은 각각 fail-closed하고 남아 있어 자동 수선이 없음을 증명한다.
- REQ-M5D1-002 / AC-M5D1-002: 정상 gameplay bootstrap과 전투 tick에는 requester가 no-op이고 기존 입력/카메라/전투 동작이 회귀하지 않는다. terminal 이전, incomplete handoff, stale/future/mismatched receipt, cross-wire 및 잘못된 실행 순서는 요청·성공 receipt 없이 거부된다.
- REQ-M5D1-003 / AC-M5D1-003: death source tick `t` 뒤 `-211`이 정확히 Transition@`t+1`을 한 번 요청하고 `-210` router가 첫 post-death tick을 locked empty frame으로 커밋한다. gameplay map off, mode epoch +1, Movement/Transfer/Combat 비어 있음, terminal cleanup과 camera/handoff 순서가 실제 driver graph에서 입증된다.
- REQ-M5D1-004 / AC-M5D1-004: 성공 뒤 immutable completion proof는 router의 exact receipt와 일치한다. duplicate/disable-reenable/overflow/conflicting pending request/router failure는 재요청·가짜 완료·tick 재라벨을 만들지 못한다.
- REQ-M5D1-005 / AC-M5D1-005: authored camera가 고정 중심과 M5C2 profile을 유지하고 640×360 및 2560×1440에서 실제 completed-camera DTO를 제공한다. 640×360 미만은 기존 gap 정책대로 aim을 억제하며 새 fallback을 만들지 않는다.
- REQ-M5D1-006 / AC-M5D1-006: 전체 EditMode/PlayMode 회귀가 zero failure/skip이다. 증거는 focused/전체 XML, 명령·Unity 버전·해시·허용 파일·미실행 항목을 기록하며 구현자는 스스로 최종 수용하지 않는다.

## 허용 파일

- 신규 `Assets/AcadeGameMaker/Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs`와 meta.
- 필요 최소의 `Input.Unity`/테스트 assembly friend 또는 asmdef 참조 조정. `InputRouter`에는 validator/requester가 쓰는 read-only bound-player/transfer/combat/terminal/camera-provider/authored-initial-mode getter만 허용한다. 기존 Core DTO와 InputRouter runtime 동작 변경은 금지한다.
- 기존 Ordan builder/validator, 해당 prefab/scene 및 metas.
- 전용 EditMode authoring tests와 PlayMode requester/end-to-end tests 및 metas; 기존 Ordan authoring test의 정확한 구조 기대값만 필요한 만큼 갱신.
- 본 계약, 신규 verification/handoff, docs index의 최소 링크.

그 밖의 runtime/core, M5A, Run/프로필/보상/UI, ProjectSettings/packages, media/art/audio, 다른 scene/prefab, 기존 verification 기록은 수정 금지다.
