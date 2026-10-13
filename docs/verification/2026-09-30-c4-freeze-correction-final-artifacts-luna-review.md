# C4 동결 보정 최종 산출물 독립 검토

- 동결 보정 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-amendment.md`, SHA-256 `61E2309CEFE24FB22648BB7A07FF1EEC1EA7BDE52CA9EA37A078F6C7780F5D7F`.
- 정렬 보정 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-input-sort-correction-amendment.md`, SHA-256 `7FA32AFB06B66121C4686C5996104CD434C60BDE31E620D6DA88CF4E2668EE15`.
- v1 원장: `artifacts/c4-frozen-source-manifest-v1.json`, SHA-256 `C31A610613CA4CD5F5F3B69720F08158CEECCE090A665C7496DCA8D57528A4EF`.
- v2 원장: `artifacts/c4-frozen-source-manifest-v2.json`, SHA-256 `52E4A870B9892C586B8AFA94C032B467342EC913B8E1071B4A30B10193DA197E`.
- 선행 지도: `artifacts/c4-predecessor-selection-name-map-v1.json`, SHA-256 `7647BB0AB3DD488080A53794C8C27DB0D7600FD7D3F79FB21E40CA9102851C91`.
- 입력 목록 v2: `artifacts/c4-frozen-input-paths-v2.json`, SHA-256 `289B265ADA3583C60E1950CE95FDCCFD49D99E9FB43C58D4818119C53C8C3BDC`.
- 판정: P0 0, P1 0. queue plan·Unity 실행·C4 통합 수용은 아직 아니다.

v1과 v2의 `Files` 102개는 내용·순서가 같다. 상위 `SchemaVersion`, `Trace`, `Count`, `PredecessorManifest`, `WholeAccepted`도 동일하고, v2에서 바뀐 것은 `AllowedChanges`뿐이다. v2의 파일별 SHA 102개를 현재 작업 파일과 비교해 불일치는 0개였다. v1/v2 모두 `WholeAccepted:false`이며, v2의 20개 허용 경로는 기존 v1 경로 안에 있다. 허용 요구사항의 전체 집합은 001..007이고, Bridge 및 대응 Edit/Play 시험은 003/005를 포함한다. C2 cutover는 005를 포함한다. 짝 메타데이터도 대응 source의 추적을 유지하며 Q0 감사 경로는 별도 승인된 pin만 허용한다.

새 선행 지도는 이전 준비 지도와 값·순서가 같고, 선택 5개 및 원본 경로·SHA를 보존한다. 요구 이름 `artifacts/c4-predecessor-selection-name-map-v1.json`을 사용하고 `WholeAccepted:false`다.

입력 목록 v2는 969개의 고유 경로와 969개의 고유 사유를 가진다. 기존 967개 경로가 전부 유지됐고, 두 승인 계약만 추가됐다. 기존 R11 캡처의 884개 경로도 모두 포함된다. 추가 항목 85개와 유지 항목 884개의 경로·사유 분류가 정확히 대응한다. 입력 목록에는 fixture가 읽는 소스 원장 v1, 실행 권위 원장 v2, 정확한 선행 지도, 두 승인 계약이 포함된다. 실행에서 읽지 않는 이전 입력 목록 v1은 포함하지 않고, 현재 자기 경로·queue 계획/결과·실행 출력도 제외한다.

`Paths`와 `ReasonMap.Path`는 모두 `Sort-Object -CaseSensitive` 순서와 정확히 일치한다. 이전 884개 캡처를 선행 자료와 대조해 누락이 없고, 현재 `Assets`, `Packages`, `ProjectSettings`, `qa` 아래 승인된 제외 규칙을 적용해 열거한 886개 파일도 전부 포함됐다. 입력 경로 중 존재하지 않는 파일, 출력 패턴과 일치하는 경로, 중복 경로·사유는 없었다. 목록의 `PredecessorCapture`는 승인된 기존 884개 capture SHA에 결속하며 `WholeAccepted:false`다.

실물 동결 산출물의 차단 결함은 발견되지 않았다. 다음 단계에서도 queue plan은 정확한 v2 원장·지도·입력 목록 SHA를 고정하고, 별도 검토와 Astra 승인을 받은 후에만 실행해야 한다. 이번 검토는 파일·경로·해시의 정적 대조만 수행했으며 Unity, 컴파일, queue 실행은 하지 않았다.
