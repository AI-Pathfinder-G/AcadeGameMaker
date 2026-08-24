# Documentation Map

이 문서는 프로젝트 문서의 단일 진실원천, 충돌 해결 순서, 변경 절차를 정의한다.

## 읽는 순서

1. [CONTEXT.md](../CONTEXT.md) — 프로젝트 고유 용어
2. [게임 디자인 캐논](./canon/game-design.md), [서사 캐논](./canon/narrative-canon.md), [미술 캐논](./canon/art-direction.md) — 현재 프로젝트의 사실
3. [ADR 색인](./adr/README.md) — 중요한 결정과 이유
4. [스펙 색인](./specs/README.md) — 구현 가능한 계약과 검증 기준
5. [에이전트 운영 모델](./agent-operating-model.md) — 역할과 승인 흐름
6. [위키 홈](../wiki/Home.md) — 발행·탐색용 보기

[제안서 색인](./proposals/README.md)은 검토 이력과 비캐논 초안을 구분해 보여 준다.

## 현재 준비도

- [2026-08-24 본격 개발 착수 전 준비도 점검](./project-readiness-audit-2026-08-24.md)
- [2026-08-24 환경 준비 및 제품 기준 승인 기록](./approvals/2026-08-24-environment-and-product-baseline.md)
- [2026-08-24 개발 환경 준비 상태](./environment/setup-status-2026-08-24.md)
- [Sol P0 통합 기본안 — Accepted](./proposals/sol-p0-integration-proposal.md)
- [2026-08-24 P0 통합 승인 기록](./approvals/2026-08-24-p0-integration-approval.md)
- [2026-08-24 P1 이동 계약 승인 기록](./approvals/2026-08-24-p1-movement-approval.md)
- [2026-08-24 P1 입력 구조 승인 기록](./approvals/2026-08-24-p1-input-architecture-approval.md)
- [2026-08-24 P1 포인터 조준 조작 승인 기록](./approvals/2026-08-24-p1-pointer-control-approval.md)
- [2026-08-24 P1 포인터·스틱 타겟 판정 승인 기록](./approvals/2026-08-24-p1-targeting-approval.md)
- [2026-08-24 P1 UI·조준 피드백 승인 기록](./approvals/2026-08-24-p1-ui-and-aim-feedback-approval.md)
- [2026-08-24 P1 런타임 입력 재지정 범위 승인 기록](./approvals/2026-08-24-p1-rebinding-scope-approval.md)
- [2026-08-24 P1 입력 재지정 충돌 정책 승인 기록](./approvals/2026-08-24-p1-rebinding-conflict-approval.md)
- [2026-08-24 P1 단일 프로필 저장 스키마 승인 기록](./approvals/2026-08-24-p1-profile-schema-approval.md)
- [2026-08-24 P1 프로필 원자 저장 순서 승인 기록](./approvals/2026-08-24-p1-profile-atomic-write-approval.md)
- [2026-08-24 P1 프로필 로드 복구 승인 기록](./approvals/2026-08-24-p1-profile-load-recovery-approval.md)
- [2026-08-24 OD-PLAT-001 Luna 독립 검토 — PASS](./verification/2026-08-24-od-plat-001-luna-review.md)
- [2026-08-24 P1 18 PPU·2560×1440 출력 기준 승인 기록](./approvals/2026-08-24-p1-art-density-output-approval.md)
- [2026-08-24 P1 640×360 내부 픽셀 캔버스 승인 기록](./approvals/2026-08-24-p1-art-internal-canvas-approval.md)
- [2026-08-24 P1 고정 16:9 화면 프레임 승인 기록](./approvals/2026-08-24-p1-art-aspect-frame-approval.md)
- [2026-08-24 P1 이동 예측형 고정 배율 카메라 승인 기록](./approvals/2026-08-24-p1-art-camera-behavior-approval.md)
- [2026-08-24 P1 카메라 추적 수치 승인 기록](./approvals/2026-08-24-p1-art-camera-values-approval.md)
- [2026-08-24 P1 픽셀 UI·SDF 텍스트 배율 승인 기록](./approvals/2026-08-24-p1-art-ui-scale-approval.md)
- [2026-08-24 P1 32색 역할 분리 팔레트 승인 기록](./approvals/2026-08-24-p1-art-palette-roles-approval.md)
- [2026-08-24 P1 32색 HEX 팔레트 승인 기록](./approvals/2026-08-24-p1-art-palette-hex-approval.md)
- [2026-08-24 P1 픽셀 윤곽선 승인 기록](./approvals/2026-08-24-p1-art-outline-approval.md)
- [2026-08-24 P1 URP 2D 조명 승인 기록](./approvals/2026-08-24-p1-art-lighting-approval.md)
- [2026-08-24 P1 조준기 exact values 승인 기록](./approvals/2026-08-24-p1-art-reticle-values-approval.md)
- [2026-08-24 OD-ART-001 Luna 독립 검토 — PASS](./verification/2026-08-24-od-art-001-luna-review.md)
- [ADR-0020 히로인 유대·사이드킥·진엔딩](./adr/0020-heroine-bond-sidekick-and-true-ending.md)
- [ADR-0021 AimArc·차지 탄도](./adr/0021-character-aim-arc-and-charged-ballistics.md)
- [2026-08-24 인물 중심 8챕터 시놉시스 승인](./approvals/2026-08-24-character-first-eight-chapter-synopsis-approval.md)
- [ADR-0022 인물 중심 8챕터·자기정당화형 광오](./adr/0022-character-first-eight-chapter-narrative.md)
- [NAR-00 전체 게임 서사 스펙 — Review](./specs/full-game-narrative/00-spec-index.md)
- [2026-08-24 NAR-00 Luna 독립 문서 검토 — CONDITIONAL](./verification/2026-08-24-nar-00-luna-review.md)
- [독립 QA 마스터 플랜](./qa/qa-master-plan.md)
- [Pre-Unity QA 아티팩트 작업 계약](./qa/pre-unity-qa-work-contract.md)
- [2026-08-24 VD-11 Luna 독립 검증](./verification/2026-08-24-vd-11-luna-review.md)

