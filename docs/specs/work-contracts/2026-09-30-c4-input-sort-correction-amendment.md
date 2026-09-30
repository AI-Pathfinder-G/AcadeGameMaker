# C4 입력 목록 정렬 보정

- 상태: **Approved — 입력 목록 재발급만 승인, Unity 실행·통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 재검토 `docs/verification/2026-09-30-c4-input-sort-correction-luna-rereview.md` SHA-256 `0D53386786C816C80125987DC268207942BCF8BBB88B418DD49AAD497DE1DF58`, 초안 SHA-256 `EFA7C6B6E8FCD51249CB9A1DE5F623EC6EE9FD0FAE7C64DF08DE18CA0B9FA507`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4·QA r2 및 [동결 증거 보정](2026-09-30-c4-freeze-correction-amendment.md).

최초 정확 이름으로 발급한 `artifacts/c4-frozen-input-paths-v1.json`은 경로 967개·이유 967개와 이전 884개 보존은 충족하지만 .NET `StringComparer.Ordinal` 순서로 정렬했다. 승인된 `artifacts/c4-capture-inputs.ps1`은 PowerShell `Sort-Object -CaseSensitive` 순서를 요구하므로 첫 입력 캡처에서 목록 전체를 거절한다. 이 v1은 불변 보존하고 실행 계획에 결속하지 않는다. queue plan·Unity 실행은 아직 없다.

독립 검수와 아스트라 승인 후 `artifacts/c4-frozen-input-paths-v2.json`을 `FileMode.CreateNew`로 한 번 발급한다. 기존 입력 목록 v1의 경로 967개와 각 경로의 `ReasonMap` 값을 모두 유지하고, 승인된 동결 보정 계약 `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-amendment.md`와 본 정렬 보정의 최종 Approved 계약 두 경로를 `Added` 사유와 함께 추가한다. 새 집합과 이유는 각 969개다. `SchemaVersion`, `Trace`, `PredecessorCapture`, `ExcludedOutputPatterns`, `WholeAccepted:false`는 기존 입력 목록 v1과 같다. `Paths`를 승인된 capture 도구의 `Sort-Object -CaseSensitive` 순서로 배치하고 `ReasonMap`도 같은 경로 순서로 배치한다. 목록 자신·queue plan/result·실행 출력은 여전히 제외한다. 최종 queue plan·runner·verifier에는 입력 목록 v2의 정확 경로와 SHA를 결속한다. 이전 **입력 목록** `c4-frozen-input-paths-v1.json`은 비권위 이력으로 보존하며 실행에서 직접 읽지 않으므로 새 입력 목록에 넣지 않는다. 반면 Edit/Play fixture가 직접 읽는 **소스 원장** `c4-frozen-source-manifest-v1.json`과 실행 권위 소스 원장 v2는 둘 다 새 입력 목록에 계속 포함한다.

발급 뒤 독립 루나 검수는 기존 967개 경로 보존·승인 계약 두 경로 추가·이유·이전 884개·fixture의 소스 원장 v1 직접 입력 포함, 실제 capture 정렬식과의 완전 일치를 확인한다. 새 입력 파일 이외의 소스·선택·행·원장·도구를 변경하지 않는다. Unity 실행은 새 파일과 최종 계획의 별도 독립 검수 및 아스트라 배분 뒤에만 시작한다.
