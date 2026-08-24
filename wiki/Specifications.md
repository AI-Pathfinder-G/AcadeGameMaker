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

플랫폼 P1에서는 입력·재지정 계약이 확정됐다. 영구 상태는 version 1 단일 `profile.json`에 설정·binding override·tutorial·확정 progression만 canonical JSON과 integrity hash로 기록한다. persistentDataPath의 temp를 기록·flush·재검증한 뒤에만 primary를 원자 교체하고 직전 정상본은 `profile.prev.json`으로 보존한다. 시작 시 파일 선택과 손상 복구 순서는 아직 열려 있다.
