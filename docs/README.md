# Documentation Map

- [C3 실행 관리자 조회 함수 사용자 승인](./approvals/2026-09-28-c3-readonly-launch-root-accessor-approval.md) — 이번 세션에서 내부 읽기 전용 함수 추가를 명시적으로 승인받았다. Q0 후속 지문 변경은 정확한 소스 독립 검수와 별도 아스트라 승인이 필요하다.
- [C3L 관찰 잠금 구분 — Approved, 미검증](./specs/work-contracts/2026-09-28-vd09-m5d7q-c3l-observation-lease-provenance.md) · [C4 초기화 실행 연결 — Review](./specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md) — C3/C3L 새 검증 후 C4 계약 승인과 구현을 진행한다. 이전 인계서의 승인 대기는 당시 기록이며 위 사용자 승인으로 조회 함수 차단만 해소됐다.

- [다음 세션 인계서](./handoffs/2026-09-28-next-session.md) · [핵심 파일 지문](./handoffs/2026-09-28-workspace-checkpoint.json) — 사용자 요청으로 구현을 종료하고 작업물을 보존했다. C2/C2R 수용 결과와 C3L 미검증 변경분을 구분했으며, 다음 세션은 실행 관리자 내부 조회 함수의 미답변 승인부터 확인해야 한다. 이 인계 요청 자체는 해당 변경 승인이 아니다.

- [M5D7Q-C2/C2R 최종 독립 검수 — PASS](./verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) · [R36 실행·입력 증빙](./verification/2026-09-28-vd09-m5d7q-c2r-r36-execution.md) — 사용자 재개 후 C2/C2R을 Verified로 수용했다. 편집 모드 562+51=613건, 실행 모드 536+74=610건으로 당시 동결 소스를 정확히 대조했다. 실패·건너뜀·판정 불가 0, Luna P0/P1=0이다. C3L의 이후 수정은 아직 미검증이며 C3 구현은 승인 대기 상태로 다음 세션에 인계했다. 아래 이전 중단·미완료 기록은 당시 이력이다.

- [Unity 검사 프로세스 대기 도구 — Verified](./specs/work-contracts/2026-09-28-unity-qa-owned-editor-wait.md) · [Luna 실제 실행 검수](./verification/2026-09-28-unity-qa-owned-editor-wait-luna-r33-actual-review.md) — 사용자가 재개를 지시했다. 보완 도구의 자체 검사 104개와 실제 Unity 집중 검사 32/32를 검증해 QA 도구 범위만 수용했다. 게임 C2/C2R 회귀의 대상 선택 문제와 최종 수용은 별도로 남아 있다.

- [현재 작업분 종료·일시 중단 기록](./verification/2026-09-28-current-batch-pause-checkpoint.md) · [Luna R22 실제 결과 검수](./verification/2026-09-28-vd09-m5d7q-c2r-r22-pause-luna-digest.md) — R22는 614개 중 610개 통과, 별도 실행 정보가 필요한 프로세스 테스트 4개 실패로 종료했다. 의도한 제외 필터가 적용되지 않은 검사 범위 문제이며, 전체 통과로 처리하지 않는다. C2/C2R 최종 수용은 미완료다. 사용자 재개 지시 전에는 재실행·C3 구현·C4 승인·새 검사 실행을 하지 않는다.

- [M5D7Q-C2R 실제 재시작 복구 — Verified](./specs/work-contracts/2026-09-28-vd09-m5d7q-c2r-restart-bootstrap.md) · [최종 독립 검수](./verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) · [실제 프로세스 검증](./verification/2026-09-28-vd09-m5d7q-c2r-process-execution.md) — 초기화 후 복구와 표식 삭제 후 종료·재시작을 실제 프로세스로 검증했다. 현재 편집 모드 613건과 실행 모드 610건의 범위별 회귀를 통과하고 최종 수용했다. 이전 592건의 결과를 현재 수용 근거로 재사용하지 않았으며 실제 메뉴·장면은 연결하지 않았다.

- [M5D7Q-C3 새 게임 확인·취소 소유권 — Approved, synthetic-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md) · [Luna 사전 설계 검토](./verification/2026-09-28-vd09-m5d7q-c3-luna-pregate.md) — 최신 프로필 관찰, 확인·취소 단일 승자와 취소 후 새 입력 세대를 규정한다. 구현·실행·실제 확인 UI 연결은 아직 미검증이다.

- [M5D7Q-C1 새 게임 저장 트랜잭션 — Verified, disk-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md) · [실행·Astra 수용 기록](./verification/2026-09-28-vd09-m5d7q-c1-continuation.md) · [Luna 최종 독립 검토](./verification/2026-09-28-vd09-m5d7q-c1-final-luna-review.md) — 단위·장애 212/212, 실제 프로세스 종료·복구 51/51, 프로필 회귀 175/175. R13 집계 도구 조기 종료와 실제 Unity 성공을 구분했다. 실제 메뉴·메모리 적용·확인 UI·장면 전환은 연결하지 않았다.
- [M5D7Q-C2 메모리·입력 전환 — Verified](./specs/work-contracts/2026-09-28-vd09-m5d7q-c2-memory-cutover.md) — 원래 프로필 경로·실행 소유자를 재검증하고 입력과 현재 프로필을 안전하게 전환한다. 루나 독립 검증 후 아스트라가 수용했으며 실제 메뉴나 장면 연결은 별도 계약이다.

