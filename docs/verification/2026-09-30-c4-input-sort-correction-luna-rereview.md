# C4 입력 경로 정렬 보정 초안 재검토

- 검토 대상: `docs/specs/work-contracts/2026-09-30-c4-input-sort-correction-draft.md`
- 정확 SHA-256: `EFA7C6B6E8FCD51249CB9A1DE5F623EC6EE9FD0FAE7C64DF08DE18CA0B9FA507`
- 선행 검토: `docs/verification/2026-09-30-c4-input-sort-correction-luna-review.md` SHA-256 `C3FC722FB58CCE29938C65913F85E14C4369AB31E7BE91BBA7E03156E9C3D1E0`.
- 기준: Approved r4 SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, 동결 보정 승인 SHA-256 `61E2309CEFE24FB22648BB7A07FF1EEC1EA7BDE52CA9EA37A078F6C7780F5D7F`.
- 판정: P0 0, P1 0. 이 판정은 Draft 설계에만 해당하며 새 입력 목록 발급·실행 승인이 아니다.

갱신 초안은 두 선행 P1을 닫는다. 첫째, 실행에 직접 쓰이지 않는 입력 목록 v1만 새 실행 입력에서 제외하고, fixture가 직접 읽는 소스 원장 v1과 권위 원장 v2는 계속 포함한다고 분명히 했다. 이는 freeze-correction 승인 계약의 양쪽 원장 입력 조건 및 실제 Edit/Play fixture 경로와 일치한다. 둘째, v1의 967개 경로와 사유를 유지하면서 승인된 동결 보정 계약과 이 정렬 보정의 최종 Approved 계약을 추가하도록 해 권한 근거 누락을 해결한다. 새 집합 969개와 사유 969개, 정렬, 자기 목록·계획·실행 출력 제외 조건도 서로 양립한다.

기존 입력 목록을 불변 보존하고, `Paths`를 capture 도구의 `Sort-Object -CaseSensitive` 결과와 맞추며 `ReasonMap`을 같은 경로 순서로 재배치하는 보정은 최소 범위다. `PredecessorCapture`와 기존 884개 포함도 유지한다. 두 추가 문서의 최종 승인 경로를 실제 발급에서 정확히 사용하고 969개 경로·사유·정렬을 확인해야 한다.

새 입력 목록 v2는 아직 발급되지 않았고 Unity·컴파일·큐 실행은 수행하지 않았다. 후속 실물 산출물은 기존 v1, 두 Approved 계약, fixture 원장 v1/v2, 정확 경로·정렬·집합·사유 및 출력 배제 규칙과 다시 대조해야 한다.
