# C4 기존 변경 열 파일의 지문 전이 원장 제한 수용

2026-10-01, 아스트라, 실제 `gpt-6-astra`.

[Approved 한정 계약](../specs/work-contracts/2026-10-01-c4-existing10-change-chain.md) SHA-256 `119470C78C66ACCCA7EF19518A57622948675D6B1801FD4CE3DFE37818A4FC15`에 따른 지문 전이 **관측 원장의 정확성만 수용한다.** 테라 JSON `artifacts/c4-existing10-change-chain-v1.json` SHA-256 `FB4D6929BBB3325DE452FE3386E6A88EEF847D4CF992CAD5742A5F63AA4F727F`, 설명 SHA-256 `5B05885BA22A856D2647CE665DEE1D02759667CD09A35D1B62ABD5148F8AFDEA`, 테라 보고 SHA-256 `E8592E4F402E5427129C1D83DA2D65B7DE8C3F9317AD4B4C4B6F3C410CD70294`와 [루나 독립 검수](../verification/2026-10-01-c4-existing10-change-chain-luna-review.md) SHA-256 `7A12F4F069E103FAB2A0C108A67F9D0B0288FA798FA67E13B6FAA43F2C244A9F`, P0/P1=0을 확인했다. 추적은 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`이다.

기존 후보와 겹친 정확 10파일의 후보 SHA와 현재 실물/v19 SHA는 일치한다. v1–v19 동결 원장의 파일별 20단계 지문에서 **서로 다른 바이트로 바뀐 전이 14개**를 확인했다. 승인 문맥·독립 검수 인용·동결 위치의 기록은 원장과 일치하지만 각 전이의 당시 구현 바이트 증거는 14개 모두 없다. 따라서 열린 간격은 14/14, 완전한 `ChainClosed` 파일은 0/10이다. 관측된 지문 변화는 승인된 구현 바이트의 연속 사슬과 다르다.

이번 제한 수용은 부족한 역사적 바이트를 생성하거나 이전 승인을 확대하지 않는다. 신규 source/test6·meta6, 범위 밖 C3 도구2와 나머지 최초 게시 공백도 그대로다. 정확 게시 허용 목록은 빈 집합이고 `PublicationApproved=false`를 유지한다. 코드 업로드·깨끗한 복제·Unity 검증·C3/C4 공동 최종 수용은 이 결과로 승인하지 않는다.
