# P1 프로필 로드 복구 및 OD-PLAT-001 최종 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Resolves: `OD-PLAT-001`
- Owner: Sol

## Approved boundary

- 시작 시 valid primary를 항상 우선하며 더 높은 revision의 valid stale temp도 로드하지 않음
- primary가 valid여도 invalid previous를 검사·격리·기록
- stale temp는 recovery directory에 진단용으로 보존
- primary missing·invalid 시 valid previous, previous도 없거나 invalid면 revision 0 default profile 사용
- corrupt·unsupported 파일은 삭제하거나 v1로 추측하지 않고 reason·UTC·hash8 이름으로 보존
- valid previous `r` 복구는 result revision `r+1`; default는 uncommitted source `-1`에서 첫 result revision `0`
- input asset ID·binding schema mismatch 또는 override 적용 실패는 input block만 기본값으로 복구하고 settings·tutorial·progression을 보존해 source `r`에서 result `r+1`
- 복구 원인·source·원본/결과 revision·격리 경로를 진단 로그에 남기고 launch당 한 번 비차단 알림
- 복구 저장 실패는 검증된 in-memory profile로 거점 진입을 허용하되 비보존 상태를 알리고 다음 저장에서 재시도

## Resolution

Input System 구조, Gameplay/UI binding, pointer targeting, runtime rebind 범위·충돌, versioned profile schema, atomic write와 load recovery가 모두 확정되어 `OD-PLAT-001`을 해결한다.

[Luna 독립 문서 검토](../verification/2026-08-24-od-plat-001-luna-review.md)는 보완 후 PASS했다. 관련 스펙은 다른 P1 결정과 전체 소비 스펙 검토가 끝날 때까지 Review이며 구현은 아직 시작하지 않는다.
