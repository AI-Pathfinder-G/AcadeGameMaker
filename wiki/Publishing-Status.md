# Wiki Publishing Status

## 2026-09-29 변경 요청 생성 완료

사용자가 공식 기기 인증을 직접 승인한 뒤 [문서 정리 변경 요청 #1](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/1)을 초안으로 생성했다. 대상은 main이며 문서 브랜치와 위키 게시 상태를 검토할 수 있다. 변경 요청을 이 채팅에 첨부했고 병합은 수행하지 않았다. 아래 인증 갱신 대기·변경 요청 부재는 이전 시점의 기록이다.

## 2026-09-29 후속 관찰 구현 검증 반영

문서 브랜치와 위키에 사용자 승인 조회 함수의 정확한 후속 지문 승인, C3L 집중 11개와 동일 소스 필수 회귀 고유 613개, 최종 독립 검수·관찰 단위 수용 및 ADR-0036 단계적 수용 순서를 반영한다. R4 시간 초과 이력과 R6 현재 재실행을 구분한다. C3 전체 검증이나 Review C4 구현 완료로 표시하지 않는다. 변경 요청은 여전히 생성하지 못했으며 인증 갱신을 기다린다. 아래 개정 값은 이전 게시 이력이다.

중간 보완 문서 개정 `9c4014e1d448f398de9603fd59754fbad26434a9`와 위키 개정 `06cb91542b4699a8ec7e4654306169122326d20e`는 원격 게시를 완료했다. 최종 수용 상태는 별도 후속 개정으로 게시한다.

## 2026-09-28 게시 준비

이번 정리본은 `codex/documentation-checkpoint-20260928` 문서 브랜치를 원본으로 사용한다. 문서 개정 `4d3fd80079392368f9726e232467f7dd20b36f2a`는 원격 게시됐으며 변경 요청 생성은 인증 갱신 대기 상태다. 아직 병합 전이므로 계약 링크는 해당 브랜치로 연결한다. 아래 2026-08-24 게시 완료·개정 기록은 당시 이력이다.

위키 본문 게시 개정은 `8ea7ee9332779992ae3bf6b5f02d89cd260c7f61`이며 2026-09-28에 원격 게시 성공과 실제 최신 구현 상태 페이지를 확인했다. 독자 페이지 14개와 탐색 파일 1개를 게시했고 위키 내부 연결의 누락은 0개다. 본 기록과 권위 문서 링크 정정은 그 뒤 별도 게시 개정으로 남긴다. 위키 게시 자체는 게임 구현·시험 수용 증거가 아니다.

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
