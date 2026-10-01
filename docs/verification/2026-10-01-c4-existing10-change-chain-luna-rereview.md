# C4 기존 변경 10개 파일 사슬 계약 승인본 재검토

- Approved 계약: `docs/specs/work-contracts/2026-10-01-c4-existing10-change-chain.md`, SHA-256 `119470C78C66ACCCA7EF19518A57622948675D6B1801FD4CE3DFE37818A4FC15`.
- 비교 Draft: `docs/specs/work-contracts/2026-10-01-c4-existing10-change-chain-draft.md`, SHA-256 `6FB59AF732C0D26134AFA4BCE2CC0C79BE51AFB3F9715AAF8BA99E350BCE7B19`.
- 이전 설계 검토: `docs/verification/2026-10-01-c4-existing10-change-chain-luna-design-review.md`, SHA-256 `2D6727240F4D3A0A5CDED593ADB5A4E760079642D92BEC61226D0F98706D589C`.
- 기준 동결: v19 source manifest SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`; input list SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`.

## 판정

P0 0건, P1 0건. Draft와 Approved 본문을 대조한 결과 기존 설계의 정확 대상 10개 경로와 시작·끝 SHA, 전이별 승인·구현·독립 검토·동결 증거 규칙, 미입증 구간 보존, REQ/AC 추적, 비게시 경계는 유지됐다. 변경은 상태를 Approved로 전환하고 테라 산출 세 경로 및 루나 독립 결과 경로를 확정하며 Draft·선행 검토 SHA와 P0/P1 판정을 역사로 남기는 데 한정된다. 조사 범위나 파일 조건은 확대되지 않았다.

테라 JSON·설명·보고의 세 경로와 루나 결과 경로는 v19 입력·source manifest에 없고, 재검토 문서 경로도 작성 전에 존재하지 않았으며 v19 입력 밖이었다. 따라서 새 산출이 기존 v19 증거 집합에 순환 입력되지 않는다. 승인본은 읽기 전용 근거 대조와 테라 산출만 허용하고 Unity/QA/Git/원격, 게시, 클린 복제 권한은 주지 않는다.

선행 원장과 제한 수용의 상태는 변하지 않는다. 기존 변경 10개 파일의 이전→현재 사슬은 실제 승인·구현·독립 검토·동결 링크가 생성되고 확인되기 전까지 미폐쇄다. 앞선 22파일 사슬도 폐쇄 0·열림 22이며 게시 허용 목록은 계속 비어 있다. 신규 meta6·source/test6 및 초기 게시 공백, C3/C4 전체 수용은 별도다. 이번 문서는 계약 상태와 산출 경로의 독립 검토이며 실제 사슬 증거 산출·실행·게시 판정이 아니다.
