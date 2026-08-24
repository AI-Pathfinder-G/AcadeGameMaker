# NAR 광오 5단계·인간성 연속 보존 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Decision owner: Sol
- Applies to: OD-NAR-002 partial resolution

## User approval

> 그래

## Approved states

| Tier | Name | Narrative meaning |
|---:|---|---|
| 0 | 책임 | 남의 짐을 대신 지려 하지만 선택권은 존중한다. |
| 1 | 독단 | 시간이 없다는 이유로 동의 절차를 생략한다. |
| 2 | 권리화 | 자신의 희생이 타인을 대신할 결정권을 준다고 믿는다. |
| 3 | 지배 | 구조한 사람에게 감사와 복종을 요구하고 사람을 자원처럼 대한다. |
| 4 | 절대 | 선택 자체를 위험으로 규정하고 제거하려 한다. |

## Invariants

- 광오 단계는 UI에 숫자로 노출하지 않고 말투·호칭·자세·전투 후 행동으로 표현한다.
- 하나의 서사 저장에서 광오 단계는 낮아지지 않는다.
- 사과·보상·재합류는 관계를 일부 회복할 수 있지만 이미 내린 수탈 선택과 광오 단계를 삭제하지 않는다.
- `humanityUnbroken`은 광오 단계와 별도인 불변 상태다. 새 서사 저장에서 `true`로 시작하고 최초 유효 수탈 선택이 확정되면 `false`로 바뀐 뒤 해당 플레이에서 복구되지 않는다.
- `각자의 아래`와 진엔딩은 `humanityUnbroken == true`를 요구한다.

## Still open

- 수탈 이벤트별 광오 단계 전환과 최소 단계 규칙
- 동일 이벤트에서 여러 전환 조건이 겹칠 때의 우선순위
- 선택 확정·저장·로드·재시도 생명주기

## References

[NAR-00](../specs/full-game-narrative/00-spec-index.md), [ADR-0003](../adr/0003-humanity-is-preserving-others-agency.md), [ADR-0005](../adr/0005-choices-grant-different-skill-families.md), [ADR-0022](../adr/0022-character-first-eight-chapter-narrative.md)
