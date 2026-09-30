# C4 실행 전 동결 증거 보정

- 상태: **Approved — 동결 증거 보정만 승인, 실제 실행·통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 재검토 `docs/verification/2026-09-30-c4-freeze-correction-luna-rereview.md` SHA-256 `B85128084375C3D9B7BA87F5317A1617E8C6273E89069589AB705E6A50D42816`, 검토 대상 초안 SHA-256 `3F2FB070041510EC56CE615C0413E116157073B35A730BAA0BCBE67C4B08650B`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4, QA r2와 그 후속 승인 개정. 제품 동작·시험 행·실행 허용 범위는 바꾸지 않는다.

최초 발급한 `artifacts/c4-frozen-source-manifest-v1.json`은 `Files`의 바이트 결속은 유효하나 `AllowedChanges`가 모든 경로에 동일한 다섯 요구사항을 붙여 실제 C1/C2 연결의 `REQ-M5D7QC4-003/005` 추적을 누락했다. 이 파일은 불변 보존하고 승인 원장으로 사용하지 않는다. 아직 queue plan·Unity 실행·수용 결과는 발급되지 않았다.

독립 검수 후 새 `artifacts/c4-frozen-source-manifest-v2.json`을 `FileMode.CreateNew`로 한 번 발급한다. v2의 `Files`, `Count`, `PredecessorManifest`, `Trace`, `SchemaVersion`, `WholeAccepted:false`는 v1과 값과 순서가 같아야 한다. 바뀔 수 있는 것은 `AllowedChanges`뿐이다. 변경 경로마다 실제 적용 요구사항과 좁은 이유를 기재하고, 변경되지 않은 경로 또는 승인 밖 경로를 넣지 않는다. Q0 감사 파일의 승인 범위는 r4의 별도 Q0 pin 승인과 2026-09-30 독립 검수에만 한정한다. v1과 v2의 정확 바이트 SHA 및 `Files` 동일성 검사를 독립 검수 문서에 기록한다.

특히 C1 원본 결과를 실제로 소비·보존하는 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`와 해당 Edit/Play 시험·fixture의 `RequirementIds`에는 `REQ-M5D7QC4-003`을 반드시 넣는다. 같은 Bridge가 C1 proof를 C2에 전달하고 C2 원본을 결속하므로 `REQ-M5D7QC4-005`도 반드시 넣는다. C2 연결과 완료 사건을 다루는 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` 및 해당 Edit/Play 시험·fixture에는 `REQ-M5D7QC4-005`를 넣는다. 새 `.meta` 행에는 짝이 되는 source의 요구사항과 생성 근거를 명시한다. 발급 후 독립 검수는 이 경로별 연결과 001..007 전체 집합을 검사하며, 어떤 경로에도 근거 없는 요구사항을 붙이지 않는다.

기존 Edit/Play fixture가 `c4-frozen-source-manifest-v1.json`에서 읽는 것은 `Files`의 정확 바이트 결속뿐이다. fixture는 `AllowedChanges`를 읽거나 승인으로 판정하지 않는다. 최종 queue plan·runner·verifier의 권위 원장은 v2이며, 모든 실행의 입력 목록에는 v1과 v2를 모두 포함한다. v1과 v2의 `Files` 동등성이 깨지면 실행을 금지한다. 소스와 예상 행·선택은 변경하지 않으며 기존 정적 검수를 유지한다.

r4의 정확 생성 이름대로 `artifacts/c4-predecessor-selection-name-map-v1.json`과 `artifacts/c4-frozen-input-paths-v1.json`을 새로 발급한다. 이미 생성된 `artifacts/c4-predecessor-selection-map.json`과 `artifacts/c4-input-path-list-v1.json`은 준비 이력으로 불변 보존하며 권위 결속에 사용하지 않는다. 새 선행 지도는 이전 지도와 내용·순서가 정확 같고, 새 입력 목록은 기존 884 경로를 모두 보존하며 v1·v2 원장, 정확 이름의 선행 지도, 직접 읽는 자원을 포함한다. 입력 목록 자신·계획·실행 출력은 제외한다. 기존 목록 대비 추가·제외 경로를 `ReasonMap`과 독립 검수에 명시한다.

이 개정의 승인은 동결 출력의 정확한 재발급에만 적용한다. queue plan 발급과 실제 Unity 실행은 v2·새 지도·새 입력 목록을 루나가 독립 검수하고 아스트라가 별도 승인한 뒤 진행한다. 어떤 기존 파일도 덮어쓰거나 `WholeAccepted`를 true로 바꾸지 않는다.
