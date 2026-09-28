---
status: superseded by ADR-0026
supersedes: ADR-0023
---

# 모든 구현 마일스톤에 네 개의 Ollama 작업 흐름을 배치한다

- Date: 2026-08-28
- Decision owner: Sol
- Approved by: user

Ollama 모델을 단순 보조 인력이나 완성 후 QA에만 두면 실제 개발 중 산출물이 생기지 않는다. 따라서 모든 코드가 있는 마일스톤은 기본적으로 `Qwen 로컬 분석 → Kimi 구현 실험 → GLM 적대적 QA → MiniMax 검증 도구·픽스처` 네 작업 흐름을 거친다. 각 산출물은 승인 계약의 REQ·AC와 연결된 비권위 초안이며 Terra 또는 Luna가 선별하고 Sol만 통합을 승인한다.

Kimi는 저장소 원문 없이 추상 작업명세만 받아 격리된 C#·테스트 초안을 만든다. Qwen은 로컬 저장소를 읽어 영향 범위, 반복 작업, 로그와 데이터 변환을 담당한다. GLM은 구현 전 실패 모드와 구현 후 누락 시나리오를 각각 공격적으로 검토한다. MiniMax는 게임 완성 전에도 public runtime 계약을 건드리지 않는 테스트 픽스처, replay·fixture generator, validator, QA·Editor 도구 초안을 구현할 수 있고, 완성 후에는 독립 QA 프로그램 구현으로 범위를 넓힌다.

## Consequences

- 작업 계약마다 네 Ollama 흐름의 배정 결과 또는 구체적인 비적용 사유를 기록한다.
- Ollama 산출물이 없거나 사용할 수 없으면 GPT가 작업을 대체하되, 호출 실패·반려·대체 사실을 증적에 남긴다.
- 처리량을 늘리기 위해 역할은 확대하지만 architecture, public API, canon, acceptance, merge와 최종 통합 권한은 계속 GPT에만 있다.
- Cloud 모델에는 비밀·자격 증명·개인 정보·로컬 경로·저장소 원문을 전달하지 않는다. 로컬 전체 문맥 작업은 Qwen만 맡는다.