- [새 게임 단일 프로필 초기화 확인 — 사용자 승인](./approvals/2026-09-28-new-game-single-profile-confirmation-approval.md) · [ADR-0035 수동 복구 전용 이전 진행](./adr/0035-new-game-manual-only-prior-profile.md) · [M5D7Q-C 초기화 계약 — Approved](./specs/work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md) · [잠금·표식 차단 기반](./verification/2026-09-28-vd09-m5d7q-c-lock-barrier-foundation.md) · [게임 시작 전체 잠금 — 부분 검증](./verification/2026-09-28-vd09-m5d7q-c-launch-lease-evidence.md) — 수동 보관·자동 복구 제외를 고정하고 저장 안전장치와 게임 시작 전체 잠금을 검증했다(EditMode 175/175, PlayMode 36/36). 실제 초기화·확인 UI·인게임 거점 전환은 미완료.

- [M5D7Q-B hub intent handoff — Verified, synthetic-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-b-hub-intent-handoff.md) · [Terra implementation and execution evidence](./verification/2026-09-28-vd09-m5d7q-b-preexecution-evidence.md) · [Luna independent postreview](./verification/2026-09-28-vd09-m5d7q-b-luna-postreview.md) — four menu selections transfer as one typed, one-shot request without executing effects. Host-context Unity focused EditMode `5/5`, PlayMode `4/4`, direct HubPresentation EditMode `68/68`, PlayMode `8/8`; Luna `P0/P1=0` and Astra acceptance. No destination scene, profile mutation, or real wardrobe/gameplay connection is implied.

- [2026-09-13 costume pure-core implementation handoff](./verification/2026-09-13-costume-pure-core-implementation-handoff.md) — REQ-COST-001..009/012/014 additive first slice. 36-entry inventory remains production pending; Unity, media processing, and filesystem adapters are deferred.
- [Costume CIO file adapter — Verified](./specs/work-contracts/2026-09-20-costume-cio-file-adapter.md) — 의상 전용 primary/previous/temp 저장·복구를 분리한다. [Terra 구현 증적](./verification/2026-09-20-costume-cio-implementation-evidence.md)과 [Luna 기존 독립 검토](./verification/2026-09-20-costume-cio-luna-independent-review.md)의 정확한 소스가 유지된 상태에서 후속 전체 Unity 스위트 788/788 EditMode·947/947 PlayMode가 통과했고, [AC-CIO-007 closure — PASS](./verification/2026-09-27-costume-cio-ac007-closure-review.md) 후 Astra가 최종 통합했다.
- [Character sprite and wardrobe readiness audit](./verification/2026-09-20-character-sprite-and-wardrobe-readiness-audit.md) — Luna 독립 감사 결과 세령 조합은 Accepted `0/432`이며 현재 자료는 source-only이다. 저장·표시 프레임워크는 별도 승인으로 진행하되 실제 착용/인게임 바인딩은 첫 미디어 패키지 수용 게이트까지 보류한다.
- [Costume CUA Unity presentation adapter — Approved, synthetic-only verified](./specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md) · [implementation and Astra acceptance evidence](./verification/2026-09-20-costume-cua-implementation-evidence.md) · [R15 full PlayMode](./verification/2026-09-28-costume-cua-r15-full-playmode-evidence.md) · [Luna independent postreview](./verification/2026-09-28-costume-cua-r15-full-playmode-luna-postreview.md) — `AC-CUA-009: PASS`: focused PlayMode `195/195`, full EditMode `800/800`, and unfiltered full PlayMode `1142/1142`, all with zero failed/skipped/inconclusive. The slice remains synthetic-only: the 36 real Seryeong rows are pending, with no scene, live renderer, real-media import, or catalog promotion implied.
- [ADR-0033 — 세령 64px 미리보기와 32px 인게임 파생본](./adr/0033-seryeong-64px-preview-and-32px-gameplay-derivatives.md) · [결정 검토](./proposals/2026-09-20-seryeong-runtime-density-decision.md) — 사용자가 권고안 C를 승인했다. 현 640×360/18 PPU를 유지하고 64px 옷장 미리보기와 수작업 검수된 32px 양방향 인게임 아틀라스를 한 외형 패키지로 사용한다.
- [Seryeong A1/H1 first vertical-demo media slice — offline pending tooling accepted](./specs/work-contracts/2026-09-20-seryeong-a1-h1-first-media-package.md) — 첫 실제 후보를 A1/H1의 64px 옷장 정지 미리보기와 32px idle 6프레임×좌우로 제한했다. [Terra 구현 증적](./verification/2026-09-20-seryeong-a1-h1-implementation-evidence.md)의 결정론적 진단·검증 도구는 [Luna 최종 독립검수](./verification/2026-09-20-seryeong-a1-h1-luna-independent-review.md)에서 P0/P1/P2 0건으로 닫혀 Astra가 bounded offline tooling 범위만 수용했다. 프로젝트 내 사용 권한은 권리 사슬로 고정됐고 ADR-0034가 A1을 향후 초기 기본 의상으로 정했다. 다만 왼쪽 6프레임 별도 저작, 32px 수동 검수와 Unity publication 전까지 결과는 pending·non-wearable·non-addressable이며, APV3는 `0/432`, catalog는 `NoAcceptedDefault`를 유지한다.

