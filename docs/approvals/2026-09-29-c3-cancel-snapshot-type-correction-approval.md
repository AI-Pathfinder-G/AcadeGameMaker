# C3 취소 관찰 타입 이름 보정 승인

2026-09-29. 아스트라 승인, 상태 `Approved`.
REQ-M5D7QC3-001/005/007, AC-M5D7QC3-005/009/010.

[솔 제안](../proposals/2026-09-29-c3-r10-cancel-snapshot-exact-type-correction.md) SHA `5919567813731A1C17AF53B5D6E6C32C0AE74C5BC61C9C3114C618573FA125BC` 및 [루나 설계 검수](../verification/2026-09-29-c3-r10-cancel-snapshot-exact-type-correction-luna-design-review.md) SHA `E03EAD9F1FDDECB3871D1C328A0D06546FEFBAF387D7A2E0A1D40510754B2DD9`, P0/P1=0을 근거로 승인한다.

기존 Play Owner 시험 CancelSnapshot의 `AcadeGameMaker.Movement.Unity.PlayerMovementController, AcadeGameMaker.Movement.Unity` 문자열 한 곳만 `AcadeGameMaker.Movement.PlayerMovementController, AcadeGameMaker.Movement.Unity`로 정정한다. 정확 `_player` 필드와 조립 이름, 나머지 네 항목·엄격한 선언/타입 검사·전후 전체 스냅샷 조건은 유지한다. 현재 Play B573AA593F8624C4A1E6F86059EC02E4DC26EEB3203F8774E74828B166B7850B와 원장을 보존한다. 제품·조립·friend·설정·meta·입력·권한·시간 제한은 변경하지 않는다.

별도 gpt-6-sol 구현과 gpt-6-luna 독립 검수 후 최종 R11 소스에서 실행 모드 15개 전체를 한 번 수행한다. 이름/수·실제 종료·실패/건너뜀/판정 불가·전후 지문을 검증하고 두 실제 취소 사례를 포함한다. 새 14파일 원장과 정확 240/15 선택·377행/91분할을 기록할 수 있으며 기존 생성기·기본 출력·행/매핑과 필수 562/51/610 선택은 불변이다. 원래 실패는 취소 전 스냅샷 미완료이므로 취소 제품 결함이나 기존 실행 통과를 주장하지 않는다. 이 승인은 식별자 보정에 한정하고 실제 통과·C3/C4 수용은 별도이다.
