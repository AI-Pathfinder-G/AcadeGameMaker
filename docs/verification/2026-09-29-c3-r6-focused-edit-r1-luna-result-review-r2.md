# R6 EditMode 결과 대조 정정

이 문서는 [이전 검토](/C:/Users/me/Documents/GPT-workspace/AcadeGameMaker/docs/verification/2026-09-29-c3-r6-focused-edit-r1-luna-result-review.md)를 보존하면서 원본 XML 출력과 비교 JSON의 원시 집계를 보충한다. 이전 검토가 `ObservedPlanned`와 `ObservedPassed` 원시 수치를 빠뜨려 377행의 실행 출력이 없다고 잘못 읽은 부분을 바로잡는다. 코드·시험·선택·기존 증거를 수정하거나 실행하지 않았다.

## 정정된 증거

`artifacts/c3-r6-focused-edit-r1-decision-row-comparison.json`의 집계는 `ExpectedRows=377`, `ObservedPlanned=377`, `ObservedPassed=377`, `ObservedFailed=0`, `MatchedRows=0`, `EvidenceMatched=false`, `SourceMatchesExecution=true`다. 각 행의 출력에는 계획 항목과 해당 항목의 관측이 실제로 들어 있다. 예를 들어 `C2-default-0`은 기대 상태 `NoConfirmationRequired`, 관측 상태 `passed`, before/after 경로 상태와 동일한 AC002 NUnit 케이스 이름을 포함한다. 그러므로 377행의 실행 출력 자체가 없었던 것은 아니다.

다만 XML에서 이 행들을 출력한 상위 AC002 케이스와 AC004의 세 매개변수 케이스는 모두 `Failed`로 기록됐다. 네 케이스 각각의 XML 실패 원인은 NUnit `Timeout value of 180000 ms was exceeded`다. JSON의 해당 행들 역시 `CaseResult=Failed`이며 validator는 실패한 NUnit 케이스에서 나온 하위 행을 수용하지 않아 377개 모두 `Matched=false`, 총 일치 0으로 둔다. 따라서 `ObservedPassed=377`은 각 행 내부 관측 상태의 집계이지, NUnit 테스트 통과나 승인된 AC 증거 377건을 뜻하지 않는다. timeout 뒤에는 해당 케이스의 완결된 NUnit 결과가 없으므로 이 하위 출력만으로 승인 행을 통과 처리하지 않는 현재 0 일치 결론이 타당하다.

## 실패 구분과 다음 확인 범위

XML 전체는 예상 이름 155개와 일치하고 중복·차이 0, 총 149 통과·6 실패·건너뜀 0이며, 입력 884개도 경로별 변경 0이다. 실제 Editor 종료 코드는 2다. 실패 여섯 건은 다음과 같다.

- AC001 비활성화 시험은 `Closed`를 기대했지만 `AwaitingRequest`를 읽었다. 이 경로가 Owner의 실제 Unity `OnDisable` 생명주기 콜백을 거쳤는지 증명되지 않았으므로 제품 결함 가능성과 EditMode fixture 문제를 열어 둔다. 단독 시험에서 호스트 및 컴포넌트의 활성 상태, 비활성화 직후 `isActiveAndEnabled`, 콜백 후 Owner와 handoff 상태를 함께 수집한다. 콜백을 직접 호출하거나 기대 상태를 낮춰 성공 처리하지 않는다.
- AC006 시험은 키보드 Enter 이벤트 뒤 `CurrentUiFrame.SubmitPressed`가 참일 것을 기대했으나 거짓이었다. 이 런은 실제 Submit publication을 확인하지 못했다. 단독 진단에서 장치 추가 여부, Submit action의 binding·활성 상태, InputSystem 업데이트, router publication receipt·현재 frame 순서를 확인한다. 수동 프레임 주입이나 assertion 삭제는 허용하지 않는다.
- AC002 한 케이스와 AC004의 세 매개변수 케이스는 각각 승인된 180초 NUnit 제한을 초과했다. 그 전에 377개의 행 출력은 남았지만 네 상위 케이스는 실패했다. 개별 시험을 별도 선택으로 돌리는 것은 어느 케이스가 어디까지 출력하는지 분리하는 진단일 뿐이며, 이미 한 케이스별 180초를 넘긴 상황을 해결하거나 행 증거를 자동 승인하지 않는다. 제한 시간은 그대로 둔다. 구현 보정이 승인되면 필요한 경우 큰 매트릭스를 더 작은 NUnit 케이스로 나누되 기존 377행의 기대값·행 ID·검사 범위를 보존하고, 각 새 케이스가 자체 통과 결과로 대응 행을 내는지 새 XML에서 대조한다.

따라서 실행 사실을 정정해도 수용 판단은 바뀌지 않는다. 실제 행 출력은 377개 관측됐으나 상위 NUnit 케이스 실패에 결속되어 validator가 수용한 행은 0개다. 이전 검토의 세 P1 묶음(AC001 생명주기, AC006 입력 publication, AC002/AC004 timeout)은 유지한다. 이번 결과만으로 제품 결함을 확정하지 않으며, C3 전체 AC007/008 또는 C4를 수용했다고 보지 않는다.