- [SPARK-01-R2 Terra recovery — Verified](./verification/2026-09-10-spark01-r2-terra-recovery-luna-review.md) — SelfTest 실패 재현을 Terra가 회수 구현하고 Luna가 정상·실패 모드와 독립 반례를 재검증. HeaderAndGrid 사전검사 범위 최종 수용.
- [Spark 다음 작업 SPARK-01-R1](./specs/work-contracts/2026-09-09-spark01-r1-correctness-fixes.md) — 최초 구현 독립 검토에서 판정/헤더/배열 결함 발견. bounded Approved 수정계약, 원 구현 미수용. [검토 결과](./verification/2026-09-09-spark01-independent-review.md).
- [Spark 독립 구현 명세 — 스프라이트 사전검사](./specs/work-contracts/2026-09-09-spark-sprite-sheet-preflight.md) — SPARK-01 bounded Approved. 사용자 수동 전달용, 구현·실행 전.
- [관계·진엔딩·상징적 희생자 별도 보고](./proposals/2026-09-09-relationship-ending-and-tragedy-report.md) — 연애 필수, 핵심 신뢰 분기, 문오 제안, CH7 재시도. [후속 스펙](./specs/full-game-narrative/01-relationship-ending-and-retry.md)은 Review.
- [최신 8챕터 이야기·진행 재정렬](./proposals/2026-09-09-eight-chapter-synopsis-realignment.md) — Draft. 서하 4장, 무진의 초반 복선, 7장 승인 분기형 보스전, 4→6장 선택 연속성과 8장 연결. 구현 승인 아님.
- [등장 몬스터·크리처 목록](./canon/creature-roster.md) — 공중 크리처 F1~F6 등록 완료. 후속 스토리·맵 배치 인계 기준.
- [8챕터 적·맵·조작 학습 구성안](./proposals/2026-09-08-eight-chapter-enemy-map-curriculum.md) — 사용자 검토용 Draft. 첫 등장·재등장·안전 대응·서사 분기 보완, 보스 상세 설계는 다음 단계.
- [8챕터 보스 디자인 패키지](./proposals/2026-09-08-chapter-boss-design.md) — Draft. 캐릭터·스토리·공격과 시안 제작용 지시. 이미지 제작과 런타임 구현은 제외.
- [최신 보스 배정·채륜 디자인](./proposals/2026-09-08-boss-roster-unique-revision.md) — 보스 8개체, 라겐만 반복. 오르단 재전투 철회, 5장 신규 보스와 시안 인계 Draft.

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

- [M5D2 활성 런·실패 판정 코어 계약 — Verified](./specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md) · [Luna 계약 사전검토 — PASS](./verification/2026-09-11-vd05-m5d2-contract-pregate.md) · [Terra 구현·R2 실행 증적](./verification/2026-09-11-vd05-m5d2-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd05-m5d2-luna-independent-review.md) — Unity `6000.6.0f1`에서 집중 EditMode `11/11`, 전체 EditMode `494/494`, 전체 PlayMode `576/576` 통과(모두 fail/skip/inconclusive `0`); P0/P1/P2 없이 Astra 최종 수용 완료. 실제 실패 source와 M5A Unity receipt 연결은 후속 범위다.

- [M5D1 오르단 플레이어블 터미널 전환 계약 — Verified](./specs/work-contracts/2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md) · [Terra 인계](./verification/2026-09-11-vd07-m5d1-terra-handoff.md) · [Luna 독립 검토](./verification/2026-09-11-vd07-m5d1-luna-independent-review.md) — `-211` terminal requester, 실제 InputRouter·고정 카메라 저작 연결, 런타임 prefab clone 및 fail-closed 경계를 통합했다. Unity `6000.6.0f1`에서 집중 `9/9`·`22/22`·`1/1`, 전체 EditMode `483/483`, PlayMode `576/576` 통과; Luna 독립 검토와 Astra 최종 수용 완료.

- [보스전 구성 결정](./canon/boss-encounter-direction.md) — 초반 한 화면·중반 확장·최종 다단계 거대 보스·진보스 근거리 결전. OD-M5D1-001 해소, [설계 기준](./specs/boss-encounter-framework.md) 반영.

- [M5B3 실제 입력 에셋·공식 생성 래퍼 — Verified](./verification/2026-09-08-vd07-m5b3-device-action-evidence.md) — 전체 EditMode 440/440, PlayMode 435/435; 실제 InputRouter 연결은 후속 단계.
- [M5B4 입력 프레임 원자 적용 — Verified](./verification/2026-09-08-vd07-m5b4-input-frame-evidence.md) — 전체 EditMode 440/440, PlayMode 474/474.
- [M5B5 실제 입력 연결·실행 확인·보스 종료 경계 — Verified](./verification/2026-09-08-vd07-m5b5-input-router-evidence.md) — 전체 EditMode 440/440, PlayMode 539/539, 총 979개 통과. 실제 카메라·메뉴·Run 통합과 플레이 가능한 장면은 후속 단계.
- [M5C1 결정론적 카메라 추적 코어 — Verified](./verification/2026-09-08-vd07-m5c1-camera-follow-evidence.md) — 전체 EditMode474 + PlayMode539, 총1013개 통과. Unity 카메라·장면 연결은 포함하지 않는다.
- [M5C2 실제 고정 방 카메라 연결 — Verified](./verification/2026-09-08-vd07-m5c2-camera-driver-evidence.md) — 전체 EditMode480 + PlayMode567, 총1047개 통과. 완료된 이동 위치 기반 카메라·다음 틱 조준 연결과 픽셀 프로필 제작 설정 검증. 플레이 장면·GPU 시각 검수는 후속 범위.

- [2026-09-08 역할 전환 독립 검토와 보스 종료 실패 모드](./verification/2026-09-08-role-transition-and-m4b3c-pregate.md)
- [2026-09-08 작업 재개 기준 검증 — 기존 보스 저작 테스트 19/19](./verification/2026-09-08-resume-baseline.md)

