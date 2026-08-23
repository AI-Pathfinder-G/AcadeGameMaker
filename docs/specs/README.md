# Specification System

스펙은 구현과 검수에 직접 사용할 수 있는 규범 문서다. 캐논과 ADR을 반복 설명하지 않고, 관찰 가능한 요구사항과 인수 기준으로 변환한다.

## 상태

| 상태 | 의미 | 구현 가능 여부 |
|---|---|---|
| Draft | 핵심 계약 또는 미결정이 남아 있음 | 불가 |
| Review | 계약과 기준이 작성되어 검토 중 | 불가 |
| Approved | Sol이 범위·계약·인수 기준을 승인함 | 가능 |
| Implemented | Terra가 구현과 단위 검증 증적을 제출함 | 통합 대기 |
| Verified | Luna 검증과 Sol 통합 승인이 끝남 | 완료 |

문서에 상태가 없으면 Draft로 취급한다.

## ID 규칙

- 스펙: `VD-NN`
- 요구사항: `REQ-{영역}-{NNN}`
- 인수 기준: `AC-{영역}-{NNN}`
- 미결정: `OD-{영역}-{NNN}`
- 검증 증적: `EV-{날짜}-{AC ID}`

영역 접두사는 `SCOPE`, `MOV`, `WT`, `COM`, `ROOM`, `RUN`, `CHOICE`, `UX`, `ART`, `PLAT`을 사용한다.

## 승인 조건

스펙을 Approved로 전환하려면 다음이 모두 있어야 한다.

- 범위와 비범위
- 입력, 출력, 소유 상태, 불변 조건을 설명한 Contract
- 고유 ID를 가진 요구사항
- Given/When/Then 또는 동등하게 측정 가능한 인수 기준
- 각 REQ를 적어도 하나의 AC가 검증하는 추적성
- 열려 있는 P0 미결정 없음
- 허용 파일, 금지 영역, 롤백 지점은 구현 작업 할당 시 기록
- Sol 승인자와 승인일

## 완료 조건

구현 완료는 코드 존재가 아니라 관련 AC가 증적으로 검증된 상태다. Terra는 단위 테스트와 구현 메모를 제출하고, Luna는 독립 검증 결과를 남기며, Sol은 계약 변경 여부와 통합 결과를 승인한다.

## 현재 패키지

- [15분 수직 데모 스펙 색인](./vertical-demo/00-spec-index.md)
- [추적성 매트릭스](./vertical-demo/TRACEABILITY.md)
- [열린 결정](./vertical-demo/OPEN-DECISIONS.md)
- [단위 스펙 템플릿](./templates/feature-spec-template.md)
- [작업 계약 템플릿](./templates/work-contract-template.md)
