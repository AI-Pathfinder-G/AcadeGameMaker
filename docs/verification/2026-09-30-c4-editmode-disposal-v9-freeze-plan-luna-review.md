# C4 편집 모드 복구 v9 동결·계획 사전 검토

- source manifest v9 SHA-256: `68397B84016E556013425BA0BEE3D223DCB4E14903EF96FF25113837A0A4C589` (117개 파일, 허용 변경 42개).
- input list v9 SHA-256: `F9075479A23314373E7199C9195468DDDBEDA520A84B4BA7D346B597D8BB32A8` (1001개 경로).
- queue plan v7 SHA-256: `E0FF3698F92DB05AFACC9A3BA50BC8479BCF7C7183AA54A94B51497B4B285AE5` (9회).
- 판정: P0 0건, P1 0건. 사전 정적 검토이며 Unity 실행은 하지 않았다.

manifest v9는 v8의 107개 경로와 순서를 모두 보존하고 10개 증거 파일을 추가했다. 기존 파일 중 SHA가 바뀐 것은 queue, `InputRouter`, Strict 감사 시험, Edit fixture, Q0 감사 시험, GPT 참여 원장 등 6개이며 각 파일의 현재 SHA가 manifest와 일치한다. 전체 117개 파일이 존재하고 모든 실제 SHA가 맞다. 허용 변경 42개에는 해당 변경 경로와 새 증거의 REQ 연결이 포함되어 있다. 참여 원장의 새 SHA도 실제 파일과 일치한다.

input v9는 v8의 990개 경로를 모두 유지하고 11개를 추가해 1001개가 됐다. 이전 경로의 순서 상대관계와 ReasonMap의 Disposition/Reason가 보존됐고 새 목록은 `Sort-Object -CaseSensitive` 순서와 일치한다. 새 manifest와 선택 v4/Hub v2/행 v4, 승인 계약, 검토 자료가 직접 입력에 포함됐다. 새 목록의 모든 경로가 존재한다. `Assets`, `Packages`, `ProjectSettings`, `qa` 아래에서 제외된 53개는 모두 승인된 `qa/results` 출력 경로이며, 출력 제외 규칙과 맞는다. source manifest와 input list 자체, 계획 및 실행 출력은 입력에 포함하지 않는다.

plan v7은 source/input manifest v9, checkpoint 행 v4, Edit 선택 v4와 Hub 선택 v2를 정확한 SHA로 참조한다. 선택 개수는 Edit 139, Play 32, Hub 5이며 나머지 분할은 91/149, 15, 562/51/610이다. 모든 9회 선택의 이름 개수와 기대 개수가 맞고 선택자·출력 경로가 각 실행과 결속된다. 72개 출력 경로와 `queue-result-v6.json`은 현재 없다.

첫 실행은 `c4-r5-focused-edit`이며 새 stem의 여덟 출력 경로를 쓴다. 첫 명령행 길이 19,566자를 포함해 9개 길이를 QA 도구의 실제 Windows 인자 직렬화 함수로 다시 계산했으며 계획 값과 모두 일치하고 32,767자보다 짧다. 사례 제한 180초, 관찰 제한 21,600초, 단일 프로젝트 mutex, 중단 우선 정책이 보존되고 `WholeAccepted`는 false다. v6 계획과 비교하면 첫 실행의 stem/NativeWitnessRunStem/selection/output만 갱신됐다. Hub 선택 지문이 바뀌어 세 번째 실행은 Hub 선택 v2로 갱신됐고, 그 외 나머지 7회는 동일하다. 이 한정 차이는 Strict 소스 SHA를 실행 선택에 반영한 것이다.

이 판정은 계획 검증에 한정된다. 실제 Unity 실행이나 테스트 통과, 전체 C4 수용을 의미하지 않는다. 별도 실행 승인 뒤 실제 XML·native/QA/outer 종료·입력 전후 자료를 다시 검증해야 한다.
