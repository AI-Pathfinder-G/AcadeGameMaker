# Open Decisions Before Implementation

P0은 관련 스펙의 Approved 전환과 구현을 막는다. P1은 단위 구현 계약 전, P2는 통합 전 해결한다. 2026-08-24 현재 열린 P0은 0개다.

## Open

| ID | Priority | Owner | Decision required | Blocks |
|---|---|---|---|---|
| OD-PLAT-001 | P1 | Sol | 확정된 포인터 조준 역할 배치 안의 마우스 보정 반경·후보 정렬·스틱 보정/유지, runtime binding override 정책과 버전 저장 스키마·손상 복구 정책 | VD-02, VD-07, VD-09 |
| OD-ART-001 | P1 | Sol | 기준 픽셀 밀도, 팔레트, 기준 해상도별 카메라/조명 규칙 | VD-08, VD-09 |
| OD-SCENE-001 | P1 | Sol | 중간보스→선택→히로인 장면의 정확한 성공 경로와 실패 시 히로인 노출 여부 | VD-00, VD-06 |

## Resolved

| ID | Priority | Resolved by | Normative specs |
|---|---|---|---|
| OD-MOV-001 | P1 | [2026-08-24 이동 계약 승인](../../approvals/2026-08-24-p1-movement-approval.md) | VD-01, VD-02, VD-04, SYSTEM-CONTRACTS |
| OD-WT-001 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-01, VD-02, VD-03, SYSTEM-CONTRACTS |
| OD-ROOM-001 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-00, VD-04, SYSTEM-CONTRACTS |
| OD-RUN-001 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-05, VD-09, SYSTEM-CONTRACTS |
| OD-CHOICE-001 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-00, VD-06, SYSTEM-CONTRACTS |
| OD-CHOICE-002 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-03, VD-06, VD-07, SYSTEM-CONTRACTS |
| OD-COM-001 | P0 | [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md) | VD-03, SYSTEM-CONTRACTS |

결정이 확정되면 해당 스펙에 결과와 AC를 반영하고 `Resolved` 표에 근거 링크를 남긴다. 어렵게 되돌릴 결정이고 대안 비교가 있었다면 별도 ADR을 만든다.
