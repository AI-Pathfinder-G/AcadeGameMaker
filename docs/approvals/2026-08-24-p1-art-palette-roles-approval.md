# P1 32색 역할 분리 팔레트 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- master palette는 world 21색과 semantic accent 11색, 총 32색 role set
- 평균 장면 면적 목표: cold blue-grey/charcoal 70%, brass/rust/concrete 20%, semantic accents 합계 10% 이하
- TransferReady=electric cyan, Cooldown=amber orange, blocked=debt-stamp red, ActiveTransfer=cyan+white
- Extraction skill=crimson+ink violet, Solidarity skill=pale cyan+ivory
- heroine identity=warm ivory+muted rose/gold
- semantic 11색은 TransferReady cyan, ActiveTransfer white, Cooldown amber orange, blocked red, Extraction crimson·ink violet, Solidarity pale cyan·ivory, heroine warm ivory·muted rose·gold에 각각 한 slot을 배정한다. Solidarity ivory와 heroine warm ivory는 별개 색이다.
- semantic accent의 background·ordinary prop decoration 사용 금지
- heroine identity hue의 interactable·loot·reward indicator 재사용 금지
- URP lighting은 value·saturation을 바꿀 수 있지만 semantic hue family를 다른 state family로 회전시키지 않음
- color는 단독 상태 신호가 아니며 line shape·outline count를 함께 사용

초기 `world 24 + semantic 8` 안은 위 역할에 필요한 11개의 독립 의미색과 충돌하므로, 사용자의 후속 승인에 따라 총색 수를 유지한 채 `world 21 + semantic 11`로 정정했다.

## Remaining boundary

32색 exact HEX는 후속 승인으로 확정됐다. world/character outline value, URP 2D light layer·intensity·blend와 reticle base/acquired pixel size·glow·animation을 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
