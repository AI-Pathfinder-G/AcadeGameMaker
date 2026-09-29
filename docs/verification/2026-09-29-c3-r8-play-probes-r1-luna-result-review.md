# C3 R8 Play probe 실행 독립 결과 검토

원본 XML SHA-256 `E8C6AFA445998AB0AD397D0DFA1436A9DAE6DEACCBD2588C53C93FB1D67D8556`을 대조했다. 예상한 두 정확한 PlayMode 시험 이름이 각각 한 번 실행됐고 누락/중복은 0이다. 실제 결과는 `Total=2, Passed=0, Failed=2, Skipped=0, Inconclusive=0`, 입력 원장 884개 전후 변화 0, 비교 `Verified=false`, QA 반환 5다. 부모가 확인한 Unity 종료 2와도 일치한다.

두 시험 모두 `SelectInitialNewGame()` 안에서 실행되는 실제 Enter 발행/`CurrentUiFrame.SubmitPressed=true`와 Q-B `RequestReady` 검증을 통과한 뒤 실패했다. 이어지는 `Fixture.Take()`의 `TryTakeNewGameForConfirmation` 결과가 false여서 `Assert.That(..., Is.True)`가 깨졌고, XML 위치는 helper 440행이다. 비활성화 probe는 line 29의 첫 take 단계에서, 강화 AC006은 line 87의 decision 생성 전 첫 take 단계에서 중단됐다. 각 실행시간은 7.630223초와 7.369897초다.

따라서 이 실행은 초기 실제 Submit 처리 및 request-ready 지점까지의 증거를 제공하지만, 실제 request take가 실패한 사실도 명확하다. 비활성화/late-intake 이후 closed-history 검사와 successor 재무장 후 첫 Submit 폐기·후속 take 검사는 어느 것도 실행되지 않았다. 이전 설계의 목적 assertion에 도달하지 않았으므로 해당 AC coverage는 통과로 대체할 수 없다.

관측된 실패는 초기 request 생성·보존 이후 실제 take 경계에 국한된다. XML만으로 소유자·라우터·latch 중 어느 상태가 거절 원인인지 확정할 수 없고 제품 결함으로 단정하지 않는다. 이전 R7 Edit 실패나 이번 Editor 갱신 결과와 원인을 합쳐 추정하지 않는다. 기존 실패와 증거를 그대로 보존하며 재실행·소스 변경·Unity 실행은 없었다.
