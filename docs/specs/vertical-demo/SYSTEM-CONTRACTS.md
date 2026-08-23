# Vertical Demo System Contracts

- Status: Draft
- Owner and approval: Sol
- Consumers: VD-01 through VD-10
- Last updated: 2026-08-24

이 문서는 단위 시스템이 공유하는 상태·사건·생명주기의 유일한 소유자를 정한다. 구체적인 C# API와 직렬화 필드는 P0/P1 결정 후 Sol이 승인한다.

## State ownership

| State | Sole owner | Read-only consumers | Must not own |
|---|---|---|---|
| Player motion and grounded state | VD-01 Movement | Weight transfer, combat, UI | Run result, choice |
| Active transfer and target weight modifier | VD-02 Weight transfer | Movement, combat, room hazards, UI | Target health, room order |
| Health, damage, enemy life state | VD-03 Combat | Run, UI | Transfer session, persistent choice |
| Seed, room plan, current room, run phase | VD-04/VD-05 Expedition | Combat, UI, verification | Player physics, skill effect |
| Confirmed choice and granted choice skill | VD-06 Choice | Combat, movement, UI, persistence | Run phase, health |
| Versioned persistent snapshot | VD-09 Platform/persistence adapter | VD-05 run restore | Live scene objects, active transfer |
| Presentation state | VD-07 UI | none | Authoritative gameplay state |

어떤 상태도 두 시스템이 함께 쓰지 않는다. 소비자는 원본을 수정하지 않고 명시된 명령 또는 사건을 사용한다.

## Lifecycle order

1. `RunStarted`: 시드와 유효한 방 계획이 확정된 뒤 방을 연다.
2. `RoomEntered`: 방 객체가 준비된 뒤 이동·전투 입력을 연다.
3. `RoomLeaving`: 입력을 잠그고 무게 전이를 정리한 뒤 방 객체를 제거한다.
4. `RunEnded(Succeeded|Failed)`: 종료 결과를 한 번 확정하고 추가 전투·보상 변경을 막는다.
5. `PersistentSnapshotRequested`: OD-RUN-001에서 승인한 보존 상태만 스냅샷한다.
6. `ReturnedToHub`: 이전 방·적·전이 참조가 없는 상태에서 거점 입력을 연다.

중복 종료 사건은 첫 유효 사건만 적용한다. 순서가 어긋나면 자동 복구를 추측하지 않고 진단 가능한 계약 오류로 처리한다.

## Cross-system invariants

- 방 객체 제거 전에 활성 무게 전이를 정리한다.
- 사망·실패 확정 후에는 피해, 보상, 선택, 기술 해금을 새로 적용하지 않는다.
- UI는 권위 상태를 읽고 표현할 뿐 게임플레이 상태를 직접 변경하지 않는다.
- 저장에는 활성 물리 객체, 활성 전이 참조, 런타임 컴포넌트 참조를 넣지 않는다.
- 선택 기술은 이동·전투 공개 계약을 통해 효과를 요청하며 그 상태를 우회 수정하지 않는다.

## Blocking decisions

- 공개 명령/사건 이름과 페이로드: OD-WT-001, OD-RUN-001, OD-CHOICE-002
- 버전 저장 필드와 복구 정책: OD-RUN-001, OD-PLAT-001
- 입력 액션 목록과 기준 재생 프로필: OD-PLAT-001

이 항목이 해결되고 모든 소비 스펙이 동일 계약을 참조하기 전에는 이 문서를 Approved로 전환하지 않는다.
