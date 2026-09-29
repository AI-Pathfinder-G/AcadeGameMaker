# C3 Q-B 발급 handle 설계 후속 독립 검수

- 검수자: Luna
- 날짜: 2026-09-29
- 대상 SHA-256: `45180BCD0D73FFF9D39D0051131452AF2D2B853C5F607C803649A71B651C8750`
- 기준: Approved C3 `AC-M5D7QC3-001/003/005/009`
- 범위: 정적 설계 검수. 소스·시험·Unity 실행·수용은 수행하지 않음.

## 판정

기존 P1인 비-NewGame·foreign·receipt 불일치의 선검증 문제는 폐쇄되었다. 최초
발급 단계가 private live row와 proof를 actual legacy take 전에 읽고 `NewGame` 및
presenter handoff를 확인하며, 실패 시 live request, 양쪽 pending 슬롯, legacy와
history를 변경하지 않고 lower capture/root 경계에도 진입하지 않는다고 명시한다.

actual `IssuedNewGameRequestV1` handle만 `AcceptNewGame`의 필수 입력으로 삼고,
row-only entry·Item/Receipt/epoch 값 검색·owner pending 자동선택을 금지한다.
`Consumed 0 -> 1` 단일 소비, copied row의 consumed 0 유지 후 원래 handle 1회 성공,
legacy take 본문과 최초 `_takenRequest`/proof/`RequestTaken` 보존도
`AC-M5D7QC3-001`, `AC-M5D7QC3-003`, `AC-M5D7QC3-005`에 부합한다.

## 남은 P1 — 최초 pristine와 successor 슬롯의 적용 범위

첫 발급 순서 1번은 confirmation owner가 pristine `AwaitingRequest`이고 private
pending-issued handle/issuance/history가 없어야 한다고 명시한다. 이는 최초 세대
조건으로는 정확하다. 그러나 successor 절은 fresh epoch token과 fresh
`SuccessorRequestSlot`을 만들고 최초 history를 보존한다고만 설명한다. 실제 구현
시 이 pristine 검사가 전체 owner의 과거 append-only history가 비어 있어야 한다는
뜻으로 재사용되면 Cancel successor가 정당하게 거부된다.

Astra가 허용하는 최소 문구 보정은 다음을 분리해 고정하는 것이다. 최초 세대에서만
owner 전체가 pristine이고 history가 없어야 하며, successor 발급에서는 과거
append-only record와 최초 taken 필드를 반드시 보존한 채 **현재 epoch의 fresh
successor slot, pending handle/issuance 및 현재 live request만** pristine이어야
한다. 이전 epoch handle은 자동 검색·재활성화하지 않는다. 이 문구가 구현 계약에
반영되기 전에는 successor 경로를 `AC-M5D7QC3-005` 수용으로 판정하지 않는다.

## 상태

- P0: 0
- P1: 1 — 최초 세대 무-history 조건과 successor 현재 슬롯 pristine 조건을 명시적
  으로 분리
- 설계 보정은 구현·시험·C3 전체 수용·C4 수용을 의미하지 않는다.
