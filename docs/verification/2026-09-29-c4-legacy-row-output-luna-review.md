# C4 legacy 행 출력 분기 독립 검토

검토 대상 Draft `docs/specs/work-contracts/2026-09-29-c4-legacy-row-output-amendment.md`, SHA-256 `9F0D047312A3194941FC2317092190E3AA2B667573ED99A12F4C33CEB57A840C`를 R4 및 Approved QA R2와 대조했다. 고정 legacy 원장 `artifacts/c3-r11-required-decision-expected-rows.json`은 제시 SHA `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`와 일치하고, 변경하지 않은 원래 검사기 `artifacts/c3-verify-decision-rows.ps1`도 `7BB2CC75B09979E77DE883EB495DCBE29780337861D7F37883F9EFB732CE27E2`와 일치한다.

판정: P0 0, P1 1. Matrix91에 한정된 원래 377행 검사 분기 자체는 정확한 PartId·플랫폼·선행선택 연결, 이름/순서/분할 결속, 원장·검사기 지문 고정, 기존 출력 부재 확인 및 단일 작성자 규칙으로 좁게 제한되어 있다. 새 C4 행이 legacy 실행에 섞이면 실패시키고, legacy 비교 결과를 C4 스키마로 바꾸지 않으며, 다른 실행은 C4 비교를 유지한다. 기존 377행 통과를 C4 전체 수용으로 확대하지 않는다는 경계도 적절하다.

남은 P1은 Approved QA R2의 결과 스키마와의 규범 연결이다. QA R2는 `RequiredRowComparison`을 실행 계획의 정확 출력 경로로 정의하고, 행 비교 결과의 키와 검증기 결속을 C4 스키마로 규정한다. 이번 초안은 같은 경로에 R11 legacy JSON을 두고 별도 legacy validator 분기를 쓰도록 허용하지만, QA R2의 출력 형식 규정에 대한 명시적 예외 또는 제한 개정으로 연결하지 않는다. 따라서 두 소유 규격을 함께 적용하면 RequiredRowComparison이 legacy 형식인지 C4 형식인지 모호해질 수 있다. QA R2에 정확한 Matrix91 분기를 추가하는 승인 개정 또는 이 초안이 QA R2의 해당 조항을 한정 보완한다는 명시적 규범 링크가 필요하다. 그 보정은 고정 조건과 단일 파일 경로를 유지하고, C4 비교 결과를 별도 경로로 추가하거나 출력 덮어쓰기를 허용하지 않아야 한다.

이 검토는 정적 계약 대조만 수행했다. 분기 구현, 검사기 호출, 신규 C4 fixture/selection 존재 여부, Unity 결과는 확인하지 않았다. 초안은 Draft이며 위 P1 및 아스트라 승인 전에 구현할 수 없다. 소스·QA·Unity·Git을 수정하거나 실행하지 않았다.
