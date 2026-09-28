# C3L R5 worker 회귀 독립 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 결과: `artifacts/c3l-r5-worker.xml`, `artifacts/c3l-r5-verification.json`
- 추적 기준: `AC-M5D7QC3L-005`

## R5 원본 판정

R5 worker 범위는 51개로 정확히 선택되었다. XML은 51/51 통과, 실패 0, 건너뜀 0, 판정보류 0이며 실제 소유 프로세스 `29440`은 종료 코드 0으로 종료했다. 선택 이름 차이와 중복은 0이고 입력 원본은 전후 874개로 차이 0이다. 검증 기록의 XML SHA-256은 `2A4A4A21D891EC10B95BDC6E226BD049221F1AE7C1EFDD2A74AADD9126E59FC6`이다.

실행된 고유 집합은 `ProfileResetDiskProcessV1Tests`의 실제 worker 충돌·재시작 51행이며, R4의 562행과 중복되지 않는 required C3/C1 회귀 부분이다. R5 자체에는 P0/P1 결함을 발견하지 않았지만, R4의 별도 timeout 실패를 해소하지 않으므로 `AC-M5D7QC3L-005` 전체 통과나 C3 `AC-M5D7QC3-010` 통과로 합산하지 않는다.

## R4·R6과의 집합 수용 가능성

계약의 “focused and required regression suites” 복수 표현과 기존 독립 범위 해석에 따르면, 현재 소스가 동일하다는 지문, 각 선택 원본의 정확한 예상 이름, 선택 집합의 무중복·무누락, 각 행의 실제 종료 코드 0을 새 ledger에서 행 단위로 입증하면 R4 561개 + R6의 정확한 실패행 1개 + R5 worker 51개를 합친 고유 613개 증거로 기록할 수 있다. 별도 전체 613개 단일 실행을 계약이 명시적으로 요구하지는 않는다.

다만 R6가 통과하기 전에는 이 ledger를 완성할 수 없다. R4의 1회 실패는 삭제하거나 통과로 바꾸지 말고 역사 기록으로 보존해야 하며, R6 통과 후에도 R4 XML 자체를 562/562 성공으로 재기록해서는 안 된다. ledger에는 R4의 561행 성공, R4의 timeout 실패, R6의 동일 단일 행 재실행, R5의 51행 성공을 각각 분리해 기록하고 최종 행 집합의 정확한 합집합을 계산해야 한다. R6 실패·건너뜀·판정보류 또는 source-before/after 불일치가 있으면 전체 613개 증거는 성립하지 않는다.

R6 성공 후에도 이는 현재 변경 범위의 C3L required regression 증거에 한정된다. C3 owner/rearm 통합 이후 필요한 최종 C3 EditMode·PlayMode 회귀와 `AC-M5D7QC3-010`의 Luna P0/P1=0·Astra 수용 요건은 별도로 유지된다.

현재 결론: **R5 자체 P0=0/P1=0, 전체 C3L 회귀는 R6 결과 대기**. R6 완료 전 추가 전체 613개 실행은 계약상 필수라고 단정하지 않지만, ledger의 정확한 불연속 범위와 source 지문 검증이 전제다. 코드·시험·timeout 설정은 수정하지 않았다.
