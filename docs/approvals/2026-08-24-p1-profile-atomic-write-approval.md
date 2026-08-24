# P1 프로필 원자 저장 순서 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Owner: Sol

## Approved boundary

- `Application.persistentDataPath` 아래 sibling `profile.json`, `profile.prev.json`, `profile.tmp.json` 사용
- 다음 revision canonical JSON을 temp에 전체 기록하고 storage까지 flush
- file close 뒤 temp를 다시 읽어 schema·field·canonical byte·hash 검증
- primary가 있으면 검증된 temp로 atomic replace하면서 교체 전 primary를 previous로 보존
- 최초 저장은 검증된 temp를 같은 directory의 primary로 move
- write·flush·validation·replace 어느 실패에서도 기존 primary·previous는 변경하지 않음
- 저장 성공은 replace 또는 move가 끝난 뒤에만 발행
- primary 선삭제·truncate·in-place overwrite 금지

## Remaining boundary

시작 시 primary·previous·stale temp의 선택 우선순위와 손상·미지원 version·binding asset 불일치의 격리·복구를 확정하기 전에는 `OD-PLAT-001`을 해결 처리하지 않는다.
