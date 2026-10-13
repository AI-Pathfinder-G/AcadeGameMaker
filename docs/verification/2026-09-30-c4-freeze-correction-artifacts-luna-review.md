# C4 동결 보정 산출물 독립 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-amendment.md`, SHA-256 `61E2309CEFE24FB22648BB7A07FF1EEC1EA7BDE52CA9EA37A078F6C7780F5D7F`.
- v1 원장: `artifacts/c4-frozen-source-manifest-v1.json`, SHA-256 `C31A610613CA4CD5F5F3B69720F08158CEECCE090A665C7496DCA8D57528A4EF`.
- v2 원장: `artifacts/c4-frozen-source-manifest-v2.json`, SHA-256 `52E4A870B9892C586B8AFA94C032B467342EC913B8E1071B4A30B10193DA197E`.
- 선행 지도: `artifacts/c4-predecessor-selection-name-map-v1.json`, SHA-256 `7647BB0AB3DD488080A53794C8C27DB0D7600FD7D3F79FB21E40CA9102851C91`.
- 입력 목록: `artifacts/c4-frozen-input-paths-v1.json`, SHA-256 `3033E5D597B653FE37C02573E6270086CB4D0EE00233D82CA5AE4DCE419C4DD7`.
- 판정: P0 0, P1 2. 검토는 정적 산출물 검증이며 Unity 실행이나 C4 통합 수용이 아니다.

v2의 전체 `Files`는 v1과 항목·순서까지 일치한다. `SchemaVersion`, `Trace`, `Count`, `PredecessorManifest`, `WholeAccepted`도 같고, 달라진 필드는 `AllowedChanges`뿐이다. 두 원장 모두 `WholeAccepted:false`다. v2는 v1과 같은 20개 경로만 허용한다. `AllowedChanges` 전체에서 REQ001..007이 모두 나타나며, Bridge·C1/C2 시험 및 fixture에는 003/005가, `ProfileResetMemoryCutoverV1.cs`에는 005가 연결된다. 메타 파일도 짝이 되는 소스의 요구사항을 따른다. Q0 감사 시험은 별도 pin 변경 범위인 002/006/007만 갖고, Q0 변경을 런타임 전반 권한으로 넓히지 않았다.

새 선행 지도는 기존 지도와 객체 내용·순서가 같다. 다섯 선택, 기존 qualified name, 원본 경로·SHA가 유지되고 `WholeAccepted:false`다. 이름도 승인 계약이 요구한 `c4-predecessor-selection-name-map-v1.json`이다.

새 입력 목록은 967개의 고유 경로와 967개의 고유 사유를 가지며, 기존 R11 884개를 모두 보존하고 83개를 추가한다. 추가 항목에는 v1/v2 원장과 새 지도가 있으며, v1/v2 모두 목록에 있다. `ReasonMap`의 추가·유지 분류는 개수와 경로 집합이 일치한다. 현재 `Assets`, `Packages`, `ProjectSettings`, `qa`를 승인된 제외 규칙으로 열거한 경로 886개는 전부 포함되고 누락이 없다. 자기 목록·queue 계획/결과·테스트 출력 패턴은 입력 경로에 포함되지 않는다. 기존 준비용 지도와 입력 목록은 파일로 보존되며 권위 대체물로 선언되지 않았다.

P1 첫째는 새 목록의 권한 근거 누락이다. 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-amendment.md`는 v2 재발급 및 입력 목록 재생성의 직접 규범인데, 새 입력 목록의 `Paths`와 `ReasonMap`에 없다. 현재 목록은 이전 R4 계약과 QA 규약은 포함하지만, 이번 보정 승인 문서는 포함하지 않는다. plan/실행의 권위 원장 선택과 AllowedChanges 해석을 재현할 수 있도록 이 승인 계약을 새 입력으로 추가하고 정확 사유를 기록해야 한다.

P1 둘째는 입력 경로 순서다. `c4-capture-inputs.ps1`는 `Paths`를 `Sort-Object -CaseSensitive`한 결과와 원래 배열을 정확 비교한다. 현재 967개 경로는 정렬되지 않았다. 첫 불일치는 두 번째 항목에서 발생한다. 현재값은 `Assets/AcadeGameMaker.meta`이고, 검증기가 기대하는 정렬값은 `artifacts/c2-r36-selection-preflight.json`이다. 이대로면 입력 캡처가 `입력 경로 정렬 불일치`로 중단된다. 목록을 도구와 같은 대소문자 구분 정렬로 다시 발급하고 `ReasonMap`의 집합 결속을 유지해야 한다. 두 P1이 닫히기 전에는 queue plan 발급이나 실행 입력 동결을 승인할 수 없다.

검토에서 확인한 속성은 경로 존재·중복 및 기준 목록 대비 집합 대조다. Unity, 컴파일, queue 및 외부 작업은 수행하지 않았다.
