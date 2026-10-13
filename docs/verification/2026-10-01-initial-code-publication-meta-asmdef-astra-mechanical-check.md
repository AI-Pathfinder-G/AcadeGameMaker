# 메타·조립 정의 예비 조사 아스트라 기계 대조

- 범위: [Approved 예비 조사 계약](../specs/work-contracts/2026-10-01-initial-code-publication-meta-asmdef-precheck.md) SHA-256 `0CA0EBA7BD48E5025286519F690AA7DD844DB3C3340A95EE68A9DE201CE9544F`의 `AC-PLAT-021..023`. 루나 독립 검수와 아스트라 결과 수용은 별도다.
- 테라 출력: JSON `artifacts/c4-initial-publication-meta-asmdef-precheck-v1.json` SHA-256 `9CF7837E600EE3862EA88F7C01D5FB8BE0A69C9FF836A5C9C8F6067B23CC509A`; 판독 문서 SHA-256 `E53963606F4D7C8473F3D11CE3307B17BF54351DC1EAA2FDC18C6ACFEAB292B7`; [테라 보고](2026-10-01-initial-code-publication-meta-asmdef-terra-report.md) SHA-256 `1FAA0BA0EEA7D929456D551657DCB6D6706C0F89382E6D30CCBDBCEA71D9A756`.

## 로컬 기계 대조

- `AC-PLAT-021`: JSON의 메타 254경로와 조립 정의 20경로가 v2의 해당 종류·확장자 경로 집합과 각각 정확히 일치했다. 274개 파일의 현재 지문은 출력 지문과 같고, 아스트라가 전체 501개를 별도 재조회한 결과도 v2 기준과 501/501 일치했다. 테라 기록의 시작·끝 501/501은 원장 내부 수치로 확인했다.
- `AC-PLAT-022`: 메타 종류는 파일 208·폴더 40·신규 C4 6이며 GUID 누락·중복은 0이다. 254개 메타의 짝 경로 실물은 모두 있다. 폴더 메타의 짝 폴더 40개는 501개 **파일** 후보 밖으로 기록됐다. 20개 조립 정의에서 직접 참조 74개를 기록해 후보 내부 단일 소유 61개, 범위 밖 후보 13개로 분리했다. 이전 33파일 조사에서 나온 GUID 형식 참조 406개 중 후보 메타의 단일 소유를 찾은 115개와 후보 소유 미발견 291개를 구분했다. 후보 소유 미발견은 엔진/패키지 결함 확정이 아니다.
- `AC-PLAT-023`: JSON 전체와 274개 행의 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`는 모두 거짓이다. 범위 밖 조립 참조 13개와 후보 소유 미발견 GUID 291개는 전체 의존 폐쇄를 막는 미해결 입력으로 남았다.

기계 대조에서 집합·지문·수량·게시 금지 플래그의 불일치는 발견하지 못했다. 이 결과로 `AC-M5D7QC3-009/010` 또는 `AC-M5D7QC4-009/010`의 수용 범위를 확대하지 않는다. 현재 원격 501 blob 결속, 실제 패키지 GUID 소유, 전체 파일별 내용·참조, 루나 독립 검수와 파일별 게시 결정은 아직 없다.
