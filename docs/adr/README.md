# Architecture Decision Records

ADR은 쉽게 되돌리기 어렵고, 맥락 없이 보면 의외이며, 실제 대안을 비교해 선택한 결정의 이유를 보존한다. 상태가 명시되지 않은 기존 ADR은 accepted로 취급한다.

| ADR | 상태 | 결정 |
|---|---|---|
| [0035](./0035-new-game-manual-only-prior-profile.md) | accepted product direction; implementation pending | 새 게임 전 진행은 수동 복구 전용으로 보존하고 자동 복구 후보에서 제외 |
| [0034](./0034-seryeong-a1-initial-default-costume.md) | accepted | 세령 수직 데모의 초기 기본 의상을 A1로 지정하되 미디어·임포트 수용 전에는 지급하지 않음 |
| [0032](./0032-gpt-terra-luna-subagent-standard.md) | accepted | 미래 작업은 GPT Terra 구현·GPT Luna 독립 QA로 표준화; Ollama 및 기타 대체 모델 호출 금지 |
| [0031](./0031-bounded-external-audit-and-spark-preference.md) | accepted direction, superseded by 0032 for active routing | 최소 컨텍스트 Kimi·GLM 주변 감사 재허용이라는 과거 예외 |
| [0030](./0030-romance-ending-tragedy-and-ch7-retry.md) | accepted direction | 연애 필수 진엔딩·상징적 필연 희생·CH7 동일 분기 재시도·최신 장 배치 |
| [0033](./0033-seryeong-64px-preview-and-32px-gameplay-derivatives.md) | accepted | 64px 옷장 미리보기·제작 마스터와 수작업 검수된 32px 인게임 파생본을 한 의상 패키지로 사용 |
| [0001](./0001-metered-gravity-city.md) | accepted | 중력을 계량·과금하는 도시 |
| [0002](./0002-human-counterweight-protagonist.md) | accepted | 주인공은 인간 대리추 |
| [0003](./0003-humanity-is-preserving-others-agency.md) | accepted | 인간성은 타인의 주체성 보존 |
| [0004](./0004-heroine-is-a-conditional-human-anchor.md) | accepted | 히로인은 조건 있는 인간의 닻 |
| [0005](./0005-choices-grant-different-skill-families.md) | accepted | 선택별 성장 기술군 |
| [0006](./0006-protagonist-designed-consensual-gravity-resonance.md) | accepted | 주인공은 공명의 최초 설계자 |
| [0007](./0007-weight-transfer-is-the-core-player-verb.md) | accepted | 무게 전이는 핵심 플레이 동사 |
| [0008](./0008-assemble-authored-expedition-rooms.md) | accepted | 검증된 수작업 방 재조립 |
| [0009](./0009-godot-for-the-first-playable.md) | superseded by 0010 | Godot 첫 선택 |
| [0010](./0010-unity-for-the-asset-first-prototype.md) | accepted | Unity 6.3 LTS, C#, URP 2D |
| [0011](./0011-high-resolution-pixel-art.md) | accepted | 고해상도 픽셀 아트와 2D 조명 |
| [0012](./0012-bureaucratic-dieselpunk-art-direction.md) | accepted | 관료주의 디젤펑크 산업도시 |
| [0013](./0013-fifteen-minute-vertical-slice.md) | accepted | 15분 수직 데모 범위 |
| [0014](./0014-gpt-governs-ollama-delegates-routine-work.md) | superseded by 0015 | GPT/Ollama 역할의 첫 분리 |
| [0015](./0015-sol-orchestrates-terra-builds-units.md) | superseded by 0023 | Sol 오케스트레이션, Terra 단위 개발 |
| [0016](./0016-document-authority-and-wiki-publishing.md) | accepted | 권위 문서와 위키 발행 계층 분리 |
| [0017](./0017-quarantine-assets-before-import.md) | accepted | 외부 에셋을 증적과 함께 격리 후 임포트 |
| [0018](./0018-vertical-demo-p0-integration.md) | accepted | 공동장부 원정으로 수직 데모 P0 통합 |
| [0019](./0019-pointer-aimed-sidescroller-controls.md) | accepted | 포인터 조준형 횡스크롤 조작 |
| [0020](./0020-heroine-bond-sidekick-and-true-ending.md) | accepted | 히로인 유대·자율 지원·진엔딩 관계 축 |
| [0021](./0021-character-aim-arc-and-charged-ballistics.md) | accepted | 캐릭터 중심 AimArc와 차지 탄도 |
| [0022](./0022-character-first-eight-chapter-narrative.md) | accepted | 인물 중심 8챕터와 자기정당화형 광오 |
| [0023](./0023-terra-kimi-implementation-and-qa-review-pipeline.md) | superseded by 0025 | Terra·Kimi 구현, Luna 독립 검수, GLM·MiniMax QA 파이프라인 |
| [0024](./0024-deterministic-player-motor-owns-gravity.md) | accepted | 결정론적 플레이어 모터가 실제 중력과 이동 상태를 소유 |
| [0025](./0025-four-lane-ollama-workstreams.md) | superseded by 0026 | Qwen·Kimi·GLM·MiniMax 네 작업 흐름을 개발 전 과정에 배치 |
| [0026](./0026-three-lane-ollama-workstreams-without-qwen.md) | superseded by 0027 | Qwen 제외, Kimi·GLM·MiniMax 세 작업 흐름으로 재배분 |
| [0027](./0027-astra-orchestration-and-gpt-only-delivery.md) | accepted, routing clarified by 0032 | Astra 총괄·Sol 설계 지원·Terra 구현·Luna QA, Ollama 외주 회수 |
| [0028](./0028-boss-arena-escalation-and-final-duel.md) | accepted | 초반 한 화면·중반 확장·최종 다단계 거대 보스·진보스 근거리 결전 |
| [0029](./0029-seoha-rivalry-and-secret-shop.md) | accepted direction | 서하의 회유·세령의 감정 자각·관계별 비밀상점과 미확인 무기 |
