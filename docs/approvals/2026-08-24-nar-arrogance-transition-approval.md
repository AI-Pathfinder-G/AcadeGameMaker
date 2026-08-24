# NAR 광오 전환·우선순위 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Decision owner: Sol
- Applies to: OD-NAR-002 partial resolution

## User approval

> 그래

## Approved transition contract

- 런타임은 UI에 노출하지 않는 `arroganceMarks` 0~4를 소유한다.
- 유효한 수탈 선택 하나가 확정될 때마다 `arroganceMarks` 가 1 증가한다.
- 기본 광오 단계는 `arroganceMarks`와 같은 단계를 지향한다.
- `손대지 않는다` 또는 `숨기지 않는다` 약속 파기는 광오 최소 2단계 `권리화`를 적용한다.
- `붙잡지 않는다` 약속 파기는 광오 최소 3단계 `지배`를 적용한다.
- 7장에서 라겐의 선택권 제거 논리를 수용하거나 강제 단일 공명을 확정하면 4단계 `절대`를 적용한다.

## Atomic priority

하나의 선택이 여러 조건을 동시에 충족하면 한 트랜잭션으로 다음 순서를 적용한다.

1. `humanityUnbroken = false`
2. `arroganceMarks = min(arroganceMarks + 1, 4)`
3. `arroganceTier = max(현재 단계, arroganceMarks 단계, 이벤트 최소 단계)`
4. 핵심 약속 파기와 관계 결과 확정

하나의 선택이 여러 약속을 동시에 침해해도 표식은 한 번만 증가한다. 단, 가장 높은 이벤트 최소 단계는 즉시 적용한다.

## Still open

- 선택 트랜잭션의 확정 시점과 저장·로드·재시도 생명주기

## References

[광오 상태 승인](./2026-08-24-nar-arrogance-state-approval.md), [NAR-00](../specs/full-game-narrative/00-spec-index.md), [ADR-0022](../adr/0022-character-first-eight-chapter-narrative.md)
