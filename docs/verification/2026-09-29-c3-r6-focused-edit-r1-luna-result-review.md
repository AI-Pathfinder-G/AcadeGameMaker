# R6 EditMode 실제 실행 결과 독립 검토

## 판정

이 기록은 종료된 기존 R6 집중 실행의 증거 대조다. 런타임·시험·선택·설정을 변경하거나 재실행하지 않았다. 실행은 QA 통과가 아니다. 결과를 막는 미해결 항목은 P1 3개 묶음이다: AC001 비활성화 기대상태 불일치 1건, AC006 실제 Submit 입력 증거 부재 1건, AC002/AC004 네 시험의 180초 제한시간 초과 4건이다. 이는 우선 시험/fixture가 기대한 실행경로를 입증하지 못했다는 뜻이며, 현재 증거만으로 제품 결함 또는 시험환경 문제 중 하나로 확정하지 않는다.

## 실제 실행과 원장 대조

`artifacts/c3-r6-focused-edit-r1-native-exit-observation.json`은 PID 44184와 고정 생성시각 `2026-09-29T01:02:30.3598020Z`를 기록하고 실제 종료 코드 2, 결과 분류 `TestFailure`, `NewLaunchPerformed=false`, `TerminationPerformed=false`를 남긴다. 종료 시각은 `2026-09-29T03:15:49.8546451Z`다. 실제 결과 XML SHA-256은 `39A4C94679923DEAD39AE88EF0A6F8AA69C707D165304A509A447EF09EFDB847`, 로그 SHA-256은 `59966E3CBFACBFCE74946DFB6806C76C7B48F4D678A4C7D67FA5E6363AC52A5B`다.

`artifacts/c3-r6-focused-edit-r1-final-comparison.json`의 XML 집계는 총 155, 통과 149, 실패 6, 건너뜀 0, 미확정 0이다. 예상 및 실제 전체 시험 이름 155개가 일치하고 중복·이름차이는 0이다. 입력 스냅샷 884개는 각 경로별 변경이 0으로 비교되어 실행 전후 입력 변화 증거는 없다. 이는 통과를 뜻하지 않는다.

`artifacts/c3-r6-focused-edit-r1-decision-row-comparison.json`은 기대 원장 377행을 모두 `planned`로 유지한다. XML 결과에서 이 377행 중 일치한 실제 증거행은 0이고 `EvidenceMatched=false`, `SourceMatchesExecution=true`다. 내부 계획 377개와 실제 NUnit 시험 155개를 혼동하거나, 계획행을 실행 통과로 세지 않는다.

## 여섯 실패의 해석

- `AC001_NormalDisableAfterActualTakeRejectsLateIntakeWithoutChangingClosedHistory`는 실제 비활성화 직후 기대 `Closed`, 관측 `AwaitingRequest`로 다르다. fixture는 활성 호스트에 붙은 Owner의 `Behaviour.enabled=false`를 사용한다. Owner의 비활성화 콜백은 닫힘 처리를 맡으므로 이 결과는 제품의 생명주기 종료 결함일 수 있다. 다만 EditMode fixture에서 콜백이 실제 호출됐는지와 컴포넌트·상위 GameObject 활성 상태를 입증하는 별도 관측이 없다. 해당 분기 단독으로 재현하면서 `isActiveAndEnabled`, 실제 콜백 호출 관측, Owner·handoff 종료 상태를 함께 남겨야 제품 판정이 가능하다. 시험을 통과시키기 위해 `OnDisable`을 직접 호출하거나 예상값을 낮추면 안 된다.
- `AC006_ImmediateReadyCursorDiscardsActualSubmitFrameWithoutActivation`은 `<Keyboard>/enter` 상태 이벤트를 큐에 넣고 `InputSystem.Update` 및 fixture 입력 단계 뒤에 `CurrentUiFrame.SubmitPressed`가 참이라고 기대했지만 거짓이었다. 그러므로 이번 증거는 Submit 프레임 격리 동작 자체를 검증하지 못했다. 입력 장치 등록, Submit action binding·enabled 상태, publication receipt와 단계 순서를 읽기 전용 진단하고, fixture가 실제 router publication을 관찰하게 해야 한다. Submit을 직접 프레임에 주입하거나 assertion을 제거해 통과로 만들면 안 된다.
- `AC002_ActualFreshCaptureAllRolesFieldsAndPriorityMatrix` 및 `AC004_ActualFileTransitionMatrixClosesOldDecisionAndBindsFreshGeneration`의 매개변수 0·1·2는 각각 180,000ms에 시간 초과됐다. XML은 이를 실패로 기록하고 per-row 실제 결과는 남기지 않는다. 이는 소요시간/정지 여부 문제를 보여 주지만 미완료 행의 제품 동작을 증명하지 않는다. 네 항목을 각각 독립 선택해 재현하면 어느 단위에서 시간이 멈추는지 분리할 수 있다. 각 시험의 승인된 180초 제한은 유지한다. 단독 실행도 시간 초과하면 본문 작업량과 대기를 제한적으로 계측·분해하되 모든 377 기대행을 실제 시험결과에 연결하는 범위는 보존하고, 시간제한 변경은 별도 승인 없이 하지 않는다.

## 수용 범위

XML과 로그는 실제 실행 증거로 보존되며, 이번 런의 시험이름·입력 동결 대조는 일치한다. 여섯 실패로 인해 377행 원장과 실행 증거의 대응이 전혀 성립하지 않아 AC 실행 증거는 수용할 수 없다. 이는 그 자체로 런타임 전체에 대한 결함 판정은 아니며, 위의 최소 진단 뒤 승인된 범위로 재실행하고 새 결과를 별도 증거로 평가해야 한다. C3 전체 AC007/008 및 C4 수용 여부를 이 실행에서 추론하지 않는다.
