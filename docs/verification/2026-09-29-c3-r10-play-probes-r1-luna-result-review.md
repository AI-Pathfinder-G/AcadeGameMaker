# C3 R10 Play probe 실행 독립 결과 검토

PlayMode XML SHA-256 `1760907B345AE6F7A41410F1B5FFCD6C93E3E2AE13347C3BBE298E8A047ABF5C`; 실행 비교 JSON SHA-256 `285084532DEF76CABC1E66A0A7932DC3C5EBC40DC17FBF9940E7DC7D0DA71561`; QA 반환 JSON SHA-256 `E1E846B6F56AE32E839DCA8A70859592011C4148B42C92ADE06CEF0538995AAD`. 정확 최종 동결 원장 `artifacts/c3-upper-r10-frozen-source-manifest.json` SHA-256 `796057D37EF6331CB5E43B7D1DC9CBA7F21F82848B24E8FC259C27A7DC21E6D0`도 현재 14개 경로에서 재해시해 14/14 일치했다.

**probe 범위 실행 판정 통과: P0 0, P1 0.** 실제 XML은 예상한 두 이름을 각각 한 번 실행해 `Total=2, Passed=2, Failed=0, Skipped=0, Inconclusive=0`을 기록했다. 실행 비교 JSON은 `Verified=true`, runner exit 0, QA 도구 반환 0, 이름 차이/중복 0을 기록한다. 884 입력의 전후 비교도 884/884·차이 0이고 before/after manifest가 완전히 같다. XML 전체 실행시간은 17.9918212초다.

첫 probe `AC001_NormalDisableAfterActualTakeRejectsLateIntakeWithoutChangingClosedHistory`는 실제 take 후 Owner 비활성화에 따른 owner/Q-B Closed 상태, late Accept 거절과 전체 intake snapshot 및 operation 불변 검증을 통과했다. 두 번째 `AC006_ImmediateReadyFactoryDiscardsActualSubmitBeforeNewTake`는 첫 successor cursor Ready/BaselinePending, 첫 실제 Enter Submit true 발행과 폐기 후 retained 없음, 이전 cursor/intent/taken history 유지 및 RequestTaken 상태, 중간 재take 거절, 그 뒤 release와 두 번째 실제 Submit·take·Accept의 DecisionRequired 결과 및 새 decision capability 참조를 통과했다. 이 결과는 R9 두 probe 실패를 R10 소스에서 해결한 이 두 사례에 한정한다.

**전체 acceptance는 아직 미완료다.** Play Owner 선택의 나머지 13개 시험, 240 Edit 시험, 377행/91-case matrix의 실제 실행 대조, 필수 562/51/610 회귀는 이 probe XML에 포함되지 않았다. C3 전체 AC와 C4도 수용되지 않았다. 이 결과를 full selection이나 한 번의 15-case 실행으로 표현하지 않는다. 이번 검토는 원본 증거의 읽기 대조만 수행했고 추가 소스 변경·Unity 실행은 없었다.
