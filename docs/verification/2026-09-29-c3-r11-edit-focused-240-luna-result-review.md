# C3 R11 편집 집중 선택 결과 독립 검수

검수 대상은 R11 EditMode 분할 중 나머지 149개와 91개 행렬 실행의 선택 합집합이다. 이 결과로 C3 전체 또는 사전 C4를 수용하지 않는다.

## 판정

P0 0건, P1 0건이다. 나머지 선택의 실제 XML은 149/149 Passed이며 실제 편집기 종료 코드와 QA 도구 반환값은 모두 0이다. XML 사례 이름은 선택 원장과 정확히 일치하고 중복이 없다. 앞서 검수한 91개 행렬 선택과 합치면 원래 240개 Edit 선택과 정확히 일치하며 교집합은 0이다.

## 대조 근거

- 분할 계획 `artifacts/c3-r11-edit-disjoint-focus-plan.json` SHA-256 `6C6A5FEFA1915F479991B758200CE5A5CA051380D0CD5FC1C4F5B9D03475DA58`는 원래 선택 240개, 계획의 합집합 일치, 누락·중복 0을 기록한다. 91/149 두 파트의 실제 인자 길이는 14,455/24,170자로 제한 내다. 분할 선택 자체의 SHA-256은 각각 `B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED` 및 `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF`다.
- 분할 원장에 적힌 91개 이름과 149개 이름은 교집합 0, 합집합 240이다. 원래 전체 선택 `artifacts/c3-r11-editmode-focused-selection.json`의 240개 이름과도 정확히 같다. 실제 이번 XML의 149개 `fullname`은 나머지 선택 이름과 일치하며 사례 중복은 0이다.
- `artifacts/c3-r11-edit-remaining-r1.xml` SHA-256 `FF7DFC92C99E636C45E3DC4D2215C38DE6870D58953A02A4F6373F04586179E0`: NUnit 결과 Passed, total/passed/failed/skipped/inconclusive는 149/149/0/0/0이고 실행 시간은 2537.9236703초다. 실제 로그 `artifacts/c3-r11-edit-remaining-r1.log` SHA-256 `8AC831F294938B619C78FCB8FB7D992BD203BEA76DC67E497125AD6014D5901C`의 종료 줄은 `Test run completed. Exiting with code 0 (Ok). Run completed.`이다.
- 종료 검증 `artifacts/c3-r11-edit-remaining-r1-verification.json` SHA-256 `0DEC6C65A06FC49E24BBD3968DF805FC10DD820F9E826E2D0B49E105D05903BF`는 `ActualRunnerExitCode=0`, `Verified=true`, 기대·실제 149, 실패·누락·중복·입력 차이 0을 기록한다. QA 도구 반환 기록 `artifacts/c3-r11-edit-remaining-r1-qa-tool-return.json` SHA-256 `B52D346ED12892357F2AF4CDEAEA4FFA354760B68E322286EB59A5F008692CD6`도 반환 코드 0이다.
- 입력 전후 스냅샷은 각각 884개 항목이며 비교 차이는 0이다. 스냅샷 파일 SHA-256은 전 `4EA66059062338CD02259C3D27E37859F193B46E77C3665A280D9FA87153B2E0`, 후 `64CAE1EDFFBC2C084E42B59C6492B1D63F5C636DAEC774F98BD0B296D28665EC`다. 동결 R11 파일 14개의 현재 지문 대조도 차이 0이다.
- 선택 원장의 추적 범위는 `AC-M5D7QC3-001/002/004/006/009/010`이다. 이 선택은 여러 회귀 및 승인 기준 관련 Edit 사례를 포함하지만, 전체 수용 판정은 각 기준의 남은 필수 행과 나머지 선택 결과까지 모두 확인한 뒤에만 가능하다. 91개 행렬의 내부 377행 증거는 이전 별도 검수에서만 판정했으며 이 149개 실행에 377행 대조를 전용하지 않았다.

## 한계

이 결과와 앞서 검수한 91개 행렬의 합집합은 R11 EditMode 240개 선택의 실행 증거다. PlayMode 15개 부분 수용 기록은 별도이며 여기서 다시 판정하지 않는다. Required Edit 562, 그 밖의 집중 회귀와 전체 C3/C4 기준은 이 결과만으로 수용되지 않는다. Unity·컴파일·Git·네트워크 실행은 하지 않았고, 실행 중인 다음 큐 작업에는 개입하지 않았다.
