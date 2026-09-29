# C3 Q-B 발급 handle 설계 독립 반대검수

- 검수자: Luna
- 날짜: 2026-09-29
- 대상: `docs/proposals/2026-09-29-c3-qb-issued-request-handle-design.md`
- 대상 SHA-256: `26A0E6215CB727A670F99680464D8E895882C0DDD80E923831D9819B550B5863`
- 기준: Approved C3 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md`
- 범위: 정적 설계 검수. 소스·시험·Unity 실행·수용은 수행하지 않음.

## 판정

P0는 없다. readonly value row의 Item/Receipt/epoch 값 검색을 권한 증거로 쓰지
않고, sealed opaque `IssuedNewGameRequestV1`와 Q-B·owner의 exact 참조를 함께
검증하며 `Consumed 0 -> 1` 승자만 소비하도록 한 설계는
`AC-M5D7QC3-001`, `AC-M5D7QC3-003`, `AC-M5D7QC3-005`의 방향과 일치한다.
copied row와 실제 owner만 제공하는 경로를 capture/root 조회 전에 거부하고 실제
handle의 consumed를 0으로 유지한 뒤 원래 handle이 정확히 한 번 성공하는 음성 후
양성 순서도 필요한 위조 방어 증거를 충족한다. foreign handle, owner/router/epoch
불일치, legacy-only take 및 duplicate handle의 fail-closed 요구도 적절하다.

## P1 — 비-NewGame 거부 순서의 계약 명시 필요

제안된 첫 발급 순서는 legacy `TryTakeRequest`로 실제 take와
`_takenRequest`/proof/`RequestTaken` 이력을 먼저 확정한 뒤 row가 `NewGame`인지와
presenter handoff receipt를 검사한다. 따라서 현재 private request를 C3 전용
preflight로 안전하게 읽을 수 있는 경우에도 비-NewGame 또는 handoff 불일치가
먼저 소비된 뒤 terminal close될 수 있다. Approved C3의
`AC-M5D7QC3-001`은 비-NewGame·foreign receipt를 fail-closed로 요구하고,
`AC-M5D7QC3-005`는 Q-B 이력의 불변성과 재사용 금지를 요구한다.

Astra가 허용하는 최소 보정은 legacy overload 본문을 바꾸지 않고 C3 overload 안에서
현재 private request의 Item과 immutable handoff를 읽어 비-NewGame/불일치를 actual
take 전에 clean reject하는 것이다. 이 preflight가 안전하게 불가능하면 현재 제안의
take 후 terminal close를 의도된 이력 소비로 명시하고, 그 경우에도 C3 handle은
발급하지 않으며 재발급·rollback·역사 덮어쓰기를 금지해야 한다. 이 순서 선택은
구현 전에 Astra가 계약에 기록해야 하며, Luna는 어느 쪽도 수용으로 간주하지 않는다.

## 허용되는 범위 보정 및 유지 조건

Approved C3 예시의 내부 `AcceptNewGame(HubMenuIntentRequestV1, ...)`는 실제
`IssuedNewGameRequestV1` handle(또는 owner의 exact private pending handle)을
입력으로 받도록 좁혀야 한다. 이는 `AC-M5D7QC3-001`의 정확 intake와
`AC-M5D7QC3-003`의 숨은 권한 방지에 필요한 Astra 승인 범위의 내부 인터페이스
보정이며, 새 public ABI·friend/asmdef·역방향 참조를 만들 권한은 아니다.

기존 `TryTakeRequest(out ...)`, 최초 `_takenRequest`, proof, `RequestTaken`과
legacy 시험 의미는 그대로 보존해야 한다. C3 intake는 Item/Receipt/epoch scalar나
caller bool로 pending handle을 찾지 않고, Q-B·confirmation owner·presenter·router·
epoch·record를 exact reference로 검증해야 한다. 실행 결과/C1·C4 scalar를 Q-B 발급
증거로 추가해서도 안 된다. 따라서 `AC-M5D7QC3-009`와 C4 Review 경계를 완화하지
않는다.

## 상태

- P0: 0
- P1: 1 — 비-NewGame/handoff 불일치의 preflight 대 take 순서를 Astra 계약에 명시
- 이 기록은 설계 반대검수이며 구현·시험·C3 전체 수용·C4 수용을 의미하지 않는다.
