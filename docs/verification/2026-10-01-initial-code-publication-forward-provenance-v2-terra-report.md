# 현재 출처 원장 v2 보정: 테라 자체 검증

- Approved 보정 계약 `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction.md` SHA-256 `B6BCFD7C243053FC95E6BDF38E267CE13C01551B574AFDC239C33117C00A4E04`.
- 원본 v1 SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE` 보존.
- v2 JSON `artifacts/c4-initial-publication-forward-provenance-v2.json` SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`.
- v2 설명 `artifacts/c4-initial-publication-forward-provenance-v2.md` SHA-256 `57B00701041DA392DF3C8C2367FFAA0770D7238CF1F6E29B2C0B487E6E21B066`.

501파일 경로와 현재 기준 SHA를 조사 전후 전수 대조했다. 누락·중복·바이트 변화는 0이다. 참여 원장 옛 SHA 참조 3건을 현재 독립근거에서 역사 문맥으로 이동했고 옛 전체 문서 바이트는 미검증이라고 기록했다. 남은 8문서의 실제 SHA·줄 접두·대상 SHA를 확인했으며 구현 증거 한 건을 독립 루나 검수로 오분류하지 않았다.

UTC 2026-10-01T02:53:17.986430Z부터 2026-10-01T02:53:19.253916Z까지 읽기 전용 원격 HEAD 두 번과 정확 트리·501경로·기존 blob 바이트를 같은 커밋에 결속했다. HEAD 변동은 없었다. 원격 기존7/부재494, 동일 경로 바이트 충돌2는 원문 바이트 기준으로 재계산했다. 게시 직전 원격 상태는 다시 조회해야 한다.

역사 직접61/공백428, C4 기존 변경 전이14/열린14/닫힌0은 변함없다. 모든 포함 후보는 미결정이고 `PublicationApproved=false`, `WholeAccepted=false`다. 이 자체 검증은 루나 독립 결과와 아스트라 한정 수용을 대신하지 않는다. 기존 코드·원장·동결·Git·원격은 변경하지 않았고 Unity·빌드·클린 복제·게시를 실행하지 않았다. 추적: REQ-M5D7QC3-001/005/006/007, REQ-M5D7QC4-001..007, AC-M5D7QC3-009/010, AC-M5D7QC4-009/010.
