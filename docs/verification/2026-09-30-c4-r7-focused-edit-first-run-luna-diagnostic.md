# C4 첫 집중 Edit 실행 독립 진단

대상 실행: `c4-r7-focused-edit`\
계획: `artifacts/c4-final-validation-queue-plan-v11.json`, SHA-256 `01841B2C28F15B17370C1BAE84675F102EEB39A5C9E9608C913E352A7F33069D`\
판정: **P0 0건, P1 1건. 실행은 실패했으며 수용되지 않았다. 원인은 현 증거상 시험 fixture의 과거 고정 지문 불일치다.**

XML `artifacts/c4-r7-focused-edit.xml` SHA-256 `D26B47FA60328A910AD9A274DFD5CAC3897C12EB92465D9098D86943C9ECABAB`에는 139개 시험이 있고, 5 통과·134 실패·건너뜀 0·미결정 0이다. 이름은 선택 v6의 139개 전체와 정확히 일치하며 누락·초과·중복은 없다. 실패 134건은 모두 동일한 SHA 단언 오류다. 133건은 `C4ActualExecutionEditFixtureV1.AssertFrozenNoOpSingleCallSource` 1344행, 1건(`AC008_ActualC4CheckpointFailureNeverRestoresAuthority(17)`)은 `AssertFrozenBusySource` 854행에서 중단됐다. 두 helper는 과거 Bridge SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`를 기대하지만 실제 Bridge는 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. 후자는 현재 승인 원장 v15 및 현재 파일과 일치한다. 따라서 이 결과는 현재 테스트 픽스처의 고정값이 C4에서 승인된 Bridge 변경을 따라가지 못한 것이며, 제품 동작 결함을 입증하지 않는다. 실패가 helper의 사전 소스 지문 단언에서 끝나므로 나머지 동작/assertion은 이 실행에서 검증되지 않았다.

**P1 — fixture의 고정 원본 버전이 현재 승인된 Bridge와 충돌해 전체 집중 Edit 선택을 실패시킨다.** 재검증 전에는 이 XML을 합격으로 해석하거나 기대값을 완화해서는 안 된다. 후속 변경은 두 helper의 해당 원본 결속만 승인된 현행 원장에 맞추고, 기존 런타임 본문 토큰·횟수 단언과 모든 시험 이름·기대값을 유지하는 별도 승인 및 새 지문·선택·원장·계획이 필요하다.

네이티브 종료 관측 JSON은 `Status=Observed`, `ActualExitCode=2`, 사전 연결됨, 시험 프로세스 1개·자산 작업자 0개를 기록한다. 실행 로그와 XML은 존재한다. 전후 캡처는 각각 1,044개이며 모두 입력 v15 SHA `20F8EF85F3F14D3CF4991B728A06DF49E78BF0F4E01FB27ADC429901D922E4DE`에 결속되고, 파일 경로·SHA 쌍 차이는 0이다. 다만 직렬화된 캡처 파일 자체는 시각 필드 때문에 SHA가 서로 다르다.

큐 결과 v9 SHA-256 `DD5B6E5F348A6CF05DCEEA116E745AD705725086CB59968F949BD20CDC7D47DF`는 `Runs=[]`, 중단 stem `c4-r7-focused-edit`, `WholeAccepted=false`를 기록하며 하위 실행 exit 1로 실패 종료했다. 별도 `qa-tool-return`, `verification`, `required-row-comparison` 산출물은 생성되지 않아 QA 도구 자체 반환 코드는 이 저장 결과에서 독립 확인할 수 없다. 네이티브 exit 2와 NUnit XML/로그는 보존돼 있다. 이는 fail-stop 결과이며 전체 실행 수용이 아니다.

이 검토는 첫 집중 Edit 실행만 분류했다. 소스·fixture·원장·실행 증거를 수정하거나 재실행하지 않았다. 나머지 큐 실행은 이 실패로 중단된 상태이며 C4 전체 수용은 미판정이다.
