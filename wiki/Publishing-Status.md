# Wiki Publishing Status

## 2026-09-28 게시 준비

이번 정리본은 문서 변경 요청의 `codex/documentation-checkpoint-20260928` 브랜치를 원본으로 사용한다. 아직 병합 전이므로 최신 계약 링크는 해당 브랜치로 연결한다. 아래 2026-08-24 게시 완료·개정 기록은 당시 이력이며 이번 게시의 성공 증거가 아니다. 실제 게시 후 개정과 검수일을 별도 기록한다.

## Repository publication

- Repository: [AI-Pathfinder-G/AcadeGameMaker](https://github.com/AI-Pathfinder-G/AcadeGameMaker)
- Visibility: Public
- Default branch: `main`
- Initial documentation revision: `5b8d090`
- Latest narrative documentation revision: `7fb211f`
- Wiki package: `wiki/`
- Entry page: `Home.md`
- Navigation: `_Sidebar.md`
- Repository package status: Published
- Checked: 2026-08-24 (Asia/Seoul)

## Native GitHub Wiki status

- Wiki: [AcadeGameMaker Wiki](https://github.com/AI-Pathfinder-G/AcadeGameMaker/wiki)
- Status: Published
- Initial full-package revision: `ca9953376a47d6aa3690d8675004440f96573e63`
- Character-first narrative package revision: `1a0c76b48dde2e2c94b4a88b81cd6ae03a178d6e`
- Published pages: 13 reader pages and `_Sidebar.md`
- Verified: Git push success, 13 reader-page file count, local Markdown links, Wiki-style page targets, and authoritative source links
- Checked: 2026-08-24 (Asia/Seoul)

Wiki는 권위 문서가 아니라 발행·탐색 계층이다. 충돌 시 `CONTEXT.md`, `docs/canon/`, 현재 ADR, Approved 스펙 순으로 원본을 따른다.

## Republish procedure

1. 권위 문서 변경을 먼저 `main`에 반영한다.
2. 대응하는 `wiki/` 요약과 원본 링크를 갱신한다.
3. `wiki/` 전체를 Wiki 전용 Git 저장소에 게시한다.
4. Home, Sidebar, 모든 권위 문서 링크를 검수한다.
5. 게시 revision과 검수일을 이 페이지에 기록한다.
