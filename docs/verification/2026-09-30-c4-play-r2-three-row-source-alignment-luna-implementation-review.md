# C4 Play 세 행 출처 정렬 구현 검수

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-play-r2-three-row-source-alignment-amendment.md`, SHA-256 `AA9FDB5A6055991E9726891167EE16209578862E3178C587CB44903E8505ADCD`.
- Play fixture: `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`, SHA-256 `2237DACC6344E0BDD2B31B24AEE6852C2912DCC47B20472D376FB76ED2DA4B7F`.
- 큐 도구: `artifacts/c4-final-validation-queue.ps1`, SHA-256 `60BA312E15CF8F0823AA47F281F789BC1D8DB64C47939EED06AACBEBC6E64446`.
- 범위: 위 두 변경 파일의 승인 범위와 실제 차이만 정적으로 검사했다. 원장·행·계획 생성이나 Unity 실행은 하지 않았다.

## 검수 결과

P0 0건, P1 0건. Fixture의 변경은 승인된 다섯 leaf와 현재 원장 참조 두 곳으로 한정된다. AC007 두 행 각각에서 `ExpectedGuard.permanentFaultRecorded`와 대응 `RequiredFacts`가 함께 `Yes`다. AC008 행은 receipt 값 `Absent`를 유지하고 certainty만 `SourceEstablished`다. Play ExpectedRows literal의 35행 중 대상은 Order 149, 150, 154이며, 나머지 행과 실제 관측 helper는 바꾸지 않았다.

역변환으로 변경 범위를 확인했다. 지정 두 행의 fault 값을 `No`로, AC008 certainty를 `Observed`로, 두 현재 원장 참조를 v12로 되돌리면 이전 fixture 지문 `F73BCC9EA0493214FC65B3005862DA5666F6EBEDD759C68CB9910D006D2C7B10`이 정확히 재현된다. 파일 내 원장 참조는 역사 v1 하나와 현재 v17 두 개다. 따라서 세 행의 근거 귀속·자기 결속 외 변경은 없다.

큐 도구는 결과 경로를 v10에서 v11로 바꾼 위치가 하나뿐이다. 이를 v10으로 역변환하면 이전 지문 `0AF89DF398BEEFD2A933B9BEA283597922CE25A4A4E4658B2F8CC794F1BEB301`이 정확히 재현된다. 잠금, 실패 중단, 반환값 처리와 나머지 큐 동작은 그대로다.

## 한계

현재 지문은 승인된 파일 변경만 확인한다. v17 manifest, 행 v9, 생성기 격리 재현 증거, 입력 v17 및 계획 v13은 아직 검수 대상이 아니며 Unity 재실행도 하지 않았다. 이 검수는 변경 구현의 한정 정적 판정으로, R2의 세 행 수용이나 전체 C4 수용을 뜻하지 않는다.
