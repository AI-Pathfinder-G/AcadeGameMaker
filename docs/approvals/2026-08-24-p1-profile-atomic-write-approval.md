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
- Windows 구현은 `File.Replace(temp, primary, previous, true)`이며 기존 previous는 교체 전 primary byte로 대체
- 최초 저장은 검증된 temp를 같은 directory의 primary로 move
- replace 전 write·flush·validation 실패는 기존 primary·previous를 변경하지 않음
- replace 호출 오류는 성공을 발행하지 않고 세 파일을 재검증해 최소 하나의 직전 정상 snapshot이 primary 또는 previous에 남도록 요구
- 저장 성공은 replace 또는 move가 끝난 뒤에만 발행
- primary 선삭제·truncate·in-place overwrite 금지

## Remaining boundary

시작 시 primary·previous·stale temp의 선택 우선순위와 손상·미지원 version·binding asset 불일치 복구는 후속 승인으로 확정됐으며 `OD-PLAT-001`은 해결됐다.
