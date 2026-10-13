# C4 Edit 관측 복구 초안 독립 검토

- 검토 대상 초안: `docs/specs/work-contracts/2026-09-30-c4-focused-edit-observation-recovery-draft.md`
- 초안 SHA-256: `3F68EBA8AA2283A9E96036E84EDCA5395E265B873C9491083537CB8FB406D98B`
- 판정 범위: 설계 정적 검토. 코드 수정·Unity 실행·새 선택/원장 발급은 하지 않았다.
- 연결 기준: Approved C4 r4, Approved QA r2, Approved 시험 컴파일 복구 계약.

## 판정

P0 0건, P1 0건이다. 앞서 확인된 19·20번 체크포인트의 시점 설명 결함은 이번 초안에서 정확히 분리됐다. 다만 본 초안은 Draft이므로 수정 승인이나 실행 통과를 뜻하지 않는다.

## 대조 결과

- R3 Edit 실행의 116개 실패는 현재 시험 SHA를 고정 원장 v1의 과거 Edit 시험 SHA와 비교한 경로에서 발생했다. 초안은 현재 Edit fixture의 두 지문 읽기 경로와 Play fixture의 한 경로만 원장 v8로 옮기고, 변경되지 않은 선행 소스를 확인하는 나머지 다섯 v1 참조를 보존한다. 기존 v1~v7 원장을 덮거나 최신 지문을 무조건 승인하는 변경은 없다.
- C1 시험은 51개 사례 중 재조회가 중복 도달한 여덟 checkpoint(40, 41, 44–49)와 나머지 43개를 구분한다. 모든 방문을 세고 예외 및 `EvidenceRow.Reached()`는 사례당 한 번만 실행하게 하므로 중복 도달이 원래 실패를 가리는 문제를 막는다. 결과·장벽·가드·종료 단언은 그대로 유지한다. 정확 방문 수는 새 시험에서 실제 단언할 값이며 이번 검토가 Unity 실행으로 입증한 값은 아니다.
- 여섯 Owner fresh 사례의 중복 handback은 `ExecuteOwner()`가 이미 수행한 실제 handback 뒤에 같은 결과를 다시 넘기던 시험 경로에 한정해 제거한다. 단일 소비 규칙과 lower 전용 결과가 Owner 권한을 만들지 않는 기존 거절은 보존한다.
- 네 실패 사례의 CWT 구분은 정확하다. `BeforeFreshReserve`(19)는 lower 예약 생성·등록 전이며, `AfterFreshReserve`(20)는 lower `ExecutionReservations.Add` 뒤지만 lower 함수 반환 전이다. 둘 다 Owner의 `ExecutionFreshCapabilities.Add` 전이므로 해당 CWT 조회는 false여야 한다. 새 문안은 lower 예약도 19 미등록·20 등록으로 별도 원장/참조에 대조하도록 명시해 서로 다른 상태를 합치지 않는다.
- AC003 검색 시작점을 첫 `FinalizeReset` 호출 전체 토큰 뒤로 이동한다. 기존 코드는 첫 호출이 하나이며, 이전 검사의 `finalize+1` 위치는 같은 호출 내부 문자열을 다시 찾는 오탐이었다. 초안은 호출 순서와 단일 호출 검사를 유지한다.
- 수정 허용 파일은 Edit fixture, Play fixture, Edit test 세 개로 한정된다. 선택 v3 및 행 v3은 `SourceFiles` 지문만 바꾸고 188행·순서·선택 이름을 보존한다. 107파일/31 허용 변경의 manifest v8, 990경로의 input v8, 9회·72 출력 경로를 유지하는 plan v6, 별도 result-v5 경로도 서로 결속하도록 기술돼 있다. R3 실패 자료는 보존되고 아직 발급되지 않은 v8 자료나 결과를 이미 검증된 사실처럼 취급하지 않는다.

## 실제 바이트 및 한계

대조한 현재 시험 소스 SHA-256은 Edit fixture `0900831635B02D1F9CE0177A9FA8587C8CFFA2CD3E2D984B394825FA604CDB89`, Play fixture `23E91AF7F4DB54A2937E51E4BC1FEDDE677C702477591115D9562CF05B7A29B2`, Edit test `9F051A4A1F5BE4418CF1BD6E9CA1477B9C3933AEB645FF7FABEC4F655EB6F17E`다. 확인한 runtime 기준은 Owner `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`, C1/C4 실행 경계 Bridge `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`다. 이 검토는 제안된 수정본, 새 선택/행 자료, 컴파일 또는 Unity 실행 결과를 검증하지 않았다. 다음 독립 단계에서 해당 새 지문과 기존 이름·행 보존을 확인해야 한다.
