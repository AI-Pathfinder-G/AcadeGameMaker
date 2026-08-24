# Specifications

스펙 상태는 `Draft → Review → Approved → Implemented → Verified` 순서다. Sol이 Approved로 전환하기 전에는 구현을 시작하지 않는다.

| Area | Spec | Current status |
|---|---|---|
| Scope | [VD-00](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/00-spec-index.md) | Review |
| Movement | [VD-01](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/01-player-movement.md) | Review |
| Weight transfer | [VD-02](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/02-weight-transfer.md) | Review |
| Combat | [VD-03](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/03-combat-and-enemies.md) | Review |
| Rooms/expedition | [VD-04](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/04-authored-rooms-and-expedition.md) | Review |
| Failure/persistence | [VD-05](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/05-failure-and-persistence.md) | Review |
| Choice/narrative | [VD-06](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/06-humanity-choice-and-narrative.md) | Review |
| Input/UI | [VD-07](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/07-input-ui-and-feedback.md) | Review |
| Art/assets | [VD-08](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/08-art-and-asset-integration.md) | Review |
| Platform/quality | [VD-09](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/09-platform-and-quality.md) | Draft |
| Verification | [VD-10](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/10-verification-script.md) | Review |
| Pre-Unity QA infrastructure | [VD-11](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/11-pre-unity-qa-artifacts.md) | Verified |

**권위 문서:** [스펙 운영 규칙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/README.md), [열린 결정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/OPEN-DECISIONS.md)

P0 여섯 항목과 P1 이동·플랫폼·미술 계약은 해결됐다. `Approved` 전환에는 남은 `OD-SCENE-001`, 소비 스펙 간 계약 일치, Luna 독립 검토가 필요하다.

플랫폼 P1 입력·재지정·저장·복구 계약은 해결됐고 Luna 독립 문서 검토를 PASS했다. version 1 단일 `profile.json`을 원자 저장하며 시작 시 valid primary→valid previous→revision 0 default만 사용한다. stale temp와 손상·미지원 파일은 로드하지 않고 보존하며 binding만 불일치하면 진행 상태를 유지한 채 입력만 기본값으로 복구한다.

미술 P1은 18 PPU, 640×360 fixed world/UI frame, predictive camera, pixel UI·SDF text, exact 32색 palette, logical pixel outline, URP 2D lighting과 9×9/13×13px reticle exact values까지 확정돼 `OD-ART-001`이 해결됐다.

VD-11은 GLM의 독립 QA 계획 초안, MiniMax M3의 부분 구현안, GPT 계약 교정과 Luna 독립 검증을 거쳐 Verified가 됐다. 현재 pre-Unity catalog는 UI·menu부터 gameplay·저장·render·build·E2E까지 13개 scenario로 VD-00~09의 68개 AC를 전수 연결한다. 실제 Unity EditMode·PlayMode·Windows build test는 각 기능 스펙 승인과 Unity 프로젝트 생성 뒤 구현한다.
