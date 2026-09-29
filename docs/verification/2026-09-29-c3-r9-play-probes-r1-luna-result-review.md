# C3 R9 Play probe 실행 독립 결과 검토

XML SHA-256 `B70589124F3889F6647375061DF5E948EFD53E52E3C5FBBB67B8634F1AD45763` 및 비교 JSON SHA-256 `AF7A2AA59CC5AAAB374EF067359CFB247F7F5F585EC54DD62EF81C4C7A8A3643`을 원문 대조했다. 예상 두 Play probe 이름은 각 1회였고 이름 차이/중복 0이다. 실제 `Total=2, Passed=0, Failed=2, Skipped=0, Inconclusive=0`, 884개 입력 전후 차이 0, 비교 `Verified=false`, QA 반환 5다. Unity 실행 종료는 2로 보고됐다.

두 시험 모두 새 최초 선택 순서에서 실제 NavigateChanged, 음수 Navigate Y, actual Submit=true 검증까지 진행했으나 `SelectInitialNewGame`의 Q-B 상태 assertion에서 실패했다. XML의 정확 메시지는 `actual UI selection reaches Q-B`, 예상 `RequestReady`, 실제 `Closed`; 실패 위치는 helper 437행이다. 각 실행시간은 7.644901초 및 7.454794초다. 따라서 이전 R8의 첫 `Fixture.Take()` 실패 위치는 지나갔지만 Q-B 초기 상태를 정상 발급 단계로 넘기지 못했다.

실제 `_request`의 `NewGame` 항목 assertion은 RequestReady assertion 다음에 있으므로 두 실행 모두 미도달이다. 첫 take, owner 비활성화/closed-history 확인, decision/cancel/rearm, successor 첫 Submit 폐기와 다음 정상 take 역시 실행되지 않았다. XML에는 예외 진단 레코드가 없으며 별도 제품 원인 자료도 없다. Navigate와 Submit assertion 통과는 두 관측만 입증하며, `Closed`로 전이된 이유 또는 항목 값은 확정하지 않는다.

**독립 결과 판정: P0 0, 실행 수용 차단 P1 1.** R8 실패는 지나간 위치 변경으로 해결됐다고 종결할 수 없고, R9도 관심 AC의 통과 증거가 아니다. 원래 R8/R9 XML·원장을 그대로 보존한다. 이번 검토에서 재실행, Unity 실행, 소스 변경은 없었다.