## 단일 진실원천

| 질문 | 권위 있는 위치 | 담지 않는 것 |
|---|---|---|
| 이 용어는 무엇을 뜻하는가? | `CONTEXT.md` | 구현 수치, 일정, 테스트 |
| 현재 게임 세계와 제품 방향은 무엇인가? | `docs/canon/` | 결정 과정, 코드 구조 |
| 왜 이 선택을 했는가? | `docs/adr/`의 현재 accepted ADR | 세부 동작, 작업 목록 |
| 무엇을 어떻게 검증해야 하는가? | `docs/specs/`의 Approved 스펙 | 결정의 역사, 홍보 문구 |
| 누가 설계·구현·검수하는가? | `AGENTS.md`, `docs/agent-operating-model.md` | 게임 규칙 |
| 독자는 어디서 내용을 찾는가? | `wiki/` | 새로운 결정이나 요구사항 |

## 충돌 해결 규칙

1. superseded ADR은 현재 판단 근거로 사용하지 않는다.
2. 현재 ADR과 캐논이 충돌하면 구현을 멈추고 Sol이 둘을 함께 정정한다.
3. 스펙이 캐논 또는 현재 ADR과 충돌하면 스펙을 수정한다.
4. 위키가 원본 문서와 다르면 원본을 따르고 위키를 갱신한다.
5. 대화, 작업 메모, 에이전트 초안은 문서에 승인 반영되기 전까지 권위가 없다.

## 변경과 추적성

- 중요한 비가역 결정은 ADR 번호 `ADR-NNNN`으로 기록한다.
- 기능 요구사항은 스펙별 접두사가 있는 `REQ-...` ID를 갖는다.
- 인수 기준은 `AC-...` ID를 갖고 하나 이상의 요구사항을 검증한다.
- 미결정은 `OD-...` ID로 기록하며, 해결 전에는 관련 스펙을 `Approved`로 올리지 않는다.
- 구현 변경은 관련 REQ ID를, 테스트·플레이 검수 결과는 관련 AC ID를 남긴다.
- 위키 페이지는 하단의 `권위 문서` 링크로 원본을 가리킨다.

## 상태 게이트

`Draft → Review → Approved → Implemented → Verified`

- Sol만 스펙을 `Approved`로 전환하고 통합을 승인한다.
- Terra는 Approved 계약 안에서 단위 파트를 설계·구현한다.
- Luna는 AC ID에 따라 독립 검수하고 `Verified` 증적을 남긴다.
- Ollama 산출물은 Draft이며 GPT 검토 전에는 어느 단계도 전환하지 못한다.
