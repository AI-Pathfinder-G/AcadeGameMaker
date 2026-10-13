# 원문 보존 도구 독립 검수

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 별도 `gpt-6-luna`의 읽기 전용 검수다. 소유 규격 `2026-10-02-publication-raw-lineage-snapshot.md`, `REQ-PLAT-059/060`, `AC-PLAT-057/058`에 따른다.

정확 입력: 도구 `qa/tools/New-PublicationRawLineageSnapshot.ps1` SHA-256 `AA71CF71B30A5457AE17E63F37A7ADC5036AB0518BA1D58D0E98F4CFBA6A91FE`, 보관 JSON `artifacts/c4-publication-raw-lineage-v1.json` SHA-256 `E3B86595404638BC15F3CB1281057044ACC731B3C6602542ADA6FAF8126BEAEA`, 시험 JSON `artifacts/c4-publication-raw-lineage-tool-tests-v1.json` SHA-256 `1DA7BD09702E0E4EA99DE2541A2620DED94459DB617D8E0717B0047817DE3469`, 테라 보고 `docs/verification/2026-10-02-publication-raw-lineage-terra-report.md` SHA-256 `E0EC727BEBC07855156F3398F5D3B1D8EDDCEDF7FA798C598BB6BA34B6DEB48B`.

최종 소스의 원시 지문, 최종/보강 전 실행 구분, 원장/엄격 도구 선검증, 정확534/전이25/무변환509, 출력 경계·기존파일·후보 안 출력 거절, 25 Base64 복원과 정확 원문/후보SHA·쌍수, 전후 동일 지문, 거짓 승인 플래그를 독립 대조한다. 코드·도구의 실제 호출은 하지 않고 기존 실행 기록을 검수한다. 정적 판독/기록만으로 확인되지 않는 조건은 미검증으로 남긴다. 과거14전이 복원·역사 검수 적합을 새 보관 사실과 혼동하지 않는다.

추가 설계 판독은 `docs/proposals/2026-10-02-q0-cleanclone-implementation-contract-draft.md` SHA-256 `CDD4710B3AAA297D2077332776872084726BB9C00A354F74E836AAA3D44B2006`의 Q0 현재/역사 분기·외부 계획 권위·실제Git 청결·자기지문 순환 방지에 한정한다. 이는 `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`의 역사 보존과 현재 바이트 검증 설계 검토이며 Draft를 Approved로 만들거나 구현을 허가하지 않는다. 필요한 차단조건을 짧게 별도 기록한다.

허용 출력은 새 `docs/verification/2026-10-02-raw-lineage-luna-review.md` 한 파일이다. 정확 AC·P0/P1·기록 증거의 범위와 설계 보완을 구별한다. 기존 원문·후보·소스·원장·Git·원격·Unity·빌드·시험 실행은 변경/수행하지 않는다. 오류 호출은 반복하지 않는다.
