# C3 후속 예약의 현재 실행 권한 보정 승인

아스트라는 Approved C3와 솔·루나 독립 검수의 동일한 늦은 예약 재사용 P1을 근거로 제한 보정을 승인한다. 추적은 REQ-M5D7QC3-001/003/005/006과 AC-M5D7QC3-001/003/005/006/010이다. 초기 동결 원장 `artifacts/c3-upper-frozen-source-manifest.json` 지문 `0849AE5B55FDD448175A3534CB2A6E67F36102257FDFA8FEA27C0D39DAEF1175`은 실패 발견 당시 이력으로 유지한다.

`HubMenuIntentHandoffOwnerV1.cs`에서 이미 커밋된 예약의 역사 결속 확인과 현재 Prepare/Commit 권한을 구분한다. 현재 권한은 실제 비공개 등록 예약, 정확한 pending 참조, 미커밋, 현재 원래 세대 숫자와 참조 증표, 정확한 소유자·presenter·router 및 `IsRearmInProgress`를 모두 요구한다. `HubMenuPresenterV1.cs`의 Prepare와 Q-B Commit은 이 현재 권한을 사용한다. 커밋 이력 조회를 후속 프레임·의도 이력에서 계속 사용할 수 있지만 이력이 새 controller/cursor 생성 권한이 되어서는 안 된다.

늦은 실제 예약 제출은 변이 전에 거절하고 현재 세대의 실제 슬롯·cursor·미결 발급 및 결정 권한을 보존한다. 실제 AwaitingBaseline, Ready, IntentRetained/Transferred 상태에서 같은 실제 예약·증표·세대를 제출하는 회귀와 원래 정상 경로의 후속 성공을 기존 신규 C3 편집/실행 모드 시험에 추가한다. 반사로 권한을 제조하거나 슬롯을 되돌리지 않는다. 하위 ABD4/F80, 어댑터·라우터, 기존 감사와 필수 회귀 이름을 유지한다.

담당 테라 역할은 별도 `gpt-6-sol` 작업자이며 루나 `gpt-6-luna`가 새 정확 지문을 독립 검수한다. 변경 후 새 동결 파일과 실제 컴파일·시험 증거를 별도로 남긴다. 기존 통과 재사용, 새 제품 콜백 시험 통로, 조립·friend·공개 권한·장면·입력 자료 변경, C1/C2 실행과 C4 구현은 승인하지 않는다. 이는 수정 구현 승인이고 실제 검증·수용이 아니다.
