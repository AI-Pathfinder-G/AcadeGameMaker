# C4 메타 여섯 파일: 테라 자체 대조 기록

- 계약: `docs/specs/work-contracts/2026-10-01-c4-meta6-exact-byte-evidence.md` SHA-256 `710CE568BA359D1BA329D1DDBA16B138CAB9F5E97EF1D7BAF62C4DBA338D3FBB`.
- 승인: SHA-256 `975084441F598EF1F7CDDADBFBEE603C4150AD72153905293AC42E9058D4AE4D`; 승인본 루나 재검토: SHA-256 `4DC1B521134D2B13D688DDD74537C30272EAAAA69E772F1614C6432EBD2CFEF3`.
- JSON: `artifacts/c4-meta6-exact-byte-evidence-v1.json` SHA-256 `524F34CA8745E443C4A4A61179D234200A4B5289EF3CC2686EA6A07231C09937`.
- 문서: `artifacts/c4-meta6-exact-byte-evidence-v1.md` SHA-256 `24D14B5CF3461C5A73B1711AED4111FDDDA5754F07D4BF09DD1DA6B6C40E4BA8`.

착수 전 테라 출력 세 경로와 루나 검토 별도 한 경로가 모두 부재하고 v19 입력 1084·소스 193 경로 밖임을 확인했다. 신규 메타 6개 전체 SHA, GUID, 메타 직렬화 필드, 짝 source·asmdef의 실제 SHA와 조립을 읽기 전용으로 대조했다. 여섯 GUID 모두 Assets 메타에서 고유 소유다.

REQ-M5D7QC3-007/REQ-M5D7QC4-007 및 AC-M5D7QC3-009/010/AC-M5D7QC4-009/010 기준으로 r4 신규 메타 허용 문맥은 참 6개다. runtime 1개는 r4 50행 직접 표기, 시험 5개는 r4 139행 일반 규칙과 143~147행 정확 source 표의 결합이다. 이 참 판정은 메타 생성의 역사적 허용에 한정한다. 현재 바이트의 별도 독립 검수는 6개 모두 미입증이다. 후속 source 변경 계약을 메타 승인으로 자동 확장하지 않았고, 선행 22파일 변경 사슬은 열린 상태로 남겼다.

테라 자체 해시와 원장 생성은 독립 수용이 아니다. 루나가 여섯 실제 파일을 재해시하고 문맥·후속 범위를 검토한 다음 아스트라가 한정 판정해야 한다. 기존 소스·메타·QA·동결 원장·Git·원격은 변경하지 않았고 Unity·클린 복제·게시도 수행하지 않았다.
