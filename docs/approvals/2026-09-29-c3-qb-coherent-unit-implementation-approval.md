# C3 실제 Q-B 발급 참조와 일관 구현 단위 승인

아스트라는 사용자 후속 구현·GPT-6 전환 지시에 따라 Approved C3의 내부 진입점 구체화를 승인한다. 솔(`gpt-6-sol`)의 [Q-B 발급 참조 설계](../proposals/2026-09-29-c3-qb-issued-request-handle-design.md) SHA-256 `28926347FAA0E247726177578C3B3A79358993F9ED0FC61C9C5886563C5F5CD3`, [일관 단위 계획](../proposals/2026-09-29-c3-owner-coherent-unit-plan.md) SHA-256 `E68280CD03EA28B3F0CC9B60530FD88762BB27BBE24B966BAD2810D02DF1DC59` 및 [루나 독립 설계 폐쇄](../verification/2026-09-29-c3-qb-gpt6-luna-closure.md) P0=0/P1=0을 확인했다.

내부 `AcceptNewGame`은 실제 등록된 `IssuedNewGameRequestV1` 참조를 필수 입력으로 받는다. 값으로 발급 증표를 찾거나 자동 선택하는 진입점은 금지한다. 내부 생성자는 미등록 후보만 생성하며 등록과 정확한 양쪽 소유 결속만 발급 권한을 만든다. 정상 불일치는 실제 take 전에 모든 요청·이력을 보존하고 하위 관찰에 진입하지 않는다. 실제 take 이후 부분 실패는 이력을 보존한 채 양쪽을 종료한다.

최초 세대의 무이력 조건과 후속 세대의 현재 슬롯 초기 조건을 분리한다. 후속 세대는 증가한 숫자·새 참조 증표·실제 재무장 증거를 요구하며 최초 Q-A 의도·Q-B taken과 모든 과거 기록을 유지한다. 새 cursor가 즉시 준비됐더라도 첫 실제 유효 프레임은 해석 없이 폐기한다. REQ-M5D7QC3-001..007과 AC-M5D7QC3-001/003/004/005/006/008/009에 기존 Approved C3 허용 목록을 적용한다.

구현 담당 테라 역할은 별도 `gpt-6-sol` 작업자이며 루나(`gpt-6-luna`)가 독립 검수한다. 하위 보정은 먼저 정확 지문으로 정적 검수하고, 새 상위 소유자·Q-B·presenter와 집중·필수 회귀는 마지막 동결 소스에서 수행해야 한다. 기존 하위 21/21 결과나 설계 폐쇄를 새 코드의 실행·전체 수용으로 대신하지 않는다. C3 전체 AC-007/008은 Open/Not Verified이고 C4는 Review다. C4 보고·identity 추출·C1 Begin·C2·실제 UI·장면·맵·외형·새 assembly/friend/public ABI는 승인하지 않는다.
