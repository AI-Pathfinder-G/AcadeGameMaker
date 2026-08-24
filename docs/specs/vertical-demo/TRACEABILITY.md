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
| ADR-0018 | P0 무게 전이·방·런·선택·기술·전투를 하나의 공동장부 원정 계약으로 통합 | REQ-SCOPE-003, REQ-MOV-004, REQ-WT-001~007, REQ-COM-001~006, REQ-ROOM-001~006, REQ-RUN-001~006, REQ-CHOICE-001~007, REQ-UX-001~005, REQ-PLAT-006 | AC-SCOPE-004, AC-MOV-002, AC-WT-001~005, AC-COM-001~004, AC-ROOM-001~005, AC-RUN-001~004, AC-CHOICE-001~004, AC-UX-001~004, AC-PLAT-004 |
| [2026-08-24 P1 movement approval](../../approvals/2026-08-24-p1-movement-approval.md) | 균형 정밀 이동 수치와 검수 허용오차 | REQ-MOV-006~010, REQ-ROOM-007 | AC-MOV-004~006, AC-ROOM-006 |
| [2026-08-24 P1 input architecture approval](../../approvals/2026-08-24-p1-input-architecture-approval.md) | Input System 1.20.0 단독, 생성 wrapper, InputRouter, Gameplay/UI map | REQ-UX-006, REQ-PLAT-007 | AC-UX-005, AC-PLAT-005 |
| [ADR-0019](../../adr/0019-pointer-aimed-sidescroller-controls.md), [pointer control approval](../../approvals/2026-08-24-p1-pointer-control-approval.md) | 왼손 이동·오른손 포인터 전투와 게임패드 twin-stick 의미 대응 | REQ-WT-006, REQ-UX-007 | AC-WT-005, AC-UX-006 |
| [2026-08-24 P1 targeting approval](../../approvals/2026-08-24-p1-targeting-approval.md) | mouse direct pointer와 gamepad angular soft target의 결정적 판정 | REQ-WT-008, REQ-UX-008 | AC-WT-006, AC-UX-007 |
| [2026-08-24 P1 UI·조준 피드백 승인](../../approvals/2026-08-24-p1-ui-and-aim-feedback-approval.md) | UI 기본 binding·focus navigation과 mouse 총구 reticle·포착 강조 | REQ-UX-009~010 | AC-UX-008~009 |
| [2026-08-24 P1 재지정 범위 승인](../../approvals/2026-08-24-p1-rebinding-scope-approval.md) | Gameplay runtime rebind 허용 범위와 안전 UI·Pause binding 보호 | REQ-UX-011 | AC-UX-010 |
| [2026-08-24 P1 재지정 충돌 정책 승인](../../approvals/2026-08-24-p1-rebinding-conflict-approval.md) | 같은 Gameplay scheme 충돌의 확인 교환·취소와 중복 방지 | REQ-UX-012 | AC-UX-011 |
| [2026-08-24 P1 프로필 스키마 승인](../../approvals/2026-08-24-p1-profile-schema-approval.md) | canonical 단일 profile.json의 version 1 field·integrity·비범위 | REQ-PLAT-008 | AC-PLAT-006 |
| [2026-08-24 P1 원자 저장 승인](../../approvals/2026-08-24-p1-profile-atomic-write-approval.md) | persistentDataPath temp 검증 뒤 primary 원자 교체와 previous 보존 | REQ-PLAT-009 | AC-PLAT-007 |
| [2026-08-24 P1 로드 복구 승인](../../approvals/2026-08-24-p1-profile-load-recovery-approval.md) | primary→previous→default 선택, stale temp 격리와 binding 부분 복구 | REQ-PLAT-010~011 | AC-PLAT-008~009 |
| [2026-08-24 P1 18 PPU·2560×1440 승인](../../approvals/2026-08-24-p1-art-density-output-approval.md) | 18px environment grid와 2560×1440 output baseline | REQ-ART-008, REQ-PLAT-002 | AC-ART-005, AC-PLAT-001 |
| [2026-08-24 P1 640×360 canvas 승인](../../approvals/2026-08-24-p1-art-internal-canvas-approval.md) | gameplay logical canvas와 1440p 4× integer scale | REQ-ART-009 | AC-ART-006 |
| [2026-08-24 P1 fixed 16:9 frame 승인](../../approvals/2026-08-24-p1-art-aspect-frame-approval.md) | integer-scaled safe frame, letterbox/pillarbox와 bar input 차단 | REQ-ART-010, REQ-UX-013, REQ-PLAT-002 | AC-ART-007, AC-UX-012, AC-PLAT-001 |
| [2026-08-24 P1 predictive camera 승인](../../approvals/2026-08-24-p1-art-camera-behavior-approval.md) | fixed zoom, movement/fall anticipation, room clamp와 fixed-tick pixel snap | REQ-ART-011 | AC-ART-008 |
| [2026-08-24 P1 camera values 승인](../../approvals/2026-08-24-p1-art-camera-values-approval.md) | dead-zone·look-ahead·transition·follow cap·snap lifecycle exact values | REQ-ART-011 | AC-ART-008 |
| [2026-08-24 P1 UI scale 승인](../../approvals/2026-08-24-p1-art-ui-scale-approval.md) | logical pixel UI와 final-output SDF text·hit-area minimum | REQ-ART-012, REQ-UX-014 | AC-ART-009, AC-UX-013 |
| [2026-08-24 P1 palette roles 승인](../../approvals/2026-08-24-p1-art-palette-roles-approval.md) | world 21색·semantic 11색 역할 분리와 reserved hue 사용 제한 | REQ-ART-013 | AC-ART-010 |
| [2026-08-24 P1 palette HEX 승인](../../approvals/2026-08-24-p1-art-palette-hex-approval.md) | 32개 sRGB HEX와 역할별 고정 mapping | REQ-ART-013 | AC-ART-010 |
| [2026-08-24 P1 outline 승인](../../approvals/2026-08-24-p1-art-outline-approval.md) | logical pixel outline 두께·색·상태 우선순위와 비투시 규칙 | REQ-ART-014, REQ-UX-010 | AC-ART-011, AC-UX-009 |
| [2026-08-24 P1 URP 2D lighting 승인](../../approvals/2026-08-24-p1-art-lighting-approval.md) | light layer·preset minimum·local cap·shadow와 semantic unlit 보호 | REQ-ART-015 | AC-ART-012 |
| [2026-08-24 P1 reticle exact values 승인](../../approvals/2026-08-24-p1-art-reticle-values-approval.md) | 9×9/13×13px reticle, 3/6 tick 전환, 1px ring·halo와 mode hide | REQ-UX-010, REQ-UX-013, REQ-ART-014~015 | AC-UX-009, AC-UX-012, AC-ART-011~012 |
| [VD-11 Pre-Unity QA infrastructure](./11-pre-unity-qa-artifacts.md) | Unity 생성 전 기계 판독 QA catalog·coverage·validator | REQ-PLAT-012~014 | AC-PLAT-010~012 |

