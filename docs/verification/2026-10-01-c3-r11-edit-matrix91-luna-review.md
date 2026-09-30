# R11 Edit 행렬 91개 독립 실행 결과

- 검토 범위: v15 큐의 네 번째 실행 `c4-r3-edit-matrix91`만 확인했다. 적용 기준은 AC-M5D7QC4-010과 공동 AC-M5D7QC3-007/008이다.
- 실제 XML `artifacts/c4-r3-edit-matrix91.xml` SHA `99DC1262B9FB163BDC9B325F50C2162EDBDB9F1E8E4C6D6D699BA985C817660B`: 선택된 91개 이름이 모두 한 번씩 존재했고 모두 Passed였다. Failed/Skipped/Inconclusive, 누락·초과·중복은 0이다.
- 기존 C3 행 대조 `artifacts/c4-r3-edit-matrix91-required-row-comparison.json` SHA `953F7A0C30F938ADB7D56516CD82FE945B625A557F4A0151A3C9C683FB236F09`: 377개 예상 행 중 계획·실행·통과·일치가 각각 377, 실패·누락·무효·예상 밖·미분류가 0이며 `SourceMatchesExecution=true`, `EvidenceMatched=true`다. 이는 377개 내부 증거행이며 NUnit 시험 개수로 계산하지 않았다.
- 실제 종료 자료: native 종료 관측 `6E2906F1DC659BF20159D8F5BB6DA72A10E39CC2A194F9C7AE01BC8D1E808FB1`에서 실제 종료 코드 0, QA 반환 `8D16C0EFF987357E59B501827155A457137B4AE5650BB0537D58144E6BB31D4E`에서 반환 코드 0, 외부 종료 코드 0. 검증 자료 `BA9CA6F5E0DEE14FA34C6E18084AAE314395624E92CD11C1590DFCD6D821C985`는 검증 완료로 기록한다.
- 입력 전후 자료는 각각 `FB948AFA45292CD56F53B00DB38AF43AF9036BE663A68D0C4FD1525C887F9BEC`, `81BD8E74CB40EA98186A81C54D3114F23A106DD0FF21123F8C0D82FFF543A86E`이며, 1084개 경로의 SHA 비교 차이는 0이다.
- 판정: 이 실행의 AC-M5D7QC3-007/008 관련 행렬 및 AC-M5D7QC4-010 실행 요건은 통과했다. 이는 v15 큐 일부 실행의 독립 결과일 뿐이다. 나머지 큐 실행, 전체 C4 및 통합 수용은 판정하지 않았다.