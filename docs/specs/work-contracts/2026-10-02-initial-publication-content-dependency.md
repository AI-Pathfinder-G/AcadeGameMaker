# 최초 코드 게시의 현재 내용·의존 조사

상태: **Approved**, 2026-10-02 아스트라 역할 승인. [v2 독립 검수의 제한 수용](../../approvals/2026-10-02-forward-provenance-v2-limited-acceptance.md) SHA-256 `F010AB40626F2A24BA8BB5BA89C649E569A3DCF1A086C202D5342B60FB80CD70`으로 출처 선행 관문이 충족됐다. 사용자의 후속 판단 위임과 재개 요청에 따라 제한 정식 판독을 승인한다.

[원래 설계 초안](2026-10-01-initial-code-publication-forward-content-dependency-draft.md) SHA-256 `8C65FDB3166E243216697A9C4058FE3AD1A0AD8235BCCF36638476D63461F23A`은 Draft로 보존한다. 이 새 계약은 그 초안의 조사 요구·501경로·종류별 판독·출력 형식을 아래 보충 조건과 함께 채택한다. `REQ-PLAT-015..019`, `AC-PLAT-013..017`은 **이 새 Approved 계약에서만** 적용된다. 과거 Draft를 소급 Approved로 바꾸지 않는다.

## 정확 범위와 보충 조건

구현·조사 역할의 별도 `gpt-6-sol`은 원래501 고유 경로와 v2 `ForwardBaselineSha256`를 시작·끝 실제 지문에 결속한다. 원본v2 SHA-256은 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`다. 각 행의 실제 내용 판독·적용 가능한 Approved REQ/AC/금지·정확 근거 줄·직접 참조·소유 조립/자산/패키지와 미확인 범위를 기록한다. 이름·폴더·과거 시험 통과만으로 내용 승인·의존 폐쇄를 판정하지 않는다. 실제 판독으로 닫히지 않는 행은 미입증으로 남긴다.

현재 사용자가 선택한 게시 기준은 Git 정규화 바이트다. 이 조사501 원문과505/534 새후보를 혼동하지 않는다.505 원장 SHA `9E23DCFBFAA7C1B3C2AE006FBD6BDEC58F456974E7C4C878E9F38416C654762C`를 추가 읽기 근거로만 사용해 원본SHA와 정확정규화SHA를 별도 열로 기록한다. 원문 내용 분석·역사review가 정규화 바이트의 신규 독립 검수를 대신하지 않는다. 추가29·새 검사기·후속 fixture 변경을501 결과에 자동 포함하지 않는다. 정확 후보 확장은 별도다.

원격은 새 UTC 시작→기본 가지HEAD→그 HEAD의 전체 트리/해당501 존재파일의blob/바이트→HEAD 재조회→UTC 종료 순서로 읽는다. 공개 GitHub의 연결 도구 GET을 사용할 수 있고 실제 수단·개정·시각·전체 트리/동일HEAD·blob/바이트를 결과에 기록한다. 로컬Git 읽기는 허용하나 fetch·객체 생성·인덱스/설정·인증·원격 쓰기는 금지한다. HEAD 이동·잘린 트리·누락된blob/바이트면 해당 관측은 미입증이다. 과거v2의7/494·두 원문 충돌 또는505의10/495·세 정규화 충돌 숫자를 복사하지 않고501 정확집합에서 다시 계산한다. 원문과 정규화 충돌을 분리한다.

후보 밖 파일·캐시 패키지를 판독에 사용하면 정확 경로·SHA·발견 위치/대상/종류·권한 한계를 별도 기록한다. 알려진Renderer2D P1·범위 밖 검증 입력·Q0 작업상태와깨끗한복제 충돌·역사14전이 미입증은 독립 결론 전까지 보존한다. 미확인 참조가 도달하는 행의 의존 폐쇄는 거짓이다. 조사자가 전체 의존 폐쇄나 파일별 게시 허용을 발급하지 않는다.

## 출력과 실행 한계

허용 새 출력은 `artifacts/c4-initial-publication-content-dependency-v1.json`, 같은 이름 `.md`, `docs/verification/2026-10-02-initial-publication-content-dependency-terra-report.md`다. 별도 독립 루나 출력은 후속 검수 계약에서 지정한다. 현재 원본·후보·과거 증거·원장·제품·QA·Git·원격을 수정하지 않는다. 파일 복사·Unity/빌드/시험 실행·fixture 변경·게시도 허용하지 않는다.

501행 모두 `PublicationApproved=false`다. 내용 판독 사실과 의미상 미입증을 구별하며 총판정 `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`, `DependencyClosure`는 거짓이다. 아스트라의 파일별 허용과 실제 복제/회귀·루나 독립 검수가 별도로 남는다. quota/auth/model 오류는 한 번 보고하고 반복하지 않는다.
