# C4 기존 변경 열 파일의 바이트 사슬 조사 한정 승인

2026-10-01, 아스트라, 실제 `gpt-6-astra`.

[변경 사슬 조회 계약](../specs/work-contracts/2026-10-01-c4-existing10-change-chain.md) SHA-256 `119470C78C66ACCCA7EF19518A57622948675D6B1801FD4CE3DFE37818A4FC15`를 **Approved**로 수용한다. 설계 Draft SHA-256 `6FB59AF732C0D26134AFA4BCE2CC0C79BE51AFB3F9715AAF8BA99E350BCE7B19`, [초안 독립 검토](../verification/2026-10-01-c4-existing10-change-chain-luna-design-review.md) SHA-256 `2D6727240F4D3A0A5CDED593ADB5A4E760079642D92BEC61226D0F98706D589C`와 [승인본 재검토](../verification/2026-10-01-c4-existing10-change-chain-luna-rereview.md) SHA-256 `4AB322C20B030D239C5708E3B3313499267BD18D074B69BC69DA719D370E184F`, P0/P1=0을 확인했다.

허용 범위는 선행 22파일 원장 중 기존 후보와 겹친 **변경 열 파일의 정확 경로**에 한정한다. 이전 후보 SHA부터 C4 v19 현재 SHA까지의 각 실제 변경에 Approved 정확 허용·구현 바이트·루나 독립 검수·후속 동결을 읽기 전용으로 연결한다. 하나라도 없으면 `ChainClosed=false`와 미입증 간격을 남긴다. 테라에게 계약의 새 JSON·설명·보고 세 파일만, 루나에게 별도 독립 검토 한 파일만 허용한다. 착수 전 네 경로가 v19 입력1084·소스193 밖이고 부재함을 다시 확인한다. 추적은 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`이다.

신규 C4 source/test6·meta6, C3 산출물2, 나머지 초기 게시 공백은 이 범위 밖이다. 기존 파일·동결 입력·QA·Git·원격은 수정하지 않고 Unity·클린 복제는 실행하지 않는다. 현재 22파일 사슬 폐쇄0·정확 게시 허용 목록 빈 집합·`PublicationApproved=false`를 조사 결과가 독립 수용되기 전까지 유지한다. 이번 승인은 코드 업로드나 C3/C4 공동 전체 수용을 뜻하지 않는다.
