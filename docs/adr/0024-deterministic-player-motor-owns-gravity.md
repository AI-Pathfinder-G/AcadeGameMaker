---
status: accepted
---

# 결정론적 플레이어 모터가 실제 중력과 이동 상태를 소유한다

- Date: 2026-08-25
- Decision owner: Sol
- Approved by: Sol under the user-approved orchestration authority

플레이어는 kinematic `Rigidbody2D`와 cast-first adapter를 사용하고, Q4096 모터가 위치·속도와 실제 하방 가속 약 30.72u/s²를 소유한다. Unity dynamic solver의 중력·force·접촉 보정은 플레이어 이동에 적용하지 않는다.

프로젝트의 `Physics2D.gravity=(0,-9.81)`와 주인공 `gravityScale=3.1315`는 공통 물리 환경, 저작 검수, 향후 비플레이어 동적 물체와의 수치 호환을 위해 유지한다. Lightweight에서는 모터 중력을 정확히 ×0.65 적용하고 kinematic 바디의 호환 `gravityScale`도 같은 배율로 반영하지만, 이 바디 값은 플레이어 가속을 중복 생성하지 않는다.

Dynamic Rigidbody2D를 권위 원천으로 삼는 대안은 충돌 solver와 부동소수점 적분이 모터의 두 번째 이동 상태가 되어 tick replay, Q4096 snapshot, 동일 입력 재현성을 깨뜨릴 수 있어 반려했다. Kinematic 바디에서 `gravityScale`이 실제 가속을 만든다고 서술하는 대안도 Unity 동작과 맞지 않아 반려했다.

## Consequences

- `PlayerMotionSnapshot`과 kinematic body target은 같은 모터 commit에서 나온다.
- 접지·벽·천장·대시 차단은 solver callback이 아니라 현재 fixed tick의 cast 결과로 모터에 전달된다.
- `REQ-MOV-006`과 `AC-MOV-004`는 프로젝트 호환 설정과 실제 모터 가속을 별도로 검증한다.
- 플레이어 body type을 dynamic으로 바꾸거나 solver force를 추가하려면 새 ADR과 VD-01 계약 변경이 필요하다.
