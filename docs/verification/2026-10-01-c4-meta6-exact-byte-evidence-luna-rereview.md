# C4 신규 메타 여섯 파일 근거 계약 재검토

- 검토 대상 Approved 계약: `docs/specs/work-contracts/2026-10-01-c4-meta6-exact-byte-evidence.md` — SHA-256 `710CE568BA359D1BA329D1DDBA16B138CAB9F5E97EF1D7BAF62C4DBA338D3FBB`
- 비교 원본 Draft: SHA-256 `94AF2606ABC3C96C0F3570DA55E9063782114A7ACB43DF900322BE0268A8724E`
- 선행 검토·수용 기록: 설계 검토 `DEEEB0C9107140FA308088A6172062CEF43B07270EB0C74E493CAD51FF8CB9D8`, 22파일 독립 검토 `9EA4709166F861D5F5D7E0B590345C835E1B54F6C145B77B1FE4342A00A93AD5`, 제한 수용 `D9A02137DC82CF6DB454E39290DC5E80B4BE496388A572FA1C25ABC0ADE62278`.
- 동결 범위 대조: v19 원장 SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`, 입력 목록 SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`.

## 판정

P0 0건, P1 0건. Approved 본문과 Draft의 줄 단위 대조에서 실질 변경은 상태 전환과 출력 권한의 명시, 그리고 Draft·선행 검토·제한 수용 지문의 역사 기록에 한정된다. 여섯 meta 대상, 검사 항목, r4의 명시 경로와 일반 동반 규칙 구분, 출력 데이터 요구, 재해시와 독립 검토 조건은 유지됐다.

이전 P1은 닫혔다. Approved는 테라 산출물 세 경로와 루나 결과 검토 한 경로를 각각 분리해 지정한다. 루나 산출 경로 `docs/verification/2026-10-01-c4-meta6-exact-byte-evidence-luna-result-review.md`와 테라의 세 산출 경로, 이 재검토 기록 경로는 존재하지 않았고 v19의 Files 및 Paths에 포함되지 않았다. 따라서 새 검토 결과를 v19 동결 입력에 되넣는 순환도 없다. 승인 계약은 게시·클린 복제·Unity 실행 권한을 부여하지 않는다.

선행 근거의 경계도 보존된다. 22파일 원장의 제한 수용은 메타 여섯 파일의 현재 바이트 검수나 정확 경로 승인으로 승격되지 않는다. 이 계약은 읽기 전용 조사를 허용할 뿐이며 `PublicationApproved=false`, 게시 허용 목록 비어 있음, 전체 변경 사슬 미입증, C3/C4 전체 미수용을 그대로 둔다. 이 검토는 계약 차이와 경로 충돌만 확인했으며 실제 메타 바이트를 재해시하거나 Unity/QA를 실행하지 않았다.
