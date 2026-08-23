---
status: superseded by ADR-0015
---

# GPT가 핵심을 소유하고 Ollama는 비핵심 작업을 위임받는다

중요한 설계·아키텍처·핵심 게임플레이·서사 캐논·통합은 GPT-5.6 Terra가 소유하고, GPT-5.6 Luna는 범위가 확정된 구현과 테스트 및 독립 검수를 맡는다. Ollama는 GLM 5.2 Cloud, MiniMax M3 Cloud, 로컬 Qwen3.8만 사용하며 초안·분류·탐색·반복 작업을 수행하되 결정권과 통합 권한은 갖지 않는다. 모든 Ollama 산출물은 GPT 검토를 통과해야 프로젝트나 캐논 문서에 반영된다.
