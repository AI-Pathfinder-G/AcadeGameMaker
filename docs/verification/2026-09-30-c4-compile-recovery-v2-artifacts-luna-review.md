# C4 컴파일 복구 v2 선택·행 자료 독립 검수

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-test-compile-recovery-amendment.md` — SHA-256 `EF6B0966836FEA1896AF8BA8313D052F8C24E43F93DCD92D27B392377399B4CB`.
- 검수 자료 SHA-256: Edit 선택 v2 `C03B4585F3FBA2481F1B680C4F6DF3E1BBCBD1B1AFCB88E5655B30CE3A75F677`, Play 선택 v2 `D7A625CC34DFA8E00908A7BBC2157D153C762B64ADA73D848097938D9500E1B4`, 행 원장 v2 `DB8BC58AFB6926545EAD5EDC9C10DEBD754A52C8D66ECAF8EF6D63970C7A07AE`.
- 현재 소스 SHA-256: Edit fixture `0900831635B02D1F9CE0177A9FA8587C8CFFA2CD3E2D984B394825FA604CDB89`, Edit 시험 `9F051A4A1F5BE4418CF1BD6E9CA1477B9C3933AEB645FF7FABEC4F655EB6F17E`, Play fixture `23E91AF7F4DB54A2937E51E4BC1FEDDE677C702477591115D9562CF05B7A29B2`, Play 시험 `1CDA66FB944D17489D6003AED24FAB665E4979C74ABAD242D81C7914133DDAA6`.
- 판정 범위: 입력 자료와 선언의 정적 대조만. Unity 실행 및 전체 검증 수용은 아니다. P0 0건, P1 0건.

각 선택 v2는 대응 v1과 비교해 `SourceFiles`의 시험 소스 SHA만 변경됐다. 나머지 모든 필드는 값이 동일하며, Edit에는 139개, Play에는 32개의 기대 전체 이름이 유지된다. 현재 시험 소스의 각 기대 이름에 대응하는 메서드명이 존재한다. 선택 자료의 source 지문은 현재 시험 파일과 일치한다.

행 원장 v2는 v1과 비교해 `SourceFiles`만 달라졌다. 188개 `Rows`의 값과 순서, `EnumSources`, `Count`가 동일하다. 188개 행 ID와 순서는 각각 중복 없이 0부터 187까지 연속한다. 두 fixture 및 StrictAudit enum-source 세 파일의 원장 지문은 현재 파일과 일치한다. Play fixture 내장 JSON은 35행이며, 원장 v2에서 같은 ID의 Play 행 35개와 값·순서가 일치한다.

선택 자료의 기존 이름 목록·selector·개수·parser 근거가 그대로 보존되어, 이 자료 갱신만으로 시험 집합을 넓히거나 줄인 증거는 없다. 본 검토는 시험 코드가 실제 컴파일 또는 실행됐음을 입증하지 않으며, 큐의 새 plan/manifest/input 및 Unity XML 결과도 검토 대상에 포함되지 않았다.
