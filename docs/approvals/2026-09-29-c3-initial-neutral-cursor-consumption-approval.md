# C3 최초 중립 프레임 정상 소비 승인

2026-09-29. 아스트라 승인, 상태 `Approved`.
REQ-M5D7QC3-001/005/007, AC-M5D7QC3-001/006/009/010.

[솔 제한안](../proposals/2026-09-29-c3-r9-initial-neutral-cursor-consumption.md) SHA `847ADCAC9CA526A06DD6FE2A5065602879AB446F7163BCD17D35BA0E6FC283FC`와 [독립 설계 검수](../verification/2026-09-29-c3-r9-initial-neutral-cursor-consumption-luna-design-review.md) SHA `B6F361F3352100242B97914F953D1489D86FF358707E6DB1207E3F044586DF8A`, P0/P1=0을 근거로 승인한다.

기존 Play Owner 시험 SelectInitialNewGame의 최초 중립 `Publish()` 바로 뒤에 정상 `PresenterFixed()` 한 번만 추가한다. Router 발행과 메뉴 커서의 정상 연속 소비를 대응시킨다. 현재 Play 원본 `6A3FB8BE37AE8644E446AA04FAE63D4BF75469C4DA704D253D146E715D086FEF`와 원장을 보존한다. 다른 본문·검증·입력·형식·권한·시간 제한은 변경하지 않는다. 새 입력 발행·cursor reset·상태 대입·콜백 직접 호출을 추가하지 않는다. successor의 첫 실제 Submit 앞 빈 발행은 금지한다.

별도 gpt-6-sol 구현과 gpt-6-luna 독립 검수 후 같은 선행 두 사례와 나머지 13개 실행 시험을 검증한다. R10의 정확 14파일 지문·240/15 선택·377행/91분할을 새 버전으로 남길 수 있으며 원래 행과 매핑·생성기·기본 출력·필수 562/51/610 원장은 불변이다. 제품·조립·friend·settings·meta 변경은 없다. 실제 결과를 전후 지문과 이름·종료·실패/건너뜀/판정 불가까지 대조한다. 기존 실패와 그 한계를 보존하며 원인 확정·통과·C3/C4 수용을 사전에 주장하지 않는다.
