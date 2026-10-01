# C4 변경·신규 22파일 근거 원장의 제한 수용

2026-10-01, 아스트라, 실제 `gpt-6-astra`.

[Approved 계약](../specs/work-contracts/2026-10-01-c4-changed-file-evidence-closure.md) SHA-256 `FDBC8ECE8DA136CDA62E58E23CCAD8C3CABE8A197492A2BFB5ADBDB8A06E48E5`에 따른 테라의 **파일별 근거 분류 결과만 수용한다.** 원본 JSON `artifacts/c4-changed-file-evidence-closure-v1.json` SHA-256 `1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB`, 설명 SHA-256 `12AD67063501692BB1B1CB8E1A53440745AB7389D554FDF26EB9405906CFE869`, 테라 보고 SHA-256 `C95D08006C9FDF0EF59DFA848689DAA538CC40F72B68304AB3009C84510D9D8B`와 [루나 독립 검토](../verification/2026-10-01-c4-changed-file-evidence-closure-luna-result-review.md) SHA-256 `9EA4709166F861D5F5D7E0B590345C835E1B54F6C145B77B1FE4342A00A93AD5`, P0/P1=0을 확인했다. 추적은 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`이다.

초기 원장 501행에서 정확 변경 승인 필요24 중 C4 동결에 속하는 22경로는 기존 변경10·신규 source/test6·신규 meta6이며, C3 산출물2는 범위 밖이다. 22개 현재 파일 SHA가 원장과 v19 동결에 일치하고 GUID·조립 결속22를 확인했다. 정확 승인 경로와 독립 현 바이트 검수는 각16개만 참이다. 신규 meta6은 두 조건 모두 미입증이다. 이전 바이트에서 현재 바이트까지의 완전한 변경 사슬은 22개 모두 미입증이므로 **완전히 폐쇄된 파일은 0개**다. 거짓이 아닌 미입증을 참으로 바꾸지 않는다.

정확 게시 허용 목록은 계속 빈 집합이다. 이 22개를 포함해 이전 원장의 501개 전부 `PublicationApproved=false`와 게시 제외를 유지한다. 후속 계약은 남은 독립 현 SHA·정확 meta 허용·변경 사슬을 좁게 다뤄야 한다. 이 제한 수용은 게임 코드 게시·깨끗한 복제·C3/C4 공동 최종 수용이 아니며, 현재 진행 중인 v15 마지막 실행을 대체하지 않는다.
