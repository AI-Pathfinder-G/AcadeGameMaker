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
5. 지원 시드 101, 202, 303, 404 각각에서 런당 4개 방의 snapshot hash를 기록하고 종점까지 도달한다. (`AC-ROOM-001`, `AC-ROOM-003`, `AC-ROOM-005`)
6. 같은 조건의 전이 미사용/사용 3쌍으로 중간보스를 격파하고 패턴 틱과 완료 시간을 비교한다. (`AC-COM-002`, `AC-COM-004`)
7. 기준 상태를 복제해 수탈과 연대 선택을 각각 실행한다. (`AC-CHOICE-001`, `AC-CHOICE-002`)
8. 두 기술로 같은 봉쇄선을 통과하고 진행 시간 차이를 기록한다. (`AC-CHOICE-004`)
9. 네 지원 시드와 두 선택을 조합한 8개 성공 경로가 마지막 히로인 장면에 도달하는지 확인한다. (`AC-CHOICE-003`)
10. 신규 프로필·자동 시드 101에서 첫 플레이 검수자 3명의 성공 경로 소요 시간과 로그를 기록한다. (`AC-SCOPE-003`, `AC-PLAT-002`)

## Failure-path script

1. 원정 자산과 활성 무게 전이를 만든다.
2. 같은 시점에 둘 이상의 실패 원인이 발생하는 검수 조건을 실행한다.
3. 실패가 한 번만 처리되고 거점으로 복귀하는지 확인한다. (`AC-RUN-001`, `AC-RUN-003`)
4. 보존 상태와 초기화 상태를 스냅샷 비교한다. (`AC-RUN-002`)
5. 새 원정을 시작하고 이전 전이·방·적 상태가 남지 않았는지 확인한다. (`AC-WT-004`)
6. 원정 중 앱을 종료·재시작해 활성 런은 사라지고 승인된 프로필 상태만 남는지 확인한다. (`AC-RUN-004`, `AC-PLAT-004`)

## Determinism script

1. 같은 후보 세트와 입력 벡터를 두 번 재생해 angle, distance, target ID 순서로 같은 대상이 선택되는지 확인한다. (`AC-WT-005`)
2. 신규 프로필에서 자동 원정을 연속 시작해 `101 → 303 → 202 → 404` 순환과 각 snapshot hash를 비교한다. (`AC-ROOM-005`)
3. 같은 시드·입력 기록으로 적과 오르단의 패턴 틱을 재생해 동일한 사건 순서를 확인한다. (`AC-COM-004`)
4. 선택 기술과 전이 입력을 같은 틱에 넣어 기술 성공 시 전이 입력 소비, 기술 실패 시 전이 처리 계속을 확인한다. (`AC-UX-004`)

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

[VD-00](./00-spec-index.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [추적성 매트릭스](./TRACEABILITY.md), [에이전트 운영 모델](../../agent-operating-model.md)
