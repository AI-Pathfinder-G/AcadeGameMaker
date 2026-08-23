# Vertical Demo Traceability Matrix

## Decision to specification

| Source | Normative consequence | Requirements | Acceptance criteria |
|---|---|---|---|
| ADR-0003, 0005, 0006 | 선택은 주체성 보존/수탈의 서로 다른 유효 성장 경로다 | REQ-CHOICE-001~004 | AC-CHOICE-001~002 |
| ADR-0004 | 히로인은 조건 있는 인간의 닻이며 보상물이 아니다 | REQ-CHOICE-005 | AC-CHOICE-003 |
| ADR-0007 | 무게 전이는 이동·전투·환경의 핵심 동사다 | REQ-WT-001~005, REQ-MOV-004 | AC-WT-001~004, AC-MOV-002 |
| ADR-0008 | 검증된 수작업 방을 규칙적으로 재조립한다 | REQ-ROOM-001~005 | AC-ROOM-001~003 |
| ADR-0010 | Unity 6.3 LTS, C#, URP 2D | REQ-PLAT-001 | AC-PLAT-001 |
| ADR-0011, 0012 | 고해상도 픽셀·2D 조명·관료주의 디젤펑크 | REQ-ART-001~005 | AC-ART-001~002 |
| ADR-0017 | 승인된 외부 에셋은 증적·체크섬과 함께 격리 후 임포트 | REQ-ART-003~004, REQ-ART-006~007 | AC-ART-001, AC-ART-003~004 |
| ADR-0013 | 15분 데모의 기능·콘텐츠 범위 | REQ-SCOPE-001~004, 하위 스펙 전체 | AC-SCOPE-001~004와 각 하위 스펙의 AC |
| ADR-0015 | Sol/Terra/Luna 역할과 승인 경계 | `docs/specs/README.md`의 상태 게이트와 작업 계약 필수 필드 | 모든 AC 증적에 Luna 판정, Sol 통합 승인 기록 |

## Requirement coverage

| Spec | Requirement range | Acceptance coverage | Status |
|---|---|---|---|
| VD-00 | REQ-SCOPE-001~004 | AC-SCOPE-001~004 | Review |
| VD-01 | REQ-MOV-001~005 | AC-MOV-001~003 | Review |
| VD-02 | REQ-WT-001~005 | AC-WT-001~004 | Draft — OD-WT-001 |
| VD-03 | REQ-COM-001~004 | AC-COM-001~003 | Draft — OD-COM-001 |
| VD-04 | REQ-ROOM-001~005 | AC-ROOM-001~004 | Draft — OD-ROOM-001 |
| VD-05 | REQ-RUN-001~004 | AC-RUN-001~003 | Draft — OD-RUN-001 |
| VD-06 | REQ-CHOICE-001~005 | AC-CHOICE-001~003 | Draft — OD-CHOICE-001/002 |
| VD-07 | REQ-UX-001~004 | AC-UX-001~003; REQ-UX-001은 입력 맵 정적 검사 | Draft — OD-PLAT-001 |
| VD-08 | REQ-ART-001~007 | AC-ART-001~004 | Review — acquisition allowed; import blocked by OD-ART-001 |
| VD-09 | REQ-PLAT-001~005 | AC-PLAT-001~003 | Draft — OD-PLAT-001 |

모든 REQ는 구현 작업 계약과 코드 변경에, 모든 AC는 자동 테스트 또는 수동 검수 증적에 역참조되어야 한다.
