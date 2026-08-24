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

**권위 문서:** [스펙 운영 규칙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/README.md), [열린 결정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/OPEN-DECISIONS.md)

P0 여섯 항목과 P1 이동 계약은 해결됐다. `Approved` 전환에는 남은 P1 세 항목, 소비 스펙 간 계약 일치, Luna 독립 검토가 필요하다.

플랫폼 P1에서는 Input System 1.20.0 단독, 생성 C# wrapper, 단일 InputRouter와 Gameplay/UI 두 map, 기본 UI binding·focus navigation, 포인터 조준 역할 배치와 mouse/gamepad 타겟 판정·포착 피드백이 부분 확정됐다. 리바인딩과 저장 계약은 아직 열려 있다.
