# C4 선행 내부 행 출력의 버전 구분

- 상태: **Approved — 정확 선행 행 버전 분기 구현 승인, 실제 검증 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-29. 루나 [최종 독립 재검수](../../verification/2026-09-29-c4-legacy-row-output-r2-luna-review.md) SHA `A43CB05F103B20ED6288FC0FAAF6A1EA41FF3D566E1A21179A54B0CE20CA5513`의 정적 P0/P1=0/0과 원본240/91/149의 실제 배열·집합을 확인했다. 검수 원문 SHA `5E80F4B0513F07624A3B28C8E960A2D23DB9226474D31C086D170BA477E89062`는 `2026-09-29-c4-legacy-row-output-reviewed-draft.md`로 보존한다. 본문 미승인 문장은 초안 작성 시점의 이력이며 현재 개정 구현 권한은 이 상태를 따른다. 실제 Unity 실행·통합 수용은 미완료다.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-29.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-010`, `AC-M5D7QC3-002/004/010`.
- 선행: Approved [r4 실행 계약](2026-09-29-c4-r4-exact-implementation-amendment.md), [QA r2 규약](2026-09-29-c4-qa-evidence-protocol.md). 두 원문·새 C4 행 스키마·기존377 원장과 검사기를 수정하지 않는다.

이 문서가 Approved로 전환되면 QA r2의 `OutputPaths.RequiredRowComparison`과 새 C4 행 비교 결과 스키마 적용 조항에 대한 **아래 정확 Matrix91 분기만의 한정 개정**이 된다. 해당 분기의 실제 출력 스키마와 verify-run 처리에는 이 문서가 우선하고, 나머지 모든 실행과 신규 C4 행 스키마에는 QA r2를 그대로 적용한다. QA r2의 원문 바이트를 변경하지 않는 것은 역사 보존이며 이 분기의 개정 권한을 부정하는 뜻이 아니다. 아직 Draft이므로 현재 QA r2를 대신하는 구현 권한은 없다.

선행 matrix91 실행에는 기존 C3 내부377행만 있고 신규 C4 행은 없다. 이미 규정한 원장 버전별 검증을 정확 출력 슬롯에 연결하기 위해, 해당 실행에 한해서 계획의 `OutputPaths.RequiredRowComparison`은 기존 비교기의 원래 버전 JSON을 보관한다. 다른 출력 키·파일·행 스키마를 추가하거나 기존 JSON을 새 C4 스키마인 척 변환하지 않는다. 이 분기를 통해 새 C4 행이나 실제 NUnit 요구를 축소할 수 없다.

## 정확 선행 판별과 결속

분기는 고정 `PartId=Matrix91`, `Platform=EditMode`, 해당 선행 선택 지도의 `Edit240/Matrix91` 일치에 모두 결속한다. 임의 PartId 문자열이나 observed JSON 형태만으로 legacy 검증을 고르지 않는다. 선택의 정확 원본91 이름·순서·개수·원본 selection 지문 및 전체240의91+149 서로소 합집합은 불변이다. 전체240 배열은 원본 전체 선택의 순서를 보존하고 부분91·149 배열은 각각 원본 부분 선택의 순서를 보존한다. 실제 부분 배열을 이어붙인 순서가 전체 배열과 같다는 잘못된 조건은 요구하지 않는다. 서로소 이름 집합의 합집합이 전체240과 정확히 같고 각 원본 배열과 지문이 동일해야 한다. 실행 순서는 고정 계획 Runs에 결속한다. 다른 필수 선택의 허용 분할도 원래 전체 순서의 부분수열로 고정하고, 서로소 합집합이 원래 전체 이름 집합과 동일해야 한다. 동적 분할·재정렬·개명은 금지한다. 이 실행의 신규 `RequiredRowIds`는 빈 배열이지만 전역 새 C4 원장은 최종 소스의 실제 전체 행이며 빈 원장으로 대체하지 않는다. 같은 실행에 신규 C4 schema 행이 나타나면 예상 밖 증거로 실패한다.

고정 입력과 계획 Tools에는 원래377 예상 원장 `artifacts/c3-r11-required-decision-expected-rows.json` SHA `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`와 변경하지 않은 `artifacts/c3-verify-decision-rows.ps1` SHA `7BB2CC75B09979E77DE883EB495DCBE29780337861D7F37883F9EFB732CE27E2`를 결속한다. 원장·검사기 실제 원본을 실행 경계에서 재검증한다. 이름만 비슷한 새 원장·fallback·기존 행 재발급은 금지한다.

큐는 출력 부재를 검사하고 해당 원래 비교기를 `ExpectedPath/XmlPath/BeforePath/OutputPath` 정확 인자로 한 번 호출한다. OutputPath는 그 run의 RequiredRowComparison이다. 원래 비교기는 Set-Content를 사용하므로 큐가 사전 부재와 단일 writer를 강제하며 기존 출력이 있으면 호출하지 않는다. 같은 출력에 C4 비교기를 추가 호출해 덮어쓰지 않는다. 원래 비교기 자체의 byte·동작·schema·원본377 내용을 바꾸지 않는다.

## 새 실행 검증기의 정확 legacy 검증

신규 verify-run은 이 결속된 분기에만 원래 비교 결과의 정확 키·형식과 값 및 고정 예상 원장·실제 XML 지문을 검증한다. 성공은 원래 source가 동일하고 ExpectedRows/ObservedPlanned/ObservedPassed/MatchedRows가 모두377, ObservedFailed/UnknownStatusCount0, MissingOrInvalidIds/UnexpectedIds 빈 배열, EvidenceMatched=true이며 모든 내부 행의 부모 NUnit이 실제 Passed인 경우에만 인정한다. 새 capture의 Files에는 원래 비교기의 Source와 동일한 기존 시험 소스가 있어야 하고 원본 기대 SHA와 정확히 같아야 한다. 이를 새 C4 source 전체의 수용으로 확대하지 않는다.

나머지 모든 실행에서는 기존 QA r2의 새 C4 row comparison schema를 사용한다. 선행149/15/562/51/610처럼 그 실행의 새 C4 RequiredRowIds가 없는 경우에는 같은 전역 실제 C4 원장 결속을 유지하고 해당 부분 집합이 비어 있음을 기록한다. 이런 per-run 빈 행은 신규 전체 원장 개수나 통과한 C4 행 수가 아니다. 원래 부모 사례·실제 선택·source/입력·native/QA/outer0 검증은 모든 실행에서 동일하게 필수다. 미예상 신규 행·부모 실패·기존377 미도달을 빈 행으로 감추지 않는다.

검증 결과의 `RowComparisonBinding`은 실제 버전별 결과 파일 지문에 결속한다. 외부 독립 검수는 계획의 Matrix91 분기와377 대조를 확인하고 신규 C4 행과 합산하지 않는다. 구현·정확 원본 경로·계획 결속은 소스/도구 사전 검수에서 확인하며 실제 신규 소스 실행 전 원래 R11 통과를 재사용하지 않는다. 루나 독립 검수와 아스트라 Approved 이전에는 이 추가 분기를 구현하지 않는다.