## Requirement coverage

| Spec | Requirement range | Acceptance coverage | Status |
|---|---|---|---|
| VD-00 | REQ-SCOPE-001~004 | AC-SCOPE-001~004 | Review |
| VD-01 | REQ-MOV-001~010 | AC-MOV-001~006 | Review — OD-MOV-001 resolved; Luna review pending |
| VD-02 | REQ-WT-001~008 | AC-WT-001~006 | Review — pointer targeting contract fixed; Luna review pending |
| VD-03 | REQ-COM-001~006 | AC-COM-001~004 | Review — binding contract fixed; Luna review pending |
| VD-04 | REQ-ROOM-001~007 | AC-ROOM-001~006 | Review — P0 and traversal contract resolved; Luna review pending |
| VD-05 | REQ-RUN-001~006 | AC-RUN-001~004 | Review — persistence contract fixed; Luna review pending |
| VD-06 | REQ-CHOICE-001~007 | AC-CHOICE-001~004 | Review — P0 resolved; scene detail waits on OD-SCENE-001 |
| VD-07 | REQ-UX-001~014 | AC-UX-001~013; REQ-UX-001은 입력 맵 정적 검사 | Review — P1 contracts resolved; Luna review pending |
| VD-08 | REQ-ART-001~015 | AC-ART-001~012 | Review — OD-ART-001 resolved; Luna review pending |
| VD-09 | REQ-PLAT-001~011 | AC-PLAT-001~009 | Review — platform PASS and art values resolved; Luna review pending |
| VD-11 | REQ-PLAT-012~014 | AC-PLAT-010~012 | Verified — Luna independent review PASS |

모든 REQ는 구현 작업 계약과 코드 변경에, 모든 AC는 자동 테스트 또는 수동 검수 증적에 역참조되어야 한다.
