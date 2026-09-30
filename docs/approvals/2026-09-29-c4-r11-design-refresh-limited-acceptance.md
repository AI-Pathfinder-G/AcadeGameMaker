# C4 R11 경계 설계 갱신의 제한 수용

2026-09-29, 아스트라. 솔의 [읽기 전용 경계 갱신 제안](../proposals/2026-09-29-c4-r11-frozen-boundary-design-refresh.md) SHA `BAB33568BD479AF46C265F3FCCEC232BBEF339C472B6F9CB1DF9A90E17347F0E`와 루나 [독립 검토](../verification/2026-09-29-c4-r11-frozen-boundary-design-luna-review.md) SHA `C9E969C33BF58F8EF401E5D468C6674239BF785BFBB952B936F3E5CE47592AA4`, 정적 P0/P1=0을 확인했다. REQ-M5D7QC4-001..007 및 AC-M5D7QC4-001..010의 경계 대조와 후속 설계 입력으로만 수용한다.

C2의 실제 Verified 및 정확 FinalizeReset 경계, C1 현재 후속 지문, C3 실행 commit 완료와 별도 C4 소비의 구분을 보존한다. 현재 Cancel/Rearm은 C1 NoBarrier Busy/Stale 이후의 새 확인 결속을 대체하지 못한다. 이 정적 접점 공백을 위한 별도 일회용 handback·새 결속·상호 가드와 좁은 Q-A/Q-B 보정 제안은 승인 검토 입력이다. 실행 반례나 실제 구현으로 주장하지 않는다.

현재 R11 동결을 변경하지 않는다. ADR-0036의 집중·필수 회귀와 독립 검수 후 아스트라 선행 수용이 필요하며, C4는 Review다. 제안 API·새 파일·Q-A/Q-B 보정 및 strict audit 변경은 별도 Approved 계약에 정확 경로와 사례를 동결하기 전 구현하지 않는다. C3 AC007/008과 실제 결과 발급, 전체 C4, 최초 코드 게시 차단은 미수용으로 유지한다.
