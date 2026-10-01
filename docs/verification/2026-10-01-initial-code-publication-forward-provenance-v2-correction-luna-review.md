# 최초 코드 게시 출처 원장 v2 보정 초안: 루나 독립 재검토

- 검토 대상: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction-reviewed-draft.md`, SHA-256 `426DD223C2B83B71969D423194728E91BB0DB1288DE3D36B685A9EBA4AFA5B6C`.
- 보존된 원본 Draft: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction-draft.md`, SHA-256 `2DB944C117F38509F6CD17B24249F8BA493DC9619090D4147B7CA7FDF3AAC306`.
- 기준 계약: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md`, SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`.
- 수정 대상 v1 원장: `artifacts/c4-initial-publication-forward-provenance-v1.json`, SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`.
- 앞선 독립 결과: `docs/verification/2026-10-01-initial-code-publication-forward-provenance-result-luna-review.md`, SHA-256 `D964D98D5C8D01FC08CE9FFE820DD9D9E30D0B60EAB71A4150090B658D3C1E56`.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

## 이전 P1 두 건의 보정

수정본은 기존 Draft의 55자리 잘못된 지문만 실물 SHA-256 `84FDEE13D70A609A110E04ECD9C79C71F96FFD242073F008FD2733EB870073C7`로 고쳤다. 실제 원문과 대조해 확인했으며 나머지 본문은 바뀌지 않았다. 검토 이력 한 줄이 추가되어 본문 줄 수만 34에서 35로 늘었다. 다른 조항의 완화나 확장은 발견되지 않았다.

첫 번째 보정은 v1의 세 오래된 참여 원장 참조를 현재 근거에서 제거하고, 각 경로의 `HistoricalContextEvidence`에 과거 SHA·줄 번호·원래 발췌와 `HistoricalByteUnavailableOrUnverified=true`, `CurrentByteAuthority=false`로 분리한다. 과거 바이트를 복구하지 못하면 역사 근거는 미입증으로 남으며, SHA를 현 원장 SHA로 바꾸거나 v1을 덮어쓰지 않는다. 나머지 8개 참조는 문서별 현재 SHA·줄·발췌·대상 파일 SHA 언급을 재검증하도록 명시한다. 그 8개 경로·지문·줄을 실물과 재비교한 결과 8/8 문서 SHA, 8/8 줄과 발췌, 8/8 대상 파일 SHA 언급이 일치했다. 3/3/2의 경로별 구분도 v1의 실제 참조 배열과 맞는다.

두 번째 보정은 새 원격 조회를 UTC 시작 → 읽기 전용 `main` HEAD 조회 → 같은 HEAD의 트리와 후보 501개 경로·blob 확인 → HEAD 재조회 → UTC 종료 순으로 묶는다. 종료 시각을 `ObservedAtUtc`로 기록하고 시작/종료 필드를 따로 남기며, HEAD 변동이나 시간·트리·blob·바이트 결속 실패 시 원격값을 미입증으로 두고 파일별 포함 판단을 중단한다. 501개 경로를 모두 다시 대조하므로 기존 7/494 및 충돌2를 그대로 복사할 수 없다. 로컬 객체 쓰기·가져오기와 원격 변경을 금지한 범위도 유지한다. 이 설계는 앞선 P1의 시간 누락과 9d3… 관측을 재현할 수 없었던 문제에 필요한 폐쇄 절차를 구체화한다.

## 범위와 판정

501개 후보, 역사적 직접 근거61·공백428·현재 동일 직접 근거60, C4 신규12, 기존10 전이14·열린14·닫힌0, 모든 행 미결정 및 `PublicationApproved=false`가 그대로 요구된다. 새 v2 증거 네 경로는 v19 입력 목록 밖이며 모두 부재임을 확인했다. 클린 복제·Unity·빌드·게시 승인은 여전히 범위 밖이다.

**설계 검토 판정: P0=0, P1=0.** v1에서 확인한 두 P1은 보정안에 좁고 검증 가능한 방식으로 반영됐고, v1을 보존하며 출처 기준 B의 역사 공백을 소급 폐쇄하지 않는다. 다만 이는 Draft의 설계 적합성 판단이다. 새 원격 조회, 세 과거 참조 분리, 501개 전수 재검증이 실행되기 전에는 v1의 P1이 실제로 닫혔다고 할 수 없고 v2 증거를 수용할 수도 없다. 이번 검토는 원본·원장·입력·QA·Git·원격을 변경하거나 Unity·컴파일·클린 복제를 실행하지 않았다.
