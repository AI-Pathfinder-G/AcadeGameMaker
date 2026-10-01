# 최초 코드 게시의 현재 바이트 출처 조사: 테라 보고

- Approved 계약: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md` SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`.
- 승인 기록 SHA-256 `FD52B1A556FE311E5511376537E189D484DAA388D474C5FBCEBBCC7AF6E184B5`.
- 501행 JSON: `artifacts/c4-initial-publication-forward-provenance-v1.json` SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`.
- 한국어 설명: `artifacts/c4-initial-publication-forward-provenance-v1.md` SHA-256 `632CA29B88F6B35A909B7C214308143F19D4EA63E820D3F9EC4CFDC0639CE5A8`.
- 조사 구간: 2026-10-01T02:35:45.768297Z부터 2026-10-01T02:35:46.152275Z까지.

501개 파일 경로는 고유하며 조사 전·후 실제 SHA가 원본 원장의 현재 SHA와 모두 일치했다. 원래 489개 직접 근거61/역사 공백428, C4 신규12, 기존 변경 전이14/열린 간격14/폐쇄0을 새 기준으로 재분류하거나 소급 승인하지 않았다. v19 소스·입력과 겹치는 파일은 각 원장 지문에 결속했고, 나머지 후보도 501 원장의 각 SHA를 기준으로 삼았다.

원격 main 커밋 `9d3e2573cf4a1b6646c4332b349a2a988f8dd827`의 트리 `96530a7cce8aae4bb933ac0cbffb7e5cc85c744a`에서 후보 파일별 존재·blob 식별자·SHA를 읽었다. 원격 기존7/부재494이고 같은 경로 바이트 충돌은2다: `artifacts/c3-required-decision-expected-rows.json`, `qa/tools/Test-QaCatalog.ps1`. 원격 조회의 정확 UTC 시각은 도구 출력에서 확보하지 못했으므로 미입증이다. 게시 직전 원격 상태 재확인이 필요하다.

파일별 현재 출처 상태의 집계: 471개 현재 SHA 기준 확보·독립 출처 미입증, 2개 원격 동일 경로 바이트 충돌·출처 미입증, 22개 현재 SHA 독립 문서 근거 있음·게시 의존성 미입증, 6개 현재 SHA·메타 제한 검수 확인·전체 출처 미입증. 포함·제외는 전부 미결정이며 게시 승인0이다. 메타/GUID·asmdef 및 원장의 직접 참조를 대조했지만 Unity 패키지 해석, 설정 적용, 프리팹·장면 참조, 깨끗한 복제의 가져오기·컴파일은 실행하지 않았다. 원격 동일 경로 충돌은 병합·덮어쓰기 허가가 아니다.

추적: REQ-M5D7QC3-001/005/006/007, REQ-M5D7QC4-001..007, AC-M5D7QC3-009/010, AC-M5D7QC4-009/010. 테라 자체 조사는 루나의 독립 재조회나 아스트라의 파일별 허용 판단을 대신하지 않는다. 원본 코드·메타·자산·설정·QA·동결·Git·원격을 수정하지 않았고 Unity·빌드·클린 복제·PR을 수행하지 않았다.
