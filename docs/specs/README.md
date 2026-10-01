# Specification System

- [현재 C3·C4 공동 수용](../approvals/2026-10-01-c3-c4-v15-joint-final-integration-acceptance.md) — Approved 계약의 현재 v19 동결 소스에서 C3 AC007/008과 C4 AC001..010을 공동 검증했다. 아래 진행 상태 문구는 해당 문서가 작성된 당시의 단계 기록이며, 최초 게임 코드의 원격 게시 권한은 별개다.

- [C4 발급 후 미반환 경계 증거 — Approved](./work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md) · [추가 부모와 Busy 출처 — Approved](./work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md) — 원본 투영 손상·생명주기 중단·proof 손상과 C2 전/새 handback 공개 전의 일곱 실패 경계를 한정한다. 동일 손상 본문의 추가 공개 부모와 직접 읽을 수 없는 C1 Busy의 source 출처를 구별하며 구현·검증은 진행 중이다.
- [C4 C1 미호출 실패 증거 — Approved](./work-contracts/2026-09-30-c4-pre-c1-failure-evidence-amendment.md) — 실행 기록 발급 후 C1 전 일곱 경계에서 소비 전후와 미관측 세대를 정확 구분한다. 구현·검증은 진행 중이다.

- [C4 일반 저장 차단 미관측 증거 — Approved](./work-contracts/2026-09-30-c4-ordinary-writer-observation-amendment.md) — 실제 일반 저장 호출이 없는 동일 사례의 `ordinaryWriterBlocked` 한 값만 정확 미관측으로 허용한다. 기존 실제 writer 검사와 나머지 정상 증거는 유지한다.

- [C4 r4 실행 연결 — Approved, 구현 진행](./work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md) · [QA 증거 규약 r2](./work-contracts/2026-09-29-c4-qa-evidence-protocol.md) · [발급 세대 증거 개정](./work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md) · [C2 미호출 실패 증거 개정](./work-contracts/2026-09-30-c4-pre-c2-failure-evidence-amendment.md) — 현재 구현 계약이다. 한정 접근성·선행 선택 형식·행 출력 개정은 [현재 문서 지도](../README.md)에 연결한다. 독립 정적 설계 검수와 구현 승인 이후 최종 시험 연결·동결·실제 실행·공동 통합 수용은 진행 중이다.

- [VD-09 M5D7Q-C3L 관찰 전용 잠금 구분 — Verified](./work-contracts/2026-09-28-vd09-m5d7q-c3l-observation-lease-provenance.md) · [최종 독립 검수](../verification/2026-09-29-c3l-final-regression-luna-acceptance-review.md) · [통합 수용](../approvals/2026-09-29-c3l-integration-acceptance.md) — 집중 11개와 동일 소스 필수 회귀 고유 613개로 관찰 단위만 수용했다. 실제 잠금 충돌 시간 초과만 재시도 상태이며 접근·읽기 실패는 종료 상태로 구분한다. C3 전체나 C4 실행 연결 수용은 아니다.

- [VD-09 M5D7Q-C2R 실제 재시작 복구 — Verified](./work-contracts/2026-09-28-vd09-m5d7q-c2r-restart-bootstrap.md) · [Luna 최종 독립 검수](../verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) — 실제 별도 프로세스 복구와 삭제 후 종료·재시작, 현재 EditMode 613건·PlayMode 610건의 범위별 회귀를 검증했다. 부모 C2 AC-007을 닫았으며 실제 메뉴·목적지 연결은 승인하지 않는다.

- [VD-09 M5D7Q-C3 새 게임 확인·취소 — Approved, synthetic-only](./work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md) · [Luna 사전 검토](../verification/2026-09-28-vd09-m5d7q-c3-luna-pregate.md) — 확인 정책과 새 입력 세대의 구현 계약. 실제 메뉴 UI·초기화 실행·장면 연결을 승인하지 않는다.

- [VD-09 M5D7Q-C1 저장 초기화 — Verified, disk-only](./work-contracts/2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md) · [실행·수용 기록](../verification/2026-09-28-vd09-m5d7q-c1-continuation.md) — 212/212 단위·장애, 51/51 실제 종료·복구, 175/175 프로필 회귀 및 Luna P0/P1=0 후 Astra 수용. 실제 메뉴 연결은 미완료.
- [VD-09 M5D7Q-C2 입력·메모리 전환 — Verified, synthetic-only](./work-contracts/2026-09-28-vd09-m5d7q-c2-memory-cutover.md) — Luna P0/P1=0 후 Astra 수용. UI·장면·실제 요청 연결은 별도 계약이다.

