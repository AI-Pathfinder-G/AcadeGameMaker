# C4 동결 보정 초안 재검토

- 검토 대상: `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-draft.md`
- 정확 SHA-256: `3F2FB070041510EC56CE615C0413E116157073B35A730BAA0BCBE67C4B08650B`
- 기준: Approved C4 r4 SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, 이전 독립 검토 `docs/verification/2026-09-30-c4-freeze-correction-luna-review.md` SHA-256 `0389858CF7B9B25D03EE805D5FFEBA52094B75853A6FC2D39BD6C4F7166EAFC6`.
- 판정: P0 0, P1 0. 이번 판정은 보정 설계에 한정하며 원장 발급·실행·C4 수용을 뜻하지 않는다.

갱신 초안은 앞선 P1을 닫는다. `ProfileResetExecutionBridgeV1.cs`와 이에 대응하는 Edit/Play 시험·fixture의 `RequirementIds`에 C1 원본 결과 경계의 `REQ-M5D7QC4-003` 및 C2 연결의 `REQ-M5D7QC4-005`를 요구한다. `ProfileResetMemoryCutoverV1.cs`와 대응 시험·fixture에는 `REQ-M5D7QC4-005`를 요구하고, 새 `.meta` 행에는 짝이 되는 파일의 요구사항과 생성 근거를 연결한다. 변경 경로별 근거를 기록하고 001..007 전체 요구사항 집합을 확인하도록 한 점은 v1의 일괄 다섯 ID 결손을 수정하는 데 충분하다.

v1의 `Files` 바이트 목록만 fixture가 읽고, v2가 실행 권위 원장이 되는 분리도 승인 r4의 구현 경계와 맞는다. v2의 `Files` 및 순서는 v1과 같게 유지하고 `AllowedChanges`만 보정한다. 실행 입력에는 두 원장을 함께 넣고 동등성을 확인하므로 fixture가 권한 누락을 재사용하지 않으면서도 시험 입력 바이트는 기존 경로를 유지한다.

새 산출물 이름 `artifacts/c4-predecessor-selection-name-map-v1.json`과 `artifacts/c4-frozen-input-paths-v1.json`은 r4의 정확 이름을 따른다. 준비용 기존 지도와 입력 목록은 덮어쓰지 않고 권위 입력으로 승격하지 않는다. 새 입력 목록에 이력상 기존 884개 경로를 유지하고 새 원장·지도·직접 읽는 자료를 추가하며 자기 자신·계획·실행 출력을 제외하고, 차이를 경로별 `ReasonMap`으로 기록하게 한 조건도 적절하다. 이전 목록·지도는 새 절차가 실제로 직접 읽는 때에만 입력으로 포함한다.

후속 v2 실물 검수에서는 C1/C2 관련 지정 경로와 대응 테스트·fixture·메타 파일의 `AllowedChanges`를 승인 근거에 따라 대조하고, 전체 `Files` 순서 동일성·두 원장 SHA·정확 산출물 경로·새 입력 목록의 집합 및 사유를 확인해야 한다. queue plan 발급과 Unity 실행은 이 검토 범위 밖이며 별도 승인 gate로 남는다. Unity·컴파일·큐 실행은 하지 않았다.
