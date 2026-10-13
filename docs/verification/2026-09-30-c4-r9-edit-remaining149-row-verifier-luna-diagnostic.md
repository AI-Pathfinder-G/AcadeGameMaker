# C4 Remaining149 행 검증기 진단

검토는 실패한 실행의 기존 증거와 현재 행 검증기만 읽어 수행했다. Unity 재실행, 파일 수정 또는 결과 재발급은 하지 않았다.

## 관찰된 결과

- 계획 `artifacts/c4-final-validation-queue-plan-v13.json`은 `c4-edit-remaining149`에 NUnit 149개를 선택하고 `RequiredRowIds`를 빈 배열로 지정한다. 이 하위집합은 C4 행렬 행을 실행하지 않는 계획이다.
- `artifacts/c4-edit-remaining149.xml`(SHA-256 `505C686F1D2E528558BAE57BA7165301D0DAF3FC88AB6E0AC6A76C55E537F121`)에는 149개 결과가 모두 통과로 기록되어 있고 실패·건너뜀·미확정 결과는 없다. 실제 프로세스 종료 관측과 QA 반환은 각각 0, 외부 반환도 0이다. 입력 전후 캡처는 각각 1,060개 경로이며 각 경로의 SHA-256과 입력 목록 결속이 서로 같다.
- 그럼에도 큐는 `artifacts/c4-final-validation-queue-result-v11.json`(SHA-256 `D19C57E5B76C5BCBD95864E2F02695355251E2F9E71A6EB66F52F3F3A4DC9152`)에서 `c4-edit-remaining149`의 행 비교 실패를 이유로 중단했다. 해당 실행의 행 비교 보고서는 생성되지 않았다.

## 원인과 범위

현재 `artifacts/c4-verify-required-rows.ps1`(SHA-256 `74F26AC5BFD34A2ABD78814B981844CB2A9CACCADED1062973FB22665BB4CD12`) 343–344행은 `if` 식 결과로 행 배열을 대입한 뒤 개수를 검사한다. `RequiredRowIds`가 명시적으로 비어 있으면 PowerShell이 빈 배열 출력을 `$null`로 풀 수 있어 `$rows.Count` 접근에서 오류가 난다. 따라서 이번 실패는 149개 시험이 실패했다는 증거가 아니라, 빈 행 선택을 처리하지 못한 행 검증기 결함이다.

`artifacts/c4-edit-remaining149-row-diagnostic-v1.json`(SHA-256 `22F180AD1905A4BD995325046269AB3193C19CAC0DD49415E5A1179A71B06BB9`)은 C3의 377행 비교기에서 나온 0/377 결과다. 이 실행의 C4 행 비교 결과가 아니며 수용 근거로 사용할 수 없다.

최소 보정은 빈 배열을 명시적으로 보존해 0개 선택을 정상 처리하는 것이다. 그 경우에도 예상 행 원장 전체의 스키마·순서·중복 검증, XML의 잘못된/예상 밖 행 거절, 선택된 행의 정확 매칭, 계획·선택·입력 전후·XML 지문 결속은 유지해야 한다. 빈 예상 행 집합을 자동 통과로 간주하거나 기존 검사를 약화해서는 안 된다.

## 판정

P0 0, P1 1. P1은 AC-M5D7QC4-009/010 실행 결과 판정을 막는 행 검증기 결함이며, 제품 동작 결함으로 분류할 근거는 없다. NUnit 결과가 통과했더라도 이 실행은 검증기 오류로 큐의 성공 실행으로 승인되지 않았다. 전체 큐와 C4 수용은 미완료다.
