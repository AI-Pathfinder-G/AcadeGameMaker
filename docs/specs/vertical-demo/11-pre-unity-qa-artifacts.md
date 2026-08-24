# [VD-11] Pre-Unity QA Artifact Infrastructure

- Status: Verified
- Owner and approval: Sol
- Draft implementation proposal: MiniMax M3 Cloud
- Proposal screening: Terra or Luna
- Independent verification: Luna
- Approved: 2026-08-24

## Scope

Unity 프로젝트가 생기기 전에 사용할 machine-readable QA schema, required-AC manifest, scenario catalog와 dependency-free PowerShell validator를 정의한다.

## Non-scope

Unity project·`Assets/`·`Packages/`·`ProjectSettings/`, C# test, runtime hook, scene, prefab, build runner, gameplay·canon·public contract 변경은 포함하지 않는다.

## Contract

- **Input:** VD-00~10에 현재 선언된 requirement·acceptance-criterion ID, unresolved OD ID와 scenario JSON. 여기서 68-ID manifest는 VD-00~09의 Review 후보 AC 집합을 고정한 coverage baseline이며, 해당 기능 스펙의 Approved·구현 완료를 주장하지 않는다.
- **Output:** deterministic validation exit code와 duplicate·missing coverage·invalid field·unknown reference 진단.
- **Owned state:** `qa/schema/`, `qa/catalog/`, `qa/coverage/`, `qa/tools/`, `qa/README.md`.
- **Invariants:** scenario는 gameplay 값을 소유하지 않고 existing REQ/AC를 참조한다. `blocked` scenario만 `OD-ART-001` 또는 `OD-SCENE-001`을 참조한다. validator는 네트워크·Unity·외부 module 없이 동작하고 파일을 수정하지 않는다.

Catalog schema는 `schemaVersion=1`, unique `scenarioId`, `domain`, `caseType`, `priority`, `phase`, `status`, nonempty `requirementIds`, nonempty `acceptanceCriterionIds`, `fixtureRefs`, ordered `steps`, typed `oracle`, `evidence`, `blockedBy`, `tags`를 요구한다. phase는 `pre_unity`, `editmode`, `playmode`, `windows_build`, `manual`; status는 `draft`, `ready`, `blocked`다.

Required-AC manifest는 `AC-SCOPE-001~004`, `AC-MOV-001~006`, `AC-WT-001~006`, `AC-COM-001~004`, `AC-ROOM-001~006`, `AC-RUN-001~004`, `AC-CHOICE-001~004`, `AC-UX-001~013`, `AC-ART-001~012`, `AC-PLAT-001~009`의 68개 unique ID를 정확히 가진다.
VD-11 자체의 `AC-PLAT-010~012`는 QA infrastructure 검증용이므로 이 gameplay coverage manifest에는 포함하지 않는다.

## Requirements

- **REQ-PLAT-012:** QA catalog schema와 catalog는 unique ID·enum·required field·REQ/AC pattern을 검증하고 required-AC manifest의 68개 ID를 최소 한 scenario가 모두 cover해야 한다.
- **REQ-PLAT-013:** dependency-free PowerShell validator는 valid artifact에 exit 0, invalid JSON·duplicate ID·missing field·invalid enum·unknown or uncovered AC·부당한 blocker에 nonzero를 반환하고 stable diagnostic을 출력해야 한다.
- **REQ-PLAT-014:** pre-Unity QA artifact는 `qa/` 밖을 쓰지 않고 Unity/runtime 파일을 만들지 않으며 unresolved exact behavior는 `blocked`+approved OD reference로 남겨야 한다.

## Acceptance criteria

### AC-PLAT-010 — schema and complete AC coverage

- **Given** checked-in schema, 68-ID manifest and scenario catalog가 있고
- **When** validator를 repository root에서 실행하면
- **Then** exit 0이고 scenario ID는 unique이며 manifest 68개 AC가 적어도 한 scenario에 모두 연결된다.

### AC-PLAT-011 — deterministic rejection

- **Given** valid catalog의 invalid JSON, duplicate scenario ID, missing required field, invalid enum, unknown AC, uncovered AC와 unauthorized blocker mutation이 각각 있고
- **When** validator self-test가 각 mutation을 검사하면
- **Then** 각 case는 nonzero validation result와 stable category diagnostic을 만들고 원본 artifact를 수정하지 않는다.

### AC-PLAT-012 — scope isolation

- **Given** VD-11의 `qa/` 구현 payload diff와 repository tree가 있고 (`docs/`의 spec·plan·verification·navigation 기록은 governance evidence로 별도 분류한다)
- **When** allowed/forbidden path와 generated file을 검사하면
- **Then** 구현 파일은 `qa/`에만 있고 Unity project/runtime file, external dependency, network call과 cloud credential이 없다.

## Verification

`pwsh -NoProfile -File qa/tools/Test-QaCatalog.ps1`와 `pwsh -NoProfile -File qa/tools/Test-QaCatalog.ps1 -SelfTest`를 실행하고 path diff를 검사한다. 결과는 AC-PLAT-010~012를 인용한다.

독립 검증 결과는 [2026-08-24 VD-11 Luna review](../../verification/2026-08-24-vd-11-luna-review.md)에 기록한다.

## Traceability

[QA master plan](../../qa/qa-master-plan.md), [VD-10](./10-verification-script.md), [TRACEABILITY](./TRACEABILITY.md), [agent operating model](../../agent-operating-model.md)
