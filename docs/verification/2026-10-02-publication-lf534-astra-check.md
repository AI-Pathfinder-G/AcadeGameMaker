# 후보534개와 변환수 보완 기계 대조

추적: `REQ-PLAT-051..053/057/058`, `AC-PLAT-049..051/055/056`. 534 v1 원장（`../../artifacts/c4-publication-lf534-baseline-v1.json`；로컬 보존 자료） SHA-256 `B0EBCE1009CDCD01C18596C1271F17AB8181F934B36A2149BA37A8A5F3595E41`의534행에 대해 현재 원본과 `output/publication-lf534` 후보SHA를 대조했다. 정확505 경로와 선별29 경로의 합집합을 실제 후보 파일 집합과 비교해 집합 차이0, 원본/후보SHA 불일치0이다.

v1의 변환수 누락은 [별도 Approved 계약](../specs/work-contracts/2026-10-02-lf534-transition-count-supplement.md)으로 새v2에 보완했다. v2（`../../artifacts/c4-publication-lf534-baseline-v2.json`；로컬 보존 자료） SHA-256은 `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`다. v1과v2의534행을 정확 경로로 짝지어 기존 모든 행 필드를 비교했다. 허용된 전이명 표현 통일 외 기존 필드 차이는0이고, 동일509·CRLF→LF25이다. 원본v1의 SHA는 그대로 보존했다.

새 검사기는 별도 계약에서 실제 바이트 변환수와 Git 객체·정확 경로 집합을 엄격 대조한다. 이 문서의 기계 대조는 그 자체 검사나 루나 독립 검수를 대신하지 않는다. 역사 내부pin·시험 원장·제품 소스는 변경하지 않았다. 전체 회귀·게시·의존 폐쇄·루나 독립 수용·깨끗한 복제 실행을 발급하지 않는다.
