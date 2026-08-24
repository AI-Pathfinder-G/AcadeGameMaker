# Independent QA Master Plan

- Status: Sol-screened planning baseline
- Planning draft source: GLM 5.2 Cloud, generic non-sensitive prompt only
- Contract integration: Sol
- Artifact proposal: MiniMax M3 Cloud
- Independent verification: Luna
- Date: 2026-08-24

## Purpose

UI·menu에서 이동·무게 전이·전투·원정·선택·저장·렌더링·Windows build까지 수직 데모의 결함을 독립적으로 재현하고, 모든 결과를 VD-00~09에 현재 선언된 `REQ-*`와 68개 `AC-*`에 역추적한다. 이 coverage baseline은 Review 상태 기능의 구현 승인을 의미하지 않는다. Ollama 산출물은 초안이며 승인·판정·통합 권한이 없다.

## GLM draft screening

채택한 제안은 scenario ID, P0/P1/P2 priority, pre-Unity/EditMode/PlayMode/Windows/manual phase 분리, deterministic fixture·input replay, state/event/visual/performance oracle, evidence·defect field, flake control과 machine-readable catalog다.

다음 GLM 가정은 기존 계약과 충돌하거나 근거가 없어 폐기했다.

- dash invincibility: VD-01은 대시 중 무적을 금지한다.
- boss health phase: 오르단은 계약된 deterministic pattern 순환을 사용한다.
- failure meta-progression update: 실패는 승인된 보존 경계만 유지한다.
- pause=`timeScale 0` 강제: 계약은 InputMode와 map gate를 소유하고 구현 방식을 고정하지 않는다.
- generic patrol/chase AI와 random room shuffle: 적 프로필과 네 authored snapshot 계약을 따른다.

## Test layers

| Phase | Purpose | Gate |
|---|---|---|
| pre_unity | JSON schema·scenario catalog·AC coverage·정적 validator | VD-11 Approved |
| editmode | pure state, quantization, ordering, serialization, hash, failure injection | owning feature spec Approved and Unity project exists |
| playmode | input→tick→state, physics, lifecycle, UI feedback, room and boss integration | owning feature spec Approved |
| windows_build | build smoke, device input, persistence recovery, render/performance, E2E replay | VD-09 and consumers Approved |
| manual | readability, authored traversal, narrative meaning, paired boss and first-player timing | playable build and Luna protocol |

## Scenario identity and priority

ID format is `QA-{DOMAIN}-{POS|NEG|BND|LIFE|DET|REC|VIS|READ|BUILD}-{NNN}`. P0 means crash·data loss·launch/run blocker·contract determinism breach, P1 means core-loop or lifecycle breach, P2 means non-blocking visual/readability/performance-quality defect.

## Required domains

- UI/menu: boot, focus, mouse and XInput navigation, pause stack, settings, rebind allow/deny/swap/cancel, HUD, reticle, bars and safe frame.
- gameplay: movement boundaries, target ranking, transfer atomicity, damage dedupe, enemies, boss pattern, room snapshots, seed cycle, run end, choice skills and lifecycle cleanup.
- persistence: canonical payload, atomic failure injection, primary/previous/temp recovery, binding partial recovery and active-run exclusion.
- visual/platform: integer scaling, camera determinism, palette, outline, lighting, Windows build, 60 FPS and unresolved-exception checks.
- E2E: four seeds × two choices, success, each approved failure cause, restart, editor/build equivalence and evidence completeness.

## Fixtures and oracles

Fixtures use authored IDs, four supported room snapshots, integer SimulationTick, quantized input samples, explicit profile file combinations and deterministic event logs. They never use wall clock, Unity instance ID or creation order as gameplay keys.

Oracles are `exact_state`, `exact_event_sequence`, `exact_bytes_or_hash`, `numeric_tolerance`, `visual_contract`, `performance_budget` or `manual_observation`. A scenario must cite at least one existing requirement and acceptance criterion. Unresolved behavior is marked `blocked` with `OD-ART-001` or `OD-SCENE-001`; the catalog may not invent the missing value.

## Evidence and defects

Every result records scenario ID, AC IDs, commit/build identity, environment, fixture hash, replay hash when applicable, expected/actual, pass/fail/blocked, artifacts and linked defect. Defects additionally record priority, minimal reproduction, first bad revision if known, lifecycle state, target device and verification owner.

## Flake control

- deterministic cases require three identical state/event/hash runs unless an owning AC specifies another count.
- retry never converts failure to pass; a pass/fail mixture is a flake defect.
- visual nondeterminism is masked only by an approved mask manifest; gameplay-semantic pixels cannot be masked.
- timeout, missing fixture, missing build and unresolved OD are `Blocked`, not `Pass`.
- Luna does not accept evidence produced only by the implementer without independent replay or inspection.

## Implementation order

1. VD-11 schema, required-AC manifest, scenario catalog and dependency-free validator.
2. Terra maps approved feature interfaces to EditMode fixtures and deterministic probes.
3. Terra implements PlayMode input replay and lifecycle test scenes inside Approved contracts.
4. Windows build runner collects logs, screenshots, profile files and performance captures.
5. Luna executes the independent matrix; Sol integrates only AC-linked Pass evidence.

## Current boundary

Only step 1 is authorized now. There is no Unity project, VD-00~09 remain Review, and `OD-ART-001`·`OD-SCENE-001` remain open. Creating `Assets/`, `Packages/`, `ProjectSettings/`, Unity test code or runtime hooks is prohibited until their owning specs are Approved.
