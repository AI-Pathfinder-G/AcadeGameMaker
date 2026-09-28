# C3L 최종 회귀 독립 수용 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 최종 원장: `artifacts/c3l-final-regression-evidence.json`
- 원장 SHA-256: `13B8AEC521EF350714BABE9C4D83A040CDC071A50CA2C83A4616CFA3134025C8`
- 기준: Approved C3L `AC-M5D7QC3L-001..005`

## 실행 및 집합 판정

R6는 정확한 단일 실패행을 동일 소스와 기본 180초 제한으로 재실행하여 1/1 통과, 실패·건너뜀·판정보류 0, 실제 소유 Editor 종료 코드 0을 기록했다. qualified name 차이와 입력 전후 차이는 0이다. R6 XML SHA-256은 `80107DBA94E17AFB39C84DB1964A64034AD20246C2B10B4111623E85BDA7F7C9`이다.

조립 원장은 R4 통과 561행, R6 보정 1행, R5 worker 51행을 합쳐 613개 고유 이름을 만들고, 이전 R35 기대 선택 613개와 이름 차이 0·중복 0을 확인했다. R3/R4/R5/R6의 전후 입력 목록은 각각 874개이며 모두 차이 0이다. R5 XML은 51/51 통과이고 SHA-256은 `2A4A4A21D891EC10B95BDC6E226BD049221F1AE7C1EFDD2A74AADD9126E59FC6`이다.

R4의 원래 562개 실행은 561 통과와 1 timeout 실패였고 실제 종료 코드 2였다. 원장은 이 실패를 제거하지 않고 `R4 전체 통과를 주장하지 않는다`고 보존한다. 따라서 613개 ledger는 현재 동결 소스의 행 단위 분할 증거이며, R4 XML 자체의 결과를 수정한 것이 아니다.

## AC 범위

- **AC-M5D7QC3L-001:** R3 focused 11행에서 원본 본문 보존, foreign receipt/actions, 정적 issuer/API 경계를 확인했고, 613개 조립 원장은 required 회귀 집합의 현재 소스 행 증거를 추가한다.
- **AC-M5D7QC3L-002:** R3의 실제 held-lock timeout·early release 행과 R5/R6 원장은 동결 소스 회귀를 보강한다. 직접 생성한 timeout이나 반사 발급은 사용하지 않았다.
- **AC-M5D7QC3L-003:** R3에서 root-file·lock-directory·barrier·unreadable·relative/root 경계와 timeout provenance 변조 거부를 확인했다.
- **AC-M5D7QC3L-004:** R3에서 실제 세 leaf capture, projection/identity agreement, reflection 변조 fail-closed를 확인했다.
- **AC-M5D7QC3L-005:** R3 focused 11행과 현재 소스의 required 613행은 실패·건너뜀·판정보류 0으로 구성되며 Luna의 현재 독립 판정은 P0=0/P1=0이다.

정적 보존 검수는 기존 service·disk legacy 본문 재구성 지문과 lower/API 경계를 그대로 유지한다. 새 원장은 실행 증거를 조립할 뿐 source/test를 수정하지 않았다.

## 수용 경계

본 기록은 **C3L `AC-M5D7QC3L-001..005` 범위의 독립 수용 가능 판정**이다. C3 전체 `Verified`, C3 `AC-M5D7QC3-010`, C3 owner/rearm 최종 회귀, C4 수용 또는 구현 권한은 주장하지 않는다. R4 timeout 이력은 보존되며, 이후 C3 owner/rearm 변경이 생기면 필요한 EditMode·PlayMode 회귀를 새 소스로 다시 수행해야 한다.

최종 판정: **C3L AC-001..005 P0=0/P1=0, 현재 범위 수용 가능**. 코드·시험·계약은 수정하지 않았다.
