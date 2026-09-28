# Independent QA Master Plan

2026-09-15 [ADR-0032](../adr/0032-gpt-terra-luna-subagent-standard.md) 후속: 미래 구현·QA 위임은 GPT Terra/Luna로 표준화한다. Spark는 실제 지원될 때만 사용하며, 미노출이면 Luna를 사용한다. 중요 계약은 Luna 독립 검증, Astra 수용. 아래 데모 QA 이력은 본편 관계/보스 검증 완료가 아니며 [NAR-REL](../specs/full-game-narrative/01-relationship-ending-and-retry.md)의 AC는 별도 추가·실행해야 한다. Ollama 및 기타 대체 모델 호출·한도 감시·재시도·예약은 하지 않는다.

- Status: Astra-approved planning baseline
- Planning/design support: Sol when bounded complex design review is needed
- Contract integration: Astra (ADR-0027, effective 2026-09-08)
- Implementation/tooling: GPT Terra (`gpt-5.6-terra`)
- Independent verification: Luna
- Date: 2026-09-06

## Purpose

현재 역할 기준: Terra가 도구·fixture·구현을 맡고 Luna가 사전·사후 실패 모드와 독립 누락 시나리오를 검토한다. 과거 외부 모델 draft screening은 역사적 채택 이력으로만 보존하며 추가 외주 지시가 아니다. 통합 승인자는 Astra다. 기존 QA 범위와 증적 기준은 유지한다.

UI·menu에서 이동·무게 전이·전투·원정·선택·저장·렌더링·Windows build까지 수직 데모의 결함을 독립적으로 재현하고, 모든 결과를 VD-00~09에 현재 선언된 `REQ-*`와 68개 `AC-*`에 역추적한다. 이 문서는 QA 범위와 증적 형식을 정의할 뿐 구현 승인이나 `Verified` 판정을 대신하지 않는다. 각 AC는 Luna의 독립 증적과 Astra의 통합 승인까지 완료되어야 한다. 과거 Ollama 산출물은 역사 기록일 뿐 현재 승인·판정·통합 입력이 아니다.

## Historical external-model screening

과거 screening에서 채택된 제안은 scenario ID, P0/P1/P2 priority, pre-Unity/EditMode/PlayMode/Windows/manual phase 분리, deterministic fixture·input replay, state/event/visual/performance oracle, evidence·defect field, flake control과 machine-readable catalog다. 아래 항목은 역사적 검토 결과이며 현재 외부 모델 호출 지시가 아니다.

당시 외부 모델 가정 중 다음 항목은 기존 계약과 충돌하거나 근거가 없어 폐기했다.

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

Oracles are `exact_state`, `exact_event_sequence`, `exact_bytes_or_hash`, `numeric_tolerance`, `visual_contract`, `performance_budget` or `manual_observation`. A scenario must cite at least one existing requirement and acceptance criterion. Unresolved behavior is marked `blocked` only with an approved, currently open decision ID; 현재 열린 P0·P1이 없으므로 catalog의 blocker 목록은 비어 있다.

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
5. Luna executes the independent matrix; Astra integrates only AC-linked Pass evidence.

## Current boundary (2026-09-06 audit)

- VD-11의 스키마·필수 AC manifest·시나리오 catalog·정적 validator는 `Verified`다.
- VD-01~VD-03의 승인된 계약 단위 구현이 진행 중이며, M4B1·M4B2는 독립 회귀 증적이 있다. M4B3A authored graph는 계약과 사전 게이트가 `Approved`지만 fresh Graph PlayMode 증적이 아직 없어 `Verified`가 아니다. 관련 AC는 `AC-COM-002`, `AC-COM-003`, `AC-COM-004`, `AC-WT-005`다.
- Work contract의 `Approved`는 구현 allowlist를 승인한다는 뜻이고, 구현 증적의 `Verified`와 같은 상태가 아니다. 부분 AC만 증명된 계약은 전체 계약을 `Verified`로 승격하지 않는다.
- `Assets/`, `Packages/`, `ProjectSettings/`, Unity test code와 runtime hooks의 변경은 이 QA 문서가 아니라 해당 기능의 `Approved` work contract가 허용한 allowlist 안에서만 수행한다.
- Windows build, room/run/choice/UI/persistence 소비자와 최종 아트·오디오·티저 파일은 현재 QA 완료 범위가 아니다. 해당 결과가 없는 AC는 `Pass`로 승격하지 않는다.
