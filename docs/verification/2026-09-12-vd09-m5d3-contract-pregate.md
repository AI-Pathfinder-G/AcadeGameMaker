# VD-09 M5D3 계약 독립 사전 게이트

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D3 프로필 진행 상태 투영 코어](../specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md)
- Final result: **PASS — P0=0, P1=0**
- Execution: 문서·계약 정적 검토만 수행; 구현 및 Unity 테스트 미실행

## 검토 이력

첫 검토는 `default(ProfileChoiceSkillPair)`가 첫 유효 enum 조합과 겹칠 수 있고, default snapshot의 projection이 명시적 no-choice로 숨겨질 수 있다는 P1을 발견했다. 계약은 모든 enum의 underlying `0`을 invalid로 예약하고 legal 값을 `1+`로 고정했다. 또한 명시적 no-choice는 유효 consent와 null이 아닌 빈 branch collection을 생성자로 제공하도록 하고, default snapshot의 `Validate`/`HasCommittedPair`/`TryGetCommittedPair` 및 default pair의 `Validate`/public getter가 재검증 후 throw하도록 보강했다.

second pre-gate에서 위 P1은 닫혔다. nullable enum의 zero/unknown cast도 constructor와 revalidation 양쪽에서 거부된다.

## Parent 및 경계 판정

- `completedBranches`가 두 branch를 모두 포함하는 것은 VD-09 sorted-unique history와 VD-05 보존 의미에 충돌하지 않는다.
- consent–pair 교차제약을 이 구조 projection에서 새로 만들지 않는 것은 허용된다. 후속 save/M5A adapter는 실제 VD-06 receipt와 owner identity를 요구해야 한다.
- `ProfileRevision`의 `0..long.MaxValue` 경계, 이 단위에서 증가하지 않는다는 규칙, 초과 JSON을 후속 codec이 거부한다는 경계가 닫혀 있다.
- disk/canonical/hash/source authentication을 주장하지 않으며 Profile 전용 engine-free allowlist가 유지된다.

## 비차단 명료화 반영

Luna는 `Validate`의 정확한 public signature를 구현 전 고정하라는 P2를 남겼다. Astra는 두 value type에 `public void Validate()`를 명시하고 관련 public projection/getter가 같은 재검증을 선행하도록 계약에 반영한 뒤 승인했다.

## 결론

M5D3 구현은 계약의 정확한 allowlist 안에서 진행할 수 있다. 이 PASS는 pure progression projection 계약 승인 근거이며 실제 profile 파일, 저장 성공, 복구 또는 playable M5A 연결을 검증하지 않는다.
