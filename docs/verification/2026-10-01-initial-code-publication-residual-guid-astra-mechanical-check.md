# 잔여 26개 식별자 참조 아스트라 기계 대조

- 범위: [Approved 현 환경 검색 계약](../specs/work-contracts/2026-10-01-initial-code-publication-residual-guid-precheck.md) SHA-256 `69541704F847C6C93F6E17B1CACD840022AEA9A3AF301185942E0913EAF5438B`의 `AC-PLAT-027..029`. 루나 독립 검수나 아스트라 결과 수용이 아니다.
- 테라 출력: JSON `artifacts/c4-initial-publication-residual-guid-precheck-v1.json` SHA-256 `EE593DBAEFF30B9354375B5A7F902E96FC323016A99757CC707D4A36C6836CB4`; 판독 문서 SHA-256 `25BECD73246CF4C3B0B4491DB30BBECCE9F1FCC1B6AB9D1B5C6995A4E6D3BD17`; [테라 보고](2026-10-01-initial-code-publication-residual-guid-terra-report.md) SHA-256 `CADDA990DAEFB8A4863F294780DAB2B23B3376C2B565F871FF2C82DD55D47F50`.

## 확인한 사실과 한계

- `AC-PLAT-027`: 패키지 예비 원장의 미발견 26참조 행과 테라 결과의 출발 경로·줄·GUID 집합이 정확히 같다. 고유 GUID는 9개다. 원장에는 501 후보의 조사 시작·끝 SHA가 각각 501/501 일치한다고 기록됐다.
- `AC-PLAT-028`: 26행의 실제 출발 파일·줄을 다시 읽어 저장 원문·GUID와 모두 일치함을 확인했다. 네 검색 루트의 테라 집계는 `Assets` 메타444, `Packages` 0, 현 `Library/PackageCache` 7,981, 설치된 Unity `6000.6.0f1`의 `Editor/Data` 18,602로 합계 27,027이다. 각 루트의 실제 경로·조사 시각·메타 집합 지문과 상태가 원장에 있다. 초기 캐시 순회에서 두 메타 읽기 오류가 있었으나 별도 직접 읽기와 전체 재순회로 최종 집계를 완성했다고 별도로 기록했다. 아스트라는 이 27,027개 집합 지문을 독립 전수 재계산하지 않았다.
- `AC-PLAT-029`: 26행은 조사한 네 루트의 `.meta`에서 모두 정확 소유 미발견으로 남는다. 영 접두 두 값 19행은 특수 값 후보일 뿐 소유 확정이 아니며, `Renderer2D.asset`의 비영 일곱 GUID는 기존 [루나 독립 검토](2026-09-29-c3-renderer2d-guid7-luna-review.md)의 미해석 게시 차단 P1을 유지한다. 이 검색으로 압축 내장 리소스·다른 설치·실제 Unity 가져오기 결과를 판단하지 않는다. 전체·각 행의 게시·의존 폐쇄·전체 수용·루나 검수·이식 가능 게시 증거 표시는 모두 거짓이다.

현재 기계 대조에서 입력 집합·원문 줄·수량·금지 플래그 불일치는 발견하지 못했다. 이 결과는 `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`의 기존 수용을 확대하거나 파일별 게시를 허가하지 않는다. 정확 버전의 격리된 Unity 가져오기 진단과 루나 독립 검수는 여전히 필요하다.
