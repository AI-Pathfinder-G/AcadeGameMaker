# P1 입력 재지정 충돌 정책 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Owner: Sol

## Approved boundary

- 같은 control scheme의 Gameplay map 안에서 exact duplicate binding 금지
- 이미 사용 중인 control 지정 시 기존 action과 교환할지 명시적으로 확인
- 승인하면 두 binding을 한 transaction에서 교환하고 한쪽이라도 실패하면 둘 다 rollback
- 취소하면 어느 binding도 변경하지 않음
- 자동 삭제와 경고 없는 overwrite 금지
- Gameplay와 UI map은 상호 배타적이므로 map 사이 동일 control 허용
- keyboard Move 방향과 Gameplay action, mouse Attack·Transfer도 같은 교환 정책 적용
- protected UI·Pause 기본 binding과의 교환 거부
- chord·multi-key runtime binding은 수직 데모 비범위

## Remaining boundary

binding override를 포함한 저장 schema·경로·원자 저장·시작 복구는 후속 승인으로 확정됐으며 `OD-PLAT-001`은 해결됐다.
