# 최소 컨텍스트 서사 감사 실행 기록

- 2026-09-09 Astra. Advisory review only; 게임/런타임/AC 통과 아님.
- 사용자 재허용 ADR-0031. localhost Ollama의 기존 등록 모델 확인 뒤 각 한 번 POST /api/generate, think=false, num_predict420, timeout50초. 자동 재시도/예약 없음. 새 과금 API 없음.
- 전체 저장소/대화·경로·자격증명·개인정보 전송 없음. 익명화된 규칙 두 개만 영문으로 전달, 각각 4개 이하/220단어 이하 반례 요청. 스토리 이름도 제외.

## 실제 결과와 Astra 수용

| 요청 모델 | 반환 모델 | 결과 | 출력 토큰(eval_count) | 반영 |
|---|---|---|---|---|
| glm-5.2:cloud | glm-5.2 | done=true, stop, 비어 있지 않음 | 237 | 중간 선택 원자성, 중요 신뢰 비가역, 일상 갈등 회복, 허용 공명 반례 |
| kimi-k3:cloud | kimi-k3 | done=true, stop, 비어 있지 않음 | 292 | 소망 선행 복선/미완, 손상 지속, 죽음과 보상·실패 분리, 다른 인물 각인 중복 금지 |

모델 제안의 모든 전제를 채택하지 않았다. GLM의 조건 충족 시 true ending reached는 진보스 입장 자격으로 수정하며 실제 승리를 별도로 요구한다. Kimi의 death unrelated to plot progression은 사망 사건의 챕터 배치를 부정하므로 반려한다. damage worsens linearly와 상처를 다루는 장면 전체를 exactly once로 제한하는 내용도 불필요/과도하여 반려하고 원상복구 및 동일 각인 재수탈 보상 금지만 채택했다.

외주가 테스트를 실행하거나 프로젝트 전체를 독립 감사했다고 주장하지 않는다. 단발 통신에서 quota/429/인증 오류는 발생하지 않았으나 잔여 한도나 장기 가용성을 확인한 것은 아니다. 숫자는 Ollama가 보고한 출력량이며 GPT 절감량을 측정한 값이 아니다.

## Spark와 GPT 사용 경계

OpenAI Docs 스킬에 따라 [공식 모델 문서](https://learn.chatgpt.com/docs/models)를 열었으나 이 세션 도구의 Spark 호출 가능성을 보장하는 근거는 아니다. 현재 subagent 모델 enum에 Spark가 없어 미사용. 별도 API·가짜 모델 ID 우회 없음. Luna에 신규 관계/저장 문서만 한정한 검토를 요청했다.

Luna 독립 문서 검토 완료: 확정된 실질 모순 0건. 중요 신뢰 이벤트/예고/고백 경계와 문오 사망·마지막 대화/유품의 경로별 상태표는 P1 계약 공백으로 보고됐다. Astra는 이를 Review 유지 사유로 수용했다. AC-NREL-002/005 실행/통과를 선언하지 않는다.
