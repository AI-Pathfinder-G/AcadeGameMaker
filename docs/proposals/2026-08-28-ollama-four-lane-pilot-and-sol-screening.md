# Ollama 네 작업 흐름 파일럿과 Sol 선별

- Date: 2026-08-28
- Status: screened non-authoritative proposal
- Decision authority: none; Sol screening only
- Models: `qwen3.8:latest`, `kimi-k3:cloud`, `glm-5.2:cloud`, `minimax-m3:cloud`

> Historical note: ADR-0026 excludes Qwen from all future project work. The Qwen row below records the superseded ADR-0025 pilot only and grants no continuing assignment.

ADR-0025를 형식적인 역할 선언으로 끝내지 않기 위해 네 모델에 서로 다른 비민감 운영 과제를 배정했다. Cloud 모델에는 저장소 원문, 로컬 경로, 자격 증명 또는 개인 정보를 제공하지 않았고 Kimi는 `think=false`로 최종 본문을 강제했다.

## 선별 결과

| 작업 흐름 | 배정 | Sol 판정 | 수용 | 반려·교정 |
|---|---|---|---|---|
| Qwen 로컬 분석 | 마일스톤 영향 보고 필드와 자동 점검 제안 | 부분 수용 | 변경 범위, 충돌 지점, 실패 로그, 회귀 위험, 승인 상태 | 측정 도구가 없는 성능 변화율·coverage 수치를 의무화하지 않는다. 일반 CVE·Conventional Commits 제안은 현재 계약보다 우선하지 않는다. |
| Kimi 구현 실험 | 추상 Unity 작업명세의 최종 출력 envelope | 부분 수용 | assumptions, logical files, code draft, tests, deviations, honest status 구획 | 임의 `public` 메서드, `AddComponentMenu`, Unity 2021.3 호환 가정과 샘플 placeholder assertion은 폐기한다. 실제 packet은 Approved 계약의 public surface와 REQ·AC만 따른다. |
| GLM 적대적 QA | 다단계 파이프라인 실패 모드와 탐지 게이트 | 부분 수용 | spec drift, hallucinated dependency/API, summary-to-implementation mismatch, validator false negative와 tool self-error | TF-IDF 점수, 자동 테스트 생성, SAST/DAST를 보편 게이트로 강제하지 않는다. 프로젝트의 exact contract·diff·AC 검증을 사용한다. |
| MiniMax 검증 도구 | 결정론적 utilization ledger JSON schema와 validator | 부분 수용 | 네 모델의 상태 enum, REQ·AC 연결, stable ordering, runtime API 비변경 validator 방향 | `generatedAtUtc`는 byte-determinism을 깨므로 canonical report에서 제거한다. 임의 `Assets/Editor/Utilization` 경로는 Sol 계약 전 고정하지 않는다. |

## 운영에 반영한 규칙

1. Qwen 보고는 실제로 측정 가능한 저장소·diff·test result 사실만 기록한다.
2. Kimi packet은 `think=false`와 명시적 final envelope를 사용하고, 모든 컴파일·테스트 상태를 `unverified`로 표시한다.
3. GLM의 공격 검토는 일반 점수가 아니라 frozen contract, REQ, AC, public surface와 failure atomicity에 대조한다.
4. MiniMax utilization report는 timestamp가 없는 canonical body를 ordinal stable order로 만들고, 실행 시각은 비정규 메타데이터로 분리한다.
5. 어느 출력도 Terra/Luna의 검토와 Sol의 수용 전에는 구현·검증·캐논 상태를 바꾸지 않는다.

이 파일럿은 모델 연결과 역할의 실효성을 확인한 기록이다. 실제 validator 구현은 해당 Approved 작업 계약이 열릴 때 MiniMax가 다시 제안하고 Terra가 구현·통합하며 Luna가 독립 검수한다.
