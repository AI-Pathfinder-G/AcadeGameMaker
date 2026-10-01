# 최초 코드 게시 현재 기준 v2 검증 인계

- 기준일: 2026-10-01.
- 사용자 결정: 현재 501개 후보의 바이트를 새 독립 출처 기준으로 검증한다. 과거 공백 428개와 기존 C4 변경 10파일의 미입증 전이 14개는 그대로 보존한다. [ADR-0038](../adr/0038-forward-provenance-for-initial-code-publication.md)이 이 방향을 기록한다.
- 상태: v2 원장 작성 및 아스트라의 로컬 기계 대조 완료. **루나 최종 결과 검토 미완료, 아스트라 제한 수용 미발급, 파일별 게시 허용 목록 공백, 코드 PR 없음.**

## 보존된 정확 자료

- [새 기준 v1 Approved 계약](../specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md) SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`; v1 JSON `artifacts/c4-initial-publication-forward-provenance-v1.json` SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`. [루나 v1 결과](../verification/2026-10-01-initial-code-publication-forward-provenance-result-luna-review.md) P0=0/P1=2다. v1 원본은 고치지 않았다.
- [v2 보정 Approved 계약](../specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction.md) SHA-256 `B6BCFD7C243053FC95E6BDF38E267CE13C01551B574AFDC239C33117C00A4E04`; 승인 기록 SHA-256 `D4941F094E0977ECFA1EB9083A8785DF5D388FFC07058B79EB6CECF9BEA6D49C`.
- v2 JSON `artifacts/c4-initial-publication-forward-provenance-v2.json` SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`, 설명 문서 SHA-256 `57B00701041DA392DF3C8C2367FFAA0770D7238CF1F6E29B2C0B487E6E21B066`, [테라 보고](../verification/2026-10-01-initial-code-publication-forward-provenance-v2-terra-report.md) SHA-256 `917EF828F2A52EFC92BAB97E4472E5EB13D573CD851837B2292A2B46D5E2D7CD`.
- v2는 참여 원장 과거 지문 참조 3개를 현재 독립 근거에서 역사 문맥으로 분리했고 남은 문서 근거 8개를 재확인했다고 기록한다. 501개 경로의 조사 전후 지문은 원래 후보 원장과 일치한다. 원격 조회 UTC `2026-10-01T02:53:17.986430Z`–`02:53:19.253916Z`, HEAD `9d3e2573cf4a1b6646c4332b349a2a988f8dd827`, 트리 `96530a7cce8aae4bb933ac0cbffb7e5cc85c744a`, 기존 7·부재 494·바이트 충돌 2를 기록했다.
- 아스트라의 별도 읽기 전용 대조에서 원격 HEAD와 트리를 다시 확인했고, 로컬에 있는 해당 커밋의 501경로·기존 7개 blob과 v2 행의 객체·SHA가 모두 일치했다. 이는 루나의 독립 결과 검토를 대신하지 않는다.

## 중단 지점과 재개 조건

루나(`gpt-6-luna`)는 v2의 로컬 501/501 지문, v1 불변, 과거 참조 분리, 현 문서 참조 96/96 및 원장 안의 원격 시각·HEAD·7/494 정합을 예비 대조했으나, 사용량 한도로 최종 검토 문서를 저장하기 전에 중단됐다. 이 작업에서 GPT-5.6이나 다른 모델로 독립 검수를 대체하거나 할당량 오류 후 재시도하지 않는다. 계약이 지정한 `docs/verification/2026-10-01-initial-code-publication-forward-provenance-v2-luna-result-review.md`는 아직 없다.

사용 가능한 루나 검수 경로가 회복되면 v2 원본의 정확 SHA와 501 실제 파일·문서 근거·원격 결속을 독립 재조회해 P0/P1 및 한계를 지정 경로에 기록한다. 그 뒤에만 아스트라가 새 기준 원장의 제한 수용 여부를 판단한다. 다음 내용·의존 조사 계약 초안 (`docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-content-dependency-draft.md`; 로컬 원본)은 v2 독립 P0/P1=0과 아스트라 제한 수용 전에는 Approved 전환·구현하지 않는다. 파일별 포함·제외, 깨끗한 복제의 Unity/QA 재검증, 원격 코드 게시도 각각 별도 게이트다. 현재 501개 행 모두 `PublicationApproved=false`다.