- [ADR-0032 GPT Terra/Luna 서브에이전트 표준](./adr/0032-gpt-terra-luna-subagent-standard.md) — 2026-09-15 승인. 미래 구현·QA 위임은 GPT Terra/Luna만 사용하며 Ollama 및 기타 대체 모델 호출은 금지한다. 과거 기록은 사실 그대로 보존한다.
- [ADR-0027 Astra 총괄·GPT 전용 역할 재배분](./adr/0027-astra-orchestration-and-gpt-only-delivery.md) — 2026-09-08 승인. 이전 Sol 승인은 유효하며 이후 승인·에스컬레이션은 Astra, 구현은 Terra, 독립 검증은 Luna가 맡는다. 현재 외부 모델 라우팅은 ADR-0032가 명확히 한다.

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
- [ADR-0026 Qwen 제외·세 개의 Ollama 상시 작업 흐름](./adr/0026-three-lane-ollama-workstreams-without-qwen.md)
- [NAR-00 전체 게임 서사 스펙 — Review](./specs/full-game-narrative/00-spec-index.md)
- [2026-08-24 NAR-00 Luna 독립 문서 검토 — CONDITIONAL](./verification/2026-08-24-nar-00-luna-review.md)
- [BGM 방향·생성본 패키지 — 20곡 방향·20곡 청취 후보](./music/README.md)
- [MiniMax H3 티저 방향·생성본 패키지 — 3종](./video/README.md)
- [2026-08-24 NAR 유대도 수치·표시 상태 승인](./approvals/2026-08-24-nar-bond-state-approval.md)
- [2026-08-24 NAR 세령 핵심 약속 승인](./approvals/2026-08-24-nar-core-promises-approval.md)
- [2026-08-24 NAR 세령 이탈·재합류 규칙 승인](./approvals/2026-08-24-nar-heroine-departure-reentry-approval.md)
- [2026-08-24 NAR 광오 5단계·인간성 연속 보존 승인](./approvals/2026-08-24-nar-arrogance-state-approval.md)
- [2026-08-24 NAR 광오 전환·우선순위 승인](./approvals/2026-08-24-nar-arrogance-transition-approval.md)
- [2026-08-25 P1 AimArc 형상·생명주기 승인](./approvals/2026-08-25-p1-aimarc-geometry-approval.md)
- [2026-08-25 P1 수직 데모 성공 장면 흐름 승인](./approvals/2026-08-25-p1-demo-success-scene-flow-approval.md)
- [GLM 첫 구현 마일스톤 QA 보완안과 Sol 심사](./qa/2026-08-25-glm-milestone-zero-qa-supplement.md) — 역사 기록
- [UI·Art·Run Ollama 외주 패킷과 Sol 선별](./proposals/2026-09-01-ollama-ui-art-run-outsourcing.md) — 역사 기록, 현재 외주 아님
- [8챕터 실제 대화 — Sol 최종 검토본](./proposals/dialogue/2026-08-24-sol-final-eight-chapter-dialogue.md) — 역사 기록: Kimi 초안·GLM 1차 검수·Sol 최종 심사, 사용자 승인 대기
- [Kimi·GLM 검수와 Sol 수용 판단](./proposals/dialogue/2026-08-24-kimi-glm-screening-and-sol-decisions.md) — 역사 기록
- [독립 QA 마스터 플랜](./qa/qa-master-plan.md)
- [Unity Licensing Client 재연결 실패 감사 및 재발 방지 런북](./qa/unity-licensing-runbook.md)
- [Pre-Unity QA 아티팩트 작업 계약](./qa/pre-unity-qa-work-contract.md)
- [2026-08-24 VD-11 Luna 독립 검증](./verification/2026-08-24-vd-11-luna-review.md)
- [2026-08-25 수직 데모 구현 게이트 승인](./approvals/2026-08-25-vertical-demo-implementation-gate-approval.md)
- [2026-08-25 수직 데모 계약 준비도 Luna 독립 검토 — PASS](./verification/2026-08-25-vertical-demo-contract-readiness-luna-review.md)
- [2026-08-25 VD-09 Unity bootstrap 구현 증적](./verification/2026-08-25-vd09-bootstrap-implementation-evidence.md)
- [2026-08-25 VD-09 Unity bootstrap Luna 독립 검토 — PASS](./verification/2026-08-25-vd09-bootstrap-luna-review.md)
- [VD-09 Unity bootstrap 작업 계약](./specs/work-contracts/2026-08-25-vd09-unity-bootstrap.md)
- [Unity 6000.6.0f1 기준 갱신 작업 계약](./specs/work-contracts/2026-09-06-unity-6000-6-baseline-update.md)
- [Unity 6000.6.0f1 기준 갱신 검증](./verification/2026-09-06-unity-6000-6-baseline-update.md)
- [VD-01 결정론적 이동 sandbox 작업 계약](./specs/work-contracts/2026-08-25-vd01-movement-sandbox.md)
- [2026-08-26 VD-02 순수 무게 전이 M1 구현·Luna 검증 — PASS](./verification/2026-08-26-vd02-weight-transfer-m1-evidence.md)
- [2026-08-26 VD-02 Unity M2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2-contract-pregate.md)
- [2026-08-26 VD-02 Unity M2A 고정 수학·LOS 끝점 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2a-evidence.md)
- [2026-08-26 VD-02 Unity M2B1 target registry·production LOS 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2b1-evidence.md)
- [2026-08-26 VD-02 Unity M2B2A observation·movement seam 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2b2a-evidence.md)
- [2026-08-27 VD-02 Unity M2B2B session driver·owner sink 증적 — PASS](./verification/2026-08-27-vd02-weight-transfer-m2b2b-evidence.md)
- [VD-03 M1 결정론적 피해 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m1-damage-core.md)
- [2026-08-27 VD-03 M1 피해 코어 계약 Luna 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m1-contract-pregate.md)
- [2026-08-27 VD-03 M1 결정론적 피해·체력 코어 증적 — PASS](./verification/2026-08-27-vd03-combat-m1-evidence.md)
- [VD-03 M2A 결정론적 기본 공격 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m2a-basic-attack-core.md)
- [2026-08-27 VD-03 M2A 기본 공격 코어 계약 Luna 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m2a-contract-pregate.md)
- [2026-08-27 VD-03 M2A 결정론적 기본 공격 코어 증적 — PASS](./verification/2026-08-27-vd03-combat-m2a-evidence.md)
- [VD-03 M2B Unity 전투 어댑터 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m2b-unity-adapter.md)
- [2026-08-27 VD-03 M2B Unity 전투 어댑터 계약 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m2b-contract-pregate.md)
- [2026-08-27 VD-03 M2B1 Unity 전투 저작·관찰 캡처 증적 — PASS](./verification/2026-08-27-vd03-combat-m2b1-evidence.md)
- [2026-08-28 VD-03 M2B2 Unity 전투 고정 페이즈 통합 증적 — PASS](./verification/2026-08-28-vd03-combat-m2b2-evidence.md)
- [VD-03 M3A 일반 적 결정론적 반응 코어 작업 계약 — Verified](./specs/work-contracts/2026-08-28-vd03-combat-m3a-regular-enemy-reaction-core.md)
- [2026-08-28 VD-03 M3A 계약 Luna 사전 게이트 — PASS](./verification/2026-08-28-vd03-combat-m3a-contract-pregate.md)
- [2026-08-28 VD-03 M3A 일반 적 반응 코어 구현 증적 — PASS](./verification/2026-08-28-vd03-combat-m3a-evidence.md)
- [VD-03 M3B1 일반 적 반응 Unity 어댑터 작업 계약 — Verified](./specs/work-contracts/2026-08-29-vd03-combat-m3b1-reaction-unity-adapter.md)
- [2026-08-29 VD-03 M3B1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b1-contract-pregate.md)
- [2026-08-29 VD-03 M3B1 구현 증적 — PASS](./verification/2026-08-29-vd03-combat-m3b1-implementation-evidence.md)
- [VD-03 M3B2A 일반 적 행동 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-29-vd03-combat-m3b2a-regular-enemy-behavior-core.md)
- [2026-08-29 VD-03 M3B2A 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b2a-contract-pregate.md)
- [VD-03 M3B2B 일반 적 행동 Unity 브리지 작업 계약 — Approved](./specs/work-contracts/2026-08-29-vd03-combat-m3b2b-behavior-unity-bridge.md)
- [2026-08-29 VD-03 M3B2B 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b2b-contract-pregate.md)
- [2026-08-29 VD-03 M3B2B 구현 증적 — PASS](./verification/2026-08-29-vd03-combat-m3b2b-implementation-evidence.md)
- [VD-03 M3C1 결정론적 일반 적 이동 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-30-vd03-combat-m3c1-enemy-locomotion-core.md)
- [2026-08-30 VD-03 M3C1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-30-vd03-combat-m3c1-contract-pregate.md)
- [2026-08-30 VD-03 M3C1 구현 증적 — PASS](./verification/2026-08-30-vd03-combat-m3c1-implementation-evidence.md)
- [VD-03 M3C2 일반 적 이동 Unity 브리지 작업 계약 — Approved](./specs/work-contracts/2026-08-30-vd03-combat-m3c2-enemy-locomotion-unity-bridge.md)
- [2026-08-30 VD-03 M3C2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-30-vd03-combat-m3c2-contract-pregate.md)
- [2026-08-30 VD-03 M3C2 구현 증적 — PASS](./verification/2026-08-30-vd03-combat-m3c2-implementation-evidence.md)
- [VD-03 M3D1 결정론적 일반 적 위협 코어 작업 계약 — Approved (equal-X 호환성 부록)](./specs/work-contracts/2026-08-31-vd03-combat-m3d1-regular-enemy-threat-core.md)
- [2026-08-31 VD-03 M3D1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-31-vd03-combat-m3d1-contract-pregate.md)
- [2026-08-31 VD-03 M3D1 구현 증적 — PASS](./verification/2026-08-31-vd03-combat-m3d1-implementation-evidence.md)
- [VD-03 M3D2 위협 전달·Unity 브리지 작업 계약 — Approved (구현 수정 부록)](./specs/work-contracts/2026-08-31-vd03-combat-m3d2-threat-delivery-unity-bridge.md)
- [2026-08-31 VD-03 M3D2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-31-vd03-combat-m3d2-contract-pregate.md)
- [2026-09-01 VD-03 M3D2 구현 증적 — PASS](./verification/2026-09-01-vd03-combat-m3d2-implementation-evidence.md)
- [VD-03 M3E1 일반 적 조우 장면 조립·표현 handoff 작업 계약 — Approved](./specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md)
- [VD-03 M3E1 계약 사전 게이트 증적 — PASS](./verification/2026-09-01-vd03-combat-m3e1-contract-pregate.md)
- [VD-03 M3E1A 충돌 제외 타당성 증적 — FAIL / STOP](./verification/2026-09-01-vd03-combat-m3e1a-collision-feasibility.md)
- [VD-01 M2 ignored-pair Cast hit 필터 부록 — Approved](./specs/work-contracts/2026-09-01-vd01-movement-m2-ignore-pair-hit-filter-addendum.md)
- [VD-01 M2 ignored-pair Cast hit 필터 계약 사전 게이트 — PASS](./verification/2026-09-01-vd01-movement-m2-ignore-pair-filter-contract-pregate.md)
- [VD-01 M2 ignored-pair Cast hit 필터 구현 증적](./verification/2026-09-01-vd01-movement-m2-ignore-pair-filter-implementation-evidence.md)
- [VD-03 M3E1B0 일반 적 조우 작성 그래프 구현 증적 — PASS](./verification/2026-09-01-vd03-combat-m3e1b0-authored-encounter-evidence.md)
- [VD-01 Movement 작성 테스트 임시경로 격리 증적 — PASS](./verification/2026-09-02-vd01-movement-authoring-test-isolation-evidence.md)
- [VD-03 M3E1B1 lifecycle·표현 handoff 구현 증적 — PASS](./verification/2026-09-05-vd03-combat-m3e1b1-implementation-evidence.md)
- [VD-03 M4A 환수관 오르단 결정론적 보스 코어 작업 계약 — Verified](./specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md)
- [VD-03 M4A 계약 Luna 사전 게이트 — PASS](./verification/2026-09-05-vd03-combat-m4a-contract-pregate.md)
- [VD-03 M4A 환수관 오르단 보스 코어 구현 증적 — PASS](./verification/2026-09-05-vd03-combat-m4a-implementation-evidence.md)
- [VD-03 M4B1 오르단 Unity Combat 브리지 작업 계약 — Verified](./specs/work-contracts/2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md)
- [VD-03 M4B1 계약 Luna 사전 게이트 — PASS](./verification/2026-09-05-vd03-combat-m4b1-contract-pregate.md)
- [VD-03 M4B1 오르단 Unity Combat 브리지 구현 증적 — PASS / Verified](./verification/2026-09-05-vd03-combat-m4b1-implementation-evidence.md)
- [VD-03 M4B2 오르단 scripted Transfer 노출 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md)
- [VD-03 M4B2 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b2-contract-pregate.md)
- [VD-03 M4B2 오르단 scripted Transfer 노출 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b2-implementation-evidence.md)
- [VD-03 M4B3A 오르단 저작 보스 그래프 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3a-ordan-authored-graph.md)
- [VD-03 M4B3A 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b3a-contract-pregate.md)
- [VD-03 M4B3A 오르단 저작 보스 그래프 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b3a-implementation-evidence.md)
- [VD-03 M4B3B1 오르단 Audit forecast·노출 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3b1-ordan-audit-exposure.md)
- [VD-03 M4B3B1 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b3b1-contract-pregate.md)
- [VD-03 M4B3B1 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b3b1-implementation-evidence.md)
- [VD-03 M4B3B2 오르단 적대 기하·다음 틱 플레이어 피해 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3b2-ordan-hostile-geometry.md)
- [VD-03 M4B3B2 계약 Luna 사전 게이트 — PASS](./verification/2026-09-07-vd03-combat-m4b3b2-contract-pregate.md)
- [VD-03 M4B3B2 구현·Luna 독립 검증 — PASS](./verification/2026-09-07-vd03-combat-m4b3b2-implementation-evidence.md)
- [VD-03 M4B3B3 오르단 BalanceAudit 결정론적 견인 작업 계약 — Verified](./specs/work-contracts/2026-09-07-vd03-combat-m4b3b3-ordan-audit-pull.md)
- [VD-03 M4B3B3 계약 사전 게이트 — PASS](./verification/2026-09-07-vd03-combat-m4b3b3-contract-pregate.md)
- [VD-03 M4B3B3 구현 증거 — PASS / Verified](./verification/2026-09-07-vd03-combat-m4b3b3-implementation-evidence.md)
- [VD-03 M4B3C 보스 종료 정리 작업 계약 — Verified](./specs/work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)
- [VD-03 M4B3C 계약 사전 게이트](./verification/2026-09-08-vd03-combat-m4b3c-contract-pregate.md)
- [VD-03 M4B3C 상세 종료 검증 — PASS / Verified](./verification/2026-09-08-vd03-combat-m4b3c-implementation-evidence.md)
- [VD-05 M5A 보스 이후 진행 판단 코어 — Verified](./specs/work-contracts/2026-09-08-vd05-m5a-post-boss-progression-core.md)
- [VD-05 M5A 계약 사전 게이트 — PASS](./verification/2026-09-08-vd05-m5a-contract-pregate.md)
- [VD-05 M5A 구현·독립 검증 — PASS](./verification/2026-09-08-vd05-m5a-implementation-evidence.md)
- [VD-07 M5B1 이동 입력 잠금 기반 — Verified](./specs/work-contracts/2026-09-08-vd07-m5b1-movement-input-gate.md)
- [VD-07 M5B1 구현·독립 검증 — PASS](./verification/2026-09-08-vd07-m5b1-input-gate-evidence.md)
- [VD-07 M5B2 전이·보스 전투 입력 잠금 — Verified](./specs/work-contracts/2026-09-08-vd07-m5b2-transfer-combat-input-gates.md)
- [VD-07 M5B2 구현·독립 검증 — PASS](./verification/2026-09-08-vd07-m5b2-input-gate-evidence.md)
- [VD-09 M5D3 프로필 진행 상태 투영 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d3-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d3-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d3-luna-independent-review.md) — engine-free progression value projection만 검증했다. profile file·codec/hash·persistence·M5A/Unity 연결은 후속 범위다.
- [VD-09 M5D3 계약 Luna 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d3-contract-pregate.md)
- [VD-09 M5D4 프로필 설정·튜토리얼 값 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d4-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d4-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d4-luna-independent-review.md) — engine-free settings/tutorial 값만 검증했다. persisted profile·settings 적용·tutorial event·Unity 연결은 후속 범위다.
- [VD-09 M5D5 binding override RFC 8785 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d5-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d5-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d5-luna-independent-review.md) — engine-free inner JSON canonicalization만 검증했다. binding 적용·outer profile/hash·persistence·Unity 연결은 후속 범위다.
- [VD-09 M5D6 프로필 입력 블록·호환성 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d6-profile-input-compatibility-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d6-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d6-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d6-luna-independent-review.md) — input metadata compatibility만 검증했다. actual override apply·outer profile recovery·Unity 연결은 후속 범위다.
- [VD-09 M5D7A profile v1 canonical encoder — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-13-vd09-m5d7a-contract-pregate.md) · [Terra 구현·Astra 실행 증적](./verification/2026-09-13-vd09-m5d7a-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7a-luna-independent-review.md) — 집중 EditMode `8/8`, 전체 EditMode `529/529`, 전체 PlayMode `576/576` 통과(실패·건너뜀·미결정 `0`). canonical encoder와 mismatch recovery 후보 보존만 검증했으며 decoder·atomic persistence는 후속 범위다.
- [VD-09 M5D7B profile v1 canonical decoder — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7b-profile-v1-canonical-decoder.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-13-vd09-m5d7b-contract-pregate.md) · [Terra 구현·Astra 실행 증적](./verification/2026-09-13-vd09-m5d7b-implementation-evidence.md) · [Luna R2 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7b-luna-independent-review.md) — 최종 집중 EditMode `16/16`, 전체 EditMode `545/545`, 전체 PlayMode `576/576` 통과. exact canonical/hash validator와 metadata·binding input-only recovery projection을 검증했으며 file IO/atomic recovery는 후속 범위다.
- [VD-09 M5D7C profile recovery transformation core — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7c-profile-recovery-transformation-core.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7c-contract-pregate.md) · [구현·실행 증적](./verification/2026-09-13-vd09-m5d7c-implementation-evidence.md) · [Luna R3 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7c-luna-independent-review.md) — 승인 기본값과 caller-owned default/input repair/previous promotion 변환을 검증. 최종 집중 `10/10`, 전체 EditMode `555/555`, PlayMode `576/576` 통과.
- [VD-09 M5D7D atomic profile save adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7d-atomic-profile-save-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7d-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7d-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7d-luna-independent-review.md) — exact temp write-through·flush·재검증, `profile.prev.json` Windows replace/move, 접근 오류 구분, recoverable-only exception containment, ref-counted directory lease를 검증했다. Focused EditMode 29/29, full EditMode 584/584, full PlayMode 576/576.
- [VD-09 M5D7E profile load selection core — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7e-profile-load-selection-core.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7e-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7e-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7e-luna-independent-review.md) — primary→previous→default 선택, input-only repair, temp source 제외와 역할별 보존 의도를 검증했다. Focused EditMode 10/10, full EditMode 594/594, full PlayMode 576/576.
- [VD-09 M5D7F profile quarantine adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7f-profile-quarantine-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7f-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7f-implementation-evidence.md) · [Luna R2 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7f-luna-independent-review.md) — exact recovery identity/nohash proof와 19자 UTC 검증을 보완했다. 최종 집중 `18/18`, 전체 EditMode `612/612`, PlayMode `576/576` 통과.
- [VD-09 M5D7G profile launch observation adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7g-profile-launch-observation-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7g-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7g-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7g-luna-independent-review.md) — exact three-leaf 관찰, typed completion/count, 실제 read fault와 protocol fault 분리, non-atomic 경계를 검증했다. 집중 `10/10`, 전체 EditMode `622/622`, PlayMode `576/576` 통과.
- [VD-09 M5D7H primary input-recovery atomic save extension — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7h-primary-input-recovery-atomic-save.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7h-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7h-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7h-luna-postreview.md) — Primary/Decoded typed proof, fresh semantic plan, exact revision proof와 recovery-only atomic replacement를 검증했다. 실제 Input System 적용과 launch coordination은 후속 범위다.
- [VD-09 M5D7I Input System binding-override apply adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7i-input-binding-override-apply-adapter.md) · [Luna R2 pre-gate — PASS](./verification/2026-09-13-vd09-m5d7i-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7i-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7i-luna-postreview.md) — fresh disabled action candidate의 실제 override 적용과 canonical round-trip 검증을 완료했다. 복구 변환·저장·launch coordination은 후속 범위다.
- [VD-09 M5D7J binding-apply-failure recovery transform — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7j-binding-apply-failure-recovery-transform.md) · [Luna pre-gate — PASS](./verification/2026-09-13-vd09-m5d7j-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7j-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7j-luna-postreview.md) — 실제 적용 실패 뒤 primary/previous source를 중복 revision 증가 없이 current-default input 복구 문서로 바꾸는 순수 변환을 검증했다. 실제 launch 순서는 후속 범위다.
- [VD-09 M5D7K launch preservation sequence executor — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7k-launch-preservation-sequence.md) · [Luna pre-gate — PASS](./verification/2026-09-13-vd09-m5d7k-contract-pregate.md) · [구현 증적](./verification/2026-09-13-vd09-m5d7k-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7k-luna-postreview.md) — 저장 전 stale temp·비정상 primary·previous를 Temp→Primary→Previous 순서로 보존하고 실패 시 `MayPersist`를 금지하는 경계를 검증했다.
- [VD-09 M5D7L profile launch preparation coordinator — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7l-profile-launch-preparation-coordinator.md) · [Luna R2 pre-gate — PASS](./verification/2026-09-13-vd09-m5d7l-contract-pregate.md) · [구현 증적](./verification/2026-09-13-vd09-m5d7l-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7l-luna-postreview.md) — 관찰·선택·보존·격리 입력 적용·필요 복구·저장을 정확한 준비 순서로 통합하고, 입력 객체 소유권과 저장 실패 시 안전한 메모리 상태를 검증했다. 집중 `33/33`, M5D7I `8/8`, 전체 EditMode `663/663`, PlayMode `617/617` 통과.
- [VD-09 M5D7M desktop profile launch adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md) · [Luna final post-review — PASS](./verification/2026-09-13-vd09-m5d7m-luna-postreview.md) — 최종 R50 집중 117/117, M5B5 3/3, M5D7L 33/33, 전체 EditMode 663/663, 전체 PlayMode 734/734가 실패·건너뜀·미결정 없이 통과했고 Luna P0=0/P1=0 후 Astra가 수용했다.
- [VD-09 M5D7N hub-entry handoff latch — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7n-hub-entry-handoff-latch.md) · [Terra 구현·실행 증적](./verification/2026-09-13-vd09-m5d7n-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-14-vd09-m5d7n-luna-postreview.md) — M5D7M의 검증된 UIOnly 영수증과 예상 알림을 첫 Update에서 한 번만 검증·이관한다. 최종 집중 `69/69`, M5D7M 직접 회귀 `117/117`, 전체 EditMode `669/669`, 전체 PlayMode `803/803`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다.
- [VD-09 M5D7O hub-menu presentation controller — Verified](./specs/work-contracts/2026-09-14-vd09-m5d7o-hub-menu-presentation-controller.md) · [Terra 구현·실행 증적](./verification/2026-09-14-vd09-m5d7o-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-14-vd09-m5d7o-luna-postreview.md) — Primary/Previous만 프로필 이어가기로 판정하고 고정 메뉴 순서·초기 포커스·수동 알림 닫기·단일 선택 intent·640×360 UI 의미값을 장면/패키지와 분리했다. 최종 집중 `19/19`, M5D7N 직접 회귀 `69/69`, 전체 EditMode `688/688`, 전체 PlayMode `803/803`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다.
- [VD-09 M5D7P-A UI semantic frame seam — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md) · [Terra 구현·실행 증적](./verification/2026-09-20-vd09-m5d7p-a-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-20-vd09-m5d7p-a-luna-postreview.md) — 기존 InputRouter 단독 소유권을 유지하면서 UIOnly 입력을 성공 영수증에 결속된 불변 프레임으로 전달한다. 최종 집중 EditMode `5/5`, PlayMode `52/52`, M5B5 `3/3`, M5D7O `19/19`, 전체 EditMode `693/693`, PlayMode `855/855`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다. 비차단 P2 두 건은 후속 authored-presentation gate로 이관했다.
- [VD-09 M5D7P-B Unity UI package baseline — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md) · [Terra 구현 증적](./verification/2026-09-20-vd09-m5d7p-b-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-20-vd09-m5d7p-b-luna-postreview.md) — Unity 6000.6.0f1 내장 uGUI 2.6.0만 직접 선언하고 지원 종료된 TextMeshPro shim은 추가하지 않았다. 동결 기준선의 두 SHA와 정확한 resolver 차분을 독립 재검산했고, 전체 EditMode `688/688`, PlayMode `805/805`가 실패·건너뜀·미결정 없이 통과했다. Luna `P0=0`/`P1=0`/`P2=0` 후 Astra가 수용했다.
- [VD-09 M5D7Q0 Hub-UIOnly router authoring graph — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md) · [Terra 구현 증적](./verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md) · [Luna 최종 독립검증 — ACCEPT](./verification/2026-09-23-vd09-m5d7q0-luna-independent-review.md) — 허브 전용 무게임플레이 라우터 그래프와 결정론적 정규 프리팹 복구를 완성했다. 빌더 독립 프로세스 `31/31`, 최종 범위 감사 `4/4`, 전체 EditMode `737/737`, 영향 검토로 재사용 승인된 전체 PlayMode `943/943`가 실패·건너뜀·미결정 없이 통과했고, Luna `P0=0`/`P1=0`/`P2=0` 후 Astra가 수용했다.
- [VD-09 M5D7Q-A authored uGUI hub shell — Verified](./specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md) · [Terra implementation evidence](./verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md) · [Luna final postreview — PASS](./verification/2026-09-27-vd09-m5d7q-a-final-luna-postreview.md) · [retry-r visual review — PASS](./verification/2026-09-27-vd09-m5d7q-a-resolution-capture-retry-r-visual-review.md) — Astra integrated the final package after independent Luna P0/P1/P2=0 review. The final record preserves the Approved-at-execution SHA and the separate fresh-process capture-equality proof.

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
2. 현재 ADR과 캐논이 충돌하면 구현을 멈추고 Astra가 둘을 함께 정정한다.
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

- Astra만 스펙을 `Approved`로 전환하고 통합을 승인한다.
- Terra는 Approved 계약 안에서 단위 파트를 설계·구현한다.
- Luna는 AC ID에 따라 독립 검수하고 `Verified` 증적을 남긴다.
- 과거 외부 Ollama 산출물은 현재 작업 흐름에 사용하지 않는다. 새 단위는 GPT Terra 구현과 GPT Luna 독립 검증을 거치며 Astra 승인 전에는 상태를 전환하지 못한다.
