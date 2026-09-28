# C3L R4 EditMode 회귀 실패 독립 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 결과 원본: `artifacts/c3l-r4-edit-regression.xml`, `artifacts/c3l-r4-edit-regression.log`, `artifacts/c3l-r4-verification.json`
- 실행 범위: `AC-M5D7QC3L-005`, 562개

## 판정

선택 범위는 정상이다. 결과에는 총 562개, 중복 0, 이름 차이 0, 건너뜀 0, 판정보류 0이 기록되어 실제 의도한 R4 집합이 실행되었다. 입력 원본도 전후 874개와 차이 0이며, corrected watcher는 소유 Editor PID `35344`의 실제 종료 코드 `2`와 시험 실패를 기록했다.

이번 실행은 **561 통과·1 실패**이므로 `AC-M5D7QC3L-005`의 필수 회귀를 통과로 표시할 수 없다.

## 실패 원인과 영향

실패한 정확한 시험은 다음 하나다.

`AcadeGameMaker.Tests.EditMode.InputUnity.HubMenuPresentationControllerV1Tests.AC004_RecoveryCompletedNotificationUsesPreviousReceipt`

XML은 180,000ms 시간 제한 초과만 기록하고 스택 추적이나 assertion 실패를 제공하지 않는다. 현재 시험 소스에서 이 행은 `PreviousObservation(4)`와 `CommittedFirst` 저장 영수증을 실제 `Build`로 만들고, `ProfileLaunchNotificationV1.FromReceipt`가 `RecoveryCompleted`를 선택하는지와 이전 영수증 상관을 확인하는 정상 테스트 선택이다. reflection·가짜 결과·대체 경로는 사용하지 않는다.

동일 실행의 다른 시험도 58~151초로 비정상적으로 길고, 이 행은 187초에서 제한을 초과했다. 같은 시험은 `artifacts/c2-r35-final-editmode.xml`에서 175.735355초에 통과했지만 기본 180초 제한에 이미 근접했다. 시험·영수증·컨트롤러 관련 소스의 전후 지문 차이는 0이고, 이 시험은 새 Adapter getter 경로도 호출하지 않는다. 따라서 현재 증거만으로 영구 런타임 결함이나 단순 환경 지연을 확정할 수 없다. 이 행의 timeout은 회귀 기준상 미해결 P1이며, 현재 C3L 수용 또는 AC-M5D7QC3L-005 PASS 근거가 아니다. 시간 제한을 늘려 실패를 숨길 근거도 없다.

## 좁은 진단 재실행 범위

R5 worker 51개 실행이 끝난 뒤, 현재 소스·시험을 그대로 둔 새 소유 Editor에서 아래 한 행만 먼저 재실행한다. 동일한 기본 180초 제한을 유지한다.

`AcadeGameMaker.Tests.EditMode.InputUnity.HubMenuPresentationControllerV1Tests.AC004_RecoveryCompletedNotificationUsesPreviousReceipt`

동일 timeout을 다시 넘기면 곧바로 `AC004_AbsentNotificationUsesCleanPrimaryReceipt`와 `AC004_PersistenceDeferredNotificationUsesFailedSaveReceipt`만 인접 비교행으로 추가하여, `PreviousObservation`/`CommittedFirst` 조합과 일반 notification 분기의 차이를 분리한다. 성공해도 R4 실패 이력은 보존되며 전체 562개 회귀 또는 C3L 합성 범위의 재수용이 아니다. 반복 실패 시 `ProfileLaunchPreparationCoordinatorV1.PrepareCore`의 해당 fixture 경계와 영수증 검증 경로를 별도 원인 분석 대상으로 남긴다.

현재 결론: **P0=0, P1=1(시험 timeout 미해결)**. 소스·시험·timeout 설정은 수정하지 않았다.
