---
status: superseded by ADR-0027
supersedes: ADR-0025
---

# Qwen을 제외하고 Kimi·GLM·MiniMax 세 작업 흐름으로 운영한다

- Date: 2026-08-28
- Decision owner: Sol
- Approved by: user

Qwen3.8을 허용 모델과 프로젝트 배정에서 제외한다. 코드가 있는 마일스톤의 Ollama 작업 흐름은 `Kimi 구현·테스트 실험 → GLM 구현 전후 적대적 QA → MiniMax 검증 도구·픽스처` 세 갈래로 재편한다. Kimi는 저장소 원문 없이 추상 작업명세만 받고, GLM과 MiniMax도 redacted 비민감 요약만 받는다. Qwen이 맡았던 로컬 저장소 영향 분석은 Terra가, 로컬 테스트 로그·결과 정리는 Luna가 회수한다.

## Consequences

- 작업 계약과 검증 증적은 Kimi·GLM·MiniMax 각각의 `used and accepted`, `used and rejected`, `failed and replaced`, `not applicable` 결과를 기록한다.
- Kimi는 모든 코드 단위에서 bounded C# 또는 test draft를 제출하고 Terra가 재작성·통합한다.
- GLM은 구현 전 failure-mode pass와 구현 후 scenario-gap pass를 수행한다.
- MiniMax는 개발 중 fixture, replay/scenario generator, validator, QA harness, safe Editor-tool 제안을 담당하고 기능 완성 뒤 전체 QA 프로그램으로 확장한다.
- 로컬 경로, 저장소 원문, 원시 로그와 비밀 자료는 Cloud Ollama 모델에 전달하지 않는다. GPT가 로컬 문맥과 최종 판단을 소유한다.
