# [VD-10] Vertical Demo Verification Script

- Status: Review
- Owner: Luna
- Contract approval/integration: Sol
- Last updated: 2026-08-24

## Required environments

1. Unity 편집기 PlayMode
2. Windows 개발 빌드
3. 신규 상태와 실패 후 복귀 상태

## Success-path script

1. 거점에서 출정한다. (`AC-SCOPE-001`)
2. 이동, 점프, 대시, 벽차기를 검수 방에서 각각 확인한다. (`AC-MOV-001`)
3. 상자에 무게를 전이·회수하고 달라진 물리 반응을 확인한다. (`AC-WT-001`)
4. 두 일반 적을 기본 공격과 무게 전이로 각각 상대한다. (`AC-COM-001`)
5. 생성 시드와 방 구성을 기록하고 종점까지 도달한다. (`AC-ROOM-001`, `AC-ROOM-003`)
6. 중간보스를 격파한다. (`AC-COM-002`)
7. 기준 상태를 복제해 수탈과 연대 선택을 각각 실행한다. (`AC-CHOICE-001`, `AC-CHOICE-002`)
8. 두 경로가 마지막 히로인 장면에 도달하는지 확인한다. (`AC-CHOICE-003`)
9. 성공 경로의 소요 시간과 로그를 기록한다. (`AC-SCOPE-003`, `AC-PLAT-002`)

## Failure-path script

1. 원정 자산과 활성 무게 전이를 만든다.
2. 같은 시점에 둘 이상의 실패 원인이 발생하는 검수 조건을 실행한다.
3. 실패가 한 번만 처리되고 거점으로 복귀하는지 확인한다. (`AC-RUN-001`, `AC-RUN-003`)
4. 보존 상태와 초기화 상태를 스냅샷 비교한다. (`AC-RUN-002`)
5. 새 원정을 시작하고 이전 전이·방·적 상태가 남지 않았는지 확인한다. (`AC-WT-004`)

## Evidence manifest

| Evidence | Required content |
|---|---|
| Build record | Unity version, target, commit/revision or archive hash, time |
| Test result | test name, AC IDs, pass/fail, environment |
| Play capture | uninterrupted path or timestamped clips, AC IDs |
| State snapshot | before/after values for persistence and transfer cleanup |
| Exception log | zero unresolved exceptions or linked defect ID |
| Verification report | Luna verdict by AC ID, deviations, reproducible steps |

검증 보고서는 모든 필수 AC를 `Pass`, `Fail`, `Blocked` 중 하나로 기록한다. `Blocked`나 계약 위반이 하나라도 있으면 Sol은 Verified 또는 통합 완료로 승인하지 않는다.

## Traceability

[VD-00](./00-spec-index.md), [추적성 매트릭스](./TRACEABILITY.md), [에이전트 운영 모델](../../agent-operating-model.md)