- [VD-09 M5D7Q-C 새 게임 확인·단일 프로필 초기화 — Approved](./work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md) · [잠금·표식 차단 기반 — 부분 검증](../verification/2026-09-28-vd09-m5d7q-c-lock-barrier-foundation.md) — 이전 진행은 수동 복구 전용으로 보관하고 자동 복구 후보에서 제외한다. Terra의 1차 기반은 Luna 독립 검증 후 수용했으나 실제 초기화·UI·전환은 미완료.
- [BOSS-F 보스전 구성 기준](./boss-encounter-framework.md) — 2026-09-08 사용자 결정 반영. 디자인 제약 Approved, 후반 런타임 구현·전투 검증은 별도 계약.
- [VD-09 M5D7Q-A authored uGUI hub shell — Verified](./work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md) · [Terra execution evidence](../verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md) · [Luna final postreview — PASS](../verification/2026-09-27-vd09-m5d7q-a-final-luna-postreview.md) · [retry-r visual review — PASS](../verification/2026-09-27-vd09-m5d7q-a-resolution-capture-retry-r-visual-review.md) — Astra integrated the independently reviewed final package. The evidence distinguishes the Approved-at-execution SHA from the current Verified contract SHA and includes separate-process byte equality.

2026-09-15 [ADR-0032](../adr/0032-gpt-terra-luna-subagent-standard.md) 적용: Astra가 현재 계약 승인과 최종 통합을 소유한다. 과거 승인일·구현 계약은 유지하며, 새 단위는 GPT Terra가 구현하고 GPT Luna가 독립 검증한다. Ollama 및 기타 대체 모델은 현재 작업 흐름에 배정하지 않는다.

스펙은 구현과 검수에 직접 사용할 수 있는 규범 문서다. 캐논과 ADR을 반복 설명하지 않고, 관찰 가능한 요구사항과 인수 기준으로 변환한다.

## 상태

| 상태 | 의미 | 구현 가능 여부 |
|---|---|---|
| Draft | 핵심 계약 또는 미결정이 남아 있음 | 불가 |
| Review | 계약과 기준이 작성되어 검토 중 | 불가 |
| Approved | Astra가 범위·계약·인수 기준을 승인함 | 가능 |
| Implemented | Terra가 구현과 단위 검증 증적을 제출함 | 통합 대기 |
| Verified | Luna 검증과 Astra 통합 승인이 끝남 | 완료 |

문서에 상태가 없으면 Draft로 취급한다.

## ID 규칙

- 스펙: `VD-NN`
- 요구사항: `REQ-{영역}-{NNN}`
- 인수 기준: `AC-{영역}-{NNN}`
- 미결정: `OD-{영역}-{NNN}`
- 검증 증적: `EV-{날짜}-{AC ID}`

영역 접두사는 `SCOPE`, `MOV`, `WT`, `COM`, `ROOM`, `RUN`, `CHOICE`, `UX`, `ART`, `PLAT`, `NAR`을 사용한다.

## 승인 조건

스펙을 Approved로 전환하려면 다음이 모두 있어야 한다.

- 범위와 비범위
- 입력, 출력, 소유 상태, 불변 조건을 설명한 Contract
- 고유 ID를 가진 요구사항
- Given/When/Then 또는 동등하게 측정 가능한 인수 기준
- 각 REQ를 적어도 하나의 AC가 검증하는 추적성
- 열려 있는 P0 미결정 없음
- 허용 파일, 금지 영역, 롤백 지점은 구현 작업 할당 시 기록
- Astra 승인자와 승인일

## 완료 조건

구현 완료는 코드 존재가 아니라 관련 AC가 증적으로 검증된 상태다. Terra는 단위 테스트와 구현 메모를 제출하고, Luna는 독립 검증 결과를 남기며, Astra는 계약 변경 여부와 통합 결과를 승인한다.

## 현재 패키지

- [15분 수직 데모 스펙 색인](./vertical-demo/00-spec-index.md)
- [인물 중심 8챕터 서사 스펙](./full-game-narrative/00-spec-index.md)
- [Pre-Unity QA Artifact Infrastructure](./vertical-demo/11-pre-unity-qa-artifacts.md)
- [추적성 매트릭스](./vertical-demo/TRACEABILITY.md)
- [열린 결정](./vertical-demo/OPEN-DECISIONS.md)
- [단위 스펙 템플릿](./templates/feature-spec-template.md)
- [작업 계약 템플릿](./templates/work-contract-template.md)
