# Specifications

스펙 상태는 `Draft → Review → Approved → Implemented → Verified` 순서다. Astra가 Approved로 전환하기 전에는 구현을 시작하지 않는다. 복잡한 설계 검토가 필요할 때만 Sol이 보조하고, 구현은 GPT Terra, 독립 검증은 GPT Luna가 담당한다.

| Area | Spec | Current status |
|---|---|---|
| Scope | [VD-00](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/00-spec-index.md) | Approved |
| Movement | [VD-01](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/01-player-movement.md) | Approved |
| Weight transfer | [VD-02](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/02-weight-transfer.md) | Approved |
| Combat | [VD-03](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/03-combat-and-enemies.md) | Approved |
| Rooms/expedition | [VD-04](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/04-authored-rooms-and-expedition.md) | Approved |
| Failure/persistence | [VD-05](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/05-failure-and-persistence.md) | Approved |
| Choice/narrative | [VD-06](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/06-humanity-choice-and-narrative.md) | Approved |
| Input/UI | [VD-07](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/07-input-ui-and-feedback.md) | Approved |
| Art/assets | [VD-08](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/08-art-and-asset-integration.md) | Approved |
| Platform/quality | [VD-09](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/09-platform-and-quality.md) | Approved |
| Verification | [VD-10](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/10-verification-script.md) | Approved |
| Pre-Unity QA infrastructure | [VD-11](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/11-pre-unity-qa-artifacts.md) | Verified |
| Full-game narrative | [NAR-00](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/full-game-narrative/00-spec-index.md) | Review |

**권위 문서:** [스펙 운영 규칙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/README.md), [열린 결정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/OPEN-DECISIONS.md)

P0 여섯 항목과 P1 이동·플랫폼·미술·AimArc·성공 장면 흐름 계약은 모두 해결됐다. Luna의 최종 계약 검토가 PASS했고 Sol이 2026-08-25 수직 데모 구현 게이트를 승인했다. 구현은 승인된 작업 계약 단위로만 진행한다.

플랫폼 P1 입력·재지정·저장·복구 계약은 해결됐고 Luna 독립 문서 검토를 PASS했다. version 1 단일 `profile.json`을 원자 저장하며 시작 시 valid primary→valid previous→revision 0 default만 사용한다. stale temp와 손상·미지원 파일은 로드하지 않고 보존하며 binding만 불일치하면 진행 상태를 유지한 채 입력만 기본값으로 복구한다.

미술 P1은 18 PPU, 640×360 fixed world/UI frame, predictive camera, pixel UI·SDF text, exact 32색 palette, logical pixel outline, URP 2D lighting과 9×9/13×13px reticle exact values까지 확정돼 `OD-ART-001`이 해결됐다.

VD-11은 과거 GLM의 독립 QA 계획 초안과 MiniMax M3의 부분 구현안을 참고했으나, GPT의 계약 교정과 Luna 독립 검증을 거쳐 Verified가 됐다. 이 외부 모델 참조는 역사 기록이며 현재 호출하지 않는다. 현재 pre-Unity catalog는 UI·menu부터 gameplay·저장·render·build·E2E까지 13개 scenario로 VD-00~09의 68개 AC를 전수 연결한다. 실제 Unity EditMode·PlayMode·Windows build test는 각 기능 스펙 승인과 Unity 프로젝트 생성 뒤 구현한다.

NAR-00은 승인된 8챕터 시놉시스를 관찰 가능한 요구사항과 인수 기준으로 전환한 `Review` 스펙이다. OD-NAR-001의 유대도·핵심 약속·이탈·재합류 규칙은 해결됐다. 남은 P0은 광오 상태 전환·우선순위·저장 규칙이며, 전체 챕터 구현은 아직 시작하지 않는다.

Luna 독립 문서 검토에서 REQ–AC 추적성과 캐논·ADR·VD-06 정합성은 PASS했다. 판정은 미결 승인 차단 항목이 남아 있는 `CONDITIONAL`이며 구현 권한을 제공하지 않는다.
