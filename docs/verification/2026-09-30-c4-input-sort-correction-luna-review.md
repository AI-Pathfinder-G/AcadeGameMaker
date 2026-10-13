# C4 입력 경로 정렬 보정 초안 독립 검토

- 검토 대상: `docs/specs/work-contracts/2026-09-30-c4-input-sort-correction-draft.md`
- 정확 SHA-256: `9DC451917F2F20A98FEE1B6133D72D01CEF67B46478060F150A6760A499D2BA2`
- 대조 기준: Approved r4 SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, freeze-correction amendment SHA-256 `61E2309CEFE24FB22648BB7A07FF1EEC1EA7BDE52CA9EA37A078F6C7780F5D7F`, 입력 목록 v1 SHA-256 `3033E5D597B653FE37C02573E6270086CB4D0EE00233D82CA5AE4DCE419C4DD7`.
- 판정: P0 0, P1 2. 초안은 Draft이며 실행 근거가 아니다.

초안이 식별한 정렬 문제와 해결 방향은 정확하다. 현재 입력 목록은 .NET `StringComparer.Ordinal` 순서로 되어 있고, 승인된 `c4-capture-inputs.ps1`가 요구하는 PowerShell `Sort-Object -CaseSensitive` 순서와 다르다. 독립 재정렬 확인에서 두 번째 경로가 현재 `Assets/AcadeGameMaker.meta`, 도구 기대값은 `artifacts/c2-r36-selection-preflight.json`으로 갈렸다. v1을 보존하고 정렬만 바꾼 v2를 별도 `CreateNew`로 발급하며, 경로별 이유를 같은 집합으로 유지해 대조하는 범위는 적절하다.

P1 첫째는 초안이 v1을 새 실행 입력에서 제외하도록 한 점이다. 이는 앞서 승인된 freeze-correction amendment의 “모든 실행 입력에 v1과 v2를 모두 포함” 조건과 충돌한다. 실제 Edit fixture는 v1을 `File.ReadAllText`로 직접 읽는다(현재 파일의 824, 1102, 1163, 1289, 1329행); Play fixture도 827, 883, 976행에서 읽는다. v1은 비권위 변경 허용 목록이더라도 실행 중 검증되는 직접 입력이다. 새 경로 집합에서 제외하지 말고 두 원장 모두 유지해야 한다.

P1 둘째는 직전 산출물 검토에서 확인한 승인 근거 누락이 남는 점이다. 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-freeze-correction-amendment.md`는 v2 발급과 v1/v2 실행 권한을 정한 직접 규범이지만 현재 입력 목록에 없다. 단순 정렬만으로 경로 집합을 고정하면 누락은 해결되지 않는다. 이 계약을 입력으로 추가하고 `ReasonMap` 및 실제 개수에 반영하거나, 권한 문서가 실행 입력에 필요하지 않다는 상위 규범상 근거를 명시해야 한다. 추가하는 경우 v2 입력 경로 집합·count는 기존 967개와 달라지므로 초안의 집합 불변 조건도 함께 조정해야 한다.

후속 v2에서는 기존 884개 보존, 필요한 신규 직접 입력, 출력·자기 경로 제외, 경로-사유 일대일성, 도구와 동일한 정렬을 모두 재검수해야 한다. 이 검토에서는 Unity·컴파일·큐 실행을 하지 않았다.
