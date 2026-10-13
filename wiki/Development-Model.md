# Development Model

## 현재 운영 — 2026-10-07

사용자 정정에 따라 테라 역할과 테라 이름의 작업자 배정을 제거했다. 아스트라 `gpt-6-astra`가 총괄·계약 승인·최종 통합, 솔 `gpt-6.1-sol`이 설계·구현, 루나 `gpt-6-luna`가 독립 검증을 맡는다. 구현자가 자기 작업을 독립 수용하지 않는다. 실제 가용 상위 안정 버전의 같은 계열 자동 적용과 기존 제품·검사·Q0 승인 관문은 유지한다.

현재 권위는 [운영 문서](../docs/agent-operating-model.md)와 [ADR-0043](../docs/adr/0043-remove-terra-role.md)이다. 이 문서는 로컬 탐색 보기이며 아래 이전 배정·실행은 당시 이력이다.

매시간 모델 기준 자동갱신 예약은 같은 날 사용자 요청으로 삭제했다. 새 작업 배정 직전의 실제 가용 목록 확인은 유지한다.

## 2026-09-29 GPT-6 전환

사용자 지시로 모든 새 작업과 재시도에서 5.6 호출을 금지한다. 아스트라는 `gpt-6-astra`, 솔은 `gpt-6-sol`, 루나는 `gpt-6-luna`를 사용한다. 현재 도구에 `gpt-6-terra`가 없어 테라 구현 역할은 별도 `gpt-6-sol` 작업자로 수행한다. 실제 구현·설계·독립 검수 작업자를 새로 생성했고 응답을 확인했다. 역할별 소유권과 독립 검수는 유지하며, 과거 모델 참여 기록은 보존한다. 이 변경으로 사용량 제한이 해소됐다고 보장하지 않는다.

## 2026-09-15 우선 변경

Astra 핵심 판단·Terra 구현·Luna 독립 검증을 유지한다. 미래 프로젝트 작업은 GPT 모델만 사용한다. Spark는 실제 도구에 노출될 때만 검증에 사용하며, 현재 미노출이면 Luna가 담당한다. Ollama, Kimi, GLM, MiniMax, Qwen 호출·탐색·재시도·예약 감시와 새 유료 API는 사용하지 않는다. 게시본은 권위 문서의 탐색용 보기이며 승인·수용 상태를 변경하지 않는다.

## 과거 운영 — 2026-09-08

사용자 승인으로 `Astra 계약·최종 판단 → Terra 구현 → Luna 독립 검증 → Astra 통합`으로 변경했다. Sol은 복잡한 설계·계약 초안·반대 검토를 지원한다.

과거 Ollama 외주와 대체 모델 배정은 모두 종료했다. 기존 산출물과 기록은 역사적 사실로 보존하며 현재 실행하지 않는다. 새 작업은 GPT Terra가 구현·도구를, GPT Luna가 독립 QA·회귀 검증을 담당하고 Astra가 최종 통합한다.

Pro는 실제 실행 지원이 확인될 때만 사용으로 기록한다. 현재 서브에이전트 도구의 모델·추론 강도 지정은 Pro 모드 활성화가 아니다.

모든 구현 작업은 Astra가 승인한 스펙, REQ ID, AC ID, 허용 파일, 금지 영역, 롤백 지점을 포함한 작업 계약을 가져야 한다. 역사적 역할 배정과 승인 기록은 해당 ADR과 검증 증적에서 확인한다.

**권위 문서:** [ADR-0037](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/adr/0037-gpt6-role-model-migration.md), [ADR-0027](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/adr/0027-astra-orchestration-and-gpt-only-delivery.md), [에이전트 운영 규칙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/AGENTS.md), [멀티에이전트 운영 모델](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/agent-operating-model.md), [작업 계약 템플릿](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/templates/work-contract-template.md)
