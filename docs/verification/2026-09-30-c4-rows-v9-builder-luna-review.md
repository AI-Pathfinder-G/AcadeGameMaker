# C4 checkpoint 행 v9 생성 검수

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-play-r2-three-row-source-alignment-amendment.md`, SHA-256 `AA9FDB5A6055991E9726891167EE16209578862E3178C587CB44903E8505ADCD`.
- 행 v9: `artifacts/c4-required-checkpoint-rows-v9.json`, SHA-256 `2769FA04AF42317D116C9C28D26EDA7948B58ECB598778EC9FE204538EB07EDE`.
- 생성 증거: `artifacts/c4-rows-v9-builder-evidence.json`, SHA-256 `19DD75A0F1E5159D91F5A23AC9CF8CC1166030783AA1666573C005A124C03F97`.
- 판정 범위: 자료·해시·원장 구조를 읽기 전용으로 대조했다. 생성기를 재실행하거나 Unity를 실행하지 않았다.

## 결과

P0 0건, P1 0건. 생성 증거의 원본 builder·검증기와 6개 정의/열거 소스 SHA는 현재 파일과 모두 일치한다. 증거는 격리 경로 `.c4-rows-v9-builder-r1`에서 고정 v1 출력의 사전 부재, 실제 호출과 종료 코드 0, 임시 생성 v1 SHA가 행 v9 SHA와 동일함, 전체 바이트·구조 동일 판정을 기록한다. 실행 당시 복사본 전후 SHA도 각 원본 SHA와 맞고, 기존 rows v1과 v8의 전후 지문은 불변이다. v9의 188개 행은 v8과 ID·순서를 보존한다.

v8과 v9를 독립 비교한 결과, top-level 차이는 Play fixture `SourceFiles` SHA 하나뿐이다. 행 내용은 지정한 다섯 leaf뿐이다. 두 AC007 행의 `ExpectedGuard.permanentFaultRecorded`와 대응 `RequiredFacts`가 `No`에서 `Yes`로 정렬됐고, AC008 행의 `authorityCorrelation.receipt` certainty가 값 `Absent`를 유지한 채 `Observed`에서 `SourceEstablished`로 바뀌었다. 나머지 185개 행 및 EnumSources 등 다른 top-level 항목은 동일하다.

현재 `artifacts/c4-required-checkpoint-rows-v1.json`의 SHA는 생성 증거의 역사 지문과 일치한다. `artifacts/c4-rows-v9-builder-evidence.json` 외에 v9 생성용 별도 `c4-*.log`는 발견되지 않았다.

## 한계

격리 임시 디렉터리와 그 안의 생성 v1 파일은 현재 남아 있지 않다. 따라서 임시 파일을 다시 읽어 바이트 비교할 수는 없으며, 생성 당시 비교 결과·동일 SHA는 생성 증거 JSON에 기록된 값으로 검증했다. 이 검수는 Unity 실행, v17 원장·계획 또는 전체 C4 수용을 판정하지 않는다.
