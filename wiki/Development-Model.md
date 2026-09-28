# Development Model

## 2026-09-15 우선 변경

Astra 핵심 판단·Terra 구현·Luna 독립 검증을 유지한다. 미래 프로젝트 작업은 GPT 모델만 사용한다. Spark는 실제 도구에 노출될 때만 검증에 사용하며, 현재 미노출이면 Luna가 담당한다. Ollama, Kimi, GLM, MiniMax, Qwen 호출·탐색·재시도·예약 감시와 새 유료 API는 사용하지 않는다. 게시본은 권위 문서의 탐색용 보기이며 승인·수용 상태를 변경하지 않는다.

## 현재 운영 — 2026-09-08

사용자 승인으로 `Astra 계약·최종 판단 → Terra 구현 → Luna 독립 검증 → Astra 통합`으로 변경했다. Sol은 복잡한 설계·계약 초안·반대 검토를 지원한다.

과거 Ollama 외주와 대체 모델 배정은 모두 종료했다. 기존 산출물과 기록은 역사적 사실로 보존하며 현재 실행하지 않는다. 새 작업은 GPT Terra가 구현·도구를, GPT Luna가 독립 QA·회귀 검증을 담당하고 Astra가 최종 통합한다.

Pro는 실제 실행 지원이 확인될 때만 사용으로 기록한다. 현재 서브에이전트 도구의 모델·추론 강도 지정은 Pro 모드 활성화가 아니다.

모든 구현 작업은 Astra가 승인한 스펙, REQ ID, AC ID, 허용 파일, 금지 영역, 롤백 지점을 포함한 작업 계약을 가져야 한다. 역사적 역할 배정과 승인 기록은 해당 ADR과 검증 증적에서 확인한다.

**권위 문서:** [ADR-0027](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/adr/0027-astra-orchestration-and-gpt-only-delivery.md), [에이전트 운영 규칙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/AGENTS.md), [멀티에이전트 운영 모델](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/agent-operating-model.md), [작업 계약 템플릿](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/specs/templates/work-contract-template.md)
